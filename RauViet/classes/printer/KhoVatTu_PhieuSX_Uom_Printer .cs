using RauViet.ui;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;

public class KhoVatTu_PhieuSX_Uom_Printer
{
    private DataTable mPlantingManagement_dt, mPlantTrayDensity_dt;
    private int rowIndex = 0; // để phân trang
    private int startX = 0;
    private int lineHeight = 23;

    public KhoVatTu_PhieuSX_Uom_Printer(List<int> departmentIDs, DataTable plantTrayDensity, DataTable plantingManagement_dt, int month, int year)
    {
        DateTime start = new DateTime(year, month, 1);
        DateTime end = start.AddMonths(1);

        string deptFilter = string.Join(",", departmentIDs);

        DataView dv = new DataView(plantingManagement_dt);
        dv.RowFilter = $"PlantingDate >= #{start:MM/dd/yyyy}# AND PlantingDate < #{end:MM/dd/yyyy}# AND Department IN ({deptFilter})";
        dv.Sort = "PlantingDate ASC";

        this.mPlantingManagement_dt = dv.ToTable();
        this.mPlantTrayDensity_dt = plantTrayDensity;
    }

    private void DrawCellText(Graphics g, string text, Font font, Rectangle rect, StringAlignment alignment = StringAlignment.Near, StringAlignment lineAlignment = StringAlignment.Center)
    {
        StringFormat format = new StringFormat()
        {
            Alignment = alignment,
            LineAlignment = lineAlignment
        };
        g.DrawString(text, font, Brushes.Black, rect, format);
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
        int pageWidth = e.PageBounds.Width;
        int pageHeight = e.PageBounds.Height - 50; // margin dưới           
        
        Font headerFont = new Font("Times New Roman", 11, FontStyle.Bold);
        Font headerFont1 = new Font("Times New Roman", 11, FontStyle.Bold | FontStyle.Underline);
        Font normalFont = new Font("Times New Roman", 11);

        int col1Width = 150, col2Width = 100, distanceWidth = 15, distanceHeight = 15;
        int numRow = 8;
        int intdex = 0;
        int offsetX = 5;
        while (rowIndex < mPlantingManagement_dt.Rows.Count)
        {
            DataRow row = mPlantingManagement_dt.Rows[rowIndex];

            DateTime plantingDate = Convert.ToDateTime(row["PlantingDate"]);            
            int sku = Convert.ToInt32(row["SKU"]);
            DataRow plantTrayDensityRow = mPlantTrayDensity_dt.Select($"SKU = '{sku}'").FirstOrDefault();
            decimal trayPerSquareMeter = 0;
            if(plantTrayDensityRow != null)
                trayPerSquareMeter = Convert.ToDecimal(plantTrayDensityRow["TrayPerSquareMeter"]);

            decimal soKhay = Convert.ToDecimal(row["Area"]) * trayPerSquareMeter;

            int y = startX + (lineHeight * numRow + distanceHeight) * (int)(intdex / 3);
            int colHeaderX = startX + (col1Width + col2Width + distanceWidth) * (intdex% 3);

            // Table Header
            g.DrawRectangle(Pens.Black, colHeaderX, y, col1Width + col2Width, lineHeight * numRow);
            g.DrawLine(Pens.Black, colHeaderX + col1Width, y + lineHeight*2, colHeaderX + col1Width, y + lineHeight * (numRow - 2));

            g.DrawLine(Pens.Black, colHeaderX, y, colHeaderX + col1Width + col2Width, y);
            DrawCellText(g, "Tên Cây Trồng", headerFont, new Rectangle(colHeaderX, y, col1Width + col2Width, lineHeight), StringAlignment.Center); y += lineHeight;
            g.DrawLine(Pens.Black, colHeaderX, y, colHeaderX + col1Width + col2Width, y);
            DrawCellText(g, row["PlantName"].ToString(), headerFont1, new Rectangle(colHeaderX, y, col1Width + col2Width, lineHeight), StringAlignment.Center); y += lineHeight;

            g.DrawLine(Pens.Black, colHeaderX, y, colHeaderX + col1Width + col2Width, y);
            DrawCellText(g, "Lệnh Sản Xuất", headerFont, new Rectangle(colHeaderX + offsetX, y, col1Width, lineHeight));
            DrawCellText(g, row["ProductionOrder"].ToString(), normalFont, new Rectangle(colHeaderX + col1Width + offsetX, y, col2Width, lineHeight)); y += lineHeight;

            g.DrawLine(Pens.Black, colHeaderX, y, colHeaderX + col1Width + col2Width, y);
            DrawCellText(g, "Số Khay", headerFont, new Rectangle(colHeaderX + offsetX, y, col1Width, lineHeight));
            DrawCellText(g, soKhay.ToString("N0"), normalFont, new Rectangle(colHeaderX + col1Width + offsetX, y, col2Width, lineHeight)); y += lineHeight;

            g.DrawLine(Pens.Black, colHeaderX, y, colHeaderX + col1Width + col2Width, y);
            DrawCellText(g, "Ngày Ươm", headerFont, new Rectangle(colHeaderX + offsetX, y, col1Width, lineHeight));
            DrawCellText(g, Convert.ToDateTime(row["NurseryDate"]).ToString("dd/MM/yyyy"), normalFont, new Rectangle(colHeaderX + col1Width + offsetX, y, col2Width, lineHeight)); y += lineHeight;

            g.DrawLine(Pens.Black, colHeaderX, y, colHeaderX + col1Width + col2Width, y);
            DrawCellText(g, "Ngày Trồng", headerFont, new Rectangle(colHeaderX + offsetX, y, col1Width, lineHeight));
            DrawCellText(g, plantingDate.ToString("dd/MM/yyyy"), normalFont, new Rectangle(colHeaderX + col1Width + offsetX, y, col2Width, lineHeight)); y += lineHeight;

            g.DrawLine(Pens.Black, colHeaderX, y, colHeaderX + col1Width + col2Width, y);
            DrawCellText(g, $"Ghi Chú: {row["Note"].ToString()}", normalFont, new Rectangle(colHeaderX, y, col1Width + col2Width, lineHeight*2), StringAlignment.Near, StringAlignment.Near);


            rowIndex++;
            intdex++;

            if ((y + lineHeight * numRow > pageHeight) && (intdex % 3 == 0) && rowIndex < mPlantingManagement_dt.Rows.Count && rowIndex < mPlantingManagement_dt.Rows.Count)
            {
                e.HasMorePages = true;
                return; // sang trang tiếp theo
            }
        }
        
        e.HasMorePages = false;

    }
}
