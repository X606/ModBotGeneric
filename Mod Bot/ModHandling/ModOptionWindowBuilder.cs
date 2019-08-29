﻿using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using ModLibrary;
using InternalModBot;

namespace ModLibrary
{
    /// <summary>
    /// Used to place all of the options in the options window
    /// </summary>
    public class ModOptionsWindowBuilder
    {
        private readonly GameObject Content;
        private readonly GameObject SpawnedBase;
        private readonly Button XButton;
        private readonly GameObject Owner;
        private readonly Mod OwnerMod;

        internal ModOptionsWindowBuilder(GameObject owner, Mod ownerMod)
        {
            owner.SetActive(false);
            Owner = owner;
            OwnerMod = ownerMod;
            GameObject modsWindowPrefab = AssetLoader.GetObjectFromFile("modswindow", "ModOptionsCanvas", "Clone Drone in the Danger Zone_Data/");
            SpawnedBase = GameObject.Instantiate(modsWindowPrefab);
            ModdedObject modObject = SpawnedBase.GetComponent<ModdedObject>();
            Content = modObject.GetObject<GameObject>(0);
            XButton = modObject.GetObject<Button>(1);
            XButton.onClick.AddListener(CloseWindow);


            // Debug stuff
            // int RandomNumber = UnityEngine.Random.Range(0, 40);

            // OptionsSaver.SaveString(OwnerMod, "test" + RandomNumber, DEBUG_RandomString(10) );
            // debug.Log( OptionsSaver.LoadString(OwnerMod, "testStringHaHa") );
        }

        /// <summary>
        /// Adds a slider, note that the value of the slider will be saved by Mod-Bot so you dont need to save it in a ny way
        /// </summary>
        /// <param name="min">The minimum value of the slider</param>
        /// <param name="max">The maximum value of the slider</param>
        /// <param name="defaultValue">The value the slider will be set to before it is changed by the user</param>
        /// <param name="name">The name of the slider, this will both be displayed to the user and used in the mod to get the value (no 2 names should EVER be the same)</param>
        /// <param name="onChange">A callback that gets called when the slider gets changed, if null wont do anything</param>
        public void AddSlider(float min, float max, float defaultValue, string name, Action<float> onChange = null)
        {
            GameObject SliderPrefab = AssetLoader.GetObjectFromFile("modswindow", "Slider", "Clone Drone in the Danger Zone_Data/");
            ModdedObject moddedObject = GameObject.Instantiate(SliderPrefab).GetComponent<ModdedObject>();
            moddedObject.transform.parent = Content.transform;
            moddedObject.GetObject<Text>(0).text = name;
            Slider slider = moddedObject.GetObject<Slider>(1);
            slider.minValue = min;
            slider.maxValue = max;
            slider.value = defaultValue;
            Text numberDisplay = moddedObject.GetObject<Text>(2);

            float? loadedFloat = OptionsSaver.LoadFloat(OwnerMod, name);
            if (loadedFloat.HasValue)
            {
                slider.value = loadedFloat.Value;
            }
            onChange?.Invoke(slider.value);
            numberDisplay.text = slider.value.ToString();
            slider.onValueChanged.AddListener(delegate (float value) { OptionsSaver.SaveFloat(OwnerMod, name, value); onChange?.Invoke(value); numberDisplay.text = value.ToString(); });
        }
        /// <summary>
        /// Adds a slider to the options window that can only be whole numbers
        /// </summary>
        /// <param name="min">The minimum value of the slider</param>
        /// <param name="max">That maximum value of the slider</param>
        /// <param name="defaultValue">The value the slider will be set to before it is changed by the user</param>
        /// <param name="name">Both the display name in the list and used by you to get the value (no 2 names should EVER be the same)</param>
        /// <param name="onChange">Called when the value is changed, if null does nothing</param>
        public void AddIntSlider(int min, int max, int defaultValue, string name, Action<int> onChange = null)
        {
            GameObject SliderPrefab = AssetLoader.GetObjectFromFile("modswindow", "Slider", "Clone Drone in the Danger Zone_Data/");
            ModdedObject moddedObject = GameObject.Instantiate(SliderPrefab).GetComponent<ModdedObject>();
            moddedObject.transform.parent = Content.transform;
            moddedObject.GetObject<Text>(0).text = name;
            Slider slider = moddedObject.GetObject<Slider>(1);
            slider.minValue = min;
            slider.maxValue = max;
            slider.wholeNumbers = true;
            slider.value = defaultValue;
            Text numberDisplay = moddedObject.GetObject<Text>(2);

            int? loadedInt = OptionsSaver.LoadInt(OwnerMod, name);
            if (loadedInt.HasValue)
            {
                slider.value = loadedInt.Value;
            }
            onChange?.Invoke((int)slider.value);
            numberDisplay.text = slider.value.ToString();
            slider.onValueChanged.AddListener(delegate (float value) { OptionsSaver.SaveInt(OwnerMod, name, (int)value); onChange?.Invoke((int)value); numberDisplay.text = value.ToString(); });
        }
        /// <summary>
        /// Adds a checkbox to the mods window
        /// </summary>
        /// <param name="defaultValue">The value the checkbox will be set to before the user changes it</param>
        /// <param name="name">Both the display name of the checkbox and what you use to get the value of the checkbox (no 2 names should EVER be the same)</param>
        /// <param name="onChange">Called when the value of the checkbox is changed, if null does nothing</param>
        public void AddCheckbox(bool defaultValue, string name, Action<bool> onChange = null)
        {
            GameObject CheckBoxPrefab = AssetLoader.GetObjectFromFile("modswindow", "Checkbox", "Clone Drone in the Danger Zone_Data/");
            GameObject spawnedObject = GameObject.Instantiate(CheckBoxPrefab);
            spawnedObject.transform.parent = Content.transform;
            ModdedObject moddedObject = spawnedObject.GetComponent<ModdedObject>();
            Toggle toggle = moddedObject.GetObject<Toggle>(0);
            toggle.isOn = defaultValue;
            moddedObject.GetObject<GameObject>(1).GetComponent<Text>().text = name;

            bool? loadedBool = OptionsSaver.LoadBool(OwnerMod, name);
            if (loadedBool.HasValue)
            {
                toggle.isOn = loadedBool.Value;
            }
            onChange?.Invoke(toggle.isOn);
            toggle.onValueChanged.AddListener(delegate (bool value) { OptionsSaver.SaveBool(OwnerMod, name, value); onChange?.Invoke(value); });
        }
        /// <summary>
        /// Adds a input field to the mods window
        /// </summary>
        /// <param name="defaultValue">The defualt value before it is edited by the user</param>
        /// <param name="name">Name used both as a display name and as a key for you to get the value by later (no 2 names should EVER be the same)</param>
        /// <param name="onChange">Gets called when the value of the inputField gets changed, if null doesnt nothing</param>
        public void AddInputField(string defaultValue, string name, Action<string> onChange = null)
        {
            GameObject InputFieldPrefab = AssetLoader.GetObjectFromFile("modswindow", "InputField", "Clone Drone in the Danger Zone_Data/");
            GameObject spawnedPrefab = GameObject.Instantiate(InputFieldPrefab);
            spawnedPrefab.transform.parent = Content.transform;
            ModdedObject spawnedModdedObject = spawnedPrefab.GetComponent<ModdedObject>();
            spawnedModdedObject.GetObject<Text>(0).text = name;
            InputField field = spawnedModdedObject.GetObject<InputField>(1);
            field.text = defaultValue;

            string loadedString = OptionsSaver.LoadString(OwnerMod, name);
            if (loadedString != null)
            {
                field.text = loadedString;
            }
            onChange?.Invoke(field.text);
            field.onValueChanged.AddListener(delegate (string value) { OptionsSaver.SaveString(OwnerMod, name, value); onChange?.Invoke(value); });
        }
        /// <summary>
        /// Adds a dropdown to the mods window
        /// </summary>
        /// <param name="options">The diffrent options that should be selectable</param>
        /// <param name="defaultIndex">what index in the previus array will be selected before the user edits it</param>
        /// <param name="name">Display name and key for you later (no 2 names should EVER be the same)</param>
        /// <param name="onChange">Gets called when the value of the dropdown is changed, if null does nothing</param>
        public void AddDropdown(string[] options, int defaultIndex, string name, Action<int> onChange = null)
        {
            if (options.Length <= defaultIndex || defaultIndex < 0)
                return;

            GameObject dropdownPrefab = AssetLoader.GetObjectFromFile("modswindow", "DropDown", "Clone Drone in the Danger Zone_Data/");
            GameObject spawnedPrefab = GameObject.Instantiate(dropdownPrefab);
            spawnedPrefab.transform.parent = Content.transform;
            ModdedObject spawnedModdedObject = spawnedPrefab.GetComponent<ModdedObject>();
            spawnedModdedObject.GetObject<Text>(0).text = name;

            Dropdown dropdown = spawnedModdedObject.GetObject<Dropdown>(1);
            dropdown.options.Clear();
            
            foreach (string option in options)
            {
                Dropdown.OptionData data = new Dropdown.OptionData(option);
                dropdown.options.Add(data);
            }
            dropdown.value = defaultIndex;
            dropdown.RefreshShownValue();

            int? loadedInt = OptionsSaver.LoadInt(OwnerMod, name);
            if (loadedInt.HasValue)
            {
                dropdown.value = loadedInt.Value;
                dropdown.RefreshShownValue();
            }
            onChange?.Invoke(dropdown.value);
            dropdown.onValueChanged.AddListener(delegate (int value) { OptionsSaver.SaveInt(OwnerMod, name, value); onChange?.Invoke(value); });
        }
        /// <summary>
        /// Adds a dropdown to the options window
        /// </summary>
        /// <typeparam name="T">Must be a enum, the options of this enum will be displayed as the options of the dropdown</typeparam>
        /// <param name="defaultIndex">The index in the enum that will be selected before the user edits it</param>
        /// <param name="name">Display name and key to get value (no 2 names should EVER be the same)</param>
        /// <param name="onChange"></param>
        public void AddDropDown<T>(int defaultIndex, string name, Action<int> onChange = null) where T : IComparable, IFormattable, IConvertible
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("The generic type T must be an enum type");
            }
            List<string> enums = ModTools.EnumTools.GetNames<T>();
            AddDropdown(enums.ToArray(), defaultIndex, name, onChange);

        }
        /// <summary>
        /// Adds a button to the options window
        /// </summary>
        /// <param name="text">The text displayed on the button</param>
        /// <param name="callback">Called when the user clicks the button</param>
        public void AddButton(string text, UnityEngine.Events.UnityAction callback)
        {
            GameObject buttonPrefab = AssetLoader.GetObjectFromFile("modswindow", "Button", "Clone Drone in the Danger Zone_Data/");
            GameObject spawnedPrefab = GameObject.Instantiate(buttonPrefab);
            spawnedPrefab.transform.parent = Content.transform;
            ModdedObject spawnedModdedObject = spawnedPrefab.GetComponent<ModdedObject>();
            spawnedModdedObject.GetObject<Button>(0).onClick.AddListener(callback);
            spawnedModdedObject.GetObject<Text>(1).text = text;
        }
        /// <summary>
        /// Adds a plain text to the options window
        /// </summary>
        /// <param name="text">string that will be displayed</param>
        public void AddLabel(string text)
        {
            GameObject labelPrefab = AssetLoader.GetObjectFromFile("modswindow", "Label", "Clone Drone in the Danger Zone_Data/");
            GameObject spawnedPrefab = GameObject.Instantiate(labelPrefab);
            spawnedPrefab.transform.parent = Content.transform;
            ModdedObject spawnedModdedObject = spawnedPrefab.GetComponent<ModdedObject>();
            spawnedModdedObject.GetObject<Text>(0).text = text;
        }

        /// <summary>
        /// Closes the options window, this also opens its parent window (probably the mods window)
        /// </summary>
        public void CloseWindow()
        {
            GameObject.Destroy(SpawnedBase);
            Owner.SetActive(true);
        }

        /// <summary>
        /// Closes the options window, does NOT open the parent window
        /// </summary>
        public void ForceCloseWindow()
        {
            GameObject.Destroy(SpawnedBase);
        }


    }
}
namespace InternalModBot {

    /// <summary>
    /// Used by Mod-Bot to save mod options
    /// </summary>
    public static class OptionsSaver
    {
        private static List<KeyValuePair<string, object>> Loadedkeys = new List<KeyValuePair<string, object>>();

        [JsonIgnore]
        private readonly static string Path;

        static OptionsSaver()
        {
            Path = Application.persistentDataPath + "/SavedModSettings.json";
            if (System.IO.File.Exists(Path))
            {
                string json = System.IO.File.ReadAllText(Path);
                Load(json);
            }
        }
        /// <summary>
        /// Sets the loaded options from an input json string
        /// </summary>
        /// <param name="json"></param>
        public static void Load(string json)
        {
            Loadedkeys = JsonConvert.DeserializeObject<List<KeyValuePair<string, object>>>(json);
            if (Loadedkeys == null)
            {
                Loadedkeys = new List<KeyValuePair<string, object>>();
            }
        }
        /// <summary>
        /// Saves the current loaded options to a file
        /// </summary>
        private static void Save()
        {
            string json = JsonConvert.SerializeObject(Loadedkeys);
            System.IO.File.WriteAllText(Path, json);
        }

        /// <summary>
        /// Used to make sure that mods always get saved in the same format
        /// </summary>
        /// <param name="mod"></param>
        /// <param name="saveFormat"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static string GenerateSaveFormatString(Mod mod, SaveFormats saveFormat, string name)
        {
            string generatedString = mod.GetUniqueID();
            switch (saveFormat)
            {
                case SaveFormats.String:
                    generatedString += "_str_";
                    break;
                case SaveFormats.Int:
                    generatedString += "_int_";
                    break;
                case SaveFormats.Float:
                    generatedString += "_flt_";
                    break;
                case SaveFormats.Bool:
                    generatedString += "_bol_";
                    break;
            }
            generatedString += name;
            return generatedString;
        }

        /// <summary>
        /// The diffrent types of save formats supported
        /// </summary>
        private enum SaveFormats
        {
            String,
            Int,
            Float,
            Bool
        }

        private static int? GetIndexOfKey(string key)
        {
            for (int i = 0; i < Loadedkeys.Count; i++)
            {
                if (Loadedkeys[i].Key == key)
                {
                    return i;
                }
            }

            return null;
        }

        /// <summary>
        /// Saves a string to the save file and loaded options
        /// </summary>
        /// <param name="mod"></param>
        /// <param name="name"></param>
        /// <param name="_string"></param>
        public static void SaveString(Mod mod, string name, string _string)
        {
            string key = GenerateSaveFormatString(mod, SaveFormats.String, name);
            int? index = GetIndexOfKey(key);
            if (index != null)
            {
                Loadedkeys[index.Value] = new KeyValuePair<string, object>(key, _string);
            } else
            {
                Loadedkeys.Add(new KeyValuePair<string, object>(key, _string));
            }

            Save();
        }
        /// <summary>
        /// Loads a string from the loaded options
        /// </summary>
        /// <param name="mod"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string LoadString(Mod mod, string name)
        {
            if (Loadedkeys == null)
                return null;

            string outputString = null;
            foreach (KeyValuePair<string, object> value in Loadedkeys)
            {
                if (value.Key == GenerateSaveFormatString(mod, SaveFormats.String, name))
                {
                    outputString = value.Value as string;
                    debug.Log("setting string stuff a \"" + outputString + "\"");
                    break;
                }
            }

            return outputString;
        }

        /// <summary>
        /// Saves a int to the save file and loaded options
        /// </summary>
        /// <param name="mod"></param>
        /// <param name="name"></param>
        /// <param name="_int"></param>
        public static void SaveInt(Mod mod, string name, int _int)
        {
            string key = GenerateSaveFormatString(mod, SaveFormats.Int, name);
            int? index = GetIndexOfKey(key);
            if (index != null)
            {
                Loadedkeys[index.Value] = new KeyValuePair<string, object>(key, _int);
            }
            else
            {
                Loadedkeys.Add(new KeyValuePair<string, object>(key, _int));
            }
            
            Save();
        }
        /// <summary>
        /// loads an int from the loaded options
        /// </summary>
        /// <param name="mod"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static int? LoadInt(Mod mod, string name)
        {
            if (Loadedkeys == null)
                return null;

            int? outputInt = null;
            foreach (KeyValuePair<string, object> value in Loadedkeys)
            {
                if (value.Key == GenerateSaveFormatString(mod, SaveFormats.Int, name))
                {
                    outputInt = value.Value as int?;
                    break;
                }
            }
            
            return outputInt;
        }

        /// <summary>
        /// Saves a float to the save file and loaded options
        /// </summary>
        /// <param name="mod"></param>
        /// <param name="name"></param>
        /// <param name="_float"></param>
        public static void SaveFloat(Mod mod, string name, float _float)
        {
            string key = GenerateSaveFormatString(mod, SaveFormats.Float, name);
            int? index = GetIndexOfKey(key);
            if (index != null)
            {
                Loadedkeys[index.Value] = new KeyValuePair<string, object>(key, _float);
            }
            else
            {
                Loadedkeys.Add(new KeyValuePair<string, object>(key, _float));
            }

            Save();
        }
        /// <summary>
        /// loads a float from the loaded options
        /// </summary>
        /// <param name="mod"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static float? LoadFloat(Mod mod, string name)
        {
            if (Loadedkeys == null)
                return null;

            float? outputFloat = null;
            foreach (KeyValuePair<string, object> value in Loadedkeys)
            {
                if (value.Key == GenerateSaveFormatString(mod, SaveFormats.Float, name))
                {
                    outputFloat = value.Value as float?;
                    break;
                }
            }

            return outputFloat;
        }

        /// <summary>
        /// Saves a bool to the save file and loaded options
        /// </summary>
        /// <param name="mod"></param>
        /// <param name="name"></param>
        /// <param name="_bool"></param>
        public static void SaveBool(Mod mod, string name, bool _bool)
        {
            string key = GenerateSaveFormatString(mod, SaveFormats.Bool, name);
            int? index = GetIndexOfKey(key);
            if (index != null)
            {
                Loadedkeys[index.Value] = new KeyValuePair<string, object>(key, _bool);
            }
            else
            {
                Loadedkeys.Add(new KeyValuePair<string, object>(key, _bool));
            }

            Save();
        }
        /// <summary>
        /// loads a bool from the loaded options
        /// </summary>
        /// <param name="mod"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool? LoadBool(Mod mod, string name)
        {
            if (Loadedkeys == null)
                return null;

            bool? outputBool = null;
            foreach (KeyValuePair<string, object> value in Loadedkeys)
            {
                if (value.Key == GenerateSaveFormatString(mod, SaveFormats.Bool, name))
                {
                    outputBool = value.Value as bool?;
                    break;
                }
            }

            return outputBool;
        }
    }

}