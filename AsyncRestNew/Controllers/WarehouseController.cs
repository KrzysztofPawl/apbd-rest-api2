using System.Data;
using AsyncRestNew.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace AsyncRestNew.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WarehouseController : ControllerBase
{
    private readonly WarehouseDbContext _context;

    public WarehouseController(WarehouseDbContext context)
    {
        _context = context;
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddProductToWarehouse([FromBody] WarehouseInput input)
    {

        if (input.Amount < 0)
        {
            return BadRequest("Amount must be grater than 0");
        }

        var product = await _context.Product.FindAsync(input.IdProduct);
        if (product == null)
        {
            return NotFound("Product not found");
        }

        var warehouse = await _context.Warehouse.FindAsync(input.IdWarehouse);
        if (warehouse == null)
        {
            return NotFound("Warehouse not found");
        }
        
        var order = await _context.Order
            .FirstOrDefaultAsync(o => o.IdProduct == input.IdProduct && o.Amount >= input.Amount && o.CreatedAt <= input.CreatedAt && o.FulfilledAt == null);
        if (order == null)
        {
            return BadRequest("No valid order, or order already fulfilled");
        }

        var existingProductWarehouse = await _context.Product_Warehouse
            .AnyAsync(pw => pw.IdOrder == order.IdOrder);
        if (existingProductWarehouse)
        {
            return BadRequest("Order has already been fulfilled");
        }
        
        order.FulfilledAt   = DateTime.Now;
        await _context.SaveChangesAsync();

        var productWarehouse = new ProductWarehouse
        {
            IdWarehouse = input.IdWarehouse,
            IdProduct = input.IdProduct,
            IdOrder = order.IdOrder,
            Amount = input.Amount,
            Price = product.Price * input.Amount,
            CreatedAt = DateTime.Now
        };

        _context.Product_Warehouse.Add(productWarehouse);
        await _context.SaveChangesAsync();

        return Ok(new { ProductWarehouseId = productWarehouse.IdProductWarehouse });
    }

    [HttpPost("addTrans")]
    public async Task<IActionResult> AddProductToWarehouseTrans([FromBody] WarehouseInput input)
    {
        if (input.Amount < 0)
        {
            return BadRequest("Amount must be grater than 0");
        }

        var connection = _context.Database.GetDbConnection();
        try
        {
            await connection.OpenAsync();
            using (var command = connection.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "AddProductToWarehouse";
                command.Parameters.Add(new SqlParameter("@IdProduct", input.IdProduct));
                command.Parameters.Add(new SqlParameter("@IdWarehouse", input.IdWarehouse));
                command.Parameters.Add(new SqlParameter("@Amount", input.Amount));
                command.Parameters.Add(new SqlParameter("@CreatedAt", input.CreatedAt));

                var result = await command.ExecuteScalarAsync();
                if (result != null && Convert.ToInt32(result) > 0)
                {
                    return Ok(new { NewId = result, Message = "Product added successfully." });
                }
                else
                {
                    return BadRequest("Failed to add product.");
                }


            }
        }
        catch (SqlException ex)
        {
            return StatusCode(500, $"SQL error: {ex.Message}");
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Internal server error: {e.Message}");
        }
        finally
        {
            await connection.CloseAsync();
        }

    }
}