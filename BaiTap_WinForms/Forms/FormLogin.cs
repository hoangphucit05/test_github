using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using BaiTap_WinForms.Data;
using BaiTap_WinForms.Models;

namespace BaiTap_WinForms.Forms
{
    public partial class FormLogin : Form
    {
        public FormLogin()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {
            txtUsername.Focus();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Tên đăng nhập và Mật khẩu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                byte[] hash = DatabaseHelper.HashPasswordSHA256(password);

                string query = @"
                    SELECT u.UserID, u.EmployeeID, u.Username, u.IsActive, 
                           r.RoleName, e.FullName, e.Position
                    FROM dbo.Users u
                    JOIN dbo.Roles r ON u.RoleID = r.RoleID
                    JOIN dbo.Employees e ON u.EmployeeID = e.EmployeeID
                    WHERE u.Username = @Username AND u.PasswordHash = @PasswordHash";

                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@Username", SqlDbType.VarChar) { Value = username },
                    new SqlParameter("@PasswordHash", SqlDbType.VarBinary) { Value = hash }
                };

                DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    bool isActive = Convert.ToBoolean(row["IsActive"]);
                    if (!isActive)
                    {
                        MessageBox.Show("Tài khoản của bạn đã bị khóa. Vui lòng liên hệ Quản trị viên!", "Khóa tài khoản", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Lưu session
                    UserSession.UserID = Convert.ToInt32(row["UserID"]);
                    UserSession.EmployeeID = Convert.ToInt32(row["EmployeeID"]);
                    UserSession.Username = row["Username"].ToString() ?? "";
                    UserSession.FullName = row["FullName"].ToString() ?? "";
                    UserSession.RoleName = row["RoleName"].ToString() ?? "";
                    UserSession.Position = row["Position"].ToString() ?? "";

                    // Cập nhật LastLogin
                    string updateLoginSql = "UPDATE dbo.Users SET LastLogin = SYSDATETIME() WHERE UserID = @UserID";
                    DatabaseHelper.ExecuteNonQuery(updateLoginSql, new SqlParameter[] { new SqlParameter("@UserID", UserSession.UserID) });

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Tên đăng nhập hoặc mật khẩu không chính xác!", "Đăng nhập thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtPassword.Clear();
                    txtPassword.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối cơ sở dữ liệu: " + ex.Message + "\nVui lòng kiểm tra lại SQL Server đang chạy!", "Lỗi kết nối", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !chkShowPassword.Checked;
        }

        private void btnTestConn_Click(object sender, EventArgs e)
        {
            if (DatabaseHelper.TestConnection(out string error))
            {
                MessageBox.Show("Kết nối tới CSDL BanhMiShopDB thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Không thể kết nối CSDL: " + error, "Lỗi kết nối", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
