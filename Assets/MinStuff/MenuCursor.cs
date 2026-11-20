using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuCursor : MonoBehaviour
{
    public RectTransform cursorPicture;
    public Canvas canvas;
    public Sprite normalCursor;
    public Sprite handCursor;
    public float cursorHeight = 34f;

    Image picture;
    PointerEventData pointer;
    EventSystem pointerSystem;
    readonly List<RaycastResult> hits = new List<RaycastResult>();
    bool previousCursorVisible;
    bool focused = true;

    void OnEnable()
    {
        previousCursorVisible = Cursor.visible;
    }

    void Start()
    {
        if(cursorPicture == null || canvas == null)
        {
            Debug.LogWarning("Assign Cursor Picture and Canvas on MenuCursor.");
            enabled = false;
            return;
        }
        picture = cursorPicture.GetComponent<Image>();
        picture.raycastTarget = false;
        cursorPicture.SetParent(canvas.transform,false);
        cursorPicture.SetAsLastSibling();
    }

    void LateUpdate()
    {
        if(!focused || Cursor.lockState != CursorLockMode.None)
        {
            cursorPicture.gameObject.SetActive(false);
            return;
        }

        bool clickable = false;
        if(EventSystem.current != null)
        {
            if(pointer == null || pointerSystem != EventSystem.current)
            {
                pointerSystem = EventSystem.current;
                pointer = new PointerEventData(pointerSystem);
            }
            pointer.position = Input.mousePosition;
            hits.Clear();
            pointerSystem.RaycastAll(pointer,hits);
            // The frontmost UI graphic decides what is under the mouse.
            if(hits.Count > 0)
            {
                Button button = hits[0].gameObject.GetComponentInParent<Button>();
                clickable = button != null && button.IsActive() && button.IsInteractable();
            }
        }

        Sprite sprite = normalCursor;
        if(clickable && handCursor != null)
        {
            sprite = handCursor;
        }
        if(sprite != null)
        {
            picture.sprite = sprite;
            cursorPicture.sizeDelta = new Vector2(cursorHeight * sprite.rect.width / sprite.rect.height,cursorHeight);
        }

        // Place the pointer tip, or the hand's fingertip, on the actual mouse position.
        cursorPicture.pivot = clickable ? new Vector2(.36f,.94f) : new Vector2(.08f,.95f);
        Vector2 position;
        Camera camera = null;
        if(canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            camera = canvas.worldCamera;
        }
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),Input.mousePosition,camera,out position);
        cursorPicture.localPosition = new Vector3(position.x,position.y,0);
        cursorPicture.gameObject.SetActive(true);
        Cursor.visible = false;
    }

    void OnDisable()
    {
        Cursor.visible = previousCursorVisible;
        if(cursorPicture != null)
        {
            cursorPicture.gameObject.SetActive(false);
        }
    }

    void OnApplicationFocus(bool hasFocus)
    {
        focused = hasFocus;
        if(!hasFocus)
        {
            Cursor.visible = true;
        }
    }
}
