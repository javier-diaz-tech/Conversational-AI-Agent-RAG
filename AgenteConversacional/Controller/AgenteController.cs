using ApiAgenteConversacional.AgenteConversacional.Aplicacion;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace ApiAgenteConversacional.AgenteConversacional.Controller
{
    public class AgenteController: ControllerBase
    {
        IAgenteAplicacion _agenteAplicacion;

        public AgenteController(IAgenteAplicacion agenteAplicacion)
        {
            _agenteAplicacion = agenteAplicacion;
        }

        [HttpPost("GenerarRespuestaAgente")]
        public async Task<IActionResult> GenerarRespuestaAgente(string sessionId,[FromBody] string mensajeUsuario)
        {
            var respuestas = await _agenteAplicacion.GenerarRespuestaAgente(sessionId,mensajeUsuario);
            return Ok(respuestas);
        }

        [HttpPost("GenerarRespuestaRagAgente")]
        public async Task<IActionResult> GenerarRespuestaRagAgente(string sessionId,[FromBody] string mensajeUsuario)
        {
            var respuestas = await _agenteAplicacion.GenerarRespuestaRagAgente(sessionId,mensajeUsuario);
            return Ok(respuestas);
        }

        [HttpPost("IngestDocumentosParaRAG_Async")]
        public async Task<IActionResult> IngestDocumentosParaRAG_Async([FromBody] string texto, [FromBody] string nombreDocumento)
        {
            await _agenteAplicacion.IngestDocumentosParaRAG_Async(texto, nombreDocumento);
            return Ok();
        }
    }
}
