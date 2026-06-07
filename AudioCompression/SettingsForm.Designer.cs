namespace AudioCompression
{
    partial class SettingsForm
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
            this.groupBoxGlobal = new System.Windows.Forms.GroupBox();
            this.chkTargetBitRate = new System.Windows.Forms.CheckBox();
            this.numTargetBitRate = new System.Windows.Forms.NumericUpDown();
            this.cmbAlgorithm = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.panelSpecific = new System.Windows.Forms.Panel();
            this.panelDPCM = new System.Windows.Forms.Panel();  // ← جديد!
            this.cmbQuantizationLevels = new System.Windows.Forms.ComboBox();  // ← جديد!
            this.label11 = new System.Windows.Forms.Label();  // ← جديد!
            this.panelADM = new System.Windows.Forms.Panel();
            this.numDecay = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.numMultiplier = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.numMaxStep = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.numMinStep = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.numBitHistory = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.panelDM = new System.Windows.Forms.Panel();
            this.numFixedStepSize = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbChannels = new System.Windows.Forms.ComboBox();
            this.cmbBitDepth = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbSampleRate = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBoxGlobal.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTargetBitRate)).BeginInit();
            this.panelSpecific.SuspendLayout();
            this.panelDPCM.SuspendLayout();  // ← جديد!
            this.panelADM.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDecay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMultiplier)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxStep)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinStep)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBitHistory)).BeginInit();
            this.panelDM.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numFixedStepSize)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBoxGlobal
            // 
            this.groupBoxGlobal.Controls.Add(this.chkTargetBitRate);
            this.groupBoxGlobal.Controls.Add(this.numTargetBitRate);
            this.groupBoxGlobal.Controls.Add(this.cmbAlgorithm);
            this.groupBoxGlobal.Controls.Add(this.label4);
            this.groupBoxGlobal.Controls.Add(this.panelSpecific);
            this.groupBoxGlobal.Controls.Add(this.label3);
            this.groupBoxGlobal.Controls.Add(this.cmbChannels);
            this.groupBoxGlobal.Controls.Add(this.cmbBitDepth);
            this.groupBoxGlobal.Controls.Add(this.label2);
            this.groupBoxGlobal.Controls.Add(this.cmbSampleRate);
            this.groupBoxGlobal.Controls.Add(this.label1);
            this.groupBoxGlobal.Location = new System.Drawing.Point(12, 12);
            this.groupBoxGlobal.Name = "groupBoxGlobal";
            this.groupBoxGlobal.Size = new System.Drawing.Size(360, 318);
            this.groupBoxGlobal.TabIndex = 0;
            this.groupBoxGlobal.TabStop = false;
            this.groupBoxGlobal.Text = "Global Settings";
            // 
            // chkTargetBitRate
            // 
            this.chkTargetBitRate.AutoSize = true;
            this.chkTargetBitRate.Location = new System.Drawing.Point(6, 140);
            this.chkTargetBitRate.Name = "chkTargetBitRate";
            this.chkTargetBitRate.Size = new System.Drawing.Size(77, 21);
            this.chkTargetBitRate.TabIndex = 9;
            this.chkTargetBitRate.Text = "Bit Rate";
            this.chkTargetBitRate.UseVisualStyleBackColor = true;
            this.chkTargetBitRate.Click += new System.EventHandler(this.chkTargetBitRate_CheckedChanged);
            // 
            // numTargetBitRate
            // 
            this.numTargetBitRate.Increment = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numTargetBitRate.Location = new System.Drawing.Point(120, 137);
            this.numTargetBitRate.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.numTargetBitRate.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numTargetBitRate.Name = "numTargetBitRate";
            this.numTargetBitRate.Size = new System.Drawing.Size(220, 24);
            this.numTargetBitRate.TabIndex = 8;
            this.numTargetBitRate.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // cmbAlgorithm
            // 
            this.cmbAlgorithm.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAlgorithm.FormattingEnabled = true;
            this.cmbAlgorithm.Items.AddRange(new object[] {
            "NonlinearQuantization",
            "DPCM",
            "PredictiveDifferentialCoding",
            "DeltaModulation",
            "AdaptiveDeltaModulation"});
            this.cmbAlgorithm.Location = new System.Drawing.Point(120, 103);
            this.cmbAlgorithm.Name = "cmbAlgorithm";
            this.cmbAlgorithm.Size = new System.Drawing.Size(220, 24);
            this.cmbAlgorithm.TabIndex = 7;
            this.cmbAlgorithm.SelectedIndexChanged += new System.EventHandler(this.cmbAlgorithm_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 106);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 17);
            this.label4.TabIndex = 6;
            this.label4.Text = "Algorithm:";
            // 
            // panelSpecific
            // 
            this.panelSpecific.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelSpecific.Controls.Add(this.panelDPCM);  // ← جديد! (لازم يكون أول وحدة)
            this.panelSpecific.Controls.Add(this.panelADM);
            this.panelSpecific.Controls.Add(this.panelDM);
            this.panelSpecific.Location = new System.Drawing.Point(0, 167);
            this.panelSpecific.Name = "panelSpecific";
            this.panelSpecific.Size = new System.Drawing.Size(360, 150);
            this.panelSpecific.TabIndex = 1;
            // 
            // panelDPCM  // ← جديد! كامل
            // 
            this.panelDPCM.Controls.Add(this.cmbQuantizationLevels);
            this.panelDPCM.Controls.Add(this.label11);
            this.panelDPCM.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelDPCM.Location = new System.Drawing.Point(0, 0);
            this.panelDPCM.Name = "panelDPCM";
            this.panelDPCM.Size = new System.Drawing.Size(358, 148);
            this.panelDPCM.TabIndex = 2;
            this.panelDPCM.Visible = false;
            // 
            // cmbQuantizationLevels
            // 
            this.cmbQuantizationLevels.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbQuantizationLevels.FormattingEnabled = true;
            this.cmbQuantizationLevels.Items.AddRange(new object[] {
            "4",
            "8",
            "16",
            "32",
            "64",
            "128",
            "256"});
            this.cmbQuantizationLevels.Location = new System.Drawing.Point(150, 25);
            this.cmbQuantizationLevels.Name = "cmbQuantizationLevels";
            this.cmbQuantizationLevels.Size = new System.Drawing.Size(120, 24);
            this.cmbQuantizationLevels.TabIndex = 1;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 28);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(138, 17);
            this.label11.TabIndex = 0;
            this.label11.Text = "Quantization Levels:";
            // 
            // panelADM
            // 
            this.panelADM.Controls.Add(this.numDecay);
            this.panelADM.Controls.Add(this.label10);
            this.panelADM.Controls.Add(this.numMultiplier);
            this.panelADM.Controls.Add(this.label9);
            this.panelADM.Controls.Add(this.numMaxStep);
            this.panelADM.Controls.Add(this.label8);
            this.panelADM.Controls.Add(this.numMinStep);
            this.panelADM.Controls.Add(this.label7);
            this.panelADM.Controls.Add(this.numBitHistory);
            this.panelADM.Controls.Add(this.label6);
            this.panelADM.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelADM.Location = new System.Drawing.Point(0, 0);
            this.panelADM.Name = "panelADM";
            this.panelADM.Size = new System.Drawing.Size(358, 148);
            this.panelADM.TabIndex = 1;
            this.panelADM.Visible = false;
            // 
            // numDecay
            // 
            this.numDecay.DecimalPlaces = 2;
            this.numDecay.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numDecay.Location = new System.Drawing.Point(240, 85);
            this.numDecay.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numDecay.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            262144});
            this.numDecay.Name = "numDecay";
            this.numDecay.Size = new System.Drawing.Size(100, 24);
            this.numDecay.TabIndex = 9;
            this.numDecay.Value = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(180, 88);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(49, 17);
            this.label10.TabIndex = 8;
            this.label10.Text = "Decay:";
            // 
            // numMultiplier
            // 
            this.numMultiplier.DecimalPlaces = 2;
            this.numMultiplier.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numMultiplier.Location = new System.Drawing.Point(240, 55);
            this.numMultiplier.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numMultiplier.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMultiplier.Name = "numMultiplier";
            this.numMultiplier.Size = new System.Drawing.Size(100, 24);
            this.numMultiplier.TabIndex = 7;
            this.numMultiplier.Value = new decimal(new int[] {
            15,
            0,
            0,
            65536});
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(180, 58);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(67, 17);
            this.label9.TabIndex = 6;
            this.label9.Text = "Multiplier:";
            // 
            // numMaxStep
            // 
            this.numMaxStep.DecimalPlaces = 3;
            this.numMaxStep.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.numMaxStep.Location = new System.Drawing.Point(240, 25);
            this.numMaxStep.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMaxStep.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.numMaxStep.Name = "numMaxStep";
            this.numMaxStep.Size = new System.Drawing.Size(100, 24);
            this.numMaxStep.TabIndex = 5;
            this.numMaxStep.Value = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(180, 28);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(69, 17);
            this.label8.TabIndex = 4;
            this.label8.Text = "Max Step:";
            // 
            // numMinStep
            // 
            this.numMinStep.DecimalPlaces = 4;
            this.numMinStep.Increment = new decimal(new int[] {
            1,
            0,
            0,
            262144});
            this.numMinStep.Location = new System.Drawing.Point(127, 55);
            this.numMinStep.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMinStep.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            393216});
            this.numMinStep.Name = "numMinStep";
            this.numMinStep.Size = new System.Drawing.Size(100, 24);
            this.numMinStep.TabIndex = 3;
            this.numMinStep.Value = new decimal(new int[] {
            1,
            0,
            0,
            393216});
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 58);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(66, 17);
            this.label7.TabIndex = 2;
            this.label7.Text = "Min Step:";
            // 
            // numBitHistory
            // 
            this.numBitHistory.Location = new System.Drawing.Point(127, 25);
            this.numBitHistory.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numBitHistory.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numBitHistory.Name = "numBitHistory";
            this.numBitHistory.Size = new System.Drawing.Size(100, 24);
            this.numBitHistory.TabIndex = 1;
            this.numBitHistory.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 28);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(115, 17);
            this.label6.TabIndex = 0;
            this.label6.Text = "Bit History Length:";
            // 
            // panelDM
            // 
            this.panelDM.Controls.Add(this.numFixedStepSize);
            this.panelDM.Controls.Add(this.label5);
            this.panelDM.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelDM.Location = new System.Drawing.Point(0, 0);
            this.panelDM.Name = "panelDM";
            this.panelDM.Size = new System.Drawing.Size(358, 148);
            this.panelDM.TabIndex = 0;
            // 
            // numFixedStepSize
            // 
            this.numFixedStepSize.DecimalPlaces = 4;
            this.numFixedStepSize.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.numFixedStepSize.Location = new System.Drawing.Point(127, 7);
            this.numFixedStepSize.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numFixedStepSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            262144});
            this.numFixedStepSize.Name = "numFixedStepSize";
            this.numFixedStepSize.Size = new System.Drawing.Size(220, 24);
            this.numFixedStepSize.TabIndex = 1;
            this.numFixedStepSize.Value = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 10);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(127, 17);
            this.label5.TabIndex = 0;
            this.label5.Text = "Fixed Step Size (Δ):";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 79);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 17);
            this.label3.TabIndex = 5;
            this.label3.Text = "Channels";
            // 
            // cmbChannels
            // 
            this.cmbChannels.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbChannels.FormattingEnabled = true;
            this.cmbChannels.Items.AddRange(new object[] {
            "original",
            "1",
            "2"});
            this.cmbChannels.Location = new System.Drawing.Point(120, 76);
            this.cmbChannels.Name = "cmbChannels";
            this.cmbChannels.Size = new System.Drawing.Size(220, 24);
            this.cmbChannels.TabIndex = 4;
            // 
            // cmbBitDepth
            // 
            this.cmbBitDepth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBitDepth.FormattingEnabled = true;
            this.cmbBitDepth.Items.AddRange(new object[] {
            "original",
            "8",
            "16",
            "24",
            "32"});
            this.cmbBitDepth.Location = new System.Drawing.Point(120, 49);
            this.cmbBitDepth.Name = "cmbBitDepth";
            this.cmbBitDepth.Size = new System.Drawing.Size(220, 24);
            this.cmbBitDepth.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "Bit Depth:";
            // 
            // cmbSampleRate
            // 
            this.cmbSampleRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSampleRate.FormattingEnabled = true;
            this.cmbSampleRate.Items.AddRange(new object[] {
            "original",
            "8000",
            "16000",
            "22050",
            "44100",
            "48000"});
            this.cmbSampleRate.Location = new System.Drawing.Point(120, 22);
            this.cmbSampleRate.Name = "cmbSampleRate";
            this.cmbSampleRate.Size = new System.Drawing.Size(220, 24);
            this.cmbSampleRate.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Sample Rate:";
            // 
            // btnSave
            // 
            this.btnSave.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSave.Location = new System.Drawing.Point(216, 335);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(297, 336);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(382, 373);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.groupBoxGlobal);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Compression Settings";
            this.groupBoxGlobal.ResumeLayout(false);
            this.groupBoxGlobal.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTargetBitRate)).EndInit();
            this.panelSpecific.ResumeLayout(false);
            this.panelDPCM.ResumeLayout(false);  // ← جديد!
            this.panelDPCM.PerformLayout();  // ← جديد!
            this.panelADM.ResumeLayout(false);
            this.panelADM.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDecay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMultiplier)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxStep)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinStep)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBitHistory)).EndInit();
            this.panelDM.ResumeLayout(false);
            this.panelDM.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numFixedStepSize)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxGlobal;
        private System.Windows.Forms.ComboBox cmbAlgorithm;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbChannels;
        private System.Windows.Forms.ComboBox cmbBitDepth;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbSampleRate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panelSpecific;
        private System.Windows.Forms.Panel panelDM;
        private System.Windows.Forms.NumericUpDown numFixedStepSize;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.NumericUpDown numTargetBitRate;
        private System.Windows.Forms.CheckBox chkTargetBitRate;

        private System.Windows.Forms.Panel panelADM;
        private System.Windows.Forms.NumericUpDown numBitHistory;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown numMinStep;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown numMaxStep;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown numMultiplier;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown numDecay;
        private System.Windows.Forms.Label label10;


        private System.Windows.Forms.Panel panelDPCM;
        private System.Windows.Forms.ComboBox cmbQuantizationLevels;
        private System.Windows.Forms.Label label11;
    }
}