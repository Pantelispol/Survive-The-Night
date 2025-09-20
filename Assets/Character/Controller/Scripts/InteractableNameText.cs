using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class InteractableNameText : MonoBehaviour
{
    TextMeshProUGUI text;
    string[] validLayers = { "Items", "Interactables", "Enemy" };

    Transform cameraTransform;
    Transform playerTransform;
    void Start()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        cameraTransform = Camera.main.transform;

        // Find player by tag (assign "Player" tag in Unity)
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("Player not found by tag. Ensure your player has the 'Player' tag.");
        }

        HideText();
    }
    public void ShowText(Item interactable)
    {
        //Debug.Log($"[ShowText] Interactable type: {interactable.GetType().Name}, Layer: {LayerMask.LayerToName(interactable.gameObject.layer)}");

        // Check if the layer corresponds to "Item"
        if (validLayers.Contains(LayerMask.LayerToName(interactable.gameObject.layer)))
        {
            text.text = interactable.GetItemName() + "\n [E] Pick Up";
        }
        else
        {
            text.text = interactable.GetItemName();
        }
    }


    public void HideText()
    {
        text.text = "";
    }

    public void SetInteractableNamePosition(Item interactable)
    {
        if (playerTransform == null)
        {
            Debug.LogError("Player transform is null. Cannot set interactable name position.");
            return;
        }

        // Calculate the position of the text relative to the player's position
        if (interactable.TryGetComponent(out BoxCollider boxCollider))
        {
            transform.position = interactable.transform.position + Vector3.up * (boxCollider.bounds.size.y + 0.5f);
        }
        else if (interactable.TryGetComponent(out CapsuleCollider capsCollider))
        {
            transform.position = interactable.transform.position + Vector3.up * (capsCollider.height + 0.5f);
        }
        else
        {
            Debug.LogError("Error: No collider found on the interactable item!");
            return;
        }

        // Make the text face the player
        transform.LookAt(2 * transform.position - playerTransform.position);

        // Lock the X-axis rotation to -30 degrees while keeping Y and Z dynamic
        Vector3 currentRotation = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(30f, currentRotation.y, currentRotation.z);
    }

}