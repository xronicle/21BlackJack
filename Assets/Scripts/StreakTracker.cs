using UnityEngine;

public class StreakTracker : MonoBehaviour
{
    public static StreakTracker Instance;
    private int currentStreak = 0;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void IncreaseStreak() => currentStreak++;
    public void ResetStreak() => currentStreak = 0;

    public int GetStreak() => currentStreak;

    public bool TryEndStreak(out int streak, out int rank)
    {
        streak = currentStreak;
        rank = -1;

        if (streak <= 0) return false;

        bool isTop = StreakManager.Instance.CheckIfTopStreak(streak, out rank);
        ResetStreak();
        return isTop;
    }

}
