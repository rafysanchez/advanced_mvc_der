using Sids.Prodesp.Infrastructure.Helpers;
using Sids.Prodesp.Model.Entity.PagamentoContaUnica.PagamentoDer;
using Sids.Prodesp.Model.Interface.Base;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sids.Prodesp.Infrastructure.DataBase.PagamentoDer
{
    public class NlParametrizacaoDespesaTipoDal : ICrudBase<NlParametrizacaoDespesaTipo>
    {

        public NlParametrizacaoDespesaTipoDal() { }

        public string GetTableName()
        {
            return "tb_despesa_tipo";
        }

        public int Add(NlParametrizacaoDespesaTipo entity)
        {
            var sqlParameterList = DataHelper.GetSqlParameterList(entity);
            return DataHelper.Get<int>("[dbo].[PR_TIPO_DESPESA_INCLUIR]", sqlParameterList);
        }

        public int Edit(NlParametrizacaoDespesaTipo entity)
        {
            var sqlParameterList = DataHelper.GetSqlParameterList(entity);
            return DataHelper.Get<int>("[dbo].[PR_TIPO_DESPESA_ALTERAR]", sqlParameterList);
        }

        public IEnumerable<NlParametrizacaoDespesaTipo> Fetch(NlParametrizacaoDespesaTipo entity)
        {
            var sqlParameterList = DataHelper.GetSqlParameterList(entity);
            return DataHelper.List<NlParametrizacaoDespesaTipo>("[dbo].[PR_TIPO_DESPESA_CONSULTAR]", sqlParameterList);
        }

        public int Remove(int id)
        {
            return DataHelper.Get<int>("[dbo].[PR_TIPO_DESPESA_EXCLUIR]", new SqlParameter("@id_despesa_tipo", id));
        }

        public string GetById(int id = 0)
        {

            var ret = DataHelper.List<NlParametrizacaoDespesaTipo>("[dbo].[PR_TIPO_DESPESA_CONSULTAR_POR_ID]", new SqlParameter("@id_despesa_tipo", id));
            if (ret.FirstOrDefault()!=null)
            {
                return ret.FirstOrDefault().Descricao;
            }
            else
            {
                return null;
            }      

        }
    }
}