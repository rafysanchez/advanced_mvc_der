using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sids.Prodesp.Model.Entity.PagamentoContaUnica.PagamentoDer
{
    public class ConfirmacaoPagamentoTotais
    {
        [Column("id_confirmacao_pagamento_total")]
        public int IdConfirmacaoPagamentoTotal { get; set; }

        [Column("id_confirmacao_pagamento")]
        public int IdConfirmacaoPagamento { get; set; }

        [Column("nr_fonte_lista")]
        public string NrFonteLista { get; set; }

        [Column("vr_total_fonte_lista")]
        public decimal VrTotalFonteLista { get; set; }

        [Column("vr_total_confirmar_ir")]
        public decimal VrTotalConfirmarIR { get; set; }
       
        [Column("vr_total_confirmar_issqn")]
        public decimal VrTotalConfirmarISSQN { get; set; }

        [Column("vr_total_confirmar")]
        public decimal VrTotalConfirmar { get; set; }
    }
}
