namespace TestRevit1.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class RebarDiameter
    {
        public int ID { get; set; }

        public DateTime CreateDate { get; set; }

        [Required]
        [StringLength(10)]
        public string Name { get; set; }

        public int Diameter { get; set; }
    }
}
