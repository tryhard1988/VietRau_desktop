using ClosedXML.Excel;
using RauViet.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO.Packaging;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Color = System.Drawing.Color;

namespace RauViet.ui
{
    public partial class INVOICE : Form
    {
        DataTable mExportCode_dt, mOrdersTotal_dt, mCustomerOrdersTotal_dt, mCartonOrdersTotal_dt, mProductSKU_dt;
        private LoadingOverlay loadingOverlay;
        int mCurrentExportID = -1;
        public INVOICE()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = true;

            status_lb.Text = "";

            customerHomeBtn.Click += CustomerHomeBtn_Click;
            LuuThayDoiBtn.Click += saveBtn_Click;
            this.KeyDown += INVOICE_KeyDown;
        }

        private void INVOICE_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                if (mCurrentExportID <= 0)
                {
                    return;
                }
                SQLStore_Kho.Instance.removeOrdersCartonInvoice(mCurrentExportID);
                SQLStore_Kho.Instance.removeOrdersCusInvoice(mCurrentExportID);
                SQLStore_Kho.Instance.removeOrdersInvoice(mCurrentExportID);
                ShowData();
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
                string[] keepColumns = { "ExportCodeID", "ExportCode", "ExportDate", "ExchangeRate", "ShippingCost", "ExportCodeIndex" };
                var parameters = new Dictionary<string, object> { { "Complete", false } };
                mExportCode_dt = await SQLStore_Kho.Instance.getExportCodesAsync(keepColumns, parameters);                  

                if (mCurrentExportID <= 0 && mExportCode_dt.Rows.Count > 0)
                {
                    mCurrentExportID = Convert.ToInt32(mExportCode_dt.AsEnumerable()
                                   .Max(r => r.Field<int>("ExportCodeID")));
                }

                var cartonOrdersTask = SQLStore_Kho.Instance.getOrdersCartonInvoiceAsync(mCurrentExportID);
                var customersOrdersTask = SQLStore_Kho.Instance.getOrdersCusInvoiceAsync(mCurrentExportID);
                var OrdersInvoiceTask = SQLStore_Kho.Instance.getOrdersInvoiceAsync(mCurrentExportID);
                var ProductSKUTask = SQLManager_Kho.Instance.getProductSKUAsync();

                await Task.WhenAll(customersOrdersTask, OrdersInvoiceTask, cartonOrdersTask, ProductSKUTask);
                mOrdersTotal_dt = OrdersInvoiceTask.Result;
                mCustomerOrdersTotal_dt = customersOrdersTask.Result;
                mCartonOrdersTotal_dt = cartonOrdersTask.Result;
                mProductSKU_dt = ProductSKUTask.Result;

                showOrdersExport();
                showCustomerOrderGV();
                showCartonOrderGV();

                exportCode_cbb.SelectedIndexChanged -= exportCode_search_cbb_SelectedIndexChanged;
                exportCode_cbb.DataSource = mExportCode_dt;
                exportCode_cbb.DisplayMember = "ExportCode";  // hiển thị tên
                exportCode_cbb.ValueMember = "ExportCodeID";
                exportCode_cbb.SelectedValue = mCurrentExportID;
                exportCode_cbb.SelectedIndexChanged += exportCode_search_cbb_SelectedIndexChanged;

                UpdateBotttomUI();
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

        private void UpdateBotttomUI()
        {
            var sumNWObj = mCustomerOrdersTotal_dt.Compute("SUM(NWReal)", "");
            decimal sumNWOrder = sumNWObj == DBNull.Value ? 0 : Convert.ToDecimal(sumNWObj);

            var sumCNTSObj = mCustomerOrdersTotal_dt.Compute("SUM(CNTS)", "");
            int sumCNTS = sumCNTSObj == DBNull.Value ? 0 : Convert.ToInt32(sumCNTSObj);

            var sumPCSObj = mOrdersTotal_dt.Compute("SUM(PCSReal)", "");
            int sumPCS = sumPCSObj == DBNull.Value ? 0 : Convert.ToInt32(sumPCSObj);

            var sumAmountCHFObj = mCustomerOrdersTotal_dt.Compute("SUM(AmountCHF)", "");
            decimal sumAmountCHF = sumAmountCHFObj == DBNull.Value ? 0 : Convert.ToDecimal(sumAmountCHFObj);

            NWTotal_label.Text = sumNWOrder.ToString("N2");
            TotalAmount_label.Text = sumAmountCHF.ToString("N2");
            CNTSTotal_label.Text = sumCNTS.ToString("N0");
            PCSTotal_label.Text = sumPCS.ToString("N0");

        }

        private void showOrdersExport()
        {
            DataView dv = new DataView(mOrdersTotal_dt);
            dataGV.DataSource = dv;
            Utils.HideColumns(dataGV, new[] { "ExportCodeID" });

            Utils.SetGridHeaders(dataGV, new System.Collections.Generic.Dictionary<string, string> {
                    {"No", "No" },
                    {"ProductNameEN", "English Name" },
                    {"ProductNameVN", "Vietnamese Name" },
                    {"Package", "Unit" },
                    {"NWReal", "N.W(kg)" },
                    {"PCSReal", "PCS" },
                    {"OrderPackingPriceCNF", "Price (CHF)" },
                    {"AmountCHF", "Amount (CHF)" },
                    {"Priority", "Ưu\nTiên" }
                });

            Utils.SetGridWidths(dataGV, new System.Collections.Generic.Dictionary<string, int> {
                    {"Priority", 50},
                    {"No", 50},
                    {"PLU", 50},
                    {"ProductNameEN", 130},
                    {"ProductNameVN", 160},
                    {"Package", 50},
                    {"Quantity", 60},
                    {"NWReal", 50},
                    {"Packing", 50},
                    {"PCSReal", 50},
                    {"OrderPackingPriceCNF", 50},
                    {"AmountCHF", 70}
                });

            dataGV.Columns["AmountCHF"].DefaultCellStyle.Format = "N3";
            dataGV.Columns["Quantity"].DefaultCellStyle.Format = "N3";

            dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            CheckWarning();
        }

        private void showCustomerOrderGV()
        {
            DataView dv = new DataView(mCustomerOrdersTotal_dt);
            cusOrderGV.DataSource = dv;
            Utils.HideColumns(cusOrderGV, new[] { "Home", "ExportCodeID" });

            Utils.SetGridHeaders(dataGV, new System.Collections.Generic.Dictionary<string, string> {
                    {"FullName", "MARK" },
                    {"NWReal", "N.W" },
                    {"AmountCHF", "Amount\nCHF" }
                });

            Utils.SetGridWidths(cusOrderGV, new System.Collections.Generic.Dictionary<string, int> {
                    {"No", 30},
                    {"CNTS", 40},
                    {"NWReal", 70},
                    {"AmountCHF", 70},
                    {"FullName", 100}
                });

            cusOrderGV.Columns["AmountCHF"].DefaultCellStyle.Format = "N3";
            
        }
        private void CheckWarning()
        {
          //  await Task.Delay(500);

            var priceLookup = mProductSKU_dt.AsEnumerable()
                .Where(r => r["SKU"] != DBNull.Value)
                .ToDictionary(
                    r => r.Field<int>("SKU"),
                    r => r.Field<decimal>("PriceCNF")
                );

            foreach (DataGridViewRow row in dataGV.Rows)
            {
                if (row.IsNewRow) continue;

                int sku = 0;
                if (!int.TryParse(row.Cells["SKU"].Value?.ToString(), out sku))
                {
                    // SKU không hợp lệ → bỏ qua dòng này
                    continue;
                }

                decimal orderPrice = 0;
                if (row.Cells["OrderPackingPriceCNF"].Value != DBNull.Value)
                    decimal.TryParse(row.Cells["OrderPackingPriceCNF"].Value.ToString(), out orderPrice);

                // Nếu SKU tồn tại trong ProductSKU
                if (priceLookup.ContainsKey(sku))
                {
                    decimal priceCNF = priceLookup[sku];

                    // Điều kiện cảnh báo
                    if (orderPrice <= 0)
                    {
                        row.DefaultCellStyle.BackColor = Color.Red;
                    }
                    else if (orderPrice != priceCNF)
                    {
                        row.DefaultCellStyle.BackColor = Color.Khaki;
                    }
                    else
                    {
                        // reset màu nếu hợp lệ
                        row.DefaultCellStyle.BackColor = Color.White;
                        row.DefaultCellStyle.ForeColor = Color.Black;
                    }
                }
            }
        }

        private void showCartonOrderGV()
        {
            DataView dv = new DataView(mCartonOrdersTotal_dt);
            cartonSizeGV.DataSource = dv;
            Utils.HideColumns(cartonSizeGV, new[] { "ExportCodeID" });

            Utils.SetGridHeaders(dataGV, new System.Collections.Generic.Dictionary<string, string> {
                    {"CartonSize", "Carton Size" },
                    {"CountCarton", "Quantity" }
                });


            Utils.SetGridWidths(cartonSizeGV, new System.Collections.Generic.Dictionary<string, int> {
                    {"No", 30},
                    {"Weight", 60},
                    {"CountCarton", 50},
                    {"CartonSize", 120}
                });
        }

        private async void exportCode_search_cbb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!int.TryParse(exportCode_cbb.SelectedValue.ToString(), out int exportCodeId))
                return;

            mCurrentExportID = exportCodeId;

            var cartonOrdersTask = SQLStore_Kho.Instance.getOrdersCartonInvoiceAsync(mCurrentExportID);
            var customersOrdersTask = SQLStore_Kho.Instance.getOrdersCusInvoiceAsync(mCurrentExportID);
            var OrdersInvoiceTask = SQLStore_Kho.Instance.getOrdersInvoiceAsync(mCurrentExportID);

            await Task.WhenAll(customersOrdersTask, OrdersInvoiceTask, cartonOrdersTask);
            mOrdersTotal_dt = OrdersInvoiceTask.Result;
            mCustomerOrdersTotal_dt = customersOrdersTask.Result;
            mCartonOrdersTotal_dt = cartonOrdersTask.Result;

            // Tạo DataView để filter
            DataView dv = new DataView(mOrdersTotal_dt);
            dataGV.DataSource = dv;

            DataView dvCus = new DataView(mCustomerOrdersTotal_dt);
            cusOrderGV.DataSource = dvCus;

            DataView dvCarton = new DataView(mCartonOrdersTotal_dt);
            cartonSizeGV.DataSource = dvCarton;

            UpdateBotttomUI();
            CheckWarning();
        }      
        
        private void saveBtn_Click(object sender, EventArgs e)
        {
            ExportDataGVToExcel(dataGV);
        }

        private async void ExportDataGVToExcel(DataGridView dgv)
        {
            if (exportCode_cbb.SelectedItem == null) return;

            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Message = "Đang xử lý ...";
            loadingOverlay.Show();
            await Task.Delay(100);

            string exportCode = ((DataRowView)exportCode_cbb.SelectedItem)["ExportCode"].ToString();
            int exportCodeIndex = Convert.ToInt32(((DataRowView)exportCode_cbb.SelectedItem)["ExportCodeIndex"]);
            DateTime exportDate = Convert.ToDateTime(((DataRowView)exportCode_cbb.SelectedItem)["ExportDate"]);
            decimal exRate = Convert.ToDecimal(((DataRowView)exportCode_cbb.SelectedItem)["ExchangeRate"]);
            decimal ShippingCost = Convert.ToDecimal(((DataRowView)exportCode_cbb.SelectedItem)["ShippingCost"]);
            try
            {
                using (var wb = new XLWorkbook())
                {
                    var ws = wb.Worksheets.Add("Data");
                    ws.Style.Font.FontName = "Arial";
                    ws.Style.Font.FontSize = 9;
                    ws.RowHeight = 16;
                    ws.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    // Danh sách cột muốn xuất theo Name
                    string[] columnsToExport = new string[] { "No", "PLU", "ProductNameEN", "ProductNameVN", "Package", "Quantity", "NWReal", "Packing", "PCSReal", "OrderPackingPriceCNF", "AmountCHF" };

                    // Lọc cột hiển thị và có trong danh sách
                    var exportColumns = dgv.Columns.Cast<DataGridViewColumn>()
                                            .Where(c => c.Visible && columnsToExport.Contains(c.Name))
                                            .OrderBy(c => Array.IndexOf(columnsToExport, c.Name))
                                            .ToList();

                    

                    // Hàng 1: Tên công ty
                    ws.Range(1, 1, 1, exportColumns.Count).Merge();
                    ws.Cell(1, 1).Value = "VIET RAU JOINT STOCK COMPANY";
                    ws.Cell(1, 1).Style.Font.Bold = true;
                    ws.Cell(1, 1).Style.Font.FontSize = 14;
                    ws.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    // Hàng 2: Địa chỉ
                    ws.Range(2, 1, 2, exportColumns.Count).Merge();
                    ws.Cell(2, 1).Value = Utils.getCompanyAddress_EN();
                    ws.Cell(2, 1).Style.Font.FontSize = 10;
                    ws.Cell(2, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    // Hàng 3: Tel/Fax và Email
                    ws.Range(3, 1, 3, exportColumns.Count / 2).Merge();
                    ws.Cell(3, 1).Value = "Tel/Fax: +84 251 2860828";
                    ws.Cell(3, 1).Style.Font.FontSize = 10;
                    ws.Cell(3, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    ws.Range(3, (exportColumns.Count / 2) + 1, 3, exportColumns.Count).Merge();
                    ws.Cell(3, (exportColumns.Count / 2) + 1).Value = "Email: acc@vietrau.com";
                    ws.Cell(3, (exportColumns.Count / 2) + 1).Style.Font.FontSize = 10;
                    ws.Cell(3, (exportColumns.Count / 2) + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    // Hàng 4: INVOICE
                    ws.Range(4, 1, 4, exportColumns.Count).Merge();
                    ws.Cell(4, 1).Value = "INVOICE";
                    ws.Cell(4, 1).Style.Font.Bold = true;
                    ws.Cell(4, 1).Style.Font.FontSize = 18;
                    ws.Cell(4, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    // Hàng 5-6: Shipper
                    ws.Range(5, 1, 6, 4).Merge();
                    ws.Cell(5, 1).Value = $"Shipper:\r\nVIET RAU JOINT STOCK COMPANY\r\n{Utils.getCompanyAddress_EN()}\r\nTel/Fax: {Utils.get_Tele_Fax()}\r\nEmail: acc@vietrau.com";
                    ws.Cell(5, 1).Style.Font.FontSize = 10;
                    ws.Cell(5, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell(5, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;

                    // Thêm border
                    ws.Range(5, 1, 6, 3).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    // Hàng 5: Invoice No
                    ws.Range(5, 5, 5, exportColumns.Count).Merge();
                    ws.Cell(5, 5).Value = "Invoice No:  " + exportDate.Day + exportDate.Month + exportDate.Year + "SQ";
                    ws.Cell(5, 5).Style.Font.FontSize = 10;
                    ws.Cell(5, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Range(5, 5, 5, exportColumns.Count).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    // Hàng 6: Date
                    ws.Range(6, 5, 6, 7).Merge();
                    ws.Cell(6, 5).Value = "Date:    " + exportDate.ToString("dd/MM/yyyy");
                    ws.Cell(6, 5).Style.Font.FontSize = 10;
                    ws.Cell(6, 5).Style.Font.Bold = true;
                    ws.Cell(6, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Range(6, 5, 6, 7).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    ws.Range(6, 8, 6, exportColumns.Count).Merge();
                    ws.Cell(6, 8).Value = "Reference No:    " + exportCode;
                    ws.Cell(6, 8).Style.Font.FontSize = 10;
                    ws.Cell(6, 8).Style.Font.Bold = true;
                    ws.Cell(6, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Range(6, 8, 6, exportColumns.Count).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    // Hàng 7: Consignee & TOTAL
                    ws.Range(7, 1, 7, 4).Merge();
                    ws.Cell(7, 1).Value = "Consignee:";
                    ws.Cell(7, 1).Style.Font.FontSize = 10;
                    ws.Cell(7, 1).Style.Font.Bold = true;
                    ws.Cell(7, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Range(7, 1, 9, 4).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    ws.Range(7, 5, 7, exportColumns.Count).Merge();
                    ws.Cell(7, 5).Value = "TOTAL";
                    ws.Cell(7, 5).Style.Font.FontSize = 10;
                    ws.Cell(7, 5).Style.Font.Bold = true;
                    ws.Cell(7, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Range(7, 5, 7, exportColumns.Count).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    // Hàng 8-9: Consignee details & Freight
                    ws.Range(8, 1, 8, 4).Merge();
                    ws.Cell(8, 1).Value = "Asiaway AG";
                    ws.Cell(8, 1).Style.Font.FontSize = 10;
                    ws.Cell(8, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell(8, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;

                    ws.Range(9, 1, 9, 4).Merge();
                    ws.Cell(9, 1).Value = "Schwamendingenstrasse 10, 8050 Zürich,\r\nSwitzerland";
                    ws.Cell(9, 1).Style.Font.FontSize = 10;
                    ws.Cell(9, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    ws.Range(8, 5, 8, 6).Merge();
                    ws.Cell(8, 5).Value = "Air freight:";
                    ws.Cell(8, 5).Style.Font.FontSize = 10;
                    ws.Cell(8, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Range(8, 5, 8, 6).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    ws.Range(8, 7, 8, exportColumns.Count).Merge();
                    ws.Cell(8, 7).Value = "FREIGHT PREPAID";
                    ws.Cell(8, 7).Style.Font.FontSize = 10;
                    ws.Cell(8, 7).Style.Font.Bold = true;
                    ws.Cell(8, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Range(8, 7,8, exportColumns.Count).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    ws.Range(9, 5, 9, 6).Merge();
                    ws.Cell(9, 5).Value = "Delivery terms:";
                    ws.Cell(9, 5).Style.Font.FontSize = 10;
                    ws.Cell(9, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Range(9, 5, 9, 6).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    ws.Range(9, 7, 9, exportColumns.Count).Merge();
                    ws.Cell(9, 7).Value = "CNF";
                    ws.Cell(9, 7).Style.Font.FontSize = 10;
                    ws.Cell(9, 7).Style.Font.Bold = true;
                    ws.Cell(9, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Range(9, 7, 9, exportColumns.Count).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    ws.Row(10).Height = 2;

                    int headerStartRow = 11; // Header cấp 2 và 3 bắt đầu từ hàng 5

                    // ===== Title công ty =====
                    // Xác định vùng khung: từ hàng 1 đến 4, từ cột 1 đến exportColumns.Count
                    var headerRange = ws.Range(1, 1, 4, exportColumns.Count);

                    // Chỉ tạo viền ngoài, không có viền trong
                    headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    headerRange.Style.Border.OutsideBorderColor = XLColor.Black;
                    // ===== Header cấp 2 và cấp 3 =====
                    int colIndex = 1;
                    foreach (var col in exportColumns)
                    {
                        if (col.Name == "ProductNameEN" || col.Name == "ProductNameVN")
                        {
                            // Gộp 3 cột con cho "Name of Goods" ở hàng 5
                            if (colIndex == exportColumns.FindIndex(c => c.Name == "ProductNameEN") + 1)
                            {
                                var range = ws.Range(headerStartRow, colIndex, headerStartRow, colIndex + 1);
                                range.Merge();
                                ws.Cell(headerStartRow, colIndex).Value = "Name of Goods";
                                ws.Cell(headerStartRow, colIndex).Style.Font.Bold = true;
                                ws.Cell(headerStartRow, colIndex).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Đóng khung
                            }

                            // Header con ở hàng 6
                            var cell = ws.Cell(headerStartRow + 1, colIndex);
                            cell.Value = col.HeaderText;
                            cell.Style.Font.Bold = true;
                            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Đóng khung

                            colIndex++;
                        }
                        else if (col.Name == "OrderPackingPriceCNF" || col.Name == "AmountCHF")
                        {
                            // Gộp 2 cột con cho "Unit Price" ở hàng 5
                            if (colIndex == exportColumns.FindIndex(c => c.Name == "OrderPackingPriceCNF") + 1)
                            {
                                var range = ws.Range(headerStartRow, colIndex, headerStartRow, colIndex + 1);
                                range.Merge();
                                ws.Cell(headerStartRow, colIndex).Value = "Unit Price";
                                ws.Cell(headerStartRow, colIndex).Style.Font.Bold = true;
                                ws.Cell(headerStartRow, colIndex).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Đóng khung
                            }

                            // Header con ở hàng 6
                            var cell = ws.Cell(headerStartRow + 1, colIndex);
                            cell.Value = col.HeaderText;
                            cell.Style.Font.Bold = true;
                            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Đóng khung

                            colIndex++;
                        }
                        else
                        {
                            // Các cột khác gộp 2 hàng (5 và 6)
                            var range = ws.Range(headerStartRow, colIndex, headerStartRow + 1, colIndex);
                            range.Merge();
                            ws.Cell(headerStartRow, colIndex).Value = col.HeaderText;
                            ws.Cell(headerStartRow, colIndex).Style.Font.Bold = true;
                            ws.Cell(headerStartRow, colIndex).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Đóng khung

                            colIndex++;
                        }
                    }


                    // ===== Data bắt đầu từ hàng 7 =====
                    decimal totalNWReal = 0;
                    decimal totalAmount = 0;

                    int rowIndex = headerStartRow + 2;
                    foreach (DataGridViewRow row in dgv.Rows)
                    {
                        if (row.IsNewRow) continue;
                                                
                        colIndex = 1;
                        foreach (var col in exportColumns)
                        {
                            var cell = ws.Cell(rowIndex, colIndex);
                            var cellValue = row.Cells[col.Index].Value?.ToString() ?? "";
                            cell.Value = cellValue;

                            cell.Style.Alignment.WrapText = true;

                            if (col.Name == "Package" && cellValue.CompareTo("weight") == 0)
                            {
                                cell.Value = "kg";
                            }
                            // Căn phải các cột số
                            if (col.ValueType == typeof(int) || col.ValueType == typeof(decimal))
                                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                            else
                                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                            if (col.Name == "PCSReal" || col.Name == "Package" || col.Name == "Packing" || col.Name == "No" || col.Name == "PLU")
                                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            else if (col.Name == "AmountCHF")
                            {
                                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                            }

                            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                            // Tính tổng cột NWOther
                            if (col.Name == "NWReal")
                            {
                                if (decimal.TryParse(cellValue, out decimal num))
                                {
                                    cell.Value = num;
                                    cell.Style.NumberFormat.Format = "0.00";
                                    totalNWReal += num;
                                }
                            }

                            if (col.Name == "AmountCHF")
                            {
                                if (decimal.TryParse(cellValue, out decimal num))
                                {
                                    cell.Value = num;
                                    cell.Style.NumberFormat.Format = "0.00";
                                    totalAmount += num;

                                    if (num <= 0)
                                    {
                                        ws.Range(rowIndex, 1, rowIndex, colIndex).Style.Fill.BackgroundColor = XLColor.Red;
                                        ws.Range(rowIndex, 1, rowIndex, colIndex).Style.Font.FontColor = XLColor.Black;
                                    }
                                }
                            }

                            if (col.Name == "OrderPackingPriceCNF" || col.Name == "Quantity")
                            {
                                if (decimal.TryParse(cellValue, out decimal num))
                                {
                                    cell.Value = num;
                                    cell.Style.NumberFormat.Format = "0.00";
                                }
                            }

                            if (col.Name == "PCSReal")
                            {
                                if (int.TryParse(cellValue, out int num))
                                {
                                    cell.Value = num;
                                }
                            }


                            colIndex++;
                        }
                        rowIndex++;
                    }

                    decimal totalCartonWeight = 0;

                    foreach (DataGridViewRow row in cartonSizeGV.Rows)
                    {
                        if (row.IsNewRow) continue;

                        var cellValue = row.Cells["Weight"].Value?.ToString() ?? "";
                        if (decimal.TryParse(cellValue, out decimal num))
                            totalCartonWeight += num;
                    }

                    decimal totalFreightCharge = totalCartonWeight * exRate * ShippingCost;
                    int totalRow = rowIndex;
                    int nwOtherColIndex = exportColumns.FindIndex(c => c.Name == "NWReal") + 1;
                    int nwAmountColIndex = exportColumns.FindIndex(c => c.Name == "AmountCHF") + 1;

                    ws.Range(totalRow, 1, totalRow, 3).Merge();
                    ws.Cell(totalRow, 1).Value = "TOTAL";
                    ws.Cell(totalRow, 1).Style.Font.Bold = true;
                    ws.Cell(totalRow, 1).Style.Font.FontSize = 10;
                    ws.Cell(totalRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Range(totalRow, 1, totalRow, 3).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    ws.Cell(totalRow, nwOtherColIndex).Value = totalNWReal;
                    ws.Cell(totalRow, nwOtherColIndex).Style.NumberFormat.Format = "#,##0.00";
                    ws.Cell(totalRow, nwOtherColIndex).Style.Font.Bold = true;
                    ws.Cell(totalRow, nwOtherColIndex).Style.Font.FontSize = 10;
                    ws.Cell(totalRow, nwOtherColIndex).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell(totalRow, nwOtherColIndex).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    ws.Cell(totalRow, nwAmountColIndex).Value = totalAmount;
                    ws.Cell(totalRow, nwAmountColIndex).Style.NumberFormat.Format = "#,##0.00";
                    ws.Cell(totalRow, nwAmountColIndex).Style.Font.Bold = true;
                    ws.Cell(totalRow, nwAmountColIndex).Style.Font.FontSize = 10;
                    ws.Cell(totalRow, nwAmountColIndex).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell(totalRow, nwAmountColIndex).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    totalRow += 1;
                    ws.Range(totalRow, 1, totalRow, 3).Merge();
                    ws.Cell(totalRow, 1).Value = "Total Amount (FOB HCM):";
                    ws.Cell(totalRow, 1).Style.Font.Bold = true;
                    ws.Cell(totalRow, 1).Style.Font.FontSize = 10;
                    ws.Cell(totalRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Range(totalRow, 1, totalRow, 3).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    ws.Range(totalRow, 4, totalRow, exportColumns.Count).Merge();
                    ws.Cell(totalRow, 4).Value = totalAmount - totalFreightCharge;
                    ws.Cell(totalRow, 4).Style.NumberFormat.Format = "#,##0.00 \"CHF\"";
                    ws.Cell(totalRow, 4).Style.Font.Bold = true;
                    ws.Cell(totalRow, 4).Style.Font.FontSize = 10;
                    ws.Cell(totalRow, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Range(totalRow, 4, totalRow, exportColumns.Count).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    totalRow += 1;
                    ws.Range(totalRow, 1, totalRow, 3).Merge();
                    ws.Cell(totalRow, 1).Value = "Freight Charge:";
                    ws.Cell(totalRow, 1).Style.Font.Bold = true;
                    ws.Cell(totalRow, 1).Style.Font.FontSize = 10;
                    ws.Cell(totalRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Range(totalRow, 1, totalRow, 3).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    ws.Range(totalRow, 4, totalRow, exportColumns.Count).Merge();
                    ws.Cell(totalRow, 4).Style.NumberFormat.Format = "#,##0.00 \"CHF\"";
                    ws.Cell(totalRow, 4).Value = totalFreightCharge;
                    ws.Cell(totalRow, 4).Style.Font.Bold = true;
                    ws.Cell(totalRow, 4).Style.Font.FontSize = 10;
                    ws.Cell(totalRow, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Range(totalRow, 4, totalRow, exportColumns.Count).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    totalRow += 1;
                    ws.Range(totalRow, 1, totalRow, 3).Merge();
                    ws.Cell(totalRow, 1).Value = "Total Amount (C&F):";
                    ws.Cell(totalRow, 1).Style.Font.Bold = true;
                    ws.Cell(totalRow, 1).Style.Font.FontSize = 10;
                    ws.Cell(totalRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Range(totalRow, 1, totalRow, 3).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    ws.Range(totalRow, 4, totalRow, exportColumns.Count).Merge();
                    ws.Cell(totalRow, 4).Style.NumberFormat.Format = "#,##0.00 \"CHF\"";
                    ws.Cell(totalRow, 4).Value = totalAmount;
                    ws.Cell(totalRow, 4).Style.Font.Bold = true;
                    ws.Cell(totalRow, 4).Style.Font.FontSize = 10;
                    ws.Cell(totalRow, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Range(totalRow, 4, totalRow, exportColumns.Count).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    totalRow += 1;
                    ws.Range(totalRow, 1, totalRow, 3).Merge();
                    ws.Cell(totalRow, 1).Value = "Total NW:";
                    ws.Cell(totalRow, 1).Style.Font.Bold = true;
                    ws.Cell(totalRow, 1).Style.Font.FontSize = 10;
                    ws.Cell(totalRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Range(totalRow, 1, totalRow, 3).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    ws.Range(totalRow, 4, totalRow, exportColumns.Count).Merge();
                    ws.Cell(totalRow, 4).Style.NumberFormat.Format = "#,##0.00 \"kg\"";
                    ws.Cell(totalRow, 4).Value = totalNWReal;
                    ws.Cell(totalRow, 4).Style.Font.Bold = true;
                    ws.Cell(totalRow, 4).Style.Font.FontSize = 10;
                    ws.Cell(totalRow, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Range(totalRow, 4, totalRow, exportColumns.Count).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    totalRow += 1;
                    ws.Range(totalRow, 1, totalRow, 3).Merge();
                    ws.Cell(totalRow, 1).Value = "Total Carton:";
                    ws.Cell(totalRow, 1).Style.Font.Bold = true;
                    ws.Cell(totalRow, 1).Style.Font.FontSize = 10;
                    ws.Cell(totalRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Range(totalRow, 1, totalRow, 3).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    ws.Range(totalRow, 4, totalRow, exportColumns.Count).Merge();
                    ws.Cell(totalRow, 4).Value = " chờ tính";
                    ws.Cell(totalRow, 4).Style.Font.Bold = true;
                    ws.Cell(totalRow, 4).Style.Font.FontSize = 10;
                    ws.Cell(totalRow, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Range(totalRow, 4, totalRow, exportColumns.Count).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    ws.Columns().AdjustToContents();
                    ws.Column(1).Width = 3;
                    ws.Column(2).Width = 7;
                    ws.Column(3).Width = 15;
                    ws.Column(4).Width = 15;
                    ws.Column(5).Width = 8;
                    ws.Column(6).Width = 7;
                    ws.Column(7).Width = 7;
                    ws.Column(8).Width = 7;
                    ws.Column(9).Width = 5;
                    ws.Column(10).Width = 9.3;
                    ws.Column(11).Width = 11.3;


                    ws.Row(5).Height = 21;
                    ws.Row(6).Height = 60;
                    ws.Row(7).Height = 13;
                    ws.Row(8).Height = 17;
                    ws.Row(9).Height = 26;

                    ws.Row(11).Height += 5;
                    ws.Row(12).Height += 5;

                    foreach (var col in exportColumns)
                    {
                        var column = ws.Column(col.Index + 1);
                        column.Style.Alignment.WrapText = true;
                    }

                    string[] columns1ToExport = new string[] { "No", "FullName", "AmountCHF", "NWReal", "CNTS"};

                    // Lọc cột hiển thị và có trong danh sách
                    var export1Columns = cusOrderGV.Columns.Cast<DataGridViewColumn>()
                                            .Where(c => c.Visible && columns1ToExport.Contains(c.Name))
                                            .OrderBy(c => Array.IndexOf(columns1ToExport, c.Name))
                                            .ToList();

                    colIndex = 2;

                    headerStartRow = totalRow + 2;
                    foreach (var col in export1Columns)
                    {
                        var range = ws.Range(headerStartRow, colIndex, headerStartRow + 1, colIndex);
                        range.Merge();
                        ws.Cell(headerStartRow, colIndex).Value = col.HeaderText;
                        ws.Cell(headerStartRow, colIndex).Style.Font.Bold = true;
                        ws.Cell(headerStartRow, colIndex).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Đóng khung

                        colIndex++;
                    }

                    decimal totalCarton1 = 0;
                    decimal totalNW1 = 0;
                    decimal totalAmount1 = 0;

                    rowIndex = headerStartRow + 2;
                    foreach (DataGridViewRow row in cusOrderGV.Rows)
                    {
                        if (row.IsNewRow) continue;

                        colIndex = 2;
                        foreach (var col in export1Columns)
                        {
                            var cell = ws.Cell(rowIndex, colIndex);
                            var cellValue = row.Cells[col.Index].Value?.ToString() ?? "";
                            cell.Value = cellValue;

                            cell.Style.Alignment.WrapText = true;

                            // Căn phải các cột số
                            if (col.ValueType == typeof(int) || col.ValueType == typeof(decimal))
                                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                            else
                                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                            if (col.Name == "CNTS")
                            {
                                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                                cell.Style.NumberFormat.Format = "#,##0 \"CTN\"";

                                if (decimal.TryParse(cellValue, out decimal num))
                                {
                                    cell.Value = num;
                                    totalCarton1 += num;
                                }
                            }
                            else if (col.Name == "NWReal")
                            {
                                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                                cell.Style.NumberFormat.Format = "#,##0.00 \"kg\"";

                                if (decimal.TryParse(cellValue, out decimal num))
                                {
                                    cell.Value = num;
                                    totalNW1 += num;
                                }
                            }
                            else if (col.Name == "AmountCHF")
                            {
                                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                                cell.Style.NumberFormat.Format = "#,##0.00";
                                if (decimal.TryParse(cellValue, out decimal num))
                                {
                                    cell.Value = num;
                                    totalAmount1 += num;
                                    if(num <= 0)
                                    {
                                        cell.Style.Fill.BackgroundColor = XLColor.Red;
                                        cell.Style.Font.FontColor = XLColor.Black;
                                    }
                                }
                            }
                            else if (col.Name == "No")
                                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                            // Thêm border
                            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                            colIndex++;
                        }
                        rowIndex++;
                    }

                    int totalRow1 = rowIndex;
                    int nwColIndex = export1Columns.FindIndex(c => c.Name == "NWReal") + 2;
                    int amountColIndex = export1Columns.FindIndex(c => c.Name == "AmountCHF") + 2;
                    int CTNSColIndex = export1Columns.FindIndex(c => c.Name == "CNTS") + 2;

                    ws.Range(totalRow1, 2, totalRow1, 3).Merge();
                    ws.Cell(totalRow1, 2).Value = "TOTAL";
                    ws.Cell(totalRow1, 2).Style.Font.Bold = true;
                    ws.Cell(totalRow1, 2).Style.Font.FontSize = 10;
                    ws.Cell(totalRow1, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Range(totalRow1, 2, totalRow1, 3).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    ws.Cell(totalRow1, nwColIndex).Value = totalNW1;
                    ws.Cell(totalRow1, nwColIndex).Style.NumberFormat.Format = "#,##0.00";
                    ws.Cell(totalRow1, nwColIndex).Style.Font.Bold = true;
                    ws.Cell(totalRow1, nwColIndex).Style.Font.FontSize = 10;
                    ws.Cell(totalRow1, nwColIndex).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell(totalRow1, nwColIndex).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    ws.Cell(totalRow1, amountColIndex).Value = totalAmount1;
                    ws.Cell(totalRow1, amountColIndex).Style.NumberFormat.Format = "#,##0.00";
                    ws.Cell(totalRow1, amountColIndex).Style.Font.Bold = true;
                    ws.Cell(totalRow1, amountColIndex).Style.Font.FontSize = 10;
                    ws.Cell(totalRow1, amountColIndex).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell(totalRow1, amountColIndex).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    ws.Cell(totalRow1, CTNSColIndex).Value = totalCarton1;
                    ws.Cell(totalRow1, CTNSColIndex).Style.NumberFormat.Format = "#,##0";
                    ws.Cell(totalRow1, CTNSColIndex).Style.Font.Bold = true;
                    ws.Cell(totalRow1, CTNSColIndex).Style.Font.FontSize = 10;
                    ws.Cell(totalRow1, CTNSColIndex).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell(totalRow1, CTNSColIndex).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    ws.Cell(totalRow, 4).Style.NumberFormat.Format = "#,##0 \"CTNS\"";
                    ws.Cell(totalRow, 4).Value = totalCarton1;
                    // ===== Save file =====
                    using (SaveFileDialog sfd = new SaveFileDialog())
                    {
                        sfd.Filter = "Excel Workbook|*.xlsx";
                        sfd.FileName = "Invoice CNF - Final - N.W 2 ETD " + exportDate.ToString("dd")+"."+ exportDate.ToString("MM") + " " + exportCodeIndex + ".xlsx";
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

                    _ = SQLManager_Kho.Instance.UpsertExportHistoryAsync(exportCode, exportDate, totalAmount, totalNWReal, (int)totalCarton1, totalFreightCharge);

                    _ = SaveCustomerOrderDetailHistory();
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

        private async Task SaveCustomerOrderDetailHistory()
        {
            string exportCode = ((DataRowView)exportCode_cbb.SelectedItem)["ExportCode"].ToString();
            DateTime exportDate = Convert.ToDateTime(((DataRowView)exportCode_cbb.SelectedItem)["ExportDate"]);

            var orders_dt = await SQLStore_Kho.Instance.getOrdersAsync(mCurrentExportID);
            var query = orders_dt.AsEnumerable()
                                .GroupBy(r => new
                                {
                                    CustomerName = r.Field<string>("CustomerName"),
                                    GroupProduct = r.Field<int>("GroupProduct")
                                })
                                .Select(g => new
                                {
                                    CustomerName = g.Key.CustomerName,
                                    Package = g.First().Field<string>("Package"),
                                    // GroupProduct = g.Key.GroupProduct,

                                    TotalAmount = g.Sum(r =>
                                    {
                                        string package = r["Package"]?.ToString().ToLower() ?? "";
                                        decimal price = Convert.ToDecimal(r["OrderPackingPriceCNF"]);
                                        decimal nwReal = Convert.ToDecimal(string.IsNullOrEmpty(r["NWReal"].ToString()) ? 0 : r["NWReal"]);
                                        int pcsReal = Convert.ToInt32(string.IsNullOrEmpty(r["PCSReal"].ToString()) ? 0 : r["PCSReal"]);

                                        if (package == "kg" || package == "weight")
                                            return nwReal * price;
                                        else
                                            return pcsReal * price;
                                    }),

                                    TotalPCSReal = g.Sum(r => Convert.ToInt32(string.IsNullOrEmpty(r["PCSReal"].ToString()) ? 0 : r["PCSReal"])),
                                    TotalNWReal = g.Sum(r => Convert.ToDecimal(string.IsNullOrEmpty(r["NWReal"].ToString()) ? 0 : r["NWReal"])),
                                    ProductNameVN = GetCommonPrefixCleaned(g.Select(r => r.Field<string>("ProductNameVN")).ToList(), g.First().Field<string>("Package")),
                                    ProductNameEN = GetCommonPrefixCleaned(g.Select(r => r.Field<string>("ProductNameEN")).ToList(), g.First().Field<string>("Package")),
                                    Priority = g.Min(r => r.Field<int?>("Priority") ?? int.MaxValue)
                                }).ToList();            

            List<(string customerName, string exportCode, DateTime exportDate, string product_VN, string product_EN, string package, int PCS, decimal netWeight, decimal amount, int priority)> orders =
                new List<(string customerName, string exportCode, DateTime exportDate, string product_VN, string product_EN, string package, int PCS, decimal netWeight, decimal amount, int priority)>();

            foreach (var item in query)
            {
                orders.Add((
                    customerName: item.CustomerName,
                    exportCode: exportCode,
                    exportDate: exportDate,
                    product_VN: item.ProductNameVN,
                    product_EN: item.ProductNameEN,
                    package: item.Package,
                    PCS: item.TotalPCSReal,
                    netWeight: item.TotalNWReal,
                    amount: item.TotalAmount,
                    priority: item.Priority
                ));
            }

            _ = SQLManager_Kho.Instance.CustomerOrderDetailHistory_SaveListAsync(orders);
        }
        
        public string CleanProductName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "";

            // Xóa phần khối lượng: tìm pattern "số + khoảng trắng + (g|gr|kg)" ở cuối chuỗi
            // ví dụ: "galadium 200 gr", "baby corn 1 kg"
            string pattern = @"\s*\d+(\.\d+)?\s*(g|gr|kg)\s*$";
            string cleaned = Regex.Replace(name, pattern, "", RegexOptions.IgnoreCase);

            return cleaned.Trim();
        }

        public string GetCommonPrefixCleaned(List<string> names, string package)
        {
            if (names == null || names.Count == 0)
                return "";

            // Làm sạch tên trước khi tìm prefix
            var cleanedNames = names;
            if (package.CompareTo("kg") == 0 || package.CompareTo("weight") == 0)
                cleanedNames = names.Select(n => CleanProductName(n)).ToList();
            string prefix = cleanedNames[0];

            foreach (var name in cleanedNames)
            {
                int len = Math.Min(prefix.Length, name.Length);
                int i = 0;

                while (i < len && prefix[i] == name[i])
                {
                    i++;
                }

                prefix = prefix.Substring(0, i);

                if (string.IsNullOrWhiteSpace(prefix))
                    return "";
            }

            if (prefix.Contains("("))
                prefix = prefix.Substring(0, prefix.IndexOf("("));

            return prefix.Trim();
        }

        private void CustomerHomeBtn_Click(object sender, EventArgs e)
        {
            DateTime exportDate = Convert.ToDateTime(((DataRowView)exportCode_cbb.SelectedItem)["ExportDate"]);

            var result = mCustomerOrdersTotal_dt
                .AsEnumerable()
                .OrderBy(r => r.Field<string>("Home"))
                .ThenBy(r => r.Field<string>("fullname"))
                .ToList();

            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add("Group");
                ws.Cell("A1").Value = exportDate.Date;
                ws.Cell("A1").Style.Font.FontColor = XLColor.Red;
                ws.Range(1, 1, 1, 2).Merge();

                ws.Cell("A2").Value = "No.";
                ws.Cell("B2").Value = "MARK";
                ws.Cell("C2").Value = "CNTS";
                ws.Cell("A2").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Cell("B2").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Cell("C2").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Cell("A2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("B2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("C2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                int row = 3;

                string currentHome = null;
                int homeTotal = 0;
                int total = 0;
                int No = 1;
                foreach (var item in result)
                {
                    string home = item.Field<string>("Home");
                    string fullname = item.Field<string>("fullname");
                    int cnts = item.Field<int>("CNTS");

                    // Khi Home thay đổi → ghi dòng total
                    if (currentHome != null && currentHome != home)
                    {
                        // Dòng tổng
                        ws.Cell(row, 1).Value = "TOTAL " + currentHome;
                        ws.Cell(row, 3).Value = homeTotal;
                        ws.Range(row, 1, row, 3).Style.Font.Bold = true;
                        ws.Range(row, 1, row, 2).Merge();
                        ws.Range(row, 1, row, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(row, 3).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Range(row, 1, row, 3).Style.Fill.BackgroundColor = XLColor.Orange;
                        ws.Range(row, 1, row, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell(row, 3).Style.Font.FontColor = XLColor.Red;
                        ws.Cell(row, 3).Style.NumberFormat.Format = "#,##0 \"CTNS\"";
                        row++;

                        // Reset tổng
                        homeTotal = 0;
                        No = 1;
                    }

                    // Ghi dòng chi tiết
                    ws.Cell(row, 1).Value = No++;
                    ws.Cell(row, 2).Value = fullname;
                    ws.Cell(row, 3).Value = cnts;
                    ws.Cell(row, 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    ws.Cell(row, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    ws.Cell(row, 3).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    ws.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell(row, 3).Style.NumberFormat.Format = "#,##0 \"CTNS\"";
                    row++;

                    // Cộng tổng
                    homeTotal += cnts;
                    currentHome = home;
                    total += cnts;
                }

                // Ghi total của nhóm cuối
                if (currentHome != null)
                {
                    ws.Cell(row, 1).Value = "TOTAL " + currentHome;
                    ws.Cell(row, 3).Value = homeTotal;
                    ws.Range(row, 1, row, 3).Style.Font.Bold = true;
                    ws.Range(row, 1, row, 2).Merge();
                    ws.Range(row, 1, row, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    ws.Cell(row, 3).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    ws.Range(row, 1, row, 3).Style.Fill.BackgroundColor = XLColor.Orange;
                    ws.Range(row, 1, row, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell(row, 3).Style.Font.FontColor = XLColor.Red;
                    ws.Cell(row, 3).Style.NumberFormat.Format = "#,##0 \"CTNS\"";
                }

                if (currentHome != null)
                {
                    row++;
                    ws.Cell(row, 1).Value = "TOTAL";
                    ws.Cell(row, 3).Value = total;
                    ws.Range(row, 1, row, 3).Style.Font.Bold = true;
                    ws.Range(row, 1, row, 2).Merge();
                    ws.Range(row, 1, row, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    ws.Cell(row, 3).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    ws.Range(row, 1, row, 3).Style.Fill.BackgroundColor = XLColor.PinkOrange;
                    ws.Range(row, 1, row, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell(row, 3).Style.NumberFormat.Format = "#,##0 \"CTNS\"";
                }

                ws.Column(1).Width = 7;
                ws.Column(2).Width = 14;
                ws.Column(3).Width = 10;

                // Lưu file
                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = "Excel Workbook|*.xlsx";
                    sfd.FileName = "CustomerTotal.xlsx";
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        workbook.SaveAs(sfd.FileName);

                        if (MessageBox.Show("Bạn có muốn mở file này không?",
                            "Lưu file thành công",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                            {
                                FileName = sfd.FileName,
                                UseShellExecute = true
                            });
                        }
                    }
                }
            }
        }

    }
}
