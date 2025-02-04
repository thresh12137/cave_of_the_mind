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
    public GameObject stoodOnCheckObject;
    private bool isPlayerStandingOnPlatform = false;

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
        lastPoint = transform.position;

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
            //TODO check for player standing on it

            //set up variables
            Ray ray = new Ray(interactor.transform.position, interactor.transform.forward);
            float distanceAlongRay;

            //raycast with the vertical plane to get the point above the path
            if(verticalPlane.Raycast(ray, out distanceAlongRay))
            {
                //get closest point to the horizontal plane, which is the distance along the path that it should be.  This needs to be clamped severely
                Vector3 newPos = horizontalPlane.ClosestPointOnPlane(ray.GetPoint(distanceAlongRay));
                newPos.x = Mathf.Clamp(newPos.x, minimumX, maximumX);
                newPos.y = Mathf.Clamp(newPos.y, minimumY, maximumY);
                newPos.z = Mathf.Clamp(newPos.z, minimumZ, maximumZ);
                transform.position = newPos;
                lastPoint = newPos;
            }
            else
            {
                transform.position = lastPoint;
            }
        }
    }

    void pickupObject()
    {
        isPickedUp = true;
        //gameObject.layer = 7; //have this if using grounded state to ensure the platform isn't moved when stood on
    }

    void dropObject()
    {
        isPickedUp = false;

        interactResponse(new InteractResponseEventArgs(gameObject, true));
        interactor = null;
        interactResponse = null;
    }

    public void onInteract(InteractEventArgs args)
    {
        if (args.target == gameObject)
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
    }
}
