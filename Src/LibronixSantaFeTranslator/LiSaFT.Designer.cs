using SIL.FieldWorks.TE.LibronixLinker;
namespace LibronixSantaFeTranslator
{
	partial class LiSaFT
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
				if (m_positionHandler != null)
					m_positionHandler.Dispose();
			}
			m_positionHandler = null;
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.NotifyIcon m_trayIcon;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LiSaFT));
			System.Windows.Forms.ContextMenuStrip m_contextMenuStrip;
			System.Windows.Forms.Button btnOk;
			System.Windows.Forms.Label label1;
			System.Windows.Forms.Button btnCancel;
			this.configToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.m_LinkSetCombo = new System.Windows.Forms.ComboBox();
			this.chkbStartLibronix = new System.Windows.Forms.CheckBox();
			m_trayIcon = new System.Windows.Forms.NotifyIcon(this.components);
			m_contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			btnOk = new System.Windows.Forms.Button();
			label1 = new System.Windows.Forms.Label();
			btnCancel = new System.Windows.Forms.Button();
			m_contextMenuStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// m_trayIcon
			// 
			m_trayIcon.ContextMenuStrip = m_contextMenuStrip;
			m_trayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("m_trayIcon.Icon")));
			m_trayIcon.Text = "Libronix Santa Fe Translator";
			m_trayIcon.Visible = true;
			// 
			// m_contextMenuStrip
			// 
			m_contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.configToolStripMenuItem,
            this.refreshToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
			m_contextMenuStrip.Name = "m_contextMenuStrip";
			m_contextMenuStrip.Size = new System.Drawing.Size(114, 76);
			// 
			// configToolStripMenuItem
			// 
			this.configToolStripMenuItem.Name = "configToolStripMenuItem";
			this.configToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
			this.configToolStripMenuItem.Text = "&Config";
			this.configToolStripMenuItem.Click += new System.EventHandler(this.OnConfig);
			// 
			// refreshToolStripMenuItem
			// 
			this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
			this.refreshToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
			this.refreshToolStripMenuItem.Text = "&Refresh";
			this.refreshToolStripMenuItem.Click += new System.EventHandler(this.OnRefresh);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(110, 6);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
			this.exitToolStripMenuItem.Text = "&Exit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.OnExit);
			// 
			// btnOk
			// 
			btnOk.Location = new System.Drawing.Point(13, 91);
			btnOk.Name = "btnOk";
			btnOk.Size = new System.Drawing.Size(75, 23);
			btnOk.TabIndex = 1;
			btnOk.Text = "OK";
			btnOk.UseVisualStyleBackColor = true;
			btnOk.Click += new System.EventHandler(this.OnOk);
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(10, 14);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(144, 13);
			label1.TabIndex = 2;
			label1.Text = "Libronix Link Set to sync with";
			// 
			// btnCancel
			// 
			btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			btnCancel.Location = new System.Drawing.Point(150, 91);
			btnCancel.Name = "btnCancel";
			btnCancel.Size = new System.Drawing.Size(75, 23);
			btnCancel.TabIndex = 1;
			btnCancel.Text = "Cancel";
			btnCancel.UseVisualStyleBackColor = true;
			btnCancel.Click += new System.EventHandler(this.OnCancel);
			// 
			// m_LinkSetCombo
			// 
			this.m_LinkSetCombo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.m_LinkSetCombo.FormattingEnabled = true;
			this.m_LinkSetCombo.Items.AddRange(new object[] {
            "Linkset A"});
			this.m_LinkSetCombo.Location = new System.Drawing.Point(13, 30);
			this.m_LinkSetCombo.Name = "m_LinkSetCombo";
			this.m_LinkSetCombo.Size = new System.Drawing.Size(212, 21);
			this.m_LinkSetCombo.TabIndex = 3;
			// 
			// chkbStartLibronix
			// 
			this.chkbStartLibronix.AutoSize = true;
			this.chkbStartLibronix.Location = new System.Drawing.Point(13, 68);
			this.chkbStartLibronix.Name = "chkbStartLibronix";
			this.chkbStartLibronix.Size = new System.Drawing.Size(214, 17);
			this.chkbStartLibronix.TabIndex = 4;
			this.chkbStartLibronix.Text = "On Startup start Libronix if it isn\'t running";
			this.chkbStartLibronix.UseVisualStyleBackColor = true;
			// 
			// LiSaFT
			// 
			this.AcceptButton = btnOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = btnCancel;
			this.ClientSize = new System.Drawing.Size(237, 126);
			this.Controls.Add(this.chkbStartLibronix);
			this.Controls.Add(this.m_LinkSetCombo);
			this.Controls.Add(label1);
			this.Controls.Add(btnCancel);
			this.Controls.Add(btnOk);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "LiSaFT";
			this.ShowInTaskbar = false;
			this.Text = "Libronix Santa Fe Translator";
			this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
			m_contextMenuStrip.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStripMenuItem configToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.ComboBox m_LinkSetCombo;
		private System.Windows.Forms.CheckBox chkbStartLibronix;

	}
}

