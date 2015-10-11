
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 10/11/2015 15:41:29
-- Generated from EDMX file: C:\Users\ngpitt\Documents\GitHub\Enrampage\Enrampage\Models\EnrampageModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [Enrampage];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_RantTag_Rant]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RantTag] DROP CONSTRAINT [FK_RantTag_Rant];
GO
IF OBJECT_ID(N'[dbo].[FK_RantTag_Tag]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RantTag] DROP CONSTRAINT [FK_RantTag_Tag];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Rants]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Rants];
GO
IF OBJECT_ID(N'[dbo].[RantTag]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RantTag];
GO
IF OBJECT_ID(N'[dbo].[Tags]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Tags];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Rants'
CREATE TABLE [dbo].[Rants] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Created] datetime  NOT NULL,
    [Text] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Tags'
CREATE TABLE [dbo].[Tags] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Text] nvarchar(max)  NOT NULL
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

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------