namespace Login
{
    partial class UserManagementView
    {
        private System.ComponentModel.IContainer components = null!;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            rootLayout = new TableLayoutPanel();
            titlePanel = new Panel();
            lblBreadcrumb = new Label();
            lblTitle = new Label();
            toolbarLayout = new TableLayoutPanel();
            filterFlow = new FlowLayoutPanel();
            txtSearch = new TextBox();
            cmbStatus = new ComboBox();
            actionFlow = new FlowLayoutPanel();
            btnAdd = new Controls.ModernButton();
            btnEdit = new Controls.ModernButton();
            btnLock = new Controls.ModernButton();
            gridCard = new Panel();
            usersGrid = new Controls.ModernDataGrid();
            colSTT = new DataGridViewTextBoxColumn();
            colUserId = new DataGridViewTextBoxColumn();
            colUsername = new DataGridViewTextBoxColumn();
            colEmployee = new DataGridViewTextBoxColumn();
            colRole = new DataGridViewTextBoxColumn();
            colBranch = new DataGridViewTextBoxColumn();
            colStatus = new DataGridViewTextBoxColumn();
            colCreatedAt = new DataGridViewTextBoxColumn();
            colActions = new DataGridViewTextBoxColumn();
            pagerLayout = new TableLayoutPanel();
            lblRecordCount = new Label();
            pagerButtons = new FlowLayoutPanel();
            btnNext = new Controls.ModernButton();
            btnCurrentPage = new Controls.ModernButton();
            btnPrevious = new Controls.ModernButton();
            rootLayout.SuspendLayout();
            titlePanel.SuspendLayout();
            toolbarLayout.SuspendLayout();
            filterFlow.SuspendLayout();
            actionFlow.SuspendLayout();
            gridCard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)usersGrid).BeginInit();
            pagerLayout.SuspendLayout();
            pagerButtons.SuspendLayout();
            SuspendLayout();
            // 
            // 
            // rootLayout
            // 
            rootLayout.ColumnCount = 1;
            rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            rootLayout.Controls.Add(titlePanel, 0, 0);
            rootLayout.Controls.Add(toolbarLayout, 0, 1);
            rootLayout.Controls.Add(gridCard, 0, 2);
            rootLayout.Controls.Add(pagerLayout, 0, 3);
            rootLayout.Dock = DockStyle.Fill;
            rootLayout.Location = new Point(0, 0);
            rootLayout.Margin = new Padding(0);
            rootLayout.Name = "rootLayout";
            rootLayout.RowCount = 4;
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 68F));
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 52F));
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));
            rootLayout.Size = new Size(1050, 648);
            rootLayout.TabIndex = 0;
            // 
            // titlePanel
            // 
            titlePanel.Controls.Add(lblBreadcrumb);
            titlePanel.Controls.Add(lblTitle);
            titlePanel.Dock = DockStyle.Fill;
            titlePanel.Location = new Point(0, 0);
            titlePanel.Margin = new Padding(0);
            titlePanel.Name = "titlePanel";
            titlePanel.Size = new Size(1050, 68);
            titlePanel.TabIndex = 0;
            // 
            // lblBreadcrumb
            // 
            lblBreadcrumb.AutoSize = true;
            lblBreadcrumb.Font = new Font("Segoe UI", 9F);
            lblBreadcrumb.ForeColor = Color.FromArgb(120, 100, 80);
            lblBreadcrumb.Location = new Point(0, 44);
            lblBreadcrumb.Name = "lblBreadcrumb";
            lblBreadcrumb.Size = new Size(216, 15);
            lblBreadcrumb.TabIndex = 1;
            lblBreadcrumb.Text = "Trang chủ  ›  Quản lý tài khoản";
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 17F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(50, 35, 15);
            lblTitle.Location = new Point(0, 2);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(209, 31);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Quản lý tài khoản";
            // 
            // toolbarLayout
            // 
            toolbarLayout.ColumnCount = 2;
            toolbarLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            toolbarLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            toolbarLayout.Controls.Add(filterFlow, 0, 0);
            toolbarLayout.Controls.Add(actionFlow, 1, 0);
            toolbarLayout.Dock = DockStyle.Fill;
            toolbarLayout.Location = new Point(0, 68);
            toolbarLayout.Margin = new Padding(0);
            toolbarLayout.Name = "toolbarLayout";
            toolbarLayout.RowCount = 1;
            toolbarLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            toolbarLayout.Size = new Size(1050, 52);
            toolbarLayout.TabIndex = 1;
            // 
            // filterFlow
            // 
            filterFlow.Controls.Add(txtSearch);
            filterFlow.Controls.Add(cmbStatus);
            filterFlow.Dock = DockStyle.Fill;
            filterFlow.FlowDirection = FlowDirection.LeftToRight;
            filterFlow.Location = new Point(0, 0);
            filterFlow.Margin = new Padding(0);
            filterFlow.Name = "filterFlow";
            filterFlow.Padding = new Padding(0, 8, 8, 8);
            filterFlow.Size = new Size(648, 52);
            filterFlow.TabIndex = 0;
            filterFlow.WrapContents = false;
            // 
            // txtSearch
            // 
            txtSearch.BorderStyle = BorderStyle.FixedSingle;
            txtSearch.Font = new Font("Segoe UI", 9.75F);
            txtSearch.ForeColor = Color.Gray;
            txtSearch.Location = new Point(0, 4);
            txtSearch.Margin = new Padding(0, 4, 8, 4);
            txtSearch.Name = "txtSearch";
            txtSearch.Size = new Size(300, 25);
            txtSearch.TabIndex = 0;
            txtSearch.Tag = "Tìm kiếm theo tên, tài khoản, nhân viên...";
            txtSearch.Text = "Tìm kiếm theo tên, tài khoản, nhân viên...";
            // 
            // cmbStatus
            // 
            cmbStatus.BackColor = Color.White;
            cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbStatus.FlatStyle = FlatStyle.Flat;
            cmbStatus.Font = new Font("Segoe UI", 9.75F);
            cmbStatus.FormattingEnabled = true;
            cmbStatus.Items.AddRange(new object[] { "Tất cả trạng thái", "Đang hoạt động", "Đã khóa" });
            cmbStatus.Location = new Point(308, 4);
            cmbStatus.Margin = new Padding(0, 4, 0, 4);
            cmbStatus.Name = "cmbStatus";
            cmbStatus.Size = new Size(180, 25);
            cmbStatus.TabIndex = 1;
            // 
            // actionFlow
            // 
            actionFlow.AutoSize = true;
            actionFlow.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            actionFlow.Controls.Add(btnAdd);
            actionFlow.Controls.Add(btnEdit);
            actionFlow.Controls.Add(btnLock);
            actionFlow.Dock = DockStyle.Right;
            actionFlow.FlowDirection = FlowDirection.LeftToRight;
            actionFlow.Location = new Point(648, 0);
            actionFlow.Margin = new Padding(0);
            actionFlow.Name = "actionFlow";
            actionFlow.Padding = new Padding(0, 6, 0, 8);
            actionFlow.Size = new Size(402, 52);
            actionFlow.TabIndex = 1;
            actionFlow.WrapContents = false;
            // 
            // btnAdd
            // 
            btnAdd.AutoSize = true;
            btnAdd.BorderColor = Color.Transparent;
            btnAdd.BorderRadius = 7;
            btnAdd.BorderWidth = 0;
            btnAdd.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnAdd.ForeColor = Color.White;
            btnAdd.HoverColor = Color.FromArgb(193, 111, 47);
            btnAdd.Location = new Point(0, 6);
            btnAdd.Margin = new Padding(0, 0, 6, 0);
            btnAdd.MinimumSize = new Size(146, 36);
            btnAdd.Name = "btnAdd";
            btnAdd.NormalColor = Color.FromArgb(177, 91, 25);
            btnAdd.Padding = new Padding(12, 0, 12, 0);
            btnAdd.PressedColor = Color.FromArgb(159, 82, 23);
            btnAdd.Size = new Size(146, 36);
            btnAdd.TabIndex = 0;
            btnAdd.Text = "+ Thêm tài khoản";
            // 
            // btnEdit
            // 
            btnEdit.AutoSize = true;
            btnEdit.BorderColor = Color.FromArgb(225, 210, 190);
            btnEdit.BorderRadius = 7;
            btnEdit.BorderWidth = 1;
            btnEdit.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnEdit.ForeColor = Color.FromArgb(50, 35, 15);
            btnEdit.HoverColor = Color.FromArgb(255, 247, 238);
            btnEdit.Location = new Point(152, 6);
            btnEdit.Margin = new Padding(0, 0, 6, 0);
            btnEdit.MinimumSize = new Size(68, 36);
            btnEdit.Name = "btnEdit";
            btnEdit.NormalColor = Color.White;
            btnEdit.Padding = new Padding(16, 0, 16, 0);
            btnEdit.PressedColor = Color.FromArgb(247, 230, 214);
            btnEdit.Size = new Size(68, 36);
            btnEdit.TabIndex = 1;
            btnEdit.Text = "Sửa";
            // 
            // btnLock
            // 
            btnLock.AutoSize = true;
            btnLock.BorderColor = Color.Transparent;
            btnLock.BorderRadius = 7;
            btnLock.BorderWidth = 0;
            btnLock.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnLock.ForeColor = Color.White;
            btnLock.HoverColor = Color.FromArgb(202, 77, 65);
            btnLock.Location = new Point(226, 6);
            btnLock.Margin = new Padding(0);
            btnLock.MinimumSize = new Size(134, 36);
            btnLock.Name = "btnLock";
            btnLock.NormalColor = Color.FromArgb(192, 57, 43);
            btnLock.Padding = new Padding(12, 0, 12, 0);
            btnLock.PressedColor = Color.FromArgb(173, 51, 39);
            btnLock.Size = new Size(134, 36);
            btnLock.TabIndex = 2;
            btnLock.Text = "Khóa tài khoản";
            // 
            // gridCard
            // 
            gridCard.BackColor = Color.White;
            gridCard.BorderStyle = BorderStyle.FixedSingle;
            gridCard.Controls.Add(usersGrid);
            gridCard.Dock = DockStyle.Fill;
            gridCard.Location = new Point(0, 120);
            gridCard.Margin = new Padding(0, 0, 0, 6);
            gridCard.Name = "gridCard";
            gridCard.Padding = new Padding(1);
            gridCard.Size = new Size(1050, 482);
            gridCard.TabIndex = 2;
            // 
            // usersGrid
            // 
            usersGrid.AllowUserToAddRows = false;
            usersGrid.AllowUserToDeleteRows = false;
            usersGrid.AllowUserToResizeRows = false;
            usersGrid.AutoGenerateColumns = false;
            usersGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            usersGrid.BackgroundColor = Color.White;
            usersGrid.BorderStyle = BorderStyle.None;
            usersGrid.ColumnHeadersHeight = 44;
            usersGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            usersGrid.Columns.AddRange(new DataGridViewColumn[] { colSTT, colUserId, colUsername, colEmployee, colRole, colBranch, colStatus, colCreatedAt, colActions });
            usersGrid.Dock = DockStyle.Fill;
            usersGrid.Location = new Point(1, 1);
            usersGrid.Margin = new Padding(0);
            usersGrid.MultiSelect = false;
            usersGrid.Name = "usersGrid";
            usersGrid.ReadOnly = true;
            usersGrid.RowHeadersVisible = false;
            usersGrid.RowTemplate.Height = 44;
            usersGrid.ScrollBars = ScrollBars.Both;
            usersGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            usersGrid.ShowCellToolTips = true;
            usersGrid.Size = new Size(1048, 480);
            usersGrid.TabIndex = 0;
            // 
            // colSTT
            // 
            colSTT.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            colSTT.DataPropertyName = "STT";
            colSTT.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colSTT.HeaderText = "STT";
            colSTT.MinimumWidth = 50;
            colSTT.Name = "STT";
            colSTT.ReadOnly = true;
            colSTT.Width = 55;
            // 
            // colUserId
            // 
            colUserId.DataPropertyName = "UserID";
            colUserId.HeaderText = "UserID";
            colUserId.Name = "UserID";
            colUserId.ReadOnly = true;
            colUserId.Visible = false;
            // 
            // colUsername
            // 
            colUsername.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colUsername.DataPropertyName = "Tên_đăng_nhập";
            colUsername.FillWeight = 20F;
            colUsername.HeaderText = "Tên đăng nhập";
            colUsername.MinimumWidth = 120;
            colUsername.Name = "Tên_đăng_nhập";
            colUsername.ReadOnly = true;
            // 
            // colEmployee
            // 
            colEmployee.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colEmployee.DataPropertyName = "Nhân_viên";
            colEmployee.FillWeight = 28F;
            colEmployee.HeaderText = "Nhân viên";
            colEmployee.MinimumWidth = 150;
            colEmployee.Name = "Nhân_viên";
            colEmployee.ReadOnly = true;
            // 
            // colRole
            // 
            colRole.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colRole.DataPropertyName = "Vai_trò";
            colRole.FillWeight = 16F;
            colRole.HeaderText = "Vai trò";
            colRole.MinimumWidth = 100;
            colRole.Name = "Vai_trò";
            colRole.ReadOnly = true;
            // 
            // colBranch
            // 
            colBranch.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colBranch.DataPropertyName = "Chi_nhánh";
            colBranch.FillWeight = 36F;
            colBranch.HeaderText = "Chi nhánh";
            colBranch.MinimumWidth = 180;
            colBranch.Name = "Chi_nhánh";
            colBranch.ReadOnly = true;
            // 
            // colStatus
            // 
            colStatus.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            colStatus.DataPropertyName = "Trạng_thái";
            colStatus.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colStatus.HeaderText = "Trạng thái";
            colStatus.MinimumWidth = 135;
            colStatus.Name = "Trạng_thái";
            colStatus.ReadOnly = true;
            colStatus.Width = 140;
            // 
            // colCreatedAt
            // 
            colCreatedAt.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            colCreatedAt.DataPropertyName = "Ngày_tạo";
            colCreatedAt.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colCreatedAt.HeaderText = "Ngày tạo";
            colCreatedAt.MinimumWidth = 100;
            colCreatedAt.Name = "Ngày_tạo";
            colCreatedAt.ReadOnly = true;
            colCreatedAt.Width = 110;
            // 
            // colActions
            // 
            colActions.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            colActions.DataPropertyName = "Thao_tác";
            colActions.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colActions.HeaderText = "Thao tác";
            colActions.MinimumWidth = 95;
            colActions.Name = "Thao_tác";
            colActions.ReadOnly = true;
            colActions.SortMode = DataGridViewColumnSortMode.NotSortable;
            colActions.Width = 100;
            // 
            // pagerLayout
            // 
            pagerLayout.ColumnCount = 2;
            pagerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            pagerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            pagerLayout.Controls.Add(lblRecordCount, 0, 0);
            pagerLayout.Controls.Add(pagerButtons, 1, 0);
            pagerLayout.Dock = DockStyle.Fill;
            pagerLayout.Location = new Point(0, 604);
            pagerLayout.Margin = new Padding(0);
            pagerLayout.Name = "pagerLayout";
            pagerLayout.RowCount = 1;
            pagerLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            pagerLayout.Size = new Size(1050, 44);
            pagerLayout.TabIndex = 3;
            pagerLayout.Paint += pagerLayout_Paint;
            // 
            // lblRecordCount
            // 
            lblRecordCount.Dock = DockStyle.Fill;
            lblRecordCount.Font = new Font("Segoe UI", 9F);
            lblRecordCount.ForeColor = Color.FromArgb(120, 100, 80);
            lblRecordCount.Location = new Point(0, 0);
            lblRecordCount.Margin = new Padding(0);
            lblRecordCount.Name = "lblRecordCount";
            lblRecordCount.Size = new Size(946, 44);
            lblRecordCount.TabIndex = 0;
            lblRecordCount.Text = "Hiển thị 0-0 trong 0 bản ghi";
            lblRecordCount.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // pagerButtons
            // 
            pagerButtons.AutoSize = true;
            pagerButtons.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            pagerButtons.Controls.Add(btnPrevious);
            pagerButtons.Controls.Add(btnCurrentPage);
            pagerButtons.Controls.Add(btnNext);
            pagerButtons.Dock = DockStyle.Right;
            pagerButtons.Location = new Point(946, 0);
            pagerButtons.Margin = new Padding(0);
            pagerButtons.Name = "pagerButtons";
            pagerButtons.Padding = new Padding(0, 6, 0, 6);
            pagerButtons.Size = new Size(104, 44);
            pagerButtons.TabIndex = 1;
            pagerButtons.WrapContents = false;
            // 
            // btnPrevious
            // 
            btnPrevious.BorderColor = Color.FromArgb(225, 210, 190);
            btnPrevious.BorderRadius = 6;
            btnPrevious.BorderWidth = 1;
            btnPrevious.Cursor = Cursors.Default;
            btnPrevious.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnPrevious.ForeColor = Color.FromArgb(50, 35, 15);
            btnPrevious.HoverColor = Color.FromArgb(255, 247, 238);
            btnPrevious.Location = new Point(0, 6);
            btnPrevious.Margin = new Padding(0, 0, 5, 0);
            btnPrevious.Name = "btnPrevious";
            btnPrevious.NormalColor = Color.White;
            btnPrevious.PressedColor = Color.FromArgb(247, 230, 214);
            btnPrevious.Size = new Size(30, 30);
            btnPrevious.TabIndex = 0;
            btnPrevious.TabStop = false;
            btnPrevious.Text = "‹";
            // 
            // btnCurrentPage
            // 
            btnCurrentPage.BorderColor = Color.FromArgb(177, 91, 25);
            btnCurrentPage.BorderRadius = 6;
            btnCurrentPage.BorderWidth = 1;
            btnCurrentPage.Cursor = Cursors.Default;
            btnCurrentPage.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnCurrentPage.ForeColor = Color.White;
            btnCurrentPage.HoverColor = Color.FromArgb(177, 91, 25);
            btnCurrentPage.Location = new Point(35, 6);
            btnCurrentPage.Margin = new Padding(0, 0, 5, 0);
            btnCurrentPage.Name = "btnCurrentPage";
            btnCurrentPage.NormalColor = Color.FromArgb(177, 91, 25);
            btnCurrentPage.PressedColor = Color.FromArgb(177, 91, 25);
            btnCurrentPage.Size = new Size(30, 30);
            btnCurrentPage.TabIndex = 1;
            btnCurrentPage.TabStop = false;
            btnCurrentPage.Text = "1";
            // 
            // btnNext
            // 
            btnNext.BorderColor = Color.FromArgb(225, 210, 190);
            btnNext.BorderRadius = 6;
            btnNext.BorderWidth = 1;
            btnNext.Cursor = Cursors.Default;
            btnNext.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnNext.ForeColor = Color.FromArgb(50, 35, 15);
            btnNext.HoverColor = Color.FromArgb(255, 247, 238);
            btnNext.Location = new Point(70, 6);
            btnNext.Margin = new Padding(0);
            btnNext.Name = "btnNext";
            btnNext.NormalColor = Color.White;
            btnNext.PressedColor = Color.FromArgb(247, 230, 214);
            btnNext.Size = new Size(30, 30);
            btnNext.TabIndex = 2;
            btnNext.TabStop = false;
            btnNext.Text = "›";
            // 
            // UserManagementView
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(248, 245, 240);
            Controls.Add(rootLayout);
            Dock = DockStyle.Fill;
            MinimumSize = new Size(620, 420);
            Name = "UserManagementView";
            Size = new Size(1050, 648);
            rootLayout.ResumeLayout(false);
            titlePanel.ResumeLayout(false);
            titlePanel.PerformLayout();
            toolbarLayout.ResumeLayout(false);
            toolbarLayout.PerformLayout();
            filterFlow.ResumeLayout(false);
            filterFlow.PerformLayout();
            actionFlow.ResumeLayout(false);
            gridCard.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)usersGrid).EndInit();
            pagerLayout.ResumeLayout(false);
            pagerLayout.PerformLayout();
            pagerButtons.ResumeLayout(false);
            ResumeLayout(false);
        }

        private TableLayoutPanel rootLayout = null!;
        private Panel titlePanel = null!;
        private Label lblBreadcrumb = null!;
        private Label lblTitle = null!;
        private TableLayoutPanel toolbarLayout = null!;
        private FlowLayoutPanel filterFlow = null!;
        private TextBox txtSearch = null!;
        private ComboBox cmbStatus = null!;
        private FlowLayoutPanel actionFlow = null!;
        private Controls.ModernButton btnAdd = null!;
        private Controls.ModernButton btnEdit = null!;
        private Controls.ModernButton btnLock = null!;
        private Panel gridCard = null!;
        private Controls.ModernDataGrid usersGrid = null!;
        private DataGridViewTextBoxColumn colSTT = null!;
        private DataGridViewTextBoxColumn colUserId = null!;
        private DataGridViewTextBoxColumn colUsername = null!;
        private DataGridViewTextBoxColumn colEmployee = null!;
        private DataGridViewTextBoxColumn colRole = null!;
        private DataGridViewTextBoxColumn colBranch = null!;
        private DataGridViewTextBoxColumn colStatus = null!;
        private DataGridViewTextBoxColumn colCreatedAt = null!;
        private DataGridViewTextBoxColumn colActions = null!;
        private TableLayoutPanel pagerLayout = null!;
        private Label lblRecordCount = null!;
        private FlowLayoutPanel pagerButtons = null!;
        private Controls.ModernButton btnPrevious = null!;
        private Controls.ModernButton btnCurrentPage = null!;
        private Controls.ModernButton btnNext = null!;
    }
}
