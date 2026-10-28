namespace Login
{
    partial class ForgotPasswordForm
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
            lblTitle = new Label();
            lblDescription = new Label();
            lblUsername = new Label();
            txtUsername = new Controls.CustomInputBox();
            lblEmail = new Label();
            txtEmail = new Controls.CustomInputBox();
            btnSend = new Controls.ModernButton();
            btnCancel = new Controls.ModernButton();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(88, 42, 16);
            lblTitle.Location = new Point(26, 24);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(145, 28);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Quên mật khẩu?";
            // 
            // lblDescription
            // 
            lblDescription.ForeColor = Color.FromArgb(120, 100, 80);
            lblDescription.Location = new Point(26, 58);
            lblDescription.Name = "lblDescription";
            lblDescription.Size = new Size(378, 36);
            lblDescription.TabIndex = 1;
            lblDescription.Text = "Nhập tên đăng nhập và email để gửi yêu cầu đặt lại mật khẩu.";
            // 
            // lblUsername
            // 
            lblUsername.AutoSize = true;
            lblUsername.ForeColor = Color.FromArgb(80, 60, 40);
            lblUsername.Location = new Point(26, 106);
            lblUsername.Name = "lblUsername";
            lblUsername.Size = new Size(105, 17);
            lblUsername.TabIndex = 2;
            lblUsername.Text = "Tên đăng nhập *";
            // 
            // txtUsername
            // 
            txtUsername.InputText = "";
            txtUsername.IsPassword = false;
            txtUsername.LeftIcon = null;
            txtUsername.Location = new Point(26, 128);
            txtUsername.Name = "txtUsername";
            txtUsername.PlaceholderText = "Ví dụ: nhanvien01";
            txtUsername.ShowPasswordToggle = false;
            txtUsername.Size = new Size(378, 44);
            txtUsername.TabIndex = 3;
            // 
            // lblEmail
            // 
            lblEmail.AutoSize = true;
            lblEmail.ForeColor = Color.FromArgb(80, 60, 40);
            lblEmail.Location = new Point(26, 184);
            lblEmail.Name = "lblEmail";
            lblEmail.Size = new Size(47, 17);
            lblEmail.TabIndex = 4;
            lblEmail.Text = "Email *";
            // 
            // txtEmail
            // 
            txtEmail.InputText = "";
            txtEmail.IsPassword = false;
            txtEmail.LeftIcon = null;
            txtEmail.Location = new Point(26, 206);
            txtEmail.Name = "txtEmail";
            txtEmail.PlaceholderText = "email@banhmingon.vn";
            txtEmail.ShowPasswordToggle = false;
            txtEmail.Size = new Size(378, 44);
            txtEmail.TabIndex = 5;
            // 
            // btnSend
            // 
            btnSend.BorderColor = Color.Transparent;
            btnSend.BorderRadius = 7;
            btnSend.BorderWidth = 0;
            btnSend.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnSend.ForeColor = Color.White;
            btnSend.HoverColor = Color.FromArgb(195, 108, 42);
            btnSend.Location = new Point(186, 264);
            btnSend.Name = "btnSend";
            btnSend.NormalColor = Color.FromArgb(177, 91, 25);
            btnSend.PressedColor = Color.FromArgb(145, 68, 16);
            btnSend.Size = new Size(136, 34);
            btnSend.TabIndex = 6;
            btnSend.Text = "Gửi yêu cầu";
            btnSend.Click += btnSend_Click;
            // 
            // btnCancel
            // 
            btnCancel.BorderColor = Color.FromArgb(225, 210, 190);
            btnCancel.BorderRadius = 7;
            btnCancel.BorderWidth = 1;
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnCancel.ForeColor = Color.FromArgb(80, 60, 40);
            btnCancel.HoverColor = Color.FromArgb(255, 247, 238);
            btnCancel.Location = new Point(330, 264);
            btnCancel.Name = "btnCancel";
            btnCancel.NormalColor = Color.White;
            btnCancel.PressedColor = Color.FromArgb(247, 230, 214);
            btnCancel.Size = new Size(74, 34);
            btnCancel.TabIndex = 7;
            btnCancel.Text = "Quay lại";
            // 
            // ForgotPasswordForm
            // 
            AcceptButton = btnSend;
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.White;
            CancelButton = btnCancel;
            ClientSize = new Size(430, 310);
            Controls.Add(btnCancel);
            Controls.Add(btnSend);
            Controls.Add(txtEmail);
            Controls.Add(lblEmail);
            Controls.Add(txtUsername);
            Controls.Add(lblUsername);
            Controls.Add(lblDescription);
            Controls.Add(lblTitle);
            Font = new Font("Segoe UI", 9.5F);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ForgotPasswordForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Quên mật khẩu";
            ResumeLayout(false);
            PerformLayout();
        }

        private Label lblTitle = null!;
        private Label lblDescription = null!;
        private Label lblUsername = null!;
        private Controls.CustomInputBox txtUsername = null!;
        private Label lblEmail = null!;
        private Controls.CustomInputBox txtEmail = null!;
        private Controls.ModernButton btnSend = null!;
        private Controls.ModernButton btnCancel = null!;
    }
}
