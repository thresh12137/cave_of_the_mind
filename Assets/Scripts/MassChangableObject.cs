//using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MassChangableObject : MonoBehaviour
{
    public static KeyCode increaseDensityKey = KeyCode.T;
    public static KeyCode decreaseDensityKey = KeyCode.G;

    public float amountChangePerStep = 1.0f;
    public float currentMass;
    public float maxMass = 50f;
    public float minMass = 0.1f;

    PickupableObject pickupableObjectComponent;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pickupableObjectComponent = GetComponent<PickupableObject>();
        
        if (pickupableObjectComponent == null) throw new System.Exception("Object is not a pickupableObject:" + gameObject);
        if (rb == null) throw new System.Exception("Object does not have a rigidbody:" + gameObject);

        currentMass = rb.mass;
    }

    // Update is called once per frame
    void Update()
    {
        if (pickupableObjectComponent != null && pickupableObjectComponent.isPickedUp)
        {
            if (Input.GetKeyDown(increaseDensityKey)) rb.mass = Mathf.Clamp(rb.mass + amountChangePerStep, minMass, maxMass);
            if (Input.GetKeyDown(decreaseDensityKey)) rb.mass = Mathf.Clamp(rb.mass - amountChangePerStep, minMass, maxMass);
        }
    }
}
