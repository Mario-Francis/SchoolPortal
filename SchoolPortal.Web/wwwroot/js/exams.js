$(() => {
    // initialize datatable
    var examsTable = $('#examsTable').DataTable({
        serverSide: true,
        processing: true,
        ajax: {
            url: $base + 'exams/examsDataTable',
            type: "POST"
        },
        "order": [[2, "asc"]],
        "lengthMenu": [10, 20, 30, 50, 100],
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
            sheetName: 'Exam Schedules',
            exportOptions: {
                columns: [0, 1, 2, 3, 5, 7, 9, 10, 12]
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
                    "filter": "Session",
                    "display": "session"
                }, visible: true
            },
            {
                data: {
                    "filter": "Term",
                    "display": "term"
                }, visible: true
            },
            {
                data: {
                    "filter": "ExamType",
                    "display": "examType"
                }, visible: true
            },
            {
                data: {
                    "filter": "StartDate",
                    "display": "startDate"
                }, visible: false
            },
            {
                data: {
                    "filter": "FormattedStartDate",
                    "display": "formattedStartDate"
                }, orderData: 4
            }, {
                data: {
                    "filter": "EndDate",
                    "display": "endDate"
                }, visible: false
            },
            {
                data: {
                    "filter": "FormattedEndDate",
                    "display": "formattedEndDate"
                }, orderData: 6
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
                }, orderData: 8
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
                }, orderData: 11
            },
            {
                data: {
                    "filter": "Id",
                    "display": "id"
                }, "orderable": false, "render": function (data, type, row, meta) {
                    return '<div class="dropdown f14">'
                        + '<button type="button" class="btn px-3 f12" data-toggle="dropdown">'
                        + '<i class="fa fa-ellipsis-v"></i>'
                        + '</button>'
                        + '<div class="dropdown-menu f14">'
                        //+ `<a class="dropdown-item" href="#" eid="${row.id}">View Classrooms</a>`
                        //+ `<div class="dropdown-divider"></div>`
                        + `<a class="dropdown-item edit" href="javascript:void(0)" eid="${row.id}">Edit</a>`
                        + `<a class="dropdown-item delete" href="javascript:void(0)" eid="${row.id}">Delete</a>`
                        + '</div>'
                        + '</div>';
                }
            },
        ]
    }).buttons().container()
        .appendTo('#examsTable_wrapper .col-md-6:eq(0)');

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
                let examTypeId = $.trim($('#typeId').val());
                let session = $.trim($('#session').val());
                let termId = $.trim($('#termId').val());
                let startDate = $.trim($('#startDate').val());
                let endDate = $.trim($('#endDate').val());

                if (examTypeId == '' || session == '' || termId == '' || startDate == '' || endDate == '') {
                    notify('All fields are required', 'warning');
                } else {
                    $('fieldset').prop('disabled', true);
                    btn.html('<i class="fa fa-circle-notch fa-spin"></i> Adding exam...');
                    let url = $base + 'exams/AddExam';
                    let data = {
                        examTypeId,
                        session,
                        termId,
                        startDate,
                        endDate
                    };
                    $.ajax({
                        type: 'POST',
                        url: url,
                        data: data,
                        success: (response) => {
                            if (response.isSuccess) {
                                examsTable.ajax.reload();
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
        let eid = $(e.currentTarget).attr('eid');
        let loader = bootLoaderDialog('Fetching exam...');
        let exam = await getExam(eid);
        loader.hide();

        $('#e_typeId').val(exam.examTypeId);
        $('#e_session').val(exam.session);
        $('#e_termId').val(exam.termId);
        $('#e_startDate').val(exam.startDate.split('T')[0]);
        $('#e_endDate').val(exam.endDate.split('T')[0]);

        $('#updateBtn').attr('eid', eid);

        setTimeout(() => {
            $('#editModal').modal({ backdrop: 'static', keyboard: false }, 'show');
        }, 700);
    });

    // on update
    $('#updateBtn').on('click', (e) => {
        e.preventDefault();
        let btn = $(e.currentTarget);
        let eid = btn.attr('eid');
        try {
            let form = $("form")[1];
            if (validateForm(form)) {
                let examTypeId = $.trim($('#e_typeId').val());
                let session = $.trim($('#e_session').val());
                let termId = $.trim($('#e_termId').val());
                let startDate = $.trim($('#e_startDate').val());
                let endDate = $.trim($('#e_endDate').val());

                if (examTypeId == '' || session == '' || termId == '' || startDate == '' || endDate == '') {
                    notify('All fields are required', 'warning');
                } else {
                    $('fieldset').prop('disabled', true);
                    btn.html('<i class="fa fa-circle-notch fa-spin"></i> Updating exam...');
                    let url = $base + 'exams/UpdateExam';
                    let data = {
                        id: eid,
                        examTypeId,
                        session,
                        termId,
                        startDate,
                        endDate
                    };
                    $.ajax({
                        type: 'POST',
                        url: url,
                        data: data,
                        success: (response) => {
                            if (response.isSuccess) {
                                examsTable.ajax.reload();
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
        let eid = $(e.currentTarget).attr('eid');
        bootConfirm('Are you sure you want to delete exam?', {
            title: 'Confirm Action', size: 'small', callback: async (res) => {
                if (res) {
                    try {
                        loader = bootLoaderDialog('Deleting exam...');
                        let message = await deleteExam(eid);
                        loader.hide();

                        notify(message + '.', 'success');
                        examsTable.ajax.reload();
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



function getExam(id) {
    var promise = new Promise((resolve, reject) => {
        try {
            if (id == undefined || id == '' || id == 0) {
                reject('Invalid exam id');
            } else {
                let url = $base + 'exams/GetExam/' + id;
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

function deleteExam(id) {
    var promise = new Promise((resolve, reject) => {
        try {
            if (id == undefined || id == '' || id == 0) {
                reject('Invalid exam id');
            } else {
                let url = $base + 'exams/DeleteExam/' + id;
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
