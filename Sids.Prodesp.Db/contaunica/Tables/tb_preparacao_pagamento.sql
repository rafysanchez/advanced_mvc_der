﻿CREATE TABLE [contaunica].[tb_preparacao_pagamento] (
    [id_preparacao_pagamento]         INT             IDENTITY (1, 1) NOT NULL,
    [id_regional]                     SMALLINT        NULL,
    [id_tipo_documento]               INT             NULL,
    [id_tipo_preparacao_pagamento]    INT             NULL,
    [nr_op_inicial]                   VARCHAR (18)    NULL,
    [nr_op_final]                     VARCHAR (18)    NULL,
    [nr_ano_exercicio]                INT             NULL,
    [cd_credor_organizacao]           CHAR (1)        NULL,
    [nr_cpf_cnpj_credor]              VARCHAR (15)    NULL,
    [cd_assinatura]                   VARCHAR (5)     NULL,
    [cd_grupo_assinatura]             VARCHAR (1)     NULL,
    [cd_orgao_assinatura]             VARCHAR (2)     NULL,
    [ds_cargo_assinatura]             VARCHAR (55)    NULL,
    [nm_assinatura]                   VARCHAR (55)    NULL,
    [cd_contra_assinatura]            VARCHAR (5)     NULL,
    [cd_grupo_contra_assinatura]      VARCHAR (1)     NULL,
    [cd_orgao_contra_assinatura]      VARCHAR (2)     NULL,
    [nm_contra_assinatura]            VARCHAR (55)    NULL,
    [ds_cargo_contra_assinatura]      VARCHAR (55)    NULL,
    [nr_documento]                    VARCHAR (19)    NULL,
    [vr_documento]                    DECIMAL (18, 2) NULL,
    [cd_conta]                        VARCHAR (3)     NULL,
    [nr_banco]                        VARCHAR (30)    NULL,
    [nr_agencia]                      VARCHAR (10)    NULL,
    [nr_conta]                        VARCHAR (15)    NULL,
    [cd_despesa]                      VARCHAR (2)     NULL,
    [dt_vencimento]                   DATE            NULL,
    [ds_referencia]                   VARCHAR (60)    NULL,
    [ds_despesa_credor]               VARCHAR (50)    NULL,
    [nr_contrato]                     VARCHAR (9)     NULL,
    [ds_credor1]                      VARCHAR (70)    NULL,
    [ds_credor2]                      VARCHAR (70)    NULL,
    [ds_endereco]                     VARCHAR (40)    NULL,
    [nr_cep]                          VARCHAR (9)     NULL,
    [nr_banco_credor]                 VARCHAR (30)    NULL,
    [nr_agencia_credor]               VARCHAR (10)    NULL,
    [nr_conta_credor]                 VARCHAR (15)    NULL,
    [nr_banco_pgto]                   VARCHAR (30)    NULL,
    [nr_agencia_pgto]                 VARCHAR (10)    NULL,
    [nr_conta_pgto]                   VARCHAR (15)    NULL,
    [dt_emissao]                      DATETIME        NULL,
    [qt_op_preparada]                 INT             NULL,
    [vr_total]                        DECIMAL (18, 2) NULL,
    [bl_transmitir_prodesp]           BIT             NULL,
    [bl_transmitido_prodesp]          BIT             NULL,
    [ds_status_prodesp]               CHAR (1)        NULL,
    [ds_transmissao_mensagem_prodesp] VARCHAR (256)   NULL,
    [dt_transmitido_prodesp]          DATE            NULL,
    [ds_status_documento]             BIT             NULL,
    [bl_cadastro_completo]            BIT             NULL,
    [dt_cadastro]                     DATE            NULL,
    CONSTRAINT [PK_tb_preparacao_pagamento] PRIMARY KEY CLUSTERED ([id_preparacao_pagamento] ASC),
    CONSTRAINT [FK_tb_preparacao_pagamento_tb_regional] FOREIGN KEY ([id_regional]) REFERENCES [seguranca].[tb_regional] ([id_regional]),
    CONSTRAINT [FK_tb_preparacao_pagamento_tb_tipo_desdobramento] FOREIGN KEY ([id_tipo_preparacao_pagamento]) REFERENCES [contaunica].[tb_tipo_desdobramento] ([id_tipo_desdobramento]),
    CONSTRAINT [FK_tb_preparacao_pagamento_tb_tipo_documento] FOREIGN KEY ([id_tipo_documento]) REFERENCES [contaunica].[tb_tipo_documento] ([id_tipo_documento])
);


