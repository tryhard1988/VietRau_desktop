using System;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using RauViet.classes;

namespace RauViet.ui
{
    public partial class ProductSKU : Form
    {
        bool isNewState = false;
        private LoadingOverlay loadingOverlay;
        DataTable mPoductSKUHistory_dt, mProductSKU_dt;
        public ProductSKU()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            Utils.SetTabStopRecursive(this, false);
            int countTab = 0;
            product_VN_tb.TabIndex = countTab++; product_VN_tb.TabStop = true;
            product_EN_tb.TabIndex = countTab++; product_EN_tb.TabStop = true;
            package_tb.TabIndex = countTab++; package_tb.TabStop = true;
            packing_tb.TabIndex = countTab++; packing_tb.TabStop = true;
            botanicalName_tb.TabIndex = countTab++; botanicalName_tb.TabStop = true;
            priceCNF_tb.TabIndex = countTab++; priceCNF_tb.TabStop = true;
            plantingareaCode_tb.TabIndex = countTab++; plantingareaCode_tb.TabStop = true;
            lotCodeHeader_tb.TabIndex = countTab++; lotCodeHeader_tb.TabStop = true;
            priority_tb.TabIndex = countTab++; priority_tb.TabStop = true;
            luuBtn.TabIndex = countTab++; luuBtn.TabStop = true;


            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            status_lb.Text = "";

            newBtn.Click += newBtn_Click;
            luuBtn.Click += saveBtn_Click;
            dataGV.SelectionChanged += this.dataGV_CellClick;
            priceCNF_tb.KeyPress += Tb_KeyPress_OnlyNumber;
            priority_tb.KeyPress += Tb_KeyPress_OnlyNumber;
            lotCodeHeader_tb.KeyPress += Tb_KeyPress_OnlyNumber;
            search_tb.TextChanged += search_txt_TextChanged;

            edit_btn.Click += Edit_btn_Click;
            readOnly_btn.Click += ReadOnly_btn_Click;
            this.KeyDown += ProductSKU_KeyDown;
            sku_tb.Visible = false;
        }

        private void ProductSKU_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                SQLStore.Instance.removeProductSKUHistory();
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

                    deleteProduct();
                }
            }
        }

        public async void ShowData()
        {
            await Task.Delay(50);
            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Show();
            await Task.Delay(50);
            try
            {
                // Chạy truy vấn trên thread riêng
                var productSKUATask = SQLStore.Instance.getProductSKUAsync();
                var productSKUHistoryTask = SQLStore.Instance.GetProductSKUHistoryAsync();

                await Task.WhenAll(productSKUATask, productSKUHistoryTask);
                mProductSKU_dt = productSKUATask.Result;
                mPoductSKUHistory_dt = productSKUHistoryTask.Result;

                int count = 0;
                mProductSKU_dt.Columns["SKU"].SetOrdinal(count++);
                mProductSKU_dt.Columns["ProductNameVN"].SetOrdinal(count++);
                mProductSKU_dt.Columns["ProductNameEN"].SetOrdinal(count++);
                mProductSKU_dt.Columns["PackingType"].SetOrdinal(count++);
                mProductSKU_dt.Columns["Package"].SetOrdinal(count++);
                mProductSKU_dt.Columns["PackingList"].SetOrdinal(count++);
                mProductSKU_dt.Columns["BotanicalName"].SetOrdinal(count++);
                mProductSKU_dt.Columns["PriceCNF"].SetOrdinal(count++);
                mProductSKU_dt.Columns["PlantingAreaCode"].SetOrdinal(count++);
                mProductSKU_dt.Columns["LOTCodeHeader"].SetOrdinal(count++);
                mProductSKU_dt.Columns["Priority"].SetOrdinal(count++);
                priceCNFHisGV.DataSource = mPoductSKUHistory_dt;
                dataGV.DataSource = mProductSKU_dt;                

                dataGV.Columns["ProductNameVN"].HeaderText = "Tên Sản Phẩm (VN)";
                dataGV.Columns["ProductNameEN"].HeaderText = "Tên Sản Phẩm (EN)";
                dataGV.Columns["BotanicalName"].HeaderText = "Botanical Name";
                dataGV.Columns["PriceCNF"].HeaderText = "Giá CNF\n(CHF/Kg)";
                dataGV.Columns["Priority"].HeaderText = "Ưu\nTiên";
                dataGV.Columns["PackingList"].HeaderText = "Packing\nList";
                dataGV.Columns["PackingType"].HeaderText = "Packing\nType";
                dataGV.Columns["PlantingAreaCode"].HeaderText = "Mã\nVùng Trồng";
                dataGV.Columns["LOTCodeHeader"].HeaderText = "Mã LOT\n3 số đầu";
                dataGV.Columns["IsActive"].HeaderText = "";

                dataGV.Columns["ProductNameVN"].Width = 170;
                dataGV.Columns["ProductNameEN"].Width = 140;

                dataGV.Columns["BotanicalName"].Width = 100;
                dataGV.Columns["PlantingAreaCode"].Width = 50;
                dataGV.Columns["LOTCodeHeader"].Width = 50;
                dataGV.Columns["PriceCNF"].Width = 60;
                dataGV.Columns["Priority"].Width = 35;
                dataGV.Columns["package"].Width = 60;
                dataGV.Columns["PackingType"].Width = 60;
                dataGV.Columns["PackingList"].Width = 60;
                dataGV.Columns["SKU"].Width = 60;
                dataGV.Columns["IsActive"].Width = 30;

                dataGV.Columns["ProductNameVN_NoSign"].Visible = false;
                dataGV.Columns["PackingType"].Visible = false;
                dataGV.Columns["GroupProduct"].Visible = false;
                dataGV.Columns["PriceCNF"].Visible = !UserManager.Instance.hasRole_AnGiaSanPham();
                priceCNFHisGV.Columns["id"].Visible = false;
                priceCNFHisGV.Columns["SKU"].Visible = false;
                priceCNFHisGV.Visible = !UserManager.Instance.hasRole_AnGiaSanPham();

                // dataGV.Columns["SKU"].Visible = false;

                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGV.Columns["PriceCNF"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGV.Columns["package"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGV.Columns["PackingType"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGV.Columns["Priority"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGV.Columns["PackingList"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGV.Columns["SKU"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                //dataGV.AutoResizeColumns();

                search_txt_TextChanged(null, null);

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
                
        private void dataGV_CellClick(object sender, EventArgs e)
        {
            if (dataGV.CurrentRow == null) 
                return;

            int rowIndex = dataGV.CurrentRow.Index;
            if (rowIndex < 0)
                return;


            UpdateRightUI(rowIndex);
        }

        private void UpdateRightUI(int index)
        {
            if (isNewState) return;

            var cells = dataGV.Rows[index].Cells;

            string SKU = cells["SKU"].Value.ToString();
            string productNameVN = cells["ProductNameVN"].Value.ToString();
            string productNameEN = cells["ProductNameEN"].Value.ToString();
            string packingType = cells["PackingType"].Value.ToString();
            string package = cells["Package"].Value.ToString();
            string packingList = cells["PackingList"].Value.ToString();
            string botanicalName = cells["BotanicalName"].Value.ToString();
            string PriceCNF = cells["PriceCNF"].Value.ToString();
            string priority = cells["Priority"].Value.ToString();
            string plantingAreaCode = cells["PlantingAreaCode"].Value.ToString();
            string lotCodeHeader = cells["LOTCodeHeader"].Value.ToString();
            bool isActive = Convert.ToBoolean(cells["IsActive"].Value);

            sku_tb.Text = SKU;
            product_VN_tb.Text = productNameVN;
            product_EN_tb.Text = productNameEN;
            package_tb.Text = package;
            packingType_tb.Text = packingType;
            packing_tb.Text = packingList;
            botanicalName_tb.Text = botanicalName;
            priceCNF_tb.Text = PriceCNF;
            priority_tb.Text = priority;
            plantingareaCode_tb.Text = plantingAreaCode;
            lotCodeHeader_tb.Text = lotCodeHeader;
            isActive_cb.Checked = isActive;

            DataView dv = new DataView(mPoductSKUHistory_dt);
            dv.RowFilter = $"SKU = '{SKU}'";

            priceCNFHisGV.DataSource = dv;

            priceCNF_tb.Visible = !UserManager.Instance.hasRole_AnGiaSanPham();

            status_lb.Text = "";
        }


       

        private async void updateProductSKU(int SKU, string productNameVN, string productNameEN, string packingType, string package, 
            string packingList, string botanicalName, decimal priceCNF, int priority, string plantingareaCode, string LOTCodeHeader, bool isActive)
        {
            foreach (DataRow row in mProductSKU_dt.Rows)
            {
                int _SKU =Convert.ToInt32(row["SKU"]);
                if (_SKU.CompareTo(SKU) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager.Instance.updateProductSKUAsync(SKU, productNameVN, productNameEN, packingType, package, 
                                packingList, botanicalName, priceCNF, priority, plantingareaCode, LOTCodeHeader, isActive);

                            if (isScussess == true)
                            {
                                SQLStore.Instance.removeProductpacking();

                                await SQLManager.Instance.SaveProductSKUHistoryAsync(SKU, priceCNF);

                                row["ProductNameVN"] = productNameVN;
                                row["ProductNameEN"] = productNameEN;
                                row["PackingType"] = packingType;
                                row["Package"] = package;
                                row["PackingList"] = packingList;
                                row["BotanicalName"] = botanicalName;
                                row["PriceCNF"] = priceCNF;
                                row["Priority"] = priority;
                                row["PlantingAreaCode"] = plantingareaCode;
                                row["LOTCodeHeader"] = LOTCodeHeader;
                                row["IsActive"] = isActive;
                                row["ProductNameVN_NoSign"] = Utils.RemoveVietnameseSigns(productNameVN + " " + SKU).ToLower(); ;
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;
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

        private async void createNewProductSKU(string productNameVN, string productNameEN, string packingType, string package, 
            string packingList, string botanicalName, decimal priceCNF, int priority, string plantingareaCode, string LOTCodeHeader)
        {
            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    int newId = await SQLManager.Instance.insertProductSKUAsync(productNameVN, productNameEN, packingType, package, 
                        packingList, botanicalName, priceCNF, priority, plantingareaCode, LOTCodeHeader);
                    if (newId > 0)
                    {
                        SQLStore.Instance.removeProductpacking();

                        await SQLManager.Instance.SaveProductSKUHistoryAsync(newId, priceCNF);

                        DataTable dataTable = (DataTable)dataGV.DataSource;
                        DataRow drToAdd = dataTable.NewRow();

                        drToAdd["SKU"] = newId;
                        drToAdd["GroupProduct"] = newId;
                        drToAdd["ProductNameVN"] = productNameVN;
                        drToAdd["ProductNameEN"] = productNameEN;
                        drToAdd["PackingType"] = packingType;
                        drToAdd["Package"] = package;
                        drToAdd["PackingList"] = packingList;
                        drToAdd["BotanicalName"] = botanicalName;
                        drToAdd["PriceCNF"] = priceCNF;
                        drToAdd["Priority"] = priority;
                        drToAdd["PlantingAreaCode"] = plantingareaCode;
                        drToAdd["LOTCodeHeader"] = LOTCodeHeader;
                        drToAdd["isActive"] = true;
                        drToAdd["ProductNameVN_NoSign"] = Utils.RemoveVietnameseSigns(productNameVN + " " + newId).ToLower(); ;
                        sku_tb.Text = newId.ToString();


                        dataTable.Rows.Add(drToAdd);
                        dataTable.AcceptChanges();

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
                    status_lb.Text = "Thất bại.";
                    status_lb.ForeColor = Color.Red;
                }
                
            }
        }
        private async void saveBtn_Click(object sender, EventArgs e)
        {
            if(package_tb.Text.CompareTo("") == 0 ||
                product_EN_tb.Text.CompareTo("") == 0 || product_VN_tb.Text.CompareTo("") == 0 || 
                priceCNF_tb.Text.CompareTo("") == 0 || priority_tb.Text.CompareTo("") == 0)
            {
                MessageBox.Show(
                                "Thiếu Dữ Liệu, Kiểm Tra Lại!",
                                "Thông Báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning
                            );
                return;
            }
                
            String packingList = packing_tb.Text;
            packingList = packingList.Replace(" ", "").ToLower();
            if (packingList.EndsWith("-"))
            {
                packingList = packingList.Substring(0, packingList.Length - 1);
            }
            string productNameVN = product_VN_tb.Text;
            string productNameEN = product_EN_tb.Text;
            string packingType = packingType_tb.Text;
            string plantingareaCode = plantingareaCode_tb.Text;
            string lotCode = lotCodeHeader_tb.Text;
            string package = package_tb.Text.Replace(" ", "").ToLower();
            bool isActive = isActive_cb.Checked;

            if (sku_tb.Text.Length != 0)
                updateProductSKU(Convert.ToInt32(sku_tb.Text), productNameVN, productNameEN, packingType, package, packingList, botanicalName_tb.Text,
                    Convert.ToDecimal(priceCNF_tb.Text), Convert.ToInt32(priority_tb.Text), plantingareaCode, lotCode, isActive);
            else
            {
                createNewProductSKU(productNameVN, productNameEN, packingType, package,
                    packingList, botanicalName_tb.Text, Convert.ToDecimal(priceCNF_tb.Text), Convert.ToInt32(priority_tb.Text), plantingareaCode, lotCode);
            }
            info_gb.BackColor = Color.Gray;
           // await SQLStore.Instance.getProductpackingAsync(true);

        }
        private async void deleteProduct()
        {
            if (dataGV.SelectedRows.Count == 0) return;

            string SKU = sku_tb.Text;

            foreach (DataRow row in mProductSKU_dt.Rows)
            {
                string maKH = row["SKU"].ToString();
                if (maKH.CompareTo(SKU) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("XÓA THÔNG TIN KHÁCH HÀNG ĐÓ NHA \n Chắc chắn chưa ?", " Xóa Thông Tin Khách Hàng", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager.Instance.deleteProductSKUAsync(Convert.ToInt32(SKU));

                            if (isScussess == true)
                            {
                                SQLStore.Instance.removeProductpacking();
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                mProductSKU_dt.Rows.Remove(row);
                                mProductSKU_dt.AcceptChanges();
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

            await SQLStore.Instance.getProductpackingAsync(true);

        }

        private void newBtn_Click(object sender, EventArgs e)
        {
            sku_tb.Text = "";
            product_VN_tb.Text = "";
            product_EN_tb.Text = "";
            package_tb.Text = "kg";
            packing_tb.Text = "kg-gr";
            botanicalName_tb.Text = "";
            priceCNF_tb.Text = "0";
            status_lb.Text = "";
            plantingareaCode_tb.Text = "";
            lotCodeHeader_tb.Text = "";

            product_VN_tb.Focus();
            info_gb.BackColor = newBtn.BackColor;
            edit_btn.Visible = false;
            newBtn.Visible = false;
            readOnly_btn.Visible = true;
            luuBtn.Visible = true;
            isNewState = true;
            luuBtn.Text = "Lưu Mới";
            isActive_cb.Visible = false;
            RightUIReadOnly(false);
            RightUIEnable(true);
        }

        private void ReadOnly_btn_Click(object sender, EventArgs e)
        {
            edit_btn.Visible = true;
            newBtn.Visible = true;
            readOnly_btn.Visible = false;
            luuBtn.Visible = false;
            info_gb.BackColor = Color.DarkGray;
            isNewState = false;
            isActive_cb.Visible = true;
            RightUIReadOnly(true);

            if (dataGV.SelectedRows.Count > 0)
                UpdateRightUI(0);
            RightUIEnable(true);
        }

        private void Edit_btn_Click(object sender, EventArgs e)
        {
            edit_btn.Visible = false;
            newBtn.Visible = false;
            readOnly_btn.Visible = true;
            luuBtn.Visible = true;
            info_gb.BackColor = edit_btn.BackColor;
            isNewState = false;
            luuBtn.Text = "Lưu C.Sửa";
            isActive_cb.Enabled = true;

            RightUIReadOnly(false);
            RightUIEnable(false);
        }

        private void Tb_KeyPress_OnlyNumber(object sender, KeyPressEventArgs e)
        {
            TextBox tb = sender as TextBox;

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

        private void search_txt_TextChanged(object sender, EventArgs e)
        {
            string keyword = Utils.RemoveVietnameseSigns(search_tb.Text.Trim().ToLower())
                     .Replace("'", "''"); // tránh lỗi cú pháp '

            DataTable dt = dataGV.DataSource as DataTable;
            if (dt == null) return;

            DataView dv = dt.DefaultView;
            dv.RowFilter = $"[ProductNameVN_NoSign] LIKE '%{keyword}%'";

        }

        private void RightUIReadOnly(bool isReadOnly)
        {
            product_VN_tb.ReadOnly = isReadOnly;
            product_EN_tb.ReadOnly = isReadOnly;
            package_tb.ReadOnly = isReadOnly;
            packing_tb.ReadOnly = isReadOnly;
            botanicalName_tb.ReadOnly = isReadOnly;
            priceCNF_tb.ReadOnly = isReadOnly;
            plantingareaCode_tb.ReadOnly = isReadOnly;
            lotCodeHeader_tb.ReadOnly = isReadOnly;
            priority_tb.ReadOnly = isReadOnly;
            isActive_cb.Enabled = !isReadOnly;
            
        }

        private void RightUIEnable(bool enable)
        {
            product_VN_tb.Enabled = enable;
            product_EN_tb.Enabled = enable;
            package_tb.Enabled = enable;
            packing_tb.Enabled = enable;
            botanicalName_tb.Enabled = enable;
        }

    }
}
