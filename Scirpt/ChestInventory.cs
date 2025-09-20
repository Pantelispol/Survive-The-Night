//using UnityEngine;

//public class ChestInventory : MonoBehaviour
//{
//    [SerializeField] private int slotCount = 12;
//    [SerializeField] private Item[] initialItems;

//    public Item[] Items { get; private set; }

//    private void Awake()
//    {
//        InitializeSlots();
//    }

//    private void InitializeSlots()
//    {
//        Items = new Item[slotCount];
//        for (int i = 0; i < initialItems.Length && i < slotCount; i++)
//        {
//            Items[i] = initialItems[i];
//        }
//    }

//    public bool AddItem(Item item)
//    {
//        for (int i = 0; i < Items.Length; i++)
//        {
//            if (Items[i] == null)
//            {
//                Items[i] = item;
//                return true;
//            }
//        }
//        return false;
//    }

//    public void RemoveItem(int slotIndex)
//    {
//        if (slotIndex >= 0 && slotIndex < Items.Length)
//        {
//            Items[slotIndex] = null;
//        }
//    }
//}