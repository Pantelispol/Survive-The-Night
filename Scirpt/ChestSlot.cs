//using UnityEngine;
//using UnityEngine.EventSystems;

//public class ChestSlot : InventorySlot
//{
//    public override void OnDrop(PointerEventData eventData)
//    {
//        if (eventData.pointerDrag == null) return;

//        // Set the dragged item to this slot
//        eventData.pointerDrag.transform.SetParent(transform);
//        var inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();
//        inventoryItem.SetAvailable();

//        // Handle item transfer between chest and player inventory
//        if (InstanceHandler.TryGetInstance(out InventoryManager inventoryManager))
//        {
//            inventoryManager.ItemMoved(inventoryItem, this);
//        }
//    }
//}
