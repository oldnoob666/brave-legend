using System;
using UnityEngine;

[Serializable]
public sealed class InventoryStack
{
    [SerializeField] private ItemData item;
    [SerializeField] private int quantity;

    public ItemData Item => item;
    public int Quantity => quantity;

    public InventoryStack(ItemData item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    }

    public void Add(int amount)
    {
        quantity += amount;
    }
}
