using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using Assets.Scripts;
using SeaBattle;
using System.Collections.Generic;
using System.Linq;

public class Grid : MonoBehaviour, ISeaBattleRules
{    
    static readonly Vector2 INVALID_COORDS = new Vector2(-1, -1);
    static readonly GridPoint INVALID_CELL_POS = new GridPoint(-1, -1);
    static readonly int INVALID_CELL_IDX = -1;

    public float cubeSize = 0.9f;
    public float coordsMultiplier = 0.9f;
    public int gridWidth;
    public int gridHeight;

    private GridPoint hooverCellPos = new GridPoint(-1, -1);
    private GridCell[,] grid;
    private int lastMouseIdx;
    private List<GridPoint> nowHightLighted = new List<GridPoint>();
    private List<Ship> ships = new List<Ship>();

    public Ship CurrentShip { get; set; }

    public int CursorState { get; set; }

    public int OneDeckedCount { get; set; }

    public int TwoDeckedCount { get; set; }

    public int ThreeDeckedCount { get; set; }

    public int FourDeckedCount { get; set; }

    public delegate void ShipPlacedEventHandler(object sender, ShipPlacedEventArgs e);

    public event ShipPlacedEventHandler ShipPlacedEvent;

    public delegate void EndGameEventHandler(Grid sender, EventArgs e);

    public event EndGameEventHandler EndGameEvent;

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
                cube.AddComponent(typeof(CellClick));
                cube.GetComponent<CellClick>().position = new GridPoint(i, j);


                grid[i, j] = new GridCell(cube, new GridPoint(i, j));
            }
        }

        //RandomShipPlacing();
        // set colors for first time drawing
        ResetHighlighting();
        // make a default dragged building
        CurrentShip = null;
    }

    void Update()
    {
        if (CurrentShip != null)
        {
            ResetHighlighting();
            ShipPositionHighlighting();
            return;
        }
        if (Input.GetMouseButtonDown(0) && CurrentShip != null)
        {
            CurrentShip.Rotate();
            lastMouseIdx = INVALID_CELL_IDX;
        }
    }

    private List<GridPoint> GetFreeCells()
    {
        List<GridPoint> freeCells = new List<GridPoint>();
        for (int i = 0; i < grid.GetLength(0); i++)
            for (int j = 0; j < grid.GetLength(1); j++)
                if (grid[i,j].State == CellState.Empty)
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
                shipAdded = CheckFitRandom(new Ship(1, OnShipDestroyed), randomRange[UnityEngine.Random.Range(0, randomRange.Count)]);
            }
        }
        for (int i = 0; i < twoDeckedCount; i++)
        {
            shipAdded = false;
            while (!shipAdded)
            {
                List<GridPoint> randomRange = GetFreeCells();
                shipAdded = CheckFitRandom(new Ship(2, OnShipDestroyed), randomRange[UnityEngine.Random.Range(0, randomRange.Count)]);
            }
        }
        for (int i = 0; i < threeDeckedCount; i++)
        {
            shipAdded = false;
            while (!shipAdded)
            {
                List<GridPoint> randomRange = GetFreeCells();
                shipAdded = CheckFitRandom(new Ship(3, OnShipDestroyed), randomRange[UnityEngine.Random.Range(0, randomRange.Count)]);
            }
        }
        for (int i = 0; i < fourDeckedCount; i++)
        {
            shipAdded = false;
            while (!shipAdded)
            {
                List<GridPoint> randomRange = GetFreeCells();
                shipAdded = CheckFitRandom(new Ship(4, OnShipDestroyed), randomRange[UnityEngine.Random.Range(0, randomRange.Count)]);
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
                    if (grid[indexInGrid.i, indexInGrid.j].State == CellState.ReservedByShip)
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
            CheckFit(CurrentShip);
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
                    return;

                int indexInBuilding = ship.coordToIndex(x, y);

                // if this index inside the building is reserved, check if the underlying grid cell is reserved
                if (!shipCells[indexInBuilding])
                    continue;
                grid[indexInGrid.i, indexInGrid.j].ChangeColor(grid[indexInGrid.i, indexInGrid.j].State ==
                                                               CellState.ReservedByShip
                    ? Color.red
                    : Color.magenta);
                nowHightLighted.Add(indexInGrid);
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
            if (grid[cell.i, cell.j].State == CellState.ReservedByShip || grid[cell.i, cell.j].State == CellState.ReservedNearShip)
                rightPlace = false;
        if (rightPlace)
        {
            foreach (GridPoint cell in nowHightLighted)
            {
                grid[cell.i, cell.j].State = CellState.ReservedByShip;
            }
            //TODO: Optimize that shit
            foreach (var cell in nowHightLighted)
            {
                if (cell.i < 9)
                {
                    if (grid[cell.i + 1, cell.j].State != CellState.ReservedByShip)
                    {
                        grid[cell.i + 1, cell.j].State = CellState.ReservedNearShip;
                        currentShip.NearShipCells.Add(grid[cell.i + 1, cell.j]);
                    }
                    if (cell.j < 9)
                        if (grid[cell.i + 1, cell.j + 1].State != CellState.ReservedByShip)
                        {
                            grid[cell.i + 1, cell.j + 1].State = CellState.ReservedNearShip;
                            currentShip.NearShipCells.Add(grid[cell.i + 1, cell.j + 1]);
                        }
                    if (cell.j > 0)
                        if (grid[cell.i + 1, cell.j - 1].State != CellState.ReservedByShip)
                        {
                            grid[cell.i + 1, cell.j - 1].State = CellState.ReservedNearShip;
                            currentShip.NearShipCells.Add(grid[cell.i + 1, cell.j - 1]);
                        }
                }
                if (cell.i > 0)
                {
                    if (grid[cell.i - 1, cell.j].State != CellState.ReservedByShip)
                    {
                        grid[cell.i - 1, cell.j].State = CellState.ReservedNearShip;
                        currentShip.NearShipCells.Add(grid[cell.i - 1, cell.j]);
                    }
                    if (cell.j < 9)
                        if (grid[cell.i - 1, cell.j + 1].State != CellState.ReservedByShip)
                        {
                            grid[cell.i - 1, cell.j + 1].State = CellState.ReservedNearShip;
                            currentShip.NearShipCells.Add(grid[cell.i - 1, cell.j + 1]);
                        }
                    if (cell.j > 0)
                        if (grid[cell.i - 1, cell.j - 1].State != CellState.ReservedByShip)
                        {
                            grid[cell.i - 1, cell.j - 1].State = CellState.ReservedNearShip;
                            currentShip.NearShipCells.Add(grid[cell.i - 1, cell.j - 1]);
                        }
                }
                if (cell.j < 9)
                {
                    if (grid[cell.i, cell.j + 1].State != CellState.ReservedByShip)
                    {
                        grid[cell.i, cell.j + 1].State = CellState.ReservedNearShip;
                        currentShip.NearShipCells.Add(grid[cell.i, cell.j + 1]);
                    }
                    if (cell.i < 9)
                        if (grid[cell.i + 1, cell.j + 1].State != CellState.ReservedByShip)
                        {
                            grid[cell.i + 1, cell.j + 1].State = CellState.ReservedNearShip;
                            currentShip.NearShipCells.Add(grid[cell.i + 1, cell.j + 1]);
                        }
                    if (cell.i > 0)
                        if (grid[cell.i - 1, cell.j + 1].State != CellState.ReservedByShip)
                        {
                            grid[cell.i - 1, cell.j + 1].State = CellState.ReservedNearShip;
                            currentShip.NearShipCells.Add(grid[cell.i - 1, cell.j + 1]);
                        }
                }
                if (cell.j > 0)
                {
                    if (grid[cell.i, cell.j - 1].State != CellState.ReservedByShip)
                    {
                        grid[cell.i, cell.j - 1].State = CellState.ReservedNearShip;
                        currentShip.NearShipCells.Add(grid[cell.i, cell.j - 1]);
                    }
                    if (cell.i < 9)

                        if (grid[cell.i + 1, cell.j - 1].State != CellState.ReservedByShip)
                        {
                            grid[cell.i + 1, cell.j - 1].State = CellState.ReservedNearShip;
                            currentShip.NearShipCells.Add(grid[cell.i + 1, cell.j - 1]);
                        }
                    if (cell.i > 0)
                        if (grid[cell.i - 1, cell.j - 1].State != CellState.ReservedByShip)
                        {
                            grid[cell.i - 1, cell.j - 1].State = CellState.ReservedNearShip;
                            currentShip.NearShipCells.Add(grid[cell.i - 1, cell.j - 1]);
                        }
                }
                grid[cell.i, cell.j].ChangeColor(Color.black);
            }

            currentShip.PositionOnGrid.AddRange(nowHightLighted);
            ships.Add(currentShip);
            
            if (ShipPlacedEvent != null)
                ShipPlacedEvent(this, new ShipPlacedEventArgs(currentShip.decks));
        }
        return rightPlace;
    }

    public void ActivateShooting(GridCell.MissedShotEventHandler missedShotEventHandler)
    {
        for (int i = 0; i < grid.GetLength(0); i++)
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                grid[i, j].MissedShotEvent += missedShotEventHandler;
                grid[i, j].HitEvent += OnHit;
            }
    }

    public void ActivateGrid()
    {
        for (int i = 0; i < grid.GetLength(0); i++)
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                if (grid[i, j].cellObject.GetComponent<CellClick>() != null)
                    grid[i, j].cellObject.GetComponent<CellClick>().enabled = true;
            }
    }

    public void DeactivateGrid()
    {
        for (int i = 0; i < grid.GetLength(0); i++)
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                if (grid[i, j].cellObject.GetComponent<CellClick>() != null)
                    grid[i, j].cellObject.GetComponent<CellClick>().enabled = false;
            }
    }

    /// <summary>
    /// CellMouseOver event handler. Tracing now hovering cell
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnCellMouseOver(object sender, MouseActionEventArgs e)
    {
        hooverCellPos = e.Point;
    }

    public void OnCellClick(object sender, MouseActionEventArgs e)
    {
        grid[e.Point.i, e.Point.j].Shoot();
    }

    public void OnHit(object sender, MouseActionEventArgs e)
    {
        foreach (var ship in ships)
        {
            if (ship.PositionOnGrid.Exists(gp => gp == e.Point))
            {
                ship.Hit(e.Point);
                return;
            }
        }
    }

    public void OnShipDestroyed(Ship sender, EventArgs e)
    {
        ships.Remove(sender);
        if (ships.Count == 0)
            if (EndGameEvent != null)
                EndGameEvent(this, new EventArgs());

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
                switch (grid[i, j].State)
                {
                    case CellState.ReservedByShip:
                        grid[i, j].ChangeColor(Color.black);
                        break;
                    case CellState.ReservedNearShip:
                        grid[i, j].ChangeColor(Color.gray);
                        break;
                    case CellState.ShootedShip:
                        grid[i, j].ChangeColor(Color.red);
                        break;
                    default:
                        grid[i, j].ChangeColor(Color.white);
                        break;
                }
            }
        }
    }

    public void HideShips()
    {
        for (int i = 0; i < gridHeight; i++)
        {
            for (int j = 0; j < gridWidth; j++)
            {
                grid[i, j].ChangeColor(Color.white);
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
                grid[i, j].State = CellState.Empty;
            }
        }
        ResetHighlighting();
        ships = new List<Ship>();
        CurrentShip = null;
    }
}


