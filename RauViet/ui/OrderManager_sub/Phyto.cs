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
    public partial class Phyto : Form
    {
        DataTable mExportCode_dt, mOrdersTotal_dt;
        private LoadingOverlay loadingOverlay;
        int mCurrentExportID = -1;
        public Phyto()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;


            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = true;

            status_lb.Text = "";


            LuuThayDoiBtn.Click += saveBtn_Click;
            this.KeyDown += Phyto_KeyDown;

        }

        private void Phyto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                if (mCurrentExportID <= 0)
                {
                    return;
                }

                SQLStore_Kho.Instance.removeOrdersPhyto(mCurrentExportID);
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
                string[] keepColumns = { "ExportCodeID", "ExportCode", "ExportDate", "ExportCodeIndex" };
                var parameters = new Dictionary<string, object> { { "Complete", false } };
                mExportCode_dt =await SQLStore_Kho.Instance.getExportCodesAsync(keepColumns, parameters);

                if (mCurrentExportID <= 0 && mExportCode_dt.Rows.Count > 0)
                {
                    mCurrentExportID = Convert.ToInt32(mExportCode_dt.AsEnumerable().Max(r => r.Field<int>("ExportCodeID")));
                }

                mOrdersTotal_dt = await SQLStore_Kho.Instance.getOrdersPhytoAsync(mCurrentExportID);

                
                dataGV.DataSource = mOrdersTotal_dt;
                DataView dv = new DataView(mOrdersTotal_dt);
                dataGV.DataSource = dv;

                Utils.HideColumns(dataGV, new[] { "ExportCodeID"});

                Utils.SetGridHeaders(dataGV, new System.Collections.Generic.Dictionary<string, string> {
                    {"ProductNameEN", "Common name" },
                    {"ProductNameVN", "Vietnamese"},
                    {"BotanicalName", "Botanical Name"},
                    {"NetWeightFinal", "Quantity N.W\r\n(kgs)"},
                    {"Priority", "Ưu\nTiên"}
                });

                Utils.SetGridWidths(dataGV, new System.Collections.Generic.Dictionary<string, int> {
                    {"Priority", 50},
                    {"No", 50},
                    {"ProductNameEN", 180},
                    {"ProductNameVN", 180},
                    {"BotanicalName", 150},
                    {"NetWeightFinal", 80}
                });

                Utils.SetGridFormat_Alignment(dataGV, "Priority", DataGridViewContentAlignment.MiddleRight);
                Utils.SetGridFormat_Alignment(dataGV, "NetWeightFinal", DataGridViewContentAlignment.MiddleCenter);

                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

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

            mOrdersTotal_dt = await SQLStore_Kho.Instance.getOrdersPhytoAsync(mCurrentExportID);
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
            string exportCodeIndex = ((DataRowView)exportCode_cbb.SelectedItem)["ExportCodeIndex"].ToString();
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
                                cell.Style.NumberFormat.Format = "0.00";
                                if (decimal.TryParse(cellValue, out decimal num))
                                {
                                    totalNWOther += num;
                                    cell.Value = num;
                                }
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
                    ws.Cell(totalRow, nwOtherColIndex).Style.NumberFormat.Format = "#,##0.00";
                    ws.Cell(totalRow, nwOtherColIndex).Style.Font.Bold = true;
                    ws.Cell(totalRow, nwOtherColIndex).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell(totalRow, nwOtherColIndex).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    totalRow += 3;
                    ws.Range(totalRow, 5, totalRow, 8).Merge();
                    ws.Cell(totalRow, 5).Value = "Place of issue, HO CHI MINH CITY FEB.     " + exportDate.ToString("dd/MM/yyyy");
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
                    ws.Column(2).Width= 21;
                    ws.Column(3).Width = 9.5;
                    ws.Column(3).Width = 8.5;
                    ws.Column(4).Width = 7.85;
                    ws.Column(5).Width = 7.8;
                    ws.Column(6).Width = 1.2;
                    ws.Column(7).Width = 21;
                    ws.Column(8).Width = 11.2;

                    foreach (var col in exportColumns)
                    {
                        var column = ws.Column(col.Index + 1);
                        column.Style.Alignment.WrapText = true;
                    }
                    //ws.Column(8).Width = 27;
                    // ===== Save file =====
                    using (SaveFileDialog sfd = new SaveFileDialog())
                    {
                        sfd.Filter = "Excel Workbook|*.xlsx";
                        sfd.FileName = "Attachment to Phytosanitary - " + exportCodeIndex + ".xlsx";
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
