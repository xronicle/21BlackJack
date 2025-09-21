using TMPro;
using UnityEngine;

public class TopStreakMessageUI : MonoBehaviour
{
    public GameObject popupCanvas;
    public TMP_Text messageText;

    public void Show(int rank, int streak)
    {
        popupCanvas.SetActive(true);
        messageText.text = $"Ты попал в ТОП-{rank + 1} с результатом {streak}!";
    }

    public void Hide()
    {
        popupCanvas.SetActive(false);
    }
}
