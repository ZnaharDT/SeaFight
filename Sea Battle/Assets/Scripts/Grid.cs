using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using Assets.Scripts;

public class Grid : MonoBehaviour
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
    private ArrayList ships = new ArrayList();

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

    public delegate void ShipPlacedEventHandler(object sender, ShipPlacedEventArgs e);
    public event ShipPlacedEventHandler ShipPlacedEvent;

    void Start()
    {
        // create a grid of "GridCell" cubes
        grid = new GridCell[gridWidth,gridHeight];
        for (int i = 0; i < gridHeight; i++)
        {
            for (int j = 0; j < gridWidth; j++)
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.parent = transform;
                cube.transform.localPosition = new Vector2(j * coordsMultiplier, i * coordsMultiplier);
                cube.transform.localScale = Vector3.one * cubeSize;
                cube.AddComponent<CellHover>().position = new GridPoint(i, j);

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
        if (currentShip == null)
        {
            resetHighlighting();
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            currentShip.Rotate();
            lastMouseIdx = INVALID_CELL_IDX;
        }      
        ShipPositionHighlighting();              
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
            resetHighlighting();
            checkFit(currentShip, cursorWorldPos);
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

                int[] indexInGrid = { hooverCellPos.i + y, hooverCellPos.j + x };
                if (indexInGrid[0] == INVALID_CELL_IDX || indexInGrid[1] == INVALID_CELL_IDX)
                    continue;

                int indexInBuilding = ship.coordToIndex(x, y);

                // if this index inside the building is reserved, check if the underlying grid cell is reserved
                if (shipCells[indexInBuilding])
                {
                    if (grid[indexInGrid[0], indexInGrid[1]].reservedByShip)
                        grid[indexInGrid[0], indexInGrid[1]].ChangeColor(Color.red);
                    else
                        grid[indexInGrid[0], indexInGrid[1]].ChangeColor(Color.magenta);
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
                grid[cell[0], cell[1]].ChangeColor(Color.black);
            }
            ships.Add(currentShip);
            if (ShipPlacedEvent != null)
                ShipPlacedEvent(this, new ShipPlacedEventArgs(currentShip.decks));
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
            return x  + (y * gridWidth);
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
    void resetHighlighting()
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

}


