USE [master]
GO
/****** Object:  Database [API_DAMs]    Script Date: 02-Feb-25 7:36:51 AM ******/
CREATE DATABASE [API_DAMs]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'API_DAMs', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\DATA\API_DAMs.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'API_DAMs_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\DATA\API_DAMs_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [API_DAMs] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [API_DAMs].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [API_DAMs] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [API_DAMs] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [API_DAMs] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [API_DAMs] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [API_DAMs] SET ARITHABORT OFF 
GO
ALTER DATABASE [API_DAMs] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [API_DAMs] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [API_DAMs] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [API_DAMs] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [API_DAMs] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [API_DAMs] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [API_DAMs] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [API_DAMs] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [API_DAMs] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [API_DAMs] SET  ENABLE_BROKER 
GO
ALTER DATABASE [API_DAMs] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [API_DAMs] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [API_DAMs] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [API_DAMs] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [API_DAMs] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [API_DAMs] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [API_DAMs] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [API_DAMs] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [API_DAMs] SET  MULTI_USER 
GO
ALTER DATABASE [API_DAMs] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [API_DAMs] SET DB_CHAINING OFF 
GO
ALTER DATABASE [API_DAMs] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [API_DAMs] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [API_DAMs] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [API_DAMs] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [API_DAMs] SET QUERY_STORE = ON
GO
ALTER DATABASE [API_DAMs] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [API_DAMs]
GO
/****** Object:  Table [dbo].[api_header]    Script Date: 02-Feb-25 7:36:51 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[api_header](
	[code_id] [int] IDENTITY(1,1) NOT NULL,
	[code_text] [text] NOT NULL,
	[code_platform] [varchar](255) NULL,
	[code_description] [varchar](500) NULL,
	[code_uploadDate] [datetime] NOT NULL,
	[user_id] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[code_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[api_methods]    Script Date: 02-Feb-25 7:36:52 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[api_methods](
	[API_id] [int] IDENTITY(1,1) NOT NULL,
	[API_name] [varchar](255) NOT NULL,
	[API_paracount] [int] NOT NULL,
	[API_returnType] [varchar](255) NOT NULL,
	[API_HTTP_method] [varchar](10) NOT NULL,
	[API_desc] [varchar](500) NULL,
	[API_endpoint] [varchar](255) NOT NULL,
	[API_post_method] [varchar](10) NULL,
	[API_update_date] [datetime] NULL,
	[code_id] [int] NULL,
	[app_id] [int] NULL,
	[file_id] [int] NULL,
	[API_invoke_count] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[API_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[application]    Script Date: 02-Feb-25 7:36:52 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[application](
	[app_id] [int] IDENTITY(1,1) NOT NULL,
	[app_name] [varchar](255) NOT NULL,
	[app_testing_path] [varchar](500) NULL,
	[app_production_path] [varchar](500) NULL,
	[app_language] [varchar](255) NULL,
	[user_id] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[app_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[collaborator]    Script Date: 02-Feb-25 7:36:52 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[collaborator](
	[collab_id] [int] IDENTITY(1,1) NOT NULL,
	[collab_permission] [varchar](255) NOT NULL,
	[collab_date] [datetime] NOT NULL,
	[owner_id] [int] NOT NULL,
	[shared_id] [int] NOT NULL,
	[app_id] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[collab_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[collaborator_backup]    Script Date: 02-Feb-25 7:36:52 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[collaborator_backup](
	[collab_id] [int] IDENTITY(1,1) NOT NULL,
	[collab_permission] [varchar](255) NOT NULL,
	[collab_date] [datetime] NOT NULL,
	[API_id] [int] NOT NULL,
	[owner_id] [int] NOT NULL,
	[shared_id] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[file_details]    Script Date: 02-Feb-25 7:36:52 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[file_details](
	[file_id] [int] IDENTITY(1,1) NOT NULL,
	[file_platform] [varchar](255) NULL,
	[file_path] [varchar](500) NULL,
	[file_desc] [varchar](500) NULL,
	[file_upload_date] [datetime] NOT NULL,
	[user_id] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[file_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[friends]    Script Date: 02-Feb-25 7:36:52 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[friends](
	[friend_id] [int] IDENTITY(1,1) NOT NULL,
	[friend_date] [datetime] NOT NULL,
	[initiator_id] [int] NULL,
	[receiver_id] [int] NULL,
	[friend_req] [tinyint] NOT NULL,
	[friend_status] [tinyint] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[friend_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[notification]    Script Date: 02-Feb-25 7:36:52 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[notification](
	[noti_id] [int] IDENTITY(1,1) NOT NULL,
	[noti_type] [varchar](255) NOT NULL,
	[noti_time] [datetime] NOT NULL,
	[noti_message] [varchar](500) NULL,
	[user1_id] [int] NOT NULL,
	[user2_id] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[noti_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[parameter_details]    Script Date: 02-Feb-25 7:36:52 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[parameter_details](
	[para_id] [int] IDENTITY(1,1) NOT NULL,
	[para_type] [varchar](255) NOT NULL,
	[para_name] [varchar](255) NOT NULL,
	[para_json_keys] [varchar](500) NULL,
	[API_id] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[para_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[users]    Script Date: 02-Feb-25 7:36:52 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[users](
	[user_id] [int] IDENTITY(1,1) NOT NULL,
	[user_joined_date] [datetime] NOT NULL,
	[user_email] [varchar](255) NOT NULL,
	[user_username] [varchar](255) NOT NULL,
	[user_password] [varchar](255) NOT NULL,
	[user_image] [varchar](255) NULL,
	[user_phone] [varchar](50) NULL,
	[user_name] [varchar](50) NULL,
	[user_visibility] [bit] NULL,
	[user_tagline] [varchar](255) NULL,
	[user_desc] [varchar](500) NULL,
PRIMARY KEY CLUSTERED 
(
	[user_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[user_username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[user_email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[api_header] ADD  DEFAULT (getdate()) FOR [code_uploadDate]
GO
ALTER TABLE [dbo].[api_methods] ADD  DEFAULT (getdate()) FOR [API_update_date]
GO
ALTER TABLE [dbo].[api_methods] ADD  CONSTRAINT [DF_API_invoke_count]  DEFAULT ((0)) FOR [API_invoke_count]
GO
ALTER TABLE [dbo].[collaborator] ADD  DEFAULT (getdate()) FOR [collab_date]
GO
ALTER TABLE [dbo].[file_details] ADD  DEFAULT (getdate()) FOR [file_upload_date]
GO
ALTER TABLE [dbo].[friends] ADD  DEFAULT (getdate()) FOR [friend_date]
GO
ALTER TABLE [dbo].[friends] ADD  DEFAULT ((0)) FOR [friend_req]
GO
ALTER TABLE [dbo].[friends] ADD  DEFAULT ((0)) FOR [friend_status]
GO
ALTER TABLE [dbo].[notification] ADD  DEFAULT (getdate()) FOR [noti_time]
GO
ALTER TABLE [dbo].[users] ADD  DEFAULT (getdate()) FOR [user_joined_date]
GO
ALTER TABLE [dbo].[users] ADD  DEFAULT ((1)) FOR [user_visibility]
GO
ALTER TABLE [dbo].[api_header]  WITH CHECK ADD FOREIGN KEY([user_id])
REFERENCES [dbo].[users] ([user_id])
GO
ALTER TABLE [dbo].[api_methods]  WITH CHECK ADD FOREIGN KEY([code_id])
REFERENCES [dbo].[api_header] ([code_id])
GO
ALTER TABLE [dbo].[api_methods]  WITH CHECK ADD  CONSTRAINT [FK_api_methods_application] FOREIGN KEY([app_id])
REFERENCES [dbo].[application] ([app_id])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[api_methods] CHECK CONSTRAINT [FK_api_methods_application]
GO
ALTER TABLE [dbo].[api_methods]  WITH CHECK ADD  CONSTRAINT [FK_api_methods_file_details] FOREIGN KEY([file_id])
REFERENCES [dbo].[file_details] ([file_id])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[api_methods] CHECK CONSTRAINT [FK_api_methods_file_details]
GO
ALTER TABLE [dbo].[application]  WITH CHECK ADD  CONSTRAINT [FK_application_user] FOREIGN KEY([user_id])
REFERENCES [dbo].[users] ([user_id])
ON UPDATE CASCADE
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[application] CHECK CONSTRAINT [FK_application_user]
GO
ALTER TABLE [dbo].[collaborator]  WITH CHECK ADD FOREIGN KEY([owner_id])
REFERENCES [dbo].[users] ([user_id])
GO
ALTER TABLE [dbo].[collaborator]  WITH CHECK ADD FOREIGN KEY([shared_id])
REFERENCES [dbo].[users] ([user_id])
GO
ALTER TABLE [dbo].[collaborator]  WITH CHECK ADD  CONSTRAINT [FK_collaborator_application] FOREIGN KEY([app_id])
REFERENCES [dbo].[application] ([app_id])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[collaborator] CHECK CONSTRAINT [FK_collaborator_application]
GO
ALTER TABLE [dbo].[file_details]  WITH CHECK ADD FOREIGN KEY([user_id])
REFERENCES [dbo].[users] ([user_id])
GO
ALTER TABLE [dbo].[friends]  WITH CHECK ADD FOREIGN KEY([initiator_id])
REFERENCES [dbo].[users] ([user_id])
GO
ALTER TABLE [dbo].[friends]  WITH CHECK ADD FOREIGN KEY([receiver_id])
REFERENCES [dbo].[users] ([user_id])
GO
ALTER TABLE [dbo].[notification]  WITH CHECK ADD FOREIGN KEY([user1_id])
REFERENCES [dbo].[users] ([user_id])
GO
ALTER TABLE [dbo].[notification]  WITH CHECK ADD FOREIGN KEY([user2_id])
REFERENCES [dbo].[users] ([user_id])
GO
ALTER TABLE [dbo].[parameter_details]  WITH CHECK ADD FOREIGN KEY([API_id])
REFERENCES [dbo].[api_methods] ([API_id])
GO
USE [master]
GO
ALTER DATABASE [API_DAMs] SET  READ_WRITE 
GO
