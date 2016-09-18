using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.UI;

public class MenuConnect : MonoBehaviour
{

    public GameObject StartMenu;
    public GameObject ActionsMenu;
    public UDPClient Client;
    
    public Text IpAddressText;

    string PatientName;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnBtnBack_Click()
    {
        gameObject.SetActive(false);
        StartMenu.SetActive(true);
    }

    public void OnBtnConnect_Click()
    {
        var ipAddress = IpAddressText.text.Trim();
        Client.Connect(ipAddress);
    }

    public void OnConnected()
    {
        gameObject.SetActive(false);
        ActionsMenu.SetActive(true);
    }

    public void OnPatientFound(string patient)
    {
        gameObject.SetActive(true);
        PatientName = patient;
    }
}
