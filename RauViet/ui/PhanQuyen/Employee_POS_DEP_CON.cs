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
            loading_lb.Text = "Đang tải dữ liệu, vui lòng chờ...";
            loading_lb.Visible = false;

            LuuThayDoiBtn.Click += saveBtn_Click;
            dataGV.SelectionChanged += this.dataGV_CellClick;
        }

        public async void ShowData()
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            loading_lb.Visible = true;            

            try
            {
                // Chạy truy vấn trên thread riêng
                var employeesTask = SQLManager.Instance.GetEmployeeWorkAsyc();
                var departmentTask = SQLManager.Instance.GetActiveDepartmentAsync();
                var positionTask = SQLManager.Instance.GetActivePositionAsync();
                var contractTypeTask = SQLManager.Instance.GetContractTypeAsync();
                var salaryGradeTask = SQLManager.Instance.GetActiveSalaryGradeAsync();

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

                mEmployees_dt.Columns.Add(new DataColumn("Position", typeof(string)));
                mEmployees_dt.Columns.Add(new DataColumn("Department", typeof(string)));
                mEmployees_dt.Columns.Add(new DataColumn("ContractType", typeof(string)));
                mEmployees_dt.Columns.Add(new DataColumn("GradeName", typeof(string)));

                foreach (DataRow dr in mEmployees_dt.Rows)
                {
                    int? positionID = Utils.GetIntValue(dr["PositionID"]);
                    int? departmentID = Utils.GetIntValue(dr["DepartmentID"]);
                    int? salaryGradeID = Utils.GetIntValue(dr["SalaryGradeID"]);
                    int? contractTypeID = Utils.GetIntValue(dr["ContractTypeID"]);

                    string positionName = "";
                    string departmentName = "";
                    string gradeName = "";
                    string contractTypeName = "";
                    if (positionID != null)
                    {
                        DataRow[] postionRows = mPosition_dt.Select($"PositionID = {positionID}");
                        if (postionRows.Length > 0)
                            positionName = postionRows[0]["PositionName"].ToString();
                    }

                    if (departmentID != null)
                    {
                        DataRow[] departmentRows = mDepartment_dt.Select($"departmentID = {departmentID}");
                        if (departmentRows.Length > 0)
                            departmentName = departmentRows[0]["DepartmentName"].ToString();
                    }
                    if (salaryGradeID != null)
                    {
                        DataRow[] salaryGradeRows = mSalaryGrade_dt.Select($"SalaryGradeID = {salaryGradeID}");
                        if (salaryGradeRows.Length > 0)
                            gradeName = salaryGradeRows[0]["GradeName"].ToString();
                    }

                    if (contractTypeID != null)
                    {
                        DataRow[] contractTypeRows = mContractType_dt.Select($"ContractTypeID = {contractTypeID}");
                        if (contractTypeRows.Length > 0)
                            contractTypeName = contractTypeRows[0]["ContractTypeName"].ToString();
                    }                    
                    dr["Position"] = positionName;
                    dr["Department"] = departmentName;
                    dr["ContractType"] = contractTypeName;
                    dr["GradeName"] = gradeName;
                }

                int count = 0;
                mEmployees_dt.Columns["EmployeeCode"].SetOrdinal(count++);
                mEmployees_dt.Columns["FullName"].SetOrdinal(count++);                
                mEmployees_dt.Columns["HireDate"].SetOrdinal(count++);
                mEmployees_dt.Columns["Department"].SetOrdinal(count++);
                mEmployees_dt.Columns["Position"].SetOrdinal(count++);
                mEmployees_dt.Columns["ContractType"].SetOrdinal(count++);                
                mEmployees_dt.Columns["GradeName"].SetOrdinal(count++);
                

                dataGV.DataSource = mEmployees_dt;

                dataGV.Columns["EmployeeCode"].HeaderText = "Mã NV";
                dataGV.Columns["FullName"].HeaderText = "Họ Và Tên";
                dataGV.Columns["HireDate"].HeaderText = "Ngày Vào Làm";
                dataGV.Columns["Position"].HeaderText = "Chức Vụ";
                dataGV.Columns["Department"].HeaderText = "Phòng Ban";
                dataGV.Columns["ContractType"].HeaderText = "Loại Hợp Đồng";
                dataGV.Columns["GradeName"].HeaderText = "Bậc Lương";

                dataGV.Columns["SalaryGradeID"].Visible = false;
                dataGV.Columns["PositionID"].Visible = false;
                dataGV.Columns["DepartmentID"].Visible = false;
                dataGV.Columns["ContractTypeID"].Visible = false;

                dataGV.Columns["EmployeeCode"].Width = 50;
                dataGV.Columns["FullName"].Width = 160;
                dataGV.Columns["HireDate"].Width = 70;
                dataGV.Columns["Position"].Width = 100;
                dataGV.Columns["Department"].Width = 120;
                dataGV.Columns["ContractType"].Width = 90;
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
                loading_lb.Visible = false; // ẩn loading
                loading_lb.Enabled = true; // enable lại button
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

                                row.Cells["Position"].Value = positionName;
                                row.Cells["Department"].Value = departmentName;
                                row.Cells["ContractType"].Value = contractTypeName;
                                row.Cells["GradeName"].Value = gradeName;


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
