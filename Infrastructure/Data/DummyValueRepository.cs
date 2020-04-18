using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class DummyValueRepository : IDummyValueRepository
    {
        private readonly DataContext _context;
        public DummyValueRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<DummyValue> GetValueByIdAsync(Guid id)
        {
            return await _context.DummyValues.Where(v => v.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<DummyValue>> GetValuesAsync()
        {
            return await _context.DummyValues.ToListAsync();
        }
    }
}