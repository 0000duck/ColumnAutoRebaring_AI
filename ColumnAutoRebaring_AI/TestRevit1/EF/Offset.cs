namespace TestRevit1.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Offset
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Offset()
        {
            ElevationOffsets = new HashSet<ElevationOffset>();
            ElevationOffsets1 = new HashSet<ElevationOffset>();
        }

        public int ID { get; set; }

        public DateTime CreateDate { get; set; }

        public int OffsetValue { get; set; }

        public int OffsetRatio { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ElevationOffset> ElevationOffsets { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ElevationOffset> ElevationOffsets1 { get; set; }
    }
}
