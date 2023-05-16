using GG40.Data;
using GG40.Models;
using GG40.Support;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace GG40.Data
{
    [Table("log_plasma_red")]
    public class PlasmaRedLogEntry
    {
        [Column("id")]
        public string Id { get; set; }

        [Column("data")]
        public DateTime Data { get; set; }

        [Column("ora")]
        public string Ora { get; set; }

        [Column("comando")]
        public string Comando { get; set; }

        [Column("dati_extra")]
        public string? DatiExtra { get; set; }

        [Column("file_name")]
        public string FileName { get; set; }
    }

    public class PlasmaRedLogService
    {
        public static Task ImportAll()
        {
            return Task.Run(async () =>
            {
                var conn = new DatabaseContext();
                var provider = CultureInfo.InvariantCulture;
                var today = DateTime.Today.Day;

                if (await Network.PingAddress(Configuration.Instance.PlasmaRedIndirizzoIp))
                {
                    var logDir = Configuration.Instance.PlasmaRedLogDir;
                    var logBkDir = Path.Combine(logDir, "Backup");
                    if (!Directory.Exists(logBkDir)) Directory.CreateDirectory(logBkDir);

                    var files = Directory.GetFiles(Configuration.Instance.PlasmaRedLogDir, "*.txt");
                    foreach (var file in files)
                    {
                        if (file.EndsWith($"{today}.txt")) continue;

                        using (var sr = new StreamReader(file))
                        {
                            while (!sr.EndOfStream)
                            {
                                var line = sr.ReadLine();
                                var lineSplit = line.Split(";");
                                if (lineSplit.Length < 3) continue;
                                var entry = new PlasmaRedLogEntry
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    Data = DateTime.ParseExact(lineSplit[0], "yyyy-MM-dd", provider),
                                    Ora = lineSplit[1],
                                    Comando = lineSplit[2],
                                    FileName = Path.GetFileName(file)
                                };

                                if (lineSplit.Length > 3)
                                {
                                    entry.DatiExtra = lineSplit[3];
                                }
                                conn.Add(entry);
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

                try
                {
                    var listaProgrammiTaglio = await conn.ProgrammiTaglio.Where(pt => !pt.DataAvvio.HasValue).ToListAsync();
                    foreach (var programmaTaglio in listaProgrammiTaglio)
                    {
                        var load = await conn.PlasmaRedLog.Where(l => l.Comando != null && l.Comando.Trim().ToLower() == "load" &&
                        l.DatiExtra != null &&
                        l.DatiExtra.ToLower().EndsWith(programmaTaglio.NomeProgramma.ToLower()))
                        .OrderByDescending(l => l.Data)
                        .FirstOrDefaultAsync();

                        if (load == null) continue;

                        programmaTaglio.DataAvvio = DateTime.ParseExact(load.Data.ToString("yyyy-MM-dd") + " " + load.Ora, "yyyy-MM-dd HH:mm:ss", provider);
                        conn.Update(programmaTaglio);
                    }

                    await conn.SaveChangesAsync();
                }
                catch (Exception ex)
                {

                }
            });            
        }

        public static Task<List<GraficoDurataProduzioneGiornaliera>> LoadDurataProduzioneGiornaliera(DateTime dateFrom, DateTime dateTo)
        {
            return Task.Run(async () =>
            {
                var provider = CultureInfo.InvariantCulture;

                var conn = new DatabaseContext();
                var eventiNelPeriodo = await conn.PlasmaRedLog.Where(l => 
                    l.Data >= dateFrom
                    && l.Data <= dateTo
                    && (l.Comando.ToUpper().Contains("START")
                    || l.Comando.ToUpper().Equals("STOP")
                    || l.Comando.ToUpper().Equals("END")
                    || l.Comando.ToUpper().Equals("ABORT"))
                ).ToListAsync();

                var eventiGiornalieri = eventiNelPeriodo.GroupBy(l => l.Data);

                var entries = new List<GraficoDurataProduzioneGiornaliera>();
                foreach (var gruppoGiornaliero in eventiGiornalieri)
                {
                    var eventi = gruppoGiornaliero.OrderBy(e => DateTime.ParseExact($"{e.Data.ToString("yyyy-MM-dd")} {e.Ora}", "yyyy-MM-dd HH:mm:ss", provider)).ToList();
                    for (var idxStart = 0; idxStart < eventi.Count; idxStart++)
                    {
                        if (eventi[idxStart].Comando.ToUpper().Contains("START"))
                        {
                            for (var idxEnd = idxStart; idxEnd < eventi.Count; idxEnd++)
                            {
                                if (eventi[idxEnd].Comando.ToUpper().Contains("STOP")
                                || eventi[idxEnd].Comando.ToUpper().Contains("END")
                                || eventi[idxEnd].Comando.ToUpper().Contains("ABORT"))
                                {
                                    var dtStart = DateTime.ParseExact($"{eventi[idxStart].Data.ToString("yyyy-MM-dd")} {eventi[idxStart].Ora}", "yyyy-MM-dd HH:mm:ss", provider);
                                    var dtEnd = DateTime.ParseExact($"{eventi[idxEnd].Data.ToString("yyyy-MM-dd")} {eventi[idxEnd].Ora}", "yyyy-MM-dd HH:mm:ss", provider);
                                    var duration = dtEnd - dtStart;

                                    entries.Add(new GraficoDurataProduzioneGiornaliera
                                    {
                                        Data = gruppoGiornaliero.Key,
                                        Durata = duration.TotalMinutes
                                    });

                                    idxStart = idxEnd;
                                    break;
                                }
                            }
                        }
                    }
                }

                var result = new List<GraficoDurataProduzioneGiornaliera>();
                foreach (var entry in entries.GroupBy(e => e.Data))
                {
                    result.Add(new GraficoDurataProduzioneGiornaliera
                    {
                        Data = entry.Key,
                        Durata = entry.Sum(e => e.Durata)
                    });
                }

                result.Sort((a, b) => a.Data.CompareTo(b.Data));

                return result;
            });
        }
    }
}
