CREATE TABLE [Australia].[State] (
    [CountryCode]     CHAR (2) NOT NULL,
    [SubdivisionCode] CHAR (6) NOT NULL,
    CONSTRAINT [PK_State] PRIMARY KEY CLUSTERED ([CountryCode] ASC, [SubdivisionCode] ASC),
    CONSTRAINT [FK_State_Subdivision] FOREIGN KEY ([CountryCode], [SubdivisionCode]) REFERENCES [Australia].[Subdivision] ([CountryCode], [SubdivisionCode])
);

