using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using RauViet.classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO.Packaging;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace RauViet.ui
{
    public partial class LeaveAttendance : Form
    {
        System.Data.DataTable mLeaveAttendance_dt, mEmployee_dt, mLeaveType;
        Dictionary<string, (string PositionCode, string ContractTypeCode)> employeeDict;
        bool isNewState = false;
        int curYear = -1;
        // DataTable mShift_dt;
        public LeaveAttendance()
        {
            InitializeComponent();

            Utils.SetTabStopRecursive(this, false);

            int countTab = 0;
            leaveType_cbb.TabIndex = countTab++; leaveType_cbb.TabStop = true;
            dateOffStart_dtp.TabIndex = countTab++; dateOffStart_dtp.TabStop = true;
            hourLeave_tb.TabIndex = countTab++; hourLeave_tb.TabStop = true;
            note_tb.TabIndex = countTab++; note_tb.TabStop = true;
            LuuThayDoiBtn.TabIndex = countTab++; LuuThayDoiBtn.TabStop = true;

            year_tb.Text = DateTime.Now.Year.ToString();

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            attendanceGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            attendanceGV.MultiSelect = false;

            status_lb.Text = "";

            dateOffEnd_dtp.Format = DateTimePickerFormat.Custom;
            dateOffEnd_dtp.CustomFormat = "dd/MM/yyyy";
            dateOffStart_dtp.Format = DateTimePickerFormat.Custom;
            dateOffStart_dtp.CustomFormat = "dd/MM/yyyy";

            LuuThayDoiBtn.Click += saveBtn_Click;
            dataGV.SelectionChanged += this.dataGV_CellClick;
            

            loadAttandance_btn.Click += LoadLeaveAttendance_btn_Click;
            newBtn.Click += NewBtn_Click;
            delete_btn.Click += Delete_btn_Click;
            hourLeave_tb.KeyPress += Tb_KeyPress_OnlyNumber;
            year_tb.KeyPress += Tb_KeyPress_OnlyNumber;
            dateOffStart_dtp.ValueChanged += DateOffStart_dtp_ValueChanged;
            dateOffEnd_dtp.ValueChanged += DateOffEnd_dtp_ValueChanged;

            edit_btn.Click += Edit_btn_Click;
            readOnly_btn.Click += ReadOnly_btn_Click;
            ReadOnly_btn_Click(null, null);
        }


        public async void ShowData()
        {

            await Task.Delay(50);
            LoadingOverlay loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Show();

            try
            {
                int year = Convert.ToInt32(year_tb.Text);
                List<string> leavecodeParam =new List<string>();
                leavecodeParam.Add("NL_1");
                // Chạy truy vấn trên thread riêng
                string[] keepColumns = { "EmployeeCode", "FullName", "PositionName", "ContractTypeName", };
                var employeesTask = SQLStore.Instance.GetEmployeesAsync(keepColumns);
                var leaveTypeTask = SQLStore.Instance.GetLeaveTypeWithoutAsync(leavecodeParam);
                var employeeRemainingLeaveTask = SQLStore.Instance.GetAnnualLeaveBalanceAsync(year);
                var leaveAttendanceTask = SQLStore.Instance.GetLeaveAttendancesAsyn(year);
                

                await Task.WhenAll(employeeRemainingLeaveTask, leaveAttendanceTask, leaveTypeTask, employeesTask);
                mEmployee_dt = employeeRemainingLeaveTask.Result;
                mLeaveAttendance_dt = leaveAttendanceTask.Result;
                mLeaveType = leaveTypeTask.Result;

                curYear = year;

                leaveType_cbb.DataSource = mLeaveType;
                leaveType_cbb.DisplayMember = "LeaveTypeName";
                leaveType_cbb.ValueMember = "LeaveTypeCode";

                int count = 0;
                mEmployee_dt.Columns["EmployeeCode"].SetOrdinal(count++);
                mEmployee_dt.Columns["FullName"].SetOrdinal(count++);
                mEmployee_dt.Columns["PositionName"].SetOrdinal(count++);
                mEmployee_dt.Columns["RemainingLeave"].SetOrdinal(count++);

                dataGV.DataSource = mEmployee_dt;
                dataGV.Columns["EmployeeCode"].HeaderText = "Mã Nhân Viên";
                dataGV.Columns["FullName"].HeaderText = "Tên Nhân Viên";
              //  dataGV.Columns["ContractTypeName"].HeaderText = "Loại H.Đồng";
                dataGV.Columns["PositionName"].HeaderText = "Chức Vụ";
                dataGV.Columns["RemainingLeave"].HeaderText = "Phép Năm Còn";

                dataGV.Columns["Month"].Visible = false;
                dataGV.Columns["Year"].Visible = false;
                dataGV.Columns["LeaveCount"].Visible = false;

                dataGV.Columns["EmployeeCode"].Width = 50;
                dataGV.Columns["FullName"].Width = 160;
              //  dataGV.Columns["ContractTypeName"].Width = 70;
                dataGV.Columns["PositionName"].Width = 70;

                dataGV.Width = 500;

                if (dataGV.Rows.Count > 0)
                {
                    dataGV.ClearSelection();
                    dataGV.Rows[0].Selected = true;
                }

                loadLeaveAttendance();

                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
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

        private async void loadLeaveAttendance()
        {
            await Task.Delay(50);
            LoadingOverlay loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Show();

            attendanceGV.SelectionChanged -= this.attendanceGV_CellClick;

            int count = 0;
            mLeaveAttendance_dt.Columns["DayOfWeek"].SetOrdinal(count++);
            mLeaveAttendance_dt.Columns["DateOff"].SetOrdinal(count++);
            mLeaveAttendance_dt.Columns["LeaveTypeName"].SetOrdinal(count++);
            mLeaveAttendance_dt.Columns["Note"].SetOrdinal(count++);

            attendanceGV.DataSource = mLeaveAttendance_dt;
            attendanceGV.Columns["LeaveID"].Visible = false;
            attendanceGV.Columns["LeaveTypeCode"].Visible = false;
            attendanceGV.Columns["EmployeeCode"].Visible = false;
            attendanceGV.Columns["UpdatedHistory"].Visible = false;

            attendanceGV.Columns["DateOff"].DefaultCellStyle.Format = "dd/MM/yyyy";

            attendanceGV.Columns["DayOfWeek"].HeaderText = "Thứ";
            attendanceGV.Columns["LeaveTypeName"].HeaderText = "Loại Nghỉ Phép";
            attendanceGV.Columns["DateOff"].HeaderText = "Ngày Nghỉ";
            attendanceGV.Columns["Note"].HeaderText = "Ghi Chú";
            attendanceGV.Columns["LeaveHours"].HeaderText = "Số Giờ Nghỉ";

            attendanceGV.Columns["DayOfWeek"].Width = 40;
            attendanceGV.Columns["LeaveTypeName"].Width = 150;
            attendanceGV.Columns["DateOff"].Width = 100;
            attendanceGV.Columns["Note"].Width = 250;

            attendanceGV.Columns["UpdatedHistory"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            attendanceGV.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            attendanceGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            attendanceGV.SelectionChanged += this.attendanceGV_CellClick;
            if (dataGV.CurrentRow != null) {
                int selectedIndex = dataGV.CurrentRow?.Index ?? -1;
                UpdateAttendanceUI(selectedIndex);
            }

            await Task.Delay(100);
            loadingOverlay.Hide();
        }


        private async void LoadLeaveAttendance_btn_Click(object sender, EventArgs e)
        {
            int year = Convert.ToInt32(year_tb.Text);

            var leaveAttendanceTask = SQLStore.Instance.GetLeaveAttendancesAsyn(year);

            await Task.WhenAll(leaveAttendanceTask);

            mLeaveAttendance_dt = leaveAttendanceTask.Result;
            loadLeaveAttendance();

            curYear = year;
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


            UpdateAttendanceUI(rowIndex);
        }

        private void attendanceGV_CellClick(object sender, EventArgs e)
        {
            if (attendanceGV.CurrentRow == null) return;
            int rowIndex = attendanceGV.CurrentRow.Index;
            if (rowIndex < 0)
                return;

            UpdateRightUI(rowIndex);
        }

        private void UpdateRightUI(int rowIndex)
        {
            if (isNewState) return;
            var cells = attendanceGV.Rows[rowIndex].Cells;
            int leaveID = Convert.ToInt32(cells["LeaveID"].Value);
            string leaveTypeCode = cells["LeaveTypeCode"].Value.ToString();
            DateTime dateOff = Convert.ToDateTime(cells["DateOff"].Value);
            string note = cells["Note"].Value.ToString();
            string updatedHistory = cells["UpdatedHistory"].Value.ToString();
            decimal hourLeave = Convert.ToDecimal(cells["LeaveHours"].Value);

            leaveID_tb.Text = leaveID.ToString();
            dateOffStart_dtp.Value = dateOff.Date;
            dateOffEnd_dtp.Value = dateOff.Date;
            leaveType_cbb.SelectedValue = leaveTypeCode;
            note_tb.Text = note;
            updatedHistory_tb.Text = updatedHistory;
            hourLeave_tb.Text = hourLeave.ToString();

            info_gb.BackColor = Color.DarkGray;
            status_lb.Text = "";
            dateOffEnd_dtp.Visible = false;
            label5.Visible = false;
        }

        private void UpdateAttendanceUI(int index)
        {
            var cells = dataGV.Rows[index].Cells;
            //int employeeID = Convert.ToInt32(cells["EmployeeID"].Value);
            string employeeCode = cells["EmployeeCode"].Value.ToString();

            DataView dv = new DataView(mLeaveAttendance_dt);
            dv.RowFilter = $"EmployeeCode = '{employeeCode}' AND LeaveTypeCode <> 'NL_1'";

            attendanceGV.DataSource = dv;
        }

        private async void updateData(int leaveID, string employeeCode, string leaveTypeCode, DateTime dateOff, string Note, string UpdatedHistory, decimal hourLeave)
        {
            foreach (DataGridViewRow row in attendanceGV.Rows)
            {
                int id = Convert.ToInt32(row.Cells["LeaveID"].Value);
                if (id.CompareTo(leaveID) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager.Instance.updateLeaveAttendanceAsync(leaveID, employeeCode, leaveTypeCode, dateOff, Note, UpdatedHistory, hourLeave);

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                row.Cells["EmployeeCode"].Value = employeeCode;
                                row.Cells["LeaveTypeCode"].Value = leaveTypeCode;
                                row.Cells["DateOff"].Value = dateOff.Date;
                                row.Cells["Note"].Value = Note;
                                row.Cells["UpdatedHistory"].Value = UpdatedHistory;
                                row.Cells["LeaveHours"].Value = hourLeave;

                                DataRow[] foundRows = mLeaveType.Select($"LeaveTypeCode = '{leaveTypeCode}'");
                                if (foundRows.Length > 0)
                                {
                                    row.Cells["LeaveTypeName"].Value = foundRows[0]["LeaveTypeName"].ToString();
                                }

                                string[] vietDays = { "CN", "T.2", "T.3", "T.4", "T.5", "T.6", "T.7" };
                                row.Cells["DayOfWeek"].Value = vietDays[(int)dateOff.DayOfWeek];

                            }
                            else
                            {
                                status_lb.Text = "Thất bại.";
                                status_lb.ForeColor = Color.Red;
                            }
                        }
                        catch (Exception ex)
                        {
                            status_lb.Text = "Thất bại.";
                            status_lb.ForeColor = Color.Red;
                        }




                    }
                    break;
                }
            }
        }

        private async void createNew(string employeeCode, string leaveTypeCode, DateTime dateOffStart, DateTime dateOffEnd, string Note, string UpdatedHistory, decimal hourLeave)
        {
            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult != DialogResult.Yes)
                return;

            string[] vietDays = { "CN", "T.2", "T.3", "T.4", "T.5", "T.6", "T.7" };
            bool hasError = false;
            attendanceGV.SuspendLayout();
            for (DateTime date = dateOffStart.Date; date <= dateOffEnd.Date; date = date.AddDays(1))
            {
                try
                {
                    int newEmployee = await SQLManager.Instance.insertLeaveAttendanceAsync(employeeCode, leaveTypeCode, date, Note, UpdatedHistory, hourLeave);
                    if (newEmployee > 0)
                    {
                        DataRow drToAdd = mLeaveAttendance_dt.NewRow();
                        leaveID_tb.Text = newEmployee.ToString();
                        drToAdd["LeaveID"] = newEmployee;
                        drToAdd["EmployeeCode"] = employeeCode;
                        drToAdd["LeaveTypeCode"] = leaveTypeCode;
                        drToAdd["DateOff"] = date.Date;
                        drToAdd["Note"] = Note;
                        drToAdd["UpdatedHistory"] = UpdatedHistory;
                        drToAdd["LeaveHours"] = hourLeave;

                        DataRow[] foundRows = mLeaveType.Select($"LeaveTypeCode = '{leaveTypeCode}'");
                        if (foundRows.Length > 0)
                        {
                            drToAdd["LeaveTypeName"] = foundRows[0]["LeaveTypeName"].ToString();
                        }
                        
                        drToAdd["DayOfWeek"] = vietDays[(int)date.DayOfWeek];

                        mLeaveAttendance_dt.Rows.Add(drToAdd);                       

                        if (SQLStore.Instance.IsDeductAnnualLeave(leaveTypeCode))
                        {
                            DataRow[] rows = mEmployee_dt.Select($"EmployeeCode = '{employeeCode}'");
                            DataRow row = mEmployee_dt.Select($"EmployeeCode = '{employeeCode}'").FirstOrDefault();
                            if (row != null)
                            {
                                int leaveCount = row.Field<int?>("LeaveCount") ?? 0;
                                int remaining = row.Field<int?>("RemainingLeave") ?? 0;

                                row["LeaveCount"] = leaveCount + 1;
                                row["RemainingLeave"] = remaining - 1;
                            }
                        }
                    }
                    else
                    {
                        hasError = true;
                    }
                }
                catch (Exception ex)
                {
                    hasError = true;
                }
            }

            attendanceGV.ResumeLayout();
            mLeaveAttendance_dt.AcceptChanges();
            NewBtn_Click(null, null);
            status_lb.Text = hasError ? "Có lỗi trong quá trình thêm." : "Thêm thành công tất cả.";
            status_lb.ForeColor = hasError ? Color.Red : Color.Green;
        }
        

        private async void saveBtn_Click(object sender, EventArgs e)
        {
            if (leaveType_cbb.SelectedValue == null || dataGV.CurrentRow == null) return;

            if (string.IsNullOrEmpty(hourLeave_tb.Text))
            {
                MessageBox.Show("Thiếu Dữ Liệu", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string employeeCode = dataGV.CurrentRow.Cells["EmployeeCode"].Value?.ToString();
            DateTime dateOffStart = dateOffStart_dtp.Value;
            DateTime dateOffEnd = dateOffEnd_dtp.Value;
            decimal hourLeave = Convert.ToDecimal(hourLeave_tb.Text);
            int year = Convert.ToInt32(year_tb.Text);

            for (DateTime date = dateOffStart.Date; date <= dateOffEnd.Date; date = date.AddDays(1))
            {
                bool isLock = await SQLStore.Instance.IsSalaryLockAsync(date.Month, date.Year);
                if (isLock)
                {
                    MessageBox.Show("Tháng " + date.Month + "/" + date.Year + " đã bị khóa.", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (curYear != year || date.Year < curYear)
                {
                    MessageBox.Show("Tháng hoặc Năm có vẫn đề", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            string leaveTypeCode = Convert.ToString(leaveType_cbb.SelectedValue);
            string note = note_tb.Text;
            string updatedHistory = updatedHistory_tb.Text;
            updatedHistory += DateTime.Now + ": " + UserManager.Instance.fullName + " | ";

            if (leaveID_tb.Text.Length != 0)
                updateData(Convert.ToInt32(leaveID_tb.Text), employeeCode, leaveTypeCode, dateOffStart, note, updatedHistory, hourLeave);
            else
                createNew(employeeCode, leaveTypeCode, dateOffStart, dateOffEnd, note, updatedHistory, hourLeave);

        }

        private void NewBtn_Click(object sender, EventArgs e)
        {
            leaveID_tb.Text = "";
            note_tb.Text = "";
            leaveType_cbb.Focus();
            info_gb.BackColor = newBtn.BackColor;
            edit_btn.Visible = false;
            newBtn.Visible = false;
            readOnly_btn.Visible = true;
            LuuThayDoiBtn.Visible = true;
            delete_btn.Visible = false;
            isNewState = true;
            LuuThayDoiBtn.Text = "Lưu Mới";

            dateOffEnd_dtp.Visible = true;
            label5.Visible = true;
        }

        private void ReadOnly_btn_Click(object sender, EventArgs e)
        {
            edit_btn.Visible = true;
            newBtn.Visible = true;
            readOnly_btn.Visible = false;
            LuuThayDoiBtn.Visible = false;
            delete_btn.Visible = false;
            info_gb.BackColor = Color.DarkGray;
            isNewState = false;
            dateOffEnd_dtp.Visible = false;
            label5.Visible = false;
        }

        private void Edit_btn_Click(object sender, EventArgs e)
        {
            edit_btn.Visible = false;
            newBtn.Visible = false;
            readOnly_btn.Visible = true;
            LuuThayDoiBtn.Visible = true;
            delete_btn.Visible = true;
            info_gb.BackColor = edit_btn.BackColor;
            isNewState = false;
            LuuThayDoiBtn.Text = "Lưu C.Sửa";
        }

        private async void Delete_btn_Click(object sender, EventArgs e)
        {
            if (attendanceGV.SelectedRows.Count == 0 || string.IsNullOrEmpty(leaveID_tb.Text)) return;

            int id = Convert.ToInt32(leaveID_tb.Text);

            foreach (DataGridViewRow row in attendanceGV.Rows)
            {
                int leaveID = Convert.ToInt32(row.Cells["LeaveID"].Value);
                if (leaveID.CompareTo(id) == 0)
                {
                    DateTime dateOff = Convert.ToDateTime(row.Cells["DateOff"].Value);
                    bool isLock = await SQLStore.Instance.IsSalaryLockAsync(dateOff.Month, dateOff.Year);
                    if (isLock)
                    {
                        MessageBox.Show("Tháng " + dateOff.Month + "/" + dateOff.Year + " đã bị khóa.", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    DialogResult dialogResult = MessageBox.Show("XÓA ĐÓ NHA \n Chắc chắn chưa ?", " Xóa Thông Tin", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager.Instance.deleteLeaveAttendanceAsync(leaveID);

                            if (isScussess == true)
                            {
                                if (SQLStore.Instance.IsDeductAnnualLeave(row.Cells["LeaveTypeCode"].Value.ToString()))
                                {
                                    string employeeCode = dataGV.CurrentRow.Cells["EmployeeCode"].Value?.ToString();
                                    DataRow row1 = mEmployee_dt.Select($"EmployeeCode = '{employeeCode}'").FirstOrDefault();
                                    if (row1 != null)
                                    {
                                        int leaveCount = row1.Field<int?>("LeaveCount") ?? 0;
                                        int remaining = row1.Field<int?>("RemainingLeave") ?? 0;

                                        row1["LeaveCount"] = leaveCount - 1;
                                        row1["RemainingLeave"] = remaining + 1;
                                    }
                                }

                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;
                                attendanceGV.Rows.Remove(row);
                            }
                            else
                            {
                                status_lb.Text = "Thất bại.";
                                status_lb.ForeColor = Color.Red;
                            }
                        }
                        catch (Exception ex)
                        {
                            status_lb.Text = "Thất bại.";
                            status_lb.ForeColor = Color.Red;
                        }




                    }
                    break;
                }
            }
        }
        private async void DateOffStart_dtp_ValueChanged(object sender, EventArgs e)
        {
            DateTime dayyOffStart = dateOffStart_dtp.Value.Date;
            DateTime dayyOffEnd = dateOffEnd_dtp.Value.Date;

            if (dayyOffStart > dayyOffEnd)
                dateOffEnd_dtp.Value = dayyOffStart;
            bool isLock = await SQLStore.Instance.IsSalaryLockAsync(dayyOffStart.Month, dayyOffStart.Year);
            LuuThayDoiBtn.Visible = !isLock;
            newBtn.Visible = !isLock;
            delete_btn.Visible = !isLock;
            edit_btn.Visible = !isLock;
        }

        private async void DateOffEnd_dtp_ValueChanged(object sender, EventArgs e)
        {
            DateTime dayyOffStart = dateOffStart_dtp.Value.Date;
            DateTime dayyOffEnd = dateOffEnd_dtp.Value.Date;

            if (dayyOffStart > dayyOffEnd)
                dateOffEnd_dtp.Value = dayyOffStart;
        }
    }
}
