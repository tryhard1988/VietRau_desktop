using RauViet.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Management.Instrumentation;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RauViet.ui
{
    public partial class LeaveAttendance : Form
    {
        private Timer _monthYearDebounceTimer;
        System.Data.DataTable mLeaveAttendance_dt, mEmployee_dt, mLeaveType;
        DataView mLogDV;
        bool isNewState = false;
        int curYear = -1;
        // DataTable mShift_dt;
        public LeaveAttendance()
        {
            InitializeComponent();
            this.KeyPreview = true;
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
            
            newBtn.Click += NewBtn_Click;
            delete_btn.Click += Delete_btn_Click;
            hourLeave_tb.KeyPress += Tb_KeyPress_OnlyNumber;
            year_tb.KeyPress += Tb_KeyPress_OnlyNumber;
            dateOffStart_dtp.ValueChanged += DateOffStart_dtp_ValueChanged;
            dateOffEnd_dtp.ValueChanged += DateOffEnd_dtp_ValueChanged;

            edit_btn.Click += Edit_btn_Click;
            readOnly_btn.Click += ReadOnly_btn_Click;
            ReadOnly_btn_Click(null, null);

            linkStartEnd_cb.CheckedChanged += LinkStartEnd_cb_CheckedChanged;
            this.KeyDown += LeaveAttendance_KeyDown;

            search_tb.TextChanged += Search_tb_TextChanged;
            this.Load += OvertimeAttendace_Load;
            leaveType_cbb.SelectedIndexChanged += LeaveType_cbb_SelectedIndexChanged;
        }

        private void OvertimeAttendace_Load(object sender, EventArgs e)
        {
            _monthYearDebounceTimer = new Timer();
            _monthYearDebounceTimer.Interval = 500;
            _monthYearDebounceTimer.Tick += MonthYearDebounceTimer_Tick;
        }

        private void LeaveAttendance_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                int year = Convert.ToInt32(year_tb.Text);

                SQLStore_QLNS.Instance.removeLeaveAttendances(year);
               // LoadLeaveAttendance_btn_Click(null, null);
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
                year_tb.TextChanged -= monthYearDtp_ValueChanged;
                
                int year = Convert.ToInt32(year_tb.Text);
                var leavecodeParam = new List<string>{"NL_1"};
                var leaveTypeTask = SQLStore_QLNS.Instance.GetLeaveTypeWithoutAsync(leavecodeParam);
                var employeeRemainingLeaveTask = SQLStore_QLNS.Instance.GetAnnualLeaveBalanceAsync();
                var leaveAttendanceTask = SQLStore_QLNS.Instance.GetLeaveAttendancesAsyn(year);
                var leaveAttendanceLogTask = SQLStore_QLNS.Instance.GetLeaveAttendanceLogAsync(year);


                await Task.WhenAll(employeeRemainingLeaveTask, leaveAttendanceTask, leaveTypeTask, leaveAttendanceLogTask);
                mEmployee_dt = employeeRemainingLeaveTask.Result;
                mLeaveAttendance_dt = leaveAttendanceTask.Result;
                mLeaveType = leaveTypeTask.Result;
                mLogDV = new DataView(leaveAttendanceLogTask.Result);
                curYear = year;
                monthYearLabel.Text = $"Năm {year}";

                leaveType_cbb.DataSource = mLeaveType;
                leaveType_cbb.DisplayMember = "LeaveTypeName";
                leaveType_cbb.ValueMember = "LeaveTypeCode";

                dataGV.DataSource = mEmployee_dt;
                dataGV.Columns["EmployeeCode"].HeaderText = "Mã Nhân Viên";
                dataGV.Columns["FullName"].HeaderText = "Tên Nhân Viên";
              //  dataGV.Columns["ContractTypeName"].HeaderText = "Loại H.Đồng";
                dataGV.Columns["PositionName"].HeaderText = "Chức Vụ";
                dataGV.Columns["RemainingLeaveDays_1"].HeaderText = "Phép Năm Còn";

                mEmployee_dt.Columns["LeaveCount"].ReadOnly = false;
                dataGV.Columns["Month"].Visible = false;
                dataGV.Columns["RemainingLeaveDays"].Visible = false;
                dataGV.Columns["LeaveCount"].Visible = false;
                dataGV.Columns["EmployessName_NoSign"].Visible = false;


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


                log_GV.DataSource = mLogDV;
                loadLeaveAttendance();

                log_GV.Columns["LogID"].Visible = false;
                log_GV.Columns["EmployeeCode"].Visible = false;

                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                log_GV.Columns["CreatedAt"].Width = 120;
                log_GV.Columns["ActionBy"].Width = 150;
                log_GV.Columns["LeaveName"].Width = 100;
                log_GV.Columns["Description"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                log_GV.Columns["DateOff"].Width = 80;
                log_GV.Columns["LeaveHour"].Width = 60;
                log_GV.Columns["CreatedAt"].HeaderText = "Thời điểm thay đổi";
                log_GV.Columns["ACtionBy"].HeaderText = "Người thay đổi";
                log_GV.Columns["LeaveName"].HeaderText = "Loại nghỉ phép";
                log_GV.Columns["Description"].HeaderText = "Hành động";
                log_GV.Columns["DateOff"].HeaderText = "Ngày Nghỉ";
                log_GV.Columns["LeaveHour"].HeaderText = "Số giờ nghỉ";

                log_GV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                year_tb.TextChanged += monthYearDtp_ValueChanged;
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

        

        private async void loadLeaveAttendance()
        {
            await Task.Delay(50);
            LoadingOverlay loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Show();
            await Task.Delay(200);

            attendanceGV.SelectionChanged -= this.attendanceGV_CellClick;

            attendanceGV.DataSource = mLeaveAttendance_dt;
            attendanceGV.Columns["LeaveID"].Visible = false;
            attendanceGV.Columns["LeaveTypeCode"].Visible = false;
            attendanceGV.Columns["EmployeeCode"].Visible = false;
            attendanceGV.Columns["IsRemoved"].Visible = false;

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


        //private async void LoadLeaveAttendance_btn_Click(object sender, EventArgs e)
        //{

            

        //}

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
            decimal hourLeave = Convert.ToDecimal(cells["LeaveHours"].Value);

            leaveID_tb.Text = leaveID.ToString();
            dateOffStart_dtp.Value = dateOff.Date;
            dateOffEnd_dtp.Value = dateOff.Date;
            leaveType_cbb.SelectedValue = leaveTypeCode;
            note_tb.Text = note;
            hourLeave_tb.Text = hourLeave.ToString();

            
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
            dv.RowFilter = $"EmployeeCode = '{employeeCode}' AND LeaveTypeCode <> 'NL_1' AND IsRemoved = 0";

            attendanceGV.DataSource = dv;
            mLogDV.RowFilter = $"EmployeeCode = '{employeeCode}'";
        }

        private async void updateData(int leaveID, string employeeCode, string leaveTypeCode, DateTime dateOff, string Note, decimal hourLeave)
        {
            foreach (DataGridViewRow row in attendanceGV.Rows)
            {
                int id = Convert.ToInt32(row.Cells["LeaveID"].Value);
                if (id.CompareTo(leaveID) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogResult == DialogResult.Yes)
                    {
                        string temp1 = row.Cells["LeaveTypeName"].Value.ToString();
                        string temp2 = row.Cells["DateOff"].Value.ToString();
                        string temp3 = row.Cells["LeaveHours"].Value.ToString();
                        string temp4 = row.Cells["Note"].Value.ToString();
                        try
                        {
                            bool isScussess = await SQLManager_QLNS.Instance.updateLeaveAttendanceAsync(leaveID, employeeCode, leaveTypeCode, dateOff, Note, hourLeave);

                            DataRow[] foundRows = mLeaveType.Select($"LeaveTypeCode = '{leaveTypeCode}'");
                            string leaveName = foundRows.Length > 0 ? leaveName = foundRows[0]["LeaveTypeName"].ToString() : "";

                            if (isScussess == true)
                            {
                                _ = SQLManager_QLNS.Instance.InsertLeaveAttandanceLogAsync(employeeCode, leaveName,
                                    $"Edit Success {leaveID} - {temp1} - {temp2} - {temp3} - {temp4}",
                                    dateOff.Date, hourLeave, Note);

                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                string LeaveTypeCodeOld = row.Cells["LeaveTypeCode"].Value.ToString();
                                if (LeaveTypeCodeOld.CompareTo(leaveTypeCode) != 0)
                                {
                                    bool isDeductAnnualLeaveOld = SQLStore_QLNS.Instance.IsDeductAnnualLeave(LeaveTypeCodeOld);
                                    bool isDeductAnnualLeaveNew = SQLStore_QLNS.Instance.IsDeductAnnualLeave(leaveTypeCode);
                                    if (isDeductAnnualLeaveOld != isDeductAnnualLeaveNew)
                                    {
                                        DataRow eRow = mEmployee_dt.Select($"EmployeeCode = '{employeeCode}'").FirstOrDefault();
                                        if (eRow != null)
                                        {
                                            int leaveCount = eRow.Field<int?>("LeaveCount") ?? 0;
                                            int remaining = eRow.Field<int?>("RemainingLeaveDays_1") ?? 0;

                                            if (isDeductAnnualLeaveNew)
                                            {
                                                eRow["LeaveCount"] = leaveCount + 1;
                                                eRow["RemainingLeaveDays_1"] = remaining - 1;
                                            }
                                            else
                                            {
                                                eRow["LeaveCount"] = leaveCount - 1;
                                                eRow["RemainingLeaveDays_1"] = remaining + 1;
                                            }
                                        }
                                    }
                                }

                                row.Cells["EmployeeCode"].Value = employeeCode;
                                row.Cells["LeaveTypeCode"].Value = leaveTypeCode;
                                row.Cells["DateOff"].Value = dateOff.Date;
                                row.Cells["Note"].Value = Note;
                                row.Cells["LeaveHours"].Value = hourLeave;
                                row.Cells["LeaveTypeName"].Value = leaveName;
                                

                                string[] vietDays = { "CN", "T.2", "T.3", "T.4", "T.5", "T.6", "T.7" };
                                row.Cells["DayOfWeek"].Value = vietDays[(int)dateOff.DayOfWeek];

                            }
                            else
                            {
                                _ = SQLManager_QLNS.Instance.InsertLeaveAttandanceLogAsync(employeeCode, leaveName,
                                    $"Edit Fail {leaveID} - {temp1} - {temp2} - {temp3} - {temp4}", 
                                    dateOff.Date, hourLeave, Note);
                                status_lb.Text = "Thất bại.";
                                status_lb.ForeColor = Color.Red;
                            }
                        }
                        catch (Exception ex)
                        {
                            DataRow[] foundRows = mLeaveType.Select($"LeaveTypeCode = '{leaveTypeCode}'");
                            string leaveName = foundRows.Length > 0 ? leaveName = foundRows[0]["LeaveTypeName"].ToString() : "";
                            
                            _ = SQLManager_QLNS.Instance.InsertLeaveAttandanceLogAsync(employeeCode, leaveName,
                                    $"Edit Fail Exception: {ex.Message} : {leaveID} - {temp1} - {temp2} - {temp3} - {temp4}",
                                    dateOff.Date, hourLeave, Note);
                            status_lb.Text = "Thất bại.";
                            status_lb.ForeColor = Color.Red;
                        }
                    }
                    break;
                }
            }
        }

        private async void createNew(string employeeCode, string leaveTypeCode, DateTime dateOffStart, DateTime dateOffEnd, string Note, decimal hourLeave)
        {
            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult != DialogResult.Yes)
                return;

            string[] vietDays = { "CN", "T.2", "T.3", "T.4", "T.5", "T.6", "T.7" };
            bool hasError = false;
            List<DateTime> datetimeErrorList = new List<DateTime>();
            string errorMessage = "";
            attendanceGV.SuspendLayout();
            for (DateTime date = dateOffStart.Date; date <= dateOffEnd.Date; date = date.AddDays(1))
            {
                try
                {
                    DataRow[] foundRows = mLeaveType.Select($"LeaveTypeCode = '{leaveTypeCode}'");
                    string leaveName = foundRows.Length > 0 ? foundRows[0]["LeaveTypeName"].ToString() : "";
                    int newEmployee = await SQLManager_QLNS.Instance.insertLeaveAttendanceAsync(employeeCode, leaveTypeCode, date, Note, hourLeave);
                    if (newEmployee > 0)
                    {
                        DataRow drToAdd = mLeaveAttendance_dt.NewRow();
                        leaveID_tb.Text = newEmployee.ToString();
                        drToAdd["LeaveID"] = newEmployee;
                        drToAdd["EmployeeCode"] = employeeCode;
                        drToAdd["LeaveTypeCode"] = leaveTypeCode;
                        drToAdd["DateOff"] = date.Date;
                        drToAdd["Note"] = Note;
                        drToAdd["LeaveHours"] = hourLeave;
                        drToAdd["IsRemoved"] = false;


                        drToAdd["LeaveTypeName"] = leaveName;                        
                        drToAdd["DayOfWeek"] = vietDays[(int)date.DayOfWeek];

                        mLeaveAttendance_dt.Rows.Add(drToAdd);

                        _ = SQLManager_QLNS.Instance.InsertLeaveAttandanceLogAsync(employeeCode, leaveName, $"Create Success {newEmployee}", date.Date, hourLeave, Note);

                        if (SQLStore_QLNS.Instance.IsDeductAnnualLeave(leaveTypeCode))
                        {
                            DataRow row = mEmployee_dt.Select($"EmployeeCode = '{employeeCode}'").FirstOrDefault();
                            if (row != null)
                            {
                                int leaveCount = row.Field<int?>("LeaveCount") ?? 0;
                                int remaining = row.Field<int?>("RemainingLeaveDays_1") ?? 0;

                                row["LeaveCount"] = leaveCount + 1;
                                row["RemainingLeaveDays_1"] = remaining - 1;
                            }
                        }
                    }
                    else
                    {
                        hasError = true;
                        datetimeErrorList.Add(date);
                        _ = SQLManager_QLNS.Instance.InsertLeaveAttandanceLogAsync(employeeCode, leaveName, $"Create Fail", date.Date, hourLeave, Note);
                    }
                }
                catch (Exception ex)
                {
                    DataRow[] foundRows = mLeaveType.Select($"LeaveTypeCode = '{leaveTypeCode}'");
                    string leaveName = foundRows.Length > 0 ? foundRows[0]["LeaveTypeName"].ToString() : "";
                    _ = SQLManager_QLNS.Instance.InsertLeaveAttandanceLogAsync(employeeCode, leaveName, $"Create Fail Exception: {ex.Message}", date.Date, hourLeave, Note);
                    Console.WriteLine("Error:" + ex.Message);
                    errorMessage = "Error:" + ex.Message;
                    hasError = true;
                }
            }

            attendanceGV.ResumeLayout();
            mLeaveAttendance_dt.AcceptChanges();
            NewBtn_Click(null, null);
            if (!hasError)
            {
                status_lb.Text = "Thành công.";
                status_lb.ForeColor = Color.Green;
            }
            else
            {
                if (datetimeErrorList.Count > 0)
                {
                    string result = string.Join(", ", datetimeErrorList.Select(d => d.ToString("dd/MM/yyyy")));
                    MessageBox.Show("Lỗi: " + result);
                }
                else
                {
                    MessageBox.Show(errorMessage);
                }
            }
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
                bool isLock = await SQLStore_QLNS.Instance.IsSalaryLockAsync(date.Month, date.Year);
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

            if (leaveID_tb.Text.Length != 0)
                updateData(Convert.ToInt32(leaveID_tb.Text), employeeCode, leaveTypeCode, dateOffStart, note, hourLeave);
            else
                createNew(employeeCode, leaveTypeCode, dateOffStart, dateOffEnd, note, hourLeave);

            SQLStore_QLNS.Instance.removeAttendamce(dateOffStart.Month, dateOffStart.Year);
            SQLStore_QLNS.Instance.removeAttendamce(dateOffEnd.Month, dateOffEnd.Year);
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
            SetUIReadOnly(false);
            linkStartEnd_cb.Visible = true;
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
            SetUIReadOnly(true);
            attendanceGV_CellClick(null, null);
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
            
            SetUIReadOnly(false);
            linkStartEnd_cb.Checked = true;
            linkStartEnd_cb.Visible = false;
        }

        private async void Delete_btn_Click(object sender, EventArgs e)
        {
            if (attendanceGV.SelectedRows.Count == 0 || string.IsNullOrEmpty(leaveID_tb.Text)) return;

            int id = Convert.ToInt32(leaveID_tb.Text);
            foreach (DataGridViewRow row in attendanceGV.Rows)
            {
                int leaveID = Convert.ToInt32(row.Cells["LeaveID"].Value);
                string employeeCode = row.Cells["EmployeeCode"].Value.ToString();
                string leaveName = row.Cells["LeaveTypeName"].Value.ToString();
                if (leaveID.CompareTo(id) == 0)
                {
                    DateTime dateOff = Convert.ToDateTime(row.Cells["DateOff"].Value);
                    bool isRemoved = Convert.ToBoolean(row.Cells["IsRemoved"].Value);
                    bool isLock = await SQLStore_QLNS.Instance.IsSalaryLockAsync(dateOff.Month, dateOff.Year);
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
                            bool isScussess = await SQLManager_QLNS.Instance.deleteLeaveAttendanceAsync(leaveID);

                            if (isScussess == true)
                            {
                                _ = SQLManager_QLNS.Instance.InsertLeaveAttandanceLogAsync(employeeCode, leaveName, $"Delete Success", DateTime.Now, 0, "Delete");

                                if (!isRemoved && SQLStore_QLNS.Instance.IsDeductAnnualLeave(row.Cells["LeaveTypeCode"].Value.ToString()))
                                {
                                    DataRow row1 = mEmployee_dt.Select($"EmployeeCode = '{employeeCode}'").FirstOrDefault();
                                    if (row1 != null)
                                    {
                                        int leaveCount = row1.Field<int?>("LeaveCount") ?? 0;
                                        int remaining = row1.Field<int?>("RemainingLeaveDays_1") ?? 0;
                                        
                                        row1["LeaveCount"] = leaveCount - 1;
                                        row1["RemainingLeaveDays_1"] = remaining + 1;

                                        SQLStore_QLNS.Instance.removeAttendamce(dateOff.Month, dateOff.Year);
                                    }
                                }

                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;
                                attendanceGV.Rows.Remove(row);

                                mLeaveAttendance_dt.AcceptChanges();
                            }
                            else
                            {
                                _ = SQLManager_QLNS.Instance.InsertLeaveAttandanceLogAsync(employeeCode, leaveName, $"Delete Fail", DateTime.Now, 0, "Delete");
                                status_lb.Text = "Thất bại.";
                                status_lb.ForeColor = Color.Red;
                            }
                        }
                        catch (Exception ex)
                        {
                            _ = SQLManager_QLNS.Instance.InsertLeaveAttandanceLogAsync(employeeCode, leaveName, $"Delete Fail Exception {ex.Message}", DateTime.Now, 0, "Delete");
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
            if (linkStartEnd_cb.Checked)
            {
                dateOffEnd_dtp.Value = dayyOffStart;
            }
            else
            {
                DateTime dayyOffEnd = dateOffEnd_dtp.Value.Date;

                if (dayyOffStart > dayyOffEnd)
                    dateOffEnd_dtp.Value = dayyOffStart;
            }

            bool isLock = await SQLStore_QLNS.Instance.IsSalaryLockAsync(dayyOffStart.Month, dayyOffStart.Year);

            LuuThayDoiBtn.Enabled = !isLock;
            newBtn.Enabled = !isLock;
            delete_btn.Enabled = !isLock;
            edit_btn.Enabled = !isLock;
        }

        private async void DateOffEnd_dtp_ValueChanged(object sender, EventArgs e)
        {
            DateTime dayyOffStart = dateOffStart_dtp.Value.Date;
            DateTime dayyOffEnd = dateOffEnd_dtp.Value.Date;

            if (dayyOffStart > dayyOffEnd)
                dateOffEnd_dtp.Value = dayyOffStart;
        }

        private void SetUIReadOnly(bool isReadOnly)
        {
            leaveType_cbb.Enabled = !isReadOnly;
            dateOffStart_dtp.Enabled = !isReadOnly;
            dateOffEnd_dtp.Enabled = !isReadOnly;
            hourLeave_tb.ReadOnly = isReadOnly;
            note_tb.ReadOnly = isReadOnly;
            linkStartEnd_cb.Visible = !isReadOnly;
            if (!isReadOnly)
            {
                LinkStartEnd_cb_CheckedChanged(null, null);
            }
        }

        private void LinkStartEnd_cb_CheckedChanged(object sender, EventArgs e)
        {
            dateOffEnd_dtp.Visible = !linkStartEnd_cb.Checked;
            label5.Visible = !linkStartEnd_cb.Checked;
                        
            if (linkStartEnd_cb.Checked)
            {
                DateTime dayyOffStart = dateOffStart_dtp.Value.Date;
                dateOffEnd_dtp.Value = dayyOffStart;
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
            await Task.Delay(50);
            LoadingOverlay loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Show();
            await Task.Delay(200);

            int year = Convert.ToInt32(year_tb.Text);

            var leaveAttendanceTask = SQLStore_QLNS.Instance.GetLeaveAttendancesAsyn(year);
            var leaveAttendanceLogTask = SQLStore_QLNS.Instance.GetLeaveAttendanceLogAsync(year);
            await Task.WhenAll(leaveAttendanceTask, leaveAttendanceLogTask);

            mLeaveAttendance_dt = leaveAttendanceTask.Result;
            mLogDV = new DataView(leaveAttendanceLogTask.Result);
            log_GV.DataSource = mLogDV;
            loadLeaveAttendance();

            curYear = year;

            monthYearLabel.Text = $"Năm {year}";

            await Task.Delay(100);
            loadingOverlay.Hide();
        }

        private void LeaveType_cbb_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataRowView dR = (DataRowView)leaveType_cbb.SelectedItem;

            string leaveTypeCode = dR["LeaveTypeCode"].ToString();
            if (SQLStore_QLNS.Instance.IsDeductAnnualLeave(leaveTypeCode))
            {
                hourLeave_tb.Text = "8";
                hourLeave_tb.Enabled = false;
            }
            else
            {
                hourLeave_tb.Enabled = true;
            }
        }
    }
}
