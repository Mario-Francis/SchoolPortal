const allowedExtensions = ['jpeg', 'jpg', 'png'];
var cropper, imageType, studentId = null;

$(() => {
    studentId = $('#studentId').val();
    $('#file').on('change', e => {
        let file = $(e.currentTarget)[0].files[0];
        console.log(file);
        let arr = file.name.split('.');
        if (arr.length == 0) {
            notify('Invalid file selected!', 'danger');
        } else {
            let ext = arr[arr.length - 1];
            if (!allowedExtensions.includes(ext)) {
                notify(`Invalid file selected! Only ${allowedExtensions.join(', ')} files are supported`, 'danger');
                $('#file').val(null);
            } else if (file.size > (maxUploadsize * 1024 * 1024)) {
                notify(`Max upload file size of ${maxUploadsize}MB exceeded`, 'danger');
                $('#file').val(null);
            } else {
                var reader = new FileReader();
                reader.onload = function () {
                    imageType = file.type;
                    $("#cropperImg").attr("src", reader.result);
                    $('#editPhotoModal').modal({ backdrop: 'static', keyboard: false }, 'show');
                }
                reader.readAsDataURL(file);
            }
        }
        $('#file').val(null);
    });

    // on crop modal open
    $('#editPhotoModal').on('shown.bs.modal', function (event) {
        cropper = new Cropper($('#cropperImg')[0], {
            aspectRatio: 1 / 1,
            viewMode: 2,
            dragMode: 'move',
            minCropBoxWidth: 200,
            maxCropBoxWidth: 200
        });
    });

    $('#editPhotoModal').on('hidden.bs.modal', function (event) {
        cropper.destroy();
    });
    $('#cropperImg').on('zoom', (event) => {
        if (event.detail.ratio > 1) {
            event.preventDefault();
        }
    });

    $('#uploadBtn').on('click', async e => {
        let btn = $(e.currentTarget);
        let file = await getCroppedImageFile();
        let data = new FormData();
        data.append('file', file);
        let loader;
        try {
            loader = bootLoaderDialog('Uploading photo...');
            let path = await uploadPhoto(data);
            loader.hide();
            $('#profilePhoto').attr('src', $base + path);
            $('#navImage').attr('src', $base + path);
            notify('Photo uploaded successfully', 'success');

            $('#editPhotoModal').modal('hide');
        } catch (ex) {
            loader.hide();
            if (typeof (ex) == 'string') {
                notify(ex + '.', 'danger');
            }
            console.error(ex);
        }
    });

    // on edit
    $('#editBtn').on('click', async (e) => {
        let uid = studentId;
        let loader = bootLoaderDialog('Fetching student...');
        let student = null;
        try {
            student = await getStudent(uid);
            loader.hide();

            $('#e_fname').val(student.firstName);
            $('#e_mname').val(student.middleName);
            $('#e_sname').val(student.surname);
            $('#e_dob').val(student.dateOfBirth?.split('T')[0]);
            $('#e_gender').val(student.gender);
            $('#e_phone').val(student.phoneNumber);

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

    // on update
    $('#updateBtn').on('click', (e) => {
        e.preventDefault();
        let btn = $(e.currentTarget);
        let uid = studentId;
        try {
            let form = $("form")[0];
            if (validateForm(form)) {
                let firstName = $.trim($('#e_fname').val());
                let middleName = $.trim($('#e_mname').val());
                let surname = $.trim($('#e_sname').val());
                let gender = $.trim($('#e_gender').val());
                let dob = $.trim($('#e_dob').val());
                let phone = $.trim($('#e_phone').val());

                if (firstName == '' || surname == '' || gender == '' || phone == '') {
                    notify('Fields with asteriks (*) are required', 'warning');
                } else {
                    $('fieldset').prop('disabled', true);
                    btn.html('<i class="fa fa-circle-notch fa-spin"></i> Updating student...');
                    let url = $base + 'profile/UpdateStudentProfile';
                    let data = {
                        id: uid,
                        firstName,
                        middleName,
                        surname,
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

async function getCroppedImageFile() {
    var promise = new Promise((resolve, reject) => {
        cropper.getCroppedCanvas({
            maxWidth: 300
        }).toBlob(b => {
            var f = new File([b], b.type.replace('/', '.'), { type: b.type });
            resolve(f);
        });
    });
    return promise;
}

function uploadPhoto(fileData) {
    var promise = new Promise((resolve, reject) => {
        try {
            if (fileData == null || fileData == undefined) {
                reject('File data is required');
            } else {
                let url = $base + 'profile/UploadStudentPhoto';
                $.ajax({
                    type: 'POST',
                    url: url,
                    data: fileData,
                    contentType: false,
                    cache: false,
                    processData: false,
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