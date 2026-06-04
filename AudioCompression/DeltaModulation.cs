using NAudio.Wave;
using System;
using System.IO;
using System.Linq;

namespace AudioCompression
{
    public class DeltaModulation : CompressionAlgorithmBase
    {
        private class DMHeader
        {
            // base
            public int SampleRate { get; set; }
            public int BitDepth { get; set; }
            public int Channels { get; set; }
            public int BitRate { get; set; }
            
            // DM specific
            public int SampleCount { get; set; }
            public float StepSize { get; set; }
        }

        private static readonly byte[] MagicBytes = new byte[] { 0x44, 0x4D }; // "DM"

        public DeltaModulation(CompressionSettings settings) : base(settings)
        {
        }

        /*public override string Compress(string inputFile)
        {
            string outputFile = Path.ChangeExtension(inputFile, ".dm");

            AudioPreprocessor processor = new AudioPreprocessor(_settings);
            ProcessedAudio processed = processor.Process(inputFile);
            float[] samples = processed.Samples;

            if (samples == null || samples.Length == 0)
                throw new InvalidOperationException("Preprocessor returned no samples.");

            float rms = (float)Math.Sqrt(samples.Average(s => s * s));
            float stepSize = Math.Max(1e-6f, Math.Min(rms * 0.5f, 0.5f));

            byte[] encodedData = Encode(samples, stepSize, processed.Channels);

            int bitRate = processed.SampleRate * processed.Channels;

            DMHeader header = new DMHeader
            {
                SampleRate = processed.SampleRate,
                BitDepth = processed.BitDepth,
                Channels = processed.Channels,
                BitRate = bitRate,
                SampleCount = samples.Length,
                StepSize = stepSize
            };

            using (var fs = new FileStream(outputFile, FileMode.Create))
            using (var writer = new BinaryWriter(fs))
            {
                WriteHeader(writer, header);
                writer.Write(encodedData);
            }

            return outputFile;
        }*/

        public override string Compress(string inputFile)
        {
            string outputFile = Path.ChangeExtension(inputFile, ".dm");

            var processed = new AudioPreprocessor(_settings).Process(inputFile);
            float[] samples = processed.Samples;

            if (samples == null || samples.Length == 0)
                throw new InvalidOperationException("Preprocessor returned no samples.");

            float rms = (float)Math.Sqrt(samples.Average(s => s * s));
            float stepSize = Math.Max(1e-6f, Math.Min(rms * 0.5f, 0.5f));

            // predicted state lives here — captured by the lambda
            float[] predicted = new float[processed.Channels];

            // Pass only the per-sample logic — loop runs in base class
            byte[] encodedData = RunEncodeLoop(samples, processed.Channels,
                (sample, channel) =>
                {
                    bool bit;
                    if (sample >= predicted[channel])
                    {
                        bit = true;
                        predicted[channel] += stepSize;
                    }
                    else
                    {
                        bit = false;
                        predicted[channel] -= stepSize;
                    }

                    predicted[channel] = Math.Max(-1f, Math.Min(1f, predicted[channel]));
                    return bit;
                });

            int bitRate = processed.SampleRate * processed.Channels;

            DMHeader header = new DMHeader
            {
                SampleRate = processed.SampleRate,
                BitDepth = processed.BitDepth,
                Channels = processed.Channels,
                BitRate = bitRate,
                SampleCount = samples.Length,
                StepSize = stepSize
            };

            using (var fs = new FileStream(outputFile, FileMode.Create))
            using (var writer = new BinaryWriter(fs))
            {
                WriteHeader(writer, header);
                writer.Write(encodedData);
            }

            return outputFile;
        }
        /*public override string Compress(string inputFile)
        {
            string outputFile = Path.ChangeExtension(inputFile, ".dm");

            AudioPreprocessor processor = new AudioPreprocessor(_settings);
            ProcessedAudio processed = processor.Process(inputFile);
            float[] samples = processed.Samples;

            if (samples == null || samples.Length == 0)
                throw new InvalidOperationException("Preprocessor returned no samples. Check the input file.");

            float rms = (float)Math.Sqrt(samples.Average(s => s * s));
            float stepSize = Math.Max(1e-6f, Math.Min(rms * 0.5f, 0.5f));

            byte[] encodedData = Encode(samples, stepSize, processed.Channels);

            int bitRate = processed.SampleRate * processed.Channels;

            DMHeader header = new DMHeader
            {
                SampleRate = processed.SampleRate,
                BitDepth = processed.BitDepth,
                Channels = processed.Channels,
                BitRate = bitRate,
                SampleCount = samples.Length,
                StepSize = stepSize
            };

            using (var fs = new FileStream(outputFile, FileMode.Create))
            using (var writer = new BinaryWriter(fs))
            {
                WriteHeader(writer, header);
                writer.Write(encodedData);
            }

            return outputFile;
        }*/

        public override string Decompress(string dmFile)
        {
            if (!File.Exists(dmFile))
                throw new FileNotFoundException($"DM file not found: {dmFile}");

            string outputFile = Path.Combine(
                Path.GetDirectoryName(dmFile),
                Path.GetFileNameWithoutExtension(dmFile) + "_decoded.wav");

            DMHeader header;
            byte[] encodedData;
            using (var fs = new FileStream(dmFile, FileMode.Open))
            using (var reader = new BinaryReader(fs))
            {
                header = ReadHeader(reader);

                long remaining = fs.Length - fs.Position;
                if (remaining > int.MaxValue)
                    throw new InvalidOperationException("DM file too large to decompress in one pass.");

                encodedData = reader.ReadBytes((int)remaining);
            }
            float[] samples = Decode(encodedData, header.SampleCount, header.StepSize, header.Channels);

            WriteWavFile(outputFile, samples, header.SampleRate, header.Channels, header.BitDepth);

            return outputFile;
        }

        private byte[] Encode(float[] samples, float stepSize, int channels)
        {
            int sampleCount = samples.Length;
            int byteCount = (sampleCount + 7) / 8;
            byte[] encoded = new byte[byteCount];
            float[] predicted = new float[channels];

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            int reportInterval = Math.Max(1, sampleCount / 50);

            for (int i = 0; i < sampleCount; i++)
            {
                // Check for cancellation every reportInterval samples
                // same frequency as progress reporting — no extra overhead
                if (i % reportInterval == 0)
                    CancellationToken.ThrowIfCancellationRequested();

                int channel = i % channels;
                float currentSample = Math.Max(-1f, Math.Min(1f, samples[i]));

                bool bit;
                if (currentSample >= predicted[channel])
                {
                    bit = true;
                    predicted[channel] += stepSize;
                }
                else
                {
                    bit = false;
                    predicted[channel] -= stepSize;
                }

                predicted[channel] = Math.Max(-1f, Math.Min(1f, predicted[channel]));

                if (bit)
                {
                    int byteIndex = i / 8;
                    int bitIndex = i % 8;
                    encoded[byteIndex] |= (byte)(1 << bitIndex);
                }

                if (i % reportInterval == 0 || i == sampleCount - 1)
                {
                    float percentage = (float)(i + 1) / sampleCount;
                    long elapsedMs = stopwatch.ElapsedMilliseconds;
                    float speed = elapsedMs > 0 ? (i + 1) / (elapsedMs / 1000f) : 0f;
                    float ratio = 1f / 32f;

                    ReportProgress(new CompressionProgress
                    {
                        Percentage = percentage,
                        ProcessingSpeed = speed,
                        CompressionRatio = ratio,
                        ElapsedMs = elapsedMs
                    });
                }
            }

            return encoded;
        }
        
        private float[] Decode(byte[] encodedData, int sampleCount, float stepSize, int channels)
        {
            float[] samples = new float[sampleCount];
            float[] predicted = new float[channels];

            for (int i = 0; i < sampleCount; i++)
            {
                int channel = i % channels;
                int byteIndex = i / 8;
                int bitIndex = i % 8;

                bool bit = false;
                if (byteIndex < encodedData.Length)
                    bit = ((encodedData[byteIndex] >> bitIndex) & 1) == 1;

                if (bit)
                    predicted[channel] += stepSize;
                else
                    predicted[channel] -= stepSize;

                predicted[channel] = Math.Max(-1f, Math.Min(1f, predicted[channel]));
                samples[i] = predicted[channel];
            }

            return samples;
        }

        private void WriteHeader(BinaryWriter writer, DMHeader header)
        {
            writer.Write(MagicBytes);
            writer.Write(header.SampleRate);
            writer.Write(header.BitDepth);
            writer.Write(header.Channels);
            writer.Write(header.BitRate);
            writer.Write(header.SampleCount);
            writer.Write(header.StepSize);
        }

        private DMHeader ReadHeader(BinaryReader reader)
        {
            byte[] magic = reader.ReadBytes(2);
            if (magic[0] != MagicBytes[0] || magic[1] != MagicBytes[1])
                throw new InvalidDataException("Not a valid DM file — magic bytes mismatch.");

            return new DMHeader
            {
                SampleRate = reader.ReadInt32(),
                BitDepth = reader.ReadInt32(),
                Channels = reader.ReadInt32(),
                BitRate = reader.ReadInt32(),
                SampleCount = reader.ReadInt32(),
                StepSize = reader.ReadSingle()
            };
        }
    }
}