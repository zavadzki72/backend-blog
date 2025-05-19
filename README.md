# Blog API Documentation

API backend para sistema de blog desenvolvido em .NET 9.0.

Base URL: `http://backend-blog-8uqk.onrender.com`

## Autenticação

Todos os endpoints (exceto onde indicado) requerem autenticação via token JWT no header:
```
Authorization: Bearer {seu-token}
```

## Endpoints

### User

#### Login - Obter Token
```http
POST /api/User/get-token
```
**Não requer autenticação**

**Request Body:**
```json
{
    "email": "string",     // Required
    "password": "string"   // Required
}
```

**Response:** `200 OK`
```json
{
    "data": "string"  // JWT Token
}
```

#### Listar Usuários
```http
GET /api/User
```

**Response:** `200 OK`
```json
{
    "data": [
        {
            "id": "uuid",
            "name": "string",
            "email": "string",
            "description": "string",
            "siteUrl": "string",
            "pictureUrl": "string"
        }
    ]
}
```

#### Atualizar Usuário
```http
PUT /api/User/{id}
```

**Path Parameters:**
- `id` (UUID, required)

**Request Body:**
```json
{
    "name": "string",          // Required
    "description": "string",   // Required
    "siteUrl": "string",      // Optional
    "pictureUrl": "string"    // Optional
}
```

**Response:** `204 No Content`

### Post

#### Listar Posts (Paginado)
```http
GET /api/Post
```

**Request Body:**
```json
{
    "page": 0,                    // Required, int32
    "size": 0,                    // Required, int32
    "minCreatedAt": "datetime",   // Optional
    "maxCreatedAt": "datetime",   // Optional
    "titles": ["string"],         // Optional
    "usersId": ["uuid"],         // Optional
    "categories": ["uuid"],      // Optional
    "tags": ["string"],          // Optional
    "orderType": 0               // Optional, enum
}
```

**Response:** `200 OK`
```json
{
    "data": {
        "items": [],
        "total": 0,
        "page": 0,
        "size": 0
    }
}
```

#### Criar Post
```http
POST /api/Post
```

**Request Body:**
```json
{
    "title": "string",           // Required
    "subTitle": "string",        // Required
    "content": "string",         // Required
    "coverImageUrl": "string",   // Required
    "categories": ["string"],    // Optional
    "tags": ["string"]          // Optional
}
```

**Response:** `201 Created`

#### Atualizar Post
```http
PUT /api/Post/{id}
```

**Path Parameters:**
- `id` (UUID, required)

**Request Body:**
```json
{
    "title": "string",           // Required
    "subTitle": "string",        // Required
    "content": "string",         // Required
    "coverImageUrl": "string",   // Required
    "categories": ["string"],    // Optional
    "tags": ["string"]          // Optional
}
```

**Response:** `204 No Content`

#### Excluir Post
```http
DELETE /api/Post/{id}
```

**Path Parameters:**
- `id` (UUID, required)

**Response:** `204 No Content`

#### Arquivar Post
```http
PATCH /archive
```

**Query Parameters:**
- `id` (UUID, required)

**Response:** `204 No Content`

#### Reativar Post
```http
PATCH /reactivate
```

**Query Parameters:**
- `id` (UUID, required)

**Response:** `204 No Content`

#### Votar em Post
```http
PATCH /up-vote
```

**Query Parameters:**
- `id` (UUID, required)

**Response:** `204 No Content`

#### Registrar Visualização
```http
PATCH /view
```

**Query Parameters:**
- `id` (UUID, required)

**Response:** `204 No Content`

### Category

#### Listar Categorias
```http
GET /api/Category
```

**Response:** `200 OK`
```json
{
    "data": [
        {
            "id": "uuid",
            "createdAt": "datetime",
            "updatedAt": "datetime",
            "name": "string"
        }
    ]
}
```

#### Criar Categoria
```http
POST /api/Category
```

**Request Body:**
```json
{
    "name": "string"
}
```

**Response:** `201 Created`

#### Atualizar Categoria
```http
PUT /api/Category/{id}
```

**Path Parameters:**
- `id` (UUID, required)

**Request Body:**
```json
{
    "name": "string"
}
```

**Response:** `204 No Content`

#### Excluir Categoria
```http
DELETE /api/Category/{id}
```

**Path Parameters:**
- `id` (UUID, required)

**Response:** `204 No Content`

### File

#### Upload de Imagem do Post
```http
POST /api/File/upload/post-files
```

**Query Parameters:**
- `postId` (UUID, required)

**Request Body:**
- Content-Type: `multipart/form-data`
- `file`: Binary (Required)

**Response:** `200 OK`
```json
{
    "data": "string"  // URL da imagem
}
```

#### Upload de Imagem do Usuário
```http
POST /api/File/upload/user-files
```

**Request Body:**
- Content-Type: `multipart/form-data`
- `file`: Binary (Required)

**Response:** `200 OK`
```json
{
    "data": "string"  // URL da imagem
}
```

#### Gerar URL de Acesso
```http
GET /api/File/access-url
```

**Query Parameters:**
- `key` (string, required)

**Response:** `200 OK`
```json
{
    "data": "string"  // URL pré-assinada
}
```

## Tipos de Dados

### UUID
- Formato: `xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx`
- Exemplo: `"123e4567-e89b-12d3-a456-426614174000"`

### DateTime
- Formato ISO 8601: `YYYY-MM-DDTHH:mm:ss.sssZ`
- Exemplo: `"2024-03-20T15:30:00.000Z"`

## Códigos de Status

- `200 OK` - Requisição bem-sucedida
- `201 Created` - Recurso criado com sucesso
- `204 No Content` - Requisição bem-sucedida sem conteúdo de retorno
- `400 Bad Request` - Erro de validação
- `401 Unauthorized` - Token ausente ou inválido
- `403 Forbidden` - Sem permissão para acessar o recurso
- `404 Not Found` - Recurso não encontrado
- `500 Internal Server Error` - Erro interno do servidor

## Tecnologias

```xml
<PackageReference Include="Minio" Version="4.0.7" />
<PackageReference Include="BCrypt.Net-Next" Version="4.0.3" />
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="9.0.5" />
<PackageReference Include="MongoDB.Driver" Version="3.4.0" />
<PackageReference Include="Swashbuckle.AspNetCore.SwaggerUI" Version="8.1.1" />
```
