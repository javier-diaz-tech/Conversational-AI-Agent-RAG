namespace ApiAgenteConversacional.AgenteConversacional.Entities
{
    public enum RolChat
    {
        Sistema,
        Usuario,
        Asistente
    }
    public class MensajeChatModel
    {
        public RolChat Rol { get; set; }
        public string Contenido { get; set; } = string.Empty;
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
    }
}
