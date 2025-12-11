using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace RGame
{
    [CustomEditor(typeof(GameMode))]
    public class GameModeEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("RESET ALL"))
            {
                PlayerPrefs.DeleteAll();
                Debug.Log("RESET ALL!");
            }


            if (GUILayout.Button("UNLOCK ALL"))
            {
                GlobalValue.LevelPass = 1000;
                GlobalValue.SavedCoins = 99999;
                Debug.Log("UNLOCKED ALL!");
            }
        }
    }

    // Thêm menu nhanh để reset data
    public class QuickDataTools
    {
        [MenuItem("Game Tools/Reset All Data")]
        public static void ResetData()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log("✅ ĐÃ XÓA TOÀN BỘ DỮ LIỆU! Chạy game để thấy kết quả.");
        }

        [MenuItem("Game Tools/Show Current Data")]
        public static void ShowData()
        {
            Debug.Log("=== DỮ LIỆU HIỆN TẠI ===");
            Debug.Log("Coins: " + PlayerPrefs.GetInt("Coins", -1) + " (-1 = chưa có data)");
            Debug.Log("LevelReached: " + PlayerPrefs.GetInt("LevelReached", -1));
        }

        [MenuItem("Game Tools/Apply Progressive Difficulty (Auto)")]
        public static void ApplyProgressiveDifficultyAuto()
        {
            // Tìm GameLevelSetup trong scene hiện tại
            GameLevelSetup levelSetup = GameObject.FindObjectOfType<GameLevelSetup>();

            if (levelSetup == null)
            {
                // Thử tìm trong tất cả các scene
                var allScenes = new string[] {
                    "Assets/_MonstersOut/Scene/Other/Playing.unity",
                    "Assets/_MonstersOut/Scene/Init Scene.unity"
                };

                foreach (var scenePath in allScenes)
                {
                    if (System.IO.File.Exists(scenePath))
                    {
                        UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scenePath);
                        levelSetup = GameObject.FindObjectOfType<GameLevelSetup>();
                        if (levelSetup != null)
                        {
                            Debug.Log($"✅ Tìm thấy GameLevelSetup trong scene: {scenePath}");
                            break;
                        }
                    }
                }
            }

            if (levelSetup == null)
            {
                // Tìm trong Hierarchy của scene hiện tại
                GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
                foreach (var obj in allObjects)
                {
                    if (obj.name.Contains("Level") || obj.name.Contains("GameLevelSetup"))
                    {
                        Debug.Log($"Found object: {obj.name}");
                    }
                }

                Debug.LogError("❌ Không tìm thấy GameLevelSetup trong bất kỳ scene nào!");
                UnityEditor.EditorUtility.DisplayDialog(
                    "Lỗi",
                    "Không tìm thấy GameLevelSetup!\n\n" +
                    "GameLevelSetup cần phải có trong scene để điều chỉnh độ khó.\n\n" +
                    "Hướng dẫn:\n" +
                    "1. Mở scene Playing hoặc Init Scene\n" +
                    "2. Tìm GameObject có component GameLevelSetup\n" +
                    "3. Chạy lại tool này",
                    "OK"
                );
                return;
            }

            if (!UnityEditor.EditorUtility.DisplayDialog(
                "Áp Dụng Độ Khó Tăng Dần (CÂN BẰNG)",
                $"Sẽ áp dụng độ khó tăng dần CHO {levelSetup.levelWaves.Count} levels:\n\n" +
                "⚡ MANA (Chơi ngay, không chờ!):\n" +
                "  - Mana BAN ĐẦU: 500 (cho sẵn)\n" +
                "  - Level 1: 700 mana NGAY KHI BẮT ĐẦU\n" +
                "  - Tăng 20 mỗi level\n" +
                "  - Tăng 100 mỗi world\n" +
                "  - Tốc độ thu: 3 mana/1.5s (nhanh x2)\n" +
                "  → Không phải chờ, chơi luôn!\n\n" +
                "🏰 PHÁO ĐÀI (Tăng chậm):\n" +
                "  - Bắt đầu: Cấp 1\n" +
                "  - Tăng cấp sau 15 levels\n\n" +
                "👾 QUÁI (Tăng dần cân bằng):\n" +
                "  - Bắt đầu: Số lượng gốc (100%)\n" +
                "  - Tăng 4% mỗi level\n" +
                "  - Tăng 25% mỗi world\n" +
                "  - Luôn có ít nhất 2 quái\n\n" +
                "✅ Vào là chơi được ngay!\n\n" +
                "Bạn có chắc chắn muốn áp dụng?",
                "Áp Dụng",
                "Hủy"))
            {
                return;
            }

            // Cài đặt mặc định - ĐÃ CÂN BẰNG LẠI + MANA BAN ĐẦU CAO
            int baseMana = 200;                      // Mana ban đầu CỰC THẤP (để tăng dần)
            int manaIncreasePerLevel = 20;           // Tăng ít mỗi level
            int manaIncreasePerWorld = 100;          // Tăng ít mỗi world
            int initialGivenMana = 500;              // Mana cho SẴN khi vào level (QUAN TRỌNG!)
            int baseFortressLevel = 1;
            int levelsPerFortressIncrease = 15;      // Tăng chậm hơn (15 levels thay vì 12)
            float baseEnemyMultiplier = 1.0f;        // BẮT ĐẦU TỪ 100% (không giảm)
            float enemyMultiplierPerLevel = 0.04f;   // Tăng 4% mỗi level
            float enemyMultiplierPerWorld = 0.25f;   // Tăng 25% mỗi world

            UnityEditor.Undo.RecordObject(levelSetup, "Apply Progressive Difficulty");

            // Tăng tốc độ thu mana để không phải chờ lâu
            levelSetup.amountMana = 3;  // Tăng từ 2 lên 3 mana mỗi lần
            levelSetup.rate = 1.5f;     // Giảm thời gian chờ từ 2s xuống 1.5s
            UnityEditor.EditorUtility.SetDirty(levelSetup);

            int totalLevels = levelSetup.levelWaves.Count;
            int levelsPerWorld = 12;

            for (int i = 0; i < totalLevels; i++)
            {
                LevelWave level = levelSetup.levelWaves[i];
                int levelNumber = i + 1;
                int worldNumber = (levelNumber - 1) / levelsPerWorld + 1;
                int levelInWorld = (levelNumber - 1) % levelsPerWorld + 1;

                UnityEditor.Undo.RecordObject(level, "Modify Level " + levelNumber);

                // Tính Mana - Cho SẴN khi bắt đầu level
                int calculatedMana = initialGivenMana  // Mana cho sẵn ngay từ đầu
                    + baseMana
                    + (levelInWorld - 1) * manaIncreasePerLevel
                    + (worldNumber - 1) * manaIncreasePerWorld;
                level.givenMana = calculatedMana;

                // Tính Fortress Level
                int fortressLevel = baseFortressLevel + (levelNumber - 1) / levelsPerFortressIncrease;
                level.enemyFortrestLevel = Mathf.Clamp(fortressLevel, 1, 5);

                // Tính số lượng quái
                float enemyMultiplier = baseEnemyMultiplier
                    + (levelInWorld - 1) * enemyMultiplierPerLevel
                    + (worldNumber - 1) * enemyMultiplierPerWorld;

                // Áp dụng cho tất cả waves
                if (level.Waves != null)
                {
                    foreach (var wave in level.Waves)
                    {
                        if (wave.enemySpawns != null)
                        {
                            foreach (var spawn in wave.enemySpawns)
                            {
                                // Lưu số lượng gốc (base number)
                                int baseNumber = spawn.numberEnemy;

                                // Nếu quá ít, đặt số lượng cơ bản
                                if (baseNumber < 3)
                                    baseNumber = 5;

                                // Áp dụng multiplier
                                int newNumber = Mathf.RoundToInt(baseNumber * enemyMultiplier);

                                // ĐẢM BẢO LUÔN CÓ ÍT NHẤT 2 QUÁI
                                spawn.numberEnemy = Mathf.Max(2, newNumber);

                                // Debug cho level 100
                                if (levelNumber == 100)
                                {
                                    Debug.Log($"Level 100 - Enemy: {spawn.enemy?.name ?? "null"}, Base: {baseNumber}, Multiplier: {enemyMultiplier:F2}, Final: {spawn.numberEnemy}");
                                }
                            }
                        }
                    }
                }

                UnityEditor.EditorUtility.SetDirty(level);
            }

            UnityEditor.EditorUtility.SetDirty(levelSetup);

            Debug.Log("========== ĐỘ KHÓ ĐÃ ÁP DỤNG (CÂN BẰNG) ==========");
            Debug.Log($"✅ Đã áp dụng độ khó tăng dần cho {totalLevels} levels!");
            Debug.Log($"⚡ Tốc độ thu mana: {levelSetup.amountMana} mana mỗi {levelSetup.rate}s");
            Debug.Log($"\n📊 Level 1: Mana={levelSetup.levelWaves[0].givenMana}, Fortress={levelSetup.levelWaves[0].enemyFortrestLevel}");
            if (totalLevels > 11)
                Debug.Log($"📊 Level 12: Mana={levelSetup.levelWaves[11].givenMana}, Fortress={levelSetup.levelWaves[11].enemyFortrestLevel}");
            if (totalLevels > 23)
                Debug.Log($"📊 Level 24: Mana={levelSetup.levelWaves[23].givenMana}, Fortress={levelSetup.levelWaves[23].enemyFortrestLevel}");
            if (totalLevels > 49)
                Debug.Log($"📊 Level 50: Mana={levelSetup.levelWaves[49].givenMana}, Fortress={levelSetup.levelWaves[49].enemyFortrestLevel}");
            if (totalLevels > 99)
                Debug.Log($"📊 Level 100: Mana={levelSetup.levelWaves[99].givenMana}, Fortress={levelSetup.levelWaves[99].enemyFortrestLevel}");
            Debug.Log("==================================================");

            UnityEditor.EditorUtility.DisplayDialog(
                "Thành Công!",
                $"✅ Đã áp dụng độ khó CÂN BẰNG cho {totalLevels} levels!\n\n" +
                "🎮 Các thay đổi:\n" +
                "• Mana ban đầu: 500 (vào game có sẵn!)\n" +
                "• Tốc độ thu mana: 3 mana/1.5s (nhanh x2)\n" +
                "• Số quái tăng dần cân bằng (có ít nhất 2)\n" +
                "• Độ khó tăng từ từ\n\n" +
                "→ VÀO GAME LÀ CHƠI ĐƯỢC NGAY!\n" +
                "→ Không phải chờ thu mana nữa!\n\n" +
                "Nhấn Ctrl+S để lưu scene.\n" +
                "Xem Console để biết chi tiết.",
                "OK"
            );

            // Tự động lưu scene
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene()
            );
        }
    }
}