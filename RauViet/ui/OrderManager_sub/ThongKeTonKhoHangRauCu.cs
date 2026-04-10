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

            this.KeyDown += ProductList_KeyDown;
            excel_btn.Click += Excel_btn_Click;
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
                var vegetableWarehouseTransactionTask = SQLStore_Kho.Instance.getVegetableWarehouseTransactionSync();

                await Task.WhenAll(vegetableWarehouseTransactionTask);

                int currentYear = DateTime.Now.Year;
                var result = vegetableWarehouseTransactionTask.Result
                            .AsEnumerable()
                            .GroupBy(r => r.Field<int>("SKU"))
                            .Select(g =>
                            {
                                var firstRow = g.First();

                                Func<DataRow, bool> isVR = r => r.Field<int>("SellerID") == 46;
                                Func<DataRow, bool> isBT = r => r.Field<int>("SellerID") == 47;
                                Func<DataRow, bool> isMN = r => r.Field<int>("SellerID") != 46 && r.Field<int>("SellerID") != 47;

                                // Tồn đầu năm
                                var tonDauNam =
                                                g.Where(r => r.Field<string>("TransactionType") == "N" && r.Field<DateTime>("TransactionDate").Year < currentYear)
                                                 .Sum(r => r.Field<decimal>("Quantity"))
                                              -
                                                g.Where(r => r.Field<string>("TransactionType") != "N" && r.Field<DateTime>("TransactionDate").Year < currentYear)
                                                 .Sum(r => r.Field<decimal>("Quantity"));

                                // Nhập
                                var nhapHang_VR = g.Where(r => r.Field<string>("TransactionType") == "N" && isVR(r) && r.Field<DateTime>("TransactionDate").Year == currentYear).Sum(r => r.Field<decimal>("Quantity"));
                                var nhapHang_BT = g.Where(r => r.Field<string>("TransactionType") == "N" && isBT(r) && r.Field<DateTime>("TransactionDate").Year == currentYear).Sum(r => r.Field<decimal>("Quantity"));
                                var nhapHang_MN = g.Where(r => r.Field<string>("TransactionType") == "N" && isMN(r) && r.Field<DateTime>("TransactionDate").Year == currentYear).Sum(r => r.Field<decimal>("Quantity"));

                                // Xuất đóng gói
                                var xuatDongGoi_VR = g.Where(r => r.Field<string>("TransactionType") == "X" && isVR(r) && r.Field<DateTime>("TransactionDate").Year == currentYear).Sum(r => r.Field<decimal>("Quantity"));
                                var xuatDongGoi_BT = g.Where(r => r.Field<string>("TransactionType") == "X" && isBT(r) && r.Field<DateTime>("TransactionDate").Year == currentYear).Sum(r => r.Field<decimal>("Quantity"));
                                var xuatDongGoi_MN = g.Where(r => r.Field<string>("TransactionType") == "X" && isMN(r) && r.Field<DateTime>("TransactionDate").Year == currentYear).Sum(r => r.Field<decimal>("Quantity"));

                                // Xuất hủy
                                var xuatHuy_VR = g.Where(r => r.Field<string>("TransactionType") == "H" && isVR(r) && r.Field<DateTime>("TransactionDate").Year == currentYear).Sum(r => r.Field<decimal>("Quantity"));
                                var xuatHuy_BT = g.Where(r => r.Field<string>("TransactionType") == "H" && isBT(r) && r.Field<DateTime>("TransactionDate").Year == currentYear).Sum(r => r.Field<decimal>("Quantity"));
                                var xuatHuy_MN = g.Where(r => r.Field<string>("TransactionType") == "H" && isMN(r) && r.Field<DateTime>("TransactionDate").Year == currentYear).Sum(r => r.Field<decimal>("Quantity"));

                                // Xuất phơi
                                var xuatPhoi_VR = g.Where(r => r.Field<string>("TransactionType") == "P" && isVR(r) && r.Field<DateTime>("TransactionDate").Year == currentYear).Sum(r => r.Field<decimal>("Quantity"));
                                var xuatPhoi_BT = g.Where(r => r.Field<string>("TransactionType") == "P" && isBT(r) && r.Field<DateTime>("TransactionDate").Year == currentYear).Sum(r => r.Field<decimal>("Quantity"));
                                var xuatPhoi_MN = g.Where(r => r.Field<string>("TransactionType") == "P" && isMN(r) && r.Field<DateTime>("TransactionDate").Year == currentYear).Sum(r => r.Field<decimal>("Quantity"));

                                // Xuất khác
                                var xuatKhac_VR = g.Where(r => r.Field<string>("TransactionType") == "K" && isVR(r) && r.Field<DateTime>("TransactionDate").Year == currentYear).Sum(r => r.Field<decimal>("Quantity"));
                                var xuatKhac_BT = g.Where(r => r.Field<string>("TransactionType") == "K" && isBT(r) && r.Field<DateTime>("TransactionDate").Year == currentYear).Sum(r => r.Field<decimal>("Quantity"));
                                var xuatKhac_MN = g.Where(r => r.Field<string>("TransactionType") == "K" && isMN(r) && r.Field<DateTime>("TransactionDate").Year == currentYear).Sum(r => r.Field<decimal>("Quantity"));

                                return new
                                {
                                    SKU = g.Key,
                                    Name_VN = firstRow.Field<string>("Name_VN"),
                                    Package = firstRow.Field<string>("unit"),
                                    TonDauNam = tonDauNam,

                                    nhapHang_VR,
                                    nhapHang_BT,
                                    nhapHang_MN,

                                    xuatDongGoi_VR,
                                    xuatDongGoi_BT,
                                    xuatDongGoi_MN,

                                    xuatHuy_VR,
                                    xuatHuy_BT,
                                    xuatHuy_MN,

                                    xuatPhoi_VR,
                                    xuatPhoi_BT,
                                    xuatPhoi_MN,

                                    xuatKhac_VR,
                                    xuatKhac_BT,
                                    xuatKhac_MN
                                };
                            })
                            .ToList();

                DataTable dtResult = new DataTable();
                dtResult.Columns.Add("SKU", typeof(int));
                dtResult.Columns.Add("TenSP", typeof(string));
                dtResult.Columns.Add("DVI", typeof(string));

                dtResult.Columns.Add("TonDau", typeof(decimal));

                dtResult.Columns.Add("NVR", typeof(decimal));
                dtResult.Columns.Add("NBT", typeof(decimal));
                dtResult.Columns.Add("NMN", typeof(decimal));

                dtResult.Columns.Add("XDG_VR", typeof(decimal));
                dtResult.Columns.Add("XDG_BT", typeof(decimal));
                dtResult.Columns.Add("XDG_MN", typeof(decimal));

                dtResult.Columns.Add("XH_VR", typeof(decimal));
                dtResult.Columns.Add("XH_BT", typeof(decimal));
                dtResult.Columns.Add("XH_MN", typeof(decimal));

                dtResult.Columns.Add("XP_VR", typeof(decimal));
                dtResult.Columns.Add("XP_BT", typeof(decimal));
                dtResult.Columns.Add("XP_MN", typeof(decimal));

                dtResult.Columns.Add("XK_VR", typeof(decimal));
                dtResult.Columns.Add("XK_BT", typeof(decimal));
                dtResult.Columns.Add("XK_MN", typeof(decimal));

                dtResult.Columns.Add("TonTrongKho", typeof(decimal));

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

                dataGV.DataSource = dtResult;

                Utils.SetGridHeaders(dataGV, new System.Collections.Generic.Dictionary<string, string> {
                    {"SKU", "SKU"},
                    {"TenSP", "Tên Sản Phẩm"},
                    {"DVI", "Đ.Vị"},
                    {"TonDau", "Tồn\nĐầu Năm"},
                    {"NVR", "Nhập\n(Việt Rau)"},
                    {"NBT", "Nhập\n(Bình Thuận)"},
                    {"NMN", "Nhập\n(Mua Ngoài)"},
                    {"XDG_VR", "X.Đóng Gói\n(Việt Rau)"},
                    {"XDG_BT", "X.Đóng Gói\n(Bình Thuận)"},
                    {"XDG_MN", "X.Đóng Gói\n(Mua Ngoài)"},
                    {"XH_VR", "X.Hủy\n(Việt Rau)"},
                    {"XH_BT", "X.Hủy\n(Bình Thuận)"},
                    {"XH_MN", "X.Hủy\n(Mua Ngoài)"},
                    {"XP_VR", "X.Phơi\n(Việt Rau)"},
                    {"XP_BT", "X.Phơi\n(Bình Thuận)"},
                    {"XP_MN", "X.Phơi\n(Mua Ngoài)"},
                    {"XK_VR", "X.Khác\n(Việt Rau)"},
                    {"XK_BT", "X.Khác\n(Bình Thuận)"},
                    {"XK_MN", "X.Khác\n(Mua Ngoài)"},
                    {"TonTrongKho", "Tồn\nTrong Kho"},
                });

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

                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                Utils.SetTabStopRecursive(this, false);    
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
            }
            finally
            {
                await Task.Delay(200);
                loadingOverlay.Hide();
            }
 
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

        private void Excel_btn_Click(object sender, EventArgs e)
        {
            
        }
    }
}
