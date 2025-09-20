//using UnityEngine;

//public class ChestInteraction : MonoBehaviour
//{
//    private bool isPlayerNearby = false;
//    [SerializeField] private ChestInventoryManager chestInventoryManager;
//    [SerializeField] private CanvasGroup chestPanelCanvasGroup;  // UI Panel for the chest

//    private void Start()
//    {
//        if (chestInventoryManager == null)
//            chestInventoryManager = GetComponent<ChestInventoryManager>(); // Reference to the chest inventory
//    }

//    private void Update()
//    {
//        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
//        {
//            OpenChestInventory();
//        }
//        else if (Input.GetKeyDown(KeyCode.Escape))
//        {
//            CloseChestInventory();
//        }
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        if (other.CompareTag("Player"))
//        {
//            isPlayerNearby = true;
//        }
//    }

//    private void OnTriggerExit(Collider other)
//    {
//        if (other.CompareTag("Player"))
//        {
//            isPlayerNearby = false;
//        }
//    }

//    private void OpenChestInventory()
//    {
//        chestPanelCanvasGroup.alpha = 1;  // Make the chest UI visible
//        chestPanelCanvasGroup.blocksRaycasts = true;  // Enable interactions with the chest UI
//        chestInventoryManager.InitializeChestInventory();  // Load the chest's unique items into the UI
//    }

//    private void CloseChestInventory()
//    {
//        chestPanelCanvasGroup.alpha = 0;  // Hide the chest UI
//        chestPanelCanvasGroup.blocksRaycasts = false;  // Disable interactions with the chest UI
//        chestInventoryManager.ClearChestInventory();  // Clear the chest UI
//    }
//}
