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
    public partial class OrdersList : Form
    {
        System.Data.DataTable mProductPacking_dt, mCustomers_dt, mProduct_dt, mExportCode_dt, mOrders_dt, mlatestOrders_dt;
        private Timer debounceTimer = new Timer { Interval = 150 };
        private LoadingOverlay loadingOverlay;
        bool isNewState = false;
        private int sortMode = 0;
        // private DataView dvProducts;
        public OrdersList()
        {
            InitializeComponent();

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            Utils.SetTabStopRecursive(this, false);

            int countTab = 0;
            customer_ccb.TabIndex = countTab++; customer_ccb.TabStop = true;
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
            delete_btn.Click += deleteBtn_Click;
            preview_print_TD_btn.Click += preview_print_TD_btn_Click;
            prewiew_print_DSDH_btn.Click += prewiew_print_DSDH_btn_Click;
            print_order_list_btn.Click += printOtherList_btn_Click;
            printPendingOrderSummary_btn.Click += printPendingOrderSummary_btn_Click;
            dataGV.SelectionChanged += this.dataGV_CellClick;
            PCSOther_tb.TextChanged += PCSOrder_tb_TextChanged;
            packing_ccb.SelectedIndexChanged += packing_ccb_SelectedIndexChanged;
            exportCode_search_cbb.SelectedIndexChanged += exportCode_search_cbb_SelectedIndexChanged;
            PCSOther_tb.KeyPress += Tb_KeyPress_OnlyNumber1;
            netWeight_tb.KeyPress += Tb_KeyPress_OnlyNumber;

            cusProduct_GV.CellEndEdit += CusProduct_GV_CellEndEdit; ;

            debounceTimer.Tick += DebounceTimer_Tick;
            edit_btn.Click += Edit_btn_Click;
            readOnly_btn.Click += ReadOnly_btn_Click;
            ReadOnly_btn_Click(null, null);

            Refesh_btn.Click += Refesh_btn_Click;
            search_tb.TextChanged += search_txt_TextChanged;

            customer_ccb.SelectedIndexChanged += Customer_ccb_SelectedIndexChanged;
            cusProduct_GV.CellFormatting += CusProduct_GV_CellFormatting;
        }

        private void CusProduct_GV_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;

           
            if (cusProduct_GV.Columns[e.ColumnIndex].Name == "PCSOther")
            {
                var row = cusProduct_GV.Rows[e.RowIndex];
                var packageVal = row.Cells["Package"].Value?.ToString();

                if (!string.Equals(packageVal, "weight", StringComparison.OrdinalIgnoreCase))
                {
                    row.Cells["NWOther"].ReadOnly = false;
                    e.CellStyle.BackColor = System.Drawing.Color.LightGray;
                }
                else
                {
                    row.Cells["PCSOther"].ReadOnly = true;
                    //  row.Cells["PCSReal"].Value = 0;
                }
            }
            else if (cusProduct_GV.Columns[e.ColumnIndex].Name == "NWOther")
            {
                var row = cusProduct_GV.Rows[e.RowIndex];
                var packageVal = row.Cells["Package"].Value?.ToString();

                if (string.Equals(packageVal, "weight", StringComparison.OrdinalIgnoreCase))
                {
                    row.Cells["NWOther"].ReadOnly = false;
                    e.CellStyle.BackColor = System.Drawing.Color.LightGray;
                }
                else
                {
                    row.Cells["NWOther"].ReadOnly = true;
                }
            }
        }

        private void Refesh_btn_Click(object sender, EventArgs e)
        {
            SQLStore.Instance.removeOrders();
            SQLStore.Instance.removeCustomers();
            SQLStore.Instance.removeProductSKU();
            SQLStore.Instance.removeProductpacking();
            SQLStore.Instance.removeExportCodes();
            SQLStore.Instance.removeExportCodes();
            SQLStore.Instance.removeLatestOrdersAsync();
            ShowData();
        }

        public async void ShowData()
        {
            await Task.Delay(50);
            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Show();
            await Task.Delay(50);

            try
            {
                string[] keepColumns = { "ExportCodeID", "ExportCode", "ExportDate", "PackingByName", "InputByName_NoSign" };
                var parameters = new Dictionary<string, object> { { "Complete", false } };
                var exportCodeTask = SQLStore.Instance.getExportCodesAsync(keepColumns, parameters);
                var customersTask = SQLStore.Instance.getCustomersAsync();
                var productTask = SQLStore.Instance.getProductSKUAsync();
                var packingTask = SQLStore.Instance.getProductpackingAsync();
                

                await Task.WhenAll(customersTask, productTask, packingTask, exportCodeTask);
                var othersTask = SQLStore.Instance.getOrdersAsync();
                var latestOrdersTask = SQLStore.Instance.get3LatestOrdersAsync();
                await Task.WhenAll(othersTask,latestOrdersTask);

                mOrders_dt = othersTask.Result;
                mCustomers_dt = customersTask.Result;
                mProduct_dt = productTask.Result;
                mProductPacking_dt = packingTask.Result;
                mExportCode_dt = exportCodeTask.Result;
                mlatestOrders_dt = latestOrdersTask.Result;

                cusProduct_GV.DataSource = mlatestOrders_dt;
                cusProduct_GV.Columns["CustomerID"].Visible = false;
                cusProduct_GV.Columns["ProductPackingID"].Visible = false;
                cusProduct_GV.Columns["Package"].Visible = false;
                cusProduct_GV.Columns["OrderPackingPriceCNF"].Visible = false;
                cusProduct_GV.Columns["packing"].Visible = false;
                cusProduct_GV.Columns["Amount"].Visible = false;


                cusProduct_GV.Columns["ProductNameVN"].HeaderText = "Tên Sản Phẩm";
                cusProduct_GV.Columns["PCSOther"].HeaderText = "PCS\nĐặt Hàng";
                cusProduct_GV.Columns["NWOther"].HeaderText = "NW\nĐặt Hàng";
                cusProduct_GV.Columns["ProductNameVN"].Width = 150;
                cusProduct_GV.Columns["PCSOther"].Width = 80;
                cusProduct_GV.Columns["NWOther"].Width = 80;
                cusProduct_GV.Columns["PCSOther"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                cusProduct_GV.Columns["NWOther"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                cusProduct_GV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                cusProduct_GV.ReadOnly = false;
                cusProduct_GV.Columns["PCSOther"].ReadOnly = false;
                cusProduct_GV.Columns["NWOther"].ReadOnly = false;
                cusProduct_GV.Columns["ProductNameVN"].ReadOnly = true;

                customer_ccb.DataSource = mCustomers_dt;
                customer_ccb.DisplayMember = "FullName";  // hiển thị tên
                customer_ccb.ValueMember = "CustomerID";
                customer_ccb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                customer_ccb.AutoCompleteSource = AutoCompleteSource.ListItems;


                product_ccb.DataSource = mProduct_dt;
                product_ccb.DisplayMember = "ProductNameVN";
                product_ccb.ValueMember = "SKU";
                product_ccb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                product_ccb.AutoCompleteSource = AutoCompleteSource.ListItems;
                product_ccb.SelectedIndexChanged += product_ccb_SelectedIndexChanged;
                product_ccb.TextUpdate += product_ccb_TextUpdate;

                foreach (DataColumn col in mOrders_dt.Columns)
                    col.ReadOnly = false;

                int count = 0;
                mOrders_dt.Columns["OrderId"].SetOrdinal(count++);
                mOrders_dt.Columns["ExportCode"].SetOrdinal(count++);
                mOrders_dt.Columns["CustomerName"].SetOrdinal(count++);
                mOrders_dt.Columns["ProductNameVN"].SetOrdinal(count++);
                mOrders_dt.Columns["OrderPackingPriceCNF"].SetOrdinal(count++);
                mOrders_dt.Columns["PCSOther"].SetOrdinal(count++);
                mOrders_dt.Columns["NWOther"].SetOrdinal(count++);
                mOrders_dt.Columns["PCSReal"].SetOrdinal(count++);
                mOrders_dt.Columns["NWReal"].SetOrdinal(count++);

                // Gán testData cho DataGridView tạm để test
                dataGV.DataSource = mOrders_dt;                

                dataGV.Columns["OrderPackingPriceCNF"].HeaderText = "Giá CNF";
                dataGV.Columns["ExportCode"].HeaderText = "mã Xuất Cảng";
                dataGV.Columns["CustomerName"].HeaderText = "Khách Hàng";
                dataGV.Columns["ProductNameVN"].HeaderText = "Tên Sản Phẩm";
                dataGV.Columns["PCSOther"].HeaderText = "PCS\nĐặt Hàng";
                dataGV.Columns["NWOther"].HeaderText = "NW\nĐặt Hàng";
                dataGV.Columns["Priority"].HeaderText = "ưu tiên";
                dataGV.Columns["OrderId"].HeaderText = "ID";

                //dataGV.Columns["OrderId"].Visible = false;
                dataGV.Columns["CustomerID"].Visible = false;
                dataGV.Columns["Search_NoSign"].Visible = false;
                dataGV.Columns["ProductPackingID"].Visible = false;
                dataGV.Columns["ExportCodeID"].Visible = false;
                dataGV.Columns["NWReal"].Visible = false;
              //  dataGV.Columns["OrderPackingPriceCNF"].Visible = false;
                dataGV.Columns["CartonSize"].Visible = false;
                dataGV.Columns["CustomerCarton"].Visible = false;
                dataGV.Columns["LOTCode"].Visible = false;
                dataGV.Columns["LOTCodeComplete"].Visible = false;
                dataGV.Columns["CustomerCode"].Visible = false;
                dataGV.Columns["ProductNameEN"].Visible = false;
                dataGV.Columns["Package"].Visible = false;
                dataGV.Columns["packing"].Visible = false;
                dataGV.Columns["Amount"].Visible = false;
                dataGV.Columns["ExportDate"].Visible = false;
                dataGV.Columns["ExportCode"].Visible = false;
                dataGV.Columns["PCSReal"].Visible = false;
                dataGV.Columns["CartonNo"].Visible = false;


                dataGV.Columns["OrderId"].Width = 60;
                dataGV.Columns["CustomerName"].Width = 120;
                dataGV.Columns["ProductNameVN"].Width = 150;
                dataGV.Columns["PCSOther"].Width = 80;
                dataGV.Columns["NWOther"].Width = 80;
                dataGV.Columns["OrderPackingPriceCNF"].Width = 80;
                dataGV.Columns["Priority"].Width = 50;

                dataGV.Columns["PCSOther"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGV.Columns["NWOther"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGV.Columns["Priority"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGV.Columns["OrderId"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                exportCode_search_cbb.DataSource = mExportCode_dt.Copy();
                exportCode_search_cbb.DisplayMember = "ExportCode";  // hiển thị tên
                exportCode_search_cbb.ValueMember = "ExportCodeID";

                if (mExportCode_dt.Rows.Count > 0)
                {
                    int maxID = Convert.ToInt32(mExportCode_dt.AsEnumerable()
                                   .Max(r => r.Field<int>("ExportCodeID")));
                    exportCode_search_cbb.SelectedValue = maxID;
                }

                if (dataGV.SelectedRows.Count > 0)
                    _ = updateRightUI(0);

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

        private void loadDataCusProduct()
        {
            if (exportCode_search_cbb.SelectedValue == null) return;
            if (!int.TryParse(exportCode_search_cbb.SelectedValue.ToString(), out int exportCodeId))
            {
                return;
            }

            
            foreach (DataRow row in mlatestOrders_dt.Rows)
            {
                object pkgValue = row["Package"];
                string package = pkgValue == DBNull.Value ? "" : pkgValue.ToString();
                row["PCSOther"] = DBNull.Value;
                row["NWOther"] = DBNull.Value;
                if (package.Equals("weight", StringComparison.OrdinalIgnoreCase))
                {
                    // Nếu là "weight" → đặt PCSOther = 0
                    row["PCSOther"] = 0;
                }
            }

            var rows = mOrders_dt.AsEnumerable()
                .Where(r => r.Field<int>("ExportCodeID") == exportCodeId) // chỉ lấy ExportCode cần xử lý
                .ToList();
            foreach (DataRow row in rows)
            {
                // Lấy giá trị PCSOther và NWOther
                int pcsOther = int.TryParse(row["PCSOther"]?.ToString(), out int pcs) ? pcs : 0;
                decimal nwOther = decimal.TryParse(row["NWOther"]?.ToString(), out decimal nw) ? nw : 0;

                // Chỉ xử lý khi có giá trị > 0
                if (pcsOther > 0 || nwOther > 0)
                {
                    int productPackingID = int.TryParse(row["ProductPackingID"]?.ToString(), out int pp) ? pp : 0;
                    int customerID = int.TryParse(row["CustomerID"]?.ToString(), out int cust) ? cust : 0;

                    // Tìm dòng tương ứng trong mlatestOrders_dt
                    DataRow[] foundRows = mlatestOrders_dt.Select(
                        $"ProductPackingID = {productPackingID} AND CustomerID = {customerID}");

                    if (foundRows.Length > 0)
                    {
                        DataRow targetRow = foundRows[0];

                        // Cập nhật các cột tương ứng
                        targetRow["PCSOther"] = pcsOther;
                        targetRow["NWOther"] = nwOther;
                    }
                    else
                    {
                        // Nếu chưa có dòng tương ứng thì thêm mới
                        DataRow newRow = mlatestOrders_dt.NewRow();
                        newRow["ProductPackingID"] = productPackingID;
                        newRow["ProductNameVN"] = row["ProductNameVN"]?.ToString() ?? "";
                        newRow["CustomerID"] = customerID;
                        newRow["PCSOther"] = pcsOther;
                        newRow["NWOther"] = nwOther;
                        mlatestOrders_dt.Rows.Add(newRow);
                    }
                }
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
                    .Where(r => r["ProductNameVN_NoSign"].ToString().Contains(plain))
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

        private void exportCode_search_cbb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mOrders_dt == null || mOrders_dt.Rows.Count == 0)
                return;

            string selectedExportCode = exportCode_search_cbb.Text;

            if (!string.IsNullOrEmpty(selectedExportCode))
            {
                // Tạo DataView để filter
                DataView dv = new DataView(mOrders_dt);
                var safeCode = selectedExportCode.Replace("'", "''"); // escape ký tự '
                dv.RowFilter = $"ExportCode = '{safeCode}' AND (PCSOther > 0 OR NWOther > 0)";

                // Gán lại cho DataGridView
                dataGV.DataSource = dv;
            }
            else
            {
                // Nếu chưa chọn gì thì hiển thị toàn bộ
                dataGV.DataSource = mOrders_dt;
            }

            loadDataCusProduct();
            ReadOnly_btn_Click(null, null);
        }

        private void product_ccb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (product_ccb.SelectedValue == null) return;

            int selectedSKU = Convert.ToInt32(product_ccb.SelectedValue);

            // Lọc packing theo SKU
            DataView dv = new DataView(mProductPacking_dt);
            dv.RowFilter = $"SKU = '{selectedSKU}'";

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
        
        private void dataGV_CellClick(object sender, EventArgs e)
        {
            if (exportCode_search_cbb.SelectedValue == null) return;
            if (dataGV.CurrentRow == null) return;

            var currentRow = dataGV.CurrentRow;

            _ = updateRightUI(currentRow.Index);

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

            var rows = mOrders_dt.AsEnumerable()
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

            total_label.Text ="[" + sumPCS + " pcs, " + sumNW + " kg" + "]";
            cus_name_label.Text = Convert.ToString(currentRow.Cells["CustomerName"].Value) + ": ";
            cus_lable.Text = "[" + (sumPCS_cus.ToString() + " pcs, ") + (sumNW_cus.ToString() + " kg") + "]";

            
        }

        private void Customer_ccb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (customer_ccb.SelectedValue == null) return;

            int customerId = 0;

            if (int.TryParse(customer_ccb.SelectedValue.ToString(), out customerId))
            {
                DataView dv = new DataView(mlatestOrders_dt);
                if (!isNewState)
                {
                    dv.RowFilter = $"CustomerID = {customerId} AND (NWOther > 0 OR PCSOther > 0)";
                }
                else
                {
                    dv.RowFilter = $"CustomerID = {customerId}";
                }

                cusProduct_GV.DataSource = dv;
            }
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
            customer_ccb.SelectedValue = customerID;

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

           // product_ccb_SelectedIndexChanged(null, null);
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
       

        private async void updateOrder(int orderId, int exportCodeId, int customerId, int packingId, int PCSOther, decimal NWOther, decimal priceCNF, bool isShowDialog = true)
        {
            var rows = mOrders_dt.AsEnumerable()
                .Where(r => r.Field<int>("ExportCodeID") == exportCodeId) // chỉ lấy ExportCode cần xử lý
                .ToList();

            foreach (DataRow row in rows)
            {
                int id =Convert.ToInt32(row["OrderId"]);
                if (id.CompareTo(orderId) == 0)
                {
                    if (isShowDialog)
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

                            row["OrderId"] = orderId;
                            row["ExportCodeID"] = exportCodeId;
                            row["CustomerID"] = customerId;
                            row["ProductPackingID"] = packingId;
                            row["PCSOther"] = PCSOther;
                            row["NWOther"] = NWOther;
                            row["OrderPackingPriceCNF"] = priceCNF;

                            DataRow[] exportCodeRows = mExportCode_dt.Select($"ExportCodeID = {exportCodeId}");
                            DataRow[] customerRows = mCustomers_dt.Select($"CustomerID = {customerId}");
                            DataRow[] packingRows = mProductPacking_dt.Select($"ProductPackingID = {packingId}");
                            string cusName = customerRows.Length > 0 ? customerRows[0]["FullName"].ToString() : "Unknown";
                            string proVN = packingRows.Length > 0 ? packingRows[0]["Name_VN"].ToString() : "Unknown";
                            row["ExportCode"] = exportCodeRows.Length > 0 ? exportCodeRows[0]["ExportCode"].ToString() : "Unknown";
                            row["Amount"] = packingRows.Length > 0 ? Convert.ToInt32(packingRows[0]["Amount"]) : 0;
                            row["Package"] = packingRows.Length > 0 ? packingRows[0]["Package"].ToString() : "";
                            row["packing"] = packingRows.Length > 0 ? packingRows[0]["packing"].ToString() : "";
                            row["CustomerName"] = cusName;                                
                            row["ProductNameVN"] = proVN;                                
                            row["Search_NoSign"] = Utils.RemoveVietnameseSigns(cusName + " " + proVN).ToLower();
                        }
                        else
                        {
                            status_lb.Text = "Thất bại.";
                            status_lb.ForeColor = System.Drawing.Color.Red;
                        }
                    }
                    catch (Exception ex)
                    {
                        status_lb.Text = "Thất bại.";
                        status_lb.ForeColor = System.Drawing.Color.Red;
                    }
                    
                    break;
                }
            }
        }

        private async void createOrder(int customerId, int exportCodeId, int packingId, int PCSOther, decimal NWOther, decimal priceCNF, bool isUpdateScroll = true)
        {
            try
            {
                int newId = await SQLManager.Instance.insertOrderAsync(customerId, exportCodeId, packingId, PCSOther, NWOther, priceCNF);
                if (newId > 0)
                {
                    DataRow drToAdd = mOrders_dt.NewRow();

                    drToAdd["OrderId"] = newId;
                    drToAdd["CustomerID"] = customerId;
                    drToAdd["ExportCodeID"] = exportCodeId;
                    drToAdd["ProductPackingID"] = packingId;
                    drToAdd["PCSOther"] = PCSOther;
                    drToAdd["NWOther"] = NWOther;
                        
                    drToAdd["OrderPackingPriceCNF"] = priceCNF;
                        

                    DataRow[] exportCodeRows = mExportCode_dt.Select($"ExportCodeID = {exportCodeId}");
                    DataRow[] customerRows = mCustomers_dt.Select($"CustomerID = {customerId}");
                    DataRow[] packingRows = mProductPacking_dt.Select($"ProductPackingID = {packingId}");
                    string cusName = customerRows.Length > 0 ? customerRows[0]["FullName"].ToString() : "Unknown";
                    string proVN = packingRows.Length > 0 ? packingRows[0]["Name_VN"].ToString() : "Unknown";
                    drToAdd["Amount"] = packingRows.Length > 0 ? Convert.ToInt32(packingRows[0]["Amount"]) : 0;
                    drToAdd["Package"] = packingRows.Length > 0 ? packingRows[0]["Package"].ToString() : "";
                    drToAdd["packing"] = packingRows.Length > 0 ? packingRows[0]["packing"].ToString() : "";
                    drToAdd["ExportCode"] = exportCodeRows.Length > 0 ? exportCodeRows[0]["ExportCode"].ToString() : "Unknown";
                    drToAdd["CustomerName"] = cusName;
                    drToAdd["ProductNameVN"] = proVN;
                    drToAdd["Search_NoSign"] = Utils.RemoveVietnameseSigns(cusName + " " + proVN).ToLower();

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

                    mOrders_dt.Rows.Add(drToAdd);
                    mOrders_dt.AcceptChanges();

                    status_lb.Text = "Thành công";
                    status_lb.ForeColor = System.Drawing.Color.Green;

                    if (isUpdateScroll)
                    {
                        newCustomerBtn_Click(null, null);
                    }
                }
                else
                {
                    status_lb.Text = "Thất bại";
                    status_lb.ForeColor = System.Drawing.Color.Red;
                }
            }
            catch (Exception ex)
            {
                status_lb.Text = "Thất bại.";
                status_lb.ForeColor = System.Drawing.Color.Red;
            }
        }
        private void saveBtn_Click(object sender, EventArgs e)
        {
            if (exportCode_search_cbb.SelectedValue == null) return;

            DataRowView pdData = (DataRowView)product_ccb.SelectedItem;
            if (pdData == null) return;
            string package = pdData["Package"].ToString();

            bool isShowMessage = false;
            if (customer_ccb.SelectedValue == null
                || packing_ccb.SelectedValue == null
                || string.IsNullOrWhiteSpace(netWeight_tb.Text)
                || string.IsNullOrWhiteSpace(priceCNF_tb.Text))
            {
                isShowMessage = true;
            }
            else if (string.IsNullOrWhiteSpace(PCSOther_tb.Text) && package.CompareTo("weight") != 0){
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

            int exportCodeId = Convert.ToInt32(exportCode_search_cbb.SelectedValue);
            int customerId = Convert.ToInt32(customer_ccb.SelectedValue);
            int packingId = Convert.ToInt32(packing_ccb.SelectedValue);
            int PCSOther = int.TryParse(PCSOther_tb.Text, out var value) ? value : 0;
            decimal NWOther = Convert.ToDecimal(netWeight_tb.Text);
            decimal priceCNF = Convert.ToDecimal(priceCNF_tb.Text);

            if(PCSOther <= 0 && NWOther <= 0)
            {
                MessageBox.Show(
                                "PCS hoặc Net Weight phải lớn hơn 0",
                                "Thông Báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning
                            );
                return; 
            }


            if (orderId_tb.Text.Length != 0)
            {
                int orderId = Convert.ToInt32(orderId_tb.Text);
                updateOrder(orderId, exportCodeId, customerId, packingId, PCSOther, NWOther, priceCNF);

            }
            else
            {
                var rows = mOrders_dt.AsEnumerable()
                    .Where(r => r.Field<int>("ExportCodeID") == exportCodeId) // chỉ lấy ExportCode cần xử lý
                    .ToList();

                if (CountDuplicateInDataGridView(rows, customerId, packingId, exportCodeId) == 0)
                {
                    createOrder(customerId, exportCodeId, packingId, PCSOther, NWOther, priceCNF);
                }
                else
                {
                    DialogResult dialogResult = MessageBox.Show(
                                           "Khách hàng đã đặt đơn hàng này rồi",
                                           "Thông Báo",
                                           MessageBoxButtons.OK,
                                           MessageBoxIcon.Warning); // Icon dấu chấm hỏi
                }
            }
            

        }
        private void deleteBtn_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show(
                       "XÓA THÔNG ĐÓ NHA\nChắc chắn chưa?",
                       "Thông Báo",
                       MessageBoxButtons.YesNo,
                       MessageBoxIcon.Warning  // Thêm icon cảnh báo
                   );


            if (dialogResult == DialogResult.Yes)
            {
                int id = Convert.ToInt32(orderId_tb.Text);
                deleteOrderProduct(id);
            }
        }

        private async void deleteOrderProduct(int id)
        {
            foreach (DataRow row in mOrders_dt.Rows)
            {
                int orderId = Convert.ToInt32(row["OrderId"]);
                if (id.CompareTo(orderId) == 0)
                {
                    try
                    {
                        bool isSuccess = await SQLManager.Instance.deleteOrderAsync(id);

                        if (isSuccess)
                        {
                            status_lb.Text = "Thành công.";
                            status_lb.ForeColor = System.Drawing.Color.Green;

                            // Xóa row khỏi DataTable
                            mOrders_dt.Rows.Remove(row);
                            mOrders_dt.AcceptChanges();
                        }
                        else
                        {
                            status_lb.Text = "Thất bại.";
                            status_lb.ForeColor = System.Drawing.Color.Red;
                        }
                    }
                    catch (Exception ex)
                    {
                        status_lb.Text = "Thất bại.";
                        status_lb.ForeColor = System.Drawing.Color.Red;
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

            product_ccb_SelectedIndexChanged(null, null);

            info_gb.BackColor = newCustomerBtn.BackColor;
            edit_btn.Visible = false;
            newCustomerBtn.Visible = false;
            readOnly_btn.Visible = true;
            LuuThayDoiBtn.Visible = true;
            delete_btn.Visible = false;
            isNewState = true;
            cusProduct_GV.Visible = true;
            LuuThayDoiBtn.Text = "Lưu Mới";            
            setRightUIReadOnly(false);
            if (dataGV.Rows.Count > 0)
            {
                dataGV.FirstDisplayedScrollingRowIndex = dataGV.Rows.Count - 1;
            }
            customer_ccb.Focus();
        }

        private void ReadOnly_btn_Click(object sender, EventArgs e)
        {
            edit_btn.Visible = true;
            newCustomerBtn.Visible = true;
            readOnly_btn.Visible = false;
            LuuThayDoiBtn.Visible = false;
            delete_btn.Visible = false;
            info_gb.BackColor = System.Drawing.Color.DarkGray;
            isNewState = false;
            cusProduct_GV.Visible = false;
            product_ccb.DropDownStyle = ComboBoxStyle.DropDownList;
            setRightUIReadOnly(true);
        }

        private void Edit_btn_Click(object sender, EventArgs e)
        {
            edit_btn.Visible = false;
            newCustomerBtn.Visible = false;
            readOnly_btn.Visible = true;
            LuuThayDoiBtn.Visible = true;
            delete_btn.Visible = true;
            info_gb.BackColor = edit_btn.BackColor;
            isNewState = false;
            cusProduct_GV.Visible = true;
            LuuThayDoiBtn.Text = "Lưu C.Sửa";
            setRightUIReadOnly(false);
        }

        private void PCSOrder_tb_TextChanged(object sender, EventArgs e)
        {
            updateNetWeight();
        }

        private void prewiew_print_DSDH_btn_Click(object sender, EventArgs e)
        {
            PrintDataGridView(dataGV, true);
        }

        private void preview_print_TD_btn_Click(object sender, EventArgs e)
        {
            _ = PrintPendingOrderSummary(1);
        }

        private void printOtherList_btn_Click(object sender, EventArgs err)
        {
            if (dataGV.Rows.Count == 0)
                return;

            PrintDataGridView(dataGV);
        }
                
        private async void PrintDataGridView(DataGridView dataGV, bool preview = false)
        {
            if (exportCode_search_cbb.SelectedItem == null) return;

            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Message = "Đang xử lý ...";
            loadingOverlay.Show();
            await Task.Delay(100);

            string selectedExportCode = exportCode_search_cbb.Text;
            DataView dv = mOrders_dt.DefaultView;
            dv.RowFilter = $"ExportCode = '{selectedExportCode}'";
            dv.Sort = "Priority ASC, ProductNameVN ASC, CustomerName ASC";
            dataGV.DataSource = dv;

            DataRowView dataR = (DataRowView)exportCode_search_cbb.SelectedItem;

            string staff = dataR["PackingByName"].ToString();

            if (string.IsNullOrWhiteSpace(staff))
            {
                await Task.Delay(200);
                loadingOverlay.Hide();
                return;
            }
            
            OrderListPrinter printer = new OrderListPrinter();
            if(!preview)
                printer.PrintDirect(dataGV, exportCode_search_cbb.Text, DateTime.Now, staff, dsdd_in2mat_cb.Checked);
            else
                printer.PrintPreview(dataGV, exportCode_search_cbb.Text, DateTime.Now, staff, this);

            await Task.Delay(200);
            loadingOverlay.Hide();
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
                    customer_ccb.Enabled = false;
                    product_ccb.Enabled = false;
                    packing_ccb.Enabled = false;
                    PCSOther_tb.ReadOnly = true;
                    netWeight_tb.ReadOnly = true;
                    return;
                }
            }
            customer_ccb.Enabled = !isReadOnly;
            product_ccb.Enabled = !isReadOnly;
            packing_ccb.Enabled = !isReadOnly;
            PCSOther_tb.ReadOnly = isReadOnly;
            netWeight_tb.ReadOnly = isReadOnly;
            product_ccb.DropDownStyle = ComboBoxStyle.DropDown;

            dddh_gb.Visible = isReadOnly;
            tongdon_gb.Visible = isReadOnly;
            if (!isReadOnly)
            {
                Customer_ccb_SelectedIndexChanged(null, null);
            }
        }

        
        private void search_txt_TextChanged(object sender, EventArgs e)
        {
            string selectedExportCode = exportCode_search_cbb.Text;

            if (string.IsNullOrEmpty(selectedExportCode))
                return;

            string keyword = Utils.RemoveVietnameseSigns(search_tb.Text.Trim().ToLower()).Replace("'", "''"); // tránh lỗi cú pháp '

            DataView currentView = dataGV.DataSource is DataView dv ? dv : new DataView(mOrders_dt);
            string filter = "";
            if (!string.IsNullOrEmpty(keyword))
                filter += $"[Search_NoSign] LIKE '%{keyword}%'";  // escape dấu nháy đơn

            if (!string.IsNullOrEmpty(selectedExportCode))
            {
                if (filter.Length > 0) filter += " AND ";
                filter += $"ExportCode = '{selectedExportCode}'";
            }

            if (filter.Length > 0) filter += " AND ";
            filter += "(PCSOther > 0 OR NWOther > 0)";
            currentView.RowFilter = filter;
            dataGV.DataSource = currentView;
        }

        private void CusProduct_GV_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // Chỉ xử lý nếu vị trí hợp lệ
                if (e.RowIndex < 0 || e.ColumnIndex < 0)
                    return;

                var columnName = cusProduct_GV.Columns[e.ColumnIndex].Name;
                               

                // Lấy ExportCodeID đang chọn
                if (exportCode_search_cbb.SelectedValue == null)
                    return;

                int exportCodeId = Convert.ToInt32(exportCode_search_cbb.SelectedValue);

                var rows = mOrders_dt.AsEnumerable()
                    .Where(r => r.Field<int>("ExportCodeID") == exportCodeId) // chỉ lấy ExportCode cần xử lý
                    .ToList();

                var row = cusProduct_GV.Rows[e.RowIndex];
                if (row == null)
                    return;
                if (!int.TryParse(row.Cells["CustomerID"]?.Value?.ToString(), out int customerId))
                    return;
                if (!int.TryParse(row.Cells["ProductPackingID"]?.Value?.ToString(), out int productPackingID))
                    return;
   
                    
                int? PCSOther = int.TryParse(row.Cells["PCSOther"]?.Value?.ToString(), out int pcs) ? pcs : (int?)null;
                decimal? NWOther = decimal.TryParse(row.Cells["NWOther"]?.Value?.ToString(), out decimal nw) ? nw : (decimal?)null;
                decimal price = decimal.TryParse(row.Cells["OrderPackingPriceCNF"]?.Value?.ToString(), out decimal pr) ? pr : 0;
                decimal amount = decimal.TryParse(row.Cells["Amount"]?.Value?.ToString(), out decimal am) ? am : 0;
                string packing = row.Cells["packing"].Value?.ToString() ?? "";
                string package = row.Cells["Package"]?.Value?.ToString() ?? "";
                DataRow orderRow = null;

                if (columnName == "PCSOther")
                {
                    if(PCSOther == null || PCSOther <= 0)
                    {
                        row.Cells["NWOther"].Value = DBNull.Value;
                        NWOther = null;
                    }
                }

                foreach (DataRow r in rows)
                {
                    int rowCustomerId = Convert.ToInt32(r["CustomerID"]);
                    int rowProductPackingID = Convert.ToInt32(r["ProductPackingID"]);

                    int pcsOther = int.TryParse(r["PCSOther"]?.ToString(), out int pcs1) ? pcs1 : 0;
                    decimal nwOther = decimal.TryParse(r["NWOther"]?.ToString(), out decimal nw1) ? nw1 : 0;                    

                    if (rowCustomerId == customerId &&
                        rowProductPackingID == productPackingID &&
                        (pcsOther > 0 || nwOther > 0))
                    {
                        orderRow = r;
                        break; // lấy dòng đầu tiên thỏa điều kiện
                    }
                }

                if (orderRow != null)
                {
                    int orderId = Convert.ToInt32(orderRow["OrderId"]);
                    if ( (NWOther ?? 0) <= 0)
                    {
                        deleteOrderProduct(orderId);
                    }
                    else
                    {
                        if (package.CompareTo("weight") != 0)
                        {
                            NWOther = Utils.calNetWeight(PCSOther ?? 0, (int)amount, packing);
                            row.Cells["NWOther"].Value = NWOther;
                        }
                        else
                        {
                            PCSOther = 0;
                            row.Cells["PCSOther"].Value = PCSOther;
                        }
                                                
                        decimal price1 = Convert.ToDecimal(orderRow["OrderPackingPriceCNF"]);
                        updateOrder(orderId, exportCodeId, customerId, productPackingID, PCSOther ?? 0, NWOther ?? 0, price1, false);
                    }
                }
                else
                {
                    if ((PCSOther ?? 0) <= 0 && (NWOther ?? 0) <= 0)
                        return;

                    if (package.CompareTo("weight") != 0)
                    {
                        NWOther = Utils.calNetWeight(PCSOther ?? 0, (int)amount, packing);
                        row.Cells["NWOther"].Value = NWOther;
                    }
                    else
                    {
                        PCSOther = 0;
                        row.Cells["PCSOther"].Value = PCSOther;
                    }

                    createOrder(customerId, exportCodeId, productPackingID, PCSOther ?? 0, NWOther ?? 0, price, false);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xử lý thay đổi dữ liệu:\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
