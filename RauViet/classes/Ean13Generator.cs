using System;
using System.Drawing;

public static class Ean13Generator
{
    private static readonly string[] LCode =
    {
        "0001101","0011001","0010011","0111101","0100011",
        "0110001","0101111","0111011","0110111","0001011"
    };

    private static readonly string[] GCode =
    {
        "0100111","0110011","0011011","0100001","0011101",
        "0111001","0000101","0010001","0001001","0010111"
    };

    private static readonly string[] RCode =
    {
        "1110010","1100110","1101100","1000010","1011100",
        "1001110","1010000","1000100","1001000","1110100"
    };

    private static readonly string[] Parity =
    {
        "LLLLLL","LLGLGG","LLGGLG","LLGGGL","LGLLGG",
        "LGGLLG","LGGGLL","LGLGLG","LGLGGL","LGGLGL"
    };

    /// <summary>
    /// Tạo ảnh barcode EAN13
    /// </summary>
    public static Bitmap GenerateEAN13(string code)
    {
        if (code.Length == 12)
            code += CalculateChecksum(code);

        if (code.Length != 13)
            throw new Exception("EAN13 must be 12 or 13 digits.");

        string first = code[0].ToString();
        string left = code.Substring(1, 6);
        string right = code.Substring(7, 6);

        string pattern = "101"; // Start

        string p = Parity[int.Parse(first)];

        for (int i = 0; i < 6; i++)
        {
            int n = int.Parse(left[i].ToString());
            pattern += p[i] == 'L' ? LCode[n] : GCode[n];
        }

        pattern += "01010"; // Middle

        for (int i = 0; i < 6; i++)
        {
            int n = int.Parse(right[i].ToString());
            pattern += RCode[n];
        }

        pattern += "101"; // End

        int barWidth = 2;
        int height = 80;
        Bitmap bmp = new Bitmap(pattern.Length * barWidth, height);

        using (Graphics g = Graphics.FromImage(bmp))
        {
            g.Clear(Color.White);
            int x = 0;
            foreach (char c in pattern)
            {
                if (c == '1')
                    g.FillRectangle(Brushes.Black, x, 0, barWidth, height);
                x += barWidth;
            }
        }

        return bmp;
    }

    private static int CalculateChecksum(string code)
    {
        int sum = 0;

        for (int i = 0; i < 12; i++)
        {
            int n = int.Parse(code[i].ToString());
            sum += (i % 2 == 0) ? n : n * 3;
        }

        int mod = sum % 10;
        return (10 - mod) % 10;
    }
}
