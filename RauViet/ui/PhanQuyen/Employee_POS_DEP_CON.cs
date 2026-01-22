using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using RauViet.classes;

namespace RauViet.ui
{    
    public partial class Employee_POS_DEP_CON : Form
    {
        private DataTable mDepartment_dt, mPosition_dt, mContractType_dt, mEmployees_dt;
        private DataView mLogDV;
        public Employee_POS_DEP_CON()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            status_lb.Text = "";

            LuuThayDoiBtn.Click += saveBtn_Click;
            dataGV.SelectionChanged += this.dataGV_CellClick;

            edit_btn.Click += Edit_btn_Click;
            readOnly_btn.Click += ReadOnly_btn_Click;
            ReadOnly_btn_Click(null, null);
            this.KeyDown += Employee_POS_DEP_CON_KeyDown;
            this.FormClosing += Employee_POS_DEP_CON_FormClosing;
            search_tb.TextChanged += Search_tb_TextChanged;
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
                SQLStore_QLNS.Instance.removeEmployees();
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
                // Chạy truy vấn trên thread riêng
                string[] keepColumns = { "EmployeeCode", "FullName", "HireDate", "PositionID", "DepartmentID", "ContractTypeID", "PositionName", "DepartmentName", "ContractTypeName", "EmployessName_NoSign" };
                var employeesTask = SQLStore_QLNS.Instance.GetEmployeesAsync(keepColumns);
                var departmentTask = SQLStore_QLNS.Instance.GetActiveDepartmentAsync();
                var positionTask = SQLStore_QLNS.Instance.GetActivePositionAsync();
                var contractTypeTask = SQLStore_QLNS.Instance.GetContractTypeAsync();
                var employee_POS_DEP_CON_LogTask = SQLStore_QLNS.Instance.GetEmployee_POS_DEP_CON_LogAsync();
                await Task.WhenAll(employeesTask, departmentTask, positionTask, contractTypeTask, employee_POS_DEP_CON_LogTask);
                mEmployees_dt = employeesTask.Result;
                mDepartment_dt = departmentTask.Result;
                mPosition_dt = positionTask.Result;
                mContractType_dt = contractTypeTask.Result;
                mLogDV = new DataView(employee_POS_DEP_CON_LogTask.Result);

                log_GV.DataSource = mLogDV;
                Utils.HideColumns(log_GV, new[] { "LogID", "EmployeeCode" });
                department_cbb.DataSource = mDepartment_dt;
                department_cbb.DisplayMember = "DepartmentName";
                department_cbb.ValueMember = "DepartmentID";

                position_cbb.DataSource = mPosition_dt;
                position_cbb.DisplayMember = "PositionName";
                position_cbb.ValueMember = "PositionID";

                contractType_cbb.DataSource = mContractType_dt;
                contractType_cbb.DisplayMember = "ContractTypeName";
                contractType_cbb.ValueMember = "ContractTypeID";

                int count = 0;
                mEmployees_dt.Columns["EmployeeCode"].SetOrdinal(count++);
                mEmployees_dt.Columns["FullName"].SetOrdinal(count++);                
                mEmployees_dt.Columns["HireDate"].SetOrdinal(count++);
                mEmployees_dt.Columns["DepartmentName"].SetOrdinal(count++);
                mEmployees_dt.Columns["PositionName"].SetOrdinal(count++);
                mEmployees_dt.Columns["ContractTypeName"].SetOrdinal(count++);        
                

                dataGV.DataSource = mEmployees_dt;
                Utils.HideColumns(dataGV, new[] { "EmployessName_NoSign", "PositionID", "DepartmentID", "ContractTypeID" });
               
                Utils.SetGridHeaders(dataGV, new System.Collections.Generic.Dictionary<string, string> {
                    {"EmployeeCode", "Mã NV" },
                    {"FullName", "Họ Và Tên" },
                    {"HireDate", "Ngày Vào Làm" },
                    {"PositionName", "Chức Vụ" },
                    {"DepartmentName", "Phòng Ban" },
                    {"ContractTypeName", "Loại Hợp Đồng" }
                });

                Utils.SetGridHeaders(log_GV, new System.Collections.Generic.Dictionary<string, string> {
                    {"CreatedAt", "Thời điểm thay đổi" },
                    {"ACtionBy", "Người thay đổi" },
                    {"OldValue", "Giá trị cũ" },
                    {"NewValue", "Giá trị mới" }
                });

                dataGV.Columns["EmployeeCode"].Width = 50;
                dataGV.Columns["FullName"].Width = 160;
                dataGV.Columns["HireDate"].Width = 70;
                dataGV.Columns["PositionName"].Width = 100;
                dataGV.Columns["DepartmentName"].Width = 120;
                dataGV.Columns["ContractTypeName"].Width = 90;

                log_GV.Columns["CreatedAt"].Width = 120;
                log_GV.Columns["ACtionBy"].Width = 150;
                log_GV.Columns["OldValue"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                log_GV.Columns["NewValue"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                log_GV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                if (dataGV.Rows.Count > 0)
                {
                    dataGV.ClearSelection();
                    dataGV.Rows[0].Selected = true;
                    UpdateRightUI(0);
                }


                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
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
            string employeeCode = cells["EmployeeCode"].Value.ToString();
            string fullName = cells["FullName"].Value.ToString();
            DateTime hireDate = Convert.ToDateTime(cells["HireDate"].Value);

            employeeCode_tb.Text = employeeCode.ToString();
            Utils.SafeSelectValue(position_cbb, positionID);
            Utils.SafeSelectValue(department_cbb, departmentID);
            Utils.SafeSelectValue(contractType_cbb, contractTypeID);
            status_lb.Text = "";

            mLogDV.RowFilter = $"EmployeeCode = '{employeeCode}'";
        }
        
        private async void updateData(string maNV, int department, int position, int contractType)
        {
            foreach (DataGridViewRow row in dataGV.Rows)
            {
                string id = Convert.ToString(row.Cells["EmployeeCode"].Value);
                if (id.CompareTo(maNV) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogResult == DialogResult.Yes)
                    {
                        string positionName = mPosition_dt.Select($"PositionID = {position}")[0]["PositionName"].ToString();
                        string departmentName = mDepartment_dt.Select($"DepartmentID = {department}")[0]["DepartmentName"].ToString();
                        string contractTypeName = mContractType_dt.Select($"ContractTypeID = {contractType}")[0]["ContractTypeName"].ToString();
                        string positionCode = mPosition_dt.Select($"PositionID = {position}")[0]["PositionCode"].ToString();
                        string contractTypeCode = mContractType_dt.Select($"ContractTypeID = {contractType}")[0]["ContractTypeCode"].ToString();
                        string oldLog = $"{row.Cells["PositionName"].Value} - {row.Cells["DepartmentName"].Value} - {row.Cells["ContractTypeName"].Value}";
                        string newLog = $"{positionName} - {departmentName} - {contractTypeName}";
                        try
                        {
                            bool isScussess = await SQLManager_QLNS.Instance.updateEmployeeWorkAsync(maNV, position, department, contractType);
                            
                            if (isScussess == true)
                            {
                                newLog = "Success: " + newLog;
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                row.Cells["EmployeeCode"].Value = maNV;
                                row.Cells["PositionID"].Value = position;
                                row.Cells["DepartmentID"].Value = department;
                                row.Cells["ContractTypeID"].Value = contractType;

                                row.Cells["PositionName"].Value = positionName;
                                row.Cells["DepartmentName"].Value = departmentName;
                                row.Cells["ContractTypeName"].Value = contractTypeName;

                                var parameters = new Dictionary<string, object>
                                {
                                    ["PositionID"] = position,
                                    ["DepartmentID"] = department,
                                    ["ContractTypeID"] = contractType,

                                    ["PositionName"] = positionName,
                                    ["DepartmentName"] = departmentName,
                                    ["ContractTypeName"] = contractTypeName,

                                    ["PositionCode"] = positionCode,
                                    ["ContractTypeCode"] = contractTypeCode
                                };
                                SQLStore_QLNS.Instance.updateEmploy(maNV, parameters);
                                SQLStore_QLNS.Instance.removeAnnualLeaveBalance();

                            }
                            else
                            {
                                newLog = "Fail: " + newLog;
                                status_lb.Text = "Thất bại.";
                                status_lb.ForeColor = Color.Red;
                            }
                        }
                        catch (Exception ex)
                        {
                            newLog = "Fail: " + ex.Message + ": " + newLog;
                            status_lb.Text = "Thất bại.";
                            status_lb.ForeColor = Color.Red;
                        }

                        _ = SQLManager_QLNS.Instance.InsertEmployees_POS_DEP_CON_LogAsync(maNV, oldLog, newLog);
                    }
                    break;
                }
            }
        }


        private void saveBtn_Click(object sender, EventArgs e)
        {

            if (department_cbb.SelectedItem == null || position_cbb.SelectedItem == null)
            {
                MessageBox.Show("Sai Dữ Liệu, Kiểm Tra Lại!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int department = Convert.ToInt32(department_cbb.SelectedValue);
            int position = Convert.ToInt32(position_cbb.SelectedValue);
            int contractType = Convert.ToInt32(contractType_cbb.SelectedValue);


            if (employeeCode_tb.Text.Length != 0)
                updateData(employeeCode_tb.Text, department,position, contractType);
        }

        private void ReadOnly_btn_Click(object sender, EventArgs e)
        {
            edit_btn.Visible = true;
            readOnly_btn.Visible = false;
            LuuThayDoiBtn.Visible = false;
            info_gb.BackColor = Color.DarkGray;
            SetUIReadOnly(true);
            dataGV_CellClick(null, null);
        }

        private void Edit_btn_Click(object sender, EventArgs e)
        {
            edit_btn.Visible = false;
            readOnly_btn.Visible = true;
            LuuThayDoiBtn.Visible = true;
            info_gb.BackColor = edit_btn.BackColor;
            LuuThayDoiBtn.Text = "Lưu C.Sửa";
            SetUIReadOnly(false);
        }

        private void SetUIReadOnly(bool isReadOnly)
        {
            department_cbb.Enabled = !isReadOnly;
            position_cbb.Enabled = !isReadOnly;
            contractType_cbb.Enabled = !isReadOnly;
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
