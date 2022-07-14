
$(() => {
    var exclude = [0, 10, 11, 12, 13, 14, 15];
    $('#recordsTable tfoot th').each(function (i, v) {
        if (!exclude.includes(i)) {
            var title = $(this).text();
            $(this).html('<input type="text" class="form-control bg-light f12" style="min-width:64px;" placeholder="\u{2315} ' + title + '" />');
        } else {
            $(this).html('');
        }
    });
    // initialize datatable
    let recordsTable = $('#recordsTable').DataTable({
        serverSide: true,
        processing: true,
        ajax: {
            url: $base + 'healthRecords/RecordsDataTable',
            type: "POST"
        },
        "order": [[3, "desc"]],
        "lengthMenu": [10, 20, 30, 50, 100, 500, 1000, 2000, 5000],
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
            sheetName: 'Health Records',
            exportOptions: {
                columns: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 11, 12, 14]
            }
        }],
        //rowId: 'id',
        initComplete: function () {
            var r = $('#recordsTable tfoot tr');
            r.find('th').each(function () {
                $(this).css('padding', '4px 8px');
            });
            $('#recordsTable thead').append(r);
            $('#search_0').css('text-align', 'center');

            // Apply the search
            this.api().columns().every(function () {
                var that = this;

                $('input', this.footer()).on('keyup change clear', function () {
                    if (that.search() !== this.value) {
                        that.search(this.value)
                            .draw();
                    }
                });
            });
        },
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
                    "filter": "StudentName",
                    "display": "studentName"
                }, visible: true
            },
            {
                data: {
                    "filter": "AdmissionNo",
                    "display": "admissionNo"
                }
            },
            {
                data: {
                    "filter": "Session",
                    "display": "session"
                }
            },
            {
                data: {
                    "filter": "TermName",
                    "display": "termName"
                }
            },
            {
                data: {
                    "filter": "Class",
                    "display": "class"
                }
            },
            {
                data: {
                    "filter": "StartHeight",
                    "display": "startHeight"
                }, "render": function (data, type, row, meta) {
                    return `${data}mtr`;
                }
            },
            {
                data: {
                    "filter": "EndHeight",
                    "display": "endHeight"
                }, "render": function (data, type, row, meta) {
                    return `${data}mtr`;
                }
            },
            {
                data: {
                    "filter": "StartWeight",
                    "display": "startWeight"
                }, "render": function (data, type, row, meta) {
                    return `${data}kg`;
                }
            },
            {
                data: {
                    "filter": "EndWeight",
                    "display": "endWeight"
                }, "render": function (data, type, row, meta) {
                    return `${data}kg`;
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
                }, orderData: 10
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
                }, orderData: 13
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
                        // + `<a class="dropdown-item" href="${$base}Students/${row.id}" rid="${row.id}">View Exam Details</a>`
                        // + `<div class="dropdown-divider"></div>`
                        + `<a class="dropdown-item edit" href="javascript:void(0)" rid="${row.id}">Edit</a>`
                        + '</div>'
                        + '</div>';
                }
            },
        ]
    }).buttons().container()
        .appendTo('#recordsTable_wrapper .col-md-6:eq(0)');




    $('#batchAddBtn').on('click', (e) => {
        $('#batchUploadModal').modal({ backdrop: 'static', keyboard: false }, 'show');
    });


    // edit
    $(document).on('click', '.edit', async (e) => {
        let rid = $(e.currentTarget).attr('rid');
        let loader = bootLoaderDialog('Fetching health record...');
        try {
            let record = await getRecord(rid);
            loader.hide();

            $('#startHeight').val(record.startHeight);
            $('#endHeight').val(record.endHeight);
            $('#startWeight').val(record.startWeight);
            $('#endWeight').val(record.endWeight);

            $('#updateBtn').attr('rid', rid);
            $('#updateBtn').attr('session', record.session);
            $('#updateBtn').attr('termId', record.termId);
            $('#updateBtn').attr('studentId', record.studentId);

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
        let id = btn.attr('rid');
        try {
            let form = $("form")[0];
            if (validateForm(form)) {
                let startHeight = $.trim($('#startHeight').val());
                let endHeight = $.trim($('#endHeight').val());
                let startWeight = $.trim($('#startWeight').val());
                let endWeight = $.trim($('#endWeight').val());

                let session = $('#updateBtn').attr('session');
                let termId = $('#updateBtn').attr('termId');
                let studentId = $('#updateBtn').attr('studentId');

                if (startHeight == '' || endHeight == '' || startWeight == '' || endWeight=='') {
                    notify('Fields with asteriks (*) are required', 'warning');
                } else {
                    $('fieldset, .btn.action').prop('disabled', true);
                    btn.html('<i class="fa fa-circle-notch fa-spin"></i> Updating health record...');
                    let url = $base + 'healthRecords/UpdateRecord';
                    let data = {
                        id,
                        session,
                        termId,
                        studentId,
                        startHeight,
                        endHeight,
                        startWeight,
                        endWeight
                    };
                    $.ajax({
                        type: 'POST',
                        url: url,
                        data: data,
                        success: (response) => {
                            if (response.isSuccess) {
                                notify(response.message + '.', 'success');
                                recordsTable.ajax.reload();
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


    // batch upload students
    $('#uploadBtn').on('click', (e) => {
        e.preventDefault();
        let btn = $(e.currentTarget);
        try {
            let form = $("form")[1];
            if (validateForm(form)) {
                let session = $('#session').val();
                let termId = $('#termId').val();
                let files = $('#file')[0].files;

                if (session == '' || termId == '') {
                    notify('All fields with asteriks (*) are required.', 'warning');
                }
                else if (files.length == 0) {
                    notify('No file selected! Kindly select a valid excel file.', 'warning');
                } else {
                    let formData = new FormData();
                    formData.append('session', session);
                    formData.append('termId', termId);
                    formData.append('file', files[0]);

                    $('fieldset, .btn.action').prop('disabled', true);
                    btn.html('<i class="fa fa-circle-notch fa-spin"></i> Uploading file...');
                    let url = $base + 'healthRecords/BatchUploadRecords';

                    $.ajax({
                        type: 'POST',
                        url: url,
                        data: formData,
                        contentType: false,
                        cache: false,
                        processData: false,
                        success: (response) => {
                            if (response.isSuccess) {
                                //recordsTable.ajax.reload()
                                recordsTable.ajax.reload();
                                notify(response.message + '.', 'success');

                                form.reset();
                                $('#batchUploadModal').modal('hide');
                            } else {
                                notify(response.message + '.', 'danger');
                            }
                            btn.html('<i class="fa fa-upload"></i> &nbsp;Upload File');
                            $('fieldset, .btn.action').prop('disabled', false);
                        },
                        error: (req, status, err) => {
                            ajaxErrorHandler(req, status, err, {
                                callback: () => {
                                    btn.html('<i class="fa fa-upload"></i> &nbsp;Upload File');
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
            btn.html('<i class="fa fa-upload"></i> &nbsp;Upload File');
            $('fieldset, .btn.action').prop('disabled', false);
        }
    });

});


function getRecord(id) {
    var promise = new Promise((resolve, reject) => {
        try {
            if (id == undefined || id == '' || id == 0) {
                reject('Invalid record id');
            } else {
                let url = $base + 'healthRecords/GetRecord/' + id;
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
            //notify(ex.message, 'danger');
            reject(ex.message);
        }
    });
    return promise;
}
