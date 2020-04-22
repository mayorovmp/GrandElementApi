using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GrandElementApi.Data
{
    public enum RowStatus
    {
        Active,
        Removed
    }
    public class BaseEntity
    {
        [Column("created")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? Created { get; set; }

        [Column("updated")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? Updated { get; set; }

        [Column("row_status")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public RowStatus? RowStatus { get; set; }
    }
}
