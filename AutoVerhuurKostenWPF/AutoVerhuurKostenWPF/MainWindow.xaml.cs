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

namespace AutoVerhuurKostenWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private char[] digits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};
        private DateTime startDate, eindDate;
        private bool personenAuto = true;

        private int kilometers = 0, dagen = 1;
        private float benzinePrijs = 1, prijs = 0;

        public MainWindow()
        {
            InitializeComponent();

            startDate = DateTime.Now;
            eindDate = startDate.AddDays((double)1.0);

            for (int i = 0; i < grid.Children.Count; i++)
            {
                Console.WriteLine("{0}", grid.Children[i].GetType().ToString());
                string typeString = grid.Children[i].GetType().ToString();
                if (typeString.Contains("DatePicker"))
                {
                    DatePicker dp = grid.Children[i] as DatePicker;
                    dp.SelectedDateChanged += OnDateChanged;
                    if (dp.Name.Contains("Start")) dp.SelectedDate = startDate;
                    if (dp.Name.Contains("Eind")) dp.SelectedDate = eindDate;

                }
                else if (typeString.Contains("RadioButton"))
                {
                    RadioButton rb = grid.Children[i] as RadioButton;
                    rb.Checked += onCecked;
                    if (rb.Content.ToString().Contains("Auto")) rb.IsChecked = true;

                } else if (typeString.Contains("TextBox"))
                {
                    TextBox tb = grid.Children[i] as TextBox;
                    tb.TextChanged += OnTextChanged;
                    tb.GotFocus += Tb_GotFocus;
                }

            }

        }

        private void Tb_GotFocus(object sender, RoutedEventArgs e)
        {
            OnTextChanged(sender, e as TextChangedEventArgs);
        }

        private string sanitizeNumberString(string value)
        {
            char[] valueChars = value.ToCharArray();
            value = "";
            bool hasFloatingPoint = false;

            for (int i = 0; i < valueChars.Length; i++)
            {
                if (digits.Contains(valueChars[i])) value += valueChars[i];
                if (valueChars[i].Equals('.') && !hasFloatingPoint)
                {
                    value += valueChars[i];
                    hasFloatingPoint = true;
                }
            }

            if (value == "" || value == null) value = "0";

            return value;
        }

        private float berekenPrijs()
        {
            float benzineKosten = (float)kilometers * benzinePrijs;
            float km = kilometers;

            if (personenAuto)
            {
                if (kilometers - (dagen * 100) > 0) km = kilometers - (dagen * 100);
                else km = 0;
                prijs = (dagen * 50) + (km * (float) 0.2) + benzineKosten;
            }
            else prijs = (dagen * 95) + (km * (float) 0.3) + benzineKosten;

            return prijs;
        }

        private void UpdatePrijsLabel()
        {
            lb_RitPrijs.Content = "\u30CA " + berekenPrijs().ToString();
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb.Name.Contains("Kilometers"))
            {
                kilometers = int.Parse(sanitizeNumberString(tb.Text.ToString()));
                tb_Kilometers.Text = kilometers != 0 ? kilometers.ToString() : "";
            }
            else if (tb.Name.Contains("Prijs"))
            {
                benzinePrijs = float.Parse(sanitizeNumberString(tb.Text.ToString()));
                tb_BenzinePrijs.Text = (float)benzinePrijs > (float) 0 ? benzinePrijs.ToString() : "";
                tb_BenzinePrijs.CaretIndex = tb_BenzinePrijs.Text.Length > 0 ? tb_BenzinePrijs.Text.Length : 0;
            }
            UpdatePrijsLabel();
        }

        private void onCecked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb.Content.ToString().Contains("Auto")) personenAuto = true;
            else personenAuto = false;
            UpdatePrijsLabel();
        }

        private void OnDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dp_Eind.SelectedDate != null && dp_Start.SelectedDate != null)
            {
                eindDate = (DateTime)dp_Eind.SelectedDate;
                startDate = (DateTime)dp_Start.SelectedDate;

                System.TimeSpan ts = eindDate.Subtract(startDate);
                Console.WriteLine("{0}", ts.Days.ToString());
                dagen = ts.Days;
            }
            UpdatePrijsLabel();
        }
    }
}
