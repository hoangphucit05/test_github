using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace Login.Controls
{
    public class DividerLabel : Control
    {
        private string _text = "hoặc";
        private Color _lineColor = Color.FromArgb(229, 231, 235);
        private Color _textColor = Color.FromArgb(156, 163, 175);

        public new string Text
        {
            get => _text;
            set { _text = value; Invalidate(); }
        }

        public Color LineColor
        {
            get => _lineColor;
            set { _lineColor = value; Invalidate(); }
        }

        public Color TextColor
        {
            get => _textColor;
            set { _textColor = value; Invalidate(); }
        }

        public DividerLabel()
        {
            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.SupportsTransparentBackColor, true);
            DoubleBuffered = true;
            BackColor = Color.Transparent;
            Font = new Font("Segoe UI", 9f, FontStyle.Regular);
            Size = new Size(390, 24);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            Size textSize = TextRenderer.MeasureText(_text, Font);
            int textX = (Width - textSize.Width) / 2;
            int textY = (Height - textSize.Height) / 2;
            int lineY = Height / 2;

            using (var pen = new Pen(_lineColor, 1))
            {
                // Left line
                if (textX - 12 > 0)
                {
                    g.DrawLine(pen, 0, lineY, textX - 12, lineY);
                }
                // Right line
                if (textX + textSize.Width + 12 < Width)
                {
                    g.DrawLine(pen, textX + textSize.Width + 12, lineY, Width, lineY);
                }
            }

            TextRenderer.DrawText(g, _text, Font, new Point(textX, textY), _textColor);
        }
    }
}
