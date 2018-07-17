namespace RevitAddin1
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ViewInformation
    {
        public int ID { get; set; }

        public DateTime CreateDate { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public int FromDate { get; set; }

        public int ToDate { get; set; }
    }
}
