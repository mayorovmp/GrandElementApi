using System.Collections.Generic;

namespace GrandElementApi.Data
{
    public class CarCategory : BaseEntity
    {

        public CarCategory()
        {
            Cars = new HashSet<Car>();
        }
        public int? Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Car> Cars { get; set; }
    }
}
