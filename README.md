# Helpdesk System

## Description

A robust helpdesk system designed to manage support tickets, user, and support interactions efficiently. This system features role-based access control and ticket management. The system was designed using hexagonal architecture for easy management.

<br>

This project also uses unit tests to check that all features are working correctly and includes a simple cache implementation to improve performance.

## Installation

To run this project, you need to install <b>.NET 6</b> on your machine. Also, don't forget to install the dependencies in each project. You can use the command below to install the dependencies:

```
dotnet restore
```

After that, you need to configure your SQL Server database and set up the connection string in <b>"/HelpDeskService/Consumers/Api/appsettings.json"</b>. Additionally, run the SQL commands below to initialize the system base roles:

```
INSERT INTO [AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES 
(1, 'Client','Client','Client')

INSERT INTO [AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES 
(2, 'Support','Support','Support')
```

Finally, you need to run the application and auth migrations. First, navigate to the directory <b>"/HelpDeskService/Adapters/DataEF"</b> and run:

```
dotnet ef database update --project DataEF.csproj --startup-project "../../Consumers/Api/Api.csproj" --context DataEF.AppDbContext
```

Then, run the auth migrations. Navigate to <b>"/HelpDeskService/Adapters/IdentityAuth"</b> and run:

```
dotnet ef database update --project IdentityAuth.csproj --startup-project "../../Consumers/Api/Api.csproj" --context IdentityAuth.AuthDbContext
```

## Conventional Commits

| Type     | Emoji                 | Code                    |
|:---------|:----------------------|:------------------------|
| feat     | :sparkles:            | `:sparkles:`            |
| fix      | :bug:                 | `:bug:`                 |
| docs     | :books:               | `:books:`               |
| style    | :gem:                 | `:gem:`                 |
| refactor | :hammer:              | `:hammer:`              |
| perf     | :rocket:              | `:rocket:`              |
| test     | :rotating_light:      | `:rotating_light:`      |
| build    | :package:             | `:package:`             |
| ci       | :construction_worker: | `:construction_worker:` |
| chore    | :wrench:              | `:wrench:`              |
