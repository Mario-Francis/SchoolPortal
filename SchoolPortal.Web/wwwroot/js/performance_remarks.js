
$(() => {
    var exclude = [0, 7, 8, 9, 10, 11, 12];
    $('#remarksTable tfoot th').each(function (i, v) {
        if (!exclude.includes(i)) {
            var title = $(this).text();
            $(this).html('<input type="text" class="form-control bg-light f12" style="min-width:64px;" placeholder="\u{2315} ' + title + '" />');
        } else {
            $(this).html('');
        }
    });
    // initialize datatable
    let remarksTable = $('#remarksTable').DataTable({
        serverSide: true,
        processing: true,
        ajax: {
            url: $base + 'remarks/RemarksDataTable',
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
            sheetName: 'Performace Remarks',
            exportOptions: {
                columns: [0, 1, 2, 3, 4, 5, 6, 8, 9, 11]
            }
        }],
        //rowId: 'id',
        initComplete: function () {
            var r = $('#remarksTable tfoot tr');
            r.find('th').each(function () {
                $(this).css('padding', '4px 8px');
            });
            $('#remarksTable thead').append(r);
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
                    "filter": "ExamName",
                    "display": "examName"
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
                    "filter": "TeacherRemark",
                    "display": "teacherRemark"
                }, "render": function (data, type, row, meta) {
                    return `<p class="text-dark" style="min-width:240px;max-width:300px;white-space:break-spaces;">${data ?? '---'}</p>`;
                },
            },
            {
                data: {
                    "filter": "HeadTeacherRemark",
                    "display": "headTeacherRemark"
                }, "render": function (data, type, row, meta) {
                    return `<p class="text-dark" style="min-width:200px;max-width:300px;white-space:break-spaces;">${data ?? '---'}</p>`;
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
                }, orderData: 7
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
                }, orderData: 10
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
        .appendTo('#remarksTable_wrapper .col-md-6:eq(0)');




    $('#batchAddBtn').on('click', (e) => {
        $('#batchUploadModal').modal({ backdrop: 'static', keyboard: false }, 'show');
    });


    // edit
    $(document).on('click', '.edit', async (e) => {
        let rid = $(e.currentTarget).attr('rid');
        let loader = bootLoaderDialog('Fetching performance remark...');
        try {
            let record = await getRemark(rid);
            loader.hide();

            $('#tremark').val(record.teacherRemark);
            $('#htremark').val(record.headTeacherRemark);

            $('#updateBtn').attr('rid', rid);
            $('#updateBtn').attr('examId', record.examId);
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
                let teacherRemark = $.trim($('#tremark').val());
                let headTeacherRemark = $.trim($('#htremark').val());

                let examId = $('#updateBtn').attr('examId');
                let studentId = $('#updateBtn').attr('studentId');

                if (teacherRemark == '' || headTeacherRemark == '') {
                    notify('Fields with asteriks (*) are required', 'warning');
                } else {
                    $('fieldset, .btn.action').prop('disabled', true);
                    btn.html('<i class="fa fa-circle-notch fa-spin"></i> Updating performance remarks...');
                    let url = $base + 'remarks/UpdateRemark';
                    let data = {
                        id,
                        examId,
                        studentId,
                        teacherRemark,
                        headTeacherRemark
                    };
                    $.ajax({
                        type: 'POST',
                        url: url,
                        data: data,
                        success: (response) => {
                            if (response.isSuccess) {
                                notify(response.message + '.', 'success');
                                remarksTable.ajax.reload();
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
                let examId = $('#examId').val();
                let files = $('#file')[0].files;

                if (examId == '') {
                    notify('All fields with asteriks (*) are required.', 'warning');
                }
                else if (files.length == 0) {
                    notify('No file selected! Kindly select a valid excel file.', 'warning');
                } else {
                    let formData = new FormData();
                    formData.append('examId', examId);
                    formData.append('file', files[0]);

                    $('fieldset, .btn.action').prop('disabled', true);
                    btn.html('<i class="fa fa-circle-notch fa-spin"></i> Uploading file...');
                    let url = $base + 'remarks/BatchUploadRemarks';

                    $.ajax({
                        type: 'POST',
                        url: url,
                        data: formData,
                        contentType: false,
                        cache: false,
                        processData: false,
                        success: (response) => {
                            if (response.isSuccess) {
                                //remarksTable.ajax.reload()
                                remarksTable.ajax.reload();
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


function getRemark(id) {
    var promise = new Promise((resolve, reject) => {
        try {
            if (id == undefined || id == '' || id == 0) {
                reject('Invalid remark id');
            } else {
                let url = $base + 'remarks/GetRemark/' + id;
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
