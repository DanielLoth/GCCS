CREATE TABLE [Jurisdiction].[Country] (
    [CountryCode] CHAR (2)      NOT NULL,
    [CountryName] NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_Country] PRIMARY KEY CLUSTERED ([CountryCode] ASC)
);

