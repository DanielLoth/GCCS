CREATE TABLE [Australia].[Territory] (
    [CountryCode]     CHAR (2) NOT NULL,
    [SubdivisionCode] CHAR (6) NOT NULL,
    CONSTRAINT [PK_Territory] PRIMARY KEY CLUSTERED ([CountryCode] ASC, [SubdivisionCode] ASC),
    CONSTRAINT [FK_Territory_Subdivision] FOREIGN KEY ([CountryCode], [SubdivisionCode]) REFERENCES [Australia].[Subdivision] ([CountryCode], [SubdivisionCode])
);

