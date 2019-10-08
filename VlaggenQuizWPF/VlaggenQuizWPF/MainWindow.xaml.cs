using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace VlaggenQuizWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        private static Quiz Quiz;
        List<Vlag> alleVlaggen { get; }
        List<Vlag> copyAlleVlaggen;
        private Image vlagImage;
        private StreamReader textFile;
        private Random rand = new Random();

        private DispatcherTimer dpt = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();

            dpt.Tick += Dpt_Tick;
            dpt.Interval = new TimeSpan(0,0,1);

            string basePath = @"C:\Users\Jonathan\Desktop\FLAGS\";

            textFile = new StreamReader(basePath + @"list.txt");
            alleVlaggen = new List<Vlag>();

            while (!textFile.EndOfStream)
            {
                string newLine = textFile.ReadLine();
                Console.WriteLine("{0} - {1}", SanitizeCountryName(newLine), SanitizeFileName(newLine));
                alleVlaggen.Add(new Vlag(SanitizeCountryName(newLine),
                                new Uri(basePath + SanitizeFileName(newLine)))
                                );
            }
       
            copyAlleVlaggen = alleVlaggen.ToList();
            copyAlleVlaggen.RemoveAt(6);

            vlagImage = new Image();
            vlagImage.Name = "img_Vlag";
            vlagImage.Stretch = Stretch.Uniform;
            vlagImage.StretchDirection = StretchDirection.Both;
            vlagImage.Width = 300;
            vlagImage.Height = 300;
            vlagImage.VerticalAlignment = VerticalAlignment.Top;
            vlagImage.MouseDown += VlagImage_MouseDown;
            grid.Children.Add(vlagImage);

            lb_score.Content = "0 points";

            bt_answer1.Click += Bt_answer_Click;
            bt_answer2.Click += Bt_answer_Click;
            bt_answer3.Click += Bt_answer_Click;
            bt_answer4.Click += Bt_answer_Click;

            Quiz = new Quiz(alleVlaggen, 10);

            Question qn = Quiz.getNextQuestion();
            vlagImage.Source = qn.answer.bitmap;
            DrawQuestion(qn);
        }

        private void Dpt_Tick(object sender, EventArgs e)
        {
            DrawQuestion(Quiz.getNextQuestion());
            dpt.Stop();
        }

        private void VlagImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Image img = sender as Image;
            Question currentQuestion = Quiz.getNextQuestion();
            DrawQuestion(currentQuestion);
        }

        private void DrawQuestion(Question qn)
        {
            vlagImage.Source = qn.answer.bitmap;
            bt_answer1.Content = qn.mChoices[0].countryName;
            bt_answer2.Content = qn.mChoices[1].countryName;
            bt_answer3.Content = qn.mChoices[2].countryName;
            bt_answer4.Content = qn.mChoices[3].countryName;

            bt_answer1.Background = Brushes.LightGray;
            bt_answer2.Background = Brushes.LightGray;
            bt_answer3.Background = Brushes.LightGray;
            bt_answer4.Background = Brushes.LightGray;
        }

        private void Bt_answer_Click(object sender, RoutedEventArgs e)
        {

            Button btn = sender as Button;
            Console.WriteLine("{0}", btn.Content);
            if (Quiz.checkAnswer(btn.Content.ToString()))
            {
                btn.Background = Brushes.Green;
                Quiz.setScore(1);
                dpt.Start();
            }
            else
            {
                btn.Background = Brushes.Red;
                Quiz.setScore(-1);
            }
            lb_score.Content = Quiz.getScore().ToString() + " points";
        }

        private string SanitizeCountryName(string path)
        {
            int startIndexCName = path.LastIndexOf(@"Flag_of_") + @"Flag_of_".Length;
            path = path.Substring(startIndexCName,
                                  path.Length - startIndexCName -
                                 (path.Contains(".png") ? @".png".Length : 0) -
                                 (path.Contains(".webp") ? @".webp".Length : 0));

            return path.Replace('_', ' ');
        }
        private string SanitizeFileName(string path)
        {
            return path.Substring(0,
                                  path.Length -
                                 (path.Contains(".webp") ? @".webp".Length : 0));
        }
    }

    public class Quiz
    {
        private Random rand = new Random();
        public int maxQuestions, correctAnswers;
        private int index = 0;
        private List<Question> questions = new List<Question>();
        private int score = 0;
        public Question getNextQuestion()
        {
            if (index < questions.Count-1)
            {
                return questions[++index];
            }
            return questions[index=0];
        }

        public Quiz(List<Vlag> vlaggen, int maxQ)
        {

            maxQuestions = maxQ;

            for (int i = 0; i < maxQuestions; i++)
            {
                Console.WriteLine("---Question {0}----", i);
                questions.Add(new Question(vlaggen, rand));
            }
        }

        public bool checkAnswer(string answer)
        {
            return questions[index].CheckAnwser(answer);
        }

        public void setScore(int points)
        {
            if (score >= 0 && points > 0)
                score += points;
            else
                score = 0;
        }

        public int getScore()
        {
            return score;
        }

    }

    public class Vlag {

        public string countryName;
        private Uri imageUri;
        public BitmapImage bitmap;
        public bool isShown = false;
        public bool imageNotFound = false;

        public Vlag(string name, Uri uri)
        {
            imageUri = uri;
            countryName = name;
            bitmap = new BitmapImage();
            
            try
            {
                bitmap.BeginInit();
                bitmap.UriSource = uri;
                bitmap.EndInit();
            }
            catch (FileNotFoundException)
            {
                bitmap = null;
                imageNotFound = true;
            }
            
        }

        public void drawVlag(Image img)
        {
            img.Source = bitmap;
        }

    }

    public class Question
    {
        public List<Vlag> mChoices = new List<Vlag>();
        private List<Vlag> copyAlleVlaggen;
        public Vlag answer;

        public Question(in List<Vlag> vlaggen, Random rand)
        {
            Vlag vlag;
            int indexAnswer = rand.Next(4);
            copyAlleVlaggen = vlaggen.ToList();

            for (int i = 0; i < 4; i++)
            {
                int indexVlag = rand.Next(copyAlleVlaggen.Count);
                vlag = vlaggen[indexVlag];
                copyAlleVlaggen.RemoveAt(indexVlag);

                mChoices.Add(vlag);

                if (i == indexAnswer)
                {
                    answer = vlag;
                    Console.WriteLine("{0} a {1}", i, vlag.countryName);
                }
                else
                {
                    Console.WriteLine("{0} - {1}", i, vlag.countryName);
                }
            }

            copyAlleVlaggen = null;
        }

        public bool CheckAnwser(string answer)
        {
            if (answer.Equals(this.answer.countryName)) return true;
            else return false;
        }

    }

}
