using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonMotion : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler, ISelectHandler, IDeselectHandler
{
    public Animator motion;
    bool hovering;
    bool selected;

    void State(string state)
    {
        if(motion == null || !GetComponent<Button>().IsInteractable())
        {
            return;
        }
        motion.ResetTrigger("Normal");
        motion.ResetTrigger("Highlighted");
        motion.ResetTrigger("Pressed");
        motion.ResetTrigger("Selected");
        motion.SetTrigger(state);
    }

    public void OnPointerEnter(PointerEventData data)
    {
        hovering = true;
        State("Highlighted");
    }

    public void OnPointerExit(PointerEventData data)
    {
        hovering = false;
        State(selected ? "Selected" : "Normal");
    }

    public void OnPointerDown(PointerEventData data)
    {
        if(data.button == PointerEventData.InputButton.Left)
        {
            State("Pressed");
        }
    }

    public void OnPointerUp(PointerEventData data)
    {
        State(hovering ? "Highlighted" : "Normal");
    }

    public void OnSelect(BaseEventData data)
    {
        selected = true;
        State("Selected");
    }

    public void OnDeselect(BaseEventData data)
    {
        selected = false;
        State(hovering ? "Highlighted" : "Normal");
    }

    void OnEnable()
    {
        hovering = selected = false;
    }
}
