using System.Windows.Forms;
using NAudio.Wave;
using System.IO;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

namespace AudioCompression
{
    public partial class Form1 : Form
    {
        private string originalFilePath;
        private string compressedFilePath;
        private string decompressedFilePath;
        private AudioFileReader audioFile;
        private WaveOutEvent outputDevice;
        private CompressionSettings settings;
        private CancellationTokenSource _cancellationSource;
        private string reportText;

        public Form1()
        {
            InitializeComponent();
            settings = new CompressionSettings();
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Title = "Select an Audio File";
                dialog.Filter = "Audio Files|*.mp3;*.wav;*.aac;*.flac;*.m4a;*.ogg";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    LoadAudioFile(dialog.FileName, true);
                    progressBar.Value = 0;
                    lblProgress.Text = "0%";
                    chartPerformance.Series["Speed"].Points.Clear();
                    chartPerformance.Series["Ratio"].Points.Clear();
                    btnSaveCompressed.Enabled = false;
                    btnSaveDecompressed.Enabled = false;
                    btnDecompress.Enabled = false;
                    btnShowReport.Enabled = false;
                    reportText = null;
                }
            }
        }

        private void panelDropZone_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop)
                ? DragDropEffects.Copy
                : DragDropEffects.None;
        }

        private void panelDropZone_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length > 0)
            {
                try
                {
                    LoadAudioFile(files[0], true);
                    progressBar.Value = 0;
                    lblProgress.Text = "0%";
                    chartPerformance.Series["Speed"].Points.Clear();
                    chartPerformance.Series["Ratio"].Points.Clear();
                    btnSaveCompressed.Enabled = false;
                    btnSaveDecompressed.Enabled = false;
                    btnDecompress.Enabled = false;
                    btnShowReport.Enabled = false;
                    reportText = null;
                }
                catch
                {
                    MessageBox.Show("The selected file is not a valid Audio.");
                }
            }
        }

        private void LoadAudioFile(string filePath, bool isOriginal = false)
        {
            try
            {
                outputDevice?.Stop();
                outputDevice?.Dispose();
                audioFile?.Dispose();

                audioFile = new AudioFileReader(filePath);
                outputDevice = new WaveOutEvent();
                outputDevice.Init(audioFile);

                if (string.IsNullOrEmpty(originalFilePath) || isOriginal)
                {
                    originalFilePath = filePath;
                }

                lblMessage.Text = $"Audio File: \n{Path.GetFileName(filePath)}";
                DisplayAudioInfo(filePath);

                string label = isOriginal ? "Original" : "Decompressed";
                PlotWaveform(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading audio:\n" + ex.Message);
            }
        }

        private void DisplayAudioInfo(string filePath)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(filePath);

                // Always use AudioFileReader for duration - it's more reliable
                using (var reader = new AudioFileReader(filePath))
                {
                    lblFileSize.Text = (fileInfo.Length / 1024.0 / 1024.0).ToString("0.00") + " MB";
                    lblFileType.Text = fileInfo.Extension.ToUpper();
                    lblDuration.Text = reader.TotalTime.ToString(@"hh\:mm\:ss");
                    lblSampleRate.Text = (reader.WaveFormat.SampleRate / 1000) + " KHz";
                    lblChannels.Text = reader.WaveFormat.Channels.ToString();
                }

                // Get bitrate from TagLib if possible
                try
                {
                    var file = TagLib.File.Create(filePath);
                    lblBitRate.Text = file.Properties.AudioBitrate + " kbps";
                    lblEncoding.Text = file.Properties.Description;
                }
                catch
                {
                    lblBitRate.Text = "N/A";
                    lblEncoding.Text = "Audio";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading audio info:\n" + ex.Message);
            }
        }

        private void PlotWaveform(string filePath)
        {
            Task.Run(() =>
            {
                var sampleList = new List<float>();
                int sampleRate = 0;

                using (var reader = new AudioFileReader(filePath))
                {
                    sampleRate = reader.WaveFormat.SampleRate;

                    ISampleProvider provider = reader.WaveFormat.Channels == 2
                        ? (ISampleProvider)new NAudio.Wave.SampleProviders.StereoToMonoSampleProvider(reader)
                        : reader;

                    float[] buffer = new float[8192];
                    int read;
                    while ((read = provider.Read(buffer, 0, buffer.Length)) > 0)
                        for (int i = 0; i < read; i++)
                            sampleList.Add(buffer[i]);
                }

                float[] samples = sampleList.ToArray();

                if (samples.Length == 0) return;

                var model = new OxyPlot.PlotModel
                {
                    Background = OxyPlot.OxyColor.FromRgb(30, 30, 30),
                    PlotAreaBorderColor = OxyPlot.OxyColor.FromRgb(80, 80, 80)
                };

                model.Axes.Add(new OxyPlot.Axes.LinearAxis
                {
                    Position = OxyPlot.Axes.AxisPosition.Bottom,
                    Title = "Time (s)",
                    IsAxisVisible = false,
                    TextColor = OxyPlot.OxyColors.LightGray,
                    TicklineColor = OxyPlot.OxyColors.LightGray,
                    MajorGridlineStyle = OxyPlot.LineStyle.Dot,
                    MajorGridlineColor = OxyPlot.OxyColor.FromRgb(60, 60, 60)
                });

                model.Axes.Add(new OxyPlot.Axes.LinearAxis
                {
                    Position = OxyPlot.Axes.AxisPosition.Left,
                    Title = "Amplitude",
                    Minimum = -1.0,
                    Maximum = 1.0,
                    IsAxisVisible=false,
                    TextColor = OxyPlot.OxyColors.LightGray,
                    TicklineColor = OxyPlot.OxyColors.LightGray,
                    MajorGridlineStyle = OxyPlot.LineStyle.Dot,
                    MajorGridlineColor = OxyPlot.OxyColor.FromRgb(60, 60, 60)
                });

                var series = new OxyPlot.Series.LineSeries
                {
                    Color = OxyPlot.OxyColor.FromRgb(0, 162, 255),
                    StrokeThickness = 1,
                };

                int displayPoints = 4000;
                int step = Math.Max(1, samples.Length / displayPoints);

                for (int i = 0; i < samples.Length; i += step)
                {
                    double timeSeconds = (double)i / sampleRate;
                    series.Points.Add(new OxyPlot.DataPoint(timeSeconds, samples[i]));
                }

                model.Series.Add(series);

                Invoke((Action)(() => plotAudio.Model = model));
            });
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            outputDevice?.Play();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (audioFile != null && outputDevice != null)
            {
                outputDevice.Stop();
                audioFile.Position = 0;
            }
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            outputDevice?.Pause();
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            using (var settingsForm = new SettingsForm(settings))
            {
                if (settingsForm.ShowDialog() == DialogResult.OK)
                {
                    settings = settingsForm.Settings;
                    
                }
            }
        }

        private async void btnCompress_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(originalFilePath))
            {
                MessageBox.Show("Load an audio file first.");
                return;
            }

            outputDevice?.Stop();
            outputDevice?.Dispose();
            outputDevice = null;
            audioFile?.Dispose();
            audioFile = null;

            btnCompress.Enabled = false;
            btnDecompress.Enabled = false;
            btnUpload.Enabled = false;
            btnCancel.Enabled = true;  

            progressBar.Value = 0;
            lblProgress.Text = "0%";
            chartPerformance.Series["Speed"].Points.Clear();
            chartPerformance.Series["Ratio"].Points.Clear();

            // نعمل توكن مشان إيقاف الضغط, يكون جديد لكل عملية ضغط
            _cancellationSource = new CancellationTokenSource();

            Stopwatch sw = Stopwatch.StartNew();

            try
            {
                CompressionAlgorithmBase algorithm = CompressionFactory.Create(settings);
                algorithm.SetCancellationToken(_cancellationSource.Token);

                algorithm.ProgressChanged += progress =>
                {
                    Invoke((Action)(() =>
                    {
                        int percentage = (int)(progress.Percentage * 100);
                        progressBar.Value = percentage;
                        lblProgress.Text = $"{percentage}%";

                        double timeSeconds = progress.ElapsedMs / 1000.0;
                        chartPerformance.Series["Speed"].Points.AddXY(
                            timeSeconds, progress.ProcessingSpeed / 1000.0);
                        chartPerformance.Series["Ratio"].Points.AddXY(
                            timeSeconds, progress.CompressionRatio * 100);
                    }));
                };

                compressedFilePath = await Task.Run(
                    () => algorithm.Compress(originalFilePath),
                    _cancellationSource.Token);

                sw.Stop();
                //progressBar.Value = 100;
                //lblProgress.Text = "100%";

                GenerateReport(sw.ElapsedMilliseconds);
                MessageBox.Show("Compression completed.\n\n" + compressedFilePath);

                btnDecompress.Enabled = true;
                btnSaveCompressed.Enabled = true;
                btnShowReport.Enabled = true;
            }
            catch (OperationCanceledException)
            {
                sw.Stop();

                if (!string.IsNullOrEmpty(compressedFilePath) && File.Exists(compressedFilePath))
                {
                    try { File.Delete(compressedFilePath); } catch { }
                    compressedFilePath = null;
                }

                MessageBox.Show("Compression cancelled.");

                progressBar.Value = 0;
                lblProgress.Text = "0%";
                chartPerformance.Series["Speed"].Points.Clear();
                chartPerformance.Series["Ratio"].Points.Clear();
            }
            catch (Exception ex)
            {
                sw.Stop();
                MessageBox.Show("Compression failed:\n" + ex.Message);
            }
            finally
            {
                btnCompress.Enabled = true;
                btnUpload.Enabled = true;
                btnCancel.Enabled = false;

                _cancellationSource?.Dispose();
                _cancellationSource = null;
            }
        }
        
        private void btnDecompress_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(compressedFilePath))
            {
                MessageBox.Show("Compress a file first.");
                return;
            }

            try
            {
                CompressionAlgorithmBase algorithm = CompressionFactory.Create(settings);
                decompressedFilePath = algorithm.Decompress(compressedFilePath);

                LoadAudioFile(decompressedFilePath);
                MessageBox.Show("Decompression completed.");
                btnSaveDecompressed.Enabled = true;
            }
            catch (NotImplementedException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Decompression failed:\n" + ex.Message);
            }
        }

        private void btnSaveCompressed_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(compressedFilePath))
            {
                MessageBox.Show("No compressed file exists.");
                return;
            }

            outputDevice?.Stop();
            outputDevice?.Dispose();
            outputDevice = null;
            audioFile?.Dispose();
            audioFile = null;

            using (var dialog = new SaveFileDialog())
            {

                dialog.Filter = "Compressed Audio (*.dm)|*.dm";
                dialog.FileName = Path.GetFileName(compressedFilePath);

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    File.Copy(compressedFilePath, dialog.FileName, true);
                    MessageBox.Show("Compressed file saved.");
                }
            }
        }

        /*private void btnSaveDecompressed_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(decompressedFilePath))
            {
                MessageBox.Show("No decompressed file exists.");
                return;
            }

            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "Wave File (*.wav)|*.wav";
                dialog.FileName = Path.GetFileName(decompressedFilePath);

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    File.Copy(decompressedFilePath, dialog.FileName, true);
                    MessageBox.Show("Decompressed file saved.");
                }
            }
        }*/
        private void btnSaveDecompressed_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(decompressedFilePath))
            {
                MessageBox.Show("No decompressed file exists.");
                return;
            }

            // Detect the actual format from the decompressed file
            string ext = Path.GetExtension(decompressedFilePath).ToLowerInvariant();

            // Build filter string based on actual format
            string filter;
            switch (ext)
            {
                case ".mp3":
                    filter = "MP3 Audio (*.mp3)|*.mp3";
                    break;
                case ".aac":
                    filter = "AAC Audio (*.aac)|*.aac";
                    break;
                case ".flac":
                    filter = "FLAC Audio (*.flac)|*.flac";
                    break;
                case ".m4a":
                    filter = "M4A Audio (*.m4a)|*.m4a";
                    break;
                case ".wma":
                    filter = "WMA Audio (*.wma)|*.wma";
                    break;
                case ".aiff":
                case ".aif":
                    filter = "AIFF Audio (*.aiff)|*.aiff";
                    break;
                case ".wav":
                    filter = "Wave File (*.wav)|*.wav";
                    break;
                default:
                    filter = "All Files (*.*)|*.*";
                    break;
            }

            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = filter;
                dialog.FileName = Path.GetFileName(decompressedFilePath);

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    File.Copy(decompressedFilePath, dialog.FileName, true);
                    MessageBox.Show("Decompressed file saved.");
                }
            }
        }

        private void GenerateReport(long elapsedMs)
        {
            FileInfo original = new FileInfo(originalFilePath);
            FileInfo compressed = new FileInfo(compressedFilePath);

            double reductionPercent = original.Length > 0
                ? (1.0 - ((double)compressed.Length / original.Length)) * 100.0
                : 0.0;

            int reportSampleRate = 0;
            int reportBitDepth = 0;
            int reportChannels = 0;
            int reportBitRate = 0;
            float reportStepSize = 0f; // DM

            // ADM
            float reportInitialStepSize = 0f;
            float reportMinStepSize = 0f;
            float reportMaxStepSize = 0f;
            float reportStepMultiplier = 0f;
            float reportStepDecay = 0f;
            int reportBitHistoryLength = 0;

            // DPCM
            int reportQuantizationLevels = 0;

            try
            {
                using (var fs = new FileStream(compressedFilePath, FileMode.Open))
                using (var reader = new BinaryReader(fs))
                {
                
                    reader.ReadBytes(2);

             
                    reportSampleRate = reader.ReadInt32();
                    reportBitDepth = reader.ReadInt32();
                    reportChannels = reader.ReadInt32();

                    switch (settings.Algorithm)
                    {
                        case CompressionAlgorithm.DeltaModulation:
                            reader.ReadInt32(); // SampleCount
                            reportStepSize = reader.ReadSingle();
                            break;

                        case CompressionAlgorithm.AdaptiveDeltaModulation:
                            reader.ReadInt32(); // SampleCount
                            reportInitialStepSize = reader.ReadSingle();
                            reportMinStepSize = reader.ReadSingle();
                            reportMaxStepSize = reader.ReadSingle();
                            reportStepMultiplier = reader.ReadSingle();
                            reportStepDecay = reader.ReadSingle();
                            reportBitHistoryLength = reader.ReadInt32();
                            break;

                        case CompressionAlgorithm.DPCM:
                        
                            reader.ReadInt32();                          
                            reportQuantizationLevels = reader.ReadInt32();
                            reader.ReadSingle();                       
                            reader.ReadSingle();                         
                            reader.ReadSingle();                         
                            reader.ReadString();                       
                            reportBitRate = reader.ReadInt32();          
                            break;
                    }
                }
            }
            catch { }

            reportText =
                "Compression Report\n\n" +
                $"Original Size: {original.Length / 1024.0:F2} KB\n" +
                $"Compressed Size: {compressed.Length / 1024.0:F2} KB\n" +
                $"Size Reduction: {reductionPercent:F2}%\n" +
                $"Compression Time: {elapsedMs} ms\n\n" +
                $"Algorithm: {settings.Algorithm}\n" +
                $"Sample Rate: {reportSampleRate} Hz\n" +
                //$"Encoded Bit Rate: {reportBitRate / 1000} kbps\n" +
                $"Bit Depth: {reportBitDepth}\n" +
                $"Channels: {reportChannels}\n";

            switch (settings.Algorithm)
            {
                case CompressionAlgorithm.DeltaModulation:
                    reportText += $"Step Size: {reportStepSize}\n";
                    break;

                case CompressionAlgorithm.DPCM:
                    reportText += $"Quantization Levels: {reportQuantizationLevels}\n";
                    break;

                case CompressionAlgorithm.NonlinearQuantization:
                    break;
                case CompressionAlgorithm.AdaptiveDeltaModulation:
                    reportText += $"CVSD Parameters:\n";
                    reportText += $"  Initial Step Size: {reportInitialStepSize}\n";
                    reportText += $"  Min Step Size: {reportMinStepSize}\n";
                    reportText += $"  Max Step Size: {reportMaxStepSize}\n";
                    reportText += $"  Step Multiplier: {reportStepMultiplier}\n";
                    reportText += $"  Step Decay: {reportStepDecay}\n";
                    reportText += $"  Bit History Length: {reportBitHistoryLength}\n";
                    break;
            }
        }
        private void btnShowReport_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(reportText))
            {
                MessageBox.Show("No report available.");
                return;
            }

            MessageBox.Show(reportText, "Compression Report");
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _cancellationSource?.Cancel();
            btnCancel.Enabled = false;
            lblProgress.Text = "Cancelling...";
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(originalFilePath))
            {
                MessageBox.Show("No original file to reset to.");
                return;
            }

            if (!string.IsNullOrEmpty(compressedFilePath) && File.Exists(compressedFilePath))
            {
                try { File.Delete(compressedFilePath); } catch { }
                compressedFilePath = null;
            }

            if (!string.IsNullOrEmpty(decompressedFilePath) && File.Exists(decompressedFilePath))
            {
                try { File.Delete(decompressedFilePath); } catch { }
                decompressedFilePath = null;
            }

            LoadAudioFile(originalFilePath);
            progressBar.Value = 0;
            lblProgress.Text = "0%";
            chartPerformance.Series["Speed"].Points.Clear();
            chartPerformance.Series["Ratio"].Points.Clear();
            btnSaveCompressed.Enabled = false;
            btnSaveDecompressed.Enabled = false;
            btnDecompress.Enabled = false;
            btnShowReport.Enabled = false;
            reportText = null;
            MessageBox.Show("Reset to original file.");
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            outputDevice?.Stop();
            outputDevice?.Dispose();
            audioFile?.Dispose();

            // Clean up temp compressed file
            if (!string.IsNullOrEmpty(compressedFilePath) && File.Exists(compressedFilePath))
            {
                try { File.Delete(compressedFilePath); } catch { }
            }

            if (!string.IsNullOrEmpty(decompressedFilePath) && File.Exists(decompressedFilePath))
            {
                try { File.Delete(decompressedFilePath); } catch { }
                decompressedFilePath = null;
            }

            base.OnFormClosing(e);
        }

    }
}