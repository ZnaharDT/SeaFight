using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SeaFight
{
    class FieldCell : Label
    {
        private int X;
        private int Y;

        FieldCell(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
