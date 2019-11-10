using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ToHDrag
{
    public class Disc
    {
        public string Name { get; set; }
        public int BaseIndex { get; set; }
        public int Size { get; set; }

        public Rectangle Rect = new Rectangle()
        {
            Stroke = Brushes.Black,
            Fill = Brushes.White,
            Height = 20,
            VerticalAlignment = VerticalAlignment.Top,
        };

        public Disc(double width, int size)
        {
            Name = "disc" + size.ToString();
            Size = size;
            Rect.Name = Name;
            Rect.Margin = new Thickness(0, -350, 0, 0);
            Rect.Width = width;
            Rect.Tag = this;
        }
    }

}
