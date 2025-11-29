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
    public partial class YearlyReport : Form
    {
        private LoadingOverlay loadingOverlay;
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
                var years = customerOrderHistoryTask.Result.AsEnumerable().Select(r => r.Field<int>("Year")).Distinct().OrderBy(y => y).ToList();
                DataTable productOrderHistory_dt = new DataTable();
                {
                    

                    var result = customerOrderHistoryTask.Result.AsEnumerable()
                                                        .GroupBy(r => new
                                                        {
                                                            ProductName_VN = r.Field<string>("ProductName_VN"),
                                                            ProductName_EN = r.Field<string>("ProductName_EN")
                                                        })
                                                        .Select(g =>
                                                        {
                                                            var row = new Dictionary<string, object>();

                                                            row["ProductName_VN"] = g.Key.ProductName_VN;
                                                            row["ProductName_EN"] = g.Key.ProductName_EN;

                                                            
                                                            for (int month = 1; month <= 12; month++)
                                                            {
                                                                foreach (var year in years)
                                                                {
                                                                    var total = g.Where(r => r.Field<int>("Year") == year &&
                                                                                             r.Field<int>("Month") == month)
                                                                                 .Sum(r => {
                                                                                     string package = r.Field<string>("Package")?.ToLower() ?? "";
                                                                                     return (package == "kg" || package == "weight") ? r.Field<decimal>("TotalNetWeight") : r.Field<int>("TotalPCS");
                                                                                 });

                                                                    row[$"{month}/{year}"] = total;
                                                                }
                                                            }
                                                            foreach (var year in years)
                                                            {
                                                                var total = g.Where(r => r.Field<int>("Year") == year)
                                                                             .Sum(r => {
                                                                                 string package = r.Field<string>("Package")?.ToLower() ?? "";
                                                                                 return (package == "kg" || package == "weight") ? r.Field<decimal>("TotalNetWeight") : r.Field<int>("TotalPCS");
                                                                             });

                                                                row[$"{year}"] = total;
                                                            }

                                                            return row;
                                                        })
                                                        .ToList();


                    productOrderHistory_dt.Columns.Add("ProductName_VN", typeof(string));
                    productOrderHistory_dt.Columns.Add("ProductName_EN", typeof(string));

                    foreach (var year in years)
                    {
                        productOrderHistory_dt.Columns.Add($"{year}", typeof(decimal));
                    }

                    for (int month = 1; month <= 12; month++)
                    {
                        foreach (var year in years)
                        {
                            productOrderHistory_dt.Columns.Add($"{month}/{year}", typeof(decimal));
                        }
                    }
                    // productOrderHistoryByYear_dt.Columns.Add("TotalPCS", typeof(int));


                    foreach (var item in result)
                    {
                        var dr = productOrderHistory_dt.NewRow();
                        foreach (var key in item.Keys)
                            dr[key] = item[key];
                        productOrderHistory_dt.Rows.Add(dr);
                    }
                }
   
                product_GV.DataSource = productOrderHistory_dt;

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
                    foreach (var year in years)
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

                foreach (var year in years)
                {
                    string key = $"{year}";
                    var col = product_GV.Columns[key];
                    col.Width = 60;
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    col.DefaultCellStyle.Format = "N2";
                    col.HeaderCell.Style.BackColor = Color.DarkRed;
                    col.HeaderCell.Style.ForeColor = Color.White;
                }

                //product_GV.Columns["ProductName_EN"].Visible = false;
                product_GV.Columns["ProductName_VN"].HeaderText = "Tên Sản Phẩm";
                product_GV.Columns["ProductName_VN"].Width = 180;
                product_GV.Columns["ProductName_VN"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                product_GV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                // for (int i = 1; i <= 12; i++)
                // {
                //     string key = $"Thang{i}";
                //     product_GV.Columns[key].HeaderText = $"Tháng {i}";
                //     product_GV.Columns[key].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                //     product_GV.Columns[key].Width = 70;
                // }

                //// product_GV.Columns["ProductName_EN"].Visible = false;
                // product_GV.Columns["ProductName_VN"].HeaderText = "Tên Sản Phẩm";
                // product_GV.Columns["ProductName_EN"].HeaderText = "Product Name";
                // product_GV.Columns["TotalQuanitity"].HeaderText = "Quanitity";
                // product_GV.Columns["TotalAmountCHF"].HeaderText = "Thành Tiền";
                // product_GV.Columns["ProductName_VN"].Width = 150;
                // product_GV.Columns["ProductName_EN"].Width = 130;
                // product_GV.Columns["TotalQuanitity"].Width = 70;
                // product_GV.Columns["TotalAmountCHF"].Width = 70;

                // product_GV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                // product_GV.Columns["TotalAmountCHF"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                // product_GV.Columns["TotalQuanitity"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                // product_GV.Columns["TotalQuanitity"].DefaultCellStyle.Format = "N2";
                // product_GV.Columns["TotalAmountCHF"].DefaultCellStyle.Format = "N2";
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

    }
}
