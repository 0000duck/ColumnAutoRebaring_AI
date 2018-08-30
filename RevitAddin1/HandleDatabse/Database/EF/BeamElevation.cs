namespace HandleDatabse.Database.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class BeamElevation
    {
        public int ID { get; set; }

        public DateTime CreateDate { get; set; }

        public int Value { get; set; }
    }
}
