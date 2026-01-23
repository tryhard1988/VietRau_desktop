using DocumentFormat.OpenXml.Bibliography;
using Microsoft.Win32;
using RauViet.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using zkemkeeper;
using DataTable = System.Data.DataTable;

namespace RauViet.ui
{
    public partial class Attendance : Form
    {
        DataTable mAttendamce_dt, mEmployee_dt, mHoliday_dt, mLeaveAttendance_dt;
        DataView mlogDV;
        Dictionary<string, (string PositionCode, string ContractTypeCode)> employeeDict;
        LoadingOverlay loadingOverlay;
        int mCurrentMonth = -1;
        int mCurrentYear = -1;
        object oldValue;
        CZKEM axCZKEM = new CZKEM();
        int machineNumber = 1;
        public Attendance()
        {
            InitializeComponent();
            this.KeyPreview = true;
            monthYearDtp.Format = DateTimePickerFormat.Custom;
            monthYearDtp.CustomFormat = "MM/yyyy";
            monthYearDtp.ShowUpDown = true;
            monthYearDtp.Value = DateTime.Now.AddMonths(-1);

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            status_lb.Text = "";

            this.holiday_btn.Click += Holiday_btn_Click; ;

            attendanceGV.CellFormatting += AttandaceGV_CellFormatting;
            attendanceGV.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.attendanceGV_EditingControlShowing);

            attendanceGV.CellBeginEdit += AttendanceGV_CellBeginEdit;
            attendanceGV.CellEndEdit += AttendanceGV_CellEndEdit;

            excelAttendance_btn.Click += ExcelAttendance_btn_Click;

            loadAttandance_btn.Click += LoadAttandance_btn_Click;

            attendanceGV.CellParsing += AttendanceGV_CellParsing;
            attendanceGV.SelectionChanged += AttendanceGV_SelectionChanged;

            this.KeyDown += Attendance_KeyDown;
        }

        private void Attendance_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                int month = monthYearDtp.Value.Month;
                int year = monthYearDtp.Value.Year;
                SQLStore_QLNS.Instance.removeAttendamce(month, year);                
                LoadAttandance_btn_Click(null, null);
            }
        }

        public async void ShowData()
        {
            await Task.Delay(50);
            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Show();

            try
            {
                dataGV.SelectionChanged -= this.dataGV_CellClick;
                int month = monthYearDtp.Value.Month;
                int year = monthYearDtp.Value.Year;
                // Chạy truy vấn trên thread riêng
                string[] keepColumns = { "EmployeeCode", "FullName", "PositionName", "ContractTypeName", "PositionCode", "ContractTypeCode" };
                var employeesTask = SQLStore_QLNS.Instance.GetEmployeesAsync(keepColumns);
                var attendamceTask = SQLStore_QLNS.Instance.GetAttendamceAsync(null, month, year);
                var holidayTask = SQLStore_QLNS.Instance.GetHolidaysAsync(month, year);
                var leaveAttendanceTask = SQLStore_QLNS.Instance.GetLeaveAttendancesAsyn(year);
                var attendanceLogTask = SQLStore_QLNS.Instance.GetAttendanceLogAsync(month, year);
                await Task.WhenAll(employeesTask, attendamceTask, holidayTask, leaveAttendanceTask, attendanceLogTask);
                mEmployee_dt = employeesTask.Result;
                mAttendamce_dt = attendamceTask.Result;
                mHoliday_dt = holidayTask.Result;
                mLeaveAttendance_dt = leaveAttendanceTask.Result;
                mlogDV = new DataView(attendanceLogTask.Result);

                mCurrentMonth = month;
                mCurrentYear = year;

                employeeDict = mEmployee_dt.AsEnumerable()
                                .ToDictionary(
                                    r => r.Field<string>("EmployeeCode"),
                                    r => (
                                        PositionCode: r.Field<string>("PositionCode"),
                                        ContractTypeCode: r.Field<string>("ContractTypeCode")
                                    )
                                );


                mEmployee_dt.Columns.Add(new DataColumn("TotalWorkingHour", typeof(double)));
                mEmployee_dt.Columns.Add(new DataColumn("TotalWorkingDay", typeof(int)));

                dataGV.DataSource = mEmployee_dt;
                Utils.HideColumns(dataGV, new[] { "PositionCode", "ContractTypeCode" });

                Utils.SetGridHeaders(dataGV, new System.Collections.Generic.Dictionary<string, string> {
                    {"EmployeeCode", "Mã Nhân Viên" },
                    {"FullName", "Tên Nhân Viên" },
                    {"TotalWorkingHour", "Tổng G.Làm" },
                    {"TotalWorkingDay", "Tổng N.Làm" },
                    {"ContractTypeName", "Loại H.Đồng" },
                    {"PositionName", "Chức Vụ" }
                });

                dataGV.Columns["EmployeeCode"].Width = 50;
                dataGV.Columns["FullName"].Width = 160;
                dataGV.Columns["TotalWorkingHour"].Width = 50;
                dataGV.Columns["TotalWorkingDay"].Width = 50;
                dataGV.Columns["ContractTypeName"].Width = 70;
                dataGV.Columns["PositionName"].Width = 70;

                dataGV.Width = 500;

                if (dataGV.Rows.Count > 0)
                {
                    dataGV.ClearSelection();
                    dataGV.Rows[0].Selected = true;
                }

                log_GV.DataSource = mlogDV;
                Utils.HideColumns(log_GV, new[] { "LogID", "EmployeeCode" });
                Attendamce(month, year);
                attendanceGV.DataSource = mAttendamce_dt;
                Utils.HideColumns(attendanceGV, new[] { "EmployeeCode", "LeaveHours" });
                Utils.SetGridHeaders(attendanceGV, new System.Collections.Generic.Dictionary<string, string> {
                    {"WorkDate", "Ngày Làm" },
                    {"WorkingHours", "Số G.Làm" },
                    {"DayOfWeek", "Thứ" },
                    {"AttendanceLog", "Ra/Vào Cổng" },
                    {"LeaveTypeName", "Nghỉ(Nếu Có)" },
                    {"OvertimeName", "Tăng Ca" },
                    {"Note", "Ghi Chú" }
                });

                Utils.SetGridHeaders(log_GV, new System.Collections.Generic.Dictionary<string, string> {
                    {"CreatedAt", "Ngày thay đổi" },
                    {"ActionBy", "Người thay đổi" },
                    {"Description", "Hành động" },
                    {"WorkDate", "Ngày Làm" },
                    {"WorkingHours", "Giờ Làm" }
                });


                attendanceGV.Columns["WorkDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
                attendanceGV.Columns["WorkingHours"].DefaultCellStyle.Format = "0.##";

                Utils.SetGridWidths(attendanceGV, new System.Collections.Generic.Dictionary<string, int> {
                    {"DayOfWeek", 40},
                    {"WorkDate", 70},
                    {"LeaveTypeName", 100},
                    {"OvertimeName", 120},
                    {"WorkingHours", 50},
                    {"AttendanceLog", 250},
                    {"Note", 150},
                });

                attendanceGV.Columns["AttendanceLog"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                attendanceGV.Columns["OvertimeName"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                attendanceGV.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                attendanceGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                bool isLock = await SQLStore_QLNS.Instance.IsSalaryLockAsync(month, year);
                attendanceGV.ReadOnly = isLock;
                attendanceGV.Columns["WorkingHours"].ReadOnly = false;
                attendanceGV.Columns["Note"].ReadOnly = false;
                attendanceGV.Columns["DayOfWeek"].ReadOnly = true;
                attendanceGV.Columns["WorkDate"].ReadOnly = true;
                attendanceGV.Columns["AttendanceLog"].ReadOnly = true;

                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                log_GV.Columns["CreatedAt"].Width = 120;
                log_GV.Columns["ActionBy"].Width = 150;
                log_GV.Columns["WorkDate"].Width = 100;
                log_GV.Columns["WorkingHours"].Width = 100;
                log_GV.Columns["Description"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                log_GV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGV.SelectionChanged += this.dataGV_CellClick;                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                status_lb.Text = "Thất bại.";
                status_lb.ForeColor = Color.Red;
            }
            finally
            {
                await Task.Delay(100);
                loadingOverlay.Hide();
            }

            
        }

        private async void Attendamce(int month, int year)
        {

            bool isLock = await SQLStore_QLNS.Instance.IsSalaryLockAsync(month, year);
            attendanceGV.ReadOnly = isLock;

            cal_WorkingHour_WorkingDay();            

            if (dataGV.CurrentRow != null)
            {
                int selectedIndex = dataGV.CurrentRow?.Index ?? -1;
                UpdateRightUI(selectedIndex);
            }


        }

        //private async void CalWorkHour_btn_Click(object sender, EventArgs e)
        //{
        //    List<(string EmployeeCode, DateTime WorkDate, decimal WorkingHours, string Note, string log)> attendanceData = new List<(string, DateTime, decimal, string, string)>();
        //    foreach (DataRow dr in mAttendamce_dt.Rows)
        //    {
        //        if (double.TryParse(dr["LeaveHours"]?.ToString(), out double leaveHour))
        //        {
        //            dr["WorkingHours"] = Math.Max(0, 8 - leaveHour);                    

        //            string employeeCode = Convert.ToString(dr["EmployeeCode"]);
        //            DateTime workDate = Convert.ToDateTime(dr["WorkDate"]);
        //            decimal workingHours = Convert.ToDecimal(dr["WorkingHours"]);
        //            string note = Convert.ToString(dr["Note"]);
        //            string attendanceLog = Convert.ToString(dr["AttendanceLog"]);

        //            attendanceData.Add((employeeCode, workDate, workingHours, note, attendanceLog));                    
        //        }
        //    }

        //    try
        //    {
        //        Boolean iSuccess = await SQLManager.Instance.UpsertAttendanceBatchAsync(attendanceData);
        //        if (!iSuccess)
        //        {
        //            MessageBox.Show("Cập nhật thất bại!");
        //        }
        //        else
        //        {
        //            status_lb.Text = "Thành Công.";
        //            status_lb.ForeColor = Color.Blue;
        //        }
        //    }
        //    catch
        //    {
        //        MessageBox.Show("Thất Bại ?", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}

        private async void AttandaceGV_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            
            if (attendanceGV.Columns[e.ColumnIndex].Name == "WorkingHours" ||
                    attendanceGV.Columns[e.ColumnIndex].Name == "Note")
            {
                e.CellStyle.BackColor = System.Drawing.Color.LightGray;
            }
            else if (attendanceGV.Columns[e.ColumnIndex].Name == "DayOfWeek")
            {
                if (e.Value != null && e.Value.ToString() == "CN")
                {
                    e.CellStyle.BackColor = Color.LightPink;
                    e.CellStyle.ForeColor = Color.Red;
                }
            }
            else if (attendanceGV.Columns[e.ColumnIndex].Name == "WorkDate")
            {
                if (e.Value != null)
                {
                    bool isHoliday = mHoliday_dt.AsEnumerable().Any(r => r.Field<DateTime>("HolidayDate").Date == Convert.ToDateTime(e.Value).Date);
                    if (isHoliday)
                    {
                        e.CellStyle.BackColor = Color.LightPink;
                        e.CellStyle.ForeColor = Color.Red;
                    }
                }
            }

        }

        private void attendanceGV_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            System.Windows.Forms.TextBox tb = e.Control as System.Windows.Forms.TextBox;
            if (tb == null) return;

            tb.KeyPress -= Tb_KeyPress_OnlyNumber;

            var colName = attendanceGV.CurrentCell.OwningColumn.Name;

            if (colName == "WorkingHours")
            {
                tb.KeyPress += Tb_KeyPress_OnlyNumber;
            }
        }

        private void Tb_KeyPress_OnlyNumber(object sender, KeyPressEventArgs e)
        {
            System.Windows.Forms.TextBox tb = sender as System.Windows.Forms.TextBox;

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

        private async void ExcelAttendance_btn_Click(object sender, EventArgs e)
        {
            await Task.Delay(50);
            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Message = "Đang xử lý ...";
            loadingOverlay.Show();
            await Task.Delay(200);
            try
            {
                using (System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog())
                {
                    ofd.Title = "Chọn file Excel";
                    ofd.Filter = "Excel Files|*.xlsx;*.xls|All Files|*.*";
                    ofd.Multiselect = false; // chỉ cho chọn 1 file

                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = ofd.FileName;
                        System.Data.DataTable excelData = Utils.LoadExcel_NoHeader(filePath);

                        List<(string EmployeeCode, DateTime WorkDate, decimal WorkingHours, string Note, string log)> attendanceData = new List<(string, DateTime, decimal, string, string)>();
                        List<(string EmployeeCode, string Description, DateTime WorkDate, decimal WorkingHours, string Note, string ActionBy)> logs = new List<(string, string, DateTime, decimal, string, string)>();
                        TimeSpan startTime_Morning = new TimeSpan(9, 30, 0);
                        TimeSpan startTime_AfterNoon = new TimeSpan(13, 20, 0);
                        int month = -1;
                        int year = -1;

                        foreach (DataRow edr in excelData.Rows)
                        {
                            if (!int.TryParse(edr["Column2"].ToString(), out int employeeID))
                                continue;

                            string employeeCode = $"VR{employeeID:D4}";
                            if (!employeeDict.ContainsKey(employeeCode))
                                continue;

                            string positionCode = employeeDict[employeeCode].PositionCode;
                            string contractTypeCode = employeeDict[employeeCode].ContractTypeCode;

                            if (contractTypeCode.CompareTo("t_vu") == 0)
                                continue;

                            DateTime workDate = Convert.ToDateTime(edr["Column4"]).Date;

                            if ((month != -1 && month != workDate.Month) ||
                                year != -1 && year != workDate.Year)
                            {
                                month = -1;
                                break;
                            }
                            else
                            {
                                if (month == -1)
                                {
                                    var holidayTask = SQLManager_QLNS.Instance.GetHolidayAsync(workDate.Month, workDate.Year);
                                    await Task.WhenAll(holidayTask);
                                    mHoliday_dt = holidayTask.Result;
                                }
                                month = workDate.Month;
                                year = workDate.Year;
                            }
                            bool isWork_Morning = false;
                            bool isWork_Afternoon = false;

                            string log = "";
                            for (int colIndex = 5; colIndex < excelData.Columns.Count; colIndex++)
                            {
                                var cellValue = edr[colIndex];
                                if (cellValue != null && cellValue != DBNull.Value)
                                {
                                    if (TimeSpan.TryParse(cellValue.ToString(), out TimeSpan checkInTime))
                                    {
                                        if (log.CompareTo("") != 0) log += " => ";
                                        log += checkInTime;

                                        if (checkInTime < startTime_Morning)
                                            isWork_Morning = true;
                                        else if (checkInTime > startTime_AfterNoon)
                                            isWork_Afternoon = true;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }

                            decimal workingHours = 0m;
                            bool isSunDay = workDate.DayOfWeek == DayOfWeek.Sunday;
                            bool isHoliday = mHoliday_dt.AsEnumerable().Any(r => r.Field<DateTime>("HolidayDate").Date == workDate.Date);
                            string note = "";
                            if (positionCode.CompareTo("bao_ve") == 0)
                            {
                                workingHours = 12m;
                            }
                            else if (isHoliday)
                            {
                                workingHours = 0;
                            }
                            else if (!isSunDay)
                            {
                                if (isWork_Morning) workingHours += 4.5m;
                                if (isWork_Afternoon) workingHours += 3.5m;
                            }

                            attendanceData.Add((employeeCode, workDate, workingHours, note, log));
                            logs.Add((employeeCode, "File Excel", workDate, workingHours, note, UserManager.Instance.fullName));
                        }

                        if (month == -1 || year == -1)
                        {
                            MessageBox.Show("Dữ Liệu Ngày Làm Có Vấn Đề Hoặc File Đang Mở ?", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            bool isLock = await SQLStore_QLNS.Instance.IsSalaryLockAsync(month, year);
                            if (isLock)
                            {
                                await Task.Delay(200);
                                loadingOverlay.Hide();
                                MessageBox.Show($"File Excel Là Dữ Liệu Của Tháng {month}/{year} \n Nhưng Tháng Này Đã Bị Khóa Rồi ?", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            //tạo dữ liệu cho nhân viên thời vụ
                            var thoiVuList = employeeDict
                                                .Where(kv => kv.Value.ContractTypeCode.Equals("thoi_vu", StringComparison.OrdinalIgnoreCase))
                                                .Select(kv => kv.Key).ToList();

                            int daysInMonth = DateTime.DaysInMonth(year, month);

                            foreach (var employeeCode in thoiVuList)
                            {
                                for (int day = 1; day <= daysInMonth; day++)
                                {
                                    var workDate = new DateTime(year, month, day);
                                    if (workDate.DayOfWeek != DayOfWeek.Sunday)
                                    {
                                        attendanceData.Add((employeeCode, workDate, 8, "", ""));
                                    }
                                }
                            }


                            DialogResult dialogResult = MessageBox.Show($"File Excel Là Dữ Liệu Của Tháng {month}/{year} \nSai Là Không sửa lại được đâu đó ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                            if (dialogResult == DialogResult.Yes)
                            {
                                try
                                {
                                    bool isSuccess = await SQLManager_QLNS.Instance.DeleteAttendanceByMonthYearAsync(month, year);
                                    if(!isSuccess)
                                    {
                                        await Task.Delay(200);
                                        loadingOverlay.Hide();
                                        MessageBox.Show("Xóa dữ liệu cũ Thất Bại ?", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        return;
                                    }
                                    mAttendamce_dt.Clear();
                                    isSuccess = await SQLManager_QLNS.Instance.UpsertAttendanceBatchAsync(attendanceData);
                                    if (isSuccess)
                                    {
                                        var updatedLogs = logs
                                                .Select(item =>
                                                    (
                                                        item.EmployeeCode,
                                                        Description: item.Description + " Success",   // <— sửa ở đây
                                                        item.WorkDate,
                                                        item.WorkingHours,
                                                        item.Note,
                                                        item.ActionBy
                                                    )
                                                ).ToList();

                                        _ = SQLManager_QLNS.Instance.InsertAttendanceLogListAsync( updatedLogs );

                                        monthYearDtp.Value = new DateTime(year, month, 1);

                                        await AddMissingEmployees(month, year);

                                        SQLStore_QLNS.Instance.removeAttendamce(month, year);
                                        var attendamceTask = SQLStore_QLNS.Instance.GetAttendamceAsync(null, month, year);
                                        await Task.WhenAll(attendamceTask);
                                        mAttendamce_dt = attendamceTask.Result;

                                        Attendamce(month, year);
                                    }
                                    else
                                    {
                                        var updatedLogs = logs
                                                .Select(item =>
                                                    (
                                                        item.EmployeeCode,
                                                        Description: item.Description + " Fail",   // <— sửa ở đây
                                                        item.WorkDate,
                                                        item.WorkingHours,
                                                        item.Note,
                                                        item.ActionBy
                                                    )
                                                ).ToList();

                                        _ = SQLManager_QLNS.Instance.InsertAttendanceLogListAsync(updatedLogs);

                                        MessageBox.Show("Thất Bại ?", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    await Task.Delay(200);
                                    loadingOverlay.Hide();
                                    Console.WriteLine("ERROR: " + ex.ToString());

                                    var updatedLogs = logs
                                                .Select(item =>
                                                    (
                                                        item.EmployeeCode,
                                                        Description: item.Description + " Fail Exception: " + ex.Message,   // <— sửa ở đây
                                                        item.WorkDate,
                                                        item.WorkingHours,
                                                        item.Note,
                                                        item.ActionBy
                                                    )
                                                ).ToList();

                                    _ = SQLManager_QLNS.Instance.InsertAttendanceLogListAsync(updatedLogs);

                                    MessageBox.Show("Thất Bại ?", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                    }

                    await Task.Delay(200);
                    loadingOverlay.Hide();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.ToString());
                MessageBox.Show("Thất Bại ?", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                await Task.Delay(200);
                loadingOverlay.Hide();
            }
        }

        private async void LoadAttandance_btn_Click(object sender, EventArgs e)
        {
            await Task.Delay(50);
            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Show();
            await Task.Delay(200);
            int month = monthYearDtp.Value.Month;
            int year = monthYearDtp.Value.Year;

            var attendamceTask = SQLStore_QLNS.Instance.GetAttendamceAsync(null, month, year);
            var holidayTask = SQLStore_QLNS.Instance.GetHolidaysAsync(month, year);
            var leaveAttendanceTask = SQLStore_QLNS.Instance.GetLeaveAttendancesAsyn(year);
            var attendanceLogTask = SQLStore_QLNS.Instance.GetAttendanceLogAsync(month, year);
            await Task.WhenAll(attendamceTask, holidayTask, leaveAttendanceTask, attendanceLogTask);

            mCurrentYear = year;
            mCurrentMonth = month;

            mAttendamce_dt = attendamceTask.Result;
            mHoliday_dt = holidayTask.Result;
            mLeaveAttendance_dt = leaveAttendanceTask.Result;
            mlogDV = new DataView(attendanceLogTask.Result);
            log_GV.DataSource = mlogDV;

            Attendamce(month, year);
            await Task.Delay(500);
            loadingOverlay.Hide();
        }

        private async void dataGV_CellClick(object sender, EventArgs e)
        {
            status_lb.Text = "";
            if (dataGV.CurrentRow == null) return;
            int rowIndex = dataGV.CurrentRow.Index;
            if (rowIndex < 0) return;

            string employeeCode = dataGV.Rows[rowIndex].Cells["EmployeeCode"].Value.ToString();

            // Tính toán dữ liệu trên thread pool
            DataView dv = await Task.Run(() =>
            {
                DataView view = new DataView(mAttendamce_dt);
                view.RowFilter = $"EmployeeCode = '{employeeCode}'";
                return view;
            });
            attendanceGV.DataSource = dv;

            // Update UI trên UI thread
            
        }

        private void AttendanceGV_SelectionChanged(object sender, EventArgs e)
        {
            if (attendanceGV.CurrentRow == null) return;
            int rowIndex = attendanceGV.CurrentRow.Index;
            if (rowIndex < 0) return;

            var row = attendanceGV.Rows[rowIndex];
            string employeeCode = row.Cells["EmployeeCode"].Value.ToString();
            DateTime workDate = Convert.ToDateTime(row.Cells["WorkDate"].Value);

            mlogDV.RowFilter = $"EmployeeCode = '{employeeCode}' AND WorkDate = #{workDate:MM/dd/yyyy}#";

        }

        private void cal_WorkingHour_WorkingDay()
        {
            var summaryDict = mAttendamce_dt.AsEnumerable() .GroupBy(r => r.Field<string>("EmployeeCode"))
            .ToDictionary(
                g => g.Key,
                g => new
                {
                    TotalWorkingHours = g.Sum(r => r.Field<decimal?>("WorkingHours") ?? 0m),
                    WorkingDays = g.Count(r => (r.Field<decimal?>("WorkingHours") ?? 0m) > 0)
                });

            foreach (DataRow dr in mEmployee_dt.Rows)
            {
                string employeeCode = dr["EmployeeCode"].ToString();

                if (summaryDict.TryGetValue(employeeCode, out var s))
                {
                    dr["TotalWorkingHour"] = s.TotalWorkingHours;
                    dr["TotalWorkingDay"] = s.WorkingDays;
                }
                else
                {
                    dr["TotalWorkingHour"] = 0;
                    dr["TotalWorkingDay"] = 0;
                }
            }
        } 

        private void UpdateRightUI(int index)
        {
            var cells = dataGV.Rows[index].Cells;
            //int employeeID = Convert.ToInt32(cells["EmployeeID"].Value);
            string employeeCode = cells["EmployeeCode"].Value.ToString();

            DataView dv = new DataView(mAttendamce_dt);
            dv.RowFilter = $"EmployeeCode = '{employeeCode}'";            
            attendanceGV.DataSource = dv;

            status_lb.Text = "";
        }

        private async void AttendanceGV_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            oldValue = attendanceGV.CurrentCell?.Value;
            bool isLock = await SQLStore_QLNS.Instance.IsSalaryLockAsync(mCurrentMonth, mCurrentYear);
            if (isLock)
                e.Cancel = true;

            status_lb.Text = "";
        }

        private async void AttendanceGV_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                var columnName = attendanceGV.Columns[e.ColumnIndex].Name;
                object newValue = attendanceGV.CurrentCell?.Value;//.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString() ?? "";

                if (object.Equals(oldValue, newValue)) return;

                var row = attendanceGV.Rows[e.RowIndex];

                List<(string EmployeeCode, DateTime WorkDate, decimal WorkingHours, string Note, string log)> attendanceData = new List<(string, DateTime, decimal, string, string)>();

                string employeeCode = Convert.ToString(row.Cells["EmployeeCode"].Value);
                DateTime workDate = Convert.ToDateTime(row.Cells["WorkDate"].Value);
                decimal workingHours = Convert.ToDecimal(row.Cells["WorkingHours"].Value);
                string note = Convert.ToString(row.Cells["Note"].Value);
                string attendanceLog = Convert.ToString(row.Cells["AttendanceLog"].Value);

                attendanceData.Add((employeeCode, workDate, workingHours, note, attendanceLog));

                try
                {
                    Boolean iSuccess = await SQLManager_QLNS.Instance.UpsertAttendanceBatchAsync(attendanceData);

                    List<(string EmployeeCode, string Description, DateTime WorkDate, decimal WorkingHours, string Note, string ActionBy)> logs = new List<(string, string, DateTime, decimal, string, string)>();
                    logs.Add((employeeCode, $"Edit {columnName}: {oldValue} {(iSuccess ? "Success" : "Fail")}", workDate, workingHours, note, UserManager.Instance.fullName));

                    _ = SQLManager_QLNS.Instance.InsertAttendanceLogListAsync(logs);
                    if (!iSuccess)
                    {                        
                        MessageBox.Show("Cập nhật thất bại!");
                    }
                    else
                    {
                        status_lb.Text = "Thành Công.";
                        status_lb.ForeColor = Color.Green;
                        cal_WorkingHour_WorkingDay();
                    }
                }
                catch (Exception ex)
                {
                    List<(string EmployeeCode, string Description, DateTime WorkDate, decimal WorkingHours, string Note, string ActionBy)> logs = new List<(string, string, DateTime, decimal, string, string)>();
                    logs.Add((employeeCode, $"Edit {columnName}: {oldValue} Fail Exception {ex.Message}", workDate, workingHours, note, UserManager.Instance.fullName));
                    _ = SQLManager_QLNS.Instance.InsertAttendanceLogListAsync(logs);

                    MessageBox.Show("Thất Bại ?", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        //private async void RonaldMachine_btn_Click_1(object sender, EventArgs e)
        //{
        //    using (System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog())
        //    {
        //        ofd.Title = "Chọn file Excel";
        //        ofd.Filter = "Excel Files|*.xlsx;*.xls|All Files|*.*";
        //        ofd.Multiselect = false; // chỉ cho chọn 1 file

        //        if (ofd.ShowDialog() == DialogResult.OK)
        //        {
        //            string filePath = ofd.FileName;
        //            System.Data.DataTable excelData = Utils.LoadExcel_NoHeader(filePath);
        //            try
        //            {
        //                foreach (DataRow edr in excelData.Rows)
        //                {
        //                    string empCode = edr["Column1"].ToString();
        //                    bool exists = mEmployee_dt.AsEnumerable().Any(r => r.Field<string>("EmployeeCode") == empCode);
        //                    if (!exists) continue;

        //                    for (int i = 1; i <= 31; i++)
        //                    {

        //                        decimal wh = 0;
        //                        decimal.TryParse(edr[$"Column{i + 1}"].ToString(), out wh);
                                
        //                            await SQLManager_QLNS.Instance.updateWorkHourAsync(empCode, new DateTime(2025, 12, i), wh);
                                


        //                    }

        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                MessageBox.Show("có lỗi " + ex.Message);
        //            }
        //            MessageBox.Show("xong");
        //        }
        //    }
        //}

        private void AttendanceGV_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
        {
            if (attendanceGV.Columns[e.ColumnIndex].Name == "WorkingHours" && e.Value != null)
            {
                // Cho phép nhập 7,5 hoặc 7.5
                string val = e.Value.ToString().Replace(',', '.');

                if (decimal.TryParse(val, System.Globalization.NumberStyles.Any,
                                     System.Globalization.CultureInfo.InvariantCulture, out decimal d))
                {
                    e.Value = d;
                    e.ParsingApplied = true; // báo DataGridView giá trị đã parse xong
                }
            }
        }

        public async Task AddMissingEmployees(int month, int year)
        {
            if (mEmployee_dt == null || mAttendamce_dt == null)
                throw new ArgumentNullException("DataTable không được null");

            int daysInMonth = DateTime.DaysInMonth(year, month);

            // Lấy danh sách EmployeeCode đã có trong Attendance
            var existingCodes = mAttendamce_dt.AsEnumerable()
                                             .Select(r => r.Field<string>("EmployeeCode"))
                                             .Distinct()
                                             .ToHashSet();

            // Lấy danh sách nhân viên chưa có trong Attendance
            var missingEmployees = mEmployee_dt.AsEnumerable()
                                              .Where(emp => !existingCodes.Contains(emp.Field<string>("EmployeeCode")));

            // Thêm dữ liệu vào Attendance
            List<(string EmployeeCode, DateTime WorkDate, decimal WorkingHours, string Note, string log)> attendanceData = new List<(string, DateTime, decimal, string, string)>();
            List<(string EmployeeCode, string Description, DateTime WorkDate, decimal WorkingHours, string Note, string ActionBy)> logs = new List<(string, string, DateTime, decimal, string, string)>();
            foreach (var emp in missingEmployees)
            {
                string empCode = emp.Field<string>("EmployeeCode");
                string contractTypeCode = employeeDict[empCode].ContractTypeCode;
                if (contractTypeCode.CompareTo("t_vu") != 0)
                    continue;

                string note = "Làm việc từ xa";
                for (int day = 1; day <= daysInMonth; day++)
                {
                    DateTime date = new DateTime(year, month, day);
                    
                    DataRow newRow = mAttendamce_dt.NewRow();
                    int workHour = date.DayOfWeek == DayOfWeek.Sunday ? 0 : 8;
                    
                    newRow["EmployeeCode"] = empCode;
                    newRow["WorkDate"] = date;
                    newRow["Note"] = note;       
                    newRow["WorkingHours"] = workHour;
                    newRow["AttendanceLog"] = "";

                    mAttendamce_dt.Rows.Add(newRow);
                    attendanceData.Add((empCode, date, workHour, note, ""));
                    logs.Add((empCode, "AddMissingEmployees", date, workHour, note, UserManager.Instance.fullName));
                }
            }

            
            Boolean iSuccess = await SQLManager_QLNS.Instance.UpsertAttendanceBatchAsync(attendanceData);

            var updatedLogs = logs.Select(item =>
                                (
                                    item.EmployeeCode,
                                    Description: item.Description + (iSuccess ? " Success" : "Fail"),   // <— sửa ở đây
                                    item.WorkDate,
                                    item.WorkingHours,
                                    item.Note,
                                    item.ActionBy
                                )).ToList();
            _ = SQLManager_QLNS.Instance.InsertAttendanceLogListAsync(updatedLogs);
        }

        private async void Holiday_btn_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataRow holidayRow in mHoliday_dt.Rows)
                {
                    DateTime dateOff = Convert.ToDateTime(holidayRow["HolidayDate"]);
                    string holidayName = Convert.ToString(holidayRow["HolidayName"]);

                    await SQLManager_QLNS.Instance.InsertHolidayToAttendanceAsync(dateOff, holidayName);
                }

                int month = monthYearDtp.Value.Month;
                int year = monthYearDtp.Value.Year;
                SQLStore_QLNS.Instance.removeLeaveAttendances(year);                
                SQLStore_QLNS.Instance.removeAttendamce(month, year);
                LoadAttandance_btn_Click(null, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
            }
        }
    }
}
