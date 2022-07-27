$(() => {
    // initialize datatable
    var subjectsTable = $('#subjectsTable').DataTable({
        serverSide: true,
        processing: true,
        ajax: {
            url: $base + 'subjects/SubjectsDataTable',
            type: "POST"
        },
        "order": [[2, "asc"]],
        "lengthMenu": [10, 20, 30, 50, 100, 500, 1000],
        "paging": true,
        autoWidth: false,
        dom: "<'row'<'col-sm-12 col-md-6'l><'col-sm-12 col-md-6'f>>" +
            "<'row'<'col-sm-12'B>>" +
            "<'row'<'col-sm-12'tr>>" +
            "<'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>",
        buttons: [{
            extend: 'excelHtml5',
            text: `<i class="fa fa-file-excel"></i> Export to excel`,
            className: 'f14 btn-success py-1 my-1',
            autoFilter: true,
            sheetName: 'Subjects',
            exportOptions: {
                columns: [0, 1, 2, 3, 4, 6, 7, 9]
            }
        }],
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
                    "filter": "Class",
                    "display": "class"
                }, visible: true
            },
            {
                data: {
                    "filter": "Name",
                    "display": "name"
                }
            },
            {
                data: {
                    "filter": "Code",
                    "display": "code"
                }
            },
            {
                data: {
                    "filter": "Description",
                    "display": "description"
                }, "render": function (data, type, row, meta) {
                    return `<p style="width:200px;white-space:normal;" class="text-dark">${data ? data : ''}</p>`;
                }
            },
            {
                data: {
                    "filter": "CreatedDate",
                    "display": "createdDate"
                }, visible: false
            },
            {
                data: {
                    "filter": "FormattedCreatedDate",
                    "display": "formattedCreatedDate"
                }, orderData: 5
            },
            {
                data: {
                    "filter": "UpdatedBy",
                    "display": "updatedBy"
                }
            },
            {
                data: {
                    "filter": "UpdatedDate",
                    "display": "updatedDate"
                }, visible: false
            },
            {
                data: {
                    "filter": "FormattedUpdatedDate",
                    "display": "formattedUpdatedDate"
                }, orderData: 8
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
                        // + `<a class="dropdown-item" href="#" cid="${row.id}">View Mid-Term Results</a>`
                        //+ `<a class="dropdown-item" href="#" cid="${row.id}">View End-Term Results</a>`
                        // + `<div class="dropdown-divider"></div>`
                        + `<a class="dropdown-item edit" href="javascript:void(0)" sid="${row.id}">Edit</a>`
                        + `<a class="dropdown-item delete" href="javascript:void(0)" sid="${row.id}">Delete</a>`
                        + '</div>'
                        + '</div>';
                }
            },
        ]
    });
    subjectsTable.buttons().container()
        .appendTo('#subjectsTable_wrapper .col-md-6:eq(0)');

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
                let classId = $.trim($('#classId').val());
                let name = $.trim($('#name').val());
                let code = $.trim($('#code').val());
                let description = $.trim($('#description').val());

                if (classId == '' || name == '') {
                    notify('Fields with asteriks (*) are required', 'warning');
                } else {
                    $('fieldset').prop('disabled', true);
                    btn.html('<i class="fa fa-circle-notch fa-spin"></i> Adding subject...');
                    let url = $base + 'subjects/AddSubject';
                    let data = {
                        classId,
                        name,
                        code,
                        description
                    };
                    $.ajax({
                        type: 'POST',
                        url: url,
                        data: data,
                        success: (response) => {
                            if (response.isSuccess) {
                                subjectsTable.ajax.reload();
                                notify(response.message + '.', 'success');

                                form.reset();
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

    // on edit
    $(document).on('click', '.edit', async (e) => {
        let sid = $(e.currentTarget).attr('sid');
        let loader = bootLoaderDialog('Fetching subject...');
        let subject = null;
        try {
            subject = await getSubject(sid);
            loader.hide();

            $('#e_classId').val(subject.classId);
            $('#e_name').val(subject.name);
            $('#e_code').val(subject.code);
            $('#e_description').val(subject.description);

            $('#updateBtn').attr('sid', sid);

            setTimeout(() => {
                $('#editModal').modal({ backdrop: 'static', keyboard: false }, 'show');
            }, 700);
        } catch (ex) {
            console.error(ex);
            notify(ex.message, 'danger');
            loader.hide();
        }
    });

    // on update
    $('#updateBtn').on('click', (e) => {
        e.preventDefault();
        let btn = $(e.currentTarget);
        let sid = btn.attr('sid');
        try {
            let form = $("form")[1];
            if (validateForm(form)) {
                let classId = $.trim($('#e_classId').val());
                let name = $.trim($('#e_name').val());
                let code = $.trim($('#e_code').val());
                let description = $.trim($('#e_description').val());

                if (classId == '' || name == '') {
                    notify('Fields with asteriks (*) are required', 'warning');
                } else {
                    $('fieldset').prop('disabled', true);
                    btn.html('<i class="fa fa-circle-notch fa-spin"></i> Updating subject...');
                    let url = $base + 'subjects/UpdateSubject';
                    let data = {
                        id: sid,
                        classId,
                        name,
                        code,
                        description
                    };
                    $.ajax({
                        type: 'POST',
                        url: url,
                        data: data,
                        success: (response) => {
                            if (response.isSuccess) {
                                subjectsTable.ajax.reload();
                                notify(response.message + '.', 'success');

                                form.reset();
                                $('#editModal').modal('hide');

                            } else {
                                notify(response.message, 'danger');
                            }
                            btn.html('<i class="fa fa-check-circle"></i> &nbsp;Update');
                            $('fieldset').prop('disabled', false);
                        },
                        error: (req, status, err) => {
                            ajaxErrorHandler(req, status, err, {
                                callback: () => {
                                    btn.html('<i class="fa fa-check-circle"></i> &nbsp;Update');
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
            btn.html('<i class="fa fa-check-circle"></i> &nbsp;Update');
            $('fieldset').prop('disabled', false);
        }
    });

    // on remove
    $(document).on('click', '.delete', async (e) => {
        let loader;
        let sid = $(e.currentTarget).attr('sid');
        bootConfirm('Are you sure you want to delete this subject?', {
            title: 'Confirm Action', size: 'small', callback: async (res) => {
                if (res) {
                    try {
                        loader = bootLoaderDialog('Deleting subject...');
                        let message = await deleteSubject(sid);
                        loader.hide();

                        notify(message + '.', 'success');
                        subjectsTable.ajax.reload();
                    } catch (ex) {
                        loader.hide();
                        console.error(ex);
                        notify(ex + '.', 'danger');
                    }
                }
            }
        });
    });

});



function getSubject(id) {
    var promise = new Promise((resolve, reject) => {
        try {
            if (id == undefined || id == '' || id == 0) {
                reject('Invalid subject id');
            } else {
                let url = $base + 'subjects/GetSubject/' + id;
                $.ajax({
                    type: 'GET',
                    url: url,
                    success: (response) => {
                        if (response.isSuccess) {
                            resolve(response.data);
                        } else {
                            reject(response.message);
                        }
                    },
                    error: (req, status, err) => {
                        ajaxErrorHandler(req, status, err, {});
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

function deleteSubject(id) {
    var promise = new Promise((resolve, reject) => {
        try {
            if (id == undefined || id == '' || id == 0) {
                reject('Invalid subject id');
            } else {
                let url = $base + 'subjects/DeleteSubject/' + id;
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
                        ajaxErrorHandler(req, status, err, {});
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
