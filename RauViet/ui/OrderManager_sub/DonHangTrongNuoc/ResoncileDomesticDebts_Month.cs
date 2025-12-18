using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using RauViet.classes;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Color = System.Drawing.Color;

namespace RauViet.ui
{
    public partial class ResoncileDomesticDebts_Month : Form
    {
        private LoadingOverlay loadingOverlay;
        DataTable mDetail_dt, mSummary_dt;
        DataView mDetail_DV, mSummary_DV;
        public ResoncileDomesticDebts_Month()
        {
            InitializeComponent();
            this.KeyPreview = true;

            timeReport_dtp.Format = DateTimePickerFormat.Custom;
            timeReport_dtp.CustomFormat = "MM/yyyy";
            timeReport_dtp.ShowUpDown = true;
            timeReport_dtp.Value = DateTime.Now;

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;
            sumary_GV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            sumary_GV.MultiSelect = false;
            detailGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            detailGV.MultiSelect = false;

            status_lb.Text = "";

            load_btn.Click += Load_btn_Click;
            this.KeyDown += ReportOrder_Year_KeyDown;

            preview_print_TD_btn.Click += Preview_print_TD_btn_Click;
            print_btn.Click += Print_btn_Click;
            exportToExcel_btn.Click += ExportToExcel_btn_Click;
        }

        private void ReportOrder_Year_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                int year = timeReport_dtp.Value.Year;                

                SQLStore_Kho.Instance.RemoveExportHistoryByYear(year);
                SQLStore_Kho.Instance.RemoveCustomerOrderDetailHistoryByYear(year);
                ShowData();
            }
        }

        public async void ShowData()
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            await Task.Delay(50);
            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Show();
            await Task.Delay(50);

            dataGV.CellClick -= CustomerGV_CellClick;

            try
            {
                int year = timeReport_dtp.Value.Year;
                int month = timeReport_dtp.Value.Month;
                var customerOrderHistoryByYearTask = SQLStore_Kho.Instance.GetOrderDomesticByMonthYearAsync(month, year);

                await Task.WhenAll(customerOrderHistoryByYearTask);
                mDetail_dt = customerOrderHistoryByYearTask.Result;
                DataTable customer_dt = new DataTable();
                {
                    var query = customerOrderHistoryByYearTask.Result
                                                                .AsEnumerable()
                                                                .GroupBy(r => r.Field<int>("CustomerID"))
                                                                .Select(g => new
                                                                {
                                                                    CustomerID = g.Key,
                                                                    Company = g.Select(r => r.Field<string>("Company")).FirstOrDefault(c => !string.IsNullOrWhiteSpace(c))
                                                                });

                    customer_dt.Columns.Add("CustomerID", typeof(int));
                    customer_dt.Columns.Add("Company", typeof(string));

                    foreach (var item in query)
                    {
                        customer_dt.Rows.Add(item.CustomerID, item.Company);
                    }
                }

                mSummary_dt = new DataTable();
                {
                    var query = customerOrderHistoryByYearTask.Result
                                                            .AsEnumerable()
                                                            .GroupBy(r => new
                                                            {
                                                                CustomerID = r.Field<int>("CustomerID"),
                                                                ProductNameVN = r.Field<string>("ProductNameVN"),
                                                                ProductTypeName = r.Field<string>("ProductTypeName")
                                                            })
                                                            .Select(g => new
                                                            {
                                                                CustomerID = g.Key.CustomerID,
                                                                ProductNameVN = g.Key.ProductNameVN,
                                                                ProductTypeName = g.Key.ProductTypeName,
                                                                PCSReal = g.Sum(r => r.Field<int?>("PCSReal") ?? 0),
                                                                NWReal = g.Sum(r => r.Field<decimal?>("NWReal") ?? 0m),
                                                                Price = g.Select(r => r.Field<int?>("Price")).FirstOrDefault(p => p.HasValue) ?? 0,
                                                                TotalAmount = g.Sum(r => r.Field<int?>("TotalAmount") ?? 0)
                                                            }).OrderBy(x => x.ProductNameVN);

                    mSummary_dt.Columns.Add("CustomerID", typeof(int));
                    mSummary_dt.Columns.Add("ProductNameVN", typeof(string));
                    mSummary_dt.Columns.Add("ProductTypeName", typeof(string));
                    mSummary_dt.Columns.Add("PCSReal", typeof(int));
                    mSummary_dt.Columns.Add("NWReal", typeof(decimal));
                    mSummary_dt.Columns.Add("Price", typeof(decimal));
                    mSummary_dt.Columns.Add("TotalAmount", typeof(decimal));

                    foreach (var item in query)
                    {
                        mSummary_dt.Rows.Add(item.CustomerID, item.ProductNameVN, item.ProductTypeName, item.PCSReal, item.NWReal, item.Price, item.TotalAmount);
                    }
                }


                dataGV.DataSource = customer_dt;

                mDetail_DV = new DataView(mDetail_dt);
                detailGV.DataSource = mDetail_DV;

                mSummary_DV = new DataView(mSummary_dt);
                sumary_GV.DataSource = mSummary_DV;



                dataGV.Columns["CustomerID"].Visible = false;
                dataGV.Columns["Company"].HeaderText = "Khách Hàng";
                dataGV.Columns["Company"].Width = 160;

                sumary_GV.Columns["CustomerID"].Visible = false;
                sumary_GV.Columns["ProductNameVN"].Width = 110;
                sumary_GV.Columns["ProductTypeName"].Width = 85;
                sumary_GV.Columns["PCSReal"].Width = 50;
                sumary_GV.Columns["NWReal"].Width = 50;
                sumary_GV.Columns["Price"].Width = 60;
                sumary_GV.Columns["TotalAmount"].Width = 80;
                sumary_GV.Columns["ProductNameVN"].HeaderText = "Tên Sản Phẩm";
                sumary_GV.Columns["ProductTypeName"].HeaderText = "Loại Hàng";
                sumary_GV.Columns["PCSReal"].HeaderText = "PCS";
                sumary_GV.Columns["NWReal"].HeaderText = "N.W";
                sumary_GV.Columns["Price"].HeaderText = "Giá";
                sumary_GV.Columns["Price"].DefaultCellStyle.Format = "N0";
                sumary_GV.Columns["TotalAmount"].DefaultCellStyle.Format = "N0";
                sumary_GV.Columns["PCSReal"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                sumary_GV.Columns["NWReal"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                sumary_GV.Columns["Price"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                sumary_GV.Columns["TotalAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                detailGV.Columns["ProductPackingID"].Visible = false;
                detailGV.Columns["CustomerProductTypesCode"].Visible = false;
                detailGV.Columns["SKU"].Visible = false;
                detailGV.Columns["Company"].Visible = false;
                detailGV.Columns["CustomerID"].Visible = false;
                detailGV.Columns["Amount"].Visible = false;
                detailGV.Columns["packing"].Visible = false;
                detailGV.Columns["ProductNameVN"].HeaderText = "Tên Sản Phẩm";
                detailGV.Columns["ProductTypeName"].HeaderText = "Loại Hàng";
                detailGV.Columns["Package"].HeaderText = "Đ.Vị";
                detailGV.Columns["Price"].HeaderText = "Giá";
                detailGV.Columns["PCSReal"].HeaderText = "PCS";
                detailGV.Columns["NWReal"].HeaderText = "N.W";
                detailGV.Columns["OrderDomesticIndex"].HeaderText = "Số Phiếu";
                detailGV.Columns["DeliveryDate"].HeaderText = "Ngày Giao";
                detailGV.Columns["AmountPacking"].HeaderText = "Quy Cách";
                detailGV.Columns["TotalAmount"].HeaderText = "Thành Tiền";
                detailGV.Columns["PCSReal"].Width = 50;
                detailGV.Columns["NWReal"].Width = 50;
                detailGV.Columns["Price"].Width = 50;
                detailGV.Columns["Package"].Width = 50;
                detailGV.Columns["OrderDomesticIndex"].Width = 50;
                detailGV.Columns["DeliveryDate"].Width = 80;
                detailGV.Columns["TotalAmount"].Width = 80;

                detailGV.Columns["Price"].DefaultCellStyle.Format = "N0";
                detailGV.Columns["TotalAmount"].DefaultCellStyle.Format = "N0";

                
                dataGV.CellClick += CustomerGV_CellClick;

                if (dataGV.Rows.Count > 0)
                {
                    dataGV.ClearSelection();
                    dataGV.Rows[0].Selected = true;
                    dataGV.CurrentCell = dataGV.Rows[0].Cells[1];
                }

                var sumNWRealObj = mSummary_dt.Compute("SUM(NWReal)", "");
                decimal sumNWReal = sumNWRealObj == DBNull.Value ? 0 : Convert.ToDecimal(sumNWRealObj);

                var sumTotalAmountObj = mSummary_dt.Compute("SUM(TotalAmount)", "");
                decimal sumTotalAmount = sumTotalAmountObj == DBNull.Value ? 0 : Convert.ToDecimal(sumTotalAmountObj);

                totalNW_tb.Text = sumNWReal.ToString("N2");
                totalMoney_tb.Text = sumTotalAmount.ToString("N0");

                CustomerGV_CellClick(null, null);
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

        private void CustomerGV_CellClick(object sender, EventArgs e)
        {
            if (dataGV.CurrentRow == null) return;
            int CustomerID = Convert.ToInt32(dataGV.CurrentRow.Cells["CustomerID"].Value);

            mDetail_DV.RowFilter = $"CustomerID = {CustomerID}";
            mSummary_DV.RowFilter = $"CustomerID = {CustomerID}";
            //mCustomerDetail_DV.Sort = "Priority ASC, ProductName_VN ASC";
        }

        private async void Load_btn_Click(object sender, EventArgs e)
        {
            ShowData();
        }

        private void Print_btn_Click(object sender, EventArgs e)
        {
            _ = PrintPendingOrderSummary(2);
        }

        private void Preview_print_TD_btn_Click(object sender, EventArgs e)
        {
            _ = PrintPendingOrderSummary(1);
        }

        private async Task PrintPendingOrderSummary(int state)
        {
            if (dataGV.CurrentRow == null) return;

            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Message = "Đang xử lý ...";
            loadingOverlay.Show();
            await Task.Delay(100);

            DataTable cus_dt = await SQLStore_Kho.Instance.getCustomersAsync();
            int CustomerID = Convert.ToInt32(dataGV.CurrentRow.Cells["CustomerID"].Value);

            DataRow[] rows = cus_dt.Select($"CustomerID = {CustomerID}");
            if (rows.Length < 0)
            {
                MessageBox.Show("Có Lỗi");
                return;
            }

            string Company = rows[0]["Company"].ToString();
            string Address = rows[0]["Address"].ToString();
            string CustomerName = rows[0]["FullName"].ToString();
            string email = rows[0]["Email"].ToString();
            string taxCode = rows[0]["TaxCode"].ToString();

            DataTable resultTable = mDetail_dt.AsEnumerable()
                .Where(r => r.Field<int>("CustomerID") == CustomerID)
                .CopyToDataTable();

            DoiChieuCongNo_Printer printer = new DoiChieuCongNo_Printer();

            if (state == 1)
                printer.PrintPreview(resultTable, Company, CustomerName, Address, email, taxCode, this);
            else if (state == 2)
                printer.PrintDirect(resultTable, Company, CustomerName, Address, email, taxCode, tongdon_in2mat_cb.Checked);
            await Task.Delay(200);
            loadingOverlay.Hide();
        }


        private async void ExportToExcel_btn_Click(object sender, EventArgs e)
        {
            DataTable cus_dt = await SQLStore_Kho.Instance.getCustomersAsync();
            int CustomerID = Convert.ToInt32(dataGV.CurrentRow.Cells["CustomerID"].Value);

            DataRow[] rows = cus_dt.Select($"CustomerID = {CustomerID}");
            if (rows.Length < 0)
            {
                MessageBox.Show("Có Lỗi");
                return;
            }

            string Company = rows[0]["Company"].ToString();
            string Address = rows[0]["Address"].ToString();
            string CustomerName = rows[0]["FullName"].ToString();
            string email = rows[0]["Email"].ToString();
            string taxCode = rows[0]["TaxCode"].ToString();

            DataTable resultTable = mDetail_dt.AsEnumerable()
                .Where(r => r.Field<int>("CustomerID") == CustomerID)
                .CopyToDataTable();

            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Message = "Đang xử lý ...";
            loadingOverlay.Show();
            await Task.Delay(100);

            try
            {
                using (var wb = new XLWorkbook())
                {
                    var ws = wb.Worksheets.Add("Data");
                    ws.Style.Font.FontName = "Times New Roman";
                    ws.Style.Font.FontSize = 11;
                    ws.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    
                    ws.Range(1, 4, 1, 5).Merge();
                    int rowInd = 1;

                    int totalColumn = 11;
                    ws.Range(rowInd, 1, rowInd, totalColumn).Merge();
                    ws.Cell(rowInd, 1).Value = "CÔNG TY CỔ PHẦN VIỆT RAU";
                    ws.Cell(rowInd, 1).Style.Font.Bold = true;
                    ws.Cell(rowInd, 1).Style.Font.FontSize = 15;
                    ws.Cell(rowInd, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell(rowInd, 1).Style.Font.FontColor = XLColor.FromArgb(198, 89, 17);

                    rowInd++;
                    ws.Range(rowInd, 1, rowInd, totalColumn).Merge();
                    ws.Cell(rowInd, 1).Value = "MST : 0313983703";
                    ws.Cell(rowInd, 1).Style.Font.Bold = true;
                    ws.Cell(rowInd, 1).Style.Font.FontSize = 10;
                    ws.Cell(rowInd, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell(rowInd, 1).Style.Font.FontColor = XLColor.FromArgb(198, 89, 17);

                    rowInd++;
                    ws.Range(rowInd, 1, rowInd, totalColumn).Merge();
                    ws.Cell(rowInd, 1).Value = "Địa chỉ: Tổ 1, Ấp 4, Xã Phước Thái, Tỉnh Đông Nai, Việt Nam";
                    ws.Cell(rowInd, 1).Style.Font.FontSize = 10;
                    ws.Cell(rowInd, 1).Style.Font.Italic = true;
                    ws.Cell(rowInd, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell(rowInd, 1).Style.Font.FontColor = XLColor.FromArgb(198, 89, 17);

                    rowInd++;
                    ws.Range(rowInd, 1, rowInd, totalColumn).Merge();
                    ws.Cell(rowInd, 1).Value = "Email: Acc@vietrau.com                           ĐT: 0251 2860828/0909244916";
                    ws.Cell(rowInd, 1).Style.Font.FontSize = 10;
                    ws.Cell(rowInd, 1).Style.Font.Italic = true;
                    ws.Cell(rowInd, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell(rowInd, 1).Style.Font.FontColor = XLColor.FromArgb(198, 89, 17);

                    rowInd+=2;
                    ws.Range(rowInd, 1, rowInd, totalColumn).Merge();
                    ws.Range(rowInd, 1, rowInd, totalColumn).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    rowInd++;
                    ws.Range(rowInd, 1, rowInd, totalColumn).Merge();
                    ws.Cell(rowInd, 1).Value = "BẢNG KÊ CHI TIẾT HÀNG BÁN HÀNG THÁNG 11/2025";
                    ws.Cell(rowInd, 1).Style.Font.Bold = true;
                    ws.Cell(rowInd, 1).Style.Font.FontSize = 15;
                    ws.Cell(rowInd, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    rowInd+=2;
                    ws.Range(rowInd, 1, rowInd, totalColumn).Merge();
                    ws.Cell(rowInd, 1).Value = "Tên đơn vị: " + Company;
                    ws.Cell(rowInd, 1).Style.Font.Bold = true;
                    ws.Cell(rowInd, 1).Style.Font.FontSize = 11;
                    ws.Cell(rowInd, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    rowInd++;
                    ws.Range(rowInd, 1, rowInd, totalColumn/2).Merge();
                    ws.Cell(rowInd, 1).Value = "MST: " + taxCode;
                    ws.Cell(rowInd, 1).Style.Font.Bold = true;
                    ws.Cell(rowInd, 1).Style.Font.FontSize = 10;
                    ws.Cell(rowInd, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    ws.Range(rowInd, totalColumn/2 + 1, rowInd, totalColumn).Merge();
                    ws.Cell(rowInd, totalColumn / 2 + 1).Value = "Email: " + email;
                    ws.Cell(rowInd, totalColumn / 2 + 1).Style.Font.FontSize = 11;
                    ws.Cell(rowInd, totalColumn / 2 + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    rowInd++;
                    ws.Range(rowInd, 1, rowInd, totalColumn).Merge();
                    ws.Cell(rowInd, 1).Value = "Địa chỉ: " + Address;
                    ws.Cell(rowInd, 1).Style.Font.Bold = true;
                    ws.Cell(rowInd, 1).Style.Font.FontSize = 10;
                    ws.Cell(rowInd, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    rowInd++;
                    ws.Range(rowInd, 1, rowInd, totalColumn).Merge();
                    ws.Cell(rowInd, 1).Value = "Liện hệ: " + CustomerName;
                    ws.Cell(rowInd, 1).Style.Font.FontSize = 10;
                    ws.Cell(rowInd, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                                        
                    int headerStartRow = 13; // Header cấp 2 và 3 bắt đầu từ hàng 5
                    int columnIndex = 1;

                    SetHeader(ws, headerStartRow, columnIndex++, "STT");
                    SetHeader(ws, headerStartRow, columnIndex++, "NGÀY XUẤT");
                    SetHeader(ws, headerStartRow, columnIndex++, "SỐ PHIẾU");

                    SetSubHeader(ws, headerStartRow, columnIndex, "SKU");
                    SetSubHeader(ws, headerStartRow + 1, columnIndex, "Art.\nNr");
                    columnIndex++;

                    SetHeader(ws, headerStartRow, columnIndex++, "TÊN HÀNG");
                    SetHeader(ws, headerStartRow, columnIndex++, "loại Hàng");
                    SetHeader(ws, headerStartRow, columnIndex++, "Đ.Vị");

                    SetHeader(ws, headerStartRow, columnIndex, "SỐ LƯỢNG", rowSpan: 1, colSpan: 2);
                    SetSubHeader(ws, headerStartRow + 1, columnIndex++, "Gói");
                    SetSubHeader(ws, headerStartRow + 1, columnIndex++, "Kg");

                    SetHeader(ws, headerStartRow, columnIndex++, "ĐƠN GIÁ");
                    SetHeader(ws, headerStartRow, columnIndex++, "THÀNH TIỀN");
                    string[] columnsToExport = new string[] { "DeliveryDate", "SKU", "ProductNameVN", "ProductTypeName", "Package", "PCSReal", "NWReal", "Price", "TotalAmount"};
                    // ===== Data bắt đầu từ hàng 7 =====
                    decimal totalAmount = 0; // Biến lưu tổng
                    int rowIndex = headerStartRow + 2;
                    int STTCount = 1;

                    DateTime? currentDate = null;
                    int deliveryDateStartRow = rowIndex;
                    int deliveryDateColumnIndex = 0;

                    foreach (DataRow row in resultTable.Rows)
                    {
                        columnIndex = 1;
                        var STTcell = ws.Cell(rowIndex, columnIndex);
                        STTcell.Value = STTCount++;
                        STTcell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        columnIndex++;

                        foreach (var col in columnsToExport)
                        {
                            var cell = ws.Cell(rowIndex, columnIndex);
                            var cellValue = row[col].ToString() ?? "";
                            cell.Value = cellValue;
                            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            if (col == "Package")
                            {
                                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                if (cellValue.CompareTo("weight") == 0)
                                    cell.Value = "kg";
                            }
                            else if(col == "NWReal")
                            {
                                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                cell.Style.NumberFormat.Format = "#,##0.00";
                                if (decimal.TryParse(cellValue, out decimal num))
                                    cell.Value = num;
                            }
                            else if (col == "PCSReal" )
                            {
                                cell.Style.NumberFormat.Format = "#,##0;-#,##0;;";
                                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                if (decimal.TryParse(cellValue, out decimal num))
                                    cell.Value = num;
                            }
                            else if (col == "Price" || col == "TotalAmount")
                            {
                                cell.Style.NumberFormat.Format = "#,##0";
                                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                                if (decimal.TryParse(cellValue, out decimal num))
                                    cell.Value = num;

                                if (col == "TotalAmount")
                                {
                                    totalAmount += num;
                                }
                            }
                            else if (col == "DeliveryDate")
                            {
                                deliveryDateColumnIndex = columnIndex;

                                DateTime dateValue = Convert.ToDateTime(row[col]);

                                // Nếu là ngày đầu tiên hoặc khác ngày trước
                                if (currentDate == null || currentDate.Value.Date != dateValue.Date)
                                {
                                    // merge nhóm trước đó
                                    if (currentDate != null && rowIndex - 1 > deliveryDateStartRow)
                                    {
                                        ws.Range(deliveryDateStartRow, deliveryDateColumnIndex,  rowIndex - 1, deliveryDateColumnIndex)
                                            .Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                                        ws.Range(deliveryDateStartRow, deliveryDateColumnIndex + 1, rowIndex - 1, deliveryDateColumnIndex + 1)
                                            .Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                                    }

                                    // bắt đầu nhóm mới
                                    currentDate = dateValue.Date;
                                    deliveryDateStartRow = rowIndex;

                                    cell.Value = dateValue;
                                    cell.Style.DateFormat.Format = "dd/MM/yyyy";
                                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                    cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                                    ws.Cell(rowIndex, columnIndex + 1).Value = row["OrderDomesticIndex"].ToString() ?? "";
                                    ws.Cell(rowIndex, columnIndex + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                                }
                                else
                                {
                                    // cùng ngày → không set value (để merge)
                                    cell.Value = "";
                                    ws.Cell(rowIndex, columnIndex + 1).Value = "";
                                }
                                columnIndex++;
                            }
                            columnIndex++;
                        }

                        
                        rowIndex++;
                    }

                    if (currentDate != null && rowIndex - 1 > deliveryDateStartRow)
                    {
                        ws.Range(deliveryDateStartRow, deliveryDateColumnIndex, rowIndex - 1, deliveryDateColumnIndex)
                            .Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                        ws.Range(deliveryDateStartRow, deliveryDateColumnIndex + 1, rowIndex - 1, deliveryDateColumnIndex + 1)
                            .Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                        ws.Range(deliveryDateStartRow, deliveryDateColumnIndex + 1, rowIndex - 1, deliveryDateColumnIndex + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    }

                    ws.Range(rowIndex, 1, rowIndex, totalColumn - 2)
                        .Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                    ws.Range(rowIndex, 1, rowIndex, totalColumn - 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    ws.Cell(rowIndex, 1).Value = "TỔNG CỘNG";
                    ws.Cell(rowIndex, 1).Style.Font.Bold = true;
                    ws.Cell(rowIndex, totalColumn-1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    ws.Cell(rowIndex, totalColumn).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    
                    ws.Cell(rowIndex, 1).Value = "TỔNG CỘNG";
                    ws.Cell(rowIndex, totalColumn).Style.Font.Bold = true;
                    ws.Cell(rowIndex, totalColumn).Value = totalAmount;
                    ws.Cell(rowIndex, totalColumn).Style.NumberFormat.Format = "#,##0";

                    ws.Columns().AdjustToContents();

                    // Lấy hình từ Resources
                    Image logo1 = Properties.Resources.ic_logo;

                    // Chuyển Image thành Stream
                    using (var ms = new MemoryStream())
                    {
                        logo1.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        ms.Position = 0; // reset vị trí stream về đầu

                        // Thêm hình vào Excel
                        var picture = ws.AddPicture(ms).MoveTo(ws.Cell(1, 1), 25, 10).Scale(0.13);
                    }

                    ws.Row(6).Height = 2;
                    ws.Row(13).Height = 29.5;
                    ws.Row(14).Height = 29.5;
                    ws.Column(1).Width = 4;
                    ws.Column(2).Width = 9.5;
                    ws.Column(3).Width = 8;
                    ws.Column(4).Width = 4.8;
                    ws.Column(5).Width = 18;
                    ws.Column(6).Width = 13;
                    ws.Column(7).Width = 5.5;
                    ws.Column(8).Width = 3.7;
                    ws.Column(9).Width = 5.7;
                    ws.Column(10).Width = 7.5;
                    ws.Column(11).Width = 12;
                    

                    // ===== Save file =====
                    using (SaveFileDialog sfd = new SaveFileDialog())
                    {
                        sfd.Filter = "Excel Workbook|*.xlsx";
                        sfd.FileName = "Gam.xlsx";
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

        void SetHeader(IXLWorksheet ws, int row, int col, string text, int rowSpan = 2, int colSpan = 1, bool bold = true)
        {
            var range = ws.Range(row, col, row + rowSpan - 1, col + colSpan - 1);
            range.Merge();

            var cell = ws.Cell(row, col);
            cell.Value = text;
            cell.Style.Font.Bold = bold;
            cell.Style.Font.FontSize = 11;
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            cell.Style.Alignment.WrapText = true;
            range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            range.Style.Fill.BackgroundColor = XLColor.FromArgb(248, 203, 173);
        }

        void SetSubHeader(IXLWorksheet ws, int row, int col, string text, bool bold = true)
        {
            var cell = ws.Cell(row, col);
            cell.Value = text;
            cell.Style.Font.Bold = bold;
            cell.Style.Alignment.WrapText = true;
            cell.Style.Font.FontSize = 11;
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            cell.Style.Fill.BackgroundColor = XLColor.FromArgb(248, 203, 173);
        }
    }
}
