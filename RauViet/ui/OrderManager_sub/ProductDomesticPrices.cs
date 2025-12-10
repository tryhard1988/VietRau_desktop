using DocumentFormat.OpenXml.Drawing.Charts;
using RauViet.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RauViet.ui
{
    public partial class ProductDomesticPrices : Form
    {
        System.Data.DataTable mSKU_dt, mProductDomesticPrices_dt;
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
            dataGV.SelectionChanged += this.dataGV_CellClick;

            debounceTimer.Tick += DebounceTimer_Tick;

            edit_btn.Click += Edit_btn_Click;
            readOnly_btn.Click += ReadOnly_btn_Click;
            this.KeyDown += ProductList_KeyDown;

        }

        private void ProductList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                SQLStore.Instance.removeProductpacking();
                SQLStore.Instance.removeProductSKU();
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

                    deletePackingProduct();
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
                mSKU_dt = await SQLStore.Instance.getProductSKUAsync(parameters);
                mProductDomesticPrices_dt = await SQLStore.Instance.getProductDomesticPricesAsync();

                //foreach (DataColumn col in mProductDomesticPrices_dt.Columns)
                //    col.ReadOnly = false;

                sku_cbb.DataSource = mSKU_dt;
                sku_cbb.DisplayMember = "ProductNameVN";  // hiển thị tên
                sku_cbb.ValueMember = "SKU";
                sku_cbb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                sku_cbb.TextUpdate += sku_cbb_TextUpdate;

                dataGV.DataSource = mProductDomesticPrices_dt;
                DataView dv = mProductDomesticPrices_dt.DefaultView;
                dv.RowFilter = $"IsActive = true";

                //dataGV.Columns["ProductPackingID"].Visible = false;
                //dataGV.Columns["IsActive_SKU"].Visible = false;
                //// dataGV.Columns["Amount"].Visible = false;
                //// dataGV.Columns["Packing"].Visible = false;
                //dataGV.Columns["PriceCNF"].Visible = false;
                //dataGV.Columns["SKU"].Visible = false; 
                //dataGV.Columns["ProductNameVN_NoSign"].Visible = false;
                //dataGV.Columns["PackingName"].Visible = false;
                //dataGV.Columns["Package"].Visible = false;
                //dataGV.Columns["Amount"].Visible = false;
                dataGV.Columns["PriceID"].Visible = false;

                dataGV.Columns["ProductName_VN"].HeaderText = "Tên Sản Phẩm";
                dataGV.Columns["RawPrice"].HeaderText = "Giá Hàng Xá";
                dataGV.Columns["RefinedPrice"].HeaderText = "Giá Hàng Tinh";
                dataGV.Columns["PackedPrice"].HeaderText = "Giá Hàng Đóng Gói";
                dataGV.Columns["IsActive"].HeaderText = "Active";


                //dataGV.Columns["PLU"].Width = 50;
                //dataGV.Columns["BarCode"].Width = 90;
                //dataGV.Columns["BarCodeEAN13"].Width = 90;
                //dataGV.Columns["ArtNr"].Width = 50;
                //dataGV.Columns["GGN"].Width = 90;
                //dataGV.Columns["Name_VN"].Width = 200;
                //dataGV.Columns["Name_EN"].Width = 200;
                //dataGV.Columns["Priority"].Width = 50;
                //dataGV.Columns["IsActive"].Width = 30;


                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

               // dataGV.Columns["Packing"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                updateDataTextBoxFlowSKU();

                Utils.SetTabStopRecursive(this, false);

                int countTab = 0;
                sku_cbb.TabIndex = countTab++; sku_cbb.TabStop = true;               
                rawPrice_tb.TabIndex = countTab++; rawPrice_tb.TabStop = true;
                refinedPrice_tb.TabIndex = countTab++; refinedPrice_tb.TabStop = true;
                packedPrice_tb.TabIndex = countTab++; packedPrice_tb.TabStop = true;
                LuuThayDoiBtn.TabIndex = countTab++; LuuThayDoiBtn.TabStop = true;

                ReadOnly_btn_Click(null, null);
            }
            catch (Exception ex)
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
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager.Instance.updateProductDomesticPriceAsync(SKU, rawPrice, refinePrice, packedPrice, isActive);

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                row["RawPrice"] = rawPrice;
                                row["RefinedPrice"] = refinePrice;
                                row["PackedPrice"] = packedPrice;
                                row["IsActive"] = isActive;
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
                try
                {
                    int newId = await SQLManager.Instance.insertProductDomesticPriceAsync(SKU, rawPrice, refinePrice, packedPrice);
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


                        newBtn_Click(null, null);
                    }
                    else
                    {
                        status_lb.Text = "Thất bại";
                        status_lb.ForeColor = Color.Red;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR: " + ex.Message);
                    status_lb.Text = "Thất bại.";
                    status_lb.ForeColor = Color.Red;
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
        private async void deletePackingProduct()
        {
            //string id = id_tb.Text;

            //foreach (DataRow row in packing_dt.Rows)
            //{
            //    string productPackingID = row["ProductPackingID"].ToString();
            //    if (productPackingID.CompareTo(id) == 0)
            //    {
            //        DialogResult dialogResult = MessageBox.Show("Xóa Nha, Chắc Chắn Chưa!", "Thông Báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            //        if (dialogResult == DialogResult.Yes)
            //        {
            //            try
            //            {
            //                bool isScussess = await SQLManager.Instance.deleteProductpackingAsync(Convert.ToInt32(id));

            //                if (isScussess == true)
            //                {
            //                    status_lb.Text = "Thành công.";
            //                    status_lb.ForeColor = Color.Green;

            //                    packing_dt.Rows.Remove(row);
            //                    packing_dt.AcceptChanges();
            //                }
            //                else
            //                {
            //                    status_lb.Text = "Thất bại.";
            //                    status_lb.ForeColor = Color.Red;
            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                status_lb.Text = "Thất bại.";
            //                status_lb.ForeColor = Color.Red;
            //            }
            //        }
            //        break;
            //    }
            //}

        }

        private void newBtn_Click(object sender, EventArgs e)
        {
            sku_cbb.SelectedIndex = -1;
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

        //private void sku_cbb_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (sku_cbb.SelectedItem != null)
        //    {
        //        DataRowView productSKUData = (DataRowView)sku_cbb.SelectedItem;

        //        if (productSKUData["PackingList"].ToString().CompareTo("") != 0)
        //        {
        //            string[] arr = productSKUData["PackingList"].ToString().Split('-');
        //            int top = 00;

        //            foreach (var item in arr)
        //            {
        //                RadioButton rb = new RadioButton();
        //                rb.Text = item;
        //                rb.Name = item;
        //                rb.Location = new Point(top, 5);
        //                rb.AutoSize = true;
        //                top += 70;
        //                rb.Checked = true;
        //             //   rb.TabIndex = sku_cbb.TabIndex + 1;
        //            }

        //            if (dataGV.SelectedRows.Count > 0)
        //            {
        //                foreach (var item in arr)
        //                {
        //                    foreach (Control ctrl in packing_panel.Controls)
        //                    {
        //                        if (ctrl is RadioButton rb)
        //                        {
        //                            var rb1 = (RadioButton)ctrl; //; ;.Checked = true;
        //                            if (item.Equals(rb1.Name, StringComparison.OrdinalIgnoreCase))
        //                            {
        //                                rb.Checked = true;
        //                                break;
        //                            }
        //                        }
        //                    }

        //                }
        //            }
        //        }
        //    }
        //}

        private void updateDataTextBoxFlowSKU()
        {
            try
            {
                if (isNewState) return;

                if (dataGV.SelectedRows.Count > 0)
                {
                    var cells = dataGV.SelectedRows[0].Cells;

                    string ID = cells["ProductPackingID"].Value.ToString();
                    string SKU = cells["SKU"].Value.ToString();
                    string barcode = cells["BarCode"].Value.ToString();
                    string PLU = cells["PLU"].Value.ToString();
                    string nameVN = cells["Name_VN"].Value.ToString();
                    string nameEN = cells["Name_EN"].Value.ToString();
                    string amount = cells["Amount"].Value.ToString();
                    string priceCNF = cells["PriceCNF"].Value.ToString();
                    string barCodeEAN13 = cells["BarCodeEAN13"].Value.ToString();
                    string artNr = cells["ArtNr"].Value.ToString();
                    string GGN = cells["GGN"].Value.ToString();
                    bool isActive = Convert.ToBoolean(cells["IsActive"].Value);

                    if (!sku_cbb.Items.Cast<object>().Any(i => ((DataRowView)i)["SKU"].ToString() == SKU))
                    {
                        sku_cbb.DataSource = mSKU_dt;
                    }
                    sku_cbb.SelectedValue = SKU;
                    DataRowView productSKUData = (DataRowView)sku_cbb.SelectedItem;
                    if (productSKUData == null) return;

                    id_tb.Text = ID;
                    rawPrice_tb.Text = barCodeEAN13;
                    refinedPrice_tb.Text = artNr;
                    packedPrice_tb.Text = GGN;
                    isActive_cb.Checked = isActive;

                    if (productSKUData["PackingList"].ToString().CompareTo("") != 0)
                    {
                        string[] arr = productSKUData["PackingList"].ToString().Split('-');
                        int top = 00;

                        foreach (var item in arr)
                        {
                            RadioButton rb = new RadioButton();
                            rb.Text = item;
                            rb.Location = new Point(top, 5);
                            rb.AutoSize = true;

                            if (item.Equals(cells["Packing"].Value.ToString(), StringComparison.OrdinalIgnoreCase))
                            {
                                rb.Checked = true;
                            }

                            top += 70;
                        }
                    }

                    status_lb.Text = "";
                }
            }
            catch { };
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
