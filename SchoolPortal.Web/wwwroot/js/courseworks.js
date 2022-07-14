var courseWorksTable;
$(() => {
    
    // initialize datatable
    courseWorksTable = $('#courseWorksTable').DataTable({
        serverSide: true,
        processing: true,
        ajax: {
            url: $base + 'CourseWorks/CourseWorksDataTable',
            type: "POST"
        },
        "order": [[0, "desc"]],
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
            sheetName: 'Courseworks',
            exportOptions: {
                columns: [0, 1, 2, 3, 5, 7, 8, 10, 11, 13]
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
                    "filter": "Title",
                    "display": "title"
                }, visible: true,
                render: function (data, type, row, meta) {
                    let ext = row.filePath.split('.').pop();
                    return `${getDocIcon(ext)} ${data}`;
                }
            },
            {
                data: {
                    "filter": "Description",
                    "display": "description"
                }
            },
            {
                data: {
                    "filter": "WeekNo",
                    "display": "weekNo"
                }
            },
            {
                data: {
                    "filter": "From",
                    "display": "from"
                }, visible: false
            },
            {
                data: {
                    "filter": "FormattedFrom",
                    "display": "formattedFrom"
                }, orderData: 4
            },
            {
                data: {
                    "filter": "To",
                    "display": "to"
                }, visible: false
            },
            {
                data: {
                    "filter": "FormattedTo",
                    "display": "formattedTo"
                }, orderData: 6
            },
            {
                data: {
                    "filter": "ClassRoomName",
                    "display": "classRoomName"
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
                }, orderData: 9
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
                }, orderData: 12
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
                         + `<a class="dropdown-item" download href="${$base}${row.filePath}">Download</a>`
                         + `<div class="dropdown-divider"></div>`
                        + `<a class="dropdown-item edit" href="javascript:void(0)" cid="${row.id}">Edit</a>`
                        + `<a class="dropdown-item delete" href="javascript:void(0)" cid="${row.id}">Delete</a>`
                        + '</div>'
                        + '</div>';
                }
            },
        ]
    }).buttons().container()
        .appendTo('#courseWorksTable_wrapper .col-md-6:eq(0)');




    $('#addBtn').on('click', (e) => {
        $('#addModal').modal({ backdrop: 'static', keyboard: false }, 'show');
    });

    // add course work
    $('#submitBtn').on('click', (e) => {
        e.preventDefault();
        let btn = $(e.currentTarget);
        try {
            let form = $("form")[0];
            if (validateForm(form)) {
                let classRoomId = $.trim($('#classRoomId').val());
                let title = $.trim($('#title').val());
                let description = $.trim($('#description').val());
                let from = $.trim($('#from').val());
                let to = $.trim($('#to').val());
                let week = $.trim($('#week').val());
                let files = $('#file')[0].files;

                if (classRoomId == '' || title == '' || description=='' || from == '' || to == '' || week == '') {
                    notify('All fields with asteriks (*) are required.', 'warning');
                }
                else if (files.length == 0) {
                    notify('No file selected! Kindly select a valid file.', 'warning');
                } else {
                    let formData = new FormData();
                    formData.append('classRoomId', classRoomId);
                    formData.append('title', title);
                    formData.append('description', description);
                    formData.append('from', from);
                    formData.append('to', to);
                    formData.append('weekNo', week);
                    formData.append('file', files[0]);

                    $('fieldset, .btn.action').prop('disabled', true);
                    btn.html('<i class="fa fa-circle-notch fa-spin"></i> Adding course work...');
                    let url = $base + 'courseWorks/AddCourseWork';

                    $.ajax({
                        type: 'POST',
                        url: url,
                        data: formData,
                        contentType: false,
                        cache: false,
                        processData: false,
                        success: (response) => {
                            if (response.isSuccess) {
                                courseWorksTable.ajax.reload();
                                notify(response.message + '.', 'success');

                                form.reset();
                                $('#addModal').modal('hide');
                            } else {
                                notify(response.message + '.', 'danger');
                            }
                            btn.html('<i class="fa fa-check-circle"></i> &nbsp;Submit');
                            $('fieldset, .btn.action').prop('disabled', false);
                        },
                        error: (req, status, err) => {
                            ajaxErrorHandler(req, status, err, {
                                callback: () => {
                                    btn.html('<i class="fa fa-check-circle"></i> &nbsp;Submit');
                                    $('fieldset, .btn.action').prop('disabled', false);
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
            $('fieldset, .btn.action').prop('disabled', false);
        }
    });

    // edit
    $(document).on('click', '.edit', async (e) => {
        let cid = $(e.currentTarget).attr('cid');
        let loader = bootLoaderDialog('Fetching course work...');
        try {
            let coursework = await getCourseWork(cid);
            loader.hide();

            $('#e_classRoomId').val(coursework.classRoomId);
            $('#e_title').val(coursework.title);
            $('#e_description').val(coursework.description);
            $('#e_from').val(coursework.from.split('T')[0]);
            $('#e_to').val(coursework.to.split('T')[0]);
            $('#e_week').val(coursework.weekNo);

            $('#updateBtn').attr('cid', cid);

            setTimeout(() => {
                $('#editModal').modal({ backdrop: 'static', keyboard: false }, 'show');
            }, 700);
        } catch (ex) {
            loader.hide();
            if (ex != null) {
                console.error(ex);
                notify(ex.message, 'danger');
            }
        }
    });

    // update
    $('#updateBtn').on('click', (e) => {
        e.preventDefault();
        let btn = $(e.currentTarget);
        let id = btn.attr('cid');
        try {
            let form = $("form")[1];
            if (validateForm(form)) {
                let classRoomId = $.trim($('#e_classRoomId').val());
                let title = $.trim($('#e_title').val());
                let description = $.trim($('#e_description').val());
                let from = $.trim($('#e_from').val());
                let to = $.trim($('#e_to').val());
                let week = $.trim($('#e_week').val());

                if (classRoomId == '' || title == '' || description == '' || from == '' || to == '' || week == '') {
                    notify('All fields with asteriks (*) are required.', 'warning');
                } else {
                    $('fieldset, .btn.action').prop('disabled', true);
                    btn.html('<i class="fa fa-circle-notch fa-spin"></i> Updating coursework...');
                    let url = $base + 'courseWorks/UpdateCourseWork';
                    let data = {
                        id,
                        classRoomId,
                        title,
                        description,
                        from,
                        to,
                        weekNo:week
                    };
                    $.ajax({
                        type: 'POST',
                        url: url,
                        data: data,
                        success: (response) => {
                            if (response.isSuccess) {
                                notify(response.message + '.', 'success');
                                courseWorksTable.ajax.reload();
                                form.reset();

                                $('#editModal').modal('hide');

                            } else {
                                notify(response.message, 'danger');
                            }
                            btn.html('<i class="fa fa-check-circle"></i> &nbsp;Update');
                            $('fieldset, .btn.action').prop('disabled', false);
                        },
                        error: (req, status, err) => {
                            ajaxErrorHandler(req, status, err, {
                                callback: () => {
                                    btn.html('<i class="fa fa-check-circle"></i> &nbsp;Update');
                                    $('fieldset, .btn.action').prop('disabled', false);
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
            $('fieldset, .btn.action').prop('disabled', false);
        }
    });

    // on remove
    $(document).on('click', '.delete', async (e) => {
        let loader;
        let cid = $(e.currentTarget).attr('cid');
        bootConfirm('Are you sure you want to delete this course work?', {
            title: 'Confirm Action', size: 'small', callback: async (res) => {
                if (res) {
                    try {
                        loader = bootLoaderDialog('Deleting course work...');
                        let message = await deleteCourseWork(cid);
                        loader.hide();

                        notify(message + '.', 'success');
                        courseWorksTable.ajax.reload();
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


function getCourseWork(id) {
    var promise = new Promise((resolve, reject) => {
        try {
            if (id == undefined || id == '' || id == 0) {
                reject('Invalid coursework id');
            } else {
                let url = $base + `courseWorks/${id}/GetCourseWork`;
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
                        reject(null);
                    }
                });
            }

        } catch (ex) {
            console.error(ex);
            reject(ex.message);
        }
    });
    return promise;
}

function deleteCourseWork(id) {
    var promise = new Promise((resolve, reject) => {
        try {
            if (id == undefined || id == '' || id == 0) {
                reject('Invalid coursework id');
            } else {
                let url = $base + `courseWorks/${id}/DeleteCourseWork`;
                $.ajax({
                    type: 'DELETE',
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
                        reject(null);
                    }
                });
            }

        } catch (ex) {
            console.error(ex);
            reject(ex.message);
        }
    });
    return promise;
}

function getDocIcon(ext) {
    switch (ext) {
        case 'pdf':
            return '<i class="fa fa-file-pdf f24 pr-2" style="color:#ff0000;"></i>';
            break;
        case 'doc':
        case 'docx':
            return '<i class="fa fa-file-word f24 pr-2" style="color:#2b579a;"></i>';
            break;
        case 'xls':
        case 'xlsx':
            return '<i class="fa fa-file-excel f24 pr-2" style="color:#217346;"></i>';
            break;
        case 'txt':
            return '<i class="fa fa-file-alt f24 pr-2" style="color:#444;"></i>';
            break;
        case 'ppt':
        case 'pptx':
            return '<i class="fa fa-file-powerpoint f24 pr-2" style="color:#d24726;"></i>';
            break;
        default:
            return '<i class="fa fa-file f24 pr-2" style="color:#444;"></i>';
            break;
    }
}