declare @Subdivision table (
	CountryCode char(2) not null,
	SubdivisionCode char(6) not null,
	NameEnglish nvarchar(50) not null,

	primary key (CountryCode, SubdivisionCode)
);

insert into		@Subdivision (CountryCode, SubdivisionCode, NameEnglish)
values			('AU', 'AU-ACT', 'Australian Capital Territory'),
				('AU', 'AU-NSW', 'New South Wales'),
				('AU', 'AU-NT ', 'Northern Territory'),
				('AU', 'AU-QLD', 'Queensland'),
				('AU', 'AU-SA ', 'South Australia'),
				('AU', 'AU-TAS', 'Tasmania'),
				('AU', 'AU-VIC', 'Victoria'),
				('AU', 'AU-WA ', 'Western Australia'),
				('NZ', 'NZ-AUK', 'Auckland'),
				('NZ', 'NZ-BOP', 'Bay of Plenty'),
				('NZ', 'NZ-CAN', 'Canterbury'),
				('NZ', 'NZ-CIT', 'Chatham Islands Territory'),
				('NZ', 'NZ-GIS', 'Gisborne'),
				('NZ', 'NZ-HKB', 'Hawke''s Bay'),
				('NZ', 'NZ-MBH', 'Marlborough'),
				('NZ', 'NZ-MWT', 'Manawatu-Wanganui'),
				('NZ', 'NZ-NSN', 'Nelson'),
				('NZ', 'NZ-NTL', 'Northland'),
				('NZ', 'NZ-OTA', 'Otago'),
				('NZ', 'NZ-STL', 'Southland'),
				('NZ', 'NZ-TAS', 'Tasman'),
				('NZ', 'NZ-TKI', 'Taranaki'),
				('NZ', 'NZ-WGN', 'Wellington'),
				('NZ', 'NZ-WKO', 'Waikato'),
				('NZ', 'NZ-WTC', 'West Coast');

merge into Jurisdiction.Subdivision t
using @Subdivision s
on s.CountryCode = t.CountryCode
and s.SubdivisionCode = t.SubdivisionCode
when matched
	then
		update
		set t.NameEnglish = s.NameEnglish
when not matched by target
	then
		insert (CountryCode, SubdivisionCode, NameEnglish)
		values (s.CountryCode, s.SubdivisionCode, s.NameEnglish)
when not matched by source
	then delete;

go