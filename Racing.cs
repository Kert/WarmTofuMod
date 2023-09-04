using BepInEx;
using UnityEngine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CodeStage.AntiCheat.Storage;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;
using CodeStage.AntiCheat.ObscuredTypes;

namespace WarmTofuMod
{
    public partial class WarmTofuMod : BaseUnityPlugin
    {
        public class CustomRaceManager : MonoBehaviour
        {
            private CustomRaceManager instance = null;
            private static RCC_CarControllerV3 rival = null;
            public struct BattleSettings
            {
                public string direction = "Downhill";
                public string order = "Parallel";
                public bool nitro = false;
                public bool collision = true;
                BattleSettings(string direction, string order, bool nitro, bool collision)
                {
                    this.direction = direction;
                    this.order = order;
                    this.nitro = nitro;
                    this.collision = collision;
                }
            }
            public static BattleSettings raceSettings;
            public static bool customRaceStarted = false;
            public static bool customRaceInvite = false;
            public static bool isMyInvitation = true;

            CustomRaceManager()
            {
                if (!instance)
                    Debug.Log("Trying to create CustomRaceManager when it already exists");
                instance = this;
            }

            void OnDestroy()
            {
                instance = null;
            }

            public static void SetRival(RCC_CarControllerV3 carController)
            {
                rival = carController;
            }

            public static string GetRivalPlayerName()
            {
                return rival.GetComponentInParent<SRPlayerCollider>().gameObject.transform.GetComponentInChildren<TextMeshPro>()?.text;
            }

            public static string GetRivalPhotonName()
            {
                return rival.name;
            }

            private static string GetPlayerPhotonName()
            {
                return RCC_SceneManager.Instance.activePlayerVehicle.gameObject.GetComponent<SRPlayerCollider>().name;
            }

            public static void SendBattleInvitation()
            {
                customRaceInvite = true;
                isMyInvitation = true;
                //rival.GetComponentInParent<SRPlayerCollider>().SendRaceInvitation(GetRivalPhotonName(), GetPlayerName());
                //Debug.Log("Opened battle options against " + rivalPhotonName + " " + rivalName);
                RaceManager rm = GameObject.FindObjectOfType<RaceManager>();

                typeof(RaceManager).GetField("CibleName", bindingFlags).SetValue(rm, (ObscuredString)GetPlayerPhotonName());
                typeof(RaceManager).GetField("CiblePhotonName", bindingFlags).SetValue(rm, (ObscuredString)GetRivalPhotonName());
                rm.SendRaceInvitation();

                //battleMenu.SetActive(true);
                //battleMenu.GetComponentInChildren<Text>().text = "Battle against " + rivalName;
                //GameObject.FindObjectOfType<SRPlayerListRoom>().PlayerListUI.SetActive(false);
            }

            public static void SendBattleInvitationRPC()
            {
                SRPlayerCollider player = RCC_SceneManager.Instance.activePlayerVehicle.gameObject.GetComponent<SRPlayerCollider>();
                typeof(SRPlayerCollider).GetField("NameTarget", bindingFlags).SetValue(player, GetRivalPhotonName());
                typeof(SRPlayerCollider).GetField("EnvoyeurDelaDemande", bindingFlags).SetValue(player, GetPlayerPhotonName());
                PhotonView view = (PhotonView)typeof(SRPlayerCollider).GetField("view", bindingFlags).GetValue(player);

                Debug.Log("sending battle invitation MY BATTLE ");

                string order = raceSettings.order;
                if (order == "You lead")
                    order = "You chase";
                else if (order == "You chase")
                    order = "You lead";

                view.RPC("ReceiveBattleInvitationRPC", RpcTarget.All, new object[]
                {
                    GetRivalPhotonName(),
                    PlayerPrefs.GetString("PLAYERNAMEE"),
                    GetPlayerPhotonName(),
                    raceSettings.direction,
                    order,
                    raceSettings.nitro,
                    raceSettings.collision
                });
            }

            public static void RaceEnd()
            {
                customRaceStarted = false;
            }
        }

        void SRPlayerCollider_SendRaceInvitation(On.SRPlayerCollider.orig_SendRaceInvitation orig, SRPlayerCollider self, string rivalPhotonName, string playerName)
        {
            if (CustomRaceManager.customRaceInvite)
                CustomRaceManager.SendBattleInvitationRPC();
            else
                orig(self, rivalPhotonName, playerName);
        }

        void RaceManager_ShowMyInvitation(On.RaceManager.orig_ShowMyInvitation orig, RaceManager self, string Sender, string EnvoyeurDelaDemande)
        {
            orig(self, Sender, EnvoyeurDelaDemande);
            if (CustomRaceManager.customRaceInvite)
            {
                CustomRaceManager.BattleSettings battleSettings = CustomRaceManager.raceSettings;
                self.RaceNotification.GetComponentInChildren<Text>().text = "<color=#B95353>" + Sender + "</color> \n" + self.ChallengeYou + "\n" +
                battleSettings.direction + ". " + battleSettings.order + "\n" + "Nitro: " + (battleSettings.nitro ? "Yes" : "No") + " Collision: " + (battleSettings.collision ? "Yes" : "No");
                Debug.Log("Battle settings set for receiver");
            }
        }

        void RaceManager_LetsGoRaceP1(On.RaceManager.orig_LetsGoRaceP1 orig, RaceManager self)
        {
            if (CustomRaceManager.customRaceInvite)
                ApplyBattleSettings();
            orig(self);
        }

        void RaceManager_LetsGoRaceP2(On.RaceManager.orig_LetsGoRaceP2 orig, RaceManager self)
        {
            if (CustomRaceManager.customRaceInvite)
                ApplyBattleSettings();
            orig(self);
        }

        void RaceManager_OtherLeaveRun(On.RaceManager.orig_OtherLeaveRun orig, RaceManager self)
        {
            orig(self);
            CustomRaceManager.RaceEnd();
        }

        void RaceManager_StopRun(On.RaceManager.orig_StopRun orig, RaceManager self)
        {
            orig(self);
            CustomRaceManager.RaceEnd();
        }

        void RaceManager_FinishFirst(On.RaceManager.orig_FinishFirst orig, RaceManager self)
        {
            orig(self);
            CustomRaceManager.RaceEnd();
        }

        void RaceManager_FinishSecond(On.RaceManager.orig_FinishSecond orig, RaceManager self)
        {
            orig(self);
            CustomRaceManager.RaceEnd();
        }

        struct RacePositionsData
        {
            public Vector3 posP1;
            public Quaternion rotP1;
            public Vector3 posP2;
            public Quaternion rotP2;
            public Vector3 pos_lead;
            public Quaternion rot_lead;
            public Vector3 posFinish;
            public Quaternion rotFinish;
            public RacePositionsData(Vector3 _posP1, Quaternion _rotP1, Vector3 _posP2, Quaternion _rotP2,
                Vector3 _pos_lead, Quaternion _rot_lead, Vector3 _posFinish, Quaternion _rotFinish)
            {
                posP1 = _posP1;
                rotP1 = _rotP1;
                posP2 = _posP2;
                rotP2 = _rotP2;
                pos_lead = _pos_lead;
                rot_lead = _rot_lead;
                posFinish = _posFinish;
                rotFinish = _rotFinish;
            }
        }

        Dictionary<string, Dictionary<string, RacePositionsData>> RacePositionData = new Dictionary<string, Dictionary<string, RacePositionsData>>
        {
            { "Irohazaka", new Dictionary<string, RacePositionsData>
                {
                    { "Downhill", new RacePositionsData(
                        new Vector3(1232.7f, 260.2f, 55.5f), new Quaternion(0.0f, 1.0f, 0.0f, -0.3f), new Vector3(1230.2f, 260.2f, 57.0f), new Quaternion(0.0f, 1.0f, 0.0f, -0.3f),
                        new Vector3(1232.7f, 260.2f, 59.0f), new Quaternion(0.0f, -1.0f, 0.0f, 0.3f),
                        new Vector3(-1321.0f, -289.0f, 218.2f), new Quaternion(0.0f, 0.4f, 0.0f, -0.9f)
                    )},
                    { "Uphill", new RacePositionsData(
                        new Vector3(-1223.2f, -280.6f, 132.5f), new Quaternion(0.0f, 0.9f, 0.0f, 0.4f), new Vector3(-1226.2f, -280.6f, 129.5f), new Quaternion(0.0f, 0.9f, 0.0f, 0.4f),
                        new Vector3(-1183.1f, -276.4f, 96.1f), new Quaternion(0.0f, 0.9f, 0.0f, 0.4f),
                        new Vector3(1232.0f, 260.0f, 55.0f), new Quaternion(0.0f, 0.3f, 0.0f, 1.0f)
                    )},
                    { "Downhill 2", new RacePositionsData(
                        new Vector3(-216.9f, 203.8f, 560.0f), new Quaternion(0.0f, -0.8f, 0.0f, -0.7f), new Vector3(-217.9f, 203.8f, 555.0f), new Quaternion(0.0f, -0.8f, 0.0f, -0.7f),
                        new Vector3(-216.3f, 203.7f, 560.1f), new Quaternion(0.0f, 0.7f, 0.0f, 0.7f),
                        new Vector3(-1321.0f, -289.0f, 218.2f), new Quaternion(0.0f, 0.4f, 0.0f, -0.9f)
                    )},
                    { "Uphill 2", new RacePositionsData(
                        new Vector3(-1303.1f, -287.6f, 211.7f), new Quaternion(0.0f, 0.9f, 0.0f, 0.4f), new Vector3(-1305.9f, -287.6f, 207.6f), new Quaternion(0.0f, 0.9f, 0.0f, 0.4f),
                        new Vector3(-1303.1f, -287.6f, 211.7f), new Quaternion(0.0f, 0.9f, 0.0f, 0.4f),
                        new Vector3(-234.0f, 203.0f, 560.0f), new Quaternion(0.0f, 0.7f, 0.0f, 0.7f)
                    )}
                }
            },
            { "Akina", new Dictionary<string, RacePositionsData>
                {
                    { "Downhill", new RacePositionsData(
                        new Vector3(915.5f, 136.5f, 1323.3f), new Quaternion(0.0f, 1.0f, 0.0f, -0.1f), new Vector3(912.1f, 136.5f, 1323.7f), new Quaternion(0.0f, 1.0f, 0.0f, -0.1f),
                        new Vector3(915.5f, 136.5f, 1323.3f), new Quaternion(0.0f, 1.0f, 0.0f, -0.1f),
                        new Vector3(-1369.4f, -146.0f, -1083.6f), new Quaternion(0.0f, 0.3f, 0.0f, 1.0f)
                    )},
                    { "Uphill", new RacePositionsData(
                        new Vector3(-1372.4f, -145.4f, -1081.3f), new Quaternion(0.0f, 0.2f, 0.0f, 1.0f), new Vector3(-1369.9f, -145.4f, -1082.9f), new Quaternion(0.0f, 0.2f, 0.0f, 1.0f),
                        new Vector3(-1372.4f, -145.4f, -1081.3f), new Quaternion(0.0f, 0.2f, 0.0f, 1.0f),
                        new Vector3(912.4f, 137.0f, 1323.0f), new Quaternion(0.0f, 0.2f, 0.0f, 1.0f)
                    )}
                }
            },
            { "Akagi", new Dictionary<string, RacePositionsData>
                {
                    { "Downhill", new RacePositionsData(
                        new Vector3(-124.9f, 125.8f, -748.6f), new Quaternion(0.0f, -0.4f, 0.0f, -0.9f), new Vector3(-121.6f, 125.9f, -751.6f), new Quaternion(0.0f, -0.4f, 0.0f, -0.9f),
                        new Vector3(-124.9f, 125.8f, -748.6f), new Quaternion(0.0f, -0.4f, 0.0f, -0.9f),
                        new Vector3(693.2f, -134.5f, 320.0f), new Quaternion(0.0f, 0.4f, 0.0f, 0.9f)
                    )},
                    { "Uphill", new RacePositionsData(
                        new Vector3(693.9f, -134.6f, 320.5f), new Quaternion(0.0f, 0.4f, 0.0f, -0.9f), new Vector3(696.1f, -134.5f, 324.5f), new Quaternion(0.0f, 0.4f, 0.0f, -0.9f),
                        new Vector3(693.9f, -134.6f, 320.5f), new Quaternion(0.0f, 0.4f, 0.0f, -0.9f),
                        new Vector3(-124.2f, 125.5f, -748.0f), new Quaternion(0.0f, 0.4f, 0.0f, 0.9f)
                    )}
                }
            },
            { "USUI", new Dictionary<string, RacePositionsData>
                {
                    { "Downhill", new RacePositionsData(
                        new Vector3(1194.4f, 66.3f, 780.0f), new Quaternion(0.0f, -0.8f, 0.0f, 0.5f), new Vector3(1192.8f, 66.4f, 784.4f), new Quaternion(0.0f, -0.8f, 0.0f, 0.6f),
                        new Vector3(1194.4f, 66.3f, 780.0f), new Quaternion(0.0f, -0.8f, 0.0f, 0.5f),
                        new Vector3(-1554.9f, -211.4f, -711.5f), new Quaternion(0.0f, 0.7f, 0.0f, -0.7f)
                    )},
                    { "Uphill", new RacePositionsData(
                        new Vector3(-1554.5f, -213.4f, -745.5f), new Quaternion(0.0f, 0.0f, 0.0f, 1.0f), new Vector3(-1550.4f, -213.3f, -745.6f), new Quaternion(0.0f, 0.0f, 0.0f, 1.0f),
                        new Vector3(-1554.5f, -213.4f, -745.5f), new Quaternion(0.0f, 0.0f, 0.0f, 1.0f),
                        new Vector3(1270.0f, 68.4f, 784.5f), new Quaternion(0.0f, 0.0f, 0.0f, 1.0f)
                    )}
                }
            }
        };

        public void ApplyBattleSettings()
        {
            CustomRaceManager.BattleSettings battleSettings = CustomRaceManager.raceSettings;

            var mapData = RacePositionData[SceneManager.GetActiveScene().name][battleSettings.direction];
            RaceManager rm = GameObject.FindObjectOfType<RaceManager>();

            switch (battleSettings.order)
            {
                case "Parallel":
                    rm.StartingPointP1.position = mapData.posP1;
                    rm.StartingPointP1.rotation = mapData.rotP1;
                    rm.StartingPointP2.position = mapData.posP2;
                    rm.StartingPointP2.rotation = mapData.rotP2;
                    break;
                case "You lead":
                    rm.StartingPointP1.position = mapData.pos_lead;
                    rm.StartingPointP1.rotation = mapData.rot_lead;
                    rm.StartingPointP2.position = mapData.pos_lead + rm.StartingPointP1.TransformDirection(Vector3.back * 10f);
                    rm.StartingPointP2.rotation = rm.StartingPointP1.rotation;
                    break;
                case "You chase":
                    rm.StartingPointP2.position = mapData.pos_lead;
                    rm.StartingPointP2.rotation = mapData.rot_lead;
                    rm.StartingPointP1.position = mapData.pos_lead + rm.StartingPointP2.TransformDirection(Vector3.back * 10f);
                    rm.StartingPointP1.rotation = rm.StartingPointP2.rotation;
                    break;
            }
            Debug.Log("IS MY INVITATION " + CustomRaceManager.isMyInvitation);
            if (!CustomRaceManager.isMyInvitation && battleSettings.order != "Parallel")
            {
                var tempP = rm.StartingPointP1.position;
                var tempR = rm.StartingPointP1.rotation;
                rm.StartingPointP1.position = rm.StartingPointP2.position;
                rm.StartingPointP1.rotation = rm.StartingPointP2.rotation;
                rm.StartingPointP2.position = tempP;
                rm.StartingPointP2.rotation = tempR;
                Debug.Log("I AM THE RECEIVER YOU KNOW IT?");
            }
            FinishZone fz = GameObject.FindObjectOfType<FinishZone>();

            fz.transform.SetPositionAndRotation(mapData.posFinish, mapData.rotFinish);
            if (SceneManager.GetActiveScene().name == "Akagi")
            {
                fz.gameObject.transform.GetChild(1).localPosition = new Vector3(0f, 0f, 0f);
                fz.gameObject.transform.GetChild(1).localEulerAngles = new Vector3(90f, 90f, 0f);
            }
            else
            {
                fz.gameObject.transform.GetChild(0).transform.GetChild(0).localPosition = new Vector3(0f, 0f, 0f);
                fz.gameObject.transform.GetChild(0).transform.GetChild(0).localEulerAngles = new Vector3(90f, 90f, 0f);
            }

            //fz.transform.rotation = ;
            //fz.gameObject.transform.GetChild(0).transform.GetChild(0).position = mapData.posFinish;
            //Destroy(fz.gameObject.transform.GetChild(0).gameObject);
            //HUDNavigationElement woo = fz.gameObject.AddComponent<HUDNavigationElement>();
            //Destroy(fz.gameObject.GetComponentInChildren<HUDNavigationElement>());
            //fz.gameObject.transform.GetChild(0).gameObject.AddComponent<HUDNavigationElement>();
        }

        void SRPlayerListRoom_Update(On.SRPlayerListRoom.orig_Update orig, SRPlayerListRoom self)
        {
            if (Input.GetKeyDown(KeyCode.Tab) ||
            (Input.GetKeyDown(KeyCode.Joystick1Button8) && ObscuredPrefs.GetBool("TooglePlayerListXbox", false) && PlayerPrefs.GetString("ControllerTypeChoose") == "Xbox360One") ||
            (Input.GetButtonDown("PS4_ClickLeft") && ObscuredPrefs.GetBool("TooglePlayerListXbox", false) && PlayerPrefs.GetString("ControllerTypeChoose") == "PS4"))
            {
                self.PlayerListUI.SetActive(!self.PlayerListUI.activeSelf);
                if (self.PlayerListUI.activeSelf)
                {
                    self.SteamIcon();
                    self.PlayerListing();
                }
                else
                    battleMenu.SetActive(false);
            }
        }

        GameObject battleGameObject = new GameObject();
        List<GameObject> playerListItems = new();
        GameObject battleMenu = new GameObject();
        void SRPlayerListRoom_Start(On.SRPlayerListRoom.orig_Start orig, SRPlayerListRoom self)
        {
            orig(self);
            CustomRaceManager warmTofuBattleManager = self.gameObject.AddComponent<CustomRaceManager>();

            RectTransform rect;

            // Create Battle Menu
            battleMenu = Instantiate(Instantiate(GameObject.FindObjectOfType<SRUIManager>().MenuExit));
            battleMenu.name = "Battle Settings";
            Transform transform = battleMenu.transform;
            transform.SetParent(self.PlayerListUI.transform);
            transform.position = new Vector3(0, 0, 0);
            transform.localScale = new Vector3(1f, 1f, 1f);

            battleMenu.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, 0, 0);
            battleMenu.GetComponentInChildren<Text>().text = "Battle settings";
            Button[] battleButtons = battleMenu.GetComponentsInChildren<Button>();
            //Destroy(battleButtons[0].gameObject);
            Destroy(battleButtons[1].gameObject);
            battleButtons[0].gameObject.GetComponentInChildren<Text>().text = "Battle player";
            battleButtons[0].onClick = new Button.ButtonClickedEvent();
            battleButtons[0].onClick.AddListener(SetBattleSettings);
            battleButtons[0].onClick.AddListener(() => self.PlayerListUI.SetActive(false));
            battleButtons[0].onClick.AddListener(() => battleMenu.SetActive(false));
            battleButtons[0].onClick.AddListener(CustomRaceManager.SendBattleInvitation);
            rect = battleButtons[0].GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.6f, 0.5f);
            rect.anchorMax = new Vector2(0.78f, 0.5f);
            rect.anchoredPosition = new Vector2(0f, -110f);

            Dropdown dummyDropdown = GameObject.FindObjectOfType<SRUIManager>().MenuSettings.GetComponentInChildren<Dropdown>();
            Toggle dummyToggle = GameObject.FindObjectOfType<SRUIManager>().MenuSettings.GetComponentInChildren<Toggle>();

            Dropdown trackDropdown = Instantiate(dummyDropdown);
            Dropdown posDropdown = Instantiate(dummyDropdown);
            Toggle nitroToggle = Instantiate(dummyToggle);
            Toggle collisionToggle = Instantiate(dummyToggle);
            nitroToggle.onValueChanged = new Toggle.ToggleEvent();
            collisionToggle.onValueChanged = new Toggle.ToggleEvent();
            trackDropdown.transform.SetParent(battleMenu.transform);
            Destroy(trackDropdown.gameObject.transform.FindChild("EnTutoPS4").gameObject);
            posDropdown.transform.SetParent(battleMenu.transform);
            Destroy(posDropdown.gameObject.transform.FindChild("EnTutoPS4").gameObject);
            nitroToggle.transform.SetParent(battleMenu.transform);
            collisionToggle.transform.SetParent(battleMenu.transform);
            trackDropdown.name = "TrackDropdown";
            posDropdown.name = "OrderDropdown";
            nitroToggle.name = "NitroToggle";
            collisionToggle.name = "CollisionToggle";

            rect = trackDropdown.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition3D = new Vector3(-335f, 100, 0);
            rect.sizeDelta = new Vector2(450f, 100f);

            rect = posDropdown.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition3D = new Vector3(-25f, 100, 0);
            rect.sizeDelta = new Vector2(450f, 100f);

            rect = nitroToggle.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition3D = new Vector3(300f, 100, 0);
            rect.sizeDelta = new Vector2(280f, 85f);

            rect = collisionToggle.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition3D = new Vector3(300f, 0, 0);
            rect.sizeDelta = new Vector2(280f, 85f);

            rect = battleMenu.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.2f, 0.1f);
            rect.anchorMax = new Vector2(0.8f, 0.7f);
            rect.anchoredPosition3D = new Vector3(0, 0, 0);
            rect.sizeDelta = new Vector2(490f, 150f);

            rect = battleMenu.transform.FindChild("White_BG").gameObject.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.2f, 0.22f);
            rect.anchorMax = new Vector2(0.8f, 0.68f);
            rect.anchoredPosition3D = new Vector3(0, 0, 0);
            rect.sizeDelta = new Vector2(410f, 200f);

            nitroToggle.gameObject.GetComponentInChildren<Text>().text = "Nitro";
            collisionToggle.gameObject.GetComponentInChildren<Text>().text = "Collision";

            posDropdown.ClearOptions();
            posDropdown.AddOptions(new List<string> { "Parallel", "You lead", "You chase" });

            trackDropdown.ClearOptions();
            List<string> trackList = RacePositionData[SceneManager.GetActiveScene().name].Keys.ToList();
            trackDropdown.AddOptions(trackList);
            trackDropdown.template.sizeDelta = new Vector2(0, 500f);


            playerListItems.Clear();
            Text[] texts = self.PlayerListUI.GetComponentsInChildren<Text>();
            foreach (Text text in texts)
            {
                if (text.gameObject.name == "P1 (0)")
                {
                    Button smallButton = Instantiate(GameObject.FindObjectOfType<SRUIManager>().BtnLobby.GetComponent<Button>());
                    smallButton.GetComponentInChildren<Text>().text = "Battle";

                    BattleHover bh = smallButton.gameObject.AddComponent<BattleHover>();
                    smallButton.transform.SetParent(text.gameObject.transform);
                    RectTransform r = smallButton.GetComponent<RectTransform>();
                    r.anchoredPosition = new Vector3(120, 0, 0);
                    r.anchorMin = r.anchorMax = new Vector2(0, 0.5f);
                    r.sizeDelta = new Vector2(260f, 27f);
                    r.localScale = new Vector3(1, 1, 1);
                    Text t = smallButton.GetComponentInChildren<Text>();
                    t.fontSize = t.resizeTextMaxSize = 20;
                    t.resizeTextMinSize = 1;
                    playerListItems.Add(smallButton.gameObject);
                    smallButton.onClick = new Button.ButtonClickedEvent();
                    smallButton.onClick.AddListener(() => BattleOptions(bh.rival));
                }
            }
        }

        void SetBattleSettings()
        {
            GameObject battleOptions = battleMenu;
            Dropdown[] dropdowns = battleOptions.GetComponentsInChildren<Dropdown>();
            foreach (Dropdown dropdown in dropdowns)
            {
                if (dropdown.name == "TrackDropdown")
                {
                    CustomRaceManager.raceSettings.direction = dropdown.options[dropdown.value].text;
                }
                else if (dropdown.name == "OrderDropdown")
                {
                    CustomRaceManager.raceSettings.order = dropdown.options[dropdown.value].text;
                }
            }

            Toggle[] toggles = battleOptions.GetComponentsInChildren<Toggle>();
            foreach (Toggle toggle in toggles)
            {
                if (toggle.name == "NitroToggle")
                {
                    CustomRaceManager.raceSettings.nitro = toggle.isOn;
                }
                else if (toggle.name == "CollisionToggle")
                {
                    CustomRaceManager.raceSettings.collision = toggle.isOn;
                }
            }
        }

        void BattleOptions(RCC_CarControllerV3 carControllerV3)
        {
            Debug.Log("rival is " + carControllerV3);
            CustomRaceManager.SetRival(carControllerV3);

            Debug.Log("Opened battle options against " + CustomRaceManager.GetRivalPhotonName() + " " + CustomRaceManager.GetRivalPlayerName());
            battleMenu.SetActive(true);
            battleMenu.GetComponentInChildren<Text>().text = "Battle against " + CustomRaceManager.GetRivalPlayerName();
            //GameObject.FindObjectOfType<SRPlayerListRoom>().PlayerListUI.SetActive(false);
        }

        public class BattleHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
        {
            public string playerName;
            private Text hoverText;
            public bool hovered;
            RectTransform rect;
            public RCC_CarControllerV3 rival = null;
            void Start()
            {
                rect = gameObject.GetComponent<RectTransform>();
                hoverText = GetComponentInChildren<Text>();
                playerName = gameObject.transform.GetParent().GetComponent<Text>().text;
                hoverText.text = playerName;
                hovered = false;
            }

            public void OnPointerEnter(PointerEventData eventData)
            {
                hoverText.text = "Battle\n" + playerName;
                rect.anchorMax = new Vector2(rect.anchorMax.x, 1.1f);
                hovered = true;
            }

            public void OnPointerExit(PointerEventData eventData)
            {
                hoverText.text = playerName;
                rect.anchorMax = new Vector2(rect.anchorMax.x, 0.5f);
                hovered = false;
            }
        }

        void UpdateBattleHovers()
        {
            string playerPhotonName = RCC_SceneManager.Instance.activePlayerVehicle.gameObject.GetComponent<SRPlayerCollider>().name;
            foreach (GameObject playerItem in playerListItems)
            {

                playerItem.SetActive(true);
                BattleHover asd = playerItem.gameObject.GetComponent<BattleHover>();
                asd.rival = RCC_SceneManager.Instance.activePlayerVehicle.gameObject.GetComponent<RCC_CarControllerV3>();
                continue;

                SRCheckOtherPlayerCam obj = playerItem.transform.GetParent().transform.GetParent().GetComponentInChildren<SRCheckOtherPlayerCam>();
                if (!obj.MyTargetPlayer_Go)
                {
                    playerItem.SetActive(false);
                    continue;
                }
                RCC_CarControllerV3 photonCar = (RCC_CarControllerV3)typeof(SRCheckOtherPlayerCam).GetField("jack", bindingFlags).GetValue(obj);
                string photonName = obj.MyTargetPlayer_Go.name;
                if (!photonCar || photonName == playerPhotonName)
                {
                    playerItem.SetActive(false);
                    continue;
                }

                // disable everyone who is not warmtofu
                Text buttonText = playerItem.gameObject.GetComponentInChildren<Text>();
                if (!NetworkTest.PlayerHasMod(photonName))
                    buttonText.color = Color.blue;
                else
                    buttonText.color = Color.yellow;

                BattleHover bh = playerItem.gameObject.GetComponent<BattleHover>();
                buttonText.text = bh.playerName = playerItem.gameObject.transform.GetParent().GetComponent<Text>().text;
                if (bh.hovered)
                    buttonText.text = "Battle\n" + buttonText.text;
                if (playerItem.transform.GetParent().transform.GetParent().GetComponentInChildren<Image>().enabled)
                    playerItem.SetActive(true);
                bh.rival = photonCar;
            }
        }

        void SRPlayerListRoom_PlayerListing(On.SRPlayerListRoom.orig_PlayerListing orig, SRPlayerListRoom self)
        {
            orig(self);
            self.StartCoroutine(PlayerListUpdate(self));
        }

        void SRPlayerListRoom_PlayerListingRefresh(On.SRPlayerListRoom.orig_PlayerListingRefresh orig, SRPlayerListRoom self)
        {
            orig(self);
            UpdateBattleHovers();
        }

        IEnumerator PlayerListUpdate(SRPlayerListRoom sRPlayerListRoom)
        {
            while (true)
            {
                sRPlayerListRoom.PlayerListingRefresh();
                yield return new WaitForSeconds(1);
                if (!sRPlayerListRoom.PlayerListUI.active)
                    break;
            }
            yield break;
        }

        IEnumerator RaceManager_DecompteRunningIE(On.RaceManager.orig_DecompteRunningIE orig, RaceManager self)
        {
            yield return orig(self);
            if (CustomRaceManager.customRaceInvite)
            {
                CustomRaceManager.customRaceInvite = false;
                CustomRaceManager.customRaceStarted = true;
            }
            if (CustomRaceManager.customRaceStarted && CustomRaceManager.raceSettings.collision == true)
            {
                RCC_SceneManager.Instance.activePlayerVehicle.gameObject.GetComponentInParent<SRPlayerCollider>().AppelRPCSetGhostModeV2(8);
            }
        }

        void SRNosManager_Update(On.SRNosManager.orig_Update orig, SRNosManager self)
        {
            if (CustomRaceManager.customRaceStarted && !CustomRaceManager.raceSettings.nitro)
                return;
            else
                orig(self);

        }

        void RCC_CarControllerV3_Inputs(On.RCC_CarControllerV3.orig_Inputs orig, RCC_CarControllerV3 self)
        {
            orig(self);
            if (CustomRaceManager.customRaceStarted && !CustomRaceManager.raceSettings.nitro)
                self.boostInput = 0f;
        }
    }
}