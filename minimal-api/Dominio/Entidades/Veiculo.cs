using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace minimal_api.Dominio.Entidades
{
    public class Veiculo
    {
        #region Propriedades

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Nome { get; set; } = default!;
        [Required]
        [MaxLength(100)]
        public string Marca { get; set; } = default!;
        [Required]
        [MaxLength(10)]
        public int Ano { get; set; } = default!;

        #endregion
    }
}
