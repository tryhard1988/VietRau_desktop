using DocumentFormat.OpenXml.Drawing.Charts;
using RauViet.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataTable = System.Data.DataTable;

namespace RauViet.ui
{
    public partial class InventoryTransaction : Form
    {
        DataTable mSKU_dt, mInventoryTransaction_dt;
        private Timer debounceTimer = new Timer { Interval = 300 };
        bool isNewState = false;
        private LoadingOverlay loadingOverlay;
        public InventoryTransaction()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            transactionType_CB.DisplayMember = "Text";
            transactionType_CB.ValueMember = "Value";
            transactionType_CB.DataSource = new[]
            {
                new { Text = "Nhập kho", Value = "IN" },
                new { Text = "Xuất kho", Value = "OUT" }
            };

            transactionDate_dtp.Format = DateTimePickerFormat.Custom;
            transactionDate_dtp.CustomFormat = "dd/MM/yyyy";

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            status_lb.Text = "";

            newCustomerBtn.Click += newBtn_Click;
            LuuThayDoiBtn.Click += saveBtn_Click;
            dataGV.SelectionChanged += this.dataGV_CellClick;
            quantity_tb.KeyPress += Tb_KeyPress_OnlyNumber;

            debounceTimer.Tick += DebounceTimer_Tick;

            edit_btn.Click += Edit_btn_Click;
            readOnly_btn.Click += ReadOnly_btn_Click;
            this.KeyDown += ProductList_KeyDown;

        }

        private void ProductList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                SQLStore_Kho.Instance.removeInventoryTransaction();
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

                    deleteItemSelected();
                }
            }
        }

        public async void ShowData()
        {
            await Task.Delay(50);
            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Show();

            try
            {
                var parameters = new Dictionary<string, object> { { "IsActive", true } };
                mSKU_dt = await SQLStore_Kho.Instance.getProductSKUAsync(parameters);
                mInventoryTransaction_dt = await SQLStore_Kho.Instance.getInventoryTransactionSync();

                foreach (DataColumn col in mInventoryTransaction_dt.Columns)
                    col.ReadOnly = false;

                sku_cbb.DataSource = mSKU_dt;
                sku_cbb.DisplayMember = "ProductNameVN";  // hiển thị tên
                sku_cbb.ValueMember = "SKU";
                sku_cbb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                sku_cbb.TextUpdate += sku_cbb_TextUpdate;
                                

                dataGV.DataSource = mInventoryTransaction_dt;

                dataGV.Columns["SKU"].Visible = false;
                dataGV.Columns["TransactionType"].Visible = false;
                dataGV.Columns["ProductNameVN_NoSign"].Visible = false;

                dataGV.Columns["TransactionID"].HeaderText = "ID";
                dataGV.Columns["Name_VN"].HeaderText = "Tên Tiếng Việt";
                dataGV.Columns["Package"].HeaderText = "Đ.Vị";
                dataGV.Columns["TransactionTypeName"].HeaderText = "Loại";
                dataGV.Columns["Quantity"].HeaderText = "Số Lượng";
                dataGV.Columns["TransactionDate"].HeaderText = "Ngày";
                dataGV.Columns["Note"].HeaderText = "Ghi Chú";

                dataGV.Columns["TransactionID"].Width = 70;
                dataGV.Columns["Name_VN"].Width = 250;
                dataGV.Columns["Note"].Width = 300;


                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                //    dataGV.Columns["Packing"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                updateRightUI();

                Utils.SetTabStopRecursive(this, false);

                int countTab = 0;                
                sku_cbb.TabIndex = countTab++; sku_cbb.TabStop = true;
                transactionType_CB.TabIndex = countTab++; transactionType_CB.TabStop = true;
                transactionDate_dtp.TabIndex = countTab++; transactionDate_dtp.TabStop = true;
                quantity_tb.TabIndex = countTab++; quantity_tb.TabStop = true;
                note_tb.TabIndex = countTab++; note_tb.TabStop = true;
                LuuThayDoiBtn.TabIndex = countTab++; LuuThayDoiBtn.TabStop = true;

                ReadOnly_btn_Click(null, null);
            }
            catch
            {
                status_lb.Text = "Thất bại.";
                status_lb.ForeColor = Color.Red;
            }
            finally
            {
                await Task.Delay(200);
                loadingOverlay.Hide();
            }
 
        }

        
        private void sku_cbb_TextUpdate(object sender, EventArgs e)
        {
            // Restart timer mỗi khi gõ
            debounceTimer.Stop();
            debounceTimer.Start();
        }

        private void DebounceTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                debounceTimer.Stop();

                string typed = sku_cbb.Text ?? "";
                string plain = Utils.RemoveVietnameseSigns(typed).ToLower();

                // Filter bằng LINQ
                var filtered = mSKU_dt.AsEnumerable()
                    .Where(r => r["ProductNameVN_NoSign"].ToString()
                    .Contains(plain));

                DataTable temp;
                if (filtered.Any())
                    temp = filtered.CopyToDataTable();
                else
                    temp = mSKU_dt.Clone(); // nếu không có kết quả thì trả về table rỗng

                // Gán lại DataSource
                sku_cbb.DataSource = temp;
                sku_cbb.DisplayMember = "ProductNameVN";
                sku_cbb.ValueMember = "SKU";

                // Giữ lại text người đang gõ
                sku_cbb.DroppedDown = true;
                sku_cbb.Text = typed;
                sku_cbb.SelectionStart = typed.Length;
                sku_cbb.SelectionLength = 0;
            }
            catch { }
        }
                
        private void dataGV_CellClick(object sender, EventArgs e)
        {
            updateRightUI();            
        }

        private async void updateItem(int ID, int SKU, string TransactionType, int Quantity, DateTime TransactionDate, string note)
        {
            foreach (DataRow row in mInventoryTransaction_dt.Rows)
            {
                int tranID = Convert.ToInt32(row["TransactionID"]);
                if (tranID.CompareTo(ID) == 0)
                {                    
                    DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogResult == DialogResult.Yes)
                    {
                        DataRowView productSKUData = (DataRowView)sku_cbb.SelectedItem;
                        string newVallueStr = $"Tên SP: {productSKUData["ProductNameVN"]}; Hành động: {transactionType_CB.Text}; số lượng: {Quantity}; ngày: {TransactionDate}; ghi chứ: {note}";
                        string oldVallueStr = $"Tên SP: {row["Name_VN"].ToString()}; Hành động: {row["TransactionType"].ToString()}; số lượng: {row["Quantity"].ToString()}; ngày: {row["TransactionDate"].ToString()}; ghi chứ: {row["Note"].ToString()}";
                        try
                        {      
                            bool isScussess = await SQLManager_Kho.Instance.updateInventoryTransactionAsync(ID, SKU, TransactionType, Quantity, TransactionDate, note);
                            
                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                string package = productSKUData["Package"].ToString();

                                row["SKU"] = SKU;
                                row["TransactionType"] = TransactionType;
                                row["TransactionTypeName"] = transactionType_CB.Text;
                                row["Quantity"] = Quantity;
                                row["Name_VN"] = productSKUData["ProductNameVN"];
                                row["Package"] = productSKUData["Package"];
                                row["ProductNameVN_NoSign"] = Utils.RemoveVietnameseSigns(productSKUData["ProductNameVN"] + " " + SKU).ToLower();
                                row["TransactionDate"] = TransactionDate;
                                row["Note"] = note;

                                _ = SQLManager_Kho.Instance.insertInventoryTransactionLOGAsync(SKU, "edit Sucess: " + newVallueStr, oldVallueStr);
                            }
                            else
                            {
                                status_lb.Text = "Thất bại.";
                                status_lb.ForeColor = Color.Red;

                                _ = SQLManager_Kho.Instance.insertInventoryTransactionLOGAsync(SKU, "edit FAIL: " + newVallueStr, oldVallueStr);
                            }
                        }
                        catch (Exception ex)
                        {
                            status_lb.Text = "Thất bại.";
                            status_lb.ForeColor = Color.Red;

                            _ = SQLManager_Kho.Instance.insertInventoryTransactionLOGAsync(SKU, "edit ERROR: " + newVallueStr, "Exception: " + ex.Message);
                        }  
                    }
                    break;
                }
            }
        }

        private async void createNew(int SKU, string TransactionType, int Quantity, DateTime TransactionDate, string note)
        {
            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                DataRowView productSKUData = (DataRowView)sku_cbb.SelectedItem;

                string newVallueStr = $"Tên SP: {productSKUData["ProductNameVN"]}; Hành động: {transactionType_CB.Text}; số lượng: {Quantity}; ngày: {TransactionDate}; ghi chứ: {note}";
                try
                {
                    int newId = await SQLManager_Kho.Instance.insertInventoryTransactionAsync(SKU, TransactionType, Quantity, TransactionDate, note);
                    
                    if (newId > 0)
                    {
                        
                        DataRow drToAdd = mInventoryTransaction_dt.NewRow();

                        drToAdd["TransactionID"] = newId;
                        drToAdd["SKU"] = SKU;
                        drToAdd["TransactionType"] = TransactionType;
                        drToAdd["TransactionTypeName"] = transactionType_CB.Text;
                        drToAdd["Quantity"] = Quantity;
                        drToAdd["TransactionDate"] = TransactionDate;
                        drToAdd["Note"] = note;
                        drToAdd["Name_VN"] = productSKUData["ProductNameVN"];
                        drToAdd["Package"] = productSKUData["Package"];
                        drToAdd["ProductNameVN_NoSign"] = Utils.RemoveVietnameseSigns(productSKUData["ProductNameVN"] + " " + SKU).ToLower();


                        // SKU_tb.Text = newCustomerID.ToString();

                        id_tb.Text = newId.ToString();
                        mInventoryTransaction_dt.Rows.Add(drToAdd);
                        mInventoryTransaction_dt.AcceptChanges();

                        // chuyển selected vào row mới
                        dataGV.ClearSelection(); // bỏ chọn row cũ
                        int rowIndex = dataGV.Rows.Count - 1;
                        dataGV.Rows[rowIndex].Selected = true;


                        status_lb.Text = "Thành công";
                        status_lb.ForeColor = Color.Green;

                        _ = SQLManager_Kho.Instance.insertInventoryTransactionLOGAsync(SKU, "New Sucess: " + newVallueStr, "");
                        newBtn_Click(null, null);
                    }
                    else
                    {
                        _ = SQLManager_Kho.Instance.insertInventoryTransactionLOGAsync(SKU, "New Fail: " + newVallueStr, "");

                        status_lb.Text = "Thất bại";
                        status_lb.ForeColor = Color.Red;
                    }
                }
                catch (Exception ex)
                {
                    _ = SQLManager_Kho.Instance.insertInventoryTransactionLOGAsync(SKU, "New Fail: " + newVallueStr, "Exception: " + ex.Message);
                    status_lb.Text = "Thất bại.";
                    status_lb.ForeColor = Color.Red;
                }
                
            }
        }
        private void saveBtn_Click(object sender, EventArgs e)
        {
            if (sku_cbb.Text.CompareTo("") == 0 || quantity_tb.Text.CompareTo("") == 0 || transactionType_CB.Text.CompareTo("") == 0)
            {
                MessageBox.Show(
                                "Thiếu Dữ Liệu, Kiểm Tra Lại!",
                                "Thông Báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning
                            );
                return;
            }

            int sku = Convert.ToInt32(sku_cbb.SelectedValue);
            int quantity = Convert.ToInt32(quantity_tb.Text);
            string tranType = transactionType_CB.SelectedValue?.ToString() ?? "";
            DateTime tranDate = transactionDate_dtp.Value.Date;
            string note = note_tb.Text;
            if (id_tb.Text.Length != 0)
                updateItem(int.Parse(id_tb.Text), sku, tranType, quantity, tranDate, note);
            else
                createNew(sku, tranType, quantity, tranDate, note);

        }
        private async void deleteItemSelected()
        {
            string id = id_tb.Text;

            foreach (DataRow row in mInventoryTransaction_dt.Rows)
            {
                string tranID = row["TransactionID"].ToString();
                if (tranID.CompareTo(id) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Xóa Nha, Chắc Chắn Chưa!", "Thông Báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager_Kho.Instance.deleteInventoryTransactionAsync(Convert.ToInt32(id));

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                mInventoryTransaction_dt.Rows.Remove(row);
                                mInventoryTransaction_dt.AcceptChanges();
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

        private void newBtn_Click(object sender, EventArgs e)
        {
            id_tb.Text = "";           
            status_lb.Text = "";
            quantity_tb.Text = "";

            info_gb.BackColor = newCustomerBtn.BackColor;
            edit_btn.Visible = false;
            newCustomerBtn.Visible = false;
            readOnly_btn.Visible = true;
            LuuThayDoiBtn.Visible = true;
            isNewState = true;
            LuuThayDoiBtn.Text = "Lưu Mới";
            sku_cbb.Enabled = true;
            RightUiReadOnly(false);

            sku_cbb.Focus();
            //   RightUiEnable(true);
        }

        private void ReadOnly_btn_Click(object sender, EventArgs e)
        {
            edit_btn.Visible = true;
            newCustomerBtn.Visible = true;
            readOnly_btn.Visible = false;
            LuuThayDoiBtn.Visible = false;
            info_gb.BackColor = Color.DarkGray;
            isNewState = false;
            sku_cbb.Enabled = false;
            RightUiReadOnly(true);
            if (dataGV.SelectedRows.Count > 0)
                updateRightUI();

           // RightUiEnable(true);
        }

        private void Edit_btn_Click(object sender, EventArgs e)
        {
            sku_cbb.Enabled = true;
            edit_btn.Visible = false;
            newCustomerBtn.Visible = false;
            readOnly_btn.Visible = true;
            LuuThayDoiBtn.Visible = true;
            info_gb.BackColor = edit_btn.BackColor;
            isNewState = false;
            LuuThayDoiBtn.Text = "Lưu C.Sửa";

            RightUiReadOnly(false);
          //  RightUiEnable(false);
        }

        private void updateRightUI()
        {
            try
            {
                if (isNewState) return;

                if (dataGV.SelectedRows.Count > 0)
                {
                    var cells = dataGV.SelectedRows[0].Cells;

                    int ID = Convert.ToInt32(cells["TransactionID"].Value);
                    int SKU = Convert.ToInt32(cells["SKU"].Value);
                    string tranType = cells["TransactionType"].Value.ToString();
                    int quantity = Convert.ToInt32(cells["Quantity"].Value);
                    DateTime tranDate = Convert.ToDateTime(cells["TransactionDate"].Value);
                    string note = cells["Note"].Value.ToString();
                   
                    if (!sku_cbb.Items.Cast<object>().Any(i => ((DataRowView)i)["SKU"].ToString() == SKU.ToString()))
                    {
                        sku_cbb.DataSource = mSKU_dt;
                    }

                    id_tb.Text = ID.ToString();
                    sku_cbb.SelectedValue = SKU;
                    transactionType_CB.SelectedValue = tranType;
                    transactionDate_dtp.Value = tranDate;
                    quantity_tb.Text = quantity.ToString();
                    note_tb.Text = note.ToString();                   

                    status_lb.Text = "";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void Tb_KeyPress_OnlyNumber(object sender, KeyPressEventArgs e)
        {
            System.Windows.Forms.TextBox tb = sender as System.Windows.Forms.TextBox;

            // Chỉ cho nhập số, dấu chấm hoặc ký tự điều khiển
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            // Nếu đã có 1 dấu chấm, không cho nhập thêm
            if (e.KeyChar == '.' && tb.Text.Contains("."))
            {
                e.Handled = true;
            }
        }

        private void RightUiReadOnly(bool isReadOnly)
        {
            
            quantity_tb.ReadOnly = isReadOnly;
        }        
    }
}
