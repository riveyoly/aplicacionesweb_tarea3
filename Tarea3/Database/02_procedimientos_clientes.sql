SET client_min_messages = warning;

-- =============================================================
-- 02 - PROCEDIMIENTOS ALMACENADOS DEL CRUD DE CLIENTES (PostgreSQL)
--
--   C (Create) -> sp_clientes_insertar    (PROCEDURE, devuelve el id)
--   R (Read)   -> sp_clientes_listar      (FUNCTION, devuelve filas)
--                 sp_clientes_obtener     (FUNCTION, devuelve una fila)
--   U (Update) -> sp_clientes_actualizar  (PROCEDURE)
--   D (Delete) -> sp_clientes_eliminar    (PROCEDURE)
--
-- Nota: en PostgreSQL un PROCEDURE no puede devolver un conjunto de
-- filas; por eso las consultas (listar/obtener) son FUNCTION que
-- retornan TABLE, y las operaciones de escritura son PROCEDURE.
-- =============================================================

-- -------------------------------------------------------------
-- R: LISTAR (con búsqueda opcional por cédula, nombres o apellidos)
-- -------------------------------------------------------------
DROP FUNCTION IF EXISTS sp_clientes_listar(VARCHAR);

CREATE FUNCTION sp_clientes_listar(p_buscar VARCHAR DEFAULT NULL)
RETURNS TABLE (
    id               INTEGER,
    cedula           VARCHAR,
    nombres          VARCHAR,
    apellidos        VARCHAR,
    fecha_nacimiento DATE,
    genero           CHAR(1),
    email            VARCHAR,
    telefono         VARCHAR,
    celular          VARCHAR,
    direccion        VARCHAR,
    ciudad           VARCHAR,
    provincia        VARCHAR,
    limite_credito   NUMERIC,
    activo           BOOLEAN,
    fecha_registro   TIMESTAMP
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT c.id, c.cedula, c.nombres, c.apellidos, c.fecha_nacimiento,
           c.genero, c.email, c.telefono, c.celular, c.direccion,
           c.ciudad, c.provincia, c.limite_credito, c.activo, c.fecha_registro
    FROM clientes c
    WHERE p_buscar IS NULL
       OR p_buscar = ''
       OR c.cedula    ILIKE '%' || p_buscar || '%'
       OR c.nombres   ILIKE '%' || p_buscar || '%'
       OR c.apellidos ILIKE '%' || p_buscar || '%'
    ORDER BY c.apellidos, c.nombres;
END;
$$;

-- -------------------------------------------------------------
-- R: OBTENER UN CLIENTE POR ID
-- -------------------------------------------------------------
DROP FUNCTION IF EXISTS sp_clientes_obtener(INTEGER);

CREATE FUNCTION sp_clientes_obtener(p_id INTEGER)
RETURNS TABLE (
    id               INTEGER,
    cedula           VARCHAR,
    nombres          VARCHAR,
    apellidos        VARCHAR,
    fecha_nacimiento DATE,
    genero           CHAR(1),
    email            VARCHAR,
    telefono         VARCHAR,
    celular          VARCHAR,
    direccion        VARCHAR,
    ciudad           VARCHAR,
    provincia        VARCHAR,
    limite_credito   NUMERIC,
    activo           BOOLEAN,
    fecha_registro   TIMESTAMP
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT c.id, c.cedula, c.nombres, c.apellidos, c.fecha_nacimiento,
           c.genero, c.email, c.telefono, c.celular, c.direccion,
           c.ciudad, c.provincia, c.limite_credito, c.activo, c.fecha_registro
    FROM clientes c
    WHERE c.id = p_id;
END;
$$;

-- -------------------------------------------------------------
-- C: INSERTAR (devuelve el id generado en el parámetro INOUT p_id)
-- -------------------------------------------------------------
DROP PROCEDURE IF EXISTS sp_clientes_insertar(VARCHAR, VARCHAR, VARCHAR, DATE, CHAR, VARCHAR, VARCHAR, VARCHAR, VARCHAR, VARCHAR, VARCHAR, NUMERIC, BOOLEAN, INTEGER);

CREATE PROCEDURE sp_clientes_insertar(
    p_cedula           VARCHAR,
    p_nombres          VARCHAR,
    p_apellidos        VARCHAR,
    p_fecha_nacimiento DATE,
    p_genero           CHAR(1),
    p_email            VARCHAR,
    p_telefono         VARCHAR,
    p_celular          VARCHAR,
    p_direccion        VARCHAR,
    p_ciudad           VARCHAR,
    p_provincia        VARCHAR,
    p_limite_credito   NUMERIC,
    p_activo           BOOLEAN,
    INOUT p_id         INTEGER DEFAULT NULL
)
LANGUAGE plpgsql
AS $$
BEGIN
    INSERT INTO clientes (
        cedula, nombres, apellidos, fecha_nacimiento, genero, email,
        telefono, celular, direccion, ciudad, provincia, limite_credito, activo
    )
    VALUES (
        TRIM(p_cedula), TRIM(p_nombres), TRIM(p_apellidos), p_fecha_nacimiento,
        p_genero, LOWER(TRIM(p_email)), p_telefono, p_celular, p_direccion,
        p_ciudad, p_provincia, COALESCE(p_limite_credito, 0), COALESCE(p_activo, TRUE)
    )
    RETURNING id INTO p_id;
END;
$$;

-- -------------------------------------------------------------
-- U: ACTUALIZAR
-- -------------------------------------------------------------
DROP PROCEDURE IF EXISTS sp_clientes_actualizar(INTEGER, VARCHAR, VARCHAR, VARCHAR, DATE, CHAR, VARCHAR, VARCHAR, VARCHAR, VARCHAR, VARCHAR, VARCHAR, NUMERIC, BOOLEAN);

CREATE PROCEDURE sp_clientes_actualizar(
    p_id               INTEGER,
    p_cedula           VARCHAR,
    p_nombres          VARCHAR,
    p_apellidos        VARCHAR,
    p_fecha_nacimiento DATE,
    p_genero           CHAR(1),
    p_email            VARCHAR,
    p_telefono         VARCHAR,
    p_celular          VARCHAR,
    p_direccion        VARCHAR,
    p_ciudad           VARCHAR,
    p_provincia        VARCHAR,
    p_limite_credito   NUMERIC,
    p_activo           BOOLEAN
)
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE clientes
    SET cedula           = TRIM(p_cedula),
        nombres          = TRIM(p_nombres),
        apellidos        = TRIM(p_apellidos),
        fecha_nacimiento = p_fecha_nacimiento,
        genero           = p_genero,
        email            = LOWER(TRIM(p_email)),
        telefono         = p_telefono,
        celular          = p_celular,
        direccion        = p_direccion,
        ciudad           = p_ciudad,
        provincia        = p_provincia,
        limite_credito   = COALESCE(p_limite_credito, 0),
        activo           = COALESCE(p_activo, TRUE)
    WHERE id = p_id;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'No existe el cliente con id %', p_id USING ERRCODE = 'P0002';
    END IF;
END;
$$;

-- -------------------------------------------------------------
-- D: ELIMINAR
-- -------------------------------------------------------------
DROP PROCEDURE IF EXISTS sp_clientes_eliminar(INTEGER);

CREATE PROCEDURE sp_clientes_eliminar(p_id INTEGER)
LANGUAGE plpgsql
AS $$
BEGIN
    DELETE FROM clientes WHERE id = p_id;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'No existe el cliente con id %', p_id USING ERRCODE = 'P0002';
    END IF;
END;
$$;
