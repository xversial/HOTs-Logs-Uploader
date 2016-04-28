using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace HOTSLogsUploader
{
	public class About : Form
	{
		private IContainer components;

		private Label label1;

		private Button button1;

		private GroupBox groupBox1;

		private PictureBox pictureBox1;

		public About()
		{
			this.InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			base.Hide();
		}

		private void About_FormClosing(object sender, FormClosingEventArgs e)
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(About));
			this.label1 = new Label();
			this.button1 = new Button();
			this.groupBox1 = new GroupBox();
			this.pictureBox1 = new PictureBox();
			this.groupBox1.SuspendLayout();
			((ISupportInitialize)this.pictureBox1).BeginInit();
			base.SuspendLayout();
			this.label1.AutoSize = true;
			this.label1.Location = new Point(23, 16);
			this.label1.Name = "label1";
			this.label1.Size = new Size(401, 156);
			this.label1.TabIndex = 0;
			this.label1.Text = componentResourceManager.GetString("label1.Text");
			this.button1.Location = new Point(199, 276);
			this.button1.Name = "button1";
			this.button1.Size = new Size(75, 23);
			this.button1.TabIndex = 1;
			this.button1.Text = "OK";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new EventHandler(this.button1_Click);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new Point(12, 68);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new Size(448, 202);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.pictureBox1.Image = (Image)componentResourceManager.GetObject("pictureBox1.Image");
			this.pictureBox1.Location = new Point(132, 12);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new Size(206, 50);
			this.pictureBox1.TabIndex = 3;
			this.pictureBox1.TabStop = false;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = SystemColors.ButtonHighlight;
			base.ClientSize = new Size(472, 308);
			base.Controls.Add(this.pictureBox1);
			base.Controls.Add(this.button1);
			base.Controls.Add(this.groupBox1);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "About";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "About";
			base.TopMost = true;
			base.FormClosing += new FormClosingEventHandler(this.About_FormClosing);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((ISupportInitialize)this.pictureBox1).EndInit();
			base.ResumeLayout(false);
		}
	}
}
