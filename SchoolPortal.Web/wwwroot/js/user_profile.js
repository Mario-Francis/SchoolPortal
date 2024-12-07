var userId = $('#userId').val();
var e_roleChoices;
$(() => {
    e_roleChoices = new Choices($('#e_roles')[0], {
        removeItemButton: true,
    });
    // on edit
    $('#editBtn').on('click', async (e) => {
        let uid = userId;
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


    // on update
    $('#updateBtn').on('click', (e) => {
        e.preventDefault();
        let btn = $(e.currentTarget);
        let uid = userId;
        try {
            let form = $("form")[0];
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
