using UnityEngine;
using BepInEx;
using UnityEngine.UI;
using System.IO;
using System;

namespace WarmTofuMod
{
    public partial class WarmTofuMod : BaseUnityPlugin
    {
        void Advanced_ColorPicker(GameObject Picker, string type_preset, Vector3 position, float scale_factor, int presets_count)
        {
            //Color picker
            Picker.transform.localPosition = position;
            Picker.transform.localScale = new Vector3(scale_factor, scale_factor, scale_factor);

            //Main Objects
            GameObject ColorPresets = Picker.transform.Find("Presets").gameObject;
            GameObject ColorMenu = Picker.transform.GetParent().gameObject;

            //Rebuild Presets Buttons
            GameObject CleanPreset = Instantiate(ColorPresets.transform.Find("Preset (4)").gameObject);
            CleanPreset.transform.SetParent(ColorPresets.transform.Find("Preset (4)").GetParent());
            CleanPreset.name = "Clean Preset";

            //Delete Old Presets Buttons
            Destroy_old_Presets(ColorPresets);

            //Destroy "Color Image" object in Color Presets
            Destroy(ColorPresets.GetComponent<UnityEngine.UI.Image>());

            //Create manipulate presets container
            GameObject Manipulate_Btn_Presets = Instantiate(ColorPresets);
            Manipulate_Btn_Presets.transform.SetParent(ColorPresets.transform.GetParent());
            Manipulate_Btn_Presets.transform.localScale = Vector3.one;
            Manipulate_Btn_Presets.name = "Manipulate_Presets_Buttons";
            Manipulate_Btn_Presets.SetActive(true);

            ColorPresets.transform.Find("Create Button").gameObject.SetActive(false);

            //Create "New Presets" Buttons
            Create_new_Presets(CleanPreset, type_preset, presets_count);

            //Setting the priority of the main "...NewPreset (0)" object
            ColorPresets.transform.Find($"{type_preset}NewPreset (0)").SetSiblingIndex(CleanPreset.transform.GetSiblingIndex());

            //Destroy objects
            Destroy(ColorPresets.transform.Find("Clean Preset").gameObject);
            Destroy(Manipulate_Btn_Presets.transform.Find("Clean Preset").gameObject);

            //Create manipulate buttons presets container
            Destroy_old_Presets(Manipulate_Btn_Presets);
            GameObject CreatePresetBtn = Manipulate_Btn_Presets.transform.Find("Create Button").gameObject;
            CreatePresetBtn.SetActive(false);

            //Recreate "Create Button"
            CreatePresetBtn = Instantiate(CreatePresetBtn);
            CreatePresetBtn.transform.SetParent(Manipulate_Btn_Presets.transform);
            CreatePresetBtn.transform.localScale = Vector3.one;
            CreatePresetBtn.name = "Main Create Button";
            CreatePresetBtn.GetComponent<UnityEngine.UI.Image>().color = Color.white;
            CreatePresetBtn.SetActive(true);

            Destroy(ColorPresets.GetComponent<UnityEngine.UI.Image>());

            //Create "Delete Button"
            GameObject DeletePresetBtn = Instantiate(CreatePresetBtn);
            DeletePresetBtn.transform.SetParent(Manipulate_Btn_Presets.transform);
            DeletePresetBtn.transform.localScale = Vector3.one;
            DeletePresetBtn.name = "Delete Button";
            DeletePresetBtn.GetComponent<UnityEngine.UI.Image>().color = Color.white;
            DeletePresetBtn.transform.Find("Text").gameObject.GetComponent<UnityEngine.UI.Text>().text = "-";

            Button CreateButtonComponent = CreatePresetBtn.GetComponent<Button>();
            CreateButtonComponent.onClick = new Button.ButtonClickedEvent();
            CreateButtonComponent.onClick.AddListener(() => CreatePreset(Picker, type_preset));

            Button DeleteButtonComponent = DeletePresetBtn.GetComponent<Button>();
            DeleteButtonComponent.onClick = new Button.ButtonClickedEvent();
            DeleteButtonComponent.onClick.AddListener(() => DeletePreset(Picker, type_preset));

            //Enable Slders Container
            GameObject ColorSliders = Picker.transform.Find("Sliders").gameObject;
            ColorSliders.SetActive(true);
            if (type_preset == "LightColor_")
            {
                GameObject Light_menu = Picker.transform.GetParent().gameObject;
                GameObject Header = Light_menu.transform.Find("Header").gameObject;
                GameObject Text_Header = Header.transform.Find("Text").gameObject;
                Header.transform.localPosition = new Vector3(668f, 310f, 1f);
                Header.transform.localScale = new Vector3(1.23f, 1f, 1f);
                Text_Header.transform.localScale = new Vector3(0.77f, 1f, 1f);

            }
            else 
            {
                //Stock Color Button
                GameObject StockColor = ColorMenu.transform.Find("Stock_Color").gameObject;
                StockColor.transform.GetComponent<UnityEngine.UI.Outline>().effectColor = Color.clear;
                StockColor.transform.localPosition = new Vector3(845f, 310f, 0f);
                StockColor.transform.localScale = new Vector3(3f, 1.5f, 1f);
                StockColor.SetActive(true);

                //Title for Stock Color Button
                GameObject StockColorText = Instantiate(ColorMenu.transform.Find("Title").gameObject);
                StockColorText.name = "StockColor_Title";
                StockColorText.transform.SetParent(ColorMenu.transform);
                StockColorText.transform.SetSiblingIndex(StockColor.transform.GetSiblingIndex());
                StockColorText.transform.localPosition = new Vector3(600f, 310f, 0f);
                StockColorText.transform.localScale = Vector3.one;
                StockColorText.transform.GetComponent<UnityEngine.UI.Text>().text = "Stock Color";
            }
            //Destroy Seperator
            Destroy(ColorSliders.transform.Find("Seperator").gameObject);

            //Cusstomization Colors in HSV/RGB Fields Values
            Set_color_HSV_RGB_Fields(ColorSliders, Color.white);

            InitPresets(Picker, type_preset, presets_count);
        }
        
        void InitPresets(GameObject Picker, string type_preset, int count_presets)
        {
            GameObject ColorPresets = Picker.transform.Find("Presets").gameObject;

            for (int i = 0; i <= count_presets - 1; i++)
            {
                if (PlayerPrefs.HasKey($"{type_preset}NewPreset ({i})"))
                {
                    string presetValue = PlayerPrefs.GetString($"{type_preset}NewPreset ({i})");
                    ColorPresets.transform.Find($"{type_preset}NewPreset ({i})").gameObject.SetActive(ParsePlayerPref_Active(presetValue));
                }
                else { continue; }
            }
        }


        void CreatePreset(GameObject Picker, string type_preset)
        {
            GameObject ColorPresets = Picker.transform.Find("Presets").gameObject;
            Color CurrentColor = ColorPresets.transform.Find("Create Button").GetComponent<UnityEngine.UI.Image>().color;
            int childCount = ColorPresets.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Transform child = ColorPresets.transform.GetChild(i);
                if (!child.gameObject.activeSelf && child.name.StartsWith($"{type_preset}NewPreset"))
                {
                    child.gameObject.SetActive(true);
                    child.gameObject.transform.GetComponent<UnityEngine.UI.Image>().color = CurrentColor;
                    string playerpref = $"{CurrentColor.r}-{CurrentColor.g}-{CurrentColor.b}-{CurrentColor.a}-{child.gameObject.activeSelf}";
                    PlayerPrefs.SetString(child.name, playerpref);
                    return;
                }
            }
        }

        void DeletePreset(GameObject Picker, string type_preset)
        {
            GameObject ColorPresets = Picker.transform.Find("Presets").gameObject;
            int childCount = ColorPresets.transform.childCount;
            for (int i = childCount - 1; i >= 0; i--)
            {
                Transform child = ColorPresets.transform.GetChild(i);
                if (child.gameObject.activeSelf && child.name.StartsWith($"{type_preset}NewPreset"))
                {
                    child.gameObject.SetActive(false);
                    Color CurrentColor = child.gameObject.transform.GetComponent<UnityEngine.UI.Image>().color = Color.white;
                    string playerpref = $"{CurrentColor.r}-{CurrentColor.g}-{CurrentColor.b}-{CurrentColor.a}-{child.gameObject.activeSelf}";
                    PlayerPrefs.SetString(child.name, playerpref);
                    return;
                }
            }
        }

        static Color ParsePlayerPref_Color(string playerpref)
        {
            string[] parts = playerpref.Split('-');

            float ParseFloat(string value)
            {
                float.TryParse(value.Replace(',', '.'), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float result);
                return result;
            }

            float r = ParseFloat(parts[0]);
            float g = ParseFloat(parts[1]);
            float b = ParseFloat(parts[2]);
            float a = ParseFloat(parts[3]);
            return new Color(r, g, b, a);
        }

        static bool ParsePlayerPref_Active(string playerpref)
        {
            string[] parts = playerpref.Split('-');
            return bool.Parse(parts[4]);
        }

        void UpdateColor_ActivePresets(GameObject Picker, string type_preset, int count_presets)
        {
            GameObject ColorPresets = Picker.transform.Find("Presets").gameObject;

            for (int i = 0; i <= count_presets - 1; i++)
            {
                GameObject presetObject = ColorPresets.transform.Find($"{type_preset}NewPreset ({i})").gameObject;

                if (presetObject.activeSelf == true)
                {
                    presetObject.GetComponent<UnityEngine.UI.Image>().color = ParsePlayerPref_Color(PlayerPrefs.GetString(presetObject.name));
                }
            }
        }

        //Parametric Methods
        void Destroy_old_Presets(GameObject old_presets)
        {
            for (int i = 0; i <= 10; i++)
            {
                Destroy(old_presets.transform.Find($"Preset ({i})").gameObject);
            }
        }

        void Create_new_Presets(GameObject clean_preset_button, string type_preset, int count_presets)
        {
            for (int i = 0; i <= count_presets - 1; i++)
            {
                GameObject NewBtnPreset = Instantiate(clean_preset_button);
                NewBtnPreset.transform.SetParent(clean_preset_button.transform.GetParent());
                NewBtnPreset.name = $"{type_preset}NewPreset ({i})";
                NewBtnPreset.transform.localScale = Vector3.one;
            }
        }

        void Active_ColorSliders_In_Picker(GameObject Picker, bool Active) 
        {
            GameObject ColorSliders = Picker.transform.Find("Sliders").gameObject;
            ColorSliders.SetActive(Active);
            //Button enable sliders RGB/HSV
            GameObject Toggle = ColorSliders.transform.Find("Toggle").gameObject;
            Toggle.SetActive(Active);
            GameObject ColorBtn = Toggle.transform.Find("Button").gameObject;
            ColorBtn.SetActive(Active);
            GameObject Text_btn = ColorBtn.transform.Find("Text (TMP)").gameObject;
            Text_btn.SetActive(Active);
            Text_btn.GetComponent<TMPro.TextMeshProUGUI>().text = "Color";
            ColorSliders.transform.Find("Seperator").gameObject.SetActive(false);
        }

        void Set_color_HSV_RGB_Fields(GameObject ColorSliders, Color color)
        {
            ColorSliders.transform.Find("R").Find("LeftText (TMP)").transform.GetComponent<TMPro.TextMeshProUGUI>().color = color;
            ColorSliders.transform.Find("R").Find("ValueText (TMP)").gameObject.SetActive(false);

            ColorSliders.transform.Find("G").Find("LeftText (TMP)").transform.GetComponent<TMPro.TextMeshProUGUI>().color = color;
            ColorSliders.transform.Find("G").Find("ValueText (TMP)").gameObject.SetActive(false);

            ColorSliders.transform.Find("B").Find("LeftText (TMP)").transform.GetComponent<TMPro.TextMeshProUGUI>().color = color;
            ColorSliders.transform.Find("B").Find("ValueText (TMP)").gameObject.SetActive(false);

            ColorSliders.transform.Find("H").Find("LeftText (TMP)").transform.GetComponent<TMPro.TextMeshProUGUI>().color = color;
            ColorSliders.transform.Find("H").Find("ValueText (TMP)").gameObject.SetActive(false);

            ColorSliders.transform.Find("S").Find("LeftText (TMP)").transform.GetComponent<TMPro.TextMeshProUGUI>().color = color;
            ColorSliders.transform.Find("S").Find("ValueText (TMP)").gameObject.SetActive(false);

            ColorSliders.transform.Find("V").Find("LeftText (TMP)").transform.GetComponent<TMPro.TextMeshProUGUI>().color = color;
            ColorSliders.transform.Find("V").Find("ValueText (TMP)").gameObject.SetActive(false);
        }
    }
}
