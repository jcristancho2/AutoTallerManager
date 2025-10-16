namespace AutoTallerManager.Application.Dtos
{
    public class RepuestoUpdateDto
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public string RowVersion { get; set; } // Base64
    }
}