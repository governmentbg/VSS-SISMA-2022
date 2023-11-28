let autocompleteUrls = {
    caseCodeSearch: rootDir + 'Ajax/SearchCaseCodes?query=',
    caseCodeGet: rootDir + 'Ajax/SearchCaseCodes?id='
};


function initAutoCompleteControl(control, url, query, parentControl, paramFunc) {
    if ($(control).hasClass('ac-isloaded')) {
        return false;
    }
    if (!query) {
        query = '';
    }
    let minLength = 3;
    if ($(control).data('minlength')) {
        minLength = $(control).data('minlength');
    }
    $(control).addClass('ac-isloaded');
    $(control).autocomplete({
        minLength: minLength,
        delay: 250,
        appendTo: document.getElementById($(control).attr('id') + "list"),//'body',
        source: function source(request, response) {
            if (parentControl) {
                query += $(parentControl).val();
            }
            let params = '';
            if (paramFunc) {
                params += paramFunc();
            }
            fetch(url + encodeURIComponent(request.term) + query + params)
                .then((res) => {
                    return res.json();
                })
                .then((res) => {
                    response(res);
                });
        }
        , select: function select(event, ui) {
            let id = ui.item.value;
            ui.item.value = ui.item.text;
            let input_hidden = event.target.parentElement.querySelector('input[type="hidden"]');
            input_hidden.value = id;
            $(control).parent().trigger('change');
            $(control).parent().find('span.description').text(ui.item.description);
        }, focus: function (event, ui) {
            $(control).val(ui.item.text);
            return false;
        }
    }).change(function () {
        let input = this;
        if (!input.value || input.value < minLength) {
            let input_hidden = input.parentElement.querySelector('input[type="hidden"]');
            input_hidden.value = '0';
            $(control).parent().find('span.description').text('');
        }
    }).blur(function () {
        let input = this;
        if (!input.value || input.value < minLength) {
            let input_hidden = input.parentElement.querySelector('input[type="hidden"]');
            input_hidden.value = '0';
            $(control).parent().find('span.description').text('');
        }
    });

    return true;
}

function loadAutoData(_url, _control, _parentSelector, _value, emptyVal) {
    $.get(_url + _value)
        .done(function (data) {
            if (data.length > 0) {
                $(_control).val(data[0].text);
                $(_control).parent().find('input[type="hidden"]:first').val(data[0].value);
                $(_control).parents(_parentSelector).trigger('change');
                $(caseCodeControl).parent().find('.description').text(data[0].description);
            } else {
                $(_control).parent().find('input.ui-autocomplete-input').val('');
                if (emptyVal) {
                    $(_control).parent().find('input[type="hidden"]:first').val(emptyVal);
                }
                $(_control).parent().find('.description').text('');
            }
        }).fail(function (errors) {
            console.log(errors);
        });
}




function initCaseCode() {
    $('.casecode-container').each(function (i, _container) {

        let _control = $(_container).find('.casecode-control')[0];

        initAutoCompleteControl(_control, autocompleteUrls.caseCodeSearch);

        let caseCodeVal = $(_container).find('.casecode-val').val();
        if (caseCodeVal && caseCodeVal !== '0') {
            loadCaseCode(_control, caseCodeVal);
            $(_container).trigger('change');
        }
    });
}

function loadCaseCode(control, value, emptyVal) {
    loadAutoData(autocompleteUrls.caseCodeGet, control, '.casecode-container:first', value, emptyVal);

    //    $.get(autocompleteUrls.caseCodeGet + caseCode)
    //        .done(function (data) {
    //            if (data.length > 0) {
    //                $(_control).val(data[0].text);
    //                $(_control).parent().find('input[type="hidden"]:first').val(data[0].value);
    //                //$(caseCodeControl).parent().find('.description').text(data[0].description);
    //                $(_control).parents('.casecode-container:first').trigger('change');
    //            } else {
    //                $(_control).parent().find('input.ui-autocomplete-input').val('');
    //                if (emptyVal) {
    //                    $(_control).parent().find('input[type="hidden"]:first').val(emptyVal);
    //                }
    //                $(_control).parent().find('.description').text('');
    //            }
    //        }).fail(function (errors) {
    //            console.log(errors);
    //        });
}





