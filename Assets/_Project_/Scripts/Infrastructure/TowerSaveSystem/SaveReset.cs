using MessagePipe;
using R3;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class SaveReset : MonoBehaviour
{
    [Inject] private IPublisher <TowerMessages.TowerResetData> _resetTowerData;

    [SerializeField] private Button _resetSaveButton;

    [Inject]
    private void Construct()
    {
        _resetSaveButton.OnClickAsObservable().Subscribe(_ => ResetSave()).AddTo(this);
    }

    private void ResetSave()
    {
        _resetTowerData.Publish(new());
    }
}