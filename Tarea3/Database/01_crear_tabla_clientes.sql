-- =============================================================
-- 01 - CREACIÓN DE LA TABLA CLIENTES (PostgreSQL)
-- Ejecutar conectado a la base de datos del proyecto (crud_clientes)
-- =============================================================

CREATE TABLE IF NOT EXISTS clientes (
    id               SERIAL        PRIMARY KEY,                  -- 1
    cedula           VARCHAR(13)   NOT NULL UNIQUE,              -- 2
    nombres          VARCHAR(100)  NOT NULL,                     -- 3
    apellidos        VARCHAR(100)  NOT NULL,                     -- 4
    fecha_nacimiento DATE          NULL,                         -- 5
    genero           CHAR(1)       NULL CHECK (genero IN ('M', 'F', 'O')), -- 6
    email            VARCHAR(150)  NULL,                         -- 7
    telefono         VARCHAR(15)   NULL,                         -- 8
    celular          VARCHAR(15)   NULL,                         -- 9
    direccion        VARCHAR(250)  NULL,                         -- 10
    ciudad           VARCHAR(80)   NULL,                         -- 11
    provincia        VARCHAR(80)   NULL,                         -- 12
    limite_credito   NUMERIC(12,2) NOT NULL DEFAULT 0 CHECK (limite_credito >= 0), -- 13
    activo           BOOLEAN       NOT NULL DEFAULT TRUE,        -- 14
    fecha_registro   TIMESTAMP     NOT NULL DEFAULT CURRENT_TIMESTAMP -- 15
);

CREATE INDEX IF NOT EXISTS ix_clientes_apellidos ON clientes (apellidos);
