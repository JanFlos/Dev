using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MetadataService.Model;

namespace MetadataService.Model
{
    public class Method
    {

        [Key, Column(Order = 0)]
        public int ObjectId { get; set; }

        [Key, Column(Order = 1)]
        public int SubprogramId { get; set; }

        [Key, Column(Order = 2)]
        [StringLength(40)]
        public string Overload { get; set; }

        [Required]
        [StringLength(30)]
        public string Name { get; set; }

        [StringLength(90)]
        public string Datatype { get; set; }

        [ForeignKey("ObjectId")]
        public Package Package { get; set; }

        public List<Argument> Arguments { get; set; }

        public string FullName {
            get {
                return string.Format("{1}.{0}{2}", Name, Package.Name, Overload == "0" ? "" : "." + Overload); 
            } 
        }
    }
}
