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

        public async Task Show()
        {
            if (_owner == null || _owner.IsDisposed)
                return;

            if (_owner.InvokeRequired)
            {
                _owner.Invoke(new Action(async () => await Show()));
                return;
            }

            if (_overlayForm != null && !_overlayForm.IsDisposed)
                return;

            await Task.Yield(); // thay cho Delay

            _overlayForm = new Form
            {
                BackColor = Color.Black,
                Opacity = 0.7f,
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.Manual,
                ShowInTaskbar = false,
                Owner = _owner
            };

            if (!_owner.IsHandleCreated)
                _owner.CreateControl();

            _overlayForm.Bounds = _owner.RectangleToScreen(_owner.ClientRectangle);

            var label = new Label
            {
                AutoSize = true,
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Text = Message
            };

            _overlayForm.Controls.Add(label);
            _overlayForm.Show();

            label.Location = new Point(
                (_overlayForm.ClientSize.Width - label.PreferredWidth) / 2,
                (_overlayForm.ClientSize.Height - label.Height) / 2
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
