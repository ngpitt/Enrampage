
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 02/09/2016 22:51:50
-- Generated from EDMX file: C:\Users\ngpitt\Documents\GitHub\Enrampage\Enrampage\Models\EnrampageModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [Enrampage-Dev];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_RantTag_Rants]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RantTag] DROP CONSTRAINT [FK_RantTag_Rants];
GO
IF OBJECT_ID(N'[dbo].[FK_RantTag_Tags]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RantTag] DROP CONSTRAINT [FK_RantTag_Tags];
GO
IF OBJECT_ID(N'[dbo].[FK_ReportRant]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Reports] DROP CONSTRAINT [FK_ReportRant];
GO
IF OBJECT_ID(N'[dbo].[FK_UserRant]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Rants] DROP CONSTRAINT [FK_UserRant];
GO
IF OBJECT_ID(N'[dbo].[FK_UserReport]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Reports] DROP CONSTRAINT [FK_UserReport];
GO
IF OBJECT_ID(N'[dbo].[FK_UserTag]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Tags] DROP CONSTRAINT [FK_UserTag];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Rants]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Rants];
GO
IF OBJECT_ID(N'[dbo].[Tags]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Tags];
GO
IF OBJECT_ID(N'[dbo].[Reports]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Reports];
GO
IF OBJECT_ID(N'[dbo].[Users]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Users];
GO
IF OBJECT_ID(N'[dbo].[RantTag]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RantTag];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Rants'
CREATE TABLE [dbo].[Rants] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserId] int  NOT NULL,
    [Timestamp] datetime  NOT NULL,
    [Text] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Tags'
CREATE TABLE [dbo].[Tags] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserId] int  NOT NULL,
    [Text] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Reports'
CREATE TABLE [dbo].[Reports] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserId] int  NOT NULL,
    [RantId] int  NOT NULL,
    [Timestamp] datetime  NOT NULL,
    [Text] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Users'
CREATE TABLE [dbo].[Users] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Email] nvarchar(max)  NOT NULL,
    [Admin] bit  NOT NULL,
    [Banned] bit  NOT NULL
);
GO

-- Creating table 'Logs'
CREATE TABLE [dbo].[Logs] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Exception] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'RantTag'
CREATE TABLE [dbo].[RantTag] (
    [Rants_Id] int  NOT NULL,
    [Tags_Id] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Rants'
ALTER TABLE [dbo].[Rants]
ADD CONSTRAINT [PK_Rants]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Tags'
ALTER TABLE [dbo].[Tags]
ADD CONSTRAINT [PK_Tags]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Reports'
ALTER TABLE [dbo].[Reports]
ADD CONSTRAINT [PK_Reports]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [PK_Users]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Logs'
ALTER TABLE [dbo].[Logs]
ADD CONSTRAINT [PK_Logs]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Rants_Id], [Tags_Id] in table 'RantTag'
ALTER TABLE [dbo].[RantTag]
ADD CONSTRAINT [PK_RantTag]
    PRIMARY KEY CLUSTERED ([Rants_Id], [Tags_Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [Rants_Id] in table 'RantTag'
ALTER TABLE [dbo].[RantTag]
ADD CONSTRAINT [FK_RantTag_Rants]
    FOREIGN KEY ([Rants_Id])
    REFERENCES [dbo].[Rants]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Tags_Id] in table 'RantTag'
ALTER TABLE [dbo].[RantTag]
ADD CONSTRAINT [FK_RantTag_Tags]
    FOREIGN KEY ([Tags_Id])
    REFERENCES [dbo].[Tags]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_RantTag_Tags'
CREATE INDEX [IX_FK_RantTag_Tags]
ON [dbo].[RantTag]
    ([Tags_Id]);
GO

-- Creating foreign key on [RantId] in table 'Reports'
ALTER TABLE [dbo].[Reports]
ADD CONSTRAINT [FK_ReportRant]
    FOREIGN KEY ([RantId])
    REFERENCES [dbo].[Rants]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ReportRant'
CREATE INDEX [IX_FK_ReportRant]
ON [dbo].[Reports]
    ([RantId]);
GO

-- Creating foreign key on [UserId] in table 'Rants'
ALTER TABLE [dbo].[Rants]
ADD CONSTRAINT [FK_UserRant]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[Users]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserRant'
CREATE INDEX [IX_FK_UserRant]
ON [dbo].[Rants]
    ([UserId]);
GO

-- Creating foreign key on [UserId] in table 'Reports'
ALTER TABLE [dbo].[Reports]
ADD CONSTRAINT [FK_UserReport]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[Users]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserReport'
CREATE INDEX [IX_FK_UserReport]
ON [dbo].[Reports]
    ([UserId]);
GO

-- Creating foreign key on [UserId] in table 'Tags'
ALTER TABLE [dbo].[Tags]
ADD CONSTRAINT [FK_UserTag]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[Users]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserTag'
CREATE INDEX [IX_FK_UserTag]
ON [dbo].[Tags]
    ([UserId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------