using DocumentFormat.OpenXml.Vml.Office;
using RauViet.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Color = System.Drawing.Color;

namespace RauViet.ui
{    
    public partial class EmployeeDeduction_ATT : Form
    {
        private Timer _monthYearDebounceTimer;
        private DataTable mEmployeeLeave_dt;
        private DataView mDeductionLogDV;
        private const string DeductionTypeCode = "ATT";
        private int curMonth, curYear;
        private object oldValue;
        public EmployeeDeduction_ATT()
        {
            InitializeComponent();
            this.KeyPreview = true;
            monthYearDtp.Format = DateTimePickerFormat.Custom;
            monthYearDtp.CustomFormat = "MM/yyyy";
            monthYearDtp.ShowUpDown = true;
            monthYearDtp.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            dataGV.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dataGV.MultiSelect = false;

            employeeDeductionGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            employeeDeductionGV.MultiSelect = false;

            status_lb.Text = "";

            dataGV.SelectionChanged += this.dataGV_CellClick;
            this.KeyDown += EmployeeDeduction_ATT_KeyDown;
            search_tb.TextChanged += Search_tb_TextChanged;
            this.Load += OvertimeAttendace_Load;

            dataGV.CellFormatting += dataGV_CellFormatting;
            dataGV.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dataGV_EditingControlShowing);
            dataGV.CellBeginEdit += dataGV_CellBeginEdit;
            dataGV.CellEndEdit += dataGV_CellEndEdit;
        }

        private void OvertimeAttendace_Load(object sender, EventArgs e)
        {
            _monthYearDebounceTimer = new Timer();
            _monthYearDebounceTimer.Interval = 500;
            _monthYearDebounceTimer.Tick += MonthYearDebounceTimer_Tick;
        }

        private void EmployeeDeduction_ATT_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                int year = monthYearDtp.Value.Year;

                SQLStore_QLNS.Instance.removeDeduction(year);
                ShowData();
            }
        }

        public async void ShowData()
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            await Task.Delay(50);
            LoadingOverlay loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Show();
            await Task.Delay(200);
            try
            {
                monthYearDtp.ValueChanged -= monthYearDtp_ValueChanged;

                int month = monthYearDtp.Value.Month;
                int year = monthYearDtp.Value.Year;
                var employeeTask = SQLManager_QLNS.Instance.GetActiveEmployee_DeductionATT_Async(month, year);
                var employeeLeaveAsync = SQLManager_QLNS.Instance.GetEmployeeLeave_PT_KP_Async(month, year);
                var EmployeeDeductionLogTask = SQLStore_QLNS.Instance.GetEmployeeDeductionLogAsync(month, year, DeductionTypeCode);
                await Task.WhenAll(employeeTask, employeeLeaveAsync, EmployeeDeductionLogTask);
                DataTable employee_dt = employeeTask.Result;
                mEmployeeLeave_dt = employeeLeaveAsync.Result;
                mDeductionLogDV = new DataView(EmployeeDeductionLogTask.Result);
                curMonth = month;
                curYear = year;

                monthYearLabel.Text = $"Tháng {month}/{year}";
                
                foreach (DataRow dr in employee_dt.Rows)
                {
                    if (dr["PositionName"] == DBNull.Value) dr["PositionName"] = "";
                    if (dr["ContractTypeName"] == DBNull.Value) dr["ContractTypeName"] = "";
                }

                employee_dt.Columns.Add(new DataColumn("TotalOffDay", typeof(int)));
                employee_dt.Columns.Add("EmployessName_NoSign", typeof(string));
                Dictionary<string, int> grouped = mEmployeeLeave_dt.AsEnumerable().GroupBy(r => r.Field<string>("EmployeeCode")).ToDictionary(g => g.Key, g => g.Count());
                foreach (DataRow dr in employee_dt.Rows)
                {
                    string empCode = dr["EmployeeCode"].ToString();
                    dr["TotalOffDay"] = grouped.ContainsKey(empCode) ? grouped[empCode] : 0;
                    dr["EmployessName_NoSign"] =Utils.RemoveVietnameseSigns($"{dr["EmployeeCode"]} {dr["FullName"]}");
                }

                DataRow[] rowsToDelete = employee_dt.Select("TotalOffDay = 0");
                foreach (DataRow row in rowsToDelete)
                    row.Delete();
                employee_dt.AcceptChanges();

                employeeDeductionGV.DataSource = mEmployeeLeave_dt;

                employee_dt.Columns["DeductionAmount"].ReadOnly = false;
                dataGV.AutoGenerateColumns = true;
                dataGV.DataSource = employee_dt;
                log_GV.DataSource = mDeductionLogDV;

                employeeDeductionGV.Columns["EmployeeCode"].Visible = false;
                dataGV.Columns["EmployessName_NoSign"].Visible = false;

                int count = 0;
                mEmployeeLeave_dt.Columns["DateOff"].SetOrdinal(count++);
                mEmployeeLeave_dt.Columns["LeaveTypeName"].SetOrdinal(count++);
                mEmployeeLeave_dt.Columns["LeaveHours"].SetOrdinal(count++);
                mEmployeeLeave_dt.Columns["Note"].SetOrdinal(count++);

                Utils.HideColumns(log_GV, new[] { "LogID", "EmployeeCode", "DeductionTypeCode" });

                employeeDeductionGV.Columns["DateOff"].HeaderText = "Ngày Nghỉ";
                employeeDeductionGV.Columns["LeaveTypeName"].HeaderText = "Loại Nghỉ Phép";
                employeeDeductionGV.Columns["LeaveHours"].HeaderText = "Số Giờ Vắng";
                employeeDeductionGV.Columns["Note"].HeaderText = "Ghi Chú";

                bool isLock = await SQLStore_QLNS.Instance.IsSalaryLockAsync(month, year);
                dataGV.ReadOnly = isLock;
                dataGV.Columns["FullName"].ReadOnly = true;
                dataGV.Columns["EmployeeCode"].ReadOnly = true;
                dataGV.Columns["PositionName"].ReadOnly = true;
                dataGV.Columns["ContractTypeName"].ReadOnly = true;
                dataGV.Columns["AllowanceAmount"].ReadOnly = true;
                dataGV.Columns["DeductionAmount"].ReadOnly = isLock;
                dataGV.Columns["TotalOffDay"].ReadOnly = true;

                dataGV.Columns["FullName"].HeaderText = "Tên Nhân Viên";
                dataGV.Columns["EmployeeCode"].HeaderText = "Mã NV";
                dataGV.Columns["PositionName"].HeaderText = "Chức Vụ";
                dataGV.Columns["ContractTypeName"].HeaderText = "Loại Hợp Đồng";
                dataGV.Columns["AllowanceAmount"].HeaderText = "PC Chuyên Cần";
                dataGV.Columns["DeductionAmount"].HeaderText = "Trừ Chuyên Cần";
                dataGV.Columns["TotalOffDay"].HeaderText = "Số Ngày Nghỉ";

                dataGV.Columns["EmployeeCode"].Width = 50;
                dataGV.Columns["FullName"].Width = 150;
                dataGV.Columns["PositionName"].Width = 100;
                dataGV.Columns["ContractTypeName"].Width = 80;
                dataGV.Columns["AllowanceAmount"].Width = 70;
                dataGV.Columns["DeductionAmount"].Width = 70;
                dataGV.Columns["TotalOffDay"].Width = 50;

                employeeDeductionGV.Columns["DateOff"].Width = 70;
                employeeDeductionGV.Columns["LeaveTypeName"].Width = 100;
                employeeDeductionGV.Columns["LeaveHours"].Width = 60;
                employeeDeductionGV.Columns["Note"].Width = 150;

                if (dataGV.Rows.Count > 0)
                {
                    dataGV.ClearSelection();
                    dataGV.Rows[0].Selected = true;
                }


                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                employeeDeductionGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                log_GV.Columns["CreateAt"].Width = 120;
                log_GV.Columns["ActionBy"].Width = 150;
                log_GV.Columns["Description"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                log_GV.Columns["DeductionDate"].Width = 80;
                log_GV.Columns["Amount"].Width = 80;
                log_GV.Columns["CreateAt"].HeaderText = "Thời điểm thay đổi";
                log_GV.Columns["ACtionBy"].HeaderText = "Người thay đổi";
                log_GV.Columns["Description"].HeaderText = "Hành động";
                log_GV.Columns["DeductionDate"].HeaderText = "Ngày";
                log_GV.Columns["Amount"].HeaderText = "Số Tiền";

                log_GV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

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
                private void dataGV_CellClick(object sender, EventArgs e)
        {
            if (dataGV.CurrentRow == null) return;
            int rowIndex = dataGV.CurrentRow.Index;
            if (rowIndex < 0)
                return;


            UpdateEmployeeDeductionUI(rowIndex);
        }

        private void UpdateEmployeeDeductionUI(int index)
        {
            employeeDeductionGV.ClearSelection();

            var cells = dataGV.Rows[index].Cells;
            string employeeCode = Convert.ToString(cells["EmployeeCode"].Value);

            DataView dv = new DataView(mEmployeeLeave_dt);
            dv.RowFilter = $"EmployeeCode = '{employeeCode}'";

            employeeDeductionGV.DataSource = dv;

            mDeductionLogDV.RowFilter = $"EmployeeCode = '{employeeCode}'";
        }

        private async void updateData(string employeeCode, DateTime deductionDate, int deductionAmount)
        {
            try
            {
                bool isScussess = await SQLManager_QLNS.Instance.UpsertEmployeeDeductionAsync(employeeCode, DeductionTypeCode, deductionDate, deductionAmount, "");

                if (isScussess == true)
                {
                    _ = SQLManager_QLNS.Instance.InsertEmployeeDeductionLogAsync(employeeCode, DeductionTypeCode, $"Edit: Success", deductionDate.Date, deductionAmount, "Trừ chuyên cần");                    
                }
                else
                {
                    MessageBox.Show("Thất bại rồi ! Huhu", "Thong Báo", MessageBoxButtons.OK);
                    _ = SQLManager_QLNS.Instance.InsertEmployeeDeductionLogAsync(employeeCode, DeductionTypeCode, $"Edit: Fail", deductionDate.Date, deductionAmount, "Trừ chuyên cần");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Thất bại rồi ! Huhu: \n" + ex.Message, "Thong Báo", MessageBoxButtons.OK);
                _ = SQLManager_QLNS.Instance.InsertEmployeeDeductionLogAsync(employeeCode, DeductionTypeCode, $"Edit Fail Exception: " + ex.Message, deductionDate.Date, deductionAmount, "Trừ chuyên cần");
            }
        }

        private void Search_tb_TextChanged(object sender, EventArgs e)
        {
            string keyword = Utils.RemoveVietnameseSigns(search_tb.Text.Trim().ToLower())
                     .Replace("'", "''"); // tránh lỗi cú pháp '

            DataTable dt = dataGV.DataSource as DataTable;
            if (dt == null) return;

            DataView dv = dt.DefaultView;
            dv.RowFilter = $"[EmployessName_NoSign] LIKE '%{keyword}%'";
        }

        private void monthYearDtp_ValueChanged(object sender, EventArgs e)
        {
            // Mỗi lần thay đổi thì reset timer
            _monthYearDebounceTimer.Stop();
            _monthYearDebounceTimer.Start();
        }

        private void MonthYearDebounceTimer_Tick(object sender, EventArgs e)
        {
            _monthYearDebounceTimer.Stop();
            HandleMonthYearChanged();
        }

        private async void HandleMonthYearChanged()
        {
            ShowData();
        }

        private async void dataGV_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            
            if (dataGV.Columns[e.ColumnIndex].Name == "DeductionAmount")
            {
                e.CellStyle.BackColor = System.Drawing.Color.LightGray;
            }            
        }

        private void dataGV_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            System.Windows.Forms.TextBox tb = e.Control as System.Windows.Forms.TextBox;
            if (tb == null) return;

            tb.KeyPress -= Tb_KeyPress_OnlyNumber;

            var colName = dataGV.CurrentCell.OwningColumn.Name;

            if (colName == "DeductionAmount")
            {
                tb.KeyPress += Tb_KeyPress_OnlyNumber;
            }
        }

        private async void dataGV_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            oldValue = dataGV.CurrentCell?.Value;
            bool isLock = await SQLStore_QLNS.Instance.IsSalaryLockAsync(curMonth, curYear);
            if (isLock)
                e.Cancel = true;

            status_lb.Text = "";
        }

        private async void dataGV_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                var columnName = dataGV.Columns[e.ColumnIndex].Name;
                object newValue = dataGV.CurrentCell?.Value;//.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString() ?? "";

                if (object.Equals(oldValue, newValue)) return;

                int month = monthYearDtp.Value.Month;
                int year = monthYearDtp.Value.Year;
                DateTime deductionDate = new DateTime(year, month, 15);

                string employeeCode = Convert.ToString(dataGV.CurrentRow.Cells["EmployeeCode"].Value);
                int amount = Convert.ToInt32(newValue);

                if (month != curMonth || year != curYear)
                {
                    MessageBox.Show("Tháng, Năm có vẫn đề.", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                updateData(employeeCode, deductionDate, amount);
            }
        }
    }
}
