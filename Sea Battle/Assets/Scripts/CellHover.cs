using UnityEngine;
using System.Collections;
using Assets.Scripts;
using SeaBattle;

public class CellHover : MonoBehaviour {

    public delegate void MouseOverEventHandler(object sender, MouseActionEventArgs e);
    public event MouseOverEventHandler MouseOverEvent;

    public GridPoint position;
    // Use this for initialization
    void Start()
    {
        MouseOverEvent += gameObject.GetComponentInParent<Grid>().OnCellMouseOver;
    }

    // Update is called once per frame
    void Update () {
	
	}

    void OnMouseOver()
    {
        if (MouseOverEvent != null)
            MouseOverEvent(this, new MouseActionEventArgs(position));
    }

   
}
