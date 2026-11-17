# SETUP NEW .NET WEBAPI PROJECT

### 1. Move to apps folder
```bash

cd apps

```

### 2. Create new project
```bash

dotnet new webapi --use-controllers -n folder-name

```

### 3 Move to project folder
```bash

cd folder-name

```

### 4. Install package for auth system
```bash

dotnet add package Microsoft.AspNetCore.OpenApi --version 10.0.14
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore --version 10.0.14
dotnet add package Microsoft.EntityFrameworkCore --version 10.0.14
dotnet add package Microsoft.EntityFrameworkCore.Design --version 10.0.14
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 10.0.14
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 10.0.3
dotnet add package MailKit --version 4.18.2
dotnet add package BCrypt.Net-Next --version 4.2.0

```

### 5. Install migrations tool (if not installed)
```bash

dotnet tool install --global dotnet-ef --version 10.0.14

```

### 6. Run schema database
```bash

psql -U username -f Database/db_schema.sql

```

### 7. Run dotnet project
```bash

dotnet run

```bash