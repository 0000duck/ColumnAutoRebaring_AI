namespace Test_CAR
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class RebarDesign
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RebarDesign()
        {
            DataCombines = new HashSet<DataCombine>();
        }

        public int ID { get; set; }

        public DateTime CreateDate { get; set; }

        [Required]
        [StringLength(100)]
        public string DiameterOrder { get; set; }

        [Required]
        [StringLength(100)]
        public string LocationOrder { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DataCombine> DataCombines { get; set; }
    }
}
