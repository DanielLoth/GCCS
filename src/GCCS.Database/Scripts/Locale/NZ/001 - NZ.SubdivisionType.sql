declare @SubdivisionType table (
	SubdivisionType char(1) not null,
	SubdivisionName nvarchar(25) not null,

	primary key (SubdivisionType)
);

insert into		@SubdivisionType (SubdivisionType, SubdivisionName)
values			('R', 'Region'),
				('S', 'Special Island Authority');

merge into NewZealand.SubdivisionType t
using @SubdivisionType s
on s.SubdivisionType = t.SubdivisionType
when matched
	then
		update
		set t.SubdivisionName = s.SubdivisionName
when not matched by target
	then
		insert (SubdivisionType, SubdivisionName)
		values (s.SubdivisionType, s.SubdivisionName)
when not matched by source
	then delete;

go