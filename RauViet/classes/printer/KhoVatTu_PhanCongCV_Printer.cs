using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;

public class KhoVatTu_PhanCongCV_Printer
{
    private DataTable mMaterialExport_dt;
    private int rowIndex = 0; // để phân trang
    private int startX = 0;
    private int lineHeight = 24;
    private DateTime mDate;
    private string mEmployeeName;

    public KhoVatTu_PhanCongCV_Printer(string employeeName, string employeeCode, DataTable materialExport_dt, DateTime date)
    {
        DateTime start = date;
        DateTime end = start.AddMonths(1);

        DataView dv = new DataView(materialExport_dt);
        dv.RowFilter = $"ExportDate >= #{start:MM/dd/yyyy}# AND ExportDate < #{end:MM/dd/yyyy}# AND Receiver = '{employeeCode}'";
        dv.Sort = "ExportDate ASC";

        this.mMaterialExport_dt = dv.ToTable();
        this.mDate = date;
        this.mEmployeeName = employeeName;
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
        int colWidth_ngay = 85, colWidth_caytrong = 130, colWidth_LSX = 90, colWidth_CV = 140, colWidth_VT = 280, colWidth_SL = 75, colWidth_DVT = 100;
        int col_ngay = startX;
        int col_caytrong = col_ngay + colWidth_ngay;
        int col_LSX = col_caytrong + colWidth_caytrong;
        int col_CV = col_LSX + colWidth_LSX;
        int col_VT = col_CV + colWidth_CV;
        int col_SL = col_VT + colWidth_VT;
        int col_DVT = col_SL + colWidth_SL;
        int col_NPT = col_DVT + colWidth_DVT;
        int colWidth_NPT = e.MarginBounds.Right - col_NPT;

        Font titleFont = new Font("Times New Roman", 20, FontStyle.Bold);

        Font normalFont = new Font("Times New Roman", 11);
        Font tableHeaderFont = new Font("Times New Roman", 11, FontStyle.Bold);

        int y = e.MarginBounds.Top - lineHeight*2;

        // Header chỉ in 1 lần ở đầu trang
        if (rowIndex == 0)
        {
            Utils.DrawCellText(g, $"BẢNG PHÂN CÔNG CÔNG VIỆC {mDate.ToString("dd/MM/yyyy")}", titleFont, new Rectangle(colWidth_ngay, y, col_NPT + colWidth_NPT - colWidth_ngay, lineHeight * 2), StringAlignment.Center);
            y += lineHeight * 2;
        }        
        

        int tableHeaderLineHeight = lineHeight * 2;
        // Table Gray

        SolidBrush bgBrush_LightGray = new SolidBrush(Color.FromArgb(217, 217, 217));
        g.FillRectangle(bgBrush_LightGray, new Rectangle(col_ngay, y, col_NPT + colWidth_NPT - col_ngay, tableHeaderLineHeight));



        g.DrawRectangle(Pens.Gray, col_ngay, y, col_NPT + colWidth_NPT - col_ngay, tableHeaderLineHeight);
        g.DrawLine(Pens.Gray, col_caytrong, y, col_caytrong, y + tableHeaderLineHeight);
        g.DrawLine(Pens.Gray, col_LSX, y, col_LSX, y + tableHeaderLineHeight);
        g.DrawLine(Pens.Gray, col_SL, y, col_SL, y + tableHeaderLineHeight);
        g.DrawLine(Pens.Gray, col_CV, y, col_CV, y + tableHeaderLineHeight);
        g.DrawLine(Pens.Gray, col_VT, y, col_VT, y + tableHeaderLineHeight);
        g.DrawLine(Pens.Gray, col_SL, y, col_SL, y + tableHeaderLineHeight);
        g.DrawLine(Pens.Gray, col_DVT, y, col_DVT, y + tableHeaderLineHeight);
        g.DrawLine(Pens.Gray, col_NPT, y, col_NPT, y + tableHeaderLineHeight);

        Utils.DrawCellText(g, "Ngày", tableHeaderFont, new Rectangle(col_ngay, y, colWidth_ngay, tableHeaderLineHeight), StringAlignment.Center);
        Utils.DrawCellText(g, "Tên Cây Trồng", tableHeaderFont, new Rectangle(col_caytrong, y, colWidth_caytrong, tableHeaderLineHeight), StringAlignment.Center);
        Utils.DrawCellText(g, "LSX", tableHeaderFont, new Rectangle(col_LSX, y, colWidth_LSX, tableHeaderLineHeight), StringAlignment.Center);
        Utils.DrawCellText(g, "Công Việc", tableHeaderFont, new Rectangle(col_CV, y, colWidth_CV, tableHeaderLineHeight), StringAlignment.Center);
        Utils.DrawCellText(g, "Tên Vật Tư", tableHeaderFont, new Rectangle(col_VT, y, colWidth_VT, tableHeaderLineHeight), StringAlignment.Center);
        Utils.DrawCellText(g, "Số Lượng", tableHeaderFont, new Rectangle(col_SL, y, colWidth_SL, tableHeaderLineHeight), StringAlignment.Center);
        Utils.DrawCellText(g, "ĐVT", tableHeaderFont, new Rectangle(col_DVT, y, colWidth_DVT, tableHeaderLineHeight), StringAlignment.Center);
        Utils.DrawCellText(g, "Người Ph.Trách", tableHeaderFont, new Rectangle(col_NPT, y, colWidth_NPT, tableHeaderLineHeight), StringAlignment.Center);
                
        y += tableHeaderLineHeight;

        // Table Data với phân trang
        while (rowIndex < mMaterialExport_dt.Rows.Count)
        {
            if (y + lineHeight > pageHeight)
            {
                e.HasMorePages = true;
                return; // sang trang tiếp theo
            }

            DataRow row = mMaterialExport_dt.Rows[rowIndex];
            DateTime exportDate = Convert.ToDateTime(row["ExportDate"]);
            string plantName = row["PlantName"].ToString(); 
            string productionOrder = row["ProductionOrder"].ToString();
            string workTypeName = row["WorkTypeName"].ToString();
            string materialName = row["MaterialName"].ToString();
            decimal amount = Convert.ToDecimal(row["Amount"]);
            string unitName = row["UnitName"].ToString();
            string recieverName = row["RecieverName"].ToString();

            g.DrawRectangle(Pens.Gray, col_ngay, y, col_NPT + colWidth_NPT - col_ngay, lineHeight);
            g.DrawLine(Pens.Gray, col_caytrong, y, col_caytrong, y + lineHeight);
            g.DrawLine(Pens.Gray, col_LSX, y, col_LSX, y + lineHeight);
            g.DrawLine(Pens.Gray, col_SL, y, col_SL, y + lineHeight);
            g.DrawLine(Pens.Gray, col_CV, y, col_CV, y + lineHeight);
            g.DrawLine(Pens.Gray, col_VT, y, col_VT, y + lineHeight);
            g.DrawLine(Pens.Gray, col_SL, y, col_SL, y + lineHeight);
            g.DrawLine(Pens.Gray, col_DVT, y, col_DVT, y + lineHeight);
            g.DrawLine(Pens.Gray, col_NPT, y, col_NPT, y + lineHeight);

            Utils.DrawCellText(g, exportDate.ToString("dd/MM/yyyy"), normalFont, new Rectangle(col_ngay, y, colWidth_ngay, lineHeight), StringAlignment.Near);
            Utils.DrawCellText(g, plantName, normalFont, new Rectangle(col_caytrong, y, colWidth_caytrong, lineHeight), StringAlignment.Near);
            Utils.DrawCellText(g, productionOrder, normalFont, new Rectangle(col_LSX, y, colWidth_LSX, lineHeight), StringAlignment.Center);
            Utils.DrawCellText(g, workTypeName, normalFont, new Rectangle(col_CV, y, colWidth_CV, lineHeight), StringAlignment.Near);
            Utils.DrawCellText(g, materialName, normalFont, new Rectangle(col_VT, y, colWidth_VT, lineHeight), StringAlignment.Near);
            Utils.DrawCellText(g, amount.ToString("N2"), normalFont, new Rectangle(col_SL, y, colWidth_SL, lineHeight), StringAlignment.Far);
            Utils.DrawCellText(g, unitName, normalFont, new Rectangle(col_DVT, y, colWidth_DVT, lineHeight), StringAlignment.Center);
            Utils.DrawCellText(g, recieverName, normalFont, new Rectangle(col_NPT, y, colWidth_NPT, lineHeight), StringAlignment.Near);

            y += lineHeight;
            rowIndex++;
        }

        e.HasMorePages = false;

    }
}
