using Inicio2.Models.Estudiantes;
using Estudiante.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;
using Inicio2.Data;

public class ProblemController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly Judge0Service _judge0Service;
    public ProblemController(Judge0Service judge0Service, ApplicationDbContext context)
    {
        _judge0Service = judge0Service;
        _context = context;
    }

    
    public IActionResult ListaDocente()
    {
        var problem = _context.Problems.ToList();
        return View("ListaDocente", problem); 
    }

    public IActionResult ListaEstudiante()
    {
        var problem = _context.Problems.ToList();
        return View("ListaEstudiante", problem); 
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Register(Problem newProblem)
    {
        newProblem.Code = $"PC-{100 + _context.Problems.Count() + 1}";

        if (newProblem.TestCases.Count > 0)
        {
            newProblem.SampleInput = newProblem.TestCases[0].Input.Trim();
            newProblem.SampleOutput = newProblem.TestCases[0].ExpectedOutput.Trim();

            var additional = newProblem.TestCases
                .Skip(1)
                .Select(tc => $"{tc.Input.Trim()}|||{tc.ExpectedOutput.Trim()}");

            newProblem.AdditionalCases = string.Join("|||", additional);
        }

        _context.Problems.Add(newProblem);
        _context.SaveChanges();
        return RedirectToAction("ListaDocente");
    }

    [HttpPost]
    public async Task<IActionResult> EvaluateCode(int id, string code, int language)
    {
        var problem = _context.Problems.FirstOrDefault(p => p.Id == id);
        if (problem == null) return NotFound();

        List<(string input, string output)> cases = new()
        {
            (problem.SampleInput, problem.SampleOutput)
        };

        if (!string.IsNullOrWhiteSpace(problem.AdditionalCases))
        {
            var blocks = problem.AdditionalCases.Split("|||", StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i + 1 < blocks.Length; i += 2)
            {
                cases.Add((blocks[i].Trim(), blocks[i + 1].Trim()));
            }
        }

        List<object> results = new();
        bool allCorrect = true;

        foreach (var testCase in cases)
        {
            try
            {
                var resultJson = await _judge0Service.SendCodeAsync(code, testCase.input, language);
                var result = JsonSerializer.Deserialize<Judge0Result>(resultJson);

                string output = result?.stdout ?? "";
                string error = result?.stderr ?? result?.compile_output ?? result?.message ?? "";

                bool correct = output.Trim() == testCase.output.Trim();
                if (!correct) allCorrect = false;

                results.Add(new
                {
                    correct,
                    input = testCase.input,
                    output = string.IsNullOrWhiteSpace(output) ? "null" : output,
                    expected = testCase.output,
                    error = string.IsNullOrWhiteSpace(error) ? "null" : error
                });
            }
            catch (Exception ex)
            {
                results.Add(new
                {
                    correct = false,
                    input = testCase.input,
                    output = "null",
                    expected = testCase.output,
                    error = ex.Message
                });
            }
        }

        return Json(new { correct = allCorrect, results });
    }

    public IActionResult Solve(int id)
    {
        var problem = _context.Problems.FirstOrDefault(p => p.Id == id);
        return View(problem);
    }

    [HttpGet]
    public IActionResult Detalles(int id)
    {
        var problem = _context.Problems.FirstOrDefault(p => p.Id == id);
        if (problem == null) return NotFound();

        return View(problem);
    }

    [HttpGet]
    public IActionResult Editar(int id)
    {
        var problema = _context.Problems.FirstOrDefault(p => p.Id == id);
        if (problema == null) return NotFound();

        return View(problema);
    }

    [HttpPost]
    public IActionResult Editar(Problem problemaEditado)
    {
        var existente = _context.Problems.FirstOrDefault(p => p.Id == problemaEditado.Id);
        if (existente == null) return NotFound();

        existente.Title = problemaEditado.Title;
        existente.Description = problemaEditado.Description;
        existente.Difficulty = problemaEditado.Difficulty;
        existente.Topics = problemaEditado.Topics;
        existente.SampleInput = problemaEditado.SampleInput;
        existente.SampleOutput = problemaEditado.SampleOutput;

        _context.SaveChanges();
        return RedirectToAction("ListaDocente");
    }

    public async Task<IActionResult> Eliminar(int id)
    {
        var problema = _context.Problems.FirstOrDefault(p => p.Id == id);
        if (problema == null) return NotFound();

        return View(problema);
    }

    // This method is not used in the current workflow but kept for reference.
    private bool RunJava(string file, string input, out string output, out string error)
    {
        output = error = string.Empty;
        string folder = Path.GetDirectoryName(file)!;
        string className = Path.GetFileNameWithoutExtension(file);

        try
        {
            var compile = Process.Start(new ProcessStartInfo("javac", file)
            {
                RedirectStandardError = true,
                WorkingDirectory = folder
            });
            compile.WaitForExit();
            if (compile.ExitCode != 0)
            {
                error = compile.StandardError.ReadToEnd();
                return false;
            }

            var execute = new Process
            {
                StartInfo = new ProcessStartInfo("java", className)
                {
                    WorkingDirectory = folder,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            execute.Start();
            execute.StandardInput.WriteLine(input);
            execute.StandardInput.Close();
            output = execute.StandardOutput.ReadToEnd();
            error = execute.StandardError.ReadToEnd();
            execute.WaitForExit();
            return execute.ExitCode == 0;
        }
        catch (Exception ex)
        {
            error = ex.Message;
            return false;
        }
    }
}