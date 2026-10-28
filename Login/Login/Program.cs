using Microsoft.EntityFrameworkCore;

namespace Login
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            ApplicationConfiguration.Initialize();

            // Tự động đảm bảo CSDL có dữ liệu mẫu ban đầu
            try
            {
                using var db = new DAL.BakeryDbContext();
                // Đồng bộ các thay đổi schema trước khi đọc/ghi dữ liệu tài khoản.
                db.Database.Migrate();
                DAL.DatabaseSeeder.SeedAsync(db).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                string message = DAL.DatabaseErrorMessage.From(ex);

                if (args != null && (args.Contains("--test") || args.Contains("--seed-only")))
                {
                    Console.Error.WriteLine(message);
                    Environment.ExitCode = 1;
                    return;
                }

                MessageBox.Show(message, "Lỗi kết nối cơ sở dữ liệu",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (args != null && args.Contains("--test"))
            {
                TestRunner.RunAllTestsAsync().GetAwaiter().GetResult();
                return;
            }

            if (args != null && args.Contains("--seed-only"))
            {
                return;
            }

            Application.Run(new Form1());
        }
    }
}
