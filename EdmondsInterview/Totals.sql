select * from EdmondsInterview.dbo.Course
go

select * from EdmondsInterview.dbo.Student
go

select * from EdmondsInterview.dbo.Year
go

select * from EdmondsInterview.dbo.Class
go

select * from EdmondsInterview.dbo.Quarter
go

select Count(*) as Class from EdmondsInterview.dbo.Class
select Count(*) as Course from EdmondsInterview.dbo.Course
select Count(*) as Quarter from EdmondsInterview.dbo.Quarter
select Count(*) as Student from EdmondsInterview.dbo.Student
select Count(*) as Year from EdmondsInterview.dbo.Year
