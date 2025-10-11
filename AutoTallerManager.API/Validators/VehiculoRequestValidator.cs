using FluentValidation;
using AutoTallerManager.API.DTOs.Request;

namespace AutoTallerManager.API.Validators;

public class VehiculoRequestValidator : AbstractValidator<VehiculoRequest>
{
    public VehiculoRequestValidator()
    {
        RuleFor(x => x.Placa)
            .NotEmpty().WithMessage("La placa es obligatoria.")
            .MaximumLength(10).WithMessage("La placa no puede exceder 10 caracteres.");

        RuleFor(x => x.Anio)
            .InclusiveBetween(1886, DateTime.Now.Year).WithMessage("El año debe estar entre 1886 y el año actual.");

        RuleFor(x => x.VIN)
            .NotEmpty().WithMessage("El VIN es obligatorio.")
            .MaximumLength(30).WithMessage("El VIN no puede exceder 30 caracteres.");

        RuleFor(x => x.Kilometraje)
            .GreaterThanOrEqualTo(0).WithMessage("El kilometraje no puede ser negativo.");

        RuleFor(x => x.ClienteId)
            .GreaterThan(0).WithMessage("El ClienteId es obligatorio.");

        RuleFor(x => x.TipoVehiculoId)
            .GreaterThan(0).WithMessage("El TipoVehiculoId es obligatorio.");

        RuleFor(x => x.MarcaVehiculoId)
            .GreaterThan(0).WithMessage("El MarcaVehiculoId es obligatorio.");

        RuleFor(x => x.ModeloVehiculoId)
            .GreaterThan(0).WithMessage("El ModeloVehiculoId es obligatorio.");
    }
}