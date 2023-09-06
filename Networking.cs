using BepInEx;
using UnityEngine;
using Photon.Pun;
using System;
using System.Collections.Generic;

namespace WarmTofuMod
{
    public partial class WarmTofuMod : BaseUnityPlugin
    {
        class WarmTofuNetwork : MonoBehaviour
        {
            public PhotonView view;
            public static string oldPhotonName = "";
            static Dictionary<string, string> playerModVersions = new();
            public static WarmTofuNetwork myInstance = null;
            WarmTofuNetwork()
            {
                view = this.gameObject.GetComponent<PhotonView>();
                if (view.IsMine)
                    myInstance = this;
            }

            void OnDestroy()
            {
                Debug.Log("WarmTofuNetwork destroyed for " + view.gameObject.name);
                playerModVersions.Remove(view.gameObject.name);
            }

            public void SendModInfo()
            {
                playerModVersions.Clear();
                Debug.Log("Sending mod info");
                try
                {
                    view.RPC("ReceiveModInfo", RpcTarget.Others, new object[]
                    {
                        oldPhotonName,
                        RCC_SceneManager.Instance.activePlayerVehicle.gameObject.GetComponent<SRPlayerCollider>().name,
                        PlayerPrefs.GetString("PLAYERNAMEE"),
                        PluginInfo.PLUGIN_VERSION
                    });
                    WarmTofuNetwork.oldPhotonName = "";
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                    throw;
                }
            }

            [PunRPC]
            public void ReceiveModInfo(string oldPhotonName, string senderPhotonName, string senderName, string modVersion)
            {
                Debug.Log("Received mod info from " + oldPhotonName + " " + senderPhotonName + " " + senderName + " " + modVersion);
                // if car is changed - forget old photon name
                if (playerModVersions.ContainsKey(oldPhotonName))
                    playerModVersions.Remove(oldPhotonName);
                if (!playerModVersions.ContainsKey(senderPhotonName))
                    playerModVersions.Add(senderPhotonName, modVersion);
                SendModInfoReply();
            }

            void SendModInfoReply()
            {
                Debug.Log("Sending mod info reply");
                try
                {
                    view.RPC("SendModInfoReply", RpcTarget.Others, new object[]
                    {
                        RCC_SceneManager.Instance.activePlayerVehicle.gameObject.GetComponent<SRPlayerCollider>().name,
                        PlayerPrefs.GetString("PLAYERNAMEE"),
                        PluginInfo.PLUGIN_VERSION
                    });
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
            }

            [PunRPC]
            private void SendModInfoReply(string senderPhotonName, string senderName, string modVersion)
            {
                Debug.Log("Received mod info reply from " + senderPhotonName + " " + senderName + " " + modVersion);
                if (!playerModVersions.ContainsKey(senderPhotonName))
                    playerModVersions.Add(senderPhotonName, modVersion);
            }

            [PunRPC]
            private void ReceiveBattleInvitationRPC(string rivalPhotonName, string playerName, string playerPhotonName, string direction, string order, bool nitro, bool collision)
            {
                if (RCC_SceneManager.Instance.activePlayerVehicle.gameObject.transform.name == rivalPhotonName)
                {
                    Debug.Log("Received battle invitation from " + playerName + " " + playerPhotonName + " " + direction + " " + order + " Nitro: " + nitro + " Collision: " + collision);
                    CustomRaceManager.customRaceInvite = true;
                    CustomRaceManager.isMyInvitation = false;
                    CustomRaceManager.raceSettings.direction = direction;
                    CustomRaceManager.raceSettings.order = order;
                    CustomRaceManager.raceSettings.nitro = nitro;
                    CustomRaceManager.raceSettings.collision = collision;
                    GameObject.FindGameObjectWithTag("RaceManager").GetComponent<RaceManager>().ShowMyInvitation(playerName, playerPhotonName);
                }
            }

            public static bool PlayerHasMod(string photonName)
            {
                return playerModVersions.ContainsKey(photonName);
            }

            public string GetPlayerModVersion(string photonName)
            {
                if (!playerModVersions.ContainsKey(photonName))
                    return "";
                else
                    return playerModVersions[photonName];
            }
        }

        public void RCC_PhotonNetwork_Start(On.ZionBandwidthOptimizer.Examples.RCC_PhotonNetwork.orig_Start orig, ZionBandwidthOptimizer.Examples.RCC_PhotonNetwork self)
        {
            orig(self);
            WarmTofuNetwork nt = self.gameObject.GetComponent<WarmTofuNetwork>();
            if (!nt)
                nt = self.gameObject.AddComponent<WarmTofuNetwork>();
            if (self.gameObject.GetComponent<PhotonView>().IsMine)
                nt.SendModInfo();
        }
    }
}