//using System.Collections.Generic;
//using UnityEngine;

//public class ChestManager : MonoBehaviour
//{
//    [Header("Chest Inventory")]
//    [SerializeField] private List<Item> chestItems = new(); // Items to be loaded into the chest
//    [SerializeField] private List<InventorySlot> chestSlots = new(); // Chest UI slots
//    [SerializeField] private InventoryItem inventoryItemPrefab; // Prefab for InventoryItem UI

//    /// <summary>
//    /// Loads the items from the chestItems list into the chestSlots.
//    /// This method should be called every time the chest is opened.
//    /// </summary>
//    public void LoadChestItems()
//    {
//        Debug.Log("Loading chest items...");

//        // Clear all chest slots before loading items
//        foreach (var slot in chestSlots)
//        {
//            slot.SetItem(null);
//        }

//        // Populate chest slots with items from the chestItems list
//        for (int i = 0; i < chestItems.Count && i < chestSlots.Count; i++)
//        {
//            if (chestItems[i] != null)
//            {
//                Debug.Log($"Assigning item {chestItems[i].ItemName} to slot {i}");

//                // Instantiate an InventoryItem for the chest slot
//                var inventoryItem = Instantiate(inventoryItemPrefab, chestSlots[i].transform);
//                inventoryItem.Init(chestItems[i].ItemName, chestItems[i].ItemPicture, 1); // Initialize the item
//                chestSlots[i].SetItem(inventoryItem); // Assign the item to the slot
//            }
//            else
//            {
//                Debug.LogWarning($"Chest item at index {i} is null!");
//            }
//        }
//    }

//    /// <summary>
//    /// Adds an item to the chest.
//    /// </summary>
//    /// <param name="item">The item to add.</param>
//    public void AddItemToChest(Item item)
//    {
//        if (chestItems.Count < chestSlots.Count)
//        {
//            chestItems.Add(item);
//            Debug.Log($"Item {item.ItemName} added to chest.");
//        }
//        else
//        {
//            Debug.LogWarning("Chest is full!");
//        }
//    }

//    /// <summary>
//    /// Removes an item from the chest.
//    /// </summary>
//    /// <param name="item">The item to remove.</param>
//    public void RemoveItemFromChest(Item item)
//    {
//        if (chestItems.Contains(item))
//        {
//            chestItems.Remove(item);
//            Debug.Log($"Item {item.ItemName} removed from chest.");
//        }
//        else
//        {
//            Debug.LogWarning("Item not found in chest!");
//        }
//    }
//}
