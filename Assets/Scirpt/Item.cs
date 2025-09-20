using System;
using UnityEngine;
using Unity.VisualScripting;
using System.Collections.Generic;
using Unity.Collections;

public abstract class Item : MonoBehaviour
{
    [Header("Item Data")]
    [SerializeField] private string itemName;
    [SerializeField] private Sprite itemPicture;
    [SerializeField] private Rigidbody rigidbody;

    [Header("Interaction Data")]
    public float interactionDistance = 2f;
    [SerializeField] public bool isInteractable = true;

    private bool isPlayerNearby = false;

    private InteractableNameText interactableNameText;
    private GameObject interactableNameCanvas;

    public string ItemName => itemName;
    public Sprite ItemPicture => itemPicture;

    public virtual void Start()
    {
        interactableNameCanvas = GameObject.FindGameObjectWithTag("Canvas");
        if (interactableNameCanvas == null)
        {
            Debug.LogError("Canvas with tag 'Canvas' not found. Ensure a Canvas exists in the scene with the correct tag.");
            return;
        }

        interactableNameText = interactableNameCanvas.GetComponentInChildren<InteractableNameText>();
        if (interactableNameText == null)
        {
            Debug.LogError("InteractableNameText component not found in the Canvas. Ensure the Canvas has a child with this component.");
        }
    }

    public void TargetOn()
    {
        if (interactableNameText == null)
        {
            Debug.LogError("InteractableNameText is null. Cannot display interactable name.");
            return;
        }

        interactableNameText.ShowText(this);
        interactableNameText.SetInteractableNamePosition(this);

        if (!isPlayerNearby)
        {
            isPlayerNearby = true;
        }
    }


    [ContextMenu("Test Pick Up")]
    public void PickUp()
    {
        Debug.Log($"[PickUp] Attempting to pick up item: {ItemName}", this);

        if (!InstanceHandler.TryGetInstance(out InventoryManager inventoryManager))
        {
            Debug.LogError("[PickUp] InventoryManager not found. Item cannot be picked up.", this);
            return;
        }

        // Removed the HasItem check here to allow stacking

        Debug.Log($"[PickUp] InventoryManager found. Adding item: {ItemName}");
        inventoryManager.AddItem(this);

        Debug.Log($"[PickUp] Destroying item: {ItemName}");
        Destroy(gameObject);
    }

    public virtual String GetItemName()
    {
        return itemName;
    }

    public void TargetOff()
    {
        interactableNameText.HideText();

        if (isPlayerNearby)
        {
            isPlayerNearby = false;
        }
    }

    public virtual void Interact()
    {
        if (isInteractable)
        {
            //Debug.Log($"Interacting with: {ItemName}", this);
            PickUp();
        }
    }

    protected virtual void Interaction()
    {
        // Optional override in subclasses
    }

    public virtual void UseItem() { }

    public virtual void ConsumeItem() { }

    public virtual bool CanConsumeItem()
    {
        return false;
    }

    public void SetKinematic(bool toggle)
    {
        rigidbody.isKinematic = toggle;
        rigidbody.useGravity = !toggle;

        if (!toggle)
        {
            rigidbody.linearVelocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }

    private void OnDestroy()
    {
        TargetOff();
    }
}