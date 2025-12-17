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
    public partial class ResoncileDomesticDebts_Month : Form
    {
        private LoadingOverlay loadingOverlay;
        DataTable mDetail_dt, mSummary_dt;
        DataView mDetail_DV, mSummary_DV;
        public ResoncileDomesticDebts_Month()
        {
            InitializeComponent();
            this.KeyPreview = true;

            timeReport_dtp.Format = DateTimePickerFormat.Custom;
            timeReport_dtp.CustomFormat = "MM/yyyy";
            timeReport_dtp.ShowUpDown = true;
            timeReport_dtp.Value = DateTime.Now;

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;
            sumary_GV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            sumary_GV.MultiSelect = false;
            detailGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            detailGV.MultiSelect = false;

            status_lb.Text = "";

            load_btn.Click += Load_btn_Click;
            this.KeyDown += ReportOrder_Year_KeyDown;

            preview_print_TD_btn.Click += Preview_print_TD_btn_Click;
            print_btn.Click += Print_btn_Click;
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

            dataGV.CellClick -= CustomerGV_CellClick;

            try
            {
                int year = timeReport_dtp.Value.Year;
                int month = timeReport_dtp.Value.Month;
                var customerOrderHistoryByYearTask = SQLStore_Kho.Instance.GetOrderDomesticByMonthYearAsync(month, year);

                await Task.WhenAll(customerOrderHistoryByYearTask);
                mDetail_dt = customerOrderHistoryByYearTask.Result;
                DataTable customer_dt = new DataTable();
                {
                    var query = customerOrderHistoryByYearTask.Result
                                                                .AsEnumerable()
                                                                .GroupBy(r => r.Field<int>("CustomerID"))
                                                                .Select(g => new
                                                                {
                                                                    CustomerID = g.Key,
                                                                    Company = g.Select(r => r.Field<string>("Company")).FirstOrDefault(c => !string.IsNullOrWhiteSpace(c))
                                                                });

                    customer_dt.Columns.Add("CustomerID", typeof(int));
                    customer_dt.Columns.Add("Company", typeof(string));

                    foreach (var item in query)
                    {
                        customer_dt.Rows.Add(item.CustomerID, item.Company);
                    }
                }

                mSummary_dt = new DataTable();
                {
                    var query = customerOrderHistoryByYearTask.Result
                                                            .AsEnumerable()
                                                            .GroupBy(r => new
                                                            {
                                                                CustomerID = r.Field<int>("CustomerID"),
                                                                ProductNameVN = r.Field<string>("ProductNameVN"),
                                                                ProductTypeName = r.Field<string>("ProductTypeName")
                                                            })
                                                            .Select(g => new
                                                            {
                                                                CustomerID = g.Key.CustomerID,
                                                                ProductNameVN = g.Key.ProductNameVN,
                                                                ProductTypeName = g.Key.ProductTypeName,
                                                                PCSReal = g.Sum(r => r.Field<int?>("PCSReal") ?? 0),
                                                                NWReal = g.Sum(r => r.Field<decimal?>("NWReal") ?? 0m),
                                                                Price = g.Select(r => r.Field<int?>("Price")).FirstOrDefault(p => p.HasValue) ?? 0,
                                                                TotalAmount = g.Sum(r => r.Field<int?>("TotalAmount") ?? 0)
                                                            }).OrderBy(x => x.ProductNameVN);

                    mSummary_dt.Columns.Add("CustomerID", typeof(int));
                    mSummary_dt.Columns.Add("ProductNameVN", typeof(string));
                    mSummary_dt.Columns.Add("ProductTypeName", typeof(string));
                    mSummary_dt.Columns.Add("PCSReal", typeof(int));
                    mSummary_dt.Columns.Add("NWReal", typeof(decimal));
                    mSummary_dt.Columns.Add("Price", typeof(decimal));
                    mSummary_dt.Columns.Add("TotalAmount", typeof(decimal));

                    foreach (var item in query)
                    {
                        mSummary_dt.Rows.Add(item.CustomerID, item.ProductNameVN, item.ProductTypeName, item.PCSReal, item.NWReal, item.Price, item.TotalAmount);
                    }
                }


                dataGV.DataSource = customer_dt;

                mDetail_DV = new DataView(mDetail_dt);
                detailGV.DataSource = mDetail_DV;

                mSummary_DV = new DataView(mSummary_dt);
                sumary_GV.DataSource = mSummary_DV;



                dataGV.Columns["CustomerID"].Visible = false;
                dataGV.Columns["Company"].HeaderText = "Khách Hàng";
                dataGV.Columns["Company"].Width = 160;

                sumary_GV.Columns["CustomerID"].Visible = false;
                sumary_GV.Columns["ProductNameVN"].Width = 110;
                sumary_GV.Columns["ProductTypeName"].Width = 85;
                sumary_GV.Columns["PCSReal"].Width = 50;
                sumary_GV.Columns["NWReal"].Width = 50;
                sumary_GV.Columns["Price"].Width = 60;
                sumary_GV.Columns["TotalAmount"].Width = 80;
                sumary_GV.Columns["ProductNameVN"].HeaderText = "Tên Sản Phẩm";
                sumary_GV.Columns["ProductTypeName"].HeaderText = "Loại Hàng";
                sumary_GV.Columns["PCSReal"].HeaderText = "PCS";
                sumary_GV.Columns["NWReal"].HeaderText = "N.W";
                sumary_GV.Columns["Price"].HeaderText = "Giá";
                sumary_GV.Columns["Price"].DefaultCellStyle.Format = "N0";
                sumary_GV.Columns["TotalAmount"].DefaultCellStyle.Format = "N0";
                sumary_GV.Columns["PCSReal"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                sumary_GV.Columns["NWReal"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                sumary_GV.Columns["Price"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                sumary_GV.Columns["TotalAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                detailGV.Columns["ProductPackingID"].Visible = false;
                detailGV.Columns["CustomerProductTypesCode"].Visible = false;
                detailGV.Columns["SKU"].Visible = false;
                detailGV.Columns["Company"].Visible = false;
                detailGV.Columns["CustomerID"].Visible = false;
                detailGV.Columns["Amount"].Visible = false;
                detailGV.Columns["packing"].Visible = false;
                detailGV.Columns["ProductNameVN"].HeaderText = "Tên Sản Phẩm";
                detailGV.Columns["ProductTypeName"].HeaderText = "Loại Hàng";
                detailGV.Columns["Package"].HeaderText = "Đ.Vị";
                detailGV.Columns["Price"].HeaderText = "Giá";
                detailGV.Columns["PCSReal"].HeaderText = "PCS";
                detailGV.Columns["NWReal"].HeaderText = "N.W";
                detailGV.Columns["OrderDomesticIndex"].HeaderText = "Số Phiếu";
                detailGV.Columns["DeliveryDate"].HeaderText = "Ngày Giao";
                detailGV.Columns["AmountPacking"].HeaderText = "Quy Cách";
                detailGV.Columns["TotalAmount"].HeaderText = "Thành Tiền";
                detailGV.Columns["PCSReal"].Width = 50;
                detailGV.Columns["NWReal"].Width = 50;
                detailGV.Columns["Price"].Width = 50;
                detailGV.Columns["Package"].Width = 50;
                detailGV.Columns["OrderDomesticIndex"].Width = 50;
                detailGV.Columns["DeliveryDate"].Width = 80;
                detailGV.Columns["TotalAmount"].Width = 80;

                detailGV.Columns["Price"].DefaultCellStyle.Format = "N0";
                detailGV.Columns["TotalAmount"].DefaultCellStyle.Format = "N0";

                
                dataGV.CellClick += CustomerGV_CellClick;

                if (dataGV.Rows.Count > 0)
                {
                    dataGV.ClearSelection();
                    dataGV.Rows[0].Selected = true;
                    dataGV.CurrentCell = dataGV.Rows[0].Cells[1];
                }

                decimal sumNWReal = mSummary_dt.Compute("SUM(NWReal)", "")
                               == DBNull.Value ? 0
                               : Convert.ToDecimal(mSummary_dt.Compute("SUM(NWReal)", ""));

                decimal sumTotalAmount = mSummary_dt.Compute("SUM(TotalAmount)", "")
                                                    == DBNull.Value ? 0
                                                    : Convert.ToDecimal(mSummary_dt.Compute("SUM(TotalAmount)", ""));

                totalNW_tb.Text = sumNWReal.ToString("N2");
                totalMoney_tb.Text = sumTotalAmount.ToString("N0");

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
            if (dataGV.CurrentRow == null) return;
            int CustomerID = Convert.ToInt32(dataGV.CurrentRow.Cells["CustomerID"].Value);

            mDetail_DV.RowFilter = $"CustomerID = {CustomerID}";
            mSummary_DV.RowFilter = $"CustomerID = {CustomerID}";
            //mCustomerDetail_DV.Sort = "Priority ASC, ProductName_VN ASC";
        }

        private async void Load_btn_Click(object sender, EventArgs e)
        {
            ShowData();
        }

        private void Print_btn_Click(object sender, EventArgs e)
        {
            _ = PrintPendingOrderSummary(2);
        }

        private void Preview_print_TD_btn_Click(object sender, EventArgs e)
        {
            _ = PrintPendingOrderSummary(1);
        }

        private async Task PrintPendingOrderSummary(int state)
        {
            if (dataGV.CurrentRow == null) return;

            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Message = "Đang xử lý ...";
            loadingOverlay.Show();
            await Task.Delay(100);

            DataTable cus_dt = await SQLStore_Kho.Instance.getCustomersAsync();
            int CustomerID = Convert.ToInt32(dataGV.CurrentRow.Cells["CustomerID"].Value);

            DataRow[] rows = cus_dt.Select($"CustomerID = {CustomerID}");
            if (rows.Length < 0)
            {
                MessageBox.Show("Có Lỗi");
                return;
            }

            string Company = rows[0]["Company"].ToString();
            string Address = rows[0]["Address"].ToString();
            string CustomerName = rows[0]["FullName"].ToString();
            string email = rows[0]["Email"].ToString();
            string taxCode = rows[0]["TaxCode"].ToString();

            DataTable sourceTable = mDetail_dt; // DataTable gốc

            DataTable resultTable = sourceTable.AsEnumerable()
                .Where(r => r.Field<int>("CustomerID") == CustomerID)
                .CopyToDataTable();

            DoiChieuCongNo_Printer printer = new DoiChieuCongNo_Printer();

            if (state == 1)
                printer.PrintPreview(resultTable, Company, CustomerName, Address, email, taxCode, this);
            else if (state == 2)
                printer.PrintDirect(resultTable, Company, CustomerName, Address, email, taxCode, tongdon_in2mat_cb.Checked);
            await Task.Delay(200);
            loadingOverlay.Hide();
        }
    }
}
