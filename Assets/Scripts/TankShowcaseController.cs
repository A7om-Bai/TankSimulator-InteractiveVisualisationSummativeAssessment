using System.Collections.Generic;
using UnityEngine;

public class TankShowcaseController : MonoBehaviour
{
    [Header("Exterior Meshes (ALL exterior Renderers)")]
    public List<MeshRenderer> exteriorRenderers = new List<MeshRenderer>();
    public Material normalExteriorMaterial;
    public Material xrayExteriorMaterial;


    public GarageUIManager GarageUImanager;

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

    // --- 内构、说明文字 ---
    private Dictionary<int, GameObject> moduleByKey = new Dictionary<int, GameObject>();
    private Dictionary<int, string> descByKey = new Dictionary<int, string>();

    // --- 状态记录（用于检测重复按键） ---
    private int currentMode = 0;
    private bool descriptionVisible = false;


    // ==========================================================
    //                     初始化
    // ==========================================================
    void Awake()
    {
        moduleByKey[3] = engine;
        moduleByKey[4] = crew;
        moduleByKey[5] = gearBox;
        moduleByKey[6] = machineGun;
        moduleByKey[7] = turret;
        moduleByKey[8] = turretControl;

        descByKey[1] = "Standard armor exterior.";
        descByKey[2] = "The Panzerkampfwagen VI Ausführung H1 (Tiger H1) is the first (early-production) variant of the Tiger I heavy tank family, designed and built by Henschel and used by the German Army during World War II. It offered the German Army its first armoured fighting vehicle equipped with the 88 mm Kampfwagenkanone (KwK) 36 tank gun, developed from the 88 mm Flugabwehrkanone (FlaK) 36 anti-aircraft gun.\r\nWeight:\r\n54 Tonnes\r\n57 Tonnes(Combat weight)\r\nLength\r\n6.30m\r\nHight\r\n3.00m.";
        descByKey[3] = "Engine module.";
        descByKey[4] = "Crew arrangements.";
        descByKey[5] = "GearBox structure.";
        descByKey[6] = "MachineGun components.";
        descByKey[7] = "Turret structure.";
        descByKey[8] = "Turret control gear.";
    }

    void Start()
    {
        UI_DescriptionPanel.Hide();   // 开场隐藏 UI
        ShowDefault(false, false);    // 初始化模型：不显示UI、不播放声音
        if (GarageUImanager != null)
            GarageUImanager.HideUI();
    }


    // ==========================================================
    //                     Update 监听按键
    // ==========================================================
    void Update()
    {
        CheckKey(KeyCode.Alpha1, 1);
        CheckKey(KeyCode.Alpha2, 2);
        CheckKey(KeyCode.Alpha3, 3);
        CheckKey(KeyCode.Alpha4, 4);
        CheckKey(KeyCode.Alpha5, 5);
        CheckKey(KeyCode.Alpha6, 6);
        CheckKey(KeyCode.Alpha7, 7);
        CheckKey(KeyCode.Alpha8, 8);
    }


    // ==========================================================
    //         核心逻辑：重复按相同按键 → 隐藏说明面板
    // ==========================================================
    void CheckKey(KeyCode key, int mode)
    {
        if (Input.GetKeyDown(key))
        {
            // 再次按下当前模式 → 关闭 UI，并播放音效
            if (currentMode == mode && descriptionVisible)
            {
                UI_DescriptionPanel.Hide();
                descriptionVisible = false;

                if (GarageUImanager != null)
                    GarageUImanager.HideUI();

                // 播放关闭提示音
                PlaySound(audioSourceClip);
                return;
            }

            // 切换模式
            ActivateMode(mode);
            currentMode = mode;
            descriptionVisible = true;
        }
    }


    // ==========================================================
    //                 模式触发（1~8）
    // ==========================================================
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


    // ==========================================================
    //                     音效播放
    // ==========================================================    
    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }


    // ==========================================================
    //               外观材质切换工具函数
    // ==========================================================
    void ApplyExteriorMaterial(Material m)
    {
        foreach (var r in exteriorRenderers)
        {
            if (r != null)
                r.material = m;
        }
    }


    // ==========================================================
    //                     内构显示控制
    // ==========================================================
    void HideAllModules()
    {
        foreach (var kv in moduleByKey)
            if (kv.Value != null)
                kv.Value.SetActive(false);
    }


    // ==========================================================
    //                     模式：默认装甲
    // ==========================================================
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


    // ==========================================================
    //                     模式：全 X 光
    // ==========================================================
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


    // ==========================================================
    //                     模式：单一模块
    // ==========================================================
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
