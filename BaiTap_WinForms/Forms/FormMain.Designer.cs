namespace BaiTap_WinForms.Forms
{
    partial class FormMain
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Panel panelSidebar;
        private System.Windows.Forms.Panel panelContent;
        private System.Windows.Forms.Label lblHeaderTitle;
        private System.Windows.Forms.Label lblUserInfo;
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.Button btnNavPOS;
        private System.Windows.Forms.Button btnNavProducts;
        private System.Windows.Forms.Button btnNavIngredients;
        private System.Windows.Forms.Button btnNavReports;
        private System.Windows.Forms.Button btnNavEmployees;
        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.TabPage tabPOS;
        private System.Windows.Forms.TabPage tabProducts;
        private System.Windows.Forms.TabPage tabIngredients;
        private System.Windows.Forms.TabPage tabReports;
        private System.Windows.Forms.TabPage tabEmployees;

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
            this.lblHeaderTitle = new System.Windows.Forms.Label();
            this.lblUserInfo = new System.Windows.Forms.Label();
            this.btnLogout = new System.Windows.Forms.Button();
            this.panelSidebar = new System.Windows.Forms.Panel();
            this.btnNavPOS = new System.Windows.Forms.Button();
            this.btnNavProducts = new System.Windows.Forms.Button();
            this.btnNavIngredients = new System.Windows.Forms.Button();
            this.btnNavReports = new System.Windows.Forms.Button();
            this.btnNavEmployees = new System.Windows.Forms.Button();
            this.panelContent = new System.Windows.Forms.Panel();
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tabPOS = new System.Windows.Forms.TabPage();
            this.tabProducts = new System.Windows.Forms.TabPage();
            this.tabIngredients = new System.Windows.Forms.TabPage();
            this.tabReports = new System.Windows.Forms.TabPage();
            this.tabEmployees = new System.Windows.Forms.TabPage();

            // FormMain
            this.ClientSize = new System.Drawing.Size(1280, 760);
            this.MinimumSize = new System.Drawing.Size(1100, 700);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Hệ Thống Quản Lý & Bán Hàng Quán Bánh Mì";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Controls.Add(this.panelContent);
            this.Controls.Add(this.panelSidebar);
            this.Controls.Add(this.panelTop);
            this.Load += new System.EventHandler(this.FormMain_Load);

            // panelTop
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Height = 65;
            this.panelTop.BackColor = System.Drawing.Color.FromArgb(211, 84, 0); // Orange primary
            this.panelTop.Controls.Add(this.lblHeaderTitle);
            this.panelTop.Controls.Add(this.lblUserInfo);
            this.panelTop.Controls.Add(this.btnLogout);

            // lblHeaderTitle
            this.lblHeaderTitle.Text = "🥪 TIỆM BÁNH MÌ NGON - QUẢN LÝ BÁN HÀNG";
            this.lblHeaderTitle.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold);
            this.lblHeaderTitle.ForeColor = System.Drawing.Color.White;
            this.lblHeaderTitle.Location = new System.Drawing.Point(20, 16);
            this.lblHeaderTitle.AutoSize = true;

            // lblUserInfo
            this.lblUserInfo.Text = "Xin chào: Admin (Quản lý)";
            this.lblUserInfo.Font = new System.Drawing.Font("Segoe UI", 10.5F);
            this.lblUserInfo.ForeColor = System.Drawing.Color.FromArgb(254, 237, 222);
            this.lblUserInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblUserInfo.Location = new System.Drawing.Point(900, 22);
            this.lblUserInfo.AutoSize = true;

            // btnLogout
            this.btnLogout.Text = "Đăng xuất";
            this.btnLogout.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnLogout.BackColor = System.Drawing.Color.FromArgb(192, 57, 43);
            this.btnLogout.ForeColor = System.Drawing.Color.White;
            this.btnLogout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogout.FlatAppearance.BorderSize = 0;
            this.btnLogout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLogout.Location = new System.Drawing.Point(1170, 16);
            this.btnLogout.Size = new System.Drawing.Size(95, 34);
            this.btnLogout.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);

            // panelSidebar
            this.panelSidebar.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelSidebar.Width = 220;
            this.panelSidebar.BackColor = System.Drawing.Color.FromArgb(44, 62, 80);
            this.panelSidebar.Controls.Add(this.btnNavEmployees);
            this.panelSidebar.Controls.Add(this.btnNavReports);
            this.panelSidebar.Controls.Add(this.btnNavIngredients);
            this.panelSidebar.Controls.Add(this.btnNavProducts);
            this.panelSidebar.Controls.Add(this.btnNavPOS);

            // btnNavPOS
            this.btnNavPOS.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnNavPOS.Height = 55;
            this.btnNavPOS.Text = "🛒  BÁN HÀNG (POS)";
            this.btnNavPOS.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnNavPOS.ForeColor = System.Drawing.Color.White;
            this.btnNavPOS.BackColor = System.Drawing.Color.FromArgb(230, 126, 34);
            this.btnNavPOS.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNavPOS.FlatAppearance.BorderSize = 0;
            this.btnNavPOS.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNavPOS.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnNavPOS.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNavPOS.Click += new System.EventHandler(this.btnNav_Click);

            // btnNavProducts
            this.btnNavProducts.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnNavProducts.Height = 55;
            this.btnNavProducts.Text = "🥪  Thực Đơn (Món)";
            this.btnNavProducts.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.btnNavProducts.ForeColor = System.Drawing.Color.Gainsboro;
            this.btnNavProducts.BackColor = System.Drawing.Color.FromArgb(44, 62, 80);
            this.btnNavProducts.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNavProducts.FlatAppearance.BorderSize = 0;
            this.btnNavProducts.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNavProducts.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnNavProducts.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNavProducts.Click += new System.EventHandler(this.btnNav_Click);

            // btnNavIngredients
            this.btnNavIngredients.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnNavIngredients.Height = 55;
            this.btnNavIngredients.Text = "🥬  Kho Nguyên Liệu";
            this.btnNavIngredients.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.btnNavIngredients.ForeColor = System.Drawing.Color.Gainsboro;
            this.btnNavIngredients.BackColor = System.Drawing.Color.FromArgb(44, 62, 80);
            this.btnNavIngredients.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNavIngredients.FlatAppearance.BorderSize = 0;
            this.btnNavIngredients.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNavIngredients.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnNavIngredients.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNavIngredients.Click += new System.EventHandler(this.btnNav_Click);

            // btnNavReports
            this.btnNavReports.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnNavReports.Height = 55;
            this.btnNavReports.Text = "📊  Báo Cáo Doanh Thu";
            this.btnNavReports.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.btnNavReports.ForeColor = System.Drawing.Color.Gainsboro;
            this.btnNavReports.BackColor = System.Drawing.Color.FromArgb(44, 62, 80);
            this.btnNavReports.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNavReports.FlatAppearance.BorderSize = 0;
            this.btnNavReports.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNavReports.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnNavReports.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNavReports.Click += new System.EventHandler(this.btnNav_Click);

            // btnNavEmployees
            this.btnNavEmployees.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnNavEmployees.Height = 55;
            this.btnNavEmployees.Text = "👥  Nhân Viên";
            this.btnNavEmployees.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.btnNavEmployees.ForeColor = System.Drawing.Color.Gainsboro;
            this.btnNavEmployees.BackColor = System.Drawing.Color.FromArgb(44, 62, 80);
            this.btnNavEmployees.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNavEmployees.FlatAppearance.BorderSize = 0;
            this.btnNavEmployees.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNavEmployees.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnNavEmployees.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNavEmployees.Click += new System.EventHandler(this.btnNav_Click);

            // panelContent
            this.panelContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContent.Controls.Add(this.tabMain);

            // tabMain
            this.tabMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabMain.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabMain.ItemSize = new System.Drawing.Size(0, 1);
            this.tabMain.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabMain.Controls.Add(this.tabPOS);
            this.tabMain.Controls.Add(this.tabProducts);
            this.tabMain.Controls.Add(this.tabIngredients);
            this.tabMain.Controls.Add(this.tabReports);
            this.tabMain.Controls.Add(this.tabEmployees);

            // Build individual tab contents
            this.InitializePOSTab();
            this.InitializeProductsTab();
            this.InitializeIngredientsTab();
            this.InitializeReportsTab();
            this.InitializeEmployeesTab();
        }
    }
}
