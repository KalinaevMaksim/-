create database [СМО АЗС]
go

use [СМО АЗС]
go

create table [Эксперимент]
(
	Id int primary key identity,
	[Дата эксперимента] datetime not null,
	[Постановка задачи] nvarchar(max) not null,
)
go

create trigger [ТриггерЭксперимент] on [Эксперимент]
after update, insert as
begin
	declare @data datetime
	declare _cursor1 Cursor for select [Дата эксперимента] from inserted
	open _cursor1
	fetch next from _cursor1 into @data

	while @@FETCH_STATUS = 0
	begin
		if @data > GETDATE()
		begin
			rollback
			raiserror('Дата больше текущей', 16, 2)
		end

		fetch next from _cursor1 into @data
	end

	CLOSE _cursor1
	DEALLOCATE _cursor1
end;
go

create table [Исходные данные] 
(
	Id int primary key identity,
	[Эксперимент Id] int references [Эксперимент](Id) not null,
	[Число каналов] int not null,
	[Интенсивность входного потока (число клиентов)] float not null,
	[Среднее время обслуживания одной заявки в минутах] float not null,
	[Интенсивность потока обслуживания] float not null,
	[Интенсивность нагрузки] float not null,
	[Продолжительность рабочего дня в часах] int null,
	[Требуемая вероятность обслуживания] float null,
	[Длина очереди] int null
)
go

create trigger [ТриггерИсходныеДанные] on [Исходные данные]
after update, insert as
begin
	declare @data int
	declare _cursor2 Cursor for select [Продолжительность рабочего дня в часах] from inserted
	open _cursor2
	fetch next from _cursor2 into @data

	while @@FETCH_STATUS = 0
	begin
		if @data > 24 or @data <= 0
		begin
			rollback
			raiserror('Продолжительность рабочего дня не может быть меньше 0 или больше 24', 16, 2)
		end

		fetch next from _cursor2 into @data
	end

	CLOSE _cursor2
	DEALLOCATE _cursor2
end;
go


create table [Очередь с отказом] 
(
	Id int primary key identity,
	[Исходные данные Id] int references [Исходные данные](Id) not null,
	[Вероятность простоя каналов обслуживания, когда нет заявок] float not null,
	[Вероятность отказа в обслуживании, когда поступившая на обслуживание заявка найдет все каналы занятыми] float not null,
	[Вероятность обслуживания] float not null,
	[Среднее число занятых обслуживанием каналов] float not null,
	[Доля каналов, занятых обслуживанием] float not null,
	[Абсолютная пропускная способность СМО] float not null
);

create table [Очередь с ограничением длины] 
(
	Id int primary key identity,
	[Исходные данные Id] int references [Исходные данные](Id) not null,
	[Вероятность простоя каналов обслуживания, когда нет заявок] float not null,
	[Вероятность отказа в обслуживании] float not null,
	[Вероятность обслуживания] float not null,
	[Абсолютная пропускная способность] float not null,
	[Среднее число занятых каналов] float not null,
	[Среднее число заявок в очереди] float not null,
	[Среднее время ожидания обслуживания в минутах] float not null,
	[Среднее число заявок в системе] float not null,
	[Среднее время пребывания в системе в минутах] float not null

);

create table [Очередь с неограниченным ожиданием] 
(
	Id int primary key identity,
	[Исходные данные Id] int references [Исходные данные](Id) not null,
	[Вероятность простоя каналов обслуживания, когда нет заявок] float not null,
	[Вероятность занятости обслуживанием всех каналов] float not null,
	[Вероятность того, что заявка окажется в очереди] float not null,
	[Среднее число заявок в очереди] float not null,
	[Среднее время ожидания заявки в очереди в минутах] float not null,
	[Среднее время пребывания заявки в СМО в минутах] float not null,
	[Среднее число занятых обслуживанием каналов] float not null,
	[Среднее число свободных каналов] float not null,
	[Коэффициент занятости каналов обслуживания] float not null,
	[Среднее число заявок в СМО] float not null
);
go

create procedure [ДобавлениеЭксперимент] @date datetime, @problemStatement nvarchar(max) as
begin
	insert into [Эксперимент] values
	(@date, @problemStatement)
end;
go

create procedure [ИзменениеЭксперимент] @id int, @date datetime, @problemStatement nvarchar(max) as
begin
	update [Эксперимент]
	set 
	[Дата эксперимента] = @date,
	[Постановка задачи] = @problemStatement
	where Id = @id
end;
go

create procedure [ДобавлениеИсходныеДанные] @expId int, @countChannels int, @inputStream float, @avgTimeService float, @intenseStreaService float = 0, @intenseLoad float = 0, @workingDay int = null, @requiredService float = null, @lengthQueue int = null as
begin
	insert into [Исходные данные] values
	(@expId, @countChannels, @inputStream, @avgTimeService, @intenseStreaService, @intenseLoad, @workingDay, @requiredService, @lengthQueue)
end;
go

create procedure [ИзменениеИсходныеДанные] @id int, @expId int, @countChannels int, @inputStream float, @avgTimeService float, @intenseStreaService float = 0, @intenseLoad float = 0, @workingDay int = null, @requiredService float = null, @lengthQueue int = null as
begin
	update [Исходные данные]
	set 
	[Эксперимент Id] = @expId,
	[Число каналов] = @countChannels,
	[Интенсивность входного потока (число клиентов)] = @inputStream,
	[Среднее время обслуживания одной заявки в минутах] = @avgTimeService,
	[Интенсивность потока обслуживания] = @intenseStreaService,
	[Интенсивность нагрузки] = @intenseLoad,
	[Продолжительность рабочего дня в часах] = @workingDay,
	[Требуемая вероятность обслуживания] = @requiredService,
	[Длина очереди] = @lengthQueue
	where Id = @id
end;
go

create procedure [ДобавлениеОчередьСОтказом] @initDataId int, @probabilityDowntime float, @probabilityRejection float, @probabilityService float, @avgCountBusy float, @percentageChannels float, @absProb float as
begin
	insert into [Очередь с отказом] values
	(@initDataId, @probabilityDowntime, @probabilityRejection, @probabilityService, @avgCountBusy, @percentageChannels, @absProb)
end;
go


create procedure [ИзменениеОчередьСОтказом] @id int, @initDataId int, @probabilityDowntime float, @probabilityRejection float, @probabilityService float, @avgCountBusy float, @percentageChannels float, @absProb float as
begin
	update [Очередь с отказом]
	set 
	[Исходные данные Id] = @initDataId,
	[Вероятность простоя каналов обслуживания, когда нет заявок] = @probabilityDowntime,
	[Вероятность отказа в обслуживании, когда поступившая на обслуживание заявка найдет все каналы занятыми] = @probabilityRejection,
	[Вероятность обслуживания] = @probabilityService,
	[Среднее число занятых обслуживанием каналов] = @avgCountBusy,
	[Доля каналов, занятых обслуживанием] = @percentageChannels,
	[Абсолютная пропускная способность СМО] = @absProb
	where Id = @id
end;
go

create procedure [ДобавлениеОчередьСОграничениемДлины] @initDataId int, @probabilityDowntime float, @probabilityRejection float, @probabilityService float, @absProb float, @avgCountBusy float, @avgCountRequest float, @avgTimeOjid float, @avgCountRequestSys float, @avgTimeSys float as
begin
	insert into [Очередь с ограничением длины] values
	(@initDataId, @probabilityDowntime, @probabilityRejection, @probabilityService, @absProb, @avgCountBusy, @avgCountRequest, @avgTimeOjid, @avgCountRequestSys, @avgTimeSys)
end;
go

create procedure [ИзменениеОчередьСОграничениемДлины] @id int, @initDataId int, @probabilityDowntime float, @probabilityRejection float, @probabilityService float, @absProb float, @avgCountBusy float, @avgCountRequest float, @avgTimeOjid float, @avgCountRequestSys float, @avgTimeSys float as
begin
	update [Очередь с ограничением длины]
	set 
	[Исходные данные Id] = @initDataId,
	[Вероятность простоя каналов обслуживания, когда нет заявок] = @probabilityDowntime,
	[Вероятность отказа в обслуживании] = @probabilityRejection,
	[Вероятность обслуживания] = @probabilityService,
	[Абсолютная пропускная способность] = @absProb,
	[Среднее число занятых каналов] = @avgCountBusy,
	[Среднее число заявок в очереди] = @avgCountRequest,
	[Среднее время ожидания обслуживания в минутах] = @avgTimeOjid,
	[Среднее число заявок в системе] = @avgCountRequestSys,
	[Среднее время пребывания в системе в минутах] = @avgTimeSys
	where Id = @id
end;
go

create procedure [ДобавлениеОчередьСНеограниченнымОжиданием] @initDataId int, @probabilityDowntime float, @probabilityRejection float, @probabilityService float, @avgCountBusy float, @avgCountRequest float, @avgTimeOjid float, @avgCountRequestSys float, @avgTimeSys float, @coefBusy float, @abgCountReq float as
begin
	insert into [Очередь с неограниченным ожиданием] values
	(@initDataId, @probabilityDowntime, @probabilityRejection, @probabilityService, @avgCountBusy, @avgCountRequest, @avgTimeOjid, @avgCountRequestSys, @avgTimeSys, @coefBusy, @abgCountReq)
end;
go

create procedure [ИзменениеОчередьСНеограниченнымОжиданием] @id int, @initDataId int, @probabilityDowntime float, @probabilityRejection float, @probabilityService float, @avgCountBusy float, @avgCountRequest float, @avgTimeOjid float, @avgCountRequestSys float, @avgTimeSys float, @coefBusy float, @abgCountReq float as
begin
	update [Очередь с неограниченным ожиданием]
	set 
	[Исходные данные Id] = @initDataId,
	[Вероятность простоя каналов обслуживания, когда нет заявок] = @probabilityDowntime,
	[Вероятность занятости обслуживанием всех каналов] = @probabilityRejection,
	[Вероятность того, что заявка окажется в очереди] = @probabilityService,
	[Среднее число заявок в очереди] = @avgCountBusy,
	[Среднее время ожидания заявки в очереди в минутах] = @avgCountRequest,
	[Среднее время пребывания заявки в СМО в минутах] = @avgTimeOjid,
	[Среднее число занятых обслуживанием каналов] = @avgCountRequestSys,
	[Среднее число свободных каналов] = @avgTimeSys,
	[Коэффициент занятости каналов обслуживания] = @coefBusy,
	[Среднее число заявок в СМО] = @abgCountReq
	where Id = @id
end;