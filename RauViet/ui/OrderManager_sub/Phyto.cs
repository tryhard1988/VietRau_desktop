using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using RauViet.classes;

namespace RauViet.ui
{
    public partial class Phyto : Form
    {
        DataTable mExportCode_dt, mOrdersTotal_dt;
        private LoadingOverlay loadingOverlay;
        public Phyto()
        {
            InitializeComponent();

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = true;

            status_lb.Text = "";


            LuuThayDoiBtn.Click += saveBtn_Click;           
            dataGV.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.dataGV_RowPrePaint);
            //dataGV.CellEndEdit += dataGV_CellEndEdit;

            exportCode_cbb.SelectedIndexChanged += exportCode_search_cbb_SelectedIndexChanged;
        }

        

        public async void ShowData()
        {
            await Task.Delay(50);
            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Show();           

            try
            {
                var ordersPackingTask = SQLManager.Instance.getOrdersPhytoAsync();
                string[] keepColumns = { "ExportCodeID", "ExportCode", "ExportDate" };
                var parameters = new Dictionary<string, object> { { "Complete", false } };
                var exportCodeTask = SQLStore.Instance.getExportCodesAsync(keepColumns, parameters);

                await Task.WhenAll(ordersPackingTask, exportCodeTask);

                mExportCode_dt = exportCodeTask.Result;
                mOrdersTotal_dt = ordersPackingTask.Result;

                mOrdersTotal_dt.Columns.Add(new DataColumn("No", typeof(string)));

                int count = 1;
                foreach (DataRow dr in mOrdersTotal_dt.Rows)
                {
                    dr["No"] = count++;

                }

                count = 0;
                mOrdersTotal_dt.Columns["No"].SetOrdinal(count++);
                mOrdersTotal_dt.Columns["BotanicalName"].SetOrdinal(count++);
                mOrdersTotal_dt.Columns["ProductNameEN"].SetOrdinal(count++);
                mOrdersTotal_dt.Columns["ProductNameVN"].SetOrdinal(count++);                
                mOrdersTotal_dt.Columns["NetWeightFinal"].SetOrdinal(count++);
                dataGV.DataSource = mOrdersTotal_dt;

                dataGV.Columns["ExportCodeID"].Visible = false;

                dataGV.Columns["ProductNameEN"].HeaderText = "Common name";
                dataGV.Columns["ProductNameVN"].HeaderText = "Vietnamese";
                dataGV.Columns["BotanicalName"].HeaderText = "Botanical Name";
                dataGV.Columns["NetWeightFinal"].HeaderText = "Quantity N.W\r\n(kgs)";

                dataGV.Columns["Priority"].HeaderText = "Ưu\nTiên";

                dataGV.Columns["Priority"].Width = 30;
                dataGV.Columns["No"].Width = 30;
                // dataGV.Columns["STT"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGV.Columns["ProductNameEN"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGV.Columns["ProductNameVN"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGV.Columns["BotanicalName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGV.Columns["NetWeightFinal"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGV.Columns["Priority"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

                dataGV.Columns["Priority"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dataGV.Columns["NetWeightFinal"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

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
                await Task.Delay(200);
                loadingOverlay.Hide();
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
            if (exportCode_cbb.SelectedItem == null) return;
            string exportCode = ((DataRowView)exportCode_cbb.SelectedItem)["ExportCode"].ToString();
            DateTime exportDate = Convert.ToDateTime(((DataRowView)exportCode_cbb.SelectedItem)["ExportDate"]);
            try
            {
                using (var wb = new XLWorkbook())
                {
                    var ws = wb.Worksheets.Add("Data");
                    ws.Style.Font.FontName = "Times New Roman";
                    ws.Style.Font.FontSize = 10;
                    ws.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    // Danh sách cột muốn xuất theo Name
                    string[] columnsToExport = new string[] { "No", "BotanicalName", "ProductNameEN", "ProductNameVN", "NetWeightFinal" };

                    // Lọc cột hiển thị và có trong danh sách
                    var exportColumns = dgv.Columns.Cast<DataGridViewColumn>()
                                            .Where(c => c.Visible && columnsToExport.Contains(c.Name))
                                            .OrderBy(c => Array.IndexOf(columnsToExport, c.Name))
                                            .ToList();


                    ws.Range(1, 4, 1, 5).Merge();

                    int count = exportColumns.Count + 3;
                    int rowInd = 2;     
                    
                    ws.Range(rowInd, 1, rowInd, count).Merge();
                    ws.Cell(rowInd, 1).Value = "CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM";
                    ws.Cell(rowInd, 1).Style.Font.Bold = true;
                    ws.Cell(rowInd, 1).Style.Font.FontSize = 10;
                    ws.Cell(rowInd, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    rowInd++;
                    ws.Range(rowInd, 1, rowInd, count).Merge();
                    ws.Cell(rowInd, 1).Value = "SOCIALIST REPUBLIC OF VIET NAM";
                    ws.Cell(rowInd, 1).Style.Font.Bold = true;
                    ws.Cell(rowInd, 1).Style.Font.FontSize = 10;
                    ws.Cell(rowInd, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    rowInd++;
                    ws.Range(rowInd, 1, rowInd, count).Merge();
                    ws.Cell(rowInd, 1).Value = "ĐỘC LẬP - TỰ DO - HẠNH PHÚC";
                    ws.Cell(rowInd, 1).Style.Font.FontSize = 10;
                    ws.Cell(rowInd, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    rowInd++;
                    ws.Range(rowInd, 1, rowInd, count).Merge();
                    ws.Cell(rowInd, 1).Value = "INDEPENDENCE-FREEDOM-HAPPINESS";
                    ws.Cell(rowInd, 1).Style.Font.FontSize = 10;
                    ws.Cell(rowInd, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    rowInd++;
                    ws.Range(rowInd, 1, rowInd, count).Merge();
                    ws.Cell(rowInd, 1).Value = "BỘ NÔNG NGHIỆP VÀ PHÁT TRIỂN NÔNG THÔN";
                    ws.Cell(rowInd, 1).Style.Font.Bold = true;
                    ws.Cell(rowInd, 1).Style.Font.FontSize = 10;
                    ws.Cell(rowInd, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    rowInd++;
                    ws.Range(rowInd, 1, rowInd, count).Merge();
                    ws.Cell(rowInd, 1).Value = "MINISTRY OF AGRICUL TURE & RURAL DEVELOPMENT";
                    ws.Cell(rowInd, 1).Style.Font.Bold = true;
                    ws.Cell(rowInd, 1).Style.Font.FontSize = 10;
                    ws.Cell(rowInd, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    rowInd++;
                    ws.Range(rowInd, 1, rowInd, count).Merge();
                    ws.Cell(rowInd, 1).Value = "CỤC BẢO VỆ THỰC VẬT";
                    ws.Cell(rowInd, 1).Style.Font.FontSize = 10;
                    ws.Cell(rowInd, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    rowInd++;
                    ws.Range(rowInd, 1, rowInd, count).Merge();
                    ws.Cell(rowInd, 1).Value = "PLANT PROTECTION DEPARTMENT";
                    ws.Cell(rowInd, 1).Style.Font.FontSize = 10;
                    ws.Cell(rowInd, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    rowInd++;
                    ws.Range(rowInd, 1, rowInd, count).Merge();
                    ws.Cell(rowInd, 1).Value = "Attachment to Phytosanitary Certificate No. ...............................................................";
                    ws.Cell(rowInd, 1).Style.Font.FontSize = 10;
                    ws.Cell(rowInd, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;



                    int headerStartRow = 11; // Header cấp 2 và 3 bắt đầu từ hàng 5
                                        
                    // ===== Header cấp 2 và cấp 3 =====
                    int colIndex = 1;
                    foreach (var col in exportColumns)
                    {
                        // Nếu là ProductNameEN thì merge 3 cột
                        if (col.Name == "ProductNameEN")
                        {
                            ws.Range(headerStartRow + 1, colIndex, headerStartRow + 1, colIndex + 2).Merge();

                            // Header con ở hàng 6
                            var cell = ws.Cell(headerStartRow + 1, colIndex);
                            cell.Value = col.HeaderText;
                            cell.Style.Font.Bold = true;
                            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Range(headerStartRow + 1, colIndex, headerStartRow + 1, colIndex + 3).Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Đóng khung

                            // Cập nhật colIndex để bỏ qua 2 cột tiếp theo đã merge
                            colIndex += 4;
                        }
                        else
                        {
                            // Header con ở hàng 6
                            var cell = ws.Cell(headerStartRow + 1, colIndex);
                            cell.Value = col.HeaderText;
                            cell.Style.Font.Bold = true;
                            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Đóng khung
                            colIndex++;
                        }
                        
                    }

                    // ===== Data bắt đầu từ hàng 7 =====
                    decimal totalNWOther = 0; // Biến lưu tổng
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

                            // Căn phải các cột số
                            if (col.ValueType == typeof(int) || col.ValueType == typeof(decimal))
                                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                            else
                                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                            // Thêm border
                            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                            // Merge dữ liệu nếu là ProductNameEN
                            if (col.Name == "ProductNameEN")
                            {
                                ws.Range(rowIndex, colIndex, rowIndex, colIndex + 3).Merge();
                                ws.Range(rowIndex, colIndex, rowIndex, colIndex + 3).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                                colIndex += 4; // Bỏ qua 2 cột tiếp theo
                            }
                            else
                            {
                                colIndex++;
                            }

                            // Tính tổng cột NWOther
                            if (col.Name == "NetWeightFinal")
                            {
                                if (decimal.TryParse(cellValue, out decimal num))
                                    totalNWOther += num;
                            }
                        }
                        rowIndex++;
                    }


                    // Ghi tổng xuống Excel, ví dụ ở cột NWOther, hàng tiếp theo
                    int totalRow = rowIndex;
                    int nwOtherColIndex = exportColumns.FindIndex(c => c.Name == "NetWeightFinal") + 1 + 3;

                    ws.Range(totalRow, 1, totalRow, nwOtherColIndex - 1).Merge();
                    ws.Cell(totalRow, 1).Value = "TOTAL";
                    ws.Cell(totalRow, 1).Style.Font.Bold = true;
                    ws.Cell(totalRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Range(totalRow, 1, totalRow, nwOtherColIndex - 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    ws.Cell(totalRow, nwOtherColIndex).Value = totalNWOther;
                    ws.Cell(totalRow, nwOtherColIndex).Style.Font.Bold = true;
                    ws.Cell(totalRow, nwOtherColIndex).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell(totalRow, nwOtherColIndex).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    totalRow += 3;
                    ws.Range(totalRow, 5, totalRow, 8).Merge();
                    ws.Cell(totalRow, 5).Value = "Place of issue, HO CHI MINH CITY FEB.     " + exportDate.ToString("dd/mm/yyyy");
                    ws.Cell(totalRow, 5).Style.Font.Bold = true;
                    ws.Cell(totalRow, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                   
                    totalRow += 1;
                    ws.Range(totalRow, 1, totalRow, 4).Merge();
                    ws.Cell(totalRow, 1).Value = "Stamp of organization";
                    ws.Cell(totalRow, 1).Style.Font.Bold = true;
                    ws.Cell(totalRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    ws.Range(totalRow, 5, totalRow, 8).Merge();
                    ws.Cell(totalRow, 5).Value = "Name & signature of Authorized officer";
                    ws.Cell(totalRow, 5).Style.Font.Bold = true;
                    ws.Cell(totalRow, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;



                    ws.Columns().AdjustToContents();

                    // Lấy hình từ Resources
                    Image logo1 = Properties.Resources.Picture1;

                    // Chuyển Image thành Stream
                    using (var ms = new MemoryStream())
                    {
                        logo1.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        ms.Position = 0; // reset vị trí stream về đầu

                        // Thêm hình vào Excel
                        var picture = ws.AddPicture(ms)
                                        .MoveTo(ws.Cell(1, 4));
                    }
                    ws.Row(1).Height = 125;
                    ws.Column(1).Width = 4.3;
                    ws.Column(2).Width= 28;
                    ws.Column(3).Width = 9.5;
                    ws.Column(3).Width = 8.5;
                    ws.Column(4).Width = 7.85;
                    ws.Column(5).Width = 7.8;
                    ws.Column(6).Width = 1.2;
                    ws.Column(7).Width = 28;
                    ws.Column(8).Width = 11.2;
                    //ws.Column(8).Width = 27;
                    // ===== Save file =====
                    using (SaveFileDialog sfd = new SaveFileDialog())
                    {
                        sfd.Filter = "Excel Workbook|*.xlsx";
                        sfd.FileName = "PHYTO_" + exportCode + ".xlsx";
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
