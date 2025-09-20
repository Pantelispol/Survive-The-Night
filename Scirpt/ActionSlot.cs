using UnityEngine;
using UnityEngine.UI;

public class ActionSlot : MonoBehaviour
{
    [SerializeField] private Image slotImage;
    [SerializeField] private KeyCode actionKey = KeyCode.Alpha1;
    [SerializeField] private Color activeColor;

    private Color _originalColor;
    public bool IsActive { get; private set; }  // Track if this slot is active (selected)

    private void Awake()
    {
        _originalColor = slotImage.color;
    }

    private void Update()
    {
        if (!Input.GetKeyDown(actionKey))
            return;

        InstanceHandler.GetInstance<InventoryManager>().SetActionSlotActive(this);

    }

    public void ToggleActive(bool toggle)
    {
        IsActive = toggle;  // Update selection state
        slotImage.color = toggle ? activeColor : _originalColor;
    }
}