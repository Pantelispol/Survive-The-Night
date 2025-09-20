using System.Linq;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private float interactionDistance = 6f;
    [SerializeField] private Transform playerTransform; // Assign this in the inspector

    private AInteractable[] _currentHoveredInteractables;

    private void Update()
    {
        if (playerTransform == null)
            return; // Don't do anything if the playerTransform is missing
        HandleHovers();

        // Check for interaction input
        if (!Input.GetKeyDown(KeyCode.E))
            return;
        InventoryManager inventoryManager = InstanceHandler.GetInstance<InventoryManager>();
        if (inventoryManager.IsChestOpen())
        {
            inventoryManager.ToggleChest(false);
            return;
        }
        // Use OverlapSphere to detect interactables within a radius
        Collider[] hitColliders = Physics.OverlapSphere(playerTransform.position, interactionDistance, interactableLayer);

        // Find the closest interactable
        AInteractable closestInteractable = null;
        float closestDistance = float.MaxValue;

        foreach (var hitCollider in hitColliders)
        {
            var interactables = hitCollider.GetComponents<AInteractable>();
            foreach (var interactable in interactables)
            {
                if (interactable.CanInteract())
                {
                    float distance = Vector3.Distance(playerTransform.position, interactable.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestInteractable = interactable;
                    }
                }
            }
        }

        // Interact with the closest interactable, if any
        if (closestInteractable != null)
        {
            closestInteractable.Interact();
        }
    }

    private void HandleHovers()
    {
        // Use OverlapSphere to detect interactables for hover
        Collider[] hitColliders = Physics.OverlapSphere(playerTransform.position, interactionDistance, interactableLayer);

        if (hitColliders.Length == 0)
        {
            ClearHover();
            return;
        }

        var interactables = hitColliders
            .SelectMany(collider => collider.GetComponents<AInteractable>())
            .ToArray();

        if (interactables.Length == 0)
        {
            ClearHover();
            return;
        }

        if (_currentHoveredInteractables != null && _currentHoveredInteractables.Length > 0)
        {
            if (!_currentHoveredInteractables[0])
            {
                ClearHover();
                return;
            }

            if (_currentHoveredInteractables[0].gameObject == interactables[0].gameObject)
                return;
        }

        _currentHoveredInteractables = interactables;

        foreach (var interactable in interactables)
        {
            if (interactable.CanInteract())
            {
                interactable.OnHover();
            }
        }
    }

    private void ClearHover()
    {
        if (_currentHoveredInteractables == null || _currentHoveredInteractables.Length <= 0)
            return;

        foreach (var interactable in _currentHoveredInteractables)
        {
            if (interactable)
                interactable.OnStopHover();
        }

        _currentHoveredInteractables = null;
    }
}

public abstract class AInteractable : MonoBehaviour
{
    public abstract void Interact();

    public virtual void OnHover() { }
    public virtual void OnStopHover() { }
    public virtual bool CanInteract()
    {
        return true;
    }
}