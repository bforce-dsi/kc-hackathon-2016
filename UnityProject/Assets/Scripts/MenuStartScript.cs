using UnityEngine;
using System.Collections;
using System.Threading;
using UnityEngine.UI;

public class MenuStartScript : MonoBehaviour
{
    public GameObject ConnectMenu;
    public Text QueryText;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnBtnFindPatient_Click()
    {
        gameObject.SetActive(false);

        Canvas.ForceUpdateCanvases();
        Thread.Sleep(1000);
        ConnectMenu.SetActive(true);
    }
}
