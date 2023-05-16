using CefSharp;
using CefSharp.Wpf;
using System;
using System.IO;
using System.Windows;


//https://www.flaticon.com/packs/essential-set-2

namespace GG40
{
    public partial class App : Application
    {
        public App()
        {
#if ANYCPU
            //Only required for PlatformTarget of AnyCPU
            CefRuntime.SubscribeAnyCpuAssemblyResolver();
#endif
            var cachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "GG4.0\\Cache");
            if (!Directory.Exists(cachePath))
            {
                Directory.CreateDirectory(cachePath);
            }

            var settings = new CefSettings()
            {
                CachePath = cachePath
            };
            
            settings.CefCommandLineArgs.Add("enable-media-stream");
            settings.CefCommandLineArgs.Add("use-fake-ui-for-media-stream");
            settings.CefCommandLineArgs.Add("enable-usermedia-screen-capturing");
            settings.CefCommandLineArgs.Add("enable-automatic-password-saving", "enable-automatic-password-saving");
            settings.CefCommandLineArgs.Add("enable-password-save-in-page-navigation", "enable-password-save-in-page-navigation");
            settings.PersistSessionCookies = true;

            if (!Cef.IsInitialized)
            {
                Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);
            }
        }
    }
}

//move \data\*.xls \second_q\reports\