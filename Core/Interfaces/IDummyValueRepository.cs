using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IDummyValueRepository
    {
        Task<DummyValue> GetValueByIdAsync(Guid id);
        Task<IReadOnlyList<DummyValue>> GetValuesAsync();
    }
}