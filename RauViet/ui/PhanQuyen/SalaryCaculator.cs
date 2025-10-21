using DocumentFormat.OpenXml.VariantTypes;
using DocumentFormat.OpenXml.Vml;
using DocumentFormat.OpenXml.Wordprocessing;
using RauViet.classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using Color = System.Drawing.Color;

namespace RauViet.ui
{    
    public partial class SalaryCaculator : Form
    {
        private DataTable mEmployee_dt, mAllowance_dt, mOvertimeAttendance_dt, mLeaveAttendance_dt, 
            mDeduction_dt, mLeaveType_dt, mDeductionType, mAttendamce_dt, mOvertimeType_dt;
        private const string DeductionTypeCode = "ADV";

        private int mCurentMonth = -1;
        private int mCurentYear = -1;
        public SalaryCaculator()
        {
            InitializeComponent();

            print_pl_btn.Visible = false;
            printPreview_pl_btn.Visible = false;
            LuuThayDoiBtn.Visible = false;
            exportPDF_btn.Visible = false;

            month_cbb.Items.Clear();
            for (int m = 1; m <= 12; m++)
            {
                month_cbb.Items.Add(m);
            }
            dataGV.EnableHeadersVisualStyles = false;
            allowancePanel.Height = this.ClientSize.Height * 2/5;
            leave_panel.Height = this.ClientSize.Height / 4;
            deductionPanel.Height = this.ClientSize.Height / 3;
            month_cbb.SelectedItem = DateTime.Now.Month;
            year_tb.Text = DateTime.Now.Year.ToString();

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            allowanceGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            allowanceGV.MultiSelect = false;


            dataGV.SelectionChanged += this.dataGV_CellClick;
            year_tb.KeyPress += Tb_KeyPress_OnlyNumber;

            load_btn.Click += Load_btn_Click;
            LuuThayDoiBtn.Click += saveBtn_Click;
            print_pl_btn.Click += Print_pl_btn_Click;
            printPreview_pl_btn.Click += PrintPreview_pl_btn_Click; ;
            exportPDF_btn.Click += ExportPDF_btn_Click;
            this.Resize += SalaryCaculatorForm_Resize;
        }

        

        private void SalaryCaculatorForm_Resize(object sender, EventArgs e)
        {
            allowancePanel.Height = this.ClientSize.Height * 2 / 5;
            leave_panel.Height = this.ClientSize.Height / 4;
            deductionPanel.Height = this.ClientSize.Height / 3;
        }

        public async void ShowData()
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;        

            try
            {
                int month = Convert.ToInt32(month_cbb.SelectedItem);
                int year = Convert.ToInt32(year_tb.Text);
                var employeeTask = SQLManager.Instance.GetEmployeeSalarySummaryAsync(month, year);
                var employeeAllowanceAsync = SQLManager.Instance.GetEmployeeAllowanceAsync(month, year);
                var overtimeAttendamceAsync = SQLManager.Instance.GetOvertimeAttendamceForSalaryAsync(month, year);
                var leaveAttendanceAsync = SQLManager.Instance.GetLeaveAttendanceAsync_IsPaid(month, year);
                var deductionAsync = SQLManager.Instance.GetEmployeeDeductions(month, year);
                var overtimeTypeAsync = SQLStore.Instance.GetOvertimeTypeAsync();
                var leaveTypeAsync = SQLStore.Instance.GetLeaveType_HavePaidAsync();
                var deductionTypeAsync = SQLStore.Instance.GetDeductionTypeAsync();
                var attendamceTask = SQLManager.Instance.GetAttendamceForSalaryAsync(month, year);

                await Task.WhenAll(employeeTask, employeeAllowanceAsync, overtimeAttendamceAsync, leaveAttendanceAsync,
                    deductionAsync, overtimeTypeAsync, leaveTypeAsync, deductionTypeAsync, attendamceTask);

                mCurentMonth = month;
                mCurentYear = year;

                mOvertimeType_dt = overtimeTypeAsync.Result;
                mEmployee_dt = employeeTask.Result;
                mAllowance_dt = employeeAllowanceAsync.Result;
                mOvertimeAttendance_dt = overtimeAttendamceAsync.Result;
                mLeaveAttendance_dt = leaveAttendanceAsync.Result;
                mDeduction_dt = deductionAsync.Result;
                mLeaveType_dt = leaveTypeAsync.Result;
                mDeductionType = deductionTypeAsync.Result;
                mAttendamce_dt = attendamceTask.Result;
                // ====== 1️⃣ Tính phụ cấp có/không bảo hiểm ======
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
                                          TotalHourWork = g
                                              .Sum(r => r["WorkingHours"] == DBNull.Value ? 0 : Convert.ToInt32(r["WorkingHours"])),
                                      };

                // ====== 2️⃣ Thêm cột cần tính ======
                void AddColumnIfNotExists(DataTable dt, string name, Type type)
                {
                    if (!dt.Columns.Contains(name))
                        dt.Columns.Add(new DataColumn(name, type));
                }

                AddColumnIfNotExists(mEmployee_dt, "TotalIncludedInsurance", typeof(int));
                AddColumnIfNotExists(mEmployee_dt, "TotalExcludedInsurance", typeof(int));
                AddColumnIfNotExists(mEmployee_dt, "TotalInsuranceSalary", typeof(int));
                AddColumnIfNotExists(mEmployee_dt, "EmployeeInsurancePaid", typeof(int));
                AddColumnIfNotExists(mEmployee_dt, "HourSalary", typeof(decimal));
                AddColumnIfNotExists(mEmployee_dt, "TotalHourWork", typeof(decimal));
                AddColumnIfNotExists(mEmployee_dt, "TotalSalaryHourWork", typeof(decimal));
                AddColumnIfNotExists(mEmployee_dt, "InsuranceRefund", typeof(decimal));
                AddColumnIfNotExists(mEmployee_dt, "NetSalary", typeof(decimal));

                foreach (DataRow adr in mAllowance_dt.Rows)
                {
                    string scopeCode = adr["ScopeCode"].ToString();
                    if(scopeCode.CompareTo("ONCE") == 0)
                        AddColumnIfNotExists(mEmployee_dt, "Allowance" + adr["AllowanceTypeID"].ToString(), typeof(decimal));
                }

                foreach (DataRow otdr in mOvertimeType_dt.Rows)
                {
                    AddColumnIfNotExists(mEmployee_dt, "OvertimeType" + otdr["OvertimeTypeID"].ToString(), typeof(decimal));
                    AddColumnIfNotExists(mEmployee_dt, "c_OvertimeType" + otdr["OvertimeTypeID"].ToString(), typeof(decimal));
                }

                foreach (DataRow otdr in mLeaveType_dt.Rows)
                {
                    AddColumnIfNotExists(mEmployee_dt, "LeaveType" + otdr["LeaveTypeCode"].ToString(), typeof(decimal));
                    AddColumnIfNotExists(mEmployee_dt, "c_LeaveType" + otdr["LeaveTypeCode"].ToString(), typeof(decimal));
                }

                foreach (DataRow dtdr in mDeductionType.Rows)
                {
                    AddColumnIfNotExists(mEmployee_dt, "DeductionType" + dtdr["DeductionTypeCode"].ToString(), typeof(decimal));
                }

                mAttendamce_dt.Columns.Add("DayOfWeek", typeof(string));

                foreach (DataRow row in mAttendamce_dt.Rows)
                {
                    DateTime dt = Convert.ToDateTime(row["WorkDate"]);
                    string[] vietDays = { "CN", "T.2", "T.3", "T.4", "T.5", "T.6", "T.7" };
                    row["DayOfWeek"] = vietDays[(int)dt.DayOfWeek];
                }

                int countAttendamce = 0;
                mAttendamce_dt.Columns["DayOfWeek"].SetOrdinal(countAttendamce++);
                mAttendamce_dt.Columns["WorkDate"].SetOrdinal(countAttendamce++);
                mAttendamce_dt.Columns["WorkingHours"].SetOrdinal(countAttendamce++);

                // ====== 3️⃣ Tính toán cho từng nhân viên ======
                foreach (DataRow dr in mEmployee_dt.Rows)
                {
                    string employeeCode = Convert.ToString(dr["EmployeeCode"]);
                    bool isInsuranceRefund = Convert.ToBoolean(dr["IsInsuranceRefund"]);

                    int insuranceBaseSalary = dr["InsuranceBaseSalary"] == DBNull.Value ? 0 : Convert.ToInt32(dr["InsuranceBaseSalary"]);
                    int baseSalary = dr["BaseSalary"] == DBNull.Value ? 0 : Convert.ToInt32(dr["BaseSalary"]);

                    var match = insuranceResult.FirstOrDefault(r => r.EmployeeCode == employeeCode);                    
                    int totalIncludedInsurance = match?.TotalIncludedInsurance ?? 0;
                    int totalExcludedInsurance = match?.TotalExcludedInsurance ?? 0;
                    // Tính tiền BHXH nhân viên phải nộp (10.5%)
                    int employeeInsurancePaid = Convert.ToInt32((totalIncludedInsurance + insuranceBaseSalary) * 0.105m);

                    // Tính lương ngày và giờ
                    double daySalary = ((baseSalary - employeeInsurancePaid) + totalIncludedInsurance + totalExcludedInsurance) / 26.0;
                    decimal hourSalary = Math.Round((decimal)(daySalary / 8.0), 1);

                    dr["TotalIncludedInsurance"] = totalIncludedInsurance;
                    dr["TotalExcludedInsurance"] = totalExcludedInsurance;
                    dr["TotalInsuranceSalary"] = totalIncludedInsurance + insuranceBaseSalary;
                    dr["EmployeeInsurancePaid"] = employeeInsurancePaid;
                    dr["HourSalary"] = hourSalary;
                    dr["InsuranceRefund"] = isInsuranceRefund ? employeeInsurancePaid : 0;

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
                    decimal totalSalaryHourWork = 0;;
                    decimal insuranceRefund = Convert.ToDecimal(dr["InsuranceRefund"]);
                    decimal overtimeMoney = 0;
                    decimal leaveMoney = 0;
                    decimal allowanceONCE = 0;

                    //các khoảng trừ
                    decimal deductionMoney = 0;
                    decimal employeeInsurancePaid = Convert.ToDecimal(dr["EmployeeInsurancePaid"]);

                    var hwMatch = attendamceResult.FirstOrDefault(r => r.EmployeeCode == employeeCode);
                    int totalHourWork = hwMatch?.TotalHourWork ?? 0;
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

                        decimal total = filteredRows.Any()? filteredRows.Sum(r => r.Field<decimal>("OvertimeAttendanceSalary")): 0m;
                        decimal totalhour = filteredRows.Any() ? filteredRows.Sum(r => r.Field<decimal>("HourWork")) : 0m;

                        dr["c_OvertimeType" + overtimeTypeID.ToString()] = totalhour;
                        dr["OvertimeType" + overtimeTypeID.ToString()] = total;
                        overtimeMoney += total;
                    }

                    foreach (DataRow ltdr in mLeaveType_dt.Rows)
                    {
                        string leaveTypeCode = Convert.ToString(ltdr["LeaveTypeCode"]);
                        var count = mLeaveAttendance_dt.AsEnumerable().Count(r => r.Field<string>("EmployeeCode") == employeeCode && r.Field<string>("LeaveTypeCode") == leaveTypeCode);

                        decimal temp = count * 8 * hourSalary;
                        dr["LeaveType" + leaveTypeCode] = temp;
                        dr["c_LeaveType" + leaveTypeCode] = count * 8;
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

                int countE = 0;
                mEmployee_dt.Columns["EmployeeCode"].SetOrdinal(countE++);
                mEmployee_dt.Columns["FullName"].SetOrdinal(countE++);
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

                overtimeAttendanceGV.DataSource = mOvertimeAttendance_dt;
                allowanceGV.DataSource = mAllowance_dt;
                leaveGV.DataSource = mLeaveAttendance_dt;
                deductionGV.DataSource = mDeduction_dt;
                attendamceGV.DataSource = mAttendamce_dt;
                dataGV.DataSource = mEmployee_dt;
                

                attendamceGV.Columns["EmployeeCode"].Visible = false;
                allowanceGV.Columns["EmployeeCode"].Visible = false;
                allowanceGV.Columns["AllowanceTypeID"].Visible = false;
                allowanceGV.Columns["ScopeCode"].Visible = false;
                overtimeAttendanceGV.Columns["EmployeeCode"].Visible = false;
                overtimeAttendanceGV.Columns["SalaryFactor"].Visible = false;
                overtimeAttendanceGV.Columns["OvertimeTypeID"].Visible = false;
                leaveGV.Columns["EmployeeCode"].Visible = false;
                leaveGV.Columns["LeaveTypeCode"].Visible = false;
                deductionGV.Columns["EmployeeCode"].Visible = false;
                deductionGV.Columns["DeductionTypeCode"].Visible = false;
                dataGV.Columns["IsInsuranceRefund"].Visible = false;
                dataGV.Columns["HireDate"].Visible = false;
                dataGV.Columns["RemainingLeave"].Visible = false;

                allowanceGV.Columns["AllowanceName"].HeaderText = "Phụ Cấp";
                allowanceGV.Columns["Amount"].HeaderText = "Số Tiền";
                allowanceGV.Columns["IsInsuranceIncluded"].HeaderText = "BHXH";
                allowanceGV.Columns["AllowanceName"].Width = 120;
                allowanceGV.Columns["Amount"].Width = 70;
                allowanceGV.Columns["IsInsuranceIncluded"].Width = 40;

                deductionGV.Columns["DeductionDate"].HeaderText = "Ngày Trừ";
                deductionGV.Columns["DeductionTypeName"].HeaderText = "Loại Tiền";
                deductionGV.Columns["Amount"].HeaderText = "Số Tiền";
                deductionGV.Columns["DeductionDate"].Width = 70;
                deductionGV.Columns["DeductionTypeName"].Width = 80;
                deductionGV.Columns["Amount"].Width = 60;

                attendamceGV.Columns["DayOfWeek"].HeaderText = "Thứ";
                attendamceGV.Columns["WorkDate"].HeaderText = "Ngày Làm";
                attendamceGV.Columns["WorkingHours"].HeaderText = "S.Giờ";
                attendamceGV.Columns["DayOfWeek"].Width = 40;
                attendamceGV.Columns["WorkDate"].Width = 80;
                attendamceGV.Columns["WorkingHours"].Width = 50;

                overtimeAttendanceGV.Columns["WorkDate"].HeaderText = "Ngày T.Ca";
                overtimeAttendanceGV.Columns["OvertimeName"].HeaderText = "Loại T.Ca";
                overtimeAttendanceGV.Columns["HourWork"].HeaderText = "S.Giờ";
                overtimeAttendanceGV.Columns["OvertimeAttendanceSalary"].HeaderText = "T.Công";
                overtimeAttendanceGV.Columns["WorkDate"].Width = 70;
                overtimeAttendanceGV.Columns["OvertimeName"].Width = 80;
                overtimeAttendanceGV.Columns["HourWork"].Width = 40;
                overtimeAttendanceGV.Columns["OvertimeAttendanceSalary"].Width = 60;

                leaveGV.Columns["LeaveTypeName"].HeaderText = "Loại Ngày Nghỉ";
                leaveGV.Columns["DateOff"].HeaderText = "Ngày Nghỉ";
                leaveGV.Columns["DateOff"].Width = 80;
                leaveGV.Columns["LeaveTypeName"].Width = 150;

                dataGV.Columns["EmployeeCode"].HeaderText = "Mã NV";
                dataGV.Columns["FullName"].HeaderText = "Tên Nhân Viên";
                dataGV.Columns["ContractTypeName"].HeaderText = "Hợp Đồng";
                dataGV.Columns["NetSalary"].HeaderText = "Thực Lãnh";
                dataGV.Columns["BaseSalary"].HeaderText = "Lương CB";
                dataGV.Columns["InsuranceBaseSalary"].HeaderText = "Lương C.S Đóng BHXH";
                dataGV.Columns["TotalInsuranceSalary"].HeaderText = "Lương Tính Đóng BHXH";
                dataGV.Columns["TotalSalaryHourWork"].HeaderText = "T.Lương Theo Giờ";
                dataGV.Columns["TotalHourWork"].HeaderText = "Giờ Công Làm";
                dataGV.Columns["HourSalary"].HeaderText = "Tiền 1 Giờ Làm";
                dataGV.Columns["EmployeeInsurancePaid"].HeaderText = "Tiền NV Nộp BHXH";
                dataGV.Columns["InsuranceRefund"].HeaderText = "Hoàn Trả BHXH";

                dataGV.Columns["TotalIncludedInsurance"].HeaderText = "PC Đóng BHXH";
                dataGV.Columns["TotalExcludedInsurance"].HeaderText = "PC K.Đóng BHXH";

                dataGV.Columns["EmployeeCode"].Frozen = true;
                dataGV.Columns["FullName"].Frozen = true;
                dataGV.Columns["NetSalary"].Frozen = true;

                dataGV.Columns["EmployeeCode"].Width = 50;
                dataGV.Columns["FullName"].Width = 120;
                dataGV.Columns["NetSalary"].Width = 70;
                dataGV.Columns["BaseSalary"].Width = 70;                
                dataGV.Columns["TotalSalaryHourWork"].Width = 70;
                dataGV.Columns["TotalHourWork"].Width = 50;
                dataGV.Columns["HourSalary"].Width = 50;
                dataGV.Columns["InsuranceBaseSalary"].Width = 70;
                dataGV.Columns["TotalInsuranceSalary"].Width = 70;
                dataGV.Columns["EmployeeInsurancePaid"].Width = 50;
                dataGV.Columns["InsuranceRefund"].Width = 50;
                dataGV.Columns["TotalIncludedInsurance"].Width = 50;
                dataGV.Columns["TotalExcludedInsurance"].Width = 50;

                dataGV.Columns["NetSalary"].HeaderCell.Style.BackColor = Color.BurlyWood;
                
                dataGV.Columns["InsuranceBaseSalary"].HeaderCell.Style.BackColor = Color.Coral;
                dataGV.Columns["TotalInsuranceSalary"].HeaderCell.Style.BackColor = Color.Coral;
                dataGV.Columns["EmployeeInsurancePaid"].HeaderCell.Style.BackColor = Color.Coral;
                dataGV.Columns["InsuranceRefund"].HeaderCell.Style.BackColor = Color.Coral;
                dataGV.Columns["TotalIncludedInsurance"].HeaderCell.Style.BackColor = Color.Coral;
                dataGV.Columns["TotalExcludedInsurance"].HeaderCell.Style.BackColor = Color.Coral;

                foreach (DataRow adr in mAllowance_dt.Rows)
                {
                    string scopeCode = adr["ScopeCode"].ToString();
                    if (scopeCode.CompareTo("ONCE") == 0)
                    {
                        string AllowanceCol = "Allowance" + adr["AllowanceTypeID"].ToString();
                        dataGV.Columns[AllowanceCol].HeaderText = adr["AllowanceName"].ToString();
                        dataGV.Columns[AllowanceCol].Width = 50;
                        dataGV.Columns[AllowanceCol].HeaderCell.Style.BackColor = Color.Coral;
                    }
                }

                foreach (DataRow otdr in mOvertimeType_dt.Rows)
                {
                    string overtimeTypeCol = "OvertimeType" + otdr["OvertimeTypeID"].ToString();
                    dataGV.Columns["c_" + overtimeTypeCol].Visible = false;
                    dataGV.Columns[overtimeTypeCol].HeaderText = otdr["OvertimeName"].ToString();
                    dataGV.Columns[overtimeTypeCol].Width = 50;
                    dataGV.Columns[overtimeTypeCol].HeaderCell.Style.BackColor = Color.Yellow;
                }

                foreach (DataRow otdr in mLeaveType_dt.Rows)
                {
                    string leaveTypeCol = "LeaveType" + otdr["LeaveTypeCode"].ToString();
                    dataGV.Columns["c_" + leaveTypeCol].Visible = false;
                    dataGV.Columns[leaveTypeCol].HeaderText = otdr["LeaveTypeName"].ToString();
                    dataGV.Columns[leaveTypeCol].Width = 50;
                    dataGV.Columns[leaveTypeCol].HeaderCell.Style.BackColor = Color.Aqua;
                }

                foreach (DataRow ltdr in mDeductionType.Rows)
                {
                    string deductionTypeCol = "DeductionType" + ltdr["DeductionTypeCode"].ToString();
                    dataGV.Columns[deductionTypeCol].HeaderText = ltdr["DeductionTypeName"].ToString();
                    dataGV.Columns[deductionTypeCol].Width = 50;
                    dataGV.Columns[deductionTypeCol].HeaderCell.Style.BackColor = Color.Gray;
                }


                if (dataGV.Rows.Count > 0)
                {
                    dataGV.ClearSelection();
                    dataGV.Rows[0].Selected = true;
                 //   UpdateAllowancetUI(0);
                }

                bool isLock = await SQLStore.Instance.IsSalaryLockAsync(month, year);
                print_pl_btn.Visible = isLock;
                printPreview_pl_btn.Visible = isLock;
                exportPDF_btn.Visible = isLock;
                LuuThayDoiBtn.Visible = !isLock;

                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                allowanceGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            catch (Exception ex)
            {
                Console.WriteLine("error: " + ex.ToString());
            }

            
        }

        private void Load_btn_Click(object sender, EventArgs e)
        {
            print_pl_btn.Visible = false;
            printPreview_pl_btn.Visible = false;
            exportPDF_btn.Visible = false;
            LuuThayDoiBtn.Visible = false;
            ShowData();
            MessageBox.Show("✅ Xong!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void dataGV_CellClick(object sender, EventArgs e)
        {
            if (dataGV.CurrentRow == null) return;
            int rowIndex = dataGV.CurrentRow.Index;
            if (rowIndex < 0)
                return;


            UpdateAllowanceUI(rowIndex);
            UpdateOvertimeAttendanceUI(rowIndex);
            UpdateLeaveAttendanceUI(rowIndex);
            UpdateDeductionUI(rowIndex);
            UpdateAttendandeUI(rowIndex);
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
            int month = Convert.ToInt32(month_cbb.SelectedItem);
            int year = Convert.ToInt32(year_tb.Text);

            bool isLock = await SQLStore.Instance.IsSalaryLockAsync(month, year);
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
            decimal InsuranceAllowance, decimal NonInsuranceAllowance, decimal OvertimeSalary, decimal LeaveSalary, decimal DeductionAmount)> esbsData = new List<(string, string, int, int, decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal)>();

            foreach (DataRow row in mEmployee_dt.Rows)
            {
                string employeeCode = Convert.ToString(row["EmployeeCode"]);
                string contractTypeName = Convert.ToString(row["ContractTypeName"]);
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

                esbsData.Add((employeeCode, contractTypeName, month, year, baseSalary, netSalary, netInsuranceSalary, insuranceAllowance, nonInsuranceAllowance, overtimeMoney, leaveMoney, deductionMoney));
            }

            DialogResult dialogResult = MessageBox.Show($"Sau khi thực hiện thao tác này \n dữ liệu sẽ bị khóa vĩnh viễn ?", "Cảnh Báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {

                try
                {
                    // Chạy song song 2 task
                    var task1 = SQLManager.Instance.UpsertSaveEmployeeAllowanceHistoryBatchAsync(allowanceList);
                    var task2 = SQLManager.Instance.UpsertEmployeeSalaryHistoryAsync(esbsData);

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
            int month = Convert.ToInt32(month_cbb.SelectedItem);
            int year = Convert.ToInt32(year_tb.Text);

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
            int month = Convert.ToInt32(month_cbb.SelectedItem);
            int year = Convert.ToInt32(year_tb.Text);

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

            int month = Convert.ToInt32(month_cbb.SelectedItem);
            int year = Convert.ToInt32(year_tb.Text);

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
    }
}
