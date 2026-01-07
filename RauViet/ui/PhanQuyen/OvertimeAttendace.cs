
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
    public partial class OvertimeAttendace : Form
    {
        System.Data.DataTable mOvertimeAttendamce_dt, mEmployee_dt, mOvertimeType;
        DataView mLogDV, mEmployeeDV;
        bool isNewState = false;
        bool isPrintState = false;
        private Timer _monthYearDebounceTimer;
        int mCurMonth, mCurYear;
        // DataTable mShift_dt;
        public OvertimeAttendace()
        {
            InitializeComponent();
            this.KeyPreview = true;
            Utils.SetTabStopRecursive(this, false);

            int countTab = 0;
          //  workDate_dtp.TabIndex = countTab++; workDate_dtp.TabStop = true;
            startTime_dtp.TabIndex = countTab++; startTime_dtp.TabStop = true;
            endTime_dtp.TabIndex = countTab++; endTime_dtp.TabStop = true;
            overtimeType_cbb.TabIndex = countTab++; overtimeType_cbb.TabStop = true;
            note_tb.TabIndex = countTab++; note_tb.TabStop = true;
            LuuThayDoiBtn.TabIndex = countTab++; LuuThayDoiBtn.TabStop = true;

            monthYearDtp.Format = DateTimePickerFormat.Custom;
            monthYearDtp.CustomFormat = "dd/MM/yyyy";
          //  monthYearDtp.ShowUpDown = true;
            monthYearDtp.Value = DateTime.Now;

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            departmentGV.MultiSelect = false;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = true;

            attendanceGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            attendanceGV.MultiSelect = false;

            status_lb.Text = "";

            //workDate_dtp.Format = DateTimePickerFormat.Custom;
            //workDate_dtp.CustomFormat = "dd";
            //workDate_dtp.ShowUpDown = true;

            startTime_dtp.Format = DateTimePickerFormat.Custom;
            startTime_dtp.CustomFormat = "HH:mm";
            startTime_dtp.ShowUpDown = true;

            endTime_dtp.Format = DateTimePickerFormat.Custom;
            endTime_dtp.CustomFormat = "HH:mm";
            endTime_dtp.ShowUpDown = true;

            LuuThayDoiBtn.Click += saveBtn_Click;
            
            newBtn.Click += NewBtn_Click;
            edit_btn.Click += Edit_btn_Click;
            readOnly_btn.Click += ReadOnly_btn_Click;
            in_DS_TCa_btn.Click += In_DS_TCa_btn_Click;
            inPreview_btn.Click += InPreview_btn_Click; ;
            ReadOnly_btn_Click(null, null);
            this.KeyDown += OvertimeAttendace_KeyDown;

            this.Load += OvertimeAttendace_Load;

            attendanceMonth_CB.CheckedChanged += AttendanceMonth_CB_CheckedChanged;
        }

        private void OvertimeAttendace_Load(object sender, EventArgs e)
        {
            _monthYearDebounceTimer = new Timer();
            _monthYearDebounceTimer.Interval = 500;
            _monthYearDebounceTimer.Tick += MonthYearDebounceTimer_Tick;
        }

        private void OvertimeAttendace_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.F5)
            {
                int month = monthYearDtp.Value.Month;
                int year = monthYearDtp.Value.Year;
                mCurMonth = -1;
                SQLStore_QLNS.Instance.removeOvertimeAttendamce(month, year);
                HandleMonthYearChanged();
            }
            else if (!isNewState && !edit_btn.Visible)
            {
                if (e.KeyCode == Keys.Delete)
                {
                    Control ctrl = this.ActiveControl;

                    if (ctrl is TextBox || ctrl is RichTextBox ||
                        (ctrl is DataGridView dgv && dgv.CurrentCell != null && dgv.IsCurrentCellInEditMode))
                    {
                        return; // không xử lý Delete
                    }

                    DialogResult dialogResult = MessageBox.Show("Xóa thật không?", "Thông Báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (dialogResult != DialogResult.Yes) return;

                    int id = Convert.ToInt32(attendanceGV.CurrentRow.Cells["OvertimeAttendanceID"].Value);
                    deleteOvertimeAttendanceID(id);
                }
            }
        }

        public async void ShowData()
        {
            dataGV.SelectionChanged -= this.dataGV_CellClick;
            monthYearDtp.ValueChanged -= monthYearDtp_ValueChanged;

            await Task.Delay(50);
            LoadingOverlay loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Show();
            await Task.Delay(200);
            try
            {
                departmentGV.SelectionChanged -= DepartmentGV_SelectionChanged;
                int month = monthYearDtp.Value.Month;
                int year = monthYearDtp.Value.Year;
                // Chạy truy vấn trên thread riêng
                string[] keepColumns = { "EmployeeCode", "FullName", "PositionName", "ContractTypeName", "DepartmentID"};
                mEmployee_dt = await SQLStore_QLNS.Instance.GetEmployeesAsync(keepColumns);
                var departmentTask = SQLStore_QLNS.Instance.GetActiveDepartmentAsync(UserManager.Instance.get_ChamCongTangCa_Departments());
                var overtimeAttendamceTask = SQLStore_QLNS.Instance.GetOvertimeAttendamceAsync(month, year);
                var overtimeTypeTask = SQLStore_QLNS.Instance.GetOvertimeTypeAsync(true);
                var OvertimeAttendanceLogTask = SQLStore_QLNS.Instance.GetOvertimeAttendanceLogAsync(month, year);
                
                await Task.WhenAll(overtimeAttendamceTask, overtimeTypeTask, OvertimeAttendanceLogTask, departmentTask);
                DataTable department_dt = departmentTask.Result;

                mCurMonth = month;
                mCurYear = year;
                mOvertimeAttendamce_dt = overtimeAttendamceTask.Result;
                mOvertimeType = overtimeTypeTask.Result;
                mLogDV = new DataView(OvertimeAttendanceLogTask.Result);
                log_GV.DataSource = mLogDV;
                departmentGV.DataSource = department_dt;

                monthYearLabel.Text = $"Tháng {month}/{year}";

                var departmentIds = department_dt.AsEnumerable().Select(r => r.Field<int>("DepartmentID")).ToHashSet();
                var rows = mEmployee_dt.AsEnumerable().Where(r => departmentIds.Contains(r.Field<int>("DepartmentID")));
                mEmployee_dt = rows.Any() ? rows.CopyToDataTable(): mEmployee_dt.Clone();

                overtimeType_cbb.DataSource = mOvertimeType;
                overtimeType_cbb.DisplayMember = "OvertimeName";
                overtimeType_cbb.ValueMember = "OvertimeTypeID";

                mEmployeeDV = new DataView(mEmployee_dt);
                dataGV.DataSource = mEmployeeDV;
                dataGV.Columns["EmployeeCode"].HeaderText = "Mã Nhân Viên";
                dataGV.Columns["FullName"].HeaderText = "Tên Nhân Viên";
                dataGV.Columns["ContractTypeName"].HeaderText = "Loại H.Đồng";
                dataGV.Columns["PositionName"].HeaderText = "Chức Vụ";

                dataGV.Columns["EmployeeCode"].Width = 50;
                dataGV.Columns["FullName"].Width = 160;
                dataGV.Columns["ContractTypeName"].Width = 70;
                dataGV.Columns["PositionName"].Width = 70;

                log_GV.Columns["EmployeeCode"].Visible = false;
                log_GV.Columns["LogID"].Visible = false;
                departmentGV.Columns["DepartmentID"].Visible = false;
                departmentGV.Columns["Description"].Visible = false;
                departmentGV.Columns["IsActive"].Visible = false;
                departmentGV.Columns["CreatedAt"].Visible = false;
                dataGV.Columns["DepartmentID"].Visible = false;
                dataGV.Columns["PositionName"].Visible = false;
                dataGV.Columns["ContractTypeName"].Visible = false;

                departmentGV.Columns["DepartmentName"].HeaderText = "Bộ Phận";
                departmentGV.Columns["DepartmentName"].Width = 120;
                departmentGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                log_GV.Columns["CreatedAt"].Width = 120;
                log_GV.Columns["ActionBy"].Width = 130;
                log_GV.Columns["OvertimeTypeName"].Width = 100;
                log_GV.Columns["Description"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                log_GV.Columns["WorkDate"].Width = 100;
                log_GV.Columns["StartTime"].Width = 100;
                log_GV.Columns["EndTime"].Width = 100;


                log_GV.Columns["CreatedAt"].HeaderText = "Thời điểm thay đổi";
                log_GV.Columns["ActionBy"].HeaderText = "Người thay đổi";
                log_GV.Columns["OvertimeTypeName"].HeaderText = "Loại tăng ca";
                log_GV.Columns["Description"].HeaderText = "Hành động";
                log_GV.Columns["WorkDate"].HeaderText = "Ngày làm";
                log_GV.Columns["StartTime"].HeaderText = "Giờ bắt đầu";
                log_GV.Columns["EndTime"].HeaderText = "Giờ kết thúc";
                log_GV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                Attendamce();

                

                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                bool isLock = await SQLStore_QLNS.Instance.IsSalaryLockAsync(month, year);
                edit_btn.Visible = !isLock;
                newBtn.Visible = !isLock;

                //loadAttandance_btn.Click -= LoadAttandance_btn_Click;
               // loadAttandance_btn.Click += LoadAttandance_btn_Click;
                
                dataGV.SelectionChanged += this.dataGV_CellClick;

                
                monthYearDtp.ValueChanged += monthYearDtp_ValueChanged;



                if (departmentGV.Rows.Count > 0)
                {                    
                    departmentGV.ClearSelection();
                    departmentGV.Rows[0].Selected = true;
                    departmentGV.CurrentCell = departmentGV.Rows[0].Cells[1];
                    DepartmentGV_SelectionChanged(null, null);
                }

                
                departmentGV.SelectionChanged += DepartmentGV_SelectionChanged;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
                status_lb.Text = "Thất bại.";
                status_lb.ForeColor = Color.Red;
            }
            finally
            {
                await Task.Delay(100);
                loadingOverlay.Hide();
            }
        }

        private void Attendamce()
        {
            attendanceGV.SelectionChanged -= this.attendanceGV_CellClick;

            attendanceGV.DataSource = mOvertimeAttendamce_dt;
            attendanceGV.Columns["DepartmentID"].Visible = false;
            attendanceGV.Columns["OvertimeAttendanceID"].Visible = false;
            attendanceGV.Columns["OvertimeTypeID"].Visible = false;
            attendanceGV.Columns["SalaryFactor"].Visible = false;
           // attendanceGV.Columns["HourWork"].Visible = false;

            attendanceGV.Columns["WorkDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            attendanceGV.Columns["HourWork"].DefaultCellStyle.Format = "0.##";

            attendanceGV.Columns["EmployeeCode"].HeaderText = "Mã NV";
            attendanceGV.Columns["EmployeeName"].HeaderText = "Tên NV";
            attendanceGV.Columns["WorkDate"].HeaderText = "Ngày Làm";
            attendanceGV.Columns["HourWork"].HeaderText = "Số G.Làm";
            attendanceGV.Columns["DayOfWeek"].HeaderText = "Thứ";
            attendanceGV.Columns["OvertimeName"].HeaderText = "Loại Tăng Ca";
            attendanceGV.Columns["StartTime"].HeaderText = "Giờ Bắt Đầu";
            attendanceGV.Columns["EndTime"].HeaderText = "Giờ Kết Thúc";
            attendanceGV.Columns["Note"].HeaderText = "Ghi Chú";

            attendanceGV.Columns["EmployeeCode"].Width = 70;
            attendanceGV.Columns["EmployeeName"].Width = 160;
            attendanceGV.Columns["DayOfWeek"].Width = 40;
            attendanceGV.Columns["WorkDate"].Width = 70;
            attendanceGV.Columns["HourWork"].Width = 50;
            attendanceGV.Columns["OvertimeName"].Width = 90;
            attendanceGV.Columns["Note"].Width = 150;
            attendanceGV.Columns["StartTime"].Width = 65;
            attendanceGV.Columns["EndTime"].Width = 65;

            attendanceGV.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            attendanceGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            attendanceGV.SelectionChanged += this.attendanceGV_CellClick;
            if (dataGV.CurrentRow != null) {
                int selectedIndex = dataGV.CurrentRow?.Index ?? -1;
                UpdateAttendanceUI(selectedIndex);
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

        //private async void LoadAttandance_btn_Click(object sender, EventArgs e)
        //{
            
        //}

        private void DepartmentGV_SelectionChanged(object sender, EventArgs e)
        {
            if (departmentGV.CurrentRow == null) return;

            int departmentID = Convert.ToInt32(departmentGV.CurrentRow.Cells["DepartmentID"].Value);

            mEmployeeDV.RowFilter = $"DepartmentID = {departmentID}";

            DataView dv = new DataView(mOvertimeAttendamce_dt);

            DateTime date = monthYearDtp.Value.Date;
            string filter = $"DepartmentID = {departmentID}";
            if (!attendanceMonth_CB.Checked)
            {
                filter += $" AND WorkDate = #{date:MM/dd/yyyy}#";
            }

            dv.RowFilter = filter;
            attendanceGV.DataSource = dv;

            attendanceGV.ClearSelection();
            dataGV.ClearSelection();
            UpdateInfoUI();
            In_DS_TCa_btn_Click(null, null);
        }
        private void dataGV_CellClick(object sender, EventArgs e)
        {
            if (dataGV.CurrentRow == null) return;
            int rowIndex = dataGV.CurrentRow.Index;
            if (rowIndex < 0)
                return;
            attendanceGV.ClearSelection();
            NewBtn_Click(null, null);
            UpdateAttendanceUI(rowIndex);
            
        }

        private void attendanceGV_CellClick(object sender, EventArgs e)
        {
            if (attendanceGV.CurrentRow == null) return;
            int rowIndex = attendanceGV.CurrentRow.Index;
            if (rowIndex < 0)
                return;
            dataGV.ClearSelection();
            Edit_btn_Click(null, null);
            UpdateRightUI(rowIndex);
        }

        private void UpdateRightUI(int rowIndex)
        {
            if (isNewState) return;
            var cells = attendanceGV.Rows[rowIndex].Cells;
            int overtimeAttendanceID = Convert.ToInt32(cells["OvertimeAttendanceID"].Value);
            DateTime workDate = Convert.ToDateTime(cells["WorkDate"].Value);
            TimeSpan startTime = (TimeSpan)cells["StartTime"].Value;
            TimeSpan endTime = (TimeSpan)cells["EndTime"].Value;
            int overtimeTypeID = Convert.ToInt32(cells["OvertimeTypeID"].Value);            
            string note = cells["Note"].Value.ToString();
            
            overtimeAttendaceID_tb.Text = overtimeAttendanceID.ToString();
           // workDate_dtp.Value = workDate;
            startTime_dtp.Value = workDate.Date + startTime;
            endTime_dtp.Value = workDate.Date + endTime;
            overtimeType_cbb.SelectedValue = overtimeTypeID;
            note_tb.Text = note;

            status_lb.Text = "";
        }


        private void UpdateAttendanceUI(int index)
        {
            var cells = dataGV.Rows[index].Cells;
            //int employeeID = Convert.ToInt32(cells["EmployeeID"].Value);
            string employeeCode = cells["EmployeeCode"].Value.ToString();

            

            mLogDV.RowFilter = $"EmployeeCode = '{employeeCode}'";
        }

        private async void updateData(int overtimeAttendanceID, string employeeCode, TimeSpan startTime, TimeSpan endTime,
                                    int overtimeTypeID, string note)
        {
            foreach (DataGridViewRow row in attendanceGV.Rows)
            {
                int id = Convert.ToInt32(row.Cells["OvertimeAttendanceID"].Value);
                if (id.CompareTo(overtimeAttendanceID) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager_QLNS.Instance.updateOvertimeAttendanceAsync(overtimeAttendanceID, employeeCode, 
                                                                            startTime, endTime, overtimeTypeID, note);
                            
                            DataRow[] rows = mOvertimeType.Select($"OvertimeTypeID = {overtimeTypeID}");
                            string overtimeName = rows.Length > 0 ? rows[0]["OvertimeName"].ToString() : "";

                            _ = SQLManager_QLNS.Instance.InsertOvertimeAttandanceLogAsync(employeeCode, overtimeName,
                                $"edit {(isScussess == true ? "Success" : "Fail")} {overtimeAttendanceID} - {row.Cells["OvertimeName"].Value} - {row.Cells["WorkDate"].Value} - {row.Cells["StartTime"].Value} -> {row.Cells["EndTime"].Value} - {row.Cells["Note"].Value}",
                                Convert.ToDateTime(row.Cells["WorkDate"].Value), startTime, endTime, note);
                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                row.Cells["EmployeeCode"].Value = employeeCode;
                                row.Cells["StartTime"].Value = startTime;
                                row.Cells["EndTime"].Value = endTime;
                                row.Cells["OvertimeTypeID"].Value = overtimeTypeID;
                                row.Cells["Note"].Value = note;
                                row.Cells["HourWork"].Value = (endTime - startTime).TotalHours;
                                row.Cells["SalaryFactor"].Value = SQLStore_QLNS.Instance.GetOvertime_SalaryFactor(overtimeTypeID);
                                row.Cells["OvertimeName"].Value = overtimeName;

                                var rowData = mOvertimeType.Select($"OvertimeTypeID = {overtimeTypeID}")[0];
                                string positionName = rowData["OvertimeName"].ToString();
                                double salaryFactor = Convert.ToDouble(rowData["SalaryFactor"]);

                                UpdateInfoUI();
                            }
                            else
                            {
                                status_lb.Text = "Thất bại.";
                                status_lb.ForeColor = Color.Red;
                            }
                        }
                        catch (Exception ex)
                        {
                            DataRow[] rows = mOvertimeType.Select($"OvertimeTypeID = {overtimeTypeID}");
                            string overtimeName = rows.Length > 0 ? rows[0]["OvertimeName"].ToString() : "";
                            _ = SQLManager_QLNS.Instance.InsertOvertimeAttandanceLogAsync(employeeCode, overtimeName,
                                $"edit Fail Exception: {ex.Message}: {overtimeAttendanceID} - {row.Cells["OvertimeName"].Value} - {row.Cells["WorkDate"].Value} - {row.Cells["StartTime"].Value} -> {row.Cells["EndTime"].Value} - {row.Cells["Note"].Value}",
                                Convert.ToDateTime(row.Cells["WorkDate"].Value), startTime, endTime, note);
                            status_lb.Text = "Thất bại.";
                            status_lb.ForeColor = Color.Red;
                        }
                    }
                    break;
                }
            }
        }

        private async void createNew(DateTime workDate, TimeSpan startTime, TimeSpan endTime,
                                    int overtimeTypeID, string note)
        {

            List<string> employeeCodes = dataGV.SelectedRows.Cast<DataGridViewRow>().Where(r => !r.IsNewRow)
                                                                .Select(r => r.Cells["EmployeeCode"].Value?.ToString())
                                                                .Where(code => !string.IsNullOrWhiteSpace(code))
                                                                .Distinct()
                                                                .ToList();

            if (!employeeCodes.Any())
            {
                MessageBox.Show("Vui lòng chọn ít nhất một nhân viên.");
                return;
            }

            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    List<OvertimeAttendanceResult> results = await SQLManager_QLNS.Instance.InsertOvertimeAttendanceAsync(employeeCodes, workDate, startTime, endTime, overtimeTypeID, note);

                    DataRow[] rows = mOvertimeType.Select($"OvertimeTypeID = {overtimeTypeID}");                    

                    int departmentID = Convert.ToInt32(departmentGV.CurrentRow.Cells["DepartmentID"].Value);
                    string overtimeName = rows.Length > 0 ? rows[0]["OvertimeName"].ToString() : "";

                    _ = SQLManager_QLNS.Instance.InsertOvertimeAttandance_MultiEmployee_LogAsync(employeeCodes, overtimeName, $"Create {(results.Count > 0 ? "Success" : "Fail")}",workDate.Date, startTime, endTime, note);
                    if (results.Count > 0)
                    {
                        foreach (var item in results)
                        {
                            DataRow drToAdd = mOvertimeAttendamce_dt.NewRow();

                            var eRowData = mEmployee_dt.Select($"EmployeeCode = '{item.EmployeeCode}'")[0];
                            
                            overtimeAttendaceID_tb.Text = item.OvertimeAttendanceID.ToString();
                            drToAdd["OvertimeAttendanceID"] = item.OvertimeAttendanceID;
                            drToAdd["EmployeeCode"] = item.EmployeeCode;
                            drToAdd["EmployeeName"] = eRowData["FullName"].ToString();
                            drToAdd["WorkDate"] = workDate.Date;
                            drToAdd["StartTime"] = startTime;
                            drToAdd["EndTime"] = endTime;
                            drToAdd["OvertimeTypeID"] = overtimeTypeID;
                            drToAdd["Note"] = note;
                            drToAdd["HourWork"] = (endTime - startTime).TotalHours;
                            drToAdd["SalaryFactor"] = SQLStore_QLNS.Instance.GetOvertime_SalaryFactor(overtimeTypeID);
                            drToAdd["OvertimeName"] = overtimeName;
                            drToAdd["DepartmentID"] = departmentID;
                            string[] vietDays = { "CN", "T.2", "T.3", "T.4", "T.5", "T.6", "T.7" };
                            drToAdd["DayOfWeek"] = vietDays[(int)workDate.DayOfWeek];

                            var rowData = mOvertimeType.Select($"OvertimeTypeID = {overtimeTypeID}")[0];

                            string positionName = rowData["OvertimeName"].ToString();
                            double salaryFactor = Convert.ToDouble(rowData["SalaryFactor"]);

                            mOvertimeAttendamce_dt.Rows.Add(drToAdd);
                            mOvertimeAttendamce_dt.AcceptChanges();

                            attendanceGV.ClearSelection();
                            int rowIndex = attendanceGV.Rows.Count - 1;
                            attendanceGV.Rows[rowIndex].Selected = true;


                            status_lb.Text = "Thành công";
                            status_lb.ForeColor = Color.Green;

                            UpdateInfoUI();
                        }

                        NewBtn_Click(null, null);
                    }
                    else
                    {
                        status_lb.Text = "Thất bại";
                        status_lb.ForeColor = Color.Red;
                    }
                }
                catch (Exception ex)
                {
                    DataRow[] rows = mOvertimeType.Select($"OvertimeTypeID = {overtimeTypeID}");
                    string overtimeName = rows.Length > 0 ? rows[0]["OvertimeName"].ToString() : "";
                    _ = SQLManager_QLNS.Instance.InsertOvertimeAttandance_MultiEmployee_LogAsync(employeeCodes, overtimeName, $"Create Fail Exception: {ex.Message}", workDate.Date, startTime, endTime, note);
                    status_lb.Text = "Thất bại.";
                    status_lb.ForeColor = Color.Red;
                }

            }
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            if (overtimeType_cbb.SelectedValue == null || dataGV.CurrentRow == null || departmentGV.CurrentRow == null) return;

            int month = monthYearDtp.Value.Month;
            int year = monthYearDtp.Value.Year;

            
            DateTime workDate = monthYearDtp.Value;// new DateTime(year, month, workDate_dtp.Value.Day);

            //if (workDate.Year != year || month != workDate.Month)
            //{
            //    MessageBox.Show("Tháng hoặc Năm có vẫn đề", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}

            TimeSpan startTime = new TimeSpan(startTime_dtp.Value.Hour, startTime_dtp.Value.Minute, 0);
            TimeSpan endTime = new TimeSpan(endTime_dtp.Value.Hour, endTime_dtp.Value.Minute, 0);
            int overtimeAttendanceID = Convert.ToInt32(overtimeType_cbb.SelectedValue);
            string note = note_tb.Text;

            if (endTime < startTime)
            {
                MessageBox.Show("Sai Dữ Liệu, Kiểm Tra Lại!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (isPrintState)
            {
                InDSDonHang(2);
            }
            else
            {
                if (overtimeAttendaceID_tb.Text.Length != 0)
                {
                    string employeeCode = attendanceGV.CurrentRow.Cells["EmployeeCode"].Value?.ToString();
                    updateData(Convert.ToInt32(overtimeAttendaceID_tb.Text), employeeCode, startTime, endTime, overtimeAttendanceID, note);
                }
                else
                    createNew(workDate, startTime, endTime, overtimeAttendanceID, note);

                SQLStore_QLNS.Instance.removeAttendamce(workDate.Month, workDate.Year);
            }
        }

        private void NewBtn_Click(object sender, EventArgs e)
        {
            int month = monthYearDtp.Value.Month;
            int year = monthYearDtp.Value.Year;

            overtimeAttendaceID_tb.Text = "";
            note_tb.Text = "";
            //workDate_dtp.Value = new DateTime(year, month, workDate_dtp.Value.Day);
          //  workDate_dtp.Focus();
            startTime_dtp.Focus();
            info_gb.BackColor = newBtn.BackColor;
            edit_btn.Visible = false;
            newBtn.Visible = false;
            in_DS_TCa_btn.Visible = false;
            readOnly_btn.Visible = false;
            LuuThayDoiBtn.Visible = true;
            isNewState = true;
            isPrintState = false;
            inPreview_btn.Visible = false;
            LuuThayDoiBtn.Text = "Lưu Mới";
            SetUIReadOnly(false);
           // dataGV.MultiSelect = true;
        }

        private void In_DS_TCa_btn_Click(object sender, EventArgs e)
        {
            int month = monthYearDtp.Value.Month;
            int year = monthYearDtp.Value.Year;

            //workDate_dtp.Value = new DateTime(year, month, workDate_dtp.Value.Day);
            //workDate_dtp.Focus();
            info_gb.BackColor = in_DS_TCa_btn.BackColor;
            edit_btn.Visible = false;
            newBtn.Visible = false;
            readOnly_btn.Visible = false;
            LuuThayDoiBtn.Visible = true;
            in_DS_TCa_btn.Visible = false;
            inPreview_btn.Visible = true;
            isPrintState = true;
            isNewState = false;
            LuuThayDoiBtn.Text = "In";
            SetUIReadOnly(false);
            //dataGV.MultiSelect = true;
        }

        private void ReadOnly_btn_Click(object sender, EventArgs e)
        {
            edit_btn.Visible = true;
            newBtn.Visible = true;
            in_DS_TCa_btn.Visible = true;
            readOnly_btn.Visible = false;
            LuuThayDoiBtn.Visible = false;
            info_gb.BackColor = Color.DarkGray;
            inPreview_btn.Visible = false;
            isNewState = false;
            isPrintState = false;
            //dataGV.MultiSelect = false;
            SetUIReadOnly(true);
            attendanceGV_CellClick(null, null);
        }

        private void Edit_btn_Click(object sender, EventArgs e)
        {
            edit_btn.Visible = false;
            newBtn.Visible = false;
            in_DS_TCa_btn.Visible = false;
           // readOnly_btn.Visible = true;
            LuuThayDoiBtn.Visible = true;
            inPreview_btn.Visible = false;
            isNewState = false;
            isPrintState = false;
            info_gb.BackColor = edit_btn.BackColor;
            LuuThayDoiBtn.Text = "Lưu C.Sửa";
            //dataGV.MultiSelect = false;
            SetUIReadOnly(false);
        }

        private void SetUIReadOnly(bool isReadOnly)
        {
           // workDate_dtp.Enabled = !isReadOnly;
            startTime_dtp.Enabled = !isReadOnly;
            endTime_dtp.Enabled = !isReadOnly;
            overtimeType_cbb.Enabled = !isReadOnly;
            note_tb.ReadOnly = isReadOnly;
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

            // 👉 XỬ LÝ Ở ĐÂY
            HandleMonthYearChanged();
        }

        private async void HandleMonthYearChanged()
        {
            int month = monthYearDtp.Value.Month;
            int year = monthYearDtp.Value.Year;

            if (month != mCurMonth || year != mCurYear)
            {
                await Task.Delay(50);
                LoadingOverlay loadingOverlay = new LoadingOverlay(this);
                loadingOverlay.Show();
                await Task.Delay(200);

                var overtimeAttendamceTask = SQLStore_QLNS.Instance.GetOvertimeAttendamceAsync(month, year);
                var OvertimeAttendanceLogTask = SQLStore_QLNS.Instance.GetOvertimeAttendanceLogAsync(month, year);

                await Task.WhenAll(overtimeAttendamceTask, OvertimeAttendanceLogTask);

                mOvertimeAttendamce_dt = overtimeAttendamceTask.Result;
                mLogDV = new DataView(OvertimeAttendanceLogTask.Result);
                log_GV.DataSource = mLogDV;
                Attendamce();

                bool isLock = await SQLStore_QLNS.Instance.IsSalaryLockAsync(month, year);

                newBtn.Visible = !isLock;
                edit_btn.Visible = !isLock;

                ReadOnly_btn_Click(null, null);
                await Task.Delay(100);
                loadingOverlay.Hide();
            }

            mCurMonth = month;
            mCurYear = year;

            monthYearLabel.Text = $"Tháng {month}/{year}";
            DepartmentGV_SelectionChanged(null, null);
        }

        private void InDSDonHang(int mode)
        {
            if (departmentGV.CurrentRow == null) return;

            string note = note_tb.Text;
            if(string.IsNullOrEmpty(note.Trim()) == true)
            {
                MessageBox.Show("Chưa ghi mục đích tăng ca", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
           // int month = monthYearDtp.Value.Month;
           // int year = monthYearDtp.Value.Year;
            DateTime targetDate = monthYearDtp.Value.Date;// new DateTime(year, month, workDate_dtp.Value.Day);
            int departmentID = Convert.ToInt32(departmentGV.CurrentRow.Cells["DepartmentID"].Value);
            string departmentName = departmentGV.CurrentRow.Cells["DepartmentName"].Value.ToString();

            var query = from ot in mOvertimeAttendamce_dt.AsEnumerable()
                join emp in mEmployee_dt.AsEnumerable() on ot.Field<string>("EmployeeCode") equals emp.Field<string>("EmployeeCode")
                where departmentID == emp.Field<int>("DepartmentID") && ot.Field<DateTime>("WorkDate").Date == targetDate
                select new
                {
                    EmployeeCode = emp.Field<string>("EmployeeCode"),
                    EmployeeName = emp.Field<string>("FullName"),
                    StartTime = ot.Field<TimeSpan>("StartTime"),
                    EndTime = ot.Field<TimeSpan>("EndTime"),
                    HourWork = ot.Field<decimal>("HourWork"),
                    Note = ot.Field<string>("Note")
                };

            DataTable result_dt = new DataTable();
            result_dt.Columns.Add("EmployeeCode", typeof(string));
            result_dt.Columns.Add("EmployeeName", typeof(string));
            result_dt.Columns.Add("StartTime", typeof(TimeSpan));
            result_dt.Columns.Add("EndTime", typeof(TimeSpan));
            result_dt.Columns.Add("HourWork", typeof(decimal));
            result_dt.Columns.Add("Note", typeof(string));

            foreach (var item in query)
            {
                result_dt.Rows.Add(
                    item.EmployeeCode,
                    item.EmployeeName,
                    item.StartTime,
                    item.EndTime,
                    item.HourWork,
                    item.Note
                );
            }

            TimeSpan startTime = new TimeSpan(startTime_dtp.Value.Hour, startTime_dtp.Value.Minute, 0);
            TimeSpan endTime = new TimeSpan(endTime_dtp.Value.Hour, endTime_dtp.Value.Minute, 0);
            int overtimeAttendanceID = Convert.ToInt32(overtimeType_cbb.SelectedValue);
            

            DataRow[] rows = mOvertimeType.Select($"OvertimeTypeID = {overtimeAttendanceID}");
            string overtimeName = rows.Length > 0 ? rows[0]["OvertimeName"].ToString() : "";

            DSTangCa_Printer printer = new DSTangCa_Printer(result_dt, note, departmentName, overtimeName, startTime, endTime, targetDate);

            if(mode == 1)
                printer.PrintPreview(this);
            else
                printer.PrintDirect();
        }

        private void InPreview_btn_Click(object sender, EventArgs e)
        {
            InDSDonHang(1);
        }

        private async void deleteOvertimeAttendanceID(int overtimeAttendanceID)
        {
            foreach (DataRow row in mOvertimeAttendamce_dt.Rows)
            {
                
                string employeeCode = Convert.ToString(row["EmployeeCode"]);
                string overtimeName = Convert.ToString(row["OvertimeName"]);
                TimeSpan startTime = (TimeSpan)row["StartTime"];
                TimeSpan endTime = (TimeSpan)row["EndTime"];
                int orderId = Convert.ToInt32(row["OvertimeAttendanceID"]);
                if (overtimeAttendanceID.CompareTo(orderId) == 0)
                {                   
                    try
                    {
                        bool isSuccess = await SQLManager_QLNS.Instance.deleteOvertimeAttendanceAsync(overtimeAttendanceID);
                                                
                        _ = SQLManager_QLNS.Instance.InsertOvertimeAttandanceLogAsync(employeeCode, overtimeName,
                               $"Delete {(isSuccess == true ? "Success" : "Fail")}",
                               Convert.ToDateTime(row["WorkDate"]), startTime, endTime, "Delete");

                        if (isSuccess)
                        {
                            status_lb.Text = "Thành công.";
                            status_lb.ForeColor = System.Drawing.Color.Green;

                            // Xóa row khỏi DataTable
                            mOvertimeAttendamce_dt.Rows.Remove(row);
                            mOvertimeAttendamce_dt.AcceptChanges();

                        }
                        else
                        {
                            status_lb.Text = "Thất bại.";
                            status_lb.ForeColor = System.Drawing.Color.Red;
                        }
                    }
                    catch (Exception ex)
                    {
                        status_lb.Text = "Thất bại.";
                        status_lb.ForeColor = System.Drawing.Color.Red;
                        _ = SQLManager_QLNS.Instance.InsertOvertimeAttandanceLogAsync(employeeCode, overtimeName,
                               $"Delete Fail Exception: {ex.Message}",
                               Convert.ToDateTime(row["WorkDate"]), startTime, endTime, "Delete");
                    }

                    break; // Dừng vòng lặp sau khi xóa
                }
            }
        }

        void UpdateInfoUI()
        {
            int count = 0;
            decimal totalHW = 0;
            foreach(DataGridViewRow row in attendanceGV.Rows)
            {
                decimal hw = Convert.ToDecimal(row.Cells["HourWork"].Value);
                if (hw > 0)
                {
                    totalHW += hw;
                    count++;
                }
            }

            count_label.Text = count.ToString();
            totalHour_label.Text = totalHW.ToString();
        }

        private void AttendanceMonth_CB_CheckedChanged(object sender, EventArgs e)
        {
            DepartmentGV_SelectionChanged(null, null);
        }
    }
}
