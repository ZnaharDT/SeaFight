using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SeaFight
{
    class Field : Control
    {
        private const int cellSize = 25;

        private FieldCell[,] field = new FieldCell[10, 10];

        Field()
        {

        }
    }
}
