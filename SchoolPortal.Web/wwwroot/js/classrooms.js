////var selectizedd;
$(() => {
    //selectizedd = initializeTeachersDropdown();
    // initialize datatable
    var classroomsTable = $('#classroomsTable').DataTable({
        serverSide: true,
        processing: true,
        ajax: {
            url: $base + 'classrooms/ClassRoomsDataTable',
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
                    "filter": "Class",
                    "display": "class"
                }, visible: true
            },
            {
                data: {
                    "filter": "RoomCode",
                    "display": "roomCode"
                }
            },
            //{
            //    data: {
            //        "filter": "Teacher",
            //        "display": "teacher"
            //    }
            //},
            {
                data: {
                    "filter": "IsActive",
                    "display": "isActive"
                }, "render": function (data, type, row, meta) {
                    if (data) {
                        return `<span class="badge badge-sm badge-success rounded-pill py-1 px-2">Active</span>`;
                    } else {
                        return `<span class="badge badge-sm badge-secondary rounded-pill py-1 px-2">Inactive</span>`;
                    }
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
                }, orderData: 4
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
                }, orderData: 7
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
                        + `<a class="dropdown-item" href="${$base}classrooms/${row.id}" cid="${row.id}">View</a>`
                        + `<div class="dropdown-divider"></div>`
                        + `<a class="dropdown-item edit" href="javascript:void(0)" cid="${row.id}">Edit</a>`
                        + (!status ? `<a class="dropdown-item activate" href="javascript:void(0)" cid="${row.id}">Activate</a>` : '')
                        + (status ? `<a class="dropdown-item deactivate" href="javascript:void(0)" cid="${row.id}">Deactivate</a>` : '')
                        + `<a class="dropdown-item delete" href="javascript:void(0)" cid="${row.id}">Delete</a>`
                        + '</div>'
                        + '</div>';
                }
            },
        ]
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
                let classId = $.trim($('#classId').val());
                let roomCode = $.trim($('#code').val());
                /*let teacherId = $('#teacherId').val();*/

                if (classId == '' || roomCode == '') {
                    notify('Fields with asteriks (*) are required', 'warning');
                } else {
                    $('fieldset').prop('disabled', true);
                    btn.html('<i class="fa fa-circle-notch fa-spin"></i> Adding classroom...');
                    let url = $base + 'classrooms/AddClassRoom';
                    let data = {
                        classId,
                        roomCode,
                       /* teacherId*/
                    };
                    $.ajax({
                        type: 'POST',
                        url: url,
                        data: data,
                        success: (response) => {
                            if (response.isSuccess) {
                                classroomsTable.ajax.reload();
                                notify(response.message + '.', 'success');

                                form.reset();
                               /* selectizedd[0].selectize.clear();*/
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
        let cid = $(e.currentTarget).attr('cid');
        let loader = bootLoaderDialog('Fetching classroom...');
        let classRoom = null;
        try {
            classRoom = await getClassRoom(cid);
            loader.hide();

            $('#e_classId').val(classRoom.classId);
            $('#e_code').val(classRoom.roomCode);
            //if (classRoom.teacherId != null) {
            //    $('#e_teacherId').val(classRoom.teacherId);
            //    $selectize = selectizedd[1].selectize;
            //    $selectize.addOption(classRoom.teacherObject);
            //    $selectize.addItem(classRoom.teacherId);
            //    $selectize.setValue(classRoom.teacherId);
            //}

            $('#updateBtn').attr('cid', cid);

            setTimeout(() => {
                $('#editModal').modal({ backdrop: 'static', keyboard: false }, 'show');
            }, 700);
        } catch (ex) {
            console.error(ex);
            notify(ex.message, 'danger');
            loader.hide();
        }
    });

    // on update
    $('#updateBtn').on('click', (e) => {
        e.preventDefault();
        let btn = $(e.currentTarget);
        let cid = btn.attr('cid');
        try {
            let form = $("form")[1];
            if (validateForm(form)) {
                let classId = $.trim($('#e_classId').val());
                let roomCode = $.trim($('#e_code').val());
                /*let teacherId = $('#e_teacherId').val();*/

                if (classId == '' || roomCode == '') {
                    notify('Fields with asteriks (*) are required', 'warning');
                } else {
                    $('fieldset').prop('disabled', true);
                    btn.html('<i class="fa fa-circle-notch fa-spin"></i> Updating classroom...');
                    let url = $base + 'classrooms/UpdateClassRoom';
                    let data = {
                        id: cid,
                        classId,
                        roomCode,
                       /* teacherId*/
                    };
                    $.ajax({
                        type: 'POST',
                        url: url,
                        data: data,
                        success: (response) => {
                            if (response.isSuccess) {
                                classroomsTable.ajax.reload();
                                notify(response.message + '.', 'success');

                                form.reset();
                               /* selectizedd[1].selectize.clear();*/
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
        let cid = $(e.currentTarget).attr('cid');
        bootConfirm('Are you sure you want to delete this classroom?', {
            title: 'Confirm Action', size: 'small', callback: async (res) => {
                if (res) {
                    try {
                        loader = bootLoaderDialog('Deleting classroom...');
                        let message = await deleteClassRoom(cid);
                        loader.hide();

                        notify(message + '.', 'success');
                        classroomsTable.ajax.reload();
                    } catch (ex) {
                        loader.hide();
                        console.error(ex);
                        if (typeof (ex) == 'string') {
                            notify(ex + '.', 'danger');
                        }
                    }
                }
            }
        });
    });

    // on activate
    $(document).on('click', '.activate', async (e) => {
        let loader;
        let cid = $(e.currentTarget).attr('cid');
        bootConfirm('Are you sure you want to activate this classroom?', {
            title: 'Confirm Action', size: 'small', callback: async (res) => {
                if (res) {
                    try {
                        loader = bootLoaderDialog('Activating classroom...');
                        let message = await updateClassRoomStatus(cid, true);
                        loader.hide();

                        notify(message + '.', 'success');
                        classroomsTable.ajax.reload();
                    } catch (ex) {
                        loader.hide();
                        console.error(ex);
                        if (typeof (ex) == 'string') {
                            notify(ex + '.', 'danger');
                        }
                    }
                }
            }
        });
    });

    // on deactivate
    $(document).on('click', '.deactivate', async (e) => {
        let loader;
        let cid = $(e.currentTarget).attr('cid');
        bootConfirm('Are you sure you want to deactivate this classroom?', {
            title: 'Confirm Action', size: 'small', callback: async (res) => {
                if (res) {
                    try {
                        loader = bootLoaderDialog('Deactivating classroom...');
                        let message = await updateClassRoomStatus(cid, false);
                        loader.hide();

                        notify(message + '.', 'success');
                        classroomsTable.ajax.reload();
                    } catch (ex) {
                        loader.hide();
                        console.error(ex);
                        if (typeof (ex) == 'string') {
                            notify(ex + '.', 'danger');
                        }
                    }
                }
            }
        });
    });

});



function getClassRoom(id) {
    var promise = new Promise((resolve, reject) => {
        try {
            if (id == undefined || id == '' || id == 0) {
                reject('Invalid classroom id');
            } else {
                let url = $base + 'classRooms/GetClassRoom/' + id;
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

function deleteClassRoom(id) {
    var promise = new Promise((resolve, reject) => {
        try {
            if (id == undefined || id == '' || id == 0) {
                reject('Invalid classroom id');
            } else {
                let url = $base + 'classrooms/DeleteClassRoom/' + id;
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

function updateClassRoomStatus(id, isActive=true) {
    var promise = new Promise((resolve, reject) => {
        try {
            if (id == undefined || id == '' || id == 0) {
                reject('Invalid classroom id');
            } else {
                let url = $base + 'classrooms/UpdateClassRoomStatus/' + id + '?isactive=' + isActive.toString();
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
