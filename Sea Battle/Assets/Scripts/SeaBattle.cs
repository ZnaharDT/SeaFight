using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace SeaBattle
{
    public interface ISeaBattleRules
    {
        int OneDeckedCount { get; set; }
        int TwoDeckedCount  { get; set; }
        int ThreeDeckedCount { get; set; }
        int FourDeckedCount { get; set; }
    }

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

    public class MouseActionEventArgs : EventArgs
    {

        private GridPoint point;

        public MouseActionEventArgs(GridPoint _point)
        {
            this.point = _point;
        }

        public GridPoint Point
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
        protected bool Equals(GridPoint other)
        {
            return i == other.i && j == other.j;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((GridPoint) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (i*397) ^ j;
            }
        }

        public int i;
        public int j;

        public GridPoint(int _i, int _j)
        {
            if (_i >= 10 || _j >= 10)
            {
                i = -1;
                j = -1;
            }
            else
            {
                i = _i;
                j = _j;
            }
        }

        public static bool operator ==(GridPoint p1, GridPoint p2)
        {
            if (p1.i == p2.i)
                if (p1.j == p2.j)
                    return true;
            return false;
        }
        public static bool operator !=(GridPoint p1, GridPoint p2)
        {
            if (p1.i != p2.i)
                if (p1.j != p2.j)
                    return true;
            return false;
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

        public delegate void OnShipDestoyedEventHandler(Ship sender, EventArgs e);

        public event OnShipDestoyedEventHandler OnShipDestroyedEvent;

        public int width;
        public int height;
        public int decks;

        private List<GridPoint> positionOnGrid;
        public List<GridCell> NearShipCells { get; private set; }

        private int rotation;
        protected bool[] ShipCells;

        /// <summary>
        /// Initialize new ship with specified number of decks
        /// </summary>
        /// <param name="_decks">Number of decks</param>
        /// <param name="handler">Method to delete destroyed ship from ship list</param>
        public Ship(int _decks, OnShipDestoyedEventHandler handler)
        {
            OnShipDestroyedEvent = handler;
            width = _decks;
            decks = _decks;
            height = 1;
            PositionOnGrid = new List<GridPoint>();
            ShipCells = new bool[_decks];
            NearShipCells = new List<GridCell>();
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

        public List<GridPoint> PositionOnGrid
        {
            get
            {
                return positionOnGrid;
            }

            set
            {
                positionOnGrid = value;
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

        public void Hit(GridPoint ePoint)
        {
            positionOnGrid.Remove(ePoint);
            if (positionOnGrid.Count == 0)
                DestroyShip();
        }

        private void DestroyShip()
        {
            OnShipDestroyedEvent(this, new EventArgs());
            foreach (var cell in NearShipCells)
            {
                cell.ChangeColor(Color.cyan);
                cell.State = CellState.EmptyShooted;
            }
        }
    }
    
    [Serializable]
    public class PlayersData
    {
        public List<Player> players = new List<Player>();
    }

    [Serializable]
    public abstract class Player
    {
        public string Name { get; set; }      
        public Grid PlayerGrid { get; set; }

        public abstract void RequestShoot();
    }

    public class HumanPlayer : Player
    {
        private int score = 0;

        public HumanPlayer(string _name)
        {
            Name = _name;
        }

        public int Score
        {
            get
            {
                return score;
            }

            set
            {
                score = value;
            }
        }

        public override void RequestShoot()
        {

        }
    }

    public class AIPlayer : Player
    {
        public AIPlayer()
        {
            Name = "AI Player";
        }

        public override void RequestShoot()
        {

        }
    }

    public enum CellState
    {
        ReservedByShip,
        ReservedNearShip,
        ShootedShip,
        Empty,
        EmptyShooted
    }
}
