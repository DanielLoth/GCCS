CREATE TABLE [NewZealand].[SubdivisionNameMaori] (
    [CountryCode]     CHAR (2)      NOT NULL,
    [SubdivisionCode] CHAR (6)      NOT NULL,
    [NameMaori]       NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_SubdivisionNameMaori] PRIMARY KEY CLUSTERED ([CountryCode] ASC, [SubdivisionCode] ASC),
    CONSTRAINT [FK_SubdivisionNameMaori_Subdivision] FOREIGN KEY ([CountryCode], [SubdivisionCode]) REFERENCES [NewZealand].[Subdivision] ([CountryCode], [SubdivisionCode])
);

