USE ApworksCQRSArchEventStoreTestDB;
/****** Object:  Table [dbo].[DomainEvents]    Script Date: 04/15/2011 08:19:56 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DomainEvents]') AND type in (N'U'))
DROP TABLE [dbo].[DomainEvents]
GO
/****** Object:  Table [dbo].[Snapshots]    Script Date: 04/15/2011 08:19:56 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Snapshots]') AND type in (N'U'))
DROP TABLE [dbo].[Snapshots]
GO
/****** Object:  Table [dbo].[SourcedCustomer]    Script Date: 04/15/2011 08:19:56 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SourcedCustomer]') AND type in (N'U'))
DROP TABLE [dbo].[SourcedCustomer]
GO
/****** Object:  Table [dbo].[SourcedCustomer]    Script Date: 04/15/2011 08:19:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SourcedCustomer]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[SourcedCustomer](
	[Id] [uniqueidentifier] NOT NULL,
	[Username] [nvarchar](255) COLLATE Chinese_PRC_CI_AS NULL,
	[Password] [nvarchar](255) COLLATE Chinese_PRC_CI_AS NULL,
	[FirstName] [nvarchar](255) COLLATE Chinese_PRC_CI_AS NULL,
	[LastName] [nvarchar](255) COLLATE Chinese_PRC_CI_AS NULL,
	[Email] [nvarchar](255) COLLATE Chinese_PRC_CI_AS NULL,
	[Birth] [datetime] NULL,
 CONSTRAINT [PK__SourcedC__3214EC072116E6DF] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
END
GO
/****** Object:  Table [dbo].[Snapshots]    Script Date: 04/15/2011 08:19:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Snapshots]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Snapshots](
	[Id] [uniqueidentifier] NOT NULL,
	[Timestamp] [datetime] NOT NULL,
	[SnapshotData] [varbinary](max) NOT NULL,
	[AggregateRootID] [uniqueidentifier] NOT NULL,
	[AggregateRootType] [nvarchar](max) COLLATE Chinese_PRC_CI_AS NOT NULL,
	[SnapshotType] [nvarchar](max) COLLATE Chinese_PRC_CI_AS NOT NULL,
	[Version] [bigint] NOT NULL,
	[Branch] [bigint] NOT NULL,
 CONSTRAINT [PK_Snapshots] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
END
GO
/****** Object:  Table [dbo].[DomainEvents]    Script Date: 04/15/2011 08:19:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DomainEvents]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[DomainEvents](
	[Id] [uniqueidentifier] NOT NULL,
	[SourceID] [uniqueidentifier] NOT NULL,
	[AssemblyQualifiedSourceType] [nvarchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Timestamp] [datetime] NOT NULL,
	[Version] [bigint] NOT NULL,
	[Branch] [bigint] NOT NULL,
	[AssemblyQualifiedEventType] [nvarchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Data] [varbinary](max) NOT NULL,
 CONSTRAINT [PK_DomainEvents] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
END
GO
