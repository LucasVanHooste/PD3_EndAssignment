using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoMonoBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	IEnumerable Update () {

        //wait for player to push button
        yield return new WaitUntil( () => Input.GetMouseButtonDown(1) );
        //wait 2 seconds
        yield return new WaitForSeconds(2);
        //play animation
        StartAnimation();
	}

    private void StartAnimation()
    {
        throw new NotImplementedException();
    }

    IEnumerator<int> PowerReeks(int i)
    {
        yield return i * i;
    }
}
