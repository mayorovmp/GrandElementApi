using System.Collections.Generic;

namespace GrandElementApi.DTOs
{
    public class CarDTO
    {
        public int Id { get; set; }
        public string Owner { get; set; }
        public string Contacts { get; set; }
        public string Comments { get; set; }
        public string Unit { get; set; }
        public bool Vat { get; set; }
        public virtual CarCategoryDTO CarCategory { get; set; }
        public virtual ICollection<CarNumberDTO> CarNumbers { get; set; }
    }

    public class CarOnAddDTO
    {
        public string Owner { get; set; }
        public string Contacts { get; set; }
        public string Comments { get; set; }
        public string Unit { get; set; }
        public bool Vat { get; set; }
        public virtual CarCategoryDTO CarCategory { get; set; }
        public virtual ICollection<CarNumberDTO> CarNumbers { get; set; }
    }
}
