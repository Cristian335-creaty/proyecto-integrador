using Microsoft.AspNetCore.Mvc;
using Estudiante.Models;
using System.Diagnostics;

public class ProblemaController : Controller
{
    private static List<Problema> problemas = new()
    {
        new Problema {
            Id = 1, Codigo = "PC-142", Titulo = "Camino más corto con restricciones",
            Descripcion = "Dado un grafo...", Dificultad = "Media", Temas = "Grafos, Dijkstra",
            InputEsperado = "5\n1 2 3 4 5", OutputEsperado = "15"
        },
        new Problema {
            Id = 2, Codigo = "PC-087", Titulo = "Subsecuencia palindrómica",
            Descripcion = "Dado un string...", Dificultad = "Fácil", Temas = "DP, Strings",
            InputEsperado = "abcba", OutputEsperado = "5"
        },
        new Problema {
            Id = 3, Codigo = "PC-205", Titulo = "Área máxima bajo histograma",
            Descripcion = "Dado un histograma...", Dificultad = "Difícil", Temas = "Stack, Divide y vencerás",
            InputEsperado = "6\n2 1 5 6 2 3", OutputEsperado = "10"
        }
    };


    public IActionResult Lista()
    {
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
        nuevoProblema.Id = problemas.Count + 1;
        nuevoProblema.Codigo = $"PC-{100 + problemas.Count + 1}";

        if (nuevoProblema.CasosDePrueba.Count > 0)
        {
            nuevoProblema.InputEsperado = nuevoProblema.CasosDePrueba[0].Entrada;
            nuevoProblema.OutputEsperado = nuevoProblema.CasosDePrueba[0].SalidaEsperada;

            var adicionales = nuevoProblema.CasosDePrueba.Skip(1)
                .Select(cp => $"{cp.Entrada}\n{cp.SalidaEsperada}")
                .ToList();

            nuevoProblema.CasosAdicionales = string.Join("\n", adicionales);
        }

        problemas.Add(nuevoProblema);
        return RedirectToAction("Lista");
    }

    [HttpPost]
    public IActionResult EvaluarCodigo(int id, string codigo)
    {
        var problema = problemas.FirstOrDefault(p => p.Id == id);
        if (problema == null) return NotFound();

        string nombreClase = "Solucion";
        string codigoCompleto = codigo.Replace("public class", $"public class {nombreClase}");
        string archivo = Path.Combine(Path.GetTempPath(), $"{nombreClase}.java");
        System.IO.File.WriteAllText(archivo, codigoCompleto);

        List<(string input, string output)> casos = new()
    {
        (problema.InputEsperado, problema.OutputEsperado)
    };

        if (!string.IsNullOrWhiteSpace(problema.CasosAdicionales))
        {
            var lineas = problema.CasosAdicionales.Split("\n", StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i + 1 < lineas.Length; i += 2)
            {
                casos.Add((lineas[i].Trim(), lineas[i + 1].Trim()));
            }
        }

        List<object> resultados = new();
        bool todoCorrecto = true;

        foreach (var caso in casos)
        {
            bool exito = EjecutarJava(archivo, caso.input, out string salida, out string error);
            bool correcto = salida.Trim() == caso.output.Trim();
            if (!correcto) todoCorrecto = false;

            resultados.Add(new { correcto, entrada = caso.input, salida, esperado = caso.output, error });
        }

        return Json(new { correcto = todoCorrecto, resultados });
    }




    public IActionResult Resolver(int id)
    {
        var problema = problemas.FirstOrDefault(p => p.Id == id);
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

