using System.ComponentModel.DataAnnotations;

namespace AsyncRestNew.Models;

public class Warehouse
{
    [Key]
    public int IdWarehouse { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    
}