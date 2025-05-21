using Estudiante.Models;
using Estudiante.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;
using Estudiante.Data;

public class ProblemaController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProblemaController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Lista()
    {
        var problemas = _context.Problemas.ToList();
        return View(problemas);

    }
    [HttpGet]
    public IActionResult Registrar()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Registrar(Problema nuevoProblema)
    {
        nuevoProblema.Codigo = $"PC-{100 + _context.Problemas.Count() + 1}";

        if (nuevoProblema.CasosDePrueba.Count > 0)
        {
            nuevoProblema.InputEsperado = nuevoProblema.CasosDePrueba[0].Entrada.Trim();
            nuevoProblema.OutputEsperado = nuevoProblema.CasosDePrueba[0].SalidaEsperada.Trim();

            var adicionales = nuevoProblema.CasosDePrueba
                .Skip(1)
                .Select(cp => $"{cp.Entrada.Trim()}|||{cp.SalidaEsperada.Trim()}");

            nuevoProblema.CasosAdicionales = string.Join("|||", adicionales);
        }

        _context.Problemas.Add(nuevoProblema);
        _context.SaveChanges();
        return RedirectToAction("Lista");
    }

    [HttpPost]
    public async Task<IActionResult> EvaluarCodigo(int id, string codigo, int lenguaje)
    {
        var problema = _context.Problemas.FirstOrDefault(p => p.Id == id);
        if (problema == null) return NotFound();

        var judge0Service = new Judge0Service();

        List<(string input, string output)> casos = new()
    {
        (problema.InputEsperado, problema.OutputEsperado)
    };

        if (!string.IsNullOrWhiteSpace(problema.CasosAdicionales))
        {
            var bloques = problema.CasosAdicionales.Split("|||", StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i + 1 < bloques.Length; i += 2)
            {
                casos.Add((bloques[i].Trim(), bloques[i + 1].Trim()));
            }
        }

        List<object> resultados = new();
        bool todoCorrecto = true;

        foreach (var caso in casos)
        {
            try
            {
                var resultadoJson = await judge0Service.EnviarCodigoAsync(codigo, caso.input, lenguaje);
                var resultado = JsonSerializer.Deserialize<Judge0Result>(resultadoJson);

                string salida = resultado?.stdout ?? "";
                string error = resultado?.stderr ?? resultado?.compile_output ?? resultado?.message ?? "";

                bool correcto = salida.Trim() == caso.output.Trim();
                if (!correcto) todoCorrecto = false;

                resultados.Add(new
                {
                    correcto,
                    entrada = caso.input,
                    salida = string.IsNullOrWhiteSpace(salida) ? "null" : salida,
                    esperado = caso.output,
                    error = string.IsNullOrWhiteSpace(error) ? "null" : error
                });
            }
            catch (Exception ex)
            {
                resultados.Add(new
                {
                    correcto = false,
                    entrada = caso.input,
                    salida = "null",
                    esperado = caso.output,
                    error = ex.Message
                });
            }
        }

        return Json(new { correcto = todoCorrecto, resultados });
    }

    public IActionResult Resolver(int id)
    {
        var problema = _context.Problemas.FirstOrDefault(p => p.Id == id);
        return View(problema);
    }

    private bool EjecutarJava(string archivo, string entrada, out string salida, out string error)
    {
        salida = error = string.Empty;
        string carpeta = Path.GetDirectoryName(archivo)!;
        string clase = Path.GetFileNameWithoutExtension(archivo);

        try
        {
            var compilar = Process.Start(new ProcessStartInfo("javac", archivo)
            {
                RedirectStandardError = true,
                WorkingDirectory = carpeta
            });
            compilar.WaitForExit();
            if (compilar.ExitCode != 0)
            {
                error = compilar.StandardError.ReadToEnd();
                return false;
            }

            var ejecutar = new Process
            {
                StartInfo = new ProcessStartInfo("java", clase)
                {
                    WorkingDirectory = carpeta,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            ejecutar.Start();
            ejecutar.StandardInput.WriteLine(entrada);
            ejecutar.StandardInput.Close();
            salida = ejecutar.StandardOutput.ReadToEnd();
            error = ejecutar.StandardError.ReadToEnd();
            ejecutar.WaitForExit();
            return ejecutar.ExitCode == 0;
        }
        catch (Exception ex)
        {
            error = ex.Message;
            return false;
        }
    }
}

