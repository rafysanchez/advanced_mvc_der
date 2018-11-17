﻿function CarregarAplicacoes(sender) {
    $('#ddlAplicacao').html("").append($('<option value>--Selecione--</option>'));
    if (sender.value == "")
        return;

    $.ajax({
        url: "/Log/CarregarAplicacoes",
        data: { id_empresa: sender.value },
        method: "POST",
        success: function (result) {

            $.each(result, function (i, field) {
                $('#ddlAplicacao').append($('<option>', {
                    value: field.Codigo,
                    text: field.Descricao
                }));
            });
        }

    });
}

function CarregarUsuarios(sender) {
    $.ajax({
        url: "/Log/CarregarUsuarios",
        data: { id_empresa: sender.value == "" ? null : sender.value },
        method: "POST",
        success: function (result) {
            $('#ddlUsuario').html("").append($('<option value>Selecione</option>'));
            $.each(result, function (i, field) {
                if (field.Nome == null)
                    $('#ddlUsuario').append($('<option>', {
                        value: field.Codigo,
                        text: field.ChaveDeAcesso
                    }));
                else
                    $('#ddlUsuario').append($('<option>', {
                        value: field.Codigo,
                        text: field.Nome
                    }));
            });
        }

    });
}

function CarregarRecursos(sender) {
    $.ajax({
        url: "/Log/CarregarRecursos",
        data: { id_aplicacao: sender.value },
        method: "POST",
        success: function (result) {
            $('#ddlRecurso').html("").append($('<option value>--Selecione--</option>'));
            $.each(result, function (i, field) {
                $('#ddlRecurso').append($('<option>', {
                    value: field.Codigo,
                    text: field.Descricao
                }));
            });
        }

    });
}

function AbrirDetalhe(id) {
    jQuery.ajax({
        url: "/Log/Detalhe",
        data: { id_log: id },
        method: "POST",
        success: function () {
        }
    });
}

