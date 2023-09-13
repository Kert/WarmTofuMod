using BepInEx;
using UnityEngine;
using System.Collections;
using CodeStage.AntiCheat.Storage;
using CodeStage.AntiCheat.ObscuredTypes;
using HeathenEngineering.SteamApi.PlayerServices.UI;
using HeathenEngineering.SteamApi.PlayerServices;
using Steamworks;
using ZionBandwidthOptimizer.Examples;
using System.Linq;
using UnityEngine.UI;

namespace WarmTofuMod
{
    public partial class WarmTofuMod : BaseUnityPlugin
    {
        SteamworksLeaderboardData boardIroDH2;
        SteamworksLeaderboardData boardIroUH2;
        SteamworksLeaderboardData boardAkinaUH2;
        GameObject LB_IroBT2;
        GameObject LB_IroBTReverse2;
        GameObject LB_HarunaBTReverse2;

        void CreateIrohazakTofuDownhill2()
        {
            GameObject tofuZone = Instantiate(GameObject.Find("[UP]Zone_TOFU_Grossiste"));
            tofuZone.name = "[UP]Downhill2_Tofu";
            tofuZone.transform.position = new Vector3(-302.0f, 203.0f, 510.0f);

            SRToffuLivraison tofuDeliveryData = tofuZone.GetComponent<SRToffuLivraison>();
            tofuDeliveryData.location = "Downhill2";
            tofuDeliveryData.LangWelcome = "Sayori: Greetings!";
            tofuDeliveryData.LangDeliveryInProgress = "Sayori: You should finish delivery before coming for more";
            tofuDeliveryData.LangSeeYouSoon = "Sayori: See you again!";

            Transform sceneObjects = GameObject.Find("Scene Objects").transform;

            GameObject building = Instantiate(GameObject.Find("kyoto_style_house_faramiyuki_building"));
            building.transform.SetParent(sceneObjects);
            building.name = "Tofushop_Downhill2";
            building.transform.position = new Vector3(-295.5f, 204.0f, 505.5f);
            building.transform.eulerAngles = new Vector3(0, 90f, 0);

            GameObject lb_downhill2 = Instantiate(GameObject.Find("3D_LB_UPHILL"));
            lb_downhill2.transform.SetParent(sceneObjects);
            lb_downhill2.name = "3D_LB_UPHILL2";
            lb_downhill2.transform.position = new Vector3(-297.31f, 205.5f, 511.3f);
            lb_downhill2.transform.eulerAngles = new Vector3(8f, 195f, 1f);

            GameObject tofuSignWood = Instantiate(GameObject.Find("0351_combined_mattex_AlbedoTransparency  [Standard]"));
            tofuSignWood.transform.SetParent(sceneObjects);
            tofuSignWood.name = "TofuSignWood_Downhill2";
            tofuSignWood.transform.position = new Vector3(-290.7f, 205.5f, 498.7f);
            tofuSignWood.transform.eulerAngles = new Vector3(0, 150f, 0);

            GameObject lb_3d_parent = GameObject.Find("LB_3D_DOWN");
            GameObject lb_3d_dowhnill2 = Instantiate(lb_3d_parent);
            lb_3d_dowhnill2.transform.SetParent(lb_3d_parent.transform.GetParent().transform);
            lb_3d_dowhnill2.transform.position = lb_3d_parent.transform.position;
            lb_3d_dowhnill2.name = "LB_3D_DOWN2";

            lb_3d_dowhnill2.GetComponentInChildren<SteamworksLeaderboardList>().Settings = boardIroDH2;
            lb_3d_dowhnill2.SetActive(true);
            lb_downhill2.GetComponent<SR3DLB>().EnableLB = lb_3d_dowhnill2;

            GameObject tofuRadarLogoParent = GameObject.Find("Radar_Logo_Tofu");
            GameObject mapLabel = Instantiate(tofuRadarLogoParent.transform.FindChild("HUD Element Pivot").gameObject);
            mapLabel.transform.SetParent(tofuRadarLogoParent.transform);
            mapLabel.name = "Downhill2_radar";
            mapLabel.transform.position = building.transform.position;
        }

        IEnumerator CreateIrohazakTofuUphill2()
        {
            GameObject tofuZone = Instantiate(GameObject.Find("[Down]Zone_TOFU_Grossiste"));
            tofuZone.name = "[Down]Uphill2_Tofu";
            tofuZone.transform.position = new Vector3(-1320.0f, -289.0f, 238.0f);
            tofuZone.transform.eulerAngles = new Vector3(0.0f, 47.0f, 6.0f);

            SRToffuLivraison tofuDeliveryData = tofuZone.GetComponent<SRToffuLivraison>();
            tofuDeliveryData.location = "Uphill2";
            tofuDeliveryData.LangWelcome = "Sayori: Greetings!";
            tofuDeliveryData.LangDeliveryInProgress = "Sayori: You should finish delivery before coming for more";
            tofuDeliveryData.LangSeeYouSoon = "Sayori: See you again!";

            Transform sceneObjects = GameObject.Find("Scene Objects").transform;

            GameObject building = Instantiate(GameObject.Find("kyoto_style_house_faramiyuki_building"));
            building.transform.SetParent(sceneObjects);
            building.name = "Tofushop_Uphill2";
            building.transform.position = new Vector3(-1316.9f, -291.0f, 242.1f);
            building.transform.eulerAngles = new Vector3(6f, 314f, 1f);

            GameObject lb_uphill2 = Instantiate(GameObject.Find("3D_LB_DOWNHILL"));
            lb_uphill2.transform.SetParent(sceneObjects);
            lb_uphill2.name = "3D_LB_DOWNHILL2";
            lb_uphill2.transform.position = new Vector3(-1302.95f, -294.2f, 248.95f);
            lb_uphill2.transform.eulerAngles = new Vector3(15.1f, 41.7f, 7.2f);

            GameObject tofuSignWood = Instantiate(GameObject.Find("0351_combined_mattex_AlbedoTransparency  [Standard]"));
            tofuSignWood.transform.SetParent(sceneObjects);
            tofuSignWood.name = "TofuSignWood_Uphill2";
            tofuSignWood.transform.position = new Vector3(-1320.2f, -290.8f, 250.7f);
            tofuSignWood.transform.eulerAngles = new Vector3(6.0f, 355.0f, 2.0f);

            GameObject lb_3d_parent = GameObject.Find("LB_3D_UP");
            GameObject lb_3d_uphill2 = Instantiate(lb_3d_parent);
            lb_3d_uphill2.transform.SetParent(lb_3d_parent.transform.GetParent().transform);
            lb_3d_uphill2.transform.position = lb_3d_parent.transform.position;
            lb_3d_uphill2.name = "LB_3D_UP2";

            lb_3d_uphill2.GetComponentInChildren<SteamworksLeaderboardList>().Settings = boardIroUH2;
            lb_3d_uphill2.SetActive(true);
            lb_uphill2.GetComponent<SR3DLB>().EnableLB = lb_3d_uphill2;

            GameObject tofuFinish = Instantiate(tofuDeliveryData.ZoneDarriveEnable);
            tofuDeliveryData.ZoneDarriveEnable = tofuFinish;
            tofuFinish.transform.SetParent(sceneObjects);
            tofuFinish.transform.position = new Vector3(1295.0f, 260.1f, 129.2f);
            tofuFinish.transform.FindChild("Target3D_minimap").transform.position = tofuFinish.transform.position + tofuFinish.transform.TransformDirection(Vector3.up * 50f);

            // delete existing streetlight
            Destroy(GameObject.Find("iroa2019_streetlightremap_obj_22_SUB0"));
            Destroy(GameObject.Find("iroa2019_streetlightremap_obj_22_SUB1"));
            Destroy(GameObject.Find("iroa2019_streetlightremap_obj_22_SUB2"));
            Destroy(GameObject.Find("1WALL_xxiroa2019_streetlightremap_obj_22_sub1"));

            DayNightManager dayNightManager = GameObject.FindObjectOfType<DayNightManager>();
            dayNightManager.Lampadaires = dayNightManager.Lampadaires.Where(val =>
                val.name != "iroa2019_streetlightremap_obj_22_SUB0" &&
                val.name != "iroa2019_streetlightremap_obj_22_SUB1" &&
                val.name != "iroa2019_streetlightremap_obj_22_SUB2" &&
                val.name != "1WALL_xxiroa2019_streetlightremap_obj_22_sub1"
            ).ToArray();

            GameObject tofuRadarLogoParent = GameObject.Find("Radar_Logo_Tofu");
            GameObject mapLabel = Instantiate(tofuRadarLogoParent.transform.FindChild("HUD Element Pivot").gameObject);
            mapLabel.transform.SetParent(tofuRadarLogoParent.transform);
            mapLabel.name = "Uphill2_radar";
            mapLabel.transform.position = building.transform.position;

            // SR_Minimap_Manager_RaceMap doesn't exist at this point so we should wait a little bit
            yield return new WaitForSeconds(2f);
            SR_Minimap_Manager_RaceMap miniManager = GameObject.FindObjectOfType<SR_Minimap_Manager_RaceMap>();
            System.Array.Resize(ref miniManager.FixedGO, miniManager.FixedGO.Length + 1);
            miniManager.FixedGO[miniManager.FixedGO.Length - 1] = tofuFinish.transform.FindChild("Target3D_minimap").gameObject;
        }

        IEnumerator CreateAkinaTofuUphill2()
        {
            GameObject tofuZone = Instantiate(GameObject.Find("Zone_TOFU_Grossiste_down"));
            tofuZone.name = "Zone_TOFU_Grossiste_down2";
            tofuZone.transform.position = new Vector3(-1318.07f, -145.97f, -978f);
            tofuZone.transform.eulerAngles = new Vector3(0.0f, 300.0f, 0.0f);

            SRToffuLivraison tofuDeliveryData = tofuZone.GetComponent<SRToffuLivraison>();
            tofuDeliveryData.location = "Uphill2";
            tofuDeliveryData.LangWelcome = "Riko: Greetings!";
            tofuDeliveryData.LangDeliveryInProgress = "Riko: You should finish delivery before coming for more";
            tofuDeliveryData.LangSeeYouSoon = "Riko: See you again!";

            Transform sceneObjects = GameObject.Find("Scene Objects").transform;

            GameObject building = Instantiate(GameObject.Find("Small House"));
            building.transform.SetParent(sceneObjects);
            building.name = "Tofushop_Uphill2";
            building.transform.position = new Vector3(-1317.5f, -146.7f, -976.4f);
            building.transform.eulerAngles = new Vector3(0f, 230.8f, 358f);

            GameObject lamp;
            lamp = Instantiate(GameObject.Find("Lamp Street Village"));
            lamp.transform.SetParent(sceneObjects);
            lamp.name = "Uphill2_lamp";
            lamp.transform.position = new Vector3(-1316.2f, -147.0f, -981.5f);

            // add the lamp to arrays so it lights up at night too
            DayNightManager dayNightManager = GameObject.FindObjectOfType<DayNightManager>();
            System.Array.Resize(ref dayNightManager.Lampadaires, dayNightManager.Lampadaires.Length + 1);
            dayNightManager.Lampadaires[dayNightManager.Lampadaires.Length - 1] = lamp;
            System.Array.Resize(ref dayNightManager.LittleLight, dayNightManager.LittleLight.Length + 1);
            dayNightManager.LittleLight[dayNightManager.LittleLight.Length - 1] = lamp.transform.FindChild("Spot Light").gameObject;

            GameObject tofuSignWood = Instantiate(GameObject.Find("Panneaux").transform.FindChild("Cube (2)").gameObject);
            tofuSignWood.transform.SetParent(sceneObjects);
            tofuSignWood.name = "TofuSignWood_Uphill2";
            tofuSignWood.transform.position = new Vector3(-1319.1f, -145.5f, -977.75f);
            tofuSignWood.transform.eulerAngles = new Vector3(0f, 50f, 2.1f);
            tofuSignWood.transform.localScale = new Vector3(3.3f, 1.8f, 0.1f);

            GameObject lb_uphill2 = Instantiate(GameObject.Find("3D_LB_DOWNHILL"));
            lb_uphill2.transform.SetParent(sceneObjects);
            lb_uphill2.name = "3D_LB_DOWNHILL2";
            lb_uphill2.transform.position = new Vector3(-1302.24f, -144.8f, -972.5f);
            lb_uphill2.transform.eulerAngles = new Vector3(0f, 57f, 2.1f);

            GameObject lb_3d_parent = GameObject.Find("LB_3D_UP");
            GameObject lb_3d_uphill2 = Instantiate(lb_3d_parent);
            lb_3d_uphill2.transform.SetParent(lb_3d_parent.transform.GetParent().transform);
            lb_3d_uphill2.transform.position = lb_3d_parent.transform.position;
            lb_3d_uphill2.name = "LB_3D_UP2";

            lb_3d_uphill2.GetComponentInChildren<SteamworksLeaderboardList>().Settings = boardAkinaUH2;
            lb_3d_uphill2.SetActive(true);
            lb_uphill2.GetComponent<SR3DLB>().EnableLB = lb_3d_uphill2;

            GameObject tofuFinish = Instantiate(tofuDeliveryData.ZoneDarriveEnable);
            tofuDeliveryData.ZoneDarriveEnable = tofuFinish;
            tofuFinish.transform.SetParent(sceneObjects);
            tofuFinish.transform.position = new Vector3(876.6f, 137.5f, 1222.0f);
            tofuFinish.transform.eulerAngles = new Vector3(0f, 270.0f, 0f);
            Transform minimapIconTransform = tofuFinish.transform.FindChild("Target3D_minimap").transform;
            minimapIconTransform.position = tofuFinish.transform.position + tofuFinish.transform.TransformDirection(Vector3.up * 50f);
            minimapIconTransform.eulerAngles = new Vector3(90f, 80f, 0f);
            tofuFinish.transform.FindChild("Target3D").eulerAngles = new Vector3(0f, 0f, 0f);

            GameObject tofuRadarLogoParent = GameObject.Find("Radar_Logo_Tofu");
            GameObject mapLabel = Instantiate(tofuRadarLogoParent.transform.FindChild("HUD Element Pivot").gameObject);
            mapLabel.transform.SetParent(tofuRadarLogoParent.transform);
            mapLabel.name = "Uphill2_radar";
            mapLabel.transform.position = lamp.transform.position;

            // SR_Minimap_Manager_RaceMap doesn't exist at this point so we should wait a little bit
            yield return new WaitForSeconds(2f);
            SR_Minimap_Manager_RaceMap miniManager = GameObject.FindObjectOfType<SR_Minimap_Manager_RaceMap>();
            System.Array.Resize(ref miniManager.FixedGO, miniManager.FixedGO.Length + 1);
            miniManager.FixedGO[miniManager.FixedGO.Length - 1] = tofuFinish.transform.FindChild("Target3D_minimap").gameObject;
        }

        void InitLeaderboards()
        {
            boardIroDH2 = ScriptableObject.CreateInstance<SteamworksLeaderboardData>();
            boardIroDH2.MaxDetailEntries = 2;
            boardIroDH2.leaderboardName = "IROBESTTIMEDH2";
            boardIroDH2.name = "IROBESTTIMEDH2";
            boardIroDH2.Register();

            SRUIManager uiMgr = GameObject.FindObjectOfType<SRUIManager>();
            GameObject lb_parent = uiMgr.MenuGeneral.transform.FindChild("LB_IroBT").gameObject;
            LB_IroBT2 = Instantiate(lb_parent);
            LB_IroBT2.transform.SetParent(lb_parent.transform.GetParent().transform);
            LB_IroBT2.transform.position = lb_parent.transform.position;
            LB_IroBT2.GetComponent<RectTransform>().anchoredPosition = lb_parent.GetComponent<RectTransform>().anchoredPosition;
            LB_IroBT2.GetComponent<RectTransform>().sizeDelta = lb_parent.GetComponent<RectTransform>().sizeDelta;
            LB_IroBT2.transform.localScale = lb_parent.transform.localScale;
            LB_IroBT2.name = "LB_IroBT2";
            LB_IroBT2.GetComponentInChildren<SteamworksLeaderboardList>().Settings = boardIroDH2;

            boardIroUH2 = ScriptableObject.CreateInstance<SteamworksLeaderboardData>();
            boardIroUH2.MaxDetailEntries = 2;
            boardIroUH2.leaderboardName = "REVERSE2IROBESTTIME";
            boardIroUH2.name = "REVERSE2IROBESTTIME";
            boardIroUH2.Register();

            lb_parent = uiMgr.MenuGeneral.transform.FindChild("LB_IroBTReverse").gameObject;
            LB_IroBTReverse2 = Instantiate(lb_parent);
            LB_IroBTReverse2.transform.SetParent(lb_parent.transform.GetParent().transform);
            LB_IroBTReverse2.transform.position = lb_parent.transform.position;
            LB_IroBTReverse2.GetComponent<RectTransform>().anchoredPosition = lb_parent.GetComponent<RectTransform>().anchoredPosition;
            LB_IroBTReverse2.GetComponent<RectTransform>().sizeDelta = lb_parent.GetComponent<RectTransform>().sizeDelta;
            LB_IroBTReverse2.transform.localScale = lb_parent.transform.localScale;
            LB_IroBTReverse2.name = "LB_IroBTReverse2";
            LB_IroBTReverse2.GetComponentInChildren<SteamworksLeaderboardList>().Settings = boardIroUH2;


            boardAkinaUH2 = ScriptableObject.CreateInstance<SteamworksLeaderboardData>();
            boardAkinaUH2.MaxDetailEntries = 2;
            boardAkinaUH2.leaderboardName = "REVERSE2HARUNABESTTIME";
            boardAkinaUH2.name = "REVERSE2HARUNABESTTIME";
            boardAkinaUH2.Register();

            lb_parent = uiMgr.MenuGeneral.transform.FindChild("LB_HarunaBTReverse").gameObject;
            LB_HarunaBTReverse2 = Instantiate(lb_parent);
            LB_HarunaBTReverse2.transform.SetParent(lb_parent.transform.GetParent().transform);
            LB_HarunaBTReverse2.transform.position = lb_parent.transform.position;
            LB_HarunaBTReverse2.GetComponent<RectTransform>().anchoredPosition = lb_parent.GetComponent<RectTransform>().anchoredPosition;
            LB_HarunaBTReverse2.GetComponent<RectTransform>().sizeDelta = lb_parent.GetComponent<RectTransform>().sizeDelta;
            LB_HarunaBTReverse2.transform.localScale = lb_parent.transform.localScale;
            LB_HarunaBTReverse2.name = "LB_HarunaBTReverse2";
            LB_HarunaBTReverse2.GetComponentInChildren<SteamworksLeaderboardList>().Settings = boardAkinaUH2;


            // menu leaderboards
            Vector2 DELTA_SIZE = new Vector2(120.9f, 103.1f);
            LeaderboardUsersManager lum = GameObject.FindObjectOfType<LeaderboardUsersManager>();

            Transform lb = uiMgr.MenuLeaderboard.transform.FindChild("LB");

            GameObject src = lb.FindChild("IROHAZAKA_downhill").gameObject;
            GameObject iroDownhill2 = Instantiate(src);
            iroDownhill2.transform.SetParent(src.transform.GetParent());
            iroDownhill2.name = "IROHAZAKA_downhill2";
            RectTransform r = iroDownhill2.GetComponent<RectTransform>();
            r.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            r.anchoredPosition = new Vector2(44.0f, -52.0f);
            r.sizeDelta = DELTA_SIZE;
            iroDownhill2.transform.FindChild("AKINA_TITLE").gameObject.GetComponent<Text>().text = "Downhill2";
            Button btn = iroDownhill2.transform.FindChild("Button").gameObject.GetComponent<Button>();
            btn.onClick = new Button.ButtonClickedEvent();
            btn.onClick.AddListener(() => lum.RefreshScore(16));
            btn.onClick.AddListener(uiMgr.DisableOtherbtn);
            btn.onClick.AddListener(() => LB_IroBT2.SetActive(true));
            btn.onClick.AddListener(() => uiMgr.MenuLeaderboard.SetActive(false));
            Button iroDh2Button = btn;
            Button iroDhButton = src.transform.FindChild("Button").gameObject.GetComponent<Button>();

            src = lb.FindChild("IROHAZAKA_uphill").gameObject;
            GameObject iroUphill2 = Instantiate(src);
            iroUphill2.transform.SetParent(src.transform.GetParent());
            iroUphill2.name = "IROHAZAKA_uphill2";
            r = iroUphill2.GetComponent<RectTransform>();
            r.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            r.anchoredPosition = new Vector2(44.0f, -180.3f);
            r.sizeDelta = DELTA_SIZE;
            iroUphill2.transform.FindChild("AKINA_TITLE").gameObject.GetComponent<Text>().text = "Uphill2";
            btn = iroUphill2.transform.FindChild("Button").gameObject.GetComponent<Button>();
            btn.onClick = new Button.ButtonClickedEvent();
            btn.onClick.AddListener(() => lum.RefreshScore(17));
            btn.onClick.AddListener(uiMgr.DisableOtherbtn);
            btn.onClick.AddListener(() => LB_IroBTReverse2.SetActive(true));
            btn.onClick.AddListener(() => uiMgr.MenuLeaderboard.SetActive(false));
            Button iroUh2Button = btn;
            Button iroUhButton = src.transform.FindChild("Button").gameObject.GetComponent<Button>();

            src = lb.FindChild("AKINA_uphil").gameObject;
            GameObject akinaUphill2 = Instantiate(src);
            akinaUphill2.transform.SetParent(src.transform.GetParent());
            akinaUphill2.name = "AKINA_uphill2";
            r = akinaUphill2.GetComponent<RectTransform>();
            r.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            r.anchoredPosition = new Vector2(-254.0f, -180.3f);
            r.sizeDelta = DELTA_SIZE;
            akinaUphill2.transform.FindChild("AKINA_TITLE").gameObject.GetComponent<Text>().text = "Uphill2";
            btn = akinaUphill2.transform.FindChild("AKINA_UPHILL_BTN").gameObject.GetComponent<Button>();
            btn.onClick = new Button.ButtonClickedEvent();
            btn.onClick.AddListener(() => lum.RefreshScore(18));
            btn.onClick.AddListener(uiMgr.DisableOtherbtn);
            btn.onClick.AddListener(() => LB_HarunaBTReverse2.SetActive(true));
            btn.onClick.AddListener(() => uiMgr.MenuLeaderboard.SetActive(false));
            Button akinaUh2Button = btn;
            Button akinaUhButton = src.transform.FindChild("AKINA_UPHILL_BTN").gameObject.GetComponent<Button>();
            Button akagiDhButton = lb.FindChild("AKAGI_downhill").FindChild("AKAGI_DOWNHILL_BTN").gameObject.GetComponent<Button>();
            Button akagiUhButton = lb.FindChild("AKAGI_uphill").FindChild("AKAGI_UPHILL_BTN").gameObject.GetComponent<Button>();

            Navigation newNav = iroDhButton.navigation;
            newNav.selectOnRight = iroDh2Button;
            iroDhButton.navigation = newNav;

            newNav = iroDh2Button.navigation;
            newNav.selectOnLeft = iroDhButton;
            newNav.selectOnRight = akagiDhButton;
            newNav.selectOnDown = iroUh2Button;
            iroDh2Button.navigation = newNav;

            newNav = iroUhButton.navigation;
            newNav.selectOnLeft = akinaUh2Button;
            newNav.selectOnRight = iroUh2Button;
            iroUhButton.navigation = newNav;

            newNav = iroUh2Button.navigation;
            newNav.selectOnLeft = iroUhButton;
            newNav.selectOnUp = iroDh2Button;
            iroUh2Button.navigation = newNav;

            newNav = akinaUhButton.navigation;
            newNav.selectOnRight = akinaUh2Button;
            akinaUhButton.navigation = newNav;

            newNav = akinaUh2Button.navigation;
            newNav.selectOnLeft = akinaUhButton;
            akinaUh2Button.navigation = newNav;

            newNav = akagiDhButton.navigation;
            newNav.selectOnLeft = iroDh2Button;
            akagiDhButton.navigation = newNav;

            newNav = akagiUhButton.navigation;
            newNav.selectOnLeft = iroUh2Button;
            akagiUhButton.navigation = newNav;

            r = lb.FindChild("IROHAZAKA_downhill").gameObject.GetComponent<RectTransform>();
            r.anchoredPosition = new Vector2(-104.4f, -52.1f);
            r.sizeDelta = DELTA_SIZE;

            r = lb.FindChild("IROHAZAKA_uphill").gameObject.GetComponent<RectTransform>();
            r.anchoredPosition = new Vector2(-104.4f, -180.5f);
            r.sizeDelta = DELTA_SIZE;

            r = lb.FindChild("AKINA_downhill").gameObject.GetComponent<RectTransform>();
            r.anchoredPosition = new Vector2(-327.5f, -52.1f);
            r.sizeDelta = new Vector2(242.9f, 103.1f);

            r = lb.FindChild("AKINA_uphil").gameObject.GetComponent<RectTransform>();
            r.anchoredPosition = new Vector2(-403.5f, -180.5f);
            r.sizeDelta = DELTA_SIZE;

            r = lb.FindChild(">BestLvl").gameObject.GetComponent<RectTransform>();
            r.anchoredPosition = new Vector2(-542.0f, 138.8f);
            r.sizeDelta = new Vector2(102.1f, 102.7f);

            r = lb.FindChild(">TOPWIN").gameObject.GetComponent<RectTransform>();
            r.anchoredPosition = new Vector2(-542.0f, -51.8f);
            r.sizeDelta = new Vector2(102.1f, 102.7f);

            r = lb.FindChild(">TOPCHASE").gameObject.GetComponent<RectTransform>();
            r.anchoredPosition = new Vector2(-542.0f, -180.0f);
            r.sizeDelta = new Vector2(102.1f, 102.7f);

            r = lb.FindChild("AKINA_RunCount").gameObject.GetComponent<RectTransform>();
            r.anchoredPosition = new Vector2(-327.6f, 138.6f);
            r.sizeDelta = new Vector2(-997.1f, -478.9f);

            r = lb.FindChild("IROHAZAKA_RunCount").gameObject.GetComponent<RectTransform>();
            r.anchoredPosition = new Vector2(-30.4f, 138.6f);
            r.sizeDelta = new Vector2(-997.1f, -478.9f);
        }

        void SRToffuLivraison_OnTriggerEnter(On.SRToffuLivraison.orig_OnTriggerEnter orig, SRToffuLivraison self, Collider other)
        {
            orig(self, other);
            if (other.GetComponentInParent<RCC_PhotonNetwork>().isMine && !ObscuredPrefs.GetBool("TOFU RUN", false) && PlayerPrefs.GetInt("ImInRun") == 0 && other.gameObject.tag != "ASKBATTLE")
            {
                if (other.gameObject.name != "AskBattleArea")
                {
                    GameObject target3D = self.ZoneDarriveEnable.transform.FindChild("Target3D").gameObject;
                    target3D.SetActive(true);
                    GameObject.FindObjectOfType<SRToffuManager>().LogoTarget2 = target3D;
                    self.ZoneDarriveEnable.transform.FindChild("Target3D_minimap").gameObject.SetActive(true);
                    SRToffuArriver[] arrivers = GameObject.FindObjectsOfType<SRToffuArriver>();
                    foreach (SRToffuArriver arriver in arrivers)
                    {
                        if (self.ZoneDarriveEnable != arriver.gameObject)
                        {
                            arriver.gameObject.SetActive(false);
                        }
                    }

                }
            }
        }

        void SRToffuLivraison_OnTriggerExit(On.SRToffuLivraison.orig_OnTriggerExit orig, SRToffuLivraison self, Collider other)
        {
            orig(self, other);
            if (other.GetComponentInParent<RCC_PhotonNetwork>().isMine && !ObscuredPrefs.GetBool("TOFU RUN", false) && other.gameObject.tag != "ASKBATTLE" && other.gameObject.name != "AskBattleArea")
            {
                self.ZoneDarriveEnable.transform.FindChild("Target3D").gameObject.SetActive(false);
                self.ZoneDarriveEnable.transform.FindChild("Target3D_minimap").gameObject.SetActive(false);
            }
        }

        IEnumerator LeaderboardUsersManager_sendInfo(On.LeaderboardUsersManager.orig_sendInfo orig, LeaderboardUsersManager self)
        {
            yield return orig(self);
            ObscuredInt bestTimeIroDH2 = ObscuredPrefs.GetInt("BestRunTimeIrohazakaDownhill2", 0);
            if (bestTimeIroDH2 != 0 && bestTimeIroDH2 != ObscuredPrefs.GetInt("CheatingTimeTofuIrohazakaDownhill2", 0) && ObscuredPrefs.GetInt("MyIroDH2BT", 0) != bestTimeIroDH2)
            {
                int[] scoreDetails = { ObscuredPrefs.GetInt("UsedCarsIrohazakaDownhill2", 0) };
                ObscuredPrefs.SetInt("MyIroDH2BT", bestTimeIroDH2);
                boardIroDH2.UploadScore(bestTimeIroDH2, scoreDetails, ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodForceUpdate);
            }
            ObscuredInt bestTimeIroUH2 = ObscuredPrefs.GetInt("BestRunTimeIrohazakaUphill2", 0);
            if (bestTimeIroUH2 != 0 && bestTimeIroUH2 != ObscuredPrefs.GetInt("CheatingTimeTofuIrohazakaUphill2", 0) && ObscuredPrefs.GetInt("MyIroUH2BT", 0) != bestTimeIroUH2)
            {
                int[] scoreDetails = { ObscuredPrefs.GetInt("UsedCarsIrohazakaUphill2", 0) };
                ObscuredPrefs.SetInt("MyIroUH2BT", bestTimeIroUH2);
                boardIroUH2.UploadScore(bestTimeIroUH2, scoreDetails, ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodForceUpdate);
            }
            ObscuredInt bestTimeAkinaUH2 = ObscuredPrefs.GetInt("BestRunTimeAkinaUphill2", 0);
            if (bestTimeAkinaUH2 != 0 && bestTimeAkinaUH2 != ObscuredPrefs.GetInt("CheatingTimeTofuAkinaUphill2", 0) && ObscuredPrefs.GetInt("MyAkinaUH2BT", 0) != bestTimeAkinaUH2)
            {
                int[] scoreDetails = { ObscuredPrefs.GetInt("UsedCarsAkinaUphill2", 0) };
                ObscuredPrefs.SetInt("MyAkinaUH2BT", bestTimeAkinaUH2);
                boardAkinaUH2.UploadScore(bestTimeAkinaUH2, scoreDetails, ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodForceUpdate);
            }
        }

        public void LeaderboardUsersManager_Start(On.LeaderboardUsersManager.orig_Start orig, LeaderboardUsersManager self)
        {
            orig(self);
            string resultName = "BestRunTimeIrohazakaDownhill2";
            if (ObscuredPrefs.GetInt(resultName, 0) < 150)
                ObscuredPrefs.SetInt(resultName, 0);
            resultName = "BestRunTimeIrohazakaUphill2";
            if (ObscuredPrefs.GetInt(resultName, 0) < 150)
                ObscuredPrefs.SetInt(resultName, 0);
            resultName = "BestRunTimeAkinaUphill2";
            if (ObscuredPrefs.GetInt(resultName, 0) < 150)
                ObscuredPrefs.SetInt(resultName, 0);
        }

        public void LeaderboardUsersManager_ResetLB(On.LeaderboardUsersManager.orig_ResetLB orig, LeaderboardUsersManager self)
        {
            orig(self);
            boardIroDH2.UploadScore(999, ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodForceUpdate);
            boardIroUH2.UploadScore(999, ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodForceUpdate);
            boardAkinaUH2.UploadScore(999, ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodForceUpdate);
            ObscuredPrefs.SetInt("BestRunTimeIrohazakaDownhill2", 0);
            ObscuredPrefs.SetInt("BestRunTimeIrohazakaUphill2", 0);
            ObscuredPrefs.SetInt("BestRunTimeAkinaUphill2", 0);
        }
    }
}