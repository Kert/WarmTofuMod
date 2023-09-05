using BepInEx;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace WarmTofuMod
{
    public partial class WarmTofuMod : BaseUnityPlugin
    {
        public partial class CustomRaceManager : MonoBehaviour
        {
            private class CustomRaceMenu
            {
                private static GameObject menu;
                private static Button start;
                private static GameObject parentUI;
                private static Dropdown directionDropdown;
                private static Dropdown orderDropdown;
                private static Toggle nitroToggle;
                private static Toggle collisionToggle;
                private static List<string> trackList;

                public CustomRaceMenu(GameObject parent)
                {
                    menu = Instantiate(Instantiate(GameObject.FindObjectOfType<SRUIManager>().MenuExit));
                    menu.name = "Custom Race Menu";

                    parentUI = parent.GetComponent<SRPlayerListRoom>().PlayerListUI;
                    Transform transform = menu.transform;
                    transform.SetParent(parentUI.transform);
                    transform.position = new Vector3(0, 0, 0);
                    transform.localScale = new Vector3(1f, 1f, 1f);

                    menu.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, 0, 0);
                    menu.GetComponentInChildren<Text>().text = "Battle settings";

                    Button[] battleButtons = menu.GetComponentsInChildren<Button>();
                    Destroy(battleButtons[1].gameObject);
                    start = battleButtons[0];

                    start.gameObject.GetComponentInChildren<Text>().text = "Send race invitation";
                    start.onClick = new Button.ButtonClickedEvent();
                    start.onClick.AddListener(SetRaceSettings);
                    start.onClick.AddListener(() => parentUI.SetActive(false));
                    start.onClick.AddListener(() => menu.SetActive(false));
                    start.onClick.AddListener(CustomRaceManager.SendBattleInvitation);

                    RectTransform rect = start.GetComponent<RectTransform>();
                    rect.anchorMin = new Vector2(0.6f, 0.5f);
                    rect.anchorMax = new Vector2(0.78f, 0.5f);
                    rect.anchoredPosition = new Vector2(0f, -110f);

                    Dropdown dummyDropdown = GameObject.FindObjectOfType<SRUIManager>().MenuSettings.GetComponentInChildren<Dropdown>();
                    Toggle dummyToggle = GameObject.FindObjectOfType<SRUIManager>().MenuSettings.GetComponentInChildren<Toggle>();

                    directionDropdown = Instantiate(dummyDropdown);
                    orderDropdown = Instantiate(dummyDropdown);
                    nitroToggle = Instantiate(dummyToggle);
                    collisionToggle = Instantiate(dummyToggle);
                    nitroToggle.onValueChanged = new Toggle.ToggleEvent();
                    collisionToggle.onValueChanged = new Toggle.ToggleEvent();
                    directionDropdown.transform.SetParent(menu.transform);
                    Destroy(directionDropdown.gameObject.transform.FindChild("EnTutoPS4").gameObject);
                    orderDropdown.transform.SetParent(menu.transform);
                    Destroy(orderDropdown.gameObject.transform.FindChild("EnTutoPS4").gameObject);
                    nitroToggle.transform.SetParent(menu.transform);
                    collisionToggle.transform.SetParent(menu.transform);
                    directionDropdown.name = "TrackDropdown";
                    orderDropdown.name = "OrderDropdown";
                    nitroToggle.name = "NitroToggle";
                    collisionToggle.name = "CollisionToggle";

                    rect = directionDropdown.GetComponent<RectTransform>();
                    rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
                    rect.anchoredPosition3D = new Vector3(-335f, 100, 0);
                    rect.sizeDelta = new Vector2(450f, 100f);

                    rect = orderDropdown.GetComponent<RectTransform>();
                    rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
                    rect.anchoredPosition3D = new Vector3(-25f, 100, 0);
                    rect.sizeDelta = new Vector2(450f, 100f);

                    rect = nitroToggle.GetComponent<RectTransform>();
                    rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
                    rect.anchoredPosition3D = new Vector3(300f, 100, 0);
                    rect.sizeDelta = new Vector2(280f, 85f);

                    rect = collisionToggle.GetComponent<RectTransform>();
                    rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
                    rect.anchoredPosition3D = new Vector3(300f, 0, 0);
                    rect.sizeDelta = new Vector2(280f, 85f);

                    rect = menu.GetComponent<RectTransform>();
                    rect.anchorMin = new Vector2(0.2f, 0.1f);
                    rect.anchorMax = new Vector2(0.8f, 0.7f);
                    rect.anchoredPosition3D = new Vector3(0, 0, 0);
                    rect.sizeDelta = new Vector2(490f, 150f);

                    rect = menu.transform.FindChild("White_BG").gameObject.GetComponent<RectTransform>();
                    rect.anchorMin = new Vector2(0.2f, 0.22f);
                    rect.anchorMax = new Vector2(0.8f, 0.68f);
                    rect.anchoredPosition3D = new Vector3(0, 0, 0);
                    rect.sizeDelta = new Vector2(410f, 200f);

                    nitroToggle.gameObject.GetComponentInChildren<Text>().text = "Nitro";
                    collisionToggle.gameObject.GetComponentInChildren<Text>().text = "Collision";

                    orderDropdown.ClearOptions();
                    orderDropdown.AddOptions(new List<string> { "Parallel", "You lead", "You chase" });

                    directionDropdown.ClearOptions();
                    trackList = customRaceData[SceneManager.GetActiveScene().name].Keys.ToList();
                    directionDropdown.AddOptions(trackList);
                    directionDropdown.template.sizeDelta = new Vector2(0, 500f);
                }

                public static void SetRaceSettings()
                {
                    CustomRaceManager.raceSettings.direction = directionDropdown.options[directionDropdown.value].text;
                    CustomRaceManager.raceSettings.order = orderDropdown.options[orderDropdown.value].text;
                    CustomRaceManager.raceSettings.nitro = nitroToggle.isOn;
                    CustomRaceManager.raceSettings.collision = collisionToggle.isOn;
                }

                public static GameObject GetMenu()
                {
                    return menu;
                }
            }
        }
    }
}