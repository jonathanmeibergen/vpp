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

namespace TowerOfHanoi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Rack ToH = new Rack();
        }

    }

    public class Rack
    {
        //absolute references
        private List<Disc> leftPole = new List<Disc>();
        private List<Disc> middlePole = new List<Disc>();
        private List<Disc> rightPole = new List<Disc>();

        private List<Disc>[] poles = new List<Disc>[3];
        //relative referense
        private List<Disc> origin;
        private List<Disc> destination;
        private List<Disc> side;

        private Stack<Disc> targets = new Stack<Disc>();

        private int indexDestination, indexOrigin, indexSide;

        public Rack()
        {
            poles[indexOrigin = 0] = leftPole;
            poles[indexSide = 1] = middlePole;
            poles[indexDestination = 2] = rightPole;

            origin = leftPole;
            destination = rightPole;
            side = middlePole;

            leftPole.Add(new Disc(6, 0));
            leftPole.Add(new Disc(5, 0));
            leftPole.Add(new Disc(4, 0));
            leftPole.Add(new Disc(3, 0));
            leftPole.Add(new Disc(2, 0));
            leftPole.Add(new Disc(1, 0));

            moveTower();

        }

        public void drawRack()
        {
            int lpvalue, mpvalue, rpvalue;//, tgvalue;
            //Disc[] targetsArray = targets.ToArray();
            int mostDiscs = 0;
            for (int j = 0; j < poles.Length; j++)
            {
                mostDiscs = poles[j].Count >= mostDiscs ? poles[j].Count : mostDiscs;
            }
            int i = mostDiscs;

            do
            {
                lpvalue = i <= leftPole.Count && i > 0? leftPole[i-1].size : 0;
                mpvalue = i <= middlePole.Count && i > 0 ? middlePole[i-1].size : 0;
                rpvalue = i <= rightPole.Count && i > 0 ? rightPole[i-1].size : 0;
                //tgvalue = i <= targetsArray.Length && i > 0 ? targetsArray[i - 1].size : 0;

                if ((lpvalue + mpvalue + rpvalue) != 0)
                {
                    Console.WriteLine("{0} {1} {2}", lpvalue, mpvalue, rpvalue);//, tgvalue);
                }
                i--;
            } while ((lpvalue + mpvalue + rpvalue) != 0);

            Console.WriteLine("-----");

        }

        public Disc getDiscAboveElseSame(Disc disc)
        {
            List<Disc> pole = poles[disc.pole];
            int index = pole.IndexOf(disc);
            int total = pole.Count;
            if (index + 1 == total)
                return pole[index];
            else
                return pole[index + 1];
        }

        public Disc getTopDiscNextPoleElseSame(Disc disc)
        {
            List<Disc> npole = poles[getNextPoleIndex(disc)];
            return npole.Count > 0 ? npole[npole.Count-1] : disc;
        }

        public void placeDisc(Disc disc)
        {
            List<Disc> npole = poles[disc.pole];
            

            /*if (destination.Count == 0)
            {
                npole.Remove(disc);
                disc.pole = indexDestination;
                npole = destination;
            }
            else if (destination[destination.Count - 1].size > disc.size) {

                npole.Remove(disc);
                disc.pole = indexDestination;
                npole = destination;
            }
            else {*/
                npole.Remove(disc);
                disc.pole = getNextPoleIndex(disc);
                npole = poles[disc.pole];
            //}

            npole.Add(disc);

        }

        public int getNextPoleIndex(Disc disc)
        {
            if(disc.pole + 1 >= poles.Length)
            {
                return 0;
            }

            return disc.pole + 1;
        }

        public void moveTower()
        {

            origin = leftPole;
            destination = rightPole;
            side = middlePole;
            int towerHeight = origin.Count;
            Disc currentDisc;
            Disc topDisc;
            Disc belowDisc;

            drawRack();

            while (destination.Count != towerHeight)
            {
                if(origin.Count > 0)
                {
                    if (origin.Count > 0) targets.Push(origin[origin.Count-1]);
                }
                else
                {
                    if (side.Count > 0) targets.Push(side[side.Count - 1]);
                }

                while(targets.Count > 0)
                {
                    currentDisc = targets.Peek();
                    topDisc = getDiscAboveElseSame(currentDisc);
                    belowDisc = getTopDiscNextPoleElseSame(currentDisc);
                    if (topDisc.size == currentDisc.size)
                    {
                        if(belowDisc.size < currentDisc.size)
                        {
                            targets.Push(belowDisc);
                        }
                        else
                        {
                            placeDisc(currentDisc);
                            drawRack();
                            targets.Pop();
                        }
                    }
                    else
                    {
                        targets.Push(topDisc);
                    }

                }
            }

        }

    }

    public class Disc
    {
        public int size { get; set; }
        public int pole { get; set; }

        public Disc(int size, int place)
        {
            this.pole = place;
            this.size = size;
        }
    }

}
