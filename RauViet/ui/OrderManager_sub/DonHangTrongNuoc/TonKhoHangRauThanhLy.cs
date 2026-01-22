using DocumentFormat.OpenXml.Office2010.Excel;
using RauViet.classes;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Color = System.Drawing.Color;

namespace RauViet.ui
{
    public partial class TonKhoHangRauThanhLy : Form
    {
        System.Data.DataTable mDomesticLiquidation_dt;
        private LoadingOverlay loadingOverlay;
        public TonKhoHangRauThanhLy()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;            

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;                        

            this.KeyDown += ProductList_KeyDown;
        }

        private void ProductList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                SQLStore_Kho.Instance.removeDomesticLiquidationImport();
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
                int curentYear = DateTime.Now.Year;

                var domesticLiquidationPriceTask = SQLStore_Kho.Instance.getDomesticLiquidationPriceAsync();
                var domesticLiquidationImportTask = SQLStore_Kho.Instance.getDomesticLiquidationImportAsync();
                var domesticLiquidationExportTask = SQLStore_Kho.Instance.getDomesticLiquidationExportAsync();
                
                await Task.WhenAll(domesticLiquidationImportTask, domesticLiquidationExportTask, domesticLiquidationPriceTask);

                var import_Ton_Result = domesticLiquidationImportTask.Result.AsEnumerable().Where(r => r.Field<DateTime>("ImportDate").Year < curentYear).GroupBy(r => r.Field<int>("DomesticLiquidationPriceID"))
                                                                                            .Select(g => new
                                                                                            {
                                                                                                DomesticLiquidationPriceID = g.Key,
                                                                                                TotalQuantity = g.Sum(x => Convert.ToDecimal(x["Quantity"]))
                                                                                            }).ToList();

                var export_Ton_Result = domesticLiquidationExportTask.Result.AsEnumerable().Where(r => r.Field<DateTime>("ExportDate").Year < curentYear).GroupBy(r => r.Field<int>("DomesticLiquidationPriceID"))
                                                                                            .Select(g => new
                                                                                            {
                                                                                                DomesticLiquidationPriceID = g.Key,
                                                                                                TotalQuantity = g.Sum(x => Convert.ToDecimal(x["Quantity"]))
                                                                                            }).ToList();

                var import_Current_Result = domesticLiquidationImportTask.Result.AsEnumerable().Where(r => r.Field<DateTime>("ImportDate").Year == curentYear).GroupBy(r => r.Field<int>("DomesticLiquidationPriceID"))
                                                                                            .Select(g => new
                                                                                            {
                                                                                                DomesticLiquidationPriceID = g.Key,
                                                                                                TotalQuantity = g.Sum(x => Convert.ToDecimal(x["Quantity"]))
                                                                                            }).ToList();

                var export_Current_Result = domesticLiquidationExportTask.Result.AsEnumerable().Where(r => r.Field<DateTime>("ExportDate").Year == curentYear).GroupBy(r => r.Field<int>("DomesticLiquidationPriceID"))
                                                                                            .Select(g => new
                                                                                            {
                                                                                                DomesticLiquidationPriceID = g.Key,
                                                                                                TotalQuantity = g.Sum(x => Convert.ToDecimal(x["Quantity"]))
                                                                                            }).ToList();

                mDomesticLiquidation_dt = domesticLiquidationPriceTask.Result.Copy();
                mDomesticLiquidation_dt.Columns.Add(new DataColumn("TonDauKi", typeof(decimal)));
                mDomesticLiquidation_dt.Columns.Add(new DataColumn("NhapTrongKy", typeof(decimal)));
                mDomesticLiquidation_dt.Columns.Add(new DataColumn("XuatTrongKy", typeof(decimal)));
                mDomesticLiquidation_dt.Columns.Add(new DataColumn("TonCuoiKi", typeof(decimal)));

                foreach (DataRow row in mDomesticLiquidation_dt.Rows)
                {
                    int domesticLiquidationPriceID = Convert.ToInt32(row["DomesticLiquidationPriceID"]);

                    var import_Ton_Row = import_Ton_Result .FirstOrDefault(x => x.DomesticLiquidationPriceID == domesticLiquidationPriceID);
                    var export_Ton_Row = export_Ton_Result.FirstOrDefault(x => x.DomesticLiquidationPriceID == domesticLiquidationPriceID);
                    var import_Current_Row = import_Current_Result.FirstOrDefault(x => x.DomesticLiquidationPriceID == domesticLiquidationPriceID);
                    var export_Current_Row = export_Current_Result.FirstOrDefault(x => x.DomesticLiquidationPriceID == domesticLiquidationPriceID);

                    int import_Ton_Quantity = import_Ton_Row != null ? Convert.ToInt32(import_Ton_Row.TotalQuantity): 0;
                    decimal export_Ton_Quantity = export_Ton_Row != null ? Convert.ToInt32(export_Ton_Row.TotalQuantity) : 0;
                    decimal import_Current_Quantity = import_Current_Row != null ? Convert.ToInt32(import_Current_Row.TotalQuantity) : 0;
                    decimal export_Current_Quantity = export_Current_Row != null ? Convert.ToInt32(export_Current_Row.TotalQuantity) : 0;

                    row["TonDauKi"] = import_Ton_Quantity - export_Ton_Quantity;
                    row["NhapTrongKy"] = import_Current_Quantity;
                    row["XuatTrongKy"] = export_Current_Quantity;
                    row["TonCuoiKi"] = (import_Ton_Quantity - export_Ton_Quantity) + (import_Current_Quantity - export_Current_Quantity);
                }

                int count = 0;
                mDomesticLiquidation_dt.Columns["Name_VN"].SetOrdinal(count++);
                mDomesticLiquidation_dt.Columns["Package"].SetOrdinal(count++);
                mDomesticLiquidation_dt.Columns["TonDauKi"].SetOrdinal(count++);
                mDomesticLiquidation_dt.Columns["NhapTrongKy"].SetOrdinal(count++);
                mDomesticLiquidation_dt.Columns["XuatTrongKy"].SetOrdinal(count++);
                mDomesticLiquidation_dt.Columns["TonCuoiKi"].SetOrdinal(count++);

                dataGV.DataSource = mDomesticLiquidation_dt;
                Utils.HideColumns(dataGV, new[] { "DomesticLiquidationPriceID", "ProductNameVN_NoSign", "SKU" });

                Utils.SetGridHeaders(dataGV, new System.Collections.Generic.Dictionary<string, string> {
                    {"Name_VN", "Tên Sản Phẩm" },
                    {"Package", "Đ.Vị" },
                    {"TonDauKi", "Tồn Đầu Kỳ" },
                    {"NhapTrongKy", "Nhập Trong Kỳ" },
                    {"XuatTrongKy", "Xuất Trong Kỳ" },
                    {"TonCuoiKi", "Tồn Cuối Kỳ" },
                    {"SalePrice", "Giá Thanh Lí" }
                });

                Utils.SetGridWidths(dataGV, new System.Collections.Generic.Dictionary<string, int> {
                    {"Name_VN", 180},
                    {"Package", 60},
                    {"TonDauKi", 80},
                    {"NhapTrongKy", 80},
                    {"XuatTrongKy", 80},
                    {"TonCuoiKi", 80},
                    {"SalePrice", 80},
                });

                dataGV.Columns["SalePrice"].DefaultCellStyle.Format = "N0";
                dataGV.Columns["TonDauKi"].DefaultCellStyle.Format = "N1";
                dataGV.Columns["NhapTrongKy"].DefaultCellStyle.Format = "N1";
                dataGV.Columns["XuatTrongKy"].DefaultCellStyle.Format = "N1";
                dataGV.Columns["TonCuoiKi"].DefaultCellStyle.Format = "N1";


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
    }
}
