using ClosedXML.Parser;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
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
    public partial class PurchasePrices : Form
    {
        DataTable mSKU_dt, mSeller_dt, mPurchasePrices_dt;
        DataView mLogDV;
        private Timer debounceTimer = new Timer { Interval = 300 };
        private Timer sellerDebounceTimer = new Timer { Interval = 300 };
        bool isNewState = false;
        private LoadingOverlay loadingOverlay;
        public PurchasePrices()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            Utils.SetTabStopRecursive(this, false);

            int countTab = 0;
            seller_cbb.TabIndex = countTab++; seller_cbb.TabStop = true;
            sku_cbb.TabIndex = countTab++; sku_cbb.TabStop = true;
            price_tb.TabIndex = countTab++; price_tb.TabStop = true;
            LuuThayDoiBtn.TabIndex = countTab++; LuuThayDoiBtn.TabStop = true;

            status_lb.Text = "";

            newCustomerBtn.Click += newBtn_Click;
            LuuThayDoiBtn.Click += saveBtn_Click;
            dataGV.SelectionChanged += this.dataGV_CellClick;
            price_tb.KeyPress += Tb_KeyPress_OnlyNumber;

            debounceTimer.Tick += DebounceTimer_Tick;
            sellerDebounceTimer.Tick += SellerDebounceTimer_Tick;

            edit_btn.Click += Edit_btn_Click;
            readOnly_btn.Click += ReadOnly_btn_Click;
            this.KeyDown += ProductList_KeyDown;


            sku_cbb.TextUpdate += sku_cbb_TextUpdate;
            seller_cbb.TextUpdate += Seller_cbb_TextUpdate;
        }

        private void ProductList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                SQLStore_Kho.Instance.removePurchasePrices();
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
                var PurchasePricesTask = SQLStore_Kho.Instance.getPurchasePricesSync();
                var PurchasePricesLogTask = SQLStore_Kho.Instance.GetPurchasePricesLogsAsync();
                await Task.WhenAll(SKUTask, SellerTask, PurchasePricesTask, PurchasePricesLogTask);
                mSKU_dt = SKUTask.Result;
                mSeller_dt = SellerTask.Result;
                mPurchasePrices_dt = PurchasePricesTask.Result;
                mLogDV = new DataView(PurchasePricesLogTask.Result);

                foreach (DataColumn col in mPurchasePrices_dt.Columns)
                    col.ReadOnly = false;

                sku_cbb.DataSource = mSKU_dt;
                sku_cbb.DisplayMember = "ProductNameVN";  // hiển thị tên
                sku_cbb.ValueMember = "SKU";
                sku_cbb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

                seller_cbb.DataSource = mSeller_dt;
                seller_cbb.DisplayMember = "SupplierName";  // hiển thị tên
                seller_cbb.ValueMember = "SupplierID";
                seller_cbb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

                log_GV.DataSource  = mLogDV;
                dataGV.DataSource = mPurchasePrices_dt;
                Utils.HideColumns(dataGV, new[] { "SKU", "PurchasePricesID", "TransactionID", "SellerID" });
                Utils.SetGridOrdinal(mPurchasePrices_dt, new[] { "SellerName", "Name_VN", "Package", "Price" });
                Utils.SetGridHeaders(dataGV, new System.Collections.Generic.Dictionary<string, string> {
                    {"Name_VN", "Tên Tiếng Việt" },
                    {"Package", "Đ.Vị" },
                    {"TransactionTypeName", "Loại" },
                    {"Quantity", "Số Lượng" },
                    {"TransactionDate", "Ngày" },
                    {"Note", "Ghi Chú" },
                    {"FarmSourceCode", "Khu Vực" },
                    {"SellerName", "Nhà Cung Cấp" },
                    {"Price", "Giá" }
                });

                Utils.SetGridWidths(dataGV, new System.Collections.Generic.Dictionary<string, int> {
                    {"TransactionID", 70},
                    {"Name_VN", 250},
                    {"SellerName", 200},
                    {"Note", 300}
                });

                Utils.SetGridFormat_NO(dataGV, "Price");
                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;


                Utils.HideColumns(log_GV, new[] { "SKU", "LogID", "SellerID", "SellerID" });

                updateRightUI();
                        
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

        private async void updateItem(int ID, int SKU, int sellerIDe, int price)
        {
            foreach (DataRow row in mPurchasePrices_dt.Rows)
            {
                int tranID = Convert.ToInt32(row["PurchasePricesID"]);
                if (tranID.CompareTo(ID) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogResult == DialogResult.Yes)
                    {
                        DataRowView productSKUData = (DataRowView)sku_cbb.SelectedItem;
                        DataRowView SellerData = (DataRowView)seller_cbb.SelectedItem;
                        string newVallueStr = $"Người Bán: {SellerData["SupplierName"]}; Tên SP: {productSKUData["ProductNameVN"]}; Giá: {price_tb.Text}; ";
                        string oldVallueStr = $"Người Bán: {row["SellerName"]}; Tên SP: {row["Name_VN"]}; Giá: {row["Price"]}; ";
                        try
                        {
                            bool isScussess = await SQLManager_Kho.Instance.updatePurchasePricesAsync(ID, sellerIDe, SKU, price);

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                string package = productSKUData["Package"].ToString();
                                if (package.CompareTo("weight") == 0)
                                    package = "kg";
                                row["SKU"] = SKU;
                                row["SellerID"] = sellerIDe;
                                row["Price"] = price;
                                row["Name_VN"] = productSKUData["ProductNameVN"];
                                row["Package"] = package;
                                row["SellerName"] = SellerData["SupplierName"];

                                _ = SQLManager_Kho.Instance.insertPurchasePricesLOGAsync(SKU, sellerIDe, "edit Sucess: " + newVallueStr, oldVallueStr);
                            }
                            else
                            {
                                status_lb.Text = "Thất bại.";
                                status_lb.ForeColor = Color.Red;

                                _ = SQLManager_Kho.Instance.insertPurchasePricesLOGAsync(SKU, sellerIDe, "edit FAIL: " + newVallueStr, oldVallueStr);
                            }
                        }
                        catch (Exception ex)
                        {
                            status_lb.Text = "Thất bại.";
                            status_lb.ForeColor = Color.Red;

                            _ = SQLManager_Kho.Instance.insertPurchasePricesLOGAsync(SKU, sellerIDe, "edit ERROR: " + newVallueStr, "Exception: " + ex.Message);
                        }
                    }
                    break;
                }
            }
        }

        private async void createNew(int SKU, int sellerIDe, int price)
        {
            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                DataRowView productSKUData = (DataRowView)sku_cbb.SelectedItem;
                DataRowView SellerData = (DataRowView)seller_cbb.SelectedItem;

                string newVallueStr = $"Người Bán: {SellerData["SupplierName"]}; Tên SP: {productSKUData["ProductNameVN"]}; Giá: {price_tb.Text}; ";
                try
                {
                    int newId = await SQLManager_Kho.Instance.insertPurchasePricesAsync(sellerIDe, SKU, price);

                    if (newId > 0)
                    {
                        string package = productSKUData["Package"].ToString();
                        if (package.CompareTo("weight") == 0)
                            package = "kg";

                        DataRow drToAdd = mPurchasePrices_dt.NewRow();

                        drToAdd["PurchasePricesID"] = newId;
                        drToAdd["SKU"] = SKU;
                        drToAdd["SellerID"] = sellerIDe;
                        drToAdd["Price"] = price;
                        drToAdd["Name_VN"] = productSKUData["ProductNameVN"];
                        drToAdd["Package"] = package;
                        drToAdd["SellerName"] = SellerData["SupplierName"];

                        mPurchasePrices_dt.Rows.Add(drToAdd);
                        mPurchasePrices_dt.AcceptChanges();

                        status_lb.Text = "Thành công";
                        status_lb.ForeColor = Color.Green;

                        _ = SQLManager_Kho.Instance.insertPurchasePricesLOGAsync(SKU, sellerIDe, "New Sucess: " + newVallueStr, "");
                        newBtn_Click(null, null);
                    }
                    else
                    {
                        _ = SQLManager_Kho.Instance.insertPurchasePricesLOGAsync(SKU, sellerIDe, "New Fail: " + newVallueStr, "");

                        status_lb.Text = "Thất bại";
                        status_lb.ForeColor = Color.Red;
                    }
                }
                catch (Exception ex)
                {
                    _ = SQLManager_Kho.Instance.insertPurchasePricesLOGAsync(SKU, sellerIDe, "New Fail: " + newVallueStr, "Exception: " + ex.Message);
                    status_lb.Text = "Thất bại.";
                    status_lb.ForeColor = Color.Red;
                }

            }
        }
        private void saveBtn_Click(object sender, EventArgs e)
        {
            if (sku_cbb.Text.CompareTo("") == 0 || sku_cbb.SelectedValue == null || price_tb.Text.CompareTo("") == 0 || seller_cbb.SelectedValue == null || seller_cbb.Text.CompareTo("") == 0)
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
            int price = string.IsNullOrWhiteSpace(price_tb.Text) ? 0 : Convert.ToInt32(price_tb.Text);            
            int sellerID = Convert.ToInt32(seller_cbb.SelectedValue);

            if (id_tb.Text.Length != 0)
                updateItem(int.Parse(id_tb.Text), sku, sellerID, price);
            else
                createNew(sku, sellerID, price);

        }
        private async void deleteItemSelected()
        {
            string id = id_tb.Text;

            foreach (DataRow row in mPurchasePrices_dt.Rows)
            {
                string tranID = row["PurchasePricesID"].ToString();
                if (tranID.CompareTo(id) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Xóa Nha, Chắc Chắn Chưa!", "Thông Báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager_Kho.Instance.deletePurchasePricesAsync(Convert.ToInt32(id));

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                mPurchasePrices_dt.Rows.Remove(row);
                                mPurchasePrices_dt.AcceptChanges();
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
            price_tb.Text = "";

            info_gb.BackColor = newCustomerBtn.BackColor;
            edit_btn.Visible = false;
            newCustomerBtn.Visible = false;
            readOnly_btn.Visible = true;
            LuuThayDoiBtn.Visible = true;
            isNewState = true;
            LuuThayDoiBtn.Text = "Lưu Mới";
            sku_cbb.Enabled = true;
            seller_cbb.Enabled = true;
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
            seller_cbb.Enabled = false;
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
            seller_cbb.Enabled = true;
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

                    int ID = Convert.ToInt32(cells["PurchasePricesID"].Value);
                    int SKU = Convert.ToInt32(cells["SKU"].Value);
                    int sellerID = cells["SellerID"].Value != null && cells["SellerID"].Value != DBNull.Value? Convert.ToInt32(cells["SellerID"].Value): -1;
                    int price = Convert.ToInt32(cells["Price"].Value);
                   
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
                    price_tb.Text = price.ToString();
                    seller_cbb.SelectedValue = sellerID;

                    status_lb.Text = "";

                    mLogDV.RowFilter = $"SKU = {SKU} AND SellerID = {sellerID}";
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
            
            price_tb.ReadOnly = isReadOnly;
        }
    }
}
