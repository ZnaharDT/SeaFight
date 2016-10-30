using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    public void OnStart()
    {
        SceneManager.LoadScene("OpponentChoose");
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
