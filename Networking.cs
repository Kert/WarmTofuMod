using BepInEx;
using UnityEngine;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using ZionBandwidthOptimizer.Examples;

namespace WarmTofuMod
{
    public partial class WarmTofuMod : BaseUnityPlugin
    {
        class NetworkTest : MonoBehaviour
        {
            public PhotonView view;
            static Dictionary<string, string> playerModVersions = new();
            NetworkTest()
            {
                view = this.gameObject.GetComponent<PhotonView>();
            }

            public void SendModInfo(string oldPhotonName)
            {
                playerModVersions.Clear();
                Debug.Log("Sending mod info");
                try
                {
                    view.RPC("ReceiveModInfo", RpcTarget.All, new object[]
                    {
                        oldPhotonName,
                        RCC_SceneManager.Instance.activePlayerVehicle.gameObject.GetComponent<SRPlayerCollider>().name,
                        PlayerPrefs.GetString("PLAYERNAMEE"),
                        PluginInfo.PLUGIN_VERSION
                    });
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
                if (oldPhotonName != "" && playerModVersions.ContainsKey(oldPhotonName))
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
                    view.RPC("SendModInfoReply", RpcTarget.All, new object[]
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

            public static bool PlayerHasMod(string colliderName)
            {
                return playerModVersions.ContainsKey(colliderName);
            }

            public string GetPlayerModVersion(string colliderName)
            {
                if (!playerModVersions.ContainsKey(colliderName))
                    return "";
                else
                    return playerModVersions[colliderName];
            }
        }

        public void RCC_PhotonNetwork_Start(On.ZionBandwidthOptimizer.Examples.RCC_PhotonNetwork.orig_Start orig, ZionBandwidthOptimizer.Examples.RCC_PhotonNetwork self)
        {
            string oldPhotonName = "";
            if (RCC_SceneManager.Instance.activePlayerVehicle)
                oldPhotonName = RCC_SceneManager.Instance.activePlayerVehicle.gameObject.GetComponent<SRPlayerCollider>().name;
            orig(self);
            Debug.Log("Photon network started");
            NetworkTest nt = self.gameObject.AddComponent<NetworkTest>();
            if (self.gameObject.GetComponent<PhotonView>().IsMine)
            {
                nt.SendModInfo(oldPhotonName);
            }
        }
    }
}
