namespace minimal_api.Dominio.ModelViews
{
    public struct Home
    {
        public string Mensagem { get => "Bem vindo a API de Veículos - Minimal API! "; }
        public readonly string Doc => "/scalar/v1";
    }
}
