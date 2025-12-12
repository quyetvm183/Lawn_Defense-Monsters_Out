using UnityEngine;
using UnityEditor;

/// <summary>
/// Game Development Tools - Công cụ hỗ trợ test và debug game
/// Truy cập: Unity Menu → Game Tools → Dev Tools
///
/// ⚠️ LƯU Ý: File này nằm trong folder Editor nên:
/// - CHỈ hoạt động trong Unity Editor
/// - KHÔNG được include vào game build (.exe, .apk)
/// - An toàn 100% khi release game
/// </summary>
public class GameDevTools : EditorWindow
{
    #region Constants
    private const int MAX_LEVEL = 1000;
    private const int MAX_COINS = 999999;
    private const string LEVEL_PREFS_KEY = "LevelReached";
    private const string COINS_PREFS_KEY = "Coins";
    private const string MENU_SCENE_PATH = "Assets/_MonstersOut/Scene/Other/Menu.unity";
    private const string PLAYING_SCENE_PATH = "Assets/_MonstersOut/Scene/Other/Playing.unity";
    #endregion

    #region Private Fields
    private int coinsToAdd = 10000;
    private int levelToUnlock = 100;
    #endregion

    #region Unity Editor Menu

    [MenuItem("Game Tools/Dev Tools")]
    public static void ShowWindow()
    {
        GameDevTools window = GetWindow<GameDevTools>("Game Dev Tools");
        window.minSize = new Vector2(400, 600);
    }
    #endregion

    #region GUI Drawing

    void OnGUI()
    {
        GUILayout.Label("🛠️ GAME DEVELOPMENT TOOLS", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Công cụ hỗ trợ test và debug game nhanh chóng", MessageType.Info);

        GUILayout.Space(10);

        DrawLevelManagement();
        GUILayout.Space(10);

        DrawCoinsManagement();
        GUILayout.Space(10);

        DrawCharacterUnlock();
        GUILayout.Space(10);

        DrawDataManagement();
        GUILayout.Space(10);

        DrawInfoDisplay();
        GUILayout.Space(10);

        DrawSceneTools();
        GUILayout.Space(10);

        DrawFooter();
    }

    /// <summary>
    /// Vẽ section quản lý level
    /// </summary>
    private void DrawLevelManagement()
    {
        DrawSectionHeader("🎮 QUẢN LÝ LEVEL");

        EditorGUILayout.BeginHorizontal();
        levelToUnlock = EditorGUILayout.IntField("Level muốn mở:", Mathf.Max(1, levelToUnlock));
        if (GUILayout.Button("Unlock", GUILayout.Width(100)))
        {
            UnlockToLevel(levelToUnlock);
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("🔓 UNLOCK TẤT CẢ LEVELS", GUILayout.Height(40)))
        {
            UnlockAllLevels();
        }

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        DrawQuickLevelButton(10);
        DrawQuickLevelButton(20);
        DrawQuickLevelButton(50);
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// Vẽ section quản lý coins
    /// </summary>
    private void DrawCoinsManagement()
    {
        DrawSectionHeader("💰 QUẢN LÝ COINS");

        EditorGUILayout.BeginHorizontal();
        coinsToAdd = EditorGUILayout.IntField("Số coins muốn thêm:", Mathf.Max(0, coinsToAdd));
        if (GUILayout.Button("Add Coins", GUILayout.Width(100)))
        {
            AddCoins(coinsToAdd);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        DrawQuickCoinsButton(1000);
        DrawQuickCoinsButton(10000);
        DrawQuickCoinsButton(99999);
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button($"💰 SET {MAX_COINS:N0} COINS", GUILayout.Height(40)))
        {
            SetCoins(MAX_COINS);
        }
    }

    /// <summary>
    /// Vẽ section mở khóa nhân vật
    /// </summary>
    private void DrawCharacterUnlock()
    {
        DrawSectionHeader("🦸 MỞ KHÓA NHÂN VẬT");

        if (GUILayout.Button("🔓 Unlock Tất Cả Nhân Vật", GUILayout.Height(35)))
        {
            UnlockAllCharacters();
        }
    }

    /// <summary>
    /// Vẽ section quản lý dữ liệu
    /// </summary>
    private void DrawDataManagement()
    {
        DrawSectionHeader("💾 QUẢN LÝ DỮ LIỆU");

        EditorGUILayout.HelpBox("⚠️ Cẩn thận! Thao tác này không thể hoàn tác", MessageType.Warning);

        if (GUILayout.Button("🔄 RESET TẤT CẢ DỮ LIỆU", GUILayout.Height(40)))
        {
            if (EditorUtility.DisplayDialog("Xác nhận Reset",
                "Bạn có chắc muốn XÓA TẤT CẢ dữ liệu game?\n\n" +
                "Thao tác này sẽ reset:\n" +
                "- Level progress\n" +
                "- Coins\n" +
                "- Nhân vật đã unlock\n" +
                "- Upgrades\n\n" +
                "KHÔNG THỂ HOÀN TÁC!",
                "XÓA TẤT CẢ",
                "Hủy"))
            {
                ResetAllData();
            }
        }
    }

    /// <summary>
    /// Vẽ section hiển thị thông tin
    /// </summary>
    private void DrawInfoDisplay()
    {
        DrawSectionHeader("📊 THÔNG TIN HIỆN TẠI");
        ShowCurrentData();
    }

    /// <summary>
    /// Vẽ section công cụ scene
    /// </summary>
    private void DrawSceneTools()
    {
        DrawSectionHeader("🎬 CÔNG CỤ SCENE");

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Mở Menu Scene"))
        {
            OpenScene(MENU_SCENE_PATH);
        }
        if (GUILayout.Button("Mở Playing Scene"))
        {
            OpenScene(PLAYING_SCENE_PATH);
        }
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// Vẽ footer với tips
    /// </summary>
    private void DrawFooter()
    {
        EditorGUILayout.HelpBox(
            "💡 Tip: Sau khi thay đổi dữ liệu, reload scene để cập nhật UI\n" +
            "Nhấn Ctrl+R hoặc Play rồi Stop",
            MessageType.Info);
    }

    /// <summary>
    /// Vẽ button unlock level nhanh
    /// </summary>
    private void DrawQuickLevelButton(int level)
    {
        if (GUILayout.Button($"Unlock Level {level}"))
        {
            UnlockToLevel(level);
        }
    }

    /// <summary>
    /// Vẽ button add coins nhanh
    /// </summary>
    private void DrawQuickCoinsButton(int amount)
    {
        if (GUILayout.Button($"+ {amount:N0}"))
        {
            AddCoins(amount);
        }
    }
    #endregion

    #region Helper Methods

    /// <summary>
    /// Vẽ tiêu đề section
    /// </summary>
    private void DrawSectionHeader(string title)
    {
        GUILayout.Space(5);
        GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 12
        };
        headerStyle.normal.textColor = new Color(0.3f, 0.7f, 1f);
        GUILayout.Label(title, headerStyle);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
    }
    #endregion

    #region Level Management

    /// <summary>
    /// Unlock tất cả levels
    /// </summary>
    private void UnlockAllLevels()
    {
        PlayerPrefs.SetInt(LEVEL_PREFS_KEY, MAX_LEVEL);
        PlayerPrefs.Save();
        Debug.Log("✅ Đã unlock TẤT CẢ levels!");
        ShowSuccessDialog("✅ Đã unlock tất cả levels!\n\nReload scene để cập nhật.");
    }

    /// <summary>
    /// Unlock đến level cụ thể
    /// </summary>
    private void UnlockToLevel(int level)
    {
        if (level < 1)
        {
            Debug.LogWarning("Level phải >= 1");
            return;
        }

        PlayerPrefs.SetInt(LEVEL_PREFS_KEY, level);
        PlayerPrefs.Save();
        Debug.Log($"✅ Đã unlock đến Level {level}!");
        ShowSuccessDialog($"✅ Đã unlock đến Level {level}!\n\nReload scene để cập nhật.");
    }
    #endregion

    #region Coins Management

    /// <summary>
    /// Thêm coins
    /// </summary>
    private void AddCoins(int amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning("Số coins phải >= 0");
            return;
        }

        int currentCoins = PlayerPrefs.GetInt(COINS_PREFS_KEY, 0);
        int newCoins = currentCoins + amount;
        PlayerPrefs.SetInt(COINS_PREFS_KEY, newCoins);
        PlayerPrefs.Save();
        Debug.Log($"✅ Đã thêm {amount:N0} coins! Total: {newCoins:N0}");
        ShowSuccessDialog($"✅ Đã thêm {amount:N0} coins!\n\nTổng coins: {newCoins:N0}");
    }

    /// <summary>
    /// Set coins về giá trị cụ thể
    /// </summary>
    private void SetCoins(int amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning("Số coins phải >= 0");
            return;
        }

        PlayerPrefs.SetInt(COINS_PREFS_KEY, amount);
        PlayerPrefs.Save();
        Debug.Log($"✅ Đã set coins = {amount:N0}!");
        ShowSuccessDialog($"✅ Coins hiện tại: {amount:N0}");
    }
    #endregion

    #region Character Management

    /// <summary>
    /// Unlock tất cả nhân vật (bằng cách unlock max level)
    /// </summary>
    private void UnlockAllCharacters()
    {
        PlayerPrefs.SetInt(LEVEL_PREFS_KEY, MAX_LEVEL);
        PlayerPrefs.Save();
        Debug.Log("✅ Đã unlock tất cả nhân vật!");
        ShowSuccessDialog(
            "✅ Đã unlock tất cả nhân vật!\n\n" +
            $"(Unlock level đến {MAX_LEVEL}, tất cả nhân vật sẽ khả dụng)\n\n" +
            "Reload scene để cập nhật.");
    }
    #endregion

    #region Data Management

    /// <summary>
    /// Reset toàn bộ dữ liệu game
    /// </summary>
    private void ResetAllData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("✅ Đã reset TẤT CẢ dữ liệu!");
        ShowSuccessDialog(
            "✅ Đã xóa tất cả dữ liệu!\n\n" +
            "Game sẽ bắt đầu từ đầu.\n\n" +
            "Reload scene để cập nhật.");
    }

    /// <summary>
    /// Hiển thị dữ liệu hiện tại
    /// </summary>
    private void ShowCurrentData()
    {
        EditorGUILayout.BeginVertical("box");

        int currentLevel = PlayerPrefs.GetInt(LEVEL_PREFS_KEY, 0);
        int currentCoins = PlayerPrefs.GetInt(COINS_PREFS_KEY, 0);

        EditorGUILayout.LabelField("Level đã pass:", currentLevel.ToString("N0"), EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Coins hiện tại:", currentCoins.ToString("N0"), EditorStyles.boldLabel);

        EditorGUILayout.EndVertical();

        if (GUILayout.Button("🔄 Refresh Data"))
        {
            Repaint();
        }
    }
    #endregion

    #region Scene Management

    /// <summary>
    /// Mở scene với xác nhận
    /// </summary>
    private void OpenScene(string scenePath)
    {
        if (string.IsNullOrEmpty(scenePath))
        {
            Debug.LogError("Scene path is null or empty!");
            return;
        }

        if (EditorUtility.DisplayDialog("Mở Scene",
            $"Bạn có muốn mở scene:\n{scenePath}?",
            "Mở",
            "Hủy"))
        {
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scenePath);
        }
    }
    #endregion

    #region Utility Methods
    /// <summary>
    /// Hiển thị dialog thành công
    /// </summary>
    private void ShowSuccessDialog(string message)
    {
        EditorUtility.DisplayDialog("Thành công", message, "OK");
    }
    #endregion
}
