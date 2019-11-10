using System.Collections.Generic;
using System.Windows.Controls;

namespace ToHDrag
{
    public class Plateau
    {
        private static readonly string[] BaseNames = { "Origin", "Side", "Destination" };
        public List<Base> Bases = new List<Base>();

        public static int GetBaseIndex(string name)
        {
            for (int i = 0; i < BaseNames.Length; i++)
            {
                if (BaseNames[i].Contains(name) || name.Contains(BaseNames[i]))
                    return i;
            }
            return -1;
        }

        public static int GetNextBaseIndex(int index)
        {
            return (index + 1) % BaseNames.Length;
        }

        public Plateau()
        {
            Bases.Capacity = 3;
        }

        public void AddBase(Panel panel, double left)
        {
            string name = Bases.Count > 0 ? BaseNames[Bases.Count] : BaseNames[0];
            Base b = new Base(panel, left, name);
            Bases.Insert(GetBaseIndex(b.Name), b);
        }
    }

}
