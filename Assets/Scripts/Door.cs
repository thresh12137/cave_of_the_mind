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
    private Outline outlineEffect;
    private bool canBeInteractedWith = false;

    void Start()
    {
        isOpen = doorAnim.GetCurrentAnimatorStateInfo(0).IsName(doorOpenAnimName);
        foreach (Button b in buttons)
        {
            b.pressedEvent += onButtonPressed;
            b.releasedEvent += onButtonReleased;
        }

        outlineEffect = GetComponent<Outline>();
        if (outlineEffect != null)
        {
            outlineEffect.OutlineColor = Interact.outlineColor;
            outlineEffect.OutlineWidth = Interact.outlineWidth;
            outlineEffect.OutlineMode = Interact.outlineMode;
            outlineEffect.enabled = false;
        }
    }

    void Update()
    {
        if (outlineEffect != null)
        {
            if (canBeInteractedWith) outlineEffect.enabled = true;
            else outlineEffect.enabled = false;
            canBeInteractedWith = false;
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
        else
        {
            canBeInteractedWith = distance < maxInteractionDistance;
            return canBeInteractedWith;
        }
    }
}
