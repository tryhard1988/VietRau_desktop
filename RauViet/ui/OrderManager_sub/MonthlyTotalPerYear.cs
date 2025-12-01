using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Vml;
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
    public partial class MonthlyTotalPerYear : Form
    {
        private LoadingOverlay loadingOverlay;
        DataTable mProductOrderHistory_dt;
        List<int> mYears;
        public MonthlyTotalPerYear()
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
                                                                .GroupBy(r => r.Field<int>("Month")) // group theo tháng
                                                                .Select(g =>
                                                                {
                                                                    var row = new Dictionary<string, object>();
                                                                    int month = g.Key;
                                                                    row["Month"] = month;
                                                                    row["MonthStr"] = $"Tháng {month.ToString("00")}";

                                                                    foreach (var year in mYears)
                                                                    {
                                                                        // Tổng cho tháng đó trong năm đó
                                                                        var total = g.Where(r => r.Field<int>("Year") == year)
                                                                                     .Sum(r =>r.Field<decimal>("TotalNetWeight"));
                                                                        var amount = g.Where(r => r.Field<int>("Year") == year)
                                                                                     .Sum(r => r.Field<decimal>("TotalAmountCHF"));

                                                                        row[$"{year}"] = total;
                                                                        row[$"amount_{year}"] = amount;
                                                                    }

                                                                    return row;
                                                                }).OrderBy(r => (int)r["Month"]).ToList();


                    mProductOrderHistory_dt.Columns.Add("Month", typeof(int));
                    mProductOrderHistory_dt.Columns.Add("MonthStr", typeof(string));
                    foreach (var year in mYears)
                    {
                        mProductOrderHistory_dt.Columns.Add($"{year}", typeof(decimal));
                        mProductOrderHistory_dt.Columns.Add($"amount_{year}", typeof(decimal));
                    }


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

                Color[] yearColors = new Color[]
                                                {
                                                    Color.LightBlue, Color.LightGreen, Color.LightYellow,
                                                    Color.LightCoral, Color.LightPink, Color.LightSalmon,
                                                    Color.LightCyan, Color.LightGoldenrodYellow, Color.LightGray,
                                                    Color.LightSkyBlue, Color.LightSeaGreen, Color.LightSteelBlue
                                                };

                foreach (var year in mYears)
                {
                    string yearkey = $"{year}";
                    string amountkey = $"amount_{year}";
                    var yearcol = product_GV.Columns[yearkey];
                    var amountcol = product_GV.Columns[amountkey];
                    yearcol.Width = 100;
                    yearcol.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    yearcol.DefaultCellStyle.Format = "N2";
                    yearcol.HeaderCell.Style.BackColor = yearColors[(year - 1) % 12];
                    yearcol.HeaderCell.Style.ForeColor = Color.Black;

                    amountcol.Width = 150;
                    amountcol.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    amountcol.DefaultCellStyle.Format = "N2";
                    amountcol.HeaderCell.Style.BackColor = yearColors[(year - 1) % 12];
                    amountcol.HeaderCell.Style.ForeColor = Color.Black;
                    amountcol.HeaderText = "Total Amount";
                }


                product_GV.Columns["Month"].Visible = false;
                product_GV.Columns["MonthStr"].HeaderText = "";
                product_GV.Columns["MonthStr"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
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

                sheet1_ws.Range(1, 1, 1, mProductOrderHistory_dt.Columns.Count - 1).Merge();
                sheet1_ws.Cell(1, 1).Value = "TỔNG SỐ LƯỢNG THEO THÁNG CỦA TỪNG NĂM";
                sheet1_ws.Cell(1, 1).Style.Font.Bold = true;
                sheet1_ws.Cell(1, 1).Style.Font.FontSize = 24;
                sheet1_ws.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                var cell1 = sheet1_ws.Cell(3, 1);
                cell1.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                int colIndex = 2;

                XLColor[] monthColors = new XLColor[]{XLColor.LightBlue, XLColor.LightGreen};
              
                foreach (var year in mYears)
                {
                    {
                        var cell = sheet1_ws.Cell(2, colIndex);
                        cell.Value = $"{year}";
                        cell.Style.Font.Bold = true;
                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        cell.Style.Fill.BackgroundColor = monthColors[colIndex % monthColors.Length];
                        colIndex++;
                    }

                    {
                        var cell = sheet1_ws.Cell(2, colIndex);
                        cell.Value = "Total Amount";
                        cell.Style.Font.Bold = true;
                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        cell.Style.Fill.BackgroundColor = monthColors[colIndex % monthColors.Length];
                        colIndex++;
                    }
                }

                int rowIndex = 3;

                Dictionary<string, decimal> totals = new Dictionary<string, decimal>();
                foreach (DataRow row in mProductOrderHistory_dt.Rows)
                {
                    {
                        var cell = sheet1_ws.Cell(rowIndex, 1);
                        cell.Value = row["MonthStr"].ToString();
                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    }
                    colIndex = 2;

                    foreach (var year in mYears)
                    {
                        {
                            string key = $"{year}";
                            decimal value = Convert.ToDecimal(row[key]);
                            var cell = sheet1_ws.Cell(rowIndex, colIndex);
                            cell.Value = value;
                            cell.Style.NumberFormat.Format = "#,##0.00;-#,##0.00;\"-\"";
                            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            colIndex++;

                            if (!totals.ContainsKey(key))
                            {
                                totals[key] = 0;
                            }
                            totals[key] += value;
                        }

                        {
                            string key = $"amount_{year}";
                            decimal value = Convert.ToDecimal(row[key]);
                            var cell = sheet1_ws.Cell(rowIndex, colIndex);
                            cell.Value = value;
                            cell.Style.NumberFormat.Format = "#,##0.00;-#,##0.00;\"-\"";
                            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            cell.Style.Fill.BackgroundColor = monthColors[colIndex % monthColors.Length];
                            colIndex++;

                            if (!totals.ContainsKey(key))
                            {
                                totals[key] = 0;
                            }
                            totals[key] += value;
                        }
                    }

                    rowIndex++;
                }

                colIndex = 2;
                foreach (var item in totals)
                {
                    var cell = sheet1_ws.Cell(rowIndex, colIndex);
                    cell.Value = item.Value;
                    cell.Style.Font.Bold = true;
                    cell.Style.NumberFormat.Format = "#,##0.00;-#,##0.00;\"-\"";
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    colIndex++;
                }

                sheet1_ws.Column(1).Width = 15;
                sheet1_ws.Row(1).Height = 45;
                sheet1_ws.Row(2).Height = 30;
                for (int i = 1; i < mProductOrderHistory_dt.Rows.Count; i++)
                    sheet1_ws.Column(i + 1).Width = 14;

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
