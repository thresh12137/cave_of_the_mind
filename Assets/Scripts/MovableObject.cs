using System.Collections.Generic;
using System.Data;
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
    private float maxInteractableDistance = 10;
    public Vector3 point1;
    public Vector3 point2;
    public float startLerpPosition; //fraction along the path point1 -> point2 to start.  For example, 0 is at point 1, 1 is at point 2, and 0.5 is the midpoint between them
    public bool isPickedUp = false;
    private GameObject interactor;
    public PlayerDetectionTrigger playerDetectionTrigger;
    public ObjectDetectionTrigger objectDetectionTrigger;
    private bool isPlayerStandingOnPlatform = false;
    private bool currentlyInCollision = false;

    private Vector3 movementAxis; //this normalized vector points towards point2 from point1
    private Plane verticalPlane;
    private Plane horizontalPlane;
    float minimumX, maximumX, minimumY, maximumY, minimumZ, maximumZ;
    private Vector3 lastPoint;
    private List<GameObject> objectsOnPlatform;
    private List<FixedJoint> jointsOnPlatform;

    private Interact.InteractResponse interactResponse;

    void Start()
    {
        if (playerDetectionTrigger == null || objectDetectionTrigger == null) throw new System.Exception("No playerDetectionTrigger or objectDetectionTrigger associated with " + gameObject);

        isPickedUp = false;
        transform.position = Vector3.Lerp(point1, point2, startLerpPosition);
        lastPoint = transform.position;

        objectsOnPlatform = new List<GameObject>();
        jointsOnPlatform = new List<FixedJoint>();

        playerDetectionTrigger.enterEvent += playerDetected;
        playerDetectionTrigger.exitEvent += playerNotDetected;
        objectDetectionTrigger.enterEvent += objectOnPlatform;
        objectDetectionTrigger.exitEvent += objectOffPlatform;



        Vector3 axisAlignedCheckValue = point1 - point2; 
        movementAxis = axisAlignedCheckValue.normalized;

        if (movementAxis.x == 0 && movementAxis.z == 0) movementAxis.x += -0.001f;

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
            bool horizontalPlaneBool = horizontalPlane.Raycast(ray, out horizontalDistanceAlongRay);
            bool verticalPlaneBool = verticalPlane.Raycast(ray, out verticalDistanceAlongRay);

            //set new position
            if (verticalPlaneBool && horizontalPlaneBool)
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

            //draw planes for debugging
            DrawPlane(point1, horizontalPlane.normal, Color.green, 10);
            DrawPlane(point1, verticalPlane.normal, Color.blue, 8);
        }
        

    }
    void playerDetected()
    {
        isPlayerStandingOnPlatform = true;
        if (isPickedUp) dropObject();
    }

    void playerNotDetected()
    {
        isPlayerStandingOnPlatform = false;
    }

    void objectOnPlatform(GameObject obj)
    {
        if(obj.GetComponent<Rigidbody>() != null) objectsOnPlatform.Add(obj);

        //if (isPickedUp) // add a joint to grab objects that fall onto the platform while it is picked up
        //{
        //    Rigidbody rb = obj.GetComponent<Rigidbody>();
        //    if (rb == null) return;

        //    FixedJoint joint = gameObject.AddComponent<FixedJoint>();
        //    joint.connectedBody = obj.GetComponent<Rigidbody>();
        //    jointsOnPlatform.Add(joint);
        //}
    }

    void objectOffPlatform(GameObject obj)
    {
        objectsOnPlatform.Remove(obj);
    }

    void OnCollisionEnter(Collision c)
    {
        if (c.relativeVelocity.magnitude < 1) return;
        if (!currentlyInCollision)
        {
            if (isPickedUp && !objectsOnPlatform.Contains(c.gameObject)) dropObject();
        }
        else currentlyInCollision = false;
    }


    void pickupObject()
    {
        isPickedUp = true;

        //create joint with every object to have them move with the platform
        foreach (GameObject obj in objectsOnPlatform)
        {
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb == null) continue;

            FixedJoint joint = gameObject.AddComponent<FixedJoint>();
            joint.connectedBody = obj.GetComponent<Rigidbody>();
            joint.enableCollision= true;
            jointsOnPlatform.Add(joint);
        }
    }

    void dropObject()
    {
        foreach (FixedJoint joint in jointsOnPlatform) Destroy(joint);
        jointsOnPlatform.Clear();
        //foreach (GameObject obj in objectsOnPlatform) obj.transform.position = obj.transform.position + Vector3.up * .1f;

        isPickedUp = false;
        transform.position = lastPoint;

        interactResponse(new InteractResponseEventArgs(gameObject, true));
        interactor = null;
        interactResponse = null;
    }

    public void onInteract(InteractEventArgs args)
    {
        if (isPlayerStandingOnPlatform)
        {
            args.interactResponseCallback(new InteractResponseEventArgs(gameObject, true));
            return;
        }

        if (isPickedUp)
        {
            dropObject();
        }
        else
        {
            //set response variable for later response and respond to interactor
            interactor = args.sender;
            interactResponse = args.interactResponseCallback;

            pickupObject();

            interactResponse(new InteractResponseEventArgs(gameObject, false));
        }
    }

    public bool interactionQuery(float distance)
    {
        if (isPlayerStandingOnPlatform) return false;
        else return distance < maxInteractableDistance;
    }



    public void DrawPlane(Vector3 position, Vector3 normal, Color color, float size)
    {
        Vector3 v3;

        if (normal.normalized != Vector3.forward)
            v3 = Vector3.Cross(normal, Vector3.forward).normalized * size;
        else
            v3 = Vector3.Cross(normal, Vector3.up).normalized * size;

        var corner0 = position + v3;
        var corner2 = position - v3;
        var q = Quaternion.AngleAxis(90.0f, normal);
        v3 = q * v3;
        var corner1 = position + v3;
        var corner3 = position - v3;

        Debug.DrawLine(corner0, corner2, color);
        Debug.DrawLine(corner1, corner3, color);
        Debug.DrawLine(corner0, corner1, color);
        Debug.DrawLine(corner1, corner2, color);
        Debug.DrawLine(corner2, corner3, color);
        Debug.DrawLine(corner3, corner0, color);
        Debug.DrawRay(position, normal, Color.red);
    }
}