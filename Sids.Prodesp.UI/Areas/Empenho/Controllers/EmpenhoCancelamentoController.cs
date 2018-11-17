﻿namespace Sids.Prodesp.UI.Areas.Empenho.Controllers
{
    using Application;
    using Model.Entity.Empenho;
    using Model.Enum;
    using Report;
    using Security;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    public class EmpenhoCancelamentoController : EmpenhoBaseController
    {
        [PermissaoAcesso(Controller = typeof(EmpenhoCancelamentoController), Operacao = "Listar")]
        public ActionResult Index(string id)
        {
            try
            {
                App.PerfilService.SetCurrentFilter(null, "EmpenhoCancelamento");
                App.FuncionalidadeService.SalvarFuncionalidadeAtual(Convert.ToInt32(id));
                return View("Index", Display(new EmpenhoCancelamento()));
            }
            catch
            {
                return RedirectToAction("Index", "Home", new { Area = string.Empty });
            }
        }

        [PermissaoAcesso(Controller = typeof(EmpenhoCancelamentoController), Operacao = "Listar"), HttpPost]
        public ActionResult Index(FormCollection form)
        {
            try
            {
                App.PerfilService.SetCurrentFilter(form, "EmpenhoCancelamento");
                _filterItems = Display(new EmpenhoCancelamento(), 
                    form);

                if (!_filterItems.Any())
                    ExibirMensagemErro("Registro não encontrado.");

                return View("Index", _filterItems);
            }
            catch (Exception ex)
            {
                ExibirMensagemErro("Não foi possível concluir a consulta. " + ex.Message);
                return View("Index");
            }
        }

        [PermissaoAcesso(Controller = typeof(EmpenhoCancelamentoController), Operacao = "Incluir")]
        public ActionResult Create(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
                App.FuncionalidadeService.SalvarFuncionalidadeAtual(Convert.ToInt32(id));

            return View("CreateEdit", Display(new EmpenhoCancelamento(), true));
        }

        [PermissaoAcesso(Controller = typeof(EmpenhoCancelamentoController), Operacao = "Incluir")]
        public ActionResult CreateThis(string id)
        {
            var objModel = App.EmpenhoCancelamentoService
                .Buscar(new EmpenhoCancelamento { Id = Convert.ToInt32(id) }).SingleOrDefault();
            return View("CreateEdit", Display(objModel, true));
        }

        [PermissaoAcesso(Controller = typeof(EmpenhoCancelamentoController), Operacao = "Alterar")]
        public ActionResult Edit(string id)
        {
            var objModel = App.EmpenhoCancelamentoService.Buscar(new EmpenhoCancelamento { Id = Convert.ToInt32(id) }).FirstOrDefault();

            return View("CreateEdit", Display(objModel, false));
        }

        [PermissaoAcesso(Controller = typeof(EmpenhoCancelamentoController), Operacao = "Excluir")]
        public ActionResult Delete(string id)
        {
            try
            {
                var objModel = App.EmpenhoCancelamentoService.Buscar(new EmpenhoCancelamento { Id = Convert.ToInt32(id) }).Single();
                if (objModel.TransmitidoSiafem || objModel.TransmitidoSiafisico || objModel.TransmitidoProdesp)
                    throw new Exception("O Cancelamento do Empenho transmitido não pode ser excluído");

                var acao = App.EmpenhoCancelamentoService.Excluir(objModel, Convert.ToInt32(_funcId), Convert.ToInt16(EnumAcao.Excluir));
                return Json(acao.ToString(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Imprimir(string id)
        {
            try
            {
                var usuario = App.AutenticacaoService.GetUsuarioLogado();
                var empenho = App.EmpenhoCancelamentoService.Buscar(new EmpenhoCancelamento { Id = Convert.ToInt32(id) }).FirstOrDefault();
                var consultaNe = App.EmpenhoCancelamentoService.ConsultaNe(empenho, usuario);

                Session[App.BaseService.GetCurrentIp()] = HelperReport.GerarPdfEmpenho(consultaNe, "Cancelamento de Empenho", empenho);

                return Json(new { Status = "Sucesso" });
            }
            catch (Exception ex)
            {
                return Json(new { Status = "Falha", Msg = ex.Message });
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Transmitir(ModelSalvar objSave)
        {
            var objModel = new EmpenhoCancelamento();
            try
            {
                var usuario = App.AutenticacaoService.GetUsuarioLogado();

                _modelId = SalvarService(objSave, (int)_funcId);

                App.EmpenhoCancelamentoService.Transmitir(_modelId, usuario, (int)_funcId);

                objModel = App.EmpenhoCancelamentoService.Buscar(
                    new EmpenhoCancelamento { Id = _modelId }).FirstOrDefault();

                return Json(new { Status = "Sucesso", Codigo = objModel.Id, objModel });
            }
            catch (Exception ex)
            {
                objModel = App.EmpenhoCancelamentoService.Buscar(
                    new EmpenhoCancelamento { Id = _modelId }).FirstOrDefault();

                var status = "Falha";

                if (objModel.StatusProdesp.Equals("E") && objModel.TransmitirProdesp)
                    status = "Falha Prodesp";

                if (objModel.StatusProdesp == "S" && !objModel.StatusDocumento &&
                   objModel.StatusSiafemSiafisico == "S")
                    status = "Falha Doc";

                return Json(new { Status = status, Msg = ex.Message, Codigo = objModel.Id, objModel });
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Retransmitir(List<int> ids)
        {
            try
            {
                var usuario = App.AutenticacaoService.GetUsuarioLogado();
                var msg = App.EmpenhoCancelamentoService.Transmitir(ids, usuario, (int)_funcId);

                return Json(new { Status = string.IsNullOrWhiteSpace(msg) ? "Sucesso" : "Falha", Msg = msg });
            }
            catch (Exception ex) { return Json(new { Status = "Falha", Msg = ex.Message }); }
        }

        [PermissaoAcesso(Controller = typeof(EmpenhoController), Operacao = "Listar")]
        public ActionResult AtualizarIndex()
        {
            try
            {
                var filtro = App.ProgramaService.GetCurrentFilter("EmpenhoCancelamento");
                return filtro != null
                    ? Index(filtro)
                    : RedirectToAction("Index", new { Id = _funcId.ToString() });
            }
            catch (Exception ex)
            {
                ExibirMensagemErro("Não foi possível concluir a consulta. " + ex.Message);
                return View("Index", _filterItems);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Save(ModelSalvar objSave)
        {
            try
            {
                return Json(new { Status = "Sucesso", Id = SalvarService(objSave, Convert.ToInt32(_funcId)) });
            }
            catch (Exception ex)
            {
                return Json(new { Status = "Falha", Msg = ex.Message });
            }
        }

        private int SalvarService(ModelSalvar objSave, int funcionalidade)
        {
            var acao = objSave.Model.Id > 0 ? EnumAcao.Alterar : EnumAcao.Inserir;

            return App.EmpenhoCancelamentoService.Salvar(
                objSave.Model,
                objSave.Meses ?? new List<EmpenhoCancelamentoMes>(),
                objSave.Itens ?? new List<EmpenhoCancelamentoItem>(),
                funcionalidade,
                Convert.ToInt16(acao));
        }

        public class ModelSalvar
        {
            public EmpenhoCancelamento Model { get; set; }
            public IEnumerable<EmpenhoCancelamentoItem> Itens { get; set; }
            public IEnumerable<EmpenhoCancelamentoMes> Meses { get; set; }
        }
    }
}