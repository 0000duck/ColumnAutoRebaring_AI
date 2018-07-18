namespace RevitAddin1
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Timeline
    {
        public int ID { get; set; }

        public DateTime CreateDate { get; set; }

        public int IDProject { get; set; }

        [Required]
        [StringLength(10)]
        public string Prefix { get; set; }

        public int Date { get; set; }

        public virtual Project Project { get; set; }
    }
}
