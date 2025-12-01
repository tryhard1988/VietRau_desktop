using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Vml;
using DocumentFormat.OpenXml.Wordprocessing;
using MySqlX.XDevAPI.Common;
using RauViet.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Color = System.Drawing.Color;

namespace RauViet.ui
{
    public partial class YearlyReport : Form
    {
        private LoadingOverlay loadingOverlay;
        DataTable mProductOrderHistory_dt;
        List<int> mYears;

        public YearlyReport()
        {
            InitializeComponent();
            this.KeyPreview = true;

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            //product_GV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            product_GV.MultiSelect = false;

            this.KeyDown += ReportOrder_Year_KeyDown;
            product_GV.CellFormatting += product_GV_CellFormatting;

            exportToExcel_btn.Click += ExportToExcel_btn_Click;
        }

        private void ReportOrder_Year_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                SQLStore.Instance.RemoveCustomerOrderDetailHistory();
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
                var customerOrderHistoryTask = SQLStore.Instance.GetCustomerOrderDetailHistory();
                await Task.WhenAll(customerOrderHistoryTask);

                mYears = customerOrderHistoryTask.Result.AsEnumerable().Select(r => r.Field<int>("Year")).Distinct().OrderBy(y => y).ToList();
                mProductOrderHistory_dt = new DataTable();
                {


                    var result = customerOrderHistoryTask.Result.AsEnumerable()
                                                        .GroupBy(r => new
                                                        {
                                                            ProductName_VN = r.Field<string>("ProductName_VN")
                                                            // ProductName_EN = r.Field<string>("ProductName_EN")
                                                        })
                                                        .Select(g =>
                                                        {
                                                            var row = new Dictionary<string, object>();

                                                            row["ProductName_VN"] = g.Key.ProductName_VN;
                                                            row["ProductName_EN"] = g.First().Field<string>("ProductName_EN");

                                                            int priority = g.Min(r => r.Field<int?>("Priority") ?? int.MaxValue);
                                                            row["Priority"] = priority;

                                                            for (int month = 1; month <= 12; month++)
                                                            {
                                                                foreach (var year in mYears)
                                                                {
                                                                    var total = g.Where(r => r.Field<int>("Year") == year &&
                                                                                             r.Field<int>("Month") == month)
                                                                                 .Sum(r =>
                                                                                 {
                                                                                     string package = r.Field<string>("Package")?.ToLower() ?? "";
                                                                                     return (package == "kg" || package == "weight") ? r.Field<decimal>("TotalNetWeight") : r.Field<int>("TotalPCS");
                                                                                 });

                                                                    row[$"{month}/{year}"] = total;
                                                                }
                                                            }
                                                            foreach (var year in mYears)
                                                            {
                                                                var total = g.Where(r => r.Field<int>("Year") == year)
                                                                             .Sum(r =>
                                                                             {
                                                                                 string package = r.Field<string>("Package")?.ToLower() ?? "";
                                                                                 return (package == "kg" || package == "weight") ? r.Field<decimal>("TotalNetWeight") : r.Field<int>("TotalPCS");
                                                                             });

                                                                row[$"{year}"] = total;
                                                            }

                                                            return row;
                                                        }).OrderBy(r => (int)r["Priority"]).ThenBy(r => r["ProductName_VN"]?.ToString()).ToList();


                    mProductOrderHistory_dt.Columns.Add("ProductName_VN", typeof(string));
                    mProductOrderHistory_dt.Columns.Add("ProductName_EN", typeof(string));
                    mProductOrderHistory_dt.Columns.Add("Priority", typeof(int));

                    foreach (var year in mYears)
                    {
                        mProductOrderHistory_dt.Columns.Add($"{year}", typeof(decimal));
                    }

                    for (int month = 1; month <= 12; month++)
                    {
                        foreach (var year in mYears)
                        {
                            mProductOrderHistory_dt.Columns.Add($"{month}/{year}", typeof(decimal));
                        }
                    }
                    // productOrderHistoryByYear_dt.Columns.Add("TotalPCS", typeof(int));


                    foreach (var item in result)
                    {
                        var dr = mProductOrderHistory_dt.NewRow();
                        foreach (var key in item.Keys)
                            dr[key] = item[key];
                        mProductOrderHistory_dt.Rows.Add(dr);
                    }
                }

                product_GV.DataSource = mProductOrderHistory_dt;

                product_GV.EnableHeadersVisualStyles = false;

                Color[] monthColors = new Color[]
                                                {
                                                    Color.LightBlue, Color.LightGreen, Color.LightYellow,
                                                    Color.LightCoral, Color.LightPink, Color.LightSalmon,
                                                    Color.LightCyan, Color.LightGoldenrodYellow, Color.LightGray,
                                                    Color.LightSkyBlue, Color.LightSeaGreen, Color.LightSteelBlue
                                                };

                for (int month = 1; month <= 12; month++)
                {
                    foreach (var year in mYears)
                    {
                        string key = $"{month}/{year}";
                        var col = product_GV.Columns[key];
                        col.Width = 50;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        col.DefaultCellStyle.Format = "N2";
                        col.HeaderCell.Style.BackColor = monthColors[(month - 1) % 12];
                        col.HeaderCell.Style.ForeColor = Color.Black;
                    }
                }//

                foreach (var year in mYears)
                {
                    string key = $"{year}";
                    var col = product_GV.Columns[key];
                    col.Width = 60;
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    col.DefaultCellStyle.Format = "N2";
                    col.HeaderCell.Style.BackColor = Color.DarkRed;
                    col.HeaderCell.Style.ForeColor = Color.White;
                }

                product_GV.Columns["ProductName_EN"].Visible = false;
                product_GV.Columns["Priority"].Visible = false;
                product_GV.Columns["ProductName_VN"].HeaderText = "Tên Sản Phẩm";
                product_GV.Columns["ProductName_VN"].Width = 180;
                product_GV.Columns["ProductName_VN"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                product_GV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            catch (Exception ex)
            {
            }
            finally
            {
                await Task.Delay(200);
                loadingOverlay.Hide();
            }
        }

        private async void Load_btn_Click(object sender, EventArgs e)
        {
            ShowData();
        }

        private void product_GV_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value != null)
            {
                // Nếu là số
                if (decimal.TryParse(e.Value.ToString(), out decimal val))
                {
                    if (val == 0)
                    {
                        e.Value = "-";
                        e.FormattingApplied = true;
                    }
                }
            }
        }

        private void ExportToExcel_btn_Click(object sender, EventArgs e)
        {
            using (var wb = new XLWorkbook())
            {
                var sheet1_ws = wb.Worksheets.Add("Sheet1");
                sheet1_ws.Style.Font.FontName = "Arial";
                sheet1_ws.Style.Font.FontSize = 9;
                sheet1_ws.RowHeight = 16;
                sheet1_ws.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                sheet1_ws.Range(1, 1, 1, mProductOrderHistory_dt.Columns.Count - 2).Merge();
                sheet1_ws.Cell(1, 1).Value = "THỐNG KÊ SỐ LƯỢNG TỪNG MẶT HÀNG THEO THÁNG TỪNG NĂM";
                sheet1_ws.Cell(1, 1).Style.Font.Bold = true;
                sheet1_ws.Cell(1, 1).Style.Font.FontSize = 36;
                sheet1_ws.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                int colIndex = 1;

                {
                    var range = sheet1_ws.Range(2, colIndex, 3, colIndex);
                    range.Merge();
                    var cell = sheet1_ws.Cell(2, colIndex);
                    cell.Value = "Tên Hàng";
                    cell.Style.Font.Bold = true;
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    colIndex++;
                }

                int yearCount = mYears.Count;
                int yearIndex = 0;
                {
                    var range = sheet1_ws.Range(2, colIndex, 2, colIndex + yearCount - 1);
                    range.Merge();
                    var cell = sheet1_ws.Cell(2, colIndex);
                    cell.Value = "Tổng Số Lượng";
                    cell.Style.Font.Bold = true;
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    cell.Style.Fill.BackgroundColor = XLColor.LightPink;

                    foreach (var year in mYears)
                    {
                        cell = sheet1_ws.Cell(3, colIndex + yearIndex);
                        cell.Value = $"{year}";
                        cell.Style.Font.Bold = true;
                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        cell.Style.Fill.BackgroundColor = XLColor.LightPink;
                        yearIndex++;
                    }

                    colIndex += yearCount;
                }

                XLColor[] monthColors = new XLColor[]
                                                {
                                                    XLColor.LightBlue, XLColor.LightGreen, XLColor.LightYellow,
                                                    XLColor.LightCoral, XLColor.LightPink, XLColor.LightSalmon,
                                                    XLColor.LightCyan, XLColor.LightGoldenrodYellow, XLColor.LightGray,
                                                    XLColor.LightSkyBlue, XLColor.LightSeaGreen, XLColor.LightSteelBlue
                                                };

                for (int i = 0; i < 12; i++)
                {
                    {
                        var range = sheet1_ws.Range(2, colIndex, 2, colIndex + yearCount - 1);
                        range.Merge();
                        var cell = sheet1_ws.Cell(2, colIndex);
                        cell.Value = $"Tháng {(i + 1).ToString("00")}";
                        cell.Style.Font.Bold = true;
                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        cell.Style.Fill.BackgroundColor = monthColors[i];
                    }
                    yearIndex = 0;
                    foreach (var year in mYears)
                    {
                        var cell = sheet1_ws.Cell(3, colIndex + yearIndex);
                        cell.Value = $"{year}";
                        cell.Style.Font.Bold = true;
                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        cell.Style.Fill.BackgroundColor = monthColors[i];
                        yearIndex++;
                    }


                    colIndex += yearCount;
                }


                int rowIndex = 4;

                foreach (DataRow row in mProductOrderHistory_dt.Rows)
                {
                    colIndex = 1;

                    {
                        var cell = sheet1_ws.Cell(rowIndex, colIndex);
                        cell.Value = row["ProductName_VN"].ToString();
                        cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        colIndex++;
                    }

                    foreach (var year in mYears)
                    {
                        var cell = sheet1_ws.Cell(rowIndex, colIndex);
                        cell.Value = Convert.ToDecimal(row[$"{year}"]);
                        cell.Style.NumberFormat.Format = "#,##0.00;-#,##0.00;\"-\"";
                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        colIndex++;
                    }

                    for (int i = 0; i < 12; i++)
                    {
                        foreach (var year in mYears)
                        {
                            var cell = sheet1_ws.Cell(rowIndex, colIndex);
                            cell.Style.NumberFormat.Format = "#,##0.00;-#,##0.00;\"-\"";
                            cell.Value = Convert.ToDecimal(row[$"{i + 1}/{year}"]);
                            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            colIndex++;
                        }
                    }
                    rowIndex++;
                }

                sheet1_ws.Column(1).Width = 30;
                sheet1_ws.Row(1).Height = 56;
                sheet1_ws.Row(2).Height = 20;
                sheet1_ws.Row(3).Height = 15;
                for (int i = 2; i < mProductOrderHistory_dt.Rows.Count; i++)
                    sheet1_ws.Column(i+1).Width = 10.43;

                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = "Excel Workbook|*.xlsx";
                    sfd.FileName = "excel.xlsx";
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
    }
}
