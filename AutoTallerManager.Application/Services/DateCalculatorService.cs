using AutoTallerManager.Domain.Entities;

namespace AutoTallerManager.Application.Services;

public interface IDateCalculatorService
{
    DateTime CalculateEstimatedDeliveryDate(ServiceType serviceType, int complexity = 1);
    int CalculateServiceComplexity(ServiceType serviceType);
}

public class DateCalculatorService : IDateCalculatorService
{
    private readonly Dictionary<string, int> _daysPerServiceType = new()
    {
        { "Mantenimiento Preventivo", 1 },
        { "Cambio de Aceite", 1 },
        { "Diagnóstico", 1 },
        { "Reparación", 3 },
        { "Revisión Técnico-Mecánica", 2 }
    };

    private readonly Dictionary<string, int> _complexityPerServiceType = new()
    {
        { "Mantenimiento Preventivo", 1 },
        { "Cambio de Aceite", 1 },
        { "Diagnóstico", 2 },
        { "Reparación", 3 },
        { "Revisión Técnico-Mecánica", 2 }
    };

    public DateTime CalculateEstimatedDeliveryDate(ServiceType serviceType, int complexity = 1)
    {
        var baseDate = DateTime.UtcNow;
        var baseDays = _daysPerServiceType.GetValueOrDefault(serviceType.ServiceTypeName ?? "Reparación", 2);

        // Ajustar días según complejidad
        var finalDays = baseDays + (complexity - 1);

        // Asegurar mínimo 1 día
        finalDays = Math.Max(1, finalDays);
        
        // Sumar un pequeño desfase para evitar truncamiento a 0 días en comparaciones inmediatas
        return baseDate.AddDays(finalDays).AddMinutes(1);
    }

    public int CalculateServiceComplexity(ServiceType serviceType)
    {
        return _complexityPerServiceType.GetValueOrDefault(serviceType.ServiceTypeName ?? "Reparación", 2);
    }
}


