using Docentes2.Models;
using Docentes2.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Net.Http.Headers;

namespace Docentes2.Controllers
{
    public class ContenidoController : Controller
    {
        private readonly IContenidoService _contenidoService;

        public ContenidoController(IContenidoService contenidoService)
        {
            _contenidoService = contenidoService;
        }

        public async Task<IActionResult> Index()
        {
            var lista = await _contenidoService.GetAllAsync();
            return View(lista);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Contenido contenido, IFormFile imagen)
        {
            if (imagen != null && imagen.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await imagen.CopyToAsync(memoryStream);
                var base64Image = Convert.ToBase64String(memoryStream.ToArray());

                using var httpClient = new HttpClient();
                var apiKey = "5f0e1dd6af3ff4b1afcc7dfc4fa5fc6b"; 

                var content = new MultipartFormDataContent();
                content.Add(new StringContent(apiKey), "key");
                content.Add(new StringContent(base64Image), "image");

                var response = await httpClient.PostAsync("https://api.imgbb.com/1/upload", content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var json = JsonDocument.Parse(responseString);
                    var imageUrl = json.RootElement
                                       .GetProperty("data")
                                       .GetProperty("url")
                                       .GetString();

                    contenido.ImagenURL = imageUrl; // Asumiendo que tu modelo tiene esta propiedad
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al subir la imagen.");
                    return View(contenido);
                }
            }

            await _contenidoService.CreateAsync(contenido);
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var contenido = await _contenidoService.GetByIdAsync(id);
            if (contenido == null)
                return NotFound();

            return View(contenido);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(Contenido contenido, IFormFile nuevaImagen)
        {
            if (!ModelState.IsValid)
                return View(contenido);

            if (nuevaImagen != null && nuevaImagen.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await nuevaImagen.CopyToAsync(memoryStream);
                var base64Image = Convert.ToBase64String(memoryStream.ToArray());

                var apiKey = "5f0e1dd6af3ff4b1afcc7dfc4fa5fc6b"; 
                var client = new HttpClient();
                var content = new MultipartFormDataContent
        {
            { new StringContent(apiKey), "key" },
            { new StringContent(base64Image), "image" }
        };

                var response = await client.PostAsync("https://api.imgbb.com/1/upload", content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var json = JsonDocument.Parse(responseString);
                    var imageUrl = json.RootElement
                                       .GetProperty("data")
                                       .GetProperty("url")
                                       .GetString();

                    contenido.ImagenURL = imageUrl;
                }
                else
                {
                    ModelState.AddModelError("", "Error al subir la nueva imagen.");
                    return View(contenido);
                }
            }
            else
            {
                // Si no se subió nueva imagen, mantenemos la actual (ya viene en el hidden field)
            }

            await _contenidoService.UpdateAsync(contenido);

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(int id)
        {
            var contenido = await _contenidoService.GetByIdAsync(id);
            return View(contenido);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            await _contenidoService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
