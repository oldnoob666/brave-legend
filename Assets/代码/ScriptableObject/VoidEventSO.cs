using UnityEngine;
using UnityEngine.Events;
[CreateAssetMenu(menuName ="Event/VoidEventSO")]
public class VoidEventSO:ScriptableObject
{
    public UnityAction OnEventRised;
    public void RaiseEvent()
    {
        OnEventRised?.Invoke();
    }
}
