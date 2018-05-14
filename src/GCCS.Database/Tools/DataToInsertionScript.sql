--select *, '(''' + CountryCode + ''', ''' + CountryName + ''')'
--from Jurisdiction.Country

declare @NewLine char(2) = char(13) + char(10);
declare @Tab char(1) = char(9);

with ColumnNamesCommaSeparated as (
	select
		c.TABLE_SCHEMA, c.TABLE_NAME,
		QUOTENAME(c.TABLE_SCHEMA) + '.' + QUOTENAME(c.TABLE_NAME) as FullyQualifiedTableName,
		STRING_AGG(c.COLUMN_NAME, ', ') as ColumnNamesCommaSeparated
	from INFORMATION_SCHEMA.COLUMNS c
	group by c.TABLE_SCHEMA, c.TABLE_NAME
),
SelectQueries as (
	select
		c.TABLE_SCHEMA, c.TABLE_NAME,

		'select ' + c.ColumnNamesCommaSeparated + @NewLine +
		'from ' + c.FullyQualifiedTableName + @NewLine + @NewLine
		as SelectQuery
	from ColumnNamesCommaSeparated c
),
InsertQueries as (
	select
		'insert into ' + c.FullyQualifiedTableName +
		' (' +
		c.ColumnNamesCommaSeparated +
		')' + @NewLine +
		'values' + @NewLine + @NewLine
		as InsertQuery
	from ColumnNamesCommaSeparated c
)

select *
from SelectQueries;

--exec sp_msforeachtable 'select ''?'' as TableName, * from ?'
