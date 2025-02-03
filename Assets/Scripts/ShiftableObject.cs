using UnityEditor.SceneManagement;
using UnityEngine;

public class ShiftableObject : MonoBehaviour
{
    public static KeyCode obj1Key = KeyCode.Alpha1;
    public static KeyCode obj2Key = KeyCode.Alpha2;
    public static KeyCode obj3Key = KeyCode.Alpha3;
    public static KeyCode obj4Key = KeyCode.Alpha4;

    public Mesh cubeMesh;
    public Mesh sphereMesh;
    public Mesh tetrahedronMesh;

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
        if (pickupableObjectComponent != null && pickupableObjectComponent.isPickedUp)
        {
            if (Input.GetKeyDown(obj1Key)) changeObjectTo(ObjectType.Cube);
            if (Input.GetKeyDown(obj2Key)) changeObjectTo(ObjectType.Sphere);
            if (Input.GetKeyDown(obj3Key)) changeObjectTo(ObjectType.Tetrahedron);
        }
    }

    void changeObjectTo(ObjectType type)
    {
        switch (type)
        {
            case ObjectType.Cube:
                GetComponent<MeshFilter>().mesh = cubeMesh;
                Destroy(GetComponent<Collider>());
                gameObject.AddComponent<BoxCollider>();
                break;
            case ObjectType.Sphere:
                GetComponent<MeshFilter>().mesh = sphereMesh;
                Destroy(GetComponent<Collider>());
                gameObject.AddComponent<SphereCollider>();
                break;
            case ObjectType.Tetrahedron:
                GetComponent<MeshFilter>().mesh = tetrahedronMesh;
                Destroy(GetComponent<Collider>());
                MeshCollider col = gameObject.AddComponent<MeshCollider>();
                col.sharedMesh = tetrahedronMesh;
                col.convex = true;
                col.enabled = true;
                break;
        }
    }
}

//enum for object Types
public enum ObjectType
{
    Cube = 0,
    Sphere = 1,
    Tetrahedron = 2
}
