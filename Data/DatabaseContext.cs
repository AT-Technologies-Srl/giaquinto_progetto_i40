using GG40.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GG40.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext() : base()
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            var connectionString = $"Server={Configuration.Instance.DataSourceServerAddress},{Configuration.Instance.DataSourceServerPort};Database=gg40;User Id={Configuration.Instance.DataSourceUser};Password={Configuration.Instance.DataSourcePassword};Trust Server Certificate=true;";
            optionsBuilder.UseSqlServer(connectionString);
        }

        public DbSet<ProgrammaTaglio> ProgrammiTaglio { get; set; }
        public DbSet<PlasmaRedLogEntry> PlasmaRedLog { get; set; }
        public DbSet<StazioneSaldanteLogEntry> StazioneSaldanteLog { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<ProgrammaTaglio>().ToTable("programmi_di_taglio");
            //modelBuilder.Entity<EvtUser>().ToTable("evt_user");
        }
    }
}
