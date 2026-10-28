using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using BaiTap_WinForms.Data;
using BaiTap_WinForms.Models;

namespace BaiTap_WinForms.Forms
{
    public partial class FormMain : Form
    {
        private readonly CultureInfo _vnCulture = new CultureInfo("vi-VN");

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            // Hiển thị thông tin đăng nhập
            lblUserInfo.Text = $"Xin chào: {UserSession.FullName} ({UserSession.RoleName})";

            // Phân quyền: nếu là STAFF thì ẩn các tab quản lý nâng cao (hoặc chỉ cho phép POS)
            if (!UserSession.IsAdmin)
            {
                btnNavEmployees.Visible = false;
                btnNavReports.Visible = false;
            }

            // Tải dữ liệu ban đầu
            LoadPOSCategories();
            LoadPOSProducts();
            LoadProductsTab();
            LoadIngredientsTab();
            LoadReportsTab();
            LoadEmployeesTab();

            // Mặc định chọn POS
            SwitchTab(0, btnNavPOS);
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                UserSession.Clear();
                this.Hide();
                var loginForm = new FormLogin();
                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    this.FormMain_Load(sender, e);
                    this.Show();
                }
                else
                {
                    this.Close();
                }
            }
        }

        private void btnNav_Click(object sender, EventArgs e)
        {
            if (sender == btnNavPOS) SwitchTab(0, btnNavPOS);
            else if (sender == btnNavProducts) SwitchTab(1, btnNavProducts);
            else if (sender == btnNavIngredients) SwitchTab(2, btnNavIngredients);
            else if (sender == btnNavReports) SwitchTab(3, btnNavReports);
            else if (sender == btnNavEmployees) SwitchTab(4, btnNavEmployees);
        }

        private void SwitchTab(int tabIndex, Button activeBtn)
        {
            tabMain.SelectedIndex = tabIndex;
            var buttons = new[] { btnNavPOS, btnNavProducts, btnNavIngredients, btnNavReports, btnNavEmployees };
            foreach (var btn in buttons)
            {
                if (btn == activeBtn)
                {
                    btn.BackColor = Color.FromArgb(230, 126, 34);
                    btn.ForeColor = Color.White;
                    btn.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
                }
                else
                {
                    btn.BackColor = Color.FromArgb(44, 62, 80);
                    btn.ForeColor = Color.Gainsboro;
                    btn.Font = new Font("Segoe UI", 11F, FontStyle.Regular);
                }
            }
        }

        #region ======================== 1. MODULE BÁN HÀNG (POS) ========================

        private ComboBox cboPOSCategory = null!;
        private TextBox txtPOSSearch = null!;
        private DataGridView dgvPOSMenu = null!;
        private DataGridView dgvPOSCart = null!;
        private Label lblPOSTotal = null!;
        private NumericUpDown numPOSDiscount = null!;
        private Label lblPOSFinal = null!;
        private ComboBox cboPOSPaymentMethod = null!;
        private TextBox txtPOSCustomerMoney = null!;
        private Label lblPOSChange = null!;
        private TextBox txtPOSNote = null!;
        private Button btnPOSCheckout = null!;
        private Button btnPOSClear = null!;

        private readonly DataTable _cartTable = new DataTable();

        private void InitializePOSTab()
        {
            tabPOS.BackColor = Color.FromArgb(245, 246, 250);

            // Cấu trúc cartTable
            _cartTable.Columns.Add("ProductID", typeof(int));
            _cartTable.Columns.Add("ProductCode", typeof(string));
            _cartTable.Columns.Add("ProductName", typeof(string));
            _cartTable.Columns.Add("UnitPrice", typeof(decimal));
            _cartTable.Columns.Add("Quantity", typeof(decimal));
            _cartTable.Columns.Add("TotalAmount", typeof(decimal));

            // Chia làm 2 cột: Trái (Thực đơn), Phải (Giỏ hàng & Thanh toán)
            var splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                SplitterDistance = 640,
                IsSplitterFixed = false
            };
            tabPOS.Controls.Add(splitContainer);

            // ================= CỘT TRÁI: THỰC ĐƠN =================
            var panelMenu = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            splitContainer.Panel1.Controls.Add(panelMenu);

            var groupMenu = new GroupBox
            {
                Text = "📋 THỰC ĐƠN MÓN",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80)
            };
            panelMenu.Controls.Add(groupMenu);

            // Thanh tìm kiếm + lọc danh mục
            var pnlMenuFilter = new Panel { Dock = DockStyle.Top, Height = 45, Padding = new Padding(5) };
            groupMenu.Controls.Add(pnlMenuFilter);

            var lblCat = new Label { Text = "Danh mục:", Location = new Point(5, 12), AutoSize = true, Font = new Font("Segoe UI", 9.5F) };
            cboPOSCategory = new ComboBox { Location = new Point(80, 8), Size = new Size(180, 28), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9.5F) };
            cboPOSCategory.SelectedIndexChanged += (s, e) => LoadPOSProducts();

            var lblSearch = new Label { Text = "Tìm món:", Location = new Point(280, 12), AutoSize = true, Font = new Font("Segoe UI", 9.5F) };
            txtPOSSearch = new TextBox { Location = new Point(350, 8), Size = new Size(220, 28), Font = new Font("Segoe UI", 9.5F) };
            txtPOSSearch.TextChanged += (s, e) => LoadPOSProducts();

            pnlMenuFilter.Controls.AddRange(new Control[] { lblCat, cboPOSCategory, lblSearch, txtPOSSearch });

            // DataGridView Thực đơn
            dgvPOSMenu = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                Font = new Font("Segoe UI", 9.5F)
            };
            dgvPOSMenu.CellDoubleClick += (s, e) => AddSelectedProductToCart();
            groupMenu.Controls.Add(dgvPOSMenu);
            dgvPOSMenu.BringToFront();

            var btnAddCart = new Button
            {
                Text = "➕ Thêm Vào Giỏ Hàng (Hoặc Double-click)",
                Dock = DockStyle.Bottom,
                Height = 40,
                BackColor = Color.FromArgb(39, 174, 96),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnAddCart.FlatAppearance.BorderSize = 0;
            btnAddCart.Click += (s, e) => AddSelectedProductToCart();
            groupMenu.Controls.Add(btnAddCart);

            // ================= CỘT PHẢI: GIỎ HÀNG & THANH TOÁN =================
            var panelCart = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            splitContainer.Panel2.Controls.Add(panelCart);

            var groupCart = new GroupBox
            {
                Text = "🛒 ĐƠN HÀNG HIỆN TẠI",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80)
            };
            panelCart.Controls.Add(groupCart);

            // DataGridView Giỏ hàng – dùng cột thủ công để tránh bị chồng
            dgvPOSCart = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
                AutoGenerateColumns = false,
                BackgroundColor = Color.White,
                Font = new Font("Segoe UI", 9.5F),
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
            };

            // Định nghĩa cột thủ công
            dgvPOSCart.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ProductID",   Visible = false, HeaderText = "ID" });
            dgvPOSCart.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ProductCode", Visible = false, HeaderText = "Mã" });
            dgvPOSCart.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ProductName", HeaderText = "Tên Món",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                MinimumWidth = 120
            });
            dgvPOSCart.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "UnitPrice", HeaderText = "Đơn Giá", Width = 90,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N0", Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            dgvPOSCart.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Quantity", HeaderText = "SL", Width = 48,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
            dgvPOSCart.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "TotalAmount", HeaderText = "Thành Tiền", Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N0", Alignment = DataGridViewContentAlignment.MiddleRight },
                ReadOnly = true
            });

            dgvPOSCart.DataSource = _cartTable;
            dgvPOSCart.CellValueChanged += (s, e) => RecalculateCart();
            dgvPOSCart.RowsRemoved += (s, e) => RecalculateCart();

            // Panel thanh toán phía dưới giỏ hàng
            var pnlCheckout = new Panel { Dock = DockStyle.Bottom, Height = 280, Padding = new Padding(10), BackColor = Color.FromArgb(240, 243, 244) };

            int y = 10;
            var lblT1 = new Label { Text = "Tổng tiền hàng:", Location = new Point(15, y), AutoSize = true, Font = new Font("Segoe UI", 10F) };
            lblPOSTotal = new Label { Text = "0 đ", Location = new Point(160, y), AutoSize = true, Font = new Font("Segoe UI", 11F, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };

            y += 30;
            var lblT2 = new Label { Text = "Giảm giá (VNĐ):", Location = new Point(15, y), AutoSize = true, Font = new Font("Segoe UI", 10F) };
            numPOSDiscount = new NumericUpDown { Location = new Point(160, y - 2), Size = new Size(160, 26), Maximum = 10000000, Increment = 1000, Font = new Font("Segoe UI", 9.5F) };
            numPOSDiscount.ValueChanged += (s, e) => RecalculateCart();

            y += 30;
            var lblT3 = new Label { Text = "KHÁCH PHẢI TRẢ:", Location = new Point(15, y), AutoSize = true, Font = new Font("Segoe UI", 10.5F, FontStyle.Bold) };
            lblPOSFinal = new Label { Text = "0 đ", Location = new Point(160, y), AutoSize = true, Font = new Font("Segoe UI", 13F, FontStyle.Bold), ForeColor = Color.FromArgb(192, 57, 43) };

            y += 35;
            var lblT4 = new Label { Text = "Hình thức TT:", Location = new Point(15, y), AutoSize = true, Font = new Font("Segoe UI", 9.5F) };
            cboPOSPaymentMethod = new ComboBox { Location = new Point(160, y - 2), Size = new Size(160, 26), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9.5F) };
            cboPOSPaymentMethod.Items.AddRange(new object[] { "CASH (Tiền mặt)", "BANK_TRANSFER (Chuyển khoản)" });
            cboPOSPaymentMethod.SelectedIndex = 0;

            y += 30;
            var lblT5 = new Label { Text = "Tiền khách đưa:", Location = new Point(15, y), AutoSize = true, Font = new Font("Segoe UI", 9.5F) };
            txtPOSCustomerMoney = new TextBox { Location = new Point(160, y - 2), Size = new Size(160, 26), Font = new Font("Segoe UI", 9.5F) };
            txtPOSCustomerMoney.TextChanged += (s, e) => CalculateChangeMoney();

            y += 28;
            var lblT6 = new Label { Text = "Tiền thối lại:", Location = new Point(15, y), AutoSize = true, Font = new Font("Segoe UI", 9.5F) };
            lblPOSChange = new Label { Text = "0 đ", Location = new Point(160, y), AutoSize = true, Font = new Font("Segoe UI", 10.5F, FontStyle.Bold), ForeColor = Color.FromArgb(39, 174, 96) };

            y += 30;
            var lblT7 = new Label { Text = "Ghi chú:", Location = new Point(15, y), AutoSize = true, Font = new Font("Segoe UI", 9F) };
            txtPOSNote = new TextBox { Location = new Point(80, y - 2), Size = new Size(240, 24), Font = new Font("Segoe UI", 9F) };

            y += 35;
            btnPOSCheckout = new Button
            {
                Text = "💰 THANH TOÁN (LƯU ĐƠN)",
                Location = new Point(15, y),
                Size = new Size(210, 42),
                BackColor = Color.FromArgb(211, 84, 0),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnPOSCheckout.FlatAppearance.BorderSize = 0;
            btnPOSCheckout.Click += (s, e) => ProcessPOSCheckout();

            btnPOSClear = new Button
            {
                Text = "❌ Hủy Giỏ",
                Location = new Point(235, y),
                Size = new Size(95, 42),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9.5F),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnPOSClear.FlatAppearance.BorderSize = 0;
            btnPOSClear.Click += (s, e) => ClearPOSCart();

            pnlCheckout.Controls.AddRange(new Control[]
            {
                lblT1, lblPOSTotal, lblT2, numPOSDiscount, lblT3, lblPOSFinal,
                lblT4, cboPOSPaymentMethod, lblT5, txtPOSCustomerMoney, lblT6, lblPOSChange,
                lblT7, txtPOSNote, btnPOSCheckout, btnPOSClear
            });

            groupCart.Controls.Add(dgvPOSCart);
            groupCart.Controls.Add(pnlCheckout);
        }

        private void LoadPOSCategories()
        {
            try
            {
                string query = "SELECT CategoryID, CategoryName FROM dbo.Categories WHERE IsActive = 1 ORDER BY CategoryID";
                DataTable dt = DatabaseHelper.ExecuteQuery(query);
                var row = dt.NewRow();
                row["CategoryID"] = 0;
                row["CategoryName"] = "--- Tất cả danh mục ---";
                dt.Rows.InsertAt(row, 0);

                cboPOSCategory.DataSource = dt;
                cboPOSCategory.DisplayMember = "CategoryName";
                cboPOSCategory.ValueMember = "CategoryID";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh mục: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadPOSProducts()
        {
            try
            {
                int catId = 0;
                if (cboPOSCategory.SelectedValue != null && int.TryParse(cboPOSCategory.SelectedValue.ToString(), out int val))
                {
                    catId = val;
                }

                string search = txtPOSSearch.Text.Trim();

                string query = @"
                    SELECT p.ProductID, p.ProductCode AS [Mã Món], p.ProductName AS [Tên Món], 
                           c.CategoryName AS [Danh Mục], p.SellingPrice AS [Giá Bán (VNĐ)], p.Unit AS [ĐVT]
                    FROM dbo.Products p
                    JOIN dbo.Categories c ON p.CategoryID = c.CategoryID
                    WHERE p.IsActive = 1";

                var parameters = new List<SqlParameter>();

                if (catId > 0)
                {
                    query += " AND p.CategoryID = @CatID";
                    parameters.Add(new SqlParameter("@CatID", catId));
                }

                if (!string.IsNullOrEmpty(search))
                {
                    query += " AND (p.ProductName LIKE @Search OR p.ProductCode LIKE @Search)";
                    parameters.Add(new SqlParameter("@Search", "%" + search + "%"));
                }

                query += " ORDER BY p.ProductID";

                DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters.ToArray());
                dgvPOSMenu.DataSource = dt;
                if (dgvPOSMenu.Columns["ProductID"] != null)
                    dgvPOSMenu.Columns["ProductID"].Visible = false;

                if (dgvPOSMenu.Columns["Giá Bán (VNĐ)"] != null)
                    dgvPOSMenu.Columns["Giá Bán (VNĐ)"].DefaultCellStyle.Format = "N0";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải thực đơn: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddSelectedProductToCart()
        {
            if (dgvPOSMenu.CurrentRow == null) return;

            int productId = Convert.ToInt32(dgvPOSMenu.CurrentRow.Cells["ProductID"].Value);
            string code = dgvPOSMenu.CurrentRow.Cells["Mã Món"].Value.ToString() ?? "";
            string name = dgvPOSMenu.CurrentRow.Cells["Tên Món"].Value.ToString() ?? "";
            decimal price = Convert.ToDecimal(dgvPOSMenu.CurrentRow.Cells["Giá Bán (VNĐ)"].Value);

            // Kiểm tra món đã có trong giỏ hàng chưa
            DataRow[] existing = _cartTable.Select("ProductID = " + productId);
            if (existing.Length > 0)
            {
                existing[0]["Quantity"] = Convert.ToDecimal(existing[0]["Quantity"]) + 1;
                existing[0]["TotalAmount"] = Convert.ToDecimal(existing[0]["Quantity"]) * price;
            }
            else
            {
                var newRow = _cartTable.NewRow();
                newRow["ProductID"] = productId;
                newRow["ProductCode"] = code;
                newRow["ProductName"] = name;
                newRow["UnitPrice"] = price;
                newRow["Quantity"] = 1;
                newRow["TotalAmount"] = price;
                _cartTable.Rows.Add(newRow);
            }

            RecalculateCart();
        }

        private decimal _totalOrderAmount = 0;
        private decimal _finalOrderAmount = 0;

        private void RecalculateCart()
        {
            _totalOrderAmount = 0;
            foreach (DataRow row in _cartTable.Rows)
            {
                decimal qty = Convert.ToDecimal(row["Quantity"]);
                decimal price = Convert.ToDecimal(row["UnitPrice"]);
                row["TotalAmount"] = qty * price;
                _totalOrderAmount += qty * price;
            }

            lblPOSTotal.Text = _totalOrderAmount.ToString("N0", _vnCulture) + " đ";

            decimal discount = numPOSDiscount.Value;
            _finalOrderAmount = Math.Max(0, _totalOrderAmount - discount);
            lblPOSFinal.Text = _finalOrderAmount.ToString("N0", _vnCulture) + " đ";

            CalculateChangeMoney();
        }

        private void CalculateChangeMoney()
        {
            if (decimal.TryParse(txtPOSCustomerMoney.Text.Replace(",", "").Replace(".", "").Trim(), out decimal customerMoney))
            {
                decimal change = customerMoney - _finalOrderAmount;
                if (change >= 0)
                {
                    lblPOSChange.Text = change.ToString("N0", _vnCulture) + " đ";
                    lblPOSChange.ForeColor = Color.FromArgb(39, 174, 96);
                }
                else
                {
                    lblPOSChange.Text = "Thiếu: " + Math.Abs(change).ToString("N0", _vnCulture) + " đ";
                    lblPOSChange.ForeColor = Color.FromArgb(192, 57, 43);
                }
            }
            else
            {
                lblPOSChange.Text = "0 đ";
            }
        }

        private void ClearPOSCart()
        {
            _cartTable.Rows.Clear();
            numPOSDiscount.Value = 0;
            txtPOSCustomerMoney.Clear();
            txtPOSNote.Clear();
            RecalculateCart();
        }

        private void ProcessPOSCheckout()
        {
            if (_cartTable.Rows.Count == 0)
            {
                MessageBox.Show("Giỏ hàng đang trống! Vui lòng chọn món trước khi thanh toán.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string orderCode = "HD" + DateTime.Now.ToString("yyyyMMddHHmmss");
            decimal subTotal = _totalOrderAmount;
            decimal discount = numPOSDiscount.Value;
            decimal totalAmount = _finalOrderAmount;
            string paymentMethod = cboPOSPaymentMethod.SelectedIndex == 1 ? "BANK_TRANSFER" : "CASH";
            string note = txtPOSNote.Text.Trim();

            try
            {
                using var conn = DatabaseHelper.GetConnection();
                conn.Open();
                using var transaction = conn.BeginTransaction();

                // 1. Tạo đơn hàng Orders
                string insertOrderSql = @"
                    INSERT INTO dbo.Orders (OrderCode, EmployeeID, OrderDate, SubTotal, DiscountAmount, TotalAmount, PaymentStatus, OrderStatus, Note)
                    VALUES (@OrderCode, @EmployeeID, SYSDATETIME(), @SubTotal, @Discount, @TotalAmount, 'PAID', 'COMPLETED', @Note);
                    SELECT SCOPE_IDENTITY();";

                using var cmdOrder = new SqlCommand(insertOrderSql, conn, transaction);
                cmdOrder.Parameters.AddWithValue("@OrderCode", orderCode);
                cmdOrder.Parameters.AddWithValue("@EmployeeID", UserSession.EmployeeID);
                cmdOrder.Parameters.AddWithValue("@SubTotal", subTotal);
                cmdOrder.Parameters.AddWithValue("@Discount", discount);
                cmdOrder.Parameters.AddWithValue("@TotalAmount", totalAmount);
                cmdOrder.Parameters.AddWithValue("@Note", string.IsNullOrEmpty(note) ? (object)DBNull.Value : note);

                int orderId = Convert.ToInt32(cmdOrder.ExecuteScalar());

                // 2. Tạo OrderDetails
                foreach (DataRow row in _cartTable.Rows)
                {
                    string insertDetailSql = @"
                        INSERT INTO dbo.OrderDetails (OrderID, ProductID, Quantity, UnitPrice, DiscountAmount)
                        VALUES (@OrderID, @ProductID, @Quantity, @UnitPrice, 0);";

                    using var cmdDetail = new SqlCommand(insertDetailSql, conn, transaction);
                    cmdDetail.Parameters.AddWithValue("@OrderID", orderId);
                    cmdDetail.Parameters.AddWithValue("@ProductID", Convert.ToInt32(row["ProductID"]));
                    cmdDetail.Parameters.AddWithValue("@Quantity", Convert.ToDecimal(row["Quantity"]));
                    cmdDetail.Parameters.AddWithValue("@UnitPrice", Convert.ToDecimal(row["UnitPrice"]));
                    cmdDetail.ExecuteNonQuery();
                }

                // 3. Tạo Payments
                string insertPaymentSql = @"
                    INSERT INTO dbo.Payments (OrderID, PaymentMethod, Amount, PaidAt, PaymentStatus)
                    VALUES (@OrderID, @Method, @Amount, SYSDATETIME(), 'SUCCESS');";

                using var cmdPay = new SqlCommand(insertPaymentSql, conn, transaction);
                cmdPay.Parameters.AddWithValue("@OrderID", orderId);
                cmdPay.Parameters.AddWithValue("@Method", paymentMethod);
                cmdPay.Parameters.AddWithValue("@Amount", totalAmount);
                cmdPay.ExecuteNonQuery();

                // Commit transaction tạo đơn
                transaction.Commit();

                // 4. Tự động gọi Stored Procedure trừ kho
                try
                {
                    DatabaseHelper.ExecuteNonQuery("dbo.sp_DeductStockForOrder", new SqlParameter[]
                    {
                        new SqlParameter("@OrderID", orderId)
                    }, CommandType.StoredProcedure);
                }
                catch (Exception exSp)
                {
                    Console.WriteLine("SP deduct error: " + exSp.Message);
                }

                string msg = "Thanh toán thành công!\n\nMã hóa đơn: " + orderCode + "\nTổng tiền: " + totalAmount.ToString("N0", _vnCulture) + " đ\nTồn kho nguyên liệu đã được tự động cập nhật!";
                MessageBox.Show(msg, "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                ClearPOSCart();
                LoadIngredientsTab(); // Làm mới kho
                LoadReportsTab();     // Làm mới báo cáo
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thanh toán đơn hàng:\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region ======================== 2. MODULE QUẢN LÝ THỰC ĐƠN ========================

        private DataGridView dgvProducts = null!;
        private TextBox txtProdCode = null!;
        private TextBox txtProdName = null!;
        private ComboBox cboProdCategory = null!;
        private NumericUpDown numProdPrice = null!;
        private TextBox txtProdUnit = null!;
        private CheckBox chkProdActive = null!;
        private int _selectedProductID = 0;

        private void InitializeProductsTab()
        {
            tabProducts.BackColor = Color.FromArgb(245, 246, 250);

            var split = new SplitContainer { Dock = DockStyle.Fill, SplitterDistance = 780 };
            tabProducts.Controls.Add(split);

            // Trái: Bảng danh sách
            var grpList = new GroupBox
            {
                Text = "DANH SÁCH MÓN ĂN & NƯỚC UỐNG",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                Padding = new Padding(10)
            };
            split.Panel1.Controls.Add(grpList);

            dgvProducts = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                Font = new Font("Segoe UI", 9.5F)
            };
            dgvProducts.CellClick += (s, e) => BindProductToForm();
            grpList.Controls.Add(dgvProducts);

            // Phải: Form thông tin
            var grpForm = new GroupBox
            {
                Text = "THÔNG TIN SẢN PHẨM",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                Padding = new Padding(15)
            };
            split.Panel2.Controls.Add(grpForm);

            int y = 30;
            var l1 = new Label { Text = "Mã sản phẩm:", Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 9.5F) };
            txtProdCode = new TextBox { Location = new Point(20, y + 22), Size = new Size(280, 26), Font = new Font("Segoe UI", 9.5F) };

            y += 60;
            var l2 = new Label { Text = "Tên món:", Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 9.5F) };
            txtProdName = new TextBox { Location = new Point(20, y + 22), Size = new Size(280, 26), Font = new Font("Segoe UI", 9.5F) };

            y += 60;
            var l3 = new Label { Text = "Danh mục:", Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 9.5F) };
            cboProdCategory = new ComboBox { Location = new Point(20, y + 22), Size = new Size(280, 26), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9.5F) };

            y += 60;
            var l4 = new Label { Text = "Giá bán (VNĐ):", Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 9.5F) };
            numProdPrice = new NumericUpDown { Location = new Point(20, y + 22), Size = new Size(280, 26), Maximum = 10000000, Increment = 1000, Font = new Font("Segoe UI", 9.5F) };

            y += 60;
            var l5 = new Label { Text = "Đơn vị tính (Ổ, Ly, Chai...):", Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 9.5F) };
            txtProdUnit = new TextBox { Location = new Point(20, y + 22), Size = new Size(280, 26), Font = new Font("Segoe UI", 9.5F), Text = "Ổ" };

            y += 55;
            chkProdActive = new CheckBox { Text = "Đang kinh doanh", Location = new Point(20, y), AutoSize = true, Checked = true, Font = new Font("Segoe UI", 9.5F) };

            y += 40;
            var btnAdd = new Button { Text = "➕ Thêm Mới", Location = new Point(20, y), Size = new Size(130, 36), BackColor = Color.FromArgb(39, 174, 96), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold) };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += (s, e) => SaveProduct(isNew: true);

            var btnEdit = new Button { Text = "✏️ Cập Nhật", Location = new Point(160, y), Size = new Size(140, 36), BackColor = Color.FromArgb(41, 128, 185), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold) };
            btnEdit.FlatAppearance.BorderSize = 0;
            btnEdit.Click += (s, e) => SaveProduct(isNew: false);

            y += 45;
            var btnDelete = new Button { Text = "🗑️ Xóa Món", Location = new Point(20, y), Size = new Size(130, 36), BackColor = Color.FromArgb(192, 57, 43), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold) };
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.Click += (s, e) => DeleteProduct();

            var btnReset = new Button { Text = "🔄 Làm Mới", Location = new Point(160, y), Size = new Size(140, 36), BackColor = Color.FromArgb(149, 165, 166), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9.5F) };
            btnReset.FlatAppearance.BorderSize = 0;
            btnReset.Click += (s, e) => ResetProductForm();

            grpForm.Controls.AddRange(new Control[]
            {
                l1, txtProdCode, l2, txtProdName, l3, cboProdCategory,
                l4, numProdPrice, l5, txtProdUnit, chkProdActive,
                btnAdd, btnEdit, btnDelete, btnReset
            });
        }

        private void LoadProductsTab()
        {
            try
            {
                // Danh mục
                string queryCat = "SELECT CategoryID, CategoryName FROM dbo.Categories WHERE IsActive = 1";
                DataTable dtCat = DatabaseHelper.ExecuteQuery(queryCat);
                cboProdCategory.DataSource = dtCat;
                cboProdCategory.DisplayMember = "CategoryName";
                cboProdCategory.ValueMember = "CategoryID";

                // Sản phẩm
                string queryProd = @"
                    SELECT p.ProductID, p.ProductCode AS [Mã Món], p.ProductName AS [Tên Món],
                           c.CategoryName AS [Danh Mục], p.SellingPrice AS [Giá Bán (VNĐ)], 
                           p.Unit AS [ĐVT], p.IsActive AS [Hoạt Động], p.CategoryID
                    FROM dbo.Products p
                    JOIN dbo.Categories c ON p.CategoryID = c.CategoryID
                    ORDER BY p.ProductID";

                DataTable dtProd = DatabaseHelper.ExecuteQuery(queryProd);
                dgvProducts.DataSource = dtProd;
                if (dgvProducts.Columns["ProductID"] != null) dgvProducts.Columns["ProductID"].Visible = false;
                if (dgvProducts.Columns["CategoryID"] != null) dgvProducts.Columns["CategoryID"].Visible = false;
                if (dgvProducts.Columns["Giá Bán (VNĐ)"] != null) dgvProducts.Columns["Giá Bán (VNĐ)"].DefaultCellStyle.Format = "N0";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải sản phẩm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BindProductToForm()
        {
            if (dgvProducts.CurrentRow == null) return;
            var row = dgvProducts.CurrentRow;
            _selectedProductID = Convert.ToInt32(row.Cells["ProductID"].Value);
            txtProdCode.Text = row.Cells["Mã Món"].Value.ToString();
            txtProdName.Text = row.Cells["Tên Món"].Value.ToString();
            cboProdCategory.SelectedValue = row.Cells["CategoryID"].Value;
            numProdPrice.Value = Convert.ToDecimal(row.Cells["Giá Bán (VNĐ)"].Value);
            txtProdUnit.Text = row.Cells["ĐVT"].Value.ToString();
            chkProdActive.Checked = Convert.ToBoolean(row.Cells["Hoạt Động"].Value);
        }

        private void ResetProductForm()
        {
            _selectedProductID = 0;
            txtProdCode.Clear();
            txtProdName.Clear();
            numProdPrice.Value = 0;
            txtProdUnit.Text = "Ổ";
            chkProdActive.Checked = true;
        }

        private void SaveProduct(bool isNew)
        {
            string code = txtProdCode.Text.Trim();
            string name = txtProdName.Text.Trim();
            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Vui lòng nhập Mã món và Tên món!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int catId = Convert.ToInt32(cboProdCategory.SelectedValue);
            decimal price = numProdPrice.Value;
            string unit = txtProdUnit.Text.Trim();
            bool isActive = chkProdActive.Checked;

            try
            {
                if (isNew)
                {
                    string sql = @"
                        INSERT INTO dbo.Products (ProductCode, ProductName, CategoryID, SellingPrice, Unit, IsActive)
                        VALUES (@Code, @Name, @CatID, @Price, @Unit, @IsActive)";
                    var p = new SqlParameter[]
                    {
                        new SqlParameter("@Code", code),
                        new SqlParameter("@Name", name),
                        new SqlParameter("@CatID", catId),
                        new SqlParameter("@Price", price),
                        new SqlParameter("@Unit", unit),
                        new SqlParameter("@IsActive", isActive)
                    };
                    DatabaseHelper.ExecuteNonQuery(sql, p);
                    MessageBox.Show("Thêm món mới thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    if (_selectedProductID == 0)
                    {
                        MessageBox.Show("Vui lòng chọn món cần cập nhật từ bảng danh sách!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    string sql = @"
                        UPDATE dbo.Products
                        SET ProductCode = @Code, ProductName = @Name, CategoryID = @CatID, 
                            SellingPrice = @Price, Unit = @Unit, IsActive = @IsActive, UpdatedAt = SYSDATETIME()
                        WHERE ProductID = @ProductID";
                    var p = new SqlParameter[]
                    {
                        new SqlParameter("@ProductID", _selectedProductID),
                        new SqlParameter("@Code", code),
                        new SqlParameter("@Name", name),
                        new SqlParameter("@CatID", catId),
                        new SqlParameter("@Price", price),
                        new SqlParameter("@Unit", unit),
                        new SqlParameter("@IsActive", isActive)
                    };
                    DatabaseHelper.ExecuteNonQuery(sql, p);
                    MessageBox.Show("Cập nhật thông tin món thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                LoadProductsTab();
                LoadPOSProducts();
                ResetProductForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lưu sản phẩm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteProduct()
        {
            if (_selectedProductID == 0)
            {
                MessageBox.Show("Vui lòng chọn món cần xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show("Bạn có chắc muốn xóa (ngừng kinh doanh) món này không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.Yes)
            {
                try
                {
                    string sql = "UPDATE dbo.Products SET IsActive = 0 WHERE ProductID = @ProductID";
                    DatabaseHelper.ExecuteNonQuery(sql, new SqlParameter[] { new SqlParameter("@ProductID", _selectedProductID) });
                    MessageBox.Show("Đã ngừng kinh doanh món này!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadProductsTab();
                    LoadPOSProducts();
                    ResetProductForm();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #endregion

        #region ======================== 3. MODULE KHO NGUYÊN LIỆU ========================

        private DataGridView dgvIngredients = null!;
        private CheckBox chkLowStockOnly = null!;
        private TextBox txtIngCode = null!;
        private TextBox txtIngName = null!;
        private TextBox txtIngUnit = null!;
        private NumericUpDown numIngCurrentStock = null!;
        private NumericUpDown numIngMinStock = null!;
        private NumericUpDown numIngCost = null!;
        private int _selectedIngredientID = 0;

        private void InitializeIngredientsTab()
        {
            tabIngredients.BackColor = Color.FromArgb(245, 246, 250);

            var split = new SplitContainer { Dock = DockStyle.Fill, SplitterDistance = 780 };
            tabIngredients.Controls.Add(split);

            // Bảng danh sách
            var grpList = new GroupBox
            {
                Text = "📦 DANH SÁCH NGUYÊN LIỆU TRONG KHO",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                Padding = new Padding(10)
            };
            split.Panel1.Controls.Add(grpList);

            var pnlFilter = new Panel { Dock = DockStyle.Top, Height = 40 };
            chkLowStockOnly = new CheckBox
            {
                Text = "⚠️ Chỉ hiện nguyên liệu sắp hết (Cảnh báo tồn kho)",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(192, 57, 43),
                Location = new Point(10, 8),
                AutoSize = true
            };
            chkLowStockOnly.CheckedChanged += (s, e) => LoadIngredientsTab();
            pnlFilter.Controls.Add(chkLowStockOnly);
            grpList.Controls.Add(pnlFilter);

            dgvIngredients = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                Font = new Font("Segoe UI", 9.5F)
            };
            dgvIngredients.CellClick += (s, e) => BindIngredientToForm();
            grpList.Controls.Add(dgvIngredients);
            dgvIngredients.BringToFront();

            // Form cập nhật
            var grpForm = new GroupBox
            {
                Text = "QUẢN LÝ NGUYÊN LIỆU",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                Padding = new Padding(15)
            };
            split.Panel2.Controls.Add(grpForm);

            int y = 30;
            var l1 = new Label { Text = "Mã nguyên liệu:", Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 9.5F) };
            txtIngCode = new TextBox { Location = new Point(20, y + 22), Size = new Size(280, 26), Font = new Font("Segoe UI", 9.5F) };

            y += 60;
            var l2 = new Label { Text = "Tên nguyên liệu:", Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 9.5F) };
            txtIngName = new TextBox { Location = new Point(20, y + 22), Size = new Size(280, 26), Font = new Font("Segoe UI", 9.5F) };

            y += 60;
            var l3 = new Label { Text = "Đơn vị (Kg, Quả, Ổ, Hộp...):", Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 9.5F) };
            txtIngUnit = new TextBox { Location = new Point(20, y + 22), Size = new Size(280, 26), Font = new Font("Segoe UI", 9.5F) };

            y += 60;
            var l4 = new Label { Text = "Số lượng tồn kho:", Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 9.5F) };
            numIngCurrentStock = new NumericUpDown { Location = new Point(20, y + 22), Size = new Size(280, 26), Maximum = 100000, DecimalPlaces = 2, Font = new Font("Segoe UI", 9.5F) };

            y += 60;
            var l5 = new Label { Text = "Tồn kho tối thiểu:", Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 9.5F) };
            numIngMinStock = new NumericUpDown { Location = new Point(20, y + 22), Size = new Size(280, 26), Maximum = 100000, DecimalPlaces = 2, Font = new Font("Segoe UI", 9.5F) };

            y += 60;
            var l6 = new Label { Text = "Giá vốn (VNĐ):", Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 9.5F) };
            numIngCost = new NumericUpDown { Location = new Point(20, y + 22), Size = new Size(280, 26), Maximum = 10000000, Increment = 1000, Font = new Font("Segoe UI", 9.5F) };

            y += 60;
            var btnSaveIng = new Button { Text = "💾 Lưu / Cập Nhật", Location = new Point(20, y), Size = new Size(135, 36), BackColor = Color.FromArgb(41, 128, 185), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold) };
            btnSaveIng.FlatAppearance.BorderSize = 0;
            btnSaveIng.Click += (s, e) => SaveIngredient();

            var btnAddStock = new Button { Text = "📥 Nhập Thêm", Location = new Point(165, y), Size = new Size(135, 36), BackColor = Color.FromArgb(39, 174, 96), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold) };
            btnAddStock.FlatAppearance.BorderSize = 0;
            btnAddStock.Click += (s, e) => QuickAddStock();

            grpForm.Controls.AddRange(new Control[]
            {
                l1, txtIngCode, l2, txtIngName, l3, txtIngUnit,
                l4, numIngCurrentStock, l5, numIngMinStock, l6, numIngCost,
                btnSaveIng, btnAddStock
            });
        }

        private void LoadIngredientsTab()
        {
            try
            {
                string query;
                if (chkLowStockOnly.Checked)
                {
                    query = @"
                        SELECT IngredientID, IngredientCode AS [Mã NL], IngredientName AS [Tên Nguyên Liệu],
                               Unit AS [ĐVT], CurrentStock AS [Tồn Kho], MinimumStock AS [Tồn Tối Thiểu], CostPrice AS [Giá Vốn]
                        FROM dbo.Ingredients
                        WHERE CurrentStock <= MinimumStock AND IsActive = 1
                        ORDER BY CurrentStock ASC";
                }
                else
                {
                    query = @"
                        SELECT IngredientID, IngredientCode AS [Mã NL], IngredientName AS [Tên Nguyên Liệu],
                               Unit AS [ĐVT], CurrentStock AS [Tồn Kho], MinimumStock AS [Tồn Tối Thiểu], CostPrice AS [Giá Vốn]
                        FROM dbo.Ingredients
                        WHERE IsActive = 1
                        ORDER BY IngredientID";
                }

                DataTable dt = DatabaseHelper.ExecuteQuery(query);
                dgvIngredients.DataSource = dt;
                if (dgvIngredients.Columns["IngredientID"] != null)
                    dgvIngredients.Columns["IngredientID"].Visible = false;

                if (dgvIngredients.Columns["Giá Vốn"] != null)
                    dgvIngredients.Columns["Giá Vốn"].DefaultCellStyle.Format = "N0";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải kho nguyên liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BindIngredientToForm()
        {
            if (dgvIngredients.CurrentRow == null) return;
            var row = dgvIngredients.CurrentRow;
            _selectedIngredientID = Convert.ToInt32(row.Cells["IngredientID"].Value);
            txtIngCode.Text = row.Cells["Mã NL"].Value.ToString();
            txtIngName.Text = row.Cells["Tên Nguyên Liệu"].Value.ToString();
            txtIngUnit.Text = row.Cells["ĐVT"].Value.ToString();
            numIngCurrentStock.Value = Convert.ToDecimal(row.Cells["Tồn Kho"].Value);
            numIngMinStock.Value = Convert.ToDecimal(row.Cells["Tồn Tối Thiểu"].Value);
            numIngCost.Value = Convert.ToDecimal(row.Cells["Giá Vốn"].Value);
        }

        private void SaveIngredient()
        {
            string code = txtIngCode.Text.Trim();
            string name = txtIngName.Text.Trim();
            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Vui lòng nhập Mã và Tên nguyên liệu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (_selectedIngredientID > 0)
                {
                    string sql = @"
                        UPDATE dbo.Ingredients
                        SET IngredientCode = @Code, IngredientName = @Name, Unit = @Unit,
                            CurrentStock = @Current, MinimumStock = @Min, CostPrice = @Cost
                        WHERE IngredientID = @ID";
                    var p = new SqlParameter[]
                    {
                        new SqlParameter("@ID", _selectedIngredientID),
                        new SqlParameter("@Code", code),
                        new SqlParameter("@Name", name),
                        new SqlParameter("@Unit", txtIngUnit.Text.Trim()),
                        new SqlParameter("@Current", numIngCurrentStock.Value),
                        new SqlParameter("@Min", numIngMinStock.Value),
                        new SqlParameter("@Cost", numIngCost.Value)
                    };
                    DatabaseHelper.ExecuteNonQuery(sql, p);
                    MessageBox.Show("Cập nhật nguyên liệu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    string sql = @"
                        INSERT INTO dbo.Ingredients (IngredientCode, IngredientName, Unit, CurrentStock, MinimumStock, CostPrice, IsActive)
                        VALUES (@Code, @Name, @Unit, @Current, @Min, @Cost, 1)";
                    var p = new SqlParameter[]
                    {
                        new SqlParameter("@Code", code),
                        new SqlParameter("@Name", name),
                        new SqlParameter("@Unit", txtIngUnit.Text.Trim()),
                        new SqlParameter("@Current", numIngCurrentStock.Value),
                        new SqlParameter("@Min", numIngMinStock.Value),
                        new SqlParameter("@Cost", numIngCost.Value)
                    };
                    DatabaseHelper.ExecuteNonQuery(sql, p);
                    MessageBox.Show("Thêm nguyên liệu mới thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                LoadIngredientsTab();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lưu nguyên liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void QuickAddStock()
        {
            if (_selectedIngredientID == 0)
            {
                MessageBox.Show("Vui lòng chọn nguyên liệu cần nhập thêm!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            numIngCurrentStock.Value += 10;
            SaveIngredient();
        }

        #endregion

        #region ======================== 4. MODULE BÁO CÁO DOANH THU ========================

        private DataGridView dgvDailyRevenue = null!;
        private DataGridView dgvTopProducts = null!;
        private Label lblReportTotalRev = null!;
        private Label lblReportTotalOrders = null!;

        private void InitializeReportsTab()
        {
            tabReports.BackColor = Color.FromArgb(245, 246, 250);

            var pnlTopCards = new Panel { Dock = DockStyle.Top, Height = 90, Padding = new Padding(15) };
            tabReports.Controls.Add(pnlTopCards);

            // Card 1: Tổng doanh thu
            var card1 = new Panel { Location = new Point(20, 10), Size = new Size(300, 70), BackColor = Color.FromArgb(39, 174, 96) };
            var lblC1T = new Label { Text = "TỔNG DOANH THU", Location = new Point(15, 10), ForeColor = Color.White, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), AutoSize = true };
            lblReportTotalRev = new Label { Text = "0 đ", Location = new Point(15, 32), ForeColor = Color.White, Font = new Font("Segoe UI", 14F, FontStyle.Bold), AutoSize = true };
            card1.Controls.AddRange(new Control[] { lblC1T, lblReportTotalRev });

            // Card 2: Tổng đơn hàng
            var card2 = new Panel { Location = new Point(340, 10), Size = new Size(250, 70), BackColor = Color.FromArgb(41, 128, 185) };
            var lblC2T = new Label { Text = "TỔNG ĐƠN HOÀN THÀNH", Location = new Point(15, 10), ForeColor = Color.White, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), AutoSize = true };
            lblReportTotalOrders = new Label { Text = "0 đơn", Location = new Point(15, 32), ForeColor = Color.White, Font = new Font("Segoe UI", 14F, FontStyle.Bold), AutoSize = true };
            card2.Controls.AddRange(new Control[] { lblC2T, lblReportTotalOrders });

            var btnRefreshReport = new Button
            {
                Text = "🔄 Làm Mới Báo Cáo",
                Location = new Point(610, 20),
                Size = new Size(160, 45),
                BackColor = Color.FromArgb(44, 62, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnRefreshReport.FlatAppearance.BorderSize = 0;
            btnRefreshReport.Click += (s, e) => LoadReportsTab();

            pnlTopCards.Controls.AddRange(new Control[] { card1, card2, btnRefreshReport });

            var split = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Horizontal, SplitterDistance = 300 };
            tabReports.Controls.Add(split);
            split.BringToFront();

            // Bảng 1: Doanh thu theo ngày (dùng View v_DailyRevenue)
            var grpDaily = new GroupBox
            {
                Text = "📅 DOANH THU THEO NGÀY (v_DailyRevenue)",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                Padding = new Padding(10)
            };
            split.Panel1.Controls.Add(grpDaily);

            dgvDailyRevenue = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                Font = new Font("Segoe UI", 9.5F)
            };
            grpDaily.Controls.Add(dgvDailyRevenue);

            // Bảng 2: Top sản phẩm bán chạy (dùng View v_ProductSales)
            var grpTop = new GroupBox
            {
                Text = "🏆 THỐNG KÊ SẢN PHẨM BÁN CHẠY (v_ProductSales)",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                Padding = new Padding(10)
            };
            split.Panel2.Controls.Add(grpTop);

            dgvTopProducts = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                Font = new Font("Segoe UI", 9.5F)
            };
            grpTop.Controls.Add(dgvTopProducts);
        }

        private void LoadReportsTab()
        {
            try
            {
                // Doanh thu theo ngày
                string queryDaily = @"
                    SELECT SaleDate AS [Ngày Bán], TotalOrders AS [Số Đơn Hàng], TotalRevenue AS [Doanh Thu (VNĐ)]
                    FROM dbo.v_DailyRevenue
                    ORDER BY SaleDate DESC";

                DataTable dtDaily = DatabaseHelper.ExecuteQuery(queryDaily);
                dgvDailyRevenue.DataSource = dtDaily;
                if (dgvDailyRevenue.Columns["Doanh Thu (VNĐ)"] != null)
                    dgvDailyRevenue.Columns["Doanh Thu (VNĐ)"].DefaultCellStyle.Format = "N0";

                // Bán theo món
                string querySales = @"
                    SELECT ProductCode AS [Mã Món], ProductName AS [Tên Món], 
                           TotalQuantitySold AS [Số Lượng Bán], TotalRevenue AS [Tổng Thu (VNĐ)]
                    FROM dbo.v_ProductSales
                    ORDER BY TotalQuantitySold DESC";

                DataTable dtSales = DatabaseHelper.ExecuteQuery(querySales);
                dgvTopProducts.DataSource = dtSales;
                if (dgvTopProducts.Columns["Tổng Thu (VNĐ)"] != null)
                    dgvTopProducts.Columns["Tổng Thu (VNĐ)"].DefaultCellStyle.Format = "N0";

                // Tính tổng Card
                decimal totalRev = 0;
                int totalOrders = 0;
                foreach (DataRow r in dtDaily.Rows)
                {
                    totalRev += Convert.ToDecimal(r["Doanh Thu (VNĐ)"]);
                    totalOrders += Convert.ToInt32(r["Số Đơn Hàng"]);
                }

                lblReportTotalRev.Text = totalRev.ToString("N0", _vnCulture) + " đ";
                lblReportTotalOrders.Text = totalOrders.ToString() + " đơn";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải báo cáo: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region ======================== 5. MODULE NHÂN VIÊN ========================

        private DataGridView dgvEmployees = null!;

        private void InitializeEmployeesTab()
        {
            tabEmployees.BackColor = Color.FromArgb(245, 246, 250);

            var grp = new GroupBox
            {
                Text = "👥 DANH SÁCH NHÂN VIÊN & TÀI KHOẢN HỆ THỐNG",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                Padding = new Padding(15)
            };
            tabEmployees.Controls.Add(grp);

            dgvEmployees = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                Font = new Font("Segoe UI", 9.5F)
            };
            grp.Controls.Add(dgvEmployees);
        }

        private void LoadEmployeesTab()
        {
            try
            {
                string query = @"
                    SELECT e.EmployeeID, e.EmployeeCode AS [Mã NV], e.FullName AS [Họ và Tên], 
                           e.Phone AS [SĐT], e.Email, e.Position AS [Chức Vụ], 
                           u.Username AS [Tên Đăng Nhập], r.RoleName AS [Quyền Hạn], 
                           e.Status AS [Trạng Thái]
                    FROM dbo.Employees e
                    LEFT JOIN dbo.Users u ON e.EmployeeID = u.EmployeeID
                    LEFT JOIN dbo.Roles r ON u.RoleID = r.RoleID
                    ORDER BY e.EmployeeID";

                DataTable dt = DatabaseHelper.ExecuteQuery(query);
                dgvEmployees.DataSource = dt;
                if (dgvEmployees.Columns["EmployeeID"] != null)
                    dgvEmployees.Columns["EmployeeID"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải nhân viên: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion
    }
}
