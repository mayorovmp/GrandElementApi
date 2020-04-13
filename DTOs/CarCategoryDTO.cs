using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GrandElementApi.DTOs
{
    public class CarCategoryDTO
    {

        public int? Id { get; set; }
        public string Name { get; set; }
        public CarCategoryDTO() { }
    }

    public class CarCategoryOnAddDTO
    {
        public string Name { get; set; }
        public CarCategoryOnAddDTO() { }
    }

}
