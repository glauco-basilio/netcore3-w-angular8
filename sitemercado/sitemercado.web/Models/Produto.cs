using Microsoft.AspNetCore.Identity;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace sitemercado.web.Models
{
    [Table("produto")]
    public class Produto 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProdutoID { get; set; }

        [Required]
        public string Nome { get; set; }

        [Required,
        Column(TypeName = "decimal(10, 2)")]
        public Decimal ValorVenda { get; set; }

        [Required]
        public bool PossuiImagem { get; set; } 
    }
}
