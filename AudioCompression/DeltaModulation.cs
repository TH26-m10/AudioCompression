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
            public string OriginalFormat { get; set; }
            public int OriginalBitRate { get; set; }  // Store original bitrate
        }

        private static readonly byte[] MagicBytes = new byte[] { 0x44, 0x4D }; // "DM"

        public DeltaModulation(CompressionSettings settings) : base(settings)
        {
        }

        public override string Compress(string inputFile)
        {
            string outputFile = Path.ChangeExtension(inputFile, ".dm");

            // عملنا معالجة مسبقة
            var processed = new AudioPreprocessor(_settings).Process(inputFile);
            float[] samples = processed.Samples;

            if (samples == null || samples.Length == 0)
                throw new InvalidOperationException("Preprocessor returned no samples.");

            //
            float rms = (float)Math.Sqrt(samples.Average(s => s * s));
            float stepSize = Math.Max(1e-6f, Math.Min(rms * 0.5f, 0.5f));

            long originalSize = new FileInfo(inputFile).Length;
            byte[] encodedData = Encode(samples, stepSize, processed.Channels, originalSize);

            int bitRate = processed.SampleRate * processed.Channels;
            int originalBitRate = GetFileBitRate(inputFile);

            DMHeader header = new DMHeader
            {
                SampleRate = processed.SampleRate,
                BitDepth = processed.BitDepth,
                Channels = processed.Channels,
                BitRate = bitRate,
                SampleCount = samples.Length,
                StepSize = stepSize,
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

        public override string Decompress(string dmFile)
        {
            if (!File.Exists(dmFile))
                throw new FileNotFoundException($"DM file not found: {dmFile}");

            // نقرأ معلومات الملف
            DMHeader header;
            byte[] encodedData;
            using (var fs = new FileStream(dmFile, FileMode.Open))
            using (var reader = new BinaryReader(fs))
            {
                header = ReadHeader(reader);

                // الباقي هو عبارة عبارة عن باقي الملفبعد قراءة ال هيدر
                long remaining = fs.Length - fs.Position;
                if (remaining > int.MaxValue)
                    throw new InvalidOperationException("DM file too large to decompress in one pass.");

                encodedData = reader.ReadBytes((int)remaining);
            }

            float[] samples = Decode(encodedData, header.SampleCount, header.StepSize, header.Channels);

            // اول شي منحول لل wav بقيمة عمق البت الأصلية
            string tempWav = Path.Combine(
                Path.GetDirectoryName(dmFile),
                Path.GetFileNameWithoutExtension(dmFile) + "_decoded.wav");

            // عم نبعت المتغيرات ال 3 هدول مشان بعض الصيغ بحاجتون ليشتغلو
            WriteWavFile(tempWav, samples, header.SampleRate, header.Channels, header.BitDepth);

            string originalFormat = header.OriginalFormat ?? ".wav";
            string finalOutput = ConvertFromWav(tempWav, originalFormat, header.BitDepth, header.OriginalBitRate);

            // احذف ملف ال wav اللي تشكل إذا كنت حولت لصيغة تانية
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

                        // Round to nearest common bitrate (8 kbps increments)
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

        private byte[] Encode(float[] samples, float stepSize, int channels, long originalFileSizeBytes)
        {
            int sampleCount = samples.Length;
            int byteCount = (sampleCount + 7) / 8;
            byte[] encoded = new byte[byteCount];
            float[] predicted = new float[channels];

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            // هي يعني تقريبا حيتم تحديث الواجهة 50 مرة
            int reportInterval = Math.Max(1, sampleCount / 50);
            int cancelInterval = Math.Max(1, sampleCount / 200);

            for (int i = 0; i < sampleCount; i++)
            {
                if (i % cancelInterval == 0)
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
                    // عملية ال >> هي عملية شفت يسار لل 1 بمقدار قيمة المتغير
                    // 1 => 0000_0001 => 1 << bitIndex(1) => 0000_0010 => 2
                    // ونحول لبايت لأنو نتيجة الشفت هي int

                    // عملية |= هي عملية "أو" مع مساواة هدفها تغير قيمة البت المحدد بدون ما تغير قيمة الباقي
                    encoded[byteIndex] |= (byte)(1 << bitIndex);
                }

                if (i % reportInterval == 0 || i == sampleCount - 1)
                {
                    float percentage = (float)(i + 1) / sampleCount;
                    long elapsedMs = stopwatch.ElapsedMilliseconds;
                    float speed = elapsedMs > 0 ? (i + 1) / (elapsedMs / 1000f) : 0f;
                    float bytesProduced = (i + 1) / 8f;
                    float ratio = originalFileSizeBytes > 0 ? bytesProduced / originalFileSizeBytes : 0f;
                    /*originalFileSizeBytes > 0
                    ? bytesProduced / originalFileSizeBytes
                    : 0f;*/

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
                // أي بايت انا عندو هلا
                int byteIndex = i / 8; 
                // اي بت ضمن البايت انا عندو هلا
                int bitIndex = i % 8;

                bool bit = false;
                // الشرط لاتأكد انو ما طلعت برا الفايل
                if (byteIndex < encodedData.Length)
                    // هلا عملية قراءة كل بت وشوف قيمتو بعمل شفت لليمين
                    // 1011_0010 => >> bitIndex(1) => 0101_1001
                    // بعدا ال & بتعمل ماسك لكل البتات عدا أقصى اليمين
                    // 0101_1001 & 1 => 0000_0001
                    // بعدا نقارن مع ال 1 ونشوف إذا القيمة الأساسية كانت 1 او 0
                    bit = ((encodedData[byteIndex] >> bitIndex) & 1) == 1;

                if (bit)
                    predicted[channel] += stepSize;
                else
                    predicted[channel] -= stepSize;

                // نرجع للمجال من -1 لل 1
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
            writer.Write(header.OriginalBitRate);

            // Write format as fixed-length string (max 20 chars)
            string format = (header.OriginalFormat ?? ".wav").PadRight(20).Substring(0, 20);
            writer.Write(format.ToCharArray());
        }

        private DMHeader ReadHeader(BinaryReader reader)
        {
            byte[] magic = reader.ReadBytes(2);
            if (magic[0] != MagicBytes[0] || magic[1] != MagicBytes[1])
                throw new InvalidDataException("Not a valid DM file — magic bytes mismatch.");

            var header = new DMHeader
            {
                SampleRate = reader.ReadInt32(),
                BitDepth = reader.ReadInt32(),
                Channels = reader.ReadInt32(),
                BitRate = reader.ReadInt32(),
                SampleCount = reader.ReadInt32(),
                StepSize = reader.ReadSingle(),
                OriginalBitRate = reader.ReadInt32(),
                OriginalFormat = new string(reader.ReadChars(20)).Trim()
            };

            return header;
        }
    }
}