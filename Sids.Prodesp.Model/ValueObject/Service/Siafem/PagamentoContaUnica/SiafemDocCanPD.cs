﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sids.Prodesp.Model.ValueObject.Service.Siafem.LiquidacaoDespesa;

namespace Sids.Prodesp.Model.ValueObject.Service.Siafem.PagamentoContaUnica
{
    public class SiafemDocCanPD
    {
        public documento documento { get; set; }

        public descricao descricao { get; set; }
    }
}
