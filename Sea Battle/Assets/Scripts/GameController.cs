using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SeaBattle;

public class GameController : MonoBehaviour {   

    public GameObject firstPlayerField;
    public GameObject secondPlayerField;
    public GameObject userInterface;

    private Grid activePlayerGrid;
    private Grid firstPlayerGrid;
    private Grid secondPlayerGrid;

    private UIController uiController;

    void Awake()
    {
        activePlayerGrid = firstPlayerField.GetComponent<Grid>();
        activePlayerGrid.ShipPlacedEvent += OnShipPlaced;
        uiController = userInterface.GetComponent<UIController>();
    }

    // Use this for initialization
    void Start ()
    {
        uiController.FinishButton.interactable = false;
        uiController.OneDeckedLeft.GetComponent<Text>().text = "Left: " + activePlayerGrid.OneDeckedCount;
        uiController.TwoDeckedLeft.GetComponent<Text>().text = "Left: " + activePlayerGrid.TwoDeckedCount;
        uiController.ThreeDeckedLeft.GetComponent<Text>().text = "Left: " + activePlayerGrid.ThreeDeckedCount;
        uiController.FourDeckedLeft.GetComponent<Text>().text = "Left: " + activePlayerGrid.FourDeckedCount;
        OnRandom();
    }

    // Update is called once per frame
    void Update ()
    {
	
	}

    private void SwitchPlayer()
    {
        if (activePlayerGrid.Equals(firstPlayerGrid))
            activePlayerGrid = secondPlayerGrid;
        else
            activePlayerGrid = firstPlayerGrid;
    }

    public void OnRandom()
    {
        activePlayerGrid.RandomShipPlacing();
    }

    public void OnClear()
    {
        uiController.OneDeckedButton.interactable = true;
        uiController.TwoDeckedButton.interactable = true;
        uiController.ThreeDeckedButton.interactable = true;
        uiController.FourDeckedButton.interactable = true;
        uiController.FinishButton.interactable = false;

        activePlayerGrid.ClearGrid();

        uiController.OneDeckedLeft.text = "Left: " + activePlayerGrid.OneDeckedCount.ToString();
        uiController.TwoDeckedLeft.text = "Left: " + activePlayerGrid.TwoDeckedCount.ToString();
        uiController.ThreeDeckedLeft.text = "Left: " + activePlayerGrid.ThreeDeckedCount.ToString();
        uiController.FourDeckedLeft.text = "Left: " + activePlayerGrid.FourDeckedCount.ToString();
    }

    public void OnFinish()
    {

    }

    public void OnOneDeckedButtonClick()
    {
        activePlayerGrid.CurrentShip = new Ship(1);
    }

    public void OnTwoDeckedButtonClick()
    {
        activePlayerGrid.CurrentShip = new Ship(2);
    }

    public void OnThreeDeckedButtonClick()
    {
        activePlayerGrid.CurrentShip = new Ship(3);
    }

    public void OnFourDeckedButtonClick()
    {
        activePlayerGrid.CurrentShip = new Ship(4);
    }

    public void OnShipPlaced(object sender, ShipPlacedEventArgs e)
    {
        switch (e.decks)
        {
            case 1:
                activePlayerGrid.OneDeckedCount--;
                uiController.OneDeckedLeft.GetComponent<Text>().text = "Left: " + activePlayerGrid.OneDeckedCount.ToString();
                if (activePlayerGrid.OneDeckedCount <= 0)
                {
                    if (activePlayerGrid.TwoDeckedCount <= 0
                        && activePlayerGrid.ThreeDeckedCount <= 0
                        && activePlayerGrid.FourDeckedCount <= 0)
                        uiController.FinishButton.interactable = true;
                    activePlayerGrid.CurrentShip = null;
                    uiController.OneDeckedButton.interactable = false;
                }
                break;
            case 2:
                activePlayerGrid.TwoDeckedCount--;
                uiController.TwoDeckedLeft.GetComponent<Text>().text = "Left: " + activePlayerGrid.TwoDeckedCount.ToString();
                if (activePlayerGrid.TwoDeckedCount <= 0)
                {
                    if (activePlayerGrid.OneDeckedCount <= 0
                        && activePlayerGrid.ThreeDeckedCount <= 0
                        && activePlayerGrid.FourDeckedCount <= 0)
                        uiController.FinishButton.interactable = true;
                    activePlayerGrid.CurrentShip = null;
                    uiController.TwoDeckedButton.interactable = false;
                }
                break;
            case 3:
                activePlayerGrid.ThreeDeckedCount--;
                uiController.ThreeDeckedLeft.GetComponent<Text>().text = "Left: " + activePlayerGrid.ThreeDeckedCount.ToString();
                if (activePlayerGrid.ThreeDeckedCount <= 0)
                {
                    if (activePlayerGrid.OneDeckedCount <= 0
                        && activePlayerGrid.TwoDeckedCount <= 0
                        && activePlayerGrid.FourDeckedCount <= 0)
                        uiController.FinishButton.interactable = true;
                    activePlayerGrid.CurrentShip = null;
                    uiController.ThreeDeckedButton.interactable = false;
                }
                break;
            case 4:
                activePlayerGrid.FourDeckedCount--;
                uiController.FourDeckedLeft.GetComponent<Text>().text = "Left: " + activePlayerGrid.FourDeckedCount.ToString();
                if (activePlayerGrid.FourDeckedCount <= 0)
                {
                    if (activePlayerGrid.OneDeckedCount <= 0
                        && activePlayerGrid.TwoDeckedCount <= 0
                        && activePlayerGrid.ThreeDeckedCount <= 0)
                        uiController.FinishButton.interactable = true;
                    activePlayerGrid.CurrentShip = null;
                    uiController.FourDeckedButton.interactable = false;
                }
                break;
        }
    }
}
