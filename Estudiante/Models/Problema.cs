namespace Estudiante.Models
{
    public class Problema
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Dificultad { get; set; } = string.Empty;
        public string Temas { get; set; } = string.Empty;
        public string InputEsperado { get; set; } = string.Empty;
        public string OutputEsperado { get; set; } = string.Empty;
        public string CasosAdicionales { get; set; } = string.Empty;
        public List<CasoPrueba> CasosDePrueba { get; set; } = new();

    }
}


