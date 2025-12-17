using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RauViet.classes
{
    public class LoadingOverlay
    {
        private readonly Form _owner;
        private Form _overlayForm;

        public string Message { get; set; } = "Đang tải dữ liệu, vui lòng chờ...";

        public LoadingOverlay(Form owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        public async void Show()
        {
            if (_overlayForm != null && !_overlayForm.IsDisposed)
                return;

            await Task.Delay(50); // đợi form chính render xong

            _overlayForm = new Form
            {
                BackColor = Color.Black,
                Opacity = 0.7f,
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.Manual,
                ShowInTaskbar = false,
                //TopMost = true,
                Owner = _owner
            };

            _overlayForm.Bounds = _owner.RectangleToScreen(_owner.ClientRectangle);

            var contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };
            _overlayForm.Controls.Add(contentPanel);

            var loadingLabel = new Label
            {
                AutoSize = true,
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Text = Message
            };

            contentPanel.Controls.Add(loadingLabel);
            _overlayForm.Show();
            _overlayForm.BringToFront();
            _overlayForm.Refresh();

            loadingLabel.Location = new Point(
                (_overlayForm.ClientSize.Width - loadingLabel.PreferredWidth) / 2,
                (_overlayForm.ClientSize.Height - loadingLabel.Height) / 2
            );

        }

        public void Hide()
        {
            if (_overlayForm != null && !_overlayForm.IsDisposed)
            {
                _overlayForm.Close();
                _overlayForm.Dispose();
                _overlayForm = null;
            }
        }
    }

}
