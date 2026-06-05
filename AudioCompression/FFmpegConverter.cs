using System;
using System.Diagnostics;
using System.IO;

namespace AudioCompression
{
    public static class FFmpegConverter
    {
        // Check if FFmpeg is available on this machine
        private static readonly string FFmpegPath = @"C:\ffmpeg\ffmpeg-8.1.1-essentials_build\bin\ffmpeg.exe";

        public static bool IsAvailable()
        {
            try
            {
                return File.Exists(FFmpegPath);
            }
            catch
            {
                return false;
            }
        }

        // Convert any file to any format
        // FFmpeg figures out the format from the file extension automatically
        public static void Convert(string inputFile, string outputFile)
        {
            Convert(inputFile, outputFile, "");
        }

        // Overload with quality settings
        // Overload with quality settings
        public static void Convert(string inputFile, string outputFile, string extraArgs)
        {
            if (!File.Exists(inputFile))
                throw new FileNotFoundException($"Input file not found: {inputFile}");

            if (!File.Exists(FFmpegPath))
                throw new FileNotFoundException($"FFmpeg not found at: {FFmpegPath}");

            // Build arguments with format flag BEFORE output file
            string arguments = string.IsNullOrEmpty(extraArgs)
                ? $"-y -i \"{inputFile}\" \"{outputFile}\""
                : $"-y -i \"{inputFile}\" {extraArgs} \"{outputFile}\"";

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = FFmpegPath,
                    Arguments = arguments,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };

            process.Start();
            string ffmpegLog = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0 || !File.Exists(outputFile))
                throw new Exception($"FFmpeg conversion failed (exit code: {process.ExitCode}).\n\nCommand: ffmpeg {arguments}\n\nFFmpeg output:\n{ffmpegLog}");
        }
    }
}