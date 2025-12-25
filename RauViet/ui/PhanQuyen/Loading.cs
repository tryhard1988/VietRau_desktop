using RauViet.classes;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            //var task1 = SQLManager.Instance.AutoDeleteLoginHistoryAsync();
            //var task2 = SQLManager.Instance.AutoDeleteExportCodeLogAsync();
            //var task3 = SQLManager.Instance.AutoDeleteOrderLogAsync();
            //var task4 = SQLManager.Instance.AutoDeleteOrderPackingLogAsync();
            //var task5 = SQLManager.Instance.AutoDeleteDo47LogAsync();
            //await Task.WhenAll(task1, task2, task3, task4, task5);
            var task1 = SQLManager_Kho.Instance.AutoUpdateCompleteExportCodeAsync();
            var task2 = SQLManager_QLNS.Instance.AutoUpsertAnnualLeaveMonthListAsync();
            await Task.WhenAll(task1, task2);

            await SQLStore_QLNS.Instance.preload();
            await SQLStore_Kho.Instance.preload();
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
