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
    public partial class User : Form
    {
        DataTable mEmployees_dt;
        public User()
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
                var userTask = SQLManager.Instance.GetUserDataAsync();
                var employeesTask = SQLManager.Instance.GetActiveEmployees_For_CreateUserAsync();
                var rolesTask = SQLManager.Instance.GetRolesAsync();

                await Task.WhenAll(userTask, employeesTask, rolesTask);
                DataTable user_dt = userTask.Result;
                DataTable role_dt = rolesTask.Result;
                mEmployees_dt = employeesTask.Result;

                user_dt.Columns.Add("EmployeeCode", typeof(string));
                user_dt.Columns.Add("FullName", typeof(string));
                foreach (DataRow dr in user_dt.Rows)
                {
                    Console.WriteLine($"EmployeeCode ReadOnly: {user_dt.Columns["EmployeeCode"].ReadOnly}");

                    int employeeID = Convert.ToInt32(dr["EmployeeID"]);

                    var Employees = mEmployees_dt.Select($"EmployeeID = {employeeID}");
                    if (Employees.Length > 0)
                    {
                        dr["EmployeeCode"] = Employees[0]["EmployeeCode"].ToString();
                        dr["FullName"] = Employees[0]["FullName"].ToString();
                    }
                }

                user_dt.Columns["RoleIDs"].ReadOnly = false;

                int count = 0;
                user_dt.Columns["EmployeeCode"].SetOrdinal(count++);
                user_dt.Columns["FullName"].SetOrdinal(count++);
                user_dt.Columns["Username"].SetOrdinal(count++);
                user_dt.Columns["PasswordHash"].SetOrdinal(count++);
                user_dt.Columns["IsActive"].SetOrdinal(count++);

                dataGV.DataSource = user_dt;
                dataGV.Columns["UserID"].Visible = false;
                dataGV.Columns["EmployeeID"].Visible = false;
                dataGV.Columns["RoleIDs"].Visible = false;

                dataGV.Columns["Username"].HeaderText = "Tên Đăng Nhập";
                dataGV.Columns["PasswordHash"].HeaderText = "Mật Khẩu";
                dataGV.Columns["EmployeeCode"].HeaderText = "Mã NV";
                dataGV.Columns["FullName"].HeaderText = "Tên NV";
                dataGV.Columns["IsActive"].HeaderText = "Còn Hoạt Động";

                dataGV.Columns["Username"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGV.Columns["PasswordHash"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGV.Columns["EmployeeCode"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGV.Columns["FullName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

                // dataGV.Columns["UserID"].Visible = false;

                employee_cbb.DataSource = mEmployees_dt;
                employee_cbb.DisplayMember = "FullName";  // hiển thị tên
                employee_cbb.ValueMember = "EmployeeID";

                phanquyen_clb.CheckOnClick = true;
                phanquyen_clb.DataSource = role_dt;
                phanquyen_clb.DisplayMember = "RoleName";  // Hiển thị tên vai trò
                phanquyen_clb.ValueMember = "RoleID";

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
            if (dataGV.CurrentRow == null)
                return;

            int rowIndex = dataGV.CurrentRow.Index;
            if (rowIndex < 0)
                return;


            UpdateRightUI(rowIndex);
        }

        private void UpdateRightUI(int index)
        {
            var cells = dataGV.Rows[index].Cells;
            string userID = cells["UserID"].Value.ToString();
            string username = cells["Username"].Value.ToString();
            string passwordHash = cells["PasswordHash"].Value.ToString();
            int employeeID = Convert.ToInt32(cells["EmployeeID"].Value);
            Boolean isActive = Convert.ToBoolean(cells["IsActive"].Value);
            string roleIDs = cells["RoleIDs"].Value.ToString();
            List<int> selectedRoleIDs = roleIDs.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                .Select(id => int.Parse(id.Trim())).ToList();

            userID_tb.Text = userID;
            password_tb.Text = passwordHash;
            username_tb.Text = username;
            employee_cbb.SelectedValue = employeeID;
            isActive_cb.Checked = isActive;

            for (int i = 0; i < phanquyen_clb.Items.Count; i++)
                phanquyen_clb.SetItemChecked(i, false);

            for (int i = 0; i < phanquyen_clb.Items.Count; i++)
            {
                DataRowView item = (DataRowView)phanquyen_clb.Items[i];
                int roleID = Convert.ToInt32(item["RoleID"]);

                if (selectedRoleIDs.Contains(roleID))
                {
                    phanquyen_clb.SetItemChecked(i, true);
                }
            }

            delete_btn.Enabled = true;
            info_gb.BackColor = Color.DarkGray;
            status_lb.Text = "";
        }

        private async void updateData(int userID, string userName, string password, int employeeID, Boolean isActive, List<int> roleIDs)
        {
            foreach (DataGridViewRow row in dataGV.Rows)
            {
                int id = Convert.ToInt32(row.Cells["UserID"].Value);
                if (id.CompareTo(userID) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            string passDb = row.Cells["PasswordHash"].Value.ToString();
                            Boolean isupdatePass = passDb.CompareTo(password) == 0 ? false : true;

                            string passworshHash = password;
                            bool isScussess = false;
                            if (isupdatePass)
                            {
                                passworshHash = Utils.HashPassword(password);
                                isScussess = await SQLManager.Instance.updateUserAsync(userID, userName, passworshHash, employeeID, isActive, roleIDs);
                            }
                            else
                                isScussess = await SQLManager.Instance.updateUser_notPasswordAsync(userID, userName, employeeID, isActive, roleIDs);

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;
                                string roleIDsString = string.Join(",", roleIDs);

                                row.Cells["Username"].Value = userName;
                                row.Cells["PasswordHash"].Value = passworshHash;
                                row.Cells["EmployeeID"].Value = employeeID;
                                row.Cells["IsActive"].Value = isActive;
                                row.Cells["RoleIDs"].Value = roleIDsString;



                                var Employees = mEmployees_dt.Select($"EmployeeID = {employeeID}");
                                if (Employees.Length > 0)
                                {
                                    row.Cells["EmployeeCode"].Value = Employees[0]["EmployeeCode"].ToString();
                                    row.Cells["FullName"].Value = Employees[0]["FullName"].ToString();
                                }
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

        private async void createNew(string userName, string password, int employeeID, Boolean isActive, List<int> roleIDs)
        {
            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    string passworshHash = Utils.HashPassword(password);
                    int newId = await SQLManager.Instance.insertUserDataAsync(userName, passworshHash, employeeID, isActive, roleIDs);
                    if (newId > 0)
                    {

                        DataTable dataTable = (DataTable)dataGV.DataSource;
                        DataRow drToAdd = dataTable.NewRow();

                        drToAdd["UserID"] = newId;
                        drToAdd["Username"] = userName;
                        drToAdd["PasswordHash"] = passworshHash;
                        drToAdd["EmployeeID"] = employeeID;
                        drToAdd["IsActive"] = isActive;
                        userID_tb.Text = newId.ToString();

                        var Employees = mEmployees_dt.Select($"EmployeeID = {employeeID}");
                        if (Employees.Length > 0)
                        {
                            drToAdd["EmployeeCode"] = Employees[0]["EmployeeCode"].ToString();
                            drToAdd["FullName"] = Employees[0]["FullName"].ToString();
                        }


                        dataTable.Rows.Add(drToAdd);
                        dataTable.AcceptChanges();

                        dataGV.ClearSelection(); // bỏ chọn row cũ
                        int rowIndex = dataGV.Rows.Count - 1;
                        dataGV.Rows[rowIndex].Selected = true;
                        UpdateRightUI(dataGV.Rows.Count - 1);
                        

                        status_lb.Text = "Thành công";
                        status_lb.ForeColor = Color.Green;
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
            var checkedIDs = phanquyen_clb.CheckedItems
                              .Cast<DataRowView>()
                              .Select(item => Convert.ToInt32(item["RoleID"]))
                              .ToList();

            if (username_tb.Text.CompareTo("") == 0 || password_tb.Text.CompareTo("") == 0 || checkedIDs.Count == 0)
            {
                MessageBox.Show(
                                "Thiếu Dữ Liệu, Kiểm Tra Lại!",
                                "Thông Báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning
                            );
                return;
            }

            string username = username_tb.Text;
            string password = password_tb.Text;
            int employeeID = Convert.ToInt32(employee_cbb.SelectedValue);
            Boolean isActive = isActive_cb.Checked;

            if (userID_tb.Text.Length != 0)
                updateData(Convert.ToInt32(userID_tb.Text), username, password, employeeID, isActive, checkedIDs);
            else
                createNew(username, password, employeeID, isActive, checkedIDs);

        }
        private async void deleteBtn_Click(object sender, EventArgs e)
        {
            if (dataGV.SelectedRows.Count == 0) return;

            string id = userID_tb.Text;

            foreach (DataGridViewRow row in dataGV.Rows)
            {
                string userID = row.Cells["UserID"].Value.ToString();
                if (userID.CompareTo(id) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("XÓA THÔNG TIN ĐÓ NHA \n Chắc chắn chưa ?", " Xóa Thông Tin", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager.Instance.deleteUserAsync(Convert.ToInt32(userID));

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
            userID_tb.Text = "";
            username_tb.Text = "";
            password_tb.Text = "";
            isActive_cb.Checked = true;
            status_lb.Text = "";
            delete_btn.Enabled = false;
            info_gb.BackColor = Color.Green;

            for (int i = 0; i < phanquyen_clb.Items.Count; i++)
                phanquyen_clb.SetItemChecked(i, false);
            return;            
        }
    }
}
