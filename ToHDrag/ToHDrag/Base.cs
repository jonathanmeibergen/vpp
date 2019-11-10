using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ToHDrag
{
    public class Base
    {
        public string Name { get; set; }
        public List<Disc> Tower = new List<Disc>();
        public Panel Panel;
        private double Left;

        public void AddToTower(Disc disc)
        {
            disc.BaseIndex = Plateau.GetBaseIndex(Name);
            Tower.Insert(0, disc);
            System.Console.WriteLine("{0} placed on base: {1}", disc.Name, this.Name);
            SnapDiscToBottom(disc);
        }

        public void InitTower(Panel pnl, int towerHeight)
        {
            for (int i = 0; i < towerHeight; i++)
            {
                double width = 20 + (20 * i);//20 + (20 * towerHeight) - (20 * i);
                Disc disc = new Disc(width, i + 1);
                Canvas.SetLeft(disc.Rect, Left + ((Rect.Width - disc.Rect.Width) / 2));
                Canvas.SetBottom(disc.Rect, -290 + (20 * (towerHeight - i)));
                disc.Rect.RenderTransform = new TranslateTransform();

                Tower.Add(disc);
                pnl.Children.Insert(pnl.Children.Count, disc.Rect);
            }
        }

        private void SnapDiscToBottom(Disc disc)
        {
            Canvas.SetLeft(disc.Rect, Left + ((Rect.Width - disc.Rect.Width) / 2));
            Canvas.SetBottom(disc.Rect, -270 + (20 * (Tower.Count - 1)));
            disc.Rect.RenderTransform = new TranslateTransform();
        }

        public void RemoveFromTower(Disc disc, bool visual)
        {
            Tower.Remove(disc);
            System.Console.WriteLine("{0} removed from base: {1}", disc.Name, this.Name);

            if (visual)
                SnapDiscToBottom(disc);
        }

        public Disc GetTopDisc()
        {
            if (Tower.Count > 0)
            {
                return Tower[0];
            }
            return null;
        }

        public Disc GetDiscAboveElseSame(Disc disc)
        {
            int index = Tower.IndexOf(disc);
            int total = Tower.Count;

            if (index == 0) // If it is already top disc, 
                return Tower[index]; // Return it
            else
                return Tower[index - 1]; // Else return disc above
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
