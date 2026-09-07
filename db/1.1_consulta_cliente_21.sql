-- 1.1 Operaciones de un cliente con su total
SELECT
    o.numero_operacion,
    o.estado,
    o.fecha_apertura,
    COALESCE(SUM(c.valor_cop), 0) AS total_cop
FROM operaciones o
LEFT JOIN costos c ON c.operacion_id = o.id
WHERE o.cliente_id = 21
GROUP BY o.id, o.numero_operacion, o.estado, o.fecha_apertura
ORDER BY o.fecha_apertura DESC;