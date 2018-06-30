declare @Subdivision table (
	CountryCode char(2) not null,
	SubdivisionCode char(6) not null,
	SubdivisionType char(1) not null,

	primary key (CountryCode, SubdivisionCode)
);

insert into		@Subdivision (CountryCode, SubdivisionCode, SubdivisionType)
values			('AU', 'AU-ACT', 'T'),
				('AU', 'AU-NSW', 'S'),
				('AU', 'AU-NT ', 'T'),
				('AU', 'AU-QLD', 'S'),
				('AU', 'AU-SA ', 'S'),
				('AU', 'AU-TAS', 'S'),
				('AU', 'AU-VIC', 'S'),
				('AU', 'AU-WA ', 'S');

merge into Australia.Subdivision t
using @Subdivision s
on s.CountryCode = t.CountryCode
and s.SubdivisionCode = t.SubdivisionCode
--when matched
--	then
--		update set ...
when not matched by target
	then
		insert (CountryCode, SubdivisionCode, SubdivisionType)
		values (s.CountryCode, s.SubdivisionCode, s.SubdivisionType)
when not matched by source
	then delete;

go