using UnityEngine;
using BepInEx;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;
using CodeStage.AntiCheat.Storage;

namespace WarmTofuMod
{
    public partial class WarmTofuMod : BaseUnityPlugin
    {
        struct RespawnCubeData
        {
            public Vector3 position;
            public Vector3 eulerAngles;
            public RespawnCubeData(Vector3 v1, Vector3 v2) : this()
            {
                position = v1;
                eulerAngles = v2;
            }
        }

        Dictionary<string, RespawnCubeData> fixedAkinaRespawns = new Dictionary<string, RespawnCubeData>
        {
            { "Zone (1)", new RespawnCubeData(new Vector3(880.7f, 136.6f, 1167.7f), new Vector3(0.0f, 12.6f, 0.0f))},
            { "Zone (2)", new RespawnCubeData(new Vector3(893.2f, 136.6f, 1225.0f), new Vector3(0.0f, 12.6f, 0.0f))},
            { "Zone (3)", new RespawnCubeData(new Vector3(906.0f, 136.6f, 1279.9f), new Vector3(0.0f, 10.0f, 0.0f))},
            { "Zone (4)", new RespawnCubeData(new Vector3(915.0f, 136.6f, 1335.9f), new Vector3(0.0f, 192.6f, 0.0f))},
            { "Zone (5)", new RespawnCubeData(new Vector3(856.6f, 135.5f, 1096.0f), new Vector3(0.0f, 198.6f, 0.0f))},
            { "Zone (6)", new RespawnCubeData(new Vector3(878.7f, 131.0f, 715.5f), new Vector3(0.0f, 132.8f, 0.0f))},
            { "Zone (7)", new RespawnCubeData(new Vector3(935.8f, 121.2f, 420.8f), new Vector3(0.0f, 200.9f, 0.0f))},
            { "Zone (8)", new RespawnCubeData(new Vector3(897.8f, 118.6f, 447.9f), new Vector3(0.0f, 34.9f, 0.0f))},
            { "Zone (9)", new RespawnCubeData(new Vector3(770.2f, 114.1f, 701.5f), new Vector3(0.0f, 307.7f, 0.0f))},
            { "Zone (10)", new RespawnCubeData(new Vector3(600.5f, 109.8f, 842.1f), new Vector3(0.0f, 217.6f, 0.0f))},
            { "Zone (11)", new RespawnCubeData(new Vector3(664.4f, 102.2f, 604.7f), new Vector3(0.0f, 174.5f, 0.0f))},
            { "Zone (12)", new RespawnCubeData(new Vector3(596.0f, 96.4f, 318.8f), new Vector3(0.0f, 216.6f, 0.0f))},
            { "Zone (13)", new RespawnCubeData(new Vector3(485.5f, 90.0f, 159.8f), new Vector3(0.0f, 292.6f, 0.0f))},
            { "Zone (14)", new RespawnCubeData(new Vector3(479.0f, 88.1f, 208.2f), new Vector3(0.0f, 20.0f, 0.0f))},
            { "Zone (15)", new RespawnCubeData(new Vector3(514.9f, 84.5f, 293.4f), new Vector3(0.0f, 33.0f, 0.0f))},
            { "Zone (16)", new RespawnCubeData(new Vector3(528.5f, 83.0f, 327.5f), new Vector3(0.0f, 350.0f, 0.0f))},
            { "Zone (17)", new RespawnCubeData(new Vector3(422.6f, 80.1f, 225.1f), new Vector3(0.0f, 225.6f, 0.0f))},
            { "Zone (18)", new RespawnCubeData(new Vector3(473.5f, 80.1f, 321.5f), new Vector3(0.0f, 200.6f, 0.0f))},
            { "Zone (19)", new RespawnCubeData(new Vector3(511.1f, 82.4f, 348.5f), new Vector3(0.0f, 276.6f, 0.0f))},
            { "Zone (20)", new RespawnCubeData(new Vector3(265.0f, 76.2f, 129.3f), new Vector3(0.0f, 239.1f, 0.0f))},
            { "Zone (21)", new RespawnCubeData(new Vector3(-2.1f, 70.5f, -16.9f), new Vector3(0.0f, 257.6f, 0.0f))},
            { "Zone (22)", new RespawnCubeData(new Vector3(-203.0f, 66.6f, -76.6f), new Vector3(0.0f, 208.2f, 0.0f))},
            { "Zone (23)", new RespawnCubeData(new Vector3(-132.7f, 64.2f, -209.5f), new Vector3(0.0f, 158.2f, 0.0f))},
            { "Zone (24)", new RespawnCubeData(new Vector3(37.5f, 58.6f, -221.6f), new Vector3(0.0f, 97.2f, 0.0f))},
            { "Zone (25)", new RespawnCubeData(new Vector3(182.9f, 54.9f, -263.0f), new Vector3(0.0f, 68.6f, 0.0f))},
            { "Zone (26)", new RespawnCubeData(new Vector3(334.1f, 51.9f, -247.4f), new Vector3(0.0f, 123.3f, 0.0f))},
            { "Zone (27)", new RespawnCubeData(new Vector3(469.0f, 47.3f, -260.9f), new Vector3(0.0f, 38.6f, 0.0f))},
            { "Zone (28)", new RespawnCubeData(new Vector3(546.0f, 40.7f, -239.3f), new Vector3(0.0f, 190.0f, 0.0f))},
            { "Zone (29)", new RespawnCubeData(new Vector3(400.3f, 36.1f, -359.2f), new Vector3(0.0f, 267.0f, 0.0f))},
            { "Zone (30)", new RespawnCubeData(new Vector3(171.8f, 32.0f, -400.6f), new Vector3(0.0f, 267.6f, 0.0f))},
            { "Zone (31)", new RespawnCubeData(new Vector3(-99.4f, 28.0f, -381.5f), new Vector3(0.0f, 255.0f, 0.0f))},
            { "Zone (32)", new RespawnCubeData(new Vector3(-324.0f, 20.0f, -346.4f), new Vector3(0.0f, 271.0f, 0.0f))},
            { "Zone (33)", new RespawnCubeData(new Vector3(-483.3f, 12.0f, -324.1f), new Vector3(0.0f, 168.6f, 0.0f))},
            { "Zone (34)", new RespawnCubeData(new Vector3(-355.6f, 13.0f, -425.7f), new Vector3(0.0f, 110.0f, 0.0f))},
            { "Zone (35)", new RespawnCubeData(new Vector3(-237.0f, 14.0f, -595.7f), new Vector3(0.0f, 171.0f, 0.0f))},
            { "Zone (36)", new RespawnCubeData(new Vector3(-74.9f, 14.0f, -648.1f), new Vector3(0.0f, 100.0f, 0.0f))},
            { "Zone (37)", new RespawnCubeData(new Vector3(30.9f, 12.0f, -766.1f), new Vector3(0.0f, 180.0f, 0.0f))},
            { "Zone (38)", new RespawnCubeData(new Vector3(35.6f, 5.0f, -901.3f), new Vector3(0.0f, 179.0f, 0.0f))},
            { "Zone (39)", new RespawnCubeData(new Vector3(-2.5f, 0.0f, -928.3f), new Vector3(0.0f, 356.4f, 0.0f))},
            { "Zone (40)", new RespawnCubeData(new Vector3(-51.4f, -4.0f, -931.4f), new Vector3(0.0f, 176.4f, 0.0f))},
            { "Zone (41)", new RespawnCubeData(new Vector3(-89.9f, -10.0f, -973.7f), new Vector3(0.0f, 338.0f, 0.0f))},
            { "Zone (42)", new RespawnCubeData(new Vector3(-144.7f, -15.0f, -977.1f), new Vector3(0.0f, 160.0f, 0.0f))},
            { "Zone (43)", new RespawnCubeData(new Vector3(-149.5f, -24.0f, -1169.5f), new Vector3(0.0f, 202.0f, 0.0f))},
            { "Zone (44)", new RespawnCubeData(new Vector3(-210.0f, -30.0f, -1156.2f), new Vector3(0.0f, 180.0f, 0.0f))},
            { "Zone (45)", new RespawnCubeData(new Vector3(-302.7f, -38.0f, -1212.4f), new Vector3(0.0f, 210.4f, 0.0f))},
            { "Zone (46)", new RespawnCubeData(new Vector3(-364.1f, -54.0f, -1380.2f), new Vector3(0.0f, 94.0f, 0.0f))},
            { "Zone (47)", new RespawnCubeData(new Vector3(-285.9f, -74.0f, -1462.1f), new Vector3(0.0f, 274.0f, 0.0f))},
            { "Zone (48)", new RespawnCubeData(new Vector3(-148.4f, -68.9f, -1455.1f), new Vector3(0.0f, 213.0f, 0.0f))},
            { "Zone (49)", new RespawnCubeData(new Vector3(-522.1f, -80.0f, -1428.1f), new Vector3(0.0f, 280.0f, 0.0f))},
            { "Zone (50)", new RespawnCubeData(new Vector3(-679.9f, -91.0f, -1257.8f), new Vector3(0.0f, 37.0f, 0.0f))},
            { "Zone (51)", new RespawnCubeData(new Vector3(-679.5f, -98.4f, -1087.9f), new Vector3(0.0f, 322.0f, 0.0f))},
            { "Zone (52)", new RespawnCubeData(new Vector3(-894.1f, -109.3f, -1117.9f), new Vector3(0.0f, 290.0f, 0.0f))},
            { "Zone (53)", new RespawnCubeData(new Vector3(-987.5f, -115.8f, -1001.7f), new Vector3(0.0f, 333.5f, 0.0f))},
            { "Zone (54)", new RespawnCubeData(new Vector3(-1174.3f, -132.8f, -1190.0f), new Vector3(0.0f, 250.0f, 0.0f))},
            { "Zone (55)", new RespawnCubeData(new Vector3(-1378.0f, -145.0f, -1097.0f), new Vector3(0.0f, 32.0f, 0.0f))}
        };

        IEnumerator TeleportPlayerVehicle(Vector3 pos, Quaternion rot)
        {
            RCC_CarControllerV3 vehicle = RCC_SceneManager.Instance.activePlayerVehicle;
            GameObject gameObject = vehicle.gameObject;
            Rigidbody rb = gameObject.GetComponent<Rigidbody>();
            Transform t = gameObject.transform;
            rb.isKinematic = true;
            rb.velocity = rb.angularVelocity = new Vector3(0f, 0f, 0f);
            t.position = pos;
            t.rotation = rot;
            vehicle.currentGear = 0;
            vehicle.KillEngine();
            vehicle.StartEngine();
            GameObject[] cams = GameObject.FindGameObjectsWithTag("cam");
            foreach (GameObject cam in cams)
                cam.GetComponent<RCC_Camera>().ResetCamera();
            yield return new WaitForSeconds(0.1f);
            rb.isKinematic = false;
            yield break;
        }

        void RespawnCube_TPSOUSMAP(On.RespawnCube.orig_TPSOUSMAP orig, RespawnCube self)
        {
            if (ObscuredPrefs.GetBool("TOFU RUN", false))
                ObscuredPrefs.SetBool("RespawnInTofu", true);

            if (IsInUphillTofu() && SceneManager.GetActiveScene().name == "Akina")
                base.StartCoroutine(TeleportPlayerVehicle(self.RespawnPointer.position + self.RespawnPointer.TransformDirection(Vector3.left * 3f), self.RespawnPointer.rotation * Quaternion.Euler(Vector3.up * 180)));
            else
                base.StartCoroutine(TeleportPlayerVehicle(self.RespawnPointer.position, self.RespawnPointer.rotation));
        }

        bool IsInUphillTofu()
        {
            if (ObscuredPrefs.GetString("TOFULOCATION") == "ReverseNew")
            {
                if (SceneManager.GetActiveScene().name != "Akagi")
                    return true;
            }
            return false;
        }

        void RespawnCube_Update(On.RespawnCube.orig_Update orig, RespawnCube self)
        {
            if (self.Detection != 10)
                return;

            bool respawnKeyPressed = PlayerPrefs.GetString("ControllerTypeChoose") switch
            {
                "Xbox360One" => Input.GetKeyDown(KeyCode.Joystick1Button2),
                "LogitechSteeringWheel" => RCC_LogitechSteeringWheel.GetKeyPressed(0, 2),
                "PS4" => Input.GetButton("PS4_Square"),
                _ => Input.GetKeyDown(KeyCode.R),
            };

            if (!respawnKeyPressed)
                return;

            if (ObscuredPrefs.GetInt("ONTYPING", 0) == 0)
            {
                self.TPSOUSMAP();
            }
        }

        void RCC_PhotonDemo_Spawn(On.RCC_PhotonDemo.orig_Spawn orig, RCC_PhotonDemo self)
        {
            orig(self);

            // Akagi race without finish fix
            if (SceneManager.GetActiveScene().name == "Akagi")
                GameObject.Find("Scene Objects").transform.Find("Zone_RACE_Arriver").gameObject.SetActive(true);

            if (SceneManager.GetActiveScene().name != "Akina")
                return;

            RespawnCube[] cubes = GameObject.FindObjectsOfType<RespawnCube>();
            foreach (RespawnCube respawnCube in cubes)
            {
                if (!fixedAkinaRespawns.ContainsKey(respawnCube.name))
                    continue;
                RespawnCubeData current = fixedAkinaRespawns[respawnCube.name];
                respawnCube.RespawnPointer.position = current.position;
                respawnCube.RespawnPointer.eulerAngles = current.eulerAngles;
            }
        }
    }
}
