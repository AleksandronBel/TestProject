using MessagePipe;
using Zenject;
using R3;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.Localization.Components;
using System;
using TMPro;
using DG.Tweening;

public class MessagesSubscriber : MonoBehaviour, IDisposable
{
    [SerializeField] private LocalizeStringEvent _actionTextEvent;
    [SerializeField] private TMP_Text _actionText;
    
    [SerializeField] private float fadeTextDuration = 0.3f;
    [SerializeField] private float visibleTextDuration = 1f;

    private IDisposable _subscription;

    [Inject] ISubscriber<FigureActionMessage.FigurePlaced> _figureDragStart;
    [Inject] ISubscriber<FigureActionMessage.FigureOut> _figureOut;
    [Inject] ISubscriber<FigureActionMessage.FigureDisappear> _figureDissapear;
    [Inject] ISubscriber<FigureActionMessage.TowerHeightLimit> _figureHeightLimit;

    public void Dispose() => _subscription?.Dispose();

    [Inject]
    private void Construct()
    {
        var bag = MessagePipe.DisposableBag.CreateBuilder();

        _figureDragStart.Subscribe(HandleMessage).AddTo(bag);
        _figureOut.Subscribe(HandleMessage).AddTo(bag);
        _figureDissapear.Subscribe(HandleMessage).AddTo(bag);
        _figureHeightLimit.Subscribe(HandleMessage).AddTo(bag);

        _subscription = bag.Build();
    }

    private void HandleMessage<T>(T message) where T : IMessage
    {
        _actionTextEvent.StringReference.TableEntryReference = message.MessageType.ToString();

        ShowTextWithAnimation();
    }

    private void ShowTextWithAnimation()
    {
        _actionText.gameObject.SetActive(true);
        _actionText.color = new Color(_actionText.color.r, _actionText.color.g, _actionText.color.b, 0);

        Sequence sequence = DOTween.Sequence();

        sequence.Append(_actionText.DOFade(1, fadeTextDuration))
        .AppendInterval(visibleTextDuration)
                .Append(_actionText.DOFade(0, fadeTextDuration))
                .OnComplete(() => _actionText.gameObject.SetActive(false));
    }
}
