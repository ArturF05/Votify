# Votify 🗳️

Aplicación web para organizar eventos competitivos y evaluar los proyectos participantes mediante votaciones configurables, con premios, resultados en tiempo real y notificaciones por corre.

> 🎓 Proyecto universitario desarrollado en equipo de 4 integrantes para la asignatura PSW/DDS, UPV, curso 2025/26.

## Funcionalidades
- Registro e inicio de sesión de usuarios con roles por evento (organizador, participante, jurado…).
- Creación, edición y archivado de eventos.
- Alta de proyectos con participantes y materiales adjuntos (imágenes, PDF, presentaciones, ZIP).
- Votaciones de jurado, público o mixtas, con criterios y categorías parametrizables.
- Gestión de premios por categoría y puesto, vinculados a votaciones.
- Dashboard de resultados por criterio.
- Síntesis de comentarios con IA (resumen, puntos fuertes y de mejora).

## Tecnologías
- **Frontend/Backend:** Blazor Server (.NET 8) + API REST con controladores.
- **Persistencia:** Entity Framework Core + PostgreSQL (Neon).
- **Ficheros:** Azure Blob Storage.
- **IA:** Google Gemini API.
- **Tests:** MSTest.

## Arquitectura
Solución dividida en capas:
- `Votify.Core`: entidades, interfaces, servicios y DTOs.
- `Votify.Infrastructure`: DbContext, migraciones y repositorios.
- `Vofity.API`: interfaz Blazor y controladores.
- `Votify.Core.Tests`: tests unitarios.

Patrones aplicados: **Repository**, **Factory** (creación de eventos y votaciones), **Strategy** (reglas de permiso de voto según el tipo de votación), **State** (comportamiento de la votación según su estado), **Observer** (envío de notificaciones por correo a los usuarios suscritos a un evento) y **Proxy** (validación de ficheros antes de subirlos a Azure).

## ⚠️ Estado actual del proyecto
El proyecto se desarrolló entre marzo y mayo de 2026 usando servicios cloud con cuentas de estudiante o gratuitas que ya no están activas. Por eso:
- **Las imágenes y materiales subidos no se muestran**: estaban alojados en Azure Blob Storage y esa cuenta ya no existe.
- **La base de datos original no está disponible**: para ejecutarlo hay que usar una base PostgreSQL propia.
- **La síntesis con IA** necesita una API key propia de Gemini.

El código está completo y refleja la funcionalidad desarrollada.

## Mi aportación
- Sistema de votaciones completo: emisión de votos, votación por criterios,
  pantallas de creación, edición y gestión, y visibilidad según el rol del usuario.
- Gestión de eventos: creación, edición, archivado y eliminación con confirmación.
- Dashboard de resultados por evento y rediseño de gran parte de la interfaz.
- Síntesis de comentarios con IA (con la API de Gemini).
- Implementación del patrón Strategy para los permisos de voto.
- Tests unitarios (MSTest), refactorings guiados (Explaining Variable, Magic Number,
  Inline Temp, Replace Conditional) y aplicación del patrón Strategy.
