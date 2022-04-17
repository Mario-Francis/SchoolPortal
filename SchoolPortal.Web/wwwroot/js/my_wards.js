var userId = $('#userId').val();
$(() => {
    // initialize datatable
    wardsTable = $('#wardsTable').DataTable({
        serverSide: true,
        processing: true,
        ajax: {
            url: $base + 'users/WardsDataTable/' + userId,
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
                    "filter": "Relationship",
                    "display": "relationship"
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
                }, "render": function (data, type, row, meta) {
                    return `${data}`;
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
                }
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
                        + `<a class="dropdown-item" href="${$base}wardProfile/${row.studentId}" uid="${row.studentId}">View Profile</a>`
                        + `<a class="dropdown-item" href="${$base}classrooms/wardClassroom/${row.classRoomId}">View Classroom</a>`
                        + `<a class="dropdown-item" href="${$base}studentresults/${row.studentId}/view" uid="${row.studentId}">View Results</a>`
                        + '</div>'
                        + '</div>';
                }
            },
        ]
    });

});
