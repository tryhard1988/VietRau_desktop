using System;
using System.Data;
using System.Windows.Forms;

public static class Utils
{

    public static string FormatDate(DateTime dt)
    {
        return dt.ToString("dd/MM/yyyy");
    }

    public static decimal calNetWeight(int PCS, int amount, string packing)
    {
        float realAmount = amount;
        string packingLower = packing.Replace(" ", "").ToLower();
        switch (packingLower)
        {
            case "gr":
                realAmount = amount / 1000.0f;
                break;
        }

        decimal netWeight = Convert.ToDecimal(PCS * realAmount);
        return netWeight;
    }
}