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
    public partial class LeaveAttendance : Form
    {
        System.Data.DataTable mLeaveAttendance_dt, mEmployee_dt, mLeaveType;
        Dictionary<string, (string PositionCode, string ContractTypeCode)> employeeDict;
       // DataTable mShift_dt;
        public LeaveAttendance()
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

            dateOff_dtp.Format = DateTimePickerFormat.Custom;
            dateOff_dtp.CustomFormat = "dd/MM/yyyy";

            LuuThayDoiBtn.Click += saveBtn_Click;
            dataGV.SelectionChanged += this.dataGV_CellClick;
            

            loadAttandance_btn.Click += LoadLeaveAttendance_btn_Click;
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
                var employeeTask = SQLManager.Instance.GetEmployeesForEttendamceAsync("c_thuc");
                var leaveAttendanceTask = SQLManager.Instance.GetLeaveAttendanceAsync(month, year);
                var leaveTypeTask = SQLManager.Instance.GetLeaveTypeAsync();

                await Task.WhenAll(employeeTask, leaveAttendanceTask, leaveTypeTask);
                mEmployee_dt = employeeTask.Result;
                mLeaveAttendance_dt = leaveAttendanceTask.Result;
                mLeaveType = leaveTypeTask.Result;

                leaveType_cbb.DataSource = mLeaveType;
                leaveType_cbb.DisplayMember = "LeaveTypeName";
                leaveType_cbb.ValueMember = "LeaveTypeCode";

                employeeDict = mEmployee_dt.AsEnumerable()
                                .ToDictionary(
                                    r => r.Field<string>("EmployeeCode"),
                                    r => (
                                        PositionCode: r.Field<string>("PositionCode"),
                                        ContractTypeCode: r.Field<string>("ContractTypeCode")
                                    )
                                );


                mEmployee_dt.Columns.Add(new DataColumn("TotalWorkingHour", typeof(double)));

                dataGV.DataSource = mEmployee_dt;
                dataGV.Columns["EmployeeCode"].HeaderText = "Mã Nhân Viên";
                dataGV.Columns["FullName"].HeaderText = "Tên Nhân Viên";
                dataGV.Columns["TotalWorkingHour"].HeaderText = "Tổng G.Làm";
                dataGV.Columns["ContractTypeName"].HeaderText = "Loại H.Đồng";
                dataGV.Columns["PositionName"].HeaderText = "Chức Vụ";

                dataGV.Columns["PositionCode"].Visible = false;
                dataGV.Columns["ContractTypeCode"].Visible = false;

                dataGV.Columns["EmployeeCode"].Width = 50;
                dataGV.Columns["FullName"].Width = 160;
                dataGV.Columns["TotalWorkingHour"].Width = 50;
                dataGV.Columns["ContractTypeName"].Width = 70;
                dataGV.Columns["PositionName"].Width = 70;

                dataGV.Width = 450;

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
                loading_lb.Visible = false; // ẩn loading
                loading_lb.Enabled = true; // enable lại button
            }
        }

        private void loadLeaveAttendance()
        {
            attendanceGV.SelectionChanged -= this.attendanceGV_CellClick;

            int count = 0;
            mLeaveAttendance_dt.Columns["LeaveTypeCode"].SetOrdinal(count++);
            mLeaveAttendance_dt.Columns["DateOff"].SetOrdinal(count++);            
            mLeaveAttendance_dt.Columns["Note"].SetOrdinal(count++);

            attendanceGV.DataSource = mLeaveAttendance_dt;
            attendanceGV.Columns["LeaveID"].Visible = false;
            attendanceGV.Columns["EmployeeCode"].Visible = false;
            attendanceGV.Columns["UpdatedHistory"].Visible = false;

            attendanceGV.Columns["DateOff"].DefaultCellStyle.Format = "dd/MM/yyyy";

            attendanceGV.Columns["LeaveTypeCode"].HeaderText = "Ngày Làm";
            attendanceGV.Columns["DateOff"].HeaderText = "Ngày Nghỉ";
            attendanceGV.Columns["Note"].HeaderText = "Ghi Chú";

            attendanceGV.Columns["LeaveTypeCode"].Width = 40;
            attendanceGV.Columns["DateOff"].Width = 70;
            attendanceGV.Columns["Note"].Width = 250;

            attendanceGV.Columns["UpdatedHistory"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            attendanceGV.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            attendanceGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            attendanceGV.SelectionChanged += this.attendanceGV_CellClick;
            if (dataGV.CurrentRow != null) {
                int selectedIndex = dataGV.CurrentRow?.Index ?? -1;
                UpdateAttendanceUI(selectedIndex);
            }
        }


        private async void LoadLeaveAttendance_btn_Click(object sender, EventArgs e)
        {
            int month = Convert.ToInt32(month_cbb.SelectedItem);
            int year = Convert.ToInt32(year_tb.Text);

            var leaveAttendanceTask = SQLManager.Instance.GetLeaveAttendanceAsync(month, year);

            await Task.WhenAll(leaveAttendanceTask);

            mLeaveAttendance_dt = leaveAttendanceTask.Result;
            loadLeaveAttendance();
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
            int leaveID = Convert.ToInt32(cells["LeaveID"].Value);
            string leaveTypeCode = cells["LeaveTypeCode"].Value.ToString();
            DateTime dateOff = Convert.ToDateTime(cells["DateOff"].Value);
            string note = cells["Note"].Value.ToString();
            string updatedHistory = cells["UpdatedHistory"].Value.ToString();

            leaveID_tb.Text = leaveID.ToString();
            dateOff_dtp.Value = dateOff.Date;
            leaveType_cbb.SelectedValue = leaveTypeCode;
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

            DataView dv = new DataView(mLeaveAttendance_dt);
            dv.RowFilter = $"EmployeeCode = '{employeeCode}'";

            attendanceGV.DataSource = dv;
        }

        private async void updateData(int leaveID, string employeeCode, string leaveTypeCode, DateTime dateOff, string Note, string UpdatedHistory)
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
                            bool isScussess = await SQLManager.Instance.updateLeaveAttendanceAsync(leaveID, employeeCode, leaveTypeCode, dateOff, Note, UpdatedHistory);

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                row.Cells["EmployeeCode"].Value = employeeCode;
                                row.Cells["LeaveTypeCode"].Value = leaveTypeCode;
                                row.Cells["DateOff"].Value = dateOff.Date;
                                row.Cells["Note"].Value = Note;
                                row.Cells["UpdatedHistory"].Value = UpdatedHistory;

                                //DataRow[] rows = mOvertimeType.Select($"OvertimeTypeID = {overtimeTypeID}");
                                //if (rows.Length > 0)
                                //    row.Cells["OvertimeName"].Value = rows[0]["OvertimeName"].ToString();

                                //TimeSpan duration = endTime - startTime;
                                //row.Cells["WorkingHours"].Value = Math.Round(duration.TotalHours, 1);

                                //string[] vietDays = { "CN", "T.2", "T.3", "T.4", "T.5", "T.6", "T.7" };
                                //row.Cells["DayOfWeek"].Value = vietDays[(int)workDate.DayOfWeek];

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

        private async void createNew(string employeeCode, string leaveTypeCode, DateTime dateOff, string Note, string UpdatedHistory)
        {
            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    int newEmployee = await SQLManager.Instance.insertLeaveAttendanceAsync(employeeCode, leaveTypeCode, dateOff, Note, UpdatedHistory);
                    if (newEmployee > 0)
                    {
                        DataRow drToAdd = mLeaveAttendance_dt.NewRow();


                        leaveID_tb.Text = newEmployee.ToString();
                        drToAdd["LeaveID"] = newEmployee;
                        drToAdd["EmployeeCode"] = employeeCode;
                        drToAdd["LeaveTypeCode"] = leaveTypeCode;
                        drToAdd["DateOff"] = dateOff.Date;
                        drToAdd["Note"] = Note;
                        drToAdd["UpdatedHistory"] = UpdatedHistory;

                        //DataRow[] rows = mOvertimeType.Select($"OvertimeTypeID = {overtimeTypeID}");
                        //if (rows.Length > 0)
                        //    drToAdd["OvertimeName"] = rows[0]["OvertimeName"].ToString();

                        //TimeSpan duration = endTime - startTime;
                        //drToAdd["WorkingHours"] = Math.Round(duration.TotalHours, 1);

                        //string[] vietDays = { "CN", "T.2", "T.3", "T.4", "T.5", "T.6", "T.7" };
                        //drToAdd["DayOfWeek"] = vietDays[(int)workDate.DayOfWeek];

                        mLeaveAttendance_dt.Rows.Add(drToAdd);
                        mLeaveAttendance_dt.AcceptChanges();

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
            if (leaveType_cbb.SelectedValue == null || dataGV.CurrentRow == null) return;


            string employeeCode = dataGV.CurrentRow.Cells["EmployeeCode"].Value?.ToString();
            DateTime dateOff = dateOff_dtp.Value;

            int year = Convert.ToInt32(year_tb.Text);
            int month = Convert.ToInt32(month_cbb.SelectedItem);

            if (dateOff.Year != year || month != dateOff.Month)
            {
                MessageBox.Show("Tháng hoặc Năm có vẫn đề", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string leaveTypeCode = Convert.ToString(leaveType_cbb.SelectedValue);
            string note = note_tb.Text;
            string updatedHistory = updatedHistory_tb.Text;
            updatedHistory += DateTime.Now + ": " + UserManager.Instance.fullName + " | ";

            if (leaveID_tb.Text.Length != 0)
                updateData(Convert.ToInt32(leaveID_tb.Text), employeeCode, leaveTypeCode, dateOff, note, updatedHistory);
            else
                createNew(employeeCode, leaveTypeCode, dateOff, note, updatedHistory);

        }

        private void NewBtn_Click(object sender, EventArgs e)
        {
            leaveID_tb.Text = "";
            note_tb.Text = "";
            info_gb.BackColor = Color.Green;
        }
    }
}
