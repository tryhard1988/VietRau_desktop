using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Spreadsheet;
using MySqlX.XDevAPI.Common;
using RauViet.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RauViet.ui
{
    public partial class KhoVatTu_TKSanLuongTrong_Thang : Form
    {
        Dictionary<string, DataTable> mPlantingManagement_Departments;
        private Timer _monthYearDebounceTimer = new Timer { Interval = 500 };
        private LoadingOverlay loadingOverlay;
        int _prevYear;
        public KhoVatTu_TKSanLuongTrong_Thang()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;
                        
            _monthYearDebounceTimer.Tick += MonthYearDebounceTimer_Tick;

            monthYearDtp.Format = DateTimePickerFormat.Custom;
            monthYearDtp.CustomFormat = "MM/yyyy";
            monthYearDtp.ShowUpDown = true;
            monthYearDtp.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            monthYearDtp.ValueChanged += monthYearDtp_ValueChanged;

            dataGV_A.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV_A.MultiSelect = false;

            this.KeyDown += KhoVatTu_MaterialInvenStore_KeyDown;

            exportExcel_btn.Click += ExportExcel_btn_Click;
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
            await loadingOverlay.Show();

            mPlantingManagement_Departments = new Dictionary<string, DataTable>();

            try
            {
                _prevYear = monthYearDtp.Value.Year;

                DataTable plantingManagemen_dt = await SQLStore_KhoVatTu.Instance.getPlantingManagementAsync_HarvestQuantityMonth(_prevYear);

                DataTable dtDistinct = plantingManagemen_dt.DefaultView.ToTable(true, "Department", "DepartmentName");

                foreach (DataRow dr in dtDistinct.Rows)
                {
                    int Department = Convert.ToInt32(dr["Department"]);
                    string DepartmentName = Convert.ToString(dr["DepartmentName"]);

                    mPlantingManagement_Departments[Utils.RemoveVietnameseSigns(DepartmentName).ToLower().Replace(" ", "")] = GetPlantingManager(plantingManagemen_dt, Department);
                }

                var template = mPlantingManagement_Departments.Values.FirstOrDefault();
                mPlantingManagement_Departments["toa"] = GetOrCreateTable("toa", template);
                mPlantingManagement_Departments["tob"] = GetOrCreateTable("tob", template);
                mPlantingManagement_Departments["nhauom-thuycanh"] = GetOrCreateTable("nhauom-thuycanh", template);
                mPlantingManagement_Departments["farmbt"] = GetOrCreateTable("farmbt", template);

                monthYearDtp_ValueChanged(null, null);

                dataGV_A.DataSource = mPlantingManagement_Departments["toa"];
                dataGV_B.DataSource = mPlantingManagement_Departments["tob"];
                dataGV_NU.DataSource = mPlantingManagement_Departments["nhauom-thuycanh"];
                dataGV_FTB.DataSource = mPlantingManagement_Departments["farmbt"];

                {
                    Utils.HideColumns(dataGV_A, new[] { "SKU", "HarvestDate_Month", "HarvestDate_Year" });
                    Utils.SetGridHeaders(dataGV_A, new System.Collections.Generic.Dictionary<string, string> {
                            {"ProductionOrder", "Lệnh\nSản Xuất" },
                            {"PlantName", "Tên\nSản Phẩm" },
                            {"HarvestQuantity", "Sản Lượng" },
                            {"HarvestQuantity_1", "Hủy" },
                            {"HarvestQuantity_2", "Thu" },
                            {"Area", "Diện\nTích" },
                            {"NangSuatTrong", "Năng\nXuất\n(kg/m2)" }});

                    Utils.SetGridWidths(dataGV_A, new System.Collections.Generic.Dictionary<string, int> {
                            {"ProductionOrder", 80 },
                            {"PlantName", 100 },
                            {"HarvestQuantity", 50 },
                            {"HarvestQuantity_1", 50 },
                            {"HarvestQuantity_2", 50 },
                            {"Area", 65 },
                            {"NangSuatTrong", 70 }});

                    Utils.SetGridFormat_N2(dataGV_A, "HarvestQuantity");
                    Utils.SetGridFormat_N2(dataGV_A, "Area");
                    Utils.SetGridFormat_N2(dataGV_A, "NangSuatTrong");
                    Utils.SetGridFormat_Alignment(dataGV_A, "HarvestQuantity", DataGridViewContentAlignment.MiddleRight);
                    Utils.SetGridFormat_Alignment(dataGV_A, "Area", DataGridViewContentAlignment.MiddleRight);
                    Utils.SetGridFormat_Alignment(dataGV_A, "NangSuatTrong", DataGridViewContentAlignment.MiddleRight);

                    dataGV_A.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }

                {
                    Utils.HideColumns(dataGV_B, new[] { "SKU", "HarvestDate_Month", "HarvestDate_Year" });
                    Utils.SetGridHeaders(dataGV_B, new System.Collections.Generic.Dictionary<string, string> {
                            {"ProductionOrder", "Lệnh\nSản Xuất" },
                            {"PlantName", "Tên\nSản Phẩm" },
                            {"HarvestQuantity", "Sản Lượng" },
                            {"HarvestQuantity_1", "Hủy" },
                            {"HarvestQuantity_2", "Thu" },
                            {"Area", "Diện\nTích" },
                            {"NangSuatTrong", "Năng\nXuất\n(kg/m2)" }});

                    Utils.SetGridWidths(dataGV_B, new System.Collections.Generic.Dictionary<string, int> {
                            {"ProductionOrder", 80 },
                            {"PlantName", 100 },
                            {"HarvestQuantity", 50 },
                            {"HarvestQuantity_1", 50 },
                            {"HarvestQuantity_2", 50 },
                            {"Area", 65 },
                            {"NangSuatTrong", 70 }});

                    Utils.SetGridFormat_N2(dataGV_B, "HarvestQuantity");
                    Utils.SetGridFormat_N2(dataGV_B, "Area");
                    Utils.SetGridFormat_N2(dataGV_B, "NangSuatTrong");
                    Utils.SetGridFormat_Alignment(dataGV_B, "HarvestQuantity", DataGridViewContentAlignment.MiddleRight);
                    Utils.SetGridFormat_Alignment(dataGV_B, "Area", DataGridViewContentAlignment.MiddleRight);
                    Utils.SetGridFormat_Alignment(dataGV_B, "NangSuatTrong", DataGridViewContentAlignment.MiddleRight);

                    dataGV_B.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }

                {
                    Utils.HideColumns(dataGV_NU, new[] { "SKU", "HarvestDate_Month", "HarvestDate_Year" });
                    Utils.SetGridHeaders(dataGV_NU, new System.Collections.Generic.Dictionary<string, string> {
                            {"ProductionOrder", "Lệnh\nSản Xuất" },
                            {"PlantName", "Tên\nSản Phẩm" },
                            {"HarvestQuantity", "Sản Lượng" },
                            {"HarvestQuantity_1", "Hủy" },
                            {"HarvestQuantity_2", "Thu" },
                            {"Area", "Diện\nTích" },
                            {"NangSuatTrong", "Năng\nXuất\n(kg/m2)" }});

                    Utils.SetGridWidths(dataGV_NU, new System.Collections.Generic.Dictionary<string, int> {
                            {"ProductionOrder", 80 },
                            {"PlantName", 100 },
                            {"HarvestQuantity", 50 },
                            {"HarvestQuantity_1", 50 },
                            {"HarvestQuantity_2", 50 },
                            {"Area", 65 },
                            {"NangSuatTrong", 70 }});

                    Utils.SetGridFormat_N2(dataGV_NU, "HarvestQuantity");
                    Utils.SetGridFormat_N2(dataGV_NU, "Area");
                    Utils.SetGridFormat_N2(dataGV_NU, "NangSuatTrong");
                    Utils.SetGridFormat_Alignment(dataGV_NU, "HarvestQuantity", DataGridViewContentAlignment.MiddleRight);
                    Utils.SetGridFormat_Alignment(dataGV_NU, "Area", DataGridViewContentAlignment.MiddleRight);
                    Utils.SetGridFormat_Alignment(dataGV_NU, "NangSuatTrong", DataGridViewContentAlignment.MiddleRight);

                    dataGV_NU.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }

                {
                    Utils.HideColumns(dataGV_FTB, new[] { "SKU", "HarvestDate_Month", "HarvestDate_Year" });
                    Utils.SetGridHeaders(dataGV_FTB, new System.Collections.Generic.Dictionary<string, string> {
                            {"ProductionOrder", "Lệnh\nSản Xuất" },
                            {"PlantName", "Tên\nSản Phẩm" },
                            {"HarvestQuantity", "Sản Lượng" },
                            {"HarvestQuantity_1", "Hủy" },
                            {"HarvestQuantity_2", "Thu" },
                            {"Area", "Diện\nTích" },
                            {"NangSuatTrong", "Năng\nXuất\n(kg/m2)" }});

                    Utils.SetGridWidths(dataGV_FTB, new System.Collections.Generic.Dictionary<string, int> {
                            {"ProductionOrder", 80 },
                            {"PlantName", 100 },
                            {"HarvestQuantity", 50 },
                            {"HarvestQuantity_1", 50 },
                            {"HarvestQuantity_2", 50 },
                            {"Area", 65 },
                            {"NangSuatTrong", 70 }});

                    Utils.SetGridFormat_N2(dataGV_FTB, "HarvestQuantity");
                    Utils.SetGridFormat_N2(dataGV_FTB, "Area");
                    Utils.SetGridFormat_N2(dataGV_FTB, "NangSuatTrong");
                    Utils.SetGridFormat_Alignment(dataGV_FTB, "HarvestQuantity", DataGridViewContentAlignment.MiddleRight);
                    Utils.SetGridFormat_Alignment(dataGV_FTB, "Area", DataGridViewContentAlignment.MiddleRight);
                    Utils.SetGridFormat_Alignment(dataGV_FTB, "NangSuatTrong", DataGridViewContentAlignment.MiddleRight);

                    dataGV_FTB.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
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

        DataTable GetOrCreateTable(string key, DataTable template)
        {
            if (mPlantingManagement_Departments.TryGetValue(key, out var dt) && dt != null)
                return dt;

            // Tạo table rỗng theo structure
            var newTable = template != null ? template.Clone() : new DataTable();

            // Add vào dictionary
            mPlantingManagement_Departments[key] = newTable;

            return newTable;
        }

        private DataTable GetPlantingManager(DataTable plantingManagement_dt, int depID)
        {
            DataTable result = new DataTable();
            result.Columns.Add("SKU", typeof(int));
            result.Columns.Add("HarvestDate_Month", typeof(int));
            result.Columns.Add("HarvestDate_Year", typeof(int));
            result.Columns.Add("ProductionOrder", typeof(string));
            result.Columns.Add("PlantName", typeof(string));
            result.Columns.Add("HarvestQuantity", typeof(decimal));
            result.Columns.Add("HarvestQuantity_1", typeof(decimal));
            result.Columns.Add("HarvestQuantity_2", typeof(decimal));
            result.Columns.Add("Area", typeof(decimal));
            result.Columns.Add("NangSuatTrong", typeof(decimal)); // năng suất

            // Lọc dữ liệu
            DataRow[] Arows = plantingManagement_dt.Select($"Department = {depID}");

            foreach (DataRow row in Arows)
            {
                decimal harvestQty = row["HarvestQuantity"] == DBNull.Value ? 0 : Convert.ToDecimal(row["HarvestQuantity"]);
                decimal harvestQty1 = row["TotalQuantity_1"] == DBNull.Value ? 0 : Convert.ToDecimal(row["TotalQuantity_1"]);
                decimal harvestQty2 = row["TotalQuantity_2"] == DBNull.Value ? 0 : Convert.ToDecimal(row["TotalQuantity_2"]);
                decimal area = row["Area"] == DBNull.Value ? 0 : Convert.ToDecimal(row["Area"]);

                decimal nangSuat = (area == 0) ? 0 : harvestQty / area;

                result.Rows.Add(
                    row["SKU"],
                    row["ProcessDate_Month"],
                    row["ProcessDate_Year"],
                    row["ProductionOrder"],
                    row["PlantName"],
                    harvestQty,
                    harvestQty1,
                    harvestQty2,
                    area,
                    nangSuat
                );
            }

            result.DefaultView.Sort = "PlantName ASC, ProductionOrder ASC";
            result = result.DefaultView.ToTable();

            return result;
        }

        private void monthYearDtp_ValueChanged(object sender, EventArgs e)
        {
            // Mỗi lần thay đổi thì reset timer
            _monthYearDebounceTimer.Stop();
            _monthYearDebounceTimer.Start();
        }

        private void MonthYearDebounceTimer_Tick(object sender, EventArgs e)
        {
            _monthYearDebounceTimer.Stop();
            
            int month = monthYearDtp.Value.Month;
            int year = monthYearDtp.Value.Year;

            if(year != _prevYear)
            {
                ShowData();
            }
            else
            {
                foreach(var item in mPlantingManagement_Departments)
                    item.Value.DefaultView.RowFilter = $"HarvestDate_Month = {month} AND HarvestDate_Year = {year}";
            }            
        }

        private async void ExportExcel_btn_Click(object sender, EventArgs e)
        {
            int month = monthYearDtp.Value.Month;
            int year = monthYearDtp.Value.Year;

            
            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Message = "Đang xử lý ...";
            loadingOverlay.Show();
            await Task.Delay(100);

            try
            {
                using (var wb = new XLWorkbook())
                {
                    Dictionary<string, string> departs = new Dictionary<string, string> { { "toa", "TỔ A" }, { "tob", "TỔ B" }, { "nhauom-thuycanh", "NHÀ ƯƠM - THỦY CANH" }, { "farmbt", "FARM BÌNH THUẬN" } };
                    List<string> titles = new List<string> { "Lệnh\nSản Xuất", "Tên\nSản Phẩm", "Sản Lượng\n(Kg)", "Diện Tích\n(m2)", "Năng Xuất\n(kg/m2)" };
                    List<XLColor> ColorTitles = new List<XLColor>{ XLColor.FromHtml("#FFF2CC"), XLColor.FromHtml("#D9E1F2"), XLColor.FromHtml("#C6E0B4"), XLColor.FromHtml("#F8CBAD") };
                    int numColPart = titles.Count;

                    int index = 0;
                    int columnPart = 1;
                    

                    var ws = wb.Worksheets.Add("Sản Lượng Tháng");
                    ws.Style.Font.FontName = "Times New Roman";
                    ws.Style.Font.FontSize = 12;
                    int rowExcel = 1;
                    ws.Cell(rowExcel, 1).Value = $"SẢN LƯỢNG CÂY TRỒNG THÁNG {month.ToString("d2")}/{year}";
                    ws.Range(rowExcel, 1, rowExcel, numColPart * departs.Count).Merge();
                    ws.Cell(rowExcel, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell(rowExcel, 1).Style.Font.Bold = true;
                    ws.Cell(rowExcel, 1).Style.Font.FontSize = 24;

                    int startRowTable = rowExcel + 1;
                    int maxRow = startRowTable;
                   
                    foreach (var depart in departs)
                    {
                        int colExcel = columnPart + index * numColPart;
                        rowExcel = startRowTable;

                        if (!mPlantingManagement_Departments.TryGetValue(depart.Key, out var plantingManagement_dt)|| plantingManagement_dt == null)
                        {
                            continue;
                        }

                        ws.Cell(rowExcel, colExcel).Value = depart.Value;
                        ws.Range(rowExcel, colExcel, rowExcel, colExcel + numColPart - 1).Merge();
                        ws.Cell(rowExcel, colExcel).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell(rowExcel, colExcel).Style.Font.Bold = true;
                        ws.Cell(rowExcel, colExcel).Style.Fill.BackgroundColor = ColorTitles[index];

                        rowExcel++;
                        foreach (var title in titles)
                        {
                            ws.Cell(rowExcel, colExcel).Value = title;
                            ws.Cell(rowExcel, colExcel).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(rowExcel, colExcel).Style.Alignment.WrapText = true;
                            ws.Cell(rowExcel, colExcel).Style.Font.Bold = true;
                            ws.Cell(rowExcel, colExcel).Style.Fill.BackgroundColor = ColorTitles[index];
                            colExcel++;
                        }

                        rowExcel++;
                        colExcel = columnPart + index * numColPart;
                        foreach (DataRow row in plantingManagement_dt.Rows)
                        {
                            ws.Cell(rowExcel, colExcel).Value = row["ProductionOrder"].ToString();
                            ws.Cell(rowExcel, colExcel + 1).Value = row["PlantName"].ToString();
                            ws.Cell(rowExcel, colExcel + 2).Value = Convert.ToDecimal(row["HarvestQuantity"]);
                            ws.Cell(rowExcel, colExcel + 3).Value = Convert.ToDecimal(row["Area"]);
                            ws.Cell(rowExcel, colExcel + 4).Value = Convert.ToDecimal(row["NangSuatTrong"]);
                            rowExcel++;
                        }

                        if (maxRow < rowExcel)
                            maxRow = rowExcel;

                        index++;
                    }

                    ws.Range(2, 1, maxRow - 1, numColPart* departs.Count).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    ws.Range(2, 1, maxRow - 1, numColPart * departs.Count).Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                    ws.Columns().AdjustToContents();

                    using (SaveFileDialog sfd = new SaveFileDialog())
                    {
                        sfd.Filter = "Excel Workbook|*.xlsx";
                        sfd.FileName = $"sanluongtrong_Thang{month.ToString("D2")}{year.ToString()}";
                        if (sfd.ShowDialog() == DialogResult.OK)
                        {
                            wb.SaveAs(sfd.FileName);
                            DialogResult result = MessageBox.Show("Bạn có muốn mở file này không?", "Lưu file thành công", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (result == DialogResult.Yes)
                            {
                                try
                                {
                                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                                    {
                                        FileName = sfd.FileName,
                                        UseShellExecute = true
                                    });
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("Không thể mở file: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xuất Excel: " + ex.Message);
            }
            finally
            {
                await Task.Delay(200);
                loadingOverlay.Hide();
            }
        }
    }
}
