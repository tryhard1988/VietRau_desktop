using RauViet.classes;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

public class DoiChieuCongNo_Printer
{
    private int currentRowIndex = 0; // chỉ số dòng hiện tại
    private int yPosition = 0;
    private bool firstPage = true;
    private bool firstRowData = true;
    private DataTable data;
    private string company;
    private string customerName;
    private string customerAddress;
    private string email;
    private string taxCode;
    int totalPCS = 0;
    decimal totalNetWeight = 0;
    int TotalAmount = 0;
    // ------------------- Hàm Print Preview -------------------, this);
    public void PrintPreview(DataTable dataTable, string company, string customerName, string customerAddress, string email, string taxCode, Form owner)
    {
        if (dataTable == null || dataTable.Rows.Count == 0)
            return;

        SetupPrint(dataTable, company, customerName, customerAddress, email, taxCode);

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
    public void PrintDirect(DataTable dataTable, string company, string customerName, string customerAddress, string email, string taxCode, bool isIn2Mat)
    {
        if (dataTable == null || dataTable.Rows.Count == 0)
            return;

        SetupPrint(dataTable, company, customerName, customerAddress, email, taxCode);

        PrintDocument printDoc = new PrintDocument();
        printDoc.PrintPage += PrintDoc_PrintPage;
        printDoc.PrinterSettings.Duplex = isIn2Mat ? Duplex.Vertical : Duplex.Simplex;

        // Hiển thị hộp thoại chọn máy in
        using (PrintDialog printDialog = new PrintDialog())
        {
            printDialog.Document = printDoc;
            printDialog.AllowSomePages = true; // cho phép chọn trang
            printDialog.AllowSelection = true;  // cho phép chọn lựa
            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                printDoc.Print(); // in ra máy in được chọn
            }
        }
    }

    // ------------------- Setup dữ liệu chung -------------------
    private void SetupPrint(DataTable dataTable, string company, string customerName, string customerAddress, string email, string taxCode)
    {
        this.data = dataTable;
        this.company = company;
        this.customerName = customerName;
        this.customerAddress = customerAddress;
        this.email = email;
        this.taxCode = taxCode;

        currentRowIndex = 0;
        firstPage = true;
        yPosition = 0;
        totalPCS = 0;
        totalNetWeight = 0;
    }

    // ------------------- Hàm PrintPage -------------------
    private void PrintDoc_PrintPage(object sender, PrintPageEventArgs e)
    {
        if (data.Rows.Count == 0) return;

        int paddingBottom = e.PageBounds.Bottom - e.MarginBounds.Bottom;
        int startX = 50;
        int startY = firstPage ? 50 : yPosition;
        int rowHeight = 27;

        int colWidth_STT = 35;
        int colWidth_DeliveryDate = 85;
        int colWidth_SKU = 55;
        int colWidth_ProductType = 105;
        int colWidth_Product = 115;
        int colWidth_Package = 40;
        int colWidth_Price = 65;
        int colWidth_TotalAmount = 85;
        int colWidth_SLGH = 90;
        int colWidth_Code = e.PageBounds.Width - colWidth_STT - colWidth_DeliveryDate - colWidth_SKU - colWidth_ProductType - colWidth_Product - 
            colWidth_Package - colWidth_Price - colWidth_TotalAmount - colWidth_SLGH - startX*2 - 30;

        int y = startY;

        Font fontTitle1 = new Font("Times New Roman", 16, FontStyle.Bold);
        Font fontTitle = new Font("Times New Roman", 16, FontStyle.Bold);
        Font fontHeader = new Font("Times New Roman", 11, FontStyle.Bold);
        Font fontHeader1 = new Font("Times New Roman", 13, FontStyle.Bold);
        Font fontContent = new Font("Times New Roman", 11);
        Font fontContent1 = new Font("Times New Roman", 14);
        Font fontContent2 = new Font("Times New Roman", 10, FontStyle.Italic);
        System.Drawing.Brush brush = System.Drawing.Brushes.Black;
        System.Drawing.Brush brush1 = new SolidBrush(System.Drawing.Color.FromArgb(198, 89, 17));
        System.Drawing.Brush headerBackBrush = new SolidBrush(System.Drawing.Color.FromArgb(248, 203, 173));

        StringFormat sfHeader = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        StringFormat sfDataLeft = new StringFormat() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
        StringFormat sfDataRight = new StringFormat() { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };
        StringFormat sfDataCenter = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

        DateTime deliveryDate0 = Convert.ToDateTime(data.Rows[0]["DeliveryDate"]);
        // Tiêu đề trang đầu
        if (firstPage)
        {
            Image img = RauViet.Properties.Resources.ic_logo;
            e.Graphics.DrawImage(img, e.MarginBounds.Left, y - rowHeight / 3, (int)(img.Width * 0.15), (int)(img.Height * 0.15));
            y -= rowHeight / 2;

            // vùng in (giữa trang)
            System.Drawing.Rectangle printRect = new System.Drawing.Rectangle(e.MarginBounds.Left + 10, y, e.MarginBounds.Width, rowHeight + 10);

            e.Graphics.DrawString("CÔNG TY CỔ PHẦN VIỆT RAU", fontTitle1, brush1, printRect, sfDataCenter);
            y += rowHeight - 5;

            printRect.Y = y;
            e.Graphics.DrawString("MST : 0313983703", fontContent2, brush1, printRect, sfDataCenter);
            y += rowHeight - 7;

            printRect.Y = y;
            e.Graphics.DrawString("Địa chỉ: Tổ 1, Ấp 4, Xã Phước Thái, Tỉnh Đồng Nai, Việt Nam", fontContent2, brush1, printRect, sfDataCenter);
            y += rowHeight - 7;

            printRect.Y = y;
            e.Graphics.DrawString("Email: Acc@vietrau.com      ĐT: 0251 2860828 / 0909244916", fontContent2, brush1, printRect, sfDataCenter);
            y += (rowHeight*2);

            // kẻ line giữa
            e.Graphics.DrawLine(Pens.Black, startX, y, e.PageBounds.Width - startX, y);
            e.Graphics.DrawLine(Pens.Black, startX, y + 2, e.PageBounds.Width - startX, y + 2);
            y += 10;

            string title = "BẢNG KÊ CHI TIẾT HÀNG BÁN HÀNG THÁNG " + deliveryDate0.Month + "/" + deliveryDate0.Year;
            SizeF titleSize = e.Graphics.MeasureString(title, fontTitle);
            float titleX = e.MarginBounds.Left + (e.MarginBounds.Width - titleSize.Width) / 2;
            e.Graphics.DrawString(title, fontTitle, brush, titleX, y);
            y += rowHeight*2;

            e.Graphics.DrawString($"Tên đơn vị:", fontContent, brush, startX, y + 2);
            e.Graphics.DrawString($"{company}", fontHeader1, brush, startX + 100, y);
            e.Graphics.DrawString($"MST:", fontContent, brush, startX + e.MarginBounds.Width - 200, y + 2);
            e.Graphics.DrawString($"{taxCode}", fontHeader1, brush, startX + e.MarginBounds.Width - 130, y);
            y += (rowHeight - 3);
            e.Graphics.DrawString($"Địa chỉ:", fontContent, brush, startX, y + 2);
            e.Graphics.DrawString($"{customerAddress}", fontHeader1, brush, startX + 100, y);
            y += (rowHeight - 3);
            e.Graphics.DrawString($"Liên hệ:", fontContent, brush, startX, y + 2);
            e.Graphics.DrawString($"{customerName}", fontHeader1, brush, startX + 100, y);
            e.Graphics.DrawString($"Email:", fontContent, brush, startX + e.MarginBounds.Width - 200, y + 2);
            e.Graphics.DrawString($"{email}", fontHeader1, brush, startX + e.MarginBounds.Width - 130, y);
            y += (rowHeight - 3);
        }
        y += 10;
        // Header bảng
        int x = startX;
        System.Drawing.Rectangle rectSTT = new System.Drawing.Rectangle(x, y, colWidth_STT, rowHeight * 2);
        e.Graphics.FillRectangle(headerBackBrush, rectSTT);
        e.Graphics.DrawRectangle(Pens.Black, rectSTT);
        e.Graphics.DrawString("STT", fontHeader, brush, rectSTT, sfHeader);
        x += colWidth_STT;

        System.Drawing.Rectangle rectDeliveryDate = new System.Drawing.Rectangle(x, y, colWidth_DeliveryDate, rowHeight * 2);
        e.Graphics.FillRectangle(headerBackBrush, rectDeliveryDate);
        e.Graphics.DrawRectangle(Pens.Black, rectDeliveryDate);
        e.Graphics.DrawString("Ngày Xuất", fontHeader, brush, rectDeliveryDate, sfHeader);
        x += colWidth_DeliveryDate;

        System.Drawing.Rectangle rectCode = new System.Drawing.Rectangle(x, y, colWidth_Code, rowHeight * 2);
        e.Graphics.FillRectangle(headerBackBrush, rectCode);
        e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_Code, rowHeight * 2);
        e.Graphics.DrawString("Số Phiếu", fontHeader, brush, rectCode, sfHeader);
        x += colWidth_Code;

        System.Drawing.Rectangle rectSKU = new System.Drawing.Rectangle(x, y, colWidth_SKU, rowHeight);
        e.Graphics.FillRectangle(headerBackBrush, rectSKU);
        e.Graphics.DrawRectangle(Pens.Black, rectSKU);

        e.Graphics.DrawString("SKU", fontHeader, brush, rectSKU, sfHeader);
        rectSKU = new System.Drawing.Rectangle(x, y + rowHeight, colWidth_SKU, rowHeight);
        e.Graphics.FillRectangle(headerBackBrush, rectSKU);
        e.Graphics.DrawRectangle(Pens.Black, rectSKU);
        e.Graphics.DrawString("Art.Nr", fontHeader, brush, rectSKU, sfHeader);
        x += colWidth_SKU;

        System.Drawing.Rectangle rectProduct = new System.Drawing.Rectangle(x, y, colWidth_Product, rowHeight * 2);
        e.Graphics.FillRectangle(headerBackBrush, rectProduct);
        e.Graphics.DrawRectangle(Pens.Black, rectProduct);
        e.Graphics.DrawString("Tên Sản Phẩm", fontHeader, brush, rectProduct, sfHeader);
        x += colWidth_Product;

        System.Drawing.Rectangle rectProductType = new System.Drawing.Rectangle(x, y, colWidth_ProductType, rowHeight * 2);
        e.Graphics.FillRectangle(headerBackBrush, rectProductType);
        e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_ProductType, rowHeight * 2);
        e.Graphics.DrawString("Loại Hàng", fontHeader, brush, rectProductType, sfHeader);
        x += colWidth_ProductType;

        System.Drawing.Rectangle rectPackage = new System.Drawing.Rectangle(x, y, colWidth_Package, rowHeight * 2);
        e.Graphics.FillRectangle(headerBackBrush, rectPackage);
        e.Graphics.DrawRectangle(Pens.Black, rectPackage);
        e.Graphics.DrawString("Đ.Vị", fontHeader, brush, rectPackage, sfHeader);
        x += colWidth_Package;

        System.Drawing.Rectangle rectSLGH = new System.Drawing.Rectangle(x, y, colWidth_SLGH, rowHeight);
        e.Graphics.FillRectangle(headerBackBrush, rectSLGH);
        e.Graphics.DrawRectangle(Pens.Black, rectSLGH);
        e.Graphics.DrawString("Số Lượng", fontHeader, brush, rectSLGH, sfHeader);

        rectSLGH = new System.Drawing.Rectangle(x, y + rowHeight, colWidth_SLGH / 2, rowHeight);
        e.Graphics.FillRectangle(headerBackBrush, rectSLGH);
        e.Graphics.DrawRectangle(Pens.Black, rectSLGH);
        e.Graphics.DrawString("Gói", fontHeader, brush, rectSLGH, sfHeader);
        x += colWidth_SLGH / 2;

        rectSLGH = new System.Drawing.Rectangle(x, y + rowHeight, colWidth_SLGH/2, rowHeight);
        e.Graphics.FillRectangle(headerBackBrush, rectSLGH);
        e.Graphics.DrawRectangle(Pens.Black, rectSLGH);
        e.Graphics.DrawString("Kg", fontHeader, brush, rectSLGH, sfHeader);
        x += colWidth_SLGH / 2;

        System.Drawing.Rectangle rectPrice = new System.Drawing.Rectangle(x, y, colWidth_Price, rowHeight * 2);
        e.Graphics.FillRectangle(headerBackBrush, rectPrice);
        e.Graphics.DrawRectangle(Pens.Black, rectPrice);
        e.Graphics.DrawString("Đơn Giá", fontHeader, brush, rectPrice, sfHeader);
        x += colWidth_Price;

        System.Drawing.Rectangle rectTotalAmount = new System.Drawing.Rectangle(x, y, colWidth_TotalAmount, rowHeight * 2);
        e.Graphics.FillRectangle(headerBackBrush, rectTotalAmount);
        e.Graphics.DrawRectangle(Pens.Black, rectTotalAmount);
        e.Graphics.DrawString("Thành\nTiền", fontHeader, brush, rectTotalAmount, sfHeader);
        x += colWidth_TotalAmount;

        
        y += (rowHeight * 2);

        // ------------------ Dữ liệu ------------------
                
        for (; currentRowIndex < data.Rows.Count; currentRowIndex++)
        {
            var row = data.Rows[currentRowIndex];
            x = startX;

            DateTime currentDate = row.Field<DateTime>("DeliveryDate").Date;

            bool isFirstRowOfGroup = currentRowIndex == 0 || data.Rows[currentRowIndex - 1].Field<DateTime>("DeliveryDate").Date != currentDate;

            int tempIndex = currentRowIndex;
            int groupHeight = 0;

            while (tempIndex < data.Rows.Count)
            {
                var tempRow = data.Rows[tempIndex];
                DateTime tempDate = tempRow.Field<DateTime>("DeliveryDate").Date;

                if (tempDate != currentDate)
                    break;

                // Tính dynamicHeight của từng row trong group
                string tempName = Regex.Replace(tempRow["ProductNameVN"].ToString(),  @"\s*\d+\s*(g|gr|kg)\b", "", RegexOptions.IgnoreCase).Trim();
                SizeF size = e.Graphics.MeasureString(tempName, fontContent, (int)colWidth_Product);

                int tempHeight = Math.Max(rowHeight, (int)size.Height);
                groupHeight += tempHeight;

                if (y + groupHeight > e.MarginBounds.Bottom + paddingBottom / 2)
                {
                    groupHeight -= tempHeight;
                    break;
                }

                tempIndex++;
            }

            if (isFirstRowOfGroup && y + groupHeight > e.MarginBounds.Bottom + paddingBottom / 2)
            {
                yPosition = e.MarginBounds.Top;
                firstPage = false;
                e.HasMorePages = true;
                firstRowData = true;
                return;
            }

            string SKU = row["SKU"].ToString();
            string name = Regex.Replace(row["ProductNameVN"].ToString(), @"\s*\d+\s*(g|gr|kg)\b", "",RegexOptions.IgnoreCase).Trim();
            string productTypeName = row["ProductTypeName"].ToString();
            string package = row["Package"].ToString();
            DateTime DeliveryDate = Convert.ToDateTime(row["DeliveryDate"]);
            int price = Convert.ToInt32(row["Price"]);
            int totalAmount = Convert.ToInt32(row["TotalAmount"]);
            int orderDomesticIndex = Convert.ToInt32(row["OrderDomesticIndex"]);

            RectangleF rect = new RectangleF(x, y, colWidth_Product, 9999);
            SizeF textSize = e.Graphics.MeasureString(name,fontContent,(int)colWidth_Product, StringFormat.GenericDefault);
            int dynamicHeight = Math.Max(rowHeight, (int)textSize.Height);

            // STT
            e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_STT, dynamicHeight);
            e.Graphics.DrawString((currentRowIndex + 1).ToString(), fontContent, brush, new RectangleF(x, y, colWidth_STT, rowHeight), sfHeader);
            x += colWidth_STT;

            if (firstRowData ||currentRowIndex == 0 || data.Rows[currentRowIndex - 1].Field<DateTime>("DeliveryDate").Date != currentDate)
            {
                e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_DeliveryDate, groupHeight);
                e.Graphics.DrawString(currentDate.ToString("dd/MM/yyyy"), fontContent, brush, new RectangleF(x, y, colWidth_DeliveryDate, groupHeight), sfHeader);

            }
            x += colWidth_DeliveryDate;

            if (firstRowData ||currentRowIndex == 0 || data.Rows[currentRowIndex - 1].Field<DateTime>("DeliveryDate").Date != currentDate)
            {
                e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_Code, groupHeight);
                e.Graphics.DrawString(orderDomesticIndex.ToString(), fontContent, brush, new RectangleF(x, y, colWidth_Code, groupHeight), sfHeader);

            }
            x += colWidth_Code;

            e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_SKU, dynamicHeight);
            e.Graphics.DrawString(SKU, fontContent, brush, new RectangleF(x, y, colWidth_SKU, rowHeight), sfHeader);
            x += colWidth_SKU;

            e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_Product, dynamicHeight);
            e.Graphics.DrawString(name, fontContent, brush, new RectangleF(x + 10, y, colWidth_Product, dynamicHeight), sfDataLeft);
            x += colWidth_Product;

            e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_ProductType, dynamicHeight);
            e.Graphics.DrawString(productTypeName, fontContent, brush, new RectangleF(x + 5, y, colWidth_ProductType, dynamicHeight), sfDataLeft);
            x += colWidth_ProductType;

            
            e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_Package, dynamicHeight);
            e.Graphics.DrawString(package.CompareTo("weight") == 0 ? "kg" : package, fontContent, brush, new RectangleF(x, y, colWidth_Package, dynamicHeight), sfDataCenter);
            x += colWidth_Package;

            // TotalPCSOther
            int pcs = row.Field<int?>("PCSReal") ?? 0;
            string pcsStr = pcs > 0 ? pcs.ToString() : "";
            e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_SLGH / 2, dynamicHeight);
            e.Graphics.DrawString(pcsStr, fontContent, brush, new RectangleF(x, y, colWidth_SLGH / 2, rowHeight), sfDataCenter);
            totalPCS += pcs;
            x += colWidth_SLGH / 2;

            // TotalNetWeight
            decimal net = row.Field<decimal?>("NWReal") ?? 0m;
            e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_SLGH / 2, dynamicHeight);
            e.Graphics.DrawString(net.ToString("N2"), fontContent, brush, new RectangleF(x, y, colWidth_SLGH / 2, rowHeight), sfDataCenter);
            totalNetWeight += net;
            x += colWidth_SLGH / 2;

            e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_Price, dynamicHeight);
            e.Graphics.DrawString(price.ToString("N0"), fontContent, brush, new RectangleF(x, y, colWidth_Price, dynamicHeight), sfDataRight);
            x += colWidth_Price;

            e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_TotalAmount, dynamicHeight);
            e.Graphics.DrawString(totalAmount.ToString("N0"), fontContent, brush, new RectangleF(x, y, colWidth_TotalAmount, dynamicHeight), sfDataRight);
            this.TotalAmount += totalAmount;
            x += colWidth_TotalAmount; 


            y += dynamicHeight;
            firstRowData = false;


            if (y + rowHeight > e.MarginBounds.Bottom + paddingBottom / 2)
            {
                currentRowIndex++;
                yPosition = e.MarginBounds.Top;
                firstPage = false;//
                e.HasMorePages = true;
                firstRowData = true;
                return;
            }
        }

        float width_totalAmount = colWidth_STT + colWidth_DeliveryDate + colWidth_SKU + colWidth_ProductType + colWidth_Product +
            colWidth_Package + colWidth_Price + colWidth_SLGH + colWidth_Code;
        e.Graphics.DrawRectangle(Pens.Black, startX, y, width_totalAmount, 25);
        e.Graphics.DrawString("TỔNG CỘNG", fontHeader, brush, new RectangleF(startX, y, width_totalAmount, 25), sfDataCenter);

        e.Graphics.DrawRectangle(Pens.Black, startX + width_totalAmount, y, colWidth_TotalAmount, 25);
        e.Graphics.DrawString(TotalAmount.ToString("N0"), fontHeader, brush, new RectangleF(startX + width_totalAmount, y, colWidth_TotalAmount, 25), sfDataCenter);

        if (y + 150 > e.MarginBounds.Bottom + paddingBottom)
        {
            currentRowIndex++;
            yPosition = e.MarginBounds.Top;
            firstPage = false;//
            e.HasMorePages = true;
            firstRowData = true;
            return;
        }

        y += 10;
        DateTime lastDayOfMonth = new DateTime(deliveryDate0.Year, deliveryDate0.Month, DateTime.DaysInMonth(deliveryDate0.Year, deliveryDate0.Month));
        e.Graphics.DrawString($"Ngày {lastDayOfMonth.Day} tháng {lastDayOfMonth.Month} năm {lastDayOfMonth.Year}\nNgười Lập Bảng\n\n\n\n{UserManager.Instance.fullName}", fontHeader, brush, new RectangleF(e.PageBounds.Right - 400, y, 400, 160), sfDataCenter);
        e.HasMorePages = false;
        currentRowIndex = 0;
        firstPage = true;
        yPosition = 0;
    }
}
