using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AudioCompression
{
    public class AdaptiveDeltaModulation : CompressionAlgorithmBase
    {
        private class ADMHeader
        {
            // base
            public int SampleRate { get; set; }
            public int BitDepth { get; set; }
            public int Channels { get; set; }
            public int BitRate { get; set; }

            // ADM specific
            public int SampleCount { get; set; }
            public float InitialStepSize { get; set; }
            public float MinStepSize { get; set; }
            public float MaxStepSize { get; set; }
            public float StepMultiplier { get; set; }
            public float StepDecay { get; set; }
            public int BitHistoryLength { get; set; }
            public string OriginalFormat { get; set; }
            public int OriginalBitRate { get; set; }
        }

        private static readonly byte[] MagicBytes = new byte[] { 0x41, 0x44 };

        public AdaptiveDeltaModulation(CompressionSettings settings) : base(settings)
        {
        }

        public override string Compress(string inputFile)
        {
            string outputFile = Path.ChangeExtension(inputFile, ".adm");

            var processed = new AudioPreprocessor(_settings).Process(inputFile);
            float[] samples = processed.Samples;

            if (samples == null || samples.Length == 0)
                throw new InvalidOperationException("Preprocessor returned no samples.");

            long originalSize = new FileInfo(inputFile).Length;

         
            float rms = (float)Math.Sqrt(samples.Average(s => s * s));
            float initialStep = Math.Max(_settings.MinStepSize, Math.Min(rms * 0.1f, _settings.MaxStepSize));

            byte[] encodedData = Encode(samples, initialStep, processed.Channels, originalSize);

            int bitRate = processed.SampleRate * processed.Channels;
            int originalBitRate = GetFileBitRate(inputFile);

            ADMHeader header = new ADMHeader
            {
                SampleRate = processed.SampleRate,
                BitDepth = processed.BitDepth,
                Channels = processed.Channels,
                BitRate = bitRate,
                SampleCount = samples.Length,
                InitialStepSize = initialStep,
                MinStepSize = _settings.MinStepSize,
                MaxStepSize = _settings.MaxStepSize,
                StepMultiplier = _settings.StepMultiplier,
                StepDecay = _settings.StepDecay,
                BitHistoryLength = _settings.BitHistoryLength,
                OriginalFormat = Path.GetExtension(inputFile).ToLowerInvariant(),
                OriginalBitRate = originalBitRate
            };

            using (var fs = new FileStream(outputFile, FileMode.Create))
            using (var writer = new BinaryWriter(fs))
            {
                WriteHeader(writer, header);
                writer.Write(encodedData);
            }

            return outputFile;
        }

        public override string Decompress(string admFile)
        {
            if (!File.Exists(admFile))
                throw new FileNotFoundException($"ADM file not found: {admFile}");

            ADMHeader header;
            byte[] encodedData;
            using (var fs = new FileStream(admFile, FileMode.Open))
            using (var reader = new BinaryReader(fs))
            {
                header = ReadHeader(reader);

                long remaining = fs.Length - fs.Position;
                if (remaining > int.MaxValue)
                    throw new InvalidOperationException("ADM file too large to decompress in one pass.");

                encodedData = reader.ReadBytes((int)remaining);
            }

            float[] samples = Decode(encodedData, header.SampleCount, header.Channels, header);

         
            string tempWav = Path.Combine(
                Path.GetDirectoryName(admFile),
                Path.GetFileNameWithoutExtension(admFile) + "_decoded.wav");

            WriteWavFile(tempWav, samples, header.SampleRate, header.Channels, header.BitDepth);

           
            string originalFormat = header.OriginalFormat ?? ".wav";
            string finalOutput = ConvertFromWav(tempWav, originalFormat, header.BitDepth, header.OriginalBitRate);

     
            if (!finalOutput.Equals(tempWav, StringComparison.OrdinalIgnoreCase) && File.Exists(tempWav))
                File.Delete(tempWav);

            return finalOutput;
        }

        private int GetFileBitRate(string inputFile)
        {
            try
            {
                using (var reader = new AudioFileReader(inputFile))
                {
                    var fileInfo = new FileInfo(inputFile);
                    long fileSizeBytes = fileInfo.Length;
                    double durationSeconds = reader.TotalTime.TotalSeconds;

                    if (durationSeconds > 0)
                    {
                        int bitRateKbps = (int)(fileSizeBytes * 8 / durationSeconds / 1000);
                        bitRateKbps = (bitRateKbps / 8) * 8;
                        return bitRateKbps > 0 ? bitRateKbps : 128;
                    }
                    return 128;
                }
            }
            catch
            {
                return 128;
            }
        }

 
        private byte[] Encode(float[] samples, float initialStep, int channels, long originalFileSizeBytes)
        {
            int sampleCount = samples.Length;
            int byteCount = (sampleCount + 7) / 8;
            byte[] encoded = new byte[byteCount];

    
            float[] predicted = new float[channels];
            float[] stepSize = new float[channels];
            Queue<bool>[] bitHistory = new Queue<bool>[channels];

            for (int ch = 0; ch < channels; ch++)
            {
                stepSize[ch] = initialStep;
                bitHistory[ch] = new Queue<bool>(_settings.BitHistoryLength);
            }

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            int reportInterval = Math.Max(1, sampleCount / 50);

            for (int i = 0; i < sampleCount; i++)
            {
                if (i % reportInterval == 0)
                    CancellationToken.ThrowIfCancellationRequested();

                int channel = i % channels;
                float currentSample = Math.Max(-1f, Math.Min(1f, samples[i]));

                bool bit;
                if (currentSample >= predicted[channel])
                {
                    bit = true;
                    predicted[channel] += stepSize[channel];
                }
                else
                {
                    bit = false;
                    predicted[channel] -= stepSize[channel];
                }

           
                predicted[channel] = Math.Max(-1f, Math.Min(1f, predicted[channel]));

                int byteIndex = i / 8;
                int bitIndex = i % 8;
                if (bit)
                    encoded[byteIndex] |= (byte)(1 << bitIndex);

                var history = bitHistory[channel];
                history.Enqueue(bit);
                if (history.Count > _settings.BitHistoryLength)
                    history.Dequeue();

                if (history.Count == _settings.BitHistoryLength && history.All(b => b == bit))
                {
                    stepSize[channel] *= _settings.StepMultiplier;
                }
                else
                {
                 
                    stepSize[channel] *= _settings.StepDecay;
                }

         
                stepSize[channel] = Math.Max(_settings.MinStepSize, Math.Min(_settings.MaxStepSize, stepSize[channel]));

               
                if (i % reportInterval == 0 || i == sampleCount - 1)
                {
                    float percentage = (float)(i + 1) / sampleCount;
                    long elapsedMs = stopwatch.ElapsedMilliseconds;
                    float speed = elapsedMs > 0 ? (i + 1) / (elapsedMs / 1000f) : 0f;
                    float bytesProduced = (i + 1) / 8f;
                    float ratio = originalFileSizeBytes > 0
                        ? bytesProduced / originalFileSizeBytes
                        : 0f;

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

        private float[] Decode(byte[] encodedData, int sampleCount, int channels, ADMHeader header)
        {
            float[] samples = new float[sampleCount];

            float[] predicted = new float[channels];
            float[] stepSize = new float[channels];
            Queue<bool>[] bitHistory = new Queue<bool>[channels];

            for (int ch = 0; ch < channels; ch++)
            {
                stepSize[ch] = header.InitialStepSize;
                bitHistory[ch] = new Queue<bool>(header.BitHistoryLength);
            }

            for (int i = 0; i < sampleCount; i++)
            {
                int channel = i % channels;
                int byteIndex = i / 8;
                int bitIndex = i % 8;

                bool bit = false;
                if (byteIndex < encodedData.Length)
                    bit = ((encodedData[byteIndex] >> bitIndex) & 1) == 1;

                if (bit)
                    predicted[channel] += stepSize[channel];
                else
                    predicted[channel] -= stepSize[channel];

                predicted[channel] = Math.Max(-1f, Math.Min(1f, predicted[channel]));
                samples[i] = predicted[channel];

           
                var history = bitHistory[channel];
                history.Enqueue(bit);
                if (history.Count > header.BitHistoryLength)
                    history.Dequeue();

                if (history.Count == header.BitHistoryLength && history.All(b => b == bit))
                {
                    stepSize[channel] *= header.StepMultiplier;
                }
                else
                {
                    stepSize[channel] *= header.StepDecay;
                }

                stepSize[channel] = Math.Max(header.MinStepSize, Math.Min(header.MaxStepSize, stepSize[channel]));
            }

            return samples;
        }

        private void WriteHeader(BinaryWriter writer, ADMHeader header)
        {
            writer.Write(MagicBytes);
            writer.Write(header.SampleRate);
            writer.Write(header.BitDepth);
            writer.Write(header.Channels);
            writer.Write(header.BitRate);
            writer.Write(header.SampleCount);
            writer.Write(header.InitialStepSize);
            writer.Write(header.MinStepSize);
            writer.Write(header.MaxStepSize);
            writer.Write(header.StepMultiplier);
            writer.Write(header.StepDecay);
            writer.Write(header.BitHistoryLength);
            writer.Write(header.OriginalBitRate);

      
            string format = (header.OriginalFormat ?? ".wav").PadRight(20).Substring(0, 20);
            writer.Write(format.ToCharArray());
        }

        private ADMHeader ReadHeader(BinaryReader reader)
        {
            byte[] magic = reader.ReadBytes(2);
            if (magic[0] != MagicBytes[0] || magic[1] != MagicBytes[1])
                throw new InvalidDataException("Not a valid ADM file — magic bytes mismatch.");

            var header = new ADMHeader
            {
                SampleRate = reader.ReadInt32(),
                BitDepth = reader.ReadInt32(),
                Channels = reader.ReadInt32(),
                BitRate = reader.ReadInt32(),
                SampleCount = reader.ReadInt32(),
                InitialStepSize = reader.ReadSingle(),
                MinStepSize = reader.ReadSingle(),
                MaxStepSize = reader.ReadSingle(),
                StepMultiplier = reader.ReadSingle(),
                StepDecay = reader.ReadSingle(),
                BitHistoryLength = reader.ReadInt32(),
                OriginalBitRate = reader.ReadInt32(),
                OriginalFormat = new string(reader.ReadChars(20)).Trim()
            };

            return header;
        }
    }
}