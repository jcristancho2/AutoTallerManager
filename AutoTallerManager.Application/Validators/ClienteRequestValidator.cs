// using FluentValidation;
// using AutoTallerManager.Application.Features.Commands.CreateClienteCommand;


// namespace AutoTallerManager.Application.Validators;

// public class ClienteRequestValidator : AbstractValidator<CreateClienteCommand>
// {
//     public ClienteRequestValidator()
//     {
//         RuleFor(x => x.NombreCompleto)
//             .NotEmpty().WithMessage("El nombre completo es obligatorio.")
//             .MaximumLength(150).WithMessage("El nombre completo no puede exceder 150 caracteres.");

//         RuleFor(x => x.Telefono)
//             .NotEmpty().WithMessage("El teléfono es obligatorio.")
//             .MaximumLength(30).WithMessage("El teléfono no puede exceder 30 caracteres.");

//         RuleFor(x => x.Email)
//             .NotEmpty().WithMessage("El email es obligatorio.")
//             .EmailAddress().WithMessage("El email no tiene un formato válido.")
//             .MaximumLength(150).WithMessage("El email no puede exceder 150 caracteres.");

//         RuleFor(x => x.TipoCliente_Id)
//             .GreaterThan(0).WithMessage("El tipo de cliente es obligatorio.");

//         RuleFor(x => x.Direccion_Id)
//             .GreaterThan(0).WithMessage("La dirección es obligatoria.");
//     }
// }