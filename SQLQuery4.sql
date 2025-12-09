CREATE TABLE [dbo].[bookorder] (
    [Id]        INT          IDENTITY (1, 1) NOT NULL,
    [custname]  VARCHAR (50) NOT NULL,
    [orderdate] DATE         DEFAULT (getdate()) NOT NULL,
    [total]     INT          NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
