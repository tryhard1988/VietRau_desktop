using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using RauViet.classes;
using System;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RauViet.ui
{
    public partial class Supplier : Form
    {
        DataTable mSupplier_dt;
        bool isNewState = false;
        public Supplier()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            Utils.SetTabStopRecursive(this, false);

            int countTab = 0;
            SupplierName_tb.TabIndex = countTab++; SupplierName_tb.TabStop = true;
            Phone_tb.TabIndex = countTab++; Phone_tb.TabStop = true;
            LuuThayDoiBtn.TabIndex = countTab++; LuuThayDoiBtn.TabStop = true;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            status_lb.Text = "";

            newCustomerBtn.Click += newCustomerBtn_Click;
            LuuThayDoiBtn.Click += saveBtn_Click;
            dataGV.SelectionChanged += this.dataGV_CellClick;

            edit_btn.Click += Edit_btn_Click;
            readOnly_btn.Click += ReadOnly_btn_Click;
            ReadOnly_btn_Click(null, null);

            this.KeyDown += Department_KeyDown; ;
        }

        private void Department_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                SQLStore_Kho.Instance.removeSupplier();
                ShowData();
            }
            else if (!isNewState && !edit_btn.Visible)
            {
                if (e.KeyCode == Keys.Delete)
                {
                    Control ctrl = this.ActiveControl;

                    if (ctrl is TextBox || ctrl is RichTextBox ||
                        (ctrl is DataGridView dgv && dgv.CurrentCell != null && dgv.IsCurrentCellInEditMode))
                    {
                        return; // không xử lý Delete
                    }

                    deleteBtn_Click(null, null);
                }
            }
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
                var supplierTask = SQLStore_Kho.Instance.GetSupplierAsync();

                await Task.WhenAll(supplierTask);
                mSupplier_dt = supplierTask.Result;

                dataGV.DataSource = mSupplier_dt;

                Utils.HideColumns(dataGV, new[] {"searching_nosign" });
                Utils.SetGridHeaders(dataGV, new System.Collections.Generic.Dictionary<string, string> {
                    {"SupplierID", "Mã NCC" },
                    {"SupplierName", "Tên Nhà Cung Cấp" },
                    {"Phone", "Số Điện Thoại" },
                    {"Citizen", "Số CCCD" },
                    {"Address", "Địa Chỉ" },
                    {"BankName", "Tên Ngân Hàng" },
                    {"BacnkAccount", "Số TK" }
                });

                Utils.SetGridWidths(dataGV, new System.Collections.Generic.Dictionary<string, int> {
                    {"SupplierID", 70},
                    {"Phone", 130},
                    {"Citizen", 200 },
                    {"Address", 200 },
                    {"BankName", 150 },
                    {"BacnkAccount", 150 }
                });

                Utils.SetGridWidth(dataGV, "SupplierName", DataGridViewAutoSizeColumnMode.Fill);

                if (dataGV.Rows.Count > 0)
                {
                    dataGV.ClearSelection();
                    dataGV.Rows[0].Selected = true;
                    dataGV.CurrentCell = dataGV.Rows[0].Cells[0];
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
            if (isNewState == true) return;

            var cells = dataGV.Rows[index].Cells;
            int supplierID = Convert.ToInt32(cells["SupplierID"].Value);
            string supplierName = cells["SupplierName"].Value.ToString();
            string phone = cells["Phone"].Value.ToString();
            string citizen = cells["Citizen"].Value.ToString();
            string address = cells["Address"].Value.ToString();
            string bankName = cells["BankName"].Value.ToString();
            string bankAccount = cells["BankAccount"].Value.ToString();

            supplierID_tb.Text = supplierID.ToString();
            Phone_tb.Text = phone;
            SupplierName_tb.Text = supplierName;
            citizen_tb.Text = citizen;
            address_tb.Text = address;
            bankName_tb.Text = bankName;
            bankAccount_tb.Text = bankAccount;
            status_lb.Text = "";
        }

        private async void updateData(int supplierID, string supplierName, string phone, string citizen, string address, string bankName, string bankAccount)
        {
            foreach (DataRow row in mSupplier_dt.Rows)
            {
                int id = Convert.ToInt32(row["SupplierID"]);
                if (id.CompareTo(supplierID) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager_Kho.Instance.updateSupplierAsync(supplierID, supplierName, phone, citizen, address, bankName, bankAccount);

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                row["SupplierName"] = supplierName;
                                row["Phone"] = phone;
                                row["Citizen"] = citizen;
                                row["Address"] = address;
                                row["BankName"] = bankName;
                                row["BankAccount"] = bankAccount;
                                row["searching_nosign"] = Utils.RemoveVietnameseSigns(supplierName).ToLower();
                                
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

        private async void createNew(string supplierName, string phone, string citizen, string address, string bankName, string bankAccount)
        {
            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    int supplierID = await SQLManager_Kho.Instance.insertSupplierAsync(supplierName, phone, citizen, address, bankName, bankAccount);
                    if (supplierID > 0)
                    {
                        DataTable dataTable = (DataTable)dataGV.DataSource;
                        DataRow drToAdd = dataTable.NewRow();

                        drToAdd["SupplierID"] = supplierID;
                        drToAdd["SupplierName"] = supplierName;
                        drToAdd["Phone"] = phone;
                        drToAdd["Citizen"] = citizen;
                        drToAdd["Address"] = address;
                        drToAdd["BankName"] = bankName;
                        drToAdd["BankAccount"] = bankAccount;
                        drToAdd["searching_nosign"] = Utils.RemoveVietnameseSigns(supplierName).ToLower();

                        dataTable.Rows.Add(drToAdd);
                        dataTable.AcceptChanges();


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
                catch
                {
                    status_lb.Text = "Thất bại.";
                    status_lb.ForeColor = Color.Red;
                }
                
            }
        }
        private void saveBtn_Click(object sender, EventArgs e)
        {
            if (SupplierName_tb.Text.CompareTo("") == 0)
            {
                MessageBox.Show(
                                "Thiếu Dữ Liệu, Kiểm Tra Lại!",
                                "Thông Báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning
                            );
                return;
            }

            string supplierName = SupplierName_tb.Text;
            string phone = Phone_tb.Text;
            string citizen = citizen_tb.Text;
            string address = address_tb.Text;
            string bankName = bankName_tb.Text;
            string bankAccount = bankAccount_tb.Text;

            if (supplierID_tb.Text.Length != 0)
                updateData(Convert.ToInt32(supplierID_tb.Text), supplierName, phone, citizen, address, bankName, bankAccount);
            else
                createNew(supplierName, phone, citizen, address, bankName, bankAccount);

        }
        private async void deleteBtn_Click(object sender, EventArgs e)
        {
            if (dataGV.SelectedRows.Count == 0) return;

            string supplierID = supplierID_tb.Text;

            foreach (DataGridViewRow row in dataGV.Rows)
            {
                string id = row.Cells["SupplierID"].Value.ToString();
                if (id.CompareTo(supplierID) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("XÓA \n Chắc chắn chưa ?", " Xóa Thông Tin", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager_Kho.Instance.deleteSupplierAsync(Convert.ToInt32(id));

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
            supplierID_tb.Text = "";
            SupplierName_tb.Text = "";
            Phone_tb.Text = "";

            status_lb.Text = "";

            SupplierName_tb.Focus();
            info_gb.BackColor = newCustomerBtn.BackColor;
            edit_btn.Visible = false;
            newCustomerBtn.Visible = false;
            readOnly_btn.Visible = true;
            LuuThayDoiBtn.Visible = true;
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
            info_gb.BackColor = edit_btn.BackColor;
            isNewState = false;
            LuuThayDoiBtn.Text = "Lưu C.Sửa";
            SetUIReadOnly(false);
        }

        private void SetUIReadOnly(bool isReadOnly)
        {
            SupplierName_tb.ReadOnly = isReadOnly;
            Phone_tb.ReadOnly = isReadOnly;
        }
    }
}
