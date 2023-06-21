create database NotebookShop;
use NotebookShop;

create table if not exists AboutProject
(
	Id serial primary key not null,
	Header varchar(100) not null,
	Text text not null
);

create table if not exists Admins
(
	Id serial primary key not null,
	Email varchar(50) not null,
	Password varchar(255) not null
);

create table if not exists EmailList
(
	Id serial primary key not null,
	Email varchar(100) not null,
	CreateDate varchar(50) not null,
	CreateTime varchar(50) not null
);

create table if not exists Screens
(
	Id serial primary key not null,
	Model varchar(100) not null,
	Description varchar(255) not null,
	Diagonal int not null,
	ScreenType varchar(70) not null,
	Cost varchar(50) not null,
	Photo varchar(255) null
);

create table if not exists Processors
(
	Id serial primary key not null,
	Model varchar(100) not null,
	Description varchar(255) not null,
	ClockSpeed varchar(50) not null,
	SystemBus varchar(50) not null,
	CasheMemory varchar(50) not null,
	Socket varchar(50) not null,
	Cost varchar(50) not null,
	Photo varchar(255) null
);

create table if not exists VideoCards
(
	Id serial primary key not null,
	Model varchar(100) not null,
	Description varchar(255) not null,
	ClockSpeed varchar(50) not null,
	Memory varchar(50) not null,
	SystemBus varchar(50) not null,
	DirectX varchar(50) not null,
	Interface varchar(50) not null,
	Cost varchar(50) not null,
	Photo varchar(255) null
);

create table if not exists Memories
(
	Id serial primary key not null,
	Model varchar(100) not null,
	Description varchar(255) not null,
	MemorySize varchar(50) not null,
	MemoryType varchar(50) not null,
	Speed varchar(50) not null,
	Cost varchar(50) not null,
	Photo varchar(255) null
);

create table if not exists Motherboards
(
	Id serial primary key not null,
	Model varchar(100) not null,
	Description varchar(255) not null,
	Socket varchar(50) not null,
	Chipset varchar(50) not null,
	BusFrequency varchar(50) not null,
	MotherboardType varchar(50) not null,
	MaxMemory varchar(50) not null,
	CountSlots varchar(50) not null,
	SoundCard varchar(50) not null,
	Cost varchar(50) not null,
	Photo varchar(255) null
);

create table if not exists Winchesters
(
	Id serial primary key not null,
	Model varchar(100) not null,
	Description varchar(255) not null,
	Speed varchar(50) not null,
	Memory varchar(50) not null,
	Slots varchar(50) not null,
	Cost varchar(50) not null,
	Photo varchar(255) null
);

create table if not exists Basket
(
	Id serial primary key not null,
	UserKey varchar(10) not null,
	TableName varchar(50) not null,
	ModelId int not null
);

create table if not exists Orders
(
	Id serial primary key not null,
	Description text not null,
	DateOrder varchar(50) not null,
	Initials varchar(100) not null,
	Address varchar(100) not null,
	Cost varchar(50) not null
);

/*Inserts*/
INSERT INTO admins(id, email, password)
VALUES
(1,'zicshepard@gmail.com','bV/y6gSyacLlN1rIx5sPjQWgXE0BRJvg407gLablTKs=');

INSERT INTO aboutproject(id, header, text)
VALUES 
(1,'Магазин ноутбуков','Идейные соображения высшего порядка, а также начало повседневной работы по формированию позиции влечет за собой процесс внедрения и 
модернизации позиций, занимаемых участниками в отношении поставленных задач. Задача организации, в особенности же рамки и место обучения кадров требуют 
определения и уточнения направлений прогрессивного развития. Таким образом реализация намеченных плановых заданий требуют определения и уточнения 
системы обучения кадров, соответствует насущным потребностям. Идейные соображения высшего порядка, а также консультация с широким активом представляет 
собой интересный эксперимент проверки соответствующий условий активизации. Повседневная практика показывает, что постоянный количественный рост и сфера 
нашей активности способствует подготовки и реализации позиций, занимаемых участниками в отношении поставленных задач.
<br/><br/>
Не следует, однако забывать, что постоянное информационно-пропагандистское обеспечение нашей деятельности влечет за собой процесс внедрения и модернизации
модели развития. С другой стороны реализация намеченных плановых заданий обеспечивает широкому кругу (специалистов) участие в формировании соответствующий 
условий активизации. Идейные соображения высшего порядка, а также укрепление и развитие структуры влечет за собой процесс внедрения и модернизации модели 
развития. Товарищи! постоянный количественный рост и сфера нашей активности способствует подготовки и реализации позиций, занимаемых участниками в отношении 
поставленных задач.
<br/><br/>
Идейные соображения высшего порядка, а также рамки и место обучения кадров обеспечивает широкому кругу (специалистов) участие в формировании модели развития. 
Повседневная практика показывает, что консультация с широким активом требуют от нас анализа существенных финансовых и административных условий. Не следует, 
однако забывать, что новая модель организационной деятельности представляет собой интересный эксперимент проверки новых предложений. Повседневная практика 
показывает, что реализация намеченных плановых заданий позволяет оценить значение модели развития. Товарищи! сложившаяся структура организации играет 
важную роль в формировании модели развития.');

INSERT INTO public.emaillist(email, createdate, createtime)
VALUES 
('text@gmail.com','',''),('text@gmail.com','',''),('text@gmail.com','',''),
('text@gmail.com','',''),('text@gmail.com','',''),('text@gmail.com','',''),
('text@gmail.com','',''),('text@gmail.com','',''),('text@gmail.com','',''),
('text@gmail.com','',''),('text@gmail.com','',''),('text@gmail.com','',''),
('text@gmail.com','',''),('text@gmail.com','',''),('text@gmail.com','',''),
('text@gmail.com','',''),('text@gmail.com','',''),('text@gmail.com','',''),
('text@gmail.com','',''),('text@gmail.com','',''),('text@gmail.com','',''),
('text@gmail.com','',''),('text@gmail.com','',''),('text@gmail.com','',''),
('text@gmail.com','',''),('text@gmail.com','',''),('text@gmail.com','',''),
('text@gmail.com','',''),('text@gmail.com','',''),('text@gmail.com','',''),
('text@gmail.com','',''),('text@gmail.com','',''),('text@gmail.com','',''),
('text@gmail.com','',''),('text@gmail.com','',''),('text@gmail.com','',''),
('text@gmail.com','',''),('text@gmail.com','',''),('text@gmail.com','',''),
('text@gmail.com','',''),('text@gmail.com','',''),('text@gmail.com','',''),
('text@gmail.com','',''),('text@gmail.com','',''),('text@gmail.com','',''),
('text@gmail.com','',''),('text@gmail.com','',''),('text@gmail.com','','');

INSERT INTO public.screens(model, description, diagonal, screentype, cost, photo)
VALUES 
('Model 1','Descripton 1',15,'OLED',100500,'~/images/user/Default.png'),
('Model 2','Descripton 2',15,'OLED',100500,'~/images/user/Default.png'),
('Model 3','Descripton 3',15,'OLED',100500,'~/images/user/Default.png'),
('Model 4','Descripton 4',15,'OLED',100500,'~/images/user/Default.png'),
('Model 5','Descripton 5',15,'OLED',100500,'~/images/user/Default.png'),
('Model 6','Descripton 6',15,'OLED',100500,'~/images/user/Default.png'),
('Model 7','Descripton 7',15,'OLED',100500,'~/images/user/Default.png'),
('Model 8','Descripton 8',15,'OLED',100500,'~/images/user/Default.png'),
('Model 9','Descripton 9',15,'OLED',100500,'~/images/user/Default.png'),
('Model 10','Descripton 10',15,'OLED',100500,'~/images/user/Default.png'),
('Model 11','Descripton 11',15,'OLED',100500,'~/images/user/Default.png'),
('Model 12','Descripton 12',15,'OLED',100500,'~/images/user/Default.png'),
('Model 13','Descripton 13',15,'OLED',100500,'~/images/user/Default.png'),
('Model 14','Descripton 14',15,'OLED',100500,'~/images/user/Default.png'),
('Model 15','Descripton 15',15,'OLED',100500,'~/images/user/Default.png'),
('Model 16','Descripton 16',15,'OLED',100500,'~/images/user/Default.png'),
('Model 17','Descripton 17',15,'OLED',100500,'~/images/user/Default.png'),
('Model 18','Descripton 18',15,'OLED',100500,'~/images/user/Default.png'),
('Model 19','Descripton 19',15,'OLED',100500,'~/images/user/Default.png'),
('Model 20','Descripton 20',15,'OLED',100500,'~/images/user/Default.png');

INSERT INTO public.winchesters(model, description, speed, memory, slots, cost, photo)
VALUES 
('Model 1','Description 1','Speed 1', 'Memory 1','Slots 1',650,'~/images/user/Default.png'),
('Model 2','Description 2','Speed 2','Memory 2','Slots 2',650,'~/images/user/Default.png'),
('Model 3','Description 3','Speed 3','Memory 3','Slots 3',650,'~/images/user/Default.png'),
('Model 4','Description 4','Speed 4','Memory 4','Slots 4',650,'~/images/user/Default.png'),
('Model 5','Description 5','Speed 5','Memory 5','Slots 5',650,'~/images/user/Default.png'),
('Model 6','Description 6','Speed 6','Memory 6','Slots 6',650,'~/images/user/Default.png'),
('Model 7','Description 7','Speed 7','Memory 7','Slots 7',650,'~/images/user/Default.png'),
('Model 8','Description 8','Speed 8','Memory 8','Slots 8',650,'~/images/user/Default.png'),
('Model 9','Description 9','Speed 9','Memory 9','Slots 9',650,'~/images/user/Default.png'),
('Model 10','Description 10','Speed 10','Memory 10','Slots 10',650,'~/images/user/Default.png');
