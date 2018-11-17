using Sids.Prodesp.Infrastructure.Helpers;
using Sids.Prodesp.Model.Entity.PagamentoContaUnica.PagamentoDer;
using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using Sids.Prodesp.Model.Interface.PagamentoContaDer;


namespace Sids.Prodesp.Infrastructure.DataBase.PagamentoDer
{
    public class ConfirmacaoPagamentoOrigemDal : ICrudConfirmacaoPagamentoOrigem
    {
        public IEnumerable<ConfirmacaoPagamentoOrigem> Fetch(ConfirmacaoPagamentoOrigem entity)
        {
            var paramId_origem = new SqlParameter("@id_origem", entity.Id);
            var paramDs_origem = new SqlParameter("@ds_origem", entity.Descricao);

            var dbResult = DataHelper.List<ConfirmacaoPagamentoOrigem>("PR_CONFIRMACAO_PAGAMENTO_ORIGEM_CONSULTAR", paramId_origem, paramDs_origem);

            return dbResult;
        }

        public IEnumerable<ConfirmacaoPagamentoOrigem> FetchForGrid(ConfirmacaoPagamentoOrigem entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ConfirmacaoPagamentoOrigem> FetchForGrid(ConfirmacaoPagamentoOrigem entity, DateTime since, DateTime until)
        {
            throw new NotImplementedException();
        }

        public ConfirmacaoPagamentoOrigem Get(int id)
        {
            throw new NotImplementedException();
        }

        public ConfirmacaoPagamentoOrigem Get(int? id, int? idAutorizacaoOB)
        {
            throw new NotImplementedException();
        }

        public int Remove(int id)
        {
            throw new NotImplementedException();
        }

        public int Save(ConfirmacaoPagamentoOrigem entity)
        {
            throw new NotImplementedException();
        }
    }
}
