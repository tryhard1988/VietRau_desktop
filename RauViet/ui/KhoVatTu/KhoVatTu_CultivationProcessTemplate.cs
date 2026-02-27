using RauViet.classes;
using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Color = System.Drawing.Color;

namespace RauViet.ui
{
    public partial class KhoVatTu_CultivationProcessTemplate : Form
    {
        System.Data.DataTable mMaterial_dt, mWorkType_dt, mProduct_dt, mCultivationType_dt, mCultivationProcessTemplate_dt, mBaseDateType_dt;
    //    private DataView mLogDV;
        private Timer WorkTypeDebounceTimer = new Timer { Interval = 300 };
        private Timer MaterialDebounceTimer = new Timer { Interval = 300 };
        private Timer PlantDebounceTimer = new Timer { Interval = 300 };
        bool isNewState = false;
        private LoadingOverlay loadingOverlay;
        public KhoVatTu_CultivationProcessTemplate()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            plantGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            plantGV.MultiSelect = false;

            status_lb.Text = "";

            newCustomerBtn.Click += newBtn_Click;
            LuuThayDoiBtn.Click += saveBtn_Click;
            edit_btn.Click += Edit_btn_Click;
            readOnly_btn.Click += ReadOnly_btn_Click; 
            this.KeyDown += form_KeyDown;


            WorkTypeDebounceTimer.Tick += WokTypeDebounceTimer_Tick;
            congViec_CBB.TextUpdate += CongViec_CBB_TextUpdate;

            MaterialDebounceTimer.Tick += MaterialDebounceTimer_Tick;
            vatTu_CB.TextUpdate += VatTu_CB_TextUpdate;


            PlantDebounceTimer.Tick += PlantDebounceTimer_Tick;
            sku_CBB.TextUpdate += Sku_CBB_TextUpdate;
            materialQuantity_tb.KeyPress += Tb_KeyPress_OnlyNumber;
            plantGV.SelectionChanged += PlantGV_SelectionChanged;
        }

        private void form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                SQLStore_KhoVatTu.Instance.RemoveCultivationProcessTemplate();
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
                var cultivationProcessTemplateTask = SQLStore_KhoVatTu.Instance.GetCultivationProcessTemplateAsync();
                var materialTask = SQLStore_KhoVatTu.Instance.getMaterialAsync();
                var workTypeTask = SQLStore_KhoVatTu.Instance.GetWorkTypeAsync();
                var productTask = SQLStore_Kho.Instance.getProductSKUAsync();
                var CultivationTypeTask = SQLStore_KhoVatTu.Instance.GetCultivationTypeAsync();

                await Task.WhenAll(cultivationProcessTemplateTask, materialTask, workTypeTask, CultivationTypeTask, productTask);
                mCultivationProcessTemplate_dt = cultivationProcessTemplateTask.Result;
                mMaterial_dt = materialTask.Result;
                mWorkType_dt = workTypeTask.Result;
                mProduct_dt = productTask.Result;
                mCultivationType_dt = CultivationTypeTask.Result;
                mBaseDateType_dt = SQLStore_KhoVatTu.Instance.GetBaseDateType();
                // mLogDV = new DataView(logDataTask.Result);

                plantGV.DataSource = getPlantData();

                vatTu_CB.DataSource = mMaterial_dt;
                vatTu_CB.DisplayMember = "MaterialName";  // hiển thị tên
                vatTu_CB.ValueMember = "MaterialID";
                vatTu_CB.AutoCompleteMode = AutoCompleteMode.SuggestAppend;


                congViec_CBB.DataSource = mWorkType_dt;
                congViec_CBB.DisplayMember = "WorkTypeName";  // hiển thị tên
                congViec_CBB.ValueMember = "WorkTypeID";
                congViec_CBB.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

                fertilizationWork_CBB.DataSource = mWorkType_dt.Copy();
                fertilizationWork_CBB.DisplayMember = "WorkTypeName";  // hiển thị tên
                fertilizationWork_CBB.ValueMember = "WorkTypeID";
                fertilizationWork_CBB.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

                sku_CBB.DataSource = mProduct_dt;
                sku_CBB.DisplayMember = "ProductNameVN";  // hiển thị tên
                sku_CBB.ValueMember = "ProductSKU";
                sku_CBB.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

                cultivationType_CBB.DataSource = mCultivationType_dt;
                cultivationType_CBB.DisplayMember = "CultivationTypeName"; 
                cultivationType_CBB.ValueMember = "CultivationTypeID";

                
                baseDateType_CBB.DataSource = mBaseDateType_dt;
                baseDateType_CBB.DisplayMember = "Text";
                baseDateType_CBB.ValueMember = "Value";


                dataGV.DataSource = mCultivationProcessTemplate_dt;
                //   log_GV.DataSource = mLogDV;

                Utils.HideColumns(plantGV, new[] { "SKU", "CultivationTypeID"});
                Utils.SetGridHeaders(plantGV, new System.Collections.Generic.Dictionary<string, string> {
                        {"ProductNameVN", "Cây Trồng" },
                        {"CultivationTypeName", "Canh Tác" }
                    });

                Utils.HideColumns(dataGV, new[] { "ProcessTemplateID", "SKU", "CultivationTypeID", "BaseDateType", "WorkTypeID", "MaterialID", "FertilizationWorkTypeID" });
                Utils.SetGridOrdinal(mCultivationProcessTemplate_dt, new[] { "ProductNameVN", "CultivationTypeName", "BaseDateTypeName", "DaysAfter", "FertilizationWorkTypeName", "WorkTypeName", "MaterialName", "MaterialQuantity", "WaterAmount" });
                Utils.SetGridHeaders(dataGV, new System.Collections.Generic.Dictionary<string, string> {
                        {"ProductNameVN", "Cây Trồng" },
                        {"CultivationTypeName", "Canh Tác" },
                        {"BaseDateTypeName", "Mốc Thời Gian" },
                        {"DaysAfter", "Cộng Thêm" },
                        {"WorkTypeName", "Công Việc" },
                        {"MaterialName", "Vật Tư" },
                        {"MaterialQuantity", "SL Vật Tư" },
                        {"WaterAmount", "Lượng Nước" },
                        {"FertilizationWorkTypeName", "Hình Thức Bón" }
                    });

                //Utils.HideColumns(log_GV, new[] { "LogID", "ExportDate" });
                //Utils.SetGridHeaders(log_GV, new System.Collections.Generic.Dictionary<string, string> {
                //        {"OldValue", "Cũ" },
                //        {"NewValue", "Mới" },
                //        {"CreatedDate", "Ngày tạo" },
                //        {"ActionBy", "Người tạo" }
                //    });

                Utils.SetGridWidths(dataGV, new System.Collections.Generic.Dictionary<string, int> {
                        {"ProductNameVN", 150 },
                        {"CultivationTypeName", 100 },
                        {"BaseDateTypeName", 100 },
                        {"DaysAfter", 100 },
                        {"WorkTypeName", 100 },
                        {"MaterialName", 100 },
                        {"MaterialQuantity", 100 },
                        {"WaterAmount", 100}
                    });

                

                Utils.SetTabStopRecursive(this, false);
                int countTab = 0;
                sku_CBB.TabIndex = countTab++; sku_CBB.TabStop = true;
                cultivationType_CBB.TabIndex = countTab++; cultivationType_CBB.TabStop = true;
                baseDateType_CBB.TabIndex = countTab++; baseDateType_CBB.TabStop = true;
                daysAfter_tb.TabIndex = countTab++; daysAfter_tb.TabStop = true;
                fertilizationWork_CBB.TabIndex = countTab++; fertilizationWork_CBB.TabStop = true;
                congViec_CBB.TabIndex = countTab++; congViec_CBB.TabStop = true;
                vatTu_CB.TabIndex = countTab++; vatTu_CB.TabStop = true;
                materialQuantity_tb.TabIndex = countTab++; materialQuantity_tb.TabStop = true;
                waterAmount_tb.TabIndex = countTab++; waterAmount_tb.TabStop = true;
                LuuThayDoiBtn.TabIndex = countTab++; LuuThayDoiBtn.TabStop = true;

                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                log_GV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                ReadOnly_btn_Click(null, null);
                dataGV.SelectionChanged += this.dataGV_CellClick;
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
            catch (Exception ex){ 
                Console.WriteLine("ERROR " + ex.ToString());
            }
        }

        private void Sku_CBB_TextUpdate(object sender, EventArgs e)
        {
            PlantDebounceTimer.Stop();
            PlantDebounceTimer.Start();
        }
        private void PlantDebounceTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                PlantDebounceTimer.Stop();

                string typed = sku_CBB.Text ?? "";
                string plain = Utils.RemoveVietnameseSigns(typed).ToLower();

                // Filter bằng LINQ
                var filtered = mProduct_dt.AsEnumerable()
                    .Where(r => r["ProductNameVN_NoSign"].ToString().ToLower()
                    .Contains(plain));

                System.Data.DataTable temp;
                if (filtered.Any())
                    temp = filtered.CopyToDataTable();
                else
                    temp = mMaterial_dt.Clone(); // nếu không có kết quả thì trả về table rỗng

                // Gán lại DataSource
                sku_CBB.DataSource = temp;
                sku_CBB.DisplayMember = "ProductNameVN";  // hiển thị tên
                sku_CBB.ValueMember = "ProductSKU";

                // Giữ lại text người đang gõ
                sku_CBB.DroppedDown = true;
                sku_CBB.Text = typed;
                sku_CBB.SelectionStart = typed.Length;
                sku_CBB.SelectionLength = 0;
            }
            catch { }
        }

        private void PlantGV_SelectionChanged(object sender, EventArgs e)
        {
            if (plantGV.SelectedRows.Count == 0)
                return;
            var cells = plantGV.SelectedRows[0].Cells;
            if (cells == null) return;
            int sku = Convert.ToInt32(cells["SKU"].Value);
            int cultivationTypeID = Convert.ToInt32(cells["CultivationTypeID"].Value);


            DataView dv = mCultivationProcessTemplate_dt.DefaultView;
            dv.RowFilter = $"SKU = {sku} AND CultivationTypeID = {cultivationTypeID}";

        }

        private void dataGV_CellClick(object sender, EventArgs e)
        {
            updateRightUI();            
        }

        private async void updateItem(int processTemplateID, int sku, int cultivationTypeID, string baseDateType, int daysAfter, int? fertilizationWorkTypeID, int workTypeID, int? materialID, decimal materialQuantity, decimal waterAmount)
        {
            DataRow[] workTypeRows = mWorkType_dt.Select($"WorkTypeID = {workTypeID}");
            DataRow[] skuRows = mProduct_dt.Select($"ProductSKU = {sku}");
            DataRow[] cultivationTypeRows = mCultivationType_dt.Select($"CultivationTypeID = {cultivationTypeID}");
            DataRow[] baseDateTypeRows = mBaseDateType_dt.Select($"Value = '{baseDateType}'");
            DataRow[] matiralRows = Array.Empty<DataRow>();
            DataRow[] fertilizationWorkRows = Array.Empty<DataRow>();
            if (materialID.HasValue)
                matiralRows = mMaterial_dt.Select($"MaterialID = {materialID}");

            if (fertilizationWorkTypeID.HasValue)
                fertilizationWorkRows = mWorkType_dt.Select($"WorkTypeID = {fertilizationWorkTypeID}");

            foreach (DataRow row in mCultivationProcessTemplate_dt.Rows)
            {
                int ID = Convert.ToInt32(row["ProcessTemplateID"]);
                if (ID.CompareTo(processTemplateID) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager_KhoVatTu.Instance.updateCultivationProcessTemplateAsync(processTemplateID, sku, cultivationTypeID, baseDateType, daysAfter, fertilizationWorkTypeID, workTypeID, materialID, materialQuantity, waterAmount);

                            if (isScussess == true)
                            {
                                row["SKU"] = sku;
                                row["CultivationTypeID"] = cultivationTypeID;
                                row["BaseDateType"] = baseDateType;
                                row["DaysAfter"] = daysAfter;
                                row["WorkTypeID"] = workTypeID;
                                row["MaterialID"] = materialID.HasValue ? (object)materialID.Value : DBNull.Value;
                                row["FertilizationWorkTypeID"] = fertilizationWorkTypeID.HasValue ? (object)fertilizationWorkTypeID.Value : DBNull.Value;
                                row["MaterialQuantity"] = materialQuantity;
                                row["WaterAmount"] = waterAmount;

                                if (baseDateTypeRows.Length > 0)
                                    row["BaseDateTypeName"] = baseDateTypeRows[0]["Text"].ToString();

                                if (skuRows.Length > 0)
                                    row["ProductNameVN"] = skuRows[0]["ProductNameVN"].ToString();

                                if (cultivationTypeRows != null)
                                    row["CultivationTypeName"] = cultivationTypeRows[0]["CultivationTypeName"].ToString();

                                if (workTypeRows.Length > 0)
                                    row["WorkTypeName"] = workTypeRows[0]["WorkTypeName"].ToString();

                                if (materialID.HasValue && matiralRows.Length > 0)
                                    row["MaterialName"] = matiralRows[0]["MaterialName"].ToString();

                                if (fertilizationWorkTypeID.HasValue && fertilizationWorkRows.Length > 0)
                                    row["FertilizationWorkTypeName"] = fertilizationWorkRows[0]["WorkTypeName"].ToString();

                                plantGV.DataSource = getPlantData();
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
                        finally
                        {
                           // _ = SQLManager_KhoVatTu.Instance.insertMaterialExportLogAsync(ExportDate, oldValue, newValue);
                        }
                    }
                    break;
                }
            }
        }

        private async void createItem(int sku, int cultivationTypeID, string baseDateType, int daysAfter, int? fertilizationWorkTypeID, int workTypeID, int? materialID, decimal materialQuantity, decimal waterAmount)
        {
            DataRow[] workTypeRows =  mWorkType_dt.Select($"WorkTypeID = {workTypeID}");
            DataRow[] skuRows = mProduct_dt.Select($"ProductSKU = {sku}");
            DataRow[] cultivationTypeRows = mCultivationType_dt.Select($"CultivationTypeID = {cultivationTypeID}");
            DataRow[] baseDateTypeRows = mBaseDateType_dt.Select($"Value = '{baseDateType}'");
            DataRow[] matiralRows = Array.Empty<DataRow>();
            DataRow[] fertilizationWorkRows = Array.Empty<DataRow>();

            if (materialID.HasValue)
                matiralRows = mMaterial_dt.Select($"MaterialID = {materialID}");
            
            if (fertilizationWorkTypeID.HasValue)
                fertilizationWorkRows = mWorkType_dt.Select($"WorkTypeID = {fertilizationWorkTypeID}");

            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                string oldValue = "create: ";
                string newValue = "";

                try
                {
                    int newId = await SQLManager_KhoVatTu.Instance.insertCultivationProcessTemplateAsync(sku, cultivationTypeID, baseDateType, daysAfter, fertilizationWorkTypeID, workTypeID, materialID, materialQuantity, waterAmount);
                    if (newId > 0)
                    {
                        DataRow drToAdd = mCultivationProcessTemplate_dt.NewRow();

                        drToAdd["ProcessTemplateID"] = newId;
                        drToAdd["SKU"] = sku;
                        drToAdd["CultivationTypeID"] = cultivationTypeID;
                        drToAdd["BaseDateType"] = baseDateType;
                        drToAdd["DaysAfter"] = daysAfter;
                        drToAdd["WorkTypeID"] = workTypeID;
                        drToAdd["MaterialID"] = materialID.HasValue ? (object)materialID.Value : DBNull.Value;
                        drToAdd["FertilizationWorkTypeID"] = fertilizationWorkTypeID.HasValue ? (object)fertilizationWorkTypeID.Value : DBNull.Value;
                        drToAdd["MaterialQuantity"] = materialQuantity;
                        drToAdd["WaterAmount"] = waterAmount;

                        
                        if (baseDateTypeRows.Length > 0)
                            drToAdd["BaseDateTypeName"] = baseDateTypeRows[0]["Text"].ToString();
                        
                        if (skuRows.Length > 0)
                            drToAdd["ProductNameVN"] = skuRows[0]["ProductNameVN"].ToString();
                        
                        if (cultivationTypeRows != null)
                            drToAdd["CultivationTypeName"] = cultivationTypeRows[0]["CultivationTypeName"].ToString();
                        
                        if (workTypeRows.Length > 0)
                            drToAdd["WorkTypeName"] = workTypeRows[0]["WorkTypeName"].ToString();
                        
                        if (materialID.HasValue && matiralRows.Length > 0)
                            drToAdd["MaterialName"] = matiralRows[0]["MaterialName"].ToString();

                        if (fertilizationWorkTypeID.HasValue && fertilizationWorkRows.Length > 0)
                            drToAdd["FertilizationWorkTypeName"] = fertilizationWorkRows[0]["WorkTypeName"].ToString();

                        mCultivationProcessTemplate_dt.Rows.Add(drToAdd);
                        mCultivationProcessTemplate_dt.AcceptChanges();

                        status_lb.Text = "Thành công";
                        status_lb.ForeColor = Color.Green;
                        oldValue += "Success";

                        plantGV.DataSource = getPlantData();

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
                    //_ = SQLManager_KhoVatTu.Instance.insertMaterialExportLogAsync(ExportDate, oldValue, newValue);
                }
            }
        }
        private void saveBtn_Click(object sender, EventArgs e)
        {
            if (sku_CBB.SelectedValue == null || string.IsNullOrEmpty(sku_CBB.Text.Trim()) ||
                congViec_CBB.SelectedValue == null || string.IsNullOrEmpty(congViec_CBB.Text.Trim()) ||
                string.IsNullOrEmpty(materialQuantity_tb.Text.Trim()) ||
                string.IsNullOrEmpty(waterAmount_tb.Text.Trim()) ||
                string.IsNullOrEmpty(daysAfter_tb.Text.Trim())
                )
                return;

            int sku = Convert.ToInt32(sku_CBB.SelectedValue);
            int cultivationTypeID = Convert.ToInt32(cultivationType_CBB.SelectedValue);
            string baseDateType = baseDateType_CBB.SelectedValue.ToString();
            int daysAfter = Convert.ToInt32(daysAfter_tb.Text);
            int workTypeID = Convert.ToInt32(congViec_CBB.SelectedValue);
            int? fertilizationWorkTypeID = (string.IsNullOrEmpty(fertilizationWork_CBB.Text) || fertilizationWork_CBB.SelectedValue == null || fertilizationWork_CBB.SelectedValue == DBNull.Value) ? (int?)null : Convert.ToInt32(fertilizationWork_CBB.SelectedValue);
            int? materialID = (string.IsNullOrEmpty(vatTu_CB.Text) || vatTu_CB.SelectedValue == null || vatTu_CB.SelectedValue == DBNull.Value) ? (int?)null : Convert.ToInt32(vatTu_CB.SelectedValue);

            decimal materialQuantity = Utils.ParseDecimalSmart(materialQuantity_tb.Text);
            decimal waterAmount = Utils.ParseDecimalSmart(waterAmount_tb.Text);

            if (id_tb.Text.Length != 0)
                updateItem(Convert.ToInt32(id_tb.Text), sku, cultivationTypeID, baseDateType, daysAfter, fertilizationWorkTypeID, workTypeID, materialID, materialQuantity, waterAmount);
            else
                createItem(sku, cultivationTypeID, baseDateType, daysAfter, fertilizationWorkTypeID, workTypeID, materialID, materialQuantity, waterAmount);

        }
        private async void deleteProduct()
        {
            string id = id_tb.Text;

            foreach (DataRow row in mCultivationProcessTemplate_dt.Rows)
            {
                string processTemplateID = row["ProcessTemplateID"].ToString();
                if (processTemplateID.CompareTo(id) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Xóa Nha, Chắc Chắn Chưa!", "Thông Báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager_KhoVatTu.Instance.deletetCultivationProcessTemplateAsync(Convert.ToInt32(id));

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                mCultivationProcessTemplate_dt.Rows.Remove(row);
                                mCultivationProcessTemplate_dt.AcceptChanges();

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
                        finally
                        {
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
            materialQuantity_tb.Text = "";

            info_gb.BackColor = newCustomerBtn.BackColor;
            edit_btn.Visible = false;
            newCustomerBtn.Visible = false;
            readOnly_btn.Visible = true;
            LuuThayDoiBtn.Visible = true;
            isNewState = true;
            LuuThayDoiBtn.Text = "Lưu Mới";
            materialQuantity_tb.Enabled = true;
            vatTu_CB.Enabled = true;
            baseDateType_CBB.Enabled = true;
            congViec_CBB.Enabled = true;
            sku_CBB.Focus();
        }

        private void ReadOnly_btn_Click(object sender, EventArgs e)
        {
            edit_btn.Visible = true;
            newCustomerBtn.Visible = true;
            readOnly_btn.Visible = false;
            LuuThayDoiBtn.Visible = false;
            info_gb.BackColor = Color.DarkGray;
            isNewState = false;
            materialQuantity_tb.Enabled = false;
            vatTu_CB.Enabled = false;
            baseDateType_CBB.Enabled = false;
            congViec_CBB.Enabled = false;
            if (dataGV.SelectedRows.Count > 0)
                updateRightUI();
        }

        private void Edit_btn_Click(object sender, EventArgs e)
        {
            materialQuantity_tb.Enabled = true;
            vatTu_CB.Enabled = true;
            edit_btn.Visible = false;
            newCustomerBtn.Visible = false;
            readOnly_btn.Visible = true;
            LuuThayDoiBtn.Visible = true;
            baseDateType_CBB.Enabled = true;
            congViec_CBB.Enabled = true;
            info_gb.BackColor = edit_btn.BackColor;
            isNewState = false;
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

                    int sku = int.TryParse(cells["SKU"].Value?.ToString(), out int skuTemp) ? skuTemp : -1;
                    int cultivationTypeID = int.TryParse(cells["CultivationTypeID"].Value?.ToString(), out int cultivationType) ? cultivationType : -1;
                    string baseDateType = cells["BaseDateType"].Value.ToString();
                    int daysAfter = Convert.ToInt32(cells["DaysAfter"].Value);
                    int workTypeID = int.TryParse(cells["WorkTypeID"].Value?.ToString(), out int workType) ? workType : -1;
                    int fertilizationWorkTypeID = int.TryParse(cells["FertilizationWorkTypeID"].Value?.ToString(), out int fertilizationWorkType) ? fertilizationWorkType : -1;
                    int materialID = int.TryParse(cells["MaterialID"].Value?.ToString(), out int material) ? material : -1;

                    if (!vatTu_CB.Items.Cast<object>().Any(i => ((DataRowView)i)["MaterialID"].ToString() == materialID.ToString()))
                    {
                        vatTu_CB.DataSource = mMaterial_dt;
                    }

                    if (!congViec_CBB.Items.Cast<object>().Any(i => ((DataRowView)i)["WorkTypeID"].ToString() == workTypeID.ToString()))
                    {
                        congViec_CBB.DataSource = mWorkType_dt;
                    }

                    if (!sku_CBB.Items.Cast<object>().Any(i => ((DataRowView)i)["ProductSKU"].ToString() == sku.ToString()))
                    {
                        sku_CBB.DataSource = mProduct_dt;
                    }

                    id_tb.Text = cells["ProcessTemplateID"].Value.ToString();
                    sku_CBB.SelectedValue = sku;
                    cultivationType_CBB.SelectedValue = cultivationTypeID;
                    baseDateType_CBB.SelectedValue = baseDateType;
                    daysAfter_tb.Text = daysAfter.ToString();
                    congViec_CBB.SelectedValue = workTypeID;
                    fertilizationWork_CBB.SelectedValue = fertilizationWorkType;
                    vatTu_CB.SelectedValue = materialID;
                    materialQuantity_tb.Text = Convert.ToDecimal(cells["MaterialQuantity"].Value).ToString("G29", CultureInfo.InvariantCulture);
                    waterAmount_tb.Text = Convert.ToDecimal(cells["WaterAmount"].Value).ToString("G29", CultureInfo.InvariantCulture);
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

        private DataTable getPlantData()
        {
            DataView dv = new DataView(mCultivationProcessTemplate_dt);

            return dv.ToTable(
                true,
                "SKU",
                "CultivationTypeID",
                "ProductNameVN",
                "CultivationTypeName"
            );
        }

        private void button1_Click(object sender, EventArgs e)
        {
            KhoVatTu_CultivationProcessTemplate_Insert frm = new KhoVatTu_CultivationProcessTemplate_Insert();
            frm.ShowDialog(); // hoặc Show()
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
