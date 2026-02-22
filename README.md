# GrowMate

**GrowMate** — небольшое приложение для планирования посадок, ведения грядок и задач по уходу за растениями. Реализовано на ASP.NET Core Razor Pages с использованием Entity Framework Core и PostgreSQL.

**Ключевые возможности**
- Личный календарь задач с напоминаниями (FullCalendar)
- CRUD для посадок и грядок
- Загрузка фотографий посадок (сохранение в `wwwroot/uploads/plants`)
- Аутентификация через ASP.NET Identity

**Структура репозитория**
- **src/GrowMate.Presentation**: веб-приложение (Razor Pages, UI, загрузка файлов)
- **src/GrowMate.Applicatoin**: сервисы приложения и DTO
- **src/GrowMate.Infrastructure**: EF Core, конфигурация сущностей и миграции
- **src/GrowMate.Domain**: доменные сущности

**Требования**
- .NET 10 SDK/Runtime
- PostgreSQL (или контейнер с БД)
- Docker & docker-compose (рекомендуется для локального запуска)

**Быстрый старт (Docker Compose)**

Собрать и запустить контейнеры:

```powershell
docker compose build --no-cache --pull
docker compose up -d
```

Если в контейнере возникли проблемы с правами при сохранении загруженных изображений, пересоберите образ, передав UID/GID (они используются в Dockerfile):

```powershell
docker compose build --no-cache --pull --build-arg APP_UID=1000 --build-arg APP_GID=1000
docker compose up -d
```

Альтернатива: смонтировать локальную папку в контейнер для `wwwroot/uploads` в `docker-compose.yml`:

```yaml
services:
  web:
    volumes:
      - ./src/GrowMate.Presentation/wwwroot/uploads:/app/wwwroot/uploads
```

**Запуск локально (без Docker)**

```powershell
# восстановить зависимости
dotnet restore

# применить миграции к БД
dotnet ef database update --project src/GrowMate.Infrastructure/GrowMate.Infrastructure.csproj --startup-project src/GrowMate.Presentation/GrowMate.Presentation.csproj

# запустить приложение
dotnet run --project src/GrowMate.Presentation/GrowMate.Presentation.csproj
```

**Миграции**

Добавление миграции (пример):

```powershell
dotnet ef migrations add AddPlantImagePath --project src/GrowMate.Infrastructure/GrowMate.Infrastructure.csproj --startup-project src/GrowMate.Presentation/GrowMate.Presentation.csproj
```

**Загрузка изображений**
- Загруженные файлы сохраняются в `wwwroot/uploads/plants` и в базе хранится относительный URL `/uploads/plants/{filename}`.
- Для корректной отдачи файлов приложение включает статические файлы (`app.UseStaticFiles()`), поэтому URL можно использовать прямо в `img src`.

**Частые проблемы и отладка**
- Картинка не отображается (404): откройте DevTools → Network и посмотрите точный `GET`-запрос к `/uploads/plants/<file>`. Убедитесь, что файл существует внутри контейнера в `/app/wwwroot/uploads/plants`.
- Картинка не сохраняется (UnauthorizedAccessException): проблема прав записи в контейнере. Решения:
  - Пересоберите образ с аргументами `APP_UID`/`APP_GID` (см. Dockerfile) и перезапустите.
  - Смонтируйте внешний том/папку с корректными правами на запись.
  - Рассмотрите использование object storage (S3/Azure Blob) для продакшн-окружения.
- Статические файлы не отдаются: проверьте, что в [src/GrowMate.Presentation/Program.cs](src/GrowMate.Presentation/Program.cs) присутствует `app.UseStaticFiles()` и что приложение перезапущено.

**Полезные файлы**
- Конфигурация приложения и точка входа: [src/GrowMate.Presentation/Program.cs](src/GrowMate.Presentation/Program.cs)
- Dockerfile веб-приложения: [src/GrowMate.Presentation/Dockerfile](src/GrowMate.Presentation/Dockerfile)
- Обработчик сохранения фото посадки: [src/GrowMate.Presentation/Pages/Plants/Create.cshtml.cs](src/GrowMate.Presentation/Pages/Plants/Create.cshtml.cs)
- Сущность Plant (с полем ImagePath): [src/GrowMate.Domain/Plants/Plant.cs](src/GrowMate.Domain/Plants/Plant.cs)

**Разработка и тесты**
- В проекте пока нет автоматических юнит-тестов. Для разработки пользуйтесь локальным запуском и отладкой через `dotnet run`.

Если хотите, могу дополнить README разделом по CI/CD, инструкциями для запуска миграций в контейнере или примером `docker-compose.override.yml` с томами для локальной разработки.

***
Автор: команда разработки GrowMate
