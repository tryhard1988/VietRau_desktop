using RauViet.classes;
using System;
using System.Collections.Generic;
using System.Data;

using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RauViet.ui
{
    public partial class Do_CBM : Form
    {
        DataTable mExportCode_dt, mGroupCartonSize, mGroupCustomer, mOrders_dt;
        private LoadingOverlay loadingOverlay;
        public Do_CBM()
        {
            InitializeComponent();

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            cartonSizeGroupGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            cartonSizeGroupGV.MultiSelect = false;

            status_lb.Text = "";
            exportCode_cbb.SelectedIndexChanged += exportCode_search_cbb_SelectedIndexChanged;
        }

        public async void ShowData()
        {
            await Task.Delay(50);
            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Show();
            await Task.Delay(50);

            try
            {
                var ordersPackingTask = SQLStore.Instance.getOrdersAsync(); ;
                string[] keepColumns = { "ExportCodeID", "ExportCode", "ExchangeRate", "ShippingCost" };
                var parameters = new Dictionary<string, object> { { "Complete", false } };
                var exportCodeTask = SQLStore.Instance.getExportCodesAsync(keepColumns, parameters);

                await Task.WhenAll(ordersPackingTask, exportCodeTask);

                mExportCode_dt = exportCodeTask.Result;
                mOrders_dt = ordersPackingTask.Result;

                mGroupCustomer = GroupByCustomer(mOrders_dt);
                mGroupCartonSize = GroupCartonSize(mOrders_dt);

                cartonSizeGroupGV.DataSource = mGroupCartonSize;
                cusGroupGV.DataSource = mGroupCartonSize;

                cartonSizeGroupGV.Columns["ExportCodeID"].Visible = false;
                cusGroupGV.Columns["ExportCodeID"].Visible = false;

                exportCode_cbb.DataSource = mExportCode_dt;
                exportCode_cbb.DisplayMember = "ExportCode";  // hiển thị tên
                exportCode_cbb.ValueMember = "ExportCodeID";


                cartonSizeGroupGV.Columns["CartonSize"].HeaderText = "Carton Size";
                cartonSizeGroupGV.Columns["CountCarton"].HeaderText = "Số Thùng";
                cartonSizeGroupGV.Columns["ChargeWeight"].HeaderText = "Charge Weight";
                cartonSizeGroupGV.Columns["GrossWeight"].HeaderText = "Gross Weight";
                cartonSizeGroupGV.Columns["FreightCharge"].HeaderText = "Freight Charge";

                cartonSizeGroupGV.Columns["FreightCharge"].DefaultCellStyle.Format = "#,##0.00";
                cartonSizeGroupGV.Columns["GrossWeight"].DefaultCellStyle.Format = "#,##0.00";
                cartonSizeGroupGV.Columns["ChargeWeight"].DefaultCellStyle.Format = "#,##0.00";
                cartonSizeGroupGV.Columns["CBM"].DefaultCellStyle.Format = "#,##0";

                cusGroupGV.Columns["CustomerName"].HeaderText = "Khách Hàng";
                cusGroupGV.Columns["CountCarton"].HeaderText = "Số Thùng";
                cusGroupGV.Columns["ChargeWeight"].HeaderText = "Charge Weight";
                cusGroupGV.Columns["GrossWeight"].HeaderText = "Gross Weight";
                cusGroupGV.Columns["FreightCharge"].HeaderText = "Freight Charge";

                cusGroupGV.Columns["FreightCharge"].DefaultCellStyle.Format = "#,##0.00";
                cusGroupGV.Columns["GrossWeight"].DefaultCellStyle.Format = "#,##0.00";
                cusGroupGV.Columns["ChargeWeight"].DefaultCellStyle.Format = "#,##0.00";
                cusGroupGV.Columns["CBM"].DefaultCellStyle.Format = "#,##0";

                cartonSizeGroupGV.Columns["CartonSize"].Width = 80;
                cartonSizeGroupGV.Columns["CountCarton"].Width = 50;
                cartonSizeGroupGV.Columns["CBM"].Width = 80;
                cartonSizeGroupGV.Columns["ChargeWeight"].Width = 80;
                cartonSizeGroupGV.Columns["GrossWeight"].Width = 80;
                cartonSizeGroupGV.Columns["FreightCharge"].Width = 80;

                cusGroupGV.Columns["CustomerName"].Width = 120;
                cusGroupGV.Columns["CountCarton"].Width = 50;
                cusGroupGV.Columns["CBM"].Width = 80;
                cusGroupGV.Columns["ChargeWeight"].Width = 80;
                cusGroupGV.Columns["GrossWeight"].Width = 80;
                cusGroupGV.Columns["FreightCharge"].Width = 80;

                if (mExportCode_dt.Rows.Count > 0)
                {
                    int maxID = Convert.ToInt32(mExportCode_dt.AsEnumerable()
                                   .Max(r => r.Field<int>("ExportCodeID")));
                    exportCode_cbb.SelectedValue = maxID;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                status_lb.Text = "Thất bại.";
                status_lb.ForeColor = System.Drawing.Color.Red;
            }
            finally
            {
                await Task.Delay(200);
                loadingOverlay.Hide();
            }
        }

        public DataTable GroupCartonSize(DataTable mOrders_dt)
        {
            // Tạo DataTable kết quả
            DataTable dtResult = new DataTable();
            dtResult.Columns.Add("ExportCodeID", typeof(int));
            dtResult.Columns.Add("CartonSize", typeof(string));
            dtResult.Columns.Add("CountCarton", typeof(int));
            dtResult.Columns.Add("CBM", typeof(decimal));
            dtResult.Columns.Add("ChargeWeight", typeof(decimal));
            dtResult.Columns.Add("GrossWeight", typeof(decimal));
            dtResult.Columns.Add("FreightCharge", typeof(decimal));

            // Nhóm theo ExportCodeID + CartonSize
            var grouped = mOrders_dt.AsEnumerable()
                .Where(r => !string.IsNullOrWhiteSpace(r.Field<string>("CartonSize"))
                            && r.Field<int?>("CartonNo") != null
                            && r.Field<int?>("ExportCodeID") != null)
                .GroupBy(r => new
                {
                    ExportCodeID = r.Field<int>("ExportCodeID"),
                    CartonSize = r.Field<string>("CartonSize")
                });

           

            foreach (var g in grouped)
            {
                // Lấy các carton duy nhất (theo CartonNo)
                var distinctCartons = g
                    .GroupBy(r => r.Field<int>("CartonNo"))
                    .Select(x => x.First());

                decimal totalCBM = 0;
                decimal totalNW = 0;
                foreach (var row in distinctCartons)
                {
                    decimal nw = row.Field<decimal>("NWReal");
                    string cartonSize = row.Field<string>("CartonSize");
                    if (!string.IsNullOrEmpty(cartonSize))
                    {
                        var parts = cartonSize.Split('x');
                        if (parts.Length == 3
                            && decimal.TryParse(parts[0].Trim(), out decimal length)
                            && decimal.TryParse(parts[1].Trim(), out decimal width)
                            && decimal.TryParse(parts[2].Trim(), out decimal height))
                        {
                            totalCBM += length * width * height;
                            totalNW += nw;
                        }
                    }
                }

                DataRow row1 = mExportCode_dt.AsEnumerable().FirstOrDefault(r => r.Field<int>("ExportCodeID") == g.Key.ExportCodeID);
                decimal exchangeRate = 0;
                decimal shippingCost = 0;
                if (row1 != null)
                {
                    exchangeRate = row1.Field<decimal>("ExchangeRate");
                    shippingCost = row1.Field<decimal>("ShippingCost");
                }

                dtResult.Rows.Add(
                    g.Key.ExportCodeID,
                    g.Key.CartonSize,
                    distinctCartons.Count(),
                    Math.Round(totalCBM, 3),
                    Math.Round(totalCBM/6000.0m, 3),
                    totalNW + distinctCartons.Count()*3,
                    Math.Round(totalCBM / 6000.0m * exchangeRate * shippingCost, 3)
                );
            }

           
            return dtResult;
        }

        public DataTable GroupByCustomer(DataTable mOrders_dt)
        {
            // Tạo DataTable kết quả
            DataTable dtResult = new DataTable();
            dtResult.Columns.Add("ExportCodeID", typeof(int));
            dtResult.Columns.Add("Customername", typeof(string));
            dtResult.Columns.Add("CountCarton", typeof(int));
            dtResult.Columns.Add("CBM", typeof(decimal));
            dtResult.Columns.Add("ChargeWeight", typeof(decimal));
            dtResult.Columns.Add("GrossWeight", typeof(decimal));
            dtResult.Columns.Add("FreightCharge", typeof(decimal));
            // Nhóm theo ExportCodeID + CustomerID
            var grouped = mOrders_dt.AsEnumerable()
                .Where(r => !string.IsNullOrWhiteSpace(r.Field<string>("CartonSize"))
                            && r.Field<int?>("CartonNo") != null
                            && r.Field<int?>("ExportCodeID") != null
                            && r.Field<int?>("CustomerID") != null)
                .GroupBy(r => new
                {
                    ExportCodeID = r.Field<int>("ExportCodeID"),
                    Customername = r.Field<string>("Customername")
                });

            foreach (var g in grouped)
            {
                // Lấy các carton duy nhất (theo CartonNo)
                var distinctCartons = g
                    .GroupBy(r => r.Field<int>("CartonNo"))
                    .Select(x => x.First());

                decimal totalCBM = 0;
                decimal totalNW = 0;
                foreach (var row in distinctCartons)
                {
                    decimal nw = row.Field<decimal>("NWReal");
                    string cartonSize = row.Field<string>("CartonSize");
                    if (!string.IsNullOrEmpty(cartonSize))
                    {
                        var parts = cartonSize.Split('x');
                        if (parts.Length == 3
                            && decimal.TryParse(parts[0].Trim(), out decimal length)
                            && decimal.TryParse(parts[1].Trim(), out decimal width)
                            && decimal.TryParse(parts[2].Trim(), out decimal height))
                        {
                            totalCBM += length * width * height;
                            totalNW += nw;
                        }
                    }
                }

                DataRow row1 = mExportCode_dt.AsEnumerable().FirstOrDefault(r => r.Field<int>("ExportCodeID") == g.Key.ExportCodeID);
                decimal exchangeRate = 0;
                decimal shippingCost = 0;
                if (row1 != null)
                {
                    exchangeRate = row1.Field<decimal>("ExchangeRate");
                    shippingCost = row1.Field<decimal>("ShippingCost");
                }
                dtResult.Rows.Add(
                    g.Key.ExportCodeID,
                    g.Key.Customername,
                    distinctCartons.Count(),
                    Math.Round(totalCBM, 3),
                    Math.Round(totalCBM / 6000.0m, 3),
                    totalNW  + distinctCartons.Count()*3,
                    Math.Round(totalCBM / 6000.0m * exchangeRate * shippingCost, 3)
                );
            }

            return dtResult;
        }

        private void exportCode_search_cbb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mGroupCartonSize == null || mExportCode_dt.Rows.Count == 0 || exportCode_cbb.SelectedItem == null)
                return;
            int ExportCodeID = Convert.ToInt32(((DataRowView)exportCode_cbb.SelectedItem)["ExportCodeID"]);
            decimal ShippingCost = Convert.ToDecimal(((DataRowView)exportCode_cbb.SelectedItem)["ShippingCost"]);
            decimal ExchangeRate = Convert.ToDecimal(((DataRowView)exportCode_cbb.SelectedItem)["ExchangeRate"]);

            DataView dv1 = new DataView(mGroupCartonSize);
            dv1.RowFilter = $"ExportCodeID = '{ExportCodeID}'";
            cartonSizeGroupGV.DataSource = dv1;

            DataView dv2 = new DataView(mGroupCustomer);
            dv2.RowFilter = $"ExportCodeID = '{ExportCodeID}'";
            cusGroupGV.DataSource = dv2;

            decimal totalCBM = 0;
            decimal totalChargeWeight = 0;
            int totalCarton = 0;
            decimal totalNWReal = 0;
            totalChargeWeight = 0;
            totalCarton = 0;
            

            decimal totalAmount = mOrders_dt.AsEnumerable()
                                            .Where(r => !r.IsNull("ExportCodeID") && r.Field<int>("ExportCodeID") == ExportCodeID)
                                            .Sum(r =>
                                            {
                                                if (r.IsNull("OrderPackingPriceCNF"))
                                                    return 0m;

                                                string package = r.Field<string>("Package")?.Trim().ToLower() ?? "";
                                                decimal price = r.Field<decimal>("OrderPackingPriceCNF");
                                                decimal pcsReal = r.IsNull("PCSReal") ? 0 : r.Field<int>("PCSReal");
                                                decimal nwReal = r.IsNull("NWReal") ? 0m : r.Field<decimal>("NWReal");

                                                return (package == "weight" || package == "kg")
                                                    ? price * nwReal
                                                    : price * pcsReal;
                                            });

            object result = mGroupCartonSize.Compute( "SUM(ChargeWeight)", $"ExportCodeID = {ExportCodeID}");
            if (result != DBNull.Value)
                totalChargeWeight = Convert.ToDecimal(result);

            object result1 = mOrders_dt.Compute("SUM(NWReal)", $"ExportCodeID = {ExportCodeID}");
            if (result1 != DBNull.Value)
                totalNWReal = Convert.ToDecimal(result1);

            object result2 = mGroupCartonSize.Compute("SUM(CountCarton)", $"ExportCodeID = {ExportCodeID}");
            if (result2 != DBNull.Value)
                totalCarton = Convert.ToInt32(result2);

            object result3 = mGroupCartonSize.Compute("SUM(CBM)", $"ExportCodeID = {ExportCodeID}");
            if (result3 != DBNull.Value)
                totalCBM = Convert.ToInt32(result3);

            totalChargeWeight_tb.Text = totalChargeWeight.ToString("#,##0.00");
            totalFreightCharge_tb.Text = (totalChargeWeight * ShippingCost * ExchangeRate).ToString("#,##0.00");
            totalCarton_tb.Text = totalCarton.ToString();
            netWeight_tb.Text = totalNWReal.ToString("#,##0.00");
            totalCBM_tb.Text = totalCBM.ToString("#,##0");
            totalAmount_tb.Text = totalAmount.ToString("F2");

        }
    }
}
