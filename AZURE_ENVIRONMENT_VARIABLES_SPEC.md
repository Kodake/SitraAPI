# Especificación de Variables de Entorno y Secretos - Backend SITRA.API

Este documento consolida las cadenas de conexión, variables de aplicación y secretos estrictamente requeridos para el despliegue del backend **SITRA.API** en Azure (App Service / Key Vault / Contenedores).

---

## 1. Cadenas de Conexión (Connection Strings)

En Azure App Service deben ubicarse bajo **Configuration > Connection strings**:

| Nombre | Tipo en Azure | Es Secreto | Descripción / Uso en SITRA.API |
| :--- | :---: | :---: | :--- |
| **`DefaultConnection`** | `SQLAzure` | **SÍ** | Conexión a la base de datos Azure SQL / SQL Server principal. Utilizada por Entity Framework Core (`ApplicationDbContext`). |
| **`AzureStorage`** | `Custom` | **SÍ** | Connection String del Storage Account (`DefaultEndpointsProtocol=https;AccountName=...`). Almacena certificados `.p12`, esquemas XSD y XMLs de e-CF (DGII). |

> **Nota para Arquitectura (.NET):**  
> Azure App Service inyecta estas cadenas con los prefijos `SQLAZURECONNSTR_DefaultConnection` y `CUSTOMCONNSTR_AzureStorage`, las cuales el proveedor de configuración de .NET resuelve automáticamente mediante `Configuration.GetConnectionString("DefaultConnection")` y `Configuration.GetConnectionString("AzureStorage")`.

---

## 2. Variables de Aplicación y Secretos (Application Settings)

En Azure App Service se configuran en **Configuration > Application settings** o referenciadas desde **Azure Key Vault**:

### 2.1. Autenticación y Firma JWT

| Variable | Es Secreto | Formato / Ejemplo | Descripción |
| :--- | :---: | :--- | :--- |
| **`Authentication:Schemes:Bearer:SigningKeys:0:Id`** | No | `50d634f9` | Identificador único (`KeyId`) de la clave de firma JWT activa. |
| **`Authentication:Schemes:Bearer:SigningKeys:0:Value`** | **SÍ** | *Cadena Base64 de 32+ bytes* | **Clave principal activa** para firmar y validar tokens JWT. El código la decodifica desde Base64. |
| **`Jwt__Key`** *(o `Jwt:Key`)* | **SÍ** | *Texto secreto aleatorio* | **Clave de respaldo JWT**. Utilizada como fallback si la sección de `SigningKeys` está ausente. |

### 2.2. Facturación Electrónica DGII (Certificados Digitales)

| Variable | Es Secreto | Propósito |
| :--- | :---: | :--- |
| **`DgiiTest__CertificatePassword`** | **SÍ** | Contraseña para desencriptar el certificado digital `.p12` en el entorno de **Certificación / Pruebas DGII** (`DgiiTest`). |
| **`Dgii__CertificatePassword`** | **SÍ** | Contraseña para desencriptar el certificado digital `.p12` en el entorno de **Producción DGII** (`Dgii`). |

---

## 3. Variables de Plataforma de Azure (App Service)

| Variable | Valor | Relevancia |
| :--- | :---: | :--- |
| **`WEBSITE_LOAD_USER_PROFILE`** | `1` | **CRÍTICA:** Requerida en Windows App Service para que la API criptográfica de Windows permita cargar claves privadas de certificados (`X509Certificate2`) en memoria para el firmado digital de e-CF. |
| **`APPLICATIONINSIGHTS_CONNECTION_STRING`** | `InstrumentationKey=...;IngestionEndpoint=...` | Cadena de ingesta para monitoreo y logs con Azure Application Insights. |
| **`APPINSIGHTS_INSTRUMENTATIONKEY`** | *GUID* | Clave de instrumentación para el agente de telemetría APM. |

---

## 4. Configuraciones Adicionales del Proyecto a Considerar

| Variable | Es Secreto | Ejemplo / Recomendación | Descripción |
| :--- | :---: | :--- | :--- |
| **`KeyVault:Url`** | No | `https://<keyvault-name>.vault.azure.net/` | Si se proporciona en producción, la aplicación cargará los secretos automáticamente vía `DefaultAzureCredential` (Managed Identity). |
| **`SyncJob:ApiKey`** | **SÍ** | *Clave secreta* | Token compartido requerido para autorizar las solicitudes que realiza el worker en segundo plano `SITRA.SyncJob`. |
| **`Smtp:Host`** | No | `smtp.office365.com` / `smtp.sendgrid.net` | Host del servidor de correo. |
| **`Smtp:Port`** | No | `587` | Puerto SMTP. |
| **`Smtp:Username`** | **SÍ** | *usuario/correo* | Credencial de autenticación para correos. |
| **`Smtp:Password`** | **SÍ** | *contraseña* | Credencial de autenticación para correos. |

---

## 5. Limpieza y Variables a Descartar (No utilizadas)

Las siguientes variables presentes en el App Service actual son obsoletas o generadas por asistentes de Visual Studio y **no son consumidas por el código**:

* ❌ `AZURE_SQL_CONNECTIONSTRING` *(SITRA.API busca `DefaultConnection`)*
* ❌ `DatabaseConnection` *(Innecesaria, busca `DefaultConnection`)*
* ❌ `DefaultConnection1` / `AZURE_SQL_CONNECTIONSTRINGSDEFAULTCONNECTION1_*` *(Base de datos antigua/backup)*
* ❌ `ConnectionStrings__AzureStorage1` y sub-claves (`__blob`, `__file`, etc.) *(El código busca `AzureStorage`)*
* ❌ `WEBSITE_NODE_DEFAULT_VERSION` *(Innecesaria para .NET)*
* ❌ `Authentication:Schemes:Bearer:SigningKeys:0:Issuer` y `Length` *(Ignoradas por el parser)*

---

## 6. Plantillas JSON para Importación Rápida en Azure Portal (Advanced Edit)

El portal de Azure permite pegar configuraciones masivamente en formato JSON mediante la opción **Advanced Edit** (dentro de *Configuration* o *Environment Variables*).

---

### 6.1. JSON: Cadenas de Conexión (Connection Strings)
> **Dónde pegar:** Azure App Service > *Configuration* > pestaña *Connection strings* > clic en **Advanced edit**.

```json
[
  {
    "name": "DefaultConnection",
    "value": "Server=tcp:backendposapidbserver.database.windows.net;Authentication=Active Directory Default;Database=BackEnd_db;",
    "type": "SQLAzure",
    "slotSetting": true
  },
  {
    "name": "AzureStorage",
    "value": "DefaultEndpointsProtocol=https;AccountName=posappstorage;AccountKey=0u5MWj5+ytHskSW+LDR5XZPzWQCkbapuKIHy/q6FvaUASkrNbWCSvaUxGkidQADG6o5q3OZF3diV+AStvwxcpg==;EndpointSuffix=core.windows.net",
    "type": "Custom",
    "slotSetting": true
  }
]
```

---

### 6.2. JSON: Autenticación y Tokens JWT (Application Settings)

```json
[
  {
    "name": "Authentication:Schemes:Bearer:SigningKeys:0:Id",
    "value": "50d634f9",
    "slotSetting": false
  },
  {
    "name": "Authentication:Schemes:Bearer:SigningKeys:0:Value",
    "value": "MKPeu+ecbG4NWowB4OKusyrnXn/ZhbdnQDQPMePRYJ4=",
    "slotSetting": false
  },
  {
    "name": "Jwt__Key",
    "value": "VTFgdYMRXU80o0CZ7iIPXk3qUS/vsHscVynznMGq/4sOHGAKxMZ+P4CknN5so1KR",
    "slotSetting": false
  }
]
```

---

### 6.3. JSON: Facturación Electrónica DGII (Application Settings)

```json
[
  {
    "name": "DgiiTest__CertificatePassword",
    "value": "Silex450",
    "slotSetting": true
  },
  {
    "name": "Dgii__CertificatePassword",
    "value": "Silex450",
    "slotSetting": true
  }
]
```

---

### 6.4. JSON: Plataforma y Telemetría Azure (Application Settings)

```json
[
  {
    "name": "WEBSITE_LOAD_USER_PROFILE",
    "value": "1",
    "slotSetting": false
  },
  {
    "name": "APPLICATIONINSIGHTS_CONNECTION_STRING",
    "value": "InstrumentationKey=8f059b29-1d58-4c01-af23-f76f21dae801;IngestionEndpoint=https://centralus-2.in.applicationinsights.azure.com/;LiveEndpoint=https://centralus.livediagnostics.monitor.azure.com/;ApplicationId=022c13f3-93bc-4bfc-a74b-3c0a4f509fe1",
    "slotSetting": false
  },
  {
    "name": "APPINSIGHTS_INSTRUMENTATIONKEY",
    "value": "8f059b29-1d58-4c01-af23-f76f21dae801",
    "slotSetting": true
  }
]
```

---

### 6.5. JSON: Tareas en Segundo Plano y Servicios (Application Settings)

```json
[
  {
    "name": "SyncJob:ApiKey",
    "value": "sitra-dgii-sync-secret-key-2026",
    "slotSetting": false
  },
  {
    "name": "KeyVault:Url",
    "value": "",
    "slotSetting": false
  },
  {
    "name": "Smtp:Host",
    "value": "",
    "slotSetting": false
  },
  {
    "name": "Smtp:Port",
    "value": "587",
    "slotSetting": false
  },
  {
    "name": "Smtp:Username",
    "value": "",
    "slotSetting": false
  },
  {
    "name": "Smtp:Password",
    "value": "",
    "slotSetting": false
  }
]
```

---

### 6.6. JSON Consolidado Completo (Todo en Uno para Application Settings)
> **Dónde pegar:** Azure App Service > *Configuration* > pestaña *Application settings* (o *Environment variables*) > clic en **Advanced edit**. Pega este bloque completo:

```json
[
  {
    "name": "Authentication:Schemes:Bearer:SigningKeys:0:Id",
    "value": "50d634f9",
    "slotSetting": false
  },
  {
    "name": "Authentication:Schemes:Bearer:SigningKeys:0:Value",
    "value": "MKPeu+ecbG4NWowB4OKusyrnXn/ZhbdnQDQPMePRYJ4=",
    "slotSetting": false
  },
  {
    "name": "Jwt__Key",
    "value": "VTFgdYMRXU80o0CZ7iIPXk3qUS/vsHscVynznMGq/4sOHGAKxMZ+P4CknN5so1KR",
    "slotSetting": false
  },
  {
    "name": "DgiiTest__CertificatePassword",
    "value": "Silex450",
    "slotSetting": true
  },
  {
    "name": "Dgii__CertificatePassword",
    "value": "Silex450",
    "slotSetting": true
  },
  {
    "name": "WEBSITE_LOAD_USER_PROFILE",
    "value": "1",
    "slotSetting": false
  },
  {
    "name": "APPLICATIONINSIGHTS_CONNECTION_STRING",
    "value": "InstrumentationKey=8f059b29-1d58-4c01-af23-f76f21dae801;IngestionEndpoint=https://centralus-2.in.applicationinsights.azure.com/;LiveEndpoint=https://centralus.livediagnostics.monitor.azure.com/;ApplicationId=022c13f3-93bc-4bfc-a74b-3c0a4f509fe1",
    "slotSetting": false
  },
  {
    "name": "APPINSIGHTS_INSTRUMENTATIONKEY",
    "value": "8f059b29-1d58-4c01-af23-f76f21dae801",
    "slotSetting": true
  },
  {
    "name": "SyncJob:ApiKey",
    "value": "sitra-dgii-sync-secret-key-2026",
    "slotSetting": false
  },
  {
    "name": "KeyVault:Url",
    "value": "",
    "slotSetting": false
  },
  {
    "name": "Smtp:Host",
    "value": "",
    "slotSetting": false
  },
  {
    "name": "Smtp:Port",
    "value": "587",
    "slotSetting": false
  },
  {
    "name": "Smtp:Username",
    "value": "",
    "slotSetting": false
  },
  {
    "name": "Smtp:Password",
    "value": "",
    "slotSetting": false
  }
]
```
