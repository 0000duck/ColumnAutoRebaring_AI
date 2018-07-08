namespace TestRevit1.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class StandardLengthType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public StandardLengthType()
        {
            StandardLengthOrders = new HashSet<StandardLengthOrder>();
        }

        public int ID { get; set; }

        public DateTime CreateDate { get; set; }

        [Required]
        [StringLength(10)]
        public string Value { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StandardLengthOrder> StandardLengthOrders { get; set; }
    }
}
