using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;
using static Interact;
using static UnityEngine.Rendering.GPUSort;

/// <summary>
/// Right now, This only accounts for platforms that move horizontally only
/// </summary>
public class MovableObject : MonoBehaviour, IInteractable
{
    public Vector3 point1;
    public Vector3 point2;
    public float startLerpPosition; //fraction along the path point1 -> point2 to start.  For example, 0 is at point 1, 1 is at point 2, and 0.5 is the midpoint between them
    public bool isPickedUp = false;
    private GameObject interactor;
    public PlayerDetectionTrigger playerDetectionTrigger;
    private bool isPlayerStandingOnPlatform = false;
    private bool currentlyInCollision = false;
    private Vector3 centerOfPath;

    private Vector3 movementAxis; //this normalized vector points towards point2 from point1
    private float distance; //distance between point 1 and point 2
    private Plane verticalPlane;
    private Plane horizontalPlane;
    float minimumX, maximumX, minimumY, maximumY, minimumZ, maximumZ;
    private Vector3 lastPoint;

    private Interact.InteractResponse interactResponse;

    void Start()
    {
        isPickedUp = false;
        transform.position = Vector3.Lerp(point1, point2, startLerpPosition);
        centerOfPath = Vector3.Lerp(point1, point2, 0.5f);
        lastPoint = transform.position;

        playerDetectionTrigger.enterEvent += playerDetected;
        playerDetectionTrigger.exitEvent += playerNotDetected;

        //GetComponent<Rigidbody>().isKinematic = true;

        Vector3 axisAlignedCheckValue = point1 - point2; 
        movementAxis = axisAlignedCheckValue.normalized;
        distance = axisAlignedCheckValue.magnitude;

        Vector3 verticalPlaneNormal = Vector3.Cross(Vector3.up, movementAxis);
        verticalPlane = new Plane(verticalPlaneNormal, point1);

        Vector3 horizontalPlaneNormal;
        if(movementAxis.z != 0) horizontalPlaneNormal = Vector3.Cross(Vector3.right, movementAxis);
        else horizontalPlaneNormal = Vector3.Cross(Vector3.forward, movementAxis);
        horizontalPlane = new Plane(horizontalPlaneNormal, point1);

        //calculate min and max values
        minimumX = Mathf.Min(point1.x, point2.x);
        maximumX = Mathf.Max(point1.x, point2.x);
        minimumY = Mathf.Min(point1.y, point2.y);
        maximumY = Mathf.Max(point1.y, point2.y);
        minimumZ = Mathf.Min(point1.z, point2.z);
        maximumZ = Mathf.Max(point1.z, point2.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (isPickedUp)
        {
            //set up variables and cast rays
            Ray ray = new Ray(interactor.transform.position, interactor.transform.forward);
            float horizontalDistanceAlongRay, verticalDistanceAlongRay;
            Vector3 newPos;
            Vector3 newPosHorizontal = Vector3.zero;
            Vector3 newPosVertical = Vector3.zero;
            bool horizontalPlaneBool = horizontalPlane.Raycast(ray, out horizontalDistanceAlongRay);
            bool verticalPlaneBool = verticalPlane.Raycast(ray, out verticalDistanceAlongRay);

            //set new position
            if(verticalPlaneBool && horizontalPlaneBool)
            {
                bool verticalPointCloserThanHorizontalPoint = Vector3.Distance(ray.GetPoint(verticalDistanceAlongRay), transform.position) < Vector3.Distance(ray.GetPoint(horizontalDistanceAlongRay), transform.position);
                if (verticalPointCloserThanHorizontalPoint)
                {
                    //use vertical distance
                    newPos = horizontalPlane.ClosestPointOnPlane(ray.GetPoint(verticalDistanceAlongRay));
                }
                else
                {
                    //use horizontal distance
                    newPos = verticalPlane.ClosestPointOnPlane(ray.GetPoint(horizontalDistanceAlongRay));
                }
            }
            else if(verticalPlaneBool && !horizontalPlaneBool)
            {
                newPos = horizontalPlane.ClosestPointOnPlane(ray.GetPoint(verticalDistanceAlongRay));
            }
            else if(horizontalPlaneBool && !verticalPlaneBool)
            {
                newPos = verticalPlane.ClosestPointOnPlane(ray.GetPoint(horizontalDistanceAlongRay));
            }
            else
            {
                newPos = lastPoint;
            }

            //clamp new position
            newPos.x = Mathf.Clamp(newPos.x, minimumX, maximumX);
            newPos.y = Mathf.Clamp(newPos.y, minimumY, maximumY);
            newPos.z = Mathf.Clamp(newPos.z, minimumZ, maximumZ);

            //apply new position
            transform.position = newPos;
            lastPoint = newPos;
            //raycast with the vertical plane to get the point above the path
        }
    }
    void playerDetected()
    {
        Debug.Log("Player Detected");
        isPlayerStandingOnPlatform = true;
    }

    void playerNotDetected()
    {
        Debug.Log("Player No Longer Detected");
        isPlayerStandingOnPlatform = false;
    }

    void OnCollisionEnter(Collision c)
    {
        if (!currentlyInCollision)
        {
            if (isPickedUp) dropObject();
        }
        else currentlyInCollision = false;
    }


    void pickupObject()
    {
        isPickedUp = true;
        //GetComponent<Rigidbody>().isKinematic = false;
        //gameObject.layer = 7; //have this if using grounded state to ensure the platform isn't moved when stood on
    }

    void dropObject()
    {
        isPickedUp = false;
        //GetComponent<Rigidbody>().isKinematic = true;
        transform.position = lastPoint;

        interactResponse(new InteractResponseEventArgs(gameObject, true));
        interactor = null;
        interactResponse = null;
    }

    public void onInteract(InteractEventArgs args)
    {
        if (!isPlayerStandingOnPlatform)
        {
            if (isPickedUp)
            {
                dropObject();
            }
            else
            {
                pickupObject();

                //set response variable for later response and respond to interactor
                interactor = args.sender;
                interactResponse = args.interactResponseCallback;
                interactResponse(new InteractResponseEventArgs(gameObject, false));
            }
        }
        else
        {
            args.interactResponseCallback(new InteractResponseEventArgs(gameObject, true));
        }
    }
}
