using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace UnitConverter
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void validateTextDouble(object sender)
        {
            Exception X = new Exception();
            TextBox T = (TextBox)sender;
            try
            {
                double x = double.Parse(T.Text.Replace(',', '.'), CultureInfo.InvariantCulture);

                //Customizing Condition (Only numbers larger than or 
                //equal to zero are permitted)
                if (x < 0 || T.Text.Contains(','))
                    throw X;
            }
            catch (Exception)
            {
                try
                {
                    int CursorIndex = T.SelectionStart - 1;
                    T.Text = T.Text.Remove(CursorIndex, 1);

                    //Align Cursor to same index
                    T.SelectionStart = CursorIndex;
                    T.SelectionLength = 0;
                }
                catch (Exception) { }
            }
        }
        private async void validateClipboard(object sender)
        {
            TextBox T = (TextBox)sender;
            double value;
            try
            {
                string val = await Clipboard.GetContent().GetTextAsync();
                if (double.TryParse(val, out value))
                {
                    T.Text = val;
                }
            }
            catch (Exception)
            {
                value = Convert.ToDouble(T.Text);

            }
        }

        int focus = 0;
        TextBox txt = new TextBox();
        List<Unit> unitListForTemp = new List<Unit>();
        List<Unit> unitListForLength = new List<Unit>();
        List<TextBox> textBoxListForTemp = new List<TextBox>();
        List<TextBox> textBoxListForLength = new List<TextBox>();
        Unit unit = new Unit();

        public List<TextBox> AllTextBoxes(DependencyObject parent)
        {
            var list = new List<TextBox>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is TextBox)
                    list.Add(child as TextBox);
                list.AddRange(AllTextBoxes(child));
            }
            return list;
        }

        public static List<Unit> UnitConversion(Unit from, double value, List<Unit> unitList)
        {
            var list = unitList;
            double v = 0.0;


            foreach (var unit in list)
            {
                v = (value - from.Shift) / from.Scale;
                unit.Value = v * unit.Scale + unit.Shift;

            }
            return list;
        }

        public void Calculate(Unit unit, List<Unit> unitList, string text, List<TextBox> textBoxList, TextBox txt)
        {
            double value = 0.0;


            if (txt.Text != string.Empty)
            {
                validateTextDouble(txt);
                validateClipboard(txt);
                if (double.TryParse(text, out value))
                {
                    value = Convert.ToDouble(text);
                    TextBox textbox = new TextBox();
                    var list = UnitConversion(unit, value, unitList);
                    var itemToRemove = list.Where(x => x.Name == unit.Name).FirstOrDefault();
                    list.Remove(itemToRemove);
                    foreach (var item in list)
                    {
                        textbox = textBoxList.Where(x => x.Name == item.TextBoxName).FirstOrDefault();
                        textbox.Text = item.Value.ToString();
                    }
                    list.Add(itemToRemove);
                }
            }
            else if (txt.Text == string.Empty)
            {
                textBoxList.Remove(txt);
                foreach (var textBox in textBoxList)
                {
                    textBox.Text = string.Empty;
                }
            }
        }


        private void txtBox_GotFocus(object sender, RoutedEventArgs e)
        {

            switch (((TextBox)sender).Name)
            {
                case "txtCelcius":
                    focus = 1;
                    break;
                case "txtFahrenheit":
                    focus = 2;
                    break;
                case "txtKelvin":
                    focus = 3;
                    break;
                case "txtKilometer":
                    focus = 4;
                    break;
                case "txtMeter":
                    focus = 5;
                    break;
                case "txtCentimeter":
                    focus = 6;
                    break;
                case "txtMillimeter":
                    focus = 7;
                    break;
                case "txtLeague":
                    focus = 8;
                    break;
                case "txtMile":
                    focus = 9;
                    break;
                case "txtYard":
                    focus = 10;
                    break;
                case "txtInch":
                    focus = 11;
                    break;
            }
        }

        private void txtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            switch (focus)
            {
                case 1:
                    txt = txtCelcius;
                    unit = TemperatureUnit.list.Where(x => x.Name == "Celcius").FirstOrDefault();
                    Calculate(unit, unitListForTemp, txt.Text, textBoxListForTemp, txt);
                    break;
                case 2:
                    txt = txtFahrenheit;
                    unit = TemperatureUnit.list.Where(x => x.Name == "Fahrenheit").FirstOrDefault();
                    Calculate(unit, unitListForTemp, txt.Text, textBoxListForTemp, txt);
                    break;
                case 3:
                    txt = txtKelvin;
                    unit = TemperatureUnit.list.Where(x => x.Name == "Kelvin").FirstOrDefault();
                    Calculate(unit, unitListForTemp, txt.Text, textBoxListForTemp, txt);
                    break;
                case 4:
                    txt = txtKilometer;
                    unit = LengthUnit.list.Where(x => x.Name == "Kilometer").FirstOrDefault();
                    Calculate(unit, unitListForLength, txt.Text, textBoxListForLength, txt);
                    break;
                case 5:
                    txt = txtMeter;
                    unit = LengthUnit.list.Where(x => x.Name == "Meter").FirstOrDefault();
                    Calculate(unit, unitListForLength, txt.Text, textBoxListForLength, txt);
                    break;
                case 6:
                    txt = txtCentimeter;
                    unit = LengthUnit.list.Where(x => x.Name == "Centimeter").FirstOrDefault();
                    Calculate(unit, unitListForLength, txt.Text, textBoxListForLength, txt);
                    break;
                case 7:
                    txt = txtMillimeter;
                    unit = LengthUnit.list.Where(x => x.Name == "Millimeter").FirstOrDefault();
                    Calculate(unit, unitListForLength, txt.Text, textBoxListForLength, txt);
                    break;
                case 8:
                    txt = txtLeague;
                    unit = LengthUnit.list.Where(x => x.Name == "League").FirstOrDefault();
                    Calculate(unit, unitListForLength, txt.Text, textBoxListForLength, txt);
                    break;
                case 9:
                    txt = txtMile;
                    unit = LengthUnit.list.Where(x => x.Name == "Mile").FirstOrDefault();
                    Calculate(unit, unitListForLength, txt.Text, textBoxListForLength, txt);
                    break;
                case 10:
                    txt = txtYard;
                    unit = LengthUnit.list.Where(x => x.Name == "Yard").FirstOrDefault();
                    Calculate(unit, unitListForLength, txt.Text, textBoxListForLength, txt);
                    break;
                case 11:
                    txt = txtInch;
                    unit = LengthUnit.list.Where(x => x.Name == "Inch").FirstOrDefault();
                    Calculate(unit, unitListForLength, txt.Text, textBoxListForLength, txt);
                    break;

            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            unitListForTemp = TemperatureUnit.list;
            textBoxListForTemp = AllTextBoxes(grdTemp);

            unitListForLength = LengthUnit.list;
            textBoxListForLength = AllTextBoxes(grdLength);
        }
    }

    public class Unit
    {
        public string TextBoxName { get; set; }
        public string Name { get; set; }
        public double Scale { get; set; }
        public double Shift { get; set; }
        public string Type { get; set; }
        public double Value { get; set; }
    }

    public class TemperatureUnit
    {
        public static List<Unit> list = new List<Unit>() {
            new Unit() { Name = "Celcius",TextBoxName="txtCelcius", Scale = 1.0, Shift = 0.0, Type = "Temperature",Value=0 },
            new Unit() { Name = "Fahrenheit", TextBoxName="txtFahrenheit",Scale = 1.8, Shift = 32, Type = "Temperature",Value=0 },
            new Unit() { Name = "Kelvin", TextBoxName="txtKelvin",Scale = 1.0, Shift = 273.15, Type = "Temperature" ,Value=0}
    };
    }

    public class LengthUnit
    {
        public static List<Unit> list = new List<Unit>()
        {
            new Unit() { Name ="Kilometer" , TextBoxName="txtKilometer", Scale = 0.001, Shift = 0.0, Type = "Length",Value=0 },
            new Unit() { Name ="Meter" , TextBoxName="txtMeter", Scale = 1, Shift = 0.0, Type = "Length",Value=0 },
            new Unit() { Name ="Centimeter" , TextBoxName="txtCentimeter", Scale = 100, Shift = 0.0, Type = "Length",Value=0 },
            new Unit() { Name ="Millimeter" , TextBoxName="txtMillimeter", Scale = 1000, Shift = 0.0, Type = "Length",Value=0 },
            new Unit() { Name ="League" , TextBoxName="txtLeague", Scale = 0.0002071, Shift = 0.0, Type = "Length",Value=0 },
            new Unit() { Name ="Mile" , TextBoxName="txtMile", Scale = 0.0006214, Shift = 0.0, Type = "Length",Value=0 },
            new Unit() { Name ="Yard" , TextBoxName="txtYard", Scale = 1.094, Shift = 0.0, Type = "Length",Value=0 },
            new Unit() { Name ="Inch" , TextBoxName="txtInch", Scale = 39.37, Shift = 0.0, Type = "Length",Value=0 },
        };
    }

}

