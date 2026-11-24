using ApiAgenteConversacional.AgenteConversacional.Entities;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace ApiAgenteConversacional.AgenteConversacional.Services
{
    internal class ServicioAgente: IAgenteService
    {
        private readonly Kernel _kernel;
        private readonly VectorStore _vectorStore;
        private IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator;
        VectorStoreCollection<Guid, DocumentoRagModel> collection;
        public ServicioAgente(Kernel kernel,VectorStore vectorStore)
        {
            _kernel = kernel;
            _vectorStore = vectorStore;
            _embeddingGenerator = _kernel.Services.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();

        }
      
        public async Task<string> ObtenerRespuestaLLMAsync(IEnumerable<MensajeChatModel> mensajes)
        {
            var chatService = _kernel.GetRequiredService<IChatCompletionService>();
            var historialAdaptado = MapearAChatHistory(mensajes);
            var respuesta = await chatService.GetChatMessageContentAsync(historialAdaptado, kernel: _kernel);            
            return respuesta.Content ?? string.Empty;
        }

        public async Task<IAsyncEnumerable<VectorSearchResult<DocumentoRagModel>>> BuscarEnCollection(string mensajeUsuario)
        {
            //Generar el embedding
            var queryEmbedding = await _embeddingGenerator.GenerateAsync(mensajeUsuario);

            //Consultar en el RAG
            collection = _vectorStore.GetCollection<Guid, DocumentoRagModel>("DocumentosRAG");
            var results = collection.SearchAsync(queryEmbedding.Vector, top: 3);

            return results;
        }


        
        private static ChatHistory MapearAChatHistory(IEnumerable<MensajeChatModel> mensajes)
        {
            var chatHistory = new ChatHistory();

            foreach (var msg in mensajes)
            {
                switch (msg.Rol)
                {
                    case RolChat.Sistema:
                        chatHistory.AddSystemMessage(msg.Contenido);
                        break;
                    case RolChat.Usuario:
                        chatHistory.AddUserMessage(msg.Contenido);
                        break;
                    case RolChat.Asistente:
                        chatHistory.AddAssistantMessage(msg.Contenido);
                        break;
                }
            }

            return chatHistory;
        }

        public async Task<DocumentoActual> GuardarDocumentoFragmentoAsync(string textoFragmento,string nombreDocumento)
        {
            collection = _vectorStore.GetCollection<Guid, DocumentoRagModel>("DocumentosRAG");
            await collection.EnsureCollectionExistsAsync();
            var embedding = await _embeddingGenerator.GenerateAsync(textoFragmento);
            var chunks = DividirDocumento(textoFragmento, 500);
            foreach (var chunk in chunks)
            {
                embedding =
                    await _embeddingGenerator.GenerateAsync(chunk);

                await collection.UpsertAsync(
                    new DocumentoRagModel
                    {
                        Id = Guid.NewGuid(),
                        Contenido = chunk,
                        Embedding = embedding.Vector
                    });
            }          
            return new DocumentoActual
            {
                NombreArchivo = nombreDocumento,
                Fragmentos = chunks.Count
            };
        }

        private List<string> DividirDocumento(string text, int size)
        {
            var result = new List<string>();

            for (int i = 0; i < text.Length; i += size)
            {
                int length = Math.Min(size, text.Length - i);

                result.Add(text.Substring(i, length));
            }

            return result;
        }
      
    }
}
