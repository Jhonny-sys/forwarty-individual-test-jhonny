-- 1.2 Los cinco clientes que más facturan
-- "Abiertas" = no CERRADA ni ANULADA (ver nota en RESPUESTAS.md)
SELECT
    cl.razon_social,
    COUNT(DISTINCT o.id) AS operaciones,
    COALESCE(SUM(c.valor_cop), 0) AS total_facturable
FROM clientes cl
JOIN operaciones o ON o.cliente_id = cl.id
LEFT JOIN costos c ON c.operacion_id = o.id AND c.facturable = 1
WHERE o.estado NOT IN ('CERRADA', 'ANULADA')
  AND o.fecha_apertura >= '2025-01-01'
  AND o.fecha_apertura <  '2026-01-01'
GROUP BY cl.id, cl.razon_social
ORDER BY total_facturable DESC
LIMIT 5;