using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GG40.Data
{
    public class Configuration
    {
        public Task Load()
        {
            return Task.Run(() =>
            {
                var path = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, ".env");
                if (!File.Exists(path)) return;

                using (var sr = new StreamReader(path))
                {
                    while (!sr.EndOfStream)
                    {
                        var line = sr.ReadLine();
                        var lineSplit = line.Split("=");
                        if (lineSplit.Length < 2) continue;
                        switch (lineSplit[0])
                        {
                            case "PlasmaRedIndirizzoIp":
                                PlasmaRedIndirizzoIp = lineSplit[1];
                                break;

                            case "PlasmaRedLogDir":
                                PlasmaRedLogDir = lineSplit[1];
                                break;

                            case "PlasmaRedNpgDir":
                                PlasmaRedNpgDir = lineSplit[1];
                                break;

                            case "StazioneSaldanteLogDir":
                                StazioneSaldanteLogDir = lineSplit[1];
                                break;

                            case "DataSourceServerAddress":
                                DataSourceServerAddress = lineSplit[1];
                                break;

                            case "DataSourceServerPort":
                                DataSourceServerPort = lineSplit[1];
                                break;

                            case "DataSourceUser":
                                DataSourceUser = lineSplit[1];
                                break;

                            case "DataSourcePassword":
                                DataSourcePassword = lineSplit[1];
                                break;
                        }
                    }
                }
            });            
        }

        public Task Save()
        {
            return Task.Run(() =>
            {
                var path = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, ".env");
                if (File.Exists(path)) File.Delete(path);

                using (var sr = new StreamWriter(path))
                {
                    var line = $"PlasmaRedIndirizzoIp={PlasmaRedIndirizzoIp}";
                    sr.WriteLine(line);

                    line = $"PlasmaRedLogDir={PlasmaRedLogDir}";
                    sr.WriteLine(line);

                    line = $"PlasmaRedNpgDir={PlasmaRedNpgDir}";
                    sr.WriteLine(line);

                    line = $"StazioneSaldanteLogDir={StazioneSaldanteLogDir}";
                    sr.WriteLine(line);

                    line = $"DataSourceServerAddress={DataSourceServerAddress}";
                    sr.WriteLine(line);

                    line = $"DataSourceServerPort={DataSourceServerPort}";
                    sr.WriteLine(line);

                    line = $"DataSourceUser={DataSourceUser}";
                    sr.WriteLine(line);

                    line = $"DataSourcePassword={DataSourcePassword}";
                    sr.WriteLine(line);
                }
            });            
        }

        public string PlasmaRedIndirizzoIp { get; set; }
        public string PlasmaRedLogDir { get; set; }
        public string PlasmaRedNpgDir { get; set; }
        public string StazioneSaldanteLogDir { get; set; }
        public string DataSourceServerAddress { get; set; }
        public string DataSourceServerPort { get; set; }
        public string DataSourceUser { get; set; }
        public string DataSourcePassword { get; set; }

        private static Configuration _instance;
        public static Configuration Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Configuration();
                }
                return _instance;
            }
        }
    }
}
