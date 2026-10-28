using System;
using System.Data.Common;

namespace Login.DAL
{
    /// <summary>
    /// Converts database exceptions into a message that can be shown safely in the UI.
    /// </summary>
    public static class DatabaseErrorMessage
    {
        public static string From(Exception exception)
        {
            Exception current = exception;
            while (current.InnerException != null)
            {
                current = current.InnerException;
            }

            if (exception is DbException || current is DbException ||
                exception.Message.Contains("LocalDB", StringComparison.OrdinalIgnoreCase) ||
                current.Message.Contains("LocalDB", StringComparison.OrdinalIgnoreCase))
            {
                return "Không thể kết nối cơ sở dữ liệu BanhMiShopDB.\n\n" +
                       "SQL Server LocalDB (MSSQLLocalDB) hiện không khởi động được. " +
                       "Hãy kiểm tra dịch vụ LocalDB hoặc cập nhật chuỗi kết nối trong appsettings.json, rồi mở lại ứng dụng.\n\n" +
                       "Dữ liệu không được tải để tránh hiển thị số liệu không chính xác.";
            }

            return "Không thể tải dữ liệu từ cơ sở dữ liệu.\n\n" + current.Message;
        }
    }
}
