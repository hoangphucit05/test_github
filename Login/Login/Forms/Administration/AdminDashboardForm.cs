using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Login.BLL.DTOs;
using Login.BLL.Services;
using Login.Controls;
using Login.DAL;
using Login.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Login
{
    /// <summary>
    /// Dashboard quản trị viên — xây dựng theo thiết kế "Bánh Mì Ngon".
    /// Gồm: Trang chủ · Quản lý tài khoản · Phân quyền vai trò · Quản lý chi nhánh · Quản lý ca làm.
    /// </summary>
    public partial class AdminDashboardForm : Form
    {
        // ═══════════════════ CONSTANTS / THEME ═══════════════════
        private static readonly Color C_Sidebar = Color.FromArgb(117, 58, 18);
        private static readonly Color C_SidebarDark = Color.FromArgb(90, 40, 10);
        private static readonly Color C_Accent = Color.FromArgb(177, 91, 25);
        private static readonly Color C_AccentLight = Color.FromArgb(255, 185, 100);
        private static readonly Color C_BgMain = Color.FromArgb(248, 245, 240);
        private static readonly Color C_BgCard = Color.White;
        private static readonly Color C_TextDark = Color.FromArgb(50, 35, 15);
        private static readonly Color C_TextGray = Color.FromArgb(120, 100, 80);
        private static readonly Color C_Success = Color.FromArgb(39, 174, 96);
        private static readonly Color C_Danger = Color.FromArgb(192, 57, 43);
        private static readonly Color C_Border = Color.FromArgb(225, 210, 190);
        private static readonly Font F_Title = new("Segoe UI", 17, FontStyle.Bold);
        private static readonly Font F_Sub = new("Segoe UI", 9f);
        private static readonly Font F_Btn = new("Segoe UI", 9.5f, FontStyle.Bold);
        private static readonly Font F_BtnSm = new("Segoe UI", 9f, FontStyle.Bold);
        private static readonly Font F_Header = new("Segoe UI Semibold", 10.5f);
        private static readonly Font F_Label = new("Segoe UI", 9.5f);

        // ═══════════════════ FIELDS ═══════════════════
        private readonly UserSession _session;

        // Dynamic page tracker
        private Panel _currentPage = null!;

        // Grids
        private ModernDataGrid _usersGrid = null!;
        private ModernDataGrid _branchesGrid = null!;
        private ModernDataGrid _shiftsGrid = null!;

        // Users page controls
        private UserManagementView _usersView = null!;
        private TextBox _searchUsers = null!;
        private ComboBox _filterStatus = null!;

        // Branches page controls
        private TextBox _searchBranches = null!;
        private ComboBox _filterBranchStatus = null!;

        // Shifts page controls
        private TextBox _searchShifts = null!;
        private ComboBox _filterShiftStatus = null!;

        // Permissions page
        private ListBox _roleList = null!;
        private Panel _roleDetailPanel = null!;
        private TextBox _roleNameBox = null!;
        private TextBox _roleDescBox = null!;

        private Label _selectedRoleLabel = null!;

        private List<Role> _roles = new();
        private List<Permission> _allPermissions = new();
        private Role? _editingRole;

        // A newer filter/search request must always win over an older, slower query.
        private int _usersLoadVersion;
        private int _branchesLoadVersion;
        private int _shiftsLoadVersion;
        private int _roleDetailLoadVersion;
        private bool _databaseErrorShown;

        // ═══════════════════ CONSTRUCTOR ═══════════════════
        /// <summary>
        /// Constructor mặc định phục vụ cho Visual Studio Form Designer và kiểm thử giao diện.
        /// </summary>
        public AdminDashboardForm() : this(new UserSession
        {
            UserID = 1,
            Username = "admin",
            FullName = "Quản trị viên",
            RoleName = "ADMIN",
            BranchName = "Chi nhánh Bến Nghé"
        })
        {
        }

        public AdminDashboardForm(UserSession session)
        {
            _session = session;

            InitializeComponent();
            SetupCustomStylesAndIcons();
            ShowPage(BuildHomePage());

            // InitializeComponent and the static home preview remain available in the
            // WinForms Designer. Navigation/database work is strictly runtime-only.
            if (DesignTimeHelper.IsDesignTime(this))
            {
                return;
            }

            SetupNavEvents();

            Shown += async (_, _) =>
            {
                await LoadAllAsync();
            };
        }

        // ═══════════════════ DESIGN & EVENT SETUP ═══════════════════
        private void SetupCustomStylesAndIcons()
        {
            lblUserName.Text = _session.FullName;
            lblUserRole.Text = "Quản trị viên";

            // Logo icon background circle & image
            logoPanel.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.FillEllipse(new SolidBrush(C_Accent), 14, 18, 44, 44);
            };

            string logoPath = Path.Combine(AppContext.BaseDirectory, "Assets", "Images", "bread_icon_trans.png");
            if (!File.Exists(logoPath)) logoPath = Path.Combine(AppContext.BaseDirectory, "Assets", "Images", "bread_icon.png");
            if (File.Exists(logoPath))
            {
                try { picLogo.Image = Image.FromFile(logoPath); } catch { }
            }

            // Logout panel border line
            logoutPanel.Paint += (s, e) =>
            {
                e.Graphics.DrawLine(new Pen(Color.FromArgb(80, 160, 90, 40)), 14, 0, logoutPanel.Width - 14, 0);
            };

            // Topbar bottom border
            _topbar.Paint += (s, e) =>
            {
                e.Graphics.DrawLine(new Pen(C_Border), 0, _topbar.Height - 1, _topbar.Width, _topbar.Height - 1);
            };

            // Topbar breadcrumb icon & circle
            breadPanel.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.FillEllipse(new SolidBrush(C_Accent), 8, 18, 30, 30);
            };
            if (File.Exists(logoPath))
            {
                try { picTopBread.Image = Image.FromFile(logoPath); } catch { }
            }

            // Topbar avatar circle
            avatarPanel.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.FillEllipse(new SolidBrush(C_Accent), 0, 0, 37, 37);
                string initials = _session.FullName.Length > 0 ? _session.FullName[0].ToString().ToUpper() : "A";
                using var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(initials, new Font("Segoe UI", 13, FontStyle.Bold), Brushes.White,
                    new RectangleF(0, 0, 37, 37), sf);
            };

            // Topbar: reposition right-side controls on resize
            _topbar.Resize += (s, e) =>
            {
                int right = _topbar.ClientSize.Width;
                avatarPanel.Left = right - avatarPanel.Width - 12;
                lblUserName.Left = avatarPanel.Left - lblUserName.Width - 8;
                lblUserRole.Left = avatarPanel.Left - lblUserRole.Width - 8;
            };

            // Load icons for nav items
            LoadNavIcon(_navHome, "icon_home_clean.png");
            LoadNavIcon(_navUsers, "icon_users_clean.png");
            LoadNavIcon(_navRoles, "icon_shield_clean.png");
            LoadNavIcon(_navBranches, "icon_store_clean.png");
            LoadNavIcon(_navShifts, "icon_shift_clean.png");
            LoadNavIcon(_logoutBtn, "icon_lock_white.png");
        }

        private static void LoadNavIcon(SidebarNavButton btn, string iconFileName)
        {
            string path = Path.Combine(AppContext.BaseDirectory, "Assets", "Icons", iconFileName);
            if (File.Exists(path))
            {
                try { btn.NavIcon = Image.FromFile(path); } catch { }
            }
        }

        private void SetupNavEvents()
        {
            _logoutBtn.Click += (_, _) =>
            {
                if (MessageBox.Show("Bạn có muốn đăng xuất?", "Xác nhận",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    Close();
            };

            _navHome.Click += (_, _) => { SelectNav(_navHome); ShowPage(BuildHomePage()); };
            _navUsers.Click += (_, _) => { SelectNav(_navUsers); ShowPage(BuildUsersPage()); };
            _navRoles.Click += (_, _) => { SelectNav(_navRoles); ShowPage(BuildRolesPage()); };
            _navBranches.Click += (_, _) => { SelectNav(_navBranches); ShowPage(BuildBranchesPage()); };
            _navShifts.Click += (_, _) => { SelectNav(_navShifts); ShowPage(BuildShiftsPage()); };
        }

        private SidebarNavButton[] AllNavs => new[] { _navHome, _navUsers, _navRoles, _navBranches, _navShifts };

        private void SelectNav(SidebarNavButton selected)
        {
            foreach (var n in AllNavs) n.IsSelected = false;
            selected.IsSelected = true;
        }

        // ─── PAGE SWITCHER ───
        private void ShowPage(Panel page)
        {
            _contentPanel.Controls.Clear();
            page.Dock = DockStyle.Fill;
            _contentPanel.Controls.Add(page);
            _currentPage = page;
        }

        // ═══════════════════ PAGES ═══════════════════

        // ─── HOME PAGE ───
        private Panel BuildHomePage()
        {
            var p = new Panel { BackColor = C_BgMain };

            var title = MakePageTitle("Trang chủ", "Chào mừng bạn đến với hệ thống quản trị Bánh Mì Ngon!");

            // Stat cards
            var cardsFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 130,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = C_BgMain,
                Padding = new Padding(0, 12, 0, 0)
            };

            // Sẽ load số liệu async
            var cardUsers = MakeStatCard("Người dùng", "—", C_Accent);
            var cardBranches = MakeStatCard("Chi nhánh", "—", Color.FromArgb(41, 128, 185));
            var cardShifts = MakeStatCard("Ca làm việc", "—", Color.FromArgb(39, 174, 96));
            var cardRoles = MakeStatCard("Vai trò", "—", Color.FromArgb(142, 68, 173));

            cardsFlow.Controls.AddRange(new Control[] { cardUsers, cardBranches, cardShifts, cardRoles });

            // Welcome banner
            var banner = new Panel
            {
                Dock = DockStyle.Top,
                Height = 120,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 8, 0, 0)
            };
            banner.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using var gp = new GraphicsPath();
                var rect = new Rectangle(0, 0, banner.Width - 1, banner.Height - 1);
                int r = 12;
                gp.AddArc(rect.X, rect.Y, r, r, 180, 90);
                gp.AddArc(rect.Right - r, rect.Y, r, r, 270, 90);
                gp.AddArc(rect.Right - r, rect.Bottom - r, r, r, 0, 90);
                gp.AddArc(rect.X, rect.Bottom - r, r, r, 90, 90);
                gp.CloseFigure();
                using var gradBrush = new LinearGradientBrush(rect,
                    Color.FromArgb(177, 91, 25), Color.FromArgb(117, 58, 18), 135f);
                g.FillPath(gradBrush, gp);
            };

            var bannerText = new Label
            {
                Text = $"Xin chào, {_session.FullName}!\nHệ thống đang hoạt động bình thường.",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(24, 0, 0, 0)
            };
            var bannerSub = new Label
            {
                Text = $"Vai trò: Quản trị viên  •  Đăng nhập lúc: {_session.LastLogin?.ToString("HH:mm dd/MM/yyyy") ?? "Vừa xong"}",
                ForeColor = Color.FromArgb(255, 220, 180),
                Font = new Font("Segoe UI", 9.5f),
                Dock = DockStyle.Bottom,
                Height = 28,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(24, 0, 0, 0)
            };
            banner.Controls.Add(bannerText);
            banner.Controls.Add(bannerSub);
            // Data belongs to the runtime page. The Designer keeps the static cards
            // editable and never opens a SQL Server connection.
            if (!DesignTimeHelper.IsDesignTime(this))
            {
                _ = LoadHomeStatsAsync(cardUsers, cardBranches, cardShifts, cardRoles);
            }

            var layout = CreatePageLayout(68, 130, 120, 0);
            AddToPageLayout(layout, title, 0);
            AddToPageLayout(layout, cardsFlow, 1);
            AddToPageLayout(layout, banner, 2);
            p.Controls.Add(layout);

            return p;
        }

        private async Task LoadHomeStatsAsync(Panel cardUsers, Panel cardBranches, Panel cardShifts, Panel cardRoles)
        {
            try
            {
                using var db = new BakeryDbContext();
                int users = await db.Users.CountAsync();
                int branches = await db.Branches.CountAsync(b => b.IsActive);
                int shifts = await db.Shifts.CountAsync(s => s.IsActive);
                int roles = await db.Roles.CountAsync();

                if (cardUsers.IsDisposed || cardBranches.IsDisposed ||
                    cardShifts.IsDisposed || cardRoles.IsDisposed)
                    return;

                UpdateStatCard(cardUsers, users.ToString());
                UpdateStatCard(cardBranches, branches.ToString());
                UpdateStatCard(cardShifts, shifts.ToString());
                UpdateStatCard(cardRoles, roles.ToString());
            }
            catch (Exception ex)
            {
                SetStatCardsUnavailable(cardUsers, cardBranches, cardShifts, cardRoles);

                if (!_databaseErrorShown && !IsDisposed)
                {
                    _databaseErrorShown = true;
                    ShowError(DatabaseErrorMessage.From(ex));
                }
            }
        }

        private static void SetStatCardsUnavailable(params Panel[] cards)
        {
            foreach (Panel card in cards)
            {
                if (!card.IsDisposed)
                    UpdateStatCard(card, "—");
            }
        }

        private Panel MakeStatCard(string label, string value, Color accentColor)
        {
            var card = new Panel
            {
                Size = new Size(230, 108),
                Margin = new Padding(0, 0, 16, 0),
                BackColor = C_BgCard
            };

            card.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using var gp = BuildRoundedRect(new Rectangle(0, 0, card.Width - 1, card.Height - 1), 12);
                g.FillPath(new SolidBrush(C_BgCard), gp);
                g.DrawPath(new Pen(C_Border), gp);
                // Accent strip
                g.FillRectangle(new SolidBrush(accentColor), 0, 0, 6, card.Height);
            };

            var valLbl = new Label
            {
                Name = "valLbl",
                Text = value,
                Font = new Font("Segoe UI", 26, FontStyle.Bold),
                ForeColor = accentColor,
                AutoSize = false,
                Bounds = new Rectangle(20, 12, 180, 50),
                TextAlign = ContentAlignment.MiddleLeft
            };
            var nameLbl = new Label
            {
                Text = label,
                Font = new Font("Segoe UI", 10),
                ForeColor = C_TextGray,
                AutoSize = false,
                Bounds = new Rectangle(20, 62, 180, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };
            card.Controls.AddRange(new Control[] { valLbl, nameLbl });
            return card;
        }

        private static void UpdateStatCard(Panel card, string value)
        {
            foreach (Control c in card.Controls)
            {
                if (c.Name == "valLbl") c.Text = value;
            }
        }

        // ─── USERS PAGE ───
        private Panel BuildUsersPage()
        {
            var p = new Panel { BackColor = C_BgMain };

            _usersView = new UserManagementView { Dock = DockStyle.Fill };
            _searchUsers = _usersView.SearchBox;
            _filterStatus = _usersView.StatusFilter;
            _usersGrid = _usersView.UsersGrid;

            _usersView.AddButton.Click += async (_, _) => await ShowAddUserDialogAsync();
            _usersView.EditButton.Click += async (_, _) => await ShowEditUserDialogAsync();
            _usersView.LockButton.Click += async (_, _) => await ToggleSelectedUserAsync();

            _searchUsers.TextChanged += async (_, _) => await LoadUsersAsync();
            _filterStatus.SelectedIndexChanged += async (_, _) => await LoadUsersAsync();

            _usersGrid.RowEditClicked += async (s, idx) => await ShowEditUserDialogAsync();
            _usersGrid.RowDeleteClicked += async (s, idx) => await ToggleSelectedUserAsync();
            _usersGrid.CellToolTipTextNeeded += (_, e) =>
            {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                    e.ToolTipText = _usersGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString();
            };

            p.Controls.Add(_usersView);

            _ = LoadUsersAsync();
            return p;
        }

        private async Task LoadUsersAsync()
        {
            int loadVersion = ++_usersLoadVersion;
            var grid = _usersGrid;
            try
            {
                string kw = GetSearchKeyword(_searchUsers);
                bool? active = _filterStatus?.SelectedIndex switch
                {
                    1 => true,
                    2 => false,
                    _ => null
                };

                using var db = new BakeryDbContext();
                var list = await new UserService(db).GetAllUsersAsync(kw, null, active);

                if (loadVersion != _usersLoadVersion || grid.IsDisposed ||
                    !ReferenceEquals(grid, _usersGrid))
                    return;

                if (grid.InvokeRequired)
                    grid.Invoke(() => BindUsersGrid(list));
                else
                    BindUsersGrid(list);
            }
            catch (Exception ex) { ShowError(DatabaseErrorMessage.From(ex)); }
        }

        private void BindUsersGrid(List<User> list)
        {
            _usersGrid.DataSource = list.Select((u, i) => new
            {
                STT = i + 1,
                UserID = u.UserID,
                Tên_đăng_nhập = u.Username,
                Nhân_viên = u.Employee?.FullName ?? "—",
                Vai_trò = u.Role?.RoleName ?? "—",
                Chi_nhánh = u.Employee?.Branch?.BranchName ?? "Hệ thống",
                Trạng_thái = u.IsActive ? "Đang hoạt động" : "Đã tạm khóa",
                Ngày_tạo = u.CreatedAt.ToString("dd/MM/yyyy"),
                Thao_tác = ""
            }).ToList();

            _usersView.SetRecordCount(list.Count);
        }

        private async Task ShowAddUserDialogAsync()
        {
            using var db = new BakeryDbContext();
            var employees = await new UserService(db).GetEmployeesWithoutAccountAsync();
            var roles = await db.Roles.OrderBy(r => r.RoleName).ToListAsync();
            var branches = await db.Branches.Where(b => b.IsActive).OrderBy(b => b.BranchName).ToListAsync();

            using var dlg = BuildAddUserDialog(employees, roles, branches);
            if (dlg.ShowDialog(this) == DialogResult.OK) await LoadUsersAsync();
        }

        private Form BuildAddUserDialog(List<Employee> employees, List<Role> roles, List<Branch> branches)
        {
            var dlg = MakeDialog("Thêm tài khoản", 500, 460);

            // Fields
            var cmbEmployee = MakeCombo(employees, "FullName", "EmployeeID");
            var cmbRole = MakeCombo(roles, "RoleName", "RoleID");
            var cmbBranch = MakeCombo(branches, "BranchName", "BranchID", nullable: true);
            var txtUsername = MakeTextField();
            var txtPassword = MakeTextField(password: true);
            var txtConfirm = MakeTextField(password: true);
            var rdoActive = new RadioButton { Text = "Hoạt động", Checked = true, AutoSize = true };
            var rdoInactive = new RadioButton { Text = "Tạm khóa", AutoSize = true };

            var fields = new (string Label, Control Ctrl)[]
            {
                ("Nhân viên *",        cmbEmployee),
                ("Tên đăng nhập *",    txtUsername),
                ("Mật khẩu *",         txtPassword),
                ("Xác nhận mật khẩu *", txtConfirm),
                ("Vai trò *",          cmbRole),
                ("Chi nhánh *",        cmbBranch),
            };

            int y = 20;
            foreach (var (lbl, ctrl) in fields)
            {
                AddFormField(dlg, lbl, ctrl, ref y);
            }

            // Trạng thái
            var statusPanel = new FlowLayoutPanel { Left = 160, Top = y, AutoSize = true, BackColor = Color.Transparent };
            statusPanel.Controls.AddRange(new Control[] { rdoActive, rdoInactive });
            dlg.Controls.Add(new Label { Text = "Trạng thái", Left = 20, Top = y + 2, AutoSize = true, Font = F_Label });
            dlg.Controls.Add(statusPanel);

            // Buttons
            var btnSave = MakeDialogButton("💾  Lưu", C_Accent);
            var btnCancel = MakeDialogButton("↩  Hủy", C_TextGray);
            btnSave.DialogResult = DialogResult.None;
            btnCancel.DialogResult = DialogResult.Cancel;

            btnSave.Click += async (_, _) =>
            {
                string username = txtUsername.Text.Trim();
                string password = txtPassword.Text;
                string confirm = txtConfirm.Text;

                if (cmbEmployee.SelectedValue is not int empId) { ShowWarn(dlg, "Vui lòng chọn nhân viên!"); return; }
                if (string.IsNullOrWhiteSpace(username)) { ShowWarn(dlg, "Vui lòng nhập tên đăng nhập!"); return; }
                if (string.IsNullOrWhiteSpace(password)) { ShowWarn(dlg, "Vui lòng nhập mật khẩu!"); return; }
                if (password != confirm) { ShowWarn(dlg, "Mật khẩu xác nhận không khớp!"); return; }
                if (cmbRole.SelectedValue is not int roleId) { ShowWarn(dlg, "Vui lòng chọn vai trò!"); return; }

                using var db = new BakeryDbContext();
                int? branchId = cmbBranch.SelectedValue is int selectedBranchId
                    ? selectedBranchId
                    : null;
                var result = await new UserService(db).CreateUserForEmployeeAsync(
                    _session.UserID, empId, roleId, username, password, branchId, rdoActive.Checked);

                if (!result.Success) { ShowWarn(dlg, result.Message); return; }

                MessageBox.Show(result.Message, "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                dlg.DialogResult = DialogResult.OK;
            };

            LayoutDialogButtons(dlg, btnSave, btnCancel);
            return dlg;
        }

        private async Task ShowEditUserDialogAsync()
        {
            if (!TryGetSelectedId(_usersGrid, "UserID", out int userId)) return;

            using var db0 = new BakeryDbContext();
            var user = await db0.Users.Include(u => u.Employee).Include(u => u.Role)
                                      .FirstOrDefaultAsync(u => u.UserID == userId);
            if (user == null) return;

            using var dlg = MakeDialog("Sửa tài khoản", 460, 320);
            var txtPassword = MakeTextField(password: true);
            var rdoActive = new RadioButton { Text = "Hoạt động", Checked = user.IsActive, AutoSize = true };
            var rdoInactive = new RadioButton { Text = "Tạm khóa", Checked = !user.IsActive, AutoSize = true };

            int y = 20;
            var lblInfo = new Label
            {
                Text = $"Tài khoản: {user.Username}  •  Nhân viên: {user.Employee?.FullName}",
                Left = 20,
                Top = y,
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = C_Accent
            };
            dlg.Controls.Add(lblInfo);
            y += 36;

            AddFormField(dlg, "Mật khẩu mới\n(bỏ trống = giữ nguyên)", txtPassword, ref y);
            var statusPanel = new FlowLayoutPanel { Left = 160, Top = y, AutoSize = true, BackColor = Color.Transparent };
            statusPanel.Controls.AddRange(new Control[] { rdoActive, rdoInactive });
            dlg.Controls.Add(new Label { Text = "Trạng thái", Left = 20, Top = y + 2, AutoSize = true, Font = F_Label });
            dlg.Controls.Add(statusPanel);

            var btnSave = MakeDialogButton("💾  Lưu", C_Accent);
            var btnCancel = MakeDialogButton("↩  Hủy", C_TextGray);
            btnCancel.DialogResult = DialogResult.Cancel;
            btnSave.DialogResult = DialogResult.None;

            btnSave.Click += async (_, _) =>
            {
                using var db = new BakeryDbContext();
                var targetUser = await db.Users.FindAsync(userId);
                if (targetUser == null) return;

                // Đặt lại mật khẩu nếu có nhập
                if (!string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    var res = await new UserService(db).ResetPasswordByAdminAsync(_session.UserID, userId, txtPassword.Text);
                    if (!res.Success) { ShowWarn(dlg, res.Message); return; }
                }

                // Toggle nếu khác
                if (targetUser.IsActive != rdoActive.Checked)
                {
                    var res = await new UserService(db).ToggleUserStatusAsync(_session.UserID, userId);
                    if (!res.Success) { ShowWarn(dlg, res.Message); return; }
                }

                dlg.DialogResult = DialogResult.OK;
            };

            LayoutDialogButtons(dlg, btnSave, btnCancel);
            if (dlg.ShowDialog(this) == DialogResult.OK) await LoadUsersAsync();
        }

        private async Task ToggleSelectedUserAsync()
        {
            if (!TryGetSelectedId(_usersGrid, "UserID", out int id)) return;

            string currentStatus = GetGridCellValue(_usersGrid, "Trạng_thái");
            bool isActive = currentStatus == "Đang hoạt động";
            string action = isActive ? "khóa" : "kích hoạt";

            if (MessageBox.Show($"Xác nhận {action} tài khoản này?", "Xác nhận",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            using var db = new BakeryDbContext();
            var result = await new UserService(db).ToggleUserStatusAsync(_session.UserID, id);
            ShowResult(result.Success, result.Message);
            if (result.Success) await LoadUsersAsync();
        }

        // ─── ROLES / PERMISSIONS PAGE ───
        private Panel BuildRolesPage()
        {
            var p = new Panel { BackColor = C_BgMain };
            var title = MakePageTitle("Phân quyền vai trò", "Trang chủ  ›  Phân quyền vai trò");

            var split = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = C_BgMain
            };
            // Keep the role list compact so the editable permission matrix receives
            // the remaining width of the dashboard.
            split.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 260));
            split.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            // ── Left: role list ──
            var leftCard = new Panel { Dock = DockStyle.Fill, BackColor = C_BgCard, Padding = new Padding(0) };
            leftCard.Paint += PaintCard;

            var leftHeader = new Panel { Dock = DockStyle.Top, Height = 52, BackColor = C_BgCard, Padding = new Padding(0, 8, 12, 8) };
            var btnAddRole = MakeSmallButton("+ Thêm vai trò", C_Accent);
            btnAddRole.Click += async (_, _) => await ShowAddRoleDialogAsync();
            btnAddRole.Dock = DockStyle.Right;
            btnAddRole.Width = 110;

            var leftTitle = new Label
            {
                Text = "Danh sách vai trò",
                Font = F_Header,
                ForeColor = C_TextDark,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(14, 0, 0, 0)
            };

            leftHeader.Controls.Add(btnAddRole);
            leftHeader.Controls.Add(leftTitle);

            _roleList = new ListBox
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10.5f),
                BackColor = C_BgCard,
                DrawMode = DrawMode.OwnerDrawFixed,
                ItemHeight = 54
            };
            _roleList.DrawItem += RoleList_DrawItem;
            _roleList.SelectedIndexChanged += async (_, _) => await LoadRoleDetailAsync();

            var leftLayout = CreatePageLayout(52, 0);
            AddToPageLayout(leftLayout, leftHeader, 0);
            AddToPageLayout(leftLayout, _roleList, 1);
            leftCard.Controls.Add(leftLayout);

            split.Controls.Add(leftCard, 0, 0);

            // ── Right: detail ──
            _roleDetailPanel = new Panel { Dock = DockStyle.Fill, BackColor = C_BgCard, Padding = new Padding(16, 18, 16, 20) };
            _roleDetailPanel.Paint += PaintCard;

            split.Controls.Add(_roleDetailPanel, 1, 0);
            var layout = CreatePageLayout(68, 0);
            AddToPageLayout(layout, title, 0);
            AddToPageLayout(layout, split, 1);
            p.Controls.Add(layout);

            _ = LoadRolesListAsync();
            return p;
        }

        private async Task LoadRolesListAsync()
        {
            try
            {
                using var db = new BakeryDbContext();
                _roles = await db.Roles.OrderBy(r => r.RoleName).ToListAsync();
                _roleList.DataSource = null;
                _roleList.DataSource = _roles;
                _roleList.DisplayMember = "RoleName";
                if (_roles.Count > 0)
                    await LoadRoleDetailAsync();
            }
            catch (Exception ex) { ShowError(DatabaseErrorMessage.From(ex)); }
        }

        private void RoleList_DrawItem(object? sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= _roleList.Items.Count) return;
            var role = (Role)_roleList.Items[e.Index];

            bool selected = (e.State & DrawItemState.Selected) != 0;
            e.Graphics.FillRectangle(new SolidBrush(selected ? Color.FromArgb(255, 237, 218) : C_BgCard),
                e.Bounds);

            if (selected)
                e.Graphics.FillRectangle(new SolidBrush(C_Accent),
                    new Rectangle(0, e.Bounds.Y, 4, e.Bounds.Height));

            string icon = role.RoleName == "ADMIN" ? "🛡" : (role.RoleName == "MANAGER" ? "👥" : "🏪");
            e.Graphics.DrawString($"{icon}  {role.RoleName}",
                new Font("Segoe UI", 10.5f, selected ? FontStyle.Bold : FontStyle.Regular),
                new SolidBrush(selected ? C_Accent : C_TextDark),
                e.Bounds.Left + 14, e.Bounds.Top + 10);

            if (!string.IsNullOrEmpty(role.Description))
                e.Graphics.DrawString(role.Description,
                    new Font("Segoe UI", 8.5f), new SolidBrush(C_TextGray),
                    e.Bounds.Left + 42, e.Bounds.Top + 30);
        }

        private async Task LoadRoleDetailAsync()
        {
            if (_roleList.SelectedItem is not Role role) return;
            int loadVersion = ++_roleDetailLoadVersion;
            var detailPanel = _roleDetailPanel;
            _editingRole = role;
            detailPanel.Controls.Clear();

            try
            {
                using var db = new BakeryDbContext();
                _allPermissions = await db.Permissions
                    .OrderBy(p => p.GroupName).ThenBy(p => p.PermissionName)
                    .ToListAsync();
                var assigned = await db.RolePermissions
                    .Where(rp => rp.RoleID == role.RoleID)
                    .Select(rp => rp.PermissionID)
                    .ToListAsync();

                if (loadVersion != _roleDetailLoadVersion || detailPanel.IsDisposed ||
                    !ReferenceEquals(detailPanel, _roleDetailPanel) ||
                    _roleList.SelectedItem is not Role selectedRole || selectedRole.RoleID != role.RoleID)
                    return;

                bool isSystemRole = role.RoleName is "ADMIN" or "MANAGER" or "STAFF";

                // ── Root container inside detail card ──
                var rootTable = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    RowCount = 2,
                    ColumnCount = 1,
                    BackColor = C_BgCard,
                    Padding = Padding.Empty,
                    Margin = Padding.Empty
                };
                rootTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 46));
                rootTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

                // 1. Header Bar
                var headerPanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = C_BgCard,
                    Padding = new Padding(0, 0, 0, 8)
                };
                headerPanel.Paint += (s, e) =>
                {
                    e.Graphics.DrawLine(new Pen(C_Border), 0, headerPanel.Height - 1, headerPanel.Width, headerPanel.Height - 1);
                };

                _selectedRoleLabel = new Label
                {
                    Text = $"Chi tiết vai trò: {role.RoleName}",
                    Font = new Font("Segoe UI", 12.5f, FontStyle.Bold),
                    ForeColor = C_TextDark,
                    Dock = DockStyle.Left,
                    AutoSize = true,
                    TextAlign = ContentAlignment.MiddleLeft
                };
                headerPanel.Controls.Add(_selectedRoleLabel);

                if (isSystemRole)
                {
                    var badge = new Label
                    {
                        Text = "Vai trò hệ thống",
                        Font = new Font("Segoe UI", 8.25f, FontStyle.Bold),
                        ForeColor = C_Accent,
                        BackColor = Color.FromArgb(255, 238, 222),
                        Dock = DockStyle.Left,
                        AutoSize = false,
                        Width = 115,
                        Height = 24,
                        TextAlign = ContentAlignment.MiddleCenter
                    };
                    badge.Paint += (s, e) =>
                    {
                        using var pen = new Pen(Color.FromArgb(240, 180, 130));
                        e.Graphics.DrawRectangle(pen, 0, 0, badge.Width - 1, badge.Height - 1);
                    };
                    headerPanel.Controls.Add(new Panel { Dock = DockStyle.Left, Width = 12 });
                    headerPanel.Controls.Add(badge);
                }

                rootTable.Controls.Add(headerPanel, 0, 0);

                // 2. Body Table: 2 Columns (Left: Form, Right: Permissions Matrix)
                var bodyTable = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    RowCount = 1,
                    ColumnCount = 2,
                    BackColor = C_BgCard,
                    Padding = new Padding(0, 12, 0, 0),
                    Margin = Padding.Empty
                };
                // The details form only needs a compact column. Permissions are the
                // primary working area and therefore use all remaining space.
                bodyTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 260));
                bodyTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

                // ── Left Column: Role Details Form ──
                var leftForm = new Panel
                {
                    Dock = DockStyle.Fill,
                    Padding = new Padding(0, 0, 16, 0),
                    BackColor = C_BgCard
                };

                var lblName = new Label
                {
                    Text = "TÊN VAI TRÒ *",
                    Dock = DockStyle.Top,
                    Height = 24,
                    Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                    ForeColor = C_TextGray
                };
                _roleNameBox = new TextBox
                {
                    Dock = DockStyle.Top,
                    Height = 32,
                    Font = F_Label,
                    Text = role.RoleName,
                    BackColor = isSystemRole ? Color.FromArgb(242, 239, 235) : Color.FromArgb(253, 248, 243),
                    ReadOnly = isSystemRole,
                    BorderStyle = BorderStyle.FixedSingle
                };

                var nameNote = new Label
                {
                    Text = isSystemRole ? "🔒 Vai trò hệ thống mặc định không thể đổi tên" : "",
                    Dock = DockStyle.Top,
                    Height = isSystemRole ? 24 : 10,
                    Font = new Font("Segoe UI", 8f),
                    ForeColor = Color.FromArgb(170, 130, 90)
                };

                var lblDesc = new Label
                {
                    Text = "MÔ TẢ",
                    Dock = DockStyle.Top,
                    Height = 24,
                    Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                    ForeColor = C_TextGray
                };
                _roleDescBox = new TextBox
                {
                    Dock = DockStyle.Top,
                    Height = 64,
                    Multiline = true,
                    Font = F_Label,
                    Text = role.Description ?? "",
                    BackColor = Color.FromArgb(253, 248, 243),
                    BorderStyle = BorderStyle.FixedSingle
                };

                var buttonPanel = new Panel
                {
                    Dock = DockStyle.Top,
                    Height = 52,
                    Padding = new Padding(0, 12, 0, 0)
                };

                var btnSave = MakeSmallButton("💾  Lưu thay đổi", C_Accent);
                btnSave.Width = 140;
                btnSave.Height = 36;
                btnSave.Dock = DockStyle.Left;

                var btnCancel = MakeSmallButton("↩  Hủy", Color.FromArgb(140, 115, 90));
                btnCancel.Width = 80;
                btnCancel.Height = 36;
                btnCancel.Dock = DockStyle.Left;

                buttonPanel.Controls.Add(btnCancel);
                buttonPanel.Controls.Add(new Panel { Dock = DockStyle.Left, Width = 10 });
                buttonPanel.Controls.Add(btnSave);

                // Stats overview card below buttons
                var statsCard = new Panel
                {
                    Dock = DockStyle.Top,
                    Height = 90,
                    BackColor = Color.FromArgb(250, 246, 240)
                };
                statsCard.Paint += (s, e) =>
                {
                    using var pBorder = new Pen(C_Border);
                    e.Graphics.DrawRectangle(pBorder, 0, 0, statsCard.Width - 1, statsCard.Height - 1);
                };
                var statsTitle = new Label
                {
                    Text = "TỔNG QUAN QUYỀN",
                    Dock = DockStyle.Top,
                    Height = 24,
                    Font = new Font("Segoe UI", 8f, FontStyle.Bold),
                    ForeColor = C_TextGray,
                    Padding = new Padding(12, 6, 0, 0)
                };
                var statsValue = new Label
                {
                    Text = $"Đã cấp: {assigned.Count} / {_allPermissions.Count} quyền",
                    Dock = DockStyle.Top,
                    Height = 26,
                    Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                    ForeColor = C_Accent,
                    Padding = new Padding(12, 2, 0, 0)
                };
                var statsDesc = new Label
                {
                    Text = "Tích chọn danh mục quyền bên phải để phân quyền cho vai trò.",
                    Dock = DockStyle.Fill,
                    Font = new Font("Segoe UI", 8f),
                    ForeColor = C_TextGray,
                    Padding = new Padding(12, 0, 12, 0)
                };
                statsCard.Controls.AddRange(new Control[] { statsDesc, statsValue, statsTitle });

                // Add in reverse top-dock order
                leftForm.Controls.Add(statsCard);
                leftForm.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 14 });
                leftForm.Controls.Add(buttonPanel);
                leftForm.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 10 });
                leftForm.Controls.Add(_roleDescBox);
                leftForm.Controls.Add(lblDesc);
                leftForm.Controls.Add(nameNote);
                leftForm.Controls.Add(_roleNameBox);
                leftForm.Controls.Add(lblName);

                bodyTable.Controls.Add(leftForm, 0, 0);

                // ── Right Column: Permissions Matrix ──
                var rightPermPanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = C_BgCard,
                    Padding = new Padding(16, 0, 0, 0)
                };

                var permHeader = new Panel
                {
                    Dock = DockStyle.Top,
                    Height = 36,
                    BackColor = C_BgCard
                };
                var permTitle = new Label
                {
                    Text = "DANH SÁCH QUYỀN HẠN HỆ THỐNG",
                    Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                    ForeColor = C_TextDark,
                    Dock = DockStyle.Left,
                    AutoSize = true,
                    TextAlign = ContentAlignment.MiddleLeft
                };
                permHeader.Controls.Add(permTitle);

                // A single vertical flow area owns scrolling for permissions. Group
                // cards have deterministic heights, so checkboxes never become
                // clipped after the container width changes.
                var permScroll = new FlowLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    AutoScroll = true,
                    BackColor = Color.FromArgb(250, 247, 243),
                    Padding = new Padding(12),
                    FlowDirection = FlowDirection.TopDown,
                    WrapContents = false
                };
                permScroll.Paint += (s, e) =>
                {
                    using var pen = new Pen(C_Border);
                    e.Graphics.DrawRectangle(pen, 0, 0, permScroll.Width - 1, permScroll.Height - 1);
                };

                var checkboxes = new List<CheckBox>();
                var groups = _allPermissions.GroupBy(p => p.GroupName);

                var permissionGroupCards = new List<Panel>();
                foreach (var group in groups)
                {
                    var groupCard = new Panel
                    {
                        BackColor = Color.White,
                        Margin = new Padding(0, 0, 0, 12),
                        MinimumSize = new Size(400, 0)
                    };
                    groupCard.Paint += (s, e) =>
                    {
                        using var pen = new Pen(Color.FromArgb(228, 216, 202));
                        e.Graphics.DrawRectangle(pen, 0, 0, groupCard.Width - 1, groupCard.Height - 1);
                    };

                    string groupIcon = group.Key switch
                    {
                        "Hệ thống" => "🔑",
                        "Người dùng" => "👥",
                        "Phân quyền" => "🛡️",
                        "Chi nhánh" => "🏪",
                        "Ca làm" => "⏰",
                        _ => "📌"
                    };

                    var gHeader = new Panel
                    {
                        Dock = DockStyle.Top,
                        Height = 34,
                        BackColor = Color.FromArgb(248, 242, 234),
                        Padding = new Padding(12, 0, 10, 0)
                    };
                    var gTitle = new Label
                    {
                        Text = $"{groupIcon}  Nhóm quyền: {group.Key}",
                        Font = new Font("Segoe UI", 9.25f, FontStyle.Bold),
                        ForeColor = C_Accent,
                        Dock = DockStyle.Fill,
                        TextAlign = ContentAlignment.MiddleLeft
                    };
                    gHeader.Controls.Add(gTitle);

                    var groupPermissions = group.ToList();
                    var chkContainer = new TableLayoutPanel
                    {
                        Dock = DockStyle.Top,
                        AutoSize = false,
                        ColumnCount = 1,
                        RowCount = groupPermissions.Count,
                        Padding = new Padding(12, 10, 12, 10),
                        BackColor = Color.White,
                        Height = (groupPermissions.Count * 32) + 20
                    };

                    chkContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
                    foreach (var perm in groupPermissions)
                    {
                        var chk = new CheckBox
                        {
                            Text = $"{perm.PermissionName} ({perm.PermissionCode})",
                            AutoSize = false,
                            Height = 28,
                            Checked = assigned.Contains(perm.PermissionID),
                            Tag = perm,
                            Font = new Font("Segoe UI", 9.25f),
                            ForeColor = C_TextDark,
                            BackColor = Color.Transparent,
                            Cursor = Cursors.Hand,
                            Dock = DockStyle.Fill,
                            TextAlign = ContentAlignment.MiddleLeft
                        };
                        chk.CheckedChanged += (_, _) =>
                        {
                            int count = checkboxes.Count(c => c.Checked);
                            statsValue.Text = $"Đã cấp: {count} / {_allPermissions.Count} quyền";
                        };
                        int row = chkContainer.Controls.Count;
                        chkContainer.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
                        chkContainer.Controls.Add(chk, 0, row);
                        checkboxes.Add(chk);
                    }

                    groupCard.Controls.Add(chkContainer);
                    groupCard.Controls.Add(gHeader);
                    groupCard.Height = gHeader.Height + chkContainer.Height;

                    permScroll.Controls.Add(groupCard);
                    permissionGroupCards.Add(groupCard);
                }

                // Keep every card as wide as the visible permission area. If the
                // viewport becomes too narrow, the same (single) scroll area shows
                // a horizontal bar rather than cutting checkbox labels.
                void ResizePermissionCards()
                {
                    int width = Math.Max(400,
                        permScroll.ClientSize.Width - permScroll.Padding.Horizontal - SystemInformation.VerticalScrollBarWidth);
                    foreach (Panel groupCard in permissionGroupCards)
                    {
                        groupCard.Width = width;
                    }
                }

                permScroll.Resize += (_, _) => ResizePermissionCards();
                ResizePermissionCards();

                rightPermPanel.Controls.Add(permScroll);
                rightPermPanel.Controls.Add(permHeader);

                bodyTable.Controls.Add(rightPermPanel, 1, 0);

                rootTable.Controls.Add(bodyTable, 0, 1);
                detailPanel.Controls.Add(rootTable);

                // Button events
                btnSave.Click += async (_, _) =>
                {
                    await SaveRoleAndPermissionsAsync(checkboxes);
                };
                btnCancel.Click += async (_, _) => await LoadRoleDetailAsync();
            }
            catch (Exception ex) { ShowError(DatabaseErrorMessage.From(ex)); }
        }

        private async Task SaveRoleAndPermissionsAsync(List<CheckBox> checkboxes)
        {
            if (_editingRole == null) return;
            try
            {
                using var db = new BakeryDbContext();
                var role = await db.Roles.FindAsync(_editingRole.RoleID);
                if (role == null) return;

                string roleName = _roleNameBox.Text.Trim().ToUpperInvariant();
                if (string.IsNullOrWhiteSpace(roleName))
                {
                    ShowWarn(this, "Tên vai trò không được để trống!");
                    return;
                }

                bool duplicate = await db.Roles.AnyAsync(r =>
                    r.RoleID != role.RoleID && r.RoleName == roleName);
                if (duplicate)
                {
                    ShowWarn(this, $"Vai trò '{roleName}' đã tồn tại!");
                    return;
                }

                role.RoleName = roleName;
                role.Description = _roleDescBox.Text.Trim();

                var old = await db.RolePermissions.Where(rp => rp.RoleID == role.RoleID).ToListAsync();
                db.RolePermissions.RemoveRange(old);

                foreach (var chk in checkboxes.Where(c => c.Checked))
                {
                    if (chk.Tag is Permission perm)
                        db.RolePermissions.Add(new RolePermission { RoleID = role.RoleID, PermissionID = perm.PermissionID });
                }

                await db.SaveChangesAsync();
                MessageBox.Show("Đã lưu phân quyền thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadRolesListAsync();
            }
            catch (Exception ex) { ShowError(DatabaseErrorMessage.From(ex)); }
        }

        private async Task ShowAddRoleDialogAsync()
        {
            using var dlg = MakeDialog("Thêm vai trò", 400, 220);
            int y = 20;
            var txtName = MakeTextField();
            var txtDesc = MakeTextField();
            AddFormField(dlg, "Tên vai trò *", txtName, ref y);
            AddFormField(dlg, "Mô tả", txtDesc, ref y);

            var btnSave = MakeDialogButton("💾  Lưu", C_Accent);
            var btnCancel = MakeDialogButton("↩  Hủy", C_TextGray);
            btnCancel.DialogResult = DialogResult.Cancel;
            btnSave.DialogResult = DialogResult.None;

            btnSave.Click += async (_, _) =>
            {
                if (string.IsNullOrWhiteSpace(txtName.Text)) { ShowWarn(dlg, "Vui lòng nhập tên vai trò!"); return; }
                using var db = new BakeryDbContext();
                string roleName = txtName.Text.Trim().ToUpperInvariant();
                if (await db.Roles.AnyAsync(r => r.RoleName == roleName))
                {
                    ShowWarn(dlg, $"Vai trò '{roleName}' đã tồn tại!");
                    return;
                }

                db.Roles.Add(new Role { RoleName = roleName, Description = txtDesc.Text.Trim() });
                await db.SaveChangesAsync();
                dlg.DialogResult = DialogResult.OK;
            };

            LayoutDialogButtons(dlg, btnSave, btnCancel);
            if (dlg.ShowDialog(this) == DialogResult.OK)
                await LoadRolesListAsync();
        }

        // ─── BRANCHES PAGE ───
        private Panel BuildBranchesPage()
        {
            var p = new Panel { BackColor = C_BgMain };
            var title = MakePageTitle("Quản lý chi nhánh", "Trang chủ  ›  Quản lý chi nhánh");

            var toolbar = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = Color.Transparent };
            _searchBranches = MakeSearchBox("Tìm kiếm theo tên, địa chỉ, số điện thoại...");
            _searchBranches.Width = 275;
            _searchBranches.Left = 0;
            _searchBranches.Top = 10;

            _filterBranchStatus = MakeComboFilter(new[] { "Tất cả trạng thái", "Đang hoạt động", "Ngừng hoạt động" });
            _filterBranchStatus.Left = 283;
            _filterBranchStatus.Top = 12;

            var btnAdd = MakeToolbarButton("+ Thêm chi nhánh", C_Accent);
            var btnEdit = MakeSecondaryButton("Sửa");
            var btnToggle = MakeToolbarButton("Ngừng/kích hoạt", C_Danger);

            btnAdd.Click += async (_, _) => await ShowAddBranchDialogAsync();
            btnEdit.Click += async (_, _) => await ShowEditBranchDialogAsync();
            btnToggle.Click += async (_, _) => await ToggleSelectedBranchAsync();

            _searchBranches.TextChanged += async (_, _) => await LoadBranchesAsync();
            _filterBranchStatus.SelectedIndexChanged += async (_, _) => await LoadBranchesAsync();

            toolbar.Resize += (_, _) =>
            {
                int right = toolbar.Width;
                btnToggle.Left = right - btnToggle.Width - 4;
                btnEdit.Left = btnToggle.Left - btnEdit.Width - 6;
                btnAdd.Left = btnEdit.Left - btnAdd.Width - 6;
                btnToggle.Top = btnEdit.Top = btnAdd.Top = 10;
            };

            toolbar.Controls.AddRange(new Control[] { _searchBranches, _filterBranchStatus, btnAdd, btnEdit, btnToggle });
            _branchesGrid = new ModernDataGrid();
            _branchesGrid.RowEditClicked += async (s, _) => await ShowEditBranchDialogAsync();
            _branchesGrid.RowDeleteClicked += async (s, _) => await ToggleSelectedBranchAsync();
            var gridCard = WrapInCard(_branchesGrid);
            var pagerPanel = BuildPagerPanel();
            var layout = CreatePageLayout(68, 50, 0, 38);
            AddToPageLayout(layout, title, 0);
            AddToPageLayout(layout, toolbar, 1);
            AddToPageLayout(layout, gridCard, 2);
            AddToPageLayout(layout, pagerPanel, 3);
            p.Controls.Add(layout);

            _ = LoadBranchesAsync();
            return p;
        }

        private async Task LoadBranchesAsync()
        {
            int loadVersion = ++_branchesLoadVersion;
            var grid = _branchesGrid;
            try
            {
                bool? active = _filterBranchStatus?.SelectedIndex switch { 1 => true, 2 => false, _ => null };
                string kw = GetSearchKeyword(_searchBranches);

                using var db = new BakeryDbContext();
                var list = await new BranchService(db).GetAllAsync(active, kw.Length > 0 ? kw : null);

                if (loadVersion != _branchesLoadVersion || grid.IsDisposed ||
                    !ReferenceEquals(grid, _branchesGrid))
                    return;

                if (grid.InvokeRequired)
                    grid.Invoke(() => BindBranchesGrid(list));
                else
                    BindBranchesGrid(list);
            }
            catch (Exception ex) { ShowError(DatabaseErrorMessage.From(ex)); }
        }

        private void BindBranchesGrid(List<Branch> list)
        {
            _branchesGrid.DataSource = list.Select((b, i) => new
            {
                STT = i + 1,
                BranchID = b.BranchID,
                Tên_chi_nhánh = b.BranchName,
                Địa_chỉ = b.Address ?? "—",
                Số_điện_thoại = b.Phone ?? "—",
                Trạng_thái = b.IsActive ? "Đang hoạt động" : "Ngừng hoạt động",
                Ngày_tạo = b.CreatedAt.ToString("dd/MM/yyyy"),
                Thao_tác = ""
            }).ToList();

            UpdatePager(_currentPage, list.Count);
        }

        private async Task ShowAddBranchDialogAsync()
        {
            using var dlg = MakeDialog("Thêm chi nhánh", 480, 340);
            int y = 20;
            var txtName = MakeTextField();
            var txtAddr = MakeTextField();
            var txtPhone = MakeTextField();
            var rdoActive = new RadioButton { Text = "Hoạt động", Checked = true, AutoSize = true };
            var rdoInactive = new RadioButton { Text = "Ngừng hoạt động", AutoSize = true };

            AddFormField(dlg, "Tên chi nhánh *", txtName, ref y);
            AddFormField(dlg, "Địa chỉ *", txtAddr, ref y);
            AddFormField(dlg, "Số điện thoại", txtPhone, ref y);

            var statusFlow = new FlowLayoutPanel { Left = 160, Top = y, AutoSize = true };
            statusFlow.Controls.AddRange(new Control[] { rdoActive, rdoInactive });
            dlg.Controls.Add(new Label { Text = "Trạng thái", Left = 20, Top = y + 2, AutoSize = true, Font = F_Label });
            dlg.Controls.Add(statusFlow);

            var btnSave = MakeDialogButton("💾  Lưu", C_Accent);
            var btnCancel = MakeDialogButton("↩  Hủy", C_TextGray);
            btnCancel.DialogResult = DialogResult.Cancel;
            btnSave.DialogResult = DialogResult.None;

            btnSave.Click += async (_, _) =>
            {
                if (string.IsNullOrWhiteSpace(txtName.Text)) { ShowWarn(dlg, "Vui lòng nhập tên chi nhánh!"); return; }
                if (string.IsNullOrWhiteSpace(txtAddr.Text)) { ShowWarn(dlg, "Vui lòng nhập địa chỉ!"); return; }

                // Tạo mã tự động từ tên
                string code = GenerateBranchCode(txtName.Text);
                using var db = new BakeryDbContext();
                var result = await new BranchService(db).CreateAsync(new Branch
                {
                    BranchCode = code,
                    BranchName = txtName.Text.Trim(),
                    Address = txtAddr.Text.Trim(),
                    Phone = txtPhone.Text.Trim(),
                    IsActive = rdoActive.Checked
                });

                if (!result.Success) { ShowWarn(dlg, result.Message); return; }
                dlg.DialogResult = DialogResult.OK;
            };

            LayoutDialogButtons(dlg, btnSave, btnCancel);
            if (dlg.ShowDialog(this) == DialogResult.OK) await LoadBranchesAsync();
        }

        private async Task ShowEditBranchDialogAsync()
        {
            if (!TryGetSelectedId(_branchesGrid, "BranchID", out int id)) return;

            using var db0 = new BakeryDbContext();
            var branch = await new BranchService(db0).GetByIdAsync(id);
            if (branch == null) return;

            using var dlg = MakeDialog("Sửa chi nhánh", 480, 340);
            int y = 20;
            var txtName = MakeTextField(); txtName.Text = branch.BranchName;
            var txtAddr = MakeTextField(); txtAddr.Text = branch.Address ?? "";
            var txtPhone = MakeTextField(); txtPhone.Text = branch.Phone ?? "";
            var rdoActive = new RadioButton { Text = "Hoạt động", Checked = branch.IsActive, AutoSize = true };
            var rdoInactive = new RadioButton { Text = "Ngừng hoạt động", Checked = !branch.IsActive, AutoSize = true };

            AddFormField(dlg, "Tên chi nhánh *", txtName, ref y);
            AddFormField(dlg, "Địa chỉ *", txtAddr, ref y);
            AddFormField(dlg, "Số điện thoại", txtPhone, ref y);
            var statusFlow = new FlowLayoutPanel { Left = 160, Top = y, AutoSize = true };
            statusFlow.Controls.AddRange(new Control[] { rdoActive, rdoInactive });
            dlg.Controls.Add(new Label { Text = "Trạng thái", Left = 20, Top = y + 2, AutoSize = true, Font = F_Label });
            dlg.Controls.Add(statusFlow);

            var btnSave = MakeDialogButton("💾  Lưu", C_Accent);
            var btnCancel = MakeDialogButton("↩  Hủy", C_TextGray);
            btnCancel.DialogResult = DialogResult.Cancel;
            btnSave.DialogResult = DialogResult.None;

            btnSave.Click += async (_, _) =>
            {
                if (string.IsNullOrWhiteSpace(txtName.Text)) { ShowWarn(dlg, "Vui lòng nhập tên chi nhánh!"); return; }
                using var db = new BakeryDbContext();
                var result = await new BranchService(db).UpdateAsync(new Branch
                {
                    BranchID = id,
                    BranchCode = branch.BranchCode,
                    BranchName = txtName.Text.Trim(),
                    Address = txtAddr.Text.Trim(),
                    Phone = txtPhone.Text.Trim(),
                    IsActive = rdoActive.Checked
                });
                if (!result.Success) { ShowWarn(dlg, result.Message); return; }
                dlg.DialogResult = DialogResult.OK;
            };

            LayoutDialogButtons(dlg, btnSave, btnCancel);
            if (dlg.ShowDialog(this) == DialogResult.OK) await LoadBranchesAsync();
        }

        private async Task ToggleSelectedBranchAsync()
        {
            if (!TryGetSelectedId(_branchesGrid, "BranchID", out int id)) return;
            string status = GetGridCellValue(_branchesGrid, "Trạng_thái");
            string action = status == "Đang hoạt động" ? "ngừng hoạt động" : "kích hoạt";
            if (MessageBox.Show($"Xác nhận {action} chi nhánh này?", "Xác nhận",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            using var db = new BakeryDbContext();
            var result = await new BranchService(db).ToggleStatusAsync(id);
            ShowResult(result.Success, result.Message);
            if (result.Success) await LoadBranchesAsync();
        }

        // ─── SHIFTS PAGE ───
        private Panel BuildShiftsPage()
        {
            var p = new Panel { BackColor = C_BgMain };
            var title = MakePageTitle("Quản lý ca làm", "Trang chủ  ›  Quản lý ca làm");

            var toolbar = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = Color.Transparent };
            _searchShifts = MakeSearchBox("Tìm kiếm theo tên ca...");
            _searchShifts.Width = 260;
            _searchShifts.Left = 0;
            _searchShifts.Top = 10;

            _filterShiftStatus = MakeComboFilter(new[] { "Tất cả trạng thái", "Đang hoạt động", "Ngừng sử dụng" });
            _filterShiftStatus.Left = 272;
            _filterShiftStatus.Top = 12;

            var btnAdd = MakeToolbarButton("+ Thêm ca làm", C_Accent);
            var btnEdit = MakeSecondaryButton("Sửa");
            var btnToggle = MakeToolbarButton("Ngừng sử dụng", C_Danger);

            btnAdd.Click += async (_, _) => await ShowAddShiftDialogAsync();
            btnEdit.Click += async (_, _) => await ShowEditShiftDialogAsync();
            btnToggle.Click += async (_, _) => await ToggleSelectedShiftAsync();

            _searchShifts.TextChanged += async (_, _) => await LoadShiftsAsync();
            _filterShiftStatus.SelectedIndexChanged += async (_, _) => await LoadShiftsAsync();

            toolbar.Resize += (_, _) =>
            {
                int right = toolbar.Width;
                btnToggle.Left = right - btnToggle.Width - 4;
                btnEdit.Left = btnToggle.Left - btnEdit.Width - 6;
                btnAdd.Left = btnEdit.Left - btnAdd.Width - 6;
                btnToggle.Top = btnEdit.Top = btnAdd.Top = 10;
            };

            toolbar.Controls.AddRange(new Control[] { _searchShifts, _filterShiftStatus, btnAdd, btnEdit, btnToggle });
            _shiftsGrid = new ModernDataGrid();
            _shiftsGrid.RowEditClicked += async (s, _) => await ShowEditShiftDialogAsync();
            _shiftsGrid.RowDeleteClicked += async (s, _) => await ToggleSelectedShiftAsync();
            var gridCard = WrapInCard(_shiftsGrid);
            var pagerPanel = BuildPagerPanel();
            var layout = CreatePageLayout(68, 50, 0, 38);
            AddToPageLayout(layout, title, 0);
            AddToPageLayout(layout, toolbar, 1);
            AddToPageLayout(layout, gridCard, 2);
            AddToPageLayout(layout, pagerPanel, 3);
            p.Controls.Add(layout);

            _ = LoadShiftsAsync();
            return p;
        }

        private async Task LoadShiftsAsync()
        {
            int loadVersion = ++_shiftsLoadVersion;
            var grid = _shiftsGrid;
            try
            {
                bool? active = _filterShiftStatus?.SelectedIndex switch { 1 => true, 2 => false, _ => null };
                using var db = new BakeryDbContext();
                var list = await new ShiftService(db).GetAllAsync(active);

                string kw = GetSearchKeyword(_searchShifts);
                if (!string.IsNullOrWhiteSpace(kw))
                    list = list.Where(s => s.ShiftName.Contains(kw, StringComparison.OrdinalIgnoreCase)).ToList();

                if (loadVersion != _shiftsLoadVersion || grid.IsDisposed ||
                    !ReferenceEquals(grid, _shiftsGrid))
                    return;

                if (grid.InvokeRequired)
                    grid.Invoke(() => BindShiftsGrid(list));
                else
                    BindShiftsGrid(list);
            }
            catch (Exception ex) { ShowError(DatabaseErrorMessage.From(ex)); }
        }

        private void BindShiftsGrid(List<Shift> list)
        {
            _shiftsGrid.DataSource = list.Select((s, i) => new
            {
                STT = i + 1,
                ShiftID = s.ShiftID,
                Tên_ca = s.ShiftName,
                Giờ_bắt_đầu = s.StartTime.ToString(@"hh\:mm"),
                Giờ_kết_thúc = s.EndTime.ToString(@"hh\:mm"),
                Thời_lượng = ComputeDuration(s.StartTime, s.EndTime),
                Trạng_thái = s.IsActive ? "Đang hoạt động" : "Ngừng sử dụng",
                Ngày_tạo = s.CreatedAt.ToString("dd/MM/yyyy"),
                Thao_tác = ""
            }).ToList();

            UpdatePager(_currentPage, list.Count);
        }

        private async Task ShowAddShiftDialogAsync()
        {
            using var dlg = MakeDialog("Thêm ca làm", 460, 340);
            int y = 20;
            var txtName = MakeTextField();
            var tpStart = new DateTimePicker { Format = DateTimePickerFormat.Time, ShowUpDown = true, Left = 160, Width = 140, Height = 30 };
            var tpEnd = new DateTimePicker { Format = DateTimePickerFormat.Time, ShowUpDown = true, Left = 160, Width = 140, Height = 30 };
            tpStart.Value = DateTime.Today.AddHours(6);
            tpEnd.Value = DateTime.Today.AddHours(14);
            var rdoActive = new RadioButton { Text = "Hoạt động", Checked = true, AutoSize = true };
            var rdoInactive = new RadioButton { Text = "Ngừng sử dụng", AutoSize = true };

            AddFormField(dlg, "Tên ca *", txtName, ref y);
            // Giờ bắt đầu
            dlg.Controls.Add(new Label { Text = "Giờ bắt đầu *", Left = 20, Top = y + 4, AutoSize = true, Font = F_Label, ForeColor = C_TextGray });
            tpStart.Top = y; dlg.Controls.Add(tpStart); y += 44;
            // Giờ kết thúc
            dlg.Controls.Add(new Label { Text = "Giờ kết thúc *", Left = 20, Top = y + 4, AutoSize = true, Font = F_Label, ForeColor = C_TextGray });
            tpEnd.Top = y; dlg.Controls.Add(tpEnd); y += 44;

            var note = new Label
            {
                Text = "Lưu ý: Có thể thiết lập ca qua đêm (giờ kết thúc < giờ bắt đầu)",
                Left = 20,
                Top = y,
                AutoSize = true,
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = C_TextGray
            };
            dlg.Controls.Add(note); y += 20;

            var statusFlow = new FlowLayoutPanel { Left = 160, Top = y, AutoSize = true };
            statusFlow.Controls.AddRange(new Control[] { rdoActive, rdoInactive });
            dlg.Controls.Add(new Label { Text = "Trạng thái", Left = 20, Top = y + 2, AutoSize = true, Font = F_Label });
            dlg.Controls.Add(statusFlow);

            var btnSave = MakeDialogButton("💾  Lưu", C_Accent);
            var btnCancel = MakeDialogButton("↩  Hủy", C_TextGray);
            btnCancel.DialogResult = DialogResult.Cancel;
            btnSave.DialogResult = DialogResult.None;

            btnSave.Click += async (_, _) =>
            {
                if (string.IsNullOrWhiteSpace(txtName.Text)) { ShowWarn(dlg, "Vui lòng nhập tên ca!"); return; }
                string code = "CA" + DateTime.Now.Ticks.ToString().Substring(10);
                using var db = new BakeryDbContext();
                var result = await new ShiftService(db).CreateAsync(new Shift
                {
                    ShiftCode = code,
                    ShiftName = txtName.Text.Trim(),
                    StartTime = tpStart.Value.TimeOfDay,
                    EndTime = tpEnd.Value.TimeOfDay,
                    IsActive = rdoActive.Checked
                });
                if (!result.Success) { ShowWarn(dlg, result.Message); return; }
                dlg.DialogResult = DialogResult.OK;
            };

            LayoutDialogButtons(dlg, btnSave, btnCancel);
            if (dlg.ShowDialog(this) == DialogResult.OK) await LoadShiftsAsync();
        }

        private async Task ShowEditShiftDialogAsync()
        {
            if (!TryGetSelectedId(_shiftsGrid, "ShiftID", out int id)) return;

            using var db0 = new BakeryDbContext();
            var shift = await new ShiftService(db0).GetByIdAsync(id);
            if (shift == null) return;

            using var dlg = MakeDialog("Sửa ca làm", 460, 340);
            int y = 20;
            var txtName = MakeTextField(); txtName.Text = shift.ShiftName;
            var tpStart = new DateTimePicker { Format = DateTimePickerFormat.Time, ShowUpDown = true, Left = 160, Width = 140, Height = 30 };
            var tpEnd = new DateTimePicker { Format = DateTimePickerFormat.Time, ShowUpDown = true, Left = 160, Width = 140, Height = 30 };
            tpStart.Value = DateTime.Today + shift.StartTime;
            tpEnd.Value = DateTime.Today + shift.EndTime;
            var rdoActive = new RadioButton { Text = "Hoạt động", Checked = shift.IsActive, AutoSize = true };
            var rdoInactive = new RadioButton { Text = "Ngừng sử dụng", Checked = !shift.IsActive, AutoSize = true };

            AddFormField(dlg, "Tên ca *", txtName, ref y);
            dlg.Controls.Add(new Label { Text = "Giờ bắt đầu *", Left = 20, Top = y + 4, AutoSize = true, Font = F_Label, ForeColor = C_TextGray });
            tpStart.Top = y; dlg.Controls.Add(tpStart); y += 44;
            dlg.Controls.Add(new Label { Text = "Giờ kết thúc *", Left = 20, Top = y + 4, AutoSize = true, Font = F_Label, ForeColor = C_TextGray });
            tpEnd.Top = y; dlg.Controls.Add(tpEnd); y += 44;

            var statusFlow = new FlowLayoutPanel { Left = 160, Top = y, AutoSize = true };
            statusFlow.Controls.AddRange(new Control[] { rdoActive, rdoInactive });
            dlg.Controls.Add(new Label { Text = "Trạng thái", Left = 20, Top = y + 2, AutoSize = true, Font = F_Label });
            dlg.Controls.Add(statusFlow);

            var btnSave = MakeDialogButton("💾  Lưu", C_Accent);
            var btnCancel = MakeDialogButton("↩  Hủy", C_TextGray);
            btnCancel.DialogResult = DialogResult.Cancel;
            btnSave.DialogResult = DialogResult.None;

            btnSave.Click += async (_, _) =>
            {
                if (string.IsNullOrWhiteSpace(txtName.Text)) { ShowWarn(dlg, "Vui lòng nhập tên ca!"); return; }
                using var db = new BakeryDbContext();
                var result = await new ShiftService(db).UpdateAsync(new Shift
                {
                    ShiftID = id,
                    ShiftCode = shift.ShiftCode,
                    ShiftName = txtName.Text.Trim(),
                    StartTime = tpStart.Value.TimeOfDay,
                    EndTime = tpEnd.Value.TimeOfDay,
                    IsActive = rdoActive.Checked
                });
                if (!result.Success) { ShowWarn(dlg, result.Message); return; }
                dlg.DialogResult = DialogResult.OK;
            };

            LayoutDialogButtons(dlg, btnSave, btnCancel);
            if (dlg.ShowDialog(this) == DialogResult.OK) await LoadShiftsAsync();
        }

        private async Task ToggleSelectedShiftAsync()
        {
            if (!TryGetSelectedId(_shiftsGrid, "ShiftID", out int id)) return;
            string status = GetGridCellValue(_shiftsGrid, "Trạng_thái");
            string action = status == "Đang hoạt động" ? "ngừng sử dụng" : "kích hoạt";
            if (MessageBox.Show($"Xác nhận {action} ca làm này?", "Xác nhận",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            using var db = new BakeryDbContext();
            var result = await new ShiftService(db).ToggleStatusAsync(id);
            ShowResult(result.Success, result.Message);
            if (result.Success) await LoadShiftsAsync();
        }

        // ═══════════════════ LOAD ALL ═══════════════════
        private async Task LoadAllAsync()
        {
            // Home page stats are loaded per-page; pre-warm DB
            try
            {
                using var db = new BakeryDbContext();
                // EnsureCreated does not update an existing database. Apply pending
                // migrations so newly-added columns are available before pages query them.
                await db.Database.MigrateAsync();
            }
            catch { /* ignore */ }
        }

        // ═══════════════════ UI HELPERS ═══════════════════

        /// <summary>
        /// Places page sections in fixed rows instead of relying on the z-order-sensitive
        /// behavior of several DockStyle controls. This keeps titles, filters and grid
        /// headers visible at every window size.
        /// </summary>
        private static TableLayoutPanel CreatePageLayout(params int[] rowHeights)
        {
            var layout = new TableLayoutPanel
            {
                AutoScroll = true,
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = rowHeights.Length,
                BackColor = Color.Transparent,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            foreach (int height in rowHeights)
            {
                layout.RowStyles.Add(height > 0
                    ? new RowStyle(SizeType.Absolute, height)
                    : new RowStyle(SizeType.Percent, 100));
            }

            return layout;
        }

        private static void AddToPageLayout(TableLayoutPanel layout, Control control, int row)
        {
            control.Dock = DockStyle.Fill;
            control.Margin = Padding.Empty;
            layout.Controls.Add(control, 0, row);
        }

        private static Panel MakePageTitle(string title, string breadcrumb)
        {
            var panel = new Panel { Dock = DockStyle.Top, Height = 72, BackColor = Color.Transparent };
            var lbl = new Label
            {
                Text = title,
                Font = F_Title,
                ForeColor = C_TextDark,
                Left = 0,
                Top = 4,
                AutoSize = true
            };
            var bc = new Label
            {
                Text = breadcrumb,
                Font = new Font("Segoe UI", 9f),
                ForeColor = C_TextGray,
                Left = 0,
                Top = 46,
                AutoSize = true
            };
            panel.Controls.AddRange(new Control[] { lbl, bc });
            return panel;
        }

        private static Panel BuildPagerPanel()
        {
            var pagerPanel = new Panel { Dock = DockStyle.Bottom, Height = 38, BackColor = Color.Transparent };
            pagerPanel.Paint += (s, e) =>
            {
                if (s is not Control ctrl) return;
                e.Graphics.DrawLine(new Pen(Color.FromArgb(225, 210, 190)), 0, 0, ctrl.Width, 0);
            };
            var pagerLbl = new Label
            {
                Name = "pagerLbl",
                Text = "Hiển thị 0 bản ghi",
                Dock = DockStyle.Left,
                Width = 300,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(120, 100, 80)
            };
            pagerPanel.Controls.Add(pagerLbl);

            // The data is currently loaded on one page, but showing the pager affordance
            // makes the table footer consistent with the management screens in the design.
            var next = MakePagerButton("›");
            var page = MakePagerButton("1");
            var previous = MakePagerButton("‹");
            next.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            page.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            previous.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pagerPanel.Resize += (_, _) =>
            {
                next.Left = pagerPanel.ClientSize.Width - next.Width;
                page.Left = next.Left - page.Width - 5;
                previous.Left = page.Left - previous.Width - 5;
            };
            pagerPanel.Controls.AddRange(new Control[] { previous, page, next });
            return pagerPanel;
        }

        private static Panel WrapInCard(Control content)
        {
            var card = new Panel { Dock = DockStyle.Fill, BackColor = C_BgCard, Padding = new Padding(0) };
            card.Paint += PaintCard;
            content.Dock = DockStyle.Fill;
            card.Controls.Add(content);
            return card;
        }

        private static void PaintCard(object? sender, PaintEventArgs e)
        {
            if (sender is not Control ctrl) return;
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using var gp = BuildRoundedRect(new Rectangle(0, 0, ctrl.Width - 1, ctrl.Height - 1), 10);
            g.FillPath(new SolidBrush(C_BgCard), gp);
            g.DrawPath(new Pen(C_Border), gp);
        }

        private static GraphicsPath BuildRoundedRect(Rectangle rect, int radius)
        {
            var gp = new GraphicsPath();
            gp.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            gp.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
            gp.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
            gp.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
            gp.CloseFigure();
            return gp;
        }

        private static TextBox MakeSearchBox(string placeholder)
        {
            var tb = new TextBox
            {
                Height = 32,
                Font = new Font("Segoe UI", 9.5f),
                ForeColor = Color.Gray,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(8, 5, 8, 5),
                Text = placeholder,
                Tag = placeholder
            };
            tb.GotFocus += (s, e) => { if (tb.Text == placeholder) { tb.Text = ""; tb.ForeColor = C_TextDark; } };
            tb.LostFocus += (s, e) => { if (string.IsNullOrWhiteSpace(tb.Text)) { tb.Text = placeholder; tb.ForeColor = Color.Gray; } };
            return tb;
        }

        private static string GetSearchKeyword(TextBox? searchBox)
        {
            if (searchBox == null) return string.Empty;

            string value = searchBox.Text.Trim();
            return string.Equals(value, searchBox.Tag as string, StringComparison.Ordinal)
                ? string.Empty
                : value;
        }

        private static ComboBox MakeComboFilter(string[] items)
        {
            var cb = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 174,
                Height = 32,
                Font = F_Label,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White
            };
            cb.Items.AddRange(items);
            cb.SelectedIndex = 0;
            return cb;
        }

        private static ModernButton MakeToolbarButton(string text, Color bgColor)
        {
            int contentWidth = TextRenderer.MeasureText(text, F_BtnSm).Width + 32;
            var btn = new ModernButton
            {
                Text = text,
                Width = Math.Clamp(contentWidth, 100, 160),
                Height = 34,
                BorderRadius = 7,
                NormalColor = bgColor,
                HoverColor = ControlPaint.Light(bgColor, 0.08f),
                PressedColor = ControlPaint.Dark(bgColor, 0.10f),
                ForeColor = Color.White,
                Font = F_BtnSm,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.MouseOverBackColor = ControlPaint.Light(bgColor, 0.2f);
            return btn;
        }

        private static ModernButton MakeSecondaryButton(string text)
        {
            var btn = new ModernButton
            {
                Text = text,
                Width = Math.Max(70, TextRenderer.MeasureText(text, F_BtnSm).Width + 26),
                Height = 34,
                BorderRadius = 7,
                NormalColor = Color.White,
                HoverColor = Color.FromArgb(255, 247, 238),
                PressedColor = Color.FromArgb(247, 230, 214),
                BorderColor = C_Border,
                BorderWidth = 1,
                ForeColor = C_TextDark,
                Font = F_BtnSm,
                Cursor = Cursors.Hand
            };
            return btn;
        }

        private static ModernButton MakeSmallButton(string text, Color bgColor)
        {
            int minWidth = TextRenderer.MeasureText(text, F_BtnSm).Width + 24;
            var btn = new ModernButton
            {
                Text = text,
                AutoSize = false,
                Height = 32,
                Width = Math.Max(minWidth, 90),
                BorderRadius = 7,
                NormalColor = bgColor,
                HoverColor = ControlPaint.Light(bgColor, 0.08f),
                PressedColor = ControlPaint.Dark(bgColor, 0.10f),
                ForeColor = Color.White,
                Font = F_BtnSm,
                Cursor = Cursors.Hand
            };
            return btn;
        }

        private static Form MakeDialog(string title, int width, int height)
        {
            return new Form
            {
                Text = title,
                Size = new Size(width, height + 60),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.White,
                Font = F_Label,
                ShowInTaskbar = false
            };
        }

        private static TextBox MakeTextField(bool password = false)
        {
            return new TextBox
            {
                Width = 270,
                Height = 32,
                Font = F_Label,
                UseSystemPasswordChar = password,
                BackColor = Color.FromArgb(255, 252, 248),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(7, 4, 7, 4)
            };
        }

        private static ComboBox MakeCombo<T>(List<T> items, string displayMember, string valueMember, bool nullable = false)
        {
            var cb = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 270,
                Height = 32,
                Font = F_Label,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(255, 252, 248),
                DataSource = items,
                DisplayMember = displayMember,
                ValueMember = valueMember
            };
            return cb;
        }

        private static void AddFormField(Form dlg, string label, Control ctrl, ref int y)
        {
            bool multiLine = label.Contains('\n');
            int lblHeight = multiLine ? 36 : 22;
            var lbl = new Label
            {
                Text = label,
                Left = 20,
                Top = y + 2,
                Width = 132,
                Height = lblHeight,
                Font = F_Label,
                ForeColor = C_TextGray
            };
            ctrl.Left = 160;
            ctrl.Top = y;
            dlg.Controls.Add(lbl);
            dlg.Controls.Add(ctrl);
            y += multiLine ? 52 : 48;
        }

        private static ModernButton MakeDialogButton(string text, Color bg)
        {
            bool secondary = bg == C_TextGray;
            int minWidth = TextRenderer.MeasureText(text, F_BtnSm).Width + 28;
            var btn = new ModernButton
            {
                Text = text,
                Width = Math.Max(110, minWidth),
                Height = 36,
                BorderRadius = 7,
                NormalColor = secondary ? Color.White : bg,
                HoverColor = secondary ? Color.FromArgb(255, 247, 238) : ControlPaint.Light(bg, 0.08f),
                PressedColor = secondary ? Color.FromArgb(247, 230, 214) : ControlPaint.Dark(bg, 0.10f),
                BorderColor = secondary ? C_Border : Color.Transparent,
                BorderWidth = secondary ? 1 : 0,
                ForeColor = secondary ? C_TextDark : Color.White,
                Font = F_BtnSm,
                Cursor = Cursors.Hand
            };
            return btn;
        }

        private static ModernButton MakePagerButton(string text)
        {
            return new ModernButton
            {
                Text = text,
                Width = 28,
                Height = 28,
                Top = 5,
                BorderRadius = 6,
                NormalColor = text == "1" ? C_Accent : Color.White,
                HoverColor = text == "1" ? C_Accent : Color.FromArgb(255, 247, 238),
                PressedColor = text == "1" ? C_Accent : Color.FromArgb(247, 230, 214),
                BorderColor = text == "1" ? C_Accent : C_Border,
                BorderWidth = 1,
                ForeColor = text == "1" ? Color.White : C_TextDark,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                Cursor = Cursors.Default,
                TabStop = false
            };
        }

        private static void LayoutDialogButtons(Form dlg, Button btnPrimary, Button btnSecondary)
        {
            int gap = 8;
            int totalW = btnPrimary.Width + gap + btnSecondary.Width;
            int startX = (dlg.ClientSize.Width - totalW) / 2;
            btnPrimary.Left = startX;
            btnSecondary.Left = startX + btnPrimary.Width + gap;
            btnPrimary.Top = btnSecondary.Top = dlg.ClientSize.Height - btnPrimary.Height - 14;
            dlg.Controls.AddRange(new Control[] { btnPrimary, btnSecondary });
            dlg.AcceptButton = btnPrimary;
        }

        private static void StyleStatusColumn(DataGridView grid, string colName, string activeValue)
        {
            grid.CellFormatting += (s, e) =>
            {
                if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
                if (grid.Columns[e.ColumnIndex]?.Name != colName) return;
                if (e.Value?.ToString() == activeValue)
                    e.CellStyle.ForeColor = C_Success;
                else
                    e.CellStyle.ForeColor = C_Danger;
                e.CellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            };
        }

        private static void UpdatePager(Panel page, int count)
        {
            if (page == null) return;
            foreach (Control c in FindControls(page))
            {
                if (c is Panel pagerPanel && pagerPanel.Controls["pagerLbl"] is Label lbl)
                {
                    lbl.Text = count == 0
                        ? "Hiển thị 0-0 trong 0 bản ghi"
                        : $"Hiển thị 1-{count} trong {count} bản ghi";
                    return;
                }
            }
        }

        private static IEnumerable<Control> FindControls(Control root)
        {
            foreach (Control child in root.Controls)
            {
                yield return child;
                foreach (Control nested in FindControls(child))
                    yield return nested;
            }
        }

        private static bool TryGetSelectedId(DataGridView grid, string colName, out int id)
        {
            id = 0;
            if (grid.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn một dòng dữ liệu.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            if (grid.CurrentRow.Cells[colName]?.Value is int val) { id = val; return true; }
            MessageBox.Show("Vui lòng chọn một dòng dữ liệu.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return false;
        }

        private static string GetGridCellValue(DataGridView grid, string colName)
        {
            return grid.CurrentRow?.Cells[colName]?.Value?.ToString() ?? "";
        }

        private static string GenerateBranchCode(string name)
        {
            // CN + 2 first chars + sequential number
            string prefix = "CN";
            string upper = System.Text.RegularExpressions.Regex.Replace(name.ToUpper(), @"[^A-Z0-9]", "");
            return prefix + (upper.Length >= 2 ? upper.Substring(0, 2) : upper) + DateTime.Now.Second;
        }

        private static string ComputeDuration(TimeSpan start, TimeSpan end)
        {
            double hours = end >= start
                ? (end - start).TotalHours
                : (TimeSpan.FromHours(24) - start + end).TotalHours;
            return hours == Math.Floor(hours) ? $"{(int)hours} giờ" : $"{hours:F1} giờ";
        }

        private static void ShowError(string msg) =>
            MessageBox.Show(msg, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);

        private static void ShowWarn(Form parent, string msg) =>
            MessageBox.Show(msg, "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        private static void ShowResult(bool success, string message) =>
            MessageBox.Show(message, success ? "Thành công" : "Không thể thực hiện",
                MessageBoxButtons.OK, success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

        private void _navUsers_Click(object sender, EventArgs e)
        {

        }
    }
}
