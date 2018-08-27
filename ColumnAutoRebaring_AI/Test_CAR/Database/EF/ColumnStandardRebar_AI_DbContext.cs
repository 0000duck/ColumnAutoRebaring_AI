namespace Test_CAR
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class ColumnStandardRebar_AI_DbContext : DbContext
    {
        public ColumnStandardRebar_AI_DbContext()
            : base(ConstantValue.ConnectionString)
        {
        }

        public virtual DbSet<BeamElevationDesign> BeamElevationDesigns { get; set; }
        public virtual DbSet<BeamElevation> BeamElevations { get; set; }
        public virtual DbSet<DataCombine> DataCombines { get; set; }
        public virtual DbSet<DataLearning> DataLearnings { get; set; }
        public virtual DbSet<DevelopmentLength> DevelopmentLengths { get; set; }
        public virtual DbSet<ElevationDesign> ElevationDesigns { get; set; }
        public virtual DbSet<ElevationOffset> ElevationOffsets { get; set; }
        public virtual DbSet<Elevation> Elevations { get; set; }
        public virtual DbSet<FullStandardLengthOrder> FullStandardLengthOrders { get; set; }
        public virtual DbSet<LengthInformation> LengthInformations { get; set; }
        public virtual DbSet<Length> Lengths { get; set; }
        public virtual DbSet<Offset> Offsets { get; set; }
        public virtual DbSet<RebarDesign> RebarDesigns { get; set; }
        public virtual DbSet<RebarDiameter> RebarDiameters { get; set; }
        public virtual DbSet<StandardLengthOrder> StandardLengthOrders { get; set; }
        public virtual DbSet<StandardLength> StandardLengths { get; set; }
        public virtual DbSet<StandardLengthType> StandardLengthTypes { get; set; }
        public virtual DbSet<StartOffset> StartOffsets { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BeamElevationDesign>()
                .Property(e => e.ElevationOrder)
                .IsUnicode(false);

            modelBuilder.Entity<BeamElevationDesign>()
                .HasMany(e => e.DataCombines)
                .WithRequired(e => e.BeamElevationDesign)
                .HasForeignKey(e => e.IDBeamElevationDesign)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DataCombine>()
                .HasMany(e => e.DataLearnings)
                .WithRequired(e => e.DataCombine)
                .HasForeignKey(e => e.IDDataCombine)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DataLearning>()
                .Property(e => e.AOLLengthOrder)
                .IsUnicode(false);

            modelBuilder.Entity<DataLearning>()
                .Property(e => e.NAOLLengthOrder)
                .IsUnicode(false);

            modelBuilder.Entity<DevelopmentLength>()
                .HasMany(e => e.DataCombines)
                .WithRequired(e => e.DevelopmentLength)
                .HasForeignKey(e => e.IDDevelopmentLength)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ElevationDesign>()
                .Property(e => e.ElevationOrder)
                .IsUnicode(false);

            modelBuilder.Entity<ElevationDesign>()
                .Property(e => e.ShortenOrder)
                .IsUnicode(false);

            modelBuilder.Entity<ElevationDesign>()
                .HasMany(e => e.DataCombines)
                .WithRequired(e => e.ElevationDesign)
                .HasForeignKey(e => e.IDElevationDesign)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ElevationOffset>()
                .HasMany(e => e.DataCombines)
                .WithRequired(e => e.ElevationOffset)
                .HasForeignKey(e => e.IDElevationOffset)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FullStandardLengthOrder>()
                .Property(e => e.FullStandardLengthOrder1)
                .IsUnicode(false);

            modelBuilder.Entity<FullStandardLengthOrder>()
                .HasMany(e => e.DataCombines)
                .WithRequired(e => e.FullStandardLengthOrder)
                .HasForeignKey(e => e.IDFullStandardLengthOrder)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<LengthInformation>()
                .HasMany(e => e.DataCombines)
                .WithRequired(e => e.LengthInformation)
                .HasForeignKey(e => e.IDLengthInformation)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Length>()
                .HasMany(e => e.StandardLengths)
                .WithRequired(e => e.Length)
                .HasForeignKey(e => e.IDLength)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Offset>()
                .HasMany(e => e.ElevationOffsets)
                .WithRequired(e => e.Offset)
                .HasForeignKey(e => e.IDBottomOffset)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Offset>()
                .HasMany(e => e.ElevationOffsets1)
                .WithRequired(e => e.Offset1)
                .HasForeignKey(e => e.IDTopOffset)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RebarDesign>()
                .Property(e => e.DiameterOrder)
                .IsUnicode(false);

            modelBuilder.Entity<RebarDesign>()
                .Property(e => e.LocationOrder)
                .IsUnicode(false);

            modelBuilder.Entity<RebarDesign>()
                .HasMany(e => e.DataCombines)
                .WithRequired(e => e.RebarDesign)
                .HasForeignKey(e => e.IDRebarDesign)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RebarDiameter>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<StandardLengthOrder>()
                .Property(e => e.StandardLengthOrder1)
                .IsUnicode(false);

            modelBuilder.Entity<StandardLengthType>()
                .Property(e => e.Value)
                .IsUnicode(false);

            modelBuilder.Entity<StandardLengthType>()
                .HasMany(e => e.StandardLengthOrders)
                .WithRequired(e => e.StandardLengthType)
                .HasForeignKey(e => e.IDType)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<StartOffset>()
                .HasMany(e => e.DataCombines)
                .WithRequired(e => e.StartOffset)
                .HasForeignKey(e => e.IDStartOffset)
                .WillCascadeOnDelete(false);
        }
    }
}
