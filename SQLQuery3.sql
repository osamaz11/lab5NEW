CREATE TABLE [dbo].[orders] (
    [Id]       INT  IDENTITY (1, 1) NOT NULL,
    [custname]  VARCHAR (50) NOT NULL,
    [bookname]  VARCHAR (50) NOT NULL,
    [buydate]  DATE DEFAULT (getdate()) NULL,
    [quantity] INT  NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
