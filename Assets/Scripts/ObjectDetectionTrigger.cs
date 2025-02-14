using UnityEngine;

public class ObjectDetectionTrigger : MonoBehaviour
{
    public delegate void ObjectEnteredTrigger(GameObject obj);
    public event ObjectEnteredTrigger enterEvent;
    public delegate void ObjectInTrigger(GameObject obj);
    public event ObjectInTrigger inTriggerEvent;
    public delegate void ObjectExitedTrigger(GameObject obj);
    public event ObjectExitedTrigger exitEvent;

    private void OnTriggerEnter(Collider other)
    {
        print("object entered trigger: " + other.gameObject);
        if (enterEvent != null) enterEvent(other.gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        if (inTriggerEvent != null) inTriggerEvent(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (exitEvent != null) exitEvent(other.gameObject);
    }
    
}
