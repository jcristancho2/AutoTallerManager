using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Application.Features.Clientes.Queries;
using AutoTallerManager.Domain.Entities;
using MediatR;

namespace AutoTallerManager.Application.Features.Clientes.Handlers;

public sealed class GetCustomerByIdHandler : IRequestHandler<GetCustomerByIdQuery, Customer?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCustomerByIdHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Customer?> Handle(GetCustomerByIdQuery request, CancellationToken ct)
    {
        var result = await _unitOfWork.Customer.GetAllAsync(
            filter: c => c.Id == request.Id,
            includeProperties: "Vehicles,Invoices",
            ct: ct
        );

        return result.FirstOrDefault();
    }
}


