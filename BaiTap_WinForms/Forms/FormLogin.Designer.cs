namespace BaiTap_WinForms.Forms
{
    partial class FormLogin
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label lblIcon;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubtitle;
        private System.Windows.Forms.Panel panelCard;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.CheckBox chkShowPassword;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnTestConn;
        private System.Windows.Forms.Label lblServerHint;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.panelTop = new System.Windows.Forms.Panel();
            this.lblIcon = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblSubtitle = new System.Windows.Forms.Label();
            this.panelCard = new System.Windows.Forms.Panel();
            this.lblUsername = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.chkShowPassword = new System.Windows.Forms.CheckBox();
            this.btnLogin = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnTestConn = new System.Windows.Forms.Button();
            this.lblServerHint = new System.Windows.Forms.Label();

            // ── FormLogin ──────────────────────────────────────────────
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(520, 560);
            this.BackColor = System.Drawing.Color.FromArgb(243, 244, 246);
            this.Text = "Đăng Nhập - Hệ Thống Quán Bánh Mì";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Controls.Add(this.panelCard);
            this.Controls.Add(this.panelTop);
            this.Load += new System.EventHandler(this.FormLogin_Load);

            // ── panelTop ──────────────────────────
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Height = 148;
            this.panelTop.BackColor = System.Drawing.Color.FromArgb(37, 99, 235);
            this.panelTop.Controls.Add(this.lblIcon);
            this.panelTop.Controls.Add(this.lblTitle);
            this.panelTop.Controls.Add(this.lblSubtitle);

            // lblIcon – emoji bánh mì
            this.lblIcon.Text = "🥪";
            this.lblIcon.Font = new System.Drawing.Font("Segoe UI Emoji", 28F);
            this.lblIcon.ForeColor = System.Drawing.Color.White;
            this.lblIcon.Location = new System.Drawing.Point(0, 10);
            this.lblIcon.Size = new System.Drawing.Size(520, 46);
            this.lblIcon.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // lblTitle
            this.lblTitle.Text = "TIỆM BÁNH MÌ NGON";
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(0, 60);
            this.lblTitle.Size = new System.Drawing.Size(520, 34);
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // lblSubtitle
            this.lblSubtitle.Text = "Hệ thống Quản lý Bán hàng & Tồn kho";
            this.lblSubtitle.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(254, 215, 190);
            this.lblSubtitle.Location = new System.Drawing.Point(0, 98);
            this.lblSubtitle.Size = new System.Drawing.Size(520, 36);
            this.lblSubtitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // ── panelCard ───────────
            this.panelCard.Location = new System.Drawing.Point(40, 164);
            this.panelCard.Size = new System.Drawing.Size(440, 372);
            this.panelCard.BackColor = System.Drawing.Color.White;
            this.panelCard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelCard.Controls.Add(this.lblUsername);
            this.panelCard.Controls.Add(this.txtUsername);
            this.panelCard.Controls.Add(this.lblPassword);
            this.panelCard.Controls.Add(this.txtPassword);
            this.panelCard.Controls.Add(this.chkShowPassword);
            this.panelCard.Controls.Add(this.btnLogin);
            this.panelCard.Controls.Add(this.btnTestConn);
            this.panelCard.Controls.Add(this.btnExit);
            this.panelCard.Controls.Add(this.lblServerHint);

            // lblUsername
            this.lblUsername.Text = "👤  Tên đăng nhập:";
            this.lblUsername.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblUsername.ForeColor = System.Drawing.Color.FromArgb(55, 65, 81);
            this.lblUsername.Location = new System.Drawing.Point(30, 22);
            this.lblUsername.AutoSize = true;

            // txtUsername
            this.txtUsername.Location = new System.Drawing.Point(30, 48);
            this.txtUsername.Size = new System.Drawing.Size(378, 32);
            this.txtUsername.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtUsername.Text = "admin";

            // lblPassword
            this.lblPassword.Text = "🔒  Mật khẩu:";
            this.lblPassword.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblPassword.ForeColor = System.Drawing.Color.FromArgb(55, 65, 81);
            this.lblPassword.Location = new System.Drawing.Point(30, 100);
            this.lblPassword.AutoSize = true;

            // txtPassword
            this.txtPassword.Location = new System.Drawing.Point(30, 126);
            this.txtPassword.Size = new System.Drawing.Size(378, 32);
            this.txtPassword.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtPassword.UseSystemPasswordChar = true;
            this.txtPassword.Text = "123456";

            // chkShowPassword
            this.chkShowPassword.Text = "Hiện mật khẩu";
            this.chkShowPassword.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.chkShowPassword.ForeColor = System.Drawing.Color.FromArgb(107, 114, 128);
            this.chkShowPassword.Location = new System.Drawing.Point(30, 170);
            this.chkShowPassword.AutoSize = true;
            this.chkShowPassword.CheckedChanged += new System.EventHandler(this.chkShowPassword_CheckedChanged);

            // btnLogin
            this.btnLogin.Text = "ĐĂNG NHẬP HỆ THỐNG";
            this.btnLogin.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnLogin.BackColor = System.Drawing.Color.FromArgb(37, 99, 235);
            this.btnLogin.ForeColor = System.Drawing.Color.White;
            this.btnLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogin.FlatAppearance.BorderSize = 0;
            this.btnLogin.Location = new System.Drawing.Point(30, 208);
            this.btnLogin.Size = new System.Drawing.Size(378, 46);
            this.btnLogin.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);

            // btnTestConn
            this.btnTestConn.Text = "⚡ Test Kết Nối SQL";
            this.btnTestConn.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnTestConn.BackColor = System.Drawing.Color.FromArgb(243, 244, 246);
            this.btnTestConn.ForeColor = System.Drawing.Color.FromArgb(31, 41, 55);
            this.btnTestConn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTestConn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(209, 213, 219);
            this.btnTestConn.Location = new System.Drawing.Point(30, 270);
            this.btnTestConn.Size = new System.Drawing.Size(185, 40);
            this.btnTestConn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTestConn.Click += new System.EventHandler(this.btnTestConn_Click);

            // btnExit
            this.btnExit.Text = "❌ Thoát";
            this.btnExit.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.btnExit.BackColor = System.Drawing.Color.FromArgb(243, 244, 246);
            this.btnExit.ForeColor = System.Drawing.Color.FromArgb(31, 41, 55);
            this.btnExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExit.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(209, 213, 219);
            this.btnExit.Location = new System.Drawing.Point(223, 270);
            this.btnExit.Size = new System.Drawing.Size(185, 40);
            this.btnExit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);

            // lblServerHint
            this.lblServerHint.Text = "💡 Mặc định: admin / 123456 hoặc staff01 / 123456";
            this.lblServerHint.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Italic);
            this.lblServerHint.ForeColor = System.Drawing.Color.FromArgb(107, 114, 128);
            this.lblServerHint.Location = new System.Drawing.Point(30, 326);
            this.lblServerHint.Size = new System.Drawing.Size(378, 26);
            this.lblServerHint.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        }
    }
}
