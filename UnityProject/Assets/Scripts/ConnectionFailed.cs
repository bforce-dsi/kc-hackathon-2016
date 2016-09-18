using UnityEngine;
using System.Collections;

public class ConnectionFailed : MonoBehaviour
{

    public GameObject ConnectionMenu;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnBack()
    {
        gameObject.SetActive(false);
        ConnectionMenu.SetActive(true);
    }

    public void OnConnectionFailed()
    {
        gameObject.SetActive(true);
    }
}
