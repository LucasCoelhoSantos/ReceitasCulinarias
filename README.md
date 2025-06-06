# Catálogo Interativo de Receitas Culinárias

```dotnet ef database update -s src/MeuProjeto.API```

1. ```dotnet ef migrations add InitialCreate -p Desafio.Infrastructure -s Desafio.API```

Ou no Console do Gerenciador de Pacotes, selecione MeuProjeto.Infrastructure como o "Projeto padrão". Execute o comando: ```Add-Migration InitialCreate```

2. ```dotnet ef database update -s Desafio.API```

Ou no Console do Gerenciador de Pacotes, selecione MeuProjeto.Infrastructure como o "Projeto padrão". Execute o comando: ```Update-Database```