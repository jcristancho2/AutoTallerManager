using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities;

    public class VehicleModel : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public int BrandId { get; set; }
        public VehicleBrand Brand { get; set; } = null!;

    public VehicleModel(string name)
    {
        Name = name;

    }

    public ICollection<Vehicle> Vehicles { get; set; } = new HashSet<Vehicle>();

    

    }

