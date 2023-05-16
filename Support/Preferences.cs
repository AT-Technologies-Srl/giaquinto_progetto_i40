using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using Pomelo.EntityFrameworkCore.Lolita.Update;

namespace GG40.Support;

public class Preferences
{
    private string prefName;

    public Preferences(string prefName)
    {
        this.prefName = prefName;
    }

    private RegistryKey GetPreferences(bool edit)
    {
        RegistryKey rk = Registry.CurrentUser.OpenSubKey(prefName, edit);
        if (rk == null)
        {
            rk = Registry.CurrentUser.CreateSubKey(prefName);
        }            
        return rk;
    }

    public bool ExistsSubKey(string key)
    {
        RegistryKey prefs = GetPreferences(false);
        string[] subKeys = prefs.GetSubKeyNames();
        prefs.Close();

        return Array.Exists(subKeys, k => k.EqualsIgnoreCase(key));
    }

    public bool ExistsValueName(string valueName)
    {
        RegistryKey prefs = GetPreferences(false);
        string[] subKeys = prefs.GetValueNames();
        prefs.Close();

        return Array.Exists(subKeys, k => k.EqualsIgnoreCase(valueName));
    }

    public void DeleteSubKey(string key)
    {
        RegistryKey prefs = GetPreferences(true);
        prefs.DeleteSubKey(key);
        prefs.Close();
    }

    public void DeleteValueName(string valueName)
    {
        RegistryKey prefs = GetPreferences(true);
        prefs.DeleteValue(valueName);
        prefs.Close();
    }

    public void PutBool(string key, bool value)
    {
        RegistryKey prefs = GetPreferences(true);
        prefs.SetValue(key, value ? 1 : 0, RegistryValueKind.DWord);
        prefs.Close();
    }

    public bool GetBool(string key, bool defValue)
    {
        RegistryKey prefs = GetPreferences(false);
        object value = prefs.GetValue(key, defValue ? 1 : 0);
        prefs.Close();

        return ((int)value) == 1 ? true : false;
    }

    public void PutInt(string key, int value)
    {
        RegistryKey prefs = GetPreferences(true);
        prefs.SetValue(key, value, RegistryValueKind.DWord);
        prefs.Close();
    }

    public int GetInt(string key, int defValue)
    {
        RegistryKey prefs = GetPreferences(false);
        object value = prefs.GetValue(key, defValue);
        prefs.Close();

        return (int)value;
    }

    public void PutLong(string key, long value)
    {
        RegistryKey prefs = GetPreferences(true);
        prefs.SetValue(key, value, RegistryValueKind.QWord);
        prefs.Close();
    }

    public long GetLong(string key, long defValue)
    {
        RegistryKey prefs = GetPreferences(false);
        object value = prefs.GetValue(key, defValue);
        prefs.Close();

        return (long)value;
    }

    public void PutDouble(string key, double value)
    {
        RegistryKey prefs = GetPreferences(true);
        prefs.SetValue(key, value, RegistryValueKind.QWord);
        prefs.Close();
    }

    public double GetDouble(string key, double defValue)
    {
        RegistryKey prefs = GetPreferences(false);
        object value = prefs.GetValue(key, defValue);
        prefs.Close();

        return (double)value;
    }

    public void PutString(string key, string value)
    {
        RegistryKey prefs = GetPreferences(true);
        prefs.SetValue(key, value, RegistryValueKind.String);
        prefs.Close();
    }

    public string GetString(string key)
    {
        RegistryKey prefs = GetPreferences(false);
        object value = prefs.GetValue(key, "");
        prefs.Close();

        return (string)value;
    }

    public void PutStrings(string key, string[] value)
    {
        RegistryKey prefs = GetPreferences(true);
        prefs.SetValue(key, value, RegistryValueKind.MultiString);
        prefs.Close();
    }

    public string[] GetStrings(string key)
    {
        RegistryKey prefs = GetPreferences(false);
        object value = prefs.GetValue(key, new string[0]);
        prefs.Close();

        return (string[])value;
    }

    public void PutBytes(string key, byte[] value)
    {
        RegistryKey prefs = GetPreferences(true);
        prefs.SetValue(key, value, RegistryValueKind.Binary);
        prefs.Close();
    }

    public byte[] GetBytes(string key)
    {
        RegistryKey prefs = GetPreferences(false);
        object value = prefs.GetValue(key, new byte[0]);
        prefs.Close();

        return (byte[])value;
    }

    private static Preferences _instance;
    public static Preferences Instance
    {
        get
        {
            if (_instance == null)
            {
                Assembly curAssembly = Assembly.GetExecutingAssembly();
                var versionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(curAssembly.Location);

                string keyName = $@"Software\{versionInfo.ProductName}";
                return new Preferences(keyName);
            }
            return _instance;
        }
    }
}