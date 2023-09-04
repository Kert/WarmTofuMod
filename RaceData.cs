using BepInEx;
using UnityEngine;
using System.Collections.Generic;

namespace WarmTofuMod
{
    public partial class WarmTofuMod : BaseUnityPlugin
    {
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

        static Dictionary<string, Dictionary<string, RacePositionsData>> customRaceData = new Dictionary<string, Dictionary<string, RacePositionsData>>
        {
            { "Irohazaka", new Dictionary<string, RacePositionsData>
                {
                    { "Downhill", new RacePositionsData(
                        new Vector3(1232.7f, 260.2f, 55.5f), new Quaternion(0.0f, 1.0f, 0.0f, -0.3f), new Vector3(1230.2f, 260.2f, 57.0f), new Quaternion(0.0f, 1.0f, 0.0f, -0.3f),
                        new Vector3(1232.7f, 260.2f, 59.0f), new Quaternion(0.0f, -1.0f, 0.0f, 0.3f),
                        new Vector3(-1321.0f, -289.0f, 218.2f), new Quaternion(0.0f, 0.4f, 0.0f, -0.9f)
                    )},
                    { "Uphill", new RacePositionsData(
                        new Vector3(-1223.2f, -280.6f, 132.5f), new Quaternion(0.0f, 0.9f, 0.0f, 0.4f), new Vector3(-1226.2f, -280.6f, 129.5f), new Quaternion(0.0f, 0.9f, 0.0f, 0.4f),
                        new Vector3(-1183.1f, -276.4f, 96.1f), new Quaternion(0.0f, 0.9f, 0.0f, 0.4f),
                        new Vector3(1232.0f, 260.0f, 55.0f), new Quaternion(0.0f, 0.3f, 0.0f, 1.0f)
                    )},
                    { "Downhill 2", new RacePositionsData(
                        new Vector3(-216.9f, 203.8f, 560.0f), new Quaternion(0.0f, -0.8f, 0.0f, -0.7f), new Vector3(-217.9f, 203.8f, 555.0f), new Quaternion(0.0f, -0.8f, 0.0f, -0.7f),
                        new Vector3(-216.3f, 203.7f, 560.1f), new Quaternion(0.0f, 0.7f, 0.0f, 0.7f),
                        new Vector3(-1321.0f, -289.0f, 218.2f), new Quaternion(0.0f, 0.4f, 0.0f, -0.9f)
                    )},
                    { "Uphill 2", new RacePositionsData(
                        new Vector3(-1303.1f, -287.6f, 211.7f), new Quaternion(0.0f, 0.9f, 0.0f, 0.4f), new Vector3(-1305.9f, -287.6f, 207.6f), new Quaternion(0.0f, 0.9f, 0.0f, 0.4f),
                        new Vector3(-1303.1f, -287.6f, 211.7f), new Quaternion(0.0f, 0.9f, 0.0f, 0.4f),
                        new Vector3(-234.0f, 203.0f, 560.0f), new Quaternion(0.0f, 0.7f, 0.0f, 0.7f)
                    )}
                }
            },
            { "Akina", new Dictionary<string, RacePositionsData>
                {
                    { "Downhill", new RacePositionsData(
                        new Vector3(915.5f, 136.5f, 1323.3f), new Quaternion(0.0f, 1.0f, 0.0f, -0.1f), new Vector3(912.1f, 136.5f, 1323.7f), new Quaternion(0.0f, 1.0f, 0.0f, -0.1f),
                        new Vector3(915.5f, 136.5f, 1323.3f), new Quaternion(0.0f, 1.0f, 0.0f, -0.1f),
                        new Vector3(-1369.4f, -146.0f, -1083.6f), new Quaternion(0.0f, 0.3f, 0.0f, 1.0f)
                    )},
                    { "Uphill", new RacePositionsData(
                        new Vector3(-1372.4f, -145.4f, -1081.3f), new Quaternion(0.0f, 0.2f, 0.0f, 1.0f), new Vector3(-1369.9f, -145.4f, -1082.9f), new Quaternion(0.0f, 0.2f, 0.0f, 1.0f),
                        new Vector3(-1372.4f, -145.4f, -1081.3f), new Quaternion(0.0f, 0.2f, 0.0f, 1.0f),
                        new Vector3(912.4f, 137.0f, 1323.0f), new Quaternion(0.0f, 0.2f, 0.0f, 1.0f)
                    )}
                }
            },
            { "Akagi", new Dictionary<string, RacePositionsData>
                {
                    { "Downhill", new RacePositionsData(
                        new Vector3(-124.9f, 125.8f, -748.6f), new Quaternion(0.0f, -0.4f, 0.0f, -0.9f), new Vector3(-121.6f, 125.9f, -751.6f), new Quaternion(0.0f, -0.4f, 0.0f, -0.9f),
                        new Vector3(-124.9f, 125.8f, -748.6f), new Quaternion(0.0f, -0.4f, 0.0f, -0.9f),
                        new Vector3(693.2f, -134.5f, 320.0f), new Quaternion(0.0f, 0.4f, 0.0f, 0.9f)
                    )},
                    { "Uphill", new RacePositionsData(
                        new Vector3(693.9f, -134.6f, 320.5f), new Quaternion(0.0f, 0.4f, 0.0f, -0.9f), new Vector3(696.1f, -134.5f, 324.5f), new Quaternion(0.0f, 0.4f, 0.0f, -0.9f),
                        new Vector3(693.9f, -134.6f, 320.5f), new Quaternion(0.0f, 0.4f, 0.0f, -0.9f),
                        new Vector3(-124.2f, 125.5f, -748.0f), new Quaternion(0.0f, 0.4f, 0.0f, 0.9f)
                    )}
                }
            },
            { "USUI", new Dictionary<string, RacePositionsData>
                {
                    { "Downhill", new RacePositionsData(
                        new Vector3(1194.4f, 66.3f, 780.0f), new Quaternion(0.0f, -0.8f, 0.0f, 0.5f), new Vector3(1192.8f, 66.4f, 784.4f), new Quaternion(0.0f, -0.8f, 0.0f, 0.6f),
                        new Vector3(1194.4f, 66.3f, 780.0f), new Quaternion(0.0f, -0.8f, 0.0f, 0.5f),
                        new Vector3(-1554.9f, -211.4f, -711.5f), new Quaternion(0.0f, 0.7f, 0.0f, -0.7f)
                    )},
                    { "Uphill", new RacePositionsData(
                        new Vector3(-1554.5f, -213.4f, -745.5f), new Quaternion(0.0f, 0.0f, 0.0f, 1.0f), new Vector3(-1550.4f, -213.3f, -745.6f), new Quaternion(0.0f, 0.0f, 0.0f, 1.0f),
                        new Vector3(-1554.5f, -213.4f, -745.5f), new Quaternion(0.0f, 0.0f, 0.0f, 1.0f),
                        new Vector3(1270.0f, 68.4f, 784.5f), new Quaternion(0.0f, 0.0f, 0.0f, 1.0f)
                    )}
                }
            }
        };

        static Dictionary<string, RacePositionsData> defaultRaceData = new Dictionary<string, RacePositionsData>
        {
            { "Irohazaka", new RacePositionsData(
                        new Vector3(1232.7f, 260.2f, 55.5f), new Quaternion(0.0f, 1.0f, 0.0f, -0.3f), new Vector3(1230.2f, 260.2f, 57.0f), new Quaternion(0.0f, 1.0f, 0.0f, -0.3f),
                        new Vector3(1232.7f, 260.2f, 59.0f), new Quaternion(0.0f, -1.0f, 0.0f, 0.3f),
                        new Vector3(-1321.0f, -289.0f, 218.1f), new Quaternion(0.0f, 0.4f, 0.0f, -0.9f)
            )},
            { "Akina", new RacePositionsData(
                        new Vector3(875.1f, 136.2f, 1144.5f), new Quaternion(0.0f, 1.0f, 0.0f, -0.1f), new Vector3(871.8f, 136.0f, 1145.4f), new Quaternion(0.0f, 1.0f, 0.0f, -0.1f),
                        new Vector3(875.1f, 136.2f, 1144.5f), new Quaternion(0.0f, 1.0f, 0.0f, -0.1f),
                        new Vector3(-1382.4f, -146.0f, -1103.6f), new Quaternion(0.0f, 0.3f, 0.0f, 1.0f)
            )},
            { "Akagi", new RacePositionsData(
                        new Vector3(686.5f, -134.1f, 332.0f), new Quaternion(0.0f, -0.5f, 0.0f, 0.9f), new Vector3(684.1f, -134.1f, 328.2f), new Quaternion(0.0f, -0.5f, 0.0f, 0.9f),
                        new Vector3(686.5f, -134.1f, 332.0f), new Quaternion(0.0f, -0.5f, 0.0f, 0.9f),
                        new Vector3(-291.2f, 140.5f, -1058.0f), new Quaternion(0.0f, -0.7f, 0.0f, 0.7f)
            )},
            { "USUI", new RacePositionsData(
                        new Vector3(1366.8f, 66.1f, 784.3f), new Quaternion(0.0f, -0.7f, 0.0f, 0.7f), new Vector3(1366.6f, 66.1f, 779.1f), new Quaternion(0.0f, -0.7f, 0.0f, 0.7f),
                        new Vector3(1366.8f, 66.1f, 784.3f), new Quaternion(0.0f, -0.7f, 0.0f, 0.7f),
                        new Vector3(-1554.9f, -211.4f, -711.5f), new Quaternion(0.0f, 0.7f, 0.0f, -0.7f)
            )}
        };
    }
}