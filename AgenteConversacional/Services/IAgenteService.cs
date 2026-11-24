using ApiAgenteConversacional.AgenteConversacional.Entities;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace ApiAgenteConversacional.AgenteConversacional.Services
{
    public interface IAgenteService
    {
        Task<string> ObtenerRespuestaLLMAsync(IEnumerable<MensajeChatModel> mensajes);

        Task<DocumentoActual> GuardarDocumentoFragmentoAsync(string textoFragmento, string nombreDocumento);

        Task<IAsyncEnumerable<VectorSearchResult<DocumentoRagModel>>> BuscarEnCollection(string mensajeUsuario);


    }
}
