using DocumentFormat.OpenXml.Bibliography;
using RauViet.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Color = System.Drawing.Color;

namespace RauViet.ui
{
    public partial class Holidays : Form
    { 
        public Holidays()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            Utils.SetTabStopRecursive(this, false);
            int countTab = 0;
            holidayDateStart_dtp.TabIndex = countTab++; holidayDateStart_dtp.TabStop = true;
            linkStartEnd_cb.TabIndex = countTab++; linkStartEnd_cb.TabStop = true;
            holidayDateEnd_dtp.TabIndex = countTab++; holidayDateEnd_dtp.TabStop = true;
            holidayName_tb.TabIndex = countTab++; holidayName_tb.TabStop = true;
            saveBtn.TabIndex = countTab++; saveBtn.TabStop = true;


            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            status_lb.Text = "";
            delete_btn.Enabled = false;

            holidayDateStart_dtp.Format = DateTimePickerFormat.Custom;
            holidayDateStart_dtp.CustomFormat = "dd/MM/yyyy";

            holidayDateEnd_dtp.Format = DateTimePickerFormat.Custom;
            holidayDateEnd_dtp.CustomFormat = "dd/MM/yyyy";

            saveBtn.Click += newCustomerBtn_Click;
            delete_btn.Click += deleteBtn_Click;
            dataGV.SelectionChanged += this.dataGV_CellClick;

            holidayDateStart_dtp.ValueChanged += HolidayDateStart_dtp_ValueChanged;
            holidayDateEnd_dtp.ValueChanged += HolidayDateEnd_dtp_ValueChanged;

            newBtn.Click += NewBtn_Click;
            readOnly_btn.Click += ReadOnly_btn_Click;
            ReadOnly_btn_Click(null, null);
            linkStartEnd_cb.CheckedChanged += LinkStartEnd_cb_CheckedChanged;

            this.KeyDown += Holidays_KeyDown;
        }

        private void Holidays_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                SQLStore_QLNS.Instance.removeHoliday();
                ShowData();
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
                // Chạy truy vấn trên thread riêng
                var holidayAsync = SQLStore_QLNS.Instance.GetHolidaysAsync();

                await Task.WhenAll(holidayAsync);
                DataTable holiday_dt = holidayAsync.Result;

                dataGV.DataSource = holiday_dt;
                Utils.SetGridHeaders(dataGV, new System.Collections.Generic.Dictionary<string, string> {
                    {"HolidayDate", "Ngày Nghỉ Lễ" },
                    {"HolidayName", "Diễn Giải" }
                });

                dataGV.Columns["HolidayDate"].DefaultCellStyle.Format = "dd/MM/yyyy";

                dataGV.Columns["HolidayDate"].Width = 150;
                dataGV.Columns["HolidayName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                if (dataGV.Rows.Count > 0)
                {
                    dataGV.ClearSelection();
                    dataGV.Rows[0].Selected = true;
                    UpdateRightUI(0);
                }

                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
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
      
        private void dataGV_CellClick(object sender, EventArgs e)
        {
            if (dataGV.CurrentRow == null) return;
            int rowIndex = dataGV.CurrentRow.Index;
            if (rowIndex < 0)
                return;


            UpdateRightUI(rowIndex);
        }

        private void UpdateRightUI(int index)
        {
            var cells = dataGV.Rows[index].Cells;
            DateTime holidayDate = Convert.ToDateTime(cells["HolidayDate"].Value);
            string holidayName = cells["HolidayName"].Value.ToString();

            holidayDateStart_dtp.Value = holidayDate.Date;
            holidayDateEnd_dtp.Value = holidayDate.Date;
            holidayName_tb.Text = holidayName;

            delete_btn.Enabled = true;

            info_gb.BackColor = Color.DarkGray;
            status_lb.Text = "";
        }

        private async Task createNew(DateTime holidayDateStart, DateTime holidayDateEnd, string holidayName)
        {
            for (DateTime date = holidayDateStart.Date; date <= holidayDateEnd.Date; date = date.AddDays(1))
            {
                bool isLock = await SQLStore_QLNS.Instance.IsSalaryLockAsync(date.Month, date.Year);
                if (isLock)
                {
                    MessageBox.Show("Tháng/Năm Đã Bị Khóa", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult != DialogResult.Yes)
                return;
            
            DataTable dataTable = (DataTable)dataGV.DataSource;
            bool hasError = false;
            dataGV.SuspendLayout();

            List<DateTime> datetimeErrorList = new List<DateTime>();
            string errorMessage = "";
 
            
            for (DateTime date = holidayDateStart.Date; date <= holidayDateEnd.Date; date = date.AddDays(1))
            {
                try
                {

                    bool isSuccess = await SQLManager_QLNS.Instance.insertHolidayAsync(date, holidayName);
                    if (isSuccess)
                    {

                            
                        DataRow drToAdd = dataTable.NewRow();

                        drToAdd["HolidayDate"] = date.Date;
                        drToAdd["HolidayName"] = holidayName;
                        dataTable.Rows.Add(drToAdd); 
                    }
                    else
                    {
                        hasError = true;
                        datetimeErrorList.Add(date.Date);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR: " + ex.Message);
                    hasError = true;
                    errorMessage = ex.Message;
                }
            }

            dataGV.ResumeLayout();
            dataTable.AcceptChanges();
            SQLStore_QLNS.Instance.removeLeaveAttendances(holidayDateStart.Year);
            SQLStore_QLNS.Instance.removeLeaveAttendances(holidayDateEnd.Year);

            if (!hasError)
            {
                status_lb.Text =  "Thành công.";
                status_lb.ForeColor = Color.Green;
            }
            else
            {
                if (datetimeErrorList.Count > 0)
                {
                    string result = string.Join(", ", datetimeErrorList.Select(d => d.ToString("dd/MM/yyyy")));
                    MessageBox.Show("Lỗi: " + result);
                }
                else
                {
                    MessageBox.Show(errorMessage);
                }
            }
            
            
        }
        private async void deleteBtn_Click(object sender, EventArgs e)
        {
            if (dataGV.SelectedRows.Count == 0) return;

            DateTime date = holidayDateStart_dtp.Value;
            DataTable dataTable = (DataTable)dataGV.DataSource;
            foreach (DataGridViewRow row in dataGV.Rows)
            {
                DateTime holidayDate = Convert.ToDateTime(row.Cells["HolidayDate"].Value);
                bool isLock = await SQLStore_QLNS.Instance.IsSalaryLockAsync(holidayDate.Month, holidayDate.Year);
                if (isLock)
                {
                    MessageBox.Show(holidayDate.Date + " đã bị khóa.", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (holidayDate.Date.CompareTo(date.Date) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("XÓA ĐÓ NHA \n Chắc chắn chưa ?", " Xóa Thông Tin", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager_QLNS.Instance.DeleteHolidayAsync(date);

                            if (isScussess == true)
                            {
                                SQLStore_QLNS.Instance.removeLeaveAttendances(holidayDate.Year);

                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                int delRowInd = row.Index;
                                dataGV.Rows.Remove(row);

                                SQLStore_QLNS.Instance.removeLeaveAttendances(holidayDate.Year);
                                SQLStore_QLNS.Instance.removeAttendamce(holidayDate.Month, holidayDate.Year);
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
            dataTable.AcceptChanges();
           
        }

        private async void newCustomerBtn_Click(object sender, EventArgs e)
        {
            DateTime holidayDateStart = holidayDateStart_dtp.Value;
            DateTime holidayDateEnd = holidayDateEnd_dtp.Value;
            string holidayName = holidayName_tb.Text;

            await createNew(holidayDateStart, holidayDateEnd, holidayName);
            SQLStore_QLNS.Instance.removeAttendamce(holidayDateStart.Month, holidayDateStart.Year);
            SQLStore_QLNS.Instance.removeAttendamce(holidayDateEnd.Month, holidayDateEnd.Year);
            SQLStore_QLNS.Instance.removeLeaveAttendances(holidayDateStart.Year);

            ReadOnly_btn_Click(null, null);
        }

        private async void HolidayDateStart_dtp_ValueChanged(object sender, EventArgs e)
        {
            DateTime holidayDateStart = holidayDateStart_dtp.Value.Date;
            if (linkStartEnd_cb.Checked)
            {
                holidayDateEnd_dtp.Value = holidayDateStart;
            }
            else
            {
                DateTime holidayDateEnd = holidayDateEnd_dtp.Value.Date;
                if (holidayDateStart > holidayDateEnd)
                    holidayDateEnd_dtp.Value = holidayDateStart;
            }

            if (newBtn.Visible == true)
            {
                bool isLock = await SQLStore_QLNS.Instance.IsSalaryLockAsync(holidayDateStart.Month, holidayDateStart.Year);
                delete_btn.Visible = !isLock;
            }
        }

        private void HolidayDateEnd_dtp_ValueChanged(object sender, EventArgs e)
        {
            DateTime holidayDateStart = holidayDateStart_dtp.Value.Date;
            DateTime holidayDateEnd = holidayDateEnd_dtp.Value.Date;
            if (holidayDateStart > holidayDateEnd)
                holidayDateEnd_dtp.Value = holidayDateStart;

        }

        private void NewBtn_Click(object sender, EventArgs e)
        {
            info_gb.BackColor = newBtn.BackColor;
            newBtn.Visible = false;
            delete_btn.Visible = false;
            readOnly_btn.Visible = true;
            saveBtn.Text = "Lưu Mới";
            saveBtn.Visible = true;
            SetUIReadOnly(false);
        }

        private void ReadOnly_btn_Click(object sender, EventArgs e)
        {
            newBtn.Visible = true;
            delete_btn.Visible = true;
            readOnly_btn.Visible = false;
            saveBtn.Visible = false;
            info_gb.BackColor = Color.DarkGray;
            SetUIReadOnly(true);
            dataGV_CellClick(null, null);
        }

        private void SetUIReadOnly(bool isReadOnly)
        {
            holidayDateStart_dtp.Enabled = !isReadOnly;
            holidayDateEnd_dtp.Visible = !isReadOnly;
            label3.Visible = !isReadOnly;
            holidayName_tb.Enabled = !isReadOnly;
            linkStartEnd_cb.Visible = !isReadOnly;

            if (!isReadOnly)
            {
                LinkStartEnd_cb_CheckedChanged(null, null);
            }
        }

        private void LinkStartEnd_cb_CheckedChanged(object sender, EventArgs e)
        {
            holidayDateEnd_dtp.Visible = !linkStartEnd_cb.Checked;
            label3.Visible = !linkStartEnd_cb.Checked;

            if (linkStartEnd_cb.Checked)
            {
                DateTime dayStart = holidayDateStart_dtp.Value.Date;
                holidayDateEnd_dtp.Value = dayStart;
            }
        }

    }
}
