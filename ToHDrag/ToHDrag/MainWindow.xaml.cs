using System;
using System.Resources;
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
using System.IO;

namespace ToHDrag
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Point originPt, prevPt, prevOffset;
        Window wdw;
        IInputElement dragging, hit;
        List<Rectangle> allowedToMove = new List<Rectangle>();
        Plateau plt;
        int towerHeight;

        public MainWindow()
        {
            InitializeComponent();

            Image backgroundImage = new Image();
            backgroundImage.Name = "background";
            backgroundImage.Stretch = Stretch.Uniform;
            backgroundImage.StretchDirection = StretchDirection.Both;
            backgroundImage.Width = 800;
            backgroundImage.Height = 430;
            backgroundImage.VerticalAlignment = VerticalAlignment.Top;

            BitmapImage bitmap = BitmapToImageSource(Properties.Resources.hanoi);

            backgroundImage.Source = bitmap;

            Canvas canvas1 = new Canvas()
            {
                Width = 780,
                Height = 410,
                Name = "MainCanvas",
                Margin = new Thickness(0, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            canvas1.Children.Add(backgroundImage);

            mgrid.Children.Insert(0, canvas1);//make sure its behind uigrid
            mgrid.Margin = new Thickness(0,0,0,0);

            wdw = this;

            this.MouseMove += MainWindow_MouseMove;
            this.MouseDown += MainWindow_MouseDown;
            this.MouseUp += MainWindow_MouseUp;

            Solve.MouseDown += Solve_MouseDown;
            Solve.Click += Solve_Click;

            plt = new Plateau();

            plt.AddBase(canvas1, 105); //Origin
            plt.AddBase(canvas1, 305); //Side
            plt.AddBase(canvas1, 505); //Destination

            towerHeight = 4;

            plt.Bases[0].InitTower(canvas1, 4);
        }

        private void Solve_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("solve");

            List<Disc> origin = plt.Bases[Plateau.GetBaseIndex("Origin")].Tower;
            List<Disc> destination = plt.Bases[Plateau.GetBaseIndex("Destination")].Tower;
            List<Disc> side = plt.Bases[Plateau.GetBaseIndex("Side")].Tower;
            Disc currentDisc;
            Disc topDisc;
            Disc belowDisc;

            Stack<Disc> targets = new Stack<Disc>();

            //drawRack();


            //Loop while the tower on destination base is not as heigh as the original tower
            while (destination.Count != towerHeight)
            {
                //Is there a disc on the origin base? Put it in the target list
                if (origin.Count > 0) targets.Push(origin[origin.Count - 1]);

                //Also is there a disc on the side base? Put it in the target list
                else if (side.Count > 0) targets.Push(side[side.Count - 1]);

                //As long there are disc in the target list, move them
                while (targets.Count > 0)
                {
                    currentDisc = targets.Peek(); // We want to know the top disc on the target list without consuming it
                    topDisc = plt.Bases[currentDisc.BaseIndex].GetDiscAboveElseSame(currentDisc); // Get the top disc

                    int currentBaseIndex = currentDisc.BaseIndex;
                    int nextBaseIndex = Plateau.GetNextBaseIndex(currentDisc.BaseIndex);
                    belowDisc = plt.Bases[nextBaseIndex].GetTopDisc();

                    if (topDisc.Size == currentDisc.Size)
                    {
                        if (belowDisc != null && belowDisc.Size < currentDisc.Size) targets.Push(belowDisc);
                        else
                        {
                            plt.Bases[currentBaseIndex].RemoveFromTower(currentDisc, true);
                            plt.Bases[nextBaseIndex].AddToTower(currentDisc);//test if nextBaseIndex is correct, should be :-)
                            //placeDisc(currentDisc);
                            //drawRack();
                            targets.Pop();
                        }
                    }
                    else targets.Push(topDisc);
                }
            }

        }

        private void Solve_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        public Disc GetTopDiscNextPoleElseSame(Disc disc)
        {
            Base currentBase = plt.Bases[GetNextBaseIndex(disc)];
            return currentBase.Tower.Count > 0 ? currentBase.Tower[currentBase.Tower.Count - 1] : disc;
        }

        public int GetNextBaseIndex(Disc disc)
        {
            int currentBase = 0;
            for (int i = 0; i < plt.Bases.Count; i++)
            {
                if (plt.Bases[i].Tower.Contains(disc))
                    currentBase = i;
            }

            return (currentBase + 1) % plt.Bases.Count;
        }

        BitmapImage BitmapToImageSource(System.Drawing.Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        private void MainWindow_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (hit != null && dragging != null)
            {
                Disc currentDisc = null;
                Base newBase = null;
                Base oldBase = null;
                Rectangle baseRect = hit as Rectangle;
                Rectangle discRect = dragging as Rectangle;


                currentDisc = discRect.Tag as Disc;
                oldBase = plt.Bases[currentDisc.BaseIndex];
                newBase = plt.Bases[Plateau.GetBaseIndex(baseRect.Name)];

                oldBase.RemoveFromTower(currentDisc, false);


                /*for (int i = 0; i < plt.Bases.Count; i++)
                {
                    if (discRect!=null && 
                        plt.Bases[i].Tower.Contains(discRect.Tag))
                    {
                        oldBase = plt.Bases[i];
                        currentDisc = discRect.Tag as Disc;
                        oldBase.RemoveFromTower(currentDisc, false);
                    }

                    if (baseRect != null && 
                        baseRect.Name.Contains(plt.Bases[i].Name))
                    {
                        newBase = plt.Bases[i];
                    }
                }*/

                if (currentDisc != null &&
                    newBase != null &&
                    !newBase.Tower.Contains(currentDisc))
                {
                    if (newBase.GetTopDisc() == null ||
                       newBase.GetTopDisc().Rect.Width > currentDisc.Rect.Width)
                        newBase.AddToTower(currentDisc);
                    else
                        oldBase.AddToTower(currentDisc);
                }
            }

            dragging = null;
        }

        private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            originPt = Mouse.GetPosition(wdw);
            if (wdw.InputHitTest(originPt) != null || wdw.InputHitTest(originPt) is Rectangle)
            {

                allowedToMove.Clear();

                for (int i = 0; i < plt.Bases.Count; i++)//get all top discs and put them in in allowed to move
                {
                    if (plt.Bases[i].Tower.Count > 0)
                    {
                        allowedToMove.Add(plt.Bases[i].Tower[0].Rect);
                    }
                }

                if (allowedToMove.Contains(wdw.InputHitTest(originPt)))//when disc is in allowed to move, make it dragging
                {
                    dragging = wdw.InputHitTest(originPt);
                    Rectangle rect = dragging as Rectangle;
                    prevOffset = new Point(rect.RenderTransform.Value.OffsetX,
                                           rect.RenderTransform.Value.OffsetY);
                }
                
            }
        }

        private void MainWindow_MouseMove(object sender, MouseEventArgs e)
        {
            prevPt = Mouse.GetPosition(wdw);
            if (dragging != null)
            {
                if (e.LeftButton.ToString().Contains("Pressed") )
                {
                    Rectangle rect = dragging as Rectangle;
                    if (allowedToMove.Contains(dragging))//dragging is allowed to move, move!
                    {
                        rect.RenderTransform = new TranslateTransform(  prevPt.X - originPt.X + prevOffset.X,
                                                                        prevPt.Y - originPt.Y + prevOffset.Y);
                    }

                    Point pt = e.GetPosition((UIElement)sender);
                    //perform hittest
                    VisualTreeHelper.HitTest(wdw,
                                                new HitTestFilterCallback(filterCallback),
                                                new HitTestResultCallback(resultCallback),
                                                new PointHitTestParameters(pt));

                }
            }
        }

        private HitTestFilterBehavior filterCallback(DependencyObject o)
        {
            if (o != null && o.GetType() == typeof(Rectangle))
            {
                Rectangle rect = o as Rectangle;
                Rectangle rect2 = dragging as Rectangle;
                if (rect.Equals(rect2))
                {
                    if (hit != null)
                    {
                        Rectangle rect3 = hit as Rectangle;
                        if (rect3.Name.Contains("drag"))
                        {
                            rect3.Opacity = 0.5;
                        }
                    }
                    return HitTestFilterBehavior.ContinueSkipSelfAndChildren;

                }
                else
                {
                    return HitTestFilterBehavior.ContinueSkipChildren;
                }

            }
            return HitTestFilterBehavior.ContinueSkipSelf;
        }


        public HitTestResultBehavior resultCallback(HitTestResult result)
        {
            if (result.VisualHit is Rectangle)
            {
                
                Rectangle rect = result.VisualHit as Rectangle;
                Rectangle cnv2 = dragging as Rectangle;
                if (rect.Name.Contains("drag"))
                {
                    rect.Opacity = 0.25;
                }
                hit = rect;
            }
            return HitTestResultBehavior.Stop;
        }
    }
}
