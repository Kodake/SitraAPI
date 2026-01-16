IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    IF SCHEMA_ID(N'identity') IS NULL EXEC(N'CREATE SCHEMA [identity];');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE TABLE [identity].[AspNetRoles] (
        [Id] nvarchar(450) NOT NULL,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE TABLE [identity].[AspNetUsers] (
        [Id] nvarchar(450) NOT NULL,
        [CompanyName] nvarchar(100) NOT NULL,
        [RucNumber] nvarchar(11) NOT NULL,
        [Address] nvarchar(100) NOT NULL,
        [TenantId] nvarchar(50) NULL,
        [UserName] nvarchar(256) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [Email] nvarchar(256) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [SecurityStamp] nvarchar(max) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(10) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE TABLE [identity].[Brands] (
        [Id] bigint NOT NULL IDENTITY,
        [Name] nvarchar(50) NOT NULL,
        [ImageURL] nvarchar(max) NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Brands] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE TABLE [identity].[Categories] (
        [Id] bigint NOT NULL IDENTITY,
        [Name] nvarchar(50) NOT NULL,
        [Description] nvarchar(100) NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Categories] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE TABLE [identity].[Contacts] (
        [Id] bigint NOT NULL IDENTITY,
        [ContactType] int NULL,
        [IdentificationType] int NULL,
        [Identification] nvarchar(15) NULL,
        [Name] nvarchar(50) NOT NULL,
        [Province] nvarchar(2) NULL,
        [Address] nvarchar(100) NOT NULL,
        [Phone] nvarchar(max) NULL,
        [Phone2] nvarchar(max) NULL,
        [Email] nvarchar(max) NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Contacts] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE TABLE [identity].[Currencies] (
        [Id] bigint NOT NULL IDENTITY,
        [Code] nvarchar(20) NOT NULL,
        [Name] nvarchar(50) NOT NULL,
        [Symbol] nvarchar(3) NOT NULL,
        [Rate] decimal(18,2) NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Currencies] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE TABLE [identity].[Deadlines] (
        [Id] bigint NOT NULL IDENTITY,
        [Name] nvarchar(50) NOT NULL,
        [Days] int NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Deadlines] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE TABLE [identity].[Expenses] (
        [Id] bigint NOT NULL IDENTITY,
        [Date] datetime2 NOT NULL,
        [Description] nvarchar(50) NOT NULL,
        [Amount] decimal(18,2) NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Expenses] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE TABLE [identity].[Pricings] (
        [Id] bigint NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        [PricingType] int NOT NULL,
        [Percentage] decimal(18,2) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Pricings] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE TABLE [identity].[Taxes] (
        [Id] bigint NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        [Amount] decimal(18,2) NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Taxes] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE TABLE [identity].[Units] (
        [Id] bigint NOT NULL IDENTITY,
        [Name] nvarchar(20) NOT NULL,
        [ShortName] nvarchar(20) NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Units] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE TABLE [identity].[Warehouses] (
        [Id] bigint NOT NULL IDENTITY,
        [Name] nvarchar(50) NOT NULL,
        [Address] nvarchar(100) NOT NULL,
        [Phone] nvarchar(10) NULL,
        [Email] nvarchar(50) NULL,
        [City] nvarchar(50) NULL,
        [Country] nvarchar(2) NULL,
        [ZipCode] nvarchar(5) NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Warehouses] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE TABLE [identity].[AspNetRoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [identity].[AspNetRoles] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE TABLE [identity].[AspNetUserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [identity].[AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE TABLE [identity].[AspNetUserLogins] (
        [LoginProvider] nvarchar(450) NOT NULL,
        [ProviderKey] nvarchar(450) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [identity].[AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE TABLE [identity].[AspNetUserRoles] (
        [UserId] nvarchar(450) NOT NULL,
        [RoleId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [identity].[AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [identity].[AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE TABLE [identity].[AspNetUserTokens] (
        [UserId] nvarchar(450) NOT NULL,
        [LoginProvider] nvarchar(450) NOT NULL,
        [Name] nvarchar(450) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [identity].[AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE TABLE [identity].[PettyCashes] (
        [Id] bigint NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [OpeningAmount] decimal(18,2) NOT NULL,
        [OpeningDate] datetime2 NOT NULL,
        [ClosingAmount] decimal(18,2) NULL,
        [WithdrawalAmount] float NULL,
        [ClosingDate] datetime2 NULL,
        [Status] int NOT NULL,
        [Comments] nvarchar(100) NULL,
        CONSTRAINT [PK_PettyCashes] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PettyCashes_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [identity].[AspNetUsers] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE TABLE [identity].[Models] (
        [Id] bigint NOT NULL IDENTITY,
        [Name] nvarchar(50) NOT NULL,
        [Description] nvarchar(100) NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [BrandId] bigint NOT NULL,
        CONSTRAINT [PK_Models] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Models_Brands_BrandId] FOREIGN KEY ([BrandId]) REFERENCES [identity].[Brands] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE TABLE [identity].[SubCategories] (
        [Id] bigint NOT NULL IDENTITY,
        [Name] nvarchar(50) NOT NULL,
        [Description] nvarchar(100) NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CategoryId] bigint NOT NULL,
        CONSTRAINT [PK_SubCategories] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_SubCategories_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [identity].[Categories] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE TABLE [identity].[FiscalSupports] (
        [Id] bigint NOT NULL IDENTITY,
        [DocumentType] int NOT NULL,
        [FiscalSupportType] int NOT NULL,
        [Name] nvarchar(50) NOT NULL,
        [Symbol] nvarchar(1) NOT NULL,
        [InitialSequence] bigint NOT NULL,
        [FinalSequence] bigint NOT NULL,
        [SpaceNumber] bigint NOT NULL,
        [WarehouseId] bigint NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_FiscalSupports] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_FiscalSupports_Warehouses_WarehouseId] FOREIGN KEY ([WarehouseId]) REFERENCES [identity].[Warehouses] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE TABLE [identity].[Invoices] (
        [Id] bigint NOT NULL IDENTITY,
        [Date] datetime2 NOT NULL,
        [PricingId] bigint NOT NULL,
        [BillerId] bigint NULL,
        [CustomerId] bigint NULL,
        [FiscalSupportId] bigint NULL,
        [FiscalSupportNumber] nvarchar(max) NULL,
        [CurrencyId] bigint NOT NULL,
        [WarehouseId] bigint NOT NULL,
        [ConditionType] int NULL,
        [InvoiceType] int NOT NULL,
        [TotalQtyItems] decimal(18,2) NOT NULL,
        [TotalItems] decimal(18,2) NOT NULL,
        [TotalAmount] decimal(18,2) NOT NULL,
        [TotalPrice] decimal(18,2) NOT NULL,
        [TotalTaxes] decimal(18,2) NOT NULL,
        [Shipping] decimal(18,2) NULL,
        [Discount] decimal(18,2) NULL,
        [Description] nvarchar(max) NULL,
        [Note] nvarchar(max) NULL,
        [DueDate] datetime2 NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Invoices] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Invoices_Contacts_BillerId] FOREIGN KEY ([BillerId]) REFERENCES [identity].[Contacts] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Invoices_Contacts_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [identity].[Contacts] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Invoices_Currencies_CurrencyId] FOREIGN KEY ([CurrencyId]) REFERENCES [identity].[Currencies] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Invoices_Pricings_PricingId] FOREIGN KEY ([PricingId]) REFERENCES [identity].[Pricings] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Invoices_Warehouses_WarehouseId] FOREIGN KEY ([WarehouseId]) REFERENCES [identity].[Warehouses] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE TABLE [identity].[Items] (
        [Id] bigint NOT NULL IDENTITY,
        [ItemType] int NOT NULL,
        [Name] nvarchar(50) NOT NULL,
        [Reference] nvarchar(50) NULL,
        [Barcode] nvarchar(50) NULL,
        [Description] nvarchar(100) NULL,
        [ImageUrl] nvarchar(max) NULL,
        [BrandId] bigint NULL,
        [ModelId] bigint NULL,
        [CategoryId] bigint NULL,
        [SubCategoryId] bigint NULL,
        [UnitId] bigint NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Items] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Items_Brands_BrandId] FOREIGN KEY ([BrandId]) REFERENCES [identity].[Brands] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Items_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [identity].[Categories] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Items_Models_ModelId] FOREIGN KEY ([ModelId]) REFERENCES [identity].[Models] ([Id]),
        CONSTRAINT [FK_Items_SubCategories_SubCategoryId] FOREIGN KEY ([SubCategoryId]) REFERENCES [identity].[SubCategories] ([Id]),
        CONSTRAINT [FK_Items_Units_UnitId] FOREIGN KEY ([UnitId]) REFERENCES [identity].[Units] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE TABLE [identity].[Payables] (
        [Id] bigint NOT NULL IDENTITY,
        [InvoiceId] bigint NOT NULL,
        [ContactId] bigint NULL,
        [TotalAmountDue] decimal(18,2) NOT NULL,
        [TotalOutstanding] decimal(18,2) NOT NULL,
        [Status] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Payables] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Payables_Contacts_ContactId] FOREIGN KEY ([ContactId]) REFERENCES [identity].[Contacts] ([Id]),
        CONSTRAINT [FK_Payables_Invoices_InvoiceId] FOREIGN KEY ([InvoiceId]) REFERENCES [identity].[Invoices] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE TABLE [identity].[PettyCashMovements] (
        [Id] bigint NOT NULL IDENTITY,
        [PettyCashId] bigint NOT NULL,
        [InvoiceId] bigint NULL,
        [Amount] float NOT NULL,
        [Type] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_PettyCashMovements] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PettyCashMovements_Invoices_InvoiceId] FOREIGN KEY ([InvoiceId]) REFERENCES [identity].[Invoices] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_PettyCashMovements_PettyCashes_PettyCashId] FOREIGN KEY ([PettyCashId]) REFERENCES [identity].[PettyCashes] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE TABLE [identity].[Quotations] (
        [Id] bigint NOT NULL IDENTITY,
        [InvoiceId] bigint NULL,
        [ContactId] bigint NULL,
        [QuotationNumber] nvarchar(max) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Quotations] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Quotations_Contacts_ContactId] FOREIGN KEY ([ContactId]) REFERENCES [identity].[Contacts] ([Id]),
        CONSTRAINT [FK_Quotations_Invoices_InvoiceId] FOREIGN KEY ([InvoiceId]) REFERENCES [identity].[Invoices] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE TABLE [identity].[Receivables] (
        [Id] bigint NOT NULL IDENTITY,
        [InvoiceId] bigint NULL,
        [ContactId] bigint NULL,
        [TotalAmountDue] decimal(18,2) NOT NULL,
        [TotalOutstanding] decimal(18,2) NOT NULL,
        [Status] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Receivables] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Receivables_Contacts_ContactId] FOREIGN KEY ([ContactId]) REFERENCES [identity].[Contacts] ([Id]),
        CONSTRAINT [FK_Receivables_Invoices_InvoiceId] FOREIGN KEY ([InvoiceId]) REFERENCES [identity].[Invoices] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE TABLE [identity].[SpotPayables] (
        [Id] bigint NOT NULL IDENTITY,
        [InvoiceId] bigint NULL,
        [ContactId] bigint NULL,
        [PaymentDate] datetime2 NOT NULL,
        [PaymentMethod] int NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_SpotPayables] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_SpotPayables_Contacts_ContactId] FOREIGN KEY ([ContactId]) REFERENCES [identity].[Contacts] ([Id]),
        CONSTRAINT [FK_SpotPayables_Invoices_InvoiceId] FOREIGN KEY ([InvoiceId]) REFERENCES [identity].[Invoices] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE TABLE [identity].[SpotPayments] (
        [Id] bigint NOT NULL IDENTITY,
        [InvoiceId] bigint NOT NULL,
        [ContactId] bigint NULL,
        [AmountReturned] decimal(18,2) NOT NULL,
        [PaymentDate] datetime2 NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_SpotPayments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_SpotPayments_Contacts_ContactId] FOREIGN KEY ([ContactId]) REFERENCES [identity].[Contacts] ([Id]),
        CONSTRAINT [FK_SpotPayments_Invoices_InvoiceId] FOREIGN KEY ([InvoiceId]) REFERENCES [identity].[Invoices] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE TABLE [identity].[ComboItems] (
        [ParentItemId] bigint NOT NULL,
        [ChildItemId] bigint NOT NULL,
        [Qty] decimal(18,2) NOT NULL,
        CONSTRAINT [PK_ComboItems] PRIMARY KEY ([ParentItemId], [ChildItemId]),
        CONSTRAINT [FK_ComboItems_Items_ChildItemId] FOREIGN KEY ([ChildItemId]) REFERENCES [identity].[Items] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ComboItems_Items_ParentItemId] FOREIGN KEY ([ParentItemId]) REFERENCES [identity].[Items] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE TABLE [identity].[ItemTaxes] (
        [ItemId] bigint NOT NULL,
        [TaxId] bigint NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_ItemTaxes] PRIMARY KEY ([TaxId], [ItemId]),
        CONSTRAINT [FK_ItemTaxes_Items_ItemId] FOREIGN KEY ([ItemId]) REFERENCES [identity].[Items] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ItemTaxes_Taxes_TaxId] FOREIGN KEY ([TaxId]) REFERENCES [identity].[Taxes] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE TABLE [identity].[PricingItems] (
        [Id] bigint NOT NULL IDENTITY,
        [ItemId] bigint NOT NULL,
        [PricingId] bigint NOT NULL,
        [InitialCost] decimal(18,2) NOT NULL,
        [BasePrice] decimal(18,2) NOT NULL,
        [IsPrincipal] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_PricingItems] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PricingItems_Items_ItemId] FOREIGN KEY ([ItemId]) REFERENCES [identity].[Items] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_PricingItems_Pricings_PricingId] FOREIGN KEY ([PricingId]) REFERENCES [identity].[Pricings] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE TABLE [identity].[Stocks] (
        [Id] bigint NOT NULL IDENTITY,
        [ItemId] bigint NOT NULL,
        [WarehouseId] bigint NOT NULL,
        [Qty] decimal(18,2) NOT NULL,
        [MinQty] decimal(18,2) NULL,
        [MaxQty] decimal(18,2) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Stocks] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Stocks_Items_ItemId] FOREIGN KEY ([ItemId]) REFERENCES [identity].[Items] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Stocks_Warehouses_WarehouseId] FOREIGN KEY ([WarehouseId]) REFERENCES [identity].[Warehouses] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE TABLE [identity].[PayableReceipts] (
        [Id] bigint NOT NULL IDENTITY,
        [PayableId] bigint NOT NULL,
        [Amount] decimal(18,2) NOT NULL,
        [PaymentDate] datetime2 NOT NULL,
        [PaymentMethod] int NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_PayableReceipts] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PayableReceipts_Payables_PayableId] FOREIGN KEY ([PayableId]) REFERENCES [identity].[Payables] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE TABLE [identity].[InvoiceItems] (
        [Id] bigint NOT NULL IDENTITY,
        [InvoiceId] bigint NOT NULL,
        [ItemId] bigint NOT NULL,
        [Qty] decimal(18,2) NOT NULL,
        [BasePrice] decimal(18,2) NOT NULL,
        [TotalAmount] decimal(18,2) NOT NULL,
        [TotalPrice] decimal(18,2) NOT NULL,
        [TotalTaxes] decimal(18,2) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [QuotationId] bigint NULL,
        CONSTRAINT [PK_InvoiceItems] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_InvoiceItems_Invoices_InvoiceId] FOREIGN KEY ([InvoiceId]) REFERENCES [identity].[Invoices] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_InvoiceItems_Items_ItemId] FOREIGN KEY ([ItemId]) REFERENCES [identity].[Items] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_InvoiceItems_Quotations_QuotationId] FOREIGN KEY ([QuotationId]) REFERENCES [identity].[Quotations] ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE TABLE [identity].[PaymentReceipts] (
        [Id] bigint NOT NULL IDENTITY,
        [ReceivableId] bigint NOT NULL,
        [Amount] decimal(18,2) NOT NULL,
        [PaymentDate] datetime2 NOT NULL,
        [PaymentMethod] int NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_PaymentReceipts] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PaymentReceipts_Receivables_ReceivableId] FOREIGN KEY ([ReceivableId]) REFERENCES [identity].[Receivables] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE TABLE [identity].[Payments] (
        [Id] bigint NOT NULL IDENTITY,
        [SpotPaymentId] bigint NOT NULL,
        [PaymentMethod] int NOT NULL,
        [Amount] decimal(18,2) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Payments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Payments_SpotPayments_SpotPaymentId] FOREIGN KEY ([SpotPaymentId]) REFERENCES [identity].[SpotPayments] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE TABLE [identity].[Transfers] (
        [Id] bigint NOT NULL IDENTITY,
        [ProductId] bigint NOT NULL,
        [OriginWarehouseId] bigint NOT NULL,
        [DestinationWarehouseId] bigint NOT NULL,
        [Qty] decimal(18,2) NOT NULL,
        [StockId] bigint NOT NULL,
        [Comment] nvarchar(max) NOT NULL,
        [Status] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Transfers] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Transfers_Items_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [identity].[Items] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Transfers_Stocks_StockId] FOREIGN KEY ([StockId]) REFERENCES [identity].[Stocks] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Transfers_Warehouses_DestinationWarehouseId] FOREIGN KEY ([DestinationWarehouseId]) REFERENCES [identity].[Warehouses] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Transfers_Warehouses_OriginWarehouseId] FOREIGN KEY ([OriginWarehouseId]) REFERENCES [identity].[Warehouses] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [identity].[AspNetRoleClaims] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [RoleNameIndex] ON [identity].[AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [identity].[AspNetUserClaims] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [identity].[AspNetUserLogins] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [identity].[AspNetUserRoles] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE INDEX [EmailIndex] ON [identity].[AspNetUsers] ([NormalizedEmail]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [identity].[AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE INDEX [IX_ComboItems_ChildItemId] ON [identity].[ComboItems] ([ChildItemId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE INDEX [IX_FiscalSupports_WarehouseId] ON [identity].[FiscalSupports] ([WarehouseId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE INDEX [IX_InvoiceItems_InvoiceId] ON [identity].[InvoiceItems] ([InvoiceId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE INDEX [IX_InvoiceItems_ItemId] ON [identity].[InvoiceItems] ([ItemId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE INDEX [IX_InvoiceItems_QuotationId] ON [identity].[InvoiceItems] ([QuotationId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE INDEX [IX_Invoices_BillerId] ON [identity].[Invoices] ([BillerId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE INDEX [IX_Invoices_CurrencyId] ON [identity].[Invoices] ([CurrencyId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE INDEX [IX_Invoices_CustomerId] ON [identity].[Invoices] ([CustomerId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE INDEX [IX_Invoices_PricingId] ON [identity].[Invoices] ([PricingId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE INDEX [IX_Invoices_WarehouseId] ON [identity].[Invoices] ([WarehouseId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE INDEX [IX_Items_BrandId] ON [identity].[Items] ([BrandId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE INDEX [IX_Items_CategoryId] ON [identity].[Items] ([CategoryId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE INDEX [IX_Items_ModelId] ON [identity].[Items] ([ModelId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE INDEX [IX_Items_SubCategoryId] ON [identity].[Items] ([SubCategoryId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE INDEX [IX_Items_UnitId] ON [identity].[Items] ([UnitId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE INDEX [IX_ItemTaxes_ItemId] ON [identity].[ItemTaxes] ([ItemId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE INDEX [IX_Models_BrandId] ON [identity].[Models] ([BrandId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE INDEX [IX_PayableReceipts_PayableId] ON [identity].[PayableReceipts] ([PayableId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE INDEX [IX_Payables_ContactId] ON [identity].[Payables] ([ContactId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Payables_InvoiceId] ON [identity].[Payables] ([InvoiceId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE INDEX [IX_PaymentReceipts_ReceivableId] ON [identity].[PaymentReceipts] ([ReceivableId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE INDEX [IX_Payments_SpotPaymentId] ON [identity].[Payments] ([SpotPaymentId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE INDEX [IX_PettyCashes_UserId] ON [identity].[PettyCashes] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE INDEX [IX_PettyCashMovements_InvoiceId] ON [identity].[PettyCashMovements] ([InvoiceId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE INDEX [IX_PettyCashMovements_PettyCashId] ON [identity].[PettyCashMovements] ([PettyCashId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE INDEX [IX_PricingItems_ItemId] ON [identity].[PricingItems] ([ItemId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE INDEX [IX_PricingItems_PricingId] ON [identity].[PricingItems] ([PricingId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE INDEX [IX_Quotations_ContactId] ON [identity].[Quotations] ([ContactId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Quotations_InvoiceId] ON [identity].[Quotations] ([InvoiceId]) WHERE [InvoiceId] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE INDEX [IX_Receivables_ContactId] ON [identity].[Receivables] ([ContactId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Receivables_InvoiceId] ON [identity].[Receivables] ([InvoiceId]) WHERE [InvoiceId] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE INDEX [IX_SpotPayables_ContactId] ON [identity].[SpotPayables] ([ContactId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_SpotPayables_InvoiceId] ON [identity].[SpotPayables] ([InvoiceId]) WHERE [InvoiceId] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE INDEX [IX_SpotPayments_ContactId] ON [identity].[SpotPayments] ([ContactId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_SpotPayments_InvoiceId] ON [identity].[SpotPayments] ([InvoiceId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE INDEX [IX_Stocks_ItemId] ON [identity].[Stocks] ([ItemId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE INDEX [IX_Stocks_WarehouseId] ON [identity].[Stocks] ([WarehouseId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE INDEX [IX_SubCategories_CategoryId] ON [identity].[SubCategories] ([CategoryId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE INDEX [IX_Transfers_DestinationWarehouseId] ON [identity].[Transfers] ([DestinationWarehouseId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE INDEX [IX_Transfers_OriginWarehouseId] ON [identity].[Transfers] ([OriginWarehouseId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE INDEX [IX_Transfers_ProductId] ON [identity].[Transfers] ([ProductId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    CREATE INDEX [IX_Transfers_StockId] ON [identity].[Transfers] ([StockId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251226185352_Initial'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251226185352_Initial', N'8.0.14');
END;
GO

COMMIT;
GO

