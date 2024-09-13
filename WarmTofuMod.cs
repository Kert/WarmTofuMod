using System;
using System.Reflection;
using UnityEngine;
using BepInEx;
using CodeStage.AntiCheat.Storage;
using CodeStage.AntiCheat.ObscuredTypes;
using ZionBandwidthOptimizer.Examples;
using HeathenEngineering.SteamApi.PlayerServices.UI;
using UnityEngine.UI;
using HeathenEngineering.SteamApi.PlayerServices;
using System.Collections;
using UnityEngine.SceneManagement;
using HeathenEngineering.SteamApi.Foundation;

namespace WarmTofuMod
{
    [BepInPlugin("com.kert.warmtofumod", "WarmTofuMod", "1.9.0")]
    public partial class WarmTofuMod : BaseUnityPlugin
    {
        public enum Menus
        {
            MENU_NONE,
            MENU_TUNING,
            MENU_SUSPENSION
        }
        public static Menus currentMenu;
        public static float uiScaleX;
        public static float uiScaleY;
        public static GUIStyle sliderStyle;
        public static GUIStyle sliderStyleThumb;
        public static GUIStyle boxStyle;
        public static GUIStyle buttonStyle;
        public static float lastSkyUpdateTime = Time.time;
        bool achievements100kmh = false;
        bool achievements200kmh = false;
        bool inTuningMenu = false;
        static SRToffuManager tofuManager;

        // used to get data from private fields with reflection
        const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

        private void Awake()
        {
            Type type = this.GetType();
            BepInPlugin bepInPlugin = (BepInPlugin)Attribute.GetCustomAttribute(type, typeof(BepInPlugin));
            Logger.LogInfo($"Plugin {bepInPlugin.GUID} is loaded!");
            try
            {
                // hooks
                // camera view
                On.RCC_Camera.ChangeCamera += RCC_Camera_ChangeCamera;
                On.RCC_Camera.OnEnable += RCC_Camera_OnEnable;

                // transmission
                On.RCC_Customization.SetTransmission += RCC_Customization_SetTransmission;
                On.RCC_CarControllerV3.OnEnable += RCC_CarControllerV3_OnEnable;

                //Front Camber Slider
                On.RCC_CustomizerExample.SetFrontCambersBySlider += RCC_CustomizerExample_SetFrontCambersBySlider;

                //Rear Camber Slider
                On.RCC_CustomizerExample.SetRearCambersBySlider += RCC_CustomizerExample_SetRearCambersBySlider;

                // Front FrontSuspension Distances
                On.RCC_CustomizerExample.SetFrontSuspensionDistancesBySlider += RCC_CustomizerExample_SetFrontSuspensionDistancesBySlider;

                // Rear FrontSuspension Distances
                On.RCC_CustomizerExample.SetRearSuspensionDistancesBySlider += RCC_CustomizerExample_SetRearSuspensionDistancesBySlider;

                //Color Hex In Color Pickers
                On.Becquet.Update += Becquet_Update;
                On.SRLightTunerUI.Update += SRLightTunerUI_Update;

                // additional suspension settings
                On.RCC_Customization.LoadStatsTemp += RCC_Customization_LoadStatsTemp;

                // Front suspension not saving fix
                On.GarageManager.Start += GarageManager_Start;

                // mod GUI and logic
                On.RCC_PhotonManager.OnGUI += RCC_PhotonManager_OnGUI;
                On.SRToffuManager.StopDelivery += SRToffuManager_StopDelivery;
                On.SRToffuManager.YesBTN += SRToffuManager_YesBTN;
                On.SRToffuManager.FinDeLivraison += SRToffuManager_FinDeLivraison;

                // Mod network code
                On.ZionBandwidthOptimizer.Examples.RCC_PhotonNetwork.Start += RCC_PhotonNetwork_Start;
                On.RaceManager.ShowMyInvitation += RaceManager_ShowMyInvitation;
                On.SRPlayerListRoom.Update += SRPlayerListRoom_Update;
                On.SRPlayerListRoom.Start += SRPlayerListRoom_Start;
                On.SRPlayerListRoom.PlayerListing += SRPlayerListRoom_PlayerListing;
                On.SRPlayerListRoom.PlayerListingRefresh += SRPlayerListRoom_PlayerListingRefresh;
                On.SRPlayerCollider.SendRaceInvitation += SRPlayerCollider_SendRaceInvitation;
                On.RaceManager.DecompteRunningIE += RaceManager_DecompteRunningIE;
                On.SRNosManager.Update += SRNosManager_Update;
                On.RCC_CarControllerV3.Inputs += RCC_CarControllerV3_Inputs;
                On.RaceManager.LetsGoRaceP1 += RaceManager_LetsGoRaceP1;
                On.RaceManager.LetsGoRaceP2 += RaceManager_LetsGoRaceP2;
                On.RaceManager.OtherLeaveRun += RaceManager_OtherLeaveRun;
                On.RaceManager.StopRun += RaceManager_StopRun;
                On.RaceManager.FinishFirst += RaceManager_FinishFirst;
                On.RaceManager.FinishSecond += RaceManager_FinishSecond;
                On.RaceManager.Start += RaceManager_Start;

                // More tofu routes
                On.LeaderboardUsersManager.sendInfo += LeaderboardUsersManager_sendInfo;
                On.LeaderboardUsersManager.Start += LeaderboardUsersManager_Start;
                On.SRToffuLivraison.OnTriggerEnter += SRToffuLivraison_OnTriggerEnter;
                On.SRToffuLivraison.OnTriggerExit += SRToffuLivraison_OnTriggerExit;
                On.SRUIManager.SetValue += SRUIManager_SetValue;
                On.SRUIManager.SetButtonBase += SRUIManager_SetButtonBase;
                On.SRUIManager.LeaderboardeMenu += SRUIManager_LeaderboardeMenu;

                // map select
                On.SpielmannSpiel_Launcher.LauncherManager.Start += LauncherManager_Start;
                On.SpielmannSpiel_Launcher.LauncherManager.start += LauncherManager_start;
                On.SpielmannSpiel_Launcher.LauncherManager.Update += LauncherManager_Update;
                On.SRTransitionMap.Start += SRTransitionMap_Start;

                // Fixed player name labels not rotating properly
                On.SRPlayerFonction.Update += SRPlayerFonction_Update;

                // Broken spawns fixes
                On.RespawnCube.Update += RespawnCube_Update;
                On.RespawnCube.TPSOUSMAP += RespawnCube_TPSOUSMAP;
                On.RCC_PhotonDemo.Spawn += RCC_PhotonDemo_Spawn;

                // 3d leaderboard fixes
                On.SR3DLBZONE.OnTriggerEnter += SR3DLBZONE_OnTriggerEnter;
                On.SR3DLBZONE.OnTriggerExit += SR3DLBZONE_OnTriggerExit;
                On.SR3DLB.EnableLBB += SR3DLB_EnableLBB;
                On.SR3DLB.Update += delegate { };

                // Akagi Menu Leaderboard fix
                On.LeaderboardUsersManager.RefreshScore2 += LeaderboardUsersManager_RefreshScore2;

                // performance fixes
                On.SRPlayerCollider.Update += delegate { };
                On.SRMessageOther.Update += delegate { };
                On.SRSkyManager.Update += SRSkyManager_Update;

                On.UnityEngine.PlayerPrefs.SetInt += PlayerPrefs_SetInt;
                On.UnityEngine.PlayerPrefs.SetFloat += PlayerPrefs_SetFloat;
                On.UnityEngine.PlayerPrefs.SetString += PlayerPrefs_SetString;

                On.UnityEngine.PlayerPrefs.GetInt_string += PlayerPrefs_GetInt_string;
                On.UnityEngine.PlayerPrefs.GetFloat_string += PlayerPrefs_GetFloat_string;
                On.UnityEngine.PlayerPrefs.GetString_string += PlayerPrefs_GetString_string;

                On.CodeStage.AntiCheat.Storage.ObscuredPrefs.GetInt += ObscuredPrefs_GetInt;
                On.CodeStage.AntiCheat.Storage.ObscuredPrefs.SetInt += ObscuredPrefs_SetInt;
                On.CodeStage.AntiCheat.Storage.ObscuredPrefs.GetBool += ObscuredPrefs_GetBool;
                On.CodeStage.AntiCheat.Storage.ObscuredPrefs.SetBool += ObscuredPrefs_SetBool;
                On.CodeStage.AntiCheat.Storage.ObscuredPrefs.HasKey += ObscuredPrefs_HasKey;

                IL.SRConcessionManager.Update += SRConcessionManager_Update;
                On.EnterArea.Update += EnterArea_Update;
                On.EnterAreaGarage.Update += EnterAreaGarage_Update;
                On.SRTransitionMap.Update += SRTransitionMap_Update;
                IL.SRUIManager.Update += SRUIManager_Update;
                On.SRUIManager.Start += SRUIManager_Start;
                On.SRUIManager.OpenMenu += SRUIManager_OpenMenu;
                On.SRCusorManager.Update += SRCusorManager_Update;

                // missing lights fix
                On.RCC_LightEmission.Update += RCC_LightEmission_Update;

                // don't grant achievements all the time
                On.SRPlayerFonction.More100kmh += SRPlayerFonction_More100kmh;
                On.SRPlayerFonction.More200kmh += SRPlayerFonction_More200kmh;

                // Skidmarks amount config
                On.RCC_Skidmarks.Start += RCC_Skidmarks_Start;
            }
            catch (Exception e)
            {
                Logger.LogError("Failed to initialize");
                Logger.LogError(e);
                throw;
            }
        }

        void EnterAreaGarage_Update(On.EnterAreaGarage.orig_Update orig, EnterAreaGarage self)
        {
            if (self.GarageMana.ButtonList != null && self.GarageMana.ButtonList.active)
                inTuningMenu = true;
            else
                inTuningMenu = false;

            orig(self);
        }

        void RCC_LightEmission_Update(On.RCC_LightEmission.orig_Update orig, RCC_LightEmission self)
        {
            if (typeof(RCC_LightEmission).GetField("material", bindingFlags).GetValue(self) == null)
                return;
            orig(self);
        }

        void SR3DLBZONE_OnTriggerEnter(On.SR3DLBZONE.orig_OnTriggerEnter orig, SR3DLBZONE self, Collider other)
        {
            if (other.GetComponentInParent<RCC_PhotonNetwork>().isMine && other.name.Contains("(Clone)"))
            {
                self.gameObject.GetComponent<SR3DLB>().EnableLBB(true);
            }
        }

        void SR3DLBZONE_OnTriggerExit(On.SR3DLBZONE.orig_OnTriggerExit orig, SR3DLBZONE self, Collider other)
        {
            if (other.GetComponentInParent<RCC_PhotonNetwork>().isMine && other.name.Contains("(Clone)"))
            {
                self.gameObject.GetComponent<SR3DLB>().EnableLBB(false);
            }
        }

        void SR3DLB_EnableLBB(On.SR3DLB.orig_EnableLBB orig, SR3DLB self, bool jack)
        {
            self.EnableLB.SetActive(jack);
            if (jack)
            {
                self.EnableLB.GetComponentInChildren<SteamworksLeaderboardList>().QueryTopEntries(5);
                StartCoroutine(UpdateLeaderboard3D(self));
            }
        }

        IEnumerator UpdateLeaderboard3D(SR3DLB leaderboard)
        {
            yield return new WaitForSeconds(1f);
            BasicLeaderboardEntry[] entries = leaderboard.EnableLB.GetComponentsInChildren<BasicLeaderboardEntry>();
            if (SceneManager.GetActiveScene().name == "USUI")
            {
                SR3DLB[] LBobjs = GameObject.FindObjectsOfType<SR3DLB>();
                foreach (SR3DLB lb in LBobjs)
                {
                    if (lb != leaderboard)
                        entries = lb.EnableLB.GetComponentsInChildren<BasicLeaderboardEntry>();
                }
            }

            leaderboard.score.text = "";
            int i = 0;
            foreach (BasicLeaderboardEntry entry in entries)
            {
                if (i >= 5)
                    break;
                leaderboard.score.text += entry.score.text + "<size=" + leaderboard.ssize + "> 's </size>\n";
                leaderboard.nameA[i].text = entry.GetComponentInChildren<Text>().text;
                i++;
            }
        }

        IEnumerator LeaderboardUsersManager_RefreshScore2(On.LeaderboardUsersManager.orig_RefreshScore2 orig, LeaderboardUsersManager self, int target)
        {
            switch (target)
            {
                case 10:
                    if (self.MyRankAkagiDH.GetComponent<SRMyLBRank>().ScoreText.text == "N/A" || self.MyRankAkagiDH.GetComponent<SRMyLBRank>().MyRank.text == "N/A")
                    {
                        self.RVAkagiBT.QueryPeerEntries(0);
                        yield return new WaitForSeconds(0.4f);
                        self.RVAkagiBT.QueryTopEntries(100);
                    }
                    break;
                case 11:
                    if (self.MyRankAkagiUH.GetComponent<SRMyLBRank>().ScoreText.text == "N/A" || self.MyRankAkagiUH.GetComponent<SRMyLBRank>().MyRank.text == "N/A")
                    {
                        self.AkagiBT.QueryPeerEntries(0);
                        yield return new WaitForSeconds(0.4f);
                        self.AkagiBT.QueryTopEntries(100);
                    }
                    break;
                case 16:
                    boardIroDH2.QueryPeerEntries(0);
                    yield return new WaitForSeconds(0.4f);
                    boardIroDH2.QueryTopEntries(100);
                    break;
                case 17:
                    boardIroUH2.QueryPeerEntries(0);
                    yield return new WaitForSeconds(0.4f);
                    boardIroUH2.QueryTopEntries(100);
                    break;
                case 18:
                    boardAkinaUH2.QueryPeerEntries(0);
                    yield return new WaitForSeconds(0.4f);
                    boardAkinaUH2.QueryTopEntries(100);
                    break;
                default:
                    yield return orig(self, target);
                    break;
            }
            yield break;
        }

        void GarageManager_Start(On.GarageManager.orig_Start orig, GarageManager self)
        {
            orig(self);
            self.frontSuspensionDistances.onValueChanged.AddListener(delegate
            {
                RCC_CustomizerExample.Instance.SaveStatsTemp();
            });
            self.frontSuspensionDistances.onValueChanged.AddListener(delegate
            {
                self.SendInfo3RPC();
            });
            self.frontCamber.wholeNumbers = self.rearCamber.wholeNumbers = true;
            self.frontCamber.minValue = self.rearCamber.minValue = -15f;
            self.frontCamber.maxValue = self.rearCamber.maxValue = 30f;
            self.frontSuspensionDistances.maxValue = self.rearSuspensionDistances.maxValue = 0.3f;

            self.frontCamber.onValueChanged.AddListener(delegate
            {
                PlayerPrefs.SetFloat("FrontCamber", self.frontCamber.value);
                RCC_CustomizerExample.Instance.SaveStatsTemp();
                self.SendInfo3RPC();
            });

            self.rearCamber.onValueChanged.AddListener(delegate
            {
                PlayerPrefs.SetFloat("RearCamber", self.rearCamber.value);
                RCC_CustomizerExample.Instance.SaveStatsTemp();
                self.SendInfo3RPC();
            });

            self.frontSuspensionDistances.onValueChanged.AddListener(delegate
            {
                PlayerPrefs.SetFloat("FrontSuspensionDistances", self.frontSuspensionDistances.value);
                RCC_CustomizerExample.Instance.SaveStatsTemp();
                self.SendInfo3RPC();
            });

            self.rearSuspensionDistances.onValueChanged.AddListener(delegate
            {
                PlayerPrefs.SetFloat("RearSuspensionDistances", self.rearSuspensionDistances.value);
                RCC_CustomizerExample.Instance.SaveStatsTemp();
                self.SendInfo3RPC();
            });

        }

        void Becquet_Update(On.Becquet.orig_Update orig, Becquet self)
        {
            orig(self);
            self.CoveringMen.GetComponent<SRCheckSkinNumberTuningShop>().g1.transform.Find("ColorField").gameObject.SetActive(true);
            self.WheelMenu.GetComponent<TuningWheelColor>().ColorPicker.transform.Find("ColorField").gameObject.SetActive(true);
        }

        void SRLightTunerUI_Update(On.SRLightTunerUI.orig_Update orig, SRLightTunerUI self)
        {
            orig(self);
            self.CameraPreview.SetActive(false);
            //Fix Lock Tuner
            GameObject LockLight = self.LockTuner;
            RectTransform LockLight_Rect = LockLight.GetComponent<RectTransform>();
            LockLight_Rect.anchoredPosition = new Vector2(-114.0f, 90.0f);
            LockLight_Rect.offsetMin = new Vector2(-264.0f, -12.0f);
            LockLight_Rect.offsetMax = new Vector2(36.0f, 192.0f);
            GameObject ColorField = self.ColorSelector.transform.Find("ColorField").gameObject;
            ColorField.SetActive(true);
            //Fix Hex In Toshi Light
            GameObject Color = ColorField.gameObject.transform.Find("Color").gameObject;
            GameObject InputField = ColorField.gameObject.transform.Find("InputField (TMP)").gameObject;
            RectTransform color_Rect = Color.GetComponent<RectTransform>();
            RectTransform inputfield_Rect = Color.GetComponent<RectTransform>();
            color_Rect.offsetMin = new Vector2(2.0f, -30.0f);
            color_Rect.offsetMax = new Vector2(82.0f, 0f);
            inputfield_Rect.offsetMin = new Vector2(82.0f, -30.0f);
            inputfield_Rect.offsetMax = new Vector2(132.0f, 0.0f);
        }


        void RCC_Camera_ChangeCamera(On.RCC_Camera.orig_ChangeCamera orig, RCC_Camera self)
        {
            orig(self);
            PlayerPrefs.SetInt("CameraMode", (int)self.cameraMode);
        }

        void RCC_Camera_OnEnable(On.RCC_Camera.orig_OnEnable orig, RCC_Camera self)
        {
            orig(self);
            self.ChangeCamera((RCC_Camera.CameraMode)PlayerPrefs.GetInt("CameraMode", 0));
        }

        void RCC_Customization_SetTransmission(On.RCC_Customization.orig_SetTransmission orig, bool automatic)
        {
            orig(automatic);
            PlayerPrefs.SetInt("AutomaticTransmission", Convert.ToInt32(automatic));
        }


        void RCC_CustomizerExample_SetFrontCambersBySlider(On.RCC_CustomizerExample.orig_SetFrontCambersBySlider orig, RCC_CustomizerExample self, Slider slider)
        {
            RCC_Customization.SetFrontCambers(RCC_SceneManager.Instance.activePlayerVehicle, PlayerPrefs.GetFloat("FrontCamber"));
        }

        void RCC_CustomizerExample_SetRearCambersBySlider(On.RCC_CustomizerExample.orig_SetRearCambersBySlider orig, RCC_CustomizerExample self, Slider slider)
        {
            RCC_Customization.SetRearCambers(RCC_SceneManager.Instance.activePlayerVehicle, PlayerPrefs.GetFloat("RearCamber"));
        }

        void RCC_CustomizerExample_SetFrontSuspensionDistancesBySlider(On.RCC_CustomizerExample.orig_SetFrontSuspensionDistancesBySlider orig, RCC_CustomizerExample self, Slider slider)
        {
            RCC_Customization.SetFrontSuspensionsDistances(RCC_SceneManager.Instance.activePlayerVehicle, PlayerPrefs.GetFloat("FrontSuspensionDistances"));
        }

        void RCC_CustomizerExample_SetRearSuspensionDistancesBySlider(On.RCC_CustomizerExample.orig_SetRearSuspensionDistancesBySlider orig, RCC_CustomizerExample self, Slider slider)
        {
            RCC_Customization.SetRearSuspensionsDistances(RCC_SceneManager.Instance.activePlayerVehicle, PlayerPrefs.GetFloat("RearSuspensionDistances"));
        }

        void RCC_Customization_LoadStatsTemp(On.RCC_Customization.orig_LoadStatsTemp orig, RCC_CarControllerV3 vehicle)
        {
            orig(vehicle);
            LoadSuspensionSettings();
        }

        void RCC_CarControllerV3_OnEnable(On.RCC_CarControllerV3.orig_OnEnable orig, RCC_CarControllerV3 self)
        {
            orig(self);
            RCC_Settings.Instance.useAutomaticGear = Convert.ToBoolean(PlayerPrefs.GetInt("AutomaticTransmission", 1));
        }

        void RCC_PhotonManager_OnGUI(On.RCC_PhotonManager.orig_OnGUI orig, RCC_PhotonManager self)
        {
            orig(self);
            OnGUI();
        }

        void SRToffuManager_StopDelivery(On.SRToffuManager.orig_StopDelivery orig, SRToffuManager self)
        {
            orig(self);
            tofuTimerLabel.SetActive(false);
        }

        void SRToffuManager_YesBTN(On.SRToffuManager.orig_YesBTN orig, SRToffuManager self)
        {
            orig(self);
            tofuTimerLabel.SetActive(true);
        }

        void SRToffuManager_FinDeLivraison(On.SRToffuManager.orig_FinDeLivraison orig, SRToffuManager self)
        {
            orig(self);
            tofuTimerLabel.SetActive(false);
            GameObject.FindObjectOfType<LeaderboardUsersManager>().Start();
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

            CreateTeleportMenu(self);

            CreateNewTofuDeliveryRoutes();

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

        void OnGUI()
        {
            if (ObscuredPrefs.GetBool("TOFU RUN", false))
                UpdateTofuTimer();

            // Additional suspension settings menus
            if (inTuningMenu)
            {
                ShowModTuningMenu();
            }
            else if (currentMenu != Menus.MENU_NONE)
            {
                RCC_CarControllerV3 activePlayerVehicle = RCC_SceneManager.Instance.activePlayerVehicle;
                PlayerPrefs.SetFloat("SuspensionDamper", activePlayerVehicle.FrontLeftWheelCollider.GetComponent<WheelCollider>().suspensionSpring.damper);
                PlayerPrefs.SetFloat("SuspensionSpring", activePlayerVehicle.FrontLeftWheelCollider.GetComponent<WheelCollider>().suspensionSpring.spring);
                currentMenu = Menus.MENU_NONE;
            }
        }

        static void ShowModTuningMenu()
        {
            if (currentMenu == Menus.MENU_NONE)
                currentMenu = Menus.MENU_TUNING;

            if (currentMenu != Menus.MENU_TUNING)
            {

                if (currentMenu != Menus.MENU_SUSPENSION)
                {
                    return;
                }
                SuspensionSettings();
                SettingsBackButton();
                return;
            }
            else if (GUI.Button(new Rect((float)((double)Screen.width / 1.73), (float)((double)Screen.height / 1.4), uiScaleX * 128f, uiScaleY * 22f), "WarmTofuMod Options", buttonStyle))
            {
                currentMenu = Menus.MENU_SUSPENSION;
                return;
            }
        }

        static GameObject tofuTimerLabel;
        static GameObject modInfoLabel;
        static void InitMenuStyles()
        {
            Type type = typeof(WarmTofuMod);
            BepInPlugin bepInPlugin = (BepInPlugin)Attribute.GetCustomAttribute(type, typeof(BepInPlugin));
            buttonStyle = new GUIStyle("Button");
            Vector2 vector = new Vector2(640f, 480f);
            buttonStyle.fontSize = (int)(12f * ((float)Screen.height / vector.y));
            buttonStyle.font = Resources.FindObjectsOfTypeAll<Font>()[4];
            uiScaleY = (float)Screen.height / vector.y;
            uiScaleX = (float)Screen.width / vector.x;
            sliderStyle = new GUIStyle("horizontalSlider");
            sliderStyle.fixedHeight = (float)((int)(12f * ((float)Screen.height / vector.y)));
            sliderStyleThumb = new GUIStyle("horizontalSliderThumb");
            sliderStyleThumb.fixedHeight = (sliderStyleThumb.fixedWidth = sliderStyle.fixedHeight);
            boxStyle = new GUIStyle("Box");
            boxStyle.font = buttonStyle.font;
            boxStyle.fontSize = buttonStyle.fontSize;
            boxStyle.normal.textColor = Color.white;
            GameObject dummyMessage = GameObject.Find("UIMessage");
            tofuTimerLabel = Instantiate(dummyMessage);
            tofuTimerLabel.transform.SetParent(dummyMessage.transform.GetParent().transform);
            Destroy(tofuTimerLabel.GetComponent<SRMessageOther>());
            Destroy(tofuTimerLabel.GetComponent<Animator>());
            RectTransform r = tofuTimerLabel.GetComponent<RectTransform>();
            r.anchorMin = r.anchorMax = new Vector2(0.5f, 0);
            r.pivot = new Vector2(0.5f, 0.1f);
            r.anchoredPosition = new Vector2(0, 0);
            r.sizeDelta = new Vector2(300f, 40f);
            tofuTimerLabel.GetComponent<Text>().color = Color.white;
            tofuTimerLabel.GetComponent<Text>().transform.localScale = new Vector3(1, 1, 1);
            tofuTimerLabel.SetActive(false);

            modInfoLabel = Instantiate(tofuTimerLabel);
            Destroy(modInfoLabel.GetComponent<SRMessageOther>());
            Destroy(modInfoLabel.GetComponent<Animator>());
            modInfoLabel.transform.SetParent(tofuTimerLabel.transform.GetParent().transform);
            r = modInfoLabel.GetComponent<RectTransform>();
            r.anchorMin = r.anchorMax = r.anchoredPosition = r.pivot = new Vector2(0, 0);
            r.sizeDelta = new Vector2(400f, 40f);
            Text t = modInfoLabel.GetComponent<Text>();
            t.text = $"{bepInPlugin.Name} v{bepInPlugin.Version} by Kert";
            t.alignment = TextAnchor.LowerLeft;
            t.transform.localScale = new Vector3(1, 1, 1);
            t.resizeTextMaxSize = 20;
            t.color = Color.gray;
            modInfoLabel.SetActive(true);
        }

        static void SettingsBackButton()
        {
            if (GUI.Button(new Rect((float)((double)Screen.width / 1.73), (float)((double)Screen.height / 1.4), uiScaleX * 128f, uiScaleY * 22f), "Back", buttonStyle))
            {
                currentMenu = Menus.MENU_TUNING;
                RCC_Customization.SaveStats(RCC_SceneManager.Instance.activePlayerVehicle);
            }
        }

        static void SuspensionSettings()
        {
            RCC_CarControllerV3 activePlayerVehicle = RCC_SceneManager.Instance.activePlayerVehicle;
            GUILayout.BeginArea(new Rect((float)((double)Screen.width / 1.73), (float)Screen.height / 3f, (float)Screen.width / 5f, (float)Screen.width / 4f));
            GUILayout.Box("Suspension spring force", boxStyle, Array.Empty<GUILayoutOption>());
            GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
            float targetValue = GUILayout.HorizontalSlider(activePlayerVehicle.RearLeftWheelCollider.wheelCollider.suspensionSpring.spring, 10000f, 100000f, sliderStyle, sliderStyleThumb, Array.Empty<GUILayoutOption>());
            GUILayout.Box(targetValue.ToString(), boxStyle, new GUILayoutOption[]
            {
                    GUILayout.MaxWidth(uiScaleX * 40f)
            });
            GUILayout.EndHorizontal();
            GUILayout.Box("Suspension spring damper", boxStyle, Array.Empty<GUILayoutOption>());
            GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
            float targetValue2 = GUILayout.HorizontalSlider(activePlayerVehicle.RearLeftWheelCollider.wheelCollider.suspensionSpring.damper, 1000f, 10000f, sliderStyle, sliderStyleThumb, Array.Empty<GUILayoutOption>());
            GUILayout.Box(targetValue2.ToString(), boxStyle, new GUILayoutOption[]
            {
                    GUILayout.MaxWidth(uiScaleX * 40f)
            });
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Reset", buttonStyle, Array.Empty<GUILayoutOption>()))
            {
                RCC_Customization.SetFrontSuspensionsSpringForce(activePlayerVehicle, 40000f);
                RCC_Customization.SetRearSuspensionsSpringForce(activePlayerVehicle, 40000f);
                RCC_Customization.SetFrontSuspensionsSpringDamper(activePlayerVehicle, 1500f);
                RCC_Customization.SetRearSuspensionsSpringDamper(activePlayerVehicle, 1500f);
            }
            else
            {
                RCC_Customization.SetFrontSuspensionsSpringForce(activePlayerVehicle, targetValue);
                RCC_Customization.SetRearSuspensionsSpringForce(activePlayerVehicle, targetValue);
                RCC_Customization.SetFrontSuspensionsSpringDamper(activePlayerVehicle, targetValue2);
                RCC_Customization.SetRearSuspensionsSpringDamper(activePlayerVehicle, targetValue2);
            }
            GUILayout.EndArea();
        }

        static void ShowDebugData()
        {
            RCC_CarControllerV3 activePlayerVehicle = RCC_SceneManager.Instance.activePlayerVehicle;
            string text = "";
            text = string.Concat(new object[]
            {
                    text,
                    activePlayerVehicle.RearLeftWheelCollider.wheelCollider.suspensionSpring.spring,
                    " ",
                    activePlayerVehicle.FrontLeftWheelCollider.wheelCollider.suspensionSpring.spring,
                    " ",
                    activePlayerVehicle.RearLeftWheelCollider.wheelCollider.suspensionSpring.damper,
                    " ",
                    activePlayerVehicle.FrontLeftWheelCollider.wheelCollider.suspensionSpring.damper
            });
            activePlayerVehicle.wheelTypeChoise = RCC_CarControllerV3.WheelType.AWD;
            activePlayerVehicle.TCS = false;
            activePlayerVehicle.ABS = false;
            RCC_Settings.Instance.useFixedWheelColliders = false;
            text = string.Concat(new object[]
            {
                    text,
                    "\n",
                    activePlayerVehicle.antiRollFrontHorizontal,
                    " ",
                    currentMenu.ToString()
            });
            GUILayout.Label(text, new GUILayoutOption[]
            {
                    GUILayout.ExpandWidth(true),
                    GUILayout.ExpandHeight(true)
            });
        }

        static void LoadSuspensionSettings()
        {
            RCC_CarControllerV3 activePlayerVehicle = RCC_SceneManager.Instance.activePlayerVehicle;
            float spring = PlayerPrefs.GetFloat("SuspensionSpring", 40000f);
            float damper = PlayerPrefs.GetFloat("SuspensionDamper", 1500f);
            RCC_Customization.SetFrontSuspensionsSpringForce(activePlayerVehicle, spring);
            RCC_Customization.SetRearSuspensionsSpringForce(activePlayerVehicle, spring);
            RCC_Customization.SetFrontSuspensionsSpringDamper(activePlayerVehicle, damper);
            RCC_Customization.SetRearSuspensionsSpringDamper(activePlayerVehicle, damper);
        }

        static void UpdateTofuTimer()
        {
            if (!tofuManager)
                tofuManager = GameObject.FindObjectOfType<SRToffuManager>();
            int time = (ObscuredInt)typeof(SRToffuManager).GetField("Compteur", bindingFlags).GetValue(tofuManager);
            tofuTimerLabel.GetComponent<Text>().text = "Tofu Time: " + time.ToString() + " 's";
        }
    }
}
