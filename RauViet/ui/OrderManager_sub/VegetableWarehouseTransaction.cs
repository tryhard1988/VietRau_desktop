using Microsoft.VisualBasic;
using PdfSharp.Drawing.BarCodes;
using RauViet.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataTable = System.Data.DataTable;

namespace RauViet.ui
{
    public partial class VegetableWarehouseTransaction : Form
    {
        DataTable mSKU_dt, mSeller_dt, mInventoryTransaction_dt, mPurchasePrices_dt;
        DataView mLogDV;
        private Timer debounceTimer = new Timer { Interval = 300 };
        private Timer sellerDebounceTimer = new Timer { Interval = 300 };
        bool isNewState = false;
        private LoadingOverlay loadingOverlay;
        public VegetableWarehouseTransaction()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            transactionType_CB.DisplayMember = "Text";
            transactionType_CB.ValueMember = "Value";
            transactionType_CB.DataSource = new[]
            {
                new { Text = "Nhập Hàng.", Value = "N" },
                new { Text = "Xuất Đóng Gói.", Value = "X" },
                new { Text = "Xuất Hủy.", Value = "H" },
                new { Text = "Xuất Phơi.", Value = "P" },
                new { Text = "Khác.", Value = "K" }
            };

            transactionDate_dtp.Format = DateTimePickerFormat.Custom;
            transactionDate_dtp.CustomFormat = "dd/MM/yyyy";

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            Utils.SetTabStopRecursive(this, false);

            int countTab = 0;
            transactionDate_dtp.TabIndex = countTab++; transactionDate_dtp.TabStop = true;
            transactionType_CB.TabIndex = countTab++; transactionType_CB.TabStop = true;
            farmSourceCode_tb.TabIndex = countTab++; farmSourceCode_tb.TabStop = true;
            seller_cbb.TabIndex = countTab++; seller_cbb.TabStop = true;
            sku_cbb.TabIndex = countTab++; sku_cbb.TabStop = true;
            quantity_tb.TabIndex = countTab++; quantity_tb.TabStop = true;            
            note_tb.TabIndex = countTab++; note_tb.TabStop = true;
            LuuThayDoiBtn.TabIndex = countTab++; LuuThayDoiBtn.TabStop = true;

            status_lb.Text = "";

            newCustomerBtn.Click += newBtn_Click;
            LuuThayDoiBtn.Click += saveBtn_Click;
            dataGV.SelectionChanged += this.dataGV_CellClick;
            code_GV.SelectionChanged += Code_GV_SelectionChanged;
            quantity_tb.KeyPress += Tb_KeyPress_OnlyNumber;

            debounceTimer.Tick += DebounceTimer_Tick;
            sellerDebounceTimer.Tick += SellerDebounceTimer_Tick;

            edit_btn.Click += Edit_btn_Click;
            readOnly_btn.Click += ReadOnly_btn_Click;
            this.KeyDown += ProductList_KeyDown;

            transactionType_CB.SelectedIndexChanged += UpdateSellerUI_CB_Click;

            sku_cbb.TextUpdate += sku_cbb_TextUpdate;
            seller_cbb.TextUpdate += Seller_cbb_TextUpdate;
            
            thanhToan_btn.Click += ThanhToan_btn_Click;

            inHoaDon_btn.Click += InHoaDon_btn_Click;
            xemHoaDon_btn.Click += XemHoaDon_btn_Click;

            inBangKe_btn.Click += InBangKe_btn_Click;
            xemBangKe_btn.Click += XemBangKe_btn_Click;

            inBangKe2_btn.Click += InBangKe2_btn_Click;
            xemBangKe2_btn.Click += XemBangKe2_btn_Click;

            inPhieuDeNghi_btn.Click += InPhieuDeNghi_btn_Click;
            xemPhieuDeNghi_btn.Click += XemPhieuDeNghi_btn_Click;

            seller_cbb.SelectedIndexChanged += UpdatePrice;
            sku_cbb.SelectedIndexChanged += Sku_cbb_SelectedIndexChanged; ;

        }

        private void ProductList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                SQLStore_Kho.Instance.removeVegetableWarehouseTransaction();
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
                var SKUTask = SQLStore_Kho.Instance.getProductSKUAsync(parameters);
                var SellerTask = SQLStore_Kho.Instance.GetSupplierAsync();                
                var InventoryTransactionTask = SQLStore_Kho.Instance.getVegetableWarehouseTransactionSync();
                var PurchasePricesTask = SQLStore_Kho.Instance.getPurchasePricesSync();
                var InventoryTransactionLogTask = SQLStore_Kho.Instance.getVegetableWarehouseTransactionLOGAsync();
                await Task.WhenAll(SKUTask, SellerTask, InventoryTransactionTask, PurchasePricesTask, InventoryTransactionLogTask);
                mSKU_dt = SKUTask.Result;
                mSeller_dt = SellerTask.Result;
                mInventoryTransaction_dt = InventoryTransactionTask.Result;
                mPurchasePrices_dt = PurchasePricesTask.Result;
                mLogDV = new DataView(InventoryTransactionLogTask.Result);

                foreach (DataColumn col in mInventoryTransaction_dt.Columns)
                    col.ReadOnly = false;

                var data = mInventoryTransaction_dt.AsEnumerable().Where(r => !r.IsNull("TransactionDate")).Select(r => r.Field<DateTime>("TransactionDate").ToString("MM/yyyy")).Distinct().OrderBy(x => x).ToList();

                DataTable dtMonthYear = new DataTable();
                dtMonthYear.Columns.Add("MonthYear", typeof(string));
                foreach (var item in data)
                {
                    dtMonthYear.Rows.Add(item);
                }

                var data1 = mInventoryTransaction_dt.AsEnumerable().Where(r => !r.IsNull("SellerID") && !r.IsNull("SellerName"))
                                                                    .Select(r => new
                                                                    {
                                                                        SellerID = r.Field<int>("SellerID"),
                                                                        SellerName = r.Field<string>("SellerName")
                                                                    }).Distinct().OrderBy(x => x.SellerName).ToList();

                DataTable dtSeller = new DataTable();
                dtSeller.Columns.Add("SellerID", typeof(int));
                dtSeller.Columns.Add("SellerName", typeof(string));

                foreach (var item in data1)
                {
                    dtSeller.Rows.Add(item.SellerID, item.SellerName);
                }

                sku_cbb.DataSource = mSKU_dt;
                sku_cbb.DisplayMember = "ProductNameVN";  // hiển thị tên
                sku_cbb.ValueMember = "SKU";
                sku_cbb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

                seller_cbb.DataSource = mSeller_dt;
                seller_cbb.DisplayMember = "SupplierName";  // hiển thị tên
                seller_cbb.ValueMember = "SupplierID";
                seller_cbb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

                sellerGV.DataSource = dtSeller;
                monthGV.DataSource = dtMonthYear;
                log_GV.DataSource = mLogDV;
                dataGV.DataSource = mInventoryTransaction_dt;
                Utils.HideColumns(sellerGV, new[] { "SellerID" });
                Utils.HideColumns(dataGV, new[] { "SKU", "TransactionType", "TransactionID", "SellerID", "FarmSourceCode" });

                Utils.SetGridHeaders(dataGV, new System.Collections.Generic.Dictionary<string, string> {
                    {"Name_VN", "Tên Tiếng Việt" },
                    {"unit", "Đ.Vị" },
                    {"TransactionTypeName", "Loại" },
                    {"Quantity", "Số Lượng" },
                    {"TransactionDate", "Ngày" },
                    {"Note", "Ghi Chú" },
                    {"FarmSourceCode", "Mã Lệnh" },
                    {"SellerName", "Nhà Cung Cấp" }
                });

                Utils.SetGridWidths(dataGV, new System.Collections.Generic.Dictionary<string, int> {
                    {"TransactionID", 70},
                    {"IsPaid", 70},
                    {"Name_VN", 250},
                    {"SellerName", 200},
                    {"Note", 300}
                });


                Utils.HideColumns(log_GV, new[] { "LogID", "FarmSourceCode" });
                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                updateRightUI();
                        
                ReadOnly_btn_Click(null, null);
                UpdateSellerUI_CB_Click(null, null);

                preLoadCode();
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

        private void preLoadCode()
        {
            string targetCode = farmSourceCode_tb.Text.Trim();
            var dataTemp = mInventoryTransaction_dt.AsEnumerable().Where(r => !r.IsNull("FarmSourceCode")).Select(r => r.Field<string>("FarmSourceCode")).Distinct().ToList();

            DataTable dtFarmSourceCode = new DataTable();
            dtFarmSourceCode.Columns.Add("FarmSourceCode", typeof(string));

            foreach (var item in dataTemp)
            {
                dtFarmSourceCode.Rows.Add(item);
            }

            code_GV.DataSource = dtFarmSourceCode;
            Utils.SetGridHeaders(code_GV, new System.Collections.Generic.Dictionary<string, string> { { "FarmSourceCode", "Mã Lệnh" } });
            Utils.SetGridWidths(code_GV, new System.Collections.Generic.Dictionary<string, int> { { "TransactionID", 100 } });

            
            foreach (DataGridViewRow row in code_GV.Rows)
            {
                if (row.Cells["FarmSourceCode"].Value?.ToString() == targetCode)
                {
                    row.Selected = true;
                    code_GV.CurrentCell = row.Cells["FarmSourceCode"];
                    code_GV.FirstDisplayedScrollingRowIndex = row.Index; // scroll tới dòng
                    break;
                }
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
            debounceTimer.Stop();
            Utils.ComboBoxSearchResult(sku_cbb, mSKU_dt, "ProductNameVN_NoSign");
        }

        private void Seller_cbb_TextUpdate(object sender, EventArgs e)
        {
            sellerDebounceTimer.Stop();
            sellerDebounceTimer.Start();
        }

        private void SellerDebounceTimer_Tick(object sender, EventArgs e)
        {
            sellerDebounceTimer.Stop();
            Utils.ComboBoxSearchResult(seller_cbb, mSeller_dt, "searching_nosign");
        }


        private void dataGV_CellClick(object sender, EventArgs e)
        {
            updateRightUI();            
        }

        private async void updateItem(int ID, int SKU, string TransactionType, decimal Quantity, string FarmSourceCode, DateTime TransactionDate, string Note, int? sellerID, int price, string unit)
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
                        DataRowView SellerData = (DataRowView)seller_cbb.SelectedItem;
                        string newVallueStr = $"Tên SP: {productSKUData["ProductNameVN"]}; Hành động: {transactionType_CB.Text}; số lượng: {Quantity}; Khu Vực: {FarmSourceCode}; ngày: {TransactionDate}; ghi chứ: {Note}; Người Bán: {seller_cbb.Text}";
                        string oldVallueStr = $"Tên SP: {row["Name_VN"].ToString()}; Hành động: {row["TransactionType"].ToString()}; số lượng: {row["Quantity"].ToString()}; Khu Vực: {row["FarmSourceCode"].ToString()}; ngày: {row["TransactionDate"].ToString()}; ghi chú: {row["Note"].ToString()}; Người Bán: {row["SellerName"]}";
                        try
                        {      
                            bool isScussess = await SQLManager_Kho.Instance.updateVegetableWarehouseTransactionAsync(ID, SKU, TransactionType, Quantity, FarmSourceCode, TransactionDate, Note, sellerID, price, unit);
                            
                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                row["SKU"] = SKU;
                                row["TransactionType"] = TransactionType;
                                row["TransactionTypeName"] = transactionType_CB.Text;
                                row["Quantity"] = Quantity;
                                row["FarmSourceCode"] = FarmSourceCode;
                                row["Name_VN"] = productSKUData["ProductNameVN"];
                                row["TransactionDate"] = TransactionDate;
                                row["Note"] = Note;
                                row["SellerID"] = sellerID ?? (object)DBNull.Value;
                                row["Price"] = price;
                                row["unit"] = unit;

                                if (sellerID.HasValue)
                                    row["SellerName"] = SellerData["SupplierName"];

                                _ = SQLManager_Kho.Instance.insertVegetableWarehouseTransactionLOGAsync(FarmSourceCode, oldVallueStr, "edit Sucess: " + newVallueStr);

                                preLoadCode();
                            }
                            else
                            {
                                status_lb.Text = "Thất bại.";
                                status_lb.ForeColor = Color.Red;

                                _ = SQLManager_Kho.Instance.insertVegetableWarehouseTransactionLOGAsync(FarmSourceCode, oldVallueStr, "edit FAIL: " + newVallueStr);
                            }
                        }
                        catch (Exception ex)
                        {
                            status_lb.Text = "Thất bại.";
                            status_lb.ForeColor = Color.Red;

                            _ = SQLManager_Kho.Instance.insertVegetableWarehouseTransactionLOGAsync(FarmSourceCode, "Exception: " + ex.Message, "edit ERROR: " + newVallueStr);
                        }  
                    }
                    break;
                }
            }
        }

        private async void createNew(int SKU, string TransactionType, decimal Quantity, string FarmSourceCode, DateTime TransactionDate, string Note, int? sellerID, int price, string unit)
        {
            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                DataRowView productSKUData = (DataRowView)sku_cbb.SelectedItem;
                DataRowView SellerData = (DataRowView)seller_cbb.SelectedItem;

                string newVallueStr = $"Tên SP: {productSKUData["ProductNameVN"]}; Hành động: {transactionType_CB.Text}; số lượng: {Quantity}; Khu Vực: {FarmSourceCode}; ngày: {TransactionDate}; ghi chứ: {Note}; Người Bán: {seller_cbb.Text}";
                try
                {
                    int newId = await SQLManager_Kho.Instance.insertVegetableWarehouseTransactionAsync(SKU, TransactionType, Quantity, FarmSourceCode, TransactionDate, Note, sellerID, price, unit);
                    
                    if (newId > 0)
                    {
                        
                        DataRow drToAdd = mInventoryTransaction_dt.NewRow();


                        drToAdd["TransactionID"] = newId;
                        drToAdd["SKU"] = SKU;
                        drToAdd["TransactionType"] = TransactionType;
                        drToAdd["TransactionTypeName"] = transactionType_CB.Text;
                        drToAdd["Quantity"] = Quantity;
                        drToAdd["FarmSourceCode"] = FarmSourceCode;
                        drToAdd["TransactionDate"] = TransactionDate;
                        drToAdd["Note"] = Note;
                        drToAdd["SellerID"] = sellerID ?? (object)DBNull.Value;
                        drToAdd["Name_VN"] = productSKUData["ProductNameVN"];
                        drToAdd["Price"] = price;
                        drToAdd["unit"] = unit;
                        drToAdd["IsPaid"] = false;
                        if (sellerID.HasValue)
                            drToAdd["SellerName"] = SellerData["SupplierName"];


                        mInventoryTransaction_dt.Rows.Add(drToAdd);
                        mInventoryTransaction_dt.AcceptChanges();

                        status_lb.Text = "Thành công";
                        status_lb.ForeColor = Color.Green;

                        _ = SQLManager_Kho.Instance.insertVegetableWarehouseTransactionLOGAsync(FarmSourceCode, "", "New Sucess: " + newVallueStr);

                        preLoadCode();
                        newBtn_Click(null, null);

                    }
                    else
                    {
                        _ = SQLManager_Kho.Instance.insertVegetableWarehouseTransactionLOGAsync(FarmSourceCode, "", "New Fail: " + newVallueStr);

                        status_lb.Text = "Thất bại";
                        status_lb.ForeColor = Color.Red;
                    }
                }
                catch (Exception ex)
                {
                    _ = SQLManager_Kho.Instance.insertVegetableWarehouseTransactionLOGAsync(FarmSourceCode, "Exception: " + ex.Message, "New Fail: " + newVallueStr);
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
            decimal quantity = string.IsNullOrWhiteSpace(quantity_tb.Text) ? 0 : decimal.Parse(quantity_tb.Text, CultureInfo.InvariantCulture);
            string tranType = transactionType_CB.SelectedValue?.ToString() ?? "";
            string farmSourceCode = farmSourceCode_tb.Text;
            DateTime tranDate = transactionDate_dtp.Value.Date;
            string note = note_tb.Text;
            string unit = unit_tb.Text;
            int? sellerID = null;
            int price = Convert.ToInt32(price_tb.Text);
            

            if (string.IsNullOrEmpty(farmSourceCode))
                farmSourceCode = tranDate.ToString("ddMMyyyy");

            farmSourceCode = farmSourceCode.Trim();

            if (seller_cbb.Visible && seller_cbb.SelectedValue != null && seller_cbb.SelectedValue != DBNull.Value && seller_cbb.Text != null)
            {
                sellerID = Convert.ToInt32(seller_cbb.SelectedValue);
            }

            if (id_tb.Text.Length != 0)
                updateItem(int.Parse(id_tb.Text), sku, tranType, quantity, farmSourceCode, tranDate, note, sellerID, price, unit);
            else
                createNew(sku, tranType, quantity, farmSourceCode, tranDate, note, sellerID, price, unit);

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
                            bool isScussess = await SQLManager_Kho.Instance.deleteVegetableWarehouseTransactionAsync(Convert.ToInt32(id));

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
                    int sellerID = cells["SellerID"].Value != null && cells["SellerID"].Value != DBNull.Value? Convert.ToInt32(cells["SellerID"].Value): -1;
                    string tranType = cells["TransactionType"].Value.ToString();
                    string farmSourceCode = cells["FarmSourceCode"].Value.ToString();
                    decimal quantity = Convert.ToDecimal(cells["Quantity"].Value);
                    DateTime tranDate = Convert.ToDateTime(cells["TransactionDate"].Value);
                    string note = cells["Note"].Value.ToString();
                    int price = Convert.ToInt32(cells["Price"].Value);
                    string unit = cells["unit"].Value.ToString();

                    if (!sku_cbb.Items.Cast<object>().Any(i => ((DataRowView)i)["SKU"].ToString() == SKU.ToString()))
                    {
                        sku_cbb.DataSource = mSKU_dt;
                    }

                    if (!seller_cbb.Items.Cast<object>().Any(i => ((DataRowView)i)["SupplierID"].ToString() == sellerID.ToString()))
                    {
                        seller_cbb.DataSource = mSeller_dt;
                    }

                    id_tb.Text = ID.ToString();
                    sku_cbb.SelectedValue = SKU;
                    transactionType_CB.SelectedValue = tranType;
                    transactionDate_dtp.Value = tranDate;
                    quantity_tb.Text = quantity.ToString("F1", CultureInfo.InvariantCulture);
                    note_tb.Text = note.ToString();
                    seller_cbb.SelectedValue = sellerID;
                    farmSourceCode_tb.Text = farmSourceCode;
                    price_tb.Text = price.ToString();
                    status_lb.Text = "";
                    unit_tb.Text = unit;
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
            farmSourceCode_tb.ReadOnly = isReadOnly;
            note_tb.ReadOnly = isReadOnly;
            transactionDate_dtp.Enabled = !isReadOnly;
            transactionType_CB.Enabled = !isReadOnly;
            seller_cbb.Enabled = !isReadOnly;
        }

        private void UpdateSellerUI_CB_Click(object sender, EventArgs e)
        {
            string transactionTypeStr = transactionType_CB.SelectedValue.ToString();

            if (transactionTypeStr.CompareTo("N") == 0)
            {                
                seller_cbb.Visible = true;
                return; 
            }
            else
            {
                seller_cbb.Visible = false;
                return;
            }
        }

        private void Code_GV_SelectionChanged(object sender, EventArgs e)
        {

            if (code_GV.SelectedRows.Count > 0)
            {
                var cells = code_GV.SelectedRows[0].Cells;
                string code = cells["FarmSourceCode"].Value.ToString();
                mInventoryTransaction_dt.DefaultView.RowFilter = $"FarmSourceCode = '{code}'";

                mLogDV.RowFilter = $"FarmSourceCode = '{code}'";
            }
        }

        private void XemHoaDon_btn_Click(object sender, EventArgs e)
        {
            InHoaDon(1);
        }

        private void InHoaDon_btn_Click(object sender, EventArgs e)
        {
            InHoaDon(2);
        }

        void InHoaDon(int type)
        {
            if (code_GV.SelectedRows.Count > 0)
            {
                string receiverName = Interaction.InputBox("Nhập tên người nhận hàng:", "Thông tin người nhận","Đôn Thị Mỹ Phương");

                // Nếu bấm Cancel hoặc không nhập thì dừng
                if (string.IsNullOrWhiteSpace(receiverName))
                    return;

                var cells = code_GV.SelectedRows[0].Cells;
                var farmSourceCode = cells["FarmSourceCode"].Value.ToString();

                HoaDonMuaHang_Printer printer = new HoaDonMuaHang_Printer(farmSourceCode, mInventoryTransaction_dt, receiverName);

                if (type == 1)
                    printer.PrintPreview(this);
                else if (type == 2)
                    printer.PrintDirect();
                else if (type == 3)
                    printer.ExportExcel();
            }
        }

        private void InBangKe_btn_Click(object sender, EventArgs e)
        {
            inBangrKe(1);
        }

        private void XemBangKe_btn_Click(object sender, EventArgs e)
        {
            inBangrKe(2);
        }


        async void inBangrKe(int state)
        {
            if (monthGV.SelectedRows.Count <= 0 || sellerGV.SelectedRows.Count <= 0) return;

            var monthCells = monthGV.SelectedRows[0].Cells;
            var monthYear = Convert.ToDateTime(monthCells["MonthYear"].Value);

            var sellerCells = sellerGV.SelectedRows[0].Cells;
            var sellerID = Convert.ToInt32(sellerCells["SellerID"].Value);

            BangKeThuMuaHangHoa_Printer printer = new BangKeThuMuaHangHoa_Printer(monthYear.Month, monthYear.Year, sellerID, mInventoryTransaction_dt);
            await printer.loadData();
            if (state == 1) //in
            {
                printer.PrintDirect();
            }
            else if (state == 2) //xem
            {
                printer.PrintPreview(this);
            }
            else if (state == 3) //excel
            {
                printer.ExportExcel();
            }
        }

        private void InBangKe2_btn_Click(object sender, EventArgs e)
        {
            inBangrKe2(1);
        }

        private void XemBangKe2_btn_Click(object sender, EventArgs e)
        {
            inBangrKe2(2);
        }


        async void inBangrKe2(int state)
        {
            if (code_GV.SelectedRows.Count > 0)
            {
                var cells = code_GV.SelectedRows[0].Cells;
                var farmSourceCode = cells["FarmSourceCode"].Value.ToString();

                BangKeThuMuaHangHoa_Printer2 printer = new BangKeThuMuaHangHoa_Printer2(farmSourceCode, mInventoryTransaction_dt);
                await printer.loadData();
                if (state == 1) //in
                {
                    printer.PrintDirect();
                }
                else if (state == 2) //xem
                {
                    printer.PrintPreview(this);
                }
                else if (state == 3) //excel
                {
                    printer.ExportExcel();
                }
            }
        }

        private void XemPhieuDeNghi_btn_Click(object sender, EventArgs e)
        {
            PhieuDeNghi(true);
        }

        private void InPhieuDeNghi_btn_Click(object sender, EventArgs e)
        {
            PhieuDeNghi(false);
        }

        void PhieuDeNghi(bool isPreview)
        {
            if (code_GV.SelectedRows.Count <= 0) return;

            var cells = code_GV.SelectedRows[0].Cells;
            var farmSourceCode = cells["FarmSourceCode"].Value.ToString();

            var row = mInventoryTransaction_dt.AsEnumerable().FirstOrDefault(r => r.Field<string>("FarmSourceCode") == farmSourceCode);
            DateTime transactionDate = row?.Field<DateTime?>("TransactionDate") ?? DateTime.Now;

            decimal total = mInventoryTransaction_dt.AsEnumerable().Where(r => r.Field<string>("FarmSourceCode") == farmSourceCode).Sum(r => r.Field<int>("Price") * r.Field<decimal>("Quantity"));
            PhieuDeNghiThanhToan form = new PhieuDeNghiThanhToan(transactionDate, Convert.ToInt32(total), farmSourceCode, isPreview);
            form.ShowData();
            form.ShowDialog();
            }

        private async void ThanhToan_btn_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Hỏi Lần Cuối, Chắc Chắn Chưa!", "Thông Báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult != DialogResult.Yes)
                return;

            if (code_GV.SelectedRows.Count > 0)
            { 
                var cells = code_GV.SelectedRows[0].Cells;
                var codeSelected = cells["FarmSourceCode"].Value.ToString();

                bool isPaid = true;
                foreach (DataRow row in mInventoryTransaction_dt.Rows)
                {
                    string farmSourceCode = Convert.ToString(row["FarmSourceCode"]);
                    if (farmSourceCode.CompareTo(codeSelected) == 0)
                    {
                        string oldValue = $"Đã Thanh Toán: {row["IsPaid"]}";
                        string newValue = $"Đã Thanh Toán: {true}";
                        string actionType = "Thanh Toan";

                        int SKU = Convert.ToInt32(row["SKU"]);
                        try
                        {
                            int transactionID = Convert.ToInt32(row["TransactionID"]);

                            bool isScussess = await SQLManager_Kho.Instance.updateVegetableWarehouseTransaction_ThanhToanAsync(transactionID, true);

                            if (isScussess == true)
                            {
                                row["IsPaid"] = isPaid;

                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                actionType += ": Success";
                            }
                            else
                            {
                                status_lb.Text = "Thất bại.";
                                status_lb.ForeColor = Color.Red;
                                actionType += ": False";
                            }
                        }
                        catch (Exception ex)
                        {
                            status_lb.Text = "Thất bại.";
                            status_lb.ForeColor = Color.Red;
                            actionType += $": {ex.Message}";
                        }
                        finally
                        {
                            await SQLManager_Kho.Instance.insertVegetableWarehouseTransactionLOGAsync(farmSourceCode, oldValue, actionType + " " + newValue );
                        }
                    }
                }
            }
        }

        private void UpdatePrice(object sender, EventArgs e)
        {
            if (sku_cbb.Text.CompareTo("") == 0 || seller_cbb.Text.CompareTo("") == 0 || sku_cbb.SelectedValue == null || seller_cbb.SelectedValue == null)
            {
                return;
            }

            if (readOnly_btn.Visible == false)
                return;

            int sSKU = -1;
            int? sSellerID = null;
            if (dataGV.SelectedRows.Count > 0)
            {
                var cells = dataGV.SelectedRows[0].Cells;
                sSKU = Convert.ToInt32(cells["SKU"].Value);
                sSellerID = cells["SellerID"].Value == DBNull.Value ? (int?)null : Convert.ToInt32(cells["SellerID"].Value);
            }

            int sku = Convert.ToInt32(sku_cbb.SelectedValue);
            int? sellerID = seller_cbb.Visible ? seller_cbb.SelectedValue as int? : null;

            if (!isNewState && sSKU == sku && sSellerID == sellerID) return;

            if (sellerID.HasValue)
            {
                var row = mPurchasePrices_dt.AsEnumerable().FirstOrDefault(r => r.Field<int>("SKU") == sku && r.Field<int>("SellerID") == sellerID);
                if (row != null)
                    price_tb.Text = (row?["Price"] != DBNull.Value ? Convert.ToInt32(row["Price"]) : 0).ToString();
            }
        }

        private void Sku_cbb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sku_cbb.Text.CompareTo("") == 0 || sku_cbb.SelectedValue == null)
            {
                return;                
            }
            if (readOnly_btn.Visible == false)
                return;

            UpdatePrice(sender, e);

            int sku;
            if (!int.TryParse(sku_cbb.SelectedValue?.ToString(), out sku))
            {
                return; // tránh crash
            }

            DataRow[] rows = mSKU_dt.Select($"SKU = {sku}");
            string unitName = rows[0]["Package"].ToString();
            if (unitName.CompareTo("weight") == 0)
                unitName = "kg";

            unit_tb.Text = unitName;
        }

    }
}
