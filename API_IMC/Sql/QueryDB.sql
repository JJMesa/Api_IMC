IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'IMC')
BEGIN
	CREATE DATABASE IMC
END
GO

USE IMC
GO


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tblRoles]') AND type in (N'U'))
BEGIN
	CREATE TABLE [dbo].[tblRoles](
		[intPkIdRol] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
		[strDescription] [varchar](200) NOT NULL
	)

	INSERT INTO [dbo].[tblRoles] (strDescription) VALUES ('Administrator')
	
	INSERT INTO [dbo].[tblRoles] (strDescription) VALUES ('Evaluator')

	INSERT INTO [dbo].[tblRoles] (strDescription) VALUES ('Client')
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tblUsers]') AND type in (N'U'))
BEGIN
	CREATE TABLE [dbo].[tblUsers](
		[intPkIdUser] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
		[strIdentification] [varchar] (50) NOT NULL,
		[strUserName] [varchar](200) NOT NULL,
		[strPassword] [varchar](200) NOT NULL,
		[strName] [varchar](200) NOT NULL,
		[strLastName] [varchar](200) NOT NULL,
		[intFkIdRol] [int] NOT NULL,
		FOREIGN KEY (intFkIdRol) REFERENCES tblRoles (intPkIdRol)
	)
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tblGenders]') AND type in (N'U'))
BEGIN
	CREATE TABLE [dbo].[tblGenders](
		[intPkIdGender] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
		[strDescription] [varchar](50) NOT NULL
	)

	INSERT INTO [dbo].[tblGenders] (strDescription) VALUES ('Femenino')
	
	INSERT INTO [dbo].[tblGenders] (strDescription) VALUES ('Masculino')
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tblHistoricalEvaluations]') AND type in (N'U'))
BEGIN
	CREATE TABLE [dbo].[tblHistoricalEvaluations](
		[intPkIdEvaluation] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
		[intConsultationNumber] [int] NOT NULL,
		[intFkIdUser] [int] NOT NULL,
		[intFKIdGender] [int] NOT NULL,
		[intUserAge] [int] NOT NULL,
		[fltWeight] [float] NOT NULL,
		[fltHeight] [float] NOT NULL,
		[strBodyType] [varchar](50) NOT NULL,
		[fltBodyMass] [float] NOT NULL DEFAULT(0),
		[strObservations] [varchar](500),
		[dtmEvaluationDate] [datetime] NOT NULL DEFAULT(GETDATE()),
		FOREIGN KEY (intFkIdUser) REFERENCES tblUsers (intPkIdUser),
		FOREIGN KEY (intFKIdGender) REFERENCES tblGenders (intPkIdGender)
	)
END
GO