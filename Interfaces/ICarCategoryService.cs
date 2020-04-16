using GrandElementApi.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandElementApi.Interfaces
{
    public interface ICarCategoryService
    {
        public Task<List<CarCategory>> AllCategoriesAsync();
        public Task<CarCategory> EditCategoryAsync(CarCategory category);
        public Task<CarCategory> AddCategoryAsync(CarCategory category);
        public Task DeleteCategoryAsync(int carCategoryId);
    }
}
