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

        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<ViewInformation> ViewInformations { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ViewInformation>()
                .Property(e => e.Name)
                .IsUnicode(false);
        }
    }
}
