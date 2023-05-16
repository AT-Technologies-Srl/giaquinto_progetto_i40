using GG40.Pages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GG40.Data
{
    [Table("programmi_di_taglio")]
    public class ProgrammaTaglio
    {
        [Column("id")]
        public string Id { get; set; }

        [Column("data_inserimento")]
	    public DateTime DataInserimento { get; set; }

        [Column("data_avvio")]
	    public DateTime? DataAvvio { get; set; }

        [Column("nome_programma")]
        public string NomeProgramma { get; set; }

        [Column("descrizione")]
        public string? Descrizione { get; set; }

        [Column("file_allegato")]
        public string? FileAllegato { get; set; }	
    }

    public class ProgrammaTaglioService
    {
        public static Task<List<ProgrammaTaglio>> FindAllAsync()
        {
            return Task.Run(async () =>
            {
                var conn = new DatabaseContext();
                return await conn.ProgrammiTaglio.OrderByDescending(t => t.DataInserimento).ToListAsync();
            });            
        }
    }
}
