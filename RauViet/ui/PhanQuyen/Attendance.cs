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
    public partial class Attendance : Form
    {
        DataTable mAttendamce_dt, mEmployee_dt, mHoliday_dt;
        Dictionary<string, (string PositionCode, string ContractTypeCode)> employeeDict;
       // DataTable mShift_dt;
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
            loading_lb.Text = "Đang tải dữ liệu, vui lòng chờ...";
            loading_lb.Visible = false;

            LuuThayDoiBtn.Click += saveBtn_Click;
            dataGV.SelectionChanged += this.dataGV_CellClick;
            attendanceGV.CellFormatting += AttandaceGV_CellFormatting;
            attendanceGV.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.attendanceGV_EditingControlShowing);

            year_tb.KeyPress += Tb_KeyPress_OnlyNumber;

            excelAttendance_btn.Click += ExcelAttendance_btn_Click;

            loadAttandance_btn.Click += LoadAttandance_btn_Click;
        }


        public async void ShowData()
        {
            
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            loading_lb.Visible = true;            

            try
            {
                int month = Convert.ToInt32(month_cbb.SelectedItem);
                int year = Convert.ToInt32(year_tb.Text);
                // Chạy truy vấn trên thread riêng
                var employeeTask = SQLManager.Instance.GetEmployeesForEttendamceAsync();
                var attendamceTask = SQLManager.Instance.GetAttendamceAsync(month, year);
                var holidayTask = SQLManager.Instance.GetHolidayAsync(month, year);

                await Task.WhenAll(employeeTask, attendamceTask, holidayTask);
                mEmployee_dt = employeeTask.Result;
                mAttendamce_dt = attendamceTask.Result;
                mHoliday_dt = holidayTask.Result;

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

                Attendamce();

                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            catch (Exception ex)
            {
                status_lb.Text = "Thất bại.";
                status_lb.ForeColor = Color.Red;
            }
            finally
            {
                loading_lb.Visible = false; // ẩn loading
                loading_lb.Enabled = true; // enable lại button
            }
        }

        private void Attendamce()
        {
            mAttendamce_dt.Columns.Add("DayOfWeek", typeof(string));

            foreach (DataRow row in mAttendamce_dt.Rows)
            {
                DateTime dt = Convert.ToDateTime(row["WorkDate"]);
                string[] vietDays = { "CN", "T.2", "T.3", "T.4", "T.5", "T.6", "T.7" };
                row["DayOfWeek"] = vietDays[(int)dt.DayOfWeek];
            }

            int count = 0;
            mAttendamce_dt.Columns["DayOfWeek"].SetOrdinal(count++);
            mAttendamce_dt.Columns["WorkDate"].SetOrdinal(count++);
            mAttendamce_dt.Columns["WorkingHours"].SetOrdinal(count++);
            mAttendamce_dt.Columns["AttendanceLog"].SetOrdinal(count++);
            mAttendamce_dt.Columns["LeaveTypeName"].SetOrdinal(count++);
            mAttendamce_dt.Columns["OvertimeName"].SetOrdinal(count++);
            mAttendamce_dt.Columns["Note"].SetOrdinal(count);

            attendanceGV.DataSource = mAttendamce_dt;
            attendanceGV.Columns["EmployeeCode"].Visible = false;
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
            attendanceGV.Columns["LeaveTypeName"].Width = 70;
            attendanceGV.Columns["OvertimeName"].Width = 120;
            attendanceGV.Columns["WorkingHours"].Width = 50;
            attendanceGV.Columns["AttendanceLog"].Width = 250;
            attendanceGV.Columns["Note"].Width = 150;

            attendanceGV.Columns["AttendanceLog"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            attendanceGV.Columns["OvertimeName"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            attendanceGV.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            attendanceGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            attendanceGV.ReadOnly = false;
            attendanceGV.Columns["WorkingHours"].ReadOnly = false;
            attendanceGV.Columns["Note"].ReadOnly = false;
            attendanceGV.Columns["DayOfWeek"].ReadOnly = true;
            attendanceGV.Columns["WorkDate"].ReadOnly = true;
            attendanceGV.Columns["AttendanceLog"].ReadOnly = true;

            cal_WorkingHour_WorkingDay();

            if (dataGV.CurrentRow != null)
            {
                int selectedIndex = dataGV.CurrentRow?.Index ?? -1;
                UpdateRightUI(selectedIndex);
            }
        }

        private void AttandaceGV_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            
            if (attendanceGV.Columns[e.ColumnIndex].Name == "WorkingHours" ||
                    attendanceGV.Columns[e.ColumnIndex].Name == "Note")
            {
                e.CellStyle.BackColor = System.Drawing.Color.LightGray;
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
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Chọn file Excel";
                ofd.Filter = "Excel Files|*.xlsx;*.xls|All Files|*.*";
                ofd.Multiselect = false; // chỉ cho chọn 1 file

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string filePath = ofd.FileName;
                    DataTable excelData = Utils.LoadExcel_NoHeader(filePath);

                    List<(string EmployeeCode, DateTime WorkDate, double WorkingHours, string Note, string log)> attendanceData = new List<(string, DateTime, double, string, string)>();

                    TimeSpan startTime_Morning = new TimeSpan(9, 30, 0);
                    TimeSpan startTime_AfterNoon_1 = new TimeSpan(12, 30, 0);
                    TimeSpan startTime_AfterNoon_2 = new TimeSpan(13, 30, 0);
                    int month = -1;
                    int year = -1;
                                        
                    foreach (DataRow edr in excelData.Rows)
                    {
                        int employeeID = Convert.ToInt32(edr["Column1"]);
                        string employeeCode = $"VR{employeeID:D4}";
                        if (!employeeDict.ContainsKey(employeeCode))
                            continue;

                        string positionCode = employeeDict[employeeCode].PositionCode;
                        string contractTypeCode = employeeDict[employeeCode].ContractTypeCode;

                        DateTime workDate = Convert.ToDateTime(edr["Column2"]).Date;
                        

                        if((month != -1 && month != workDate.Month) ||
                            year != -1 && year != workDate.Year)
                        {
                            month = -1;
                            break;
                        }
                        else
                        {
                            if(month == -1)
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
                        for (int colIndex = 2; colIndex < excelData.Columns.Count; colIndex++)
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
                                    else if (checkInTime > startTime_AfterNoon_2)
                                        isWork_Afternoon = true;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }

                        float workingHours = 0;
                        bool isSunDay = workDate.DayOfWeek == DayOfWeek.Sunday;
                        bool isHoliday = mHoliday_dt.AsEnumerable().Any(r => r.Field<DateTime>("HolidayDate").Date == workDate.Date);
                        string note = "";
                        if (positionCode.CompareTo("bao_ve") == 0)
                        {
                            workingHours = 12f;
                        }
                        else if (isHoliday) {
                            workingHours = 0;
                            note = "NL";
                        }
                        else if (!isSunDay) { 
                            if (contractTypeCode.CompareTo("t_vu") == 0)
                            {
                                workingHours = 8f;
                            }
                            else
                            {
                                if (isWork_Morning) workingHours += 4.5f;
                                if (isWork_Afternoon) workingHours += 3.5f;
                            }

                        }
                        attendanceData.Add((employeeCode, workDate, workingHours, note, log));
                    }



                    if (month == -1 || year == -1)
                    {
                        MessageBox.Show("Dữ Liệu Ngày Làm Có Vấn Đề Hoặc File Đang Mở ?", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        //tạo dữ liệu cho nhân viên thời vụ
                        var thoiVuList = employeeDict
                                            .Where(kv => kv.Value.ContractTypeCode.Equals("thoi_vu", StringComparison.OrdinalIgnoreCase))
                                            .Select(kv => kv.Key) // chỉ lấy EmployeeCode
                                            .ToList();

                        int daysInMonth = DateTime.DaysInMonth(year, month);

                        foreach (var employeeCode in thoiVuList)
                        {
                            for(int day = 1; day <= daysInMonth; day++)
                            {
                                var workDate = new DateTime(year, month, day);
                                if(workDate.DayOfWeek != DayOfWeek.Sunday)
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

                                    var attendamceTask = SQLManager.Instance.GetAttendamceAsync(month, year);
                                    

                                    await Task.WhenAll(attendamceTask);
                                    mAttendamce_dt = attendamceTask.Result;

                                    Attendamce();
                                }
                                else
                                {
                                    MessageBox.Show("Thất Bại ?", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            catch
                            {
                                MessageBox.Show("Thất Bại ?", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
        }

        private async void LoadAttandance_btn_Click(object sender, EventArgs e)
        {
            int month = Convert.ToInt32(month_cbb.SelectedItem);
            int year = Convert.ToInt32(year_tb.Text);

            var attendamceTask = SQLManager.Instance.GetAttendamceAsync(month, year);
            var holidayTask = SQLManager.Instance.GetHolidayAsync(month, year);
            
            await Task.WhenAll(attendamceTask, holidayTask);

            mAttendamce_dt = attendamceTask.Result;
            mHoliday_dt = holidayTask.Result;
            Attendamce();
        }

        private void dataGV_CellClick(object sender, EventArgs e)
        {
            if (dataGV.CurrentRow == null) return;
            int rowIndex = dataGV.CurrentRow.Index;
            if (rowIndex < 0)
                return;


            UpdateRightUI(rowIndex);
        }

        private void cal_WorkingHour_WorkingDay()
        {
            var summaryDict = mAttendamce_dt.AsEnumerable()
            .GroupBy(r => r.Field<string>("EmployeeCode"))
            .ToDictionary(
                g => g.Key,
                g => new
                {
                    TotalWorkingHours = g.Sum(r => r.Field<double?>("WorkingHours") ?? 0f),
                    WorkingDays = g.Count(r => (r.Field<double?>("WorkingHours") ?? 0f) > 0)
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

        private async void saveBtn_Click(object sender, EventArgs e)
        {
            List<(string EmployeeCode, DateTime WorkDate, double WorkingHours, string Note, string log)> attendanceData = new List<(string, DateTime, double, string, string)>();

            foreach (DataRow edr in mAttendamce_dt.Rows)
            {
                string employeeCode = Convert.ToString(edr["EmployeeCode"]);
                DateTime workDate = Convert.ToDateTime(edr["WorkDate"]);
                double workingHours = Convert.ToDouble(edr["WorkingHours"]);
                string note = Convert.ToString(edr["Note"]);
                string attendanceLog = Convert.ToString(edr["AttendanceLog"]);

                attendanceData.Add((employeeCode, workDate, workingHours, note, attendanceLog));
                
            }

            DialogResult dialogResult = MessageBox.Show($"Sai Là Không sửa lại được đâu đó ?", "Chỉnh Sửa Lại Giờ Làm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    Boolean iSuccess = await SQLManager.Instance.UpsertAttendanceBatchAsync(attendanceData);
                    if (iSuccess)
                    {
                        cal_WorkingHour_WorkingDay();
                        MessageBox.Show("Cập Nhật Thành Công Rồi Đó ?", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Thất Bại ?", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch
                {
                    MessageBox.Show("Thất Bại ?", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
