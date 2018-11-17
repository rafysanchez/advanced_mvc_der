CREATE PROCEDURE [dbo].[PR_CONFIRMACAO_PAGAMENTO_CONSULTAR_GRID]
@id_tipo_documento INT = NULL,
@nr_documento VARCHAR(30) = NULL,
@nr_conta VARCHAR(3) = NULL,
@dt_preparacao DATETIME = NULL,
@id_tipo_pagamento INT = NULL,
@nr_orgao CHAR(2) = NULL,
@id_despesa_tipo INT = NULL,
@nr_contrato VARCHAR(13) = NULL,
@cd_obra INT = NULL,
@id_origem INT = NULL,
@nm_reduzido_credor VARCHAR(14) = NULL,
@cd_cpf_cnpj VARCHAR(14) = NULL,
@dt_confirmacao DATETIME = NULL,
@status_prodesp CHAR(1) = NULL,
@dt_cadastramento_de DATETIME = NULL, 
@dt_cadastramento_ate DATETIME = NULL
AS

--DECLARE
--@id_tipo_documento INT = NULL,
--@nr_documento VARCHAR(30) = NULL,
--@nr_conta VARCHAR(3) = NULL,
--@dt_preparacao DATETIME = NULL,
--@id_tipo_pagamento INT = NULL,
--@nr_orgao CHAR(2) = NULL,
--@id_despesa_tipo INT = NULL,
--@nr_contrato VARCHAR(13) = NULL,
--@cd_obra INT = NULL,
--@id_origem INT = NULL,
--@nm_reduzido_credor VARCHAR(14) = NULL,
--@cd_cpf_cnpj VARCHAR(14) = NULL,
--@dt_confirmacao DATETIME = NULL,
--@status_prodesp CHAR(1) = NULL,
--@dt_cadastramento_de DATETIME = NULL, 
--@dt_cadastramento_ate DATETIME = NULL

BEGIN
	
	SELECT 
		cp.nr_agrupamento AS CodigoAgrupamentoConfirmacaoPagamento,
		cpi.cd_orgao_assinatura AS Orgao,
		cpi.id_despesa_tipo as id_despesa_tipo,
		cpi.nr_documento AS NumeroDocumento,
		cpi.nm_reduzido_credor AS NomeReduzidoCredor,
		cpi.nr_cnpj_cpf_ug_credor AS CPF_CNPJ_Credor,
		cpi.vr_documento AS ValorDocumentoDecimal,
		cp.dt_confirmacao,
		cpi.id_origem,
		cpi.cd_transmissao_status_prodesp AS StatusProdesp,
		rr.nr_siafem_siafisico AS NumeroNLBaixaRepasse,
		cp.id_confirmacao_pagamento,
		--cp.id_execucao_pd,
		cpi.id_confirmacao_pagamento_item
		--cpi.ds_transmissao_mensagem_prodesp
	FROM [pagamento].[tb_confirmacao_pagamento_item] cpi (NOLOCK)
	LEFT JOIN [pagamento].[tb_confirmacao_pagamento] cp (NOLOCK) ON cp.id_confirmacao_pagamento = cpi.id_confirmacao_pagamento
	LEFT JOIN [contaunica].[tb_reclassificacao_retencao] rr (NOLOCK) ON cpi.id_reclassificacao_retencao = rr.id_reclassificacao_retencao
	--LEFT JOIN [pagamento].[tb_origem] o (NOLOCK) ON cpi.id_origem = o.id_origem
	--LEFT JOIN [seguranca].tb_regional r (NOLOCK) ON cpi.id_regional = r.id_regional
	--LEFT JOIN [contaunica].tb_tipo_documento td (NOLOCK) ON cpi.id_tipo_documento = td.id_tipo_documento
	--LEFT JOIN [contaunica].tb_programacao_desembolso_execucao_tipo_pagamento pdetp (NOLOCK) ON cp.id_tipo_pagamento = pdetp.id_programacao_desembolso_execucao_tipo_pagamento
	WHERE 
		(NULLIF(@id_tipo_documento, 0) IS NULL OR cpi.id_tipo_documento = @id_tipo_documento ) AND
		(NULLIF(@nr_documento, '') IS NULL OR cpi.nr_documento = @nr_documento ) AND
		(NULLIF(@nr_conta, '') IS NULL OR cp.nr_conta = @nr_conta ) AND
		(@dt_confirmacao IS NULL OR cp.dt_confirmacao = @dt_confirmacao ) AND
		(@dt_preparacao IS NULL OR cp.dt_preparacao = @dt_preparacao ) AND
		(@dt_cadastramento_de IS NULL OR cp.dt_cadastro >= @dt_cadastramento_de ) AND  
		(@dt_cadastramento_ate IS NULL OR cp.dt_cadastro < DATEADD(DAY, 1, @dt_cadastramento_ate) ) AND
		(NULLIF(@id_tipo_pagamento, 0) IS NULL OR cp.id_tipo_pagamento = @id_tipo_pagamento ) AND
		(NULLIF(@id_despesa_tipo, 0) IS NULL OR cpi.id_despesa_tipo = @id_despesa_tipo ) AND
		(NULLIF(@nr_contrato, '') IS NULL OR cpi.nr_contrato = @nr_contrato ) AND
		(NULLIF(@id_origem, 0) IS NULL OR cpi.id_origem = @id_origem ) AND
		(NULLIF(@cd_cpf_cnpj, '') IS NULL OR cpi.nr_cnpj_cpf_ug_credor = @cd_cpf_cnpj ) AND
		(NULLIF(@status_prodesp, '') IS NULL OR cpi.cd_transmissao_status_prodesp = @status_prodesp) AND
		(NULLIF(@nr_orgao, '') IS NULL OR NULLIF(@nr_orgao, '00') IS NULL OR cpi.cd_orgao_assinatura = @nr_orgao) AND
		(NULLIF(@cd_obra, 0) IS NULL OR cpi.cd_obra = @cd_obra) AND
		(NULLIF(@nm_reduzido_credor, '') IS NULL OR cpi.nm_reduzido_credor = @nm_reduzido_credor)

END