using AutoTallerManager.Domain.Entities;
using MediatR;

namespace AutoTallerManager.Application.Features.Clientes.Queries;

public sealed record GetCustomerByIdQuery(int Id) : IRequest<Customer?>;


