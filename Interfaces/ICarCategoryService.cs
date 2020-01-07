﻿using GrandElementApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandElementApi.Interfaces
{
    public interface ICarCategoryService
    {
        public Task<List<CarCategory>> AllCategoriesAsync();
        public Task<CarCategory> EditCategoryAsync(int carCategoryId, string newName);
        public Task<CarCategory> AddCategoryAsync(string name);
        public Task DeleteCategoryAsync(int carCategoryId);
    }
}