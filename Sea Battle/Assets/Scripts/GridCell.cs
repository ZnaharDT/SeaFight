using UnityEngine;
using System.Collections;
using System;
using Assets.Scripts;


/// <summary>
/// Represents one cell on the grid
/// </summary>
public class GridCell {

    public bool reservedByShip;
    public bool reservedNearShip;

    public GameObject cellObject;

    /// <summary>
    /// Initialize GridCell object
    /// </summary>
    /// <param name="cube">Primitive cube, that current GridCell will represent</param>
    public GridCell(GameObject cube)
    {
        cellObject = cube;
    }

    /// <summary>
    /// Change color of current cube
    /// </summary>
    /// <param name="_color">New color</param>
    public void ChangeColor(Color _color)
    {
        cellObject.GetComponent<Renderer>().material.color = _color;
    }
}
