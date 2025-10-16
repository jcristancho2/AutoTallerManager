namespace AutoTallerManager.Application.Dtos
{
    public class OrdenServicioUpdateDto
    {
        public string NumeroOrden { get; set; }
        public string Estado { get; set; }
        public decimal Total { get; set; }
        public string RowVersion { get; set; } // Base64
    }
}