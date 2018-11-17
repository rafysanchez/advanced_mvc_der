using Sids.Prodesp.Core.Services.PagamentoContaUnica;
using Sids.Prodesp.Core.Services.Seguranca;
using Sids.Prodesp.Core.Services.WebService;
using Sids.Prodesp.Core.Services.WebService.Empenho;
using Sids.Prodesp.Infrastructure.DataBase.PagamentoDer;
using Sids.Prodesp.Infrastructure.DataBase.Seguranca;
using Sids.Prodesp.Model.Entity.PagamentoContaUnica.FormaGerarNl;
using Sids.Prodesp.Model.Entity.PagamentoContaUnica.PagamentoDer;
using Sids.Prodesp.Model.Entity.PagamentoContaUnica.ReclassificacaoRetencao;
using Sids.Prodesp.Model.Entity.Seguranca;
using Sids.Prodesp.Model.Enum;
using Sids.Prodesp.Model.Exceptions;
using Sids.Prodesp.Model.Extension;
using Sids.Prodesp.Model.Interface.Log;
using Sids.Prodesp.Model.Interface.Reserva;
using Sids.Prodesp.Model.Interface.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sids.Prodesp.Core.Services.PagamentoDer
{
    public class ParametrizacaoFormaGerarNlService : Base.BaseService
    {
        public string uge = string.Empty;
        private readonly ConfirmacaoPagamentoItemDal _paraConfirmacaoPagamentoItemDal;
        private readonly NlParametrizacaoFormaGerarNlDal _paraNlParametrizacaoFormaGerarNlDal;
        private readonly NlParametrizacaoService _parametrizacaoService;
        private readonly ParaRestoAPagarService _paraRestoAPagarService;
        private readonly ReclassificacaoRetencaoService _reclassificacaoRetencaoService;
        private readonly ConfirmacaoPagamentoDal _paraConfirmacaoPagamentoIDal;
        private readonly CommonService _commomService;
        private readonly NlParametrizacaoEventoDal _nlParametrizacaoEventoDal;
        private readonly ReclassificacaoRetencaoNotaService _reclassificacaoRetencaoNotaService;
        private const int TAMANHOOBS = 228;
        internal readonly RegionalService _regional;

        public ParametrizacaoFormaGerarNlService(ILogError log, ConfirmacaoPagamentoItemDal paraConfirmacaoPagamentoItemDal, NlParametrizacaoFormaGerarNlDal paraNlParametrizacaoFormaGerarNlDal,
                NlParametrizacaoService parametrizacaoService, ParaRestoAPagarService paraRestoAPagarService, ReclassificacaoRetencaoService reclassificacaoRetencaoService, ConfirmacaoPagamentoDal paraConfirmacaoPagamentoIDal,
                 CommonService commomService, NlParametrizacaoEventoDal nlParametrizacaoEventoDal, ReclassificacaoRetencaoNotaService reclassificacaoRetencaoNotaService) : base(log)
        {
            _paraConfirmacaoPagamentoItemDal = paraConfirmacaoPagamentoItemDal;
            _paraNlParametrizacaoFormaGerarNlDal = paraNlParametrizacaoFormaGerarNlDal;
            _parametrizacaoService = parametrizacaoService;
            _paraRestoAPagarService = paraRestoAPagarService;
            _reclassificacaoRetencaoService = reclassificacaoRetencaoService;
            _paraConfirmacaoPagamentoIDal = paraConfirmacaoPagamentoIDal;
            _commomService = commomService;
            _nlParametrizacaoEventoDal = nlParametrizacaoEventoDal;
            _reclassificacaoRetencaoNotaService = reclassificacaoRetencaoNotaService;
            _regional = new RegionalService(log, new RegionalDal());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">IdConfirmacaoPagamento</param>
        /// <param name="confirmacaoPagamento"></param>
        /// <param name="usuario"></param>
        /// <param name="regionais"></param>
        /// <returns></returns>
        public AcaoEfetuada GerarNl(int id, ConfirmacaoPagamento confirmacaoPagamento, Usuario usuario, List<Regional> regionais, bool transmitirNls)
        {
            try
            {

                // Listar todos Itens Aprovados
                //Comentar quando POC estiver ok -- apenas para testar - após retirar item1
                var itens = _paraConfirmacaoPagamentoItemDal.Fetch(new ConfirmacaoPagamentoItem { IdConfirmacaoPagamento = id }).ToList();

                // int? idConformacao = id;

                // item1- descomentar quando terminar
                // var itens = _paraConfirmacaoPagamentoItemDal.Fetch(a).Where(x => x.StatusProdesp == "S").ToList();


                var itensAposValidacao = ValidacaoItens(itens);


                if (itensAposValidacao.Any())
                {
                    var itensPorFormasNl = ObterTipoDespesaPorFormaGerarNl(itensAposValidacao);
                    confirmacaoPagamento.Id = id;
                    confirmacaoPagamento.Items = itensPorFormasNl;
                    confirmacaoPagamento.CodigoAgrupamentoConfirmacaoPagamento = _paraConfirmacaoPagamentoIDal.Fetch(new ConfirmacaoPagamento { Id = id }).FirstOrDefault().CodigoAgrupamentoConfirmacaoPagamento;

                    List<NlParametrizacao> parametrizacoesCadastradas = _parametrizacaoService.ObterTodas(); 

                    var nlsGeradas = GerarNlPorForma(confirmacaoPagamento, parametrizacoesCadastradas, usuario, regionais);

                    if (transmitirNls)
                    {
                        foreach (var item in nlsGeradas)
                        {
                            var param = parametrizacoesCadastradas.FirstOrDefault(x => x.DescricaoTipoNL.Equals(item.dsTipoNL));
                            if (param != null && param.Transmitir)
                            {
                                _reclassificacaoRetencaoService.Transmitir(item.Id, usuario, 0);
                            }
                        }
                    } 
                }


                return AcaoEfetuada.Sucesso;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<ConfirmacaoPagamentoItem> ValidacaoItens(List<ConfirmacaoPagamentoItem> itens)
        {
            // Se houver itens com os dados nulos os mesmo são descartados

            var linhas = itens.Where(x => x.CodigoOrganizacaoCredor != 8 );

            var contazerada = itens.Where(x => x.NumeroContaFavorecido == "000000000" && x.nmReduzidoCredor != "BANCO BRASIL");

            linhas = linhas.Except(contazerada).ToList();

            var despesaExtraOrc = itens.Where(x => x.NumeroDocumentoGerador.Substring(0, 2).Equals("40") && x.IdTipoDespesa != 78);

            return linhas.Except(despesaExtraOrc).ToList();

        }

        private List<ConfirmacaoPagamentoItem> ObterTipoDespesaPorFormaGerarNl(List<ConfirmacaoPagamentoItem> itens)
        {
            try
            {
                List<int?> listIdTipoDespesa = new List<int?>();
                //Todos os Tipos de Despesa
                foreach (var item in itens)
                    listIdTipoDespesa.Add(item.IdTipoDespesa);

                List<FormaGerarNl> formasGerarNlPorTipoDespesa = new List<FormaGerarNl>();

                var filtro = new FormaGerarNl();

                //Cada tipo de Despesa possui uma forma de Gerar NL
                foreach (var tipoDespesa in listIdTipoDespesa.Distinct())
                {
                    filtro.IdDespesaTipo = tipoDespesa ?? 0;
                    formasGerarNlPorTipoDespesa.Add(GetFormaGerarNlPorTipoDespesa(filtro));
                }


                //Setando cada item da forma que ira gerar a NL (OP, Credor/Empenho ou Empenho)
                foreach (var item in itens)
                {
                    var result = formasGerarNlPorTipoDespesa.Where(x => x.IdDespesaTipo == item.IdTipoDespesa).FirstOrDefault();
                    item.FormaGerar = result.FormaGerar;
                    item.CodigoTipoDespesa = result.CodigoTipoDespesa;
                    item.IdNlParametrizacao = result.IdNlParametrizacao;
                    item.numeroTipoNl = result.numeroTipoNl;

                }


                return itens;
            }
            catch (Exception ex)
            {
                throw new SidsException("Dados do parâmetro não encontrados para a despesa.");
               
            }

            

        }


        private FormaGerarNl GetFormaGerarNlPorTipoDespesa(FormaGerarNl entity)
        {
            return _paraNlParametrizacaoFormaGerarNlDal.GetFormaGerarNlPorTipoDespesa(entity);
        }



        private IEnumerable<IGrouping<dynamic, ConfirmacaoPagamentoItem>> ObterItensPorCredorEmpenhoDespesa(List<ConfirmacaoPagamentoItem> entidades)
        {

            var agrupadosPorCredorEmpenho = entidades.GroupBy(x => new { x.NumeroBancoFavorecido, x.NumeroAgenciaFavorecido, x.NumeroContaFavorecido, x.NumeroEmpenhoSiafem, x.IdTipoDespesa });

            return agrupadosPorCredorEmpenho;
        }

        private IEnumerable<IGrouping<dynamic, ConfirmacaoPagamentoItem>> ObterItensPorEmpenhoDespesa(List<ConfirmacaoPagamentoItem> entidades)
        {

            var agrupadosPorEmpenhoDespesa = entidades.GroupBy(x => new { x.NumeroEmpenhoSiafem, x.IdTipoDespesa });

            return agrupadosPorEmpenhoDespesa;
        }

        private IEnumerable<IGrouping<dynamic, ConfirmacaoPagamentoItem>> ObterItensPorOpDespesa(List<ConfirmacaoPagamentoItem> entidades)
        {

            var agrupadosPorOpDespesa = entidades.GroupBy(x => new { x.NumeroOp, x.IdTipoDespesa });

            return agrupadosPorOpDespesa;
        }

        private List<ReclassificacaoRetencao> GerarNlPorForma(ConfirmacaoPagamento confirmacao, List<NlParametrizacao> parametrizacoesCadastradas, Usuario usuario, List<Regional> regionais, int recursoId = 0)
        {

            try
            {
                this.uge = _regional.Buscar(new Regional { Id = (int)usuario.RegionalId }).First().Uge;
                List<ReclassificacaoRetencao> nlsGeradas = new List<ReclassificacaoRetencao>();

                List<ParaRestoAPagar> tiposRap = _paraRestoAPagarService.Listar(new ParaRestoAPagar()).ToList();

                // inicio da area de Nl de repasse
                if (confirmacao.DespesaTipo!=78)
                {
                    var nlRepasse = FormacaoRepasse(confirmacao, tiposRap, parametrizacoesCadastradas, usuario, regionais);
                    var idRepasse = _reclassificacaoRetencaoService.SalvarOuAlterar(nlRepasse, 25, (short)EnumAcao.Inserir);
                    ReclassificacaoRetencaoNota notaRep = new ReclassificacaoRetencaoNota();
                    notaRep.IdReclassificacaoRetencao = idRepasse;
                    notaRep.Ordem = 1;
                    notaRep.CodigoNotaFiscal = "NADA CONSTA";
                    _reclassificacaoRetencaoNotaService.SalvarOuAlterar(notaRep, 25, (short)EnumAcao.Inserir);
                }
                

                // fim do repasse

                if (confirmacao.Items.Any(x => x.FormaGerar == EnumFormaGerarNl.CredorEmpenho))
                {
                    var nlsPorCredorEmpenhoDespesa = ObterItensPorCredorEmpenhoDespesa(confirmacao.Items.Where(x => x.FormaGerar == EnumFormaGerarNl.CredorEmpenho).ToList());

                    foreach (var nlPorCredorEmpenhoDespesa in nlsPorCredorEmpenhoDespesa)
                    {
                        var baseNl = FormacaoBaseNl(confirmacao, tiposRap, nlPorCredorEmpenhoDespesa, parametrizacoesCadastradas, usuario, regionais, uge);

                        nlsGeradas.Add(NlPorTipoDespesa(nlPorCredorEmpenhoDespesa.ToList(), baseNl));
                    }
                }

                if (confirmacao.Items.Any(x => x.FormaGerar == EnumFormaGerarNl.Empenho))
                {
                    var nlsPorEmpenhoDespesa = ObterItensPorEmpenhoDespesa(confirmacao.Items.Where(x => x.FormaGerar == EnumFormaGerarNl.Empenho).ToList());
                    foreach (var nlPorEmpenhoDespesa in nlsPorEmpenhoDespesa)
                    {
                        var baseNl = FormacaoBaseNl(confirmacao, tiposRap, nlPorEmpenhoDespesa, parametrizacoesCadastradas, usuario, regionais, uge);

                        nlsGeradas.Add(NlPorTipoDespesa(nlPorEmpenhoDespesa.ToList(), baseNl));
                    }
                }

                if (confirmacao.Items.Any(x => x.FormaGerar == EnumFormaGerarNl.Op))
                {
                    var nlsPorOpDespesa = ObterItensPorOpDespesa(confirmacao.Items.Where(x => x.FormaGerar == EnumFormaGerarNl.Op).ToList());
                    foreach (var nlPorOpDespesa in nlsPorOpDespesa)
                    {
                        var baseNl = FormacaoBaseNl(confirmacao, tiposRap, nlPorOpDespesa, parametrizacoesCadastradas, usuario, regionais, uge);

                        nlsGeradas.Add(NlPorTipoDespesa(nlPorOpDespesa.ToList(), baseNl));
                    }
                }



                // com os dados organizados  - processa para amrmazenar - rafael magna 2018
                foreach (var nlGerada in nlsGeradas)
                {



                    // 25 - Reclassificação Retenção

                    var idReclassificacaoRentencao = _reclassificacaoRetencaoService.SalvarOuAlterar(nlGerada, 25, (short)EnumAcao.Inserir);

                    // ADICIONA OS DADOS nl-> NOTA
                    ReclassificacaoRetencaoNota nota = new ReclassificacaoRetencaoNota();
                    nota.IdReclassificacaoRetencao = idReclassificacaoRentencao;
                    nota.Ordem = 1;
                    nota.CodigoNotaFiscal = "NADA CONSTA";
                    _reclassificacaoRetencaoNotaService.SalvarOuAlterar(nota, recursoId, (short)EnumAcao.Inserir);
                    // FIM DE ADICAO NOTA

                    foreach (var item in nlGerada.itensPertenceNl)
                    {
                        _paraConfirmacaoPagamentoItemDal.EditReclassificacaoRetencao(new ConfirmacaoPagamentoItem { Id = item.Id, IdReclassificacaoRetencao = idReclassificacaoRentencao });
                    }

                }
                ConfirmacaoPagamentoDal dadosTotais = new ConfirmacaoPagamentoDal();
                var linhasDadosTotais = dadosTotais.Listar(confirmacao.Id).ToList();
                // gerar os totais IR

                foreach (var item in linhasDadosTotais)
                {
                    if (item.VrTotalConfirmarIR > 0)
                    {
                        var nlIR = FormacaoNLIR(confirmacao, tiposRap, parametrizacoesCadastradas, usuario, regionais, item);
                        var idIR = _reclassificacaoRetencaoService.SalvarOuAlterar(nlIR, 25, (short)EnumAcao.Inserir);
                        ReclassificacaoRetencaoNota notaIR = new ReclassificacaoRetencaoNota();
                        notaIR.IdReclassificacaoRetencao = idIR;
                        notaIR.Ordem = 1;
                        notaIR.CodigoNotaFiscal = "NADA CONSTA";
                        _reclassificacaoRetencaoNotaService.SalvarOuAlterar(notaIR, 25, (short)EnumAcao.Inserir);
                    }


                }

                // fim totais IR

                // gerar os totais ISSQN
                foreach (var item in linhasDadosTotais)
                {
                    if (item.VrTotalConfirmarISSQN > 0)
                    {
                        var nlISSQN = FormacaoNLISSQN(confirmacao, tiposRap, parametrizacoesCadastradas, usuario, regionais, item, uge);
                        var idISSQN = _reclassificacaoRetencaoService.SalvarOuAlterar(nlISSQN, 25, (short)EnumAcao.Inserir);
                        ReclassificacaoRetencaoNota notaISSQN = new ReclassificacaoRetencaoNota();
                        notaISSQN.IdReclassificacaoRetencao = idISSQN;
                        notaISSQN.Ordem = 1;
                        notaISSQN.CodigoNotaFiscal = "NADA CONSTA";
                        _reclassificacaoRetencaoNotaService.SalvarOuAlterar(notaISSQN, 25, (short)EnumAcao.Inserir);
                    }
                }
                // fim totais ISSQN


                return nlsGeradas;
            }

            catch (Exception ex)
            {

                throw ex;
            }

        }



        private ReclassificacaoRetencao NlPorTipoDespesa(List<ConfirmacaoPagamentoItem> grupoItensPorTipoDespesa, ReclassificacaoRetencao nlParametrizada)
        {

            //Gravar as Nls por TipoDespesa
            switch ((EnumTipoNl)grupoItensPorTipoDespesa.FirstOrDefault().numeroTipoNl)
            {
                case EnumTipoNl.RepasseFinanceiro:
                    return NlPorRepasseFinanceiro(grupoItensPorTipoDespesa, nlParametrizada);
                    break;

            }

            return nlParametrizada;
        }

        /// <summary>
        /// vai ao banco e traz o idnlparametrizacao da tabela tb_nl_parametrizacao
        /// </summary>
        /// <param name="termo"></param>
        /// <returns></returns>
        public int retornaId(string termo)
        {
            return _paraConfirmacaoPagamentoIDal.retornaIDNLparametro(termo);
        }

        //FormacaoNLISSQN
        /// <summary>
        /// ajusta os dados da NL de ISSQN
        /// </summary>
        /// <param name="confirmacaoPagamento"></param>
        /// <param name="tiposRap"></param>
        /// <param name="parametrizacoesCadastradas"></param>
        /// <param name="usuario"></param>
        /// <param name="regionais"></param>
        /// <param name="dado"></param>
        /// <returns></returns>
        private ReclassificacaoRetencao FormacaoNLISSQN(ConfirmacaoPagamento confirmacaoPagamento, List<ParaRestoAPagar> tiposRap, List<NlParametrizacao> parametrizacoesCadastradas, Usuario usuario, List<Regional> regionais, ConfirmacaoPagamentoTotais dado, string uge)
        {
            var IDISSQN = retornaId("Baixa de ISSQN"); // 66 por enquanto
            //Os itens ja se encontram separados por Tipo de Despesa
            var infoBaseItem = confirmacaoPagamento.Items.FirstOrDefault();

            //Informações do item na parametrização
            var infoBaseParametrizacao = parametrizacoesCadastradas.Where(x => x.Id == IDISSQN).FirstOrDefault();

            infoBaseParametrizacao.Eventos = _nlParametrizacaoEventoDal.Fetch(new NlParametrizacaoEvento { IdNlParametrizacao = infoBaseParametrizacao.Id });

            ReclassificacaoRetencao entidadeNl = new ReclassificacaoRetencao();

            if (regionais.Any(x => x.Descricao.Contains(infoBaseItem.IdRegional.ToString())))
                entidadeNl.RegionalId = regionais.Where(x => x.Descricao.Contains(infoBaseItem.IdRegional.ToString())).FirstOrDefault().Id;

            //Atributos em comum
            entidadeNl.dsTipoNL = infoBaseParametrizacao.DescricaoTipoNL;
            entidadeNl.Id = 0; // geral
            entidadeNl.DataEmissao = entidadeNl.DataCadastro = DateTime.Now; // geral
            entidadeNl.CodigoUnidadeGestora = infoBaseParametrizacao.UnidadeGestora.HasValue ? infoBaseParametrizacao.UnidadeGestora.Value.ToString() : string.Empty;
            entidadeNl.NumeroProcesso = infoBaseItem.NumeroProcesso;
            entidadeNl.NumeroDocumento = infoBaseItem.NrDocumento;
            entidadeNl.DataCadastro = DateTime.Now.Date;

            entidadeNl.CodigoAplicacaoObra = infoBaseItem.CodigoObra;
            entidadeNl.NumeroCNPJCPFFornecedorId = infoBaseItem.NumeroCnpjCpfUgCredor;
            entidadeNl.NormalEstorno = "1";
            entidadeNl.NumeroCNPJCPFCredor = infoBaseParametrizacao.FavorecidaCnpjCpfUg;

            entidadeNl.CodigoGestaoCredor = infoBaseParametrizacao.FavorecidaNumeroGestao.ToString().PadLeft(5, '0');
            entidadeNl.CodigoGestao = "16055";
            entidadeNl.Origem = OrigemReclassificacaoRetencao.ConfirmacaoDePagamento;
            entidadeNl.DescricaoObservacao1 = SepararObservacao(infoBaseParametrizacao.Observacao, TAMANHOOBS, 1);
            entidadeNl.DescricaoObservacao2 = SepararObservacao(infoBaseParametrizacao.Observacao, TAMANHOOBS, 2);
            entidadeNl.DescricaoObservacao3 = SepararObservacao(infoBaseParametrizacao.Observacao, TAMANHOOBS, 3);
            // entidadeNl.Valor = confirmacaoPagamento.Items.Sum(x => x.ValorDocumento * 100) ?? 0;

            entidadeNl.AgrupamentoConfirmacao = confirmacaoPagamento.CodigoAgrupamentoConfirmacaoPagamento;
            entidadeNl.id_confirmacao_pagamento = confirmacaoPagamento.Id;
            AtribuiValorCampoAnoExercicio(infoBaseItem, entidadeNl);

            // var NLPGOBRAS = AtribuiValoresQuandoNLPGOBRAS(usuario, infoBaseItem, infoBaseParametrizacao, entidadeNl);
            bool NLPGOBRAS = false;
            entidadeNl.ReclassificacaoRetencaoTipoId = EnumTipoReclassificacaoRetencao.NotaLancamento.GetHashCode();

            List<ReclassificacaoRetencaoEvento> eventos = new List<ReclassificacaoRetencaoEvento>();

            var parametrosGeracaoNl = new ParametrosGeracaoNl(confirmacaoPagamento, infoBaseItem, null, tiposRap);
            var tipoDocumento = infoBaseItem.NumeroDocumentoGerador.Substring(0, 2);
            parametrosGeracaoNl.IsNlPgObras = NLPGOBRAS;
            parametrosGeracaoNl.IsSubempenho = tipoDocumento.Equals(EnumTipoDocumento.Subempenho.GetHashCode().ToString("d2"));
            parametrosGeracaoNl.IsRap = tipoDocumento.Equals(EnumTipoDocumento.Rap.GetHashCode().ToString("d2"));
            parametrosGeracaoNl.valorAcumuladoNl = dado.VrTotalConfirmarISSQN * 100;
            entidadeNl.NumeroOriginalSiafemSiafisico = infoBaseItem.NumeroEmpenhoSiafem;
            entidadeNl.CodigoCredorOrganizacaoId = infoBaseItem.CodigoOrganizacaoCredor ?? 0;

            //***********novos campos - rafael magna see ************
            entidadeNl.DocumentoTipoId = infoBaseItem.IdTipoDocumento;
            //********** fim ****************************************

            GerarEventosSaidaISSQN(parametrosGeracaoNl, eventos, infoBaseParametrizacao, dado);
            entidadeNl.Valor = eventos.Sum(x => x.ValorUnitario);

            GerarEventoEntradaISSQN(parametrosGeracaoNl, entidadeNl, eventos, infoBaseParametrizacao, dado, uge);
            entidadeNl.NormalEstorno = ((int)EnumNormalEstorno.Normal).ToString();


            entidadeNl.itensPertenceNl = confirmacaoPagamento.Items.ToList();

            return entidadeNl;

        }


        /// <summary>
        /// ajustas os dadso da NL de IR
        /// </summary>
        /// <param name="confirmacaoPagamento"></param>
        /// <param name="tiposRap"></param>
        /// <param name="parametrizacoesCadastradas"></param>
        /// <param name="usuario"></param>
        /// <param name="regionais"></param>
        /// <param name="dado"></param>
        /// <returns></returns>
        private ReclassificacaoRetencao FormacaoNLIR(ConfirmacaoPagamento confirmacaoPagamento, List<ParaRestoAPagar> tiposRap, List<NlParametrizacao> parametrizacoesCadastradas, Usuario usuario, List<Regional> regionais, ConfirmacaoPagamentoTotais dado)
        {
            var idIR = retornaId("Repasse de IR"); // 66 por enquanto
            //Os itens ja se encontram separados por Tipo de Despesa
            var infoBaseItem = confirmacaoPagamento.Items.FirstOrDefault();

            //Informações do item na parametrização
            var infoBaseParametrizacao = parametrizacoesCadastradas.Where(x => x.Id == idIR).FirstOrDefault();

            infoBaseParametrizacao.Eventos = _nlParametrizacaoEventoDal.Fetch(new NlParametrizacaoEvento { IdNlParametrizacao = infoBaseParametrizacao.Id });

            ReclassificacaoRetencao entidadeNl = new ReclassificacaoRetencao();

            if (regionais.Any(x => x.Descricao.Contains(infoBaseItem.IdRegional.ToString())))
                entidadeNl.RegionalId = regionais.Where(x => x.Descricao.Contains(infoBaseItem.IdRegional.ToString())).FirstOrDefault().Id;

            //Atributos em comum
            entidadeNl.dsTipoNL = infoBaseParametrizacao.DescricaoTipoNL;
            entidadeNl.Id = 0; // geral
            entidadeNl.DataEmissao = entidadeNl.DataCadastro = DateTime.Now; // geral
            entidadeNl.CodigoUnidadeGestora = infoBaseParametrizacao.UnidadeGestora.HasValue ? infoBaseParametrizacao.UnidadeGestora.Value.ToString() : string.Empty;
            entidadeNl.NumeroProcesso = infoBaseItem.NumeroProcesso;
            entidadeNl.NumeroDocumento = infoBaseItem.NrDocumento;
            entidadeNl.DataCadastro = DateTime.Now.Date;

            entidadeNl.CodigoAplicacaoObra = infoBaseItem.CodigoObra;
            entidadeNl.NumeroCNPJCPFFornecedorId = infoBaseItem.NumeroCnpjCpfUgCredor;
            entidadeNl.NormalEstorno = "1";
            entidadeNl.NotaLancamenoMedicao = infoBaseItem.NumeroNlDocumento;
            entidadeNl.NumeroContrato = infoBaseItem.NumeroContrato;
            entidadeNl.CadastroCompleto = true;
            entidadeNl.Origem = OrigemReclassificacaoRetencao.ConfirmacaoDePagamento;
            entidadeNl.NumeroCNPJCPFCredor = infoBaseParametrizacao.FavorecidaCnpjCpfUg;
            // entidadeNl.NumeroCNPJCPFCredor = infoBaseItem.NumeroCnpjCpfUgCredor;
            entidadeNl.CodigoGestaoCredor = infoBaseParametrizacao.FavorecidaNumeroGestao.ToString().PadLeft(5, '0');
            entidadeNl.CodigoGestao = "16055";



            entidadeNl.DescricaoObservacao1 = SepararObservacao(infoBaseParametrizacao.Observacao, TAMANHOOBS, 1);
            entidadeNl.DescricaoObservacao2 = SepararObservacao(infoBaseParametrizacao.Observacao, TAMANHOOBS, 2);
            entidadeNl.DescricaoObservacao3 = SepararObservacao(infoBaseParametrizacao.Observacao, TAMANHOOBS, 3);
            entidadeNl.Valor = confirmacaoPagamento.Items.Sum(x => x.ValorDocumento * 100) ?? 0;
            entidadeNl.AgrupamentoConfirmacao = confirmacaoPagamento.CodigoAgrupamentoConfirmacaoPagamento;
            entidadeNl.id_confirmacao_pagamento = confirmacaoPagamento.Id;
            AtribuiValorCampoAnoExercicio(infoBaseItem, entidadeNl);

            // var NLPGOBRAS = AtribuiValoresQuandoNLPGOBRAS(usuario, infoBaseItem, infoBaseParametrizacao, entidadeNl);
            bool NLPGOBRAS = false;
            entidadeNl.ReclassificacaoRetencaoTipoId = EnumTipoReclassificacaoRetencao.NotaLancamento.GetHashCode();
            List<ReclassificacaoRetencaoEvento> eventos = new List<ReclassificacaoRetencaoEvento>();

            var parametrosGeracaoNl = new ParametrosGeracaoNl(confirmacaoPagamento, infoBaseItem, null, tiposRap);
            var tipoDocumento = infoBaseItem.NumeroDocumentoGerador.Substring(0, 2);
            parametrosGeracaoNl.IsNlPgObras = NLPGOBRAS;
            parametrosGeracaoNl.IsSubempenho = tipoDocumento.Equals(EnumTipoDocumento.Subempenho.GetHashCode().ToString("d2"));
            parametrosGeracaoNl.IsRap = tipoDocumento.Equals(EnumTipoDocumento.Rap.GetHashCode().ToString("d2"));
            parametrosGeracaoNl.valorAcumuladoNl = dado.VrTotalConfirmarIR * 100;
            entidadeNl.NumeroOriginalSiafemSiafisico = infoBaseItem.NumeroEmpenhoSiafem;
            entidadeNl.CodigoCredorOrganizacaoId = infoBaseItem.CodigoOrganizacaoCredor ?? 0;

            //***********novos campos - rafael magna see ************
            entidadeNl.DocumentoTipoId = infoBaseItem.IdTipoDocumento;
            //********** fim ****************************************

            GerarEventosSaidaIR(parametrosGeracaoNl, eventos, infoBaseParametrizacao, dado);
            entidadeNl.Valor = eventos.Sum(x => x.ValorUnitario);

            GerarEventoEntradaIR(parametrosGeracaoNl, entidadeNl, eventos, infoBaseParametrizacao, dado);
            entidadeNl.NormalEstorno = ((int)EnumNormalEstorno.Normal).ToString();


            entidadeNl.itensPertenceNl = confirmacaoPagamento.Items.ToList();

            return entidadeNl;


        }

        /// <summary>
        /// ajusta os dados da NL de repasse financeiro
        /// </summary>
        /// <param name="confirmacaoPagamento"></param>
        /// <param name="tiposRap"></param>
        /// <param name="parametrizacoesCadastradas"></param>
        /// <param name="usuario"></param>
        /// <param name="regionais"></param>
        /// <returns></returns>
        private ReclassificacaoRetencao FormacaoRepasse(ConfirmacaoPagamento confirmacaoPagamento, List<ParaRestoAPagar> tiposRap, List<NlParametrizacao> parametrizacoesCadastradas, Usuario usuario, List<Regional> regionais)
        {
            var idRepasse = retornaId("Repasse Financeiro");

            ConfirmacaoPagamentoDal dadosTotais = new ConfirmacaoPagamentoDal();

            var linhasDadosTotais = dadosTotais.Listar(confirmacaoPagamento.Id).ToList();

            //Os itens ja se encontram separados por Tipo de Despesa
            var infoBaseItem = confirmacaoPagamento.Items.FirstOrDefault();

            //Informações do item na parametrização
            var infoBaseParametrizacao = parametrizacoesCadastradas.Where(x => x.Id == idRepasse).FirstOrDefault();

            infoBaseParametrizacao.Eventos = _nlParametrizacaoEventoDal.Fetch(new NlParametrizacaoEvento { IdNlParametrizacao = infoBaseParametrizacao.Id });

            ReclassificacaoRetencao entidadeNl = new ReclassificacaoRetencao();

            if (regionais.Any(x => x.Descricao.Contains(infoBaseItem.IdRegional.ToString())))
                entidadeNl.RegionalId = regionais.Where(x => x.Descricao.Contains(infoBaseItem.IdRegional.ToString())).FirstOrDefault().Id;

            //Atributos em comum
            entidadeNl.dsTipoNL = infoBaseParametrizacao.DescricaoTipoNL;
            entidadeNl.Id = 0; // geral
            entidadeNl.DataEmissao = entidadeNl.DataCadastro = DateTime.Now; // geral
            entidadeNl.CodigoUnidadeGestora = infoBaseParametrizacao.UnidadeGestora.HasValue ? infoBaseParametrizacao.UnidadeGestora.Value.ToString() : string.Empty;
            entidadeNl.NumeroProcesso = infoBaseItem.NumeroProcesso;
            entidadeNl.NumeroDocumento = infoBaseItem.NrDocumento;
            entidadeNl.DataCadastro = DateTime.Now.Date;

            entidadeNl.CodigoAplicacaoObra = infoBaseItem.CodigoObra;
            entidadeNl.NumeroCNPJCPFFornecedorId = infoBaseItem.NumeroCnpjCpfUgCredor;
            entidadeNl.NormalEstorno = "1";
            entidadeNl.NotaLancamenoMedicao = infoBaseItem.NumeroNlDocumento;
            entidadeNl.NumeroContrato = infoBaseItem.NumeroContrato;
            entidadeNl.CadastroCompleto = true;
            entidadeNl.Origem = OrigemReclassificacaoRetencao.ConfirmacaoDePagamento;
            entidadeNl.NumeroCNPJCPFCredor = infoBaseParametrizacao.FavorecidaCnpjCpfUg; 
           // entidadeNl.NumeroCNPJCPFCredor = infoBaseParametrizacao.UnidadeGestora.HasValue ? infoBaseParametrizacao.UnidadeGestora.Value.ToString() : string.Empty;
            entidadeNl.CodigoGestaoCredor = infoBaseParametrizacao.FavorecidaNumeroGestao.ToString().PadLeft(5,'0');
            entidadeNl.CodigoGestao = "16055";

            var descricaoDespesa = _parametrizacaoService.ret_DespesaObservacao(infoBaseItem.CodigoTipoDespesa);
            string observacao = infoBaseParametrizacao.Observacao + " " + descricaoDespesa;

            entidadeNl.DescricaoObservacao1 = SepararObservacao(observacao, TAMANHOOBS, 1);
            entidadeNl.DescricaoObservacao2 = SepararObservacao(observacao, TAMANHOOBS, 2);
            entidadeNl.DescricaoObservacao3 = SepararObservacao(observacao, TAMANHOOBS, 3);

            entidadeNl.Valor = confirmacaoPagamento.Items.Sum(x => x.ValorDocumento * 100) ?? 0;
            entidadeNl.AgrupamentoConfirmacao = confirmacaoPagamento.CodigoAgrupamentoConfirmacaoPagamento;
            entidadeNl.id_confirmacao_pagamento = confirmacaoPagamento.Id;
            AtribuiValorCampoAnoExercicio(infoBaseItem, entidadeNl);

            // var NLPGOBRAS = AtribuiValoresQuandoNLPGOBRAS(usuario, infoBaseItem, infoBaseParametrizacao, entidadeNl);
            bool NLPGOBRAS = false;
            entidadeNl.ReclassificacaoRetencaoTipoId = EnumTipoReclassificacaoRetencao.NotaLancamento.GetHashCode();


            List<ReclassificacaoRetencaoEvento> eventos = new List<ReclassificacaoRetencaoEvento>();

            var parametrosGeracaoNl = new ParametrosGeracaoNl(confirmacaoPagamento, infoBaseItem, null, tiposRap);
            var tipoDocumento = infoBaseItem.NumeroDocumentoGerador.Substring(0, 2);
            parametrosGeracaoNl.IsNlPgObras = NLPGOBRAS;
            parametrosGeracaoNl.IsSubempenho = tipoDocumento.Equals(EnumTipoDocumento.Subempenho.GetHashCode().ToString("d2"));
            parametrosGeracaoNl.IsRap = tipoDocumento.Equals(EnumTipoDocumento.Rap.GetHashCode().ToString("d2"));
            parametrosGeracaoNl.valorAcumuladoNl = linhasDadosTotais.Select(X => X.VrTotalConfirmar * 100).FirstOrDefault();
            entidadeNl.NumeroOriginalSiafemSiafisico = infoBaseItem.NumeroEmpenhoSiafem;
            entidadeNl.CodigoCredorOrganizacaoId = infoBaseItem.CodigoOrganizacaoCredor ?? 0;

            //***********novos campos - rafael magna see ************
            entidadeNl.DocumentoTipoId = infoBaseItem.IdTipoDocumento;
            //********** fim ****************************************

            GerarEventosSaidaTotal(parametrosGeracaoNl, eventos, infoBaseParametrizacao, linhasDadosTotais);
            entidadeNl.Valor = eventos.Sum(x => x.ValorUnitario);

            GerarEventoEntradaTotal(parametrosGeracaoNl, entidadeNl, eventos, infoBaseParametrizacao, linhasDadosTotais);
            entidadeNl.NormalEstorno = ((int)EnumNormalEstorno.Normal).ToString();


            entidadeNl.itensPertenceNl = confirmacaoPagamento.Items.ToList();

            return entidadeNl;


        }

        /// <summary>
        /// ajusta os daso basicos dos itens de agendamentos confirmados
        /// </summary>
        /// <param name="confirmacaoPagamento"></param>
        /// <param name="tiposRap"></param>
        /// <param name="itensNl"></param>
        /// <param name="parametrizacoesCadastradas"></param>
        /// <param name="usuario"></param>
        /// <param name="regionais"></param>
        /// <returns></returns>
        private ReclassificacaoRetencao FormacaoBaseNl(ConfirmacaoPagamento confirmacaoPagamento, List<ParaRestoAPagar> tiposRap, IGrouping<dynamic, ConfirmacaoPagamentoItem> itensNl, List<NlParametrizacao> parametrizacoesCadastradas, Usuario usuario, List<Regional> regionais, string UGE)
        {

            //Os itens ja se encontram separados por Tipo de Despesa
            var infoBaseItem = itensNl.FirstOrDefault();

            //Informações do item na parametrização
            var infoBaseParametrizacao = parametrizacoesCadastradas.Where(x => x.Id == infoBaseItem.IdNlParametrizacao).FirstOrDefault();

            infoBaseParametrizacao.Eventos = _nlParametrizacaoEventoDal.Fetch(new NlParametrizacaoEvento { IdNlParametrizacao = infoBaseParametrizacao.Id });

            ReclassificacaoRetencao entidadeNl = new ReclassificacaoRetencao();

            if (regionais.Any(x => x.Descricao.Contains(infoBaseItem.IdRegional.ToString())))
                entidadeNl.RegionalId = regionais.Where(x => x.Descricao.Contains(infoBaseItem.IdRegional.ToString())).FirstOrDefault().Id;

            var ugRegional = new Regional();
            if (infoBaseParametrizacao.DescricaoTipoNL == "Repasse para Regionais")
            {
                ugRegional = UGregional(infoBaseItem.nmReduzidoCredor.Substring(3, 2));
            }           

            //Atributos em comum
            entidadeNl.dsTipoNL = infoBaseParametrizacao.DescricaoTipoNL;
            entidadeNl.Id = 0; // geral
            entidadeNl.DataEmissao = entidadeNl.DataCadastro = DateTime.Now; // geral

            entidadeNl.NumeroProcesso = infoBaseItem.NumeroProcesso;
            entidadeNl.NumeroDocumento = infoBaseItem.NrDocumento;
            entidadeNl.DataCadastro = DateTime.Now.Date;

            if (infoBaseParametrizacao.DescricaoTipoNL == "Repasse para Regionais")
            {
                entidadeNl.CodigoUnidadeGestora = "182184";
                entidadeNl.NumeroCNPJCPFCredor = ugRegional.Uge;
            }
            else if (infoBaseParametrizacao.DescricaoTipoNL == "Baixa de Jeton-Jari")
            {
                entidadeNl.CodigoUnidadeGestora = infoBaseParametrizacao.UnidadeGestora.HasValue ? infoBaseParametrizacao.UnidadeGestora.Value.ToString() : string.Empty;
                entidadeNl.NumeroCNPJCPFCredor = infoBaseParametrizacao.FavorecidaCnpjCpfUg;
            }
            else if (infoBaseParametrizacao.DescricaoTipoNL == "Baixa de Pagamento de Licença-Prêmio")
            {
                entidadeNl.CodigoUnidadeGestora = infoBaseParametrizacao.UnidadeGestora.HasValue ? infoBaseParametrizacao.UnidadeGestora.Value.ToString() : string.Empty;
                entidadeNl.NumeroCNPJCPFCredor = infoBaseItem.NumeroCnpjCpfUgCredor;
            }
            else if (infoBaseParametrizacao.DescricaoTipoNL == "Baixa de Devolução de Caução")
            {
                entidadeNl.CodigoUnidadeGestora = infoBaseParametrizacao.UnidadeGestora.HasValue ? infoBaseParametrizacao.UnidadeGestora.Value.ToString() : string.Empty;
                entidadeNl.NumeroCNPJCPFCredor = infoBaseItem.NumeroCnpjCpfUgCredor + "";
            }
            else if (infoBaseParametrizacao.DescricaoTipoNL == "Baixa de Pagamento de Adiantamento")
            {
                entidadeNl.CodigoUnidadeGestora = infoBaseParametrizacao.UnidadeGestora.HasValue ? infoBaseParametrizacao.UnidadeGestora.Value.ToString() : string.Empty;
                entidadeNl.NumeroCNPJCPFCredor = infoBaseItem.NumeroCnpjCpfUgCredor + " ";
            }
            else if (infoBaseParametrizacao.DescricaoTipoNL == "Baixa de Pagamento de Restituição de Multa")
            {
                entidadeNl.CodigoUnidadeGestora = infoBaseParametrizacao.UnidadeGestora.HasValue ? infoBaseParametrizacao.UnidadeGestora.Value.ToString() : string.Empty;
                entidadeNl.NumeroCNPJCPFCredor = infoBaseParametrizacao.FavorecidaCnpjCpfUg + "";
            }
            else if (infoBaseParametrizacao.DescricaoTipoNL == "Baixa de Pagamento de Perito do Quadro/ Judicial")
            {
                entidadeNl.CodigoUnidadeGestora = infoBaseParametrizacao.UnidadeGestora.HasValue ? infoBaseParametrizacao.UnidadeGestora.Value.ToString()+"" : string.Empty;
                entidadeNl.NumeroCNPJCPFCredor = infoBaseItem.NumeroCnpjCpfUgCredor ;
            }
            else if (infoBaseParametrizacao.DescricaoTipoNL == "Baixa de Pagamento de Bonificação por Resultados")
            {
                entidadeNl.CodigoUnidadeGestora =  infoBaseParametrizacao.UnidadeGestora.HasValue ? ""+ infoBaseParametrizacao.UnidadeGestora.Value.ToString() : string.Empty;
                entidadeNl.NumeroCNPJCPFCredor = infoBaseItem.NumeroCnpjCpfUgCredor + "";
            }

            else
            {
                entidadeNl.CodigoUnidadeGestora = infoBaseParametrizacao.UnidadeGestora.HasValue ? infoBaseParametrizacao.UnidadeGestora.Value.ToString() : string.Empty;
                entidadeNl.NumeroCNPJCPFCredor = infoBaseItem.NumeroCnpjCpfUgCredor;
            }



            entidadeNl.CodigoAplicacaoObra = infoBaseItem.CodigoObra;
            entidadeNl.NumeroCNPJCPFFornecedorId = infoBaseItem.NumeroCnpjCpfUgCredor;
            entidadeNl.NormalEstorno = "1";
            entidadeNl.NotaLancamenoMedicao = infoBaseItem.NumeroNlDocumento;
            entidadeNl.NumeroContrato = infoBaseItem.NumeroContrato;
            entidadeNl.CadastroCompleto = true;
            entidadeNl.Origem = OrigemReclassificacaoRetencao.ConfirmacaoDePagamento;
            // entidadeNl.NumeroCNPJCPFCredor = infoBaseParametrizacao.FavorecidaCnpjCpfUg; comentado  - verificar

            entidadeNl.CodigoGestaoCredor = infoBaseParametrizacao.FavorecidaNumeroGestao.ToString().PadLeft(5, '0'); 
            entidadeNl.CodigoGestao = "16055";

            string texto = infoBaseParametrizacao.Observacao;

            if (infoBaseParametrizacao.DescricaoTipoNL == "Baixa de Pagamento de Fornecedor")
            {
                texto = infoBaseParametrizacao.Observacao.Replace("YYYY", infoBaseItem.NumeroContrato);
                StringBuilder NotasFiscais = new StringBuilder();
                foreach (var item in itensNl)
                {
                    if (item.NumeroNotaFiscal > 0 && item.NumeroNotaFiscal != null)
                    {
                        NotasFiscais.Append(item.NumeroNotaFiscal + " / ");
                    }
                }

                texto = texto.Replace("XXXX", NotasFiscais.ToString());
                if (infoBaseItem.NumeroContrato == "" && NotasFiscais.ToString() == "")
                {
                    texto = "Baixa de Pagamento";
                }
            }
            else if (infoBaseParametrizacao.DescricaoTipoNL == "Baixa de Pagamento de Licença-Prêmio")
            {
                texto += texto + " DE " + infoBaseItem.nmReduzidoCredor;
            }
            else if (infoBaseParametrizacao.DescricaoTipoNL == "Baixa de Pagamento Outros")
            {
                StringBuilder NotasFiscais = new StringBuilder();
                foreach (var item in itensNl)
                {
                    if (item.NumeroNotaFiscal > 0 && item.NumeroNotaFiscal != null)
                    {
                        NotasFiscais.Append(item.NumeroNotaFiscal + " / ");
                    }
                }

                texto += texto + NotasFiscais;
            }

            entidadeNl.DescricaoObservacao1 = SepararObservacao(texto, TAMANHOOBS, 1);
            entidadeNl.DescricaoObservacao2 = SepararObservacao(texto, TAMANHOOBS, 2);
            entidadeNl.DescricaoObservacao3 = SepararObservacao(texto, TAMANHOOBS, 3);
            entidadeNl.Valor = itensNl.Sum(x => x.ValorDocumento * 100) ?? 0;
            entidadeNl.AgrupamentoConfirmacao = confirmacaoPagamento.CodigoAgrupamentoConfirmacaoPagamento;
            entidadeNl.id_confirmacao_pagamento = confirmacaoPagamento.Id;
            AtribuiValorCampoAnoExercicio(infoBaseItem, entidadeNl);

            var NLPGOBRAS = AtribuiValoresQuandoNLPGOBRAS(usuario, infoBaseItem, infoBaseParametrizacao, entidadeNl);

            List<ReclassificacaoRetencaoEvento> eventos = new List<ReclassificacaoRetencaoEvento>();

            var parametrosGeracaoNl = new ParametrosGeracaoNl(confirmacaoPagamento, infoBaseItem, null, tiposRap);
            var tipoDocumento = infoBaseItem.NumeroDocumentoGerador.Substring(0, 2);
            parametrosGeracaoNl.IsNlPgObras = NLPGOBRAS;
            parametrosGeracaoNl.IsSubempenho = tipoDocumento.Equals(EnumTipoDocumento.Subempenho.GetHashCode().ToString("d2"));
            parametrosGeracaoNl.IsRap = tipoDocumento.Equals(EnumTipoDocumento.Rap.GetHashCode().ToString("d2"));
            parametrosGeracaoNl.valorAcumuladoNl = entidadeNl.Valor;
            entidadeNl.NumeroOriginalSiafemSiafisico = infoBaseItem.NumeroEmpenhoSiafem;
            entidadeNl.CodigoCredorOrganizacaoId = infoBaseItem.CodigoOrganizacaoCredor ?? 0;

            //***********novos campos - rafael magna see ************
            entidadeNl.DocumentoTipoId = infoBaseItem.IdTipoDocumento;
            //********** fim ****************************************

            GerarEventosSaida(parametrosGeracaoNl, eventos, infoBaseParametrizacao);

            entidadeNl.Valor = eventos.Sum(x => x.ValorUnitario);

            GerarEventoEntrada(parametrosGeracaoNl, entidadeNl, eventos, infoBaseParametrizacao, UGE, ugRegional.Descricao);

            entidadeNl.NormalEstorno = ((int)EnumNormalEstorno.Normal).ToString();


            entidadeNl.itensPertenceNl = itensNl.ToList();

            //entidade.AgrupamentoConfirmacao = parametrosGeracao.ConfirmacaoPagamento.CodigoAgrupamentoConfirmacaoPagamento; // geral



            return entidadeNl;
        }

        private Regional UGregional(string desc)
        {

            var dado = _regional.retornaRegional(desc);
    
            return dado.FirstOrDefault();
        }

        private bool AtribuiValoresQuandoNLPGOBRAS(Usuario usuario, ConfirmacaoPagamentoItem infoBaseItem, NlParametrizacao infoBaseParametrizacao, ReclassificacaoRetencao entidadeNl)
        {
            var NLPGOBRAS = false;
            if (VerrificarNlPgObras(infoBaseItem.NaturezaDespesa))
            {
                var infoNe = _commomService.ConsultaNe(infoBaseItem.NumeroEmpenhoSiafem, usuario);

                // Se sim quer dizer que temos uma NLPGOBRAS
                if (!string.IsNullOrEmpty(infoNe.IdentificadorObra))
                {
                    entidadeNl.ReclassificacaoRetencaoTipoId = EnumTipoReclassificacaoRetencao.PagamentoObrasSemOB.GetHashCode();
                    entidadeNl.CodigoEvento = infoBaseParametrizacao.Eventos.FirstOrDefault().NumeroEvento;
                    entidadeNl.CodigoFonte = infoBaseItem.NumeroFonteSiafem;
                    entidadeNl.CodigoInscricao = infoBaseItem.NumeroEmpenhoSiafem;
                    entidadeNl.CodigoClassificacao = infoBaseParametrizacao.Eventos.FirstOrDefault().NumeroClassificacao;
                    var anoMedicao = infoNe.IdentificadorObra.Substring(9,4);
                    var mesMedicao = infoNe.IdentificadorObra.Substring(14,2);

                    entidadeNl.MesMedicao = mesMedicao;
                    entidadeNl.AnoMedicao = anoMedicao;
                    NLPGOBRAS = true;
                }
                else
                {
                    entidadeNl.ReclassificacaoRetencaoTipoId = EnumTipoReclassificacaoRetencao.NotaLancamento.GetHashCode();
                }
            }
            else
            {
                entidadeNl.ReclassificacaoRetencaoTipoId = EnumTipoReclassificacaoRetencao.NotaLancamento.GetHashCode();
            }

            return NLPGOBRAS;
        }

        private static void AtribuiValorCampoAnoExercicio(ConfirmacaoPagamentoItem infoBaseItem, ReclassificacaoRetencao entidadeNl)
        {
            var tipoDocumento = infoBaseItem.NumeroDocumentoGerador.Substring(0, 2);
            var isSubempenho = tipoDocumento.Equals(EnumTipoDocumento.Subempenho.GetHashCode().ToString("d2"));
            var isRap = tipoDocumento.Equals(EnumTipoDocumento.Rap.GetHashCode().ToString("d2"));


            string ano = "20";

            if (isSubempenho)
            {
                ano += infoBaseItem.NumeroDocumentoGerador.Substring(5, 2);
            }
            else if (isRap)
            {
                ano = infoBaseItem.NumeroDocumentoGerador.Substring(2, 2);
            }

            entidadeNl.AnoExercicio = int.Parse(ano);
        }

        private ReclassificacaoRetencao NlPorRepasseFinanceiro(List<ConfirmacaoPagamentoItem> itens, ReclassificacaoRetencao rr)
        {


            //Até aqui chegamos  com os itens separados por Tipo de Despesa e temos setados as propiedades comuns
            //fazer as somas para nota de lançamento com a propiedade (Itens) e salvar parametrosGeracao


            //if (parametrizacao != null)
            //{

            //    rr.Valor = itens.Sum(x => x.ValorDocumento) ?? 0;

            //    // TODO valor
            //    // TODO UG Favorecido já definido no começo?
            //    // TODO Gestão Favorecido já definido no começo?
            //    rr.DescricaoObservacao1 = SepararObservacao(parametrizacao.Observacao, TAMANHOOBS, 1);
            //    rr.DescricaoObservacao2 = SepararObservacao(parametrizacao.Observacao, TAMANHOOBS, 2);
            //    rr.DescricaoObservacao3 = SepararObservacao(parametrizacao.Observacao, TAMANHOOBS, 3);

            //}

            return rr;

        }

        internal class ParametrosGeracaoNl
        {
            public List<ParaRestoAPagar> TiposRap { get; set; }
            public ConfirmacaoPagamento ConfirmacaoPagamento { get; set; }
            public List<NlParametrizacao> ParametrizacoesNl { get; set; }
            public ConfirmacaoPagamentoItem ItemConfirmacao { get; set; }
            public string Ug { get; set; }
            public bool IsSubempenho { get; set; }
            public bool IsRap { get; set; }
            public int AnoAtual { get; set; }
            public int Ano { get; set; }
            public bool IsNlPgObras { get; internal set; }
            public decimal valorAcumuladoNl { get; set; }

            public ParametrosGeracaoNl(ConfirmacaoPagamento confirmacaoPagamento, ConfirmacaoPagamentoItem itemConfirmacao, List<NlParametrizacao> parametrizacoes, List<ParaRestoAPagar> TiposRap)
            {
                this.ConfirmacaoPagamento = confirmacaoPagamento;
                this.ParametrizacoesNl = parametrizacoes;
                this.ItemConfirmacao = itemConfirmacao;
                this.TiposRap = TiposRap;
            }
        }

        private string FormatarAnoMedicao(string referencia)
        {
            // Ano da Medição = Apenas para NLPGObras, os dígitos da posição 27 e 28 do campo Referência 
            // retornado da consulta de pagamentos a confirmar, completar o ano para formatar 4 dígitos(2017).
            if (!String.IsNullOrEmpty(referencia))
            {
                var anoReferencia = referencia.Substring(26, 2);

                return string.Format("20{0}", anoReferencia);
            }

            return string.Empty;
        }
        private string FormatarMesMedicao(string referencia)
        {
            // Mês da medição = Apenas para NLPGObras, os dígitos da posição 24 e 25  do campo Referência 
            // retornado da consulta de pagamentos a confirmar, 
            if (!String.IsNullOrEmpty(referencia))
            {
                var mesReferencia = referencia.Substring(23, 2);

                return mesReferencia;
            }

            return string.Empty;
        }

        private bool VerrificarNlPgObras(int? naturezaDespesa)
        {
            // TODO implementar regra NLPGObras
            // FA3 – Notas de Lançamento de baixa para pagamento de obras - NLPGObras[RDN1][RDN4]
            // 1 – Para o cadastro das NL’s de Obra, se o nº do CED retornado da consulta de pagamentos a confirmar 
            // o primeiro digito for = 4 ou o primeiro digito = 3 e o quinto e sexto digito = “3” e “9”, 
            // o sistema deverá acionar o WebService do SIAFEM(CONNE) enviando a Unidade Gestora, a Gestão e o Número do Empenho 
            // para verificar se o empenho possui identificador da obra e se retorno positivo, com os dados retornados da consulta 
            // pagamentos a confirmar o sistema deverá cadastrar uma NLPGObras;

            try
            {
                if (naturezaDespesa.HasValue)
                {
                    var ced = naturezaDespesa.Value.ToString().RemoveSpecialChar();
                    var primeiroDigito = ced.Substring(0, 1);
                    var quintoSextoDigitos = ced.Substring(4, 2);

                    if (primeiroDigito.Equals("3") || primeiroDigito.Equals("4") && (quintoSextoDigitos.Equals("39"))) // TODO remover hardcode
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception)
            {

                return false;
            }

        }

        private static string SepararObservacao(string observacaoInteira, int tamanho, int posicao)
        {
            int inicio = (posicao - 1) * tamanho;
            int maximo = observacaoInteira.Length;
            var restante = maximo - inicio;
            tamanho = restante >= tamanho ? tamanho : restante;

            string obs = "";
            if (inicio < tamanho)
            {
                var selectedText = observacaoInteira.Substring(inicio, tamanho);
                obs = string.IsNullOrWhiteSpace(selectedText) ? null : selectedText;
            }


            return obs;
        }
        private static void GerarEventoEntradaISSQN(ParametrosGeracaoNl parametrosGeracao, ReclassificacaoRetencao rr, List<ReclassificacaoRetencaoEvento> eventos, NlParametrizacao parametrizacao, ConfirmacaoPagamentoTotais dado, string uge)
        {

            var eventosEntradaParametrizado = parametrizacao.Eventos.Where(x => x.EntradaSaidaDescricao.Trim().Contains(EnumDirecaoEvento.Entrada.ToString())).OrderBy(x => x.Id);
            if (eventosEntradaParametrizado != null)
            {
                var eventosFiltrados = RetornaEventosFiltrandoPeloTipoDocumento(parametrosGeracao.IsRap, eventosEntradaParametrizado);

                foreach (var eventoEntradaParametrizado in eventosFiltrados)
                {

                    rr.CodigoEvento = eventoEntradaParametrizado.NumeroEvento.ToString();
                    rr.CodigoClassificacao = eventoEntradaParametrizado.NumeroClassificacao.ToString();

                    var evento = new ReclassificacaoRetencaoEvento();
                    evento.InscricaoEvento = "PE0" + uge;
                    evento.NumeroEvento = eventoEntradaParametrizado.NumeroEvento.ToString();
                    evento.Classificacao = eventoEntradaParametrizado.NumeroClassificacao.ToString();
                    evento.Fonte = dado.NrFonteLista;
                    evento.ValorUnitario = Convert.ToInt32(dado.VrTotalConfirmarISSQN * 100);
                    eventos.Add(evento);
                }



                rr.Eventos = eventos;
            }

        }
        private static void GerarEventoEntradaIR(ParametrosGeracaoNl parametrosGeracao, ReclassificacaoRetencao rr, List<ReclassificacaoRetencaoEvento> eventos, NlParametrizacao parametrizacao, ConfirmacaoPagamentoTotais dado)
        {

            var eventosEntradaParametrizado = parametrizacao.Eventos.Where(x => x.EntradaSaidaDescricao.Trim().Contains(EnumDirecaoEvento.Entrada.ToString())).OrderBy(x => x.Id);
            if (eventosEntradaParametrizado != null)
            {
                var eventosFiltrados = RetornaEventosFiltrandoPeloTipoDocumento(parametrosGeracao.IsRap, eventosEntradaParametrizado);

                foreach (var eventoEntradaParametrizado in eventosFiltrados)
                {

                    rr.CodigoEvento = eventoEntradaParametrizado.NumeroEvento.ToString();
                    rr.CodigoClassificacao = eventoEntradaParametrizado.NumeroClassificacao.ToString();

                    var evento = new ReclassificacaoRetencaoEvento();
                    evento.InscricaoEvento = "";
                    evento.NumeroEvento = eventoEntradaParametrizado.NumeroEvento.ToString();
                    evento.Classificacao = eventoEntradaParametrizado.NumeroClassificacao.ToString();
                    evento.Fonte = dado.NrFonteLista;
                    evento.ValorUnitario = Convert.ToInt32(dado.VrTotalConfirmarIR * 100);
                    eventos.Add(evento);
                }



                rr.Eventos = eventos;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parametrosGeracao"></param>
        /// <param name="rr"></param>
        /// <param name="eventos"></param>
        /// <param name="parametrizacao"></param>
        private static void GerarEventoEntradaTotal(ParametrosGeracaoNl parametrosGeracao, ReclassificacaoRetencao rr, List<ReclassificacaoRetencaoEvento> eventos, NlParametrizacao parametrizacao, IEnumerable<ConfirmacaoPagamentoTotais> Lista)
        {

            var eventosEntradaParametrizado = parametrizacao.Eventos.Where(x => x.EntradaSaidaDescricao.Trim().Contains(EnumDirecaoEvento.Entrada.ToString())).OrderBy(x => x.Id);
            if (eventosEntradaParametrizado != null)
            {
                var eventosFiltrados = RetornaEventosFiltrandoPeloTipoDocumento(parametrosGeracao.IsRap, eventosEntradaParametrizado);

                foreach (var eventoEntradaParametrizado in eventosFiltrados)
                {
                    foreach (var item in Lista)
                    {
                        rr.CodigoEvento = eventoEntradaParametrizado.NumeroEvento.ToString();
                        rr.CodigoClassificacao = eventoEntradaParametrizado.NumeroClassificacao.ToString();

                        var evento = new ReclassificacaoRetencaoEvento();
                        evento.InscricaoEvento = "PE0" + parametrizacao.UnidadeGestora;
                        evento.NumeroEvento = eventoEntradaParametrizado.NumeroEvento.ToString();
                        evento.Classificacao = eventoEntradaParametrizado.NumeroClassificacao.ToString();
                        evento.Fonte = item.NrFonteLista; // eventoEntradaParametrizado.NumeroFonte.ToString();
                        evento.ValorUnitario = Convert.ToInt32(item.VrTotalFonteLista * 100);
                        eventos.Add(evento);
                    }

                }

                rr.Eventos = eventos;
            }

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="parametrosGeracao"></param>
        /// <param name="rr"></param>
        /// <param name="eventos"></param>
        /// <param name="parametrizacao"></param>
        private static void GerarEventoEntrada(ParametrosGeracaoNl parametrosGeracao, ReclassificacaoRetencao rr, List<ReclassificacaoRetencaoEvento> eventos, NlParametrizacao parametrizacao, string UGE, string ugRegional = null)
        {
            var eventosEntradaParametrizado = parametrizacao.Eventos.Where(x => x.EntradaSaidaDescricao.Trim().Contains(EnumDirecaoEvento.Entrada.ToString())).OrderBy(x => x.Id);
            if (eventosEntradaParametrizado != null)
            {
                var eventosFiltrados = RetornaEventosFiltrandoPeloTipoDocumento(parametrosGeracao.IsRap, eventosEntradaParametrizado);

                foreach (var eventoEntradaParametrizado in eventosFiltrados)
                {
                    rr.CodigoEvento = eventoEntradaParametrizado.NumeroEvento.ToString();
                    rr.CodigoClassificacao = eventoEntradaParametrizado.NumeroClassificacao.ToString();

                    var evento = new ReclassificacaoRetencaoEvento();
                    if (parametrizacao.DescricaoTipoNL == "Baixa de Pagamento de Fornecedor")
                    {
                        evento.InscricaoEvento = "PE0" + UGE;
                    }
                    else if (parametrizacao.DescricaoTipoNL == "Repasse para Regionais")
                    {
                        evento.InscricaoEvento = "PE0" + ugRegional;
                    }
                    else if (parametrizacao.DescricaoTipoNL == "Baixa de Jeton-Jari")
                    {
                        evento.InscricaoEvento = "PE0" + UGE;
                    }
                    else if (parametrizacao.DescricaoTipoNL == "Baixa de Pagamento de Licença-Prêmio")
                    {
                        evento.InscricaoEvento = "PE0" + UGE;
                    }
                    else if (parametrizacao.DescricaoTipoNL == "Baixa de Pagamento Outros")
                    {
                        evento.InscricaoEvento = "PE0" + UGE;
                    }
                    else if (parametrizacao.DescricaoTipoNL == "Baixa de Devolução de Caução")
                    {
                        evento.InscricaoEvento = "PE0" + UGE;
                    }
                    else if (parametrizacao.DescricaoTipoNL == "Baixa de Pagamento de Adiantamento")
                    {
                        evento.InscricaoEvento = "PE0" + UGE;
                    }
                    else if (parametrizacao.DescricaoTipoNL == "Baixa de Pagamento de Restituição de Multa")
                    {
                        evento.InscricaoEvento = "PE0" + UGE;
                    }
                    else if (parametrizacao.DescricaoTipoNL == "Baixa de Pagamento de Perito do Quadro/ Judicial")
                    {
                        evento.InscricaoEvento = "PE0" + UGE;
                    }
                    else if (parametrizacao.DescricaoTipoNL == "Baixa de Pagamento de Bonificação por Resultados")
                    {
                        evento.InscricaoEvento = "PE0" + UGE;
                    }
                    else
                    {
                        evento.InscricaoEvento = "PE0" + UGE;
                    }


                    evento.NumeroEvento = eventoEntradaParametrizado.NumeroEvento.ToString();
                    evento.Classificacao = eventoEntradaParametrizado.NumeroClassificacao.ToString();
                    if (parametrizacao.DescricaoTipoNL == "Baixa de Devolução de Caução")
                    {
                        evento.Fonte = eventoEntradaParametrizado.NumeroFonte;
                    }
                    else
                    {
                        evento.Fonte = parametrosGeracao.ItemConfirmacao.NumeroFonteSiafem.ToString();
                    }
                    evento.ValorUnitario = Convert.ToInt32(parametrosGeracao.valorAcumuladoNl);
                    eventos.Add(evento);
                }

                rr.Eventos = eventos;
            }
        }
        private static void GerarEventosSaidaISSQN(ParametrosGeracaoNl parametrosGeracao, List<ReclassificacaoRetencaoEvento> eventos, NlParametrizacao parametrizacao, ConfirmacaoPagamentoTotais Dado)
        {

            var eventosSaidaParametrizado = parametrizacao.Eventos.Where(x => x.EntradaSaidaDescricao.Trim().Contains(EnumDirecaoEvento.Saida.ToString())).OrderBy(x => x.Id);
            if (eventosSaidaParametrizado != null)
            {
                var eventosFiltrados = RetornaEventosFiltrandoPeloTipoDocumento(parametrosGeracao.IsRap, eventosSaidaParametrizado);

                foreach (var eventoSaidaParametrizado in eventosFiltrados)
                {

                    var evento = new ReclassificacaoRetencaoEvento();
                    evento.InscricaoEvento = "";
                    evento.Classificacao = eventoSaidaParametrizado.NumeroClassificacao.ToString();
                    evento.Fonte = Dado.NrFonteLista;
                    evento.NumeroEvento = eventoSaidaParametrizado.NumeroEvento.ToString();
                    evento.ValorUnitario = Convert.ToInt32(Dado.VrTotalConfirmarISSQN * 100);

                    eventos.Add(evento);
                }

            }
        }

        private static void GerarEventosSaidaIR(ParametrosGeracaoNl parametrosGeracao, List<ReclassificacaoRetencaoEvento> eventos, NlParametrizacao parametrizacao, ConfirmacaoPagamentoTotais Dado)
        {

            var eventosSaidaParametrizado = parametrizacao.Eventos.Where(x => x.EntradaSaidaDescricao.Trim().Contains(EnumDirecaoEvento.Saida.ToString())).OrderBy(x => x.Id);
            if (eventosSaidaParametrizado != null)
            {
                var eventosFiltrados = RetornaEventosFiltrandoPeloTipoDocumento(parametrosGeracao.IsRap, eventosSaidaParametrizado);

                foreach (var eventoSaidaParametrizado in eventosFiltrados)
                {

                    var evento = new ReclassificacaoRetencaoEvento();
                    evento.InscricaoEvento = String.Format("{0}{1}", parametrizacao.UnidadeGestora, parametrizacao.FavorecidaNumeroGestao);
                    evento.Classificacao = eventoSaidaParametrizado.NumeroClassificacao.ToString();
                    evento.Fonte = Dado.NrFonteLista;
                    evento.NumeroEvento = eventoSaidaParametrizado.NumeroEvento.ToString();
                    evento.ValorUnitario = Convert.ToInt32(Dado.VrTotalConfirmarIR * 100);

                    eventos.Add(evento);


                }



            }
        }


        /// <summary>
        /// GerarEventosSaidaTotal
        /// </summary>
        /// <param name="parametrosGeracao"></param>
        /// <param name="eventos"></param>
        /// <param name="parametrizacao"></param>
        private static void GerarEventosSaidaTotal(ParametrosGeracaoNl parametrosGeracao, List<ReclassificacaoRetencaoEvento> eventos, NlParametrizacao parametrizacao, IEnumerable<ConfirmacaoPagamentoTotais> Lista)
        {

            var eventosSaidaParametrizado = parametrizacao.Eventos.Where(x => x.EntradaSaidaDescricao.Trim().Contains(EnumDirecaoEvento.Saida.ToString())).OrderBy(x => x.Id);
            if (eventosSaidaParametrizado != null)
            {
                var eventosFiltrados = RetornaEventosFiltrandoPeloTipoDocumento(parametrosGeracao.IsRap, eventosSaidaParametrizado);

                foreach (var eventoSaidaParametrizado in eventosFiltrados)
                {
                    foreach (var item in Lista)
                    {
                        var evento = new ReclassificacaoRetencaoEvento();
                        evento.InscricaoEvento = String.Format("{0}{1}{2}", parametrosGeracao.ItemConfirmacao.NumeroBancoPagador, parametrosGeracao.ItemConfirmacao.NumeroAgenciaPagador, parametrosGeracao.ItemConfirmacao.NumeroContaPagador);
                        evento.Classificacao = eventoSaidaParametrizado.NumeroClassificacao.ToString();
                        evento.Fonte = item.NrFonteLista; // eventoSaidaParametrizado.NumeroFonte.ToString();
                        evento.NumeroEvento = eventoSaidaParametrizado.NumeroEvento.ToString();
                        evento.ValorUnitario = Convert.ToInt32(item.VrTotalFonteLista * 100);

                        eventos.Add(evento);
                    }

                }



            }
        }

        /// <summary>
        /// GerarEventosSaida
        /// </summary>
        /// <param name="parametrosGeracao"></param>
        /// <param name="eventos"></param>
        /// <param name="parametrizacao"></param>
        private static void GerarEventosSaida(ParametrosGeracaoNl parametrosGeracao, List<ReclassificacaoRetencaoEvento> eventos, NlParametrizacao parametrizacao)
        {
            var eventosSaidaParametrizado = parametrizacao.Eventos.Where(x => x.EntradaSaidaDescricao.Trim().Contains(EnumDirecaoEvento.Saida.ToString())).OrderBy(x => x.Id);
            if (eventosSaidaParametrizado != null)
            {
                var eventosFiltrados = RetornaEventosFiltrandoPeloTipoDocumento(parametrosGeracao.IsRap, eventosSaidaParametrizado);

                foreach (var eventoSaidaParametrizado in eventosFiltrados)
                {
                    var evento = new ReclassificacaoRetencaoEvento();
                    if (parametrizacao.DescricaoTipoNL == "Baixa de Pagamento de Fornecedor")
                    {
                        evento.InscricaoEvento = String.Format("{0}", parametrosGeracao.ItemConfirmacao.NumeroEmpenhoSiafem);
                    }
                    else if (parametrizacao.DescricaoTipoNL == "Baixa de Jeton-Jari")
                    {
                        evento.InscricaoEvento = String.Format("{0}", parametrosGeracao.ItemConfirmacao.NumeroEmpenhoSiafem);
                    }
                    else if (parametrizacao.DescricaoTipoNL == "Baixa de Pagamento Outros")
                    {
                        evento.InscricaoEvento = String.Format("{0}", parametrosGeracao.ItemConfirmacao.NumeroEmpenhoSiafem);
                    }
                    else if (parametrizacao.DescricaoTipoNL == "Baixa de Devolução de Caução")
                    {
                        evento.InscricaoEvento = String.Format("{0}", parametrosGeracao.ItemConfirmacao.NumeroCnpjCpfUgCredor);
                    }
                    else if (parametrizacao.DescricaoTipoNL == "Baixa de Pagamento de Adiantamento")
                    {
                        evento.InscricaoEvento = String.Format("{0}", parametrosGeracao.ItemConfirmacao.NumeroEmpenhoSiafem);
                    }
                    else if (parametrizacao.DescricaoTipoNL == "Baixa de Pagamento de Restituição de Multa")
                    {
                        evento.InscricaoEvento = String.Format("{0}", parametrosGeracao.ItemConfirmacao.NumeroEmpenhoSiafem);
                    }
                    else if (parametrizacao.DescricaoTipoNL == "Baixa de Pagamento de Perito do Quadro/ Judicial")
                    {
                        evento.InscricaoEvento = String.Format("{0}", parametrosGeracao.ItemConfirmacao.NumeroEmpenhoSiafem);
                    }
                    else if (parametrizacao.DescricaoTipoNL == "Baixa de Pagamento de Bonificação por Resultados")
                    {
                        evento.InscricaoEvento = String.Format("{0}", parametrosGeracao.ItemConfirmacao.NumeroEmpenhoSiafem);
                    }
                    else
                    {
                        evento.InscricaoEvento = String.Format("{0}{1}{2}", parametrosGeracao.ItemConfirmacao.NumeroBancoPagador, parametrosGeracao.ItemConfirmacao.NumeroAgenciaPagador, parametrosGeracao.ItemConfirmacao.NumeroContaPagador);
                    }

                    evento.Classificacao = eventoSaidaParametrizado.NumeroClassificacao.ToString();

                    if (parametrizacao.DescricaoTipoNL == "Baixa de Devolução de Caução")
                    {
                        evento.Fonte = eventoSaidaParametrizado.NumeroFonte;
                    }
                    else
                    {
                        evento.Fonte = parametrosGeracao.ItemConfirmacao.NumeroFonteSiafem.ToString();
                    }

                    evento.NumeroEvento = eventoSaidaParametrizado.NumeroEvento.ToString();
                    evento.ValorUnitario = Convert.ToInt32(parametrosGeracao.valorAcumuladoNl);

                    eventos.Add(evento);
                }



            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="IsRap"></param>
        /// <param name="eventosSaidaParametrizado"></param>
        /// <returns></returns>
        private static IEnumerable<NlParametrizacaoEvento> RetornaEventosFiltrandoPeloTipoDocumento(bool IsRap, IOrderedEnumerable<NlParametrizacaoEvento> eventosSaidaParametrizado)
        {
            var eventos = eventosSaidaParametrizado.Where(x => x.IdDocumentoTipo == EnumTipoDocumento.Subempenho.GetHashCode());

            if (IsRap)
            {
                eventos = eventosSaidaParametrizado.Where(x => x.IdDocumentoTipo == EnumTipoDocumento.Rap.GetHashCode());
            }

            return eventos;
        }
    }


}
