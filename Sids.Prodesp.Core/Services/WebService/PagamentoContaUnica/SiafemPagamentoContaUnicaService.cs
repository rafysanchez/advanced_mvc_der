﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using Sids.Prodesp.Core.Base;
using Sids.Prodesp.Core.Extensions;
using Sids.Prodesp.Infrastructure.Helpers;
using Sids.Prodesp.Model.Entity.PagamentoContaUnica.ListaBoletos;
using Sids.Prodesp.Model.Entity.PagamentoContaUnica.ProgramacaoDesembolso;
using Sids.Prodesp.Model.Entity.PagamentoContaUnica.ReclassificacaoRetencao;
using Sids.Prodesp.Model.Interface.Log;
using Sids.Prodesp.Model.Interface.PagamentoContaUnica.Base;
using Sids.Prodesp.Model.Interface.Service.PagamentoContaUnica;
using Sids.Prodesp.Model.ValueObject.Service.Siafem.Empenho;
using Sids.Prodesp.Model.ValueObject.Service.Siafem.LiquidacaoDespesa;
using Sids.Prodesp.Model.ValueObject.Service.Siafem.PagamentoContaUnica;
using desc = Sids.Prodesp.Model.ValueObject.Service.Siafem.LiquidacaoDespesa.desc;
using descricao = Sids.Prodesp.Model.ValueObject.Service.Siafem.PagamentoContaUnica.descricao;
using Repeticao = Sids.Prodesp.Model.ValueObject.Service.Siafem.LiquidacaoDespesa.Repeticao;
using SIAFDOC = Sids.Prodesp.Model.ValueObject.Service.Siafem.PagamentoContaUnica.SIAFDOC;
using Sids.Prodesp.Model.Exceptions;
using Sids.Prodesp.Model.Enum;
using Sids.Prodesp.Model.Entity.PagamentoContaUnica.ImpressaoRelacaoRERT;
using Sids.Prodesp.Model.ValueObject.Service.Siafem.PagamentoContaUnica.ImpressaoRelacaoRERT;
using System.IO;
using System.Xml.Serialization;
using Sids.Prodesp.Infrastructure;

namespace Sids.Prodesp.Core.Services.WebService.PagamentoContaUnica
{
    public class SiafemPagamentoContaUnicaService : BaseService
    {
        private readonly ISiafemPagamentoContaUnica _contaUnica;

        public SiafemPagamentoContaUnicaService(ILogError logError) : base(logError) { }

        public SiafemPagamentoContaUnicaService(ILogError logError, ISiafemPagamentoContaUnica contaUnica) : base(logError)
        {
            _contaUnica = contaUnica;
        }

        public string InserirReclassificacaoRetencao(string login, string password, string unidadeGestora, ReclassificacaoRetencao entity)
        {
            try
            {
                var siafdoc = SiafemDocumentProvider(entity);
                var result = _contaUnica.InserirReclassificacaoRetencao(login, password, unidadeGestora, siafdoc).ToXml("SIAFEM");

                return ReturnMessageWithStatusForSiafemService(result);

            }
            catch (Exception e)
            {
                HttpContext.Current.Session["xml"] = HttpContext.Current.Session["xmlSiafem"];
                HttpContext.Current.Session["xmlSiafem"] = "";
                throw new SidsException(e.Message);
            }
        }

        public string InserirListaBoletos(string login, string password, string unidadeGestora, ListaBoletos entity, ListaCodigoBarras boletos)
        {
            try
            {
                var result = new XmlDocument();

                Documento document = new Documento
                {
                    ListaCodigoBarras = boletos,
                    ListaBoletos = entity
                };
                var siafdoc = new SIAFDOC();

                siafdoc = string.IsNullOrWhiteSpace(entity.NumeroSiafem) ? SiafemBoletosDocumentProvider(document) : SiafemAltBoletosDocumentProvider(document);

                result = _contaUnica.InserirListaBoletos(login, password, unidadeGestora, siafdoc).ToXml("SIAFEM");


                return ReturnMessageWithStatusListaBoletosForSiafemService(result);
            }
            catch (Exception e)
            {
                HttpContext.Current.Session["xml"] = HttpContext.Current.Session["xmlSiafem"];
                HttpContext.Current.Session["xmlSiafem"] = "";
                throw new SidsException(e.Message);
            }
        }
        public string InserirProgramacaoDesembolso(string login, string password, string unidadeGestora, ProgramacaoDesembolso entity)
        {

            try
            {
                var siafdoc = CreateSiafemDocProg(entity);
                var result = _contaUnica.InserirProgramacaoDesembolso(login, password, unidadeGestora, siafdoc).ToXml("SIAFEM");

                return ReturnMessageWithStatusForSiafemService(result);

            }
            catch (Exception e)
            {
                if (HttpContext.Current == null) throw new SidsException(e.Message);

                HttpContext.Current.Session["xml"] = HttpContext.Current.Session["xmlSiafem"];
                HttpContext.Current.Session["xmlSiafem"] = "";
                throw new SidsException(e.Message);
            }
        }
        public string InserirProgramacaoDesembolsoSiafisico(string login, string password, string unidadeGestora, ProgramacaoDesembolso entity)
        {
            try
            {

                var sfcodoc = new Model.ValueObject.Service.Siafem.LiquidacaoDespesa.SFCODOC
                {
                    cdMsg = "SFCOPDBEC001",
                    SiafisicoDocPDBEC = new SiafisicoDocPDBEC
                    {
                        documento = new documento
                        {
                            UG = unidadeGestora,
                            Gestao = entity.CodigoGestao,
                            DataEmissao = entity.DataEmissao.ToString("ddMMMyyyy").ToUpper(),
                            NumeroCT = entity.NumeroCT,
                            NumeroNE = entity.NumeroNE,
                            NumeroNL = entity.NumeroNLReferencia,
                            Observacao = entity.Obs,
                            UGPagadora = entity.NumeroCnpjcpfCredor,
                            Banco = entity.NumeroBancoPagto,
                            Agencia = entity.NumeroAgenciaPagto,
                            Conta = entity.NumeroContaPagto
                        }
                    }
                };

                //var xmlmooc = @"<MSG><BCMSG><Doc_Estimulo><SFCODOC><cdMsg>SFCOPDBEC001</cdMsg><SiafisicoDocPDBEC><documento><DataEmissao>26DEZ2017</DataEmissao><UG>162101</UG><Gestao>16055</Gestao><NumeroNE>2017NE00786</NumeroNE><Observacao>teste</Observacao><NumeroNL>2017NL00540</NumeroNL><Banco>001</Banco><Agencia>01893</Agencia><Conta>000000012</Conta><UGPagadora>162184</UGPagadora></documento></SiafisicoDocPDBEC></SFCODOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SFCODOC><cdMsg>SFCOPDBEC001</cdMsg><SiafisicoDocPDBEC><documento><Status>true</Status><NumeroPD>2017PD00033</NumeroPD><DataEmissao>26DEZ2017</DataEmissao><MsgErro>PROGRAMACAO DE DESEMBOLSO CONTABILIZADA</MsgErro></documento></SiafisicoDocPDBEC></SFCODOC></Doc_Retorno></SISERRO></MSG>";
                //var result = xmlmooc.ToXml("SFCODOC");

                var result = _contaUnica.InserirProgramacaoDesembolsoSiafisico(login, password, unidadeGestora, sfcodoc).ToXml("SFCODOC");

                var numeroPD = result.GetElementsByTagName("NumeroPD").Item(0)?.InnerText;
                var MsgErro = result.GetElementsByTagName("MsgErro").Item(0)?.InnerText;

                if (String.IsNullOrWhiteSpace(numeroPD))
                    throw new SidsException($"SIAFISICO - {MsgErro}");

                return numeroPD;


            }
            catch (Exception e)
            {
                if (HttpContext.Current == null) throw new SidsException(e.Message);

                HttpContext.Current.Session["xml"] = HttpContext.Current.Session["xmlSiafem"];
                HttpContext.Current.Session["xmlSiafem"] = "";
                throw new SidsException(e.Message);
            }
        }

        public string CancelarProgramacaoDesembolso(string login, string password, string unidadeGestora, IProgramacaoDesembolso entity)
        {

            try
            {
                var siafdoc = CreateSiafemDocCanProg(entity);
                var result = _contaUnica.CancelarProgramacaoDesembolso(login, password, unidadeGestora, siafdoc).ToXml("SIAFEM");

                return ReturnMessageWithStatusForSiafemService(result);

            }
            catch (Exception e)
            {
                if (HttpContext.Current == null) throw new SidsException(e.Message);

                HttpContext.Current.Session["xml"] = HttpContext.Current.Session["xmlSiafem"];
                HttpContext.Current.Session["xmlSiafem"] = "";
                throw new SidsException(e.Message);
            }
        }


        public ConsultaPd ConsultaPD(string login, string password, string unidadeGestora, string numeroSiafemSiafisco)
        {
            try
            {
                var document = CreateSiafemConsultaPd(unidadeGestora, numeroSiafemSiafisco);
                var response = _contaUnica.Consultar(login, password, unidadeGestora, document);

                return RespostaSiafemToObject<ConsultaPd>(response);
            }
            catch (Exception e)
            {
                throw new SidsException(e.Message);
            }
        }

        public ConsultaPd ConsultaPD(string login, string password, string unidadeGestoraLiquidante, string gestaoLiquidante, string unidadeGestora, string NumeroPD)
        {
            try
            {
                var siafdoc = new SIAFDOC
                {
                    cdMsg = "SIAFConsultaPD",
                    SiafemDocConsultaPD = new SiafemDocConsultaPD()
                    {
                        documento = new documento
                        {
                            UnidadeGestora = unidadeGestoraLiquidante,
                            Gestao = gestaoLiquidante,
                            NumeroPD = NumeroPD
                        }
                    }
                };

                var result = _contaUnica.ConsultaPD(login, password, unidadeGestoraLiquidante, siafdoc);
                //var result = @"<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFConsultaPD</cdMsg><SiafemDocConsultaPD><documento><UnidadeGestora>200164</UnidadeGestora><Gestao>00001</Gestao><NumeroPD>2017PD00001</NumeroPD></documento></SiafemDocConsultaPD></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno>
                //<SIAFDOC>
                //    <SiafemDocConsultaPD>
                //  <StatusOperacao>true</StatusOperacao>
                //   <documento>
                //    <UG>200164</UG>
                //    <NumPD>2018PD00068</NumPD>
                //    <CGC_CPF_UG_Favorecida>62577929000135  - CIA.PROC.DADOS DO ESTADO DE SAO PAULO-PRODE</CGC_CPF_UG_Favorecida>
                //    <ContaCorrenteFavorecido>130000108</ContaCorrenteFavorecido>
                //    <BancoFavorecido>001</BancoFavorecido>
                //    <AgenciaFavorecido>06972</AgenciaFavorecido>
                //    <AgenciaPagadora>01897</AgenciaPagadora>
                //    <BancoPagador>001</BancoPagador>
                //    <Classificacao1></Classificacao1>
                //    <Classificacao2></Classificacao2>
                //    <Classificacao3></Classificacao3>
                //    <HoraLanc>13:30</HoraLanc>
                //    <DataLanc>09MAR2017</DataLanc>
                //    <Consultaas>15:41</Consultaas>
                //    <ConsultaEm>28/06/2017</ConsultaEm>
                //    <ContaCorrentePagadora>013000012</ContaCorrentePagadora>
                //    <DataEmissao>21MAI2018</DataEmissao>
                //    <DataVencimento>27MAI2018</DataVencimento>
                //    <Evento1>700601</Evento1>
                //    <Evento2></Evento2>
                //    <Evento3></Evento3>
                //    <Finalidade>PAGAMENTO DE CONSUMO</Finalidade>
                //    <Fonte1>001001001</Fonte1>
                //    <Fonte2></Fonte2>
                //    <Fonte3></Fonte3>
                //    <Gestao>00001   - ADMINIST. DIRETA</Gestao>
                //    <GestaoFavorecido></GestaoFavorecido>
                //    <GestaoPagadora>00001   - ADMINIST. DIRETA</GestaoPagadora>
                //    <InscricaoEvento1>2017NE00004</InscricaoEvento1>
                //    <InscricaoEvento2></InscricaoEvento2>
                //    <InscricaoEvento3></InscricaoEvento3>
                //    <RecDesp1>33903041</RecDesp1>
                //    <RecDesp2></RecDesp2>
                //    <RecDesp3></RecDesp3>
                //    <Lista></Lista>
                //    <NLRef>2017NL00019</NLRef>
                //    <Numero>2017PD00001</Numero>
                //    <OB>2017OB00001</OB>
                //    <Status>* PAGA *</Status>
                //    <UG>200164  - DEPTO. DE COMPRAS ELETRONICAS</UG>
                //    <UGPagadora>200001  - SECRETARIA DA FAZENDA</UGPagadora>
                //    <Lancadopor>USUARIO PUBLICO DO SIAFEM</Lancadopor>
                //    <Usuario>PUB. SIAFEM2017</Usuario>
                //    <Valor>2,12</Valor>
                //    <Valor1>2,12</Valor1>
                //    <Valor2></Valor2>
                //    <Valor3></Valor3>
                //    <Processo>PREGAO</Processo>
                //    <AnoMedicaoObra></AnoMedicaoObra>
                //    <AnoObra></AnoObra>
                //    <MesMedicaoObra></MesMedicaoObra>
                //    <MesObra></MesObra>
                //    <NumObra></NumObra>
                //    <TipoObra></TipoObra>
                //    <UGObra></UGObra>
                //    <OfertaCompra></OfertaCompra>
                //   </documento>
                //  <MsgRetorno></MsgRetorno>
                //    </SiafemDocConsultaPD>
                //</SIAFDOC>
                //</Doc_Retorno></SISERRO></MSG>";
                return RespostaSiafemToObject<ConsultaPd>(result);
            }
            catch (Exception e)
            {

                if (HttpContext.Current == null) throw new SidsException(e.Message);

                HttpContext.Current.Session["xml"] = HttpContext.Current.Session["xmlSiafem"];
                HttpContext.Current.Session["xmlSiafem"] = "";
                throw new SidsException(e.Message);
            }
        }

        public List<ConsultaOB> ListarOB(string login, string password, string unidadeGestora, string Gestao, string NumeroOB)
        {
            try
            {
                var siafdoc = new SIAFDOC
                {
                    //cdMsg = "SIAFConsultaOB",
                    cdMsg = "SIAFAUTORIZAOBVI",
                    //SiafemDocConsultaOB = new SiafemDocConsultaOB()
                    SiafemDocAutorizaOBVI = new SiafemDocAutorizaOBVI()
                    {
                        documento = new documento
                        {
                            UnidadeGestora = unidadeGestora,
                            Gestao = Gestao
                            //NumeroOB = NumeroOB
                        }
                    }
                };

                // // //xml's
                // var result = @"<SIAFDOC>
                // <cdMsg>SIAFAUTORIZAOBVI</cdMsg>
                // <SiafemDocAutorizaOBVI>
                // <documento>
                // <StatusOperacao>true</StatusOperacao>
                // <UnidadeGestora>180001</UnidadeGestora>
                // <MsgErro></MsgErro>
                // <repete>
                //     <ordem>0</ordem>
                //     <NoOB>2018OB00001</NoOB>
                //     <Favorecido>TESTE XXXX</Favorecido>
                //     <Despesa>00000000</Despesa>
                //     <Banco>1</Banco>
                //     <Valor>650,00</Valor>
                // </repete>
                // <repete>
                //     <ordem>1</ordem>
                //     <NoOB>2018OB00011</NoOB>
                //     <Favorecido>LIMPEL SISTEMAS DE SERVICOS S/</Favorecido>
                //     <Despesa>00000000</Despesa>
                //     <Banco>1</Banco>
                //     <Valor>840,00</Valor>
                // </repete>
                //     <repete>
                //     <ordem>2</ordem>
                //     <NoOB>2018OB00012</NoOB>
                //     <Favorecido>INSTITUTO NACIONAL DE SEGURIDA</Favorecido>
                //     <Despesa>00000000</Despesa>
                //     <Banco>1</Banco>
                //     <Valor>110,00</Valor>
                // </repete>
                // <repete>
                //     <ordem>3</ordem>
                //     <NoOB>2018OB00013</NoOB>
                //     <Favorecido>LIMPEL SISTEMAS DE SERVICOS S/</Favorecido>
                //     <Despesa>00000000</Despesa>
                //     <Banco>1</Banco>
                //     <Valor>385,00</Valor>
                //     </repete>
                //<repete>
                //     <ordem>4</ordem>
                //     <NoOB>2018OB00014</NoOB>
                //     <Favorecido>KALUNGA COMERCIO E INDUSTRIA G</Favorecido>
                //     <Despesa>00000000</Despesa>
                //     <Banco>1</Banco>
                //     <Valor>200,00</Valor>
                // </repete>
                // </documento>
                // </SiafemDocAutorizaOBVI>
                // </SIAFDOC>";

                //                var result = @"<SIAFDOC>
                //                <cdMsg>SIAFAUTORIZAOBVI</cdMsg>
                //                <SiafemDocAutorizaOBVI>
                //                <documento>
                //                <StatusOperacao>true</StatusOperacao>
                //                <UnidadeGestora>180001</UnidadeGestora>
                //                <MsgErro></MsgErro>
                //                <repete>
                //                    <ordem>0</ordem>
                //                    <NoOB>2018OB00213</NoOB>
                //                    <Favorecido>TESTE XXXX</Favorecido>
                //                    <Despesa>00000000</Despesa>
                //                    <Banco>1</Banco>
                //                    <Valor>650,00</Valor>
                //                </repete>
                //<repete>
                //                    <ordem>0</ordem>
                //                    <NoOB>2018OB00214</NoOB>
                //                    <Favorecido>TESTE XXXX</Favorecido>
                //                    <Despesa>00000000</Despesa>
                //                    <Banco>1</Banco>
                //                    <Valor>650,00</Valor>
                //                </repete>
                //<repete>
                //                    <ordem>0</ordem>
                //                    <NoOB>2018OB00215</NoOB>
                //                    <Favorecido>TESTE XXXX</Favorecido>
                //                    <Despesa>00000000</Despesa>
                //                    <Banco>1</Banco>
                //                    <Valor>650,00</Valor>
                //                </repete>
                //<repete>
                //                    <ordem>0</ordem>
                //                    <NoOB>2018OB00216</NoOB>
                //                    <Favorecido>TESTE XXXX</Favorecido>
                //                    <Despesa>00000000</Despesa>
                //                    <Banco>1</Banco>
                //                    <Valor>650,00</Valor>
                //                </repete>
                //                </documento>
                //                </SiafemDocAutorizaOBVI>
                //                </SIAFDOC>";

                var result = _contaUnica.ConsultaOB(login, password, unidadeGestora, siafdoc);
                return RespostaSiafemToObject<ConsultaOB>(result, "repete");

            }
            catch (Exception e)
            {

                if (HttpContext.Current == null) throw new SidsException(e.Message);

                HttpContext.Current.Session["xml"] = HttpContext.Current.Session["xmlSiafem"];
                HttpContext.Current.Session["xmlSiafem"] = "";
                throw new SidsException(e.Message);
            }
        }

        public List<ConsultaPd> ListarPD(string login, string password, string unidadeGestora, string UgLiquidante, string GestaoLiquidante, string UgPagadora, string GestaoPagadora, string Favorecido, string DataInicial, string DataFinal, string AnoInicial, string AnoFinal, string Opcao, string TipoOB, string PdCanceladTotal)
        {
            try
            {
                var siafdoc = new SIAFDOC
                {
                    cdMsg = "SIAFListaPd",
                    SiafemDocListaPd = new SiafemDocListaPd()
                    {
                        documento = new documento
                        {
                            UgLiquidante = UgLiquidante ?? "",
                            GestaoLiquidante = GestaoLiquidante ?? "",
                            UgPagadora = UgPagadora ?? "",
                            GestaoPagadora = GestaoPagadora ?? "",
                            Favorecido = Favorecido ?? "",
                            DataInicial = DataInicial ?? "",
                            DataFinal = DataFinal ?? "",
                            AnoInicial = AnoInicial ?? "",
                            AnoFinal = AnoFinal ?? "",
                            Opcao = Opcao ?? "0",
                            TipoOB = TipoOB ?? "",
                            PdCanceladTotal = PdCanceladTotal ?? ""
                        }
                    }
                };



                var result = _contaUnica.ListaPd(login, password, unidadeGestora, siafdoc);
                //var result = @"<SIAFDOC>
                //<cdMsg>ListaPd</cdMsg>
                //<ListaPd>
                //<documento>
                //<StatusOperacao>true</StatusOperacao>
                //<MsgRetorno></MsgRetorno>
                //<repete>
                //<ordem>1</ordem>
                //<Favorecido>46.179.941/0001-35</Favorecido>
                //<FavorecidoDesc>PREF. MUNICIPAL DE ASSIS</FavorecidoDesc>
                //<NPD>2018PD00068</NPD>
                //<Emissao>21MAI2018</Emissao>
                //<Vencimento>27MAI2018</Vencimento>
                //<Valor>2,12</Valor>
                //</repete>
                //</documento>
                //</ListaPd>
                //</SIAFDOC>;"

                return RespostaSiafemToObject<ConsultaPd>(result, "repete");
            }
            catch (Exception e)
            {

                if (HttpContext.Current == null) throw new SidsException(e.Message);

                HttpContext.Current.Session["xml"] = HttpContext.Current.Session["xmlSiafem"];
                HttpContext.Current.Session["xmlSiafem"] = "";
                throw new SidsException(e.Message);
            }
        }

        public ConsultaOB ConsultaOB(string login, string password, string unidadeGestora, string Gestao, string NumeroOB)
        {
            try
            {
                var siafdoc = new SIAFDOC
                {
                    cdMsg = "SIAFConsultaOB",
                    SiafemDocConsultaOB = new SiafemDocConsultaOB()
                    {
                        documento = new documento
                        {
                            UnidadeGestora = unidadeGestora,
                            Gestao = Gestao,
                            NumeroOB = NumeroOB
                        }
                    }
                };

                var result = _contaUnica.ConsultaOB(login, password, unidadeGestora, siafdoc);
                //var result = @"<MSG>
                //     <BCMSG>
                //      <Doc_Estimulo>
                //       <SIAFDOC>
                //        <cdMsg>SIAFConsultaOB</cdMsg>
                //        <SiafemDocConsultaOB>
                //         <documento>
                //          <UnidadeGestora>162184</UnidadeGestora>
                //          <Gestao>16055</Gestao>
                //          <NumeroOB>2018OB00001</NumeroOB>
                //         </documento>
                //        </SiafemDocConsultaOB>
                //       </SIAFDOC>
                //      </Doc_Estimulo>
                //     </BCMSG>
                //     <SISERRO>
                //      <Doc_Retorno>
                //       <SIAFDOC>
                //        <cdMsg>SIAFConsultaOB</cdMsg>
                //        <SiafemDocConsultaOB>
                //         <StatusOperacao>true</StatusOperacao>
                //         <MsgRetorno></MsgRetorno>
                //         <documento>
                //          <UG>162184</UG>
                //          <DescUnidadeGestora>DEPTO.ESTR.E RODAGEM-SEDE - UGFRP</DescUnidadeGestora>
                //          <Gestao>16055   - DER</Gestao>
                //          <NumeroOB>2018OB" + NumeroOB + @"</NumeroOB>
                //          <PDNLOCLista>162101 16055 2017PD00002 2017NL00190</PDNLOCLista>
                //          <Banco>001</Banco>
                //          <BancoFav>001</BancoFav>
                //          <Agencia>01897</Agencia>
                //          <AgenciaFav>06501</AgenciaFav>
                //          <GestaoFav></GestaoFav>
                //          <ContaCorrente>013000012</ContaCorrente>
                //          <ContaCorrenteFav>000032247</ContaCorrenteFav>
                //          <CgcCpfUG>00000028000129  - TARGET ENGENHARIA E CONSULTORIA S/C LTDA</CgcCpfUG>
                //          <outRecDesp1>33903930</outRecDesp1>
                //          <outRecDesp2></outRecDesp2>
                //          <outRecDesp3></outRecDesp3>
                //          <Classificacao1></Classificacao1>
                //          <Classificacao2></Classificacao2>
                //          <Classificacao3></Classificacao3>
                //          <Evento1>700601</Evento1>
                //          <Evento2>701977</Evento2>
                //          <Evento3></Evento3>
                //          <Finalidade>PD TESTE FONTE 04 CED 3</Finalidade>
                //          <Fonte1>004001001</Fonte1>
                //          <Fonte2>004001001</Fonte2>
                //          <Fonte3></Fonte3>
                //          <InscricaoEvento1>2017NE00376</InscricaoEvento1>
                //          <InscricaoEvento2></InscricaoEvento2>
                //          <InscricaoEvento3></InscricaoEvento3>
                //          <Processo>PROCESSO1</Processo>
                //          <Situacao1>A RELACIONAR</Situacao1>
                //          <Situacao2>** PAGAMENTO COM PRIORIDADE **</Situacao2>
                //          <Valor>0,01</Valor>
                //          <Valor1>0,01</Valor1>
                //          <Valor2>0,01</Valor2>
                //          <Valor3></Valor3>
                //          <Consulta>22/11/2017</Consulta>
                //          <Hora>15:36</Hora>
                //          <HoraLancado>13:22</HoraLancado>
                //          <DataEmissao>28AGO2017</DataEmissao>
                //          <DataLancamento>28AGO2017</DataLancamento>
                //          <Lancadopor>USUARIO PUBLICO DO SIAFEM</Lancadopor>
                //          <TipoOB>012</TipoOB>
                //                            <Obra>
                //                             <Tipo>1</Tipo>
                //                             <UG>171312</UG>
                //                             <Ano>2015</Ano>
                //                             <Mes>02</Mes>
                //                             <Numero>100000806</Numero>
                //                             <AnoMedicao>2015</AnoMedicao>
                //                             <MesMedicao>5</MesMedicao>
                //                            </Obra>
                //                            <ItensLiquidados>
                //                                <Item>
                //                                    <Sequencia>002</Sequencia>
                //                                    <SubSequencia>00</SubSequencia>
                //                                    <CodItem>1022610</CodItem>
                //                                    <Quantidade>2,000</Quantidade>
                //                                    <Valor>49,00</Valor>
                //                                </Item>
                //                            </ItensLiquidados>
                //         </documento>
                //        </SiafemDocConsultaOB>
                //       </SIAFDOC>
                //      </Doc_Retorno>
                //     </SISERRO>
                //    </MSG>";

                return RespostaSiafemToObject<ConsultaOB>(result);
            }
            catch (Exception e)
            {

                if (HttpContext.Current == null) throw new SidsException(e.Message);

                HttpContext.Current.Session["xml"] = HttpContext.Current.Session["xmlSiafem"];
                HttpContext.Current.Session["xmlSiafem"] = "";
                throw new SidsException(e.Message);
            }
        }

        public CancelarOB CancelarOB(string login, string password, string unidadeGestora, string Gestao, string OB, string Causa1, string Causa2)
        {
            try
            {
                var siafdoc = new SIAFDOC
                {
                    cdMsg = "SIAFCanOBCTU",
                    SiafemDocCanOBCTU = new SIAFCanOBCTU()
                    {
                        documento = new documento
                        {
                            UnidadeGestora = unidadeGestora,
                            Gestao = Gestao,
                            OB = OB,
                            Causa1 = Causa1,
                            Causa2 = Causa2
                        }
                    }
                };

                var result = _contaUnica.ConsultaOB(login, password, unidadeGestora, siafdoc);
                var obj = RespostaSiafemToObject<CancelarOB>(result);

                if (!string.IsNullOrWhiteSpace(obj.MsgErro))
                {
                    throw new SidsException(obj.MsgErro);
                }

                return obj;

            }
            catch (Exception e)
            {
                if (HttpContext.Current == null) throw new SidsException(e.Message);

                HttpContext.Current.Session["xml"] = HttpContext.Current.Session["xmlSiafem"];
                HttpContext.Current.Session["xmlSiafem"] = "";
                throw new SidsException(e.Message);
            }
        }

        public ConsultaPd ExecutarPD(string login, string password, string unidadeGestora, string UGPagadora, string GestaoPagadora, string UGLiquidante, string GestaoLiquidante, string anoAserpaga, string NouP, string numPD)
        {
            try
            {
                var siafdoc = new SIAFDOC
                {
                    cdMsg = "SIAFExepd2",
                    SiafemDocExepd2 = new SiafemDocExecutarPD()
                    {
                        documento = new documento
                        {
                            UGPagadora = UGPagadora,
                            GestaoPagadora = GestaoPagadora,
                            UGLiquidante = UGLiquidante,
                            GestaoLiquidante = GestaoLiquidante,
                            AnoAserpaga = anoAserpaga.ToString(),
                            NouP = NouP,
                            NumPD = numPD
                        }
                    }
                };

                if (AppConfig.WsUrl == "siafemProd")
                {
                    unidadeGestora = "#" + unidadeGestora;
                }

                var xmlstring = _contaUnica.ExecutaPD(login, password, unidadeGestora, siafdoc);
                var documentoxml = xmlstring.ToXml("SIAFEM");
                //var result = @"<?xml version='1.0' encoding='UTF-8'?>
                //<MSG>
                //    <BCMSG>
                //        <Doc_Estimulo>
                //        <SIAFDOC>
                //            <cdMsg>SIAFExepd2</cdMsg>
                //            <SiafemDocExepd2>
                //            <documento>
                //            <UGPagadora>162184</UGPagadora>
                //            <GestaoPagadora>16055</GestaoPagadora>
                //            <UGLiquidante>162101</UGLiquidante>
                //            <GestaoLiquidante>16055</GestaoLiquidante>
                //            <AnoAserpaga>2017</AnoAserpaga>
                //            <NouP>N</NouP>
                //            <NumPD>00026</NumPD>
                //            </documento>
                //            </SiafemDocExepd2>
                //        </SIAFDOC>
                //        </Doc_Estimulo>
                //    </BCMSG>
                //    <SISERRO>
                //    <Doc_Retorno>
                //    <SIAFDOC>
                //        <cdMsg>SIAFExepd2</cdMsg>
                //        <SiafemDocExepd2>
                //        <documento>
                //            <StatusOperacao>true</StatusOperacao>
                //            <MsgErro>PROGRAMACAO DE DESEMBOLSO PAGA : 162101 16055 2017PD00026</MsgErro>
                //            <UnidadeGestora>162184</UnidadeGestora>
                //            <Gestao>16055</Gestao>
                //            <NumOB>2017OB00015</NumOB>
                //        </documento>
                //        </SiafemDocExepd2>
                //        </SIAFDOC>
                //        </Doc_Retorno>
                //    </SISERRO>
                //</MSG>";

                var consultapd = documentoxml.GetElementsByTagName("documento").ConvertNodeTo<ConsultaPd>();
                var msg = documentoxml.GetElementsByTagName("MsgErro").Item(0)?.InnerText;
                var statusOperacao = bool.Parse(documentoxml.GetElementsByTagName("StatusOperacao").Item(0)?.InnerText);

                if (!statusOperacao)
                {
                    throw new SidsException(msg);
                }

                return consultapd;
            }
            catch (Exception e)
            {

                if (HttpContext.Current == null) throw new SidsException(e.Message);

                HttpContext.Current.Session["xml"] = HttpContext.Current.Session["xmlSiafem"];
                HttpContext.Current.Session["xmlSiafem"] = "";
                throw new SidsException(e.Message);
            }
        }

        public ConsultaOB AutorizarOB(string login, string password, string unidadeGestora, string gestao, string numOB, string valor)
        {
            try
            {
                var siafdoc = new SIAFDOC
                {
                    cdMsg = "SIAFAutobSef",
                    SiafemDocAutorizaOB = new SiafemDocAutobSef
                    {
                        Documento = new SiafemDocAutobSefDocumento
                        {
                            UnidadeGestora = unidadeGestora,
                            Gestao = gestao,
                            Ob = numOB,
                            Valor = valor
                        }
                    }
                };

                if (AppConfig.WsUrl == "siafemProd")
                {
                    unidadeGestora = "#" + unidadeGestora;
                }

                var xmlstring = _contaUnica.AutorizaOB(login, password, unidadeGestora, siafdoc);
                var documentoxml = xmlstring.ToXml("SIAFEM");

                var consultaOB = documentoxml.GetElementsByTagName("documento").ConvertNodeTo<ConsultaOB>();
                var msg = documentoxml.GetElementsByTagName("MsgErro").Item(0)?.InnerText;
                var statusOperacao = bool.Parse(documentoxml.GetElementsByTagName("StatusOperacao").Item(0)?.InnerText);

                if (!statusOperacao)
                {
                    throw new SidsException(msg);
                }

                return consultaOB;
            }
            catch (Exception e)
            {

                if (HttpContext.Current == null) throw new SidsException(e.Message);

                HttpContext.Current.Session["xml"] = HttpContext.Current.Session["xmlSiafem"];
                HttpContext.Current.Session["xmlSiafem"] = "";
                throw new SidsException(e.Message);
            }
        }

        public CancelarImpressaoRelacaoReRt CancelarImpressaoRelacaoReRt(string login, string password, string unidadeGestora, string gestao, string prefixoREouRT, string numREouRT)
        {
            try
            {
                var siafdoc = new SIAFDOC
                {
                    cdMsg = "SIAFCanRel001",
                    SiafemDocCanRel = new SiafemDocCanRel()
                    {
                        Documento = new DocumentoImpressaoRelacaoReRt()
                        {
                            UnidadeGestora = unidadeGestora,
                            Gestao = gestao,
                            PrefixoREouRT = prefixoREouRT,
                            NumREouRT = numREouRT
                        }
                    }
                };

                var result = _contaUnica.CancelarImpressaoRelacaoReRt(login, password, unidadeGestora, siafdoc);

                var msgErros = result.ToXml("SIAFDOC");
                var statusOperacao = msgErros.GetElementsByTagName("StatusOperacao");
                var messagem = msgErros.GetElementsByTagName("MsgErro");

                if (messagem.Count <= 0)
                    messagem = msgErros.GetElementsByTagName("MsgRetorno");

                string root = "";
                if (statusOperacao.Count > 0)
                    root = statusOperacao[0].FirstChild.Value;
                else if (messagem.Count > 0)
                    root = false.ToString();

                if (!bool.Parse(root))
                {
                    throw new SiafemException(messagem[0].FirstChild.Value);
                }

                var document = msgErros.GetElementsByTagName("documento");

                CancelarImpressaoRelacaoReRt resultingMessage = ConvertNode<CancelarImpressaoRelacaoReRt>(document);

                return resultingMessage;
            }
            catch (Exception e)
            {
                if (HttpContext.Current == null) throw new SidsException(e.Message);

                HttpContext.Current.Session["xml"] = HttpContext.Current.Session["xmlSiafem"];
                HttpContext.Current.Session["xmlSiafem"] = "";
                throw new SidsException(e.Message);
            }
        }

        public RespostaImpressaoRelacaoReRt TransmitirImpressaoRelacaoRE(string login, string password, string unidadeGestora, string gestao, string banco, string dataSolicitacao, string numeroRelatorio)
        {
            try
            {
                var siafdoc = new SIAFDOC
                {
                    cdMsg = "SIAFRE",
                    SiafemDocRE = new SiafemDocRE()
                    {
                        Documento = new DocumentoImpressaoRelacaoReRt()
                        {
                            UnidadeGestora = unidadeGestora,
                            Gestao = gestao,
                            Banco = banco,
                            DataSolicitacao = dataSolicitacao,
                            NumeroRelatorio = numeroRelatorio
                        }
                    }
                };

                var result = _contaUnica.TransmitirImpressaoRelacaoReRt(login, password, unidadeGestora, siafdoc);

                #region result RE com OB
                //var result = @"<?xml version='1.0' encoding='UTF-8'?>
                //<MSG>
                //    <BCMSG>
                //    <Doc_Estimulo>
                //        <SIAFDOC>
                //        <cdMsg>SIAFRE</cdMsg>
                //        <SiafemDocRE>
                //            <documento>
                //            <UnidadeGestora>200002</UnidadeGestora>
                //            <Gestao>00001</Gestao>
                //            <Banco/>
                //            <DataSolicitacao/>
                //            <NumeroRelatorio/>
                //            </documento>
                //        </SiafemDocRE>
                //        </SIAFDOC>
                //    </Doc_Estimulo>
                //    </BCMSG>
                //    <SISERRO>
                //    <Doc_Retorno>
                //        <SIAFDOC>
                //        <cdMsg>SIAFRE</cdMsg>
                //        <SiafemDocRE>
                //            <StatusOperacao>true</StatusOperacao>
                //            <MsgRetorno/>
                //            <documento>
                //            <DATA_REFERENCIA>19032018</DATA_REFERENCIA>
                //            <CODIGO_RELATORIO>L.33172.CJ</CODIGO_RELATORIO>
                //            <RELOB>2018RE00001</RELOB>
                //            <UNIDADE_GESTORA>200002</UNIDADE_GESTORA>
                //            <NOME_DA_UNIDADE_GESTORA>TESOURO DO ESTADO</NOME_DA_UNIDADE_GESTORA>
                //            <GESTAO>00001</GESTAO>
                //            <NOME_DA_GESTAO>GOVERNO DO ESTADO DE SAO PAULO</NOME_DA_GESTAO>
                //            <BANCO>001</BANCO>
                //            <NOME_DO_BANCO>BANCO DO BRASIL S.A.</NOME_DO_BANCO>
                //            <AGENCIA>01897</AGENCIA>
                //            <NOME_DA_AGENCIA>S.PUBLICO SAO PAULO</NOME_DA_AGENCIA>
                //            <CONTA_C>013000020</CONTA_C>
                //            <VALOR_TOTAL_DOCUMENTO>00000000000010000</VALOR_TOTAL_DOCUMENTO>
                //            <VALOR_POR_EXTENSO>CEM REAIS****************************************************************************************************************************************************************************************************************************************************</VALOR_POR_EXTENSO>
                //            <TEXTO_AUTORIZACAO>AUTORIZO A BANCO DO BRASIL A EFETIVAR OS PAGAMENTOS ACIMA RELACIONADOS, EXCETUANDO AQUELAS OBS CANCELADAS PELAS GRS ANEXAS.</TEXTO_AUTORIZACAO>
                //            <DATA_EMISSAO>17092018</DATA_EMISSAO>
                //            <CIDADE>SAO PAULO-SP</CIDADE>
                //            <NOME_ORDENADOR_ASSINATURA>FRANCISCO CARLOS C SALES</NOME_ORDENADOR_ASSINATURA>
                //            <NOME_GESTOR_FINANCEIRO>NANCI CORTAZZO M. GALUZIO</NOME_GESTOR_FINANCEIRO>
                //            <REPETICOES>
                //                <OB>
                //                <NUMERO>2018OB00002</NUMERO>
                //                <IND_PRIORIDADE>P</IND_PRIORIDADE>
                //                <TIPO_OB>12</TIPO_OB>
                //                <NOME_DO_FAVORECIDO>SECRETARIA DE ESTADO DOS NEG. DA FAZENDA</NOME_DO_FAVORECIDO>
                //                <BANCO_FAVORECIDO>001</BANCO_FAVORECIDO>
                //                <AGENCIA_FAVORECIDO>01897</AGENCIA_FAVORECIDO>
                //                <CONTA_FAVORECIDO>000080225</CONTA_FAVORECIDO>
                //                <VALOR_OB>00000000000010000</VALOR_OB>
                //                </OB>
                //                <OB>
                //                <NUMERO>2018OB00003</NUMERO>
                //                <IND_PRIORIDADE>P</IND_PRIORIDADE>
                //                <TIPO_OB>12</TIPO_OB>
                //                <NOME_DO_FAVORECIDO>TARGET ENGENHARIA E CONSULTORIA S/C LTDA</NOME_DO_FAVORECIDO>
                //                <BANCO_FAVORECIDO>001</BANCO_FAVORECIDO>
                //                <AGENCIA_FAVORECIDO>06501</AGENCIA_FAVORECIDO>
                //                <CONTA_FAVORECIDO>000032247</CONTA_FAVORECIDO>
                //                <VALOR_OB>00000000000003808</VALOR_OB>
                //                </OB>
                //                <OB>
                //                <NUMERO>2018OB00004</NUMERO>
                //                <IND_PRIORIDADE>P</IND_PRIORIDADE>
                //                <TIPO_OB>12</TIPO_OB>
                //                <NOME_DO_FAVORECIDO>PREFEITURA MUNICIPAL DE SAO PAULO</NOME_DO_FAVORECIDO>
                //                <BANCO_FAVORECIDO>001</BANCO_FAVORECIDO>
                //                <AGENCIA_FAVORECIDO>03370</AGENCIA_FAVORECIDO>
                //                <CONTA_FAVORECIDO>000130931</CONTA_FAVORECIDO>
                //                <VALOR_OB>00000000000000112</VALOR_OB>
                //                </OB>
                //                <OB>
                //                <NUMERO>2018OB00005</NUMERO>
                //                <IND_PRIORIDADE>P</IND_PRIORIDADE>
                //                <TIPO_OB>12</TIPO_OB>
                //                <NOME_DO_FAVORECIDO>PREFEITURA MUNICIPAL DE SAO PAULO</NOME_DO_FAVORECIDO>
                //                <BANCO_FAVORECIDO>001</BANCO_FAVORECIDO>
                //                <AGENCIA_FAVORECIDO>03370</AGENCIA_FAVORECIDO>
                //                <CONTA_FAVORECIDO>000130931</CONTA_FAVORECIDO>
                //                <VALOR_OB>00000000000081112</VALOR_OB>
                //                </OB>
                //                <OB>
                //                <NUMERO>2018OB00006</NUMERO>
                //                <IND_PRIORIDADE>P</IND_PRIORIDADE>
                //                <TIPO_OB>12</TIPO_OB>
                //                <NOME_DO_FAVORECIDO>PREFEITURA MUNICIPAL DE SAO PAULO</NOME_DO_FAVORECIDO>
                //                <BANCO_FAVORECIDO>001</BANCO_FAVORECIDO>
                //                <AGENCIA_FAVORECIDO>03370</AGENCIA_FAVORECIDO>
                //                <CONTA_FAVORECIDO>000130931</CONTA_FAVORECIDO>
                //                <VALOR_OB>00000000000000200</VALOR_OB>
                //                </OB>
                //                <OB>
                //                <NUMERO>2018OB00007</NUMERO>
                //                <IND_PRIORIDADE>P</IND_PRIORIDADE>
                //                <TIPO_OB>12</TIPO_OB>
                //                <NOME_DO_FAVORECIDO>PREFEITURA MUNICIPAL DE SAO PAULO</NOME_DO_FAVORECIDO>
                //                <BANCO_FAVORECIDO>001</BANCO_FAVORECIDO>
                //                <AGENCIA_FAVORECIDO>03370</AGENCIA_FAVORECIDO>
                //                <CONTA_FAVORECIDO>000130931</CONTA_FAVORECIDO>
                //                <VALOR_OB>00000000000197111</VALOR_OB>
                //                </OB>
                //            </REPETICOES>
                //            </documento>
                //        </SiafemDocRE>
                //        </SIAFDOC>
                //    </Doc_Retorno>
                //    </SISERRO>
                //</MSG>";
                #endregion

                #region result sem RE com RT
                //var result = @"<?xml version='1.0' encoding='UTF-8'?>
                //<MSG>
                //    <BCMSG>
                //        <Doc_Estimulo>
                //            <SIAFDOC>
                //                <cdMsg>SIAFRE</cdMsg>
                //                <SiafemDocRE>
                //                    <documento>
                //                        <UnidadeGestora>162184</UnidadeGestora>
                //                        <Gestao>16055</Gestao>
                //                        <Banco/>
                //                        <DataSolicitacao/>
                //                        <NumeroRelatorio/>
                //                    </documento>
                //                </SiafemDocRE>
                //            </SIAFDOC>
                //        </Doc_Estimulo>
                //    </BCMSG>
                //    <SISERRO>
                //        <Doc_Retorno>
                //            <SIAFDOC>
                //            <cdMsg>SIAFRE</cdMsg>
                //                <SiafemDocRE>
                //                    <StatusOperacao>false</StatusOperacao>
                //                    <MsgRetorno>HA SOMENTE RT</MsgRetorno>
                //                </SiafemDocRE>
                //            </SIAFDOC>  
                //        </Doc_Retorno>
                //    </SISERRO>
                //</MSG>";
                #endregion

                #region result sem RE e RT
                //var result = @"<?xml version='1.0' encoding='UTF-8'?>
                //<MSG>
                //    <BCMSG>
                //        <Doc_Estimulo>
                //            <SIAFDOC>
                //                <cdMsg>SIAFRE</cdMsg>
                //                <SiafemDocRE>
                //                    <documento>
                //                        <UnidadeGestora>162184</UnidadeGestora>
                //                        <Gestao>16055</Gestao>
                //                        <Banco/>
                //                        <DataSolicitacao/>
                //                        <NumeroRelatorio/>
                //                    </documento>
                //                </SiafemDocRE>
                //            </SIAFDOC>
                //        </Doc_Estimulo>
                //    </BCMSG>
                //    <SISERRO>
                //        <Doc_Retorno>
                //            <SIAFDOC>
                //            <cdMsg>SIAFRE</cdMsg>
                //                <SiafemDocRE>
                //                    <StatusOperacao>false</StatusOperacao>
                //                    <MsgRetorno>(0203) NAO EXISTE DOCUMENTO PENDENTE PARA EMISSAO - TECLE 'ENTER'</MsgRetorno>
                //                </SiafemDocRE>
                //            </SIAFDOC>  
                //        </Doc_Retorno>
                //    </SISERRO>
                //</MSG>";
                #endregion

                var obj = new RespostaImpressaoRelacaoReRt();
                var documentoxml = result.ToXml("SIAFEM");
                obj.MsgErro = documentoxml.GetElementsByTagName("MsgErro").Item(0)?.InnerText;
                if (string.IsNullOrEmpty(obj.MsgErro))
                {
                    obj.MsgRetorno = documentoxml.GetElementsByTagName("MsgRetorno").Item(0)?.InnerText;
                }

                if (string.IsNullOrWhiteSpace(obj.MsgRetorno) || !string.IsNullOrWhiteSpace(obj.MsgErro))
                {

                    obj = RespostaSiafemToObject<RespostaImpressaoRelacaoReRt>(result);
                    obj.StatusOperacao = Convert.ToBoolean(documentoxml.GetElementsByTagName("StatusOperacao").Item(0)?.InnerText);
                }
                else
                {
                    if (obj.MsgRetorno.Substring(0, 6) == "(0203)")
                    {
                        obj.SemReRt = "Sem Registros RE e RT";
                    }
                    else if (obj.MsgRetorno == "HA SOMENTE RT")
                    {
                        obj = TransmitirImpressaoRelacaoRT(login, password, unidadeGestora, gestao, banco, dataSolicitacao, numeroRelatorio);
                    }
                }

                return obj;
            }
            catch (Exception e)
            {
                if (HttpContext.Current == null) throw new SidsException(e.Message);

                HttpContext.Current.Session["xml"] = HttpContext.Current.Session["xmlSiafem"];
                HttpContext.Current.Session["xmlSiafem"] = "";
                throw new SidsException(e.Message);
            }
        }

        public RespostaImpressaoRelacaoReRt TransmitirImpressaoRelacaoRT(string login, string password, string unidadeGestora, string gestao, string banco, string dataSolicitacao, string numeroRelatorio)
        {
            try
            {
                var siafdoc = new SIAFDOC
                {
                    cdMsg = "SIAFRT",
                    SiafemDocRT = new SiafemDocRT()
                    {
                        Documento = new DocumentoImpressaoRelacaoReRt()
                        {
                            UnidadeGestora = unidadeGestora,
                            Gestao = gestao,
                            Banco = banco,
                            DataSolicitacao = dataSolicitacao,
                            NumeroRelatorio = numeroRelatorio
                        }
                    }
                };

                var result = _contaUnica.TransmitirImpressaoRelacaoReRt(login, password, unidadeGestora, siafdoc);

                #region result RT com OB
                //var result = @"<?xml version='1.0' encoding='UTF-8'?>
                //<MSG>
                //    <BCMSG>
                //        <Doc_Estimulo>
                //            <SIAFDOC>
                //                <cdMsg>SIAFRT</cdMsg>
                //                <SiafemDocRT>
                //                    <documento>
                //                        <UnidadeGestora>162184</UnidadeGestora>
                //                        <Gestao>16055</Gestao>
                //                        <Banco/>
                //                        <DataSolicitacao/>
                //                        <NumeroRelatorio/>
                //                    </documento>
                //                </SiafemDocRT>
                //            </SIAFDOC>
                //        </Doc_Estimulo>
                //    </BCMSG>
                //    <SISERRO>
                //        <Doc_Retorno>
                //            <SIAFDOC>
                //                <cdMsg>SIAFRT</cdMsg>
                //                <SiafemDocRT>
                //                    <StatusOperacao>true</StatusOperacao>
                //                    <MsgRetorno></MsgRetorno>
                //                    <documento>
                //                        <DATA_REFERENCIA>05092018</DATA_REFERENCIA>
                //                        <CODIGO_RELATORIO>L.33172.CK</CODIGO_RELATORIO>
                //                        <RELOB>2018RT00012</RELOB>
                //                        <UNIDADE_GESTORA>162184</UNIDADE_GESTORA>
                //                        <NOME_UNIDADE_GESTORA>DEPTO.ESTR.E RODAGEM-SEDE - UGFRP</NOME_UNIDADE_GESTORA>
                //                        <GESTAO>16055</GESTAO>
                //                        <NOME_GESTAO>DEPTO. DE ESTRADAS E RODAGEM</NOME_GESTAO>
                //                        <BANCO>001</BANCO>
                //                        <NOME_BANCO>BANCO DO BRASIL S.A.</NOME_BANCO>
                //                        <VALOR_TOTAL_DOCUMENTO>00000000000008000</VALOR_TOTAL_DOCUMENTO>
                //                        <VALOR_POR_EXTENSO>OITENTA REAIS************************************************************************************************************************************************************************************************************************************************</VALOR_POR_EXTENSO>
                //                        <TEXTO_AUTORIZACAO>HOMOLOGO AS TRANSFERENCIAS E/OU PAGAMENTOS INTRA-SIAFEM ACIMA RELACIONADOS.</TEXTO_AUTORIZACAO>
                //                        <DATA_EMISSAO>05092018</DATA_EMISSAO>
                //                        <CIDADE>SAO PAULO-SP</CIDADE>
                //                        <NOME_ORDENADOR_ASSINATURA>INEZ BRUSTOLIN</NOME_ORDENADOR_ASSINATURA>
                //                        <NOME_GESTOR_FINANCEIRO>GERSON RAMOS</NOME_GESTOR_FINANCEIRO>
                //                        <REPETICOES>
                //                            <OB>
                //                                <NUMERO>2018OB01357</NUMERO>
                //                                <CONTA_BANCARIA_EMITENTE>013000012</CONTA_BANCARIA_EMITENTE>
                //                                <UNIDADE_GESTORA_FAVORECIDA>200002</UNIDADE_GESTORA_FAVORECIDA>
                //                                <GESTAO_FAVORECIDA>00001</GESTAO_FAVORECIDA>
                //                                <MNEMONICO_UG_FAVORECIDA>TESOURO DO ESTADO</MNEMONICO_UG_FAVORECIDA>
                //                                <BANCO_FAVORECIDO>001</BANCO_FAVORECIDO>
                //                                <AGENCIA_FAVORECIDO>01897</AGENCIA_FAVORECIDO>
                //                                <CONTA_FAVORECIDO>013000012</CONTA_FAVORECIDO>
                //                                <VALOR_OB>00000000000003000</VALOR_OB>
                //                            </OB>
                //                            <OB>
                //                                <NUMERO>2018OB01358</NUMERO>
                //                                <CONTA_BANCARIA_EMITENTE>013000012</CONTA_BANCARIA_EMITENTE>
                //                                <UNIDADE_GESTORA_FAVORECIDA>200002</UNIDADE_GESTORA_FAVORECIDA>
                //                                <GESTAO_FAVORECIDA>00001</GESTAO_FAVORECIDA>
                //                                <MNEMONICO_UG_FAVORECIDA>TESOURO DO ESTADO</MNEMONICO_UG_FAVORECIDA>
                //                                <BANCO_FAVORECIDO>001</BANCO_FAVORECIDO>
                //                                <AGENCIA_FAVORECIDO>01897</AGENCIA_FAVORECIDO>
                //                                <CONTA_FAVORECIDO>013000012</CONTA_FAVORECIDO>
                //                                <VALOR_OB>00000000000001000</VALOR_OB>
                //                            </OB>
                //                            <OB>
                //                                <NUMERO>2018OB01359</NUMERO>
                //                                <CONTA_BANCARIA_EMITENTE>013000012</CONTA_BANCARIA_EMITENTE>
                //                                <UNIDADE_GESTORA_FAVORECIDA>200002</UNIDADE_GESTORA_FAVORECIDA>
                //                                <GESTAO_FAVORECIDA>00001</GESTAO_FAVORECIDA>
                //                                <MNEMONICO_UG_FAVORECIDA>TESOURO DO ESTADO</MNEMONICO_UG_FAVORECIDA>
                //                                <BANCO_FAVORECIDO>001</BANCO_FAVORECIDO>
                //                                <AGENCIA_FAVORECIDO>01897</AGENCIA_FAVORECIDO>
                //                                <CONTA_FAVORECIDO>013000012</CONTA_FAVORECIDO>
                //                                <VALOR_OB>00000000000001000</VALOR_OB>
                //                            </OB>
                //                            <OB>
                //                                <NUMERO>2018OB01360</NUMERO>
                //                                <CONTA_BANCARIA_EMITENTE>013000012</CONTA_BANCARIA_EMITENTE>
                //                                <UNIDADE_GESTORA_FAVORECIDA>200002</UNIDADE_GESTORA_FAVORECIDA>
                //                                <GESTAO_FAVORECIDA>00001</GESTAO_FAVORECIDA>
                //                                <MNEMONICO_UG_FAVORECIDA>TESOURO DO ESTADO</MNEMONICO_UG_FAVORECIDA>
                //                                <BANCO_FAVORECIDO>001</BANCO_FAVORECIDO>
                //                                <AGENCIA_FAVORECIDO>01897</AGENCIA_FAVORECIDO>
                //                                <CONTA_FAVORECIDO>013000012</CONTA_FAVORECIDO>
                //                                <VALOR_OB>00000000000003000</VALOR_OB>
                //                            </OB>
                //                            <OB>
                //                                <NUMERO>2018OB01361</NUMERO>
                //                                <CONTA_BANCARIA_EMITENTE>013000012</CONTA_BANCARIA_EMITENTE>
                //                                <UNIDADE_GESTORA_FAVORECIDA>200002</UNIDADE_GESTORA_FAVORECIDA>
                //                                <GESTAO_FAVORECIDA>00001</GESTAO_FAVORECIDA>
                //                                <MNEMONICO_UG_FAVORECIDA>TESOURO DO ESTADO</MNEMONICO_UG_FAVORECIDA>
                //                                <BANCO_FAVORECIDO>001</BANCO_FAVORECIDO>
                //                                <AGENCIA_FAVORECIDO>01897</AGENCIA_FAVORECIDO>
                //                                <CONTA_FAVORECIDO>013000012</CONTA_FAVORECIDO>
                //                                <VALOR_OB>00000000000103000</VALOR_OB>
                //                            </OB>
                //                            <OB>
                //                                <NUMERO>2018OB01362</NUMERO>
                //                                <CONTA_BANCARIA_EMITENTE>013000012</CONTA_BANCARIA_EMITENTE>
                //                                <UNIDADE_GESTORA_FAVORECIDA>200002</UNIDADE_GESTORA_FAVORECIDA>
                //                                <GESTAO_FAVORECIDA>00001</GESTAO_FAVORECIDA>
                //                                <MNEMONICO_UG_FAVORECIDA>TESOURO DO ESTADO</MNEMONICO_UG_FAVORECIDA>
                //                                <BANCO_FAVORECIDO>001</BANCO_FAVORECIDO>
                //                                <AGENCIA_FAVORECIDO>01897</AGENCIA_FAVORECIDO>
                //                                <CONTA_FAVORECIDO>013000012</CONTA_FAVORECIDO>
                //                                <VALOR_OB>00000000000004000</VALOR_OB>
                //                            </OB>
                //                            <OB>
                //                                <NUMERO>2018OB01363</NUMERO>
                //                                <CONTA_BANCARIA_EMITENTE>013000012</CONTA_BANCARIA_EMITENTE>
                //                                <UNIDADE_GESTORA_FAVORECIDA>200002</UNIDADE_GESTORA_FAVORECIDA>
                //                                <GESTAO_FAVORECIDA>00001</GESTAO_FAVORECIDA>
                //                                <MNEMONICO_UG_FAVORECIDA>TESOURO DO ESTADO</MNEMONICO_UG_FAVORECIDA>
                //                                <BANCO_FAVORECIDO>001</BANCO_FAVORECIDO>
                //                                <AGENCIA_FAVORECIDO>01897</AGENCIA_FAVORECIDO>
                //                                <CONTA_FAVORECIDO>013000012</CONTA_FAVORECIDO>
                //                                <VALOR_OB>00000000000013000</VALOR_OB>
                //                            </OB>
                //                        </REPETICOES>
                //                    </documento>
                //                </SiafemDocRT>
                //            </SIAFDOC>
                //        </Doc_Retorno>
                //    </SISERRO>
                //</MSG>";
                #endregion

                #region result acabou OB das RT
                //var result = @"<?xml version='1.0' encoding='UTF-8'?>
                //<MSG>
                //    <BCMSG>
                //        <Doc_Estimulo>
                //            <SIAFDOC>
                //                <cdMsg>SIAFRT</cdMsg>
                //                <SiafemDocRT>
                //                    <documento>
                //                        <UnidadeGestora>162184</UnidadeGestora>
                //                        <Gestao>16055</Gestao>
                //                        <Banco/>
                //                        <DataSolicitacao/>
                //                        <NumeroRelatorio/>
                //                    </documento>
                //                </SiafemDocRT>
                //            </SIAFDOC>
                //        </Doc_Estimulo>
                //    </BCMSG>
                //    <SISERRO>
                //        <Doc_Retorno>
                //            <SIAFDOC>
                //            <cdMsg>SIAFRT</cdMsg>
                //                <SiafemDocRT>
                //                    <StatusOperacao>false</StatusOperacao>
                //                    <MsgRetorno>(0203) NAO EXISTE DOCUMENTO PENDENTE PARA EMISSAO - TECLE 'ENTER'</MsgRetorno>
                //                </SiafemDocRT>
                //            </SIAFDOC>  
                //        </Doc_Retorno>
                //    </SISERRO>
                //</MSG>";
                #endregion

                var obj = new RespostaImpressaoRelacaoReRt();
                var documentoxml = result.ToXml("SIAFEM");
                obj.MsgErro = documentoxml.GetElementsByTagName("MsgErro").Item(0)?.InnerText;
                if (string.IsNullOrEmpty(obj.MsgErro))
                {
                    obj.MsgRetorno = documentoxml.GetElementsByTagName("MsgRetorno").Item(0)?.InnerText;
                }

                if (string.IsNullOrWhiteSpace(obj.MsgRetorno) || !string.IsNullOrWhiteSpace(obj.MsgErro))
                {
                    obj = RespostaSiafemToObject<RespostaImpressaoRelacaoReRt>(result);
                    obj.StatusOperacao = Convert.ToBoolean(documentoxml.GetElementsByTagName("StatusOperacao").Item(0)?.InnerText);
                }

                return obj;
            }
            catch (Exception e)
            {
                if (HttpContext.Current == null) throw new SidsException(e.Message);

                HttpContext.Current.Session["xml"] = HttpContext.Current.Session["xmlSiafem"];
                HttpContext.Current.Session["xmlSiafem"] = "";
                throw new SidsException(e.Message);
            }
        }

        public T RespostaSiafemToObject<T>(string resposta) where T : class
        {

            var document = resposta.ToXml("SIAFEM");

            var status = Convert.ToBoolean(document.GetElementsByTagName("StatusOperacao").Item(0)?.InnerText);
            var message = document.GetElementsByTagName("MsgErro").Item(0)?.InnerText;
            if (string.IsNullOrEmpty(message))
                message = document.GetElementsByTagName("MsgRetorno").Item(0)?.InnerText;

            if (!status || !string.IsNullOrWhiteSpace(message))
                throw new SidsException("SIAFEM - " + message);

            return document.GetElementsByTagName("documento").ConvertNodeTo<T>();
        }

        public List<T> RespostaSiafemToObject<T>(string resposta, string node) where T : class
        {
            resposta = resposta.Replace("</MsgRetorno>\n\t\t\t<tabela>\n\t\t</documento>", "</MsgRetorno>\n\t\t\t<tabela></tabela>\n\t\t</documento>"); // HACK o SIAFEM está retornando XML inválido com a tag "tabela" sem fechamento. Gambiarra necessária.

            var document = resposta.ToXml("SIAFEM");

            var status = Convert.ToBoolean(document.GetElementsByTagName("StatusOperacao").Item(0)?.InnerText);
            var message = document.GetElementsByTagName("MsgErro").Item(0)?.InnerText;
            if (string.IsNullOrEmpty(message))
                message = document.GetElementsByTagName("MsgRetorno").Item(0)?.InnerText;

            if (!status || !string.IsNullOrWhiteSpace(message))
                throw new SidsException("SIAFEM - " + message);

            var retorno = new List<T>();

            foreach (XmlElement item in document.GetElementsByTagName(node))
            {
                var text = String.Format("<{0}><StatusOperacao>true</StatusOperacao><{1}></{1}><documento>{2}</documento> </{0}>", "SIAFEM", "MsgRetorno", item.InnerXml);
                var t = RespostaSiafemToObject<T>(text);
                retorno.Add(t);
            }

            return retorno;
        }

        private static string ReturnMessageWithStatusListaBoletosForSiafemService(XmlDocument document)
        {
            var status = Convert.ToBoolean(document.GetElementsByTagName("StatusOperacao").Item(0)?.InnerText);

            Convert.ToBoolean(document.GetElementsByTagName("StatusOperacao").Item(0)?.InnerText);
            var message = Convert.ToString(document.GetElementsByTagName("Doc_Retorno").Item(0)?.InnerText);


            message = message ?? document.GetElementsByTagName("MsgRetorno").Item(0)?.InnerText;

            var numero =
                $"{document.GetElementsByTagName("NumLista").Item(0)?.InnerText};" +
                $"{document.GetElementsByTagName("TotalLista").Item(0)?.InnerText};" +
                $"{document.GetElementsByTagName("TotalCredorPosterior").Item(0)?.InnerText}";

            numero = numero == ";;" ? document.GetElementsByTagName("ValorDescrito").Item(0)?.InnerText : numero;

            if ((!status || !string.IsNullOrWhiteSpace(message)) && string.IsNullOrWhiteSpace(numero))
                throw new SidsException($"SIAFEM - {message}");

            return numero;
        }

        private SIAFDOC SiafemAltBoletosDocumentProvider(Documento document)
        {
            switch ((EnumTipoBoleto)document.ListaCodigoBarras.TipoBoletoId)
            {
                case EnumTipoBoleto.Taxa:
                    return CreateSiafemAltDocTaxa(document);
                case EnumTipoBoleto.Boleto:
                    return CreateSiafemAltDocBoleto(document);

                default:
                    return new SIAFDOC();
            }
        }

        private static SIAFDOC SiafemBoletosDocumentProvider(Documento document)
        {

            switch ((EnumTipoBoleto)document.ListaCodigoBarras.TipoBoletoId)
            {
                case EnumTipoBoleto.Taxa:
                    return CreateSiafemDocTaxa(document);
                case EnumTipoBoleto.Boleto:
                    return CreateSiafemDocBoleto(document);

                default:
                    return new SIAFDOC();
            }
        }

        private static SIAFDOC SiafemDocumentProvider(ReclassificacaoRetencao entity)
        {
            switch (entity.ReclassificacaoRetencaoTipoId)
            {
                case 2: //Nota de Lançamento – NL
                    return CreateSiafemDocNl(entity);
                case 3: //Pagamento de Obras Sem OB
                    return CreateSiafemDocNlpObras(entity);
                case 5: //Retenção de INSS
                    return CreateSiafemDocSiafnlRetInss(entity);
                case 4: //Retenção de ISS Prefeituras
                    return CreateSiafemDocSiafnlIssReten(entity);
                default:
                    return new SIAFDOC();
            }
        }

        private string ReturnMessageWithStatusForSiafemService(XmlDocument document)
        {
            var status = Convert.ToBoolean(document.GetElementsByTagName("StatusOperacao").Item(0)?.InnerText);
            var message = document.GetElementsByTagName("MsgErro").Item(0)?.InnerText;

            if (document.GetElementsByTagName("MsgErro").Count > 1)
            {
                message = document.GetElementsByTagName("MsgErro").Cast<XmlElement>().Aggregate(message, (current, msg) => string.Concat(current, " \n ", msg.InnerText));
            }

            message = message ?? document.GetElementsByTagName("MsgRetorno").Item(0)?.InnerText;

            var numero = document.GetElementsByTagName("NumeroNL").Item(0)?.InnerText;
            numero = numero ?? document.GetElementsByTagName("NumeroPD").Item(0)?.InnerText;


            if ((!status || !string.IsNullOrWhiteSpace(message)) && string.IsNullOrWhiteSpace(numero))
                throw new SidsException($"SIAFEM - {message}");

            return numero;
        }

        private string ReturnMessageWithStatusForSiafisicoService(XmlDocument document)
        {
            var status = Convert.ToBoolean(document.GetElementsByTagName("StatusOperacao").Item(0)?.InnerText);
            var message = document.GetElementsByTagName("MsgErro").Item(0)?.InnerText;

            if (document.GetElementsByTagName("MsgErro").Count > 1)
            {
                message = document.GetElementsByTagName("MsgErro").Cast<XmlElement>().Aggregate(message, (current, msg) => string.Concat(current, " \n ", msg.InnerText));
            }

            message = message ?? document.GetElementsByTagName("MsgRetorno").Item(0)?.InnerText;

            var numero = document.GetElementsByTagName("NumeroNL").Item(0)?.InnerText;
            numero = numero ?? document.GetElementsByTagName("NumeroPD").Item(0)?.InnerText;


            if (!status)
                throw new SidsException($"SIAFISICO - {message}");

            return numero;
        }

        #region Siafem

        private static SIAFDOC CreateSiafemDocTaxa(Documento document)
        {
            return new SIAFDOC
            {
                cdMsg = "SIAFListaBoletos",
                SiafemListaBoletos = new SiafemListaBoletos
                {
                    documento = new documento
                    {
                        UnidadeGestora = document.ListaBoletos.CodigoUnidadeGestora,
                        Gestao = document.ListaBoletos.CodigoGestao,
                        Cnpj = document.ListaBoletos.NumeroCnpjcpfFavorecido,
                        Nome = document.ListaBoletos.NomeLista,
                        //NumeroLista = document.ListaBoletos.NumeroSiafem??" ",
                        UTILPUBLICAIMPOSTOSTAXASECONTRIBUICOES = "X",
                        Conta1 = document.ListaCodigoBarras.CodigoBarraTaxa.NumeroConta1,
                        Conta2 = document.ListaCodigoBarras.CodigoBarraTaxa.NumeroConta2,
                        Conta3 = document.ListaCodigoBarras.CodigoBarraTaxa.NumeroConta3,
                        Conta4 = document.ListaCodigoBarras.CodigoBarraTaxa.NumeroConta4
                    },

                }
            };
        }

        private static SIAFDOC CreateSiafemDocBoleto(Documento document)
        {
            return new SIAFDOC
            {
                cdMsg = "SIAFListaBoletos",
                SiafemListaBoletos = new SiafemListaBoletos
                {
                    documento = new documento
                    {
                        UnidadeGestora = document.ListaBoletos.CodigoUnidadeGestora,
                        Gestao = document.ListaBoletos.CodigoGestao,
                        Cnpj = document.ListaBoletos.NumeroCnpjcpfFavorecido,
                        Nome = document.ListaBoletos.NomeLista,
                        NomeLista = document.ListaBoletos.NomeLista,
                        NumeroLista = document.ListaBoletos.NumeroSiafem,
                        BOLETODECOBRANCA = "X",
                        Conta1Cob = document.ListaCodigoBarras.CodigoBarraBoleto.NumeroConta1,
                        Conta2Cob = document.ListaCodigoBarras.CodigoBarraBoleto.NumeroConta2,
                        Conta3Cob = document.ListaCodigoBarras.CodigoBarraBoleto.NumeroConta3,
                        Conta4Cob = document.ListaCodigoBarras.CodigoBarraBoleto.NumeroConta4,
                        Conta5Cob = document.ListaCodigoBarras.CodigoBarraBoleto.NumeroConta5,
                        Conta6Cob = document.ListaCodigoBarras.CodigoBarraBoleto.NumeroConta6,
                        DigitoB6 = document.ListaCodigoBarras.CodigoBarraBoleto.NumeroDigito,
                        Conta7Cob = document.ListaCodigoBarras.CodigoBarraBoleto.NumeroConta7
                    },

                }
            };
        }

        private static SIAFDOC CreateSiafemAltDocTaxa(Documento document)
        {
            return new SIAFDOC
            {
                cdMsg = "SIAFAltListaBoletos",
                SiafemDocAltListaBoletos = new SiafemDocAltListaBoletos
                {
                    documento = new documento
                    {
                        UnidadeGestora = document.ListaBoletos.CodigoUnidadeGestora,
                        Gestao = document.ListaBoletos.CodigoGestao,
                        NumeroLista = document.ListaBoletos.NumeroSiafem,
                        NomeLista = document.ListaBoletos.NomeLista,
                        UTILPUBLICAIMPOSTOSTAXASECONTRIBUICOES = "X",
                        Conta1 = document.ListaCodigoBarras.CodigoBarraTaxa.NumeroConta1,
                        Conta2 = document.ListaCodigoBarras.CodigoBarraTaxa.NumeroConta2,
                        Conta3 = document.ListaCodigoBarras.CodigoBarraTaxa.NumeroConta3,
                        Conta4 = document.ListaCodigoBarras.CodigoBarraTaxa.NumeroConta4
                    },

                }
            };
        }

        private static SIAFDOC CreateSiafemAltDocBoleto(Documento document)
        {
            return new SIAFDOC
            {
                cdMsg = "SIAFAltListaBoletos",
                SiafemDocAltListaBoletos = new SiafemDocAltListaBoletos
                {
                    documento = new documento
                    {
                        UnidadeGestora = document.ListaBoletos.CodigoUnidadeGestora,
                        Gestao = document.ListaBoletos.CodigoGestao,
                        Cnpj = document.ListaBoletos.NumeroCnpjcpfFavorecido,
                        Nome = document.ListaBoletos.NomeLista,
                        NumeroLista = document.ListaBoletos.NumeroSiafem,
                        NomeLista = document.ListaBoletos.NomeLista,
                        BOLETODECOBRANCA = "X",
                        Conta1Cob = document.ListaCodigoBarras.CodigoBarraBoleto != null ? document.ListaCodigoBarras.CodigoBarraBoleto.NumeroConta1 : null,
                        Conta2Cob = document.ListaCodigoBarras.CodigoBarraBoleto != null ? document.ListaCodigoBarras.CodigoBarraBoleto.NumeroConta2 : null,
                        Conta3Cob = document.ListaCodigoBarras.CodigoBarraBoleto != null ? document.ListaCodigoBarras.CodigoBarraBoleto.NumeroConta3 : null,
                        Conta4Cob = document.ListaCodigoBarras.CodigoBarraBoleto != null ? document.ListaCodigoBarras.CodigoBarraBoleto.NumeroConta4 : null,
                        Conta5Cob = document.ListaCodigoBarras.CodigoBarraBoleto != null ? document.ListaCodigoBarras.CodigoBarraBoleto.NumeroConta5 : null,
                        Conta6Cob = document.ListaCodigoBarras.CodigoBarraBoleto != null ? document.ListaCodigoBarras.CodigoBarraBoleto.NumeroConta6 : null,
                        DigitoB6 = document.ListaCodigoBarras.CodigoBarraBoleto != null ? document.ListaCodigoBarras.CodigoBarraBoleto.NumeroDigito : null,
                        Conta7Cob = document.ListaCodigoBarras.CodigoBarraBoleto != null ? document.ListaCodigoBarras.CodigoBarraBoleto.NumeroConta7 : null
                    },

                }
            };
        }

        private static SIAFDOC CreateSiafemDocNl(ReclassificacaoRetencao document)
        {
            return new SIAFDOC
            {
                cdMsg = "SIAFNL001",
                SiafemDocNL = new SiafemDocNL
                {
                    documento = new documento
                    {
                        DataEmissao = document.DataEmissao.ToSiafisicoDateTime(false),
                        UnidadeGestora = document.CodigoUnidadeGestora,
                        Gestao = document.CodigoGestao,
                        CgcCpfUgfav = document.NumeroCNPJCPFCredor,
                        GestaoFav = document.CodigoGestaoCredor
                    },
                    Evento = new Evento
                    {
                        Repeticao = new Repeticao
                        {
                            desc = document.Eventos.Select(s => new desc
                            {
                                Evento = s.NumeroEvento,
                                InscricaoEvento = s.InscricaoEvento,
                                Classificacao = s.Classificacao,
                                Fonte = s.Fonte,
                                Valor = Convert.ToString(s.ValorUnitario)
                            }).ToList()
                        }
                    },
                    Observacao = new Model.ValueObject.Service.Siafem.LiquidacaoDespesa.Observacao
                    {
                        Repeticao = new Repeticao
                        {
                            obs = FetchObservations(document)
                            .Where(w => !string.IsNullOrWhiteSpace(w)).Select(s => new obs
                            {
                                Observacao = s
                            }).ToList()
                        }
                    },
                    NotaFiscal = new NotaFiscal
                    {
                        Repeticao = new Repeticao
                        {
                            NF = document.Notas.Select(s => new NF
                            {
                                NotaFiscal = s.CodigoNotaFiscal
                            }).ToList()
                        }
                    }
                }
            };
        }


        private static SIAFDOC CreateSiafemDocProg(ProgramacaoDesembolso document)
        {
            return new SIAFDOC
            {
                cdMsg = "SIAFPD001",
                SiafemDocPD = new SiafemDocPD
                {
                    documento = new documento
                    {
                        DataEmissao = document.DataEmissao.ToSiafisicoDateTime(false),
                        UnidadeGestora = document.CodigoUnidadeGestora,
                        Gestao = document.CodigoGestao,
                        DataVencimento = document.DataVencimento.ToSiafisicoDateTime(false),
                        ListaAnexo = document.NumeroListaAnexo,
                        NlRef = document.NumeroNLReferencia,
                        UgPagadora = document.NumeroCnpjcpfCredor,
                        GestaoPagadora = document.GestaoCredor,
                        BancoPagador = document.NumeroBancoCredor ?? " ",
                        AgenciaPagadora = document.NumeroAgenciaCredor ?? " ",
                        ContaCorrentePagadora = document.NumeroContaCredor,


                        UgFavorecido = document.NumeroCnpjcpfPagto,
                        GestaoFavorecido = document.GestaoPagto ?? " ",
                        BancoFavorecido = document.NumeroBancoPagto,
                        AgenciaFavorecido = document.NumeroAgenciaPagto,
                        ContaCorrenteFavorecido = document.NumeroContaPagto,

                        Processo = document.NumeroProcesso,
                        Valor = document.Eventos.Sum(x => x.ValorUnitario).ToString("D3"),
                        Finalidade = document.Finalidade,
                        Causa1 = " ",
                        Causa2 = " "
                    },
                    Evento = new Evento
                    {
                        Repeticao = new Repeticao
                        {
                            desc = document.Eventos.Select(s => new desc
                            {
                                Evento = s.NumeroEvento,
                                InscricaoEvento = s.InscricaoEvento,
                                Classificacao = s.Classificacao,
                                Fonte = s.Fonte,
                                Valor = s.ValorUnitario.ToString("D3")
                            }).ToList()
                        }
                    }
                }
            };
        }


        private static SIAFDOC CreateSiafemDocCanProg(IProgramacaoDesembolso document)
        {
            return new SIAFDOC
            {
                cdMsg = "SIAFCanPD",
                SiafemDocCanPD = new SiafemDocCanPD
                {
                    documento = new documento
                    {
                        DataEmissao = document.DataEmissao.ToSiafisicoDateTime(false),
                        UnidadeGestora = document.CodigoUnidadeGestora,
                        Gestao = document.CodigoGestao,
                        Prefixo = document.NumeroSiafem?.Substring(0, 6)
                    },
                    descricao = new descricao
                    {
                        Repeticao = new Repeticao
                        {
                            des = new List<des>
                            {
                                new des {
                                NumPD = document.NumeroSiafem?.Substring(6,5),
                                Causa1 = ListaString(60, document.CausaCancelamento, 2)[0],
                                Causa2 = ListaString(60, document.CausaCancelamento, 2)[1]
                                }
                            }
                        }
                    }
                }
            };
        }

        private static SIAFDOC CreateSiafemDocNlpObras(ReclassificacaoRetencao document)
        {
            return new SIAFDOC
            {
                cdMsg = "SIAFNLPGOBRAS",
                SiafemDocNLPGObras = new SiafemDocNLPGObras
                {
                    documento = new documento
                    {
                        DataEmissao = document.DataEmissao.ToSiafisicoDateTime(),
                        UnidadeGestora = document.CodigoUnidadeGestora,
                        Gestao = document.CodigoGestao,
                        Empenho = document.NumeroOriginalSiafemSiafisico,
                        Normal = document.NormalEstorno == "1" ? "X" : null,
                        Estorno = document.NormalEstorno == "2" ? "X" : null,
                        AnoMedicao = document.AnoMedicao,
                        MesMedicao = document.MesMedicao,
                        Situacao = "1",
                        Valor = Convert.ToString(document.Valor),
                        Evento = document.CodigoEvento,
                        Classificacao = document.CodigoClassificacao,
                        InscricaoEvento = document.CodigoInscricao,
                        Fonte = document.CodigoFonte,
                        Obs01 = document.DescricaoObservacao1?.NormalizeForService(),
                        Obs02 = document.DescricaoObservacao2?.NormalizeForService(),
                        Obs03 = document.DescricaoObservacao3?.NormalizeForService(),
                        NotaFiscal1 = document.Notas.FirstOrDefault(f => f.Ordem == 1)?.CodigoNotaFiscal,
                        NotaFiscal2 = document.Notas.FirstOrDefault(f => f.Ordem == 2)?.CodigoNotaFiscal,
                        NotaFiscal3 = document.Notas.FirstOrDefault(f => f.Ordem == 3)?.CodigoNotaFiscal,
                        NotaFiscal4 = document.Notas.FirstOrDefault(f => f.Ordem == 4)?.CodigoNotaFiscal,
                        NotaFiscal5 = document.Notas.FirstOrDefault(f => f.Ordem == 5)?.CodigoNotaFiscal,
                        NotaFiscal6 = document.Notas.FirstOrDefault(f => f.Ordem == 6)?.CodigoNotaFiscal,
                        NotaFiscal7 = document.Notas.FirstOrDefault(f => f.Ordem == 7)?.CodigoNotaFiscal,
                        NotaFiscal8 = document.Notas.FirstOrDefault(f => f.Ordem == 8)?.CodigoNotaFiscal,
                        NotaFiscal9 = document.Notas.FirstOrDefault(f => f.Ordem == 9)?.CodigoNotaFiscal,
                        NotaFiscal10 = document.Notas.FirstOrDefault(f => f.Ordem == 10)?.CodigoNotaFiscal,
                        NotaFiscal11 = document.Notas.FirstOrDefault(f => f.Ordem == 11)?.CodigoNotaFiscal,
                        NotaFiscal12 = document.Notas.FirstOrDefault(f => f.Ordem == 12)?.CodigoNotaFiscal,
                        NotaFiscal13 = document.Notas.FirstOrDefault(f => f.Ordem == 13)?.CodigoNotaFiscal,
                        NotaFiscal14 = document.Notas.FirstOrDefault(f => f.Ordem == 14)?.CodigoNotaFiscal,
                        NotaFiscal15 = document.Notas.FirstOrDefault(f => f.Ordem == 15)?.CodigoNotaFiscal,
                    }
                }
            };
        }

        private static SIAFDOC CreateSiafemDocSiafnlIssReten(ReclassificacaoRetencao document)
        {
            return new SIAFDOC
            {
                cdMsg = "SIAFNLIssReten",
                SIAFNLIssReten = new SIAFNLIssReten
                {
                    documento = new documento
                    {
                        DataEmissao = document.DataEmissao.ToSiafisicoDateTime(),
                        UnidadeGestora = document.CodigoUnidadeGestora,
                        Gestao = document.CodigoGestao,
                        Empenho = document.NumeroOriginalSiafemSiafisico,
                        Normal = document.NormalEstorno == "1" ? "X" : null,
                        Estorno = document.NormalEstorno == "2" ? "X" : null,
                        Valor = Convert.ToString(document.Valor),
                        RestosaPagar = document.RestoPagarId,
                        NLRefLiqMedicao = document.NotaLancamenoMedicao,
                        CNPJDAPREFEITURA = document.NumeroCnpjPrefeitura,
                        Obs01 = document.DescricaoObservacao1?.NormalizeForService(),
                        Obs02 = document.DescricaoObservacao2?.NormalizeForService(),
                        Obs03 = document.DescricaoObservacao3?.NormalizeForService()
                    }
                }
            };
        }

        private static SIAFDOC CreateSiafemDocSiafnlRetInss(ReclassificacaoRetencao document)
        {
            return new SIAFDOC
            {
                cdMsg = "SIAFNLRetInss",
                SIAFNLRetInss = new SIAFNLRetInss
                {
                    documento = new documento
                    {
                        DataEmissao = document.DataEmissao.ToSiafisicoDateTime(),
                        UnidadeGestora = document.CodigoUnidadeGestora,
                        Gestao = document.CodigoGestao,
                        Empenho = document.NumeroOriginalSiafemSiafisico,
                        Normal = document.NormalEstorno == "1" ? "X" : null,
                        Estorno = document.NormalEstorno == "2" ? "X" : null,
                        Valor = Convert.ToString(document.Valor),
                        RestosaPagar = document.RestoPagarId,
                        NLRefLiqMedicao = document.NotaLancamenoMedicao,
                        Obs01 = document.DescricaoObservacao1?.NormalizeForService(),
                        Obs02 = document.DescricaoObservacao2?.NormalizeForService(),
                        Obs03 = document.DescricaoObservacao3?.NormalizeForService(),
                        NotaFiscal1 = document.Notas.FirstOrDefault(f => f.Ordem == 1)?.CodigoNotaFiscal,
                        NotaFiscal2 = document.Notas.FirstOrDefault(f => f.Ordem == 2)?.CodigoNotaFiscal,
                        NotaFiscal3 = document.Notas.FirstOrDefault(f => f.Ordem == 3)?.CodigoNotaFiscal,
                        NotaFiscal4 = document.Notas.FirstOrDefault(f => f.Ordem == 4)?.CodigoNotaFiscal,
                        NotaFiscal5 = document.Notas.FirstOrDefault(f => f.Ordem == 5)?.CodigoNotaFiscal,
                        NotaFiscal6 = document.Notas.FirstOrDefault(f => f.Ordem == 6)?.CodigoNotaFiscal,
                        NotaFiscal7 = document.Notas.FirstOrDefault(f => f.Ordem == 7)?.CodigoNotaFiscal,
                        NotaFiscal8 = document.Notas.FirstOrDefault(f => f.Ordem == 8)?.CodigoNotaFiscal,
                        NotaFiscal9 = document.Notas.FirstOrDefault(f => f.Ordem == 9)?.CodigoNotaFiscal,
                        NotaFiscal10 = document.Notas.FirstOrDefault(f => f.Ordem == 10)?.CodigoNotaFiscal,
                        NotaFiscal11 = document.Notas.FirstOrDefault(f => f.Ordem == 11)?.CodigoNotaFiscal,
                        NotaFiscal12 = document.Notas.FirstOrDefault(f => f.Ordem == 12)?.CodigoNotaFiscal,
                        NotaFiscal13 = document.Notas.FirstOrDefault(f => f.Ordem == 13)?.CodigoNotaFiscal,
                        NotaFiscal14 = document.Notas.FirstOrDefault(f => f.Ordem == 14)?.CodigoNotaFiscal,
                        NotaFiscal15 = document.Notas.FirstOrDefault(f => f.Ordem == 15)?.CodigoNotaFiscal
                    }
                }
            };
        }

        private SIAFDOC CreateSiafemConsultaPd(string unidadeGestora, string numeroSiafemSiafisco)
        {
            return new SIAFDOC
            {
                cdMsg = "SIAFConsultaPD",
                SiafemDocConsultaPD = new SiafemDocConsultaPD
                {
                    documento = new documento
                    {
                        UnidadeGestora = unidadeGestora,
                        Gestao = "16055",
                        NumeroPD = numeroSiafemSiafisco
                    }
                }
            };
        }

        //public static SIAFDOC CreateSiafemImpressaoRelacaoReRt(ImpressaoRelacaoRERT document)
        //{
        //    return new SIAFDOC
        //    {
        //        cdMsg = "SIAFRE",
        //        SiafemDocRE = new SiafemDocRE()
        //        {
        //            Documento = new DocumentoImpressaoRelacaoReRt()
        //            {
        //                UnidadeGestora = document.CodigoUnidadeGestora,
        //                Gestao = document.CodigoGestao,
        //                Banco = document.CodigoBanco,
        //                DataSolicitacao = document.DataSolicitacao.ToString(),
        //                NumeroRelatorio = document.CodigoRelacaoRERT
        //            }
        //        }
        //    };
        //}

        #endregion
        private static IEnumerable<string> FetchObservations(ReclassificacaoRetencao document)
        {
            return new List<string> {
                document.DescricaoObservacao1?.NormalizeForService(),
                document.DescricaoObservacao2?.NormalizeForService(),
                document.DescricaoObservacao3?.NormalizeForService()
            };
        }

        internal class Documento
        {
            public ListaCodigoBarras ListaCodigoBarras { get; set; }
            public ListaBoletos ListaBoletos { get; set; }

        }

        private XmlDocument ConverterXml(string xml)
        {
            try
            {
                xml = xml.Replace("&", "&amp;");

                var document = new XmlDocument();

                document.LoadXml(xml);

                return document;
            }
            catch
            {
                throw new SiafemException(xml);
            }
        }

        private T ConvertNode<T>(XmlNodeList node) where T : class
        {
            MemoryStream stm = new MemoryStream();

            StreamWriter stw = new StreamWriter(stm);
            stw.Write(node[node.Count - 1].OuterXml);
            stw.Flush();

            stm.Position = 0;


            XmlSerializer ser = new XmlSerializer(typeof(T));
            T result = ser.Deserialize(stm) as T;

            return result;
        }
    }
}
