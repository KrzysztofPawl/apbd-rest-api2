CREATE PROCEDURE AddProductToWarehouse @IdProduct INT, @IdWarehouse INT, @Amount INT,
                                       @CreatedAt DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @IdProductFromDb INT, @IdOrder INT, @Price DECIMAL(5, 2);

    BEGIN TRAN;

    SELECT @IdProductFromDb = p.IdProduct, @Price = p.Price
    FROM Product p
    WHERE p.IdProduct = @IdProduct;

    IF @IdProductFromDb IS NULL
    BEGIN
        RAISERROR('Invalid parameter: Provided IdProduct does not exist', 18, 0);
        ROLLBACK;
        RETURN;
    END;

    IF NOT EXISTS (SELECT 1 FROM Warehouse WHERE IdWarehouse = @IdWarehouse)
    BEGIN
        RAISERROR ('Invalid parameter: Provided IdWarehouse does not exist', 18, 0);
        ROLLBACK;
        RETURN;
    END;

    SELECT TOP 1 @IdOrder = o.IdOrder
    FROM "Order" o
             LEFT JOIN Product_Warehouse pw ON o.IdOrder = pw.IdOrder
    WHERE o.IdProduct = @IdProduct
      AND o.Amount >= @Amount
      AND pw.IdProductWarehouse IS NULL
      AND o.CreatedAt < @CreatedAt;

    IF @IdOrder IS NULL
    BEGIN
        RAISERROR ('Invalid parameter: There is no order to fulfill or it is already fulfilled', 18, 0);
        ROLLBACK;
        RETURN;
    END;

    UPDATE "Order"
    SET FulfilledAt = @CreatedAt
    WHERE IdOrder = @IdOrder;

    INSERT INTO Product_Warehouse (IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt)
    VALUES (@IdWarehouse, @IdProduct, @IdOrder, @Amount, @Amount * @Price, @CreatedAt);

    COMMIT;

    SELECT @@IDENTITY AS NewId;
END
go