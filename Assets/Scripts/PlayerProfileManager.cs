using UnityEngine;

public class PlayerProfileManager : MonoBehaviour
{
    private const string PlayerNameKey = "PlayerName";
    public static PlayerProfileManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void SavePlayerName(string name)
    {
        PlayerPrefs.SetString(PlayerNameKey, name);
        PlayerPrefs.Save();
    }

    public static string GetPlayerName()
    {
        return PlayerPrefs.GetString(PlayerNameKey, "");
    }

    public static bool HasNameSaved()
    {
        return PlayerPrefs.HasKey(PlayerNameKey) && !string.IsNullOrWhiteSpace(GetPlayerName());
    }
}
