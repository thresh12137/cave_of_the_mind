using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Dead_Zone : MonoBehaviour
{
    [SerializeField]
    private GameObject _respawnPoint;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Out Of Bounds");
        if (other.tag == "Interactable")
        {
            Rigidbody rg = other.GetComponent<Rigidbody>();
            if (rg != null)
            {
                other.transform.position = _respawnPoint.transform.position;
                rg.linearVelocity = Vector3.zero;
                rg.angularVelocity = Vector3.zero;
            }

        }
    }
}