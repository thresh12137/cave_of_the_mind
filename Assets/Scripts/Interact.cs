using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static Interact;

public class Interact : MonoBehaviour
{
    public static KeyCode interactKey; //global key variable to reference from other scripts
    public KeyCode interactionKey; //key to get from inspector  THIS IS BAD CODE STYLE!  FIX THIS!
    public Transform pos;
    public float maxInteractDistance;
    public delegate void OnInteract(InteractEventArgs args);
    public static event OnInteract interactEvent;
    public delegate void OnInteractPossibleEvent(bool isInteractPossible);
    public static event OnInteractPossibleEvent interactPossibleEvent;
    public delegate void InteractResponse(InteractResponseEventArgs args);
    static bool canInteract = true;
    static GameObject currentInteractionObject = null;
    private double interactResponseNotRecievedTimer = 0d;
    private bool isInteractResponseRecieved = true;
    private bool isInteractPossible = false;

    private void Start()
    {
        interactKey = interactionKey;
        //interactResponseEvent += respondToEvent;
    }

    // Update is called once per frame
    void Update()
    {
        //cast a ray to see what player is looking at
        if (canInteract)
        {
            TryInteract();
        }
        else
        {
            if (Input.GetKeyDown(interactKey) && currentInteractionObject != null)
            {
                //fire interact event with current object
                interactEvent(new InteractEventArgs(gameObject, currentInteractionObject, respondToEvent));
            }
        }

        //failsafe for if no response is recieved from interacted object after 5 seconds.  THIS SHOULD NEVER BE RELEVANT
        if(interactResponseNotRecievedTimer > 0) interactResponseNotRecievedTimer -= Time.deltaTime;
        if (interactResponseNotRecievedTimer <= 0 && !isInteractResponseRecieved)
        {
            canInteract = true;
            currentInteractionObject = null;
        } 
    }

    private void TryInteract()
    {
        RaycastHit hit;
        bool hitBool = Physics.Raycast(pos.position, pos.forward, out hit, maxInteractDistance);
        if (hitBool)
        {
            if (hit.collider.gameObject.GetComponent<IInteractable>() != null)
            {
                if (Input.GetKeyDown(interactKey))
                {
                    //fire interact event
                    interactEvent(new InteractEventArgs(gameObject, hit.collider.gameObject, respondToEvent));
                    Debug.Log("Interact Event sent to object: " + hit.collider.gameObject);
                    canInteract = false;
                    interactResponseNotRecievedTimer = 5;
                }

                //tell player controller/hud stuff to indicate that object can be interacted with
                if (!isInteractPossible) fireInteractPossibleEvent(true);
            }
        }

        if(hitBool && hit.collider.gameObject.GetComponent<IInteractable>() != null || !hitBool)
            if(isInteractPossible)
                fireInteractPossibleEvent(false);
    }

    private void fireInteractPossibleEvent(bool interactionPossibleVal)
    {
        isInteractPossible = interactionPossibleVal;
        interactPossibleEvent(interactionPossibleVal);
    }

    void respondToEvent(InteractResponseEventArgs args)
    {
        Debug.Log("Interact response recieved from:" + args.obj + ". doneWithInteraction? " + args.doneWithInteraction);
        canInteract = args.doneWithInteraction;

        //currentInteractionObject is set to the current object IF the interaction isn't done
        if (!args.doneWithInteraction) currentInteractionObject = args.obj;
        else currentInteractionObject = null;
        isInteractResponseRecieved = true;
        interactResponseNotRecievedTimer = 0;
    }
}







//Interface that all Interactable game objects must implement (buttons, cubes, etc.)
public interface IInteractable
{
    void onInteract(InteractEventArgs args);
}

//args for interact event
public struct InteractEventArgs
{
    public GameObject sender;
    public GameObject target;
    public InteractResponse interactResponseCallback;

    public InteractEventArgs(GameObject sender, GameObject target, InteractResponse interactResponseEvent)
    {
        this.sender = sender;
        this.target = target;
        this.interactResponseCallback = interactResponseEvent;
    }
}

//args for event response
public struct InteractResponseEventArgs
{
    public GameObject obj
    {
        get;
        private set;
    }
    public bool doneWithInteraction
    {
        get;
        private set;
    }

    public InteractResponseEventArgs(GameObject obj, bool doneWithInteraction)
    {
        this.obj = obj;
        this.doneWithInteraction = doneWithInteraction;
    }
}
