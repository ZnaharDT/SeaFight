using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SeaBattle;

public class OpponentChose : MonoBehaviour {

    public InputField playerNameField;
    public InputField opponentNameField;

    // Use this for initialization
    void Start () {
        opponentNameField.interactable = false;
	}

    public void OnAI(bool isOn)
    {
        if (isOn)
            opponentNameField.interactable = false;
        else
            opponentNameField.interactable = true;
    }

    public void OnStart()
    {
        var playerHolder = GameObject.Find("PlayerHolder");
        playerHolder.GetComponent<PlayerHolder>().player = new Player(playerNameField.text);
        if (opponentNameField.interactable)
        {
            playerHolder.GetComponent<PlayerHolder>().player = new Player(opponentNameField.text);
            SceneManager.LoadScene("GameVsHuman");
        }
        else
        {
            playerHolder.GetComponent<PlayerHolder>().opponent = new Player("AI");
            SceneManager.LoadScene("GameVsAI");
        }
    }

    // Update is called once per frame
    void Update () {
	
	}
}
