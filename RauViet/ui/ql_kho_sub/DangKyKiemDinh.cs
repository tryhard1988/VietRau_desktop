using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using RauViet.classes;
using ClosedXML.Excel;

namespace RauViet.ui
{
    public partial class DangKyKiemDinh : Form
    {
        DataTable mExportCode_dt, mOrdersTotal_dt;
        public DangKyKiemDinh()
        {
            InitializeComponent();

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            status_lb.Text = "";
            loading_lb.Text = "Đang tải dữ liệu, vui lòng chờ...";
            loading_lb.Visible = false;


            LuuThayDoiBtn.Click += saveBtn_Click;           
            dataGV.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.dataGV_RowPrePaint);
            //dataGV.CellEndEdit += dataGV_CellEndEdit;

            exportCode_cbb.SelectedIndexChanged += exportCode_search_cbb_SelectedIndexChanged;
        }

        

        public async void ShowData()
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            loading_lb.Visible = true;            

            try
            {
                var ordersPackingTask = SQLManager.Instance.getOrdersDKKDAsync();
                var exportCodeTask = SQLManager.Instance.getExportCodes_Incomplete();

                await Task.WhenAll(ordersPackingTask, exportCodeTask);

                mExportCode_dt = exportCodeTask.Result;
                mOrdersTotal_dt = ordersPackingTask.Result;

                mOrdersTotal_dt.Columns.Add(new DataColumn("No", typeof(string)));

                int count = 1;
                foreach (DataRow dr in mOrdersTotal_dt.Rows)
                {
                    dr["No"] = count++;

                }

                mOrdersTotal_dt.Columns.Add(new DataColumn("Packing", typeof(string)));
                mOrdersTotal_dt.Columns.Add(new DataColumn("PCS", typeof(string)));
                mOrdersTotal_dt.Columns.Add(new DataColumn("PriceCHF", typeof(string)));
                mOrdersTotal_dt.Columns.Add(new DataColumn("AmountCHF", typeof(string)));

                count = 0;
                mOrdersTotal_dt.Columns["No"].SetOrdinal(count++);
                mOrdersTotal_dt.Columns["ProductNameEN"].SetOrdinal(count++);
                mOrdersTotal_dt.Columns["ProductNameVN"].SetOrdinal(count++);
                mOrdersTotal_dt.Columns["BotanicalName"].SetOrdinal(count++);
                mOrdersTotal_dt.Columns["NWOther"].SetOrdinal(count++);
                mOrdersTotal_dt.Columns["Packing"].SetOrdinal(count++);
                mOrdersTotal_dt.Columns["PCS"].SetOrdinal(count++);
                mOrdersTotal_dt.Columns["PriceCHF"].SetOrdinal(count++);
                mOrdersTotal_dt.Columns["AmountCHF"].SetOrdinal(count++);
                dataGV.DataSource = mOrdersTotal_dt;

                dataGV.Columns["ExportCodeID"].Visible = false;

                dataGV.Columns["ProductNameEN"].HeaderText = "English name";
                dataGV.Columns["ProductNameVN"].HeaderText = "Vietnamese name";
                dataGV.Columns["BotanicalName"].HeaderText = "Botanical name";
                dataGV.Columns["NWOther"].HeaderText = "N.W(kg)";
                dataGV.Columns["PriceCHF"].HeaderText = "Price (CHF)";
                dataGV.Columns["AmountCHF"].HeaderText = "Amount (CHF)";
                dataGV.Columns["No"].HeaderText = "No";

                dataGV.Columns["Priority"].HeaderText = "Ưu\nTiên";

                dataGV.Columns["Priority"].Width = 30;
                dataGV.Columns["No"].Width = 30;
                // dataGV.Columns["STT"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGV.Columns["ProductNameEN"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGV.Columns["ProductNameVN"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGV.Columns["BotanicalName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGV.Columns["NWOther"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGV.Columns["Priority"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGV.Columns["ProductNameVN"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                dataGV.Columns["Priority"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dataGV.Columns["NWOther"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                exportCode_cbb.DataSource = mExportCode_dt;
                exportCode_cbb.DisplayMember = "ExportCode";  // hiển thị tên
                exportCode_cbb.ValueMember = "ExportCodeID";
                
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
            if (mOrdersTotal_dt == null || mExportCode_dt.Rows.Count == 0)
                return;

            string selectedExportCode = ((DataRowView)exportCode_cbb.SelectedItem)["ExportCodeID"].ToString();

            if (!string.IsNullOrEmpty(selectedExportCode))
            {
                // Tạo DataView để filter
                DataView dv = new DataView(mOrdersTotal_dt);
                dv.RowFilter = $"ExportCodeID = '{selectedExportCode}'";

                // Gán lại cho DataGridView
                dataGV.DataSource = dv;
            }
            else
            {
                // Nếu chưa chọn gì thì hiển thị toàn bộ
                dataGV.DataSource = mOrdersTotal_dt;
            }
        }

        private void dataGV_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (e.RowIndex % 2 == 0)
            {
                dataGV.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Beige;
            }
        }        
        
        private async void saveBtn_Click(object sender, EventArgs e)
        {
            ExportDataGridViewToExcel(dataGV);
        }



        private void ExportDataGridViewToExcel(DataGridView dgv)
        {
            string exportCode = ((DataRowView)exportCode_cbb.SelectedItem)["ExportCode"].ToString();
            DateTime exportDate = Convert.ToDateTime(((DataRowView)exportCode_cbb.SelectedItem)["ExportDate"]);
            try
            {
                using (var wb = new XLWorkbook())
                {
                    var ws = wb.Worksheets.Add("Data");
                    ws.Style.Font.FontName = "Arial";
                    ws.Style.Font.FontSize = 9;
                    ws.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    // Danh sách cột muốn xuất theo Name
                    string[] columnsToExport = new string[] { "No", "ProductNameEN", "ProductNameVN", "BotanicalName", "NWOther", "Packing", "PCS", "PriceCHF", "AmountCHF" };

                    // Lọc cột hiển thị và có trong danh sách
                    var exportColumns = dgv.Columns.Cast<DataGridViewColumn>()
                                            .Where(c => c.Visible && columnsToExport.Contains(c.Name))
                                            .OrderBy(c => Array.IndexOf(columnsToExport, c.Name))
                                            .ToList();

                    

                    // Hàng 1: Tên công ty
                    ws.Range(1, 1, 1, exportColumns.Count).Merge();
                    ws.Cell(1, 1).Value = "VIET RAU JOINT STOCK COMPANY";
                    ws.Cell(1, 1).Style.Font.Bold = true;
                    ws.Cell(1, 1).Style.Font.FontSize = 12;
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
                    ws.Cell(5, 5).Value = "Invoice No:";
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

                    ws.Range(6, 7, 6, 9).Merge();
                    ws.Cell(6, 7).Value = "Reference No:    " + exportCode;
                    ws.Cell(6, 7).Style.Font.FontSize = 10;
                    ws.Cell(6, 7).Style.Font.Bold = true;
                    ws.Cell(6, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Range(6, 7, 6, 9).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    // Hàng 7: Consignee & TOTAL
                    ws.Range(7, 1, 7, 4).Merge();
                    ws.Cell(7, 1).Value = "Consignee:";
                    ws.Cell(7, 1).Style.Font.FontSize = 10;
                    ws.Cell(7, 1).Style.Font.Bold = true;
                    ws.Cell(7, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Range(7, 1, 7, 4).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

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
                    ws.Range(8, 1, 8, 4).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    ws.Range(9, 1, 9, 4).Merge();
                    ws.Cell(9, 1).Value = "Schwamendingenstrasse 10, 8050 Zürich,\r\nSwitzerland";
                    ws.Cell(9, 1).Style.Font.FontSize = 10;
                    ws.Cell(9, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Range(9, 1, 9, 4).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

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
                        if (col.Name == "ProductNameEN" || col.Name == "ProductNameVN" || col.Name == "BotanicalName")
                        {
                            // Gộp 3 cột con cho "Name of Goods" ở hàng 5
                            if (colIndex == exportColumns.FindIndex(c => c.Name == "ProductNameEN") + 1)
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
                            cell.Value = col.HeaderText;
                            cell.Style.Font.Bold = true;
                            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Đóng khung

                            colIndex++;
                        }
                        else if (col.Name == "PriceCHF" || col.Name == "AmountCHF")
                        {
                            // Gộp 2 cột con cho "Unit Price" ở hàng 5
                            if (colIndex == exportColumns.FindIndex(c => c.Name == "PriceCHF") + 1)
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
                    decimal totalNWOther = 0; // Biến lưu tổng

                    int rowIndex = headerStartRow + 2;
                    foreach (DataGridViewRow row in dgv.Rows)
                    {
                        if (row.IsNewRow) continue;

                        // Lấy giá trị PlantingAreaCode
                        var plantingCode = row.Cells["PlantingAreaCode"].Value?.ToString();
                        bool highlight = !string.IsNullOrWhiteSpace(plantingCode);

                        colIndex = 1;
                        foreach (var col in exportColumns)
                        {
                            var cell = ws.Cell(rowIndex, colIndex);
                            var cellValue = row.Cells[col.Index].Value?.ToString() ?? "";
                            cell.Value = cellValue;

                            // Căn phải các cột số
                            if (col.ValueType == typeof(int) || col.ValueType == typeof(decimal))
                                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                            else
                                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                            // Thêm border
                            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                            // Tính tổng cột NWOther
                            if (col.Name == "NWOther")
                            {
                                if (decimal.TryParse(cellValue, out decimal num))
                                    totalNWOther += num;
                            }

                            // Nếu cần tô màu nền
                            if (highlight &&
                                (col.Name == "ProductNameEN" || col.Name == "ProductNameVN" || col.Name == "BotanicalName"))
                            {
                                cell.Style.Fill.BackgroundColor = XLColor.Yellow; // Màu tùy chỉnh
                            }

                            colIndex++;
                        }
                        rowIndex++;
                    }


                    // Ghi tổng xuống Excel, ví dụ ở cột NWOther, hàng tiếp theo
                    int totalRow = rowIndex;
                    int nwOtherColIndex = exportColumns.FindIndex(c => c.Name == "NWOther") + 1;

                    ws.Range(totalRow, 1, totalRow, nwOtherColIndex - 1).Merge();
                    ws.Cell(totalRow, 1).Value = "TOTAL";
                    ws.Cell(totalRow, 1).Style.Font.Bold = true;
                    ws.Cell(totalRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Range(totalRow, 1, totalRow, nwOtherColIndex - 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    ws.Cell(totalRow, nwOtherColIndex).Value = totalNWOther;
                    ws.Cell(totalRow, nwOtherColIndex).Style.Font.Bold = true;
                    ws.Cell(totalRow, nwOtherColIndex).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell(totalRow, nwOtherColIndex).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;



                    ws.Columns().AdjustToContents();
                    ws.Column(1).Width = 3;
                    ws.Column(2).Width += 3;
                    ws.Column(3).Width += 3;
                    ws.Column(4).Width += 3;
                    ws.Row(5).Height = 20;
                    ws.Row(6).Height = 45;
                    ws.Row(9).Height = 25;

                    ws.Row(11).Height += 5;
                    ws.Row(12).Height += 5;
                    // ===== Save file =====
                    using (SaveFileDialog sfd = new SaveFileDialog())
                    {
                        sfd.Filter = "Excel Workbook|*.xlsx";
                        sfd.FileName = "DKKD_" + exportCode + ".xlsx";
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



    }
}
