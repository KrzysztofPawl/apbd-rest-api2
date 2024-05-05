using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AsyncRestNew.Models;

public class ProductWarehouse
{
    [Key]
    public int IdProductWarehouse { get; set; }
    
    [ForeignKey("Warehouse")]
    public int IdWarehouse { get; set; }
    
    [ForeignKey("Product")]
    public int IdProduct { get; set; }
    
    [ForeignKey("Order")]
    public int IdOrder { get; set; }
    public int Amount { get; set; }
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }

    public Product Product { get; set; }
    public Warehouse Warehouse { get; set; }
    public Order Order { get; set; }
}
