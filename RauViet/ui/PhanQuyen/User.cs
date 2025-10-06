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
    {        public User()
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
            this.dataGV.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.dataGV_RowPrePaint);
        }

        public async void ShowData()
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            loading_lb.Visible = true;            

            try
            {
                // Chạy truy vấn trên thread riêng
                DataTable dt = await SQLManager.Instance.GetUserData();
                dataGV.DataSource = dt;

                dataGV.Columns["Username"].HeaderText = "Tên Đăng Nhập";
                dataGV.Columns["PasswordHash"].HeaderText = "Mật Khẩu";
                dataGV.Columns["EmployeeCode"].HeaderText = "Mã NV";
                dataGV.Columns["FullName"].HeaderText = "Tên NV";

                dataGV.Columns["Username"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGV.Columns["PasswordHash"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGV.Columns["EmployeeCode"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGV.Columns["FullName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

                dataGV.Columns["UserID"].Visible = false;

                if (dataGV.Rows.Count > 0)
                {
                    dataGV.ClearSelection();
                    dataGV.Rows[0].Selected = true;
                    dataGV.CurrentCell = dataGV.Rows[0].Cells[0];
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

        private void dataGV_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (e.RowIndex % 2 == 0)
            {
                dataGV.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Beige;
            }
        }
        
        private int createID()
        {
            var existingIds = dataGV.Rows
            .Cast<DataGridViewRow>()
            .Where(r => !r.IsNewRow && r.Cells["CustomerID"].Value != null)
            .Select(r => Convert.ToInt32(r.Cells["CustomerID"].Value))
            .ToList();

            int newCustomerID = existingIds.Count > 0 ? existingIds.Max() + 1 : 1;

            return newCustomerID;
        }

        private void dataGV_CellClick(object sender, EventArgs e)
        {
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

            userID_tb.Text = userID;
            password_tb.Text = passwordHash;
            username_tb.Text = username;
            employee_cbb.SelectedValue = employeeID;
            delete_btn.Enabled = true;

            info_gb.BackColor = Color.DarkGray;
            status_lb.Text = "";
        }

        private async void updateData(int customerId, string fullName, string code)
        {
            foreach (DataGridViewRow row in dataGV.Rows)
            {
                int maKH = Convert.ToInt32(row.Cells["CustomerID"].Value);
                if (maKH.CompareTo(customerId) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager.Instance.updateCustomerAsync(customerId, fullName, code);

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                row.Cells["FullName"].Value = fullName;
                                row.Cells["CustomerCode"].Value = code;
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

        private async void createNew(string fullName, string code)
        {
            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    bool isScussess = await SQLManager.Instance.insertCustomerAsync(fullName, code);
                    if (isScussess == true)
                    {

                        DataTable dataTable = (DataTable)dataGV.DataSource;
                        DataRow drToAdd = dataTable.NewRow();

                        int newCustomerID = createID();
                        drToAdd["CustomerID"] = newCustomerID;
                        drToAdd["FullName"] = fullName;
                        drToAdd["CustomerCode"] = code;
                        userID_tb.Text = newCustomerID.ToString();


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
            if (username_tb.Text.CompareTo("") == 0)
            {
                MessageBox.Show(
                                "Thiếu Dữ Liệu, Kiểm Tra Lại!",
                                "Thông Báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning
                            );
                return;
            }

            string name = username_tb.Text;
            string code = password_tb.Text;

            if (code.CompareTo("") == 0)
                code = name;

            if (userID_tb.Text.Length != 0)
                updateData(Convert.ToInt32(userID_tb.Text), name, code);
            else
                createNew(name, code);

        }
        private async void deleteBtn_Click(object sender, EventArgs e)
        {
            if (dataGV.SelectedRows.Count == 0) return;

            string customerId = userID_tb.Text;

            foreach (DataGridViewRow row in dataGV.Rows)
            {
                string maKH = row.Cells["CustomerID"].Value.ToString();
                if (maKH.CompareTo(customerId) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("XÓA THÔNG TIN KHÁCH HÀNG ĐÓ NHA \n Chắc chắn chưa ?", " Xóa Thông Tin Khách Hàng", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager.Instance.deleteCustomerAsync(Convert.ToInt32(customerId));

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
            status_lb.Text = "";
            delete_btn.Enabled = false;
            info_gb.BackColor = Color.Green;
            dataGV.ClearSelection();
            return;            
        }
    }
}
