# Mi Barbería Digital 💈

Sistema de gestión para barberías desarrollado en .NET 8 con Windows Forms y SQL Server en Docker.

## 🚀 Cómo ejecutar el proyecto

Este proyecto utiliza **Docker** para la base de datos, por lo que no necesitas instalar SQL Server manualmente.

### Requisitos previos
* Docker Desktop instalado y corriendo.
* .NET SDK 8.

### Pasos

1. **Clonar el repositorio:**
   ```bash
   git clone [https://github.com/Anibalista/MiBarberiaDigital.git](https://github.com/Anibalista/MiBarberiaDigital.git)
   
2. Levantar la Base de Datos: Abre una terminal en la carpeta raíz del proyecto y ejecuta:
   ```bash
   docker-compose up -d
   ```
   (Esto descargará e iniciará un contenedor con SQL Server listo para usar.)
   
3. Ejecutar la Aplicación: Abre la solución ``` MiBarberiaDigital.sln ``` en Visual Studio y presiona Iniciar.

Nota: La aplicación ejecutará automáticamente las migraciones al iniciar, creando las tablas y relaciones necesarias en el contenedor.

## 🛠 Tecnologías
Lenguaje: C# (.NET 8)

Framework: Windows Forms

ORM: Entity Framework Core

Base de Datos: SQL Server 2022 (Dockerizado)

Arquitectura: N-Capas (Entidades, Datos, Negocio, Presentación)
