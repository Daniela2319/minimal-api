using minimal_api.Dominio.DTOs;
using minimal_api.Dominio.Entidades;

namespace minimal_api.Dominio.Interfaces
{
    public interface IVeiculoServico
    {
        List<Veiculo> ObterTodos(int pagina = 1, string? nome = null, string? marca = null);
        Veiculo? ObterPorId(int id);
        void Adicionar(Veiculo veiculo);
        void Atualizar(Veiculo veiculo);
        void Excluir(Veiculo veiculo);
    }
}
