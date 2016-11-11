using UnityEngine;
using System.Collections;
using SeaBattle;

public class SampleShip : MonoBehaviour {

    public float cubeSize = 0.9f;
    public float coordsMultiplier = 0.9f;
    public int gridWidth;
    public int gridHeight;

    private GridCell[] ship;
    // Use this for initialization
    void Start () {
        CreateSampleShip();
	}
	
	// Update is called once per frame
	void Update () {
        ResetHighlighting();
	}

    private void CreateSampleShip()
    {
        ship = new GridCell[3];
        for (int i = 0; i < gridHeight; i++)
        {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.parent = transform;
                cube.transform.localPosition = new Vector2(0, i * coordsMultiplier);
                cube.transform.localScale = Vector3.one * cubeSize;

                ship[i] = new GridCell(cube, new GridPoint(1, 1));
        }
    }

    void ResetHighlighting()
    {
        for (int i = 0; i < ship.GetLength(0); i++)
        {
                // default coloring for reserved and free grid tiles
                switch (ship[i].State)
                {
                    case CellState.ReservedByShip:
                        ship[i].ChangeColor(Color.black);
                        break;
                    case CellState.ReservedNearShip:
                        ship[i].ChangeColor(Color.gray);
                        break;
                    default:
                        ship[i].ChangeColor(Color.white);
                        break;
                }
        }
    }
}
