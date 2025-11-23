using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

public class LabelPrinter
{
    private string mProductName_EN;
    private string mBotanicalName;
    private string mPackingName;
    private string mBarCodeEAN13;
    private string mArtNr;
    private string mLotCode;
    private string mPackingDay;

    private bool isPreview = false;

    int pageWidth = 0;
    int pageHeight = 0;

    public LabelPrinter(string mProductName_EN, string mBotanicalName, string mPackingName,
                        string mBarCodeEAN13, string mArtNr, string mLotCode, string mPackingDay)
    {
        this.mProductName_EN = mProductName_EN;
        this.mBotanicalName = mBotanicalName;
        this.mPackingName = mPackingName;
        this.mBarCodeEAN13 = mBarCodeEAN13;
        this.mArtNr = mArtNr;
        this.mLotCode = mLotCode;
        this.mPackingDay = mPackingDay;
    }

    // Convert mm to 1/100 inch
    private int MmToHundredths(float mm)
    {
        return (int)(mm / 25.4f * 100f);
    }

    private PrintDocument CreateDocument()
    {
        PrintDocument doc = new PrintDocument();
        doc.PrintController = new StandardPrintController();
        // ⚠️ ĐẶT GIẤY 65mm × 100mm CHÍNH XÁC
        pageWidth = MmToHundredths(65f);
        pageHeight = MmToHundredths(100f);

        // Bắt buộc đặt PaperSize BEFORE print
        doc.DefaultPageSettings.PaperSize = new PaperSize("Label65x100", pageWidth, pageHeight);
        doc.DefaultPageSettings.Landscape = false;
        doc.OriginAtMargins = false;
        doc.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);

        doc.PrintPage += Doc_PrintPage;
        return doc;
    }

    public void Print()
    {
        isPreview = false;
        PrintDocument doc = CreateDocument();

        using (PrintDialog dlg = new PrintDialog())
        {
            dlg.Document = doc;
            if (dlg.ShowDialog() == DialogResult.OK)
                doc.Print();
        }
    }

    public void PrintPreview(Form owner)
    {
        isPreview = true;
        PrintDocument doc = CreateDocument();
        doc.DefaultPageSettings.Landscape = false;
        PrintPreviewDialog preview = new PrintPreviewDialog();
        preview.Document = doc;
        preview.Width = pageWidth;
        preview.Height = pageHeight;

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

    private void Doc_PrintPage(object sender, PrintPageEventArgs e)
    {
        Graphics g = e.Graphics;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

        // ⚠️ DÙNG PaperSize, KHÔNG DÙNG PageBounds (PageBounds sai trên PDF)
        int pageW = pageWidth;// e.PageSettings.PaperSize.Width;
        int pageH = pageHeight;// e.PageSettings.PaperSize.Height;

        if (!isPreview)
        {
            g.TranslateTransform(0, pageH);
            g.RotateTransform(-90);
        }
        // Vẽ khung kiểm tra
      //  g.DrawRectangle(Pens.Black, 0, 0, pageW, pageH);

        Console.WriteLine($"pageWidth={pageWidth}, pageHeight={pageHeight}");
        Console.WriteLine($"Width={pageW}, Height={pageH}");
        e.Graphics.DrawRectangle(Pens.Black, 0, 0, 0, 0);
        // ⚠️ PDF HOẶC PRINTER CÓ HARD MARGIN
        float offsetX = -(pageWidth / 2 + 17);
        float offsetY = 0;
        if (isPreview)
            offsetX = 0;

        // Vẽ hình nền
        if (isPreview)
        {
            Image bg = RauViet.Properties.Resources.vr_label;
            g.DrawImage(bg, -offsetX, -offsetY, pageW, pageH);
        }
        Font bold = new Font("Arial", 20, FontStyle.Bold);
        Font normal = new Font("Arial", 8);
        Font small = new Font("Arial", 7);

        // PRODUCT NAME
        float maxWidth = pageW - 30; // chừa 5px mỗi bên
        float newSize = FitFont(g, mProductName_EN, bold, maxWidth);

        // Font mới đã vừa chiều ngang
        Font scaled = new Font(bold.FontFamily, newSize, bold.Style);

        // Vẽ text
        SizeF sz = g.MeasureString(mProductName_EN, scaled);
        float x = (pageW - sz.Width) / 2 - offsetX;
        float y = pageH / 2 - 50 - offsetY;

        g.DrawString(mProductName_EN, scaled, Brushes.Black, x, y);

        // BotanicalName
        sz = g.MeasureString(mBotanicalName, small);
        x = (pageW - sz.Width) / 2 - offsetX;
        y = pageH / 2 - 10 - offsetY;
        g.DrawString(mBotanicalName, small, Brushes.Black, x, y);

        // ArtNr
        g.DrawString("Art-Nr: " + mArtNr, normal, Brushes.Black,
                     30 - offsetX, pageH / 2 + 5 - offsetY);

        // Lot
        string lotText = "Lot: " + mLotCode;
        SizeF lotSz = g.MeasureString(lotText, normal);
        g.DrawString(lotText, normal, Brushes.Black,
                     pageW - lotSz.Width - 20 - offsetX, pageH / 2 + 5 - offsetY);

        // Gewicht
        string gw = "GEWICHT:    " + mPackingName;
        SizeF gwSz = g.MeasureString(gw, normal);
        g.DrawString(gw, normal, Brushes.Black,
                     (pageW - gwSz.Width) / 2 - 25 - offsetX, pageH * 2 / 3 + 13 - offsetY);

        // PackingDay
        string PackedText = "Packed: " + mPackingDay;
        sz = g.MeasureString(PackedText, normal);
        g.DrawString(PackedText, small, Brushes.Black, 
            20 - offsetX, 27 - offsetY);
        // BARCODE
        if (!string.IsNullOrEmpty(mBarCodeEAN13))
        {
            Bitmap bmp = Ean13Generator.GenerateEAN13(mBarCodeEAN13);

            int bw = 200;
            int bh = 40;

            float bx = (pageW - bw) / 2 - offsetX;
            float by = (pageH * 2 / 3 - bh) - 5 - offsetY;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            g.DrawImage(bmp, bx, by, bw, bh);

            // Numbers under barcode
            Font fNum = new Font("Arial", 8);
            string code = mBarCodeEAN13;
            float cell = bw / (float)code.Length;

            for (int i = 0; i < code.Length; i++)
            {
                string c = code[i].ToString();
                SizeF s = g.MeasureString(c, fNum);

                float cx = bx + i * cell + (cell - s.Width) / 2;
                float cy = by + bh + 2;

                g.DrawString(c, fNum, Brushes.Black, cx, cy);
            }

            bmp.Dispose();
        }

        e.HasMorePages = false;
    }

    private float FitFont(Graphics g, string text, Font original, float maxWidth)
    {
        float fontSize = original.Size;

        while (fontSize > 4)   // không cho nhỏ dưới 4pt
        {
            Font testFont = new Font(original.FontFamily, fontSize, original.Style);
            SizeF size = g.MeasureString(text, testFont);

            if (size.Width <= maxWidth)
                return fontSize;

            fontSize -= 0.5f; // giảm từ từ 0.5pt
        }

        return 4f;
    }

}
