﻿using minimal_api.Dominio.Enus;

namespace minimal_api.Dominio.DTOs
{
    public class AdministradorDTO
    {
        public string Email { get; set; } = default!;
        public string Senha { get; set; } = default!;

        public Perfil? Perfil{ get; set; } = default!;
    }
}
