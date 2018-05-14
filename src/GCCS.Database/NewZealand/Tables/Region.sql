CREATE TABLE [NewZealand].[Region] (
    [CountryCode]     CHAR (2) NOT NULL,
    [SubdivisionCode] CHAR (6) NOT NULL,
    CONSTRAINT [PK_Region] PRIMARY KEY CLUSTERED ([CountryCode] ASC, [SubdivisionCode] ASC),
    CONSTRAINT [FK_Region_Subdivision] FOREIGN KEY ([CountryCode], [SubdivisionCode]) REFERENCES [NewZealand].[Subdivision] ([CountryCode], [SubdivisionCode])
);

