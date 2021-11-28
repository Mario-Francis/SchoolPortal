var userId = $('#userId').val();
var selectizedd;
$(() => {
    selectizedd = initializeParentssDropdown();
    // initialize datatable
    wardsTable = $('#wardsTable').DataTable({
        serverSide: true,
        processing: true,
        ajax: {
            url: $base + 'users/WardsDataTable/' + userId,
            type: "POST"
        },
        "order": [[2, "asc"]],
        "lengthMenu": [10, 20, 30, 50, 100],
        "paging": true,
        autoWidth: false,
        //rowId: 'id',
        columns: [
            {
                data: {
                    "filter": "Id",
                    "display": "id"
                }, "orderable": true, "render": function (data, type, row, meta) {
                    return (meta.row + 1 + meta.settings._iDisplayStart) + '.';
                }
            },
          
            {
                data: {
                    "filter": "Relationship",
                    "display": "relationship"
                }
            },
            {
                data: {
                    "filter": "FullName",
                    "display": "fullName"
                }, visible: true
            },
            {
                data: {
                    "filter": "Username",
                    "display": "username"
                }, "render": function (data, type, row, meta) {
                    return `${data}`;
                }
            },
            {
                data: {
                    "filter": "Email",
                    "display": "email"
                }, "render": function (data, type, row, meta) {
                    return `${data}`;
                }
            },
            {
                data: {
                    "filter": "FormattedClassRoom",
                    "display": "formattedClassRoom"
                }
            },
            {
                data: {
                    "filter": "Id",
                    "display": "id"
                }, "orderable": false, "render": function (data, type, row, meta) {
                    let status = row.isActive;
                    return '<div class="dropdown f14">'
                        + '<button type="button" class="btn px-3 f12" data-toggle="dropdown">'
                        + '<i class="fa fa-ellipsis-v"></i>'
                        + '</button>'
                        + '<div class="dropdown-menu f14">'
                        + `<a class="dropdown-item" href="${$base}students/${row.studentId}" uid="${row.studentId}">View Profile</a>`
                        + `<div class="dropdown-divider"></div>`
                        + `<a class="dropdown-item remove" href="javascript:void(0)" uid="${data}">Remove</a>`
                        + '</div>'
                        + '</div>';
                }
            },
        ]
    });


    $('#addBtn').on('click', (e) => {
        $('#addModal').modal({ backdrop: 'static', keyboard: false }, 'show');
    });


    // on add
    $('#createBtn').on('click', (e) => {
        e.preventDefault();
        let btn = $(e.currentTarget);
        try {
            let form = $("form")[0];
            if (validateForm(form)) {
                let relationshipId = $.trim($('#relationship').val());
                let studentId = $.trim($('#student').val());

                if (relationshipId == '' || studentId == '') {
                    notify('Fields with asteriks (*) are required', 'warning');
                } else {
                    $('fieldset').prop('disabled', true);
                    btn.html('<i class="fa fa-circle-notch fa-spin"></i> Adding ward...');
                    let url = $base + 'users/AddWard';
                    let data = {
                        studentId,
                        userId,
                        relationshipId
                    };
                    $.ajax({
                        type: 'POST',
                        url: url,
                        data: data,
                        success: (response) => {
                            if (response.isSuccess) {
                                wardsTable.ajax.reload();
                                notify(response.message + '.', 'success');

                                form.reset();
                                selectizedd[0].selectize.clear();
                                $('#addModal').modal('hide');

                            } else {
                                notify(response.message, 'danger');
                            }
                            btn.html('<i class="fa fa-check-circle"></i> &nbsp;Submit');
                            $('fieldset').prop('disabled', false);
                        },
                        error: (req, status, err) => {
                            ajaxErrorHandler(req, status, err, {
                                callback: () => {
                                    btn.html('<i class="fa fa-check-circle"></i> &nbsp;Submit');
                                    $('fieldset').prop('disabled', false);
                                }
                            });
                        }
                    });
                }
            }
        } catch (ex) {
            console.error(ex);
            notify(ex.message, 'danger');
            btn.html('<i class="fa fa-check-circle"></i> &nbsp;Submit');
            $('fieldset').prop('disabled', false);
        }
    });


    // on remove
    $(document).on('click', '.remove', async (e) => {
        let loader;
        let uid = $(e.currentTarget).attr('uid');
        bootConfirm('Are you sure you want to remove this ward?', {
            title: 'Confirm Action', size: 'small', callback: async (res) => {
                if (res) {
                    try {
                        loader = bootLoaderDialog('Removing ward...');
                        let message = await deleteWard(uid);
                        loader.hide();

                        notify(message + '.', 'success');
                        wardsTable.ajax.reload();
                    } catch (ex) {
                        console.error(ex);
                        if (ex != null) {
                            notify(ex + '.', 'danger');
                        }
                        loader.hide();
                    }
                }
            }
        });
    });


});

function initializeParentssDropdown() {
    var _select = $(".studentsdd").selectize({
        valueField: "id",
        searchField: ["email", "username", "firstName", "surname", "middleName", "phoneNumber"],
        placeholder: '- Search parent -',
        dropdownParent: 'body',
        create: false,
        preload: 'focus',
        render: {
            option: function (item, escape) {
                return (
                    `<div class="d-flex flex-row px-3 py-2 border-top bg-white">
                        <div>
                            <div class="rounded-circle mr-3 bg-claret" style="height:36px;width:36px;padding-top:8px;">
                                <p class="m-0 f14 text-center text-white">${getInitial(item, escape)}</p>
                            </div>
                        </div>
                        <div class="flex-fill">
                            <p class="f14 font-weight-bold text-dark mt-1">${capitalize(escape(item.firstName).trim())} ${capitalize(escape(item.middleName).trim())} ${capitalize(escape(item.surname).trim())}</p>
                            <p class="f12" style="margin-top:-6px;">${escape(item.email).trim()}</p>
                        </div>
                    </div>`
                );
            },
            item: function (item, escape) {
                return (
                    `<div class="d-flex flex-row px-3 py-1">
                        <div>
                            <div class="rounded-circle mr-3 bg-claret" style="height:36px;width:36px;padding-top:8px;">
                                <p class="m-0 f14 text-center text-white">${getInitial(item, escape)}</p>
                            </div>
                        </div>
                        <div class="flex-fill">
                            <p class="f14 font-weight-bold text-dark mt-1">${capitalize(escape(item.firstName).trim())} ${capitalize(escape(item.middleName).trim())} ${capitalize(escape(item.surname).trim())}</p>
                            <p class="f12" style="margin-top:-6px;">${escape(item.email).trim()}</p>
                        </div>
                    </div>`
                );
            },
        },
        load: function (query, callback) {
            if (!query.length) return callback();
            $.ajax({
                url: $base + 'students/SearchStudents?max=50&query=' + encodeURIComponent(query),
                type: "GET",
                error: function (err) {
                    console.log(err);
                    callback();
                },
                success: function (res) {
                    callback(res.data);
                },
            });
        },
    });
    return _select;
}

function getInitial(item, escape) {
    var l1 = item.firstName == null ? "" : escape(item.firstName.trim())[0];
    var l2 = item.surname == null ? "" : escape(item.surname.trim())[0];
    return l1 + l2;
}


function deleteWard(id) {
    var promise = new Promise((resolve, reject) => {
        try {
            if (id == undefined || id == '' || id == 0) {
                reject('Invalid ward id');
            } else {
                let url = $base + 'users/RemoveWard/' + id;
                $.ajax({
                    type: 'GET',
                    url: url,
                    success: (response) => {
                        if (response.isSuccess) {
                            resolve(response.message);
                        } else {
                            reject(response.message);
                        }
                    },
                    error: (req, status, err) => {
                        reject(null);
                        ajaxErrorHandler(req, status, err, {

                        });
                    }
                });
            }

        } catch (ex) {
            console.error(ex);
            //notify(ex.message, 'danger');
            reject(ex.message);
        }
    });
    return promise;
}

