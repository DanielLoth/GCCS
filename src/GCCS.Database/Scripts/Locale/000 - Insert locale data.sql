-- Insert Country and Subdivision tables.

:r ".\001 - Jurisdiction.Country.sql"
:r ".\002 - Jurisdiction.Subdivision.sql"


-- Then insert one country at a time.
:r ".\AU\000 - Insert Australia data.sql"
:r ".\NZ\000 - Insert NZ data.sql"