using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Application.Features.Clientes.Queries;
using AutoTallerManager.Domain.Entities;
using MediatR;

namespace AutoTallerManager.Application.Features.Clientes.Handlers;

public sealed class GetClienteByIdHandler : IRequestHandler<GetClienteByIdQuery, Cliente?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetClienteByIdHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Cliente?> Handle(GetClienteByIdQuery request, CancellationToken ct)
    {
        var result = await _unitOfWork.Clientes.GetAllAsync(
            filter: c => c.Id == request.Id,
            includeProperties: "Vehiculos,Facturas",
            ct: ct
        );

        return result.FirstOrDefault();
    }
}


