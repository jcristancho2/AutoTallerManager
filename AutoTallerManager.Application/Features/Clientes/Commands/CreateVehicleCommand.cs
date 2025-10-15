using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace AutoTallerManager.Application.Features.Clientes.Commands;

    public sealed record CreateVehicleCommand(
        string LicensePlate,
        int Milage,
        string VehicleBrandId,
        string VehicleModelId,
        int Year
    ) : IRequest<int>;
    
