using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class InventoryItemView : MonoBehaviour
{
    [SerializeField] private Image colorBlock;
    [SerializeField] private TMP_Text itemLabel;

    public void Bind(InventoryStack stack)
    {
        colorBlock.color = stack.Item.DisplayColor;
        itemLabel.text = $"{stack.Item.DisplayName} x {stack.Quantity}";
    }
}
