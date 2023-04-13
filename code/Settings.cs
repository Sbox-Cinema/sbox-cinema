using Sandbox;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Cinema;

public partial class Settings
{
    public static Settings Current
    {
        get
        {
            if ( _instance == null )
            {
                _instance = new Settings();

                if ( FileSystem.OrganizationData.FileExists(SettingsFilename) )
                {
                    try
                    {
                        _instance = FileSystem.OrganizationData.ReadJson<Settings>(
                            SettingsFilename
                        );

                        var toRemove = new List<string>();
                        foreach ( var (key, value) in _instance.Values )
                        {
                            var j = (System.Text.Json.JsonElement)value;

                            if ( j.TryGetDecimal(out decimal parsedValue) )
                            {
                                _instance.Values[key] = (float)parsedValue;
                            }
                            else
                            {
                                toRemove.Add(key);
                            }
                        }

                        foreach ( var key in toRemove )
                        {
                            _instance.Values.Remove(key);
                        }
                    }
                    catch ( Exception e )
                    {
                        Log.Warning($"Failed to load settings file {e.Message}");
                    }
                }
            }

            return _instance;
        }
    }

    private static Settings _instance;

    public Dictionary<string, object> Values { get; set; } = new();

    public Settings()
    {
        Values["MusicVolume"] = 0.5f;
    }

    [JsonIgnore]
    public float MusicVolume => (float)Values["MusicVolume"];

    [JsonIgnore]
    public float AmbienceVolume => (float)Values["AmbienceVolume"];

    public readonly static string SettingsFilename = "cinema-settings.json";

    public void Save()
    {
        FileSystem.OrganizationData.WriteJson(SettingsFilename, this);
    }
}
