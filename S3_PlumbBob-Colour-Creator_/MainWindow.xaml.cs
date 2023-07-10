using System;
using System.Windows;
using System.Windows.Controls;
using Haley.Services;
using System.Drawing;
using Color = System.Drawing.Color;
using System.IO;
using s3pi.Interfaces;
using s3pi.Package;
using s3pi.Extensions;
using Microsoft.Win32;
using System.Diagnostics;
using System.Windows.Navigation;

namespace S3_PlumbBob_Colour_Creator_
{
    public partial class MainWindow : Window
    {
        // Variables
        string xmlPath = @"Assets\moodmanager.xml";
        string confirmationMessage = "Package successfully created!";
        string packageCreateCanceled = "No Package file created";
        string newPackageFilePath = "";
        string rgbFormat;

        bool packageNameEntered;

        public MainWindow()
        {
            InitializeComponent();
            ReloadRGBDisplay();
        }

        // This brings up Colourpicker dialog and lets you choose a colour! It assigns that colour to the button!
        private void colour_picker(object sender, RoutedEventArgs e)
        {
            Button colourButton = (Button)sender;
            ColorPickerDialog colourPicker = new ColorPickerDialog();
            bool? result = colourPicker.ShowDialog();

            if ( result == true )
            {
                colourButton.Background = colourPicker.SelectedBrush;
                ReloadRGBDisplay();
            }
        }

        // When you click the generate button, this is called...
        private void generate_rgb(object sender, RoutedEventArgs e)
        {
            ReloadRGBDisplay();

            // Formats the codes into correct format.
            rgbFormat = GenerateRgb(btn_horrible.Background.ToString()) + ", " + GenerateRgb(btn_bad.Background.ToString()) + ", " +
                                  GenerateRgb(btn_okay.Background.ToString()) + ", " + GenerateRgb(btn_good.Background.ToString());

            // Prompts user to save.
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "PACKAGE Files (*.package)|*.package";
            saveDialog.Title = "Save Package";
            bool? result = saveDialog.ShowDialog();

            // Opens file directory to save using user inputted name.
            if (result == true)
            {
                newPackageFilePath = saveDialog.FileName;
                packageNameEntered = true;
            }
            else
            {
                packageNameEntered = false;
            }

            if(packageNameEntered == true)
            {
                string newLine = "    <kPlumbbobColorRanges value=\"" + rgbFormat + "\">";
                lineChanger(newLine, xmlPath, 70);
                CreatePackage();
            }
            else
            {
                MessageBox.Show(packageCreateCanceled);
            }
        }

        // This converts the 8 digit hex code you get from buttons and changes them to rgb values
        // #00112233 The first 2 digits are alpha followed by R. G. B.
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

        private void ReloadRGBDisplay()
        {
            lbl_horribleCode.Content = "";
            lbl_badCode.Content = "";
            lbl_okayCode.Content = "";
            lbl_goodCode.Content = "";
            lbl_horribleCode.Content = GenerateRgb(btn_horrible.Background.ToString());
            lbl_badCode.Content = GenerateRgb(btn_bad.Background.ToString());
            lbl_okayCode.Content = GenerateRgb(btn_okay.Background.ToString());
            lbl_goodCode.Content = GenerateRgb(btn_good.Background.ToString());
        }

        // This method adds the new custom colours in the moodmanager xml in the new package.
        static void lineChanger(string newText, string fileName, int lineNumber)
        {
            string[] fileLines = File.ReadAllLines(fileName);
            fileLines[lineNumber - 1] = newText;
            File.WriteAllLines(fileName, fileLines);
        }

        public void CreatePackage()
        {
            // create the package
            IPackage NewPackage = Package.NewPackage(0);

            // create resource
            TGIN tgin = new TGIN();
            tgin.ResType = 0x0333406C;
            tgin.ResGroup = 0x00000000;
            tgin.ResInstance = 0x0b8655fc55e8a6a7;
            IResourceKey rk = (TGIBlock)tgin;

            // Reads the moodmanager xml and adds it to a new package file (created from the save file dialog)
            FileStream fs = new FileStream(xmlPath, FileMode.OpenOrCreate, FileAccess.Read);
            MemoryStream ms = new MemoryStream();
            fs.CopyTo(ms);
            IResourceIndexEntry irie = NewPackage.AddResource(rk, ms, true);
            irie.Compressed = 0x5A42;

            // save the package
            NewPackage.SaveAs(newPackageFilePath);
            MessageBox.Show(confirmationMessage);
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process p = new Process();
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.FileName = "https://modthesims.info/d/675999/custom-plumbob-color-tool.html";
            p.Start();
        }

        private void Hyperlink_github(object sender, RequestNavigateEventArgs e)
        {
            Process p = new Process();
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.FileName = "https://github.com/thekdrstory/S3_PlumbBob-Colour-Creator_Project";
            p.Start();
        }
    }
}
