using UnityEngine;
using System.Collections;

public class Cell : MonoBehaviour {

    public enum CellState
    {
        Normal,
        MissedShot,
        Ship,
        ShotShip,
        ShipDrag,
        ShipDragInvalid,
        ShowDrowned
    }

   

    private const char ShipHitChar = (char)0x72;
    private const char MissedHitChar = (char)0x3D;

    
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
