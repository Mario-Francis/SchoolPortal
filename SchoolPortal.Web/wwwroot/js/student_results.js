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
                        "filter": "Total",
                        "display": "total"
                    }
                }
               
            ]
        }).on('xhr.dt', (e, settings, json, xhr) => {
            if (json != null) {
                console.log(json);
                $('#midTmoSpan').html(json.totalScoreObtained);
                $('#midTmoSpan2').html(json.totalScoreObtainable);
                $('#midAvgSpan').html(json.percentage);
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
        $('#midCommentId').val(resultComment.id);
        $('#midTeacherComment').val(resultComment.teacherComment);
        $('#midHeadTeacherComment').val(resultComment.headTeacherComment);
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
                    "filter": "TermTotal",
                    "display": "termTotal"
                }
            }

        ]
    }).on('xhr.dt', (e, settings, json, xhr) => {
        if (json != null) {
            console.log(json);
            $('#endTmoSpan').html(json.totalScoreObtained);
            $('#endTmoSpan2').html(json.totalScoreObtainable);
            $('#endAvgSpan').html(json.percentage);
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
                    "filter": "AverageScore",
                    "display": "averageScore"
                }
            }

        ]
    }).on('xhr.dt', (e, settings, json, xhr) => {
        if (json != null) {
            console.log(json);
            $('#sEndTmoSpan').html(json.totalScoreObtained);
            $('#sEndTmoSpan2').html(json.totalScoreObtainable);
            $('#sEndAvgSpan').html(json.percentage);
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
            $('#endCommentId').val(resultComment.id);
            $('#endTeacherComment').val(resultComment.teacherComment);
            $('#endHeadTeacherComment').val(resultComment.headTeacherComment);
        } else {
            $('#endCommentId').val('0');
        }

        // health record
        if (data.healthRecord != null) {
            $('#endRecordId').val(healthRecord.id);
            $('#startHeight').val(healthRecord.startHeight);
            $('#endHeight').val(healthRecord.endHeight);
            $('#startWeight').val(healthRecord.startWeight);
            $('#endWeight').val(healthRecord.endWeight);
        } else {
            $('#endRecordId').val('0');
        }

        //behavioural ratings
        if (data.behaviouralResults.length > 0) {
            data.behaviouralResults.forEach((r, i) => {
                $(`#rating_${r.behaviouralRatingId}`).attr('resid', r.id).val(r.score);
            });
        } else {
            $('select[name="ratings[]"]').attr('resid', '0').val('');
        }
    }
}

function clearResult() {
    // clear ratings
    $('select[name="ratings[]"]').attr('resid', '0').val('');

    // clear health records
    $('#endRecordId').val(0);
    $('#startHeight').val('');
    $('#endHeight').val('');
    $('#startWeight').val('');
    $('#endWeight').val('');

    // clear end term comment
    $('#endCommentId').val(0);
    $('#endTeacherComment').val('');
    $('#endHeadTeacherComment').val('');

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
    $('#midTeacherComment').val('');
    $('#midHeadTeacherComment').val('');

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

function getMidTermResult() {

}

function updateMidTermResult() {

}

function getEndTermResult() {

}

function updateEndTermResult() {

}

function saveComment() {

}

function saveBehaviouralRatings() {

}

function saveHealthRecords() {

}