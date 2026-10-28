using System;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.SqlClient;

namespace BaiTap_WinForms.Data
{
    public static class DatabaseHelper
    {
        // Chuỗi kết nối đến SQL Server localhost\SQLEXPRESS01
        public static string ConnectionString { get; set; } = 
            "Server=localhost\\SQLEXPRESS01;Database=BanhMiShopDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        public static bool TestConnection(out string error)
        {
            error = string.Empty;
            try
            {
                using var conn = GetConnection();
                conn.Open();
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        public static DataTable ExecuteQuery(string query, SqlParameter[]? parameters = null)
        {
            using var conn = GetConnection();
            using var cmd = new SqlCommand(query, conn);
            if (parameters != null && parameters.Length > 0)
            {
                cmd.Parameters.AddRange(parameters);
            }

            using var adapter = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }

        public static int ExecuteNonQuery(string query, SqlParameter[]? parameters = null, CommandType commandType = CommandType.Text)
        {
            using var conn = GetConnection();
            using var cmd = new SqlCommand(query, conn)
            {
                CommandType = commandType
            };
            if (parameters != null && parameters.Length > 0)
            {
                cmd.Parameters.AddRange(parameters);
            }

            conn.Open();
            return cmd.ExecuteNonQuery();
        }

        public static object? ExecuteScalar(string query, SqlParameter[]? parameters = null, CommandType commandType = CommandType.Text)
        {
            using var conn = GetConnection();
            using var cmd = new SqlCommand(query, conn)
            {
                CommandType = commandType
            };
            if (parameters != null && parameters.Length > 0)
            {
                cmd.Parameters.AddRange(parameters);
            }

            conn.Open();
            return cmd.ExecuteScalar();
        }

        /// <summary>
        /// Băm mật khẩu bằng SHA-256 (khớp với HASHBYTES('SHA2_256', ...) trong SQL Server)
        /// </summary>
        public static byte[] HashPasswordSHA256(string password)
        {
            using var sha256 = SHA256.Create();
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }
}
