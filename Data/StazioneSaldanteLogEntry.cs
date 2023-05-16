using GG40.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GG40.Data
{

    [Table("log_stazione_saldante")]
    public class StazioneSaldanteLogEntry
    {
        [Column("id")]
        public string Id { get; set; }

        [Column("date_time")]
        public DateTime? DateTime { get; set; }

        [Column("ot_device")]
        public string? OtDevice { get; set; }

        [Column("protocol")]
        public string? Protocol { get; set; }

        [Column("level")]
        public string? Level { get; set; }

        [Column("objects")]
        public string? Objects { get; set; }

        [Column("alias")]
        public string? Alias { get; set; }

        [Column("values")]
        public string? Values { get; set; }

        [Column("file_name")]
        public string? FileName { get; set; }
    }

    public class StazioneSaldanteLogService
    {
        public static Task ImportAll()
        {
            return Task.Run(async () =>
            {
                var logDir = Configuration.Instance.StazioneSaldanteLogDir;

                var conn = new DatabaseContext();
                var provider = CultureInfo.InvariantCulture;
                var today = DateTime.Today.Day;
                var yearDirs = Directory.GetDirectories(logDir);
                foreach (var yearDir in yearDirs)
                {
                    if (!int.TryParse(Path.GetFileName(yearDir), out int _y)) continue;

                    var logBkDir = Path.Combine(yearDir, "Backup");
                    if (!Directory.Exists(logBkDir)) Directory.CreateDirectory(logBkDir);

                    var monthDirs = Directory.GetDirectories(yearDir);
                    foreach (var monthDir in monthDirs) 
                    {
                        if (!int.TryParse(Path.GetFileName(monthDir), out int _m)) continue;

                        var files = Directory.GetFiles(monthDir, "*.csv");
                        foreach (var file in files)
                        {
                            using (var sr = new StreamReader(file))
                            {
                                var idxRow = 0;
                                while (!sr.EndOfStream)
                                {
                                    idxRow++;

                                    var line = sr.ReadLine();

                                    if (idxRow == 1) continue;

                                    try
                                    {
                                        var lineSplit = line.Split(";");
                                        if (lineSplit.Length < 7) continue;
                                        var entry = new StazioneSaldanteLogEntry
                                        {
                                            Id = Guid.NewGuid().ToString(),
                                            DateTime = DateTime.ParseExact(lineSplit[0], "yyyy-MM-dd HH:mm:ss.fff", provider),
                                            OtDevice = lineSplit[1],
                                            Protocol = lineSplit[2],
                                            Level = lineSplit[3],
                                            Objects = lineSplit[4],
                                            Alias = lineSplit[5],
                                            Values = lineSplit[6],
                                            FileName = Path.GetFileName(file)
                                        };
                                        conn.Add(entry);
                                    }
                                    catch
                                    {

                                    }
                                }
                            }

                            conn.SaveChanges();

                            try
                            {
                                var bkFileName = Path.GetFileNameWithoutExtension(file) + "_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                                bkFileName = Path.ChangeExtension(bkFileName, Path.GetExtension(file));
                                File.Move(file, Path.Combine(logBkDir, bkFileName));
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                }
            });
        }

        public static Task<List<GraficoDurataProduzioneGiornalieraStazioneSaldante>> LoadTotaleProduzioneGiornaliera(DateTime dateFrom, DateTime dateTo)
        {
            return Task.Run(async () =>
            {
                var provider = CultureInfo.InvariantCulture;

                var conn = new DatabaseContext();
                var eventiNelPeriodo = await conn.StazioneSaldanteLog.Where(l =>
                    l.DateTime >= dateFrom
                    && l.DateTime <= dateTo
                    && l.Alias.ToUpper().Contains("W_CONTATORE_GENERALE_TRALICCI")
                ).ToListAsync();

                var eventiGiornalieri = eventiNelPeriodo.GroupBy(l => new DateTime(l.DateTime.Value.Year, l.DateTime.Value.Month, l.DateTime.Value.Day));

                var result = new List<GraficoDurataProduzioneGiornalieraStazioneSaldante>();
                foreach (var entry in eventiGiornalieri)
                {
                    result.Add(new GraficoDurataProduzioneGiornalieraStazioneSaldante
                    {
                        Data = entry.Key,
                        TotaleTralicci = entry.Where(e => !string.IsNullOrEmpty(e.Values) && int.TryParse(e.Values, out int val)).Sum(e => int.Parse(e.Values))
                    });
                }

                result.Sort((a, b) => a.Data.CompareTo(b.Data));

                return result;
            });
        }
    }
}
