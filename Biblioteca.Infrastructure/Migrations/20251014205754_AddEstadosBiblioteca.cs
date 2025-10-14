using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Biblioteca.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEstadosBiblioteca : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ============================
            //  USUARIO.Activo -> default TRUE
            // ============================
            migrationBuilder.AlterColumn<bool>(
                name: "Activo",
                table: "Usuario",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");

            // ============================
            //  LIBRO.CondicionGeneral  (crear si no existe, asegurar tipo, normalizar datos)
            // ============================

            // 1) Crear columna CondicionGeneral si NO existe (SQL dinámico)
            migrationBuilder.Sql(@"
SET @sql := IF(
  (SELECT COUNT(*) FROM information_schema.COLUMNS 
   WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'Libro' AND COLUMN_NAME = 'CondicionGeneral') = 0,
  'ALTER TABLE `Libro` ADD COLUMN `CondicionGeneral` varchar(10) NOT NULL DEFAULT ''Good'';',
  'SELECT 1;'
);
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;
");

            // 2) Asegurar tipo/longitud/NOT NULL/default de CondicionGeneral
            migrationBuilder.Sql(@"
ALTER TABLE `Libro`
MODIFY COLUMN `CondicionGeneral` varchar(10) NOT NULL DEFAULT 'Good';
");

            // 3) Normalizar datos inválidos previos
            migrationBuilder.Sql(@"
UPDATE `Libro`
SET `CondicionGeneral` = 'Good'
WHERE `CondicionGeneral` IS NULL
   OR `CondicionGeneral` NOT IN ('New','Good','Fair','Poor');
");

            // 4) Borrar CHECK existente (si lo hay) y crear el nuevo
            migrationBuilder.Sql(@"
SET @sql := (
  SELECT IF(EXISTS(
      SELECT 1
      FROM information_schema.TABLE_CONSTRAINTS
      WHERE CONSTRAINT_SCHEMA = DATABASE()
        AND TABLE_NAME = 'Libro'
        AND CONSTRAINT_NAME = 'CK_Libro_Condicion'
        AND CONSTRAINT_TYPE = 'CHECK'
    ),
    'ALTER TABLE `Libro` DROP CHECK `CK_Libro_Condicion`;',
    'SELECT 1;'
  )
);
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;
");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Libro_Condicion",
                table: "Libro",
                sql: "CondicionGeneral IN ('New','Good','Fair','Poor')"
            );

            // ============================
            //  LIBRO.Habilitado  (crear si no existe, asegurar tipo/default)
            // ============================

            // 5) Crear columna Habilitado si NO existe (SQL dinámico)
            migrationBuilder.Sql(@"
SET @sql := IF(
  (SELECT COUNT(*) FROM information_schema.COLUMNS 
   WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'Libro' AND COLUMN_NAME = 'Habilitado') = 0,
  'ALTER TABLE `Libro` ADD COLUMN `Habilitado` tinyint(1) NOT NULL DEFAULT 1;',
  'SELECT 1;'
);
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;
");

            // 6) Asegurar NOT NULL + default 1 de Habilitado
            migrationBuilder.Sql(@"
ALTER TABLE `Libro`
MODIFY COLUMN `Habilitado` tinyint(1) NOT NULL DEFAULT 1;
");

            // ============================
            //  CHECK de USUARIO.Rol  (estudiante|staff)
            // ============================
            migrationBuilder.Sql(@"
SET @sql := (
  SELECT IF(EXISTS(
      SELECT 1
      FROM information_schema.TABLE_CONSTRAINTS
      WHERE CONSTRAINT_SCHEMA = DATABASE()
        AND TABLE_NAME = 'Usuario'
        AND CONSTRAINT_NAME = 'CK_Usuario_Rol'
        AND CONSTRAINT_TYPE = 'CHECK'
    ),
    'ALTER TABLE `Usuario` DROP CHECK `CK_Usuario_Rol`;',
    'SELECT 1;'
  )
);
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;
");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Usuario_Rol",
                table: "Usuario",
                sql: "Rol IN ('estudiante','staff')"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Quitar CHECKs si existen (sin romper en versiones viejas)
            migrationBuilder.Sql(@"
SET @sql := (
  SELECT IF(EXISTS(
      SELECT 1
      FROM information_schema.TABLE_CONSTRAINTS
      WHERE CONSTRAINT_SCHEMA = DATABASE()
        AND TABLE_NAME = 'Libro'
        AND CONSTRAINT_NAME = 'CK_Libro_Condicion'
        AND CONSTRAINT_TYPE = 'CHECK'
    ),
    'ALTER TABLE `Libro` DROP CHECK `CK_Libro_Condicion`;',
    'SELECT 1;'
  )
);
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;
");

            migrationBuilder.Sql(@"
SET @sql := (
  SELECT IF(EXISTS(
      SELECT 1
      FROM information_schema.TABLE_CONSTRAINTS
      WHERE CONSTRAINT_SCHEMA = DATABASE()
        AND TABLE_NAME = 'Usuario'
        AND CONSTRAINT_NAME = 'CK_Usuario_Rol'
        AND CONSTRAINT_TYPE = 'CHECK'
    ),
    'ALTER TABLE `Usuario` DROP CHECK `CK_Usuario_Rol`;',
    'SELECT 1;'
  )
);
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;
");

            // Revertir default de Usuario.Activo (opcional)
            migrationBuilder.AlterColumn<bool>(
                name: "Activo",
                table: "Usuario",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)",
                oldDefaultValue: true
            );

            // (Opcional) Si quisieras realmente eliminar columnas:
            // migrationBuilder.Sql("ALTER TABLE `Libro` DROP COLUMN `CondicionGeneral`;");
            // migrationBuilder.Sql("ALTER TABLE `Libro` DROP COLUMN `Habilitado`;");
        }
    }
}
