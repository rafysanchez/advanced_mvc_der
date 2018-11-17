 -- ==============================================================
-- Author:  Jose Braz
-- Create date: 30/01/2018
-- Description: Procedure para Excluir pagamento.tb_confirmacao_pagamento_item
-- ==============================================================
CREATE PROCEDURE[dbo].[PR_CONFIRMACAO_PAGAMENTO_ITEM_EXCLUIR]
@id INT = NULL
AS

BEGIN

	SET NOCOUNT ON;

	DELETE FROM [pagamento].[tb_confirmacao_pagamento_item] WHERE [id_confirmacao_pagamento] = @id;

END

-----------------------------------------------------------------------------------------