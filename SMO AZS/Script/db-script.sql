create database [��� ���]
go

use [��� ���]
go

create table [�����������]
(
	Id int primary key identity,
	[���� ������������] datetime not null,
	[���������� ������] nvarchar(max) not null,
)
go

create trigger [������������������] on [�����������]
after update, insert as
begin
	declare @data datetime
	declare _cursor1 Cursor for select [���� ������������] from inserted
	open _cursor1
	fetch next from _cursor1 into @data

	while @@FETCH_STATUS = 0
	begin
		if @data > GETDATE()
		begin
			rollback
			raiserror('���� ������ �������', 16, 2)
		end

		fetch next from _cursor1 into @data
	end

	CLOSE _cursor1
	DEALLOCATE _cursor1
end;
go

create table [�������� ������] 
(
	Id int primary key identity,
	[����������� Id] int references [�����������](Id) not null,
	[����� �������] int not null,
	[������������� �������� ������ (����� ��������)] float not null,
	[������� ����� ������������ ����� ������ � �������] float not null,
	[������������� ������ ������������] float not null,
	[������������� ��������] float not null,
	[����������������� �������� ��� � �����] int null,
	[��������� ����������� ������������] float null,
	[����� �������] int null
)
go

create trigger [���������������������] on [�������� ������]
after update, insert as
begin
	declare @data int
	declare _cursor2 Cursor for select [����������������� �������� ��� � �����] from inserted
	open _cursor2
	fetch next from _cursor2 into @data

	while @@FETCH_STATUS = 0
	begin
		if @data > 24 or @data <= 0
		begin
			rollback
			raiserror('����������������� �������� ��� �� ����� ���� ������ 0 ��� ������ 24', 16, 2)
		end

		fetch next from _cursor2 into @data
	end

	CLOSE _cursor2
	DEALLOCATE _cursor2
end;
go


create table [������� � �������] 
(
	Id int primary key identity,
	[�������� ������ Id] int references [�������� ������](Id) not null,
	[����������� ������� ������� ������������, ����� ��� ������] float not null,
	[����������� ������ � ������������, ����� ����������� �� ������������ ������ ������ ��� ������ ��������] float not null,
	[����������� ������������] float not null,
	[������� ����� ������� ������������� �������] float not null,
	[���� �������, ������� �������������] float not null,
	[���������� ���������� ����������� ���] float not null
);

create table [������� � ������������ �����] 
(
	Id int primary key identity,
	[�������� ������ Id] int references [�������� ������](Id) not null,
	[����������� ������� ������� ������������, ����� ��� ������] float not null,
	[����������� ������ � ������������] float not null,
	[����������� ������������] float not null,
	[���������� ���������� �����������] float not null,
	[������� ����� ������� �������] float not null,
	[������� ����� ������ � �������] float not null,
	[������� ����� �������� ������������ � �������] float not null,
	[������� ����� ������ � �������] float not null,
	[������� ����� ���������� � ������� � �������] float not null

);

create table [������� � �������������� ���������] 
(
	Id int primary key identity,
	[�������� ������ Id] int references [�������� ������](Id) not null,
	[����������� ������� ������� ������������, ����� ��� ������] float not null,
	[����������� ��������� ������������� ���� �������] float not null,
	[����������� ����, ��� ������ �������� � �������] float not null,
	[������� ����� ������ � �������] float not null,
	[������� ����� �������� ������ � ������� � �������] float not null,
	[������� ����� ���������� ������ � ��� � �������] float not null,
	[������� ����� ������� ������������� �������] float not null,
	[������� ����� ��������� �������] float not null,
	[����������� ��������� ������� ������������] float not null,
	[������� ����� ������ � ���] float not null
);
go

create procedure [���������������������] @date datetime, @problemStatement nvarchar(max) as
begin
	insert into [�����������] values
	(@date, @problemStatement)
end;
go

create procedure [��������������������] @id int, @date datetime, @problemStatement nvarchar(max) as
begin
	update [�����������]
	set 
	[���� ������������] = @date,
	[���������� ������] = @problemStatement
	where Id = @id
end;
go

create procedure [������������������������] @expId int, @countChannels int, @inputStream float, @avgTimeService float, @intenseStreaService float = 0, @intenseLoad float = 0, @workingDay int = null, @requiredService float = null, @lengthQueue int = null as
begin
	insert into [�������� ������] values
	(@expId, @countChannels, @inputStream, @avgTimeService, @intenseStreaService, @intenseLoad, @workingDay, @requiredService, @lengthQueue)
end;
go

create procedure [�����������������������] @id int, @expId int, @countChannels int, @inputStream float, @avgTimeService float, @intenseStreaService float = 0, @intenseLoad float = 0, @workingDay int = null, @requiredService float = null, @lengthQueue int = null as
begin
	update [�������� ������]
	set 
	[����������� Id] = @expId,
	[����� �������] = @countChannels,
	[������������� �������� ������ (����� ��������)] = @inputStream,
	[������� ����� ������������ ����� ������ � �������] = @avgTimeService,
	[������������� ������ ������������] = @intenseStreaService,
	[������������� ��������] = @intenseLoad,
	[����������������� �������� ��� � �����] = @workingDay,
	[��������� ����������� ������������] = @requiredService,
	[����� �������] = @lengthQueue
	where Id = @id
end;
go

create procedure [�������������������������] @initDataId int, @probabilityDowntime float, @probabilityRejection float, @probabilityService float, @avgCountBusy float, @percentageChannels float, @absProb float as
begin
	insert into [������� � �������] values
	(@initDataId, @probabilityDowntime, @probabilityRejection, @probabilityService, @avgCountBusy, @percentageChannels, @absProb)
end;
go


create procedure [������������������������] @id int, @initDataId int, @probabilityDowntime float, @probabilityRejection float, @probabilityService float, @avgCountBusy float, @percentageChannels float, @absProb float as
begin
	update [������� � �������]
	set 
	[�������� ������ Id] = @initDataId,
	[����������� ������� ������� ������������, ����� ��� ������] = @probabilityDowntime,
	[����������� ������ � ������������, ����� ����������� �� ������������ ������ ������ ��� ������ ��������] = @probabilityRejection,
	[����������� ������������] = @probabilityService,
	[������� ����� ������� ������������� �������] = @avgCountBusy,
	[���� �������, ������� �������������] = @percentageChannels,
	[���������� ���������� ����������� ���] = @absProb
	where Id = @id
end;
go

create procedure [�����������������������������������] @initDataId int, @probabilityDowntime float, @probabilityRejection float, @probabilityService float, @absProb float, @avgCountBusy float, @avgCountRequest float, @avgTimeOjid float, @avgCountRequestSys float, @avgTimeSys float as
begin
	insert into [������� � ������������ �����] values
	(@initDataId, @probabilityDowntime, @probabilityRejection, @probabilityService, @absProb, @avgCountBusy, @avgCountRequest, @avgTimeOjid, @avgCountRequestSys, @avgTimeSys)
end;
go

create procedure [����������������������������������] @id int, @initDataId int, @probabilityDowntime float, @probabilityRejection float, @probabilityService float, @absProb float, @avgCountBusy float, @avgCountRequest float, @avgTimeOjid float, @avgCountRequestSys float, @avgTimeSys float as
begin
	update [������� � ������������ �����]
	set 
	[�������� ������ Id] = @initDataId,
	[����������� ������� ������� ������������, ����� ��� ������] = @probabilityDowntime,
	[����������� ������ � ������������] = @probabilityRejection,
	[����������� ������������] = @probabilityService,
	[���������� ���������� �����������] = @absProb,
	[������� ����� ������� �������] = @avgCountBusy,
	[������� ����� ������ � �������] = @avgCountRequest,
	[������� ����� �������� ������������ � �������] = @avgTimeOjid,
	[������� ����� ������ � �������] = @avgCountRequestSys,
	[������� ����� ���������� � ������� � �������] = @avgTimeSys
	where Id = @id
end;
go

create procedure [�����������������������������������������] @initDataId int, @probabilityDowntime float, @probabilityRejection float, @probabilityService float, @avgCountBusy float, @avgCountRequest float, @avgTimeOjid float, @avgCountRequestSys float, @avgTimeSys float, @coefBusy float, @abgCountReq float as
begin
	insert into [������� � �������������� ���������] values
	(@initDataId, @probabilityDowntime, @probabilityRejection, @probabilityService, @avgCountBusy, @avgCountRequest, @avgTimeOjid, @avgCountRequestSys, @avgTimeSys, @coefBusy, @abgCountReq)
end;
go

create procedure [����������������������������������������] @id int, @initDataId int, @probabilityDowntime float, @probabilityRejection float, @probabilityService float, @avgCountBusy float, @avgCountRequest float, @avgTimeOjid float, @avgCountRequestSys float, @avgTimeSys float, @coefBusy float, @abgCountReq float as
begin
	update [������� � �������������� ���������]
	set 
	[�������� ������ Id] = @initDataId,
	[����������� ������� ������� ������������, ����� ��� ������] = @probabilityDowntime,
	[����������� ��������� ������������� ���� �������] = @probabilityRejection,
	[����������� ����, ��� ������ �������� � �������] = @probabilityService,
	[������� ����� ������ � �������] = @avgCountBusy,
	[������� ����� �������� ������ � ������� � �������] = @avgCountRequest,
	[������� ����� ���������� ������ � ��� � �������] = @avgTimeOjid,
	[������� ����� ������� ������������� �������] = @avgCountRequestSys,
	[������� ����� ��������� �������] = @avgTimeSys,
	[����������� ��������� ������� ������������] = @coefBusy,
	[������� ����� ������ � ���] = @abgCountReq
	where Id = @id
end;