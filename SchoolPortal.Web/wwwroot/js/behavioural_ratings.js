
$(() => {
    var exclude = [0, 8,9,10,11,12, 13];
    $('#resultsTable tfoot th').each(function (i, v) {
        if (!exclude.includes(i)) {
            var title = $(this).text();
            $(this).html('<input type="text" class="form-control bg-light f12" style="min-width:64px;" placeholder="\u{2315} ' + title + '" />');
        } else {
            $(this).html('');
        }
    });
    // initialize datatable
    let resultsTable = $('#resultsTable').DataTable({
        serverSide: true,
        processing: true,
        ajax: {
            url: $base + 'behaviouralRatings/RatingsDataTable',
            type: "POST"
        },
        "order": [[3, "desc"]],
        "lengthMenu": [10, 20, 30, 50, 100, 500],
        "paging": true,
        autoWidth: false,
        //rowId: 'id',
        initComplete: function () {
            var r = $('#resultsTable tfoot tr');
            r.find('th').each(function () {
                $(this).css('padding', '4px 8px');
            });
            $('#resultsTable thead').append(r);
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
                    "filter": "Rating",
                    "display": "rating"
                }
            },
            {
                data: {
                    "filter": "Score",
                    "display": "score"
                }, "render": function (data, type, row, meta) {
                    return `${data}`;
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
    });



   
    $('#batchAddBtn').on('click', (e) => {
        $('#batchUploadModal').modal({ backdrop: 'static', keyboard: false }, 'show');
    });

  
    // edit
    $(document).on('click', '.edit', async (e) => {
        let rid = $(e.currentTarget).attr('rid');
        let loader = bootLoaderDialog('Fetching behavioural result...');
        try {
            let result = await getRemark(rid);
            loader.hide();

            $('#behaviour').val(result.rating);
            $('#rating').val(result.score);
            $('#behaviour').attr('ratingId', result.behaviouralRatingId);


            $('#updateBtn').attr('rid', rid);
            $('#updateBtn').attr('session', result.session);
            $('#updateBtn').attr('termId', result.termId);
            $('#updateBtn').attr('studentId', result.studentId);

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
                let score = $.trim($('#rating').val());
                let session = $('#updateBtn').attr('session');
                let termId = $('#updateBtn').attr('termId');
                let studentId = $('#updateBtn').attr('studentId');
                let behaviouralRatingId = $('#behaviour').attr('ratingId');

                if (score == '') {
                    notify('Fields with asteriks (*) are required', 'warning');
                } else {
                    $('fieldset, .btn.action').prop('disabled', true);
                    btn.html('<i class="fa fa-circle-notch fa-spin"></i> Updating behavioural result...');
                    let url = $base + 'behaviouralRatings/UpdateBehaviouralResult';
                    let data = {
                        id,
                        score,
                        session,
                        termId,
                        studentId,
                        behaviouralRatingId
                    };
                    $.ajax({
                        type: 'POST',
                        url: url,
                        data: data,
                        success: (response) => {
                            if (response.isSuccess) {
                                notify(response.message + '.', 'success');
                                resultsTable.ajax.reload();
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
                    let url = $base + 'behaviouralRatings/BatchUploadRatings';

                    $.ajax({
                        type: 'POST',
                        url: url,
                        data: formData,
                        contentType: false,
                        cache: false,
                        processData: false,
                        success: (response) => {
                            if (response.isSuccess) {
                                //resultsTable.ajax.reload()
                                resultsTable.ajax.reload();
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
                reject('Invalid result id');
            } else {
                let url = $base + 'behaviouralRatings/GetBehaviouralResult/' + id;
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
