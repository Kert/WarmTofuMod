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
            TP_Downhill2
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
                            pos = new Vector3(-1327.4f, -290.8f, 238.6f);
                            rot = new Quaternion(0.0f, 0.9f, 0.0f, 0.4f);
                            break;
                        case TeleportPoints.TP_Downhill2:
                            pos = new Vector3(-285.0f, 204.4f, 505.8f);
                            rot = new Quaternion(0.0f, 0.0f, 0.0f, -1.0f);
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
            orig(self);
            InitMenuStyles();


            if (SceneManager.GetActiveScene().name == "DriftSekai")
                return;

            // Change lobby button behaviour
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

            if (SceneManager.GetActiveScene().name == "Irohazaka")
            {
                Button dh2 = Instantiate(tpButtons[0]);
                dh2.transform.SetParent(tpButtons[0].transform.GetParent().transform);
                dh2.GetComponentInChildren<Text>().text = "Downhill 2";
                RectTransform t = dh2.GetComponent<RectTransform>();
                t.localScale = new Vector3(1, 1, 1);
                t.localPosition = new Vector3(200, 0, 0);
                tpButtons[0].GetComponent<RectTransform>().localPosition = new Vector3(-200, 0, 0);
                tpButtons[1].GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                GameObject whiteBG = dh2.transform.GetParent().gameObject;
                whiteBG.GetComponent<RectTransform>().sizeDelta = new Vector2(670.4f, 151.4f);
                GameObject menuOverlay = whiteBG.transform.GetParent().gameObject;
                menuOverlay.GetComponent<RectTransform>().sizeDelta = new Vector2(668.9f, 190.6f);

                dh2.onClick = new Button.ButtonClickedEvent();
                dh2.onClick.AddListener(() => TeleportPlayer(TeleportPoints.TP_Downhill2));
                Navigation newNav = new Navigation();
                newNav.mode = Navigation.Mode.Explicit;
                newNav.selectOnLeft = tpButtons[1];
                dh2.navigation = newNav;
                newNav = new Navigation();
                newNav.mode = Navigation.Mode.Explicit;
                newNav.selectOnLeft = tpButtons[0];
                newNav.selectOnRight = dh2;
                tpButtons[1].navigation = newNav;
            }

            lobby.onClick.AddListener(TeleportMenu);
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
