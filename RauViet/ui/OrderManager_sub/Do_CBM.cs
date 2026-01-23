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
        private int mCurrentExportID = -1;
        public Do_CBM()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            cartonSizeGroupGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            cartonSizeGroupGV.MultiSelect = false;

            status_lb.Text = "";

            this.KeyDown += Do_CBM_KeyDown;
        }

        private void Do_CBM_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                if (mCurrentExportID <= 0)
                {
                    return;
                }

                SQLStore_Kho.Instance.removeOrders(mCurrentExportID);
                ShowData();
            }
        }

        public async void ShowData()
        {
            await Task.Delay(50);
            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Show();
            await Task.Delay(50);

            try
            {
                string[] keepColumns = { "ExportCodeID", "ExportCode", "ExchangeRate", "ShippingCost" };
                var parameters = new Dictionary<string, object> { { "Complete", false } };
                mExportCode_dt = await SQLStore_Kho.Instance.getExportCodesAsync(keepColumns, parameters);

                if(mCurrentExportID <= 0 && mExportCode_dt.Rows.Count > 0)
                    mCurrentExportID = Convert.ToInt32(mExportCode_dt.AsEnumerable().Max(r => r.Field<int>("ExportCodeID")));


                mOrders_dt = await SQLStore_Kho.Instance.getOrdersAsync(mCurrentExportID);

                mGroupCustomer = GroupByCustomer(mOrders_dt);
                mGroupCartonSize = GroupCartonSize(mOrders_dt);

                DataView dv1 = new DataView(mGroupCartonSize);
                dv1.RowFilter = $"ExportCodeID = '{mCurrentExportID}'";
                cartonSizeGroupGV.DataSource = dv1;

                DataView dv2 = new DataView(mGroupCustomer);
                dv2.RowFilter = $"ExportCodeID = '{mCurrentExportID}'";
                cusGroupGV.DataSource = dv2;

                Utils.HideColumns(cartonSizeGroupGV, new[] { "ExportCodeID" });
                Utils.HideColumns(cusGroupGV, new[] { "ExportCodeID" });

                exportCode_cbb.SelectedIndexChanged -= exportCode_search_cbb_SelectedIndexChanged;
                exportCode_cbb.DataSource = mExportCode_dt;
                exportCode_cbb.DisplayMember = "ExportCode";  // hiển thị tên
                exportCode_cbb.ValueMember = "ExportCodeID";

                Utils.SetGridHeaders(cartonSizeGroupGV, new System.Collections.Generic.Dictionary<string, string> {
                    {"CartonSize", "Carton Size" },
                    {"CountCarton", "Số Thùng" },
                    {"ChargeWeight", "Charge Weight" },
                    {"GrossWeight", "Gross Weight" },
                    {"FreightCharge", "Freight Charge" }
                });

                Utils.SetGridFormat(cartonSizeGroupGV, "FreightCharge", "#,##0.00");
                Utils.SetGridFormat(cartonSizeGroupGV, "GrossWeight", "#,##0.00");
                Utils.SetGridFormat(cartonSizeGroupGV, "ChargeWeight", "#,##0.00");
                Utils.SetGridFormat(cartonSizeGroupGV, "CBM", "#,##0");

                Utils.SetGridHeaders(cusGroupGV, new System.Collections.Generic.Dictionary<string, string> {
                    {"CustomerName", "Khách Hàng" },
                    {"CountCarton", "Số Thùng" },
                    {"ChargeWeight", "Charge Weight" },
                    {"GrossWeight", "Gross Weight" },
                    {"FreightCharge", "Freight Charge" }
                });

                Utils.SetGridWidths(cartonSizeGroupGV, new System.Collections.Generic.Dictionary<string, int> {
                    {"CartonSize", 80},
                    {"CountCarton", 50},
                    {"CBM", 80},
                    {"ChargeWeight", 80},
                    {"GrossWeight", 80},
                    {"FreightCharge", 80}
                });

                Utils.SetGridWidths(cusGroupGV, new System.Collections.Generic.Dictionary<string, int> {
                    {"CustomerName", 120},
                    {"CountCarton", 50},
                    {"CBM", 80},
                    {"ChargeWeight", 80},
                    {"GrossWeight", 80},
                    {"FreightCharge", 80}
                });

                Utils.SetGridFormat(cusGroupGV, "FreightCharge", "#,##0.00");
                Utils.SetGridFormat(cusGroupGV, "GrossWeight", "#,##0.00");
                Utils.SetGridFormat(cusGroupGV, "ChargeWeight", "#,##0.00");
                Utils.SetGridFormat(cusGroupGV, "CBM", "#,##0");

                                
                exportCode_cbb.SelectedValue = mCurrentExportID;                
                exportCode_cbb.SelectedIndexChanged += exportCode_search_cbb_SelectedIndexChanged;

                UpdateRightUI();
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
            dtResult.Columns.Add("CustomerName", typeof(string));
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
                    Customername = r.Field<string>("CustomerName")
                });
            int count = grouped.Count();
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

        private async void exportCode_search_cbb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!int.TryParse(exportCode_cbb.SelectedValue.ToString(), out int exportCodeId))
                return;
            mCurrentExportID = exportCodeId;

            mOrders_dt = await SQLStore_Kho.Instance.getOrdersAsync(exportCodeId);
            mGroupCustomer = GroupByCustomer(mOrders_dt);
            mGroupCartonSize = GroupCartonSize(mOrders_dt);

            decimal ShippingCost = Convert.ToDecimal(((DataRowView)exportCode_cbb.SelectedItem)["ShippingCost"]);
            decimal ExchangeRate = Convert.ToDecimal(((DataRowView)exportCode_cbb.SelectedItem)["ExchangeRate"]);

            DataView dv1 = new DataView(mGroupCartonSize);
            dv1.RowFilter = $"ExportCodeID = '{exportCodeId}'";
            cartonSizeGroupGV.DataSource = dv1;

            DataView dv2 = new DataView(mGroupCustomer);
            dv2.RowFilter = $"ExportCodeID = '{exportCodeId}'";
            cusGroupGV.DataSource = dv2;

            UpdateRightUI();
        }

        private void UpdateRightUI()
        {
            var rows = mExportCode_dt.AsEnumerable().Where(r => r.Field<int>("ExportCodeID") == mCurrentExportID).ToList();

            if (rows.Count <= 0) return;

            decimal ShippingCost = Convert.ToDecimal(rows[0]["ShippingCost"]);
            decimal ExchangeRate = Convert.ToDecimal(rows[0]["ExchangeRate"]);

            decimal totalCBM = 0;
            decimal totalChargeWeight = 0;
            int totalCarton = 0;
            decimal totalNWReal = 0;
            totalChargeWeight = 0;
            totalCarton = 0;


            decimal totalAmount = mOrders_dt.AsEnumerable()
                                            .Where(r => !r.IsNull("ExportCodeID") && r.Field<int>("ExportCodeID") == mCurrentExportID)
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

            object result = mGroupCartonSize.Compute("SUM(ChargeWeight)", $"ExportCodeID = {mCurrentExportID}");
            if (result != DBNull.Value)
                totalChargeWeight = Convert.ToDecimal(result);

            object result1 = mOrders_dt.Compute("SUM(NWReal)", $"ExportCodeID = {mCurrentExportID}");
            if (result1 != DBNull.Value)
                totalNWReal = Convert.ToDecimal(result1);

            object result2 = mGroupCartonSize.Compute("SUM(CountCarton)", $"ExportCodeID = {mCurrentExportID}");
            if (result2 != DBNull.Value)
                totalCarton = Convert.ToInt32(result2);

            object result3 = mGroupCartonSize.Compute("SUM(CBM)", $"ExportCodeID = {mCurrentExportID}");
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
