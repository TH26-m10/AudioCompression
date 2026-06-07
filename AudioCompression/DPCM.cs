using NAudio.Wave;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace AudioCompression
{
    public class DPCM : CompressionAlgorithmBase
    {
        private class DPCMHeader
        {
            // Audio info
            public int SampleRate { get; set; }

            public int BitDepth { get; set; }

            public int Channels { get; set; }

            public int SampleCount { get; set; }

            // DPCM settings
            public int QuantizationLevels { get; set; }

            public float InitialSample { get; set; }

            // Original file info
            public string OriginalFormat { get; set; }

            public int OriginalBitRate { get; set; }
        }

        private static readonly byte[] MagicBytes =
            new byte[] { 0x44, 0x50 }; // "DP"

        public DPCM(CompressionSettings settings)
            : base(settings)
        {
        }

        public override string Compress(string inputFile)
        {
            string outputFile =
                Path.ChangeExtension(inputFile, ".dpcm");

            var processed =
                new AudioPreprocessor(_settings).Process(inputFile);

            float[] samples = processed.Samples;

            if (samples == null || samples.Length == 0)
                throw new InvalidOperationException(
                    "No samples found.");
            int quantizationLevels = _settings.QuantizationLevels ?? 16;


            byte[] encodedData =
                Encode(samples, quantizationLevels);

            int originalBitRate = 0;

            try
            {
                var file = TagLib.File.Create(inputFile);
                originalBitRate = file.Properties.AudioBitrate;
            }
            catch
            {
            }

            DPCMHeader header = new DPCMHeader
            {
                SampleRate = processed.SampleRate,
                BitDepth = processed.BitDepth,
                Channels = processed.Channels,
                SampleCount = samples.Length,
                QuantizationLevels = quantizationLevels,
                InitialSample = samples[0],
                OriginalFormat = Path.GetExtension(inputFile),
                OriginalBitRate = originalBitRate
            };

            using (var fs =
                new FileStream(outputFile, FileMode.Create))
            using (var writer = new BinaryWriter(fs))
            {
                WriteHeader(writer, header);

                writer.Write(encodedData);
            }

            return outputFile;
        }

        public override string Decompress(string compressedFile)
        {
            if (!File.Exists(compressedFile))
            {
                throw new FileNotFoundException(
                    "Compressed file not found.");
            }

            DPCMHeader header;

            byte[] encodedData;

            using (var fs =
                new FileStream(compressedFile, FileMode.Open))
            using (var reader = new BinaryReader(fs))
            {
                header = ReadHeader(reader);

                long remainingBytes =
                    fs.Length - fs.Position;

                encodedData =
                    reader.ReadBytes((int)remainingBytes);
            }

            float[] samples =
                Decode(
                    encodedData,
                    header.SampleCount,
                    header.InitialSample,
                    header.QuantizationLevels);

            string outputFile =
                Path.Combine(
                    Path.GetDirectoryName(compressedFile),
                    Path.GetFileNameWithoutExtension(compressedFile)
                    + "_decoded.wav");

            WriteWavFile(
                outputFile,
                samples,
                header.SampleRate,
                header.Channels,
                header.BitDepth);

            return outputFile;
        }

        private byte[] Encode(
            float[] samples,
            int quantizationLevels)
        {
            int sampleCount = samples.Length;

            byte[] encoded =
                new byte[sampleCount - 1];

            float previousSample = samples[0];

            float maxDifference = 2f;

            Stopwatch stopwatch =
                Stopwatch.StartNew();

            int reportInterval =
                Math.Max(1, sampleCount / 50);

            for (int i = 1; i < sampleCount; i++)
            {
                if (i % reportInterval == 0)
                {
                    CancellationToken
                        .ThrowIfCancellationRequested();
                }

                float currentSample =
                    Math.Max(-1f,
                    Math.Min(1f, samples[i]));

                float difference =
                    currentSample - previousSample;

                float normalized =
                    (difference + maxDifference)
                    / (2f * maxDifference);

                normalized =
                    Math.Max(0f,
                    Math.Min(1f, normalized));

                int quantized =
                    (int)Math.Round(
                        normalized
                        * (quantizationLevels - 1));

                encoded[i - 1] =
                    (byte)quantized;

                previousSample =
                    currentSample;

                if (i % reportInterval == 0
                    || i == sampleCount - 1)
                {
                    float percentage =
                        (float)(i + 1)
                        / sampleCount;

                    long elapsedMs =
                        stopwatch.ElapsedMilliseconds;

                    float speed =
                        elapsedMs > 0
                        ? (i + 1)
                        / (elapsedMs / 1000f)
                        : 0f;

                    float ratio =
                        (float)encoded.Length
                        / (sampleCount * sizeof(float));

                    ReportProgress(
                        new CompressionProgress
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

        private float[] Decode(
            byte[] encodedData,
            int sampleCount,
            float initialSample,
            int quantizationLevels)
        {
            float[] samples =
                new float[sampleCount];

            samples[0] = initialSample;

            float previousSample =
                initialSample;

            float maxDifference = 2f;

            for (int i = 1; i < sampleCount; i++)
            {
                int quantized =
                    encodedData[i - 1];

                float normalized =
                    (float)quantized
                    / (quantizationLevels - 1);

                float difference =
                    (normalized
                    * 2f
                    * maxDifference)
                    - maxDifference;

                float currentSample =
                    previousSample + difference;

                currentSample =
                    Math.Max(-1f,
                    Math.Min(1f, currentSample));

                samples[i] =
                    currentSample;

                previousSample =
                    currentSample;
            }

            return samples;
        }

        private void WriteHeader(
            BinaryWriter writer,
            DPCMHeader header)
        {
            writer.Write(MagicBytes);

            writer.Write(header.SampleRate);

            writer.Write(header.BitDepth);

            writer.Write(header.Channels);

            writer.Write(header.SampleCount);

            writer.Write(header.QuantizationLevels);

            writer.Write(header.InitialSample);

            writer.Write(header.OriginalFormat);

            writer.Write(header.OriginalBitRate);
        }

        private DPCMHeader ReadHeader(
            BinaryReader reader)
        {
            byte[] magic =
                reader.ReadBytes(2);

            if (magic[0] != MagicBytes[0]
                || magic[1] != MagicBytes[1])
            {
                throw new InvalidDataException(
                    "Invalid DPCM file.");
            }

            return new DPCMHeader
            {
                SampleRate = reader.ReadInt32(),

                BitDepth = reader.ReadInt32(),

                Channels = reader.ReadInt32(),

                SampleCount = reader.ReadInt32(),

                QuantizationLevels = reader.ReadInt32(),

                InitialSample = reader.ReadSingle(),

                OriginalFormat = reader.ReadString(),

                OriginalBitRate = reader.ReadInt32()
            };
        }
    }
}