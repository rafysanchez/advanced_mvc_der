using System.ComponentModel.DataAnnotations.Schema;

namespace Sids.Prodesp.Model.Entity.PagamentoContaUnica.PagamentoDer
{
    public class ConfirmacaoPagamentoOrigem
    {
        [Column("id_origem")]
        public int Id { get; set; }

        [Column("ds_origem")]
        public string Descricao { get; set; }
    }
}
