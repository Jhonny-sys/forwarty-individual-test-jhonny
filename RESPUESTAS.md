En este documento especifico cada una de mis respuesta junto con la ejecución de cada paso para la resolución el mismo.

1.1 Operaciones de un cliente con su total

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

En la consulta traemos las operaciones del cliente id 21, con un left join a costos para que me traiga los valores de operaciones aunque no tenga costos, ordenado por fecha de apertura descendiente

1.2 Los cinco clientes que más facturan

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

La consulta se aplica sobre clientes, haciendo un join con las operaciones al ser obligatorio que haya facturadp, haciendo otro left join con costos, ya que si tiene una opreación, esta puede venir vacia, priorizando la opreación en la consulta, pero agregando la condicion sobre el join, asi optimizando la consulta para traer los datos y no aplicar una condicion where mas exigente, se toma un tando de fechas para calcular el año y que no esten cerradas ni anuladas y se limita a 5 para obtener los 5 clientes que mas facturan en orden descendete por total.

## 1.3 — Índice

**Índice propuesto:**
```sql
CREATE INDEX idx_operaciones_fecha_apertura ON operaciones (fecha_apertura);
```

**EXPLAIN antes del índice:**

| id | select_type | table | type | possible_keys | key  | rows | filtered | Extra                          |
|----|-------------|-------|------|----------------|------|------|----------|---------------------------------|
| 1  | SIMPLE      | o     | ALL  | NULL           | NULL | 800  | 11.11    | Using where; Using filesort    |

**EXPLAIN después del índice:**

| id | select_type | table | type | possible_keys                  | key  | rows | filtered | Extra                          |
|----|-------------|-------|------|----------------------------------|------|------|----------|---------------------------------|
| 1  | SIMPLE      | o     | ALL  | idx_operaciones_fecha_apertura  | NULL | 800  | 30.00    | Using where; Using filesort    |

**Análisis:** el índice se creó bien, MySQL lo reconoce (sale en `possible_keys`), pero al final no lo usó — `key` sigue en NULL y el plan sigue siendo `ALL` con filesort en los dos casos.

Tiene sentido: con solo 800 filas y un rango que trae como el 30% de la tabla, para MySQL es más barato hacer un scan completo que ponerse a saltar entre el índice y la tabla fila por fila. Eso no significa que el índice esté de más — con las "cientos de miles" de operaciones que menciona el enunciado para producción, ahí sí lo tomaría, porque el rango sería una porción mucho más chica del total. Si quisiera comprobarlo ya, sin esperar a que la tabla crezca, podría forzar su uso con `FORCE INDEX`.

EXPLAIN SELECT o.id, o.numero_operacion, o.fecha_apertura
FROM operaciones o FORCE INDEX (idx_operaciones_fecha_apertura)
WHERE o.fecha_apertura >= '2025-01-01' AND o.fecha_apertura < '2025-04-01'
ORDER BY o.fecha_apertura DESC;

Acá sí se ve la diferencia: `type` pasa de `ALL` a `range`, `key` ya no es NULL, y en `Extra` desapareció el `Using filesort` — aparece `Backward index scan`, que es MySQL 8 recorriendo el índice al revés para servir el `ORDER BY DESC` sin tener que ordenar aparte. Esto confirma que el índice sí ayuda cuando se usa, solo que con 800 filas el optimizador prefiere el full scan por su cuenta. En una tabla más grande no haría falta forzarlo.

2.1 Filtro por estado y paginación en el endpoint

Se agrego en el controlador de las operaciones paginación, con 3 condiciones basicas para el correcto funcionamiento y en caso que no vengan las variables.

if (page < 1) page = 1;
if (pageSize < 1) pageSize = 20;
if (pageSize > 100) pageSize = 100;

Con el conector, se agegan los parametros

public int Page { get; set; }
public int PageSize { get; set; }

y se agrega el filtro como query param de estado 

if (!string.IsNullOrWhiteSpace(estado))
    consulta = consulta.Where(o => o.Estado == estado);

Por ultimo, se devuelve la cantidad de objetos por pagina, la pagina actual en el response

return Ok(new OperacionListResponse
{
    Items = items,
    Total = total,
    Page = page,
    PageSize = pageSize
});

2.2 Conectarlo en la pantalla

Para el selector de estado, se agregó un signal estadosDisponibles que se llena al iniciar el componente (ngOnInit) llamando al endpoint /api/operaciones/estados que ya existía. Si esa llamada falla, no se bloquea el listado principal, el selector simplemente queda solo con la opción "Todos" y las operaciones se siguen cargando igual.

En el modelo (operaciones.model.ts) se agregó estado, page y pageSize al FiltroOperaciones, y page/pageSize al OperacionListResponse para que coincida con lo que ya devuelve el backend. En el servicio se agregaron esos mismos parámetros al HttpParams, solo si vienen definidos, igual que ya se hacía con desde y hasta.

Para la paginación se separó la lógica en dos métodos en el componente:

- buscar(): se dispara con el submit del formulario de filtros. Siempre resetea la página a 1 antes de cargar, porque si estoy en la página 5 y cambio el filtro de estado no tiene sentido quedarme ahí, ese resultado nuevo puede ni tener 5 páginas.
- cargar(): es el que realmente llama al servicio. Lo reutilizan tanto buscar() como los botones de anterior/siguiente, así no se duplica la llamada HTTP en dos lugares distintos.

El total de páginas se calcula con un computed a partir del total que devuelve el backend (el que ya viene filtrado) y el pageSize fijo de 20, no a partir de operaciones().length, porque eso solo contaría lo que trae la página actual y totalPaginas siempre daría 1.

Cuando el selector de estado está en "Todos" (value vacío), se manda undefined al servicio en vez de un string vacío, para que el HttpParams no agregue estado= vacío a la URL.

Para que la pantalla no quede en un estado raro:
- Mientras cargando() es true, se oculta toda la sección de tabla y paginación (el @if ya existente lo cubre), así los botones de paginación no quedan clickeables mientras hay una petición en curso.
- El @empty del @for que ya estaba en el template cubre el caso de "sin resultados para estos filtros".
- Los botones de Anterior y Siguiente se deshabilitan solos cuando estoy en la primera o última página (pagina() <= 1 y pagina() >= totalPaginas()).

2.3 El bug

Qué estaba mal: el filtro de hasta comparaba con "<= hasta.Value", pero hasta llega sin hora (00:00:00) y fecha_apertura sí tiene hora. Entonces "hasta=2025-03-31" en realidad cortaba justo a la medianoche del 31, no al final del día. Por eso OP-2025-00196, abierta esa misma tarde, quedaba fuera del rango.

Dónde lo arreglé: en el backend, cambiando la condición a que traiga todo lo que sea menor al día siguiente:

consulta = consulta.Where(o => o.FechaApertura < hasta.Value.Date.AddDays(1));

Lo arreglé ahí y no en el frontend porque el endpoint es el que debe garantizar el comportamiento correcto para cualquiera que lo consuma, no solo esta pantalla. Si lo resuelvo mandando la hora desde el frontend, cualquier otro consumidor del endpoint vuelve a pisar el mismo bug.

Para que no vuelva a pasar: agregaría un test con una operación abierta tarde en el día límite y validando que el filtro sí la trae. Es justo el caso borde que no se nota probando con fechas "redondas".