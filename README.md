# Conversational-AI-Agent-RAG

Agente conversacional desarrollado en ASP.NET Core utilizando Semantic Kernel y Ollama.

Permite mantener conversaciones con modelos LLM locales e incorporar capacidades RAG (Retrieval Augmented Generation) mediante la carga de documentos PDF para responder preguntas basadas en información documental.

## Características

✅ Chat conversacional con memoria por sesión

✅ Integración con Ollama

✅ Semantic Kernel

✅ RAG (Retrieval Augmented Generation)

✅ Embeddings locales con nomic-embed-text

✅ Carga de documentos PDF

✅ Búsqueda semántica

✅ Guardrails básicos para evitar respuestas fuera del contexto documental

✅ Interfaz web desarrollada con Razor Pages

## Tecnologías

- ASP.NET Core
- Razor Pages
- Semantic Kernel
- Ollama
- Gemma 2B / Llama 3.1
- Nomic Embed Text
- InMemory Vector Store
- Memory Cache

## Arquitectura

```text
AgenteConversacional
│
├── Aplicacion
│   ├── AgenteAplicacion
│
├── Controller
│
├── Entities
│   ├── MensajeChatModel
│   ├── DocumentoActual
│   └── DocumentoRagModel
│
├── Pages
│   └── Chat
│
└── Services
    ├── AgenteService
    └── IAgenteService
```

## Funcionamiento

### Conversación tradicional

```text
Usuario
    ↓
Semantic Kernel
    ↓
Ollama
    ↓
Respuesta
```

### Conversación con RAG

```text
Usuario
    ↓
Generación de Embedding
    ↓
Búsqueda Vectorial
    ↓
Recuperación de contexto
    ↓
Prompt enriquecido
    ↓
Ollama
    ↓
Respuesta basada en documentos
```

## Configuración

appsettings.json

```json
{
  "NombreAgente": "gemma:2b",
  "UrlAgente": "http://localhost:11434",
  "EmbeddingGenerador": "nomic-embed-text"
}
```

## Requisitos

### Instalar Ollama

https://ollama.com

### Descargar modelos

```bash
ollama pull gemma:2b
ollama pull llama3.1
ollama pull nomic-embed-text
```

### Ejecutar Ollama

```bash
ollama serve
```

### Ejecutar la aplicación

```bash
dotnet run
```

## Video demostrativo

https://youtu.be/c_vG2MsYkoI

## Próximas mejoras

- PostgreSQL + pgvector
- Persistencia de documentos
- Persistencia de conversaciones
- Multiagentes
- Churn Agent
- Fraud Agent
- Agent Orchestrator
- Métricas y observabilidad

## Autor

Javier Díaz

Software Developer | .NET | IA Generativa | Semantic Kernel | AWS

## Licencia
 
Todos los derechos reservados.
 
Este repositorio se publica con fines educativos y de demostración profesional. El uso, distribución o comercialización de su contenido requiere autorización expresa del autor.
