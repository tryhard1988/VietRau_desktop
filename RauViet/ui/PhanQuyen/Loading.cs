using RauViet.classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace RauViet.ui.PhanQuyen
{
    public partial class Loading : Form
    {
        public Loading()
        {
            InitializeComponent();

            

            this.FormBorderStyle = FormBorderStyle.FixedDialog; // hoặc FixedSingle
            this.MaximizeBox = false; // tắt nút maximize
            this.MinimizeBox = false; // tắt nút minimize nếu muốn

            // 2️⃣ Luôn hiển thị trên cùng
            this.TopMost = true;
            this.StartPosition = FormStartPosition.CenterScreen;

            this.Load += Loading_Load;
        }

        private async void Loading_Load(object sender, EventArgs e)
        {
            await Task.Delay(200);
            _ = SQLManager.Instance.AutoDeleteLoginHistoryAsync();
            _ = SQLManager.Instance.AutoUpdateCompleteExportCodeAsync();
            await SQLManager.Instance.AutoUpsertAnnualLeaveMonthListAsync();
            await SQLStore.Instance.preload_NhanSu();
            await SQLStore.Instance.preload_Suong();
            await Task.Delay(200);
            try
            {
                this.Hide();
                using (var form = new FormManager())
                {
                    form.ShowDialog();
                }
                this.Close();
            }
            catch { }
        }
    }
}
