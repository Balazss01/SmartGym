using GymWebApiBackend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace GymWebApiBackend.Data
{

    public class ApplicationDbContext
        : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Tag> Tagok { get; set; }

        public DbSet<Berlet> Berletek { get; set; }
        public DbSet<BerletTipus> BerletTipusok { get; set; }
        public DbSet<Belepes> Belepesek { get; set; }
        public DbSet<Szekreny> Szekrenyek { get; set; }
        public DbSet<SzekrenyFoglalas> SzekrenyFoglalasok { get; set; }
        public DbSet<Ertesites> Ertesitesek { get; set; }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.ToTable("tagok");
                entity.HasKey(e => e.TagId);

                entity.Property(e => e.TagId).HasColumnName("tag_id");
                entity.Property(e => e.IdentityUserId).HasColumnName("identity_user_id");
                entity.Property(e => e.Vezeteknev).HasColumnName("vezeteknev").HasMaxLength(50);
                entity.Property(e => e.Keresztnev).HasColumnName("keresztnev").HasMaxLength(50);
                entity.Property(e => e.SzuletesiDatum).HasColumnName("szuletesi_datum");
                entity.Property(e => e.Aktiv).HasColumnName("aktiv");
            });

            modelBuilder.Entity<BerletTipus>(entity =>
            {
                entity.ToTable("berlettipusok");
                entity.HasKey(e => e.BerletTipusId);

                entity.Property(e => e.BerletTipusId).HasColumnName("berlet_tipus_id");
                entity.Property(e => e.Megnevezes).HasColumnName("megnevezes").HasMaxLength(50);
                entity.Property(e => e.IdotartamNapok).HasColumnName("idotartam_napok");
                entity.Property(e => e.Ar).HasColumnName("ar").HasColumnType("decimal(10,2)");
            });

            modelBuilder.Entity<Berlet>(entity =>
            {
                entity.ToTable("berletek");
                entity.HasKey(e => e.BerletId);

                entity.Property(e => e.BerletId).HasColumnName("berlet_id");
                entity.Property(e => e.TagId).HasColumnName("tag_id");
                entity.Property(e => e.BerletTipusId).HasColumnName("berlet_tipus_id");
                entity.Property(e => e.KezdetDatum).HasColumnName("kezdet_datum");
                entity.Property(e => e.VegeDatum).HasColumnName("vege_datum");
                entity.Property(e => e.Aktiv).HasColumnName("aktiv");

                entity.HasOne(d => d.Tag)
                    .WithMany(p => p.Berletek)
                    .HasForeignKey(d => d.TagId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.BerletTipus)
                    .WithMany(p => p.Berletek)
                    .HasForeignKey(d => d.BerletTipusId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Belepes>(entity =>
            {
                entity.ToTable("belepesek");
                entity.HasKey(e => e.BelepesId);

                entity.Property(e => e.BelepesId).HasColumnName("belepes_id");
                entity.Property(e => e.TagId).HasColumnName("tag_id");
                entity.Property(e => e.BelepesIdopont).HasColumnName("belepes_idopont");

                entity.HasOne(d => d.Tag)
                    .WithMany(p => p.Belepesek)
                    .HasForeignKey(d => d.TagId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Szekreny>(entity =>
            {
                entity.ToTable("szekrenyek");
                entity.HasKey(e => e.SzekrenyId);

                entity.Property(e => e.SzekrenyId).HasColumnName("szekreny_id");
                entity.Property(e => e.SzekrenySzam).HasColumnName("szekreny_szam");
                entity.Property(e => e.Aktiv).HasColumnName("aktiv");
            });

            modelBuilder.Entity<SzekrenyFoglalas>(entity =>
            {
                entity.ToTable("szekreny_foglalasok");
                entity.HasKey(e => e.FoglalasId);

                entity.Property(e => e.FoglalasId).HasColumnName("foglalas_id");
                entity.Property(e => e.TagId).HasColumnName("tag_id");
                entity.Property(e => e.SzekrenyId).HasColumnName("szekreny_id");
                entity.Property(e => e.Zarva).HasColumnName("zarva");
                entity.Property(e => e.FoglalvaKezdete).HasColumnName("foglalva_kezdete");
                entity.Property(e => e.FoglalvaVege).HasColumnName("foglalva_vege");

                entity.HasOne(d => d.Tag)
                    .WithMany(p => p.SzekrenyFoglalasok)
                    .HasForeignKey(d => d.TagId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Szekreny)
                    .WithMany(p => p.SzekrenyFoglalasok)
                    .HasForeignKey(d => d.SzekrenyId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Ertesites>(entity =>
            {
                entity.ToTable("ertesitesek");
                entity.HasKey(e => e.ErtesitesId);

                entity.Property(e => e.ErtesitesId).HasColumnName("ertesites_id");
                entity.Property(e => e.TagId).HasColumnName("tag_id");
                entity.Property(e => e.Uzenet).HasColumnName("uzenet");
                entity.Property(e => e.Olvasott).HasColumnName("olvasott");
                entity.Property(e => e.Datum).HasColumnName("datum");
            });
        }
    }
}