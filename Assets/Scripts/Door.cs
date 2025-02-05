using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Transactions;
using UnityEngine;
using UnityEngine.Rendering;

public class Door : MonoBehaviour, IInteractable, ITriggeredByButton
{
    public string doorOpenAnimName, doorCloseAnimName;

    public Animator doorAnim;
    public List<Button> buttons;
    public bool areButtonsRequired = false;
    public int numButtonsRequired = 1;
    private int numButtonsPressed = 0;
    private bool isOpen;
    private float maxInteractionDistance = 5;

    void Start()
    {
        isOpen = doorAnim.GetCurrentAnimatorStateInfo(0).IsName(doorOpenAnimName);
        foreach (Button b in buttons)
        {
            b.pressedEvent += onButtonPressed;
            b.releasedEvent += onButtonReleased;
        }
    }

    void openDoor() 
    {
        if (isOpen) return;

        doorAnim.ResetTrigger("Close");
        doorAnim.SetTrigger("Open");
        isOpen = true;
    }

    void closeDoor()
    {
        if (!isOpen) return;

        doorAnim.ResetTrigger("Open");
        doorAnim.SetTrigger("Close");
        isOpen = false;
    }

    public void onInteract(InteractEventArgs args)
    {
        if (areButtonsRequired)
        {
            args.interactResponseCallback(new InteractResponseEventArgs(gameObject, true));
            return;
        }

        if (isOpen) closeDoor();
        else openDoor();

        args.interactResponseCallback(new InteractResponseEventArgs(gameObject, true));
    }

    public void onButtonPressed(GameObject button)
    {
        numButtonsPressed++;
        if(numButtonsPressed >= numButtonsRequired) openDoor();
    }

    public void onButtonReleased(GameObject button)
    {
        numButtonsPressed--;
        if (numButtonsPressed < numButtonsRequired) closeDoor();
    }

    public bool interactionQuery(float distance)
    {
        if (areButtonsRequired) return false;
        else return distance < maxInteractionDistance;
    }
}
