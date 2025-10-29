using System;
using System.Drawing;
using System.Windows.Forms;

namespace RauViet.classes
{
    

    public static class ToolTipHelper
    {
        private static ToolTip sharedToolTip;

        static ToolTipHelper()
        {
            // Chỉ tạo 1 lần duy nhất cho toàn ứng dụng
            sharedToolTip = new ToolTip
            {
                AutoPopDelay = 5000,
                InitialDelay = 300,
                ReshowDelay = 200,
                ShowAlways = true,
                OwnerDraw = true
            };

            // Custom giao diện tooltip
            sharedToolTip.Draw += (s, eArgs) =>
            {
                eArgs.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(40, 40, 40)), eArgs.Bounds);
                eArgs.Graphics.DrawRectangle(Pens.Gray, new Rectangle(Point.Empty, eArgs.Bounds.Size - new Size(1, 1)));

                using (Font f = new Font("Segoe UI", 9, FontStyle.Bold))
                {
                    eArgs.Graphics.DrawString(eArgs.ToolTipText, f, Brushes.White, new PointF(5, 3));
                }
            };
        }

        /// <summary>
        /// Gắn tooltip cho 1 control bất kỳ
        /// </summary>
        /// <param name="control">Control cần hiển thị tooltip</param>
        /// <param name="message">Nội dung tooltip</param>
        public static void SetToolTip(Control control, string message)
        {
            if (control == null || string.IsNullOrEmpty(message))
                return;

            sharedToolTip.SetToolTip(control, message);
        }
    }

}
