# TrainingProject

## Описание
Шаблон проекта для прохождения стажировки. Стажер может использовать предложенную структуру или, по согласованию с ментором, свою.

## Начало работы
- Сделать форк проекта в свой репозиторий
- Переименовать **TrainingProject** согласно своему заданию
- Сконфигурировать логирование
- Выбрать движок и сконфигурировать подключение к БД

## Структура
- **TrainingProject.Web** - api 
- **TrainingProject.Domain.Logic** - должен содержать бизнес-логику проекта
- **TrainingProject.Domain** - содержит описание доменных моделей
- **TrainingProject.Data** - уровень работы с данными 
- **TrainingProject.Common** - общий функционал проекта. **НЕ** содержит иных зависимостей
- **TrainingProject.Logic.Tests** - тесты уровня логики

## Используемые пакеты по-умолчанию
- [Automapper](https://automapper.org)
- [NSwag](https://github.com/RicoSuter/NSwag)
- [Moq](https://github.com/moq/moq4)
- [EntityFramework.Core](https://docs.microsoft.com/en-us/ef/core)
- [Serilog](https://serilog.net)
- [FluentValidation](https://fluentvalidation.net)
- [FluentAssertions](https://fluentassertions.com)
- [xUnit](https://xunit.net)

По согласованию с ментором, стажер вправе использовать дополнительный набор сторонних пакетов.

## Лицензия
GPL v3 - open source
