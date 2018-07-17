namespace RevitAddin1
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class RenderDbContext : DbContext
    {
        public RenderDbContext()
            : base(ConstantValue.ConnectionString)
        {
        }

        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<Timeline> Timelines { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<Project>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Project>()
                .HasMany(e => e.Timelines)
                .WithRequired(e => e.Project)
                .HasForeignKey(e => e.IDProject)
                .WillCascadeOnDelete(false);
        }
    }
}
