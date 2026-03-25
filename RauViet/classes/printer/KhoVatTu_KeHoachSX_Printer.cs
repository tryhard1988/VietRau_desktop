using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;

public class KhoVatTu_KeHoachSX_Printer
{
    private DataTable mPlantingManagement_dt;
    private int rowIndex = 0; // để phân trang
    private int startX = 0;
    private int lineHeight = 24;
    private int mMonth, mYear, mCountSTT;
    private string mDepartmentName;

    public KhoVatTu_KeHoachSX_Printer(int departmentID, string departmentName, DataTable plantingManagement_dt, int month, int year)
    {
        DateTime start = new DateTime(year, month, 1);
        DateTime end = start.AddMonths(1);

        DataView dv = new DataView(plantingManagement_dt);
        dv.RowFilter = $"PlantingDate >= #{start:MM/dd/yyyy}# AND PlantingDate < #{end:MM/dd/yyyy}# AND Department = ({departmentID})";
        dv.Sort = "PlantingDate ASC";

        this.mPlantingManagement_dt = dv.ToTable();
        this.mMonth = month;
        this.mYear = year;
        this.mDepartmentName = departmentName;
        this.mCountSTT = 1;
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
        int colWidth_STT = 40, colWidth_LenhSX = 80, colWidth_TenSP = 110, colWidth_SL = 85, colWidth_Ngay = 90, colWidth_area = 50, colWidth_Thu = 60;
        int col_STT = startX;
        int col_LenhSX = col_STT + colWidth_STT;
        int col_TenSP = col_LenhSX + colWidth_LenhSX;
        int col_SL = col_TenSP + colWidth_TenSP;
        int col_NgayUom = col_SL + colWidth_SL;
        int col_NgayTrong = col_NgayUom + colWidth_Ngay;
        int col_Area = col_NgayTrong + colWidth_Ngay;
        int col_NgayThu = col_Area + colWidth_area;
        int col_ThuTrong = col_NgayThu + colWidth_Ngay;
        int col_ThuTH = col_ThuTrong + colWidth_Thu;
        int col_ViTriTrong = col_ThuTH + colWidth_Thu;
        int colWidth_ViTriTrong = e.MarginBounds.Right - col_ViTriTrong;

        Font titleFont = new Font("Times New Roman", 20, FontStyle.Bold);

        Font normalFont = new Font("Times New Roman", 11);
        Font tableHeaderFont = new Font("Times New Roman", 11, FontStyle.Bold);

        int y = e.MarginBounds.Top - lineHeight*2;

        // Header chỉ in 1 lần ở đầu trang
        if (rowIndex == 0)
        {
            Utils.DrawCellText(g, $"KẾ HOẠCH SẢN XUẤT THÁNG {mMonth.ToString("D2")}/{mYear.ToString()} - {mDepartmentName}", titleFont, new Rectangle(col_STT, y, col_ViTriTrong + colWidth_ViTriTrong - col_STT, lineHeight*2), StringAlignment.Center);
            y += lineHeight * 2;
        }
        
        

        int tableHeaderLineHeight = lineHeight * 3;
        // Table Gray

        SolidBrush bgBrush_LightGray = new SolidBrush(Color.FromArgb(217, 217, 217));
        g.FillRectangle(bgBrush_LightGray, new Rectangle(col_NgayTrong, y, colWidth_Ngay, tableHeaderLineHeight));
        g.FillRectangle(bgBrush_LightGray, new Rectangle(col_NgayThu, y, colWidth_Ngay, tableHeaderLineHeight));

        g.DrawRectangle(Pens.Gray, col_STT, y, col_ViTriTrong + colWidth_ViTriTrong - col_STT, tableHeaderLineHeight);
        g.DrawLine(Pens.Gray, col_LenhSX, y, col_LenhSX, y + tableHeaderLineHeight);
        g.DrawLine(Pens.Gray, col_TenSP, y, col_TenSP, y + tableHeaderLineHeight);
        g.DrawLine(Pens.Gray, col_SL, y, col_SL, y + tableHeaderLineHeight);
        g.DrawLine(Pens.Gray, col_NgayUom, y, col_NgayUom, y + tableHeaderLineHeight);
        g.DrawLine(Pens.Gray, col_NgayTrong, y, col_NgayTrong, y + tableHeaderLineHeight);
        g.DrawLine(Pens.Gray, col_Area, y, col_Area, y + tableHeaderLineHeight);
        g.DrawLine(Pens.Gray, col_NgayThu, y, col_NgayThu, y + tableHeaderLineHeight);
        g.DrawLine(Pens.Gray, col_ThuTrong, y, col_ThuTrong, y + tableHeaderLineHeight);
        g.DrawLine(Pens.Gray, col_ThuTH, y, col_ThuTH, y + tableHeaderLineHeight);
        g.DrawLine(Pens.Gray, col_ViTriTrong, y, col_ViTriTrong, y + tableHeaderLineHeight);

        Utils.DrawCellText(g, "STT", tableHeaderFont, new Rectangle(col_STT, y, colWidth_STT, tableHeaderLineHeight), StringAlignment.Center);
        Utils.DrawCellText(g, "Lệnh\nSản Xuất", tableHeaderFont, new Rectangle(col_LenhSX, y, colWidth_LenhSX, tableHeaderLineHeight), StringAlignment.Center);
        Utils.DrawCellText(g, "Tên\nSản Phẩm", tableHeaderFont, new Rectangle(col_TenSP, y, colWidth_TenSP, tableHeaderLineHeight), StringAlignment.Center);
        Utils.DrawCellText(g, "Số Lượng\nCây", tableHeaderFont, new Rectangle(col_SL, y, colWidth_SL, tableHeaderLineHeight), StringAlignment.Center);
        Utils.DrawCellText(g, "Ngày\nƯơm Hạt", tableHeaderFont, new Rectangle(col_NgayUom, y, colWidth_Ngay, tableHeaderLineHeight), StringAlignment.Center);
        Utils.DrawCellText(g, "Ngày Trồng", tableHeaderFont, new Rectangle(col_NgayTrong, y, colWidth_Ngay, tableHeaderLineHeight), StringAlignment.Center);
        Utils.DrawCellText(g, "Diện\nTích\n(m2)", tableHeaderFont, new Rectangle(col_Area, y, colWidth_area, tableHeaderLineHeight), StringAlignment.Center);
        Utils.DrawCellText(g, "Ngày\nThu Hoạch", tableHeaderFont, new Rectangle(col_NgayThu, y, colWidth_Ngay, tableHeaderLineHeight), StringAlignment.Center);
        Utils.DrawCellText(g, "Thứ\nTrồng\nCây", tableHeaderFont, new Rectangle(col_ThuTrong, y, colWidth_Thu, tableHeaderLineHeight), StringAlignment.Center);
        Utils.DrawCellText(g, "Thứ\nThu\nHoạch", tableHeaderFont, new Rectangle(col_ThuTH, y, colWidth_Thu, tableHeaderLineHeight), StringAlignment.Center);
        Utils.DrawCellText(g, "Vị Trí Trồng", tableHeaderFont, new Rectangle(col_ViTriTrong, y, colWidth_ViTriTrong, tableHeaderLineHeight), StringAlignment.Center);
                
        y += tableHeaderLineHeight;

        decimal totalArea = 0;
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
            string plantName = row["PlantName"].ToString();
            int quantity = Convert.ToInt32(row["Quantity"]);
            DateTime nurseryDate = Convert.ToDateTime(row["NurseryDate"]);
            DateTime plantingDate = Convert.ToDateTime(row["PlantingDate"]);
            decimal area = Convert.ToDecimal(row["Area"]);
            DateTime harvestDate = Convert.ToDateTime(row["HarvestDate"]);
            totalArea += area;
            g.DrawRectangle(Pens.Gray, col_STT, y, col_ViTriTrong + colWidth_ViTriTrong - col_STT, lineHeight);
            g.DrawLine(Pens.Gray, col_LenhSX, y, col_LenhSX, y + lineHeight);
            g.DrawLine(Pens.Gray, col_TenSP, y, col_TenSP, y + lineHeight);
            g.DrawLine(Pens.Gray, col_SL, y, col_SL, y + lineHeight);
            g.DrawLine(Pens.Gray, col_NgayUom, y, col_NgayUom, y + lineHeight);
            g.DrawLine(Pens.Gray, col_NgayTrong, y, col_NgayTrong, y + lineHeight);
            g.DrawLine(Pens.Gray, col_Area, y, col_Area, y + lineHeight);
            g.DrawLine(Pens.Gray, col_NgayThu, y, col_NgayThu, y + lineHeight);
            g.DrawLine(Pens.Gray, col_ThuTrong, y, col_ThuTrong, y + lineHeight);
            g.DrawLine(Pens.Gray, col_ThuTH, y, col_ThuTH, y + lineHeight);
            g.DrawLine(Pens.Gray, col_ViTriTrong, y, col_ViTriTrong, y + lineHeight);

            Utils.DrawCellText(g, mCountSTT.ToString(), normalFont, new Rectangle(col_STT + 2, y, colWidth_STT, lineHeight));
            Utils.DrawCellText(g, productionOrder, normalFont, new Rectangle(col_LenhSX + 2, y, colWidth_LenhSX, lineHeight));
            Utils.DrawCellText(g, plantName, normalFont, new Rectangle(col_TenSP + 2, y, colWidth_TenSP, lineHeight));
            Utils.DrawCellText(g, $"{quantity.ToString("N0")} cây", normalFont, new Rectangle(col_SL + 2, y, colWidth_SL, lineHeight));
            Utils.DrawCellText(g, nurseryDate.ToString("dd/MM/yyyy"), normalFont, new Rectangle(col_NgayUom, y, colWidth_Ngay, lineHeight), StringAlignment.Far);
            Utils.DrawCellText(g, plantingDate.ToString("dd/MM/yyyy"), normalFont, new Rectangle(col_NgayTrong, y, colWidth_Ngay, lineHeight), StringAlignment.Far);
            Utils.DrawCellText(g, area.ToString("0.##"), normalFont, new Rectangle(col_Area, y, colWidth_area, lineHeight), StringAlignment.Far);
            Utils.DrawCellText(g, harvestDate.ToString("dd/MM/yyyy"), normalFont, new Rectangle(col_NgayThu, y, colWidth_Ngay, lineHeight), StringAlignment.Far);
            Utils.DrawCellText(g, Utils.GetThu_Viet(plantingDate), normalFont, new Rectangle(col_ThuTrong, y, colWidth_Thu, lineHeight), StringAlignment.Center);
            Utils.DrawCellText(g, Utils.GetThu_Viet(harvestDate), normalFont, new Rectangle(col_ThuTH, y, colWidth_Thu, lineHeight), StringAlignment.Center);

            y += lineHeight;
            rowIndex++;
            mCountSTT++;
        }
        Utils.DrawCellText(g, $"TOTAL", tableHeaderFont, new Rectangle(col_STT, y, col_Area - col_STT, lineHeight), StringAlignment.Center);
        Utils.DrawCellText(g, totalArea.ToString("0.##"), tableHeaderFont, new Rectangle(col_Area, y, colWidth_area, lineHeight), StringAlignment.Far);

        e.HasMorePages = false;

    }
}
