var studentId = $('#studentId').val();
$(() => {
    // on edit
    $('#editBtn').on('click', async (e) => {
        let loader = bootLoaderDialog('Fetching student...');
        try {
            let student = await getStudent(studentId);
            loader.hide();

            $('#fname').val(student.firstName);
            $('#mname').val(student.middleName);
            $('#sname').val(student.surname);
            $('#gender').val(student.gender);
            $('#dob').val(student.dateOfBirth.split('T')[0]);
            $('#email').val(student.email);
            $('#phone').val(student.phoneNumber);
            $('#enrollmentDate').val(student.enrollmentDate.split('T')[0]);
            $('#admissionNo').val(student.admissionNo);
            $('#entryClassId').val(student.entryClassId);
            $('#entryTermId').val(student.entryTermId);
            $('#entrySession').val(student.entrySession);
            $('#classId').val(student.classId);
            await updateClassroomdd(student.classId);
            $('#roomId').val(student.classRoomId);
           
            setTimeout(() => {
                $('#editModal').modal({ backdrop: 'static', keyboard: false }, 'show');
            }, 700);
        } catch (ex) {
            console.error(ex);
            notify(ex.message, 'danger');
            loader.hide();
        }
    });

    $('#classId').on('change', async (e) => {
        var classId = $('#classId').val();
        //console.log(classId);
        if (classId != '') {
            await updateClassroomdd(classId);
        } else {
            $('#roomId').html('<option value="">- Select classroom -</option>').val('').prop('disabled', true);
        }
    });

    // on update
    $('#updateBtn').on('click', (e) => {
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
                    || entrySession == '' || classId == '' || roomId == ''
                ) {
                    notify('Fields with asteriks (*) are required', 'warning');
                } else {
                    $('fieldset').prop('disabled', true);
                    btn.html('<i class="fa fa-circle-notch fa-spin"></i> Updating student...');
                    let url = $base + 'students/UpdateStudent';
                    let data = {
                        id: studentId,
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
                                notify(response.message + '.<br /><i class="fa fa-circle-notch fa-spin"></i> &nbsp;Refreshing...', 'success');
                                form.reset();

                                $('#editModal').modal('hide');

                                setTimeout(() => {
                                    location.reload();
                                }, 2500);

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


});

async function updateClassroomdd(classId) {
    try {
        $('#roomLoader').show();
        var classrooms = await getClassrooms(classId);
        $('#roomLoader').hide();
        var options = classrooms.map(c => `<option value="${c.id}">${c.class} ${c.roomCode}</option>`);
        options.splice(0, 0, `<option value="">- Select classroom -</option>`);
        $('#roomId').html(options.join('')).prop('disabled', false);

    } catch (ex) {
        $('#roomLoader').hide();
        console.log(ex);
        notify('Error encountered while fetching classrooms', 'danger')
    }
}


function getStudent(id) {
    var promise = new Promise((resolve, reject) => {
        try {
            if (id == undefined || id == '' || id == 0) {
                reject('Invalid student id');
            } else {
                let url = $base + 'students/GetStudent/' + id;
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
function getClassrooms(id) {
    var promise = new Promise((resolve, reject) => {
        try {
            if (id == undefined || id == '' || id == 0) {
                reject('Invalid class id');
            } else {
                let url = $base + 'Classrooms/GetClassRooms/' + id;
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