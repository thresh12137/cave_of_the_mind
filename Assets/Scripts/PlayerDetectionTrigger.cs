using UnityEngine;

public class PlayerDetectionTrigger : MonoBehaviour
{
    public delegate void PlayerEnteredTrigger();
    public event PlayerEnteredTrigger enterEvent;
    public delegate void PlayerInTrigger();
    public event PlayerInTrigger inTriggerEvent;
    public delegate void PlayerExitedTrigger();
    public event PlayerExitedTrigger exitEvent;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            if (enterEvent != null) enterEvent();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
            if (inTriggerEvent != null) inTriggerEvent();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
            if (exitEvent != null) exitEvent();
    }
    
}
