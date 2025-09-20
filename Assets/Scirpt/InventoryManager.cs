using NUnit.Framework;
using Script.Controller;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private List<Item> allItems = new();
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private CanvasGroup secondPanelCanvasGroup;
    [SerializeField] private InventoryItem itemPrefab;
    [SerializeField] private List<InventorySlot> slots = new();
    [SerializeField] private List<ActionSlot> actionSlots = new();
    [SerializeField] private InventoryItemData[] _inventoryData;
    [SerializeField] private GameOverScreen gameOverScreen;


    private ActionSlot _activeActionSlot;

    public void Awake()
    {
        InstanceHandler.RegisterInstance(this);
        _inventoryData = new InventoryItemData[slots.Count];
        ToggleInventory(false);
        ToggleChest(false); 
    }

    public void OnDestroy()
    {
        InstanceHandler.UnregisterInstance<InventoryManager>();
    }

    private void Update()
    {
        if (gameOverScreen.gameObject.activeSelf)
        {
            return; 
        }
        if (!PauseMenu.Paused && Input.GetKeyDown(KeyCode.Tab))
        {
            bool isOpen = canvasGroup.alpha > 0;
            ToggleInventory(!isOpen);
        }


    }

    public bool IsChestOpen()
    {
        return secondPanelCanvasGroup.alpha > 0;
    }

    public void ToggleInventory(bool toggle)
    {
        canvasGroup.alpha = toggle ? 1 : 0;
        canvasGroup.blocksRaycasts = toggle;
        if (toggle)
            CursorManager.UnlockAndShow(); 
        else
            CursorManager.LockAndHide();   
    }

    public void ToggleChest(bool toggle)
    {
        secondPanelCanvasGroup.alpha = toggle ? 1 : 0;
        secondPanelCanvasGroup.blocksRaycasts = toggle;
        secondPanelCanvasGroup.interactable = toggle;
    }


    public void AddItem(Item item)
    {
        if (!TryStackItem(item))
            AddNewItems(item);
    }

    private bool TryStackItem(Item item)
    {

        for (int i = 0; i < _inventoryData.Length; i++)
        {
            var data = _inventoryData[i];
            if (String.IsNullOrEmpty(data.itemName))
                continue;

            if (data.itemName != item.ItemName)
                continue;

            data.amount++;
            data.inventoryItem.Init(item.ItemName, item.ItemPicture, data.amount);
            _inventoryData[i] = data;
            return true;
        }
        return false;
    }

    private void AddNewItems(Item item)
    {
        for (var i = 0; i < slots.Count; i++)
        {
            var slot = slots[i];
            if (!slot.IsEmpty)
                continue;

            var inventoryItem = Instantiate(itemPrefab, slot.transform);
            inventoryItem.Init(item.ItemName, item.ItemPicture, 1);
            var itemData = new InventoryItemData()
            {
                itemName = item.ItemName,
                itemPicture = item.ItemPicture,
                inventoryItem = inventoryItem,
                amount = 1
            };
            _inventoryData[i] = itemData;
            slot.SetItem(inventoryItem);
            break;
        }
    }

    internal void DropItem(InventoryItem inventoryItem)
    {
        for (int i = 0; i < _inventoryData.Length; i++)
        {
            var data = _inventoryData[i];
            if (data.inventoryItem != inventoryItem)
                continue;

            var itemToSpawn = GetItemByName(data.itemName);
            if (itemToSpawn == null)
            {
                Debug.LogError($"Item to spawn with name {data.itemName} not found!", this);
                return;
            }
            Vector3 spawnPosition = PlayerController.localPlayerController.transform.position + PlayerController.localPlayerController.transform.forward + Vector3.up;
            var item = Instantiate(itemToSpawn, spawnPosition, Quaternion.identity);

            if (DetuctItem(inventoryItem) <= 0)
                PlayerInventory.localInventory.UnequipItem(itemToSpawn);
            break;
        }
    }

    private int DetuctItem(InventoryItem inventoryItem)
    {
        for (int i = 0; i < _inventoryData.Length; i++)
        {
            var data = _inventoryData[i];
            if (data.inventoryItem != inventoryItem)
                continue;

            data.amount--;
            if (data.amount <= 0)
            {
                _inventoryData[i] = default;
                slots[i].SetItem(null);
                Destroy(inventoryItem.gameObject);
                return 0;
            }
            else
            {
                data.inventoryItem.Init(data.itemName, data.itemPicture, data.amount);
                _inventoryData[i] = data;
                return data.amount;
            }
        }
        return 0;

    }

    public void ItemMoved(InventoryItem item, InventorySlot newSlot)
    {
        int newSlotIndex = slots.IndexOf(newSlot);
        int oldSlotIndex = Array.FindIndex(_inventoryData, x => x.inventoryItem == item);

        if (oldSlotIndex == -1)
            return;

        var oldData = _inventoryData[oldSlotIndex];
        _inventoryData[oldSlotIndex] = default;
        _inventoryData[newSlotIndex] = oldData;

        if (newSlot.TryGetComponent<ActionSlot>(out var newActionSlot))
        {
            if (newActionSlot.IsActive)
            {
                SetActionSlotActive(newActionSlot); 
            }
        }
        if (slots[oldSlotIndex].TryGetComponent<ActionSlot>(out var oldActionSlot))
        {
            if (_activeActionSlot == oldActionSlot)
            {
                PlayerInventory.localInventory.UnequipItem(GetItemByName(oldData.itemName));
                _activeActionSlot.ToggleActive(false);
                _activeActionSlot = null;
            }
        }
    }

    public void SetActionSlotActive(ActionSlot actionSlot)
    {
        if (_activeActionSlot == actionSlot)
            return;

        if (_activeActionSlot != null)
        {
            PlayerInventory.localInventory.UnequipItem(GetItemByActionSlot(_activeActionSlot));
            _activeActionSlot.ToggleActive(false);
        }
        actionSlot.ToggleActive(true);
        _activeActionSlot = actionSlot;

        PlayerInventory.localInventory.EquipItem(GetItemByActionSlot(actionSlot));
    }

    public Item GetItemByName(String itemName)
    {
        return allItems.Find(x => x.ItemName == itemName);
    }

    private Item GetItemByActionSlot(ActionSlot actionSlot)
    {
        var inventorySlot = actionSlot.GetComponent<InventorySlot>();
        for (int i = slots.Count - 1; i >= 0; i--)
        {
            if (slots[i] == inventorySlot)
                return GetItemByName(_inventoryData[i].itemName);

        }
        return null;
    }

    public void RemoveItem(Item itemToRemove)
    {
        Debug.Log($"Attempting to remove item: {itemToRemove.ItemName}");
        for (int i = 0; i < _inventoryData.Length; i++)
        {
            var data = _inventoryData[i];
            if (data.itemName != itemToRemove.ItemName)
                continue;

            data.amount--;

            if (data.amount <= 0)
            {
                _inventoryData[i] = default;
                slots[i].SetItem(null);
                Destroy(data.inventoryItem.gameObject);

                if (PlayerInventory.localInventory.IsHoldingItem(itemToRemove))
                {
                    PlayerInventory.localInventory.UnequipItem(itemToRemove);
                }
            }
            else
            {
                data.inventoryItem.Init(data.itemName, data.itemPicture, data.amount);
                _inventoryData[i] = data;
            }

            return;
        }

        Debug.LogWarning($"Item '{itemToRemove.ItemName}' not found in inventory.");
    }

    public bool HasItem(Item item)
    {
        foreach (var data in _inventoryData)
        {
            if (data.itemName == item.ItemName)
            {
                return true; 
            }
        }
        return false; 
    }

    [Serializable]

    public struct InventoryItemData
    {
        public String itemName;
        public Sprite itemPicture;
        public InventoryItem inventoryItem;
        public int amount;
    }
}