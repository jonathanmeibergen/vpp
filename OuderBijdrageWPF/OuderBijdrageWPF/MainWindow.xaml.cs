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

namespace OuderBijdrageWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int aKindJD10 = 0;
        private int aKindOD10 = 0;
        private bool aOuder = false;
        private char[] digits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', };
        //List<RadioButton> rb_KindJD10_List = new List<RadioButton>();
        //List<RadioButton> rb_KindOD10_List = new List<RadioButton>();

        public MainWindow()
        {
            InitializeComponent();
            lb_toeslag.Content = "\u20AC 50";

            for (int i = 0; i < grid.Children.Count; i++)
            {
                //Console.WriteLine("{0}", grid.Children[i].GetType().ToString());
                if (grid.Children[i].GetType().ToString().Contains("RadioButton"))
                {
                    RadioButton rb = grid.Children[i] as RadioButton;
                    if (rb.Content.ToString().Equals("geen") || rb.Content.ToString().Equals("nee")) rb.IsChecked=true;
                    if (rb.GroupName.Equals("rbg_aKindJD10")) rb.Checked += Rb_KindJD10_Checked;//rb_KindJD10_List.Add(rb);
                    if (rb.GroupName.Equals("rbg_aKindOD10")) rb.Checked += Rb_KindOD10_Checked;//rb_KindOD10_List.Add(rb);
                    if (rb.GroupName.Equals("rbg_aOuder")) rb.Checked += Rb_aOuder_Checked;//rb_KindOD10_List.Add(rb);

                }
            }
        }

        private void Rb_aOuder_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb.Content.ToString().Equals("ja")) aOuder = true;
            else aOuder = false;
            lb_toeslag.Content = berekenBijdrage();
        }

        private void Rb_KindOD10_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if ((bool)rb.IsChecked) aKindOD10 = int.Parse(sanitizeNumberString(rb.Content.ToString()));
            lb_toeslag.Content = berekenBijdrage();
        }

        private void Rb_KindJD10_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if ((bool)rb.IsChecked) aKindJD10 = int.Parse(sanitizeNumberString(rb.Content.ToString()));
            lb_toeslag.Content = berekenBijdrage();
        }

        private string berekenBijdrage()
        {
            float bedrag = 50 + (aKindJD10 * 25) + (aKindOD10 * 37);
            if (bedrag > 150) bedrag = 150;
            if (aOuder) bedrag *= (float)(100 - 25) / 100;
            return "\u20AC " + bedrag.ToString();
        }

        private string sanitizeNumberString(string value)
        {
            char[] valueChars = value.ToCharArray();
            value = "";

            for (int i = 0; i < valueChars.Length; i++)
            {
                if(digits.Contains(valueChars[i])) value += valueChars[i];
            }

            if (value == "" || value == null) value = "0";

            return value;
        }
    }
}
