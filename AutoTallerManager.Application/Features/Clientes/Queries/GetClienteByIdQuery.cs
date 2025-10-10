using AutoTallerManager.Domain.Entities;
using MediatR;

namespace AutoTallerManager.Application.Features.Clientes.Queries;

public sealed record GetClienteByIdQuery(int Id) : IRequest<Cliente?>;


