using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;

public class KhoVatTu_LichUomCay_Printer
{
    private DataTable mPlantingManagement_dt, mPlantTrayDensity_dt;
    private int rowIndex = 0; // để phân trang
    private int startX = 0;
    private int lineHeight = 30;
    private DateTime mStartDay, mEndDay;
    private string mWeekName;

    public KhoVatTu_LichUomCay_Printer(string weekName, DateTime startDay, DateTime endDay, DataTable mPlantTrayDensity_dt, DataTable plantingManagement_dt)
    {
        DataView dv = new DataView(plantingManagement_dt);
        dv.RowFilter = $"NurseryDate >= #{startDay:MM/dd/yyyy}# AND NurseryDate <= #{endDay:MM/dd/yyyy}#";
        dv.Sort = "NurseryDate ASC";

        this.mPlantingManagement_dt = dv.ToTable();
        this.mStartDay = startDay;
        this.mEndDay = endDay;
        this.mWeekName = weekName;
        this.mPlantTrayDensity_dt = mPlantTrayDensity_dt;
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

    private void Pd_PrintPage(object sender, PrintPageEventArgs e)
    {
        Graphics g = e.Graphics;
        int pageWidth = e.MarginBounds.Width;
        int pageHeight = e.MarginBounds.Height;

        // Cột
        int colWidth_Thu = 60, colWidth_Ngay = 100, colWidth_LenhSX = 80, colWidth_TenSP = 115, colWidth_SL = 85, colWidth_khay = 60, colWidth_area = 70;
        
        int col_ThuUom = startX;
        int col_NgayUom = col_ThuUom + colWidth_Thu;
        int col_LenhSX = col_NgayUom + colWidth_Ngay;
        int col_TenSP = col_LenhSX + colWidth_LenhSX;
        int col_SL = col_TenSP + colWidth_TenSP;
        int col_Khay = col_SL + colWidth_SL;
        int col_NgayTrong = col_Khay + colWidth_khay;
        int col_Area = col_NgayTrong + colWidth_Ngay;
        int colWidth_ghiChu = e.MarginBounds.Right - (col_Area + colWidth_area);
        int col_ghiChu = col_Area + colWidth_area;

        Font titleFont = new Font("Times New Roman", 24, FontStyle.Bold);

        Font normalFont = new Font("Times New Roman", 12);
        Font tableHeaderFont = new Font("Times New Roman", 12, FontStyle.Bold);

        int y = e.MarginBounds.Top - lineHeight*2;

        // Header chỉ in 1 lần ở đầu trang
        if (rowIndex == 0)
        {
            Utils.DrawCellText(g, $"LỊCH ƯƠM HẠT {mWeekName.ToUpper()}\n({mStartDay.Day.ToString("D2")}/{mStartDay.Month.ToString("D2")} - {mEndDay.Day.ToString("D2")}/{mEndDay.Month.ToString("D2")})", titleFont,
                new Rectangle(col_ThuUom, y, col_ghiChu + colWidth_ghiChu - col_ThuUom, lineHeight*4), StringAlignment.Center);
        }
        
        y += lineHeight * 4;

        int tableHeaderLineHeight = Convert.ToInt32(lineHeight * 1.5f);
        // Table Gray
        
        g.DrawRectangle(Pens.Gray, col_ThuUom, y, col_ghiChu + colWidth_ghiChu - col_ThuUom, tableHeaderLineHeight);
        g.DrawLine(Pens.Gray, col_NgayUom, y, col_NgayUom, y + tableHeaderLineHeight);
        g.DrawLine(Pens.Gray, col_LenhSX, y, col_LenhSX, y + tableHeaderLineHeight);
        g.DrawLine(Pens.Gray, col_TenSP, y, col_TenSP, y + tableHeaderLineHeight);
        g.DrawLine(Pens.Gray, col_SL, y, col_SL, y + tableHeaderLineHeight);
        g.DrawLine(Pens.Gray, col_Khay, y, col_Khay, y + tableHeaderLineHeight);
        g.DrawLine(Pens.Gray, col_NgayTrong, y, col_NgayTrong, y + tableHeaderLineHeight);
        g.DrawLine(Pens.Gray, col_Area, y, col_Area, y + tableHeaderLineHeight);
        g.DrawLine(Pens.Gray, col_ghiChu, y, col_ghiChu, y + tableHeaderLineHeight);

        Utils.DrawCellText(g, "Thứ", tableHeaderFont, new Rectangle(col_ThuUom, y, colWidth_Thu, tableHeaderLineHeight), StringAlignment.Center);
        Utils.DrawCellText(g, "Ngày Ươm", tableHeaderFont, new Rectangle(col_NgayUom, y, colWidth_Ngay, tableHeaderLineHeight), StringAlignment.Center);
        Utils.DrawCellText(g, "Lệnh SX", tableHeaderFont, new Rectangle(col_LenhSX, y, colWidth_LenhSX, tableHeaderLineHeight), StringAlignment.Center);
        Utils.DrawCellText(g, "Tên Cây", tableHeaderFont, new Rectangle(col_TenSP, y, colWidth_TenSP, tableHeaderLineHeight), StringAlignment.Center);
        Utils.DrawCellText(g, "S.Lượng", tableHeaderFont, new Rectangle(col_SL, y, colWidth_SL, tableHeaderLineHeight), StringAlignment.Center);
        Utils.DrawCellText(g, "S.Khay", tableHeaderFont, new Rectangle(col_Khay, y, colWidth_khay, tableHeaderLineHeight), StringAlignment.Center);
        Utils.DrawCellText(g, "Ngày Trồng", tableHeaderFont, new Rectangle(col_NgayTrong, y, colWidth_Ngay, tableHeaderLineHeight), StringAlignment.Center);
        Utils.DrawCellText(g, "D.Tích", tableHeaderFont, new Rectangle(col_Area, y, colWidth_area, tableHeaderLineHeight), StringAlignment.Center);
        Utils.DrawCellText(g, "Ghi Chú", tableHeaderFont, new Rectangle(col_ghiChu, y, colWidth_ghiChu, tableHeaderLineHeight), StringAlignment.Center);
                
        y += tableHeaderLineHeight;

        // Table Data với phân trang
        while (rowIndex < mPlantingManagement_dt.Rows.Count)
        {
            if (y + lineHeight > pageHeight)
            {
                e.HasMorePages = true;
                return; // sang trang tiếp theo
            }

            DataRow row = mPlantingManagement_dt.Rows[rowIndex];
            string productionOrder = row["ProductionOrder"].ToString();
            string note = row["Note"].ToString();
            string plantName = row["PlantName"].ToString();
            int quantity = Convert.ToInt32(row["Quantity"]);
            DateTime nurseryDate = Convert.ToDateTime(row["NurseryDate"]);
            DateTime plantingDate = Convert.ToDateTime(row["PlantingDate"]);
            decimal area = Convert.ToDecimal(row["Area"]);
            DateTime harvestDate = Convert.ToDateTime(row["HarvestDate"]);
            int sku = Convert.ToInt32(row["SKU"]);

            DataRow plantTrayDensityRow = mPlantTrayDensity_dt.Select($"SKU = '{sku}'").FirstOrDefault();
            decimal trayPerSquareMeter = 0;
            if (plantTrayDensityRow != null)
                trayPerSquareMeter = Convert.ToDecimal(plantTrayDensityRow["TrayPerSquareMeter"]);

            decimal soKhay = Convert.ToDecimal(row["Area"]) * trayPerSquareMeter;

            g.DrawRectangle(Pens.Gray, col_ThuUom, y, col_ghiChu + colWidth_ghiChu - col_ThuUom, lineHeight);
            g.DrawLine(Pens.Gray, col_NgayUom, y, col_NgayUom, y + lineHeight);
            g.DrawLine(Pens.Gray, col_LenhSX, y, col_LenhSX, y + lineHeight);
            g.DrawLine(Pens.Gray, col_TenSP, y, col_TenSP, y + lineHeight);
            g.DrawLine(Pens.Gray, col_SL, y, col_SL, y + lineHeight);
            g.DrawLine(Pens.Gray, col_Khay, y, col_Khay, y + lineHeight);
            g.DrawLine(Pens.Gray, col_NgayTrong, y, col_NgayTrong, y + lineHeight);
            g.DrawLine(Pens.Gray, col_Area, y, col_Area, y + lineHeight);
            g.DrawLine(Pens.Gray, col_ghiChu, y, col_ghiChu, y + lineHeight);

            Utils.DrawCellText(g, Utils.GetThu_Viet(nurseryDate), normalFont, new Rectangle(col_ThuUom, y, colWidth_Thu, lineHeight), StringAlignment.Center);
            Utils.DrawCellText(g, nurseryDate.ToString("dd/MM/yyyy"), normalFont, new Rectangle(col_NgayUom, y, colWidth_Ngay, lineHeight), StringAlignment.Center);
            Utils.DrawCellText(g, productionOrder, normalFont, new Rectangle(col_LenhSX, y, colWidth_LenhSX, lineHeight), StringAlignment.Center);
            Utils.DrawCellText(g, plantName, normalFont, new Rectangle(col_TenSP, y, colWidth_TenSP, lineHeight), StringAlignment.Near);
            Utils.DrawCellText(g, quantity.ToString("N0"), normalFont, new Rectangle(col_SL, y, colWidth_SL, lineHeight), StringAlignment.Far);
            Utils.DrawCellText(g, soKhay.ToString("N0"), normalFont, new Rectangle(col_Khay, y, colWidth_khay, lineHeight), StringAlignment.Far);
            Utils.DrawCellText(g, plantingDate.ToString("dd/MM/yyyy"), normalFont, new Rectangle(col_NgayTrong, y, colWidth_Ngay, lineHeight), StringAlignment.Center);
            Utils.DrawCellText(g, area.ToString("0.##"), normalFont, new Rectangle(col_Area, y, colWidth_area, lineHeight), StringAlignment.Far);
            Utils.DrawCellText(g, note, normalFont, new Rectangle(col_ghiChu, y, colWidth_ghiChu, lineHeight), StringAlignment.Center);

            y += lineHeight;
            rowIndex++;
        }

        e.HasMorePages = false;

    }
}
