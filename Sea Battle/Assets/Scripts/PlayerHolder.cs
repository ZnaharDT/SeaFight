using UnityEngine;
using System.Collections;
using SeaBattle;

public class PlayerHolder : MonoBehaviour {

    public Player opponent;
    public Player player;

    // Use this for initialization
    void Start () {
        Object.DontDestroyOnLoad(this);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
