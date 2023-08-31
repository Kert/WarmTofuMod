using BepInEx;
using UnityEngine;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using ZionBandwidthOptimizer.Examples;
using CodeStage.AntiCheat.Storage;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace WarmTofuMod
{
    public partial class WarmTofuMod : BaseUnityPlugin
    {
        enum RaceDirection
        {
            Race_Downhill,
            Race_Uphill,
            Race_Downhill2,
            Race_Uphill2
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

        Dictionary<string, Dictionary<RaceDirection, RacePositionsData>> RacePositionData = new Dictionary<string, Dictionary<RaceDirection, RacePositionsData>>
        {
            { "Irohazaka", new Dictionary<RaceDirection, RacePositionsData>
                {
                    { RaceDirection.Race_Downhill, new RacePositionsData(
                        new Vector3(1232.7f, 260.2f, 55.5f), new Quaternion(0.0f, 1.0f, 0.0f, -0.3f), new Vector3(1230.2f, 260.2f, 57.0f), new Quaternion(0.0f, 1.0f, 0.0f, -0.3f),
                        new Vector3(1232.7f, 260.2f, 59.0f), new Quaternion(0.0f, -1.0f, 0.0f, 0.3f),
                        new Vector3(-1321.0f, -289.0f, 218.2f), new Quaternion(0.0f, 0.4f, 0.0f, -0.9f)
                    )},
                    { RaceDirection.Race_Uphill, new RacePositionsData(
                        new Vector3(-1223.2f, -280.6f, 132.5f), new Quaternion(0.0f, 0.9f, 0.0f, 0.4f), new Vector3(-1226.2f, -280.6f, 129.5f), new Quaternion(0.0f, 0.9f, 0.0f, 0.4f),
                        new Vector3(-1183.1f, -276.4f, 96.1f), new Quaternion(0.0f, 0.9f, 0.0f, 0.4f),
                        new Vector3(-234.0f, 203.0f, 560.0f), new Quaternion(0.0f, 0.7f, 0.0f, 0.7f)
                    )},
                    { RaceDirection.Race_Downhill2, new RacePositionsData(
                        new Vector3(-216.9f, 203.8f, 560.0f), new Quaternion(0.0f, -0.8f, 0.0f, -0.7f), new Vector3(-217.9f, 203.8f, 555.0f), new Quaternion(0.0f, -0.8f, 0.0f, -0.7f),
                        new Vector3(-216.3f, 203.7f, 560.1f), new Quaternion(0.0f, 0.7f, 0.0f, 0.7f),
                        new Vector3(), new Quaternion()
                    )},
                    { RaceDirection.Race_Uphill2, new RacePositionsData(
                        new Vector3(-1303.1f, -287.6f, 211.7f), new Quaternion(0.0f, 0.9f, 0.0f, 0.4f), new Vector3(-1305.9f, -287.6f, 207.6f), new Quaternion(0.0f, 0.9f, 0.0f, 0.4f),
                        new Vector3(-1303.1f, -287.6f, 211.7f), new Quaternion(0.0f, 0.9f, 0.0f, 0.4f),
                        new Vector3(1232.0f, 260.0f, 55.0f), new Quaternion(0.0f, 0.3f, 0.0f, 1.0f)
                    )}
                }
            },
            { "Akina", new Dictionary<RaceDirection, RacePositionsData>
                {
                    { RaceDirection.Race_Downhill, new RacePositionsData(
                        new Vector3(915.5f, 136.5f, 1323.3f), new Quaternion(0.0f, 1.0f, 0.0f, -0.1f), new Vector3(912.1f, 136.5f, 1323.7f), new Quaternion(0.0f, 1.0f, 0.0f, -0.1f),
                        new Vector3(915.5f, 136.5f, 1323.3f), new Quaternion(0.0f, 1.0f, 0.0f, -0.1f),
                        new Vector3(-1369.4f, -146.0f, -1083.6f), new Quaternion(0.0f, 0.3f, 0.0f, 1.0f)
                    )},
                    { RaceDirection.Race_Uphill, new RacePositionsData(
                        new Vector3(-1372.4f, -145.4f, -1081.3f), new Quaternion(0.0f, 0.2f, 0.0f, 1.0f), new Vector3(-1369.9f, -145.4f, -1082.9f), new Quaternion(0.0f, 0.2f, 0.0f, 1.0f),
                        new Vector3(-1372.4f, -145.4f, -1081.3f), new Quaternion(0.0f, 0.2f, 0.0f, 1.0f),
                        new Vector3(912.4f, 137.0f, 1323.0f), new Quaternion(0.0f, 0.2f, 0.0f, 1.0f)
                    )}
                }
            },
            { "Akagi", new Dictionary<RaceDirection, RacePositionsData>
                {
                    { RaceDirection.Race_Downhill, new RacePositionsData(
                        new Vector3(-124.9f, 125.8f, -748.6f), new Quaternion(0.0f, -0.4f, 0.0f, -0.9f), new Vector3(-121.6f, 125.9f, -751.6f), new Quaternion(0.0f, -0.4f, 0.0f, -0.9f),
                        new Vector3(-124.9f, 125.8f, -748.6f), new Quaternion(0.0f, -0.4f, 0.0f, -0.9f),
                        new Vector3(-124.2f, 125.5f, -748.0f), new Quaternion(0.0f, 0.4f, 0.0f, 0.9f)
                    )},
                    { RaceDirection.Race_Uphill, new RacePositionsData(
                        new Vector3(693.9f, -134.6f, 320.5f), new Quaternion(0.0f, 0.4f, 0.0f, -0.9f), new Vector3(696.1f, -134.5f, 324.5f), new Quaternion(0.0f, 0.4f, 0.0f, -0.9f),
                        new Vector3(693.9f, -134.6f, 320.5f), new Quaternion(0.0f, 0.4f, 0.0f, -0.9f),
                        new Vector3(693.2f, -134.5f, 320.0f), new Quaternion(0.0f, 0.4f, 0.0f, 0.9f)
                    )}
                }
            },
            { "USUI", new Dictionary<RaceDirection, RacePositionsData>
                {
                    { RaceDirection.Race_Downhill, new RacePositionsData(
                        new Vector3(1194.4f, 66.3f, 780.0f), new Quaternion(0.0f, -0.8f, 0.0f, 0.5f), new Vector3(1192.8f, 66.4f, 784.4f), new Quaternion(0.0f, -0.8f, 0.0f, 0.6f),
                        new Vector3(1194.4f, 66.3f, 780.0f), new Quaternion(0.0f, -0.8f, 0.0f, 0.5f),
                        new Vector3(-1554.9f, -211.4f, -711.5f), new Quaternion(0.0f, 0.7f, 0.0f, -0.7f)
                    )},
                    { RaceDirection.Race_Uphill, new RacePositionsData(
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

            Text[] texts = self.PlayerListUI.GetComponentsInChildren<Text>();
            foreach (Text text in texts)
            {
                if (text.gameObject.name == "P1 (0)")
                {
                    Button smallButton = Instantiate(battle);
                    smallButton.gameObject.AddComponent<BattleHover>();
                    smallButton.transform.SetParent(text.gameObject.transform);
                    RectTransform r = smallButton.GetComponent<RectTransform>();
                    r.anchoredPosition = new Vector3(0, 0, 0);
                    r.anchorMin = r.anchorMax = new Vector2(0.5f, 0.5f);
                    r.sizeDelta = new Vector2(190f, 20f);
                    Text t = smallButton.GetComponentInChildren<Text>();
                    t.fontSize = t.resizeTextMaxSize = 20;
                    t.resizeTextMinSize = 1;
                }
            }
        }

        public class BattleHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
        {
            private string playerName;
            private Text hoverName;
            RectTransform rect;
            void Start()
            {
                rect = gameObject.GetComponent<RectTransform>();
                hoverName = GetComponentInChildren<Text>();
                playerName = gameObject.transform.GetParent().GetComponent<Text>().text;
                hoverName.text = playerName;
            }

            public void OnPointerEnter(PointerEventData eventData)
            {
                hoverName.text = "Battle\n" + playerName;
                rect.anchorMax = new Vector2(rect.anchorMax.x, 1.1f);
            }

            public void OnPointerExit(PointerEventData eventData)
            {
                hoverName.text = playerName;
                rect.anchorMax = new Vector2(rect.anchorMax.x, 0.5f);
            }
        }
    }
}
