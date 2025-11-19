using DocumentFormat.OpenXml.Vml.Office;
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
using System.Windows.Forms.VisualStyles;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace RauViet.ui
{
    public partial class User : Form
    {
        DataTable mEmployees_dt;
        bool isNewState = false;
        Dictionary<int, CheckBox> mRoles;
        public User()
        {
            InitializeComponent();
            
            mRoles = new Dictionary<int, CheckBox>();
            Utils.SetTabStopRecursive(this, false);

            int countTab = 0;
            username_tb.TabIndex = countTab++; username_tb.TabStop = true;
            password_tb.TabIndex = countTab++; password_tb.TabStop = true;
            employee_cbb.TabIndex = countTab++; employee_cbb.TabStop = true;
            isActive_cb.TabIndex = countTab++; isActive_cb.TabStop = true;
            LuuThayDoiBtn.TabIndex = countTab++; LuuThayDoiBtn.TabStop = true;

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            status_lb.Text = "";
            loading_lb.Text = "Đang tải dữ liệu, vui lòng chờ...";
            loading_lb.Visible = false;

            newCustomerBtn.Click += newCustomerBtn_Click;
            LuuThayDoiBtn.Click += saveBtn_Click;
            delete_btn.Click += deleteBtn_Click;
            dataGV.SelectionChanged += this.dataGV_CellClick;

            edit_btn.Click += Edit_btn_Click;
            readOnly_btn.Click += ReadOnly_btn_Click;
            ReadOnly_btn_Click(null, null);
        }

        public async void ShowData()
        {
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

                user_dt.Columns.Add("FullName", typeof(string));
                foreach (DataRow dr in user_dt.Rows)
                {
                    string employeeCode = Convert.ToString(dr["EmployeeCode"]);

                    var Employees = mEmployees_dt.Select($"EmployeeCode = '{employeeCode}'");
                    if (Employees.Length > 0)
                    {
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
                employee_cbb.ValueMember = "EmployeeCode";

                LoadRolesToGroupBox(role_dt);

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
                Console.WriteLine("ERROR:" + ex.Message);
                status_lb.Text = "Thất bại.";
                status_lb.ForeColor = Color.Red;
            }
            finally
            {
                loading_lb.Visible = false; // ẩn loading
                loading_lb.Enabled = true; // enable lại button
            }
        }

        private void LoadRolesToGroupBox(DataTable role_dt)
        {
            phanquyen_gb.Controls.Clear(); // Xóa nội dung cũ

            // Lấy danh sách nhóm
            var groups = role_dt.AsEnumerable()
                .GroupBy(r => r.Field<string>("RoleGroupName"))
                .OrderBy(g => g.First().Field<int>("RoleGroupID"));

            int yGroup = 50; // vị trí top bắt đầu cho nhóm

            foreach (var g in groups)
            {
                // Tạo label làm tiêu đề nhóm
                Label groupLabel = new Label
                {
                    Text = g.Key,
                    Font = new Font(phanquyen_gb.Font, FontStyle.Bold),
                    AutoSize = true,
                    Location = new Point(10, yGroup)
                };
                phanquyen_gb.Controls.Add(groupLabel);

                yGroup += groupLabel.Height + 5;

                int xCheck = 50; // vị trí left bắt đầu cho checkbox
                int yCheck = yGroup;

                foreach (var row in g)
                {
                    int rolwID = row.Field<int>("RoleID");
                    CheckBox cb = new CheckBox
                    {
                        Text = row.Field<string>("RoleName"),
                        Tag = rolwID, 
                        AutoSize = true,
                        Location = new Point(xCheck, yCheck)
                    };
                    mRoles[rolwID] = cb;
                    phanquyen_gb.Controls.Add(cb);

                    yCheck += cb.Height + 2; // khoảng cách giữa checkbox
                }

                yGroup = yCheck + 10; // cách nhóm tiếp theo
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
            if (isNewState) return;

            var cells = dataGV.Rows[index].Cells;
            string userID = cells["UserID"].Value.ToString();
            string username = cells["Username"].Value.ToString();
            string passwordHash = cells["PasswordHash"].Value.ToString();
            string employeeCode = Convert.ToString(cells["EmployeeCode"].Value);
            Boolean isActive = Convert.ToBoolean(cells["IsActive"].Value);
            string roleIDs = cells["RoleIDs"].Value.ToString();
            List<int> selectedRoleIDs = roleIDs.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                .Select(id => int.Parse(id.Trim())).ToList();

            userID_tb.Text = userID;
            password_tb.Text = passwordHash;
            username_tb.Text = username;
            employee_cbb.SelectedValue = employeeCode;
            isActive_cb.Checked = isActive;

            foreach (var kv in mRoles)
            {
                int roleId = kv.Key;
                CheckBox cb = kv.Value;
                cb.Checked = selectedRoleIDs.Contains(roleId);
            }

            status_lb.Text = "";
        }

        private async void updateData(int userID, string userName, string password, string employeeCode, Boolean isActive, List<int> roleIDs)
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
                                isScussess = await SQLManager.Instance.updateUserAsync(userID, userName, passworshHash, employeeCode, isActive, roleIDs);
                            }
                            else
                                isScussess = await SQLManager.Instance.updateUser_notPasswordAsync(userID, userName, employeeCode, isActive, roleIDs);

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;
                                string roleIDsString = string.Join(",", roleIDs);

                                row.Cells["Username"].Value = userName;
                                row.Cells["PasswordHash"].Value = passworshHash;
                                row.Cells["EmployeeCode"].Value = employeeCode;
                                row.Cells["IsActive"].Value = isActive;
                                row.Cells["RoleIDs"].Value = roleIDsString;



                                var Employees = mEmployees_dt.Select($"EmployeeCode = '{employeeCode}'");
                                if (Employees.Length > 0)
                                {
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

        private async void createNew(string userName, string password, string employeeCode, Boolean isActive, List<int> roleIDs)
        {
            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    string passworshHash = Utils.HashPassword(password);
                    int newId = await SQLManager.Instance.insertUserDataAsync(userName, passworshHash, employeeCode, isActive, roleIDs);
                    if (newId > 0)
                    {

                        DataTable dataTable = (DataTable)dataGV.DataSource;
                        DataRow drToAdd = dataTable.NewRow();

                        drToAdd["UserID"] = newId;
                        drToAdd["Username"] = userName;
                        drToAdd["PasswordHash"] = passworshHash;
                        drToAdd["EmployeeCode"] = employeeCode;
                        drToAdd["IsActive"] = isActive;
                        userID_tb.Text = newId.ToString();

                        var Employees = mEmployees_dt.Select($"EmployeeCode = '{employeeCode}'");
                        if (Employees.Length > 0)
                        {
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
            var checkedIDs = mRoles.Where(kv => kv.Value.Checked).Select(kv => kv.Key).ToList();

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
            string employeeCode = Convert.ToString(employee_cbb.SelectedValue);
            Boolean isActive = isActive_cb.Checked;

            if (userID_tb.Text.Length != 0)
                updateData(Convert.ToInt32(userID_tb.Text), username, password, employeeCode, isActive, checkedIDs);
            else
                createNew(username, password, employeeCode, isActive, checkedIDs);

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

            foreach (var kv in mRoles)
                kv.Value.Checked = false;

            username_tb.Focus();
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

        private void SetUIReadOnly(bool isReadOnly)
        {
            username_tb.ReadOnly = isReadOnly;
            password_tb.ReadOnly = isReadOnly;
            employee_cbb.Enabled = !isReadOnly;
            isActive_cb.Enabled = !isReadOnly;
            phanquyen_gb.Enabled = !isReadOnly;
        }
    }
}
