using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace Login.Controls
{
    /// <summary>
    /// Nút sidebar hiện đại theo mockup thiết kế Bánh Mì Ngon.
    /// Có hiệu ứng bo góc (pill), highlight cam-nâu khi active, hover mềm mại.
    /// </summary>
    public class SidebarNavButton : Control
    {
        private bool _isSelected;
        private bool _isHovered;
        private Image? _icon;
        private string _label = string.Empty;

        // Palette
        private static readonly Color BgSidebar   = Color.FromArgb(115, 55, 24);
        private static readonly Color BgHover     = Color.FromArgb(145, 75, 35);
        private static readonly Color BgSelected  = Color.FromArgb(182, 88, 28);
        private static readonly Color AccentBar   = Color.FromArgb(255, 195, 110);
        private static readonly Color TextNormal  = Color.FromArgb(240, 222, 210);
        private static readonly Color TextActive  = Color.White;

        public bool IsSelected
        {
            get => _isSelected;
            set { _isSelected = value; Invalidate(); }
        }

        public Image? NavIcon
        {
            get => _icon;
            set { _icon = value; Invalidate(); }
        }

        public string Label
        {
            get => _label;
            set { _label = value; Invalidate(); }
        }

        public SidebarNavButton()
        {
            Height = 48;
            Dock = DockStyle.Top;
            Cursor = Cursors.Hand;
            Font = new Font("Segoe UI", 9.75f, FontStyle.Regular);
            DoubleBuffered = true;
            SetStyle(ControlStyles.SupportsTransparentBackColor |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint, true);
            BackColor = Color.Transparent;
        }

        protected override void OnMouseEnter(EventArgs e) { _isHovered = true; Invalidate(); }
        protected override void OnMouseLeave(EventArgs e) { _isHovered = false; Invalidate(); }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            // Inset button rect so it appears as a rounded card inside sidebar
            var btnRect = new Rectangle(8, 2, Width - 16, Height - 4);

            if (_isSelected)
            {
                using var path = CreateRoundedRect(btnRect, 8);
                using var brush = new SolidBrush(BgSelected);
                g.FillPath(brush, path);

                // Accent bar on the left
                using var barBrush = new SolidBrush(AccentBar);
                using var barPath = CreateRoundedRect(new Rectangle(btnRect.X + 2, btnRect.Y + 4, 4, btnRect.Height - 8), 2);
                g.FillPath(barBrush, barPath);
            }
            else if (_isHovered)
            {
                using var path = CreateRoundedRect(btnRect, 8);
                using var brush = new SolidBrush(BgHover);
                g.FillPath(brush, path);
            }

            int x = btnRect.X + (_isSelected ? 16 : 14);

            // Icon
            if (_icon != null)
            {
                int iconSize = 20;
                int iconY = (Height - iconSize) / 2;
                g.DrawImage(_icon, x, iconY, iconSize, iconSize);
                x += iconSize + 12;
            }
            else
            {
                x += 8;
            }

            // Text
            using var sf = new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center
            };
            Color textColor = _isSelected ? TextActive : (_isHovered ? Color.White : TextNormal);
            var font = _isSelected ? new Font(Font, FontStyle.Bold) : Font;
            using var textBrush = new SolidBrush(textColor);
            g.DrawString(_label, font, textBrush, new RectangleF(x, 0, Width - x - 8, Height), sf);
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

