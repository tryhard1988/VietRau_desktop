using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RauViet.ui
{
    public class PickDateDialog : Form
    {
        public DateTime SelectedDate { get; private set; }

        private DateTimePicker dtp;
        private Button btnOK;
        private Button btnCancel;

        public PickDateDialog(DateTime? defaultDate = null)
        {
            this.Text = "Chọn Ngày Đóng Gói";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.ClientSize = new Size(300, 130);
            this.MinimizeBox = false;
            this.MaximizeBox = false;

            Label lbl = new Label();
            lbl.Text = "Ngày đóng gói:";
            lbl.SetBounds(10, 15, 120, 20);

            dtp = new DateTimePicker();
            dtp.Format = DateTimePickerFormat.Custom;
            dtp.CustomFormat = "dd/MM/yyyy";
            dtp.Value = defaultDate ?? DateTime.Now;
            dtp.SetBounds(130, 10, 150, 25);

            btnOK = new Button();
            btnOK.Text = "OK";
            btnOK.SetBounds(70, 60, 70, 30);
            btnOK.DialogResult = DialogResult.OK;

            btnCancel = new Button();
            btnCancel.Text = "Hủy";
            btnCancel.SetBounds(160, 60, 70, 30);
            btnCancel.DialogResult = DialogResult.Cancel;

            this.Controls.Add(lbl);
            this.Controls.Add(dtp);
            this.Controls.Add(btnOK);
            this.Controls.Add(btnCancel);

            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            dtp.Focus();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (this.DialogResult == DialogResult.OK)
            {
                SelectedDate = dtp.Value;
            }
            base.OnFormClosing(e);
        }
    }

}
