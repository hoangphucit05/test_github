namespace Login
{
    public partial class AdminDashboardForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null!;

        // Layout containers
        private System.Windows.Forms.TableLayoutPanel tableRoot = null!;
        private System.Windows.Forms.Panel _sidebar = null!;
        private System.Windows.Forms.Panel logoPanel = null!;
        private System.Windows.Forms.PictureBox picLogo = null!;
        private System.Windows.Forms.Label logoLabel = null!;
        private System.Windows.Forms.Label logoSub = null!;
        private System.Windows.Forms.Panel logoSpacer = null!;
        private System.Windows.Forms.Panel logoutPanel = null!;
        private Login.Controls.SidebarNavButton _logoutBtn = null!;

        // Sidebar Navigation buttons
        private Login.Controls.SidebarNavButton _navHome = null!;
        private Login.Controls.SidebarNavButton _navUsers = null!;
        private Login.Controls.SidebarNavButton _navRoles = null!;
        private Login.Controls.SidebarNavButton _navBranches = null!;
        private Login.Controls.SidebarNavButton _navShifts = null!;

        // Right side layout (Topbar + Content)
        private System.Windows.Forms.TableLayoutPanel tableRight = null!;
        private System.Windows.Forms.Panel _topbar = null!;
        private System.Windows.Forms.Panel breadPanel = null!;
        private System.Windows.Forms.PictureBox picTopBread = null!;
        private System.Windows.Forms.Label breadLabel = null!;
        private System.Windows.Forms.Label lblUserName = null!;
        private System.Windows.Forms.Label lblUserRole = null!;
        private System.Windows.Forms.Panel avatarPanel = null!;

        // Main dynamic content host
        private System.Windows.Forms.Panel _contentPanel = null!;

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
            tableRoot = new TableLayoutPanel();
            _sidebar = new Panel();
            _navShifts = new Login.Controls.SidebarNavButton();
            _navBranches = new Login.Controls.SidebarNavButton();
            _navRoles = new Login.Controls.SidebarNavButton();
            _navUsers = new Login.Controls.SidebarNavButton();
            _navHome = new Login.Controls.SidebarNavButton();
            logoSpacer = new Panel();
            logoPanel = new Panel();
            picLogo = new PictureBox();
            logoSub = new Label();
            logoLabel = new Label();
            logoutPanel = new Panel();
            _logoutBtn = new Login.Controls.SidebarNavButton();
            tableRight = new TableLayoutPanel();
            _topbar = new Panel();
            avatarPanel = new Panel();
            lblUserRole = new Label();
            lblUserName = new Label();
            breadPanel = new Panel();
            picTopBread = new PictureBox();
            breadLabel = new Label();
            _contentPanel = new Panel();
            tableRoot.SuspendLayout();
            _sidebar.SuspendLayout();
            logoPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picLogo).BeginInit();
            logoutPanel.SuspendLayout();
            tableRight.SuspendLayout();
            _topbar.SuspendLayout();
            breadPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picTopBread).BeginInit();
            SuspendLayout();
            // 
            // tableRoot
            // 
            tableRoot.BackColor = Color.FromArgb(248, 245, 240);
            tableRoot.ColumnCount = 2;
            tableRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 230F));
            tableRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableRoot.Controls.Add(_sidebar, 0, 0);
            tableRoot.Controls.Add(tableRight, 1, 0);
            tableRoot.Dock = DockStyle.Fill;
            tableRoot.Location = new Point(0, 0);
            tableRoot.Margin = new Padding(0);
            tableRoot.Name = "tableRoot";
            tableRoot.RowCount = 1;
            tableRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableRoot.Size = new Size(1280, 780);
            tableRoot.TabIndex = 0;
            // 
            // _sidebar
            // 
            _sidebar.BackColor = Color.FromArgb(117, 58, 18);
            _sidebar.Controls.Add(_navShifts);
            _sidebar.Controls.Add(_navBranches);
            _sidebar.Controls.Add(_navRoles);
            _sidebar.Controls.Add(_navUsers);
            _sidebar.Controls.Add(_navHome);
            _sidebar.Controls.Add(logoSpacer);
            _sidebar.Controls.Add(logoPanel);
            _sidebar.Controls.Add(logoutPanel);
            _sidebar.Dock = DockStyle.Fill;
            _sidebar.Location = new Point(0, 0);
            _sidebar.Margin = new Padding(0);
            _sidebar.Name = "_sidebar";
            _sidebar.Padding = new Padding(0, 8, 0, 0);
            _sidebar.Size = new Size(230, 780);
            _sidebar.TabIndex = 0;
            // 
            // _navShifts
            // 
            _navShifts.BackColor = Color.Transparent;
            _navShifts.Cursor = Cursors.Hand;
            _navShifts.Dock = DockStyle.Top;
            _navShifts.Font = new Font("Segoe UI", 9.75F);
            _navShifts.IsSelected = false;
            _navShifts.Label = "Quản lý ca làm";
            _navShifts.Location = new Point(0, 292);
            _navShifts.Name = "_navShifts";
            _navShifts.NavIcon = null;
            _navShifts.Size = new Size(230, 48);
            _navShifts.TabIndex = 6;
            // 
            // _navBranches
            // 
            _navBranches.BackColor = Color.Transparent;
            _navBranches.Cursor = Cursors.Hand;
            _navBranches.Dock = DockStyle.Top;
            _navBranches.Font = new Font("Segoe UI", 9.75F);
            _navBranches.IsSelected = false;
            _navBranches.Label = "Quản lý chi nhánh";
            _navBranches.Location = new Point(0, 244);
            _navBranches.Name = "_navBranches";
            _navBranches.NavIcon = null;
            _navBranches.Size = new Size(230, 48);
            _navBranches.TabIndex = 5;
            // 
            // _navRoles
            // 
            _navRoles.BackColor = Color.Transparent;
            _navRoles.Cursor = Cursors.Hand;
            _navRoles.Dock = DockStyle.Top;
            _navRoles.Font = new Font("Segoe UI", 9.75F);
            _navRoles.IsSelected = false;
            _navRoles.Label = "Phân quyền vai trò";
            _navRoles.Location = new Point(0, 196);
            _navRoles.Name = "_navRoles";
            _navRoles.NavIcon = null;
            _navRoles.Size = new Size(230, 48);
            _navRoles.TabIndex = 4;
            // 
            // _navUsers
            // 
            _navUsers.BackColor = Color.Transparent;
            _navUsers.Cursor = Cursors.Hand;
            _navUsers.Dock = DockStyle.Top;
            _navUsers.Font = new Font("Segoe UI", 9.75F);
            _navUsers.IsSelected = false;
            _navUsers.Label = "Quản lý tài khoản";
            _navUsers.Location = new Point(0, 148);
            _navUsers.Name = "_navUsers";
            _navUsers.NavIcon = null;
            _navUsers.Size = new Size(230, 48);
            _navUsers.TabIndex = 3;
            _navUsers.Click += _navUsers_Click;
            // 
            // _navHome
            // 
            _navHome.BackColor = Color.Transparent;
            _navHome.Cursor = Cursors.Hand;
            _navHome.Dock = DockStyle.Top;
            _navHome.Font = new Font("Segoe UI", 9.75F);
            _navHome.IsSelected = true;
            _navHome.Label = "Trang chủ";
            _navHome.Location = new Point(0, 100);
            _navHome.Name = "_navHome";
            _navHome.NavIcon = null;
            _navHome.Size = new Size(230, 48);
            _navHome.TabIndex = 2;
            // 
            // logoSpacer
            // 
            logoSpacer.BackColor = Color.Transparent;
            logoSpacer.Dock = DockStyle.Top;
            logoSpacer.Location = new Point(0, 88);
            logoSpacer.Name = "logoSpacer";
            logoSpacer.Size = new Size(230, 12);
            logoSpacer.TabIndex = 1;
            // 
            // logoPanel
            // 
            logoPanel.BackColor = Color.FromArgb(90, 40, 10);
            logoPanel.Controls.Add(picLogo);
            logoPanel.Controls.Add(logoSub);
            logoPanel.Controls.Add(logoLabel);
            logoPanel.Dock = DockStyle.Top;
            logoPanel.Location = new Point(0, 8);
            logoPanel.Name = "logoPanel";
            logoPanel.Size = new Size(230, 80);
            logoPanel.TabIndex = 0;
            // 
            // picLogo
            // 
            picLogo.BackColor = Color.Transparent;
            picLogo.Location = new Point(18, 22);
            picLogo.Name = "picLogo";
            picLogo.Size = new Size(36, 36);
            picLogo.SizeMode = PictureBoxSizeMode.Zoom;
            picLogo.TabIndex = 2;
            picLogo.TabStop = false;
            // 
            // logoSub
            // 
            logoSub.Font = new Font("Segoe UI", 8.5F);
            logoSub.ForeColor = Color.FromArgb(210, 180, 160);
            logoSub.Location = new Point(68, 44);
            logoSub.Name = "logoSub";
            logoSub.Size = new Size(150, 20);
            logoSub.TabIndex = 1;
            logoSub.Text = "Hệ thống quản lý";
            logoSub.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // logoLabel
            // 
            logoLabel.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            logoLabel.ForeColor = Color.White;
            logoLabel.Location = new Point(68, 14);
            logoLabel.Name = "logoLabel";
            logoLabel.Size = new Size(150, 30);
            logoLabel.TabIndex = 0;
            logoLabel.Text = "BÁNH MÌ NGON";
            logoLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // logoutPanel
            // 
            logoutPanel.BackColor = Color.Transparent;
            logoutPanel.Controls.Add(_logoutBtn);
            logoutPanel.Dock = DockStyle.Bottom;
            logoutPanel.Location = new Point(0, 724);
            logoutPanel.Name = "logoutPanel";
            logoutPanel.Size = new Size(230, 56);
            logoutPanel.TabIndex = 7;
            // 
            // _logoutBtn
            // 
            _logoutBtn.BackColor = Color.Transparent;
            _logoutBtn.Cursor = Cursors.Hand;
            _logoutBtn.Dock = DockStyle.Fill;
            _logoutBtn.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            _logoutBtn.IsSelected = false;
            _logoutBtn.Label = "Đăng xuất";
            _logoutBtn.Location = new Point(0, 0);
            _logoutBtn.Name = "_logoutBtn";
            _logoutBtn.NavIcon = null;
            _logoutBtn.Size = new Size(230, 56);
            _logoutBtn.TabIndex = 0;
            // 
            // tableRight
            // 
            tableRight.BackColor = Color.FromArgb(248, 245, 240);
            tableRight.ColumnCount = 1;
            tableRight.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableRight.Controls.Add(_topbar, 0, 0);
            tableRight.Controls.Add(_contentPanel, 0, 1);
            tableRight.Dock = DockStyle.Fill;
            tableRight.Location = new Point(230, 0);
            tableRight.Margin = new Padding(0);
            tableRight.Name = "tableRight";
            tableRight.RowCount = 2;
            tableRight.RowStyles.Add(new RowStyle(SizeType.Absolute, 64F));
            tableRight.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableRight.Size = new Size(1050, 780);
            tableRight.TabIndex = 1;
            // 
            // _topbar
            // 
            _topbar.BackColor = Color.White;
            _topbar.Controls.Add(avatarPanel);
            _topbar.Controls.Add(lblUserRole);
            _topbar.Controls.Add(lblUserName);
            _topbar.Controls.Add(breadPanel);
            _topbar.Dock = DockStyle.Fill;
            _topbar.Location = new Point(0, 0);
            _topbar.Margin = new Padding(0);
            _topbar.Name = "_topbar";
            _topbar.Size = new Size(1050, 64);
            _topbar.TabIndex = 0;
            // 
            // avatarPanel
            // 
            avatarPanel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            avatarPanel.Location = new Point(1000, 13);
            avatarPanel.Name = "avatarPanel";
            avatarPanel.Size = new Size(40, 40);
            avatarPanel.TabIndex = 3;
            // 
            // lblUserRole
            // 
            lblUserRole.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblUserRole.Font = new Font("Segoe UI", 8.5F);
            lblUserRole.ForeColor = Color.FromArgb(120, 100, 80);
            lblUserRole.Location = new Point(820, 34);
            lblUserRole.Name = "lblUserRole";
            lblUserRole.Size = new Size(172, 20);
            lblUserRole.TabIndex = 2;
            lblUserRole.Text = "Quản trị viên";
            lblUserRole.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblUserName
            // 
            lblUserName.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblUserName.Font = new Font("Segoe UI Semibold", 10F);
            lblUserName.ForeColor = Color.FromArgb(50, 35, 15);
            lblUserName.Location = new Point(820, 10);
            lblUserName.Name = "lblUserName";
            lblUserName.Size = new Size(172, 24);
            lblUserName.TabIndex = 1;
            lblUserName.Text = "Quản trị viên";
            lblUserName.TextAlign = ContentAlignment.MiddleRight;
            // 
            // breadPanel
            // 
            breadPanel.BackColor = Color.Transparent;
            breadPanel.Controls.Add(picTopBread);
            breadPanel.Controls.Add(breadLabel);
            breadPanel.Location = new Point(12, 0);
            breadPanel.Name = "breadPanel";
            breadPanel.Size = new Size(300, 64);
            breadPanel.TabIndex = 0;
            // 
            // picTopBread
            // 
            picTopBread.BackColor = Color.Transparent;
            picTopBread.Location = new Point(10, 21);
            picTopBread.Name = "picTopBread";
            picTopBread.Size = new Size(22, 22);
            picTopBread.SizeMode = PictureBoxSizeMode.Zoom;
            picTopBread.TabIndex = 1;
            picTopBread.TabStop = false;
            // 
            // breadLabel
            // 
            breadLabel.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            breadLabel.ForeColor = Color.FromArgb(50, 35, 15);
            breadLabel.Location = new Point(44, 0);
            breadLabel.Name = "breadLabel";
            breadLabel.Size = new Size(244, 64);
            breadLabel.TabIndex = 0;
            breadLabel.Text = "Quản trị hệ thống";
            breadLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // _contentPanel
            // 
            _contentPanel.BackColor = Color.FromArgb(248, 245, 240);
            _contentPanel.AutoScroll = true;
            _contentPanel.Dock = DockStyle.Fill;
            _contentPanel.Location = new Point(0, 64);
            _contentPanel.Margin = new Padding(0);
            _contentPanel.Name = "_contentPanel";
            _contentPanel.Padding = new Padding(24, 16, 24, 16);
            _contentPanel.Size = new Size(1050, 716);
            _contentPanel.TabIndex = 1;
            // 
            // AdminDashboardForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(248, 245, 240);
            ClientSize = new Size(1280, 780);
            Controls.Add(tableRoot);
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;
            MinimumSize = new Size(1100, 700);
            Name = "AdminDashboardForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Quản trị hệ thống – Bánh Mì Ngon";
            tableRoot.ResumeLayout(false);
            _sidebar.ResumeLayout(false);
            logoPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)picLogo).EndInit();
            logoutPanel.ResumeLayout(false);
            tableRight.ResumeLayout(false);
            _topbar.ResumeLayout(false);
            breadPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)picTopBread).EndInit();
            ResumeLayout(false);
        }

        #endregion
    }
}
