//using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SizeChangableObject : MonoBehaviour
{
    public static KeyCode increaseSizeKey = KeyCode.T;
    public static KeyCode decreaseSizeKey = KeyCode.G;

    public float increaseSizeMultiplier = 1.2f;
    public float decreaseSizeMultiplier = 0.8f;
    public float maxScale = 2.0f;
    public float minScale = 0.5f;

    private Vector3 maxLocalScale;
    private Vector3 minLocalScale;
    private float maxMass;
    private float minMass;

    PickupableObject pickupableObjectComponent;
    Rigidbody rb;

    void Start()
    {   
        pickupableObjectComponent= GetComponent<PickupableObject>();
        rb = GetComponent<Rigidbody>();
        if (pickupableObjectComponent == null) throw new System.Exception("Object is not a pickupableObject:" + gameObject);
        if (rb == null) throw new System.Exception("Object does not have a RigidBody:" + gameObject);

        maxLocalScale = transform.localScale * maxScale;
        minLocalScale = transform.localScale * minScale;

        maxMass = rb.mass * maxScale * 4;
        minMass = rb.mass * (minScale/4);
    }

    // Update is called once per frame
    void Update()
    {
        if (pickupableObjectComponent != null && pickupableObjectComponent.isPickedUp)
        {
            if (Input.GetKeyDown(increaseSizeKey))
            {
                float newX = Mathf.Clamp(transform.localScale.x * increaseSizeMultiplier, minLocalScale.x, maxLocalScale.x);
                float newY = Mathf.Clamp(transform.localScale.y * increaseSizeMultiplier, minLocalScale.y, maxLocalScale.y);
                float newZ = Mathf.Clamp(transform.localScale.z * increaseSizeMultiplier, minLocalScale.z, maxLocalScale.z);
                transform.localScale = new Vector3(newX, newY, newZ);

                rb.mass = Mathf.Clamp(rb.mass * increaseSizeMultiplier * 2, minMass, maxMass);
            }
            if (Input.GetKeyDown(decreaseSizeKey))
            {
                float newX = Mathf.Clamp(transform.localScale.x * decreaseSizeMultiplier, minLocalScale.x, maxLocalScale.x);
                float newY = Mathf.Clamp(transform.localScale.y * decreaseSizeMultiplier, minLocalScale.y, maxLocalScale.y);
                float newZ = Mathf.Clamp(transform.localScale.z * decreaseSizeMultiplier, minLocalScale.z, maxLocalScale.z);
                transform.localScale = new Vector3(newX, newY, newZ);

                rb.mass = Mathf.Clamp(rb.mass * (decreaseSizeMultiplier/2), minMass, maxMass);
            }
        }
    }
}
