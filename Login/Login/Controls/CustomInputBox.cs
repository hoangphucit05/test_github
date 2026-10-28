using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Login.Controls
{
    [ToolboxItem(true)]
    [DefaultProperty(nameof(PlaceholderText))]
    [DefaultEvent(nameof(TextChangedEvent))]
    public class CustomInputBox : UserControl
    {
        private readonly TextBox _textBox;
        private readonly PictureBox? _eyeButton;
        private Image? _leftIcon = null;
        private Image? _eyeOpenIcon = null;
        private Image? _eyeSlashIcon = null;

        private int _borderRadius = 8;
        private Color _borderColor = Color.FromArgb(209, 213, 219);
        private Color _focusedBorderColor = Color.FromArgb(161, 82, 26);
        private bool _isFocused = false;
        private bool _isPassword = false;
        private bool _showPasswordToggle = false;
        private bool _passwordVisible = false;

        public event EventHandler? TextChangedEvent;

        public string InputText
        {
            get => _textBox.Text;
            set => _textBox.Text = value;
        }

        public string PlaceholderText
        {
            get => _textBox.PlaceholderText;
            set => _textBox.PlaceholderText = value;
        }

        public Image? LeftIcon
        {
            get => _leftIcon;
            set { _leftIcon = value; Invalidate(); }
        }

        public bool IsPassword
        {
            get => _isPassword;
            set
            {
                _isPassword = value;
                UpdatePasswordChar();
            }
        }

        public bool ShowPasswordToggle
        {
            get => _showPasswordToggle;
            set
            {
                _showPasswordToggle = value;
                if (_eyeButton != null)
                {
                    _eyeButton.Visible = value;
                }
                AdjustLayout();
            }
        }

        public CustomInputBox()
        {
            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);
            DoubleBuffered = true;
            BackColor = Color.White;
            _textBox = new TextBox
            {
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 11f, FontStyle.Regular),
                ForeColor = Color.FromArgb(31, 41, 55),
                BackColor = Color.White,
                Location = new Point(46, 13),
                Size = new Size(295, 24)
            };

            _textBox.GotFocus += (s, e) => { _isFocused = true; Invalidate(); };
            _textBox.LostFocus += (s, e) => { _isFocused = false; Invalidate(); };
            _textBox.TextChanged += (s, e) => TextChangedEvent?.Invoke(this, EventArgs.Empty);

            // Eye toggle button
            _eyeButton = new PictureBox
            {
                Size = new Size(24, 24),
                SizeMode = PictureBoxSizeMode.CenterImage,
                Cursor = Cursors.Hand,
                Visible = false,
                BackColor = Color.Transparent
            };
            _eyeButton.Click += EyeButton_Click;

            Controls.Add(_textBox);
            Controls.Add(_eyeButton);

            Size = new Size(390, 48);

            Click += (s, e) => _textBox.Focus();
        }

        public void SetEyeIcons(Image? openIcon, Image? slashIcon)
        {
            _eyeOpenIcon = openIcon;
            _eyeSlashIcon = slashIcon;
            UpdateEyeIcon();
        }

        private void EyeButton_Click(object? sender, EventArgs e)
        {
            _passwordVisible = !_passwordVisible;
            UpdatePasswordChar();
            UpdateEyeIcon();
            _textBox.Focus();
            _textBox.SelectionStart = _textBox.Text.Length;
        }

        private void UpdatePasswordChar()
        {
            if (_isPassword)
            {
                _textBox.PasswordChar = _passwordVisible ? '\0' : '●';
            }
            else
            {
                _textBox.PasswordChar = '\0';
            }
        }

        private void UpdateEyeIcon()
        {
            if (_eyeButton == null) return;
            _eyeButton.Image = _passwordVisible ? _eyeSlashIcon : _eyeOpenIcon;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            AdjustLayout();
        }

        private void AdjustLayout()
        {
            if (_textBox == null) return;

            int leftOffset = _leftIcon != null ? 46 : 14;
            int rightOffset = (_showPasswordToggle && _eyeButton != null) ? 46 : 14;

            int tbY = (Height - _textBox.PreferredHeight) / 2;
            int tbWidth = Math.Max(10, Width - leftOffset - rightOffset);

            _textBox.Location = new Point(leftOffset, tbY);
            _textBox.Width = tbWidth;

            if (_eyeButton != null)
            {
                _eyeButton.Location = new Point(Width - 38, (Height - 24) / 2);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            using var path = RoundedPanel.CreateRoundedRectangle(rect, _borderRadius);

            // Fill
            using (var brush = new SolidBrush(BackColor))
            {
                g.FillPath(brush, path);
            }

            // Draw border
            Color currentBorder = _isFocused ? _focusedBorderColor : _borderColor;
            int currentWidth = _isFocused ? 2 : 1;
            using (var pen = new Pen(currentBorder, currentWidth))
            {
                pen.Alignment = PenAlignment.Inset;
                g.DrawPath(pen, path);
            }

            // Draw left icon
            if (_leftIcon != null)
            {
                int iconSize = 20;
                int iconX = 14;
                int iconY = (Height - iconSize) / 2;
                g.DrawImage(_leftIcon, new Rectangle(iconX, iconY, iconSize, iconSize));
            }
        }
    }
}
