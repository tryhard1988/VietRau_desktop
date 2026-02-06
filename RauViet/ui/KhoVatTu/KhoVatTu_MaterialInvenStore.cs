using RauViet.classes;
using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Media3D;
using Color = System.Drawing.Color;

namespace RauViet.ui
{
    public partial class KhoVatTu_MaterialInvenStore : Form
    {
        System.Data.DataTable mMaterial_dt;
        private DataView mLogDV;
        private Timer _monthYearDebounceTimer = new Timer { Interval = 500 };
        private Timer MaterialDebounceTimer = new Timer { Interval = 300 };
        bool isNewState = false;
        private LoadingOverlay loadingOverlay;
        public KhoVatTu_MaterialInvenStore()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;


            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

         
            this.KeyDown += KhoVatTu_MaterialInvenStore_KeyDown;          
        }

        private void KhoVatTu_MaterialInvenStore_KeyDown(object sender, KeyEventArgs e)
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
            loadingOverlay.Show();

            try
            {
                int year = DateTime.Now.Year;

                var materialTask = SQLStore_KhoVatTu.Instance.getMaterialAsync();
                var sumaryMaterialImportTask = SQLManager_KhoVatTu.Instance.GetSumaryMaterialImportAsync();
                var sumaryMaterialExportTask = SQLManager_KhoVatTu.Instance.GetSumaryMaterialExportAsync();

                await Task.WhenAll( materialTask, sumaryMaterialImportTask, sumaryMaterialExportTask);

                mMaterial_dt = materialTask.Result.Copy();
                var sumaryMaterialImport_dt = sumaryMaterialImportTask.Result;
                var sumaryMaterialExport_dt = sumaryMaterialExportTask.Result;

                Utils.AddColumnIfNotExists(mMaterial_dt, "TonDau", typeof(decimal));
                Utils.AddColumnIfNotExists(mMaterial_dt, "TonCuoi", typeof(decimal));
                Utils.AddColumnIfNotExists(mMaterial_dt, "SLNhap", typeof(decimal));
                Utils.AddColumnIfNotExists(mMaterial_dt, "SLXuat", typeof(decimal));

                var sumaryMaterialImport_Pre = sumaryMaterialImport_dt.AsEnumerable()
                                                            .Where(r => r.Field<int>("ImportYear") < year)
                                                            .GroupBy(r => r.Field<int>("MaterialID"))
                                                            .Select(g => new
                                                            {
                                                                MaterialID = g.Key,
                                                                ImportAmount = g.Sum(x => x.Field<decimal>("ImportAmount"))
                                                            });

                var sumaryMaterialExport_Pre = sumaryMaterialExport_dt.AsEnumerable()
                                                            .Where(r => r.Field<int>("ExportYear") < year)
                                                            .GroupBy(r => r.Field<int>("MaterialID"))
                                                            .Select(g => new
                                                            {
                                                                MaterialID = g.Key,
                                                                ExportAmount = g.Sum(x => x.Field<decimal>("ExportAmount"))
                                                            });

                var sumaryMaterialImport_Now = sumaryMaterialImport_dt.AsEnumerable()
                                                            .Where(r => r.Field<int>("ImportYear") == year)
                                                            .GroupBy(r => r.Field<int>("MaterialID"))
                                                            .Select(g => new
                                                            {
                                                                MaterialID = g.Key,
                                                                ImportAmount = g.Sum(x => x.Field<decimal>("ImportAmount"))
                                                            });

                var sumaryMaterialExport_Now = sumaryMaterialExport_dt.AsEnumerable()
                                                            .Where(r => r.Field<int>("ExportYear") == year)
                                                            .GroupBy(r => r.Field<int>("MaterialID"))
                                                            .Select(g => new
                                                            {
                                                                MaterialID = g.Key,
                                                                ExportAmount = g.Sum(x => x.Field<decimal>("ExportAmount"))
                                                            });

                foreach (DataRow row in mMaterial_dt.Rows)
                {
                    int materialID = Convert.ToInt32(row["MaterialID"]);
                    var importItem_pre = sumaryMaterialImport_Pre.FirstOrDefault(x => x.MaterialID == materialID);
                    var exportItem_pre = sumaryMaterialExport_Pre.FirstOrDefault(x => x.MaterialID == materialID);
                    var importItem_now = sumaryMaterialImport_Now.FirstOrDefault(x => x.MaterialID == materialID);
                    var exportItem_now = sumaryMaterialExport_Now.FirstOrDefault(x => x.MaterialID == materialID);

                    decimal importAmountPre = importItem_pre?.ImportAmount ?? 0;
                    decimal exportAmountPre = exportItem_pre?.ExportAmount ?? 0;
                    decimal importAmountNow = importItem_now?.ImportAmount ?? 0;
                    decimal exportAmountNow = exportItem_now?.ExportAmount ?? 0;

                    row["TonDau"] = importAmountPre - exportAmountPre;
                    row["SLNhap"] = importAmountNow;
                    row["SLXuat"] = exportAmountNow;
                    row["TonCuoi"] = (importAmountPre - exportAmountPre) + importAmountNow - exportAmountNow;

                }

                dataGV.DataSource = mMaterial_dt;
                Utils.HideColumns(dataGV, new[] { "MaterialID", "CategoryID", "UnitID", "MaterialName_nosign" });
                Utils.SetGridOrdinal(mMaterial_dt, new[] { "CategoryName", "MaterialName", "UnitName", "TonDau", "SLNhap", "SLXuat", "TonCuoi" });
                Utils.SetGridHeaders(dataGV, new System.Collections.Generic.Dictionary<string, string> {
                            {"CategoryName", "Phân Loại" },
                            {"MaterialName", "Vật Tư" },
                            {"UnitName", "Đ.Vị" },
                            {"TonDau", "Tồn Đầu" },
                            {"SLNhap", "SL Nhập" },
                            {"SLXuat", "SL Xuất" },
                            {"TonCuoi", "Tồn Cuối" }

                        });

                Utils.SetGridWidths(dataGV, new System.Collections.Generic.Dictionary<string, int> {
                            {"CategoryName", 150 },
                            {"MaterialName", 230 },
                            {"UnitName", 50 },
                            {"TonDau", 100 },
                            {"SLNhap", 100 },
                            {"SLXuat", 100 },
                            {"TonCuoi", 100 }
                        });

                Utils.SetGridFormat_N2(dataGV, "TonDau");
                Utils.SetGridFormat_N2(dataGV, "SLNhap");
                Utils.SetGridFormat_N2(dataGV, "SLXuat");
                Utils.SetGridFormat_N2(dataGV, "TonCuoi");
                Utils.SetGridFormat_Alignment(dataGV, "TonDau", DataGridViewContentAlignment.MiddleRight);
                Utils.SetGridFormat_Alignment(dataGV, "SLNhap", DataGridViewContentAlignment.MiddleRight);
                Utils.SetGridFormat_Alignment(dataGV, "SLXuat", DataGridViewContentAlignment.MiddleRight);
                Utils.SetGridFormat_Alignment(dataGV, "TonCuoi", DataGridViewContentAlignment.MiddleRight);

                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            catch
            {
            }
            finally
            {
                await Task.Delay(200);
                loadingOverlay.Hide();
            }

        }

        System.Data.DataTable excelData;
        private async void button1_Click(object sender, EventArgs e)
        {

            var materialCategory_dt = await SQLManager_KhoVatTu.Instance.GetMaterialCategoryAsync();

            using (System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog())
            {
                ofd.Title = "Chọn file Excel";
                ofd.Filter = "Excel Files|*.xlsx;*.xls|All Files|*.*";
                ofd.Multiselect = false; // chỉ cho chọn 1 file

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string filePath = ofd.FileName;
                    excelData = Utils.LoadExcel_NoHeader(filePath);

                    Utils.AddColumnIfNotExists(excelData, "MaterialID", typeof(int));
                    foreach (DataRow row in excelData.Rows)
                    {
                        string raw = Utils.RemoveVietnameseSigns(row["Column1"].ToString());


                        DataRow material = mMaterial_dt.AsEnumerable().FirstOrDefault(r => Utils.RemoveVietnameseSigns(r.Field<string>("MaterialName")).Trim().ToLower().Equals(raw.Trim().ToLower(), StringComparison.OrdinalIgnoreCase));
                        if (material != null)
                            row["MaterialID"] = material["MaterialID"];
                    }

                    dataGV.DataSource = excelData;
                    // dataGV.Columns["Column2"].Visible = false;
                }
            }
        }

        private async void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                DateTime ImportDate = new DateTime(DateTime.Now.Year - 1, 12, 31);
                foreach (DataRow edr in excelData.Rows)
                {                    
                    int MaterialID = Convert.ToInt32(edr["MaterialID"]);
                    decimal Amount = Convert.ToDecimal(edr["Column2"]);

                    int salaryInfoID = await SQLManager_KhoVatTu.Instance.insertMaterialImportAsync(ImportDate, MaterialID, Amount, null, "Tạo Dữ liệu tồn đầu");
                    if (salaryInfoID <= 0)
                    {
                        MessageBox.Show("có lỗi ");
                        return;
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("có lỗi " + ex.Message);
            }
            MessageBox.Show("xong");
        }
    }
}
