-- =============================================
-- Author:		rafael
-- Create date: novembro de 2018
-- Description:	retorna o id da tabela tb_nl_parametrizacao
-- parametro de entrada texto do campo ds_nl_tipo da tabela tb_nl_tipo
-- =============================================
CREATE PROCEDURE  PR_CONSULTAR_TIPO_NL
	@texto varchar(100) = null
AS
BEGIN
	declare @idTipo int;
	declare @idNlParamemetro int;

	SET NOCOUNT ON;
	SELECT @idTipo =  [id_nl_tipo]
       FROM [dbSIDS].[pagamento].[tb_nl_tipo] where [ds_nl_tipo] = cast( @texto as varchar) ;

   SELECT  @idNlParamemetro = par.id_nl_parametrizacao
	FROM [dbSIDS].[pagamento].[tb_nl_parametrizacao] par
	where par.id_nl_tipo =  @idTipo ;

	select isnull(@idNlParamemetro , 0) ;
END