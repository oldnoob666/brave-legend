using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item Data")]
public sealed class ItemData : ScriptableObject
{
    [SerializeField] private string displayName = "Item";
    [SerializeField] private Color displayColor = Color.gray;

    public string DisplayName => displayName;
    public Color DisplayColor => displayColor;
}
