// PlanetDetailsUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PlanetDetailsUI : MonoBehaviour
{
    public static PlanetDetailsUI Instance { get; private set; }
    [Header("UI Panel")]
    public GameObject detailsPanel;

    [Header("Basic Info")]
    public TextMeshProUGUI planetNameText;
    public Image planetImage;

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
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    private void Start()
    {
        HidePanel();
    }

    public void ShowPlanetDetails(Planet planet)
    {
        currentPlanet = planet;
        detailsPanel.SetActive(true);
        UpdateUI();
    }

    public void HidePanel()
    {
        detailsPanel.SetActive(false);
        currentPlanet = null;
        ClearAllButtons();
    }

    private void Update()
    {
        //if (currentPlanet != null)
        //{
        //    UpdateUI();
        //}

        // Close panel if Escape is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HidePanel();
        }
    }

    private void UpdateUI()
    {
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
        if(currentPlanet.GetSpaceships().Count > 0)
        {
            foreach(Spaceship spaceship in currentPlanet.GetSpaceships())
            {
                CreateSpaceshipButton(spaceship);
            }
        }
    }
    public void CreateSpaceshipButton(Spaceship ship)
    {
        GameObject buttonObj = Instantiate(btn_prefab, spaceshipPanel);
        TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
        Button button = buttonObj.GetComponent<Button>();

        buttonText.text = $"Ship {ship.spaceshipName}"; // Assuming ship has a name property

        button.onClick.AddListener(() => OnShipButtonClicked(ship));
        buttons.Add(buttonObj);
    }

    private void OnShipButtonClicked(Spaceship ship)
    {
        // Handle button click - select ship, show info, etc.
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

    // Method to clear all buttons if needed
    public void ClearAllButtons()
    {
        foreach(GameObject button in buttons)
        {
            Destroy(button);
        }
        buttons.Clear();
    }
}