using UnityEngine;
using UnityEngine.EventSystems;

public class MenuSound : MonoBehaviour, IPointerEnterHandler, ISelectHandler
{
    public MenuButton menu;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(menu != null)
        {
            menu.Hover();
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (UnityEngine.Input.GetMouseButton(0))
        {
            return;
        }

        if (menu != null)
        {
            menu.Hover();
        }
    }
}
