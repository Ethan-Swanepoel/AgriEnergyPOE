using System.Collections.Generic;
using AgriEnergy.Models;

namespace AgriEnergy.ViewModels
{
    public class EmployeeViewModel
    {
        public int SelectedFarmerId { get; set; }
        public string SelectedCategory { get; set; }
        public IEnumerable<User> Farmers { get; set; }
        public IEnumerable<string> Categories { get; set; }
        public IEnumerable<Product> Products { get; set; }
    }
}
