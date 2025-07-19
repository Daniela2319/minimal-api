using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace minimal_api.Dominio.Entidades
{
    public class Administrador
    {
        #region Propriedades

            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int Id { get; set; }

            [Required]
            [MaxLength(255)]
            public string Email { get; set; } = default!;
            [Required]
            [MaxLength(55)]
            public string Senha { get; set; } = default!;
            [Required]
            [MaxLength(10)]
            public string Perfil { get; set; } = default!;

        #endregion

    }
}
