using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LeaderboardUI : MonoBehaviour
{
    public GameObject leaderboardCanvas;
    public GameObject recordRowPrefab;
    public Transform contentParent;

    void OnEnable()
    {
        PopulateLeaderboard();
    }

    public void PopulateLeaderboard()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        if (StreakManager.Instance == null) return;

        StreakData data = StreakManager.Instance.LoadStreaks();
        List<StreakEntry> entries = data.entries;

        for (int i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];
            GameObject row = Instantiate(recordRowPrefab, contentParent);

            // 🔧 Ищем TMP_Text внутри дочерних объектов, а не на корневом объекте
            TMP_Text text = row.GetComponentInChildren<TMP_Text>();
            if (text != null)
                text.text = $"{i + 1}. {entry.playerName} — {entry.streak}";
            else
                Debug.LogWarning("TMP_Text не найден в префабе строки таблицы.");
        }
    }

    public void CloseLeaderboard()
    {
        leaderboardCanvas.SetActive(false);
    }
}
