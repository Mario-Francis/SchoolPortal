var roleChoices;
var e_roleChoices
var classroomdd;
var usersTable;
var adminsTable;
var teachersTable;
var parentsTable;

$(() => {
    roleChoices = new Choices($('#roles')[0], {
        removeItemButton: true,
    });

    e_roleChoices = new Choices($('#e_roles')[0], {
        removeItemButton: true,
    });
    classroomdd = $('#classroom').selectize({
        dropdownParent:'body'
    });

    // initialize datatable
    usersTable = $('#usersTable').DataTable({
        serverSide: true,
        processing: true,
        ajax: {
            url: $base + 'users/UsersDataTable',
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
                    "filter": "FullName",
                    "display": "fullName"
                }, visible: true
            },
            {
                data: {
                    "filter": "Username",
                    "display": "username"
                }
            },
            {
                data: {
                    "filter": "Email",
                    "display": "email"
                }, "render": function (data, type, row, meta) {
                    return `${data} <br />${row.roles.map(r => getRoleBadge(r.id)).join('')}`;
                }
            },
            {
                data: {
                    "filter": "IsActive",
                    "display": "isActive"
                }, "render": function (data, type, row, meta) {
                    if (data) {
                        return `<spa class="badge badge-success badge-sm rounded-pill px-3 py-2">Active</span>`;
                    } else {
                        return `<spa class="badge badge-secondary badge-sm rounded-pill px-3 py-2">Inactive</span>`;
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
                }, orderData: 5
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
                }, orderData: 8
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
                        + `<a class="dropdown-item" href="#" uid="${row.id}">View Profile</a>`
                        + (row.roles.map(r => r.id).includes(Parent) ? `<a class="dropdown-item" href="#" uid="${row.id}">View Wards</a>` : '')
                        + (row.roles.map(r => r.id).includes(Teacher) ? `<a class="dropdown-item" href="#" uid="${row.id}">View ClassRoom</a>` : '')
                        + (row.roles.map(r => r.id).includes(Teacher) ? `<a class="dropdown-item assign-class" href="javascript:void(0)" uid="${row.id}">Assign Classroom</a>` : '')
                        + `<a class="dropdown-item" href="#" uid="${row.id}">View Login History</a>`
                        + `<div class="dropdown-divider"></div>`
                        + (!status ? `<a class="dropdown-item activate" href="javascript:void(0)" uid="${row.id}">Activate</a>` : '')
                        + (status ? `<a class="dropdown-item deactivate" href="javascript:void(0)" uid="${row.id}">Deactivate</a>` : '')
                        + `<a class="dropdown-item edit" href="javascript:void(0)" uid="${row.id}">Edit</a>`
                        + `<a class="dropdown-item delete" href="javascript:void(0)" uid="${row.id}">Delete</a>`
                        + '</div>'
                        + '</div>';
                }
            },
        ]
    });

    adminsTable = $('#adminsTable').DataTable({
        serverSide: true,
        processing: true,
        ajax: {
            url: $base + 'users/AdminsDataTable',
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
                    "filter": "FullName",
                    "display": "fullName"
                }, visible: true
            },
            {
                data: {
                    "filter": "Username",
                    "display": "username"
                }
            },
            {
                data: {
                    "filter": "Email",
                    "display": "email"
                }, "render": function (data, type, row, meta) {
                    return `${data} <br />${row.roles.map(r => getRoleBadge(r.id)).join('')}`;
                }
            },
            {
                data: {
                    "filter": "IsActive",
                    "display": "isActive"
                }, "render": function (data, type, row, meta) {
                    if (data) {
                        return `<spa class="badge badge-success badge-sm rounded-pill px-3 py-2">Active</span>`;
                    } else {
                        return `<spa class="badge badge-secondary badge-sm rounded-pill px-3 py-2">Inactive</span>`;
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
                }, orderData: 5
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
                }, orderData: 8
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
                        + `<a class="dropdown-item" href="#" uid="${row.id}">View Profile</a>`
                        + (row.roles.map(r => r.id).includes(Parent) ? `<a class="dropdown-item" href="#" uid="${row.id}">View Wards</a>` : '')
                        + (row.roles.map(r => r.id).includes(Teacher) ? `<a class="dropdown-item" href="#" uid="${row.id}">View ClassRoom</a>` : '')
                        + (row.roles.map(r => r.id).includes(Teacher) ? `<a class="dropdown-item assign-class" href="javascript:void(0)" uid="${row.id}">Assign Classroom</a>` : '')
                        + `<a class="dropdown-item" href="#" uid="${row.id}">View Login History</a>`
                        + `<div class="dropdown-divider"></div>`
                        + (!status ? `<a class="dropdown-item activate" href="javascript:void(0)" uid="${row.id}">Activate</a>` : '')
                        + (status ? `<a class="dropdown-item deactivate" href="javascript:void(0)" uid="${row.id}">Deactivate</a>` : '')
                        + `<a class="dropdown-item edit" href="javascript:void(0)" uid="${row.id}">Edit</a>`
                        + `<a class="dropdown-item delete" href="javascript:void(0)" uid="${row.id}">Delete</a>`
                        + '</div>'
                        + '</div>';
                }
            },
        ]
    });

    teachersTable = $('#teachersTable').DataTable({
        serverSide: true,
        processing: true,
        ajax: {
            url: $base + 'users/TeachersDataTable',
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
                    "filter": "FullName",
                    "display": "fullName"
                }, visible: true
            },
            {
                data: {
                    "filter": "Username",
                    "display": "username"
                }
            },
            {
                data: {
                    "filter": "Email",
                    "display": "email"
                }, "render": function (data, type, row, meta) {
                    return `${data} <br />${row.roles.map(r => getRoleBadge(r.id)).join('')}`;
                }
            },
            {
                data: {
                    "filter": "ClassRoom",
                    "display": "classRoom"
                }, "render": function (data, type, row, meta) {
                    return `${data.class} ${data.roomCode}`;
                }
            },
            {
                data: {
                    "filter": "IsActive",
                    "display": "isActive"
                }, "render": function (data, type, row, meta) {
                    if (data) {
                        return `<spa class="badge badge-success badge-sm rounded-pill px-3 py-2">Active</span>`;
                    } else {
                        return `<spa class="badge badge-secondary badge-sm rounded-pill px-3 py-2">Inactive</span>`;
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
                }, orderData: 6
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
                }, orderData: 9
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
                        + `<a class="dropdown-item" href="#" uid="${row.id}">View Profile</a>`
                        + (row.roles.map(r => r.id).includes(Parent) ? `<a class="dropdown-item" href="#" uid="${row.id}">View Wards</a>` : '')
                        + (row.roles.map(r => r.id).includes(Teacher) ? `<a class="dropdown-item" href="#" uid="${row.id}">View ClassRoom</a>` : '')
                        + (row.roles.map(r => r.id).includes(Teacher) ? `<a class="dropdown-item assign-class" href="javascript:void(0)" uid="${row.id}">Assign Classroom</a>` : '')
                        + `<a class="dropdown-item" href="#" uid="${row.id}">View Login History</a>`
                        + `<div class="dropdown-divider"></div>`
                        + (!status ? `<a class="dropdown-item activate" href="javascript:void(0)" uid="${row.id}">Activate</a>` : '')
                        + (status ? `<a class="dropdown-item deactivate" href="javascript:void(0)" uid="${row.id}">Deactivate</a>` : '')
                        + `<a class="dropdown-item edit" href="javascript:void(0)" uid="${row.id}">Edit</a>`
                        + `<a class="dropdown-item delete" href="javascript:void(0)" uid="${row.id}">Delete</a>`
                        + '</div>'
                        + '</div>';
                }
            },
        ]
    });

    parentsTable = $('#parentsTable').DataTable({
        serverSide: true,
        processing: true,
        ajax: {
            url: $base + 'users/ParentsDataTable',
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
                    "filter": "FullName",
                    "display": "fullName"
                }, visible: true
            },
            {
                data: {
                    "filter": "Username",
                    "display": "username"
                }
            },
            {
                data: {
                    "filter": "Email",
                    "display": "email"
                }, "render": function (data, type, row, meta) {
                    return `${data} <br />${row.roles.map(r => getRoleBadge(r.id)).join('')}`;
                }
            },
            {
                data: {
                    "filter": "WardCount",
                    "display": "wardCount"
                }, "render": function (data, type, row, meta) {
                    return data;
                }
            },
            {
                data: {
                    "filter": "IsActive",
                    "display": "isActive"
                }, "render": function (data, type, row, meta) {
                    if (data) {
                        return `<spa class="badge badge-success badge-sm rounded-pill px-3 py-2">Active</span>`;
                    } else {
                        return `<spa class="badge badge-secondary badge-sm rounded-pill px-3 py-2">Inactive</span>`;
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
                }, orderData: 6
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
                }, orderData: 9
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
                        + `<a class="dropdown-item" href="#" uid="${row.id}">View Profile</a>`
                        + (row.roles.map(r => r.id).includes(Parent) ? `<a class="dropdown-item" href="#" uid="${row.id}">View Wards</a>` : '')
                        + (row.roles.map(r => r.id).includes(Teacher) ? `<a class="dropdown-item" href="#" uid="${row.id}">View ClassRoom</a>` : '')
                        + (row.roles.map(r => r.id).includes(Teacher) ? `<a class="dropdown-item assign-class" href="javascript:void(0)" uid="${row.id}">Assign Classroom</a>` : '')
                        + `<a class="dropdown-item" href="#" uid="${row.id}">View Login History</a>`
                        + `<div class="dropdown-divider"></div>`
                        + (!status ? `<a class="dropdown-item activate" href="javascript:void(0)" uid="${row.id}">Activate</a>` : '')
                        + (status ? `<a class="dropdown-item deactivate" href="javascript:void(0)" uid="${row.id}">Deactivate</a>` : '')
                        + `<a class="dropdown-item edit" href="javascript:void(0)" uid="${row.id}">Edit</a>`
                        + `<a class="dropdown-item delete" href="javascript:void(0)" uid="${row.id}">Delete</a>`
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
                let firstName = $.trim($('#fname').val());
                let middleName = $.trim($('#mname').val());
                let surname = $.trim($('#sname').val());
                let roles = $('#roles').val();
                let email = $.trim($('#email').val());
                let gender = $.trim($('#gender').val());
                let dob = $.trim($('#dob').val());
                let phone = $.trim($('#phone').val());

                if (firstName == '' || surname == '' || roles.length == 0 || email == '' ||gender=='' || phone=='') {
                    notify('Fields with asteriks (*) are required', 'warning');
                } else {
                    $('fieldset').prop('disabled', true);
                    btn.html('<i class="fa fa-circle-notch fa-spin"></i> Adding user...');
                    let url = $base + 'users/AddUser';
                    let data = {
                        firstName,
                        middleName,
                        surname,
                        roles: roles.map(r => ({
                            id:r
                        })),
                        email,
                        gender,
                        dateOfBirth: dob,
                        phoneNumber:phone
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

    // on edit
    $(document).on('click', '.edit', async (e) => {
        let uid = $(e.currentTarget).attr('uid');
        let loader = bootLoaderDialog('Fetching user...');
        let user = null;
        try {
            user = await getUser(uid);
            loader.hide();

            $('#e_fname').val(user.firstName);
            $('#e_mname').val(user.middleName);
            $('#e_sname').val(user.surname);
            $('#e_dob').val(user.dateOfBirth?.split('T')[0]);
            $('#e_gender').val(user.gender);
            $('#e_phone').val(user.phoneNumber);
            $('#e_email').val(user.email);
            e_roleChoices.removeActiveItems();
            e_roleChoices.setChoiceByValue(user.roles.map(r => r.id.toString()))

            $('#updateBtn').attr('uid', uid);

            setTimeout(() => {
                $('#editModal').modal({ backdrop: 'static', keyboard: false }, 'show');
            }, 700);
        } catch (ex) {
            console.error(ex);
            notify(ex.message, 'danger');
            loader.hide();
        }
    });

    // on edit classroom assignment
    $(document).on('click', '.assign-class', async (e) => {
        let uid = $(e.currentTarget).attr('uid');
        let loader = bootLoaderDialog('Fetching user classroom...');
        let user = null;
        try {
            user = await getUser(uid);
            loader.hide();

            //$('#classroom').val(user.classRoom?.id);
            classroomdd[0].selectize.setValue(user.classRoom?.id);

            $('#assignBtn').attr('uid', uid);

            setTimeout(() => {
                $('#assignClassModal').modal({ backdrop: 'static', keyboard: false }, 'show');
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
        let uid = btn.attr('uid');
        try {
            let form = $("form")[1];
            if (validateForm(form)) {
                let firstName = $.trim($('#e_fname').val());
                let middleName = $.trim($('#e_mname').val());
                let surname = $.trim($('#e_sname').val());
                let roles = $('#e_roles').val();
                let email = $.trim($('#e_email').val());
                let gender = $.trim($('#e_gender').val());
                let dob = $.trim($('#e_dob').val());
                let phone = $.trim($('#e_phone').val());

                if (firstName == '' || surname == '' || roles.length == 0 || email == '' || gender == '' || phone == '') {
                    notify('Fields with asteriks (*) are required', 'warning');
                } else {
                    $('fieldset').prop('disabled', true);
                    btn.html('<i class="fa fa-circle-notch fa-spin"></i> Updating user...');
                    let url = $base + 'users/UpdateUser';
                    let data = {
                        id: uid,
                        firstName,
                        middleName,
                        surname,
                        roles: roles.map(r => ({
                            id: r
                        })),
                        email,
                        gender,
                        dateOfBirth: dob,
                        phoneNumber: phone
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

    // on assign class
    $('#assignBtn').on('click', async (e) => {
        e.preventDefault();
        let btn = $(e.currentTarget);
        let uid = btn.attr('uid');
        try {
            let form = $("form")[2];
            if (validateForm(form)) {
                let classRoomId = $.trim($('#classroom').val());
                $('fieldset').prop('disabled', true);
                if (classRoomId == '') {
                    btn.html('<i class="fa fa-circle-notch fa-spin"></i> Unassigning teacher from classroom...');
                } else {
                    btn.html('<i class="fa fa-circle-notch fa-spin"></i> Assigning teacher to cclassroom...');
                }

                var message = await assignClassroom(uid, classRoomId);
                refreshTables();
                notify(message + '.', 'success');

                form.reset();
                $('#assignClassModal').modal('hide');
                btn.html('<i class="fa fa-check-circle"></i> &nbsp;Submit');
                $('fieldset').prop('disabled', false);
                
            }
        } catch (ex) {
            if (ex != null) {
                console.error(ex);
                notify(ex, 'danger');
            }
            
            btn.html('<i class="fa fa-check-circle"></i> &nbsp;Submit');
            $('fieldset').prop('disabled', false);
        }
    });



    // on remove
    $(document).on('click', '.delete', async (e) => {
        let loader;
        let uid = $(e.currentTarget).attr('uid');
        bootConfirm('Are you sure you want to delete this user?', {
            title: 'Confirm Action', size: 'small', callback: async (res) => {
                if (res) {
                    try {
                        loader = bootLoaderDialog('Deleting user...');
                        let message = await deleteUser(uid);
                        loader.hide();

                        notify(message + '.', 'success');
                        refreshTables();
                    } catch (ex) {
                        loader.hide();
                        console.error(ex);
                        notify(ex + '.', 'danger');
                    }
                }
            }
        });
    });

    // on activate
    $(document).on('click', '.activate', async (e) => {
        let loader;
        let uid = $(e.currentTarget).attr('uid');
        bootConfirm('Are you sure you want to activate this user?', {
            title: 'Confirm Action', size: 'small', callback: async (res) => {
                if (res) {
                    try {
                        loader = bootLoaderDialog('Activating user...');
                        let message = await updateUserStatus(uid, true);
                        loader.hide();

                        notify(message + '.', 'success');
                        refreshTables();
                    } catch (ex) {
                        loader.hide();
                        console.error(ex);
                        if (ex != null) {
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
        let uid = $(e.currentTarget).attr('uid');
        bootConfirm('Are you sure you want to deactivate this user?', {
            title: 'Confirm Action', size: 'small', callback: async (res) => {
                if (res) {
                    try {
                        loader = bootLoaderDialog('Deactivating user...');
                        let message = await updateUserStatus(uid, false);
                        loader.hide();

                        notify(message + '.', 'success');
                        refreshTables();
                    } catch (ex) {
                        loader.hide();
                        console.error(ex);
                        if (ex != null) {
                            notify(ex + '.', 'danger');
                        }
                    }
                }
            }
        });
    });

});



function getUser(id) {
    var promise = new Promise((resolve, reject) => {
        try {
            if (id == undefined || id == '' || id == 0) {
                reject('Invalid user id');
            } else {
                let url = $base + 'users/GetUser/' + id;
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

function deleteUser(id) {
    var promise = new Promise((resolve, reject) => {
        try {
            if (id == undefined || id == '' || id == 0) {
                reject('Invalid user id');
            } else {
                let url = $base + 'users/DeleteUser/' + id;
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

function updateUserStatus(id, isActive = true) {
    var promise = new Promise((resolve, reject) => {
        try {
            if (id == undefined || id == '' || id == 0) {
                reject('Invalid user id');
            } else {
                let url = $base + 'users/UpdateUserStatus/' + id + '?isactive=' + isActive.toString();
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

function assignClassroom(id, roomId=null) {
    var promise = new Promise((resolve, reject) => {
        try {
            if (id == undefined || id == '' || id == 0) {
                reject('Invalid user id');
            } else {
                let url = $base + 'users/AssignClassRoom/' + id + '?roomId=' + roomId;
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

function getRoleBadge(role) {
    if (role == Admin) {
        return `<span class="badge bg-claret text-white badge-sm rounded-pill px-3 py-2 mx-1">Administrator</span>`;
    } else if (role == HeadTeacher) {
        return `<span class="badge bg-warning badge-sm rounded-pill px-3 py-2 mx-1">Head Teacher</span>`;
    } else if (role == Teacher) {
        return `<span class="badge bg-info text-white badge-sm rounded-pill px-3 py-2 mx-1">Teacher</span>`;
    } else if (role == Parent) {
        return `<span class="badge bg-success text-white badge-sm rounded-pill px-3 py-2 mx-1">Parent</span>`;
    } else {
        return '';
    }
}

function refreshTables() {
    usersTable.ajax.reload();
    adminsTable.ajax.reload();
    teachersTable.ajax.reload();
    parentsTable.ajax.reload();
}