﻿using DocumentFormat.OpenXml.Wordprocessing;
using RauViet.classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using Color = System.Drawing.Color;

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

            holidayDateStart_dtp.Format = DateTimePickerFormat.Custom;
            holidayDateStart_dtp.CustomFormat = "dd/MM/yyyy";

            holidayDateEnd_dtp.Format = DateTimePickerFormat.Custom;
            holidayDateEnd_dtp.CustomFormat = "dd/MM/yyyy";

            newCustomerBtn.Click += newCustomerBtn_Click;
            delete_btn.Click += deleteBtn_Click;
            dataGV.SelectionChanged += this.dataGV_CellClick;

            holidayDateStart_dtp.ValueChanged += HolidayDateStart_dtp_ValueChanged;
            holidayDateEnd_dtp.ValueChanged += HolidayDateEnd_dtp_ValueChanged;
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
                bool isLock = await SQLStore.Instance.IsSalaryLockAsync(date.Month, date.Year);
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
            
            for (DateTime date = holidayDateStart.Date; date <= holidayDateEnd.Date; date = date.AddDays(1))
            {
                try
                {

                    bool isSuccess = await SQLManager.Instance.insertHolidayAsync(date, holidayName);
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
                    }
                }
                catch (Exception ex)
                {
                    hasError = true;
                }
            }

            dataGV.ResumeLayout();
            dataTable.AcceptChanges();
            SQLStore.Instance.removeLeaveAttendances(holidayDateStart.Year);
            SQLStore.Instance.removeLeaveAttendances(holidayDateEnd.Year);
            status_lb.Text = hasError ? "Có lỗi trong quá trình thêm." : "Thêm thành công tất cả.";
            status_lb.ForeColor = hasError ? Color.Red : Color.Green;
            
        }
        private async void deleteBtn_Click(object sender, EventArgs e)
        {
            if (dataGV.SelectedRows.Count == 0) return;

            DateTime date = holidayDateStart_dtp.Value;

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

        private async void newCustomerBtn_Click(object sender, EventArgs e)
        {
            DateTime holidayDateStart = holidayDateStart_dtp.Value;
            DateTime holidayDateEnd = holidayDateEnd_dtp.Value;
            string holidayName = holidayName_tb.Text;

            await createNew(holidayDateStart, holidayDateEnd, holidayName);
        }

        private async void HolidayDateStart_dtp_ValueChanged(object sender, EventArgs e)
        {
            DateTime holidayDateStart = holidayDateStart_dtp.Value.Date;
            DateTime holidayDateEnd = holidayDateEnd_dtp.Value.Date;
            if (holidayDateStart > holidayDateEnd)
                holidayDateEnd_dtp.Value = holidayDateStart;

            bool isLock = await SQLStore.Instance.IsSalaryLockAsync(holidayDateStart.Month, holidayDateStart.Year);
            delete_btn.Visible = !isLock;
        }

        private void HolidayDateEnd_dtp_ValueChanged(object sender, EventArgs e)
        {
            DateTime holidayDateStart = holidayDateStart_dtp.Value.Date;
            DateTime holidayDateEnd = holidayDateEnd_dtp.Value.Date;
            if (holidayDateStart > holidayDateEnd)
                holidayDateEnd_dtp.Value = holidayDateStart;

        }
    }
}
