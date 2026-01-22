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

namespace RauViet.ui
{
    public partial class MonthlyReportForYear : Form
    {
        private LoadingOverlay loadingOverlay;
        private DataTable mProductOrderHistory_dt;
        public MonthlyReportForYear()
        {
            InitializeComponent();
            this.KeyPreview = true;

            timeReport_dtp.Format = DateTimePickerFormat.Custom;
            timeReport_dtp.CustomFormat = "yyyy";
            timeReport_dtp.ShowUpDown = true;
            timeReport_dtp.Value = DateTime.Now;

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            product_GV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            product_GV.MultiSelect = false;

            status_lb.Text = "";

            load_btn.Click += Load_btn_Click;
            this.KeyDown += ReportOrder_Year_KeyDown;
            product_GV.CellFormatting += product_GV_CellFormatting;

            exportToExcel_btn.Click += ExportToExcel_btn_Click;
        }

        private void ReportOrder_Year_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                int year = timeReport_dtp.Value.Year;                

                SQLStore_Kho.Instance.RemoveExportHistoryByYear(year);
                SQLStore_Kho.Instance.RemoveCustomerOrderDetailHistoryByYear(year);
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
                int year = timeReport_dtp.Value.Year;
                var customerOrderHistoryByYearTask = SQLStore_Kho.Instance.GetCustomerOrderDetailHistoryByYear(year);

                await Task.WhenAll(customerOrderHistoryByYearTask);
                mProductOrderHistory_dt = new DataTable();
                {
                    var grouped = customerOrderHistoryByYearTask.Result.AsEnumerable()
                                                                .GroupBy(row => new
                                                                {                                                                    
                                                                    ProductNameVN = row.Field<string>("ProductName_VN"),
                                                                  //  ProductNameEN = row.Field<string>("ProductName_EN"),
                                                                })
                                                                .Select(g => new
                                                                {
                                                                    ProductName_VN = g.Key.ProductNameVN,
                                                                    ProductName_EN = g.Select(r => r.Field<string>("ProductName_EN")).FirstOrDefault(en => !string.IsNullOrWhiteSpace(en)),
                                                                    priority = g.Min(r => r.Field<int?>("Priority") ?? int.MaxValue),

                                                                    Thang1 = g.Where(r => r.Field<int>("Month") == 1).Sum(r => {
                                                                        string package = r.Field<string>("Package")?.ToLower() ?? "";
                                                                        return (package == "kg" || package == "weight")? r.Field<decimal>("TotalNetWeight"): r.Field<int>("TotalPCS");
                                                                    }),
                                                                    Thang2 = g.Where(r => r.Field<int>("Month") == 2).Sum(r => {
                                                                        string package = r.Field<string>("Package")?.ToLower() ?? "";
                                                                        return (package == "kg" || package == "weight")? r.Field<decimal>("TotalNetWeight"): r.Field<int>("TotalPCS");
                                                                    }),
                                                                    Thang3 = g.Where(r => r.Field<int>("Month") == 3).Sum(r => {
                                                                        string package = r.Field<string>("Package")?.ToLower() ?? "";
                                                                        return (package == "kg" || package == "weight")? r.Field<decimal>("TotalNetWeight"): r.Field<int>("TotalPCS");
                                                                    }),
                                                                    Thang4 = g.Where(r => r.Field<int>("Month") == 4).Sum(r => {
                                                                        string package = r.Field<string>("Package")?.ToLower() ?? "";
                                                                        return (package == "kg" || package == "weight")? r.Field<decimal>("TotalNetWeight"): r.Field<int>("TotalPCS");
                                                                    }),
                                                                    Thang5 = g.Where(r => r.Field<int>("Month") == 5).Sum(r => {
                                                                        string package = r.Field<string>("Package")?.ToLower() ?? "";
                                                                        return (package == "kg" || package == "weight")? r.Field<decimal>("TotalNetWeight"): r.Field<int>("TotalPCS");
                                                                    }),
                                                                    Thang6 = g.Where(r => r.Field<int>("Month") == 6).Sum(r => {
                                                                        string package = r.Field<string>("Package")?.ToLower() ?? "";
                                                                        return (package == "kg" || package == "weight")? r.Field<decimal>("TotalNetWeight") : r.Field<int>("TotalPCS");
                                                                    }),
                                                                    Thang7 = g.Where(r => r.Field<int>("Month") == 7).Sum(r => {
                                                                        string package = r.Field<string>("Package")?.ToLower() ?? "";
                                                                        return (package == "kg" || package == "weight") ? r.Field<decimal>("TotalNetWeight") : r.Field<int>("TotalPCS");
                                                                    }),
                                                                    Thang8 = g.Where(r => r.Field<int>("Month") == 8).Sum(r => {
                                                                        string package = r.Field<string>("Package")?.ToLower() ?? "";
                                                                        return (package == "kg" || package == "weight") ? r.Field<decimal>("TotalNetWeight"): r.Field<int>("TotalPCS");
                                                                    }),
                                                                    Thang9 = g.Where(r => r.Field<int>("Month") == 9).Sum(r => {
                                                                        string package = r.Field<string>("Package")?.ToLower() ?? "";
                                                                        return (package == "kg" || package == "weight")? r.Field<decimal>("TotalNetWeight"): r.Field<int>("TotalPCS");
                                                                    }),
                                                                    Thang10 = g.Where(r => r.Field<int>("Month") == 10).Sum(r => {
                                                                        string package = r.Field<string>("Package")?.ToLower() ?? "";
                                                                        return (package == "kg" || package == "weight")? r.Field<decimal>("TotalNetWeight"): r.Field<int>("TotalPCS");
                                                                    }),
                                                                    Thang11 = g.Where(r => r.Field<int>("Month") == 11).Sum(r => {
                                                                        string package = r.Field<string>("Package")?.ToLower() ?? "";
                                                                        return (package == "kg" || package == "weight") ? r.Field<decimal>("TotalNetWeight"): r.Field<int>("TotalPCS");
                                                                    }),
                                                                    Thang12 = g.Where(r => r.Field<int>("Month") == 12).Sum(r => {
                                                                        string package = r.Field<string>("Package")?.ToLower() ?? "";
                                                                        return (package == "kg" || package == "weight")? r.Field<decimal>("TotalNetWeight"): r.Field<int>("TotalPCS");
                                                                    }),
                                                                    Total = g.Sum(r => {
                                                                        string package = r.Field<string>("Package")?.ToLower() ?? "";
                                                                        return (package == "kg" || package == "weight") ? r.Field<decimal>("TotalNetWeight"): r.Field<int>("TotalPCS");
                                                                    }),
                                                                    TotalAmountCHF = g.Sum(r => r.Field<decimal>("TotalAmountCHF"))
                                                                }).OrderBy(x=>x.priority).ThenBy(x=>x.ProductName_VN).ToList();

                    mProductOrderHistory_dt.Columns.Add("ProductName_VN", typeof(string));
                    mProductOrderHistory_dt.Columns.Add("ProductName_EN", typeof(string));

                    mProductOrderHistory_dt.Columns.Add("TotalQuanitity", typeof(decimal));
                    mProductOrderHistory_dt.Columns.Add("TotalAmountCHF", typeof(decimal));
                    for (int i = 1; i<= 12; i++)
                    {
                        mProductOrderHistory_dt.Columns.Add($"Thang{i}", typeof(decimal));
                    }
                   // productOrderHistoryByYear_dt.Columns.Add("TotalPCS", typeof(int));
                    

                    foreach (var item in grouped)
                    {
                        DataRow row = mProductOrderHistory_dt.NewRow();

                        row["ProductName_VN"] = item.ProductName_VN;
                        row["ProductName_EN"] = item.ProductName_EN;

                        row["Thang1"] = item.Thang1;
                        row["Thang2"] = item.Thang2;
                        row["Thang3"] = item.Thang3;
                        row["Thang4"] = item.Thang4;
                        row["Thang5"] = item.Thang5;
                        row["Thang6"] = item.Thang6;
                        row["Thang7"] = item.Thang7;
                        row["Thang8"] = item.Thang8;
                        row["Thang9"] = item.Thang9;
                        row["Thang10"] = item.Thang10;
                        row["Thang11"] = item.Thang11;
                        row["Thang12"] = item.Thang12;

                        row["TotalQuanitity"] = item.Total;

                        row["TotalAmountCHF"] = item.TotalAmountCHF;

                        mProductOrderHistory_dt.Rows.Add(row);
                    }
                }
   
                product_GV.DataSource = mProductOrderHistory_dt;
                Utils.HideColumns(product_GV, new[] { "ProductName_EN" });

                for (int i = 1; i <= 12; i++)
                {
                    string key = $"Thang{i}";
                    product_GV.Columns[key].HeaderText = $"Tháng {i}";
                    product_GV.Columns[key].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    product_GV.Columns[key].Width = 70;
                }

                Utils.SetGridHeaders(product_GV, new System.Collections.Generic.Dictionary<string, string> {
                    {"ProductName_VN", "Tên Sản Phẩm" },
                    {"ProductName_EN", "Product Name" },
                    {"TotalQuanitity", "Quanitity" },
                    {"TotalAmountCHF", "Thành Tiền" }
                });

                Utils.SetGridWidths(product_GV, new System.Collections.Generic.Dictionary<string, int> {
                    {"ProductName_VN", 150},
                    {"ProductName_EN", 130},
                    {"TotalQuanitity", 70},
                    {"TotalAmountCHF", 70}
                });

                product_GV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                product_GV.Columns["TotalAmountCHF"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                product_GV.Columns["TotalQuanitity"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                product_GV.Columns["TotalQuanitity"].DefaultCellStyle.Format = "N2";
                product_GV.Columns["TotalAmountCHF"].DefaultCellStyle.Format = "N2";

                
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
                sheet1_ws.Style.Font.FontName = "Tahoma";
                sheet1_ws.Style.Font.FontSize = 11;
                sheet1_ws.RowHeight = 16;
                sheet1_ws.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                sheet1_ws.Range(1, 1, 1, mProductOrderHistory_dt.Columns.Count - 1).Merge();
                sheet1_ws.Cell(1, 1).Value = "THỐNG KÊ SỐ LƯỢNG TỪNG MẶT HÀNG THEO THÁNG";
                sheet1_ws.Cell(1, 1).Style.Font.Bold = true;
                sheet1_ws.Cell(1, 1).Style.Font.FontSize = 24;
                sheet1_ws.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                sheet1_ws.Cell(1, 1).Style.Fill.BackgroundColor = XLColor.LightYellow;

                var cell1 = sheet1_ws.Cell(2, 1);
                cell1.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;                
                int colIndex = 2;

                XLColor[] monthColors = new XLColor[] { XLColor.LightBlue, XLColor.LightGreen };

                {                    
                    var cell = sheet1_ws.Cell(2, colIndex);
                    cell.Value = "Tổng SL";
                    cell.Style.Font.Bold = true;
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    cell.Style.Fill.BackgroundColor = monthColors[colIndex % monthColors.Length];
                    colIndex++;
                }
                {
                    var cell = sheet1_ws.Cell(2, colIndex);
                    cell.Value = "Amount CHF";
                    cell.Style.Font.Bold = true;
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    cell.Style.Fill.BackgroundColor = monthColors[colIndex % monthColors.Length];
                    colIndex++;
                }
                for (int i =0; i <12; i++)
                {
                    var cell = sheet1_ws.Cell(2, colIndex);
                    cell.Value = $"Tháng {(i+1).ToString("00")}";
                    cell.Style.Font.Bold = true;
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    cell.Style.Fill.BackgroundColor = XLColor.LightSalmon;
                    colIndex++;
                }

                int rowIndex = 3;

                Dictionary<string, decimal> totals = new Dictionary<string, decimal>();
                foreach (DataRow row in mProductOrderHistory_dt.Rows)
                {
                    colIndex = 1;
                    {
                        var cell = sheet1_ws.Cell(rowIndex, colIndex);
                        cell.Value = row["ProductName_VN"].ToString();
                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        colIndex++;
                    }
                    {
                        string key = $"TotalQuanitity";
                        decimal value = Convert.ToDecimal(row[key]);
                        var cell = sheet1_ws.Cell(rowIndex, colIndex);
                        cell.Value = value;
                        cell.Style.NumberFormat.Format = "#,##0.00;-#,##0.00;\"-\"";
                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        cell.Style.Fill.BackgroundColor = monthColors[colIndex % monthColors.Length];
                        colIndex++;
                    }
                    {
                        string key = $"TotalAmountCHF";
                        decimal value = Convert.ToDecimal(row[key]);
                        var cell = sheet1_ws.Cell(rowIndex, colIndex);
                        cell.Value = value;
                        cell.Style.NumberFormat.Format = "#,##0.00;-#,##0.00;\"-\"";
                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        cell.Style.Fill.BackgroundColor = monthColors[colIndex % monthColors.Length];
                        colIndex++;
                    }

                    for (int i = 0; i < 12; i++)
                    {
                        {
                            string key = $"Thang{i + 1}";
                            decimal value = Convert.ToDecimal(row[key]);
                            var cell = sheet1_ws.Cell(rowIndex, colIndex);
                            cell.Value = value;
                            cell.Style.NumberFormat.Format = "#,##0.00;-#,##0.00;\"-\"";
                            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;                            
                            colIndex++;
                        }
                    }

                    rowIndex++;
                }

                sheet1_ws.Row(1).Height = 45;
                sheet1_ws.Row(2).Height = 30;
                for (int i = 1; i < mProductOrderHistory_dt.Rows.Count; i++)
                    sheet1_ws.Column(i + 1).Width = 15;
                for(int i=3; i < rowIndex; i ++)
                    sheet1_ws.Row(i).Height = 16;

                sheet1_ws.Column(1).Width = 30;

                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = "Excel Workbook|*.xlsx";
                    sfd.FileName = "Gấm 1.xlsx";
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
