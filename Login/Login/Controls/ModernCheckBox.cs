using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace Login.Controls
{
    public class ModernCheckBox : Control
    {
        private bool _checked = true;
        private string _text = "Ghi nhớ tài khoản";
        private Color _boxCheckedColor = Color.FromArgb(37, 99, 235); // Blue
        private Color _boxBorderColor = Color.FromArgb(209, 213, 219);
        private Color _textColor = Color.FromArgb(55, 65, 81);
        private bool _isHovered = false;

        public event EventHandler? CheckedChanged;

        public bool Checked
        {
            get => _checked;
            set
            {
                if (_checked != value)
                {
                    _checked = value;
                    Invalidate();
                    CheckedChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public new string Text
        {
            get => _text;
            set { _text = value; Invalidate(); }
        }

        public Color BoxCheckedColor
        {
            get => _boxCheckedColor;
            set { _boxCheckedColor = value; Invalidate(); }
        }

        public ModernCheckBox()
        {
            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.SupportsTransparentBackColor, true);
            DoubleBuffered = true;
            BackColor = Color.Transparent;
            Cursor = Cursors.Hand;
            Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);
            Size = new Size(160, 24);
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
            Checked = !Checked;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            int boxSize = 18;
            int boxY = (Height - boxSize) / 2;
            var boxRect = new Rectangle(0, boxY, boxSize, boxSize);

            using var path = RoundedPanel.CreateRoundedRectangle(boxRect, 4);

            if (_checked)
            {
                using var fillBrush = new SolidBrush(_boxCheckedColor);
                g.FillPath(fillBrush, path);

                // Draw checkmark
                using var pen = new Pen(Color.White, 2f);
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                var p1 = new Point(4, boxY + 9);
                var p2 = new Point(7, boxY + 12);
                var p3 = new Point(14, boxY + 5);

                g.DrawLine(pen, p1, p2);
                g.DrawLine(pen, p2, p3);
            }
            else
            {
                using var fillBrush = new SolidBrush(Color.White);
                g.FillPath(fillBrush, path);

                Color borderCol = _isHovered ? _boxCheckedColor : _boxBorderColor;
                using var borderPen = new Pen(borderCol, 1.5f);
                borderPen.Alignment = PenAlignment.Inset;
                g.DrawPath(borderPen, path);
            }

            // Draw text
            int textX = boxSize + 8;
            int textY = (Height - TextRenderer.MeasureText(_text, Font).Height) / 2;
            TextRenderer.DrawText(g, _text, Font, new Point(textX, textY), _textColor);
        }
    }
}
