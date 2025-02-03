using UnityEngine;

public class ShiftableObject : MonoBehaviour
{
    public static KeyCode obj1Key = KeyCode.Alpha1;
    public static KeyCode obj2Key = KeyCode.Alpha2;
    public static KeyCode obj3Key = KeyCode.Alpha3;
    public static KeyCode obj4Key = KeyCode.Alpha4;

    public Mesh cubeMesh;
    public Collider cubeCollider;
    public Mesh sphereMesh;
    public Collider sphereCollider;

    PickupableObject pickupableObjectComponent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pickupableObjectComponent = GetComponent<PickupableObject>();
        if(pickupableObjectComponent == null) Debug.Log("Object is not a pickupableObject:" + gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (pickupableObjectComponent.isPickedUp)
        {
            if (Input.GetKeyDown(obj1Key)) changeObjectTo(ObjectType.Cube);
            if (Input.GetKeyDown(obj2Key)) changeObjectTo(ObjectType.Sphere);
            if (Input.GetKeyDown(obj3Key)) changeObjectTo(ObjectType.Tetrahedron);
        }
    }

    void changeObjectTo(ObjectType type)
    {
        //TODO change mesh and collider
    }
}

//enum for object Types
public enum ObjectType
{
    Cube = 0,
    Sphere = 1,
    Tetrahedron = 2
}
