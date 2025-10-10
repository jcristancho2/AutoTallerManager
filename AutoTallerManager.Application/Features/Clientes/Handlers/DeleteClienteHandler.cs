using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Application.Features.Clientes.Commands;
using MediatR;

namespace AutoTallerManager.Application.Features.Clientes.Handlers;

public sealed class DeleteClienteHandler : IRequestHandler<DeleteClienteCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteClienteHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<bool> Handle(DeleteClienteCommand request, CancellationToken ct)
    {
        throw new NotImplementedException("Eliminar cliente requiere ajustar repositorio para Id Guid.");
    }
}


