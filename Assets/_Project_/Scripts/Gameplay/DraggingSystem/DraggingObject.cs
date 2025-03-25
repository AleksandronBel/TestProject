using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggingObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image _image;
    [SerializeField] private RectTransform _rectTransform;

    public Canvas MainCanvas { get; set; }

    [SerializeField] private CanvasGroup _canvasGroup;

    public bool IsFigureInTower { get; set; }

    public Image Image => _image;
    public RectTransform RectTransform => _rectTransform;
    public CanvasGroup CanvasGroup => _canvasGroup;

    public bool IsDraggingObjectRemoved { get; set; }

    Vector3 startPosition;

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPosition = transform.position;
     
        _canvasGroup.blocksRaycasts = false;
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        _rectTransform.anchoredPosition += eventData.delta / MainCanvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = true;

        if (IsDraggingObjectRemoved) return;

        transform.position = startPosition;
    }

    public void DestroyWithAnimation()
    {
        transform.DOScale(Vector3.zero, 0.3f).OnComplete(() => Destroy(gameObject));
    }
}
