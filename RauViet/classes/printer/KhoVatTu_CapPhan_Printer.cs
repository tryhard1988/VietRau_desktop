using RauViet.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Threading.Tasks;
using System.Windows.Forms;

public class KhoVatTu_CapPhan_Printer
{
    private DataTable mPlantingManagement_dt;
    private int rowIndex = 0; // để phân trang
    private int startX = 0;
    private int lineHeight = 27;
    private string mDepartmentName;
    Dictionary<int, DataTable> mCultivationCache;
    DataTable mCultivation_dt = null;
    private int mCultivationRowIndex = 0; // để phân trang
    private int pageNumber = 1;
    private int mMonth, mYear;
    public KhoVatTu_CapPhan_Printer(int departmentID, string departmentName, DataTable plantingManagement_dt, int month, int year)
    {
        mCultivationCache = new Dictionary<int, DataTable>();

        DateTime start = new DateTime(year, month, 1);
        DateTime end = start.AddMonths(1);

        DataView dv = new DataView(plantingManagement_dt);
        dv.RowFilter = $"PlantingDate >= #{start:MM/dd/yyyy}# AND PlantingDate < #{end:MM/dd/yyyy}# AND Department = ({departmentID})";
        dv.Sort = "PlantingDate ASC";

        this.mPlantingManagement_dt = dv.ToTable();
        this.mDepartmentName = departmentName;
        this.mMonth = month;
        this.mYear = year;
    }


    public async Task LoadCultivationCacheAsync()
    {
        foreach (DataRow row in mPlantingManagement_dt.Rows)
        {
            int plantingID = Convert.ToInt32(row["PlantingID"]);
            DataTable dt = await SQLStore_KhoVatTu.Instance.GetCultivationProcessAsync(plantingID);

            DataView dv = new DataView(dt);
            dv.Sort = "ProcessDate ASC";

            mCultivationCache[plantingID] = dv.ToTable();
        }
    }

    public void PrintPreview(Form owner)
    {
        rowIndex = 0; // reset trước khi in
        PrintDocument pd = new PrintDocument();
        pd.DefaultPageSettings.PaperSize = new PaperSize("A4", 827, 1169);
        pd.DefaultPageSettings.Landscape = true;
        pd.PrintPage += Pd_PrintPage;
        PrintPreviewDialog preview = new PrintPreviewDialog();
        preview.Document = pd;
        preview.WindowState = FormWindowState.Maximized;  // phóng to toàn màn hình
        preview.StartPosition = FormStartPosition.CenterScreen;

        foreach (Control c in preview.Controls)
        {
            if (c is ToolStrip ts)
            {
                foreach (ToolStripItem item in ts.Items)
                {
                    if (
                        item.Name.Equals("print", StringComparison.OrdinalIgnoreCase) ||
                        item.Name.Equals("printButton", StringComparison.OrdinalIgnoreCase) ||
                        item.Name.Equals("toolStripButton1", StringComparison.OrdinalIgnoreCase) ||
                        (item.ToolTipText?.Contains("Print") ?? false) ||
                        (item.Text?.Contains("Print") ?? false)
                       )
                    {
                        item.Visible = false;
                    }
                }
            }
        }

        preview.ShowDialog(owner);
    }

    public void PrintDirect()
    {
        rowIndex = 0; // reset trước khi in

        PrintDocument pd = new PrintDocument();
        pd.DefaultPageSettings.Landscape = true;
        pd.PrintPage += Pd_PrintPage;

        // 🔹 Hộp thoại chọn máy in
        PrintDialog printDialog = new PrintDialog();
        printDialog.Document = pd;
        printDialog.AllowSomePages = false;
        printDialog.AllowSelection = false;
        printDialog.UseEXDialog = true;

        if (printDialog.ShowDialog() == DialogResult.OK)
        {
            // Máy in đã được gán tự động cho pd
            pd.Print();
        }
    }

    private async void Pd_PrintPage(object sender, PrintPageEventArgs e)
    {
        Graphics g = e.Graphics;
        int pageWidth = e.MarginBounds.Width;
        int pageHeight = e.MarginBounds.Height; 

        // Cột
        int colWidth_TenSP = 110, colWidth_LenhSX = 80, colWidth_Ngay = 90, colWidth_Thu = 55, colWidth_CongViec = 200, colWidth_SL = 80, colWidth_DonVi = 50;
        int col_TenSP = startX;
        int col_LenhSX = col_TenSP + colWidth_TenSP;
        int col_Ngay = col_LenhSX + colWidth_LenhSX;
        int col_Thu = col_Ngay + colWidth_Ngay;
        int col_CongViec = col_Thu + colWidth_Thu;
        int col_VatTu = col_CongViec + colWidth_CongViec;
        int colWidth_VatTu = e.MarginBounds.Right - (col_CongViec + colWidth_CongViec + colWidth_SL + colWidth_DonVi);
        int col_SL = col_VatTu + colWidth_VatTu;
        int col_DonVi = col_SL + colWidth_SL;
        

        Font titleFont = new Font("Times New Roman", 20, FontStyle.Bold);

        Font normalFont = new Font("Times New Roman", 12);
        Font tableHeaderFont = new Font("Times New Roman", 12, FontStyle.Bold);

        int y = e.MarginBounds.Top;
        // Header chỉ in 1 lần ở đầu trang
        if (rowIndex == 0 && mCultivationRowIndex == 0)
        {
            Utils.DrawCellText(g, $"CẤP PHÂN THÁNG {mMonth.ToString("D2")}/{mYear.ToString()} - {mDepartmentName}", titleFont, new Rectangle(col_TenSP, y, col_DonVi + colWidth_DonVi - col_TenSP, lineHeight * 2), StringAlignment.Center);
        }

        y += lineHeight * 2;
        int tableHeaderLineHeight = lineHeight * 2;
        // Table Gray
        SolidBrush bgBrush_LightGray = new SolidBrush(Color.FromArgb(217, 217, 217));
        g.FillRectangle(bgBrush_LightGray, new Rectangle(col_TenSP, y, col_DonVi + colWidth_DonVi, tableHeaderLineHeight));

        g.DrawRectangle(Pens.Gray, col_TenSP, y, col_DonVi + colWidth_DonVi - col_TenSP, tableHeaderLineHeight);
        g.DrawLine(Pens.Gray, col_LenhSX, y, col_LenhSX, y + tableHeaderLineHeight);
        g.DrawLine(Pens.Gray, col_Ngay, y, col_Ngay, y + tableHeaderLineHeight);
        g.DrawLine(Pens.Gray, col_Thu, y, col_Thu, y + tableHeaderLineHeight);
        g.DrawLine(Pens.Gray, col_CongViec, y, col_CongViec, y + tableHeaderLineHeight);
        g.DrawLine(Pens.Gray, col_VatTu, y, col_VatTu, y + tableHeaderLineHeight);
        g.DrawLine(Pens.Gray, col_SL, y, col_SL, y + tableHeaderLineHeight);
        g.DrawLine(Pens.Gray, col_DonVi, y, col_DonVi, y + tableHeaderLineHeight);

        Utils.DrawCellText(g, "Tên\nSản Phẩm", tableHeaderFont, new Rectangle(col_TenSP, y, colWidth_TenSP, tableHeaderLineHeight), StringAlignment.Center);
        Utils.DrawCellText(g, "Lệnh\nSản Xuất", tableHeaderFont, new Rectangle(col_LenhSX, y, colWidth_LenhSX, tableHeaderLineHeight), StringAlignment.Center);
        Utils.DrawCellText(g, "Ngày", tableHeaderFont, new Rectangle(col_Ngay, y, colWidth_Ngay, tableHeaderLineHeight), StringAlignment.Center);
        Utils.DrawCellText(g, "Thứ", tableHeaderFont, new Rectangle(col_Thu, y, colWidth_Thu, tableHeaderLineHeight), StringAlignment.Center);
        Utils.DrawCellText(g, "Công Việc", tableHeaderFont, new Rectangle(col_CongViec, y, colWidth_CongViec, tableHeaderLineHeight), StringAlignment.Center);
        Utils.DrawCellText(g, "Tên Vật Tư", tableHeaderFont, new Rectangle(col_VatTu, y, colWidth_VatTu, tableHeaderLineHeight), StringAlignment.Center);
        Utils.DrawCellText(g, "Số\nLượng", tableHeaderFont, new Rectangle(col_SL, y, colWidth_SL, tableHeaderLineHeight), StringAlignment.Center);
        Utils.DrawCellText(g, "Đv", tableHeaderFont, new Rectangle(col_DonVi, y, colWidth_DonVi, tableHeaderLineHeight), StringAlignment.Center);
                
        y += tableHeaderLineHeight;

        // Table Data với phân trang
        while (rowIndex < mPlantingManagement_dt.Rows.Count)
        {
            DataRow row = mPlantingManagement_dt.Rows[rowIndex];
            string productionOrder = row["ProductionOrder"].ToString();
            string plantName = row["PlantName"].ToString();
            int plantingID = Convert.ToInt32(row["PlantingID"]);
            int cultivationTypeID = Convert.ToInt32(row["CultivationTypeID"]);

            if (mCultivation_dt == null)
            {
                mCultivation_dt = mCultivationCache[plantingID];
                mCultivationRowIndex = 0;
            }
            
            while (mCultivationRowIndex < mCultivation_dt.Rows.Count)
            {
                if (y + lineHeight > e.MarginBounds.Bottom)
                {

                    Utils.DrawPageNumber(g, e, pageNumber); // vẽ số trang trước khi sang trang
                    pageNumber++;
                    e.HasMorePages = true;
                    return; // sang trang tiếp theo
                }

                DataRow cpRow = mCultivation_dt.Rows[mCultivationRowIndex];
                string categoryCode = cpRow["CategoryCode"].ToString();
                int workTypeID = Convert.ToInt32(cpRow["WorkTypeID"]);
                
                if (cultivationTypeID == 4)
                {
                    string workTypeName_nosign = Utils.RemoveVietnameseSigns(cpRow["WorkTypeName"].ToString()).ToLower().Trim();
                    if (workTypeName_nosign.StartsWith("bon thuc") && workTypeName_nosign.CompareTo("bon thuc 1") != 0)
                    {
                        mCultivationRowIndex++;
                        continue;
                    }
                }

                if (workTypeID == 11 || workTypeID == 18)
                {
                    mCultivationRowIndex++;
                    continue;
                }

                if (categoryCode.CompareTo("PBL") != 0 && categoryCode.CompareTo("PHC") != 0 && categoryCode.CompareTo("PVC") != 0 && categoryCode.CompareTo("VTNN") != 0 && categoryCode.CompareTo("CPSH") != 0)
                {
                    mCultivationRowIndex++;
                    continue;
                }

                float width = col_DonVi +colWidth_DonVi - col_TenSP;
                Console.WriteLine($"x={col_TenSP}, y={y}, width={width}, height={lineHeight}");

                g.DrawRectangle(Pens.Gray, col_TenSP, y, width, lineHeight);
                g.DrawLine(Pens.Gray, col_LenhSX, y, col_LenhSX, y + lineHeight);
                g.DrawLine(Pens.Gray, col_Ngay, y, col_Ngay, y + lineHeight);
                g.DrawLine(Pens.Gray, col_Thu, y, col_Thu, y + lineHeight);
                g.DrawLine(Pens.Gray, col_CongViec, y, col_CongViec, y + lineHeight);
                g.DrawLine(Pens.Gray, col_VatTu, y, col_VatTu, y + lineHeight);
                g.DrawLine(Pens.Gray, col_SL, y, col_SL, y + lineHeight);
                g.DrawLine(Pens.Gray, col_DonVi, y, col_DonVi, y + lineHeight);

                DateTime processDate = Convert.ToDateTime(cpRow["ProcessDate"]);

                Utils.DrawCellText(g, plantName, normalFont, new Rectangle(col_TenSP, y, colWidth_TenSP, lineHeight), StringAlignment.Near);
                Utils.DrawCellText(g, productionOrder, normalFont, new Rectangle(col_LenhSX, y, colWidth_LenhSX, lineHeight), StringAlignment.Near);
                Utils.DrawCellText(g, processDate.ToString("dd/MM/yyyy"), normalFont, new Rectangle(col_Ngay, y, colWidth_Ngay, lineHeight), StringAlignment.Near);
                Utils.DrawCellText(g, Utils.GetThu_Viet(processDate), normalFont, new Rectangle(col_Thu, y, colWidth_Thu, lineHeight), StringAlignment.Center);
                Utils.DrawCellText(g, cpRow["WorkTypeName"].ToString(), normalFont, new Rectangle(col_CongViec, y, colWidth_CongViec, lineHeight), StringAlignment.Near);
                Utils.DrawCellText(g, cpRow["MaterialName"].ToString(), normalFont, new Rectangle(col_VatTu, y, colWidth_VatTu, lineHeight), StringAlignment.Near);
                Utils.DrawCellText(g, Convert.ToDecimal(cpRow["MaterialQuantity"]).ToString("0.##"), normalFont, new Rectangle(col_SL, y, colWidth_SL, lineHeight), StringAlignment.Far);
                Utils.DrawCellText(g, cpRow["MaterialUnit"].ToString(), normalFont, new Rectangle(col_DonVi, y, colWidth_DonVi, lineHeight), StringAlignment.Center);

                y += lineHeight;

                mCultivationRowIndex++;
            }

            rowIndex++;
            mCultivation_dt = null;
            mCultivationRowIndex = 0;
        }

        e.HasMorePages = false;

    }
}
