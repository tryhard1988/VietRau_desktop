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
    public partial class KhoVatTu_TKSanLuongTrong_Nam : Form
    {
        DataTable mPlantingAggregate, mPlantingAggregate_Pivot;
        List<string> mMonths, mYears;
        private LoadingOverlay loadingOverlay;
        public KhoVatTu_TKSanLuongTrong_Nam()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;


            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            this.KeyDown += KhoVatTu_MaterialInvenStore_KeyDown;
            exportExcel_btn.Click += ExportExcel_btn_Click;
            farm_cbb.SelectedIndexChanged += Farm_cbb_SelectedIndexChanged;
        }

        private void KhoVatTu_MaterialInvenStore_KeyDown(object sender, KeyEventArgs e)
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

                mPlantingAggregate = await SQLStore_KhoVatTu.Instance.getPlantingManagementAsync_HarvestQuantity();
                var farm_dt = await SQLStore_KhoVatTu.Instance.GetFarmsAsync();

                farm_cbb.DataSource = farm_dt;
                farm_cbb.DisplayMember = "FarmName";  // hiển thị tên
                farm_cbb.ValueMember = "FarmID";
                farm_cbb.SelectedValue = 2;
                
                mMonths = mPlantingAggregate.AsEnumerable().Select(r => Convert.ToInt32(r["ProcessDate_month"]).ToString("D2")).Distinct().OrderBy(y => y).ToList();
                mYears = mPlantingAggregate.AsEnumerable().Select(r => r["ProcessDate_year"].ToString()).Distinct().OrderBy(y => y).ToList();
                mPlantingAggregate_Pivot = PivotDataTable(mPlantingAggregate, mMonths, mYears);                
                dataGV.DataSource = mPlantingAggregate_Pivot;

                var gridHeadersDic = new System.Collections.Generic.Dictionary<string, string>();
                var gridWidthsDic = new System.Collections.Generic.Dictionary<string, int>();
                var gridColorsDic = new Dictionary<string, System.Drawing.Color>();

                gridHeadersDic["PlantName"] = "Tên\nSản Phẩm";
                gridWidthsDic["PlantName"] = 90;

                foreach (var y in mYears)
                {
                    gridHeadersDic[$"HarvestQuantity_{y}"] = $"Sản Lượng\n{y}";
                    gridHeadersDic[$"TotalArea_{y}"] = $"Diện Tích\n{y}";

                    gridWidthsDic[$"HarvestQuantity_{y}"] = 85;
                    gridWidthsDic[$"TotalArea_{y}"] = 80;

                    int yearInt = Convert.ToInt32(y);
                    gridColorsDic[$"HarvestQuantity_{y}"] = yearInt % 2 == 0 ? Color.LightBlue : Color.LightCoral;
                    gridColorsDic[$"TotalArea_{y}"] = yearInt % 2 == 0 ? Color.LightBlue : Color.LightCoral;

                    Utils.SetGridFormat_N1(dataGV, $"HarvestQuantity_{y}");
                    Utils.SetGridFormat_N1(dataGV, $"TotalArea_{y}");
                    Utils.SetGridFormat_N2(dataGV, $"Yield_{y}");
                    Utils.SetGridFormat_Alignment(dataGV, $"HarvestQuantity_{y}", DataGridViewContentAlignment.MiddleRight);
                    Utils.SetGridFormat_Alignment(dataGV, $"TotalArea_{y}", DataGridViewContentAlignment.MiddleRight);
                }

                List<Color> colors = new List<Color> { Color.Red, Color.Orange, Color.Gold, Color.YellowGreen, Color.Green, Color.Teal, Color.Cyan, Color.DeepSkyBlue, Color.Blue, Color.Indigo, Color.Violet, Color.Pink};
                foreach (var m in mMonths)
                {
                    int monthInt = Convert.ToInt32(m);
                    foreach (var y in mYears)
                    {
                        string key = $"{m}{y}";
                        gridHeadersDic[$"HarvestQuantity_{key}"] = $"Sản Lượng\n{m}/{y}";
                        gridHeadersDic[$"TotalArea_{key}"] = $"Diện Tích\n{m}/{y}";

                        gridWidthsDic[$"HarvestQuantity_{key}"] = 85;
                        gridWidthsDic[$"TotalArea_{key}"] = 80;

                        gridColorsDic[$"HarvestQuantity_{key}"] = colors[monthInt - 1];
                        gridColorsDic[$"TotalArea_{key}"] = colors[monthInt - 1];

                        Utils.SetGridFormat_N1(dataGV, $"HarvestQuantity_{key}");
                        Utils.SetGridFormat_N1(dataGV, $"TotalArea_{key}");
                        Utils.SetGridFormat_Alignment(dataGV, $"HarvestQuantity_{key}", DataGridViewContentAlignment.MiddleRight);
                        Utils.SetGridFormat_Alignment(dataGV, $"TotalArea_{key}", DataGridViewContentAlignment.MiddleRight);
                    }
                }
                Utils.HideColumns(dataGV, new[] { "SKU" });
                Utils.SetGridHeaders(dataGV, gridHeadersDic);
                Utils.SetGridWidths(dataGV, gridWidthsDic);
                Utils.SetGridColors(dataGV, gridColorsDic);

                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                Farm_cbb_SelectedIndexChanged(null, null);
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
            result.Columns.Add("FarmID");
            result.Columns.Add("SKU");
            result.Columns.Add("PlantName");

            // Cột theo năm
            foreach (var y in years)
            {
                result.Columns.Add($"HarvestQuantity_{y}", typeof(decimal));
                result.Columns.Add($"TotalArea_{y}", typeof(decimal));
            }

            // Tạo cột động theo tháng+năm (MMYYYY)
            foreach (var m in months)
            {
                foreach (var y in years)
                {
                    string key = $"{m}{y}";
                    result.Columns.Add($"HarvestQuantity_{key}", typeof(decimal));
                    result.Columns.Add($"TotalArea_{key}", typeof(decimal));
                }
            }

            // Group theo SKU + PlantName
            var groups = dt.AsEnumerable()
                           .GroupBy(r => new
                           {
                               FarmID = r["FarmID"].ToString(),
                               SKU = r["SKU"].ToString(),
                               PlantName = r["PlantName"].ToString()
                           });

            foreach (var g in groups)
            {
                DataRow newRow = result.NewRow();
                newRow["FarmID"] = g.Key.FarmID;
                newRow["SKU"] = g.Key.SKU;
                newRow["PlantName"] = g.Key.PlantName;

                // Dictionary để cộng dồn theo năm
                Dictionary<string, decimal> sumHarvestYear = new Dictionary<string, decimal>();
                Dictionary<string, decimal> sumAreaYear = new Dictionary<string, decimal>();

                foreach (var row in g)
                {
                    string year = row["ProcessDate_year"].ToString();
                    string month = row["ProcessDate_month"].ToString().PadLeft(2, '0');

                    string key = $"{month}{year}";

                    decimal harvest = row["HarvestQuantity"] == DBNull.Value ? 0 : Convert.ToDecimal(row["HarvestQuantity"]);
                    decimal area = row["TotalArea"] == DBNull.Value ? 0 : Convert.ToDecimal(row["TotalArea"]);

                    // Gán theo tháng
                    newRow[$"HarvestQuantity_{key}"] = harvest;
                    newRow[$"TotalArea_{key}"] = area;

                    // Cộng dồn theo năm
                    if (!sumHarvestYear.ContainsKey(year))
                    {
                        sumHarvestYear[year] = 0;
                        sumAreaYear[year] = 0;
                    }

                    sumHarvestYear[year] += harvest;
                    sumAreaYear[year] += area;
                }

                // Gán cột theo năm
                foreach (var y in years)
                {
                    newRow[$"HarvestQuantity_{y}"] = sumHarvestYear.ContainsKey(y) ? sumHarvestYear[y] : 0;
                    newRow[$"TotalArea_{y}"] = sumAreaYear.ContainsKey(y) ? sumAreaYear[y] : 0;
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
                DataTable clonedTable = mPlantingAggregate_Pivot.Clone(); // clone structure

                foreach (DataRowView drv in mPlantingAggregate_Pivot.DefaultView)
                {
                    clonedTable.ImportRow(drv.Row);
                }

                using (var wb = new XLWorkbook())
                {
                    Dictionary<string, string> columnNames = new Dictionary<string, string> { { "HarvestQuantity", "Sản Lượng\n(Kg)" }, { "TotalArea", "Diện Tích\n(m2)" } };
                    List<XLColor> ColorTitles = new List<XLColor> { XLColor.Red, XLColor.Orange, XLColor.Gold, XLColor.YellowGreen, XLColor.Green, XLColor.Teal, XLColor.Cyan, XLColor.DeepSkyBlue, XLColor.Blue, XLColor.Indigo, XLColor.Violet, XLColor.Pink };
                    int numColPart = columnNames.Count;

                    int rowExcel = 1;
                    int maxCol = 0;
                    int maxrow = 0;

                    var ws = wb.Worksheets.Add("Thông Kê Qua Các Năm");
                    ws.Style.Font.FontName = "Times New Roman";
                    ws.Style.Font.FontSize = 12;
                    
                    ws.Cell(rowExcel, 1).Value = $"THỐNG KÊ QUA CÁC NĂM";
                    ws.Range(rowExcel, 1, rowExcel, 13).Merge();
                    ws.Cell(rowExcel, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell(rowExcel, 1).Style.Font.Bold = true;
                    ws.Cell(rowExcel, 1).Style.Font.FontSize = 24;

                    ws.Cell(2, 1).Value = "Tên\nSản Phẩm";
                    ws.Range(2, 1, 3, 1).Merge();
                    int colExcel = 2;
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
                        ws.Cell(rowExcel, colExcel).Value = row["PlantName"].ToString();
                        colExcel++;
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

                    ws.Columns().AdjustToContents();

                    using (SaveFileDialog sfd = new SaveFileDialog())
                    {
                        sfd.Filter = "Excel Workbook|*.xlsx";
                        sfd.FileName = $"ThongKeQuaCacNam_{Utils.RemoveVietnameseSigns(farm_cbb.Text).Replace(" ","")}";
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

        private void Farm_cbb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (farm_cbb.SelectedValue == null || farm_cbb.SelectedValue == DBNull.Value || mPlantingAggregate_Pivot == null)
                return;

            int farmId;
            if (!int.TryParse(farm_cbb.SelectedValue.ToString(), out farmId))
                return;

            mPlantingAggregate_Pivot.DefaultView.RowFilter = $"FarmID = {farmId}";
        }
    }
}
