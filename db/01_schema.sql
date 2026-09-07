-- =====================================================================
-- Forwarty — Prueba técnica
-- Esquema de la base. Lo carga solo el contenedor de MySQL al levantarse.
-- =====================================================================

CREATE DATABASE IF NOT EXISTS forwarty
  DEFAULT CHARACTER SET utf8mb4
  DEFAULT COLLATE utf8mb4_unicode_ci;

USE forwarty;

-- ---------------------------------------------------------------------
-- clientes — importadores y exportadores de la agencia
-- ---------------------------------------------------------------------
CREATE TABLE clientes (
    id            INT           NOT NULL AUTO_INCREMENT,
    nit           VARCHAR(20)   NOT NULL,
    razon_social  VARCHAR(150)  NOT NULL,
    ciudad        VARCHAR(80)   NOT NULL,
    activo        TINYINT(1)    NOT NULL DEFAULT 1,
    PRIMARY KEY (id)
) ENGINE=InnoDB;

-- ---------------------------------------------------------------------
-- operaciones — cada expediente de comercio exterior
-- ---------------------------------------------------------------------
CREATE TABLE operaciones (
    id                INT          NOT NULL AUTO_INCREMENT,
    cliente_id        INT          NOT NULL,
    numero_operacion  VARCHAR(30)  NOT NULL,
    tipo              VARCHAR(20)  NOT NULL,  -- IMPORTACION | EXPORTACION | TRANSITO
    modalidad         VARCHAR(20)  NOT NULL,  -- MARITIMO | AEREO | TERRESTRE
    estado            VARCHAR(20)  NOT NULL,  -- BORRADOR | EN_TRAMITE | LIQUIDADA | CERRADA | ANULADA
    fecha_apertura    DATETIME     NOT NULL,
    moneda            CHAR(3)      NOT NULL DEFAULT 'COP',
    PRIMARY KEY (id),
    CONSTRAINT fk_operaciones_cliente
        FOREIGN KEY (cliente_id) REFERENCES clientes(id)
) ENGINE=InnoDB;

-- ---------------------------------------------------------------------
-- costos — renglones de costo de cada operación (1 operación : N costos)
-- ---------------------------------------------------------------------
CREATE TABLE costos (
    id              BIGINT        NOT NULL AUTO_INCREMENT,
    operacion_id    INT           NOT NULL,
    concepto        VARCHAR(120)  NOT NULL,
    tipo            VARCHAR(30)   NOT NULL,
    proveedor       VARCHAR(150)  NULL,
    moneda          CHAR(3)       NOT NULL DEFAULT 'COP',
    valor           DECIMAL(18,2) NOT NULL,  -- monto en la moneda del renglón
    valor_cop       DECIMAL(18,2) NOT NULL,  -- monto convertido a pesos
    facturable      TINYINT(1)    NOT NULL DEFAULT 1,
    fecha_registro  DATE          NOT NULL,
    PRIMARY KEY (id),
    CONSTRAINT fk_costos_operacion
        FOREIGN KEY (operacion_id) REFERENCES operaciones(id)
) ENGINE=InnoDB;

-- Nota: el esquema solo tiene llaves primarias y foráneas. No hay ningún
-- otro índice.
