using DocumentFormat.OpenXml.Drawing;
using RauViet.classes;
using System;
using System.Data;
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
        private Timer PlantingManagementDebounceTimer = new Timer { Interval = 300 };
        bool isNewState = false;
        private LoadingOverlay loadingOverlay;
        public KhoVatTu_MaterialExport()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;



            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            status_lb.Text = "";

            newCustomerBtn.Click += newBtn_Click;
            LuuThayDoiBtn.Click += saveBtn_Click;
            edit_btn.Click += Edit_btn_Click;
            readOnly_btn.Click += ReadOnly_btn_Click; 
            this.KeyDown += Kho_Materials_KeyDown;
            PlantingManagementDebounceTimer.Tick += plantingManagementDebounceTimer_Tick;
            plantingManagement_CBB.TextUpdate += plantingManagement_CBB_TextUpdate;
            //quantity_tb.KeyPress += Tb_KeyPress_OnlyNumber;
        }

        private void Kho_Materials_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                SQLStore_KhoVatTu.Instance.removeMaterial();
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
                string[] empKeepColumns = { "EmployeeCode", "FullName", "EmployessName_NoSign" };

                
                var cateloryTask = SQLStore_KhoVatTu.Instance.GetMaterialCategoryAsync();

                var plantingManagementTask = SQLManager_KhoVatTu.Instance.getPlantingManagementAsync(false);
                var materialTask = SQLStore_KhoVatTu.Instance.getMaterialAsync();
                var workTypeTask = SQLStore_KhoVatTu.Instance.GetWorkTypeAsync();
                var employeeTask = SQLStore_QLNS.Instance.GetEmployeesAsync(empKeepColumns);
                var materialExportTask = SQLStore_KhoVatTu.Instance.GetMaterialExportAsync(1,2026);

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


                ////    domesticLiquidationPrice_cbb.DataSource = mDomesticLiquidationPrice_dt;
                ////    domesticLiquidationPrice_cbb.DisplayMember = "Name_VN";  // hiển thị tên
                ////    domesticLiquidationPrice_cbb.ValueMember = "DomesticLiquidationPriceID";
                ////    domesticLiquidationPrice_cbb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

                //unit_CBB.DataSource = mUnit_dt;
                //unit_CBB.DisplayMember = "UnitName";  // hiển thị tên
                //unit_CBB.ValueMember = "UnitID";
                //unit_CBB.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

                //catelory_CB.DataSource = mCatelory_dt;
                //catelory_CB.DisplayMember = "CategoryName";  // hiển thị tên
                //catelory_CB.ValueMember = "CategoryID";
                //catelory_CB.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

                dataGV.DataSource = mMaterialExport_dt;
                ////    log_GV.DataSource = mLogDV;
                //Utils.HideColumns(dataGV, new[] { "UnitID", "CategoryID", "MaterialID" });
                //Utils.SetGridOrdinal(mMaterial_dt, new[] { "MaterialName", "CategoryName", "UnitName" });
                //Utils.SetGridHeaders(dataGV, new System.Collections.Generic.Dictionary<string, string> {
                //        {"MaterialName", "Tên Vật Tư" },
                //        {"CategoryName", "Phân Loại" },
                //        {"UnitName", "Đ.Vị" }
                //    });

                //Utils.SetGridWidths(dataGV, new System.Collections.Generic.Dictionary<string, int> {
                //        {"MaterialName", 300},
                //        {"CategoryName", 150},
                //        {"UnitName",60}
                //    });

                //dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                ////    updateRightUI();

                //Utils.SetTabStopRecursive(this, false);
                //int countTab = 0;
                //materialName_tb.TabIndex = countTab++; materialName_tb.TabStop = true;
                //catelory_CB.TabIndex = countTab++; catelory_CB.TabStop = true;
                //unit_CBB.TabIndex = countTab++; unit_CBB.TabStop = true;
                //LuuThayDoiBtn.TabIndex = countTab++; LuuThayDoiBtn.TabStop = true;

                //ReadOnly_btn_Click(null, null);
                //dataGV.SelectionChanged += this.dataGV_CellClick;

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

                string typed = plantingManagement_CBB.Text ?? "";
                string plain = Utils.RemoveVietnameseSigns(typed).ToLower();

                // Filter bằng LINQ
                var filtered = mPlantingManagement_dt.AsEnumerable()
                    .Where(r => r["UnitName_nosign"].ToString().ToLower()
                    .Contains(plain));

                System.Data.DataTable temp;
                if (filtered.Any())
                    temp = filtered.CopyToDataTable();
                else
                    temp = mPlantingManagement_dt.Clone(); // nếu không có kết quả thì trả về table rỗng

                // Gán lại DataSource
                plantingManagement_CBB.DataSource = temp;
                plantingManagement_CBB.DisplayMember = "UnitName";
                plantingManagement_CBB.ValueMember = "UnitID";

                // Giữ lại text người đang gõ
                plantingManagement_CBB.DroppedDown = true;
                plantingManagement_CBB.Text = typed;
                plantingManagement_CBB.SelectionStart = typed.Length;
                plantingManagement_CBB.SelectionLength = 0;
            }
            catch { }
        }

        private void dataGV_CellClick(object sender, EventArgs e)
        {
            updateRightUI();            
        }

        private async void updateItem(int materialID, string name, int unitID, int cateloryID)
        {
            //DataRow[] unitRows = mPlantingManagement_dt.Select($"UnitID = {unitID}");
            //DataRow[] cateloryRows = mCatelory_dt.Select($"CategoryID = {cateloryID}");
            //if (unitRows.Length <= 0 || cateloryRows.Length <= 0) return;

            //foreach (DataRow row in mMaterial_dt.Rows)
            //{
            //    int ID = Convert.ToInt32(row["MaterialID"]);
            //    if (ID.CompareTo(materialID) == 0)
            //    {
            //        DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            //        if (dialogResult == DialogResult.Yes)
            //        {
            //            try
            //            {
            //                bool isScussess = await SQLManager_KhoVatTu.Instance.updateMaterialsAsync(materialID, name, cateloryID, unitID);

            //                if (isScussess == true)
            //                {
            //                    string UnitName = unitRows[0]["UnitName"].ToString();
            //                    string CategoryName = cateloryRows[0]["CategoryName"].ToString();

            //                    row["MaterialName"] = name;
            //                    row["CategoryID"] = cateloryID;
            //                    row["UnitID"] = unitID;
            //                    row["UnitName"] = UnitName;
            //                    row["CategoryName"] = CategoryName;

            //                    status_lb.Text = "Thành công.";
            //                    status_lb.ForeColor = Color.Green;
            //                }
            //                else
            //                {
            //                    status_lb.Text = "Thất bại.";
            //                    status_lb.ForeColor = Color.Red;
            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                status_lb.Text = "Thất bại.";
            //                status_lb.ForeColor = Color.Red;
            //            }
            //        }
            //        break;
            //    }
            //}
        }

        private async void createItem(string name, int unitID, int cateloryID)
        {
            //DataRow[] unitRows = mPlantingManagement_dt.Select($"UnitID = {unitID}");
            //DataRow[] cateloryRows = mCatelory_dt.Select($"CategoryID = {cateloryID}");
            //if (unitRows.Length <= 0 || cateloryRows.Length <= 0) return;

            //DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            //if (dialogResult == DialogResult.Yes)
            //{
            //    try
            //    {
            //        int newId = await SQLManager_KhoVatTu.Instance.insertMaterialsAsync(name, cateloryID, unitID);
            //        if (newId > 0)
            //        {
            //            DataRow drToAdd = mMaterial_dt.NewRow();
            //            string UnitName = unitRows[0]["UnitName"].ToString();
            //            string CategoryName = cateloryRows[0]["CategoryName"].ToString();

            //            drToAdd["MaterialID"] = newId;
            //            drToAdd["MaterialName"] = name;
            //            drToAdd["CategoryID"] = cateloryID;
            //            drToAdd["UnitID"] = unitID;
            //            drToAdd["UnitName"] = UnitName;
            //            drToAdd["CategoryName"] = CategoryName;

            //            mMaterial_dt.Rows.Add(drToAdd);
            //            mMaterial_dt.AcceptChanges();

            //            status_lb.Text = "Thành công";
            //            status_lb.ForeColor = Color.Green;

            //            newBtn_Click(null, null);
            //        }
            //        else
            //        {
            //            status_lb.Text = "Thất bại";
            //            status_lb.ForeColor = Color.Red;
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine("ERROR: " + ex.Message);
            //        status_lb.Text = "Thất bại.";
            //        status_lb.ForeColor = Color.Red;
            //    }

            //}
        }
        private void saveBtn_Click(object sender, EventArgs e)
        {
            if (plantingManagement_CBB.SelectedValue == null || catelory_CB.SelectedValue == null || string.IsNullOrEmpty(materialName_tb.Text.Trim())) return;

            string name = materialName_tb.Text;
            int unitID = Convert.ToInt32(plantingManagement_CBB.SelectedValue);
            int cateloryID = Convert.ToInt32(catelory_CB.SelectedValue);

            if (id_tb.Text.Length != 0)
                updateItem(Convert.ToInt32(id_tb.Text), name, unitID, cateloryID);
            else
                createItem(name, unitID, cateloryID);

        }
        private async void deleteProduct()
        {
            string id = id_tb.Text;

            foreach (DataRow row in mMaterial_dt.Rows)
            {
                string materialID = row["MaterialID"].ToString();
                if (materialID.CompareTo(id) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Xóa Nha, Chắc Chắn Chưa!", "Thông Báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager_KhoVatTu.Instance.deletetMaterialsAsync(Convert.ToInt32(id));

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                mMaterial_dt.Rows.Remove(row);
                                mMaterial_dt.AcceptChanges();
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
            materialName_tb.Enabled = true;
            plantingManagement_CBB.Enabled = true;
            catelory_CB.Enabled = true;

            materialName_tb.Focus();
        }

        private void ReadOnly_btn_Click(object sender, EventArgs e)
        {
            edit_btn.Visible = true;
            newCustomerBtn.Visible = true;
            readOnly_btn.Visible = false;
            LuuThayDoiBtn.Visible = false;
            info_gb.BackColor = Color.DarkGray;
            isNewState = false;
            materialName_tb.Enabled = false;
            plantingManagement_CBB.Enabled = false;
            catelory_CB.Enabled = false;
            
            if (dataGV.SelectedRows.Count > 0)
                updateRightUI();
        }

        private void Edit_btn_Click(object sender, EventArgs e)
        {
            materialName_tb.Enabled = true;
            plantingManagement_CBB.Enabled = true;
            catelory_CB.Enabled = true;
            edit_btn.Visible = false;
            newCustomerBtn.Visible = false;
            readOnly_btn.Visible = true;
            LuuThayDoiBtn.Visible = true;
            info_gb.BackColor = edit_btn.BackColor;
            isNewState = false;
            LuuThayDoiBtn.Text = "Lưu C.Sửa";
        }

        private void updateRightUI()
        {
            //try
            //{
            //    if (isNewState) return;

            //    if (dataGV.SelectedRows.Count > 0)
            //    {
            //        var cells = dataGV.SelectedRows[0].Cells;

            //        int UnitID = Convert.ToInt32(cells["UnitID"].Value);
            //        int CategoryID = Convert.ToInt32(cells["CategoryID"].Value);
            //        string materialName = cells["MaterialName"].Value.ToString();

            //        if (!plantingManagement_CBB.Items.Cast<object>().Any(i => ((DataRowView)i)["UnitID"].ToString() == UnitID.ToString()))
            //        {
            //            plantingManagement_CBB.DataSource = mPlantingManagement_dt;
            //        }                    

            //        if (!catelory_CB.Items.Cast<object>().Any(i => ((DataRowView)i)["CategoryID"].ToString() == CategoryID.ToString()))
            //        {
            //            catelory_CB.DataSource = mCatelory_dt;
            //        }

            //        id_tb.Text = cells["MaterialID"].Value.ToString();
            //        plantingManagement_CBB.SelectedValue = UnitID;
            //        catelory_CB.SelectedValue = CategoryID;
            //        materialName_tb.Text = materialName;


            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("ERROR: " + ex.Message);
            //}            
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
