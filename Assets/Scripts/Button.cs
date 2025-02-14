using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{

    //To play animation for button pressed and not pressed
    public string buttonPressedAnim, buttonUnpressedAnim;

    Animator buttonAnim;
    public GameObject buttonMesh;

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

        buttonAnim = GetComponent<Animator>();
        buttonAnim.speed = 2.0f;

        objectDetectionTrigger.enterEvent += buttonPressedby;
        objectDetectionTrigger.exitEvent += buttonReleasedby;

        isPressed = buttonAnim.GetCurrentAnimatorStateInfo(0).IsName(buttonPressedAnim);
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
                if(buttonAnim != null) animateButtonDown();
            }
        }
        else
        {
            if (isPressed)
            {
                isPressed = false;
                if (releasedEvent != null) releasedEvent(gameObject);
                if (buttonAnim != null) animateButtonUp();
            }
        }
    }

    void buttonPressedby(GameObject obj)
    {
        if(obj != buttonMesh) pressers.Add(obj);
    }
    void buttonReleasedby(GameObject obj)
    {
        pressers.Remove(obj);
    }

    void animateButtonDown()
    {
        buttonAnim.ResetTrigger("Unpressed");
        buttonAnim.SetTrigger("Pressed");

    }
    void animateButtonUp()
    {
        buttonAnim.ResetTrigger("Pressed");
        buttonAnim.SetTrigger("Unpressed");
    }
}

public interface ITriggeredByButton
{
    void onButtonPressed(GameObject button);
    void onButtonReleased(GameObject button);
}
