
-- ==============================================================
-- Author:		Luis Santos
-- Create date: 19/09/2016
-- Description:	Procedure para consulta de regional
-- ==============================================================
 CREATE PROCEDURE [dbo].[PR_REGIONAL_CONSULTAR_DESC]
 @ds_regional varchar(2) = null
 
AS

BEGIN

	SET NOCOUNT ON;

	SELECT reg.[id_regional]
      ,reg.[ds_regional]
	  ,reg.[cd_uge]
  FROM [seguranca].[tb_regional] reg (nolock)
  where reg.ds_regional LIKE '%' + @ds_regional + '%' ;
 
END