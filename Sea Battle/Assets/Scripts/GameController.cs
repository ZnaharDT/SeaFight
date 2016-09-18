using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Assets.Scripts;

public class GameController : MonoBehaviour {

    public Text TwoDeckedButton;
    private int TwoDeckedCount = 3;
    public Text ThreeDeckedButton;
    private int ThreeDeckedCount = 2;

    public GameObject playerField;
    private Grid playerGrid;

    // Use this for initialization
    void Start ()
    {
        TwoDeckedButton.text = "2 - decked Left: 3";
        ThreeDeckedButton.text = "3 - decked Left: 2";
        playerField = GameObject.Find("Grid");
        playerGrid = playerField.GetComponent<Grid>();
        playerGrid.ShipPlacedEvent += OnShipPlaced;
    }

    // Update is called once per frame
    void Update ()
    {
	
	}

    public void OnTwoDeckedButtonClick()
    {
        playerGrid.cursorState = 2;
    }

    public void OnShipPlaced(object sender, ShipPlacedEventArgs e)
    {
        switch (e.decks)
        {
            case 1:
                break;
            case 2:
                TwoDeckedCount--;
                break;
            case 3:
                ThreeDeckedCount--;
                break;
            case 4:
                break;
        }
    }
}
