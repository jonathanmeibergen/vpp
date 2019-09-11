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

namespace CharacterCount
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Ut semper ligula vel elit tincidunt laoreet. Morbi condimentum neque nec sapien cursus a dignissim neque eleifend. Suspendisse a faucibus erat. Etiam sagittis eros erat. Mauris egestas commodo purus et dignissim. Aenean augue orci, molestie ut placerat vel, hendrerit sit amet mauris. Duis tristique porttitor mattis. Nam lacinia pellentesque leo in tincidunt. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Etiam tempor, eros sit amet porta pretium, diam eros blandit lectus, eu faucibus mauris diam vitae lacus. Cras est massa, dapibus a vestibulum ac, ullamcorper et leo. Curabitur pretium, risus vel ultrices aliquam, est urna aliquet lacus, quis dapibus metus nibh eu tellus. Morbi ac odio ut orci posuere congue. Sed sollicitudin viverra mi, vehicula pellentesque magna elementum at. Vivamus eu magna eget nunc venenatis cursus. Duis adipiscing ultricies turpis non mollis. Cras non accumsan massa.";
        Dictionary<char, int> countedChars = new Dictionary<char, int>();
        int textLength;

        public MainWindow()
        {
            InitializeComponent();

            textLength = text.Length;
            for (int i = 0; i < textLength; i++)
            {
                char searchChar = text.Substring(text.Length - 1).ToCharArray().ElementAt(0);
                text = text.Remove(text.Length - 1);
                if (!countedChars.ContainsKey(searchChar))
                {
                    countedChars.Add(searchChar, 1);
                } else
                {
                    int howMany;
                    howMany = countedChars[searchChar];
                    countedChars[searchChar] = howMany + 1;
                }
                
            }

            foreach (KeyValuePair<char, int> kvp in countedChars)
            {
                Console.WriteLine("{0} counted {1} times", kvp.Key, kvp.Value);
            }

        }
    }
}
