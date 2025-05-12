namespace ContenidoEducativoApi.Models
{
    public class Contenido
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string ImagenURL { get; set; }
    }
}
