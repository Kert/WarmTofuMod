using BepInEx;
using UnityEngine;
using System.Collections;
using CodeStage.AntiCheat.Storage;
using CodeStage.AntiCheat.ObscuredTypes;
using HeathenEngineering.SteamApi.PlayerServices.UI;
using HeathenEngineering.SteamApi.PlayerServices;
using Steamworks;
using ZionBandwidthOptimizer.Examples;
using System.Linq;

namespace WarmTofuMod
{
    public partial class WarmTofuMod : BaseUnityPlugin
    {
        SteamworksLeaderboardData boardIroDH2;
        SteamworksLeaderboardData boardIroUH2;
        SteamworksLeaderboardData boardAkinaUH2;
        void CreateIrohazakTofuDownhill2()
        {
            GameObject tofuZone = Instantiate(GameObject.Find("[UP]Zone_TOFU_Grossiste"));
            tofuZone.name = "[UP]Downhill2_Tofu";
            tofuZone.transform.position = new Vector3(-302.0f, 203.0f, 510.0f);

            SRToffuLivraison tofuDeliveryData = tofuZone.GetComponent<SRToffuLivraison>();
            tofuDeliveryData.location = "Downhill2";
            tofuDeliveryData.LangWelcome = "Sayori: Greetings!";
            tofuDeliveryData.LangDeliveryInProgress = "Sayori: You should finish delivery before coming for more";
            tofuDeliveryData.LangSeeYouSoon = "Sayori: See you again!";

            Transform sceneObjects = GameObject.Find("Scene Objects").transform;

            GameObject building = Instantiate(GameObject.Find("kyoto_style_house_faramiyuki_building"));
            building.transform.SetParent(sceneObjects);
            building.name = "Tofushop_Downhill2";
            building.transform.position = new Vector3(-295.5f, 204.0f, 505.5f);
            building.transform.eulerAngles = new Vector3(0, 90f, 0);

            GameObject lb_downhill2 = Instantiate(GameObject.Find("3D_LB_UPHILL"));
            lb_downhill2.transform.SetParent(sceneObjects);
            lb_downhill2.name = "3D_LB_UPHILL2";
            lb_downhill2.transform.position = new Vector3(-297.31f, 205.5f, 511.3f);
            lb_downhill2.transform.eulerAngles = new Vector3(8f, 195f, 1f);

            GameObject tofuSignWood = Instantiate(GameObject.Find("0351_combined_mattex_AlbedoTransparency  [Standard]"));
            tofuSignWood.transform.SetParent(sceneObjects);
            tofuSignWood.name = "TofuSignWood_Downhill2";
            tofuSignWood.transform.position = new Vector3(-290.7f, 205.5f, 498.7f);
            tofuSignWood.transform.eulerAngles = new Vector3(0, 150f, 0);

            GameObject lb_3d_parent = GameObject.Find("LB_3D_DOWN");
            GameObject lb_3d_dowhnill2 = Instantiate(lb_3d_parent);
            lb_3d_dowhnill2.transform.SetParent(lb_3d_parent.transform.GetParent().transform);
            lb_3d_dowhnill2.transform.position = lb_3d_parent.transform.position;
            lb_3d_dowhnill2.name = "LB_3D_DOWN2";

            lb_3d_dowhnill2.GetComponentInChildren<SteamworksLeaderboardList>().Settings = boardIroDH2;
            lb_3d_dowhnill2.SetActive(true);
            lb_downhill2.GetComponent<SR3DLB>().EnableLB = lb_3d_dowhnill2;

            GameObject tofuRadarLogoParent = GameObject.Find("Radar_Logo_Tofu");
            GameObject mapLabel = Instantiate(tofuRadarLogoParent.transform.FindChild("HUD Element Pivot").gameObject);
            mapLabel.transform.SetParent(tofuRadarLogoParent.transform);
            mapLabel.name = "Downhill2_radar";
            mapLabel.transform.position = building.transform.position;
        }

        IEnumerator CreateIrohazakTofuUphill2()
        {
            GameObject tofuZone = Instantiate(GameObject.Find("[Down]Zone_TOFU_Grossiste"));
            tofuZone.name = "[Down]Uphill2_Tofu";
            tofuZone.transform.position = new Vector3(-1320.0f, -289.0f, 238.0f);
            tofuZone.transform.eulerAngles = new Vector3(0.0f, 47.0f, 6.0f);

            SRToffuLivraison tofuDeliveryData = tofuZone.GetComponent<SRToffuLivraison>();
            tofuDeliveryData.location = "Uphill2";
            tofuDeliveryData.LangWelcome = "Sayori: Greetings!";
            tofuDeliveryData.LangDeliveryInProgress = "Sayori: You should finish delivery before coming for more";
            tofuDeliveryData.LangSeeYouSoon = "Sayori: See you again!";

            Transform sceneObjects = GameObject.Find("Scene Objects").transform;

            GameObject building = Instantiate(GameObject.Find("kyoto_style_house_faramiyuki_building"));
            building.transform.SetParent(sceneObjects);
            building.name = "Tofushop_Uphill2";
            building.transform.position = new Vector3(-1316.9f, -291.0f, 242.1f);
            building.transform.eulerAngles = new Vector3(6f, 314f, 1f);

            GameObject lb_uphill2 = Instantiate(GameObject.Find("3D_LB_DOWNHILL"));
            lb_uphill2.transform.SetParent(sceneObjects);
            lb_uphill2.name = "3D_LB_DOWNHILL2";
            lb_uphill2.transform.position = new Vector3(-1302.95f, -294.2f, 248.95f);
            lb_uphill2.transform.eulerAngles = new Vector3(15.1f, 41.7f, 7.2f);

            GameObject tofuSignWood = Instantiate(GameObject.Find("0351_combined_mattex_AlbedoTransparency  [Standard]"));
            tofuSignWood.transform.SetParent(sceneObjects);
            tofuSignWood.name = "TofuSignWood_Uphill2";
            tofuSignWood.transform.position = new Vector3(-1320.2f, -290.8f, 250.7f);
            tofuSignWood.transform.eulerAngles = new Vector3(6.0f, 355.0f, 2.0f);

            GameObject lb_3d_parent = GameObject.Find("LB_3D_UP");
            GameObject lb_3d_uphill2 = Instantiate(lb_3d_parent);
            lb_3d_uphill2.transform.SetParent(lb_3d_parent.transform.GetParent().transform);
            lb_3d_uphill2.transform.position = lb_3d_parent.transform.position;
            lb_3d_uphill2.name = "LB_3D_UP2";

            lb_3d_uphill2.GetComponentInChildren<SteamworksLeaderboardList>().Settings = boardIroUH2;
            lb_3d_uphill2.SetActive(true);
            lb_uphill2.GetComponent<SR3DLB>().EnableLB = lb_3d_uphill2;

            GameObject tofuFinish = Instantiate(tofuDeliveryData.ZoneDarriveEnable);
            tofuDeliveryData.ZoneDarriveEnable = tofuFinish;
            tofuFinish.transform.SetParent(sceneObjects);
            tofuFinish.transform.position = new Vector3(1295.0f, 260.1f, 129.2f);
            tofuFinish.transform.FindChild("Target3D_minimap").transform.position = tofuFinish.transform.position + tofuFinish.transform.TransformDirection(Vector3.up * 50f);

            // delete existing streetlight
            Destroy(GameObject.Find("iroa2019_streetlightremap_obj_22_SUB0"));
            Destroy(GameObject.Find("iroa2019_streetlightremap_obj_22_SUB1"));
            Destroy(GameObject.Find("iroa2019_streetlightremap_obj_22_SUB2"));
            Destroy(GameObject.Find("1WALL_xxiroa2019_streetlightremap_obj_22_sub1"));

            DayNightManager dayNightManager = GameObject.FindObjectOfType<DayNightManager>();
            dayNightManager.Lampadaires = dayNightManager.Lampadaires.Where(val =>
                val.name != "iroa2019_streetlightremap_obj_22_SUB0" &&
                val.name != "iroa2019_streetlightremap_obj_22_SUB1" &&
                val.name != "iroa2019_streetlightremap_obj_22_SUB2" &&
                val.name != "1WALL_xxiroa2019_streetlightremap_obj_22_sub1"
            ).ToArray();

            GameObject tofuRadarLogoParent = GameObject.Find("Radar_Logo_Tofu");
            GameObject mapLabel = Instantiate(tofuRadarLogoParent.transform.FindChild("HUD Element Pivot").gameObject);
            mapLabel.transform.SetParent(tofuRadarLogoParent.transform);
            mapLabel.name = "Uphill2_radar";
            mapLabel.transform.position = building.transform.position;

            // SR_Minimap_Manager_RaceMap doesn't exist at this point so we should wait a little bit
            yield return new WaitForSeconds(2f);
            SR_Minimap_Manager_RaceMap miniManager = GameObject.FindObjectOfType<SR_Minimap_Manager_RaceMap>();
            System.Array.Resize(ref miniManager.FixedGO, miniManager.FixedGO.Length + 1);
            miniManager.FixedGO[miniManager.FixedGO.Length - 1] = tofuFinish.transform.FindChild("Target3D_minimap").gameObject;
        }

        IEnumerator CreateAkinaTofuUphill2()
        {
            GameObject tofuZone = Instantiate(GameObject.Find("Zone_TOFU_Grossiste_down"));
            tofuZone.name = "Zone_TOFU_Grossiste_down2";
            tofuZone.transform.position = new Vector3(-1318.07f, -145.97f, -978f);
            tofuZone.transform.eulerAngles = new Vector3(0.0f, 300.0f, 0.0f);

            SRToffuLivraison tofuDeliveryData = tofuZone.GetComponent<SRToffuLivraison>();
            tofuDeliveryData.location = "Uphill2";
            tofuDeliveryData.LangWelcome = "Riko: Greetings!";
            tofuDeliveryData.LangDeliveryInProgress = "Riko: You should finish delivery before coming for more";
            tofuDeliveryData.LangSeeYouSoon = "Riko: See you again!";

            Transform sceneObjects = GameObject.Find("Scene Objects").transform;

            GameObject building = Instantiate(GameObject.Find("Small House"));
            building.transform.SetParent(sceneObjects);
            building.name = "Tofushop_Uphill2";
            building.transform.position = new Vector3(-1317.5f, -146.7f, -976.4f);
            building.transform.eulerAngles = new Vector3(0f, 230.8f, 358f);

            GameObject lamp;
            lamp = Instantiate(GameObject.Find("Lamp Street Village"));
            lamp.transform.SetParent(sceneObjects);
            lamp.name = "Uphill2_lamp";
            lamp.transform.position = new Vector3(-1316.2f, -147.0f, -981.5f);

            // add the lamp to arrays so it lights up at night too
            DayNightManager dayNightManager = GameObject.FindObjectOfType<DayNightManager>();
            System.Array.Resize(ref dayNightManager.Lampadaires, dayNightManager.Lampadaires.Length + 1);
            dayNightManager.Lampadaires[dayNightManager.Lampadaires.Length - 1] = lamp;
            System.Array.Resize(ref dayNightManager.LittleLight, dayNightManager.LittleLight.Length + 1);
            dayNightManager.LittleLight[dayNightManager.LittleLight.Length - 1] = lamp.transform.FindChild("Spot Light").gameObject;

            GameObject tofuSignWood = Instantiate(GameObject.Find("Panneaux").transform.FindChild("Cube (2)").gameObject);
            tofuSignWood.transform.SetParent(sceneObjects);
            tofuSignWood.name = "TofuSignWood_Uphill2";
            tofuSignWood.transform.position = new Vector3(-1319.1f, -145.5f, -977.75f);
            tofuSignWood.transform.eulerAngles = new Vector3(0f, 50f, 2.1f);
            tofuSignWood.transform.localScale = new Vector3(3.3f, 1.8f, 0.1f);

            GameObject lb_uphill2 = Instantiate(GameObject.Find("3D_LB_DOWNHILL"));
            lb_uphill2.transform.SetParent(sceneObjects);
            lb_uphill2.name = "3D_LB_DOWNHILL2";
            lb_uphill2.transform.position = new Vector3(-1302.24f, -144.8f, -972.5f);
            lb_uphill2.transform.eulerAngles = new Vector3(0f, 57f, 2.1f);

            GameObject lb_3d_parent = GameObject.Find("LB_3D_UP");
            GameObject lb_3d_uphill2 = Instantiate(lb_3d_parent);
            lb_3d_uphill2.transform.SetParent(lb_3d_parent.transform.GetParent().transform);
            lb_3d_uphill2.transform.position = lb_3d_parent.transform.position;
            lb_3d_uphill2.name = "LB_3D_UP2";

            lb_3d_uphill2.GetComponentInChildren<SteamworksLeaderboardList>().Settings = boardAkinaUH2;
            lb_3d_uphill2.SetActive(true);
            lb_uphill2.GetComponent<SR3DLB>().EnableLB = lb_3d_uphill2;

            GameObject tofuFinish = Instantiate(tofuDeliveryData.ZoneDarriveEnable);
            tofuDeliveryData.ZoneDarriveEnable = tofuFinish;
            tofuFinish.transform.SetParent(sceneObjects);
            tofuFinish.transform.position = new Vector3(876.6f, 137.5f, 1222.0f);
            tofuFinish.transform.eulerAngles = new Vector3(0f, 270.0f, 0f);
            Transform minimapIconTransform = tofuFinish.transform.FindChild("Target3D_minimap").transform;
            minimapIconTransform.position = tofuFinish.transform.position + tofuFinish.transform.TransformDirection(Vector3.up * 50f);
            minimapIconTransform.eulerAngles = new Vector3(90f, 80f, 0f);
            tofuFinish.transform.FindChild("Target3D").eulerAngles = new Vector3(0f, 0f, 0f);

            GameObject tofuRadarLogoParent = GameObject.Find("Radar_Logo_Tofu");
            GameObject mapLabel = Instantiate(tofuRadarLogoParent.transform.FindChild("HUD Element Pivot").gameObject);
            mapLabel.transform.SetParent(tofuRadarLogoParent.transform);
            mapLabel.name = "Uphill2_radar";
            mapLabel.transform.position = lamp.transform.position;

            // SR_Minimap_Manager_RaceMap doesn't exist at this point so we should wait a little bit
            yield return new WaitForSeconds(2f);
            SR_Minimap_Manager_RaceMap miniManager = GameObject.FindObjectOfType<SR_Minimap_Manager_RaceMap>();
            System.Array.Resize(ref miniManager.FixedGO, miniManager.FixedGO.Length + 1);
            miniManager.FixedGO[miniManager.FixedGO.Length - 1] = tofuFinish.transform.FindChild("Target3D_minimap").gameObject;
        }

        void InitLeaderboards()
        {
            boardIroDH2 = ScriptableObject.CreateInstance<SteamworksLeaderboardData>();
            boardIroDH2.MaxDetailEntries = 2;
            boardIroDH2.leaderboardName = "IROBESTTIMEDH2";
            boardIroDH2.name = "IROBESTTIMEDH2";
            boardIroDH2.Register();

            boardIroUH2 = ScriptableObject.CreateInstance<SteamworksLeaderboardData>();
            boardIroUH2.MaxDetailEntries = 2;
            boardIroUH2.leaderboardName = "REVERSE2IROBESTTIME";
            boardIroUH2.name = "REVERSE2IROBESTTIME";
            boardIroUH2.Register();

            boardAkinaUH2 = ScriptableObject.CreateInstance<SteamworksLeaderboardData>();
            boardAkinaUH2.MaxDetailEntries = 2;
            boardAkinaUH2.leaderboardName = "REVERSE2HARUNABESTTIME";
            boardAkinaUH2.name = "REVERSE2HARUNABESTTIME";
            boardAkinaUH2.Register();
        }

        void SRToffuLivraison_OnTriggerEnter(On.SRToffuLivraison.orig_OnTriggerEnter orig, SRToffuLivraison self, Collider other)
        {
            orig(self, other);
            if (other.GetComponentInParent<RCC_PhotonNetwork>().isMine && !ObscuredPrefs.GetBool("TOFU RUN", false) && PlayerPrefs.GetInt("ImInRun") == 0 && other.gameObject.tag != "ASKBATTLE")
            {
                if (other.gameObject.name != "AskBattleArea")
                {
                    GameObject target3D = self.ZoneDarriveEnable.transform.FindChild("Target3D").gameObject;
                    target3D.SetActive(true);
                    GameObject.FindObjectOfType<SRToffuManager>().LogoTarget2 = target3D;
                    self.ZoneDarriveEnable.transform.FindChild("Target3D_minimap").gameObject.SetActive(true);
                    SRToffuArriver[] arrivers = GameObject.FindObjectsOfType<SRToffuArriver>();
                    foreach (SRToffuArriver arriver in arrivers)
                    {
                        if (self.ZoneDarriveEnable != arriver.gameObject)
                        {
                            arriver.gameObject.SetActive(false);
                        }
                    }

                }
            }
        }

        void SRToffuLivraison_OnTriggerExit(On.SRToffuLivraison.orig_OnTriggerExit orig, SRToffuLivraison self, Collider other)
        {
            orig(self, other);
            if (other.GetComponentInParent<RCC_PhotonNetwork>().isMine && !ObscuredPrefs.GetBool("TOFU RUN", false) && other.gameObject.tag != "ASKBATTLE" && other.gameObject.name != "AskBattleArea")
            {
                self.ZoneDarriveEnable.transform.FindChild("Target3D").gameObject.SetActive(false);
                self.ZoneDarriveEnable.transform.FindChild("Target3D_minimap").gameObject.SetActive(false);
            }
        }

        IEnumerator LeaderboardUsersManager_sendInfo(On.LeaderboardUsersManager.orig_sendInfo orig, LeaderboardUsersManager self)
        {
            yield return orig(self);
            ObscuredInt bestTimeIroDH2 = ObscuredPrefs.GetInt("BestRunTimeIrohazakaDownhill2", 0);
            if (bestTimeIroDH2 != 0 && bestTimeIroDH2 != ObscuredPrefs.GetInt("CheatingTimeTofuIrohazakaDownhill2", 0) && ObscuredPrefs.GetInt("MyIroDH2BT", 0) != bestTimeIroDH2)
            {
                int[] scoreDetails = { ObscuredPrefs.GetInt("UsedCarsIrohazakaDownhill2", 0) };
                ObscuredPrefs.SetInt("MyIroDH2BT", bestTimeIroDH2);
                boardIroDH2.UploadScore(bestTimeIroDH2, scoreDetails, ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodForceUpdate);
            }
            ObscuredInt bestTimeIroUH2 = ObscuredPrefs.GetInt("BestRunTimeIrohazakaUphill2", 0);
            if (bestTimeIroUH2 != 0 && bestTimeIroUH2 != ObscuredPrefs.GetInt("CheatingTimeTofuIrohazakaUphill2", 0) && ObscuredPrefs.GetInt("MyIroUH2BT", 0) != bestTimeIroUH2)
            {
                int[] scoreDetails = { ObscuredPrefs.GetInt("UsedCarsIrohazakaUphill2", 0) };
                ObscuredPrefs.SetInt("MyIroUH2BT", bestTimeIroUH2);
                boardIroUH2.UploadScore(bestTimeIroUH2, scoreDetails, ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodForceUpdate);
            }
            ObscuredInt bestTimeAkinaUH2 = ObscuredPrefs.GetInt("BestRunTimeAkinaUphill2", 0);
            if (bestTimeAkinaUH2 != 0 && bestTimeAkinaUH2 != ObscuredPrefs.GetInt("CheatingTimeTofuAkinaUphill2", 0) && ObscuredPrefs.GetInt("MyAkinaUH2BT", 0) != bestTimeAkinaUH2)
            {
                int[] scoreDetails = { ObscuredPrefs.GetInt("UsedCarsAkinaUphill2", 0) };
                ObscuredPrefs.SetInt("MyAkinaUH2BT", bestTimeAkinaUH2);
                boardAkinaUH2.UploadScore(bestTimeAkinaUH2, scoreDetails, ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodForceUpdate);
            }
        }

        public void LeaderboardUsersManager_Start(On.LeaderboardUsersManager.orig_Start orig, LeaderboardUsersManager self)
        {
            orig(self);
            string resultName = "BestRunTimeIrohazakaDownhill2";
            if (ObscuredPrefs.GetInt(resultName, 0) < 150)
                ObscuredPrefs.SetInt(resultName, 0);
            resultName = "BestRunTimeIrohazakaUphill2";
            if (ObscuredPrefs.GetInt(resultName, 0) < 150)
                ObscuredPrefs.SetInt(resultName, 0);
            resultName = "BestRunTimeAkinaUphill2";
            if (ObscuredPrefs.GetInt(resultName, 0) < 150)
                ObscuredPrefs.SetInt(resultName, 0);
        }

        public void LeaderboardUsersManager_ResetLB(On.LeaderboardUsersManager.orig_ResetLB orig, LeaderboardUsersManager self)
        {
            orig(self);
            boardIroDH2.UploadScore(999, ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodForceUpdate);
            boardIroUH2.UploadScore(999, ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodForceUpdate);
            boardAkinaUH2.UploadScore(999, ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodForceUpdate);
            ObscuredPrefs.SetInt("BestRunTimeIrohazakaDownhill2", 0);
            ObscuredPrefs.SetInt("BestRunTimeIrohazakaUphill2", 0);
            ObscuredPrefs.SetInt("BestRunTimeAkinaUphill2", 0);
        }
    }
}