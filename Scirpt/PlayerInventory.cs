using Script.Controller;
using System.Collections;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private KeyCode useItemKey, consumeItemKey;
    [SerializeField] private Transform itemPoint;
    private bool isSwitchingItem = false;
    private bool isEquipInProgress = false;
    private Item _itemInHand;
    private CombatState _combatState;
    private Item _previousItem; 
    private Vector3 originalItemPointPosition;

    public static PlayerInventory localInventory { get; private set; }

    private void Awake()
    {
        if (localInventory == null)
        {
            localInventory = this;
        }
        else
        {
            Destroy(gameObject);
        }
        _combatState = GetComponent<CombatState>();
    }

    public Item getItemInHand()
    {
        return _itemInHand;
    }

    private void Update()
    {
        if (Input.GetKeyDown(useItemKey))
            UseItem();

        if (Input.GetKeyDown(consumeItemKey))
        {
            Debug.Log("ConsumeItem key pressed");
            ConsumeItem();
        }
    }


    private void UseItem()
    {
        if (!_itemInHand)
            return;
        _itemInHand.UseItem();
    }

    private void ConsumeItem()
    {
        if (!_itemInHand)
            return;
        if (!_itemInHand.CanConsumeItem())
            return;
        _itemInHand.ConsumeItem();
        if (InstanceHandler.TryGetInstance(out InventoryManager inventoryManager))
            inventoryManager.RemoveItem(_itemInHand);
    }

    private bool CanPickUpItem()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 2f); 
        foreach (var collider in hitColliders)
        {
            if (collider.TryGetComponent(out Item item) && item.isInteractable)
            {
                return true; 
            }
        }
        return false; 
    }


    private void OnDestroy()
    {
        if (localInventory == this)
        {
            localInventory = null;
        }
    }

    protected void OnSpawned()
    {
        localInventory = this;
    }

    protected void OnDespawned()
    {
        localInventory = null;
    }

    public void EquipItem(Item item)
    {
        if (!item || isEquipInProgress) return;

        isEquipInProgress = true;

        bool wasHoldingSword = IsHoldingSword();
        if (_itemInHand == null)
        {
            originalItemPointPosition = itemPoint.localPosition;
        }
        if (item is HealingItem || item is RegeneratingHealthPotion)
        {
            itemPoint.localPosition = new Vector3(-0.056f, -0.576f, 0.008f);
        }

        if (_itemInHand != null)
            StartCoroutine(UnequipAndEquip(item, wasHoldingSword));
        else
            StartCoroutine(EquipNewItemWithRetry(item));
    }

    private IEnumerator UnequipAndEquip(Item newItem, bool wasHoldingSword)
    {
        isSwitchingItem = true; 
        yield return StartCoroutine(UnequipItemCoroutine(_itemInHand, skipAnimation: !wasHoldingSword));

        if (wasHoldingSword)
            yield return new WaitForSeconds(1.5f); 

        yield return StartCoroutine(EquipNewItemWithRetry(newItem));
    }

    private IEnumerator EquipNewItemWithRetry(Item item)
    {
        EquipNewItem(item);

        yield return new WaitForEndOfFrame();
        if (IsHoldingSword() && _combatState != null && _combatState.IsDrawingWeapon())
        {
            Debug.LogWarning("CombatState did not update correctly. Retrying equip...");
            EquipNewItem(item);
        }

        isEquipInProgress = false; 
    }

    private void EquipNewItem(Item item)
    {
        if (_combatState != null)
        {
            _combatState.EndDealDamage();
        }

        _itemInHand = Instantiate(item, itemPoint.position, transform.rotation, itemPoint);
        _itemInHand.transform.localPosition = Vector3.zero;
        _itemInHand.transform.localRotation = Quaternion.identity;
        _itemInHand.SetKinematic(true);

        _itemInHand.gameObject.layer = LayerMask.NameToLayer("HeldItem");

        if (IsHoldingSword())
        {
            _combatState.DrawWeapon(_itemInHand.gameObject, itemPoint);
        }
    }


    public void UnequipItem(Item item, bool skipAnimation = false)
    {
        if (!item || !_itemInHand || _itemInHand.ItemName != item.ItemName || isEquipInProgress)
            return;

        if (IsHoldingSword() && !skipAnimation && _combatState.IsDrawingWeapon())
        {
            Debug.LogWarning("Cannot unequip the sword while the draw animation is still playing.");
            return;
        }

        itemPoint.localPosition = originalItemPointPosition;
        StartCoroutine(UnequipItemCoroutine(item, skipAnimation));
    }

    private IEnumerator UnequipItemCoroutine(Item item, bool skipAnimation)
    {
        isSwitchingItem = true; 
        bool wasHoldingSword = IsHoldingSword();

        if (wasHoldingSword && !skipAnimation)
        {
            _combatState.SheathWeapon();
            yield return new WaitForSeconds(1.5f);
        }

        if (_itemInHand != null)
        {
            Destroy(_itemInHand.gameObject);
            _itemInHand = null;

            _combatState.EndDealDamage(); 
        }

        isSwitchingItem = false;
    }


    public bool IsHoldingItem(Item item)
    {
        return _itemInHand != null && _itemInHand == item;
    }

    public void SelectHotbarSlot(Item itemInSlot)
    {
        if (_itemInHand == itemInSlot)
            return;

        StartCoroutine(SwitchHotbarSlot(itemInSlot));
    }

    private IEnumerator SwitchHotbarSlot(Item newItem)
    {
        if (_itemInHand != null)
            yield return StartCoroutine(UnequipItemCoroutine(_itemInHand, false));

        if (newItem != null)
            EquipNewItem(newItem);
    }

    public bool IsHoldingSword()
    {
        if (_itemInHand == null)
            return false;

        return _itemInHand.ItemName == "Sword1" ||
               _itemInHand.ItemName == "Sword2" ||
               _itemInHand.ItemName == "Sword3";
    }
}