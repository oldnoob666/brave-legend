using NUnit.Framework;
using System.Reflection;
using UnityEngine;

public sealed class InventoryTests
{
    [Test]
    public void AddItem_SameItemTwice_ProducesOneStackWithSummedQuantity()
    {
        var inventoryObject = new GameObject("Inventory Test");
        var inventory = inventoryObject.AddComponent<Inventory>();
        var ore = ScriptableObject.CreateInstance<ItemData>();

        try
        {
            inventory.AddItem(ore, 2);
            inventory.AddItem(ore, 3);

            Assert.That(inventory.Stacks.Count, Is.EqualTo(1));
            Assert.That(inventory.Stacks[0].Item, Is.SameAs(ore));
            Assert.That(inventory.Stacks[0].Quantity, Is.EqualTo(5));
        }
        finally
        {
            Object.DestroyImmediate(ore);
            Object.DestroyImmediate(inventoryObject);
        }
    }

    [Test]
    public void AddItem_NullItem_ReturnsFalseWithoutChangingInventory()
    {
        var inventoryObject = new GameObject("Inventory Test");
        var inventory = inventoryObject.AddComponent<Inventory>();
        var changedCount = 0;
        inventory.Changed += () => changedCount++;

        try
        {
            var added = inventory.AddItem(null, 1);

            Assert.That(added, Is.False);
            Assert.That(inventory.Stacks, Is.Empty);
            Assert.That(changedCount, Is.Zero);
        }
        finally
        {
            Object.DestroyImmediate(inventoryObject);
        }
    }

    [TestCase(0)]
    [TestCase(-2)]
    public void AddItem_NonPositiveAmount_ReturnsFalseWithoutChangingInventory(int amount)
    {
        var inventoryObject = new GameObject("Inventory Test");
        var inventory = inventoryObject.AddComponent<Inventory>();
        var ore = ScriptableObject.CreateInstance<ItemData>();
        var changedCount = 0;
        inventory.Changed += () => changedCount++;

        try
        {
            var added = inventory.AddItem(ore, amount);

            Assert.That(added, Is.False);
            Assert.That(inventory.Stacks, Is.Empty);
            Assert.That(changedCount, Is.Zero);
        }
        finally
        {
            Object.DestroyImmediate(ore);
            Object.DestroyImmediate(inventoryObject);
        }
    }

    [Test]
    public void Raise_ValidItem_NotifiesSubscribersWithPayload()
    {
        var addItemEvent = ScriptableObject.CreateInstance<AddItemEventSO>();
        var ore = ScriptableObject.CreateInstance<ItemData>();
        ItemData receivedItem = null;
        var receivedAmount = 0;
        addItemEvent.OnEventRaised += (item, amount) =>
        {
            receivedItem = item;
            receivedAmount = amount;
        };

        try
        {
            addItemEvent.Raise(ore, 4);

            Assert.That(receivedItem, Is.SameAs(ore));
            Assert.That(receivedAmount, Is.EqualTo(4));
        }
        finally
        {
            Object.DestroyImmediate(ore);
            Object.DestroyImmediate(addItemEvent);
        }
    }

    [Test]
    public void EnabledInventory_AddItemEventRaised_AddsRequestedItem()
    {
        var inventoryObject = new GameObject("Inventory Test");
        inventoryObject.SetActive(false);
        var inventory = inventoryObject.AddComponent<Inventory>();
        var addItemEvent = ScriptableObject.CreateInstance<AddItemEventSO>();
        var ore = ScriptableObject.CreateInstance<ItemData>();

        try
        {
            var eventField = typeof(Inventory).GetField("addItemEvent", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(eventField, Is.Not.Null);
            eventField.SetValue(inventory, addItemEvent);
            inventoryObject.SetActive(true);
            var onEnable = typeof(Inventory).GetMethod("OnEnable", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(onEnable, Is.Not.Null);
            onEnable.Invoke(inventory, null);

            addItemEvent.Raise(ore, 4);

            Assert.That(inventory.Stacks.Count, Is.EqualTo(1));
            Assert.That(inventory.Stacks[0].Item, Is.SameAs(ore));
            Assert.That(inventory.Stacks[0].Quantity, Is.EqualTo(4));
        }
        finally
        {
            Object.DestroyImmediate(ore);
            Object.DestroyImmediate(addItemEvent);
            Object.DestroyImmediate(inventoryObject);
        }
    }
}
