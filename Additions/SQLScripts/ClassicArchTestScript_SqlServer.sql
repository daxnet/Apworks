USE ApworksClassicArchTestDB;
/****** Object:  Table [dbo].[Customer]    Script Date: 12/13/2010 16:36:56 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Customer]') AND type in (N'U'))
DROP TABLE [dbo].[Customer]
GO
/****** Object:  Table [dbo].[Customer]    Script Date: 12/13/2010 16:36:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Customer]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Customer](
	[Id] [uniqueidentifier] NOT NULL,
	[Username] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Password] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[FirstName] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[LastName] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Email] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Birth] [datetime] NULL,
	[Sequence] [int],
 CONSTRAINT [PK__Customer__3214EC0730242045] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
END
GO
