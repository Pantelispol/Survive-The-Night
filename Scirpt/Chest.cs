//using System;
//using System.Collections.Generic;
//using UnityEngine;

//public class Chest : MonoBehaviour
//{
//    public CanvasGroup chestCanvasGroup;
//    public List<InventorySlot> chestSlots;
//    public List<InventoryManager.InventoryItemData> chestData = new(); // same structure as inventory
//    [SerializeField] private InventoryItem inventoryItemPrefab;

//    private bool playerNear = false;
//    [Serializable]
//    public class ChestItemData
//    {
//        public Item itemPrefab;
//        public int amount;
//    }
//    public List<ChestItemData> defaultContents;

//    void Start()
//    {
//        foreach (var entry in defaultContents)
//        {
//            for (int i = 0; i < entry.amount; i++)
//            {
//                // Create UI inventory item
//                var itemUI = Instantiate(inventoryItemPrefab, chestSlots[i].transform);
//                itemUI.Init(entry.itemPrefab.ItemName, entry.itemPrefab.ItemPicture, 1);

//                chestSlots[i].SetItem(itemUI);

//                // Save to chestData if needed
//                chestData.Add(new InventoryManager.InventoryItemData
//                {
//                    itemName = entry.itemPrefab.ItemName,
//                    amount = 1
//                });
//            }
//        }

//    }

//    void Update()
//    {
//        if (playerNear && Input.GetKeyDown(KeyCode.E))
//        {
//            InventoryManager inv = InstanceHandler.GetInstance<InventoryManager>();
//            inv.ToggleInventory(true);
//            inv.ToggleChest(true);
//            inv.SetChestSlots(chestSlots, chestData);
//        }
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        if (other.CompareTag("Player"))
//        {
//            playerNear = true;
//        }
//    }

//    private void OnTriggerExit(Collider other)
//    {
//        if (other.CompareTag("Player"))
//        {
//            playerNear = false;

//            InventoryManager inv = InstanceHandler.GetInstance<InventoryManager>();
//            inv.ToggleChest(false);
//        }
//    }
//}
