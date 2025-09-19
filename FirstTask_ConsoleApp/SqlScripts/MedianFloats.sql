WITH Ordered AS (
    SELECT -- Создаём CTE (Common Table Expression)
        FloatValue,
        ROW_NUMBER() OVER (ORDER BY FloatValue) AS rn,
        COUNT(*) OVER () AS cnt -- Присваиваем каждой строке номер по возрастанию
    FROM dbo.MyTable
)
SELECT 
    AVG(FloatValue*1.0) AS MedianValue
FROM Ordered
WHERE rn IN ((cnt+1)/2, (cnt+2)/2);  
