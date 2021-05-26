using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace saga_app.Models
{
    public class TableModel
    {

        [Key]
        public int Id { get; set; }

        [Column(TypeName = "varchar(50)")]
        [DisplayName("Field 1")]
        [Required(ErrorMessage = "This Field is required.")]
        public string Field1 { get; set; }

        [Column(TypeName = "varchar(50)")]
        [DisplayName("Field 2")]
        [Required(ErrorMessage = "This Field is required.")]
        public string Field2 { get; set; }

        [Column(TypeName = "varchar(50)")]
        [DisplayName("Field 3")]
        [Required(ErrorMessage = "This Field is required.")]
        public string Field3 { get; set; }

    }
}
