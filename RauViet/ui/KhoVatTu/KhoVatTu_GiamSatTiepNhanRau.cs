using ClosedXML.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using RauViet.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RauViet.ui
{
    public partial class KhoVatTu_GiamSatTiepNhanRau : Form
    {
        DataTable mExportCode_dt, mOrdersTotal_dt, mGlobalGapProducts_dt, mPlanting_dt;
        int mCurrentExportID = -1;
        private LoadingOverlay loadingOverlay;
        public KhoVatTu_GiamSatTiepNhanRau()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;


            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            this.KeyDown += KhoVatTu_KeyDown;
            exportExcel_btn.Click += ExportExcel_btn_Click;

            saveGlobalGAP_btn.Click += SaveGlobalGAP_btn_Click;
            dataGV.CellDoubleClick += DataGV_CellDoubleClick;
        }

        private void KhoVatTu_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                if (mCurrentExportID <= 0)
                {
                    return;
                }

                SQLStore_Kho.Instance.removeOrdersChotPhyto(mCurrentExportID);
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
                string[] keepColumns = { "ExportCodeID", "ExportCodeIndex", "ExportCode", "ExportDate" };
                string[] empKeepColumns = { "EmployeeCode", "FullName", "EmployessName_NoSign" };
                var parameters = new Dictionary<string, object> { { "Complete", false } };

                mExportCode_dt = await SQLStore_Kho.Instance.getExportCodesAsync(keepColumns, parameters);

                var GlobalGapProductsTask = SQLStore_KhoVatTu.Instance.getGlobalGapProductsAsync();                
                var employeeTask =  SQLStore_QLNS.Instance.GetEmployeesAsync(empKeepColumns);
                var departmentTask = SQLStore_QLNS.Instance.GetDepartmentAsync();
                await Task.WhenAll(GlobalGapProductsTask, employeeTask, departmentTask);
                mGlobalGapProducts_dt = GlobalGapProductsTask.Result;

                DateTime ExportDate =DateTime.Now;
                if (mCurrentExportID <= 0 && mExportCode_dt.Rows.Count > 0)
                {
                    mCurrentExportID = mExportCode_dt.AsEnumerable().Max(r => r.Field<int>("ExportCodeID"));                    
                }

                var row = mExportCode_dt.AsEnumerable().FirstOrDefault(r => r.Field<int>("ExportCodeID") == mCurrentExportID);
                if (row != null)
                {
                    ExportDate = row.Field<DateTime>("ExportDate");
                }

                var OrdersTotalTask = SQLStore_Kho.Instance.getOrdersChotPhytosync(mCurrentExportID);
                var LOTCodeTask = SQLStore_Kho.Instance.GetLOTCodeAsync(mCurrentExportID);
                await Task.WhenAll(LOTCodeTask, OrdersTotalTask);

                mOrdersTotal_dt = new DataTable();

                
                // Cột cố định
                mOrdersTotal_dt.Columns.Add("Date", typeof(DateTime));
                mOrdersTotal_dt.Columns.Add("PlantName", typeof(string));
                mOrdersTotal_dt.Columns.Add("LotCOT", typeof(string));
                mOrdersTotal_dt.Columns.Add($"CamQUan", typeof(string));
                mOrdersTotal_dt.Columns.Add($"Quantity_BanDau", typeof(decimal));
                mOrdersTotal_dt.Columns.Add($"Quantity_ThanhPham", typeof(decimal));
                mOrdersTotal_dt.Columns.Add($"ActualPacking", typeof(string));
                mOrdersTotal_dt.Columns.Add($"ExcessHandling", typeof(string));
                mOrdersTotal_dt.Columns.Add($"SoKien", typeof(int));
                foreach (DataRow ggpRow in mGlobalGapProducts_dt.Rows)
                {
                    int DaysBeforeExport = Convert.ToInt32(ggpRow["DaysBeforeExport"]);
                    int sku = Convert.ToInt32(ggpRow["SKU"]);
                    string CamQuan = Convert.ToString(ggpRow["CamQuan"]);
                    decimal UsageRate = Convert.ToDecimal(ggpRow["UsageRate"]);
                    string ActualPacking = Convert.ToString(ggpRow["ActualPacking"]);
                    string ExcessHandling = Convert.ToString(ggpRow["ExcessHandling"]);
                    int PortionsPerBox = Convert.ToInt32(ggpRow["PortionsPerBox"]);

                    var orderRow = OrdersTotalTask.Result.AsEnumerable().FirstOrDefault(r => r.Field<int>("GroupProduct") == sku);

                    var LOTRow = LOTCodeTask.Result.AsEnumerable().FirstOrDefault(r =>
                                                                        r.Field<int>("GroupProduct") == sku &&
                                                                        !string.IsNullOrWhiteSpace(r["LOTCodeComplete"]?.ToString())
                                                                    );

                    if (orderRow == null && LOTRow == null) continue;


                    DataRow newRow = mOrdersTotal_dt.NewRow();
                    newRow["Date"] = ExportDate.AddDays(DaysBeforeExport);
                    newRow["PlantName"] = ggpRow["PlantName"];
                    newRow["CamQUan"] = ggpRow["CamQUan"];
                    newRow["ActualPacking"] = ggpRow["ActualPacking"];
                    newRow["ExcessHandling"] = ggpRow["ExcessHandling"];
                    

                    if (LOTRow != null)
                        newRow["LotCOT"] = LOTRow["LOTCodeComplete"];

                    if (orderRow != null)
                    {
                        newRow["Quantity_ThanhPham"] = orderRow["TotalNetWeight"];
                        newRow["Quantity_BanDau"] = Convert.ToDecimal(orderRow["TotalNetWeight"]) / (Convert.ToDecimal(ggpRow["UsageRate"]) / 100.0m);
                        if (sku == 121 || sku == 101)
                            newRow["SoKien"] = Math.Ceiling(Convert.ToDecimal(orderRow["TotalNetWeight"]) / (PortionsPerBox));
                        else
                            newRow["SoKien"] = Math.Ceiling((Convert.ToDecimal(orderRow["TotalNetWeight"]) * 10) / (PortionsPerBox));
                    }

                    mOrdersTotal_dt.Rows.Add(newRow);
                }

                List<string> productionOrders = new List<string>();
                foreach (DataRow rowItem in mOrdersTotal_dt.Rows)
                {
                    string LotCOT = rowItem["LotCOT"].ToString().Trim();
                    string productionOrder = LotCOT.Length > 2 ? LotCOT.Substring(0, LotCOT.Length - 2) : LotCOT;
                    if (string.IsNullOrWhiteSpace(productionOrder) == false && productionOrders.Contains(productionOrder) == false)
                        productionOrders.Add(productionOrder);
                }
                mPlanting_dt = await SQLStore_KhoVatTu.Instance.getPlantingManagementAsync_ProductionOrder(productionOrders);

                dataGV.DataSource = mOrdersTotal_dt;

                Utils.SetGridFormat_NO(dataGV, "SoKien");
                Utils.SetGridFormat_N2(dataGV, "Quantity_BanDau");
                Utils.SetGridHeaders(dataGV, new System.Collections.Generic.Dictionary<string, string> {
                        {"Date", "Ngày" },
                        {"PlantName", "Tên Sản Phẩm" },
                        {"LotCOT", "Mã LOT" },
                        {"CamQUan", "Cảm Quan" },
                        {"Quantity_BanDau", "SL Ban Đầu" },
                        {"Quantity_ThanhPham", "SL Thành Phẩm" },
                        {"ActualPacking", "Quy Cách Đ.Gói" },
                        {"ExcessHandling", "Xử Lý Hàng Tồn" },
                        {"SoKien", "Số Kiện" }
                    });

                Utils.SetGridWidths(dataGV, new System.Collections.Generic.Dictionary<string, int> {
                        {"Date", 65 },
                        {"PlantName", 120 },
                        {"LotCOT", 110 },
                        {"CamQUan", 100 },
                        {"Quantity_BanDau", 100 },
                        {"Quantity_ThanhPham", 100 },
                        {"ActualPacking", 100 },
                        {"ExcessHandling", 150 },
                        {"SoKien", 100 }
                    });

                exportCode_cbb.SelectedIndexChanged -= exportCode_search_cbb_SelectedIndexChanged;
                exportCode_cbb.DataSource = mExportCode_dt;
                exportCode_cbb.DisplayMember = "ExportCode";  // hiển thị tên
                exportCode_cbb.ValueMember = "ExportCodeID";
                exportCode_cbb.SelectedValue = mCurrentExportID;
                exportCode_cbb.SelectedIndexChanged += exportCode_search_cbb_SelectedIndexChanged;

                ngPhuTrach_cbb.DataSource = employeeTask.Result.Copy();
                ngPhuTrach_cbb.DisplayMember = "FullName";  // hiển thị tên
                ngPhuTrach_cbb.ValueMember = "EmployeeCode";
                ngPhuTrach_cbb.SelectedValue = Properties.Settings.Default.MaNguoiPhuTrach_GlobalGap; 

                ngThuHoach_cbb.DataSource = employeeTask.Result.Copy();
                ngThuHoach_cbb.DisplayMember = "FullName";  // hiển thị tên
                ngThuHoach_cbb.ValueMember = "EmployeeCode";
                ngThuHoach_cbb.SelectedValue = Properties.Settings.Default.MaToTruong_ThuHoach;

                noiNhan_cbb.DataSource = departmentTask.Result;
                noiNhan_cbb.DisplayMember = "DepartmentName";  // hiển thị tên
                noiNhan_cbb.ValueMember = "DepartmentID";
                noiNhan_cbb.SelectedValue = Properties.Settings.Default.ToNhanSPThuHoach;
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

        private async void exportCode_search_cbb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!int.TryParse(exportCode_cbb.SelectedValue.ToString(), out int exportCodeId))
                return;

            mCurrentExportID = exportCodeId;
            ShowData();
        }

        private async void ExportExcel_btn_Click(object sender, EventArgs e)
        {
            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Message = "Đang xử lý ...";
            loadingOverlay.Show();
            await Task.Delay(100);

            try
            {
                using (var wb = new XLWorkbook())
                {
                    var ws_TiepNhan = wb.Worksheets.Add("Tiepnhan");
                    Excel_TiepNhan(ws_TiepNhan);

                    var ws_BaoQuan = wb.Worksheets.Add("Baoquan");
                    Excel_BaoQuan(ws_BaoQuan);

                    var ws_Dongkien = wb.Worksheets.Add("Dongkien xuathang");
                    Excel_DongKienXuatHang(ws_Dongkien);

                    DateTime ExportDate = DateTime.Now;
                    int ExportCodeIndex = 0;

                    var row1 = mExportCode_dt.AsEnumerable().FirstOrDefault(r => r.Field<int>("ExportCodeID") == mCurrentExportID);
                    if (row1 != null)
                    {
                        ExportDate = row1.Field<DateTime>("ExportDate");
                        ExportCodeIndex = row1.Field<int>("ExportCodeIndex");
                    }


                    //ws.Column(8).Width = 27;
                    // ===== Save file =====
                    using (SaveFileDialog sfd = new SaveFileDialog())
                    {
                        sfd.Filter = "Excel Workbook|*.xlsx";
                        sfd.FileName = $"VRF_SOCHE_chuyen_{ExportDate.ToString("dd.MM")}_({ExportCodeIndex.ToString("D3")})";
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

        void Excel_TiepNhan(IXLWorksheet ws)
        {
            ws.Style.Font.FontName = "Times New Roman";
            ws.Style.Font.FontSize = 12;
            ws.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            int rowInd = 1;
            int columnInd = 1;

            var image = Properties.Resources.logo_vr_1;

            using (var ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ms.Position = 0;

                var picture = ws.AddPicture(ms)
                                .MoveTo(ws.Cell(rowInd, columnInd));
            }

            columnInd += 1;
            ws.Range(rowInd, columnInd, rowInd, columnInd + 4).Merge();
            ws.Cell(rowInd, columnInd).Value = "GIÁM SÁT QUÁ TRÌNH TIẾP NHẬN RAU";
            ws.Cell(rowInd, columnInd).Style.Font.Bold = true;
            ws.Cell(rowInd, columnInd).Style.Font.FontSize = 14;
            ws.Cell(rowInd, columnInd).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Range(rowInd, columnInd, rowInd, columnInd + 4).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            columnInd += 5;
            ws.Cell(rowInd, columnInd).Value = "Số hiệu:\nNgày ban hành:\nLần ban hành:\nSố trang:";
            ws.Cell(rowInd, columnInd).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws.Cell(rowInd, columnInd).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Cell(rowInd, columnInd).Style.Alignment.WrapText = true;

            columnInd += 1;
            ws.Cell(rowInd, columnInd).Value = Utils.VRF_GAP_TiepNhan();
            ws.Cell(rowInd, columnInd).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws.Cell(rowInd, columnInd).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Cell(rowInd, columnInd).Style.Alignment.WrapText = true;

            rowInd = 3;
            columnInd = 1;
            string[] columnsName = new string[] { "Ngày", "Loại Sản Phẩm", "Mã Số Lô", "Cảm Quan\n(đ/k)", "Ban Đầu", "Thành Phẩm", "Quy Cách\nThực Tế(gr)", "Xử Lý\nSản Phẩm Dư Thừa" };
            string[] exportColumns = new string[] { "Date", "PlantName", "LotCOT", "CamQUan", "Quantity_BanDau", "Quantity_ThanhPham", "ActualPacking", "ExcessHandling" };
            string[] typeColumns = new string[] { "date", "string", "string", "string", "decimal", "decimal", "string", "string" };
            float[] widthColumns = new float[] { 16.5f, 16.5f, 12, 15, 12.2f, 12.2f, 14.3f, 20.5f };
            foreach (var tile in columnsName)
            {
                var titleCell = ws.Cell(rowInd, columnInd);
                if (tile.CompareTo("Ban Đầu") != 0 && tile.CompareTo("Thành Phẩm") != 0)
                {
                    ws.Range(rowInd, columnInd, rowInd + 1, columnInd).Merge();
                    ws.Range(rowInd, columnInd, rowInd + 1, columnInd).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                }
                else
                {
                    if (tile.CompareTo("Ban Đầu") == 0)
                    {
                        ws.Range(rowInd, columnInd, rowInd, columnInd + 1).Merge();
                        titleCell = ws.Cell(rowInd, columnInd);
                        titleCell.Style.Font.Bold = true;
                        titleCell.Style.Alignment.WrapText = true;
                        titleCell.Value = "Khối Lượng (kg)";
                        titleCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Range(rowInd, columnInd, rowInd, columnInd + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Đóng khung
                    }

                    titleCell = ws.Cell(rowInd + 1, columnInd);
                    titleCell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                }


                titleCell.Style.Font.Bold = true;
                titleCell.Style.Alignment.WrapText = true;
                titleCell.Value = tile;
                titleCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                // Đóng khung

                columnInd++;
            }

            rowInd += 2;

            foreach (DataRow row in mOrdersTotal_dt.Rows)
            {
                for (int i = 0; i < exportColumns.Length; i++)
                {
                    columnInd = i + 1;
                    var valueCell = ws.Cell(rowInd, columnInd);
                    valueCell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    if (!mOrdersTotal_dt.Columns.Contains(exportColumns[i]))
                        continue;

                    string valueStr = row[exportColumns[i]].ToString();
                    if (!string.IsNullOrEmpty(valueStr))
                    {

                        if (typeColumns[i].CompareTo("int") == 0)
                            valueCell.Value = Convert.ToInt32(valueStr);
                        else if (typeColumns[i].CompareTo("date") == 0)
                            valueCell.Value = Convert.ToDateTime(valueStr);
                        else if (typeColumns[i].CompareTo("decimal") == 0)
                        {
                            valueCell.Value = Convert.ToDecimal(valueStr);
                            valueCell.Style.NumberFormat.Format = "0.0";
                        }
                        else
                        {
                            valueCell.Value = valueStr;

                            if (exportColumns[i].CompareTo("CamQUan") == 0)
                                valueCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            else
                                valueCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        }
                    }
                }

                rowInd++;
            }

            var hdkpCell = ws.Cell(rowInd, 1);
            hdkpCell.Value = "HĐKP/PN";
            hdkpCell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Range(rowInd, 2, rowInd, 8).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Row(rowInd).Height = 45;

            rowInd += 2;
            ws.Cell(rowInd, 1).Value = "Ghi chú: Tần suất giám sát = mỗi lần nhập hàng";
            ws.Range(rowInd, 1, rowInd, 4).Merge();

            rowInd++;
            ws.Cell(rowInd, 1).Value = "Giám sát:";
            ws.Cell(rowInd, 3).Value = "Ngày thẩm tra:";
            ws.Cell(rowInd, 5).Value = "Người thẩm tra:";

            for (int i = 0; i < widthColumns.Length; i++)
            {
                ws.Column(i + 1).Width = widthColumns[i];
            }
            ws.Row(1).Height = 63;
        }

        void Excel_BaoQuan(IXLWorksheet ws)
        {
            ws.Style.Font.FontName = "Times New Roman";
            ws.Style.Font.FontSize = 12;
            ws.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            int rowInd = 1;
            int columnInd = 1;

            var image = Properties.Resources.logo_vr_1;

            using (var ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ms.Position = 0;

                var picture = ws.AddPicture(ms)
                                .MoveTo(ws.Cell(rowInd, columnInd));
            }

            columnInd += 1;
            ws.Range(rowInd, columnInd, rowInd, columnInd + 4).Merge();
            ws.Cell(rowInd, columnInd).Value = "GIÁM SÁT QUÁ TRÌNH \r\nBẢO QUẢN SẢN PHẨM \r\nTRONG KHO MÁT";
            ws.Cell(rowInd, columnInd).Style.Font.Bold = true;
            ws.Cell(rowInd, columnInd).Style.Font.FontSize = 14;
            ws.Cell(rowInd, columnInd).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Range(rowInd, columnInd, rowInd, columnInd + 4).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            columnInd += 5;
            ws.Cell(rowInd, columnInd).Value = "Số hiệu:\nNgày ban hành:\nLần ban hành:\nSố trang:";
            ws.Cell(rowInd, columnInd).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws.Cell(rowInd, columnInd).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Cell(rowInd, columnInd).Style.Alignment.WrapText = true;

            columnInd += 1;
            ws.Cell(rowInd, columnInd).Value = Utils.VRF_GAP_BaoQuan();
            ws.Cell(rowInd, columnInd).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws.Cell(rowInd, columnInd).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Cell(rowInd, columnInd).Style.Alignment.WrapText = true;

            rowInd = 3;
            columnInd = 1;
            string[] columnsName = new string[] { "Ngày", "Thời Điểm", "Kho NL/BTP", "Kho TP", "Ngày", "Thời Điềm", "Kho NL/BTP", "Kho TP" };
            float[] widthColumns = new float[] { 16.5f, 16.5f, 12, 15, 12.2f, 16.5f, 16.5f, 17.5f };

            DateTime ExportDate = DateTime.Now;
            int ExportCodeIndex = 0;
            var row1 = mExportCode_dt.AsEnumerable().FirstOrDefault(r => r.Field<int>("ExportCodeID") == mCurrentExportID);
            if (row1 != null)
            {
                ExportDate = row1.Field<DateTime>("ExportDate");
                ExportCodeIndex = row1.Field<int>("ExportCodeIndex");
            }

            foreach (var tile in columnsName)
            {
                var titleCell = ws.Cell(rowInd, columnInd);
                if (tile.CompareTo("Kho NL/BTP") != 0 && tile.CompareTo("Kho TP") != 0)
                {
                    ws.Range(rowInd, columnInd, rowInd + 1, columnInd).Merge();
                    ws.Range(rowInd, columnInd, rowInd + 1, columnInd).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                }
                else
                {
                    if (tile.CompareTo("Kho NL/BTP") == 0)
                    {
                        ws.Range(rowInd, columnInd, rowInd, columnInd + 1).Merge();
                        titleCell = ws.Cell(rowInd, columnInd);
                        titleCell.Style.Font.Bold = true;
                        titleCell.Style.Alignment.WrapText = true;
                        titleCell.Value = "Nhiệt độ (0C)";
                        titleCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Range(rowInd, columnInd, rowInd, columnInd + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Đóng khung
                    }

                    titleCell = ws.Cell(rowInd + 1, columnInd);
                    titleCell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                }

                titleCell.Style.Font.Bold = true;
                titleCell.Style.Alignment.WrapText = true;
                titleCell.Value = tile;
                titleCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                columnInd++;
            }

            var rd = new Random();

            // Hàm random + làm tròn 1 số lẻ
            double Rand1(double min, double range)
            {
                return Math.Round(min + rd.NextDouble() * range, 1);
            }

            // Cột 3 (7 → 7.5)
            ws.Cell(5, 3).Value = Rand1(7, 0.5);
            ws.Cell(6, 3).Value = Rand1(7, 0.5);
            ws.Cell(7, 3).Value = Rand1(7, 0.5);

            // Cột 4 (7.7 → 8.0)
            ws.Cell(5, 4).Value = Rand1(7.7, 0.3);
            ws.Cell(6, 4).Value = Rand1(7.7, 0.3);
            ws.Cell(7, 4).Value = Rand1(7.7, 0.3);

            // Cột 7 (7.9 → 8.2)
            ws.Cell(5, 7).Value = Rand1(7.9, 0.3);
            ws.Cell(6, 7).Value = Rand1(7.9, 0.3);
            ws.Cell(7, 7).Value = Rand1(7.9, 0.3);

            // Cột 8 (8.6 → 9.0)
            ws.Cell(5, 8).Value = Rand1(8.6, 0.4);
            ws.Cell(6, 8).Value = Rand1(8.6, 0.4);
            ws.Cell(7, 8).Value = Rand1(8.6, 0.4);

            // Set format đúng tất cả các ô
            ws.Range(5, 3, 7, 8).Style.NumberFormat.Format = "0.0";
            ws.Range(5, 1, 7, 8).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Range(5, 1, 7, 8).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            ws.Range(5, 1, 7, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Cell(5, 1).Value = ExportDate.AddDays(-3); ws.Cell(5, 1).Style.DateFormat.Format = "dd/MM/yyyy";
            ws.Cell(6, 1).Value = ExportDate.AddDays(-2); ws.Cell(6, 1).Style.DateFormat.Format = "dd/MM/yyyy";
            ws.Cell(7, 1).Value = ExportDate.AddDays(-1); ws.Cell(7, 1).Style.DateFormat.Format = "dd/MM/yyyy";
            ws.Cell(5, 5).Value = ExportDate.AddDays(-3); ws.Cell(5, 5).Style.DateFormat.Format = "dd/MM/yyyy";
            ws.Cell(6, 5).Value = ExportDate.AddDays(-2); ws.Cell(6, 5).Style.DateFormat.Format = "dd/MM/yyyy";
            ws.Cell(7, 5).Value = ExportDate.AddDays(-1); ws.Cell(7, 5).Style.DateFormat.Format = "dd/MM/yyyy";

            ws.Cell(5, 2).Value = new TimeSpan(9, 0, 0); ws.Cell(5, 2).Style.DateFormat.Format = "HH:mm";
            ws.Cell(6, 2).Value = new TimeSpan(9, 0, 0); ws.Cell(6, 2).Style.DateFormat.Format = "HH:mm";
            ws.Cell(7, 2).Value = new TimeSpan(9, 0, 0); ws.Cell(7, 2).Style.DateFormat.Format = "HH:mm";
            ws.Cell(5, 6).Value = new TimeSpan(15, 0, 0); ws.Cell(5, 6).Style.DateFormat.Format = "HH:mm";
            ws.Cell(6, 6).Value = new TimeSpan(15, 0, 0); ws.Cell(6, 6).Style.DateFormat.Format = "HH:mm";
            ws.Cell(7, 6).Value = new TimeSpan(15, 0, 0); ws.Cell(7, 6).Style.DateFormat.Format = "HH:mm";

            rowInd = 8;
            var hdkpCell = ws.Cell(rowInd, 1);
            hdkpCell.Value = "HĐKP/PN";
            hdkpCell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Range(rowInd, 2, rowInd, 8).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Row(rowInd).Height = 45;

            rowInd += 2;
            ws.Cell(rowInd, 1).Value = "Ghi chú: Tần suất giám sát = tối thiểu 2 lần/ngày";
            ws.Range(rowInd, 1, rowInd, 4).Merge();

            rowInd++;
            ws.Cell(rowInd, 1).Value = "Giám sát:";
            ws.Cell(rowInd, 3).Value = "Ngày thẩm tra:";
            ws.Cell(rowInd, 5).Value = "Người thẩm tra:";
            ws.Cell(rowInd, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            for (int i = 0; i < widthColumns.Length; i++)
            {
                ws.Column(i + 1).Width = widthColumns[i];
            }
            ws.Row(1).Height = 63;
        }

        void Excel_DongKienXuatHang(IXLWorksheet ws)
        {
            ws.Style.Font.FontName = "Times New Roman";
            ws.Style.Font.FontSize = 12;
            ws.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            int rowInd = 1;
            int columnInd = 1;

            DateTime ExportDate = DateTime.Now;
            int ExportCodeIndex = 0;
            var row1 = mExportCode_dt.AsEnumerable().FirstOrDefault(r => r.Field<int>("ExportCodeID") == mCurrentExportID);
            if (row1 != null)
            {
                ExportDate = row1.Field<DateTime>("ExportDate");
                ExportCodeIndex = row1.Field<int>("ExportCodeIndex");
            }

            var image = Properties.Resources.logo_vr_1;

            using (var ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ms.Position = 0;

                var picture = ws.AddPicture(ms)
                                .MoveTo(ws.Cell(rowInd, columnInd));
            }

            columnInd += 1;
            ws.Range(rowInd, columnInd, rowInd, columnInd + 5).Merge();
            ws.Cell(rowInd, columnInd).Value = "GIÁM SÁT QUÁ TRÌNH ĐÓNG KIỆN - XUẤT HÀNG";
            ws.Cell(rowInd, columnInd).Style.Font.Bold = true;
            ws.Cell(rowInd, columnInd).Style.Font.FontSize = 14;
            ws.Cell(rowInd, columnInd).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Range(rowInd, columnInd, rowInd, columnInd + 5).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            columnInd += 6;
            ws.Cell(rowInd, columnInd).Value = "Số hiệu:\nNgày ban hành:\nLần ban hành:\nSố trang:";
            ws.Cell(rowInd, columnInd).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws.Cell(rowInd, columnInd).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Cell(rowInd, columnInd).Style.Alignment.WrapText = true;

            columnInd += 1;
            ws.Cell(rowInd, columnInd).Value = Utils.VRF_GAP();
            ws.Cell(rowInd, columnInd).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws.Cell(rowInd, columnInd).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Cell(rowInd, columnInd).Style.Alignment.WrapText = true;

            rowInd = 3;
            columnInd = 1;
            string[] exportColumns = new string[] { "Date", "PlantName", "LotCOT", "CamQUan", "Nhan", "SoKien", "DaGel", "NepDai", "KhachHang" };
            string[] typeColumns = new string[] { "date", "string", "string", "string", "decimal", "decimal", "string", "string" };
            float[] widthColumns = new float[] { 16.5f, 16.5f, 12, 10.5f, 10.5f, 14.6f, 7.3f, 14f, 17.5f };

            ws.Range(rowInd, 1, rowInd + 1, 1).Merge();
            ws.Range(rowInd, 9, rowInd + 1, 9).Merge();
            ws.Range(rowInd, 2, rowInd, 6).Merge();
            ws.Range(rowInd, 7, rowInd, 8).Merge();

            ws.Cell(rowInd, 1).Value = "Ngày";
            ws.Cell(rowInd, 2).Value = "Sản Phẩm";
            ws.Cell(rowInd + 1, 2).Value = "Loại";
            ws.Cell(rowInd + 1, 3).Value = "Mã Số Lô";
            ws.Cell(rowInd + 1, 4).Value = "Cảm Quan\n(đ/k)";
            ws.Cell(rowInd + 1, 5).Value = "Nhãn (đ/k)";
            ws.Cell(rowInd + 1, 6).Value = "Số Kiện (Kiện)";
            ws.Cell(rowInd, 7).Value = "Quy Cách";
            ws.Cell(rowInd + 1, 7).Value = "Đá Gel";
            ws.Cell(rowInd + 1, 8).Value = "Nẹp Đai";
            ws.Cell(rowInd, 9).Value = "Khách Hàng";

            ws.Range(rowInd, 1, rowInd + 1, 9).Style.Font.Bold = true;
            ws.Range(rowInd, 1, rowInd + 1, 9).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Range(rowInd, 1, rowInd + 1, 9).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Range(rowInd, 1, rowInd + 1, 9).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            rowInd += 2;

            foreach (DataRow row in mOrdersTotal_dt.Rows)
            {
                for (int i = 0; i < exportColumns.Length; i++)
                {
                    columnInd = i + 1;
                    var valueCell = ws.Cell(rowInd, columnInd);
                    valueCell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;                    

                    if (exportColumns[i].CompareTo("Date") == 0) {
                        valueCell.Value = ExportDate.Date;
                        valueCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    }
                    else if (exportColumns[i].CompareTo("Nhan") == 0 
                        || exportColumns[i].CompareTo("DaGel") == 0 
                        || exportColumns[i].CompareTo("NepDai") == 0) {
                        valueCell.Value = "đ";
                        valueCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    }
                    else if (exportColumns[i].CompareTo("KhachHang") == 0)
                    {
                        valueCell.Value = "Asiaway AG";
                        valueCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    }
                    else
                    {
                        if (!mOrdersTotal_dt.Columns.Contains(exportColumns[i]))
                            continue;

                        string valueStr = row[exportColumns[i]].ToString();
                        if (!string.IsNullOrEmpty(valueStr))
                        {

                            if (typeColumns[i].CompareTo("int") == 0)
                                valueCell.Value = Convert.ToInt32(valueStr);
                            else if (typeColumns[i].CompareTo("decimal") == 0)
                            {
                                valueCell.Value = Convert.ToDecimal(valueStr);
                                valueCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                            }
                            else
                            {
                                valueCell.Value = valueStr;
                                if(exportColumns[i].CompareTo("CamQUan") == 0)
                                    valueCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                else
                                    valueCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                            }
                        }
                    }

                    
                }

                rowInd++;
            }

            var hdkpCell = ws.Cell(rowInd, 1);
            hdkpCell.Value = "HĐKP/PN";
            hdkpCell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Range(rowInd, 2, rowInd, 9).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Row(rowInd).Height = 45;

            rowInd += 2;
            ws.Cell(rowInd, 1).Value = "Ghi chú: Tần suất giám sát = mỗi lần nhập hàng";
            ws.Range(rowInd, 1, rowInd, 4).Merge();

            rowInd++;
            ws.Cell(rowInd, 1).Value = "QC";
            ws.Cell(rowInd, 4).Value = "Thủ kho";
            ws.Cell(rowInd, 7).Value = "Quản lý trang trại";
            ws.Range(rowInd, 7, rowInd, 8).Merge();

            for (int i = 0; i < widthColumns.Length; i++)
            {
                ws.Column(i + 1).Width = widthColumns[i];
            }
            ws.Row(1).Height = 63;
        }

        private async void SaveGlobalGAP_btn_Click(object sender, EventArgs e)
        {
            if (noiNhan_cbb.SelectedValue == null || ngThuHoach_cbb.SelectedValue == null || ngPhuTrach_cbb.SelectedValue == null || string.IsNullOrEmpty(noiNhan_cbb.Text) || string.IsNullOrEmpty(ngThuHoach_cbb.Text) || string.IsNullOrEmpty(ngPhuTrach_cbb.Text))
                return;

            string ngThuHoachCode = ngThuHoach_cbb.SelectedValue.ToString();
            string ngPhuTrach = ngPhuTrach_cbb.SelectedValue.ToString();
            int departID = Convert.ToInt32(noiNhan_cbb.SelectedValue);

            List <(int PlantingID, DateTime HarvestDate, decimal Quantity, string ProductLotCode, string HarvestEmployee, string SupervisorEmployee, int ReceiveDepartmentID)> data = new List<(int, DateTime, decimal, string, string, string, int)>();

            

            foreach (DataRow rowItem in mOrdersTotal_dt.Rows)
            {
                string LotCOT = rowItem["LotCOT"].ToString().Trim();
                string productionOrder = LotCOT.Length > 2 ? LotCOT.Substring(0, LotCOT.Length - 2) : LotCOT;

                var plantingRow = mPlanting_dt.AsEnumerable() .Where(r => r.Field<string>("ProductionOrder") == productionOrder && r.Field<int>("FarmID") == 2) .FirstOrDefault();
                if(plantingRow != null)
                {
                    int plantingID = Convert.ToInt32(plantingRow["PlantingID"]);
                    DateTime harvestDate = Convert.ToDateTime(rowItem["Date"]);
                    decimal quantity = rowItem["Quantity_BanDau"] != DBNull.Value ? Convert.ToDecimal(rowItem["Quantity_BanDau"]): 0;

                    data.Add((plantingID, harvestDate, quantity, LotCOT, ngThuHoachCode, ngPhuTrach, departID));
                }
            }

            if (data.Count <= 0)
            {
                MessageBox.Show("Không có dữ liệu mẫu", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool isSuccess = await SQLManager_KhoVatTu.Instance.InsertHarvestScheduleGlobalGapListAsync(data);
            if (!isSuccess)
            {
                MessageBox.Show("Có Lỗi Xảy Ra", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                Properties.Settings.Default.MaToTruong_ThuHoach = ngThuHoachCode;
                Properties.Settings.Default.MaNguoiPhuTrach_GlobalGap = ngPhuTrach;
                Properties.Settings.Default.ToNhanSPThuHoach = departID;
                Properties.Settings.Default.Save();

                MessageBox.Show("Xong!", "SUCCESS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void DataGV_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!UserManager.Instance.hasRole_QLK_NhatKySanXuat())
                return;
            if (e.RowIndex < 0) return; // tránh click header

            DataRowView drv = dataGV.Rows[e.RowIndex].DataBoundItem as DataRowView;
            if (drv == null) return;

            DataRow row = drv.Row;

            string LotCOT = row["LotCOT"].ToString().Trim();
            string productionOrder = LotCOT.Length > 2 ? LotCOT.Substring(0, LotCOT.Length - 2) : LotCOT;

            var plantingRow = mPlanting_dt.AsEnumerable().FirstOrDefault(r => r.Field<string>("ProductionOrder") == productionOrder);
            if (plantingRow == null)
                return;

            KhoVatTu_CultivationProcess frm = new KhoVatTu_CultivationProcess(plantingRow, true);
            frm.ShowData();
            frm.ShowDialog(); // hoặc Show()
        }
    }
}
