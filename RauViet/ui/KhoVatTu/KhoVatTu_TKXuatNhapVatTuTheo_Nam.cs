using ClosedXML.Excel;
using RauViet.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RauViet.ui
{
    public partial class KhoVatTu_TKXuatNhapVatTuTheo_Nam : Form
    {
        DataTable mMaterialReport_dt, mMaterialReport_Pivot;
        List<string> mMonths, mYears;
        private LoadingOverlay loadingOverlay;
        public KhoVatTu_TKXuatNhapVatTuTheo_Nam()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;


            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            this.KeyDown += KhoVatTu_KeyDown;
            exportExcel_btn.Click += ExportExcel_btn_Click;
            search_tb.TextChanged += Search_tb_TextChanged;
        }

        private void KhoVatTu_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                ShowData();
            }            
        }

        public async void ShowData()
        {
            await Task.Delay(50);
            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Show();

            try
            {

                mMaterialReport_dt = await SQLStore_KhoVatTu.Instance.getThongKeVatTuTheoNamAsync();

                mMonths = mMaterialReport_dt.AsEnumerable().Select(r => Convert.ToInt32(r["Month"]).ToString("D2")).Distinct().OrderBy(y => y).ToList();
                mYears = mMaterialReport_dt.AsEnumerable().Select(r => r["Year"].ToString()).Distinct().OrderBy(y => y).ToList();
                mMaterialReport_Pivot = PivotDataTable(mMaterialReport_dt, mMonths, mYears);                
                dataGV.DataSource = mMaterialReport_Pivot;

                var gridHeadersDic = new System.Collections.Generic.Dictionary<string, string>();
                var gridWidthsDic = new System.Collections.Generic.Dictionary<string, int>();
                var gridColorsDic = new Dictionary<string, System.Drawing.Color>();

                gridHeadersDic["MaterialName"] = "Tên\nVật Tư";
                gridWidthsDic["MaterialName"] = 180;
                gridHeadersDic["UnitName"] = "Đ.Vị";
                gridWidthsDic["UnitName"] = 45;

                foreach (var y in mYears)
                {
                    gridHeadersDic[$"TotalImport_{y}"] = $"SL Nhập\n{y}";
                    gridHeadersDic[$"TotalExport_{y}"] = $"SL Xuất\n{y}";

                    gridWidthsDic[$"TotalImport_{y}"] = 75;
                    gridWidthsDic[$"TotalExport_{y}"] = 70;

                    int yearInt = Convert.ToInt32(y);
                    gridColorsDic[$"TotalImport_{y}"] = yearInt % 2 == 0 ? Color.LightBlue : Color.LightCoral;
                    gridColorsDic[$"TotalExport_{y}"] = yearInt % 2 == 0 ? Color.LightBlue : Color.LightCoral;

                    Utils.SetGridFormat_N1(dataGV, $"TotalImport_{y}");
                    Utils.SetGridFormat_N1(dataGV, $"TotalExport_{y}");
                    Utils.SetGridFormat_N2(dataGV, $"Yield_{y}");
                    Utils.SetGridFormat_Alignment(dataGV, $"TotalImport_{y}", DataGridViewContentAlignment.MiddleRight);
                    Utils.SetGridFormat_Alignment(dataGV, $"TotalExport_{y}", DataGridViewContentAlignment.MiddleRight);
                }

                List<Color> colors = new List<Color> { Color.Red, Color.Orange, Color.Gold, Color.YellowGreen, Color.Green, Color.Teal, Color.Cyan, Color.DeepSkyBlue, Color.Blue, Color.Indigo, Color.Violet, Color.Pink };
                foreach (var m in mMonths)
                {
                    int monthInt = Convert.ToInt32(m);
                    foreach (var y in mYears)
                    {
                        string key = $"{m}{y}";
                        gridHeadersDic[$"TotalImport_{key}"] = $"SL Nhập\n{m}/{y}";
                        gridHeadersDic[$"TotalExport_{key}"] = $"SL Xuất\n{m}/{y}";

                        gridWidthsDic[$"TotalImport_{key}"] = 75;
                        gridWidthsDic[$"TotalExport_{key}"] = 70;

                        gridColorsDic[$"TotalImport_{key}"] = colors[monthInt - 1];
                        gridColorsDic[$"TotalExport_{key}"] = colors[monthInt - 1];

                        Utils.SetGridFormat_N1(dataGV, $"TotalImport_{key}");
                        Utils.SetGridFormat_N1(dataGV, $"TotalExport_{key}");
                        Utils.SetGridFormat_Alignment(dataGV, $"TotalImport_{key}", DataGridViewContentAlignment.MiddleRight);
                        Utils.SetGridFormat_Alignment(dataGV, $"TotalExport_{key}", DataGridViewContentAlignment.MiddleRight);
                    }
                }
                Utils.HideColumns(dataGV, new[] { "MaterialID", "MaterialName_noSign" });
                Utils.SetGridHeaders(dataGV, gridHeadersDic);
                Utils.SetGridWidths(dataGV, gridWidthsDic);
                Utils.SetGridColors(dataGV, gridColorsDic);

                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            catch
            {
            }
            finally
            {
                await Task.Delay(200);
                loadingOverlay.Hide();
            }

        }

        DataTable PivotDataTable(DataTable dt, List<string> months, List<string> years)
        {
            DataTable result = new DataTable();

            // Cột cố định
            result.Columns.Add("MaterialID", typeof(int));
            result.Columns.Add("MaterialName", typeof(string));
            result.Columns.Add("MaterialName_noSign", typeof(string));
            result.Columns.Add("UnitName", typeof(string));

            // Cột theo năm
            foreach (var y in years)
            {
                result.Columns.Add($"TotalImport_{y}", typeof(decimal));
                result.Columns.Add($"TotalExport_{y}", typeof(decimal));
            }

            // Tạo cột động theo tháng+năm (MMYYYY)
            foreach (var m in months)
            {
                foreach (var y in years)
                {
                    string key = $"{m}{y}";
                    result.Columns.Add($"TotalImport_{key}", typeof(decimal));
                    result.Columns.Add($"TotalExport_{key}", typeof(decimal));
                }
            }

            // Group theo SKU + PlantName
            var groups = dt.AsEnumerable()
                           .GroupBy(r => new
                           {
                               MaterialID = Convert.ToInt32(r["MaterialID"]),
                               MaterialName = r["MaterialName"].ToString(),
                               UnitName = r["UnitName"].ToString()
                           });

            foreach (var g in groups)
            {
                DataRow newRow = result.NewRow();
                newRow["MaterialID"] = g.Key.MaterialID;
                newRow["MaterialName"] = g.Key.MaterialName;
                newRow["MaterialName_noSign"] = Utils.RemoveVietnameseSigns(g.Key.MaterialName);
                newRow["UnitName"] = g.Key.UnitName;

                // Dictionary để cộng dồn theo năm
                Dictionary<string, decimal> totalImportYear = new Dictionary<string, decimal>();
                Dictionary<string, decimal> totalExportYear = new Dictionary<string, decimal>();

                foreach (var row in g)
                {
                    string year = row["Year"].ToString();
                    string month = row["Month"].ToString().PadLeft(2, '0');

                    string key = $"{month}{year}";

                    decimal totalImport = row["TotalImport"] == DBNull.Value ? 0 : Convert.ToDecimal(row["TotalImport"]);
                    decimal totalExport = row["TotalExport"] == DBNull.Value ? 0 : Convert.ToDecimal(row["TotalExport"]);

                    // Gán theo tháng
                    newRow[$"TotalImport_{key}"] = totalImport;
                    newRow[$"TotalExport_{key}"] = totalExport;

                    // Cộng dồn theo năm
                    if (!totalImportYear.ContainsKey(year))
                    {
                        totalImportYear[year] = 0;
                        totalExportYear[year] = 0;
                    }

                    totalImportYear[year] += totalImport;
                    totalExportYear[year] += totalExport;
                }

                // Gán cột theo năm
                foreach (var y in years)
                {
                    newRow[$"TotalImport_{y}"] = totalImportYear.ContainsKey(y) ? totalImportYear[y] : 0;
                    newRow[$"TotalExport_{y}"] = totalExportYear.ContainsKey(y) ? totalExportYear[y] : 0;
                }

                result.Rows.Add(newRow);
            }

            return result;
        }

        private void Search_tb_TextChanged(object sender, EventArgs e)
        {
            string keyword = Utils.RemoveVietnameseSigns(search_tb.Text.Trim().ToLower())
                     .Replace("'", "''"); // tránh lỗi cú pháp '

            DataTable dt = dataGV.DataSource as DataTable;
            if (dt == null) return;

            DataView dv = dt.DefaultView;
            dv.RowFilter = $"[MaterialName_noSign] LIKE '%{keyword}%'";
        }
        private async void ExportExcel_btn_Click(object sender, EventArgs e)
        {
            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Message = "Đang xử lý ...";
            loadingOverlay.Show();
            await Task.Delay(100);

            try
            {
                DataTable clonedTable = mMaterialReport_Pivot.Clone(); // clone structure

                foreach (DataRow drv in mMaterialReport_Pivot.Rows)
                {
                    clonedTable.ImportRow(drv);
                }

                using (var wb = new XLWorkbook())
                {
                    Dictionary<string, string> columnNames = new Dictionary<string, string> { { "TotalImport", "SL Nhập" }, { "TotalExport", "SL Xuất" } };
                    List<XLColor> ColorTitles = new List<XLColor> { XLColor.Red, XLColor.Orange, XLColor.Gold, XLColor.YellowGreen, XLColor.Green, XLColor.Teal, XLColor.Cyan, XLColor.DeepSkyBlue, XLColor.Blue, XLColor.Indigo, XLColor.Violet, XLColor.Pink };
                    int numColPart = columnNames.Count;

                    int rowExcel = 1;
                    int maxCol = 0;
                    int maxrow = 0;

                    var ws = wb.Worksheets.Add("Xuất Nhập Vật Tư");
                    ws.Style.Font.FontName = "Times New Roman";
                    ws.Style.Font.FontSize = 12;
                    
                    ws.Cell(rowExcel, 1).Value = $"THỐNG KÊ NHẬP XUẤT VẬT TƯ THEO NĂM";
                    ws.Range(rowExcel, 1, rowExcel, 13).Merge();
                    ws.Cell(rowExcel, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell(rowExcel, 1).Style.Font.Bold = true;
                    ws.Cell(rowExcel, 1).Style.Font.FontSize = 24;

                    ws.Cell(2, 1).Value = "Tên\nVật Tư";
                    ws.Range(2, 1, 3, 1).Merge();
                    ws.Cell(2, 2).Value = "Đ.Vị";
                    ws.Range(2, 2, 3, 2).Merge();

                    int colExcel = 3;
                    foreach (var y in mYears)
                    {
                        rowExcel = 2;
                        
                        {
                            var cell = ws.Cell(rowExcel, colExcel);
                            cell.Value = y;
                            cell.Style.Fill.BackgroundColor = XLColor.Yellow;
                        }
                        ws.Range(rowExcel, colExcel, rowExcel, colExcel + numColPart - 1).Merge();

                        rowExcel++;
                        foreach(var columnNameDic in columnNames)
                        {
                            ws.Cell(rowExcel, colExcel).Value = columnNameDic.Value;
                            colExcel++;
                        }                        
                    }

                    foreach (var m in mMonths)
                    {
                        int month = Convert.ToInt32(m);
                        foreach (var y in mYears)
                        {
                            rowExcel = 2;
                            {
                                var cell = ws.Cell(rowExcel, colExcel);
                                cell.Value = $"{m}/{y}";
                                cell.Style.Fill.BackgroundColor = ColorTitles[month - 1];
                            }
                            ws.Range(rowExcel, colExcel, rowExcel, colExcel + numColPart - 1).Merge();

                            rowExcel++;
                            foreach (var columnNameDic in columnNames)
                            {
                                ws.Cell(rowExcel, colExcel).Value = columnNameDic.Value;
                                colExcel++;
                            }
                        }
                    }

                    maxCol = colExcel - 1;
                    maxrow = 4;

                    int index = 0;

                    rowExcel = maxrow;                    
                    foreach (DataRow row in clonedTable.Rows)
                    {
                        colExcel = 1;
                        ws.Cell(rowExcel, colExcel).Value = row["MaterialName"].ToString();colExcel++;
                        ws.Cell(rowExcel, colExcel).Value = row["UnitName"].ToString(); colExcel++;
                        foreach (var y in mYears)
                        {                            
                            foreach (var columnNameDic in columnNames)
                            {
                                object obj = row[$"{columnNameDic.Key}_{y}"];
                                if (obj != DBNull.Value && obj != null) 
                                {
                                    ws.Cell(rowExcel, colExcel).Value = Convert.ToDecimal(obj);
                                }
                                colExcel++;
                            }
                        }

                        foreach (var m in mMonths)
                        {
                            foreach (var y in mYears)
                            {
                                foreach (var columnNameDic in columnNames)
                                {
                                    object obj = row[$"{columnNameDic.Key}_{y}"];
                                    if (obj != DBNull.Value && obj != null)
                                    {
                                        ws.Cell(rowExcel, colExcel).Value = Convert.ToDecimal(obj);
                                    }
                                    colExcel++;
                                }
                            }
                        }

                        rowExcel++;
                    }

                    maxrow = rowExcel;
                    ws.Range(2, 1, 2, maxCol).Style.Font.Bold = true;
                    ws.Range(2, 1, 3, maxCol).Style.Alignment.WrapText = true;
                    ws.Range(2, 1, 3, maxCol).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Range(2, 1, 3, maxCol).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Range(2, 1, maxrow - 1, maxCol).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    ws.Range(2, 1, maxrow - 1, maxCol).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                    ws.Range(2, 1, maxrow - 1, maxCol).Style.NumberFormat.Format = "#,##0;-#,##0;\"-\"";

                    ws.Columns().AdjustToContents();

                    using (SaveFileDialog sfd = new SaveFileDialog())
                    {
                        sfd.Filter = "Excel Workbook|*.xlsx";
                        sfd.FileName = $"ThongKeXuatNhapVatTuTheoNam";
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
