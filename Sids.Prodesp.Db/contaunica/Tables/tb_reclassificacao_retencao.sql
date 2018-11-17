﻿CREATE TABLE [contaunica].[tb_reclassificacao_retencao] (
    [id_reclassificacao_retencao]                 INT           IDENTITY (1, 1) NOT NULL,
    [id_resto_pagar]                              CHAR (1)      NULL,
    [id_tipo_reclassificacao_retencao]            INT           NULL,
    [id_tipo_documento]                           INT           NULL,
    [dt_cadastro]                                 DATE          NULL,
    [nr_siafem_siafisico]                         VARCHAR (11)  NULL,
    [nr_contrato]                                 VARCHAR (13)  NULL,
    [nr_processo]                                 VARCHAR (15)  NULL,
    [nr_empenho_siafem_siafisico]                 VARCHAR (11)  NULL,
    [nr_documento]                                VARCHAR (19)  NULL,
    [cd_unidade_gestora]                          VARCHAR (6)   NULL,
    [cd_gestao]                                   VARCHAR (5)   NULL,
    [nr_medicao]                                  VARCHAR (3)   NULL,
    [vl_valor]                                    DECIMAL (18)  NULL,
    [cd_evento]                                   VARCHAR (6)   NULL,
    [ds_inscricao]                                VARCHAR (22)  NULL,
    [cd_classificacao]                            VARCHAR (9)   NULL,
    [cd_fonte]                                    VARCHAR (10)  NULL,
    [dt_emissao]                                  DATE          NULL,
    [nr_cnpj_cpf_credor]                          VARCHAR (15)  NULL,
    [cd_gestao_credor]                            VARCHAR (140) NULL,
    [nr_ano_medicao]                              CHAR (4)      NULL,
    [nr_mes_medicao]                              CHAR (2)      NULL,
    [id_regional]                                 SMALLINT      NULL,
    [cd_aplicacao_obra]                           VARCHAR (140) NULL,
    [tb_obra_tipo_id_obra_tipo]                   INT           NULL,
    [cd_credor_organizacao]                       INT           NULL,
    [nr_cnpj_cpf_fornecedor]                      VARCHAR (15)  NULL,
    [ds_normal_estorno]                           CHAR (1)      NULL,
    [nr_nota_lancamento_medicao]                  VARCHAR (11)  NULL,
    [nr_cnpj_prefeitura]                          VARCHAR (15)  NULL,
    [ds_observacao_1]                             VARCHAR (76)  NULL,
    [ds_observacao_2]                             VARCHAR (76)  NULL,
    [ds_observacao_3]                             VARCHAR (76)  NULL,
    [fl_sistema_siafem_siafisico]                 BIT           NULL,
    [cd_transmissao_status_siafem_siafisico]      CHAR (1)      CONSTRAINT [DF_tb_reclassificacao_retencao_cd_transmissao_status_siafem_siafisico] DEFAULT ('N') NULL,
    [fl_transmissao_transmitido_siafem_siafisico] BIT           NULL,
    [dt_transmissao_transmitido_siafem_siafisico] DATE          NULL,
    [ds_transmissao_mensagem_siafem_siafisico]    VARCHAR (140) NULL,
    [bl_cadastro_completo]                        BIT           NULL,
    [nr_ano_exercicio]                            INT           NULL,
    [cd_origem]                                   INT           NULL,
    [cd_agrupamento_confirmacao]                  INT           NULL,
    [id_confirmacao_pagamento]                    INT           NULL,
    [ds_TipoNL]                                   VARCHAR (100) NULL,
    CONSTRAINT [PK_tb_reclassificacao_retencao] PRIMARY KEY CLUSTERED ([id_reclassificacao_retencao] ASC),
    CONSTRAINT [CK_tb_reclassificacao_retencao_cd_transmissao_status_siafem_siafisico] CHECK ([cd_transmissao_status_siafem_siafisico]='N' OR [cd_transmissao_status_siafem_siafisico]='E' OR [cd_transmissao_status_siafem_siafisico]='S'),
    CONSTRAINT [FK_tb_reclassificacao_retencao_tb_confirmacao_pagamento] FOREIGN KEY ([id_confirmacao_pagamento]) REFERENCES [pagamento].[tb_confirmacao_pagamento] ([id_confirmacao_pagamento]),
    CONSTRAINT [FK_tb_reclassificacao_retencao_tb_para_resto_pagar] FOREIGN KEY ([id_resto_pagar]) REFERENCES [contaunica].[tb_para_resto_pagar] ([id_resto_pagar]),
    CONSTRAINT [FK_tb_reclassificacao_retencao_tb_regional] FOREIGN KEY ([id_regional]) REFERENCES [seguranca].[tb_regional] ([id_regional]),
    CONSTRAINT [FK_tb_reclassificacao_retencao_tb_tipo_reclassificacao_retencao] FOREIGN KEY ([id_tipo_reclassificacao_retencao]) REFERENCES [contaunica].[tb_tipo_reclassificacao_retencao] ([id_tipo_reclassificacao_retencao])
);









