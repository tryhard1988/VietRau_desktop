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
    public partial class Employee : Form
    {
        private DataTable mDepartment_dt, mPosition_dt, mContractType_dt;
        public Employee()
        {
            InitializeComponent();

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            status_lb.Text = "";
            loading_lb.Text = "Đang tải dữ liệu, vui lòng chờ...";
            loading_lb.Visible = false;
            delete_btn.Enabled = false;

            newCustomerBtn.Click += newCustomerBtn_Click;
            LuuThayDoiBtn.Click += saveBtn_Click;
            delete_btn.Click += deleteBtn_Click;
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
                var employeesTask = SQLManager.Instance.GetEmployeesAsync();
                var departmentTask = SQLManager.Instance.GetActiveDepartmentAsync();
                var positionTask = SQLManager.Instance.GetActivePositionAsync();
                var contractTypeTask = SQLManager.Instance.GetContractTypeAsync();

                await Task.WhenAll(employeesTask, departmentTask, positionTask, contractTypeTask);
                DataTable employees_dt = employeesTask.Result;
                mDepartment_dt = departmentTask.Result;
                mPosition_dt = positionTask.Result;
                mContractType_dt = contractTypeTask.Result;

                birthdate_dtp.Format = DateTimePickerFormat.Custom;
                birthdate_dtp.CustomFormat = "dd/MM/yyyy";
                hireDate_dtp.Format = DateTimePickerFormat.Custom;
                hireDate_dtp.CustomFormat = "dd/MM/yyyy";
                issueDate_dtp.Format = DateTimePickerFormat.Custom;
                issueDate_dtp.CustomFormat = "dd/MM/yyyy";

                department_cbb.DataSource = mDepartment_dt;
                department_cbb.DisplayMember = "DepartmentName";
                department_cbb.ValueMember = "DepartmentID";

                position_cbb.DataSource = mPosition_dt;
                position_cbb.DisplayMember = "PositionName";
                position_cbb.ValueMember = "PositionID";

                contractType_cbb.DataSource = mContractType_dt;
                contractType_cbb.DisplayMember = "ContractTypeName";
                contractType_cbb.ValueMember = "ContractTypeID";

                employees_dt.Columns.Add(new DataColumn("GenderName", typeof(string)));
                employees_dt.Columns.Add(new DataColumn("Age", typeof(int)));
                employees_dt.Columns.Add(new DataColumn("Position", typeof(string)));
                employees_dt.Columns.Add(new DataColumn("Department", typeof(string)));
                employees_dt.Columns.Add(new DataColumn("ContractType", typeof(string)));

                foreach (DataRow dr in employees_dt.Rows)
                {
                    int age = DateTime.Now.Year - Convert.ToDateTime(dr["BirthDate"]).Year;
                    int positionID = Convert.ToInt32(dr["PositionID"]);
                    int departmentID = Convert.ToInt32(dr["DepartmentID"]);
                    int contractTypeID = Convert.ToInt32(dr["ContractTypeID"]);
                    Boolean gender = Convert.ToBoolean(dr["Gender"]);


                    string positionName = mPosition_dt.Select($"PositionID = {positionID}")[0]["PositionName"].ToString();
                    string departmentName = mDepartment_dt.Select($"DepartmentID = {departmentID}")[0]["DepartmentName"].ToString();
                    string contractTypeName = mContractType_dt.Select($"ContractTypeID = {contractTypeID}")[0]["ContractTypeName"].ToString();

                    dr["Age"] = age;
                    dr["GenderName"] = gender == true? "Nam" : "Nữ";
                    dr["Position"] = positionName;
                    dr["Department"] = departmentName;
                    dr["ContractType"] = contractTypeName;
                }

                int count = 0;
                employees_dt.Columns["EmployeeCode"].SetOrdinal(count++);
                employees_dt.Columns["FullName"].SetOrdinal(count++);
                employees_dt.Columns["GenderName"].SetOrdinal(count++);
                employees_dt.Columns["BirthDate"].SetOrdinal(count++);
                employees_dt.Columns["Age"].SetOrdinal(count++);
                employees_dt.Columns["HireDate"].SetOrdinal(count++);
                employees_dt.Columns["Department"].SetOrdinal(count++);
                employees_dt.Columns["Position"].SetOrdinal(count++);
                employees_dt.Columns["ContractType"].SetOrdinal(count++);
                employees_dt.Columns["IsActive"].SetOrdinal(count++);
                                
                dataGV.DataSource = employees_dt;

                dataGV.Columns["EmployeeCode"].HeaderText = "Mã NV";
                dataGV.Columns["FullName"].HeaderText = "Họ Và Tên";
                dataGV.Columns["BirthDate"].HeaderText = "Ngày Sinh";
                dataGV.Columns["HireDate"].HeaderText = "Ngày Vào Làm";
                dataGV.Columns["GenderName"].HeaderText = "G.Tính";
                dataGV.Columns["Position"].HeaderText = "Chức Vụ";
                dataGV.Columns["Department"].HeaderText = "Phòng Ban";
                dataGV.Columns["ContractType"].HeaderText = "Loại Hợp Đồng";
                dataGV.Columns["IsActive"].HeaderText = "Đang Làm";
                dataGV.Columns["Age"].HeaderText = "Tuổi";

                dataGV.Columns["BirthDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
                dataGV.Columns["HireDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
                dataGV.Columns["IssueDate"].DefaultCellStyle.Format = "dd/MM/yyyy";

                dataGV.Columns["Gender"].Visible = false;
                dataGV.Columns["EmployeeID"].Visible = false;
                dataGV.Columns["PositionID"].Visible = false;
                dataGV.Columns["DepartmentID"].Visible = false;
                dataGV.Columns["ContractTypeID"].Visible = false;
                dataGV.Columns["canCreateUserName"].Visible = false;
                dataGV.Columns["Address"].Visible = false;
                dataGV.Columns["Hometown"].Visible = false;
                dataGV.Columns["CitizenID"].Visible = false;
                dataGV.Columns["IssueDate"].Visible = false;
                dataGV.Columns["IssuePlace"].Visible = false;
                dataGV.Columns["CreatedAt"].Visible = false;

                dataGV.Columns["EmployeeCode"].Width = 50;
                dataGV.Columns["FullName"].Width = 160;
                dataGV.Columns["BirthDate"].Width = 70;
                dataGV.Columns["HireDate"].Width = 70;
                dataGV.Columns["GenderName"].Width = 50;
                dataGV.Columns["Position"].Width = 100;
                dataGV.Columns["Department"].Width = 120;
                dataGV.Columns["ContractType"].Width = 90;
                dataGV.Columns["IsActive"].Width = 50;
                dataGV.Columns["Age"].Width = 40;

                //dataGV.Columns["EmployeeCode"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                //dataGV.Columns["FullName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                //dataGV.Columns["BirthDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                //dataGV.Columns["HireDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                //dataGV.Columns["GenderName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                //dataGV.Columns["Position"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                //dataGV.Columns["Department"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                //dataGV.Columns["ContractType"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                //dataGV.Columns["IsActive"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                //dataGV.Columns["Age"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

                // dataGV.Columns["UserID"].Visible = false;

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
        
        private int createID()
        {
            var existingIds = dataGV.Rows
            .Cast<DataGridViewRow>()
            .Where(r => !r.IsNewRow && r.Cells["EmployeeID"].Value != null)
            .Select(r => Convert.ToInt32(r.Cells["EmployeeID"].Value))
            .ToList();

            int newCustomerID = existingIds.Count > 0 ? existingIds.Max() + 1 : 1;

            return newCustomerID;
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
            int employeeID = Convert.ToInt32(cells["EmployeeID"].Value);
            int positionID = Convert.ToInt32(cells["PositionID"].Value);
            int departmentID = Convert.ToInt32(cells["DepartmentID"].Value);
            int contractTypeID = Convert.ToInt32(cells["ContractTypeID"].Value);
            string employeeCode = cells["EmployeeCode"].Value.ToString();
            string fullName = cells["FullName"].Value.ToString();
            DateTime birthDate = Convert.ToDateTime(cells["BirthDate"].Value);
            DateTime hireDate = Convert.ToDateTime(cells["HireDate"].Value);
            Boolean gender = Convert.ToBoolean(cells["Gender"].Value);
            string hometown = cells["Hometown"].Value.ToString();
            string address = cells["Address"].Value.ToString();
            string citizenID = cells["CitizenID"].Value.ToString();

            DateTime? issueDate = DateTime.TryParse(cells["IssueDate"].Value?.ToString(), out DateTime tmp) ? tmp : (DateTime?)null;


            string issuePlace = cells["IssuePlace"].Value.ToString();
            Boolean isActive = Convert.ToBoolean(cells["IsActive"].Value);
            Boolean canCreateUserName = Convert.ToBoolean(cells["canCreateUserName"].Value);

            employeeID_tb.Text = employeeID.ToString();
            nvCode_tb.Text = employeeCode;
            tenNV_tb.Text = fullName;
            birthdate_dtp.Value = birthDate;
            hireDate_dtp.Value = hireDate;
            gender_cb.Checked = gender;
            hometown_tb.Text = hometown;
            address_tb.Text = address;
            citizenId_tb.Text = citizenID;
            issueDate_dtp.Value = issueDate ?? DateTime.Now;
            issuePlace_tb.Text = issuePlace;
            position_cbb.SelectedValue = positionID;
            department_cbb.SelectedValue = departmentID;
            contractType_cbb.SelectedValue = contractTypeID;
            isActive_cb.Checked = isActive;
            canCreateUserName_cb.Checked = canCreateUserName;
            nvCode_tb.Enabled = true;
            delete_btn.Enabled = true;

            info_gb.BackColor = Color.DarkGray;
            status_lb.Text = "";
        }
        
        private async void updateData(int employeeId, string maNV, string tenNV, DateTime birthDate, DateTime hireDate,
                                    bool isMale, string homeTown, string address, string citizenID, DateTime? issueDate, string issuePlace, int department, 
                                    int position, int contractType, bool isActive, bool canCreateUserName)
        {
            foreach (DataGridViewRow row in dataGV.Rows)
            {
                int id = Convert.ToInt32(row.Cells["EmployeeID"].Value);
                if (id.CompareTo(employeeId) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager.Instance.updateEmployeesAsync(employeeId, maNV, tenNV, birthDate, hireDate,
                                    isMale, homeTown, address, citizenID, issueDate, issuePlace, department, position, contractType, isActive, canCreateUserName);

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                row.Cells["EmployeeCode"].Value = maNV;
                                row.Cells["FullName"].Value = tenNV;
                                row.Cells["BirthDate"].Value = birthDate;
                                row.Cells["HireDate"].Value = hireDate;
                                row.Cells["Gender"].Value = isMale;
                                row.Cells["Hometown"].Value = homeTown;
                                row.Cells["Address"].Value = address;
                                row.Cells["CitizenID"].Value = citizenID;
                                row.Cells["IssueDate"].Value = issueDate;
                                row.Cells["IssuePlace"].Value = issuePlace;
                                row.Cells["PositionID"].Value = position;
                                row.Cells["DepartmentID"].Value = department;
                                row.Cells["ContractTypeID"].Value = contractType;
                                row.Cells["IsActive"].Value = isActive;
                                row.Cells["canCreateUserName"].Value = canCreateUserName;

                                int age = DateTime.Now.Year - birthDate.Year;

                                string positionName = mPosition_dt.Select($"PositionID = {position}")[0]["PositionName"].ToString();
                                string departmentName = mDepartment_dt.Select($"DepartmentID = {department}")[0]["DepartmentName"].ToString();
                                string contractTypeName = mContractType_dt.Select($"ContractTypeID = {contractType}")[0]["ContractTypeName"].ToString();

                                row.Cells["Age"].Value = age;
                                row.Cells["GenderName"].Value = isMale == true ? "Nam" : "Nữ";
                                row.Cells["Position"].Value = positionName;
                                row.Cells["Department"].Value = departmentName;
                                row.Cells["ContractType"].Value = contractTypeName;


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

        private async void createNew(string tenNV, DateTime birthDate, DateTime hireDate, bool isMale, string homeTown, 
                                    string address, string citizenID, DateTime? issueDate, string issuePlace, int department,
                                    int position, int contractType, bool isActive, bool canCreateUserName)
        {
            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    string nvCode_temp = "VR" + createID().ToString("D4");
                    int newEmployee = await SQLManager.Instance.insertEmployeeAsync(nvCode_temp,tenNV, birthDate, hireDate,isMale, homeTown, address, 
                        citizenID, issueDate, issuePlace, department, position, contractType, isActive, canCreateUserName);
                    if (newEmployee > 0)
                    {
                        DataTable dataTable = (DataTable)dataGV.DataSource;
                        DataRow drToAdd = dataTable.NewRow();


                        employeeID_tb.Text = newEmployee.ToString();
                        drToAdd["EmployeeID"] = newEmployee;
                        drToAdd["EmployeeCode"] = "VR" + newEmployee.ToString("D4");
                        drToAdd["FullName"] = tenNV;
                        drToAdd["BirthDate"] = birthDate;
                        drToAdd["HireDate"] = hireDate;
                        drToAdd["Gender"] = isMale;
                        drToAdd["Hometown"] = homeTown;
                        drToAdd["Address"] = address;
                        drToAdd["CitizenID"] = citizenID;
                        drToAdd["IssueDate"] = issueDate;
                        drToAdd["IssuePlace"] = issuePlace;
                        drToAdd["PositionID"] = position;
                        drToAdd["DepartmentID"] = department;
                        drToAdd["ContractTypeID"] = contractType;
                        drToAdd["IsActive"] = isActive;
                        drToAdd["canCreateUserName"] = canCreateUserName;

                        int age = DateTime.Now.Year - birthDate.Year;

                        string positionName = mPosition_dt.Select($"PositionID = {position}")[0]["PositionName"].ToString();
                        string departmentName = mDepartment_dt.Select($"DepartmentID = {department}")[0]["DepartmentName"].ToString();
                        string contractTypeName = mContractType_dt.Select($"ContractTypeID = {contractType}")[0]["ContractTypeName"].ToString();

                        drToAdd["Age"] = age;
                        drToAdd["GenderName"] = isMale == true ? "Nam" : "Nữ";
                        drToAdd["Position"] = positionName;
                        drToAdd["Department"] = departmentName;
                        drToAdd["ContractType"] = contractTypeName;


                        dataTable.Rows.Add(drToAdd);
                        dataTable.AcceptChanges();

                        dataGV.ClearSelection(); // bỏ chọn row cũ
                        int rowIndex = dataGV.Rows.Count - 1;
                        dataGV.Rows[rowIndex].Selected = true;
                        UpdateRightUI(dataGV.Rows.Count - 1);


                        status_lb.Text = "Thành công";
                        status_lb.ForeColor = Color.Green;

                        newCustomerBtn_Click(null, null);
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

        private void saveBtn_Click(object sender, EventArgs e)
        {
            string maNV = nvCode_tb.Text;
            string tenNV = tenNV_tb.Text;
            DateTime birthDate = birthdate_dtp.Value;
            DateTime hireDate = hireDate_dtp.Value;
            bool isMale = gender_cb.Checked;
            string homeTown = hometown_tb.Text;
            string address = address_tb.Text;
            string citizenID = citizenId_tb.Text;
            DateTime? issueDate = issueDate_dtp.Value;
            string issuePlace = issuePlace_tb.Text;
            int department = Convert.ToInt32(department_cbb.SelectedValue);
            int position = Convert.ToInt32(position_cbb.SelectedValue);
            int contractType = Convert.ToInt32(contractType_cbb.SelectedValue);
            bool isActive = isActive_cb.Checked;
            bool canCreateUserName = canCreateUserName_cb.Checked;

            if ((maNV.CompareTo("") == 0 && employeeID_tb.Text.CompareTo("") != 0) || tenNV.CompareTo("") == 0 || DateTime.Now.Year - birthDate.Year < 18)
            {
                MessageBox.Show("Sai Dữ Liệu, Kiểm Tra Lại!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(citizenID) == true)
            {
                issuePlace = "";
                issueDate = DateTime.Now;
            }


            if (employeeID_tb.Text.Length != 0)
                updateData(Convert.ToInt32(employeeID_tb.Text), maNV, tenNV, birthDate, hireDate,isMale, homeTown, 
                    address, citizenID, issueDate, issuePlace, department,position, contractType, isActive, canCreateUserName);
            else
                createNew(tenNV, birthDate, hireDate,isMale, homeTown, 
                    address, citizenID, issueDate, issuePlace, department,position, contractType, isActive, canCreateUserName);

        }
        private async void deleteBtn_Click(object sender, EventArgs e)
        {
            if (dataGV.SelectedRows.Count == 0) return;

            string employeeID = employeeID_tb.Text;

            foreach (DataGridViewRow row in dataGV.Rows)
            {
                string id = row.Cells["EmployeeID"].Value.ToString();
                if (id.CompareTo(employeeID) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("XÓA THÔNG TIN \n Chắc chắn chưa ?", " Xóa Thông Tin", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager.Instance.deleteEmployeeAsync(Convert.ToInt32(employeeID));

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                int delRowInd = row.Index;
                                dataGV.Rows.Remove(row);
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

        private void newCustomerBtn_Click(object sender, EventArgs e)
        {
            nvCode_tb.Enabled = false;
            employeeID_tb.Text = "";
            nvCode_tb.Text = "";
            tenNV_tb.Text = "";
            hireDate_dtp.Value = DateTime.Now;
            citizenId_tb.Text = "";
            contractType_cbb.SelectedValue = 2;
            isActive_cb.Checked = true;
            canCreateUserName_cb.Checked = false;

            status_lb.Text = "";
            delete_btn.Enabled = false;
            info_gb.BackColor = Color.Green;
         //   dataGV.ClearSelection();
            return;            
        }
    }
}
