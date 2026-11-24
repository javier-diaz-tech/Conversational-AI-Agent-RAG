using ApiAgenteConversacional.AgenteConversacional.Aplicacion;
using ApiAgenteConversacional.AgenteConversacional.Controller;
using ApiAgenteConversacional.AgenteConversacional.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UglyToad.PdfPig;

namespace ApiAgenteConversacional.AgenteConversacional.Pages.Chat
{
    public class IndexModel : PageModel
    {
        IAgenteAplicacion _agenteAplicacion;

        public IndexModel(IAgenteAplicacion agenteAplicacion)
        {
            _agenteAplicacion = agenteAplicacion;
        }

        [BindProperty]
        public string MensajeUsuario { get; set; }

        public IReadOnlyList<MensajeChatModel> RespuestaAgente { get; set; }

        [BindProperty]
        public bool UsarRAG { get; set; }

        public List<DocumentoActual> DocumentosRag { get; set; }


        public void OnGet()
        {
            RespuestaAgente = [new MensajeChatModel(){ Rol = RolChat.Asistente, Contenido = "¿En qué puedo ayudarte hoy?" }];
            DocumentosRag = _agenteAplicacion.ObtenerDocumentosRag()??new List<DocumentoActual>();
        }

        public async Task<IActionResult> OnPost()
        {
            if(MensajeUsuario == null || MensajeUsuario.Trim() == "")
            {
                ModelState.AddModelError(string.Empty, "El mensaje del usuario no puede estar vacío.");
                return Page();
            }


            const string Key = "ChatSessionId";

            var sessionId = HttpContext.Session.GetString(Key);

            if (string.IsNullOrEmpty(sessionId))
            {
                sessionId = Guid.NewGuid().ToString();

                HttpContext.Session.SetString(
                    Key,
                    sessionId);
            }
            DocumentosRag = _agenteAplicacion.ObtenerDocumentosRag();
            if (UsarRAG)
            {
                if (DocumentosRag.Count == 0)
                {
                    RespuestaAgente = new List<MensajeChatModel>
                    {
                        new MensajeChatModel
                        {
                            Rol = RolChat.Asistente,
                            Contenido = "No se han ingresado documentos para RAG. Por favor, suba un archivo primero."
                        }
                    };                   
                    return Page();
                }
                RespuestaAgente = await _agenteAplicacion.GenerarRespuestaRagAgente(sessionId, MensajeUsuario);                
            }
            else
            {
                RespuestaAgente = await _agenteAplicacion.GenerarRespuestaAgente(sessionId, MensajeUsuario);
            }        
            return Page();
        }


        public async Task<IActionResult> OnPostSubirArchivoAsync(IFormFile archivo)
        {
            if (archivo == null || archivo.Length == 0)
            {
                ModelState.AddModelError(string.Empty, "No se ha seleccionado ningún archivo.");
                return Page();
            }
            string contenidoArchivo=string.Empty;
            // Leer el contenido del archivo
            if (archivo.ContentType == "application/pdf")
            {
                using var pdf = PdfDocument.Open(archivo.OpenReadStream());

                foreach (var page in pdf.GetPages())
                {
                    contenidoArchivo += page.Text;
                }
            }
            else
            {               
                using (var reader = new StreamReader(archivo.OpenReadStream()))
                {
                    contenidoArchivo = await reader.ReadToEndAsync();
                }
            }
            // Guardar el contenido del archivo en la base de datos o en la memoria
            var documentoActual = await _agenteAplicacion.IngestDocumentosParaRAG_Async(contenidoArchivo, archivo.FileName);
            
            // Redirigir a la misma página para mostrar el mensaje de éxito
           TempData["MensajeExito"] = "Archivo subido y procesado correctamente.";            
            return RedirectToPage(); 
        }

        public IActionResult OnGetDocumentos()
        {
            DocumentosRag = _agenteAplicacion.ObtenerDocumentosRag();
            return new JsonResult(DocumentosRag);
        }


    }
}
