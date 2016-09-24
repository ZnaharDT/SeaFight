using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Assets.Scripts;

public class GameController : MonoBehaviour {

    public Button OneDeckedButton;
    public Button TwoDeckedButton;
    public Button ThreeDeckedButton;
    public Button FourDeckedButton;
    
    private int OneDeckedCount = 4;
    private int TwoDeckedCount = 3;
    private int ThreeDeckedCount = 2;
    private int FourDeckedCount = 1;

    public GameObject playerField;
    private Grid playerGrid;

    // Use this for initialization
    void Start ()
    {
        OneDeckedButton.GetComponentInChildren<Text>().text = "1 - decked Left: 4";
        TwoDeckedButton.GetComponentInChildren<Text>().text = "2 - decked Left: 3";
        ThreeDeckedButton.GetComponentInChildren<Text>().text = "3 - decked Left: 2";
        FourDeckedButton.GetComponentInChildren<Text>().text = "4 - decked Left: 1";
        playerField = GameObject.Find("Grid");
        playerGrid = playerField.GetComponent<Grid>();
        playerGrid.ShipPlacedEvent += OnShipPlaced;
    }

    // Update is called once per frame
    void Update ()
    {
	
	}

    public void OnOneDeckedButtonClick()
    {
        playerGrid.CurrentShip = new Ship(1);
    }

    public void OnTwoDeckedButtonClick()
    {
        playerGrid.CurrentShip = new Ship(2);
    }

    public void OnThreeDeckedButtonClick()
    {
        playerGrid.CurrentShip = new Ship(3);
    }

    public void OnFourDeckedButtonClick()
    {
        playerGrid.CurrentShip = new Ship(4);
    }
    public void OnShipPlaced(object sender, ShipPlacedEventArgs e)
    {
        switch (e.decks)
        {
            case 1:
                OneDeckedCount--;
                OneDeckedButton.GetComponentInChildren<Text>().text = "2 - decked Left: " + OneDeckedCount.ToString();
                if (OneDeckedCount <= 0)
                {
                    playerGrid.CurrentShip = null;
                    OneDeckedButton.interactable = false;
                }
                break;
            case 2:
                TwoDeckedCount--;
                TwoDeckedButton.GetComponentInChildren<Text>().text = "2 - decked Left: " + TwoDeckedCount.ToString();
                if (TwoDeckedCount <= 0)
                {
                    playerGrid.CurrentShip = null;
                    TwoDeckedButton.interactable = false;
                }
                break;
            case 3:
                ThreeDeckedCount--;
                ThreeDeckedButton.GetComponentInChildren<Text>().text = "3 - decked Left: " + ThreeDeckedCount.ToString();
                if (ThreeDeckedCount <= 0)
                {
                    playerGrid.CurrentShip = null;
                    ThreeDeckedButton.interactable = false;
                }
                break;
            case 4:
                FourDeckedCount--;
                FourDeckedButton.GetComponentInChildren<Text>().text = "4 - decked Left: " + FourDeckedCount.ToString();
                if (FourDeckedCount <= 0)
                {
                    playerGrid.CurrentShip = null;
                    FourDeckedButton.interactable = false;
                }
                break;
        }
    }
}
