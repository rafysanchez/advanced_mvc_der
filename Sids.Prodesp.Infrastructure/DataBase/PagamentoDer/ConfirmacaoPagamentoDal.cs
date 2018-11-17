using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using Sids.Prodesp.Infrastructure.Helpers;
using Sids.Prodesp.Model.Interface.PagamentoContaDer;
using Sids.Prodesp.Model.Entity.PagamentoContaUnica.PagamentoDer;
using Sids.Prodesp.Model.Entity.PagamentoContaUnica.AutorizacaoDeOB;
using Sids.Prodesp.Model.Entity.PagamentoContaUnica.ProgramacaoDesembolsoExecucao;
using System.Linq;

namespace Sids.Prodesp.Infrastructure.DataBase.PagamentoDer
{
    public class ConfirmacaoPagamentoDal : ICrudConfirmacaoPagamento
    {
        #region Atributos
        public ConfirmacaoPagamentoDal() { }

        public string GetTableName()
        {
            return "tb_confirmacao_pagamento";
        }
        #endregion

        #region Migração
        public int Add(ConfirmacaoPagamento entity)
        {
            var sqlParameterList = DataHelper.GetSqlParameterList(entity, new string[] { "@id_confirmacao_pagamento", "@nr_agrupamento", "@id_confirmacao_pagamento_item", "@id_confirmacao_pagamento_tipo" });
            return DataHelper.Get<int>("[dbo].[PR_CONFIRMACAO_PAGAMENTO_INCLUIR]", sqlParameterList);
        }

        public int Edit(ConfirmacaoPagamento entity)
        {
            var paramId = new SqlParameter("@id_confirmacao_pagamento", entity.Id);
            var paramIdExecucaoPd = new SqlParameter("@id_execucao_pd", entity.IdExecucaoPD);
            var paramIdAutorizacaoOB = new SqlParameter("@id_autorizacao_ob", entity.IdAutorizacaoOB);
            var paramIdTipoDocumento = new SqlParameter("@id_tipo_documento", entity.IdTipoDocumento);
            var paramTipoPagamento = new SqlParameter("@id_tipo_pagamento", entity.TipoPagamento);
            var paramNumeroConta = new SqlParameter("@nr_conta", entity.NumeroConta);
            var paramNumeroDocumento = new SqlParameter("@nr_documento", entity.NumeroDocumento);
            var paramAnoReferencia = new SqlParameter("@ano_referencia", entity.AnoReferencia);
            var paramDataCadastro = new SqlParameter("@dt_cadastro", entity.DataCadastro);
            var paramDataConfirmacao = new SqlParameter("@dt_confirmacao", entity.DataConfirmacao);
            var paramDataPreparacao = new SqlParameter("@dt_preparacao", entity.DataPreparacao);
            var paramAgrupamento = new SqlParameter("@nr_agrupamento", entity.CodigoAgrupamentoConfirmacaoPagamento);
            var paramValorTotalConfirmado = new SqlParameter("@vr_total_confirmado", entity.ValorTotalConfirmado);
            var paramStatusProdesp = new SqlParameter("@cd_transmissao_status_prodesp", entity.StatusProdesp);
            var paramTransmitidoProdesp = new SqlParameter("@fl_transmissao_transmitido_prodesp", entity.TransmitidoProdesp);
            var paramDataTransmitidoProdesp = new SqlParameter("@dt_transmissao_transmitido_prodesp", entity.DataTransmitidoProdesp);
            var paramMensagemServicoProdesp = new SqlParameter("@ds_transmissao_mensagem_prodesp", entity.MensagemServicoProdesp);

            var dbResult = DataHelper.Get<int>("[dbo].[PR_CONFIRMACAO_PAGAMENTO_ALTERAR]", paramId, paramIdExecucaoPd, paramIdAutorizacaoOB, paramIdTipoDocumento, paramTipoPagamento, paramNumeroConta, paramNumeroDocumento,
                paramAnoReferencia, paramDataCadastro, paramDataConfirmacao, paramDataPreparacao, paramAgrupamento, paramValorTotalConfirmado, 
                paramStatusProdesp, paramTransmitidoProdesp, paramDataTransmitidoProdesp, paramMensagemServicoProdesp);

            return dbResult;
        }

        public IEnumerable<ConfirmacaoPagamento> Fetch(ConfirmacaoPagamento entity)
        {
            //var sqlParameterList = DataHelper.GetSqlParameterList(entity, new string[] { "@ds_transmissao_mensagem_prodesp", "@fl_transmissao_transmitido_prodesp", "@dt_transmissao_transmitido_prodesp", "@cd_transmissao_status_prodesp", "@id_confirmacao_pagamento_item", "@id_confirmacao_pagamento_tipo", "@vr_total_confirmado" });

            var paramId = new SqlParameter("@id_confirmacao_pagamento", entity.Id);
            var paramIdExecucaoPd = new SqlParameter("@id_execucao_pd", entity.IdExecucaoPD);
            var paramIdAutorizacaoOB = new SqlParameter("@id_autorizacao_ob", entity.IdAutorizacaoOB);
            var paramIdTipoDocumento = new SqlParameter("@id_tipo_documento", entity.IdTipoDocumento);
            var paramAnoReferencia = new SqlParameter("@ano_referencia", entity.AnoReferencia);
            var paramNumeroDocumento = new SqlParameter("@nr_documento", entity.NumeroDocumento);
            var paramTipoPagamento = new SqlParameter("@id_tipo_pagamento", entity.TipoPagamento);
            var paramDataConfirmacao = new SqlParameter("@dt_confirmacao", entity.DataConfirmacao);
            var paramDataPreparacao = new SqlParameter("@dt_preparacao", entity.DataPreparacao);
            var paramNumeroConta = new SqlParameter("@nr_conta", entity.NumeroConta);
            var paramDataCadastro = new SqlParameter("@dt_cadastro", entity.DataCadastro);
            var paramDataModificacao = new SqlParameter("@dt_modificacao", entity.DataModificacao);
            var paramAgrupamento = new SqlParameter("@nr_agrupamento", entity.CodigoAgrupamentoConfirmacaoPagamento);

            var dbResult = DataHelper.List<ConfirmacaoPagamento>("[dbo].[PR_CONFIRMACAO_PAGAMENTO_CONSULTAR]", paramId, paramIdExecucaoPd, paramIdAutorizacaoOB,
                paramIdTipoDocumento, paramAnoReferencia, paramNumeroDocumento, paramTipoPagamento, paramDataConfirmacao, paramDataPreparacao, paramNumeroConta,
                paramDataCadastro, paramDataModificacao, paramAgrupamento);

            return dbResult;
        }

        public ConfirmacaoPagamento Get(int? idExecucaoPd, int? idAutorizacaoOB)
        {
            return DataHelper.Get<ConfirmacaoPagamento>("[dbo].[PR_CONFIRMACAO_PAGAMENTO_CONSULTAR]",
                new SqlParameter("@id_execucao_pd", idExecucaoPd),
                new SqlParameter("@id_autorizacao_ob", idAutorizacaoOB));
        }

        public int Add(PDExecucao entity)
        {
            return DataHelper.Get<int>("PR_CONFIRMACAO_PAGAMENTO_INCLUIR",
              new SqlParameter("@id_tipo_documento", entity.DocumentoTipoId),
              new SqlParameter("@ano_referencia", entity.Ano),
              new SqlParameter("@nr_documento", entity.NumeroDocumento),
              new SqlParameter("@id_tipo_pagamento", entity.TipoPagamento),
              new SqlParameter("@dt_confirmacao", entity.DataConfirmacao.ValidateDBNull()),
              new SqlParameter("@dt_cadastro", entity.DataCadastro.ValidateDBNull()),
              new SqlParameter("@cd_transmissao_status_prodesp", entity.StatusProdesp),
              new SqlParameter("@fl_transmissao_transmitido_prodesp", entity.TransmitidoProdesp),
              new SqlParameter("@dt_transmissao_transmitido_prodesp", entity.DataTransmitidoProdesp.ValidateDBNull()),
              new SqlParameter("@ds_transmissao_mensagem_prodesp", entity.MensagemServicoProdesp));
        }

        public int Add(OBAutorizacao entity)
        {
            return DataHelper.Get<int>("PR_CONFIRMACAO_PAGAMENTO_INCLUIR",
              new SqlParameter("@id_tipo_documento", entity.DocumentoTipoId),
              new SqlParameter("@ano_referencia", entity.AnoOB),
              new SqlParameter("@nr_documento", entity.NumeroDocumento),
              new SqlParameter("@id_tipo_pagamento", entity.TipoPagamento),
              new SqlParameter("@dt_confirmacao", entity.DataConfirmacaoCombo.ValidateDBNull()),
              new SqlParameter("@dt_cadastro", entity.DataCadastro),
              new SqlParameter("@cd_transmissao_status_prodesp", entity.StatusProdesp),
              new SqlParameter("@fl_transmissao_transmitido_prodesp", entity.TransmitidoProdesp),
              new SqlParameter("@dt_transmissao_transmitido_prodesp", entity.DataTransmitidoProdesp.ValidateDBNull()),
              new SqlParameter("@ds_transmissao_mensagem_prodesp", entity.MensagemServicoProdesp));
        }

        #endregion

        #region ConfirmacaoPagamento
        public IEnumerable<ConfirmacaoPagamento> FetchForGrid(ConfirmacaoPagamento entity)
        {
            var paramId_tipo_documento = new SqlParameter("@id_tipo_documento", entity.IdTipoDocumento);
            var paramNr_documento = new SqlParameter("@nr_documento", entity.NumeroDocumento);
            var paramNr_conta = new SqlParameter("@nr_conta", entity.NumeroConta);
            var paramDt_preparacao = new SqlParameter("@dt_preparacao", entity.DataPreparacao);
            var paramId_tipo_pagamento = new SqlParameter("@id_tipo_pagamento", entity.TipoPagamento);
            var paramNr_orgao = new SqlParameter("@nr_orgao", entity.Orgao);
            var paramId_despesa_tipo = new SqlParameter("@id_despesa_tipo", entity.TipoDespesa);
            var paramNr_contrato = new SqlParameter("@nr_contrato", entity.NumeroContrato);
            var paramCd_obra = new SqlParameter("@cd_obra", entity.CodigoObra);
            var paramId_origem = new SqlParameter("@id_origem", entity.Origem);
            var paramNm_reduzido_credor = new SqlParameter("@nm_reduzido_credor", entity.NomeReduzidoCredor);
            var paramCd_cpf_cnpj = new SqlParameter("@cd_cpf_cnpj", entity.CPF_CNPJ);
            var paramDt_confirmacao = new SqlParameter("@dt_confirmacao", entity.DataConfirmacao);
            var paramStatus_prodesp = new SqlParameter("@status_prodesp", entity.StatusProdesp);
            var paramDt_cadastramento_de = new SqlParameter("@dt_cadastramento_de", entity.DataCadastroDe);
            var paramDt_cadastramento_ate = new SqlParameter("@dt_cadastramento_ate", entity.DataCadastroAte);

            var dbResult = DataHelper.List<ConfirmacaoPagamento>("PR_CONFIRMACAO_PAGAMENTO_CONSULTAR_GRID", paramId_tipo_documento, paramNr_documento, paramNr_conta, paramDt_preparacao,
                paramId_tipo_pagamento, paramNr_orgao, paramId_despesa_tipo, paramNr_contrato, paramCd_obra, paramId_origem, paramNm_reduzido_credor, paramCd_cpf_cnpj, paramDt_confirmacao,
                paramStatus_prodesp, paramDt_cadastramento_de, paramDt_cadastramento_ate);

            return dbResult;
        }

        public int SavePayment(ConfirmacaoPagamento entity, string tipo)
        {
            int retorno = 0;
            if (tipo == "Pai")
            {
                var paramTipoConfirmacao = new SqlParameter("@id_confirmacao_pagamento_tipo", entity.TipoConfirmacao);
                var paramConta = new SqlParameter("@nr_conta", entity.NumeroConta);
                var paramDataPreparacao = new SqlParameter("@dt_preparacao", entity.DataPreparacao);
                var paramTipoPagamento = new SqlParameter("@id_tipo_pagamento", entity.TipoPagamento);
                var paramDataConfirmacao = new SqlParameter("@dt_confirmacao", entity.DataConfirmacao);
                var paramTipoDocumento = new SqlParameter("@id_tipo_documento", entity.TipoDocumento);
                var paramNumeroDocumento = new SqlParameter("@nr_documento", entity.NumeroDocumento);
                var paramAnoReferencia = new SqlParameter("@ano_referencia", entity.AnoReferencia);

                retorno = DataHelper.Get<int>("dbo.PR_CONFIRMACAO_PAGAMENTO_INCLUIR_FILTRO", paramTipoConfirmacao, paramConta, paramDataPreparacao, paramTipoPagamento,
                    paramDataConfirmacao, paramTipoDocumento, paramNumeroDocumento, paramAnoReferencia);
            }
            else
            {
                retorno = DataHelper.Get<int>("dbo.sp_confirmacao_pagamento_incluir",
                 new SqlParameter("@id_confirmacao_pagamento", entity.Id),
                 new SqlParameter("@id_execucao_pd", entity.IdExecucaoPD),
                 new SqlParameter("@id_programacao_desembolso_execucao_item", null),
                 new SqlParameter("@id_autorizacao_ob", entity.IdAutorizacaoOB),
                 new SqlParameter("@id_autorizacao_ob_item", entity.IdAutorizacaoOB),
                 new SqlParameter("@dt_confirmacao", entity.DataConfirmacao),

                 new SqlParameter("@id_tipo_documento", entity.TipoDocumento),
                 new SqlParameter("@nr_documento", entity.NumeroDocumento),

                 new SqlParameter("@id_regional", entity.RegionalId),

                 new SqlParameter("@id_reclassificacao_retencao", null),
                 new SqlParameter("@id_origem", Convert.ToInt16(entity.OrigemConfirmacao)),
                 new SqlParameter("@id_despesa_tipo", entity.TipoDespesa),
                 new SqlParameter("@dt_vencimento", Convert.ToDateTime(entity.DataVencimento)),
                 new SqlParameter("@nr_contrato", entity.NumeroContrato),
                 new SqlParameter("@cd_obra", entity.CodigoObra),
                 new SqlParameter("@nr_op", entity.NumeroOP),
                 new SqlParameter("@nr_banco_pagador", entity.BancoPagador),
                 new SqlParameter("@nr_agencia_pagador", entity.AgenciaPagador),
                 new SqlParameter("@nr_conta_pagador", entity.ContaPagador),
                 new SqlParameter("@nr_banco_favorecido", entity.BancoFavorecido),
                 new SqlParameter("@nr_agencia_favorecido", entity.AgenciaFavorecido),
                 new SqlParameter("@nr_conta_favorecido", entity.ContaFavorecido),
                 new SqlParameter("@nr_fonte_siafem", entity.FonteSIAFEM),
                 new SqlParameter("@nr_emprenho", entity.NumeroEmpenho),
                 new SqlParameter("@nr_processo", entity.NumeroProcesso),
                 new SqlParameter("@nr_nota_fiscal", entity.NotaFiscal),
                 new SqlParameter("@nr_nl_documento", entity.NLDocumento),

                 new SqlParameter("@vr_documento", Convert.ToDecimal(entity.ValorDesdobradoCredor)),

                 new SqlParameter("@nr_natureza_despesa", entity.NaturezaDespesa),

                 new SqlParameter("@ds_referencia", entity.Referencia),
                 new SqlParameter("@cd_orgao_assinatura", entity.Orgao),

                 new SqlParameter("@cd_transmissao_status_prodesp", entity.StatusProdesp),
                 new SqlParameter("@fl_transmissao_transmitido_prodesp", entity.TransmitidoProdesp),
                 new SqlParameter("@ds_transmissao_mensagem_prodesp", entity.TransmissaoConfirmacao),
                 new SqlParameter("@dt_transmissao_transmitido_prodesp", entity.DataTransmitidoProdesp),
                 new SqlParameter("@nr_empenhoSiafem", entity.numeroEmpenhoSiafem),

                 new SqlParameter("@cd_credor_organizacao", entity.CredorOrganizacaoCredorDocto),
                 new SqlParameter("@nr_cnpj_cpf_ug_credor", entity.CPF_CNPJ_Credor),
                 new SqlParameter("@nm_reduzido_credor", entity.NomeReduzidoCredor),

                 new SqlParameter("@cd_credor_organizacao_docto", entity.CredorOrganizacaoCredorOriginal),

                 new SqlParameter("@nr_cnpj_cpf_ug_credor_docto", entity.CPFCNPJCredorOriginal),

                 new SqlParameter("@vr_desdobramento", Convert.ToDecimal(entity.ValorDesdobradoCredor)),

                 new SqlParameter("@dt_realizacao", string.IsNullOrEmpty(entity.DataRealizacao) ? new Nullable<DateTime>() : Convert.ToDateTime(entity.DataRealizacao)),
                 new SqlParameter("@nm_reduzido_credor_docto", entity.NomeReduzidoCredorDocto),

                 new SqlParameter("@nr_documento_gerador", entity.NumeroDocumentoItem)
                 );

            }

            return retorno;
        }

        public int SaveFonte(ConfirmacaoPagamento entity)
        {
            return DataHelper.Get<int>("dbo.PR_CONFIRMACAO_PAGAMENTO_INCLUIR_TOTAIS",
             new SqlParameter("@nr_fonte_lista", Convert.ToString(entity.FonteLista)),
             new SqlParameter("@vr_total_confirmar_ir", Convert.ToDecimal(entity.ValorTotalConfirmarIR)),
             new SqlParameter("@vr_total_confirmar_issqn", Convert.ToDecimal(entity.ValorTotalConfirmarISSQN)),
             new SqlParameter("@vr_total_fonte_lista", Convert.ToDecimal(entity.ValorTotalFonte)),
             new SqlParameter("@vr_total_confirmar", Convert.ToDecimal(entity.ValorTotalConfirmado)),
             new SqlParameter("@id_confirmacao_pagamento", entity.Id));


        }

        /// <summary>
        /// Retorna itens da tabela [tb_confirmacao_pagamento_totais] perlo paramentro id_confirmacao_pagamento
        /// rafael sanchez portalk  - magna IBM nov 2018
        /// </summary>
        /// <param name="id"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public IEnumerable<ConfirmacaoPagamentoTotais> Listar(int id = 0)
        {
            return DataHelper.List<ConfirmacaoPagamentoTotais>("dbo.PR_CONFIRMACAO_PAGAMENTO_CONSULTAR_TOTAIS",
                  new SqlParameter("@id_confirmacao_pagamento", id));
                
        }


        public int AlteraValorTotal(ConfirmacaoPagamento entity)
        {
            //atualiza tabela tabela pai valor cofirmado
            return DataHelper.Get<int>("[dbo].[PR_CONFIRMACAO_PAGAMENTO_ALTERAR_VALOR_TOTAL]",
             new SqlParameter("@id_confirmacao_pagamento", entity.Id),
             new SqlParameter("@vr_total_confirmado", Convert.ToDecimal(entity.ValorTotalConfirmado)));
        }


        public int UpdatePayment(ConfirmacaoPagamento entity)
        {
            var paramDataTransmitidoProdesp = new SqlParameter("@dt_transmissao_transmitido_prodesp", entity.DataTransmitidoProdesp);
            var paramMensagemServicoProdesp = new SqlParameter("@ds_transmissao_mensagem_prodesp", entity.MensagemServicoProdesp);
            var paramStatusProdesp = new SqlParameter("@cd_transmissao_status_prodesp", entity.StatusProdesp);
            var paramTransmitidoProdesp = new SqlParameter("@fl_transmissao_transmitido_prodesp", entity.TransmitidoProdesp);
            var paramNumeroDocumentoItem = new SqlParameter("@nr_documento_gerador", entity.NumeroDocumentoItem);
            var paramId = new SqlParameter("@id_confirmacao_pagamento", entity.Id);

            return DataHelper.Get<int>("dbo.PR_CONFIRMACAO_PAGAMENTO_INCLUIR_ITEM_PRODESP", paramDataTransmitidoProdesp, paramMensagemServicoProdesp, paramStatusProdesp,
                paramTransmitidoProdesp, paramNumeroDocumentoItem, paramId);
        }


        public int Remove(int id)
        {
            var paramId = new SqlParameter("@id", id);

            var dbResult = DataHelper.Get<int>("dbo.PR_CONFIRMACAO_PAGAMENTO_EXCLUIR", paramId);

            return dbResult;
        }

        public IEnumerable<ConfirmacaoPagamento> FetchForId(int? id, int? item)
        {

            var paramId = new SqlParameter("@id_confirmacao_pagamento", id);
            var paramItem = new SqlParameter("@id_confirmacao_pagamento_item", item);

            var dbResult = DataHelper.List<ConfirmacaoPagamento>("dbo.PR_CONFIRMACAO_PAGAMENTO_SELECT_ID", paramId, paramItem);

            return dbResult;
        }
        #endregion

        #region Não Implementado

        public IEnumerable<ConfirmacaoPagamento> FetchForGrid(ConfirmacaoPagamento entity, DateTime since, DateTime until)
        {
            throw new NotImplementedException();
        }

        public ConfirmacaoPagamento Get(int id)
        {
            throw new NotImplementedException();
        }

        public int Save(ConfirmacaoPagamento entity)
        {
            throw new NotImplementedException();
        }

        public int retornaIDNLparametro(string texto)
        {
            return DataHelper.Get<int>("dbo.PR_CONSULTAR_TIPO_NL",
               new SqlParameter("@texto", texto));

        }
        #endregion
    }
}
