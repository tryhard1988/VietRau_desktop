using RauViet.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RauViet.ui
{
    public partial class AnnualLeaveBalance : Form
    {
        private Timer _monthYearDebounceTimer;
        DataTable mEmployee_dt;
        int currentYear;
        // DataTable mShift_dt;
        public AnnualLeaveBalance()
        {
            InitializeComponent();
            this.KeyPreview = true;
            monthYearDtp.Format = DateTimePickerFormat.Custom;
            monthYearDtp.CustomFormat = "MM/yyyy";
            monthYearDtp.ShowUpDown = true;
            monthYearDtp.Value = DateTime.Now;

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            status_lb.Text = "";

            dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGV.CellFormatting += DataGV_CellFormatting;

            capphep_btn.Click += Capphep_btn_Click;

            dataGV.CellEndEdit += DataGV_CellEndEdit;
            this.KeyDown += AnnualLeaveBalance_KeyDown;
            this.Load += OvertimeAttendace_Load;
        }

        private void OvertimeAttendace_Load(object sender, EventArgs e)
        {
            _monthYearDebounceTimer = new Timer();
            _monthYearDebounceTimer.Interval = 500;
            _monthYearDebounceTimer.Tick += MonthYearDebounceTimer_Tick;
        }

        private void AnnualLeaveBalance_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                int year = monthYearDtp.Value.Year;
                SQLStore_QLNS.Instance.removeAnnualLeaveBalance(year);
                ShowData();
            }
        }

        public async void ShowData()
        {
            await Task.Delay(50);
            LoadingOverlay loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Show();
            await Task.Delay(200);

            try
            {
                monthYearDtp.ValueChanged -= monthYearDtp_ValueChanged;
                int year = monthYearDtp.Value.Year;
                var employeeALBTask = SQLStore_QLNS.Instance.GetAnnualLeaveBalanceAsync(year);
                currentYear = year;
                await Task.WhenAll(employeeALBTask);
                mEmployee_dt = employeeALBTask.Result;

                DefineEmployeeGV();
                monthYearDtp.ValueChanged += monthYearDtp_ValueChanged;
            }
            catch
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
            int year = monthYearDtp.Value.Year;

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

            int year = monthYearDtp.Value.Year;
            int month = monthYearDtp.Value.Month;

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
                Boolean iSuccess = await SQLManager_QLNS.Instance.UpsertAnnualLeaveBalanceBatchAsync(albData);
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
                    Boolean iSuccess = await SQLManager_QLNS.Instance.UpsertAnnualLeaveBalanceBatchAsync(albData);
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

        private void monthYearDtp_ValueChanged(object sender, EventArgs e)
        {
            // Mỗi lần thay đổi thì reset timer
            _monthYearDebounceTimer.Stop();

            int year = monthYearDtp.Value.Year;
            if (currentYear != year)
            _monthYearDebounceTimer.Start();
        }

        private void MonthYearDebounceTimer_Tick(object sender, EventArgs e)
        {
            _monthYearDebounceTimer.Stop();
            HandleMonthYearChanged();
        }

        private async void HandleMonthYearChanged()
        {
            await Task.Delay(50);
            LoadingOverlay loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Show();
            await Task.Delay(200);

            int year = monthYearDtp.Value.Year;
            var annualLeaveBalance = SQLStore_QLNS.Instance.GetAnnualLeaveBalanceAsync(year);
            await Task.WhenAll(annualLeaveBalance);
            mEmployee_dt = annualLeaveBalance.Result;
            currentYear = year;
            DefineEmployeeGV();

            await Task.Delay(100);
            loadingOverlay.Hide();
        }
    }
}
