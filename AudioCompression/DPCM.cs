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
            public float InitialSample { get; set; }
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
                throw new InvalidOperationException("No samples found.");

            int quantizationLevels = _settings.QuantizationLevels ?? 4;

            float[] differences = new float[samples.Length - 1];
            for (int i = 1; i < samples.Length; i++)
            {
                differences[i - 1] = samples[i] - samples[i - 1];
            }

            float minDiff = differences.Min();
            float maxDiff = differences.Max();

            
            byte[] quantized = new byte[differences.Length];
            for (int i = 0; i < differences.Length; i++)
            {
                float normalized = (differences[i] - minDiff) / (maxDiff - minDiff);
                normalized = Math.Max(0f, Math.Min(1f, normalized));
                quantized[i] = (byte)(int)Math.Round(normalized * (quantizationLevels - 1));
            }


            byte[] packedData = PackBits(quantized, quantizationLevels);

            int originalBitRate = 0;
            try
            {
                var file = TagLib.File.Create(inputFile);
                originalBitRate = file.Properties.AudioBitrate;
            }
            catch { }

            DPCMHeader header = new DPCMHeader
            {
                SampleRate = processed.SampleRate,
                BitDepth = processed.BitDepth,
                Channels = processed.Channels,
                SampleCount = samples.Length,
                QuantizationLevels = quantizationLevels,
                MinDifference = minDiff,
                MaxDifference = maxDiff,
                InitialSample = samples[0],
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
            float ratio = (float)compressedSize / originalSize;

            ReportProgress(new CompressionProgress
            {
                Percentage = 1.0f,
                ProcessingSpeed = 0,
                CompressionRatio = ratio,
                ElapsedMs = 0
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

           
            byte[] quantized = UnpackBits(packedData, header.QuantizationLevels, header.SampleCount - 1);

            float[] differences = new float[quantized.Length];
            for (int i = 0; i < quantized.Length; i++)
            {
                float normalized = (float)quantized[i] / (header.QuantizationLevels - 1);
                differences[i] = header.MinDifference + (normalized * (header.MaxDifference - header.MinDifference));
            }
           float[] samples = new float[header.SampleCount];
            samples[0] = header.InitialSample;

            for (int i = 1; i < header.SampleCount; i++)
            {
                samples[i] = samples[i - 1] + differences[i - 1];
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

            int valuesPerByte = 8 / bitsPerValue;
            int mask = (1 << bitsPerValue) - 1;

            int packedLength = (data.Length + valuesPerByte - 1) / valuesPerByte;
            byte[] packed = new byte[packedLength];

            for (int i = 0; i < data.Length; i++)
            {
                int byteIndex = i / valuesPerByte;
                int bitOffset = (i % valuesPerByte) * bitsPerValue;

         
                int shift = 8 - bitsPerValue - bitOffset;
                packed[byteIndex] |= (byte)((data[i] & mask) << shift);
            }

            return packed;
        }

        private byte[] UnpackBits(byte[] packed, int levels, int originalLength)
        {
            int bitsPerValue = (int)Math.Ceiling(Math.Log(levels, 2));

            if (bitsPerValue >= 8)
                return packed;

            int valuesPerByte = 8 / bitsPerValue;
            int mask = (1 << bitsPerValue) - 1;

            byte[] unpacked = new byte[originalLength];

            for (int i = 0; i < originalLength; i++)
            {
                int byteIndex = i / valuesPerByte;
                int bitOffset = (i % valuesPerByte) * bitsPerValue;

                int shift = 8 - bitsPerValue - bitOffset;
                unpacked[i] = (byte)((packed[byteIndex] >> shift) & mask);
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
            writer.Write(header.InitialSample);
            writer.Write(header.OriginalFormat);
            writer.Write(header.OriginalBitRate);
        }

        private DPCMHeader ReadHeader(BinaryReader reader)
        {
            byte[] magic = reader.ReadBytes(2);
            if (magic[0] != MagicBytes[0] || magic[1] != MagicBytes[1])
                throw new InvalidDataException("Invalid DPCM file.");

            return new DPCMHeader
            {
                SampleRate = reader.ReadInt32(),
                BitDepth = reader.ReadInt32(),
                Channels = reader.ReadInt32(),
                SampleCount = reader.ReadInt32(),
                QuantizationLevels = reader.ReadInt32(),
                MinDifference = reader.ReadSingle(),
                MaxDifference = reader.ReadSingle(),
                InitialSample = reader.ReadSingle(),
                OriginalFormat = reader.ReadString(),
                OriginalBitRate = reader.ReadInt32()
            };
        }
    }
}