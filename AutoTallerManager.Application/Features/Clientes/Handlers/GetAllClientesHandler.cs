using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Application.Features.Clientes.Queries;
using AutoTallerManager.Domain.Entities;
using MediatR;

namespace AutoTallerManager.Application.Features.Clientes.Handlers;

public sealed class GetAllClientesHandler : IRequestHandler<GetAllClientesQuery, IEnumerable<Cliente>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllClientesHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Cliente>> Handle(GetAllClientesQuery request, CancellationToken ct)
    {
        var clientes = await _unitOfWork.Clientes.GetAllAsync(
            filter: c => string.IsNullOrEmpty(request.SearchTerm)
                || (c.NombreCompleto != null && c.NombreCompleto.Contains(request.SearchTerm))
                || (c.Email != null && c.Email.Contains(request.SearchTerm)),
            orderBy: q => q.OrderBy(c => c.NombreCompleto),
            includeProperties: "Vehiculos,Facturas",
            skip: (request.PageNumber - 1) * request.PageSize,
            take: request.PageSize,
            ct: ct
        );

        return clientes;
    }
}


