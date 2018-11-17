﻿
-- ===================================================================  
-- Author:		Daniel Gomes
-- Alterado:  Rafael Sanchez 11/2018
-- Create date: 15/01/2018
-- Description: Procedure para consultar parametrização de NL
-- =================================================================== 

CREATE PROCEDURE [dbo].[PR_NL_PARAMETRIZACAO_CONSULTAR]
	@id_nl_parametrizacao int = null,
	@id_nl_tipo int = null,
	@ds_observacao varchar(250) = null,
	@bl_transmitir bit = null,
	@nr_favorecida_cnpjcpfug varchar(15) = null,
	@nr_favorecida_gestao int = null,
	@nr_unidade_gestora int = null,
	@id_parametrizacao_forma_gerar_nl int = null,
	@DescricaoTipoNL varchar(200) = null
AS  
BEGIN  

 SET NOCOUNT ON;  
  
	SELECT 
		  
	id_nl_parametrizacao
	,ds_observacao 
	,bl_transmitir
	,nr_favorecida_cnpjcpfug
	,nr_favorecida_gestao
	,nr_unidade_gestora
	,id_parametrizacao_forma_gerar_nl
	,PAR.id_nl_tipo
	,TIPO.ds_nl_tipo as DescricaoTipoNL
	FROM [pagamento].[tb_nl_parametrizacao] PAR (NOLOCK) INNER JOIN [dbSIDS].[pagamento].[tb_nl_tipo] TIPO
	ON PAR.id_nl_tipo = TIPO.id_nl_tipo
	WHERE
		( NULLIF( @id_nl_parametrizacao, 0 ) IS NULL OR id_nl_parametrizacao = @id_nl_parametrizacao )
		AND ( NULLIF( @id_nl_tipo, 0) IS NULL OR PAR.id_nl_tipo = @id_nl_tipo)
        AND ( NULLIF( @ds_observacao, '' ) IS NULL OR ds_observacao LIKE '%' +  @ds_observacao + '%' )
		AND ( @bl_transmitir IS NULL OR bl_transmitir = @bl_transmitir )
		AND ( NULLIF( @nr_favorecida_cnpjcpfug, '' ) IS NULL OR nr_favorecida_cnpjcpfug = @nr_favorecida_cnpjcpfug )
		AND ( NULLIF( @nr_favorecida_gestao, 0 ) IS NULL OR nr_favorecida_gestao = @nr_favorecida_gestao )
		AND ( NULLIF( @nr_unidade_gestora, 0 ) IS NULL OR nr_unidade_gestora = @nr_unidade_gestora )
		AND ( NULLIF( @id_parametrizacao_forma_gerar_nl, 0 ) IS NULL OR id_parametrizacao_forma_gerar_nl = @id_parametrizacao_forma_gerar_nl )
		AND ( NULLIF( @DescricaoTipoNL, '' ) IS NULL OR TIPO.ds_nl_tipo LIKE '%' +  @DescricaoTipoNL + '%' )
	ORDER BY id_nl_parametrizacao
END;

--SELECT * FROM [pagamento].[tb_nl_parametrizacao]