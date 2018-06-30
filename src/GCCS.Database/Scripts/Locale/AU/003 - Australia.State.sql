declare @State table (
	CountryCode char(2) not null,
	SubdivisionCode char(6) not null,

	primary key (CountryCode, SubdivisionCode)
);

insert into		@State (CountryCode, SubdivisionCode)
values			('AU', 'AU-NSW'),
				('AU', 'AU-QLD'),
				('AU', 'AU-SA '),
				('AU', 'AU-TAS'),
				('AU', 'AU-VIC'),
				('AU', 'AU-WA ');

merge into Australia.[State] t
using @State s
on s.CountryCode = t.CountryCode
and s.SubdivisionCode = t.SubdivisionCode
--when matched
--	then
--		update set ...
when not matched by target
	then
		insert (CountryCode, SubdivisionCode)
		values (s.CountryCode, s.SubdivisionCode)
when not matched by source
	then delete;

go