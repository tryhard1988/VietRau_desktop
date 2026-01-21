using System;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using RauViet.classes;

namespace RauViet.ui
{
    public partial class Customers : Form
    {
        bool isNewState = false;
        public Customers()
        {
            InitializeComponent();

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            status_lb.Text = "";
            delete_btn.Enabled = false;

            newCustomerBtn.Click += newCustomerBtn_Click;
            LuuThayDoiBtn.Click += saveBtn_Click;
            delete_btn.Click += deleteBtn_Click;
            dataGV.SelectionChanged += this.dataGV_CellClick;
            this.dataGV.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.dataGV_RowPrePaint);

            edit_btn.Click += Edit_btn_Click;
            readOnly_btn.Click += ReadOnly_btn_Click;            
        }

        public async void ShowData()
        {
            await Task.Delay(50);
            LoadingOverlay loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Show();
            await Task.Delay(200);

            try
            {
                // Chạy truy vấn trên thread riêng
                DataTable dt = await SQLStore_Kho.Instance.getCustomersAsync();
                dataGV.DataSource = dt;

                Utils.SetGridHeaders(dataGV, new System.Collections.Generic.Dictionary<string, string> {
                    {"CustomerID", "Mã KH" },
                    {"FullName", "Tên Khách Hàng" },
                    {"CustomerCode", "Tên In Trên Thùng" },
                    {"Home", "Nhóm" },
                    {"Company", "Công Ty" },
                    {"Address", "Địa Chỉ" }
                });

                dataGV.Columns["Home"].Width = 50;
                dataGV.Columns["Priority"].Width = 50;
                dataGV.Columns["Address"].Width = 150;
                dataGV.Columns["CustomerID"].Visible = false;
                dataGV.Columns["FullName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGV.Columns["CustomerCode"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                ReadOnly_btn_Click(null, null);
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

        private void dataGV_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (e.RowIndex % 2 == 0)
            {
                dataGV.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Beige;
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
            if (isNewState) return;
            var cells = dataGV.Rows[index].Cells;
            string maKH = cells["CustomerID"].Value.ToString();
            string fullName = cells["FullName"].Value.ToString();
            string customerCode = cells["CustomerCode"].Value.ToString();
            int priority = Convert.ToInt32(cells["Priority"].Value);
            string home = Convert.ToString(cells["Home"].Value);
            string company = Convert.ToString(cells["Company"].Value);
            string address = Convert.ToString(cells["Address"].Value);
            string email = Convert.ToString(cells["Email"].Value);
            string taxCode = Convert.ToString(cells["TaxCode"].Value);
            name_tb.Text = fullName;
            customerCode_tb.Text = customerCode;
            priority_tb.Text = priority.ToString();
            maKH_tb.Text = maKH;
            home_tb.Text = home;
            company_tb.Text = company;
            address_tb.Text = address;
            email_tb.Text = email;
            taxCode_tb.Text = taxCode;
            delete_btn.Enabled = true;

            status_lb.Text = "";
        }

        private async void updateData(int customerId, string fullName, string code, int priority, string home, string company, string address, string email, string taxCode)
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
                            bool isScussess = await SQLManager_Kho.Instance.updateCustomerAsync(customerId, fullName, code, priority, home, company, address, email, taxCode);

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                row.Cells["FullName"].Value = fullName;
                                row.Cells["CustomerCode"].Value = code;
                                row.Cells["Priority"].Value = priority;
                                row.Cells["Home"].Value = home;
                                row.Cells["Company"].Value = company;
                                row.Cells["Address"].Value = address;
                                row.Cells["Email"].Value = email;
                                row.Cells["TaxCode"].Value = taxCode;
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

        private async void createNew(string fullName, string code, int priority, string home, string company, string address, string email, string taxCode)
        {
            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    int newId = await SQLManager_Kho.Instance.insertCustomerAsync(fullName, code, priority, home, company, address, email, taxCode);
                    if (newId > 0 )
                    {

                        DataTable dataTable = (DataTable)dataGV.DataSource;
                        DataRow drToAdd = dataTable.NewRow();

                        drToAdd["CustomerID"] = newId;
                        drToAdd["FullName"] = fullName;
                        drToAdd["CustomerCode"] = code;
                        drToAdd["Priority"] = priority;
                        drToAdd["Home"] = home;
                        drToAdd["Company"] = company;
                        drToAdd["Address"] = address;
                        drToAdd["Email"] = email;
                        drToAdd["TaxCode"] = taxCode;
                        maKH_tb.Text = newId.ToString();


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
                catch
                {
                    status_lb.Text = "Thất bại.";
                    status_lb.ForeColor = Color.Red;
                }
                
            }
        }
        private void saveBtn_Click(object sender, EventArgs e)
        {
            if (name_tb.Text.CompareTo("") == 0 || customerCode_tb.Text.CompareTo("") == 0 || priority_tb.Text.CompareTo("") == 0)
            {
                MessageBox.Show(
                                "Thiếu Dữ Liệu, Kiểm Tra Lại!",
                                "Thông Báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning
                            );
                return;
            }

            string name = name_tb.Text;
            string code = customerCode_tb.Text;
            string home = home_tb.Text;
            string company = company_tb.Text;
            string address = address_tb.Text;
            string email = email_tb.Text;
            string taxCode = taxCode_tb.Text;
            int priority = Convert.ToInt32(priority_tb.Text);

            if (code.CompareTo("") == 0)
                code = name;

            if (maKH_tb.Text.Length != 0)
                updateData(Convert.ToInt32(maKH_tb.Text), name, code, priority, home, company, address, email, taxCode);
            else
                createNew(name, code, priority, home, company, address, email, taxCode);

        }
        private async void deleteBtn_Click(object sender, EventArgs e)
        {
            if (dataGV.SelectedRows.Count == 0) return;

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
                            bool isScussess = await SQLManager_Kho.Instance.deleteCustomerAsync(Convert.ToInt32(customerId));

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
            maKH_tb.Text = "";
            name_tb.Text = "";
            customerCode_tb.Text = "";
            status_lb.Text = "";
            delete_btn.Enabled = false;
            info_gb.BackColor = Color.Green;

            name_tb.Focus();
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

            if (dataGV.Rows.Count > 0)
            {
                UpdateRightUI(0);
            }

            customerCode_tb.Enabled = true;
            name_tb.Enabled = true;
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

            customerCode_tb.Enabled = false;
            name_tb.Enabled = false;
        }

        private void SetUIReadOnly(bool isReadOnly)
        {
            name_tb.ReadOnly = isReadOnly;
            customerCode_tb.ReadOnly = isReadOnly;
            priority_tb.ReadOnly = isReadOnly;
        }
    }
}
