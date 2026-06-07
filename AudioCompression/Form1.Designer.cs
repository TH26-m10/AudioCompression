
namespace AudioCompression
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnPause = new System.Windows.Forms.Button();
            this.btnPlay = new System.Windows.Forms.Button();
            this.btnUpload = new System.Windows.Forms.Button();
            this.panelDropZone = new System.Windows.Forms.Panel();
            this.plotAudio = new OxyPlot.WindowsForms.PlotView();
            this.lblMessage = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lblFileSize = new System.Windows.Forms.Label();
            this.lblFileType = new System.Windows.Forms.Label();
            this.lblDuration = new System.Windows.Forms.Label();
            this.lblBitRate = new System.Windows.Forms.Label();
            this.lblSampleRate = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.lblChannels = new System.Windows.Forms.Label();
            this.lblEncoding = new System.Windows.Forms.Label();
            this.btnSettings = new System.Windows.Forms.Button();
            this.btnCompress = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnDecompress = new System.Windows.Forms.Button();
            this.btnSaveCompressed = new System.Windows.Forms.Button();
            this.btnSaveDecompressed = new System.Windows.Forms.Button();
            this.btnShowReport = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.chartPerformance = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.lblProgress = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.panelDropZone.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartPerformance)).BeginInit();
            this.SuspendLayout();
            // 
            // btnStop
            // 
            this.btnStop.Font = new System.Drawing.Font("Tahoma", 8F);
            this.btnStop.Location = new System.Drawing.Point(662, 107);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(173, 40);
            this.btnStop.TabIndex = 0;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnPause
            // 
            this.btnPause.Font = new System.Drawing.Font("Tahoma", 8F);
            this.btnPause.Location = new System.Drawing.Point(230, 107);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(217, 40);
            this.btnPause.TabIndex = 1;
            this.btnPause.Text = "Pause";
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // btnPlay
            // 
            this.btnPlay.Font = new System.Drawing.Font("Tahoma", 8F);
            this.btnPlay.Location = new System.Drawing.Point(456, 107);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(198, 40);
            this.btnPlay.TabIndex = 2;
            this.btnPlay.Text = "Play";
            this.btnPlay.UseVisualStyleBackColor = true;
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            // 
            // btnUpload
            // 
            this.btnUpload.Font = new System.Drawing.Font("Tahoma", 8F);
            this.btnUpload.Location = new System.Drawing.Point(11, 107);
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.Size = new System.Drawing.Size(213, 40);
            this.btnUpload.TabIndex = 3;
            this.btnUpload.Text = "Upload";
            this.btnUpload.UseVisualStyleBackColor = true;
            this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
            // 
            // panelDropZone
            // 
            this.panelDropZone.AllowDrop = true;
            this.panelDropZone.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelDropZone.Controls.Add(this.btnPlay);
            this.panelDropZone.Controls.Add(this.plotAudio);
            this.panelDropZone.Controls.Add(this.btnStop);
            this.panelDropZone.Controls.Add(this.btnPause);
            this.panelDropZone.Controls.Add(this.btnUpload);
            this.panelDropZone.Location = new System.Drawing.Point(12, 35);
            this.panelDropZone.Name = "panelDropZone";
            this.panelDropZone.Size = new System.Drawing.Size(848, 167);
            this.panelDropZone.TabIndex = 4;
            this.panelDropZone.DragDrop += new System.Windows.Forms.DragEventHandler(this.panelDropZone_DragDrop);
            this.panelDropZone.DragEnter += new System.Windows.Forms.DragEventHandler(this.panelDropZone_DragEnter);
            // 
            // plotAudio
            // 
            this.plotAudio.Location = new System.Drawing.Point(11, 3);
            this.plotAudio.Name = "plotAudio";
            this.plotAudio.PanCursor = System.Windows.Forms.Cursors.Hand;
            this.plotAudio.Size = new System.Drawing.Size(824, 101);
            this.plotAudio.TabIndex = 18;
            this.plotAudio.Text = "plotView1";
            this.plotAudio.ZoomHorizontalCursor = System.Windows.Forms.Cursors.SizeWE;
            this.plotAudio.ZoomRectangleCursor = System.Windows.Forms.Cursors.SizeNWSE;
            this.plotAudio.ZoomVerticalCursor = System.Windows.Forms.Cursors.SizeNS;
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Font = new System.Drawing.Font("Tahoma", 8F);
            this.lblMessage.Location = new System.Drawing.Point(12, 9);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(331, 17);
            this.lblMessage.TabIndex = 6;
            this.lblMessage.Text = "Drag an audio file here or upload with Upload button.";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(12, 205);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 17);
            this.label1.TabIndex = 5;
            this.label1.Text = "Details";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label6, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.lblFileSize, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblFileType, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblDuration, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblBitRate, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.lblSampleRate, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.label8, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.label9, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.lblChannels, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.lblEncoding, 1, 6);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(16, 226);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(310, 240);
            this.tableLayoutPanel1.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 8F);
            this.label2.Location = new System.Drawing.Point(48, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 17);
            this.label2.TabIndex = 0;
            this.label2.Text = "File Size:";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 8F);
            this.label3.Location = new System.Drawing.Point(44, 42);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 17);
            this.label3.TabIndex = 1;
            this.label3.Text = "File Type:";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 8F);
            this.label4.Location = new System.Drawing.Point(44, 76);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(66, 17);
            this.label4.TabIndex = 2;
            this.label4.Text = "Duration:";
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 8F);
            this.label5.Location = new System.Drawing.Point(47, 110);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 17);
            this.label5.TabIndex = 3;
            this.label5.Text = "Bit Rate:";
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Tahoma", 8F);
            this.label6.Location = new System.Drawing.Point(33, 144);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(89, 17);
            this.label6.TabIndex = 4;
            this.label6.Text = "Sample Rate:";
            // 
            // lblFileSize
            // 
            this.lblFileSize.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblFileSize.AutoSize = true;
            this.lblFileSize.Font = new System.Drawing.Font("Tahoma", 8F);
            this.lblFileSize.Location = new System.Drawing.Point(232, 8);
            this.lblFileSize.Name = "lblFileSize";
            this.lblFileSize.Size = new System.Drawing.Size(0, 17);
            this.lblFileSize.TabIndex = 8;
            // 
            // lblFileType
            // 
            this.lblFileType.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblFileType.AutoSize = true;
            this.lblFileType.Font = new System.Drawing.Font("Tahoma", 8F);
            this.lblFileType.Location = new System.Drawing.Point(232, 42);
            this.lblFileType.Name = "lblFileType";
            this.lblFileType.Size = new System.Drawing.Size(0, 17);
            this.lblFileType.TabIndex = 9;
            // 
            // lblDuration
            // 
            this.lblDuration.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblDuration.AutoSize = true;
            this.lblDuration.Font = new System.Drawing.Font("Tahoma", 8F);
            this.lblDuration.Location = new System.Drawing.Point(232, 76);
            this.lblDuration.Name = "lblDuration";
            this.lblDuration.Size = new System.Drawing.Size(0, 17);
            this.lblDuration.TabIndex = 10;
            // 
            // lblBitRate
            // 
            this.lblBitRate.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblBitRate.AutoSize = true;
            this.lblBitRate.Font = new System.Drawing.Font("Tahoma", 8F);
            this.lblBitRate.Location = new System.Drawing.Point(232, 110);
            this.lblBitRate.Name = "lblBitRate";
            this.lblBitRate.Size = new System.Drawing.Size(0, 17);
            this.lblBitRate.TabIndex = 11;
            // 
            // lblSampleRate
            // 
            this.lblSampleRate.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblSampleRate.AutoSize = true;
            this.lblSampleRate.Font = new System.Drawing.Font("Tahoma", 8F);
            this.lblSampleRate.Location = new System.Drawing.Point(232, 144);
            this.lblSampleRate.Name = "lblSampleRate";
            this.lblSampleRate.Size = new System.Drawing.Size(0, 17);
            this.lblSampleRate.TabIndex = 12;
            // 
            // label8
            // 
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Tahoma", 8F);
            this.label8.Location = new System.Drawing.Point(43, 178);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(68, 17);
            this.label8.TabIndex = 6;
            this.label8.Text = "Channels:";
            // 
            // label9
            // 
            this.label9.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Tahoma", 8F);
            this.label9.Location = new System.Drawing.Point(42, 213);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(70, 17);
            this.label9.TabIndex = 7;
            this.label9.Text = "Encoding:";
            // 
            // lblChannels
            // 
            this.lblChannels.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblChannels.AutoSize = true;
            this.lblChannels.Font = new System.Drawing.Font("Tahoma", 8F);
            this.lblChannels.Location = new System.Drawing.Point(232, 178);
            this.lblChannels.Name = "lblChannels";
            this.lblChannels.Size = new System.Drawing.Size(0, 17);
            this.lblChannels.TabIndex = 14;
            // 
            // lblEncoding
            // 
            this.lblEncoding.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblEncoding.AutoSize = true;
            this.lblEncoding.Font = new System.Drawing.Font("Tahoma", 8F);
            this.lblEncoding.Location = new System.Drawing.Point(232, 213);
            this.lblEncoding.Name = "lblEncoding";
            this.lblEncoding.Size = new System.Drawing.Size(0, 17);
            this.lblEncoding.TabIndex = 15;
            // 
            // btnSettings
            // 
            this.btnSettings.Location = new System.Drawing.Point(346, 225);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(510, 35);
            this.btnSettings.TabIndex = 7;
            this.btnSettings.Text = "Settings";
            this.btnSettings.UseVisualStyleBackColor = true;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // btnCompress
            // 
            this.btnCompress.Location = new System.Drawing.Point(346, 266);
            this.btnCompress.Name = "btnCompress";
            this.btnCompress.Size = new System.Drawing.Size(254, 35);
            this.btnCompress.TabIndex = 8;
            this.btnCompress.Text = "Compress";
            this.btnCompress.UseVisualStyleBackColor = true;
            this.btnCompress.Click += new System.EventHandler(this.btnCompress_Click);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(347, 430);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(509, 35);
            this.btnReset.TabIndex = 9;
            this.btnReset.Text = "Reset File";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnDecompress
            // 
            this.btnDecompress.Enabled = false;
            this.btnDecompress.Location = new System.Drawing.Point(346, 307);
            this.btnDecompress.Name = "btnDecompress";
            this.btnDecompress.Size = new System.Drawing.Size(253, 35);
            this.btnDecompress.TabIndex = 10;
            this.btnDecompress.Text = "Decompress";
            this.btnDecompress.UseVisualStyleBackColor = true;
            this.btnDecompress.Click += new System.EventHandler(this.btnDecompress_Click);
            // 
            // btnSaveCompressed
            // 
            this.btnSaveCompressed.Enabled = false;
            this.btnSaveCompressed.Location = new System.Drawing.Point(606, 266);
            this.btnSaveCompressed.Name = "btnSaveCompressed";
            this.btnSaveCompressed.Size = new System.Drawing.Size(250, 35);
            this.btnSaveCompressed.TabIndex = 11;
            this.btnSaveCompressed.Text = "Save compressed";
            this.btnSaveCompressed.UseVisualStyleBackColor = true;
            this.btnSaveCompressed.Click += new System.EventHandler(this.btnSaveCompressed_Click);
            // 
            // btnSaveDecompressed
            // 
            this.btnSaveDecompressed.Enabled = false;
            this.btnSaveDecompressed.Location = new System.Drawing.Point(606, 307);
            this.btnSaveDecompressed.Name = "btnSaveDecompressed";
            this.btnSaveDecompressed.Size = new System.Drawing.Size(250, 35);
            this.btnSaveDecompressed.TabIndex = 12;
            this.btnSaveDecompressed.Text = "Save decompressed";
            this.btnSaveDecompressed.UseVisualStyleBackColor = true;
            this.btnSaveDecompressed.Click += new System.EventHandler(this.btnSaveDecompressed_Click);
            // 
            // btnShowReport
            // 
            this.btnShowReport.Enabled = false;
            this.btnShowReport.Location = new System.Drawing.Point(347, 348);
            this.btnShowReport.Name = "btnShowReport";
            this.btnShowReport.Size = new System.Drawing.Size(510, 35);
            this.btnShowReport.TabIndex = 13;
            this.btnShowReport.Text = "Show Report";
            this.btnShowReport.UseVisualStyleBackColor = true;
            this.btnShowReport.Click += new System.EventHandler(this.btnShowReport_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(15, 477);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(796, 23);
            this.progressBar.TabIndex = 14;
            // 
            // chartPerformance
            // 
            chartArea1.AxisX.Title = "Time (seconds)";
            chartArea1.AxisY.Title = "K samples/sec";
            chartArea1.AxisY2.Enabled = System.Windows.Forms.DataVisualization.Charting.AxisEnabled.True;
            chartArea1.AxisY2.Maximum = 100D;
            chartArea1.AxisY2.Minimum = 0D;
            chartArea1.AxisY2.Title = "Compression Ratio (%)";
            chartArea1.Name = "ChartArea1";
            this.chartPerformance.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chartPerformance.Legends.Add(legend1);
            this.chartPerformance.Location = new System.Drawing.Point(15, 512);
            this.chartPerformance.Name = "chartPerformance";
            series1.BorderWidth = 2;
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Color = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            series1.Legend = "Legend1";
            series1.Name = "Speed";
            series2.BorderWidth = 2;
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series2.Color = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            series2.Legend = "Legend1";
            series2.Name = "Ratio";
            series2.YAxisType = System.Windows.Forms.DataVisualization.Charting.AxisType.Secondary;
            this.chartPerformance.Series.Add(series1);
            this.chartPerformance.Series.Add(series2);
            this.chartPerformance.Size = new System.Drawing.Size(833, 244);
            this.chartPerformance.TabIndex = 15;
            this.chartPerformance.Text = "chart1";
            // 
            // lblProgress
            // 
            this.lblProgress.AutoSize = true;
            this.lblProgress.Location = new System.Drawing.Point(818, 481);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(30, 17);
            this.lblProgress.TabIndex = 16;
            this.lblProgress.Text = "0%";
            // 
            // btnCancel
            // 
            this.btnCancel.Enabled = false;
            this.btnCancel.Location = new System.Drawing.Point(347, 389);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(510, 35);
            this.btnCancel.TabIndex = 17;
            this.btnCancel.Text = "Cancel Compression";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(869, 748);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lblProgress);
            this.Controls.Add(this.chartPerformance);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.btnShowReport);
            this.Controls.Add(this.btnSaveDecompressed);
            this.Controls.Add(this.btnSaveCompressed);
            this.Controls.Add(this.btnDecompress);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnCompress);
            this.Controls.Add(this.btnSettings);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panelDropZone);
            this.Name = "Form1";
            this.Text = "Form1";
            this.panelDropZone.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartPerformance)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.Button btnPlay;
        private System.Windows.Forms.Button btnUpload;
        private System.Windows.Forms.Panel panelDropZone;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lblFileSize;
        private System.Windows.Forms.Label lblFileType;
        private System.Windows.Forms.Label lblDuration;
        private System.Windows.Forms.Label lblBitRate;
        private System.Windows.Forms.Label lblSampleRate;
        private System.Windows.Forms.Label lblChannels;
        private System.Windows.Forms.Label lblEncoding;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.Button btnCompress;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnDecompress;
        private System.Windows.Forms.Button btnSaveCompressed;
        private System.Windows.Forms.Button btnSaveDecompressed;
        private System.Windows.Forms.Button btnShowReport;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartPerformance;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.Button btnCancel;
        private OxyPlot.WindowsForms.PlotView plotAudio;
    }
}

