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
    public partial class ChotPhyto : Form
    {
        DataTable mExportCode_dt, mOrdersTotal_dt;
        private LoadingOverlay loadingOverlay;
        int mCurrentExportID = -1;
        public ChotPhyto()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            dataGV_DK.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV_DK.MultiSelect = true;

            status_lb.Text = "";


            LuuThayDoiBtn.Click += saveBtn_Click;
            this.KeyDown += ChotPhyto_KeyDown;
        }

        private void ChotPhyto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                if (mCurrentExportID <= 0)
                {
                    return;
                }

                SQLStore_Kho.Instance.removeOrdersChotPhyto(mCurrentExportID);
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
                string[] keepColumns = { "ExportCodeID", "ExportCodeIndex", "ExportCode", "ExportDate" };
                var parameters = new Dictionary<string, object>{{ "Complete", false }};
                mExportCode_dt = await SQLStore_Kho.Instance.getExportCodesAsync(keepColumns, parameters);
                if (mCurrentExportID <= 0 && mExportCode_dt.Rows.Count > 0)
                {
                    mCurrentExportID = Convert.ToInt32(mExportCode_dt.AsEnumerable()
                                   .Max(r => r.Field<int>("ExportCodeID")));
                }
                mOrdersTotal_dt = await SQLStore_Kho.Instance.getOrdersChotPhytosync(mCurrentExportID);
                DataView dv = new DataView(mOrdersTotal_dt);
                dataGV_DK.DataSource = dv;

                dataGV_DK.Columns["ExportCodeID"].Visible = false;
                dataGV_DK.Columns["PlantingAreaCode"].Visible = false;

                dataGV_DK.Columns["ProductNameEN"].HeaderText = "English name";
                dataGV_DK.Columns["ProductNameVN"].HeaderText = "Vietnamese name";
                dataGV_DK.Columns["BotanicalName"].HeaderText = "Botanical name";
                dataGV_DK.Columns["TotalNetWeight"].HeaderText = "N.W(kg)";
                dataGV_DK.Columns["PriceCHF"].HeaderText = "Price (CHF)";
                dataGV_DK.Columns["AmountCHF"].HeaderText = "Amount (CHF)";
                dataGV_DK.Columns["No"].HeaderText = "No";

                dataGV_DK.Columns["Priority"].HeaderText = "Ưu\nTiên";

                dataGV_DK.Columns["Priority"].Width = 50;
                dataGV_DK.Columns["No"].Width = 50;
                // dataGV.Columns["STT"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGV_DK.Columns["ProductNameEN"].Width = 150;
                dataGV_DK.Columns["ProductNameVN"].Width = 150;
                dataGV_DK.Columns["BotanicalName"].Width = 130;
                dataGV_DK.Columns["TotalNetWeight"].Width = 100;
                dataGV_DK.Columns["Packing"].Width = 80;
                dataGV_DK.Columns["PCS"].Width = 60;
                dataGV_DK.Columns["PriceCHF"].Width = 80;
                dataGV_DK.Columns["AmountCHF"].Width = 80;

                dataGV_DK.Columns["Priority"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dataGV_DK.Columns["TotalNetWeight"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGV_DK.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                exportCode_cbb.SelectedIndexChanged -= exportCode_search_cbb_SelectedIndexChanged;
                exportCode_cbb.DataSource = mExportCode_dt;
                exportCode_cbb.DisplayMember = "ExportCode";  // hiển thị tên
                exportCode_cbb.ValueMember = "ExportCodeID";
                exportCode_cbb.SelectedValue = mCurrentExportID;
                exportCode_cbb.SelectedIndexChanged += exportCode_search_cbb_SelectedIndexChanged;

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

            mOrdersTotal_dt = await SQLStore_Kho.Instance.getOrdersChotPhytosync(mCurrentExportID);
            DataView dv = new DataView(mOrdersTotal_dt);
            dataGV_DK.DataSource = dv;
        }
      
        
        private async void saveBtn_Click(object sender, EventArgs e)
        {
            ExportDataGridViewToExcel(dataGV_DK);
        }



        private async void ExportDataGridViewToExcel(DataGridView dgv)
        {
            if (exportCode_cbb.SelectedItem == null) return;

            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Message = "Đang xử lý ...";
            loadingOverlay.Show();
            await Task.Delay(100);

            DataView currentView = (DataView)dgv.DataSource;
            currentView.Sort = "Priority ASC";

            string exportCode = ((DataRowView)exportCode_cbb.SelectedItem)["ExportCode"].ToString();
            int exportindex= Convert.ToInt32(((DataRowView)exportCode_cbb.SelectedItem)["ExportCodeIndex"]);
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
                    string[] columnsToExport = new string[] { "No", "ProductNameEN", "ProductNameVN", "BotanicalName", "TotalNetWeight", "Packing", "PCS", "PriceCHF", "AmountCHF" };

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
                    ws.Cell(6, 5).Value = "Date: " + exportDate.ToString("dd/MM/yyyy");
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
                        var priority = row.Cells["Priority"].Value?.ToString();
                        bool highlight = !string.IsNullOrWhiteSpace(plantingCode);

                        if (priority.CompareTo("200000") == 0)
                            continue;

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
                            if (col.Name == "TotalNetWeight")
                            {
                                cell.Style.NumberFormat.Format = "#,##0.00";
                                if (decimal.TryParse(cellValue, out decimal num))
                                {
                                    totalNWOther += num;
                                    cell.Value = num;
                                }
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
                    int nwOtherColIndex = exportColumns.FindIndex(c => c.Name == "TotalNetWeight") + 1;

                    ws.Range(totalRow, 1, totalRow, nwOtherColIndex - 1).Merge();
                    ws.Cell(totalRow, 1).Value = "TOTAL";
                    ws.Cell(totalRow, 1).Style.Font.Bold = true;
                    ws.Cell(totalRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Range(totalRow, 1, totalRow, nwOtherColIndex - 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    ws.Cell(totalRow, nwOtherColIndex).Style.NumberFormat.Format = "#,##0.00";
                    ws.Cell(totalRow, nwOtherColIndex).Value = totalNWOther;
                    ws.Cell(totalRow, nwOtherColIndex).Style.Font.Bold = true;
                    ws.Cell(totalRow, nwOtherColIndex).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell(totalRow, nwOtherColIndex).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    ws.Columns().AdjustToContents();
                    ws.Column(1).Width = 3;
                    ws.Column(2).Width = 16;
                    ws.Column(3).Width = 20;
                    ws.Column(4).Width = 24;
                    ws.Column(6).Width = 7;
                    ws.Column(7).Width = 4;
                    ws.Row(1).Height += 1;
                    ws.Row(4).Height += 1;
                    ws.Row(5).Height = 20;
                    ws.Row(6).Height = 45;
                    ws.Row(9).Height = 25;

                    ws.Row(11).Height += 5;
                    ws.Row(12).Height += 5;

                    foreach (var col in exportColumns)
                    {
                        var column = ws.Column(col.Index + 1);
                        column.Style.Alignment.WrapText = true;
                    }
                    // ===== Save file =====
                    using (SaveFileDialog sfd = new SaveFileDialog())
                    {
                        sfd.Filter = "Excel Workbook|*.xlsx";
                        sfd.FileName = "VietRau - Invoice chot phyto - " + exportindex + ".xlsx";
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
