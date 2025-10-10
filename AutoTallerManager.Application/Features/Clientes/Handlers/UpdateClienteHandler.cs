using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Application.Features.Clientes.Commands;
using MediatR;

namespace AutoTallerManager.Application.Features.Clientes.Handlers;

public sealed class UpdateClienteHandler : IRequestHandler<UpdateClienteCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateClienteHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<bool> Handle(UpdateClienteCommand request, CancellationToken ct)
    {
        throw new NotImplementedException("Actualizar cliente requiere métodos de dominio para mutar propiedades.");
    }
}


