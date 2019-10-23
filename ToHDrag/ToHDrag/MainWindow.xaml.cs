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

        private static List<Base> Bases = new List<Base>();

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

            mgrid.Children.Add(canvas1);
            mgrid.Margin = new Thickness(0,0,0,0);

            wdw = this;

            this.MouseMove += MainWindow_MouseMove;
            this.MouseDown += MainWindow_MouseDown;
            this.MouseUp += MainWindow_MouseUp;

            Bases.Add(new Base(canvas1, 105, "Origin"));
            Bases.Add(new Base(canvas1, 305, "Side"));
            Bases.Add(new Base(canvas1, 505, "Destination"));

            int towerHeight = 6;

            for (int i = 0; i < towerHeight; i++)
            {
                double width = 20 + (20 * towerHeight) - (20 * i);
                Disc disc = new Disc(width, "disc" + (i + 1));
                Bases[0].AddToTower(disc);
                canvas1.Children.Insert(canvas1.Children.Count, disc.Rect);
            }
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
            Disc currentDisc = null;
            Base newBase = null;
            Base oldBase = null;
            Rectangle baseRect = hit as Rectangle;
            Rectangle dragRect = dragging as Rectangle;

            for (int i = 0; i < Bases.Count; i++)
            {
                if (dragRect!=null && 
                    Bases[i].Tower.Contains(dragRect.Tag))
                {
                    oldBase = Bases[i];
                    currentDisc = dragRect.Tag as Disc;
                    oldBase.RemoveFromTower(currentDisc);
                    Console.WriteLine("{0} removed from {1}", currentDisc.Name, oldBase.Name);
                }

                if (baseRect != null && 
                    baseRect.Name.Contains(Bases[i].Name)) {
                    newBase = Bases[i];
                }
            }

            if (currentDisc != null && 
                newBase != null && 
                !newBase.Tower.Contains(currentDisc))
            {
                if(newBase.GetTopDisc() == null ||
                   newBase.GetTopDisc().Rect.Width > currentDisc.Rect.Width)
                    newBase.AddToTower(currentDisc);
                else
                    oldBase.AddToTower(currentDisc);
                Console.WriteLine("{0} inserted at 0 in {1}", currentDisc.Name, newBase.Name);
            }

            dragging = null;
        }

        private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            originPt = Mouse.GetPosition(wdw);
            if (wdw.InputHitTest(originPt) != null || wdw.InputHitTest(originPt) is Rectangle)
            {

                allowedToMove.Clear();

                for (int i = 0; i < Bases.Count; i++)
                {
                    if (Bases[i].Tower.Count > 0)
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
                        if (rect3.Name.Contains("drag"))
                        {
                            rect3.Opacity = 0.5;
                        }
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
                if (rect.Name.Contains("drag"))
                {
                    rect.Opacity = 0.25;
                }
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
            Canvas.SetLeft(disc.Rect, Left + ((Rect.Width - disc.Rect.Width) / 2));
            Canvas.SetBottom(disc.Rect, -270+(20*(Tower.Count-1)));
            disc.Rect.RenderTransform = new TranslateTransform();
        }

        public void RemoveFromTower(Disc disc)
        {
            Tower.Remove(disc);
        }

        public Disc GetTopDisc()
        {
            if (Tower.Count > 0)
            {
                return Tower[0];
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
            Opacity = 0.5,
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
