using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dead_Zone : MonoBehaviour
{
    [SerializeField]
    private GameObject _respawnPoint;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("fallen");
        if (other.tag == "Player")
        {
            CharacterController cc = other.GetComponent<CharacterController>();
            if (cc != null)
            {
                //momentarily takes movement away from player
                cc.enabled = false;
                other.transform.position = _respawnPoint.transform.position;
                cc.enabled = true;
            }
        }
    }
}