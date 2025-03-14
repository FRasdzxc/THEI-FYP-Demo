using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;

public class UIController : MonoBehaviour
{
    [Header("Turn Panel")]
    public TextMeshProUGUI turnText;
    public TextMeshProUGUI playerText;
    public Button endTurnButton;

    [Header("Spaceship Info Panel")]
    public GameObject informationPanel;
    public GameObject btn_prefab;
    public Transform placeholder;
    private bool isSlidingIn = false;
    public Image arrowImage;
    private RectTransform infoPanelRect;
    private Tween slideTween;
    private List<GameObject> buttons = new List<GameObject>();
    private RectTransform currentButtonContainer = null;

    private void Start()
    {
        endTurnButton.onClick.AddListener(OnEndTurnClicked);
        UpdateTurnUI();

        infoPanelRect = informationPanel.GetComponent<RectTransform>();
        infoPanelRect.anchoredPosition = new Vector2(infoPanelRect.rect.width, 0);
    }

    private void Update()
    {
        UpdateTurnUI();
    }

    public void SlideIn()
    {
        if (slideTween != null && slideTween.IsActive())
        {
            return; // Prevent overlapping tweens
        }
        CreateSpaceshipButton(SpaceshipManager.Instance.GetAllSpaceships());
        if (isSlidingIn)
        {
            arrowImage.rectTransform.DORotate(new Vector3(0, 0, 180), 0.5f, RotateMode.FastBeyond360).SetEase(Ease.InQuad);
            slideTween = infoPanelRect.DOAnchorPos(new Vector2(infoPanelRect.rect.width, 0), 0.5f).SetEase(Ease.InQuad).OnComplete(() =>
            {
                isSlidingIn = false;
                slideTween = null;
            });
        }
        else
        {
            arrowImage.rectTransform.DORotate(new Vector3(0, 0, 0), 0.5f, RotateMode.FastBeyond360).SetEase(Ease.OutQuad);
            slideTween = infoPanelRect.DOAnchorPos(new Vector2(0, 0), 0.5f).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                isSlidingIn = true;
                slideTween = null;
            });
        }
    }

    public void CreateSpaceshipButton(List<Spaceship> ships)
    {
        // Clear existing buttons
        foreach (GameObject button in buttons)
        {
            Destroy(button);
        }
        buttons.Clear();

        foreach (Spaceship ship in ships)
        {
            GameObject buttonObj = Instantiate(btn_prefab, placeholder);
            RectTransform buttonContainer = buttonObj.GetComponent<RectTransform>();
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            Button button = buttonObj.GetComponentInChildren<Button>();

            buttonText.text = $"Ship {ship.spaceshipName}"; // Assuming ship has a name property

            button.onClick.AddListener(() => OnShipButtonClicked(ship, buttonContainer));
            buttons.Add(buttonObj);
        }
    }

    private void OnShipButtonClicked(Spaceship ship, RectTransform buttonContainer)
    {
        Debug.Log($"Selected ship: {ship.spaceshipName}");
        InputHandler.Instance.HandleSpaceshipSelection(ship);

        // Move camera to spaceship
        MoveCameraToSpaceship(ship);

        // Move the previously clicked button back to its original position
        if (currentButtonContainer != null && currentButtonContainer != buttonContainer)
        {
            currentButtonContainer.DOAnchorPos(new Vector2(50, currentButtonContainer.anchoredPosition.y), 0.5f).SetEase(Ease.InOutQuad);
        }

        // Move the clicked button to the left or back to original position
        if (buttonContainer.anchoredPosition.x >= 49)
        {
            buttonContainer.DOAnchorPos(new Vector2(0, buttonContainer.anchoredPosition.y), 0.5f).SetEase(Ease.InOutQuad).OnComplete(() =>
            {
                buttonContainer.anchoredPosition = new Vector2(0, buttonContainer.anchoredPosition.y); //prevent the button not moving to the correct position
            });
            currentButtonContainer = buttonContainer;
        }
        else
        {
            buttonContainer.DOAnchorPos(new Vector2(50, buttonContainer.anchoredPosition.y), 0.5f).SetEase(Ease.InOutQuad).OnComplete(() =>
            {
                buttonContainer.anchoredPosition = new Vector2(50, buttonContainer.anchoredPosition.y); //prevent the button not moving to the correct position
            });
            currentButtonContainer = null;
        }
    }

    private void MoveCameraToSpaceship(Spaceship ship)
    {
        Camera.main.transform.DOMove(new Vector3(ship.transform.position.x, ship.transform.position.y, Camera.main.transform.position.z), 1f).SetEase(Ease.InOutQuad);
    }

    private void UpdateTurnUI()
    {
        turnText.text = $"Turn: {GameManager.Instance.currentTurn}";
        playerText.text = $"Player {GameManager.Instance.currentPlayerIndex + 1}'s Turn";
    }

    private void OnEndTurnClicked()
    {
        GameManager.Instance.EndTurn();
    }
}

