SELECT -- �������� ����� ���� �������� �� ������� IntValue
    SUM(CAST(IntValue AS BIGINT)) AS TotalSum  -- ����������� IntValue � BIGINT, ����� �������� ������������
FROM 
    dbo.MyTable;
