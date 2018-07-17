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
        public string AOLLengthOrder { get; set; }

        [Required]
        [StringLength(500)]
        public string NAOLLengthOrder { get; set; }

        public int AOLResidual { get; set; }

        public int NAOLResidual { get; set; }

        public virtual DataCombine DataCombine { get; set; }
    }
}
