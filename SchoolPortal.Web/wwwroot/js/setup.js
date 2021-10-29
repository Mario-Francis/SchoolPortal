$(() => {
    // on add
    $('#setupBtn').on('click', (e) => {
        e.preventDefault();
        let btn = $(e.currentTarget);
        try {
            let form = $("form")[0];
            if (validateForm(form)) {
                let fname = $.trim($('#fname').val());
                let mname = $.trim($('#mname').val());
                let sname = $.trim($('#sname').val());
                let gender = $.trim($('#gender').val());
                let email = $.trim($('#email').val());
                let phone = $.trim($('#phone').val());
                let password = $('#password').val();
                let cpassword = $('#cpassword').val();

                if (fname == '' || sname == '' || gender == '' || email == '' || phone == '' || password == '' || cpassword=='') {
                    notify('All fields with asteriks (*) are required.', 'warning');
                } else if (!validateEmail(email)) {
                    notify('Email is invalid.', 'warning');
                } else if (!validatePassword(password) && password.length < 8) {
                    notify('Password must be up to 8 chars in length and must contain at least one uppercase, one lowercase, one number and one special character.', 'warning');
                } else if (password != cpassword) {
                    notify('Passwords does not match.', 'warning');
                }
                else {
                    $('fieldset').prop('disabled', true);
                    btn.html('<span class="fa fa-circle-notch fa-spin"></span> Settting up...');
                    let url = $base + 'auth/setup';
                    let data = {
                        firstName: fname,
                        middleName: mname,
                        surname: sname,
                        gender,
                        email,
                        PhoneNumber: phone,
                        password,
                        confirmPassword: cpassword
                    };
                    $.ajax({
                        type: 'POST',
                        url: url,
                        data: data,
                        success: (response) => {
                            if (response.isSuccess) {
                                notify(response.message + '<br /><i class="fa fa-circle-notch fa-spin"></i> Redirecting...', 'success');

                                form.reset();
                                setTimeout(() => {
                                    location.replace($base+'auth/index');
                                }, 2000);

                            } else {
                                notify(response.message, 'danger');
                            }
                            btn.html('<span class="fa fa-check-circle"></span> &nbsp;Setup');
                            $('fieldset').prop('disabled', false);
                        },
                        error: (req, status, err) => {
                            ajaxErrorHandler(req, status, err, {
                                callback: () => {
                                    btn.html('<span class="fa fa-check-circle"></span> &nbsp;Setup');
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
            btn.html('<i class="fa fa-check-circle"></i> &nbsp;Setup');
            $('fieldset').prop('disabled', false);
        }
    });

});