namespace YSFlightReplayStretcher
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.FileBrowser = new System.Windows.Forms.OpenFileDialog();
            this.First_FileNameHolder = new System.Windows.Forms.TextBox();
            this.First_BrowseButton = new System.Windows.Forms.Button();
            this.Step1 = new System.Windows.Forms.Label();
            this.Step2 = new System.Windows.Forms.Label();
            this.Step3 = new System.Windows.Forms.Label();
            this.Step3_0 = new System.Windows.Forms.Label();
            this.Process = new System.Windows.Forms.Button();
            this.Progress = new System.Windows.Forms.ProgressBar();
            this.ProcessText = new System.Windows.Forms.Label();
            this.Second_BrowseButton = new System.Windows.Forms.Button();
            this.Second_FileNameHolder = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // FileBrowser
            // 
            this.FileBrowser.DefaultExt = "yfs";
            this.FileBrowser.FileName = "*.yfs";
            this.FileBrowser.Filter = "\"YSFlight Replay File|*.yfs|All Files|*.*\"";
            this.FileBrowser.InitialDirectory = "./";
            this.FileBrowser.Title = "Select YSFlight Replay File";
            // 
            // First_FileNameHolder
            // 
            this.First_FileNameHolder.AllowDrop = true;
            this.First_FileNameHolder.Location = new System.Drawing.Point(15, 36);
            this.First_FileNameHolder.Name = "First_FileNameHolder";
            this.First_FileNameHolder.Size = new System.Drawing.Size(556, 20);
            this.First_FileNameHolder.TabIndex = 0;
            this.First_FileNameHolder.Text = "*.yfs";
            // 
            // First_BrowseButton
            // 
            this.First_BrowseButton.Location = new System.Drawing.Point(577, 33);
            this.First_BrowseButton.Name = "First_BrowseButton";
            this.First_BrowseButton.Size = new System.Drawing.Size(75, 23);
            this.First_BrowseButton.TabIndex = 1;
            this.First_BrowseButton.Text = "Browse";
            this.First_BrowseButton.UseVisualStyleBackColor = true;
            this.First_BrowseButton.Click += new System.EventHandler(this.ShowDialog_R1);
            // 
            // Step1
            // 
            this.Step1.AutoSize = true;
            this.Step1.Location = new System.Drawing.Point(12, 20);
            this.Step1.Name = "Step1";
            this.Step1.Size = new System.Drawing.Size(141, 13);
            this.Step1.TabIndex = 2;
            this.Step1.Text = "Step 1: Enter First File Name";
            // 
            // Step2
            // 
            this.Step2.AutoSize = true;
            this.Step2.Location = new System.Drawing.Point(12, 59);
            this.Step2.Name = "Step2";
            this.Step2.Size = new System.Drawing.Size(159, 13);
            this.Step2.TabIndex = 3;
            this.Step2.Text = "Step 2: Enter Second File Name";
            // 
            // Step3
            // 
            this.Step3.AutoSize = true;
            this.Step3.Location = new System.Drawing.Point(12, 107);
            this.Step3.Name = "Step3";
            this.Step3.Size = new System.Drawing.Size(85, 13);
            this.Step3.TabIndex = 9;
            this.Step3.Text = "Step 3: Process!";
            // 
            // Step3_0
            // 
            this.Step3_0.AutoSize = true;
            this.Step3_0.Location = new System.Drawing.Point(48, 120);
            this.Step3_0.Name = "Step3_0";
            this.Step3_0.Size = new System.Drawing.Size(314, 13);
            this.Step3_0.TabIndex = 10;
            this.Step3_0.Text = "Press the \"Process\" Button below to Make your adjusted Replay!";
            // 
            // Process
            // 
            this.Process.Location = new System.Drawing.Point(577, 170);
            this.Process.Name = "Process";
            this.Process.Size = new System.Drawing.Size(75, 23);
            this.Process.TabIndex = 11;
            this.Process.Text = "Process";
            this.Process.UseVisualStyleBackColor = true;
            this.Process.Click += new System.EventHandler(this.Process_Click);
            // 
            // Progress
            // 
            this.Progress.Location = new System.Drawing.Point(15, 170);
            this.Progress.Name = "Progress";
            this.Progress.Size = new System.Drawing.Size(556, 23);
            this.Progress.TabIndex = 12;
            // 
            // ProcessText
            // 
            this.ProcessText.AutoSize = true;
            this.ProcessText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ProcessText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(180)))), ((int)(((byte)(0)))));
            this.ProcessText.Location = new System.Drawing.Point(12, 154);
            this.ProcessText.Name = "ProcessText";
            this.ProcessText.Size = new System.Drawing.Size(195, 13);
            this.ProcessText.TabIndex = 13;
            this.ProcessText.Text = "Process: Processing Not Started.";
            // 
            // Second_BrowseButton
            // 
            this.Second_BrowseButton.Location = new System.Drawing.Point(577, 73);
            this.Second_BrowseButton.Name = "Second_BrowseButton";
            this.Second_BrowseButton.Size = new System.Drawing.Size(75, 23);
            this.Second_BrowseButton.TabIndex = 15;
            this.Second_BrowseButton.Text = "Browse";
            this.Second_BrowseButton.UseVisualStyleBackColor = true;
            this.Second_BrowseButton.Click += new System.EventHandler(this.ShowDialog_R2);
            // 
            // Second_FileNameHolder
            // 
            this.Second_FileNameHolder.AllowDrop = true;
            this.Second_FileNameHolder.Location = new System.Drawing.Point(15, 75);
            this.Second_FileNameHolder.Name = "Second_FileNameHolder";
            this.Second_FileNameHolder.Size = new System.Drawing.Size(556, 20);
            this.Second_FileNameHolder.TabIndex = 14;
            this.Second_FileNameHolder.Text = "*.yfs";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(654, 202);
            this.Controls.Add(this.Second_BrowseButton);
            this.Controls.Add(this.Second_FileNameHolder);
            this.Controls.Add(this.ProcessText);
            this.Controls.Add(this.Progress);
            this.Controls.Add(this.Process);
            this.Controls.Add(this.Step3_0);
            this.Controls.Add(this.Step3);
            this.Controls.Add(this.Step2);
            this.Controls.Add(this.Step1);
            this.Controls.Add(this.First_BrowseButton);
            this.Controls.Add(this.First_FileNameHolder);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "YSFlight Replay Merger: Join two replays together!";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog FileBrowser;
        private System.Windows.Forms.TextBox First_FileNameHolder;
        private System.Windows.Forms.Button First_BrowseButton;
        private System.Windows.Forms.Label Step1;
        private System.Windows.Forms.Label Step2;
        private System.Windows.Forms.Label Step3;
        private System.Windows.Forms.Label Step3_0;
        private System.Windows.Forms.Button Process;
        private System.Windows.Forms.ProgressBar Progress;
        private System.Windows.Forms.Label ProcessText;
        private System.Windows.Forms.Button Second_BrowseButton;
        private System.Windows.Forms.TextBox Second_FileNameHolder;
    }
}

