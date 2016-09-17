using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AndroidMenu : MonoBehaviour
{

    public Text OutputLabel;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnBtnTest_Click()
    {
        OutputLabel.text = "Button Pressed";
    }
}
