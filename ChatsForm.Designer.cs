namespace Chats
{
    partial class ChatsForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      lb_Chats = new ListBox();
      wv_Chats = new Microsoft.Web.WebView2.WinForms.WebView2();
      tc = new TabControl();
      tp_Setup = new TabPage();
      gb_Utilities = new GroupBox();
      btn_ExtractGTalkFromMBox = new Button();
      gb_Data = new GroupBox();
      lbl_GTalkPath = new Label();
      btn_LoadData = new Button();
      tb_GChatPath = new TextBox();
      btn_ContactsPath = new Button();
      lbl_GChatPath = new Label();
      lbl_ContactsPath = new Label();
      btn_GChatPath = new Button();
      tb_ContactsPath = new TextBox();
      tb_GTalkPath = new TextBox();
      btn_GTalkPath = new Button();
      tp_Chats = new TabPage();
      sc_Chats = new SplitContainer();
      gb_Settings = new GroupBox();
      cb_TimeZone = new ComboBox();
      label1 = new Label();
      ((System.ComponentModel.ISupportInitialize)wv_Chats).BeginInit();
      tc.SuspendLayout();
      tp_Setup.SuspendLayout();
      gb_Utilities.SuspendLayout();
      gb_Data.SuspendLayout();
      tp_Chats.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)sc_Chats).BeginInit();
      sc_Chats.Panel1.SuspendLayout();
      sc_Chats.Panel2.SuspendLayout();
      sc_Chats.SuspendLayout();
      gb_Settings.SuspendLayout();
      SuspendLayout();
      // 
      // lb_Chats
      // 
      lb_Chats.Dock = DockStyle.Fill;
      lb_Chats.FormattingEnabled = true;
      lb_Chats.Location = new Point(0, 0);
      lb_Chats.Name = "lb_Chats";
      lb_Chats.Size = new Size(203, 602);
      lb_Chats.TabIndex = 1;
      // 
      // wv_Chats
      // 
      wv_Chats.AllowExternalDrop = true;
      wv_Chats.CreationProperties = null;
      wv_Chats.DefaultBackgroundColor = Color.White;
      wv_Chats.Dock = DockStyle.Fill;
      wv_Chats.Location = new Point(0, 0);
      wv_Chats.Name = "wv_Chats";
      wv_Chats.Size = new Size(1013, 602);
      wv_Chats.TabIndex = 2;
      wv_Chats.ZoomFactor = 1D;
      // 
      // tc
      // 
      tc.Controls.Add(tp_Setup);
      tc.Controls.Add(tp_Chats);
      tc.Dock = DockStyle.Fill;
      tc.Location = new Point(0, 0);
      tc.Name = "tc";
      tc.SelectedIndex = 0;
      tc.Size = new Size(1234, 636);
      tc.TabIndex = 3;
      // 
      // tp_Setup
      // 
      tp_Setup.Controls.Add(gb_Settings);
      tp_Setup.Controls.Add(gb_Utilities);
      tp_Setup.Controls.Add(gb_Data);
      tp_Setup.Location = new Point(4, 24);
      tp_Setup.Name = "tp_Setup";
      tp_Setup.Padding = new Padding(3);
      tp_Setup.Size = new Size(1226, 608);
      tp_Setup.TabIndex = 1;
      tp_Setup.Text = "Setup";
      tp_Setup.UseVisualStyleBackColor = true;
      // 
      // gb_Utilities
      // 
      gb_Utilities.Controls.Add(btn_ExtractGTalkFromMBox);
      gb_Utilities.Location = new Point(591, 6);
      gb_Utilities.Name = "gb_Utilities";
      gb_Utilities.Size = new Size(439, 359);
      gb_Utilities.TabIndex = 11;
      gb_Utilities.TabStop = false;
      gb_Utilities.Text = "Utilities";
      // 
      // btn_ExtractGTalkFromMBox
      // 
      btn_ExtractGTalkFromMBox.Location = new Point(6, 22);
      btn_ExtractGTalkFromMBox.Name = "btn_ExtractGTalkFromMBox";
      btn_ExtractGTalkFromMBox.Size = new Size(427, 23);
      btn_ExtractGTalkFromMBox.TabIndex = 6;
      btn_ExtractGTalkFromMBox.Text = "Extract Google Talk .eml from Google Takeouts GMail .mbox";
      btn_ExtractGTalkFromMBox.UseVisualStyleBackColor = true;
      // 
      // gb_Data
      // 
      gb_Data.Controls.Add(lbl_GTalkPath);
      gb_Data.Controls.Add(btn_LoadData);
      gb_Data.Controls.Add(tb_GChatPath);
      gb_Data.Controls.Add(btn_ContactsPath);
      gb_Data.Controls.Add(lbl_GChatPath);
      gb_Data.Controls.Add(lbl_ContactsPath);
      gb_Data.Controls.Add(btn_GChatPath);
      gb_Data.Controls.Add(tb_ContactsPath);
      gb_Data.Controls.Add(tb_GTalkPath);
      gb_Data.Controls.Add(btn_GTalkPath);
      gb_Data.Location = new Point(8, 6);
      gb_Data.Name = "gb_Data";
      gb_Data.Size = new Size(577, 359);
      gb_Data.TabIndex = 10;
      gb_Data.TabStop = false;
      gb_Data.Text = "Data";
      // 
      // lbl_GTalkPath
      // 
      lbl_GTalkPath.AutoSize = true;
      lbl_GTalkPath.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
      lbl_GTalkPath.Location = new Point(6, 19);
      lbl_GTalkPath.Name = "lbl_GTalkPath";
      lbl_GTalkPath.Size = new Size(301, 15);
      lbl_GTalkPath.TabIndex = 4;
      lbl_GTalkPath.Text = "Google Talk Folder Path (Directory containing .eml files)";
      // 
      // btn_LoadData
      // 
      btn_LoadData.Location = new Point(6, 307);
      btn_LoadData.Name = "btn_LoadData";
      btn_LoadData.Size = new Size(210, 44);
      btn_LoadData.TabIndex = 9;
      btn_LoadData.Text = "Load Data";
      btn_LoadData.UseVisualStyleBackColor = true;
      // 
      // tb_GChatPath
      // 
      tb_GChatPath.Location = new Point(6, 131);
      tb_GChatPath.Name = "tb_GChatPath";
      tb_GChatPath.Size = new Size(514, 23);
      tb_GChatPath.TabIndex = 0;
      // 
      // btn_ContactsPath
      // 
      btn_ContactsPath.Location = new Point(6, 256);
      btn_ContactsPath.Name = "btn_ContactsPath";
      btn_ContactsPath.Size = new Size(75, 23);
      btn_ContactsPath.TabIndex = 8;
      btn_ContactsPath.Text = "Pick...";
      btn_ContactsPath.UseVisualStyleBackColor = true;
      // 
      // lbl_GChatPath
      // 
      lbl_GChatPath.AutoSize = true;
      lbl_GChatPath.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
      lbl_GChatPath.Location = new Point(6, 109);
      lbl_GChatPath.Name = "lbl_GChatPath";
      lbl_GChatPath.Size = new Size(501, 15);
      lbl_GChatPath.TabIndex = 1;
      lbl_GChatPath.Text = "Google Chat Folder Path (Directory with subdirectories containing group_info, messages.json)";
      // 
      // lbl_ContactsPath
      // 
      lbl_ContactsPath.AutoSize = true;
      lbl_ContactsPath.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
      lbl_ContactsPath.Location = new Point(6, 205);
      lbl_ContactsPath.Name = "lbl_ContactsPath";
      lbl_ContactsPath.Size = new Size(308, 15);
      lbl_ContactsPath.TabIndex = 7;
      lbl_ContactsPath.Text = "Contacts File (A .json file format specific to this program)";
      // 
      // btn_GChatPath
      // 
      btn_GChatPath.Location = new Point(6, 160);
      btn_GChatPath.Name = "btn_GChatPath";
      btn_GChatPath.Size = new Size(75, 23);
      btn_GChatPath.TabIndex = 2;
      btn_GChatPath.Text = "Pick...";
      btn_GChatPath.UseVisualStyleBackColor = true;
      // 
      // tb_ContactsPath
      // 
      tb_ContactsPath.Location = new Point(6, 227);
      tb_ContactsPath.Name = "tb_ContactsPath";
      tb_ContactsPath.Size = new Size(514, 23);
      tb_ContactsPath.TabIndex = 6;
      // 
      // tb_GTalkPath
      // 
      tb_GTalkPath.Location = new Point(6, 41);
      tb_GTalkPath.Name = "tb_GTalkPath";
      tb_GTalkPath.Size = new Size(514, 23);
      tb_GTalkPath.TabIndex = 3;
      // 
      // btn_GTalkPath
      // 
      btn_GTalkPath.Location = new Point(6, 70);
      btn_GTalkPath.Name = "btn_GTalkPath";
      btn_GTalkPath.Size = new Size(75, 23);
      btn_GTalkPath.TabIndex = 5;
      btn_GTalkPath.Text = "Pick...";
      btn_GTalkPath.UseVisualStyleBackColor = true;
      // 
      // tp_Chats
      // 
      tp_Chats.Controls.Add(sc_Chats);
      tp_Chats.Location = new Point(4, 24);
      tp_Chats.Name = "tp_Chats";
      tp_Chats.Padding = new Padding(3);
      tp_Chats.Size = new Size(1226, 608);
      tp_Chats.TabIndex = 0;
      tp_Chats.Text = "Chats";
      tp_Chats.UseVisualStyleBackColor = true;
      // 
      // sc_Chats
      // 
      sc_Chats.Dock = DockStyle.Fill;
      sc_Chats.Location = new Point(3, 3);
      sc_Chats.Name = "sc_Chats";
      // 
      // sc_Chats.Panel1
      // 
      sc_Chats.Panel1.Controls.Add(lb_Chats);
      sc_Chats.Panel1MinSize = 50;
      // 
      // sc_Chats.Panel2
      // 
      sc_Chats.Panel2.Controls.Add(wv_Chats);
      sc_Chats.Panel2MinSize = 50;
      sc_Chats.Size = new Size(1220, 602);
      sc_Chats.SplitterDistance = 203;
      sc_Chats.TabIndex = 3;
      // 
      // gb_Settings
      // 
      gb_Settings.Controls.Add(label1);
      gb_Settings.Controls.Add(cb_TimeZone);
      gb_Settings.Location = new Point(8, 371);
      gb_Settings.Name = "gb_Settings";
      gb_Settings.Size = new Size(577, 229);
      gb_Settings.TabIndex = 12;
      gb_Settings.TabStop = false;
      gb_Settings.Text = "Settings";
      // 
      // cb_TimeZone
      // 
      cb_TimeZone.FormattingEnabled = true;
      cb_TimeZone.Location = new Point(6, 51);
      cb_TimeZone.Name = "cb_TimeZone";
      cb_TimeZone.Size = new Size(301, 23);
      cb_TimeZone.TabIndex = 0;
      // 
      // label1
      // 
      label1.AutoSize = true;
      label1.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
      label1.Location = new Point(6, 33);
      label1.Name = "label1";
      label1.Size = new Size(64, 15);
      label1.TabIndex = 10;
      label1.Text = "Time Zone";
      // 
      // ChatsForm
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(1234, 636);
      Controls.Add(tc);
      Name = "ChatsForm";
      StartPosition = FormStartPosition.CenterScreen;
      Text = "Chats";
      ((System.ComponentModel.ISupportInitialize)wv_Chats).EndInit();
      tc.ResumeLayout(false);
      tp_Setup.ResumeLayout(false);
      gb_Utilities.ResumeLayout(false);
      gb_Data.ResumeLayout(false);
      gb_Data.PerformLayout();
      tp_Chats.ResumeLayout(false);
      sc_Chats.Panel1.ResumeLayout(false);
      sc_Chats.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)sc_Chats).EndInit();
      sc_Chats.ResumeLayout(false);
      gb_Settings.ResumeLayout(false);
      gb_Settings.PerformLayout();
      ResumeLayout(false);
    }

    #endregion
    private ListBox lb_Chats;
    private Microsoft.Web.WebView2.WinForms.WebView2 wv_Chats;
    private TabControl tc;
    private TabPage tp_Chats;
    private TabPage tp_Setup;
    private Label lbl_GChatPath;
    private TextBox tb_GChatPath;
    private Button btn_GChatPath;
    private Button btn_GTalkPath;
    private Label lbl_GTalkPath;
    private TextBox tb_GTalkPath;
    private Button btn_ContactsPath;
    private Label lbl_ContactsPath;
    private TextBox tb_ContactsPath;
    private Button btn_LoadData;
    private GroupBox gb_Data;
    private GroupBox gb_Utilities;
    private Button btn_ExtractGTalkFromMBox;
    private SplitContainer sc_Chats;
    private GroupBox gb_Settings;
    private Label label1;
    private ComboBox cb_TimeZone;
  }
}
