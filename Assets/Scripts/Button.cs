using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    public ObjectDetectionTrigger objectDetectionTrigger;

    public delegate void ButtonPressed(GameObject button);
    public event ButtonPressed pressedEvent;
    public delegate void ButtonReleased(GameObject button);
    public event ButtonReleased releasedEvent;
    public bool isPressed;
    private List<GameObject> pressers;

    private void Start()
    {
        if (objectDetectionTrigger == null) throw new System.Exception("No objectDetectionTrigger associated with " + gameObject);

        objectDetectionTrigger.enterEvent += buttonPressedby;
        objectDetectionTrigger.exitEvent += buttonReleasedby;

        isPressed = false;
        pressers = new List<GameObject>();
    }

    private void Update()
    {
        if (pressers.Count > 0)
        {
            if (!isPressed)
            {
                isPressed = true;
                if (pressedEvent != null) pressedEvent(gameObject);
                animateButtonDown();
            }
        }
        else
        {
            if (isPressed)
            {
                isPressed = false;
                if (releasedEvent != null) releasedEvent(gameObject);
                animateButtonUp();
            }
        }
    }

    void buttonPressedby(GameObject obj)
    {
        if(obj.GetComponent<Rigidbody>()) pressers.Add(obj); //only accept presses from objects with a rigidbody
    }
    void buttonReleasedby(GameObject obj)
    {
        pressers.Remove(obj);
    }

    void animateButtonDown()
    {
        //TODO make button go down
    }
    void animateButtonUp()
    {
        //TODO make button go up
    }
}

public interface ITriggeredByButton
{
    void onButtonPressed(GameObject button);
    void onButtonReleased(GameObject button);
}
