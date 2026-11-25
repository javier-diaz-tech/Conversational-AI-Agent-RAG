using ApiAgenteConversacional.AgenteConversacional.Entities;
using ApiAgenteConversacional.AgenteConversacional.Services;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.VectorData;
using System.Text;
using System.Linq;


namespace ApiAgenteConversacional.AgenteConversacional.Aplicacion
{    

    public class AgenteAplicacion: IAgenteAplicacion
    {
        IAgenteService _agenteService;
        IMemoryCache _memoryCache;
        public AgenteAplicacion(IAgenteService agenteService, IMemoryCache memoryCache)
        {
            //Instanciar el agente
            _agenteService = agenteService;
            _memoryCache = memoryCache;
        }
       

        public async Task<IReadOnlyList<MensajeChatModel>> GenerarRespuestaAgente(string sessionId, string mensajeUsuario)
        {
            //Obtener servicio del chat
            var chatHistory = _memoryCache.GetOrCreate($"chatHistory_{sessionId}", entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(30);
                return new List<MensajeChatModel>();
            });

            var mensajes = new List<MensajeChatModel>
            {
                new() {
                       Rol = RolChat.Sistema,
                       Contenido = "Eres un asistente experto en la materia, responde únicamente con base en el contexto proporcionado."
                }
            };

            mensajes.AddRange(chatHistory.TakeLast(10));

            mensajes.Add(new MensajeChatModel
            {
                Rol = RolChat.Usuario,
                Contenido = mensajeUsuario
            });

            //Agregar el mensaje del usuario al historial
            chatHistory?.Add(new MensajeChatModel
            {
                Rol = RolChat.Usuario,
                Contenido = mensajeUsuario
            });
            //Consultar al historial
            string chatResponse = await _agenteService.ObtenerRespuestaLLMAsync(mensajes);
            chatHistory.Add(new MensajeChatModel
            {
                Rol = RolChat.Asistente,
                Contenido = chatResponse
            });

            return chatHistory;
        }

        public async Task<IReadOnlyList<MensajeChatModel>> GenerarRespuestaRagAgente(string sessionId, string mensajeUsuario)
        {           
            var results = await _agenteService.BuscarEnCollection(mensajeUsuario);
            //Obtiene los mejores fragmentos
            StringBuilder context = new();
            double maxScore = 0.0;
            await foreach (var item in results)
            {
                context.AppendLine(item.Record.Contenido);
                if (item.Score > maxScore)
                {
                    maxScore = item.Score.Value;
                }                
            }

            var chatHistory = _memoryCache.GetOrCreate($"chatHistory_{sessionId}", e =>
            {
                e.SlidingExpiration =
                    TimeSpan.FromMinutes(30);

                return new List<MensajeChatModel>();
            });

            if (maxScore < 0.7)
            {
                chatHistory.Add(new MensajeChatModel
                {
                    Rol = RolChat.Asistente,
                    Contenido = "No encontré información sobre esa pregunta en los documentos cargados."
                });
                return chatHistory;               
            }

            //Generamos el prompt
            string prompt = $"""
                    Eres un asistente que responde exclusivamente utilizando información del contexto. 
                    REGLAS:
                    - No uses conocimientos propios.
                    - No inventes información.
                    - Si la información necesaria para responder está presente en el contexto, úsala para generar la respuesta.
                    - Si la información NO está presente en el contexto, responde exactamente:
                     "No encontré información sobre esa pregunta en los documentos cargados."

                    CONTEXTO:
                    {context}
                    PREGUNTA:
                    {mensajeUsuario}
            """;


            //Obtener servicio del chat
         

            var mensajes = new List<MensajeChatModel>
                {
                    new MensajeChatModel
                    {
                        Rol = RolChat.Sistema,
                        Contenido = prompt
                    }
                };


            mensajes.AddRange(chatHistory.TakeLast(10));
            //Agregar el mensaje del usuario al historial
            mensajes?.Add(new MensajeChatModel
            {
                Rol = RolChat.Usuario,
                Contenido = mensajeUsuario
            });

            chatHistory.Add(new MensajeChatModel
            {
                Rol = RolChat.Usuario,
                Contenido = mensajeUsuario
            });
            //Consultar al historial
            var chatResponse = await _agenteService.ObtenerRespuestaLLMAsync(mensajes);
            chatHistory.Add(new MensajeChatModel
            {
                Rol = RolChat.Asistente,
                Contenido = chatResponse
            });
            return chatHistory;
        }

       
        public async Task<DocumentoActual> IngestDocumentosParaRAG_Async(string texto, string nombreDocumento)
        {
            var documentoActual = await _agenteService.GuardarDocumentoFragmentoAsync(texto, nombreDocumento);
            var documentosCargados = _memoryCache.GetOrCreate("DocumentosCargados", e =>
            {
                e.SlidingExpiration = TimeSpan.FromMinutes(30);
                return new List<DocumentoActual>();
            });

            if(!documentosCargados.Any(d => d.NombreArchivo == nombreDocumento))
            {
                documentosCargados.Add(new DocumentoActual { NombreArchivo = nombreDocumento, Fragmentos = documentoActual.Fragmentos });
            }

            return documentoActual;
        }

        public List<DocumentoActual> ObtenerDocumentosRag()
        {
            var documentosActuales = (List<DocumentoActual>)_memoryCache.Get("DocumentosCargados")?? new List<DocumentoActual>();
            return documentosActuales;
        }




    }
}
