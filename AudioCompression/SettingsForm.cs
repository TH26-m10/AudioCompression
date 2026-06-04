using System;
using System.Windows.Forms;

namespace AudioCompression
{
    public partial class SettingsForm : Form
    {
        string original = "original";
        public CompressionSettings Settings { get; private set; }

        public SettingsForm(CompressionSettings currentSettings)
        {
            InitializeComponent();
            Settings = currentSettings;
            LoadSettings();
        }

        private void LoadSettings()
        {
            // إعدادات عامة
            // sample rate
            if (Settings.SampleRate.HasValue)
                cmbSampleRate.SelectedItem = Settings.SampleRate.Value.ToString();
            else
                cmbSampleRate.SelectedItem = original;
            // bit depth
            if (Settings.BitDepth.HasValue)
                cmbBitDepth.SelectedItem = Settings.BitDepth.Value.ToString();
            else
                cmbBitDepth.SelectedItem = original;
            //channels
            if (Settings.Channels.HasValue)
                cmbChannels.SelectedItem = Settings.Channels.Value.ToString();
            else
                cmbChannels.SelectedItem = original;

            // Target Bit Rate
            if (Settings.TargetBitRate.HasValue)
            {
                chkTargetBitRate.Checked = true;
                numTargetBitRate.Value = Settings.TargetBitRate.Value;
            }
            else
            {
                chkTargetBitRate.Checked = false;
                numTargetBitRate.Enabled = false;
            }

            // algorithm
            cmbAlgorithm.SelectedItem = Settings.Algorithm.ToString();

            // Nonlinear Quantization
            //cmbCompandingLaw.SelectedItem = Settings.CompandingLaw.ToString();

            // DPCM
            //numPredictorOrder.Value = Settings.PredictorOrder;

            // ADPCM
            //numAdaptationSpeed.Value = (decimal)Settings.AdaptationSpeed;

            // Delta Modulation
            numFixedStepSize.Value = (decimal)Settings.FixedStepSize;

            // Adaptive Delta Modulation
            /*numBitHistory.Value = Settings.BitHistoryLength;
            numMinStep.Value = (decimal)Settings.MinStepSize;
            numMaxStep.Value = (decimal)Settings.MaxStepSize;
            numMultiplier.Value = (decimal)Settings.StepMultiplier;
            numDecay.Value = (decimal)Settings.StepDecay;*/

            UpdatePanelVisibility();
        }
        private void chkTargetBitRate_CheckedChanged(object sender, EventArgs e)
        {
            numTargetBitRate.Enabled = chkTargetBitRate.Checked;
        }

        private void cmbAlgorithm_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdatePanelVisibility();
        }

        private void UpdatePanelVisibility()
        {
            //panelNonlinearQuant.Visible = false;
            //panelDPCM.Visible = false;
            //panelADPCM.Visible = false;
            panelDM.Visible = false;
            //panelADM.Visible = false;

            if (cmbAlgorithm.SelectedItem == null) return;

            string algo = cmbAlgorithm.SelectedItem.ToString();

            switch (algo)
            {
                //case "NonlinearQuantization": panelNonlinearQuant.Visible = true; break;
                //case "DPCM": panelDPCM.Visible = true; break;
                //case "PredictiveDifferentialCoding": panelADPCM.Visible = true; break;
                case "DeltaModulation": panelDM.Visible = true; break;
                //case "AdaptiveDeltaModulation": panelADM.Visible = true; break;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // إعدادات عامة
            //sample rate
            if (cmbSampleRate.SelectedItem.ToString() == original)
            {
                Settings.SampleRate = null;
            }
            else
            {
                Settings.SampleRate = int.Parse(cmbSampleRate.SelectedItem.ToString());
            }
            //bit depth
            if (cmbBitDepth.SelectedItem.ToString() == original)
            {
                Settings.BitDepth = null;
            }
            else
            {
                Settings.BitDepth = int.Parse(cmbBitDepth.SelectedItem.ToString());
            }
            //channels
            if (cmbChannels.SelectedItem.ToString() == original)
            {
                Settings.Channels = null;
            }
            else
            {
                Settings.Channels = int.Parse(cmbChannels.SelectedItem.ToString());
            }

            // Target Bit Rate
            if (chkTargetBitRate.Checked)
                Settings.TargetBitRate = (int)numTargetBitRate.Value;
            else
                Settings.TargetBitRate = null;


            Settings.Algorithm = (CompressionAlgorithm)Enum.Parse(
                typeof(CompressionAlgorithm), cmbAlgorithm.SelectedItem.ToString());

            // Nonlinear Quantization
           /* Settings.CompandingLaw = (CompandingLaw)Enum.Parse(
                typeof(CompandingLaw), cmbCompandingLaw.SelectedItem.ToString());*/

            // DPCM
            //Settings.PredictorOrder = (int)numPredictorOrder.Value;

            // ADPCM
            //Settings.AdaptationSpeed = (double)numAdaptationSpeed.Value;

            // Delta Modulation
            Settings.FixedStepSize = (float)numFixedStepSize.Value;

            // Adaptive Delta Modulation
            /*Settings.BitHistoryLength = (int)numBitHistory.Value;
            Settings.MinStepSize = (float)numMinStep.Value;
            Settings.MaxStepSize = (float)numMaxStep.Value;
            Settings.StepMultiplier = (float)numMultiplier.Value;
            Settings.StepDecay = (float)numDecay.Value;*/

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}