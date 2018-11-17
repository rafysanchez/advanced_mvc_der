-- ==============================================================  
-- Author:  Rodrigo Ohashi
-- Create date: 06/11/2018
-- Description: Procedure para consulta de origem
-- ==============================================================  
CREATE PROCEDURE [dbo].[PR_CONFIRMACAO_PAGAMENTO_ORIGEM_CONSULTAR]  
@id_origem	INT = NULL,
@ds_origem	VARCHAR(30) = NULL
AS

--DECLARE
--@id_origem	INT = NULL,
--@ds_origem	VARCHAR(30) = NULL

BEGIN  
	
	SET NOCOUNT ON;
	
	SELECT 
		id_origem,
		ds_origem
	FROM [dbSIDS].[pagamento].[tb_origem] ( NOLOCK )
	WHERE
		( NULLIF(@id_origem, 0) IS NULL OR id_origem = @id_origem ) AND
		( @ds_origem IS NULL OR ds_origem = @ds_origem )
	ORDER BY id_origem

END