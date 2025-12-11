using UnityEngine;
using UnityEditor;
using RGame;

/// <summary>
/// Tool tự động điều chỉnh độ khó của tất cả levels theo công thức tăng dần
/// </summary>
public class LevelDifficultyEditor : EditorWindow
{
    private GameLevelSetup levelSetup;

    [Header("=== CÀI ĐẶT ĐỘ KHÓ ===")]
    private int baseMana = 1000;
    private int manaIncreasePerLevel = 50;
    private int manaIncreasePerWorld = 200;

    private int baseFortressLevel = 1;
    private int levelsPerFortressIncrease = 12;

    private float baseEnemyMultiplier = 1.0f;
    private float enemyMultiplierPerLevel = 0.05f;
    private float enemyMultiplierPerWorld = 0.3f;

    [MenuItem("Game Tools/Level Difficulty Editor")]
    public static void ShowWindow()
    {
        GetWindow<LevelDifficultyEditor>("Độ Khó Level");
    }

    void OnGUI()
    {
        GUILayout.Label("CÔNG CỤ ĐIỀU CHỈNH ĐỘ KHÓ", EditorStyles.boldLabel);
        GUILayout.Space(10);

        // Tìm GameLevelSetup
        if (levelSetup == null)
        {
            levelSetup = FindObjectOfType<GameLevelSetup>();
        }

        if (levelSetup == null)
        {
            EditorGUILayout.HelpBox("⚠️ Không tìm thấy GameLevelSetup!\n\nHãy mở scene 'Playing' trước.", MessageType.Warning);
            if (GUILayout.Button("Mở Scene Playing"))
            {
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(
                    "Assets/_MonstersOut/Scene/Other/Playing.unity"
                );
            }
            return;
        }

        EditorGUILayout.HelpBox($"✅ Tìm thấy {levelSetup.levelWaves.Count} levels", MessageType.Info);
        GUILayout.Space(10);

        // Cài đặt Mana
        GUILayout.Label("⚡ CÀI ĐẶT MANA", EditorStyles.boldLabel);
        baseMana = EditorGUILayout.IntField("Mana Level 1:", baseMana);
        manaIncreasePerLevel = EditorGUILayout.IntField("Tăng Mana mỗi Level:", manaIncreasePerLevel);
        manaIncreasePerWorld = EditorGUILayout.IntField("Tăng Mana mỗi World:", manaIncreasePerWorld);
        GUILayout.Space(10);

        // Cài đặt Fortress
        GUILayout.Label("🏰 CÀI ĐẶT PHÁO ĐÀI", EditorStyles.boldLabel);
        baseFortressLevel = EditorGUILayout.IntSlider("Cấp Pháo Đài Level 1:", baseFortressLevel, 1, 5);
        levelsPerFortressIncrease = EditorGUILayout.IntField("Levels để tăng 1 cấp:", levelsPerFortressIncrease);
        GUILayout.Space(10);

        // Cài đặt Enemy
        GUILayout.Label("👾 CÀI ĐẶT QUÁI", EditorStyles.boldLabel);
        baseEnemyMultiplier = EditorGUILayout.Slider("Hệ số quái cơ bản:", baseEnemyMultiplier, 0.5f, 2f);
        enemyMultiplierPerLevel = EditorGUILayout.Slider("Tăng mỗi Level:", enemyMultiplierPerLevel, 0f, 0.2f);
        enemyMultiplierPerWorld = EditorGUILayout.Slider("Tăng mỗi World:", enemyMultiplierPerWorld, 0f, 1f);
        GUILayout.Space(20);

        // Buttons
        if (GUILayout.Button("🎯 ÁP DỤNG ĐỘ KHÓ TĂNG DẦN", GUILayout.Height(40)))
        {
            ApplyProgressiveDifficulty();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("📊 Xem Preview Độ Khó"))
        {
            PreviewDifficulty();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("↩️ Reset về Mặc Định"))
        {
            ResetToDefault();
        }
    }

    void ApplyProgressiveDifficulty()
    {
        if (!EditorUtility.DisplayDialog(
            "Xác nhận",
            $"Bạn có chắc muốn áp dụng độ khó tăng dần cho {levelSetup.levelWaves.Count} levels?\n\n" +
            "Thao tác này sẽ thay đổi:\n" +
            "- Mana ban đầu\n" +
            "- Cấp độ pháo đài địch\n" +
            "- Số lượng quái\n\n" +
            "Có thể Undo sau khi áp dụng (Ctrl+Z)",
            "Áp Dụng",
            "Hủy"))
        {
            return;
        }

        Undo.RecordObject(levelSetup, "Apply Progressive Difficulty");

        int totalLevels = levelSetup.levelWaves.Count;
        int levelsPerWorld = 12; // Giả sử mỗi world có 12 levels

        for (int i = 0; i < totalLevels; i++)
        {
            LevelWave level = levelSetup.levelWaves[i];
            int levelNumber = i + 1;
            int worldNumber = (levelNumber - 1) / levelsPerWorld + 1;
            int levelInWorld = (levelNumber - 1) % levelsPerWorld + 1;

            // Ghi lại thay đổi
            Undo.RecordObject(level, "Modify Level " + levelNumber);

            // Tính toán Mana
            int calculatedMana = baseMana
                + (levelInWorld - 1) * manaIncreasePerLevel
                + (worldNumber - 1) * manaIncreasePerWorld;
            level.givenMana = calculatedMana;

            // Tính toán Fortress Level (1-5)
            int fortressLevel = baseFortressLevel + (levelNumber - 1) / levelsPerFortressIncrease;
            level.enemyFortrestLevel = Mathf.Clamp(fortressLevel, 1, 5);

            // Tính toán số lượng quái
            float enemyMultiplier = baseEnemyMultiplier
                + (levelInWorld - 1) * enemyMultiplierPerLevel
                + (worldNumber - 1) * enemyMultiplierPerWorld;

            // Áp dụng multiplier cho tất cả waves
            if (level.Waves != null)
            {
                foreach (var wave in level.Waves)
                {
                    if (wave.enemySpawns != null)
                    {
                        foreach (var spawn in wave.enemySpawns)
                        {
                            // Lưu số lượng gốc (lần đầu tiên)
                            if (spawn.numberEnemy < 3)
                                spawn.numberEnemy = 5; // Base number

                            int newNumber = Mathf.RoundToInt(spawn.numberEnemy * enemyMultiplier);
                            spawn.numberEnemy = Mathf.Max(1, newNumber);
                        }
                    }
                }
            }

            EditorUtility.SetDirty(level);
        }

        EditorUtility.SetDirty(levelSetup);

        Debug.Log($"✅ Đã áp dụng độ khó tăng dần cho {totalLevels} levels!");
        Debug.Log($"📊 World 1 Level 1: Mana={baseMana}, Fortress={baseFortressLevel}");
        Debug.Log($"📊 World {(totalLevels/levelsPerWorld)} Level {levelsPerWorld}: " +
            $"Mana={levelSetup.levelWaves[totalLevels-1].givenMana}, " +
            $"Fortress={levelSetup.levelWaves[totalLevels-1].enemyFortrestLevel}");

        EditorUtility.DisplayDialog(
            "Thành công!",
            $"✅ Đã áp dụng độ khó tăng dần cho {totalLevels} levels!\n\n" +
            "Kiểm tra Console để xem chi tiết.\n" +
            "Nhấn Ctrl+S để lưu scene.",
            "OK"
        );
    }

    void PreviewDifficulty()
    {
        Debug.Log("========== PREVIEW ĐỘ KHÓ ==========");

        int totalLevels = Mathf.Min(levelSetup.levelWaves.Count, 60); // Preview 60 levels đầu
        int levelsPerWorld = 12;

        for (int i = 0; i < totalLevels; i++)
        {
            int levelNumber = i + 1;
            int worldNumber = (levelNumber - 1) / levelsPerWorld + 1;
            int levelInWorld = (levelNumber - 1) % levelsPerWorld + 1;

            // Tính toán theo công thức
            int mana = baseMana
                + (levelInWorld - 1) * manaIncreasePerLevel
                + (worldNumber - 1) * manaIncreasePerWorld;

            int fortress = baseFortressLevel + (levelNumber - 1) / levelsPerFortressIncrease;
            fortress = Mathf.Clamp(fortress, 1, 5);

            float enemyMult = baseEnemyMultiplier
                + (levelInWorld - 1) * enemyMultiplierPerLevel
                + (worldNumber - 1) * enemyMultiplierPerWorld;

            // Log mỗi level đầu tiên của mỗi world
            if (levelInWorld == 1 || levelInWorld == 6 || levelInWorld == 12 || levelNumber <= 3)
            {
                Debug.Log($"World {worldNumber} - Level {levelNumber}: " +
                    $"Mana={mana}, Fortress={fortress}, EnemyX={enemyMult:F2}");
            }
        }

        Debug.Log("====================================");
    }

    void ResetToDefault()
    {
        baseMana = 1000;
        manaIncreasePerLevel = 50;
        manaIncreasePerWorld = 200;
        baseFortressLevel = 1;
        levelsPerFortressIncrease = 12;
        baseEnemyMultiplier = 1.0f;
        enemyMultiplierPerLevel = 0.05f;
        enemyMultiplierPerWorld = 0.3f;

        Debug.Log("✅ Đã reset cài đặt về mặc định");
    }
}
