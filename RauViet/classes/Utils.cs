using System;
using System.Data;
using System.Windows.Forms;
using DocumentFormat.OpenXml.Wordprocessing;

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

    public static decimal calQuanity(int PCS, decimal NW, string package)
    {
        string packageLower = package.Replace(" ", "").ToLower();
        if (packageLower.CompareTo("kg") == 0)
        {
            return NW;
        }
        return PCS;
    }

    public static string InputDialog(string prompt, string title, string defaultValue = "")
    {
        Form inputForm = new Form()
        {
            Width = 400,
            Height = 150,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            Text = title,
            StartPosition = FormStartPosition.CenterScreen,
            MinimizeBox = false,
            MaximizeBox = false
        };

        Label textLabel = new Label() { Left = 10, Top = 20, Text = prompt, AutoSize = true };
        TextBox textBox = new TextBox() { Left = 10, Top = 50, Width = 360, Text = defaultValue };
        Button okButton = new Button() { Text = "OK", Left = 220, Width = 70, Top = 80, DialogResult = DialogResult.OK };
        Button cancelButton = new Button() { Text = "Cancel", Left = 300, Width = 70, Top = 80, DialogResult = DialogResult.Cancel };

        inputForm.Controls.Add(textLabel);
        inputForm.Controls.Add(textBox);
        inputForm.Controls.Add(okButton);
        inputForm.Controls.Add(cancelButton);
        inputForm.AcceptButton = okButton;
        inputForm.CancelButton = cancelButton;

        return inputForm.ShowDialog() == DialogResult.OK ? textBox.Text : null;
    }
}