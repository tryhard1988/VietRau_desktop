using RauViet.classes;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Color = System.Drawing.Color;

namespace RauViet.ui
{    
    public partial class Employee : Form
    {
        private DataTable mEmployees_dt, mSalaryGrade_dt;
        private DataView mLogDV;
        bool isNewState = false;
        public Employee()
        {
            InitializeComponent();
            this.KeyPreview = true;
            Utils.SetTabStopRecursive(this, false);

            int countTab = 0;
            nvCode_tb.TabIndex = countTab++; nvCode_tb.TabStop = true;
            tenNV_tb.TabIndex = countTab++; tenNV_tb.TabStop = true;
            birthdate_dtp.TabIndex = countTab++; birthdate_dtp.TabStop = true;
            hireDate_dtp.TabIndex = countTab++; hireDate_dtp.TabStop = true;
            gender_cb.TabIndex = countTab++; gender_cb.TabStop = true;
            hometown_tb.TabIndex = countTab++; hometown_tb.TabStop = true;
            address_tb.TabIndex = countTab++; address_tb.TabStop = true;
            citizenId_tb.TabIndex = countTab++; citizenId_tb.TabStop = true;
            issueDate_dtp.TabIndex = countTab++; issueDate_dtp.TabStop = true;
            issuePlace_tb.TabIndex = countTab++; issuePlace_tb.TabStop = true;
            phone_tb.TabIndex = countTab++; phone_tb.TabStop = true;
            salaryGrade_ccb.TabIndex = countTab++; salaryGrade_ccb.TabStop = true;
            isActive_cb.TabIndex = countTab++; isActive_cb.TabStop = true;
            canCreateUserName_cb.TabIndex = countTab++; canCreateUserName_cb.TabStop = true;
            isInsuranceRefund_CB.TabIndex = countTab++; isInsuranceRefund_CB.TabStop = true;
            LuuThayDoiBtn.TabIndex = countTab++; LuuThayDoiBtn.TabStop = true;

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            status_lb.Text = "";

            newCustomerBtn.Click += newCustomerBtn_Click;
            LuuThayDoiBtn.Click += saveBtn_Click;
            delete_btn.Click += deleteBtn_Click;
            dataGV.SelectionChanged += this.dataGV_CellClick;

            edit_btn.Click += Edit_btn_Click;
            readOnly_btn.Click += ReadOnly_btn_Click;
            ReadOnly_btn_Click(null, null);

            this.KeyDown += Employee_KeyDown;
            search_tb.TextChanged += Search_tb_TextChanged;
        }

        private void Employee_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                SQLStore_QLNS.Instance.removeEmployees();
                ShowData();
            }
        }

        public async void ShowData()
        {
            await Task.Delay(50);
            LoadingOverlay loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Show();

            try
            {
                var employeesTask = SQLStore_QLNS.Instance.GetEmployeesAsync();
                var salaryGradeTask = SQLStore_QLNS.Instance.GetActiveSalaryGradeAsync();
                var employeeLogTask = SQLStore_QLNS.Instance.GetEmployeeLogAsync();
                await Task.WhenAll(employeesTask, salaryGradeTask, employeeLogTask);
                mEmployees_dt = employeesTask.Result;
                mSalaryGrade_dt = salaryGradeTask.Result;
                mLogDV = new DataView(employeeLogTask.Result);

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
                mEmployees_dt.Columns["CitizenID"].SetOrdinal(count++);
                mEmployees_dt.Columns["Address"].SetOrdinal(count++);
                mEmployees_dt.Columns["PhoneNumber"].SetOrdinal(count++);
                mEmployees_dt.Columns["NoteResign"].SetOrdinal(count++);
                mEmployees_dt.Columns["GradeName"].SetOrdinal(count++);

                salaryGrade_ccb.DataSource = mSalaryGrade_dt;
                salaryGrade_ccb.DisplayMember = "GradeName";
                salaryGrade_ccb.ValueMember = "SalaryGradeID";

                dataGV.DataSource = mEmployees_dt;
                log_GV.DataSource = mLogDV;
                Utils.HideColumns(dataGV, new[] { "IssuePlace", "IssueDate", "Hometown", "Address", "EmployeeID", "Gender", "ContractTypeName", "ContractTypeCode", "DepartmentName", "PositionName", "PositionCode", "HealthInsuranceNumber", "SocialInsuranceNumber",
                                                    "BankAccountHolder","BankAccountNumber","BankBranch","BankName","SalaryGrade","SalaryGradeID", "CreatedAt", "ContractTypeID", "DepartmentID", "PositionID", "NoteResign", "EmployessName_NoSign"});
                Utils.HideColumns(log_GV, new[] { "LogID", "EmployeeCode" });

                Utils.SetGridHeaders(dataGV, new System.Collections.Generic.Dictionary<string, string> {
                    {"EmployeeCode", "Mã NV" },
                    {"FullName", "Họ Và Tên" },
                    {"BirthDate", "Ngày Sinh" },
                    {"HireDate", "Ngày Vào Làm" },
                    {"GenderName", "G.Tính" },
                    {"IsActive", "Đang Làm" },
                    {"CitizenID", "CCCD/CMND" },
                    {"PhoneNumber", "Số Điện Thoại" },
                    {"NoteResign", "Ra/Vào Công Ty" },
                    {"canCreateUserName", "Cấp Quyền" },
                    {"IsInsuranceRefund", "Hoàn BH" },
                    {"Gradename", "Bậc\nLương" }
                });

                Utils.SetGridHeaders(log_GV, new System.Collections.Generic.Dictionary<string, string> {
                    {"CreatedAt", "Thời điểm thay đổi" },
                    {"ACtionBy", "Người thay đổi" },
                    {"OldValue", "Giá trị cũ" },
                    {"NewValue", "Giá trị mới" }
                });

                dataGV.Columns["BirthDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
                dataGV.Columns["HireDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
                dataGV.Columns["IssueDate"].DefaultCellStyle.Format = "dd/MM/yyyy";

                Utils.SetGridWidths(dataGV, new System.Collections.Generic.Dictionary<string, int> {
                    {"canCreateUserName", 50},
                    {"IsInsuranceRefund", 50},
                    {"EmployeeCode", 50},
                    {"FullName", 160},
                    {"BirthDate", 70},
                    {"HireDate", 70},
                    {"GenderName", 60},
                    {"IsActive", 50},
                    {"CitizenID", 90},
                    {"NoteResign", 90},
                    {"Gradename", 70},
                    {"PhoneNumber", 70}
                });

                Utils.SetGridWidths(log_GV, new System.Collections.Generic.Dictionary<string, int> {
                    {"CreatedAt", 120},
                    {"ACtionBy", 150},
                });

                dataGV.Columns["PhoneNumber"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGV.Columns["Gradename"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                log_GV.Columns["OldValue"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                log_GV.Columns["NewValue"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                log_GV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                if (dataGV.Rows.Count > 0)
                {
                    dataGV.ClearSelection();
                    dataGV.Rows[0].Selected = true;
                    UpdateRightUI(0);
                }
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
            if (isNewState == true) return;

            var cells = dataGV.Rows[index].Cells;
            int employeeID = Convert.ToInt32(cells["EmployeeID"].Value);
            string employeeCode = cells["EmployeeCode"].Value.ToString();
            string fullName = cells["FullName"].Value.ToString();
            DateTime hireDate = Convert.ToDateTime(cells["HireDate"].Value);
            Boolean gender = Convert.ToBoolean(cells["Gender"].Value);
            string hometown = cells["Hometown"].Value.ToString();
            string address = cells["Address"].Value.ToString();
            string citizenID = cells["CitizenID"].Value.ToString();
            string phone = cells["PhoneNumber"].Value.ToString();
            string noteResign = cells["NoteResign"].Value.ToString();
            int? salaryGradeID = Utils.GetIntValue(cells["SalaryGradeID"]);
            DateTime? birthDate = DateTime.TryParse(cells["BirthDate"].Value?.ToString(), out DateTime tmp1) ? tmp1 : (DateTime?)null;
            DateTime? issueDate = DateTime.TryParse(cells["IssueDate"].Value?.ToString(), out DateTime tmp) ? tmp : (DateTime?)null;


            string issuePlace = cells["IssuePlace"].Value.ToString();
            Boolean isActive = Convert.ToBoolean(cells["IsActive"].Value);
            Boolean canCreateUserName = Convert.ToBoolean(cells["canCreateUserName"].Value);
            Boolean isInsuranceRefund = Convert.ToBoolean(cells["IsInsuranceRefund"].Value);

            Utils.SafeSelectValue(salaryGrade_ccb, salaryGradeID);
            employeeID_tb.Text = employeeID.ToString();
            nvCode_tb.Text = employeeCode;
            tenNV_tb.Text = fullName;
            birthdate_dtp.Value = birthDate ?? DateTime.Now;
            hireDate_dtp.Value = hireDate;
            gender_cb.Checked = gender;
            hometown_tb.Text = hometown;
            address_tb.Text = address;
            citizenId_tb.Text = citizenID;
            issueDate_dtp.Value = issueDate ?? DateTime.Now;
            issuePlace_tb.Text = issuePlace;
            phone_tb.Text = phone;
            noteResign_tb.Text = noteResign;
            isActive_cb.Checked = isActive;
            canCreateUserName_cb.Checked = canCreateUserName;
            isInsuranceRefund_CB.Checked = isInsuranceRefund;
            nvCode_tb.Enabled = true;

            status_lb.Text = "";

            mLogDV.RowFilter = $"EmployeeCode = '{employeeCode}'";
        }
        
        private async void updateData(int employeeId, string maNV, string tenNV, DateTime birthDate, DateTime hireDate,
                                    bool isMale, string homeTown, string address, string citizenID, DateTime? issueDate, string issuePlace,
                                    bool isActive, bool canCreateUserName, string phone, string noteResign, bool isInsuranceRefund, int salaryGradeID)
        {
            foreach (DataRow row in mEmployees_dt.Rows)
            {
                int id = Convert.ToInt32(row["EmployeeID"]);
                if (id.CompareTo(employeeId) == 0)
                {
                    bool isActiveCurrent = Convert.ToBoolean(row["IsActive"]);
                    string noteResignUpdate = noteResign;
                    if (isActiveCurrent != isActive)
                    {
                        noteResignUpdate += DateTime.Now.ToString("MM/yyyy");
                        noteResignUpdate += isActive == true ? "(Vào); " : "(Ra);";
                    }
                    DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogResult == DialogResult.Yes)
                    {
                        string gradeName = mSalaryGrade_dt.Select($"SalaryGradeID = {salaryGradeID}")[0]["GradeName"].ToString();

                        string oldValueLog = $"{row["FullName"]} - {row["BirthDate"]} - {row["HireDate"]} - {row["Gender"]} - " +
                            $"{row["Hometown"]} - {row["Address"]} - {row["CitizenID"]} - {row["IssueDate"]} - {row["IssuePlace"]} - " +
                            $"{row["IsActive"]} - {row["PhoneNumber"]} - {row["NoteResign"]} - {row["canCreateUserName"]} - " +
                            $"{row["IsInsuranceRefund"]} - {row["GradeName"]}";

                        string newValueLog = $"{tenNV} - {birthDate} - {hireDate} - {isMale} - " +
                            $"{homeTown} - {address} - {citizenID} - {issueDate} - {issuePlace} - " +
                            $"{isActive} - {phone} - {noteResignUpdate} - {canCreateUserName} - " +
                            $"{isInsuranceRefund} - {gradeName}";


                        try
                        {
                            bool isScussess = await SQLManager_QLNS.Instance.updateEmployeesAsync(employeeId, maNV, tenNV, birthDate, hireDate,
                                    isMale, homeTown, address, citizenID, issueDate, issuePlace, isActive, canCreateUserName, 
                                    phone, noteResignUpdate, isInsuranceRefund, salaryGradeID);

                            
                            if (isScussess == true)
                            {                                
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                _ = SQLManager_QLNS.Instance.InsertEmployeesLogAsync(maNV, oldValueLog, "Edit Success: " + newValueLog);

                                row["EmployeeCode"] = maNV;
                                row["FullName"] = tenNV;
                                row["BirthDate"] = birthDate;
                                row["HireDate"] = hireDate;
                                row["Gender"] = isMale;
                                row["Hometown"] = homeTown;
                                row["Address"] = address;
                                row["CitizenID"] = citizenID;
                                row["IssueDate"] = issueDate;
                                row["IssuePlace"] = issuePlace;
                                row["IsActive"] = isActive;
                                row["PhoneNumber"] = phone;
                                row["NoteResign"] = noteResignUpdate;
                                row["canCreateUserName"] = canCreateUserName;
                                row["IsInsuranceRefund"] = isInsuranceRefund;
                                row["GradeName"] = gradeName;
                                row["EmployessName_NoSign"] = Utils.RemoveVietnameseSigns(maNV + " " + tenNV);
                                int age = DateTime.Now.Year - birthDate.Year;

                                row["GenderName"] = (isMale == true ? "Nam" : "Nữ  ") + " - " + age;


                            }
                            else
                            {
                                _ = SQLManager_QLNS.Instance.InsertEmployeesLogAsync(maNV, oldValueLog, "Edit Fail: " + newValueLog);
                                status_lb.Text = "Thất bại.";
                                status_lb.ForeColor = Color.Red;
                            }
                        }
                        catch (Exception ex)
                        {
                            _ = SQLManager_QLNS.Instance.InsertEmployeesLogAsync(maNV, oldValueLog, "Edit Exception: " + ex .Message +" : "+ newValueLog);
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
                                    bool canCreateUserName, string phone, string noteResign,bool isInsuranceRefund, int salaryGradeID)
        {
            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                var grade = mSalaryGrade_dt.Select($"SalaryGradeID = {salaryGradeID}")[0];
                string gradeName = grade["GradeName"].ToString();

                string newValueLog = $"{tenNV} - {birthDate} - {hireDate} - {isMale} - " +
                            $"{homeTown} - {address} - {citizenID} - {issueDate} - {issuePlace} - " +
                            $"{isActive} - {phone} - {noteResign} - {canCreateUserName} - " +
                            $"{isInsuranceRefund} - {gradeName}";
                try
                {
                    string nvCode_temp = "VR" + 0.ToString("D4");
                    var result = await SQLManager_QLNS.Instance.insertEmployeeAsync(nvCode_temp,tenNV, birthDate, hireDate,isMale, homeTown, address, 
                        citizenID, issueDate, issuePlace, isActive, canCreateUserName, phone, noteResign, isInsuranceRefund, salaryGradeID);
                    if (result.EmployeeID > 0)
                    {
                        DataTable dataTable = (DataTable)dataGV.DataSource;
                        DataRow drToAdd = dataTable.NewRow();

                        int salaryGrade = Convert.ToInt32(grade["Salary"]);
                        string employeeCode = result.EmployeeCode;
                        employeeID_tb.Text = result.EmployeeID.ToString();
                        drToAdd["EmployeeID"] = result.EmployeeID;
                        drToAdd["EmployeeCode"] = employeeCode;
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
                        drToAdd["IsInsuranceRefund"] = isInsuranceRefund;
                        drToAdd["GradeName"] = gradeName;
                        drToAdd["SalaryGrade"] = salaryGrade;
                        drToAdd["EmployessName_NoSign"] = Utils.RemoveVietnameseSigns(employeeCode + " " + tenNV);
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

                        
                       int id = await SQLManager_QLNS.Instance.insertEmployeeSalaryInfoAsync(employeeCode, DateTime.Now.Month, DateTime.Now.Year, salaryGrade, salaryGrade, "Theo Bậc Lương");
                       await SQLStore_QLNS.Instance.UpdateEmployeeSalaryInfo(id, employeeCode, DateTime.Now.Month, DateTime.Now.Year, salaryGrade, salaryGrade, "Theo Bậc Lương");

                        _ = SQLManager_QLNS.Instance.InsertEmployeesLogAsync(employeeCode, newValueLog, "Create Success");
                        newCustomerBtn_Click(null, null);
                    }
                    else
                    {
                        status_lb.Text = "Thất bại";
                        status_lb.ForeColor = Color.Red;
                    }
                }
                catch
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

            if ((string.IsNullOrEmpty(nvCode_tb.Text) && !string.IsNullOrEmpty(employeeID_tb.Text)) || salaryGrade_ccb.SelectedItem == null ||
                tenNV_tb.Text.CompareTo("") == 0)
            {
                MessageBox.Show("Sai Dữ Liệu, Kiểm Tra Lại!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (DateTime.Now.Year - birthDate.Year < 18)
            {
                MessageBox.Show("Nhân Viên Chưa Đủ 18t, Kiểm Tra Lại!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            bool isActive = isActive_cb.Checked;
            bool canCreateUserName = canCreateUserName_cb.Checked;
            bool isInsuranceRefund = isInsuranceRefund_CB.Checked;
            int salaryGradeID = Convert.ToInt32(salaryGrade_ccb.SelectedValue);

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
                    address, citizenID, issueDate, issuePlace, isActive, canCreateUserName, phone, noteResign, isInsuranceRefund, salaryGradeID);
            else
                createNew(tenNV, birthDate, hireDate,isMale, homeTown, 
                    address, citizenID, issueDate, issuePlace, isActive, canCreateUserName, phone, noteResign, isInsuranceRefund, salaryGradeID);

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
                            bool isScussess = await SQLManager_QLNS.Instance.deleteEmployeeAsync(Convert.ToInt32(employeeID));

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
                        catch
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
            info_gb.BackColor = Color.Green;

            nvCode_tb.Focus();
            info_gb.BackColor = newCustomerBtn.BackColor;
            edit_btn.Visible = false;
            newCustomerBtn.Visible = false;
            readOnly_btn.Visible = true;
            LuuThayDoiBtn.Visible = true;
            delete_btn.Visible = false;
            isNewState = true;
            LuuThayDoiBtn.Text = "Lưu Mới";
            SetUIReadOnly(false);
        }

        private void ReadOnly_btn_Click(object sender, EventArgs e)
        {
            edit_btn.Visible = true;
            newCustomerBtn.Visible = true;
            readOnly_btn.Visible = false;
            LuuThayDoiBtn.Visible = false;
            delete_btn.Visible = false;
            info_gb.BackColor = Color.DarkGray;
            isNewState = false;
            SetUIReadOnly(true);
            dataGV_CellClick(null, null);
        }

        private void Edit_btn_Click(object sender, EventArgs e)
        {
            edit_btn.Visible = false;
            newCustomerBtn.Visible = false;
            readOnly_btn.Visible = true;
            LuuThayDoiBtn.Visible = true;
            delete_btn.Visible = true;
            info_gb.BackColor = edit_btn.BackColor;
            isNewState = false;
            LuuThayDoiBtn.Text = "Lưu C.Sửa";
            SetUIReadOnly(false);
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

        private void SetUIReadOnly(bool isReadOnly)
        {
            nvCode_tb.ReadOnly = true;
            tenNV_tb.ReadOnly = isReadOnly;
            birthdate_dtp.Enabled = !isReadOnly;
            hireDate_dtp.Enabled = !isReadOnly;
            gender_cb.Enabled = !isReadOnly;
            hometown_tb.ReadOnly = isReadOnly;
            address_tb.ReadOnly = isReadOnly;
            citizenId_tb.ReadOnly = isReadOnly;
            issueDate_dtp.Enabled = !isReadOnly;
            salaryGrade_ccb.Enabled = !isReadOnly;
            issuePlace_tb.ReadOnly = isReadOnly;
            phone_tb.ReadOnly = isReadOnly;
            isActive_cb.Enabled = !isReadOnly;
            canCreateUserName_cb.Enabled = !isReadOnly;
            isInsuranceRefund_CB.Enabled = !isReadOnly;
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
    }
}
