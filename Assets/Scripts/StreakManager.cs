using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

[System.Serializable]
public class StreakEntry { public string playerName; public int streak; }
[System.Serializable]
public class StreakData { public List<StreakEntry> entries = new List<StreakEntry>(); }

public class StreakManager : MonoBehaviour
{
    public static StreakManager Instance;
    string filePath => Path.Combine(Application.persistentDataPath, "streaks.json");

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public StreakData LoadStreaks()
    {
        if (!File.Exists(filePath)) return new StreakData();
        return JsonUtility.FromJson<StreakData>(File.ReadAllText(filePath));
    }

    public void SaveStreaks(StreakData d) =>
        File.WriteAllText(filePath, JsonUtility.ToJson(d, true));

    public bool TryAddNewStreak(int s, out int newIndex)
    {
        var d = LoadStreaks();
        newIndex = -1;
        if (s <= 0) return false;

        if (!PlayerProfileManager.HasNameSaved()) return true;

        string name = PlayerProfileManager.GetPlayerName();
        d.entries.Add(new StreakEntry { playerName = name, streak = s });
        d.entries = d.entries.OrderByDescending(e => e.streak).Take(10).ToList();
        newIndex = d.entries.FindIndex(e => e.playerName == name && e.streak == s);
        SaveStreaks(d);
        return newIndex != -1;
    }


    public bool CheckIfTopStreak(int streak, out int rank)
    {
        var data = LoadStreaks();
        data.entries.Add(new StreakEntry { playerName = "temp", streak = streak });
        data.entries = data.entries.OrderByDescending(e => e.streak).Take(10).ToList();
        rank = data.entries.FindIndex(e => e.streak == streak);
        return rank != -1;
    }

    public void SaveStreakWithName(int streak, string name)
    {
        var data = LoadStreaks();
        data.entries.Add(new StreakEntry { playerName = name, streak = streak });
        data.entries = data.entries.OrderByDescending(e => e.streak).Take(10).ToList();
        SaveStreaks(data);
    }


}

