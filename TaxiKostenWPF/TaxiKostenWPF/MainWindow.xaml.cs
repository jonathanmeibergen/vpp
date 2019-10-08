
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

namespace TaxiKostenWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private int startUren;
        private int startMinuten;
        private int eindUren;
        private int eindMinuten;
        private int kilometers;
        private char[] digits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        private string[] dagNamen = { "maandag", "dinsdag", "woensdag", "donderdag", "vrijdag", "zaterdag", "zondag" };
        private List<Button> weekButtons;
        private Button prevButton;
        private string dag;

    public MainWindow()
        {
            InitializeComponent();



            weekButtons = new List<Button>();
            double widthValue = (double)(w_main.Width - (10 * (dagNamen.Length-1))) / dagNamen.Length;

            WrapPanel wpp = new WrapPanel();
            wpp.Margin = new Thickness(0,10,0,5);

            for (int i = 0; i < dagNamen.Length; i++)
            {
                Button btn = new Button();
                btn.Content = dagNamen[i];
                //btn.Width = widthValue;

                double value = 40;
                btn.Height = 64;
                btn.MinWidth = widthValue;
                //double widthValue = (double)(10 * i)+30*i;
                btn.Margin = new Thickness(5, 5, 0, 0);
                btn.HorizontalAlignment = HorizontalAlignment.Left;
                btn.VerticalAlignment = VerticalAlignment.Top;
                btn.Click += Btn_Click;
                wpp.Children.Add(btn);
            }
            stp_main.Children.Insert(0, wpp);

            tbKilometers.MaxLength = 5;
            tbKilometers.Text = "";
            tbKilometers.TextChanged += TbKilometers_TextChanged;

            for (int i = 0; i < 24; i++)
            {
                string timeValue = i.ToString();

                if (i < 10) timeValue = "0" + timeValue;
                cbStartTijdUren.Items.Add(timeValue);
                cbEindtTijdUren.Items.Add(timeValue);
            }

            for (int i = 0; i < 60; i++)
            {
                string timeValue = i.ToString();

                if (i < 10) timeValue = "0" + timeValue;
                cbStartTijdMinuten.Items.Add(timeValue);
                cbEindtTijdMinuten.Items.Add(timeValue);
            }

            dag = dagNamen[0];

            cbStartTijdUren.SelectedIndex = 0;
            cbStartTijdMinuten.SelectedIndex = 0;
            cbEindtTijdUren.SelectedIndex = 0;
            cbEindtTijdMinuten.SelectedIndex = 0;

            cbStartTijdUren.SelectionChanged += cb_SelectionChanged;
            cbStartTijdMinuten.SelectionChanged += cb_SelectionChanged;
            cbEindtTijdUren.SelectionChanged += cb_SelectionChanged;
            cbEindtTijdMinuten.SelectionChanged += cb_SelectionChanged;

            berekenRitPrijs();
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            if(prevButton != null)
            {
                prevButton.Background = Brushes.LightGray;
                prevButton = sender as Button;

            } else
            {
                prevButton = sender as Button;
            }

            Button btn = sender as Button;
            btn.Background = Brushes.LightGreen;
            dag = btn.Content.ToString();
            berekenRitPrijs();
            Console.WriteLine("selected dag is :{0}", dag);
        }

        private void TbKilometers_TextChanged(object sender, TextChangedEventArgs e)
        {
            string kmText = tbKilometers.Text;
            char[] kmArray = kmText.ToArray();
            kmText = "";

            for (int i = 0; i < kmArray.Length; i++)
            {
                if (digits.Contains(kmArray[i]))
                {
                    kmText += kmArray[i].ToString();
                }
            }

            tbKilometers.Text = kmText;
            kilometers = (kmText != "")? int.Parse(kmText) : 0;

            berekenRitPrijs();
        }

        private void cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            if(cb.Name == "cbStartTijdUren") startUren = int.Parse(cb.SelectedValue.ToString());
            if(cb.Name == "cbStartTijdMinuten") startMinuten = int.Parse(cb.SelectedValue.ToString());
            if(cb.Name == "cbEindtTijdUren") eindUren = int.Parse(cb.SelectedValue.ToString());
            if(cb.Name == "cbEindtTijdMinuten") eindMinuten = int.Parse(cb.SelectedValue.ToString());

            berekenRitPrijs();
        }

        private void berekenRitPrijs()
        {
            int RitStart = (startUren * 60) + startMinuten;
            int RitEind = (eindUren * 60) + eindMinuten;
            int totaalMinuten = 0;
            int minutenNormaal = 0;
            int minutenHoog;
            float totaalPrijs;
            int startNT = 8*60; //start normaal tarief
            int eindNT = 18*60; //eind normaal tarief

            if (RitEind >= RitStart) totaalMinuten = RitEind - RitStart;
            //else totaalMinuten = ((24 * 60) - totaalMinutenStart) + totaalMinutenEind;

            if(RitStart <= startNT)
            {
                if(RitEind >= startNT && RitEind < eindNT)
                {
                    minutenNormaal = RitEind - startNT;
                }
                else if (RitEind > eindNT)
                {
                    minutenNormaal = eindNT - startNT;
                }
            }
            else if (RitStart >= startNT && RitStart < eindNT)
            {
                if (RitEind > eindNT)
                {
                    minutenNormaal = eindNT - RitStart;
                }
                else
                {
                    minutenNormaal = totaalMinuten;
                }
            }

            minutenHoog = totaalMinuten - minutenNormaal;
            totaalPrijs = kilometers * 100 + (25 * minutenNormaal) + (45 * minutenHoog); //in centen

            if ((dag.Equals(dagNamen[4]) && RitStart >= (22*60)) ||
                 dag.Equals(dagNamen[5]) ||
                 dag.Equals(dagNamen[6]) ||
                (dag.Equals(dagNamen[0]) && RitStart <= (7*60)))
            {
                totaalPrijs *= (float)1.15;
            }

            lbPrijs.Content = ((float) totaalPrijs / 100).ToString() + " \u20ac";

        }
    }
}
