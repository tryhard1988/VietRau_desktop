
using RauViet.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO.Packaging;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Color = System.Drawing.Color;

namespace RauViet.ui
{
    public partial class DomesticLiquidationPrice : Form
    {
        System.Data.DataTable mSKU_dt, mDomesticLiquidationPrice_dt;
        private DataView mLogDV;
        private Timer debounceTimer = new Timer { Interval = 300 };
        bool isNewState = false;
        private LoadingOverlay loadingOverlay;
        public DomesticLiquidationPrice()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            status_lb.Text = "";
            salePrice_tb.KeyPress += Tb_KeyPress_OnlyNumber;

            newCustomerBtn.Click += newBtn_Click;
            LuuThayDoiBtn.Click += saveBtn_Click;
            

            debounceTimer.Tick += DebounceTimer_Tick;

            edit_btn.Click += Edit_btn_Click;
            readOnly_btn.Click += ReadOnly_btn_Click;
            this.KeyDown += ProductList_KeyDown;

        }

        private void ProductList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                SQLStore_Kho.Instance.removeDomesticLiquidationPrice();
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
                mDomesticLiquidationPrice_dt = await SQLStore_Kho.Instance.getDomesticLiquidationPriceAsync();
                var logData = await SQLStore_Kho.Instance.GetDomesticLiquidationPriceLogAsync();
                mLogDV = new DataView(logData);

                sku_cbb.DataSource = mSKU_dt;
                sku_cbb.DisplayMember = "ProductNameVN";  // hiển thị tên
                sku_cbb.ValueMember = "SKU";
                sku_cbb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                sku_cbb.TextUpdate += sku_cbb_TextUpdate;

                dataGV.DataSource = mDomesticLiquidationPrice_dt;
                log_GV.DataSource = mLogDV;
                Utils.HideColumns(dataGV, new[] { "DomesticLiquidationPriceID", "SKU", "ProductNameVN_NoSign" });
                Utils.HideColumns(log_GV, new[] { "LogID", "SKU" });

                Utils.SetGridHeaders(dataGV, new System.Collections.Generic.Dictionary<string, string> {
                    {"Name_VN", "Tên Sản Phẩm" },
                    {"Package", "Đ.Vị" },
                    {"SalePrice", "Giá" }
                });

                dataGV.Columns["SalePrice"].DefaultCellStyle.Format = "N0";

                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                updateRightUI();

                Utils.SetTabStopRecursive(this, false);

                int countTab = 0;
                sku_cbb.TabIndex = countTab++; sku_cbb.TabStop = true;               
                salePrice_tb.TabIndex = countTab++; salePrice_tb.TabStop = true;
                LuuThayDoiBtn.TabIndex = countTab++; LuuThayDoiBtn.TabStop = true;

                ReadOnly_btn_Click(null, null);
                dataGV.SelectionChanged += this.dataGV_CellClick;

                await Task.Delay(100);
                loadingOverlay.Hide();

                Utils.SetGridHeaders(log_GV, new System.Collections.Generic.Dictionary<string, string> {
                    {"OldPrice", "Giá Trị Cũ" },
                    {"NewPrice", "Giá Trị Mới" },
                    {"ActionBy", "Người Thực Hiện" },
                    {"CreatedAt", "Ngày Thực Hiện" }
                });

                Utils.SetGridWidths(log_GV, new System.Collections.Generic.Dictionary<string, int> {
                    {"ActionBy", 150},
                    {"CreatedAt", 120}
                });
                log_GV.Columns["OldPrice"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                log_GV.Columns["NewPrice"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;              
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
            updateRightUI();            
        }

        private async void updateProductPrice(int domesticLiquidationPriceID, int SKU, int salePrice)
        {
            DataRow[] rows = mSKU_dt.Select($"SKU = {SKU}");

            foreach (DataRow row in mDomesticLiquidationPrice_dt.Rows)
            {
                int ID = Convert.ToInt32(row["DomesticLiquidationPriceID"]);
                if (ID.CompareTo(domesticLiquidationPriceID) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    string oldValue = $"{row["Name_VN"]} - {row["SalePrice"]}";
                    string newValue = $"{rows[0]["ProductNameVN"]} - {salePrice}";
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager_Kho.Instance.updateDomesticLiquidationPriceAsync(domesticLiquidationPriceID, salePrice);

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                row["SalePrice"] = salePrice;

                                _ = SQLManager_Kho.Instance.InsertDomesticLiquidationPriceLogAsnc(SKU, "Update Success: " + oldValue, newValue);
                            }
                            else
                            {
                                status_lb.Text = "Thất bại.";
                                status_lb.ForeColor = Color.Red;

                                _ = SQLManager_Kho.Instance.InsertDomesticLiquidationPriceLogAsnc(SKU, "Update Fail: " + oldValue, newValue);
                            }
                        }
                        catch (Exception ex)
                        {
                            status_lb.Text = "Thất bại.";
                            status_lb.ForeColor = Color.Red;

                            _ = SQLManager_Kho.Instance.InsertDomesticLiquidationPriceLogAsnc(SKU, "Update Exception: " + ex.Message + oldValue, newValue);
                        }
                    }
                    break;
                }
            }
        }

        private async void createNew(int SKU, int salePrice)
        {
            DataRow[] foundRows = mDomesticLiquidationPrice_dt.Select($"SKU = {SKU}");
            if(foundRows.Length > 0)
            {
                MessageBox.Show("Sản phẩm này đã tồn tại ?", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataRow[] rows = mSKU_dt.Select($"SKU = {SKU}");

            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                string newValue = $"{rows[0]["ProductNameVN"]} - {salePrice}";
                try
                {
                    int newId = await SQLManager_Kho.Instance.insertDomesticLiquidationPriceAsync(SKU, salePrice);
                    if (newId > 0)
                    {
                        DataRow drToAdd = mDomesticLiquidationPrice_dt.NewRow();

                        drToAdd["DomesticLiquidationPriceID"] = newId;
                        drToAdd["SalePrice"] = salePrice;
                        drToAdd["Name_VN"] = rows[0]["ProductNameVN"];
                        drToAdd["SKU"] = SKU;
                        drToAdd["ProductNameVN_NoSign"] = Utils.RemoveVietnameseSigns(rows[0]["ProductNameVN"] + " " + SKU.ToString()).ToLower();

                        string package = rows[0]["Package"].ToString();
                        if (package.CompareTo("weight") == 0)
                            package = "kg";

                        drToAdd["Package"] = package;

                        mDomesticLiquidationPrice_dt.Rows.Add(drToAdd);
                        mDomesticLiquidationPrice_dt.AcceptChanges();

                        status_lb.Text = "Thành công";
                        status_lb.ForeColor = Color.Green;

                        _ = SQLManager_Kho.Instance.InsertDomesticLiquidationPriceLogAsnc(SKU, "Create Success: ", newValue);

                        newBtn_Click(null, null);
                    }
                    else
                    {
                        status_lb.Text = "Thất bại";
                        status_lb.ForeColor = Color.Red;
                        _ = SQLManager_Kho.Instance.InsertDomesticLiquidationPriceLogAsnc(SKU, "Create Fail: ", newValue);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR: " + ex.Message);
                    status_lb.Text = "Thất bại.";
                    status_lb.ForeColor = Color.Red;

                    _ = SQLManager_Kho.Instance.InsertDomesticLiquidationPriceLogAsnc(SKU, "Create Exception: " + ex.Message, newValue);
                }

            }
        }
        private void saveBtn_Click(object sender, EventArgs e)
        {
            if (sku_cbb.SelectedValue == null) return;

            int sku = Convert.ToInt32(sku_cbb.SelectedValue);
            DataRowView productSKUData = (DataRowView)sku_cbb.SelectedItem;
            string productName = productSKUData["ProductNameVN"].ToString();

            int rawPrice = string.IsNullOrWhiteSpace(salePrice_tb.Text) ? 0 : int.Parse(salePrice_tb.Text);

            if (id_tb.Text.Length != 0)
                updateProductPrice(Convert.ToInt32(id_tb.Text), sku, rawPrice);
            else
                createNew(sku, rawPrice);

        }
        private async void deleteProduct()
        {
            string id = id_tb.Text;

            foreach (DataRow row in mDomesticLiquidationPrice_dt.Rows)
            {
                string domesticLiquidationPriceID = row["DomesticLiquidationPriceID"].ToString();
                if (domesticLiquidationPriceID.CompareTo(id) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Xóa Nha, Chắc Chắn Chưa!", "Thông Báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (dialogResult == DialogResult.Yes)
                    {
                        int SKU = Convert.ToInt32(row["SKU"]);
                        string oldValue = $"{row["Name_VN"]} - {row["SalePrice"]}";
                        try
                        {
                            bool isScussess = await SQLManager_Kho.Instance.deletetDomesticLiquidationPriceAsync(Convert.ToInt32(id));

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                mDomesticLiquidationPrice_dt.Rows.Remove(row);
                                mDomesticLiquidationPrice_dt.AcceptChanges();

                                _ = SQLManager_Kho.Instance.InsertDomesticLiquidationPriceLogAsnc(SKU, "Delete Success: " + oldValue, "");
                            }
                            else
                            {
                                status_lb.Text = "Thất bại.";
                                status_lb.ForeColor = Color.Red;
                                _ = SQLManager_Kho.Instance.InsertDomesticLiquidationPriceLogAsnc(SKU, "Delete Fail: " + oldValue, "");
                            }
                        }
                        catch (Exception ex)
                        {
                            status_lb.Text = "Thất bại.";
                            status_lb.ForeColor = Color.Red;
                            _ = SQLManager_Kho.Instance.InsertDomesticLiquidationPriceLogAsnc(SKU, "Delete Fail: " + oldValue, "Exception: " + ex.Message);
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
            salePrice_tb.Text = "";        

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
            if (dataGV.SelectedRows.Count > 0)
                updateRightUI();

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

            RightUiReadOnly(false);
            RightUiEnable(false);
        }

        private void updateRightUI()
        {
            try
            {
                if (isNewState) return;

                if (dataGV.SelectedRows.Count > 0)
                {
                    var cells = dataGV.SelectedRows[0].Cells;

                    string ID = cells["DomesticLiquidationPriceID"].Value.ToString();
                    string SKU = cells["SKU"].Value.ToString();
                    string salePrice = cells["SalePrice"].Value.ToString();

                    if (!sku_cbb.Items.Cast<object>().Any(i => ((DataRowView)i)["SKU"].ToString() == SKU))
                    {
                        sku_cbb.DataSource = mSKU_dt;
                    }
                    sku_cbb.SelectedValue = SKU;

                    id_tb.Text = ID;
                    salePrice_tb.Text = salePrice;

                    status_lb.Text = "";

                    mLogDV.RowFilter = $"SKU = {SKU}";
                    mLogDV.Sort = "LogID DESC";
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
            salePrice_tb.ReadOnly = isReadOnly;
        }

        private void RightUiEnable(bool enable)
        {
        }

    }
}
