using R3;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class SaveReset : MonoBehaviour
{
    [SerializeField] private Button _resetSaveButton;

    private ISaveLoadProgressService _saveLoadProgressService;

    [Inject]
    private void Construct(ISaveLoadProgressService saveLoadProgressService)
    {
        _saveLoadProgressService = saveLoadProgressService;
        _resetSaveButton.OnClickAsObservable().Subscribe(_ => ResetSave()).AddTo(this);
    }

    private void ResetSave()
    {
        _saveLoadProgressService.ResetSave();
    }
}