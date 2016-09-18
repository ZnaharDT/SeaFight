using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using Assets.Scripts;

public class Grid : MonoBehaviour
{
    static readonly Vector2 INVALID_COORDS = new Vector2(-1, -1);
    const int INVALID_CELL_IDX = -1;

    public float cubeSize = 0.9f;
    public int gridWidth;
    public int gridHeight;

    public int cursorState;

    private GridCell[,] grid; // mark occupied cells here from bottom left to top right
    private Ship currentShip;
    private int lastMouseIdx;
    private ArrayList nowHightLighted = new ArrayList();
    private ArrayList ships = new ArrayList();

    void Start()
    {
        // center the grid to screen. fugly implementation
        //transform.position = new Vector3(-(_GridWidth / 2f) + 0.5f, -(_GridHeight / 2f) + 0.5f, 0);

        // create a grid of "GridCell" cubes
        grid = new GridCell[gridWidth,gridHeight];
        for (int i = 0; i < gridHeight; i++)
        {
            for (int j = 0; j < gridWidth; j++)
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

                cube.transform.parent = transform;
                cube.transform.localPosition = new Vector2(j, i);
                cube.transform.localScale = Vector3.one * cubeSize;

                grid[i, j] = new GridCell(cube);
            }
        }
        
        // set colors for first time drawing
        resetHighlighting();

        // make a default dragged building
        currentShip = new Ship(4);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            currentShip.Rotate();
            lastMouseIdx = INVALID_CELL_IDX;
        }
        switch (cursorState)
        {
            case 0:
                break;
            case 1:
                currentShip = new Ship(1);
                ShipPositionHighlighting();
                break;
            case 2:
                currentShip = new Ship(2);
                ShipPositionHighlighting();
                break;
            case 3:
                currentShip = new Ship(3);
                ShipPositionHighlighting();
                break;
            case 4:
                currentShip = new Ship(4);
                ShipPositionHighlighting();
                break;
        }
        
    }
    
    /// <summary>
    /// Highlights position to place ship
    /// </summary>
    private void ShipPositionHighlighting()
    {     
        // check which world coordinate the mouse is over. Since gridCells are 1x1 units, 
        // it's translateable to grid coordinates straight away. Should work well enough
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPos -= transform.position - new Vector3(0.5f, 1f, 0f);

        int hoverIndex = coordToIndex((int)worldPos.x, (int)worldPos.y);

        // only check if hovered cell is valid and changed 
        if (hoverIndex != INVALID_CELL_IDX && hoverIndex != lastMouseIdx)
        {
            resetHighlighting();
            checkFit(currentShip, worldPos);
        }
    }

   /// <summary>
   /// Calculate index in grid based on coords
   /// </summary>
   /// <param name="x">X coord</param>
   /// <param name="y">Y coord</param>
   /// <returns>Position on grid</returns>
    private int coordToIndex(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < gridWidth && y < gridHeight)
            return x + (y * gridWidth);
        else
            return INVALID_CELL_IDX;
    }

    /// <summary>
    /// Calculate grid coords based on given index
    /// </summary>
    /// <param name="i">Index to calculate coords</param>
    /// <returns>Vector2 coords</returns>
    private Vector2 indexToCoord(int i)
    {
        if (i >= 0 && i < grid.Length)
            return new Vector2(i % gridWidth, i / gridWidth);
        else
            return INVALID_COORDS;
    }

    /// <summary>
    /// Placing current ship on grid
    /// </summary>
    private void PlaceShip(Ship currentShip)
    {
        bool rightPlace = true;
        foreach (int[] cell in nowHightLighted)
            if (grid[cell[0], cell[1]].reservedByShip || grid[cell[0], cell[1]].reservedNearShip)
                rightPlace = false;
        if (rightPlace)
        {
            //currentShip = new TwoDeckedShip();            
            foreach (int[] cell in nowHightLighted)
            {
                currentShip.positionOnGrid.Add(cell);
                if (cell[0] < 9)
                {
                    grid[cell[0] + 1, cell[1]].reservedNearShip = true;
                    if (cell[1] < 9)
                    grid[cell[0] + 1, cell[1] + 1].reservedNearShip = true;
                    if (cell[1] > 0)
                    grid[cell[0] + 1, cell[1] - 1].reservedNearShip = true;
                }
                if (cell[0] > 0)
                {
                    grid[cell[0] - 1, cell[1]].reservedNearShip = true;
                    if (cell[1] < 9)
                        grid[cell[0] - 1, cell[1] + 1].reservedNearShip = true;
                    if (cell[1] > 0)
                        grid[cell[0] - 1, cell[1] - 1].reservedNearShip = true;
                }
                if (cell[1] < 9)
                {
                    grid[cell[0], cell[1] + 1].reservedNearShip = true;
                    if (cell[0] < 9)
                        grid[cell[0] + 1, cell[1] + 1].reservedNearShip = true;
                    if (cell[0] > 0)
                        grid[cell[0] - 1, cell[1] + 1].reservedNearShip = true;
                }
                if (cell[1] > 0)
                {
                    grid[cell[0], cell[1] - 1].reservedNearShip = true;
                    if (cell[0] < 9)
                        grid[cell[0] + 1, cell[1] - 1].reservedNearShip = true;
                    if (cell[0] > 0)
                        grid[cell[0] - 1, cell[1] - 1].reservedNearShip = true;
                }
                grid[cell[0], cell[1]].reservedByShip = true;
                grid[cell[0], cell[1]]._GO.GetComponent<Renderer>().material.color = Color.black;
            }
            ships.Add(currentShip);
            if (ShipPlacedEvent != null)
                ShipPlacedEvent(this, new ShipPlacedEventArgs(currentShip.decks));
        }

    }

    
    /// <summary>
    /// Check fitting ship to the grid
    /// </summary>
    /// <param name="ship">Current ship</param>
    /// <param name="worldPosition">Position in world</param>
    void checkFit(Ship ship, Vector3 worldPosition)
    {
        // bottom left coords of building in grid
        Vector2 startCoords = worldPosition;
        bool[] shipCells = ship.rotatedCells;

        nowHightLighted.Clear();
        // loop through building cells and see which cells are reserved in both building and grid
        for (int y = 0; y < ship.height; y++)
        {
            for (int x = 0; x < ship.width; x++)
            {
                // this building cell matches this index in grid

                int[] indexInGrid = { (int)(startCoords.y + y), (int)(startCoords.x + x) };
                if (indexInGrid[0] == INVALID_CELL_IDX || indexInGrid[1] == INVALID_CELL_IDX)
                    continue;

                int indexInBuilding = ship.coordToIndex(x, y);

                // if this index inside the building is reserved, check if the underlying grid cell is reserved
                if (shipCells[indexInBuilding])
                {
                    grid[indexInGrid[0], indexInGrid[1]]._GO.GetComponent<Renderer>().material.color = grid[indexInGrid[0], indexInGrid[1]].reservedByShip ? Color.red : Color.magenta;
                    nowHightLighted.Add(indexInGrid);                   
                       
                }
            }
        }
        if (Input.GetMouseButtonDown(1))
            PlaceShip(ship);
    }

    /// <summary>
    /// Refresh all the colors on the grid
    /// </summary>
    void resetHighlighting()
    {
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                // default coloring for reserved and free grid tiles
                if (grid[i, j].reservedByShip)
                    grid[i, j]._GO.GetComponent<Renderer>().material.color = Color.black;
                else if (grid[i, j].reservedNearShip)
                    grid[i, j]._GO.GetComponent<Renderer>().material.color = Color.gray;
                else grid[i, j]._GO.GetComponent<Renderer>().material.color = Color.white;
            }
        }
    }

    /// <summary>
    /// Represents one cell on the grid
    /// </summary>
    public class GridCell
    {
        public bool reservedByShip;
        public bool reservedNearShip;

        public GameObject _GO;        

        public GridCell(GameObject cube)
        {
            _GO = cube;
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
                return INVALID_CELL_IDX;
        }
    }

    /// <summary>
    /// Represents two-decked ships in game
    /// </summary>
    /*public class TwoDeckedShip : Ship
    {
        public TwoDeckedShip()
        {
            width = 2;
            height = 1;
            positionOnGrid = new ArrayList();
            ShipCells = new bool[] { true, true };
        }
    }*/

    /// <summary>
    /// Represents three-decked ships in game
    /// </summary>
    /*public class ThreeDeckedShip : Ship
    {       
        // reserved status of building's cells in it's local space from bottom left to top right
        
        public ThreeDeckedShip()
        {
            width = 3;
            height = 1;
            positionOnGrid = new ArrayList();
            ShipCells = new bool[] { true, true, true };
        }
    }*/

    /*public class TwoDeckedShip
    {
        private const int ROT_0 = 0;
        private const int ROT_90 = 1;
        private const int ROT_190 = 0;
        private const int ROT_270 = 1;
        private const int ROT_COUNT = 2;

        public const int WIDTH = 2;
        public const int HEIGHT = 2;

        public ArrayList positionOnGrid = new ArrayList();

        private int rotation;
        private int length;

        // reserved status of building's cells in it's local space from bottom left to top right
        private bool[][] _MyCells = new bool[][]{
             //         bl   br   tl   tr
             new bool[]{true,true,false,false},
             new bool[]{false,true,false,true},
             new bool[]{false,false,true,true},
             new bool[]{true,false,true,false}
         };


        public bool[] rotatedCells
        {
            get
            {
                return _MyCells[rotation];
            }
        }

        public TwoDeckedShip(int _length)
        {
        }

        public void rotateRight()
        {
            rotation = ++rotation % ROT_COUNT;
        }

        public void rotateLeft()
        {
            if (--rotation < 0)
                rotation += ROT_COUNT;
        }

        // calculate index based on given x,y -coordinates
        public int coordToIndex(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < WIDTH && y < HEIGHT)
                return x + (y * WIDTH);
            else
                return INVALID_CELL_IDX;
        }
    }*/    

    public delegate void ShipPlacedEventHandler(object sender, ShipPlacedEventArgs e);

    public event ShipPlacedEventHandler ShipPlacedEvent;
}


