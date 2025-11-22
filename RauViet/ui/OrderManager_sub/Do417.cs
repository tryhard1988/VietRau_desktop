using RauViet.classes;
using System;
using System.Collections.Generic;
using System.Data;

using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RauViet.ui
{
    public partial class Do417 : Form
    {
        DataTable mExportCode_dt, mOrdersTotal_dt;
        DataView mDo47Log_dv;
        private LoadingOverlay loadingOverlay;
        int mCurrentExportID = -1;
        object oldValue;
        public Do417()
        {
            InitializeComponent();
            this.KeyPreview = true;

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            status_lb.Text = "";

            dataGV.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dataGV_EditingControlShowing);
            dataGV.CellFormatting += dataGV_CellFormatting;
            dataGV .CellEndEdit += DataGV_CellValueChanged;
            dataGV.KeyDown += dataGV_KeyDown;
            //dataGV.CellEndEdit += dataGV_CellEndEdit;

            dataGV.CellBeginEdit += dataGV_CellBeginEdit;
            this.KeyDown += Do417_KeyDown; ;
            dataGV.SelectionChanged += DataGV_SelectionChanged; ;
        }


        private void Do417_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                if (mCurrentExportID <= 0)
                {
                    return;
                } 

                SQLStore.Instance.removeOrdersTotal(mCurrentExportID);
                ShowData();
            }
        }

        public async void ShowData()
        {
            await Task.Delay(50);
            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Show();
            await Task.Delay(50);

            try
            {
                string[] keepColumns = { "ExportCodeID", "ExportCode", "InputByName_NoSign" };
                var parameters = new Dictionary<string, object> { { "Complete", false } };
                mExportCode_dt = await SQLStore.Instance.getExportCodesAsync(keepColumns, parameters);

                if (mCurrentExportID <= 0 && mExportCode_dt.Rows.Count > 0)
                {
                    mCurrentExportID = Convert.ToInt32(mExportCode_dt.AsEnumerable()
                                   .Max(r => r.Field<int>("ExportCodeID")));
                }

                var ordersTotaltask = SQLStore.Instance.getOrdersTotalAsync(mCurrentExportID);
                var do47LogTask = SQLStore.Instance.GetDo47LogAsync(mCurrentExportID);
                await Task.WhenAll(ordersTotaltask, do47LogTask);

                mOrdersTotal_dt = ordersTotaltask.Result;
                mDo47Log_dv = new DataView(do47LogTask.Result);
                logGV.DataSource = mDo47Log_dv;

                DataView dv = new DataView(mOrdersTotal_dt);
                dv.RowFilter = $"ExportCodeID = {mCurrentExportID}";
                dataGV.DataSource = dv;

                dataGV.Columns["ProductPackingID"].Visible = false;
                dataGV.Columns["ExportCodeID"].Visible = false;
                dataGV.Columns["Package"].Visible = false;
                dataGV.Columns["Amount"].Visible = false;
                dataGV.Columns["packing"].Visible = false;
                dataGV.Columns["ExportCode"].Visible = false;
                dataGV.Columns["SKU"].Visible = false;
                logGV.Columns["LogID"].Visible = false;
                logGV.Columns["ExportCodeID"].Visible = false;
                logGV.Columns["ProductPackingID"].Visible = false;

                dataGV.ReadOnly = false;
                dataGV.Columns["NetWeightFinal"].ReadOnly = false;

                dataGV.Columns["NWRegistration"].ReadOnly = true;
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

                exportCode_cbb.SelectedIndexChanged -= exportCode_search_cbb_SelectedIndexChanged;
                exportCode_cbb.DataSource = mExportCode_dt;
                exportCode_cbb.DisplayMember = "ExportCode";  // hiển thị tên
                exportCode_cbb.ValueMember = "ExportCodeID";                
                exportCode_cbb.SelectedValue = mCurrentExportID;
                exportCode_cbb.SelectedIndexChanged += exportCode_search_cbb_SelectedIndexChanged;
                

                calvalueRightUI();
            }
            catch (Exception ex)
            {
                status_lb.Text = "Thất bại.";
                status_lb.ForeColor = System.Drawing.Color.Red;
            }
            finally
            {
                await Task.Delay(200);
                loadingOverlay.Hide();
            }
        }

        private async void exportCode_search_cbb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!int.TryParse(exportCode_cbb.SelectedValue.ToString(), out int exportCodeId))
                return;

            mCurrentExportID = exportCodeId;

            var ordersTotaltask = SQLStore.Instance.getOrdersTotalAsync(mCurrentExportID);
            var do47LogTask = SQLStore.Instance.GetDo47LogAsync(mCurrentExportID);
            await Task.WhenAll(ordersTotaltask, do47LogTask);

            mDo47Log_dv = new DataView(do47LogTask.Result);
            logGV.DataSource = mDo47Log_dv;

            mOrdersTotal_dt = ordersTotaltask.Result;
            DataView dv = new DataView(mOrdersTotal_dt);
            dv.RowFilter = $"ExportCodeID = {exportCodeId}";
            dataGV.DataSource = dv;

            calvalueRightUI();

            DataRowView dataR = (DataRowView)exportCode_cbb.SelectedItem;
            string staff = dataR["InputByName_NoSign"].ToString();

            dataGV.ReadOnly = !(UserManager.Instance.fullName_NoSign.CompareTo(staff) == 0);            
        }

        private async void DataGV_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGV.CurrentRow != null)
            {
                object newValue = dataGV.CurrentCell?.Value;//.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString() ?? "";

                if (object.Equals(oldValue, newValue)) return;

                var columnName = dataGV.Columns[e.ColumnIndex].Name;

                if (columnName == "NetWeightFinal")
                {
                    updateNWDifference(dataGV.CurrentRow);
                }

                calvalueRightUI();

                var row = dataGV.CurrentRow;
                var list = new List<(int ExportCodeID, int ProductPackingID, decimal? NetWeightFinal)>();
                int exportCodeID = Convert.ToInt32(row.Cells["ExportCodeID"].Value);
                int productPackingID = Convert.ToInt32(row.Cells["ProductPackingID"].Value);
                decimal nwOrder = Convert.ToDecimal(row.Cells["TotalNWOther"].Value);
                decimal nwReal = decimal.TryParse(row.Cells["TotalNWReal"].Value?.ToString(), out var tmp) ? tmp : 0m;


                decimal? netWeightFinal = null;
                var nwValue = row.Cells["NetWeightFinal"].Value;
                if (nwValue != null && nwValue != DBNull.Value)
                    netWeightFinal = Convert.ToDecimal(nwValue);

                list.Add((exportCodeID, productPackingID, netWeightFinal));
                try
                {
                    // Gọi hàm upsert
                    bool result = await SQLManager.Instance.UpsertOrdersTotalListAsync(list);

                    if (result)
                    {
                        status_lb.Text = "Thành công.";
                        status_lb.ForeColor = System.Drawing.Color.Green;
                        _ = SQLManager.Instance.InsertDo47LogAsync(exportCodeID, productPackingID, oldValue + " Update Thành Công", nwOrder, netWeightFinal, nwReal);
                    }
                    else
                    {
                        _ = SQLManager.Instance.InsertDo47LogAsync(exportCodeID, productPackingID, oldValue + "Update Thất Bại", nwOrder, netWeightFinal, nwReal);
                        MessageBox.Show("Cập nhật thất bại!");
                    }
                }
                catch
                {
                    MessageBox.Show("Cập nhật thất bại!");
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
                e.CellStyle.BackColor = System.Drawing.Color.LightGray;
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


        private void calvalueRightUI()
        {
            decimal sumNWRegistration = 0;
            decimal sumTotalNWOther = 0;
            decimal sumNetWeightFinal = 0;
            decimal sumTotalNWReal = 0;
            decimal sumNWDifference = 0;

            foreach (DataGridViewRow row in dataGV.Rows)
            {
                if (row.IsNewRow) continue;

                sumNWRegistration += ToDecimalSafe(row.Cells["NWRegistration"].Value);
                sumTotalNWOther += ToDecimalSafe(row.Cells["TotalNWOther"].Value);
                sumNetWeightFinal += ToDecimalSafe(row.Cells["NetWeightFinal"].Value);
                sumTotalNWReal += ToDecimalSafe(row.Cells["TotalNWReal"].Value);
                sumNWDifference += ToDecimalSafe(row.Cells["NWDifference"].Value);
            }

            nwdkkd_tb.Text = sumNWRegistration.ToString("N2");
            nwdh_tb.Text = sumTotalNWOther.ToString("N2");
            nwc_tb.Text = sumNetWeightFinal.ToString("N2");
            nwdt_tb.Text = sumTotalNWReal.ToString("N2");
            clpvdt_tb.Text = sumNWDifference.ToString("N2");
        }

        private decimal ToDecimalSafe(object value)
        {
            if (value == null || value == DBNull.Value) return 0;
            if (decimal.TryParse(value.ToString(), out decimal result))
                return result;
            return 0;
        }

        private void dataGV_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            oldValue = dataGV.CurrentCell?.Value;
            if (exportCode_cbb.SelectedItem != null)
            {
                DataRowView dataR = (DataRowView)exportCode_cbb.SelectedItem;
                string staff = dataR["InputByName_NoSign"].ToString();
                if (UserManager.Instance.fullName_NoSign.CompareTo(staff) != 0)
                {
                    e.Cancel = true;
                    return;
                }
            }
        }


        private void DataGV_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGV.CurrentRow == null) return;

            if (exportCode_cbb.SelectedValue == null) return;
            if (!int.TryParse(exportCode_cbb.SelectedValue.ToString(), out int exportCodeId))
            {
                return;
            }

            var currentRow = dataGV.CurrentRow;
            int packingID = Convert.ToInt32(currentRow.Cells["ProductPackingID"].Value);

            mDo47Log_dv.RowFilter = $"ProductPackingID = {packingID}";
            mDo47Log_dv.Sort = "LogID DESC";
        }
    }
}
