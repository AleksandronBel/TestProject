using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

public class ActionMessage : MonoBehaviour
{
    [SerializeField] private LocalizeStringEvent _actionTextEvent;
    [SerializeField] private TMP_Text _actionText;

    public LocalizeStringEvent ActionTextEvent => _actionTextEvent;
    public TMP_Text ActionText => _actionText;
}
