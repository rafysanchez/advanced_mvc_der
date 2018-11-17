 -- ==============================================================
-- Author:  JOSE BRAZ
-- Create date: 31/01/2018
-- Description: Procedure para Excluir pagamento.tb_confirmacao_pagamento
-- ==============================================================
CREATE PROCEDURE[dbo].[PR_CONFIRMACAO_PAGAMENTO_EXCLUIR]
@id INT = NULL
AS

BEGIN

	SET NOCOUNT ON;

	DELETE FROM [pagamento].[tb_confirmacao_pagamento_item] WHERE [id_confirmacao_pagamento] = @id;

	DELETE FROM [pagamento].[tb_confirmacao_pagamento_totais] WHERE [id_confirmacao_pagamento] = @id;

	DELETE FROM [pagamento].[tb_confirmacao_pagamento] WHERE [id_confirmacao_pagamento] = @id;

END

-----------------------------------------------------------------------------------------