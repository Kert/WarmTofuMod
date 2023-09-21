using UnityEngine;
using BepInEx;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine.UI;

namespace WarmTofuMod
{
    public partial class WarmTofuMod : BaseUnityPlugin
    {
        bool firstLoad = true;
        string initialMap = "Irohazaka";
        Dropdown mapSelectDropdown;
        void LauncherManager_Start(On.SpielmannSpiel_Launcher.LauncherManager.orig_Start orig, SpielmannSpiel_Launcher.LauncherManager self)
        {
            orig(self);
            if (firstLoad) // for some reason this func is called even when the launcher is long gone
            {
                GameObject dummy = GameObject.Find("drpQuality");
                GameObject mapSelect = Instantiate(dummy);
                mapSelect.transform.SetParent(dummy.transform.GetParent());
                mapSelect.name = "MapSelect";
                mapSelectDropdown = mapSelect.GetComponent<Dropdown>();
                mapSelectDropdown.ClearOptions();
                mapSelectDropdown.AddOptions(new List<string> { "Irohazaka", "Akina", "USUI", "Akagi" });
                mapSelectDropdown.onValueChanged = new Dropdown.DropdownEvent();
                initialMap = PlayerPrefs.GetString("InitialMap", "Irohazaka");
                mapSelectDropdown.value = mapSelectDropdown.options.FindIndex(option => option.text == initialMap);
                RectTransform rt = mapSelect.GetComponent<RectTransform>();
                rt.localScale = dummy.GetComponent<RectTransform>().localScale;
            }
        }

        void LauncherManager_Update(On.SpielmannSpiel_Launcher.LauncherManager.orig_Update orig, SpielmannSpiel_Launcher.LauncherManager self)
        {
            orig(self);
            if (firstLoad)
                mapSelectDropdown.gameObject.GetComponent<RectTransform>().position = new Vector2(110, 151);
        }

        void SRTransitionMap_Start(On.SRTransitionMap.orig_Start orig, SRTransitionMap self)
        {
            orig(self);
            if (firstLoad)
            {
                if (initialMap != "Irohazaka")
                    PhotonNetwork.LoadLevel(initialMap);
                firstLoad = false;
            }
        }

        void LauncherManager_start(On.SpielmannSpiel_Launcher.LauncherManager.orig_start orig, SpielmannSpiel_Launcher.LauncherManager self)
        {
            orig(self);
            initialMap = mapSelectDropdown.options[mapSelectDropdown.value].text;
            PlayerPrefs.SetString("InitialMap", initialMap);
        }
    }
}
