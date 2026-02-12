using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Spreadsheet;
using RauViet.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Color = System.Drawing.Color;
using DataTable = System.Data.DataTable;

namespace RauViet.ui
{
    public partial class SumaryAttendanceReport : Form
    {
        DataTable summaryAttendance_dt;
        List<int> mYear;
        public SumaryAttendanceReport()
        {
            InitializeComponent();

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            exportExcel_btn.Click += ExportExcel_btn_Click;
        }

        public async void ShowData()
        {
            await Task.Delay(50);
            LoadingOverlay loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Show();
            await Task.Delay(200);

            try
            {
                var summaryAttendanceTask = SQLManager_QLNS.Instance.GetSumaryAttendanceHistoryAsync();

                await Task.WhenAll(summaryAttendanceTask);
                DataTable salarySummaryByYear_dt = summaryAttendanceTask.Result;

                mYear = salarySummaryByYear_dt.AsEnumerable().Select(r => r.Field<int>("ReportYear")).Distinct().OrderBy(y => y).ToList();

                summaryAttendance_dt = new DataTable();

                summaryAttendance_dt.Columns.Add("ReportMonth", typeof(string));
                summaryAttendance_dt.Columns.Add("WorkBlock", typeof(string));

                foreach (var year in mYear)
                {
                    summaryAttendance_dt.Columns.Add($"EmployeeCount_{year}", typeof(decimal));
                    summaryAttendance_dt.Columns.Add($"WorkHours_{year}", typeof(decimal));
                    summaryAttendance_dt.Columns.Add($"OvertimeHours_{year}", typeof(decimal));
                }

                var grouped = salarySummaryByYear_dt.AsEnumerable()
                                                    .Where(r => !string.IsNullOrEmpty(r.Field<string>("WorkBlockCode")))
                                                    .GroupBy(r => new
                                                    {
                                                        ReportMonth = r.Field<int>("ReportMonth"),
                                                        WorkBlock = r.Field<string>("WorkBlock")
                                                    });

                foreach (var g in grouped)
                {
                    var newRow = summaryAttendance_dt.NewRow();

                    newRow["ReportMonth"] =$"Tháng {g.Key.ReportMonth.ToString("D2")}";
                    newRow["WorkBlock"] = g.Key.WorkBlock;

                    foreach (var year in mYear)
                    {
                        var yearRow = g.FirstOrDefault(r => r.Field<int>("ReportYear") == year);

                        if (yearRow != null)
                        {
                            newRow[$"EmployeeCount_{year}"] =
                                yearRow.Field<int?>("EmployeeCount") ?? 0;

                            newRow[$"WorkHours_{year}"] =
                                yearRow.Field<decimal?>("WorkHours") ?? 0;

                            newRow[$"OvertimeHours_{year}"] =
                                yearRow.Field<decimal?>("OvertimeHours") ?? 0;
                        }
                        else
                        {
                            newRow[$"EmployeeCount_{year}"] = 0;
                            newRow[$"WorkHours_{year}"] = 0;
                            newRow[$"OvertimeHours_{year}"] = 0;
                        }
                    }

                    summaryAttendance_dt.Rows.Add(newRow);
                }

                dataGV.DataSource = summaryAttendance_dt;

                System.Collections.Generic.Dictionary<string, string> dataDic_name = new System.Collections.Generic.Dictionary<string, string> {
                    {"ReportMonth", "Tháng" },
                    {"WorkBlock", "Khối" }
                };

                System.Collections.Generic.Dictionary<string, int> dataDic_Width = new System.Collections.Generic.Dictionary<string, int> {
                    {"ReportMonth", 70 },
                    {"WorkBlock", 90 }
                };

                System.Collections.Generic.Dictionary<string, System.Drawing.Color> dataDic_Color = new System.Collections.Generic.Dictionary<string, System.Drawing.Color>();

                foreach (var year in mYear)
                {
                    dataDic_name[$"EmployeeCount_{year}"] = $"Số CN ({year})";
                    dataDic_name[$"WorkHours_{year}"] = $"Công Thường ({year})";
                    dataDic_name[$"OvertimeHours_{year}"] = $"Công T.Ca ({year})";

                    dataDic_Width[$"EmployeeCount_{year}"] = 60;
                    dataDic_Width[$"WorkHours_{year}"] = 100;
                    dataDic_Width[$"OvertimeHours_{year}"] = 90;

                    dataDic_Color[$"EmployeeCount_{year}"] = Color.Coral;
                    dataDic_Color[$"WorkHours_{year}"] = Color.Aqua;
                    dataDic_Color[$"OvertimeHours_{year}"] = Color.LightCyan;

                    Utils.SetGridFormat_NO(dataGV, $"EmployeeCount_{year}");
                    Utils.SetGridFormat_N1(dataGV, $"WorkHours_{year}");
                    Utils.SetGridFormat_N1(dataGV, $"OvertimeHours_{year}");
                }


                Utils.SetGridHeaders(dataGV, dataDic_name);
                Utils.SetGridWidths(dataGV, dataDic_Width);
                Utils.SetGridColors(dataGV, dataDic_Color);

                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            catch
            {
            }
            finally
            {
                await Task.Delay(100);
                loadingOverlay.Hide();
            }
        }

        private async void ExportExcel_btn_Click(object sender, EventArgs e)
        {
            await Task.Delay(50);
            LoadingOverlay loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Show();
            await Task.Delay(200);

            try
            {
                using (var wb = new XLWorkbook())
                {
                    var ws_thang = wb.Worksheets.Add("Báo Cáo Tháng");
                    ws_thang.Style.Font.FontName = "Times New Roman";
                    ws_thang.Style.Font.FontSize = 12;
                    ws_thang.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    var ws_nam = wb.Worksheets.Add("Báo Cáo Năm");
                    ws_nam.Style.Font.FontName = "Times New Roman";
                    ws_nam.Style.Font.FontSize = 12;
                    ws_nam.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;


                    BaoCaoThang(ws_thang);
                    BaoCaoNam(ws_nam);

                    // ===== Save file =====
                    using (SaveFileDialog sfd = new SaveFileDialog())
                    {
                        sfd.Filter = "Excel Workbook|*.xlsx";
                        sfd.FileName = $"BANG SO SANH GIO CONG.xlsx";
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
            catch
            {
            }
            finally
            {
                await Task.Delay(100);
                loadingOverlay.Hide();
            }
        }


        private void BaoCaoThang(IXLWorksheet ws_thang)
        {
            int numColumn = 12;
            int rowInd = 1;

            ws_thang.Range(rowInd, 1, rowInd, numColumn).Merge();
            ws_thang.Cell(rowInd, 1).Value = $"BẢNG SO SÁNH GIỜ CÔNG";
            ws_thang.Cell(rowInd, 1).Style.Font.Bold = true;
            ws_thang.Cell(rowInd, 1).Style.Font.FontSize = 20;
            ws_thang.Cell(rowInd, 1).Style.Font.FontColor = XLColor.Black;
            ws_thang.Cell(rowInd, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;


            rowInd += 2;
            int colIndex = 1;

            ws_thang.Range(rowInd, colIndex, rowInd, colIndex + 1).Merge();
            var cell_nam = ws_thang.Cell(rowInd, colIndex);
            cell_nam.Value = "NĂM";
            cell_nam.Style.Font.Bold = true;
            cell_nam.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws_thang.Range(rowInd, colIndex, rowInd, colIndex + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Đóng khung
            ws_thang.Range(rowInd, colIndex, rowInd, colIndex + 1).Style.Fill.BackgroundColor = XLColor.FromArgb(218, 242, 208);
            colIndex += 2;
            // ===== Header cấp 2 và cấp 3 =====
            List<string> columnsName = new List<string> { "Tháng", "KHỐI" };
            List<string> exportColumns = new List<string> { "ReportMonth", "WorkBlock" };
            List<XLColor> yearColors = new List<XLColor> { XLColor.LightBlue, XLColor.LightBrown };
            int yearIndex = 0;
            foreach (var year in mYear)
            {
                ws_thang.Range(rowInd, colIndex, rowInd, colIndex + 2).Merge();
                var cell = ws_thang.Cell(rowInd, colIndex);
                cell.Value = year;
                cell.Style.Font.Bold = true;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Alignment.WrapText = true;
                ws_thang.Range(rowInd, colIndex, rowInd, colIndex + 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Đóng khung
                ws_thang.Range(rowInd, colIndex, rowInd, colIndex + 2).Style.Fill.BackgroundColor = yearColors[yearIndex %2];
                colIndex += 3;
                yearIndex++;

                columnsName.Add("SỐ CN");
                columnsName.Add("SỐ GIỜ CÔNG THƯỜNG");
                columnsName.Add("SỐ GIỜ TĂNG CA");
                exportColumns.Add($"EmployeeCount_{year}");
                exportColumns.Add($"WorkHours_{year}");
                exportColumns.Add($"OvertimeHours_{year}");
            }
            var Color_SoCN = XLColor.FromArgb(131, 226, 142);
            var Color_SoGioCongThuong = XLColor.FromArgb(247, 199, 172);
            var Color_SoGioTangCa = XLColor.FromArgb(188, 232, 227);

            rowInd++;
            colIndex = 1;
            foreach (var col in columnsName)
            {
                var cell = ws_thang.Cell(rowInd, colIndex);
                cell.Value = col;
                cell.Style.Font.Bold = true;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Alignment.WrapText = true;
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Đóng khung

                string col_noSign = Utils.RemoveVietnameseSigns(col).ToLower();
                if (col_noSign.CompareTo("so cn") == 0)
                {
                    cell.Style.Fill.BackgroundColor = Color_SoCN;
                }
                else if (col_noSign.CompareTo("so gio cong thuong") == 0)
                {
                    cell.Style.Fill.BackgroundColor = Color_SoGioCongThuong;
                }
                else if (col_noSign.CompareTo("so gio tang ca") == 0)
                {
                    cell.Style.Fill.BackgroundColor = Color_SoGioTangCa;
                }
                colIndex++;
            }


            var grouped = summaryAttendance_dt.AsEnumerable()
                                                .GroupBy(r => Utils.RemoveVietnameseSigns(r.Field<string>("WorkBlock")))
                                                .Select(g =>
                                                {
                                                    var result = new Dictionary<string, object>();

                                                    result["WorkBlock"] = g.Key;

                                                    foreach (var year in mYear)
                                                    {
                                                        result[$"EmployeeCount_{year}"] = g.Sum(r => Convert.ToInt32(r[$"EmployeeCount_{year}"] ?? 0));
                                                        result[$"WorkHours_{year}"] = g.Sum(r => Convert.ToDecimal(r[$"WorkHours_{year}"] ?? 0));
                                                        result[$"OvertimeHours_{year}"] = g.Sum(r => Convert.ToDecimal(r[$"OvertimeHours_{year}"] ?? 0));
                                                    }

                                                    return result;
                                                })
                                                .ToList();

            string currentWorkBlock = "";
            rowInd++;
            foreach (DataRow row in summaryAttendance_dt.Rows)
            {
                string workBlock = Utils.RemoveVietnameseSigns(row["WorkBlock"].ToString().Trim());
                if (!string.IsNullOrWhiteSpace(currentWorkBlock) && currentWorkBlock.CompareTo(workBlock) != 0)
                {
                    colIndex = 1;
                    ws_thang.Range(rowInd, colIndex, rowInd, colIndex + 1).Merge();
                    var cellText = ws_thang.Cell(rowInd, colIndex);
                    cellText.Value = "Total";
                    cellText.Style.Font.Bold = true;
                    cellText.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws_thang.Range(rowInd, colIndex, rowInd, colIndex + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    ws_thang.Range(rowInd, colIndex, rowInd, colIndex + 1).Style.Fill.BackgroundColor = XLColor.Gray;

                    colIndex = 3;
                    var item = grouped.FirstOrDefault(x => x["WorkBlock"].ToString().Equals(currentWorkBlock, StringComparison.OrdinalIgnoreCase));
                    foreach (var year in mYear)
                    {
                        var cell1 = ws_thang.Cell(rowInd, colIndex);
                        cell1.Value = Convert.ToInt32(item[$"EmployeeCount_{year}"]);
                        cell1.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        cell1.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        cell1.Style.Fill.BackgroundColor = Color_SoCN;

                        var cell2 = ws_thang.Cell(rowInd, colIndex + 1);
                        cell2.Value = Convert.ToDecimal(item[$"WorkHours_{year}"]);
                        cell2.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        cell2.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        cell2.Style.Fill.BackgroundColor = Color_SoGioCongThuong;

                        var cell3 = ws_thang.Cell(rowInd, colIndex + 2);
                        cell3.Value = Convert.ToDecimal(item[$"OvertimeHours_{year}"]);
                        cell3.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        cell3.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        cell3.Style.Fill.BackgroundColor = Color_SoGioTangCa;

                        colIndex += 3;
                    }
                    rowInd++;
                }

                colIndex = 1;
                foreach (var colName in exportColumns)
                {
                    var cell = ws_thang.Cell(rowInd, colIndex);
                    string cellValue = row[colName]?.ToString() ?? "";

                    if (colName.CompareTo("ReportMonth") == 0 || colName.CompareTo("WorkBlock") == 0)
                    {
                        cell.Value = cellValue;
                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    }
                    else if (colName.StartsWith("EmployeeCount_"))
                    {
                        cell.Value = Convert.ToInt32(cellValue);
                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        cell.Style.NumberFormat.Format = "#,##0;-#,##0;\"-\"";
                    }
                    else
                    {
                        cell.Value = Convert.ToDecimal(cellValue);
                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        cell.Style.NumberFormat.Format = "#,##0.0;-#,##0;\"-\"";
                    }

                    cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Đóng khung
                    colIndex++;
                }
                rowInd++;


                currentWorkBlock = workBlock;
            }

            if (!string.IsNullOrWhiteSpace(currentWorkBlock))
            {
                colIndex = 1;
                ws_thang.Range(rowInd, colIndex, rowInd, colIndex + 1).Merge();
                var cellText = ws_thang.Cell(rowInd, colIndex);
                cellText.Value = "Total";
                cellText.Style.Font.Bold = true;
                cellText.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws_thang.Range(rowInd, colIndex, rowInd, colIndex + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws_thang.Range(rowInd, colIndex, rowInd, colIndex + 1).Style.Fill.BackgroundColor = XLColor.Gray;

                colIndex = 3;
                var item = grouped.FirstOrDefault(x => x["WorkBlock"].ToString().Equals(currentWorkBlock, StringComparison.OrdinalIgnoreCase));
                foreach (var year in mYear)
                {
                    var cell1 = ws_thang.Cell(rowInd, colIndex);
                    cell1.Value = Convert.ToInt32(item[$"EmployeeCount_{year}"]);
                    cell1.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    cell1.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    cell1.Style.Fill.BackgroundColor = Color_SoCN;

                    var cell2 = ws_thang.Cell(rowInd, colIndex + 1);
                    cell2.Value = Convert.ToDecimal(item[$"WorkHours_{year}"]);
                    cell2.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    cell2.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    cell2.Style.Fill.BackgroundColor = Color_SoGioCongThuong;

                    var cell3 = ws_thang.Cell(rowInd, colIndex + 2);
                    cell3.Value = Convert.ToDecimal(item[$"OvertimeHours_{year}"]);
                    cell3.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    cell3.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    cell3.Style.Fill.BackgroundColor = Color_SoGioTangCa;

                    colIndex += 3;
                }
                rowInd++;
            }

            foreach (var col in ws_thang.ColumnsUsed())
            {
                col.Width = 10.3;
            }
            ws_thang.Column(1).Width = 8.5;
            ws_thang.Column(2).Width = 14;
            ws_thang.Row(3).Height = 24;
        }

        private void BaoCaoNam(IXLWorksheet ws_thang)
        {
            var grouped = summaryAttendance_dt.AsEnumerable()
                                                .GroupBy(r => Utils.RemoveVietnameseSigns(r.Field<string>("WorkBlock")))
                                                .Select(g =>
                                                {
                                                    var result = new Dictionary<string, object>();

                                                    result["WorkBlock"] = g.First().Field<string>("WorkBlock");

                                                    foreach (var year in mYear)
                                                    {
                                                        result[$"EmployeeCount_{year}"] = g.Sum(r => Convert.ToInt32(r[$"EmployeeCount_{year}"] ?? 0));
                                                        result[$"WorkHours_{year}"] = g.Sum(r => Convert.ToDecimal(r[$"WorkHours_{year}"] ?? 0));
                                                        result[$"OvertimeHours_{year}"] = g.Sum(r => Convert.ToDecimal(r[$"OvertimeHours_{year}"] ?? 0));
                                                    }

                                                    return result;
                                                })
                                                .ToList();

            int numColumn = 10;
            int rowInd = 1;

            ws_thang.Range(rowInd, 1, rowInd, numColumn).Merge();
            ws_thang.Cell(rowInd, 1).Value = $"BẢNG SO SÁNH GIỜ CÔNG";
            ws_thang.Cell(rowInd, 1).Style.Font.Bold = true;
            ws_thang.Cell(rowInd, 1).Style.Font.FontSize = 20;
            ws_thang.Cell(rowInd, 1).Style.Font.FontColor = XLColor.Black;
            ws_thang.Cell(rowInd, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;


            rowInd += 2;
            int colIndex = 1;

            List<XLColor> yearColors = new List<XLColor> { XLColor.LightBlue, XLColor.LightBrown };
            var Color_SoCN = XLColor.FromArgb(131, 226, 142);
            var Color_SoGioCongThuong = XLColor.FromArgb(247, 199, 172);
            var Color_SoGioTangCa = XLColor.FromArgb(188, 232, 227);

            List<string> columnsName_soCN = new List<string> { "SỐ CÔNG NHÂN" };
            List<string> columnsName_soGioCong = new List<string> { "SỐ GIỜ CÔNG" };
            List<string> columnsName_soGioTangCa = new List<string> { "SỐ GIỜ TĂNG CA" };
            List<string> exportColumns = new List<string> { "van phong", "farm", "xuong" };

            foreach (var year in mYear)
            {
                columnsName_soCN.Add(year.ToString());
                columnsName_soGioCong.Add(year.ToString());
                columnsName_soGioTangCa.Add(year.ToString());
            }

            rowInd++;
            //số công nhân
            {
                colIndex = 3;
                foreach (var col in columnsName_soCN)
                {
                    var cell = ws_thang.Cell(rowInd, colIndex);
                    cell.Value = col;
                    cell.Style.Font.Bold = true;
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    cell.Style.Alignment.WrapText = true;
                    cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Đóng khung
                    cell.Style.Fill.BackgroundColor = Color_SoCN;

                    colIndex++;
                }

                rowInd++;
                foreach (var workBlock in exportColumns)
                {
                    colIndex = 3;
                    var item = grouped.FirstOrDefault(x => Utils.RemoveVietnameseSigns(x["WorkBlock"].ToString()).Equals(workBlock, StringComparison.OrdinalIgnoreCase));
                    var cellworkBlock = ws_thang.Cell(rowInd, colIndex);
                    cellworkBlock.Value = Convert.ToString(item["WorkBlock"]);
                    cellworkBlock.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    cellworkBlock.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    colIndex++;
                    foreach (var year in mYear)
                    {
                        var cell1 = ws_thang.Cell(rowInd, colIndex);
                        cell1.Value = Convert.ToInt32(item[$"EmployeeCount_{year}"]);
                        cell1.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        cell1.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        cell1.Style.NumberFormat.Format = "#,##0;-#,##0;\"-\"";

                        colIndex++;
                    }
                    rowInd++;

                }
            }
            //số giờ công thường
            rowInd++;
            {
                colIndex = 3;
                foreach (var col in columnsName_soGioCong)
                {
                    var cell = ws_thang.Cell(rowInd, colIndex);
                    cell.Value = col;
                    cell.Style.Font.Bold = true;
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    cell.Style.Alignment.WrapText = true;
                    cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Đóng khung
                    cell.Style.Fill.BackgroundColor = Color_SoGioCongThuong;

                    colIndex++;
                }

                rowInd++;
                foreach (var workBlock in exportColumns)
                {
                    colIndex = 3;
                    var item = grouped.FirstOrDefault(x => Utils.RemoveVietnameseSigns(x["WorkBlock"].ToString()).Equals(workBlock, StringComparison.OrdinalIgnoreCase));
                    var cellworkBlock = ws_thang.Cell(rowInd, colIndex);
                    cellworkBlock.Value = Convert.ToString(item["WorkBlock"]);
                    cellworkBlock.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    cellworkBlock.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    colIndex++;
                    foreach (var year in mYear)
                    {

                        var cell2 = ws_thang.Cell(rowInd, colIndex);
                        cell2.Value = Convert.ToDecimal(item[$"WorkHours_{year}"]);
                        cell2.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        cell2.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        cell2.Style.NumberFormat.Format = "#,##0.0;-#,##0;\"-\"";
                        colIndex++;
                    }
                    rowInd++;

                }
            }
            //số giờ công tăng ca
            rowInd++;
            {
                colIndex = 3;
                foreach (var col in columnsName_soGioTangCa)
                {
                    var cell = ws_thang.Cell(rowInd, colIndex);
                    cell.Value = col;
                    cell.Style.Font.Bold = true;
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    cell.Style.Alignment.WrapText = true;
                    cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Đóng khung
                    cell.Style.Fill.BackgroundColor = Color_SoGioTangCa;

                    colIndex++;
                }

                rowInd++;
                foreach (var workBlock in exportColumns)
                {
                    colIndex = 3;
                    var item = grouped.FirstOrDefault(x => Utils.RemoveVietnameseSigns(x["WorkBlock"].ToString()).Equals(workBlock, StringComparison.OrdinalIgnoreCase));
                    var cellworkBlock = ws_thang.Cell(rowInd, colIndex);
                    cellworkBlock.Value = Convert.ToString(item["WorkBlock"]);
                    cellworkBlock.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    cellworkBlock.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    colIndex++;
                    foreach (var year in mYear)
                    {
                        var cell3 = ws_thang.Cell(rowInd, colIndex);
                        cell3.Value = Convert.ToDecimal(item[$"OvertimeHours_{year}"]);
                        cell3.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        cell3.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        cell3.Style.NumberFormat.Format = "#,##0.0;-#,##0;\"-\"";
                        colIndex++;
                    }
                    rowInd++;

                }
            }

            foreach (var col in ws_thang.ColumnsUsed())
            {
                col.Width = 10.3;
            }
            ws_thang.Column(3).Width = 20;
        }

    }
}
