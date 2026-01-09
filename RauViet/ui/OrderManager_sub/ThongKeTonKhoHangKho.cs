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
    public partial class ThongKeTonKhoHangKho : Form
    {
        private Timer debounceTimer = new Timer { Interval = 300 };
        DataView mLogDV;
        bool isNewState = false;
        private LoadingOverlay loadingOverlay;
        public ThongKeTonKhoHangKho()
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
                SQLStore_Kho.Instance.removeInventoryTransaction();
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
                var logTask = SQLStore_Kho.Instance.getInventoryTransactionLogSync();
                var InventoryTransactionTask = SQLStore_Kho.Instance.getInventoryTransactionSync();

                await Task.WhenAll(logTask, InventoryTransactionTask);
                mLogDV = new DataView(logTask.Result);

                int currentYear = DateTime.Now.Year;
                var result = InventoryTransactionTask.Result
                            .AsEnumerable()
                            .GroupBy(r => r.Field<int>("SKU"))
                            .Select(g =>
                            {
                                var firstRow = g.First();
                                // Tồn đầu năm
                                var tonDauNam =
                                    g.Where(r => r.Field<string>("TransactionType") == "IN"
                                              && r.Field<DateTime>("TransactionDate").Year < currentYear)
                                     .Sum(r => r.Field<int>("Quantity"))
                                  -
                                    g.Where(r => r.Field<string>("TransactionType") == "OUT"
                                              && r.Field<DateTime>("TransactionDate").Year < currentYear)
                                     .Sum(r => r.Field<int>("Quantity"));

                                // Nhập hàng năm hiện tại
                                var nhapHang = g
                                    .Where(r => r.Field<string>("TransactionType") == "IN"
                                             && r.Field<DateTime>("TransactionDate").Year == currentYear)
                                    .Sum(r => r.Field<int>("Quantity"));

                                // Xuất hàng năm hiện tại
                                var xuatHang = g
                                    .Where(r => r.Field<string>("TransactionType") == "OUT"
                                             && r.Field<DateTime>("TransactionDate").Year == currentYear)
                                    .Sum(r => r.Field<int>("Quantity"));

                                return new
                                {
                                    SKU = g.Key,
                                    Name_VN = firstRow.Field<string>("Name_VN"),
                                    Package = firstRow.Field<string>("Package"),
                                    TonDauNam = tonDauNam,
                                    NhapHang = nhapHang,
                                    XuatHang = xuatHang
                                };
                            })
                            .ToList();

                DataTable dtResult = new DataTable();
                dtResult.Columns.Add("SKU", typeof(int));
                dtResult.Columns.Add("Tên Sản Phẩm", typeof(string));
                dtResult.Columns.Add("Đ.Vị", typeof(string));
                dtResult.Columns.Add("Tồn đầu năm", typeof(int));
                dtResult.Columns.Add("Nhập hàng", typeof(int));
                dtResult.Columns.Add("Xuất hàng", typeof(int));
                dtResult.Columns.Add("Tồn Trong Kho", typeof(int));

                foreach (var item in result)
                {
                    dtResult.Rows.Add(
                        item.SKU,
                        item.Name_VN,
                        item.Package,
                        item.TonDauNam,
                        item.NhapHang,
                        item.XuatHang,
                        item.TonDauNam + item.NhapHang - item.XuatHang
                    );
                }

                log_GV.DataSource = mLogDV;
                dataGV.DataSource = dtResult;

                dataGV.Columns["SKU"].Width = 70;
                dataGV.Columns["Tên Sản Phẩm"].Width = 250;
                                
                log_GV.Columns["SKU"].Visible = false;
                log_GV.Columns["LogID"].Width = 80;
                log_GV.Columns["ActionBy"].Width = 170;
                log_GV.Columns["CreateedAt"].Width = 130;
                log_GV.Columns["OldValue"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                log_GV.Columns["NewValue"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

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
