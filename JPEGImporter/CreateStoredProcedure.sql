use JPEGImporter
go

/* Consider using ALTER PROC instead of drop/create */
IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[SaveImage]') and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
BEGIN
    DROP PROCEDURE SaveImage
END
go

CREATE PROCEDURE SaveImage
(
    @Filename   nvarchar(256),
    @Image      varbinary(MAX)
)
AS
BEGIN
    SET NOCOUNT ON
    SET XACT_ABORT ON

    INSERT INTO [dbo].[Images] (
        [Filename],
        [Image]
    ) VALUES (
        @Filename,
        @Image
    )
END
go

use master
go
