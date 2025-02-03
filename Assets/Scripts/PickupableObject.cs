using UnityEditor.UI;
using UnityEngine;
using static Interact;

public class PickupableObject : MonoBehaviour, IInteractable
{
    public GameObject hand;
    public bool isPickedUp = false;
    public static float dropForceThreshold = 100f;
    public static float throwForce = 7;

    private int startingLayer;
    private Joint pickupJoint;
    private double forcedDropCooldown = .5; //should be small number to prevent abnormally high joint force when picking an item up from triggering drop
    private double currentForcedDropTimer = 0;
    private Interact.InteractResponse interactResponse;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //subscribe to interact events
        //Interact.InteractEvent.AddListener(onInteract);
        startingLayer = gameObject.layer;
        Interact.interactEvent += onInteract;
        Debug.Log("object " + gameObject.GetInstanceID() + "is pickupable.");
    }

    void Update()
    {
        //Debug.Log("current joint force: " + pickupJoint.currentForce.magnitude);
        if (isPickedUp && pickupJoint != null)
        {
            //check joint stress and drop the Object if the stress force is past the threshold
            if (pickupJoint.currentForce.magnitude > dropForceThreshold && currentForcedDropTimer <= 0) dropObject();

            //throw on throwKey (probably mouse1)
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("Mouse Input detected");
                throwObject();
            }
        }

        if (currentForcedDropTimer > 0) currentForcedDropTimer = currentForcedDropTimer - Time.deltaTime;
    }

    void pickupObject()
    {
        transform.position = hand.transform.position;
        transform.rotation = hand.transform.rotation;

        isPickedUp = true;
        currentForcedDropTimer = forcedDropCooldown;

        pickupJoint = gameObject.AddComponent<FixedJoint>();
        pickupJoint.connectedBody = hand.GetComponent<Rigidbody>();
        gameObject.layer = 7;
    }

    void dropObject()
    {
        //disconnect joint and inform interactor that interaction is done (object is dropped)
        Destroy(pickupJoint);
        pickupJoint = null;
        isPickedUp = false;
        currentForcedDropTimer = 0;
        gameObject.layer = startingLayer;
        GetComponent<Rigidbody>().linearVelocity = Vector3.zero; // here to stop object from going flying if it got forcefully dropped from pushing into wall
        interactResponse(new InteractResponseEventArgs(gameObject, true));
    }

    void throwObject()
    {
        //calculate vector to throw in
        Vector3 throwDir = hand.transform.forward * throwForce;
        //disconnect joint
        Destroy(pickupJoint);
        pickupJoint = null;
        isPickedUp = false;
        currentForcedDropTimer = 0;
        gameObject.layer = startingLayer;
        //apply impulse in direction of throw
        GetComponent<Rigidbody>().AddForce(throwDir, ForceMode.VelocityChange);
        //let interactor know object is dropped
        interactResponse(new InteractResponseEventArgs(gameObject, true));
    }

    public void onInteract(InteractEventArgs args)
    {
        if(args.target == gameObject)
        {
            if (isPickedUp)
            {
                dropObject();
            }
            else
            {
                pickupObject();

                //set response variable for later response and respond to interactor
                interactResponse = args.interactResponseCallback;
                interactResponse(new InteractResponseEventArgs(gameObject, false));
            }
        }
    }
}
