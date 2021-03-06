﻿namespace Sids.Prodesp.Infrastructure.DataBase.Empenho
{
    using Helpers;
    using Model.Entity.Empenho;
    using Model.Interface.Empenho;
    using System.Collections.Generic;
    using System.Data.SqlClient;

    public class EmpenhoReforcoItemDal : ICrudEmpenhoReforcoItem
    {
        public int Edit(EmpenhoReforcoItem entity)
        {
            return DataHelper.Get<int>("PR_EMPENHO_REFORCO_ITEM_ALTERAR",
                new SqlParameter("@id_item_reforco", entity.Id),
                new SqlParameter("@tb_empenho_reforco_id_empenho_reforco", entity.EmpenhoId),
                new SqlParameter("@cd_item_servico", entity.CodigoItemServico),
                new SqlParameter("@cd_unidade_fornecimento", entity.CodigoUnidadeFornecimentoItem),
                new SqlParameter("@ds_unidade_medida", entity.DescricaoUnidadeMedida),
                new SqlParameter("@qt_material_servico", entity.QuantidadeMaterialServico),
                new SqlParameter("@ds_justificativa_preco", entity.DescricaoJustificativaPreco),
                new SqlParameter("@ds_item_siafem", entity.DescricaoItemSiafem),
                new SqlParameter("@vr_unitario", entity.ValorUnitario),
                new SqlParameter("@vr_preco_total ", entity.ValorTotal),
                new SqlParameter("@ds_status_siafisico_item", entity.StatusSiafisicoItem),
                new SqlParameter("@nr_sequeincia", entity.SequenciaItem));
        }

        public IEnumerable<EmpenhoReforcoItem> Fetch(EmpenhoReforcoItem entity)
        {
            return DataHelper.List<EmpenhoReforcoItem>("PR_EMPENHO_REFORCO_ITEM_CONSULTAR",
                new SqlParameter("@id_item_reforco", entity.Id),
                new SqlParameter("@tb_empenho_reforco_id_empenho_reforco", entity.EmpenhoId),
                new SqlParameter("@cd_item_servico", entity.CodigoItemServico),
                new SqlParameter("@cd_unidade_fornecimento", entity.CodigoUnidadeFornecimentoItem),
                new SqlParameter("@ds_unidade_medida", entity.DescricaoUnidadeMedida),
                new SqlParameter("@qt_material_servico", entity.QuantidadeMaterialServico),
                new SqlParameter("@ds_justificativa_preco", entity.DescricaoJustificativaPreco),
                new SqlParameter("@ds_item_siafem", entity.DescricaoItemSiafem),
                new SqlParameter("@vr_unitario", entity.ValorUnitario),
                new SqlParameter("@vr_preco_total ", entity.ValorTotal),
                new SqlParameter("@ds_status_siafisico_item", entity.StatusSiafisicoItem)
            );
        }

        public int Remove(int id)
        {
            return DataHelper.Get<int>("PR_EMPENHO_REFORCO_ITEM_EXCLUIR",
                new SqlParameter("@id_item_reforco", id)
            );
        }

        public int Add(EmpenhoReforcoItem entity)
        {
            return DataHelper.Get<int>("PR_EMPENHO_REFORCO_ITEM_INCLUIR",
                new SqlParameter("@tb_empenho_reforco_id_empenho_reforco", entity.EmpenhoId),
                new SqlParameter("@cd_item_servico", entity.CodigoItemServico),
                new SqlParameter("@cd_unidade_fornecimento", entity.CodigoUnidadeFornecimentoItem),
                new SqlParameter("@ds_unidade_medida", entity.DescricaoUnidadeMedida),
                new SqlParameter("@qt_material_servico", entity.QuantidadeMaterialServico),
                new SqlParameter("@nr_sequeincia", entity.SequenciaItem),
                new SqlParameter("@ds_justificativa_preco", entity.DescricaoJustificativaPreco),
                new SqlParameter("@ds_item_siafem", entity.DescricaoItemSiafem),
                new SqlParameter("@vr_unitario", entity.ValorUnitario),
                new SqlParameter("@vr_preco_total ", entity.ValorTotal),
                new SqlParameter("@ds_status_siafisico_item", entity.StatusSiafisicoItem)
            );
        }

        public string GetTableName()
        {
            return "tb_empenho_reforco_item";
        }
    }
}
