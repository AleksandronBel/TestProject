using MessagePipe;
using Zenject;
using R3;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.Localization.Components;
using System;
using TMPro;
using DG.Tweening;

public class MessagesUIShower : MonoBehaviour, IDisposable
{
    [Inject] ISubscriber<FigureActionMessage.FigureAction> _figureActionMessage;

    [SerializeField] private ActionMessage _actionMessagePrefab;
    [SerializeField] private RectTransform _messagesContainer;

    [SerializeField] private float _fadeStartTextDuration = 0.3f;
    [SerializeField] private float _fadeEndTextDuration = 1f;
    [SerializeField] private float _visibleTextDuration = 0.3f;

    [SerializeField] private float _offset = 500f;
    [SerializeField] private float _upOffsetDuration = 2f;

    private IDisposable _subscription;

    public void Dispose() => _subscription?.Dispose();

    [Inject]
    private void Construct()
    {
        var bag = MessagePipe.DisposableBag.CreateBuilder();

        _figureActionMessage.Subscribe(HandleMessage).AddTo(bag);

        _subscription = bag.Build();
    }

    private void HandleMessage<T>(T message) where T : IFigureMessage
    {
        var messageObjectPrefab = Instantiate(_actionMessagePrefab, _messagesContainer);

        messageObjectPrefab.ActionTextEvent.StringReference.TableEntryReference = message.MessageType.ToString();

        ShowTextWithAnimation(messageObjectPrefab);
    }

    private void ShowTextWithAnimation(ActionMessage messageObjectPrefab)
    {
        messageObjectPrefab.gameObject.SetActive(true);
        messageObjectPrefab.ActionText.color = new Color(
            messageObjectPrefab.ActionText.color.r,
            messageObjectPrefab.ActionText.color.g,
            messageObjectPrefab.ActionText.color.b,
            0
        );


        // 1. Анимация движения ВВЕРХ через AnchorPos
        var moveTween = messageObjectPrefab.GetComponent<RectTransform>()
            .DOAnchorPosY(messageObjectPrefab.transform.localPosition.y + _offset, _upOffsetDuration) // Используем положительный offset
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart) // Меняем тип повтора
            .OnKill(() => {
                if (messageObjectPrefab != null)
                    Destroy(messageObjectPrefab.gameObject);
            });

        // 2. Последовательность для фейдов (без изменений)
        Sequence fadeSequence = DOTween.Sequence();
        fadeSequence
            .Append(messageObjectPrefab.ActionText.DOFade(1, _fadeStartTextDuration))
            .AppendInterval(_visibleTextDuration)
            .Append(messageObjectPrefab.ActionText.DOFade(0, _fadeEndTextDuration))
            .OnComplete(() => moveTween?.Kill());
    }
}
