﻿CREATE TABLE [pagamento].[tb_rap_anulacao] (
    [id_rap_anulacao]                             INT           IDENTITY (1, 1) NOT NULL,
    [tb_estrutura_id_estrutura]                   INT           NULL,
    [tb_servico_tipo_id_servico_tipo]             INT           NULL,
    [tb_programa_id_programa]                     INT           NULL,
    [tb_regional_id_regional]                     SMALLINT      NULL,
    [nr_siafem_siafisico]                         VARCHAR (11)  NULL,
    [nr_prodesp]                                  VARCHAR (19)  NULL,
    [nr_prodesp_original]                         VARCHAR (15)  NULL,
    [nr_empenho_siafem_siafisico]                 VARCHAR (11)  NULL,
    [nr_contrato]                                 VARCHAR (13)  NULL,
    [nr_cnpj_cpf_credor]                          VARCHAR (15)  NULL,
    [nr_despesa_processo]                         VARCHAR (60)  NULL,
    [nr_recibo]                                   VARCHAR (9)   NULL,
    [nr_requisicao_rap]                           VARCHAR (17)  NULL,
    [cd_unidade_gestora]                          VARCHAR (6)   NULL,
    [cd_unidade_gestora_obra]                     VARCHAR (6)   NULL,
    [cd_gestao_credor]                            VARCHAR (140) NULL,
    [cd_gestao]                                   VARCHAR (5)   NULL,
    [cd_aplicacao_obra]                           VARCHAR (8)   NULL,
    [nr_medicao]                                  VARCHAR (3)   NULL,
    [vl_valor]                                    INT           NULL,
    [vl_anulado]                                  VARCHAR (20)  NULL,
    [cd_nota_fiscal_prodesp]                      VARCHAR (6)   NULL,
    [cd_tarefa]                                   VARCHAR (2)   NULL,
    [nr_classificacao]                            VARCHAR (9)   NULL,
    [nr_ano_medicao]                              CHAR (4)      NULL,
    [nr_mes_medicao]                              CHAR (2)      NULL,
    [ds_prazo_pagamento]                          VARCHAR (3)   NULL,
    [dt_realizado]                                DATE          NULL,
    [ds_despesa_autorizado_supra_folha]           CHAR (4)      NULL,
    [ds_observacao_1]                             VARCHAR (76)  NULL,
    [ds_observacao_2]                             VARCHAR (76)  NULL,
    [ds_observacao_3]                             VARCHAR (76)  NULL,
    [ds_despesa_referencia]                       VARCHAR (60)  NULL,
    [cd_despesa]                                  VARCHAR (2)   NULL,
    [cd_despesa_especificacao_despesa]            CHAR (3)      NULL,
    [ds_despesa_especificacao_1]                  VARCHAR (79)  NULL,
    [ds_despesa_especificacao_2]                  VARCHAR (79)  NULL,
    [ds_despesa_especificacao_3]                  VARCHAR (79)  NULL,
    [ds_despesa_especificacao_4]                  VARCHAR (79)  NULL,
    [ds_despesa_especificacao_5]                  VARCHAR (79)  NULL,
    [ds_despesa_especificacao_6]                  VARCHAR (79)  NULL,
    [ds_despesa_especificacao_7]                  VARCHAR (79)  NULL,
    [ds_despesa_especificacao_8]                  VARCHAR (79)  NULL,
    [cd_assinatura_autorizado]                    VARCHAR (5)   NULL,
    [cd_assinatura_autorizado_grupo]              INT           NULL,
    [cd_assinatura_autorizado_orgao]              CHAR (2)      NULL,
    [ds_assinatura_autorizado_cargo]              VARCHAR (55)  NULL,
    [nm_assinatura_autorizado]                    VARCHAR (55)  NULL,
    [cd_assinatura_examinado]                     VARCHAR (5)   NULL,
    [cd_assinatura_examinado_grupo]               INT           NULL,
    [cd_assinatura_examinado_orgao]               CHAR (2)      NULL,
    [ds_assinatura_examinado_cargo]               VARCHAR (55)  NULL,
    [nm_assinatura_examinado]                     VARCHAR (55)  NULL,
    [cd_assinatura_responsavel]                   VARCHAR (5)   NULL,
    [cd_assinatura_responsavel_grupo]             INT           NULL,
    [cd_assinatura_responsavel_orgao]             CHAR (2)      NULL,
    [ds_assinatura_responsavel_cargo]             VARCHAR (55)  NULL,
    [nm_assinatura_responsavel]                   VARCHAR (55)  NULL,
    [vl_saldo_anterior_subempenho]                VARCHAR (10)  NULL,
    [vl_saldo_apos_anulacao]                      VARCHAR (20)  NULL,
    [cd_transmissao_status_prodesp]               CHAR (1)      NULL,
    [fl_transmissao_transmitido_prodesp]          BIT           NULL,
    [dt_transmissao_transmitido_prodesp]          DATE          NULL,
    [ds_transmissao_mensagem_prodesp]             VARCHAR (140) NULL,
    [cd_transmissao_status_siafem_siafisico]      CHAR (1)      NULL,
    [fl_transmissao_transmitido_siafem_siafisico] BIT           NULL,
    [dt_transmissao_transmitido_siafem_siafisico] DATE          NULL,
    [dt_cadastro]                                 DATE          NULL,
    [ds_transmissao_mensagem_siafem_siafisico]    VARCHAR (140) NULL,
    [fl_documento_completo]                       BIT           NULL,
    [fl_documento_status]                         BIT           NULL,
    [fl_sistema_siafisico]                        BIT           NULL,
    [cd_transmissao_status_siafisico]             CHAR (1)      NULL,
    [fl_transmissao_transmitido_siafisico]        BIT           NULL,
    [cd_cenario_prodesp]                          VARCHAR (140) NULL,
    [dt_emissao]                                  DATE          NULL,
    [fl_sistema_prodesp]                          BIT           NULL,
    [fl_sistema_siafem_siafisico]                 BIT           NULL,
    CONSTRAINT [PK_tb_rap_anulacao] PRIMARY KEY CLUSTERED ([id_rap_anulacao] ASC),
    CONSTRAINT [FK_tb_rap_anulacao_tb_estrutura] FOREIGN KEY ([tb_estrutura_id_estrutura]) REFERENCES [configuracao].[tb_estrutura] ([id_estrutura]),
    CONSTRAINT [FK_tb_rap_anulacao_tb_programa] FOREIGN KEY ([tb_programa_id_programa]) REFERENCES [configuracao].[tb_programa] ([id_programa]),
    CONSTRAINT [FK_tb_rap_anulacao_tb_regional] FOREIGN KEY ([tb_regional_id_regional]) REFERENCES [seguranca].[tb_regional] ([id_regional]),
    CONSTRAINT [FK_tb_rap_anulacao_tb_servico_tipo] FOREIGN KEY ([tb_servico_tipo_id_servico_tipo]) REFERENCES [pagamento].[tb_servico_tipo] ([id_servico_tipo])
);


