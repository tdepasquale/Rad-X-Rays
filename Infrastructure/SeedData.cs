using System;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Infrastructure.Data;

namespace Infrastructure
{
    public class SeedData
    {
        public static async Task SeedDummyValuesAsync(DataContext context)
        {
            if (!context.DummyValues.Any())
            {

                var dummyValues = new DummyValue[] {
                new DummyValue{Id = Guid.NewGuid(), Name = "Value 1"},
                new DummyValue{Id = Guid.NewGuid(), Name = "Value 2"},
                new DummyValue{Id = Guid.NewGuid(), Name = "Value 3"}
            };

                foreach (DummyValue value in dummyValues)
                {
                    await context.DummyValues.AddAsync(value);
                }
                await context.SaveChangesAsync();
            }

        }
    }
}