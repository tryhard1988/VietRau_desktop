using ClosedXML.Excel;
using RauViet.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RauViet.ui
{
    public partial class CustomerDetailPackingTotal : Form
    {
        DataTable mOrdersTotal_dt;
        private LoadingOverlay loadingOverlay;
        int mCurrentExportID = -1;
        public CustomerDetailPackingTotal()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = true;
            

            status_lb.Text = "";


            LuuThayDoiBtn.Click += saveBtn_Click;
            this.KeyDown += CustomerDetailPackingTotal_KeyDown;
        }

        private void CustomerDetailPackingTotal_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                if (mCurrentExportID <= 0)
                {
                    return;
                }

                SQLStore_Kho.Instance.removeCustomerDetailPacking(mCurrentExportID);
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

            try
            {                
                string[] keepColumns = { "ExportCodeID", "ExportCode" , "ExportDate", "ExportCodeIndex" };
                var parameters = new Dictionary<string, object>{ { "Complete", false }};
                var mExportCode_dt = await SQLStore_Kho.Instance.getExportCodesAsync(keepColumns, parameters);

                if (mCurrentExportID <= 0 && mExportCode_dt.Rows.Count > 0)
                {
                    mCurrentExportID = Convert.ToInt32(mExportCode_dt.AsEnumerable()
                                   .Max(r => r.Field<int>("ExportCodeID")));
                }
                mOrdersTotal_dt =await SQLStore_Kho.Instance.GetCustomerDetailPackingAsync(mCurrentExportID);

               

                DataView dv = new DataView(mOrdersTotal_dt);
                dataGV.DataSource = dv;
                dataGV.DataSource = mOrdersTotal_dt;

                dataGV.Columns["ExportCodeID"].Visible = false;
                dataGV.Columns["Amount"].Visible = false;
                dataGV.Columns["packing"].Visible = false;

                dataGV.Columns["CartonNo"].HeaderText = "Carton No";
                dataGV.Columns["ProductNameEN"].HeaderText = "English name";
                dataGV.Columns["ProductNameVN"].HeaderText = "Vietnamese name";
                dataGV.Columns["Package"].HeaderText = "Unit";
                dataGV.Columns["NWReal"].HeaderText = "N.W";
                dataGV.Columns["PCSReal"].HeaderText = "PCS";
                dataGV.Columns["AmountPacking"].HeaderText = "Packing";
                dataGV.Columns["CustomerCarton"].HeaderText = "Note";


                dataGV.Columns["No"].Width = 30;
                dataGV.Columns["ProductNameEN"].Width = 120;
                dataGV.Columns["ProductNameVN"].Width = 160;
                dataGV.Columns["CustomerCarton"].Width = 200;
                dataGV.Columns["CartonNo"].Width = 100;
                dataGV.Columns["PLU"].Width = 50;
                dataGV.Columns["Package"].Width = 50;
                dataGV.Columns["NWReal"].Width = 50;
                dataGV.Columns["AmountPacking"].Width = 50;
                dataGV.Columns["PCSReal"].Width = 50;

                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                exportCode_cbb.SelectedIndexChanged -= exportCode_search_cbb_SelectedIndexChanged;
                exportCode_cbb.DataSource = mExportCode_dt;
                exportCode_cbb.DisplayMember = "ExportCode";  // hiển thị tên
                exportCode_cbb.ValueMember = "ExportCodeID";
                exportCode_cbb.SelectedValue = mCurrentExportID;
                exportCode_cbb.SelectedIndexChanged += exportCode_search_cbb_SelectedIndexChanged;
                

                dataGV.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                dataGV.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

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

        private async void exportCode_search_cbb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!int.TryParse(exportCode_cbb.SelectedValue.ToString(), out int exportCodeId))
                return;

            mCurrentExportID = exportCodeId;

            mOrdersTotal_dt = await SQLStore_Kho.Instance.GetCustomerDetailPackingAsync(mCurrentExportID);
            DataView dv = new DataView(mOrdersTotal_dt);
            dataGV.DataSource = dv;
        }     
        
        private async void saveBtn_Click(object sender, EventArgs e)
        {
            ExportGroupedByCustomer(dataGV);
        }

        private async void ExportGroupedByCustomer(DataGridView dgv)
        {
            if (exportCode_cbb.SelectedItem == null) return;

            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Message = "Đang xử lý ...";
            loadingOverlay.Show();
            await Task.Delay(100);

            string exportCode = ((DataRowView)exportCode_cbb.SelectedItem)["ExportCode"].ToString();
            string exportCodeIndex = ((DataRowView)exportCode_cbb.SelectedItem)["ExportCodeIndex"].ToString();
            DateTime exportDate = Convert.ToDateTime(((DataRowView)exportCode_cbb.SelectedItem)["ExportDate"]);
            string folderPath = "";
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Chọn thư mục để lưu tất cả file Excel";
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    folderPath = fbd.SelectedPath + $"\\MXC {exportCodeIndex} - {exportDate.Day}{exportDate.Month}{exportDate.Year}";
                }
                else
                {
                    await Task.Delay(200);
                    loadingOverlay.Hide();
                    return; // Người dùng bấm Cancel
                }
            }

            // Lấy danh sách FullName duy nhất
            var fullNames = dgv.Rows.Cast<DataGridViewRow>()
                             .Where(r => !r.IsNewRow && r.Cells["FullName"].Value != null)
                             .Select(r => r.Cells["FullName"].Value.ToString())
                             .Distinct()
                             .ToList();

            foreach (var fullName in fullNames)
            {
                // Lọc dữ liệu theo FullName
                var dt = new DataTable();
                foreach (DataGridViewColumn col in dgv.Columns)
                {
                    dt.Columns.Add(col.Name, col.ValueType ?? typeof(string));
                }

                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (row.IsNewRow) continue;
                    if (row.Cells["FullName"].Value?.ToString() == fullName)
                    {
                        var newRow = dt.NewRow();
                        foreach (DataGridViewColumn col in dgv.Columns)
                        {
                            newRow[col.Name] = row.Cells[col.Index].Value ?? DBNull.Value;
                        }
                        dt.Rows.Add(newRow);
                    }
                }

                
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                // Đặt tên file: packingTotal_{exportCode}_{customerName}.xlsx
                string safeCustomerName = string.Join("_", fullName.Split(Path.GetInvalidFileNameChars()));
                string filePath = Path.Combine(folderPath, $"PL-{safeCustomerName}-ETD {exportDate.Day}.{exportDate.Month} {exportCodeIndex}.xlsx");

                // Xuất ra file excel cho khách hàng này
                bool iSuccess = ExportDataTableToExcel(dt, fullName, exportCode, filePath);
                if (!iSuccess)
                {
                    await Task.Delay(200);
                    loadingOverlay.Hide();
                    return;
                }
            }

            MessageBox.Show("Xuất xong tất cả khách hàng!");
            System.Diagnostics.Process.Start("explorer.exe", folderPath);
            await Task.Delay(200);
            loadingOverlay.Hide();
        }

        private bool ExportDataTableToExcel(DataTable dt, string customerName, string exportCode, string filePath)
        {            
            try
            {
                DateTime exportDate = Convert.ToDateTime(((DataRowView)exportCode_cbb.SelectedItem)["ExportDate"]);

                using (var wb = new XLWorkbook())
                {
                    var ws = wb.Worksheets.Add(customerName);

                    ws.Style.Font.FontName = "Arial";
                    ws.Style.Font.FontSize = 9;
                    ws.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    // Danh sách cột muốn xuất theo Name
                    string[] columnsToExport = new string[] { "No", "CartonNo", "ProductNameEN", "ProductNameVN", "PLU", "Package", "NWReal", "AmountPacking", "PCSReal", "CustomerCarton" };
                    var labels = new Dictionary<string, string>
                    {
                        { "No", "No" },
                        { "CartonNo", "Carton No" },
                        { "ProductNameEN", "English name" },
                        { "ProductNameVN", "Vietnamese name" },
                        { "PLU", "PLU" },
                        { "Package", "Unit" },
                        { "NWReal", "N.W" },
                        { "AmountPacking", "Packing" },
                        { "PCSReal", "PCS" },
                        { "CustomerCarton", "Note" }
                    };
                    // Lọc cột hiển thị và có trong danh sách
                    var exportColumns = dt.Columns.Cast<DataColumn>()
                        .Where(c => columnsToExport.Contains(c.ColumnName))
                        .OrderBy(c => Array.IndexOf(columnsToExport, c.ColumnName))
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
                    ws.Cell(3, 1).Value = $"Tel/Fax: {Utils.get_Tele_Fax()}";
                    ws.Cell(3, 1).Style.Font.FontSize = 10;
                    ws.Cell(3, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    ws.Range(3, (exportColumns.Count / 2) + 1, 3, exportColumns.Count).Merge();
                    ws.Cell(3, (exportColumns.Count / 2) + 1).Value = "Email: acc@vietrau.com";
                    ws.Cell(3, (exportColumns.Count / 2) + 1).Style.Font.FontSize = 10;
                    ws.Cell(3, (exportColumns.Count / 2) + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    // Hàng 4: INVOICE
                    ws.Range(4, 1, 4, exportColumns.Count).Merge();
                    ws.Cell(4, 1).Value = "DETAILED PACKING LIST";
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
                    ws.Cell(5, 5).Value = "Packing List No:     " + exportDate.Day.ToString("00") + exportDate.Month.ToString("00") + exportDate.Year + "SQ";
                    ws.Cell(5, 5).Style.Font.FontSize = 10;
                    ws.Cell(5, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Range(5, 5, 5, exportColumns.Count).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    // Hàng 6: Date
                    ws.Range(6, 5, 6, 6).Merge();
                    ws.Cell(6, 5).Value = "Date:    " + exportDate.ToString("dd/MM/yyyy");
                    ws.Cell(6, 5).Style.Font.FontSize = 10;
                    ws.Cell(6, 5).Style.Font.Bold = true;
                    ws.Cell(6, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Range(6, 5, 6, 6).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    ws.Range(6, 5, 6, 6).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    ws.Range(6, 7, 6, exportColumns.Count).Merge();
                    ws.Cell(6, 7).Value = "Reference No:    " + exportCode;
                    ws.Cell(6, 7).Style.Font.FontSize = 10;
                    ws.Cell(6, 7).Style.Font.Bold = true;
                    ws.Cell(6, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Range(6, 7, 6, exportColumns.Count).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    // Hàng 7: Consignee & TOTAL
                    ws.Range(7, 1, 7, 4).Merge();
                    ws.Cell(7, 1).Value = "Consignee:";
                    ws.Cell(7, 1).Style.Font.FontSize = 10;
                    ws.Cell(7, 1).Style.Font.Bold = true;
                    ws.Cell(7, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Range(7, 1, 9, 4).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    ws.Range(7, 5, 7, exportColumns.Count).Merge();
                    ws.Cell(7, 5).Value = customerName;
                    ws.Cell(7, 5).Style.Font.FontSize = 11;
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

                    ws.Range(8, 5, 9, 6).Merge();
                    ws.Cell(8, 5).Value = "Air freight:";
                    ws.Cell(8, 5).Style.Font.FontSize = 10;
                    ws.Cell(8, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Range(8, 5, 9, 6).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    ws.Range(8, 7, 9, exportColumns.Count).Merge();
                    ws.Cell(8, 7).Value = "FREIGHT PREPAID";
                    ws.Cell(8, 7).Style.Font.FontSize = 10;
                    ws.Cell(8, 7).Style.Font.Bold = true;
                    ws.Cell(8, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Range(8, 7, 9, exportColumns.Count).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

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
                        if (col.ColumnName == "ProductNameEN" || col.ColumnName == "ProductNameVN" || col.ColumnName == "PLU")
                        {
                            // Gộp 3 cột con cho "Name of Goods" ở hàng 5
                            if (colIndex == exportColumns.FindIndex(c => c.ColumnName == "ProductNameEN") + 1)
                            {
                                var range = ws.Range(headerStartRow, colIndex, headerStartRow, colIndex + 2);
                                range.Merge();
                                ws.Cell(headerStartRow, colIndex).Value = "Name of Goods";
                                ws.Cell(headerStartRow, colIndex).Style.Font.Bold = true;
                                ws.Cell(headerStartRow, colIndex).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Đóng khung
                            }

                            // Header con ở hàng 6
                            var cell = ws.Cell(headerStartRow + 1, colIndex);
                            cell.Value = labels[col.ColumnName];
                            cell.Style.Font.Bold = true;
                            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Đóng khung

                            colIndex++;
                        }
                        else if (col.ColumnName == "Package" || col.ColumnName == "NWReal")
                        {
                            // Gộp 2 cột con cho "Unit Price" ở hàng 5
                            if (colIndex == exportColumns.FindIndex(c => c.ColumnName == "Package") + 1)
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
                            cell.Value = labels[col.ColumnName];
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
                            ws.Cell(headerStartRow, colIndex).Value = labels[col.ColumnName];
                            ws.Cell(headerStartRow, colIndex).Style.Font.Bold = true;
                            ws.Cell(headerStartRow, colIndex).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Đóng khung

                            colIndex++;
                        }
                    }

                    int excelColIndex = 1;
                    foreach (var col in exportColumns)
                    {
                        var column = ws.Column(excelColIndex);
                        column.Width = 2; // đặt chiều rộng cố định (quan trọng để wrap text có tác dụng)
                        column.Style.Alignment.WrapText = true;
                        excelColIndex++;

                    }
                    // ===== Data bắt đầu từ hàng 7 =====
                    decimal totalNWOther = 0; // Biến lưu tổng                    

                    // excelColIndex = 1;
                    int stt = 1;
                    int rowIndex = headerStartRow + 2;
                    List<int> CartonNoList = new List<int>();
                    foreach (DataRow row in dt.Rows)
                    {
                        colIndex = 1;
                        foreach (var col in exportColumns)
                        {
                            var cell = ws.Cell(rowIndex, colIndex);
                            var cellValue = row[col.ColumnName]?.ToString() ?? "";
                            cell.Value = cellValue;
                            // Bật wrap text
                            cell.Style.Alignment.WrapText = true;

                            if (col.ColumnName == "Package" && cellValue.CompareTo("weight") == 0)
                            {
                                cell.Value = "kg";
                            }

                            // Căn phải các cột số
                            if (col.DataType == typeof(int) || col.DataType == typeof(decimal))
                                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                            else
                                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                            // Thêm border
                            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                            // Tính tổng cột NWOther
                            if (col.ColumnName == "NWReal")
                            {
                                cell.Style.NumberFormat.Format = "#,##0.00";
                                if (decimal.TryParse(cellValue, out decimal num))
                                {
                                    cell.Value = num;
                                    totalNWOther += num;
                                }
                            }
                            else if (col.ColumnName == "PCSReal")
                            {
                                cell.Style.NumberFormat.Format = "#,##0";
                                if (int.TryParse(cellValue, out int num))
                                {
                                    cell.Value = num;
                                }
                            }
                            else if (col.ColumnName == "No")
                            {
                                cell.Value = stt++;
                            }

                            if (col.ColumnName == "PCSReal" || col.ColumnName == "Package" || col.ColumnName == "No" || col.ColumnName == "NWReal" || col.ColumnName == "AmountPacking")
                            {
                                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            }
                            else if (col.ColumnName == "CustomerCarton" || col.ColumnName == "CartonNo")
                            {
                                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                double columnWidth = ws.Column(colIndex).Width;
                                double avgCharWidth = 0.5;
                                double estimatedLines = Math.Ceiling((cellValue.Length * avgCharWidth) / columnWidth);
                               
                                estimatedLines += 1;
                                
                                ws.Row(rowIndex).Height = estimatedLines * (ws.Style.Font.FontSize) * 1.3; // lineHeight ~ font size * 1.2

                                if (col.ColumnName == "CartonNo" && cellValue.CompareTo("") != 0)
                                {
                                    string cellValue1 = Convert.ToString(cellValue).Replace('\u00A0', ' ');

                                    int[] numbers = (cellValue1 ?? "")
                                         // bỏ khoảng trắng đặc biệt (từ Excel)
                                        .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                        .Select(s => {
                                            int n;
                                            return int.TryParse(s.Trim(), out n) ? n : (int?)null;
                                        })
                                        .Where(n => n.HasValue)
                                        .Select(n => n.Value)
                                        .ToArray();


                                    CartonNoList.AddRange(numbers);
                                }
                            }
                            colIndex++;
                        }

                        rowIndex++;
                       
                    }
                    ws.Columns().AdjustToContents();
                    ws.Column(1).Width = 3;
                    ws.Column(2).Width = 6;
                    ws.Column(3).Width = 18;
                    ws.Column(4).Width = 18;
                    ws.Column(5).Width = 6.5;
                    ws.Column(6).Width = 6.5;
                    ws.Column(7).Width = 5;
                    ws.Column(8).Width = 7.5;
                    ws.Column(9).Width = 5;
                    ws.Column(10).Width = 15;



                    ws.Row(1).Height = 30;
                    ws.Row(4).Height = 17;
                    ws.Row(5).Height = 20;
                    ws.Row(6).Height = 45;
                    ws.Row(9).Height = 25;
                    ws.Row(11).Height = 14;
                    ws.Row(12).Height = 14;


                    int rowIndex1 = 1;
                    foreach (var row in ws.RowsUsed())
                    {
                        rowIndex1++;
                        if (rowIndex1 < 13)
                            continue;

                        double maxLine = 1;

                        foreach (var cell in row.CellsUsed())
                        {
                            string value = cell.GetValue<string>();
                            int colWidth = (int)cell.WorksheetColumn().Width;

                            int lines = EstimateLineCount(value, colWidth);
                            if (lines > maxLine) maxLine = lines;
                        }

                        // 15 = chiều cao chuẩn, nhân line để tăng
                        row.Height = maxLine * 16;
                    }

                    int totalCarton = CartonNoList.Distinct().Count();
                    // Ghi tổng xuống Excel, ví dụ ở cột NWOther, hàng tiếp theo
                    int totalRow = rowIndex;

                    ws.Range(totalRow, 1, totalRow, 3).Merge();
                    ws.Cell(totalRow, 1).Value = "Total NW:";
                    ws.Cell(totalRow, 1).Style.Font.Bold = true;
                    ws.Cell(totalRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Range(totalRow, 1, totalRow, 3).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    ws.Range(totalRow + 1, 1, totalRow + 1, 3).Merge();
                    ws.Cell(totalRow + 1, 1).Value = "Total Carton:";
                    ws.Cell(totalRow + 1, 1).Style.Font.Bold = true;
                    ws.Cell(totalRow + 1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Range(totalRow + 1, 1, totalRow + 1, 3).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    ws.Range(totalRow, 4, totalRow, exportColumns.Count).Merge();
                    ws.Cell(totalRow, 4).Value = totalNWOther + " kg";
                    ws.Cell(totalRow, 4).Style.Font.Bold = true;
                    ws.Cell(totalRow, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Range(totalRow, 4, totalRow, exportColumns.Count).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    ws.Range(totalRow + 1, 4, totalRow + 1, exportColumns.Count).Merge();
                    ws.Cell(totalRow + 1, 4).Value = totalCarton + " CTNS";
                    ws.Cell(totalRow + 1, 4).Style.Font.Bold = true;
                    ws.Cell(totalRow + 1, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Range(totalRow + 1, 4, totalRow + 1, exportColumns.Count).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;


                    wb.SaveAs(filePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi xuất Excel: " + ex.Message);
                MessageBox.Show("Lỗi khi xuất Excel: " + ex.Message);
                return false;
            }

            return true;
        }

        private int EstimateLineCount(string text, int columnWidth)
        {
            if (string.IsNullOrEmpty(text)) return 1;

            // hệ số 1.1 để bù trừ spacing
            int maxCharsPerLine = (int)(columnWidth * 1.1);

            int totalLength = text.Length;
            int lines = (int)Math.Ceiling((double)totalLength / maxCharsPerLine);

            // xử lý xuống dòng sẵn có
            lines += text.Count(c => c == '\n');

            return lines;
        }
    }
}
