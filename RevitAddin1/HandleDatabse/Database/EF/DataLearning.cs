namespace HandleDatabse.Database.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class DataLearning
    {
        public int ID { get; set; }

        public DateTime CreateDate { get; set; }

        public int IDDataCombine { get; set; }

        [Required]
        [StringLength(500)]
        public string LengthOrder { get; set; }

        public int? Residual { get; set; }

        public virtual DataCombine DataCombine { get; set; }
    }
}
