
using RauViet.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

public class KhoVatTu_PhieuSX_Trong_Printer
{
    private DataTable mPlantingManagement_dt, mCultivationProcess_dt;
    private int rowIndex = 0; // để phân trang
    private int startX = 0;
    private int lineHeight = 35;

    public KhoVatTu_PhieuSX_Trong_Printer(List<int> departmentIDs, DataTable plantingManagement_dt, int month, int year)
    {
        DateTime start = new DateTime(year, month, 1);
        DateTime end = start.AddMonths(1);

        string deptFilter = string.Join(",", departmentIDs);

        DataView dv = new DataView(plantingManagement_dt);
        dv.RowFilter = $"PlantingDate >= #{start:MM/dd/yyyy}# AND PlantingDate < #{end:MM/dd/yyyy}# AND Department IN ({deptFilter})";
        dv.Sort = "PlantingDate ASC";

        this.mPlantingManagement_dt = dv.ToTable();
    }

    public void PrintPreview(Form owner)
    {
        rowIndex = 0; // reset trước khi in
        PrintDocument pd = new PrintDocument();
        pd.DefaultPageSettings.PaperSize = new PaperSize("A4", 827, 1169);
        pd.DefaultPageSettings.Landscape = false;
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
        pd.DefaultPageSettings.Landscape = false;
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
        int pageWidth = e.PageBounds.Width;
        int pageHeight = e.PageBounds.Height - 50; // margin dưới           
        
        Font headerFont = new Font("Times New Roman", 11, FontStyle.Bold);
        Font normalFont = new Font("Times New Roman", 11);

        int col1Width = 150, col2Width = 100, distanceWidth = 15, distanceHeight = 15;
        int numRow = 7;
        int intdex = 0;
        int offsetX = 5;
        while (rowIndex < mPlantingManagement_dt.Rows.Count)
        {
            DataRow row = mPlantingManagement_dt.Rows[rowIndex];

            DateTime plantingDate = Convert.ToDateTime(row["PlantingDate"]);
            int departmentID = Convert.ToInt32(row["Department"]);
            
            if (departmentID == 27)
            {
                numRow = 8;                
            }                

            int y = startX + (lineHeight * numRow + distanceHeight) * (int)(intdex / 3);
            int colHeaderX = startX + (col1Width + col2Width + distanceWidth) * (intdex% 3);
            int colValueX = startX;

            // Table Header
            g.DrawRectangle(Pens.Black, colHeaderX, y, col1Width + col2Width, lineHeight * numRow);
            if (departmentID != 27)
                g.DrawLine(Pens.Black, colHeaderX + col1Width, y, colHeaderX + col1Width, y + lineHeight * numRow);
            else
                g.DrawLine(Pens.Black, colHeaderX + col1Width, y, colHeaderX + col1Width, y + lineHeight * (numRow - 3));

            g.DrawLine(Pens.Black, colHeaderX, y, colHeaderX + col1Width + col2Width, y);
            Utils.DrawCellText(g, "Lệnh Sản Xuất", headerFont, new Rectangle(colHeaderX + offsetX, y, col1Width, lineHeight));
            Utils.DrawCellText(g, row["ProductionOrder"].ToString(), normalFont, new Rectangle(colHeaderX + col1Width + offsetX, y, col2Width, lineHeight)); y += lineHeight;

            g.DrawLine(Pens.Black, colHeaderX, y, colHeaderX + col1Width + col2Width, y);
            Utils.DrawCellText(g, "Tên Cây Trồng", headerFont, new Rectangle(colHeaderX + offsetX, y, col1Width, lineHeight));
            Utils.DrawCellText(g, row["PlantName"].ToString(), normalFont, new Rectangle(colHeaderX + col1Width + offsetX, y, col2Width, lineHeight)); y += lineHeight;

            if (departmentID != 27)
            {
                g.DrawLine(Pens.Black, colHeaderX, y, colHeaderX + col1Width + col2Width, y);
                Utils.DrawCellText(g, "Mã Cây Trồng", headerFont, new Rectangle(colHeaderX + offsetX, y, col1Width, lineHeight));
                Utils.DrawCellText(g, row["SKU"].ToString(), normalFont, new Rectangle(colHeaderX + col1Width + offsetX, y, col2Width, lineHeight)); y += lineHeight;
            }

            g.DrawLine(Pens.Black, colHeaderX, y, colHeaderX + col1Width + col2Width, y);
            Utils.DrawCellText(g, "Diện Tích", headerFont, new Rectangle(colHeaderX + offsetX, y, col1Width, lineHeight));
            Utils.DrawCellText(g, Convert.ToDecimal(row["Area"]).ToString("0.##"), normalFont, new Rectangle(colHeaderX + col1Width + offsetX, y, col2Width, lineHeight)); y += lineHeight;

            g.DrawLine(Pens.Black, colHeaderX, y, colHeaderX + col1Width + col2Width, y);
            Utils.DrawCellText(g, "Ngày Trồng", headerFont, new Rectangle(colHeaderX + offsetX, y, col1Width, lineHeight));
            Utils.DrawCellText(g, plantingDate.ToString("dd/MM/yyyy"), normalFont, new Rectangle(colHeaderX + col1Width + offsetX, y, col2Width, lineHeight)); y += lineHeight;

            g.DrawLine(Pens.Black, colHeaderX, y, colHeaderX + col1Width + col2Width, y);
            Utils.DrawCellText(g, "Ngày Thu Hoạch", headerFont, new Rectangle(colHeaderX + offsetX, y, col1Width, lineHeight));
            Utils.DrawCellText(g, Convert.ToDateTime(row["HarvestDate"]).ToString("dd/MM/yyyy"), normalFont, new Rectangle(colHeaderX + col1Width + offsetX, y, col2Width, lineHeight)); y += lineHeight;

            if (departmentID != 27)
            {
                g.DrawLine(Pens.Black, colHeaderX, y, colHeaderX + col1Width + col2Width, y);
                Utils.DrawCellText(g, "S.Lượng Dự Kiến", headerFont, new Rectangle(colHeaderX + offsetX, y, col1Width, lineHeight));
            }
            else
            {
                g.DrawLine(Pens.Black, colHeaderX, y, colHeaderX + col1Width + col2Width, y);
                Utils.DrawCellText(g, "Vị Trí", headerFont, new Rectangle(colHeaderX, y, col1Width + col2Width, lineHeight), StringAlignment.Center); y += lineHeight;

                int plantingID = Convert.ToInt32(row["PlantingID"]);
                var cultivationProcess_dt = await SQLStore_KhoVatTu.Instance.GetCultivationProcessAsync(plantingID);

                g.DrawLine(Pens.Black, colHeaderX, y, colHeaderX + col1Width + col2Width, y);
                Utils.DrawCellText(g, row["PlantLocation"].ToString(), normalFont, new Rectangle(colHeaderX, y, col1Width + col2Width, lineHeight*2), StringAlignment.Center);
            }
            
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
