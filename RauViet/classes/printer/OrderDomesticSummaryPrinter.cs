using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

public class OrderDomesticSummaryPrinter
{
    private int currentRowIndex = 0; // chỉ số dòng hiện tại
    private int yPosition = 0;
    private bool firstPage = true;
    private DataTable data;
    private int orderDomesticIndex;
    private DateTime deliveryDate;
    private string customerCode;
    private string company;
    int totalPCS = 0;
    decimal totalNetWeight = 0;
    // ------------------- Hàm Print Preview -------------------
    public void PrintPreview(DataTable dataTable, int orderDomesticIndex, DateTime deliveryDate, string customerCode, string  company,Form owner)
    {
        if (dataTable == null || dataTable.Rows.Count == 0)
            return;

        SetupPrint(dataTable, orderDomesticIndex, deliveryDate, customerCode, company);

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
    public void PrintDirect(DataTable dataTable, int orderDomesticIndex, DateTime deliveryDate, string customerCode, string company, bool isIn2Mat)
    {
        if (dataTable == null || dataTable.Rows.Count == 0)
            return;

        SetupPrint(dataTable, orderDomesticIndex, deliveryDate, customerCode, company);

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

    public void PrintDirectToPdf(DataTable dataTable, int orderDomesticIndex, DateTime deliveryDate, string customerCode, string company)
    {
        if (dataTable == null || dataTable.Rows.Count == 0)
            return;

        SetupPrint(dataTable, orderDomesticIndex, deliveryDate, customerCode, company);

        using (SaveFileDialog saveDlg = new SaveFileDialog())
        {
            saveDlg.Title = "Chọn nơi lưu file PDF";
            saveDlg.Filter = "PDF File|*.pdf";
            saveDlg.FileName = $"Export_{orderDomesticIndex}_{deliveryDate:yyyyMMdd}.pdf";

            if (saveDlg.ShowDialog() != DialogResult.OK)
                return;

            string outputPath = saveDlg.FileName;

            try
            {
                PrintDocument printDoc = new PrintDocument();

                // Gán sự kiện vẽ nội dung
                printDoc.PrintPage += PrintDoc_PrintPage;

                // Thiết lập máy in ảo PDF
                printDoc.PrinterSettings.PrinterName = "Microsoft Print to PDF";
                printDoc.PrinterSettings.PrintToFile = true;
                printDoc.PrinterSettings.PrintFileName = outputPath;

                // Bắt đầu "in" ra PDF
                printDoc.Print();

                DialogResult openResult = MessageBox.Show($"✅ Đã xuất PDF thành công:\n{outputPath}\n\nBạn có muốn mở file ngay không?",
                                        "Hoàn tất", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (openResult == DialogResult.Yes)
                {
                    try
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                        {
                            FileName = outputPath,
                            UseShellExecute = true // mở file theo ứng dụng mặc định
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"❌ Không thể mở file:\n{ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi khi xuất PDF:\n{ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }


    // ------------------- Setup dữ liệu chung -------------------
    private void SetupPrint(DataTable dataTable, int orderDomesticIndex, DateTime deliveryDate, string customerCode, string company)
    {
        this.data = dataTable;
        this.orderDomesticIndex = orderDomesticIndex;
        this.deliveryDate = deliveryDate;
        this.customerCode = customerCode;
        this.company = company;
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
        int rowHeight = 30;

        int colWidth_STT = 50;
        int colWidth_Product = 170;
        int colWidth_pcs = 60;
        int colWidth_nw = 130;
        int colWidth_productType = 130;
        int colWidth_gc = 220;

        int y = startY;

        Font fontTitle = new Font("Times New Roman", 20, FontStyle.Bold);
        Font fontHeader = new Font("Times New Roman", 14, FontStyle.Bold);
        Font fontContent = new Font("Times New Roman", 13);
        Brush brush = Brushes.Black;

        StringFormat sfHeader = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        StringFormat sfDataLeft = new StringFormat() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
        StringFormat sfDataCenter = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

        // Tiêu đề trang đầu
        if (firstPage)
        {
            string title = "TỔNG HỢP ĐƠN HÀNG THEO ĐÓNG GÓI";
            SizeF titleSize = e.Graphics.MeasureString(title, fontTitle);
            float titleX = e.MarginBounds.Left + (e.MarginBounds.Width - titleSize.Width) / 2;
            e.Graphics.DrawString(title, fontTitle, brush, titleX, y);
            y += (int)titleSize.Height + 10 + rowHeight;

            e.Graphics.DrawString($"Mã đơn: {orderDomesticIndex}/{deliveryDate.Year}/{customerCode}", fontHeader, brush, startX, y);
            e.Graphics.DrawString($"Đơn Vị Đặt : {company}", fontHeader, brush, startX + e.PageBounds.Width / 3, y);
            y += rowHeight;

            e.Graphics.DrawString($"Ngày Giao: {deliveryDate:dd/MM/yyyy}", fontContent, brush, startX, y);
            e.Graphics.DrawString($"Ngày Nhận Hàng (ETA): {deliveryDate:dd/MM/yyyy}", fontContent, brush, startX + e.PageBounds.Width/2, y);
            y += rowHeight - 5;

            e.Graphics.DrawString($"Ngày Bắt Đầu SC: {deliveryDate.AddDays(-1):dd/MM/yyyy}", fontContent, brush, startX, y);
            e.Graphics.DrawString($"Ngày Hoàn Thành SC: {deliveryDate.AddDays(-1):dd/MM/yyyy}", fontContent, brush, startX + e.PageBounds.Width / 2, y);
            y += rowHeight * 2;
        }

        // Header bảng
        int x = startX;
        e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_STT, rowHeight);
        e.Graphics.DrawString("STT", fontHeader, brush, new RectangleF(x, y, colWidth_STT, rowHeight), sfHeader);
        x += colWidth_STT;

        e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_Product, rowHeight);
        e.Graphics.DrawString("Tên Sản Phẩm", fontHeader, brush, new RectangleF(x, y, colWidth_Product, rowHeight), sfHeader);
        x += colWidth_Product;

        e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_pcs, rowHeight);
        e.Graphics.DrawString("PCS", fontHeader, brush, new RectangleF(x, y, colWidth_pcs, rowHeight), sfHeader);
        x += colWidth_pcs;

        e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_nw, rowHeight);
        e.Graphics.DrawString("K.Lượng(Kg)", fontHeader, brush, new RectangleF(x, y, colWidth_nw, rowHeight), sfHeader);
        x += colWidth_nw;

        e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_nw, rowHeight);
        e.Graphics.DrawString("Loại Hàng", fontHeader, brush, new RectangleF(x, y, colWidth_productType, rowHeight), sfHeader);
        x += colWidth_productType;

        e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_gc, rowHeight);
        e.Graphics.DrawString("Ghi chú", fontHeader, brush, new RectangleF(x, y, colWidth_gc, rowHeight), sfHeader);
        y += rowHeight;
        
        // ------------------ Dữ liệu ------------------
        

        for (; currentRowIndex < data.Rows.Count; currentRowIndex++)
        {
            var row = data.Rows[currentRowIndex];

            x = startX;

            // ProductPackingName
            string name = row["ProductNameVN"]?.ToString() ?? "";
            RectangleF rect = new RectangleF(x, y, colWidth_Product, 9999);
            SizeF textSize = e.Graphics.MeasureString(name,fontContent,(int)colWidth_Product, StringFormat.GenericDefault);
            int dynamicHeight = Math.Max(rowHeight, (int)textSize.Height);

            // STT
            e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_STT, dynamicHeight);
            e.Graphics.DrawString((currentRowIndex + 1).ToString(), fontContent, brush, new RectangleF(x, y, colWidth_STT, rowHeight), sfHeader);
            x += colWidth_STT;

            e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_Product, dynamicHeight);
            e.Graphics.DrawString(name, fontContent, brush, new RectangleF(x, y, colWidth_Product, dynamicHeight), sfDataLeft);
            x += colWidth_Product;

            // TotalPCSOther
            int pcs = Convert.ToInt32(row["PCSOrder"] ?? 0);
            string pcsStr = pcs > 0 ? pcs.ToString() : "";
            e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_pcs, dynamicHeight);
            e.Graphics.DrawString(pcsStr, fontContent, brush, new RectangleF(x, y, colWidth_pcs, rowHeight), sfDataCenter);
            totalPCS += pcs;
            x += colWidth_pcs;

            // TotalNetWeight
            decimal net = Convert.ToDecimal(row["NWOrder"] ?? 0);
            e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_nw, dynamicHeight);
            e.Graphics.DrawString(net.ToString("N2"), fontContent, brush, new RectangleF(x, y, colWidth_nw, rowHeight), sfDataCenter);
            totalNetWeight += net;
            x += colWidth_nw;

            string productTypeName = row["ProductTypeName"].ToString();
            e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_nw, dynamicHeight);
            e.Graphics.DrawString(productTypeName, fontContent, brush, new RectangleF(x, y, colWidth_productType, rowHeight), sfDataCenter);
            x += colWidth_productType;

            e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_gc, dynamicHeight);

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

        // Tổng cuối bảng
        x = startX + colWidth_STT;

        e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_Product, rowHeight);
        e.Graphics.DrawString("Tổng", fontHeader, brush, new RectangleF(x, y, colWidth_Product, rowHeight), sfDataCenter);
        x += colWidth_Product;

        e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_pcs, rowHeight);
        e.Graphics.DrawString(totalPCS.ToString(), fontHeader, brush, new RectangleF(x, y, colWidth_pcs, rowHeight), sfDataCenter);
        x += colWidth_pcs;

        e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_nw, rowHeight);
        e.Graphics.DrawString(totalNetWeight.ToString("N2"), fontHeader, brush, new RectangleF(x, y, colWidth_nw, rowHeight), sfDataCenter);

        e.HasMorePages = false;
        currentRowIndex = 0;
        firstPage = true;
        yPosition = 0;
    }
}
