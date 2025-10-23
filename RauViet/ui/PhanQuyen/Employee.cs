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
        private DataTable mEmployees_dt;
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
            probationSalaryPercent_tb.KeyPress += Tb_KeyPress_OnlyNumber;
        }

        public async void ShowData()
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            loading_lb.Visible = true;            

            try
            {
                var employeesTask = SQLStore.Instance.GetEmployeesAsync();
                await Task.WhenAll(employeesTask);
                mEmployees_dt = employeesTask.Result;              

                birthdate_dtp.Format = DateTimePickerFormat.Custom;
                birthdate_dtp.CustomFormat = "dd/MM/yyyy";
                hireDate_dtp.Format = DateTimePickerFormat.Custom;
                hireDate_dtp.CustomFormat = "dd/MM/yyyy";
                issueDate_dtp.Format = DateTimePickerFormat.Custom;
                issueDate_dtp.CustomFormat = "dd/MM/yyyy";

                int count = 0;
                mEmployees_dt.Columns["EmployeeCode"].SetOrdinal(count++);
                mEmployees_dt.Columns["FullName"].SetOrdinal(count++);
                mEmployees_dt.Columns["GenderName"].SetOrdinal(count++);
                mEmployees_dt.Columns["BirthDate"].SetOrdinal(count++);
                mEmployees_dt.Columns["HireDate"].SetOrdinal(count++);                
                mEmployees_dt.Columns["IsActive"].SetOrdinal(count++);
                mEmployees_dt.Columns["ProbationSalaryPercent"].SetOrdinal(count++);
                mEmployees_dt.Columns["CitizenID"].SetOrdinal(count++);
                mEmployees_dt.Columns["Address"].SetOrdinal(count++);
                mEmployees_dt.Columns["PhoneNumber"].SetOrdinal(count++);
                mEmployees_dt.Columns["NoteResign"].SetOrdinal(count++);

                dataGV.DataSource = mEmployees_dt;

                dataGV.Columns["EmployeeCode"].HeaderText = "Mã NV";
                dataGV.Columns["FullName"].HeaderText = "Họ Và Tên";
                dataGV.Columns["BirthDate"].HeaderText = "Ngày Sinh";
                dataGV.Columns["HireDate"].HeaderText = "Ngày Vào Làm";
                dataGV.Columns["GenderName"].HeaderText = "G.Tính";
                dataGV.Columns["IsActive"].HeaderText = "Đang Làm";
                dataGV.Columns["ProbationSalaryPercent"].HeaderText = "%Lương Thử Việc";
                dataGV.Columns["CitizenID"].HeaderText = "CCCD/CMND";
                dataGV.Columns["PhoneNumber"].HeaderText = "Số Điện Thoại";
                dataGV.Columns["NoteResign"].HeaderText = "Ra/Vào Công Ty";
                dataGV.Columns["canCreateUserName"].HeaderText = "Cấp Quyền";
                dataGV.Columns["IsInsuranceRefund"].HeaderText = "Hoàn BH";
                dataGV.Columns["NoteResign"].HeaderText = "Ra/Vào Công Ty";

                dataGV.Columns["BirthDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
                dataGV.Columns["HireDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
                dataGV.Columns["IssueDate"].DefaultCellStyle.Format = "dd/MM/yyyy";

                dataGV.Columns["PositionID"].Visible = false;
                dataGV.Columns["DepartmentID"].Visible = false;
                dataGV.Columns["ContractTypeID"].Visible = false;
                dataGV.Columns["CreatedAt"].Visible = false;
                dataGV.Columns["SalaryGradeID"].Visible = false;
                dataGV.Columns["BankName"].Visible = false;
                dataGV.Columns["BankBranch"].Visible = false;
                dataGV.Columns["BankAccountNumber"].Visible = false;
                dataGV.Columns["BankAccountHolder"].Visible = false;
                dataGV.Columns["SocialInsuranceNumber"].Visible = false;
                dataGV.Columns["HealthInsuranceNumber"].Visible = false;

                dataGV.Columns["Gender"].Visible = false;
                dataGV.Columns["EmployeeID"].Visible = false;                
                //dataGV.Columns["canCreateUserName"].Visible = false;
                dataGV.Columns["Address"].Visible = false;
                dataGV.Columns["Hometown"].Visible = false;
             //   dataGV.Columns["CitizenID"].Visible = false;
                dataGV.Columns["IssueDate"].Visible = false;
                dataGV.Columns["IssuePlace"].Visible = false;

                dataGV.Columns["canCreateUserName"].Width = 50;
                dataGV.Columns["IsInsuranceRefund"].Width = 50;
                dataGV.Columns["EmployeeCode"].Width = 50;
                dataGV.Columns["FullName"].Width = 160;
                dataGV.Columns["BirthDate"].Width = 70;
                dataGV.Columns["HireDate"].Width = 70;
                dataGV.Columns["GenderName"].Width = 70;
                dataGV.Columns["IsActive"].Width = 50;
                dataGV.Columns["CitizenID"].Width = 90;
                dataGV.Columns["NoteResign"].Width = 90;


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
            string employeeCode = cells["EmployeeCode"].Value.ToString();
            string fullName = cells["FullName"].Value.ToString();
            DateTime birthDate = Convert.ToDateTime(cells["BirthDate"].Value);
            DateTime hireDate = Convert.ToDateTime(cells["HireDate"].Value);
            Boolean gender = Convert.ToBoolean(cells["Gender"].Value);
            string hometown = cells["Hometown"].Value.ToString();
            string address = cells["Address"].Value.ToString();
            string citizenID = cells["CitizenID"].Value.ToString();
            string phone = cells["PhoneNumber"].Value.ToString();
            string noteResign = cells["NoteResign"].Value.ToString();
            decimal? probationSalaryPercent = Utils.GetDecimalValue(cells["ProbationSalaryPercent"]);

            DateTime? issueDate = DateTime.TryParse(cells["IssueDate"].Value?.ToString(), out DateTime tmp) ? tmp : (DateTime?)null;


            string issuePlace = cells["IssuePlace"].Value.ToString();
            Boolean isActive = Convert.ToBoolean(cells["IsActive"].Value);
            Boolean canCreateUserName = Convert.ToBoolean(cells["canCreateUserName"].Value);
            Boolean isInsuranceRefund = Convert.ToBoolean(cells["IsInsuranceRefund"].Value);

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
            phone_tb.Text = phone;
            noteResign_tb.Text = noteResign;
            probationSalaryPercent_tb.Text = (probationSalaryPercent != null ? probationSalaryPercent : 0).ToString();
            isActive_cb.Checked = isActive;
            canCreateUserName_cb.Checked = canCreateUserName;
            isInsuranceRefund_CB.Checked = isInsuranceRefund;
            nvCode_tb.Enabled = true;
            delete_btn.Enabled = true;

            info_gb.BackColor = Color.DarkGray;
            status_lb.Text = "";
        }
        
        private async void updateData(int employeeId, string maNV, string tenNV, DateTime birthDate, DateTime hireDate,
                                    bool isMale, string homeTown, string address, string citizenID, DateTime? issueDate, string issuePlace,
                                    bool isActive, bool canCreateUserName, decimal probationSalaryPercent, string phone, string noteResign, bool isInsuranceRefund)
        {
            foreach (DataGridViewRow row in dataGV.Rows)
            {
                int id = Convert.ToInt32(row.Cells["EmployeeID"].Value);
                if (id.CompareTo(employeeId) == 0)
                {
                    bool isActiveCurrent = Convert.ToBoolean(row.Cells["IsActive"].Value);
                    string noteResignUpdate = noteResign;
                    if (isActiveCurrent != isActive)
                    {
                        noteResignUpdate += DateTime.Now.ToString("MM/yyyy");
                        noteResignUpdate += isActive == true ? "(Vào); " : "(Ra);";
                    }
                    DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager.Instance.updateEmployeesAsync(employeeId, maNV, tenNV, birthDate, hireDate,
                                    isMale, homeTown, address, citizenID, issueDate, issuePlace, isActive, canCreateUserName, 
                                    probationSalaryPercent, phone, noteResignUpdate, isInsuranceRefund);

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
                                row.Cells["IsActive"].Value = isActive;
                                row.Cells["PhoneNumber"].Value = phone;
                                row.Cells["NoteResign"].Value = noteResignUpdate;
                                row.Cells["canCreateUserName"].Value = canCreateUserName;
                                row.Cells["ProbationSalaryPercent"].Value = probationSalaryPercent;
                                row.Cells["IsInsuranceRefund"].Value = isInsuranceRefund;
                                int age = DateTime.Now.Year - birthDate.Year;

                                row.Cells["GenderName"].Value = (isMale == true ? "Nam" : "Nữ  ") + " - " + age;


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
                                    string address, string citizenID, DateTime? issueDate, string issuePlace, bool isActive,
                                    bool canCreateUserName, decimal probationSalaryPercent, string phone, string noteResign,bool isInsuranceRefund)
        {
            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    string nvCode_temp = "VR" + createID().ToString("D4");
                    int newEmployee = await SQLManager.Instance.insertEmployeeAsync(nvCode_temp,tenNV, birthDate, hireDate,isMale, homeTown, address, 
                        citizenID, issueDate, issuePlace, isActive, canCreateUserName, probationSalaryPercent, phone, noteResign, isInsuranceRefund);
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
                        drToAdd["IsActive"] = isActive;
                        drToAdd["PhoneNumber"] = phone;
                        drToAdd["NoteResign"] = noteResign;
                        drToAdd["canCreateUserName"] = canCreateUserName;
                        drToAdd["ProbationSalaryPercent"] = probationSalaryPercent;
                        drToAdd["IsInsuranceRefund"] = isInsuranceRefund;

                        int age = DateTime.Now.Year - birthDate.Year;

                        drToAdd["GenderName"] = (isMale == true ? "Nam" : "Nữ  ") + " - " + age;

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
            DateTime birthDate = birthdate_dtp.Value;
            DateTime hireDate = hireDate_dtp.Value;

            if ((string.IsNullOrEmpty(nvCode_tb.Text) && !string.IsNullOrEmpty(employeeID_tb.Text)) || 
                tenNV_tb.Text.CompareTo("") == 0 || string.IsNullOrEmpty(probationSalaryPercent_tb.Text))
            {
                MessageBox.Show("Sai Dữ Liệu, Kiểm Tra Lại!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (DateTime.Now.Year - birthDate.Year < 18)
            {
                MessageBox.Show("Nhân Viên Chưa Đủ 18t, Kiểm Tra Lại!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string maNV = nvCode_tb.Text;
            string tenNV = tenNV_tb.Text;            
            bool isMale = gender_cb.Checked;
            string homeTown = hometown_tb.Text;
            string address = address_tb.Text;
            string citizenID = citizenId_tb.Text;
            string phone = phone_tb.Text;
            string noteResign = noteResign_tb.Text;
            DateTime? issueDate = issueDate_dtp.Value;
            string issuePlace = issuePlace_tb.Text;
            decimal probationSalaryPercent = Convert.ToDecimal(probationSalaryPercent_tb.Text);
            bool isActive = isActive_cb.Checked;
            bool canCreateUserName = canCreateUserName_cb.Checked;
            bool isInsuranceRefund = isInsuranceRefund_CB.Checked;

            if (IsCitizenIDDuplicate(citizenID, maNV) == true)
            {
                MessageBox.Show("Số CCCD/CMND Bị Trùng, Kiểm Tra Lại!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(citizenID))
            {
                issuePlace = "";
                issueDate = DateTime.Now;
            }


            if (employeeID_tb.Text.Length != 0)
                updateData(Convert.ToInt32(employeeID_tb.Text), maNV, tenNV, birthDate, hireDate,isMale, homeTown, 
                    address, citizenID, issueDate, issuePlace, isActive, canCreateUserName, probationSalaryPercent, phone, noteResign, isInsuranceRefund);
            else
                createNew(tenNV, birthDate, hireDate,isMale, homeTown, 
                    address, citizenID, issueDate, issuePlace, isActive, canCreateUserName, probationSalaryPercent, phone, noteResign, isInsuranceRefund);

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
            isActive_cb.Checked = true;
            canCreateUserName_cb.Checked = false;

            status_lb.Text = "";
            delete_btn.Enabled = false;
            info_gb.BackColor = Color.Green;
         //   dataGV.ClearSelection();
            return;            
        }

        private bool IsCitizenIDDuplicate(string newCitizenID, string currentEmployeeCode = null)
        {
            if (string.IsNullOrWhiteSpace(newCitizenID))
                return false; // Cho phép trống

            var duplicates = mEmployees_dt.AsEnumerable()
                .Where(row => row.Field<string>("CitizenID") == newCitizenID);

            // Nếu đang cập nhật thì bỏ qua chính nhân viên hiện tại
            if (!string.IsNullOrEmpty(currentEmployeeCode))
            {
                duplicates = duplicates.Where(row =>
                    row.Field<string>("EmployeeCode") != currentEmployeeCode);
            }

            return duplicates.Any();
        }

    }
}
