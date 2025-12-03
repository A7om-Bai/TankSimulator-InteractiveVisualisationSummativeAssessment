using System.Collections.Generic;
using UnityEngine;

public class TankShowcaseController : MonoBehaviour
{
    [Header("Exterior Meshes (ALL exterior Renderers)")]
    public List<MeshRenderer> exteriorRenderers = new List<MeshRenderer>();
    public Material normalExteriorMaterial;
    public Material xrayExteriorMaterial;

    [Header("Interior Modules")]
    public GameObject engine;
    public GameObject crew;
    public GameObject gearBox;
    public GameObject machineGun;
    public GameObject turret;
    public GameObject turretControl;

    private Dictionary<int, GameObject> moduleByKey = new Dictionary<int, GameObject>();
    private Dictionary<int, string> descByKey = new Dictionary<int, string>();

    void Awake()
    {
        moduleByKey[3] = engine;
        moduleByKey[4] = crew;
        moduleByKey[5] = gearBox;
        moduleByKey[6] = machineGun;
        moduleByKey[7] = turret;
        moduleByKey[8] = turretControl;

        descByKey[1] = "Standard armor exterior.";
        descByKey[2] = "Full X-ray transparent mode.";
        descByKey[3] = "Engine module.";
        descByKey[4] = "Crew arrangements.";
        descByKey[5] = "GearBox structure.";
        descByKey[6] = "MachineGun components.";
        descByKey[7] = "Turret structure.";
        descByKey[8] = "Turret control gear.";
    }

    void Start()
    {
        ShowDefault();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) ShowDefault();
        if (Input.GetKeyDown(KeyCode.Alpha2)) ShowXRay();
        if (Input.GetKeyDown(KeyCode.Alpha3)) ShowModule(3);
        if (Input.GetKeyDown(KeyCode.Alpha4)) ShowModule(4);
        if (Input.GetKeyDown(KeyCode.Alpha5)) ShowModule(5);
        if (Input.GetKeyDown(KeyCode.Alpha6)) ShowModule(6);
        if (Input.GetKeyDown(KeyCode.Alpha7)) ShowModule(7);
        if (Input.GetKeyDown(KeyCode.Alpha8)) ShowModule(8);
    }


    // ====================== Exterior Material Switch ======================
    void ApplyExteriorMaterial(Material m)
    {
        foreach (var r in exteriorRenderers)
        {
            if (r != null)
                r.material = m;
        }
    }


    // ========================= Modes =========================

    void HideAllModules()
    {
        foreach (var kv in moduleByKey)
        {
            if (kv.Value != null)
                kv.Value.SetActive(false);
        }
    }

    public void ShowDefault()
    {
        ApplyExteriorMaterial(normalExteriorMaterial);
        HideAllModules();
        UI_DescriptionPanel.Show(descByKey[1]);
    }

    public void ShowXRay()
    {
        ApplyExteriorMaterial(xrayExteriorMaterial);

        // 让所有内构显示
        foreach (var kv in moduleByKey)
        {
            if (kv.Value != null)
                kv.Value.SetActive(true);
        }

        UI_DescriptionPanel.Show(descByKey[2]);
    }

    public void ShowModule(int key)
    {
        ApplyExteriorMaterial(xrayExteriorMaterial);
        HideAllModules();

        if (moduleByKey.ContainsKey(key) && moduleByKey[key] != null)
            moduleByKey[key].SetActive(true);

        UI_DescriptionPanel.Show(descByKey[key]);
    }
}
