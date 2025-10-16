using AutoTallerManager.Domain.Entities;
using MediatR;

namespace AutoTallerManager.Application.Features.Customers.Queries;

public sealed record GetCustomerByIdQuery(int Id) : IRequest<Customer?>;


