using ClosedXML.Excel;

using RauViet.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RauViet.ui
{
    public partial class OrderDomesticDetail : Form
    {
        System.Data.DataTable mProductPacking_dt, mProduct_dt, mOrderDomesticCode_dt, mOrderDomesticDetail_dt;

        private Timer debounceTimer = new Timer { Interval = 150 };
        private LoadingOverlay loadingOverlay;
        bool isNewState = false;

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
            packing_ccb.TabIndex = countTab++; packing_ccb.TabStop = true;
            PCSOther_tb.TabIndex = countTab++; PCSOther_tb.TabStop = true;
            netWeight_tb.TabIndex = countTab++; netWeight_tb.TabStop = true;
            LuuThayDoiBtn.TabIndex = countTab++; LuuThayDoiBtn.TabStop = true;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            status_lb.Text = "";

            exportExcel_TD_btn.Click += exportExcel_TD_btn_Click;
            newCustomerBtn.Click += newCustomerBtn_Click;
            LuuThayDoiBtn.Click += saveBtn_Click;
            preview_print_TD_btn.Click += preview_print_TD_btn_Click;            
            printPendingOrderSummary_btn.Click += printPendingOrderSummary_btn_Click;
            dataGV.SelectionChanged += this.dataGV_CellClick;
            PCSOther_tb.TextChanged += PCSOrder_tb_TextChanged;
            packing_ccb.SelectedIndexChanged += packing_ccb_SelectedIndexChanged;
            
            PCSOther_tb.KeyPress += Tb_KeyPress_OnlyNumber1;
            netWeight_tb.KeyPress += Tb_KeyPress_OnlyNumber;


            debounceTimer.Tick += DebounceTimer_Tick;
            edit_btn.Click += Edit_btn_Click;
            readOnly_btn.Click += ReadOnly_btn_Click;            

            this.KeyDown += OrdersList_KeyDown;
        }

        private void OrdersList_KeyDown(object sender, KeyEventArgs e)
        {
            DataRowView dataR = (DataRowView)exportCode_search_cbb.SelectedItem;
            string staff = dataR["InputByName_NoSign"].ToString();

            if (e.KeyCode == Keys.F5)
            {
                if (mCurrentOrderDomesticCodeID <= 0)
                {
                    return;
                }

                SQLStore.Instance.removeOrders(mCurrentOrderDomesticCodeID);
                SQLStore.Instance.removeCustomers();
                SQLStore.Instance.removeProductSKU();
                SQLStore.Instance.removeProductpacking();
                SQLStore.Instance.removeLatestOrdersAsync();
                ShowData();
            }
            else if(!isNewState && !edit_btn.Visible && UserManager.Instance.fullName_NoSign.CompareTo(staff) == 0)
            {
                if (e.KeyCode == Keys.Delete)
                {
                    Control ctrl = this.ActiveControl;

                    if (ctrl is TextBox || ctrl is RichTextBox ||
                        (ctrl is DataGridView dgv && dgv.CurrentCell != null && dgv.IsCurrentCellInEditMode))
                    {
                        return; // không xử lý Delete
                    }

                    deleteOrderID(true);
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
                mProduct_dt = await SQLStore.Instance.getProductSKUAsync();

                var parameters = new Dictionary<string, object> { { "Complete", false } };
                var orderDomesticCode_dtTask = SQLStore.Instance.getOrderDomesticCodeAsync(parameters);
                
                var packingTask = SQLStore.Instance.getProductpackingAsync();          
                await Task.WhenAll(packingTask, orderDomesticCode_dtTask);              
                mProductPacking_dt = packingTask.Result;
                mOrderDomesticCode_dt = orderDomesticCode_dtTask.Result;

                if (mCurrentOrderDomesticCodeID <= 0 && mOrderDomesticCode_dt.Rows.Count > 0)
                {
                    mCurrentOrderDomesticCodeID = Convert.ToInt32(mOrderDomesticCode_dt.AsEnumerable()
                                   .Max(r => r.Field<int>("OrderDomesticCodeID")));
                }

               // var orderLogTask = SQLStore.Instance.GetOrderLogAsync(mCurrentExportID);
                var othersTask = SQLStore.Instance.getOrderDomesticDetailAsync(mCurrentOrderDomesticCodeID);
              //  var latestOrdersTask = SQLStore.Instance.get3LatestOrdersAsync();
                await Task.WhenAll(othersTask);

                mOrderDomesticDetail_dt = othersTask.Result;

                product_ccb.DataSource = mProduct_dt;
                product_ccb.DisplayMember = "ProductNameVN";
                product_ccb.ValueMember = "SKU";
                product_ccb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                product_ccb.AutoCompleteSource = AutoCompleteSource.ListItems;
                product_ccb.SelectedIndexChanged += product_ccb_SelectedIndexChanged;
                product_ccb.TextUpdate += product_ccb_TextUpdate;

                foreach (DataColumn col in mOrderDomesticDetail_dt.Columns)
                    col.ReadOnly = false;

                // Gán testData cho DataGridView tạm để test
                dataGV.DataSource = mOrderDomesticDetail_dt;                

                dataGV.Columns["Price"].HeaderText = "Giá";
                dataGV.Columns["ProductNameVN"].HeaderText = "Tên Sản Phẩm";
                dataGV.Columns["PCSOrder"].HeaderText = "PCS\nĐặt Hàng";
                dataGV.Columns["NWOrder"].HeaderText = "NW\nĐặt Hàng";
                dataGV.Columns["OrderDomesticDetailID"].HeaderText = "ID";

                dataGV.Columns["packing"].Visible = false;
                dataGV.Columns["Package"].Visible = false;
                dataGV.Columns["SKU"].Visible = false;
                dataGV.Columns["ProductPackingID"].Visible = false;
                dataGV.Columns["OrderDomesticCodeID"].Visible = false;
                dataGV.Columns["Amount"].Visible = false;
                //dataGV.Columns["ExportCodeID"].Visible = false;
                //dataGV.Columns["NWReal"].Visible = false;              
                //dataGV.Columns["CartonSize"].Visible = false;
                //dataGV.Columns["CustomerCarton"].Visible = false;
                //dataGV.Columns["LOTCode"].Visible = false;
                //dataGV.Columns["LOTCodeComplete"].Visible = false;
                //dataGV.Columns["CustomerCode"].Visible = false;
                //dataGV.Columns["ProductNameEN"].Visible = false;
                //dataGV.Columns["Package"].Visible = false;
                //dataGV.Columns["packing"].Visible = false;
                //dataGV.Columns["Amount"].Visible = false;
                //dataGV.Columns["ExportDate"].Visible = false;
                //dataGV.Columns["ExportCode"].Visible = false;
                //dataGV.Columns["PCSReal"].Visible = false;
                //dataGV.Columns["CartonNo"].Visible = false;
                dataGV.Columns["Price"].Visible = !UserManager.Instance.hasRole_AnGiaSanPham();


                dataGV.Columns["OrderDomesticDetailID"].Width = 60;
                dataGV.Columns["ProductNameVN"].Width = 150;
                dataGV.Columns["PCSOrder"].Width = 80;
                dataGV.Columns["NWOrder"].Width = 80;
                dataGV.Columns["Price"].Width = 80;

                dataGV.Columns["PCSOrder"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGV.Columns["NWOrder"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGV.Columns["OrderDomesticDetailID"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                exportCode_search_cbb.SelectedIndexChanged -= exportCode_search_cbb_SelectedIndexChanged;
                exportCode_search_cbb.DataSource = mOrderDomesticCode_dt;
                exportCode_search_cbb.DisplayMember = "OrderDomesticIndex";  // hiển thị tên
                exportCode_search_cbb.ValueMember = "OrderDomesticCodeID";                
                exportCode_search_cbb.SelectedIndexChanged += exportCode_search_cbb_SelectedIndexChanged;
                exportCode_search_cbb.SelectedValue = mCurrentOrderDomesticCodeID;

                await Task.Delay(500);
                ReadOnly_btn_Click(null, null);
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

        private async void exportCode_search_cbb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!int.TryParse(exportCode_search_cbb.SelectedValue.ToString(), out int exportCodeId))
                return;

            var othersTask = SQLStore.Instance.getOrdersAsync(mCurrentOrderDomesticCodeID);
            await Task.WhenAll(othersTask);

            mOrderDomesticDetail_dt = othersTask.Result;
            // Tạo DataView để filter
            DataView dv = new DataView(mOrderDomesticDetail_dt);
            dv.RowFilter = $"ExportCodeID = {exportCodeId} AND (PCSOther > 0 OR NWOther > 0)";

            // Gán lại cho DataGridView
            dataGV.DataSource = dv;

            var rows = mOrderDomesticDetail_dt.AsEnumerable()
                .Where(r => r.Field<int>("ExportCodeID") == exportCodeId && (r.Field<decimal?>("NWReal")?? 0) > 0) // chỉ lấy ExportCode cần xử lý
                .ToList();

            ReadOnly_btn_Click(null, null);
        }

        private void product_ccb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (product_ccb.SelectedValue == null) return;

            int selectedSKU = Convert.ToInt32(product_ccb.SelectedValue);

            // Lọc packing theo SKU
            DataView dv = new DataView(mProductPacking_dt);
            if(!isNewState)
                dv.RowFilter = $"SKU = '{selectedSKU}'";
            else
                dv.RowFilter = $"SKU = '{selectedSKU}' AND IsActive = true";

            packing_ccb.DataSource = dv;
            packing_ccb.DisplayMember = "PackingName"; // cột bạn muốn hiển thị
            packing_ccb.ValueMember = "ProductPackingID";

            priceCNF_tb.Text = ((DataRowView)product_ccb.SelectedItem)["PriceCNF"].ToString();

            if (dataGV.CurrentRow != null)
            {
                int productPackingID = Convert.ToInt32(dataGV.CurrentRow.Cells["ProductPackingID"].Value);

                var currentDV = packing_ccb.DataSource as DataView;
                if (currentDV != null && currentDV.Count > 0)
                {
                    bool exists = currentDV.Cast<DataRowView>().Any(r => Convert.ToInt32(r["ProductPackingID"]) == productPackingID);

                    if (exists)
                        packing_ccb.SelectedValue = productPackingID;
                    else
                        packing_ccb.SelectedIndex = 0;
                }
            }

            DataRowView pdData = (DataRowView)product_ccb.SelectedItem;
            string package = pdData["Package"].ToString();

            if (package.CompareTo("weight") == 0)
            {
                netWeight_tb.Enabled = true;
                PCSOther_tb.Enabled = false;
                PCSOther_tb.Text = "";
            }
            else
            {
                netWeight_tb.Enabled = false;
                PCSOther_tb.Enabled = true;
                updateNetWeight();
            }
        }

        private void packing_ccb_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateNetWeight();
        }
        
        private async void dataGV_CellClick(object sender, EventArgs e)
        {
            status_lb.Text = "";
            if (exportCode_search_cbb.SelectedValue == null) return;
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


            if (!int.TryParse(exportCode_search_cbb.SelectedValue.ToString(), out int exportCodeId))
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
            string orderId = cells["OrderId"].Value.ToString();
            string customerID = cells["CustomerID"].Value.ToString();
            string exportCodeId = cells["ExportCodeID"].Value.ToString();
            string packingID = cells["ProductPackingID"].Value.ToString();
            string PCSOther = cells["PCSOther"].Value.ToString();
            string NWOther = cells["NWOther"].Value.ToString();
            string orderPackingPriceCNF = cells["OrderPackingPriceCNF"].Value.ToString();

            orderId_tb.Text = orderId;

            DataRow[] packingRows = mProductPacking_dt.Select($"ProductPackingID = {packingID}");
            string sku = packingRows[0]["SKU"].ToString();
            if (!product_ccb.Items.Cast<object>().Any(i => ((DataRowView)i)["SKU"].ToString() == sku))
            {
                product_ccb.DataSource = mProduct_dt;
            }

            product_ccb.SelectedValue = sku;
            packing_ccb.SelectedValue = packingID;

            PCSOther_tb.Text = PCSOther;
            netWeight_tb.Text = NWOther;
            priceCNF_tb.Text = orderPackingPriceCNF;

        }

        private void updateNetWeight() {
            
            DataRowView pdData = (DataRowView)product_ccb.SelectedItem;
            if (pdData == null) return;
            string package = pdData["Package"].ToString();

            if (package.CompareTo("weight") == 0)
                return;
            else if (packing_ccb.SelectedItem == null || PCSOther_tb.Text.CompareTo("") == 0)
            {
                netWeight_tb.Text = "0";
                return;
            }

            DataRowView pakingData = (DataRowView)packing_ccb.SelectedItem;

            
            int PCSOther = Convert.ToInt32(PCSOther_tb.Text);
            string packing = pakingData["Packing"].ToString();
            int? amount = pakingData["Amount"] == DBNull.Value
                        ? (int?)null
                        : Convert.ToInt32(pakingData["Amount"]);

            if (amount == null)
            {
                netWeight_tb.Text = "0";
            }
            else
            {
                netWeight_tb.Text = Utils.calNetWeight(PCSOther, (int)amount, packing).ToString();
            }
        }
       

        private async void updateOrder(int orderId, int exportCodeId, int customerId, int packingId, int PCSOther, decimal NWOther, decimal priceCNF, bool isUpdateCusProduct)
        {
            int mode = isUpdateCusProduct ? 1 : 2;

            DataRow[] exportCodeRows = mOrderDomesticCode_dt.Select($"ExportCodeID = {exportCodeId}");
            DataRow[] packingRows = mProductPacking_dt.Select($"ProductPackingID = {packingId}");
            string proVN = packingRows.Length > 0 ? packingRows[0]["Name_VN"].ToString() : "Unknown";
            string package = packingRows.Length > 0 ? packingRows[0]["Package"].ToString() : "";
            string packing = packingRows.Length > 0 ? packingRows[0]["packing"].ToString() : "";
            int amount = packingRows.Length > 0 ? Convert.ToInt32(packingRows[0]["Amount"]) : 0;

            var rows = mOrderDomesticDetail_dt.AsEnumerable()
                .Where(r => r.Field<int>("ExportCodeID") == exportCodeId) // chỉ lấy ExportCode cần xử lý
                .ToList();

            foreach (DataRow row in rows)
            {
                int id =Convert.ToInt32(row["OrderId"]);
                if (id.CompareTo(orderId) == 0)
                {
                    if (!isUpdateCusProduct)
                    {
                        DialogResult dialogResult = MessageBox.Show(
                                                "Chắc chắn chưa?",
                                                "Cập Nhật",
                                                MessageBoxButtons.YesNo,
                                                MessageBoxIcon.Question // Icon dấu chấm hỏi
                                            );

                        if (dialogResult != DialogResult.Yes)
                            return;
                    }
                    try
                    {
                        bool isScussess = await SQLManager.Instance.updateOrdersAsync(orderId, exportCodeId, customerId, packingId, PCSOther, NWOther, priceCNF);

                        if (isScussess == true)
                        {
                            status_lb.Text = "Thành công.";
                            status_lb.ForeColor = System.Drawing.Color.Green;

                            row["ExportCodeID"] = exportCodeId;
                            row["CustomerID"] = customerId;
                            row["ProductPackingID"] = packingId;
                            row["PCSOther"] = PCSOther;
                            row["NWOther"] = NWOther;
                            row["OrderPackingPriceCNF"] = priceCNF;

                            
                            row["ExportCode"] = exportCodeRows.Length > 0 ? exportCodeRows[0]["ExportCode"].ToString() : "Unknown";
                            row["Amount"] = amount;
                            row["Package"] = package;
                            row["packing"] = packing;                               
                            row["ProductNameVN"] = proVN;                                

                          //  _ = SQLManager.Instance.InsertOrderLogAsync(exportCodeId, orderId, "Update M" + mode + " Thành Công", cusName, proVN, PCSOther, NWOther);
                        }
                        else
                        {
                            status_lb.Text = "Thất bại.";
                            status_lb.ForeColor = System.Drawing.Color.Red;
                          //  _ = SQLManager.Instance.InsertOrderLogAsync(exportCodeId, orderId, "Update M" + mode + " Thất Bại", cusName, proVN, PCSOther, NWOther);
                        }
                    }
                    catch (Exception ex)
                    {
                        status_lb.Text = "Thất bại.";
                        status_lb.ForeColor = System.Drawing.Color.Red;
                      //  _ = SQLManager.Instance.InsertOrderLogAsync(exportCodeId, orderId, "Update M" + mode + " Thất Bại Do Exception: " + ex.Message, cusName, proVN, PCSOther, NWOther);
                    }
                    
                    break;
                }
            }
        }

        private async void createOrder(int customerId, int exportCodeId, int packingId, int PCSOther, decimal NWOther, decimal priceCNF, bool isUpdateCusProduct)
        {
            int mode = isUpdateCusProduct ? 1 : 2;
            DataRow[] exportCodeRows = mOrderDomesticCode_dt.Select($"ExportCodeID = {exportCodeId}");
            DataRow[] packingRows = mProductPacking_dt.Select($"ProductPackingID = {packingId}");
            string proVN = packingRows.Length > 0 ? packingRows[0]["Name_VN"].ToString() : "Unknown";
            string package = packingRows.Length > 0 ? packingRows[0]["Package"].ToString() : "";
            string packing = packingRows.Length > 0 ? packingRows[0]["packing"].ToString() : "";
            int amount = packingRows.Length > 0 ? Convert.ToInt32(packingRows[0]["Amount"]) : 0;

            try
            {
                int newId = await SQLManager.Instance.insertOrderAsync(customerId, exportCodeId, packingId, PCSOther, NWOther, priceCNF);
                if (newId > 0)
                {
                    DataRow drToAdd = mOrderDomesticDetail_dt.NewRow();

                    drToAdd["OrderId"] = newId;
                    drToAdd["CustomerID"] = customerId;
                    drToAdd["ExportCodeID"] = exportCodeId;
                    drToAdd["ProductPackingID"] = packingId;
                    drToAdd["PCSOther"] = PCSOther;
                    drToAdd["NWOther"] = NWOther;
                        
                    drToAdd["OrderPackingPriceCNF"] = priceCNF;
                        

                    
                    drToAdd["Amount"] = amount;
                    drToAdd["Package"] = package;
                    drToAdd["packing"] = packing;
                    drToAdd["ExportCode"] = exportCodeRows.Length > 0 ? exportCodeRows[0]["ExportCode"].ToString() : "Unknown";
                    drToAdd["ProductNameVN"] = proVN;

                    int sku = Convert.ToInt32(packingRows[0]["SKU"]);
                    DataRow[] prodRows = mProduct_dt.Select($"SKU = {sku}");
                    if (prodRows.Length > 0)
                    {
                        drToAdd["Priority"] = Convert.ToInt32(prodRows[0]["Priority"]);
                    }
                    else
                    {
                        drToAdd["Priority"] = Convert.ToInt32(0);
                    }

                    mOrderDomesticDetail_dt.Rows.Add(drToAdd);
                    mOrderDomesticDetail_dt.AcceptChanges();

                    

                    status_lb.Text = "Thành công";
                    status_lb.ForeColor = System.Drawing.Color.Green;

                    if (isUpdateCusProduct)
                    {
                        newCustomerBtn_Click(null, null);
                    }

                  //  _ = SQLManager.Instance.InsertOrderLogAsync(exportCodeId, newId, "Create M" + mode + " Thành Công", cusName, proVN, PCSOther, NWOther);
                }
                else
                {
                    status_lb.Text = "Thất bại";
                    status_lb.ForeColor = System.Drawing.Color.Red;
                  //  _ = SQLManager.Instance.InsertOrderLogAsync(exportCodeId, newId, "Create M" + mode + " Thất Bại", cusName, proVN, PCSOther, NWOther);
                }
            }
            catch (Exception ex)
            {
                status_lb.Text = "Thất bại.";
                status_lb.ForeColor = System.Drawing.Color.Red;
               // _ = SQLManager.Instance.InsertOrderLogAsync(exportCodeId, -1, "Create M" + mode + " Thất Bại Do Exception: " + ex.Message, cusName, proVN, PCSOther, NWOther);
            }
        }
        private void saveBtn_Click(object sender, EventArgs e)
        {
            //if (exportCode_search_cbb.SelectedValue == null) return;

            //DataRowView pdData = (DataRowView)product_ccb.SelectedItem;
            //if (pdData == null) return;
            //string package = pdData["Package"].ToString();

            //bool isShowMessage = false;
            //if (customer_ccb.SelectedValue == null
            //    || packing_ccb.SelectedValue == null
            //    || string.IsNullOrWhiteSpace(netWeight_tb.Text)
            //    || string.IsNullOrWhiteSpace(priceCNF_tb.Text))
            //{
            //    isShowMessage = true;
            //}
            //else if (string.IsNullOrWhiteSpace(PCSOther_tb.Text) && package.CompareTo("weight") != 0){
            //    isShowMessage = true;
            //}

            //if (isShowMessage)
            //{
            //    MessageBox.Show(
            //                    "Thiếu Dữ Liệu, Kiểm Tra Lại!",
            //                    "Thông Báo",
            //                    MessageBoxButtons.OK,
            //                    MessageBoxIcon.Warning
            //                );
            //    return;
            //}

            //int exportCodeId = Convert.ToInt32(exportCode_search_cbb.SelectedValue);
            //int customerId = Convert.ToInt32(customer_ccb.SelectedValue);
            //int packingId = Convert.ToInt32(packing_ccb.SelectedValue);
            //int PCSOther = int.TryParse(PCSOther_tb.Text, out var value) ? value : 0;
            //decimal NWOther = Convert.ToDecimal(netWeight_tb.Text);
            //decimal priceCNF = Convert.ToDecimal(priceCNF_tb.Text);

            //if(PCSOther <= 0 && NWOther <= 0)
            //{
            //    MessageBox.Show(
            //                    "PCS hoặc Net Weight phải lớn hơn 0",
            //                    "Thông Báo",
            //                    MessageBoxButtons.OK,
            //                    MessageBoxIcon.Warning
            //                );
            //    return; 
            //}


            //if (orderId_tb.Text.Length != 0)
            //{
            //    int orderId = Convert.ToInt32(orderId_tb.Text);
            //    updateOrder(orderId, exportCodeId, customerId, packingId, PCSOther, NWOther, priceCNF, true);

            //}
            //else
            //{
            //    var rows = mOrderDomesticDetail_dt.AsEnumerable()
            //        .Where(r => r.Field<int>("ExportCodeID") == exportCodeId) // chỉ lấy ExportCode cần xử lý
            //        .ToList();

            //    if (CountDuplicateInDataGridView(rows, customerId, packingId, exportCodeId) == 0)
            //    {
            //        createOrder(customerId, exportCodeId, packingId, PCSOther, NWOther, priceCNF, true);
            //    }
            //    else
            //    {
            //        DialogResult dialogResult = MessageBox.Show(
            //                               "Khách hàng đã đặt đơn hàng này rồi",
            //                               "Thông Báo",
            //                               MessageBoxButtons.OK,
            //                               MessageBoxIcon.Warning); // Icon dấu chấm hỏi
            //    }
            //}
            

        }
        private void deleteOrderID( bool isUpdateCusPro)
        {
            if (dataGV.CurrentRow == null) return;
            int id = Convert.ToInt32(dataGV.CurrentRow.Cells["OrderId"].Value);
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
                
            deleteOrderProduct(id, isUpdateCusPro);
            
        }

        private async void deleteOrderProduct(int id, bool isUpdateCusPro)
        {
            int mode = isUpdateCusPro ? 1 : 2;

            foreach (DataRow row in mOrderDomesticDetail_dt.Rows)
            {
                int orderId = Convert.ToInt32(row["OrderId"]);
                if (id.CompareTo(orderId) == 0)
                {
                    int exportCodeId = Convert.ToInt32(row["ExportCodeID"]);
                    try
                    {   
                        bool isSuccess = await SQLManager.Instance.deleteOrderAsync(id);

                        if (isSuccess)
                        {
                            status_lb.Text = "Thành công.";
                            status_lb.ForeColor = System.Drawing.Color.Green;


                            _ = SQLManager.Instance.InsertOrderLogAsync(exportCodeId, orderId, "Delete M" + mode + " Thành Công ", "", "", 0, 0);
                            // Xóa row khỏi DataTable
                            mOrderDomesticDetail_dt.Rows.Remove(row);
                            mOrderDomesticDetail_dt.AcceptChanges();
                            
                        }
                        else
                        {
                            status_lb.Text = "Thất bại.";
                            status_lb.ForeColor = System.Drawing.Color.Red;
                            _ = SQLManager.Instance.InsertOrderLogAsync(exportCodeId, orderId, "Delete M" + mode + " Thất Bại ","", "", 0, 0);
                        }
                    }
                    catch (Exception ex)
                    {
                        status_lb.Text = "Thất bại.";
                        status_lb.ForeColor = System.Drawing.Color.Red;
                        _ = SQLManager.Instance.InsertOrderLogAsync(exportCodeId, orderId, "Delete M" + mode + " Thất Bại Do Exception: " + ex.Message,"", "", 0, 0);
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
            orderId_tb.Text = "";
            PCSOther_tb.Text = "";
            netWeight_tb.Text= "";
            isNewState = true;
            product_ccb_SelectedIndexChanged(null, null);

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
            //customer_ccb.Focus();

        }

        private void ReadOnly_btn_Click(object sender, EventArgs e)
        {
            edit_btn.Visible = true;
            newCustomerBtn.Visible = true;
            readOnly_btn.Visible = false;
            LuuThayDoiBtn.Visible = false;
            info_gb.BackColor = System.Drawing.Color.DarkGray;
            isNewState = false;
            product_ccb.DropDownStyle = ComboBoxStyle.DropDownList;
            setRightUIReadOnly(true);

            if (dataGV.SelectedRows.Count > 0)
                _ = updateRightUI(0);

        }

        private void Edit_btn_Click(object sender, EventArgs e)
        {
            isNewState = false;

            product_ccb_SelectedIndexChanged(null, null);


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
            if (exportCode_search_cbb.SelectedItem == null) return;

            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Message = "Đang xử lý ...";
            loadingOverlay.Show();
            await Task.Delay(100);

            DataRowView pakingData = (DataRowView)exportCode_search_cbb.SelectedItem;
            string exportCode = pakingData["ExportCode"].ToString();
            DateTime exportDate = Convert.ToDateTime(pakingData["ExportDate"]);

            System.Data.DataTable pendingOrderSummary = await SQLManager.Instance.getPendingOrderSummary(Convert.ToInt32(pakingData["ExportCodeID"]));

            OrderSummaryPrinter printer = new OrderSummaryPrinter();

            if(state == 1)
                printer.PrintPreview(pendingOrderSummary, exportCode, exportDate, this);
            else if (state == 2)
                printer.PrintDirect(pendingOrderSummary, exportCode, exportDate, tongdon_in2mat_cb.Checked);
            else
                printer.PrintDirectToPdf(pendingOrderSummary, exportCode, exportDate);
            await Task.Delay(200);
            loadingOverlay.Hide();
        }


        private async void exportExcel_TD_btn_Click(object sender, EventArgs e)
        {
            if (exportCode_search_cbb.SelectedItem == null) return;

            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Message = "Đang xử lý ...";
            loadingOverlay.Show();
            await Task.Delay(100);

            DataRowView pakingData = (DataRowView)exportCode_search_cbb.SelectedItem;
            string exportCode = pakingData["ExportCode"].ToString();

            System.Data.DataTable pendingOrderSummary = await SQLManager.Instance.getPendingOrderSummary(Convert.ToInt32(pakingData["ExportCodeID"]));

            DateTime exportDate = Convert.ToDateTime(((DataRowView)exportCode_search_cbb.SelectedItem)["ExportDate"]);
            try
            {
                using (var wb = new XLWorkbook())
                {
                    var ws = wb.Worksheets.Add("Data");
                    ws.Style.Font.FontName = "Arial";
                    ws.Style.Font.FontSize = 11;
                    ws.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    // Danh sách cột muốn xuất theo Name
                    string[] columnsToExport = new string[] { "ProductPackingName", "TotalPCSOther", "TotalNWOther" };
                    string[] columnslabel = new string[] {"STT", "Tên Sản Phẩm", "PCS", "N.W", "Ghi Chú", "In Tem", "", "", "","", "Còn", "Tổng" };
                    // Lọc cột hiển thị và có trong danh sách
                    var exportColumns = pendingOrderSummary.Columns.Cast<DataColumn>()
                        .Where(c => columnsToExport.Contains(c.ColumnName))
                        .OrderBy(c => Array.IndexOf(columnsToExport, c.ColumnName))
                        .ToList();



                    // Hàng 1: Tên công ty
                    ws.Range(1, 1, 1, columnslabel.Length).Merge();
                    ws.Cell(1, 1).Value = "TỔNG HỢP ĐƠN HÀNG THEO ĐÓNG GÓI";
                    ws.Cell(1, 1).Style.Font.Bold = true;
                    ws.Cell(1, 1).Style.Font.FontSize = 20;
                    ws.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    ws.Range(2, 1, 2, columnslabel.Length).Merge();
                    ws.Cell(2, 1).Value = exportCode_search_cbb.Text;
                    ws.Cell(2, 1).Style.Font.Bold = true;
                    ws.Cell(2, 1).Style.Font.FontSize = 14;
                    ws.Cell(2, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;


                    int headerStartRow = 3; // Header cấp 2 và 3 bắt đầu từ hàng 5

                    // ===== Title công ty =====
                    // Xác định vùng khung: từ hàng 1 đến 4, từ cột 1 đến exportColumns.Count
                    var headerRange = ws.Range(1, 1, 4, exportColumns.Count);

                    // Chỉ tạo viền ngoài, không có viền trong
                    headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    headerRange.Style.Border.OutsideBorderColor = XLColor.Black;

                    int colIndex = 1;
                    foreach (var label in columnslabel)
                    {
                        if (label.CompareTo("In Tem") == 0)
                        {
                            var range = ws.Range(headerStartRow, colIndex, headerStartRow, colIndex + 4);
                            range.Merge();
                            ws.Cell(headerStartRow, colIndex).Value = label;
                            ws.Cell(headerStartRow, colIndex).Style.Font.Bold = true;
                            ws.Cell(headerStartRow, colIndex).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Đóng khung
                            colIndex += 5;
                        }
                        else if(label.CompareTo("") != 0)
                        {
                            var range = ws.Range(headerStartRow, colIndex, headerStartRow + 1, colIndex);
                            ws.Cell(headerStartRow, colIndex).Value = label;
                            ws.Cell(headerStartRow, colIndex).Style.Font.Bold = true;
                            ws.Cell(headerStartRow, colIndex).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Đóng khung

                            colIndex++;
                        }
                    }

                    int rowIndex = headerStartRow + 1;
                    int sttcount = 1;
                    foreach (DataRow row in pendingOrderSummary.Rows)
                    {
                        var cellSTT = ws.Cell(rowIndex, 1);
                        cellSTT.Value = sttcount++;
                        cellSTT.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        cellSTT.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        colIndex = 2;
                        for (int i = 1; i < columnslabel.Length; i++)
                        {
                            var cell = ws.Cell(rowIndex, colIndex);
                            if (i <= exportColumns.Count)
                            {
                                string name = exportColumns[i - 1].ColumnName;
                                var cellValue = row[exportColumns[i - 1].ColumnName]?.ToString() ?? "";
                                cell.Value = cellValue;

                                
                                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                                if (name.CompareTo("TotalPCSOther") == 0)
                                {
                                    if (int.TryParse(cellValue, out int num))
                                    {
                                        cell.Value = num;
                                    }
                                }
                                else if (name.CompareTo("TotalNWOther") == 0)
                                {
                                    cell.Style.NumberFormat.Format = "0.00";
                                    if (decimal.TryParse(cellValue, out decimal num))
                                    {
                                        cell.Value = num;
                                    }
                                }
                            }
                            

                            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                            colIndex++;
                        }

                        rowIndex++;
                    }

                    ws.Columns().AdjustToContents();

                    for (int i = 1; i < columnslabel.Length; i++)
                    {
                        ws.Column(i).Width += 2;
                    }

                    // ===== Save file =====
                    using (SaveFileDialog sfd = new SaveFileDialog())
                    {
                        sfd.Filter = "Excel Workbook|*.xlsx";
                        sfd.FileName = "TongDon_" + exportCode + ".xlsx";
                        if (sfd.ShowDialog() == DialogResult.OK)
                        {
                            wb.SaveAs(sfd.FileName);
                            DialogResult result = MessageBox.Show("Bạn có muốn mở file này không?", "Lưu file thành công", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (result == DialogResult.Yes)
                            {
                                try
                                {
                                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                                    {
                                        FileName = sfd.FileName,
                                        UseShellExecute = true
                                    });
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("Không thể mở file: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xuất Excel: " + ex.Message);
            }
            finally
            {
                await Task.Delay(200);
                loadingOverlay.Hide();
            }
        }

        private int CountDuplicateInDataGridView(List<DataRow> dt, int customerId, int productPackingId, int exportCodeId)
        {
            int count = 0;

            foreach (DataRow row in dt)
            {

                // ép về int, nếu null thì gán 0
                int cust = row["CustomerID"] == null ? -1 : Convert.ToInt32(row["CustomerID"]);
                int prod = row["ProductPackingID"] == null ? -1 : Convert.ToInt32(row["ProductPackingID"]);
                int export = row["ExportCodeID"] == null ? -1 : Convert.ToInt32(row["ExportCodeID"]);

                if (cust == customerId && prod == productPackingId && export == exportCodeId)
                {
                    count++;
                }
            }

            return count;
        }

        private void setRightUIReadOnly(bool isReadOnly)
        {
            if (exportCode_search_cbb.SelectedItem != null)
            {
                DataRowView dataR = (DataRowView)exportCode_search_cbb.SelectedItem;

                string staff = dataR["InputByName_NoSign"].ToString();
                if (UserManager.Instance.fullName_NoSign.CompareTo(staff) != 0)
                {
                    edit_btn.Visible = false;
                    newCustomerBtn.Visible = false;
                    readOnly_btn.Visible = false;
                    product_ccb.Enabled = false;
                    packing_ccb.Enabled = false;
                    PCSOther_tb.ReadOnly = true;
                    netWeight_tb.ReadOnly = true;
                    return;
                }
            }
            product_ccb.Enabled = !isReadOnly;
            packing_ccb.Enabled = !isReadOnly;
            PCSOther_tb.ReadOnly = isReadOnly;
            netWeight_tb.ReadOnly = isReadOnly;
            product_ccb.DropDownStyle = ComboBoxStyle.DropDown;

            tongdon_gb.Visible = isReadOnly;
        }
    }
}
