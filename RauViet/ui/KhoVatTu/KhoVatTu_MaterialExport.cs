using DocumentFormat.OpenXml.Drawing;
using RauViet.classes;
using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using Color = System.Drawing.Color;

namespace RauViet.ui
{
    public partial class KhoVatTu_MaterialExport : Form
    {
        System.Data.DataTable mMaterial_dt, mEmployee_dt, mWorkType_dt, mPlantingManagement_dt, mMaterialExport_dt;
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
            //quantity_tb.KeyPress += Tb_KeyPress_OnlyNumber;
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
                var employeeTask = SQLStore_QLNS.Instance.GetEmployeesAsync(empKeepColumns);
                var materialExportTask = SQLStore_KhoVatTu.Instance.GetMaterialExportAsync(month, year);

                //    var empTask = SQLStore_QLNS.Instance.GetEmployeesAsync();
                //    var logDataTask = SQLStore_Kho.Instance.GetDomesticLiquidationImportLogAsync();
                await Task.WhenAll(plantingManagementTask, cateloryTask, materialTask, employeeTask, workTypeTask, materialExportTask);

                //    mDomesticLiquidationPrice_dt = domesticLiquidationPriceTask.Result;
                //    mDomesticLiquidationImport_dt = domesticLiquidationImportTask.Result;
                mPlantingManagement_dt = plantingManagementTask.Result;
                mMaterial_dt = materialTask.Result;
                mEmployee_dt = employeeTask.Result;
                mWorkType_dt = workTypeTask.Result;
                mMaterialExport_dt = materialExportTask.Result;
                ////    mLogDV = new DataView(logDataTask.Result);


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

                dataGV.DataSource = mMaterialExport_dt;
                ////    log_GV.DataSource = mLogDV;
                Utils.HideColumns(dataGV, new[] { "NurseryDate", "PlantingID", "Receiver", "MaterialID", "WorkTypeID", "ExportID" });                
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
                        {"UnitName", "Đ.Vị" }
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
                        {"MaterialName", 230 }
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

        private void dataGV_CellClick(object sender, EventArgs e)
        {
            updateRightUI();            
        }

        private async void updateItem(int ExportID, DateTime ExportDate, int MaterialID, Decimal Amount, int? PlantingID, int? WorkTypeID, string Receiver, string Note)
        {
            DataRow[] plantingRows = Array.Empty<DataRow>();
            if (PlantingID.HasValue)
                plantingRows = mPlantingManagement_dt.Select($"PlantingID = {PlantingID}");

            DataRow[] workTypeRows = Array.Empty<DataRow>();
            if (WorkTypeID.HasValue)
                workTypeRows = mWorkType_dt.Select($"WorkTypeID = {WorkTypeID}");

            DataRow[] matiralRows = matiralRows = mMaterial_dt.Select($"MaterialID = {MaterialID}");
            DataRow[] employeeRows = mEmployee_dt.Select($"EmployeeCode = '{Receiver}'");

            foreach (DataRow row in mMaterialExport_dt.Rows)
            {
                int ID = Convert.ToInt32(row["ExportID"]);
                if (ID.CompareTo(ExportID) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager_KhoVatTu.Instance.updateMaterialExportAsync(ExportID, ExportDate, MaterialID, Amount, PlantingID, WorkTypeID, Receiver, Note);

                            if (isScussess == true)
                            {
                                row["ExportDate"] = ExportDate;
                                row["MaterialID"] = MaterialID;
                                row["Amount"] = Amount;
                                row["Note"] = Note;

                                if (PlantingID.HasValue)
                                {
                                    row["PlantingID"] = PlantingID;
                                    row["PlantName"] = plantingRows[0]["PlantName"].ToString();
                                    row["ProductionOrder"] = plantingRows[0]["ProductionOrder"].ToString();
                                    row["IsCompleted"] = plantingRows[0]["IsCompleted"].ToString();
                                    row["NurseryDate"] = Convert.ToDateTime(plantingRows[0]["NurseryDate"]);
                                }

                                if (matiralRows.Length > 0)
                                {
                                    row["UnitName"] = matiralRows[0]["UnitName"].ToString();
                                    row["MaterialName"] = matiralRows[0]["MaterialName"].ToString();
                                }
                                if (employeeRows.Length > 0)
                                {
                                    row["Receiver"] = Receiver;
                                    row["RecieverName"] = employeeRows[0]["FullName"].ToString();
                                }

                                if (WorkTypeID.HasValue)
                                {
                                    row["WorkTypeID"] = WorkTypeID;
                                    row["WorkTypeName"] = workTypeRows[0]["WorkTypeName"].ToString();
                                }

                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;
                            }
                            else
                            {
                                status_lb.Text = "Thất bại.";
                                status_lb.ForeColor = Color.Red;
                            }
                        }
                        catch (Exception ex)
                        {
                            status_lb.Text = "Thất bại.";
                            status_lb.ForeColor = Color.Red;
                        }
                    }
                    break;
                }
            }
        }

        private async void createItem(DateTime ExportDate, int MaterialID, Decimal Amount,int? PlantingID, int? WorkTypeID, string Receiver, string Note)
        {
            DataRow[] plantingRows = Array.Empty<DataRow>();
            if (PlantingID.HasValue)
                plantingRows = mPlantingManagement_dt.Select($"PlantingID = {PlantingID}");

            DataRow[] workTypeRows = Array.Empty<DataRow>();
            if (WorkTypeID.HasValue)
                workTypeRows = mWorkType_dt.Select($"WorkTypeID = {WorkTypeID}");

            DataRow[] matiralRows = matiralRows = mMaterial_dt.Select($"MaterialID = {MaterialID}");
            DataRow[] employeeRows = mEmployee_dt.Select($"EmployeeCode = '{Receiver}'");

            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    int newId = await SQLManager_KhoVatTu.Instance.insertMaterialExportAsync(ExportDate, MaterialID, Amount, PlantingID, WorkTypeID, Receiver, Note);
                    if (newId > 0)
                    {
                        DataRow drToAdd = mMaterialExport_dt.NewRow();

                        drToAdd["ExportID"] = newId;
                        drToAdd["ExportDate"] = ExportDate;
                        drToAdd["MaterialID"] = MaterialID;
                        drToAdd["Amount"] = Amount;                        
                        drToAdd["Note"] = Note;

                        if (PlantingID.HasValue)
                        {
                            drToAdd["PlantingID"] = PlantingID;
                            drToAdd["PlantName"] = plantingRows[0]["PlantName"].ToString();
                            drToAdd["ProductionOrder"] = plantingRows[0]["ProductionOrder"].ToString();
                            drToAdd["IsCompleted"] = plantingRows[0]["IsCompleted"].ToString();
                            drToAdd["NurseryDate"] = Convert.ToDateTime(plantingRows[0]["NurseryDate"]);
                        }

                        if (matiralRows.Length > 0)
                        {
                            drToAdd["UnitName"] = matiralRows[0]["UnitName"].ToString();
                            drToAdd["MaterialName"] = matiralRows[0]["MaterialName"].ToString();
                        }
                        if (employeeRows.Length > 0)
                        {
                            drToAdd["Receiver"] = Receiver;
                            drToAdd["RecieverName"] = employeeRows[0]["FullName"].ToString();
                        }

                        if (WorkTypeID.HasValue)
                        {
                            drToAdd["WorkTypeID"] = WorkTypeID;
                            drToAdd["WorkTypeName"] = workTypeRows[0]["WorkTypeName"].ToString();
                        }

                        mMaterialExport_dt.Rows.Add(drToAdd);
                        mMaterialExport_dt.AcceptChanges();

                        status_lb.Text = "Thành công";
                        status_lb.ForeColor = Color.Green;

                        newBtn_Click(null, null);
                    }
                    else
                    {
                        status_lb.Text = "Thất bại";
                        status_lb.ForeColor = Color.Red;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR: " + ex.Message);
                    status_lb.Text = "Thất bại.";
                    status_lb.ForeColor = Color.Red;
                }

            }
        }
        private void saveBtn_Click(object sender, EventArgs e)
        {
            if (vatTu_CB.SelectedValue == null || string.IsNullOrEmpty(soLuong_tb.Text.Trim())) return;

            DateTime ngayXuat = ngayXuat_dtp.Value;
            int vatTu = Convert.ToInt32(vatTu_CB.SelectedValue);
            decimal soLuong = Utils.ParseDecimalSmart(soLuong_tb.Text);
            int? lenhSX = LenhSX_CBB.SelectedValue as int?;
            int? congViec = congViec_CBB.SelectedValue as int?;
            string nguoiNhan = nguoiNhan_CBB.SelectedValue as string;
            string note = note_tb.Text;

            string name = soLuong_tb.Text;
            int unitID = Convert.ToInt32(LenhSX_CBB.SelectedValue);
            int cateloryID = Convert.ToInt32(vatTu_CB.SelectedValue);

            if (id_tb.Text.Length != 0)
                updateItem(Convert.ToInt32(id_tb.Text), ngayXuat, vatTu, soLuong, lenhSX, congViec, nguoiNhan, note);
            else
                createItem(ngayXuat, vatTu, soLuong, lenhSX, congViec, nguoiNhan, note);

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
                        try
                        {
                            bool isScussess = await SQLManager_KhoVatTu.Instance.deletetMaterialExportAsync(Convert.ToInt32(id));

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                mMaterialExport_dt.Rows.Remove(row);
                                mMaterialExport_dt.AcceptChanges();
                            }
                            else
                            {
                                status_lb.Text = "Thất bại.";
                                status_lb.ForeColor = Color.Red;
                            }
                        }
                        catch (Exception ex)
                        {
                            status_lb.Text = "Thất bại.";
                            status_lb.ForeColor = Color.Red;
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
            
            if (dataGV.SelectedRows.Count > 0)
                updateRightUI();
        }

        private void Edit_btn_Click(object sender, EventArgs e)
        {
            soLuong_tb.Enabled = true;
            LenhSX_CBB.Enabled = true;
            vatTu_CB.Enabled = true;
            edit_btn.Visible = false;
            newCustomerBtn.Visible = false;
            readOnly_btn.Visible = true;
            LuuThayDoiBtn.Visible = true;
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

        //            Utils.AddColumnIfNotExists(excelData, "WorkTypeID", typeof(int));
        //            Utils.AddColumnIfNotExists(excelData, "MaterialID", typeof(int));
        //            Utils.AddColumnIfNotExists(excelData, "PlantingID", typeof(int));
        //            Utils.AddColumnIfNotExists(excelData, "EmployeeCode", typeof(string));
        //            Utils.AddColumnIfNotExists(excelData, "Note", typeof(string));
        //            foreach (DataRow row in excelData.Rows)
        //            {
        //                string raw = Utils.RemoveVietnameseSigns(row["Column2"].ToString());

        //                if(raw.CompareTo(Utils.RemoveVietnameseSigns("Magnesium sulphate (Green mag)")) == 0)
        //                    raw = Utils.RemoveVietnameseSigns("BitterMag (MgSO4)");
        //                else if (raw.CompareTo(Utils.RemoveVietnameseSigns("YaraMila Winner")) == 0)
        //                    raw = Utils.RemoveVietnameseSigns("YaraMila Winner (15-9-20)");
        //                else if (raw.CompareTo(Utils.RemoveVietnameseSigns("Solu-K 0-0-51")) == 0)
        //                    raw = Utils.RemoveVietnameseSigns("Solu-K (K2SO4 Hàn Quốc)");

        //                DataRow material = mMaterial_dt.AsEnumerable().FirstOrDefault(r => Utils.RemoveVietnameseSigns(r.Field<string>("MaterialName")).Trim().ToLower().Equals(raw.Trim().ToLower(), StringComparison.OrdinalIgnoreCase));
        //                if(material != null)
        //                    row["MaterialID"] = material["MaterialID"];

        //                raw = Utils.RemoveVietnameseSigns(row["Column6"].ToString());
        //                DataRow employee = mEmployee_dt.AsEnumerable().FirstOrDefault(r => Utils.RemoveVietnameseSigns(r.Field<string>("FullName")).Trim().ToLower().Equals(raw.Trim().ToLower(), StringComparison.OrdinalIgnoreCase));
        //                if (employee != null)
        //                    row["EmployeeCode"] = employee["EmployeeCode"];

        //                raw = Utils.RemoveVietnameseSigns(row["Column5"].ToString());
        //                DataRow workType = mWorkType_dt.AsEnumerable().FirstOrDefault(r => Utils.RemoveVietnameseSigns(r.Field<string>("WorkTypeName")).Trim().ToLower().Equals(raw.Trim().ToLower(), StringComparison.OrdinalIgnoreCase));
        //                if (workType != null)
        //                    row["WorkTypeID"] = workType["WorkTypeID"];

        //                raw = Utils.RemoveVietnameseSigns(row["Column4"].ToString()).Trim();
        //                if (raw.CompareTo("28125020") == 0)
        //                    raw = "28126001";
        //                else if (raw.CompareTo("11126001") == 0)
        //                    raw = "11125005";
        //                else if (raw.CompareTo("143250100") == 0)
        //                    raw = "14325100";
        //                else if (raw.CompareTo("12126005") == 0) 
        //                    raw = "12125007";
        //                else if (raw.CompareTo("17225053") == 0)
        //                    raw = "17226001";
        //                else if (raw.StartsWith("MXC")) 
        //                {
        //                    row["Note"] = raw;
        //                    row["Column4"] = "";
        //                    raw = "";
        //                }
        //                else if (raw.CompareTo("33125001") == 0 || raw.CompareTo("32324002") == 0)
        //                {
        //                    row["Note"] = raw;
        //                    row["Column4"] = "";
        //                    raw = "";
        //                }

        //                DataRow plantingManagement = mPlantingManagement_dt.AsEnumerable().FirstOrDefault(r => Utils.RemoveVietnameseSigns(r.Field<string>("ProductionOrder")).Trim().ToLower().Equals(raw.Trim().ToLower(), StringComparison.OrdinalIgnoreCase));
        //                if (plantingManagement != null)
        //                    row["PlantingID"] = plantingManagement["PlantingID"];
        //            }
                    
        //            dataGV.DataSource = excelData;
        //            dataGV.Columns["Column2"].Visible = false;
        //            dataGV.Columns["Column6"].Visible = false;
        //            dataGV.Columns["Column5"].Visible = false;
        //            }
        //    }
        //}

        //private async void button2_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        foreach (DataRow edr in excelData.Rows)
        //        {
        //            if (string.IsNullOrWhiteSpace(edr["Column3"].ToString())) 
        //                continue;

        //            DateTime ExportDate = Convert.ToDateTime(edr["Column1"]);
        //            int MaterialID = Convert.ToInt32(edr["MaterialID"]);
        //            decimal Amount = Convert.ToDecimal(edr["Column3"]);
        //            int? PlantingID = edr["PlantingID"] == DBNull.Value ? (int?)null : Convert.ToInt32(edr["PlantingID"]);
        //            int? WorkTypeID = edr["WorkTypeID"] == DBNull.Value ? (int?)null : Convert.ToInt32(edr["WorkTypeID"]);
        //            string EmployeeCode = Convert.ToString(edr["EmployeeCode"]);
        //            string Note = Convert.ToString(edr["Note"]);


        //            int salaryInfoID = await SQLManager_KhoVatTu.Instance.insertMaterialExportAsync(ExportDate, MaterialID, Amount, PlantingID, WorkTypeID, EmployeeCode, Note);
        //            if(salaryInfoID <= 0)
        //            {
        //                MessageBox.Show("có lỗi ");
        //                return;
        //            }

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
