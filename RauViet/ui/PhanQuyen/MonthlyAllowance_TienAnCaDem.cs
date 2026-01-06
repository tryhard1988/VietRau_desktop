using DocumentFormat.OpenXml.Bibliography;
using RauViet.classes;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Color = System.Drawing.Color;

namespace RauViet.ui
{    
    public partial class MonthlyAllowance_TienAnCaDem : Form
    {
        private Timer _monthYearDebounceTimer;
        private DataTable mMonthlyAllowance_dt, mDepartment_dt, _mAllowance_dt;
        private DataView mlog_DV;
        
        public MonthlyAllowance_TienAnCaDem()
        {
            InitializeComponent();
            this.KeyPreview = true;
            Utils.SetTabStopRecursive(this, false);

            monthYearDtp.Format = DateTimePickerFormat.Custom;
            monthYearDtp.CustomFormat = "MM/yyyy";
            monthYearDtp.ShowUpDown = true;
            monthYearDtp.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            int countTab = 0;
            LuuThayDoiBtn.TabIndex = countTab++; LuuThayDoiBtn.TabStop = true;

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            department_GV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            department_GV.MultiSelect = false;

            status_lb.Text = "";

            LuuThayDoiBtn.Click += saveBtn_Click;            
            this.KeyDown += MonthlyAllowance_KeyDown;
            this.Load += OvertimeAttendace_Load;
        }

        private void OvertimeAttendace_Load(object sender, EventArgs e)
        {
            _monthYearDebounceTimer = new Timer();
            _monthYearDebounceTimer.Interval = 500;
            _monthYearDebounceTimer.Tick += MonthYearDebounceTimer_Tick;
        }

        private void MonthlyAllowance_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                ShowData();
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
                department_GV.SelectionChanged -= this.department_GV_CellClick;
                monthYearDtp.ValueChanged -= monthYearDtp_ValueChanged;

                int month = monthYearDtp.Value.Month;
                int year = monthYearDtp.Value.Year;

                var monthlyAllowance_AnDemTask = SQLStore_QLNS.Instance.GetMonthlyAllowance_AnDem_Async(month, year);         
                var departmentTask = SQLStore_QLNS.Instance.GetActiveDepartmentAsync(UserManager.Instance.get_ChamCongTangCa_Departments());

                await Task.WhenAll(monthlyAllowance_AnDemTask);
                mMonthlyAllowance_dt = monthlyAllowance_AnDemTask.Result;
                mDepartment_dt = departmentTask.Result;

                var rows = mMonthlyAllowance_dt.AsEnumerable().GroupBy(r => r.Field<string>("EmployeeCode")).Select(g => g.First());
                if (rows.Any())
                    _mAllowance_dt = rows.CopyToDataTable().DefaultView.ToTable(false, "EmployeeCode", "EmployeeName", "DepartmentID");                
                else                
                    _mAllowance_dt = mMonthlyAllowance_dt.Clone().DefaultView.ToTable(false, "EmployeeCode", "EmployeeName", "DepartmentID");

                _mAllowance_dt.Columns.Add("TotalMealAllowance", typeof(int));
                _mAllowance_dt.Columns.Add("TotalNoodleAllowance", typeof(int));

                monthYearLabel.Text = $"Tháng {month}/{year}";

                dataGV.DataSource = mMonthlyAllowance_dt;
                department_GV.DataSource = mDepartment_dt;
                allowanceGV.DataSource = _mAllowance_dt;

                allowanceGV.Columns["DepartmentID"].Visible = false;
                allowanceGV.Columns["EmployeeCode"].HeaderText = "Mã NV";
                allowanceGV.Columns["EmployeeName"].HeaderText = "Tên NV";
                allowanceGV.Columns["TotalMealAllowance"].HeaderText = "Tổng P.Cơm";
                allowanceGV.Columns["TotalNoodleAllowance"].HeaderText = "Tổng P.Mì";
                allowanceGV.Columns["EmployeeCode"].Width = 70;
                allowanceGV.Columns["EmployeeName"].Width = 160;
                allowanceGV.Columns["TotalMealAllowance"].Width = 90;
                allowanceGV.Columns["TotalNoodleAllowance"].Width = 90;

                int count = 0;
                mMonthlyAllowance_dt.Columns["EmployeeCode"].SetOrdinal(count++);
                mMonthlyAllowance_dt.Columns["EmployeeName"].SetOrdinal(count++);
                mMonthlyAllowance_dt.Columns["WorkDate"].SetOrdinal(count++);
                mMonthlyAllowance_dt.Columns["OvertimeTypeID"].SetOrdinal(count++);
                mMonthlyAllowance_dt.Columns["HourWork"].SetOrdinal(count++);
                mMonthlyAllowance_dt.Columns["Note"].SetOrdinal(count++);

                department_GV.Columns["DepartmentID"].Visible = false;
                department_GV.Columns["Description"].Visible = false;
                department_GV.Columns["IsActive"].Visible = false;
                department_GV.Columns["CreatedAt"].Visible = false;
                department_GV.Columns["DepartmentName"].HeaderText = "Tên Phòng Ban";
                department_GV.Columns["DepartmentName"].Width = 200;

                dataGV.Columns["OvertimeTypeID"].Visible = false;
                dataGV.Columns["DepartmentID"].Visible = false;
                dataGV.Columns["EmployeeName"].HeaderText = "Tên Nhân Viên";
                dataGV.Columns["EmployeeCode"].HeaderText = "Mã NV";
                dataGV.Columns["WorkDate"].HeaderText = "Ngày Làm";
                dataGV.Columns["HourWork"].HeaderText = "Số Giờ";
                dataGV.Columns["EmployeeCode"].Width = 70;
                dataGV.Columns["EmployeeName"].Width = 160;
                dataGV.Columns["WorkDate"].Width = 80;
                dataGV.Columns["HourWork"].Width = 70;
                //dataGV.Columns["EmployessName_NoSign"].Visible = false;
                //dataGV.Columns["EmployeeCode"].Width = 60;
                //dataGV.Columns["FullName"].Width = 160;

                //allowanceGV.Columns["Amount"].DefaultCellStyle.Format = "N0";
                //allowanceGV.Columns["EmployeeCode"].Width = 60;
                //allowanceGV.Columns["EmployeeName"].Width = 160;
                //allowanceGV.Columns["Date"].Width = 60;
                //allowanceGV.Columns["AllowanceName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                //allowanceGV.Columns["Amount"].Width = 70;
                //allowanceGV.Columns["Note"].Width = 120;

                department_GV.SelectionChanged += this.department_GV_CellClick;
                if (dataGV.Rows.Count > 0)
                {
                    dataGV.ClearSelection();
                    dataGV.Rows[0].Selected = true;
                }

                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                department_GV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                allowanceGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                monthYearDtp.ValueChanged += monthYearDtp_ValueChanged;
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

        private void department_GV_CellClick(object sender, EventArgs e)
        {
            if (department_GV.CurrentRow == null) return;
            int rowIndex = department_GV.CurrentRow.Index;
            if (rowIndex < 0)
                return;

        }


        private async void saveBtn_Click(object sender, EventArgs e)
        {
            //if (allowanceType_cbb.SelectedValue == null || string.IsNullOrEmpty(amount_tb.Text) || 
            //    dataGV.CurrentRow == null)
            //{
            //    MessageBox.Show("Sai Dữ Liệu, Kiểm Tra Lại!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return;
            //}

            //string employeeCode = Convert.ToString(dataGV.CurrentRow.Cells["EmployeeCode"].Value);
            //int amount = Convert.ToInt32(amount_tb.Text);
            //int month = monthYearDtp.Value.Month;
            //int year = monthYearDtp.Value.Year;
            //int allowanceTypeID = Convert.ToInt32(allowanceType_cbb.SelectedValue);
            //string note= note_tb.Text;

            //if (mCurrentMonth != month || mCurrentYear != year)
            //{
            //    MessageBox.Show("Tháng " + month + "/" + year + " có vẫn đề.", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}

            //bool isLock = await SQLStore_QLNS.Instance.IsSalaryLockAsync(month, year);
            //if (isLock)
            //{
            //    MessageBox.Show("Tháng " + month + "/" + year + " đã bị khóa.", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}

            //if (monthlyAllowanceID_tb.Text.Length != 0)
            //    updateData(Convert.ToInt32(monthlyAllowanceID_tb.Text), employeeCode, month, year, amount, allowanceTypeID, note);
            //else
            //    createNew(employeeCode, month, year, amount, allowanceTypeID, note);

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
            ShowData();
        }
    }
}
