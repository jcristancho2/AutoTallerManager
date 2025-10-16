namespace AutoTallerManager.Application.Dtos
{
    public class RepuestoResponseDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public string RowVersion { get; set; } // Base64
    }
}