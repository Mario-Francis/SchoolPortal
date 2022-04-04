$(() => {
    // initialize datatable
    var assessmentsTable = $('#assessmentsTable').DataTable({
        serverSide: true,
        processing: true,
        ajax: {
            url: $base + 'assessments/AssessmentsDataTable',
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
                    "filter": "AssessmentType",
                    "display": "assessmentType"
                }, visible: true
            },
            {
                data: {
                    "filter": "FromDate",
                    "display": "fromDate"
                }, visible:false
            },
            {
                data: {
                    "filter": "FormattedFromDate",
                    "display": "formattedFromDate"
                }, orderData: 2
            },
            {
                data: {
                    "filter": "ToDate",
                    "display": "toDate"
                }, visible:false
            },
            {
                data: {
                    "filter": "FormattedToDate",
                    "display": "formattedToDate"
                }, orderData: 4
            },
            {
                data: {
                    "filter": "Deadline",
                    "display": "deadline"
                }, visible:false
            },
            {
                data: {
                    "filter": "FormattedDeadline",
                    "display": "formattedDeadline"
                }, orderData: 6
            },
            {
                data: {
                    "filter": "AssessmentStatusId",
                    "display": "assessmentStatusId"
                }, "render": function (data, type, row, meta) {
                    return getAssessmentStatus(data);
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
                    "filter": "Id",
                    "display": "id"
                }, "orderable": false, "render": function (data, type, row, meta) {
                    let status = row.assessmentStatusId;
                    return '<div class="dropdown f14">'
                        + '<button type="button" class="btn px-3" data-toggle="dropdown">'
                        + '<i class="fa fa-ellipsis-v"></i>'
                        + '</button>'
                        + '<div class="dropdown-menu f14">'
                        + (status == PENDING ? `<a class="dropdown-item start" href="javascript:void(0)" aid="${row.id}">Start</a>` : '')
                        + (status == IN_PROGRESS ? `<a class="dropdown-item suspend" href="javascript:void(0)" aid="${row.id}">Suspend</a>` : '')
                        + (status == SUSPENDED ? `<a class="dropdown-item resume" href="javascript:void(0)" aid="${row.id}">Resume</a>` : '')
                        + (status == IN_PROGRESS || data == SUSPENDED ? `<a class="dropdown-item end" href="javascript:void(0)" aid="${row.id}">End</a>`:'')
                        + `<div class="dropdown-divider"></div>`
                        + `<a class="dropdown-item edit" href="javascript:void(0)" aid="${row.id}">Edit</a>`
                        + (status == PENDING ? `<a class="dropdown-item delete" href="javascript:void(0)" aid="${row.id}">Delete</a>`:'')
                        + '</div>'
                        + '</div>';
                }
            },
        ]
    }).on('preXhr.dt', (e, settings, data) => {
        //data.sessionId = $('#sessionId').val();
        //data.startDate = $('#start').val() == '' ? null : $('#start').val();
        //data.endDate = $('#end').val() == '' ? null : $('#end').val();
    });

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
                let assessmentTypeId = $.trim($('#assessmentTypeId').val());
                let fromDate = $('#fromDate').val();
                let toDate = $('#toDate').val();
                let deadline = $('#deadline').val();

                if (assessmentTypeId == '' || fromDate == '' || toDate == '' || deadline=='') {
                    notify('All fields are required', 'warning');
                } else {
                    $('fieldset').prop('disabled', true);
                    btn.html('<i class="fa fa-circle-notch fa-spin"></i> Adding assessment...');
                    let url = $base + 'assessments/AddAssessment';
                    let data = {
                        assessmentTypeId,
                        fromDate,
                        toDate,
                        deadline
                    };
                    $.ajax({
                        type: 'POST',
                        url: url,
                        data: data,
                        success: (response) => {
                            if (response.isSuccess) {
                                assessmentsTable.ajax.reload();
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
                            let res = req.responseJSON;
                            if (req.status == 401) {
                                notify(res.message, 'danger', "Unauthorized");
                            } else if (req.status == 400) {
                                let eItems = '';
                                if (res.errorItems != null) {
                                    eItems = '<ul>';
                                    res.errorItems.forEach((v, i) => {
                                        eItems += `<li>${i + 1}. ${v}</li>`;
                                    });
                                    eItems += '</ul>';
                                }
                                notify(res.message + eItems, 'danger', "Validation Error");
                            } else if (req.status == 500) {
                                notify(res.message, 'danger');
                                console.log(res.errorDetail)
                            } else {
                                notify('Something went wrong while submitting your request. Please refresh your browser and try again.', 'danger');
                                console.error(req);
                            }
                            btn.html('<i class="fa fa-check-circle"></i> &nbsp;Submit');
                            $('fieldset').prop('disabled', false);
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
        let aid = $(e.currentTarget).attr('aid');
        let loader = bootLoaderDialog('Fetching assessment...');
        let assessment = await getAssessment(aid);
        loader.hide();

        $('#e_assessmentTypeId').val(assessment.assessmentTypeId);
        $('#e_fromDate').val(assessment.fromDate.split('T')[0]);
        $('#e_toDate').val(assessment.toDate.split('T')[0]);
        $('#e_deadline').val(assessment.deadline.split('T')[0]);

        $('#updateBtn').attr('aid', aid);

        setTimeout(() => {
            $('#editModal').modal({ backdrop: 'static', keyboard: false }, 'show');
        }, 700);
    });

    // on update
    $('#updateBtn').on('click', (e) => {
        e.preventDefault();
        let btn = $(e.currentTarget);
        let aid = btn.attr('aid');
        try {
            let form = $("form")[1];
            if (validateForm(form)) {
                let assessmentTypeId = $.trim($('#e_assessmentTypeId').val());
                let fromDate = $('#e_fromDate').val();
                let toDate = $('#e_toDate').val();
                let deadline = $('#e_deadline').val();

                if (assessmentTypeId == '' || fromDate == '' || toDate == '' || deadline == '') {
                    notify('All fields are required', 'warning');
                } else {
                    $('fieldset').prop('disabled', true);
                    btn.html('<i class="fa fa-circle-notch fa-spin"></i> Updating assessment...');
                    let url = $base + 'assessments/UpdateAssessment';
                    let data = {
                        id: aid,
                        assessmentTypeId,
                        fromDate,
                        toDate,
                        deadline
                    };
                    $.ajax({
                        type: 'POST',
                        url: url,
                        data: data,
                        success: (response) => {
                            if (response.isSuccess) {
                                assessmentsTable.ajax.reload();
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
                            let res = req.responseJSON;
                            if (req.status == 401) {
                                notify(res.message, 'danger', "Unauthorized");
                            } else if (req.status == 400) {
                                let eItems = '';
                                if (res.errorItems != null) {
                                    eItems = '<ul>';
                                    res.errorItems.forEach((v, i) => {
                                        eItems += `<li>${i + 1}. ${v}</li>`;
                                    });
                                    eItems += '</ul>';
                                }
                                notify(res.message + eItems, 'danger', "Validation Error");
                            } else if (req.status == 500) {
                                notify(res.message, 'danger');
                                console.log(res.errorDetail)
                            } else {
                                notify('Something went wrong while submitting your request. Please refresh your browser and try again.', 'danger');
                                console.error(req);
                            }
                            btn.html('<i class="fa fa-check-circle"></i> &nbsp;Update');
                            $('fieldset').prop('disabled', false);
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
        let aid = $(e.currentTarget).attr('aid');
        bootConfirm('Are you sure you want to delete assessment?', {
            title: 'Confirm Action', size: 'small', callback: async (res) => {
                if (res) {
                    try {

                        loader = bootLoaderDialog('Deleting assessment...');
                        let message = await deleteAssessment(aid);
                        loader.hide();

                        notify(message + '.', 'success');
                        assessmentsTable.ajax.reload();
                    } catch (ex) {
                        loader.hide();
                        console.error(ex);
                        notify(ex + '.', 'danger');
                    }
                }
            }
        });

    });

    // on start
    $(document).on('click', '.start', async (e) => {
        let loader;
        let aid = $(e.currentTarget).attr('aid');
        bootConfirm('Are you sure you want to start assessment?', {
            title: 'Confirm Action', size: 'small', callback: async (res) => {
                if (res) {
                    try {

                        loader = bootLoaderDialog('Starting assessment...');
                        let message = await changeAssessmentState(aid, 'start');
                        loader.hide();

                        notify(message + '.', 'success');
                        assessmentsTable.ajax.reload();
                    } catch (ex) {
                        loader.hide();
                        console.error(ex);
                        notify(ex + '.', 'danger');
                    }
                }
            }
        });
    });

    // on suspend
    $(document).on('click', '.suspend', async (e) => {
        let loader;
        let aid = $(e.currentTarget).attr('aid');
        bootConfirm('Are you sure you want to suspend assessment?', {
            title: 'Confirm Action', size: 'small', callback: async (res) => {
                if (res) {
                    try {

                        loader = bootLoaderDialog('Suspending assessment...');
                        let message = await changeAssessmentState(aid, 'suspend');
                        loader.hide();

                        notify(message + '.', 'success');
                        assessmentsTable.ajax.reload();
                    } catch (ex) {
                        loader.hide();
                        console.error(ex);
                        notify(ex + '.', 'danger');
                    }
                }
            }
        });
    });

    // on suspend
    $(document).on('click', '.resume', async (e) => {
        let loader;
        let aid = $(e.currentTarget).attr('aid');
        bootConfirm('Are you sure you want to resume assessment?', {
            title: 'Confirm Action', size: 'small', callback: async (res) => {
                if (res) {
                    try {

                        loader = bootLoaderDialog('Resuming assessment...');
                        let message = await changeAssessmentState(aid, 'resume');
                        loader.hide();

                        notify(message + '.', 'success');
                        assessmentsTable.ajax.reload();
                    } catch (ex) {
                        loader.hide();
                        console.error(ex);
                        notify(ex+'.', 'danger');
                    }
                }
            }
        });
    });

    // on end
    $(document).on('click', '.end', async (e) => {
        let loader;
        let aid = $(e.currentTarget).attr('aid');
        bootConfirm('Are you sure you want to end assessment?', {
            title: 'Confirm Action', size: 'small', callback: async (res) => {
                if (res) {
                    try {

                        loader = bootLoaderDialog('Ending assessment...');
                        let message = await changeAssessmentState(aid, 'end');
                        loader.hide();

                        notify(message + '.', 'success');
                        assessmentsTable.ajax.reload();
                    } catch (ex) {
                        loader.hide();
                        console.error(ex);
                        notify(ex, 'danger');
                    }
                }
            }
        });
    });

});



function getAssessment(id) {
    var promise = new Promise((resolve, reject) => {
        try {
            if (id == undefined || id == '' || id == 0) {
                reject('Invalid assessment id');
            } else {
                let url = $base + 'assessments/GetAssessment/' + id;
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
                        let res = req.responseJSON;
                        if (req.status == 400) {
                            let eItems = '<ul>';
                            res.errorItems.forEach((v, i) => {
                                eItems += `<li>${i + 1}. ${v}</li>`;
                            });
                            eItems += '</ul>';
                            //notify(res.message + eItems, 'danger', "Validation Error");
                            reject(res.message);
                        } else if (req.status == 500) {
                            //notify(res.message, 'danger');
                            console.log(res.errorDetail);
                            reject(res.message);
                        } else {
                            reject('Something went wrong! Please try again.');
                            //notify('Something went wrong while submitting your request. Please refresh your browser and try again.', 'danger');
                            console.error(req);
                        }
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

function deleteAssessment(id) {
    var promise = new Promise((resolve, reject) => {
        try {
            if (id == undefined || id == '' || id == 0) {
                reject('Invalid assessment id');
            } else {
                let url = $base + 'assessments/DeleteAssessment/' + id;
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
                        let res = req.responseJSON;
                        if (req.status == 400) {
                            let eItems = '';
                            if (res.errorItems != null) {
                                eItems = '<ul>';
                                res.errorItems.forEach((v, i) => {
                                    eItems += `<li>${i + 1}. ${v}</li>`;
                                });
                                eItems += '</ul>';
                            }
                            reject(res.message + eItems);
                            //notify(res.message + eItems, 'danger', "Validation Error");
                        } else if (req.status == 500) {
                            //notify(res.message, 'danger');
                            console.log(res.errorDetail);
                            reject(res.message);
                        } else {
                            reject('Something went wrong! Please try again.');
                            //notify('Something went wrong while submitting your request. Please refresh your browser and try again.', 'danger');
                            console.error(req);
                        }
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

function changeAssessmentState(id, action) {
    var promise = new Promise((resolve, reject) => {
        try {
            if (id == undefined || id == '' || id == 0) {
                reject('Invalid assessment id');
            } else {
                let url = $base + `assessments/${action}Assessment/` + id;
                $.ajax({
                    type: 'POST',
                    url: url,
                    success: (response) => {
                        if (response.isSuccess) {
                            resolve(response.message);
                        } else {
                            reject(response.message);
                        }
                    },
                    error: (req, status, err) => {
                        let res = req.responseJSON;
                        if (req.status == 400) {
                            let eItems = '';
                            if (res.errorItems != null) {
                                eItems = '<ul>';
                                res.errorItems.forEach((v, i) => {
                                    eItems += `<li>${i + 1}. ${v}</li>`;
                                });
                                eItems += '</ul>';
                            }
                            reject(res.message + eItems);
                            //notify(res.message + eItems, 'danger', "Validation Error");
                        } else if (req.status == 500) {
                            //notify(res.message, 'danger');
                            console.log(res.errorDetail);
                            reject(res.message);
                        } else {
                            reject('Something went wrong! Please try again.');
                            //notify('Something went wrong while submitting your request. Please refresh your browser and try again.', 'danger');
                            console.error(req);
                        }
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


function getAssessmentStatus(status) {
    if (status == 1) {
        return `<span class="badge badge-secondary px-2 py-1">Pending</span>`;
    } else if (status == 2) {
        return `<span class="badge badge-success px-2 py-1">In Progress</span>`;
    } else if (status == 3) {
        return `<span class="badge badge-danger px-2 py-1">Suspended</span>`;
    } else if (status == 4) {
        return `<span class="badge badge-info px-2 py-1">Ended</span>`;
    } else {
        return status;
    }
}