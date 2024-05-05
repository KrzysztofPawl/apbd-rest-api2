using System.ComponentModel.DataAnnotations;

namespace AsyncRestNew.Models;

public class WarehouseInput
{
    [Key]
    public int IdProduct { get; set; }
    public int IdWarehouse { get; set; }
    public int Amount { get; set; }
    public DateTime CreatedAt { get; set; }
    
}