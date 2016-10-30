using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using Assets.Scripts;
using SeaBattle;
using System.Collections.Generic;

public class Grid : MonoBehaviour, ISeaBattleRules
{
    static readonly Vector2 INVALID_COORDS = new Vector2(-1, -1);
    static readonly GridPoint INVALID_CELL_POS = new GridPoint(-1, -1);
    const int INVALID_CELL_IDX = -1;

    public float cubeSize = 0.9f;
    public float coordsMultiplier = 0.9f;
    public int gridWidth;
    public int gridHeight;

    private int cursorState;

    GridPoint hooverCellPos = new GridPoint(-1, -1);
    private GridCell[,] grid;
    private Ship currentShip;
    private int lastMouseIdx;
    private ArrayList nowHightLighted = new ArrayList();
    private List<Ship> ships = new List<Ship>();

    public Ship CurrentShip
    {
        get
        {
            return currentShip;
        }

        set
        {
            currentShip = value;
        }
    }

    public int CursorState
    {
        get
        {
            return cursorState;
        }

        set
        {
            cursorState = value;
        }
    }

    public int OneDeckedCount { get; set; }

    public int TwoDeckedCount { get; set; }

    public int ThreeDeckedCount { get; set; }

    public int FourDeckedCount { get; set; }

    public delegate void ShipPlacedEventHandler(object sender, ShipPlacedEventArgs e);

    public event ShipPlacedEventHandler ShipPlacedEvent;

    void Awake()
    {
    }

    void Start()
    {

        // create a grid of "GridCell" cubes
        OneDeckedCount = 4;
        TwoDeckedCount = 3;
        ThreeDeckedCount = 2;
        FourDeckedCount = 1;

        grid = new GridCell[gridWidth, gridHeight];
        for (int i = 0; i < gridHeight; i++)
        {
            for (int j = 0; j < gridWidth; j++)
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Quad);
                cube.transform.parent = transform;
                cube.transform.localPosition = new Vector2(j * coordsMultiplier, i * coordsMultiplier);
                cube.transform.localScale = Vector3.one * cubeSize;
                cube.tag = "grid_cell";
                cube.AddComponent<CellHover>().position = new GridPoint(i, j);

                grid[i, j] = new GridCell(cube);
            }
        }

        //RandomShipPlacing();
        // set colors for first time drawing
        ResetHighlighting();
        // make a default dragged building
        currentShip = null;
    }

    void Update()
    {
        if (currentShip == null)
        {
            ResetHighlighting();
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            currentShip.Rotate();
            lastMouseIdx = INVALID_CELL_IDX;
        }      
        ShipPositionHighlighting();              
    }

    private List<GridPoint> GetFreeCells()
    {
        List<GridPoint> freeCells = new List<GridPoint>();
        for (int i = 0; i < grid.GetLength(0); i++)
            for (int j = 0; j < grid.GetLength(1); j++)
                if (!grid[i, j].reservedByShip || !grid[i, j].reservedNearShip)
                    freeCells.Add(new GridPoint(i, j));
        return freeCells;
    }

    public void RandomShipPlacing()
    {
        int fourDeckedCount = 1, threeDeckedCount = 2, twoDeckedCount = 3, oneDeckedCount = 4;
        ClearGrid();

        bool shipAdded = false;
        for (int i = 0; i < oneDeckedCount; i++)
        {
            shipAdded = false;
            while (!shipAdded)
            {
                List<GridPoint> randomRange = GetFreeCells();
                shipAdded = CheckFitRandom(new Ship(1), randomRange[UnityEngine.Random.Range(0, randomRange.Count)]);
            }
        }
        for (int i = 0; i < twoDeckedCount; i++)
        {
            shipAdded = false;
            while (!shipAdded)
            {
                List<GridPoint> randomRange = GetFreeCells();
                shipAdded = CheckFitRandom(new Ship(2), randomRange[UnityEngine.Random.Range(0, randomRange.Count)]);
            }
        }
        for (int i = 0; i < threeDeckedCount; i++)
        {
            shipAdded = false;
            while (!shipAdded)
            {
                List<GridPoint> randomRange = GetFreeCells();
                shipAdded = CheckFitRandom(new Ship(3), randomRange[UnityEngine.Random.Range(0, randomRange.Count)]);
            }
        }
        for (int i = 0; i < fourDeckedCount; i++)
        {
            shipAdded = false;
            while (!shipAdded)
            {
                List<GridPoint> randomRange = GetFreeCells();
                shipAdded = CheckFitRandom(new Ship(4), randomRange[UnityEngine.Random.Range(0, randomRange.Count)]);
            }
        }

        ResetHighlighting();
    }

    bool CheckFitRandom(Ship ship, GridPoint cellPos)
    {
        bool[] shipCells = ship.rotatedCells;

        nowHightLighted.Clear();
        //GridPoint cellPos = new GridPoint(UnityEngine.Random.Range(0, gridHeight), UnityEngine.Random.Range(0, gridWidth));
        int rotate = UnityEngine.Random.Range(0, 2);
        if (rotate == 1)
            ship.Rotate();

        // loop through building cells and see which cells are reserved in both building and grid
        for (int y = 0; y < ship.height; y++)
        {
            for (int x = 0; x < ship.width; x++)
            {
                // this building cell matches this index in grid

                GridPoint indexInGrid = new GridPoint( cellPos.i + y, cellPos.j + x );
                if (indexInGrid == INVALID_CELL_POS)
                    return false;

                int indexInBuilding = ship.coordToIndex(x, y);

                // if this index inside the building is reserved, check if the underlying grid cell is reserved
                if (shipCells[indexInBuilding])
                {
                    if (grid[indexInGrid.i, indexInGrid.j].reservedByShip)
                        grid[indexInGrid.i, indexInGrid.j].ChangeColor(Color.red);
                    else
                        grid[indexInGrid.i, indexInGrid.j].ChangeColor(Color.magenta);
                    nowHightLighted.Add(indexInGrid);
                }
            }
        }
        return PlaceShip(ship);
    }

    /// <summary>
    /// Highlights position to place ship
    /// </summary>
    private void ShipPositionHighlighting()
    {     
        // check which world coordinate the mouse is over. Since gridCells are 1x1 units, 
        // it's translateable to grid coordinates straight away. Should work well enough
        Vector3 cursorWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        cursorWorldPos -= transform.position - new Vector3(0.5f, 0.5f, 0f);
        
        // only check if hovered cell is valid and changed 
        if (hooverCellPos != INVALID_CELL_POS)
        {
            ResetHighlighting();
            CheckFit(currentShip);
        }
    }

    /// <summary>
    /// Check fitting ship to the grid
    /// </summary>
    /// <param name="ship">Current ship</param>
    void CheckFit(Ship ship)
    {
        // bottom left coords of building in grid
        //Vector2 startCoords = worldPosition;
        bool[] shipCells = ship.rotatedCells;

        nowHightLighted.Clear();
        // loop through building cells and see which cells are reserved in both building and grid
        for (int y = 0; y < ship.height; y++)
        {
            for (int x = 0; x < ship.width; x++)
            {
                // this building cell matches this index in grid

                GridPoint indexInGrid = new GridPoint(hooverCellPos.i + y, hooverCellPos.j + x);
                if (indexInGrid == INVALID_CELL_POS)
                    continue;

                int indexInBuilding = ship.coordToIndex(x, y);

                // if this index inside the building is reserved, check if the underlying grid cell is reserved
                if (shipCells[indexInBuilding])
                {
                    if (grid[indexInGrid.i, indexInGrid.j].reservedByShip)
                        grid[indexInGrid.i, indexInGrid.j].ChangeColor(Color.red);
                    else
                        grid[indexInGrid.i, indexInGrid.j].ChangeColor(Color.magenta);
                    nowHightLighted.Add(indexInGrid);
                }
            }
        }
        if (Input.GetMouseButtonDown(1))
            PlaceShip(ship);
    }

    /// <summary>
    /// Placing current ship on grid
    /// </summary>
    private bool PlaceShip(Ship currentShip)
    {
        bool rightPlace = true;
        foreach (GridPoint cell in nowHightLighted)
            if (grid[cell.i, cell.j].reservedByShip || grid[cell.i, cell.j].reservedNearShip)
                rightPlace = false;
        if (rightPlace)
        {                
            foreach (GridPoint cell in nowHightLighted)
            {
                currentShip.PositionOnGrid.Add(cell);
                if (cell.i < 9)
                {
                    grid[cell.i + 1, cell.j].reservedNearShip = true;
                    if (cell.j < 9)
                        grid[cell.i + 1, cell.j + 1].reservedNearShip = true;
                    if (cell.j > 0)
                        grid[cell.i + 1, cell.j - 1].reservedNearShip = true;
                }
                if (cell.i > 0)
                {
                    grid[cell.i - 1, cell.j].reservedNearShip = true;
                    if (cell.j < 9)
                        grid[cell.i - 1, cell.j + 1].reservedNearShip = true;
                    if (cell.j > 0)
                        grid[cell.i - 1, cell.j - 1].reservedNearShip = true;
                }
                if (cell.j < 9)
                {
                    grid[cell.i, cell.j + 1].reservedNearShip = true;
                    if (cell.i < 9)
                        grid[cell.i + 1, cell.j + 1].reservedNearShip = true;
                    if (cell.i > 0)
                        grid[cell.i - 1, cell.j + 1].reservedNearShip = true;
                }
                if (cell.j > 0)
                {
                    grid[cell.i, cell.j - 1].reservedNearShip = true;
                    if (cell.i < 9)
                        grid[cell.i + 1, cell.j - 1].reservedNearShip = true;
                    if (cell.i > 0)
                        grid[cell.i - 1, cell.j - 1].reservedNearShip = true;
                }
                grid[cell.i, cell.j].reservedByShip = true;
                grid[cell.i, cell.j].ChangeColor(Color.black);
            }

            currentShip.PositionOnGrid.AddRange(nowHightLighted);
            ships.Add(currentShip);
            
            if (ShipPlacedEvent != null)
                ShipPlacedEvent(this, new ShipPlacedEventArgs(currentShip.decks));
        }
        return rightPlace;
    }

    /// <summary>
    /// Calculate index in grid based on coords
    /// </summary>
    /// <param name="x">X coord</param>
    /// <param name="y">Y coord</param>
    /// <returns>Position on grid</returns>
    private int CoordToIndex(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < gridWidth && y < gridHeight)
            return x  + (y * gridWidth);
        else
            return INVALID_CELL_IDX;
    }

    /// <summary>
    /// Calculate grid coords based on given index
    /// </summary>
    /// <param name="i">Index to calculate coords</param>
    /// <returns>Vector2 coords</returns>
    private Vector2 IndexToCoord(int i)
    {
        if (i >= 0 && i < grid.Length)
            return new Vector2(i % gridWidth, i / gridWidth);
        else
            return INVALID_COORDS;
    }

    /// <summary>
    /// CellMouseOver event handler. Tracing now hovering cell
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnCellMouseOver(object sender, MouseOverEventArgs e)
    {
        hooverCellPos = e.HooverPoint;
    }

    /// <summary>
    /// Refresh all the colors on the grid
    /// </summary>
    void ResetHighlighting()
    {
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                // default coloring for reserved and free grid tiles
                if (grid[i, j].reservedByShip)
                    grid[i, j].ChangeColor(Color.black);
                else if (grid[i, j].reservedNearShip)
                    grid[i, j].ChangeColor(Color.gray);
                else grid[i, j].ChangeColor(Color.white);
            }
        }
    }

    public void ClearGrid()
    {
        OneDeckedCount = 4;
        TwoDeckedCount = 3;
        ThreeDeckedCount = 2;
        FourDeckedCount = 1;

        for (int i = 0; i < gridHeight; i++)
        {
            for (int j = 0; j < gridWidth; j++)
            {
                grid[i, j].reservedByShip = false;
                grid[i, j].reservedNearShip = false;
            }
        }
        ResetHighlighting();
        ships = new List<Ship>();
        currentShip = null;
    }
}


