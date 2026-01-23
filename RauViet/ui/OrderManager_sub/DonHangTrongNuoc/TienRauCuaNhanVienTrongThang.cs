using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using RauViet.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using Color = System.Drawing.Color;

namespace RauViet.ui
{
    public partial class TienRauCuaNhanVienTrongThang : Form
    {
        System.Data.DataTable mTongHopBanThanhLy_dt;
        private LoadingOverlay loadingOverlay;
        private Timer _monthYearDebounceTimer;
        public TienRauCuaNhanVienTrongThang()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            monthYear_dtp.Format = DateTimePickerFormat.Custom;
            monthYear_dtp.CustomFormat = "MM/yyyy";
            monthYear_dtp.ShowUpDown = true;

            LuuThayDoiBtn.Click += saveBtn_Click;            

            this.KeyDown += ProductList_KeyDown;
            this.Load += OvertimeAttendace_Load;
        }

        private void OvertimeAttendace_Load(object sender, EventArgs e)
        {
            _monthYearDebounceTimer = new Timer();
            _monthYearDebounceTimer.Interval = 500;
            _monthYearDebounceTimer.Tick += MonthYearDebounceTimer_Tick;
        }

        private void ProductList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                SQLStore_Kho.Instance.removeDomesticLiquidationExport();
                ShowData();
            }           
        }

        public async void ShowData()
        {
            await Task.Delay(50);
            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Show();

            try
            {
                monthYear_dtp.ValueChanged -= monthYearDtp_ValueChanged;

                var domesticLiquidationExportTask = SQLStore_Kho.Instance.getDomesticLiquidationExportAsync();
                var empTask = SQLStore_QLNS.Instance.GetEmployeesAsync();
                await Task.WhenAll(domesticLiquidationExportTask, empTask);

                int month = monthYear_dtp.Value.Month;
                int year = monthYear_dtp.Value.Year;
                var result = domesticLiquidationExportTask.Result.AsEnumerable()
                                                        .Where(r => r.Field<DateTime>("ExportDate").Month == month && r.Field<DateTime>("ExportDate").Year == year && r.Field<Boolean>("IsCanceled") == false)
                                                        .GroupBy(r => r.Field<int>("EmployeeBuyID"))
                                                        .Select(g => new
                                                        {
                                                            EmployeeBuyID = g.Key,
                                                            EmployeeBuy = g.First()["EmployeeBuy"].ToString(),   // lấy tên NV
                                                            ExportDate = Convert.ToDateTime(g.First()["ExportDate"]),
                                                            TotalMoney = g.Sum(x => Convert.ToInt32(x["TotalMoney"]))
                                                        }).ToList();

                mTongHopBanThanhLy_dt = new DataTable();
                mTongHopBanThanhLy_dt.Columns.Add("EmployeeID", typeof(int));
                mTongHopBanThanhLy_dt.Columns.Add("ExportDate", typeof(DateTime));
                mTongHopBanThanhLy_dt.Columns.Add("EmployeeCode", typeof(string));
                mTongHopBanThanhLy_dt.Columns.Add("EmployeeName", typeof(string));                
                mTongHopBanThanhLy_dt.Columns.Add("TotalMoney", typeof(int));

                foreach (var row in result)
                {
                    DataRow empRow = empTask.Result.AsEnumerable().FirstOrDefault(r => r.Field<int>("EmployeeID") == row.EmployeeBuyID);
                    if(empRow != null)
                        mTongHopBanThanhLy_dt.Rows.Add(row.EmployeeBuyID, row.ExportDate, empRow["EmployeeCode"].ToString(), row.EmployeeBuy, row.TotalMoney);
                }

                dataGV.DataSource = mTongHopBanThanhLy_dt;
                Utils.HideColumns(dataGV, new[] { "EmployeeID" });

                Utils.SetGridHeaders(dataGV, new System.Collections.Generic.Dictionary<string, string> {
                    {"ExportDate", "Ngày Mua" },
                    {"EmployeeCode", "Mã NV" },
                    {"EmployeeName", "Tên Nhân Viên" },
                    {"TotalMoney", "Số Tiền" },
                });

                Utils.SetGridWidths(dataGV, new System.Collections.Generic.Dictionary<string, int> {
                    {"EmployeeCode", 70},
                    {"EmployeeName", 110},
                    {"TotalMoney", 60},
                });
                                
                dataGV.Columns["ExportDate"].DefaultCellStyle.Format = "MM/yyyy";

                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                monthYear_dtp.ValueChanged += monthYearDtp_ValueChanged;
                await Task.Delay(100);
                loadingOverlay.Hide();
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

        private async void saveBtn_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show($"Sẽ cập nhật dữ liệu mua rau vào tháng {monthYear_dtp.Value.Month}/{monthYear_dtp.Value.Year}?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult != DialogResult.Yes)
                return;

            List<(string employeeCode, string DeductionTypeCode, DateTime deductionDate, int amount, string note)> newData = new List<(string employeeCode, string DeductionTypeCodev, DateTime deductionDate, int amount, string note)>();
            int month = monthYear_dtp.Value.Month;
            int year = monthYear_dtp.Value.Year;

            var isLocked = await SQLStore_QLNS.Instance.IsSalaryLockAsync(month, year);
            if (isLocked)
            {
                MessageBox.Show($"Tháng {month}/{year} Đã Bị Khóa.", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (DataRow edr in mTongHopBanThanhLy_dt.Rows)
            {
                string employeeCode = edr["EmployeeCode"].ToString();
                int amount = Convert.ToInt32(edr["TotalMoney"]);
                DateTime exportDate = Convert.ToDateTime(edr["ExportDate"]);

                if (amount <= 0 || month != exportDate.Month || year != exportDate.Year)
                    continue;

                newData.Add((employeeCode, "VEG", new DateTime(exportDate.Year, exportDate.Month, 15), amount, ""));
            }

            bool isSuccess = await SQLManager_QLNS.Instance.InsertEmployeeDeduction_ListAsync(month, year, "VEG", newData);
            if (isSuccess)
                MessageBox.Show("THÀNH CÔNG", "Thông Tin");
            else
                MessageBox.Show("THẤT BẠI", "Thông Tin");
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
            HandleMonthYearChanged();
        }

        private async void HandleMonthYearChanged()
        {
            ShowData();
        }
    }
}
