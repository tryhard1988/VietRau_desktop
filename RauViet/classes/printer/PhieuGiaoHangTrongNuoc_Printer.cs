using DocumentFormat.OpenXml.CustomProperties;
using DocumentFormat.OpenXml.Drawing;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Media;

public class PhieuGiaoHangTrongNuoc_Printer
{
    private int currentRowIndex = 0; // chỉ số dòng hiện tại
    private int yPosition = 0;
    private bool firstPage = true;
    private DataTable data;
    private int orderDomesticIndex;
    private DateTime deliveryDate;
    private string customerCode;
    private string company;
    private string customerName;
    private string customerAddress;
    private string InputByName;
    int totalPCS = 0;
    decimal totalNetWeight = 0;
    // ------------------- Hàm Print Preview -------------------, this);
    public void PrintPreview(DataTable dataTable, int orderDomesticIndex, DateTime deliveryDate, string customerCode, string  company, string customerName, string customerAddress, string InputByName, Form owner)
    {
        if (dataTable == null || dataTable.Rows.Count == 0)
            return;

        SetupPrint(dataTable, orderDomesticIndex, deliveryDate, customerCode, company, customerName, customerAddress, InputByName);

        PrintDocument printDoc = new PrintDocument();
        printDoc.PrintPage += PrintDoc_PrintPage;

        PrintPreviewDialog preview = new PrintPreviewDialog
        {
            Document = printDoc
        };
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

    // ------------------- Hàm Print trực tiếp -------------------
    public void PrintDirect(DataTable dataTable, int orderDomesticIndex, DateTime deliveryDate, string customerCode, string company, string customerName, string customerAddress, string InputByName, bool isIn2Mat)
    {
        if (dataTable == null || dataTable.Rows.Count == 0)
            return;

        SetupPrint(dataTable, orderDomesticIndex, deliveryDate, customerCode, company, customerName, customerAddress, InputByName);

        PrintDocument printDoc = new PrintDocument();
        printDoc.PrintPage += PrintDoc_PrintPage;
        printDoc.PrinterSettings.Duplex = isIn2Mat ? Duplex.Vertical : Duplex.Simplex;

        // Hiển thị hộp thoại chọn máy in
        using (PrintDialog printDialog = new PrintDialog())
        {
            printDialog.Document = printDoc;
            printDialog.AllowSomePages = true; // cho phép chọn trang
            printDialog.AllowSelection = true;  // cho phép chọn lựa
            printDialog.UseEXDialog = true;
            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                printDoc.Print(); // in ra máy in được chọn
            }
        }
    }

    // ------------------- Setup dữ liệu chung -------------------
    private void SetupPrint(DataTable dataTable, int orderDomesticIndex, DateTime deliveryDate, string customerCode, string company, string customerName, string customerAddress, string InputByName)
    {
        this.data = dataTable;
        this.orderDomesticIndex = orderDomesticIndex;
        this.deliveryDate = deliveryDate;
        this.customerCode = customerCode;
        this.company = company;
        this.customerName = customerName;
        this.customerAddress = customerAddress;
        this.InputByName = InputByName;
        currentRowIndex = 0;
        firstPage = true;
        yPosition = 0;
        totalPCS = 0;
        totalNetWeight = 0;
    }

    // ------------------- Hàm PrintPage -------------------
    private void PrintDoc_PrintPage(object sender, PrintPageEventArgs e)
    {
        int paddingBottom = e.PageBounds.Bottom - e.MarginBounds.Bottom;
        int startX = 50;
        int startY = firstPage ? 50 : yPosition;
        int rowHeight = 27;

        int colWidth_STT = 50;
        int colWidth_SKU = 70;
        int colWidth_MV = 130;
        int colWidth_Product = 115;
        int colWidth_QC = 100;
        int colWidth_SLGH = 120;
        int colWidth_gc = e.PageBounds.Width - colWidth_STT - colWidth_SKU - colWidth_MV - colWidth_Product - colWidth_QC - colWidth_SLGH - startX*2 - 30;

        int y = startY;

        Font fontTitle1 = new Font("Times New Roman", 16, FontStyle.Bold);
        Font fontTitle = new Font("Times New Roman", 20, FontStyle.Bold);
        Font fontHeader = new Font("Times New Roman", 12, FontStyle.Bold);
        Font fontHeader1 = new Font("Times New Roman", 13, FontStyle.Bold);
        Font fontContent = new Font("Times New Roman", 12);
        Font fontContent1 = new Font("Times New Roman", 14);
        System.Drawing.Brush brush = System.Drawing.Brushes.Black;

        StringFormat sfHeader = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        StringFormat sfDataLeft = new StringFormat() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
        StringFormat sfDataCenter = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

        // Tiêu đề trang đầu
        if (firstPage)
        {
            Image img = RauViet.Properties.Resources.ic_logo;
            e.Graphics.DrawImage(img, e.MarginBounds.Left, y - rowHeight / 3, (int)(img.Width * 0.15), (int)(img.Height * 0.15));
            y -= rowHeight / 2;
            e.Graphics.DrawString("CÔNG TY CỔ PHẦN VIỆT RAU", fontTitle1, brush, e.MarginBounds.Left + (int)(img.Width * 0.15) + 30, y);
            y += (rowHeight + 2);
            e.Graphics.DrawString(Utils.getCompanyAddress(), fontContent1, brush, e.MarginBounds.Left + (int)(img.Width * 0.15) + 30, y);
            y += (rowHeight + 10);
            e.Graphics.DrawString($"Tel/Zalo: {Utils.get_SDT_DiDong()}      Email: honggam.tran@vietrau.com", fontContent1, brush, e.MarginBounds.Left + (int)(img.Width * 0.15) + 30, y);
            y += rowHeight;
            e.Graphics.DrawLine(new System.Drawing.Pen(System.Drawing.Color.Black, 1), startX, y, e.PageBounds.Width - startX*2, y);
            y += (rowHeight - 10);

            string title = "PHIẾU GIAO HÀNG";
            SizeF titleSize = e.Graphics.MeasureString(title, fontTitle);
            float titleX = e.MarginBounds.Left + (e.MarginBounds.Width - titleSize.Width) / 2 - 20;
            e.Graphics.DrawString(title, fontTitle, brush, titleX, y);
            y += rowHeight;

            string code = $"Số: {orderDomesticIndex.ToString("000")}/{deliveryDate.Year}/{customerCode}";
            SizeF codeSize = e.Graphics.MeasureString(code, fontTitle);
            e.Graphics.DrawString(code, fontHeader1, brush, e.PageBounds.Width - startX * 2 - codeSize.Width/2  -30, y);
            y += rowHeight;

            e.Graphics.DrawString($"Người Mua Hàng:", fontContent, brush, startX, y);
            e.Graphics.DrawString($"{customerName}", fontHeader1, brush, startX + 150, y);
            y += rowHeight;
            e.Graphics.DrawString($"Đơn Vị:", fontContent, brush, startX, y);
            e.Graphics.DrawString($"{company}", fontHeader1, brush, startX + 150, y);
            y += rowHeight;
            e.Graphics.DrawString($"Địa Chỉ:", fontContent, brush, startX, y);
            e.Graphics.DrawString($"{customerAddress}", fontContent, brush, startX + 150, y);
            y += rowHeight;
            e.Graphics.DrawString($"Ngày Giao:", fontContent, brush, startX, y);
            e.Graphics.DrawString($"{deliveryDate:dd/MM/yyyy}", fontHeader1, brush, startX + 150, y);
            y += rowHeight;
        }
        y += 10;
        // Header bảng
        int x = startX;
        e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_STT, rowHeight*2);
        e.Graphics.DrawString("STT", fontHeader, brush, new RectangleF(x, y, colWidth_STT, rowHeight*2), sfHeader);
        x += colWidth_STT;

        e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_SKU, rowHeight);
        e.Graphics.DrawString("SKU", fontHeader, brush, new RectangleF(x, y, colWidth_SKU, rowHeight), sfHeader);
        e.Graphics.DrawRectangle(Pens.Black, x, y + rowHeight, colWidth_SKU, rowHeight);
        e.Graphics.DrawString("Art.Nr", fontHeader, brush, new RectangleF(x, y + rowHeight, colWidth_SKU, rowHeight), sfHeader);
        x += colWidth_SKU;

        e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_MV, rowHeight * 2);
        e.Graphics.DrawString("Mã Vạch", fontHeader, brush, new RectangleF(x, y, colWidth_MV, rowHeight * 2), sfHeader);
        x += colWidth_MV;

        e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_Product, rowHeight * 2);
        e.Graphics.DrawString("Tên Sản Phẩm", fontHeader, brush, new RectangleF(x, y, colWidth_Product, rowHeight * 2), sfHeader);
        x += colWidth_Product;

        e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_QC, rowHeight * 2);
        e.Graphics.DrawString("Quy cách\nđóng gói", fontHeader, brush, new RectangleF(x, y, colWidth_QC, rowHeight * 2), sfHeader);
        x += colWidth_QC;

        e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_SLGH, rowHeight);
        e.Graphics.DrawString("SL hàng giao", fontHeader, brush, new RectangleF(x, y, colWidth_SLGH, rowHeight), sfHeader);
        

        e.Graphics.DrawRectangle(Pens.Black, x, y + rowHeight, colWidth_SLGH/2, rowHeight);
        e.Graphics.DrawString("Gói", fontHeader, brush, new RectangleF(x, y + rowHeight, colWidth_SLGH/2, rowHeight), sfHeader);
        x += colWidth_SLGH / 2;
        e.Graphics.DrawRectangle(Pens.Black, x, y + rowHeight, colWidth_SLGH/2, rowHeight);
        e.Graphics.DrawString("Kg", fontHeader, brush, new RectangleF(x, y + rowHeight, colWidth_SLGH/2, rowHeight), sfHeader);
        x += colWidth_SLGH / 2;

        e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_gc, rowHeight*2);
        e.Graphics.DrawString("Ghi chú", fontHeader, brush, new RectangleF(x, y, colWidth_gc, rowHeight*2), sfHeader);
        y += (rowHeight * 2);
        
        // ------------------ Dữ liệu ------------------
        

        for (; currentRowIndex < data.Rows.Count; currentRowIndex++)
        {
            var row = data.Rows[currentRowIndex];

            x = startX;

            // ProductPackingName
            string SKU = row["SKU"].ToString();
            string name = row["ProductNameVN"].ToString();
            string barCodeEAN13 = row["BarCodeEAN13"].ToString();
            string packing = row["packing"].ToString();
            string package = row["Package"].ToString();
            string productTypeName = row["ProductTypeName"].ToString();
            string amountPaking = "";
            if (packing.CompareTo("weight") == 0 || package.CompareTo("weight") == 0)
                amountPaking = "WEIGHT";
            else
                amountPaking = row["amount"].ToString() + " " + packing;

            name = Regex.Replace(name, @"\s*\d+\s*(g|gr|kg)\b", "", RegexOptions.IgnoreCase).Trim();
            RectangleF rect = new RectangleF(x, y, colWidth_Product, 9999);
            SizeF textSize = e.Graphics.MeasureString(name,fontContent,(int)colWidth_Product, StringFormat.GenericDefault);
            int dynamicHeight = Math.Max(rowHeight, (int)textSize.Height);

            // STT
            e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_STT, dynamicHeight);
            e.Graphics.DrawString((currentRowIndex + 1).ToString(), fontContent, brush, new RectangleF(x, y, colWidth_STT, rowHeight), sfHeader);
            x += colWidth_STT;

            e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_SKU, dynamicHeight);
            e.Graphics.DrawString(SKU, fontContent, brush, new RectangleF(x, y, colWidth_SKU, rowHeight), sfHeader);
            x += colWidth_SKU;

            e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_MV, dynamicHeight);
            e.Graphics.DrawString(barCodeEAN13, fontContent, brush, new RectangleF(x, y, colWidth_MV, rowHeight), sfHeader);
            x += colWidth_MV;

            e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_Product, dynamicHeight);
            e.Graphics.DrawString(name, fontContent, brush, new RectangleF(x + 10, y, colWidth_Product, dynamicHeight), sfDataLeft);
            x += colWidth_Product;

            e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_QC, dynamicHeight);
            e.Graphics.DrawString(amountPaking, fontContent, brush, new RectangleF(x, y, colWidth_QC, dynamicHeight), sfDataCenter);
            x += colWidth_QC;

            // TotalPCSOther
            int pcs = row.Field<int?>("PCSReal") ?? 0;
            string pcsStr = pcs > 0 ? pcs.ToString() : "";
            e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_SLGH/2, dynamicHeight);
            e.Graphics.DrawString(pcsStr, fontContent, brush, new RectangleF(x, y, colWidth_SLGH/2, rowHeight), sfDataCenter);
            totalPCS += pcs;
            x += colWidth_SLGH/2;

            // TotalNetWeight
            decimal net = row.Field<decimal?>("NWReal") ?? 0m;
            e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_SLGH/2, dynamicHeight);
            e.Graphics.DrawString(net.ToString("N2"), fontContent, brush, new RectangleF(x, y, colWidth_SLGH/2, rowHeight), sfDataCenter);
            totalNetWeight += net;
            x += colWidth_SLGH/2;

            e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_gc, dynamicHeight);
            e.Graphics.DrawString(productTypeName, fontContent, brush, new RectangleF(x, y, colWidth_gc, rowHeight), sfDataCenter);
            y += dynamicHeight;

            // Kiểm tra phân trang

            if (y + rowHeight > e.MarginBounds.Bottom + paddingBottom/2)
            {
                currentRowIndex++;
                yPosition = e.MarginBounds.Top;
                firstPage = false;
                e.HasMorePages = true;
                return;
            }
        }

        y += 40;
        e.Graphics.DrawString("Người nhận hàng\n\n\n\n\n\n\n........................................................", fontHeader, brush, new RectangleF(startX + 40, y, 300, 160), sfDataCenter);
        e.Graphics.DrawString($"Người giao hàng\n\n\n\n\n\n\n{InputByName}", fontHeader, brush, new RectangleF(e.PageBounds.Right - 400, y, 300, 160), sfDataCenter);
        e.HasMorePages = false;
        currentRowIndex = 0;
        firstPage = true;
        yPosition = 0;
    }
}
