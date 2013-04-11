// This sample demonstrates how to create a file using the Microsoft Excel 
// Binary Interchange File Format (BIFF). 
// If this program works, it was written by Serhiy Perevoznyk.
// If not, I don't know who wrote it.

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;

namespace XLSExportDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            
            ExcelDocument document = new ExcelDocument();
            document.UserName = "Perevoznyk";
            document.CodePage = CultureInfo.CurrentCulture.TextInfo.ANSICodePage;

            document.ColumnWidth(0, 120);
            document.ColumnWidth(1, 80);
            
            document[0, 0].Value = "ExcelWriter Demo";
            document[0 ,0].Font = new System.Drawing.Font("Tahoma", 10, System.Drawing.FontStyle.Bold);
            document[0, 0].ForeColor = ExcelColor.DarkRed;
            document[0, 0].Alignment = Alignment.Centered;
            document[0, 0].BackColor = ExcelColor.Silver;

            document.WriteCell(1, 0, "int");
            document.WriteCell(1, 1, 10);

            document.Cell(2, 0).Value = "double";
            document.Cell(2, 1).Value = 1.5;

            document.Cell(3, 0).Value = "date";
            document.Cell(3, 1).Value = DateTime.Now;
            document.Cell(3, 1).Format = @"dd/mm/yyyy";

            FileStream stream = new FileStream("demo.xls", FileMode.Create);

            document.Save(stream);
            stream.Close();
        }
    }
}
