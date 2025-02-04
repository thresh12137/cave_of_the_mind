using NUnit.Framework.Interfaces;
using System.Runtime.InteropServices;
using System.Transactions;
using UnityEngine;
using UnityEngine.Rendering;

public class Door : MonoBehaviour, IInteractable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public float interactionDistance;
    //Uncomment if we want to display text to interact with the door
    //public GameObject interactionText;

    public string doorOpenAnimName, doorCloseAnimName;

   

    void Start()
    {
        Interact.interactEvent += onInteract;
    }

    void openDoor() {

         //Gets the parent object of the door and door hinge
        GameObject doorParent = gameObject;

        Animator doorAnim = doorParent.GetComponent<Animator>();
        
        if (Input.GetKeyDown(KeyCode.J)) {
            print("pressed");

            if (doorAnim.GetCurrentAnimatorStateInfo(0).IsName(doorOpenAnimName)) {

                print("fired");

                doorAnim.ResetTrigger("Open");
                doorAnim.SetTrigger("Close");

            }
            if (doorAnim.GetCurrentAnimatorStateInfo(0).IsName(doorCloseAnimName)) {

                doorAnim.ResetTrigger("Close");
                doorAnim.SetTrigger("Open");

                    }
        }
    }

    public void onInteract(InteractEventArgs args)
    {
        if (args.target == gameObject) {
            openDoor();
            args.interactResponseCallback(new InteractResponseEventArgs(gameObject, true));
        }
    }

}
