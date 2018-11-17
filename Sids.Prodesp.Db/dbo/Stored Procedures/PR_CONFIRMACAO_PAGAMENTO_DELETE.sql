
CREATE PROCEDURE [dbo].[PR_CONFIRMACAO_PAGAMENTO_DELETE]
(	
		@nr_agrupamento int = null
      --@id_confirmacao_pagamento int = null
	  --,@id_confirmacao_pagamento_item int = null
)
As
Begin
BEGIN TRANSACTION  
SET NOCOUNT ON;  

Begin

DECLARE @IdConfirmacaoPagamento int
SET @IdConfirmacaoPagamento = (select id_confirmacao_pagamento from [pagamento].[tb_confirmacao_pagamento] where nr_agrupamento = @nr_agrupamento);

Begin
	 DELETE FROM [pagamento].[tb_confirmacao_pagamento_totais]
     WHERE id_confirmacao_pagamento =  @IdConfirmacaoPagamento
End

Begin
	 DELETE FROM [pagamento].[tb_confirmacao_pagamento_item]
     WHERE id_confirmacao_pagamento =  @IdConfirmacaoPagamento
	 --And id_confirmacao_pagamento_item = @id_confirmacao_pagamento_item
End

Begin
	DELETE FROM [pagamento].[tb_confirmacao_pagamento]
    WHERE id_confirmacao_pagamento =  @IdConfirmacaoPagamento
End

END

COMMIT
End