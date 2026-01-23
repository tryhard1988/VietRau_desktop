using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using RauViet.classes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Color = System.Drawing.Color;
using DataTable = System.Data.DataTable;

namespace RauViet.ui
{    
    public partial class SalaryCaculator : Form
    {
        private DataTable mEmployee_dt, mAllowance_dt, mOvertimeAttendance_dt, mLeaveAttendance_dt, 
            mDeduction_dt, mLeaveType_dt, mDeductionType, mAttendamce_dt, mOvertimeType_dt;
        private Timer _monthYearDebounceTimer;
        private LoadingOverlay loadingOverlay;

        private int mCurentMonth = -1;
        private int mCurentYear = -1;
        private bool isLockData = false;
        public SalaryCaculator()
        {
            InitializeComponent();
            this.KeyPreview = true;
            LuuThayDoiBtn.Visible = false;

            dataGV.EnableHeadersVisualStyles = false;
            allowancePanel.Height = this.ClientSize.Height * 2/5;
            leave_panel.Height = this.ClientSize.Height / 4;
            deductionPanel.Height = this.ClientSize.Height / 3;

            monthYearDtp.Value = DateTime.Now.AddMonths(-1);

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            allowanceGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            allowanceGV.MultiSelect = false;

            dataGV.SelectionChanged += this.dataGV_CellClick;

            LuuThayDoiBtn.Click += saveBtn_Click;
            print_pl_btn.Click += Print_pl_btn_Click;
            printPreview_pl_btn.Click += PrintPreview_pl_btn_Click; ;
            exportPDF_btn.Click += ExportPDF_btn_Click;
            this.Resize += SalaryCaculatorForm_Resize;

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            monthYearDtp.Format = DateTimePickerFormat.Custom;
            monthYearDtp.CustomFormat = "MM/yyyy";
            monthYearDtp.ShowUpDown = true;

            this.KeyDown += SalaryCaculator_KeyDown;
            this.Load += OvertimeAttendace_Load;
            Print_BCC_btn.Click += Print_BCC_btn_Click;
            preview_BCC_btn.Click += Preview_BCC_btn_Click;
            print_BTC_btn.Click += Print_BTC_btn_Click;
            printPreview_BTC_btn.Click += PrintPreview_BTC_btn_Click;
            chiHoLuong_btn.Click += ChiHoLuong_btn_Click;
        }   

        private void OvertimeAttendace_Load(object sender, EventArgs e)
        {
            _monthYearDebounceTimer = new Timer();
            _monthYearDebounceTimer.Interval = 1000;
            _monthYearDebounceTimer.Tick += MonthYearDebounceTimer_Tick;
        }

        private void SalaryCaculator_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                int month = monthYearDtp.Value.Month;
                int year = monthYearDtp.Value.Year;

                SQLStore_QLNS.Instance.removeLeaveAttendances(year);
                SQLStore_QLNS.Instance.removeAttendamce(month, year);
                SQLStore_QLNS.Instance.removeOvertimeAttendamce(month, year);
                SQLStore_QLNS.Instance.removeEmployeeSalaryInfo();
                ShowData();
            }
        }

        private void SalaryCaculatorForm_Resize(object sender, EventArgs e)
        {
            allowancePanel.Height = this.ClientSize.Height * 2 / 5;
            leave_panel.Height = this.ClientSize.Height / 4;
            deductionPanel.Height = this.ClientSize.Height / 3;
        }


        public async void ShowData()
        {
            
            await Task.Delay(50);
            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Show();

            try
            {
                monthYearDtp.ValueChanged -= monthYearDtp_ValueChanged;
                await Task.Delay(200);
                await LoadSalaryDataAsync();

                monthYearDtp.ValueChanged += monthYearDtp_ValueChanged;

            }
            catch (Exception ex)
            {
                Console.WriteLine("error: " + ex.ToString());
            }
            finally
            {
                loadingOverlay.Hide();
            }
        }

        public async Task LoadSalaryDataAsync()
        {
            int month = monthYearDtp.Value.Month;
            int year = monthYearDtp.Value.Year;

            string[] keepColumns = { "EmployeeCode", "FullName", "HireDate", "IsInsuranceRefund", "DepartmentName", "ContractTypeName", "BankAccountNumber", "BankName", "BankAccountHolder" };
            string[] keepColumnsInfo = { "BaseSalary", "InsuranceBaseSalary", "ContractTypeName", "IsInsuranceRefund" };
            string[] keepColumnsLeave = { "EmployeeCode", "LeaveTypeCode", "DateOff", "LeaveTypeName", "LeaveHours" };
            string[] keepColumnsAttendamce = { "EmployeeCode", "WorkDate", "WorkingHours" };
            var employeInfoTask = SQLStore_QLNS.Instance.GetEmployeesAsync(keepColumns);
            var annualLeaveBalanceTask = SQLStore_QLNS.Instance.GetAnnualLeaveBalanceAsync();
            var overtimeTypeAsync = SQLStore_QLNS.Instance.GetOvertimeTypeAsync();
            var leaveTypeAsync = SQLStore_QLNS.Instance.GetLeaveTypeWithPaidAsync(true);
            var deductionTypeAsync = SQLStore_QLNS.Instance.GetDeductionTypeAsync();
            var deductionAsync = SQLStore_QLNS.Instance.GetDeductionAsync(month, year);
            var overtimeAttendamceAsync = SQLStore_QLNS.Instance.GetOvertimeAttendamceAsync(month, year);
            var employeeTask = SQLStore_QLNS.Instance.GetEmployeeSalaryInfoAsync(keepColumnsInfo, month, year);
            var leaveAttendanceAsync = SQLStore_QLNS.Instance.GetLeaveAttendancesAsyn(keepColumnsLeave, month, year);
            var attendamceTask = SQLStore_QLNS.Instance.GetAttendamceAsync(keepColumnsAttendamce, month, year);
            var isLockTask = SQLStore_QLNS.Instance.IsSalaryLockAsync(month, year);
            var employeeAllowanceAsync = SQLManager_QLNS.Instance.GetEmployeeAllowanceAsync(month, year);


            await Task.WhenAll(employeInfoTask, employeeTask, employeeAllowanceAsync, overtimeAttendamceAsync, leaveAttendanceAsync,
                deductionAsync, overtimeTypeAsync, leaveTypeAsync, deductionTypeAsync, attendamceTask, annualLeaveBalanceTask, isLockTask);

            mCurentMonth = month;
            mCurentYear = year;

            isLockData = isLockTask.Result;
            mOvertimeType_dt = overtimeTypeAsync.Result;
            mEmployee_dt = employeeTask.Result;
            mAllowance_dt = employeeAllowanceAsync.Result;
            mOvertimeAttendance_dt = overtimeAttendamceAsync.Result;
            mLeaveAttendance_dt = leaveAttendanceAsync.Result;
            mDeduction_dt = deductionAsync.Result;
            mLeaveType_dt = leaveTypeAsync.Result;
            mDeductionType = deductionTypeAsync.Result;
            mAttendamce_dt = attendamceTask.Result;



            AddColumnIfNotExists(mEmployee_dt, "EmployeeCode", typeof(string));
            AddColumnIfNotExists(mEmployee_dt, "FullName", typeof(string));
            AddColumnIfNotExists(mEmployee_dt, "HireDate", typeof(DateTime));
            AddColumnIfNotExists(mEmployee_dt, "IsInsuranceRefund", typeof(bool));
            AddColumnIfNotExists(mEmployee_dt, "ContractTypeName", typeof(string));
            AddColumnIfNotExists(mEmployee_dt, "DepartmentName", typeof(string));
            AddColumnIfNotExists(mEmployee_dt, "RemainingLeave", typeof(int));
            AddColumnIfNotExists(mEmployee_dt, "TotalIncludedInsurance", typeof(int));
            AddColumnIfNotExists(mEmployee_dt, "TotalExcludedInsurance", typeof(int));
            AddColumnIfNotExists(mEmployee_dt, "TotalInsuranceSalary", typeof(int));
            AddColumnIfNotExists(mEmployee_dt, "EmployeeInsurancePaid", typeof(int));
            AddColumnIfNotExists(mEmployee_dt, "HourSalary", typeof(decimal));
            AddColumnIfNotExists(mEmployee_dt, "TotalHourWork", typeof(decimal));
            AddColumnIfNotExists(mEmployee_dt, "TotalSalaryHourWork", typeof(decimal));
            AddColumnIfNotExists(mEmployee_dt, "InsuranceRefund", typeof(decimal));
            AddColumnIfNotExists(mEmployee_dt, "NetSalary", typeof(decimal));
            AddColumnIfNotExists(mEmployee_dt, "BankAccountNumber", typeof(string));
            AddColumnIfNotExists(mEmployee_dt, "BankName", typeof(string));
            AddColumnIfNotExists(mEmployee_dt, "BankAccountHolder", typeof(string));

            AddColumnIfNotExists(mAttendamce_dt, "DayOfWeek", typeof(string));
            

            await AddDynamicColumnsAsync();
            await CalculateAllSalaries(employeInfoTask.Result, annualLeaveBalanceTask.Result);

            var t1 = Task.Run(() => SetupAllGrids());
            var t2 = Task.Run(() => SetupDataGridViewHeaders());

            await Task.WhenAll(t1, t2);

            //if (dataGV.Rows.Count > 0)
            //{
            //    dataGV.ClearSelection();
            //    dataGV.Rows[0].Selected = true;
            //    //   UpdateAllowancetUI(0);
            //}

            LuuThayDoiBtn.Visible = !isLockData;

            dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            allowanceGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        public async Task CalculateAllSalaries(DataTable employeInfo, DataTable annualLeaveBalance)
        {
            var insuranceResult = from row in mAllowance_dt.AsEnumerable()
                                  where row.Field<string>("ScopeCode") != "ONCE"
                                  group row by row.Field<string>("EmployeeCode") into g
                                  select new
                                  {
                                      EmployeeCode = g.Key,
                                      TotalIncludedInsurance = g
                                          .Where(r => Convert.ToBoolean(r["IsInsuranceIncluded"]))
                                          .Sum(r => r["Amount"] == DBNull.Value ? 0 : Convert.ToInt32(r["Amount"])),
                                      TotalExcludedInsurance = g
                                          .Where(r => !Convert.ToBoolean(r["IsInsuranceIncluded"]))
                                          .Sum(r => r["Amount"] == DBNull.Value ? 0 : Convert.ToInt32(r["Amount"]))
                                  };

            var attendamceResult = from row in mAttendamce_dt.AsEnumerable()
                                   group row by row.Field<string>("EmployeeCode") into g
                                   select new
                                   {
                                       EmployeeCode = g.Key,
                                       TotalHourWork = g.Sum(r => r["WorkingHours"] == DBNull.Value ? 0 : Convert.ToDecimal(r["WorkingHours"]))
                                   };

            var employeeLookup = employeInfo.AsEnumerable().ToDictionary(r => r.Field<string>("EmployeeCode"));
            var annualLeaveBalanceLookup = annualLeaveBalance.AsEnumerable().ToDictionary(r => r.Field<string>("EmployeeCode"));
            _ = Task.Run(async () =>
            {
                foreach (DataRow dr in mEmployee_dt.Rows)
                {
                    string employeeCode = Convert.ToString(dr["EmployeeCode"]);

                    if (employeeLookup.TryGetValue(employeeCode, out DataRow matchRow))
                    {
                        if (!isLockData)
                        {
                            dr["DepartmentName"] = Convert.ToString(matchRow["DepartmentName"]);
                            dr["ContractTypeName"] = Convert.ToString(matchRow["ContractTypeName"]);
                            dr["IsInsuranceRefund"] = Convert.ToBoolean(matchRow["IsInsuranceRefund"]);
                        }
                        dr["HireDate"] = Convert.ToDateTime(matchRow["HireDate"]);
                        dr["FullName"] = matchRow["FullName"]?.ToString();
                        dr["BankAccountNumber"] = matchRow["BankAccountNumber"]?.ToString();
                        dr["BankName"] = matchRow["BankName"]?.ToString();
                        dr["BankAccountHolder"] = matchRow["BankAccountHolder"]?.ToString();
                    }
                    if (annualLeaveBalanceLookup.TryGetValue(employeeCode, out DataRow matchRow1))
                    {
                        dr["RemainingLeave"] = Convert.ToInt32(matchRow1["RemainingLeaveDays_1"]);
                    }
                    else
                    {
                        dr["RemainingLeave"] = 0;
                    }
                }
            });

            // Copy dữ liệu đầu vào (thread-safe)
            var rows = mEmployee_dt.AsEnumerable().ToList();

            // Mảng kết quả tạm
            var resultList = new ConcurrentBag<(string EmployeeCode, decimal HourSalary, int EmployeeInsurancePaid, int TotalIncludedInsurance, int TotalExcludedInsurance, int TotalInsuranceSalary, int InsuranceRefund)>();

            Parallel.ForEach(rows, dr =>
            {
                string employeeCode = Convert.ToString(dr["EmployeeCode"]);

                if (!employeeLookup.TryGetValue(employeeCode, out DataRow matchRow))
                    return;
                
                bool isInsuranceRefund = isLockData ? Convert.ToBoolean(dr["IsInsuranceRefund"]): Convert.ToBoolean(matchRow["IsInsuranceRefund"]);
                int insuranceBaseSalary = dr["InsuranceBaseSalary"] == DBNull.Value ? 0 : Convert.ToInt32(dr["InsuranceBaseSalary"]);
                int baseSalary = dr["BaseSalary"] == DBNull.Value ? 0 : Convert.ToInt32(dr["BaseSalary"]);

                var match = insuranceResult.FirstOrDefault(r => r.EmployeeCode == employeeCode);
                int totalIncludedInsurance = match?.TotalIncludedInsurance ?? 0;
                int totalExcludedInsurance = match?.TotalExcludedInsurance ?? 0;
                int employeeInsurancePaid = Convert.ToInt32((totalIncludedInsurance + insuranceBaseSalary) * 0.105m);

                double daySalary = ((baseSalary - employeeInsurancePaid) + totalIncludedInsurance + totalExcludedInsurance) / 26.0;
                decimal hourSalary = Math.Round((decimal)(daySalary / 8.0), 10);

                resultList.Add((
                    employeeCode,
                    hourSalary,
                    employeeInsurancePaid,
                    totalIncludedInsurance,
                    totalExcludedInsurance,
                    totalIncludedInsurance + insuranceBaseSalary,
                    isInsuranceRefund ? employeeInsurancePaid : 0
                ));
            });

            // Sau khi tính xong, cập nhật lại DataTable trên UI/main thread
            
            foreach (var r in resultList)
            {
                var dr = mEmployee_dt.AsEnumerable()
                    .FirstOrDefault(x => x.Field<string>("EmployeeCode") == r.EmployeeCode);
                if (dr == null) continue;

                dr["TotalIncludedInsurance"] = r.TotalIncludedInsurance;
                dr["TotalExcludedInsurance"] = r.TotalExcludedInsurance;
                dr["TotalInsuranceSalary"] = r.TotalInsuranceSalary;
                dr["EmployeeInsurancePaid"] = r.EmployeeInsurancePaid;
                dr["HourSalary"] = r.HourSalary;
                dr["InsuranceRefund"] = r.InsuranceRefund;
            }

            // ====== 4️⃣ Tính tiền tăng ca ======
            AddColumnIfNotExists(mOvertimeAttendance_dt, "OvertimeAttendanceSalary", typeof(decimal));

            foreach (DataRow dr in mOvertimeAttendance_dt.Rows)
            {
                string employeeCode = Convert.ToString(dr["EmployeeCode"]);
                decimal salaryFactor = dr["SalaryFactor"] == DBNull.Value ? 1 : Convert.ToDecimal(dr["SalaryFactor"]);
                decimal hourWork = dr["HourWork"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["HourWork"]);

                var employeeRow = mEmployee_dt.AsEnumerable()
                    .FirstOrDefault(r => r.Field<string>("EmployeeCode") == employeeCode);

                decimal hourSalary = employeeRow?.Field<decimal>("HourSalary") ?? 0;

                dr["OvertimeAttendanceSalary"] = Math.Round(hourSalary * hourWork * salaryFactor, 1);
            }

            // ====== 5️⃣ Tổng tiền tăng ca theo nhân viên ======
            foreach (DataRow dr in mEmployee_dt.Rows)
            {
                string employeeCode = Convert.ToString(dr["EmployeeCode"]);
                decimal hourSalary = Convert.ToDecimal(dr["HourSalary"]);

                //luong
                decimal totalIncludedInsurance = Convert.ToDecimal(dr["TotalIncludedInsurance"]);
                decimal totalExcludedInsurance = Convert.ToDecimal(dr["TotalExcludedInsurance"]);
                decimal totalSalaryHourWork = 0; ;
                decimal insuranceRefund = Convert.ToDecimal(dr["InsuranceRefund"]);
                decimal overtimeMoney = 0;
                decimal leaveMoney = 0;
                decimal allowanceONCE = 0;

                //các khoảng trừ
                decimal deductionMoney = 0;
                decimal employeeInsurancePaid = Convert.ToDecimal(dr["EmployeeInsurancePaid"]);

                var hwMatch = attendamceResult.FirstOrDefault(r => r.EmployeeCode == employeeCode);
                decimal totalHourWork = hwMatch?.TotalHourWork ?? 0;
                totalSalaryHourWork = totalHourWork * hourSalary;

                dr["TotalHourWork"] = totalHourWork;
                dr["TotalSalaryHourWork"] = totalSalaryHourWork;

                DataTable empAllowance = mAllowance_dt.Clone();
                foreach (DataRow adr in mAllowance_dt.Select($"EmployeeCode = '{employeeCode}'"))
                    empAllowance.ImportRow(adr);

                foreach (DataRow adr in empAllowance.Rows)
                {
                    string scopeCode = adr["ScopeCode"].ToString();

                    if (scopeCode.CompareTo("ONCE") == 0)
                    {
                        int allowanceTypeID = Convert.ToInt32(adr["AllowanceTypeID"]);
                        decimal amount = Convert.ToDecimal(adr["Amount"]);
                        dr["Allowance" + adr["AllowanceTypeID"].ToString()] = amount;
                        allowanceONCE += amount;
                    }
                }

                foreach (DataRow otdr in mOvertimeType_dt.Rows)
                {
                    int overtimeTypeID = Convert.ToInt32(otdr["OvertimeTypeID"]);
                    var filteredRows = mOvertimeAttendance_dt.AsEnumerable().Where(r => r.Field<string>("EmployeeCode") == employeeCode && r.Field<int>("OvertimeTypeID") == overtimeTypeID);

                    decimal total = filteredRows.Any() ? filteredRows.Sum(r => r.Field<decimal>("OvertimeAttendanceSalary")) : 0m;
                    decimal totalhour = filteredRows.Any() ? filteredRows.Sum(r => r.Field<decimal>("HourWork")) : 0m;

                    dr["c_OvertimeType" + overtimeTypeID.ToString()] = totalhour;
                    dr["OvertimeType" + overtimeTypeID.ToString()] = total;
                    overtimeMoney += total;
                }

                foreach (DataRow ltdr in mLeaveType_dt.Rows)
                {
                    string leaveTypeCode = Convert.ToString(ltdr["LeaveTypeCode"]);
                    var sumLeaveHours = mLeaveAttendance_dt.AsEnumerable().Where(r => r.Field<string>("EmployeeCode") == employeeCode &&
                                                                                        r.Field<string>("LeaveTypeCode") == leaveTypeCode)
                                                                            .Sum(r => r.Field<decimal>("LeaveHours"));
                    decimal temp = sumLeaveHours * hourSalary;
                    dr["LeaveType" + leaveTypeCode] = temp;
                    dr["c_LeaveType" + leaveTypeCode] = sumLeaveHours;
                    leaveMoney += temp;
                }

                foreach (DataRow ltdr in mDeductionType.Rows)
                {
                    string deductionTypeCode = Convert.ToString(ltdr["DeductionTypeCode"]);
                    var total = mDeduction_dt.AsEnumerable().Where(r => r.Field<string>("EmployeeCode") == employeeCode && r.Field<string>("DeductionTypeCode") == deductionTypeCode).Sum(r => r.Field<int>("Amount"));

                    dr["DeductionType" + deductionTypeCode] = total;
                    deductionMoney += total;
                }



                decimal netSalary = totalSalaryHourWork + insuranceRefund + overtimeMoney + leaveMoney + allowanceONCE;
                netSalary -= (deductionMoney);
                dr["NetSalary"] = netSalary;

            }
        }

        public async Task SetupAllGrids()
        {
            // 🔹 Task 1️⃣: Sắp xếp cột Employee
            var t1 = Task.Run(() =>
            {
                int countE = 0;
                mEmployee_dt.Columns["EmployeeCode"].SetOrdinal(countE++);
                mEmployee_dt.Columns["FullName"].SetOrdinal(countE++);
                mEmployee_dt.Columns["DepartmentName"].SetOrdinal(countE++);
                mEmployee_dt.Columns["ContractTypeName"].SetOrdinal(countE++);
                mEmployee_dt.Columns["NetSalary"].SetOrdinal(countE++);
                mEmployee_dt.Columns["BaseSalary"].SetOrdinal(countE++);
                mEmployee_dt.Columns["TotalSalaryHourWork"].SetOrdinal(countE++);
                mEmployee_dt.Columns["TotalHourWork"].SetOrdinal(countE++);
                mEmployee_dt.Columns["HourSalary"].SetOrdinal(countE++);
                mEmployee_dt.Columns["InsuranceBaseSalary"].SetOrdinal(countE++);
                mEmployee_dt.Columns["TotalInsuranceSalary"].SetOrdinal(countE++);
                mEmployee_dt.Columns["EmployeeInsurancePaid"].SetOrdinal(countE++);
                mEmployee_dt.Columns["InsuranceRefund"].SetOrdinal(countE++);
            });

            // 🔹 Task 2️⃣: Sắp xếp cột Leave
            var t2 = Task.Run(() =>
            {
                int countL = 0;
                mLeaveAttendance_dt.Columns["DateOff"].SetOrdinal(countL++);
                mLeaveAttendance_dt.Columns["LeaveTypeName"].SetOrdinal(countL++);
                mLeaveAttendance_dt.Columns["LeaveHours"].SetOrdinal(countL++);
            });

            var t3 = Task.Run(() =>
            {
                int countAttendamce = 0;
                mAttendamce_dt.Columns["DayOfWeek"].SetOrdinal(countAttendamce++);
                mAttendamce_dt.Columns["WorkDate"].SetOrdinal(countAttendamce++);
                mAttendamce_dt.Columns["WorkingHours"].SetOrdinal(countAttendamce++);
            });

            // 🔹 Chờ 2 task nền xong
            await Task.WhenAll(t1, t2, t3);

            // 🔹 Gán DataSource (phải trên UI thread)
            this.Invoke(new Action(() =>
            {
                this.SuspendLayout();
                try
                {
                    overtimeAttendanceGV.DataSource = null;
                    allowanceGV.DataSource = null;
                    leaveGV.DataSource = null;
                    deductionGV.DataSource = null;
                    attendamceGV.DataSource = null;
                    dataGV.DataSource = null;

                    overtimeAttendanceGV.DataSource = mOvertimeAttendance_dt;
                    allowanceGV.DataSource = mAllowance_dt;
                    leaveGV.DataSource = mLeaveAttendance_dt;
                    deductionGV.DataSource = mDeduction_dt;
                    attendamceGV.DataSource = mAttendamce_dt;
                    dataGV.DataSource = mEmployee_dt;
                }
                finally
                {
                    this.ResumeLayout();
                }

            }));

            // 🔹 Các task còn lại (ẩn cột, header, màu, width) có thể chạy song song
            var tHideCols = Task.Run(() => this.Invoke(new Action(() => HideUnnecessaryColumns())));
            var tHeaders = Task.Run(() => this.Invoke(new Action(() => _ = SetupGridThemesAsync())));

            await Task.WhenAll(tHideCols, tHeaders);
        }

        // 🧩 Gọn hơn bằng cách tách nhỏ thành 2 hàm con
        private void HideUnnecessaryColumns()
        {
            Utils.HideColumns(allowanceGV, new[] { "EmployeeCode", "AllowanceTypeID", "ScopeCode" });
            Utils.HideColumns(overtimeAttendanceGV, new[] { "EmployeeCode", "SalaryFactor", "OvertimeTypeID", "OvertimeAttendanceID", "StartTime", "EndTime", "Note", "UpdatedHistory", "EmployeeName", "DepartmentID", "OvertimeTypeCode" });
            Utils.HideColumns(leaveGV, new[] { "EmployeeCode", "LeaveTypeCode" });
            Utils.HideColumns(deductionGV, new[] { "EmployeeCode", "DeductionTypeCode", "EmployeeDeductionID", "Note", "updateHistory" });
            Utils.HideColumns(dataGV, new[] { "IsInsuranceRefund", "HireDate", "RemainingLeave", "BankAccountNumber", "BankName", "BankAccountHolder" });
        }

        public async Task SetupGridThemesAsync()
        {
            // Tạo danh sách các task để chạy song song
            var tasks = new List<Task>();

            // Task 1️⃣: Ẩn các cột không cần thiết
            tasks.Add(Task.Run(() =>
            {
                this.Invoke(new Action(() =>
                {
                    attendamceGV.Columns["EmployeeCode"].Visible = false;
                   // allowanceGV.Columns["EmployeeCode"].Visible = false;
                  //  allowanceGV.Columns["AllowanceTypeID"].Visible = false;
                  //  allowanceGV.Columns["ScopeCode"].Visible = false;
                  //  overtimeAttendanceGV.Columns["EmployeeCode"].Visible = false;
                  //  overtimeAttendanceGV.Columns["SalaryFactor"].Visible = false;
                  //  overtimeAttendanceGV.Columns["OvertimeTypeID"].Visible = false;
                    //leaveGV.Columns["EmployeeCode"].Visible = false;
                    //leaveGV.Columns["LeaveTypeCode"].Visible = false;
                   // deductionGV.Columns["EmployeeCode"].Visible = false;
                   // deductionGV.Columns["DeductionTypeCode"].Visible = false;
                   // dataGV.Columns["IsInsuranceRefund"].Visible = false;
                   // dataGV.Columns["HireDate"].Visible = false;
                   // dataGV.Columns["RemainingLeave"].Visible = false;
                }));
            }));

            // Task 2️⃣: Header & Width cho allowanceGV
            tasks.Add(Task.Run(() =>
            {
                this.Invoke(new Action(() =>
                {
                    Utils.SetGridHeaders(allowanceGV, new System.Collections.Generic.Dictionary<string, string> {
                        {"AllowanceName", "Phụ Cấp" },
                        {"Amount", "Số Tiền" },
                        {"IsInsuranceIncluded", "BHXH" }
                    });

                    Utils.SetGridWidths(allowanceGV, new System.Collections.Generic.Dictionary<string, int> {
                        {"AllowanceName", 120},
                        {"Amount", 70},
                        {"IsInsuranceIncluded", 40}
                    });

                }));
            }));

            // Task 3️⃣: Header & Width cho deductionGV
            tasks.Add(Task.Run(() =>
            {
                this.Invoke(new Action(() =>
                {
                    Utils.SetGridHeaders(deductionGV, new System.Collections.Generic.Dictionary<string, string> {
                        {"DeductionDate", "Ngày Trừ" },
                        {"DeductionTypeName", "Loại Tiền" },
                        {"Amount", "Số Tiền" }
                    });

                    Utils.SetGridWidths(deductionGV, new System.Collections.Generic.Dictionary<string, int> {
                        {"DeductionDate", 70},
                        {"DeductionTypeName", 80},
                        {"Amount", 60}
                    });

                }));
            }));

            // Task 4️⃣: Setup attendamceGV
            tasks.Add(Task.Run(() =>
            {
                this.Invoke(new Action(() =>
                {
                    Utils.SetGridHeaders(attendamceGV, new System.Collections.Generic.Dictionary<string, string> {
                        {"DayOfWeek", "Thứ" },
                        {"WorkDate", "Ngày Làm" },
                        {"WorkingHours", "S.Giờ" }
                    });

                    Utils.SetGridWidths(attendamceGV, new System.Collections.Generic.Dictionary<string, int> {
                        {"DayOfWeek", 40},
                        {"WorkDate", 80},
                        {"WorkingHours", 50}
                    });
                }));
            }));

            // Task 5️⃣: Setup overtimeAttendanceGV
            tasks.Add(Task.Run(() =>
            {
                this.Invoke(new Action(() =>
                {
                    Utils.SetGridHeaders(overtimeAttendanceGV, new System.Collections.Generic.Dictionary<string, string> {
                        {"WorkDate", "Ngày T.Ca" },
                        {"OvertimeName", "Loại T.Ca" },
                        {"HourWork", "S.Giờ" },
                        {"OvertimeAttendanceSalary", "T.Công" },
                        {"DayOfWeek", "Thứ" }
                    });

                    Utils.SetGridWidths(overtimeAttendanceGV, new System.Collections.Generic.Dictionary<string, int> {
                        {"WorkDate", 70},
                        {"OvertimeName", 70},
                        {"HourWork", 40},
                        {"OvertimeAttendanceSalary", 60},
                        {"DayOfWeek", 40}
                    });

                    Utils.SetGridFormat_NO(overtimeAttendanceGV, "OvertimeAttendanceSalary");
                }));
            }));

            // Task 6️⃣: Setup leaveGV
            tasks.Add(Task.Run(() =>
            {
                this.Invoke(new Action(() =>
                {
                    Utils.SetGridHeaders(leaveGV, new System.Collections.Generic.Dictionary<string, string> {
                        {"LeaveTypeName", "Loại Ngày Nghỉ" },
                        {"DateOff", "Ngày Nghỉ" },
                        {"LeaveHours", "S.Giờ" }
                    });

                    Utils.SetGridWidths(leaveGV, new System.Collections.Generic.Dictionary<string, int> {
                        {"DateOff", 80},
                        {"LeaveHours", 50},
                        {"LeaveTypeName", 130}
                    });
                }));
            }));

            // Task 7️⃣: Setup dataGV (nhiều nhất)
            tasks.Add(Task.Run(() =>
            {
                this.Invoke(new Action(() =>
                {
                    Utils.SetGridHeaders(dataGV, new System.Collections.Generic.Dictionary<string, string> {
                        {"EmployeeCode", "Mã NV" },
                        {"FullName", "Tên Nhân Viên" },
                        {"DepartmentName", "Phòng Ban" },
                        {"ContractTypeName", "Hợp Đồng" },
                        {"NetSalary", "Thực Lãnh" },
                        {"BaseSalary", "Lương CB" },
                        {"InsuranceBaseSalary", "Lương C.S Đóng BHXH" },
                        {"TotalInsuranceSalary", "Lương Tính Đóng BHXH" },
                        {"TotalSalaryHourWork", "T.Lương Theo Giờ" },
                        {"TotalHourWork", "Giờ Công Làm" },
                        {"HourSalary", "Tiền 1 Giờ Làm" },
                        {"EmployeeInsurancePaid", "Tiền NV Nộp BHXH" },
                        {"InsuranceRefund", "Hoàn Trả BHXH" },
                        {"TotalIncludedInsurance", "PC Đóng BHXH" },
                        {"TotalExcludedInsurance", "PC K.Đóng BHXH" },
                    });

                    dataGV.Columns["EmployeeCode"].Frozen = true;
                    dataGV.Columns["FullName"].Frozen = true;
                    dataGV.Columns["NetSalary"].Frozen = true;

                    Utils.SetGridFormat_NO(overtimeAttendanceGV, "NetSalary");
                    Utils.SetGridFormat_NO(overtimeAttendanceGV, "HourSalary");
                    Utils.SetGridFormat_NO(overtimeAttendanceGV, "BaseSalary");
                    Utils.SetGridFormat_NO(overtimeAttendanceGV, "TotalSalaryHourWork");
                    Utils.SetGridFormat_NO(overtimeAttendanceGV, "InsuranceBaseSalary");
                    Utils.SetGridFormat_NO(overtimeAttendanceGV, "TotalInsuranceSalary");
                    Utils.SetGridFormat_NO(overtimeAttendanceGV, "EmployeeInsurancePaid");
                    Utils.SetGridFormat_NO(overtimeAttendanceGV, "TotalIncludedInsurance");
                    Utils.SetGridFormat_NO(overtimeAttendanceGV, "TotalExcludedInsurance");
                    Utils.SetGridFormat_NO(overtimeAttendanceGV, "InsuranceRefund");    

                    foreach (DataRow otdr in mOvertimeType_dt.Rows)
                    {
                        int overtimeTypeID = Convert.ToInt32(otdr["OvertimeTypeID"]);
                        Utils.SetGridFormat_NO(dataGV, "OvertimeType" + overtimeTypeID.ToString());
                    }

                    foreach (DataRow ltdr in mLeaveType_dt.Rows)
                    {
                        string leaveTypeCode = Convert.ToString(ltdr["LeaveTypeCode"]);
                        Utils.SetGridFormat_NO(dataGV, "LeaveType" + leaveTypeCode.ToString());
                    }


                    Utils.SetGridWidths(dataGV, new System.Collections.Generic.Dictionary<string, int> {
                        {"EmployeeCode", 50},
                        {"FullName", 120},
                        {"NetSalary", 70},
                        {"BaseSalary", 70},
                        {"TotalSalaryHourWork", 70},
                        {"TotalHourWork", 50},
                        {"HourSalary", 50},
                        {"InsuranceBaseSalary", 70},
                        {"TotalInsuranceSalary", 70},
                        {"EmployeeInsurancePaid", 50},
                        {"InsuranceRefund", 50},
                        {"TotalIncludedInsurance", 50},
                        {"TotalExcludedInsurance", 50}
                    });

                    Utils.SetGridColors(dataGV, new System.Collections.Generic.Dictionary<string, System.Drawing.Color> {
                        {"NetSalary", Color.BurlyWood},
                        {"InsuranceBaseSalary", Color.Coral},
                        {"TotalInsuranceSalary", Color.Coral},
                        {"EmployeeInsurancePaid", Color.Coral},
                        {"InsuranceRefund", Color.Coral},
                        {"TotalIncludedInsurance", Color.Coral},
                        {"TotalExcludedInsurance", Color.Coral}
                    });

                }));
            }));

            // Task 8️⃣: Setup các cột động (Allowance, Overtime, Leave, Deduction)
            tasks.Add(Task.Run(() =>
            {
                this.Invoke(new Action(() =>
                {
                    foreach (DataRow adr in mAllowance_dt.Rows)
                    {
                        if (adr["ScopeCode"].ToString() == "ONCE")
                        {
                            string col = "Allowance" + adr["AllowanceTypeID"];
                            dataGV.Columns[col].HeaderText = adr["AllowanceName"].ToString();
                            dataGV.Columns[col].Width = 50;
                            dataGV.Columns[col].HeaderCell.Style.BackColor = Color.Coral;
                        }
                    }

                    foreach (DataRow otdr in mOvertimeType_dt.Rows)
                    {
                        string col = "OvertimeType" + otdr["OvertimeTypeID"];
                        dataGV.Columns["c_" + col].Visible = false;
                        dataGV.Columns[col].HeaderText = otdr["OvertimeName"].ToString();
                        dataGV.Columns[col].Width = 50;
                        dataGV.Columns[col].HeaderCell.Style.BackColor = Color.Yellow;
                    }

                    foreach (DataRow ltdr in mLeaveType_dt.Rows)
                    {
                        string col = "LeaveType" + ltdr["LeaveTypeCode"];
                        dataGV.Columns["c_" + col].Visible = false;
                        dataGV.Columns[col].HeaderText = ltdr["LeaveTypeName"].ToString();
                        dataGV.Columns[col].Width = 50;
                        dataGV.Columns[col].HeaderCell.Style.BackColor = Color.Aqua;
                    }

                    foreach (DataRow ltdr in mDeductionType.Rows)
                    {
                        string col = "DeductionType" + ltdr["DeductionTypeCode"];
                        dataGV.Columns[col].HeaderText = ltdr["DeductionTypeName"].ToString();
                        dataGV.Columns[col].Width = 50;
                        dataGV.Columns[col].HeaderCell.Style.BackColor = Color.Gray;
                    }
                }));
            }));

            // 🔹 Chờ tất cả Task hoàn tất
            //await Task.WhenAll(tasks);
        }




        public void SetupDataGridViewHeaders()
        {
            // Task 1️⃣: Allowance
            var allowanceTask = Task.Run(() =>
            {
                var allowanceHeaders = mAllowance_dt.AsEnumerable()
                    .Where(r => r.Field<string>("ScopeCode") == "ONCE")
                    .Select(r => new
                    {
                        ColumnName = "Allowance" + r["AllowanceTypeID"],
                        HeaderText = r["AllowanceName"].ToString(),
                        BackColor = Color.Coral
                    }).ToList();

                dataGV.Invoke(new Action(() =>
                {
                    foreach (var item in allowanceHeaders)
                    {
                        if (dataGV.Columns.Contains(item.ColumnName))
                        {
                            dataGV.Columns[item.ColumnName].HeaderText = item.HeaderText;
                            dataGV.Columns[item.ColumnName].Width = 50;
                            dataGV.Columns[item.ColumnName].HeaderCell.Style.BackColor = item.BackColor;
                        }
                    }
                }));
            });

            // Task 2️⃣: Overtime
            var overtimeTask = Task.Run(() =>
            {
                var overtimeHeaders = mOvertimeType_dt.AsEnumerable()
                    .Select(r => new
                    {
                        ColumnName = "OvertimeType" + r["OvertimeTypeID"],
                        HideColumn = "c_OvertimeType" + r["OvertimeTypeID"],
                        HeaderText = r["OvertimeName"].ToString(),
                        BackColor = Color.Yellow
                    }).ToList();

                dataGV.Invoke(new Action(() =>
                {
                    foreach (var item in overtimeHeaders)
                    {
                        if (dataGV.Columns.Contains(item.HideColumn))
                            dataGV.Columns[item.HideColumn].Visible = false;

                        if (dataGV.Columns.Contains(item.ColumnName))
                        {
                            dataGV.Columns[item.ColumnName].HeaderText = item.HeaderText;
                            dataGV.Columns[item.ColumnName].Width = 50;
                            dataGV.Columns[item.ColumnName].HeaderCell.Style.BackColor = item.BackColor;
                        }
                    }
                }));
            });

            // Task 3️⃣: Leave
            var leaveTask = Task.Run(() =>
            {
                var leaveHeaders = mLeaveType_dt.AsEnumerable()
                    .Select(r => new
                    {
                        ColumnName = "LeaveType" + r["LeaveTypeCode"],
                        HideColumn = "c_LeaveType" + r["LeaveTypeCode"],
                        HeaderText = r["LeaveTypeName"].ToString(),
                        BackColor = Color.Aqua
                    }).ToList();

                dataGV.Invoke(new Action(() =>
                {
                    foreach (var item in leaveHeaders)
                    {
                        if (dataGV.Columns.Contains(item.HideColumn))
                            dataGV.Columns[item.HideColumn].Visible = false;

                        if (dataGV.Columns.Contains(item.ColumnName))
                        {
                            dataGV.Columns[item.ColumnName].HeaderText = item.HeaderText;
                            dataGV.Columns[item.ColumnName].Width = 50;
                            dataGV.Columns[item.ColumnName].HeaderCell.Style.BackColor = item.BackColor;
                        }
                    }
                }));
            });

            // Task 4️⃣: Deduction
            var deductionTask = Task.Run(() =>
            {
                var deductionHeaders = mDeductionType.AsEnumerable()
                    .Select(r => new
                    {
                        ColumnName = "DeductionType" + r["DeductionTypeCode"],
                        HeaderText = r["DeductionTypeName"].ToString(),
                        BackColor = Color.Gray
                    }).ToList();

                dataGV.Invoke(new Action(() =>
                {
                    foreach (var item in deductionHeaders)
                    {
                        if (dataGV.Columns.Contains(item.ColumnName))
                        {
                            dataGV.Columns[item.ColumnName].HeaderText = item.HeaderText;
                            dataGV.Columns[item.ColumnName].Width = 50;
                            dataGV.Columns[item.ColumnName].HeaderCell.Style.BackColor = item.BackColor;
                        }
                    }
                }));
            });

            // ✅ Chạy song song cả 4 thread, không chờ đợi (fire and forget)
            Task.WhenAll(allowanceTask, overtimeTask, leaveTask, deductionTask);
        }


        void AddColumnIfNotExists(DataTable dt, string name, Type type)
        {
            if (!dt.Columns.Contains(name))
                dt.Columns.Add(new DataColumn(name, type));
        }
        public async Task AddDynamicColumnsAsync()
        {
            // 🔹 1️⃣ Chạy song song các tác vụ để gom danh sách cột
            var allowanceTask = Task.Run(() =>
            {
                return mAllowance_dt.AsEnumerable()
                    .Where(r => r.Field<string>("ScopeCode") == "ONCE")
                    .Select(r => "Allowance" + r["AllowanceTypeID"])
                    .ToList();
            });

            var overtimeTask = Task.Run(() =>
            {
                return mOvertimeType_dt.AsEnumerable()
                    .SelectMany(r => new[]
                    {
                "OvertimeType" + r["OvertimeTypeID"],
                "c_OvertimeType" + r["OvertimeTypeID"]
                    })
                    .ToList();
            });

            var leaveTask = Task.Run(() =>
            {
                return mLeaveType_dt.AsEnumerable()
                    .SelectMany(r => new[]
                    {
                "LeaveType" + r["LeaveTypeCode"],
                "c_LeaveType" + r["LeaveTypeCode"]
                    })
                    .ToList();
            });

            var deductionTask = Task.Run(() =>
            {
                return mDeductionType.AsEnumerable()
                    .Select(r => "DeductionType" + r["DeductionTypeCode"])
                    .ToList();
            });

            var attendamceStask = Task.Run(() =>
            {

                string[] vietDays = { "CN", "T.2", "T.3", "T.4", "T.5", "T.6", "T.7" };

                foreach (DataRow row in mAttendamce_dt.Rows)
                {
                    DateTime dt = Convert.ToDateTime(row["WorkDate"]);
                    row["DayOfWeek"] = vietDays[(int)dt.DayOfWeek];
                }
            });

            // 🔹 2️⃣ Đợi tất cả task hoàn thành
            await Task.WhenAll(allowanceTask, overtimeTask, leaveTask, deductionTask, attendamceStask);

            // 🔹 3️⃣ Gom danh sách tất cả cột
            var allCols = allowanceTask.Result
                .Concat(overtimeTask.Result)
                .Concat(leaveTask.Result)
                .Concat(deductionTask.Result)
                .Distinct()
                .ToList();

            // 🔹 4️⃣ Thêm cột vào DataTable (chỉ 1 thread thực hiện)
            foreach (var col in allCols)
            {
                AddColumnIfNotExists(mEmployee_dt, col, typeof(decimal));
            }
        }
                private void dataGV_CellClick(object sender, EventArgs e)
        {
            if (dataGV.CurrentRow == null) return;
            int rowIndex = dataGV.CurrentRow.Index;
            if (rowIndex < 0)
                return;

            _ = Task.Run(() =>{this.Invoke(new Action(() => UpdateAllowanceUI(rowIndex)));});
            _ = Task.Run(() => {this.Invoke(new Action(() => UpdateOvertimeAttendanceUI(rowIndex)));});
            _ = Task.Run(() => { this.Invoke(new Action(() => UpdateLeaveAttendanceUI(rowIndex))); });
            _ = Task.Run(() => { this.Invoke(new Action(() => UpdateDeductionUI(rowIndex))); });
            _ = Task.Run(() => { this.Invoke(new Action(() => UpdateAttendandeUI(rowIndex))); });
        }

        private void UpdateAttendandeUI(int index)
        {
            attendamceGV.ClearSelection();

            var cells = dataGV.Rows[index].Cells;
            string employeeCode = Convert.ToString(cells["EmployeeCode"].Value);

            DataView dv = new DataView(mAttendamce_dt);
            dv.RowFilter = $"EmployeeCode = '{employeeCode}'";

            attendamceGV.DataSource = dv;
        }
        private void UpdateAllowanceUI(int index)
        {
            allowanceGV.ClearSelection();

            var cells = dataGV.Rows[index].Cells;
            string employeeCode = Convert.ToString(cells["EmployeeCode"].Value);

            DataView dv = new DataView(mAllowance_dt);
            dv.RowFilter = $"EmployeeCode = '{employeeCode}'";

            allowanceGV.DataSource = dv;
        }

        private void UpdateOvertimeAttendanceUI(int index)
        {
            overtimeAttendanceGV.ClearSelection();

            var cells = dataGV.Rows[index].Cells;
            string employeeCode = Convert.ToString(cells["EmployeeCode"].Value);

            DataView dv = new DataView(mOvertimeAttendance_dt);
            dv.RowFilter = $"EmployeeCode = '{employeeCode}'";

            overtimeAttendanceGV.DataSource = dv;
        }

        private void UpdateLeaveAttendanceUI(int index)
        {
            leaveGV.ClearSelection();

            var cells = dataGV.Rows[index].Cells;
            string employeeCode = Convert.ToString(cells["EmployeeCode"].Value);

            DataView dv = new DataView(mLeaveAttendance_dt);
            dv.RowFilter = $"EmployeeCode = '{employeeCode}'";

            leaveGV.DataSource = dv;
        }

        private void UpdateDeductionUI(int index)
        {
            deductionGV.ClearSelection();

            var cells = dataGV.Rows[index].Cells;
            string employeeCode = Convert.ToString(cells["EmployeeCode"].Value);

            DataView dv = new DataView(mDeduction_dt);
            dv.RowFilter = $"EmployeeCode = '{employeeCode}'";

            deductionGV.DataSource = dv;
        }

        private async void saveBtn_Click(object sender, EventArgs e)
        {
            int month = Convert.ToInt32(monthYearDtp.Value.Month);
            int year = Convert.ToInt32(monthYearDtp.Value.Year);

            bool isLock = await SQLStore_QLNS.Instance.IsSalaryLockAsync(month, year);
            if (isLock)
            {
                MessageBox.Show("Tháng " + month + "/" + year + " đã bị khóa.", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (mCurentMonth != month || mCurentYear != year)
            {
                MessageBox.Show("Tháng, Năm Có gì Đó Sai Sai", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            List<(string EmployeeCode, string scopeCode, int allowanceTypeID, string AllowanceName, bool IsInsuranceIncluded, int Amount, int Month, int Year)> allowanceList = new List<(string, string, int, string, bool, int, int, int)>();

            foreach (DataRow row in mAllowance_dt.Rows)
            {
                int allowanceTypeID = Convert.ToInt32(row["AllowanceTypeID"]);
                string scopeCode = Convert.ToString(row["ScopeCode"]);
                string employeeCode = Convert.ToString(row["EmployeeCode"]);
                string allowanceName = Convert.ToString(row["AllowanceName"]);                
                bool isInsuranceIncluded = row["IsInsuranceIncluded"] != DBNull.Value && Convert.ToBoolean(row["IsInsuranceIncluded"]);
                int amount = row["Amount"] != DBNull.Value ? Convert.ToInt32(row["Amount"]) : 0;

                allowanceList.Add((employeeCode, scopeCode, allowanceTypeID, allowanceName, isInsuranceIncluded, amount, month, year ));
            }

            List<(string EmployeeCode, string ContractTypeName, int Month, int Year, decimal BaseSalary, decimal NetSalary, decimal NetInsuranceSalary,
            decimal InsuranceAllowance, decimal NonInsuranceAllowance, decimal OvertimeSalary, decimal LeaveSalary, decimal DeductionAmount, bool IsInsuranceRefund) > esbsData = new List<(string, string, int, int, decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal, bool)>();

            foreach (DataRow row in mEmployee_dt.Rows)
            {
                string employeeCode = Convert.ToString(row["EmployeeCode"]);
                string contractTypeName = Convert.ToString(row["ContractTypeName"]);
                bool isInsuranceRefund = Convert.ToBoolean(row["IsInsuranceRefund"]);
                decimal netSalary = Convert.ToDecimal(row["NetSalary"]);
                decimal baseSalary = Convert.ToDecimal(row["BaseSalary"]);
                decimal netInsuranceSalary = Convert.ToDecimal(row["TotalInsuranceSalary"]);
                decimal insuranceAllowance = Convert.ToDecimal(row["TotalIncludedInsurance"]);
                decimal nonInsuranceAllowance = Convert.ToDecimal(row["TotalExcludedInsurance"]);
                decimal overtimeMoney = 0;
                decimal leaveMoney = 0;
                decimal deductionMoney = 0;
                foreach (DataRow otdr in mOvertimeType_dt.Rows)
                {
                    int overtimeTypeID = Convert.ToInt32(otdr["OvertimeTypeID"]);
                    overtimeMoney += Convert.ToDecimal(row["OvertimeType" + overtimeTypeID.ToString()]);
                }
                foreach (DataRow ltdr in mLeaveType_dt.Rows)
                {
                    string leaveTypeCode = Convert.ToString(ltdr["LeaveTypeCode"]);
                    leaveMoney += Convert.ToDecimal(row["LeaveType" + leaveTypeCode]);
                }

                foreach (DataRow ltdr in mDeductionType.Rows)
                {
                    string deductionTypeCode = Convert.ToString(ltdr["DeductionTypeCode"]);
                    deductionMoney += Convert.ToDecimal(row["DeductionType" + deductionTypeCode]);
                }

                esbsData.Add((employeeCode, contractTypeName, month, year, baseSalary, netSalary, netInsuranceSalary, insuranceAllowance, nonInsuranceAllowance, overtimeMoney, leaveMoney, deductionMoney, isInsuranceRefund));
            }

            DialogResult dialogResult = MessageBox.Show($"Sau khi thực hiện thao tác này \n dữ liệu sẽ bị khóa vĩnh viễn ?", "Cảnh Báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {

                try
                {
                    // Chạy song song 2 task
                    var task1 = SQLManager_QLNS.Instance.UpsertSaveEmployeeAllowanceHistoryBatchAsync(allowanceList);
                    var task2 = SQLManager_QLNS.Instance.UpsertEmployeeSalaryHistoryAsync(esbsData);

                    // Chờ cả 2 task hoàn tất
                    var results = await Task.WhenAll(task1, task2);

                    bool success1 = results[0];
                    bool success2 = results[1];
                    bool success3 = false;

                    if (success1 && success2)
                    {
                        // Chỉ khóa lương khi 2 bước trước thành công
                        success3 = await SQLManager.Instance.UpsertLockSalaryAsync(month, year, true);
                    }

                    if (success1 && success2 && success3)
                    {
                        SQLStore_QLNS.Instance.ResetlockSalary();
                        MessageBox.Show("✅ Thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("❌ Thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi lưu dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Print_pl_btn_Click(object sender, EventArgs e)
        {
            int month = Convert.ToInt32(monthYearDtp.Value.Month);
            int year = Convert.ToInt32(monthYearDtp.Value.Year);

            // Lấy EmployeeCode từ cột DataGridView
            string employeeCode = dataGV.CurrentRow.Cells["EmployeeCode"].Value.ToString();

            List<string> employeeCodesToPrint = new List<string>();
            employeeCodesToPrint.Add(employeeCode);
            // Khởi tạo printer với logo từ resources
            SalarySlipPrinter printer = new SalarySlipPrinter(dataGV, mAllowance_dt, mLeaveType_dt, mOvertimeType_dt, mDeductionType, employeeCodesToPrint, month, year, Properties.Resources.logo_vr_1);

            // Hiển thị Print Preview
            printer.PrintDirect();
        }

        private void PrintPreview_pl_btn_Click(object sender, EventArgs e)
        {
            int month = Convert.ToInt32(monthYearDtp.Value.Month);
            int year = Convert.ToInt32(monthYearDtp.Value.Year);

            // Lấy EmployeeCode từ cột DataGridView
            string employeeCode = dataGV.CurrentRow.Cells["EmployeeCode"].Value.ToString();

            List<string> employeeCodesToPrint = new List<string>();
            employeeCodesToPrint.Add(employeeCode);
            // Khởi tạo printer với logo từ resources
            SalarySlipPrinter printer = new SalarySlipPrinter(dataGV, mAllowance_dt, mLeaveType_dt, mOvertimeType_dt, mDeductionType, employeeCodesToPrint, month, year, Properties.Resources.logo_vr_1);

            // Hiển thị Print Preview
            printer.PrintPreview();
        }

        private void ExportPDF_btn_Click(object sender, EventArgs e)
        {
            if (dataGV.CurrentRow == null) return;

            int month = Convert.ToInt32(monthYearDtp.Value.Month);
            int year = Convert.ToInt32(monthYearDtp.Value.Year);

            List<string> employeeCodesToPrint = new List<string>();
            if (mEmployee_dt != null && mEmployee_dt.Rows.Count > 0)
            {
                employeeCodesToPrint = mEmployee_dt
                    .AsEnumerable()
                    .Select(row => row.Field<string>("EmployeeCode"))
                    .Distinct() // nếu muốn loại bỏ trùng lặp
                    .ToList();
            }
            // Khởi tạo printer với logo từ resources
            SalarySlipPrinter printer = new SalarySlipPrinter(dataGV, mAllowance_dt, mLeaveType_dt, mOvertimeType_dt, mDeductionType, employeeCodesToPrint, month, year, Properties.Resources.logo_vr_1);

            // Hiển thị Print Preview
            printer.ExportAllToPdf();
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
            LuuThayDoiBtn.Visible = false;
            ShowData();
        }


        private void Preview_BCC_btn_Click(object sender, EventArgs e)
        {
            Print_BangChamCong(true);
        }

        private void Print_BCC_btn_Click(object sender, EventArgs e)
        {
            Print_BangChamCong(false);
        }
        private void Print_BangChamCong(bool isPreview)
        {
            if(dataGV.CurrentRow == null)
            {
                MessageBox.Show("Chưa chọn phòng Ban cần in", "thông báo", MessageBoxButtons.OK);
                return;
            }

            DateTime date = monthYearDtp.Value;
            BangChamCong_Printer deliveryPrinter = new BangChamCong_Printer(dataGV.CurrentRow.Cells["DepartmentName"].Value.ToString(), mEmployee_dt, mAttendamce_dt, mLeaveAttendance_dt, date.Month, date.Year);

            if (!isPreview)
                deliveryPrinter.PrintDirect();
            else
                deliveryPrinter.PrintPreview(this);
        }

        private void PrintPreview_BTC_btn_Click(object sender, EventArgs e)
        {
            Print_BangTăngCa(true);
        }

        private void Print_BTC_btn_Click(object sender, EventArgs e)
        {
            Print_BangTăngCa(false);
        }

        private void Print_BangTăngCa(bool isPreview)
        {
            if (dataGV.CurrentRow == null)
            {
                MessageBox.Show("Chưa chọn phòng Ban cần in", "thông báo", MessageBoxButtons.OK);
                return;
            }

            DateTime date = monthYearDtp.Value;
            BangChamTăngCa_Printer printer = new BangChamTăngCa_Printer(dataGV.CurrentRow.Cells["DepartmentName"].Value.ToString(), mEmployee_dt, mOvertimeAttendance_dt, date.Month, date.Year);

            if (!isPreview)
                printer.PrintDirect();
            else
                printer.PrintPreview(this);
        }

        private async void ChiHoLuong_btn_Click(object sender, EventArgs e)
        {
            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Message = "Đang xử lý ...";
            loadingOverlay.Show();
            await Task.Delay(100);

            DateTime salaryMonth = monthYearDtp.Value;
            try
            {
                using (var wb = new XLWorkbook())
                {
                    var ws_ACB = wb.Worksheets.Add("ACB");
                    ws_ACB.Style.Font.FontName = "Times New Roman";
                    ws_ACB.Style.Font.FontSize = 12;
                    ws_ACB.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    NganHang_ACB(ws_ACB, salaryMonth.Month, salaryMonth.Year);

                    var ws_KHAC = wb.Worksheets.Add("Ngoài ACB");
                    ws_KHAC.Style.Font.FontName = "Times New Roman";
                    ws_KHAC.Style.Font.FontSize = 12;
                    ws_KHAC.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    NganHang_KHAC(ws_KHAC, salaryMonth.Month, salaryMonth.Year);

                    //ws.Column(8).Width = 27;
                    // ===== Save file =====
                    using (SaveFileDialog sfd = new SaveFileDialog())
                    {
                        sfd.Filter = "Excel Workbook|*.xlsx";
                        sfd.FileName = $"Danh Sach Chi Ho Luong T{salaryMonth.Month.ToString("D2")}-{salaryMonth.Year}.xlsx";
                        if (sfd.ShowDialog() == DialogResult.OK)
                        {
                            wb.SaveAs(sfd.FileName);
                            DialogResult result = MessageBox.Show("Bạn có muốn mở file này không?", "Lưu file thành công", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (result == DialogResult.Yes)
                            {
                                try
                                {
                                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                                    {
                                        FileName = sfd.FileName,
                                        UseShellExecute = true
                                    });
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("Không thể mở file: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xuất Excel: " + ex.Message);
            }
            finally
            {
                await Task.Delay(200);
                loadingOverlay.Hide();
            }
        }

        void NganHang_ACB(IXLWorksheet ws, int month, int year)
        {
            // Danh sách cột muốn xuất theo Name

            int numColumn = 6;
            int rowInd = 1;

            ws.Range(rowInd, 1, rowInd, numColumn).Merge();
            ws.Cell(rowInd, 1).Value = "DANH SÁCH CHI HỘ LƯƠNG";
            ws.Cell(rowInd, 1).Style.Font.Bold = true;
            ws.Cell(rowInd, 1).Style.Font.FontSize = 24;
            ws.Cell(rowInd, 1).Style.Font.FontColor = XLColor.OrangeRed;
            ws.Cell(rowInd, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            rowInd++;
            ws.Range(rowInd, 1, rowInd, numColumn).Merge();
            ws.Cell(rowInd, 1).Value = $"KỲ LƯƠNG THÁNG {month}/{year}";
            ws.Cell(rowInd, 1).Style.Font.Bold = true;
            ws.Cell(rowInd, 1).Style.Font.FontSize = 16;
            ws.Cell(rowInd, 1).Style.Font.FontColor = XLColor.AppleGreen;
            ws.Cell(rowInd, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            rowInd++;
            // ===== Header cấp 2 và cấp 3 =====
            string[] columnsName = new string[] { "STT", "Mã NV", "Họ Và Tên", "Số TK", "Số Tiền", "Ghi Chú"};
            int colIndex = 1;
            foreach (var col in columnsName)
            {
                var cell = ws.Cell(rowInd, colIndex);
                cell.Value = col;
                cell.Style.Font.Bold = true;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Đóng khung
                colIndex++;

            }

            rowInd++;
            string[] exportColumns = new string[] { "STT", "EmployeeCode", "FullName", "BankAccountNumber", "NetSalary", "Note" };
            int totalMoney = 0;
            int countSTT = 1;
            foreach (DataRow row in mEmployee_dt.Rows)
            {
                string bankName = row["BankName"].ToString();
                if (bankName.Trim().ToLower().CompareTo("acb") != 0)
                    continue;
                colIndex = 1;
                foreach (var colName in exportColumns)
                {
                    var cell = ws.Cell(rowInd, colIndex);
                    string cellValue = countSTT.ToString();

                    if (colName.CompareTo("STT") != 0 && colName.CompareTo("Note") != 0)
                        cellValue = row[colName]?.ToString() ?? "";

                    
                    // Căn phải các cột số
                    
                    if (colName.CompareTo("NetSalary") == 0)
                    {
                        decimal value = 0;
                        decimal.TryParse(cellValue?.ToString(), out value);
                        int result = Convert.ToInt32(value >= 1000 ? Math.Floor(value / 1000) * 1000 : value);
                        cell.Value = result;
                        totalMoney += result;

                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;                        
                        cell.Style.NumberFormat.Format = "#,##0";                        
                    }
                    else
                    {
                        if (colName.CompareTo("Note") != 0)                            
                            cell.Value = cellValue;

                        if (colName.CompareTo("FullName") == 0)
                            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        else
                            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                       
                    }                    

                    // Thêm border
                    cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    colIndex++;

                }
                countSTT++;
                rowInd++;
            }

            //int nwOtherColIndex = exportColumns.FindIndex(c => c.Name == "NetWeightFinal") + 1 + 3;

            ws.Range(rowInd, 1, rowInd, 4).Merge();
            ws.Cell(rowInd, 1).Value = "TỔNG CỘNG";
            ws.Cell(rowInd, 1).Style.Font.Bold = true;
            ws.Cell(rowInd, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Range(rowInd, 1, rowInd, 4).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            ws.Cell(rowInd, 5).Value = totalMoney;
            ws.Cell(rowInd, 5).Style.NumberFormat.Format = "#,##0";
            ws.Cell(rowInd, 5).Style.Font.Bold = true;
            ws.Cell(rowInd, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell(rowInd, 5).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Cell(rowInd, 6).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            ws.Row(1).Height = 34;
            ws.Row(2).Height = 23;
            ws.Column(1).Width = 4.3;
            ws.Column(2).Width = 8;
            ws.Column(3).Width = 30;
            ws.Column(4).Width = 15;
            ws.Column(5).Width = 15;
            ws.Column(6).Width = 40;
            //ws.Column(7).Width = 21;
            //ws.Column(8).Width = 11.2;

            int ColIndex = 1;
            foreach (var col in exportColumns)
            {
                var column = ws.Column(ColIndex);
                column.Style.Alignment.WrapText = true;
                ColIndex++;
            }
        }

        void NganHang_KHAC(IXLWorksheet ws, int month, int year)
        {
            // Danh sách cột muốn xuất theo Name

            int numColumn = 6;
            int rowInd = 1;

            ws.Range(rowInd, 1, rowInd, numColumn).Merge();
            ws.Cell(rowInd, 1).Value = "DANH SÁCH CHI HỘ LƯƠNG";
            ws.Cell(rowInd, 1).Style.Font.Bold = true;
            ws.Cell(rowInd, 1).Style.Font.FontSize = 24;
            ws.Cell(rowInd, 1).Style.Font.FontColor = XLColor.OrangeRed;
            ws.Cell(rowInd, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            rowInd++;
            ws.Range(rowInd, 1, rowInd, numColumn).Merge();
            ws.Cell(rowInd, 1).Value = $"KỲ LƯƠNG THÁNG {month}/{year}";
            ws.Cell(rowInd, 1).Style.Font.Bold = true;
            ws.Cell(rowInd, 1).Style.Font.FontSize = 16;
            ws.Cell(rowInd, 1).Style.Font.FontColor = XLColor.AppleGreen;
            ws.Cell(rowInd, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            rowInd++;
            // ===== Header cấp 2 và cấp 3 =====
            string[] columnsName = new string[] { "STT", "Mã NV", "Họ Và Tên", "Số TK", "Số Tiền", "Ghi Chú" };
            int colIndex = 1;
            foreach (var col in columnsName)
            {
                var cell = ws.Cell(rowInd, colIndex);
                cell.Value = col;
                cell.Style.Font.Bold = true;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Đóng khung
                colIndex++;

            }

            rowInd++;
            string[] exportColumns = new string[] { "STT", "EmployeeCode", "FullName", "BankAccountNumber", "NetSalary", "Note" };
            int totalMoney = 0;
            int countSTT = 1;
            foreach (DataRow row in mEmployee_dt.Rows)
            {
                string bankName = row["BankName"].ToString();
                if (bankName.Trim().ToLower().CompareTo("acb") == 0)
                    continue;
                colIndex = 1;
                foreach (var colName in exportColumns)
                {
                    var cell = ws.Cell(rowInd, colIndex);
                    string cellValue = countSTT.ToString();

                    if (colName.CompareTo("STT") != 0 && colName.CompareTo("Note") != 0)
                        cellValue = row[colName]?.ToString() ?? "";


                    // Căn phải các cột số

                    if (colName.CompareTo("NetSalary") == 0)
                    {
                        decimal value = 0;
                        decimal.TryParse(cellValue?.ToString(), out value);
                        int result = Convert.ToInt32(value >= 1000 ? Math.Floor(value / 1000) * 1000 : value);
                        cell.Value = result;
                        totalMoney += result;

                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        cell.Style.NumberFormat.Format = "#,##0";
                    }
                    else
                    {
                        if (colName.CompareTo("Note") != 0)
                            cell.Value = cellValue;
                        else
                        {
                            cell.Value = $" STK: {row["BankAccountNumber"]},\n Tên TK: {row["BankAccountHolder"]},\n NH: {row["BankName"]}";
                        }

                        if (colName.CompareTo("FullName") == 0 || colName.CompareTo("Note") == 0)
                            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        else
                            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    }

                    // Thêm border
                    cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    colIndex++;

                }
                countSTT++;
                rowInd++;
            }

            //int nwOtherColIndex = exportColumns.FindIndex(c => c.Name == "NetWeightFinal") + 1 + 3;

            ws.Range(rowInd, 1, rowInd, 4).Merge();
            ws.Cell(rowInd, 1).Value = "TỔNG CỘNG";
            ws.Cell(rowInd, 1).Style.Font.Bold = true;
            ws.Cell(rowInd, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Range(rowInd, 1, rowInd, 4).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            ws.Cell(rowInd, 5).Value = totalMoney;
            ws.Cell(rowInd, 5).Style.NumberFormat.Format = "#,##0";
            ws.Cell(rowInd, 5).Style.Font.Bold = true;
            ws.Cell(rowInd, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell(rowInd, 5).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Cell(rowInd, 6).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            ws.Row(1).Height = 34;
            ws.Row(2).Height = 23;
            ws.Column(1).Width = 4.3;
            ws.Column(2).Width = 8;
            ws.Column(3).Width = 30;
            ws.Column(4).Width = 15;
            ws.Column(5).Width = 15;
            ws.Column(6).Width = 40;
            //ws.Column(7).Width = 21;
            //ws.Column(8).Width = 11.2;

            int ColIndex = 1;
            foreach (var col in exportColumns)
            {
                var column = ws.Column(ColIndex);
                column.Style.Alignment.WrapText = true;
                ColIndex++;
            }
        }
    }
}
