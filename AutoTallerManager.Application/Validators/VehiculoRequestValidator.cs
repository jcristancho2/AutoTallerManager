// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using FluentValidation;

// namespace AutoTallerManager.Application.Validators
// {
//     public class VehiculoRequestValidator : AbstractValidator<CreateVehiculoCommand>
//     {
//         public VehiculoRequestValidator()
//         {
//             RuleFor(x => x.Placa)
//                 .NotEmpty().WithMessage("La placa es obligatoria.")
//                 .MaximumLength(10).WithMessage("La placa no puede exceder 10 caracteres.");

//             RuleFor(x => x.Kilometraje)
//                 .GreaterThanOrEqualTo(0).WithMessage("El kilometraje no puede ser negativo.");
            
//             RuleFor(x => x.Marca)
//                 .NotEmpty().WithMessage("La marca es obligatoria.")
//                 .MaximumLength(100).WithMessage("La marca no puede exceder 100 caracteres.");

//             RuleFor(x => x.Modelo)
//                 .NotEmpty().WithMessage("El modelo es obligatorio.")
//                 .MaximumLength(100).WithMessage("El modelo no puede exceder 100 caracteres.");

//             RuleFor(x => x.Anio)
//                 .InclusiveBetween(1886, DateTime.Now.Year).WithMessage("El año debe estar entre 1886 y el año actual.");

           
//         }
//     }
// }