CREATE TABLE [dbo].[orderline] (
    [id]        INT          IDENTITY (1, 1) NOT NULL,
    [itemname]  VARCHAR (50) NOT NULL,
    [itemquant] INT          NOT NULL,
    [itemprice] DECIMAL (5, 1) NULL,  
     [orderid]   INT          NOT NULL,
    PRIMARY KEY CLUSTERED ([id] ASC)
);
