using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Editor;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class SimpleInventorySetup
{
    private const string InputActionsPath = "Assets/Settings/InputSystem/PlayerInputControl.inputactions";
    private const string InputWrapperPath = "Assets/Settings/InputSystem/PlayerInputControl.cs";
    private const string PersistentScenePath = "Assets/Scenes/persistent.unity";
    private const string ForestScenePath = "Assets/Scenes/Forest.unity";
    private const string InventoryDataFolder = "Assets/Data SO/Inventory";
    private const string EventAssetPath = "Assets/Data SO/Events/Add Item Event.asset";
    private const string OreAssetPath = InventoryDataFolder + "/Ore.asset";
    private const string WoodAssetPath = InventoryDataFolder + "/Wood.asset";
    private const string RowPrefabPath = "Assets/Prefabs/UI/InventoryItemRow.prefab";
    private static string ResultPath => Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Logs", "InventorySetup-result.txt");

    public static void ConfigureProject()
    {
        EnsureFolders();
        if (!EnsureTmpEssentials())
        {
            return;
        }

        FinishConfiguration();
    }

    public static void RegenerateInputWrapper()
    {
        var inputActions = AssetDatabase.LoadAssetAtPath<InputActionAsset>(InputActionsPath);
        if (inputActions == null)
        {
            throw new InvalidOperationException($"Input Actions asset was not found at '{InputActionsPath}'.");
        }

        var options = new InputActionCodeGenerator.Options
        {
            className = "PlayerInputControl",
            namespaceName = string.Empty,
            sourceAssetPath = InputActionsPath
        };
        InputActionCodeGenerator.GenerateWrapperCode(InputWrapperPath, inputActions, options);
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        Debug.Log("PlayerInputControl.cs regenerated from PlayerInputControl.inputactions.");

        if (Application.isBatchMode)
        {
            EditorApplication.Exit(0);
        }
    }

    public static void ValidateProject()
    {
        var report = ValidateSetup(out var passed);
        File.WriteAllText(ResultPath, report);
        Debug.Log(report);

        if (!passed)
        {
            throw new InvalidOperationException("Simple Inventory validation failed. See the validation report for details.");
        }

        if (Application.isBatchMode)
        {
            EditorApplication.Exit(0);
        }
    }

    private static void FinishConfiguration()
    {
        var font = GetDefaultFont();
        if (font == null)
        {
            throw new InvalidOperationException("TMP Essentials were imported, but the default font asset was not found.");
        }

        var addItemEvent = GetOrCreateEventAsset();
        var ore = GetOrCreateItem(OreAssetPath, "Ore", new Color(0.45f, 0.48f, 0.52f, 1f));
        var wood = GetOrCreateItem(WoodAssetPath, "Wood", new Color(0.48f, 0.28f, 0.12f, 1f));
        var rowPrefab = GetOrCreateRowPrefab();

        ConfigurePersistentScene(addItemEvent, rowPrefab);
        ConfigureForestScene(addItemEvent, ore, wood);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        var report = ValidateSetup(out var passed);
        File.WriteAllText(ResultPath, report);
        Debug.Log(report);

        if (!passed)
        {
            throw new InvalidOperationException("Simple Inventory validation failed after setup.");
        }

        if (Application.isBatchMode)
        {
            EditorApplication.Exit(0);
        }
    }

    private static void EnsureFolders()
    {
        EnsureFolder("Assets/Data SO", "Inventory");
        EnsureFolder("Assets/Prefabs", "UI");
    }

    private static void EnsureFolder(string parent, string child)
    {
        var path = parent + "/" + child;
        if (!AssetDatabase.IsValidFolder(path))
        {
            AssetDatabase.CreateFolder(parent, child);
        }
    }

    private static bool EnsureTmpEssentials()
    {
        const string settingsPath = "Assets/TextMesh Pro/Resources/TMP Settings.asset";
        if (AssetDatabase.LoadAssetAtPath<TMP_Settings>(settingsPath) != null)
        {
            return true;
        }

        var packageDirectories = Directory.GetDirectories("Library/PackageCache", "com.unity.textmeshpro@*");
        if (packageDirectories.Length == 0)
        {
            throw new DirectoryNotFoundException("The TextMeshPro package cache directory was not found.");
        }

        var essentialsPackage = Path.GetFullPath(Path.Combine(packageDirectories[0], "Package Resources", "TMP Essential Resources.unitypackage"));
        AssetDatabase.importPackageCompleted += OnTmpImportCompleted;
        AssetDatabase.importPackageFailed += OnTmpImportFailed;
        AssetDatabase.ImportPackage(essentialsPackage, false);
        return false;
    }

    private static void OnTmpImportCompleted(string packageName)
    {
        AssetDatabase.importPackageCompleted -= OnTmpImportCompleted;
        AssetDatabase.importPackageFailed -= OnTmpImportFailed;
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        FinishConfiguration();
    }

    private static void OnTmpImportFailed(string packageName, string errorMessage)
    {
        AssetDatabase.importPackageCompleted -= OnTmpImportCompleted;
        AssetDatabase.importPackageFailed -= OnTmpImportFailed;
        File.WriteAllText(ResultPath, $"TMP import failed: {packageName}\n{errorMessage}");
        Debug.LogError($"TMP import failed: {packageName}\n{errorMessage}");
        if (Application.isBatchMode)
        {
            EditorApplication.Exit(1);
        }
    }

    private static TMP_FontAsset GetDefaultFont()
    {
        return AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset");
    }

    private static AddItemEventSO GetOrCreateEventAsset()
    {
        var asset = AssetDatabase.LoadAssetAtPath<AddItemEventSO>(EventAssetPath);
        if (asset == null)
        {
            asset = ScriptableObject.CreateInstance<AddItemEventSO>();
            AssetDatabase.CreateAsset(asset, EventAssetPath);
        }

        return asset;
    }

    private static ItemData GetOrCreateItem(string path, string displayName, Color displayColor)
    {
        var item = AssetDatabase.LoadAssetAtPath<ItemData>(path);
        if (item == null)
        {
            item = ScriptableObject.CreateInstance<ItemData>();
            AssetDatabase.CreateAsset(item, path);
        }

        var serializedItem = new SerializedObject(item);
        serializedItem.FindProperty("displayName").stringValue = displayName;
        serializedItem.FindProperty("displayColor").colorValue = displayColor;
        serializedItem.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(item);
        return item;
    }

    private static InventoryItemView GetOrCreateRowPrefab()
    {
        var existing = AssetDatabase.LoadAssetAtPath<InventoryItemView>(RowPrefabPath);
        if (existing != null)
        {
            return existing;
        }

        var root = new GameObject("InventoryItemRow", typeof(RectTransform), typeof(Image), typeof(LayoutElement), typeof(InventoryItemView));
        var rootRect = root.GetComponent<RectTransform>();
        rootRect.sizeDelta = new Vector2(600f, 80f);
        root.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.08f);
        root.GetComponent<LayoutElement>().preferredHeight = 80f;

        var swatch = CreateUiObject("Color", root.transform);
        var swatchRect = swatch.GetComponent<RectTransform>();
        swatchRect.anchorMin = new Vector2(0f, 0.5f);
        swatchRect.anchorMax = new Vector2(0f, 0.5f);
        swatchRect.pivot = new Vector2(0f, 0.5f);
        swatchRect.anchoredPosition = new Vector2(16f, 0f);
        swatchRect.sizeDelta = new Vector2(56f, 56f);
        var swatchImage = swatch.AddComponent<Image>();
        swatchImage.color = Color.gray;

        var label = CreateUiObject("Label", root.transform);
        var labelRect = label.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = new Vector2(96f, 8f);
        labelRect.offsetMax = new Vector2(-16f, -8f);
        var labelText = label.AddComponent<TextMeshProUGUI>();
        ConfigureText(labelText, "Item x 1", 32f, TextAlignmentOptions.MidlineLeft);

        var itemView = root.GetComponent<InventoryItemView>();
        var serializedView = new SerializedObject(itemView);
        serializedView.FindProperty("colorBlock").objectReferenceValue = swatchImage;
        serializedView.FindProperty("itemLabel").objectReferenceValue = labelText;
        serializedView.ApplyModifiedPropertiesWithoutUndo();

        var prefab = PrefabUtility.SaveAsPrefabAsset(root, RowPrefabPath);
        UnityEngine.Object.DestroyImmediate(root);
        return prefab.GetComponent<InventoryItemView>();
    }

    private static void ConfigurePersistentScene(AddItemEventSO addItemEvent, InventoryItemView rowPrefab)
    {
        var scene = EditorSceneManager.OpenScene(PersistentScenePath, OpenSceneMode.Single);
        var player = RequireSceneObject(scene, "Player");
        var canvas = RequireSceneObject(scene, "Canvas");
        var uiManagerObject = RequireSceneObject(scene, "UI Manager");

        var inventory = player.GetComponent<Inventory>();
        if (inventory == null)
        {
            inventory = player.AddComponent<Inventory>();
        }

        SetObjectReference(inventory, "addItemEvent", addItemEvent);

        var panel = FindDirectChild(canvas.transform, "InventoryPanel");
        if (panel != null)
        {
            UnityEngine.Object.DestroyImmediate(panel);
        }

        panel = CreateInventoryPanel(canvas.transform, out var contentRoot, out var emptyState);
        var inventoryUi = uiManagerObject.GetComponent<InventoryUI>();
        if (inventoryUi == null)
        {
            inventoryUi = uiManagerObject.AddComponent<InventoryUI>();
        }

        var serializedUi = new SerializedObject(inventoryUi);
        serializedUi.FindProperty("inventory").objectReferenceValue = inventory;
        serializedUi.FindProperty("panel").objectReferenceValue = panel;
        serializedUi.FindProperty("contentRoot").objectReferenceValue = contentRoot;
        serializedUi.FindProperty("itemViewPrefab").objectReferenceValue = rowPrefab;
        serializedUi.FindProperty("emptyState").objectReferenceValue = emptyState;
        serializedUi.ApplyModifiedPropertiesWithoutUndo();

        panel.SetActive(false);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
    }

    private static GameObject CreateInventoryPanel(Transform canvas, out Transform contentRoot, out GameObject emptyState)
    {
        var panel = CreateUiObject("InventoryPanel", canvas);
        var panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = panelRect.anchorMax = panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.anchoredPosition = Vector2.zero;
        panelRect.sizeDelta = new Vector2(720f, 820f);
        var panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0.04f, 0.05f, 0.08f, 0.94f);

        var title = CreateUiObject("Title", panel.transform);
        var titleRect = title.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0f, 1f);
        titleRect.anchorMax = Vector2.one;
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.anchoredPosition = Vector2.zero;
        titleRect.sizeDelta = new Vector2(0f, 90f);
        ConfigureText(title.AddComponent<TextMeshProUGUI>(), "Inventory", 48f, TextAlignmentOptions.Center);

        var scrollView = CreateUiObject("Scroll View", panel.transform);
        var scrollRectTransform = scrollView.GetComponent<RectTransform>();
        scrollRectTransform.anchorMin = Vector2.zero;
        scrollRectTransform.anchorMax = Vector2.one;
        scrollRectTransform.offsetMin = new Vector2(36f, 36f);
        scrollRectTransform.offsetMax = new Vector2(-36f, -104f);
        var scrollRect = scrollView.AddComponent<ScrollRect>();
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.movementType = ScrollRect.MovementType.Clamped;
        scrollRect.scrollSensitivity = 30f;

        var viewport = CreateUiObject("Viewport", scrollView.transform);
        var viewportRect = viewport.GetComponent<RectTransform>();
        Stretch(viewportRect);
        var viewportImage = viewport.AddComponent<Image>();
        viewportImage.color = new Color(1f, 1f, 1f, 0.04f);
        var mask = viewport.AddComponent<Mask>();
        mask.showMaskGraphic = true;

        var content = CreateUiObject("Content", viewport.transform);
        var contentRect = content.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0f, 1f);
        contentRect.anchorMax = Vector2.one;
        contentRect.pivot = new Vector2(0.5f, 1f);
        contentRect.anchoredPosition = Vector2.zero;
        contentRect.sizeDelta = Vector2.zero;
        var layout = content.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(16, 16, 16, 16);
        layout.spacing = 12f;
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;
        var fitter = content.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        emptyState = CreateUiObject("Empty", viewport.transform);
        Stretch(emptyState.GetComponent<RectTransform>());
        var emptyText = emptyState.AddComponent<TextMeshProUGUI>();
        ConfigureText(emptyText, "Empty", 40f, TextAlignmentOptions.Center);
        emptyText.color = new Color(1f, 1f, 1f, 0.62f);
        emptyText.raycastTarget = false;

        scrollRect.viewport = viewportRect;
        scrollRect.content = contentRect;
        contentRoot = content.transform;
        return panel;
    }

    private static void ConfigureForestScene(AddItemEventSO addItemEvent, ItemData ore, ItemData wood)
    {
        var scene = EditorSceneManager.OpenScene(ForestScenePath, OpenSceneMode.Single);
        var chest1 = RequireSceneObject(scene, "Chest1");
        var chest2 = FindSceneObject(scene, "Chest2") ?? DuplicateChest(chest1, "Chest2", 3f);
        var chest3 = FindSceneObject(scene, "Chest3") ?? DuplicateChest(chest1, "Chest3", 6f);

        ConfigureChest(chest1, ore, 3, addItemEvent);
        ConfigureChest(chest2, ore, 2, addItemEvent);
        ConfigureChest(chest3, wood, 1, addItemEvent);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
    }

    private static GameObject DuplicateChest(GameObject source, string name, float xOffset)
    {
        var duplicate = UnityEngine.Object.Instantiate(source, source.transform.parent);
        duplicate.name = name;
        duplicate.transform.position = source.transform.position + Vector3.right * xOffset;
        return duplicate;
    }

    private static void ConfigureChest(GameObject chestObject, ItemData reward, int amount, AddItemEventSO addItemEvent)
    {
        var chestComponent = chestObject.GetComponent<chest>();
        if (chestComponent == null)
        {
            throw new InvalidOperationException($"{chestObject.name} does not have the chest component.");
        }

        chestComponent.rewardItem = reward;
        chestComponent.rewardAmount = amount;
        chestComponent.addItemEvent = addItemEvent;
        chestComponent.isDone = false;
        chestObject.tag = "Interactable";
        var collider = chestObject.GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = true;
        }

        EditorUtility.SetDirty(chestComponent);
    }

    private static string ValidateSetup(out bool passed)
    {
        var report = new StringBuilder();
        var failures = 0;
        report.AppendLine("Simple Inventory validation");
        var addItemEvent = AssetDatabase.LoadAssetAtPath<AddItemEventSO>(EventAssetPath);
        var ore = AssetDatabase.LoadAssetAtPath<ItemData>(OreAssetPath);
        var wood = AssetDatabase.LoadAssetAtPath<ItemData>(WoodAssetPath);
        var rowPrefab = AssetDatabase.LoadAssetAtPath<InventoryItemView>(RowPrefabPath);
        AddCheck(report, "Event asset", addItemEvent != null, ref failures);
        AddCheck(report, "Ore asset", ore != null && ore.DisplayName == "Ore", ref failures);
        AddCheck(report, "Wood asset", wood != null && wood.DisplayName == "Wood", ref failures);
        AddCheck(report, "TMP default font", GetDefaultFont() != null, ref failures);
        AddCheck(report, "Row prefab", rowPrefab != null, ref failures);
        if (rowPrefab != null)
        {
            var serializedRow = new SerializedObject(rowPrefab);
            AddCheck(report, "Row color reference", serializedRow.FindProperty("colorBlock").objectReferenceValue != null, ref failures);
            AddCheck(report, "Row label reference", serializedRow.FindProperty("itemLabel").objectReferenceValue != null, ref failures);
        }

        var inputActions = AssetDatabase.LoadAssetAtPath<InputActionAsset>(InputActionsPath);
        var assetToggle = inputActions != null ? inputActions.FindAction("GamePlayer/ToggleInventory") : null;
        AddCheck(report, "Input action asset", assetToggle != null, ref failures);
        AddCheck(report, "I key binding", assetToggle != null && assetToggle.bindings.Any(binding => string.Equals(binding.path, "<Keyboard>/i", StringComparison.OrdinalIgnoreCase)), ref failures);
        using (var generatedInput = new PlayerInputControl())
        {
            AddCheck(report, "Generated input wrapper", generatedInput.asset.FindAction("GamePlayer/ToggleInventory") != null, ref failures);
        }

        var persistent = EditorSceneManager.OpenScene(PersistentScenePath, OpenSceneMode.Single);
        var player = RequireSceneObject(persistent, "Player");
        var inventory = player.GetComponent<Inventory>();
        var inventoryUi = RequireSceneObject(persistent, "UI Manager").GetComponent<InventoryUI>();
        var panel = FindSceneObject(persistent, "InventoryPanel");
        AddCheck(report, "Player Inventory", inventory != null, ref failures);
        if (inventory != null)
        {
            var serializedInventory = new SerializedObject(inventory);
            AddCheck(report, "Inventory event reference", serializedInventory.FindProperty("addItemEvent").objectReferenceValue == addItemEvent, ref failures);
        }

        AddCheck(report, "Inventory UI", inventoryUi != null, ref failures);
        if (inventoryUi != null)
        {
            var serializedUi = new SerializedObject(inventoryUi);
            AddCheck(report, "UI inventory reference", serializedUi.FindProperty("inventory").objectReferenceValue == inventory, ref failures);
            AddCheck(report, "UI panel reference", serializedUi.FindProperty("panel").objectReferenceValue == panel, ref failures);
            AddCheck(report, "UI content reference", serializedUi.FindProperty("contentRoot").objectReferenceValue != null, ref failures);
            AddCheck(report, "UI row prefab reference", serializedUi.FindProperty("itemViewPrefab").objectReferenceValue == rowPrefab, ref failures);
            AddCheck(report, "UI empty-state reference", serializedUi.FindProperty("emptyState").objectReferenceValue != null, ref failures);
            AddCheck(report, "UI runtime initialization", ValidateUiRuntimeInitialization(inventoryUi), ref failures);
        }

        AddCheck(report, "Inventory Panel", panel != null && !panel.activeSelf, ref failures);
        AddCheck(report, "Persistent missing scripts", CountMissingScripts(persistent) == 0, ref failures);

        var forest = EditorSceneManager.OpenScene(ForestScenePath, OpenSceneMode.Single);
        AddCheck(report, "Chest1 reward", IsChestConfigured(forest, "Chest1", ore, 3, addItemEvent), ref failures);
        AddCheck(report, "Chest2 reward", IsChestConfigured(forest, "Chest2", ore, 2, addItemEvent), ref failures);
        AddCheck(report, "Chest3 reward", IsChestConfigured(forest, "Chest3", wood, 1, addItemEvent), ref failures);
        AddCheck(report, "Chest runtime reward", ValidateChestRuntimeReward(forest, addItemEvent, ore), ref failures);
        AddCheck(report, "Chest open path is private", typeof(chest).GetMethod("OpenChest", BindingFlags.Instance | BindingFlags.Public) == null, ref failures);
        AddCheck(report, "Opened chest clears interaction prompt", ValidateOpenedChestClearsInteraction(forest, addItemEvent), ref failures);
        AddCheck(report, "Forest missing scripts", CountMissingScripts(forest) == 0, ref failures);
        passed = failures == 0;
        report.AppendLine(passed ? "Validation: PASS" : $"Validation: FAIL ({failures} checks)");
        return report.ToString();
    }

    private static bool IsChestConfigured(Scene scene, string chestName, ItemData reward, int amount, AddItemEventSO addItemEvent)
    {
        var chestObject = FindSceneObject(scene, chestName);
        var chestComponent = chestObject != null ? chestObject.GetComponent<chest>() : null;
        var collider = chestObject != null ? chestObject.GetComponent<Collider2D>() : null;
        return chestComponent != null
            && chestComponent.rewardItem == reward
            && chestComponent.rewardAmount == amount
            && chestComponent.addItemEvent == addItemEvent
            && !chestComponent.isDone
            && chestObject.CompareTag("Interactable")
            && collider != null
            && collider.enabled;
    }

    private static int CountMissingScripts(Scene scene)
    {
        var count = 0;
        foreach (var root in scene.GetRootGameObjects())
        {
            foreach (var transform in root.GetComponentsInChildren<Transform>(true))
            {
                count += GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(transform.gameObject);
            }
        }

        return count;
    }

    private static bool ValidateUiRuntimeInitialization(InventoryUI inventoryUi)
    {
        try
        {
            InvokeLifecycle(inventoryUi, "Awake");
            InvokeLifecycle(inventoryUi, "OnEnable");
            InvokeLifecycle(inventoryUi, "OnDisable");
            return true;
        }
        catch (Exception exception)
        {
            Debug.LogException(exception is TargetInvocationException invocation && invocation.InnerException != null
                ? invocation.InnerException
                : exception);
            return false;
        }
    }

    private static bool ValidateChestRuntimeReward(Scene scene, AddItemEventSO addItemEvent, ItemData expectedItem)
    {
        var source = FindSceneObject(scene, "Chest1");
        if (source == null)
        {
            return false;
        }

        var clone = UnityEngine.Object.Instantiate(source);
        clone.hideFlags = HideFlags.HideAndDontSave;
        var cloneChest = clone.GetComponent<chest>();
        var raisedCount = 0;
        ItemData receivedItem = null;
        var receivedAmount = 0;
        Action<ItemData, int> handler = (item, amount) =>
        {
            raisedCount++;
            receivedItem = item;
            receivedAmount = amount;
        };
        addItemEvent.OnEventRaised += handler;

        try
        {
            InvokeLifecycle(cloneChest, "Awake");
            cloneChest.TriggerAction();
            cloneChest.TriggerAction();
            var collider = clone.GetComponent<Collider2D>();
            return raisedCount == 1
                && receivedItem == expectedItem
                && receivedAmount == 3
                && cloneChest.isDone
                && collider != null
                && !collider.enabled;
        }
        catch (Exception exception)
        {
            Debug.LogException(exception is TargetInvocationException invocation && invocation.InnerException != null
                ? invocation.InnerException
                : exception);
            return false;
        }
        finally
        {
            addItemEvent.OnEventRaised -= handler;
            UnityEngine.Object.DestroyImmediate(clone);
        }
    }

    private static bool ValidateOpenedChestClearsInteraction(Scene scene, AddItemEventSO addItemEvent)
    {
        var source = FindSceneObject(scene, "Chest1");
        if (source == null)
        {
            return false;
        }

        var clone = UnityEngine.Object.Instantiate(source);
        clone.hideFlags = HideFlags.HideAndDontSave;
        var cloneChest = clone.GetComponent<chest>();
        var signObject = new GameObject("Inventory Interaction Test");
        signObject.hideFlags = HideFlags.HideAndDontSave;
        signObject.SetActive(false);
        var sign = signObject.AddComponent<Sign>();
        sign.canPress = true;
        sign.targetItem = cloneChest;
        var raisedCount = 0;
        Action<ItemData, int> handler = (item, amount) => raisedCount++;
        addItemEvent.OnEventRaised += handler;

        try
        {
            InvokeLifecycle(cloneChest, "Awake");
            var confirm = typeof(Sign).GetMethod("OnConfirm", BindingFlags.Instance | BindingFlags.NonPublic);
            if (confirm == null)
            {
                return false;
            }

            confirm.Invoke(sign, new object[] { default(InputAction.CallbackContext) });
            return raisedCount == 1 && !sign.canPress && sign.targetItem == null;
        }
        catch (Exception exception)
        {
            Debug.LogException(exception is TargetInvocationException invocation && invocation.InnerException != null
                ? invocation.InnerException
                : exception);
            return false;
        }
        finally
        {
            addItemEvent.OnEventRaised -= handler;
            UnityEngine.Object.DestroyImmediate(signObject);
            UnityEngine.Object.DestroyImmediate(clone);
        }
    }

    private static void InvokeLifecycle(UnityEngine.Object target, string methodName)
    {
        var method = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
        if (method == null)
        {
            throw new MissingMethodException(target.GetType().Name, methodName);
        }

        method.Invoke(target, null);
    }

    private static void AddCheck(StringBuilder report, string label, bool condition, ref int failures)
    {
        report.AppendLine($"{label}: {(condition ? "PASS" : "FAIL")}");
        if (!condition)
        {
            failures++;
        }
    }

    private static GameObject RequireSceneObject(Scene scene, string name)
    {
        var result = FindSceneObject(scene, name);
        if (result == null)
        {
            throw new InvalidOperationException($"Scene '{scene.path}' is missing GameObject '{name}'.");
        }

        return result;
    }

    private static GameObject FindSceneObject(Scene scene, string name)
    {
        foreach (var root in scene.GetRootGameObjects())
        {
            var found = FindInHierarchy(root.transform, name);
            if (found != null)
            {
                return found.gameObject;
            }
        }

        return null;
    }

    private static Transform FindInHierarchy(Transform current, string name)
    {
        if (current.name == name)
        {
            return current;
        }

        foreach (Transform child in current)
        {
            var found = FindInHierarchy(child, name);
            if (found != null)
            {
                return found;
            }
        }

        return null;
    }

    private static GameObject FindDirectChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
            {
                return child.gameObject;
            }
        }

        return null;
    }

    private static GameObject CreateUiObject(string name, Transform parent)
    {
        var gameObject = new GameObject(name, typeof(RectTransform));
        gameObject.layer = LayerMask.NameToLayer("UI");
        gameObject.transform.SetParent(parent, false);
        return gameObject;
    }

    private static void ConfigureText(TextMeshProUGUI text, string value, float fontSize, TextAlignmentOptions alignment)
    {
        text.font = GetDefaultFont();
        text.text = value;
        text.fontSize = fontSize;
        text.alignment = alignment;
        text.color = Color.white;
        text.raycastTarget = false;
    }

    private static void Stretch(RectTransform rectTransform)
    {
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }

    private static void SetObjectReference(UnityEngine.Object target, string propertyName, UnityEngine.Object value)
    {
        var serializedObject = new SerializedObject(target);
        var property = serializedObject.FindProperty(propertyName);
        if (property == null)
        {
            throw new InvalidOperationException($"Serialized property '{propertyName}' was not found on {target.GetType().Name}.");
        }

        property.objectReferenceValue = value;
        serializedObject.ApplyModifiedPropertiesWithoutUndo();
    }
}
