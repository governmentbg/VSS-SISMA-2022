(function () {
    // DataTables global settings

    $.fn.dataTable.ext.buttons.io_excel = {
        extend: 'excel',
        text: '<i class="far fa-file-excel"></i>',
        titleAttr: 'Excel',
        className: 'btn-success',
        exportOptions: {
            "columns": "thead th:not(.noExport):visible",
            stripNewlines: false,
            stripHtml:true
        },
        title: function () {
            if (typeof (dtTitle) == 'function') {
                return dtTitle();
            }
            return document.title;
        },
        messageBottom: function () {
            if (typeof (dtMessageBottom) == 'function') {
                return dtMessageBottom();
            }
            return '';
        },
        messageTop: function () {
            if (typeof (dtMessageTop) == 'function') {
                return dtMessageTop();
            }
            return '';
        }
    };

    $.fn.dataTable.ext.buttons.io_pdf = {
        extend: 'collection',
        text: '<i class="far fa-file-pdf"></i>',
        titleAttr: 'Pdf',
        className: 'btn-danger',
        autoClose: true,
        buttons: [
            {
                extend: 'pdfHtml5',
                text: 'Портретно',
                exportOptions: {
                    "columns": "thead th:not(.noExport):visible",
                    stripNewlines: false                    
                },
                orientation: 'portrait',
                title: function () {
                    if (typeof (dtTitle) == 'function') {
                        return dtTitle();
                    }
                    return document.title;
                },
                messageBottom: function () {
                    if (typeof (dtMessageBottom) == 'function') {
                        return dtMessageBottom();
                    }
                },
                messageTop: function () {
                    if (typeof (dtMessageTop) == 'function') {
                        return dtMessageTop();
                    }
                }
            },
            {
                extend: 'pdfHtml5',
                text: 'Пейзажно',
                exportOptions: {
                    "columns": "thead th:not(.noExport):visible",
                    stripNewlines: false,
                    //stripHtml: false
                },
                orientation: 'landscape',
                title: function () {
                    if (typeof (dtTitle) == 'function') {
                        return dtTitle();
                    }
                    return document.title;
                },
                messageBottom: function () {
                    if (typeof (dtMessageBottom) == 'function') {
                        return dtMessageBottom();
                    }
                    return '';
                },
                messageTop: function () {
                    if (typeof (dtMessageTop) == 'function') {
                        return dtMessageTop();
                    }
                    return '';
                }
            },
        ]
    };

    $.fn.dataTable.ext.buttons.io_print = {
        extend: 'print',
        text: '<i class="fa fa-print"></i>',
        titleAttr: 'Печат',
        className: 'btn-primary',
        exportOptions: {
            "columns": "thead th:not(.noExport):visible"
        }
    };

    $.fn.dataTable.ext.buttons.io_colvis = {
        extend: 'colvis',
        text: '<i class="fa  fa-eye-slash"></i>',
        titleAttr: 'Видими Колони',
        className: 'btn-info'
    };

    $.extend(true, $.fn.dataTable.defaults, {
        "initComplete": function (settings, json) {
            initDataTablesSearch(settings);
        },
        dom: '<"row"<"col-md-8 dataTables_buttons"B<"custom-filter d-inline">><"col-md-4"f>>rtip',
        buttons: {
            dom: {
                button: {
                    tag: 'button',
                    className: 'btn btn-sm'
                },
                container: {
                    className: 'd-inline'
                }
            },
            buttons: ['pageLength', 'io_colvis', 'io_excel', 'io_pdf', 'io_print']
        },
        "lengthMenu": [
            [10, 25, 50, 100, -1],
            ['10 реда', '25 реда', '50 реда', '100 реда', 'Покажи всички']
        ],
        "bAutoWidth": false,
        "language": {
            "url": "/lib/adminlte/plugins/datatables/dataTables.bgBG.json"
        },
        filter: true,
        "bLengthChange": false,
        "serverSide": true,
        "processing": true,
        "paging": true,
        "pageLength": 10,
        "stateSave": true,
        "stateDuration": -1
    });

    $('.date-picker').datepicker({
        todayHighlight: true,
        autoclose: true,
        format: 'dd.mm.yyyy',
        language: 'bg-BG'
    });

    $('.datetime-picker').datetimepicker({
        format: 'DD.MM.YYYY HH:mm:SS',
        locale: 'bg-BG'
    });

    $('.date-range-picker').daterangepicker({
        locale: {
            direction: 'ltr',
            format: 'DD.MM.YYYY',
            separator: ' - ',
            applyLabel: 'Избери',
            cancelLabel: 'Отказ',
            weekLabel: 'С',
            customRangeLabel: 'Период',
            daysOfWeek: ["П", "В", "С", "Ч", "П", "С", "Н"],
            monthNames: ["Януари", "Февруари", "Март", "Април", "Май", "Юни", "Юли", "Август", "Септември", "Октомври", "Ноември", "Декември"],
            firstDay: moment.localeData().firstDayOfWeek()
        }
    });

    $('.select2').select2({
        width: '100%'
    });

    //$(".textarea").wysihtml5();
})();

function initDataTablesSearch(dtSettings) {
    // Search form events
    var initSearchForm = $('.search-form');

    //var initTable = $('.dataTable');
    var initWrapper = $(dtSettings.nTableWrapper);
    var initTable = $(dtSettings.nTable);

    if (initSearchForm.length > 0 && initTable.length > 0) {
        initSearchForm.on('submit', function () {
            var t = initTable.DataTable();
            t.state.clear();
        });
    }

    var secondCount = 0;
    var keysPressed = -1;
    var searchQuery = '';
    var timer = '';

    //var $searchInput = $('div.dataTables_filter input');
    var $searchInput = initWrapper.find('div.dataTables_filter input');
    $searchInput.unbind();
    $searchInput.bind('keyup', function (e) {
        if (this.value.length > 2 || this.value === '') {
            secondCount = 0;
        }
        keysPressed = this.value.length;
        searchQuery = this.value;
    });
    $searchInput.bind('keydown', function (e) {
        if (!timer) {
            timer = setInterval(function () {
                if (secondCount >= 1 && (keysPressed > 2 || keysPressed === 0)) {
                    keysPressed = -1;
                    SearchDataTable(searchQuery, initTable);
                    clearInterval(timer);
                    timer = '';
                } else {
                    secondCount += 1;
                }
            }, 1000);
        }
    });
}

function SearchDataTable(searchQuery, initTable) {
    if (searchQuery.length > 2 || searchQuery === '') {
        initTable.DataTable().search(searchQuery).draw();
    }
}

function initDualListBox(control, rows) {
    if (rows) {
        $(control).attr('size', rows);
    }
    let elementLabel = $(control).data('label')
    if ($(control).data('loaded')) {
        let lbDel = $(control).bootstrapDualListbox('refresh');
    }
    let lb = $(control).bootstrapDualListbox({
        moveOnSelect: false,
        preserveSelectionOnMove: true,
        selectedListLabel: 'Избрани ' + elementLabel,
        nonSelectedListLabel: 'Свободни ' + elementLabel,
        infoText: '{0} ' + elementLabel,
        infoTextEmpty: 'Няма избрани ' + elementLabel,
        infoTextFiltered: '<span class="label label-warning">Филтрирани</span> {0} от {1}',
        filterTextClear: 'покажи всички',
        filterPlaceHolder: 'Търсене',
        moveSelectedLabel: 'Добави',
        moveAllLabel: 'Добави всички',
        removeAllLabel: 'Премахни всички',
        removeSelectedLabel: 'Премахни'
    });
    $(control).data('loaded', 'loaded');
}