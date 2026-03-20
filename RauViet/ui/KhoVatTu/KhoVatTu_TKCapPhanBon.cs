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
    public partial class KhoVatTu_TKCapPhanBon : Form
    {
        DataTable mThongKeCapPhan_dt;
        List<string> mMonths, mYears;
        private LoadingOverlay loadingOverlay;
        public KhoVatTu_TKCapPhanBon()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;


            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            this.KeyDown += KhoVatTu_KeyDown;
            exportExcel_btn.Click += ExportExcel_btn_Click;
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

                mThongKeCapPhan_dt = await SQLStore_KhoVatTu.Instance.getThongKeCapPhanAsync();

                mMonths = mThongKeCapPhan_dt.AsEnumerable().Select(r => Convert.ToInt32(r["Month"]).ToString("D2")).Distinct().OrderBy(y => y).ToList();
                mYears = mThongKeCapPhan_dt.AsEnumerable().Select(r => r["Year"].ToString()).Distinct().OrderBy(y => y).ToList();
                mThongKeCapPhan_dt = PivotDataTable(mThongKeCapPhan_dt, mMonths, mYears);                
                dataGV.DataSource = mThongKeCapPhan_dt;

                var gridHeadersDic = new System.Collections.Generic.Dictionary<string, string>();
                var gridWidthsDic = new System.Collections.Generic.Dictionary<string, int>();
                var gridColorsDic = new Dictionary<string, System.Drawing.Color>();

                gridHeadersDic["MaterialName"] = "Tên\nVật Tư";
                gridWidthsDic["MaterialName"] = 200;

                gridHeadersDic["UnitName"] = "Đ.Vị";
                gridWidthsDic["UnitName"] = 45;

                foreach (var y in mYears)
                {
                    gridHeadersDic[$"Quantity_{y}"] = $"Số Lượng\n{y}";
                    gridWidthsDic[$"Quantity_{y}"] = 85;
                    gridColorsDic[$"Quantity_{y}"] = Color.LightBlue;

                    Utils.SetGridFormat_N1(dataGV, $"Quantity_{y}");
                    Utils.SetGridFormat_Alignment(dataGV, $"Quantity_{y}", DataGridViewContentAlignment.MiddleRight);
                }

                List<Color> colors = new List<Color> { Color.Red, Color.Orange, Color.Gold, Color.YellowGreen, Color.Green, Color.Teal, Color.Cyan, Color.DeepSkyBlue, Color.Blue, Color.Indigo, Color.Violet, Color.Pink };
                foreach (var m in mMonths)
                {
                    int monthInt = Convert.ToInt32(m);
                    foreach (var y in mYears)
                    {
                        string key = $"{m}{y}";
                        gridHeadersDic[$"Quantity_{key}"] = $"Sản Lượng\n{m}/{y}";
                        gridWidthsDic[$"Quantity_{key}"] = 85;
                        gridColorsDic[$"Quantity_{key}"] = colors[monthInt - 1];

                        Utils.SetGridFormat_N1(dataGV, $"Quantity_{key}");
                        Utils.SetGridFormat_Alignment(dataGV, $"Quantity_{key}", DataGridViewContentAlignment.MiddleRight);
                    }
                }
                Utils.HideColumns(dataGV, new[] { "MaterialID" });
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
            result.Columns.Add("MaterialID", typeof(int));
            result.Columns.Add("MaterialName", typeof(string));
            result.Columns.Add("UnitName", typeof(string));

            // Cột theo năm
            foreach (var y in years)
            {
                result.Columns.Add($"Quantity_{y}", typeof(decimal));
            }

            // Tạo cột động theo tháng+năm (MMYYYY)
            foreach (var m in months)
            {
                foreach (var y in years)
                {
                    string key = $"{m}{y}";
                    result.Columns.Add($"Quantity_{key}", typeof(decimal));
                }
            }

            // Group theo SKU + PlantName
            var groups = dt.AsEnumerable()
                           .GroupBy(r => new
                           {
                               MaterialID = Convert.ToInt32(r["MaterialID"]),
                               MaterialName = r["MaterialName"].ToString(),
                               UnitName = r["UnitName"].ToString(),
                           });

            foreach (var g in groups)
            {
                DataRow newRow = result.NewRow();
                newRow["MaterialID"] = g.Key.MaterialID;
                newRow["MaterialName"] = g.Key.MaterialName;
                newRow["UnitName"] = g.Key.UnitName;

                // Dictionary để cộng dồn theo năm
                Dictionary<string, decimal> sumQuantityYear = new Dictionary<string, decimal>();

                foreach (var row in g)
                {
                    string month = row["Month"].ToString().PadLeft(2, '0');
                    string year = row["Year"].ToString();

                    string key = $"{month}{year}";

                    decimal totalMaterialQuantity = row["TotalMaterialQuantity"] == DBNull.Value ? 0 : Convert.ToDecimal(row["TotalMaterialQuantity"]);

                    // Gán theo tháng
                    newRow[$"Quantity_{key}"] = totalMaterialQuantity;

                    // Cộng dồn theo năm
                    if (!sumQuantityYear.ContainsKey(year))
                    {
                        sumQuantityYear[year] = 0;
                    }

                    sumQuantityYear[year] += totalMaterialQuantity;
                }

                // Gán cột theo năm
                foreach (var y in years)
                {
                    newRow[$"Quantity_{y}"] = sumQuantityYear.ContainsKey(y) ? sumQuantityYear[y] : 0;
                }

                result.Rows.Add(newRow);
            }

            return result;
        }

        private async void ExportExcel_btn_Click(object sender, EventArgs e)
        {
            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Message = "Đang xử lý ...";
            loadingOverlay.Show();
            await Task.Delay(100);

            try
            {
                DataTable clonedTable = mThongKeCapPhan_dt.Clone(); // clone structure

                foreach (DataRowView drv in mThongKeCapPhan_dt.DefaultView)
                {
                    clonedTable.ImportRow(drv.Row);
                }

                using (var wb = new XLWorkbook())
                {
                    Dictionary<string, string> columnNames = new Dictionary<string, string> { { "Quantity", "Số Lượng" }, };
                    List<XLColor> ColorTitles = new List<XLColor> { XLColor.Red, XLColor.Orange, XLColor.Gold, XLColor.YellowGreen, XLColor.Green, XLColor.Teal, XLColor.Cyan, XLColor.DeepSkyBlue, XLColor.Blue, XLColor.Indigo, XLColor.Violet, XLColor.Pink };
                    int numColPart = columnNames.Count;

                    int rowExcel = 1;
                    int maxCol = 0;
                    int maxrow = 0;

                    var ws = wb.Worksheets.Add("Cấp Phân");
                    ws.Style.Font.FontName = "Times New Roman";
                    ws.Style.Font.FontSize = 12;

                    ws.Cell(rowExcel, 1).Value = $"THỐNG KÊ CẤP PHÂN QUA CÁC NĂM";
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
                        foreach (var columnNameDic in columnNames)
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
                        ws.Cell(rowExcel, colExcel).Value = row["MaterialName"].ToString(); colExcel++;
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
                        sfd.FileName = $"ThongKeCapPhanBon";
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
