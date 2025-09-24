using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using RauViet.classes;

namespace RauViet.ui
{
    public partial class Do417 : Form
    {
        DataTable mExportCode_dt, mOrdersTotal_dt;
        public Do417()
        {
            InitializeComponent();

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            status_lb.Text = "";
            loading_lb.Text = "Đang tải dữ liệu, vui lòng chờ...";
            loading_lb.Visible = false;


            LuuThayDoiBtn.Click += saveBtn_Click;           
            dataGV.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.dataGV_RowPrePaint);
            dataGV.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dataGV_EditingControlShowing);
            dataGV.CellFormatting += dataGV_CellFormatting;
            dataGV.CellValueChanged += dataGV_CellValueChanged;
            dataGV.KeyDown += dataGV_KeyDown;
            //dataGV.CellEndEdit += dataGV_CellEndEdit;

            exportCode_cbb.SelectedIndexChanged += exportCode_search_cbb_SelectedIndexChanged;
        }

        

        public async void ShowData()
        {
            

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            loading_lb.Visible = true;            

            try
            {
                var ordersPackingTask = SQLManager.Instance.getOrdersTotalAsync();
                var exportCodeTask = SQLManager.Instance.getExportCodes_Incomplete();

                await Task.WhenAll(ordersPackingTask, exportCodeTask);

                mExportCode_dt = exportCodeTask.Result;
                mOrdersTotal_dt = ordersPackingTask.Result;

                mOrdersTotal_dt.Columns.Add(new DataColumn("STT", typeof(string)));
                mOrdersTotal_dt.Columns.Add(new DataColumn("NWRegistration", typeof(string)));
                mOrdersTotal_dt.Columns.Add(new DataColumn("NWDifference", typeof(decimal)));

                int count = 1;
                foreach (DataRow dr in mOrdersTotal_dt.Rows)
                {
                    decimal nwReal, nwFinal,nwOrder;

                    // thử ép kiểu, nếu không thành công thì bỏ qua
                    bool isNWRealValid = decimal.TryParse(dr["TotalNWReal"]?.ToString(), out nwReal);
                    bool isNWFinalValid = decimal.TryParse(dr["NetWeightFinal"]?.ToString(), out nwFinal);
                    bool isNWOrderValid = decimal.TryParse(dr["TotalNWOther"]?.ToString(), out nwOrder);

                    if (isNWRealValid && isNWFinalValid) dr["NWDifference"] = nwReal - nwFinal;
                    else dr["NWDifference"] = DBNull.Value;

                    if (isNWOrderValid) dr["NWRegistration"] = nwOrder * Convert.ToDecimal(1.1);
                    else dr["NWDifference"] = DBNull.Value;

                    dr["STT"] = count++;

                }

                mOrdersTotal_dt.Columns["STT"].SetOrdinal(0);
                mOrdersTotal_dt.Columns["ProductNameVN"].SetOrdinal(1);
                mOrdersTotal_dt.Columns["NWRegistration"].SetOrdinal(2);
                mOrdersTotal_dt.Columns["TotalNWOther"].SetOrdinal(3);
                mOrdersTotal_dt.Columns["NetWeightFinal"].SetOrdinal(4);
                mOrdersTotal_dt.Columns["TotalNWReal"].SetOrdinal(5);
                mOrdersTotal_dt.Columns["NWDifference"].SetOrdinal(6);
                dataGV.DataSource = mOrdersTotal_dt;

                dataGV.Columns["ProductPackingID"].Visible = false;
                dataGV.Columns["ExportCodeID"].Visible = false;

                dataGV.ReadOnly = false;
                dataGV.Columns["NetWeightFinal"].ReadOnly = false;

                dataGV.Columns["ProductNameVN"].ReadOnly = true;
                dataGV.Columns["ProductPackingID"].ReadOnly = true;
                dataGV.Columns["Priority"].ReadOnly = true;
                dataGV.Columns["NWDifference"].ReadOnly = true;

                dataGV.Columns["ProductNameVN"].HeaderText = "Tên Sản Phẩm";
                dataGV.Columns["NWRegistration"].HeaderText = "N.W\nđkkd";
                dataGV.Columns["TotalNWOther"].HeaderText = "N.W\nđặt hàng";
                dataGV.Columns["NetWeightFinal"].HeaderText = "N.W\nchốt";
                dataGV.Columns["TotalNWReal"].HeaderText = "N.W\nđóng thùng";
                dataGV.Columns["NWDifference"].HeaderText = "Ch.lệch Phyto và đ.thùng";
                dataGV.Columns["Priority"].HeaderText = "Ưu Tiên";

                dataGV.Columns["Priority"].Width = 30;
                dataGV.Columns["STT"].Width = 30;
                // dataGV.Columns["STT"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGV.Columns["NWRegistration"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGV.Columns["TotalNWOther"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGV.Columns["NetWeightFinal"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGV.Columns["TotalNWReal"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGV.Columns["NWDifference"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGV.Columns["ProductNameVN"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                dataGV.Columns["Priority"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dataGV.Columns["TotalNWOther"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGV.Columns["NetWeightFinal"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGV.Columns["TotalNWReal"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGV.Columns["NWDifference"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGV.Columns["NWRegistration"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                exportCode_cbb.DataSource = mExportCode_dt;
                exportCode_cbb.DisplayMember = "ExportCode";  // hiển thị tên
                exportCode_cbb.ValueMember = "ExportCodeID";
                
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

        private void exportCode_search_cbb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mOrdersTotal_dt == null || mExportCode_dt.Rows.Count == 0)
                return;

            string selectedExportCode = ((DataRowView)exportCode_cbb.SelectedItem)["ExportCodeID"].ToString();

            if (!string.IsNullOrEmpty(selectedExportCode))
            {
                // Tạo DataView để filter
                DataView dv = new DataView(mOrdersTotal_dt);
                dv.RowFilter = $"ExportCodeID = '{selectedExportCode}'";

                // Gán lại cho DataGridView
                dataGV.DataSource = dv;
            }
            else
            {
                // Nếu chưa chọn gì thì hiển thị toàn bộ
                dataGV.DataSource = mOrdersTotal_dt;
            }
        }

        private void dataGV_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                var columnName = dataGV.Columns[e.ColumnIndex].Name;

                if (columnName == "NetWeightFinal")
                {
                    updateNWDifference(dataGV.Rows[e.RowIndex]);
                }
            }
        }

        private void updateNWDifference(DataGridViewRow row)
        {
            object nwFinalObj = row.Cells["NetWeightFinal"].Value;
            object nwRealObj = row.Cells["TotalNWReal"].Value;

            decimal nwFinal = 0m, nwReal = 0m; // khởi tạo

            bool isNWFinalValid = nwFinalObj != null && decimal.TryParse(nwFinalObj.ToString(), out nwFinal);
            bool isNWRealValid = nwRealObj != null && decimal.TryParse(nwRealObj.ToString(), out nwReal);

            if (isNWFinalValid && isNWRealValid)
            {
                row.Cells["NWDifference"].Value = nwReal - nwFinal;
            }
            else
            {
                row.Cells["NWDifference"].Value = DBNull.Value;
            }

        }



        private void dataGV_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGV.Columns[e.ColumnIndex].Name == "NetWeightFinal")
            {
                e.CellStyle.BackColor = Color.LightGray;
            }
        }
        private void dataGV_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (e.RowIndex % 2 == 0)
            {
                dataGV.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Beige;
            }
        }        

        private void dataGV_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            TextBox tb = e.Control as TextBox;
            if (tb == null) return;

            // Gỡ toàn bộ trước để tránh add trùng
            tb.KeyPress -= Tb_KeyPress_OnlyNumber;

            var colName = dataGV.CurrentCell.OwningColumn.Name;

            if (colName == "NetWeightFinal")
            {
                tb.KeyPress += Tb_KeyPress_OnlyNumber;
            }
        }

        private void Tb_KeyPress_OnlyNumber(object sender, KeyPressEventArgs e)
        {
            TextBox tb = sender as TextBox;

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


        private void dataGV_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // chặn xuống dòng mặc định

                int row = dataGV.CurrentCell.RowIndex;
                int col = dataGV.CurrentCell.ColumnIndex;

                // Nếu chưa phải dòng cuối thì nhảy xuống
                if (row < dataGV.Rows.Count - 1)
                {
                    dataGV.CurrentCell = dataGV.Rows[row + 1].Cells[col];
                    dataGV.BeginEdit(true); // mở chế độ nhập luôn
                }
            }
        }

        private async void saveBtn_Click(object sender, EventArgs e)
        {
            // Giả sử datagridview là dgvOrdersTotal
            var list = new List<(int ExportCodeID, int ProductPackingID, decimal? NetWeightFinal)>();

            foreach (DataGridViewRow row in dataGV.Rows)
            {
                // Bỏ qua row mới hoặc null
                if (row.IsNewRow) continue;

                try
                {
                    int exportCodeID = Convert.ToInt32(row.Cells["ExportCodeID"].Value);
                    int productPackingID = Convert.ToInt32(row.Cells["ProductPackingID"].Value);

                    // NetWeightFinal có thể null
                    decimal? netWeightFinal = null;
                    var nwValue = row.Cells["NetWeightFinal"].Value;
                    if (nwValue != null && nwValue != DBNull.Value)
                    {
                        netWeightFinal = Convert.ToDecimal(nwValue);
                    }

                    list.Add((exportCodeID, productPackingID, netWeightFinal));
                }
                catch
                {
                    // Nếu row có dữ liệu sai định dạng, bỏ qua
                    continue;
                }
            }

            // Gọi hàm upsert
            bool result = await SQLManager.Instance.UpsertOrdersTotalListAsync(list);

            if (result)
            {
                MessageBox.Show("Cập nhật thành công!");
            }
            else
            {
                MessageBox.Show("Cập nhật thất bại!");
            }
        }

    }
}
