using RauViet.ui;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;

public class PhieuDuToanLuong_Printer
{
    private DataRow[] _employees;
    private DataTable allowanceData;
    private int rowIndex = 0; // để phân trang
    private int startX = 0;
    private int lineHeight = 27;
    private int _currentEmployeeIndex = 0;

    public PhieuDuToanLuong_Printer(DataRow[] _employees, DataTable allowanceData)
    {
        this._employees = _employees;
        this.allowanceData = allowanceData;
        _currentEmployeeIndex = 0;
    }

    private void DrawCellText(Graphics g, string text, Font font, Rectangle rect, StringAlignment alignment = StringAlignment.Center)
    {
        StringFormat format = new StringFormat()
        {
            Alignment = alignment,
            LineAlignment = StringAlignment.Center
        };
        g.DrawString(text, font, Brushes.Black, rect, format);
    }

    public void PrintPreview(Form owner)
    {
        rowIndex = 0; // reset trước khi in
        PrintDocument pd = new PrintDocument();
        // A4 (đơn vị: hundredths of an inch)
        pd.DefaultPageSettings.PaperSize = new PaperSize("A4", 827, 1169);
        pd.DefaultPageSettings.Margins = new Margins(40, 40, 40, 40); // ~1cm

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
        // A4 (đơn vị: hundredths of an inch)
        pd.DefaultPageSettings.PaperSize = new PaperSize("A4", 827, 1169);
        pd.DefaultPageSettings.Margins = new Margins(40, 40, 40, 40); // ~1cm
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
        Rectangle bounds = e.MarginBounds;
        int pageWidth = bounds.Width;
        int pageHeight = bounds.Height;

        Font header1Font = new Font("Times New Roman", 12, FontStyle.Bold);
        Font normalFont = new Font("Times New Roman", 12);
        Font headerFont = new Font("Times New Roman", 24, FontStyle.Bold);
        Font header2Font = new Font("Times New Roman", 16, FontStyle.Bold);

        SolidBrush bgBrush_gray = new SolidBrush(Color.LightGray);

        DataRow empInfo = _employees[_currentEmployeeIndex];

        string empCode = empInfo["EmployeeCode"].ToString();
        DataRow[] empAllowancesRows = allowanceData.Select($"EmployeeCode = '{empCode}'");
        int baseSalary = Convert.ToInt32(empInfo["BaseSalary"]);
        int allowance_Insurance = Convert.ToInt32(empInfo["Allowance_Insurance"]);
        int allowance_NonInsurance = Convert.ToInt32(empInfo["Allowance_NonInsurance"]);
        int employeeInsurancePaid = Convert.ToInt32((allowance_Insurance + baseSalary) * 0.105m);
        int thuclanh = baseSalary + allowance_Insurance + allowance_NonInsurance - employeeInsurancePaid;

        int y = bounds.Top;
        int cot1Width = bounds.Width / 2 + 50;
        int cot2Width = bounds.Width / 2 - 50;
        int cot2TextWidth = cot2Width - 15;

        int cot1X = bounds.Left;
        int cot1TextX = cot1X + 15;
        int cot2X = cot1X + cot1Width;
        int totalWidth = cot1Width + cot2Width;

        DrawCellText(g, "PHIẾU DỰ TOÁN LƯƠNG", headerFont, new Rectangle(cot1X, y, totalWidth, lineHeight + 15));
        y += (lineHeight + 15);
        DrawCellText(g, "Dự toán số tiền lương + phụ cấp hàng tháng", header1Font, new Rectangle(cot1X, y, totalWidth, lineHeight));
        y += lineHeight;

        g.DrawRectangle(Pens.Gray, cot1X, y, totalWidth, lineHeight * (empAllowancesRows.Length + 12) + 10);
        g.DrawLine(Pens.Gray, cot2X, y, cot2X, y + lineHeight * 9);
        g.DrawLine(Pens.Gray, cot2X, y + lineHeight * 10, cot2X, y + lineHeight * (empAllowancesRows.Length + 12) + 10);

        for (int i = 0; i < 11 + empAllowancesRows.Length; i++)
        {
            if(i >= 9)
                g.DrawLine(Pens.Gray, cot1X, y + lineHeight * (i + 1) + 10, cot1X + totalWidth, y + lineHeight * (i + 1) + 10);
            else
                g.DrawLine(Pens.Gray, cot1X, y + lineHeight * (i + 1), cot1X + totalWidth, y + lineHeight * (i + 1));
        }

        DrawCellText(g, "Mã nhân viên", normalFont, new Rectangle(cot1TextX, y, cot1Width, lineHeight), StringAlignment.Near);
        DrawCellText(g, empInfo["EmployeeCode"].ToString(), normalFont, new Rectangle(cot2X, y, cot2TextWidth, lineHeight));
        y += lineHeight;        
        DrawCellText(g, "Họ và tên", normalFont, new Rectangle(cot1TextX, y, cot1Width, lineHeight), StringAlignment.Near);
        DrawCellText(g, empInfo["FullName"].ToString(), header1Font, new Rectangle(cot2X, y, cot2TextWidth, lineHeight));
        y += lineHeight;
        DrawCellText(g, "Chức vụ", normalFont, new Rectangle(cot1TextX, y, cot1Width, lineHeight), StringAlignment.Near);
        DrawCellText(g, empInfo["PositionName"].ToString(), normalFont, new Rectangle(cot2X, y, cot2TextWidth, lineHeight));
        y += lineHeight;
        DrawCellText(g, "LƯƠNG CB (1)", header1Font, new Rectangle(cot1TextX, y, cot1Width, lineHeight), StringAlignment.Near);
        DrawCellText(g, baseSalary.ToString("N0"), header1Font, new Rectangle(cot2X, y, cot2TextWidth, lineHeight), StringAlignment.Far);
        y += lineHeight;
        DrawCellText(g, "Phụ cấp đóng BHXH (2)", normalFont, new Rectangle(cot1TextX, y, cot1Width, lineHeight), StringAlignment.Near);
        DrawCellText(g, allowance_Insurance.ToString("N0"), normalFont, new Rectangle(cot2X, y, cot2TextWidth, lineHeight), StringAlignment.Far);
        y += lineHeight;
        DrawCellText(g, "Phụ cấp không BHXH (3)", normalFont, new Rectangle(cot1TextX, y, cot1Width, lineHeight), StringAlignment.Near);
        DrawCellText(g, allowance_NonInsurance.ToString("N0"), normalFont, new Rectangle(cot2X, y, cot2TextWidth, lineHeight), StringAlignment.Far);
        y += lineHeight;
        DrawCellText(g, "Tiền cơ sơ đóng BHXH", normalFont, new Rectangle(cot1TextX, y, cot1Width, lineHeight), StringAlignment.Near);
        DrawCellText(g, (baseSalary + allowance_Insurance).ToString("N0"), normalFont, new Rectangle(cot2X, y, cot2TextWidth, lineHeight), StringAlignment.Far);
        y += lineHeight;
        DrawCellText(g, "NLĐ đóng BHXH (4)", normalFont, new Rectangle(cot1TextX, y, cot1Width, lineHeight), StringAlignment.Near);
        DrawCellText(g, employeeInsurancePaid.ToString("N0"), normalFont, new Rectangle(cot2X, y, cot2TextWidth, lineHeight), StringAlignment.Far);
        y += lineHeight;
        DrawCellText(g, "Lương thực lãnh dự toán (1) + (2) + (3) - (4)", header1Font, new Rectangle(cot1TextX, y, cot1Width, lineHeight), StringAlignment.Near);
        DrawCellText(g, thuclanh.ToString("N0"), header1Font, new Rectangle(cot2X, y, cot2TextWidth, lineHeight), StringAlignment.Far);
        y += lineHeight;

        DrawCellText(g, "Diễn giải các loại phụ cấp", header2Font, new Rectangle(cot1X, y, totalWidth, lineHeight + 10));
        y += (lineHeight + 10);
        DrawCellText(g, "Tên phụ cấp", header1Font, new Rectangle(cot1X, y, cot1Width, lineHeight));
        DrawCellText(g, "Số Tiền", header1Font, new Rectangle(cot2X, y, cot2Width, lineHeight));
        y += lineHeight;

        int totalAllowance = 0;
        foreach(DataRow empAllowancesRow in empAllowancesRows)
        {
            int amount = Convert.ToInt32(empAllowancesRow["Amount"]);
            DrawCellText(g,"Phụ Cấp " + empAllowancesRow["AllowanceName"].ToString(), normalFont, new Rectangle(cot1TextX, y, cot1Width, lineHeight), StringAlignment.Near);
            DrawCellText(g, amount.ToString("N0"), normalFont, new Rectangle(cot2X, y, cot2TextWidth, lineHeight), StringAlignment.Far);
            y += lineHeight;

            totalAllowance += amount;
        }

        DrawCellText(g, "Tổng Phụ Cấp", header1Font, new Rectangle(cot1X, y, cot1Width, lineHeight));
        DrawCellText(g, totalAllowance.ToString("N0"), header1Font, new Rectangle(cot2X, y, cot2TextWidth, lineHeight), StringAlignment.Far);
        y += lineHeight;

        y += lineHeight/3;
        g.DrawRectangle(Pens.Gray, cot1X, y, totalWidth, lineHeight * 6);
        g.FillRectangle(bgBrush_gray, new Rectangle(cot1X + 1, y + lineHeight, totalWidth - 2, lineHeight + 10));

        DrawCellText(g, "Lương theo giờ:", normalFont, new Rectangle(cot1X, y, totalWidth, lineHeight), StringAlignment.Near);
        y += lineHeight;
        DrawCellText(g, (thuclanh / (26 * 8)).ToString("N0") + " đồng/giờ", headerFont, new Rectangle(cot1X, y, totalWidth, lineHeight + 10));
        y += (lineHeight + 10);
        DrawCellText(g, "Chú Ý:", header1Font, new Rectangle(cot1X, y + 5, totalWidth, lineHeight), StringAlignment.Near);
        y += lineHeight;
        DrawCellText(g, "\"Lương thực lãnh dự toán\" là số tiền người lao động nhận được nếu đi làm đủ 26 công và không bị giảm trừ phụ cấp hoặc không được thưởng thêm.", normalFont, new Rectangle(cot1X + 60, y, totalWidth - 80, lineHeight*2), StringAlignment.Near);
        y += lineHeight;


        _currentEmployeeIndex++;

        // 👉 Nếu còn employee thì in tiếp trang mới
        e.HasMorePages = _currentEmployeeIndex < _employees.Length;

    }
}
