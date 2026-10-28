using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace Login.Controls
{
    public class ModernButton : Button
    {
        private int _borderRadius = 8;
        private Color _normalColor = Color.FromArgb(161, 82, 26); // Terracotta bread crust brown
        private Color _hoverColor = Color.FromArgb(140, 70, 20);
        private Color _pressedColor = Color.FromArgb(115, 53, 12);
        private Color _borderColor = Color.Transparent;
        private int _borderWidth = 0;
        private Image? _buttonIcon = null;
        private bool _isHovered = false;
        private bool _isPressed = false;

        public int BorderRadius
        {
            get => _borderRadius;
            set { _borderRadius = value; Invalidate(); }
        }

        public Color NormalColor
        {
            get => _normalColor;
            set { _normalColor = value; Invalidate(); }
        }

        public Color HoverColor
        {
            get => _hoverColor;
            set { _hoverColor = value; Invalidate(); }
        }

        public Color PressedColor
        {
            get => _pressedColor;
            set { _pressedColor = value; Invalidate(); }
        }

        public Color BorderColor
        {
            get => _borderColor;
            set { _borderColor = value; Invalidate(); }
        }

        public int BorderWidth
        {
            get => _borderWidth;
            set { _borderWidth = value; Invalidate(); }
        }

        public Image? ButtonIcon
        {
            get => _buttonIcon;
            set { _buttonIcon = value; Invalidate(); }
        }

        public ModernButton()
        {
            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);
            DoubleBuffered = true;
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            BackColor = Color.Transparent;
            ForeColor = Color.White;
            Cursor = Cursors.Hand;
            Font = new Font("Segoe UI", 11f, FontStyle.Bold);
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
            _isPressed = false;
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            base.OnMouseDown(mevent);
            if (mevent.Button == MouseButtons.Left)
            {
                _isPressed = true;
                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            base.OnMouseUp(mevent);
            _isPressed = false;
            Invalidate();
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            var baseSize = base.GetPreferredSize(proposedSize);
            int iconSize = _buttonIcon != null ? 22 : 0;
            int gap = _buttonIcon != null ? 10 : 0;
            Size textSize = TextRenderer.MeasureText(Text, Font);
            int desiredW = iconSize + gap + textSize.Width + 28;
            int desiredH = Math.Max(baseSize.Height, 34);
            return new Size(Math.Max(baseSize.Width, desiredW), desiredH);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            var g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            using var path = RoundedPanel.CreateRoundedRectangle(rect, _borderRadius);

            // Determine background color
            Color currentBg = _isPressed ? _pressedColor : (_isHovered ? _hoverColor : _normalColor);

            using (var brush = new SolidBrush(currentBg))
            {
                g.FillPath(brush, path);
            }

            // Draw border if needed
            if (_borderWidth > 0 && _borderColor != Color.Transparent)
            {
                using var pen = new Pen(_borderColor, _borderWidth);
                pen.Alignment = PenAlignment.Inset;
                g.DrawPath(pen, path);
            }

            // Calculate content placement (icon + text centered horizontally)
            int iconSize = _buttonIcon != null ? 22 : 0;
            int gap = _buttonIcon != null ? 10 : 0;
            Size textSize = TextRenderer.MeasureText(Text, Font);

            int totalWidth = iconSize + gap + textSize.Width;
            int startX = Math.Max(4, (Width - totalWidth) / 2);
            int centerY = Height / 2;

            if (_buttonIcon != null)
            {
                int iconY = centerY - (iconSize / 2);
                g.DrawImage(_buttonIcon, new Rectangle(startX, iconY, iconSize, iconSize));
            }

            int textX = startX + iconSize + gap;
            int textY = centerY - (textSize.Height / 2);

            TextRenderer.DrawText(g, Text, Font, new Point(textX, textY), ForeColor);
        }
    }
}
