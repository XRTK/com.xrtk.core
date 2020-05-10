using UnityEngine;
using UnityEngine.UI;
using XRTK.EventDatum.Input;
using XRTK.Interfaces.InputSystem.Handlers;

public class DemoButton : MonoBehaviour, IMixedRealityPointerHandler
{
    [SerializeField]
    private Button.ButtonClickedEvent onClick = null;

    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
        onClick.Invoke();
    }

    public void OnPointerDown(MixedRealityPointerEventData eventData)
    {

    }

    public void OnPointerUp(MixedRealityPointerEventData eventData)
    {

    }
}
