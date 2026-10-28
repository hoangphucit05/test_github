using System;
using System.Drawing;
using System.Windows.Forms;
using Login.Controls;

namespace Login
{
    /// <summary>
    /// Designer-editable view for the account management page. Business operations
    /// and database access remain in <see cref="AdminDashboardForm"/>.
    /// </summary>
    public partial class UserManagementView : UserControl
    {
        private bool _compactToolbar;

        public UserManagementView()
        {
            InitializeComponent();
            cmbStatus.SelectedIndex = 0;
            ConfigureSearchPlaceholder();
            ApplyResponsiveLayout();
        }

        public TextBox SearchBox => txtSearch;
        public ComboBox StatusFilter => cmbStatus;
        public ModernButton AddButton => btnAdd;
        public ModernButton EditButton => btnEdit;
        public ModernButton LockButton => btnLock;
        public ModernDataGrid UsersGrid => usersGrid;

        public void SetRecordCount(int count)
        {
            lblRecordCount.Text = count == 0
                ? "Hiển thị 0-0 trong 0 bản ghi"
                : $"Hiển thị 1-{count} trong {count} bản ghi";
        }

        private void ConfigureSearchPlaceholder()
        {
            string placeholder = txtSearch.Tag as string ?? string.Empty;
            txtSearch.GotFocus += (_, _) =>
            {
                if (txtSearch.Text == placeholder)
                {
                    txtSearch.Clear();
                    txtSearch.ForeColor = Color.FromArgb(50, 35, 15);
                }
            };
            txtSearch.LostFocus += (_, _) =>
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    txtSearch.Text = placeholder;
                    txtSearch.ForeColor = Color.Gray;
                }
            };
        }

        private void pagerLayout_Paint(object? sender, PaintEventArgs e)
        {
            using var pen = new Pen(Color.FromArgb(225, 210, 190));
            e.Graphics.DrawLine(pen, 0, 0, pagerLayout.Width, 0);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            ApplyResponsiveLayout();
        }

        private void ApplyResponsiveLayout()
        {
            if (rootLayout == null || toolbarLayout == null || rootLayout.RowStyles.Count < 2)
            {
                return;
            }

            bool compact = ClientSize.Width < 930;
            if (_compactToolbar == compact && toolbarLayout.RowCount == (compact ? 2 : 1))
            {
                return;
            }

            _compactToolbar = compact;
            toolbarLayout.SuspendLayout();
            rootLayout.SuspendLayout();

            toolbarLayout.RowStyles.Clear();
            if (compact)
            {
                rootLayout.RowStyles[1].Height = 92;
                toolbarLayout.RowCount = 2;
                toolbarLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
                toolbarLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
                toolbarLayout.SetCellPosition(filterFlow, new TableLayoutPanelCellPosition(0, 0));
                toolbarLayout.SetColumnSpan(filterFlow, 2);
                toolbarLayout.SetCellPosition(actionFlow, new TableLayoutPanelCellPosition(0, 1));
                toolbarLayout.SetColumnSpan(actionFlow, 2);
                actionFlow.Dock = DockStyle.Right;
            }
            else
            {
                rootLayout.RowStyles[1].Height = 52;
                toolbarLayout.RowCount = 1;
                toolbarLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
                toolbarLayout.SetCellPosition(filterFlow, new TableLayoutPanelCellPosition(0, 0));
                toolbarLayout.SetColumnSpan(filterFlow, 1);
                toolbarLayout.SetCellPosition(actionFlow, new TableLayoutPanelCellPosition(1, 0));
                toolbarLayout.SetColumnSpan(actionFlow, 1);
                actionFlow.Dock = DockStyle.Right;
            }

            rootLayout.ResumeLayout(true);
            toolbarLayout.ResumeLayout(true);
        }
    }
}
