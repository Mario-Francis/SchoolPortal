const studentId = $('#studentId').val();
var midTermResultsTable, endTermResultsTable, sEndTermResultsTable;
var lastSearchedSession, lastSearchedTermId;

$(() => {
    // on session change
    $('#s_session').on('change', async (e) => {
        let session = $(e.currentTarget).val();
        //console.log(classId);
        if (session != '') {
            try {
                $('#termLoader').show();
                var terms = await getTerms(studentId, session);
                $('#termLoader').hide();
                var options = terms.map(t => `<option value="${t.id}">${t.name}</option>`);
                options.splice(0, 0, `<option value="">- Select term -</option>`);
                $('#s_term').html(options.join('')).prop('disabled', false);

            } catch (ex) {
                $('#termLoader').hide();
                console.log(ex);
                notify('Error encountered while fetching classrooms', 'danger')
            }
        } else {
            $('#s_term').val('').prop('disabled', true);
        }
    });
    $('#s_session').trigger('change');

    // search buttonn clicck
    $('#searchBtn').on('click', async (e) => {
        let loader;
        let session = $('#s_session').val();
        let termId = $('#s_term').val();
        if (session == '' || termId == '') {
            notify('Session and term is required.', 'warning');
        } else {
            try {
                loader = bootLoaderDialog('Fetching results...');
                var resultObj = await searchResult(studentId, session, termId);
                loader.hide();
                lastSearchedSession = session;
                lastSearchedTermId = termId;
                populateResultData(resultObj);
            } catch (ex) {
                loader.hide();
                console.log(ex);
                if (typeof ex === 'string') {
                    notify(ex);
                } else {
                    notify('Error encountered while fetching classrooms', 'danger')
                }

            }
        }

    });

    $('#closeBtn').on('click', (e) => {
        clearResult();
    });

    $('#m_classwork, #m_test, #m_exam').on('keyup change', (e) => {
        let classwork = $('#m_classwork').val() == '' ? 0 : parseInt($('#m_classwork').val());
        let test = $('#m_test').val() == '' ? 0 : parseInt($('#m_test').val());
        let exam = $('#m_exam').val() == '' ? 0 : parseInt($('#m_exam').val());
        $('#m_total').val(classwork + test + exam);
    });

    $('#e_classwork, #e_test, #e_exam').on('keyup change', (e) => {
        let classwork = $('#e_classwork').val() == '' ? 0 : parseInt($('#e_classwork').val());
        let test = $('#e_test').val() == '' ? 0 : parseInt($('#e_test').val());
        let exam = $('#e_exam').val() == '' ? 0 : parseInt($('#e_exam').val());
        $('#e_total').val(classwork + test + exam);
    });

    // mid term edit
    $(document).on('click', '.m_edit', async (e) => {
        let rid = $(e.currentTarget).attr('rid');
        let loader = bootLoaderDialog('Fetching result...');
        try {
            let result = await getMidTermResult(rid);
            loader.hide();

            $('#m_examId').val(result.examId);
            $('#m_classId').val(result.classId);
            $('#m_subjectId').val(result.subjectId);
            $('#m_studentId').val(result.studentId);
            $('#m_classwork').val(result.classWorkScore);
            $('#m_test').val(result.testScore);
            $('#m_exam').val(result.examScore);
            $('#m_total').val(result.total);
            $('.subject').html(result.subjectName);

            $('#updateMidTermResultBtn').attr('rid', rid);

            setTimeout(() => {
                $('#editMidTermModal').modal({ backdrop: 'static', keyboard: false }, 'show');
            }, 700);
        } catch (ex) {
            console.error(ex);
            notify(ex.message, 'danger');
            loader.hide();
        }
    });

    // end term edit
    $(document).on('click', '.e_edit', async (e) => {
        let rid = $(e.currentTarget).attr('rid');
        let loader = bootLoaderDialog('Fetching result...');
        try {
            let result = await getEndTermResult(rid);
            loader.hide();

            $('#e_examId').val(result.examId);
            $('#e_classId').val(result.classId);
            $('#e_subjectId').val(result.subjectId);
            $('#e_studentId').val(result.studentId);
            $('#e_classwork').val(result.classWorkScore);
            $('#e_test').val(result.testScore);
            $('#e_exam').val(result.examScore);
            $('#e_total').val(result.total);
            $('.subject').html(result.subjectName);

            $('#updateEndTermResultBtn').attr('rid', rid);

            setTimeout(() => {
                $('#editEndTermModal').modal({ backdrop: 'static', keyboard: false }, 'show');
            }, 700);
        } catch (ex) {
            console.error(ex);
            notify(ex.message, 'danger');
            loader.hide();
        }
    });


    // update midterm result
    $('#updateMidTermResultBtn').on('click', (e) => {
        e.preventDefault();
        let btn = $(e.currentTarget);
        let id = btn.attr('rid');
        try {
            let form = $("form")[5];
            if (validateForm(form)) {
                let examId = $.trim($('#m_examId').val());
                let classId = $.trim($('#m_classId').val());
                let subjectId = $.trim($('#m_subjectId').val());
                let studentId = $.trim($('#m_studentId').val());
                let classwork = $.trim($('#m_classwork').val());
                let test = $.trim($('#m_test').val());
                let exam = $.trim($('#m_exam').val());
                let total = $.trim($('#m_total').val());

                if (examId == '' || classId == '' || subjectId == '' || studentId == '' || classwork == ''
                    || test == '' || exam == '' || total == ''
                ) {
                    notify('Fields with asteriks (*) are required', 'warning');
                } else {
                    $('fieldset').prop('disabled', true);
                    btn.html('<i class="fa fa-circle-notch fa-spin"></i> Updating result...');
                    let url = $base + 'results/midterm/UpdateResult';
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
                                midTermResultsTable.ajax.reload();
                                //endTermResultsTable.ajax.reload();
                                if ($.fn.DataTable.isDataTable('#endTermResultsTable')) {
                                    endTermResultsTable.ajax.reload();
                                }
                                if ($.fn.DataTable.isDataTable('#sEndTermResultsTable')) {
                                    sEndTermResultsTable.ajax.reload();
                                }
                                form.reset();

                                $('#editMidTermModal').modal('hide');

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

    // update end term result
    $('#updateEndTermResultBtn').on('click', (e) => {
        e.preventDefault();
        let btn = $(e.currentTarget);
        let id = btn.attr('rid');
        try {
            let form = $("form")[6];
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
                                endTermResultsTable.ajax.reload();
                                if ($.fn.DataTable.isDataTable('#sEndTermResultsTable')) {
                                    sEndTermResultsTable.ajax.reload();
                                }

                                form.reset();

                                $('#editEndTermModal').modal('hide');

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

    $('#midSaveComment').on('click', async (e) => {
        e.preventDefault();
        let btn = $(e.currentTarget);
        let id = $('#midCommentId').val();
        let examId = $('#midExamId').val();
        try {
            let form = $("form")[1];
            if (validateForm(form)) {
                let tcomment = $.trim($('#midTeacherComment').val());
                let htcomment = $.trim($('#midHeadTeacherComment').val());

                if (tcomment == '' && htcomment == '') {
                    notify('At least one field must be filled', 'warning');
                } else {
                    $('fieldset').prop('disabled', true);
                    btn.html('<i class="fa fa-circle-notch fa-spin"></i> Saving comment...');

                    let data = {
                        id,
                        examId,
                        studentId,
                        teacherRemark: tcomment,
                        headTeacherRemark: htcomment
                    };
                    let message = await saveComment(data, id == '0' ? 'add' : 'update');
                    notify(message + '.', 'success');
                    form.reset();

                    btn.html('<i class="fa fa-check-circle"></i> &nbsp;Save Comments');
                    $('fieldset').prop('disabled', false);

                    $('#searchBtn').trigger('click');
                }
            }
        } catch (ex) {
            console.error(ex);
            if (ex != null) {
                if (typeof (ex) == 'string') {
                    notify(ex, 'danger');
                } else {
                    notify(ex.message, 'danger');
                }
            }

            btn.html('<i class="fa fa-check-circle"></i> &nbsp;Save Comments');
            $('fieldset').prop('disabled', false);
        }
    });

    $('#endSaveComment').on('click', async (e) => {
        e.preventDefault();
        let btn = $(e.currentTarget);
        let id = $('#endCommentId').val();
        let examId = $('#endExamId').val();
        try {
            let form = $("form")[4];
            if (validateForm(form)) {
                let tcomment = $.trim($('#endTeacherComment').val());
                let htcomment = $.trim($('#endHeadTeacherComment').val());

                if (tcomment == '' && htcomment == '') {
                    notify('At least one field must be filled', 'warning');
                } else {
                    $('fieldset').prop('disabled', true);
                    btn.html('<i class="fa fa-circle-notch fa-spin"></i> Saving comment...');

                    let data = {
                        id,
                        examId,
                        studentId,
                        teacherRemark: tcomment,
                        headTeacherRemark: htcomment
                    };
                    let message = await saveComment(data, id == '0' ? 'add' : 'update');
                    notify(message + '.', 'success');
                    form.reset();

                    btn.html('<i class="fa fa-check-circle"></i> &nbsp;Save Comments');
                    $('fieldset').prop('disabled', false);

                    $('#searchBtn').trigger('click');
                }
            }
        } catch (ex) {
            console.error(ex);
            if (ex != null) {
                if (typeof (ex) == 'string') {
                    notify(ex, 'danger');
                } else {
                    notify(ex.message, 'danger');
                }
            }

            btn.html('<i class="fa fa-check-circle"></i> &nbsp;Save Comments');
            $('fieldset').prop('disabled', false);
        }
    });

    $('#endSaveRecord').on('click', async (e) => {
        e.preventDefault();
        let btn = $(e.currentTarget);
        let id = $('#endRecordId').val();
        let session = lastSearchedSession;
        let termId = lastSearchedTermId;
        try {
            let form = $("form")[3];
            if (validateForm(form)) {
                let startHeight = $.trim($('#startHeight').val());
                let endHeight = $.trim($('#endHeight').val());
                let startWeight = $.trim($('#startWeight').val());
                let endWeight = $.trim($('#endWeight').val());

                if (startHeight == '' || endHeight == '' || startWeight == '' || endWeight == '') {
                    notify('All fields are required', 'warning');
                } else {
                    $('fieldset').prop('disabled', true);
                    btn.html('<i class="fa fa-circle-notch fa-spin"></i> Saving records...');

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
                    let message = await saveHealthRecords(data, id == '0' ? 'add' : 'update');
                    notify(message + '.', 'success');
                    form.reset();

                    btn.html('<i class="fa fa-check-circle"></i> &nbsp;Save Records');
                    $('fieldset').prop('disabled', false);

                    $('#searchBtn').trigger('click');
                }
            }
        } catch (ex) {
            console.error(ex);
            if (ex != null) {
                if (typeof (ex) == 'string') {
                    notify(ex, 'danger');
                } else {
                    notify(ex.message, 'danger');
                }
            }

            btn.html('<i class="fa fa-check-circle"></i> &nbsp;Save Records');
            $('fieldset').prop('disabled', false);
        }
    });

    $('#endSaveRatings').on('click', async (e) => {
        e.preventDefault();
        let btn = $(e.currentTarget);
        let session = lastSearchedSession;
        let termId = lastSearchedTermId;
        try {
            let form = $("form")[2];
            if (validateForm(form)) {

                let ratings = Array.from($('select[name="ratings[]"]')).map(e => ({
                    id: $(e).attr('resid'),
                    ratingId: $(e).attr('rid'),
                    score: $(e).val()
                }));

                if (ratings.some(r => r.score.trim() === '')) {
                    notify('All fields are required', 'warning');
                } else {
                    $('fieldset').prop('disabled', true);
                    btn.html('<i class="fa fa-circle-notch fa-spin"></i> Saving ratings...');

                    let data = {
                        session,
                        termId,
                        studentId,
                        ratings
                    };
                    let message = await saveBehaviouralRatings(data);
                    notify(message + '.', 'success');
                    form.reset();

                    btn.html('<i class="fa fa-check-circle"></i> &nbsp;Save Ratings');
                    $('fieldset').prop('disabled', false);

                    $('#searchBtn').trigger('click');
                }
            }
        } catch (ex) {
            console.error(ex);
            if (ex != null) {
                if (typeof (ex) == 'string') {
                    notify(ex, 'danger');
                } else {
                    notify(ex.message, 'danger');
                }
            }

            btn.html('<i class="fa fa-check-circle"></i> &nbsp;Save Ratings');
            $('fieldset').prop('disabled', false);
        }
    });

    $('#mExportBtn').on('click', e => {
        let url = $base + `studentResults/${studentId}/MidTermResult/Export?session=${lastSearchedSession}&termId=${lastSearchedTermId}`;
        location.assign(url);
    });
    $('#eExportBtn').on('click', e => {
        let url = $base + `studentResults/${studentId}/EndTermResult/Export?session=${lastSearchedSession}&termId=${lastSearchedTermId}`;
        location.assign(url);
    });
    $('#esExportBtn').on('click', e => {
        let url = $base + `studentResults/${studentId}/EndSessionResult/Export?session=${lastSearchedSession}&termId=${lastSearchedTermId}`;
        location.assign(url);
    });

});


function getTerms(id, session) {
    var promise = new Promise((resolve, reject) => {
        try {
            if (id == undefined || id == '' || id == 0) {
                reject('Invalid student id');
            } else if (session == '' || session == undefined) {
                reject('Session is required');
            } else {
                let url = $base + `StudentResults/${id}/GetSessionTerms?session=` + session;
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

function searchResult(id, session, termId) {
    var promise = new Promise((resolve, reject) => {
        try {
            if (id == undefined || id == '' || id == 0) {
                reject('Invalid student id');
            } else if (session == '' || session == undefined) {
                reject('Session is required');
            } else if (termId == undefined || termId == '' || termId == 0) {
                reject('Term is required');
            } else {
                let url = $base + `StudentResults/${id}/GetResult?session=${session}&termId=${termId}`;
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


function populateResultData(resultObj) {
    $('#resultsCard').slideUp(300, () => {
        clearResult();

        let midTerm = resultObj.midTermResult;
        let endTerm = resultObj.endTermResult;

        // set title
        let title = capitalize(`${midTerm.session} ${midTerm.term.name} Term Results`);
        $('#resultTitle').html(title);

        // populate midterm section
        populateMidTerm(midTerm);

        // popoulat endterm section
        populateEndTerm(endTerm);

        console.log(resultObj);
        $('#resultsCard').slideDown(300);
    });

}

// mid term section
function initializeMidTermDataTable() {
    if ($.fn.DataTable.isDataTable('#midTermResultsTable')) {
        midTermResultsTable.destroy();
    }
    midTermResultsTable = $('#midTermResultsTable').DataTable({
        serverSide: true,
        processing: true,
        ajax: {
            url: $base + `StudentResults/${studentId}/MidTermResultsDataTable?session=${lastSearchedSession}&termId=${lastSearchedTermId}`,
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
                    "filter": "Id",
                    "display": "id"
                }, "orderable": false, "render": function (data, type, row, meta) {
                    return '<div class="dropdown f14">'
                        + '<button type="button" class="btn px-3 f12" data-toggle="dropdown">'
                        + '<i class="fa fa-ellipsis-v"></i>'
                        + '</button>'
                        + '<div class="dropdown-menu f14">'
                        // + `<a class="dropdown-item" href="#" cid="${row.id}">View Classrooms</a>`
                        // + `<div class="dropdown-divider"></div>`
                        + `<a class="dropdown-item m_edit" href="javascript:void(0)" rid="${row.id}">Edit</a>`
                        //+ `<a class="dropdown-item delete" href="javascript:void(0)" cid="${row.id}">Delete</a>`
                        + '</div>'
                        + '</div>';
                }
            },
            {
                data: {
                    "filter": "SubjectName",
                    "display": "subjectName"
                }, visible: true
            },
            {
                data: {
                    "filter": "ClassWorkScore",
                    "display": "classWorkScore"
                }
            },
            {
                data: {
                    "filter": "TestScore",
                    "display": "testScore"
                }
            },
            {
                data: {
                    "filter": "ExamScore",
                    "display": "examScore"
                }
            },
            {
                data: {
                    "filter": "Total",
                    "display": "total"
                }
            },
            {
                data: {
                    "filter": "Grade",
                    "display": "grade"
                }
            }

        ]
    }).on('xhr.dt', (e, settings, json, xhr) => {
        if (json != null) {
            console.log(json);
            $('#midTmoSpan').html(json.totalScoreObtained);
            $('#midTmoSpan2').html(json.totalScoreObtainable);
            $('#midAvgSpan').html(json.percentage);
            $('#midAvgGradeSpan').html(json.percentageGrade);
        }
    });


}

function populateMidTerm(data) {
    // data table
    $('#midSessionSpan').html(data.session);
    $('#midTermSpan').html(data.term.name);
    $('#midClassSpan').html(`${data.classRoom.class} ${data.classRoom.roomCode}`);
    $('#midExamId').val(data.exam.id);
    initializeMidTermDataTable();

    // comment
    if (data.resultComment != null) {
        $('#midCommentId').val(data.resultComment.id);
        $('#midTeacherComment').html(data.resultComment.teacherComment);
        $('#midHeadTeacherComment').html(data.resultComment.headTeacherComment);
    } else {
        $('#midCommentId').val('0');
    }
}

// end term section
function initializeEndTermDataTable() {
    if ($.fn.DataTable.isDataTable('#endTermResultsTable')) {
        endTermResultsTable.destroy();
    }
    endTermResultsTable = $('#endTermResultsTable').DataTable({
        serverSide: true,
        processing: true,
        ajax: {
            url: $base + `StudentResults/${studentId}/EndTermResultsDataTable?session=${lastSearchedSession}&termId=${lastSearchedTermId}`,
            type: "POST"
        },
        "order": [[2, "asc"]],
        "lengthMenu": [10, 20],
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
                    "filter": "Id",
                    "display": "id"
                }, "orderable": false, "render": function (data, type, row, meta) {
                    return '<div class="dropdown f14">'
                        + '<button type="button" class="btn px-3 f12" data-toggle="dropdown">'
                        + '<i class="fa fa-ellipsis-v"></i>'
                        + '</button>'
                        + '<div class="dropdown-menu f14">'
                        // + `<a class="dropdown-item" href="#" cid="${row.id}">View Classrooms</a>`
                        // + `<div class="dropdown-divider"></div>`
                        + `<a class="dropdown-item e_edit" href="javascript:void(0)" rid="${row.id}">Edit</a>`
                        //+ `<a class="dropdown-item delete" href="javascript:void(0)" cid="${row.id}">Delete</a>`
                        + '</div>'
                        + '</div>';
                }
            },
            {
                data: {
                    "filter": "SubjectName",
                    "display": "subjectName"
                }, visible: true
            },
            {
                data: {
                    "filter": "MidTermTotal",
                    "display": "midTermTotal"
                }
            },
            {
                data: {
                    "filter": "ClassWorkScore",
                    "display": "classWorkScore"
                }
            },
            {
                data: {
                    "filter": "TestScore",
                    "display": "testScore"
                }
            },
            {
                data: {
                    "filter": "ExamScore",
                    "display": "examScore"
                }
            },
            {
                data: {
                    "filter": "TermTotal",
                    "display": "termTotal"
                }
            },
            {
                data: {
                    "filter": "Grade",
                    "display": "grade"
                }
            }

        ]
    }).on('xhr.dt', (e, settings, json, xhr) => {
        if (json != null) {
            console.log(json);
            $('#endTmoSpan').html(json.totalScoreObtained);
            $('#endTmoSpan2').html(json.totalScoreObtainable);
            $('#endAvgSpan').html(json.percentage);
            $('#endAvgGradeSpan').html(json.percentageGrade);
        }
    });


}
function initializeEndOfSessionDataTable() {
    if ($.fn.DataTable.isDataTable('#sEndTermResultsTable')) {
        sEndTermResultsTable.destroy();
    }
    sEndTermResultsTable = $('#sEndTermResultsTable').DataTable({
        serverSide: true,
        processing: true,
        ajax: {
            url: $base + `StudentResults/${studentId}/EndOfSessionResultsDataTable?session=${lastSearchedSession}&termId=${lastSearchedTermId}`,
            type: "POST"
        },
        "order": [[2, "asc"]],
        "lengthMenu": [10, 20],
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
                    "filter": "Id",
                    "display": "id"
                }, "orderable": false, "render": function (data, type, row, meta) {
                    return '<div class="dropdown f14">'
                        + '<button type="button" class="btn px-3 f12" data-toggle="dropdown">'
                        + '<i class="fa fa-ellipsis-v"></i>'
                        + '</button>'
                        + '<div class="dropdown-menu f14">'
                        // + `<a class="dropdown-item" href="#" cid="${row.id}">View Classrooms</a>`
                        // + `<div class="dropdown-divider"></div>`
                        + `<a class="dropdown-item edit" href="javascript:void(0)" cid="${row.id}">Edit</a>`
                        + `<a class="dropdown-item delete" href="javascript:void(0)" cid="${row.id}">Delete</a>`
                        + '</div>'
                        + '</div>';
                }
            },
            {
                data: {
                    "filter": "SubjectName",
                    "display": "subjectName"
                }, visible: true
            },
            {
                data: {
                    "filter": "MidTermTotal",
                    "display": "midTermTotal"
                }
            },
            {
                data: {
                    "filter": "ClassWorkScore",
                    "display": "classWorkScore"
                }
            },
            {
                data: {
                    "filter": "TestScore",
                    "display": "testScore"
                }
            },
            {
                data: {
                    "filter": "ExamScore",
                    "display": "examScore"
                }
            },
            {
                data: {
                    "filter": "TermTotal",
                    "display": "termTotal"
                }
            },
            {
                data: {
                    "filter": "FirstTermTotal",
                    "display": "firstTermTotal"
                }
            },
            {
                data: {
                    "filter": "SecondTermTotal",
                    "display": "secondTermTotal"
                }
            },
            {
                data: {
                    "filter": "AverageScore",
                    "display": "averageScore"
                }
            },
            {
                data: {
                    "filter": "Grade",
                    "display": "grade"
                }
            }

        ]
    }).on('xhr.dt', (e, settings, json, xhr) => {
        if (json != null) {
            console.log(json);
            $('#sEndTmoSpan').html(json.totalScoreObtained);
            $('#sEndTmoSpan2').html(json.totalScoreObtainable);
            $('#sEndAvgSpan').html(json.percentage);
            $('#sEndAvgGradeSpan').html(json.percentageGrade);
        }
    });


}


function populateEndTerm(data) {
    if (data == null) {
        $('#nav-end-tab').addClass('disabled').hide();
    } else {
        $('#nav-end-tab').removeClass('disabled').show();
        if (data.term.id != 3) {
            $('#v-pills-session-tab').addClass('disabled').hide();
        } else {
            $('#v-pills-session-tab').removeClass('disabled').show();
        }

        // data table
        $('#endSessionSpan').html(data.session);
        $('#endTermSpan').html(data.term.name);
        $('#endClassSpan').html(`${data.classRoom.class} ${data.classRoom.roomCode}`);
        $('#endExamId').val(data.exam.id);
        initializeEndTermDataTable();

        // end of session
        if (data.term.id == 3) {
            $('#sEndSessionSpan').html(data.session);
            $('#sEndTermSpan').html(data.term.name);
            $('#sEndClassSpan').html(`${data.classRoom.class} ${data.classRoom.roomCode}`);
            $('#sEndExamId').val(data.exam.id);
            initializeEndOfSessionDataTable();
        }

        // end term comment
        if (data.resultComment != null) {
            $('#endCommentId').val(data.resultComment.id);
            $('#endTeacherComment').html(data.resultComment.teacherComment);
            $('#endHeadTeacherComment').html(data.resultComment.headTeacherComment);
        } else {
            $('#endCommentId').val('0');
        }

        // health record
        if (data.healthRecord != null) {
            $('#endRecordId').val(data.healthRecord.id);
            $('#startHeight').html(data.healthRecord.startHeight);
            $('#endHeight').html(data.healthRecord.endHeight);
            $('#startWeight').html(data.healthRecord.startWeight);
            $('#endWeight').html(data.healthRecord.endWeight);
        } else {
            $('#endRecordId').val('0');
        }

        //behavioural ratings
        if (data.behaviouralResults.length > 0) {
            data.behaviouralResults.forEach((r, i) => {
                $(`#rating_${r.behaviouralRatingId}`).attr('resid', r.id).html(r.score);
            });
        } else {
            $('p[name="ratings[]"]').attr('resid', '0').html('---');
        }
    }
}

function clearResult() {
    // clear ratings
    $('select[name="ratings[]"]').attr('resid', '0').html('---');

    // clear health records
    $('#endRecordId').val(0);
    $('#startHeight').html('---');
    $('#endHeight').html('---');
    $('#startWeight').html('---');
    $('#endWeight').html('---');

    // clear end term comment
    $('#endCommentId').val(0);
    $('#endTeacherComment').html('---');
    $('#endHeadTeacherComment').html('---');

    // clear ed of session table
    $('#sEndSessionSpan').html('');
    $('#sEndTermSpan').html('');
    $('#sEndClassSpan').html('');
    $('#sEndExamId').val('');
    if ($.fn.DataTable.isDataTable('#sEndTermResultsTable')) {
        sEndTermResultsTable.destroy();
    }

    // clear end term table
    $('#endSessionSpan').html('');
    $('#endTermSpan').html('');
    $('#endClassSpan').html('');
    $('#endExamId').val('');
    if ($.fn.DataTable.isDataTable('#endTermResultsTable')) {
        endTermResultsTable.destroy();
    }

    // clear mid term comment
    $('#midCommentId').val(0);
    $('#midTeacherComment').html('---');
    $('#midHeadTeacherComment').html('---');

    // clear mid term table
    $('#midSessionSpan').html('');
    $('#midTermSpan').html('');
    $('#midClassSpan').html('');
    $('#midExamId').val('');
    if ($.fn.DataTable.isDataTable('#midTermResultsTable')) {
        midTermResultsTable.destroy();
    }

    $('#resultsCard').slideUp(300);
}




function saveComment(comment = null, type = 'update') {
    var promise = new Promise((resolve, reject) => {
        try {
            if (comment == null) {
                reject('Comment object is required');
            } else {
                let url = $base + (type == 'update' ? 'remarks/updateRemark' : 'remarks/addRemark');
                $.ajax({
                    type: 'POST',
                    url: url,
                    data: comment,
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

function saveBehaviouralRatings(ratings = null, type = 'update') {
    var promise = new Promise((resolve, reject) => {
        try {
            if (ratings == null) {
                reject('Ratings object is required');
            } else {
                let url = $base + (type == 'update' ? 'behaviouralRatings/UpdateBehaviouralResults' : 'behaviouralRatings/AddBehaviouralResults');
                $.ajax({
                    type: 'POST',
                    url: url,
                    data: ratings,
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

function saveHealthRecords(record = null, type = 'update') {
    var promise = new Promise((resolve, reject) => {
        try {
            if (record == null) {
                reject('Record object is required');
            } else {
                let url = $base + (type == 'update' ? 'healthRecords/updateRecord' : 'healthRecords/addRecord');
                $.ajax({
                    type: 'POST',
                    url: url,
                    data: record,
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
function getMidTermResult(id) {
    var promise = new Promise((resolve, reject) => {
        try {
            if (id == undefined || id == '' || id == 0) {
                reject('Invalid result id');
            } else {
                let url = $base + 'results/midterm/GetResult/' + id;
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

function getEndTermResult(id) {
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