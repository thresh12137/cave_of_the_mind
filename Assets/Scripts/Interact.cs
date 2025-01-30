using UnityEngine;
using UnityEngine.InputSystem;

public class Interact : MonoBehaviour
{
    public Key interactKey;
    public Transform pos;
    public float maxInteractDistance;
    private LayerMask mask;

    private void Start()
    {
        mask = LayerMask.GetMask("Interactables");
    }
    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(pos.position, pos.forward, out hit, maxInteractDistance))
        {
            if (hit.collider.gameObject.tag == "Interactable")
            {
                Debug.Log("Interactable object Found: " + hit.collider.gameObject.ToString());
            }
        }
    }
}
