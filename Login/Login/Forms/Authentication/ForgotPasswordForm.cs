using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Login.Controls;

namespace Login
{
    /// <summary>
    /// Designer-editable form for submitting a password-reset request.
    /// Runtime-only asset loading is excluded from the Visual Studio Designer.
    /// </summary>
    public partial class ForgotPasswordForm : Form
    {
        public ForgotPasswordForm(string? username = null)
        {
            InitializeComponent();
            txtUsername.InputText = username ?? string.Empty;

            if (!DesignTimeHelper.IsDesignTime(this))
            {
                LoadIcons();
            }
        }

        private void LoadIcons()
        {
            string iconDirectory = Path.Combine(AppContext.BaseDirectory, "Assets", "Icons");
            string userIcon = Path.Combine(iconDirectory, "icon_user_clean.png");
            string mailIcon = Path.Combine(iconDirectory, "icon_mail_clean.png");

            if (File.Exists(userIcon)) txtUsername.LeftIcon = Image.FromFile(userIcon);
            if (File.Exists(mailIcon)) txtEmail.LeftIcon = Image.FromFile(mailIcon);
        }

        private void btnSend_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.InputText) ||
                string.IsNullOrWhiteSpace(txtEmail.InputText) ||
                !txtEmail.InputText.Contains('@'))
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập và email hợp lệ.", "Thông tin chưa đầy đủ",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show("Yêu cầu của bạn đã được ghi nhận. Quản trị viên sẽ xác minh và hỗ trợ đặt lại mật khẩu.",
                "Đã gửi yêu cầu", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
        }
    }
}
