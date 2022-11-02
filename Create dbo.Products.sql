USE [eShopDB]
GO

/****** Object: Table [dbo].[Products] Script Date: 8/30/2022 1:48:04 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Products] (
    [ID]               INT            IDENTITY (1, 1) NOT NULL,
    [title]            NVARCHAR (MAX) NULL,
    [descriptionShort] NVARCHAR (MAX) NULL,
    [descriptionLong]  NVARCHAR (MAX) NULL,
    [category]         NVARCHAR (MAX) NULL,
    [price]            REAL           NOT NULL,
    [imageSource]      NVARCHAR (MAX) NULL,
    [Manufacturer]     NVARCHAR (MAX) NOT NULL
);

--UPDATE [dbo].[Products] 
--SET price = 5 
--where ID = 35 
--and timeStamp = 'A'



--UPDATE [dbo].[Products] 
--SET price = 5 
--where ID = 35 


--UPDATE [dbo].[Products] 
--SET price = 5 