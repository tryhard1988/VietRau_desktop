using ClosedXML.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using RauViet.classes;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RauViet.ui
{    
    public partial class PhieuDuToanLuong : Form
    {
        private DataTable mEmployees_dt, mEmployeeAllowances_dt;
        private DataView mEmployeeAllowances_dv;
        public PhieuDuToanLuong()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            dataGV.SelectionChanged += this.dataGV_CellClick;

            this.KeyDown += Employee_POS_DEP_CON_KeyDown;
            this.FormClosing += Employee_POS_DEP_CON_FormClosing;
            search_tb.TextChanged += Search_tb_TextChanged;

            preview_btn.Click += Preview_btn_Click;
            Print_btn.Click += Print_btn_Click;
            excel_btn.Click += Excel_btn_Click;
        }

        private async void Employee_POS_DEP_CON_FormClosing(object sender, FormClosingEventArgs e)
        {
            SQLStore_QLNS.Instance.removeAnnualLeaveBalance();
            await Task.WhenAll(
                    SQLManager_QLNS.Instance.AutoUpsertAnnualLeaveMonthListAsync(),
                    SQLManager_QLNS.Instance.GetAnnualLeaveBalanceAsync()
                );
        }

        private void Employee_POS_DEP_CON_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                SQLStore_QLNS.Instance.removeEmployeeSalaryInfo();
                SQLStore_QLNS.Instance.removeEmployeeAllowances_All();
                ShowData();
            }
        }

        public async void ShowData()
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            await Task.Delay(50);
            LoadingOverlay loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Show();
            await Task.Delay(200);
            try
            {
                int month = DateTime.Now.Month;
                int year = DateTime.Now.Year;
                // Chạy truy vấn trên thread riêng
                string[] keepColumns = { "EmployeeCode", "FullName", "HireDate", "PositionID", "DepartmentID", "ContractTypeID", "PositionName", "DepartmentName", "ContractTypeName", "EmployessName_NoSign" };
                string[] keepColumnsInfo = { "BaseSalary", "InsuranceBaseSalary"};
                var employeesTask = SQLStore_QLNS.Instance.GetEmployeesAsync(keepColumns);
                var employeeAllowancesTask = SQLStore_QLNS.Instance.GetEmployeeAllowances_AllAsync();
                var employeeSalaryInfoTask = SQLStore_QLNS.Instance.GetEmployeeSalaryInfoAsync(keepColumnsInfo, month, year);

                await Task.WhenAll(employeesTask, employeeAllowancesTask, employeeSalaryInfoTask);
                mEmployees_dt = employeesTask.Result;
                mEmployeeAllowances_dt = employeeAllowancesTask.Result;
                DataTable employeeSalaryInfo_dt = employeeSalaryInfoTask.Result;

                mEmployees_dt.Columns.Add("BaseSalary", typeof(int));
                mEmployees_dt.Columns.Add("InsuranceBaseSalary", typeof(int));
                mEmployees_dt.Columns.Add("Allowance_Insurance", typeof(int));
                mEmployees_dt.Columns.Add("Allowance_NonInsurance", typeof(int));
                mEmployees_dt.Columns.Add("HourSalary", typeof(int));
                foreach (DataRow row in mEmployees_dt.Rows)
                {
                    string empCode = row["EmployeeCode"].ToString();
                    DataRow[] empSalaryInfoRows = employeeSalaryInfo_dt.Select($"EmployeeCode = '{empCode}'");
                    DataRow[] empAllowancesRows = mEmployeeAllowances_dt.Select($"EmployeeCode = '{empCode}'");

                    DataRow empSalaryInfoRow = empSalaryInfoRows.Length > 0 ? empSalaryInfoRows[0] : null;
                    int baseSalary = 0;
                    if (empSalaryInfoRow != null)
                    {
                        baseSalary = Convert.ToInt32(empSalaryInfoRow["BaseSalary"]);                       
                        row["InsuranceBaseSalary"] = empSalaryInfoRow["InsuranceBaseSalary"];                        
                    }
                    else
                    {
                        row["InsuranceBaseSalary"] = 0;                        
                    }

                    int allowance_Insurance = 0;
                    int allowance_NonInsurance = 0;
                    foreach(DataRow empAllowancesRow in empAllowancesRows)
                    {
                        bool isInsuranceIncluded = Convert.ToBoolean(empAllowancesRow["IsInsuranceIncluded"]);
                        if (isInsuranceIncluded)
                            allowance_Insurance += Convert.ToInt32(empAllowancesRow["Amount"]);
                        else
                            allowance_NonInsurance += Convert.ToInt32(empAllowancesRow["Amount"]);
                    }

                    int employeeInsurancePaid = Convert.ToInt32((allowance_Insurance + baseSalary) * 0.105m);
                    int thuclanh = baseSalary + allowance_Insurance + allowance_NonInsurance - employeeInsurancePaid;                    

                    row["BaseSalary"] = baseSalary;
                    row["Allowance_Insurance"] = allowance_Insurance;
                    row["Allowance_NonInsurance"] = allowance_NonInsurance;
                    row["HourSalary"] = thuclanh / (26 * 8);
                }

                mEmployeeAllowances_dv = new DataView(mEmployeeAllowances_dt);

                int count = 0;
                mEmployees_dt.Columns["EmployeeCode"].SetOrdinal(count++);
                mEmployees_dt.Columns["FullName"].SetOrdinal(count++);                
                mEmployees_dt.Columns["HireDate"].SetOrdinal(count++);
                mEmployees_dt.Columns["DepartmentName"].SetOrdinal(count++);
                mEmployees_dt.Columns["PositionName"].SetOrdinal(count++);
                mEmployees_dt.Columns["ContractTypeName"].SetOrdinal(count++);        
                

                dataGV.DataSource = mEmployees_dt;
                employeeAllowances_GV.DataSource = mEmployeeAllowances_dv;

                Utils.HideColumns(dataGV, new[] { "EmployessName_NoSign", "PositionID", "DepartmentID", "ContractTypeID" });
                Utils.HideColumns(employeeAllowances_GV, new[] { "EmployeeCode"});

                Utils.SetGridHeaders(employeeAllowances_GV, new System.Collections.Generic.Dictionary<string, string> {
                    {"AllowanceName", "Loại Phụ Cấp" },
                    {"IsInsuranceIncluded", "Đ.Bảo Hiểm" },
                    {"Amount", "Số Tiền" }
                });

                Utils.SetGridHeaders(dataGV, new System.Collections.Generic.Dictionary<string, string> {
                    {"BaseSalary", "Lương Cơ Bản" },
                    {"InsuranceBaseSalary", "Lương CS Đóng BH" },
                    {"Allowance_Insurance", "PC Đóng BH" },
                    {"Allowance_NonInsurance", "PC không Đóng BH" },
                    {"EmployeeCode", "Mã NV" },
                    {"FullName", "Họ Và Tên" },
                    {"HireDate", "Ngày Vào Làm" },
                    {"PositionName", "Chức Vụ" },
                    {"DepartmentName", "Phòng Ban" },
                    {"ContractTypeName", "Loại Hợp Đồng" },
                    {"HourSalary", "Lương Theo Giờ" }
                });

                Utils.SetGridWidths(employeeAllowances_GV, new System.Collections.Generic.Dictionary<string, int> {
                    {"AllowanceName", 120},
                    {"IsInsuranceIncluded", 50}
                });

                Utils.SetGridWidths(dataGV, new System.Collections.Generic.Dictionary<string, int> {
                    {"BaseSalary", 70},
                    {"InsuranceBaseSalary", 70},
                    {"Allowance_Insurance", 70},
                    {"Allowance_NonInsurance", 70},
                    {"EmployeeCode", 50},
                    {"FullName", 160},
                    {"HireDate", 70},
                    {"PositionName", 100},
                    {"DepartmentName", 120},
                    {"ContractTypeName", 90}
                });

                Utils.SetGridFormat_NO(employeeAllowances_GV, "Amount");

                Utils.SetGridFormat_NO(dataGV, "InsuranceBaseSalary");
                Utils.SetGridFormat_NO(dataGV, "BaseSalary");
                Utils.SetGridFormat_NO(dataGV, "Allowance_NonInsurance");
                Utils.SetGridFormat_NO(dataGV, "Allowance_Insurance");
                Utils.SetGridFormat_NO(dataGV, "HourSalary");

                if (dataGV.Rows.Count > 0)
                {
                    dataGV.ClearSelection();
                    dataGV.Rows[0].Selected = true;
                }


                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
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

        private void dataGV_CellClick(object sender, EventArgs e)
        {
            if (dataGV.CurrentRow == null) return;
            int rowIndex = dataGV.CurrentRow.Index;
            if (rowIndex < 0)
                return;

            string empCode = dataGV.CurrentRow.Cells["EmployeeCode"].Value.ToString();
            mEmployeeAllowances_dv.RowFilter = $"EmployeeCode = '{empCode}'";
        }

        private void Search_tb_TextChanged(object sender, EventArgs e)
        {
            string keyword = Utils.RemoveVietnameseSigns(search_tb.Text.Trim().ToLower())
                     .Replace("'", "''"); // tránh lỗi cú pháp '

            DataTable dt = dataGV.DataSource as DataTable;
            if (dt == null) return;

            DataView dv = dt.DefaultView;
            dv.RowFilter = $"[EmployessName_NoSign] LIKE '%{keyword}%'";
        }



        private void Print_btn_Click(object sender, EventArgs e)
        {
            InPhieuDuToanLuong(false);
        }

        private void Preview_btn_Click(object sender, EventArgs e)
        {
            InPhieuDuToanLuong(true);
        }

        private void InPhieuDuToanLuong(bool isPreview)
        {
            if (dataGV.CurrentRow == null) return;
            int rowIndex = dataGV.CurrentRow.Index;
            if (rowIndex < 0)
                return;

            string empCode = dataGV.CurrentRow.Cells["EmployeeCode"].Value.ToString();
            DataRow firstRow = null;

            DataRow[] rows = mEmployees_dt.Select($"EmployeeCode = '{empCode}'");

            if (rows.Length > 0)
                firstRow = rows[0];


            PhieuDuToanLuong_Printer printer = new PhieuDuToanLuong_Printer(firstRow, mEmployeeAllowances_dt);
            if (isPreview)
                printer.PrintPreview(this);
            else
                printer.PrintDirect();
        }

        private void Excel_btn_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Excel Files (*.xlsx)|*.xlsx";
                sfd.FileName = $"Employees_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                if (sfd.ShowDialog() != DialogResult.OK)
                    return;

                using (var wb = new XLWorkbook())
                {
                    var ws = wb.Worksheets.Add("Employees");

                    int colIndex = 1;

                    // 1️⃣ Header (chỉ cột Visible)
                    foreach (DataGridViewColumn col in dataGV.Columns)
                    {
                        if (!col.Visible) continue;

                        ws.Cell(1, colIndex).Value = col.HeaderText;
                        ws.Cell(1, colIndex).Style.Font.Bold = true;
                        colIndex++;
                    }

                    // 2️⃣ Data
                    int rowIndex = 2;
                    foreach (DataGridViewRow row in dataGV.Rows)
                    {
                        if (row.IsNewRow) continue; // bỏ dòng trống cuối

                        colIndex = 1;
                        foreach (DataGridViewColumn col in dataGV.Columns)
                        {
                            if (!col.Visible) continue;

                            var cellValue = row.Cells[col.Name].Value;

                            if (cellValue is int || cellValue is decimal || cellValue is double)
                            {
                                ws.Cell(rowIndex, colIndex).Value = Convert.ToDouble(cellValue);
                            }
                            else if (cellValue is DateTime dt)
                            {
                                ws.Cell(rowIndex, colIndex).Value = dt;
                                ws.Cell(rowIndex, colIndex).Style.DateFormat.Format = "dd/MM/yyyy";
                            }
                            else
                            {
                                ws.Cell(rowIndex, colIndex).Value = cellValue?.ToString() ?? "";
                            }

                            colIndex++;
                        }
                        rowIndex++;
                    }

                    // Auto fit
                    ws.Columns().AdjustToContents();

                    // Save
                    wb.SaveAs(sfd.FileName);
                }

                MessageBox.Show("Xuất Excel thành công!",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
