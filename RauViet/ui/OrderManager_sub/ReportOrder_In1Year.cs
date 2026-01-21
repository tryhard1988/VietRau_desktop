using RauViet.classes;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Color = System.Drawing.Color;

namespace RauViet.ui
{
    public partial class ReportOrder_In1Year : Form
    {
        private LoadingOverlay loadingOverlay;
        DataView mCustomerDetail_DV;
        public ReportOrder_In1Year()
        {
            InitializeComponent();
            this.KeyPreview = true;

            timeReport_dtp.Format = DateTimePickerFormat.Custom;
            timeReport_dtp.CustomFormat = "yyyy";
            timeReport_dtp.ShowUpDown = true;
            timeReport_dtp.Value = DateTime.Now;

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;
            product_GV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            product_GV.MultiSelect = false;
            CustomerGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            CustomerGV.MultiSelect = false;
            CustomerDetail_GV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            CustomerDetail_GV.MultiSelect = false;

            status_lb.Text = "";

            load_btn.Click += Load_btn_Click;
            this.KeyDown += ReportOrder_Year_KeyDown;
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
                var salarySummaryByYearTask = SQLStore_Kho.Instance.GetExportHistoryByYear(year);
                var customerOrderHistoryByYearTask = SQLStore_Kho.Instance.GetCustomerOrderDetailHistoryByYear(year);

                await Task.WhenAll(salarySummaryByYearTask, customerOrderHistoryByYearTask);
                DataTable salarySummaryByYear_dt = salarySummaryByYearTask.Result;
                DataTable productOrderHistoryByYear_dt = new DataTable();
                {
                    var grouped = customerOrderHistoryByYearTask.Result.AsEnumerable()
                                                                .GroupBy(row => new
                                                                {
                                                                    ProductNameVN = row.Field<string>("ProductName_VN")//,
                                                                  //  ProductNameEN = row.Field<string>("ProductName_EN")
                                                                })
                                                                .Select(g => new
                                                                {
                                                                    ProductName_VN = g.Key.ProductNameVN,
                                                                    ProductName_EN = g.Select(r => r.Field<string>("ProductName_EN")).FirstOrDefault(en => !string.IsNullOrWhiteSpace(en)),
                                                                    TotalNetWeight = g.Sum(r => r.Field<decimal>("TotalNetWeight")),
                                                                    TotalPCS = g.Sum(r => r.Field<int>("TotalPCS")),
                                                                    TotalAmountCHF = g.Sum(r => r.Field<decimal>("TotalAmountCHF")),
                                                                    Priority = g.Min(r=>r.Field<int>("Priority"))
                                                                }).OrderBy(x => x.Priority).ThenBy(x => x.ProductName_VN).ToList();

                    
                    productOrderHistoryByYear_dt.Columns.Add("ProductName_VN", typeof(string));
                    productOrderHistoryByYear_dt.Columns.Add("ProductName_EN", typeof(string));
                    productOrderHistoryByYear_dt.Columns.Add("TotalPCS", typeof(int));
                    productOrderHistoryByYear_dt.Columns.Add("TotalNetWeight", typeof(decimal));
                    productOrderHistoryByYear_dt.Columns.Add("TotalAmountCHF", typeof(decimal));

                    foreach (var item in grouped)
                    {
                        productOrderHistoryByYear_dt.Rows.Add(item.ProductName_VN, item.ProductName_EN, item.TotalPCS, item.TotalNetWeight, item.TotalAmountCHF);
                    }
                }
                DataTable customerOrderHistoryByYear_dt = new DataTable();
                {
                    var grouped = customerOrderHistoryByYearTask.Result.AsEnumerable()
                                                                .GroupBy(row => new
                                                                {
                                                                    CustomerName = row.Field<string>("CustomerName")
                                                                })
                                                                .Select(g => new
                                                                {
                                                                    CustomerName = g.Key.CustomerName,
                                                                    TotalNetWeight = g.Sum(r => r.Field<decimal>("TotalNetWeight")),
                                                                    TotalPCS = g.Sum(r => r.Field<int>("TotalPCS")),
                                                                    TotalAmountCHF = g.Sum(r => r.Field<decimal>("TotalAmountCHF"))
                                                                })
                                                                .ToList();


                    customerOrderHistoryByYear_dt.Columns.Add("CustomerName", typeof(string));
                    customerOrderHistoryByYear_dt.Columns.Add("TotalPCS", typeof(decimal));
                    customerOrderHistoryByYear_dt.Columns.Add("TotalNetWeight", typeof(decimal));
                    customerOrderHistoryByYear_dt.Columns.Add("TotalAmountCHF", typeof(decimal));

                    foreach (var item in grouped)
                    {
                        customerOrderHistoryByYear_dt.Rows.Add(item.CustomerName, item.TotalPCS, item.TotalNetWeight, item.TotalAmountCHF);
                    }
                }
                DataTable customerOrderDetail_dt = new DataTable();
                {
                    var grouped = customerOrderHistoryByYearTask.Result.AsEnumerable()
                                                                .GroupBy(row => new
                                                                {
                                                                    CustomerName = row.Field<string>("CustomerName"),
                                                                    ProductName_VN = row.Field<string>("ProductName_VN")//,
                                                                   // ProductName_EN = row.Field<string>("ProductName_EN")
                                                                })
                                                                .Select(g => new
                                                                {
                                                                    CustomerName = g.Key.CustomerName,
                                                                    ProductName_VN = g.Key.ProductName_VN,
                                                                    ProductName_EN = g.Select(r => r.Field<string>("ProductName_EN")).FirstOrDefault(en => !string.IsNullOrWhiteSpace(en)),
                                                                    TotalPCS = g.Sum(r => r.Field<int>("TotalPCS")),
                                                                    TotalNetWeight = g.Sum(r => r.Field<decimal>("TotalNetWeight")),
                                                                    TotalAmountCHF = g.Sum(r => r.Field<decimal>("TotalAmountCHF")),
                                                                    Priority = g.Min(r=>r.Field<int>("Priority"))
                                                                }).OrderBy(x=>x.Priority).ThenBy(x=>x.ProductName_VN).ToList();


                    customerOrderDetail_dt.Columns.Add("CustomerName", typeof(string));
                    customerOrderDetail_dt.Columns.Add("ProductName_VN", typeof(string));
                    customerOrderDetail_dt.Columns.Add("ProductName_EN", typeof(string));
                    customerOrderDetail_dt.Columns.Add("TotalPCS", typeof(int));
                    customerOrderDetail_dt.Columns.Add("TotalNetWeight", typeof(decimal));
                    customerOrderDetail_dt.Columns.Add("TotalAmountCHF", typeof(decimal));
                    customerOrderDetail_dt.Columns.Add("Priority", typeof(int));

                    foreach (var item in grouped)
                    {
                        customerOrderDetail_dt.Rows.Add(item.CustomerName, item.ProductName_VN, item.ProductName_EN, item.TotalPCS, item.TotalNetWeight, item.TotalAmountCHF, item.Priority);
                    }

                    mCustomerDetail_DV = new DataView(customerOrderDetail_dt);
                }

                dataGV.DataSource = salarySummaryByYear_dt;
                product_GV.DataSource = productOrderHistoryByYear_dt;
                CustomerGV.DataSource = customerOrderHistoryByYear_dt;
                CustomerDetail_GV.DataSource = mCustomerDetail_DV;

                Utils.HideColumns(dataGV, new[] { "ExportHistoryID" });
                Utils.HideColumns(CustomerDetail_GV, new[] { "CustomerName", "ProductName_EN", "Priority" });

                CustomerDetail_GV.Columns["ProductName_VN"].HeaderText = "Tên Sản Phẩm";
                CustomerDetail_GV.Columns["ProductName_EN"].HeaderText = "Product Name";
                CustomerDetail_GV.Columns["TotalNetWeight"].HeaderText = "Net Weight";
                CustomerDetail_GV.Columns["TotalAmountCHF"].HeaderText = "Thành Tiền";
                CustomerDetail_GV.Columns["TotalPCS"].HeaderText = "PCS";

                int count = 0;
                salarySummaryByYear_dt.Columns["ExportCode"].SetOrdinal(count++);
                salarySummaryByYear_dt.Columns["ExportDate"].SetOrdinal(count++);
                salarySummaryByYear_dt.Columns["TotalMoney"].SetOrdinal(count++);
                salarySummaryByYear_dt.Columns["TotalNW"].SetOrdinal(count++);
                salarySummaryByYear_dt.Columns["NumberCarton"].SetOrdinal(count++);
                salarySummaryByYear_dt.Columns["FreightCharge"].SetOrdinal(count++);

                dataGV.Columns["ExportCode"].HeaderText = "Mã Xuất Cảng";
                dataGV.Columns["ExportDate"].HeaderText = "Ngày Xuất Cảng";
                dataGV.Columns["TotalMoney"].HeaderText = "Tổng Tiền Hàng";
                dataGV.Columns["TotalNW"].HeaderText = "Tổng Trọng Lượng";
                dataGV.Columns["NumberCarton"].HeaderText = "Tổng Số Thùng";
                dataGV.Columns["FreightCharge"].HeaderText = "Phí Vận Chuyển";

                int sizeWidth = 70;
                dataGV.Columns["ExportCode"].Width = 110;
                dataGV.Columns["ExportDate"].Width = sizeWidth;
                dataGV.Columns["TotalMoney"].Width = sizeWidth;
                dataGV.Columns["TotalNW"].Width = sizeWidth;
                dataGV.Columns["NumberCarton"].Width = sizeWidth;
                dataGV.Columns["FreightCharge"].Width = sizeWidth;

               // product_GV.Columns["ProductName_EN"].Visible = false;
                product_GV.Columns["ProductName_VN"].HeaderText = "Tên Sản Phẩm";
                product_GV.Columns["ProductName_EN"].HeaderText = "Product Name";
                product_GV.Columns["TotalNetWeight"].HeaderText = "Net Weight";
                product_GV.Columns["TotalAmountCHF"].HeaderText = "Thành Tiền";
                product_GV.Columns["TotalPCS"].HeaderText = "PCS";
                product_GV.Columns["ProductName_VN"].Width = 150;
                product_GV.Columns["ProductName_EN"].Width = 130;
                product_GV.Columns["TotalNetWeight"].Width = 70;
                product_GV.Columns["TotalAmountCHF"].Width = 70;

                
                CustomerGV.Columns["CustomerName"].HeaderText = "Tên Sản Phẩm";
                CustomerGV.Columns["TotalNetWeight"].HeaderText = "Net Weight";
                CustomerGV.Columns["TotalAmountCHF"].HeaderText = "Thành Tiền";
                CustomerGV.Columns["TotalPCS"].HeaderText = "PCS";
                CustomerGV.Columns["CustomerName"].Width = 100;
                CustomerGV.Columns["TotalNetWeight"].Width = 80;
                CustomerGV.Columns["TotalAmountCHF"].Width = 80;

                decimal totalmoney = salarySummaryByYear_dt.AsEnumerable().Sum(r => r.Field<decimal>("TotalMoney"));
                decimal totalNW = salarySummaryByYear_dt.AsEnumerable().Sum(r => r.Field<decimal>("TotalNW"));
                int totalCarton = salarySummaryByYear_dt.AsEnumerable().Sum(r => r.Field<int>("NumberCarton"));
                decimal totalFreightCharge = salarySummaryByYear_dt.AsEnumerable().Sum(r => r.Field<decimal>("FreightCharge"));
                totalMoney_tb.Text = totalmoney.ToString("N2");
                totalNW_tb.Text = totalNW.ToString("N2");
                totalCarton_tb.Text = (totalCarton).ToString();
                freightCharge_tb.Text = (totalFreightCharge).ToString("N2");

                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                product_GV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                CustomerGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                CustomerDetail_GV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                product_GV.Columns["TotalAmountCHF"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                product_GV.Columns["TotalNetWeight"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                product_GV.Columns["TotalPCS"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                CustomerGV.Columns["TotalAmountCHF"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                CustomerGV.Columns["TotalNetWeight"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                CustomerGV.Columns["TotalPCS"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                CustomerDetail_GV.Columns["TotalAmountCHF"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                CustomerDetail_GV.Columns["TotalNetWeight"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                CustomerDetail_GV.Columns["TotalPCS"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGV.Columns["NumberCarton"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGV.Columns["TotalMoney"].DefaultCellStyle.Format = "N2";
                dataGV.Columns["TotalNW"].DefaultCellStyle.Format = "N2";
                dataGV.Columns["FreightCharge"].DefaultCellStyle.Format = "N2";
                product_GV.Columns["TotalNetWeight"].DefaultCellStyle.Format = "N2";
                product_GV.Columns["TotalAmountCHF"].DefaultCellStyle.Format = "N2";
                CustomerGV.Columns["TotalNetWeight"].DefaultCellStyle.Format = "N2";
                CustomerGV.Columns["TotalAmountCHF"].DefaultCellStyle.Format = "N2";

                CustomerGV.CellClick -= CustomerGV_CellClick;
                CustomerGV.CellClick += CustomerGV_CellClick;

                CustomerGV_CellClick(null, null);
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

        private void CustomerGV_CellClick(object sender, EventArgs e)
        {
            if (CustomerGV.CurrentRow == null) return;
            string cusName = CustomerGV.CurrentRow.Cells["CustomerName"].Value?.ToString() ?? "";
            string safeName = cusName.Replace("'", "''");

            mCustomerDetail_DV.RowFilter = $"CustomerName = '{safeName}'";
            mCustomerDetail_DV.Sort = "Priority ASC, ProductName_VN ASC";
        }

        private async void Load_btn_Click(object sender, EventArgs e)
        {
            ShowData();
        }

    }
}
