using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RauViet.classes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace RauViet.ui
{
    public partial class Holidays : Form
    { 
        public Holidays()
        {
            InitializeComponent();

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            status_lb.Text = "";
            loading_lb.Text = "Đang tải dữ liệu, vui lòng chờ...";
            loading_lb.Visible = false;
            delete_btn.Enabled = false;

            holidayDate_dtp.Format = DateTimePickerFormat.Custom;
            holidayDate_dtp.CustomFormat = "dd/MM/yyyy";

            newCustomerBtn.Click += newCustomerBtn_Click;
            delete_btn.Click += deleteBtn_Click;
            dataGV.SelectionChanged += this.dataGV_CellClick;

            holidayDate_dtp.ValueChanged += HolidayDate_dtp_ValueChanged;
        }


        public async void ShowData()
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            loading_lb.Visible = true;            

            try
            {
                // Chạy truy vấn trên thread riêng
                var holidayAsync = SQLStore.Instance.GetHolidaysAsync();

                await Task.WhenAll(holidayAsync);
                DataTable holiday_dt = holidayAsync.Result;

                dataGV.DataSource = holiday_dt;

                dataGV.Columns["HolidayDate"].HeaderText = "Ngày Nghỉ Lễ";
                dataGV.Columns["HolidayName"].HeaderText = "Diễn Giải";

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
            catch (Exception ex)
            {
                status_lb.Text = "Thất bại.";
                status_lb.ForeColor = Color.Red;
            }
            finally
            {
                loading_lb.Visible = false; // ẩn loading
                loading_lb.Enabled = true; // enable lại button
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

            holidayDate_dtp.Value = holidayDate.Date;
            holidayName_tb.Text = holidayName;

            delete_btn.Enabled = true;

            info_gb.BackColor = Color.DarkGray;
            status_lb.Text = "";
        }

        private async void createNew(DateTime holidayDate, string holidayName)
        {
            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    bool isSuccess = await SQLManager.Instance.insertHolidayAsync(holidayDate, holidayName);
                    if (isSuccess)
                    {

                        DataTable dataTable = (DataTable)dataGV.DataSource;
                        DataRow drToAdd = dataTable.NewRow();

                        drToAdd["HolidayDate"] = holidayDate.Date;
                        drToAdd["HolidayName"] = holidayName;

                        dataTable.Rows.Add(drToAdd);
                        dataTable.AcceptChanges();

                        dataGV.ClearSelection(); // bỏ chọn row cũ
                        int rowIndex = dataGV.Rows.Count - 1;
                        dataGV.Rows[rowIndex].Selected = true;
                        UpdateRightUI(dataGV.Rows.Count - 1);

                        SQLStore.Instance.removeLeaveAttendances(holidayDate.Year);

                        status_lb.Text = "Thành công";
                        status_lb.ForeColor = Color.Green;
                    }
                    else
                    {
                        status_lb.Text = "Thất bại";
                        status_lb.ForeColor = Color.Red;
                    }
                }
                catch (Exception ex)
                {
                    status_lb.Text = "Thất bại.";
                    status_lb.ForeColor = Color.Red;
                }
                
            }
        }
        private async void deleteBtn_Click(object sender, EventArgs e)
        {
            if (dataGV.SelectedRows.Count == 0) return;

            DateTime date = holidayDate_dtp.Value;

            foreach (DataGridViewRow row in dataGV.Rows)
            {
                DateTime holidayDate = Convert.ToDateTime(row.Cells["HolidayDate"].Value);
                if (holidayDate.Date.CompareTo(date.Date) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("XÓA ĐÓ NHA \n Chắc chắn chưa ?", " Xóa Thông Tin", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager.Instance.DeleteHolidayAsync(date);

                            if (isScussess == true)
                            {
                                SQLStore.Instance.removeLeaveAttendances(holidayDate.Year);

                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                int delRowInd = row.Index;
                                dataGV.Rows.Remove(row);
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

        private void newCustomerBtn_Click(object sender, EventArgs e)
        {
            DateTime holidayDate = holidayDate_dtp.Value;
            string holidayName = holidayName_tb.Text;

            createNew(holidayDate, holidayName);
        }

        private async void HolidayDate_dtp_ValueChanged(object sender, EventArgs e)
        {
            DateTime holidayDate = holidayDate_dtp.Value;
            bool isLock = await SQLStore.Instance.IsSalaryLockAsync(holidayDate.Month, holidayDate.Year);
            newCustomerBtn.Visible = !isLock;
            delete_btn.Visible = !isLock;
        }
    }
}
