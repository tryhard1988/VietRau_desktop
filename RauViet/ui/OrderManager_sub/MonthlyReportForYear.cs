using DocumentFormat.OpenXml.Bibliography;
using RauViet.classes;
using System;
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
        }

        private void ReportOrder_Year_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                int year = timeReport_dtp.Value.Year;                

                SQLStore.Instance.RemoveExportHistoryByYear(year);
                SQLStore.Instance.RemoveCustomerOrderDetailHistoryByYear(year);
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
                var customerOrderHistoryByYearTask = SQLStore.Instance.GetCustomerOrderDetailHistoryByYear(year);

                await Task.WhenAll(customerOrderHistoryByYearTask);
                DataTable productOrderHistoryByYear_dt = new DataTable();
                {
                    var grouped = customerOrderHistoryByYearTask.Result.AsEnumerable()
                                                                .GroupBy(row => new
                                                                {                                                                    
                                                                    ProductNameVN = row.Field<string>("ProductName_VN"),
                                                                    ProductNameEN = row.Field<string>("ProductName_EN"),
                                                                })
                                                                .Select(g => new
                                                                {
                                                                    ProductName_VN = g.Key.ProductNameVN,
                                                                    ProductName_EN = g.Key.ProductNameEN,
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
                                                                }).ToList();

                    productOrderHistoryByYear_dt.Columns.Add("ProductName_VN", typeof(string));
                    productOrderHistoryByYear_dt.Columns.Add("ProductName_EN", typeof(string));
                    
                    productOrderHistoryByYear_dt.Columns.Add("TotalQuanitity", typeof(decimal));
                    productOrderHistoryByYear_dt.Columns.Add("TotalAmountCHF", typeof(decimal));
                    for (int i = 1; i<= 12; i++)
                    {
                        productOrderHistoryByYear_dt.Columns.Add($"Thang{i}", typeof(decimal));
                    }
                   // productOrderHistoryByYear_dt.Columns.Add("TotalPCS", typeof(int));
                    

                    foreach (var item in grouped)
                    {
                        DataRow row = productOrderHistoryByYear_dt.NewRow();

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

                        productOrderHistoryByYear_dt.Rows.Add(row);
                    }
                }
   
                product_GV.DataSource = productOrderHistoryByYear_dt;

                for (int i = 1; i <= 12; i++)
                {
                    string key = $"Thang{i}";
                    product_GV.Columns[key].HeaderText = $"Tháng {i}";
                    product_GV.Columns[key].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    product_GV.Columns[key].Width = 70;
                }

                product_GV.Columns["ProductName_EN"].Visible = false;
                product_GV.Columns["ProductName_VN"].HeaderText = "Tên Sản Phẩm";
                product_GV.Columns["ProductName_EN"].HeaderText = "Product Name";
                product_GV.Columns["TotalQuanitity"].HeaderText = "Quanitity";
                product_GV.Columns["TotalAmountCHF"].HeaderText = "Thành Tiền";
                product_GV.Columns["ProductName_VN"].Width = 150;
                product_GV.Columns["ProductName_EN"].Width = 130;
                product_GV.Columns["TotalQuanitity"].Width = 70;
                product_GV.Columns["TotalAmountCHF"].Width = 70;

                product_GV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                product_GV.Columns["TotalAmountCHF"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                product_GV.Columns["TotalQuanitity"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                product_GV.Columns["TotalQuanitity"].DefaultCellStyle.Format = "N2";
                product_GV.Columns["TotalAmountCHF"].DefaultCellStyle.Format = "N2";
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

    }
}
