# Báo cáo chuẩn hóa WinForms Designer

Ngày kiểm tra: 22/10/2026

## Phạm vi đã kiểm tra

| Thành phần | Kết quả | Ghi chú |
| --- | --- | --- |
| `Form1` | Tương thích | Đã có đầy đủ cặp `Form1.cs`, `Form1.Designer.cs`, `Form1.resx`. Các control tĩnh được khai báo trong `InitializeComponent`. |
| `ForgotPasswordForm` | Đã chuyển đổi | Hộp thoại tĩnh trước đây tạo trong `Form1.cs` nay có các tệp `.cs`, `.Designer.cs`, `.resx` riêng. |
| `AdminDashboardForm` | Tương thích một phần | Vỏ Dashboard (sidebar, topbar, content host) nằm trong Designer. Các trang quản trị và dialog được tạo theo dữ liệu khi chạy. |
| `UserManagementView` | Đã chuyển đổi | Trang Quản lý tài khoản có `.cs`, `.Designer.cs`, `.resx`; tiêu đề, toolbar, DataGridView và pager chỉnh được bằng Designer. |
| `CustomInputBox` | Tương thích | `UserControl` không dùng CSDL hoặc service trong constructor; đã khai báo metadata Toolbox, thuộc tính mặc định và event mặc định. |
| `ModernDataGrid` | Tương thích | Kế thừa `DataGridView`, không có phụ thuộc runtime trong constructor và xuất hiện trong Toolbox. |
| `ModernButton`, `RoundedPanel`, `SidebarNavButton`, `RoleCardButton`, `ModernCheckBox`, `DividerLabel` | Tương thích | Custom control vẽ bằng GDI+, không kết nối CSDL hay khởi tạo service. |

## Thay đổi đã thực hiện

- Thêm `DesignTimeHelper` dùng `LicenseManager.UsageMode` và `Site.DesignMode`. Cách này hoạt động cả trong constructor, khi `Control.DesignMode` đơn lẻ thường chưa đáng tin cậy.
- `Form1` không tải ảnh từ thư mục output khi Visual Studio đang tạo Form; không ảnh hưởng ảnh khi chạy ứng dụng.
- Chuyển hộp thoại quên mật khẩu từ UI tạo động sang `ForgotPasswordForm` có thể chỉnh bằng Visual Studio Designer; validation và nội dung thông báo được giữ nguyên.
- `AdminDashboardForm` vẫn hiển thị layout và trang chủ mẫu trong Designer, nhưng không đăng ký navigation runtime, không chạy migration và không mở kết nối SQL Server trong design-time.
- `_contentPanel` của Dashboard được đặt `DockStyle.Fill`, vì vậy mọi trang chức năng mở rộng theo toàn bộ ô nội dung của `tableRight` khi cửa sổ thay đổi kích thước.
- Việc tải thống kê của trang chủ Dashboard chỉ chạy runtime.
- `Form1` dùng `AutoScaleMode.Dpi`; hai vùng co giãn theo `TableLayoutPanel`, còn các trường đăng nhập tự căn giữa và thu gọn trong giới hạn đọc tốt.
- `Form1` và `AdminDashboardForm` dùng cửa sổ co giãn (`Sizable`) và cho phép cực đại hóa. Khung chính sử dụng `TableLayoutPanel`/`Dock`; nội dung Dashboard có thanh cuộn khi không đủ không gian thay vì bị che.
- Poster đăng nhập dùng `PictureBoxSizeMode.Zoom` để giữ đúng tỉ lệ khi kích thước Form thay đổi.
- Trang **Phân quyền vai trò** dành 260 px cho danh sách vai trò và ưu tiên phần diện tích còn lại cho ma trận quyền. Danh sách quyền dùng một `FlowLayoutPanel` cuộn dọc; mỗi nhóm dùng `TableLayoutPanel` một cột với chiều cao xác định theo số quyền, nên checkbox không bị cắt khi thay đổi kích thước.
- Trang **Quản lý tài khoản** được tách thành `UserManagementView` Designer-first. Toolbar tự chuyển từ một hàng sang hai hàng khi vùng hiển thị hẹp; DataGridView lấp đầy phần còn lại và có 8 cột dữ liệu khai báo trực tiếp trong Designer thay vì tự sinh. Cột chi nhánh được ưu tiên 220 px, ô dữ liệu có tooltip, còn pager dùng `TableLayoutPanel` để giữ thông tin bên trái và nút trang bên phải.
- Không thay đổi schema, connection string, migration, xác thực hoặc service.

## Phần nên tiếp tục quản lý bằng C# runtime

- Các trang người dùng, vai trò, chi nhánh và ca làm của `AdminDashboardForm`: chúng tạo grid, cột, dữ liệu, quyền và event dựa trên CSDL.
- Các dialog thêm/sửa và danh sách checkbox quyền: nội dung phụ thuộc bản ghi và quyền trong CSDL.
- Entity Framework Core, migrations, BCrypt, xác thực, truy vấn LINQ và toàn bộ BLL/DAL.

## Kết quả kiểm tra

- `dotnet build Login.slnx --no-restore`: thành công, 0 lỗi, 0 cảnh báo.
- Chưa thể xác minh thao tác mở Designer thực tế vì môi trường hiện tại không chạy Visual Studio. Build thành công không được xem là bằng chứng Designer đã mở thành công.

## Kiểm tra thủ công trong Visual Studio

1. Mở `Login.slnx` với Visual Studio đã cài workload **.NET desktop development** và .NET 10 SDK.
2. Chuột phải `Form1.cs` và `AdminDashboardForm.cs` → **View Designer**.
3. Xác nhận Designer mở khi SQL Server/LocalDB tắt hoặc không có database; Dashboard chỉ hiện preview tĩnh và không hiện lỗi kết nối.
4. Chọn `ModernDataGrid` trong Toolbox, thêm vào một Form thử nghiệm, rồi mở **Edit Columns** để kiểm tra UI cấu hình DataGridView kế thừa.
5. Chạy ở DPI 100%, 125% và 150%; kiểm tra Form đăng nhập không bị chồng hai card và Dashboard vẫn hiển thị sidebar/topbar/content.
