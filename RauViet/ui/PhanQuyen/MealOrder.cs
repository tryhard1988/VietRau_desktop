
using RauViet.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RauViet.ui
{
    public partial class MealOrder : Form
    {
        DataTable mMealOrder_dt;
        bool isNewState = false;
        public MealOrder()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            orderDate_dtp.Format = DateTimePickerFormat.Custom;
            orderDate_dtp.CustomFormat = "MM/yyyy";

            in_EndDay_dtp.Format = DateTimePickerFormat.Custom;
            orderDate_dtp.CustomFormat = "dd/MM/yyyy";

            in_StartDay_dtp.Format = DateTimePickerFormat.Custom;
            orderDate_dtp.CustomFormat = "dd/MM/yyyy";

            Utils.SetTabStopRecursive(this, false);

            int countTab = 0;
            orderDate_dtp.TabIndex = countTab++; orderDate_dtp.TabStop = true;
            note_tb.TabIndex = countTab++; note_tb.TabStop = true;
            quantity_tb.TabIndex = countTab++; quantity_tb.TabStop = true;
            price_tb.TabIndex = countTab++; price_tb.TabStop = true;
            VAT_tb.TabIndex = countTab++; VAT_tb.TabStop = true;
            isDone_CB.TabIndex = countTab++; isDone_CB.TabStop = true;
            LuuThayDoiBtn.TabIndex = countTab++; LuuThayDoiBtn.TabStop = true;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            status_lb.Text = "";

            newCustomerBtn.Click += newCustomerBtn_Click;
            LuuThayDoiBtn.Click += saveBtn_Click;
            
            this.dataGV.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.dataGV_RowPrePaint);

            edit_btn.Click += Edit_btn_Click;
            readOnly_btn.Click += ReadOnly_btn_Click;

            quantity_tb.KeyPress += Tb_KeyPress_OnlyNumber;
            price_tb.KeyPress += Tb_KeyPress_OnlyNumber;
            VAT_tb.KeyPress += Tb_KeyPress_OnlyNumber;

            this.KeyDown += Department_KeyDown; ;

            xem_btn.Click += Xem_btn_Click;
            in_btn.Click += In_btn_Click;
        }

        private void Department_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                SQLStore_QLNS.Instance.removeMealOrder();
                ShowData();
            }
            else if (!isNewState && !edit_btn.Visible)
            {
                if (e.KeyCode == Keys.Delete)
                {
                    Control ctrl = this.ActiveControl;

                    if (ctrl is TextBox || ctrl is RichTextBox ||
                        (ctrl is DataGridView dgv && dgv.CurrentCell != null && dgv.IsCurrentCellInEditMode))
                    {
                        return; // không xử lý Delete
                    }

                    deleteBtn_Click(null, null);
                }
            }
        }

        public async void ShowData()
        {
            await Task.Delay(50);
            LoadingOverlay loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Show();
            await Task.Delay(200);
            try
            {
                dataGV.SelectionChanged -= this.dataGV_CellClick;
                // Chạy truy vấn trên thread riêng
                var mealOrderTask = SQLStore_QLNS.Instance.GetMealOrderAsync();

                await Task.WhenAll(mealOrderTask);
                mMealOrder_dt = mealOrderTask.Result;

                dataGV.DataSource = mMealOrder_dt;
                Utils.HideColumns(dataGV, new[] { "MealOrdersID" });
                Utils.SetGridOrdinal(mMealOrder_dt, new[] { "OrderDate", "Note", "Quantity", "Price", "VAT", "TotalMoney" });
                Utils.SetGridHeaders(dataGV, new System.Collections.Generic.Dictionary<string, string> {
                    {"OrderDate", "Ngày" },
                    {"Quantity", "Số Lượng" },
                    {"Price", "Đơn Giá" },
                    {"IsPaid", "Đã Thanh Toán" },
                    {"Note", "Diễn Giải" },
                    {"TotalMoney", "Thành Tiền" },
                    {"TotalMoney_VAT", "Thành Tiền (VAT)" }
                });
                Utils.SetGridWidths(dataGV, new Dictionary<string, int> {
                     {"OrderDate", 70 },
                    {"Quantity", 70 },
                    {"Price", 70 },
                    {"IsPaid", 70 },
                    {"Note", 150 },
                    {"VAT", 70 },
                    {"TotalMoney", 80 },
                    {"TotalMoney_VAT", 80 }
                });


                var startRow = mMealOrder_dt.AsEnumerable().Where(r => r.Field<bool>("IsPaid") == false).OrderBy(r => r.Field<DateTime>("OrderDate")).FirstOrDefault();
                var endRow = mMealOrder_dt.AsEnumerable().Where(r => r.Field<bool>("IsPaid") == false).OrderByDescending(r => r.Field<DateTime>("OrderDate")).FirstOrDefault();
                if (startRow != null && endRow != null)
                {
                    in_StartDay_dtp.Value = Convert.ToDateTime(startRow["OrderDate"]);
                    in_EndDay_dtp.Value = Convert.ToDateTime(endRow["OrderDate"]);
                }

                ReadOnly_btn_Click(null, null);

                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGV.SelectionChanged += this.dataGV_CellClick;
            }
            catch
            {
                status_lb.Text = "Thất bại.";
                status_lb.ForeColor = Color.Red;
            }
            finally
            {
                await Task.Delay(100);
                loadingOverlay.Hide();
            }
        }

        private void dataGV_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (e.RowIndex % 2 == 0)
            {
                dataGV.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Beige;
            }
        }
        
        private void dataGV_CellClick(object sender, EventArgs e)
        {
            if (dataGV.CurrentRow == null) return;

            UpdateRightUI();
        }

        private void UpdateRightUI()
        {
            if (isNewState == true || dataGV.CurrentRow == null) return;

            var cells = dataGV.CurrentRow.Cells;

            DateTime OrderDate = Convert.ToDateTime(cells["OrderDate"].Value);
            int Quantity = Convert.ToInt32(cells["Quantity"].Value);
            int Price = Convert.ToInt32(cells["Price"].Value);
            decimal VAT = Convert.ToDecimal(cells["VAT"].Value);
            Boolean IsPaid = Convert.ToBoolean(cells["IsPaid"].Value);

            ID_tb.Text = cells["MealOrdersID"].Value.ToString();
            orderDate_dtp.Value = OrderDate;
            quantity_tb.Text = Quantity.ToString();
            price_tb.Text = Price.ToString();
            VAT_tb.Text = VAT.ToString("G29", CultureInfo.InvariantCulture);
            isDone_CB.Checked = IsPaid;
            note_tb.Text = cells["Note"].Value.ToString();
            
            status_lb.Text = "";
        }

        private async void updateData(int mealOrdersID, DateTime orderDate, string note, int quantity, int price, decimal VAT, bool isPaid)
        {
            foreach (DataRow row in mMealOrder_dt.Rows)
            {
                int id = Convert.ToInt32(row["MealOrdersID"]);
                if (id.CompareTo(mealOrdersID) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager_QLNS.Instance.updateMealOrderAsync(mealOrdersID, orderDate, note, quantity, price, VAT, isPaid);

                            if (isScussess == true)
                            {
                                row["OrderDate"] = orderDate;
                                row["Quantity"] = quantity;
                                row["Price"] = price;
                                row["VAT"] = VAT;
                                row["IsPaid"] = isPaid;
                                row["Note"] = note;

                                row["TotalMoney"] = quantity * price;
                                row["TotalMoney_VAT"] = Convert.ToInt32((quantity * price) * (1 + VAT / 100.0m));

                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;
                            }
                            else
                            {
                                status_lb.Text = "Thất bại.";
                                status_lb.ForeColor = Color.Red;
                            }
                        }
                        catch
                        {
                            status_lb.Text = "Thất bại.";
                            status_lb.ForeColor = Color.Red;
                        }  
                    }
                    break;
                }
            }
        }

        private async void createNew(DateTime orderDate, string note, int quantity, int price, decimal VAT)
        {
            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    int MealOrdersID = await SQLManager_QLNS.Instance.insertMealOrderAsync(orderDate, note, quantity, price, VAT);
                    if (MealOrdersID > 0)
                    {
                        DataTable dataTable = (DataTable)dataGV.DataSource;
                        DataRow drToAdd = dataTable.NewRow();

                        drToAdd["MealOrdersID"] = MealOrdersID;
                        drToAdd["OrderDate"] = orderDate;
                        drToAdd["Quantity"] = quantity;
                        drToAdd["Price"] = price;
                        drToAdd["VAT"] = VAT;
                        drToAdd["IsPaid"] = false;
                        drToAdd["Note"] = note;

                        drToAdd["TotalMoney"] = quantity * price;
                        drToAdd["TotalMoney_VAT"] = Convert.ToInt32((quantity * price) * (1 + VAT/100.0m));

                        ID_tb.Text = MealOrdersID.ToString();


                        dataTable.Rows.Add(drToAdd);
                        dataTable.AcceptChanges();

                        Properties.Settings.Default.GiaXuatCom = price;
                        Properties.Settings.Default.VAT = VAT;
                        Properties.Settings.Default.Save();

                        status_lb.Text = "Thành công";
                        status_lb.ForeColor = Color.Green;
                    }
                    else
                    {
                        status_lb.Text = "Thất bại";
                        status_lb.ForeColor = Color.Red;
                    }
                }
                catch
                {
                    status_lb.Text = "Thất bại.";
                    status_lb.ForeColor = Color.Red;
                }
                
            }
        }
        private void saveBtn_Click(object sender, EventArgs e)
        {
            if (note_tb.Text.CompareTo("") == 0)
            {
                MessageBox.Show(
                                "Thiếu Dữ Liệu, Kiểm Tra Lại!",
                                "Thông Báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning
                            );
                return;
            }

            DateTime date = orderDate_dtp.Value;
            string note = note_tb.Text;
            int quantity = Convert.ToInt32(quantity_tb.Text);
            int price = Convert.ToInt32(price_tb.Text);
            decimal VAT = Utils.ParseDecimalSmart(VAT_tb.Text);
            bool isDone = isDone_CB.Checked;

            if (ID_tb.Text.Length != 0)
                updateData(Convert.ToInt32(ID_tb.Text), date, note, quantity, price, VAT, isDone);
            else
                createNew(date, note, quantity, price, VAT);

        }
        private async void deleteBtn_Click(object sender, EventArgs e)
        {
            if (dataGV.SelectedRows.Count == 0) return;

            string mealOrdersID = ID_tb.Text;

            foreach (DataRow row in mMealOrder_dt.Rows)
            {
                string id = row["MealOrdersID"].ToString();
                if (id.CompareTo(mealOrdersID) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("XÓA \n Chắc chắn chưa ?", " Xóa Thông Tin", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager_QLNS.Instance.deleteMealOrderAsync(Convert.ToInt32(id));

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                mMealOrder_dt.Rows.Remove(row);
                                mMealOrder_dt.AcceptChanges();
                            }
                            else
                            {
                                status_lb.Text = "Thất bại.";
                                status_lb.ForeColor = Color.Red;
                            }
                        }
                        catch
                        {
                            status_lb.Text = "Thất bại.";
                            status_lb.ForeColor = Color.Red;
                        }




                    }
                    break;
                }
            }
        }

        private void newCustomerBtn_Click(object sender, EventArgs e)
        {
            ID_tb.Text = "";
            isDone_CB.Checked = false;
            orderDate_dtp.Value = DateTime.Now;
            status_lb.Text = "";
            VAT_tb.Text = Properties.Settings.Default.VAT.ToString("G29", CultureInfo.InvariantCulture);
            price_tb.Text = Properties.Settings.Default.GiaXuatCom.ToString();

            note_tb.Focus();
            info_gb.BackColor = newCustomerBtn.BackColor;
            edit_btn.Visible = false;
            newCustomerBtn.Visible = false;
            readOnly_btn.Visible = true;
            LuuThayDoiBtn.Visible = true;
            isNewState = true;
            isDone_CB.Visible = false;
            LuuThayDoiBtn.Text = "Lưu Mới";
            SetUIReadOnly(false);
        }

        private void ReadOnly_btn_Click(object sender, EventArgs e)
        {
            edit_btn.Visible = true;
            newCustomerBtn.Visible = true;
            readOnly_btn.Visible = false;
            LuuThayDoiBtn.Visible = false;
            isDone_CB.Visible = true;
            info_gb.BackColor = Color.DarkGray;
            isNewState = false;
            SetUIReadOnly(true);
            dataGV_CellClick(null, null);

        }

        private void Edit_btn_Click(object sender, EventArgs e)
        {
            edit_btn.Visible = false;
            newCustomerBtn.Visible = false;
            readOnly_btn.Visible = true;
            LuuThayDoiBtn.Visible = true;
            info_gb.BackColor = edit_btn.BackColor;
            isNewState = false;
            LuuThayDoiBtn.Text = "Lưu C.Sửa";
            SetUIReadOnly(false);
        }

        private void SetUIReadOnly(bool isReadOnly)
        {
            orderDate_dtp.Enabled = !isReadOnly;
            isDone_CB.Enabled = !isReadOnly;
            note_tb.ReadOnly = isReadOnly;
            quantity_tb.ReadOnly = isReadOnly;
            price_tb.ReadOnly = isReadOnly;
            VAT_tb.ReadOnly = isReadOnly;
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

        private void In_btn_Click(object sender, EventArgs e)
        {
            inBangTheoDoiSuatAn(false);
        }

        private void Xem_btn_Click(object sender, EventArgs e)
        {
            inBangTheoDoiSuatAn(true);
        }

        void inBangTheoDoiSuatAn(bool isPrintPreview)
        {
            TheoDoiSuatAn_Printer printer = new TheoDoiSuatAn_Printer(in_StartDay_dtp.Value, in_EndDay_dtp.Value, mMealOrder_dt);

            if (isPrintPreview)
                printer.PrintPreview(this);
            else
                printer.PrintDirect();
        }
    }
}
