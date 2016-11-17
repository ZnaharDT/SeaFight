using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SeaBattle;

public class GameController : MonoBehaviour {   

    public GameObject firstPlayerField;
    public GameObject secondPlayerField;
    public GameObject userInterface;

    public Player firstPlayer;
    public Player secondPlayer;
    public Player activePlayer;

    private UIController uiController;

    void Awake()
    {
        var playerHolder = GameObject.Find("PlayerHolder").GetComponent<PlayerHolder>();
        firstPlayer = playerHolder.player;
        secondPlayer = playerHolder.opponent;
        firstPlayer.PlayerGrid = firstPlayerField.GetComponent<Grid>();
        secondPlayer.PlayerGrid = secondPlayerField.GetComponent<Grid>();

        firstPlayer.PlayerGrid.ShipPlacedEvent += OnShipPlaced;
        secondPlayer.PlayerGrid.ShipPlacedEvent += OnShipPlaced;
        activePlayer = firstPlayer;
        uiController = userInterface.GetComponent<UIController>();
        uiController.ActivePlayerName.text = firstPlayer.Name;

        firstPlayer.PlayerGrid.EndGameEvent += OnEndGame;
        secondPlayer.PlayerGrid.EndGameEvent += OnEndGame;
    }

    // Use this for initialization
    void Start ()
    {
        uiController.FinishButton.interactable = false;
        uiController.OneDeckedLeft.GetComponent<Text>().text = "Left: " + activePlayer.PlayerGrid.OneDeckedCount;
        uiController.TwoDeckedLeft.GetComponent<Text>().text = "Left: " + activePlayer.PlayerGrid.TwoDeckedCount;
        uiController.ThreeDeckedLeft.GetComponent<Text>().text = "Left: " + activePlayer.PlayerGrid.ThreeDeckedCount;
        uiController.FourDeckedLeft.GetComponent<Text>().text = "Left: " + activePlayer.PlayerGrid.FourDeckedCount;
        OnRandom();
    }

    // Update is called once per frame
    void Update ()
    {
	
	}

    private void SwitchPlayer()
    {
        if (activePlayer.Equals(firstPlayer))
        {
            activePlayer = secondPlayer;
            uiController.ActivePlayerName.text = activePlayer.Name;
            if (uiController.FinishButtonClicked)
            {
                activePlayer.PlayerGrid.DeactivateGrid();
                firstPlayer.PlayerGrid.ActivateGrid();
            }
        }
        else
        {
            activePlayer = firstPlayer;
            uiController.ActivePlayerName.text = activePlayer.Name;
            if (uiController.FinishButtonClicked)
            {
                activePlayer.PlayerGrid.DeactivateGrid();
                secondPlayer.PlayerGrid.ActivateGrid();
            }
        }
    }

    public void OnRandom()
    {
        uiController.RandomButton.interactable = false;
        activePlayer.PlayerGrid.RandomShipPlacing();
        uiController.RandomButton.interactable = true;
    }

    public void OnClear()
    {
        uiController.OneDeckedButton.interactable = true;
        uiController.TwoDeckedButton.interactable = true;
        uiController.ThreeDeckedButton.interactable = true;
        uiController.FourDeckedButton.interactable = true;
        uiController.FinishButton.interactable = false;

        activePlayer.PlayerGrid.ClearGrid();

        uiController.OneDeckedLeft.text = "Left: " + activePlayer.PlayerGrid.OneDeckedCount.ToString();
        uiController.TwoDeckedLeft.text = "Left: " + activePlayer.PlayerGrid.TwoDeckedCount.ToString();
        uiController.ThreeDeckedLeft.text = "Left: " + activePlayer.PlayerGrid.ThreeDeckedCount.ToString();
        uiController.FourDeckedLeft.text = "Left: " + activePlayer.PlayerGrid.FourDeckedCount.ToString();
    }

    public void OnFinish()
    {
        if (!uiController.FinishButtonClicked)
        {
            SwitchPlayer();
            OnRandom();
            uiController.FinishButtonClicked = true;
        }
        else
        {
            //TODO: try to do this in easier way
            firstPlayer.PlayerGrid.CurrentShip = null;
            secondPlayer.PlayerGrid.CurrentShip = null;
            
            OnStartGame();
            SwitchPlayer();
        }
    }

    public void OnStartGame()
    {
        Destroy(GameObject.FindWithTag("PlacementMenu"));
        firstPlayer.PlayerGrid.ActivateShooting(OnPlayerMiss);
        firstPlayer.PlayerGrid.HideShips();
        secondPlayer.PlayerGrid.ActivateShooting(OnPlayerMiss);
        secondPlayer.PlayerGrid.HideShips();
    }

    public void OnEndGame(Grid sender, EventArgs e)
    {
        
    }

    private void OnPlayerMiss(object sender, MouseActionEventArgs args)
    {
        SwitchPlayer();
    }

    public void OnOneDeckedButtonClick()
    {
        activePlayer.PlayerGrid.CurrentShip = new Ship(1, activePlayer.PlayerGrid.OnShipDestroyed);
    }

    public void OnTwoDeckedButtonClick()
    {
        activePlayer.PlayerGrid.CurrentShip = new Ship(2, activePlayer.PlayerGrid.OnShipDestroyed);
    }

    public void OnThreeDeckedButtonClick()
    {
        activePlayer.PlayerGrid.CurrentShip = new Ship(3, activePlayer.PlayerGrid.OnShipDestroyed);
    }

    public void OnFourDeckedButtonClick()
    {
        activePlayer.PlayerGrid.CurrentShip = new Ship(4, activePlayer.PlayerGrid.OnShipDestroyed);
    }

    public void OnShipPlaced(object sender, ShipPlacedEventArgs e)
    {
        switch (e.decks)
        {
            case 1:
                activePlayer.PlayerGrid.OneDeckedCount--;
                uiController.OneDeckedLeft.GetComponent<Text>().text = "Left: " + activePlayer.PlayerGrid.OneDeckedCount.ToString();
                if (activePlayer.PlayerGrid.OneDeckedCount <= 0)
                {
                    if (activePlayer.PlayerGrid.TwoDeckedCount <= 0
                        && activePlayer.PlayerGrid.ThreeDeckedCount <= 0
                        && activePlayer.PlayerGrid.FourDeckedCount <= 0)
                        uiController.FinishButton.interactable = true;
                    activePlayer.PlayerGrid.CurrentShip = null;
                    uiController.OneDeckedButton.interactable = false;
                }
                break;
            case 2:
                activePlayer.PlayerGrid.TwoDeckedCount--;
                uiController.TwoDeckedLeft.GetComponent<Text>().text = "Left: " + activePlayer.PlayerGrid.TwoDeckedCount.ToString();
                if (activePlayer.PlayerGrid.TwoDeckedCount <= 0)
                {
                    if (activePlayer.PlayerGrid.OneDeckedCount <= 0
                        && activePlayer.PlayerGrid.ThreeDeckedCount <= 0
                        && activePlayer.PlayerGrid.FourDeckedCount <= 0)
                        uiController.FinishButton.interactable = true;
                    activePlayer.PlayerGrid.CurrentShip = null;
                    uiController.TwoDeckedButton.interactable = false;
                }
                break;
            case 3:
                activePlayer.PlayerGrid.ThreeDeckedCount--;
                uiController.ThreeDeckedLeft.GetComponent<Text>().text = "Left: " + activePlayer.PlayerGrid.ThreeDeckedCount.ToString();
                if (activePlayer.PlayerGrid.ThreeDeckedCount <= 0)
                {
                    if (activePlayer.PlayerGrid.OneDeckedCount <= 0
                        && activePlayer.PlayerGrid.TwoDeckedCount <= 0
                        && activePlayer.PlayerGrid.FourDeckedCount <= 0)
                        uiController.FinishButton.interactable = true;
                    activePlayer.PlayerGrid.CurrentShip = null;
                    uiController.ThreeDeckedButton.interactable = false;
                }
                break;
            case 4:
                activePlayer.PlayerGrid.FourDeckedCount--;
                uiController.FourDeckedLeft.GetComponent<Text>().text = "Left: " + activePlayer.PlayerGrid.FourDeckedCount.ToString();
                if (activePlayer.PlayerGrid.FourDeckedCount <= 0)
                {
                    if (activePlayer.PlayerGrid.OneDeckedCount <= 0
                        && activePlayer.PlayerGrid.TwoDeckedCount <= 0
                        && activePlayer.PlayerGrid.ThreeDeckedCount <= 0)
                        uiController.FinishButton.interactable = true;
                    activePlayer.PlayerGrid.CurrentShip = null;
                    uiController.FourDeckedButton.interactable = false;
                }
                break;
        }
    }
}
