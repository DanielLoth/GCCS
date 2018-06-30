declare @Territory table (
	CountryCode char(2) not null,
	SubdivisionCode char(6) not null,

	primary key (CountryCode, SubdivisionCode)
);

insert into		@Territory (CountryCode, SubdivisionCode)
values			('AU', 'AU-ACT'),
				('AU', 'AU-NT ');

merge into Australia.Territory t
using @Territory s
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