using RauViet.classes;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataTable = System.Data.DataTable;

namespace RauViet.ui
{
    public partial class ThongKeTonKhoHangRauCu : Form
    {
        private Timer debounceTimer = new Timer { Interval = 300 };
        DataView mLogDV;
        bool isNewState = false;
        private LoadingOverlay loadingOverlay;
        public ThongKeTonKhoHangRauCu()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            dataGV.CellFormatting += dataGV_CellFormatting;
            dataGV.SelectionChanged += this.dataGV_CellClick;

            this.KeyDown += ProductList_KeyDown;

        }

        private void ProductList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                SQLStore_Kho.Instance.removeVegetableWarehouseTransaction();
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
                var logTask = SQLStore_Kho.Instance.getVegetableWarehouseTransactionLOGAsync();
                var vegetableWarehouseTransactionTask = SQLStore_Kho.Instance.getVegetableWarehouseTransactionSync();

                await Task.WhenAll(logTask, vegetableWarehouseTransactionTask);
                mLogDV = new DataView(logTask.Result);

                int currentYear = DateTime.Now.Year;
                var result = vegetableWarehouseTransactionTask.Result
                            .AsEnumerable()
                            .GroupBy(r => r.Field<int>("SKU"))
                            .Select(g =>
                            {
                                var firstRow = g.First();
                                // Tồn đầu năm
                                var tonDauNam =
                                                g.Where(r => r.Field<string>("TransactionType") == "N" && r.Field<DateTime>("TransactionDate").Year < currentYear).Sum(r => r.Field<decimal>("Quantity"))
                                              -
                                                g.Where(r => r.Field<string>("TransactionType") != "N" && r.Field<DateTime>("TransactionDate").Year < currentYear).Sum(r => r.Field<decimal>("Quantity"));

                                // Nhập hàng năm hiện tại
                                var nhapHang_VR = g.Where(r => r.Field<string>("TransactionType") == "N" && r.Field<string>("Supplier") == "VR" && r.Field<DateTime>("TransactionDate").Year == currentYear).Sum(r => r.Field<decimal>("Quantity"));
                                var nhapHang_BT = g.Where(r => r.Field<string>("TransactionType") == "N" && r.Field<string>("Supplier") == "BT" && r.Field<DateTime>("TransactionDate").Year == currentYear).Sum(r => r.Field<decimal>("Quantity"));
                                var nhapHang_MN = g.Where(r => r.Field<string>("TransactionType") == "N" && r.Field<string>("Supplier") == "MN" && r.Field<DateTime>("TransactionDate").Year == currentYear).Sum(r => r.Field<decimal>("Quantity"));

                                var xuatDongGoi_VR = g.Where(r => r.Field<string>("TransactionType") == "X" && r.Field<string>("Supplier") == "VR" && r.Field<DateTime>("TransactionDate").Year == currentYear).Sum(r => r.Field<decimal>("Quantity"));
                                var xuatDongGoi_BT = g.Where(r => r.Field<string>("TransactionType") == "X" && r.Field<string>("Supplier") == "BT" && r.Field<DateTime>("TransactionDate").Year == currentYear).Sum(r => r.Field<decimal>("Quantity"));
                                var xuatDongGoi_MN = g.Where(r => r.Field<string>("TransactionType") == "X" && r.Field<string>("Supplier") == "MN" && r.Field<DateTime>("TransactionDate").Year == currentYear).Sum(r => r.Field<decimal>("Quantity"));

                                var xuatHuy_VR = g.Where(r => r.Field<string>("TransactionType") == "H" && r.Field<string>("Supplier") == "VR" && r.Field<DateTime>("TransactionDate").Year == currentYear).Sum(r => r.Field<decimal>("Quantity"));
                                var xuatHuy_BT = g.Where(r => r.Field<string>("TransactionType") == "H" && r.Field<string>("Supplier") == "BT" && r.Field<DateTime>("TransactionDate").Year == currentYear).Sum(r => r.Field<decimal>("Quantity"));
                                var xuatHuy_MN = g.Where(r => r.Field<string>("TransactionType") == "H" && r.Field<string>("Supplier") == "MN" && r.Field<DateTime>("TransactionDate").Year == currentYear).Sum(r => r.Field<decimal>("Quantity"));

                                var xuatPhoi_VR = g.Where(r => r.Field<string>("TransactionType") == "P" && r.Field<string>("Supplier") == "VR" && r.Field<DateTime>("TransactionDate").Year == currentYear).Sum(r => r.Field<decimal>("Quantity"));
                                var xuatPhoi_BT = g.Where(r => r.Field<string>("TransactionType") == "P" && r.Field<string>("Supplier") == "BT" && r.Field<DateTime>("TransactionDate").Year == currentYear).Sum(r => r.Field<decimal>("Quantity"));
                                var xuatPhoi_MN = g.Where(r => r.Field<string>("TransactionType") == "P" && r.Field<string>("Supplier") == "MN" && r.Field<DateTime>("TransactionDate").Year == currentYear).Sum(r => r.Field<decimal>("Quantity"));

                                var xuatKhac_VR = g.Where(r => r.Field<string>("TransactionType") == "K" && r.Field<string>("Supplier") == "VR" && r.Field<DateTime>("TransactionDate").Year == currentYear).Sum(r => r.Field<decimal>("Quantity"));
                                var xuatKhac_BT = g.Where(r => r.Field<string>("TransactionType") == "K" && r.Field<string>("Supplier") == "BT" && r.Field<DateTime>("TransactionDate").Year == currentYear).Sum(r => r.Field<decimal>("Quantity"));
                                var xuatKhac_MN = g.Where(r => r.Field<string>("TransactionType") == "K" && r.Field<string>("Supplier") == "MN" && r.Field<DateTime>("TransactionDate").Year == currentYear).Sum(r => r.Field<decimal>("Quantity"));
                                return new
                                {
                                    SKU = g.Key,
                                    Name_VN = firstRow.Field<string>("Name_VN"),
                                    Package = firstRow.Field<string>("Package"),
                                    TonDauNam = tonDauNam,

                                    nhapHang_VR = nhapHang_VR,
                                    nhapHang_BT = nhapHang_BT,
                                    nhapHang_MN = nhapHang_MN,

                                    xuatDongGoi_VR = xuatDongGoi_VR,
                                    xuatDongGoi_BT = xuatDongGoi_BT,
                                    xuatDongGoi_MN = xuatDongGoi_MN,

                                    xuatHuy_VR = xuatHuy_VR,
                                    xuatHuy_BT = xuatHuy_BT,
                                    xuatHuy_MN = xuatHuy_MN,

                                    xuatPhoi_VR = xuatPhoi_VR,
                                    xuatPhoi_BT = xuatPhoi_BT,
                                    xuatPhoi_MN = xuatPhoi_MN,

                                    xuatKhac_VR = xuatKhac_VR,
                                    xuatKhac_BT = xuatKhac_BT,
                                    xuatKhac_MN = xuatKhac_MN
                                };
                            })
                            .ToList();

                DataTable dtResult = new DataTable();
                dtResult.Columns.Add("SKU", typeof(int));
                dtResult.Columns.Add("Tên Sản Phẩm", typeof(string));
                dtResult.Columns.Add("Đ.Vị", typeof(string));

                dtResult.Columns.Add("Tồn\nđầu năm", typeof(decimal));

                dtResult.Columns.Add("Nhập\n(Việt Rau)", typeof(decimal));
                dtResult.Columns.Add("Nhập\n(Bình Thuận)", typeof(decimal));
                dtResult.Columns.Add("Nhập\n(Mua Ngoài)", typeof(decimal));

                dtResult.Columns.Add("X.Đóng Gói\n(Việt Rau)", typeof(decimal));
                dtResult.Columns.Add("X.Đóng Gói\n(Bình Thuận)", typeof(decimal));
                dtResult.Columns.Add("X.Đóng Gói\n(Mua Ngoài)", typeof(decimal));

                dtResult.Columns.Add("X.Hủy\n(Việt Rau)", typeof(decimal));
                dtResult.Columns.Add("X.Hủy\n(Bình Thuận)", typeof(decimal));
                dtResult.Columns.Add("X.Hủy\n(Mua Ngoài)", typeof(decimal));

                dtResult.Columns.Add("X.Phơi\n(Việt Rau)", typeof(decimal));
                dtResult.Columns.Add("X.Phơi\n(Bình Thuận)", typeof(decimal));
                dtResult.Columns.Add("X.Phơi\n(Mua Ngoài)", typeof(decimal));

                dtResult.Columns.Add("X.Khác\n(Việt Rau)", typeof(decimal));
                dtResult.Columns.Add("X.Khác\n(Bình Thuận)", typeof(decimal));
                dtResult.Columns.Add("X.Khác\n(Mua Ngoài)", typeof(decimal));

                dtResult.Columns.Add("Tồn\nTrong Kho", typeof(decimal));

                foreach (var item in result)
                {
                    dtResult.Rows.Add(
                        item.SKU,
                        item.Name_VN,
                        item.Package,
                        item.TonDauNam,

                        item.nhapHang_VR,  item.nhapHang_BT,  item.nhapHang_MN,
                        item.xuatDongGoi_VR,  item.xuatDongGoi_BT, item.xuatDongGoi_MN,
                        item.xuatHuy_VR,  item.xuatHuy_BT,  item.xuatHuy_BT,
                        item.xuatPhoi_VR,  item.xuatPhoi_BT,  item.xuatPhoi_BT,
                        item.xuatKhac_VR,  item.xuatKhac_BT, item.xuatKhac_BT,

                        (item.TonDauNam + item.nhapHang_VR + item.nhapHang_BT + item.nhapHang_MN)
                        - 
                        (item.xuatDongGoi_VR + item.xuatDongGoi_BT + item.xuatDongGoi_MN + item.xuatHuy_VR + item.xuatHuy_BT + item.xuatHuy_MN + item.xuatPhoi_VR + item.xuatPhoi_BT + item.xuatPhoi_MN + item.xuatKhac_VR + item.xuatKhac_BT + item.xuatKhac_MN)
                    );
                }

                log_GV.DataSource = mLogDV;
                dataGV.DataSource = dtResult;

                Utils.SetGridWidths(dataGV, new System.Collections.Generic.Dictionary<string, int> {
                    {"SKU", 50},
                    {"Tên Sản Phẩm", 120},
                    {"Đ.Vị", 60},
                    {"Tồn\nđầu năm", 80},
                    {"Nhập\n(Việt Rau)", 80},
                    {"Nhập\n(Bình Thuận)", 95},
                    {"Nhập\n(Mua Ngoài)", 90},
                    {"X.Đóng Gói\n(Việt Rau)", 85},
                    {"X.Đóng Gói\n(Bình Thuận)", 95},
                    {"X.Đóng Gói\n(Mua Ngoài)", 90},
                    {"X.Hủy\n(Việt Rau)", 80},
                    {"X.Hủy\n(Bình Thuận)", 95},
                    {"X.Hủy\n(Mua Ngoài)", 90},
                    {"X.Phơi\n(Việt Rau)", 80},
                    {"X.Phơi\n(Bình Thuận)", 95},
                    {"X.Phơi\n(Mua Ngoài)", 90},
                    {"X.Khác\n(Việt Rau)", 80},
                    {"X.Khác\n(Bình Thuận)", 95},
                    {"X.Khác\n(Mua Ngoài)", 90}
                });

                Utils.SetGridWidths(log_GV, new System.Collections.Generic.Dictionary<string, int> {
                    {"LogID", 80},
                    {"ActionBy", 170},
                    {"CreateedAt", 130}
                });

                log_GV.Columns["SKU"].Visible = false;

                Utils.SetGridWidth(log_GV, "OldValue", DataGridViewAutoSizeColumnMode.Fill);
                Utils.SetGridWidth(log_GV, "NewValue", DataGridViewAutoSizeColumnMode.Fill);

                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                log_GV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                Utils.SetTabStopRecursive(this, false);    
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

        private void dataGV_CellClick(object sender, EventArgs e)
        {
            if (dataGV.CurrentRow == null) return;

            int SKU = Convert.ToInt32(dataGV.CurrentRow.Cells["SKU"].Value);

            mLogDV.RowFilter = $"SKU = {SKU}";
        }

        private void dataGV_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGV.CurrentRow == null) return;
            var row = dataGV.Rows[e.RowIndex];
            
            if (dataGV.Columns[e.ColumnIndex].Name == "Tồn Trong Kho")
            {
                if (int.TryParse(row.Cells["Tồn Trong Kho"].Value?.ToString(), out int value) && value < 0)
                {
                    e.CellStyle.BackColor = Color.Red;
                    e.CellStyle.ForeColor = Color.White;

                    // Nền khi đang được chọn
                    e.CellStyle.SelectionBackColor = Color.Red;
                    e.CellStyle.SelectionForeColor = Color.White;

                    // Font đậm cho nổi bật
                    e.CellStyle.Font = new Font(dataGV.Font, FontStyle.Bold);
                }            
            }

        }
    }
}
