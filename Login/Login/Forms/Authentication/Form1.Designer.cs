namespace Login
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null!;
        private System.Windows.Forms.PictureBox picPoster = null!;
        private System.Windows.Forms.PictureBox picInfoIcon = null!;
        private Login.Controls.CustomInputBox txtUsername = null!;
        private Login.Controls.CustomInputBox txtPassword = null!;
        private Login.Controls.ModernButton btnLogin = null!;
        private Login.Controls.RoleCardButton btnRoleAdmin = null!;
        private Login.Controls.RoleCardButton btnRoleManager = null!;
        private Login.Controls.RoleCardButton btnRoleStaff = null!;
        private Login.Controls.ModernCheckBox chkRemember = null!;
        private System.Windows.Forms.LinkLabel lnkForgotPassword = null!;
        private TableLayoutPanel rootLayout = null!;
        private TableLayoutPanel loginContentHost = null!;
        private Panel loginContentPanel = null!;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
                picPoster?.Image?.Dispose();
                picInfoIcon?.Image?.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            rootLayout = new TableLayoutPanel();
            posterPanel = new Panel();
            picPoster = new PictureBox();
            lblBrandSub = new Label();
            lblBrand = new Label();
            loginCard = new Panel();
            loginContentHost = new TableLayoutPanel();
            loginContentPanel = new Panel();
            picInfoIcon = new PictureBox();
            lblInfo = new Label();
            btnRoleStaff = new Login.Controls.RoleCardButton();
            btnRoleManager = new Login.Controls.RoleCardButton();
            btnRoleAdmin = new Login.Controls.RoleCardButton();
            divider = new Login.Controls.DividerLabel();
            btnLogin = new Login.Controls.ModernButton();
            lnkForgotPassword = new LinkLabel();
            chkRemember = new Login.Controls.ModernCheckBox();
            txtPassword = new Login.Controls.CustomInputBox();
            txtUsername = new Login.Controls.CustomInputBox();
            lblSubtitle = new Label();
            lblTitle = new Label();
            rootLayout.SuspendLayout();
            posterPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picPoster).BeginInit();
            loginCard.SuspendLayout();
            loginContentHost.SuspendLayout();
            loginContentPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picInfoIcon).BeginInit();
            SuspendLayout();
            // 
            // rootLayout
            // 
            rootLayout.AutoScroll = true;
            rootLayout.ColumnCount = 2;
            rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 43F));
            rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 57F));
            rootLayout.Controls.Add(posterPanel, 0, 0);
            rootLayout.Controls.Add(loginCard, 1, 0);
            rootLayout.Dock = DockStyle.Fill;
            rootLayout.Location = new Point(0, 0);
            rootLayout.Margin = new Padding(0);
            rootLayout.Name = "rootLayout";
            rootLayout.Padding = new Padding(24);
            rootLayout.RowCount = 1;
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            rootLayout.Size = new Size(1050, 681);
            rootLayout.TabIndex = 0;
            // 
            // posterPanel
            // 
            posterPanel.BackColor = Color.FromArgb(68, 34, 14);
            posterPanel.Controls.Add(picPoster);
            posterPanel.Controls.Add(lblBrandSub);
            posterPanel.Controls.Add(lblBrand);
            posterPanel.Dock = DockStyle.Fill;
            posterPanel.Location = new Point(24, 24);
            posterPanel.Margin = new Padding(0, 0, 14, 0);
            posterPanel.MinimumSize = new Size(200, 300);
            posterPanel.Name = "posterPanel";
            posterPanel.Size = new Size(416, 633);
            posterPanel.TabIndex = 1;
            // 
            // picPoster
            // 
            picPoster.BackColor = Color.FromArgb(68, 34, 14);
            picPoster.Dock = DockStyle.Fill;
            picPoster.Location = new Point(0, 0);
            picPoster.Name = "picPoster";
            picPoster.Size = new Size(416, 633);
            picPoster.SizeMode = PictureBoxSizeMode.Zoom;
            picPoster.TabIndex = 0;
            picPoster.TabStop = false;
            // 
            // lblBrandSub
            // 
            lblBrandSub.AutoSize = true;
            lblBrandSub.BackColor = Color.Transparent;
            lblBrandSub.Font = new Font("Segoe UI", 10F);
            lblBrandSub.ForeColor = Color.FromArgb(255, 231, 204);
            lblBrandSub.Location = new Point(31, 80);
            lblBrandSub.Name = "lblBrandSub";
            lblBrandSub.Size = new Size(176, 19);
            lblBrandSub.TabIndex = 1;
            lblBrandSub.Text = "Hệ thống quản lý cửa hàng";
            // 
            // lblBrand
            // 
            lblBrand.AutoSize = true;
            lblBrand.BackColor = Color.Transparent;
            lblBrand.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            lblBrand.ForeColor = Color.White;
            lblBrand.Location = new Point(28, 36);
            lblBrand.Name = "lblBrand";
            lblBrand.Size = new Size(225, 37);
            lblBrand.TabIndex = 2;
            lblBrand.Text = "BÁNH MÌ NGON";
            // 
            // loginCard
            // 
            loginCard.BackColor = Color.White;
            loginCard.Controls.Add(loginContentHost);
            loginCard.Dock = DockStyle.Fill;
            loginCard.Location = new Point(468, 24);
            loginCard.Margin = new Padding(14, 0, 0, 0);
            loginCard.MinimumSize = new Size(460, 300);
            loginCard.Name = "loginCard";
            loginCard.Size = new Size(558, 633);
            loginCard.TabIndex = 0;
            // 
            // loginContentHost
            // 
            loginContentHost.ColumnCount = 1;
            loginContentHost.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            loginContentHost.Controls.Add(loginContentPanel, 0, 0);
            loginContentHost.Dock = DockStyle.Fill;
            loginContentHost.Location = new Point(0, 0);
            loginContentHost.Margin = new Padding(0);
            loginContentHost.Name = "loginContentHost";
            loginContentHost.RowCount = 1;
            loginContentHost.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            loginContentHost.Size = new Size(558, 633);
            loginContentHost.TabIndex = 0;
            // 
            // loginContentPanel
            // 
            loginContentPanel.AutoScroll = true;
            loginContentPanel.Controls.Add(picInfoIcon);
            loginContentPanel.Controls.Add(lblInfo);
            loginContentPanel.Controls.Add(btnRoleStaff);
            loginContentPanel.Controls.Add(btnRoleManager);
            loginContentPanel.Controls.Add(btnRoleAdmin);
            loginContentPanel.Controls.Add(divider);
            loginContentPanel.Controls.Add(btnLogin);
            loginContentPanel.Controls.Add(lnkForgotPassword);
            loginContentPanel.Controls.Add(chkRemember);
            loginContentPanel.Controls.Add(txtPassword);
            loginContentPanel.Controls.Add(txtUsername);
            loginContentPanel.Controls.Add(lblSubtitle);
            loginContentPanel.Controls.Add(lblTitle);
            loginContentPanel.Dock = DockStyle.Fill;
            loginContentPanel.Location = new Point(0, 0);
            loginContentPanel.Margin = new Padding(0);
            loginContentPanel.Name = "loginContentPanel";
            loginContentPanel.Size = new Size(558, 633);
            loginContentPanel.TabIndex = 0;
            // 
            // picInfoIcon
            // 
            picInfoIcon.Location = new Point(68, 576);
            picInfoIcon.Name = "picInfoIcon";
            picInfoIcon.Size = new Size(22, 22);
            picInfoIcon.SizeMode = PictureBoxSizeMode.Zoom;
            picInfoIcon.TabIndex = 1;
            picInfoIcon.TabStop = false;
            // 
            // lblInfo
            // 
            lblInfo.Font = new Font("Segoe UI", 8.75F);
            lblInfo.ForeColor = Color.FromArgb(132, 87, 52);
            lblInfo.Location = new Point(100, 573);
            lblInfo.Name = "lblInfo";
            lblInfo.Size = new Size(352, 38);
            lblInfo.TabIndex = 2;
            lblInfo.Text = "Liên hệ quản trị viên nếu bạn không có tài khoản đăng nhập.";
            lblInfo.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // btnRoleStaff
            // 
            btnRoleStaff.Font = new Font("Segoe UI", 9.5F);
            btnRoleStaff.IsSelected = false;
            btnRoleStaff.Location = new Point(332, 460);
            btnRoleStaff.Name = "btnRoleStaff";
            btnRoleStaff.RoleIcon = null;
            btnRoleStaff.RoleName = "Nhân viên";
            btnRoleStaff.Size = new Size(120, 80);
            btnRoleStaff.TabIndex = 3;
            // 
            // btnRoleManager
            // 
            btnRoleManager.Font = new Font("Segoe UI", 9.5F);
            btnRoleManager.IsSelected = false;
            btnRoleManager.Location = new Point(198, 460);
            btnRoleManager.Name = "btnRoleManager";
            btnRoleManager.RoleIcon = null;
            btnRoleManager.RoleName = "Quản lý";
            btnRoleManager.Size = new Size(120, 80);
            btnRoleManager.TabIndex = 4;
            // 
            // btnRoleAdmin
            // 
            btnRoleAdmin.Font = new Font("Segoe UI", 9.5F);
            btnRoleAdmin.IsSelected = true;
            btnRoleAdmin.Location = new Point(64, 460);
            btnRoleAdmin.Name = "btnRoleAdmin";
            btnRoleAdmin.RoleIcon = null;
            btnRoleAdmin.RoleName = "Quản trị viên";
            btnRoleAdmin.Size = new Size(120, 80);
            btnRoleAdmin.TabIndex = 5;
            // 
            // divider
            // 
            divider.BackColor = Color.Transparent;
            divider.Font = new Font("Segoe UI", 9F);
            divider.LineColor = Color.FromArgb(229, 231, 235);
            divider.Location = new Point(64, 420);
            divider.Name = "divider";
            divider.Size = new Size(390, 26);
            divider.TabIndex = 6;
            divider.TextColor = Color.FromArgb(156, 163, 175);
            // 
            // btnLogin
            // 
            btnLogin.BackColor = Color.Transparent;
            btnLogin.BorderColor = Color.Transparent;
            btnLogin.BorderRadius = 8;
            btnLogin.BorderWidth = 0;
            btnLogin.ButtonIcon = null;
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnLogin.ForeColor = Color.White;
            btnLogin.HoverColor = Color.FromArgb(140, 70, 20);
            btnLogin.Location = new Point(64, 352);
            btnLogin.Name = "btnLogin";
            btnLogin.NormalColor = Color.FromArgb(174, 88, 24);
            btnLogin.PressedColor = Color.FromArgb(115, 53, 12);
            btnLogin.Size = new Size(390, 52);
            btnLogin.TabIndex = 7;
            btnLogin.Text = "Đăng nhập";
            btnLogin.UseVisualStyleBackColor = false;
            // 
            // lnkForgotPassword
            // 
            lnkForgotPassword.AutoSize = true;
            lnkForgotPassword.Font = new Font("Segoe UI", 9F);
            lnkForgotPassword.LinkColor = Color.FromArgb(37, 99, 235);
            lnkForgotPassword.Location = new Point(338, 316);
            lnkForgotPassword.Name = "lnkForgotPassword";
            lnkForgotPassword.Size = new Size(94, 15);
            lnkForgotPassword.TabIndex = 8;
            lnkForgotPassword.TabStop = true;
            lnkForgotPassword.Text = "Quên mật khẩu?";
            // 
            // chkRemember
            // 
            chkRemember.BackColor = Color.Transparent;
            chkRemember.BoxCheckedColor = Color.FromArgb(37, 99, 235);
            chkRemember.Checked = true;
            chkRemember.Font = new Font("Segoe UI", 9.5F);
            chkRemember.Location = new Point(64, 312);
            chkRemember.Name = "chkRemember";
            chkRemember.Size = new Size(170, 26);
            chkRemember.TabIndex = 9;
            // 
            // txtPassword
            // 
            txtPassword.BackColor = Color.White;
            txtPassword.InputText = "";
            txtPassword.IsPassword = true;
            txtPassword.LeftIcon = null;
            txtPassword.Location = new Point(64, 248);
            txtPassword.Name = "txtPassword";
            txtPassword.PlaceholderText = "";
            txtPassword.ShowPasswordToggle = true;
            txtPassword.Size = new Size(390, 52);
            txtPassword.TabIndex = 10;
            // 
            // txtUsername
            // 
            txtUsername.BackColor = Color.White;
            txtUsername.InputText = "admin";
            txtUsername.IsPassword = false;
            txtUsername.LeftIcon = null;
            txtUsername.Location = new Point(64, 184);
            txtUsername.Name = "txtUsername";
            txtUsername.PlaceholderText = "Tên đăng nhập";
            txtUsername.ShowPasswordToggle = false;
            txtUsername.Size = new Size(390, 52);
            txtUsername.TabIndex = 11;
            // 
            // lblSubtitle
            // 
            lblSubtitle.Font = new Font("Segoe UI", 9.5F);
            lblSubtitle.ForeColor = Color.FromArgb(135, 120, 110);
            lblSubtitle.Location = new Point(40, 118);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new Size(440, 30);
            lblSubtitle.TabIndex = 12;
            lblSubtitle.Text = "Vui lòng nhập tài khoản và mật khẩu để tiếp tục";
            lblSubtitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblTitle
            // 
            lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(128, 62, 19);
            lblTitle.Location = new Point(40, 62);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(440, 46);
            lblTitle.TabIndex = 13;
            lblTitle.Text = "ĐĂNG NHẬP HỆ THỐNG";
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(250, 247, 242);
            ClientSize = new Size(1050, 681);
            Controls.Add(rootLayout);
            Font = new Font("Segoe UI", 9.5F);
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;
            MinimumSize = new Size(900, 650);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Đăng nhập hệ thống – Bánh Mì Ngon";
            rootLayout.ResumeLayout(false);
            posterPanel.ResumeLayout(false);
            posterPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picPoster).EndInit();
            loginCard.ResumeLayout(false);
            loginContentHost.ResumeLayout(false);
            loginContentPanel.ResumeLayout(false);
            loginContentPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picInfoIcon).EndInit();
            ResumeLayout(false);
        }

        private Panel posterPanel;
        private Label lblBrandSub;
        private Label lblBrand;
        private Panel loginCard;
        private Label lblInfo;
        private Controls.DividerLabel divider;
        private Label lblSubtitle;
        private Label lblTitle;
    }
}
