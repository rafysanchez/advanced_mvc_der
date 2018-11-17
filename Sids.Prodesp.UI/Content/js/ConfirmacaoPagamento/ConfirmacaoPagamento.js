var obj = "ConfirmacaoPagamento";

var controleCheck = '.chkitens:checked';

$(document).on('ready', function () {
    $('#lstTipoDocumento').change(function () {
        filterTipoDocumento();
    });
    function filterTipoDocumento() {
        $('#NumeroDocumento').removeClass("Subempenho");
        $('#NumeroDocumento').removeClass("RapRequisicao");

        if ($("#lstTipoDocumento").val() === "11") {
            $('#NumeroDocumento').addClass("RapRequisicao");
        } else if ($("#lstTipoDocumento").val() === "5") {
            $('#NumeroDocumento').addClass("Subempenho");
        }
    }
});

function ValidaFormulario() {

    var online = navigator.onLine;

    if (online !== true) {
        AbrirModal("Erro de conexão");
        return false;
    }
    ShowLoading();
    var cont = 0;
    $("#form_filtro input[type!='hidden']").each(function () {
        if ($(this).val() !== "") {
            cont++;
        }
    });

    $("#form_filtro select").each(function () {
        if ($(this).val() !== "") {
            cont++;
        }
    });

    if (cont === 0) {
        HideLoading();
        AbrirModal("Informe ao menos um campo para filtro!");
        return false;
    } else {
        var msg = "";
        var data1;
        var data2;

        var dtInicial = $("#dataCadastroDe").val();
        var dtFinal = $("#dataCadastroAte").val();

        if ($("#dataCadastroDe").length > 0) {
            var dia;
            var mes;
            var ano;
            if (dtInicial.length > 0 && isValidDate(dtInicial) === false) {
                msg = "A data inicial é invalida!";
            } else {

                dia = dtInicial.substr(0, 2);
                mes = dtInicial.substr(3, 2);
                ano = dtInicial.substr(6, 4);

                data1 = new Date(mes + "/" + dia + "/" + ano);
            }

            if (dtFinal.length > 0 && isValidDate(dtFinal) === false) {
                msg = "A data final é invalida!";
            } else {
                dia = dtFinal.substr(0, 2);
                mes = dtFinal.substr(3, 2);
                ano = dtFinal.substr(6, 4);

                data2 = new Date(mes + "/" + dia + "/" + ano);
            }

            if (data1 && data2 && isValidDate(dtFinal) && isValidDate(dtInicial))
                if (data1.getTime() > data2.getTime() && dtFinal.length > 0) {
                    msg = "A data De deve ser menor ou igual a data Até!";
                }
        }

        if ($("#dataCadastroAte").length > 0) {
            if (dtFinal.length > 0 && isValidDate(dtFinal) === false) {
                msg = "A data final é invalida!";
            }
        }

        if (msg !== "") {
            HideLoading();
            AbrirModal(msg);
            return false;
        }
    }

    $("#form_filtro").submit();
}

function Alterar(id) {
    AbrirDetalhe('@Url.Action("Alterar")' + '?id=' + id);
}

function Visualizar(id) {
    AbrirVisualizar('@Url.Action("Visualizar")' + '?id=' + id);
}

function Excluir(id) {
    AbrirExcluir('@Url.Action("Excluir")' + '?id=' + id);
}

function ShowExt() {
    var qtdeItensCheck = $(controleCheck).length;
    var conteudoRadio = obterValoresPorControle(controleCheck);

    $('#selecionados').val(conteudoRadio);
    alert(conteudoRadio);

    $(controleCheck).each(function () {
        //$(this).attr('disabled', 'disabled');
        if ($(this).attr('data-id') === conteudoRadio) {
            //alert($(this).attr('data-id'));
            //$(this).attr('disabled', 'disabled');
            return;
        }
    })
}

function obterValoresPorControle(nomeControle) {
    var controlArray = [];

    $(nomeControle).each(function () {
        controlArray.push($(this).attr('data-id'));
    });

    var selecionado;
    selecionado = controlArray.join(',');

    if (selecionado.length > 0) {
        $(controleCheck).enabled = false;

        return selecionado;
    }
    else {
        return '';
    }
}

var CONFIRMACAOPAGAMENTO = (function () {
    var Grid = {};
    Grid.RegistroSelecionado = {};

    function init() {
        console.log("memeh");
        Grid.Bind();

        $('#tblPesquisa').DataTable().order(8, 'asc').draw();
    }


    //Grid.ItemPorLinha = function (linha) {
    //    return {
    //        Id: linha.find('input[name*="Id"]').val()
    //    };
    //};

    Grid.Bind = function () {
        Grid.BindEditar();
        Grid.BindExcluir();
        Grid.BindVisualizar();

        $('[data-toggle="popover"]').popover();
        $('[data-toggle="tooltip"]').tooltip();
    };

    Grid.BindEditar = function () {

        $('body').off('click', 'button[data-button="Editar"]:not(.disabled)');
        $('body').on('click', 'button[data-button="Editar"]:not(.disabled)', function (e) {
            e.stopPropagation();

            var id = $(this).data("id-mov");

            if (navigator.onLine !== true) {
                AbrirModal("Erro de conexão"); 
                return false;
            }

            window.location.href = '/PagamentoContaDer/ConfirmacaoPagamento/Edit/' + id + '?tipo=a';

            return false;
        });
    }

    Grid.BindExcluir = function () {
        $('body').off('click', 'button[data-button="Excluir"]:not(.disabled)');
        $('body').on('click', 'button[data-button="Excluir"]:not(.disabled)', function (e) {
            e.stopPropagation();
            //var linha = $(this).parent().parent();
            //Grid.RegistroSelecionado = Grid.ItemPorLinha(linha);
            var id = $(this).data("id-mov");

            if (navigator.onLine !== true) {
                AbrirModal("Erro de conexão");
                return false;
            }

            $.confirm({
                text: "Deseja excluir todo o agrupamento da Confirmação de Pagamento?",
                title: "Confirmação",
                confirm: function () {
                    $.ajax({
                        type: "POST",
                        url: '/PagamentoContaDer/ConfirmacaoPagamento/Delete',
                        data: {
                            'id': id
                        },
                        content: 'application/x-www-form-urlencoded; charset=UTF-8',
                        beforeSend: function (hrx) {
                            waitingDialog.show('Excluindo...');
                        }
                    }).done(function (xhrResponse) {
                        console.log("done!");
                        $.confirm({
                            text: "Confirmação de Pagamento excluída com sucesso",
                            title: "Confirmação",
                            cancel: function () {
                                ShowLoading();
                                $("#form_filtro").submit();
                            },
                            cancelButton: "Ok",
                            confirmButton: "",
                            post: true,
                            cancelButtonClass: "btn-default",
                            modalOptionsBackdrop: true
                        });
                    }).fail(function (jqXHR, textStatus) {
                        AbrirModal(jqXHR.statusText, function () { });
                    }).always(function () {
                        waitingDialog.hide();
                    });
                },
                cancel: function () {
                },
                cancelButton: "Não",
                confirmButton: "Sim",
                post: true,
                confirmButtonClass: "btn-danger",
                cancelButtonClass: "btn-default",
                dialogClass: "modal-dialog modal-sm",
                modalOptionsBackdrop: true
            });

            return false;
        });
    }

    Grid.BindVisualizar = function () {
        $('body').off('click', 'button[data-button="Visualizar"]:not(.disabled)');
        $('body').on('click', 'button[data-button="Visualizar"]:not(.disabled)', function (e) {
            e.stopPropagation();

            var id = $(this).data("id-mov");

            if (navigator.onLine !== true) {
                AbrirModal("Erro de conexão");
                return false;
            }

            window.location.href = '/PagamentoContaDer/ConfirmacaoPagamento/Edit/' + id + '?tipo=c';

            return false;
        });
    }





    return {
        Init: init
    }
}());

$(function () {
    CONFIRMACAOPAGAMENTO.Init();
})