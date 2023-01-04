using UnityEngine;
using UnityEngine.EventSystems;
using static PInvoke;

public class WindowResizeHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    bool isDragging = false;
    bool isInsideOfHandler = false;
    public Vector2 hotspot = Vector2.zero;

    // Minimum and maximum values for window width/height in pixel.
    [SerializeField]
    private int minWidthPixel = 768;
    [SerializeField]
    private int maxWidthPixel = 2048;

    public Texture2D wnes;
    private float aspect = 16 / 9f;
    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData) => isDragging = eventData.pointerId==-1;
    void IDragHandler.OnDrag(PointerEventData eventData) => WindowProcess(eventData);
    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        if (!isInsideOfHandler)
        {
            Cursor.SetCursor(default, default, CursorMode.Auto);
        }
    }
    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        isInsideOfHandler = true;
        Cursor.SetCursor(wnes, hotspot, CursorMode.Auto);
    }
    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        isInsideOfHandler = false;
        if (!isDragging)
        {
            Cursor.SetCursor(default, default, CursorMode.Auto);
        }
    }
    private void WindowProcess(PointerEventData eventData)
    {
        if (Application.isEditor || eventData.pointerId != -1) return;
        RECT rc = default;
        GetWindowRect(UnityHWnd, ref rc);
        int newWidth = Mathf.Clamp(rc.Right - rc.Left + Mathf.RoundToInt(eventData.delta.x), minWidthPixel, maxWidthPixel);
        int newHeight = Mathf.RoundToInt(newWidth / aspect);
        SetWindowPos(UnityHWnd, 0, rc.Left, rc.Top, newWidth, newHeight, SWP_SHOWWINDOW);
    }
}
