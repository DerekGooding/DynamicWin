﻿using Newtonsoft.Json;
using System.IO;

namespace DynamicWin.Utils;

internal static class SaveManager
{
    public static Dictionary<string, object> SaveData { get; set; } = [];

    public static string SavePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DynamicWin");
    private const string fileName = "Settings.json";

    private static string cachedJsonSave = "";

    public static void LoadData()
    {
        System.Diagnostics.Debug.WriteLine(SavePath);

        if (!Directory.Exists(SavePath)) Directory.CreateDirectory(SavePath);

        var fullPath = Path.Combine(SavePath, fileName);

        if (!File.Exists(fullPath))
        {
            var fs = File.Create(fullPath);
            fs.Close();
            File.WriteAllText(fullPath, JsonConvert.SerializeObject(new Dictionary<string, object>()));
        }

        var json = File.ReadAllText(fullPath);
        cachedJsonSave = json;

        SaveData = JsonConvert.DeserializeObject<Dictionary<string, object>>(json) ?? [];
    }

    public static void SaveAll()
    {
        if (!Directory.Exists(SavePath)) Directory.CreateDirectory(SavePath);

        var fullPath = Path.Combine(SavePath, fileName);
        var json = JsonConvert.SerializeObject(SaveData, Formatting.Indented);

        if (!File.Exists(fullPath))
            File.Create(fullPath);

        File.WriteAllText(fullPath, json);
    }

    public static void Add(string key, object value)
    {
        if (!Contains(key))
            SaveData.Add(key, value);
        else
            SaveData[key] = value;
    }

    public static void Remove(string key)
    {
        if (Contains(key))
            SaveData.Remove(key);
    }

    public static object? Get(string key) => Contains(key) ? SaveData[key] : default;

    public static T? Get<T>(string key) => Contains(key) ? JsonConvert.DeserializeObject<T>(cachedJsonSave) : default;

    public static bool Contains(string key) => SaveData.ContainsKey(key);
}