//using UnityEngine;

//public class ChestInteractable : MonoBehaviour
//{
//    private void OnTriggerEnter(Collider other)
//    {
//        if (other.CompareTag("Player"))
//        {
//            InventoryManager inventoryManager = InstanceHandler.GetInstance<InventoryManager>();
//            if (inventoryManager != null)
//            {
//                inventoryManager.SetNearChest(true, transform);
//            }
//        }
//    }

//    private void OnTriggerExit(Collider other)
//    {
//        if (other.CompareTag("Player"))
//        {
//            InventoryManager inventoryManager = InstanceHandler.GetInstance<InventoryManager>();
//            if (inventoryManager != null)
//            {
//                inventoryManager.SetNearChest(false);
//            }
//        }
//    }
//}