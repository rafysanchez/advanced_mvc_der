﻿CREATE TABLE [empenho].[tb_empenho_cancelamento] (
    [id_empenho_cancelamento]                                   INT           IDENTITY (1, 1) NOT NULL,
    [tb_empenho_cancelamento_tipo_id_empenho_cancelamento_tipo] INT           NULL,
    [tb_regional_id_regional]                                   SMALLINT      NULL,
    [tb_programa_id_programa]                                   INT           NULL,
    [tb_estrutura_id_estrutura]                                 INT           NULL,
    [tb_fonte_id_fonte]                                         INT           NULL,
    [tb_licitacao_id_licitacao]                                 VARCHAR (2)   NULL,
    [tb_modalidade_id_modalidade]                               INT           NULL,
    [tb_aquisicao_tipo_id_aquisicao_tipo]                       INT           NULL,
    [tb_origem_material_id_origem_material]                     INT           NULL,
    [tb_destino_cd_destino]                                     CHAR (2)      NULL,
    [cd_empenho]                                                VARCHAR (11)  NULL,
    [nr_ano_exercicio]                                          INT           NULL,
    [nr_processo]                                               VARCHAR (58)  NULL,
    [nr_processo_ne]                                            VARCHAR (15)  NULL,
    [nr_processo_siafisico]                                     VARCHAR (11)  NULL,
    [nr_contrato]                                               VARCHAR (13)  NULL,
    [nr_ct]                                                     VARCHAR (11)  NULL,
    [nr_empenhoProdesp]                                         VARCHAR (13)  NULL,
    [nr_empenhoSiafem]                                          VARCHAR (11)  NULL,
    [nr_empenhoSiafisico]                                       VARCHAR (11)  NULL,
    [cd_aplicacao_obra]                                         VARCHAR (8)   NULL,
    [nr_natureza_item]                                          VARCHAR (2)   NULL,
    [dt_cadastramento]                                          DATE          NULL,
    [nr_cnpj_cpf_ug_credor]                                     VARCHAR (15)  NULL,
    [cd_gestao_credor]                                          VARCHAR (6)   NULL,
    [cd_credor_organizacao]                                     INT           NULL,
    [nr_cnpj_cpf_fornecedor]                                    VARCHAR (15)  NULL,
    [cd_unidade_gestora]                                        VARCHAR (6)   NULL,
    [cd_gestao]                                                 VARCHAR (5)   NULL,
    [cd_evento]                                                 INT           NULL,
    [dt_emissao]                                                DATE          NULL,
    [cd_unidade_gestora_fornecedora]                            VARCHAR (6)   NULL,
    [cd_gestao_fornecedora]                                     VARCHAR (5)   NULL,
    [cd_uo]                                                     INT           NULL,
    [cd_unidade_fornecimento]                                   VARCHAR (5)   NULL,
    [ds_autorizado_supra_folha]                                 VARCHAR (4)   NULL,
    [cd_especificacao_despesa]                                  VARCHAR (3)   NULL,
    [ds_especificacao_despesa]                                  VARCHAR (711) NULL,
    [cd_autorizado_assinatura]                                  VARCHAR (5)   NULL,
    [cd_autorizado_grupo]                                       INT           NULL,
    [cd_autorizado_orgao]                                       CHAR (2)      NULL,
    [nm_autorizado_assinatura]                                  VARCHAR (55)  NULL,
    [ds_autorizado_cargo]                                       VARCHAR (55)  NULL,
    [cd_examinado_assinatura]                                   VARCHAR (5)   NULL,
    [cd_examinado_grupo]                                        INT           NULL,
    [cd_examinado_orgao]                                        CHAR (2)      NULL,
    [nm_examinado_assinatura]                                   VARCHAR (55)  NULL,
    [ds_examinado_cargo]                                        VARCHAR (55)  NULL,
    [cd_responsavel_assinatura]                                 VARCHAR (5)   NULL,
    [cd_responsavel_grupo]                                      INT           NULL,
    [cd_responsavel_orgao]                                      CHAR (2)      NULL,
    [nm_responsavel_assinatura]                                 VARCHAR (55)  NULL,
    [ds_responsavel_cargo]                                      VARCHAR (140) NULL,
    [bl_transmitir_prodesp]                                     BIT           NULL,
    [fg_transmitido_prodesp]                                    BIT           NULL,
    [dt_transmitido_prodesp]                                    DATE          NULL,
    [bl_transmitir_siafem]                                      BIT           NULL,
    [fg_transmitido_siafem]                                     BIT           NULL,
    [dt_transmitido_siafem]                                     DATE          NULL,
    [bl_transmitir_siafisico]                                   BIT           NULL,
    [fg_transmitido_siafisico]                                  BIT           NULL,
    [dt_transmitido_siafisico]                                  DATE          NULL,
    [ds_status_prodesp]                                         VARCHAR (1)   NULL,
    [ds_status_siafem]                                          VARCHAR (1)   NULL,
    [ds_status_siafisico]                                       VARCHAR (1)   NULL,
    [ds_status_documento]                                       BIT           NULL,
    [bl_cadastro_completo]                                      BIT           NULL,
    [ds_msgRetornoTransmissaoProdesp]                           VARCHAR (140) NULL,
    [ds_msgRetornoTransmissaoSiafem]                            VARCHAR (140) NULL,
    [ds_msgRetornoTransmissaoSiafisico]                         VARCHAR (140) NULL,
    [nr_natureza_ne]                                            VARCHAR (12)  NULL,
    [cd_fonte_siafisico]                                        VARCHAR (30)  NULL,
    [cd_empenho_original]                                       VARCHAR (11)  NULL,
    [cd_municipio]                                              VARCHAR (5)   NULL,
    [ds_acordo]                                                 VARCHAR (2)   NULL,
    [ds_status_siafisico_ct]                                    CHAR (1)      NULL,
    [ds_local_entrega_siafem]                                   VARCHAR (45)  NULL,
    [cd_reserva]                                                VARCHAR (11)  NULL,
    [nr_ct_original]                                            VARCHAR (11)  NULL,
    CONSTRAINT [PK_tb_empenho_cancelamento] PRIMARY KEY CLUSTERED ([id_empenho_cancelamento] ASC),
    CONSTRAINT [FK_tb_empenho_cancelamento_tb_aquisicao_tipo] FOREIGN KEY ([tb_aquisicao_tipo_id_aquisicao_tipo]) REFERENCES [empenho].[tb_aquisicao_tipo] ([id_aquisicao_tipo]),
    CONSTRAINT [FK_tb_empenho_cancelamento_tb_empenho_cancelamento_tipo] FOREIGN KEY ([tb_empenho_cancelamento_tipo_id_empenho_cancelamento_tipo]) REFERENCES [empenho].[tb_empenho_cancelamento_tipo] ([id_empenho_cancelamento_tipo]),
    CONSTRAINT [FK_tb_empenho_cancelamento_tb_fonte] FOREIGN KEY ([tb_fonte_id_fonte]) REFERENCES [configuracao].[tb_fonte] ([id_fonte]),
    CONSTRAINT [FK_tb_empenho_cancelamento_tb_licitacao] FOREIGN KEY ([tb_licitacao_id_licitacao]) REFERENCES [empenho].[tb_licitacao] ([id_licitacao]),
    CONSTRAINT [FK_tb_empenho_cancelamento_tb_modalidade] FOREIGN KEY ([tb_modalidade_id_modalidade]) REFERENCES [empenho].[tb_modalidade] ([id_modalidade]),
    CONSTRAINT [FK_tb_empenho_cancelamento_tb_origem_material] FOREIGN KEY ([tb_origem_material_id_origem_material]) REFERENCES [empenho].[tb_origem_material] ([id_origem_material]),
    CONSTRAINT [FK_tb_empenho_cancelamento_tb_regional] FOREIGN KEY ([tb_regional_id_regional]) REFERENCES [seguranca].[tb_regional] ([id_regional])
);


