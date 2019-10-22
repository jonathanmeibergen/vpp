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
        List<IInputElement> allowedToMove = new List<IInputElement>();

        private static List<Base> Bases = new List<Base>();

        public MainWindow()
        {
            InitializeComponent();

            Canvas canvas1 = new Canvas()
            {
                Width = 800,
                Height = 430,
                Name = "MainCanvas",
                Margin = new Thickness(0,0,0,0),
                Background = Brushes.Red,
                VerticalAlignment = VerticalAlignment.Bottom
            };

            Rectangle rect10 = new Rectangle()
            {
                Width = 60,
                Height = 60,
                Fill = Brushes.Blue,
            };

            Rectangle rect11 = new Rectangle()
            {
                Width = 70,
                Height = 70,
                Fill = Brushes.Orange,
            };

            //canvas1.Children.Add(rect11);
            //canvas1.Children.Add(rect10);
            mgrid.Children.Add(canvas1);
            mgrid.Margin = new Thickness(0,0,0,0);

            wdw = this;

            this.MouseMove += MainWindow_MouseMove;
            this.MouseDown += MainWindow_MouseDown;
            this.MouseUp += MainWindow_MouseUp;

            Bases.Add(new Base(canvas1, 20, "Origin"));
            Bases.Add(new Base(canvas1, 190, "Side"));
            Bases.Add(new Base(canvas1, 360, "Destination"));

            int towerHeight = 6;

            for (int i = 0; i < towerHeight; i++)
            {
                Disc disc = new Disc(20 + (20 * towerHeight) - (20 * i), "disc" + (i + 1));
                Bases[0].Tower.Insert(0, disc);
                disc.Rect.Margin = new Thickness(150 - (20 * (towerHeight+1)) + (20 * i /2), 330 + (-20 * i), 0 ,0);
                canvas1.Children.Insert(canvas1.Children.Count, disc.Rect);
            }
        }

        private void MainWindow_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Disc currentDisc = null;
            Base newBase = null;
            Base oldBase = null;
            Rectangle baseRect = hit as Rectangle;
            Rectangle dragRect = dragging as Rectangle;

            for (int i = 0; i < Bases.Count; i++)
            {
                if (Bases[i].Tower.Contains(dragRect.Tag))
                {
                    oldBase = Bases[i];
                    currentDisc = dragRect.Tag as Disc;
                    oldBase.RemoveFromTower(currentDisc);
                    if (oldBase.GetTopDisc() != null)
                    {
                        allowedToMove.Add(oldBase.GetTopDisc().Rect);
                    }
                    Console.WriteLine("{0} removed from {1}", currentDisc.Name, oldBase.Name);
                }

                if (baseRect.Name.Contains(Bases[i].Name)) {
                    newBase = Bases[i];
                }
            }

            if (currentDisc != null && newBase != null && !newBase.Tower.Contains(currentDisc))
            {
                if (newBase.GetTopDisc()!=null && allowedToMove.Contains(newBase.GetTopDisc().Rect))
                {
                    allowedToMove.Remove(newBase.GetTopDisc().Rect);
                }
                newBase.AddToTower(currentDisc);
                Console.WriteLine("{0} inserted at 0 in {1}", currentDisc.Name, newBase.Name);
            }

            dragging = null;
        }

        private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            originPt = Mouse.GetPosition(wdw);
            if (wdw.InputHitTest(originPt) != null)
            {
               
                if (wdw.InputHitTest(originPt) is Rectangle)
                {
                    
                    for (int i = 0; i < Bases.Count; i++)
                    {
                        if (Bases[i].Tower.Count > 1)
                        {
                            allowedToMove.Add(Bases[i].Tower[0].Rect);
                        }
                    }

                    if (allowedToMove.Contains(wdw.InputHitTest(originPt)))
                    {
                        dragging = wdw.InputHitTest(originPt);
                        Rectangle rect = dragging as Rectangle;
                        prevOffset = new Point(rect.RenderTransform.Value.OffsetX,
                                               rect.RenderTransform.Value.OffsetY);
                    }
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
                    if (allowedToMove.Contains(dragging))
                    {
                        rect.RenderTransform = new TranslateTransform(  prevPt.X - originPt.X + prevOffset.X,
                                                                        prevPt.Y - originPt.Y + prevOffset.Y);
                    }

                    Point pt = e.GetPosition((UIElement)sender);

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
                        rect3.Opacity = 1;
                    }
                    // Visual object and descendants are NOT part of hit test results enumeration.
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
                rect.Opacity = 0.5;
                hit = rect;
            }
            return HitTestResultBehavior.Stop;

        }
    }

    public class Disc
    {
        public string Name { get; set; }

        public Rectangle Rect = new Rectangle()
        {
            Stroke = Brushes.Black,
            Fill = Brushes.White,
            Height = 20,
            VerticalAlignment = VerticalAlignment.Top,
        };

        public Disc(double width, string name)
        {
            Name = name;
            Rect.Name = name;
            Rect.Margin = new Thickness(0, -350, 0, 0);
            Rect.Width = width;
            Rect.Tag = this;
        }
    }

    public class Base
    {
        public string Name { get; set; }
        public List<Disc> Tower = new List<Disc>();
        public Panel Panel;
        private double Left;

        public void AddToTower(Disc disc)
        {
            Tower.Insert(0, disc);
            Canvas.SetLeft(disc.Rect, Left);
            Canvas.SetBottom(disc.Rect, 80+(20*(Tower.Count-1)));
            disc.Rect.RenderTransform = new TranslateTransform();
        }

        public void RemoveFromTower(Disc disc)
        {
            Tower.Remove(disc);
        }

        public Disc GetTopDisc()
        {
            if (Tower.Count > 1)
            {
                return Tower[Tower.Count - 1];
            }
            return null;
        }

        private Rectangle Rect = new Rectangle()
        {
            Fill = Brushes.Black,
            Height = 10,
            Width = 150,
        };

        private Rectangle DragArea = new Rectangle()
        {
            Fill = Brushes.LightGray,
            Height = 300,
            Width = 150,
        };

        public Base(Panel panel, double left, string name)
        {
            Rect.Name = "base" + name;
            Rect.Margin = new Thickness(left, 350, 0, 0);
            DragArea.Name = "drag" + name;
            Name = name;
            Left = left;
            DragArea.Margin = new Thickness(left, 50, 0, 0);
            Panel = panel;
            Panel.Children.Add(DragArea);
            Panel.Children.Add(Rect);
            Rect.Tag = this;
            DragArea.Tag = this;
        }
    }
}
