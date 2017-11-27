use master
go

if (select count(name) from sysdatabases where name = 'EdmondsInterview') != 0
BEGIN
	drop database EdmondsInterview
END
go

create database EdmondsInterview
go

use EdmondsInterview
go

create table Student ( 
    SID         varchar(10) primary key
)
go

create table Course (
    DEPT_DIV    varchar(8) NOT NULL,
    COURSE_NUM  varchar(8) NOT NULL,
    CONSTRAINT PK_Course PRIMARY KEY ( DEPT_DIV, COURSE_NUM )
)
go

create table Year (
    YEAR_ID     varchar(16) NOT NULL PRIMARY KEY
)
go

create table Quarter (
    QUARTER_ID          int PRIMARY KEY,
    QuarterDescription  varchar(6)
)
go

insert into Quarter (QUARTER_ID, QuarterDescription ) values ( 1, 'Summer'), (2, 'Fall'), (3, 'Winter' ), ( 4, 'Spring' )
go

create table Class (
    Class_ID	int NOT NULL IDENTITY (1,1) PRIMARY KEY,
    SID         varchar(10) NOT NULL,
    DEPT_DIV    varchar(8) NOT NULL,
    COURSE_NUM  varchar(8) NOT NULL,
    Grade       varchar(6) NOT NULL,
    YEAR_ID     varchar(16) NOT NULL,
    QUARTER_ID  int NOT NULL,
    Credits     varchar(5) NOT NULL,
    Date        DATETIME NOT NULL,
    FOREIGN KEY ( SID ) REFERENCES STUDENT ( SID ),
    FOREIGN KEY ( YEAR_ID ) REFERENCES YEAR ( YEAR_ID ),
    FOREIGN KEY ( QUARTER_ID ) REFERENCES Quarter ( QUARTER_ID ),
    FOREIGN KEY ( DEPT_DIV, COURSE_NUM ) REFERENCES Course( DEPT_DIV, COURSE_NUM )
)
go

use master
go
