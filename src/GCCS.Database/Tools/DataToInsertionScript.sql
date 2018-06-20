--------------------------------------------------------------------------------
-- CONFIGURATION SECTION
--------------------------------------------------------------------------------
--
-- The following variables can be set to include or exclude tables and schemas
-- from the resulting code-generated queries.
--------------------------------------------------------------------------------

-- Set @IncludedTables to 1 if you want to only include those tables listed in
-- the @IncludedTable table variable.
declare @IncludeTables bit = 0;


-- Set @ExcludeTables to 1 if you want to exclude any tables listed in the
-- @ExcludedTable table variable.
declare @ExcludeTables bit = 0;


declare @IncludedTable table (
	SchemaName varchar(128),
	TableName varchar(128),

	primary key (SchemaName, TableName)
);


-- Specify the list of tables to be included if @IncludeTables is set to 1.
-- When @IncludeTables is set to 1, only the tables in this list will be
-- included in the result set.
insert into @IncludedTable (SchemaName, TableName)
values
	('Jurisdiction', 'Country');


declare @ExcludedTable table (
	SchemaName varchar(128),
	TableName varchar(128),

	primary key (SchemaName, TableName)
);


-- Specify the list of tables to be excluded if @ExcludeTables is set to 1.
insert into @ExcludedTable (SchemaName, TableName)
values
	('dbo', 'sysdiagrams');

--------------------------------------------------------------------------------
-- END OF CONFIGURATION SECTION
--------------------------------------------------------------------------------




set nocount on;
set xact_abort on;
set transaction isolation level read committed;

begin transaction


declare @NewLine char(2) = char(13) + char(10);
declare @Tab char(1) = char(9);

with ColumnNamesCommaSeparated as (
	select
		c.TABLE_SCHEMA as SchemaName,
		c.TABLE_NAME as TableName,
		QUOTENAME(c.TABLE_SCHEMA) + '.' + QUOTENAME(c.TABLE_NAME) as FullyQualifiedTableName,
		STRING_AGG(QUOTENAME(c.COLUMN_NAME), ', ') as ColumnNamesCommaSeparated
	from INFORMATION_SCHEMA.COLUMNS c
	where 1=1
		and
		(
			@IncludeTables = 0
			or (
				@IncludeTables = 1
				and exists (
					select top 1 1
					from @IncludedTable it
					where c.TABLE_SCHEMA = it.SchemaName
						and c.TABLE_NAME = it.TableName
				)
			)
		)
		and
		(
			@ExcludeTables = 0
			or (
				@ExcludeTables = 1
				and not exists (
					select top 1 1
					from @ExcludedTable et
					where c.TABLE_SCHEMA = et.SchemaName
						and c.TABLE_NAME = et.TableName
				)
			)
		)
	group by c.TABLE_SCHEMA, c.TABLE_NAME
),
SelectQueries as (
	select
		c.SchemaName, c.TableName,

		'select ' + c.ColumnNamesCommaSeparated + @NewLine +
		'from ' + c.FullyQualifiedTableName + @NewLine + @NewLine
		as SelectQuery
	from ColumnNamesCommaSeparated c
),
InsertQueries as (
	select
		c.SchemaName, c.TableName,

		'insert into ' + c.FullyQualifiedTableName +
		' (' + c.ColumnNamesCommaSeparated + ')' + @NewLine +
		'values' + @NewLine + @NewLine
		as InsertQueryStart
	from ColumnNamesCommaSeparated c
)

select
	sq.SchemaName, sq.TableName,
	sq.SelectQuery, iq.InsertQueryStart
from SelectQueries sq
inner join InsertQueries iq
	on sq.SchemaName = iq.SchemaName
	and sq.TableName = iq.TableName;

commit transaction;

--exec sp_msforeachtable 'select ''?'' as TableName, * from ?'
--select *, '(''' + CountryCode + ''', ''' + CountryName + ''')'
--from Jurisdiction.Country