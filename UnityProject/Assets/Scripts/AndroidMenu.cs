using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AndroidMenu : MonoBehaviour
{

    public GameObject OutputLabel;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnBtnTest_Click()
    {
        var text = OutputLabel.GetComponent<GUIText>();

        if (text != null)
            text.text = "Button Pressed";
    }
}
