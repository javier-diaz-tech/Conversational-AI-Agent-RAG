using ApiAgenteConversacional.AgenteConversacional.Entities;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;

namespace ApiAgenteConversacional.AgenteConversacional.Aplicacion
{
    public interface IAgenteAplicacion
    {
        Task<IReadOnlyList<MensajeChatModel>> GenerarRespuestaAgente(string sessionId, string mensajeUsuario);
        Task<IReadOnlyList<MensajeChatModel>> GenerarRespuestaRagAgente(string sessionId, string mensajeUsuario);
        Task<DocumentoActual> IngestDocumentosParaRAG_Async(string texto, string nombreDocumento);
        List<DocumentoActual> ObtenerDocumentosRag();

    }
}
