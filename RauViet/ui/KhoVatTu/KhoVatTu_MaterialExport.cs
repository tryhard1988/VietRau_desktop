
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
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
    public partial class KhoVatTu_MaterialExport : Form
    {
        System.Data.DataTable mMaterial_dt, mEmployee_dt, mWorkType_dt, mPlantingManagement_dt, mMaterialExport_dt, mMaterialDepartment_dt, mReciever_dt;
        private DataView mLogDV;
        private Timer _monthYearDebounceTimer = new Timer { Interval = 500 };
        private Timer PlantingManagementDebounceTimer = new Timer { Interval = 300 };
        private Timer WorkTypeDebounceTimer = new Timer { Interval = 300 };
        private Timer EmployeeDebounceTimer = new Timer { Interval = 300 };
        private Timer MaterialDebounceTimer = new Timer { Interval = 300 };
        bool isNewState = false;
        private LoadingOverlay loadingOverlay;
        public KhoVatTu_MaterialExport()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            monthYear_dtp.Format = DateTimePickerFormat.Custom;
            monthYear_dtp.CustomFormat = "MM/yyyy";
            monthYear_dtp.ShowUpDown = true;
            monthYear_dtp.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            ngayXuat_dtp.Format = DateTimePickerFormat.Custom;
            ngayXuat_dtp.CustomFormat = "dd/MM/yyyy";

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            dayGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dayGV.MultiSelect = false;

            NVPhuTrach_GV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            NVPhuTrach_GV.MultiSelect = false;

            status_lb.Text = "";

            _monthYearDebounceTimer.Tick += MonthYearDebounceTimer_Tick;
            newCustomerBtn.Click += newBtn_Click;
            LuuThayDoiBtn.Click += saveBtn_Click;
            edit_btn.Click += Edit_btn_Click;
            readOnly_btn.Click += ReadOnly_btn_Click; 
            this.KeyDown += Kho_Materials_KeyDown;

            PlantingManagementDebounceTimer.Tick += plantingManagementDebounceTimer_Tick;
            LenhSX_CBB.TextUpdate += plantingManagement_CBB_TextUpdate;

            WorkTypeDebounceTimer.Tick += WokTypeDebounceTimer_Tick;
            congViec_CBB.TextUpdate += CongViec_CBB_TextUpdate;

            EmployeeDebounceTimer.Tick += EmployeeDebounceTimer_Tick;
            nguoiNhan_CBB.TextUpdate += NguoiNhan_CBB_TextUpdate;

            MaterialDebounceTimer.Tick += MaterialDebounceTimer_Tick;
            vatTu_CB.TextUpdate += VatTu_CB_TextUpdate;
            soLuong_tb.KeyPress += Tb_KeyPress_OnlyNumber;

            dayGV.SelectionChanged += DayGV_SelectionChanged;
            NVPhuTrach_GV.SelectionChanged += DayGV_SelectionChanged;
            goiYCapPhan_btn.Click += GoiYCapPhan_btn_Click;

            locTheoNgay_CB.CheckedChanged += Loc_CheckedChanged;
            locTheoNguoiPTrach_CB.CheckedChanged += Loc_CheckedChanged;

            excelPhanCongCV_btn.Click += ExcelPhanCongCV_btn_Click;
            xemPhanCongCV_btn.Click += XemPhanCongCV_btn_Click;
            inPhanCongCV_btn.Click += InPhanCongCV_btn_Click;
        }

        private void Kho_Materials_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                int month = monthYear_dtp.Value.Month;
                int year = monthYear_dtp.Value.Year;
                SQLStore_KhoVatTu.Instance.removeMaterialExport(month, year);
                ShowData();
            }
            else if (!isNewState && !edit_btn.Visible)
            {
                if (e.KeyCode == Keys.Delete)
                {
                    System.Windows.Forms.Control ctrl = this.ActiveControl;

                    if (ctrl is TextBox || ctrl is RichTextBox ||
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
                int month = monthYear_dtp.Value.Month;
                int year = monthYear_dtp.Value.Year;

                string[] empKeepColumns = { "EmployeeCode", "FullName", "EmployessName_NoSign" };
                var cateloryTask = SQLStore_KhoVatTu.Instance.GetMaterialCategoryAsync();

                var plantingManagementTask = SQLStore_KhoVatTu.Instance.getPlantingManagementAsync(false);
                var materialTask = SQLStore_KhoVatTu.Instance.getMaterialAsync();
                var workTypeTask = SQLStore_KhoVatTu.Instance.GetWorkTypeAsync();
                var materialDepartmentTask = SQLStore_KhoVatTu.Instance.GetMaterialDepartmentAsync();
                var employeeTask = SQLStore_QLNS.Instance.GetEmployeesAsync(empKeepColumns);
                var materialExportTask = SQLStore_KhoVatTu.Instance.GetMaterialExportAsync(month, year);
                var logDataTask = SQLStore_KhoVatTu.Instance.GetMaterialExportLogAsync(month, year);

                await Task.WhenAll(plantingManagementTask, cateloryTask, materialTask, employeeTask, workTypeTask, materialExportTask, logDataTask, materialDepartmentTask);

                mPlantingManagement_dt = plantingManagementTask.Result;
                mMaterial_dt = materialTask.Result;
                mEmployee_dt = employeeTask.Result;
                mWorkType_dt = workTypeTask.Result;
                mMaterialDepartment_dt = materialDepartmentTask.Result;
                mMaterialExport_dt = materialExportTask.Result;
                mLogDV = new DataView(logDataTask.Result);

                DataView materialExportDV = new DataView(mMaterialExport_dt);
                materialExportDV.RowFilter = "Receiver IS NOT NULL AND Receiver <> ''";
                mReciever_dt = materialExportDV.ToTable(true, "RecieverName", "Receiver");

                dayGV.DataSource = Utils.CreateDateTable(month, year);

                vatTu_CB.DataSource = mMaterial_dt;
                vatTu_CB.DisplayMember = "MaterialName";  // hiển thị tên
                vatTu_CB.ValueMember = "MaterialID";
                vatTu_CB.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

                nguoiNhan_CBB.DataSource = mEmployee_dt;
                nguoiNhan_CBB.DisplayMember = "FullName";  // hiển thị tên
                nguoiNhan_CBB.ValueMember = "EmployeeCode";
                nguoiNhan_CBB.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

                congViec_CBB.DataSource = mWorkType_dt;
                congViec_CBB.DisplayMember = "WorkTypeName";  // hiển thị tên
                congViec_CBB.ValueMember = "WorkTypeID";
                congViec_CBB.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

                LenhSX_CBB.DataSource = mPlantingManagement_dt;
                LenhSX_CBB.DisplayMember = "LenhSX_CayTrong";  // hiển thị tên
                LenhSX_CBB.ValueMember = "PlantingID";
                LenhSX_CBB.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

                boPhan_cbb.DataSource = mMaterialDepartment_dt;
                boPhan_cbb.DisplayMember = "MaterialDepartmentName";  // hiển thị tên
                boPhan_cbb.ValueMember = "MaterialDepartmentID";

                NVPhuTrach_GV.DataSource = mReciever_dt;
                dataGV.DataSource = mMaterialExport_dt;
                log_GV.DataSource = mLogDV;

            //    Utils.HideColumns(NVPhuTrach_GV, new[] { "Receiver" });
                Utils.SetGridHeaders(NVPhuTrach_GV, new System.Collections.Generic.Dictionary<string, string> {
                        {"RecieverName", "Người Phụ Trách" }
                    });
                Utils.SetGridWidths(NVPhuTrach_GV, new System.Collections.Generic.Dictionary<string, int> {
                        {"RecieverName", 150 }
                    });

                Utils.HideColumns(dataGV, new[] { "NurseryDate", "PlantingID", "Receiver", "MaterialID", "WorkTypeID", "ExportID", "MaterialDepartmentID", "IsCompleted" });
                Utils.SetGridOrdinal(mMaterialExport_dt, new[] { "ExportDate", "MaterialName", "UnitName", "Amount", "ProductionOrder", "WorkTypeName", "RecieverName", "MaterialDepartmentName", "Note" });
                Utils.SetGridHeaders(dataGV, new System.Collections.Generic.Dictionary<string, string> {
                        {"ExportDate", "Ngày Xuất" },
                        {"Amount", "S.Lượng" },
                        {"Note", "Ghi Chú" },
                        {"ProductionOrder", "Lệnh SX" },
                        {"WorkTypeName", "Công Việc" },
                        {"RecieverName", "Người Nhận" },
                        {"PlantName", "Cây Trồng" },
                        {"MaterialName", "Vật Tư" },
                        {"IsCompleted", "Đóng" },
                        {"UnitName", "Đ.Vị" },
                        {"MaterialDepartmentName", "Bộ Phận" }
                    });

                Utils.HideColumns(log_GV, new[] { "LogID", "ExportDate" });
                Utils.SetGridHeaders(log_GV, new System.Collections.Generic.Dictionary<string, string> {
                        {"OldValue", "Cũ" },
                        {"NewValue", "Mới" },
                        {"CreatedDate", "Ngày tạo" },
                        {"ActionBy", "Người tạo" }
                    });

                Utils.SetGridWidths(dataGV, new System.Collections.Generic.Dictionary<string, int> {
                        {"ExportDate", 70 },
                        {"Amount", 70 },
                        {"Note", 200 },
                        {"ProductionOrder", 70 },
                        {"WorkTypeName", 100 },
                        {"RecieverName", 150 },
                        {"PlantName", 100 },
                        {"IsCompleted", 50 },
                        {"UnitName", 50 },
                        {"MaterialName", 180 },
                        {"MaterialDepartmentName", 65 }
                    });

                

                Utils.SetTabStopRecursive(this, false);
                int countTab = 0;
                ngayXuat_dtp.TabIndex = countTab++; ngayXuat_dtp.TabStop = true;
                vatTu_CB.TabIndex = countTab++; vatTu_CB.TabStop = true;
                soLuong_tb.TabIndex = countTab++; soLuong_tb.TabStop = true;
                LenhSX_CBB.TabIndex = countTab++; LenhSX_CBB.TabStop = true;
                congViec_CBB.TabIndex = countTab++; congViec_CBB.TabStop = true;
                nguoiNhan_CBB.TabIndex = countTab++; nguoiNhan_CBB.TabStop = true;
                note_tb.TabIndex = countTab++; note_tb.TabStop = true;
                LuuThayDoiBtn.TabIndex = countTab++; LuuThayDoiBtn.TabStop = true;

                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                log_GV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                ReadOnly_btn_Click(null, null);
                dataGV.SelectionChanged += this.dataGV_CellClick;
                monthYear_dtp.ValueChanged += monthYearDtp_ValueChanged;
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

        private void plantingManagement_CBB_TextUpdate(object sender, EventArgs e)
        {
            // Restart timer mỗi khi gõ
            PlantingManagementDebounceTimer.Stop();
            PlantingManagementDebounceTimer.Start();
        }
        private void plantingManagementDebounceTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                PlantingManagementDebounceTimer.Stop();

                string typed = LenhSX_CBB.Text ?? "";
                string plain = Utils.RemoveVietnameseSigns(typed).ToLower();
                
                // Filter bằng LINQ
                var filtered = mPlantingManagement_dt.AsEnumerable()
                    .Where(r => r["search_nosign"].ToString().ToLower()
                    .Contains(plain));
                
                System.Data.DataTable temp;
                if (filtered.Any())
                    temp = filtered.CopyToDataTable();
                else
                    temp = mPlantingManagement_dt.Clone(); // nếu không có kết quả thì trả về table rỗng

                // Gán lại DataSource
                LenhSX_CBB.DataSource = temp;
                LenhSX_CBB.DisplayMember = "LenhSX_CayTrong";  // hiển thị tên
                LenhSX_CBB.ValueMember = "PlantingID";

                // Giữ lại text người đang gõ
                LenhSX_CBB.DroppedDown = true;
                LenhSX_CBB.Text = typed;
                LenhSX_CBB.SelectionStart = typed.Length;
                LenhSX_CBB.SelectionLength = 0;
            }
            catch { }
        }

        private void CongViec_CBB_TextUpdate(object sender, EventArgs e)
        {
            WorkTypeDebounceTimer.Stop();
            WorkTypeDebounceTimer.Start();
        }

        private void WokTypeDebounceTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                WorkTypeDebounceTimer.Stop();

                string typed = congViec_CBB.Text ?? "";
                string plain = Utils.RemoveVietnameseSigns(typed).ToLower();

                // Filter bằng LINQ
                var filtered = mWorkType_dt.AsEnumerable()
                    .Where(r => r["search_nosign"].ToString().ToLower()
                    .Contains(plain));

                System.Data.DataTable temp;
                if (filtered.Any())
                    temp = filtered.CopyToDataTable();
                else
                    temp = mWorkType_dt.Clone(); // nếu không có kết quả thì trả về table rỗng
                
                // Gán lại DataSource
                congViec_CBB.DataSource = temp;
                congViec_CBB.DisplayMember = "WorkTypeName";  // hiển thị tên
                congViec_CBB.ValueMember = "WorkTypeID";

                // Giữ lại text người đang gõ
                congViec_CBB.DroppedDown = true;
                congViec_CBB.Text = typed;
                congViec_CBB.SelectionStart = typed.Length;
                congViec_CBB.SelectionLength = 0;
            }
            catch { }
        }

        private void NguoiNhan_CBB_TextUpdate(object sender, EventArgs e)
        {
            EmployeeDebounceTimer.Stop();
            EmployeeDebounceTimer.Start();
        }

        private void EmployeeDebounceTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                EmployeeDebounceTimer.Stop();

                string typed = nguoiNhan_CBB.Text ?? "";
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
                nguoiNhan_CBB.DataSource = temp;
                nguoiNhan_CBB.DisplayMember = "FullName";  // hiển thị tên
                nguoiNhan_CBB.ValueMember = "EmployeeCode";

                // Giữ lại text người đang gõ
                nguoiNhan_CBB.DroppedDown = true;
                nguoiNhan_CBB.Text = typed;
                nguoiNhan_CBB.SelectionStart = typed.Length;
                nguoiNhan_CBB.SelectionLength = 0;
            }
            catch { }
        }

        private void VatTu_CB_TextUpdate(object sender, EventArgs e)
        {
            MaterialDebounceTimer.Stop();
            MaterialDebounceTimer.Start();
        }

        private void MaterialDebounceTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                MaterialDebounceTimer.Stop();

                string typed = vatTu_CB.Text ?? "";
                string plain = Utils.RemoveVietnameseSigns(typed).ToLower();

                // Filter bằng LINQ
                var filtered = mMaterial_dt.AsEnumerable()
                    .Where(r => r["MaterialName_nosign"].ToString().ToLower()
                    .Contains(plain));

                System.Data.DataTable temp;
                if (filtered.Any())
                    temp = filtered.CopyToDataTable();
                else
                    temp = mMaterial_dt.Clone(); // nếu không có kết quả thì trả về table rỗng
                
                // Gán lại DataSource
                vatTu_CB.DataSource = temp;
                vatTu_CB.DisplayMember = "MaterialName";  // hiển thị tên
                vatTu_CB.ValueMember = "MaterialID";

                // Giữ lại text người đang gõ
                vatTu_CB.DroppedDown = true;
                vatTu_CB.Text = typed;
                vatTu_CB.SelectionStart = typed.Length;
                vatTu_CB.SelectionLength = 0;
            }
            catch { }
        }

        private void DayGV_SelectionChanged(object sender, EventArgs e)
        {
            List<string> andConditions = new List<string>();

            if (locTheoNgay_CB.Checked && dayGV.SelectedRows.Count > 0)
            {
                var cells = dayGV.SelectedRows[0].Cells;
                if (cells == null) 
                    return;
                DateTime date = Convert.ToDateTime(cells["Date"].Value);
                andConditions.Add($"ExportDate >= #{date:MM/dd/yyyy}# AND ExportDate < #{date.AddDays(1):MM/dd/yyyy}#");
            }

            if (locTheoNguoiPTrach_CB.Checked && NVPhuTrach_GV.SelectedRows.Count > 0)
            {
                var cells = NVPhuTrach_GV.SelectedRows[0].Cells;
                if (cells == null)
                    return;
                string receiver = Convert.ToString(cells["Receiver"].Value);
                andConditions.Add($"Receiver = '{receiver}'");
            }

            string filter = "";
            if (andConditions.Count > 0)
            {
                filter += string.Join(" AND ", andConditions);
            }

            DataView dv = mMaterialExport_dt.DefaultView;
            dv.RowFilter = filter;

            if (dayGV.SelectedRows.Count > 0)
            {
                var cells = dayGV.SelectedRows[0].Cells;
                if (cells == null)
                    return;
                DateTime date = Convert.ToDateTime(cells["Date"].Value);
                mLogDV.RowFilter = $"ExportDate >= #{date:MM/dd/yyyy}# AND ExportDate < #{date.AddDays(1):MM/dd/yyyy}#";
            }
        }

        private void dataGV_CellClick(object sender, EventArgs e)
        {
            updateRightUI();            
        }

        private async void updateItem(int ExportID, DateTime ExportDate, int MaterialID, Decimal Amount, int? PlantingID, int? WorkTypeID, string Receiver, string Note, int MaterialDepartmentID)
        {
            DataRow[] plantingRows = Array.Empty<DataRow>();
            if (PlantingID.HasValue)
                plantingRows = mPlantingManagement_dt.Select($"PlantingID = {PlantingID}");

            DataRow[] workTypeRows = Array.Empty<DataRow>();
            if (WorkTypeID.HasValue)
                workTypeRows = mWorkType_dt.Select($"WorkTypeID = {WorkTypeID}");

            DataRow[] matiralRows = matiralRows = mMaterial_dt.Select($"MaterialID = {MaterialID}");
            DataRow[] employeeRows = mEmployee_dt.Select($"EmployeeCode = '{Receiver}'");
            DataRow[] materialDepartmentRows = mMaterialDepartment_dt.Select($"MaterialDepartmentID = {MaterialDepartmentID}");

            foreach (DataRow row in mMaterialExport_dt.Rows)
            {
                int ID = Convert.ToInt32(row["ExportID"]);
                if (ID.CompareTo(ExportID) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (dialogResult == DialogResult.Yes)
                    {
                        string oldValue = $"Update: ExportDate:{row["ExportDate"].ToString()}; PlantName:{row["PlantName"].ToString()}; MaterialName:{row["MaterialName"].ToString()}; RecieverName:{row["RecieverName"].ToString()}; WorkTypeName:{row["WorkTypeName"].ToString()}; Amount:{row["Amount"].ToString()}; Note: {row["Note"].ToString()}";
                        string newValue = "";
                        try
                        {
                            bool isScussess = await SQLManager_KhoVatTu.Instance.updateMaterialExportAsync(ExportID, ExportDate, MaterialID, Amount, PlantingID, WorkTypeID, Receiver, Note, MaterialDepartmentID);

                            if (isScussess == true)
                            {
                                newValue = "Success: ";
                                row["ExportDate"] = ExportDate;
                                row["MaterialID"] = MaterialID;
                                row["Amount"] = Amount;
                                row["Note"] = Note;
                                row["MaterialDepartmentID"] = MaterialDepartmentID;
                                row["MaterialDepartmentName"] = materialDepartmentRows[0]["MaterialDepartmentName"].ToString();

                                if (PlantingID.HasValue)
                                {
                                    row["PlantingID"] = PlantingID;
                                    row["PlantName"] = plantingRows[0]["PlantName"].ToString();
                                    row["ProductionOrder"] = plantingRows[0]["ProductionOrder"].ToString();
                                    row["IsCompleted"] = plantingRows[0]["IsCompleted"].ToString();
                                    row["NurseryDate"] = Convert.ToDateTime(plantingRows[0]["NurseryDate"]);

                                    newValue += $"PlantName: {plantingRows[0]["PlantName"].ToString()}; ";
                                }

                                if (matiralRows.Length > 0)
                                {
                                    row["UnitName"] = matiralRows[0]["UnitName"].ToString();
                                    row["MaterialName"] = matiralRows[0]["MaterialName"].ToString();

                                    newValue += $"MaterialName: {matiralRows[0]["MaterialName"].ToString()}; ";
                                }
                                if (employeeRows.Length > 0)
                                {
                                    row["Receiver"] = Receiver;
                                    row["RecieverName"] = employeeRows[0]["FullName"].ToString();

                                    newValue += $"RecieverName: {employeeRows[0]["FullName"].ToString()}; ";
                                }

                                if (WorkTypeID.HasValue)
                                {
                                    row["WorkTypeID"] = WorkTypeID;
                                    row["WorkTypeName"] = workTypeRows[0]["WorkTypeName"].ToString();

                                    newValue += $"WorkTypeName: {workTypeRows[0]["WorkTypeName"].ToString()}; ";
                                }

                                newValue += $"Amount: {Amount}; Note: {Note}";

                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;
                            }
                            else
                            {
                                newValue = "Fail: ";
                                status_lb.Text = "Thất bại.";
                                status_lb.ForeColor = Color.Red;
                            }
                        }
                        catch (Exception ex)
                        {
                            newValue = ex.Message;
                            status_lb.Text = "Thất bại.";
                            status_lb.ForeColor = Color.Red;
                        }
                        finally
                        {
                            _ = SQLManager_KhoVatTu.Instance.insertMaterialExportLogAsync(ExportDate, oldValue, newValue);
                        }
                    }
                    break;
                }
            }
        }

        private async void createItem(DateTime ExportDate, int MaterialID, Decimal Amount,int? PlantingID, int? WorkTypeID, string Receiver, string Note, int MaterialDepartmentID)
        {
            DataRow[] plantingRows = Array.Empty<DataRow>();
            if (PlantingID.HasValue)
                plantingRows = mPlantingManagement_dt.Select($"PlantingID = {PlantingID}");

            DataRow[] workTypeRows = Array.Empty<DataRow>();
            if (WorkTypeID.HasValue)
                workTypeRows = mWorkType_dt.Select($"WorkTypeID = {WorkTypeID}");

            DataRow[] matiralRows = matiralRows = mMaterial_dt.Select($"MaterialID = {MaterialID}");
            DataRow[] employeeRows = mEmployee_dt.Select($"EmployeeCode = '{Receiver}'");
            DataRow[] materialDepartmentRows = mMaterialDepartment_dt.Select($"MaterialDepartmentID = {MaterialDepartmentID}");

            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                string oldValue = "create: ";
                string newValue = "";
                try
                {
                    int newId = await SQLManager_KhoVatTu.Instance.insertMaterialExportAsync(ExportDate, MaterialID, Amount, PlantingID, WorkTypeID, Receiver, Note, MaterialDepartmentID);
                    if (newId > 0)
                    {
                        DataRow drToAdd = mMaterialExport_dt.NewRow();

                        drToAdd["ExportID"] = newId;
                        drToAdd["ExportDate"] = ExportDate;
                        drToAdd["MaterialID"] = MaterialID;
                        drToAdd["Amount"] = Amount;
                        drToAdd["Note"] = Note;
                        drToAdd["MaterialDepartmentID"] = MaterialDepartmentID;
                        drToAdd["MaterialDepartmentName"] = materialDepartmentRows[0]["MaterialDepartmentName"].ToString();

                        if (PlantingID.HasValue)
                        {
                            drToAdd["PlantingID"] = PlantingID;
                            drToAdd["PlantName"] = plantingRows[0]["PlantName"].ToString();
                            drToAdd["ProductionOrder"] = plantingRows[0]["ProductionOrder"].ToString();
                            drToAdd["IsCompleted"] = plantingRows[0]["IsCompleted"].ToString();
                            drToAdd["NurseryDate"] = Convert.ToDateTime(plantingRows[0]["NurseryDate"]);

                            newValue += $"PlantName: {plantingRows[0]["PlantName"].ToString()}; ";
                        }

                        if (matiralRows.Length > 0)
                        {
                            drToAdd["UnitName"] = matiralRows[0]["UnitName"].ToString();
                            drToAdd["MaterialName"] = matiralRows[0]["MaterialName"].ToString();

                            newValue += $"MaterialName: {matiralRows[0]["MaterialName"].ToString()}; ";
                        }
                        if (employeeRows.Length > 0)
                        {
                            drToAdd["Receiver"] = Receiver;
                            drToAdd["RecieverName"] = employeeRows[0]["FullName"].ToString();
                            newValue += $"RecieverName: {employeeRows[0]["FullName"].ToString()}; ";
                        }

                        if (WorkTypeID.HasValue)
                        {
                            drToAdd["WorkTypeID"] = WorkTypeID;
                            drToAdd["WorkTypeName"] = workTypeRows[0]["WorkTypeName"].ToString();
                            newValue += $"WorkTypeName: {workTypeRows[0]["WorkTypeName"].ToString()}; ";
                        }

                        newValue += $"Amount: {Amount}; Note: {Note}";

                        mMaterialExport_dt.Rows.Add(drToAdd);
                        mMaterialExport_dt.AcceptChanges();

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
                    _ = SQLManager_KhoVatTu.Instance.insertMaterialExportLogAsync(ExportDate, oldValue, newValue);
                }
            }
        }
        private void saveBtn_Click(object sender, EventArgs e)
        {
            if (boPhan_cbb.SelectedValue == null ||vatTu_CB.SelectedValue == null || string.IsNullOrEmpty(soLuong_tb.Text.Trim())) return;

            DateTime ngayXuat = ngayXuat_dtp.Value;
            int vatTu = Convert.ToInt32(vatTu_CB.SelectedValue);
            decimal soLuong = Utils.ParseDecimalSmart(soLuong_tb.Text);
            int? lenhSX = null;
            if(!string.IsNullOrWhiteSpace(LenhSX_CBB.Text))
                lenhSX = LenhSX_CBB.SelectedValue as int?;
            int? congViec = null;
            if(string.IsNullOrWhiteSpace(congViec_CBB.Text))
                congViec = congViec_CBB.SelectedValue as int?;
            string nguoiNhan = nguoiNhan_CBB.SelectedValue as string;
            string note = note_tb.Text;

            string name = soLuong_tb.Text;
            int unitID = Convert.ToInt32(LenhSX_CBB.SelectedValue);
            int cateloryID = Convert.ToInt32(vatTu_CB.SelectedValue);
            int materialDepartmentID = Convert.ToInt32(boPhan_cbb.SelectedValue);

            if (id_tb.Text.Length != 0)
                updateItem(Convert.ToInt32(id_tb.Text), ngayXuat, vatTu, soLuong, lenhSX, congViec, nguoiNhan, note, materialDepartmentID);
            else
                createItem(ngayXuat, vatTu, soLuong, lenhSX, congViec, nguoiNhan, note, materialDepartmentID);

        }
        private async void deleteProduct()
        {
            string id = id_tb.Text;

            foreach (DataRow row in mMaterialExport_dt.Rows)
            {
                string exportID = row["ExportID"].ToString();
                if (exportID.CompareTo(id) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Xóa Nha, Chắc Chắn Chưa!", "Thông Báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (dialogResult == DialogResult.Yes)
                    {
                        DateTime exportDate = Convert.ToDateTime(row["ExportDate"]);
                        string oldValue = $"ExportDate:{exportDate.ToString("dd/MM/yyyy")}; PlantName:{row["PlantName"].ToString()}; MaterialName:{row["MaterialName"].ToString()}; RecieverName:{row["RecieverName"].ToString()}; WorkTypeName:{row["WorkTypeName"].ToString()}; Amount:{row["Amount"].ToString()}; Note: {row["Note"].ToString()}";
                        string newValue = "Delete: ";                        
                        try
                        {
                            bool isScussess = await SQLManager_KhoVatTu.Instance.deletetMaterialExportAsync(Convert.ToInt32(id));

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                mMaterialExport_dt.Rows.Remove(row);
                                mMaterialExport_dt.AcceptChanges();

                                newValue += "Success";
                            }
                            else
                            {
                                newValue += "Fail";
                                status_lb.Text = "Thất bại.";
                                status_lb.ForeColor = Color.Red;
                            }
                        }
                        catch (Exception ex)
                        {
                            newValue += ex.Message;
                            status_lb.Text = "Thất bại.";
                            status_lb.ForeColor = Color.Red;
                        }
                        finally
                        {
                            _ = SQLManager_KhoVatTu.Instance.insertMaterialExportLogAsync(exportDate, oldValue, newValue);
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

            info_gb.BackColor = newCustomerBtn.BackColor;
            edit_btn.Visible = false;
            newCustomerBtn.Visible = false;
            readOnly_btn.Visible = true;
            LuuThayDoiBtn.Visible = true;
            isNewState = true;
            LuuThayDoiBtn.Text = "Lưu Mới";
            soLuong_tb.Enabled = true;
            LenhSX_CBB.Enabled = true;
            vatTu_CB.Enabled = true;
            boPhan_cbb.Enabled = true;
            ngayXuat_dtp.Enabled = true;
            congViec_CBB.Enabled = true;
            nguoiNhan_CBB.Enabled = true;
            note_tb.Enabled = true;
            ngayXuat_dtp.Focus();
        }

        private void ReadOnly_btn_Click(object sender, EventArgs e)
        {
            edit_btn.Visible = true;
            newCustomerBtn.Visible = true;
            readOnly_btn.Visible = false;
            LuuThayDoiBtn.Visible = false;
            info_gb.BackColor = Color.DarkGray;
            isNewState = false;
            soLuong_tb.Enabled = false;
            LenhSX_CBB.Enabled = false;
            vatTu_CB.Enabled = false;
            boPhan_cbb.Enabled = false;
            ngayXuat_dtp.Enabled = false;
            congViec_CBB.Enabled = false;
            nguoiNhan_CBB.Enabled = false;
            note_tb.Enabled = false;
            if (dataGV.SelectedRows.Count > 0)
                updateRightUI();
        }

        private void Edit_btn_Click(object sender, EventArgs e)
        {
            soLuong_tb.Enabled = true;
            LenhSX_CBB.Enabled = true;
            vatTu_CB.Enabled = true;
            boPhan_cbb.Enabled = true;
            edit_btn.Visible = false;
            newCustomerBtn.Visible = false;
            readOnly_btn.Visible = true;
            LuuThayDoiBtn.Visible = true;
            ngayXuat_dtp.Enabled = true;
            congViec_CBB.Enabled = true;
            nguoiNhan_CBB.Enabled = true;
            note_tb.Enabled = true;
            info_gb.BackColor = edit_btn.BackColor;
            isNewState = false;
            LuuThayDoiBtn.Text = "Lưu C.Sửa";
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

        private void updateRightUI()
        {
            try
            {
                if (isNewState) return;

                if (dataGV.SelectedRows.Count > 0)
                {
                    var cells = dataGV.SelectedRows[0].Cells;

                    DateTime ngayXuat = Convert.ToDateTime(cells["ExportDate"].Value);
                    string receiver = Convert.ToString(cells["Receiver"].Value);
                    int materialID = int.TryParse(cells["MaterialID"].Value?.ToString(), out int materialTemp) ? materialTemp : -1;
                    int workTypeID = int.TryParse(cells["WorkTypeID"].Value?.ToString(), out int workTypeTemp) ? workTypeTemp : -1;
                    int plantingID = int.TryParse(cells["PlantingID"].Value?.ToString(), out int plantingTemp) ? plantingTemp : -1;
                    int materialDepartmentID = int.TryParse(cells["MaterialDepartmentID"].Value?.ToString(), out int materialDepartmentTemp) ? materialDepartmentTemp : -1;

                    if (!vatTu_CB.Items.Cast<object>().Any(i => ((DataRowView)i)["MaterialID"].ToString() == materialID.ToString()))
                    {
                        vatTu_CB.DataSource = mMaterial_dt;
                    }

                    if (!congViec_CBB.Items.Cast<object>().Any(i => ((DataRowView)i)["WorkTypeID"].ToString() == workTypeID.ToString()))
                    {
                        congViec_CBB.DataSource = mWorkType_dt;
                    }

                    if (!LenhSX_CBB.Items.Cast<object>().Any(i => ((DataRowView)i)["PlantingID"].ToString() == plantingID.ToString()))
                    {
                        LenhSX_CBB.DataSource = mPlantingManagement_dt;
                    }

                    id_tb.Text = cells["ExportID"].Value.ToString();
                    ngayXuat_dtp.Value = ngayXuat;
                    vatTu_CB.SelectedValue = materialID;
                    congViec_CBB.SelectedValue = workTypeID;
                    LenhSX_CBB.SelectedValue = plantingID;
                    nguoiNhan_CBB.SelectedValue = receiver;
                    nguoiNhan_CBB.SelectedValue = receiver;
                    boPhan_cbb.SelectedValue = materialDepartmentID;
                    soLuong_tb.Text = Convert.ToDecimal(cells["Amount"].Value).ToString("G29", CultureInfo.InvariantCulture);
                    note_tb.Text = Convert.ToString(cells["Note"].Value);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
            }
        }


        private void Tb_KeyPress_OnlyNumber(object sender, KeyPressEventArgs e)
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

        private void GoiYCapPhan_btn_Click(object sender, EventArgs e)
        {
            if (dayGV.SelectedRows.Count == 0)
                return;
            var cells = dayGV.SelectedRows[0].Cells;
            if (cells == null) return;
            DateTime date = Convert.ToDateTime(cells["Date"].Value);

            monthYear_dtp.ValueChanged -= monthYearDtp_ValueChanged;
            int month = monthYear_dtp.Value.Month;
            int year = monthYear_dtp.Value.Year;
            KhoVatTu_GoiYCapPhan frm = new KhoVatTu_GoiYCapPhan(date);
            frm.FormClosed += KhoVatTu_GoiYCapPhan_FormClosed;
            frm.ShowData();
            frm.ShowDialog();
        }

        private void KhoVatTu_GoiYCapPhan_FormClosed(object sender, FormClosedEventArgs e)
        {
            monthYear_dtp.ValueChanged -= monthYearDtp_ValueChanged;
            int month = monthYear_dtp.Value.Month;
            int year = monthYear_dtp.Value.Year;
            SQLStore_KhoVatTu.Instance.removeMaterialExport(month, year);
            this.ShowData();
        }

        private void Loc_CheckedChanged(object sender, EventArgs e)
        {
            DayGV_SelectionChanged(null, null);
        }

        private void InPhanCongCV_btn_Click(object sender, EventArgs e)
        {
            PhanCongCVPrinter(false);
        }

        private void XemPhanCongCV_btn_Click(object sender, EventArgs e)
        {
            PhanCongCVPrinter(true);
        }

        private async void ExcelPhanCongCV_btn_Click(object sender, EventArgs e)
        {
            if (dayGV.SelectedRows.Count == 0 || NVPhuTrach_GV.SelectedRows.Count == 0)
                return;

            string empCode = Convert.ToString(NVPhuTrach_GV.SelectedRows[0].Cells["Receiver"].Value);
            string empName = Convert.ToString(NVPhuTrach_GV.SelectedRows[0].Cells["RecieverName"].Value);
            DateTime date = Convert.ToDateTime(dayGV.SelectedRows[0].Cells["Date"].Value);

            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Message = "Đang xử lý ...";
            loadingOverlay.Show();
            await Task.Delay(100);

            try
            {
                DateTime start = date;
                DateTime end = start.AddDays(1);

                DataView dv = new DataView(mMaterialExport_dt);
                dv.RowFilter = $"ExportDate >= #{start:MM/dd/yyyy}# AND ExportDate < #{end:MM/dd/yyyy}# AND Receiver = '{empCode}'";
                dv.Sort = "ExportDate ASC";

                var resultFilter_dt = dv.ToTable();

                using (var wb = new XLWorkbook())
                {
                    var ws = wb.Worksheets.Add("PhanCongCV");

                    int row = 1;

                    // 🔹 Title
                    ws.Cell(row, 1).Value = $"BẢNG PHÂN CÔNG CÔNG VIỆC {date:dd/MM/yyyy}";
                    ws.Range(row, 1, row, 8).Merge().Style.Font.SetBold().Font.SetFontSize(16).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    row++;

                    // 🔹 Header
                    string[] headers = { "Ngày", "Tên Cây Trồng", "LSX", "Công Việc", "Tên Vật Tư", "Số Lượng", "ĐVT", "Người Ph.Trách" };

                    for (int i = 0; i < headers.Length; i++)
                    {
                        ws.Cell(row, i + 1).Value = headers[i];
                        ws.Cell(row, i + 1).Style.Font.Bold = true;
                        ws.Cell(row, i + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
                    }
                    row++;

                    // 🔹 Data
                    foreach (DataRow dr in resultFilter_dt.Rows)
                    {
                        ws.Cell(row, 1).Value = Convert.ToDateTime(dr["ExportDate"]).ToString("dd/MM/yyyy");
                        ws.Cell(row, 2).Value = dr["PlantName"].ToString();
                        ws.Cell(row, 3).Value = dr["ProductionOrder"].ToString();
                        ws.Cell(row, 4).Value = dr["WorkTypeName"].ToString();
                        ws.Cell(row, 5).Value = dr["MaterialName"].ToString();
                        ws.Cell(row, 6).Value = Convert.ToDecimal(dr["Amount"]);
                        ws.Cell(row, 7).Value = dr["UnitName"].ToString();
                        ws.Cell(row, 8).Value = dr["RecieverName"].ToString();

                        row++;
                    }

                    ws.Style.Font.FontName = "Times New Roman";
                    ws.Style.Font.FontSize = 11;
                    ws.Range(2, 1, row - 1, 8).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    ws.Range(2, 1, row - 1, 8).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                    // 🔹 Format số
                    ws.Column(6).Style.NumberFormat.Format = "#,##0.00";

                    // 🔹 Auto width
                    ws.Columns().AdjustToContents();

                    
                    using (SaveFileDialog sfd = new SaveFileDialog())
                    {
                        sfd.Filter = "Excel Workbook|*.xlsx";
                        sfd.FileName = $"BangPhanCongCV_{date.ToString("dd_MM_yyyy")}_{Utils.RemoveVietnameseSigns(empName).Replace(" ", "")}";
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

        async void PhanCongCVPrinter(bool isPreview)
        {
            if (dayGV.SelectedRows.Count == 0 || NVPhuTrach_GV.SelectedRows.Count == 0)
                return;

            string empCode = Convert.ToString(NVPhuTrach_GV.SelectedRows[0].Cells["Receiver"].Value);
            string empName = Convert.ToString(NVPhuTrach_GV.SelectedRows[0].Cells["RecieverName"].Value);
            DateTime date = Convert.ToDateTime(dayGV.SelectedRows[0].Cells["Date"].Value);

            KhoVatTu_PhanCongCV_Printer printer = new KhoVatTu_PhanCongCV_Printer(empName, empCode, mMaterialExport_dt, date);

            if (isPreview)
                printer.PrintPreview(this);
            else
                printer.PrintDirect();
        }
    }
}
