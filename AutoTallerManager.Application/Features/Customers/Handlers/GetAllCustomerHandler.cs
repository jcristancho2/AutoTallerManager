using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Application.Features.Clientes.Queries;
using AutoTallerManager.Domain.Entities;
using MediatR;

namespace AutoTallerManager.Application.Features.Clientes.Handlers;

public sealed class GetAllCustomerHandler : IRequestHandler<GetAllCustomerQuery, IEnumerable<Customer>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllCustomerHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Customer>> Handle(GetAllCustomerQuery request, CancellationToken ct)
    {
        var customers = await _unitOfWork.Customer.GetAllAsync(
            filter: c => string.IsNullOrEmpty(request.SearchTerm)
                || (c.FullName != null && c.FullName.Contains(request.SearchTerm))
                || (c.Email != null && c.Email.Contains(request.SearchTerm)),
            orderBy: q => q.OrderBy(c => c.FullName),
            includeProperties: "Vehicles,Invoices",
            skip: (request.PageNumber - 1) * request.PageSize,
            take: request.PageSize,
            ct: ct
        );

        return customers;
    }
}


