namespace TestRevit1.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class StartOffset
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public StartOffset()
        {
            DataCombines = new HashSet<DataCombine>();
        }

        public int ID { get; set; }

        public DateTime CreateDate { get; set; }

        public int Offset1 { get; set; }

        public int Offset2 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DataCombine> DataCombines { get; set; }
    }
}
