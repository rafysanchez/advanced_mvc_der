
/****** Object:  StoredProcedure [dbo].[PR_CONFIRMACAO_PAGAMENTO_CONSULTAR_TOTAIS]    Script Date: 05/11/2018 11:03:51 *****
DESENVOLVIDO POR RAFAEL SANCHEZ  - MAGNA-IBM NOV 2018
*/

CREATE PROCEDURE PR_CONFIRMACAO_PAGAMENTO_CONSULTAR_TOTAIS(
@id_confirmacao_pagamento int = NULL)
AS
BEGIN 
	
	SET NOCOUNT ON;

	SELECT  
	id_confirmacao_pagamento,
    nr_fonte_lista,
    vr_total_fonte_lista,
    vr_total_confirmar_ir,
    vr_total_confirmar_issqn,
    vr_total_confirmar
	FROM pagamento.tb_confirmacao_pagamento_totais
	WHERE id_confirmacao_pagamento =  @id_confirmacao_pagamento ;
	
	
	
END


