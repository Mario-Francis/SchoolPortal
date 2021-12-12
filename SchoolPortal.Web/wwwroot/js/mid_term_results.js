var classdd;

$(() => {
    //classdd = $('#classId').selectize({
    //    dropdownParent: 'body'
    //});

    // initialize datatable
    resultsTable = $('#resultsTable').DataTable({
        serverSide: true,
        processing: true,
        ajax: {
            url: $base + 'results/midterm/ResultsDataTable',
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
                    "filter": "Student",
                    "display": "student"
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
                     "filter": "Term",
                     "display": "term"
                }
            },
             {
                data: {
                     "filter": "ExamType",
                     "display": "examType"
                 }, visible:false
            },
             {
                data: {
                     "filter": "Class",
                     "display": "class"
                }
            },
             {
                data: {
                     "filter": "RoomCode",
                     "display": "roomCode"
                }
            },
             {
                data: {
                     "filter": "Subject",
                     "display": "subject"
                }
            },
            {
                data: {
                    "filter": "ClassWorkScore",
                    "display": "classWorkScore"
                }, "render": function (data, type, row, meta) {
                    return `${data}`;
                }
            },
            {
                data: {
                    "filter": "TestScore",
                    "display": "testScore"
                }, "render": function (data, type, row, meta) {
                    return `${data}`;
                }
            },
            {
                data: {
                    "filter": "ExamScore",
                    "display": "examScore"
                }, "render": function (data, type, row, meta) {
                    return `${data}`;
                }
            },
            {
                data: {
                    "filter": "Total",
                    "display": "total"
                }, "render": function (data, type, row, meta) {
                    return `${data}`;
                }
            },
            {
                data: {
                    "filter": "Total",
                    "display": "total"
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
                }, orderData: 14
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
                }, orderData: 17
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
                        + `<a class="dropdown-item" href="${$base}Students/${row.id}" rid="${row.id}">View Exam Details</a>`
                        + `<div class="dropdown-divider"></div>`
                        + `<a class="dropdown-item edit" href="javascript:void(0)" rid="${row.id}">Edit</a>`
                        + `<a class="dropdown-item delete" href="javascript:void(0)" rid="${row.id}">Delete</a>`
                        + '</div>'
                        + '</div>';
                }
            },
        ]
    });



    $('#addBtn').on('click', (e) => {
        $('#addModal').modal({ backdrop: 'static', keyboard: false }, 'show');
    });
    $('#batchAddBtn').on('click', (e) => {
        $('#batchUploadModal').modal({ backdrop: 'static', keyboard: false }, 'show');
    });

    $('#b_classId').on('change', async (e) => {
        var classId = $('#b_classId').val();
        //console.log(classId);
        if (classId != '') {
            try {
                $('#subjectLoader').show();
                var subjects = await getSubjects(classId);
                $('#subjectLoader').hide();
                var options = subjects.map(s => `<option value="${s.id}">${s.name} ${s.code==null?'':`(${s.code})`}</option>`);
                options.splice(0, 0, `<option value="">- Select subject -</option>`);
                $('#b_subjectId').html(options.join('')).prop('disabled', false);

            } catch (ex) {
                $('#subjectLoader').hide();
                console.log(ex);
                notify('Error encountered while fetching subjects', 'danger')
            }
        } else {
            $('#b_subjectId').val('').prop('disabled', true);
        }
    });


    // on add
    $('#createBtn').on('click', (e) => {
        e.preventDefault();
        let btn = $(e.currentTarget);
        try {
            let form = $("form")[0];
            if (validateForm(form)) {
                let firstName = $.trim($('#fname').val());
                let middleName = $.trim($('#mname').val());
                let surname = $.trim($('#sname').val());
                let email = $.trim($('#email').val());
                let gender = $.trim($('#gender').val());
                let dob = $.trim($('#dob').val());
                let phone = $.trim($('#phone').val());
                let enrollmentDate = $.trim($('#enrollmentDate').val());
                let admissionNo = $.trim($('#admissionNo').val());
                let entryClassId = $.trim($('#entryClassId').val());
                let entryTermId = $.trim($('#entryTermId').val());
                let entrySession = $.trim($('#entrySession').val());
                let classId = $.trim($('#classId').val());
                let roomId = $.trim($('#roomId').val());

                if (firstName == '' || middleName == '' || surname == '' || email == '' || gender == ''
                    || enrollmentDate == '' || admissionNo == '' || entryClassId == '' || entryTermId == ''
                    || entrySession == ''
                ) {
                    notify('Fields with asteriks (*) are required', 'warning');
                } else {
                    $('fieldset').prop('disabled', true);
                    btn.html('<i class="fa fa-circle-notch fa-spin"></i> Adding student...');
                    let url = $base + 'students/AddStudent';
                    let data = {
                        firstName,
                        middleName,
                        surname,
                        email,
                        gender,
                        dateOfBirth: dob,
                        phoneNumber: phone,
                        enrollmentDate,
                        admissionNo,
                        entryClassId,
                        entryTermId,
                        entrySession,
                        classId,
                        classRoomId: roomId
                    };
                    $.ajax({
                        type: 'POST',
                        url: url,
                        data: data,
                        success: (response) => {
                            if (response.isSuccess) {
                                refreshTables();
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

    // edit

    // update

    // on remove
    $(document).on('click', '.delete', async (e) => {
        let loader;
        let rid = $(e.currentTarget).attr('rid');
        bootConfirm('Are you sure you want to delete this student?', {
            title: 'Confirm Action', size: 'small', callback: async (res) => {
                if (res) {
                    try {
                        loader = bootLoaderDialog('Deleting student...');
                        let message = await deleteStudent(rid);
                        loader.hide();

                        notify(message + '.', 'success');
                        refreshTables();
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


    // batch upload students
    $('#uploadBtn').on('click', (e) => {
        e.preventDefault();
        let btn = $(e.currentTarget);
        try {
            let form = $("form")[1];
            if (validateForm(form)) {
                let examId = $('#b_examId').val();
                let classId = $('#b_classId').val();
                let subjectId = $('#b_subjectId').val();
                let files = $('#file')[0].files;

                if (examId == '' || classId == '' || subjectId == '') {
                    notify('All fields with asteriks (*) are required.', 'warning');
                }
                else if (files.length == 0) {
                    notify('No file selected! Kindly select a valid excel file.', 'warning');
                } else {
                    let formData = new FormData();
                    formData.append('examId', examId);
                    formData.append('classId', classId);
                    formData.append('subjectId', subjectId);
                    formData.append('file', files[0]);

                    $('fieldset').prop('disabled', true);
                    btn.html('<i class="fa fa-circle-notch fa-spin"></i> Uploading file...');
                    let url = $base + 'results/midterm/BatchUploadResults';

                    $.ajax({
                        type: 'POST',
                        url: url,
                        data: formData,
                        contentType: false,
                        cache: false,
                        processData: false,
                        success: (response) => {
                            if (response.isSuccess) {
                                //refreshTables();
                                resultsTable.ajax.reload();
                                notify(response.message + '.', 'success');

                                form.reset();
                                $('#batchUploadModal').modal('hide');
                            } else {
                                notify(response.message + '.', 'danger');
                            }
                            btn.html('<i class="fa fa-upload"></i> &nbsp;Upload File');
                            $('fieldset').prop('disabled', false);
                        },
                        error: (req, status, err) => {
                            ajaxErrorHandler(req, status, err, {
                                callback: () => {
                                    btn.html('<i class="fa fa-upload"></i> &nbsp;Upload File');
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
            btn.html('<i class="fa fa-upload"></i> &nbsp;Upload File');
            $('fieldset').prop('disabled', false);
        }
    });

});



function getResult(id) {
    var promise = new Promise((resolve, reject) => {
        try {
            if (id == undefined || id == '' || id == 0) {
                reject('Invalid result id');
            } else {
                let url = $base + 'results/GetResult/' + id;
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

function deleteResult(id) {
    var promise = new Promise((resolve, reject) => {
        try {
            if (id == undefined || id == '' || id == 0) {
                reject('Invalid result id');
            } else {
                let url = $base + 'results/DeleteResult/' + id;
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






function getSubjects(id) {
    var promise = new Promise((resolve, reject) => {
        try {
            if (id == undefined || id == '' || id == 0) {
                reject('Invalid class id');
            } else {
                let url = $base + 'Subjects/GetSubjects/' + id;
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
                        ajaxErrorHandler(req, status, err, {
                            callback: () => {
                                reject(null);
                            }
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
