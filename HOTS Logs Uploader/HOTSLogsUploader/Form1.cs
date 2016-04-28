using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Heroes.ReplayParser;
using HOTSLogsUploader.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace HOTSLogsUploader
{
    public class Form1 : Form
    {
        private delegate void RebindDataGridViewCallback();

        [CompilerGenerated]
        [Serializable]
        private sealed class ClassC
        {
            public static readonly Form1.ClassC var9 = new Form1.ClassC();

            public static Func<Player, int> var9__14_0;

            public static Func<Player, string> var9__14_1;

            public static Func<DataGridViewRow, bool> var9__18_0;

            public static Func<DataGridViewRow, int> var9__18_1;

            public static Func<string, DateTime> var9__18_2;

            public static Func<string, string[]> var9__19_0;

            public static Func<string[], ReplayFile> var9__19_1;

            public static Func<ReplayFile, string> var9__20_0;

            public static Func<string, string> var9__22_0;

            public static Func<string, string> var9__22_1;

            public static Func<string, DateTime> var9__22_7;

            internal int b__14_0(Player i)
            {
                return i.BattleNetId;
            }

            internal string b__14_1(Player i)
            {
                return i.BattleNetId.ToString();
            }

            internal bool b__18_0(DataGridViewRow i)
            {
                return i.Selected;
            }

            internal int b__18_1(DataGridViewRow i)
            {
                return i.Index;
            }

            internal DateTime b__18_2(string i)
            {
                return File.GetCreationTimeUtc(i);
            }

            internal string[] b__19_0(string i)
            {
                return i.Split(new char[]
                {
                    '*'
                });
            }

            internal ReplayFile b__19_1(string[] i)
            {
                return new ReplayFile(i[0], i[1], new DateTime?(DateTime.Parse(i[2])));
            }

            internal string b__20_0(ReplayFile i)
            {
                return string.Concat(new object[]
                {
                    i.FileName,
                    "*",
                    i.UploadStatus,
                    "*",
                    i.DateTimeUploaded
                });
            }

            internal string b__22_0(string i)
            {
                return i;
            }

            internal string b__22_1(string i)
            {
                return i;
            }

            internal DateTime b__22_7(string i)
            {
                return File.GetCreationTimeUtc(i);
            }
        }

        private Options formOptions = new Options();

        private SetPlayerID formSetPlayerID = new SetPlayerID();

        private About formAbout = new About();

        private const string AWSS3AccessKeyId = "AKIAIESBHEUH4KAAG4UA";

        private const string AWSS3SecretAccessKey = "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";

        private const int NotificationDisplayDurationInMS = 3000;

        private string replayFolder = Settings.Default.ReplayDirectory;

        private bool automaticReplayUpload = true;

        private DateTime displayNextNotification = DateTime.UtcNow;

        private IContainer components;

        private FolderBrowserDialog folderBrowserDialog1;

        private MenuStrip menuStrip1;

        private ToolStripMenuItem optionsToolStripMenuItem;

        private DataGridView dataGridView1;

        private BackgroundWorker backgroundWorker1;

        private ToolStripMenuItem aboutToolStripMenuItem;

        private NotifyIcon notifyIcon1;

        private ContextMenuStrip contextMenuStrip1;

        private ToolStripMenuItem showToolStripMenuItem;

        private ToolStripMenuItem hideToolStripMenuItem;

        private ToolStripSeparator toolStripSeparator1;

        private ToolStripMenuItem exitToolStripMenuItem;

        private PictureBox pictureBox1;

        private ContextMenuStrip contextMenuStrip2;

        private ToolStripMenuItem UploadToolStripMenuItem;

        private Label labelUploading;

        private DataGridViewTextBoxColumn FilePath;

        private DataGridViewTextBoxColumn UploadStatus;

        private DataGridViewTextBoxColumn FileName;

        private DataGridViewTextBoxColumn DateUploaded;

        private ToolStripMenuItem viewProfileToolStripMenuItem;

        public Form1()
        {
            this.InitializeComponent();
            this.formOptions.ParentForm = this;
            this.formOptions.SetPlayerIDForm = this.formSetPlayerID;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (this.replayFolder == "")
            {
                this.SetDefaultReplayFolder();
            }
            this.labelUploading.Visible = false;
            this.BindDataGridViewSource();
            this.EnableAutomaticReplayUpload();
            this.ShowBalloonTip("HOTS Logs", "Right-click the icon for more options");
            this.formOptions.VisibleChanged += new EventHandler(this.About_VisibleChanged);
        }

        private void About_VisibleChanged(object sender, EventArgs e)
        {
            if (!((Form)sender).Visible)
            {
                this.replayFolder = Settings.Default.ReplayDirectory;
            }
        }

        private void ShowBalloonTip(string title, string text)
        {
            if (Settings.Default.ShowTrayNotifications && DateTime.UtcNow > this.displayNextNotification)
            {
                this.displayNextNotification = DateTime.UtcNow.AddMilliseconds(3000.0);
                this.notifyIcon1.BalloonTipTitle = title;
                this.notifyIcon1.BalloonTipText = text;
                this.notifyIcon1.ShowBalloonTip(3000);
            }
        }

        private void SetReplayFolder()
        {
            if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                this.replayFolder = this.folderBrowserDialog1.SelectedPath;
                Settings.Default.ReplayDirectory = this.replayFolder;
                Settings.Default.Save();
                this.BindDataGridViewSource();
            }
        }

        private void UploadReplay(string filePath)
        {
            string text;
            if (new FileInfo(filePath).Length > 20000000L)
            {
                text = "FileSizeTooLarge";
            }
            else
            {
                using (WebClient webClient = new WebClient())
                {
                    Tuple<DataParser.ReplayParseResult, Replay> tuple = DataParser.ParseReplay(filePath, false, false, false);

                    bool xWin = false;
                    if (tuple.Item1 == DataParser.ReplayParseResult.Success)
                    {
                        foreach (var player in tuple.Item2.Players.Where(i => i.IsWinner))
                        {
                            if (player.BattleTag == 1234 && player.Name.Equals("xversial", StringComparison.InvariantCultureIgnoreCase))
                            {
                                xWin = true;
                            }
                        }
                    }

                    Guid? guid = null;
                    if (tuple.Item2 != null)
                    {
                        using (MD5 mD = MD5.Create())
                        {
                            HashAlgorithm arg_D3_0 = mD;
                            Encoding arg_CE_0 = Encoding.ASCII;
                            string arg_B0_0 = "";
                            IEnumerable<Player> arg_84_0 = tuple.Item2.Players;
                            Func<Player, int> arg_84_1;
                            if ((arg_84_1 = Form1.ClassC.var9__14_0) == null)
                            {
                                arg_84_1 = (Form1.ClassC.var9__14_0 = new Func<Player, int>(Form1.ClassC.var9.b__14_0));
                            }
                            IEnumerable<Player> arg_AB_0 = arg_84_0.OrderBy(arg_84_1);
                            Func<Player, string> arg_AB_1;
                            if ((arg_AB_1 = Form1.ClassC.var9__14_1) == null)
                            {
                                arg_AB_1 = (Form1.ClassC.var9__14_1 = new Func<Player, string>(Form1.ClassC.var9.b__14_1));
                            }
                            guid = new Guid?(new Guid(arg_D3_0.ComputeHash(arg_CE_0.GetBytes(string.Join(arg_B0_0, arg_AB_0.Select(arg_AB_1)) + tuple.Item2.RandomValue.ToString()))));
                        }
                    }
                    if (tuple.Item1 == DataParser.ReplayParseResult.ComputerPlayerFound || tuple.Item1 == DataParser.ReplayParseResult.Incomplete || tuple.Item1 == DataParser.ReplayParseResult.PreAlphaWipe || tuple.Item1 == DataParser.ReplayParseResult.TryMeMode)
                    {
                        text = tuple.Item1.ToString();
                    }
                    else if (!xWin)
                    {
                        text = "Loss";
                    }
                    else if (tuple.Item1 == DataParser.ReplayParseResult.Success && webClient.DownloadString("https://www.hotslogs.com/UploadFile?ReplayHash=" + guid) == DataParser.ReplayParseResult.Duplicate.ToString())
                    {
                        text = DataParser.ReplayParseResult.Duplicate.ToString();
                    }
                    else
                    {
                        using (AmazonS3Client amazonS3Client = new AmazonS3Client("AKIAIESBHEUH4KAAG4UA", "LJUzeVlvw1WX1TmxDqSaIZ9ZU04WQGcshPQyp21x", RegionEndpoint.USWest2))
                        {
                            this.labelUploading.Invoke(new Action(delegate
                            {
                                this.labelUploading.Visible = true;
                                this.labelUploading.Refresh();
                            }));
                            string text2 = Guid.NewGuid() + ".StormReplay";
                            if (amazonS3Client.PutObject(new PutObjectRequest
                            {
                                BucketName = "heroesreplays",
                                Key = text2,
                                FilePath = filePath
                            }).HttpStatusCode != HttpStatusCode.OK)
                            {
                                throw new Exception("Couldn't Upload Replay File: " + filePath);
                            }
                            text = webClient.DownloadString("https://www.hotslogs.com/UploadFile?FileName=" + text2);
                            if (text == "Maintenance")
                            {
                                MessageBox.Show("HOTSLogs.com is currently down for maintenance, please try uploading your replays at a later time.\r\n\r\nYou can check HOTSLogs.com for more information.  This application will now close.", "Maintenance");
                                base.Close();
                            }
                        }
                    }
                }
            }
            Settings @default = Settings.Default;
            @default.ReplaysUploaded = string.Concat(new object[]
            {
                @default.ReplaysUploaded,
                (Settings.Default.ReplaysUploaded != "") ? "?" : "",
                filePath,
                "*",
                text,
                "*",
                DateTime.Now
            });
            Settings.Default.Save();
        }

        private void SetDefaultReplayFolder()
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Heroes of the Storm\\Accounts");
            if (Directory.Exists(path))
            {
                this.replayFolder = path;
            }
            else
            {
                this.replayFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            }
            Settings.Default.ReplayDirectory = this.replayFolder;
            Settings.Default.Save();
        }

        private void EnableAutomaticReplayUpload()
        {
            this.backgroundWorker1.RunWorkerAsync();
        }

        private void BindDataGridViewSource()
        {
            if (Control.MouseButtons != MouseButtons.Left)
            {
                ReplayFile[] previouslyUploadedReplays = this.GetPreviouslyUploadedReplays();
                int firstDisplayedScrollingRowIndex = this.dataGridView1.FirstDisplayedScrollingRowIndex;
                IEnumerable<DataGridViewRow> arg_5F_0 = this.dataGridView1.SelectedRows.Cast<DataGridViewRow>();
                Func<DataGridViewRow, bool> arg_5F_1;
                if ((arg_5F_1 = Form1.ClassC.var9__18_0) == null)
                {
                    arg_5F_1 = (Form1.ClassC.var9__18_0 = new Func<DataGridViewRow, bool>(Form1.ClassC.var9.b__18_0));
                }
                IEnumerable<DataGridViewRow> arg_86_0 = arg_5F_0.Where(arg_5F_1);
                Func<DataGridViewRow, int> arg_86_1;
                if ((arg_86_1 = Form1.ClassC.var9__18_1) == null)
                {
                    arg_86_1 = (Form1.ClassC.var9__18_1 = new Func<DataGridViewRow, int>(Form1.ClassC.var9.b__18_1));
                }
                List<int> source = arg_86_0.Select(arg_86_1).ToList<int>();
                try
                {
                    DataGridView arg_E5_0 = this.dataGridView1;
                    IEnumerable<string> arg_CA_0 = Directory.GetFiles(this.replayFolder, "*.StormReplay", SearchOption.AllDirectories);
                    Func<string, DateTime> arg_CA_1;
                    if ((arg_CA_1 = Form1.ClassC.var9__18_2) == null)
                    {
                        arg_CA_1 = (Form1.ClassC.var9__18_2 = new Func<string, DateTime>(Form1.ClassC.var9.b__18_2));
                    }
                    arg_E5_0.DataSource = (from i in arg_CA_0.OrderByDescending(arg_CA_1)
                                           select new
                                           {
                                               FilePath = i,
                                               FileName = i.Split(new char[]
                                               {
                            '\\'
                                               })[i.Split(new char[]
                                               {
                            '\\'
                                               }).Length - 1],
                                               UploadStatus = (previouslyUploadedReplays.Any((ReplayFile j) => j.FileName == i) ? previouslyUploadedReplays.Single((ReplayFile j) => j.FileName == i).UploadStatus : ""),
                                               DateUploaded = (previouslyUploadedReplays.Any((ReplayFile j) => j.FileName == i) ? previouslyUploadedReplays.Single((ReplayFile j) => j.FileName == i).DateTimeUploaded : null)
                                           }).ToList();
                }
                catch
                {
                    MessageBox.Show("Couldn't load the replay folder.\r\n\r\nPlease select the path to the Heroes of the Storm replay folder.", "Error Loading Replay Folder");
                    this.SetReplayFolder();
                }
                foreach (int current in from i in source
                                        where i < this.dataGridView1.RowCount
                                        select i)
                {
                    this.dataGridView1.Rows[current].Selected = true;
                }
                if (firstDisplayedScrollingRowIndex < this.dataGridView1.RowCount && firstDisplayedScrollingRowIndex != -1)
                {
                    this.dataGridView1.FirstDisplayedScrollingRowIndex = firstDisplayedScrollingRowIndex;
                }
            }
        }

        private ReplayFile[] GetPreviouslyUploadedReplays()
        {
            if (Settings.Default.ReplaysUploaded != "")
            {
                try
                {
                    IEnumerable<string> arg_55_0 = Settings.Default.ReplaysUploaded.Split(new char[]
                    {
                        '?'
                    });
                    Func<string, string[]> arg_55_1;
                    if ((arg_55_1 = Form1.ClassC.var9__19_0) == null)
                    {
                        arg_55_1 = (Form1.ClassC.var9__19_0 = new Func<string, string[]>(Form1.ClassC.var9.b__19_0));
                    }
                    IEnumerable<string[]> arg_7C_0 = arg_55_0.Select(arg_55_1);
                    Func<string[], ReplayFile> arg_7C_1;
                    if ((arg_7C_1 = Form1.ClassC.var9__19_1) == null)
                    {
                        arg_7C_1 = (Form1.ClassC.var9__19_1 = new Func<string[], ReplayFile>(Form1.ClassC.var9.b__19_1));
                    }
                    return arg_7C_0.Select(arg_7C_1).ToArray<ReplayFile>();
                }
                catch
                {
                    Settings.Default.ReplaysUploaded = "";
                    Settings.Default.Save();
                }
            }
            return new ReplayFile[0];
        }

        private void SetPreviouslyUploadedReplays(ReplayFile[] replayFiles)
        {
            Settings arg_3C_0 = Settings.Default;
            string arg_37_0 = "?";
            Func<ReplayFile, string> arg_2D_1;
            if ((arg_2D_1 = Form1.ClassC.var9__20_0) == null)
            {
                arg_2D_1 = (Form1.ClassC.var9__20_0 = new Func<ReplayFile, string>(Form1.ClassC.var9.b__20_0));
            }
            arg_3C_0.ReplaysUploaded = string.Join(arg_37_0, replayFiles.Select(arg_2D_1).ToArray<string>());
            Settings.Default.Save();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.automaticReplayUpload = false;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (this.automaticReplayUpload)
            {
                Dictionary<string, string> replayFiles;
                ReplayFile[] previouslyUploadedReplays;
                if (this.formOptions.Visible)
                {
                    Thread.Sleep(0x1388);
                }
                else
                {
                    if (ClassC.var9__22_0 == null)
                    {
                    }
                    if (ClassC.var9__22_1 == null)
                    {
                    }
                    replayFiles = Directory.GetFiles(this.replayFolder, "*.StormReplay", SearchOption.AllDirectories).ToDictionary<string, string, string>(ClassC.var9__22_0 = new Func<string, string>(ClassC.var9.b__22_0), ClassC.var9__22_1 = new Func<string, string>(ClassC.var9.b__22_1));
                    previouslyUploadedReplays = this.GetPreviouslyUploadedReplays();
                    if (previouslyUploadedReplays.Any<ReplayFile>(i => !replayFiles.ContainsKey(i.FileName)))
                    {
                        this.SetPreviouslyUploadedReplays((from i in previouslyUploadedReplays
                                                           where replayFiles.ContainsKey(i.FileName)
                                                           select i).ToArray<ReplayFile>());
                    }
                    IEnumerable<string> source = from i in replayFiles.Keys
                                                 where !previouslyUploadedReplays.Any<ReplayFile>(j => (j.FileName == i))
                                                 select i;
                    if (source.Any<string>())
                    {
                        this.labelUploading.Invoke(new Action(delegate
                        {
                            this.labelUploading.Visible = true;
                            //this.labelUploading.Refresh();
                        }));
                        if (ClassC.var9__22_7 == null)
                        {
                        }
                        string filePath = source.OrderBy<string, DateTime>((ClassC.var9__22_7 = new Func<string, DateTime>(ClassC.var9.b__22_7))).First<string>();
                        this.UploadReplay(filePath);
                        base.Invoke(new RebindDataGridViewCallback(this.BindDataGridViewSource));
                        this.ShowBalloonTip("HOTS Logs", "Replay Uploaded: " + Path.GetFileNameWithoutExtension(filePath));
                        continue;
                    }
                    base.Invoke(new RebindDataGridViewCallback(this.BindDataGridViewSource));
                    this.labelUploading.Invoke(new Action(delegate
                    {
                        this.labelUploading.Visible = false;
                    }));
                    Thread.Sleep(0x1388);
                }
            }
        }

        private void UploadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.SetPreviouslyUploadedReplays((from i in this.GetPreviouslyUploadedReplays()
                                               where i.FileName != (string)this.dataGridView1.SelectedRows[0].Cells[0].Value
                                               select i).ToArray<ReplayFile>());
            this.BindDataGridViewSource();
        }

        private void viewProfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Settings.Default.PlayerID != -1)
            {
                using (Process.Start(new ProcessStartInfo("https://www.hotslogs.com/Player/Profile?PlayerID=" + Settings.Default.PlayerID)))
                {
                    return;
                }
            }
            this.formSetPlayerID.Show();
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.formOptions.Show();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.formAbout.Show();
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            base.Show();
            base.WindowState = FormWindowState.Normal;
            base.Focus();
        }

        private void hideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            base.Hide();
            this.ShowBalloonTip("HOTS Logs", "Application is now hidden, double-click to show it.");
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            base.Show();
            base.WindowState = FormWindowState.Normal;
            base.Focus();
        }

        private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex != -1)
            {
                this.dataGridView1.Rows[e.RowIndex].Selected = true;
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (base.WindowState == FormWindowState.Minimized)
            {
                this.hideToolStripMenuItem_Click(null, null);
            }
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
            this.components = new Container();
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(Form1));
            this.folderBrowserDialog1 = new FolderBrowserDialog();
            this.menuStrip1 = new MenuStrip();
            this.viewProfileToolStripMenuItem = new ToolStripMenuItem();
            this.optionsToolStripMenuItem = new ToolStripMenuItem();
            this.aboutToolStripMenuItem = new ToolStripMenuItem();
            this.dataGridView1 = new DataGridView();
            this.FilePath = new DataGridViewTextBoxColumn();
            this.UploadStatus = new DataGridViewTextBoxColumn();
            this.FileName = new DataGridViewTextBoxColumn();
            this.DateUploaded = new DataGridViewTextBoxColumn();
            this.contextMenuStrip2 = new ContextMenuStrip(this.components);
            this.UploadToolStripMenuItem = new ToolStripMenuItem();
            this.backgroundWorker1 = new BackgroundWorker();
            this.notifyIcon1 = new NotifyIcon(this.components);
            this.contextMenuStrip1 = new ContextMenuStrip(this.components);
            this.showToolStripMenuItem = new ToolStripMenuItem();
            this.hideToolStripMenuItem = new ToolStripMenuItem();
            this.toolStripSeparator1 = new ToolStripSeparator();
            this.exitToolStripMenuItem = new ToolStripMenuItem();
            this.pictureBox1 = new PictureBox();
            this.labelUploading = new Label();
            this.menuStrip1.SuspendLayout();
            ((ISupportInitialize)this.dataGridView1).BeginInit();
            this.contextMenuStrip2.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            ((ISupportInitialize)this.pictureBox1).BeginInit();
            base.SuspendLayout();
            this.folderBrowserDialog1.RootFolder = Environment.SpecialFolder.Personal;
            this.menuStrip1.BackColor = SystemColors.ButtonHighlight;
            this.menuStrip1.ImageScalingSize = new Size(24, 24);
            this.menuStrip1.Items.AddRange(new ToolStripItem[]
            {
                this.viewProfileToolStripMenuItem,
                this.optionsToolStripMenuItem,
                this.aboutToolStripMenuItem
            });
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new Padding(9, 3, 0, 3);
            this.menuStrip1.Size = new Size(786, 35);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            this.viewProfileToolStripMenuItem.Name = "viewProfileToolStripMenuItem";
            this.viewProfileToolStripMenuItem.Size = new Size(116, 29);
            this.viewProfileToolStripMenuItem.Text = "View Profile";
            this.viewProfileToolStripMenuItem.Click += new EventHandler(this.viewProfileToolStripMenuItem_Click);
            this.optionsToolStripMenuItem.Image = (Image)componentResourceManager.GetObject("optionsToolStripMenuItem.Image");
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new Size(112, 29);
            this.optionsToolStripMenuItem.Text = "Options";
            this.optionsToolStripMenuItem.Click += new EventHandler(this.optionsToolStripMenuItem_Click);
            this.aboutToolStripMenuItem.Image = (Image)componentResourceManager.GetObject("aboutToolStripMenuItem.Image");
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new Size(98, 29);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new EventHandler(this.aboutToolStripMenuItem_Click);
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeColumns = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.BackgroundColor = SystemColors.ButtonHighlight;
            this.dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridView1.Columns.AddRange(new DataGridViewColumn[]
            {
                this.FilePath,
                this.UploadStatus,
                this.FileName,
                this.DateUploaded
            });
            this.dataGridView1.ContextMenuStrip = this.contextMenuStrip2;
            this.dataGridView1.Dock = DockStyle.Bottom;
            this.dataGridView1.EditMode = DataGridViewEditMode.EditProgrammatically;
            this.dataGridView1.GridColor = SystemColors.ButtonFace;
            this.dataGridView1.Location = new System.Drawing.Point(0, 152);
            this.dataGridView1.Margin = new Padding(4, 5, 4, 5);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new Size(786, 662);
            this.dataGridView1.TabIndex = 1;
            this.dataGridView1.CellMouseDown += new DataGridViewCellMouseEventHandler(this.dataGridView1_CellMouseDown);
            this.FilePath.DataPropertyName = "FilePath";
            this.FilePath.HeaderText = "FilePath";
            this.FilePath.Name = "FilePath";
            this.FilePath.ReadOnly = true;
            this.FilePath.Visible = false;
            this.FilePath.Width = 50;
            this.UploadStatus.DataPropertyName = "UploadStatus";
            this.UploadStatus.HeaderText = "Status";
            this.UploadStatus.Name = "UploadStatus";
            this.UploadStatus.ReadOnly = true;
            this.UploadStatus.Resizable = DataGridViewTriState.False;
            this.UploadStatus.Width = 120;
            this.FileName.DataPropertyName = "FileName";
            this.FileName.HeaderText = "File Name";
            this.FileName.Name = "FileName";
            this.FileName.ReadOnly = true;
            this.FileName.Resizable = DataGridViewTriState.False;
            this.FileName.Width = 220;
            this.DateUploaded.DataPropertyName = "DateUploaded";
            this.DateUploaded.HeaderText = "Date Uploaded";
            this.DateUploaded.Name = "DateUploaded";
            this.DateUploaded.ReadOnly = true;
            this.DateUploaded.Resizable = DataGridViewTriState.False;
            this.DateUploaded.Width = 122;
            this.contextMenuStrip2.ImageScalingSize = new Size(24, 24);
            this.contextMenuStrip2.Items.AddRange(new ToolStripItem[]
            {
                this.UploadToolStripMenuItem
            });
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new Size(208, 34);
            this.UploadToolStripMenuItem.Image = (Image)componentResourceManager.GetObject("UploadToolStripMenuItem.Image");
            this.UploadToolStripMenuItem.Name = "UploadToolStripMenuItem";
            this.UploadToolStripMenuItem.Size = new Size(207, 30);
            this.UploadToolStripMenuItem.Text = "Upload Replay";
            this.UploadToolStripMenuItem.Click += new EventHandler(this.UploadToolStripMenuItem_Click);
            this.backgroundWorker1.DoWork += new DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Icon = (Icon)componentResourceManager.GetObject("notifyIcon1.Icon");
            this.notifyIcon1.Text = "HOTS Logs";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseDoubleClick += new MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            this.contextMenuStrip1.ImageScalingSize = new Size(24, 24);
            this.contextMenuStrip1.Items.AddRange(new ToolStripItem[]
            {
                this.showToolStripMenuItem,
                this.hideToolStripMenuItem,
                this.toolStripSeparator1,
                this.exitToolStripMenuItem
            });
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new Size(129, 100);
            this.showToolStripMenuItem.Name = "showToolStripMenuItem";
            this.showToolStripMenuItem.Size = new Size(128, 30);
            this.showToolStripMenuItem.Text = "Show";
            this.showToolStripMenuItem.Click += new EventHandler(this.showToolStripMenuItem_Click);
            this.hideToolStripMenuItem.Name = "hideToolStripMenuItem";
            this.hideToolStripMenuItem.Size = new Size(128, 30);
            this.hideToolStripMenuItem.Text = "Hide";
            this.hideToolStripMenuItem.Click += new EventHandler(this.hideToolStripMenuItem_Click);
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new Size(125, 6);
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new Size(128, 30);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new EventHandler(this.exitToolStripMenuItem_Click);
            this.pictureBox1.Image = (Image)componentResourceManager.GetObject("pictureBox1.Image");
            this.pictureBox1.Location = new System.Drawing.Point(240, 68);
            this.pictureBox1.Margin = new Padding(4, 5, 4, 5);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new Size(304, 72);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            this.labelUploading.AutoSize = true;
            this.labelUploading.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            this.labelUploading.Location = new System.Drawing.Point(666, 120);
            this.labelUploading.Margin = new Padding(4, 0, 4, 0);
            this.labelUploading.Name = "labelUploading";
            this.labelUploading.Size = new Size(107, 20);
            this.labelUploading.TabIndex = 3;
            this.labelUploading.Text = "Uploading...";
            base.AutoScaleDimensions = new SizeF(9f, 20f);
            base.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = SystemColors.ButtonHighlight;
            base.ClientSize = new Size(786, 814);
            base.Controls.Add(this.labelUploading);
            base.Controls.Add(this.pictureBox1);
            base.Controls.Add(this.dataGridView1);
            base.Controls.Add(this.menuStrip1);
            base.FormBorderStyle = FormBorderStyle.FixedDialog;
            base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
            base.Margin = new Padding(4, 5, 4, 5);
            base.MaximizeBox = false;
            base.Name = "Form1";
            base.StartPosition = FormStartPosition.CenterScreen;
            base.FormClosing += new FormClosingEventHandler(this.Form1_FormClosing);
            base.Load += new EventHandler(this.Form1_Load);
            base.Resize += new EventHandler(this.Form1_Resize);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((ISupportInitialize)this.dataGridView1).EndInit();
            this.contextMenuStrip2.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            ((ISupportInitialize)this.pictureBox1).EndInit();
            base.ResumeLayout(false);
            base.PerformLayout();
        }
    }
}
