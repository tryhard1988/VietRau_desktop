using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RauViet.classes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace RauViet.ui
{
    public partial class HistoryLogIn : Form
    {
        public HistoryLogIn()
        {
            InitializeComponent();
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;
        }

        public async void ShowData()
        {
            try
            {
                // Chạy truy vấn trên thread riêng
                var postionTask = SQLManager.Instance.GetHistoryLoginAsync();

                await Task.WhenAll(postionTask);
                DataTable postion_dt = postionTask.Result;


                dataGV.DataSource = postion_dt;

                dataGV.Columns["Id"].HeaderText = "Tên Vị Trí";
                dataGV.Columns["UserName"].HeaderText = "Diễn Giải";
                dataGV.Columns["MachineInfo"].HeaderText = "Máy Đăng Nhập";
                dataGV.Columns["LoginTime"].HeaderText = "Thời điểm đăng nhập";

                dataGV.Columns["UserName"].Width = 100;
                dataGV.Columns["MachineInfo"].Width = 300;
                dataGV.Columns["LoginTime"].Width = 150;

                //dataGV.Columns["PositionID"].Visible = false;
                //dataGV.Columns.Remove("CreatedAt");

                //dataGV.Columns["PositionCode"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                //dataGV.Columns["PositionName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                //dataGV.Columns["Description"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                //dataGV.Columns["IsActive"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;


                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            catch
            {
            }
            finally
            {
            }
        }
    }
}
