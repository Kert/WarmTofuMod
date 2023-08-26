using System;
using System.Reflection;
using UnityEngine;
using BepInEx;
using CodeStage.AntiCheat.Storage;
using CodeStage.AntiCheat.ObscuredTypes;
using System.Collections.Generic;
using MonoMod.Cil;
using BepInEx.Logging;
using Photon.Pun;

namespace WarmTofuMod
{
    [BepInPlugin("com.kert.warmtofumod", "WarmTofuMod", "1.3.2")]
    public class WarmTofuMod : BaseUnityPlugin
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
        const int INT_PREF_NOT_EXIST_VAL = -999;
        const float FLOAT_PREF_NOT_EXIST_VAL = -999;
        const string STRING_PREF_NOT_EXIST_VAL = "";
        bool achievements100kmh = false;
        bool achievements200kmh = false;
        bool inTuningMenu = false;

        Dictionary<string, int> prefsInt = new Dictionary<string, int>
        {
            { "ImInRun", INT_PREF_NOT_EXIST_VAL },
            { "MenuOpen", INT_PREF_NOT_EXIST_VAL },
            { "TEMPODPAD", INT_PREF_NOT_EXIST_VAL },
            { "InputOn", INT_PREF_NOT_EXIST_VAL },
            { "PS4enable", INT_PREF_NOT_EXIST_VAL },
            { "Vibration", INT_PREF_NOT_EXIST_VAL },
            { "WANTROT", INT_PREF_NOT_EXIST_VAL},
            { "DisableRearRotation_Value", INT_PREF_NOT_EXIST_VAL}
        };
        Dictionary<string, float> prefsFloat = new Dictionary<string, float>
        {
            { "SteeringSensivity", FLOAT_PREF_NOT_EXIST_VAL},
            { "SteeringhelperValue", FLOAT_PREF_NOT_EXIST_VAL}
        };
        Dictionary<string, string> prefsString = new Dictionary<string, string>
        {
            { "ControllerTypeChoose", STRING_PREF_NOT_EXIST_VAL},
            { "HistoriqueDesMessages", STRING_PREF_NOT_EXIST_VAL},
            { "DERNIERMESSAGE", STRING_PREF_NOT_EXIST_VAL}
        };

        Dictionary<string, int> obscuredInt = new Dictionary<string, int>
        {
            {"ONTYPING", INT_PREF_NOT_EXIST_VAL},
            {"BoostQuantity", INT_PREF_NOT_EXIST_VAL},
            {"TOTALWINMONEY", INT_PREF_NOT_EXIST_VAL},
            {"MyLvl", INT_PREF_NOT_EXIST_VAL},
            {"XP", INT_PREF_NOT_EXIST_VAL}
        };

        Dictionary<string, bool> obscuredBool = new Dictionary<string, bool>
        {
            {"TOFU RUN", false}
        };

        const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

        private void Awake()
        {
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            try
            {
                // hooks
                // camera view
                On.RCC_Camera.ChangeCamera += RCC_Camera_ChangeCamera;
                On.RCC_Camera.OnEnable += RCC_Camera_OnEnable;

                // transmission
                On.RCC_Customization.SetTransmission += RCC_Customization_SetTransmission;
                On.RCC_CarControllerV3.OnEnable += RCC_CarControllerV3_OnEnable;

                // additional suspension settings
                On.RCC_Customization.LoadStatsTemp += RCC_Customization_LoadStatsTemp;

                // mod GUI and logic
                On.RCC_PhotonManager.OnGUI += RCC_PhotonManager_OnGUI;

                // Front suspension not saving fix
                On.GarageManager.Start += GarageManager_Start;

                // Fixed player name labels not rotating properly
                On.SRPlayerFonction.Update += SRPlayerFonction_Update;

                // performance fixes
                On.SRPlayerCollider.Update += SRPlayerCollider_Update;
                On.SRMessageOther.Update += SRMessageOther_Update;
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

            int PlayerPrefs_GetInt_string(On.UnityEngine.PlayerPrefs.orig_GetInt_string orig, string str)
            {
                if (prefsInt.ContainsKey(str))
                {
                    int val = prefsInt[str];
                    if (val == INT_PREF_NOT_EXIST_VAL)
                    {
                        val = orig(str);
                        prefsInt[str] = val;
                    }
                    return val;
                }
                else
                {
                    Debug.Log("Reading int " + str);
                    return orig(str);
                }
            }

            float PlayerPrefs_GetFloat_string(On.UnityEngine.PlayerPrefs.orig_GetFloat_string orig, string str)
            {
                if (prefsFloat.ContainsKey(str))
                {
                    float val = prefsFloat[str];
                    if (val == FLOAT_PREF_NOT_EXIST_VAL)
                    {
                        val = orig(str);
                        prefsFloat[str] = val;
                    }
                    return val;
                }
                else
                {
                    Debug.Log("Reading float " + str);
                    return orig(str);
                }
            }

            string PlayerPrefs_GetString_string(On.UnityEngine.PlayerPrefs.orig_GetString_string orig, string str)
            {
                if (prefsString.ContainsKey(str) || str.StartsWith("BreakBtnUsed"))
                {
                    if (!prefsString.ContainsKey(str))
                        prefsString[str] = orig(str);
                    string val = prefsString[str];
                    if (val == STRING_PREF_NOT_EXIST_VAL)
                    {
                        val = orig(str);
                        prefsString[str] = val;
                    }
                    return val;
                }
                else
                {
                    Debug.Log("Reading string " + str);
                    return orig(str);
                }
            }

            void PlayerPrefs_SetInt(On.UnityEngine.PlayerPrefs.orig_SetInt orig, string str, int value)
            {
                if (prefsInt.ContainsKey(str))
                    prefsInt[str] = value;
                else
                {
                    Debug.Log("Setting int " + str);
                }
                if (!(str == "MenuOpen" || str == "InputOn"))
                {
                    orig(str, value);
                }
            }


            void PlayerPrefs_SetFloat(On.UnityEngine.PlayerPrefs.orig_SetFloat orig, string str, float value)
            {
                if (prefsFloat.ContainsKey(str))
                    prefsFloat[str] = value;
                else
                {
                    Debug.Log("Setting float " + str);
                }
                orig(str, value);
            }

            void PlayerPrefs_SetString(On.UnityEngine.PlayerPrefs.orig_SetString orig, string str, string value)
            {
                if (prefsString.ContainsKey(str))
                    prefsString[str] = value;
                else
                {
                    Debug.Log("Setting string " + str);
                }
                orig(str, value);
            }

            int ObscuredPrefs_GetInt(On.CodeStage.AntiCheat.Storage.ObscuredPrefs.orig_GetInt orig, string key, int defaultValue)
            {
                if (obscuredInt.ContainsKey(key) || key.StartsWith("UsedPlateForFutureSpawn") || key.StartsWith("BuyPlateNumber"))
                {
                    if (!obscuredInt.ContainsKey(key))
                    {
                        obscuredInt[key] = orig(key, defaultValue);
                    }
                    int val = obscuredInt[key];
                    if (val == INT_PREF_NOT_EXIST_VAL)
                    {
                        val = orig(key, defaultValue);
                        obscuredInt[key] = val;
                    }
                    return val;
                }
                return orig(key, defaultValue);
            }

            void ObscuredPrefs_SetInt(On.CodeStage.AntiCheat.Storage.ObscuredPrefs.orig_SetInt orig, string key, int value)
            {
                if (obscuredInt.ContainsKey(key))
                    obscuredInt[key] = value;
                else
                {
                    //Debug.Log("Setting int " + key);
                }
                if (key == "ONTYPING")
                    return;
                Debug.Log("Setting int " + key);
                orig(key, value);
            }

            bool ObscuredPrefs_GetBool(On.CodeStage.AntiCheat.Storage.ObscuredPrefs.orig_GetBool orig, string key, bool defaultValue)
            {
                if (obscuredBool.ContainsKey(key) || key.StartsWith("BackCamToogle"))
                {
                    if (!obscuredBool.ContainsKey(key))
                    {
                        obscuredBool[key] = orig(key, defaultValue);
                    }
                    return obscuredBool[key];
                }
                else
                {
                    return orig(key, defaultValue);
                }
            }

            bool ObscuredPrefs_HasKey(On.CodeStage.AntiCheat.Storage.ObscuredPrefs.orig_HasKey orig, string key)
            {
                if (!obscuredInt.ContainsKey(key))
                {
                    if (key.StartsWith("UsedPlateForFutureSpawn"))
                    {
                        if (orig(key))
                        {
                            obscuredInt[key] = ObscuredPrefs.GetInt(key, 0);
                            return true;
                        }
                        obscuredInt[key] = INT_PREF_NOT_EXIST_VAL;
                        return false;
                    }
                    else
                        return orig(key);
                }
                else
                {
                    if (obscuredInt[key] == INT_PREF_NOT_EXIST_VAL)
                        return false;
                    return true;
                }
            }

            void ObscuredPrefs_SetBool(On.CodeStage.AntiCheat.Storage.ObscuredPrefs.orig_SetBool orig, string key, bool value)
            {
                if (obscuredBool.ContainsKey(key))
                    obscuredBool[key] = value;
                else
                {
                    Debug.Log("Setting bool " + key);
                }
                Debug.Log("Setting bool " + key);
                orig(key, value);
            }

            void SRConcessionManager_Update(ILContext il)
            {
                try
                {
                    var c = new ILCursor(il);
                    c.GotoNext(MoveType.After,
                        i => i.MatchStfld<SRConcessionManager>("NombreDeSkin")
                        );

                    // skip the rest of the code
                    c.Emit(Mono.Cecil.Cil.OpCodes.Ret);
                }
                catch (Exception e)
                {
                    Logger.LogError(e);
                    throw;
                }
            }

            void EnterArea_Update(On.EnterArea.orig_Update orig, EnterArea self)
            {
                if (self.CarDealer.activeSelf)
                    GameObject.FindObjectOfType<SRConcessionManager>().MoneyDisplay.text = ObscuredPrefs.GetInt("MyBalance", 0) + "¥";
                orig(self);
            }

            void EnterAreaGarage_Update(On.EnterAreaGarage.orig_Update orig, EnterAreaGarage self)
            {
                if (self.GarageMana.ButtonList != null && self.GarageMana.ButtonList.active)
                    inTuningMenu = true;
                else
                    inTuningMenu = false;

                orig(self);
            }

            void RCC_Skidmarks_Start(On.RCC_Skidmarks.orig_Start orig, RCC_Skidmarks self)
            {
                orig(self);
                self.maxMarks = 256;
            }

            void SRTransitionMap_Update(On.SRTransitionMap.orig_Update orig, SRTransitionMap self)
            {
                // changed order of conditions
                int lint = (int)typeof(SRTransitionMap).GetField("lint", bindingFlags).GetValue(self);
                if (lint == 0 && self.UIFadeout.activeSelf)
                {
                    typeof(SRTransitionMap).GetField("lint", bindingFlags).SetValue(self, 1);
                    base.GetComponentInChildren<UnityEngine.UI.Text>().text = self.lestips[UnityEngine.Random.Range(0, self.lestips.Length)];
                }
                if (lint == 1 && GameObject.FindGameObjectsWithTag("CanvasFadeOut").Length > 1)
                    GameObject.Destroy(self.RCCCanvasPhoton);
            }

            void SRUIManager_Update(ILContext il)
            {
                try
                {
                    var c = new ILCursor(il);
                    c.GotoNext(MoveType.After,
                        i => i.MatchCall<SRUIManager>("ExitMenuNo")
                        );

                    c.EmitDelegate<Action>(() =>
                    {
                        if (ObscuredPrefs.GetInt("MyLvl", 0) >= 1000 && ObscuredPrefs.GetInt("TOTALWINMONEY", 0) < 5000)
                        {
                            obscuredInt["Mylvl"] = 0;
                            obscuredInt["XP"] = 0;
                        }
                    });

                    // skip the rest of the code
                    c.Emit(Mono.Cecil.Cil.OpCodes.Ret);
                }
                catch (Exception e)
                {
                    Logger.LogError(e);
                    throw;
                }
            }

            void RCC_LightEmission_Update(On.RCC_LightEmission.orig_Update orig, RCC_LightEmission self)
            {
                if (typeof(RCC_LightEmission).GetField("material", bindingFlags).GetValue(self) == null)
                {
                    return;
                }
                orig(self);
            }

            void SRPlayerFonction_More100kmh(On.SRPlayerFonction.orig_More100kmh orig, SRPlayerFonction self)
            {
                if (!achievements100kmh)
                {
                    Steamworks.SteamUserStats.GetAchievement("100KMH", out achievements100kmh);
                    if (!achievements100kmh)
                        orig(self);
                }
            }

            void SRPlayerFonction_More200kmh(On.SRPlayerFonction.orig_More200kmh orig, SRPlayerFonction self)
            {
                if (!achievements200kmh)
                {
                    Steamworks.SteamUserStats.GetAchievement("200KMH", out achievements200kmh);
                    if (!achievements200kmh)
                        orig(self);
                }
            }

            void SRPlayerCollider_Update(On.SRPlayerCollider.orig_Update orig, SRPlayerCollider self)
            {

            }

            void SRMessageOther_Update(On.SRMessageOther.orig_Update orig, SRMessageOther self)
            {

            }

            void SRSkyManager_Update(On.SRSkyManager.orig_Update orig, SRSkyManager self)
            {
                int PeopleNbr = (int)typeof(SRSkyManager).GetField("PeopleNbr", bindingFlags).GetValue(self);
                RCC_CarControllerV3[] playerCars = GameObject.FindObjectsOfType<RCC_CarControllerV3>();
                if (playerCars.Length != PeopleNbr && self.Autorisation)
                {
                    typeof(SRSkyManager).GetField("PeopleNbr", bindingFlags).SetValue(self, playerCars.Length);
                    RCC_SceneManager.Instance.activePlayerVehicle.gameObject.GetComponent<SRPlayerCollider>().SendTheTimeOfRoom(self.Minute);
                }
                if (!self.Autorisation && !(GameObject)typeof(SRSkyManager).GetField("TargetMec", bindingFlags).GetValue(self) && (int)typeof(SRSkyManager).GetField("ReceidMaster", bindingFlags).GetValue(self) == 1)
                {
                    if (playerCars[0].gameObject.name == RCC_SceneManager.Instance.activePlayerVehicle.gameObject.name)
                    {
                        self.Autorisation = true;
                        self.ImMaster = 10;
                    }
                    else
                    {
                        typeof(SRSkyManager).GetField("TargetMec", bindingFlags).SetValue(self, playerCars[0].gameObject);
                        self.ImMaster = 5;
                    }
                }
                self.SetSky();
            }

            void SRPlayerFonction_Update(On.SRPlayerFonction.orig_Update orig, SRPlayerFonction self)
            {
                if (self.GetComponent<PhotonView>().IsMine)
                {
                    GameObject[] cams = GameObject.FindGameObjectsWithTag("cam");
                    int camIndex = 0;
                    if (cams.Length == 2)
                        camIndex = 1;
                    Quaternion rotation = cams[camIndex].transform.rotation;
                    GameObject[] array = GameObject.FindGameObjectsWithTag("3DPSEUDO");
                    for (int i = 0; i < array.Length; i++)
                    {
                        array[i].transform.rotation = rotation;
                    }
                }
                if (base.gameObject.transform.rotation.z <= -0.2507755f || base.gameObject.transform.rotation.z >= 0.2507755f)
                {
                    self.TopCamera.SetActive(false);
                    typeof(SRPlayerFonction).GetField("OK", bindingFlags).SetValue(self, true);
                    return;
                }
                if ((bool)typeof(SRPlayerFonction).GetField("OK", bindingFlags).GetValue(self))
                {
                    self.TopCamera.SetActive(true);
                    typeof(SRPlayerFonction).GetField("OK", bindingFlags).SetValue(self, false);
                }
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

            void OnGUI()
            {
                if (ObscuredPrefs.GetBool("TOFU RUN", false))
                    ShowTofuTimer();

                ShowFooter();

                if (buttonStyle == null)
                    InitMenuStyles();

                // Additional suspension settings menus
                if (inTuningMenu)
                    ShowModTuningMenu();
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
                else if (GUI.Button(new Rect((float)((double)Screen.width / 1.73), (float)((double)Screen.height / 1.4), uiScaleX * 125f, uiScaleY * 22f), "WarmTofuMod Options", buttonStyle))
                {
                    currentMenu = Menus.MENU_SUSPENSION;
                    return;
                }
            }

            static void InitMenuStyles()
            {
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
            }

            static void SettingsBackButton()
            {
                if (GUI.Button(new Rect((float)((double)Screen.width / 1.73), (float)((double)Screen.height / 1.4), uiScaleX * 125f, uiScaleY * 22f), "Back", buttonStyle))
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

            static void ShowFooter()
            {
                GUILayout.BeginArea(new Rect(5f, (float)Screen.height - 20f, 800f, 20f));
                GUILayout.Label($"{PluginInfo.PLUGIN_NAME} v{PluginInfo.PLUGIN_VERSION} by Kert", Array.Empty<GUILayoutOption>());
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

            static void ShowTofuTimer()
            {
                int tofuTimer = (ObscuredInt)typeof(SRToffuManager).GetField("Compteur", bindingFlags).GetValue(GameObject.FindObjectOfType<SRToffuManager>());
                GUILayout.BeginArea(new Rect((float)(Screen.width / 2) - 120f, (float)Screen.height - 50f, 800f, 100f));
                GUIStyle guistyle = new GUIStyle();
                guistyle.font = buttonStyle.font;
                guistyle.fontSize = buttonStyle.fontSize;
                guistyle.normal.textColor = Color.white;
                GUILayout.Label("Tofu Time: " + tofuTimer.ToString() + " 's", guistyle, Array.Empty<GUILayoutOption>());
                GUILayout.EndArea();
            }
        }
    }
}
