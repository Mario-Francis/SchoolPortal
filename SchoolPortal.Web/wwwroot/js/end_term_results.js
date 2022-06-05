var classdd;
var selectizedd;
$(() => {
    selectizedd = initializeStudentsDropdown();
    //classdd = $('#classId').selectize({
    //    dropdownParent: 'body'
    //});
    var exclude = [0, 15,16,17,18,19,20];
    $('#resultsTable tfoot th').each(function (i, v) {
        if (!exclude.includes(i)) {
            var title = $(this).text();
            $(this).html('<input type="text" class="form-control bg-light f12" style="min-width:64px;" placeholder="\u{2315} ' + title + '" />');
        } else {
            $(this).html('');
        }
    });
    // initialize datatable
    resultsTable = $('#resultsTable').DataTable({
        serverSide: true,
        processing: true,
        ajax: {
            url: $base + 'results/endterm/ResultsDataTable',
            type: "POST"
        },
        "order": [[2, "asc"]],
        "lengthMenu": [10, 20, 30, 50, 100],
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
                    "filter": "MidTermTotal",
                    "display": "midTermTotal"
                }, "render": function (data, type, row, meta) {
                    return `${data ?? '---'}`;
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
                    "filter": "TermTotal",
                    "display": "termTotal"
                }, "render": function (data, type, row, meta) {
                    return `${data}`;
                }
            },
            {
                data: {
                    "filter": "Grade",
                    "display": "grade"
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
                }, orderData: 15
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
                }, orderData: 18
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
                        + `<a class="dropdown-item view-exam" href="javascript:void(0)" rid="${row.id}">View Exam Details</a>`
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
        $('#uploadBtn').attr('force', false);
        $('#batchUploadModal').modal({ backdrop: 'static', keyboard: false }, 'show');
    });

    $('.classdd').on('change', async (e) => {
        await updateSubjectdd($(e.currentTarget));
    });

    $('#classwork, #test, #exam').on('keyup change', (e) => {
        let classwork = $('#classwork').val() == '' ? 0 : parseInt($('#classwork').val());
        let test = $('#test').val() == '' ? 0 : parseInt($('#test').val());
        let exam = $('#exam').val() == '' ? 0 : parseInt($('#exam').val());
        $('#total').val(classwork + test + exam);
    });

    $('#e_classwork, #e_test, #e_exam').on('keyup change', (e) => {
        let classwork = $('#e_classwork').val() == '' ? 0 : parseInt($('#e_classwork').val());
        let test = $('#e_test').val() == '' ? 0 : parseInt($('#e_test').val());
        let exam = $('#e_exam').val() == '' ? 0 : parseInt($('#e_exam').val());
        $('#e_total').val(classwork + test + exam);
    });

    // on add
    $('#createBtn').on('click', (e) => {
        e.preventDefault();
        let btn = $(e.currentTarget);
        try {
            let form = $("form")[0];
            if (validateForm(form)) {
                let examId = $.trim($('#examId').val());
                let classId = $.trim($('#classId').val());
                let subjectId = $.trim($('#subjectId').val());
                let studentId = $.trim($('#studentId').val());
                let classwork = $.trim($('#classwork').val());
                let test = $.trim($('#test').val());
                let exam = $.trim($('#exam').val());
                let total = $.trim($('#total').val());

                if (examId == '' || classId == '' || subjectId == '' || studentId == '' || classwork == ''
                    || test == '' || exam == '' || total == ''
                ) {
                    notify('Fields with asteriks (*) are required', 'warning');
                } else {
                    $('fieldset').prop('disabled', true);
                    btn.html('<i class="fa fa-circle-notch fa-spin"></i> Adding result...');
                    let url = $base + 'results/endterm/AddResult';
                    let data = {
                        examId,
                        classId,
                        subjectId,
                        studentId,
                        classWorkScore:classwork,
                        testScore:test,
                        examScore:exam,
                        total
                    };
                    $.ajax({
                        type: 'POST',
                        url: url,
                        data: data,
                        success: (response) => {
                            if (response.isSuccess) {
                                resultsTable.ajax.reload();
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

    // edit
    $(document).on('click', '.edit', async (e) => {
        let rid = $(e.currentTarget).attr('rid');
        let loader = bootLoaderDialog('Fetching result...');
        try {
            let result = await getResult(rid);
            loader.hide();

            $('#e_examId').val(result.examId);
            $('#e_classId').val(result.classId);
            await updateSubjectdd($('#e_classId'));
            $('#e_subjectId').val(result.subjectId);
            $('#e_studentId').val(result.studentId);
            $('#e_classwork').val(result.classWorkScore);
            $('#e_test').val(result.testScore);
            $('#e_exam').val(result.examScore);
            $('#e_total').val(result.total);

            $selectize = selectizedd[1].selectize;
            $selectize.addOption(result.student);
            $selectize.addItem(result.studentId);
            $selectize.setValue(result.studentId);

            $('#updateBtn').attr('rid', rid);

            setTimeout(() => {
                $('#editModal').modal({ backdrop: 'static', keyboard: false }, 'show');
            }, 700);
        } catch (ex) {
            console.error(ex);
            notify(ex.message, 'danger');
            loader.hide();
        }
    });

    // update
    $('#updateBtn').on('click', (e) => {
        e.preventDefault();
        let btn = $(e.currentTarget);
        let id = btn.attr('rid');
        try {
            let form = $("form")[1];
            if (validateForm(form)) {
                let examId = $.trim($('#e_examId').val());
                let classId = $.trim($('#e_classId').val());
                let subjectId = $.trim($('#e_subjectId').val());
                let studentId = $.trim($('#e_studentId').val());
                let classwork = $.trim($('#e_classwork').val());
                let test = $.trim($('#e_test').val());
                let exam = $.trim($('#e_exam').val());
                let total = $.trim($('#e_total').val());

                if (examId == '' || classId == '' || subjectId == '' || studentId == '' || classwork == ''
                    || test == '' || exam == '' || total == ''
                ) {
                    notify('Fields with asteriks (*) are required', 'warning');
                } else {
                    $('fieldset').prop('disabled', true);
                    btn.html('<i class="fa fa-circle-notch fa-spin"></i> Updating result...');
                    let url = $base + 'results/endterm/UpdateResult';
                    let data = {
                        id,
                        examId,
                        classId,
                        subjectId,
                        studentId,
                        classWorkScore: classwork,
                        testScore: test,
                        examScore: exam,
                        total
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
        let rid = $(e.currentTarget).attr('rid');
        bootConfirm('Are you sure you want to delete this result?', {
            title: 'Confirm Action', size: 'small', callback: async (res) => {
                if (res) {
                    try {
                        loader = bootLoaderDialog('Deleting result...');
                        let message = await deleteResult(rid);
                        loader.hide();

                        notify(message + '.', 'success');
                        resultsTable.ajax.reload();
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
            let form = $("form")[2];
            if (validateForm(form)) {
                let force = btn.attr('force');
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
                    formData.append('force', force);

                    $('fieldset').prop('disabled', true);
                    btn.html('<i class="fa fa-circle-notch fa-spin"></i> Uploading file...');
                    let url = $base + 'results/endterm/BatchUploadResults';

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

    // edit
    $(document).on('click', '.view-exam', async (e) => {
        let rid = $(e.currentTarget).attr('rid');
        let loader = bootLoaderDialog('Fetching exam details...');
        try {
            let result = await getResult(rid);
            loader.hide();
            $('#eType').html(result.exam.examType);
            $('#eTerm').html(result.exam.term + ' Term');
            $('#eSession').html(result.exam.session);
            $('#eStartDate').html(result.exam.formattedStartDate);
            $('#eEndDate').html(result.exam.formattedEndDate);

            setTimeout(() => {
                $('#examModal').modal({ backdrop: 'static', keyboard: false }, 'show');
            }, 700);
        } catch (ex) {
            console.error(ex);
            notify(ex.message, 'danger');
            loader.hide();
        }
    });

});

function uploadForStudentsWithoutMidTerm(){
    bootConfirm('Kindly ensure the results you wish to upload are strictly for students that resumed in the <b>2nd half</b> of the term. Not adhering to this can compromise the integrity of the records on this system. <br /><br />Do you still wish to continue?', {
        title: 'Confirm Action', size: 'medium', callback: async (res) => {
            if (res) {
                $('#uploadBtn').attr('force', true);
                $('#batchUploadModal').modal({ backdrop: 'static', keyboard: false }, 'show');
            }
        }
    });
}

async function updateSubjectdd(classdd) {
    var classId = classdd.val();
    var subjectdd = $('#' + classdd.attr('subjectdd'));
    var loader = $('#' + classdd.attr('loader'));
    //console.log(classId);
    if (classId != '') {
        try {
            loader.show(); subjectdd.prop('disabled', true);
            var subjects = await getSubjects(classId);
            loader.hide(); subjectdd.prop('disabled', false);
            var options = subjects.map(s => `<option value="${s.id}">${s.name} ${s.code == null ? '' : `(${s.code})`}</option>`);
            options.splice(0, 0, `<option value="">- Select subject -</option>`);
            subjectdd.html(options.join('')).prop('disabled', false);

        } catch (ex) {
            loader.hide();
            console.log(ex);
            notify('Error encountered while fetching subjects', 'danger')
        }
    } else {
        subjectdd.val('').prop('disabled', true);
    }
}

function getResult(id) {
    var promise = new Promise((resolve, reject) => {
        try {
            if (id == undefined || id == '' || id == 0) {
                reject('Invalid result id');
            } else {
                let url = $base + 'results/endterm/GetResult/' + id;
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
                let url = $base + 'results/endterm/DeleteResult/' + id;
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


function initializeStudentsDropdown() {
    var _select = $(".studentsdd").selectize({
        valueField: "id",
        searchField: ["email", "username", "firstName", "surname", "middleName", "phoneNumber", "admissionNo"],
        placeholder: '- Search student -',
        dropdownParent: 'body',
        create: false,
        preload: 'focus',
        render: {
            option: function (item, escape) {
                return (
                    `<div class="d-flex flex-row px-3 py-2 border-top bg-white">
                        <div>
                            <div class="rounded-circle mt-2 mr-3 bg-claret" style="height:36px;width:36px;padding-top:8px;">
                                <p class="m-0 f14 text-center text-white">${getInitial(item, escape)}</p>
                            </div>
                        </div>
                        <div class="flex-fill">
                            <p class="f14 font-weight-bold text-dark">${capitalize(escape(item.firstName).trim())} ${capitalize(escape(item.middleName).trim())} ${capitalize(escape(item.surname).trim())}</p>
                            <p class="f12" style="margin-top:-6px;">${escape(item.email).trim()}</p>
                            <p class="f10" style="margin-top:-2px;">${escape(item.admissionNo).trim()} | ${escape(item.class).trim()}</p>
                        </div>
                    </div>`
                );
            },
            item: function (item, escape) {
                return (
                    `<div class="d-flex flex-row px-3 py-1">
                        <div>
                            <div class="rounded-circle mt-2 mr-3 bg-claret" style="height:36px;width:36px;padding-top:8px;">
                                <p class="m-0 f14 text-center text-white">${getInitial(item, escape)}</p>
                            </div>
                        </div>
                        <div class="flex-fill">
                            <p class="f14 font-weight-bold text-dark mt-1">${capitalize(escape(item.firstName).trim())} ${capitalize(escape(item.middleName).trim())} ${capitalize(escape(item.surname).trim())}</p>
                            <p class="f12" style="margin-top:-6px;">${escape(item.email).trim()}</p>
                            <p class="f10" style="margin-top:-2px;">${escape(item.admissionNo).trim()} | ${escape(item.class).trim()}</p>
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