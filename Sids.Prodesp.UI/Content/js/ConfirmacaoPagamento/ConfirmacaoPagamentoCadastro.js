var CONFIRMACAOPAGAMENTOCADASTRO = (function () {

    function init() {
        oldCode();
        bindAll();

        $('select[name=opcoesConfirmacao]').change();


        IniciarCreateEdit(window.controller);
    }

    function bindAll() {
        bindBtnConsultarPagamentosConfirmar();
        bindTipoDocumento();
    }



    function bindBtnConsultarPagamentosConfirmar() {

        $('#divPainelCadastrarConfirmacaoPagamento').on('click', '#btnConsultarPagamentosConfirmar', function () {
            listar();
        });
    }

    function bindTipoDocumento() {
        $('#divFiltroPagamentosConfirmar').on('change', '#IdTipoDocumento', function () {
            var opcao = $(this).val();
            if (opcao == '5') {
                $('#NumeroDocumento').removeClass('maskRap').addClass('maskSubempenho');
            }
            else if (opcao == '11') {
                $('#NumeroDocumento').removeClass('maskSubempenho').addClass('maskRap');
            }
            else {
                $('#NumeroDocumento').removeClass('maskSubempenho').removeClass('maskRap');
            }
        })
    }

    function oldCode() {
        $('#select-all_Autorizacao').click(function (event) {
            if (this.checked) {
                // Iterate each checkbox
                $('.checkTodosAutorizacao').each(function () {
                    this.checked = true;
                });
            }
            else {
                $('.checkTodosAutorizacao').each(function () {
                    this.checked = false;
                });
            }
        });

        $(".checkTodosAutorizacao").on("change", function () {
            if (!$(this).is(":checked")) {
                $("#select-all_Autorizacao").prop('checked', false);
            }
            if ($(".checkTodosAutorizacao:checked").length == $(".checkTodosAutorizacao").length) {
                $("#select-all_Autorizacao").prop('checked', true);
            }
        });

        $("input:checkbox").on('click', function () {
            var $box = $(this);
            if ($box.is(":checked")) {
                var group = "input:checkbox[name='" + $box.attr("name") + "']";
                $(group).prop("checked", false);
                $box.prop("checked", true);
            } else {
                $box.prop("checked", false);
            }
        });

        $('.botao').click(function (e) {
            var isValid = true;
            $('select[required],input[required=required]').each(function () {
                if ($.trim($(this).val()) == '') {
                    isValid = false;
                    $('#modalAlerta').modal();
                    $('#modalAlerta #value').html('Campo obrigatório não preenchido');
                    $(this).addClass('campoVazio');
                }
                else {
                    $(this).removeClass('campoVazio');
                }
            });
            if (isValid == false) {
                e.preventDefault();
            }

            else {
                $('#modalAlerta').modal();
                $('#modalAlerta #value').html('Transmissão realizada com sucesso');
            }
        });

        $('select[name=opcoesConfirmacao]').change(function () {
            var opcao = $(this).val();
            if (opcao == '1') {
                $('.pagamentoPorLoteC').hide();
                $('.pagamentoPorDocumentoC').show();
            }

            else if (opcao == '2') {
                $('.pagamentoPorDocumentoC').hide();
                $('.pagamentoPorLoteC').show();
            }
            else {
                $('.pagamentoPorLoteC').hide();
                $('.pagamentoPorDocumentoC').hide();
            }

            $('#divFiltroPagamentosConfirmar').show();
        })

        //botão transmitir Impressao de Relação de RE e RT
        $('#btnTransmitirImpressao').click(function (e) {
            var isValid = true;
            $('input.required').each(function () {
                if ($.trim($(this).val()) == '') {
                    isValid = false;
                    $('#modalAlerta').modal();
                    $('#modalAlerta #value').html('Campo obrigatório não preenchido');
                    $(this).addClass('campoVazio');
                }
                else {
                    $(this).removeClass('campoVazio');
                }
            });
            if (isValid == false) {
                e.preventDefault();
            }

            else {
                MostraModal('#modalTransmissaoImpressao');
            }
        });
    }

    function mostrarModal(id) {
        $(id).modal();
        return false;
    }

    function fecharModal(id) {
        $(id).modal('hide');
        return false;
    }

    return {
        Init: init,
        Modal: {
            Mostrar: mostrarModal,
            Fechar: fecharModal
        }
    }
}());

$(function () {
    CONFIRMACAOPAGAMENTOCADASTRO.Init();
})

$('button[data-button="btnSalvarConfirmacao"]:not(.disabled)').on('click', function () {
    var rows = $('#tblPesquisa tbody tr').length;
    var tds = $('#tblPesquisa tbody tr td').length;

    if (rows > 0 && tds > 1) {
        $('#divResultadosBusca').html('');
        salvar();
    }
    else {
        $.confirm({
            text: "É preciso ter ao menos um documento para poder salvar.",
            title: "Sem pagamentos a confirmar",
            confirm: function () {
            },
            cancelButton: false,
            confirmButton: "OK"
        });
    }
});


$('button[data-button="btnTransmitirProdesp"]:not(.disabled)').on('click', function () {
    var rows = $('#tblPesquisa tbody tr').length;
    var tds = $('#tblPesquisa tbody tr td').length;

    var dataPreparacao = $('#DataPreparacao').val();
    var dtPrepSplit = dataPreparacao.split("/");
    var dtPreparacao = new Date(dtPrepSplit[2], parseInt(dtPrepSplit[1]) - 1, dtPrepSplit[0]);

    var dataConfirmacao = $('#DataConfirmacao').val();
    var dtConfSplit = dataConfirmacao.split("/");
    var dtConfirmacao = new Date(dtConfSplit[2], parseInt(dtConfSplit[1]) - 1, dtConfSplit[0]);

    if (rows > 0 && tds > 1) {
        if (dtPreparacao > dtConfirmacao) {
            $.confirm({
                text: "A data da confirmação é menor que a data da preparação, deseja continuar??",
                title: "Verifique as datas",
                confirm: function () {
                    preTransmitir();
                },
                cancelButton: "Não",
                confirmButton: "Sim"
            });
        }
        else {
            preTransmitir();
        }
    }
    else {
        $.confirm({
            text: "É preciso consultar antes de salvar ou transmitir",
            title: "Sem pagamentos a confirmar",
            confirm: function () {
            },
            cancelButton: false,
            confirmButton: "OK"
        });
    }
});


function listar() {
    waitingDialog.show('Consultando');

    $('#divResultadosBusca').html('');
    var data = $('.dadosBuscaPagamentosConfirmar').find(':input').serialize();
    $.ajax({
        type: 'POST',
        url: "/PagamentoContaDer/ConfirmacaoPagamento/ListarPagamentosConfirmar",
        cache: false,
        async: true,
        data: data,
        beforeSend: function (hrx) {
            waitingDialog.show('Consultando');
        }
    }).done(function (jqXHR) {
        if (jqXHR.Dados.length > 0) {
            $('#DataPreparacao').val(jqXHR.Dados[0].DataPreparacaoTexto)
        }

        $('#divResultadosBusca').html(jqXHR.Html);
    }).fail(function (jqXHR, textStatus) {
        AbrirModal(jqXHR.statusText, function () { });
    }).always(function () {
        waitingDialog.hide();
    });
}

function salvar() {
    var data = $('.dadosBuscaPagamentosConfirmar').find(':input').serialize();

    $.ajax({
        type: 'POST',
        url: "/PagamentoContaDer/ConfirmacaoPagamento/ListarPagamentosConfirmarSalvar",
        cache: false,
        async: true,
        data: data,
        beforeSend: function (hrx) {
            waitingDialog.show('Salvando');
        }
    }).done(function (response) {
        if (response.Sucesso === true) {
            $.confirm({
                text: response.Mensagem,
                title: "Sucesso!",
                confirm: function () {
                    window.location.href = '/PagamentoContaDer/ConfirmacaoPagamento/Edit/' + response.Id + '?tipo=a';
                },
                cancelButton: false,
                confirmButton: "OK"
            });
        }
        else {
            AbrirModal(response.Mensagem, function () {
                window.location.href = '/PagamentoContaDer/ConfirmacaoPagamento/Edit/' + response.Id + '?tipo=a';
            });
        }
    }).fail(function (jqXHR, textStatus) {
        AbrirModal(jqXHR.statusText, function () { });
    }).always(function () {
        waitingDialog.hide();
    });
}

function preTransmitir() {
    var transmitirSiafem = $('#transmitirSiafem').is(':checked');
    if (transmitirSiafem === false) {
        transmitir();
    }
    else {
        $.confirm({
            text: "Deseja transmitir dados de NL?",
            title: "Transmitir",
            cancel: function () {
                $('#transmitirNls').val("False");
                transmitir();
            },
            confirm: function () {
                $('#transmitirNls').val("True");
                transmitir();
            },
            cancelButton: "Não",
            confirmButton: "Sim"
        });
    }
}

function transmitir() {
    waitingDialog.show('Transmitindo');

    $('#divResultadosBusca').html('');
    var data = $('.dadosBuscaPagamentosConfirmar').find(':input').serialize();
    $.ajax({
        type: 'POST',
        url: "/PagamentoContaDer/ConfirmacaoPagamento/ListarPagamentosTransmitir",
        cache: false,
        async: true,
        data: data,
        beforeSend: function (hrx) {
            waitingDialog.show('Transmitindo');
        }
    }).done(function (response) {
        if (response.Sucesso === true) {
            $.confirm({
                text: response.Mensagem,
                title: "Sucesso!",
                confirm: function () {
                    window.location.href = '/PagamentoContaDer/ConfirmacaoPagamento/Edit/' + response.Id + '?tipo=a';
                },
                cancelButton: false,
                confirmButton: "OK"
            });
        }
        else {
            AbrirModal(response.Mensagem, function () {
                window.location.href = '/PagamentoContaDer/ConfirmacaoPagamento/Edit/' + response.Id + '?tipo=a';
            });
        }
    }).fail(function (jqXHR, textStatus) {
        AbrirModal(jqXHR.statusText, function () { });
    }).always(function () {
        waitingDialog.hide();
    });
}

