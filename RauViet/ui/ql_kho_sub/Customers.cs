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
    public partial class Customers : Form
    {        public Customers()
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
            dataGV.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGV_CellClick);
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
                DataTable dt = await Task.Run(() => SQLManager.Instance.getCustomers());
                dataGV.DataSource = dt;

                dataGV.Columns["CustomerID"].HeaderText = "Mã KH";
                dataGV.Columns["FullName"].HeaderText = "Họ và Tên";
                dataGV.Columns["Email"].HeaderText = "Email";
                dataGV.Columns["PhoneNumber"].HeaderText = "Số điện thoại";
                dataGV.Columns["Address"].HeaderText = "Địa chỉ";

                //dataGV.Columns["CustomerID"].Visible = false;
                dataGV.Columns["CustomerID"].Frozen = true;
                dataGV.Columns["FullName"].Frozen = true;
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
        
        private int createCustomerID()
        {
            var existingIds = dataGV.Rows
            .Cast<DataGridViewRow>()
            .Where(r => !r.IsNewRow && r.Cells["CustomerID"].Value != null)
            .Select(r => Convert.ToInt32(r.Cells["CustomerID"].Value))
            .ToList();

            int newCustomerID = existingIds.Count > 0 ? existingIds.Max() + 1 : 1;

            return newCustomerID;
        }

        private void dataGV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;            
            var cells = dataGV.Rows[e.RowIndex].Cells;
            string maKH = cells["CustomerID"].Value.ToString();
            string fullName = cells["FullName"].Value.ToString();
            string address = cells["Address"].Value.ToString();
            string phone = cells["PhoneNumber"].Value.ToString();
            string email = cells["Email"].Value.ToString();

            name_tb.Text = fullName;
            address_tb.Text = address;
            phone_tb.Text = phone;
            email_tb.Text = email;
            maKH_tb.Text = maKH;
            delete_btn.Enabled = true;
        }

       

        private async void updateCustomer(string customerId, string fullName, string address, string phone, string email)
        {
            foreach (DataGridViewRow row in dataGV.Rows)
            {
                string maKH = row.Cells["CustomerID"].Value.ToString();
                if (maKH.CompareTo(customerId) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", " Thay Đổi Thông Tin Khách Hàng", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await Task.Run(() => SQLManager.Instance.updateCustomer(customerId, fullName, address, phone, email));

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                row.Cells["FullName"].Value = name_tb.Text;
                                row.Cells["Address"].Value = address_tb.Text;
                                row.Cells["PhoneNumber"].Value = phone_tb.Text;
                                row.Cells["Email"].Value = email_tb.Text;
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

        private async void createNewCustomer(string fullName, string address, string phone, string email)
        {
            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", " Tạo Mới Thông Tin Khách Hàng", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    bool isScussess = await Task.Run(() => SQLManager.Instance.insertCustomer(fullName, address, phone, email));
                    if (isScussess == true)
                    {

                        DataTable dataTable = (DataTable)dataGV.DataSource;
                        DataRow drToAdd = dataTable.NewRow();

                        int newCustomerID = createCustomerID();
                        drToAdd["CustomerID"] = newCustomerID;
                        drToAdd["FullName"] = name_tb.Text;
                        drToAdd["Address"] = address_tb.Text;
                        drToAdd["PhoneNumber"] = phone_tb.Text;
                        drToAdd["Email"] = email_tb.Text;
                        maKH_tb.Text = newCustomerID.ToString();


                        dataTable.Rows.Add(drToAdd);
                        dataTable.AcceptChanges();

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
            if (maKH_tb.Text.Length != 0)
                updateCustomer(maKH_tb.Text, name_tb.Text, address_tb.Text, phone_tb.Text, email_tb.Text);
            else
                createNewCustomer(name_tb.Text, address_tb.Text, phone_tb.Text, email_tb.Text);

        }
        private async void deleteBtn_Click(object sender, EventArgs e)
        {
            string customerId = maKH_tb.Text;

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
                            bool isScussess = await Task.Run(() => SQLManager.Instance.deleteCustomer(customerId));

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

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




            delete_btn.Enabled = false;
        }

        private void newCustomerBtn_Click(object sender, EventArgs e)
        {
            maKH_tb.Text = "";
            name_tb.Text = "";
            address_tb.Text = "";
            phone_tb.Text = "";
            email_tb.Text = "";
            status_lb.Text = "";
            delete_btn.Enabled = false;
            return;            
        }
    }
}
