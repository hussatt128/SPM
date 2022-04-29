CREATE DATABASE YazzoomDB1
GO

USE [YazzoomDB]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PackageMaster](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](500) NULL,
	[Version] [varchar](50) NULL,
	[Status] [varchar](50) NULL,
	[File] [varchar](MAX) NULL
) ON [PRIMARY]
GO

INSERT INTO [dbo].[PackageMaster]
           ([Name]
           ,[Version]
           ,[Status]
           ,[File])
     VALUES
           ('Package 1'
           ,'1.0'
           ,'created'
           ,null)
GO