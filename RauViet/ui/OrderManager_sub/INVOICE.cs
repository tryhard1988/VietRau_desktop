using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using RauViet.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Color = System.Drawing.Color;

namespace RauViet.ui
{
    public partial class INVOICE : Form
    {
        DataTable mExportCode_dt, mOrdersTotal_dt, mCustomerOrdersTotal_dt, mCartonOrdersTotal_dt;
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
                SQLStore.Instance.removeOrdersCartonInvoice(mCurrentExportID);
                SQLStore.Instance.removeOrdersCusInvoice(mCurrentExportID);
                SQLStore.Instance.removeOrdersInvoice(mCurrentExportID);
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
                mExportCode_dt = await SQLStore.Instance.getExportCodesAsync(keepColumns, parameters);                  

                if (mCurrentExportID <= 0 && mExportCode_dt.Rows.Count > 0)
                {
                    mCurrentExportID = Convert.ToInt32(mExportCode_dt.AsEnumerable()
                                   .Max(r => r.Field<int>("ExportCodeID")));
                }

                var cartonOrdersTask = SQLStore.Instance.getOrdersCartonInvoiceAsync(mCurrentExportID);
                var customersOrdersTask = SQLStore.Instance.getOrdersCusInvoiceAsync(mCurrentExportID);
                var OrdersInvoiceTask = SQLStore.Instance.getOrdersInvoiceAsync(mCurrentExportID);

                await Task.WhenAll(customersOrdersTask, OrdersInvoiceTask, cartonOrdersTask);
                mOrdersTotal_dt = OrdersInvoiceTask.Result;
                mCustomerOrdersTotal_dt = customersOrdersTask.Result;
                mCartonOrdersTotal_dt = cartonOrdersTask.Result;

                showOrdersExport();
                showCustomerOrderGV();
                showCartonOrderGV();

                exportCode_cbb.SelectedIndexChanged -= exportCode_search_cbb_SelectedIndexChanged;
                exportCode_cbb.DataSource = mExportCode_dt;
                exportCode_cbb.DisplayMember = "ExportCode";  // hiển thị tên
                exportCode_cbb.ValueMember = "ExportCodeID";
                exportCode_cbb.SelectedValue = mCurrentExportID;
                exportCode_cbb.SelectedIndexChanged += exportCode_search_cbb_SelectedIndexChanged;
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

        private void showOrdersExport()
        {
            DataView dv = new DataView(mOrdersTotal_dt);
            dataGV.DataSource = dv;

            dataGV.Columns["ExportCodeID"].Visible = false;

            dataGV.Columns["No"].HeaderText = "No";
            dataGV.Columns["ProductNameEN"].HeaderText = "English name";
            dataGV.Columns["ProductNameVN"].HeaderText = "Vietnamese name";
            dataGV.Columns["Package"].HeaderText = "Unit";
            dataGV.Columns["NWReal"].HeaderText = "N.W(kg)";
            dataGV.Columns["PCSReal"].HeaderText = "PCS";
            dataGV.Columns["OrderPackingPriceCNF"].HeaderText = "Price (CHF)";
            dataGV.Columns["AmountCHF"].HeaderText = "Amount (CHF)";

            dataGV.Columns["AmountCHF"].DefaultCellStyle.Format = "N3";
            dataGV.Columns["Quantity"].DefaultCellStyle.Format = "N3";

            dataGV.Columns["Priority"].HeaderText = "Ưu\nTiên";

            dataGV.Columns["Priority"].Width = 50;
            dataGV.Columns["No"].Width = 50;
            dataGV.Columns["PLU"].Width = 50;
            dataGV.Columns["ProductNameEN"].Width = 130;
            dataGV.Columns["ProductNameVN"].Width = 160;
            dataGV.Columns["Package"].Width = 50;
            dataGV.Columns["Quantity"].Width = 60;
            dataGV.Columns["NWReal"].Width = 50;
            dataGV.Columns["Packing"].Width = 50;
            dataGV.Columns["PCSReal"].Width = 50;
            dataGV.Columns["OrderPackingPriceCNF"].Width = 50;
            dataGV.Columns["AmountCHF"].Width = 70;

            dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void showCustomerOrderGV()
        {
            DataView dv = new DataView(mCustomerOrdersTotal_dt);
            cusOrderGV.DataSource = dv;

            cusOrderGV.Columns["ExportCodeID"].Visible = false;
            cusOrderGV.Columns["FullName"].HeaderText = "MARK";
            cusOrderGV.Columns["NWReal"].HeaderText = "N.W";
            cusOrderGV.Columns["AmountCHF"].HeaderText = "Amount\nCHF";

            cusOrderGV.Columns["No"].Width = 30;
            cusOrderGV.Columns["CNTS"].Width = 40;
            cusOrderGV.Columns["NWReal"].Width = 70;
            cusOrderGV.Columns["AmountCHF"].Width = 70;
            cusOrderGV.Columns["FullName"].Width = 100;

            cusOrderGV.Columns["AmountCHF"].DefaultCellStyle.Format = "N3";
        }

        private void showCartonOrderGV()
        {

            DataView dv = new DataView(mCartonOrdersTotal_dt);
            cartonSizeGV.DataSource = dv;
            cartonSizeGV.Columns["ExportCodeID"].Visible = false;
            cartonSizeGV.Columns["CartonSize"].HeaderText = "Carton Size";
            cartonSizeGV.Columns["CountCarton"].HeaderText = "Quantity";

            cartonSizeGV.Columns["No"].Width = 30;
            cartonSizeGV.Columns["Weight"].Width = 60;
            cartonSizeGV.Columns["CountCarton"].Width = 50;
            cartonSizeGV.Columns["CartonSize"].Width = 120;

            Dictionary<int, int> countDic = new Dictionary<int, int>();
            foreach (DataRow dr in mCartonOrdersTotal_dt.Rows)
            {
                int exportCodeID = Convert.ToInt32(dr["ExportCodeID"]);
                if (!countDic.ContainsKey(exportCodeID))
                {
                    countDic.Add(exportCodeID, 1);
                }
                dr["No"] = countDic[exportCodeID]++;
                decimal countCarton = Convert.ToDecimal(dr["CountCarton"]);

                string cartonSizeStr = dr["CartonSize"].ToString().Replace(" ", "");
                if (cartonSizeStr.CompareTo("") != 0)
                {
                    string[] parts = cartonSizeStr.Split('x');
                    decimal result = parts.Select(p => int.Parse(p)).Aggregate(1, (a, b) => a * b);
                    result *= countCarton;
                    result /= Convert.ToDecimal(6000);

                    dr["Weight"] = result;
                }
            }
        }

        private async void exportCode_search_cbb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!int.TryParse(exportCode_cbb.SelectedValue.ToString(), out int exportCodeId))
                return;

            mCurrentExportID = exportCodeId;

            var cartonOrdersTask = SQLStore.Instance.getOrdersCartonInvoiceAsync(mCurrentExportID);
            var customersOrdersTask = SQLStore.Instance.getOrdersCusInvoiceAsync(mCurrentExportID);
            var OrdersInvoiceTask = SQLStore.Instance.getOrdersInvoiceAsync(mCurrentExportID);

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
                    ws.Cell(2, 1).Value = "Group 1, Hamlet 4, Phuoc Thai Ward, Dong Nai Province, Vietnam";
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
                    ws.Cell(5, 1).Value = "Shipper:\r\nVIET RAU JOINT STOCK COMPANY\r\nGroup 1, Hamlet 4, Phuoc Thai Ward, Dong Nai Province, Vietnam\r\nTel/Fax: +84 251 2860828\r\nEmail: acc@vietrau.com";
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

                    ws.Cell(totalRow, nwAmountColIndex).Value = Math.Round(totalAmount, 2);
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
                    ws.Cell(totalRow, 4).Value = Math.Round(totalAmount - totalFreightCharge, 2);
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
                    ws.Cell(totalRow, 4).Value = Math.Round(totalFreightCharge, 2);
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
                    ws.Cell(totalRow, 4).Value = Math.Round(totalAmount, 2);
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
                    ws.Cell(totalRow, 4).Value = Math.Round(totalNWReal, 2);
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

                    ws.Cell(totalRow1, amountColIndex).Value = Math.Round(totalAmount1, 2);
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
                        sfd.FileName = " Invoice CNF - Final - N.W 2 ETD " + exportDate.ToString("dd")+"."+ exportDate.ToString("MM") + " " + exportCodeIndex + ".xlsx";
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

                    _ = SQLManager.Instance.UpsertExportHistoryAsync(exportCode, exportDate, totalAmount, totalNWReal, (int)totalCarton1, totalFreightCharge);
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
    }
}
