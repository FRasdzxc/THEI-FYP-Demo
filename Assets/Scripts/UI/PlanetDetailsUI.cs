using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class PlanetDetailsUI : MonoBehaviour
{
    public static PlanetDetailsUI Instance { get; private set; }
    [Header("Header")]
    public TextMeshProUGUI bioText;
    public TextMeshProUGUI astText;
    public TextMeshProUGUI darText;
    public TextMeshProUGUI solText;

    [Header("UI Panel")]
    public GameObject gameoverPanel;
    public GameObject detailsPanel;

    [Header("Home Planet")]
    public GameObject btn_SpaceshipCreation;
    public Button collect;

    [Header("Warning Panel")]
    public GameObject warningPanel;
    public TextMeshProUGUI warningText;

    [Header("Spaceship Creation Panel")]
    public GameObject spaceshipCreationPanel;
    public Button createSpaceshipButton;
    public Button cancelSpaceshipButton;
    private GameObject previousSelected;
    private string selectedSpaceshipType;

    [Header("Spaceship Info")]
    public GameObject spaceshipInfoPanel;
    public TextMeshProUGUI spaceshipNameText;
    public Slider fuelSlider;
    public TextMeshProUGUI shipSoldierText;
    public TextMeshProUGUI shipWeaponTierText;
    public TextMeshProUGUI fuelText;

    [Header("Basic Info")]
    public TextMeshProUGUI planetNameText;
    public Image planetImage;
    public GameObject scrollView;

    [Header("Resources")]
    public TextMeshProUGUI bioMassText;
    public TextMeshProUGUI asteroidOreText;
    public TextMeshProUGUI darkMatterText;
    public TextMeshProUGUI solarCrystalText;

    [Header("Growth Rates")]
    public TextMeshProUGUI bioMassGrowthText;
    public TextMeshProUGUI asteroidOreGrowthText;
    public TextMeshProUGUI darkMatterGrowthText;
    public TextMeshProUGUI solarCrystalGrowthText;

    [Header("Alien Info")]
    public GameObject alienInfoPanel;
    public TextMeshProUGUI soldiersText;
    public TextMeshProUGUI weaponTierText;

    [Header("Spaceships")]
    public Transform spaceshipPanel;
    public GameObject btn_prefab;
    private Planet currentPlanet;
    private List<GameObject> buttons = new List<GameObject>();

    #region Unity Methods
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    private void Start()
    {
        UpdateHeaderUI();
        collect.onClick.AddListener(OnCollectClicked);
        HidePanel();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HidePanel();
            ShowSpaceshipInfoPanel(null, false);
            InputHandler.Instance.ResetForUI();
        }
    }
    #endregion Unity Methods

    #region Planet Details Panel Methods
    public void ShowPlanetDetails(Planet planet)
    {
        currentPlanet = planet;
        detailsPanel.SetActive(true);
        UpdatePlanetDetails();
    }
    public void HidePanel()
    {
        detailsPanel.SetActive(false);
        currentPlanet = null;
        ClearAllButtons();
    }
    public void UpdatePlanetDetails()
    {
        ClearAllButtons();
        if (currentPlanet.bioMass > 0)
        {
            collect.gameObject.SetActive(true);
        }
        else
        {
            collect.gameObject.SetActive(false);
        }
        if(currentPlanet.GetSpaceships().Count > 0)
        {
            scrollView.SetActive(true);
        }
        else
        {
            scrollView.SetActive(false);
        }
        if (currentPlanet.isHomePlanet)
        {
            btn_SpaceshipCreation.SetActive(true);
        }
        else
        {
            btn_SpaceshipCreation.SetActive(false);
        }
        // Basic Info
        planetNameText.text = $"{currentPlanet.planetName}";

        // Resources
        bioMassText.text = $"{currentPlanet.bioMass}/{currentPlanet.resourceLimit}"; // Bio-Mass
        asteroidOreText.text = $"{currentPlanet.asteroidOre}/{currentPlanet.resourceLimit}"; // Asteroid Ore
        darkMatterText.text = $"{currentPlanet.darkMatter}/{currentPlanet.resourceLimit}"; // Dark Matter
        solarCrystalText.text = $"{currentPlanet.solarCrystal}/{currentPlanet.resourceLimit}"; // Solar Crystal

        // Growth Rates
        bioMassGrowthText.text = $"{currentPlanet.bioMassGrowthRate}/turn";
        asteroidOreGrowthText.text = $"{currentPlanet.asteroidOreGrowthRate}/turn";
        darkMatterGrowthText.text = $"{currentPlanet.darkMatterGrowthRate}/turn";
        solarCrystalGrowthText.text = $"{currentPlanet.solarCrystalGrowthRate}/turn";

        // Alien Info
        alienInfoPanel.SetActive(currentPlanet.hasAlien);
        if (currentPlanet.hasAlien)
        {
            soldiersText.text = $"{currentPlanet.soldiers}";
            weaponTierText.text = $"{currentPlanet.weaponTier}";
        }
        ClearAllButtons();
        if(currentPlanet.GetSpaceships().Count > 0)
        {
            foreach(Spaceship spaceship in currentPlanet.GetSpaceships())
            {
                GenerateSpaceshipButton(spaceship);
            }
        }
    }
    public void GenerateSpaceshipButton(Spaceship ship)
    {
        GameObject buttonObj = Instantiate(btn_prefab, spaceshipPanel);
        TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
        Button button = buttonObj.GetComponent<Button>();

        buttonText.text = $"Ship {ship.spaceshipName}"; // Assuming ship has a name property

        button.onClick.AddListener(() => OnSpaceshipButtonClicked(ship));
        buttons.Add(buttonObj);
    }

    private void OnSpaceshipButtonClicked(Spaceship ship)
    {
        Debug.Log($"Selected ship: {ship.spaceshipName}");
        InputHandler.Instance.HandleSpaceshipSelection(ship);
    }
    public void DeleteSpaceshipButton(Spaceship ship)
    {
        foreach(GameObject button in buttons)
        {
            TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText.text == $"Ship {ship.spaceshipName}")
            {
                buttons.Remove(button);
                Destroy(button);
            }
        }
    }
    private void OnCollectClicked()
    {
        PlayerInstance.Instance.AddResources(currentPlanet.bioMass, currentPlanet.asteroidOre, currentPlanet.darkMatter, currentPlanet.solarCrystal);
        currentPlanet.bioMass = 0;
        currentPlanet.asteroidOre = 0;
        currentPlanet.darkMatter = 0;
        currentPlanet.solarCrystal = 0;
        UpdateHeaderUI();
        UpdatePlanetDetails();
    }
    public void ShowSpaceshipInfoPanel(Spaceship spaceship, bool status)
    {
        if (status)
        {
            spaceshipNameText.text = spaceship.spaceshipName;
            fuelText.text = $"{spaceship.currentFuel}/{spaceship.GetMaxFuel()}";
            fuelSlider.maxValue = spaceship.GetMaxFuel();
            fuelSlider.value = spaceship.currentFuel;
            shipSoldierText.text = $"{spaceship.soldiers}";
            shipWeaponTierText.text = $"{spaceship.weaponTier}";
        }
        spaceshipInfoPanel.SetActive(status);
    }
    public void ClearAllButtons()
    {
        foreach(GameObject button in buttons)
        {
            Destroy(button);
        }
        buttons.Clear();
    }
    #endregion Planet Details Panel Methods

    #region Header UI Methods
    private void UpdateHeaderUI()
    {
        bioText.text = PlayerInstance.Instance.totalBioMass.ToString();
        astText.text = PlayerInstance.Instance.totalAsteroidOre.ToString();
        darText.text = PlayerInstance.Instance.totalDarkMatter.ToString();
        solText.text = PlayerInstance.Instance.totalSolarCrystal.ToString();
    }
    #endregion Header UI Methods

    #region Game Over Panel Methods
    public void ShowGameOverPanel()
    {
        gameoverPanel.SetActive(true);
    }
    #endregion Game Over Panel Methods

    #region Warning Panel Methods
    public void ShowWarningMessage(string message)
    {
        warningText.text = message;
        warningPanel.SetActive(true);
    }
    #endregion Warning Panel Methods

    #region Button Clicks
    public void HideWarningPanel()
    {
        warningPanel.SetActive(false);
    }
    public void ToggleSpaceshipCreationPanel(bool status)
    {
        if (status)
        {
            spaceshipCreationPanel.SetActive(true);
        }
        else
        {
            if (previousSelected != null)
            {
                Image obj = previousSelected.GetComponent<Image>();
                if (obj != null)
                {
                    Color color = obj.color;
                    color.a = 0f;
                    obj.color = color;
                }
            }
            previousSelected = null;
            selectedSpaceshipType = null;
            spaceshipCreationPanel.SetActive(false);
        }
    }
    public void InitiateSpaceshipCreation()
    {
        if(selectedSpaceshipType == null)
        {
            ShowWarningMessage("Please select a spaceship type");
            return;
        }
        switch(selectedSpaceshipType)
        {
            case "Scout":
                SpaceshipManager.Instance.CreateSpaceship(SpaceshipManager.Instance.GetScout(), new Vector3(0, 0, 0), playerId: 1);
                break;
            case "Combat":
                SpaceshipManager.Instance.CreateSpaceship(SpaceshipManager.Instance.GetCombat(), new Vector3(0, 0, 0), playerId: 1);
                break;
            case "Delivery":
                SpaceshipManager.Instance.CreateSpaceship(SpaceshipManager.Instance.GetDelivery(), new Vector3(0, 0, 0), playerId: 1);
                break;
        }
    }
    public void ButtonVisibile()
    {
        if(previousSelected != null)
        {
            Image obj = previousSelected.GetComponent<Image>();
            if (obj != null)
            {
                Color color = obj.color;
                color.a = 0f;
                obj.color = color;
            }
        }
        GameObject clickedObject = EventSystem.current.currentSelectedGameObject;
        if (clickedObject != null)
        {
            previousSelected = clickedObject;
            Image obj = clickedObject.GetComponent<Image>();
            if (obj != null)
            {
                Color color = obj.color;
                color.a = 1f;
                obj.color = color;
            }
        }
        if(clickedObject.name == "Scout")
        {
            selectedSpaceshipType = "Scout";
        }
        else if(clickedObject.name == "Combat")
        {
            selectedSpaceshipType = "Combat";
        }
        else if(clickedObject.name == "Delivery")
        {
            selectedSpaceshipType = "Delivery";
        }
    }
    #endregion Button Clicks
}