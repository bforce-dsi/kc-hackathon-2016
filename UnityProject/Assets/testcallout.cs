using UnityEngine;
using System.Collections;
using System.IO;
using System.Net;
using System;
using System.Collections.Generic;
using Assets;

public class testcallout : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
        FhirWrapper data = new FhirWrapper("2744010");

	    List<Dictionary<string, string>> allergies = data.GetPatientAllergies();

	    foreach (Dictionary<string, string> entry in allergies)
	    {
	        foreach (KeyValuePair<string, string> valuePair in entry)
	        {
	            print(valuePair.Key + " " + valuePair.Value);
	        }
	    }


    }

    // Update is called once per frame
    void Update () {
	print("hello world");
	}


}
