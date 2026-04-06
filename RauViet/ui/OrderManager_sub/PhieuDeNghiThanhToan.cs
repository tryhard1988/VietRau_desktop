using RauViet.classes;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataTable = System.Data.DataTable;

namespace RauViet.ui
{
    public partial class PhieuDeNghiThanhToan : Form
    {
        DataTable mEmployee_dt, mSeller_dt;
        private Timer debounceTimer = new Timer { Interval = 300 };
        private Timer sellerDebounceTimer = new Timer { Interval = 300 };
        bool mIsPrintPreview;
        private LoadingOverlay loadingOverlay;
        public PhieuDeNghiThanhToan(DateTime ngayMua, int sotien, string noidung, bool isPrintPreview)
        {
            InitializeComponent();

            ngayMua_dtp.Format = DateTimePickerFormat.Custom;
            ngayMua_dtp.CustomFormat = "dd/MM/yyyy";

            soTien_tb.Text = sotien.ToString("N0");
            ngayMua_dtp.Value = ngayMua;
            noidung_tb.Text = $"Mua hàng chuyến {noidung}";

            LuuThayDoiBtn.Click += saveBtn_Click;
            debounceTimer.Tick += DebounceTimer_Tick;
            sellerDebounceTimer.Tick += SellerDebounceTimer_Tick;
                   
            seller_cbb.TextUpdate += Seller_cbb_TextUpdate;
            nguoiDeNghij_cbb.TextUpdate += NguoiDeNghij_cbb_TextUpdate;
            mIsPrintPreview = isPrintPreview;
        }

        public async void ShowData()
        {
            var SellerTask = SQLStore_Kho.Instance.GetSupplierAsync();                
            var EmployeeTask = SQLStore_QLNS.Instance.GetEmployeesAsync();
            await Task.WhenAll(EmployeeTask, SellerTask);
            mSeller_dt = SellerTask.Result;
            mEmployee_dt = EmployeeTask.Result;                        

            nguoiDeNghij_cbb.DataSource = mEmployee_dt;
            nguoiDeNghij_cbb.DisplayMember = "FullName";  // hiển thị tên
            nguoiDeNghij_cbb.ValueMember = "EmployeeCode";
            nguoiDeNghij_cbb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            nguoiDeNghij_cbb.SelectedValue = UserManager.Instance.employeeCode;

            seller_cbb.DataSource = mSeller_dt;
            seller_cbb.DisplayMember = "SupplierName";  // hiển thị tên
            seller_cbb.ValueMember = "SupplierID";
            seller_cbb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
        }

        private void NguoiDeNghij_cbb_TextUpdate(object sender, EventArgs e)
        {
            // Restart timer mỗi khi gõ
            debounceTimer.Stop();
            debounceTimer.Start();
        }

        private void DebounceTimer_Tick(object sender, EventArgs e)
        {
            debounceTimer.Stop();
            Utils.ComboBoxSearchResult(nguoiDeNghij_cbb, mEmployee_dt, "EmployessName_NoSign");
        }

        private void Seller_cbb_TextUpdate(object sender, EventArgs e)
        {
            sellerDebounceTimer.Stop();
            sellerDebounceTimer.Start();
        }

        private void SellerDebounceTimer_Tick(object sender, EventArgs e)
        {
            sellerDebounceTimer.Stop();
            Utils.ComboBoxSearchResult(seller_cbb, mSeller_dt, "searching_nosign");
        }


        private async void saveBtn_Click(object sender, EventArgs e)
        {
            string nguoiDeNghi = nguoiDeNghij_cbb.Text;
            int supplierID = Convert.ToInt32(seller_cbb.SelectedValue);
            DeNghiThanhToan_Printer printer = new DeNghiThanhToan_Printer(nguoiDeNghi, supplierID, ngayMua_dtp.Value, Convert.ToInt32(soLan_tb.Text), noidung_tb.Text, Convert.ToInt32(soTien_tb.Text), note_tb.Text);
            try
            {
                await printer.loadData();

                if (!mIsPrintPreview)
                    printer.PrintDirect();
                else
                    printer.PrintPreview(this);

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
