using AutoTallerManager.Domain.Entities;

namespace AutoTallerManager.Application.Services;

public interface ICalculadoraFechasService
{
    DateTime CalcularFechaEstimadaEntrega(TipoServicio tipoServicio, int complejidad = 1);
    int CalcularComplejidadServicio(TipoServicio tipoServicio);
}

public class CalculadoraFechasService : ICalculadoraFechasService
{
    private readonly Dictionary<string, int> _diasPorTipoServicio = new()
    {
        { "Mantenimiento Preventivo", 1 },
        { "Cambio de Aceite", 1 },
        { "Diagnóstico", 1 },
        { "Reparación", 3 },
        { "Revisión Técnico-Mecánica", 2 }
    };

    private readonly Dictionary<string, int> _complejidadPorTipoServicio = new()
    {
        { "Mantenimiento Preventivo", 1 },
        { "Cambio de Aceite", 1 },
        { "Diagnóstico", 2 },
        { "Reparación", 3 },
        { "Revisión Técnico-Mecánica", 2 }
    };

    public DateTime CalcularFechaEstimadaEntrega(TipoServicio tipoServicio, int complejidad = 1)
    {
        var fechaBase = DateTime.UtcNow;
        var diasBase = _diasPorTipoServicio.GetValueOrDefault(tipoServicio.NombreTipoServ ?? "Reparación", 2);
        
        // Ajustar días según complejidad
        var diasFinales = diasBase + (complejidad - 1);
        
        // Asegurar mínimo 1 día
        diasFinales = Math.Max(1, diasFinales);
        
        return fechaBase.AddDays(diasFinales);
    }

    public int CalcularComplejidadServicio(TipoServicio tipoServicio)
    {
        return _complejidadPorTipoServicio.GetValueOrDefault(tipoServicio.NombreTipoServ ?? "Reparación", 2);
    }
}


