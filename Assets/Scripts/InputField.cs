using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class FocusInputFieldOnClick : MonoBehaviour, IPointerDownHandler
{
    public TMP_InputField inputField;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (inputField == null) return;

        inputField.ActivateInputField();

        // Запускаем корутину, чтобы корректно установить курсор
        StartCoroutine(SetCaretPositionNextFrame());

        eventData.Use();
    }

    private IEnumerator SetCaretPositionNextFrame()
    {
        yield return null;  // Ждём один кадр

        int textLength = inputField.text.Length;
        inputField.caretPosition = textLength;  // Ставим курсор в конец текста
        inputField.selectionAnchorPosition = textLength;
        inputField.selectionFocusPosition = textLength;
    }
}
