using BepInEx;
using UnityEngine;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ZionBandwidthOptimizer.Examples;
using CodeStage.AntiCheat.Storage;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace WarmTofuMod
{
    public partial class WarmTofuMod : BaseUnityPlugin
    {
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
                        new Vector3(-234.0f, 203.0f, 560.0f), new Quaternion(0.0f, 0.7f, 0.0f, 0.7f)
                    )},
                    { "Downhill 2", new RacePositionsData(
                        new Vector3(-216.9f, 203.8f, 560.0f), new Quaternion(0.0f, -0.8f, 0.0f, -0.7f), new Vector3(-217.9f, 203.8f, 555.0f), new Quaternion(0.0f, -0.8f, 0.0f, -0.7f),
                        new Vector3(-216.3f, 203.7f, 560.1f), new Quaternion(0.0f, 0.7f, 0.0f, 0.7f),
                        new Vector3(), new Quaternion()
                    )},
                    { "Uphill 2", new RacePositionsData(
                        new Vector3(-1303.1f, -287.6f, 211.7f), new Quaternion(0.0f, 0.9f, 0.0f, 0.4f), new Vector3(-1305.9f, -287.6f, 207.6f), new Quaternion(0.0f, 0.9f, 0.0f, 0.4f),
                        new Vector3(-1303.1f, -287.6f, 211.7f), new Quaternion(0.0f, 0.9f, 0.0f, 0.4f),
                        new Vector3(1232.0f, 260.0f, 55.0f), new Quaternion(0.0f, 0.3f, 0.0f, 1.0f)
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
                        new Vector3(-124.2f, 125.5f, -748.0f), new Quaternion(0.0f, 0.4f, 0.0f, 0.9f)
                    )},
                    { "Uphill", new RacePositionsData(
                        new Vector3(693.9f, -134.6f, 320.5f), new Quaternion(0.0f, 0.4f, 0.0f, -0.9f), new Vector3(696.1f, -134.5f, 324.5f), new Quaternion(0.0f, 0.4f, 0.0f, -0.9f),
                        new Vector3(693.9f, -134.6f, 320.5f), new Quaternion(0.0f, 0.4f, 0.0f, -0.9f),
                        new Vector3(693.2f, -134.5f, 320.0f), new Quaternion(0.0f, 0.4f, 0.0f, 0.9f)
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

        public void RaceManager_AskToPlayer(On.RaceManager.orig_AskToPlayer orig, RaceManager self, string EnemyPhoton, string EnemyUI)
        {
            orig(self, EnemyPhoton, EnemyUI);
            if (NetworkTest.PlayerHasMod(EnemyPhoton))
            {
                Debug.Log("Race positions asking set");
                self.StartingPointP1.position = new Vector3();
                self.StartingPointP1.rotation = new Quaternion();
                self.StartingPointP2.position = new Vector3();
                self.StartingPointP2.rotation = new Quaternion();
            }
        }

        void RaceManager_ShowMyInvitation(On.RaceManager.orig_ShowMyInvitation orig, RaceManager self, string Sender, string EnvoyeurDelaDemande)
        {
            orig(self, Sender, EnvoyeurDelaDemande);
            if (NetworkTest.PlayerHasMod(EnvoyeurDelaDemande))
            {
                Debug.Log("Race positions receiver set");
                self.StartingPointP1.position = new Vector3(-216.9f, 203.8f, 560.0f);
                self.StartingPointP1.rotation = new Quaternion(0.0f, -0.8f, 0.0f, -0.7f);
                self.StartingPointP2.position = new Vector3(-217.9f, 203.8f, 555.0f);
                self.StartingPointP2.rotation = new Quaternion(0.0f, -0.8f, 0.0f, -0.7f);
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
                }
            }
        }

        GameObject battleGameObject = new GameObject();
        List<GameObject> playerListItems = new();
        GameObject battleMenu = new GameObject();
        void SRPlayerListRoom_Start(On.SRPlayerListRoom.orig_Start orig, SRPlayerListRoom self)
        {
            orig(self);
            GameObject battleGO = Instantiate(GameObject.FindObjectOfType<SRUIManager>().BtnLobby);
            battleGO.name = "BattleButton";
            Button battle = battleGO.gameObject.GetComponent<Button>();
            battle.onClick = new Button.ButtonClickedEvent();
            battleGO.transform.SetParent(self.PlayerListUI.transform);
            battleGO.GetComponentInChildren<Text>().text = "Battle";
            RectTransform rect = battleGO.GetComponent<RectTransform>();
            rect.anchoredPosition3D = new Vector3(240, 400, 0);
            rect.anchorMin = rect.anchorMax = new Vector2(0, 0);

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
            rect = battleButtons[0].GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.8f, 0.5f);
            rect.anchorMin = new Vector2(0.78f, 0.5f);
            rect.anchoredPosition = new Vector2(0f, -110f);
            Dropdown dummyDropdown = GameObject.FindObjectOfType<SRUIManager>().MenuSettings.GetComponentInChildren<Dropdown>();
            Toggle dummyToggle = GameObject.FindObjectOfType<SRUIManager>().MenuSettings.GetComponentInChildren<Toggle>();

            Dropdown trackDropdown = Instantiate(dummyDropdown);
            Dropdown posDropdown = Instantiate(dummyDropdown);
            Toggle nitroToggle = Instantiate(dummyToggle);
            Toggle collisionToggle = Instantiate(dummyToggle);
            trackDropdown.transform.SetParent(battleMenu.transform);
            posDropdown.transform.SetParent(battleMenu.transform);
            nitroToggle.transform.SetParent(battleMenu.transform);
            collisionToggle.transform.SetParent(battleMenu.transform);

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

            // tpButtons[0].GetComponentInChildren<Text>().text = "Downhill";
            // tpButtons[1].GetComponentInChildren<Text>().text = "Uphill";
            // tpButtons[0].onClick = new Button.ButtonClickedEvent();
            // tpButtons[1].onClick = new Button.ButtonClickedEvent();
            // tpButtons[0].onClick.AddListener(() => TeleportPlayer(TeleportPoints.TP_Downhill));
            // tpButtons[1].onClick.AddListener(() => TeleportPlayer(TeleportPoints.TP_Uphill));

            playerListItems.Clear();
            Text[] texts = self.PlayerListUI.GetComponentsInChildren<Text>();
            foreach (Text text in texts)
            {
                if (text.gameObject.name == "P1 (0)")
                {
                    Button smallButton = Instantiate(battle);
                    smallButton.gameObject.AddComponent<BattleHover>();
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
                    smallButton.onClick.AddListener(() => BattleOptions(battle.gameObject.name));
                }
            }
        }

        void BattleOptions(string rivalPhotonName)
        {
            Debug.Log("Opened battle options against " + rivalPhotonName);
            battleMenu.SetActive(true);
            //GameObject.FindObjectOfType<SRPlayerListRoom>().PlayerListUI.SetActive(false);
        }

        public class BattleHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
        {
            public string playerName;
            private Text hoverName;
            public bool hovered;
            RectTransform rect;
            void Start()
            {
                rect = gameObject.GetComponent<RectTransform>();
                hoverName = GetComponentInChildren<Text>();
                playerName = gameObject.transform.GetParent().GetComponent<Text>().text;
                hoverName.text = playerName;
                hovered = false;
            }

            public void OnPointerEnter(PointerEventData eventData)
            {
                hoverName.text = "Battle\n" + playerName;
                rect.anchorMax = new Vector2(rect.anchorMax.x, 1.1f);
                hovered = true;
            }

            public void OnPointerExit(PointerEventData eventData)
            {
                hoverName.text = playerName;
                rect.anchorMax = new Vector2(rect.anchorMax.x, 0.5f);
                hovered = false;
            }
        }

        void UpdateBattleHovers()
        {
            string playerPhotonName = RCC_SceneManager.Instance.activePlayerVehicle.gameObject.GetComponent<SRPlayerCollider>().name;
            foreach (GameObject playerItem in playerListItems)
            {
                SRCheckOtherPlayerCam obj = playerItem.transform.GetParent().transform.GetParent().GetComponentInChildren<SRCheckOtherPlayerCam>();
                if (!obj.MyTargetPlayer_Go)
                {
                    playerItem.SetActive(false);
                    continue;
                }
                string photonName = obj.MyTargetPlayer_Go.name;
                if (photonName == "" || photonName == playerPhotonName)
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

                buttonText.text = playerItem.gameObject.GetComponent<BattleHover>().playerName = playerItem.gameObject.transform.GetParent().GetComponent<Text>().text;
                if (playerItem.gameObject.GetComponent<BattleHover>().hovered)
                    buttonText.text = "Battle\n" + buttonText.text;
                if (playerItem.transform.GetParent().transform.GetParent().GetComponentInChildren<Image>().enabled)
                    playerItem.SetActive(true);
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
    }
}