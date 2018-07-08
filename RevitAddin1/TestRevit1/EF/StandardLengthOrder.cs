namespace TestRevit1.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class StandardLengthOrder
    {
        public int ID { get; set; }

        public DateTime CreateDate { get; set; }

        public int IDType { get; set; }

        [Column("StandardLengthOrder")]
        [Required]
        [StringLength(100)]
        public string StandardLengthOrder1 { get; set; }

        public virtual StandardLengthType StandardLengthType { get; set; }
    }
}
