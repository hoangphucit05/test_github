using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace Login.Controls
{
    /// <summary>
    /// DataGridView cao cấp theo thiết kế Bánh Mì Ngon:
    /// - Header sáng thanh lịch (#F4F6F8)
    /// - Badge trạng thái dạng viên nang (pill) mềm mại (Xanh lá / Đỏ nhạt)
    /// - Cột "Thao tác" với biểu tượng Sửa (bút chì) và Khóa/Xóa (thùng rác)
    /// - Click vào icon trong cột Thao tác kích hoạt event tương ứng
    /// </summary>
    [ToolboxItem(true)]
    public class ModernDataGrid : DataGridView
    {
        // Theme Colors
        private static readonly Color HeaderBg        = Color.FromArgb(244, 246, 248);
        private static readonly Color HeaderFg        = Color.FromArgb(70, 80, 95);
        private static readonly Color RowAlt          = Color.FromArgb(252, 250, 247);
        private static readonly Color RowNormal       = Color.White;
        private static readonly Color SelectionBg     = Color.FromArgb(254, 243, 235);
        private static readonly Color SelectionFg     = Color.FromArgb(50, 30, 15);
        private static readonly Color GridLine        = Color.FromArgb(240, 237, 232);

        // Status pill colors
        private static readonly Color PillActiveBg    = Color.FromArgb(220, 245, 226);
        private static readonly Color PillActiveFg    = Color.FromArgb(22, 101, 52);
        private static readonly Color PillActiveBdr   = Color.FromArgb(187, 247, 208);

        private static readonly Color PillInactiveBg  = Color.FromArgb(254, 226, 226);
        private static readonly Color PillInactiveFg  = Color.FromArgb(153, 27, 27);
        private static readonly Color PillInactiveBdr = Color.FromArgb(254, 202, 202);

        // Action icon colors
        private static readonly Color ActionEditColor  = Color.FromArgb(90, 100, 115);
        private static readonly Color ActionTrashColor = Color.FromArgb(220, 53, 69);

        // Events
        public event EventHandler<int>? RowEditClicked;
        public event EventHandler<int>? RowDeleteClicked;

        public ModernDataGrid()
        {
            Dock                        = DockStyle.Fill;
            ReadOnly                    = true;
            AllowUserToAddRows          = false;
            AllowUserToDeleteRows       = false;
            AllowUserToResizeRows       = false;
            MultiSelect                 = false;
            SelectionMode               = DataGridViewSelectionMode.FullRowSelect;
            AutoSizeColumnsMode         = DataGridViewAutoSizeColumnsMode.Fill;
            RowHeadersVisible           = false;
            BorderStyle                 = BorderStyle.None;
            CellBorderStyle             = DataGridViewCellBorderStyle.Single;
            GridColor                   = GridLine;
            DoubleBuffered              = true;

            // Background & Alternation
            BackgroundColor             = Color.White;
            AlternatingRowsDefaultCellStyle.BackColor = RowAlt;

            // Row style
            DefaultCellStyle.BackColor  = RowNormal;
            DefaultCellStyle.ForeColor  = Color.FromArgb(45, 55, 72);
            DefaultCellStyle.Font       = new Font("Segoe UI", 9.5f);
            DefaultCellStyle.Padding    = new Padding(8, 4, 8, 4);
            DefaultCellStyle.SelectionBackColor = SelectionBg;
            DefaultCellStyle.SelectionForeColor = SelectionFg;

            // Header style
            EnableHeadersVisualStyles    = false;
            ColumnHeadersDefaultCellStyle.BackColor          = HeaderBg;
            ColumnHeadersDefaultCellStyle.ForeColor          = HeaderFg;
            ColumnHeadersDefaultCellStyle.Font               = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            ColumnHeadersDefaultCellStyle.Padding            = new Padding(8, 8, 8, 8);
            ColumnHeadersDefaultCellStyle.SelectionBackColor = HeaderBg;
            ColumnHeadersDefaultCellStyle.SelectionForeColor = HeaderFg;
            ColumnHeadersHeightSizeMode        = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            ColumnHeadersHeight                = 42;
            RowTemplate.Height                 = 44;
        }

        protected override void OnDataBindingComplete(DataGridViewBindingCompleteEventArgs e)
        {
            base.OnDataBindingComplete(e);

            // Tự động định dạng các cột theo mockup
            foreach (DataGridViewColumn col in Columns)
            {
                if (col.Name.EndsWith("ID", StringComparison.OrdinalIgnoreCase) && col.Name.Length > 2)
                {
                    col.Visible = false;
                }
                else if (col.Name == "STT" || col.Name == "#")
                {
                    col.Width = 50;
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
                else if (col.Name.Contains("Trạng", StringComparison.OrdinalIgnoreCase) ||
                         col.Name.Contains("Status", StringComparison.OrdinalIgnoreCase))
                {
                    col.Width = 140;
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
                else if (col.Name.Contains("Thao_tác", StringComparison.OrdinalIgnoreCase) ||
                         col.Name.Contains("Thao tác", StringComparison.OrdinalIgnoreCase))
                {
                    col.Width = 100;
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
                else if (col.Name.Contains("Ngày", StringComparison.OrdinalIgnoreCase) ||
                         col.Name.Contains("Giờ", StringComparison.OrdinalIgnoreCase) ||
                         col.Name.Contains("Thời_lượng", StringComparison.OrdinalIgnoreCase))
                {
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
            }
        }

        protected override void OnCellPainting(DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                base.OnCellPainting(e);
                return;
            }

            string colName = Columns[e.ColumnIndex].Name;

            // 1. Vẽ badge trạng thái dạng pill
            if (colName.Contains("Trạng", StringComparison.OrdinalIgnoreCase) ||
                colName.Contains("Status", StringComparison.OrdinalIgnoreCase))
            {
                e.PaintBackground(e.CellBounds, true);

                string val = e.Value?.ToString() ?? "";
                if (!string.IsNullOrEmpty(val))
                {
                    bool isActive = val.Contains("hoạt động", StringComparison.OrdinalIgnoreCase) &&
                                    !val.Contains("ngừng", StringComparison.OrdinalIgnoreCase) &&
                                    !val.Contains("tạm", StringComparison.OrdinalIgnoreCase);

                    Color bg  = isActive ? PillActiveBg  : PillInactiveBg;
                    Color fg  = isActive ? PillActiveFg  : PillInactiveFg;
                    Color bdr = isActive ? PillActiveBdr : PillInactiveBdr;

                    var g = e.Graphics;
                    if (g == null) { e.Handled = true; return; }
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                    // Tính kích thước pill
                    using var font = new Font("Segoe UI", 8.75f, FontStyle.Bold);
                    var txtSize = TextRenderer.MeasureText(val, font);
                    int pillWidth = Math.Max(txtSize.Width + 20, 95);
                    int pillHeight = 24;
                    int pillX = e.CellBounds.X + (e.CellBounds.Width - pillWidth) / 2;
                    int pillY = e.CellBounds.Y + (e.CellBounds.Height - pillHeight) / 2;

                    var pillRect = new Rectangle(pillX, pillY, pillWidth, pillHeight);
                    using var path = CreateRoundedRect(pillRect, pillHeight / 2);
                    using var brush = new SolidBrush(bg);
                    using var pen = new Pen(bdr, 1f);

                    g.FillPath(brush, path);
                    g.DrawPath(pen, path);

                    // Vẽ text ở tâm pill
                    using var sf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    using var textBrush = new SolidBrush(fg);
                    g.DrawString(val, font, textBrush, pillRect, sf);
                }

                DrawCellBorders(e.Graphics, e.CellBounds);
                e.Handled = true;
                return;
            }

            // 2. Vẽ nút thao tác (bút chì + thùng rác)
            if (colName.Contains("Thao_tác", StringComparison.OrdinalIgnoreCase) ||
                colName.Contains("Thao tác", StringComparison.OrdinalIgnoreCase))
            {
                e.PaintBackground(e.CellBounds, true);

                var g = e.Graphics;
                if (g == null) { e.Handled = true; return; }
                g.SmoothingMode = SmoothingMode.AntiAlias;

                int midX = e.CellBounds.X + e.CellBounds.Width / 2;
                int midY = e.CellBounds.Y + e.CellBounds.Height / 2;

                // Nút Edit (trái)
                var editRect = new Rectangle(midX - 30, midY - 12, 24, 24);
                // Nút Delete/Lock (phải)
                var trashRect = new Rectangle(midX + 6, midY - 12, 24, 24);

                // Draw Edit glyph ✏
                DrawPencilGlyph(g, editRect, ActionEditColor);
                // Draw Trash glyph 🗑
                DrawTrashGlyph(g, trashRect, ActionTrashColor);

                DrawCellBorders(e.Graphics, e.CellBounds);
                e.Handled = true;
                return;
            }

            base.OnCellPainting(e);

            // 3. Nếu hàng đang được chọn (Selected), vẽ lại đường viền dọc và ngang
            // vì FullRowSelect tô nền đè lên các đường gridlines
            if (e.RowIndex >= 0 && (e.State & DataGridViewElementStates.Selected) != 0)
            {
                DrawCellBorders(e.Graphics, e.CellBounds);
            }
        }

        private void DrawCellBorders(Graphics? g, Rectangle bounds)
        {
            if (g == null) return;
            using var pen = new Pen(GridColor, 1f);
            // Đường kẻ dọc bên phải ô
            g.DrawLine(pen, bounds.Right - 1, bounds.Top, bounds.Right - 1, bounds.Bottom - 1);
            // Đường kẻ ngang đáy ô
            g.DrawLine(pen, bounds.Left, bounds.Bottom - 1, bounds.Right - 1, bounds.Bottom - 1);
        }

        protected override void OnCellMouseClick(DataGridViewCellMouseEventArgs e)
        {
            base.OnCellMouseClick(e);
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            // The action handlers read CurrentRow. A click on a painted action icon
            // does not reliably move CurrentRow first, so explicitly select the row
            // the user actually clicked.
            ClearSelection();
            Rows[e.RowIndex].Selected = true;
            CurrentCell = Rows[e.RowIndex].Cells[e.ColumnIndex];

            string colName = Columns[e.ColumnIndex].Name;
            if (colName.Contains("Thao_tác", StringComparison.OrdinalIgnoreCase) ||
                colName.Contains("Thao tác", StringComparison.OrdinalIgnoreCase))
            {
                var cellRect = GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
                int midX = cellRect.X + cellRect.Width / 2;

                if (e.X < midX)
                {
                    RowEditClicked?.Invoke(this, e.RowIndex);
                }
                else
                {
                    RowDeleteClicked?.Invoke(this, e.RowIndex);
                }
            }
        }

        private static void DrawPencilGlyph(Graphics g, Rectangle r, Color color)
        {
            // Bút chì nhỏ nghiêng 45 độ
            using var pen = new Pen(color, 1.8f);
            using var brush = new SolidBrush(color);

            int cx = r.X + r.Width / 2;
            int cy = r.Y + r.Height / 2;

            Point[] body = new[]
            {
                new Point(cx - 5, cy + 3),
                new Point(cx + 3, cy - 5),
                new Point(cx + 6, cy - 2),
                new Point(cx - 2, cy + 6)
            };
            g.DrawPolygon(pen, body);

            // Mũi bút
            Point[] tip = new[]
            {
                new Point(cx - 5, cy + 3),
                new Point(cx - 2, cy + 6),
                new Point(cx - 8, cy + 7)
            };
            g.FillPolygon(brush, tip);
        }

        private static void DrawTrashGlyph(Graphics g, Rectangle r, Color color)
        {
            // Thùng rác đỏ
            using var pen = new Pen(color, 1.6f);
            using var brush = new SolidBrush(color);

            int cx = r.X + r.Width / 2;
            int cy = r.Y + r.Height / 2;

            // Nắp
            g.DrawLine(pen, cx - 6, cy - 4, cx + 6, cy - 4);
            g.FillRectangle(brush, cx - 2, cy - 6, 4, 2);

            // Thân thùng
            Point[] can = new[]
            {
                new Point(cx - 5, cy - 3),
                new Point(cx + 5, cy - 3),
                new Point(cx + 4, cy + 6),
                new Point(cx - 4, cy + 6)
            };
            g.DrawPolygon(pen, can);

            // Nan dọc
            g.DrawLine(pen, cx - 2, cy - 1, cx - 2, cy + 4);
            g.DrawLine(pen, cx + 2, cy - 1, cx + 2, cy + 4);
        }

        private static GraphicsPath CreateRoundedRect(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int d = radius * 2;
            if (d > rect.Width) d = rect.Width;
            if (d > rect.Height) d = rect.Height;

            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
