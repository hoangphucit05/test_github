using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using Login.Controls;

namespace Login
{
    public partial class Form1 : Form
    {
        private string _selectedRole = "Quản trị viên";

        public Form1()
        {
            InitializeComponent();
            DoubleBuffered = true;

            // Do not load external image files while Visual Studio creates the form.
            // This keeps the Designer independent from the application's output folder.
            if (!DesignTimeHelper.IsDesignTime(this))
            {
                LoadAssets();
                loginContentPanel.Resize += (_, _) => ApplyResponsiveLoginLayout();
                ApplyResponsiveLoginLayout();
            }

            SetupEventHandlers();
        }

        /// <summary>
        /// Keeps the login controls at a readable maximum width while centering them
        /// in the right card. At the minimum form width they shrink safely instead
        /// of overlapping; at wider sizes they remain visually balanced.
        /// </summary>
        private void ApplyResponsiveLoginLayout()
        {
            int panelWidth = loginContentPanel.ClientSize.Width;
            if (panelWidth <= 0)
            {
                return;
            }

            int fieldWidth = Math.Min(390, Math.Max(320, panelWidth - 64));
            int fieldLeft = Math.Max(32, (panelWidth - fieldWidth) / 2);
            int titleWidth = Math.Min(440, Math.Max(320, panelWidth - 48));
            int titleLeft = Math.Max(24, (panelWidth - titleWidth) / 2);

            lblTitle.Left = titleLeft;
            lblTitle.Width = titleWidth;
            lblSubtitle.Left = titleLeft;
            lblSubtitle.Width = titleWidth;

            txtUsername.Left = fieldLeft;
            txtUsername.Width = fieldWidth;
            txtPassword.Left = fieldLeft;
            txtPassword.Width = fieldWidth;
            chkRemember.Left = fieldLeft;
            lnkForgotPassword.Left = fieldLeft + fieldWidth - lnkForgotPassword.Width;
            btnLogin.Left = fieldLeft;
            btnLogin.Width = fieldWidth;
            divider.Left = fieldLeft;
            divider.Width = fieldWidth;

            const int roleWidth = 120;
            const int roleGap = 14;
            int rolesWidth = (roleWidth * 3) + (roleGap * 2);
            int rolesLeft = Math.Max(24, (panelWidth - rolesWidth) / 2);
            btnRoleAdmin.Left = rolesLeft;
            btnRoleManager.Left = rolesLeft + roleWidth + roleGap;
            btnRoleStaff.Left = rolesLeft + (roleWidth + roleGap) * 2;

            picInfoIcon.Left = fieldLeft + 4;
            lblInfo.Left = picInfoIcon.Right + 10;
            lblInfo.Width = Math.Max(200, fieldLeft + fieldWidth - lblInfo.Left);
        }

        private void LoadAssets()
        {
            string baseDir = AppContext.BaseDirectory;
            string iconDir = Path.Combine(baseDir, "Assets", "Icons");
            string imageDir = Path.Combine(baseDir, "Assets", "Images");

            // Load window icon
            string iconPath = Path.Combine(iconDir, "app.ico");
            if (File.Exists(iconPath))
            {
                try
                {
                    this.Icon = new Icon(iconPath);
                }
                catch { }
            }

            // Load left poster
            string posterPath = Path.Combine(imageDir, "poster_rounded.png");
            if (File.Exists(posterPath))
            {
                try
                {
                    picPoster.Image = Image.FromFile(posterPath);
                }
                catch { }
            }

            // Load input icons
            string userIconPath = Path.Combine(iconDir, "icon_user_clean.png");
            if (File.Exists(userIconPath))
            {
                txtUsername.LeftIcon = Image.FromFile(userIconPath);
            }

            string lockIconPath = Path.Combine(iconDir, "icon_lock_clean.png");
            if (File.Exists(lockIconPath))
            {
                txtPassword.LeftIcon = Image.FromFile(lockIconPath);
            }

            string eyeOpenPath = Path.Combine(iconDir, "icon_eye_clean.png");
            string eyeSlashPath = Path.Combine(iconDir, "icon_eyeslash_clean.png");
            if (File.Exists(eyeOpenPath) && File.Exists(eyeSlashPath))
            {
                txtPassword.SetEyeIcons(Image.FromFile(eyeOpenPath), Image.FromFile(eyeSlashPath));
            }

            // Load button icon
            string loginIconPath = Path.Combine(iconDir, "icon_login_clean.png");
            if (File.Exists(loginIconPath))
            {
                btnLogin.ButtonIcon = Image.FromFile(loginIconPath);
            }

            // Load role icons
            string shieldPath = Path.Combine(iconDir, "icon_shield_clean.png");
            if (File.Exists(shieldPath))
            {
                btnRoleAdmin.RoleIcon = Image.FromFile(shieldPath);
            }

            string usersPath = Path.Combine(iconDir, "icon_users_clean.png");
            if (File.Exists(usersPath))
            {
                btnRoleManager.RoleIcon = Image.FromFile(usersPath);
            }

            string storePath = Path.Combine(iconDir, "icon_store_clean.png");
            if (File.Exists(storePath))
            {
                btnRoleStaff.RoleIcon = Image.FromFile(storePath);
            }

            // Load info banner icon
            string infoIconPath = Path.Combine(iconDir, "icon_info_clean.png");
            if (File.Exists(infoIconPath))
            {
                picInfoIcon.Image = Image.FromFile(infoIconPath);
            }
        }

        private void SetupEventHandlers()
        {
            // Role selection handlers
            btnRoleAdmin.SelectedChanged += (s, e) =>
            {
                if (btnRoleAdmin.IsSelected)
                {
                    btnRoleManager.IsSelected = false;
                    btnRoleStaff.IsSelected = false;
                    _selectedRole = "Quản trị viên";
                    txtUsername.InputText = "admin";
                    txtPassword.InputText = string.Empty;
                }
            };

            btnRoleManager.SelectedChanged += (s, e) =>
            {
                if (btnRoleManager.IsSelected)
                {
                    btnRoleAdmin.IsSelected = false;
                    btnRoleStaff.IsSelected = false;
                    _selectedRole = "Quản lý";
                    txtUsername.InputText = "manager";
                    txtPassword.InputText = string.Empty;
                }
            };

            btnRoleStaff.SelectedChanged += (s, e) =>
            {
                if (btnRoleStaff.IsSelected)
                {
                    btnRoleAdmin.IsSelected = false;
                    btnRoleManager.IsSelected = false;
                    _selectedRole = "Nhân viên";
                    txtUsername.InputText = "staff01";
                    txtPassword.InputText = string.Empty;
                }
            };

            // Login button handler
            btnLogin.Click += BtnLogin_Click;

            // Forgot-password request is presented as a focused form instead of a
            // generic message box, so it follows the rest of the login experience.
            lnkForgotPassword.Click += (_, _) => ShowForgotPasswordDialog();

            // Enter key to login
            this.AcceptButton = btnLogin;
        }

        private void ShowForgotPasswordDialog()
        {
            using var dialog = new ForgotPasswordForm(txtUsername.InputText);
            dialog.ShowDialog(this);
        }

        private async void BtnLogin_Click(object? sender, EventArgs e)
        {
            string username = txtUsername.InputText.Trim();
            // Khoảng trắng có thể là một phần của mật khẩu hợp lệ.
            string password = txtPassword.InputText;

            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show(
                    "Vui lòng nhập tên tài khoản!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show(
                    "Vui lòng nhập mật khẩu!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            btnLogin.Enabled = false;
            btnLogin.Text = "Đang xác thực...";

            try
            {
                using var db = new DAL.BakeryDbContext();
                var authService = new BLL.Services.AuthService(db);
                var result = await authService.LoginAsync(username, password, _selectedRole);

                if (!result.IsSuccess)
                {
                    MessageBox.Show(
                        result.Message,
                        "Đăng nhập thất bại",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return;
                }

                if (result.Session?.RoleName.Equals("ADMIN", StringComparison.OrdinalIgnoreCase) == true)
                {
                    Hide();
                    using var dashboard = new AdminDashboardForm(result.Session);
                    dashboard.ShowDialog(this);
                    Show();
                    return;
                }

                MessageBox.Show(
                    $"{result.Message}\n\n" +
                    $"• Họ và tên: {result.Session?.FullName}\n" +
                    $"• Vai trò: {result.Session?.RoleName}\n" +
                    $"• Chi nhánh: {result.Session?.BranchName ?? "Chưa phân chi nhánh"}\n" +
                    $"• Ghi nhớ tài khoản: {(chkRemember.Checked ? "Có" : "Không")}",
                    "Xác thực thành công",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    DAL.DatabaseErrorMessage.From(ex),
                    "Lỗi hệ thống",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                btnLogin.Enabled = true;
                btnLogin.Text = "Đăng nhập";
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Draw subtle wheat decorative illustrations in the background margins
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using var wheatPen = new Pen(Color.FromArgb(22, 161, 82, 26), 1.2f);
            using var wheatBrush = new SolidBrush(Color.FromArgb(18, 161, 82, 26));

            // Right side subtle wheat stalk
            DrawWheatStalk(g, wheatPen, wheatBrush, Width - 18, Height - 30, -55, 6);
            DrawWheatStalk(g, wheatPen, wheatBrush, Width - 8, 90, -45, 5);

            // Left side subtle wheat stalk (behind poster edge)
            DrawWheatStalk(g, wheatPen, wheatBrush, 12, Height - 50, 45, 5);
        }

        private void DrawWheatStalk(Graphics g, Pen pen, Brush brush, float x, float y, float angleDeg, int grains)
        {
            var state = g.Save();
            g.TranslateTransform(x, y);
            g.RotateTransform(angleDeg);

            // Stem
            g.DrawLine(pen, 0, 0, 0, -80);

            // Grains along the stem
            for (int i = 0; i < grains; i++)
            {
                float gy = -15 - (i * 11);
                // Left grain
                g.FillEllipse(brush, -7, gy - 3, 7, 12);
                g.DrawLine(pen, -4, gy - 2, -10, gy - 12);

                // Right grain
                g.FillEllipse(brush, 0, gy - 3, 7, 12);
                g.DrawLine(pen, 4, gy - 2, 10, gy - 12);
            }

            g.Restore(state);
        }
    }
}
