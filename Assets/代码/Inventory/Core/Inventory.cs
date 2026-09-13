using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class Inventory : MonoBehaviour
{
    [SerializeField] private AddItemEventSO addItemEvent;
    [SerializeField] private List<InventoryStack> stacks = new List<InventoryStack>();

    public IReadOnlyList<InventoryStack> Stacks => stacks;
    public event Action Changed;

    private void OnEnable()
    {
        if (addItemEvent != null)
        {
            addItemEvent.OnEventRaised += OnAddItemRequested;
        }
    }

    private void OnDisable()
    {
        if (addItemEvent != null)
        {
            addItemEvent.OnEventRaised -= OnAddItemRequested;
        }
    }

    public bool AddItem(ItemData item, int amount)
    {
        if (item == null)
        {
            Debug.LogWarning("Inventory cannot add a missing item.", this);
            return false;
        }

        if (amount <= 0)
        {
            Debug.LogWarning($"Inventory cannot add '{item.DisplayName}' with a non-positive amount ({amount}).", this);
            return false;
        }

        var existingStack = stacks.Find(stack => stack.Item == item);
        if (existingStack == null)
        {
            stacks.Add(new InventoryStack(item, amount));
        }
        else
        {
            existingStack.Add(amount);
        }

        Changed?.Invoke();
        return true;
    }

    private void OnAddItemRequested(ItemData item, int amount)
    {
        AddItem(item, amount);
    }
}
