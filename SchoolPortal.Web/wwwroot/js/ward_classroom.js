var roomId = $('#roomId').val();
var courseWorksTable;
$(() => {
    // initialize datatable
    courseWorksTable = $('#courseWorksTable').DataTable({
        serverSide: true,
        processing: true,
        ajax: {
            url: $base + 'CourseWorks/ClassRoomCourseWorksDataTable/' + roomId,
            type: "POST"
        },
        "order": [[0, "desc"]],
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
                    "filter": "Title",
                    "display": "title"
                }, visible: true,
                render: function (data, type, row, meta) {
                    let ext = row.filePath.split('.').pop();
                    return `${getDocIcon(ext)} ${data}`;
                }
            },
            {
                data: {
                    "filter": "Description",
                    "display": "description"
                }
            },
            {
                data: {
                    "filter": "WeekNo",
                    "display": "weekNo"
                }
            },
            {
                data: {
                    "filter": "From",
                    "display": "from"
                }, visible: false
            },
            {
                data: {
                    "filter": "FormattedFrom",
                    "display": "formattedFrom"
                }, orderData: 4
            },
            {
                data: {
                    "filter": "To",
                    "display": "to"
                }, visible: false
            },
            {
                data: {
                    "filter": "FormattedTo",
                    "display": "formattedTo"
                }, orderData: 6
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
                        + `<a class="dropdown-item" download href="${$base}${row.filePath}">Download</a>`
                        + '</div>'
                        + '</div>';
                }
            },
        ]
    });

});

function getDocIcon(ext) {
    switch (ext) {
        case 'pdf':
            return '<i class="fa fa-file-pdf f18 pr-2" style="color:#ff0000;"></i>';
            break;
        case 'doc':
        case 'docx':
            return '<i class="fa fa-file-word f18 pr-2" style="color:#2b579a;"></i>';
            break;
        case 'xls':
        case 'xlsx':
            return '<i class="fa fa-file-excel f18 pr-2" style="color:#217346;"></i>';
            break;
        case 'txt':
            return '<i class="fa fa-file-alt f18 pr-2" style="color:#444;"></i>';
            break;
        case 'ppt':
        case 'pptx':
            return '<i class="fa fa-file-powerpoint f18 pr-2" style="color:#d24726;"></i>';
            break;
        default:
            return '<i class="fa fa-file f18 pr-2" style="color:#444;"></i>';
            break;
    }
}