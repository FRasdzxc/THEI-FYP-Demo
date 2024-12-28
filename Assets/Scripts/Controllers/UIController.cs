// TurnUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    public TextMeshProUGUI turnText;
    public TextMeshProUGUI playerText;
    public Button endTurnButton;

    private void Start()
    {
        endTurnButton.onClick.AddListener(OnEndTurnClicked);
        UpdateUI();
    }

    private void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        turnText.text = $"Turn: {GameManager.Instance.currentTurn}";
        playerText.text = $"Player {GameManager.Instance.currentPlayerIndex + 1}'s Turn";
    }

    private void OnEndTurnClicked()
    {
        GameManager.Instance.EndTurn();
    }
}