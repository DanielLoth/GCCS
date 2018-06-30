declare @Country table (
	CountryCode char(2) not null,
	CountryName nvarchar(50) not null,

	primary key (CountryCode)
);

insert into	@Country (CountryCode, CountryName)
values		('AU', 'Australia'),
			('NZ', 'New Zealand');

merge into Jurisdiction.Country t
using @Country s
on s.CountryCode = t.CountryCode
when matched
	then
		update
		set t.CountryName = s.CountryName
when not matched by target
	then
		insert (CountryCode, CountryName)
		values (s.CountryCode, s.CountryName)
when not matched by source
	then delete;

go