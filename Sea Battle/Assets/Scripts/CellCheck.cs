using UnityEngine;
using System.Collections;

public class CellCheck : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "grid_cell")
            col.gameObject.GetComponent<GridCell>().ChangeColor(Color.magenta);
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.tag == "grid_cell")
            col.gameObject.GetComponent<GridCell>().ChangeColor(Color.magenta);
    }
}
