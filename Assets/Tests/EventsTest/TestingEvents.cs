using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TestingEvents : MonoBehaviour
{

    public event EventHandler OnSpacePressed;
    private void start()
    {
        //This subscribes to the event
        OnSpacePressed += Testing_OnSpacePressed;
    }

    private void Testing_OnSpacePressed(object sender, EventArgs e)
    {
        Debug.Log("Space pressed!");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // we call the event handler like a function
            //space pressed (object sender, EventArgs e)
            //if(OnSpacePressed != null) OnSpacePressed(this, EventArgs.Empty);
            //can use the null operator
            OnSpacePressed?.Invoke(this, EventArgs.Empty);


        }
        
    }

}
