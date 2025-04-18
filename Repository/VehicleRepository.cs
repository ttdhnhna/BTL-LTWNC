﻿using BTL_LTWNC.Data;
using BTL_LTWNC.Models;
using System;

namespace BTL_LTWNC.Repository
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly ApplicationDbContext _context;

        public VehicleRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public void AddVehicle(Vehicle vehicle)
        {
            _context.tbl_Vehicle.Add(vehicle);
            _context.SaveChanges();
        }

        public void UpdateVehicle(Vehicle vehicle)
        {
            _context.SaveChanges();
        }
    }
}
