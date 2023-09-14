using UnityEngine;
using BepInEx;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using HeathenEngineering.SteamApi.Foundation;

namespace WarmTofuMod
{
    public partial class WarmTofuMod : BaseUnityPlugin
    {
        GameObject teleportMenu = new GameObject();
        enum TeleportPoints
        {
            TP_Downhill,
            TP_Uphill,
            TP_Downhill2,
            TP_Uphill2
        }

        void TeleportPlayer(TeleportPoints tp)
        {
            Vector3 pos = new();
            Quaternion rot = new();
            string map = SceneManager.GetActiveScene().name;
            switch (map)
            {
                case "Irohazaka":
                    switch (tp)
                    {
                        case TeleportPoints.TP_Downhill:
                            pos = new Vector3(1309.3f, 258.8f, 167.9f);
                            rot = new Quaternion(0.0f, -1.0f, 0.0f, -0.1f);
                            break;
                        case TeleportPoints.TP_Uphill:
                            pos = new Vector3(-1349.0f, -293.6f, 262.6f);
                            rot = new Quaternion(0.0f, 0.9f, 0.0f, 0.4f);
                            break;
                        case TeleportPoints.TP_Downhill2:
                            pos = new Vector3(-295.3f, 204.5f, 534.2f);
                            rot = new Quaternion(0, 1, 0, 0);
                            break;
                    }
                    break;
                case "Akagi":
                    switch (tp)
                    {
                        case TeleportPoints.TP_Downhill:
                            pos = new Vector3(-250.4f, 133.4f, -777.3f);
                            rot = new Quaternion(0.0f, -0.8f, 0.0f, -0.6f);
                            break;
                        case TeleportPoints.TP_Uphill:
                            pos = new Vector3(716.5f, -132.5f, 359.6f);
                            rot = new Quaternion(0.0f, -0.4f, 0.0f, 0.9f);
                            break;
                    }
                    break;
                case "USUI":
                    switch (tp)
                    {
                        case TeleportPoints.TP_Downhill:
                            pos = new Vector3(1316.5f, 66.2f, 777.4f);
                            rot = new Quaternion(0.0f, -0.7f, 0.0f, 0.7f);
                            break;
                        case TeleportPoints.TP_Uphill:
                            pos = new Vector3(-1593.9f, -212.8f, -532.2f);
                            rot = new Quaternion(0.0f, -0.3f, 0.0f, 0.9f);
                            break;
                    }
                    break;
                case "Akina":
                    switch (tp)
                    {
                        case TeleportPoints.TP_Downhill:
                            pos = new Vector3(917.0f, 136.7f, 1359.4f);
                            rot = new Quaternion(0.0f, 1.0f, 0.0f, -0.1f);
                            break;
                        case TeleportPoints.TP_Uphill:
                            pos = new Vector3(-1218.8f, -135.7f, -1187.5f);
                            rot = new Quaternion(0.0f, 1.0f, 0.0f, 0.0f);
                            break;
                        case TeleportPoints.TP_Uphill2:
                            pos = new Vector3(-1328.8f, -146.0f, -992.4f);
                            rot = new Quaternion(0.0f, 0.3f, 0.0f, 1.0f);
                            break;

                    }
                    break;
            }

            RaceManager rm = GameObject.FindWithTag("RaceManager").GetComponent<RaceManager>();
            rm.StopRun();
            rm.ToffuManager.GetComponent<SRToffuManager>().StopDelivery();
            this.StartCoroutine(TeleportPlayerVehicle(pos, rot));
            teleportMenu.SetActive(false);
            prefsInt["MenuOpen"] = 0;
        }

        void TeleportMenu()
        {
            teleportMenu.SetActive(true);
            teleportMenu.GetComponentInChildren<Button>().Select();
            GameObject.FindObjectOfType<SRUIManager>().MenuGeneral.SetActive(false);
        }

        void SRUIManager_Start(On.SRUIManager.orig_Start orig, SRUIManager self)
        {
            InitMenuStyles();

            // fix empty player name in leaderboards
            SteamSettings.GameClient sfm = GameObject.FindObjectOfType<SteamworksFoundationManager>().settings.client;
            sfm.RegisterFriendsSystem(sfm.user);

            self.LB3DUP_HILL.SetActive(true);
            self.LB3DDOWN_HILL.SetActive(true);
            self.LB3DDOWN_HILL.GetComponent<SR3DLB>().EnableLBB(true);
            self.LB3DUP_HILL.GetComponent<SR3DLB>().EnableLBB(true);

            InitLeaderboards();

            string currentScene = SceneManager.GetActiveScene().name;
            // Change lobby button behaviour
            if (currentScene != "DriftSekai")
            {
                Button lobby = self.BtnLobby.gameObject.GetComponent<Button>();
                lobby.onClick = new Button.ButtonClickedEvent();

                teleportMenu = Instantiate(self.MenuExit);
                Transform transform = teleportMenu.transform;
                transform.SetParent(self.MenuGeneral.transform.GetParent());
                transform.position = self.BtnLobby.transform.position;
                transform.localScale = new Vector3(1f, 1f, 1f);

                teleportMenu.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, 0, 0);
                teleportMenu.GetComponentInChildren<Text>().text = "Teleport";
                Button[] tpButtons = teleportMenu.GetComponentsInChildren<Button>();
                tpButtons[0].GetComponentInChildren<Text>().text = "Downhill";
                tpButtons[1].GetComponentInChildren<Text>().text = "Uphill";
                tpButtons[0].onClick = new Button.ButtonClickedEvent();
                tpButtons[1].onClick = new Button.ButtonClickedEvent();
                tpButtons[0].onClick.AddListener(() => TeleportPlayer(TeleportPoints.TP_Downhill));
                tpButtons[1].onClick.AddListener(() => TeleportPlayer(TeleportPoints.TP_Uphill));

                if (currentScene == "Irohazaka")
                {
                    CreateIrohazakTofuDownhill2();
                    StartCoroutine(CreateIrohazakTofuUphill2());
                }
                if (currentScene == "Akina")
                {
                    StartCoroutine(CreateAkinaTofuUphill2());
                }

                if (currentScene == "Irohazaka" || currentScene == "Akina")
                {
                    Button additionalTP = Instantiate(tpButtons[0]);
                    additionalTP.transform.SetParent(tpButtons[0].transform.GetParent().transform);
                    RectTransform t = additionalTP.GetComponent<RectTransform>();
                    t.localScale = new Vector3(1, 1, 1);
                    t.localPosition = new Vector3(200, 0, 0);
                    tpButtons[0].GetComponent<RectTransform>().localPosition = new Vector3(-200, 0, 0);
                    tpButtons[1].GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                    GameObject whiteBG = additionalTP.transform.GetParent().gameObject;
                    whiteBG.GetComponent<RectTransform>().sizeDelta = new Vector2(670.4f, 151.4f);
                    GameObject menuOverlay = whiteBG.transform.GetParent().gameObject;
                    menuOverlay.GetComponent<RectTransform>().sizeDelta = new Vector2(668.9f, 190.6f);

                    additionalTP.onClick = new Button.ButtonClickedEvent();
                    if (currentScene == "Irohazaka")
                    {
                        additionalTP.GetComponentInChildren<Text>().text = "Downhill 2";
                        additionalTP.onClick.AddListener(() => TeleportPlayer(TeleportPoints.TP_Downhill2));
                    }
                    else if (currentScene == "Akina")
                    {
                        additionalTP.GetComponentInChildren<Text>().text = "Uphill 2";
                        additionalTP.onClick.AddListener(() => TeleportPlayer(TeleportPoints.TP_Uphill2));
                    }
                    Navigation newNav = new Navigation();
                    newNav.mode = Navigation.Mode.Explicit;
                    newNav.selectOnLeft = tpButtons[1];
                    additionalTP.navigation = newNav;
                    newNav = new Navigation();
                    newNav.mode = Navigation.Mode.Explicit;
                    newNav.selectOnLeft = tpButtons[0];
                    newNav.selectOnRight = additionalTP;
                    tpButtons[1].navigation = newNav;
                }

                lobby.onClick.AddListener(TeleportMenu);
            }

            orig(self);
            SetLeaderboardBetterParams(self.LB1);
            SetLeaderboardBetterParams(self.LB2);
            SetLeaderboardBetterParams(self.LB3);
            SetLeaderboardBetterParams(self.LB4);
            SetLeaderboardBetterParams(self.LB5);
            SetLeaderboardBetterParams(self.LB6);
            SetLeaderboardBetterParams(self.LB7);
            SetLeaderboardBetterParams(self.LB8);
            SetLeaderboardBetterParams(self.LB9);
            SetLeaderboardBetterParams(self.LB10);
            SetLeaderboardBetterParams(self.LB11);
            SetLeaderboardBetterParams(self.LB12);
            SetLeaderboardBetterParams(self.LB13);
            SetLeaderboardBetterParams(self.LB14);
            SetLeaderboardBetterParams(self.LB15);
            SetLeaderboardBetterParams(LB_IroBT2);
            SetLeaderboardBetterParams(LB_IroBTReverse2);
            SetLeaderboardBetterParams(LB_HarunaBTReverse2);
        }

        void SetLeaderboardBetterParams(GameObject gameObject)
        {
            GameObject scrollView = gameObject.transform.FindChild("Scroll View").gameObject;
            ScrollRect scrollRect = scrollView.GetComponent<ScrollRect>();
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
            scrollRect.scrollSensitivity = 50;
            scrollView.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -4);
        }

        void SRUIManager_OpenMenu(On.SRUIManager.orig_OpenMenu orig, SRUIManager self)
        {
            orig(self);
            self.BtnLobby.GetComponentInChildren<Text>().text = "Teleport";
            teleportMenu.SetActive(false);
            GameObject.FindObjectOfType<SRPlayerListRoom>().PlayerListUI.SetActive(false);
        }

        void SRCusorManager_Update(On.SRCusorManager.orig_Update orig, SRCusorManager self)
        {
            orig(self);
            if (teleportMenu && teleportMenu.activeSelf)
            {
                Cursor.visible = true;
                prefsInt["MenuOpen"] = 1;
            }
        }
    }
}
