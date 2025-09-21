using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HiddenCard : MonoBehaviour
{
    public int index;
}

public class GameManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI scoreText;
    public Button drawButton;
    public Button stopButton;
    public Button restartButton;
    public TextMeshProUGUI dealerStatusText;
    public GameObject popupCanvas;


    [Header("Streak UI")]
    public TopStreakPopupUI topStreakPopup;
    public TopStreakMessageUI topStreakMessage;

    [Header("Card Setup")]
    public GameObject cardPrefab;
    public Transform cardParent;
    public Transform dealerCardParent;
    public Sprite[] cardSprites;
    public Sprite backSprite;
    public float cardSpacing = 100f;

    [Header("Timing Settings")]
    public float dealDelay = 0.5f;
    public float dealerDelay = 0.7f;

    private List<int> deck;
    private int playerScore;
    private bool isGameOver;
    private bool playerStopped;
    private int playerCardCount = 0;
    private int dealerCardCount = 0;


    private List<GameObject> dealerHiddenCards = new List<GameObject>();
    private List<int> dealerHiddenIndices = new List<int>();

    void Start()
    {
        drawButton.onClick.AddListener(PlayerDraw);
        stopButton.onClick.AddListener(PlayerStop);
        restartButton.onClick.AddListener(StartNewGame);
    }

    public void StartNewGame()
    {
        ClearContainer(cardParent);
        ClearContainer(dealerCardParent);
        dealerHiddenCards.Clear();
        dealerHiddenIndices.Clear();

        isGameOver = false;
        playerStopped = false;
        playerScore = 0;
        playerCardCount = 0;
        dealerCardCount = 0;
        dealerStatusText.text = "";
        dealerStatusText.gameObject.SetActive(false);
        scoreText.text = "Очки: 0";

        drawButton.interactable = true;
        stopButton.interactable = true;
        restartButton.gameObject.SetActive(false);

        deck = new List<int>();
        for (int i = 0; i < 52; i++) deck.Add(i);

        StartCoroutine(InitialDeal());
    }

    IEnumerator InitialDeal()
    {
        DealCardToPlayer();
        yield return new WaitForSeconds(dealDelay);

        DealCardToDealerHidden();
        yield return new WaitForSeconds(dealDelay);

        DealCardToPlayer();
        yield return new WaitForSeconds(dealDelay);

        DealCardToDealerHidden();
        yield return new WaitForSeconds(dealDelay);

        UpdatePlayerScoreText();
    }

    void PlayerDraw()
    {
        if (isGameOver || playerStopped) return;

        DealCardToPlayer();
        UpdatePlayerScoreText();

        if (playerScore > 21)
        {
            FlipAllDealerCards();
            EndGame("Перебор! Ты проиграл.");
            return;
        }

        StartCoroutine(DealerDrawOnce());
    }

    void PlayerStop()
    {
        if (isGameOver) return;

        playerStopped = true;
        drawButton.interactable = false;
        stopButton.interactable = false;

        StartCoroutine(DealerFinish());
    }

    IEnumerator DealerDrawOnce()
    {
        if (isGameOver || playerStopped) yield break;

        yield return new WaitForSeconds(dealerDelay);

        int currentScore = ComputeDealerHiddenScore();
        if (currentScore >= 17)
        {
            dealerStatusText.text = "Дилер закончил добор.";
            dealerStatusText.gameObject.SetActive(true);
            yield break;
        }

        DealCardToDealerHidden();

        int newScore = ComputeDealerHiddenScore();
        if (newScore > 21)
        {
            FlipAllDealerCards();
            EndGame($"Дилер перебрал ({newScore}) — ты победил!");
        }
        else if (newScore >= 17)
        {
            dealerStatusText.text = "Дилер закончил добор.";
            dealerStatusText.gameObject.SetActive(true);
        }
    }

    IEnumerator DealerFinish()
    {
        while (ComputeDealerHiddenScore() < 17 && !isGameOver)
        {
            yield return new WaitForSeconds(dealerDelay);
            DealCardToDealerHidden();

            int dealerScore = ComputeDealerHiddenScore();
            if (dealerScore > 21)
            {
                FlipAllDealerCards();
                dealerStatusText.text = "Дилер закончил добор.";
                dealerStatusText.gameObject.SetActive(true);
                EndGame($"Дилер перебрал ({dealerScore}) — ты победил!");
                yield break;
            }
        }

        yield return new WaitForSeconds(dealerDelay);
        FlipAllDealerCards();
        int finalDealerScore = ComputeDealerHiddenScore();
        yield return new WaitForSeconds(dealerDelay);

        if (playerScore > finalDealerScore)
            EndGame($"Ты победил! ({playerScore} vs {finalDealerScore})");
        else if (playerScore == finalDealerScore)
            EndGame($"Ничья! ({playerScore} vs {finalDealerScore})");
        else
            EndGame($"Проиграл! ({playerScore} vs {finalDealerScore})");
    }

void EndGame(string msg)
{
    isGameOver = true;
    scoreText.text += $"\n{msg}";
    restartButton.gameObject.SetActive(true);

    Debug.Log($"[EndGame] Сообщение: «{msg}». Текущий стрик: {StreakTracker.Instance.GetStreak()}");

    bool isLoss = msg.Contains("Проиграл") || msg.Contains("Перебор");

    if (isLoss)
    {
        Debug.Log("[EndGame] Фиксация поражения — проверяем попадание в топ-10");

        int endedStreak = StreakTracker.Instance.GetStreak();
        int endedRank = -1;
        bool isTop = StreakManager.Instance.CheckIfTopStreak(endedStreak, out endedRank);
        StreakTracker.Instance.ResetStreak();

        if (isTop)
        {
            Debug.Log($"[EndGame] Попал в топ! Место: {endedRank}, стрик: {endedStreak}");

            if (PlayerProfileManager.HasNameSaved())
            {
                StreakManager.Instance.SaveStreakWithName(endedStreak, PlayerProfileManager.GetPlayerName());
                topStreakMessage.Show(endedRank, endedStreak);
            }
            else
            {
                topStreakPopup.Show(endedRank, endedStreak, true, (enteredName) =>
                {
                    PlayerProfileManager.SavePlayerName(enteredName);
                    StreakManager.Instance.SaveStreakWithName(endedStreak, enteredName);
                });
            }
        }
    }
    else if (msg.Contains("победил"))
    {
        StreakTracker.Instance.IncreaseStreak();
    }
    else
    {
        Debug.Log("[EndGame] Ничья — стрик не меняется");
    }
}





    void DealCardToPlayer()
    {
        int idx = DrawFromDeck();
        playerScore += GetCardValue(idx);

        GameObject cardGO = Instantiate(cardPrefab, cardParent);
        cardGO.GetComponent<Image>().sprite = cardSprites[idx];

        float xOffset = playerCardCount * cardSpacing;
        cardGO.GetComponent<RectTransform>().localPosition = new Vector3(xOffset, -15f, 0f);
        playerCardCount++;
    }

    void DealCardToDealerHidden()
    {
        int idx = DrawFromDeck();

        GameObject cardGO = Instantiate(cardPrefab, dealerCardParent);
        cardGO.GetComponent<Image>().sprite = backSprite;

        float xOffset = -dealerCardCount * cardSpacing;
        cardGO.GetComponent<RectTransform>().localPosition = new Vector3(xOffset, -15f, 0f);
        dealerCardCount++;

        HiddenCard hidden = cardGO.AddComponent<HiddenCard>();
        hidden.index = idx;

        dealerHiddenCards.Add(cardGO);
        dealerHiddenIndices.Add(idx);
    }

    void FlipAllDealerCards()
    {
        for (int i = 0; i < dealerHiddenCards.Count; i++)
        {
            dealerHiddenCards[i].GetComponent<Image>().sprite = cardSprites[dealerHiddenIndices[i]];
        }
    }

    int ComputeDealerHiddenScore()
    {
        int sum = 0;
        foreach (int idx in dealerHiddenIndices)
            sum += GetCardValue(idx);
        return sum;
    }

    int DrawFromDeck()
    {
        int r = Random.Range(0, deck.Count);
        int card = deck[r];
        deck.RemoveAt(r);
        return card;
    }

    int GetCardValue(int index)
    {
        int v = index % 13;
        if (v == 0) return 11;
        if (v <= 9) return v + 1;
        if (v == 10) return 2;
        if (v == 11) return 3;
        return 4;
    }

    void UpdatePlayerScoreText()
    {
        scoreText.text = $"Очки: {playerScore}";
    }

    void ClearContainer(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
            Destroy(parent.GetChild(i).gameObject);
    }
    public void ClosePopup()
    {
        popupCanvas.SetActive(false);
    }

}
