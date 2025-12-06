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
        private DataTable mEmployeeLeave_dt;
        private DataView mDeductionLogDV;
        private const string DeductionTypeCode = "ATT";
        private int curMonth, curYear;
        public EmployeeDeduction_ATT()
        {
            InitializeComponent();

            monthYearDtp.Format = DateTimePickerFormat.Custom;
            monthYearDtp.CustomFormat = "MM/yyyy";
            monthYearDtp.ShowUpDown = true;
            monthYearDtp.Value = DateTime.Now;

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            employeeDeductionGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            employeeDeductionGV.MultiSelect = false;

            status_lb.Text = "";

            LuuThayDoiBtn.Click += saveBtn_Click;
            dataGV.SelectionChanged += this.dataGV_CellClick;
            amount_tb.KeyPress += Tb_KeyPress_OnlyNumber;
            load_btn.Click += Load_btn_Click;
        }

        public async void ShowData()
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            await Task.Delay(50);
            LoadingOverlay loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Show();

            try
            {
                int month = monthYearDtp.Value.Month;
                int year = monthYearDtp.Value.Year;
                var employeeTask = SQLManager.Instance.GetActiveEmployee_DeductionATT_Async(month, year);
                var employeeLeaveAsync = SQLManager.Instance.GetEmployeeLeave_PT_KP_Async(month, year);
                var EmployeeDeductionLogTask = SQLStore.Instance.GetEmployeeDeductionLogAsync(month, year, DeductionTypeCode);
                await Task.WhenAll(employeeTask, employeeLeaveAsync, EmployeeDeductionLogTask);
                DataTable employee_dt = employeeTask.Result;
                mEmployeeLeave_dt = employeeLeaveAsync.Result;
                mDeductionLogDV = new DataView(EmployeeDeductionLogTask.Result);
                curMonth = month;
                curYear = year;

                foreach (DataRow dr in employee_dt.Rows)
                {
                    if (dr["PositionName"] == DBNull.Value) dr["PositionName"] = "";
                    if (dr["ContractTypeName"] == DBNull.Value) dr["ContractTypeName"] = "";
                }

                employee_dt.Columns.Add(new DataColumn("TotalOffDay", typeof(int)));
                Dictionary<string, int> grouped = mEmployeeLeave_dt.AsEnumerable().GroupBy(r => r.Field<string>("EmployeeCode")).ToDictionary(g => g.Key, g => g.Count());
                foreach (DataRow dr in employee_dt.Rows)
                {
                    string empCode = dr["EmployeeCode"].ToString();
                    dr["TotalOffDay"] = grouped.ContainsKey(empCode) ? grouped[empCode] : 0;
                }

                employeeDeductionGV.DataSource = mEmployeeLeave_dt;

                employee_dt.Columns["DeductionAmount"].ReadOnly = false;
                dataGV.AutoGenerateColumns = true;
                dataGV.DataSource = employee_dt;
                log_GV.DataSource = mDeductionLogDV;

                employeeDeductionGV.Columns["EmployeeCode"].Visible = false;

                int count = 0;
                mEmployeeLeave_dt.Columns["DateOff"].SetOrdinal(count++);
                mEmployeeLeave_dt.Columns["LeaveTypeName"].SetOrdinal(count++);
                mEmployeeLeave_dt.Columns["LeaveHours"].SetOrdinal(count++);
                mEmployeeLeave_dt.Columns["Note"].SetOrdinal(count++);

                log_GV.Columns["LogID"].Visible = false;
                log_GV.Columns["EmployeeCode"].Visible = false;
                log_GV.Columns["DeductionTypeCode"].Visible = false;

                employeeDeductionGV.Columns["DateOff"].HeaderText = "Ngày Nghỉ";
                employeeDeductionGV.Columns["LeaveTypeName"].HeaderText = "Loại Nghỉ Phép";
                employeeDeductionGV.Columns["LeaveHours"].HeaderText = "Số Giờ Vắng";
                employeeDeductionGV.Columns["Note"].HeaderText = "Ghi Chú";

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

        private async void Load_btn_Click(object sender, EventArgs e)
        {
            ShowData();
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
            UpdateRightUI(rowIndex);
        }

        private void UpdateEmployeeDeductionUI(int index)
        {
            amount_tb.Text = "0";

            employeeDeductionGV.ClearSelection();

            var cells = dataGV.Rows[index].Cells;
            string employeeCode = Convert.ToString(cells["EmployeeCode"].Value);

            DataView dv = new DataView(mEmployeeLeave_dt);
            dv.RowFilter = $"EmployeeCode = '{employeeCode}'";

            employeeDeductionGV.DataSource = dv;

            mDeductionLogDV.RowFilter = $"EmployeeCode = '{employeeCode}'";
        }
        private void UpdateRightUI(int index)
        {
            var cells = dataGV.Rows[index].Cells;
            int amount = Convert.ToInt32(cells["DeductionAmount"].Value);

            amount_tb.Text = amount.ToString();
            info_gb.BackColor = Color.DarkGray;
            status_lb.Text = "";
        }

        private async void updateData(string employeeCode, DateTime deductionDate, int deductionAmount)
        {
            foreach (DataGridViewRow row in dataGV.Rows)
            {
                string id = Convert.ToString(row.Cells["EmployeeCode"].Value);
                if (id.CompareTo(employeeCode) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager.Instance.UpsertEmployeeDeductionAsync(employeeCode, DeductionTypeCode, deductionDate, deductionAmount, "");

                            if (isScussess == true)
                            {
                                _ = SQLManager.Instance.InsertEmployeeDeductionLogAsync(employeeCode, DeductionTypeCode, $"Edit: Success", deductionDate.Date, deductionAmount, "Trừ chuyên cần");
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                row.Cells["DeductionAmount"].Value = deductionAmount;
                            }
                            else
                            {
                                _ = SQLManager.Instance.InsertEmployeeDeductionLogAsync(employeeCode, DeductionTypeCode, $"Edit: Fail", deductionDate.Date, deductionAmount, "Trừ chuyên cần");
                                status_lb.Text = "Thất bại.";
                                status_lb.ForeColor = Color.Red;
                            }
                        }
                        catch (Exception ex)
                        {
                            _ = SQLManager.Instance.InsertEmployeeDeductionLogAsync(employeeCode, DeductionTypeCode, $"Edit Fail Exception: " + ex.Message, deductionDate.Date, deductionAmount, "Trừ chuyên cần");
                            status_lb.Text = "Thất bại.";
                            status_lb.ForeColor = Color.Red;
                        }
                    }
                    break;
                }
            }
        }

        private async void saveBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(amount_tb.Text) || dataGV.CurrentRow == null)
            {
                MessageBox.Show("Sai Dữ Liệu, Kiểm Tra Lại!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int month = monthYearDtp.Value.Month;
            int year = monthYearDtp.Value.Year;
            DateTime deductionDate = new DateTime(year, month, 15);

            string employeeCode = Convert.ToString(dataGV.CurrentRow.Cells["EmployeeCode"].Value);
            int amount = Convert.ToInt32(amount_tb.Text);

            if(month != curMonth || year != curYear)
            {
                MessageBox.Show("Tháng, Năm có vẫn đề.", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool isLock = await SQLStore.Instance.IsSalaryLockAsync(month, year);
            if (isLock)
            {
                MessageBox.Show("Tháng " + month + "/" + year + " đã bị khóa.", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            updateData(employeeCode, deductionDate, amount);
        }

    }
}
