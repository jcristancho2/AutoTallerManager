using AutoTallerManager.Application.Features.Customers.Commands;
using AutoTallerManager.Domain.Entities;
using FluentValidation;


namespace AutoTallerManager.Application.Validators;

public class CustomerRequestValidator : AbstractValidator<CreateCustomerCommand>
{
    public CustomerRequestValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("El nombre completo es obligatorio.")
            .MaximumLength(150).WithMessage("El nombre completo no puede exceder 150 caracteres.");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("El teléfono es obligatorio.")
            .MaximumLength(30).WithMessage("El teléfono no puede exceder 30 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es obligatorio.")
            .EmailAddress().WithMessage("El email no tiene un formato válido.")
            .MaximumLength(150).WithMessage("El email no puede exceder 150 caracteres.");

        RuleFor(x => x.CustomerTypeId)
            .GreaterThan(0).WithMessage("El tipo de cliente es obligatorio.");

        RuleFor(x => x.AddressId)
            .GreaterThan(0).WithMessage("La dirección es obligatoria.");
    }
}