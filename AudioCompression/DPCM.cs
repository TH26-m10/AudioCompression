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
            public int SampleRate { get; set; }
            public int BitDepth { get; set; }
            public int Channels { get; set; }
            public int SampleCount { get; set; }
            public int QuantizationLevels { get; set; }
            public float MinDifference { get; set; }
            public float MaxDifference { get; set; }
            public float[] InitialSamples { get; set; }
            public string OriginalFormat { get; set; }
            public int OriginalBitRate { get; set; }
        }

        private static readonly byte[] MagicBytes = new byte[] { 0x44, 0x50 };

        public DPCM(CompressionSettings settings) : base(settings) { }

        public override string Compress(string inputFile)
        {
            string outputFile = Path.ChangeExtension(inputFile, ".dpcm");

            var processed = new AudioPreprocessor(_settings).Process(inputFile);
            float[] samples = processed.Samples;

            if (samples == null || samples.Length == 0)
            {
                throw new InvalidOperationException("No samples found.");
            }

            int channels = processed.Channels;
            int quantizationLevels = _settings.QuantizationLevels ?? 4;

            // Separate channels (deinterleave)
            float[][] channelSamples = new float[channels][];
            int samplesPerChannel = samples.Length / channels;

            for (int ch = 0; ch < channels; ch++)
            {
                channelSamples[ch] = new float[samplesPerChannel];
                for (int i = 0; i < samplesPerChannel; i++)
                {
                    channelSamples[ch][i] = samples[i * channels + ch];
                }
            }

            // Compute differences per channel
            float[][] channelDifferences = new float[channels][];
            float[] minDiffs = new float[channels];
            float[] maxDiffs = new float[channels];
            float[] initialSamples = new float[channels];

            for (int ch = 0; ch < channels; ch++)
            {
                float[] chSamples = channelSamples[ch];
                initialSamples[ch] = chSamples[0];

                channelDifferences[ch] = new float[chSamples.Length - 1];
                for (int i = 1; i < chSamples.Length; i++)
                {
                    channelDifferences[ch][i - 1] = chSamples[i] - chSamples[i - 1];
                }

                minDiffs[ch] = channelDifferences[ch].Min();
                maxDiffs[ch] = channelDifferences[ch].Max();
            }

            // Quantize all differences
            int totalDifferences = channelDifferences.Sum(d => d.Length);
            byte[] allQuantized = new byte[totalDifferences];
            int idx = 0;

            Stopwatch stopwatch = Stopwatch.StartNew();
            int reportInterval = Math.Max(1, totalDifferences / 50);
            int cancelInterval = Math.Max(1, totalDifferences / 200);

            for (int ch = 0; ch < channels; ch++)
            {


                float minDiff = minDiffs[ch];
                float maxDiff = maxDiffs[ch];
                float range = maxDiff - minDiff;

                for (int i = 0; i < channelDifferences[ch].Length; i++)
                {

                    if (i % cancelInterval == 0)
                        CancellationToken.ThrowIfCancellationRequested();

                    float normalized = range > 0
                        ? (channelDifferences[ch][i] - minDiff) / range
                        : 0f;

                    normalized = Math.Max(0f, Math.Min(1f, normalized));

                    allQuantized[idx] = (byte)Math.Round(normalized * (quantizationLevels - 1));
                    idx++;

                    if (idx % reportInterval == 0 || idx == totalDifferences)
                    {
                        float percentage = (float)idx / totalDifferences;
                        long elapsedMs = stopwatch.ElapsedMilliseconds;
                        float speed = elapsedMs > 0 ? idx / (elapsedMs / 1000f) : 0f;

                        ReportProgress(new CompressionProgress
                        {
                            Percentage = percentage,
                            ProcessingSpeed = speed,
                            CompressionRatio = percentage,
                            ElapsedMs = elapsedMs
                        });
                    }
                }
            }

            byte[] packedData = PackBits(allQuantized, quantizationLevels);

            int originalBitRate = 0;
            try
            {
                using (var file = TagLib.File.Create(inputFile))
                {
                    originalBitRate = file.Properties.AudioBitrate;
                }
            }
            catch { }

            DPCMHeader header = new DPCMHeader
            {
                SampleRate = processed.SampleRate,
                BitDepth = processed.BitDepth,
                Channels = channels,
                SampleCount = samplesPerChannel,
                QuantizationLevels = quantizationLevels,
                MinDifference = minDiffs[0],
                MaxDifference = maxDiffs[0],
                InitialSamples = initialSamples,
                OriginalFormat = Path.GetExtension(inputFile),
                OriginalBitRate = originalBitRate
            };

            using (var fs = new FileStream(outputFile, FileMode.Create))
            using (var writer = new BinaryWriter(fs))
            {
                WriteHeader(writer, header);
                writer.Write(packedData);
            }

            long originalSize = new FileInfo(inputFile).Length;
            long compressedSize = new FileInfo(outputFile).Length;
            float finalRatio = (float)compressedSize / originalSize;

            ReportProgress(new CompressionProgress
            {
                Percentage = 1f,
                ProcessingSpeed = 0,
                CompressionRatio = finalRatio,
                ElapsedMs = stopwatch.ElapsedMilliseconds
            });

            return outputFile;
        }

        public override string Decompress(string compressedFile)
        {
            if (!File.Exists(compressedFile))
                throw new FileNotFoundException("Compressed file not found.");

            DPCMHeader header;
            byte[] packedData;

            using (var fs = new FileStream(compressedFile, FileMode.Open))
            using (var reader = new BinaryReader(fs))
            {
                header = ReadHeader(reader);
                long remainingBytes = fs.Length - fs.Position;
                packedData = reader.ReadBytes((int)remainingBytes);
            }

            int totalDifferences = (header.SampleCount - 1) * header.Channels;
            byte[] quantized = UnpackBits(packedData, header.QuantizationLevels, totalDifferences);

            float[][] channelSamples = new float[header.Channels][];
            int idx = 0;

            for (int ch = 0; ch < header.Channels; ch++)
            {
                channelSamples[ch] = new float[header.SampleCount];
                channelSamples[ch][0] = header.InitialSamples[ch];

                float minDiff = header.MinDifference;
                float maxDiff = header.MaxDifference;
                float range = maxDiff - minDiff;

                for (int i = 1; i < header.SampleCount; i++)
                {
                    float normalized = (float)quantized[idx] / (header.QuantizationLevels - 1);
                    float diff = minDiff + (normalized * range);
                    channelSamples[ch][i] = channelSamples[ch][i - 1] + diff;
                    idx++;
                }
            }

            // Interleave channels back
            float[] samples = new float[header.SampleCount * header.Channels];
            for (int i = 0; i < header.SampleCount; i++)
            {
                for (int ch = 0; ch < header.Channels; ch++)
                {
                    samples[i * header.Channels + ch] = channelSamples[ch][i];
                }
            }

            // Apply clipping only at final output
            for (int i = 0; i < samples.Length; i++)
            {
                samples[i] = Math.Max(-1f, Math.Min(1f, samples[i]));
            }

            string outputFile = Path.Combine(
                Path.GetDirectoryName(compressedFile),
                Path.GetFileNameWithoutExtension(compressedFile) + "_decoded.wav");

            WriteWavFile(outputFile, samples, header.SampleRate, header.Channels, header.BitDepth);

            string originalFormat = header.OriginalFormat ?? ".wav";
            string finalOutput = ConvertFromWav(outputFile, originalFormat, header.BitDepth, header.OriginalBitRate);

            return finalOutput;
        }

        private byte[] PackBits(byte[] data, int levels)
        {
            int bitsPerValue = (int)Math.Ceiling(Math.Log(levels, 2));

            if (bitsPerValue >= 8)
                return data;

            int mask = (1 << bitsPerValue) - 1;
            int packedLength = (data.Length * bitsPerValue + 7) / 8;
            byte[] packed = new byte[packedLength];

            int bitPosition = 0;

            for (int i = 0; i < data.Length; i++)
            {
                int byteIndex = bitPosition / 8;
                int bitOffset = bitPosition % 8;

                int value = data[i] & mask;

                packed[byteIndex] |= (byte)(value << bitOffset);

                int overflowBits = bitOffset + bitsPerValue - 8;
                if (overflowBits > 0 && byteIndex + 1 < packed.Length)
                {
                    packed[byteIndex + 1] |= (byte)(value >> (bitsPerValue - overflowBits));
                }

                bitPosition += bitsPerValue;
            }

            return packed;
        }

        private byte[] UnpackBits(byte[] packed, int levels, int originalLength)
        {
            int bitsPerValue = (int)Math.Ceiling(Math.Log(levels, 2));

            if (bitsPerValue >= 8)
                return packed;

            int mask = (1 << bitsPerValue) - 1;
            byte[] unpacked = new byte[originalLength];

            int bitPosition = 0;

            for (int i = 0; i < originalLength; i++)
            {
                int byteIndex = bitPosition / 8;
                int bitOffset = bitPosition % 8;

                int value = packed[byteIndex] >> bitOffset;

                int bitsAvailable = 8 - bitOffset;
                if (bitsAvailable < bitsPerValue && byteIndex + 1 < packed.Length)
                {
                    int bitsFromNext = bitsPerValue - bitsAvailable;
                    value |= (packed[byteIndex + 1] & ((1 << bitsFromNext) - 1)) << bitsAvailable;
                }

                unpacked[i] = (byte)(value & mask);
                bitPosition += bitsPerValue;
            }

            return unpacked;
        }

        private void WriteHeader(BinaryWriter writer, DPCMHeader header)
        {
            writer.Write(MagicBytes);
            writer.Write(header.SampleRate);
            writer.Write(header.BitDepth);
            writer.Write(header.Channels);
            writer.Write(header.SampleCount);
            writer.Write(header.QuantizationLevels);
            writer.Write(header.MinDifference);
            writer.Write(header.MaxDifference);

            for (int i = 0; i < header.Channels; i++)
            {
                writer.Write(header.InitialSamples[i]);
            }

            writer.Write(header.OriginalFormat);
            writer.Write(header.OriginalBitRate);
        }

        private DPCMHeader ReadHeader(BinaryReader reader)
        {
            byte[] magic = reader.ReadBytes(2);
            if (magic[0] != MagicBytes[0] || magic[1] != MagicBytes[1])
                throw new InvalidDataException("Invalid DPCM file.");

            var header = new DPCMHeader
            {
                SampleRate = reader.ReadInt32(),
                BitDepth = reader.ReadInt32(),
                Channels = reader.ReadInt32(),
                SampleCount = reader.ReadInt32(),
                QuantizationLevels = reader.ReadInt32(),
                MinDifference = reader.ReadSingle(),
                MaxDifference = reader.ReadSingle()
            };

            header.InitialSamples = new float[header.Channels];
            for (int i = 0; i < header.Channels; i++)
            {
                header.InitialSamples[i] = reader.ReadSingle();
            }

            header.OriginalFormat = reader.ReadString();
            header.OriginalBitRate = reader.ReadInt32();

            return header;
        }
    }
}