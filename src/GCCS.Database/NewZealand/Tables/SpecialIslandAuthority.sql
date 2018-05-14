CREATE TABLE [NewZealand].[SpecialIslandAuthority] (
    [CountryCode]     CHAR (2) NOT NULL,
    [SubdivisionCode] CHAR (6) NOT NULL,
    CONSTRAINT [PK_SpecialIslandAuthority] PRIMARY KEY CLUSTERED ([CountryCode] ASC, [SubdivisionCode] ASC),
    CONSTRAINT [FK_SpecialIslandAuthority_Subdivision] FOREIGN KEY ([CountryCode], [SubdivisionCode]) REFERENCES [NewZealand].[Subdivision] ([CountryCode], [SubdivisionCode])
);

