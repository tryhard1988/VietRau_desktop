using DocumentFormat.OpenXml.Bibliography;
using RauViet.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RauViet.ui
{
    public partial class KhoVatTu_GoiYCapPhan : Form
    {
        DataTable mCultivationProcess_dt;
        private LoadingOverlay loadingOverlay;
        DateTime mdate;
        public KhoVatTu_GoiYCapPhan(DateTime date)
        {
            InitializeComponent();
            this.KeyPreview = true;
            mdate = date;

            farm_cbb.SelectedIndexChanged += Farm_cbb_SelectedIndexChanged;
            capPhan_btn.Click += CapPhan_btn_Click;
            quyDoi_btn.Click += QuyDoi_btn_Click;

            xodua_tb.KeyPress += Tb_KeyPress_OnlyNumber_decimal;
            phanbo_tb.KeyPress += Tb_KeyPress_OnlyNumber_decimal;
        }


        public async void ShowData()
        {
            await Task.Delay(50);
            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Show();

            try
            {
                mCultivationProcess_dt = await SQLStore_KhoVatTu.Instance.GetCultivationProcessAsync(mdate, true);
                var materialExport_dt = await SQLStore_KhoVatTu.Instance.GetMaterialExportAsync(mdate.Month, mdate.Year);
                var farm_dt = await SQLStore_KhoVatTu.Instance.GetFarmsAsync();

                var exportSet = new HashSet<string>();
                // tạo key từ materialExport_dt
                foreach (DataRow row in materialExport_dt.Rows)
                {
                    string key = $"{row["PlantingID"]}_{Convert.ToDateTime(row["ExportDate"]).Date}_{row["MaterialID"]}";
                    exportSet.Add(key);
                }

                // lọc lại mCultivationProcess_dt
                var rowsToKeep = mCultivationProcess_dt.AsEnumerable()
                    .Where(row =>
                    {
                        string key = $"{row["PlantingID"]}_{Convert.ToDateTime(row["ProcessDate"]).Date}_{row["MaterialID"]}";
                        return !exportSet.Contains(key);
                    });

                mCultivationProcess_dt = rowsToKeep.Any() ? rowsToKeep.CopyToDataTable() : mCultivationProcess_dt.Clone();

                if (!mCultivationProcess_dt.Columns.Contains("WorkTypeName_Normalized"))
                {
                    mCultivationProcess_dt.Columns.Add("WorkTypeName_Normalized", typeof(string));
                }
                foreach (DataRow row in mCultivationProcess_dt.Rows)
                {
                    string val = row["WorkTypeName"]?.ToString() ?? "";
                    val = Utils.RemoveVietnameseSigns(val).ToLower().Trim();

                    row["WorkTypeName_Normalized"] = val;
                }

                DataView dv = new DataView(mCultivationProcess_dt);
                dv.RowFilter = $@"
                    MaterialID IS NOT NULL 
                    AND MaterialQuantity > 0 
                    AND CategoryCode <> 'DDTP'
                    AND ProcessDate = #{mdate:MM/dd/yyyy}#
                ";

                mCultivationProcess_dt = dv.ToTable(false,
                    "PlantingID",
                    "ProcessDate",
                    "MaterialID",
                    "MaterialName",
                    "MaterialUnit",
                    "MaterialQuantity",
                    "ProductionOrder",
                    "WorkTypeID",
                    "WorkTypeName",
                    "EmployeeCode",
                    "EmployeeName",
                    "FarmID",
                    "Note"
                );

                foreach (DataRow row in mCultivationProcess_dt.Rows)
                {
                    if (row["Note"] != DBNull.Value && row["Note"].ToString().Trim() == "Default Data")
                    {
                        row["Note"] = "";
                    }
                }

                farm_cbb.DataSource = farm_dt;
                farm_cbb.DisplayMember = "FarmName";  // hiển thị tên
                farm_cbb.ValueMember = "FarmID";
                farm_cbb.SelectedValue = 2;
                
                dataGV.DataSource = mCultivationProcess_dt;
                Utils.HideColumns(dataGV, new[] { "MaterialID", "WorkTypeID", "EmployeeCode", "FarmID", "PlantingID" });
                Utils.SetGridHeaders(dataGV, new System.Collections.Generic.Dictionary<string, string> {
                        {"ProcessDate", "Ngày Xuất" },
                        {"MaterialName", "Vật Tư" },
                        {"MaterialQuantity", "Số Lượng" },
                        {"ProductionOrder", "Lệnh SX" },
                        {"WorkTypeName", "Công Việc" },
                        {"EmployeeName", "Người Nhận" },
                        {"Note", "Ghi Chú" }
                    });

                Utils.SetGridWidths(dataGV, new System.Collections.Generic.Dictionary<string, int> {
                        {"ProcessDate", 75 },
                        {"MaterialName", 200 },
                        {"MaterialQuantity", 75 },
                        {"ProductionOrder", 75 },
                        {"WorkTypeName", 150 },
                        {"EmployeeName", 150 },
                        {"Note", 200 }
                    });

                var editableColumns = new List<string>
                {
                    "MaterialQuantity",
                    "Note"
                };

                // Set ReadOnly
                foreach (DataGridViewColumn col in dataGV.Columns)
                {
                    if (editableColumns.Contains(col.Name))
                    {
                        col.ReadOnly = false;
                    }
                    else
                    {
                        col.ReadOnly = true;
                    }
                }

                dataGV.Columns["MaterialQuantity"].DefaultCellStyle.BackColor = Color.LightYellow;
                dataGV.Columns["Note"].DefaultCellStyle.BackColor = Color.LightYellow;

                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                Farm_cbb_SelectedIndexChanged(null, null);
                QuyDoi_btn_Click(null, null);
            }
            catch
            {
            }
            finally
            {
                await Task.Delay(200);
                loadingOverlay.Hide();
            }

        }

        private void Farm_cbb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (farm_cbb.SelectedValue == null || farm_cbb.SelectedValue == DBNull.Value || mCultivationProcess_dt == null)
                return;

            int farmId;
            if (!int.TryParse(farm_cbb.SelectedValue.ToString(), out farmId))
                return; 

            mCultivationProcess_dt.DefaultView.RowFilter = $"FarmID = {farmId}";
        }

        private void QuyDoi_btn_Click(object sender, EventArgs e)
        {
            decimal xodua = Utils.ParseDecimalSmart(xodua_tb.Text);
            decimal phanBo = Utils.ParseDecimalSmart(phanbo_tb.Text);

            foreach (DataRow row in mCultivationProcess_dt.Rows)
            {
                int materialID = Convert.ToInt32(row["MaterialID"]);
                decimal materialQuantity = Convert.ToDecimal(row["MaterialQuantity"]);
                if (materialID == 608) //xơ dừa thô
                {
                    row["MaterialQuantity"] = materialQuantity / xodua;
                }
                else if (materialID == 614) //Phân bò
                {
                    row["MaterialQuantity"] = materialQuantity / phanBo;
                }
            }
        }

        private async void CapPhan_btn_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult != DialogResult.Yes)
                return;

            DataView dv = mCultivationProcess_dt.DefaultView;
            dv.Sort = "ProcessDate ASC";
            DataTable filterResult_dt = dv.ToTable();

            List<(DateTime ExportDate, int MaterialID, decimal Amount, int? PlantingID, int? WorkTypeID, string Receiver, string Note, int MaterialDepartmentID)> list = new List<(DateTime, int, decimal, int?, int?, string, string, int)>();
            foreach (DataRow row in filterResult_dt.Rows)
            {
                DateTime ExportDate = Convert.ToDateTime(row["ProcessDate"]);
                int MaterialID = Convert.ToInt32(row["MaterialID"]);
                decimal Amount = Convert.ToDecimal(row["MaterialQuantity"]);
                int? PlantingID = row["PlantingID"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["PlantingID"]);
                int? WorkTypeID = row["WorkTypeID"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["WorkTypeID"]);
                string Receiver = row["EmployeeCode"] == DBNull.Value ? null : row["EmployeeCode"].ToString();
                string Note = row["Note"] == DBNull.Value ? null : row["Note"].ToString();
                int MaterialDepartmentID = 1;

                list.Add((ExportDate, MaterialID, Amount, PlantingID, WorkTypeID, Receiver, Note, MaterialDepartmentID));
            }

            bool isScussess = await SQLManager_KhoVatTu.Instance.InsertMaterialExportBatchAsync(list);

            if(isScussess)
            {
                this.Close();
            }
        }

        private void Tb_KeyPress_OnlyNumber_decimal(object sender, KeyPressEventArgs e)
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
    }
}
