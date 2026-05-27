using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(RectTransform))]
public class CrosshairFollow : MonoBehaviour
{
     private bool hideSystemCursor = true;
     private bool confineCursorToWindow = true;

    private RectTransform crosshairRectTransform;

    private void Awake()
    {
        crosshairRectTransform = GetComponent<RectTransform>();
        ConfigureCursor();
    }

    private void OnEnable() { ConfigureCursor(); }

    private void OnDisable()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void Update()
    {
        if (Mouse.current == null) return;
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        crosshairRectTransform.position = mousePosition;
    }

    private void ConfigureCursor()
    {
        Cursor.visible = !hideSystemCursor;
        Cursor.lockState = confineCursorToWindow
            ? CursorLockMode.Confined
            : CursorLockMode.None;
    }
}