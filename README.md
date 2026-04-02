# HoneyDoThis
A relationship preserving Full Stack Web Application for domestic tasks/duties using ASP.NET Core, Angular 16+ and SQL Server.

SQL:
<ul>
  <li>Create a SQL<code>HoneyDoThis.sql</code></li>
  <li>Database Setup and Schema Creation: Creates a HoneyDoThisDb database if it doesn't exist, then defines two core tables, Tasks and Subtasks, with a foreign key relationship. It enforces data integrity through constraints (e.g., non-empty text, non-negative order) and adds indexes to optimize query performance.</li>
  <li>Sample Data Population: Populates the tables with substantial sample data, creating 10 main tasks and numerous related subtasks for each. This provides a realistic dataset for development and testing of a task management application.</li>
  <li>Advanced Database Objects: Creates views, stored procedures, and a user-defined function to support common application logic. This includes a task statistics view, procedures for reordering items and clearing completed tasks, and a function to calculate task completion percentages.</li>
</ul>

ASP.Net Core:
<ul>
  <li>Visual Studio 2022: Create a new project("Angular and ASP.NET Core").</li>
  <li>Add Required NuGet Packages
    <ul>
      <li><code>Microsoft.EntityFrameworkCore 8.0.13</code></li>
      <li><code>Microsoft.EntityFrameworkCore.SqlServer 8.0.13</code></li>
      <li><code>Microsoft.EntityFrameworkCore.Tools 8.0.13</code></li>
      <li><code>AutoMapper</code>
        <ul>
          <li>Simplifies the process of transferring data between different objects</li>
        </ul>
      </li>
    </ul>
  </li>
  <li>Create model entities with data annotations. <code>TaskEntity.cs SubtaskEntity.cs</code></li>
  <li>Create model Dtos(Data Transder Objects) to transfer only the required data between the client and server. <code>TaskDto.cs SubtaskDto.cs</code></li>
  <li>Create Mapping Profile for Entity and Dto. <code>MappingProfile.cs</code></li>
  <li>Create Database Context. The DbContext simplifies database interactions, manages entities and their relationships, and ensures data consistency. <code>DbContext.cs</code></li>
  <li>Create messages and apiresponses. <code>ApiResponse.cs Messages.cs</code></li>
  <li>Create IRepositories(Interface) and Repositories for each model with async methods to abstract data access logic, promotes separation of concerns, and makes the code cleaner, more maintainable, and easier to test. <code>ITaskRepository.cs TaskRepository.cs ISubtaskRepository.cs SubtaskRepository.cs</code>
  </li>
  <li>Create Controllers for each model to handle incoming HTTP requests, process the HTTP requests, and return appropriate responses. <code>BaseController.cs TaskController.cs SubtaskController.cs</code></li>
  <li>Configure appsettings.json with the proper connection string to the proper database.</li>
  <li>Configure Program.cs to use connection string to SQL Server.</li>
  <li>Test all methods on Swagger.</li>
  <li>Clean and Rebuild
    <ul>
      <li>~<code>dotnet clean</code></li>
      <li>~<code>Remove-Item .\Migrations -Recurse -Force -ErrorAction SilentlyContinue</code></li>
      <li>~<code>dotnet ef migrations add InitialCreate --verbose</code></li>
      <li>~<code>dotnet ef database update --verbose</code></li>
    </ul>
  </li>
  <li></li>
  <li>Created frontend. Use <a href="https://github.com/TimothyJan/HoneyDoThis-frontend">frontend</a> for reference.</li>
</ul>

Frontend with Angular Material
<ul>
  <li>Create new Angular Project.</li>
  <li>Install Angular Material.</li>
  <li>Update all Angular packages to the same version, this case v20.</li>
  <li>Update Typescript to a compatible version.</li>
  <li>Create models.</li>
  <li>Create services to handle communication with database and snackbar.</li>
  <li>Configure environment files for development and production.</li>
  <li>Create components.</li>
  <li>Test all components.</li>
</ul>

Issues:
<ul>
  <li>Visual Studio 2022 - using bult in "Angular and ASP.NET Core" create a project was not compatible with Angular Version 21. Solution was to globally install older version(17) of Angular, create the "Angular and ASP.NET Core" project and then globally install Angular Version 21.</li>
  <li></li>
  <li></li>
</ul>