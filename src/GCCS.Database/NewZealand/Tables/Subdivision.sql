CREATE TABLE [NewZealand].[Subdivision] (
    [CountryCode]     CHAR (2) NOT NULL,
    [SubdivisionCode] CHAR (6) NOT NULL,
    [SubdivisionType] CHAR (1) NOT NULL,
    CONSTRAINT [PK_Subdivision_2] PRIMARY KEY CLUSTERED ([CountryCode] ASC, [SubdivisionCode] ASC),
    CONSTRAINT [FK_Subdivision_Subdivision] FOREIGN KEY ([CountryCode], [SubdivisionCode]) REFERENCES [Jurisdiction].[Subdivision] ([CountryCode], [SubdivisionCode]),
    CONSTRAINT [FK_Subdivision_SubdivisionType] FOREIGN KEY ([SubdivisionType]) REFERENCES [NewZealand].[SubdivisionType] ([SubdivisionType])
);



