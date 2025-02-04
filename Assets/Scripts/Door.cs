using NUnit.Framework.Interfaces;
using UnityEngine;
using UnityEngine.Rendering;

public class Door : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public float interactionDistance;
    //Uncomment if we want to display text to interact with the door
    //public GameObject interactionText;

    public string doorOpenAnimName, doorCloseAnimName;

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance)) {

            if (hit.collider.gameObject.tag == "Door")
            {

                //Gets the parent object of the door and door hinge
                GameObject doorParent = hit.collider.transform.root.gameObject;

                Animator doorAnim = doorParent.GetComponent<Animator>();
                //If we want to have interaction text displayed
                //interactionText.SetActive(true);

                if (Input.GetKeyDown(KeyCode.J))
                {

                    if (doorAnim.GetCurrentAnimatorStateInfo(0).IsName(doorOpenAnimName))
                    {

                        doorAnim.ResetTrigger("Open");
                        doorAnim.SetTrigger("Close");

                    }

                    if (doorAnim.GetCurrentAnimatorStateInfo(0).IsName(doorOpenAnimName))
                    {

                        doorAnim.ResetTrigger("Close");
                        doorAnim.SetTrigger("Open");

                    }
                }
            }
            else { 
                
                //interactionText.SetActive(false);

            }
        }
    }

}
