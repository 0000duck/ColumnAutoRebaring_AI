namespace Test_CAR
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class BeamElevationDesign
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BeamElevationDesign()
        {
            DataCombines = new HashSet<DataCombine>();
        }

        public int ID { get; set; }

        public DateTime CreateDate { get; set; }

        [Required]
        [StringLength(100)]
        public string ElevationOrder { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DataCombine> DataCombines { get; set; }
    }
}
