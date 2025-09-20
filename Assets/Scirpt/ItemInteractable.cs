using Script.Controller;
using UnityEngine;

public class ItemInteractable : AInteractable
{
    [SerializeField] private Item item;
    [SerializeField] private float interactionDistance = 2f;

    public override void Interact()
    {
        if (IsPlayerInRange() && item != null)
        {
            InventoryManager inventory = InstanceHandler.GetInstance<InventoryManager>();
            if (inventory != null)
            {
                inventory.AddItem(item);
                Destroy(gameObject);
            }
        }
    }

    private bool IsPlayerInRange()
    {
        if (PlayerController.localPlayerController == null) return false;

        float distance = Vector3.Distance(
            transform.position,
            PlayerController.localPlayerController.transform.position
        );

        return distance <= interactionDistance;
    }
}