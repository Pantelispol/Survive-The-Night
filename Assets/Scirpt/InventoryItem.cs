using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler,
    IPointerClickHandler
{
    [SerializeField] private TMP_Text amountText;

    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;
    private Transform _originalParent;
    private Canvas _canvas;
    private Image _itemImage;

    public void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _itemImage = GetComponent<Image>();
        _canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _originalParent = transform.parent;
        _canvasGroup.blocksRaycasts = false;
        _rectTransform.SetParent(_rectTransform.root);
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.pointerEnter == null ||
            !eventData.pointerEnter.TryGetComponent(out InventorySlot inventorySlot))
        {
            _rectTransform.SetParent(_originalParent);
            SetAvailable();
        }
    }

    public void SetAvailable()
    {
        _canvasGroup.blocksRaycasts = true;
        _rectTransform.anchoredPosition = Vector2.zero;
    }

    public void Init(string itemName, Sprite itemPicture, int amount)
    {
        _itemImage.sprite = itemPicture;
        amountText.text = amount.ToString();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right)
            return;

        if (!InstanceHandler.TryGetInstance(out InventoryManager inventoryManager))
        {
            Debug.LogError("InventoryManager not found", this);
            return;
        }

        inventoryManager.DropItem(this);
    }
}