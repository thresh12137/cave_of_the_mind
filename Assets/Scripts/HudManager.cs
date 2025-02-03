using UnityEngine;
using UnityEngine.UI;

public class HudManager : MonoBehaviour
{
    public Image selectableCrosshair;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        selectableCrosshair.enabled = false;
        Interact.interactPossibleEvent += changeSelectCrosshairState;
    }

    void changeSelectCrosshairState(bool interactPossible)
    {
        selectableCrosshair.enabled = interactPossible;
    }
}
