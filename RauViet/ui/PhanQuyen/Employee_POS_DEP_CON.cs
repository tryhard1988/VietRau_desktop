using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RauViet.classes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace RauViet.ui
{    
    public partial class Employee_POS_DEP_CON : Form
    {
        private DataTable mDepartment_dt, mPosition_dt, mContractType_dt, mSalaryGrade_dt, mEmployees_dt;
        public Employee_POS_DEP_CON()
        {
            InitializeComponent();

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            status_lb.Text = "";

            LuuThayDoiBtn.Click += saveBtn_Click;
            dataGV.SelectionChanged += this.dataGV_CellClick;
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
                // Chạy truy vấn trên thread riêng
                string[] keepColumns = { "EmployeeCode", "FullName", "HireDate", "PositionID", "DepartmentID", "ContractTypeID", "SalaryGradeID", "PositionName", "DepartmentName", "ContractTypeName", "GradeName" };
                var employeesTask = SQLStore.Instance.GetEmployeesAsync(keepColumns);
                var departmentTask = SQLStore.Instance.GetActiveDepartmentAsync();
                var positionTask = SQLStore.Instance.GetActivePositionAsync();
                var contractTypeTask = SQLStore.Instance.GetContractTypeAsync();
                var salaryGradeTask = SQLStore.Instance.GetActiveSalaryGradeAsync();

                await Task.WhenAll(employeesTask, departmentTask, positionTask, contractTypeTask, salaryGradeTask);
                mEmployees_dt = employeesTask.Result;
                mDepartment_dt = departmentTask.Result;
                mPosition_dt = positionTask.Result;
                mContractType_dt = contractTypeTask.Result;
                mSalaryGrade_dt = salaryGradeTask.Result;

                
                department_cbb.DataSource = mDepartment_dt;
                department_cbb.DisplayMember = "DepartmentName";
                department_cbb.ValueMember = "DepartmentID";

                position_cbb.DataSource = mPosition_dt;
                position_cbb.DisplayMember = "PositionName";
                position_cbb.ValueMember = "PositionID";

                contractType_cbb.DataSource = mContractType_dt;
                contractType_cbb.DisplayMember = "ContractTypeName";
                contractType_cbb.ValueMember = "ContractTypeID";

                salaryGrade_ccb.DataSource = mSalaryGrade_dt;
                salaryGrade_ccb.DisplayMember = "GradeName";
                salaryGrade_ccb.ValueMember = "SalaryGradeID";                              

                int count = 0;
                mEmployees_dt.Columns["EmployeeCode"].SetOrdinal(count++);
                mEmployees_dt.Columns["FullName"].SetOrdinal(count++);                
                mEmployees_dt.Columns["HireDate"].SetOrdinal(count++);
                mEmployees_dt.Columns["DepartmentName"].SetOrdinal(count++);
                mEmployees_dt.Columns["PositionName"].SetOrdinal(count++);
                mEmployees_dt.Columns["ContractTypeName"].SetOrdinal(count++);                
                mEmployees_dt.Columns["GradeName"].SetOrdinal(count++);
                

                dataGV.DataSource = mEmployees_dt;

                dataGV.Columns["EmployeeCode"].HeaderText = "Mã NV";
                dataGV.Columns["FullName"].HeaderText = "Họ Và Tên";
                dataGV.Columns["HireDate"].HeaderText = "Ngày Vào Làm";
                dataGV.Columns["PositionName"].HeaderText = "Chức Vụ";
                dataGV.Columns["DepartmentName"].HeaderText = "Phòng Ban";
                dataGV.Columns["ContractTypeName"].HeaderText = "Loại Hợp Đồng";
                dataGV.Columns["GradeName"].HeaderText = "Bậc Lương";

                dataGV.Columns["SalaryGradeID"].Visible = false;
                dataGV.Columns["PositionID"].Visible = false;
                dataGV.Columns["DepartmentID"].Visible = false;
                dataGV.Columns["ContractTypeID"].Visible = false;

                dataGV.Columns["EmployeeCode"].Width = 50;
                dataGV.Columns["FullName"].Width = 160;
                dataGV.Columns["HireDate"].Width = 70;
                dataGV.Columns["PositionName"].Width = 100;
                dataGV.Columns["DepartmentName"].Width = 120;
                dataGV.Columns["ContractTypeName"].Width = 90;
                dataGV.Columns["GradeName"].Width = 90;


                if (dataGV.Rows.Count > 0)
                {
                    dataGV.ClearSelection();
                    dataGV.Rows[0].Selected = true;
                    UpdateRightUI(0);
                }


                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
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

        private void dataGV_CellClick(object sender, EventArgs e)
        {
            if (dataGV.CurrentRow == null) return;
            int rowIndex = dataGV.CurrentRow.Index;
            if (rowIndex < 0)
                return;


            UpdateRightUI(rowIndex);
        }

        private void UpdateRightUI(int index)
        {
            var cells = dataGV.Rows[index].Cells;
            int? positionID = Utils.GetIntValue(cells["PositionID"]);
            int? departmentID = Utils.GetIntValue(cells["DepartmentID"]);
            int? contractTypeID = Utils.GetIntValue(cells["ContractTypeID"]);
            int? salaryGradeID = Utils.GetIntValue(cells["SalaryGradeID"]);
            string employeeCode = cells["EmployeeCode"].Value.ToString();
            string fullName = cells["FullName"].Value.ToString();
            DateTime hireDate = Convert.ToDateTime(cells["HireDate"].Value);

            employeeCode_tb.Text = employeeCode.ToString();
            Utils.SafeSelectValue(position_cbb, positionID);
            Utils.SafeSelectValue(department_cbb, departmentID);
            Utils.SafeSelectValue(contractType_cbb, contractTypeID);
            Utils.SafeSelectValue(salaryGrade_ccb, salaryGradeID);

            info_gb.BackColor = Color.DarkGray;
            status_lb.Text = "";
        }
        
        private async void updateData(string maNV, int department, int position, int contractType, int salaryGrade)
        {
            foreach (DataGridViewRow row in dataGV.Rows)
            {
                string id = Convert.ToString(row.Cells["EmployeeCode"].Value);
                if (id.CompareTo(maNV) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager.Instance.updateEmployeeWorkAsync(maNV, position, department, contractType, salaryGrade);

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                row.Cells["EmployeeCode"].Value = maNV;
                                row.Cells["PositionID"].Value = position;
                                row.Cells["DepartmentID"].Value = department;
                                row.Cells["ContractTypeID"].Value = contractType;
                                row.Cells["SalaryGradeID"].Value = salaryGrade;

                                string positionName = mPosition_dt.Select($"PositionID = {position}")[0]["PositionName"].ToString();
                                string departmentName = mDepartment_dt.Select($"DepartmentID = {department}")[0]["DepartmentName"].ToString();
                                string contractTypeName = mContractType_dt.Select($"ContractTypeID = {contractType}")[0]["ContractTypeName"].ToString();
                                string gradeName = mSalaryGrade_dt.Select($"SalaryGradeID = {salaryGrade}")[0]["GradeName"].ToString();
                                string positionCode = mPosition_dt.Select($"PositionID = {position}")[0]["PositionCode"].ToString();
                                string contractTypeCode = mContractType_dt.Select($"ContractTypeID = {contractType}")[0]["ContractTypeCode"].ToString();

                                row.Cells["PositionName"].Value = positionName;
                                row.Cells["DepartmentName"].Value = departmentName;
                                row.Cells["ContractTypeName"].Value = contractTypeName;
                                row.Cells["GradeName"].Value = gradeName;

                                var parameters = new Dictionary<string, object>
                                {
                                    ["PositionID"] = position,
                                    ["DepartmentID"] = department,
                                    ["ContractTypeID"] = contractType,
                                    ["SalaryGradeID"] = salaryGrade,

                                    ["PositionName"] = positionName,
                                    ["DepartmentName"] = departmentName,
                                    ["ContractTypeName"] = contractTypeName,
                                    ["GradeName"] = gradeName,

                                    ["PositionCode"] = positionCode,
                                    ["ContractTypeCode"] = contractTypeCode
                                };
                                SQLStore.Instance.updateEmploy(maNV, parameters);


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


        private void saveBtn_Click(object sender, EventArgs e)
        {

            if (department_cbb.SelectedItem == null || position_cbb.SelectedItem == null || salaryGrade_ccb.SelectedValue == null)
            {
                MessageBox.Show("Sai Dữ Liệu, Kiểm Tra Lại!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int department = Convert.ToInt32(department_cbb.SelectedValue);
            int position = Convert.ToInt32(position_cbb.SelectedValue);
            int contractType = Convert.ToInt32(contractType_cbb.SelectedValue);
            int salaryGrade = Convert.ToInt32(salaryGrade_ccb.SelectedValue);


            if (employeeCode_tb.Text.Length != 0)
                updateData(employeeCode_tb.Text, department,position, contractType, salaryGrade);
        }

    }
}
