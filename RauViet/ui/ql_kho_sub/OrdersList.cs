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
using Mysqlx.Crud;
using RauViet.classes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace RauViet.ui
{
    public partial class OrdersList : Form
    {
        DataTable mProductPacking_dt, mCustomers_dt, mProduct_dt, mExportCode_dt, mOrders_dt;
        public OrdersList()
        {
            InitializeComponent();

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            status_lb.Text = "";
            loading_lb.Text = "Đang tải dữ liệu, vui lòng chờ...";
            loading_lb.Visible = false;
            delete_btn.Enabled = false;

            newCustomerBtn.Click += newCustomerBtn_Click;
            LuuThayDoiBtn.Click += saveBtn_Click;
            delete_btn.Click += deleteBtn_Click;
            print_order_list_btn.Click += printOtherList_btn_Click;
            printPendingOrderSummary_btn.Click += printPendingOrderSummary_btn_Click;
            dataGV.SelectionChanged += this.dataGV_CellClick;
            this.dataGV.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.dataGV_RowPrePaint);
            PCSOther_tb.TextChanged += PCSOrder_tb_TextChanged;
            packing_ccb.SelectedIndexChanged += packing_ccb_SelectedIndexChanged;
            exportCode_search_cbb.SelectedIndexChanged += exportCode_search_cbb_SelectedIndexChanged;
            PCSOther_tb.KeyPress += Tb_KeyPress_OnlyNumber;

            orderId_tb.Visible = false;
            label1.Visible = false;
            priceCNF_tb.Visible = false;
            label4.Visible = false;
        }

        public async void ShowData()
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            loading_lb.Visible = true;            

            try
            {
                var othersTask = SQLManager.Instance.getOrdersAsync();
                var customersTask = SQLManager.Instance.getCustomersAsync();
                var productTask = SQLManager.Instance.getProductSKUAsync();
                var packingTask = SQLManager.Instance.getProductpackingAsync();
                var exportCodeTask = SQLManager.Instance.getExportCodes_Incomplete();

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
                    string amount = dr["Amount"].ToString();
                    string packing = dr["packing"].ToString();
                    dr["PackingName"] = $"{amount} {packing}";

                    DataRow[] prodRows = mProduct_dt.Select($"SKU = {sku}");
                    if (prodRows.Length > 0)
                    {
                        string productName = prodRows[0]["ProductNameVN"].ToString();
                        dr["ProductName"] = $"{productName} {amount} {packing}";
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


                exportCode_cbb.DataSource = mExportCode_dt;
                exportCode_cbb.DisplayMember = "ExportCode";  // hiển thị tên
                exportCode_cbb.ValueMember = "ExportCodeID";

                customer_ccb.DataSource = mCustomers_dt;
                customer_ccb.DisplayMember = "FullName";  // hiển thị tên
                customer_ccb.ValueMember = "CustomerID";

                product_ccb.DataSource = mProduct_dt;
                product_ccb.DisplayMember = "ProductNameVN";  // hiển thị tên
                product_ccb.ValueMember = "SKU";
                product_ccb.SelectedIndexChanged += product_ccb_SelectedIndexChanged;

                
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
                dataGV.Columns["PCSReal"].HeaderText = "PCS Real";
                dataGV.Columns["NWReal"].HeaderText = "NW Real";

                dataGV.Columns["OrderId"].Visible = false;
                dataGV.Columns["CustomerID"].Visible = false;
                dataGV.Columns["ProductPackingID"].Visible = false;
                dataGV.Columns["ExportCodeID"].Visible = false;
                dataGV.Columns["NWReal"].Visible = false;
                dataGV.Columns["PCSReal"].Visible = false;
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
                status_lb.ForeColor = Color.Red;
            }
            finally
            {
                loading_lb.Visible = false; // ẩn loading
                loading_lb.Enabled = true; // enable lại button
            }
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
                packing_ccb.SelectedValue = productPackingID;
            }
        }

        private void packing_ccb_SelectedIndexChanged(object sender, EventArgs e)
        {
            netWeight_tb.Text = calNetWeight().ToString();
        }
        

        private void dataGV_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (e.RowIndex % 2 == 0)
            {
                dataGV.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Beige;
            }
        }
        
        private int createID()
        {
            var existingIds = dataGV.Rows
            .Cast<DataGridViewRow>()
            .Where(r => !r.IsNewRow && r.Cells["CustomerID"].Value != null)
            .Select(r => Convert.ToInt32(r.Cells["CustomerID"].Value))
            .ToList();

            int newCustomerID = existingIds.Count > 0 ? existingIds.Max() + 1 : 1;

            return newCustomerID;
        }

        private void dataGV_CellClick(object sender, EventArgs e)
        {
            int rowIndex = dataGV.CurrentRow.Index;
            if (rowIndex < 0)
                return;

            updateRightUI(rowIndex);
        }

        private void updateRightUI(int indexRowSelected)
        {
            if (indexRowSelected < 0)
                return;
            var cells = dataGV.Rows[indexRowSelected].Cells;
            string orderId = cells["OrderId"].Value.ToString();
            string customerID = cells["CustomerID"].Value.ToString();
            string exportCodeId = cells["CustomerID"].Value.ToString();
            string packingID = cells["ProductPackingID"].Value.ToString();
            string PCSOther = cells["PCSOther"].Value.ToString();
            string NWOther = cells["NWOther"].Value.ToString();
            string orderPackingPriceCNF = cells["OrderPackingPriceCNF"].Value.ToString();

            orderId_tb.Text = orderId;
            customer_ccb.SelectedValue = customerID;
            exportCode_cbb.SelectedValue = exportCodeId;

            DataRow[] packingRows = mProductPacking_dt.Select($"ProductPackingID = {packingID}");
            if(packingRows.Length > 0) 
                product_ccb.SelectedValue = packingRows[0]["SKU"].ToString();

            PCSOther_tb.Text = PCSOther;
            netWeight_tb.Text = NWOther;
            priceCNF_tb.Text = orderPackingPriceCNF;

            product_ccb_SelectedIndexChanged(null, null);

            netWeight_tb.Text = calNetWeight().ToString();

            delete_btn.Enabled = true;
            info_gb.BackColor = Color.DarkGray;
        }

        private decimal calNetWeight() {
            if (packing_ccb.SelectedItem == null || PCSOther_tb.Text.CompareTo("") == 0)
                return 0;

            
            
            
            DataRowView pakingData = (DataRowView)packing_ccb.SelectedItem;

            
            int PCSOther = Convert.ToInt32(PCSOther_tb.Text);
            string packing = pakingData["Packing"].ToString();
            int? amount = pakingData["Amount"] == DBNull.Value
                        ? (int?)null
                        : Convert.ToInt32(pakingData["Amount"]);

            if (amount == null)
            {
                return 0;
            }
            else
            {
                return Utils.calNetWeight(PCSOther, (int)amount, packing);
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
                                            "Tạo Mới Thông Tin Khách Hàng",
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
                                status_lb.ForeColor = Color.Green;

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

        private async void createOrder(int customerId, int exportCodeId, int packingId, int PCSOther, decimal NWOther, decimal priceCNF)
        {
            DialogResult dialogResult = MessageBox.Show(
                                            "Chắc chắn chưa?",
                                            "Tạo Mới Thông Tin Khách Hàng",
                                            MessageBoxButtons.YesNo,
                                            MessageBoxIcon.Question // Icon dấu chấm hỏi
                                        );


            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    bool isScussess = await SQLManager.Instance.insertOrderAsync(customerId, exportCodeId, packingId, PCSOther, NWOther, priceCNF);
                    if (isScussess == true)
                    {
                        DataRow drToAdd = mOrders_dt.NewRow();

                        int newID = createID();
                        drToAdd["OrderId"] = newID;
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

                        mOrders_dt.Rows.Add(drToAdd);
                        mOrders_dt.AcceptChanges();

                        status_lb.Text = "Thành công";
                        status_lb.ForeColor = Color.Green;
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
        private void saveBtn_Click(object sender, EventArgs e)
        {
            if (customer_ccb.SelectedValue == null
                || packing_ccb.SelectedValue == null
                || string.IsNullOrWhiteSpace(PCSOther_tb.Text)
                || string.IsNullOrWhiteSpace(netWeight_tb.Text)
                || string.IsNullOrWhiteSpace(priceCNF_tb.Text))
            {
                // Nếu bất kỳ giá trị nào rỗng, dừng
                MessageBox.Show(
                                "Thiếu Dữ Liệu, Kiểm Tra Lại!",
                                "Thông Báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning
                            );
                return;
            }
            int exportCodeId = Convert.ToInt32(exportCode_cbb.SelectedValue);
            int customerId = Convert.ToInt32(customer_ccb.SelectedValue);
            int packingId = Convert.ToInt32(packing_ccb.SelectedValue);
            int PCSOther = Convert.ToInt32(PCSOther_tb.Text);
            decimal NWOther = Convert.ToDecimal(netWeight_tb.Text);
            decimal priceCNF = Convert.ToDecimal(priceCNF_tb.Text);

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
                            bool isSuccess = await Task.Run(() => SQLManager.Instance.deleteCustomerAsync(id));

                            if (isSuccess)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                // Xóa row khỏi DataTable
                                mOrders_dt.Rows.Remove(row);
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
                    break; // Dừng vòng lặp sau khi xóa
                }
            }
            //dataGV.DataSource = mOrders_dt;

            delete_btn.Enabled = false;
        }

        private void Tb_KeyPress_OnlyNumber(object sender, KeyPressEventArgs e)
        {
            // Chỉ cho nhập số hoặc phím điều khiển (Backspace, Delete, Enter…)
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // chặn ký tự không hợp lệ
            }
        }

        private void newCustomerBtn_Click(object sender, EventArgs e)
        {
            orderId_tb.Text = "";
            PCSOther_tb.Text = "";
            netWeight_tb.Text= "";
            info_gb.BackColor = Color.Green;

            delete_btn.Enabled = false;
        }

        private void PCSOrder_tb_TextChanged(object sender, EventArgs e)
        {
            netWeight_tb.Text = calNetWeight().ToString();
        }

        private void printOtherList_btn_Click(object sender, EventArgs err)
        {
            if (dataGV.Rows.Count == 0)
                return;

            PrintDataGridView(dataGV);
        }
                
        private void PrintDataGridView(DataGridView dataGV)
        {
            string staff = Utils.InputDialog("Nhập tên người phụ trách đóng gói:", "Nhập nhân viên", "");

            if (string.IsNullOrWhiteSpace(staff))
                return;

            OrderListPrinter printer = new OrderListPrinter();
            printer.PrintDirect(dataGV, exportCode_search_cbb.Text, DateTime.Now, staff);
        }

        private async void printPendingOrderSummary_btn_Click(object sender, EventArgs e)
        {
            if (exportCode_search_cbb.SelectedItem == null) return;

            DataRowView pakingData = (DataRowView)exportCode_search_cbb.SelectedItem;
            string exportCode = pakingData["ExportCode"].ToString();
            DateTime exportDate = Convert.ToDateTime(pakingData["ExportDate"]);

            DataTable pendingOrderSummary = await SQLManager.Instance.getPendingOrderSummary(Convert.ToInt32(pakingData["ExportCodeID"]));

            OrderSummaryPrinter printer = new OrderSummaryPrinter();

            printer.PrintDirect(pendingOrderSummary, exportCode, exportDate);
        }
    }
}
