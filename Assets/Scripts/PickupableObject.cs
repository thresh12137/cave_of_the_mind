//using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static Interact;

public class PickupableObject : MonoBehaviour, IInteractable
{
    private GameObject hand;
    public bool isPickedUp = false;
    public static float dropForceThreshold = 750f;
    public static float throwForce = 7;

    private Rigidbody rigidbodyComponent;
    private int startingLayer;
    private Joint pickupJoint;
    private double forcedDropCooldown = .5; //should be small number in seconds to prevent abnormally high joint force when picking an item up from triggering drop
    private double currentForcedDropTimer = 0;
    private Interact.InteractResponse interactResponse;
    private float maxInteractionDistance = 3;
    private Outline outlineEffect;
    private bool canBeInteractedWith = false;
    private Transform startingParent;

    public static UnityEvent objectPickedUpEvent = new UnityEvent();
    public static UnityEvent objectDroppedEvent = new UnityEvent();
    public static UnityEvent objectThrownEvent = new UnityEvent();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startingLayer = gameObject.layer;
        startingParent = gameObject.transform.parent;
        rigidbodyComponent = GetComponent<Rigidbody>();

        outlineEffect = GetComponent<Outline>();
        if(outlineEffect != null)
        {
            outlineEffect.OutlineColor = Interact.outlineColor;
            outlineEffect.OutlineWidth = Interact.outlineWidth;
            outlineEffect.OutlineMode = Interact.outlineMode;
            outlineEffect.enabled = false;
        }
    }

    void Update()
    {
        if(outlineEffect != null)
        {
            if(canBeInteractedWith) outlineEffect.enabled = true;
            else outlineEffect.enabled = false;
            canBeInteractedWith = false;
        }
        
        //Debug.Log("current joint force: " + pickupJoint.currentForce.magnitude);
        if (isPickedUp && pickupJoint != null)
        {
            //check joint stress and drop the Object if the stress force is past the threshold
            if (pickupJoint.currentForce.magnitude > dropForceThreshold && currentForcedDropTimer <= 0) dropObject();

            //throw on throwKey (probably mouse1)
            if (Input.GetMouseButtonDown(0))
            {
                throwObject();
            }
        }

        if (currentForcedDropTimer > 0) currentForcedDropTimer = currentForcedDropTimer - Time.deltaTime;
    }

    void pickupObject()
    {
        Vector3 oldPos = transform.position;
        Quaternion oldRotation = transform.rotation;

        transform.position = hand.transform.position;
        //transform.rotation = hand.transform.rotation;

        transform.parent= hand.transform;

        isPickedUp = true;
        currentForcedDropTimer = forcedDropCooldown;

        pickupJoint = gameObject.AddComponent<FixedJoint>();
        pickupJoint.connectedBody = hand.GetComponent<Rigidbody>();
        gameObject.layer = 7;

        transform.position = oldPos;
        //transform.rotation = oldRotation;

        objectPickedUpEvent.Invoke();
    }

    void dropObject()
    {
        //disconnect joint and inform interactor that interaction is done (object is dropped)
        rigidbodyComponent.constraints = RigidbodyConstraints.None;
        Destroy(pickupJoint);
        pickupJoint = null;
        isPickedUp = false;
        currentForcedDropTimer = 0;
        gameObject.layer = startingLayer;
        transform.parent = startingParent;
        GetComponent<Rigidbody>().linearVelocity = Vector3.zero; // here to stop object from going flying if it got forcefully dropped from pushing into wall
        interactResponse(new InteractResponseEventArgs(gameObject, true));

        objectDroppedEvent.Invoke();
    }

    void throwObject()
    {
        rigidbodyComponent.constraints = RigidbodyConstraints.None;
        //calculate vector to throw in
        Vector3 throwDir = hand.transform.forward * Mathf.Clamp(throwForce / rigidbodyComponent.mass, throwForce * 0.5f, throwForce * 2f);
        //disconnect joint
        Destroy(pickupJoint);
        pickupJoint = null;
        isPickedUp = false;
        currentForcedDropTimer = 0;
        gameObject.layer = startingLayer;
        transform.parent = startingParent;
        //apply impulse in direction of throw
        GetComponent<Rigidbody>().AddForce(throwDir, ForceMode.VelocityChange);
        //let interactor know object is dropped
        interactResponse(new InteractResponseEventArgs(gameObject, true));

        objectThrownEvent.Invoke();
    }

    public void onInteract(InteractEventArgs args)
    {
        if (isPickedUp)
        {
            dropObject();
        }
        else
        {
            hand = args.hand;
            pickupObject();

            //set response variable for later response and respond to interactor
            interactResponse = args.interactResponseCallback;
            interactResponse(new InteractResponseEventArgs(gameObject, false));
        }
    }

    public bool interactionQuery(float distance)
    {
        canBeInteractedWith = distance < maxInteractionDistance;
        return canBeInteractedWith;
    }
}
