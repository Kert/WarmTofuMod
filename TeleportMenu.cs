using UnityEngine;
using BepInEx;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
            TP_Uphill2,
            TP_CarDealer,
            TP_Tuning
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
                        case TeleportPoints.TP_CarDealer:
                            pos = new Vector3(1502.4f, 258.6f, 160.6f);
                            rot = new Quaternion(0.0f, -1.0f, 0.0f, -0.1f);
                            break;
                        case TeleportPoints.TP_Tuning:
                            pos = new Vector3(1898.8f, 256.6f, 317.8f);
                            rot = new Quaternion(0.0026f, 0.699f, -0.0014f, 0.7151f);
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
                        //Point for Toshi Light
                        case TeleportPoints.TP_Uphill2:
                            pos = new Vector3(1277.7f, 66.68f, 709.82f);
                            rot = new Quaternion(0.003f, 0.064f, 0.0f, 1.0f);
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

        void CreateTeleportMenu(SRUIManager uiMgr)
        {
            string currentScene = SceneManager.GetActiveScene().name;
            // Change lobby button behaviour
            if (currentScene != "DriftSekai")
            {
                Button lobby = uiMgr.BtnLobby.gameObject.GetComponent<Button>();
                lobby.onClick = new Button.ButtonClickedEvent();

                teleportMenu = Instantiate(uiMgr.MenuExit);
                Transform transform = teleportMenu.transform;
                transform.SetParent(uiMgr.MenuGeneral.transform.GetParent());
                transform.position = uiMgr.BtnLobby.transform.position;
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

                if (currentScene == "Irohazaka" || currentScene == "Akina" || currentScene == "USUI")
                {
                    Button additionalTP = Instantiate(tpButtons[0]);
                    additionalTP.transform.SetParent(tpButtons[0].transform.GetParent().transform);
                    RectTransform t = additionalTP.GetComponent<RectTransform>();
                    t.localScale = new Vector3(1, 1, 1);
                    t.localPosition = new Vector3(200, 0, 0);
                    tpButtons[0].GetComponent<RectTransform>().localPosition = new Vector3(-200, 0, 0);
                    tpButtons[1].GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                    GameObject whiteBG = additionalTP.transform.GetParent().gameObject;
                    whiteBG.GetComponent<RectTransform>().sizeDelta = new Vector2(670.4f, 150.4f);
                    GameObject menuOverlay = whiteBG.transform.GetParent().gameObject;
                    menuOverlay.GetComponent<RectTransform>().sizeDelta = new Vector2(668.9f, 190.6f);

                    additionalTP.onClick = new Button.ButtonClickedEvent();
                    
                    Navigation newNav = new Navigation();

                    if (currentScene == "Irohazaka")
                    {
                        Button additionalTP_CarDealer = Instantiate(tpButtons[0]);
                        Button additionalTP_Tuning = Instantiate(tpButtons[0]);
                        additionalTP_CarDealer.transform.SetParent(tpButtons[0].transform.GetParent().transform);
                        additionalTP_Tuning.transform.SetParent(tpButtons[0].transform.GetParent().transform);
                        RectTransform t_2 = additionalTP_CarDealer.GetComponent<RectTransform>();
                        RectTransform t_3 = additionalTP_Tuning.GetComponent<RectTransform>();

                        t.localPosition = new Vector3(200, 50, 0);
                        tpButtons[0].GetComponent<RectTransform>().localPosition = new Vector3(-200, 50, 0);
                        tpButtons[1].GetComponent<RectTransform>().localPosition = new Vector3(0, 50, 0);

                        t_2.localScale = new Vector3(1, 1, 1);
                        t_2.localPosition = new Vector3(-100, -45, 0);

                        t_3.localScale = new Vector3(1, 1, 1);
                        t_3.localPosition = new Vector3(100, -45, 0);

                        whiteBG.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, -75.0f);
                        whiteBG.GetComponent<RectTransform>().sizeDelta = new Vector2(670.4f, 210.0f);

                        additionalTP_CarDealer.onClick = new Button.ButtonClickedEvent();
                        additionalTP_Tuning.onClick = new Button.ButtonClickedEvent();

                        additionalTP.GetComponentInChildren<Text>().text = "Downhill 2";
                        additionalTP.onClick.AddListener(() => TeleportPlayer(TeleportPoints.TP_Downhill2));
                        additionalTP_CarDealer.GetComponentInChildren<Text>().text = "Car\nDealer";
                        additionalTP_CarDealer.onClick.AddListener(() => TeleportPlayer(TeleportPoints.TP_CarDealer));
                        additionalTP_Tuning.GetComponentInChildren<Text>().text = "Tuning\nShop";
                        additionalTP_Tuning.onClick.AddListener(() => TeleportPlayer(TeleportPoints.TP_Tuning));
                        
                        // Navigation for teleport buttons
                        newNav.mode = Navigation.Mode.Explicit;
                        newNav.selectOnRight = tpButtons[1];
                        newNav.selectOnLeft = additionalTP;
                        newNav.selectOnDown = additionalTP_CarDealer;
                        tpButtons[0].navigation = newNav;

                        newNav = new Navigation();
                        newNav.mode = Navigation.Mode.Explicit;
                        newNav.selectOnLeft = tpButtons[0];
                        newNav.selectOnRight = additionalTP;
                        newNav.selectOnDown = additionalTP_CarDealer;
                        tpButtons[1].navigation = newNav;

                        newNav = new Navigation();
                        newNav.mode = Navigation.Mode.Explicit;
                        newNav.selectOnRight = tpButtons[0];
                        newNav.selectOnLeft = tpButtons[1];
                        newNav.selectOnDown = additionalTP_Tuning;
                        additionalTP.navigation = newNav;

                        newNav = new Navigation();
                        newNav.mode = Navigation.Mode.Explicit;
                        newNav.selectOnUp = tpButtons[0];
                        newNav.selectOnRight = additionalTP_Tuning;
                        newNav.selectOnLeft = additionalTP_Tuning;
                        additionalTP_CarDealer.navigation = newNav;

                        newNav = new Navigation();
                        newNav.mode = Navigation.Mode.Explicit;
                        newNav.selectOnUp = additionalTP;
                        newNav.selectOnRight = additionalTP_CarDealer;
                        newNav.selectOnLeft = additionalTP_CarDealer;
                        additionalTP_Tuning.navigation = newNav;
                    }

                    else if (currentScene == "USUI")
                    {
                        additionalTP.GetComponentInChildren<Text>().text = "Toshi\nLight";
                        additionalTP.onClick.AddListener(() => TeleportPlayer(TeleportPoints.TP_Uphill2));

                        newNav.mode = Navigation.Mode.Explicit;
                        newNav.selectOnLeft = additionalTP;
                        newNav.selectOnRight = tpButtons[1];
                        tpButtons[0].navigation = newNav;

                        newNav = new Navigation();
                        newNav.mode = Navigation.Mode.Explicit;
                        newNav.selectOnLeft = tpButtons[0];
                        newNav.selectOnRight = additionalTP;
                        tpButtons[1].navigation = newNav;

                        newNav.mode = Navigation.Mode.Explicit;
                        newNav.selectOnLeft = tpButtons[1];
                        newNav.selectOnRight = tpButtons[0];
                        additionalTP.navigation = newNav;
                        
                    }

                    else if (currentScene == "Akina")
                    {
                        additionalTP.GetComponentInChildren<Text>().text = "Uphill 2";
                        additionalTP.onClick.AddListener(() => TeleportPlayer(TeleportPoints.TP_Uphill2));

                        newNav.mode = Navigation.Mode.Explicit;
                        newNav.selectOnLeft = additionalTP;
                        newNav.selectOnRight = tpButtons[1];
                        tpButtons[0].navigation = newNav;

                        newNav = new Navigation();
                        newNav.mode = Navigation.Mode.Explicit;
                        newNav.selectOnLeft = tpButtons[0];
                        newNav.selectOnRight = additionalTP;
                        tpButtons[1].navigation = newNav;

                        newNav.mode = Navigation.Mode.Explicit;
                        newNav.selectOnLeft = tpButtons[1];
                        newNav.selectOnRight = tpButtons[0];
                        additionalTP.navigation = newNav;
                    }

                }

                lobby.onClick.AddListener(TeleportMenu);
            }
        }

        void TeleportMenu()
        {
            teleportMenu.SetActive(true);
            teleportMenu.GetComponentInChildren<Button>().Select();
            GameObject.FindObjectOfType<SRUIManager>().MenuGeneral.SetActive(false);
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
