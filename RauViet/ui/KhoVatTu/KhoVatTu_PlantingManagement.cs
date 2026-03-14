using ClosedXML.Excel;
using DocumentFormat.OpenXml.Vml.Office;
using RauViet.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Color = System.Drawing.Color;

namespace RauViet.ui
{
    public partial class KhoVatTu_PlantingManagement : Form
    {
        System.Data.DataTable mPlantingManagement_dt, mDepartment_dt, mProductSKU_dt, mEmployee_dt, mCultivationType_dt, mFarm_dt;
        DataView mLog_dv;
        private Timer productSKUDebounceTimer = new Timer { Interval = 300 };
        private Timer employeeDebounceTimer = new Timer { Interval = 300 };
        private Timer departmentDebounceTimer = new Timer { Interval = 300 };
        private Timer _monthYearDebounceTimer = new Timer { Interval = 500 };
        bool isNewState = false;
        private LoadingOverlay loadingOverlay;
        public KhoVatTu_PlantingManagement()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            monthYear_dtp.Format = DateTimePickerFormat.Custom;
            monthYear_dtp.CustomFormat = "yyyy";
            monthYear_dtp.ShowUpDown = true;
            monthYear_dtp.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            ngayUom_dtp.Format = DateTimePickerFormat.Custom;
            ngayUom_dtp.CustomFormat = "dddd, dd/MM/yyyy";
            ngaytrong_dtp.Format = DateTimePickerFormat.Custom;
            ngaytrong_dtp.CustomFormat = "dddd, dd/MM/yyyy";
            ngaythu_dtp.Format = DateTimePickerFormat.Custom;
            ngaythu_dtp.CustomFormat = "dddd, dd/MM/yyyy";

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;
            monthGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            monthGV.MultiSelect = false;
            toPhuTrach_GV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            toPhuTrach_GV.MultiSelect = false;
            weekInYear_GV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            weekInYear_GV.MultiSelect = false;

            status_lb.Text = "";

            newCustomerBtn.Click += newBtn_Click;
            LuuThayDoiBtn.Click += saveBtn_Click;
            edit_btn.Click += Edit_btn_Click;
            readOnly_btn.Click += ReadOnly_btn_Click;
            caytrong_CB.SelectedIndexChanged += Caytrong_CB_SelectedIndexChanged;
            farm_cbb.SelectedIndexChanged += Farm_cbb_SelectedIndexChanged;
            this.KeyDown += Kho_Materials_KeyDown;
            productSKUDebounceTimer.Tick += productSKUDebounceTimer_Tick;
            employeeDebounceTimer.Tick += employeeDebounceTimer_Tick;
            departmentDebounceTimer.Tick += departmentDebounceTimer_Tick;
            _monthYearDebounceTimer.Tick += MonthYearDebounceTimer_Tick;
            caytrong_CB.TextUpdate += productSKU_cbb_TextUpdate;
            nguoiPhuTrach_CB.TextUpdate += Employee_CBB_TextUpdate;
            deparment_CBB.TextUpdate += Department_CBB_TextUpdate;

            dientich_tb.KeyPress += Tb_KeyPress_OnlyNumber_decimal;
            soLuong_tb.KeyPress += Tb_KeyPress_OnlyNumber_int;
            dataGV.CellFormatting += dataGV_CellFormatting;
            dataGV.CellDoubleClick += DataGV_CellDoubleClick;

            search_tb.TextChanged += Search_tb_TextChanged;

            ngaytrong_dtp.ValueChanged += Ngaytrong_dtp_ValueChanged;
            ngayUom_dtp.ValueChanged += NgayUom_dtp_ValueChanged;

            monthGV.SelectionChanged += MonthGV_SelectionChanged;
            toPhuTrach_GV.SelectionChanged += MonthGV_SelectionChanged;
            weekInYear_GV.SelectionChanged += MonthGV_SelectionChanged;
            locTheoNgayThu_CB.CheckedChanged += Loc_CheckedChanged;
            locTheoNgayTrong_CB.CheckedChanged += Loc_CheckedChanged;
            locTheoNgayUom_CB.CheckedChanged += Loc_CheckedChanged;
            locTheoTo_CB.CheckedChanged += Loc_CheckedChanged;
            locTheoTuan_CB.CheckedChanged += Loc_CheckedChanged;

            inPhieuSX_btn.Click += InPhieuSX_btn_Click;
            xemPhieuSX_btn.Click += XemPhieuSX_btn_Click;
            excelPhieuSX_btn.Click += ExcelPhieuSX_btn_Click;

            inPhieuSX_uom_btn.Click += InPhieuSX_uom_btn_Click;
            xemPhieuSX_uom_btn.Click += XemPhieuSX_uom_btn_Click;
            excelPhieuSX_uom_btn.Click += ExcelPhieuSX_uom_btn_Click;

            inKeHoachSX_btn.Click += InKeHoachSX_btn_Click; ;
            xemKeHoachSX_btn.Click += XemKeHoachSX_btn_Click; ;
            excelKeHoachSX_btn.Click += ExcelKeHoachSX_btn_Click;

            inLichUom_btn.Click += InLichUom_btn_Click;
            xemLichUom_btn.Click += XemLichUom_btn_Click;
            excelLichUom_btn.Click += ExcelLichUom_btn_Click;
        }

        private void Kho_Materials_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                SQLStore_KhoVatTu.Instance.removePlantingManagementAsync(monthYear_dtp.Value.Year);
                ShowData();
            }
            else if (!isNewState && !edit_btn.Visible)
            {
                if (e.KeyCode == Keys.Delete)
                {
                    System.Windows.Forms.Control ctrl = this.ActiveControl;

                    if (ctrl is System.Windows.Forms.TextBox || ctrl is RichTextBox ||
                        (ctrl is DataGridView dgv && dgv.CurrentCell != null && dgv.IsCurrentCellInEditMode))
                    {
                        return; // không xử lý Delete
                    }

                    deleteProduct();
                }
            }
        }

        public async void ShowData()
        {
            dataGV.SelectionChanged -= this.dataGV_CellClick;
            await Task.Delay(50);
            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Show();

            try
            {
                monthYear_dtp.ValueChanged -= monthYearDtp_ValueChanged;
                var parameters = new Dictionary<string, object> { { "IsActive", true } };
                string[] keepColumns = { "EmployeeCode", "FullName", "EmployessName_NoSign" };

                var employeesTask = SQLStore_QLNS.Instance.GetEmployeesAsync(keepColumns);                
                var departmentTask = SQLStore_QLNS.Instance.GetDepartmentAsync();
                var cultivationTypeTask = SQLStore_KhoVatTu.Instance.GetCultivationTypeAsync();
                var productSKUTask = SQLStore_Kho.Instance.getProductSKUAsync(parameters);
                var plantingManagementTask = SQLStore_KhoVatTu.Instance.getPlantingManagementAsync(monthYear_dtp.Value.Year);
                var farmTask = SQLStore_KhoVatTu.Instance.GetFarmsAsync();
                var logTask = SQLStore_KhoVatTu.Instance.getPlantingManagementLogAsync(monthYear_dtp.Value.Year);
                await Task.WhenAll(departmentTask, productSKUTask, plantingManagementTask, employeesTask, cultivationTypeTask, logTask, farmTask);
                mFarm_dt = farmTask.Result;
                mDepartment_dt = departmentTask.Result;
                mProductSKU_dt = productSKUTask.Result;
                mPlantingManagement_dt = plantingManagementTask.Result;
                mEmployee_dt = employeesTask.Result;
                mCultivationType_dt = cultivationTypeTask.Result;

                DataTable department_dt = new DataTable();
                if (mPlantingManagement_dt.Rows.Count > 0)
                {
                    DataView dv = new DataView(mPlantingManagement_dt);
                    department_dt = dv.ToTable(true, "Department", "DepartmentName");
                }

                deparment_CBB.DataSource = mDepartment_dt;
                deparment_CBB.DisplayMember = "DepartmentName";  // hiển thị tên
                deparment_CBB.ValueMember = "DepartmentID";
                deparment_CBB.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

                caytrong_CB.DataSource = mProductSKU_dt;
                caytrong_CB.DisplayMember = "ProductNameVN";  // hiển thị tên
                caytrong_CB.ValueMember = "ProductSKU";
                caytrong_CB.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

                nguoiPhuTrach_CB.DataSource = mEmployee_dt;
                nguoiPhuTrach_CB.DisplayMember = "FullName";  // hiển thị tên
                nguoiPhuTrach_CB.ValueMember = "EmployeeCode";
                nguoiPhuTrach_CB.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

                cultivationType_CB.DataSource = mCultivationType_dt;
                cultivationType_CB.DisplayMember = "CultivationTypeName";  // hiển thị tên
                cultivationType_CB.ValueMember = "CultivationTypeID";

                farm_cbb.DataSource = mFarm_dt;
                farm_cbb.DisplayMember = "FarmName";  // hiển thị tên
                farm_cbb.ValueMember = "FarmID";

                mLog_dv = new DataView(logTask.Result);
                log_GV.DataSource = mLog_dv;
                dataGV.DataSource = mPlantingManagement_dt;
                monthGV.DataSource = Utils.CreateMonthsInYearTable();
                weekInYear_GV.DataSource = Utils.GetWeeksInYearTable(monthYear_dtp.Value.Year);
                toPhuTrach_GV.DataSource = department_dt;

                Utils.HideColumns(toPhuTrach_GV, new[] { "Department" });
                Utils.HideColumns(dataGV, new[] { "PlantingID", "SKU", "Department", "Supervisor", "CreatedAt", "search_nosign", "CultivationTypeID", "FarmID" });
                Utils.SetGridOrdinal(mPlantingManagement_dt, new[] { "IsCompleted", "FarmName", "ProductionOrder", "PlantName", "CultivationTypeName", "Quantity", "NurseryDate", "PlantingDate", "Area", "HarvestDate", "DepartmentName", "SupervisorName", "PlantLocation", "Note", "PlantingID", "Department" });
                Utils.SetGridHeaders(dataGV, new System.Collections.Generic.Dictionary<string, string> {
                        {"ProductionOrder", "Lệnh\nSản Xuất" },
                        {"Area", "Diện\nTích" },
                        {"Quantity", "S.Lượng\nCây" },
                        {"NurseryDate", "Ngày\nƯơm Hạt" },
                        {"PlantingDate", "Ngày\nTrồng" },
                        {"HarvestDate", "Ngày\nThu Hoạch" },
                        {"DepartmentName", "Tổ Phụ\nTrách Trồng" },
                        {"SupervisorName", "Giám Sát" },
                        {"PlantName", "Tên\nCây Trồng" },
                        {"IsCompleted", "Đóng Lệnh" },
                        {"Note", "Ghi Chú" },
                        {"PlantLocation", "V.Trí Trồng" },
                        {"CultivationTypeName", "Canh Tác" }
                    });

                Utils.SetGridWidths(dataGV, new System.Collections.Generic.Dictionary<string, int> {
                        {"ProductionOrder", 70},
                        {"Area", 60},
                        {"Quantity",60},
                        {"NurseryDate",70},
                        {"PlantingDate",70},
                        {"HarvestDate",70},
                        {"DepartmentName",130},
                        {"SupervisorName",150},
                        {"PlantName",100},
                        {"IsCompleted",60},
                        {"Note",250},
                        {"CultivationTypeName",100}
                    });

                Utils.HideColumns(weekInYear_GV, new[] { "WeekNumber", "StartDate", "EndDate" });
                Utils.HideColumns(monthGV, new[] { "Month"});
                Utils.SetGridHeaders(monthGV, new System.Collections.Generic.Dictionary<string, string> {{"MonthName", "Tháng" }});
                Utils.SetGridWidths(monthGV, new System.Collections.Generic.Dictionary<string, int> { { "MonthName", 70}});

                Utils.SetGridWidths(weekInYear_GV, new System.Collections.Generic.Dictionary<string, int> {
                        {"WeekName", 60}
                    });


                Utils.HideColumns(log_GV, new[] { "LogID"});
                Utils.SetGridHeaders(log_GV, new System.Collections.Generic.Dictionary<string, string> {
                        {"OldValue", "Cũ" },
                        {"NewValue", "Mới" },
                        {"CreatedDate", "Ngày tạo" },
                        {"ActionBy", "Người tạo" }
                    });
                Utils.SetGridWidths(log_GV, new System.Collections.Generic.Dictionary<string, int> {
                        {"OldValue", 400},
                        {"NewValue", 400},
                    });

                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                monthGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                Utils.SetTabStopRecursive(this, false);

                int countTab = 0;
                lenhSX_tb.TabIndex = countTab++; lenhSX_tb.TabStop = true;
                caytrong_CB.TabIndex = countTab++; caytrong_CB.TabStop = true;
                cultivationType_CB.TabIndex = countTab++; cultivationType_CB.TabStop = true;
                ngayUom_dtp.TabIndex = countTab++; ngayUom_dtp.TabStop = true;
                ngaytrong_dtp.TabIndex = countTab++; ngaytrong_dtp.TabStop = true;
                ngaythu_dtp.TabIndex = countTab++; ngaythu_dtp.TabStop = true;
                soLuong_tb.TabIndex = countTab++; soLuong_tb.TabStop = true;
                dientich_tb.TabIndex = countTab++; dientich_tb.TabStop = true;
                deparment_CBB.TabIndex = countTab++; deparment_CBB.TabStop = true;
                nguoiPhuTrach_CB.TabIndex = countTab++; nguoiPhuTrach_CB.TabStop = true;
                note_tb.TabIndex = countTab++; note_tb.TabStop = true;
                LuuThayDoiBtn.TabIndex = countTab++; LuuThayDoiBtn.TabStop = true;

                ReadOnly_btn_Click(null, null);
                dataGV.SelectionChanged += this.dataGV_CellClick;
                monthYear_dtp.ValueChanged += monthYearDtp_ValueChanged;

                if (UserManager.Instance.hasRole_QLK_QuanLyLichUom_ReadOnly())
                {
                    newCustomerBtn.Visible = false;
                    edit_btn.Visible = false;
                    readOnly_btn.Visible = false;
                }
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

        
        private void productSKU_cbb_TextUpdate(object sender, EventArgs e)
        {
            // Restart timer mỗi khi gõ
            productSKUDebounceTimer.Stop();
            productSKUDebounceTimer.Start();
        }

        private void productSKUDebounceTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                productSKUDebounceTimer.Stop();

                string typed = caytrong_CB.Text ?? "";
                string plain = Utils.RemoveVietnameseSigns(typed).ToLower();

                // Filter bằng LINQ
                var filtered = mProductSKU_dt.AsEnumerable()
                    .Where(r => r["ProductNameVN_NoSign"].ToString().ToLower()
                    .Contains(plain));

                System.Data.DataTable temp;
                if (filtered.Any())
                    temp = filtered.CopyToDataTable();
                else
                    temp = mProductSKU_dt.Clone(); // nếu không có kết quả thì trả về table rỗng

                // Gán lại DataSource
                caytrong_CB.DataSource = temp;
                caytrong_CB.DisplayMember = "ProductNameVN";  // hiển thị tên
                caytrong_CB.ValueMember = "ProductSKU";

                // Giữ lại text người đang gõ
                caytrong_CB.DroppedDown = true;
                caytrong_CB.Text = typed;
                caytrong_CB.SelectionStart = typed.Length;
                caytrong_CB.SelectionLength = 0;
            }
            catch { }
        }

        private void Employee_CBB_TextUpdate(object sender, EventArgs e)
        {
            // Restart timer mỗi khi gõ
            employeeDebounceTimer.Stop();
            employeeDebounceTimer.Start();
        }
        
        private void employeeDebounceTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                employeeDebounceTimer.Stop();

                string typed = nguoiPhuTrach_CB.Text ?? "";
                string plain = Utils.RemoveVietnameseSigns(typed).ToLower();

                // Filter bằng LINQ
                var filtered = mEmployee_dt.AsEnumerable()
                    .Where(r => r["EmployessName_NoSign"].ToString().ToLower()
                    .Contains(plain));

                System.Data.DataTable temp;
                if (filtered.Any())
                    temp = filtered.CopyToDataTable();
                else
                    temp = mEmployee_dt.Clone(); // nếu không có kết quả thì trả về table rỗng

                // Gán lại DataSource
                nguoiPhuTrach_CB.DataSource = temp;
                nguoiPhuTrach_CB.DisplayMember = "FullName";
                nguoiPhuTrach_CB.ValueMember = "EmployeeCode";

                // Giữ lại text người đang gõ
                nguoiPhuTrach_CB.DroppedDown = true;
                nguoiPhuTrach_CB.Text = typed;
                nguoiPhuTrach_CB.SelectionStart = typed.Length;
                nguoiPhuTrach_CB.SelectionLength = 0;
            }
            catch { }
        }

        private void Department_CBB_TextUpdate(object sender, EventArgs e)
        {
            // Restart timer mỗi khi gõ
            departmentDebounceTimer.Stop();
            departmentDebounceTimer.Start();
        }
        private void departmentDebounceTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                departmentDebounceTimer.Stop();

                string typed = deparment_CBB.Text ?? "";
                string plain = Utils.RemoveVietnameseSigns(typed).ToLower();

                // Filter bằng LINQ
                var filtered = mDepartment_dt.AsEnumerable()
                    .Where(r => Utils.RemoveVietnameseSigns(r["DepartmentName"].ToString()).ToLower()
                    .Contains(plain));

                System.Data.DataTable temp;
                if (filtered.Any())
                    temp = filtered.CopyToDataTable();
                else
                    temp = mDepartment_dt.Clone(); // nếu không có kết quả thì trả về table rỗng

                // Gán lại DataSource
                deparment_CBB.DataSource = temp;
                deparment_CBB.DisplayMember = "DepartmentName";  // hiển thị tên
                deparment_CBB.ValueMember = "DepartmentID";

                // Giữ lại text người đang gõ
                deparment_CBB.DroppedDown = true;
                deparment_CBB.Text = typed;
                deparment_CBB.SelectionStart = typed.Length;
                deparment_CBB.SelectionLength = 0;
            }
            catch { }
        }

        private void dataGV_CellClick(object sender, EventArgs e)
        {
            updateRightUI();            
        }

        private async void updateItem(int plantingID, string ProductionOrder, int SKU, decimal Area, int Quantity, DateTime NurseryDate, DateTime PlantingDate, DateTime HarvestDate, int Department, string Supervisor, string Note, bool IsCompleted, int cultivationTypeID, int farmId)
        {
            DataRow[] farmRows = mFarm_dt.Select($"FarmID = {farmId}");
            DataRow[] employeeRows = mEmployee_dt.Select($"EmployeeCode = '{Supervisor}'");
            DataRow[] departmentRows = mDepartment_dt.Select($"DepartmentID = {Department}");
            DataRow[] productRows = mProductSKU_dt.Select($"ProductSKU = {SKU}");
            DataRow[] cultivationTypeRows = mCultivationType_dt.Select($"CultivationTypeID = {cultivationTypeID}");

            if (employeeRows.Length <= 0 || departmentRows.Length <= 0 || productRows.Length <= 0) return;

            foreach (DataRow row in mPlantingManagement_dt.Rows)
            {
                int ID = Convert.ToInt32(row["PlantingID"]);
                if (ID.CompareTo(plantingID) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (dialogResult == DialogResult.Yes)
                    {
                        string newValue = $"ProductionOrder: {ProductionOrder}; Cây Trồng: {productRows[0]["ProductNameVN"].ToString()}; Loại Canh Tác: {cultivationTypeRows[0]["CultivationTypeName"].ToString()}; Ngày Ươm: {NurseryDate}; Ngày Trồng: {PlantingDate}; Ngày Thu: {HarvestDate}; Số Cây: {Quantity}; Diện Tích: {Area}; Tổ P.Trách: {departmentRows[0]["DepartmentName"].ToString()}; Người Giám Sát: {employeeRows[0]["FullName"].ToString()}; Ghi Chú: {Note}";
                        string oldValue = $"ProductionOrder: {row["ProductionOrder"]}; Cây Trồng: {row["PlantName"]}; Loại Canh Tác: {row["CultivationTypeName"]}; Ngày Ươm: {row["NurseryDate"]}; Ngày Trồng: {row["PlantingDate"]}; Ngày Thu: {row["HarvestDate"]}; Số Cây: {row["Quantity"]}; Diện Tích: {row["Area"]}; Tổ P.Trách: {row["DepartmentName"]}; Người Giám Sát: {row["SupervisorName"]}; Ghi Chú: {row["Note"]}";
                        try
                        {
                            bool isScussess = await SQLManager_KhoVatTu.Instance.updatePlantingManagementAsync(plantingID, ProductionOrder, SKU, Area, Quantity, NurseryDate, PlantingDate, HarvestDate, Department, Supervisor, Note, IsCompleted, cultivationTypeID, farmId);

                            if (isScussess == true)
                            {
                                row["ProductionOrder"] = ProductionOrder;
                                row["SKU"] = SKU;
                                row["Area"] = Area;
                                row["Quantity"] = Quantity;
                                row["NurseryDate"] = NurseryDate;
                                row["PlantingDate"] = PlantingDate;
                                row["HarvestDate"] = HarvestDate;
                                row["Department"] = Department;
                                row["CultivationTypeID"] = cultivationTypeID;
                                row["CultivationTypeName"] = cultivationTypeRows[0]["CultivationTypeName"].ToString();
                                row["Supervisor"] = Supervisor;
                                row["Note"] = Note;
                                row["DepartmentName"] = departmentRows[0]["DepartmentName"].ToString();
                                row["PlantName"] = productRows[0]["ProductNameVN"].ToString();
                                row["SupervisorName"] = employeeRows[0]["FullName"].ToString();
                                row["FarmName"] = farmRows[0]["FarmName"].ToString();
                                row["IsCompleted"] = IsCompleted;
                                row["search_nosign"] = Utils.RemoveVietnameseSigns( $"{ProductionOrder} {productRows[0]["ProductNameVN"].ToString()}");

                                oldValue = "Update Success" + oldValue;
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;
                            }
                            else
                            {
                                oldValue = "Update Fail" + oldValue;
                                status_lb.Text = "Thất bại.";
                                status_lb.ForeColor = Color.Red;
                            }
                        }
                        catch (Exception ex)
                        {
                            oldValue = "Update " + ex.Message + " " + oldValue;
                            status_lb.Text = "Thất bại.";
                            status_lb.ForeColor = Color.Red;
                        }
                        finally
                        {
                            _ = SQLManager_KhoVatTu.Instance.insertPlantingManagementLogAsync(NurseryDate, PlantingDate, HarvestDate, oldValue, newValue);
                        }
                    }
                    break;
                }
            }
        }

        private async void createItem(string ProductionOrder, int SKU, decimal Area, int Quantity, DateTime NurseryDate, DateTime PlantingDate, DateTime HarvestDate, int Department, string Supervisor, string Note, int cultivationTypeID, int farmId)
        {
            DataRow[] farmRows = mFarm_dt.Select($"FarmID = {farmId}");
            DataRow[] employeeRows = mEmployee_dt.Select($"EmployeeCode = '{Supervisor}'");
            DataRow[] departmentRows = mDepartment_dt.Select($"DepartmentID = {Department}");
            DataRow[] cultivationTypeRows = mCultivationType_dt.Select($"CultivationTypeID = {cultivationTypeID}");
            DataRow[] productRows = mProductSKU_dt.Select($"SKU = {SKU}");
            if (employeeRows.Length <= 0 || departmentRows.Length <= 0 || productRows.Length <= 0) return;

            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                string newValue = $"ProductionOrder: {ProductionOrder}; Cây Trồng: {productRows[0]["ProductNameVN"].ToString()}; Loại Canh Tác: {cultivationTypeRows[0]["CultivationTypeName"].ToString()}; Ngày Ươm: {NurseryDate}; Ngày Trồng: {PlantingDate}; Ngày Thu: {HarvestDate}; Số Cây: {Quantity}; Diện Tích: {Area}; Tổ P.Trách: {departmentRows[0]["DepartmentName"].ToString()}; Người Giám Sát: {employeeRows[0]["FullName"].ToString()}; Ghi Chú: {Note}";
                string oldValue = "Create: ";
                try
                {
                    int newId = await SQLManager_KhoVatTu.Instance.insertPlantingManagementAsync(ProductionOrder, SKU, Area, Quantity, NurseryDate, PlantingDate, HarvestDate, Department, Supervisor, Note, cultivationTypeID, farmId);
                    if (newId > 0)
                    {
                        DataRow drToAdd = mPlantingManagement_dt.NewRow();
                        drToAdd["PlantingID"] = newId;
                        drToAdd["ProductionOrder"] = ProductionOrder;
                        drToAdd["SKU"] = SKU;
                        drToAdd["Area"] = Area;
                        drToAdd["Quantity"] = Quantity;
                        drToAdd["NurseryDate"] = NurseryDate;
                        drToAdd["PlantingDate"] = PlantingDate;
                        drToAdd["HarvestDate"] = HarvestDate;
                        drToAdd["Department"] = Department;
                        drToAdd["CultivationTypeID"] = cultivationTypeID;
                        drToAdd["CultivationTypeName"] = cultivationTypeRows[0]["CultivationTypeName"].ToString();
                        drToAdd["Supervisor"] = Supervisor;
                        drToAdd["Note"] = Note;
                        drToAdd["DepartmentName"] = departmentRows[0]["DepartmentName"].ToString();
                        drToAdd["PlantName"] = productRows[0]["ProductNameVN"].ToString();
                        drToAdd["SupervisorName"] = employeeRows[0]["FullName"].ToString();
                        drToAdd["FarmName"] = farmRows[0]["FarmName"].ToString();
                        drToAdd["IsCompleted"] = false;
                        drToAdd["IsCompleted"] = false;

                        drToAdd["search_nosign"] = Utils.RemoveVietnameseSigns( $"{ProductionOrder} {productRows[0]["ProductNameVN"].ToString()}");

                        mPlantingManagement_dt.Rows.Add(drToAdd);
                        mPlantingManagement_dt.AcceptChanges();

                        status_lb.Text = "Thành công";
                        status_lb.ForeColor = Color.Green;

                        oldValue += "Success";
                        newBtn_Click(null, null);
                    }
                    else
                    {
                        oldValue += "Fail";
                        status_lb.Text = "Thất bại";
                        status_lb.ForeColor = Color.Red;
                    }
                }
                catch (Exception ex)
                {
                    oldValue += ex.Message;
                    Console.WriteLine("ERROR: " + ex.Message);
                    status_lb.Text = "Thất bại.";
                    status_lb.ForeColor = Color.Red;
                }
                finally
                {
                    _ = SQLManager_KhoVatTu.Instance.insertPlantingManagementLogAsync(NurseryDate, PlantingDate, HarvestDate, oldValue, newValue);
                }

            }
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            if (deparment_CBB.SelectedValue == null || caytrong_CB.SelectedValue == null || string.IsNullOrEmpty(lenhSX_tb.Text.Trim()) || string.IsNullOrEmpty(dientich_tb.Text.Trim()) || string.IsNullOrEmpty(soLuong_tb.Text.Trim())) return;

            string productionOrder = lenhSX_tb.Text;
            int productSKU = Convert.ToInt32(caytrong_CB.SelectedValue);
            decimal area = Utils.ParseDecimalSmart(dientich_tb.Text);
            int quantity = Convert.ToInt32(soLuong_tb.Text);
            DateTime nurseryDate = ngayUom_dtp.Value.Date;
            DateTime plantingDate = ngaytrong_dtp.Value.Date;
            DateTime harvestDate = ngaythu_dtp.Value.Date;
            int dep = Convert.ToInt32(deparment_CBB.SelectedValue);
            int farmId = Convert.ToInt32(farm_cbb.SelectedValue);
            int cultivationTypeID = Convert.ToInt32(cultivationType_CB.SelectedValue);
            string empCode = Convert.ToString(nguoiPhuTrach_CB.SelectedValue);
            string note = note_tb.Text;
            if (id_tb.Text.Length != 0)
                updateItem(Convert.ToInt32(id_tb.Text), productionOrder, productSKU, area, quantity, nurseryDate, plantingDate, harvestDate, dep, empCode, note, complete_cb.Checked, cultivationTypeID, farmId);
            else
                createItem(productionOrder, productSKU, area, quantity, nurseryDate, plantingDate, harvestDate, dep, empCode, note, cultivationTypeID, farmId);

        }
        private async void deleteProduct()
        {
            string id = id_tb.Text;

            foreach (DataRow row in mPlantingManagement_dt.Rows)
            {
                string plantingID = row["PlantingID"].ToString();
                if (plantingID.CompareTo(id) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Xóa Nha, Chắc Chắn Chưa!", "Thông Báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (dialogResult == DialogResult.Yes)
                    {
                        string newValue = "";
                        string oldValue = $"ProductionOrder: {row["ProductionOrder"]}; Cây Trồng: {row["PlantName"]}; Loại Canh Tác: {row["CultivationTypeName"]}; Ngày Ươm: {row["NurseryDate"]}; Ngày Trồng: {row["PlantingDate"]}; Ngày Thu: {row["HarvestDate"]}; Số Cây: {row["Quantity"]}; Diện Tích: {row["Area"]}; Tổ P.Trách: {row["DepartmentName"]}; Người Giám Sát: {row["SupervisorName"]}; Ghi Chú: {row["Note"]}";
                        DateTime NurseryDate = Convert.ToDateTime(row["NurseryDate"]);
                        DateTime PlantingDate = Convert.ToDateTime(row["PlantingDate"]);
                        DateTime HarvestDate = Convert.ToDateTime(row["HarvestDate"]);
                        try
                        {
                            bool isScussess = await SQLManager_KhoVatTu.Instance.deletetPlantingManagementAsync(Convert.ToInt32(id));

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;
                                newValue = "Delete Success";
                                mPlantingManagement_dt.Rows.Remove(row);
                                mPlantingManagement_dt.AcceptChanges();
                            }
                            else
                            {
                                newValue = "Delete Fail";
                                status_lb.Text = "Thất bại.";
                                status_lb.ForeColor = Color.Red;
                            }
                        }
                        catch (Exception ex)
                        {
                            newValue = "Delete " + ex.Message;
                            status_lb.Text = "Thất bại.";
                            status_lb.ForeColor = Color.Red;
                        }
                        finally
                        {
                            _ = SQLManager_KhoVatTu.Instance.insertPlantingManagementLogAsync(NurseryDate, PlantingDate, HarvestDate, oldValue, newValue);
                        }
                    }
                    break;
                }
            }

        }

        private void newBtn_Click(object sender, EventArgs e)
        {
            id_tb.Text = "";
            status_lb.Text = "";
            farm_cbb.Enabled = true;
            info_gb.BackColor = newCustomerBtn.BackColor;
            edit_btn.Visible = false;
            newCustomerBtn.Visible = false;
            readOnly_btn.Visible = true;
            LuuThayDoiBtn.Visible = true;
            isNewState = true;
            LuuThayDoiBtn.Text = "Lưu Mới";
            lenhSX_tb.Enabled = true;
            deparment_CBB.Enabled = true;
            caytrong_CB.Enabled = true;
            cultivationType_CB.Enabled = true;
            complete_cb.Visible = false;
            ngayUom_dtp.Enabled = true;
            ngaytrong_dtp.Enabled = true;
            ngaythu_dtp.Enabled = true;
            dientich_tb.Enabled = true;
            note_tb.Enabled = true;
            soLuong_tb.Enabled = true;
            nguoiPhuTrach_CB.Enabled = true;
            caytrong_CB.Focus();
            Caytrong_CB_SelectedIndexChanged(null, null);
        }

        private void ReadOnly_btn_Click(object sender, EventArgs e)
        {
            farm_cbb.Enabled = false;
            edit_btn.Visible = true;
            newCustomerBtn.Visible = true;
            readOnly_btn.Visible = false;
            LuuThayDoiBtn.Visible = false;
            info_gb.BackColor = Color.DarkGray;
            isNewState = false;
            lenhSX_tb.Enabled = false;
            deparment_CBB.Enabled = false;
            caytrong_CB.Enabled = false;
            cultivationType_CB.Enabled = false;
            complete_cb.Visible = true;
            ngayUom_dtp.Enabled = false;
            ngaytrong_dtp.Enabled = false;
            ngaythu_dtp.Enabled = false;
            dientich_tb.Enabled = false;
            soLuong_tb.Enabled = false;
            nguoiPhuTrach_CB.Enabled = false;
            note_tb.Enabled = false;
            complete_cb.Enabled = false;
            if (dataGV.SelectedRows.Count > 0)
                updateRightUI();
        }

        private void Edit_btn_Click(object sender, EventArgs e)
        {
            farm_cbb.Enabled = true;
            lenhSX_tb.Enabled = true;
            deparment_CBB.Enabled = true;
            caytrong_CB.Enabled = false;
            cultivationType_CB.Enabled = true;
            edit_btn.Visible = false;
            newCustomerBtn.Visible = false;
            readOnly_btn.Visible = true;
            LuuThayDoiBtn.Visible = true;
            ngayUom_dtp.Enabled = true;
            ngaytrong_dtp.Enabled = true;
            ngaythu_dtp.Enabled = true;
            dientich_tb.Enabled = true;
            note_tb.Enabled = true;
            soLuong_tb.Enabled = true;
            nguoiPhuTrach_CB.Enabled = true;
            info_gb.BackColor = edit_btn.BackColor;
            isNewState = false;
            complete_cb.Enabled = true;
            LuuThayDoiBtn.Text = "Lưu C.Sửa";
        }

        private void updateRightUI()
        {
            try
            {
                if (isNewState) return;

                if (dataGV.SelectedRows.Count > 0)
                {
                    var cells = dataGV.SelectedRows[0].Cells;

                    int? department = cells["Department"].Value == null || cells["Department"].Value == DBNull.Value ? (int?)null : Convert.ToInt32(cells["Department"].Value);
                    int? farmId = cells["FarmID"].Value == null || cells["FarmID"].Value == DBNull.Value ? (int?)null : Convert.ToInt32(cells["FarmID"].Value);

                    int sku = Convert.ToInt32(cells["SKU"].Value);
                    int cultivationTypeID = Convert.ToInt32(cells["CultivationTypeID"].Value);
                    string Supervisor = Convert.ToString(cells["Supervisor"].Value);

                    if (!deparment_CBB.Items.Cast<object>().Any(i => ((DataRowView)i)["DepartmentID"].ToString() == department.ToString()))
                    {
                        deparment_CBB.DataSource = mDepartment_dt;
                    }                    

                    if (!caytrong_CB.Items.Cast<object>().Any(i => ((DataRowView)i)["SKU"].ToString() == sku.ToString()))
                    {
                        caytrong_CB.DataSource = mProductSKU_dt;
                    }

                    if (!nguoiPhuTrach_CB.Items.Cast<object>().Any(i => ((DataRowView)i)["EmployeeCode"].ToString() == Supervisor))
                    {
                        nguoiPhuTrach_CB.DataSource = mEmployee_dt;
                    }

                    id_tb.Text = cells["PlantingID"].Value.ToString();
                    lenhSX_tb.Text = cells["ProductionOrder"].Value.ToString();
                    soLuong_tb.Text = cells["Quantity"].Value.ToString();
                    dientich_tb.Text = Convert.ToDecimal(cells["Area"].Value).ToString("0.##", CultureInfo.InvariantCulture);
                    ngayUom_dtp.Value = Convert.ToDateTime(cells["NurseryDate"].Value);
                    ngaytrong_dtp.Value = Convert.ToDateTime(cells["PlantingDate"].Value);
                    ngaythu_dtp.Value = Convert.ToDateTime(cells["HarvestDate"].Value);

                    deparment_CBB.SelectedValue = department.HasValue ? department.Value : -1;
                    farm_cbb.SelectedValue = farmId.HasValue ? farmId.Value : -1;
                    nguoiPhuTrach_CB.SelectedValue = Supervisor;
                    caytrong_CB.SelectedValue = sku;
                    cultivationType_CB.SelectedValue = cultivationTypeID;
                    note_tb.Text = cells["Note"].Value.ToString();
                    complete_cb.Checked = Convert.ToBoolean(cells["IsCompleted"].Value);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
            }
        }

        private void Tb_KeyPress_OnlyNumber_decimal(object sender, KeyPressEventArgs e)
        {
            System.Windows.Forms.TextBox tb = sender as System.Windows.Forms.TextBox;

            // Chỉ cho nhập số, dấu chấm hoặc ký tự điều khiển
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            // Nếu đã có 1 dấu chấm, không cho nhập thêm
            if (e.KeyChar == '.' && tb.Text.Contains("."))
            {
                e.Handled = true;
            }
        }

        private void Tb_KeyPress_OnlyNumber_int(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
                return;

            // Cho phép số
            if (char.IsDigit(e.KeyChar))
                return;

            // Còn lại thì chặn
            e.Handled = true;
        }

        private void dataGV_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGV.Columns[e.ColumnIndex].Name == "Area" && e.Value != null)
            {
                if (e.Value is decimal dec)
                {
                    e.Value = dec.ToString("0.00");
                    e.FormattingApplied = true;
                }
            }
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
            ShowData();
        }

        private void Search_tb_TextChanged(object sender, EventArgs e)
        {
            string keyword = Utils.RemoveVietnameseSigns(search_tb.Text.Trim().ToLower())
                     .Replace("'", "''"); // tránh lỗi cú pháp '

            DataTable dt = dataGV.DataSource as DataTable;
            if (dt == null) return;

            DataView dv = dt.DefaultView;
            dv.RowFilter = $"[search_nosign] LIKE '%{keyword}%'";
        }

        private void Caytrong_CB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isNewState)
                return;
            int productSKU = Convert.ToInt32(caytrong_CB.SelectedValue);
            int farmId = Convert.ToInt32(farm_cbb.SelectedValue);

            SuggestNewData(productSKU, farmId);
        }

        private void Farm_cbb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isNewState)
                return;
            int productSKU = Convert.ToInt32(caytrong_CB.SelectedValue);
            int farmId = Convert.ToInt32(farm_cbb.SelectedValue);

            SuggestNewData(productSKU, farmId);
        }

        private void NgayUom_dtp_ValueChanged(object sender, EventArgs e)
        {
            if (!isNewState)
                return;
            int productSKU = Convert.ToInt32(caytrong_CB.SelectedValue);
            var nextDate = ngayUom_dtp.Value;
            switch (productSKU)
            {
                case 311: //bắp non
                    nextDate = ngayUom_dtp.Value;
                    break;
                case 291: //bí đao chanh
                    nextDate = ngayUom_dtp.Value.AddDays(14);
                    break;
                case 143: //cai bẻ xanh
                    nextDate = ngayUom_dtp.Value.AddDays(14);
                    break;
                case 141: //cai ngọt
                    nextDate = ngayUom_dtp.Value.AddDays(13);
                    break;
                case 144: //cai rổ
                    nextDate = ngayUom_dtp.Value.AddDays(18);
                    break;
                case 142: //cai thìa
                    nextDate = ngayUom_dtp.Value.AddDays(14);
                    break;
                case 232: //can tây
                    nextDate = ngayUom_dtp.Value.AddDays(40);
                    break;
                case 261: //đậu đũa
                    nextDate = ngayUom_dtp.Value;
                    break;
                case 181: //hành lá
                    nextDate = ngayUom_dtp.Value.AddDays(30);
                    break;
                case 172: //hương nhu
                    nextDate = ngayUom_dtp.Value.AddDays(18);
                    break;
                case 171: //Lá quế
                    nextDate = ngayUom_dtp.Value.AddDays(15);
                    break;
                case 151: //Mồng tơi
                    nextDate = ngayUom_dtp.Value.AddDays(16);
                    break;
                case 271: //Mướp hương
                    nextDate = ngayUom_dtp.Value.AddDays(14);
                    break;
                case 131: //ngò gai
                    nextDate = ngayUom_dtp.Value.AddDays(60);
                    break;
                case 161: //rau muống
                    nextDate = ngayUom_dtp.Value;
                    break;
                case 221: //tía tô
                    nextDate = ngayUom_dtp.Value.AddDays(24);
                    break;
                case 111: //Ớt Hiểm
                    nextDate = ngayUom_dtp.Value.AddDays(27);
                    break;
                case 101: //Cà Pháo
                    nextDate = ngayUom_dtp.Value.AddDays(25);
                    break;
                case 121: //Đu Đủ
                    nextDate = ngayUom_dtp.Value.AddDays(40);
                    break;
            }
            ngaytrong_dtp.Value = nextDate;
        }

        private void Ngaytrong_dtp_ValueChanged(object sender, EventArgs e)
        {
            if (!isNewState)
                return;
            int productSKU = Convert.ToInt32(caytrong_CB.SelectedValue);
            var nextDate = ngaytrong_dtp.Value;
            switch (productSKU)
            {
                case 311: //bắp non
                    nextDate = ngaytrong_dtp.Value.AddDays(45);
                    break;
                case 291: //bí đao chanh
                    nextDate = ngaytrong_dtp.Value.AddDays(50);
                    break;
                case 143: //cai bẻ xanh
                    nextDate = ngaytrong_dtp.Value.AddDays(19);
                    break;
                case 141: //cai ngọt
                    nextDate = ngaytrong_dtp.Value.AddDays(19);
                    break;
                case 144: //cai rổ
                    nextDate = ngaytrong_dtp.Value.AddDays(28);
                    break;
                case 142: //cai thìa
                    nextDate = ngaytrong_dtp.Value.AddDays(21);
                    break;
                case 232: //can tây
                    nextDate = ngaytrong_dtp.Value.AddDays(30);
                    break;
                case 261: //đậu đũa
                    nextDate = ngaytrong_dtp.Value.AddDays(45);
                    break;
                case 181: //hành lá
                    nextDate = ngaytrong_dtp.Value.AddDays(44);
                    break;
                case 172: //hương nhu
                    nextDate = ngaytrong_dtp.Value.AddDays(27);
                    break;
                case 171: //Lá quế
                    nextDate = ngaytrong_dtp.Value.AddDays(27);
                    break;
                case 151: //Mồng tơi
                    nextDate = ngaytrong_dtp.Value.AddDays(19);
                    break;
                case 271: //Mướp hương
                    nextDate = ngaytrong_dtp.Value.AddDays(35);
                    break;
                case 131: //ngò gai
                    nextDate = ngaytrong_dtp.Value.AddDays(60);
                    break;
                case 161: //rau muống
                    nextDate = ngaytrong_dtp.Value.AddDays(23);
                    break;
                case 221: //tía tô
                    nextDate = ngaytrong_dtp.Value.AddDays(25);
                    break;
                case 111: //Ớt Hiểm
                    nextDate = ngaytrong_dtp.Value.AddDays(52);
                    break;
                case 101: //Cà Pháo
                    nextDate = ngaytrong_dtp.Value.AddDays(50);
                    break;
                case 121: //Đu Đủ
                    nextDate = ngaytrong_dtp.Value.AddDays(212);
                    break;
            }
            
            DayOfWeek dow = nextDate.DayOfWeek;
            int SL = 0;
            decimal DienTich = 0;
            switch (productSKU)
            {
               
                case 141: //cai ngọt
                    SL = dow == DayOfWeek.Monday ? 870 : 435;
                    DienTich = dow == DayOfWeek.Monday ? 34.8m : 17.4m;
                    break;
                case 144: //cai rổ
                    SL = dow == DayOfWeek.Monday ? 1740 : 2175;
                    DienTich = dow == DayOfWeek.Monday ? 69.6m : 87.0m;
                    break;
                case 142: //cai thìa
                    SL = dow == DayOfWeek.Monday ? 1740 : 2175;
                    DienTich = dow == DayOfWeek.Monday ? 69.6m : 87.0m;
                    break;
                case 232: //can tây
                    SL = dow == DayOfWeek.Monday ? 870 : 870;
                    DienTich = dow == DayOfWeek.Monday ? 34.8m : 34.8m;
                    break;
                case 143: //cai be xanh
                    SL = dow == DayOfWeek.Monday ? 870 : 435;
                    DienTich = dow == DayOfWeek.Monday ? 34.8m : 17.4m;
                    break;
            }


            ngaythu_dtp.Value = nextDate;
            if(SL > 0)
                soLuong_tb.Text = SL.ToString();
            if(DienTich > 0)
                dientich_tb.Text = DienTich.ToString();
        }

        int GetMaxProductionOrderIndex(DataTable dt, int sku, int farmId, DateTime ngayUom)
        {
            if (dt == null || dt.Rows.Count == 0)
                return 0;

            var numbers = dt.AsEnumerable()
                .Where(r =>
                    !r.IsNull("SKU") &&
                    r.Field<int>("SKU") == sku &&
                    !r.IsNull("FarmID") &&
                    r.Field<int>("FarmID") == farmId &&
                    !r.IsNull("ProductionOrder") &&
                    !r.IsNull("NurseryDate") &&
                    r.Field<DateTime>("NurseryDate").Year == ngayUom.Year)
                .Select(r =>
                {
                    string po = r.Field<string>("ProductionOrder")?.Trim();
                    if (string.IsNullOrEmpty(po) || po.Length < 3)
                        return -1;

                    string last3 = po.Substring(po.Length - 3);
                    return int.TryParse(last3, out int n) ? n : -1;
                })
                .Where(n => n >= 0);

            return numbers.Any() ? numbers.Max() : 0;
        }

        void SuggestNewData(int SKU, int farmId)
        {
            DataRow maxRow = mPlantingManagement_dt.AsEnumerable()
                                                .Where(r => r.Field<int>("SKU") == SKU && r.Field<int>("FarmID") == farmId)
                                                .Select(r => new
                                                {
                                                    Row = r,
                                                    OrderNum = int.TryParse(
                                                        r.Field<string>("ProductionOrder")
                                                            .Substring(r.Field<string>("ProductionOrder").Length - 3),
                                                        out int n) ? n : 0
                                                })
                                                .OrderByDescending(x => x.OrderNum)
                                                .Select(x => x.Row)
                                                .FirstOrDefault();
            


            var nurseryDate = DateTime.Now;
            if (maxRow != null)
                nurseryDate = Convert.ToDateTime(maxRow["NurseryDate"]);

            var nextDay = nurseryDate;
            int departmentID = -1;
            string Supervisor = "";            
            switch (SKU)
            {
                case 311: //bắp non
                    nextDay = nurseryDate.AddDays(7);
                    break;
                case 291: //bí đao chanh
                    nextDay = nurseryDate.AddDays(14);
                    break;
                case 143: //cai bẻ xanh
                    nextDay = GetNextMondayOrThursday(nurseryDate, 14, 19);
                    break;
                case 141: //cai ngọt
                    nextDay = GetNextMondayOrThursday(nurseryDate, 13, 19);
                    departmentID = 27; // nhà ươm thủy canh
                    Supervisor = "VR0359";
                    break;
                case 144: //cai rổ
                    nextDay = GetNextMondayOrThursday(nurseryDate, 18, 28);
                    departmentID = 27; // nhà ươm thủy canh
                    Supervisor = "VR0359";
                    break;
                case 142: //cai thìa
                    nextDay = GetNextMondayOrThursday(nurseryDate, 14, 21);
                    departmentID = 27; // nhà ươm thủy canh
                    Supervisor = "VR0359";
                    break;
                case 232: //can tây
                    nextDay = GetNextMondayOrThursday(nurseryDate, 40, 30);
                    departmentID = 27; // nhà ươm thủy canh
                    Supervisor = "VR0359";
                    break;
                case 261: //đậu đũa
                    nextDay = nurseryDate.AddDays(7);
                    break;
                case 181: //hành lá
                    nextDay = GetNextTuedayOrFriday(nurseryDate, 30, 44);
                    break;
                case 172: //hương nhu
                    nextDay = nurseryDate.AddDays(7);
                    break;
                case 171: //Lá quế
                    nextDay = nurseryDate.AddDays(7);
                    break;
                case 151: //Mồng tơi
                    nextDay = GetNextMondayOrThursday(nurseryDate, 16, 19);
                    break;
                case 271: //Mướp hương
                    nextDay = nurseryDate.AddDays(14);
                    break;
                case 131: //ngò gai
                    nextDay = nurseryDate.AddDays(7);
                    break;
                case 161: //rau muống
                    nextDay = GetNextMondayOrThursday(nurseryDate, 0, 23);
                    break;
                case 221: //tía tô
                    nextDay = nurseryDate.AddDays(7);
                    break;
            }

            int index_lenhSX = GetMaxProductionOrderIndex(mPlantingManagement_dt, SKU, farmId, nextDay) + 1;
            
            switch (SKU)
            {
                case 311: //bắp non
                    departmentID = (index_lenhSX % 2) == 0 ? 25 : 24; // tổ B, Tô A
                    Supervisor = (index_lenhSX % 2) == 0 ? "VR0384" : "VR0063"; // tổ B, Tô A
                    break;
                case 291: //bí đao chanh
                    departmentID = (index_lenhSX % 2) == 0 ? 25 : 24; // tổ B, Tô A
                    Supervisor = (index_lenhSX % 2) == 0 ? "VR0384" : "VR0063"; // tổ B, Tô A
                    break;
                case 143: //cai bẻ xanh
                    departmentID = (index_lenhSX % 2) == 0 ? 25 : 24; // tổ B, Tô A
                    Supervisor = (index_lenhSX % 2) == 0 ? "VR0384" : "VR0063"; // tổ B, Tô A
                    break;
                case 141: //cai ngọt
                    departmentID = 27; // nhà ươm thủy canh
                    Supervisor = "VR0359";
                    break;
                case 144: //cai rổ
                    departmentID = 27; // nhà ươm thủy canh
                    Supervisor = "VR0359";
                    break;
                case 142: //cai thìa
                    departmentID = 27; // nhà ươm thủy canh
                    Supervisor = "VR0359";
                    break;
                case 232: //can tây
                    departmentID = 27; // nhà ươm thủy canh
                    Supervisor = "VR0359";
                    break;
                case 261: //đậu đũa
                    departmentID = (index_lenhSX % 2) == 0 ? 25 : 24; // tổ B, Tô A
                    Supervisor = (index_lenhSX % 2) == 0 ? "VR0384" : "VR0063"; // tổ B, Tô A
                    break;
                case 181: //hành lá
                    departmentID = (index_lenhSX % 2) == 0 ? 25 : 24; // tổ B, Tô A
                    Supervisor = (index_lenhSX % 2) == 0 ? "VR0384" : "VR0063"; // tổ B, Tô A
                    break;
                case 172: //hương nhu
                    departmentID = (index_lenhSX % 2) == 0 ? 25 : 24; // tổ B, Tô A
                    Supervisor = (index_lenhSX % 2) == 0 ? "VR0384" : "VR0063"; // tổ B, Tô A
                    break;
                case 171: //Lá quế
                    departmentID = (index_lenhSX % 2) == 0 ? 25 : 24; // tổ B, Tô A
                    Supervisor = (index_lenhSX % 2) == 0 ? "VR0384" : "VR0063"; // tổ B, Tô A
                    break;
                case 151: //Mồng tơi
                    departmentID = (index_lenhSX % 2) == 0 ? 25 : 24; // tổ B, Tô A
                    Supervisor = (index_lenhSX % 2) == 0 ? "VR0384" : "VR0063"; // tổ B, Tô A
                    break;
                case 271: //Mướp hương
                    departmentID = (index_lenhSX % 2) == 0 ? 25 : 24; // tổ B, Tô A
                    Supervisor = (index_lenhSX % 2) == 0 ? "VR0384" : "VR0063"; // tổ B, Tô A
                    break;
                case 131: //ngò gai
                    departmentID = (index_lenhSX % 2) == 0 ? 25 : 24; // tổ B, Tô A
                    Supervisor = (index_lenhSX % 2) == 0 ? "VR0384" : "VR0063"; // tổ B, Tô A
                    break;
                case 161: //rau muống
                    departmentID = (index_lenhSX % 2) == 0 ? 25 : 24; // tổ B, Tô A
                    Supervisor = (index_lenhSX % 2) == 0 ? "VR0384" : "VR0063"; // tổ B, Tô A
                    break;
                case 221: //tía tô
                    departmentID = (index_lenhSX % 2) == 0 ? 25 : 24; // tổ B, Tô A
                    Supervisor = (index_lenhSX % 2) == 0 ? "VR0384" : "VR0063"; // tổ B, Tô A
                    break;
            }

            lenhSX_tb.Text = $"{SKU.ToString()}{nextDay.ToString("yy")}{(index_lenhSX).ToString("D3")}";
            ngayUom_dtp.Value = nextDay;
            deparment_CBB.SelectedValue = departmentID;
            nguoiPhuTrach_CB.SelectedValue = Supervisor;
        }

        DateTime GetNextMondayOrThursday(DateTime date, int ngayTrongCachNgayThuHoach, int ngayThuCachNgayTrong)
        {
            DateTime dateTemp = date.AddDays(ngayTrongCachNgayThuHoach + ngayThuCachNgayTrong + 1); // bắt đầu từ ngày kế tiếp
            int count = 1;
            while (true)
            {
                if (dateTemp.DayOfWeek == DayOfWeek.Monday ||
                    dateTemp.DayOfWeek == DayOfWeek.Thursday)
                {
                    return date.AddDays(count);
                }
                dateTemp = dateTemp.AddDays(1);
                count++;
            }
        }

        DateTime GetNextTuedayOrFriday(DateTime date, int ngayTrongCachNgayThuHoach, int ngayThuCachNgayTrong)
        {
            DateTime dateTemp = date.AddDays(ngayTrongCachNgayThuHoach + ngayThuCachNgayTrong + 1); // bắt đầu từ ngày kế tiếp
            int count = 1;
            while (true)
            {
                if (dateTemp.DayOfWeek == DayOfWeek.Tuesday ||
                    dateTemp.DayOfWeek == DayOfWeek.Friday)
                {
                    return date.AddDays(count);
                }
                dateTemp = dateTemp.AddDays(1);
                count++;
            }
        }

        private void Loc_CheckedChanged(object sender, EventArgs e)
        {
            MonthGV_SelectionChanged(null, null);
        }
        private void MonthGV_SelectionChanged(object sender, EventArgs e)
        {

            if (monthGV.SelectedRows.Count == 0 || toPhuTrach_GV.SelectedRows.Count == 0 || weekInYear_GV.SelectedRows.Count == 0)
                return;
            var monthCell = monthGV.SelectedRows[0].Cells;
            var departmentCell = toPhuTrach_GV.SelectedRows[0].Cells;
            var weekInYearCell = weekInYear_GV.SelectedRows[0].Cells;
            if (monthCell == null || departmentCell == null || weekInYearCell == null) 
                return;

            int month = Convert.ToInt32(monthCell["Month"].Value);
            
            DateTime fromDate = new DateTime(monthYear_dtp.Value.Year, month, 1);
            DateTime toDate = fromDate.AddMonths(1);       

            List<string> orConditions = new List<string>();
            List<string> andConditions = new List<string>();

            if (locTheoNgayUom_CB.Checked)
            {
                orConditions.Add($"(NurseryDate >= #{fromDate:MM/dd/yyyy}# AND NurseryDate < #{toDate:MM/dd/yyyy}#)");
            }

            if (locTheoNgayTrong_CB.Checked)
            {
                orConditions.Add($"(PlantingDate >= #{fromDate:MM/dd/yyyy}# AND PlantingDate < #{toDate:MM/dd/yyyy}#)");
            }

            if (locTheoNgayThu_CB.Checked)
            {
                orConditions.Add($"(HarvestDate >= #{fromDate:MM/dd/yyyy}# AND HarvestDate < #{toDate:MM/dd/yyyy}#)");
            }

            if (locTheoTo_CB.Checked)
            {
                int department = Convert.ToInt32(departmentCell["Department"].Value);
                andConditions.Add($"Department = {department}");
            }

            if (locTheoTuan_CB.Checked)
            {
                DateTime startDay = Convert.ToDateTime(weekInYearCell["StartDate"].Value);
                DateTime endDay = Convert.ToDateTime(weekInYearCell["EndDate"].Value);

                andConditions.Add($"PlantingDate >= '{startDay:yyyy-MM-dd}' AND PlantingDate <= '{endDay:yyyy-MM-dd}'");
            }

            string filter = "";
            string filterLog = "";

            if (orConditions.Count > 0)
            {
                filter = "(" + string.Join(" OR ", orConditions) + ")";
                filterLog = filter;
            }

            if (andConditions.Count > 0)
            {
                if (filter.Length > 0)
                    filter += " AND ";

                filter += string.Join(" AND ", andConditions);
            }

            DataView dv = mPlantingManagement_dt.DefaultView;
            dv.RowFilter = filter;

            mLog_dv.RowFilter = filterLog;

        }

        private void DataGV_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return; // tránh click header

            DataRowView drv = dataGV.Rows[e.RowIndex].DataBoundItem as DataRowView;
            if (drv == null) return;

            DataRow row = drv.Row;

            int plantingID = row.Field<int>("PlantingID");
            int sku = row.Field<int>("SKU");
            int ctID = row.Field<int>("CultivationTypeID");
            string skuName = row["PlantName"].ToString();
            string ctName = row["CultivationTypeName"].ToString();

            KhoVatTu_CultivationProcess frm = new KhoVatTu_CultivationProcess(row);
            frm.ShowData();
            frm.ShowDialog(); // hoặc Show()
        }

        private void InPhieuSX_btn_Click(object sender, EventArgs e)
        {
            InPhieuSX_trong(false);
        }

        private void XemPhieuSX_btn_Click(object sender, EventArgs e)
        {
            InPhieuSX_trong(true);
        }

        private async void ExcelPhieuSX_btn_Click(object sender, EventArgs e)
        {
            if (monthGV.SelectedRows.Count == 0 || toPhuTrach_GV.SelectedRows.Count == 0)
                return;

            var monthCell = monthGV.SelectedRows[0].Cells;
            var depCell = toPhuTrach_GV.SelectedRows[0].Cells;
            if (monthCell == null || depCell == null) return;

            int month = Convert.ToInt32(monthCell["Month"].Value);
            int year = monthYear_dtp.Value.Year;

            int depID = Convert.ToInt32(depCell["Department"].Value);
            string depName = depCell["DepartmentName"].Value.ToString();

            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Message = "Đang xử lý ...";
            loadingOverlay.Show();
            await Task.Delay(100);

            try
            {
                using (var wb = new XLWorkbook())
                {
                    DateTime start = new DateTime(year, month, 1);
                    DateTime end = start.AddMonths(1);
                    DataView dv = new DataView(mPlantingManagement_dt);
                    dv.RowFilter = $"PlantingDate >= #{start:MM/dd/yyyy}# AND PlantingDate < #{end:MM/dd/yyyy}# AND Department = ({depID})";
                    dv.Sort = "PlantingDate ASC";

                    DataTable filterResult_dt = dv.ToTable();

                    var ws = wb.Worksheets.Add("Kế Hoạch Trồng");

                    int rowExcel_dock = 1;
                    int rowExcel = 1;
                    int colExcel = 1;

                    int col1Width = 20;
                    int col2Width = 15;

                    foreach (DataRow row in filterResult_dt.Rows)
                    {
                        DateTime plantingDate = Convert.ToDateTime(row["PlantingDate"]);
                        int departmentID = Convert.ToInt32(row["Department"]);

                        ws.Cell(rowExcel, colExcel).Value = "Lệnh Sản Xuất";
                        ws.Cell(rowExcel, colExcel + 1).Value = row["ProductionOrder"].ToString();
                        rowExcel++;

                        ws.Cell(rowExcel, colExcel).Value = "Tên Cây Trồng";
                        ws.Cell(rowExcel, colExcel + 1).Value = row["PlantName"].ToString();
                        rowExcel++;

                        if (departmentID != 27)
                        {
                            ws.Cell(rowExcel, colExcel).Value = "Mã Cây Trồng";
                            ws.Cell(rowExcel, colExcel + 1).Value = row["SKU"].ToString();
                            rowExcel++;
                        }

                        ws.Cell(rowExcel, colExcel).Value = "Diện Tích";
                        ws.Cell(rowExcel, colExcel + 1).Value =Convert.ToDecimal(row["Area"]).ToString("0.##");
                        rowExcel++;

                        ws.Cell(rowExcel, colExcel).Value = "Ngày Trồng";
                        ws.Cell(rowExcel, colExcel + 1).Value = plantingDate.ToString("dd/MM/yyyy");
                        rowExcel++;

                        ws.Cell(rowExcel, colExcel).Value = "Ngày Thu Hoạch";
                        ws.Cell(rowExcel, colExcel + 1).Value =
                            Convert.ToDateTime(row["HarvestDate"]).ToString("dd/MM/yyyy");
                        rowExcel++;

                        if (departmentID != 27)
                        {
                            ws.Cell(rowExcel, colExcel).Value = "S.Lượng Dự Kiến";
                            ws.Range(rowExcel_dock, colExcel, rowExcel_dock + 6, colExcel).Style.Font.Bold = true;
                        }
                        else
                        {
                            ws.Range(rowExcel, colExcel, rowExcel, colExcel + 1).Merge();
                            ws.Cell(rowExcel, colExcel).Value = "Vị Trí";
                            
                            ws.Range(rowExcel_dock, colExcel, rowExcel_dock + 5, colExcel).Style.Font.Bold = true;

                            ws.Range(rowExcel + 1, colExcel, rowExcel + 1, colExcel + 1).Merge();
                            ws.Cell(rowExcel + 1, colExcel).Value = row["PlantLocation"].ToString();
                            ws.Cell(rowExcel + 1, colExcel).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Row(rowExcel + 1).Height = 30;
                        }

                        
                        ws.Range(rowExcel_dock, colExcel, rowExcel_dock + 6, colExcel + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Range(rowExcel_dock, colExcel, rowExcel_dock + 6, colExcel + 1).Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                        colExcel += 3;
                        if (colExcel > 8)
                        {
                            colExcel = 1;
                            rowExcel_dock += 8;
                        }

                        rowExcel = rowExcel_dock;
                    }

                    ws.Column(1).Width = col1Width;
                    ws.Column(2).Width = col2Width;
                    ws.Column(3).Width = 1.5;
                    ws.Column(4).Width = col1Width;
                    ws.Column(5).Width = col2Width;
                    ws.Column(6).Width = 1.5;
                    ws.Column(7).Width = col1Width;
                    ws.Column(8).Width = col2Width;

                    ws.Style.Font.FontName = "Times New Roman";
                    ws.Style.Font.FontSize = 11;

                    using (SaveFileDialog sfd = new SaveFileDialog())
                    {
                        sfd.Filter = "Excel Workbook|*.xlsx";
                        sfd.FileName = $"PhieuTrong_Thang{month.ToString("D2")}{year.ToString()}_{Utils.RemoveVietnameseSigns(depName).Replace(" ", "")}";
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

        private async void InPhieuSX_trong(bool isPrintPreview)
        {
            if (monthGV.SelectedRows.Count == 0 || toPhuTrach_GV.SelectedRows.Count == 0)
                return;

            List<int> deps = new List<int>();
            foreach (DataGridViewRow row in toPhuTrach_GV.SelectedRows)
            {
                int id = Convert.ToInt32(row.Cells["Department"].Value);
                deps.Add(id);
            }

            var monthCell = monthGV.SelectedRows[0].Cells;
            if (monthCell == null || deps.Count <= 0) return;

            int month = Convert.ToInt32(monthCell["Month"].Value);
            int year = monthYear_dtp.Value.Year;

            KhoVatTu_PhieuSX_Trong_Printer printer = new KhoVatTu_PhieuSX_Trong_Printer(deps, mPlantingManagement_dt, month, year);

            if (isPrintPreview)
                printer.PrintPreview(this);
            else
                printer.PrintDirect();
        }

        private void InPhieuSX_uom_btn_Click(object sender, EventArgs e)
        {
            _ = InPhieuSX_uom(false);
        }

        private void XemPhieuSX_uom_btn_Click(object sender, EventArgs e)
        {
            _ = InPhieuSX_uom(true);
        }

        private async void ExcelPhieuSX_uom_btn_Click(object sender, EventArgs e)
        {
            if (monthGV.SelectedRows.Count == 0 || toPhuTrach_GV.SelectedRows.Count == 0)
                return;

            var monthCell = monthGV.SelectedRows[0].Cells;
            var depCell = toPhuTrach_GV.SelectedRows[0].Cells;
            if (monthCell == null || depCell == null) return;

            int month = Convert.ToInt32(monthCell["Month"].Value);
            int year = monthYear_dtp.Value.Year;

            int depID = Convert.ToInt32(depCell["Department"].Value);
            string depName = depCell["DepartmentName"].Value.ToString();

            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Message = "Đang xử lý ...";
            loadingOverlay.Show();
            await Task.Delay(100);

            try
            {
                using (var wb = new XLWorkbook())
                {
                    DateTime start = new DateTime(year, month, 1);
                    DateTime end = start.AddMonths(1);
                    DataView dv = new DataView(mPlantingManagement_dt);
                    dv.RowFilter = $"PlantingDate >= #{start:MM/dd/yyyy}# AND PlantingDate < #{end:MM/dd/yyyy}# AND Department = ({depID})";
                    dv.Sort = "PlantingDate ASC";

                    DataTable filterResult_dt = dv.ToTable();

                    var ws = wb.Worksheets.Add("Uom");

                    int col1Width = 20;
                    int col2Width = 15;
                    int distanceWidth = 2;
                    int numRow = 8;

                    int excelRow = 1;
                    int index = 0;

                    DataTable plantTrayDensity = await SQLStore_KhoVatTu.Instance.GetPlantTrayDensityAsync();

                    foreach (DataRow row in filterResult_dt.Rows)
                    {
                        int sku = Convert.ToInt32(row["SKU"]);
                        DateTime plantingDate = Convert.ToDateTime(row["PlantingDate"]);
                        DataRow plantTrayDensityRow = plantTrayDensity.Select($"SKU = '{sku}'").FirstOrDefault();

                        decimal trayPerSquareMeter = 0;
                        if (plantTrayDensityRow != null)
                            trayPerSquareMeter = Convert.ToDecimal(plantTrayDensityRow["TrayPerSquareMeter"]);

                        decimal soKhay = Convert.ToDecimal(row["Area"]) * trayPerSquareMeter;

                        int blockCol = (index % 3) * (1 + distanceWidth) + 1;
                        int blockRow = excelRow + (index / 3) * (numRow + 1);

                        int r = blockRow;

                        ws.Range(r, blockCol, r, blockCol + 1).Merge().Value = "Tên Cây Trồng";
                        ws.Range(r, blockCol, r, blockCol + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Range(r, blockCol, r, blockCol + 1).Style.Font.Bold = true;
                        r++;

                        ws.Range(r, blockCol, r, blockCol + 1).Merge().Value = row["PlantName"].ToString();
                        ws.Range(r, blockCol, r, blockCol + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Range(r, blockCol, r, blockCol + 1).Style.Font.Bold = true;
                        ws.Range(r, blockCol, r, blockCol + 1).Style.Font.Underline = XLFontUnderlineValues.Single;
                        r++;

                        ws.Cell(r, blockCol).Value = "Lệnh Sản Xuất";
                        ws.Cell(r, blockCol + 1).Value = row["ProductionOrder"].ToString();
                        r++;

                        ws.Cell(r, blockCol).Value = "Số Khay";
                        ws.Cell(r, blockCol + 1).Value = soKhay.ToString("#,##0");
                        r++;

                        ws.Cell(r, blockCol).Value = "Ngày Ươm";
                        ws.Cell(r, blockCol + 1).Value = Convert.ToDateTime(row["NurseryDate"]).ToString("dd/MM/yyyy");
                        r++;

                        ws.Cell(r, blockCol).Value = "Ngày Trồng";
                        ws.Cell(r, blockCol + 1).Value = plantingDate.ToString("dd/MM/yyyy");
                        r++;

                        ws.Range(r, blockCol, r + 1, blockCol + 1).Merge().Value = $"Ghi Chú: {row["Note"]}";
                        ws.Range(r, blockCol, r + 1, blockCol + 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;

                        var borderRange = ws.Range(blockRow, blockCol, blockRow + numRow - 1, blockCol + 1);
                        borderRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        borderRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                        index++;
                    }

                    ws.Column(1).Width = col1Width;
                    ws.Column(2).Width = col2Width;
                    ws.Column(3).Width = 1.5;
                    ws.Column(4).Width = col1Width;
                    ws.Column(5).Width = col2Width;
                    ws.Column(6).Width = 1.5;
                    ws.Column(7).Width = col1Width;
                    ws.Column(8).Width = col2Width;
                    ws.Style.Font.FontName = "Times New Roman";
                    ws.Style.Font.FontSize = 11;

                    using (SaveFileDialog sfd = new SaveFileDialog())
                    {
                        sfd.Filter = "Excel Workbook|*.xlsx";
                        sfd.FileName = $"PhieuUom_Thang{month.ToString("D2")}{year.ToString()}_{Utils.RemoveVietnameseSigns(depName).Replace(" ", "")}";
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

        private async Task InPhieuSX_uom(bool isPrintPreview)
        {
            if (monthGV.SelectedRows.Count == 0 || toPhuTrach_GV.SelectedRows.Count == 0)
                return;

            List<int> deps = new List<int>();
            foreach (DataGridViewRow row in toPhuTrach_GV.SelectedRows)
            {
                int id = Convert.ToInt32(row.Cells["Department"].Value);
                deps.Add(id);
            }

            var monthCell = monthGV.SelectedRows[0].Cells;
            if (monthCell == null || deps.Count <= 0) return;

            int month = Convert.ToInt32(monthCell["Month"].Value);
            int year = monthYear_dtp.Value.Year;

            DataTable plantTrayDensity = await SQLStore_KhoVatTu.Instance.GetPlantTrayDensityAsync();

            KhoVatTu_PhieuSX_Uom_Printer printer = new KhoVatTu_PhieuSX_Uom_Printer(deps, plantTrayDensity, mPlantingManagement_dt, month, year);

            if (isPrintPreview)
                printer.PrintPreview(this);
            else
                printer.PrintDirect();
        }

        private void InKeHoachSX_btn_Click(object sender, EventArgs e)
        {
            _ = InKeHoachSX(false);
        }

        private void XemKeHoachSX_btn_Click(object sender, EventArgs e)
        {
            _ = InKeHoachSX(true);
        }


        private async void ExcelKeHoachSX_btn_Click(object sender, EventArgs e)
        {
            if (monthGV.SelectedRows.Count == 0 || toPhuTrach_GV.SelectedRows.Count == 0)
                return;

            var monthCell = monthGV.SelectedRows[0].Cells;
            var depCell = toPhuTrach_GV.SelectedRows[0].Cells;
            if (monthCell == null || depCell == null) return;

            int month = Convert.ToInt32(monthCell["Month"].Value);
            int year = monthYear_dtp.Value.Year;

            int depID = Convert.ToInt32(depCell["Department"].Value);
            string depName = depCell["DepartmentName"].Value.ToString();

            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Message = "Đang xử lý ...";
            loadingOverlay.Show();
            await Task.Delay(100);

            try
            {
                using (var wb = new XLWorkbook())
                {
                    DateTime start = new DateTime(year, month, 1);
                    DateTime end = start.AddMonths(1);
                    DataView dv = new DataView(mPlantingManagement_dt);
                    dv.RowFilter = $"PlantingDate >= #{start:MM/dd/yyyy}# AND PlantingDate < #{end:MM/dd/yyyy}# AND Department = ({depID})";
                    dv.Sort = "PlantingDate ASC";

                    DataTable filterResult_dt = dv.ToTable();

                    var ws = wb.Worksheets.Add("PhieuSXTrong");
                    ws.Style.Font.FontName = "Times New Roman";
                    ws.Style.Font.FontSize = 12;

                    int rowExcel = 1;

                    ws.Range(rowExcel, 1, rowExcel, 11).Merge();
                    ws.Cell(rowExcel, 1).Value = $"KẾ HOẠCH SẢN XUẤT THÁNG {month.ToString("D2")}/{year.ToString()} - {depName}";
                    ws.Cell(rowExcel, 1).Style.Font.Bold = true;
                    ws.Cell(rowExcel, 1).Style.Font.FontSize = 20;
                    rowExcel++;
                    // Header
                    ws.Cell(rowExcel, 1).Value = "STT";
                    ws.Cell(rowExcel, 2).Value = "Lệnh\nSản Xuất";
                    ws.Cell(rowExcel, 3).Value = "Tên\nCây Trồng";
                    ws.Cell(rowExcel, 4).Value = "Số Lượng\nCây";
                    ws.Cell(rowExcel, 5).Value = "Ngày\nƯơm Hạt";
                    ws.Cell(rowExcel, 6).Value = "Ngày Trồng";
                    ws.Cell(rowExcel, 7).Value = "Diện\nTích\n(m2)";
                    ws.Cell(rowExcel, 8).Value = "Ngày\nThu Hoạch";
                    ws.Cell(rowExcel, 9).Value = "Thứ\nTrồng\nCây";
                    ws.Cell(rowExcel, 10).Value = "Thứ\nThu\nHoạch";
                    ws.Cell(rowExcel, 11).Value = "Vị Trí Trồng";

                    ws.Range(rowExcel, 1, rowExcel, 11).Style.Font.Bold = true;
                    ws.Range(rowExcel, 1, rowExcel, 11).Style.Alignment.WrapText = true;
                    ws.Range(rowExcel-1, 1, rowExcel, 11).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Range(rowExcel-1, 1, rowExcel, 11).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Row(rowExcel).Height = 47.5;

                    rowExcel++;
                    int countSTT = 1;
                    foreach (DataRow row in filterResult_dt.Rows)
                    {
                        ws.Cell(rowExcel, 1).Value = countSTT;
                        ws.Cell(rowExcel, 2).Value = row["ProductionOrder"].ToString();
                        ws.Cell(rowExcel, 3).Value = row["PlantName"].ToString();
                        ws.Cell(rowExcel, 4).Value = Convert.ToInt32(row["Quantity"]);
                        ws.Cell(rowExcel, 5).Value = Convert.ToDateTime(row["NurseryDate"]);
                        ws.Cell(rowExcel, 6).Value = Convert.ToDateTime(row["PlantingDate"]);
                        ws.Cell(rowExcel, 7).Value = Convert.ToDecimal(row["Area"]);                        
                        ws.Cell(rowExcel, 8).Value = Convert.ToDateTime(row["HarvestDate"]);
                        ws.Cell(rowExcel, 9).Value = Utils.GetThu_Viet(Convert.ToDateTime(row["PlantingDate"]));
                        ws.Cell(rowExcel, 10).Value = Utils.GetThu_Viet(Convert.ToDateTime(row["HarvestDate"]));


                        ws.Cell(rowExcel, 4).Style.NumberFormat.Format = "#,##0 \"cây\"";
                        rowExcel++;
                        countSTT++;
                    }

                    // format ngày
                    ws.Column(5).Style.DateFormat.Format = "dd/MM/yyyy";
                    ws.Column(6).Style.DateFormat.Format = "dd/MM/yyyy";
                    ws.Column(8).Style.DateFormat.Format = "dd/MM/yyyy";

                    // border
                    ws.Range(1, 1, rowExcel - 1, 11).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    ws.Range(1, 1, rowExcel - 1, 11).Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                    ws.Columns().AdjustToContents();

                    using (SaveFileDialog sfd = new SaveFileDialog())
                    {
                        sfd.Filter = "Excel Workbook|*.xlsx";
                        sfd.FileName = $"KeHoachSanXuat_Thang{month.ToString("D2")}{year.ToString()}_{Utils.RemoveVietnameseSigns(depName).Replace(" ", "")}";
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

        private async Task InKeHoachSX(bool isPrintPreview)
        {
            if (monthGV.SelectedRows.Count == 0 || toPhuTrach_GV.SelectedRows.Count == 0)
                return;

            var monthCell = monthGV.SelectedRows[0].Cells;
            var depCell = toPhuTrach_GV.SelectedRows[0].Cells;
            if (monthCell == null || depCell == null) return;

            int month = Convert.ToInt32(monthCell["Month"].Value);
            int year = monthYear_dtp.Value.Year;

            int depID = Convert.ToInt32(depCell["Department"].Value);
            string depName = depCell["DepartmentName"].Value.ToString(); 

            KhoVatTu_KeHoachSX_Printer printer = new KhoVatTu_KeHoachSX_Printer(depID, depName, mPlantingManagement_dt, month, year);

            if (isPrintPreview)
                printer.PrintPreview(this);
            else
                printer.PrintDirect();
        }

        private void InLichUom_btn_Click(object sender, EventArgs e)
        {
            _ = InLichUom(false);
        }

        private void XemLichUom_btn_Click(object sender, EventArgs e)
        {
            _ = InLichUom(true);
        }

        private async void ExcelLichUom_btn_Click(object sender, EventArgs e)
        {
            if (weekInYear_GV.SelectedRows.Count == 0)
                return;

            var weekInYearCell = weekInYear_GV.SelectedRows[0].Cells;
            if (weekInYearCell == null) return;

            DateTime startDate = Convert.ToDateTime(weekInYearCell["StartDate"].Value);
            DateTime endDate = Convert.ToDateTime(weekInYearCell["EndDate"].Value);
            string weekName = Convert.ToString(weekInYearCell["WeekName"].Value);
            DataTable plantTrayDensity = await SQLStore_KhoVatTu.Instance.GetPlantTrayDensityAsync();

            DataView dv = new DataView(mPlantingManagement_dt);
            dv.RowFilter = $"PlantingDate >= #{startDate:MM/dd/yyyy}# AND PlantingDate < #{endDate:MM/dd/yyyy}#";
            dv.Sort = "PlantingDate ASC";

            DataTable filterResult_dt = dv.ToTable();

            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Message = "Đang xử lý ...";
            loadingOverlay.Show();
            await Task.Delay(100);

            try
            {
                using (var wb = new XLWorkbook())
                {
                    var ws = wb.Worksheets.Add("Lịch Ươm Cây");
                    ws.Style.Font.FontName = "Times New Roman";
                    ws.Style.Font.FontSize = 12;

                    int rowExcel = 1;

                    // Title
                    ws.Cell(rowExcel, 1).Value = $"LỊCH ƯƠM HẠT {weekName.ToUpper()}";
                    ws.Range(rowExcel, 1, rowExcel, 9).Merge();
                    ws.Cell(rowExcel, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell(rowExcel, 1).Style.Font.Bold = true;
                    ws.Cell(rowExcel, 1).Style.Font.FontSize = 24;

                    rowExcel++;

                    ws.Cell(rowExcel, 1).Value = $"({startDate:dd/MM} - {endDate:dd/MM})";
                    ws.Range(rowExcel, 1, rowExcel, 9).Merge();
                    ws.Cell(rowExcel, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell(rowExcel, 1).Style.Font.Bold = true;
                    ws.Cell(rowExcel, 1).Style.Font.FontSize = 24;

                    rowExcel ++;

                    // Header
                    ws.Cell(rowExcel, 1).Value = "Thứ";
                    ws.Cell(rowExcel, 2).Value = "Ngày Ươm";
                    ws.Cell(rowExcel, 3).Value = "Lệnh SX";
                    ws.Cell(rowExcel, 4).Value = "Tên Cây";
                    ws.Cell(rowExcel, 5).Value = "S.Lượng";
                    ws.Cell(rowExcel, 6).Value = "S.Khay";
                    ws.Cell(rowExcel, 7).Value = "Ngày Trồng";
                    ws.Cell(rowExcel, 8).Value = "D.Tích";
                    ws.Cell(rowExcel, 9).Value = "Ghi Chú";

                    ws.Range(rowExcel, 1, rowExcel, 9).Style.Font.Bold = true;

                    rowExcel++;

                    foreach (DataRow row in filterResult_dt.Rows)
                    {
                        string productionOrder = row["ProductionOrder"].ToString();
                        string note = row["Note"].ToString();
                        string plantName = row["PlantName"].ToString();
                        int quantity = Convert.ToInt32(row["Quantity"]);
                        DateTime nurseryDate = Convert.ToDateTime(row["NurseryDate"]);
                        DateTime plantingDate = Convert.ToDateTime(row["PlantingDate"]);
                        decimal area = Convert.ToDecimal(row["Area"]);
                        int sku = Convert.ToInt32(row["SKU"]);

                        DataRow plantTrayDensityRow = plantTrayDensity.Select($"SKU = '{sku}'").FirstOrDefault();

                        decimal trayPerSquareMeter = 0;

                        if (plantTrayDensityRow != null)
                            trayPerSquareMeter = Convert.ToDecimal(plantTrayDensityRow["TrayPerSquareMeter"]);

                        decimal soKhay = area * trayPerSquareMeter;

                        ws.Cell(rowExcel, 1).Value = Utils.GetThu_Viet(nurseryDate);
                        ws.Cell(rowExcel, 2).Value = nurseryDate;
                        ws.Cell(rowExcel, 3).Value = productionOrder;
                        ws.Cell(rowExcel, 4).Value = plantName;
                        ws.Cell(rowExcel, 5).Value = quantity;
                        ws.Cell(rowExcel, 6).Value = soKhay;
                        ws.Cell(rowExcel, 7).Value = plantingDate;
                        ws.Cell(rowExcel, 8).Value = area;
                        ws.Cell(rowExcel, 9).Value = note;

                        ws.Cell(rowExcel, 2).Style.DateFormat.Format = "dd/MM/yyyy";
                        ws.Cell(rowExcel, 7).Style.DateFormat.Format = "dd/MM/yyyy";

                        rowExcel++;
                    }

                    ws.Range(3, 1, rowExcel - 1, 9).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    ws.Range(3, 1, rowExcel - 1, 9).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                    ws.Columns().AdjustToContents();

                    using (SaveFileDialog sfd = new SaveFileDialog())
                    {
                        sfd.Filter = "Excel Workbook|*.xlsx";
                        sfd.FileName = $"LichUomCay_{Utils.RemoveVietnameseSigns(weekName).Replace(" ", "")}";
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

        private async Task InLichUom(bool isPrintPreview)
        {
            if (weekInYear_GV.SelectedRows.Count == 0)
                return;

            var weekInYearCell = weekInYear_GV.SelectedRows[0].Cells;
            if (weekInYearCell == null) return;

            DateTime startDate = Convert.ToDateTime(weekInYearCell["StartDate"].Value);
            DateTime endDate = Convert.ToDateTime(weekInYearCell["EndDate"].Value);
            string weekName = Convert.ToString(weekInYearCell["WeekName"].Value);
            DataTable plantTrayDensity = await SQLStore_KhoVatTu.Instance.GetPlantTrayDensityAsync();
            KhoVatTu_LichUomCay_Printer printer = new KhoVatTu_LichUomCay_Printer(weekName, startDate, endDate, plantTrayDensity, mPlantingManagement_dt);

            if (isPrintPreview)
                printer.PrintPreview(this);
            else
                printer.PrintDirect();
        }
        //System.Data.DataTable excelData;
        //private async void button1_Click(object sender, EventArgs e)
        //{

        //    var unit_dt = await SQLManager_KhoVatTu.Instance.GetUnitAsync();
        //    var materialCategory_dt = await SQLManager_KhoVatTu.Instance.GetMaterialCategoryAsync();

        //    using (System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog())
        //    {
        //        ofd.Title = "Chọn file Excel";
        //        ofd.Filter = "Excel Files|*.xlsx;*.xls|All Files|*.*";
        //        ofd.Multiselect = false; // chỉ cho chọn 1 file

        //        if (ofd.ShowDialog() == DialogResult.OK)
        //        {
        //            string filePath = ofd.FileName;
        //            excelData = Utils.LoadExcel_NoHeader(filePath);

        //            var employees = await SQLStore_QLNS.Instance.GetEmployeesAsync();
        //            var departments = await SQLStore_QLNS.Instance.GetDepartmentAsync();
        //            var products = await SQLStore_Kho.Instance.getProductSKUAsync();

        //            Utils.AddColumnIfNotExists(excelData, "Quantity", typeof(int));
        //            Utils.AddColumnIfNotExists(excelData, "Area", typeof(decimal));
        //            Utils.AddColumnIfNotExists(excelData, "Supervisor", typeof(string));
        //            Utils.AddColumnIfNotExists(excelData, "Department", typeof(int));
        //            Utils.AddColumnIfNotExists(excelData, "SKU", typeof(int));
        //            foreach (DataRow row in excelData.Rows)
        //            {
        //                string raw = row["Column3"].ToString(); 
        //                string numberPart = raw.Replace("cây", "").Trim();
        //                numberPart = numberPart.Replace(".", "");
        //                row["Quantity"] = int.Parse(numberPart);

        //                raw = row["Column6"].ToString().ToLower();
        //                numberPart = raw.Replace("m2", "").Trim();
        //                numberPart = numberPart.Replace("m3", "").Trim();
        //                numberPart = numberPart.Replace(",", ".");
        //                if (string.IsNullOrWhiteSpace(numberPart))
        //                    numberPart = "0";
        //                row["Area"] = decimal.Parse(numberPart, System.Globalization.CultureInfo.InvariantCulture);

        //                raw = row["Column9"].ToString().ToLower();
        //                DataRow emp = employees.AsEnumerable().FirstOrDefault(r => r.Field<string>("FullName").Trim().ToLower().Equals(raw.Trim().ToLower(), StringComparison.OrdinalIgnoreCase));
        //                if(emp != null)
        //                    row["Supervisor"] = emp["EmployeeCode"];

        //                raw = row["Column8"].ToString().ToLower();
        //                if (raw.CompareTo("farm") == 0)
        //                    raw = "farm vr";
        //                DataRow dep = departments.AsEnumerable().FirstOrDefault(r => r.Field<string>("DepartmentName").Trim().ToLower().Equals(raw.Trim().ToLower(), StringComparison.OrdinalIgnoreCase));
        //                if (dep != null)
        //                    row["Department"] = dep["DepartmentID"];

        //                raw = row["Column2"].ToString().ToLower();
        //                if (raw.CompareTo("cải cúc") == 0)
        //                    raw = "tần ô (cải cúc)";
        //                DataRow product = products.AsEnumerable().FirstOrDefault(r => r.Field<string>("ProductNameVN").Trim().ToLower().Equals(raw.Trim().ToLower(), StringComparison.OrdinalIgnoreCase));
        //                if (product != null)
        //                    row["SKU"] = product["SKU"];
        //            }

        //            dataGV.DataSource = excelData;
        //        }
        //    }
        //}

        //private async void button2_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        foreach (DataRow edr in excelData.Rows)
        //        {

        //            string ProductionOrder = edr["Column1"].ToString();
        //            int SKU = Convert.ToInt32(edr["SKU"]);
        //            decimal Area = Convert.ToDecimal(edr["Area"]);
        //            int Quantity = Convert.ToInt32(edr["Quantity"]);
        //            DateTime NurseryDate = Convert.ToDateTime(edr["Column4"]);
        //            DateTime PlantingDate = Convert.ToDateTime(edr["Column5"]);
        //            DateTime HarvestDate = Convert.ToDateTime(edr["Column7"]);
        //            int? department = edr["Department"] == DBNull.Value ? (int?)null : Convert.ToInt32(edr["Department"]);
        //            string Supervisor = Convert.ToString(edr["Supervisor"]);

        //            string Note = "";
        //            if (excelData.Columns.Contains("Column10"))
        //                Note = Convert.ToString(edr["Column10"]);

        //            int salaryInfoID = await SQLManager_KhoVatTu.Instance.insertPlantingManagementAsync(ProductionOrder, SKU, Area, Quantity, NurseryDate, PlantingDate, HarvestDate, department, Supervisor, Note);

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("có lỗi " + ex.Message);
        //    }
        //    MessageBox.Show("xong");
        //}
    }
}
