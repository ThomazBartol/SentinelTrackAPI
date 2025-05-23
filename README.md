# SentinelTrack API

**SentinelTrack** é uma API REST desenvolvida em .NET 8 com Entity Framework Core e banco de dados Oracle, criada para gerenciar o fluxo de **motos** em **pátios**. O projeto simula um sistema de controle de operações para uma empresa de aluguel de motos, com regras de capacidade, alocação e gerenciamento de veículos.

A aplicação oferece endpoints organizados para cadastro, listagem, atualização e exclusão de pátios e motos, incluindo validações e filtros. O projeto foi desenvolvido como parte de um desafio técnico proposto pela **Mottu**.

## Rotas Disponíveis

---

### Pátios (`/api/yards`)

- **GET /api/yards** — Lista os pátios, com filtros opcionais via query params:

  | Query Param  | Tipo    | Descrição                                    | Exemplo         |
  |--------------|---------|----------------------------------------------|-----------------|
  | capacityMin  | integer | Filtra pátios com capacidade mínima          | `/api/yards?capacityMin=10` |
  | capacityMax  | integer | Filtra pátios com capacidade máxima           | `/api/yards?capacityMax=50` |
  | hasSpace     | boolean | Se true, filtra pátios que ainda têm espaço disponível | `/api/yards?hasSpace=true`  |

- **GET /api/yards/{id}** — Busca pátio pelo ID.

- **POST /api/yards** — Cria um novo pátio.

- **PUT /api/yards/{id}** — Atualiza um pátio existente.

- **DELETE /api/yards/{id}** — Remove um pátio.

---

### Motos (`/api/motos`)

- **GET /api/motos** — Lista motos, com filtros opcionais via query params:

  | Query Param | Tipo    | Descrição                        | Exemplo               |
  |-------------|---------|----------------------------------|-----------------------|
  | yardId      | GUID    | Filtra motos pelo ID do pátio     | `/api/motos?yardId=abc123` |
  | color       | string  | Filtra motos pela cor             | `/api/motos?color=red`       |
  | model       | string  | Filtra motos pelo modelo          | `/api/motos?model=Honda`     |

- **GET /api/motos/{id}** — Busca moto pelo ID.

- **POST /api/motos** — Cria uma nova moto.

- **PUT /api/motos/{id}** — Atualiza uma moto existente.

- **DELETE /api/motos/{id}** — Remove uma moto.

---

## Instruções de Execução

1. Clone o repositório:
   ```bash
   git clone https://github.com/ThomazBartol/SentinelTrackAPI.git
   cd SentinelTrackAPI/

2. Crie dentro da pasta SentinelTrack (no mesmo diretório que o .csproj):
    arquivo .env contendo:
   ```bash
   ORACLE_CONNECTION_STRING=User Id={usuário};Password={senha};Data Source=oracle.fiap.com.br:1521/ORCL

4. Rode o projeto com o comando:
   ```bash
   dotnet run

5. Caso o Swagger não abra sozinho acesse em:
   https://localhost:7170/swagger/index.html

## 👥 INTEGRANTES DO GRUPO
===========================

- RM555323 - Thomaz Oliveira Vilas Boas Bartol
- RM556089 - Vinicius Souza Carvalho
- RM556972 - Gabriel Duarte Pinto
