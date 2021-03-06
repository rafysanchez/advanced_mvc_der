﻿namespace Sids.Prodesp.Core.Services.Empenho
{
    using Base;
    using Model.Entity.Empenho;
    using Model.Interface.Empenho;
    using Model.Interface.Log;
    using Model.Enum;
    using Model.Extension;
    using Model.Interface;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Model.Entity.Seguranca;

    public class EmpenhoItemService : BaseService, IEmpenhoItemService<EmpenhoItem>
    {
        private readonly ICrudEmpenhoItem _itemService;
        public EmpenhoItemService(ILogError l, ICrudEmpenhoItem item) : base(l)
        {
            _itemService = item;
        }

        public AcaoEfetuada Excluir(EmpenhoItem objModel, int recursoId, int acaoId)
        {
            try
            {
                _itemService.Remove(objModel.Id);

                return AcaoEfetuada.Sucesso;
            }
            catch (Exception ex)
            {
                throw SaveLog(ex, actionId: (short?)acaoId, functionalityId: recursoId);
            }
        }

        public AcaoEfetuada Alterar(EmpenhoItem objModel, int recursoId, int acaoId)
        {
            try
            {
                _itemService.Edit(objModel);

                return AcaoEfetuada.Sucesso;
            }
            catch (Exception ex)
            {
                throw SaveLog(ex, actionId: (short?)acaoId, functionalityId: recursoId);
            }
        }

        public AcaoEfetuada Salvar(EmpenhoItem objModel, int recursoId, int acaoId)
        {
            try
            {
                objModel.QuantidadeMaterialServico = objModel.QuantidadeMaterialServico.ParaInteiro();
                objModel.ValorUnitario = objModel.ValorUnitario.ParaInteiro();
                objModel.ValorTotal = objModel.ValorTotal.ParaInteiro();

                if (objModel.Id == 0)
                    objModel.Id = _itemService.Add(objModel);
                else
                    _itemService.Edit(objModel);

                var arg = string.Format("Empenho item {0}, Codigo {1}", objModel.Id, objModel.EmpenhoId);


                objModel.QuantidadeMaterialServico = objModel.QuantidadeMaterialServico.ParaDecimal();
                objModel.ValorUnitario = objModel.ValorUnitario.ParaDecimal();
                objModel.ValorTotal = objModel.ValorTotal.ParaDecimal();

                return AcaoEfetuada.Sucesso;
            }
            catch (Exception ex)
            {
                throw SaveLog(ex, actionId: (short)acaoId, functionalityId: recursoId);
            }
        }

        public AcaoEfetuada Salvar(int empenhoId, IEnumerable<EmpenhoItem> objModel, int recursoId, short acaoId)
        {
            try
            {
                if (objModel != null)
                {
                    var itens = Buscar(new EmpenhoItem { EmpenhoId = empenhoId });

                    ((List<EmpenhoItem>)objModel).ForEach(e => e.Id = itens.FirstOrDefault(f => f.CodigoItemServico == e.CodigoItemServico)?.Id ?? 0);


                    var itensDiferentes = itens.Where(w => !objModel.Any(a => a.CodigoItemServico == (w.CodigoItemServico) || a.ValorTotal <= 0M));

                    foreach (var item in itensDiferentes)
                    {
                        Excluir(item, recursoId, 3);
                    }

                    var idsExcluidos = itensDiferentes.Select(x => x.Id).ToArray();
                    var itensParaSalvar = itensDiferentes.Where(x => !idsExcluidos.Contains(x.Id) && x.ValorTotal > 0M);

                    itensParaSalvar = (itensParaSalvar != null && itensParaSalvar.Any()) ? itensParaSalvar : objModel;

                    foreach (var item in itensParaSalvar)
                        Salvar(item, recursoId, acaoId);

                    return AcaoEfetuada.Sucesso;

                }

                else
                {

                    return AcaoEfetuada.Sucesso;
                }

            }
            catch (Exception ex)
            {
                throw SaveLog(ex, actionId: acaoId, functionalityId: recursoId);
            }
        }

        public IEnumerable<EmpenhoItem> Buscar(EmpenhoItem objModel)
        {
            var itens = _itemService.Fetch(objModel).ToList();

            foreach (var obj in itens)
            {
                obj.QuantidadeMaterialServico = obj.QuantidadeMaterialServico.ParaDecimal();
                obj.ValorTotal = obj.ValorTotal.ParaDecimal();
                obj.ValorUnitario = obj.ValorUnitario.ParaDecimal();
            }

            return itens;
           
        }
    }
}
