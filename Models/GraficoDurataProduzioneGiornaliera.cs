using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GG40.Models
{
    public class GraficoDurataProduzioneGiornaliera
    {
        public DateTime Data { get; set; }
        public double Durata { get; set; }
        public string TimeSpanDurata
        {
            get => string.Format("{0:hh\\:mm\\:ss}", TimeSpan.FromMinutes(Durata));
        }
    }
}
