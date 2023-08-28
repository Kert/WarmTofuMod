using BepInEx;
using System.Collections.Generic;
using CodeStage.AntiCheat.Storage;

namespace WarmTofuMod
{
    public partial class WarmTofuMod : BaseUnityPlugin
    {
        const int INT_PREF_NOT_EXIST_VAL = -999;
        const float FLOAT_PREF_NOT_EXIST_VAL = -999;
        const string STRING_PREF_NOT_EXIST_VAL = "";

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
            return orig(str);
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
            return orig(str);
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
            return orig(str);
        }

        void PlayerPrefs_SetInt(On.UnityEngine.PlayerPrefs.orig_SetInt orig, string str, int value)
        {
            if (prefsInt.ContainsKey(str))
                prefsInt[str] = value;
            if (!(str == "MenuOpen" || str == "InputOn"))
            {
                orig(str, value);
            }
        }

        void PlayerPrefs_SetFloat(On.UnityEngine.PlayerPrefs.orig_SetFloat orig, string str, float value)
        {
            if (prefsFloat.ContainsKey(str))
                prefsFloat[str] = value;
            orig(str, value);
        }

        void PlayerPrefs_SetString(On.UnityEngine.PlayerPrefs.orig_SetString orig, string str, string value)
        {
            if (prefsString.ContainsKey(str))
                prefsString[str] = value;
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
            if (key == "ONTYPING")
                return;
            orig(key, value);
        }

        bool ObscuredPrefs_GetBool(On.CodeStage.AntiCheat.Storage.ObscuredPrefs.orig_GetBool orig, string key, bool defaultValue)
        {
            if (obscuredBool.ContainsKey(key) || key.StartsWith("BackCamToogle"))
            {
                if (!obscuredBool.ContainsKey(key))
                    obscuredBool[key] = orig(key, defaultValue);
                return obscuredBool[key];
            }
            return orig(key, defaultValue);
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
                return orig(key);
            }
            if (obscuredInt[key] == INT_PREF_NOT_EXIST_VAL)
                return false;
            return true;
        }

        void ObscuredPrefs_SetBool(On.CodeStage.AntiCheat.Storage.ObscuredPrefs.orig_SetBool orig, string key, bool value)
        {
            if (obscuredBool.ContainsKey(key))
                obscuredBool[key] = value;
            orig(key, value);
        }
    }
}
