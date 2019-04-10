USE [master]
GO
/****** Object:  Database [WebVisualGame]    Script Date: 10.04.2019 21:27:58 ******/
CREATE DATABASE [WebVisualGame]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'WebVisualGame', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\DATA\WebVisualGame.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'WebVisualGame_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\DATA\WebVisualGame_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO
ALTER DATABASE [WebVisualGame] SET COMPATIBILITY_LEVEL = 120
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [WebVisualGame].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [WebVisualGame] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [WebVisualGame] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [WebVisualGame] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [WebVisualGame] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [WebVisualGame] SET ARITHABORT OFF 
GO
ALTER DATABASE [WebVisualGame] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [WebVisualGame] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [WebVisualGame] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [WebVisualGame] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [WebVisualGame] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [WebVisualGame] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [WebVisualGame] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [WebVisualGame] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [WebVisualGame] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [WebVisualGame] SET  ENABLE_BROKER 
GO
ALTER DATABASE [WebVisualGame] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [WebVisualGame] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [WebVisualGame] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [WebVisualGame] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [WebVisualGame] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [WebVisualGame] SET READ_COMMITTED_SNAPSHOT ON 
GO
ALTER DATABASE [WebVisualGame] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [WebVisualGame] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [WebVisualGame] SET  MULTI_USER 
GO
ALTER DATABASE [WebVisualGame] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [WebVisualGame] SET DB_CHAINING OFF 
GO
ALTER DATABASE [WebVisualGame] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [WebVisualGame] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [WebVisualGame] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [WebVisualGame] SET QUERY_STORE = OFF
GO
USE [WebVisualGame]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 10.04.2019 21:27:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Conditions]    Script Date: 10.04.2019 21:27:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Conditions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TransitionId] [int] NOT NULL,
	[Type] [bit] NOT NULL,
	[KeyСondition] [int] NOT NULL,
 CONSTRAINT [PK_Conditions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Games]    Script Date: 10.04.2019 21:27:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Games](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[Title] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](300) NULL,
	[UrlIcon] [nvarchar](100) NOT NULL,
	[Rating] [float] NOT NULL,
	[SourceCode] [nvarchar](1000) NOT NULL,
 CONSTRAINT [PK_Games] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PointDialogActions]    Script Date: 10.04.2019 21:27:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PointDialogActions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PointDialogId] [int] NOT NULL,
	[Type] [bit] NOT NULL,
	[KeyAction] [int] NOT NULL,
 CONSTRAINT [PK_PointDialogActions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PointDialogs]    Script Date: 10.04.2019 21:27:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PointDialogs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[GameId] [int] NOT NULL,
	[StateNumber] [int] NOT NULL,
	[Text] [nvarchar](300) NOT NULL,
	[Background_imageURL] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_PointDialogs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Reviews]    Script Date: 10.04.2019 21:27:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Reviews](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[GameId] [int] NOT NULL,
	[Mark] [int] NOT NULL,
	[Comment] [nvarchar](400) NULL,
	[Date] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_Reviews] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SavedGames]    Script Date: 10.04.2019 21:27:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SavedGames](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[GameId] [int] NOT NULL,
	[State] [int] NOT NULL,
	[Keys] [nvarchar](200) NOT NULL,
 CONSTRAINT [PK_SavedGames] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TransitionActions]    Script Date: 10.04.2019 21:27:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TransitionActions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TransitionId] [int] NOT NULL,
	[Type] [bit] NOT NULL,
	[KeyAction] [int] NOT NULL,
 CONSTRAINT [PK_TransitionActions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Transitions]    Script Date: 10.04.2019 21:27:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Transitions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StartPoint] [int] NOT NULL,
	[NextPoint] [int] NOT NULL,
	[GameId] [int] NOT NULL,
	[Text] [nvarchar](400) NOT NULL,
 CONSTRAINT [PK_Transitions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 10.04.2019 21:27:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Login] [nvarchar](100) NOT NULL,
	[Password] [nvarchar](100) NOT NULL,
	[FirstName] [nvarchar](100) NOT NULL,
	[LastName] [nvarchar](100) NOT NULL,
	[Email] [nvarchar](100) NOT NULL,
	[PasswordConfirm] [nvarchar](max) NULL,
	[AccessLevel] [int] NOT NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Index [IX_Conditions_TransitionId]    Script Date: 10.04.2019 21:27:59 ******/
CREATE NONCLUSTERED INDEX [IX_Conditions_TransitionId] ON [dbo].[Conditions]
(
	[TransitionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Games_UserId]    Script Date: 10.04.2019 21:27:59 ******/
CREATE NONCLUSTERED INDEX [IX_Games_UserId] ON [dbo].[Games]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_PointDialogActions_PointDialogId]    Script Date: 10.04.2019 21:27:59 ******/
CREATE NONCLUSTERED INDEX [IX_PointDialogActions_PointDialogId] ON [dbo].[PointDialogActions]
(
	[PointDialogId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_PointDialogs_GameId]    Script Date: 10.04.2019 21:27:59 ******/
CREATE NONCLUSTERED INDEX [IX_PointDialogs_GameId] ON [dbo].[PointDialogs]
(
	[GameId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Reviews_GameId]    Script Date: 10.04.2019 21:27:59 ******/
CREATE NONCLUSTERED INDEX [IX_Reviews_GameId] ON [dbo].[Reviews]
(
	[GameId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Reviews_UserId]    Script Date: 10.04.2019 21:27:59 ******/
CREATE NONCLUSTERED INDEX [IX_Reviews_UserId] ON [dbo].[Reviews]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_SavedGames_GameId]    Script Date: 10.04.2019 21:27:59 ******/
CREATE NONCLUSTERED INDEX [IX_SavedGames_GameId] ON [dbo].[SavedGames]
(
	[GameId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_SavedGames_UserId]    Script Date: 10.04.2019 21:27:59 ******/
CREATE NONCLUSTERED INDEX [IX_SavedGames_UserId] ON [dbo].[SavedGames]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_TransitionActions_TransitionId]    Script Date: 10.04.2019 21:27:59 ******/
CREATE NONCLUSTERED INDEX [IX_TransitionActions_TransitionId] ON [dbo].[TransitionActions]
(
	[TransitionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Transitions_GameId]    Script Date: 10.04.2019 21:27:59 ******/
CREATE NONCLUSTERED INDEX [IX_Transitions_GameId] ON [dbo].[Transitions]
(
	[GameId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT ((0)) FOR [AccessLevel]
GO
ALTER TABLE [dbo].[Conditions]  WITH CHECK ADD  CONSTRAINT [FK_Conditions_Transitions_TransitionId] FOREIGN KEY([TransitionId])
REFERENCES [dbo].[Transitions] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Conditions] CHECK CONSTRAINT [FK_Conditions_Transitions_TransitionId]
GO
ALTER TABLE [dbo].[Games]  WITH CHECK ADD  CONSTRAINT [FK_Games_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[Games] CHECK CONSTRAINT [FK_Games_Users_UserId]
GO
ALTER TABLE [dbo].[PointDialogActions]  WITH CHECK ADD  CONSTRAINT [FK_PointDialogActions_PointDialogs_PointDialogId] FOREIGN KEY([PointDialogId])
REFERENCES [dbo].[PointDialogs] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PointDialogActions] CHECK CONSTRAINT [FK_PointDialogActions_PointDialogs_PointDialogId]
GO
ALTER TABLE [dbo].[PointDialogs]  WITH CHECK ADD  CONSTRAINT [FK_PointDialogs_Games_GameId] FOREIGN KEY([GameId])
REFERENCES [dbo].[Games] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PointDialogs] CHECK CONSTRAINT [FK_PointDialogs_Games_GameId]
GO
ALTER TABLE [dbo].[Reviews]  WITH CHECK ADD  CONSTRAINT [FK_Reviews_Games_GameId] FOREIGN KEY([GameId])
REFERENCES [dbo].[Games] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Reviews] CHECK CONSTRAINT [FK_Reviews_Games_GameId]
GO
ALTER TABLE [dbo].[Reviews]  WITH CHECK ADD  CONSTRAINT [FK_Reviews_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Reviews] CHECK CONSTRAINT [FK_Reviews_Users_UserId]
GO
ALTER TABLE [dbo].[SavedGames]  WITH CHECK ADD  CONSTRAINT [FK_SavedGames_Games_GameId] FOREIGN KEY([GameId])
REFERENCES [dbo].[Games] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[SavedGames] CHECK CONSTRAINT [FK_SavedGames_Games_GameId]
GO
ALTER TABLE [dbo].[SavedGames]  WITH CHECK ADD  CONSTRAINT [FK_SavedGames_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[SavedGames] CHECK CONSTRAINT [FK_SavedGames_Users_UserId]
GO
ALTER TABLE [dbo].[TransitionActions]  WITH CHECK ADD  CONSTRAINT [FK_TransitionActions_Transitions_TransitionId] FOREIGN KEY([TransitionId])
REFERENCES [dbo].[Transitions] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TransitionActions] CHECK CONSTRAINT [FK_TransitionActions_Transitions_TransitionId]
GO
ALTER TABLE [dbo].[Transitions]  WITH CHECK ADD  CONSTRAINT [FK_Transitions_Games_GameId] FOREIGN KEY([GameId])
REFERENCES [dbo].[Games] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Transitions] CHECK CONSTRAINT [FK_Transitions_Games_GameId]
GO
USE [master]
GO
ALTER DATABASE [WebVisualGame] SET  READ_WRITE 
GO
