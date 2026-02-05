using RauViet.classes;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Color = System.Drawing.Color;

namespace RauViet.ui
{
    public partial class KhoVatTu_Materials : Form
    {
        System.Data.DataTable mMaterial_dt, mUnit_dt, mCatelory_dt;
        private DataView mLogDV;
        private Timer CateloryDebounceTimer = new Timer { Interval = 300 };
        private Timer UnitDebounceTimer = new Timer { Interval = 300 };
        bool isNewState = false;
        private LoadingOverlay loadingOverlay;
        public KhoVatTu_Materials()
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
            CateloryDebounceTimer.Tick += cateloryDebounceTimer_Tick;
            UnitDebounceTimer.Tick += unitDebounceTimer_Tick;
            catelory_CB.TextUpdate += catelory_cbb_TextUpdate;
            unit_CBB.TextUpdate += unit_CBB_TextUpdate;
            search_tb.TextChanged += Search_tb_TextChanged;
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
                var unitTask = SQLStore_KhoVatTu.Instance.GetUnitAsync();
                var cateloryTask = SQLStore_KhoVatTu.Instance.GetMaterialCategoryAsync();
                var materialTask = SQLStore_KhoVatTu.Instance.getMaterialAsync();
                await Task.WhenAll(unitTask, cateloryTask, materialTask);
                mUnit_dt = unitTask.Result;
                mCatelory_dt = cateloryTask.Result;
                mMaterial_dt = materialTask.Result;

                unit_CBB.DataSource = mUnit_dt;
                unit_CBB.DisplayMember = "UnitName";  // hiển thị tên
                unit_CBB.ValueMember = "UnitID";
                unit_CBB.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

                catelory_CB.DataSource = mCatelory_dt;
                catelory_CB.DisplayMember = "CategoryName";  // hiển thị tên
                catelory_CB.ValueMember = "CategoryID";
                catelory_CB.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

                dataGV.DataSource = mMaterial_dt;
                //    log_GV.DataSource = mLogDV;
                Utils.HideColumns(dataGV, new[] { "UnitID", "CategoryID", "MaterialID", "MaterialName_nosign" });
                Utils.SetGridOrdinal(mMaterial_dt, new[] { "MaterialName", "CategoryName", "UnitName" });
                Utils.SetGridHeaders(dataGV, new System.Collections.Generic.Dictionary<string, string> {
                        {"MaterialName", "Tên Vật Tư" },
                        {"CategoryName", "Phân Loại" },
                        {"UnitName", "Đ.Vị" }
                    });

                Utils.SetGridWidths(dataGV, new System.Collections.Generic.Dictionary<string, int> {
                        {"MaterialName", 300},
                        {"CategoryName", 150},
                        {"UnitName",60}
                    });
                                                    
                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                //    updateRightUI();

                Utils.SetTabStopRecursive(this, false);
                int countTab = 0;
                materialName_tb.TabIndex = countTab++; materialName_tb.TabStop = true;
                catelory_CB.TabIndex = countTab++; catelory_CB.TabStop = true;
                unit_CBB.TabIndex = countTab++; unit_CBB.TabStop = true;
                LuuThayDoiBtn.TabIndex = countTab++; LuuThayDoiBtn.TabStop = true;

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

        
        private void catelory_cbb_TextUpdate(object sender, EventArgs e)
        {
            // Restart timer mỗi khi gõ
            CateloryDebounceTimer.Stop();
            CateloryDebounceTimer.Start();
        }

        private void cateloryDebounceTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                CateloryDebounceTimer.Stop();

                string typed = catelory_CB.Text ?? "";
                string plain = Utils.RemoveVietnameseSigns(typed).ToLower();

                // Filter bằng LINQ
                var filtered = mCatelory_dt.AsEnumerable()
                    .Where(r => r["CategoryName_nosign"].ToString().ToLower()
                    .Contains(plain));

                System.Data.DataTable temp;
                if (filtered.Any())
                    temp = filtered.CopyToDataTable();
                else
                    temp = mCatelory_dt.Clone(); // nếu không có kết quả thì trả về table rỗng

                // Gán lại DataSource
                catelory_CB.DataSource = temp;
                catelory_CB.DisplayMember = "CategoryName";
                catelory_CB.ValueMember = "CategoryID";

                // Giữ lại text người đang gõ
                catelory_CB.DroppedDown = true;
                catelory_CB.Text = typed;
                catelory_CB.SelectionStart = typed.Length;
                catelory_CB.SelectionLength = 0;
            }
            catch { }
        }

        private void unit_CBB_TextUpdate(object sender, EventArgs e)
        {
            // Restart timer mỗi khi gõ
            UnitDebounceTimer.Stop();
            UnitDebounceTimer.Start();
        }
        private void unitDebounceTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                UnitDebounceTimer.Stop();

                string typed = unit_CBB.Text ?? "";
                string plain = Utils.RemoveVietnameseSigns(typed).ToLower();

                // Filter bằng LINQ
                var filtered = mUnit_dt.AsEnumerable()
                    .Where(r => r["UnitName_nosign"].ToString().ToLower()
                    .Contains(plain));

                System.Data.DataTable temp;
                if (filtered.Any())
                    temp = filtered.CopyToDataTable();
                else
                    temp = mUnit_dt.Clone(); // nếu không có kết quả thì trả về table rỗng

                // Gán lại DataSource
                unit_CBB.DataSource = temp;
                unit_CBB.DisplayMember = "UnitName";
                unit_CBB.ValueMember = "UnitID";

                // Giữ lại text người đang gõ
                unit_CBB.DroppedDown = true;
                unit_CBB.Text = typed;
                unit_CBB.SelectionStart = typed.Length;
                unit_CBB.SelectionLength = 0;
            }
            catch { }
        }

        private void dataGV_CellClick(object sender, EventArgs e)
        {
            updateRightUI();            
        }

        private async void updateItem(int materialID, string name, int unitID, int cateloryID)
        {
            DataRow[] unitRows = mUnit_dt.Select($"UnitID = {unitID}");
            DataRow[] cateloryRows = mCatelory_dt.Select($"CategoryID = {cateloryID}");
            if (unitRows.Length <= 0 || cateloryRows.Length <= 0) return;

            foreach (DataRow row in mMaterial_dt.Rows)
            {
                int ID = Convert.ToInt32(row["MaterialID"]);
                if (ID.CompareTo(materialID) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager_KhoVatTu.Instance.updateMaterialsAsync(materialID, name, cateloryID, unitID);

                            if (isScussess == true)
                            {
                                string UnitName = unitRows[0]["UnitName"].ToString();
                                string CategoryName = cateloryRows[0]["CategoryName"].ToString();

                                row["MaterialName"] = name;
                                row["CategoryID"] = cateloryID;
                                row["UnitID"] = unitID;
                                row["UnitName"] = UnitName;
                                row["CategoryName"] = CategoryName;
                                row["MaterialName_nosign"] = Utils.RemoveVietnameseSigns($"{CategoryName} {name}");
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

        private async void createItem(string name, int unitID, int cateloryID)
        {
            DataRow[] unitRows = mUnit_dt.Select($"UnitID = {unitID}");
            DataRow[] cateloryRows = mCatelory_dt.Select($"CategoryID = {cateloryID}");
            if (unitRows.Length <= 0 || cateloryRows.Length <= 0) return;

            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    int newId = await SQLManager_KhoVatTu.Instance.insertMaterialsAsync(name, cateloryID, unitID);
                    if (newId > 0)
                    {
                        DataRow drToAdd = mMaterial_dt.NewRow();
                        string UnitName = unitRows[0]["UnitName"].ToString();
                        string CategoryName = cateloryRows[0]["CategoryName"].ToString();

                        drToAdd["MaterialID"] = newId;
                        drToAdd["MaterialName"] = name;
                        drToAdd["CategoryID"] = cateloryID;
                        drToAdd["UnitID"] = unitID;
                        drToAdd["UnitName"] = UnitName;
                        drToAdd["CategoryName"] = CategoryName;
                        drToAdd["MaterialName_nosign"] = Utils.RemoveVietnameseSigns($"{CategoryName} {name}");

                        mMaterial_dt.Rows.Add(drToAdd);
                        mMaterial_dt.AcceptChanges();

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
            if (unit_CBB.SelectedValue == null || catelory_CB.SelectedValue == null || string.IsNullOrEmpty(materialName_tb.Text.Trim())) return;

            string name = materialName_tb.Text;
            int unitID = Convert.ToInt32(unit_CBB.SelectedValue);
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
            unit_CBB.Enabled = true;
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
            unit_CBB.Enabled = false;
            catelory_CB.Enabled = false;
            
            if (dataGV.SelectedRows.Count > 0)
                updateRightUI();
        }

        private void Edit_btn_Click(object sender, EventArgs e)
        {
            materialName_tb.Enabled = true;
            unit_CBB.Enabled = true;
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
            try
            {
                if (isNewState) return;

                if (dataGV.SelectedRows.Count > 0)
                {
                    var cells = dataGV.SelectedRows[0].Cells;

                    int UnitID = Convert.ToInt32(cells["UnitID"].Value);
                    int CategoryID = Convert.ToInt32(cells["CategoryID"].Value);
                    string materialName = cells["MaterialName"].Value.ToString();

                    if (!unit_CBB.Items.Cast<object>().Any(i => ((DataRowView)i)["UnitID"].ToString() == UnitID.ToString()))
                    {
                        unit_CBB.DataSource = mUnit_dt;
                    }                    

                    if (!catelory_CB.Items.Cast<object>().Any(i => ((DataRowView)i)["CategoryID"].ToString() == CategoryID.ToString()))
                    {
                        catelory_CB.DataSource = mCatelory_dt;
                    }

                    id_tb.Text = cells["MaterialID"].Value.ToString();
                    unit_CBB.SelectedValue = UnitID;
                    catelory_CB.SelectedValue = CategoryID;
                    materialName_tb.Text = materialName;


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
            }
            ;
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

        private void Search_tb_TextChanged(object sender, EventArgs e)
        {
            string keyword = Utils.RemoveVietnameseSigns(search_tb.Text.Trim().ToLower())
                     .Replace("'", "''"); // tránh lỗi cú pháp '

            DataTable dt = dataGV.DataSource as DataTable;
            if (dt == null) return;

            DataView dv = dt.DefaultView;
            dv.RowFilter = $"[MaterialName_nosign] LIKE '%{keyword}%'";
        }


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
        //            System.Data.DataTable excelData = Utils.LoadExcel_NoHeader(filePath);

        //            excelData = excelData.AsEnumerable()
        //                                .Where(r => !string.IsNullOrWhiteSpace(r["Column1"]?.ToString()))
        //                                .Select(r => r["Column1"].ToString().Trim())   // chuẩn hoá
        //                                .Distinct(StringComparer.OrdinalIgnoreCase)    // bỏ trùng, ignore case
        //                                .OrderBy(x => x)                               // sort A-Z
        //                                .Select(x =>
        //                                {
        //                                    var row = excelData.NewRow();
        //                                    row["Column1"] = x;
        //                                    return row;
        //                                })
        //                                .CopyToDataTable();
        //            try
        //            {

        //                foreach (DataRow edr in excelData.Rows)
        //                {
        //                    string name = edr["Column1"].ToString();
        //                    if (string.IsNullOrWhiteSpace(name.Trim())) continue;

        //                    int salaryInfoID = await SQLManager_KhoVatTu.Instance.insertCropAsync(name);
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                MessageBox.Show("có lỗi " + ex.Message);
        //            }
        //            MessageBox.Show("xong");
        //        }
        //    }
        //}

    }
}
