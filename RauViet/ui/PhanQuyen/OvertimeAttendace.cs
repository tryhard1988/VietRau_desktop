using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing.Charts;
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
    public partial class OvertimeAttendace : Form
    {
        System.Data.DataTable mOvertimeAttendamce_dt, mEmployee_dt, mOvertimeType;
        Dictionary<string, (string PositionCode, string ContractTypeCode)> employeeDict;
       // DataTable mShift_dt;
        public OvertimeAttendace()
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

            attendanceGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            attendanceGV.MultiSelect = false;

            status_lb.Text = "";
            loading_lb.Text = "Đang tải dữ liệu, vui lòng chờ...";
            loading_lb.Visible = false;

            workDate_dtp.Format = DateTimePickerFormat.Custom;
            workDate_dtp.CustomFormat = "dd/MM/yyyy";

            startTime_dtp.Format = DateTimePickerFormat.Custom;
            startTime_dtp.CustomFormat = "HH:mm";
            startTime_dtp.ShowUpDown = true;

            endTime_dtp.Format = DateTimePickerFormat.Custom;
            endTime_dtp.CustomFormat = "HH:mm";
            endTime_dtp.ShowUpDown = true;

            LuuThayDoiBtn.Click += saveBtn_Click;
            dataGV.SelectionChanged += this.dataGV_CellClick;
            

            year_tb.KeyPress += Tb_KeyPress_OnlyNumber;

            loadAttandance_btn.Click += LoadAttandance_btn_Click;
            newBtn.Click += NewBtn_Click;
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
                string[] keepColumns = { "EmployeeCode", "FullName", "PositionName", "ContractTypeName"};
                var employeesTask = SQLStore.Instance.GetEmployeesAsync(keepColumns);
                var overtimeAttendamceTask = SQLStore.Instance.GetOvertimeAttendamceAsync(month, year);
                var overtimeTypeTask = SQLStore.Instance.GetOvertimeTypeAsync(true);

                await Task.WhenAll(employeesTask, overtimeAttendamceTask, overtimeTypeTask);
                mEmployee_dt = employeesTask.Result;
                mOvertimeAttendamce_dt = overtimeAttendamceTask.Result;
                mOvertimeType = overtimeTypeTask.Result;

                overtimeType_cbb.DataSource = mOvertimeType;
                overtimeType_cbb.DisplayMember = "OvertimeName";
                overtimeType_cbb.ValueMember = "OvertimeTypeID";


                dataGV.DataSource = mEmployee_dt;
                dataGV.Columns["EmployeeCode"].HeaderText = "Mã Nhân Viên";
                dataGV.Columns["FullName"].HeaderText = "Tên Nhân Viên";
                dataGV.Columns["ContractTypeName"].HeaderText = "Loại H.Đồng";
                dataGV.Columns["PositionName"].HeaderText = "Chức Vụ";

                dataGV.Columns["EmployeeCode"].Width = 50;
                dataGV.Columns["FullName"].Width = 160;
                dataGV.Columns["ContractTypeName"].Width = 70;
                dataGV.Columns["PositionName"].Width = 70;

                dataGV.Width = 450;

                if (dataGV.Rows.Count > 0)
                {
                    dataGV.ClearSelection();
                    dataGV.Rows[0].Selected = true;
                }

                Attendamce();

                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                bool isLock = await SQLStore.Instance.IsSalaryLockAsync(month, year);
                LuuThayDoiBtn.Visible = !isLock;
                newBtn.Visible = !isLock;
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
            attendanceGV.SelectionChanged -= this.attendanceGV_CellClick;

           

            int count = 0;
            mOvertimeAttendamce_dt.Columns["DayOfWeek"].SetOrdinal(count++);
            mOvertimeAttendamce_dt.Columns["WorkDate"].SetOrdinal(count++);
            mOvertimeAttendamce_dt.Columns["OvertimeName"].SetOrdinal(count++);
            mOvertimeAttendamce_dt.Columns["StartTime"].SetOrdinal(count++);
            mOvertimeAttendamce_dt.Columns["EndTime"].SetOrdinal(count++);
            mOvertimeAttendamce_dt.Columns["HourWork"].SetOrdinal(count++);            
            mOvertimeAttendamce_dt.Columns["Note"].SetOrdinal(count++);
            mOvertimeAttendamce_dt.Columns["UpdatedHistory"].SetOrdinal(count++);

            attendanceGV.DataSource = mOvertimeAttendamce_dt;
            attendanceGV.Columns["EmployeeCode"].Visible = false;
            attendanceGV.Columns["OvertimeAttendanceID"].Visible = false;
            attendanceGV.Columns["OvertimeTypeID"].Visible = false;
            attendanceGV.Columns["UpdatedHistory"].Visible = false;
           // attendanceGV.Columns["SalaryFactor"].Visible = false;
           // attendanceGV.Columns["HourWork"].Visible = false;

            attendanceGV.Columns["WorkDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            attendanceGV.Columns["HourWork"].DefaultCellStyle.Format = "0.##";

            attendanceGV.Columns["WorkDate"].HeaderText = "Ngày Làm";
            attendanceGV.Columns["HourWork"].HeaderText = "Số G.Làm";
            attendanceGV.Columns["DayOfWeek"].HeaderText = "Thứ";
            attendanceGV.Columns["OvertimeName"].HeaderText = "Loại Tăng Ca";
            attendanceGV.Columns["StartTime"].HeaderText = "Giờ Bắt Đầu";
            attendanceGV.Columns["EndTime"].HeaderText = "Giờ Kết Thúc";
            attendanceGV.Columns["Note"].HeaderText = "Ghi Chú";

            attendanceGV.Columns["DayOfWeek"].Width = 40;
            attendanceGV.Columns["WorkDate"].Width = 70;
            attendanceGV.Columns["HourWork"].Width = 50;
            attendanceGV.Columns["OvertimeName"].Width = 120;
            attendanceGV.Columns["Note"].Width = 150;
            attendanceGV.Columns["StartTime"].Width = 65;
            attendanceGV.Columns["EndTime"].Width = 65;

            attendanceGV.Columns["UpdatedHistory"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
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

        private async void LoadAttandance_btn_Click(object sender, EventArgs e)
        {
            int month = Convert.ToInt32(month_cbb.SelectedItem);
            int year = Convert.ToInt32(year_tb.Text);

            var overtimeAttendamceTask = SQLStore.Instance.GetOvertimeAttendamceAsync(month, year);


            await Task.WhenAll(overtimeAttendamceTask);

            mOvertimeAttendamce_dt = overtimeAttendamceTask.Result;
            Attendamce();

            bool isLock = await SQLStore.Instance.IsSalaryLockAsync(month, year);
            LuuThayDoiBtn.Visible = !isLock;
            newBtn.Visible = !isLock;
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
            var cells = attendanceGV.Rows[rowIndex].Cells;
            int overtimeAttendanceID = Convert.ToInt32(cells["OvertimeAttendanceID"].Value);
            DateTime workDate = Convert.ToDateTime(cells["WorkDate"].Value);
            TimeSpan startTime = (TimeSpan)cells["StartTime"].Value;
            TimeSpan endTime = (TimeSpan)cells["EndTime"].Value;
            int overtimeTypeID = Convert.ToInt32(cells["OvertimeTypeID"].Value);            
            string note = cells["Note"].Value.ToString();
            string updatedHistory = cells["UpdatedHistory"].Value.ToString();
            
            overtimeAttendaceID_tb.Text = overtimeAttendanceID.ToString();
            workDate_dtp.Value = workDate;
            startTime_dtp.Value = workDate.Date + startTime;
            endTime_dtp.Value = workDate.Date + endTime;
            overtimeType_cbb.SelectedValue = overtimeTypeID;
            note_tb.Text = note;
            updatedHistory_tb.Text = updatedHistory;

            info_gb.BackColor = Color.DarkGray;
            status_lb.Text = "";
        }


        private void UpdateAttendanceUI(int index)
        {
            var cells = dataGV.Rows[index].Cells;
            //int employeeID = Convert.ToInt32(cells["EmployeeID"].Value);
            string employeeCode = cells["EmployeeCode"].Value.ToString();

            DataView dv = new DataView(mOvertimeAttendamce_dt);
            dv.RowFilter = $"EmployeeCode = '{employeeCode}'";

            attendanceGV.DataSource = dv;
        }

        private async void updateData(int overtimeAttendanceID, string employeeCode, DateTime workDate, TimeSpan startTime, TimeSpan endTime,
                                    int overtimeTypeID, string note, string updatedHistory)
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
                            bool isScussess = await SQLManager.Instance.updateOvertimeAttendanceAsync(overtimeAttendanceID, employeeCode, workDate, 
                                                                            startTime, endTime, overtimeTypeID, note, updatedHistory);

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                row.Cells["EmployeeCode"].Value = employeeCode;
                                row.Cells["WorkDate"].Value = workDate.Date;
                                row.Cells["StartTime"].Value = startTime;
                                row.Cells["EndTime"].Value = endTime;
                                row.Cells["OvertimeTypeID"].Value = overtimeTypeID;
                                row.Cells["Note"].Value = note;
                                row.Cells["UpdatedHistory"].Value = updatedHistory;
                                row.Cells["HourWork"].Value = (endTime - startTime).TotalHours;
                                row.Cells["SalaryFactor"].Value = SQLStore.Instance.GetOvertime_SalaryFactor(overtimeTypeID);

                                DataRow[] rows = mOvertimeType.Select($"OvertimeTypeID = {overtimeTypeID}");
                                if (rows.Length > 0)
                                    row.Cells["OvertimeName"].Value = rows[0]["OvertimeName"].ToString();


                                string[] vietDays = { "CN", "T.2", "T.3", "T.4", "T.5", "T.6", "T.7" };
                                row.Cells["DayOfWeek"].Value = vietDays[(int)workDate.DayOfWeek];

                                var rowData = mOvertimeType.Select($"OvertimeTypeID = {overtimeTypeID}")[0];
                                string positionName = rowData["OvertimeName"].ToString();
                                double salaryFactor = Convert.ToDouble(rowData["SalaryFactor"]);
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

        private async void createNew(string employeeCode, DateTime workDate, TimeSpan startTime, TimeSpan endTime,
                                    int overtimeTypeID, string note, string updatedHistory)
        {
            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    int newEmployee = await SQLManager.Instance.insertOvertimeAttendanceAsync(employeeCode, workDate, startTime, endTime, overtimeTypeID, note, updatedHistory);
                    if (newEmployee > 0)
                    {
                        DataRow drToAdd = mOvertimeAttendamce_dt.NewRow();


                        overtimeAttendaceID_tb.Text = newEmployee.ToString();
                        drToAdd["OvertimeAttendanceID"] = newEmployee;
                        drToAdd["EmployeeCode"] = employeeCode;
                        drToAdd["WorkDate"] = workDate.Date;
                        drToAdd["StartTime"] = startTime;
                        drToAdd["EndTime"] = endTime;
                        drToAdd["OvertimeTypeID"] = overtimeTypeID;
                        drToAdd["Note"] = note;
                        drToAdd["UpdatedHistory"] = updatedHistory;
                        drToAdd["HourWork"] = (endTime - startTime).TotalHours;
                        drToAdd["SalaryFactor"] = SQLStore.Instance.GetOvertime_SalaryFactor(overtimeTypeID);

                        DataRow[] rows = mOvertimeType.Select($"OvertimeTypeID = {overtimeTypeID}");
                        if (rows.Length > 0)
                            drToAdd["OvertimeName"] = rows[0]["OvertimeName"].ToString();

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
                    status_lb.Text = "Thất bại.";
                    status_lb.ForeColor = Color.Red;
                }

            }
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            if (overtimeType_cbb.SelectedValue == null || dataGV.CurrentRow == null) return;


            string employeeCode = dataGV.CurrentRow.Cells["EmployeeCode"].Value?.ToString();
            DateTime workDate = workDate_dtp.Value;

            int year = Convert.ToInt32(year_tb.Text);
            int month = Convert.ToInt32(month_cbb.SelectedItem);

            if (workDate.Year != year || month != workDate.Month)
            {
                MessageBox.Show("Tháng hoặc Năm có vẫn đề", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            TimeSpan startTime = new TimeSpan(startTime_dtp.Value.Hour, startTime_dtp.Value.Minute, 0);
            TimeSpan endTime = new TimeSpan(endTime_dtp.Value.Hour, endTime_dtp.Value.Minute, 0);
            int overtimeAttendanceID = Convert.ToInt32(overtimeType_cbb.SelectedValue);
            string note = note_tb.Text;
            string updatedHistory = updatedHistory_tb.Text;
            updatedHistory += DateTime.Now + ": " + UserManager.Instance.fullName + " | ";

            if (endTime < startTime)
            {
                MessageBox.Show("Sai Dữ Liệu, Kiểm Tra Lại!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (overtimeAttendaceID_tb.Text.Length != 0)
                updateData(Convert.ToInt32(overtimeAttendaceID_tb.Text), employeeCode, workDate, startTime, endTime, overtimeAttendanceID, note, updatedHistory);
            else
                createNew(employeeCode, workDate, startTime, endTime, overtimeAttendanceID, note, updatedHistory);

        }

        private void NewBtn_Click(object sender, EventArgs e)
        {
            overtimeAttendaceID_tb.Text = "";
            note_tb.Text = "";
           // workDate_dtp.Value = DateTime.Now.Date;
            info_gb.BackColor = Color.Green;
        }
    }
}
