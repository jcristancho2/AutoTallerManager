using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities
{
    public class Vehiculo
    {
        public int Id { get; private set; }
        public string? Placa { get; private set; }
        public int Ano { get; private set; }
        public string? Vin { get; private set; }
        public int Kilometraje { get; private set; }
        public int ClienteId { get; private set; }
        public Cliente Cliente { get; private set; } = null!;
        public int TipoVehiculoId { get; private set; }
        public TipoVehiculo TipoVehiculo { get; private set; } = null!;
        public int MarcaVehiculoId { get; private set; }
        public MarcaVehiculo MarcaVehiculo { get; private set; } = null!;
        public int ModeloVehiculoId { get; private set; }
        public ModeloVehiculo ModeloVehiculo { get; private set; } = null!;

        private Vehiculo() { }

        public Vehiculo(
            string placa,
            int ano,
            string vin,
            int kilometraje,
            int clienteId,
            int tipoVehiculoId,
            int marcaVehiculoId,
            int modeloVehiculoId
        )
        {
            Placa = placa.Trim().ToUpperInvariant();
            Ano = ano;
            Vin = vin.Trim().ToUpperInvariant();
            Kilometraje = kilometraje;
            ClienteId = clienteId;
            TipoVehiculoId = tipoVehiculoId;
            MarcaVehiculoId = marcaVehiculoId;
            ModeloVehiculoId = modeloVehiculoId; 
        }
    }
}