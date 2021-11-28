var roomId = $('#roomId').val();
$(() => {
    teachersTable = $('#teachersTable').DataTable({
        serverSide: true,
        processing: true,
        ajax: {
            url: $base + `classrooms/${roomId}/TeachersDataTable`,
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
                    return (data != null ? `${data.class} ${data.roomCode}` : '');
                }, visible:false
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
                        + `<a class="dropdown-item" href="${$base}users/${row.id}" uid="${row.id}">View Profile</a>`
                        + `<a class="dropdown-item" href="#" uid="${row.id}">View Login History</a>`
                       // + `<div class="dropdown-divider"></div>`
                        + '</div>'
                        + '</div>';
                }
            },
        ]
    });


    studentsTable = $('#studentsTable').DataTable({
        serverSide: true,
        processing: true,
        ajax: {
            url: $base + `classrooms/${roomId}/StudentsDataTable`,
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
                    return `${data}`;
                }
            },
            {
                data: {
                    "filter": "FormattedClassRoom",
                    "display": "formattedClassRoom"
                }, visible:false
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
                        + `<a class="dropdown-item" href="${$base}Students/${row.id}" uid="${row.id}">View Profile</a>`
                        + `<a class="dropdown-item" href="${$base}Students/${row.id}/Guardians" uid="${row.id}">View Guardians</a>`
                        + `<a class="dropdown-item" href="#" uid="${row.id}">View Login History</a>`
                        //+ `<div class="dropdown-divider"></div>`
                        + '</div>'
                        + '</div>';
                }
            },
        ]
    });

});

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