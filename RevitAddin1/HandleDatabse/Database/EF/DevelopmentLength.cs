namespace HandleDatabse.Database.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class DevelopmentLength
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DevelopmentLength()
        {
            DataCombines = new HashSet<DataCombine>();
        }

        public int ID { get; set; }

        public DateTime CreateDate { get; set; }

        public int DevelopMultilply { get; set; }

        public int DevelopOffset { get; set; }

        public int ImplantMultilply { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DataCombine> DataCombines { get; set; }
    }
}
