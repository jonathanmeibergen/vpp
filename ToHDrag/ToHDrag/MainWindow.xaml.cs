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
        IInputElement dragging, hit, allowedToMove;

        public MainWindow()
        {
            InitializeComponent();

            wdw = this;

            Rectangle rect1 = new Rectangle()
            {
                Name = "rect1",
                Fill = Brushes.White,
                Stroke = Brushes.Black,
                Width = 100,
                Height = 20,
                Margin = new Thickness(20,20,0,0)
            };
            Rectangle rect2 = new Rectangle()
            {
                Name = "rect2",
                Fill = Brushes.White,
                Stroke = Brushes.Black,
                Width = 100,
                Height = 20,
                Margin = new Thickness(20, 20, 0, 0)
            };
            Rectangle rect3 = new Rectangle()
            {
                Name = "rect3",
                Fill = Brushes.White,
                Stroke = Brushes.Black,
                Width = 100,
                Height = 20,
                Margin = new Thickness(20, 20, 0, 0)
            };

            mgrid.Children.Add(rect1);
            mgrid.Children.Add(rect2);
            mgrid.Children.Add(rect3);


                //mgrid.Children.Add(rect3);

                this.MouseMove += MainWindow_MouseMove;
            this.MouseDown += MainWindow_MouseDown;
            this.MouseUp += MainWindow_MouseUp;

        }

        private void MainWindow_MouseUp(object sender, MouseButtonEventArgs e)
        {
            dragging = null;
        }

        private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            originPt = Mouse.GetPosition(wdw);
            if (wdw.InputHitTest(originPt) != null)
            {
                object ihtObj = wdw.InputHitTest(originPt);
                if (ihtObj is Rectangle)
                {
                    dragging = wdw.InputHitTest(originPt);
                    Rectangle rect = dragging as Rectangle;
                    prevOffset = new Point(rect.RenderTransform.Value.OffsetX,
                                            rect.RenderTransform.Value.OffsetY);

                    Console.WriteLine("hello");
                    //Canvas cnv = rect.Parent as Canvas;
                    //Panel pnl = cnv.Parent as Panel;
                    //Console.WriteLine(pnl.Children.Count);

                }
            }
        }

        private void MainWindow_MouseMove(object sender, MouseEventArgs e)
        {
            prevPt = Mouse.GetPosition(wdw);
            if (dragging != null)
            { 
                if (e.LeftButton.ToString().Contains("Pressed") && dragging is Rectangle)
                {
                    Rectangle rect = dragging as Rectangle;

                    //Console.WriteLine(allowedToMove.Equals(dragging));

                        rect.RenderTransform = new TranslateTransform(  prevPt.X - originPt.X + prevOffset.X,
                                                                        prevPt.Y - originPt.Y + prevOffset.Y);

                    Point pt = e.GetPosition((UIElement)sender);
                    VisualTreeHelper.HitTest(wdw, filterCallback, MyCallback, new PointHitTestParameters(pt));
                }
            }
        }

        
        private HitTestFilterBehavior filterCallback(DependencyObject o)
        {
            Console.WriteLine("hittestfiltercallback");
            if (o != null && o.GetType() == typeof(Rectangle))
            {
                Rectangle rect = o as Rectangle;
                Rectangle rect2 = dragging as Rectangle;

                Console.WriteLine("hello");

                if (rect.Equals(rect2))
                {
                    if (hit != null)
                    {
                        Rectangle rect3 = hit as Rectangle;
                        rect3.Opacity = 1;
                    }
                    // Visual object and descendants are NOT part of hit test results enumeration.
                    return HitTestFilterBehavior.ContinueSkipSelfAndChildren;
                   
                } else
                {
                    return HitTestFilterBehavior.Continue;
                }

            }
            return HitTestFilterBehavior.Continue;
        }


        public HitTestResultBehavior MyCallback(HitTestResult result)
        {
            //Console.WriteLine(result.VisualHit);

            if (result.VisualHit is Rectangle)
            {
                Rectangle rect = result.VisualHit as Rectangle;
                //Rectangle cnv2 = dragging as Rectangle;
                rect.Opacity = 0.5;
                hit = rect;
                return HitTestResultBehavior.Stop;
            }
            return HitTestResultBehavior.Stop;

        }
    }



}
