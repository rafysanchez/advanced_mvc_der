
CREATE PROCEDURE [dbo].[PR_CONFIRMACAO_PAGAMENTO_SELECT_ID]
(	
      @id_confirmacao_pagamento int = null,
	  @id_confirmacao_pagamento_item int = null
)
AS
BEGIN

	SELECT 
	   pag.id_confirmacao_pagamento
      ,pag.id_confirmacao_pagamento_tipo
	  ,item.nr_op
	  ,item.cd_orgao_assinatura
	  ,id_despesa_tipo
	  ,item.nr_documento
	  ,item.nm_reduzido_credor
	  ,item.nr_cnpj_cpf_ug_credor
	  ,vr_desdobramento
	  ,item.nr_banco_favorecido
      ,item.nr_agencia_favorecido
      ,item.nr_conta_favorecido
	  ,pag.cd_transmissao_status_prodesp
	  
	  ,CONVERT(varchar(10),item.dt_vencimento, 103) as dt_vencimento
	  ,pag.dt_preparacao
	  ,item.id_tipo_documento

	  ,item.nr_banco_pagador
      ,item.nr_agencia_pagador
      ,item.nr_conta_pagador

   --   ,pag.id_execucao_pd
   --   ,pag.id_autorizacao_ob
   --   ,pag.nr_agrupamento
   --   ,pag.ano_referencia
   --   ,pag.id_tipo_pagamento
   --   ,pag.dt_confirmacao
   --   ,pag.dt_cadastro
   --   ,pag.dt_modificacao
   --   ,pag.vr_total_confirmado
      
   --   ,pag.fl_transmissao_transmitido_prodesp
   --   ,pag.dt_transmissao_transmitido_prodesp
   --   ,pag.ds_transmissao_mensagem_prodesp
   --   ,pag.dt_preparacao
   --   ,pag.nr_conta
      
	  --,item.id_confirmacao_pagamento_item
   --   ,item.id_confirmacao_pagamento
   --   ,item.id_execucao_pd
   --   ,item.id_programacao_desembolso_execucao_item
   --   ,item.id_autorizacao_ob
   --   ,item.id_autorizacao_ob_item
	  --,item.id_tipo_documento
	  
   --   ,item.nr_banco_pagador
   --   ,item.nr_agencia_pagador
   --   ,item.nr_conta_pagador
	  --,item.cd_credor_organizacao 
      
	 
      

   --   ,item.dt_confirmacao
   --   ,item.nr_documento
   --   ,item.id_regional
   --   ,item.id_reclassificacao_retencao
   --   ,item.id_origem

	  --,CONVERT(varchar(10),item.dt_vencimento, 103) as dt_vencimento
   --   ,item.nr_contrato
   --   ,item.cd_obra

   --   ,item.nr_fonte_siafem
   --   ,item.nr_emprenho
   --   ,item.nr_processo
	  --,CONVERT(varchar(20),item.nr_nota_fiscal) as nr_nota_fiscal
   --   ,item.nr_nl_documento
	  --,CONVERT(varchar(10),item.vr_documento) as vr_documento

	  --,CONVERT(varchar(20),item.nr_natureza_despesa) as nr_natureza_despesa

   --   ,item.ds_referencia

   --   ,item.fl_transmissao_transmitido_prodesp
   --   ,item.cd_transmissao_status_prodesp
   --   ,item.dt_transmissao_transmitido_prodesp
   --   ,item.ds_transmissao_mensagem_prodesp
	  --,tot.nr_fonte_lista
	  --,CONVERT(varchar(10),vr_total_fonte_lista) as vr_total_fonte_lista
	  --,CONVERT(varchar(10),vr_total_confirmar_ir) as vr_total_confirmar_ir
	  --,CONVERT(varchar(10),vr_total_confirmar_issqn) as vr_total_confirmar_issqn
	  --,CONVERT(varchar(10),vr_total_confirmar) as vr_total_confirmar
  FROM pagamento.tb_confirmacao_pagamento pag WITH(NOLOCK)
  INNER JOIN pagamento.tb_confirmacao_pagamento_item item WITH(NOLOCK)
  ON pag.id_confirmacao_pagamento = item.id_confirmacao_pagamento
  INNER JOIN pagamento.tb_confirmacao_pagamento_totais tot with(nolock)
  on pag.id_confirmacao_pagamento = tot.id_confirmacao_pagamento
  Where @id_confirmacao_pagamento is null OR pag.id_confirmacao_pagamento = @id_confirmacao_pagamento
    And @id_confirmacao_pagamento_item is null OR item.id_confirmacao_pagamento_item = @id_confirmacao_pagamento_item
  End