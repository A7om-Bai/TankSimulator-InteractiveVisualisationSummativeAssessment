using System.Collections.Generic;
using UnityEngine;

public class TankShowcaseController : MonoBehaviour
{
    [Header("Exterior Meshes (ALL exterior Renderers)")]
    public List<MeshRenderer> exteriorRenderers = new List<MeshRenderer>();
    public Material normalExteriorMaterial;
    public Material xrayExteriorMaterial;

    public GarageUIManager GarageUImanager;
    public UI_KeyBarController keyBar;

    [Header("Interior Modules")]
    public GameObject engine;
    public GameObject crew;
    public GameObject gearBox;
    public GameObject machineGun;
    public GameObject turret;
    public GameObject turretControl;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip audioSourceClip;

    private Dictionary<int, GameObject> moduleByKey = new Dictionary<int, GameObject>();
    private Dictionary<int, string> descByKey = new Dictionary<int, string>();

    private int currentMode = 0;
    private bool descriptionVisible = false;

    void Awake()
    {
        // Initialize the mapping of keys to modules and descriptions
        moduleByKey[3] = engine;
        moduleByKey[4] = crew;
        moduleByKey[5] = gearBox;
        moduleByKey[6] = machineGun;
        moduleByKey[7] = turret;
        moduleByKey[8] = turretControl;

        descByKey[1] = "ArmorThickness.";
        descByKey[2] = "The Tiger H1 ";
        descByKey[3] = "Engine module.";
        descByKey[4] = "Crew arrangements.";
        descByKey[5] = "GearBox structure.";
        descByKey[6] = "MachineGun.";
        descByKey[7] = "Main Gun.";
        descByKey[8] = "Turret control system.";
    }

    void Start()
    {
        // Hide the description panel and initialize the default mode
        UI_DescriptionPanel.Hide();   // Hide UI at the start
        ShowDefault(false, false);    // Initialize the model without showing UI or playing sound
        if (GarageUImanager != null)
            GarageUImanager.HideUI();
    }

    void Update()
    {
        // Check for key presses to activate different modes
        CheckKey(KeyCode.Alpha1, 1);
        CheckKey(KeyCode.Alpha2, 2);
        CheckKey(KeyCode.Alpha3, 3);
        CheckKey(KeyCode.Alpha4, 4);
        CheckKey(KeyCode.Alpha5, 5);
        CheckKey(KeyCode.Alpha6, 6);
        CheckKey(KeyCode.Alpha7, 7);
        CheckKey(KeyCode.Alpha8, 8);
    }

    /// <summary>
    /// Checks if a specific key is pressed and activates the corresponding mode.
    /// </summary>
    /// <param name="key">The key to check.</param>
    /// <param name="mode">The mode to activate.</param>
    void CheckKey(KeyCode key, int mode)
    {
        if (Input.GetKeyDown(key))
        {
            // If the same key is pressed again, hide the description and UI
            if (currentMode == mode && descriptionVisible)
            {
                UI_DescriptionPanel.Hide();
                descriptionVisible = false;

                if (GarageUImanager != null)
                    GarageUImanager.HideUI();

                if (keyBar != null)
                    keyBar.ToggleKey(mode);

                PlaySound(audioSourceClip);
                return;
            }

            // Activate the new mode
            ActivateMode(mode);
            currentMode = mode;
            descriptionVisible = true;

            if (keyBar != null)
                keyBar.ToggleKey(mode);
        }
    }

    /// <summary>
    /// Activates the specified mode (default, X-ray, or module view).
    /// </summary>
    /// <param name="mode">The mode to activate.</param>
    void ActivateMode(int mode)
    {
        switch (mode)
        {
            case 1:
                ShowDefault(true, true);
                break;
            case 2:
                ShowXRay();
                break;
            default:
                ShowModule(mode);
                break;
        }
    }

    /// <summary>
    /// Plays a sound effect.
    /// </summary>
    /// <param name="clip">The audio clip to play.</param>
    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }

    /// <summary>
    /// Applies the specified material to all exterior renderers.
    /// </summary>
    /// <param name="m">The material to apply.</param>
    void ApplyExteriorMaterial(Material m)
    {
        foreach (var r in exteriorRenderers)
        {
            if (r != null)
                r.material = m;
        }
    }

    /// <summary>
    /// Hides all interior modules.
    /// </summary>
    void HideAllModules()
    {
        foreach (var kv in moduleByKey)
            if (kv.Value != null)
                kv.Value.SetActive(false);
    }

    /// <summary>
    /// Activates the default mode, hiding all modules and applying the normal material.
    /// </summary>
    /// <param name="showUI">Whether to show the UI description.</param>
    /// <param name="playSound">Whether to play the activation sound.</param>
    public void ShowDefault(bool showUI = true, bool playSound = true)
    {
        ApplyExteriorMaterial(normalExteriorMaterial);
        HideAllModules();

        if (showUI)
            UI_DescriptionPanel.Show(descByKey[1]);

        if (GarageUImanager != null)
            GarageUImanager.ShowUI(1);

        if (playSound)
            PlaySound(audioSourceClip);
    }

    /// <summary>
    /// Activates the X-ray mode, showing all interior modules.
    /// </summary>
    public void ShowXRay()
    {
        ApplyExteriorMaterial(xrayExteriorMaterial);

        foreach (var kv in moduleByKey)
            if (kv.Value != null)
                kv.Value.SetActive(true);

        UI_DescriptionPanel.Show(descByKey[2]);

        if (GarageUImanager != null)
            GarageUImanager.ShowUI(2);
        PlaySound(audioSourceClip);
    }

    /// <summary>
    /// Activates the module view for a specific module.
    /// </summary>
    /// <param name="key">The key corresponding to the module.</param>
    public void ShowModule(int key)
    {
        ApplyExteriorMaterial(xrayExteriorMaterial);
        HideAllModules();

        if (moduleByKey.ContainsKey(key) && moduleByKey[key] != null)
            moduleByKey[key].SetActive(true);

        UI_DescriptionPanel.Show(descByKey[key]);

        if (GarageUImanager != null)
            GarageUImanager.ShowUI(key);
        PlaySound(audioSourceClip);
    }
}
