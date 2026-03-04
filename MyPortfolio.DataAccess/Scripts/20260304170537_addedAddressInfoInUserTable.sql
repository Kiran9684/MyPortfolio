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
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219143543_InitialCreate'
)
BEGIN
    CREATE TABLE [user] (
        [Id] int NOT NULL IDENTITY,
        [FirstName] varchar(100) NOT NULL,
        [LastName] varchar(100) NOT NULL,
        [Email] varchar(255) NOT NULL,
        [PasswordHash] varchar(500) NOT NULL,
        [PasswordSalt] varchar(500) NOT NULL,
        [CreatedDate] datetime NOT NULL DEFAULT (GETDATE()),
        [LastLoginDate] datetime NULL,
        [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
        [Role] int NULL,
        [ProfilePictureUrl] varchar(500) NULL,
        CONSTRAINT [PK_user] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_user_Email] UNIQUE ([Email])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219143543_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251219143543_InitialCreate', N'9.0.11');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251229153404_userTableSchemaChange'
)
BEGIN
    ALTER TABLE [user] DROP CONSTRAINT [AK_user_Email];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251229153404_userTableSchemaChange'
)
BEGIN
    CREATE UNIQUE INDEX [IX_user_Email] ON [user] ([Email]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251229153404_userTableSchemaChange'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251229153404_userTableSchemaChange', N'9.0.11');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304170537_addedAddressInfoInUserTable'
)
BEGIN
    ALTER TABLE [user] ADD [Address1] varchar(255) NOT NULL DEFAULT '';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304170537_addedAddressInfoInUserTable'
)
BEGIN
    ALTER TABLE [user] ADD [Address2] varchar(255) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304170537_addedAddressInfoInUserTable'
)
BEGIN
    ALTER TABLE [user] ADD [City] varchar(100) NOT NULL DEFAULT '';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304170537_addedAddressInfoInUserTable'
)
BEGIN
    ALTER TABLE [user] ADD [State] varchar(100) NOT NULL DEFAULT '';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304170537_addedAddressInfoInUserTable'
)
BEGIN
    ALTER TABLE [user] ADD [ZipCode] varchar(20) NOT NULL DEFAULT '';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304170537_addedAddressInfoInUserTable'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260304170537_addedAddressInfoInUserTable', N'9.0.11');
END;

COMMIT;
GO

