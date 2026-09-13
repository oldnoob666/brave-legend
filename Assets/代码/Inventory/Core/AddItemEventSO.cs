using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Add Item Event", menuName = "Inventory/Add Item Event")]//ai生成的背包内容
public sealed class AddItemEventSO : ScriptableObject
{
    public event Action<ItemData, int> OnEventRaised;

    public void Raise(ItemData item, int amount)
    {
        OnEventRaised?.Invoke(item, amount);
    }
}
