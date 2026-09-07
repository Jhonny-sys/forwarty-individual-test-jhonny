-- 1.3 — el índice, sin cambios en la lógica
-- Se adjunta query de exigencia (ver nota en RESPUESTAS.md)

CREATE INDEX idx_operaciones_fecha_apertura ON operaciones (fecha_apertura);

EXPLAIN SELECT o.id, o.numero_operacion, o.fecha_apertura
FROM operaciones o
WHERE o.fecha_apertura >= '2025-01-01' AND o.fecha_apertura < '2025-04-01'
ORDER BY o.fecha_apertura DESC;