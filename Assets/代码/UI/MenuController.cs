using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Click probe for the buttons of the Menu scene. NO gameplay logic in here.
/// Put this component on the "Canvas" object. It finds the NewGame / Continue /
/// Exit buttons under itself by name and wires their onClick listeners in code,
/// so nothing has to be filled in the Button's OnClick list by hand.
/// When a button is really clicked, a log line appears in the Console - that is
/// the proof that the whole click pipeline (EventSystem -> raycast -> Button)
/// works. Add the real logic later inside OnNewGameClick / OnContinueClick /
/// OnExitClick.
///
/// NOTE: comments are ASCII on purpose - the older scripts of this project are
/// saved as GBK while this file is UTF-8, so non-ASCII text could look garbled.
/// </summary>
public class MenuController : MonoBehaviour
{
    [Header("Buttons (leave empty to find them by name under this object)")]
    public Button newGameButton;
    public Button continueButton;
    public Button exitButton;

    private void Awake()
    {
        if (newGameButton == null) newGameButton = FindButton("NewGame");
        if (continueButton == null) continueButton = FindButton("Continue");
        if (exitButton == null) exitButton = FindButton("Exit");

        if (newGameButton != null) newGameButton.onClick.AddListener(OnNewGameClick);
        if (continueButton != null) continueButton.onClick.AddListener(OnContinueClick);
        if (exitButton != null) exitButton.onClick.AddListener(OnExitClick);
    }

    private void Start()
    {
        // A UI click is only dispatched when the scene contains an EventSystem
        // (with an input module on it). Without it every button looks dead.
        // EventSystem.current is assigned when the EventSystem itself is enabled,
        // so this check belongs in Start, never in Awake.
        if (EventSystem.current == null)
        {
            Debug.LogError("[MenuController] This scene has no EventSystem, so UI clicks " +
                "are never dispatched. Fix: GameObject > UI > Event System.");
        }

        Debug.Log("[MenuController] ready. newGame=" + (newGameButton != null) +
            " continue=" + (continueButton != null) +
            " exit=" + (exitButton != null) +
            " eventSystem=" + (EventSystem.current != null));
    }

    private void OnDestroy()
    {
        if (newGameButton != null) newGameButton.onClick.RemoveListener(OnNewGameClick);
        if (continueButton != null) continueButton.onClick.RemoveListener(OnContinueClick);
        if (exitButton != null) exitButton.onClick.RemoveListener(OnExitClick);
    }

    private Button FindButton(string objectName)
    {
        Button[] buttons = GetComponentsInChildren<Button>(true);
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i].gameObject.name == objectName)
                return buttons[i];
        }
        return null;
    }

    // Public so they can also be wired by hand from the Button OnClick list.
    public void OnNewGameClick()
    {
        Debug.Log("[MenuController] NewGame button clicked.");
    }

    public void OnContinueClick()
    {
        Debug.Log("[MenuController] Continue button clicked.");
    }

    public void OnExitClick()
    {
        Debug.Log("[MenuController] Exit button clicked.");
    }
}
