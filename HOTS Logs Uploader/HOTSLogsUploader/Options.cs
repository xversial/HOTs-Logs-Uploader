using HOTSLogsUploader.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace HOTSLogsUploader
{
	public class Options : Form
	{
		public new Form ParentForm;

		public Form SetPlayerIDForm;

		private IContainer components;

		private Button buttonSaveSettings;

		private GroupBox groupBox1;

		private CheckBox checkBoxEnableTrayNotifications;

		private TextBox textBoxReplayFolder;

		private Button buttonReplayFolderBrowser;

		private FolderBrowserDialog folderBrowserDialog1;

		private GroupBox groupBox2;

		private Button buttonResetAllUploads;

		private Button buttonSetPlayerProfile;

		public Options()
		{
			this.InitializeComponent();
		}

		public new void Show()
		{
			this.checkBoxEnableTrayNotifications.Checked = Settings.Default.ShowTrayNotifications;
			this.textBoxReplayFolder.Text = Settings.Default.ReplayDirectory;
			base.Show();
		}

		private void buttonReplayFolderBrowser_Click(object sender, EventArgs e)
		{
			if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
			{
				this.textBoxReplayFolder.Text = this.folderBrowserDialog1.SelectedPath;
			}
		}

		private void buttonSaveSettings_Click(object sender, EventArgs e)
		{
			this.textBoxReplayFolder.Text = this.textBoxReplayFolder.Text.Trim();
			Settings.Default.ShowTrayNotifications = this.checkBoxEnableTrayNotifications.Checked;
			if (Directory.Exists(this.textBoxReplayFolder.Text))
			{
				Settings.Default.ReplayDirectory = this.textBoxReplayFolder.Text;
			}
			Settings.Default.Save();
			base.Hide();
		}

		private void Options_FormClosing(object sender, FormClosingEventArgs e)
		{
			e.Cancel = true;
			base.Hide();
		}

		private void buttonResetAllUploads_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("This will reset all settings, and attempt to re-upload all of your replay files.\r\n\r\nAre you sure you want to continue?", "Reset All Settings and Uploads", MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				MessageBox.Show("Your settings and upload history will be reset, and the application will close.\r\n\r\nPlease run the application again to re-upload your replay files.", "Reset All Settings and Uploads");
				Settings.Default.Reset();
				this.ParentForm.Close();
			}
		}

		private void buttonSetPlayerProfile_Click(object sender, EventArgs e)
		{
			this.SetPlayerIDForm.Show();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(Options));
			this.buttonSaveSettings = new Button();
			this.groupBox1 = new GroupBox();
			this.groupBox2 = new GroupBox();
			this.textBoxReplayFolder = new TextBox();
			this.buttonReplayFolderBrowser = new Button();
			this.checkBoxEnableTrayNotifications = new CheckBox();
			this.folderBrowserDialog1 = new FolderBrowserDialog();
			this.buttonResetAllUploads = new Button();
			this.buttonSetPlayerProfile = new Button();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			base.SuspendLayout();
			this.buttonSaveSettings.Location = new Point(20, 189);
			this.buttonSaveSettings.Margin = new Padding(4, 5, 4, 5);
			this.buttonSaveSettings.Name = "buttonSaveSettings";
			this.buttonSaveSettings.Size = new Size(255, 35);
			this.buttonSaveSettings.TabIndex = 0;
			this.buttonSaveSettings.Text = "Save";
			this.buttonSaveSettings.UseVisualStyleBackColor = true;
			this.buttonSaveSettings.Click += new EventHandler(this.buttonSaveSettings_Click);
			this.groupBox1.Controls.Add(this.buttonSetPlayerProfile);
			this.groupBox1.Controls.Add(this.groupBox2);
			this.groupBox1.Controls.Add(this.checkBoxEnableTrayNotifications);
			this.groupBox1.Location = new Point(20, 18);
			this.groupBox1.Margin = new Padding(4, 5, 4, 5);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Padding = new Padding(4, 5, 4, 5);
			this.groupBox1.Size = new Size(702, 162);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox2.Controls.Add(this.textBoxReplayFolder);
			this.groupBox2.Controls.Add(this.buttonReplayFolderBrowser);
			this.groupBox2.Location = new Point(9, 78);
			this.groupBox2.Margin = new Padding(4, 5, 4, 5);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Padding = new Padding(4, 5, 4, 5);
			this.groupBox2.Size = new Size(685, 74);
			this.groupBox2.TabIndex = 5;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "HOTS Replay Folder:";
			this.textBoxReplayFolder.Location = new Point(9, 28);
			this.textBoxReplayFolder.Margin = new Padding(4, 5, 4, 5);
			this.textBoxReplayFolder.Name = "textBoxReplayFolder";
			this.textBoxReplayFolder.Size = new Size(624, 26);
			this.textBoxReplayFolder.TabIndex = 4;
			this.buttonReplayFolderBrowser.Location = new Point(641, 24);
			this.buttonReplayFolderBrowser.Margin = new Padding(4, 5, 4, 5);
			this.buttonReplayFolderBrowser.Name = "buttonReplayFolderBrowser";
			this.buttonReplayFolderBrowser.Size = new Size(36, 35);
			this.buttonReplayFolderBrowser.TabIndex = 2;
			this.buttonReplayFolderBrowser.Text = "...";
			this.buttonReplayFolderBrowser.UseVisualStyleBackColor = true;
			this.buttonReplayFolderBrowser.Click += new EventHandler(this.buttonReplayFolderBrowser_Click);
			this.checkBoxEnableTrayNotifications.AutoSize = true;
			this.checkBoxEnableTrayNotifications.Location = new Point(9, 29);
			this.checkBoxEnableTrayNotifications.Margin = new Padding(4, 5, 4, 5);
			this.checkBoxEnableTrayNotifications.Name = "checkBoxEnableTrayNotifications";
			this.checkBoxEnableTrayNotifications.Size = new Size(210, 24);
			this.checkBoxEnableTrayNotifications.TabIndex = 1;
			this.checkBoxEnableTrayNotifications.Text = "Enable Tray Notifications";
			this.checkBoxEnableTrayNotifications.UseVisualStyleBackColor = true;
			this.folderBrowserDialog1.RootFolder = Environment.SpecialFolder.Personal;
			this.buttonResetAllUploads.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Italic, GraphicsUnit.Point, 0);
			this.buttonResetAllUploads.ForeColor = Color.Red;
			this.buttonResetAllUploads.Location = new Point(284, 189);
			this.buttonResetAllUploads.Margin = new Padding(4, 5, 4, 5);
			this.buttonResetAllUploads.Name = "buttonResetAllUploads";
			this.buttonResetAllUploads.Size = new Size(168, 35);
			this.buttonResetAllUploads.TabIndex = 2;
			this.buttonResetAllUploads.Text = "Reset All Uploads";
			this.buttonResetAllUploads.UseVisualStyleBackColor = true;
			this.buttonResetAllUploads.Click += new EventHandler(this.buttonResetAllUploads_Click);
			this.buttonSetPlayerProfile.Location = new Point(503, 27);
			this.buttonSetPlayerProfile.Name = "buttonSetPlayerProfile";
			this.buttonSetPlayerProfile.Size = new Size(183, 43);
			this.buttonSetPlayerProfile.TabIndex = 6;
			this.buttonSetPlayerProfile.Text = "Set Player Profile";
			this.buttonSetPlayerProfile.UseVisualStyleBackColor = true;
			this.buttonSetPlayerProfile.Click += new EventHandler(this.buttonSetPlayerProfile_Click);
			base.AutoScaleDimensions = new SizeF(9f, 20f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = SystemColors.ButtonHighlight;
			base.ClientSize = new Size(735, 240);
			base.Controls.Add(this.buttonResetAllUploads);
			base.Controls.Add(this.groupBox1);
			base.Controls.Add(this.buttonSaveSettings);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.Margin = new Padding(4, 5, 4, 5);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "Options";
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "Settings";
			base.TopMost = true;
			base.FormClosing += new FormClosingEventHandler(this.Options_FormClosing);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			base.ResumeLayout(false);
		}
	}
}
