using DocumentFormat.OpenXml.Bibliography;
using RauViet.classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace RauViet.ui
{
    public partial class AnnualLeaveBalance : Form
    {
        DataTable mEmployee_dt;
        // DataTable mShift_dt;
        public AnnualLeaveBalance()
        {
            InitializeComponent();

            month_cbb.Items.Clear();
            for (int m = 1; m <= 12; m++)
            {
                month_cbb.Items.Add(m);
            }

            month_cbb.SelectedItem = DateTime.Now.Month;
            year_tb.Text = DateTime.Now.Year.ToString();

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            status_lb.Text = "";

            dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGV.CellFormatting += DataGV_CellFormatting;
            dataGV.SelectionChanged += this.dataGV_CellClick;
            year_tb.KeyPress += Tb_KeyPress_OnlyNumber;

            load_btn.Click += Load_btn_Click;
            capphep_btn.Click += Capphep_btn_Click;

            dataGV.CellEndEdit += DataGV_CellEndEdit; ;
        }


        public async void ShowData()
        {
            
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            await Task.Delay(50);
            LoadingOverlay loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Show();

            try
            {
                int year = Convert.ToInt32(year_tb.Text);
                var employeeALBTask = SQLStore.Instance.GetAnnualLeaveBalanceAsync(year);

                await Task.WhenAll(employeeALBTask);
                mEmployee_dt = employeeALBTask.Result;

                DefineEmployeeGV();
            }
            catch (Exception ex)
            {
                status_lb.Text = "Thất bại.";
                status_lb.ForeColor = Color.Red;
            }
            finally
            {
                await Task.Delay(100);
                loadingOverlay.Hide();
            }
        }

        private void DefineEmployeeGV()
        {
            int year = Convert.ToInt32(year_tb.Text);

            int count = 0;
            mEmployee_dt.Columns["EmployeeCode"].SetOrdinal(count++);
            mEmployee_dt.Columns["FullName"].SetOrdinal(count++);
            mEmployee_dt.Columns["PositionName"].SetOrdinal(count++);
            mEmployee_dt.Columns["Year"].SetOrdinal(count++);

            dataGV.ReadOnly = false;
            
            mEmployee_dt.Columns["Month"].ReadOnly = false;
            mEmployee_dt.Columns["Year"].ReadOnly = false;

            dataGV.DataSource = mEmployee_dt;

            foreach (DataGridViewColumn col in dataGV.Columns)
            {
                col.ReadOnly = true;
            }
            dataGV.Columns["Month"].ReadOnly = false;

            dataGV.Columns["EmployeeCode"].HeaderText = "Mã Nhân Viên";
            dataGV.Columns["FullName"].HeaderText = "Tên Nhân Viên";
            dataGV.Columns["PositionName"].HeaderText = "Chức Vụ";
            dataGV.Columns["Month"].HeaderText = "Tháng Có phép";
            dataGV.Columns["Year"].HeaderText = "Năm Cấp phép";
            dataGV.Columns["LeaveCount"].HeaderText = "Đã Dùng";
            dataGV.Columns["RemainingLeave"].HeaderText = "Còn Lại";

            dataGV.Columns["EmployeeCode"].Width = 70;
            dataGV.Columns["FullName"].Width = 200;
            dataGV.Columns["Month"].Width = 200;
            dataGV.Columns["PositionName"].Width = 90;
            dataGV.Columns["LeaveCount"].Width = 70;
            dataGV.Columns["RemainingLeave"].Width = 70;

            if (dataGV.Rows.Count > 0)
            {
                dataGV.ClearSelection();
                dataGV.Rows[0].Selected = true;
            }
        }


        private void Tb_KeyPress_OnlyNumber(object sender, KeyPressEventArgs e)
        {
            TextBox tb = sender as TextBox;

            // Chỉ cho nhập số, phím điều khiển hoặc dấu chấm
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true; // chặn ký tự không hợp lệ
            }

            // Không cho nhập nhiều dấu chấm
            if (e.KeyChar == '.' && tb.Text.Contains("."))
            {
                e.Handled = true;
            }
        }

        private void Capphep_btn_Click(object sender, EventArgs e)
        {

            int year = Convert.ToInt32(year_tb.Text);
            int month = Convert.ToInt32(month_cbb.SelectedItem);

            DialogResult dialogResult = MessageBox.Show($"Cấp phép tháng {month}/{year} cho toàn bộ nhân viên ?", "Cập Nhật Tồn Phép", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult != DialogResult.Yes)
                return;
                

            foreach (DataRow dr in mEmployee_dt.Rows)
            {
                int yearInDR = 0;
                if (dr["Year"] != DBNull.Value && !string.IsNullOrWhiteSpace(dr["Year"].ToString()))
                    yearInDR = Convert.ToInt32(dr["Year"]);

                if (yearInDR != year && yearInDR != 0)
                {
                    MessageBox.Show("Đang Cập Nhật Khác Năm", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                List<int> monthList = new List<int>();
                string month_str = Convert.ToString(dr["Month"]);
                if (!string.IsNullOrEmpty(month_str))
                {
                    monthList = month_str.Split(',', (char)StringSplitOptions.RemoveEmptyEntries).Select(m => int.Parse(m.Trim())).ToList();
                }

                if (monthList.Contains(month)) continue;

                monthList.Add(month);
                monthList.Sort();

                dr["Year"] = year;
                dr["Month"] = string.Join(",", monthList);
                dr["RemainingLeave"] = monthList.Count - Convert.ToInt32(dr["LeaveCount"]);
            }

            SaveData(false);
        }

        private async void Load_btn_Click(object sender, EventArgs e)
        {
            await Task.Delay(50);
            LoadingOverlay loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Show();
            await Task.Delay(50);

            int year = Convert.ToInt32(year_tb.Text);
            var annualLeaveBalance = SQLStore.Instance.GetAnnualLeaveBalanceAsync(year);            
            await Task.WhenAll(annualLeaveBalance);
            mEmployee_dt = annualLeaveBalance.Result;

            DefineEmployeeGV();

            await Task.Delay(500);
            loadingOverlay.Hide();
        }

        private void dataGV_CellClick(object sender, EventArgs e)
        {
            if (dataGV.CurrentRow == null) return;
            int rowIndex = dataGV.CurrentRow.Index;
            if (rowIndex < 0)
                return;


            UpdateRightUI(rowIndex);
        }

        
        private void UpdateRightUI(int index)
        {
            var cells = dataGV.Rows[index].Cells;
            string employeeCode = cells["EmployeeCode"].Value.ToString();
        }

        public async void SaveData(bool ask = true)
        {
            List<(string EmployeeCode, int year, string month)> albData = new List<(string, int, string)>();

            foreach (DataRow dr in mEmployee_dt.Rows)
            {
                string employeeCode = Convert.ToString(dr["EmployeeCode"]);
                int year = Convert.ToInt32(dr["Year"]);
                string months = Convert.ToString(dr["Month"]);

                albData.Add((employeeCode, year, months));

            }

            try
            {
                Boolean iSuccess = await SQLManager.Instance.UpsertAnnualLeaveBalanceBatchAsync(albData);
                if (iSuccess)
                {
                    status_lb.Text = "Thành Công.";
                    status_lb.ForeColor = Color.Green;
                }
                else
                {
                    status_lb.Text = "";
                    MessageBox.Show("Thất Bại", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch
            {
                status_lb.Text = "";
                MessageBox.Show("Thất Bại", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private void DataGV_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (dataGV.Columns[e.ColumnIndex].Name == "Month")
            {
                e.CellStyle.BackColor = System.Drawing.Color.LightGray;
            }
        }

        private async void DataGV_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                var row = dataGV.Rows[e.RowIndex];

                List<(string EmployeeCode, int year, string month)> albData = new List<(string, int, string)>();
                string employeeCode = Convert.ToString(row.Cells["EmployeeCode"].Value);
                int year = Convert.ToInt32(row.Cells["Year"].Value);
                string months = Convert.ToString(row.Cells["Month"].Value);

                albData.Add((employeeCode, year, months));

                try
                {
                    Boolean iSuccess = await SQLManager.Instance.UpsertAnnualLeaveBalanceBatchAsync(albData);
                    if (iSuccess)
                    {
                        status_lb.Text = "Thành Công.";
                        status_lb.ForeColor = Color.Green;
                    }
                    else
                    {
                        status_lb.Text = "";
                        MessageBox.Show("Thất Bại", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch
                {
                    status_lb.Text = "";
                    MessageBox.Show("Thất Bại", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
