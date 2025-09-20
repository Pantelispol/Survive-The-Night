//using UnityEngine;
//using System.Collections.Generic;

//public class ChestInventoryManager : MonoBehaviour
//{
//    [Header("Chest Inventory")]
//    [SerializeField] private List<Item> chestItems;  // Items specific to this chest
//    [SerializeField] private InventoryItem chestItemPrefab;
//    [SerializeField] private List<InventorySlot> chestSlots;

//    public void InitializeChestInventory()
//    {
//        // Load the chest's unique items into the inventory slots
//        for (int i = 0; i < chestItems.Count; i++)
//        {
//            if (i >= chestSlots.Count) break;  // Avoid overflowing the slots

//            var item = chestItems[i];
//            var inventoryItem = Instantiate(chestItemPrefab, chestSlots[i].transform);
//            inventoryItem.Init(item.ItemName, item.ItemPicture, 1);  // Assuming each chest item has 1 amount
//            chestSlots[i].SetItem(inventoryItem);
//        }
//    }

//    public void ClearChestInventory()
//    {
//        // Clear the chest inventory UI and items
//        foreach (var slot in chestSlots)
//        {
//            slot.SetItem(null);  // Clear each slot
//        }
//    }

//    public void AddItemToChest(Item item)
//    {
//        // Find the first available slot and add the item to it
//        foreach (var slot in chestSlots)
//        {
//            if (slot.IsEmpty)
//            {
//                var inventoryItem = Instantiate(chestItemPrefab, slot.transform);
//                inventoryItem.Init(item.ItemName, item.ItemPicture, 1);
//                slot.SetItem(inventoryItem);
//                return;
//            }
//        }
//    }

//    public void RemoveItemFromChest(InventoryItem item)
//    {
//        foreach (var slot in chestSlots)
//        {
//            if (slot.Item == item)
//            {
//                slot.SetItem(null);
//                Destroy(item.gameObject);  // Destroy the UI item object
//                return;
//            }
//        }
//    }
//}
