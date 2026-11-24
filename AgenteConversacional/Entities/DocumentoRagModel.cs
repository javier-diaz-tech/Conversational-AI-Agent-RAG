using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Data;
namespace ApiAgenteConversacional.AgenteConversacional.Entities
{
    public class DocumentoRagModel
    {
        [VectorStoreKey]
        public Guid Id { get; set; }

        [VectorStoreData]
        public string Contenido { get; set; }

        // Aquí se guardará el vector generado matemáticamente por OpenAI/Azure
        [VectorStoreVector(1536)] // 1536 es el tamaño estándar de text-embedding-3-small
        public ReadOnlyMemory<float> Embedding { get; set; }
    }
}
