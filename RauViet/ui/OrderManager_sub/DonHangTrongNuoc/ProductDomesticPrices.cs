
using RauViet.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Color = System.Drawing.Color;

namespace RauViet.ui
{
    public partial class ProductDomesticPrices : Form
    {
        System.Data.DataTable mSKU_dt, mProductDomesticPrices_dt;
        private DataView mLogDV;
        private Timer debounceTimer = new Timer { Interval = 300 };
        bool isNewState = false;
        private LoadingOverlay loadingOverlay;
        public ProductDomesticPrices()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            status_lb.Text = "";

            newCustomerBtn.Click += newBtn_Click;
            LuuThayDoiBtn.Click += saveBtn_Click;
            

            debounceTimer.Tick += DebounceTimer_Tick;

            edit_btn.Click += Edit_btn_Click;
            readOnly_btn.Click += ReadOnly_btn_Click;
            this.KeyDown += ProductList_KeyDown;
            
            rawPrice_tb.KeyPress += Tb_KeyPress_OnlyNumber;
            refinedPrice_tb.KeyPress += Tb_KeyPress_OnlyNumber;
            packedPrice_tb.KeyPress += Tb_KeyPress_OnlyNumber;

        }

        private void ProductList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                SQLStore_Kho.Instance.removeProductDomesticPrices();
                ShowData();
            }
            else if (!isNewState && !edit_btn.Visible)
            {
                if (e.KeyCode == Keys.Delete)
                {
                    System.Windows.Forms.Control ctrl = this.ActiveControl;

                    if (ctrl is TextBox || ctrl is RichTextBox ||
                        (ctrl is DataGridView dgv && dgv.CurrentCell != null && dgv.IsCurrentCellInEditMode))
                    {
                        return; // không xử lý Delete
                    }

                    deleteProduct();
                }
            }
        }

        public async void ShowData()
        {
            dataGV.SelectionChanged -= this.dataGV_CellClick;
            await Task.Delay(50);
            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Show();

            try
            {
                var parameters = new Dictionary<string, object> { { "IsActive", true } };
                mSKU_dt = await SQLStore_Kho.Instance.getProductSKUAsync(parameters);
                mProductDomesticPrices_dt = await SQLStore_Kho.Instance.getProductDomesticPricesAsync();
                var logData = await SQLStore_Kho.Instance.GetProductDomesticPricesHistoryAsync();
                mLogDV = new DataView(logData);

                sku_cbb.DataSource = mSKU_dt;
                sku_cbb.DisplayMember = "ProductNameVN";  // hiển thị tên
                sku_cbb.ValueMember = "SKU";
                sku_cbb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                sku_cbb.TextUpdate += sku_cbb_TextUpdate;
                
                dataGV.DataSource = mProductDomesticPrices_dt;
                log_GV.DataSource = mLogDV;
                Utils.HideColumns(dataGV, new[] { "PriceID", "ProductNameVN_NoSign", "Package", "Priority" });
                Utils.HideColumns(log_GV, new[] { "HistoryID", "SKU" });

                DataView dv = mProductDomesticPrices_dt.DefaultView;
                dv.RowFilter = $"IsActive = true";

                Utils.SetGridHeaders(dataGV, new System.Collections.Generic.Dictionary<string, string> {
                    {"ProductName_VN", "Tên Sản Phẩm" },
                    {"RawPrice", "Giá Hàng Xá" },
                    {"RefinedPrice", "Giá Hàng Tinh" },
                    {"PackedPrice", "Giá Hàng Đóng Gói" },
                    {"IsActive", "Active" },
                });

                Utils.SetGridFormat_NO(dataGV, "RawPrice");
                Utils.SetGridFormat_NO(dataGV, "RefinedPrice");
                Utils.SetGridFormat_NO(dataGV, "PackedPrice");

                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                updateDataTextBoxFlowSKU();

                Utils.SetTabStopRecursive(this, false);

                int countTab = 0;
                sku_cbb.TabIndex = countTab++; sku_cbb.TabStop = true;               
                rawPrice_tb.TabIndex = countTab++; rawPrice_tb.TabStop = true;
                refinedPrice_tb.TabIndex = countTab++; refinedPrice_tb.TabStop = true;
                packedPrice_tb.TabIndex = countTab++; packedPrice_tb.TabStop = true;
                LuuThayDoiBtn.TabIndex = countTab++; LuuThayDoiBtn.TabStop = true;

                ReadOnly_btn_Click(null, null);
                dataGV.SelectionChanged += this.dataGV_CellClick;

                await Task.Delay(100);
                loadingOverlay.Hide();

                Utils.SetGridHeaders(log_GV, new System.Collections.Generic.Dictionary<string, string> {
                    {"OldValue", "Giá Trị Cũ" },
                    {"NewValue", "Giá Trị Mới" },
                    {"ActionBy", "Người Thực Hiện" },
                    {"CreatedAt", "Ngày Thực Hiện" }
                });
                Utils.SetGridWidths(log_GV, new System.Collections.Generic.Dictionary<string, int> {
                    {"ActionBy", 150},
                    {"CreatedAt", 120}
                });

                log_GV.Columns["OldValue"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                log_GV.Columns["NewValue"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

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

                System.Data.DataTable temp;
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
            updateDataTextBoxFlowSKU();            
        }

        private async void updateProductPrice(int SKU, int rawPrice, int refinePrice, int packedPrice, bool isActive)
        {
            foreach (DataRow row in mProductDomesticPrices_dt.Rows)
            {
                int ID = Convert.ToInt32(row["SKU"]);
                if (ID.CompareTo(SKU) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    string oldValue = $"{row["RawPrice"]} - {row["RefinedPrice"]} - {row["PackedPrice"]} - {row["IsActive"]}";
                    string newValue = $"{rawPrice} - {refinePrice} - {packedPrice} - {isActive}";
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager_Kho.Instance.updateProductDomesticPriceAsync(SKU, rawPrice, refinePrice, packedPrice, isActive);

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                row["RawPrice"] = rawPrice;
                                row["RefinedPrice"] = refinePrice;
                                row["PackedPrice"] = packedPrice;
                                row["IsActive"] = isActive;

                                _ = SQLManager_Kho.Instance.InsertProductDomesticPricesHistory(SKU, "Update Success: " + oldValue, newValue);
                            }
                            else
                            {
                                status_lb.Text = "Thất bại.";
                                status_lb.ForeColor = Color.Red;

                                _ = SQLManager_Kho.Instance.InsertProductDomesticPricesHistory(SKU, "Update Fail: " + oldValue, newValue);
                            }
                        }
                        catch (Exception ex)
                        {
                            status_lb.Text = "Thất bại.";
                            status_lb.ForeColor = Color.Red;

                            _ = SQLManager_Kho.Instance.InsertProductDomesticPricesHistory(SKU, "Update Exception: " + ex.Message + oldValue, newValue);
                        }
                    }
                    break;
                }
            }
        }

        private async void createNew(int SKU, string productName, int rawPrice, int refinePrice, int packedPrice)
        {
            DataRow[] foundRows = mProductDomesticPrices_dt.Select($"SKU = {SKU}");
            if(foundRows.Length > 0)
            {
                MessageBox.Show("Sản phẩm này đã tồn tại ?", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                string newValue = $"{rawPrice} - {refinePrice} - {packedPrice}";
                try
                {
                    int newId = await SQLManager_Kho.Instance.insertProductDomesticPriceAsync(SKU, rawPrice, refinePrice, packedPrice);
                    if (newId > 0)
                    {
                        DataRow drToAdd = mProductDomesticPrices_dt.NewRow();

                        drToAdd["PriceID"] = newId;
                        drToAdd["SKU"] = SKU;
                        drToAdd["ProductName_VN"] = productName;
                        drToAdd["RawPrice"] = rawPrice;
                        drToAdd["RefinedPrice"] = refinePrice;
                        drToAdd["PackedPrice"] = packedPrice;
                        drToAdd["IsActive"] = true;

                        mProductDomesticPrices_dt.Rows.Add(drToAdd);
                        mProductDomesticPrices_dt.AcceptChanges();

                        status_lb.Text = "Thành công";
                        status_lb.ForeColor = Color.Green;

                        _ = SQLManager_Kho.Instance.InsertProductDomesticPricesHistory(SKU, "Create Success: ", newValue);

                        newBtn_Click(null, null);
                    }
                    else
                    {
                        status_lb.Text = "Thất bại";
                        status_lb.ForeColor = Color.Red;
                        _ = SQLManager_Kho.Instance.InsertProductDomesticPricesHistory(SKU, "Create Fail: ", newValue);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR: " + ex.Message);
                    status_lb.Text = "Thất bại.";
                    status_lb.ForeColor = Color.Red;

                    _ = SQLManager_Kho.Instance.InsertProductDomesticPricesHistory(SKU, "Create Exception: " + ex.Message, newValue);
                }

            }
        }
        private void saveBtn_Click(object sender, EventArgs e)
        {
            if (sku_cbb.SelectedValue == null) return;

            int sku = Convert.ToInt32(sku_cbb.SelectedValue);
            DataRowView productSKUData = (DataRowView)sku_cbb.SelectedItem;
            string productName = productSKUData["ProductNameVN"].ToString();

            int rawPrice = string.IsNullOrWhiteSpace(rawPrice_tb.Text) ? 0 : int.Parse(rawPrice_tb.Text);
            int refinePrice = string.IsNullOrWhiteSpace(refinedPrice_tb.Text) ? 0 : int.Parse(refinedPrice_tb.Text);
            int packedPrice = string.IsNullOrWhiteSpace(packedPrice_tb.Text) ? 0 : int.Parse(packedPrice_tb.Text);

            if (id_tb.Text.Length != 0)
                updateProductPrice(sku, rawPrice, refinePrice, packedPrice, isActive_cb.Checked);
            else
                createNew(sku, productName, rawPrice, refinePrice, packedPrice);

        }
        private async void deleteProduct()
        {
            string id = id_tb.Text;

            foreach (DataRow row in mProductDomesticPrices_dt.Rows)
            {
                string priceID = row["PriceID"].ToString();
                if (priceID.CompareTo(id) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Xóa Nha, Chắc Chắn Chưa!", "Thông Báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager_Kho.Instance.deleteProductDomesticPriceAsync(Convert.ToInt32(id));

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                mProductDomesticPrices_dt.Rows.Remove(row);
                                mProductDomesticPrices_dt.AcceptChanges();
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
            rawPrice_tb.Text = "";
            refinedPrice_tb.Text = "";
            packedPrice_tb.Text = "";          

            info_gb.BackColor = newCustomerBtn.BackColor;
            edit_btn.Visible = false;
            newCustomerBtn.Visible = false;
            readOnly_btn.Visible = true;
            LuuThayDoiBtn.Visible = true;
            isNewState = true;
            LuuThayDoiBtn.Text = "Lưu Mới";
            sku_cbb.Enabled = true;
            RightUiReadOnly(false);
            isActive_cb.Visible = false;

            sku_cbb.Focus();

            RightUiEnable(true);
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
            isActive_cb.Visible = true;
            if (dataGV.SelectedRows.Count > 0)
                updateDataTextBoxFlowSKU();

            RightUiEnable(true);
        }

        private void Edit_btn_Click(object sender, EventArgs e)
        {
            sku_cbb.Enabled = false;
            edit_btn.Visible = false;
            newCustomerBtn.Visible = false;
            readOnly_btn.Visible = true;
            LuuThayDoiBtn.Visible = true;
            info_gb.BackColor = edit_btn.BackColor;
            isNewState = false;
            LuuThayDoiBtn.Text = "Lưu C.Sửa";
            isActive_cb.Enabled = true;

            RightUiReadOnly(false);
            RightUiEnable(false);
        }

        private void updateDataTextBoxFlowSKU()
        {
            try
            {
                if (isNewState) return;

                if (dataGV.SelectedRows.Count > 0)
                {
                    var cells = dataGV.SelectedRows[0].Cells;

                    string ID = cells["PriceID"].Value.ToString();
                    string SKU = cells["SKU"].Value.ToString();
                    string rawPrice = cells["RawPrice"].Value.ToString();
                    string refinePrice = cells["RefinedPrice"].Value.ToString();
                    string packedPrice = cells["PackedPrice"].Value.ToString();
                    bool isActive = Convert.ToBoolean(cells["IsActive"].Value);

                    if (!sku_cbb.Items.Cast<object>().Any(i => ((DataRowView)i)["SKU"].ToString() == SKU))
                    {
                        sku_cbb.DataSource = mSKU_dt;
                    }
                    sku_cbb.SelectedValue = SKU;

                    id_tb.Text = ID;
                    rawPrice_tb.Text = rawPrice;
                    refinedPrice_tb.Text = refinePrice;
                    packedPrice_tb.Text = packedPrice;
                    isActive_cb.Checked = isActive;

                    status_lb.Text = "";

                    mLogDV.RowFilter = $"SKU = {SKU}";
                    mLogDV.Sort = "HistoryID DESC";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
            };
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
            rawPrice_tb.ReadOnly = isReadOnly;
            refinedPrice_tb.ReadOnly = isReadOnly;
            packedPrice_tb.ReadOnly = isReadOnly;
            isActive_cb.Enabled = !isReadOnly;
        }

        private void RightUiEnable(bool enable)
        {
        }
    }
}
