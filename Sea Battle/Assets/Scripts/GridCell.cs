using UnityEngine;
using System.Collections;
using System;
using Assets.Scripts;
using SeaBattle;


/// <summary>
/// Represents one cell on the grid
/// </summary>
public class GridCell
{

    public CellState State { get; set; }
    public GridPoint Position { get; set; }

    public GameObject cellObject;

    public delegate void MissedShotEventHandler(object sender, MouseActionEventArgs args);

    public event MissedShotEventHandler MissedShotEvent;

    public delegate void HitEventHandler(object sender, MouseActionEventArgs args);

    public event HitEventHandler HitEvent;

    /// <summary>
    /// Initialize GridCell object
    /// </summary>
    /// <param name="cube">Primitive cube, that current GridCell will represent</param>
    public GridCell(GameObject cube, GridPoint position)
    {
        cellObject = cube;
        Position = position;
        State = CellState.Empty;
    }

    public void Shoot()
    {
        if (State == CellState.ReservedByShip)
        {
            ChangeColor(Color.red);
            State = CellState.ShootedShip;
            if (HitEvent != null)
                HitEvent(this, new MouseActionEventArgs(Position));
        }
        else if (State == CellState.Empty || State == CellState.ReservedNearShip)
        {
            State = CellState.EmptyShooted;
            ChangeColor(Color.cyan);
            if (MissedShotEvent != null)
                MissedShotEvent(this, new MouseActionEventArgs(Position));
        }
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
