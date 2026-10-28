using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace Login.Controls
{
    public class RoleCardButton : Control
    {
        private string _roleName = "Vai trò";
        private Image? _roleIcon = null;
        private bool _isSelected = false;
        private bool _isHovered = false;

        private Color _selectedBorderColor = Color.FromArgb(200, 122, 62);
        private Color _selectedBackColor = Color.FromArgb(253, 244, 237);
        private Color _selectedTextColor = Color.FromArgb(122, 62, 29);

        private Color _normalBorderColor = Color.FromArgb(229, 231, 235);
        private Color _normalBackColor = Color.White;
        private Color _normalTextColor = Color.FromArgb(55, 65, 81);
        private Color _hoverBackColor = Color.FromArgb(250, 247, 242);

        private int _borderRadius = 10;

        public event EventHandler? SelectedChanged;

        public string RoleName
        {
            get => _roleName;
            set { _roleName = value; Invalidate(); }
        }

        public Image? RoleIcon
        {
            get => _roleIcon;
            set { _roleIcon = value; Invalidate(); }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    Invalidate();
                    SelectedChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public RoleCardButton()
        {
            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);
            DoubleBuffered = true;
            Size = new Size(122, 74);
            Cursor = Cursors.Hand;
            Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _isHovered = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _isHovered = false;
            Invalidate();
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            if (!_isSelected)
            {
                IsSelected = true;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            using var path = RoundedPanel.CreateRoundedRectangle(rect, _borderRadius);

            // Determine colors
            Color bg = _isSelected ? _selectedBackColor : (_isHovered ? _hoverBackColor : _normalBackColor);
            Color border = _isSelected ? _selectedBorderColor : _normalBorderColor;
            Color text = _isSelected ? _selectedTextColor : _normalTextColor;
            int borderWidth = _isSelected ? 2 : 1;

            // Fill background
            using (var brush = new SolidBrush(bg))
            {
                g.FillPath(brush, path);
            }

            // Draw border
            using (var pen = new Pen(border, borderWidth))
            {
                pen.Alignment = PenAlignment.Inset;
                g.DrawPath(pen, path);
            }

            // Draw icon at top center
            if (_roleIcon != null)
            {
                int iconW = 28;
                int iconH = 28;
                int iconX = (Width - iconW) / 2;
                int iconY = 11;
                g.DrawImage(_roleIcon, new Rectangle(iconX, iconY, iconW, iconH));
            }

            // Draw role text at bottom center
            using var font = new Font(Font.FontFamily, 9.5f, _isSelected ? FontStyle.Bold : FontStyle.Regular);
            Size textSize = TextRenderer.MeasureText(_roleName, font);
            int textX = (Width - textSize.Width) / 2;
            int textY = 44;

            TextRenderer.DrawText(g, _roleName, font, new Point(textX, textY), text);
        }
    }
}
