using DocumentFormat.OpenXml.Wordprocessing;
using RauViet.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RauViet.ui
{
    public partial class OrderDomesticDetail : Form
    {
        System.Data.DataTable mProductPacking_dt, mProduct_dt, mOrderDomesticCode_dt, mOrderDomesticDetail_dt, mProductType_dt;
        DataView mLogDV;
        private Timer debounceTimer = new Timer { Interval = 150 };
        private LoadingOverlay loadingOverlay;
        bool isNewState = false;
        bool ispackingState = false;
        object oldValue;
        int mCurrentOrderDomesticCodeID = -1;
        // private DataView dvProducts;
        public OrderDomesticDetail()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            Utils.SetTabStopRecursive(this, false);

            int countTab = 0;
            product_ccb.TabIndex = countTab++; product_ccb.TabStop = true;
            productType_CBB.TabIndex = countTab++; productType_CBB.TabStop = true;
            packing_ccb.TabIndex = countTab++; packing_ccb.TabStop = true;
            PCSOrder_tb.TabIndex = countTab++; PCSOrder_tb.TabStop = true;
            nwOder_tb.TabIndex = countTab++; nwOder_tb.TabStop = true;
            LuuThayDoiBtn.TabIndex = countTab++; LuuThayDoiBtn.TabStop = true;

            dataGV.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dataGV.MultiSelect = false;

            status_lb.Text = "";

            newCustomerBtn.Click += newCustomerBtn_Click;
            LuuThayDoiBtn.Click += saveBtn_Click;
            
            preview_print_TD_btn.Click += preview_print_TD_btn_Click;            
            printPendingOrderSummary_btn.Click += printPendingOrderSummary_btn_Click;
            PGH_btn.Click += PGH_btn_Click;
            PGH_preview_btn.Click += PGH_preview_btn_Click;

            dataGV.SelectionChanged += this.dataGV_CellClick;
            PCSOrder_tb.TextChanged += PCSOrder_tb_TextChanged;
            packing_ccb.SelectedIndexChanged += packing_ccb_SelectedIndexChanged;
            
            PCSOrder_tb.KeyPress += Tb_KeyPress_OnlyNumber1;
            nwOder_tb.KeyPress += Tb_KeyPress_OnlyNumber;

            debounceTimer.Tick += DebounceTimer_Tick;
            edit_btn.Click += Edit_btn_Click;
            readOnly_btn.Click += ReadOnly_btn_Click;
            packing_btn.Click += Packing_btn_Click;

            this.KeyDown += OrdersList_KeyDown;

            packing_ccb.SelectedIndexChanged += packing_cbb_SelectedIndexChanged;

            dataGV.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dataGV_EditingControlShowing);
            dataGV.CellBeginEdit += dataGV_CellBeginEdit;
            dataGV.CellEndEdit += dataGV_CellEndEdit;
            dataGV.CellFormatting += dataGV_CellFormatting;
            dataGV.CellParsing += dataGV_CellParsing;

        }

        private void OrdersList_KeyDown(object sender, KeyEventArgs e)
        {
            DataRowView dataR = (DataRowView)orderDomesticCode_cbb.SelectedItem;
            string staff = dataR["InputByName_NoSign"].ToString();

            if (e.KeyCode == Keys.F5)
            {
                if (mCurrentOrderDomesticCodeID <= 0)
                {
                    return;
                }
                
                SQLStore.Instance.removeOrderDomesticDetail(mCurrentOrderDomesticCodeID);
                ShowData();
            }
            else if(!isNewState && !edit_btn.Visible /* && UserManager.Instance.fullName_NoSign.CompareTo(staff) == 0*/)
            {
                if (e.KeyCode == Keys.Delete)
                {
                    System.Windows.Forms.Control ctrl = this.ActiveControl;

                    if (ctrl is TextBox || ctrl is RichTextBox ||
                        (ctrl is DataGridView dgv && dgv.CurrentCell != null && dgv.IsCurrentCellInEditMode))
                    {
                        return; // không xử lý Delete
                    }

                    deleteOrderID();
                }
            }
        }

        public async void ShowData()
        {
            await Task.Delay(50);
            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Show();
            await Task.Delay(50);

            orderDomesticCode_cbb.SelectedIndexChanged -= exportCode_search_cbb_SelectedIndexChanged;
            try
            {
                mProduct_dt = await SQLStore.Instance.getProductDomesticPricesAsync();

                var parameters = new Dictionary<string, object> { { "Complete", false } };
                var orderDomesticCode_dtTask = SQLStore.Instance.getOrderDomesticCodeAsync(parameters);
                var productTypeTask = SQLStore.Instance.getProductTypeAsync();
                var packingTask = SQLStore.Instance.getProductpackingAsync();          
                await Task.WhenAll(packingTask, orderDomesticCode_dtTask, productTypeTask);              
                mProductPacking_dt = packingTask.Result;
                mOrderDomesticCode_dt = orderDomesticCode_dtTask.Result;
                mProductType_dt = productTypeTask.Result;

                if (mCurrentOrderDomesticCodeID <= 0 && mOrderDomesticCode_dt.Rows.Count > 0)
                {
                    mCurrentOrderDomesticCodeID = Convert.ToInt32(mOrderDomesticCode_dt.AsEnumerable().Max(r => r.Field<int>("OrderDomesticCodeID")));
                }

                var orderLogTask = SQLStore.Instance.GetOrderDomesticDetailLogAsync(mCurrentOrderDomesticCodeID);
                var othersTask = SQLStore.Instance.getOrderDomesticDetailAsync(mCurrentOrderDomesticCodeID);
              //  var latestOrdersTask = SQLStore.Instance.get3LatestOrdersAsync();
                await Task.WhenAll(othersTask, orderLogTask);

                mOrderDomesticDetail_dt = othersTask.Result;

                product_ccb.DataSource = mProduct_dt;
                product_ccb.DisplayMember = "ProductName_VN";
                product_ccb.ValueMember = "SKU";
                product_ccb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                product_ccb.AutoCompleteSource = AutoCompleteSource.ListItems;
                product_ccb.SelectedIndexChanged += product_ccb_SelectedIndexChanged;
                product_ccb.TextUpdate += product_ccb_TextUpdate;

                productType_CBB.DataSource = mProductType_dt;
                productType_CBB.DisplayMember = "TypeName";
                productType_CBB.ValueMember = "CustomerProductTypesCode";

                foreach (DataColumn col in mOrderDomesticDetail_dt.Columns)
                    col.ReadOnly = false;

                // Gán testData cho DataGridView tạm để test
                dataGV.DataSource = mOrderDomesticDetail_dt;
                mLogDV = new DataView(orderLogTask.Result);
                log_GV.DataSource = mLogDV;

                dataGV.Columns["Price"].HeaderText = "Giá";
                dataGV.Columns["ProductNameVN"].HeaderText = "Tên Sản Phẩm";
                dataGV.Columns["PCSOrder"].HeaderText = "PCS\nĐặt Hàng";
                dataGV.Columns["NWOrder"].HeaderText = "NW\nĐặt Hàng";
                dataGV.Columns["PCSReal"].HeaderText = "PCS\nGiao Hàng";
                dataGV.Columns["NWReal"].HeaderText = "NW\nGiao Hàng";
                dataGV.Columns["OrderDomesticDetailID"].HeaderText = "ID";
                dataGV.Columns["ProductTypeName"].HeaderText = "Loại Hàng";

                dataGV.Columns["packing"].Visible = false;
                dataGV.Columns["Package"].Visible = false;
                dataGV.Columns["SKU"].Visible = false;
                dataGV.Columns["ProductPackingID"].Visible = false;
                dataGV.Columns["OrderDomesticCodeID"].Visible = false;
                dataGV.Columns["Amount"].Visible = false;
                dataGV.Columns["CustomerProductTypesCode"].Visible = false;
                dataGV.Columns["BarCodeEAN13"].Visible = false;
                dataGV.Columns["Price"].Visible = !UserManager.Instance.hasRole_AnGiaSanPham();

                dataGV.ReadOnly = false;
                dataGV.Columns["PCSReal"].ReadOnly = false;
                dataGV.Columns["NWReal"].ReadOnly = false;
                dataGV.Columns["PCSOrder"].ReadOnly = true;
                dataGV.Columns["NWOrder"].ReadOnly = true;
                dataGV.Columns["Price"].ReadOnly = true;
                dataGV.Columns["ProductTypeName"].ReadOnly = true;
                dataGV.Columns["ProductNameVN"].ReadOnly = true;
                dataGV.Columns["OrderDomesticDetailID"].ReadOnly = true;
               

                dataGV.Columns["OrderDomesticDetailID"].Width = 50;
                dataGV.Columns["ProductNameVN"].Width = 130;
                dataGV.Columns["PCSOrder"].Width = 70;
                dataGV.Columns["NWOrder"].Width = 70;
                dataGV.Columns["PCSReal"].Width = 70;
                dataGV.Columns["NWReal"].Width = 70;
                dataGV.Columns["Price"].Width = 70;

                dataGV.Columns["Price"].DefaultCellStyle.Format = "N0";

                dataGV.Columns["PCSOrder"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGV.Columns["NWOrder"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGV.Columns["PCSReal"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGV.Columns["NWReal"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGV.Columns["Price"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGV.Columns["OrderDomesticDetailID"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                
                orderDomesticCode_cbb.DataSource = mOrderDomesticCode_dt;
                orderDomesticCode_cbb.DisplayMember = "OrderDomesticIndex";  // hiển thị tên
                orderDomesticCode_cbb.ValueMember = "OrderDomesticCodeID";                
                orderDomesticCode_cbb.SelectedIndexChanged += exportCode_search_cbb_SelectedIndexChanged;
                orderDomesticCode_cbb.SelectedValue = mCurrentOrderDomesticCodeID;


                await Task.Delay(500);
                ReadOnly_btn_Click(null, null);

                log_GV.Columns["LogID"].Visible = false;
                log_GV.Columns["OrderDomesticDetailID"].Visible = false;
                log_GV.Columns["OrderDomesticCodeID"].Visible = false;

                log_GV.Columns["OldValue"].HeaderText = "Giá Trị Cũ";
                log_GV.Columns["NewValue"].HeaderText = "Giá Trị Mới";
                log_GV.Columns["ActionBy"].HeaderText = "Người Thực Hiện";
                log_GV.Columns["CreatedAt"].HeaderText = "Ngày Thực Hiện";

                log_GV.Columns["OldValue"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                log_GV.Columns["NewValue"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                log_GV.Columns["ActionBy"].Width = 140;
                log_GV.Columns["CreatedAt"].Width = 110;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR:" + ex.Message);
                status_lb.Text = "Thất bại.";
                status_lb.ForeColor = System.Drawing.Color.Red;
            }
            finally
            {
                await Task.Delay(200);
                loadingOverlay.Hide();
            }
        }


        private void product_ccb_TextUpdate(object sender, EventArgs e)
        {
            debounceTimer.Stop();
            debounceTimer.Start();
        }

        private void DebounceTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                debounceTimer.Stop();
                string typed = product_ccb.Text ?? "";
                string plain = Utils.RemoveVietnameseSigns(typed).ToLower();

                // Filter bằng LINQ thay vì RowFilter
                var filtered = mProduct_dt.AsEnumerable()
                    .Where(r =>r.Field<bool>("IsActive") == true &&
                               r["ProductNameVN_NoSign"].ToString().Contains(plain))
                    .OrderBy(r => r.Field<int>("Priority"));

                System.Data.DataTable temp;
                if (filtered.Any())
                {
                    temp = filtered.CopyToDataTable();
                    product_ccb.DroppedDown = true;
                }
                else
                {
                    temp = mProduct_dt.Clone(); // nếu không có kết quả thì trả về table rỗng
                    product_ccb.DroppedDown = false;
                }
                product_ccb.DataSource = temp;
                product_ccb.DisplayMember = "ProductNameVN";
                product_ccb.ValueMember = "SKU";

                product_ccb.Text = typed;
                product_ccb.SelectionStart = typed.Length;
                product_ccb.SelectionLength = 0;
            }
            catch { }
        }
        private async void productType_CBB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (productType_CBB.SelectedValue == null || product_ccb.SelectedValue == null)
                return;

            int selectedSKU = Convert.ToInt32(product_ccb.SelectedValue);
            string productTypeCode = productType_CBB.SelectedValue.ToString();
            DataView dv = new DataView(mProductPacking_dt);
            if (productTypeCode.CompareTo("HX") == 0 || productTypeCode.CompareTo("HT") == 0)
            {

                if (!isNewState)
                    dv.RowFilter = $"SKU = {selectedSKU} AND (packing = 'weight' OR [Package] = 'weight')";
                else
                    dv.RowFilter = $"SKU = {selectedSKU} AND IsActive = True AND (packing = 'weight' OR [Package] = 'weight')";
            }
            else
            {
                if (!isNewState)
                    dv.RowFilter = $"SKU = {selectedSKU} AND packing <> 'weight'";
                else
                    dv.RowFilter = $"SKU = {selectedSKU} AND IsActive = True AND packing <> 'weight'";
            }

            packing_ccb.DataSource = dv;
            packing_ccb.DisplayMember = "PackingName"; // cột bạn muốn hiển thị
            packing_ccb.ValueMember = "ProductPackingID";

            if (dataGV.CurrentRow != null && !isNewState)
            {
                int productPackingID = Convert.ToInt32(dataGV.CurrentRow.Cells["ProductPackingID"].Value);
                packing_ccb.SelectedValue = productPackingID;
            }

            //var currentDV = packing_ccb.DataSource as DataView;
            //if (currentDV != null && currentDV.Count > 0)
            //{
            //    bool exists = currentDV.Cast<DataRowView>().Any(r => Convert.ToInt32(r["ProductPackingID"]) == productPackingID);

            //    if (exists)
            //        packing_ccb.SelectedValue = productPackingID;
            //    else
            //        packing_ccb.SelectedIndex = 0;
            //}
        }
        
        private async void packing_cbb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (packing_ccb.SelectedValue == null)
                return;

            DataRowView pdpData = (DataRowView)packing_ccb.SelectedItem;
            string packing = pdpData["packing"].ToString();

            if (packing.CompareTo("weight") == 0)
            {
                nwOder_tb.Enabled = true;
                PCSOrder_tb.Enabled = false;
                PCSOrder_tb.Text = "0";
            }
            else
            {
                nwOder_tb.Enabled = false;
                PCSOrder_tb.Enabled = true;
                updateNetWeight();
            }
        }
        
        private async void exportCode_search_cbb_SelectedIndexChanged(object sender, EventArgs e)
        {
           if (!int.TryParse(orderDomesticCode_cbb.SelectedValue.ToString(), out int exportCodeId))
                return;

            mCurrentOrderDomesticCodeID = exportCodeId;

            var orderLogTask = SQLStore.Instance.GetOrderDomesticDetailLogAsync(mCurrentOrderDomesticCodeID);
            var othersTask = SQLStore.Instance.getOrderDomesticDetailAsync(mCurrentOrderDomesticCodeID);
            await Task.WhenAll(othersTask, orderLogTask);

            mOrderDomesticDetail_dt = othersTask.Result;
            mLogDV = new DataView(orderLogTask.Result);

            // Gán lại cho DataGridView
            dataGV.DataSource = mOrderDomesticDetail_dt;
            log_GV.DataSource = mLogDV;
            ReadOnly_btn_Click(null, null);
        }

        private void product_ccb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (product_ccb.SelectedValue == null) return;

            int selectedSKU = Convert.ToInt32(product_ccb.SelectedValue);
            // Lọc packing theo SKU

            var productItem = (DataRowView)product_ccb.SelectedItem;

            int rawPrice = Convert.ToInt32(productItem["RawPrice"]);
            int refinedPrice = Convert.ToInt32(productItem["RefinedPrice"]);
            int packedPrice = Convert.ToInt32(productItem["PackedPrice"]);

            productType_CBB.SelectedIndexChanged -= productType_CBB_SelectedIndexChanged;
            DataTable clonedTable = mProductType_dt.Copy();

            if (rawPrice <= 0)
                clonedTable.Rows.Remove(clonedTable.Select("CustomerProductTypesCode = 'HX'").FirstOrDefault());
            if (refinedPrice <= 0)
                clonedTable.Rows.Remove(clonedTable.Select("CustomerProductTypesCode = 'HT'").FirstOrDefault());
            if (packedPrice <= 0)
                clonedTable.Rows.Remove(clonedTable.Select("CustomerProductTypesCode = 'HDG'").FirstOrDefault());

            if (dataGV.CurrentRow != null)
            {
               // int productPackingID = Convert.ToInt32(dataGV.CurrentRow.Cells["ProductPackingID"].Value);
                string productTypeCode = dataGV.CurrentRow.Cells["CustomerProductTypesCode"].Value.ToString();
                if (isNewState && productType_CBB.SelectedValue != null)
                    productTypeCode = productType_CBB.SelectedValue.ToString();

                productType_CBB.DataSource = clonedTable;
                productType_CBB.SelectedValue = productTypeCode;

            }
            productType_CBB_SelectedIndexChanged(null, null);
            productType_CBB.SelectedIndexChanged += productType_CBB_SelectedIndexChanged;

            /*
            DataRowView pdpData = (DataRowView)packing_ccb.SelectedItem;
            DataRowView pdData = (DataRowView)product_ccb.SelectedItem;
            string package = pdData["Package"].ToString();
            string packing = pdpData["packing"].ToString();

            if (package.CompareTo("weight") == 0 || packing.CompareTo("weight") == 0)
            {
                nwOder_tb.Enabled = true;
                PCSOrder_tb.Enabled = false;
                PCSOrder_tb.Text = "";
            }
            else
            {
                nwOder_tb.Enabled = false;
                PCSOrder_tb.Enabled = true;
                updateNetWeight();
            }*/
        }

        private void packing_ccb_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateNetWeight();
        }
        
        private async void dataGV_CellClick(object sender, EventArgs e)
        {
            status_lb.Text = "";
            if (orderDomesticCode_cbb.SelectedValue == null) return;
            if (dataGV.CurrentRow == null) return;

            var currentRow = dataGV.CurrentRow;

            _ = updateRightUI(currentRow.Index);
            _ = UpdateBottomInfo(currentRow);
        }

        private async Task UpdateBottomInfo(DataGridViewRow currentRow)
        {
            int CustomerID = -1;
            int.TryParse(currentRow.Cells["CustomerID"].Value?.ToString(), out CustomerID);

            if (CustomerID < 0) return;
            decimal sumNW_cus = 0;
            int sumPCS_cus = 0;
            decimal sumNW = 0;
            int sumPCS = 0;


            if (!int.TryParse(orderDomesticCode_cbb.SelectedValue.ToString(), out int exportCodeId))
            {
                return;
            }

            var rows = mOrderDomesticDetail_dt.AsEnumerable()
                .Where(r => r.RowState != DataRowState.Deleted)
                .Where(r => r.Field<int>("ExportCodeID") == exportCodeId) // chỉ lấy ExportCode cần xử lý
                .ToList();
            // Duyệt toàn bộ các dòng trong DataGridView
            foreach (DataRow row in rows)
            {
                decimal nw = 0;
                int pcs = 0;
                int cus = -1;
                decimal.TryParse(row["NWOther"]?.ToString(), out nw);
                int.TryParse(row["PCSOther"]?.ToString(), out pcs);
                int.TryParse(row["CustomerID"]?.ToString(), out cus);
                if (cus == CustomerID)
                {
                    sumNW_cus += nw;
                    sumPCS_cus += pcs;
                }
                sumNW += nw;
                sumPCS += pcs;
            }

            total_label.Text = "[" + sumPCS + " pcs, " + sumNW + " kg" + "]";
            cus_name_label.Text = Convert.ToString(currentRow.Cells["CustomerName"].Value) + ": ";
            cus_lable.Text = "[" + (sumPCS_cus.ToString() + " pcs, ") + (sumNW_cus.ToString() + " kg") + "]";
        }

        private async Task updateRightUI(int indexRowSelected)
        {
            if (indexRowSelected < 0 || isNewState)
                return;
            var cells = dataGV.Rows[indexRowSelected].Cells;
            string orderDomesticDetailID = cells["OrderDomesticDetailID"].Value.ToString();
            string packingID = cells["ProductPackingID"].Value.ToString();
            string customerProductTypesCode = cells["CustomerProductTypesCode"].Value.ToString();
            string PCSOrder = cells["PCSOrder"].Value.ToString();
            string NWOrder = cells["NWOrder"].Value.ToString();
            string price = cells["Price"].Value.ToString();

            orderDomesticDetailID_tb.Text = orderDomesticDetailID;

            //DataRow[] packingRows = mProductPacking_dt.Select($"ProductPackingID = {packingID}");
            string sku = cells["SKU"].Value.ToString();
            if (!product_ccb.Items.Cast<object>().Any(i => ((DataRowView)i)["SKU"].ToString() == sku))
            {
                product_ccb.DataSource = mProduct_dt;
            }

            product_ccb.SelectedValue = sku;
            packing_ccb.SelectedValue = packingID;

            PCSOrder_tb.Text = PCSOrder;
            nwOder_tb.Text = NWOrder;

            mLogDV.RowFilter = $"OrderDomesticDetailID = {Convert.ToInt32(orderDomesticDetailID)}";
            mLogDV.Sort = "LogID DESC";
        }

        private void updateNetWeight() {
            
            DataRowView pdData = (DataRowView)product_ccb.SelectedItem;
            DataRowView pdpData = (DataRowView)packing_ccb.SelectedItem;
            if (pdData == null || pdpData == null) return;
            string package = pdData["Package"].ToString();
            string packing = pdpData["packing"].ToString();

            if (package.CompareTo("weight") == 0 || packing.CompareTo("weight") == 0)
                return;
            else if (packing_ccb.SelectedItem == null || PCSOrder_tb.Text.CompareTo("") == 0)
            {
                nwOder_tb.Text = "0";
                return;
            }
                        
            int PCSOther = Convert.ToInt32(PCSOrder_tb.Text);
            int? amount = pdpData["Amount"] == DBNull.Value
                        ? (int?)null
                        : Convert.ToInt32(pdpData["Amount"]);

            if (amount == null)
            {
                nwOder_tb.Text = "0";
            }
            else
            {
                nwOder_tb.Text = Utils.calNetWeight(PCSOther, (int)amount, packing).ToString();
            }
        }
       

        private async void updateOrder(int orderDomesticDetailID, int orderDomesticCodeID, int packingId, string productType, int PCSOrder, decimal NWOrder, decimal price)
        {
           // DataRow[] orderDomesticCodeRows = mOrderDomesticCode_dt.Select($"ExportCodeID = {orderDomesticCodeID}");
            DataRow[] productTypeRows = mProductType_dt.Select($"CustomerProductTypesCode = '{productType}'");
            DataRow[] packingRows = mProductPacking_dt.Select($"ProductPackingID = {packingId}");
            string proVN = packingRows.Length > 0 ? packingRows[0]["Name_VN"].ToString() : "Unknown";
            string package = packingRows.Length > 0 ? packingRows[0]["Package"].ToString() : "";
            string packing = packingRows.Length > 0 ? packingRows[0]["packing"].ToString() : "";
            string productTypeName = productTypeRows.Length > 0 ? productTypeRows[0]["TypeName"].ToString() : "";
            int amount = packingRows.Length > 0 ? Convert.ToInt32(packingRows[0]["Amount"]) : 0;
            int sku = Convert.ToInt32(packingRows[0]["SKU"]);

            foreach (DataRow row in mOrderDomesticDetail_dt.Rows)
            {
                int id =Convert.ToInt32(row["OrderDomesticDetailID"]);
                if (id.CompareTo(orderDomesticDetailID) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa?", "Cập Nhật", MessageBoxButtons.YesNo,MessageBoxIcon.Question );

                    if (dialogResult != DialogResult.Yes)
                        return;

                    string oldValue = $"{row["ProductNameVN"]} - {row["ProductTypeName"]} - {row["PCSOrder"]} - {row["NWOrder"]} - {row["Amount"]} {row["packing"]}";
                    string newValue = $"{proVN} - {productTypeName} - {PCSOrder} - {NWOrder} - {amount} {packing}";

                    try
                    {
                        bool isScussess = await SQLManager.Instance.updateOrderDomesticDetailAsync(orderDomesticDetailID, packingId, productType, PCSOrder, NWOrder, price);

                        if (isScussess == true)
                        {
                            status_lb.Text = "Thành công.";
                            status_lb.ForeColor = System.Drawing.Color.Green;

                            row["ProductPackingID"] = packingId;
                            row["PCSOrder"] = PCSOrder;
                            row["NWOrder"] = NWOrder;
                            row["Price"] = price;
                            row["CustomerProductTypesCode"] = productType;
                            row["ProductTypeName"] = productTypeName;
                            row["Amount"] = amount;
                            row["Package"] = package;
                            row["packing"] = packing;                               
                            row["ProductNameVN"] = proVN;
                            row["SKU"] = sku;
                            _ = SQLManager.Instance.InsertOrderDomesticDetailLogAsync(orderDomesticDetailID, orderDomesticCodeID, "Update Success: " + oldValue, newValue);
                        }
                        else
                        {
                            status_lb.Text = "Thất bại.";
                            status_lb.ForeColor = System.Drawing.Color.Red;
                            _ = SQLManager.Instance.InsertOrderDomesticDetailLogAsync(orderDomesticDetailID, orderDomesticCodeID, "Update Fail: " + oldValue, newValue);
                        }
                    }
                    catch (Exception ex)
                    {
                        status_lb.Text = "Thất bại.";
                        status_lb.ForeColor = System.Drawing.Color.Red;
                        _ = SQLManager.Instance.InsertOrderDomesticDetailLogAsync(orderDomesticDetailID, orderDomesticCodeID, "Update Exception : " + ex.Message + oldValue, newValue);
                    }
                    
                    break;
                }
            }
        }

        private async void createOrder(int OrderDomesticCodeID, int packingId, string productType, int PCSOrder, decimal NWOrder, decimal price)
        {
            DataRow[] productTypeRows = mProductType_dt.Select($"CustomerProductTypesCode = '{productType}'");
            DataRow[] orderDomesticCodeRows = mOrderDomesticCode_dt.Select($"OrderDomesticCodeID = {OrderDomesticCodeID}");
            DataRow[] packingRows = mProductPacking_dt.Select($"ProductPackingID = {packingId}");
            string proVN = packingRows.Length > 0 ? packingRows[0]["Name_VN"].ToString() : "Unknown";
            string productTypeName = productTypeRows.Length > 0 ? productTypeRows[0]["TypeName"].ToString() : "";
            string package = packingRows.Length > 0 ? packingRows[0]["Package"].ToString() : "";
            string packing = packingRows.Length > 0 ? packingRows[0]["packing"].ToString() : "";
            int amount = packingRows.Length > 0 ? Convert.ToInt32(packingRows[0]["Amount"]) : 0;
            int sku = Convert.ToInt32(packingRows[0]["SKU"]);
            string newValue = $"{proVN} - {productTypeName} - {PCSOrder} - {NWOrder} - {amount} {packing}";
            try
            {
                int newId = await SQLManager.Instance.insertOrderDomesticDetailAsync(OrderDomesticCodeID, packingId, productType, PCSOrder, NWOrder, price);
                if (newId > 0)
                {
                    DataRow drToAdd = mOrderDomesticDetail_dt.NewRow();

                    drToAdd["OrderDomesticDetailID"] = newId;
                    drToAdd["OrderDomesticCodeID"] = OrderDomesticCodeID;
                    drToAdd["ProductPackingID"] = packingId;
                    drToAdd["PCSOrder"] = PCSOrder;
                    drToAdd["NWOrder"] = NWOrder;                        
                    drToAdd["Price"] = price;
                    drToAdd["Amount"] = amount;
                    drToAdd["Package"] = package;
                    drToAdd["packing"] = packing;
                    drToAdd["ProductNameVN"] = proVN;
                    drToAdd["SKU"] = sku;
                    drToAdd["CustomerProductTypesCode"] = productType;
                    drToAdd["ProductTypeName"] = productTypeName;

                    mOrderDomesticDetail_dt.Rows.Add(drToAdd);
                    mOrderDomesticDetail_dt.AcceptChanges();

                    status_lb.Text = "Thành công";
                    status_lb.ForeColor = System.Drawing.Color.Green;

                    newCustomerBtn_Click(null, null);

                    _ = SQLManager.Instance.InsertOrderDomesticDetailLogAsync(newId, OrderDomesticCodeID, "Create Success", newValue);
                }
                else
                {
                    status_lb.Text = "Thất bại";
                    status_lb.ForeColor = System.Drawing.Color.Red;
                    _ = SQLManager.Instance.InsertOrderDomesticDetailLogAsync(newId, OrderDomesticCodeID, "Create Fail", newValue);
                }
            }
            catch (Exception ex)
            {
                status_lb.Text = "Thất bại.";
                status_lb.ForeColor = System.Drawing.Color.Red;
                _ = SQLManager.Instance.InsertOrderDomesticDetailLogAsync(-1, OrderDomesticCodeID, "Create Exception: " + ex.Message, newValue);
            }
        }
        private void saveBtn_Click(object sender, EventArgs e)
        {
            if (orderDomesticCode_cbb.SelectedValue == null || orderDomesticCode_cbb.SelectedValue == null || productType_CBB.SelectedValue == null)
                return;

            DataRowView pdData = (DataRowView)product_ccb.SelectedItem;
            if (pdData == null) return;
            DataRowView pdpData = (DataRowView)packing_ccb.SelectedItem;
            if (pdData == null) return;
            string package = pdData["Package"].ToString();
            string packing= pdpData["Packing"].ToString();

            bool isShowMessage = false;
            if (packing_ccb.SelectedValue == null || string.IsNullOrWhiteSpace(nwOder_tb.Text))
            {
                isShowMessage = true;
            }
            else if (string.IsNullOrWhiteSpace(PCSOrder_tb.Text) && package.CompareTo("weight") != 0 && packing.CompareTo("weight") != 0)
            {
                isShowMessage = true;
            }

            if (isShowMessage)
            {
                MessageBox.Show(
                                "Thiếu Dữ Liệu, Kiểm Tra Lại!",
                                "Thông Báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning
                            );
                return;
            }

            int orderDomesticCodeID = Convert.ToInt32(orderDomesticCode_cbb.SelectedValue);
            string productType = Convert.ToString(productType_CBB.SelectedValue);
            int packingId = Convert.ToInt32(packing_ccb.SelectedValue);
            int PCSOrder = int.TryParse(PCSOrder_tb.Text, out var value) ? value : 0;
            decimal NWOrder = Convert.ToDecimal(nwOder_tb.Text);
            decimal price = 0;

            string productTypeCode = productType_CBB.SelectedValue.ToString();
            var productTypeItem = (DataRowView)product_ccb.SelectedItem;
            if (productTypeCode.CompareTo("HX") == 0)
                price = Convert.ToInt32(productTypeItem["RawPrice"]);
            else if (productTypeCode.CompareTo("HT") == 0)
                price = Convert.ToInt32(productTypeItem["RefinedPrice"]);
            else
                price = Convert.ToInt32(productTypeItem["PackedPrice"]);

            if (NWOrder <= 0)
            {
                MessageBox.Show(
                                "Net Weight phải lớn hơn 0",
                                "Thông Báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning
                            );
                return;
            }


            if (orderDomesticDetailID_tb.Text.Length != 0)
            {
                int orderId = Convert.ToInt32(orderDomesticDetailID_tb.Text);
                if (CountDuplicateInDataGridView(orderId, packingId) == 0)
                    updateOrder(orderId, orderDomesticCodeID, packingId, productType, PCSOrder, NWOrder, price);
                else
                {
                    MessageBox.Show(
                                "Sản phẩm này đã được thêm vào",
                                "Thông Báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning
                            );
                }

            }
            else
            {
                if (CountDuplicateInDataGridView(-1, packingId) == 0)
                {
                    createOrder(orderDomesticCodeID, packingId, productType, PCSOrder, NWOrder, price);
                }
                else
                {
                    MessageBox.Show(
                                "Sản phẩm này đã được thêm vào",
                                "Thông Báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning
                            );
                }
            }
        }

        private int CountDuplicateInDataGridView(int orderId, int productPackingId)
        {
            int count = 0;

            foreach (DataRow row in mOrderDomesticDetail_dt.Rows)
            {

                // ép về int, nếu null thì gán 0
                int orderDomesticDetailID = Convert.ToInt32(row["OrderDomesticDetailID"]);                

                if (orderId == orderDomesticDetailID) continue;

                int prod = row["ProductPackingID"] == null ? -1 : Convert.ToInt32(row["ProductPackingID"]);
                if (prod == productPackingId)
                {
                    count++;
                }
            }

            return count;
        }

        private void deleteOrderID()
        {
            if (dataGV.CurrentRow == null) return;
            int id = Convert.ToInt32(dataGV.CurrentRow.Cells["OrderDomesticDetailID"].Value);
            int PCSReal = 0;
            int NWReal = 0;

            int.TryParse(dataGV.CurrentRow.Cells["PCSReal"].Value?.ToString(), out PCSReal);
            int.TryParse(dataGV.CurrentRow.Cells["NWReal"].Value?.ToString(), out NWReal);
            string mess = $"XÓA OrderID: {id}\nChắc chắn chưa?";
            if (PCSReal > 0 || NWReal > 0)
            {
                mess = $"OrderID: {id} đang có PCS, NW đóng thùng \nChắc chắn xóa không?";
            }

            DialogResult dialogResult = MessageBox.Show(mess,"Thông Báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (dialogResult != DialogResult.Yes) return;
                
            deleteOrderProduct(id);
            
        }

        private async void deleteOrderProduct(int id)
        {

            foreach (DataRow row in mOrderDomesticDetail_dt.Rows)
            {
                int orderId = Convert.ToInt32(row["OrderDomesticDetailID"]);
                if (id.CompareTo(orderId) == 0)
                {
                  //  int exportCodeId = Convert.ToInt32(row["ExportCodeID"]);
                    try
                    {   
                        bool isSuccess = await SQLManager.Instance.deleteOrderDomesticDetailAsync(id);

                        if (isSuccess)
                        {
                            status_lb.Text = "Thành công.";
                            status_lb.ForeColor = System.Drawing.Color.Green;


                           // _ = SQLManager.Instance.InsertOrderLogAsync(exportCodeId, orderId, "Delete M" + mode + " Thành Công ", "", "", 0, 0);
                            // Xóa row khỏi DataTable
                            mOrderDomesticDetail_dt.Rows.Remove(row);
                            mOrderDomesticDetail_dt.AcceptChanges();
                            
                        }
                        else
                        {
                            status_lb.Text = "Thất bại.";
                            status_lb.ForeColor = System.Drawing.Color.Red;
                           // _ = SQLManager.Instance.InsertOrderLogAsync(exportCodeId, orderId, "Delete M" + mode + " Thất Bại ","", "", 0, 0);
                        }
                    }
                    catch (Exception ex)
                    {
                        status_lb.Text = "Thất bại.";
                        status_lb.ForeColor = System.Drawing.Color.Red;
                        //_ = SQLManager.Instance.InsertOrderLogAsync(exportCodeId, orderId, "Delete M" + mode + " Thất Bại Do Exception: " + ex.Message,"", "", 0, 0);
                    }

                    break; // Dừng vòng lặp sau khi xóa
                }
            }
        }

        private void Tb_KeyPress_OnlyNumber(object sender, KeyPressEventArgs e)
        {
            TextBox tb = sender as TextBox;

            // Chỉ cho nhập số, phím điều khiển, hoặc dấu chấm (.)
            if (!char.IsControl(e.KeyChar)
                && !char.IsDigit(e.KeyChar)
                && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            // Không cho nhập nhiều dấu chấm
            if (e.KeyChar == '.' && tb.Text.Contains("."))
            {
                e.Handled = true;
            }
        }

        private void Tb_KeyPress_OnlyNumber1(object sender, KeyPressEventArgs e)
        {
            TextBox tb = sender as TextBox;

            // Chỉ cho nhập số, phím điều khiển, hoặc dấu chấm (.)
            if (!char.IsControl(e.KeyChar)
                && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void newCustomerBtn_Click(object sender, EventArgs e)
        {
            orderDomesticDetailID_tb.Text = "";
            PCSOrder_tb.Text = "";
            nwOder_tb.Text= "";
            isNewState = true;
            packing_btn.Visible = false;
            info_gb.BackColor = newCustomerBtn.BackColor;
            edit_btn.Visible = false;
            newCustomerBtn.Visible = false;
            readOnly_btn.Visible = true;
            LuuThayDoiBtn.Visible = true;            
            LuuThayDoiBtn.Text = "Lưu Mới";            
            setRightUIReadOnly(false);
            if (dataGV.Rows.Count > 0)
            {
                dataGV.FirstDisplayedScrollingRowIndex = dataGV.Rows.Count - 1;
            }
            product_ccb.Focus();

        }

        private void Packing_btn_Click(object sender, EventArgs e)
        {
            info_gb.Enabled = false;
            edit_btn.Visible = false;
            newCustomerBtn.Visible = false;
            readOnly_btn.Visible = true;
            LuuThayDoiBtn.Visible = false;
            packing_btn.Visible = false;
            LuuThayDoiBtn.Text = "Lưu Mới";
            setRightUIReadOnly(false);
            ispackingState = true;
        }

        private void ReadOnly_btn_Click(object sender, EventArgs e)
        {
            info_gb.Enabled = true;
            edit_btn.Visible = true;
            newCustomerBtn.Visible = true;
            readOnly_btn.Visible = false;
            LuuThayDoiBtn.Visible = false;
            packing_btn.Visible = true;
            info_gb.BackColor = System.Drawing.Color.DarkGray;
            isNewState = false;
            product_ccb.DropDownStyle = ComboBoxStyle.DropDownList;
            setRightUIReadOnly(true);

            dataGV_CellClick(null, null);
            ispackingState = false;
        }

        private void Edit_btn_Click(object sender, EventArgs e)
        {
            isNewState = false;

            product_ccb_SelectedIndexChanged(null, null);

            packing_btn.Visible = false;
            edit_btn.Visible = false;
            newCustomerBtn.Visible = false;
            readOnly_btn.Visible = true;
            LuuThayDoiBtn.Visible = true;
            info_gb.BackColor = edit_btn.BackColor;
            
            LuuThayDoiBtn.Text = "Lưu C.Sửa";
            setRightUIReadOnly(false);

        }

        private void PCSOrder_tb_TextChanged(object sender, EventArgs e)
        {
            updateNetWeight();
        }


        private void preview_print_TD_btn_Click(object sender, EventArgs e)
        {
            _ = PrintPendingOrderSummary(1);
        }

        private async void printPendingOrderSummary_btn_Click(object sender, EventArgs e)
        {
            await PrintPendingOrderSummary(2);
        }

        private async Task PrintPendingOrderSummary(int state)
        {
            if (orderDomesticCode_cbb.SelectedItem == null) return;

            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Message = "Đang xử lý ...";
            loadingOverlay.Show();
            await Task.Delay(100);
                        
            DataRowView orderDomesticCodeDRV = (DataRowView)orderDomesticCode_cbb.SelectedItem;
            int orderDomesticIndex = Convert.ToInt32(orderDomesticCodeDRV["OrderDomesticIndex"]);
            DateTime deliveryDate = Convert.ToDateTime(orderDomesticCodeDRV["DeliveryDate"]);
            string customerCode = Convert.ToString(orderDomesticCodeDRV["CustomerCode"]);
            string company = Convert.ToString(orderDomesticCodeDRV["Company"]);
            OrderDomesticSummaryPrinter printer = new OrderDomesticSummaryPrinter();

            if (state == 1)
                printer.PrintPreview(mOrderDomesticDetail_dt, orderDomesticIndex, deliveryDate, customerCode, company, this);
            else if (state == 2)
                printer.PrintDirect(mOrderDomesticDetail_dt, orderDomesticIndex, deliveryDate, customerCode, company, tongdon_in2mat_cb.Checked);
            else
                printer.PrintDirectToPdf(mOrderDomesticDetail_dt, orderDomesticIndex, deliveryDate, customerCode, company);
            await Task.Delay(200);
            loadingOverlay.Hide();
        }


        private async void PGH_preview_btn_Click(object sender, EventArgs e)
        {
            await PhieuGiaoHangPrinter(1);
        }

        private async void PGH_btn_Click(object sender, EventArgs e)
        {
            await PhieuGiaoHangPrinter(2);
        }

        private async Task PhieuGiaoHangPrinter(int state)
        {
            if (orderDomesticCode_cbb.SelectedItem == null) return;

            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Message = "Đang xử lý ...";
            loadingOverlay.Show();
            await Task.Delay(100);

            DataRowView orderDomesticCodeDRV = (DataRowView)orderDomesticCode_cbb.SelectedItem;
            int orderDomesticIndex = Convert.ToInt32(orderDomesticCodeDRV["OrderDomesticIndex"]);
            DateTime deliveryDate = Convert.ToDateTime(orderDomesticCodeDRV["DeliveryDate"]);
            string customerCode = Convert.ToString(orderDomesticCodeDRV["CustomerCode"]);
            string company = Convert.ToString(orderDomesticCodeDRV["Company"]);
            string customerName = Convert.ToString(orderDomesticCodeDRV["CustomerName"]);
            string customerAddress = Convert.ToString(orderDomesticCodeDRV["Address"]);
            PhieuGiaoHangTrongNuoc_Printer printer = new PhieuGiaoHangTrongNuoc_Printer();

            if (state == 1)
                printer.PrintPreview(mOrderDomesticDetail_dt, orderDomesticIndex, deliveryDate, customerCode, company, customerName, customerAddress, this);
            else if (state == 2)
                printer.PrintDirect(mOrderDomesticDetail_dt, orderDomesticIndex, deliveryDate, customerCode, company, customerName, customerAddress, tongdon_in2mat_cb.Checked);
            await Task.Delay(200);
            loadingOverlay.Hide();
        }

        private void setRightUIReadOnly(bool isReadOnly)
        {
            if (orderDomesticCode_cbb.SelectedItem != null)
            {
                DataRowView dataR = (DataRowView)orderDomesticCode_cbb.SelectedItem;

                string staff = dataR["InputByName_NoSign"].ToString();
                //if (UserManager.Instance.fullName_NoSign.CompareTo(staff) != 0)
                //{
                //    edit_btn.Visible = false;
                //    newCustomerBtn.Visible = false;
                //    readOnly_btn.Visible = false;
                //    product_ccb.Enabled = false;
                //    packing_ccb.Enabled = false;
                //    PCSOther_tb.ReadOnly = true;
                //    netWeight_tb.ReadOnly = true;
                //    return;
                //}
            }
            product_ccb.Enabled = !isReadOnly;
            packing_ccb.Enabled = !isReadOnly;
            PCSOrder_tb.ReadOnly = isReadOnly;
            nwOder_tb.ReadOnly = isReadOnly;
            product_ccb.DropDownStyle = ComboBoxStyle.DropDown;

            tongdon_gb.Visible = isReadOnly;
            phieugiaohang_gb.Visible = isReadOnly;
        }

        private void dataGV_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            oldValue = dataGV.CurrentCell?.Value;
            //if (orderDomesticCode_cbb.SelectedItem != null)
            //{
            //    DataRowView dataR = (DataRowView)orderDomesticCode_cbb.SelectedItem;
            //    string staff = dataR["InputByName_NoSign"].ToString();
            //    if (UserManager.Instance.fullName_NoSign.CompareTo(staff) != 0)
            //    {
            //        e.Cancel = true;
            //        return;
            //    }
            //}

            //if (isNewState)
            //{
            //    e.Cancel = true;
            //    return;
            //}
            e.Cancel = !ispackingState;
        }

        private async void dataGV_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                object newValue = dataGV.CurrentCell?.Value;
                if (object.Equals(oldValue, newValue)) return;

                var row = dataGV.CurrentRow;
                if (row?.Cells["OrderDomesticDetailID"].Value == null || row.Cells["OrderDomesticDetailID"].Value == DBNull.Value)
                    return;

                var orderDomesticDetailID = Convert.ToInt32(row.Cells["OrderDomesticDetailID"].Value);
                var orderDomesticCodeID = Convert.ToInt32(row.Cells["OrderDomesticCodeID"].Value);

                var columnName = dataGV.Columns[e.ColumnIndex].Name;
                DataRow orderRow = mOrderDomesticDetail_dt.Select($"OrderDomesticDetailID = {orderDomesticDetailID}").FirstOrDefault();
                if (orderRow == null) return;
                if (columnName == "PCSReal")
                {
                    UpdateNWReal(orderRow);
                }
                else if (columnName == "NWReal")
                {
                    orderRow["PCSReal"] = 0;
                }
                
                string newValueStr = $"{orderRow["PCSReal"]} - {orderRow["NWReal"]} - {orderRow["Note"].ToString()}";

                try
                {
                    bool isSuccess = await SQLManager.Instance.updateOrderDomesticDetail_PackingAsync(orderDomesticDetailID, Convert.ToInt32(orderRow["PCSReal"]), Convert.ToDecimal(orderRow["NWReal"]), orderRow["Note"].ToString());
                    if (!isSuccess)
                    {
                        MessageBox.Show($"Cập Nhật Thất Bại", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        _ = SQLManager.Instance.InsertOrderDomesticDetailLogAsync(orderDomesticDetailID, orderDomesticCodeID, columnName + " - Update đóng thùng Fail", newValueStr);
                    }
                    else
                    {
                        _ = SQLManager.Instance.InsertOrderDomesticDetailLogAsync(orderDomesticDetailID, orderDomesticCodeID, columnName + " - Update đóng thùng Success", newValueStr);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR: " + ex.Message);
                    _ = SQLManager.Instance.InsertOrderDomesticDetailLogAsync(orderDomesticDetailID, orderDomesticCodeID, columnName + " - Update đóng thùng Exception " + ex.Message, newValueStr);
                }
            }
        }

        private void dataGV_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            string columnName = dataGV.Columns[e.ColumnIndex].Name;
            if (columnName != "PCSReal" && columnName != "NWReal" && columnName != "Note") return;

            var row = dataGV.Rows[e.RowIndex];
            var packing = row.Cells["packing"].Value.ToString();
            var package = row.Cells["Package"].Value.ToString();

            if (columnName == "NWReal")
            {
               
                if (packing.CompareTo("weight") == 0 || package.CompareTo("weight") == 0)
                {
                    row.Cells["PCSReal"].ReadOnly = true;
                    e.CellStyle.BackColor = System.Drawing.Color.LightGray;
                }
            }
            else if (columnName == "Note")
            {
                e.CellStyle.BackColor = System.Drawing.Color.LightGray;
            }
            else
            {
                if (packing.CompareTo("weight") != 0 && package.CompareTo("weight") != 0)
                {
                    row.Cells["NWReal"].ReadOnly = true;
                    e.CellStyle.BackColor = System.Drawing.Color.LightGray;
                }
            }
        }

        private void UpdateNWReal(DataRow row)
        {
            if (row == null) return;
            var cellPCS = row["PCSReal"];
            if (cellPCS != null && int.TryParse(cellPCS.ToString(), out int pcs))
            {
                var amount = Convert.ToInt32(row["Amount"]);
                var packing = Convert.ToString(row["packing"]);

                row["NWReal"] = Utils.calNetWeight(pcs, amount, packing);
            }
            else
            {
                row["NWReal"] = DBNull.Value;
            }
        }

        private void dataGV_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            TextBox tb = e.Control as TextBox;
            if (tb == null) return;

            // Gỡ toàn bộ trước để tránh add trùng
            tb.KeyPress -= Tb_KeyPress_OnlyNumber;
            tb.KeyPress -= Tb_KeyPress_OnlyNumber1;

            var colName = dataGV.CurrentCell.OwningColumn.Name;

            if (colName == "NWReal")
            {
                tb.KeyPress += Tb_KeyPress_OnlyNumber;
            }
            else if (colName == "PCSReal")
            {
                tb.KeyPress += Tb_KeyPress_OnlyNumber1;
            }
        }

        private void dataGV_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
        {
            if (dataGV.Columns[e.ColumnIndex].Name == "NWReal" || dataGV.Columns[e.ColumnIndex].Name == "PCSReal")
            {
                if (e.Value != null)
                {
                    string input = e.Value.ToString().Replace(',', '.'); // cho phép nhập cả ',' hoặc '.'

                    if (decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal value))
                    {
                        e.Value = value;
                        e.ParsingApplied = true;
                    }
                }
            }
        }

    }
}
