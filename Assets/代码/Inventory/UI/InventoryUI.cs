using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class InventoryUI : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private GameObject panel;
    [SerializeField] private Transform contentRoot;
    [SerializeField] private InventoryItemView itemViewPrefab;
    [SerializeField] private GameObject emptyState;

    private readonly List<InventoryItemView> itemViews = new List<InventoryItemView>();
    private PlayerInputControl inputControl;
    private InputAction toggleInventoryAction;

    private void Awake()
    {
        inputControl = new PlayerInputControl();
        toggleInventoryAction = inputControl.asset.FindAction("GamePlayer/ToggleInventory", true);
        panel.SetActive(false);
    }

    private void OnEnable()
    {
        inventory.Changed += OnInventoryChanged;
        toggleInventoryAction.performed += OnToggleInventory;
        inputControl.Enable();
    }

    private void OnDisable()
    {
        inventory.Changed -= OnInventoryChanged;
        toggleInventoryAction.performed -= OnToggleInventory;
        inputControl.Disable();
    }

    private void OnToggleInventory(InputAction.CallbackContext context)
    {
        var shouldOpen = !panel.activeSelf;
        panel.SetActive(shouldOpen);
        if (shouldOpen)
        {
            Refresh();
        }
    }

    private void OnInventoryChanged()
    {
        if (panel.activeSelf)
        {
            Refresh();
        }
    }

    private void Refresh()
    {
        var stacks = inventory.Stacks;
        emptyState.SetActive(stacks.Count == 0);

        while (itemViews.Count < stacks.Count)
        {
            itemViews.Add(Instantiate(itemViewPrefab, contentRoot));
        }

        for (var index = 0; index < itemViews.Count; index++)
        {
            var isUsed = index < stacks.Count;
            itemViews[index].gameObject.SetActive(isUsed);
            if (isUsed)
            {
                itemViews[index].Bind(stacks[index]);
            }
        }
    }
}
