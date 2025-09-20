using UnityEngine;

public static class CursorManager
{
    public static void LockAndHide()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public static void UnlockAndShow()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
