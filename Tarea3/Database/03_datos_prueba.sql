-- =============================================================
-- 03 - DATOS DE PRUEBA (opcional). Usa el procedimiento de inserción.
-- =============================================================

CALL sp_clientes_insertar('0912345675', 'Ana María', 'Pérez Torres', '1995-04-10', 'F',
    'ana.perez@correo.com', '042345678', '0991234567', 'Av. 9 de Octubre 123',
    'Guayaquil', 'Guayas', 1500.00, TRUE);

CALL sp_clientes_insertar('1712345675', 'Carlos Andrés', 'Mora Vélez', '1988-11-22', 'M',
    'carlos.mora@correo.com', '022456789', '0987654321', 'Calle Amazonas N24-56',
    'Quito', 'Pichincha', 3000.00, TRUE);

CALL sp_clientes_insertar('0102030400', 'Lucía', 'Zambrano Ríos', '2000-02-15', 'F',
    'lucia.zambrano@correo.com', NULL, '0976543210', 'Calle Larga 7-45',
    'Cuenca', 'Azuay', 800.00, FALSE);
