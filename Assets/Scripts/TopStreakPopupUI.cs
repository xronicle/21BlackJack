using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TopStreakPopupUI : MonoBehaviour
{
    public GameObject popupCanvas;
    public TMP_Text messageText;
    public TMP_InputField nameInputField;
    public Button confirmButton;

    private System.Action<string> onNameConfirmed;

    void Start()
    {
        confirmButton.onClick.AddListener(OnConfirmClicked);
        popupCanvas.SetActive(false);
    }

    public void Show(int rank, int streak, bool askName, System.Action<string> onConfirm)
    {
        Debug.Log($"[TopStreakPopupUI] ShowPopup called! rank={rank}, streak={streak}, askName={askName}");
        popupCanvas.SetActive(true);
        onNameConfirmed = onConfirm;
        messageText.text = $"Ты попал в ТОП-{rank + 1} с результатом {streak}!";
        nameInputField.gameObject.SetActive(askName);
        if (askName)
        {
            nameInputField.text = "";
            nameInputField.ActivateInputField();
        }
    }

    void OnConfirmClicked()
    {
        string name = nameInputField.text.Trim();
        onNameConfirmed?.Invoke(name);

        // Скрытие окна после подтверждения
        Hide();
    }

    public void Hide()
    {
        popupCanvas.SetActive(false);
    }
}
