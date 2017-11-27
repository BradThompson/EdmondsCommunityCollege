use master
go

if (select count(name) from sysdatabases where name = 'JPEGImporter') != 0
BEGIN
    drop database JPEGImporter
END
go

create database JPEGImporter
go

use JPEGImporter
go

create table Images (
    Filename    varchar(255) NOT NULL,
    Image       varbinary(max) NULL,
    CONSTRAINT PK_Filename PRIMARY KEY ( Filename )
)
go

use master
go
