using DocumentFormat.OpenXml.Bibliography;
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
                foreach (DataRow row in mEmployees_dt.Rows)
                {
                    string empCode = row["EmployeeCode"].ToString();
                    DataRow[] empSalaryInfoRows = employeeSalaryInfo_dt.Select($"EmployeeCode = '{empCode}'");
                    DataRow[] empAllowancesRows = mEmployeeAllowances_dt.Select($"EmployeeCode = '{empCode}'");

                    DataRow empSalaryInfoRow = empSalaryInfoRows.Length > 0 ? empSalaryInfoRows[0] : null;
                    if(empSalaryInfoRow != null)
                    {
                        row["BaseSalary"] = empSalaryInfoRow["BaseSalary"];
                        row["InsuranceBaseSalary"] = empSalaryInfoRow["InsuranceBaseSalary"];
                    }
                    else
                    {
                        row["BaseSalary"] = 0;
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
                    row["Allowance_Insurance"] = allowance_Insurance;
                    row["Allowance_NonInsurance"] = allowance_NonInsurance;
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

                employeeAllowances_GV.Columns["EmployeeCode"].Visible = false;
                employeeAllowances_GV.Columns["AllowanceName"].Width = 120;
                employeeAllowances_GV.Columns["IsInsuranceIncluded"].Width = 50;
                employeeAllowances_GV.Columns["Amount"].DefaultCellStyle.Format = "N0";

                employeeAllowances_GV.Columns["AllowanceName"].HeaderText = "Loại Phụ Cấp";
                employeeAllowances_GV.Columns["IsInsuranceIncluded"].HeaderText = "Đ.Bảo Hiểm";
                employeeAllowances_GV.Columns["Amount"].HeaderText = "Số Tiền";

                dataGV.Columns["BaseSalary"].HeaderText = "Lương Cơ Bản";
                dataGV.Columns["InsuranceBaseSalary"].HeaderText = "Lương CS Đóng BH";
                dataGV.Columns["Allowance_Insurance"].HeaderText = "PC Đóng BH";
                dataGV.Columns["Allowance_NonInsurance"].HeaderText = "PC không Đóng BH";
                dataGV.Columns["EmployeeCode"].HeaderText = "Mã NV";
                dataGV.Columns["FullName"].HeaderText = "Họ Và Tên";
                dataGV.Columns["HireDate"].HeaderText = "Ngày Vào Làm";
                dataGV.Columns["PositionName"].HeaderText = "Chức Vụ";
                dataGV.Columns["DepartmentName"].HeaderText = "Phòng Ban";
                dataGV.Columns["ContractTypeName"].HeaderText = "Loại Hợp Đồng";

                dataGV.Columns["EmployessName_NoSign"].Visible = false;
                dataGV.Columns["PositionID"].Visible = false;
                dataGV.Columns["DepartmentID"].Visible = false;
                dataGV.Columns["ContractTypeID"].Visible = false;

                dataGV.Columns["BaseSalary"].Width = 70;
                dataGV.Columns["InsuranceBaseSalary"].Width = 70;
                dataGV.Columns["Allowance_Insurance"].Width = 70;
                dataGV.Columns["Allowance_NonInsurance"].Width = 70;
                dataGV.Columns["EmployeeCode"].Width = 50;
                dataGV.Columns["FullName"].Width = 160;
                dataGV.Columns["HireDate"].Width = 70;
                dataGV.Columns["PositionName"].Width = 100;
                dataGV.Columns["DepartmentName"].Width = 120;
                dataGV.Columns["ContractTypeName"].Width = 90;

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

    }
}
