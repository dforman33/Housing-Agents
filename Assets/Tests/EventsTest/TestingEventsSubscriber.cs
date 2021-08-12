using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingEventsSubscriber : MonoBehaviour
{
    public TestingEvents tEvents;

    // Start is called before the first frame update
    void Start()
    {
        tEvents = GetComponent<TestingEvents>();
        tEvents.OnGKeyPressed += TEvents_OnGKeyPressed;
        tEvents.OnFloatEvent += TEvents_OnFloatEvent;
        tEvents.OnActionEvent += TEvents_OnActionEvent;
    }

    private void TEvents_OnActionEvent(bool arg1, int arg2)
    {
        Debug.Log(arg1 + " " + arg2);
    }

    private void TEvents_OnFloatEvent(float f)
    {
        Debug.Log("Float passed: " + f);
    }

    private void TEvents_OnGKeyPressed(object sender, TestingEvents.OnGKeyPressedEventArgs e)
    {
        Debug.Log("Subscriber received event. Count: " + e.spaceCountArg);
    }

    public void TestingUnityEvent()
    {
        Debug.Log("Testing a UnityEvent");
    }

}
