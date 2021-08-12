using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class TestingEvents : MonoBehaviour
{
    //public event EventHandler OnGKeyPressed;
    public event EventHandler <OnGKeyPressedEventArgs> OnGKeyPressed;
    public class OnGKeyPressedEventArgs : EventArgs
    {
        public int spaceCountArg;
        public string massageArg;
    }

    //delegates are signatures
    public delegate void TestEventDelegate(float f);

    public event TestEventDelegate OnFloatEvent;

    //public event Action OnActionEvent;
    public event Action <bool, int> OnActionEvent;

    public UnityEvent OnUnityEvent;

    private int spaceCount;

    private void Start()
    {
        Debug.Log("Event started");
        OnGKeyPressed += Testing_OnGKeyPressed; //Example of method subscribed within the same class. Not necessary.
        spaceCount = 0;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            OnGKeyPressed?.Invoke(this, new OnGKeyPressedEventArgs { spaceCountArg = ++spaceCount });
            OnFloatEvent?.Invoke(5.5f);
            OnActionEvent?.Invoke(true, 56);
            OnUnityEvent?.Invoke();
        }                
    }

    private void Testing_OnGKeyPressed(object sender, EventArgs e)
    {
        //Debug.Log("G Key is pressed at sender");
    }
}
