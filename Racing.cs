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
                if (instance)
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
            ChangeRacePositionData();
            orig(self);
        }

        void RaceManager_LetsGoRaceP2(On.RaceManager.orig_LetsGoRaceP2 orig, RaceManager self)
        {
            ChangeRacePositionData();
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

        public void ChangeRacePositionData()
        {
            RaceManager rm = GameObject.FindObjectOfType<RaceManager>();
            if (!CustomRaceManager.customRaceInvite)
            {
                var defaultData = defaultRaceData[SceneManager.GetActiveScene().name];
                rm.StartingPointP1.position = defaultData.posP1;
                rm.StartingPointP1.rotation = defaultData.rotP1;
                rm.StartingPointP2.position = defaultData.posP2;
                rm.StartingPointP2.rotation = defaultData.rotP2;
                GameObject.FindObjectOfType<FinishZone>().transform.SetPositionAndRotation(defaultData.posFinish, defaultData.rotFinish);
                return;
            }

            CustomRaceManager.BattleSettings battleSettings = CustomRaceManager.raceSettings;
            var mapData = customRaceData[SceneManager.GetActiveScene().name][battleSettings.direction];
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

        public class CustomRaceMenu : MonoBehaviour
        {
            public static GameObject menu;
            private static Button start;
            private static GameObject parentUI;
            private static Dropdown trackDropdown;
            private static Dropdown posDropdown;
            private static Toggle nitroToggle;
            private static Toggle collisionToggle;
            private static List<string> trackList;

            public CustomRaceMenu(GameObject parent)
            {
                Debug.Log("doing stuff");
                menu = Instantiate(Instantiate(GameObject.FindObjectOfType<SRUIManager>().MenuExit));
                menu.name = "Custom Race Menu";

                Debug.Log("doing ui");
                parentUI = parent.GetComponent<SRPlayerListRoom>().PlayerListUI;
                Transform transform = menu.transform;
                transform.SetParent(parentUI.transform);
                transform.position = new Vector3(0, 0, 0);
                transform.localScale = new Vector3(1f, 1f, 1f);

                Debug.Log("doing text");
                menu.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, 0, 0);
                menu.GetComponentInChildren<Text>().text = "Battle settings";

                Debug.Log("doing buttons");
                Button[] battleButtons = menu.GetComponentsInChildren<Button>();
                Destroy(battleButtons[1].gameObject);
                start = battleButtons[0];

                start.gameObject.GetComponentInChildren<Text>().text = "Send race invitation";
                start.onClick = new Button.ButtonClickedEvent();
                start.onClick.AddListener(SetBattleSettings);
                start.onClick.AddListener(() => parentUI.SetActive(false));
                start.onClick.AddListener(() => menu.SetActive(false));
                start.onClick.AddListener(CustomRaceManager.SendBattleInvitation);

                RectTransform rect = start.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0.6f, 0.5f);
                rect.anchorMax = new Vector2(0.78f, 0.5f);
                rect.anchoredPosition = new Vector2(0f, -110f);

                Dropdown dummyDropdown = GameObject.FindObjectOfType<SRUIManager>().MenuSettings.GetComponentInChildren<Dropdown>();
                Toggle dummyToggle = GameObject.FindObjectOfType<SRUIManager>().MenuSettings.GetComponentInChildren<Toggle>();

                trackDropdown = Instantiate(dummyDropdown);
                posDropdown = Instantiate(dummyDropdown);
                nitroToggle = Instantiate(dummyToggle);
                collisionToggle = Instantiate(dummyToggle);
                nitroToggle.onValueChanged = new Toggle.ToggleEvent();
                collisionToggle.onValueChanged = new Toggle.ToggleEvent();
                trackDropdown.transform.SetParent(menu.transform);
                Destroy(trackDropdown.gameObject.transform.FindChild("EnTutoPS4").gameObject);
                posDropdown.transform.SetParent(menu.transform);
                Destroy(posDropdown.gameObject.transform.FindChild("EnTutoPS4").gameObject);
                nitroToggle.transform.SetParent(menu.transform);
                collisionToggle.transform.SetParent(menu.transform);
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

                rect = menu.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0.2f, 0.1f);
                rect.anchorMax = new Vector2(0.8f, 0.7f);
                rect.anchoredPosition3D = new Vector3(0, 0, 0);
                rect.sizeDelta = new Vector2(490f, 150f);

                rect = menu.transform.FindChild("White_BG").gameObject.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0.2f, 0.22f);
                rect.anchorMax = new Vector2(0.8f, 0.68f);
                rect.anchoredPosition3D = new Vector3(0, 0, 0);
                rect.sizeDelta = new Vector2(410f, 200f);

                nitroToggle.gameObject.GetComponentInChildren<Text>().text = "Nitro";
                collisionToggle.gameObject.GetComponentInChildren<Text>().text = "Collision";

                posDropdown.ClearOptions();
                posDropdown.AddOptions(new List<string> { "Parallel", "You lead", "You chase" });

                trackDropdown.ClearOptions();
                trackList = customRaceData[SceneManager.GetActiveScene().name].Keys.ToList();
                trackDropdown.AddOptions(trackList);
                trackDropdown.template.sizeDelta = new Vector2(0, 500f);
            }

            public static void SetBattleSettings()
            {
                Debug.Log("Setting battle settings");
                CustomRaceManager.raceSettings.direction = trackDropdown.options[trackDropdown.value].text;
                CustomRaceManager.raceSettings.order = posDropdown.options[posDropdown.value].text;
                CustomRaceManager.raceSettings.nitro = nitroToggle.isOn;
                CustomRaceManager.raceSettings.collision = collisionToggle.isOn;
            }
        }

        List<GameObject> playerListItems = new();
        void SRPlayerListRoom_Start(On.SRPlayerListRoom.orig_Start orig, SRPlayerListRoom self)
        {
            orig(self);
            self.gameObject.AddComponent<CustomRaceManager>();
            new CustomRaceMenu(self.gameObject);

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
                    smallButton.gameObject.SetActive(false);
                    playerListItems.Add(smallButton.gameObject);
                    smallButton.onClick = new Button.ButtonClickedEvent();
                    smallButton.onClick.AddListener(() => BattleOptions(bh.rival));
                }
            }
        }

        void BattleOptions(RCC_CarControllerV3 carControllerV3)
        {
            Debug.Log("rival is " + carControllerV3);
            CustomRaceManager.SetRival(carControllerV3);

            Debug.Log("Opened battle options against " + CustomRaceManager.GetRivalPhotonName() + " " + CustomRaceManager.GetRivalPlayerName());
            CustomRaceMenu.menu.SetActive(true);
            CustomRaceMenu.menu.GetComponentInChildren<Text>().text = "Race against " + CustomRaceManager.GetRivalPlayerName();
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
                // playerItem.SetActive(true);
                // BattleHover asd = playerItem.gameObject.GetComponent<BattleHover>();
                // asd.rival = RCC_SceneManager.Instance.activePlayerVehicle.gameObject.GetComponent<RCC_CarControllerV3>();
                // continue;

                SRCheckOtherPlayerCam obj = playerItem.transform.GetParent().transform.GetParent().GetComponentInChildren<SRCheckOtherPlayerCam>();
                RCC_CarControllerV3 photonCar = (RCC_CarControllerV3)typeof(SRCheckOtherPlayerCam).GetField("jack", bindingFlags).GetValue(obj);
                int state = (int)typeof(SRCheckOtherPlayerCam).GetField("State", bindingFlags).GetValue(obj);
                if (state == 0 || !photonCar)
                {
                    playerItem.SetActive(false);
                    continue;
                }
                string photonName = photonCar.gameObject.name;
                if (photonName == playerPhotonName)
                {
                    playerItem.SetActive(false);
                    continue;
                }

                // disable everyone who is not warmtofu
                Text buttonText = playerItem.gameObject.GetComponentInChildren<Text>();
                // if (!NetworkTest.PlayerHasMod(photonName))
                //     buttonText.color = Color.blue;
                // else
                //     buttonText.color = Color.yellow;

                if (!NetworkTest.PlayerHasMod(photonName))
                {
                    playerItem.SetActive(false);
                    continue;
                }


                BattleHover bh = playerItem.gameObject.GetComponent<BattleHover>();
                buttonText.text = bh.playerName = playerItem.gameObject.transform.GetParent().GetComponent<Text>().text;
                if (bh.hovered)
                    buttonText.text = "Battle\n" + buttonText.text;
                if (playerItem.transform.GetParent().transform.GetParent().GetComponentInChildren<Image>().enabled)
                    playerItem.SetActive(true);
                bh.rival = photonCar;
            }
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
                    self.StartCoroutine(PlayerListUpdate(self));
                }
                else
                    CustomRaceMenu.menu.SetActive(false);
            }
        }

        void SRPlayerListRoom_PlayerListing(On.SRPlayerListRoom.orig_PlayerListing orig, SRPlayerListRoom self)
        {
            UpdateBattleHovers();
            orig(self);
        }

        void SRPlayerListRoom_PlayerListingRefresh(On.SRPlayerListRoom.orig_PlayerListingRefresh orig, SRPlayerListRoom self)
        {
            orig(self);
        }

        IEnumerator PlayerListUpdate(SRPlayerListRoom sRPlayerListRoom)
        {
            while (true)
            {
                sRPlayerListRoom.PlayerListing();
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