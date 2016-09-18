using System;
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
}
