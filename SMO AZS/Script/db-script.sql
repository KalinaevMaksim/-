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