
-- ===================================================================  
-- Author:		RAFAEL PORTAL MAGNA IBM
-- Create date: NOVEMBRO 2018
-- Description: Procedure para retornar a obs da despesa
-- =================================================================== 

CREATE PROCEDURE [dbo].[PR_TIPO_DESPESA_CONSULTAR_POR_ID]
	@id_despesa_tipo int = null
	
AS  
BEGIN  
  
 SET NOCOUNT ON;  
  
	SELECT 
	id_despesa_tipo
	,cd_despesa_tipo 
	,ds_despesa_tipo
	FROM [pagamento].[tb_despesa_tipo] (NOLOCK)
	WHERE
		( NULLIF( @id_despesa_tipo, 0 ) IS NULL OR id_despesa_tipo = @id_despesa_tipo )
		
END;