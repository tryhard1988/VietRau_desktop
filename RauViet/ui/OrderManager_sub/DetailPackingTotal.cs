using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using RauViet.classes;
using ClosedXML.Excel;

namespace RauViet.ui
{
    public partial class DetailPackingTotal : Form
    {
        private LoadingOverlay loadingOverlay;
        DataTable mExportCode_dt, mOrdersTotal_dt;
        int mCurrentExportID = -1;
        public DetailPackingTotal()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = true;
            

            status_lb.Text = "";


            LuuThayDoiBtn.Click += saveBtn_Click;
            this.KeyDown += DetailPackingTotal_KeyDown;
        }

        private void DetailPackingTotal_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                if (mCurrentExportID <= 0)
                {
                    return;
                }

                SQLStore_Kho.Instance.removeDetailPackingTotal(mCurrentExportID);
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
                string[] keepColumns = { "ExportCodeID", "ExportCode" , "ExportDate", "ExportCodeIndex" };
                var parameters = new Dictionary<string, object> { { "Complete", false } };
                mExportCode_dt = await SQLStore_Kho.Instance.getExportCodesAsync(keepColumns, parameters);
                if (mCurrentExportID <= 0 && mExportCode_dt.Rows.Count > 0)
                {
                    mCurrentExportID = Convert.ToInt32(mExportCode_dt.AsEnumerable()
                                   .Max(r => r.Field<int>("ExportCodeID")));
                }

                mOrdersTotal_dt = await SQLStore_Kho.Instance.GetDetailPackingTotalAsync(mCurrentExportID);
                DataView dv = new DataView(mOrdersTotal_dt);
                dataGV.DataSource = dv;

                dataGV.Columns["ExportCodeID"].Visible = false;


                dataGV.Columns["CartonNo"].HeaderText = "Carton No";
                dataGV.Columns["ProductNameEN"].HeaderText = "English name";
                dataGV.Columns["ProductNameVN"].HeaderText = "Vietnamese name";
                dataGV.Columns["LOTCodeComplete"].HeaderText = "LOT";
                dataGV.Columns["Package"].HeaderText = "Unit";
                dataGV.Columns["NWReal"].HeaderText = "N.W";
                dataGV.Columns["PCSReal"].HeaderText = "PCS";
                dataGV.Columns["packing"].HeaderText = "Packing";
                dataGV.Columns["CustomerCarton"].HeaderText = "Mask No";

                dataGV.Columns["Priority"].HeaderText = "Ưu\nTiên";

                dataGV.Columns["Priority"].Width = 30;
                dataGV.Columns["No"].Width = 30;
                dataGV.Columns["CustomerCarton"].Width = 200;
                dataGV.Columns["CartonNo"].Width = 120;
                dataGV.Columns["ProductNameEN"].Width = 120; ;
                dataGV.Columns["ProductNameVN"].Width = 150;
                dataGV.Columns["LOTCodeComplete"].Width = 80;
                dataGV.Columns["PLU"].Width = 50;
                dataGV.Columns["Package"].Width = 50;
                dataGV.Columns["NWReal"].Width = 60;
                dataGV.Columns["packing"].Width = 70;
                dataGV.Columns["PCSReal"].Width = 50;

                dataGV.Columns["Priority"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

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

            mOrdersTotal_dt = await SQLStore_Kho.Instance.GetDetailPackingTotalAsync(mCurrentExportID);
            DataView dv = new DataView(mOrdersTotal_dt);
            dataGV.DataSource = dv;
        }
     
        private async void saveBtn_Click(object sender, EventArgs e)
        {
            ExportDataGridViewToExcel(dataGV);
        }

        private async void ExportDataGridViewToExcel(DataGridView dgv)
        {
            if (exportCode_cbb.SelectedItem == null) return;

            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Message = "Đang xử lý ...";
            loadingOverlay.Show();
            await Task.Delay(100);
            string exportCode = ((DataRowView)exportCode_cbb.SelectedItem)["ExportCode"].ToString();
            DateTime exportDate = Convert.ToDateTime(((DataRowView)exportCode_cbb.SelectedItem)["ExportDate"]);
            int exportCodeIndex = Convert.ToInt32(((DataRowView)exportCode_cbb.SelectedItem)["ExportCodeIndex"]);
            try
            {
                using (var wb = new XLWorkbook())
                {
                    var ws = wb.Worksheets.Add("Data");
                    ws.Style.Font.FontName = "Arial";
                    ws.Style.Font.FontSize = 9;
                    ws.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    // Danh sách cột muốn xuất theo Name
                    string[] columnsToExport = new string[] { "No", "CartonNo", "ProductNameEN", "ProductNameVN", "LOTCodeComplete", "PLU", "Package", "NWReal", "packing", "PCSReal", "CustomerCarton" };

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
                    ws.Cell(4, 1).Value = "DETAILED PACKING LIST";
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
                    ws.Cell(5, 5).Value = "Packing List No:     " + exportDate.Day+ exportDate.Month+ exportDate.Year + "SQ";
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
                        if (col.Name == "ProductNameEN" || col.Name == "ProductNameVN" || col.Name == "LOTCodeComplete" || col.Name == "PLU")
                        {
                            // Gộp 3 cột con cho "Name of Goods" ở hàng 5
                            if (colIndex == exportColumns.FindIndex(c => c.Name == "ProductNameEN") + 1)
                            {
                                var range = ws.Range(headerStartRow, colIndex, headerStartRow, colIndex + 3);
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
                        else if (col.Name == "Package" || col.Name == "NWReal")
                        {
                            // Gộp 2 cột con cho "Unit Price" ở hàng 5
                            if (colIndex == exportColumns.FindIndex(c => c.Name == "Package") + 1)
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

                    foreach (var col in exportColumns)
                    {
                        var column = ws.Column(col.Index + 1);
                        column.Width = 20; // đặt chiều rộng cố định (quan trọng để wrap text có tác dụng)
                        column.Style.Alignment.WrapText = true;
                    }

                   

                    // ===== Data bắt đầu từ hàng 7 =====
                    decimal totalNWOther = 0; // Biến lưu tổng
                    ws.Column(11).Width = 30;
                    ws.Column(2).Width = 15;

                    List<int> cartonNoList = new List<int>();
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

                            // Bật wrap text
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

                            // Thêm border
                            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                            // Tính tổng cột NWOther
                            if (col.Name == "NWReal")
                            {
                                cell.Style.NumberFormat.Format = "#,##0.00";
                                if (decimal.TryParse(cellValue, out decimal num))
                                {
                                    totalNWOther += num;
                                    cell.Value = num;
                                }
                            }
                            else if (col.Name == "PCSReal")
                            {
                                cell.Style.NumberFormat.Format = "#,##0";
                                if (int.TryParse(cellValue, out int num))
                                {
                                    cell.Value = num;
                                }
                            }

                            if (col.Name == "PCSReal" || col.Name == "Package" || col.Name == "No" || col.Name == "NWReal")
                            {
                                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            }
                            else if (col.Name == "CustomerCarton" || col.Name == "CartonNo")
                            {
                                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                double columnWidth = col.Width;
                                int estimatedLines = (int)((cellValue.Length * ws.Style.Font.FontSize / columnWidth) + 2);
                                ws.Row(rowIndex).Height = estimatedLines * ws.Style.Font.FontSize * 1.2; // lineHeight ~ font size * 1.2

                                if (col.Name == "CartonNo" && cellValue.CompareTo("") != 0)
                                {
                                    string[] parts = cellValue.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                                    foreach (string part in parts)
                                    {
                                        if (int.TryParse(part.Trim(), out int number))
                                        {
                                            if (!cartonNoList.Contains(number))
                                            {
                                                cartonNoList.Add(number);
                                            }
                                        }
                                    }
                                }
                            }
                            colIndex++;
                        }

                        // Tự động điều chỉnh chiều cao hàng
                       // ws.Row(rowIndex).Height = 0;
                       // ws.Row(rowIndex).AdjustToContents();

                        rowIndex++;
                    }

                    ws.Columns().AdjustToContents();
                    ws.Column(1).Width = 4;
                    ws.Column(2).Width = 12;
                    ws.Column(3).Width = 20;
                    ws.Column(4).Width = 20;
                    ws.Column(5).Width = 11;
                    ws.Column(6).Width = 6;
                    ws.Column(7).Width = 6.5;
                    ws.Column(8).Width = 8;
                    ws.Column(9).Width = 7;
                    ws.Column(10).Width = 5.3;
                    ws.Column(11).Width = 25;

                    ws.Row(1).Height += 5;
                    ws.Row(4).Height += 5;
                    ws.Row(5).Height = 20;
                    ws.Row(6).Height = 45;
                    ws.Row(9).Height = 25;
                    ws.Row(11).Height += 5;
                    ws.Row(12).Height += 5;

                    // Ghi tổng xuống Excel, ví dụ ở cột NWOther, hàng tiếp theo
                    int totalRow = rowIndex;

                    ws.Range(totalRow, 1, totalRow, 3).Merge();
                    ws.Cell(totalRow, 1).Value = "Total NW:";
                    ws.Cell(totalRow, 1).Style.Font.Bold = true;
                    ws.Cell(totalRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Range(totalRow, 1, totalRow, 3).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    ws.Range(totalRow +1, 1, totalRow + 1, 3).Merge();
                    ws.Cell(totalRow + 1, 1).Value = "Total Carton:";
                    ws.Cell(totalRow + 1, 1).Style.Font.Bold = true;
                    ws.Cell(totalRow + 1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Range(totalRow + 1, 1, totalRow + 1, 3).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    ws.Range(totalRow, 4, totalRow, exportColumns.Count).Merge();
                    ws.Cell(totalRow, 4).Style.NumberFormat.Format = "#,##0.00 \"kg\"";
                    ws.Cell(totalRow, 4).Value = totalNWOther;
                    ws.Cell(totalRow, 4).Style.Font.Bold = true;
                    ws.Cell(totalRow, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Range(totalRow, 4, totalRow, exportColumns.Count).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    ws.Range(totalRow + 1, 4, totalRow + 1, exportColumns.Count).Merge();
                    ws.Cell(totalRow + 1, 4).Style.NumberFormat.Format = "#,##0 \"CTNS\"";
                    ws.Cell(totalRow + 1, 4).Value = cartonNoList.Count;
                    ws.Cell(totalRow + 1, 4).Style.Font.Bold = true;
                    ws.Cell(totalRow + 1, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Range(totalRow + 1, 4, totalRow + 1, exportColumns.Count).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    // ===== Save file =====
                    using (SaveFileDialog sfd = new SaveFileDialog())
                    {
                        sfd.Filter = "Excel Workbook|*.xlsx";
                        sfd.FileName = $"DETAILED PACKING LIST Total ETD {exportDate.Day}.{exportDate.Month} {exportCodeIndex}.xlsx";
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
    }
}
