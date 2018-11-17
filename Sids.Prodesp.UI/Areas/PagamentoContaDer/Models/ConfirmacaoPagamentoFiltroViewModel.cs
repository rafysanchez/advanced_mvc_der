using System;
using System.Web.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Sids.Prodesp.Model.Entity.PagamentoContaUnica.PagamentoDer;
using Sids.Prodesp.Model.Enum;
using System.Linq;
using Sids.Prodesp.Model.Entity.Seguranca;
using Sids.Prodesp.Application;
using Sids.Prodesp.Model.Entity.PagamentoContaUnica.Desdobramento;
using Sids.Prodesp.Model.Entity.PagamentoContaUnica.ProgramacaoDesembolsoExecucao;

namespace Sids.Prodesp.UI.Areas.PagamentoContaDer.Models
{
    public class ConfirmacaoPagamentoFiltroViewModel
    {
        #region Propriedades

        public int Id { get; set; }

        public int id_confirmacao_pagamento_item { get; set; }

        [Display(Name = "Tipo de Documento")]
        public int? IdTipoDocumento { get; set; }

        [Display(Name = "Tipo de Pagamento")]
        public string TipoPagamento { get; set; }

        [Display(Name = "Código da Conta")]
        public string NumeroConta { get; set; }

        [Display(Name = "Nº do Documento")]
        public string NumeroDocumento { get; set; }

        [Display(Name = "Nº da OP")]
        public string NumeroOP { get; set; }

        public int? AnoReferencia { get; set; }

        [Display(Name = "Data da Preparação")]
        public DateTime? DataCadastro { get; set; }

        [Display(Name = "Data da Confirmação")]
        public DateTime? DataConfirmacao { get; set; }

        public string DataConfirmacaoTexto {
            get
            {
                return DataConfirmacao.HasValue? DataConfirmacao.Value.ToString("dd/MM/yyyy") : string.Empty;
            }
        }

        [Display(Name = "Data da Modificação")]
        public DateTime? DataModificacao { get; set; }

        [Display(Name = "Data da Preparação")]
        public DateTime? DataPreparacao { get; set; }

        public string DataPreparacaoTexto
        {
            get
            {
                return DataPreparacao.HasValue ? DataPreparacao.Value.ToString("dd/MM/yyyy") : string.Empty;
            }
        }

        [Display(Name = "Data da Transmissão")]
        public DateTime DataTransmitidoProdesp { get; set; }

        [Display(Name = "Status da Transmissão")]
        public bool TransmitidoProdesp { get; set; }

        public string MensagemServicoProdesp { get; set; }

        [Display(Name = "Status Prodesp")]
        public string StatusProdesp { get; set; }

        public int RegionalId { get; set; }

        [Display(Name = "Nº do Agrupamento")]
        public int? CodigoAgrupamentoConfirmacaoPagamento { get; set; }

        [Display(Name = "Órgão")]
        public string Orgao { get; set; }

        [Display(Name = "Tipo de Despesa")]
        public string TipoDespesa { get; set; }

        [Display(Name = "Tipo de Despesa")]
        public int DespesaTipo { get; set; }

        [Display(Name = "Nº do Contrato")]
        public string NumeroContrato { get; set; }

        [Display(Name = "Cód. Aplicação / Obra")]
        public string CodigoObra { get; set; }

        [Display(Name = "Nome Reduzido Credor")]
        public string NomeReduzidoCredor { get; set; }

        [Display(Name = "CPF/CNPJ Credor")]
        public string CPF_CNPJ { get; set; }

        [Display(Name = "Data de Cadastramento De")]
        public DateTime? DataCadastroDe { get; set; }

        [Display(Name = "Data de Cadastramento Até")]
        public DateTime? DataCadastroAte { get; set; }

        [Display(Name = "Tipo de Pagamento")]
        public IList<SelectListItem> lstTipoPagamento { get; set; }

        [Display(Name = "Origem da Confirmação")]
        public IList<SelectListItem> lstOrigemConfirmacao { get; set; }

        [Display(Name = "Status Prodesp")]
        public IList<SelectListItem> lstStatusProdesp { get; set; }

        [Display(Name = "Valor")]
        public decimal Valor { get; set; }

        public decimal ValorDocumentoDecimal { get; set; }

        [Display(Name = "Origem da Confirmação")]
        public string OrigemConfirmacao { get; set; }

        public int Origem { get; set; }

        [Display(Name = "Nº da NL de Baixa/Repasse")]
        public string NumeroBaixaRepasse { get; set; }

        [Display(Name = "Tipo de Sistema")]
        public string TipoSistema { get; set; }

        [Display(Name = "Banco Pagamento")]
        public string BancoPagador { get; set; }

        [Display(Name = "Agência Pagamento")]
        public string AgenciaPagador { get; set; }

        [Display(Name = "Conta Pagamento")]
        public string ContaPagador { get; set; }

        [Display(Name = "Fonte")]
        public string Fonte { get; set; }

        public string FonteLista { get; set; }

        [Display(Name = "Valor Total Fonte")]
        public string ValorTotalFonte { get; set; }

        [Display(Name = "Valor Total a Confirmar ISSQN")]
        public string ValorTotalConfirmarISSQN { get; set; }

        [Display(Name = "Valor Total a Confirmar IR")]
        public string ValorTotalConfirmarIR { get; set; }

        [Display(Name = "Valor Total a Confirmar")]
        public string ValorTotalConfirmar { get; set; }

        [Display(Name = "Valor Total Confirmado")]
        public string ValorTotalConfirmado { get; set; }

        [Display(Name = "Data de Vencimento")]
        public string DataVencimento { get; set; }

        [Display(Name = "Fonte SIAFEM")]
        public string FonteSIAFEM { get; set; }

        [Display(Name = "Número do Empenho")]
        public string NumeroEmpenho { get; set; }

        [Display(Name = "Número do Processo")]
        public string NumeroProcesso { get; set; }

        [Display(Name = "Nota Fiscal")]
        public string NotaFiscal { get; set; }

        [Display(Name = "Valor do Documento")]
        public string ValorDocumento { get; set; }

        [Display(Name = "Natureza da Despesa")]
        public string NaturezaDespesa { get; set; }

        [Display(Name = "Organização do credor original")]
        public string CredorOrganizacaoCredorOriginal { get; set; }

        [Display(Name = "CPF ou CNPJ do Credor original")]
        public string CPFCNPJCredorOriginal { get; set; }

        [Display(Name = "Credor Original")]
        public string CredorOriginal { get; set; }

        [Display(Name = "Referência")]
        public string Referencia { get; set; }

        [Display(Name = "Credor Organização")]
        public string CredorOrganizacao { get; set; }

        [Display(Name = "CPF ou CNPJ do Credor")]
        public string CPF_CNPJ_Credor { get; set; }

        [Display(Name = "Valor Desdobrado do Credor")]
        public string ValorDesdobradoCredor { get; set; }

        [Display(Name = "Banco do Favorecido")]
        public string BancoFavorecido { get; set; }

        [Display(Name = "Agência do Favorecido")]
        public string AgenciaFavorecido { get; set; }

        [Display(Name = "Conta do Favorecido")]
        public string ContaFavorecido { get; set; }

        [Display(Name = "Status da Transmissão")]
        public string StatusTransmissaoConfirmacao { get; set; }

        [Display(Name = "Erro")]
        public string MensagemErroRetornadaTransmissaoConfirmacaoPagamento { get; set; }

        public List<ConfirmacaoPagamentoFiltroViewModel> PesquisaRetorno { get; set; }

        [Display(Name = "Tipo de Confirmação")]
        public string TipoConfirmacao { get; set; }

        [Display(Name = "Confirmação de Transmissão")]
        public string TransmissaoConfirmacao { get; set; }

        [Display(Name = "Nº da NL de Baixa/Repasse")]
        public string NumeroNLBaixaRepasse { get; set; }

        [Display(Name = "NL do Documento")]
        public string NLDocumento { get; set; }


        [Display(Name = "Sistema")]
        public string Sistema { get; set; }

        [Display(Name = "Tipo de Documento")]
        public string TipoDocumento { get; set; }


        [Display(Name = "Valor Desdobrado")]
        public decimal ValorDesdobradoDecimal { get; set; }

        [Display(Name = "Organização do Credor Original")]
        public int CdCredorOrganizacao { get; set; }

        public string selecionados { get; set; }

        public bool Totalizador { get; private set; }

        public string Tela { get; set; }

        protected IEnumerable<Regional> _regionalServiceList;

        protected IEnumerable<DocumentoTipo> _documentoTipoServiceList;

        protected IEnumerable<ConfirmacaoPagamentoOrigem> _origemServiceList;

        protected IEnumerable<PDExecucaoTipoPagamento> _pagamentoTipoServiceList;

        #endregion

        #region Singleton

        public ConfirmacaoPagamentoFiltroViewModel CreateNewInstance(ConfirmacaoPagamentoFiltroViewModel entrada)
        {
            ConfirmacaoPagamentoFiltroViewModel filtro = new ConfirmacaoPagamentoFiltroViewModel();
            filtro.Id = entrada.Id > 0 ? entrada.Id : 0;
            filtro.id_confirmacao_pagamento_item = entrada.id_confirmacao_pagamento_item > 0 ? entrada.id_confirmacao_pagamento_item : 0;
            filtro.IdTipoDocumento = Convert.ToInt32(entrada.TipoDocumento);
            filtro.NumeroDocumento = entrada.NumeroDocumento;
            filtro.NumeroConta = entrada.NumeroConta;
            filtro.DataCadastro = entrada.DataCadastro;
            filtro.TipoPagamento = entrada.TipoPagamento;
            filtro.Orgao = entrada.Orgao;
            filtro.TipoDespesa = entrada.TipoDespesa;
            filtro.NumeroContrato = entrada.NumeroContrato;
            filtro.CodigoObra = entrada.CodigoObra;
            filtro.CodigoAgrupamentoConfirmacaoPagamento = entrada.CodigoAgrupamentoConfirmacaoPagamento;
            filtro.NomeReduzidoCredor = entrada.NomeReduzidoCredor;
            filtro.CPF_CNPJ = entrada.CPF_CNPJ;
            filtro.DataConfirmacao = entrada.DataConfirmacao;
            filtro.StatusProdesp = entrada.StatusProdesp;
            filtro.DataCadastroDe = entrada.DataPreparacao;
            filtro.DataCadastroAte = entrada.DataModificacao;
            return filtro;
        }

        public ConfirmacaoPagamento CreateNewEditInstance(ConfirmacaoPagamentoFiltroViewModel entrada)
        {
            ConfirmacaoPagamento filtro = new ConfirmacaoPagamento();
            filtro.IdTipoDocumento = entrada.IdTipoDocumento != 0 ? Convert.ToInt32(entrada.IdTipoDocumento) : 0;
            filtro.TipoDocumento = entrada.TipoDocumento != "" && entrada.TipoDocumento != null ? entrada.TipoDocumento : string.Empty;
            filtro.NumeroDocumento = !string.IsNullOrEmpty(entrada.NumeroDocumento) ? entrada.NumeroDocumento : string.Empty;
            filtro.NumeroConta = !string.IsNullOrEmpty(entrada.NumeroConta) ? entrada.NumeroConta : string.Empty;
            filtro.DataCadastro = entrada.DataCadastro != null ? entrada.DataCadastro : null;
            filtro.TipoPagamento = !string.IsNullOrEmpty(entrada.TipoPagamento) ? Convert.ToInt32(entrada.TipoPagamento) : 0;
            filtro.Orgao = !string.IsNullOrEmpty(entrada.Orgao) ? entrada.Orgao : string.Empty;
            filtro.TipoDespesa = !string.IsNullOrEmpty(entrada.TipoDespesa) ? entrada.TipoDespesa : string.Empty;
            filtro.NumeroContrato = !string.IsNullOrEmpty(entrada.NumeroContrato) ? entrada.NumeroContrato : string.Empty;
            filtro.CodigoObra = !string.IsNullOrEmpty(entrada.CodigoObra) ? entrada.CodigoObra : string.Empty;
            filtro.CodigoAgrupamentoConfirmacaoPagamento = !string.IsNullOrEmpty(entrada.CodigoAgrupamentoConfirmacaoPagamento.ToString()) ? entrada.CodigoAgrupamentoConfirmacaoPagamento : 0;
            filtro.NomeReduzidoCredor = !string.IsNullOrEmpty(entrada.NomeReduzidoCredor) ? entrada.NomeReduzidoCredor : string.Empty;
            filtro.CPF_CNPJ = !string.IsNullOrEmpty(entrada.CPF_CNPJ) ? entrada.CPF_CNPJ : string.Empty;
            filtro.DataConfirmacao = !string.IsNullOrEmpty(entrada.DataConfirmacao.ToString()) ? entrada.DataConfirmacao : null;
            filtro.DataPreparacao = !string.IsNullOrEmpty(entrada.DataPreparacao.ToString()) ? entrada.DataPreparacao : null;
            filtro.StatusProdesp = !string.IsNullOrEmpty(entrada.StatusProdesp) ? entrada.StatusProdesp : string.Empty;
            filtro.TipoConfirmacao = !string.IsNullOrEmpty(entrada.TipoConfirmacao) ? Convert.ToInt32(entrada.TipoConfirmacao) : 0;

            return filtro;
        }
        public ConfirmacaoPagamento CreateNewSaveInstance(FormCollection entrada)
        {
            ConfirmacaoPagamento retorno = new ConfirmacaoPagamento();
            retorno.TipoSistema = entrada["TipoSistema"] != null ? entrada["TipoSistema"] : string.Empty;
            retorno.TipoConfirmacao = entrada["TipoConfirmacao"] != null ? Convert.ToInt32(entrada["TipoConfirmacao"]) : 0;
            retorno.TipoDocumento = entrada["TipoDocumento"] != null ? entrada["TipoDocumento"].ToString() : string.Empty;
            retorno.NumeroDocumento = entrada["NumeroDocumento"] != null ? entrada["NumeroDocumento"].ToString() : string.Empty;
            retorno.TipoPagamento = entrada["TipoPagamento"] != null ? Convert.ToInt32(entrada["TipoPagamento"]) : 0;
            retorno.DataConfirmacao = entrada["DataConfirmacao"] != null ? Convert.ToDateTime(entrada["DataConfirmacao"]) : new Nullable<DateTime>();
            retorno.NumeroConta = entrada["NumeroConta"] != null ? entrada["NumeroConta"].ToString() : string.Empty;
            retorno.DataPreparacao = entrada["DataPreparacao"] != null ? Convert.ToDateTime(entrada["DataPreparacao"]) : new Nullable<DateTime>();
            retorno.NumeroOP = entrada["NumeroOP"] != null ? entrada["NumeroOP"].ToString() : string.Empty;
            retorno.Orgao = entrada["Orgao"] != null ? entrada["Orgao"].ToString() : string.Empty;
            retorno.TipoDespesa = entrada["TipoDespesa"] != null ? entrada["TipoDespesa"].ToString() : string.Empty;
            retorno.NomeReduzidoCredor = entrada["NomeReduzidoCredor"] != null ? entrada["NomeReduzidoCredor"].ToString() : string.Empty;
            retorno.CPF_CNPJ = entrada["CPF_CNPJ"] != null ? entrada["CPF_CNPJ"].ToString() : string.Empty;
            retorno.Valor = entrada["Valor"] != null ? entrada["Valor"].ToString() : string.Empty;
            retorno.BancoPagador = entrada["Banco"] != null ? entrada["Banco"].ToString() : string.Empty;
            retorno.AgenciaPagador = entrada["Agencia"] != null ? entrada["Agencia"].ToString() : string.Empty;
            retorno.ContaPagador = entrada["Conta"] != null ? entrada["Conta"].ToString() : string.Empty;
            retorno.TransmissaoConfirmacao = entrada["TransmissaoConfirmacao"] != null ? entrada["TransmissaoConfirmacao"].ToString() : string.Empty;
            retorno.NumeroNLBaixaRepasse = entrada["NumeroNLBaixaRepasse"] != null ? entrada["NumeroNLBaixaRepasse"].ToString() : string.Empty;
            retorno.Fonte = entrada["Fonte"] != null ? entrada["Fonte"].ToString() : string.Empty;
            retorno.ValorTotalConfirmarISSQN = entrada["ValorTotalConfirmarISSQN"] != null ? entrada["ValorTotalConfirmarISSQN"].ToString() : string.Empty;
            retorno.ValorTotalConfirmarIR = entrada["ValorTotalConfirmarIR"] != null ? entrada["ValorTotalConfirmarIR"].ToString() : string.Empty;
            retorno.ValorTotalConfirmado = entrada["ValorTotalConfirmar"] != null ? Convert.ToDecimal(entrada["ValorTotalConfirmar"]) : 0;
            retorno.DataVencimento = entrada["DataVencimento"] != null ? entrada["DataVencimento"].ToString() : string.Empty;
            retorno.DataPreparacao = entrada["DataPreparacao"] != null ? Convert.ToDateTime(entrada["DataPreparacao"].ToString()) : new Nullable<DateTime>();
            retorno.NumeroContrato = entrada["NumeroContrato"] != null ? entrada["NumeroContrato"].ToString() : string.Empty;
            retorno.CodigoObra = entrada["CodigoObra"] != null ? entrada["CodigoObra"].ToString() : string.Empty;
            retorno.FonteSIAFEM = entrada["FonteSIAFEM"] != null ? entrada["FonteSIAFEM"].ToString() : string.Empty;
            retorno.NumeroEmpenho = entrada["NumeroEmpenho"] != null ? entrada["NumeroEmpenho"].ToString() : string.Empty;
            retorno.NumeroProcesso = entrada["NumeroProcesso"] != null ? entrada["NumeroProcesso"].ToString() : string.Empty;
            retorno.NotaFiscal = entrada["NotaFiscal"] != null ? entrada["NotaFiscal"].ToString() : string.Empty;
            retorno.ValorDocumento = entrada["ValorDocumento"] != null ? entrada["ValorDocumento"].ToString() : string.Empty;
            retorno.NaturezaDespesa = entrada["NaturezaDespesa"] != null ? entrada["NaturezaDespesa"].ToString() : string.Empty;
            retorno.CredorOrganizacaoCredorOriginal = entrada["CredorOrganizacaoCredorOriginal"] != null ? entrada["CredorOrganizacaoCredorOriginal"].ToString() : string.Empty;
            retorno.CPFCNPJCredorOriginal = entrada["CPFCNPJCredorOriginal"] != null ? entrada["CPFCNPJCredorOriginal"].ToString() : string.Empty;
            retorno.CredorOriginal = entrada["CredorOriginal"] != null ? entrada["CredorOriginal"].ToString() : string.Empty;
            retorno.Referencia = entrada["Referencia"] != null ? entrada["Referencia"].ToString() : string.Empty;
            retorno.CredorOrganizacao = entrada["CredorOrganizacao"] != null ? entrada["CredorOrganizacao"].ToString() : string.Empty;
            retorno.CPF_CNPJ_Credor = entrada["CPF_CNPJ_Credor"] != null ? entrada["CPF_CNPJ_Credor"].ToString() : string.Empty;
            retorno.ValorDesdobradoCredor = entrada["ValorDesdobradoCredor"] != null ? entrada["ValorDesdobradoCredor"].ToString() : string.Empty;
            retorno.BancoFavorecido = entrada["BancoFavorecido"] != null ? entrada["BancoFavorecido"].ToString() : string.Empty;
            retorno.AgenciaFavorecido = entrada["AgenciaFavorecido"] != null ? entrada["AgenciaFavorecido"].ToString() : string.Empty;
            retorno.ContaFavorecido = entrada["ContaFavorecido"] != null ? entrada["ContaFavorecido"].ToString() : string.Empty;
            retorno.StatusTransmissaoConfirmacao = entrada["StatusTransmissaoConfirmacao"] != null ? entrada["StatusTransmissaoConfirmacao"].ToString() : string.Empty;
            retorno.MensagemErroRetornadaTransmissaoConfirmacaoPagamento = entrada["MensagemErroRetornadaTransmissaoConfirmacaoPagamento"] != null ? entrada["VMensagemErroRetornadaTransmissaoConfirmacaoPagamento"].ToString() : string.Empty;
            return retorno;
        }



        //usada para consulta que fica dentro do botão gravar
        public ConfirmacaoPagamentoFiltroViewModel GenerateFilterFormViewModel(FormCollection entrada)
        {
            ConfirmacaoPagamentoFiltroViewModel ret = new ConfirmacaoPagamentoFiltroViewModel();
            ret.Sistema = entrada["sistemaDer1"] != null ? entrada["sistemaDer1"].ToString() : string.Empty;
            ret.TipoConfirmacao = !string.IsNullOrEmpty(entrada["opcoesConfirmacao"]?.Replace(",", "")) ? entrada["opcoesConfirmacao"].ToString() : string.Empty;
            ret.TipoDocumento = !string.IsNullOrEmpty(entrada["IdTipoDocumento"]?.Replace(",", "")) ? entrada["IdTipoDocumento"].ToString() : string.Empty;
            //ret.NumeroDocumento = !string.IsNullOrEmpty(entrada["NumeroDocumento"]?.Replace(",", "")) ? entrada["NumeroDocumento"].ToString() : string.Empty;
            ret.TipoPagamento = !string.IsNullOrEmpty(entrada["TipoPagamento"]?.Replace(",", "")) ? entrada["TipoPagamento"].ToString().Replace(",", "") : string.Empty;
            ret.DataConfirmacao = !string.IsNullOrEmpty(entrada["DataConfirmacao"]?.Replace(",", "")) ? Convert.ToDateTime(entrada["DataConfirmacao"].ToString()) : default(DateTime);
            ret.NumeroDocumento = !string.IsNullOrEmpty(entrada["NumeroDocumento"]?.Replace(",", "")) ? entrada["NumeroDocumento"].ToString() : string.Empty;
            ret.NumeroConta = !string.IsNullOrEmpty(entrada["NumeroConta"]?.Replace(",", "")) ? entrada["NumeroConta"].ToString() : string.Empty;
            ret.DataPreparacao = !string.IsNullOrEmpty(entrada["DataPreparacao"]?.Replace(",", "")) ? Convert.ToDateTime(entrada["DataPreparacao"].ToString()) : default(DateTime);
            return ret;
        }

        public List<ConfirmacaoPagamentoFiltroViewModel> InitializeFiltroGridViewModel(IEnumerable<ConfirmacaoPagamentoFiltroViewModel> entities)
        {
            List<ConfirmacaoPagamentoFiltroViewModel> items = new List<ConfirmacaoPagamentoFiltroViewModel>();
            foreach (var entity in entities)
            {
                items.Add(new ConfirmacaoPagamentoFiltroViewModel().CreateNewInstance(entity));
            }
            return items;
        }

        public ConfirmacaoPagamento InitializeFiltroGridEditViewModel(ConfirmacaoPagamentoFiltroViewModel entities)
        {
            return new ConfirmacaoPagamentoFiltroViewModel().CreateNewEditInstance(entities);
        }
        public ConfirmacaoPagamentoFiltroViewModel CreateInstance()
        {
            ConfirmacaoPagamentoFiltroViewModel filtro = new ConfirmacaoPagamentoFiltroViewModel();
            return filtro;
        }

        public ConfirmacaoPagamentoFiltroViewModel CreateInstance(ConfirmacaoPagamento entity, FormCollection form)
        {
            ConfirmacaoPagamentoFiltroViewModel filtro = new ConfirmacaoPagamentoFiltroViewModel();

            if (!string.IsNullOrEmpty(form["lstTipoDocumento"]))
            {
                filtro.IdTipoDocumento = Convert.ToInt32(form["lstTipoDocumento"]);
                entity.IdTipoDocumento = Convert.ToInt32(form["lstTipoDocumento"]);
            }
            if (!string.IsNullOrEmpty(form["lstOrigemConfirmacao"]))
            {
                filtro.Origem = Convert.ToInt32(form["lstOrigemConfirmacao"]);
                entity.Origem = Convert.ToInt32(form["lstOrigemConfirmacao"]);
            }
            if (!string.IsNullOrEmpty(form["lstTipoPagamento"]))
            {
                filtro.TipoPagamento = form["lstTipoPagamento"];
                entity.TipoPagamento = Convert.ToInt32(form["lstTipoPagamento"]);
            }
            if (!string.IsNullOrEmpty(form["NumeroDocumento"]))
            {
                filtro.NumeroDocumento = form["NumeroDocumento"];
                entity.NumeroDocumento = form["NumeroDocumento"];
            }
            if (!string.IsNullOrEmpty(form["NumeroConta"]))
            {
                filtro.NumeroConta = form["NumeroConta"];
                entity.NumeroConta = form["NumeroConta"];
            }
            if (!string.IsNullOrEmpty(form["lstOrgao"]))
            {
                filtro.Orgao = form["lstOrgao"];
                entity.Orgao = form["lstOrgao"];
            }
            if (!string.IsNullOrEmpty(form["TipoDespesa"]))
            {
                filtro.TipoDespesa = form["TipoDespesa"];
                entity.TipoDespesa = form["TipoDespesa"];
            }
            if (!string.IsNullOrEmpty(form["NumeroContrato"]))
            {
                filtro.NumeroContrato = form["NumeroContrato"];
                entity.NumeroContrato = form["NumeroContrato"];
            }
            if (!string.IsNullOrEmpty(form["CodigoObra"]))
            {
                filtro.CodigoObra = form["CodigoObra"];
                entity.CodigoObra = form["CodigoObra"];
            }
            if (!string.IsNullOrEmpty(form["NomeReduzidoCredor"]))
            {
                filtro.NomeReduzidoCredor = form["NomeReduzidoCredor"];
                entity.NomeReduzidoCredor = form["NomeReduzidoCredor"];
            }
            if (!string.IsNullOrEmpty(form["CPF_CNPJ"]))
            {
                filtro.CPF_CNPJ = form["CPF_CNPJ"];
                entity.CPF_CNPJ = form["CPF_CNPJ"];
            }
            if (!string.IsNullOrEmpty(form["lstStatusProdesp"]))
            {
                filtro.StatusProdesp = form["lstStatusProdesp"];
                entity.StatusProdesp = form["lstStatusProdesp"];
            }
            if (!string.IsNullOrEmpty(form["DataConfirmacao"]))
            {
                filtro.DataConfirmacao = Convert.ToDateTime(form["DataConfirmacao"]);
                entity.DataConfirmacao = Convert.ToDateTime(form["DataConfirmacao"]);
            }
            if (!string.IsNullOrEmpty(form["DataCadastro"]))
            {
                filtro.DataCadastro = Convert.ToDateTime(form["DataCadastro"]);
                entity.DataCadastro = Convert.ToDateTime(form["DataCadastro"]);
            }
            if (!string.IsNullOrEmpty(form["DataCadastroDe"]))
            {
                filtro.DataCadastroDe = Convert.ToDateTime(form["DataCadastroDe"]);
                entity.DataCadastroDe = Convert.ToDateTime(form["DataCadastroDe"]);
            }
            if (!string.IsNullOrEmpty(form["DataCadastroAte"]))
            {
                filtro.DataCadastroAte = Convert.ToDateTime(form["DataCadastroAte"]);
                entity.DataCadastroAte = Convert.ToDateTime(form["DataCadastroAte"]);
            }
            if (!string.IsNullOrEmpty(form["TipoConfirmacao"]))
            {
                filtro.TipoConfirmacao = Convert.ToString(form["TipoConfirmacao"]);
                entity.TipoConfirmacao = Convert.ToInt32(form["TipoConfirmacao"]);
            }
            if (!string.IsNullOrEmpty(form["DataPreparacao"]))
            {
                filtro.DataPreparacao = Convert.ToDateTime(form["DataPreparacao"]);
                entity.DataPreparacao = Convert.ToDateTime(form["DataPreparacao"]);
            }

            return filtro;
        }

        public ConfirmacaoPagamentoFiltroViewModel CreateInstance(ConfirmacaoPagamento entity)
        {
            ConfirmacaoPagamentoFiltroViewModel filtro = new ConfirmacaoPagamentoFiltroViewModel();

            if (entity.TipoDocumento != null)
            {
                filtro.IdTipoDocumento = Convert.ToInt32(entity.TipoDocumento);
            }

            if (entity.NumeroDocumento != null)
            {
                filtro.NumeroDocumento = entity.NumeroDocumento;
            }

            if (entity.NumeroConta != null)
            {
                filtro.NumeroConta = entity.NumeroConta;
            }

            if (entity.DataCadastro != null)
            {
                filtro.DataCadastro = Convert.ToDateTime(entity.DataCadastro);
            }

            if (entity.DataPreparacao != null)
            {
                filtro.DataPreparacao = Convert.ToDateTime(entity.DataPreparacao);
            }

            if (entity.TipoPagamento != null)
            {
                filtro.TipoPagamento = Convert.ToString(entity.TipoPagamento);
            }

            if (entity.Orgao != null)
            {
                filtro.Orgao = entity.Orgao;
            }

            if (entity.TipoDespesa != null)
            {
                filtro.TipoDespesa = entity.TipoDespesa;
            }

            if (entity.NumeroContrato != null)
            {
                filtro.NumeroContrato = entity.NumeroContrato;
            }

            if (entity.CodigoObra != null)
            {
                filtro.CodigoObra = entity.CodigoObra;
            }

            if (entity.NLDocumento != null)
            {
                filtro.NLDocumento = entity.NLDocumento;
            }

            if (entity.CodigoAgrupamentoConfirmacaoPagamento != null)
            {
                filtro.CodigoAgrupamentoConfirmacaoPagamento = entity.CodigoAgrupamentoConfirmacaoPagamento;
            }

            if (entity.NomeReduzidoCredor != null)
            {
                filtro.NomeReduzidoCredor = entity.NomeReduzidoCredor;
            }

            if (entity.CPF_CNPJ != null)
            {
                filtro.CPF_CNPJ = entity.CPF_CNPJ;
            }

            if (entity.CPF_CNPJ_Credor != null)
            {
                filtro.CPF_CNPJ_Credor = entity.CPF_CNPJ_Credor;
            }

            if (entity.DataConfirmacao != null)
            {
                filtro.DataConfirmacao = entity.DataConfirmacao;
            }

            if (entity.StatusProdesp != null)
            {
                filtro.StatusProdesp = entity.StatusProdesp;
            }

            if (entity.DataCadastroDe != null)
            {
                filtro.DataCadastroDe = entity.DataCadastroDe;
            }

            if (entity.DataCadastroAte != null)
            {
                filtro.DataCadastroAte = entity.DataCadastroAte;
            }

            if (entity.TipoConfirmacao != null)
            {
                filtro.TipoConfirmacao = Convert.ToString(entity.TipoConfirmacao);
            }

            if (entity.Valor != null)
            {
                filtro.Valor = Convert.ToDecimal(entity.Valor);
            }

            if (entity.OrigemConfirmacao != null)
            {
                filtro.OrigemConfirmacao = entity.OrigemConfirmacao;
            }

            if (entity.NumeroNLBaixaRepasse != null)
            {
                filtro.NumeroNLBaixaRepasse = entity.NumeroNLBaixaRepasse;
            }

            if (entity.ValorDocumentoDecimal != null)
            {
                filtro.ValorDocumentoDecimal = entity.ValorDocumentoDecimal;
            }

            if (entity.Origem != null)
            {
                filtro.Origem = Convert.ToInt32(entity.Origem);
            }

            if (entity.DespesaTipo != null)
            {
                filtro.DespesaTipo = entity.DespesaTipo;
            }

            if (entity.MensagemServicoProdesp != null)
            {
                filtro.MensagemServicoProdesp = entity.MensagemServicoProdesp;
            }

            if (entity.Id != null)
            {
                filtro.Id = entity.Id;
            }
            
            if(entity.ValorDesdobradoCredorDecimal != null)
            {
                filtro.ValorDesdobradoDecimal = entity.ValorDesdobradoCredorDecimal;
            }

            if(entity.BancoFavorecido != null)
            {
                filtro.BancoFavorecido = entity.BancoFavorecido;
            }

            if (entity.AgenciaFavorecido != null)
            {
                filtro.AgenciaFavorecido = entity.AgenciaFavorecido;
            }

            if (entity.ContaFavorecido != null)
            {
                filtro.ContaFavorecido = entity.ContaFavorecido;
            }


            if (entity.BancoPagador != null)
            {
                filtro.BancoPagador = entity.BancoPagador;
            }

            if (entity.AgenciaPagador != null)
            {
                filtro.AgenciaPagador = entity.AgenciaPagador;
            }

            if (entity.ContaPagador != null)
            {
                filtro.ContaPagador = entity.ContaPagador;
            }

            if (entity.OrigemConfirmacao != null)
            {
                filtro.OrigemConfirmacao = entity.OrigemConfirmacao;
            }

            if (entity.NumeroOP != null)
            {
                filtro.NumeroOP = entity.NumeroOP;
            }

            if (entity.DataVencimento != null)
            {
                filtro.DataVencimento = entity.DataVencimento;
            }

            if (entity.IdTipoDocumento != null)
            {
                filtro.IdTipoDocumento = entity.IdTipoDocumento;
            }


            if (entity.FonteSIAFEM != null)
            {
                filtro.FonteSIAFEM = entity.FonteSIAFEM;
            }

            if (entity.NumeroEmpenho != null)
            {
                filtro.NumeroEmpenho = entity.NumeroEmpenho;
            }

            if (entity.NumeroProcesso != null)
            {
                filtro.NumeroProcesso = entity.NumeroProcesso;
            }

            if (entity.NotaFiscal != null)
            {
                filtro.NotaFiscal = entity.NotaFiscal;
            }

            if (entity.ValorDocumento != null)
            {
                filtro.ValorDocumento = entity.ValorDocumento;
            }

            if (entity.NaturezaDespesa != null)
            {
                filtro.NaturezaDespesa = entity.NaturezaDespesa;
            }

            if (entity.CdCredorOrganizacao != null)
            {
                filtro.CdCredorOrganizacao = entity.CdCredorOrganizacao;
            }


            if (entity.CPF_CNPJ_Credor != null)
            {
                filtro.CPF_CNPJ_Credor = entity.CPF_CNPJ_Credor;
            }

            if (entity.Referencia != null)
            {
                filtro.Referencia = entity.Referencia;
            }

            if (entity.NomeReduzidoCredor != null)
            {
                filtro.NomeReduzidoCredor = entity.NomeReduzidoCredor;
            }

            if (entity.CredorOrganizacaoCredorOriginal != null)
            {
                filtro.CredorOrganizacaoCredorOriginal = entity.CredorOrganizacaoCredorOriginal;
            }

            if (entity.CPFCNPJCredorOriginal != null)
            {
                filtro.CPFCNPJCredorOriginal = entity.CPFCNPJCredorOriginal;
            }

            if (entity.CredorOriginal != null)
            {
                filtro.CredorOriginal = entity.CredorOriginal;
            }



            return filtro;
        }

        //public List<ConfirmacaoPagamentoFiltroViewModel> CreateInstance(ConfirmacaoPagamento entity)
        //{
        //    List<ConfirmacaoPagamentoFiltroViewModel> ret = new List<ConfirmacaoPagamentoFiltroViewModel>();
        //    ConfirmacaoPagamentoFiltroViewModel filtro = new ConfirmacaoPagamentoFiltroViewModel();
        //    filtro.IdTipoDocumento = entity.TipoDocumento != null ? Convert.ToInt32(entity.TipoDocumento) : 0;
        //    filtro.NumeroDocumento = entity.NumeroDocumento != null ? entity.NumeroDocumento : string.Empty;
        //    filtro.NumeroConta = entity.NumeroConta != null ? entity.NumeroConta.ToString() : string.Empty;
        //    filtro.DataCadastro = entity.DataCadastro != null ? Convert.ToDateTime(entity.DataCadastro) : DateTime.Today;
        //    filtro.TipoPagamento = entity.TipoPagamento != null ? entity.TipoPagamento.ToString() : string.Empty;
        //    filtro.Orgao = entity.Orgao != null ? entity.Orgao : string.Empty;
        //    filtro.TipoDespesa = entity.TipoDespesa != null ? entity.TipoDespesa : string.Empty;
        //    filtro.NumeroContrato = entity.NumeroContrato != null ? entity.NumeroContrato.ToString() : string.Empty;
        //    filtro.CodigoObra = entity.CodigoObra != null ? entity.CodigoObra : string.Empty;
        //    filtro.CodigoAgrupamentoConfirmacaoPagamento = entity.OrigemConfirmacao != null ? Convert.ToInt32(entity.OrigemConfirmacao) : 0;
        //    filtro.NomeReduzidoCredor = entity.NomeReduzidoCredor != null ? entity.NomeReduzidoCredor : string.Empty;
        //    filtro.CPF_CNPJ = entity.CPF_CNPJ != null ? entity.CPF_CNPJ.ToString() : string.Empty;
        //    filtro.DataConfirmacao = entity.DataConfirmacao != null ? Convert.ToDateTime(entity.DataConfirmacao) : DateTime.Today;
        //    filtro.StatusProdesp = entity.StatusProdesp != null ? entity.StatusProdesp.ToString() : string.Empty;
        //    filtro.DataCadastroDe = entity.DataCadastroDe != null ? Convert.ToDateTime(entity.DataCadastroDe) : DateTime.Today;
        //    filtro.DataCadastroAte = entity.DataCadastroAte != null ? Convert.ToDateTime(entity.DataCadastroAte) : DateTime.Today;
        //    filtro.TipoConfirmacao = entity.TipoConfirmacao != null ? entity.TipoConfirmacao.ToString() : string.Empty;
        //    ret.Add(filtro);
        //    return ret;
        //}

        public ConfirmacaoPagamento CreateInstance(FormCollection form)
        {
            ConfirmacaoPagamento filtro = new ConfirmacaoPagamento();
           
            filtro.IdTipoDocumento = !string.IsNullOrWhiteSpace(form["IdTipoDocumento"]) ? Convert.ToInt32(form["IdTipoDocumento"]) : 0;
            filtro.NumeroDocumento = !string.IsNullOrEmpty(form["NumeroDocumento"]) ? form["NumeroDocumento"] : string.Empty;
            filtro.NumeroConta = !string.IsNullOrEmpty(form["NumeroConta"]) ? form["NumeroConta"] : string.Empty;
            filtro.DataCadastro = !string.IsNullOrEmpty(form["DataCadastroDe"]) ? Convert.ToDateTime(form["DataCadastroDe"]) : new Nullable<DateTime>();
            filtro.TipoPagamento = !string.IsNullOrEmpty(form["IdTipoPagamento"]) ? Convert.ToInt32(form["IdTipoPagamento"]) : 0;
            filtro.Orgao = !string.IsNullOrEmpty(form["Orgao"]) ? form["Orgao"] : string.Empty;
            filtro.TipoDespesa = !string.IsNullOrEmpty(form["TipoDespesa"]) ? form["TipoDespesa"] : string.Empty;
            filtro.NumeroContrato = !string.IsNullOrEmpty(form["NumeroContrato"]) ? form["NumeroContrato"] : string.Empty;
            filtro.CodigoObra = !string.IsNullOrEmpty(form["CodigoObra"]) ? form["CodigoObra"] : string.Empty;
            filtro.CodigoAgrupamentoConfirmacaoPagamento = !string.IsNullOrEmpty(form["lstOrigemConfirmacao"]) ? Convert.ToInt32(form["lstOrigemConfirmacao"]) : 0;
            filtro.NomeReduzidoCredor = !string.IsNullOrEmpty(form["NomeReduzidoCredor"]) ? form["NomeReduzidoCredor"] : string.Empty;
            filtro.CPF_CNPJ = !string.IsNullOrEmpty(form["CPF_CNPJ"]) ? form["CPF_CNPJ"] : string.Empty;
            filtro.DataConfirmacao = !string.IsNullOrEmpty(form["DataConfirmacao"]) ? Convert.ToDateTime(form["DataConfirmacao"]) : new Nullable<DateTime>();
            filtro.StatusProdesp = !string.IsNullOrEmpty(form["lstStatusProdesp"]) ? form["lstStatusProdesp"].ToString() : string.Empty;
            filtro.DataCadastroDe = !string.IsNullOrEmpty(form["DataCadastroDe"]) ? Convert.ToDateTime(form["DataCadastroDe"]) : new Nullable<DateTime>();
            filtro.DataCadastroAte = !string.IsNullOrEmpty(form["DataCadastroAte"]) ? Convert.ToDateTime(form["DataCadastroAte"]) : new Nullable<DateTime>();

            filtro.TipoConfirmacao = form["TipoConfirmacao"] != null ? Convert.ToInt32(form["TipoConfirmacao"]) : 0;

            return filtro;
        }

        public ConfirmacaoPagamento CreateInstanceEdit(ConfirmacaoPagamentoFiltroViewModel form)
        {
            ConfirmacaoPagamento filtro = new ConfirmacaoPagamento();
            filtro.IdTipoDocumento = form.IdTipoDocumento > 0 ? Convert.ToInt32(form.IdTipoDocumento) : 0;
            filtro.NumeroDocumento = !string.IsNullOrEmpty(form.NumeroDocumento.ToString()) ? form.NumeroDocumento.ToString() : string.Empty;
            filtro.TipoPagamento = form.TipoPagamento != null ? Convert.ToInt32(form.TipoPagamento) : 0;
            filtro.DataConfirmacao = form.DataConfirmacao != null ? Convert.ToDateTime(form.DataConfirmacao) : DateTime.MinValue;
            filtro.NumeroConta = !string.IsNullOrEmpty(form.NumeroConta.ToString()) ? form.NumeroConta.ToString() : string.Empty;
            filtro.DataPreparacao = form.DataPreparacao != DateTime.MinValue ? form.DataPreparacao : DateTime.MinValue;
            return filtro;
        }

        public List<ConfirmacaoPagamentoFiltroViewModel> MapViewModelToEntityModel(IEnumerable<ConfirmacaoPagamento> entity)
        {
            List<ConfirmacaoPagamentoFiltroViewModel> lstConfirmacaoPagamento = new List<ConfirmacaoPagamentoFiltroViewModel>();
            if (entity != null)
            {
                foreach (var confirmacao in entity)
                {
                    ConfirmacaoPagamentoFiltroViewModel ret = new ConfirmacaoPagamentoFiltroViewModel();
                    ret.Id = confirmacao.Id > 0 ? confirmacao.Id : 0;
                    ret.id_confirmacao_pagamento_item = confirmacao.id_confirmacao_pagamento_item > 0 ? confirmacao.id_confirmacao_pagamento_item : 0;
                    ret.CodigoAgrupamentoConfirmacaoPagamento = confirmacao.CodigoAgrupamentoConfirmacaoPagamento;
                    ret.Orgao = !string.IsNullOrEmpty(confirmacao.Orgao) ? confirmacao.Orgao : string.Empty;
                    ret.TipoDespesa = confirmacao.TipoDespesa != null ? confirmacao.TipoDespesa : string.Empty;
                    ret.NumeroDocumento = confirmacao.NumeroDocumento != null ? confirmacao.NumeroDocumento : string.Empty;
                    ret.NomeReduzidoCredor = !string.IsNullOrEmpty(confirmacao.NomeReduzidoCredor) ? confirmacao.NomeReduzidoCredor : string.Empty;
                    ret.CPF_CNPJ = !string.IsNullOrEmpty(confirmacao.CPF_CNPJ) ? confirmacao.CPF_CNPJ : string.Empty;
                    ret.Valor = confirmacao.ValorConfirmacao > 0 ? Convert.ToDecimal(confirmacao.ValorConfirmacao) : 0;
                    ret.DataConfirmacao = confirmacao.DataConfirmacao;
                    ret.OrigemConfirmacao = confirmacao.OrigemConfirmacao;
                    ret.StatusProdesp = confirmacao.StatusProdesp;
                    ret.NumeroBaixaRepasse = confirmacao.NumeroBaixaRepasse;
                    lstConfirmacaoPagamento.Add(ret);
                }
            }
            return lstConfirmacaoPagamento;
        }

        public List<ConfirmacaoPagamentoFiltroViewModel> EditMapViewModelToEntityModel(List<ConfirmacaoPagamento> entity)
        {
            List<ConfirmacaoPagamentoFiltroViewModel> lstConfirmacaoPagamento = new List<ConfirmacaoPagamentoFiltroViewModel>();
            if (entity != null)
            {
                foreach (var item in entity)
                {
                    ConfirmacaoPagamentoFiltroViewModel vm = new ConfirmacaoPagamentoFiltroViewModel();
                    vm.NumeroOP = item.NumeroOP;
                    vm.Orgao = item.Orgao;
                    vm.TipoDespesa = item.TipoDespesa;
                   
                    vm.NomeReduzidoCredor = item.NomeReduzidoCredor;
                    vm.CPF_CNPJ = item.CPF_CNPJ;
                    vm.Valor = item.ValorConfirmacao;
                    vm.BancoPagador = item.BancoPagador;
                    vm.AgenciaPagador = item.AgenciaPagador;
                    vm.ContaPagador = item.ContaPagador;
                    vm.TransmissaoConfirmacao = item.TransmissaoConfirmacao;
                    vm.NumeroNLBaixaRepasse = item.NLDocumento;
                    vm.Fonte = item.Fonte;
                    vm.FonteLista = item.FonteLista;
                    vm.ValorTotalConfirmarISSQN = item.ValorTotalConfirmarISSQN;
                    vm.ValorTotalConfirmarIR = item.ValorTotalConfirmarIR;
                    vm.ValorTotalConfirmar = item.ValorTotalConfirmado.ToString();
                    vm.DataVencimento = item.DataVencimento;
                    vm.DataPreparacao = item.DataPreparacao ;
                    vm.TipoDocumento = item.TipoDocumentoItem ;

                    vm.NumeroContrato = item.NumeroContrato;

                    if (item.TipoDocumentoItem == "05")
                    {
                        
                        vm.NumeroDocumento = item.NumeroDocumentoItem.Substring(5, 9) + "/" + item.NumeroDocumentoItem.Substring(14, 3);
                        vm.NumeroEmpenho = item.NumeroDocumentoItem.Substring(5, 9);

                    }
                    else if (item.TipoDocumentoItem == "11")
                    {
                        vm.NumeroDocumento = item.NumeroDocumentoItem.Substring(2, 9) + "/" + item.NumeroDocumentoItem.Substring(11, 3) + "/" + item.NumeroDocumentoItem.Substring(14, 3);
                        vm.NumeroEmpenho = item.NumeroDocumentoItem.Substring(2, 9);

                    }
                    else if (item.TipoDocumentoItem == "40")
                    {
                        vm.NumeroDocumento = item.NumeroDocumentoItem.Substring(8, 9);
                        vm.NumeroEmpenho = item.NumeroDocumentoItem.Substring(8, 9);
                    }

                    vm.CodigoObra = item.CodigoObra;
                    vm.FonteSIAFEM = item.FonteSIAFEM;
                    vm.NumeroProcesso = item.NumeroProcesso;
                    vm.NotaFiscal = item.NotaFiscal;
                    vm.ValorDocumento = item.ValorDocumento;
                    vm.NaturezaDespesa = item.NaturezaDespesa;
                    vm.CredorOrganizacaoCredorOriginal = item.CredorOrganizacaoCredorOriginal;
                    vm.CredorOriginal = item.CredorOriginal;
                    vm.Referencia = item.Referencia;
                    vm.CredorOrganizacao = item.CredorOrganizacao;
                    vm.CPFCNPJCredorOriginal = item.CPFCNPJCredorOriginal;
                    vm.CPF_CNPJ_Credor = item.CPF_CNPJ_Credor;
                    vm.ValorDesdobradoCredor = item.ValorDesdobradoCredor;
                    vm.BancoFavorecido = item.BancoFavorecido;
                    vm.AgenciaFavorecido = item.AgenciaFavorecido;
                    vm.ContaFavorecido = item.ContaFavorecido;
                    vm.StatusTransmissaoConfirmacao = item.StatusProdesp;
                    vm.Totalizador = item.Totalizador;

                    vm.CredorOrganizacao = item.NomeReduzidoCredorDocto;



                    vm.ValorTotalFonte = item.ValorTotalFonte;

                    lstConfirmacaoPagamento.Add(vm);
                }
            }
            return lstConfirmacaoPagamento;
        }

        #endregion

        #region Select List
        public IList<SelectListItem> CarregarStatusProdesp()
        {
            return new List<SelectListItem>
           {
                new SelectListItem{Text = "Sucesso", Value = "S"},
                new SelectListItem{Text = "Erro", Value = "E"},
                new SelectListItem{Text = "Não Transmitido", Value = "N"}
            };
        }

        public IList<SelectListItem> CarregarTipoConfirmacaoPagamento()
        {
            return new List<SelectListItem>
           {
                new SelectListItem{Text = "Confirmação de Pagamento Por Documento", Value = "1"},
                new SelectListItem{Text = "Confirmação de Pagamento Por Lote", Value = "2"}
            };
        }

        public IList<SelectListItem> CarregarTipoDocumentoCadastro()
        {
            return (Enum.GetValues(typeof(EnumTipoDocumento)).Cast<EnumTipoDocumento>().Select(
                enu => new SelectListItem() { Text = enu.ToString(), Value = ((int)enu).ToString() })).ToList();

        }
        #endregion

        #region Combo Regional (Órgão)
        protected IEnumerable<Regional> RegionalServiceList
        {
            get
            {
                return _regionalServiceList != null ? _regionalServiceList : (_regionalServiceList = App.RegionalService.Buscar(new Regional()).ToList());
            }
        }

        public IList<SelectListItem> RegionalListItems()
        {
            return RegionalServiceList.Select(s => new SelectListItem
                {
                    Text = s.Descricao,
                    Value = s.Descricao?.Substring(2, 2)
            }).ToList();
        }
        #endregion

        #region Combo Tipo Documento
        protected IEnumerable<DocumentoTipo> DocumentoTipoServiceList
        {
            get
            {
                return _documentoTipoServiceList != null ? _documentoTipoServiceList : (_documentoTipoServiceList = App.DocumentoTipoService.Listar(new DocumentoTipo()).ToList());
            }
        }

        public IList<SelectListItem> DocumentoTipoListItems()
        {
            return DocumentoTipoServiceList.Select(s => new SelectListItem
            {
                Text = s.Descricao,
                Value = s.Id.ToString()
            }).ToList();
        }
        #endregion

        #region Combo Origem da Confirmação
        protected IEnumerable<ConfirmacaoPagamentoOrigem> OrigemServiceList
        {
            get
            {
                return _origemServiceList != null ? _origemServiceList : (_origemServiceList = App.ConfirmacaoPagamentoService.Buscar(new ConfirmacaoPagamentoOrigem()).ToList());
            }
        }

        public IList<SelectListItem> OrigemListItems()
        {
            return OrigemServiceList.Select(s => new SelectListItem
            {
                Text = s.Descricao,
                Value = s.Id.ToString()
            }).ToList();
        }
        #endregion

        #region Combo Tipo Pagamento
        protected IEnumerable<PDExecucaoTipoPagamento> PagamentoTipoServiceList
        {
            get
            {
                return _pagamentoTipoServiceList != null ? _pagamentoTipoServiceList : (_pagamentoTipoServiceList = App.ProgramacaoDesembolsoExecucaoService.TiposPagamento());
            }
        }

        public IList<SelectListItem> PagamentoTipoListItems()
        {
            return PagamentoTipoServiceList.Select(s => new SelectListItem
            {
                Text = s.Descricao,
                Value = s.Codigo.ToString()
            }).ToList();
        }
        #endregion
    }
}