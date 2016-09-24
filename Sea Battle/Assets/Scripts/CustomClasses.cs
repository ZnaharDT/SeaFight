using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    public class ShipPlacedEventArgs : EventArgs
    {
        public int decks;

        public ShipPlacedEventArgs(int _decks)
        {
            this.decks = _decks;
        }

        public int PlacedShipDecks
        {
            get
            {
                return decks;
            }
        }
    }

    public class MouseOverEventArgs : EventArgs
    {

        private GridPoint point;

        public MouseOverEventArgs(GridPoint _point)
        {
            this.point = _point;
        }

        public GridPoint HooverPoint
        {
            get
            {
                return point;
            }
        }
    }

    /// <summary>
    /// Represents position of cell on the grid
    /// </summary>
    public class GridPoint
    {
        public int i;
        public int j;

        public GridPoint(int _i, int _j)
        {
            i = _i;
            j = _j;
        }
    }

    /// <summary>
    /// Represents all the ships in game
    /// </summary>
    public class Ship
    {
        private const int ROT_0 = 0;
        private const int ROT_90 = 1;
        private const int ROT_COUNT = 2;

        public int width;
        public int height;
        public int decks;

        public ArrayList positionOnGrid;

        private int rotation;

        protected bool[] ShipCells;

        /// <summary>
        /// Initialize new ship with specified number of decks
        /// </summary>
        /// <param name="_decks">Number of decks</param>
        public Ship(int _decks)
        {
            width = _decks;
            decks = _decks;
            height = 1;
            positionOnGrid = new ArrayList();
            ShipCells = new bool[_decks];
            for (int i = 0; i < _decks; i++)
                ShipCells[i] = true;
        }

        public bool[] rotatedCells
        {
            get
            {
                return ShipCells;
            }
        }

        /// <summary>
        /// Rotate ship
        /// </summary>
        public void Rotate()
        {
            int tmp = height;
            height = width;
            width = tmp;
        }

        /// <summary>
        /// Calculate index based on given x,y -coordinates
        /// </summary>
        /// <param name="x">X coord</param>
        /// <param name="y">Y coord</param>
        /// <returns>Index in ShipCells</returns>
        public int coordToIndex(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < width && y < height)
                return x + (y * width);
            else
                return -1; ;
        }
    }
}
