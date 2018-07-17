namespace TestRevit1.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class DataCombine
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DataCombine()
        {
            DataLearnings = new HashSet<DataLearning>();
        }

        public int ID { get; set; }

        public DateTime CreateDate { get; set; }

        public int IDRebarDesign { get; set; }

        public int IDElevationDesign { get; set; }

        public int IDBeamElevationDesign { get; set; }

        public int IDElevationOffset { get; set; }

        public int IDStartOffset { get; set; }

        public int IDDevelopmentLength { get; set; }

        public int IDLengthInformation { get; set; }

        public int IDFullStandardLengthOrder { get; set; }

        public virtual BeamElevationDesign BeamElevationDesign { get; set; }

        public virtual DevelopmentLength DevelopmentLength { get; set; }

        public virtual ElevationDesign ElevationDesign { get; set; }

        public virtual ElevationOffset ElevationOffset { get; set; }

        public virtual FullStandardLengthOrder FullStandardLengthOrder { get; set; }

        public virtual LengthInformation LengthInformation { get; set; }

        public virtual RebarDesign RebarDesign { get; set; }

        public virtual StartOffset StartOffset { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DataLearning> DataLearnings { get; set; }
    }
}
