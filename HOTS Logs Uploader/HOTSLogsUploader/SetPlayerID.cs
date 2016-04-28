using HOTSLogsUploader.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HOTSLogsUploader
{
	public class SetPlayerID : Form
	{
		public new Form ParentForm;

		private IContainer components;

		private Button buttonSaveSettings;

		private TextBox textBoxPlayerProfileURL;

		private FolderBrowserDialog folderBrowserDialog1;

		private GroupBox groupBox2;

		private Label label1;

		public SetPlayerID()
		{
			this.InitializeComponent();
		}

		public new void Show()
		{
			this.textBoxPlayerProfileURL.Text = ((Settings.Default.PlayerID == -1) ? "" : ("https://www.hotslogs.com/Player/Profile?PlayerID=" + Settings.Default.PlayerID));
			base.Show();
		}

		private void buttonSaveSettings_Click(object sender, EventArgs e)
		{
			this.textBoxPlayerProfileURL.Text = this.textBoxPlayerProfileURL.Text.Trim();
			int playerID;
			if (this.textBoxPlayerProfileURL.Text.Contains('?') && int.TryParse(this.textBoxPlayerProfileURL.Text.Split(new char[]
			{
				'?'
			})[1].Replace("PlayerID=", ""), out playerID))
			{
				Settings.Default.PlayerID = playerID;
				Settings.Default.Save();
				base.Hide();
				return;
			}
			MessageBox.Show("The URL you entered was invalid.  Your HOTS Logs Profile URL should be similar to this: https://www.hotslogs.com/Player/Profile?PlayerID=###", "Error: Invalid HOTS Logs Profile URL");
		}

		private void SetPlayerID_FormClosing(object sender, FormClosingEventArgs e)
		{
			e.Cancel = true;
			base.Hide();
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(SetPlayerID));
			this.buttonSaveSettings = new Button();
			this.groupBox2 = new GroupBox();
			this.textBoxPlayerProfileURL = new TextBox();
			this.folderBrowserDialog1 = new FolderBrowserDialog();
			this.label1 = new Label();
			this.groupBox2.SuspendLayout();
			base.SuspendLayout();
			this.buttonSaveSettings.Location = new Point(29, 132);
			this.buttonSaveSettings.Margin = new Padding(4, 5, 4, 5);
			this.buttonSaveSettings.Name = "buttonSaveSettings";
			this.buttonSaveSettings.Size = new Size(255, 35);
			this.buttonSaveSettings.TabIndex = 0;
			this.buttonSaveSettings.Text = "Save";
			this.buttonSaveSettings.UseVisualStyleBackColor = true;
			this.buttonSaveSettings.Click += new EventHandler(this.buttonSaveSettings_Click);
			this.groupBox2.Controls.Add(this.textBoxPlayerProfileURL);
			this.groupBox2.Location = new Point(20, 14);
			this.groupBox2.Margin = new Padding(4, 5, 4, 5);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Padding = new Padding(4, 5, 4, 5);
			this.groupBox2.Size = new Size(853, 69);
			this.groupBox2.TabIndex = 5;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Copy and Paste your HOTS Logs Profile URL:";
			this.textBoxPlayerProfileURL.Location = new Point(9, 28);
			this.textBoxPlayerProfileURL.Margin = new Padding(4, 5, 4, 5);
			this.textBoxPlayerProfileURL.Name = "textBoxPlayerProfileURL";
			this.textBoxPlayerProfileURL.Size = new Size(836, 26);
			this.textBoxPlayerProfileURL.TabIndex = 4;
			this.folderBrowserDialog1.RootFolder = Environment.SpecialFolder.Personal;
			this.label1.AutoSize = true;
			this.label1.Location = new Point(29, 92);
			this.label1.Name = "label1";
			this.label1.Size = new Size(457, 20);
			this.label1.TabIndex = 6;
			this.label1.Text = "Example: https://www.hotslogs.com/Player/Profile?PlayerID=###";
			base.AutoScaleDimensions = new SizeF(9f, 20f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = SystemColors.ButtonHighlight;
			base.ClientSize = new Size(886, 188);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.groupBox2);
			base.Controls.Add(this.buttonSaveSettings);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.Margin = new Padding(4, 5, 4, 5);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "SetPlayerID";
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "Player Profile";
			base.TopMost = true;
			base.FormClosing += new FormClosingEventHandler(this.SetPlayerID_FormClosing);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
