using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing.Charts;
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
       // DataTable mShift_dt;
        public LeaveAttendance()
        {
            InitializeComponent();

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
            delete_btn.Click += Delete_btn_Click;
            hourLeave_tb.KeyPress += Tb_KeyPress_OnlyNumber;
            year_tb.KeyPress += Tb_KeyPress_OnlyNumber;
            dateOff_dtp.ValueChanged += DateOff_dtp_ValueChanged;
        }


        public async void ShowData()
        {
            
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            loading_lb.Visible = true;            

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
                loading_lb.Visible = false; // ẩn loading
                loading_lb.Enabled = true; // enable lại button
            }
        }

        private void loadLeaveAttendance()
        {
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
        }


        private async void LoadLeaveAttendance_btn_Click(object sender, EventArgs e)
        {
            int year = Convert.ToInt32(year_tb.Text);

            var leaveAttendanceTask = SQLStore.Instance.GetLeaveAttendancesAsyn(year);

            await Task.WhenAll(leaveAttendanceTask);

            mLeaveAttendance_dt = leaveAttendanceTask.Result;
            loadLeaveAttendance();
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
            var cells = attendanceGV.Rows[rowIndex].Cells;
            int leaveID = Convert.ToInt32(cells["LeaveID"].Value);
            string leaveTypeCode = cells["LeaveTypeCode"].Value.ToString();
            DateTime dateOff = Convert.ToDateTime(cells["DateOff"].Value);
            string note = cells["Note"].Value.ToString();
            string updatedHistory = cells["UpdatedHistory"].Value.ToString();
            decimal hourLeave = Convert.ToDecimal(cells["LeaveHours"].Value);

            leaveID_tb.Text = leaveID.ToString();
            dateOff_dtp.Value = dateOff.Date;
            leaveType_cbb.SelectedValue = leaveTypeCode;
            note_tb.Text = note;
            updatedHistory_tb.Text = updatedHistory;
            hourLeave_tb.Text = hourLeave.ToString();

            info_gb.BackColor = Color.DarkGray;
            status_lb.Text = "";
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

        private async void createNew(string employeeCode, string leaveTypeCode, DateTime dateOff, string Note, string UpdatedHistory, decimal hourLeave)
        {
            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    int newEmployee = await SQLManager.Instance.insertLeaveAttendanceAsync(employeeCode, leaveTypeCode, dateOff, Note, UpdatedHistory, hourLeave);
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
                        drToAdd["LeaveHours"] = hourLeave;

                        DataRow[] foundRows = mLeaveType.Select($"LeaveTypeCode = '{leaveTypeCode}'");
                        if (foundRows.Length > 0)
                        {
                            drToAdd["LeaveTypeName"] = foundRows[0]["LeaveTypeName"].ToString();
                        }

                        string[] vietDays = { "CN", "T.2", "T.3", "T.4", "T.5", "T.6", "T.7" };
                        drToAdd["DayOfWeek"] = vietDays[(int)dateOff.DayOfWeek];

                        mLeaveAttendance_dt.Rows.Add(drToAdd);
                        mLeaveAttendance_dt.AcceptChanges();

                        attendanceGV.ClearSelection();
                        int rowIndex = attendanceGV.Rows.Count - 1;
                        attendanceGV.Rows[rowIndex].Selected = true;

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

        private async void saveBtn_Click(object sender, EventArgs e)
        {
            if (leaveType_cbb.SelectedValue == null || dataGV.CurrentRow == null) return;


            string employeeCode = dataGV.CurrentRow.Cells["EmployeeCode"].Value?.ToString();
            DateTime dateOff = dateOff_dtp.Value;

            bool isLock = await SQLStore.Instance.IsSalaryLockAsync(dateOff.Month, dateOff.Year);
            if (isLock)
            {
                MessageBox.Show("Tháng " + dateOff.Month + "/" + dateOff.Year + " đã bị khóa.", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            decimal hourLeave = Convert.ToDecimal(hourLeave_tb.Text);
            int year = Convert.ToInt32(year_tb.Text);

            if (dateOff.Year != year)
            {
                MessageBox.Show("Tháng hoặc Năm có vẫn đề", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            string leaveTypeCode = Convert.ToString(leaveType_cbb.SelectedValue);
            string note = note_tb.Text;
            string updatedHistory = updatedHistory_tb.Text;
            updatedHistory += DateTime.Now + ": " + UserManager.Instance.fullName + " | ";

            if (leaveID_tb.Text.Length != 0)
                updateData(Convert.ToInt32(leaveID_tb.Text), employeeCode, leaveTypeCode, dateOff, note, updatedHistory, hourLeave);
            else
                createNew(employeeCode, leaveTypeCode, dateOff, note, updatedHistory, hourLeave);

        }

        private void NewBtn_Click(object sender, EventArgs e)
        {
            leaveID_tb.Text = "";
            note_tb.Text = "";
            info_gb.BackColor = Color.Green;
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
        private async void DateOff_dtp_ValueChanged(object sender, EventArgs e)
        {
            DateTime dayyOff = Convert.ToDateTime(dateOff_dtp.Value);
            bool isLock = await SQLStore.Instance.IsSalaryLockAsync(dayyOff.Month, dayyOff.Year);
            LuuThayDoiBtn.Visible = !isLock;
            newBtn.Visible = !isLock;
            delete_btn.Visible = !isLock;
        }
    }
}
