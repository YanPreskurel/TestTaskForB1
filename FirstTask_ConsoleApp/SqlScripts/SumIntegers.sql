SELECT -- Выбираем сумму всех значений из колонки IntValue
    SUM(CAST(IntValue AS BIGINT)) AS TotalSum  -- Преобразуем IntValue в BIGINT, чтобы избежать переполнения
FROM 
    dbo.MyTable;
