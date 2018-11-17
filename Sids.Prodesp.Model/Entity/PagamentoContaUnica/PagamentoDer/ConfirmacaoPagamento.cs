using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sids.Prodesp.Model.Entity.PagamentoContaUnica.PagamentoDer
{
    public class ConfirmacaoPagamento
    {
        [Column("id_confirmacao_pagamento")]
        public int Id { get; set; }

        [Column("id_confirmacao_pagamento_item")]
        public int id_confirmacao_pagamento_item { get; set; }

        public string acao { get; set; }

        [Column("id_confirmacao_pagamento_tipo")]
        public int? TipoConfirmacao { get; set; }

        [Column("id_tipo_documento")]
        public int? IdTipoDocumento { get; set; }

        [Column("id_tipo_pagamento")]
        public int? TipoPagamento { get; set; }

        [Column("id_execucao_pd")]
        public int? IdExecucaoPD { get; set; }

        [Column("id_autorizacao_ob")]
        public int? IdAutorizacaoOB { get; set; }

        [Column("nr_conta")]
        public string NumeroConta { get; set; }
    
        [Column("nr_documento")]
        public string NumeroDocumento { get; set; }

        [Column("nr_op")]
        public string NumeroOP { get; set; }

        [Column("ano_referencia")]
        public int? AnoReferencia { get; set; }

        [Column("dt_cadastro")]
        public DateTime? DataCadastro { get; set; }

        [Column("dt_confirmacao")]
        public DateTime? DataConfirmacao { get; set; }

        [Column("dt_modificacao")]
        public DateTime? DataModificacao { get; set; }

        [Column("dt_preparacao")]
        public DateTime? DataPreparacao { get; set; }

        [Column("dt_vencimento")]
        public string DataVencimento { get; set; }

        [Column("vr_total_confirmado")]
        public decimal ValorTotalConfirmado { get; set; }

        [Column("vr_desdobramento")]
        public decimal ValorDesdobradoCredorDecimal { get; set; }

        [Column("fl_transmissao_transmitido_prodesp")]
        public bool TransmitidoProdesp { get; set; }

        [Column("dt_transmissao_transmitido_prodesp")]
        public DateTime? DataTransmitidoProdesp { get; set; }

        [Column("ds_transmissao_mensagem_prodesp")]
        public string MensagemServicoProdesp { get; set; }

        [Column("cd_transmissao_status_prodesp")]
        public string StatusProdesp { get; set; }

        [Column("nr_agrupamento")]
        public int? CodigoAgrupamentoConfirmacaoPagamento { get; set; }

        [Column("cd_orgao_assinatura")]
        public string Orgao { get; set; }

        [Column("id_despesa_tipo")]
        public int DespesaTipo { get; set; }

        [Column("nr_contrato")]
        public string NumeroContrato { get; set; }

        [Column("cd_obra")]
        public string CodigoObra { get; set; }

        [Column("nr_nl_documento")]
        public string NLDocumento { get; set; }

        [Column("nr_fonte_siafem")]
        public string FonteSIAFEM { get; set; }

        [Column("nr_emprenho")]
        public string NumeroEmpenho { get; set; }

        [Column("nr_processo")]
        public string NumeroProcesso { get; set; }

        [Column("nr_nota_fiscal")]
        public string NotaFiscal { get; set; }

        [Column("vr_documento")]
        public decimal ValorDocumentoDecimal { get; set; }

        [Column("nr_natureza_despesa")]
        public string NaturezaDespesa { get; set; }

        [Column("cd_credor_organizacao")]
        public int CdCredorOrganizacao { get; set; }

        [Column("nr_banco_pagador")]
        public string BancoPagador { get; set; }

        [Column("nr_agencia_pagador")]
        public string AgenciaPagador { get; set; }

        [Column("nr_conta_pagador")]
        public string ContaPagador { get; set; }

        [Column("id_origem")]
        public int Origem { get; set; }

        [Column("nr_banco_favorecido")]
        public string BancoFavorecido { get; set; }

        [Column("nr_agencia_favorecido")]
        public string AgenciaFavorecido { get; set; }

        [Column("nr_conta_favorecido")]
        public string ContaFavorecido { get; set; }

        [Column("nr_cnpj_cpf_ug_credor")]
        public string CPF_CNPJ_Credor { get; set; }

        [Column("ds_referencia")]
        public string Referencia { get; set; }

        [Column("nm_reduzido_credor")]
        public string NomeReduzidoCredor { get; set; }

        [Column("cd_credor_organizacao_docto")]
        public string CredorOrganizacaoCredorOriginal { get; set; }

        [Column("nr_cnpj_cpf_ug_credor_docto")]
        public string CPFCNPJCredorOriginal { get; set; }

        [Column("nm_reduzido_credor_docto")]
        public string CredorOriginal { get; set; }



        public IEnumerable<ConfirmacaoPagamentoItem> Items { get; set; }

        [NotMapped]
        public int RegionalId { get; set; }

        [NotMapped]
        public string TipoDespesa { get; set; }

        [NotMapped]
        public string CPF_CNPJ { get; set; }

        [NotMapped]
        public DateTime? DataCadastroDe { get; set; }

        [NotMapped]
        public DateTime? DataCadastroAte { get; set; }

        [NotMapped]
        public decimal ValorConfirmacao { get; set; }

        [NotMapped]
        public string OrigemConfirmacao { get; set; }

        [NotMapped]
        public string NumeroBaixaRepasse { get; set; }

        [NotMapped]
        public string CredorOrganizacao { get; set; }

        [NotMapped]
        public string Valor { get; set; }

        [NotMapped]
        public string TransmissaoConfirmacao { get; set; }

        [NotMapped]
        public string NumeroNLBaixaRepasse { get; set; }

        [NotMapped]
        public string Fonte { get; set; }

        public string FonteLista { get; set; }

        [NotMapped]
        public string ValorTotalConfirmarISSQN { get; set; }

        [NotMapped]
        public string ValorTotalConfirmarIR { get; set; }

        public string ValorTotalFonte { get; set; }

        [NotMapped]
        public string TipoDocumento { get; set; }

        [NotMapped]
        public string ValorDocumento { get; set; }


        [NotMapped]
        public string ValorDesdobradoCredor { get; set; }

        [NotMapped]
        public string Impressora { get; set; }

        [NotMapped]
        public string TipoSistema { get; set; }



        [NotMapped]
        public string StatusTransmissaoConfirmacao { get; set; }

        [NotMapped]
        public string MensagemErroRetornadaTransmissaoConfirmacaoPagamento { get; set; }

        [NotMapped]
        public string Chave { get; set; }

        [NotMapped]
        public string Senha { get; set; }

        [NotMapped]
        public bool Totalizador { get; set; }

        [NotMapped]
        public string CredorOrganizacaoCredorDocto { get; set; }

        [NotMapped]
        public string numeroEmpenhoSiafem { get; set; }

        public string NumeroDocumentoItem { get; set; }

        [NotMapped]
        public string TipoDocumentoItem { get; set; }

        [NotMapped]
        public string ContaProdesp { get; set; }

        [NotMapped]
        public string DataRealizacao { get; set; }

        [NotMapped]
        public string NomeReduzidoCredorDocto { get; set; }
    }
}
