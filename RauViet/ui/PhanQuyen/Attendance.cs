using DocumentFormat.OpenXml.Bibliography;
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
    public partial class Attendance : Form
    {
        DataTable mAttendamce_dt, mEmployee_dt, mHoliday_dt, mLeaveAttendance_dt;
        Dictionary<string, (string PositionCode, string ContractTypeCode)> employeeDict;
        LoadingOverlay loadingOverlay;
        int mCurrentMonth = -1;
        int mCurrentYear = -1;
        public Attendance()
        {
            InitializeComponent();

            month_cbb.Items.Clear();
            for (int m = 1; m <= 12; m++)
            {
                month_cbb.Items.Add(m);
            }

            int prevMonth, year;
            if (DateTime.Now.Month == 1)
            {
                prevMonth = 12;                     // Tháng trước của tháng 1 là tháng 12
                year = DateTime.Now.Year - 1;       // Năm trước
            }
            else
            {
                prevMonth = DateTime.Now.Month - 1; // Tháng trước
                year = DateTime.Now.Year;
            }

            month_cbb.SelectedItem = prevMonth;
            year_tb.Text = year.ToString();

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            status_lb.Text = "";

            dataGV.SelectionChanged += this.dataGV_CellClick;
            attendanceGV.CellFormatting += AttandaceGV_CellFormatting;
            attendanceGV.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.attendanceGV_EditingControlShowing);
            year_tb.KeyPress += Tb_KeyPress_OnlyNumber;

            attendanceGV.CellBeginEdit += AttendanceGV_CellBeginEdit;
            attendanceGV.CellEndEdit += AttendanceGV_CellEndEdit;

            excelAttendance_btn.Click += ExcelAttendance_btn_Click;                       

            loadAttandance_btn.Click += LoadAttandance_btn_Click;
            calWorkHour_btn.Click += CalWorkHour_btn_Click;

            attendanceGV.CellParsing += AttendanceGV_CellParsing;
        }

        public async void ShowData()
        {
            
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;


            await Task.Delay(50);
            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Show();

            try
            {
                int month = Convert.ToInt32(month_cbb.SelectedItem);
                int year = Convert.ToInt32(year_tb.Text);
                // Chạy truy vấn trên thread riêng
                string[] keepColumns = { "EmployeeCode", "FullName", "PositionName", "ContractTypeName", "PositionCode", "ContractTypeCode" };
                var employeesTask = SQLStore.Instance.GetEmployeesAsync(keepColumns);
                var attendamceTask = SQLStore.Instance.GetAttendamceAsync(null, month, year);
                var holidayTask = SQLStore.Instance.GetHolidaysAsync(month, year);
                var leaveAttendanceTask = SQLStore.Instance.GetLeaveAttendancesAsyn(year);

                await Task.WhenAll(employeesTask, attendamceTask, holidayTask, leaveAttendanceTask);
                mEmployee_dt = employeesTask.Result;
                mAttendamce_dt = attendamceTask.Result;
                mHoliday_dt = holidayTask.Result;
                mLeaveAttendance_dt = leaveAttendanceTask.Result;

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
                dataGV.Columns["EmployeeCode"].HeaderText = "Mã Nhân Viên";
                dataGV.Columns["FullName"].HeaderText = "Tên Nhân Viên";
                dataGV.Columns["TotalWorkingHour"].HeaderText = "Tổng G.Làm";
                dataGV.Columns["TotalWorkingDay"].HeaderText = "Tổng N.Làm";
                dataGV.Columns["ContractTypeName"].HeaderText = "Loại H.Đồng";
                dataGV.Columns["PositionName"].HeaderText = "Chức Vụ";

                dataGV.Columns["PositionCode"].Visible = false;
                dataGV.Columns["ContractTypeCode"].Visible = false;

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

                Attendamce(month, year);
                attendanceGV.DataSource = mAttendamce_dt;
                attendanceGV.Columns["EmployeeCode"].Visible = false;
                attendanceGV.Columns["LeaveHours"].Visible = false;
                attendanceGV.Columns["WorkDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
                attendanceGV.Columns["WorkingHours"].DefaultCellStyle.Format = "0.##";

                attendanceGV.Columns["WorkDate"].HeaderText = "Ngày Làm";
                attendanceGV.Columns["WorkingHours"].HeaderText = "Số G.Làm";
                attendanceGV.Columns["DayOfWeek"].HeaderText = "Thứ";
                attendanceGV.Columns["AttendanceLog"].HeaderText = "Ra/Vào Cổng";
                attendanceGV.Columns["LeaveTypeName"].HeaderText = "Nghỉ(Nếu Có)";
                attendanceGV.Columns["OvertimeName"].HeaderText = "Tăng Ca";
                attendanceGV.Columns["Note"].HeaderText = "Ghi Chú";

                attendanceGV.Columns["DayOfWeek"].Width = 40;
                attendanceGV.Columns["WorkDate"].Width = 70;
                attendanceGV.Columns["LeaveTypeName"].Width = 100;
                attendanceGV.Columns["OvertimeName"].Width = 120;
                attendanceGV.Columns["WorkingHours"].Width = 50;
                attendanceGV.Columns["AttendanceLog"].Width = 250;
                attendanceGV.Columns["Note"].Width = 150;

                attendanceGV.Columns["AttendanceLog"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                attendanceGV.Columns["OvertimeName"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                attendanceGV.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                attendanceGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                bool isLock = await SQLStore.Instance.IsSalaryLockAsync(month, year);
                attendanceGV.ReadOnly = isLock;
                attendanceGV.Columns["WorkingHours"].ReadOnly = false;
                attendanceGV.Columns["Note"].ReadOnly = false;
                attendanceGV.Columns["DayOfWeek"].ReadOnly = true;
                attendanceGV.Columns["WorkDate"].ReadOnly = true;
                attendanceGV.Columns["AttendanceLog"].ReadOnly = true;

                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                calWorkHour_btn.Visible = !isLock;
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

            bool isLock = await SQLStore.Instance.IsSalaryLockAsync(month, year);
            attendanceGV.ReadOnly = isLock;

            cal_WorkingHour_WorkingDay();            

            if (dataGV.CurrentRow != null)
            {
                int selectedIndex = dataGV.CurrentRow?.Index ?? -1;
                UpdateRightUI(selectedIndex);
            }

            calWorkHour_btn.Visible = !isLock;

        }

        private async void CalWorkHour_btn_Click(object sender, EventArgs e)
        {
            List<(string EmployeeCode, DateTime WorkDate, decimal WorkingHours, string Note, string log)> attendanceData = new List<(string, DateTime, decimal, string, string)>();
            foreach (DataRow dr in mAttendamce_dt.Rows)
            {
                if (double.TryParse(dr["LeaveHours"]?.ToString(), out double leaveHour))
                {
                    dr["WorkingHours"] = Math.Max(0, 8 - leaveHour);                    

                    string employeeCode = Convert.ToString(dr["EmployeeCode"]);
                    DateTime workDate = Convert.ToDateTime(dr["WorkDate"]);
                    decimal workingHours = Convert.ToDecimal(dr["WorkingHours"]);
                    string note = Convert.ToString(dr["Note"]);
                    string attendanceLog = Convert.ToString(dr["AttendanceLog"]);

                    attendanceData.Add((employeeCode, workDate, workingHours, note, attendanceLog));                    
                }
            }

            try
            {
                Boolean iSuccess = await SQLManager.Instance.UpsertAttendanceBatchAsync(attendanceData);
                if (!iSuccess)
                {
                    MessageBox.Show("Cập nhật thất bại!");
                }
                else
                {
                    status_lb.Text = "Thành Công.";
                    status_lb.ForeColor = Color.Blue;
                }
            }
            catch
            {
                MessageBox.Show("Thất Bại ?", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

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
            TextBox tb = e.Control as TextBox;
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

        private async void ExcelAttendance_btn_Click(object sender, EventArgs e)
        {
            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Message = "Đang xử lý ...";
            loadingOverlay.Show();
            await Task.Delay(100);
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Title = "Chọn file Excel";
                    ofd.Filter = "Excel Files|*.xlsx;*.xls|All Files|*.*";
                    ofd.Multiselect = false; // chỉ cho chọn 1 file

                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = ofd.FileName;
                        DataTable excelData = Utils.LoadExcel_NoHeader(filePath);

                        List<(string EmployeeCode, DateTime WorkDate, decimal WorkingHours, string Note, string log)> attendanceData = new List<(string, DateTime, decimal, string, string)>();

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
                                    var holidayTask = SQLManager.Instance.GetHolidayAsync(workDate.Month, workDate.Year);
                                    await Task.WhenAll(holidayTask);
                                    mHoliday_dt = holidayTask.Result;
                                }
                                month = workDate.Month;
                                year = workDate.Year;
                            }
                            bool isWork_Morning = false;
                            bool isWork_Afternoon = false;

                            string log = "";
                            for (int colIndex = 6; colIndex < excelData.Columns.Count; colIndex++)
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

                                var filtered = mLeaveAttendance_dt.AsEnumerable().Where(r => r.Field<DateTime>("DateOff").Date == workDate.Date && r.Field<string>("EmployeeCode") == employeeCode).ToList();
                                
                                decimal totalHourOff = 0;
                                foreach(var row in filtered)
                                {
                                    decimal.TryParse(row["LeaveHours"].ToString(), out decimal lh);
                                    totalHourOff += lh;
                                }

                                if (workingHours + totalHourOff > 8)
                                    workingHours = 8 - totalHourOff;
                            }

                            attendanceData.Add((employeeCode, workDate, workingHours, note, log));
                        }

                        if (month == -1 || year == -1)
                        {
                            MessageBox.Show("Dữ Liệu Ngày Làm Có Vấn Đề Hoặc File Đang Mở ?", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            bool isLock = await SQLStore.Instance.IsSalaryLockAsync(month, year);
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

                                    Boolean iSuccess = await SQLManager.Instance.UpsertAttendanceBatchAsync(attendanceData);
                                    if (iSuccess)
                                    {
                                        month_cbb.SelectedItem = month;
                                        year_tb.Text = year.ToString();

                                        SQLStore.Instance.removeAttendamce(month, year);
                                        var attendamceTask = SQLStore.Instance.GetAttendamceAsync(null, month, year);
                                        await Task.WhenAll(attendamceTask);
                                        mAttendamce_dt = attendamceTask.Result;

                                        Attendamce(month, year);
                                    }
                                    else
                                    {
                                        MessageBox.Show("Thất Bại ?", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                }
                                catch
                                {
                                    await Task.Delay(200);
                                    loadingOverlay.Hide();
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
            await Task.Delay(50);
            int month = Convert.ToInt32(month_cbb.SelectedItem);
            int year = Convert.ToInt32(year_tb.Text);

            var attendamceTask = SQLStore.Instance.GetAttendamceAsync(null, month, year);
            var holidayTask = SQLStore.Instance.GetHolidaysAsync(month, year);
            var leaveAttendanceTask = SQLStore.Instance.GetLeaveAttendancesAsyn(year);

            await Task.WhenAll(attendamceTask, holidayTask, leaveAttendanceTask);

            mCurrentYear = year;
            mCurrentMonth = month;

            mAttendamce_dt = attendamceTask.Result;
            mHoliday_dt = holidayTask.Result;
            mLeaveAttendance_dt = leaveAttendanceTask.Result;

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

            // Update UI trên UI thread
            attendanceGV.DataSource = dv;
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
        }

        private async void AttendanceGV_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            bool isLock = await SQLStore.Instance.IsSalaryLockAsync(mCurrentMonth, mCurrentYear);
            if (isLock)
                e.Cancel = true;
        }

        private async void AttendanceGV_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
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
                    Boolean iSuccess = await SQLManager.Instance.UpsertAttendanceBatchAsync(attendanceData);
                    if (!iSuccess)
                    {
                        MessageBox.Show("Cập nhật thất bại!");
                    }
                    else
                    {
                        status_lb.Text = "Thành Công.";
                        status_lb.ForeColor = Color.Blue;
                    }
                }
                catch
                {                    
                    MessageBox.Show("Thất Bại ?", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


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
    }
}
