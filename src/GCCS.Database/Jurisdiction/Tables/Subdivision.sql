CREATE TABLE [Jurisdiction].[Subdivision] (
    [CountryCode]     CHAR (2)      NOT NULL,
    [SubdivisionCode] CHAR (6)      NOT NULL,
    [NameEnglish]     NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_Subdivision] PRIMARY KEY CLUSTERED ([CountryCode] ASC, [SubdivisionCode] ASC),
    CONSTRAINT [FK_Subdivision_Country] FOREIGN KEY ([CountryCode]) REFERENCES [Jurisdiction].[Country] ([CountryCode])
);



