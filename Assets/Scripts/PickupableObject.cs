using UnityEditor.UI;
using UnityEngine;
using static Interact;

public class PickupableObject : MonoBehaviour//, IInteractable
{
    public GameObject hand;
    public GameObject interactor;
    public Joint pickupJoint;
    public bool isPickedUp = false;
    public float dropForceThreshold = 500f;
    public double dropCooldown = .5;

    private double currentDropTimer = 0;
    private Interact.InteractResponse interactResponse;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //subscribe to interact events
        //Interact.InteractEvent.AddListener(onInteract);
        Interact.interactEvent += onInteract;
    }

    void Update()
    {
        if (isPickedUp)
        {
            //check if force exceeds breaking threshold
            if (pickupJoint.currentForce.magnitude > dropForceThreshold && currentDropTimer <= 0) dropObject();
            //TODO add logic for drop vs throw on interactkey vs throwKey(probably mouse1)

            //TODO check joint stress and drop the Object if the stress force is past the threshold
        }

        if (currentDropTimer > 0) currentDropTimer = currentDropTimer - Time.deltaTime;
    }

    void OnJointBreak(float breakForce)
    {
        if(isPickedUp) dropObject();
        Debug.Log("A joint has just been broken!, force: " + breakForce);
    }

    void dropObject()
    {
        //disconnect joint and inform interactor that interaction is done (object is dropped)
        pickupJoint.connectedBody = null;
        isPickedUp = false;
        currentDropTimer = 0;
        interactResponse(new InteractResponseEventArgs(gameObject, true));
    }

    void throwObject()
    {
        //calculate vector to throw in

        //disconnect joint

        //apply impulse in direction of throw

        //let interactor know object is dropped
        interactResponse(new InteractResponseEventArgs(gameObject, true));
    }

    public void onInteract(InteractEventArgs args)
    {
        if (isPickedUp)
        {
            dropObject();
        }
        else
        {
            //handle pickup
            transform.position = hand.transform.position;
            transform.rotation = hand.transform.rotation;
            pickupJoint.connectedBody = hand.GetComponent<Rigidbody>();
            isPickedUp= true;
            currentDropTimer = dropCooldown;

            //set response variable for later response and respond to interactor
            interactResponse = args.interactResponseCallback;
            interactResponse(new InteractResponseEventArgs(gameObject, false));
        }
        
    }
}
