using DocumentFormat.OpenXml.Bibliography;
using Org.BouncyCastle.Tls;
using PdfSharp.Drawing;
using RauViet.classes;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Color = System.Drawing.Color;

namespace RauViet.ui
{
    public partial class KhoVatTu_LenhThuTrongNam : Form
    {
        DataTable mHarvest_dt;
        private LoadingOverlay loadingOverlay;        
        public KhoVatTu_LenhThuTrongNam()
        {
            InitializeComponent();
            this.KeyPreview = true;
            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;
            this.KeyDown += form_KeyDown;
        }

        private void form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                ShowData();
            }
        }

        public async void ShowData()
        {            
            await Task.Delay(50);
            loadingOverlay = new LoadingOverlay(this);
            await loadingOverlay.Show();

            try
            {
                department_GV.SelectionChanged -= DataGV_SelectionChanged;

                mHarvest_dt = await SQLStore_KhoVatTu.Instance.GetHarvestScheduleInYearAsync();
                mHarvest_dt = mHarvest_dt.Copy();

                var result = mHarvest_dt.AsEnumerable()
                                        .Select(r => new
                                        {
                                            Department = r.Field<int?>("Department"),
                                            DepartmentName = r.Field<string>("DepartmentName")
                                        })
                                        .Distinct()
                                        .ToList();

                DataTable newTable = new DataTable();
                newTable.Columns.Add("Department", typeof(int));
                newTable.Columns.Add("DepartmentName", typeof(string));

                foreach (var item in result)
                {
                    var row = newTable.NewRow();
                    row["Department"] = item.Department ?? 0;
                    row["DepartmentName"] = item.DepartmentName;
                    newTable.Rows.Add(row);
                }

                Utils.SetGridOrdinal(mHarvest_dt, new[] { "HarvestDate_Week", "HarvestDate", "ReceiveDepartmentName", "ProductionOrder", "ProductLotCode", "PlantName", "Quantity", "HarvestEmployeeName" });

                department_GV.DataSource = newTable;
                dataGV.DataSource = mHarvest_dt;
                Utils.HideColumns(dataGV, new[] { "SKU", "PlantingID", "HarvestID", "HarvestEmployee", "ReceiveDepartmentID", "Department", "DepartmentName" });
                Utils.HideColumns(department_GV, new[] { "Department"});

                Utils.SetGridHeaders(dataGV, new System.Collections.Generic.Dictionary<string, string> {
                        {"HarvestDate_Week", "Thứ" },
                        {"HarvestDate", "Ngày Thu" },
                        {"ReceiveDepartmentName", "Nơi Nhận" },
                        {"ProductionOrder","Lệnh SX" },
                        {"ProductLotCode", "Code" },
                        {"PlantName", "Cây Trồng" },
                        {"Quantity", "Số Lượng" },
                        {"HarvestEmployeeName", "NV Thu" },
                        {"DepartmentName", "Tổ Phụ Trách" },
                        {"IsCancelled", "Hủy" }
                    });
                Utils.SetGridWidths(dataGV, new System.Collections.Generic.Dictionary<string, int> {
                        {"HarvestDate_Week", 60 },
                        {"HarvestDate", 80 },
                        {"ReceiveDepartmentName", 150 },
                        {"ProductionOrder", 100 },
                        {"ProductLotCode", 100 },
                        {"PlantName", 150},
                        {"Quantity", 70 },
                        {"HarvestEmployeeName", 180 },
                        {"DepartmentName", 180 },
                        {"IsCancelled", 50 },
                    });

                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                DataGV_SelectionChanged(null, null);
                department_GV.SelectionChanged += DataGV_SelectionChanged;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
            }
            finally
            {
                await Task.Delay(200);
                loadingOverlay.Hide();
            }
        }

        private void DataGV_SelectionChanged(object sender, EventArgs e)
        {
            if (department_GV.SelectedRows.Count == 0)
                return;
            var cells = department_GV.SelectedRows[0].Cells;
            if (cells == null) return;

            int depID = Convert.ToInt32(cells["Department"].Value);


            DataView dv = mHarvest_dt.DefaultView;
            dv.RowFilter = $"Department = {depID}";
        }
    }
}
