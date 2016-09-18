using UnityEngine;
using System.Collections;

public class MenuActions : MonoBehaviour
{

    public UDPClient Client;
    public GameObject StartingMenu;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnBtnAllergy_Click()
    {
        Client.SendEmptyMessage(UDPHost.PacketHeader.PacketTypes.Allergy);
    }

    public void OnDistraction()
    {
        Client.SendEmptyMessage(UDPHost.PacketHeader.PacketTypes.Distraction);
    }

    public void OnWelcome()
    {
        Client.SendEmptyMessage(UDPHost.PacketHeader.PacketTypes.Welcome);
    }

    public void OnFacts()
    {
        Client.SendEmptyMessage(UDPHost.PacketHeader.PacketTypes.Facts);
    }

    public void OnBack()
    {
        gameObject.SetActive(false);
        StartingMenu.SetActive(true);
    }
}
