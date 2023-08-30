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

            public void Ping()
            {
                playerModVersions.Clear();
                Debug.Log("Sending ping");
                try
                {
                    view.RPC("WarmTofuModReceivePing", RpcTarget.All, new object[]
                    {
                        this.gameObject.name,
                        //RCC_SceneManager.Instance.activePlayerVehicle.gameObject.GetComponent<SRPlayerCollider>().name,
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
            public void WarmTofuModReceivePing(string senderPhotonName, string senderName, string modVersion)
            {
                Debug.Log("Received ping from " + senderPhotonName + " " + senderName + " " + modVersion);
                if (!playerModVersions.ContainsKey(senderPhotonName))
                    playerModVersions.Add(senderPhotonName, modVersion);
                Pong();
            }

            void Pong()
            {
                Debug.Log("Sending pong");
                try
                {
                    view.RPC("WarmTofuModReceivePong", RpcTarget.All, new object[]
                    {
                        this.gameObject.name,
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
            private void WarmTofuModReceivePong(string senderPhotonName, string senderName, string modVersion)
            {
                Debug.Log("Received pong from " + senderPhotonName + " " + senderName + " " + modVersion);
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

        public void RCC_PhotonManager_OnJoinedRoom(On.RCC_PhotonManager.orig_OnJoinedRoom orig, RCC_PhotonManager self)
        {
            orig(self);
        }

        public void RCC_PhotonNetwork_Start(On.ZionBandwidthOptimizer.Examples.RCC_PhotonNetwork.orig_Start orig, ZionBandwidthOptimizer.Examples.RCC_PhotonNetwork self)
        {
            orig(self);
            Debug.Log("Photon network started");
            NetworkTest nt = self.gameObject.AddComponent<NetworkTest>();
            if (self.gameObject.GetComponent<PhotonView>().IsMine)
            {
                nt.Ping();
            }
        }

        public void RaceManager_AskToPlayer(On.RaceManager.orig_AskToPlayer orig, RaceManager self, string EnemyPhoton, string EnemyUI)
        {
            orig(self, EnemyPhoton, EnemyUI);
            if (NetworkTest.PlayerHasMod(EnemyPhoton))
            {
                self.StartingPointP1.position = new Vector3(-216.9f, 203.8f, 560.0f);
                self.StartingPointP1.rotation = new Quaternion(0.0f, -0.8f, 0.0f, -0.7f);
                self.StartingPointP2.position = new Vector3(-217.9f, 203.8f, 555.0f);
                self.StartingPointP2.rotation = new Quaternion(0.0f, -0.8f, 0.0f, -0.7f);
            }
        }

        IEnumerator RaceManager_ReposP1(On.RaceManager.orig_ReposP1 orig, RaceManager self)
        {
            orig(self);
            yield break;
        }

        IEnumerator RaceManager_ReposP2(On.RaceManager.orig_ReposP2 orig, RaceManager self)
        {
            orig(self);
            yield break;
        }
    }
}
