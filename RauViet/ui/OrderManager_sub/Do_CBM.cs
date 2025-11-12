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
        DataTable mExportCode_dt, mGroupCartonSize, mGroupCustomer, mGroupProduct;
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
                string[] keepColumns = { "ExportCodeID", "ExportCode", "InputByName_NoSign" };
                var parameters = new Dictionary<string, object> { { "Complete", false } };
                var exportCodeTask = SQLStore.Instance.getExportCodesAsync(keepColumns, parameters);

                await Task.WhenAll(ordersPackingTask, exportCodeTask);

                mExportCode_dt = exportCodeTask.Result;
                var Orders_dt = ordersPackingTask.Result;

                mGroupCustomer = GroupByCustomer(Orders_dt);
                mGroupCartonSize = GroupCartonSize(Orders_dt);
                mGroupProduct = GroupByProduct(Orders_dt);

                cartonSizeGroupGV.DataSource = mGroupCartonSize;
                cusGroupGV.DataSource = mGroupCartonSize;
                productGroup_GV.DataSource = mGroupProduct;

                cartonSizeGroupGV.Columns["ExportCodeID"].Visible = false;
                cusGroupGV.Columns["ExportCodeID"].Visible = false;
                productGroup_GV.Columns["ExportCodeID"].Visible = false;

                exportCode_cbb.DataSource = mExportCode_dt;
                exportCode_cbb.DisplayMember = "ExportCode";  // hiển thị tên
                exportCode_cbb.ValueMember = "ExportCodeID";


                cartonSizeGroupGV.Columns["CartonSize"].HeaderText = "CarTon Size";
                cartonSizeGroupGV.Columns["CountCarton"].HeaderText = "Số Thùng";

                cusGroupGV.Columns["CustomerName"].HeaderText = "Khách Hàng";
                cusGroupGV.Columns["CountCarton"].HeaderText = "Số Thùng";

                productGroup_GV.Columns["ProductNameVN"].HeaderText = "Sản Phẩm";
                productGroup_GV.Columns["CountCarton"].HeaderText = "Số Thùng";

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
            dtResult.Columns.Add("Charweight", typeof(decimal));

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
                foreach (var row in distinctCartons)
                {
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
                        }
                    }
                }

                dtResult.Rows.Add(
                    g.Key.ExportCodeID,
                    g.Key.CartonSize,
                    distinctCartons.Count(),
                    Math.Round(totalCBM, 3),
                    Math.Round(totalCBM/6000.0m, 3)
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
            dtResult.Columns.Add("Charweight", typeof(decimal));

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
                foreach (var row in distinctCartons)
                {
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
                        }
                    }
                }

                dtResult.Rows.Add(
                    g.Key.ExportCodeID,
                    g.Key.Customername,
                    distinctCartons.Count(),
                    Math.Round(totalCBM, 3),
                    Math.Round(totalCBM / 6000.0m, 3) // Volumetric Weight
                );
            }

            return dtResult;
        }

        public DataTable GroupByProduct(DataTable mOrders_dt)
        {
            // Tạo DataTable kết quả
            DataTable dtResult = new DataTable();
            dtResult.Columns.Add("ExportCodeID", typeof(int));
            dtResult.Columns.Add("ProductNameVN", typeof(string));
            dtResult.Columns.Add("CountCarton", typeof(int));
            dtResult.Columns.Add("CBM", typeof(decimal));
            dtResult.Columns.Add("Charweight", typeof(decimal));

            // Nhóm theo ExportCodeID + ProductNameVN
            var grouped = mOrders_dt.AsEnumerable()
                .Where(r => !string.IsNullOrWhiteSpace(r.Field<string>("CartonSize"))
                            && r.Field<int?>("CartonNo") != null
                            && r.Field<int?>("ExportCodeID") != null
                            && !string.IsNullOrWhiteSpace(r.Field<string>("ProductNameVN")))
                .GroupBy(r => new
                {
                    ExportCodeID = r.Field<int>("ExportCodeID"),
                    ProductNameVN = r.Field<string>("ProductNameVN")
                });

            foreach (var g in grouped)
            {
                // Lấy các carton duy nhất (theo CartonNo)
                var distinctCartons = g
                    .GroupBy(r => r.Field<int>("CartonNo"))
                    .Select(x => x.First());

                decimal totalCBM = 0;
                foreach (var row in distinctCartons)
                {
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
                        }
                    }
                }

                dtResult.Rows.Add(
                    g.Key.ExportCodeID,
                    g.Key.ProductNameVN,
                    distinctCartons.Count(),
                    Math.Round(totalCBM, 3),
                    Math.Round(totalCBM / 6000.0m, 3) // Volumetric Weight
                );
            }

            return dtResult;
        }


        private void exportCode_search_cbb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mGroupCartonSize == null || mExportCode_dt.Rows.Count == 0)
                return;

            string selectedExportCode = ((DataRowView)exportCode_cbb.SelectedItem)["ExportCodeID"].ToString();

            if (!string.IsNullOrEmpty(selectedExportCode))
            {
                DataView dv1 = new DataView(mGroupCartonSize);
                dv1.RowFilter = $"ExportCodeID = '{selectedExportCode}'";
                cartonSizeGroupGV.DataSource = dv1;

                DataView dv2 = new DataView(mGroupCustomer);
                dv2.RowFilter = $"ExportCodeID = '{selectedExportCode}'";
                cusGroupGV.DataSource = dv2;

                DataView dv3 = new DataView(mGroupProduct);
                dv3.RowFilter = $"ExportCodeID = '{selectedExportCode}'";
                productGroup_GV.DataSource = dv3;
            }
            else
            {
                cartonSizeGroupGV.DataSource = mGroupCartonSize;
            }

        }
    }
}
