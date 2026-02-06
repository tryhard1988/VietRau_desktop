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
    public partial class KhoVatTu_MaterialImport : Form
    {
        System.Data.DataTable mMaterial_dt, mMaterialImport_dt;
        private DataView mLogDV;
        private Timer _monthYearDebounceTimer = new Timer { Interval = 500 };
        private Timer MaterialDebounceTimer = new Timer { Interval = 300 };
        bool isNewState = false;
        private LoadingOverlay loadingOverlay;
        public KhoVatTu_MaterialImport()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            monthYear_dtp.Format = DateTimePickerFormat.Custom;
            monthYear_dtp.CustomFormat = "MM/yyyy";
            monthYear_dtp.ShowUpDown = true;
            monthYear_dtp.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            ngayNhap_dtp.Format = DateTimePickerFormat.Custom;
            ngayNhap_dtp.CustomFormat = "dd/MM/yyyy";

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            status_lb.Text = "";

            _monthYearDebounceTimer.Tick += MonthYearDebounceTimer_Tick;
            newCustomerBtn.Click += newBtn_Click;
            LuuThayDoiBtn.Click += saveBtn_Click;
            edit_btn.Click += Edit_btn_Click;
            readOnly_btn.Click += ReadOnly_btn_Click; 
            this.KeyDown += Kho_Materials_KeyDown;

            MaterialDebounceTimer.Tick += MaterialDebounceTimer_Tick;
            vatTu_CB.TextUpdate += VatTu_CB_TextUpdate;
            soLuong_tb.KeyPress += Tb_KeyPress_OnlyNumber;
            giaMua_tb.KeyPress += Tb_KeyPress_OnlyNumber;
        }

        private void Kho_Materials_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                int month = monthYear_dtp.Value.Month;
                int year = monthYear_dtp.Value.Year;
                SQLStore_KhoVatTu.Instance.removeMaterialImport(month, year);
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

                var materialTask = SQLStore_KhoVatTu.Instance.getMaterialAsync();
                var materialImportTask = SQLStore_KhoVatTu.Instance.GetMaterialImportAsync(month, year);
                await Task.WhenAll( materialTask, materialImportTask);

                mMaterial_dt = materialTask.Result;
                mMaterialImport_dt = materialImportTask.Result;

                vatTu_CB.DataSource = mMaterial_dt;
                vatTu_CB.DisplayMember = "MaterialName";  // hiển thị tên
                vatTu_CB.ValueMember = "MaterialID";
                vatTu_CB.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

                dataGV.DataSource = mMaterialImport_dt;
                //    ////    log_GV.DataSource = mLogDV;
                Utils.HideColumns(dataGV, new[] { "ImportID", "MaterialID" });
                Utils.SetGridHeaders(dataGV, new System.Collections.Generic.Dictionary<string, string> {
                            {"ImportDate", "Ngày Nhập" },
                            {"MaterialName", "Vật Tư" },
                            {"UnitName", "Đ.Vị" },
                            {"Amount", "S.Lượng" },
                            {"Price", "Giá Mua" },
                            {"TotalMoney", "Thành Tiền" },
                            {"Note", "Ghi Chú" }
                            
                        });

                Utils.SetGridWidths(dataGV, new System.Collections.Generic.Dictionary<string, int> {
                            {"ImportDate", 70 },
                            {"MaterialName", 230 },
                            {"UnitName", 50 },
                            {"Amount", 70 },
                            {"Price", 80 },
                            {"TotalMoney", 100 },
                            {"Note", 200 }
                        });

                Utils.SetGridFormat_NO(dataGV, "Price");
                Utils.SetGridFormat_NO(dataGV, "TotalMoney");
                Utils.SetGridFormat_Alignment(dataGV, "Price", DataGridViewContentAlignment.MiddleRight);
                Utils.SetGridFormat_Alignment(dataGV, "TotalMoney", DataGridViewContentAlignment.MiddleRight);
                Utils.SetGridFormat_Alignment(dataGV, "Amount", DataGridViewContentAlignment.MiddleRight);

                Utils.SetTabStopRecursive(this, false);
                int countTab = 0;
                ngayNhap_dtp.TabIndex = countTab++; ngayNhap_dtp.TabStop = true;
                vatTu_CB.TabIndex = countTab++; vatTu_CB.TabStop = true;
                soLuong_tb.TabIndex = countTab++; soLuong_tb.TabStop = true;
                giaMua_tb.TabIndex = countTab++; giaMua_tb.TabStop = true;
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

        private async void updateItem(int ImportID, DateTime ImportDate, int MaterialID, Decimal Amount, int? Price, string Note)
        {
            DataRow[] matiralRows = matiralRows = mMaterial_dt.Select($"MaterialID = {MaterialID}");

            foreach (DataRow row in mMaterialImport_dt.Rows)
            {
                int ID = Convert.ToInt32(row["ImportID"]);
                if (ID.CompareTo(ImportID) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager_KhoVatTu.Instance.updateMaterialImportAsync(ImportID, ImportDate, MaterialID, Amount, Price, Note);

                            if (isScussess == true)
                            {
                                row["ImportDate"] = ImportDate;
                                row["MaterialID"] = MaterialID;
                                row["Amount"] = Amount;
                                row["Price"] = Price;
                                row["TotalMoney"] = Convert.ToInt32(Price * Amount);
                                row["Note"] = Note;

                                if (matiralRows.Length > 0)
                                {
                                    row["UnitName"] = matiralRows[0]["UnitName"].ToString();
                                    row["MaterialName"] = matiralRows[0]["MaterialName"].ToString();
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

        private async void createItem(DateTime ImportDate, int MaterialID, Decimal Amount,int? Price, string Note)
        {

            DataRow[] matiralRows = matiralRows = mMaterial_dt.Select($"MaterialID = {MaterialID}");

            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    int newId = await SQLManager_KhoVatTu.Instance.insertMaterialImportAsync(ImportDate, MaterialID, Amount, Price, Note);
                    if (newId > 0)
                    {
                        DataRow drToAdd = mMaterialImport_dt.NewRow();

                        drToAdd["ImportID"] = newId;
                        drToAdd["ImportDate"] = ImportDate;
                        drToAdd["MaterialID"] = MaterialID;
                        drToAdd["Amount"] = Amount;
                        drToAdd["Price"] = Price;
                        drToAdd["TotalMoney"] = Convert.ToInt32(Price * Amount);
                        drToAdd["Note"] = Note;


                        if (matiralRows.Length > 0)
                        {
                            drToAdd["UnitName"] = matiralRows[0]["UnitName"].ToString();
                            drToAdd["MaterialName"] = matiralRows[0]["MaterialName"].ToString();
                        }

                        mMaterialImport_dt.Rows.Add(drToAdd);
                        mMaterialImport_dt.AcceptChanges();

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

            DateTime ngayNhap = ngayNhap_dtp.Value;
            int vatTu = Convert.ToInt32(vatTu_CB.SelectedValue);
            decimal soLuong = Utils.ParseDecimalSmart(soLuong_tb.Text);
            int price = Convert.ToInt32(giaMua_tb.Text);
            string note = note_tb.Text;

            if (id_tb.Text.Length != 0)
                updateItem(Convert.ToInt32(id_tb.Text), ngayNhap, vatTu, soLuong, price, note);
            else
                createItem(ngayNhap, vatTu, soLuong, price, note);

        }
        private async void deleteProduct()
        {
            string id = id_tb.Text;

            foreach (DataRow row in mMaterialImport_dt.Rows)
            {
                string importID = row["ImportID"].ToString();
                if (importID.CompareTo(id) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Xóa Nha, Chắc Chắn Chưa!", "Thông Báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager_KhoVatTu.Instance.deletetMaterialImportAsync(Convert.ToInt32(id));

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                mMaterialImport_dt.Rows.Remove(row);
                                mMaterialImport_dt.AcceptChanges();
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
            vatTu_CB.Enabled = true;
            giaMua_tb.Enabled = true;
            note_tb.Enabled = true;
            ngayNhap_dtp.Enabled = true;
            ngayNhap_dtp.Focus();
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
            vatTu_CB.Enabled = false;
            ngayNhap_dtp.Enabled = false;
            giaMua_tb.Enabled = false;
            note_tb.Enabled = false;

            if (dataGV.SelectedRows.Count > 0)
                updateRightUI();
        }

        private void Edit_btn_Click(object sender, EventArgs e)
        {
            soLuong_tb.Enabled = true;
            vatTu_CB.Enabled = true;
            edit_btn.Visible = false;
            newCustomerBtn.Visible = false;
            readOnly_btn.Visible = true;
            giaMua_tb.Enabled = true;
            note_tb.Enabled = true;
            ngayNhap_dtp.Enabled = true;
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

                    DateTime ngayNhap = Convert.ToDateTime(cells["ImportDate"].Value);
                    int materialID = int.TryParse(cells["MaterialID"].Value?.ToString(), out int materialTemp) ? materialTemp : -1;

                    if (!vatTu_CB.Items.Cast<object>().Any(i => ((DataRowView)i)["MaterialID"].ToString() == materialID.ToString()))
                    {
                        vatTu_CB.DataSource = mMaterial_dt;
                    }

                    id_tb.Text = cells["ImportID"].Value.ToString();
                    ngayNhap_dtp.Value = ngayNhap;
                    vatTu_CB.SelectedValue = materialID;
                    soLuong_tb.Text = Convert.ToDecimal(cells["Amount"].Value).ToString("G29", CultureInfo.InvariantCulture);
                    giaMua_tb.Text = Convert.ToString(cells["Price"].Value);
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

        //            Utils.AddColumnIfNotExists(excelData, "MaterialID", typeof(int));
        //            foreach (DataRow row in excelData.Rows)
        //            {
        //                string raw = Utils.RemoveVietnameseSigns(row["Column2"].ToString());

        //                //if (raw.CompareTo(Utils.RemoveVietnameseSigns("Magnesium sulphate (Green mag)")) == 0)
        //                //    raw = Utils.RemoveVietnameseSigns("BitterMag (MgSO4)");
        //                //else if (raw.CompareTo(Utils.RemoveVietnameseSigns("YaraMila Winner")) == 0)
        //                //    raw = Utils.RemoveVietnameseSigns("YaraMila Winner (15-9-20)");
        //                //else if (raw.CompareTo(Utils.RemoveVietnameseSigns("Solu-K 0-0-51")) == 0)
        //                //    raw = Utils.RemoveVietnameseSigns("Solu-K (K2SO4 Hàn Quốc)");

        //                DataRow material = mMaterial_dt.AsEnumerable().FirstOrDefault(r => Utils.RemoveVietnameseSigns(r.Field<string>("MaterialName")).Trim().ToLower().Equals(raw.Trim().ToLower(), StringComparison.OrdinalIgnoreCase));
        //                if (material != null)
        //                    row["MaterialID"] = material["MaterialID"];
        //                                    }

        //            dataGV.DataSource = excelData;
        //           // dataGV.Columns["Column2"].Visible = false;
        //        }
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

        //            DateTime ImportDate = Convert.ToDateTime(edr["Column1"]);
        //            int MaterialID = Convert.ToInt32(edr["MaterialID"]);
        //            decimal Amount = Convert.ToDecimal(edr["Column3"]);
        //            int? Price = edr["Column4"] != DBNull.Value && int.TryParse(edr["Column4"].ToString(), out var v) ? v : (int?)null;

        //            int salaryInfoID = await SQLManager_KhoVatTu.Instance.insertMaterialImportAsync(ImportDate, MaterialID, Amount, Price, "");
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
