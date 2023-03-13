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

namespace S3_PlumbBob_Colour_Creator_
{
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        // Might change this section to work with GameplayData.package in the future...
        // I want to make this more efficient and require less steps...
        private void open_FileExplorer(object sender, RoutedEventArgs e)
        {
            // Creates OpenFileDialog and sets to search only .xml files
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.DefaultExt = ".xml";
            dialog.Filter = "XML Files (*.xml)|*.xml";

            // Display OpenFileDialog by calling ShowDialog method
            var result = dialog.ShowDialog();

            // Get the selected file name and display in Search Path text box...
            if (result == true)
            {
                txtBox_fileDirectoryPath.Text = dialog.FileName;
            }
        }

        private void colour_picker(object sender, RoutedEventArgs e)
        {
            // This brings up Colourpicker dialog and lets you choose a colour! It assigns that colour to the button!
            Button colourButton = (Button)sender;
            ColorPickerDialog colourPicker = new ColorPickerDialog();
            var result = colourPicker.ShowDialog();

            if ( result == true )
            {
                colourButton.Background = colourPicker.SelectedBrush;
            }
        }

        private void generate_rgb(object sender, RoutedEventArgs e)
        {
            // Clears textbox and generates each of the 4 colour's RGB codes and formats it.
            txtBox_rgbCode.Text = "";
            txtBox_rgbCode.Text = GenerateRgb(btn_horrible.Background.ToString()) + ", " + GenerateRgb(btn_bad.Background.ToString()) 
                + ", " + GenerateRgb(btn_okay.Background.ToString()) + ", " + GenerateRgb(btn_good.Background.ToString());

            // Opens moodmanager(or file) in notepad and edits it's values if applicable...
            if (!string.IsNullOrEmpty(txtBox_fileDirectoryPath.Text))
            {
                string fileName = txtBox_fileDirectoryPath.Text;
                string newLine = "    <kPlumbbobColorRanges value=\"" + txtBox_rgbCode.Text + "\">";
                lineChanger(newLine, fileName, 70);
                MessageBox.Show("Added RGB codes to file!");
            }
            else
            {
                MessageBox.Show("No file provided.\nGiving rgb codes anyways...");
            }
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

        // This method opens the user chosen .xml file and goes to a certain line (hard-coded to 70) and replaces the entire line
        static void lineChanger(string newText, string fileName, int lineNumber)
        {
            string[] fileLines = File.ReadAllLines(fileName);
            fileLines[lineNumber - 1] = newText;
            File.WriteAllLines(fileName, fileLines);
        }

        
    }
}
