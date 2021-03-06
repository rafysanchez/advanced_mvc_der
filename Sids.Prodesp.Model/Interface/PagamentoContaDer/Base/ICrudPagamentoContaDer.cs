﻿using System;
using System.Collections.Generic;

namespace Sids.Prodesp.Model.Interface.PagamentoContaDer.Base
{
    public interface ICrudPagamentoContaDer<T> where T: class
    {
        IEnumerable<T> FetchForGrid(T entity);

        int Remove(int id);

        IEnumerable<T> Fetch(T entity);

        int Save(T entity);

        T Get(int? id, int? idAutorizacaoOB);


        T Get(int id);

        IEnumerable<T> FetchForGrid(T entity, DateTime since, DateTime until);
    }
}
