using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Haley.WPF;
using Haley.Services;
using Haley.Enums;
using Haley.Utils;
using MediaColor = System.Windows.Media.Color;
using System.Globalization;
using System.Drawing;
using Color = System.Drawing.Color;
using System.Diagnostics;
using System.IO;
using s3pi.Interfaces;
using s3pi.Package;
using s3pi.Extensions;
using Microsoft.Win32;

namespace S3_PlumbBob_Colour_Creator_
{
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        // This brings up Colourpicker dialog and lets you choose a colour! It assigns that colour to the button!
        private void colour_picker(object sender, RoutedEventArgs e)
        {
            Button colourButton = (Button)sender;
            ColorPickerDialog colourPicker = new ColorPickerDialog();
            var result = colourPicker.ShowDialog();

            if ( result == true )
            {
                colourButton.Background = colourPicker.SelectedBrush;
            }
        }

        // When you click the generate button, this is called...
        private void generate_rgb(object sender, RoutedEventArgs e)
        {
            string xmlPath = @"Assets\moodmanager.xml";

            // Clears textbox and generates each of the 4 colour's RGB codes and formats it.
            txtBox_rgbCode.Text = "";
            txtBox_rgbCode.Text = GenerateRgb(btn_horrible.Background.ToString()) + ", " + GenerateRgb(btn_bad.Background.ToString()) 
                + ", " + GenerateRgb(btn_okay.Background.ToString()) + ", " + GenerateRgb(btn_good.Background.ToString());

            // Prompts user to save.
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "PACKAGE Files (*.package)|*.package";
            dialog.Title = "Save Package";
            var result = dialog.ShowDialog();
            string newPackageFilePath = "";

            // Opens file directory to save using user inputted name.
            if (result == true)
            {
                newPackageFilePath = dialog.FileName;
            }

            // Calls lineChanger Method below...
            string newLine = "    <kPlumbbobColorRanges value=\"" + txtBox_rgbCode.Text + "\">";
            lineChanger(newLine, xmlPath, 70);
            MessageBox.Show("Added RGB codes to file!");

            // create the package
            IPackage NewPackage = Package.NewPackage(0);

            // create resource
            TGIN tgin = new TGIN();
            tgin.ResType = 0x0333406C;
            tgin.ResGroup = 0x00000000;
            tgin.ResInstance = 0x0b8655fc55e8a6a7;
            IResourceKey rk = (TGIBlock)tgin;

            // Reads the moodmanager xml and adds it to a new package file (created from the save file dialog.
            FileStream fs = new FileStream(xmlPath, FileMode.OpenOrCreate, FileAccess.Read);
            MemoryStream ms = new MemoryStream();
            fs.CopyTo(ms);
            IResourceIndexEntry irie = NewPackage.AddResource(rk, ms, true);
            irie.Compressed = 0x5A42;

            // save the package
            NewPackage.SaveAs(newPackageFilePath);

        }

        // This converts the 8 digit hex code you get from buttons and changes them to rgb values
        // #00112233 The first 2 digits are alpha... Remove them. Followed by R G B.
        public string GenerateRgb(string bg)
        {
            Color color = ColorTranslator.FromHtml(bg);
            double r = Convert.ToInt16(color.R);
            double g = Convert.ToInt16(color.G);
            double b = Convert.ToInt16(color.B);
            double sr = r / 255;
            double sg = g / 255;
            double sb = b / 255;
            return string.Format("{0:N2}, {1:N2}, {2:N2}", sr, sg, sb);
        }

        // This method adds the new custom colours in the moodmanager xml in the new package.
        static void lineChanger(string newText, string fileName, int lineNumber)
        {
            string[] fileLines = File.ReadAllLines(fileName);
            fileLines[lineNumber - 1] = newText;
            File.WriteAllLines(fileName, fileLines);
        }

        
    }
}
