using UnityEngine;

/// <summary>
/// Helper script để reset PlayerPrefs nhanh
/// Attach vào bất kỳ GameObject nào, sau đó right-click vào script trong Inspector
/// và chọn "Reset All PlayerPrefs"
/// </summary>
public class ResetDataHelper : MonoBehaviour
{
    [ContextMenu("Reset All PlayerPrefs")]
    void ResetAllData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("✅ ĐÃ XÓA TOÀN BỘ DỮ LIỆU GAME!");
        Debug.Log("Reload scene để áp dụng dữ liệu mới...");

        // Reload scene hiện tại
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }

    [ContextMenu("Print Current Data")]
    void PrintData()
    {
        Debug.Log("=== DỮ LIỆU HIỆN TẠI ===");
        Debug.Log($"Coins: {PlayerPrefs.GetInt("Coins", -1)}");
        Debug.Log($"LevelReached: {PlayerPrefs.GetInt("LevelReached", -1)}");
        Debug.Log("(-1 nghĩa là chưa có dữ liệu)");
    }
}
