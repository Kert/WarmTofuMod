using BepInEx;
using UnityEngine;
using CodeStage.AntiCheat.Storage;
using MonoMod.Cil;
using Photon.Pun;
using System;

namespace WarmTofuMod
{
    public partial class WarmTofuMod : BaseUnityPlugin
    {
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
                self.GetComponentInChildren<UnityEngine.UI.Text>().text = self.lestips[UnityEngine.Random.Range(0, self.lestips.Length)];
            }
            if (lint == 1 && GameObject.FindGameObjectsWithTag("CanvasFadeOut").Length > 1)
            {
                GameObject.Destroy(self.RCCCanvasPhoton);
            }

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
    }
}
