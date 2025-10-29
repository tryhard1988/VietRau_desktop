using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Mysqlx.Crud;
using RauViet.classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace RauViet.ui
{
    public partial class OrdersList : Form
    {
        System.Data.DataTable mProductPacking_dt, mCustomers_dt, mProduct_dt, mExportCode_dt, mOrders_dt;
        private Timer debounceTimer = new Timer { Interval = 300 };
        private LoadingOverlay loadingOverlay;
        bool isNewState = false;
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
         //   this.dataGV.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.dataGV_RowPrePaint);
            PCSOther_tb.TextChanged += PCSOrder_tb_TextChanged;
            packing_ccb.SelectedIndexChanged += packing_ccb_SelectedIndexChanged;
            exportCode_search_cbb.SelectedIndexChanged += exportCode_search_cbb_SelectedIndexChanged;
            PCSOther_tb.KeyPress += Tb_KeyPress_OnlyNumber;
            netWeight_tb.KeyPress += Tb_KeyPress_OnlyNumber;


            debounceTimer.Tick += DebounceTimer_Tick;
            edit_btn.Click += Edit_btn_Click;
            readOnly_btn.Click += ReadOnly_btn_Click;
            ReadOnly_btn_Click(null, null);
        }

        public async void ShowData()
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            await Task.Delay(50);
            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Show();
            await Task.Delay(50);

            try
            {
                var othersTask = SQLManager.Instance.getOrdersAsync();
                var customersTask = SQLManager.Instance.getCustomersAsync();
                var productTask = SQLStore.Instance.getProductSKUAsync();
                var packingTask = SQLStore.Instance.getProductpackingAsync();

                string[] keepColumns = { "ExportCodeID", "ExportCode" };
                var parameters = new Dictionary<string, object>{{ "Complete", false }};
                var exportCodeTask = SQLStore.Instance.getExportCodesAsync(keepColumns, parameters);

                await Task.WhenAll(othersTask, customersTask, productTask, packingTask, exportCodeTask);

                mOrders_dt = othersTask.Result;
                mCustomers_dt = customersTask.Result;
                mProduct_dt = productTask.Result;
                mProductPacking_dt = packingTask.Result;
                mExportCode_dt = exportCodeTask.Result;

                mProductPacking_dt.Columns.Add(new DataColumn("PackingName", typeof(string)));
                mProductPacking_dt.Columns.Add(new DataColumn("ProductName", typeof(string)));

                foreach (DataRow dr in mProductPacking_dt.Rows)
                {
                    int sku = Convert.ToInt32(dr["SKU"]);
                    string packing = dr["packing"].ToString();
                    decimal amount = dr["Amount"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Amount"]);
                    string resultAmount = amount.ToString("0.##");

                    dr["PackingName"] = $"{resultAmount} {packing}";

                    DataRow[] prodRows = mProduct_dt.Select($"SKU = {sku}");
                    if (prodRows.Length > 0)
                    {
                        string productName = prodRows[0]["ProductNameVN"].ToString();
                        string package = prodRows[0]["Package"].ToString();

                        if (package.CompareTo("kg") == 0 && packing.CompareTo("") != 0 && amount > 0)
                        {
                            string amountStr = amount.ToString("0.##");
                            dr["ProductName"] = $"{productName} {amountStr} {packing}";
                        }
                        else
                        {
                            dr["ProductName"] = $"{productName}";
                        }

                        
                    }
                    else
                    {
                        dr["ProductPacking"] = "Unknown";
                    }
                }


                mOrders_dt.Columns.Add(new DataColumn("CustomerName", typeof(string)));
                mOrders_dt.Columns.Add(new DataColumn("ProductNameVN", typeof(string)));
                mOrders_dt.Columns.Add(new DataColumn("ExportCode", typeof(string)));

                foreach (DataRow dr in mOrders_dt.Rows)
                {
                    int customerID = Convert.ToInt32(dr["CustomerID"]);
                    DataRow[] customerRows = mCustomers_dt.Select($"CustomerID = {customerID}");
                    dr["CustomerName"] = customerRows.Length > 0 ? customerRows[0]["FullName"].ToString() : "Unknown";

                    int productPackingID = Convert.ToInt32(dr["ProductPackingID"]);
                    DataRow[] packingRows = mProductPacking_dt.Select($"ProductPackingID = {productPackingID}");
                    dr["ProductNameVN"] = packingRows.Length > 0 ? packingRows[0]["ProductName"].ToString() : "Unknown";

                    int exportCodeID = Convert.ToInt32(dr["ExportCodeID"]);
                    DataRow[] exportCodeRows = mExportCode_dt.Select($"ExportCodeID = {exportCodeID}");
                    dr["ExportCode"] = exportCodeRows.Length > 0 ? exportCodeRows[0]["ExportCode"].ToString() : "Unknown";
                }

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
                mOrders_dt.Columns["ExportCode"].SetOrdinal(0);
                mOrders_dt.Columns["CustomerName"].SetOrdinal(1);
                mOrders_dt.Columns["ProductNameVN"].SetOrdinal(2);
                mOrders_dt.Columns["OrderPackingPriceCNF"].SetOrdinal(3);
                mOrders_dt.Columns["PCSOther"].SetOrdinal(4);
                mOrders_dt.Columns["NWOther"].SetOrdinal(5);
                mOrders_dt.Columns["CustomerID"].SetOrdinal(6);
                mOrders_dt.Columns["ProductPackingID"].SetOrdinal(7);

                // Gán testData cho DataGridView tạm để test
                dataGV.DataSource = mOrders_dt;

                dataGV.Columns["OrderPackingPriceCNF"].HeaderText = "Giá CNF";
                dataGV.Columns["ExportCode"].HeaderText = "mã Xuất Cảng";
                dataGV.Columns["CustomerName"].HeaderText = "Tên Khách Hàng";
                dataGV.Columns["ProductNameVN"].HeaderText = "Tên Sản Phẩm";
                dataGV.Columns["PCSOther"].HeaderText = "PCS Order";
                dataGV.Columns["NWOther"].HeaderText = "NW Order";
                dataGV.Columns["Priority"].HeaderText = "ưu tiên";

                dataGV.Columns["OrderId"].Visible = false;
                dataGV.Columns["CustomerID"].Visible = false;
                dataGV.Columns["ProductPackingID"].Visible = false;
                dataGV.Columns["ExportCodeID"].Visible = false;
                dataGV.Columns["OrderPackingPriceCNF"].Visible = false;

                dataGV.Columns["OrderId"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGV.Columns["CustomerName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGV.Columns["ProductNameVN"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGV.Columns["PCSOther"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGV.Columns["NWOther"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

                dataGV.Columns["PCSOther"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGV.Columns["NWOther"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGV.Columns["Priority"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGV.AutoResizeColumns();


                exportCode_search_cbb.DataSource = mExportCode_dt.Copy();
                exportCode_search_cbb.DisplayMember = "ExportCode";  // hiển thị tên
                exportCode_search_cbb.ValueMember = "ExportCodeID";

                if (dataGV.SelectedRows.Count > 0)
                    updateRightUI(0);

            }
            catch (Exception ex)
            {
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
            debounceTimer.Stop();
            string typed = product_ccb.Text ?? "";
            string plain = Utils.RemoveVietnameseSigns(typed).ToLower();

            // Filter bằng LINQ thay vì RowFilter
            var filtered = mProduct_dt.AsEnumerable()
                .Where(r => Utils.RemoveVietnameseSigns(r["ProductNameVN"].ToString().ToLower())
                .Contains(plain));

            System.Data.DataTable temp;
            if (filtered.Any())
                temp = filtered.CopyToDataTable();
            else
                temp = mProduct_dt.Clone(); // nếu không có kết quả thì trả về table rỗng

            product_ccb.DataSource = temp;
            product_ccb.DisplayMember = "ProductNameVN";
            product_ccb.ValueMember = "SKU";

            // Giữ lại text người đang gõ
            product_ccb.DroppedDown = true;
            product_ccb.Text = typed;
            product_ccb.SelectionStart = typed.Length;
            product_ccb.SelectionLength = 0;
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
                dv.RowFilter = $"ExportCode = '{selectedExportCode}'";

                // Gán lại cho DataGridView
                dataGV.DataSource = dv;
            }
            else
            {
                // Nếu chưa chọn gì thì hiển thị toàn bộ
                dataGV.DataSource = mOrders_dt;
            }
        }

        private void product_ccb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (product_ccb.SelectedValue == null) return;

            int selectedSKU = Convert.ToInt32(product_ccb.SelectedValue);

            // Lọc packing theo SKU
            DataView dv = new DataView(mProductPacking_dt);
            dv.RowFilter = $"SKU = {selectedSKU}";

            packing_ccb.DataSource = dv;
            packing_ccb.DisplayMember = "PackingName"; // cột bạn muốn hiển thị
            packing_ccb.ValueMember = "ProductPackingID";

            priceCNF_tb.Text = ((DataRowView)product_ccb.SelectedItem)["PriceCNF"].ToString();

            if (dataGV.CurrentRow != null)
            {
                int productPackingID = Convert.ToInt32(dataGV.CurrentRow.Cells["ProductPackingID"].Value);

                if (packing_ccb.DataSource is System.Data.DataTable dt && dt.Rows.Count > 0)
                {
                    // Dùng Select để filter
                    DataRow[] found = dt.Select($"{packing_ccb.ValueMember} = {productPackingID}");

                    if (found.Length > 0)
                        packing_ccb.SelectedValue = productPackingID;
                    else
                        packing_ccb.SelectedIndex = 0; // chọn phần tử đầu tiên
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
        

        private void dataGV_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (e.RowIndex % 2 == 0)
            {
                dataGV.Rows[e.RowIndex].DefaultCellStyle.BackColor = System.Drawing.Color.Beige;
            }
        }
        

        private void dataGV_CellClick(object sender, EventArgs e)
        {
            if (dataGV.CurrentRow == null) return;

            int rowIndex = dataGV.CurrentRow.Index;
            if (rowIndex < 0)
                return;

            updateRightUI(rowIndex);
        }

        private void updateRightUI(int indexRowSelected)
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
            if(packingRows.Length > 0) 
                product_ccb.SelectedValue = packingRows[0]["SKU"].ToString();

            PCSOther_tb.Text = PCSOther;
            netWeight_tb.Text = NWOther;
            priceCNF_tb.Text = orderPackingPriceCNF;

            product_ccb_SelectedIndexChanged(null, null);
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
       

        private async void updateOrder(int orderId, int exportCodeId, int customerId, int packingId, int PCSOther, decimal NWOther, decimal priceCNF)
        {
            foreach (DataGridViewRow row in dataGV.Rows)
            {
                int id =Convert.ToInt32(row.Cells["OrderId"].Value);
                if (id.CompareTo(orderId) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show(
                                            "Chắc chắn chưa?",
                                            "Cập Nhật",
                                            MessageBoxButtons.YesNo,
                                            MessageBoxIcon.Question // Icon dấu chấm hỏi
                                        );

                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager.Instance.updateOrdersAsync(orderId, exportCodeId, customerId, packingId, PCSOther, NWOther, priceCNF);

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = System.Drawing.Color.Green;

                                row.Cells["OrderId"].Value = orderId;
                                row.Cells["ExportCodeID"].Value = exportCodeId;
                                row.Cells["CustomerID"].Value = customerId;
                                row.Cells["ProductPackingID"].Value = packingId;
                                row.Cells["PCSOther"].Value = PCSOther;
                                row.Cells["NWOther"].Value = NWOther;
                                row.Cells["OrderPackingPriceCNF"].Value = priceCNF;

                                DataRow[] exportCodeRows = mExportCode_dt.Select($"ExportCodeID = {exportCodeId}");
                                row.Cells["ExportCode"].Value = exportCodeRows.Length > 0 ? exportCodeRows[0]["ExportCode"].ToString() : "Unknown";

                                DataRow[] customerRows = mCustomers_dt.Select($"CustomerID = {customerId}");
                                row.Cells["CustomerName"].Value = customerRows.Length > 0 ? customerRows[0]["FullName"].ToString() : "Unknown";

                                DataRow[] packingRows = mProductPacking_dt.Select($"ProductPackingID = {packingId}");
                                row.Cells["ProductNameVN"].Value = packingRows.Length > 0 ? packingRows[0]["ProductName"].ToString() : "Unknown";
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
                    }
                    break;
                }
            }
        }

        private async void createOrder(int customerId, int exportCodeId, int packingId, int PCSOther, decimal NWOther, decimal priceCNF)
        {
            DialogResult dialogResult = MessageBox.Show(
                                            "Chắc chắn chưa?",
                                            "Tạo Mới",
                                            MessageBoxButtons.YesNo,
                                            MessageBoxIcon.Question // Icon dấu chấm hỏi
                                        );


            if (dialogResult == DialogResult.Yes)
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
                        drToAdd["ExportCode"] = exportCodeRows.Length > 0 ? exportCodeRows[0]["ExportCode"].ToString() : "Unknown";
                        DataRow[] customerRows = mCustomers_dt.Select($"CustomerID = {customerId}");
                        drToAdd["CustomerName"] = customerRows.Length > 0 ? customerRows[0]["FullName"].ToString() : "Unknown";
                        DataRow[] packingRows = mProductPacking_dt.Select($"ProductPackingID = {packingId}");
                        drToAdd["ProductNameVN"] = packingRows.Length > 0 ? packingRows[0]["ProductName"].ToString() : "Unknown";


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

                        newCustomerBtn_Click(null, null);
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
        }
        private void saveBtn_Click(object sender, EventArgs e)
        {
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

            if (CountDuplicateInDataGridView(dataGV, customerId, packingId, exportCodeId) == 0)
            {
                if (orderId_tb.Text.Length != 0)
                {
                    int orderId = Convert.ToInt32(orderId_tb.Text);
                    updateOrder(orderId, exportCodeId, customerId, packingId, PCSOther, NWOther, priceCNF);
                
                }
                else
                {
                    createOrder(customerId, exportCodeId, packingId, PCSOther, NWOther, priceCNF);
                }
            }
            else
            {
                DialogResult dialogResult = MessageBox.Show(
                                       "Trùng Dữ Liệu Rồi",
                                       "Thông Báo",
                                       MessageBoxButtons.OK,
                                       MessageBoxIcon.Warning); // Icon dấu chấm hỏi
            }

        }
        private async void deleteBtn_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(orderId_tb.Text);

            for (int i = mOrders_dt.Rows.Count - 1; i >= 0; i--)
            {
                DataRow row = mOrders_dt.Rows[i];
                int orderId = Convert.ToInt32(row["OrderId"]);
                if (id.CompareTo(orderId) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show(
                        "XÓA THÔNG ĐÓ NHA\nChắc chắn chưa?",
                        "Thông Báo",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning  // Thêm icon cảnh báo
                    );


                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isSuccess = await Task.Run(() => SQLManager.Instance.deleteOrderAsync(id));

                            if (isSuccess)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = System.Drawing.Color.Green;

                                // Xóa row khỏi DataTable
                                mOrders_dt.Rows.Remove(row);
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


        private void newCustomerBtn_Click(object sender, EventArgs e)
        {
            orderId_tb.Text = "";
            PCSOther_tb.Text = "";
            netWeight_tb.Text= "";
            customer_ccb.Focus();

            product_ccb_SelectedIndexChanged(null, null);

            info_gb.BackColor = newCustomerBtn.BackColor;
            edit_btn.Visible = false;
            newCustomerBtn.Visible = false;
            readOnly_btn.Visible = true;
            LuuThayDoiBtn.Visible = true;
            delete_btn.Visible = false;
            isNewState = true;
            LuuThayDoiBtn.Text = "Lưu Mới";
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
            LuuThayDoiBtn.Text = "Lưu C.Sửa";
        }

        private void PCSOrder_tb_TextChanged(object sender, EventArgs e)
        {
            updateNetWeight();
        }

        private void prewiew_print_DSDH_btn_Click(object sender, EventArgs e)
        {
            if (dataGV.Rows.Count == 0)
                return;

            string selectedExportCode = exportCode_search_cbb.Text;
            DataView dv = mOrders_dt.DefaultView;
            dv.RowFilter = $"ExportCode = '{selectedExportCode}'";
            dv.Sort = "ProductNameVN ASC, Priority ASC";
            dataGV.DataSource = dv;

            PrintDataGridView(dataGV, true);
        }

        private async void preview_print_TD_btn_Click(object sender, EventArgs e)
        {
            if (exportCode_search_cbb.SelectedItem == null) return;

            DataRowView pakingData = (DataRowView)exportCode_search_cbb.SelectedItem;
            string exportCode = pakingData["ExportCode"].ToString();
            DateTime exportDate = Convert.ToDateTime(pakingData["ExportDate"]);

            System.Data.DataTable pendingOrderSummary = await SQLManager.Instance.getPendingOrderSummary(Convert.ToInt32(pakingData["ExportCodeID"]));

            OrderSummaryPrinter printer = new OrderSummaryPrinter();

            printer.PrintPreview(pendingOrderSummary, exportCode, exportDate);
        }

        private void printOtherList_btn_Click(object sender, EventArgs err)
        {
            if (dataGV.Rows.Count == 0)
                return;

            string selectedExportCode = exportCode_search_cbb.Text;
            DataView dv = mOrders_dt.DefaultView;
            dv.RowFilter = $"ExportCode = '{selectedExportCode}'";
            dv.Sort = "ProductNameVN ASC, Priority ASC";
            dataGV.DataSource = dv;

            PrintDataGridView(dataGV);
        }
                
        private void PrintDataGridView(DataGridView dataGV, bool preview = false)
        {
            if (exportCode_search_cbb.SelectedItem == null) return;

            DataRowView dataR = (DataRowView)exportCode_search_cbb.SelectedItem;

            string staff = dataR["PackingByName"].ToString();

            if (string.IsNullOrWhiteSpace(staff))
                return;

            OrderListPrinter printer = new OrderListPrinter();
            if(!preview)
                printer.PrintDirect(dataGV, exportCode_search_cbb.Text, DateTime.Now, staff);
            else
                printer.PrintPreview(dataGV, exportCode_search_cbb.Text, DateTime.Now, staff);
        }

        private async void printPendingOrderSummary_btn_Click(object sender, EventArgs e)
        {
            if (exportCode_search_cbb.SelectedItem == null) return;

            DataRowView pakingData = (DataRowView)exportCode_search_cbb.SelectedItem;
            string exportCode = pakingData["ExportCode"].ToString();
            DateTime exportDate = Convert.ToDateTime(pakingData["ExportDate"]);

            System.Data.DataTable pendingOrderSummary = await SQLManager.Instance.getPendingOrderSummary(Convert.ToInt32(pakingData["ExportCodeID"]));

            OrderSummaryPrinter printer = new OrderSummaryPrinter();

            printer.PrintDirect(pendingOrderSummary, exportCode, exportDate);
           //printer.PrintPreview(pendingOrderSummary, exportCode, exportDate);
        }

        private async void exportExcel_TD_btn_Click(object sender, EventArgs e)
        {
            if (exportCode_search_cbb.SelectedItem == null) return;

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
                                var cellValue = row[exportColumns[i - 1].ColumnName]?.ToString() ?? "";
                                cell.Value = cellValue;

                                // Căn phải các cột số
                                var dataType = exportColumns[i - 1].DataType;
                                if (dataType == typeof(int) || dataType == typeof(decimal))
                                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                else
                                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                            }

                            

                            // Thêm border
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
                            MessageBox.Show("thành công\n" + sfd.FileName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xuất Excel: " + ex.Message);
            }
        }

        private int CountDuplicateInDataGridView(DataGridView dgv, int customerId, int productPackingId, int exportCodeId)
        {
            return 0;
            //int count = 0;

            //foreach (DataGridViewRow row in dgv.Rows)
            //{
            //    if (row.IsNewRow) continue;

            //    // ép về int, nếu null thì gán 0
            //    int cust = row.Cells["CustomerID"].Value == null ? -1 : Convert.ToInt32(row.Cells["CustomerID"].Value);
            //    int prod = row.Cells["ProductPackingID"].Value == null ? -1 : Convert.ToInt32(row.Cells["ProductPackingID"].Value);
            //    int export = row.Cells["ExportCodeID"].Value == null ? -1 : Convert.ToInt32(row.Cells["ExportCodeID"].Value);

            //    if (cust == customerId && prod == productPackingId && export == exportCodeId)
            //    {
            //        count++;
            //    }
            //}

            //return count;
        }
    }
}
