using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MetadataService.Model
{
     public class Argument
    {

        [Key, Column(Order = 0)]
        public int ObjectId { get; set; }

        [Key, Column(Order = 1)]
        public int SubprogramId { get; set; }

        [Key, Column(Order = 2)]
        public string Overload { get; set; }

        [Key, Column(Order = 3)]
        public int Position { get; set; }


        [StringLength(30)]
        public string Name { get; set; }

        [StringLength(90)]
        public string Datatype { get; set; }


        [StringLength(1)]
        public String Defaulted { get; set; }

        [StringLength(9)]
        public String InOut { get; set; }

        [Required]
        public Method Method { get; set; }

    
    }
}
