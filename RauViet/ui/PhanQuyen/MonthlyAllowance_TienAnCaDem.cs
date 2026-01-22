using DocumentFormat.OpenXml.Bibliography;
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
    public partial class MonthlyAllowance_TienAnCaDem : Form
    {
        private Timer _monthYearDebounceTimer;
        private DataTable mOvertime_dt, mDepartment_dt, _mAllowance_dt;
        private DataView mOvertimeDV, mAllowanceDV;
        private const int mAllowanceTypeID = 21;


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

            allowanceGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            allowanceGV.MultiSelect = false;

            LuuThayDoiBtn.Click += saveBtn_Click;            
            this.KeyDown += MonthlyAllowance_KeyDown;
            this.Load += OvertimeAttendace_Load;
            print_btn.Click += Print_btn_Click;
            printPreview_btn.Click += PrintPreview_btn_Click;
            filter_CB.CheckedChanged += Filter_CB_CheckedChanged;
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
                allowanceGV.SelectionChanged -= this.allowanceGV_CellClick;
                monthYearDtp.ValueChanged -= monthYearDtp_ValueChanged;

                int month = monthYearDtp.Value.Month;
                int year = monthYearDtp.Value.Year;

                var monthlyAllowance_AnDemTask = SQLStore_QLNS.Instance.GetMonthlyAllowance_AnDem_Async(month, year);         
                var departmentTask = SQLStore_QLNS.Instance.GetActiveDepartmentAsync(UserManager.Instance.get_ChamCongTangCa_Departments());

                await Task.WhenAll(monthlyAllowance_AnDemTask);
                mOvertime_dt = monthlyAllowance_AnDemTask.Result;
                mDepartment_dt = departmentTask.Result;

                var rows = mOvertime_dt.AsEnumerable().GroupBy(r => r.Field<string>("EmployeeCode")).Select(g => g.First());
                if (rows.Any())
                    _mAllowance_dt = rows.CopyToDataTable().DefaultView.ToTable(false, "EmployeeCode", "EmployeeName", "DepartmentID");                
                else                
                    _mAllowance_dt = mOvertime_dt.Clone().DefaultView.ToTable(false, "EmployeeCode", "EmployeeName", "DepartmentID");

                _mAllowance_dt.Columns.Add("TotalMealAllowance", typeof(int));
                _mAllowance_dt.Columns.Add("TotalNoodleAllowance", typeof(int));
                TinhPhanAn();

                monthYearLabel.Text = $"Tháng {month}/{year}";

                mOvertimeDV = new DataView(mOvertime_dt);
                mAllowanceDV = new DataView(_mAllowance_dt);
                dataGV.DataSource = mOvertimeDV;
                department_GV.DataSource = mDepartment_dt;
                allowanceGV.DataSource = mAllowanceDV;

                Utils.HideColumns(department_GV, new[] { "DepartmentID", "Description", "IsActive", "CreatedAt" });
                Utils.HideColumns(dataGV, new[] { "OvertimeTypeID", "DepartmentID" });

                Utils.SetGridHeaders(allowanceGV, new System.Collections.Generic.Dictionary<string, string> {
                    {"EmployeeCode", "Mã NV" },
                    {"EmployeeName", "Tên NV" },
                    {"TotalMealAllowance", "Tổng P.Cơm" },
                    {"TotalNoodleAllowance", "Tổng P.Mì" }
                });

                Utils.SetGridHeaders(dataGV, new System.Collections.Generic.Dictionary<string, string> {
                    {"EmployeeName", "Tên Nhân Viên" },
                    {"EmployeeCode", "Mã NV" },
                    {"WorkDate", "Ngày Làm" },
                    {"HourWork", "Số Giờ" }
                });

                Utils.SetGridHeaders(department_GV, new System.Collections.Generic.Dictionary<string, string> {
                    {"DepartmentName", "Tên Phòng Ban" }
                });


                allowanceGV.Columns["DepartmentID"].Visible = false;
                allowanceGV.Columns["EmployeeCode"].Width = 70;
                allowanceGV.Columns["EmployeeName"].Width = 160;
                allowanceGV.Columns["TotalMealAllowance"].Width = 90;
                allowanceGV.Columns["TotalNoodleAllowance"].Width = 90;

                int count = 0;
                mOvertime_dt.Columns["EmployeeCode"].SetOrdinal(count++);
                mOvertime_dt.Columns["EmployeeName"].SetOrdinal(count++);
                mOvertime_dt.Columns["WorkDate"].SetOrdinal(count++);
                mOvertime_dt.Columns["OvertimeTypeID"].SetOrdinal(count++);
                mOvertime_dt.Columns["HourWork"].SetOrdinal(count++);
                mOvertime_dt.Columns["Note"].SetOrdinal(count++);
                                
                department_GV.Columns["DepartmentName"].Width = 200;

                dataGV.Columns["EmployeeCode"].Width = 70;
                dataGV.Columns["EmployeeName"].Width = 160;
                dataGV.Columns["WorkDate"].Width = 80;
                dataGV.Columns["HourWork"].Width = 70;

                department_GV.SelectionChanged += this.department_GV_CellClick;
                allowanceGV.SelectionChanged += this.allowanceGV_CellClick;
                if (dataGV.Rows.Count > 0)
                {
                    dataGV.ClearSelection();
                    dataGV.Rows[0].Selected = true;
                }

                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                department_GV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                allowanceGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                monthYearDtp.ValueChanged += monthYearDtp_ValueChanged;

                department_GV_CellClick(null, null);
            }
            catch
            {               
            }
            finally
            {
                await Task.Delay(100);
                loadingOverlay.Hide();
            }
        }

        private void TinhPhanAn()
        {
            foreach(DataRow row in _mAllowance_dt.Rows)
            {
                string empCode = row["EmployeeCode"].ToString();
                var rows = mOvertime_dt.AsEnumerable().Where(r => r.Field<string>("EmployeeCode") == empCode);

                int noodle = 0;
                int rice = 0;
                foreach (DataRow oRow in rows)
                {
                    decimal hourWork = Convert.ToDecimal(oRow["HourWork"]);
                    if (hourWork >= 3.0m)
                        rice++;
                    else if (hourWork >= 2.5m)
                        noodle++;                    
                }

                row["TotalMealAllowance"] = rice;
                row["TotalNoodleAllowance"] = noodle;
            }
        }

        private void department_GV_CellClick(object sender, EventArgs e)
        {
            if (department_GV.CurrentRow == null) return;
            int rowIndex = department_GV.CurrentRow.Index;
            if (rowIndex < 0)
                return;

            int departmentID = Convert.ToInt32(department_GV.CurrentRow.Cells["DepartmentID"].Value);

            mAllowanceDV.RowFilter = $"DepartmentID = {departmentID}";
            if (!filter_CB.Checked || allowanceGV.CurrentRow == null)
                mOvertimeDV.RowFilter = $"DepartmentID = {departmentID}";
            else
            {
                string employeeCode = allowanceGV.CurrentRow.Cells["EmployeeCode"].Value.ToString();
                mOvertimeDV.RowFilter = $"EmployeeCode = '{employeeCode}'";
            }

        }

        private void allowanceGV_CellClick(object sender, EventArgs e)
        {
            if (allowanceGV.CurrentRow == null) return;
            int rowIndex = allowanceGV.CurrentRow.Index;
            if (rowIndex < 0)
                return;

            if (filter_CB.Checked && allowanceGV.CurrentRow != null)
            {
                string employeeCode = allowanceGV.CurrentRow.Cells["EmployeeCode"].Value.ToString();
                mOvertimeDV.RowFilter = $"EmployeeCode = '{employeeCode}'";
            }

        }


        private async void saveBtn_Click(object sender, EventArgs e)
        {
            MessageBox.Show("tôi chưa làm chỗ này nhà bà thu");
            return;

            int month = monthYearDtp.Value.Month;
            int year = monthYearDtp.Value.Year;

            bool isLock = await SQLStore_QLNS.Instance.IsSalaryLockAsync(month, year);
            if (isLock)
            {
                MessageBox.Show("Tháng " + month + "/" + year + " đã bị khóa.", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult != DialogResult.Yes)
                return;

            int departmentID = Convert.ToInt32(department_GV.CurrentRow.Cells["DepartmentID"].Value);
            var rows = _mAllowance_dt.AsEnumerable().Where(r => r["DepartmentID"] != DBNull.Value && Convert.ToInt32(r["DepartmentID"]) == departmentID);
            
            List<(string emp, int type, int month, int year, int amount, string note)> list = new List<(string, int, int, int, int, string)>();
            foreach (DataRow row in rows)
            {
                int rice = Convert.ToInt32(row["TotalMealAllowance"]);
                string emp = Convert.ToString(row["EmployeeCode"]);
                if (rice > 0)
                {
                    int amount = rice * 25000;
                    list.Add((emp, mAllowanceTypeID, month, year, amount, "Auto"));
                }
            }
            
            bool isSucess = await SQLManager_QLNS.Instance.upsertMonthlyAllowanceBySPAsync(list);
            string messStr = isSucess == true ? "Thành Công!" : "Thất Bại!";
            MessageBox.Show(messStr, "Kết Quả", MessageBoxButtons.OK);

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

        private void PrintPreview_btn_Click(object sender, EventArgs e)
        {
            Print_PC_AnDem(true);
        }

        private void Print_btn_Click(object sender, EventArgs e)
        {
            Print_PC_AnDem(false);
        }

        private void Print_PC_AnDem(bool isPreview)
        {
            if (department_GV.CurrentRow == null) return;

            int departmentID = Convert.ToInt32(department_GV.CurrentRow.Cells["DepartmentID"].Value);
            string departmentName = department_GV.CurrentRow.Cells["DepartmentName"].Value.ToString();
            int month = monthYearDtp.Value.Month;
            int year = monthYearDtp.Value.Year;
            var print = new PCTienAnDem_Printer(departmentID, departmentName, _mAllowance_dt, mOvertime_dt, month, year);
            if (isPreview)
                print.PrintPreview(this);
            else
                print.PrintDirect();
        }

        private void Filter_CB_CheckedChanged(object sender, EventArgs e)
        {

            if (!filter_CB.Checked || allowanceGV.CurrentRow == null)
            {
                int departmentID = Convert.ToInt32(department_GV.CurrentRow.Cells["DepartmentID"].Value);
                mOvertimeDV.RowFilter = $"DepartmentID = {departmentID}";
            }
            else
            {
                string employeeCode = allowanceGV.CurrentRow.Cells["EmployeeCode"].Value.ToString();
                mOvertimeDV.RowFilter = $"EmployeeCode = '{employeeCode}'";
            }
        }
    }
}
