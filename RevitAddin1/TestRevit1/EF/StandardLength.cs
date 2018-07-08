namespace TestRevit1.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class StandardLength
    {
        public int ID { get; set; }

        public DateTime CreateDate { get; set; }

        public int IDLength { get; set; }

        public virtual Length Length { get; set; }
    }
}
